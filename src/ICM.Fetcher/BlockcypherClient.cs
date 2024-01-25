namespace ICM.Fetcher;

public class BlockcypherClient : IBlockcypherClient
{
    private readonly HttpClient _httpClient;

    public BlockcypherClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientNames.Blockcypher);
    }
    
    public async Task<byte[]> DownloadAsync(string url, CancellationToken cancellationToken)
    {
        return await _httpClient.GetByteArrayAsync(url, cancellationToken);
    }
}