namespace Flour.RabbitMQ;

public interface IMessageConvention
{
    Type MessageType { get; }
    string Queue { get; }
    string Route { get; }
    string Exchange { get; }
}