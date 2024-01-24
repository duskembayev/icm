namespace ICM.Core.Endpoints;

public static class EndpointExtensions
{
    public static void MapEndpoints(this WebApplication @this)
    {
        var endpoints = @this.Services.GetServices<IEndpoint>();

        foreach (var endpoint in endpoints)
        {
            endpoint.Map(@this);
        }
    }
}