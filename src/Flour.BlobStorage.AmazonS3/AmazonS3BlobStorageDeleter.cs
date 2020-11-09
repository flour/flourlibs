using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Flour.BlobStorage.Contracts;
using Microsoft.Extensions.Logging;

namespace Flour.BlobStorage.AmazonS3
{
    internal class AmazonS3BlobStorageDeleter : IBlobStorageDeleter<AmazonS3BlobContainer>
    {
        private readonly IAmazonS3 _client;
        private readonly ILogger<AmazonS3BlobStorageDeleter> _logger;

        public AmazonS3BlobStorageDeleter(IAmazonS3 client, ILogger<AmazonS3BlobStorageDeleter> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task EmptyContainer(IBlobContainer container)
        {
            var bucket = container?.Id;
            if (string.IsNullOrWhiteSpace(bucket))
                throw new ArgumentNullException(nameof(bucket));

            try
            {
                _logger.LogInformation("Trying get objects from {@Bucket}", bucket);
                var request = new ListObjectsV2Request { BucketName = bucket, MaxKeys = int.MaxValue };
                var objectsResponse = await _client.ListObjectsV2Async(request).ConfigureAwait(false);

                _logger.LogDebug("Got {@Count} object key(s) in {@Bucket}", objectsResponse?.KeyCount, bucket);
                if (objectsResponse is null || objectsResponse.KeyCount == 0)
                    return;

                var objectsToRemove = objectsResponse.S3Objects.Select(o => new KeyVersion { Key = o.Key }).ToList();
                _logger.LogInformation("Trying delete {@Count} objects from {@Bucket}", objectsToRemove.Count, bucket);

                var deleteRequest = new DeleteObjectsRequest { BucketName = bucket, Objects = objectsToRemove };
                var deleteResponse = await _client.DeleteObjectsAsync(deleteRequest).ConfigureAwait(false);

                _logger.LogInformation("Removed {@Count} objects from {@Bucket}", deleteResponse?.DeletedObjects.Count, bucket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not remove objects from bucket {@Bucket}", bucket);
                throw new BlobStorageException($"Could not remove objects from bucket {bucket}", ex);
            }
        }

        public async Task DeleteContainer(IBlobContainer container)
        {
            await EmptyContainer(container);
            try
            {
                var response = await _client.DeleteBucketAsync(container.Id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not delete bucket {@Bucket}", container.Id);
                throw new BlobStorageException($"Could not delete bucket {container.Id}", ex);
            }
        }
    }
}
