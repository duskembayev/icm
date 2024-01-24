using System.Reflection;
using FluentValidation;
using ICM.Blockchain;
using ICM.Core.Endpoints;

var thisAssembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddModels(builder.Configuration);
builder.Services.AddBlockchain();

builder.Services.AddValidatorsFromAssembly(thisAssembly);
builder.Services.AddMediatR(c =>
{
    c.RegisterServicesFromAssembly(thisAssembly);
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

app.MapHealthChecks("/healthz");
app.MapEndpoints();

await app.Services.RestoreDatabaseAsync();
await app.RunAsync();
