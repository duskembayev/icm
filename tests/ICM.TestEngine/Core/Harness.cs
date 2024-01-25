using System.Diagnostics.CodeAnalysis;

namespace ICM.TestEngine.Core;

public abstract class Harness<TEntryPoint> : IHarness<TEntryPoint> where TEntryPoint : class
{
    private WebApplicationFactory<TEntryPoint>? _factory;
    private bool _started;

    protected WebApplicationFactory<TEntryPoint> Factory
    {
        get
        {
            ThrowIfNotStarted();
            return _factory;
        }
    }

    void IHarness<TEntryPoint>.Configure(IWebHostBuilder builder)
    {
        OnConfigure(builder);
    }

    Task IHarness<TEntryPoint>.StartAsync(WebApplicationFactory<TEntryPoint> factory,
        CancellationToken cancellationToken)
    {
        _factory = factory;
        _started = true;
        return OnStartAsync(cancellationToken);
    }

    Task IHarness<TEntryPoint>.StopAsync(CancellationToken cancellationToken)
    {
        _started = false;
        return OnStopAsync(cancellationToken);
    }

    protected virtual void OnConfigure(IWebHostBuilder builder)
    {
    }

    protected virtual Task OnStartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task OnStopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    [MemberNotNull(nameof(_factory))]
    public void ThrowIfNotStarted()
    {
        if (!_started)
            throw new InvalidOperationException(
                $"Harness is not started. Call {nameof(IHarness<TEntryPoint>.StartAsync)} first.");

        Assert.NotNull(_factory);
    }
}