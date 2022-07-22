using MassTransit;

namespace Flour.ServiceBus.MassTransit.RabbitMq;

internal class MassTransitRequestClientFactory : IMassTransitRequestClientFactory
{
    public IRequestClient<T> Create<T>(IBusControl busControl) where T : class
    {
        return busControl.CreateRequestClient<T>();
    }
}