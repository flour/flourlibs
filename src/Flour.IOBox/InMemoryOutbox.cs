using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flour.IOBox.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flour.IOBox
{
    public class InMemoryOutbox : IOutboxMessageAccessor, IOutboxHandler
    {
        private readonly IOptions<InOutSettings> _settings;
        private readonly ILogger<InMemoryOutbox> _logger;

        private readonly ConcurrentDictionary<string, bool> _inboxMessages = new();
        private readonly ConcurrentDictionary<string, OutboxMessage> _outboxMessages = new();

        public bool Enabled => _settings.Value?.Enabled ?? false;

        public InMemoryOutbox(
            IOptions<InOutSettings> settings,
            ILogger<InMemoryOutbox> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public Task<IReadOnlyList<OutboxMessage>> GetUnsentAsync()
        {
            return Task.FromResult<IReadOnlyList<OutboxMessage>>(
                _outboxMessages.Values.Where(m => m.ProcessedAt is null).ToList());
        }

        public Task ProcessAsync(OutboxMessage message)
            => ProcessAsync(new[] {message});

        public Task ProcessAsync(IEnumerable<OutboxMessage> outboxMessages)
        {
            foreach (var message in outboxMessages)
                message.ProcessedAt = DateTime.UtcNow;

            foreach (var (id, message) in _outboxMessages)
            {
                if (message.ProcessedAt.HasValue)
                    continue;

                _outboxMessages.TryRemove(id, out _);
                _inboxMessages.TryRemove(message.OutboxId, out _);
            }

            return Task.CompletedTask;
        }

        public async Task HandleAsync(string messageId, Func<Task> handler)
        {
            if (!Enabled)
            {
                _logger.LogWarning("Outbox is disabled, incoming messages won't be processed");
                return;
            }

            if (string.IsNullOrWhiteSpace(messageId))
                throw new ArgumentNullException(nameof(messageId), "Processing message id cannot be empty");

            if (_inboxMessages.ContainsKey(messageId))
            {
                _logger.LogTrace($"Message {messageId} was already processed");
                return;
            }

            _logger.LogTrace($"Processing a message {messageId}");
            await handler().ConfigureAwait(false);

            if (!_inboxMessages.TryAdd(messageId, true))
            {
                _logger.LogError($"There was an error when processing a message with id: '{messageId}'.");
                return;
            }

            _logger.LogTrace($"Message {messageId} was processed");
        }

        public Task SendAsync<T>(
            T message,
            string outboxId = null,
            string correlationId = null,
            string messageId = null,
            string messageContext = null,
            IDictionary<string, object> headers = null) where T : class
        {
            if (!Enabled)
            {
                _logger.LogWarning("Outbox is disabled, messages won't be saved into the storage.");
                return Task.CompletedTask;
            }

            var outboxMessage = new OutboxMessage
            {
                Id = string.IsNullOrWhiteSpace(messageId) ? Guid.NewGuid().ToString("N") : messageId,
                OutboxId = outboxId,
                CorrelationId = correlationId,
                Headers = (Dictionary<string, object>) headers,
                Message = message,
                Context = messageContext,
                MessageType = message?.GetType().AssemblyQualifiedName,
                SentAt = DateTime.UtcNow
            };
            _outboxMessages.TryAdd(outboxMessage.Id, outboxMessage);

            return Task.CompletedTask;
        }
    }
}