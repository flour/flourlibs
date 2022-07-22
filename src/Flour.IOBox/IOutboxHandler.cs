namespace Flour.IOBox;

public interface IOutboxHandler
{
    bool Enabled { get; }

    Task HandleAsync(string messageId, Func<Task> handler);

    Task SendAsync<T>(
        T message,
        string outboxId = null,
        string correlationId = null,
        string messageId = null,
        string messageContext = null,
        IDictionary<string, object> headers = null)
        where T : class;
}