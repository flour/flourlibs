namespace Flour.BlobStorage.Contracts;

public class BlobContainer : IBlobContainer
{
    public BlobContainer(string folder)
    {
        if (string.IsNullOrWhiteSpace(folder))
            throw new ArgumentNullException(nameof(folder));

        Id = folder;
    }

    public string Id { get; }
}