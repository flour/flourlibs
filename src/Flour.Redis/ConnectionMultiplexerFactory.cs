using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Flour.Redis.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Flour.Redis
{
    internal class ConnectionMultiplexerFactory : IConnectionMultiplexerFactory
    {
        private readonly IConnectionMultiplexerService _connectionService;
        private readonly RedisSettings _settings;
        private readonly ILogger<ConnectionMultiplexerFactory> _logger;

        public ConnectionMultiplexerFactory(
            IConnectionMultiplexerService connectionService,
            IOptions<RedisSettings> settings,
            ILogger<ConnectionMultiplexerFactory> logger)
        {
            _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger;
        }

        public Task<IConnectionMultiplexer> Create()
        {
            if (!string.IsNullOrWhiteSpace(_settings.Master))
            {
                return _connectionService.GetConnection(
                    _settings.Master,
                    string.Join(',',
                        _settings.HostAddresses.Select(e => $"{e.Host}{(e.Port > 0 ? $":{e.Port}" : "")}")));
            }

            var configuration = ConnectionSettingsFactory.GetRedisOptions(null, _settings);

            return _connectionService.GetConnection(configuration);
        }

        private static AddressFamily AddressTypeToAddressFamily(RedisHostAddressType type)
        {
            return type switch
            {
                RedisHostAddressType.IPv4 => AddressFamily.InterNetwork,
                RedisHostAddressType.IPv6 => AddressFamily.InterNetworkV6,
                RedisHostAddressType.Unspecified => AddressFamily.Unspecified,
                _ => throw new NotSupportedException($"Address type {type} not supported")
            };
        }
    }
}