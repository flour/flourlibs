using Flour.Logging.Options;

namespace Flour.Logging
{
    // TODO: spliti by assembly
    public class LoggerOptions
    {
        public ConsoleOptions Console { get; set; }
        public FileOptions File { get; set; }
        public ElasticOptions ElasticSearch { get; set; }
        public GraylogOptions Graylog { get; set; }
        public SeqOptions Seq { get; set; }
    }
}
