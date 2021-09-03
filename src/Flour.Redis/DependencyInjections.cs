using Flour.Redis.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flour.Redis
{
    public static class DependencyInjections
    {
        private const string DefaultRedisSection = "redis";

        public static IServiceCollection AddRedis(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = DefaultRedisSection)
        {
            return services
                .Configure<RedisSettings>(options => configuration.GetSection(sectionName)?.Bind(options))
                .AddTransient<IConnectionMultiplexerService, ConnectionMultiplexerService>()
                .AddTransient<IConnectionMultiplexerFactory, ConnectionMultiplexerFactory>()
                .AddSingleton(provider =>
                    provider.GetRequiredService<IConnectionMultiplexerFactory>().Create().GetAwaiter().GetResult());
        }
    }
}