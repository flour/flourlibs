using Flour.Redis.DistributedLock.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedLockNet;

namespace Flour.Redis.DistributedLock.Internals;

internal class RedisDistributedLockAcquirer : IDistributedLockAcquirer
{
    private readonly IDistributedLockFactory _lockConnection;
    private readonly ILogger<RedisDistributedLockAcquirer> _logger;
    private readonly RedisDistributedLockSettings _settings;

    public RedisDistributedLockAcquirer(
        IDistributedLockFactory lockConnection,
        IOptions<RedisDistributedLockSettings> lockAcquisitionSettings,
        ILogger<RedisDistributedLockAcquirer> logger)
    {
        _lockConnection = lockConnection;
        _settings = lockAcquisitionSettings.Value;
        _logger = logger;
    }

    public async Task<IDistributedLock> AcquireLock(string key)
    {
        var redlockKey = string.IsNullOrEmpty(_settings.RedisKeyFormat)
            ? $"redlock:{key}"
            : string.Format(_settings.RedisKeyFormat, key);
        var redisLock = await _lockConnection
            .CreateLockAsync(key,
                _settings.RedisLockTtlTimespan,
                _settings.AcquisitionAttemptTotalTimeTimespan,
                _settings.DelayBetweenAcquisitionAttemptsTimespan)
            .ConfigureAwait(false);

        if (!redisLock.IsAcquired)
            return new Contracts.DistributedLock(redlockKey, null, false, string.Empty);

        _logger.LogDebug("Lock acquired for key {Key}. {LockId}", redlockKey, redisLock.LockId);
        return new Contracts.DistributedLock(redlockKey, redisLock, redisLock.IsAcquired, redisLock.LockId);
    }
}