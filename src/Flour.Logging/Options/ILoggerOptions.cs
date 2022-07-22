using Serilog;

namespace Flour.Logging.Options;

public interface ILoggerOptions
{
    bool Enabled { get; set; }
    void Configure(LoggerConfiguration configuration);
}