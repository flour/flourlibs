using Microsoft.Extensions.Logging;
using Serilog;

namespace Flour.Logging.Options
{
    public class ConsoleOptions : ILoggerOptions
    {
        public bool Enabled { get; set; }
        public LogLevel MinLevel { get; set; } = LogLevel.Information;

        public void Configure(LoggerConfiguration configuration)
        {
            if (Enabled)
                configuration.WriteTo.Console();
        }
    }
}
