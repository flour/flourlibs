using Flour.BlobStorage.Contracts;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Object = Google.Apis.Storage.v1.Data.Object;

namespace Flour.BlobStorage.GoogleCs;

internal class GcsBlobStorageWriter(
    StorageClient client,
    IOptions<GcsSettings> options,
    ILogger<GcsBlobStorageWriter> logger)
    : IBlobStorageWriter<BucketKeyReference>
{
    private readonly string _bucket = options.Value.BucketName ??
                                      throw new ArgumentNullException(nameof(options.Value.BucketName));

    public async Task Store(BucketKeyReference reference, Blob data)
    {
        var obj = new Object
        {
            Bucket = _bucket,
            Name = reference.Path,
            Metadata = data.Metadata,
            ContentType = data.ContentType,
        };
        var uploader = client.CreateObjectUploader(obj, data.Stream);
        await uploader.UploadAsync();
    }
}