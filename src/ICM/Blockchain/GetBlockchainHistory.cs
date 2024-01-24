using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ICM.Blockchain;

public static class GetBlockchainHistory
{
    public record Request(string BlockchainName, int Take, int Skip) : IRequest<IReadOnlyCollection<byte[]>>
    {
        public const int DefaultSkip = 0;
        public const int DefaultTake = 10;
        public const int MaxTake = 100;
    }

    public class Endpoint : IEndpoint
    {
        public RouteHandlerBuilder Map(IEndpointRouteBuilder builder)
        {
            return builder
                .MapGet("/blockchain/{name}/history", async Task<IResult>
                ([FromRoute] string name, [FromQuery] int? take, [FromQuery] int? skip,
                    [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var data = await mediator.Send(
                        new Request(name, take ?? Request.DefaultTake, skip ?? Request.DefaultSkip), cancellationToken);

                    if (data.Count == 0)
                        return TypedResults.NotFound();

                    return TypedResults.Stream(async stream =>
                    {
                        await using var writer = new Utf8JsonWriter(stream);

                        writer.WriteStartArray();

                        foreach (var rawJson in data) writer.WriteRawValue(rawJson);

                        writer.WriteEndArray();
                    }, MediaTypeNames.Application.Json);
                })
                .WithOpenApi();
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(m => m.BlockchainName)
                .NotEmpty()
                .MaximumLength(10);

            RuleFor(m => m.Take)
                .InclusiveBetween(1, Request.MaxTake);

            RuleFor(m => m.Skip)
                .GreaterThanOrEqualTo(0);
        }
    }

    public class Handler(BlockchainDbContext dbContext) : IRequestHandler<Request, IReadOnlyCollection<byte[]>>
    {
        public async Task<IReadOnlyCollection<byte[]>> Handle(Request request, CancellationToken cancellationToken)
        {
            var name = request.BlockchainName.ToLowerInvariant();

            return await dbContext.BlockchainInfos
                .Where(m => m.Name == name)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => m.Data)
                .Skip(request.Skip)
                .Take(request.Take)
                .ToListAsync(cancellationToken);
        }
    }
}