namespace Flour.Redis.DistributedLock.Contracts;

/// <summary>
///     A distributed lock which contains the original lock instance as an IDisposable
/// </summary>
public class DistributedLock : IDistributedLock
{
    private int _disposeCounter;

    public DistributedLock(string key, IDisposable redlock, bool isAcquired, string lockId)
    {
        Key = key;
        Redlock = redlock;
        IsAcquired = isAcquired;
        LockId = lockId;
    }

    public string Key { get; }
    public IDisposable Redlock { get; }
    public bool IsAcquired { get; }
    public string LockId { get; }

    public void Dispose()
    {
        Dispose(true);
    }

    public void Release()
    {
        Redlock?.Dispose();
    }

    private void Dispose(bool disposing)
    {
        if (Interlocked.Increment(ref _disposeCounter) != 1)
            return;

        if (disposing) Release();
    }
}