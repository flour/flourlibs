using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Flour.BlobStorage.Contracts;

public class BucketKeyReference : IBlobReference
{
    private readonly BucketKey _bucketKeyReference;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };

    public string Bucket => _bucketKeyReference.Bucket;
    public string Key => _bucketKeyReference.Key;
    public string Path => $"{Bucket}/{Key}";

    public string Id => ToId();

    public BucketKeyReference(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentNullException(nameof(id));

        _bucketKeyReference = FromId(id);
    }

    public BucketKeyReference(string bucket, string key)
    {
        if (string.IsNullOrWhiteSpace(bucket))
            throw new ArgumentNullException(nameof(bucket));

        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        _bucketKeyReference = new BucketKey(bucket, key);
    }
    
    private string ToId()
    {
        var serializedJson = JsonSerializer.Serialize(_bucketKeyReference, JsonOptions);
        var bytes = Encoding.UTF8.GetBytes(serializedJson);
        return Convert.ToBase64String(bytes, Base64FormattingOptions.None);
    }

    private static BucketKey FromId(string id)
    {
        var bytes = Convert.FromBase64String(id);
        var serializedJson = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<BucketKey>(serializedJson, JsonOptions);
    }

    public class BucketKey
    {
        public BucketKey()
        {
        }

        public BucketKey(string bucket, string key)
        {
            Bucket = bucket.ToLowerInvariant();
            Key = key.ToLowerInvariant();
        }

        [JsonPropertyName("Bucket")] public string Bucket { get; init; }
        [JsonPropertyName("Key")] public string Key { get; init; }
    }
}