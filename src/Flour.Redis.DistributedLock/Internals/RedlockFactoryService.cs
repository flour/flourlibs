using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;

namespace Flour.Redis.DistributedLock.Internals;

internal class RedlockFactoryService : IRedlockFactoryService
{
    public Task<IDistributedLockFactory> Create(IEnumerable<RedLockMultiplexer> endpoints)
    {
        return Task.FromResult<IDistributedLockFactory>(RedLockFactory.Create(endpoints.ToList()));
    }
}