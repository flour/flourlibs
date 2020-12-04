using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Flour.RabbitMQ.Implementations
{
    public class ConventionsStore : IConventionsStore
    {

        private readonly IDictionary<Type, IMessageConvention> _conventions
            = new ConcurrentDictionary<Type, IMessageConvention>();

        public void Add<T>(IMessageConvention convention)
            => Add(typeof(T), convention);

        public void Add(Type type, IMessageConvention convention)
        {
            if (_conventions.ContainsKey(type))
                _conventions[type] = convention;
            else
                _conventions.Add(type, convention);
        }

        public IMessageConvention Get<T>()
            => Get(typeof(T));

        public IMessageConvention Get(Type type)
            => _conventions.TryGetValue(type, out var convention) ? convention : null;


        public IEnumerable<IMessageConvention> GetAll()
            => _conventions.Values;

        
    }
}