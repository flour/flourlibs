namespace Flour.RabbitMQ.Implementations;

public class MessageConvention : IMessageConvention
{
    public MessageConvention(Type type, string routingKey, string exchange, string queue)
    {
        MessageType = type;
        Route = routingKey;
        Exchange = exchange;
        Queue = queue;
    }

    public Type MessageType { get; }
    public string Route { get; }
    public string Exchange { get; }
    public string Queue { get; }

    public override string ToString()
    {
        return $"Route: {Route}, Exchange: {Exchange}, Queue: {Queue}";
    }
}