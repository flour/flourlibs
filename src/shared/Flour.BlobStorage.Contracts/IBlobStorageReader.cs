using System.Threading.Tasks;

namespace Flour.BlobStorage.Contracts
{
    public interface IBlobStorageReader<in T>
        where T : IBlobReference
    {
        Task<Blob> Get(T reference);
        Task<bool> Exists(T reference);
    }
}
