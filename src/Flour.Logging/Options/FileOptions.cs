using Serilog;
using Serilog.Events;
using System;

namespace Flour.Logging.Options
{
    public class FileOptions : ILoggerOptions
    {
        public bool Enabled { get; set; }
        public bool RollOnFileSizeLimit { get; set; } = true;
        public long LogSize { get; set; } = 1073741824;
        public string LogPath { get; set; } = "logs/log.txt";
        public string FileRollingInterval { get; set; }
        public LogEventLevel MinLevel { get; set; } = LogEventLevel.Information;

        public void Configure(LoggerConfiguration configuration)
        {
            if (!Enabled)
                return;

            if (!Enum.TryParse<RollingInterval>(FileRollingInterval, true, out var interval))
                interval = RollingInterval.Day;

            configuration.WriteTo.File(
                LogPath,
                fileSizeLimitBytes: LogSize,
                rollOnFileSizeLimit: RollOnFileSizeLimit,
                rollingInterval: interval,
                restrictedToMinimumLevel: MinLevel
            );
        }
    }
}
