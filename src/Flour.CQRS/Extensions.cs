using Flour.CQRS.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Flour.CQRS
{
    public static class Extensions
    {
        public static IServiceCollection AddCommonCommandDispatcher(this IServiceCollection services)
            => services.AddSingleton<ICommandDispatcher, CommandDispatcher>();

        public static IServiceCollection AddCommonEventDispatcher(this IServiceCollection services)
            => services.AddSingleton<IEventDispatcher, EventDispatcher>();
            
        public static IServiceCollection AddCommonQueryDispatcher(this IServiceCollection services)
            => services.AddSingleton<IQueryDispatcher, QueryDispatcher>();

        public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
            => ScanIt(services, typeof(ICommandHandler<>));

        public static IServiceCollection AddEventHandlers(this IServiceCollection services)
            => ScanIt(services, typeof(IEventHandler<>));

        public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
             => ScanIt(services, typeof(IQueryHandler<,>));

        private static IServiceCollection ScanIt(IServiceCollection services, Type type)
             => services.Scan(s =>
                s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                    .AddClasses(c => c.AssignableTo(type))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());
    }
}
