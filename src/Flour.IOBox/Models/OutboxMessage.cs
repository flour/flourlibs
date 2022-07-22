namespace Flour.IOBox.Models;

public class OutboxMessage
{
    public string Id { get; set; }
    public string OutboxId { get; set; }
    public string CorrelationId { get; set; }
    public string MessageId { get; set; }
    public object Message { get; set; }
    public string Context { get; set; }
    public string SerializedMessage { get; set; }
    public string MessageType { get; set; }
    public Dictionary<string, object> Headers { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}