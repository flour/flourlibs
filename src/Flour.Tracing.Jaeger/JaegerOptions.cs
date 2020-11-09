using System.ComponentModel;

namespace Flour.Tracing.Jaeger
{
    public enum SamplerType
    { 
        [Description("constant")]
        Constant,
        [Description("rate")]
        Rate,
        [Description("prob")]
        Probabilistic
    }

    public class JaegerOptions
    {
        public bool Enabled { get; set; }
        public string ServiceName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public int MaxPacketSize { get; set; }
        public double MaxTracesPerSecond { get; set; } = 5;
        public double SamplingRate { get; set; } = 0.5;
        public string Sampler { get; set; }
    }
}
