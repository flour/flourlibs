using System.Threading.Tasks;

namespace Flour.CQRS
{
    public interface IEventDispatcher
    {
        Task Send<T>(T anEvent) where T : class, IEvent;
    }
}
