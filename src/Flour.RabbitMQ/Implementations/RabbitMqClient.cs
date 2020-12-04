using Flour.BrokersContracts;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Flour.RabbitMQ.Implementations
{
    public class RabbitMqClient : IRabbitMqClient
    {
        private int _channelsCount;

        private readonly bool _isLoggerEnabled;
        private readonly int _maxChannels;
        private readonly IConnection _connection;
        private readonly IBrokerSerializer _serializer;
        private readonly ILogger<RabbitMqClient> _logger;
        private readonly ConcurrentDictionary<int, IModel> _channels = new ConcurrentDictionary<int, IModel>();

        public RabbitMqClient(
            IConnection connection,
            IBrokerSerializer serializer,
            RabbitMqOptions options,
            ILogger<RabbitMqClient> logger)
        {
            _connection = connection;
            _serializer = serializer;
            _logger = logger;
            _isLoggerEnabled = options.Logger.Enabled;
            _maxChannels = options.MaxProducerChannels <= 0 ? 1000 : options.MaxProducerChannels;
        }

        public void Send(
            object message,
            IMessageConvention convention,
            string messageId = null,
            string correlationId = null,
            string spanContext = null,
            IDictionary<string, object> headers = null)
        {
            if (message is null)
            {
                if (_isLoggerEnabled)
                    _logger.LogWarning("Cannot null message via convention: {Convention}", convention);
                return;
            }

            var threadId = Thread.CurrentThread.ManagedThreadId;
            if (!_channels.TryGetValue(threadId, out var channel))
            {
                if (_channelsCount >= _maxChannels)
                    throw new InvalidOperationException(
                        $"Cannot create new channel, limit reached. Got {_channelsCount} of {_maxChannels} possible");

                channel = _connection.CreateModel();
                _channels.TryAdd(threadId, channel);
                Interlocked.Increment(ref _channelsCount);
                _logger.LogTrace(
                    $"Created a channel for thread: {threadId}, total channels: {_channelsCount}/{_maxChannels}");
            }

            var messageBody = _serializer.SerializeBinary(message);
            var channelProps = GetBasicProperties(channel, messageId, correlationId, spanContext, headers);

            channel.BasicPublish(convention.Exchange, convention.Route, channelProps, messageBody);
        }

        private static IBasicProperties GetBasicProperties(
            IModel channel,
            string messageId,
            string correlationId,
            string spanContext,
            IDictionary<string, object> headers)
        {
            var channelProps = channel.CreateBasicProperties();

            channelProps.MessageId = string.IsNullOrWhiteSpace(messageId)
                ? Guid.NewGuid().ToString("N")
                : messageId;
            channelProps.CorrelationId = string.IsNullOrWhiteSpace(correlationId)
                ? Guid.NewGuid().ToString("N")
                : correlationId;
            channelProps.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            channelProps.Headers = new Dictionary<string, object>();

            if (headers is null)
                return channelProps;

            foreach (var (key, value) in headers)
            {
                if (string.IsNullOrWhiteSpace(key) || value is null)
                    continue;
                channelProps.Headers.TryAdd(key, value);
            }

            if (!string.IsNullOrWhiteSpace(spanContext))
                channelProps.Headers.TryAdd("messaging_context", spanContext);

            return channelProps;
        }
    }
}