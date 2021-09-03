﻿using System;
using Flour.Logging.Enrichers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;

namespace Flour.Logging
{
    public static class TypesRegistration
    {
        private const string DefaultSectionName = "logger";

        public static IWebHostBuilder UseWebHostLogging(
            this IWebHostBuilder hostBuilder,
            string sectionName = DefaultSectionName)
            => hostBuilder.UseSerilog((ctx, config) => ConfigureWebHostLogger(ctx, config, sectionName));

        public static IHostBuilder UseLogging(
            this IHostBuilder hostBuilder,
            string sectionName = DefaultSectionName)
            => hostBuilder.UseSerilog((ctx, config) => ConfigureHostLogger(ctx, config, sectionName));

        private static void ConfigureWebHostLogger(WebHostBuilderContext ctx, LoggerConfiguration configuration,
            string sectionName)
            => ConfigureLogger(ctx.Configuration, configuration, sectionName);

        private static void ConfigureHostLogger(HostBuilderContext ctx, LoggerConfiguration configuration,
            string sectionName)
            => ConfigureLogger(ctx.Configuration, configuration, sectionName);

        private static void ConfigureLogger(IConfiguration appConfiguration, LoggerConfiguration configuration,
            string sectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
                throw new ArgumentException("Section name cannot be null or whitespace", nameof(sectionName));

            var options = new LoggerOptions();
            appConfiguration.GetSection(sectionName).Bind(options);
            options.ConfigureAllSinks(configuration);
        }

        private static void ConfigureAllSinks(this LoggerOptions loggerOptions, LoggerConfiguration configuration)
        {
            var options = loggerOptions ??
                          throw new ArgumentNullException(nameof(loggerOptions), @"Logger options cannot by null");
            var config = configuration ??
                         throw new ArgumentNullException(nameof(configuration), @"Logger configuration cannot by null");

            config.Enrich.WithExceptionDetails();
            config.Enrich.With<LogLevelEnricher>();

            options.Filters?.Configure(config);
            options.Console?.Configure(config);
            options.File?.Configure(config);
            options.ElasticSearch?.Configure(config);
            options.Loki?.Configure(config);

            if (options.Labels is null)
                return;

            foreach (var (key, value) in options.Labels)
                configuration.Enrich.WithProperty(key, value);
        }
    }
}