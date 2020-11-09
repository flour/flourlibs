using System;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Flour.BlobStorage.Contracts;

namespace Flour.BlobStorage.AmazonS3
{
    internal class AmazonS3BlobStorageWriter : IBlobStorageWriter<AmazonS3BlobReference>
    {
        public const string BucketAlreadyOwnedByYouError = "BucketAlreadyOwnedByYou";

        private readonly IAmazonS3 _client;
        private readonly ITransferUtility _transferUtility;

        public AmazonS3BlobStorageWriter(IAmazonS3 client, ITransferUtility transferUtility)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _transferUtility = transferUtility ?? throw new ArgumentNullException(nameof(transferUtility));
        }

        public async Task Store(AmazonS3BlobReference reference, Blob data)
        {
            if (reference == null)
                throw new ArgumentNullException(nameof(reference));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            await CreateBucketIfNotExists(reference.Bucket).ConfigureAwait(false);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                BucketName = reference.Bucket,
                Key = reference.Key,
                InputStream = data.Stream,
                ContentType = data.ContentType,
            };

            if (data.Metadata != null)
                foreach (var nameValue in data.Metadata)
                    uploadRequest.Metadata.Add(nameValue.Key, nameValue.Value);

            await _transferUtility.UploadAsync(uploadRequest).ConfigureAwait(false);
        }

        internal async Task CreateBucketIfNotExists(string bucketName)
        {
            var exists = await AmazonS3Util.DoesS3BucketExistV2Async(_client, bucketName);
            if (!exists)
                try
                {
                    await _client.PutBucketAsync(bucketName).ConfigureAwait(false);
                }
                catch (AmazonS3Exception ex)
                {
                    if (ex.ErrorCode != BucketAlreadyOwnedByYouError)
                        throw;
                }

        }

    }
}
