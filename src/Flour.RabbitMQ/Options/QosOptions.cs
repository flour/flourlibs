namespace Flour.RabbitMQ.Options
{
    public class QosOptions
    {
        public bool IsGlobal { get; set; }
        public uint PrefetchSize { get; set; }
        public ushort PrefetchCount { get; set; } = 1;      
    }
}
