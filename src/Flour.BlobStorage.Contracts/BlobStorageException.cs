namespace Flour.BlobStorage.Contracts;

public class BlobStorageException : Exception
{
    public BlobStorageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}