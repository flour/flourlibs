using Flour.RabbitMQ.Options;
using System.Collections.Generic;

namespace Flour.RabbitMQ
{
    public class RabbitMqOptions
    {
        public int Port { get; set; }
        public string ClientName{ get; set; }
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public IEnumerable<string> HostNames { get; set; }
        public LoggerOptions Logger { get; set; } = new LoggerOptions();
        public QueueOptions Queue { get; set; } = new QueueOptions();
        public QosOptions QOS { get; set; } = new QosOptions();
    }
}
