using System.Collections.Generic;

namespace Flour.Tracing.Jaeger
{
    public class JaegerOptions
    {
        public bool Enabled { get; set; }
        public string ServiceName { get; set; }
        public UdpSettings Udp { get; set; }
        public HttpSettings Http { get; set; }
        public int MaxPacketSize { get; set; } = 64967;
        public double MaxTracesPerSecond { get; set; } = 5;
        public double SamplingRate { get; set; } = 0.5;
        public string Sampler { get; set; }
        public IEnumerable<string> ExcludePaths { get; set; }
    }

    public class UdpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; } = 6831;
    }

    public class HttpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; } = 14268;
        public string Token { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Ua { get; set; }
        public int MaxPacketSize { get; set; } = 1048576;
    }
}