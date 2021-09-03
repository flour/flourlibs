using System.Threading.Tasks;
using StackExchange.Redis;

namespace Flour.Redis.Abstractions
{
    public interface IConnectionMultiplexerFactory
    {
        Task<IConnectionMultiplexer> Create();
    }
}