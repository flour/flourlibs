using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;

namespace Flour.Logging.Options;

public class OtelOptions
{
    public bool Enabled { get; set; }
    public LogEventLevel MinLevel { get; set; } = LogEventLevel.Information;
    public OtlpProtocol Protocol { get; set; } = OtlpProtocol.Grpc;
    public string Endpoint { get; set; }

    public Dictionary<string, object> Attributes { get; set; } = [];

    public void Configure(LoggerConfiguration configuration)
    {
        if (Enabled && !string.IsNullOrWhiteSpace(Endpoint) && !string.IsNullOrWhiteSpace(Endpoint))
            configuration.WriteTo.OpenTelemetry(options =>
            {
                options.RestrictedToMinimumLevel = MinLevel;
                options.Endpoint = Endpoint;
                options.Protocol = Protocol;

                if (Attributes.Count > 0)
                    options.ResourceAttributes = Attributes;
            });
    }
}