using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VaultSharp;

namespace Flour.Vault.Services;

internal class VaultHostedService : BackgroundService
{
    private readonly IVaultClient _client;
    private readonly ILogger<VaultHostedService> _logger;
    private readonly VaultOptions _options;

    public VaultHostedService(
        IVaultClient client,
        VaultOptions options,
        ILogger<VaultHostedService> logger)
    {
        _client = client;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled || (_options.Renew?.Enabled ?? true))
            return;

        var interval = TimeSpan.FromSeconds(_options.Renew.Interval <= 0 ? 10 : _options.Renew.Interval);

        while (!stoppingToken.IsCancellationRequested)
            // Renew data
            await Task.Delay(interval, stoppingToken);
    }
}