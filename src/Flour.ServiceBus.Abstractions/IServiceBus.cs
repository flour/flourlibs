namespace Flour.ServiceBus.Abstractions;

public interface IServiceBus : IDisposable, IAsyncDisposable
{
    Task StartAsync(CancellationToken token = default);

    Task Publish<TEvent>(object anEvent, CancellationToken token = default)
        where TEvent : class;

    Task Publish<TEvent>(TEvent anEvent, CancellationToken token = default)
        where TEvent : class;

    Task Publish<TEvent>(
        TEvent anEvent,
        string correlationId,
        string context,
        IDictionary<string, object> headers,
        CancellationToken token = default)
        where TEvent : class;

    Task Publish(
        object anEvent,
        Type eventType,
        string correlationId,
        string context,
        IDictionary<string, object> headers = null,
        CancellationToken token = default);

    Task<TResponse> Request<TRequest, TResponse>(TRequest message, CancellationToken token = default)
        where TRequest : class
        where TResponse : class;
}