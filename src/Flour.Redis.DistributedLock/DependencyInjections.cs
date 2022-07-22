using CS.SDK.Redis.DistributedLock.Internals;
using Flour.Redis.DistributedLock.Contracts;
using Flour.Redis.DistributedLock.Internals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flour.Redis.DistributedLock;

public static class DependencyInjections
{
    private const string DefaultSection = "redisLock";

    public static IServiceCollection AddRedisDistributedLocking(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = DefaultSection)
    {
        return services
            .AddRedis(configuration)
            .Configure<RedisDistributedLockSettings>(options =>
                configuration.GetSection(sectionName)?.Bind(options))
            .AddTransient<IRedLockConnectionFactory, RedLockConnectionFactory>()
            .AddTransient<IDistributedLockAcquirer, RedisDistributedLockAcquirer>()
            .AddTransient<IDistributedLockChecker, RedisDistributedLockChecker>()
            .AddTransient<IRedlockFactoryService, RedlockFactoryService>()
            .AddSingleton(sp => sp.GetRequiredService<IRedLockConnectionFactory>().Create().GetAwaiter().GetResult());
    }
}