using System.Diagnostics;
using Flour.BrokersContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flour.IOBox;

internal class OutboxProcessor : IHostedService
{
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly IOptions<InOutSettings> _options;
    private readonly IBrokerPublisher _publisher;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private Timer _timer;

    public OutboxProcessor(
        IServiceScopeFactory serviceScopeFactory,
        IBrokerPublisher publisher,
        IOptions<InOutSettings> options,
        ILogger<OutboxProcessor> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _publisher = publisher;
        _options = options;
        _logger = logger;

        if (_options.Value.Enabled)
            _logger.LogInformation($"Outbox enable with check interval of {_options.Value.CheckInterval}ms");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.Value?.Enabled ?? false)
            return Task.CompletedTask;

        _timer = new Timer(SendOutboxMessages, null, TimeSpan.Zero,
            TimeSpan.FromMilliseconds(_options.Value?.CheckInterval ?? 500));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private void SendOutboxMessages(object state)
    {
        _ = SendOutboxMessagesAsync();
    }

    private async Task SendOutboxMessagesAsync()
    {
        _logger.LogTrace("Start processing outbox messages");
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        using var scope = _serviceScopeFactory.CreateScope();
        var outbox = scope.ServiceProvider.GetRequiredService<IOutboxMessageAccessor>();
        var messages = await outbox.GetUnsentAsync().ConfigureAwait(false);
        if (!messages.Any())
        {
            _logger.LogTrace("No outbox message");
            return;
        }

        // sequential
        foreach (var message in messages.OrderBy(e => e.SentAt))
        {
            await _publisher.Publish(
                    message.Message, message.CorrelationId, message.Id, message.Context, message.Headers)
                .ConfigureAwait(false);

            await outbox.ProcessAsync(message).ConfigureAwait(false);
        }

        // parallel
        // await outbox.ProcessAsync(messages).ConfigureAwait(false);

        stopwatch.Stop();
        _logger.LogTrace($"Processed {messages.Count} outbox messages in {stopwatch.ElapsedMilliseconds}ms");
    }
}