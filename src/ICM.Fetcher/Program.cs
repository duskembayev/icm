using ICM.Fetcher;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddModels(builder.Configuration);

builder.Services.AddOptions<BlockcypherFetchOptions>().BindConfiguration("Blockcypher");
builder.Services.AddSingleton<BlockcypherFetchHealth>();
builder.Services.AddHostedService<BlockcypherFetchService>();
builder.Services.AddTransient<BlockcypherFetchWorker>();

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<BlockchainDbContext>()
    .AddCheck<BlockcypherFetchHealth>("BlockcypherFetch");

builder.Services.AddHttpClient(HttpClientNames.Blockcypher, (provider, client) =>
{
    var options = provider.GetRequiredService<IOptions<BlockcypherFetchOptions>>();
    client.BaseAddress = new Uri(options.Value.BaseUrl);
    client.Timeout = options.Value.RequestTimeout;
});

var app = builder.Build();

app.MapHealthChecks("/healthz");

await app.RunAsync();