using Flour.BrokersContracts;
using Flour.Commons;
using Flour.RabbitMQ.Implementations;
using Flour.RabbitMQ.Internals;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Flour.RabbitMQ;

public static class Extensions
{
    private const string DefaultSectionName = "rabbitmq";

    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        string configSection = DefaultSectionName)
    {
        var options = services.GetOptions<RabbitMqOptions>(configSection);

        if (options.HostNames is null || options.HostNames.Count == 0)
            throw new ArgumentException("No rabbit hostnames", nameof(options.HostNames));

        services
            .AddSingleton(options)
            .AddSingleton(GetConnection(options))
            .AddSingleton<IConventionsStore, ConventionsStore>()
            .AddSingleton<IConventionProvider, ConventionProvider>()
            .AddSingleton<IBrokerSerializer, RabbitMqSerializer>()
            .AddSingleton<IRabbitMqClient, RabbitMqClient>()
            .AddSingleton<IBrokerPublisher, RabbitMqBrokerPublisher>()
            .AddTransient<ExchangeInitializer>()
            .AddHostedService<RabbitMqHostedService>();

        using var serviceProvider = services.BuildServiceProvider();

        var initializer = serviceProvider.GetRequiredService<ExchangeInitializer>();
        initializer.Initialize().GetAwaiter().GetResult();


        return services;
    }

    public static ISubscriber UseRabbitMq(
        this IApplicationBuilder app)
    {
        return new RabbitMqSubscriber(app.ApplicationServices);
    }

    private static IConnection GetConnection(RabbitMqOptions options)
    {
        return new ConnectionFactory
        {
            Port = options.Port,
            VirtualHost = options.VirtualHost,
            UserName = options.Username,
            Password = options.Password,
            RequestedHeartbeat = options.RequestedHeartbeat.Seconds(),
            RequestedConnectionTimeout = options.RequestedConnectionTimeout.Seconds(),
            SocketReadTimeout = options.SocketReadTimeout.Seconds(),
            SocketWriteTimeout = options.SocketWriteTimeout.Seconds(),
            RequestedChannelMax = options.RequestedChannelMax,
            RequestedFrameMax = options.RequestedFrameMax,
            UseBackgroundThreadsForIO = options.UseBackgroundThreadsForIo,
            DispatchConsumersAsync = true,
            ContinuationTimeout = options.ContinuationTimeout.Seconds(),
            HandshakeContinuationTimeout = options.HandshakeContinuationTimeout.Seconds(),
            NetworkRecoveryInterval = options.NetworkRecoveryInterval.Seconds()
        }.CreateConnection(options.HostNames.ToArray(), options.ClientName);
    }

    private static TimeSpan Seconds(this int value)
    {
        return TimeSpan.FromSeconds(value);
    }
}