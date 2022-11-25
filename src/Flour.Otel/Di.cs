using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Flour.OTel;

public static class Di
{
    private const string DefaultSection = "otel:tracing";
    private static List<Regex> _expressionFilters = new();

    public static IServiceCollection AddTracing(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<Activity, HttpRequest> enricher = null,
        string sectionName = DefaultSection)
    {
        var settings = new TracingSettings();
        configuration.GetSection(sectionName).Bind(settings);
        if (!settings.Enabled)
            return services;

        if (settings.BaggageAsTags)
        {
            var listener = new ActivityListener
            {
                ShouldListenTo = _ => true,
                ActivityStopped = act =>
                {
                    foreach (var (key, value) in act.Baggage)
                        act.AddTag(key, value);
                }
            };
            ActivitySource.AddActivityListener(listener);
        }

        return services.AddOpenTelemetryTracing(builder =>
        {
            builder
                .AddSource(settings.ServiceName)
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.EnableGrpcAspNetCoreSupport = true;
                    options.RecordException = true;
                    options.EnrichWithHttpRequest = enricher;

                    if (!settings.Filters.Enabled)
                        return;

                    if (settings.Filters.Expressions?.Count > 0)
                        _expressionFilters = settings.Filters.Expressions
                            .Select(f => new Regex(f, RegexOptions.Compiled))
                            .ToList();

                    options.Filter = context =>
                    {
                        if (!context.Request.Path.HasValue ||
                            string.IsNullOrWhiteSpace(context.Request.Path.Value))
                            return false;
                        
                        var result = true;
                        var requestPath = context.Request.Path.Value;

                        if (settings.Filters.FilterExtensions.Any())
                        {
                            var extension = requestPath.Split('.').LastOrDefault();
                            if (!string.IsNullOrWhiteSpace(extension))
                                result = !settings.Filters.FilterExtensions
                                    .Any(f => extension.StartsWith(f));
                        }

                        if (settings.Filters.Paths.Any())
                        {
                            var filteredByPath = !settings.Filters.Paths
                                .Any(f => context.Request.Path.StartsWithSegments(f) || requestPath.StartsWith(f));

                            result = result && filteredByPath;
                        }

                        if (_expressionFilters.Count > 0)
                            result = result && !_expressionFilters.Any(f => f.IsMatch(requestPath));

                        return result;
                    };
                })
                .AddGrpcInstruments(settings)
                .AddEfCoreInstruments(settings)
                .AddMassTransitInstruments(settings)
                .AddRedisInstruments(settings)
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddEnvironmentVariableDetector()
                        .AddAttributes(settings.Attributes)
                        .AddService(settings.ServiceName));

            if (settings.UseConsoleExporter)
                builder.AddConsoleExporter();

            if (settings.Jaeger.Enabled)
                builder.AddJaegerExporter(options =>
                {
                    options.AgentHost = settings.Jaeger.Host;
                    options.AgentPort = settings.Jaeger.Port;
                    options.MaxPayloadSizeInBytes = 4096;

                    options.ExportProcessorType = ExportProcessorType.Batch;
                    options.BatchExportProcessorOptions = new BatchExportProcessorOptions<Activity>
                    {
                        MaxQueueSize = 2048,
                        ScheduledDelayMilliseconds = 5000,
                        ExporterTimeoutMilliseconds = 30000,
                        MaxExportBatchSize = 512,
                    };
                });
        });
    }

    private static TracerProviderBuilder AddGrpcInstruments(
        this TracerProviderBuilder builder,
        TracingSettings settings)
    {
        if (!settings.GrpcEnabled)
            return builder;

        return builder.AddGrpcClientInstrumentation(options =>
        {
            
        });
    }

    private static TracerProviderBuilder AddEfCoreInstruments(
        this TracerProviderBuilder builder,
        TracingSettings settings)
    {
        if (!settings.EfCoreEnabled)
            return builder;

        return builder.AddEntityFrameworkCoreInstrumentation(options =>
        {
            options.SetDbStatementForText = false;
        });
    }

    private static TracerProviderBuilder AddMassTransitInstruments(
        this TracerProviderBuilder builder,
        TracingSettings settings)
    {
        if (!settings.MassTransitEnabled)
            return builder;

        return builder.AddMassTransitInstrumentation();
    }

    private static TracerProviderBuilder AddRedisInstruments(
        this TracerProviderBuilder builder,
        TracingSettings settings)
    {
        if (!settings.RedisEnabled)
            return builder;

        return builder.AddRedisInstrumentation(null, options =>
        {
            options.EnrichActivityWithTimingEvents = true;
            options.SetVerboseDatabaseStatements = true;
        });
    }
}