using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flour.Vault.Services
{
    public interface IKeyValueSecrets
    {
        Task<T> GetDefaultAsync<T>();
        Task<T> GetAsync<T>(string path);
        Task<IDictionary<string, object>> GetDefaultAsync();
        Task<IDictionary<string, object>> GetAsync(string path);
    }
}