using Grpc.Net.ClientFactory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.ClientFactory;
using GrpcClientFactory = ProtoBuf.Grpc.Client.GrpcClientFactory;

namespace Flour.Grpc;

public static class DependencyInjection
{
    public static IServiceCollection AddGRpcService<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName,
        Action<IServiceProvider, GrpcClientFactoryOptions> configure = null) where T : class
    {
        GrpcClientFactory.AllowUnencryptedHttp2 = true;
        var serviceUrl = configuration.GetValue<string>(sectionName);
        return services
            .AddCodeFirstGrpcClient<T>((sp, opts) =>
            {
                opts.Address = new Uri(serviceUrl);
                opts.ChannelOptionsActions.Add(channelOpts =>
                {
                    channelOpts.HttpHandler = new SocketsHttpHandler
                    {
                        KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                        KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                        PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                        EnableMultipleHttp2Connections = true
                    };
                });
                configure?.Invoke(sp, opts);
            }).Services;
    }

    public static IServiceCollection AddGRpcService<T>(
        this IServiceCollection services,
        Action<IServiceProvider, GrpcClientFactoryOptions> configure) where T : class
    {
        GrpcClientFactory.AllowUnencryptedHttp2 = true;
        return services.AddCodeFirstGrpcClient<T>(configure).Services;
    }
}