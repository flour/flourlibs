using System.Threading.Tasks;

namespace Flour.BlobStorage.Contracts
{
    public interface IBlobStorageDeleter<in T>
        where T : IBlobContainer
    {
        Task EmptyContainer(IBlobContainer container);
        Task DeleteContainer(IBlobContainer container);
    }
}
