
using System.Net;
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
        
        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                socketConnection.SendData("Ala ma kota!");
                break;
            case "2":
                socketConnection.ReceiveData();
                break;
            default:
                throw new Exception();
        }
    }
}
