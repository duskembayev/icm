namespace ICM.TestEngine.Core;

public interface IHarness<TEntryPoint> where TEntryPoint : class
{
    void Configure(IWebHostBuilder builder);

    Task StartAsync(WebApplicationFactory<TEntryPoint> factory, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}