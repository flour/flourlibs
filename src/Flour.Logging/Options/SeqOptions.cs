using Serilog;
using Serilog.Events;

namespace Flour.Logging.Options
{
    public class SeqOptions : ILoggerOptions
    {
        public bool Enabled { get; set; }
        public string ServerUrl { get; set; }
        public string ApiKey { get; set; }
        public LogEventLevel MinLevel { get; set; } = LogEventLevel.Information;

        public void Configure(LoggerConfiguration configuration)
        {
            if (!Enabled)
                return;

            configuration.WriteTo.Seq(ServerUrl, apiKey: ApiKey, restrictedToMinimumLevel: MinLevel);
        }
    }
}
