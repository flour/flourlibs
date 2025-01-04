using Flour.Redis.DistributedLock.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flour.Redis.DistributedLock.Internals;

internal class RedisDistributedLockChecker : IDistributedLockChecker
{
    private readonly ILogger<RedisDistributedLockChecker> _logger;
    private readonly IRedisConnectionMultiplexer _redisConnection;
    private readonly RedisDistributedLockSettings _settings;

    public RedisDistributedLockChecker(
        IRedisConnectionMultiplexer connectionMultiplexer,
        ILogger<RedisDistributedLockChecker> logger,
        IOptions<RedisDistributedLockSettings> settings)
    {
        _redisConnection = connectionMultiplexer;
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task<bool> Exists(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        var redlockKey = string.IsNullOrEmpty(_settings.RedisKeyFormat)
            ? $"redlock:{key}"
            : string.Format(_settings.RedisKeyFormat, key);
        _logger.LogInformation("Checking whether a lock with key '{Key}' exists", redlockKey);
        try
        {
            var database = await _redisConnection.GetDatabase().ConfigureAwait(false);
            var result = await database
                .StringGetAsync(redlockKey)
                .ConfigureAwait(false);
            return !result.IsNull;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Could not determine whether a lock with key '{Key}' exists due to an error. See inner exception for details",
                redlockKey);
            throw;
        }
    }
}