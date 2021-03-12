using System;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders;
using Jaeger.Senders.Thrift;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTracing;
using OpenTracing.Util;

namespace Flour.Tracing.Jaeger
{
    public static class Extensions
    {
        private const int MaxPacketSize = 64967;
        private const string DefaultSectionName = "jaeger";

        public static IServiceCollection AddJaeger(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = DefaultSectionName)
        {
            services.Configure<JaegerOptions>(opts => configuration.GetSection(sectionName)?.Bind(opts));
            var options = services.BuildServiceProvider().GetRequiredService<IOptions<JaegerOptions>>();
            return services.AddJaeger(options.Value);
        }

        private static IServiceCollection AddJaeger(
            this IServiceCollection services,
            JaegerOptions options)
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
                    var jSender = GetSender(options);
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

        private static ISender GetSender(JaegerOptions options)
            => options switch
            {
                { } when options.Udp is { } => new UdpSender(options.Udp.Host, options.Udp.Port,
                    options.MaxPacketSize <= 0 ? MaxPacketSize : options.MaxPacketSize),
                { } when options.Http is { } => GetHttpSender(options),
                _ => new NoopSender()
            };

        private static ISender GetHttpSender(JaegerOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.Http.Host))
            {
                throw new Exception("Missing Jaeger HTTP sender endpoint.");
            }

            var maxPacketSize = options.MaxPacketSize <= 0 ? MaxPacketSize : options.MaxPacketSize;
            var builder = new HttpSender.Builder(options.Http.Host).WithMaxPacketSize(maxPacketSize);

            if (!string.IsNullOrWhiteSpace(options.Http.Token))
            {
                builder = builder.WithAuth(options.Http.Token);
            }

            if (!string.IsNullOrWhiteSpace(options.Http.Username) && !string.IsNullOrWhiteSpace(options.Http.Password))
            {
                builder = builder.WithAuth(options.Http.Username, options.Http.Password);
            }

            if (!string.IsNullOrWhiteSpace(options.Http.Ua))
            {
                builder = builder.WithUserAgent(options.Http.Ua);
            }

            return builder.Build();
        }

        private static ISampler GetJaegerSampler(JaegerOptions options)
        {
            return options.Sampler.ToEnum<SamplerType>() switch
            {
                SamplerType.Rate => new RateLimitingSampler(options.MaxTracesPerSecond),
                SamplerType.Probabilistic => new ProbabilisticSampler(options.SamplingRate),
                SamplerType.Constant => new ConstSampler(true),
                _ => new ConstSampler(true),
            };
        }

        private static T ToEnum<T>(this string value) where T : struct
            => Enum.TryParse<T>(value, true, out var result) ? result : default;
    }
}