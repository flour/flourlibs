namespace Flour.BrokersContracts
{
    public interface IBrokerSerializer
    {
        string Serialize<T>(T value);
        string Serialize(object value);
        byte[] SerializeBinary<T>(T value);
        byte[] SerializeBinary(object value);
        T Deserialize<T>(string value);
        object Deserialize(string value);
        T DeserializeBinary<T>(byte[] value);
        object DeserializeBinary(byte[] value);
    }
}
