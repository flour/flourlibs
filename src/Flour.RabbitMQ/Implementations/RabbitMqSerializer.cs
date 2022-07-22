using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Flour.BrokersContracts;

namespace Flour.RabbitMQ.Implementations;

public class RabbitMqSerializer : IBrokerSerializer
{
    private readonly JsonSerializerOptions _settings;

    public RabbitMqSerializer(JsonSerializerOptions settings = null)
    {
        _settings = settings ?? new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.Always,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public T Deserialize<T>(string value)
    {
        return JsonSerializer.Deserialize<T>(value, _settings);
    }

    public string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, _settings);
    }

    public string Serialize(object value)
    {
        return JsonSerializer.Serialize(value, _settings);
    }

    public byte[] SerializeBinary<T>(T value)
    {
        return Encoding.UTF8.GetBytes(Serialize(value));
    }

    public byte[] SerializeBinary(object value)
    {
        return Encoding.UTF8.GetBytes(Serialize(value));
    }
}