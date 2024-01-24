using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ICM.Fetcher;

public class BlockcypherFetchHealth : IHealthCheck
{
    private readonly ConcurrentDictionary<string, bool> _healthStatus = new();

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
        if (_healthStatus.Count == 0)
            return Task.FromResult(HealthCheckResult.Unhealthy("No chains are being monitored"));

        var unhealthyChains = _healthStatus
            .Where(x => !x.Value)
            .Select(x => x.Key)
            .ToImmutableArray();

        if (unhealthyChains.Length == 0)
            return Task.FromResult(HealthCheckResult.Healthy());

        return Task.FromResult(
            HealthCheckResult.Unhealthy($"The following chains are unhealthy: {string.Join(", ", unhealthyChains)}"));
    }

    public void ReportHealthy(string chainName)
    {
        if (!_healthStatus.TryAdd(chainName, true))
            throw new InvalidOperationException($"Chain {chainName} is already reported as healthy");
    }

    public void ReportUnhealthy(string chainName)
    {
        if (_healthStatus.TryUpdate(chainName, false, true))
            throw new InvalidOperationException($"Chain {chainName} is already reported as unhealthy");
    }
}