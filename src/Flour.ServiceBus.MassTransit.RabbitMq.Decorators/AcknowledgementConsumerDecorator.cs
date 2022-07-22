using Flour.ServiceBus.Abstractions;
using Flour.ServiceBus.Abstractions.Events;
using MassTransit;

namespace Flour.ServiceBus.MassTransit.RabbitMq.Decorators;

public abstract class AcknowledgementConsumerDecorator<T> : IConsumer<T> where T : class
{
    private readonly IServiceBus _serviceBus;

    protected AcknowledgementConsumerDecorator(IServiceBus serviceBus)
    {
        _serviceBus = serviceBus;
    }

    async Task IConsumer<T>.Consume(ConsumeContext<T> context)
    {
        await Consume(context);
        if (context.Headers.TryGetHeader(InternalConstants.MessageIdHeader, out var messageId))
            await _serviceBus.Publish(AcknowledgedEvent.Ok(messageId?.ToString()));
    }

    protected abstract Task Consume(ConsumeContext<T> context);
}