namespace Flour.Vault;

public enum AuthType
{
    Token,
    UserPass
}

public class VaultOptions
{
    public bool Enabled { get; set; }
    public string Url { get; set; }
    public string Key { get; set; }
    public string Token { get; set; }
    public AuthType AuthType { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public RenewOptions Renew { get; set; }
    public KeyValue KeyValue { get; set; }
}

public class RenewOptions
{
    public bool Enabled { get; set; }
    public int Interval { get; set; }
}

public class KeyValue
{
    public bool Enabled { get; set; }
    public int EngineVersion { get; set; } = 2;
    public string Path { get; set; }
    public string MountPoint { get; set; } = "kv";
}