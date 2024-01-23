namespace ICM.Models;

public record BlockchainDbOptions
{
    public bool RestoreEnabled { get; init; }
    public TimeSpan RestoreTimeout { get; init; }
}