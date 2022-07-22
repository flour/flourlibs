namespace Flour.BrokersContracts;

public interface ISubscriber : IDisposable
{
    ISubscriber Subscribe<T>(Func<IServiceProvider, T, object, Task> handle) where T : class;
}