using Flour.Logging.Options;
using Microsoft.Extensions.Logging;

namespace Flour.Logging
{
    public class LoggerOptions
    {
        public LogLevel Level { get; set; }
        public ConsoleOptions Console { get; set; }
        public FileOptions File { get; set; }
        public GraylogOptions Graylog { get; set; }
    }
}
