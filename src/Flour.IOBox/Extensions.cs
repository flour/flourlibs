using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Flour.IOBox
{
    public static class Extensions
    {
        private const string DefaultSection = "outbox";

        public static IServiceCollection AddMessageOutbox(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IOutboxConfig> configure = null,
            string sectionName = DefaultSection)
        {
            services.Configure<InOutSettings>(opts => configuration.GetSection(sectionName).Bind(opts));
            services.AddHostedService<OutboxProcessor>();

            var config = new OutboxConfig(
                services,
                services.BuildServiceProvider().GetService<IOptions<InOutSettings>>());

            if (configure is null)
                config.AddInMemory();
            else
                configure(config);

            return services;
        }

        private static IOutboxConfig AddInMemory(
            this IOutboxConfig config)
        {
            config.Services
                .AddTransient<IOutboxHandler, InMemoryOutbox>()
                .AddTransient<IOutboxMessageAccessor, InMemoryOutbox>();

            return config;
        }
    }
}