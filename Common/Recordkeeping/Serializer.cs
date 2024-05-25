using MessagePack;
using System.Text.Json;

namespace Maria.Common.Recordkeeping
{
    //Its purpose is to maintain uniform serialization across the various projects
    public static class Serializer
    {
        public static JsonSerializerOptions JsonOptions { get; private set; } = new JsonSerializerOptions(){
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true
        };

        public static byte[] SerializeToBytes<T>(T obj)
        {
            return MessagePackSerializer.Serialize(obj);
        }

        public static T DeserializeBytes<T>(byte[] bytes)
        {
            return MessagePackSerializer.Deserialize<T>(bytes);
        }

        public static string SerializeToJson<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, JsonOptions);
        }

        public static T? DeserializeJson<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }

    }
}
