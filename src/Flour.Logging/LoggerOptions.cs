using Flour.Logging.Options;

namespace Flour.Logging
{
    // TODO: split by assembly
    public class LoggerOptions
    {
        public ConsoleOptions Console { get; set; }
        public FileOptions File { get; set; }
        public ElasticOptions ElasticSearch { get; set; }
        public LokiOptions Loki { get; set; }
    }
}
