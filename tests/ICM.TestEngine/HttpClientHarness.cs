namespace ICM.TestEngine;

public class HttpClientHarness<TEntryPoint> : Harness<TEntryPoint> where TEntryPoint : class
{
    public HttpClient CreateClient()
    {
        return Factory.CreateClient();
    }
}