using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flour.Commons
{
    public static class Extensions
    {
        public static TModel GetOptions<TModel>(this IConfiguration configuration, string sectionName)
            where TModel : new()
        {
            var model = new TModel();
            configuration.GetSection(sectionName).Bind(model);
            return model;
        }

        public static TModel GetOptions<TModel>(this IServiceCollection services, string sectionName)
            where TModel : new()
            => services
                .BuildServiceProvider()
                .GetRequiredService<IConfiguration>()
                .GetOptions<TModel>(sectionName);
    }
}
