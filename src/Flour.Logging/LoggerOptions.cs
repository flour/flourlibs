using System.Collections.Generic;
using Flour.Logging.Options;
using Serilog.Events;

namespace Flour.Logging
{
    public class LoggerOptions
    {
        public LogEventLevel MinLevel { get; set; } = LogEventLevel.Debug;
        public Dictionary<string, string> Labels { get; set; }
        public FilteringOptions Filters { get; set; }
        public ConsoleOptions Console { get; set; }
        public FileOptions File { get; set; }
        public ElasticOptions ElasticSearch { get; set; }
        public LokiOptions Loki { get; set; }
        public SeqOptions Seq { get; set; }
    }
}