namespace Flour.OTel;

public class TracingSettings
{
    public bool Enabled { get; set; } = true;
    public bool BaggageAsTags { get; set; } = true;
    public bool GrpcEnabled { get; set; }
    public bool MassTransitEnabled { get; set; }
    public bool EfCoreEnabled { get; set; }
    public bool RedisEnabled { get; set; }
    public bool UseConsoleExporter { get; set; }
    public string ServiceName { get; set; } = "SomeService";
    public JaegerSettings Jaeger { get; set; } = new();
    public FilterSettings Filters { get; set; } = new();

    public List<KeyValuePair<string, string>> Enrichers { get; set; } = new();
    public List<KeyValuePair<string, object>> Attributes { get; set; } = new();
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

    public List<string> FilterExtensions { get; set; } =
        new() { "js", "css", "html", "jpg", "png", "svg", "metrics", "healthz" };
}