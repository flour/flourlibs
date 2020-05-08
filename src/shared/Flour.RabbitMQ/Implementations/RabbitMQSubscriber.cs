using Flour.BrokersContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace Flour.RabbitMQ.Implementations
{
    public class RabbitMQSubscriber : ISubscriber
    {
        private readonly bool _isLoggerEnabled;
        private readonly IServiceProvider _serviceProvider;
        private readonly IModel _channel;
        private readonly IPublisher _publisher;
        private readonly IBrokerSerializer _serializer;
        private readonly IConventionProvider _conventionProvider;
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMQSubscriber> _logger;

        public RabbitMQSubscriber(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _channel = serviceProvider.GetRequiredService<IConnection>().CreateModel();
            _publisher = serviceProvider.GetRequiredService<IPublisher>();
            _serializer = serviceProvider.GetRequiredService<IBrokerSerializer>();
            _conventionProvider = serviceProvider.GetRequiredService<IConventionProvider>();
            _options = serviceProvider.GetRequiredService<RabbitMqOptions>();
            _logger = serviceProvider.GetRequiredService<ILogger<RabbitMQSubscriber>>();

            _isLoggerEnabled = _options.Logger.Enabled;
        }

        public ISubscriber Subscribe<T>(Func<IServiceProvider, T, object, Task> handler) where T : class
        {
            var durable = _options.Queue.Durable;
            var exclusive = _options.Queue.Exclusive;
            var autoDelete = _options.Queue.AutoDelete;
            var convention = _conventionProvider.Get<T>();
            var qosOprions = _options.QOS;

            if (_options.Queue.Declare)
            {
                if (_isLoggerEnabled)
                    _logger.LogTrace($"Declaring a queue: {convention}");
                _channel.QueueDeclare(convention.Queue, durable, exclusive, autoDelete);
            }

            _channel.QueueBind(convention.Queue, convention.Exchange, convention.Route);
            _channel.BasicQos(qosOprions.PrefetchSize, qosOprions.PrefetchCount, qosOprions.IsGlobal);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (sender, args) =>
            {
                var messageId = args.BasicProperties.MessageId;
                var correlationId = args.BasicProperties.CorrelationId;
                var timestamp = args.BasicProperties.Timestamp.UnixTime;

                if (_isLoggerEnabled)
                    _logger.LogInformation($"Received a message #'{messageId}', correlation ID: #'{correlationId}', at {timestamp} via {convention}.");

                try
                {
                    var message = _serializer.DeserializeBinary<T>(args.Body.ToArray());
                    await HandleAsync(message, messageId, correlationId, args, null, handler);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to handle message");
                    _channel.BasicAck(args.DeliveryTag, false);
                    throw;
                }
            };

            _channel.BasicConsume(convention.Queue, false, consumer);
            return this;
        }

        private async Task HandleAsync<T>(T message, string messageId, string correlationId, object context,
            BasicDeliverEventArgs args, Func<IServiceProvider, T, object, Task> handler)
        {
            var messageInfo = $"message #'{messageId}' and correlation ID #'{correlationId}'";
            try
            {
                await handler(_serviceProvider, message, context);
                if (_isLoggerEnabled)
                    _logger.LogTrace($"{messageInfo} handled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to handle {messageInfo}");
                // TODO: Generic error types??
                // await _publisher.Publish($"Error: {messageInfo}", correlationId, context: context);
            }
            finally
            {
                _channel.BasicAck(args.DeliveryTag, false);
            }
        }
    }
}
