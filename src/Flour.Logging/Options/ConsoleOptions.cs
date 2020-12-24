using Serilog;
using Serilog.Events;

namespace Flour.Logging.Options
{
    public class ConsoleOptions : ILoggerOptions
    {
        public bool Enabled { get; set; }
        public LogEventLevel MinLevel { get; set; } = LogEventLevel.Information;

        public void Configure(LoggerConfiguration configuration)
        {
            if (!Enabled)
                return;
            configuration.WriteTo.Console(MinLevel);
        }
    }
}