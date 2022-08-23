using Amazon.S3.Transfer;
using Flour.BlobStorage.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flour.BlobStorage.AmazonS3;

public static class TypesRegistration
{
    private const string DefaultSection = "s3blob";

    public static IServiceCollection AddAmazonS3BlobStorage(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = DefaultSection)
    {
        services
            .Configure<AmazonS3Settings>(options => configuration.GetSection(sectionName)?.Bind(options))
            .AddTransient<IAmazonS3ClientFactory, AmazonS3ClientFactory>()
            .AddScoped(s => s.GetRequiredService<IAmazonS3ClientFactory>().Create())
            .AddScoped<ITransferUtility, TransferUtility>()
            .AddTransient<IBlobStorageWriter<BucketKeyReference>, AmazonS3BlobStorageWriter>()
            .AddTransient<IBlobStorageReader<BucketKeyReference>, AmazonS3BlobStorageReader>()
            .AddTransient<IBlobStorageDeleter<BlobContainer, BucketKeyReference>, AmazonS3BlobStorageDeleter>();

        return services;
    }
}