using System.Net;
using System.Net.Sockets;
using Flour.Redis.Abstractions;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Flour.Redis;

public class ConnectionSettingsFactory
{
    private const string DefaultSectionName = "redis";

    public static ConfigurationOptions GetRedisOptions(
        IConfiguration configuration,
        RedisSettings sectionSettings = null,
        string sectionName = DefaultSectionName)
    {
        var settings = sectionSettings;
        if (settings is null && configuration is { })
        {
            settings = new RedisSettings();
            configuration.GetSection(sectionName).Bind(settings);
        }

        if (settings is null)
            throw new ArgumentException("Cannot get connection settings", nameof(settings));

        var cfg = new ConfigurationOptions { AllowAdmin = true };
        foreach (var hostAddress in settings.HostAddresses)
        {
            if (hostAddress?.Host is null)
                continue;

            var endpoint = IPAddress.TryParse(hostAddress.Host, out var ipAddress)
                ? (EndPoint)new IPEndPoint(ipAddress, hostAddress.Port)
                : new DnsEndPoint(hostAddress.Host, hostAddress.Port,
                    AddressTypeToAddressFamily(hostAddress.Type));

            cfg.EndPoints.Add(endpoint);
        }

        if (!string.IsNullOrWhiteSpace(settings.Master))
            cfg.ServiceName = settings.Master;
        cfg.Password = settings.Password;
        cfg.ClientName = settings.ClientName;

        return cfg;
    }

    private static AddressFamily AddressTypeToAddressFamily(RedisHostAddressType type)
    {
        return type switch
        {
            RedisHostAddressType.IPv4 => AddressFamily.InterNetwork,
            RedisHostAddressType.IPv6 => AddressFamily.InterNetworkV6,
            RedisHostAddressType.Unspecified => AddressFamily.Unspecified,
            _ => throw new NotSupportedException($"Address type {type} not supported")
        };
    }
}