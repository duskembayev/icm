using System.Reflection;
using ICM.Blockchain;
using ICM.Core.MediatrBehaviors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var thisAssembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddModels(builder.Configuration);
builder.Services.AddBlockchain();

builder.Services.AddProblemDetails();
builder.Services.AddValidatorsFromAssembly(thisAssembly);
builder.Services.AddMediatR(c =>
{
    c.RegisterServicesFromAssembly(thisAssembly);
    c.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<BlockchainDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(exBuilder => exBuilder.Run(HandleExceptionAsync));
app.MapHealthChecks("/healthz");
app.MapEndpoints();

await app.Services.RestoreDatabaseAsync();
await app.RunAsync();

static Task HandleExceptionAsync(HttpContext httpContext)
{
    var exception = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
    var problemDetails = exception switch
    {
        ValidationException validationException => new ProblemDetails
        {
            Title = "Validation failed",
            Detail = validationException.Message,
            Status = 400,
            Type = "https://httpstatuses.com/400"
        },
        _ => new ProblemDetails
        {
            Title = exception?.GetType().Name,
            Detail = exception?.Message,
            Status = 500,
            Type = "https://httpstatuses.com/500"
        }
    };

    return httpContext.Response.WriteAsJsonAsync(problemDetails);
}