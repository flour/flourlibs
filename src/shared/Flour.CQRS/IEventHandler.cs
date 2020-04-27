using System.Threading.Tasks;

namespace Flour.CQRS
{
    public interface IEventHandler<in T> where T : class, IEvent
    {
        Task Handle(IEvent anEvent);
    }
}
