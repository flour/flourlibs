namespace Flour.ServiceBus.MassTransit.RabbitMq;

public class RabbitMqSettings
{
    public const int DefaultNumberOfRetries = 20;
    public const int DefaultRetryIncrement = 60;

    public bool AutoReadConsumers { get; set; } = true;
    public int Retries { get; set; } = DefaultNumberOfRetries;
    public int RetryInSeconds { get; set; } = 60;
    public int RetryIncrement { get; set; } = DefaultRetryIncrement;
    public string Host { get; set; }
    public string Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}