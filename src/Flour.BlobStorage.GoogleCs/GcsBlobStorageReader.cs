using Flour.BlobStorage.Contracts;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flour.BlobStorage.GoogleCs;

internal class GcsBlobStorageReader : IBlobStorageReader<BucketKeyReference>
{
    private readonly StorageClient _client;
    private readonly ILogger<GcsBlobStorageReader> _logger;
    private readonly string _bucket;

    public GcsBlobStorageReader(
        StorageClient client,
        IOptions<GcsSettings> options,
        ILogger<GcsBlobStorageReader> logger)
    {
        _client = client;
        _logger = logger;
        _bucket = options.Value.BucketName ?? throw new ArgumentNullException(nameof(options.Value.BucketName));
    }

    public async Task<Blob> Get(BucketKeyReference reference)
    {
        var stream = new MemoryStream();
        var metadata = await _client.DownloadObjectAsync(_bucket, reference.Path, stream);
        stream.Position = 0;
        return new Blob(stream, metadata.ContentType, metadata.Metadata);
    }

    public async Task<IDictionary<string, string>> GetMetadata(BucketKeyReference reference)
    {
        var metadata = await _client.GetObjectAsync(_bucket, reference.Path);
        return metadata.Metadata;
    }

    public async Task<bool> Exists(BucketKeyReference reference)
    {
        var metadata = await _client.GetObjectAsync(_bucket, reference.Path);
        return metadata is { };
    }
}