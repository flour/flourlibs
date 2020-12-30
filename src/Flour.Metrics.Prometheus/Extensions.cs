using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Prometheus;
using Prometheus.SystemMetrics;

namespace Flour.Metrics.Prometheus
{
    public static class Extensions
    {
        private const string DefaultSection = "prometheus";

        public static IServiceCollection AddPrometheus(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = DefaultSection)
        {
            services.Configure<PrometheusOptions>(opts => configuration.GetSection(sectionName).Bind(opts));

            var prometheusOptions = services.BuildServiceProvider().GetRequiredService<IOptions<PrometheusOptions>>();
            if (!prometheusOptions.Value.Enabled)
            {
                return services;
            }


            return services
                .AddHostedService<PrometheusHost>()
                .AddSingleton<PrometheusMiddleware>()
                .AddSystemMetrics();
        }

        public static IApplicationBuilder UsePrometheus(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<PrometheusOptions>>();
            if (!options.Value.Enabled)
            {
                return app;
            }

            var endpoint = string.IsNullOrWhiteSpace(options.Value.Endpoint)
                ? "/metrics"
                : $"/{options.Value.Endpoint.Trim('/')}";

            return app
                .UseMiddleware<PrometheusMiddleware>()
                .UseHttpMetrics()
                .UseGrpcMetrics()
                .UseMetricServer(endpoint);
        }
    }
}