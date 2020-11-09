﻿using Flour.BrokersContracts;
using Flour.Commons;
using Flour.RabbitMQ.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Linq;

namespace Flour.RabbitMQ
{
    public static class Extensions
    {
        private const string DefaultSectionName = "rabbitmq";
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, string configSection = DefaultSectionName)
        {
            var options = services.GetOptions<RabbitMqOptions>(configSection);

            services.AddSingleton(options);
            services.AddSingleton(GetConnection(options));

            services.AddSingleton<IConventionsStore, ConventionsStore>();
            services.AddSingleton<IConventionProvider, ConventionProvider>();
            services.AddSingleton<IBrokerSerializer, RabbitMQSerializer>();
            services.AddSingleton<IRabbitMQClient, RabbitMqClient>();
            services.AddSingleton<IPublisher, RabbitMQPublisher>();

            return services;
        }

        public static ISubscriber UseRabbitMQ(this IApplicationBuilder app, Action<IApplicationBuilder> configure = null)
           => new RabbitMQSubscriber(app.ApplicationServices);

        private static IConnection GetConnection(RabbitMqOptions options)
            => new ConnectionFactory
            {
                Port = options.Port,
                VirtualHost = options.VirtualHost,
                UserName = options.Username,
                Password = options.Password,
                DispatchConsumersAsync = true,
            }.CreateConnection(options.HostNames.ToArray(), options.ClientName);
    }
}