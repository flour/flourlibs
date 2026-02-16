using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace Flour.BlobStorage.GoogleCs;

internal class GcsClientFactory(IOptions<GcsSettings> options) : IGcsClientFactory
{
    public StorageClient CreateClient()
    {
        var credential = CredentialFactory.FromFile<GoogleCredential>(options.Value.CredentialsPath);
        return StorageClient.Create(credential);
    }
}