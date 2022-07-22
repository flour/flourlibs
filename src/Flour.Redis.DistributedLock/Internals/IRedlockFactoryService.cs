using RedLockNet;
using RedLockNet.SERedis.Configuration;

namespace Flour.Redis.DistributedLock.Internals;

internal interface IRedlockFactoryService
{
    Task<IDistributedLockFactory> Create(IEnumerable<RedLockMultiplexer> endpoints);
}