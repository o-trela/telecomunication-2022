using System.Runtime.Serialization.Formatters.Binary;

namespace HuffmanCoding.Transmission;

public static class Serializer
{
    public static byte[] Serialize(Object objToSerialize)
    {
        var binFormatter = new BinaryFormatter();
        var mStream = new MemoryStream();
        
        binFormatter.Serialize(mStream, objToSerialize);

        return mStream.ToArray();
    }
    
    public static T Deserialize<T>(byte[] objToDeserialize) where T : class
    {
        var mStream = new MemoryStream();
        var binFormatter = new BinaryFormatter();

        mStream.Write(objToDeserialize, 0, objToDeserialize.Length);
        mStream.Position = 0;

        return binFormatter.Deserialize(mStream) as T;
    } 
}