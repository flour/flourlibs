using System.Collections.Generic;
using System.Threading.Tasks;
using Flour.IOBox.Models;

namespace Flour.IOBox
{
    public interface IOutboxMessageAccessor
    {
        Task<IReadOnlyList<OutboxMessage>> GetUnsentAsync();
        Task ProcessAsync(OutboxMessage message);
        Task ProcessAsync(IEnumerable<OutboxMessage> outboxMessages);
    }
}