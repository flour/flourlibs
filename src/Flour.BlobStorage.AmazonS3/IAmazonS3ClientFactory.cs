using Amazon.S3;

namespace Flour.BlobStorage.AmazonS3
{
    internal interface IAmazonS3ClientFactory
    {
        IAmazonS3 Create();
    }
}
