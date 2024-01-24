namespace ICM.Fetcher;

public class BlockcypherChainOptions
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public TimeSpan Interval { get; set; }
}