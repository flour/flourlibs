using System;

namespace Flour.Redis.DistributedLock.Contracts
{
    public interface IDistributedLock : IDisposable
    {
        string Key { get; }
        IDisposable Redlock { get; }
        bool IsAcquired { get; }
        string LockId { get; }
    }
}