namespace ICM.Blockchain;

public static class SetupExtensions
{
    public static IServiceCollection AddBlockchain(this IServiceCollection @this)
    {
        @this.AddTransient<IEndpoint, GetBlockchainInfo.Endpoint>();
        @this.AddTransient<IEndpoint, GetBlockchainHistory.Endpoint>();

        return @this;
    }
}