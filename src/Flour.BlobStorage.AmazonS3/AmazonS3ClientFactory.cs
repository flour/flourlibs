using System;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Options;

namespace Flour.BlobStorage.AmazonS3
{
    internal class AmazonS3ClientFactory : IAmazonS3ClientFactory
    {
        private readonly IOptions<AmazonS3Settings> _settings;

        public AmazonS3ClientFactory(IOptions<AmazonS3Settings> settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        internal BasicAWSCredentials GetBasicCredential()
        {
            return new BasicAWSCredentials(_settings.Value.AccessKey, _settings.Value.SecretKey);
        }

        internal AmazonS3Config GetConfiguration()
        {
            return new AmazonS3Config
            {
                ServiceURL = _settings.Value.ServiceUrl,
                UseHttp = false,
                ForcePathStyle = true,
            };
        }

        public IAmazonS3 Create()
        {
            return new AmazonS3Client(GetBasicCredential(), GetConfiguration());
        }
    }
}
