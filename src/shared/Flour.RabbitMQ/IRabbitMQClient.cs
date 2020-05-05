using System.Collections.Generic;

namespace Flour.RabbitMQ
{
    public interface IRabbitMQClient
    {
        void Send(
            object message,
            IMessageConvention convention,
            string messageId = null,
            string correlationId = null,
            IDictionary<string, object> headers = null);
    }
}
