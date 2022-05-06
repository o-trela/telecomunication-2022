
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using HuffmanCoding.Model;
using HuffmanCoding.Transmission;

namespace HuffmanCoding;
class Program
{
    public static void Main(string[] args)
    {
        string str = "Ala ma kota";
        Huffman huffman = new Huffman(str);
        
        foreach (var keyPair in huffman.Dictionary)
        {
            Console.WriteLine(keyPair.Key + " : " + keyPair.Value);
        }

        OptimusPrime transformer = new OptimusPrime();
        var encoded = transformer.Encode(str, huffman.Dictionary);
        var decoded = transformer.Decode(encoded, huffman.Dictionary);
        Console.WriteLine(encoded);
        Console.WriteLine(decoded);


        const string ipAddress = "127.0.0.1";
        const int port = 8080;

        SocketConnection socketConnection = new SocketConnection(ipAddress, port);

        var bytes = Serializer.Serialize(huffman.Dictionary);
        var dict = Serializer.Deserialize<Dictionary<char, string>>(bytes);

        foreach (var keyPair in dict)
        {
            Console.WriteLine(keyPair.Key + " : " + keyPair.Value);
        }
        
        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                socketConnection.SendData(huffman.Dictionary);
                break;
            case "2":
                Dictionary<char, string> data = socketConnection.ReceiveData<Dictionary<char, string>>();
                foreach (var keyPair in data)
                {
                    Console.WriteLine(keyPair.Key + " : " + keyPair.Value);
                }
                break;
            default:
                throw new Exception();
        }
    }
}
