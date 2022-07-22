namespace Flour.BrokersContracts;

[AttributeUsage(AttributeTargets.Class)]
public class MessagingAttribute : Attribute
{
    public MessagingAttribute(string exchange = null, string routingKey = null, string queue = null)
    {
        Exchange = string.IsNullOrEmpty(exchange) ? null : exchange;
        Route = string.IsNullOrEmpty(routingKey) ? null : routingKey;
        Queue = string.IsNullOrEmpty(queue) ? null : queue;
    }

    public string Exchange { get; }
    public string Route { get; }
    public string Queue { get; }
}