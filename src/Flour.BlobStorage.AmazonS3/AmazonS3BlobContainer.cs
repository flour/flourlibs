using Flour.BlobStorage.Contracts;

namespace Flour.BlobStorage.AmazonS3;

public class AmazonS3BlobContainer : IBlobContainer
{
    public AmazonS3BlobContainer(string bucket)
    {
        if (string.IsNullOrWhiteSpace(bucket))
            throw new ArgumentNullException(nameof(bucket));

        Id = bucket;
    }

    public string Id { get; }
}