namespace Flour.Redis.Abstractions;

public class RedisHostAddressSettings
{
    public string Host { get; set; }
    public int Port { get; set; } = 6379;
    public RedisHostAddressType Type { get; set; } = RedisHostAddressType.Unspecified;

    public override string ToString()
    {
        return $"{Host}:{Port}";
    }
}