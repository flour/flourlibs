using Flour.BlobStorage.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flour.BlobStorage.GoogleCs;

public static class TypesRegistration
{
    private const string DefaultSection = "gcs";

    public static IServiceCollection AddGoogleBlobStorage(
        this IServiceCollection services,
        IConfiguration configuration,
        string section = DefaultSection)
    {
        return services
            .Configure<GcsSettings>(opts => configuration.GetSection(section).Bind(opts))
            .AddScoped<IGcsClientFactory, GcsClientFactory>()
            .AddScoped(s => s.GetRequiredService<IGcsClientFactory>().CreateClient())
            .AddTransient<IBlobStorageWriter<BucketKeyReference>, GcsBlobStorageWriter>()
            .AddTransient<IBlobStorageReader<BucketKeyReference>, GcsBlobStorageReader>()
            .AddTransient<IBlobStorageDeleter<BlobContainer, BucketKeyReference>, GcsBlobStorageDeleter>();
    }
}