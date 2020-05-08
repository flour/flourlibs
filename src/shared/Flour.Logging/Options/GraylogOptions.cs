using Serilog;
using Serilog.Sinks.Graylog;
using System;

namespace Flour.Logging.Options
{
    public class GraylogOptions : ILoggerOptions
    {
        public bool Enabled { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        // TODO more options

        public int MyProperty { get; set; }

        public void Configure(LoggerConfiguration configuration)
        {
            if (!Enabled)
                return;
            var host = Host ?? throw new ArgumentException("Host cannot be null", nameof(Host));

            // Default and only protocol is UDP (for now)
            // Read more at https://docs.graylog.org and https://github.com/whir1/serilog-sinks-graylog

            configuration.WriteTo.Graylog(new GraylogSinkOptions
            {
                HostnameOrAddress = host,
                Port = Port
            });
        }
    }
}
