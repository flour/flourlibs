using MassTransit;

namespace Flour.ServiceBus.MassTransit.RabbitMq;

public interface IMassTransitRequestClientFactory
{
    IRequestClient<T> Create<T>(IBusControl busControl) where T : class;
}