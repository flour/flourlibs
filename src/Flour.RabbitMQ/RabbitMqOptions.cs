using Flour.RabbitMQ.Options;

namespace Flour.RabbitMQ;

public class RabbitMqOptions
{
    public bool UseBackgroundThreadsForIo { get; set; }
    public int Port { get; set; }
    public string ClientName { get; set; }
    public string VirtualHost { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public List<string> HostNames { get; set; }
    public bool PersistedMessages { get; set; }
    public int Retries { get; set; }
    public int RetryInterval { get; set; }
    public int MaxProducerChannels { get; set; }
    public string SpanContextHeader { get; set; } = "span_context";
    public ushort RequestedChannelMax { get; set; }
    public uint RequestedFrameMax { get; set; }
    public int RequestedHeartbeat { get; set; } = 60;
    public int RequestedConnectionTimeout { get; set; } = 30;
    public int SocketReadTimeout { get; set; } = 30;
    public int SocketWriteTimeout { get; set; } = 30;
    public int ContinuationTimeout { get; set; } = 20;
    public int HandshakeContinuationTimeout { get; set; } = 10;
    public int NetworkRecoveryInterval { get; set; } = 5;
    public LoggerOptions Logger { get; set; } = new();
    public QueueOptions Queue { get; set; } = new();
    public QosOptions Qos { get; set; } = new();
    public ExchangeOptions Exchange { get; set; } = new();
}