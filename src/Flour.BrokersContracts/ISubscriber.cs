using System;
using System.Threading.Tasks;

namespace Flour.BrokersContracts
{
    public interface ISubscriber
    {
        ISubscriber Subscribe<T>(Func<IServiceProvider, T, object, Task> handle) where T : class;
    }
}
