using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;

namespace Flour.Logging
{
    public static class Extensions
    {
        public static IWebHostBuilder UseLogger(
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

        public static TModel GetOptions<TModel>(this IConfiguration configuration, string sectionName)
            where TModel : new()
        {
            var model = new TModel();
            configuration.GetSection(sectionName).Bind(model);
            return model;
        }

        private static void ConfigureAll(this LoggerOptions loggerOptions, LoggerConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("Logger configuration coonot by null", nameof(configuration));

            loggerOptions.Console?.Configure(configuration);
            loggerOptions.File?.Configure(configuration);
        }
    }
}
