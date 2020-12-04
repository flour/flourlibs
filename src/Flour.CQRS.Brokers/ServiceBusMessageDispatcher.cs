using Flour.BrokersContracts;
using System.Threading.Tasks;

namespace Flour.CQRS.Brokers
{
    internal class ServiceBusMessageDispatcher : ICommandDispatcher, IEventDispatcher
    {
        private readonly IBrokerPublisher _brokerPublisher;

        public ServiceBusMessageDispatcher(IBrokerPublisher brokerPublisher)
        {
            _brokerPublisher = brokerPublisher;
        }

        public Task Execute<T>(T command) where T : class, ICommand
            => _brokerPublisher.Publish(command);

        public Task Send<T>(T anEvent) where T : class, IEvent
            => _brokerPublisher.Publish(anEvent);
    }
}
