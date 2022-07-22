namespace Flour.RabbitMQ;

public interface IRabbitMqClient
{
    void Send(
        object message,
        IMessageConvention convention,
        string messageId = null,
        string correlationId = null,
        string spanContext = null,
        IDictionary<string, object> headers = null);
}