namespace ICM.TestEngine.Core;

public static class HarnessExtensions
{
    public static WebApplicationFactory<TEntryPoint> AddHarness<TEntryPoint>(
        this WebApplicationFactory<TEntryPoint> factory, IHarness<TEntryPoint> harness)
        where TEntryPoint : class
    {
        return factory.WithWebHostBuilder(harness.Configure);
    }
}