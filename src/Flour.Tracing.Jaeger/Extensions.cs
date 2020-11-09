using Flour.Commons;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders.Thrift;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Util;

namespace Flour.Tracing.Jaeger
{
    public static class Extensions
    {
        private const string DefaultSectionName = "jaeger";

        public static IServiceCollection AddJaeger(this IServiceCollection services, string configSectionName = DefaultSectionName)
            => services.AddJaeger(services.GetOptions<JaegerOptions>(configSectionName));

        public static IServiceCollection AddJaeger(this IServiceCollection services, JaegerOptions options)
        {
            if (options == null || !options.Enabled)
                return services;

            return services
                .AddSingleton(options)
                .AddOpenTracing()
                .AddSingleton<ITracer>(provider =>
                {
                    var sampler = GetJaegerSampler(options);
                    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                    var jSender = new UdpSender(options.Host, options.Port, options.MaxPacketSize);
                    var jReporter = new RemoteReporter.Builder()
                        .WithLoggerFactory(loggerFactory)
                        .WithSender(jSender)
                        .Build();

                    var tracer = new Tracer.Builder(options.ServiceName)
                        .WithLoggerFactory(loggerFactory)
                        .WithReporter(jReporter)
                        .WithSampler(sampler)
                        .Build();

                    GlobalTracer.Register(tracer);

                    return tracer;
                });
        }

        private static ISampler GetJaegerSampler(JaegerOptions options)
        {
            return options.Sampler.ToEnum<SamplerType>() switch
            {
                SamplerType.Rate => new RateLimitingSampler(options.MaxTracesPerSecond),
                SamplerType.Probabilistic => new ProbabilisticSampler(options.SamplingRate),
                _ => new ConstSampler(true),
            };
        }
    }
}
