using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flour.Redis.DistributedCache
{
    public static class DependencyInjections
    {
        private const string DefaultSection = "redis";

        public static IServiceCollection AddDistributedCacheRedis(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = DefaultSection)
        {
            var cfg = ConnectionSettingsFactory.GetRedisOptions(configuration, sectionName: sectionName);

            return services
                .AddStackExchangeRedisCache(options => options.ConfigurationOptions = cfg);
        }
    }
}