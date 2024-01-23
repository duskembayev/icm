namespace ICM.Models;

public class BlockchainInfo
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public byte[] Data { get; init; } = Array.Empty<byte>();
    public DateTimeOffset CreatedAt { get; init; }
}