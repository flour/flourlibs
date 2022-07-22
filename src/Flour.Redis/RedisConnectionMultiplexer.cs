using Flour.Redis.Abstractions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Flour.Redis;

internal class RedisConnectionMultiplexer : IRedisConnectionMultiplexer
{
    private readonly IConnectionMultiplexerFactory _factory;
    private readonly RedisSettings _settings;
    private int _disposeCounter;

    private IConnectionMultiplexer _instance;

    public RedisConnectionMultiplexer(IConnectionMultiplexerFactory factory, IOptions<RedisSettings> settings)
    {
        _factory = factory;
        _settings = settings.Value;
    }

    public async Task<IDatabase> GetDatabase()
    {
        var connectionMultiplexer = await GetMultiplexerInstance().ConfigureAwait(false);
        return _settings.DatabaseIndex.HasValue
            ? connectionMultiplexer.GetDatabase(_settings.DatabaseIndex.Value)
            : connectionMultiplexer.GetDatabase();
    }

    public async Task<IDatabase> GetDatabase(int database)
    {
        var connectionMultiplexer = await GetMultiplexerInstance().ConfigureAwait(false);
        return connectionMultiplexer.GetDatabase(database);
    }

    public async Task<IConnectionMultiplexer> GetMultiplexerInstance()
    {
        return _instance ??= await _factory.Create().ConfigureAwait(false);
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (Interlocked.Increment(ref _disposeCounter) != 1)
            return;

        if (disposing) _instance?.Dispose();
    }
}