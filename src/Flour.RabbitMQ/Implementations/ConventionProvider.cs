using System.Reflection;
using Flour.BrokersContracts;
using Flour.RabbitMQ.Options;

namespace Flour.RabbitMQ.Implementations;

internal class ConventionProvider : IConventionProvider
{
    private readonly IConventionsStore _conventionsStore;
    private readonly string _queueTemplate;

    public ConventionProvider(IConventionsStore conventionsStore, RabbitMqOptions options)
    {
        _conventionsStore = conventionsStore;
        _queueTemplate = string.IsNullOrWhiteSpace(options.Queue.QueueTemplate)
            ? QueueOptions.DefaultTemplate
            : options.Queue.QueueTemplate;
    }

    public IMessageConvention Get<T>()
    {
        return Get(typeof(T));
    }

    public IMessageConvention Get(Type type)
    {
        var convention = _conventionsStore.Get(type);
        if (convention is not null)
            return convention;

        convention = new MessageConvention(type, GetRoute(type), GetExchange(type), GetQueue(type));
        _conventionsStore.Add(type, convention);
        return convention;
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

    private static MessagingAttribute GetMessagingAttribute(Type type)
    {
        return type.GetCustomAttribute<MessagingAttribute>();
    }

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