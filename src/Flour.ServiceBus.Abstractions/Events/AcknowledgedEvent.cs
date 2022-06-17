namespace Flour.ServiceBus.Abstractions.Events;

public class AcknowledgedEvent
{
    public string MessageId { get; set; }
    public DateTime Stamp { get; set; }

    public static AcknowledgedEvent Ok(string id)
    {
        return new() {Stamp = DateTime.UtcNow, MessageId = id};
    }
}