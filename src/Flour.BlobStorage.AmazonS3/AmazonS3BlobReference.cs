using System.Text;
using System.Text.Json;
using Flour.BlobStorage.Contracts;

namespace Flour.BlobStorage.AmazonS3;

public class AmazonS3BlobReference : IBlobReference
{
    private readonly BucketKey _bucketKeyReference;

    public AmazonS3BlobReference(string bucket, string key)
    {
        if (string.IsNullOrWhiteSpace(bucket))
            throw new ArgumentNullException(nameof(bucket));

        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        _bucketKeyReference = new BucketKey(bucket, key);
    }

    public AmazonS3BlobReference(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentNullException(nameof(id));

        _bucketKeyReference = FromId(id);
    }

    public string Bucket => _bucketKeyReference.Bucket;
    public string Key => _bucketKeyReference.Key;

    public string Id => ToId();

    private string ToId()
    {
        var serializedJson = JsonSerializer.Serialize(_bucketKeyReference);
        var bytes = Encoding.UTF8.GetBytes(serializedJson);
        return Convert.ToBase64String(bytes, Base64FormattingOptions.None);
    }

    private BucketKey FromId(string id)
    {
        var bytes = Convert.FromBase64String(id);
        var serializedJson = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<BucketKey>(serializedJson);
    }

    private class BucketKey
    {
        public BucketKey()
        {
        }

        public BucketKey(string bucket, string key)
        {
            Bucket = bucket.ToLowerInvariant();
            Key = key.ToLowerInvariant();
        }

        public string Bucket { get; }
        public string Key { get; }
    }
}