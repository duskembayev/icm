using System.Net.Mime;
using FluentValidation;
using ICM.Core.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ICM.Blockchain;

public static class GetBlockchainInfo
{
    public record Request(string Slug) : IRequest<byte[]?>;

    public class Endpoint : IEndpoint
    {
        public RouteHandlerBuilder Map(IEndpointRouteBuilder builder)
        {
            return builder
                .MapGet("/blockchain/{slug}", async Task<Results<IResult, NotFound>>
                    ([FromRoute] string slug, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var data = await mediator.Send(new Request(slug), cancellationToken);

                    return data is null
                        ? TypedResults.NotFound()
                        : TypedResults.Bytes(data, MediaTypeNames.Application.Json);
                })
                .WithOpenApi();
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(m => m.Slug)
                .NotEmpty()
                .MaximumLength(10);
        }
    }

    public class Handler(BlockchainDbContext dbContext) : IRequestHandler<Request, byte[]?>
    {
        public async Task<byte[]?> Handle(Request request, CancellationToken cancellationToken)
        {
            var slug = request.Slug.ToLowerInvariant();

            return await dbContext.BlockchainInfos
                .Where(m => m.Name == slug)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => m.Data)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}