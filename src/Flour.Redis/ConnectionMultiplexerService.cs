using System.Threading.Tasks;
using Flour.Redis.Abstractions;
using StackExchange.Redis;

namespace Flour.Redis
{
    internal class ConnectionMultiplexerService : IConnectionMultiplexerService
    {
        public async Task<IConnectionMultiplexer> GetConnection(ConfigurationOptions configuration)
        {
            return await ConnectionMultiplexer.ConnectAsync(configuration).ConfigureAwait(false);
        }

        public async Task<IConnectionMultiplexer> GetConnection(string serviceName, params string[] hostAddresses)
        {
            return await ConnectionMultiplexer
                .ConnectAsync($"{string.Join(',', hostAddresses)},serviceName={serviceName},allowAdmin=true")
                .ConfigureAwait(false);
        }
        
    }
}