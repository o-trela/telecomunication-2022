using System.Text;
using System.Text.Json;

namespace HuffmanCoding.Util;

public static class Serializer
{
    private static readonly Encoding encoding = Encoding.UTF8;

    public static byte[] Serialize<T>(T obj)
    {
        string data = JsonSerializer.Serialize(obj);
        return encoding.GetBytes(data);
    }

    public static T Deserialize<T>(byte[] data)
    {
        string stringData = encoding.GetString(data);
        T dictionary = JsonSerializer.Deserialize<T>(stringData) ?? throw new Exception("Json Deserialization went wrong!");
        return dictionary;
    }
}