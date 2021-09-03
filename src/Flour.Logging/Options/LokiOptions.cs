using System;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

namespace Flour.Logging.Options
{
    public class LokiOptions : ILoggerOptions
    {
        public bool Enabled { get; set; }
        public bool UseCredentials { get; set; }
        public string Url { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public LogEventLevel MinLevel { get; set; } = LogEventLevel.Information;

        public void Configure(LoggerConfiguration configuration)
        {
            if (!Enabled)
                return;

            var credentials = UseCredentials ? new LokiCredentials {Login = Login, Password = Password} : null;
            configuration.WriteTo.GrafanaLoki(
                Url,
                credentials: credentials,
                restrictedToMinimumLevel: MinLevel,
                period: TimeSpan.FromMilliseconds(1000),
                textFormatter: new LokiJsonTextFormatter()
            );
        }
    }
}