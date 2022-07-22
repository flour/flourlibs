namespace Flour.RabbitMQ.Options;

public class ExchangeOptions
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool Declare { get; set; }
    public bool Durable { get; set; }
    public bool AutoDelete { get; set; }
}