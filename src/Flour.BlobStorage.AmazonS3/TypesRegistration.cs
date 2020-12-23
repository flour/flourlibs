using Amazon.S3.Transfer;
using Flour.BlobStorage.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flour.BlobStorage.AmazonS3
{
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
                .AddSingleton(s => s.GetRequiredService<IAmazonS3ClientFactory>().Create())
                .AddSingleton<ITransferUtility, TransferUtility>()
                .AddTransient<IBlobStorageWriter<AmazonS3BlobReference>, AmazonS3BlobStorageWriter>()
                .AddTransient<IBlobStorageReader<AmazonS3BlobReference>, AmazonS3BlobStorageReader>()
                .AddTransient<IBlobStorageDeleter<AmazonS3BlobContainer>, AmazonS3BlobStorageDeleter>();

            return services;
        }
    }
}
