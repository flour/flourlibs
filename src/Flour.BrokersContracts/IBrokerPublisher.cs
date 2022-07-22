namespace Flour.BrokersContracts;

public interface IBrokerPublisher
{
    Task Publish<T>(
        T message,
        string correlationId = null,
        string messageId = null,
        string context = null,
        IDictionary<string, object> headers = null) where T : class;
}