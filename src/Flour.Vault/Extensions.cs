using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Flour.Commons;
using Flour.Vault.Internals;
using Flour.Vault.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.UserPass;

namespace Flour.Vault
{
    public static class Extensions
    {
        private const string DefaultSectionName = "vault";

        public static IWebHostBuilder UseVault(
            this IWebHostBuilder hosBuilder,
            string keyValuePath = null,
            string sectionName = DefaultSectionName)
            => hosBuilder
                .ConfigureServices(services => services.AddVault(sectionName))
                .ConfigureAppConfiguration((_, builder) =>
                {
                    var options = builder.Build().GetOptions<VaultOptions>(sectionName);
                    if (!options.Enabled)
                        return;
                    builder.SetVaultConfig(options, keyValuePath).GetAwaiter().GetResult();
                });

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
            this IConfigurationBuilder builder,
            VaultOptions options,
            string kvp)
        {
            VerifyOptions(options);
            var kvPath = string.IsNullOrWhiteSpace(kvp) ? options.KeyValue?.Path : kvp;
            if (!(options.KeyValue?.Enabled ?? false) && string.IsNullOrWhiteSpace(kvPath))
                return;

            var (client, _) = GetClientAndSettings(options);
            var keyValueService = new KeyValueService(client, options);
            var secret = await keyValueService.GetAsync(kvPath);
            var parser = new JsonConfigurationParser();
            var data = parser.Parse(JObject.FromObject(secret));
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
    }
}