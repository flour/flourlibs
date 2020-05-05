using Flour.BrokersContracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Flour.RabbitMQ.Implementations
{
    public class RabbitMQSerializer : IBrokerSerializer
    {
        private readonly JsonSerializerSettings _settings;
        public RabbitMQSerializer(JsonSerializerSettings settings = null)
        {
            _settings = settings ?? new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public T Deserialize<T>(string value)
            => JsonConvert.DeserializeObject<T>(value, _settings);

        public object Deserialize(string value)
            => JsonConvert.DeserializeObject(value, _settings);

        public T DeserializeBinary<T>(byte[] value)
            => Deserialize<T>(Encoding.UTF8.GetString(value));

        public object DeserializeBinary(byte[] value)
            => Deserialize(Encoding.UTF8.GetString(value));

        public string Serialize<T>(T value)
            => JsonConvert.SerializeObject(value, _settings);

        public string Serialize(object value)
            => JsonConvert.SerializeObject(value, _settings);

        public byte[] SerializeBinary<T>(T value)
            => Encoding.UTF8.GetBytes(Serialize(value));

        public byte[] SerializeBinary(object value)
            => Encoding.UTF8.GetBytes(Serialize(value));
    }
}
