using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Flour.OTel
{
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
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.EnableGrpcAspNetCoreSupport = true;
                        options.RecordException = true;
                        options.Enrich = (activity, eventName, rawObject) =>
                        {
                            if (!eventName.Equals("OnStartActivity"))
                                return;

                            settings.Enrichers.ForEach(f => activity.AddTag(f.Key, f.Value));

                            if (rawObject is HttpRequest httpRequest && enricher != null)
                                enricher.Invoke(activity, httpRequest);
                        };

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
                                {
                                    result = !settings.Filters.FilterExtensions
                                        .Any(f => extension.StartsWith(f));
                                }
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
                    .AddHttpClientInstrumentation()
                    .AddGrpcClientInstrumentation(options => { options.SuppressDownstreamInstrumentation = true; })
                    .AddSource(settings.ServiceName)
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddEnvironmentVariableDetector()
                            .AddAttributes(settings.Attributes)
                            .AddService(settings.ServiceName));

                if (settings.Jaeger.Enabled)
                {
                    builder.AddJaegerExporter(options =>
                    {
                        options.AgentHost = settings.Jaeger.Host;
                        options.AgentPort = settings.Jaeger.Port;
                        options.ExportProcessorType = OpenTelemetry.ExportProcessorType.Simple;
                    });
                }
            });
        }
    }
}