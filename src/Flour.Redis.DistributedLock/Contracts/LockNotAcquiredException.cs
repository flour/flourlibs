namespace Flour.Redis.DistributedLock.Contracts;

[Serializable]
public class LockNotAcquiredException : Exception
{
    public LockNotAcquiredException(string message)
        : base(message)
    {
    }
}