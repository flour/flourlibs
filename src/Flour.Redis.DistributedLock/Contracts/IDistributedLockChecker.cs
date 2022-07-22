namespace Flour.Redis.DistributedLock.Contracts;

public interface IDistributedLockChecker
{
    /// <summary>
    ///     Check whether lock with specified key exists
    /// </summary>
    /// <param name="key">The key to use for the lock</param>
    /// <returns>True if lock exists, otherwise false</returns>
    Task<bool> Exists(string key);
}