using System;

namespace CS.SDK.Redis.DistributedLock.Internals
{
    public class RedisDistributedLockSettings
    {
        /// <summary>
        /// The TTL of the key stored in redis in seconds. Used to clean up if something went wrong with releasing the lock
        /// </summary>
        public int RedisLockTtl { get; set; } = (int) TimeSpan.FromSeconds(30).TotalSeconds;

        public TimeSpan RedisLockTtlTimespan => TimeSpan.FromSeconds(RedisLockTtl);

        /// <summary>
        /// The total time given to the lock attempt before giving up in seconds
        /// </summary>
        public int AcquisitionAttemptTotalTime { get; set; } = (int) TimeSpan.FromMinutes(1).TotalSeconds;

        public TimeSpan AcquisitionAttemptTotalTimeTimespan => TimeSpan.FromSeconds(AcquisitionAttemptTotalTime);

        /// <summary>
        /// The delay time between each lock attempt in seconds
        /// </summary>
        public int DelayBetweenAcquisitionAttempts { get; set; } = (int) TimeSpan.FromSeconds(1).TotalSeconds;

        public TimeSpan DelayBetweenAcquisitionAttemptsTimespan =>
            TimeSpan.FromSeconds(DelayBetweenAcquisitionAttempts);

        public string RedisKeyFormat { get; set; }
    }
}