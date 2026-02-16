using Flour.BlobStorage.Contracts;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flour.BlobStorage.GoogleCs;

internal class GcsBlobStorageReader(
    StorageClient client,
    IOptions<GcsSettings> options,
    ILogger<GcsBlobStorageReader> logger)
    : IBlobStorageReader<BucketKeyReference>
{
    private readonly string _bucket = options.Value.BucketName ??
                                      throw new ArgumentNullException(nameof(options.Value.BucketName));

    public async Task<Blob> Get(BucketKeyReference reference)
    {
        var stream = new MemoryStream();
        var metadata = await client.DownloadObjectAsync(_bucket, reference.Path, stream);
        stream.Position = 0;
        return new Blob(stream, metadata.ContentType, metadata.Metadata);
    }

    public async Task<IDictionary<string, string>> GetMetadata(BucketKeyReference reference)
    {
        var metadata = await client.GetObjectAsync(_bucket, reference.Path);
        return metadata.Metadata;
    }

    public async Task<bool> Exists(BucketKeyReference reference)
    {
        var metadata = await client.GetObjectAsync(_bucket, reference.Path);
        return metadata is { };
    }
}