namespace Flour.IOBox
{
    public class InOutSettings
    {
        public bool Enabled { get; set; }
        public int Expiry { get; set; }
        public double CheckInterval { get; set; }
        public string InboxCollection { get; set; }
        public string OutboxCollection { get; set; }
        public string Type { get; set; } 
        public bool DisableTransactions { get; set; }
    }
}