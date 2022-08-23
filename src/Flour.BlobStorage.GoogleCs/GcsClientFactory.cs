using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace Flour.BlobStorage.GoogleCs;

internal class GcsClientFactory : IGcsClientFactory
{
    private readonly IOptions<GcsSettings> _options;

    public GcsClientFactory(IOptions<GcsSettings> options)
    {
        _options = options;
    }

    public StorageClient CreateClient()
    {
        var credential = GoogleCredential.FromFile(_options.Value.CredentialsPath);
        return StorageClient.Create(credential);
    }
}