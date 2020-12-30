using Flour.BrokersContracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flour.RabbitMQ.Implementations
{
    public class RabbitMqBrokerPublisher : IBrokerPublisher
    {
        private readonly IRabbitMqClient _client;
        private readonly IConventionProvider _conventionProvider;

        public RabbitMqBrokerPublisher(IRabbitMqClient client, IConventionProvider conventionProvider)
        {
            _client = client;
            _conventionProvider = conventionProvider;
        }

        public Task Publish<T>(
            T message,
            string correlationId = null,
            string messageId = null,
            string context = null,
            IDictionary<string, object> headers = null
        ) where T : class
        {
            if (message is {})
                _client.Send(message, _conventionProvider.Get(message.GetType()), messageId, correlationId, context, headers);
            return Task.CompletedTask;
        }
    }
}