using System.Linq;
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

        public static IServiceCollection AddTracing(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = DefaultSection)
        {
            var settings = new TracingSettings();
            configuration.GetSection(sectionName).Bind(settings);
            if (!settings.Enabled)
                return services;

            return services.AddOpenTelemetryTracing(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation(e =>
                    {
                        e.EnableGrpcAspNetCoreSupport = true;
                        e.RecordException = true;
                        e.Enrich = (activity, eventName, rawObject) =>
                        {
                            if (!eventName.Equals("OnStartActivity"))
                                return;

                            settings.Enrichers.ForEach(f => activity.AddTag(f.Key, f.Value));

                            if (rawObject is not HttpRequest httpRequest)
                                return;

                            activity.SetTag("requestProtocol", httpRequest.Protocol);
                            activity.AddBaggage("traceId_bag", httpRequest.HttpContext.TraceIdentifier);
                            activity.SetCustomProperty("traceId_prop", httpRequest.HttpContext.TraceIdentifier);
                        };

                        if (settings.Filters.Enabled)
                        {
                            e.Filter = context =>
                            {
                                var result = false;
                                if (settings.Filters.FilterExtensions.Any() &&
                                    !string.IsNullOrWhiteSpace(context.Request.Path.Value))
                                {
                                    var extension = context.Request.Path.Value.Split('.').LastOrDefault();
                                    if (!string.IsNullOrWhiteSpace(extension))
                                    {
                                        result = settings.Filters.FilterExtensions
                                            .Any(f => extension.StartsWith(f));
                                    }
                                }

                                if (settings.Filters.FilterPaths.Any() && context.Request.Path.HasValue)
                                {
                                    result = result || settings.Filters.FilterPaths
                                        .Any(f => context.Request.Path.StartsWithSegments(f));
                                }

                                return result;
                            };
                        }
                    })
                    .AddHttpClientInstrumentation()
                    .AddGrpcClientInstrumentation()
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