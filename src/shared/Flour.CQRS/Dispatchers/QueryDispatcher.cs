using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Flour.CQRS.Dispatchers
{
    internal class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceScopeFactory _serviceFactory;

        public QueryDispatcher(IServiceScopeFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public async Task<T> SendQuery<T>(IQuery<T> query)
        {
            using (var scope = _serviceFactory.CreateScope())
            {
                var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(T));
                dynamic handler = scope.ServiceProvider.GetRequiredService(handlerType);
                return await handler.Handle(query);
            }
        }

        public async Task<T> SendQuery<Q, T>(Q query) where Q : class, IQuery<T>
        {
            using (var scope = _serviceFactory.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<IQueryHandler<Q, T>>();
                return await handler.Handle(query);
            }
        }
    }
}
