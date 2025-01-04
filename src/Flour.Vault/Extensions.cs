using System.ComponentModel;
using System.Text.Json;
using Flour.Vault.Internals;
using Flour.Vault.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.UserPass;

namespace Flour.Vault;

public static class Extensions
{
    private const string DefaultSectionName = "vault";

    public static IHostApplicationBuilder UseVault(
        this IHostApplicationBuilder hostBuilder,
        string keyValuePath = null,
        string sectionName = DefaultSectionName)
    {
        var settings = new VaultOptions();

        hostBuilder.Configuration.GetSection(sectionName).Bind(settings);
        if (!settings.Enabled)
            return hostBuilder;

        hostBuilder.Services.AddVault(sectionName);
        hostBuilder.Configuration.AddVaultConfiguration(settings);

        return hostBuilder;
    }

    private static IConfigurationManager AddVaultConfiguration(
        this IConfigurationManager manager, VaultOptions options)
    {
        IConfigurationBuilder configBuilder = manager;
        configBuilder.SetVaultConfig(options, null).GetAwaiter().GetResult();
        return manager;
    }

    private static IServiceCollection AddVault(
        this IServiceCollection services,
        string sectionName = DefaultSectionName)
    {
        IConfiguration configuration;
        using (var provider = services.BuildServiceProvider())
        {
            configuration = provider.GetRequiredService<IConfiguration>();
        }

        var options = configuration.GetOptions<VaultOptions>(sectionName);
        VerifyOptions(options);
        services.AddSingleton(options);
        services.AddTransient<IKeyValueSecrets, KeyValueService>();
        var (client, settings) = GetClientAndSettings(options);
        services.AddSingleton(settings);
        services.AddSingleton(client);

        return services;
    }


    private static async Task SetVaultConfig(
        this IConfigurationBuilder builder, VaultOptions options, string kvp)
    {
        VerifyOptions(options);
        var kvPath = string.IsNullOrWhiteSpace(kvp) ? options.KeyValue?.Path : kvp;
        if (!(options.KeyValue?.Enabled ?? false) && string.IsNullOrWhiteSpace(kvPath))
            return;

        var (client, _) = GetClientAndSettings(options);
        var keyValueService = new KeyValueService(client, options);
        var secret = await keyValueService.GetAsync(kvPath);
        var parser = new JsonConfigurationParser();
        var data = parser.Parse(JsonDocument.Parse(JsonSerializer.Serialize(secret)));
        var source = new MemoryConfigurationSource {InitialData = data};
        builder.Add(source);
    }


    private static void VerifyOptions(VaultOptions options)
    {
        if (options.KeyValue is null)
        {
            if (!string.IsNullOrWhiteSpace(options.Key))
                options.KeyValue = new KeyValue
                {
                    Enabled = options.Enabled,
                    Path = options.Key
                };
            return;
        }

        if (options.KeyValue.EngineVersion == 0)
        {
            options.KeyValue.EngineVersion = 2;
            return;
        }

        if (options.KeyValue.EngineVersion > 2 || options.KeyValue.EngineVersion < 1)
            throw new VaultException(
                $"Invalid KV engine version: {options.KeyValue.EngineVersion} (available: 1 or 2).");
    }

    private static (IVaultClient client, VaultClientSettings settings) GetClientAndSettings(VaultOptions options)
    {
        IAuthMethodInfo authMethodInfo = options.AuthType switch
        {
            AuthType.Token => new TokenAuthMethodInfo(options.Token),
            AuthType.UserPass => new UserPassAuthMethodInfo(options.Username, options.Password),
            _ => throw new InvalidEnumArgumentException(
                nameof(options.AuthType), (int) options.AuthType, typeof(AuthType))
        };

        var settings = new VaultClientSettings(options.Url, authMethodInfo);

        var client = new VaultClient(settings);
        return (client, settings);
    }

    private static TModel GetOptions<TModel>(this IConfiguration configuration, string sectionName)
        where TModel : new()
    {
        var model = new TModel();
        configuration.GetSection(sectionName).Bind(model);
        return model;
    }

    public static TModel GetOptions<TModel>(this IServiceCollection services, string sectionName)
        where TModel : new()
    {
        return services
            .BuildServiceProvider()
            .GetRequiredService<IConfiguration>()
            .GetOptions<TModel>(sectionName);
    }
}