using System.Collections.Generic;

namespace Flour.Redis.Abstractions
{
    public class RedisSettings
    {
        public int? DatabaseIndex { get; set; }
        public List<RedisHostAddressSettings> HostAddresses { get; set; }
        public string Password { get; set; }
        public string ClientName { get; set; }
        public string Master { get; set; }
    }
}