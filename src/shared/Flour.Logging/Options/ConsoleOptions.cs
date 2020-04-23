using Serilog;

namespace Flour.Logging.Options
{
    public class ConsoleOptions : ILoggerOptions
    {
        public bool Enabled { get; set; }

        public void Configure(LoggerConfiguration configuration)
        {
            if (Enabled)
                configuration.WriteTo.Console();
        }
    }
}
