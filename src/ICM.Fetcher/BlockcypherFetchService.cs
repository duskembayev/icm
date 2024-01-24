using System.Collections.Immutable;

namespace ICM.Fetcher;

public class BlockcypherFetchService : IHostedService
{
    private readonly IOptions<BlockcypherFetchOptions> _options;
    private readonly BlockcypherFetchHealth _health;
    private readonly IServiceProvider _serviceProvider;
    private readonly CancellationTokenSource _cts;
    private readonly TaskFactory _taskFactory;
    private ImmutableArray<Task<Task>> _fetchTasks;

    public BlockcypherFetchService(IOptions<BlockcypherFetchOptions> options, BlockcypherFetchHealth health,
        IServiceProvider serviceProvider)
    {
        _options = options;
        _health = health;
        _serviceProvider = serviceProvider;
        _cts = new CancellationTokenSource();
        _taskFactory = new TaskFactory(_cts.Token);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_options.Value.StartupDelay > TimeSpan.Zero)
            await Task.Delay(_options.Value.StartupDelay, cancellationToken);

        _fetchTasks = _options.Value.Chains
            .Select(chain =>
                _taskFactory.StartNew(async () => await RunWorkerAsync(chain, _cts.Token), cancellationToken))
            .ToImmutableArray();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        return Task.WhenAll(_fetchTasks);
    }

    private async Task RunWorkerAsync(BlockcypherChainOptions chain, CancellationToken cancellationToken)
    {
        _health.ReportHealthy(chain.Name);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var worker = scope.ServiceProvider.GetRequiredService<BlockcypherFetchWorker>();
            await worker.RunAsync(chain, cancellationToken);
        }
        finally
        {
            _health.ReportUnhealthy(chain.Name);
        }
    }
}