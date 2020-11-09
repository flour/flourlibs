using Flour.BrokersContracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flour.RabbitMQ.Implementations
{
    public class RabbitMQPublisher : IPublisher
    {
        private readonly IRabbitMQClient _client;
        private readonly IConventionProvider _conventionProvider;

        public RabbitMQPublisher(IRabbitMQClient client, IConventionProvider conventionProvider)
        {
            _client = client;
            _conventionProvider = conventionProvider;
        }

        public Task Publish<T>(
            T message, 
            string correlationId = null, 
            string messageId = null, 
            object context = null, 
            IDictionary<string, object> headers = null
        ) where T : class
        {
            _client.Send(message, _conventionProvider.Get<T>(), messageId, correlationId, headers);
            return Task.CompletedTask;
        }
    }
}
