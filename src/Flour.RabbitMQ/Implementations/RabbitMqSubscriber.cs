using Flour.BrokersContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flour.RabbitMQ.Implementations
{
    public class RabbitMqSubscriber : ISubscriber
    {
        private int _disposed = 0;

        private static readonly ConcurrentDictionary<string, ChannelInfo> _channels =
            new ConcurrentDictionary<string, ChannelInfo>();

        private readonly bool _isLoggerEnabled;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnection _connection;
        private readonly IBrokerPublisher _brokerPublisher;
        private readonly IBrokerSerializer _serializer;
        private readonly IConventionProvider _conventionProvider;
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqSubscriber> _logger;

        public RabbitMqSubscriber(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _connection = serviceProvider.GetRequiredService<IConnection>();
            _brokerPublisher = serviceProvider.GetRequiredService<IBrokerPublisher>();
            _serializer = serviceProvider.GetRequiredService<IBrokerSerializer>();
            _conventionProvider = serviceProvider.GetRequiredService<IConventionProvider>();
            _options = serviceProvider.GetRequiredService<RabbitMqOptions>();
            _logger = serviceProvider.GetRequiredService<ILogger<RabbitMqSubscriber>>();

            _isLoggerEnabled = _options.Logger.Enabled;
        }

        public ISubscriber Subscribe<T>(Func<IServiceProvider, T, object, Task> handler) where T : class
        {
            var declare = _options.Queue?.Declare ?? true;
            var durable = _options.Queue.Durable;
            var exclusive = _options.Queue.Exclusive;
            var autoDelete = _options.Queue.AutoDelete;
            var convention = _conventionProvider.Get<T>();
            var qosOptions = _options.Qos;
            var channelKey = $"{convention.Exchange}:{convention.Queue}:{convention.Route}";

            if (_channels.ContainsKey(channelKey))
                return this;

            var channel = _connection.CreateModel();
            if (!_channels.TryAdd(channelKey, new ChannelInfo(channel, convention)))
                return this;

            if (declare)
            {
                if (_isLoggerEnabled)
                    _logger.LogTrace($"Declaring a queue: {convention}");
                channel.QueueDeclare(convention.Queue, durable, exclusive, autoDelete);
            }

            channel.QueueBind(convention.Queue, convention.Exchange, convention.Route);
            channel.BasicQos(qosOptions.PrefetchSize, qosOptions.PrefetchCount, qosOptions.IsGlobal);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (sender, args) =>
            {
                var messageId = args.BasicProperties.MessageId;
                var correlationId = args.BasicProperties.CorrelationId;
                var timestamp = args.BasicProperties.Timestamp.UnixTime;

                if (_isLoggerEnabled)
                    _logger.LogInformation(
                        $"Received a message #'{messageId}', correlation ID: #'{correlationId}', at {timestamp} via {convention}.");

                try
                {
                    var payload = Encoding.UTF8.GetString(args.Body.Span);
                    var message = _serializer.Deserialize<T>(payload);
                    await HandleAsync(channel, message, messageId, correlationId, null, args, handler)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to handle message");
                    channel.BasicAck(args.DeliveryTag, false);
                    throw;
                }
            };

            channel.BasicConsume(convention.Queue, false, consumer);
            return this;
        }

        public void Dispose()
        {
            if (Interlocked.Increment(ref _disposed) > 1)
                return;

            foreach (var (key, channel) in _channels)
            {
                channel?.Dispose();
                _channels.TryRemove(key, out _);
            }
        }

        private async Task HandleAsync<T>(
            IModel channel,
            T message,
            string messageId,
            string correlationId,
            object context,
            BasicDeliverEventArgs args,
            Func<IServiceProvider, T, object, Task> handler)
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
                // await _brokerPublisher.Publish($"Error: {messageInfo}", correlationId, context: context);
            }
            finally
            {
                channel.BasicAck(args.DeliveryTag, false);
            }
        }

        private class ChannelInfo : IDisposable
        {
            public IModel Channel { get; }
            public IMessageConvention Convention { get; }

            public ChannelInfo(IModel channel, IMessageConvention convention)
            {
                Channel = channel;
                Convention = convention;
            }

            public void Dispose()
            {
                Channel?.Dispose();
            }
        }
    }
}