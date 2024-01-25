using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ICM.TestEngine;

public class DatabaseHarness<TEntryPoint, TDbContext> : Harness<TEntryPoint>
    where TEntryPoint : class
    where TDbContext : DbContext
{
    private readonly PostgresHarness<TEntryPoint> _postgresHarness;

    public DatabaseHarness(PostgresHarness<TEntryPoint> postgresHarness)
    {
        _postgresHarness = postgresHarness;
    }

    public async Task ExecuteAsync(Func<TDbContext, Task> action)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await action(dbContext);
    }

    public async Task<T> ExecuteAsync<T>(Func<TDbContext, Task<T>> action)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        return await action(dbContext);
    }

    protected override void OnConfigure(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"Database:RestoreEnabled", "false"}
            });
        });
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        _postgresHarness.ThrowIfNotStarted();

        await using var scope = Factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}