using Flour.BrokersContracts;
using Microsoft.Extensions.DependencyInjection;

namespace Flour.CQRS.Brokers;

public static class Extensions
{
    public static IServiceCollection AddServiceBusCommandDispatcher(this IServiceCollection services)
    {
        return services.AddTransient<ICommandDispatcher, ServiceBusMessageDispatcher>();
    }

    public static IServiceCollection AddServiceBusEventDispatcher(this IServiceCollection services)
    {
        return services.AddTransient<IEventDispatcher, ServiceBusMessageDispatcher>();
    }

    public static ISubscriber SubscribeCommand<T>(this ISubscriber busSubscriber) where T : class, ICommand
    {
        return busSubscriber.Subscribe<T>(async (serviceProvider, command, _) =>
        {
            using var scope = serviceProvider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<ICommandHandler<T>>().Handle(command);
        });
    }

    public static ISubscriber SubscribeEvent<T>(this ISubscriber busSubscriber) where T : class, IEvent
    {
        return busSubscriber.Subscribe<T>(async (serviceProvider, anEvent, _) =>
        {
            using var scope = serviceProvider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<IEventHandler<T>>().Handle(anEvent);
        });
    }
}