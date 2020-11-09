using Flour.BrokersContracts;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace Flour.RabbitMQ.Implementations
{
    public class RabbitMqClient : IRabbitMQClient
    {
        private readonly bool _isLoggerEnabled;
        private readonly IModel _channel;
        private readonly IBrokerSerializer _serializer;
        private readonly ILogger<RabbitMqClient> _logger;

        public RabbitMqClient(
            IConnection connection,
            IBrokerSerializer serializer,
            RabbitMqOptions options,
            ILogger<RabbitMqClient> logger)
        {
            _channel = connection.CreateModel();
            _serializer = serializer;
            _logger = logger;
            _isLoggerEnabled = options.Logger.Enabled;
        }

        public void Send(
            object message,
            IMessageConvention convention,
            string messageId = null,
            string correlationId = null,
            IDictionary<string, object> headers = null)
        {
            if (message is null)
            {
                if (_isLoggerEnabled)
                    _logger.LogWarning($"Cannot null message via convention: {convention}");
                return;
            }

            var messageBody = _serializer.SerializeBinary(message);
            var channelProps = GetBasicProperties(messageId, correlationId, headers);

            _channel.BasicPublish(convention.Exchange, convention.Route, channelProps, messageBody);
        }

        private IBasicProperties GetBasicProperties(string messageId, string correlationId, IDictionary<string, object> headers)
        {
            var channelProps = _channel.CreateBasicProperties();

            channelProps.MessageId = string.IsNullOrWhiteSpace(messageId) ? Guid.NewGuid().ToString("N") : messageId;
            channelProps.CorrelationId = string.IsNullOrWhiteSpace(correlationId) ? Guid.NewGuid().ToString("N") : correlationId;
            channelProps.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            channelProps.Headers = new Dictionary<string, object>();

            if (headers is { })
            {
                foreach (var (key, value) in headers)
                {
                    if (string.IsNullOrWhiteSpace(key) || value is null)
                        continue;
                    channelProps.Headers.TryAdd(key, value);
                }
            }

            return channelProps;
        }
    }
}
