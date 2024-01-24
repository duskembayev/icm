namespace ICM.Fetcher;

public class BlockcypherFetchOptions
{
    public TimeSpan StartupDelay { get; set; }
    public TimeSpan RequestTimeout { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    public IReadOnlyCollection<BlockcypherChainOptions> Chains { get; init; } = new List<BlockcypherChainOptions>();
}