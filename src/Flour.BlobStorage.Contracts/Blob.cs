namespace Flour.BlobStorage.Contracts;

public class Blob : IDisposable
{
    private int _disposeCounter;

    public Blob(Stream stream, string contentType, IDictionary<string, string> metadata)
    {
        Stream = stream;
        ContentType = contentType;
        Metadata = metadata;
    }

    public Blob() : this(Stream.Null, string.Empty, new Dictionary<string, string>())
    {
    }

    public Stream Stream { get; }
    public string ContentType { get; }
    public IDictionary<string, string> Metadata { get; }

    public virtual void Dispose()
    {
        if (Interlocked.Increment(ref _disposeCounter) != 1)
            return;
        Stream?.Dispose();
    }
}