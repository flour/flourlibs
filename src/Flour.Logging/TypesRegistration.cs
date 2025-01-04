using Flour.Logging.Enrichers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Configuration;
using Serilog.Exceptions;

namespace Flour.Logging;

public static class TypesRegistration
{
    private const string DefaultSectionName = "logger";

    public static IHostBuilder UseLogging(
        this IHostBuilder hostBuilder,
        string sectionName = DefaultSectionName,
        Action<LoggerEnrichmentConfiguration> enrich = null)
    {
        return hostBuilder.UseSerilog((ctx, config) => ConfigureLogger(ctx.Configuration, config, sectionName, enrich));
    }

    private static void ConfigureLogger(
        IConfiguration appConfiguration,
        LoggerConfiguration configuration,
        string sectionName,
        Action<LoggerEnrichmentConfiguration> enrich = null)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
            throw new ArgumentException("Section name cannot be null or whitespace", nameof(sectionName));

        var options = new LoggerOptions();
        appConfiguration.GetSection(sectionName).Bind(options);
        options.ConfigureAllSinks(configuration, enrich);
    }

    private static void ConfigureAllSinks(
        this LoggerOptions loggerOptions,
        LoggerConfiguration configuration,
        Action<LoggerEnrichmentConfiguration> enrich = null)
    {
        var options = loggerOptions ??
                      throw new ArgumentNullException(nameof(loggerOptions), "Logger options cannot by null");
        var config = configuration ??
                     throw new ArgumentNullException(nameof(configuration), "Logger configuration cannot by null");

        config.Enrich.WithExceptionDetails();
        config.Enrich.With<LogLevelEnricher>();
        enrich?.Invoke(config.Enrich);
        config.MinimumLevel.Is(options.MinLevel);

        options.Filters?.Configure(config);
        options.Console?.Configure(config);
        options.File?.Configure(config);
        options.ElasticSearch?.Configure(config);
        options.Seq?.Configure(config);

        if (options.Labels is null)
            return;

        foreach (var (key, value) in options.Labels)
            configuration.Enrich.WithProperty(key, value);
    }
}