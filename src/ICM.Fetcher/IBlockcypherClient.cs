namespace ICM.Fetcher;

public interface IBlockcypherClient
{
    Task<byte[]> DownloadAsync(string url, CancellationToken cancellationToken);
}