namespace ICM.Core.Endpoints;

public interface IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder builder);
}