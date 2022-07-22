using StackExchange.Redis;

namespace Flour.Redis;

public interface IRedisConnectionMultiplexer
{
    Task<IConnectionMultiplexer> GetMultiplexerInstance();
    Task<IDatabase> GetDatabase();
    Task<IDatabase> GetDatabase(int index);
}