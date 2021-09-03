using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS.SDK.Redis.DistributedLock.Internals;
using Flour.Redis.Abstractions;
using Microsoft.Extensions.Options;
using RedLockNet;
using RedLockNet.SERedis.Configuration;

namespace Flour.Redis.DistributedLock.Internals
{
    internal class RedLockConnectionFactory : IRedLockConnectionFactory
    {
        private readonly IRedisConnectionMultiplexer _connectionMultiplexer;
        private readonly IRedlockFactoryService _redlockFactoryService;
        private readonly RedisDistributedLockSettings _settings;
        private readonly RedisSettings _redisSettings;

        public RedLockConnectionFactory(
            IRedisConnectionMultiplexer connectionMultiplexer,
            IRedlockFactoryService redlockFactoryService, 
            IOptions<RedisDistributedLockSettings> settings,
            IOptions<RedisSettings> redisSettings)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _redlockFactoryService = redlockFactoryService;
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _redisSettings = redisSettings?.Value ?? throw new ArgumentNullException(nameof(redisSettings));
        }

        public async Task<IDistributedLockFactory> Create()
        {
            var endpoints = await GetRedLockMultiplexer().ConfigureAwait(false);
            return await _redlockFactoryService.Create(endpoints.ToArray()).ConfigureAwait(false);
        }
            
        internal async Task<IEnumerable<RedLockMultiplexer>> GetRedLockMultiplexer()
        {
            var connectionMultiplexer = await _connectionMultiplexer.GetMultiplexerInstance().ConfigureAwait(false);
            var redLockMultiplexer = new RedLockMultiplexer(connectionMultiplexer);

            if (_redisSettings.DatabaseIndex.HasValue)
                redLockMultiplexer.RedisDatabase = _redisSettings.DatabaseIndex;

            if (string.IsNullOrEmpty(_settings.RedisKeyFormat))
                return new[]
                {
                    redLockMultiplexer
                };
            
            if (!_settings.RedisKeyFormat.Contains("{0}"))
                throw new FormatException($"The {_settings.RedisKeyFormat} property must have a {{0}} in the string");

            redLockMultiplexer.RedisKeyFormat = _settings.RedisKeyFormat;

            return new[]
            {
                redLockMultiplexer
            };
        }
    }
}