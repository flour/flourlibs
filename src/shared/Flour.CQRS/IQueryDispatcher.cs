using System.Threading.Tasks;

namespace Flour.CQRS
{
    public interface IQueryDispatcher
    {
        Task<T> SendQuery<T>(IQuery<T> query);
        Task<T> SendQuery<Q, T>(Q query) where Q : class, IQuery<T>;
    }
}
