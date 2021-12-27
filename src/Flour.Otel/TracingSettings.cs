using System.Collections.Generic;

namespace Flour.OTel
{
    public class TracingSettings
    {
        public bool Enabled { get; set; } = true;
        public string ServiceName { get; set; } = "SomeService";
        public List<KeyValuePair<string, string>> Enrichers { get; set; } = new();
        public List<KeyValuePair<string, object>> Attributes { get; set; } = new();
        public FilterSettings Filters { get; set; } = new();
        public JaegerSettings Jaeger { get; set; } = new();
    }

    public class JaegerSettings
    {
        public bool Enabled { get; set; }
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 6831;
    }

    public class FilterSettings
    {
        public bool Enabled { get; set; } = true;
        public List<string> Paths { get; set; } = new();
        public List<string> Expressions { get; set; } = new();
        public List<string> FilterExtensions { get; set; } = new() { "js", "css", "html", "jpg", "png", "svg" };
    }
}
