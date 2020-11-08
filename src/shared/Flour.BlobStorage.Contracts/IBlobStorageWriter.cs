using System.Threading.Tasks;

namespace Flour.BlobStorage.Contracts
{
    public interface IBlobStorageWriter<in T>
        where T : IBlobReference
    {
        Task Store(T reference, Blob data);
    }
}
