namespace Flour.ServiceBus.Abstractions.Helpers;

public static class MessageExtensions
{
    public static IDictionary<string, object> WithHeaders(this string messageId)
    {
        return string.IsNullOrWhiteSpace(messageId)
            ? null
            : new Dictionary<string, object> {{InternalConstants.MessageIdHeader, messageId}};
    }
}