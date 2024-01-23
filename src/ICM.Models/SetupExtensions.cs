using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ICM.Models;

public static class SetupExtensions
{
    public static IServiceCollection AddModels(this IServiceCollection @this, IConfiguration configuration)
    {
        @this.AddDbContext<BlockchainDbContext>(builder =>
        {
            builder.UseNpgsql(configuration.GetConnectionString(BlockchainDbConstants.ConnectionStringName));
        });
        @this.AddOptions<BlockchainDbOptions>().BindConfiguration(BlockchainDbConstants.ConfigSectionPath);

        return @this;
    }

    public static async Task RestoreDatabaseAsync(this IServiceProvider @this)
    {
        using var scope = @this.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BlockchainDbContext>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<BlockchainDbOptions>>();

        if (!options.Value.RestoreEnabled)
            return;

        if (options.Value.RestoreTimeout > TimeSpan.Zero)
            await Task.Delay(options.Value.RestoreTimeout);

        await dbContext.Database.EnsureCreatedAsync();
        await dbContext.Database.MigrateAsync();
    }
}