namespace Flour.BlobStorage.Contracts;

public interface IBlobStorageWriterLazy<in T>
    where T : IBlobReference
{
    Task<bool> Store(T reference, Func<Task<Blob>> factory);
}