using Flour.BrokersContracts;
using Flour.RabbitMQ.Options;
using System;
using System.Reflection;

namespace Flour.RabbitMQ.Implementations
{
    internal class ConventionProvider : IConventionProvider
    {
        private readonly string _queueTemplate;
        private readonly IConventionsStore _conventionsStore;

        public ConventionProvider(IConventionsStore conventionsStore, RabbitMqOptions options)
        {
            _conventionsStore = conventionsStore;
            _queueTemplate = string.IsNullOrWhiteSpace(options.Queue.QueueTemplate)
                ? QueueOptions.DefaultTemplate
                : options.Queue.QueueTemplate;
        }

        public IMessageConvention Get<T>()
            => Get(typeof(T));

        public IMessageConvention Get(Type type)
        {
            var convention = _conventionsStore.Get(type);
            return convention ?? new MessageConvention(type, GetRoute(type), GetExchange(type), GetQueue(type));
        }

        public string GetExchange(Type type)
        {
            var attribute = GetMessagingAttribute(type);
            return attribute?.Route ?? type.Name;
        }

        public string GetQueue(Type type)
        {
            var attribute = GetMessagingAttribute(type);
            var assembly = type.Assembly.GetName().Name;
            var exchange = GetExchange(type);
            var message = type.Name;

            return attribute?.Queue
                   ?? _queueTemplate.Replace($"%{nameof(assembly)}%", assembly)
                       .Replace($"%{nameof(exchange)}%", exchange)
                       .Replace($"%{nameof(message)}%", message);
        }

        public string GetRoute(Type type)
        {
            var attribute = GetMessagingAttribute(type);
            return attribute?.Route ?? type.Name;
        }

        private MessagingAttribute GetMessagingAttribute(Type type)
            => type.GetCustomAttribute<MessagingAttribute>();

        private IMessageConvention BuildConvention(Type type)
        {
            var convention = new MessageConvention(
                type,
                GetRoute(type),
                GetExchange(type),
                GetQueue(type));

            _conventionsStore.Add(type, convention);
            return convention;
        }
    }
}