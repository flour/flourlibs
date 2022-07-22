namespace Flour.RabbitMQ.Options;

public class QueueOptions
{
    internal static readonly string DefaultTemplate = "%assembly%/%exchange%.%message%";

    public bool Declare { get; set; } = true;
    public bool Durable { get; set; } = true;
    public bool Exclusive { get; set; }
    public bool AutoDelete { get; set; }
    public string QueueTemplate { get; set; } = DefaultTemplate;
}