using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Flour.RabbitMQ.Internals
{
    internal class RabbitMqHostedService : IHostedService
    {
        private readonly IConnection _connection;

        public RabbitMqHostedService(IConnection connection)
        {
            _connection = connection;
        }

        public Task StartAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _connection.Close(200, "finish work");
            return Task.CompletedTask;
        }
    }
}