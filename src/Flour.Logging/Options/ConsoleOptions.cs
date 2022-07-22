using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Elasticsearch;

namespace Flour.Logging.Options;

public class ConsoleOptions : ILoggerOptions
{
    public string Format { get; set; }
    public LogEventLevel MinLevel { get; set; } = LogEventLevel.Information;
    public bool Enabled { get; set; }

    public void Configure(LoggerConfiguration configuration)
    {
        if (!Enabled)
            return;

        switch (Format)
        {
            case "json":
                configuration.WriteTo.Console(new CompactJsonFormatter(), MinLevel);
                break;
            case "elastic":
                configuration.WriteTo.Console(new ElasticsearchJsonFormatter(), MinLevel);
                break;
            default:
                configuration.WriteTo.Console(MinLevel);
                break;
        }
    }
}