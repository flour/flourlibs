using System;
using System.Threading.Tasks;
using Flour.Redis.DistributedLock.Contracts;
using Microsoft.Extensions.Logging;

namespace Flour.Redis.DistributedLock.Core
{
    internal class RedisDistributedLockExecutor : IDistributedLockExecutor
    {
        private readonly IDistributedLockAcquirer _lockAcquirer;
        private readonly ILogger<RedisDistributedLockExecutor> _logger;

        public RedisDistributedLockExecutor(
            IDistributedLockAcquirer lockAcquirer,
            ILogger<RedisDistributedLockExecutor> logger)
        {
            _lockAcquirer = lockAcquirer ?? throw new ArgumentNullException(nameof(lockAcquirer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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
                        _logger.LogWarning(
                            "Lock {Key} not acquired. {LockId}", distributedLock.Key, distributedLock.LockId);
                        throw new LockNotAcquiredException(
                            $"Lock {distributedLock.Key} not acquired. LockId={distributedLock.LockId}");
                    }

                    _logger.LogDebug("Lock {Key} acquired. {LockId}", distributedLock.Key, distributedLock.LockId);

                    return await codeToExecute().ConfigureAwait(false);
                }
                finally
                {
                    _logger.LogDebug("Releasing lock {Key}. {LockId}", distributedLock.Key, distributedLock.LockId);
                }
            }
        }
    }
}