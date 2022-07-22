using Newtonsoft.Json;
using VaultSharp;

namespace Flour.Vault.Services;

internal class KeyValueService : IKeyValueSecrets
{
    private readonly IVaultClient _client;
    private readonly VaultOptions _options;

    public KeyValueService(
        IVaultClient client,
        VaultOptions options)
    {
        _client = client;
        _options = options;
    }

    public async Task<T> GetDefaultAsync<T>()
    {
        return await GetAsync<T>(_options.Key);
    }

    public async Task<T> GetAsync<T>(string path)
    {
        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(await GetAsync(path)));
    }


    public async Task<IDictionary<string, object>> GetDefaultAsync()
    {
        return await GetAsync(_options.Key);
    }

    public async Task<IDictionary<string, object>> GetAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new VaultException("Vault KV secret path can not be empty.");

        try
        {
            switch (_options.KeyValue.EngineVersion)
            {
                case 1:
                    var secretV1 = await _client.V1.Secrets.KeyValue.V1.ReadSecretAsync(path,
                        _options.KeyValue.MountPoint);
                    return secretV1.Data;
                case 2:
                    var secretV2 =
                        await _client.V1.Secrets.KeyValue.V2.ReadSecretAsync(path, null,
                            _options.KeyValue.MountPoint);
                    return secretV2.Data.Data;
                default:
                    throw new VaultException($"Invalid KV engine version: {_options.KeyValue.EngineVersion}.");
            }
        }
        catch (Exception ex)
        {
            throw new VaultException(
                $"Getting Vault secret for path: '{path}' caused an error. {ex.Message}", ex, path);
        }
    }
}