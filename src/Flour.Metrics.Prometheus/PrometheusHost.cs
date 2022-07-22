using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus.DotNetRuntime;

namespace Flour.Metrics.Prometheus;

internal class PrometheusHost : IHostedService
{
    private readonly bool _enabled;
    private IDisposable _collector;

    public PrometheusHost(IOptions<PrometheusOptions> options, ILogger<PrometheusHost> logger)
    {
        _enabled = options?.Value.Enabled ?? false;

        logger.LogInformation($"Prometheus integration is {(_enabled ? "enabled" : "disabled")}.");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_enabled)
            _collector = DotNetRuntimeStatsBuilder
                .Customize()
                .WithContentionStats()
                .WithJitStats()
                .WithThreadPoolStats()
                .WithGcStats()
                .WithExceptionStats()
                .StartCollecting();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _collector?.Dispose();
        return Task.CompletedTask;
    }
}