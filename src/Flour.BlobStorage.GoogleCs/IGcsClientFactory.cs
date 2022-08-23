using Google.Cloud.Storage.V1;

namespace Flour.BlobStorage.GoogleCs;

internal interface IGcsClientFactory
{
    StorageClient CreateClient();
}