﻿using RedLockNet;

namespace Flour.Redis.DistributedLock.Internals;

public interface IRedLockConnectionFactory
{
    Task<IDistributedLockFactory> Create();
}