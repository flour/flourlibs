using System.Threading.Tasks;

namespace Flour.CQRS
{
    public interface ICommandDispatcher
    {
        Task Execute<T>(T command) where T : class, ICommand;
    }
}
