using System.Reflection;
using Flour.ServiceBus.Abstractions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flour.ServiceBus.MassTransit.RabbitMq.DependencyInjection;

public static class TypeRegistrations
{
    private const string DefaultSectionName = "rabbitMq";
    private static RabbitMqSettings _settings;

    public static IServiceCollection AddMassTransitRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator> configureBus = null,
        Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator> configureRabbitMq = null,
        params Assembly[] consumersAssemblies)
    {
        _settings = new RabbitMqSettings();

        configuration.GetSection(DefaultSectionName)?.Bind(_settings);
        var consumerTypes = GetConsumerTypes(consumersAssemblies ?? Array.Empty<Assembly>()).ToList();

        return services
            .Configure<RabbitMqSettings>(options => configuration.GetSection(DefaultSectionName)?.Bind(options))
            .AddSingleton<IServiceBus, MassTransitServiceBus>()
            .AddSingleton<IMassTransitRequestClientFactory, MassTransitRequestClientFactory>()
            .AddMassTransit(config =>
            {
                if (_settings.AutoReadConsumers)
                    consumerTypes.ForEach(c => config.AddConsumer(c));

                configureBus?.Invoke(config);
                config.SetKebabCaseEndpointNameFormatter();
                config.UsingRabbitMq((context, cfg) =>
                {
                    var port = !string.IsNullOrWhiteSpace(_settings.Port) ? $":{_settings.Port}" : string.Empty;
                    cfg.Host($"rabbitmq://{_settings.Username}:{_settings.Password}@{_settings.Host}{port}");

                    cfg.PrefetchCount = 20;
                    cfg.UseInMemoryOutbox();

                    cfg.UseMessageRetry(r =>
                    {
                        r.Incremental(
                            _settings.Retries <= 0 ? RabbitMqSettings.DefaultNumberOfRetries : _settings.Retries,
                            TimeSpan.FromSeconds(_settings.RetryInSeconds),
                            TimeSpan.FromSeconds(_settings.RetryIncrement));
                    });

                    configureRabbitMq?.Invoke(context, cfg);
                    cfg.ConfigureEndpoints(context);
                });
            });
    }

    private static IEnumerable<Type> GetConsumerTypes(IEnumerable<Assembly> assemblies)
    {
        return assemblies.SelectMany(a => a.GetExportedTypes())
            .Where(t => typeof(IConsumer).IsAssignableFrom(t))
            .Distinct()
            .ToList();
    }
}