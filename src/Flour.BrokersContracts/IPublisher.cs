using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flour.BrokersContracts
{
    public interface IPublisher
    {
        Task Publish<T>(
            T message,
            string correlationId = null,
            string messageId = null,
            object context = null,
            IDictionary<string, object> headers = null) where T : class;
    }
}
