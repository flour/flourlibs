using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Flour.CQRS.Dispatchers
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EventDispatcher> _logger;

        public EventDispatcher(IServiceScopeFactory scopeFactory, ILogger<EventDispatcher> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task Send<T>(T anEvent) where T : class, IEvent
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var handlers = scope.ServiceProvider.GetServices<IEventHandler<T>>();
                foreach (var handler in handlers)
                {
                    _logger.LogTrace($"Handling an event of type {typeof(T)} with {handler.GetType()}");
                    await handler.Handle(anEvent);
                }
            }
        }
    }
}
