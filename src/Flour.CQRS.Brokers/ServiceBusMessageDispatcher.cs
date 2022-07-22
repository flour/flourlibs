using Flour.BrokersContracts;

namespace Flour.CQRS.Brokers;

internal class ServiceBusMessageDispatcher : ICommandDispatcher, IEventDispatcher
{
    private readonly IBrokerPublisher _brokerPublisher;

    public ServiceBusMessageDispatcher(IBrokerPublisher brokerPublisher)
    {
        _brokerPublisher = brokerPublisher;
    }

    public Task Execute<T>(T command) where T : class, ICommand
    {
        return _brokerPublisher.Publish(command);
    }

    public Task Send<T>(T anEvent) where T : class, IEvent
    {
        return _brokerPublisher.Publish(anEvent);
    }
}