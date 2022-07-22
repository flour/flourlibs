using StackExchange.Redis;

namespace Flour.Redis.Abstractions;

public interface IConnectionMultiplexerService
{
    Task<IConnectionMultiplexer> GetConnection(ConfigurationOptions configuration);
    Task<IConnectionMultiplexer> GetConnection(string serviceName, params string[] hostAddresses);
}