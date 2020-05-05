using System;

namespace Flour.RabbitMQ
{
    public interface IConventionProvider
    {
        string GetRoute(Type type);
        string GetExchange(Type type);
        string GetQueue(Type type);
        IMessageConvention Get<T>();
        IMessageConvention Get(Type type);
    }
}
