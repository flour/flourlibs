using Flour.ServiceBus.Abstractions;
using MassTransit;

namespace Flour.ServiceBus.MassTransit.RabbitMq;

internal class MassTransitServiceBus : IServiceBus
{
    private readonly IBusControl _internalBus;
    private readonly IMassTransitRequestClientFactory _requestClientFactory;

    private int _disposeCount;

    public MassTransitServiceBus(
        IBusControl internalBus,
        IMassTransitRequestClientFactory requestClientFactory)
    {
        _internalBus = internalBus
                       ?? throw new ArgumentNullException(nameof(internalBus));

        _requestClientFactory = requestClientFactory
                                ?? throw new ArgumentNullException(nameof(requestClientFactory));
    }

    public async Task StartAsync(CancellationToken token = default)
    {
        await _internalBus.StartAsync(token).ConfigureAwait(false);
    }

    public Task Publish<TEvent>(object anEvent, CancellationToken token = default) where TEvent : class
    {
        return _internalBus.Publish<TEvent>(anEvent, token);
    }

    public Task Publish<TEvent>(TEvent anEvent, CancellationToken token = default) where TEvent : class
    {
        return _internalBus.Publish(anEvent, token);
    }

    public Task Publish<TEvent>(
        TEvent anEvent,
        string correlationId,
        string messageId,
        IDictionary<string, object> headers = null,
        CancellationToken token = default) where TEvent : class
    {
        return _internalBus.Publish(anEvent, ctx =>
        {
            if (Guid.TryParse(correlationId, out var corId))
                ctx.CorrelationId = corId;

            if (Guid.TryParse(messageId, out var msgId))
                ctx.MessageId = msgId;

            ctx.SetAwaitAck(true);

            if (headers is null)
                return;

            foreach (var (key, value) in headers)
                ctx.Headers.Set(key, value);
        }, token);
    }

    public async Task Publish(
        object anEvent,
        Type eventType,
        string correlationId,
        string messageId,
        IDictionary<string, object> headers = null,
        CancellationToken token = default)
    {
        await _internalBus.Publish(anEvent, eventType, ctx =>
        {
            if (Guid.TryParse(correlationId, out var corId))
                ctx.CorrelationId = corId;

            if (Guid.TryParse(messageId, out var msgId))
                ctx.MessageId = msgId;

            ctx.SetAwaitAck(true);

            if (headers is null)
                return;

            foreach (var (key, value) in headers)
                ctx.Headers.Set(key, value);
        }, token);
    }

    public async Task<TResponse> Request<TRequest, TResponse>(TRequest message, CancellationToken token = default)
        where TRequest : class where TResponse : class
    {
        var client = _requestClientFactory.Create<TRequest>(_internalBus);
        var result = await client.GetResponse<TResponse>(message, token).ConfigureAwait(false);
        return result.Message;
    }

    public void Dispose()
    {
        if (Interlocked.Increment(ref _disposeCount) != 1)
            return;
        DisposeAsync().GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        await _internalBus.StopAsync();
    }
}