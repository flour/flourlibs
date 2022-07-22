using System.Collections.Concurrent;

namespace Flour.RabbitMQ.Implementations;

public class ConventionsStore : IConventionsStore
{
    private readonly IDictionary<Type, IMessageConvention> _conventions
        = new ConcurrentDictionary<Type, IMessageConvention>();

    public void Add<T>(IMessageConvention convention)
    {
        Add(typeof(T), convention);
    }

    public void Add(Type type, IMessageConvention convention)
    {
        if (_conventions.ContainsKey(type))
            _conventions[type] = convention;
        else
            _conventions.Add(type, convention);
    }

    public IMessageConvention Get<T>()
    {
        return Get(typeof(T));
    }

    public IMessageConvention Get(Type type)
    {
        return _conventions.TryGetValue(type, out var convention) ? convention : null;
    }


    public IEnumerable<IMessageConvention> GetAll()
    {
        return _conventions.Values;
    }
}