using System.Net;

namespace ICM.Fetcher;

public class BlockcypherFetchWorker
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly BlockchainDbContext _dbContext;
    private readonly ILogger<BlockcypherFetchWorker> _logger;

    public BlockcypherFetchWorker(IHttpClientFactory httpClientFactory, BlockchainDbContext dbContext, ILogger<BlockcypherFetchWorker> logger)
    {
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task RunAsync(BlockcypherChainOptions options, CancellationToken cancellationToken)
    {
        using var _ = _logger.BeginScope("Chain: {ChainName}", options.Name);
        var httpClient = _httpClientFactory.CreateClient(HttpClientNames.Blockcypher);

        _logger.LogInformation("Starting fetch loop for chain {ChainName}", options.Name);
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteCore(options.Name, options.Url, httpClient, cancellationToken);
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _logger.LogWarning(e, "Too many requests for chain {ChainName}", options.Name);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unhandled exception in fetch loop for chain {ChainName}", options.Name);
                throw;
            }

            await Task.Delay(options.Interval * 2, cancellationToken);
        }
        
        _logger.LogInformation("Fetch loop for chain {ChainName} has been cancelled", options.Name);
    }

    private async ValueTask ExecuteCore(string name, string url, HttpClient httpClient, CancellationToken cancellationToken)
    {
        var data = await httpClient.GetByteArrayAsync(url, cancellationToken);
        _logger.LogDebug("Fetched data for chain {ChainName}: {Bytes} bytes", name, data.Length);

        _dbContext.BlockchainInfos.Add(new BlockchainInfo
        {
            Name = name,
            Data = data,
            CreatedAt = TimeProvider.System.GetUtcNow()
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Saved data for chain {ChainName}: {Bytes} bytes", name, data.Length);
    }
}