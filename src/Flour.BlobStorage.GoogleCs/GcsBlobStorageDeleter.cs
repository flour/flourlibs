using Flour.BlobStorage.Contracts;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flour.BlobStorage.GoogleCs;

internal class GcsBlobStorageDeleter(
    StorageClient client,
    IOptions<GcsSettings> options,
    ILogger<GcsBlobStorageDeleter> logger)
    : IBlobStorageDeleter<BlobContainer, BucketKeyReference>
{
    private readonly string _bucket = options.Value.BucketName ??
                                      throw new ArgumentNullException(nameof(options.Value.BucketName));

    public async Task EmptyContainer(BlobContainer container)
        => await Delete(container);

    public async Task DeleteObject(BucketKeyReference reference)
    {
        await client.DeleteObjectAsync(_bucket, reference.Path);
    }

    public async Task DeleteContainer(BlobContainer container)
        => await Delete(container, true);


    private async Task Delete(IBlobContainer container, bool withFolder = false)
    {
        var listRequest = client.Service.Objects.List(_bucket);
        listRequest.Prefix = $"{container.Id}/";
        var objects = await listRequest.ExecuteAsync();
        foreach (var obj in objects.Items.Where(e => withFolder || e.Name != listRequest.Prefix))
        {
            var deleteRequest = client.Service.Objects.Delete(_bucket, obj.Name);
            await deleteRequest.ExecuteAsync();
        }
    }
}