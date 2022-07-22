using Flour.CQRS.Dispatchers;
using Microsoft.Extensions.DependencyInjection;

namespace Flour.CQRS;

public static class Extensions
{
    public static IServiceCollection AddCommonCommandDispatcher(this IServiceCollection services)
    {
        return services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
    }

    public static IServiceCollection AddCommonEventDispatcher(this IServiceCollection services)
    {
        return services.AddSingleton<IEventDispatcher, EventDispatcher>();
    }

    public static IServiceCollection AddCommonQueryDispatcher(this IServiceCollection services)
    {
        return services.AddSingleton<IQueryDispatcher, QueryDispatcher>();
    }

    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        return ScanIt(services, typeof(ICommandHandler<>));
    }

    public static IServiceCollection AddEventHandlers(this IServiceCollection services)
    {
        return ScanIt(services, typeof(IEventHandler<>));
    }

    public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        return ScanIt(services, typeof(IQueryHandler<,>));
    }

    private static IServiceCollection ScanIt(IServiceCollection services, Type type)
    {
        return services.Scan(s =>
            s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .AddClasses(c => c.AssignableTo(type))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
    }
}