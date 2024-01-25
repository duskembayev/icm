using System.Net;

namespace ICM.Fetcher;

public class BlockcypherFetchWorker
{
    private readonly BlockchainDbContext _dbContext;
    private readonly IBlockcypherClient _client;
    private readonly ILogger<BlockcypherFetchWorker> _logger;

    public BlockcypherFetchWorker(BlockchainDbContext dbContext, IBlockcypherClient client,
        ILogger<BlockcypherFetchWorker> logger)
    {
        _dbContext = dbContext;
        _client = client;
        _logger = logger;
    }

    public async Task RunAsync(BlockcypherChainOptions options, CancellationToken cancellationToken)
    {
        using var _ = _logger.BeginScope("Chain: {ChainName}", options.Name);

        _logger.LogInformation("Starting fetch loop for chain {ChainName}", options.Name);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteCore(options.Name, options.Url, cancellationToken);
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

            await Task.Delay(options.Interval, cancellationToken);
        }

        _logger.LogInformation("Fetch loop for chain {ChainName} has been cancelled", options.Name);
    }

    private async ValueTask ExecuteCore(string name, string url, CancellationToken cancellationToken)
    {
        var timestamp = TimeProvider.System.GetUtcNow();
        var data = await _client.DownloadAsync(url, cancellationToken);

        _logger.LogDebug("Fetched data for chain {ChainName}: {Bytes} bytes", name, data.Length);

        _dbContext.BlockchainInfos.Add(new BlockchainInfo
        {
            Name = name,
            Data = data,
            CreatedAt = timestamp
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Saved data for chain {ChainName}: {Bytes} bytes", name, data.Length);
    }
}