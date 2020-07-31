using Flour.Commons;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using System;

namespace Flour.Logging
{
    public static class Extensions
    {
        public static IWebHostBuilder UseLogging(
            this IWebHostBuilder hostBuilder,
            string sectionName = "logger")
            => hostBuilder.UseSerilog((ctx, configuration) =>
            {
                if (string.IsNullOrWhiteSpace(sectionName))
                    throw new ArgumentException("Section name cannot be null or whitespace", nameof(sectionName));

                var loggerOptions = ctx.Configuration
                    .GetOptions<LoggerOptions>(sectionName);

                if (loggerOptions == null)
                    return;

                loggerOptions.ConfigureAll(configuration);
            });

        private static void ConfigureAll(this LoggerOptions loggerOptions, LoggerConfiguration configuration)
        {
            var options = loggerOptions ?? throw new ArgumentNullException("Logger options cannot by null", nameof(loggerOptions));
            var config = configuration ?? throw new ArgumentNullException("Logger configuration cannot by null", nameof(configuration));

            options.Console?.Configure(config);
            options.File?.Configure(config);
            options.Graylog?.Configure(config);
            options.Seq?.Configure(config);
        }
    }
}
