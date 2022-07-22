namespace Flour.BlobStorage.Contracts;

public class BlobNotFoundException : Exception
{
    public BlobNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}