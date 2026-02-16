using Flour.Redis.DistributedLock.Contracts;
using Microsoft.Extensions.Logging;

namespace Flour.Redis.DistributedLock.Core;

internal class RedisDistributedLockExecutor(
    IDistributedLockAcquirer lockAcquirer,
    ILogger<RedisDistributedLockExecutor> logger)
    : IDistributedLockExecutor
{
    private readonly IDistributedLockAcquirer _lockAcquirer =
        lockAcquirer ?? throw new ArgumentNullException(nameof(lockAcquirer));

    public async Task<T> Execute<T>(Func<Task<T>> codeToExecute, string lockKey)
    {
        if (string.IsNullOrWhiteSpace(lockKey))
            throw new ArgumentException("A lock key must be supplied.");

        var distributedLock = await _lockAcquirer
            .AcquireLock(lockKey)
            .ConfigureAwait(false);

        using (distributedLock)
        {
            try
            {
                if (!distributedLock.IsAcquired)
                {
                    logger.LogWarning(
                        "Lock {Key} not acquired. {LockId}", distributedLock.Key, distributedLock.LockId);
                    throw new LockNotAcquiredException(
                        $"Lock {distributedLock.Key} not acquired. LockId={distributedLock.LockId}");
                }

                logger.LogDebug("Lock {Key} acquired. {LockId}", distributedLock.Key, distributedLock.LockId);
                return await codeToExecute().ConfigureAwait(false);
            }
            finally
            {
                logger.LogDebug("Releasing lock {Key}. {LockId}", distributedLock.Key, distributedLock.LockId);
            }
        }
    }
}