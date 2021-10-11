using Serilog;

namespace Flour.Logging.Options
{
    public class SeqOptions : ILoggerOptions
    {
        public bool Enabled { get; set; }
        public string Url { get; set; }
        public string ApiKey { get; set; }

        public void Configure(LoggerConfiguration configuration)
        {
            if (Enabled && !string.IsNullOrWhiteSpace(Url) && !string.IsNullOrWhiteSpace(ApiKey))
            {
                configuration.WriteTo.Seq(Url, apiKey: ApiKey);
            }
        }
    }
}