using System;
using System.Threading.Tasks;

namespace Flour.Redis.DistributedLock.Contracts
{
    /// <summary>
    /// Uses a distributed lock to run a block of code
    /// </summary>
    public interface IDistributedLockExecutor
    {
        /// <summary>
        /// Executes the provided codeToExecute, applying a distributed lock with the provided key.
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="codeToExecute">The code block that will execute after the lock has been applied</param>
        /// <param name="distributedLockKey">The key to use for the distributed lock</param>
        /// <returns>A task of type <see cref="T"/></returns>
        Task<T> Execute<T>(Func<Task<T>> codeToExecute, string distributedLockKey);
    }
}