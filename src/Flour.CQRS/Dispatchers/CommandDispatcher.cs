using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Flour.CQRS.Dispatchers
{
    internal class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceScopeFactory _serviceFactory;

        public CommandDispatcher(IServiceScopeFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public async Task Execute<T>(T command) where T : class, ICommand
        {
            using var scope = _serviceFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<T>>();
            await handler.Handle(command);
        }
    }
}
