namespace ICM.Core.MediatrBehaviors;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validator is not null)
            await validator.ValidateAsync(request, strategy => strategy.ThrowOnFailures(), cancellationToken);

        return await next();
    }
}