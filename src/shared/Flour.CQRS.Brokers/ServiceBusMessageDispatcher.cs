using Flour.BrokersContracts;
using System.Threading.Tasks;

namespace Flour.CQRS.Brokers
{
    internal class ServiceBusMessageDispatcher : ICommandDispatcher, IEventDispatcher
    {
        private readonly IPublisher _publisher;

        public ServiceBusMessageDispatcher(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public Task Execute<T>(T command) where T : class, ICommand
            => _publisher.Publish(command);

        public Task Send<T>(T anEvent) where T : class, IEvent
            => _publisher.Publish(anEvent);
    }
}
