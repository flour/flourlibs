using System.Threading.Tasks;

namespace Flour.Redis.DistributedLock.Contracts
{
    /// <summary>
    /// Used to acquire a lock from the implemented class
    /// </summary>
    public interface IDistributedLockAcquirer
    {
        /// <summary>
        /// Acquire the lock, optionally blocking and retrying every retry seconds until the lock is available, or wait seconds have passed
        /// </summary>
        /// <param name="key">The key to use for the lock</param>
        /// <returns>The lock information with the lock instance. This object can be used to release the lock by the caller.</returns>
        Task<IDistributedLock> AcquireLock(string key);
    }
}