using ICM.Core.Endpoints;

namespace ICM.Blockchain;

public static class SetupExtensions
{
    public static IServiceCollection AddBlockchain(this IServiceCollection @this)
    {
        @this.AddTransient<IEndpoint, GetBlockchainInfo.Endpoint>();
        return @this;
    }
}