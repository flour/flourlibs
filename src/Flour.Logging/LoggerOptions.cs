using System.Collections.Generic;
using Flour.Logging.Options;

namespace Flour.Logging
{
    // TODO: split by assembly
    public class LoggerOptions
    {
        public Dictionary<string, string> Labels { get; set; }
        public FilteringOptions Filters { get; set; }
        public ConsoleOptions Console { get; set; }
        public FileOptions File { get; set; }
        public ElasticOptions ElasticSearch { get; set; }
        public LokiOptions Loki { get; set; }
    }
}