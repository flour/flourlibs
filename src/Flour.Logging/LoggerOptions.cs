using Flour.Logging.Options;
using Serilog.Events;
using FileOptions = Flour.Logging.Options.FileOptions;

namespace Flour.Logging;

public class LoggerOptions
{
    public LogEventLevel MinLevel { get; set; } = LogEventLevel.Debug;
    public Dictionary<string, string> Labels { get; set; }
    public FilteringOptions Filters { get; set; }
    public ConsoleOptions Console { get; set; }
    public FileOptions File { get; set; }
    public ElasticOptions ElasticSearch { get; set; }
    public SeqOptions Seq { get; set; }
    public OtelOptions Otel { get; set; }
}