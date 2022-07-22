using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace Flour.Logging.Options;

public class ElasticOptions : ILoggerOptions
{
    public bool BasicAuthEnabled { get; set; }
    public string Url { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string IndexFormat { get; set; }
    public LogEventLevel MinLevel { get; set; } = LogEventLevel.Information;
    public bool Enabled { get; set; }

    public void Configure(LoggerConfiguration configuration)
    {
        if (!Enabled)
            return;

        configuration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(Url))
        {
            MinimumLogEventLevel = MinLevel,
            AutoRegisterTemplate = true,
            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
            IndexFormat = string.IsNullOrWhiteSpace(IndexFormat)
                ? "logstash-{0:yyyy.MM.dd}"
                : IndexFormat,
            ModifyConnectionSettings = connectionConfiguration =>
                BasicAuthEnabled
                    ? connectionConfiguration.BasicAuthentication(Username, Password)
                    : connectionConfiguration
        });
    }
}