using Flour.BlobStorage.Contracts;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Object = Google.Apis.Storage.v1.Data.Object;

namespace Flour.BlobStorage.GoogleCs;

internal class GcsBlobStorageWriter : IBlobStorageWriter<BucketKeyReference>
{
    private readonly StorageClient _client;
    private readonly ILogger<GcsBlobStorageWriter> _logger;
    private readonly string _bucket;

    public GcsBlobStorageWriter(
        StorageClient client,
        IOptions<GcsSettings> options,
        ILogger<GcsBlobStorageWriter> logger)
    {
        _client = client;
        _logger = logger;
        _bucket = options.Value.BucketName ?? throw new ArgumentNullException(nameof(options.Value.BucketName));
    }

    public async Task Store(BucketKeyReference reference, Blob data)
    {
        var obj = new Object
        {
            Bucket = _bucket,
            Name = reference.Path,
            Metadata = data.Metadata,
            ContentType = data.ContentType,
        };
        var uploader = _client.CreateObjectUploader(obj, data.Stream);
        await uploader.UploadAsync();
    }
}