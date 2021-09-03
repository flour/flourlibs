using System.Threading.Tasks;

namespace Flour.BlobStorage.Contracts
{
    public interface IBlobStorageDeleter<in T, in R>
        where T : IBlobContainer
        where R : IBlobReference
    {
        Task EmptyContainer(T container);
        Task DeleteObject(R reference);
        Task DeleteContainer(T container);
    }
}