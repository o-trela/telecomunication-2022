
using System.Net;
using HuffmanCoding.Model;
using HuffmanCoding.Transmission;
using HuffmanCoding.Util;

namespace HuffmanCoding;
class Program
{
    public static void Main(string[] args)
    {
        const string ipAddress = "127.0.0.1";
        const int port = 8080;

        string str = "Ala ma kota, komputer, dzbanek i wiele innych bardzo ciekawym rzeczy, ale nie zaprezentuje nam ich, bo jest bardzo wstydliwa i nie ma na to ochoty";
        var huffman = new Huffman(str);
        
        foreach (var keyPair in huffman.Dictionary)
        {
            Console.WriteLine(keyPair.Key + " --> " + keyPair.Value);
        }

        string encoded = OptimusPrime.Encode(str, huffman.Dictionary);
        string decoded = OptimusPrime.Decode(encoded, huffman.Dictionary);
        if (!String.Equals(str, decoded)) throw new Exception("Compression or decompression went wrong!");
        /*Console.WriteLine(encoded);
        Console.WriteLine(decoded);*/
        Console.WriteLine($"Compressed file is {100.0 * encoded.Length / (str.Length * 8.0 ):n2}% of the original size.");
        
        var socketConnection = new SocketConnection(ipAddress, port);

        int choice = Utils.ReadInt32(1, 2, def: 1);
        switch (choice)
        {
            case 1:
                socketConnection.SendData(str);
                break;
            case 2:
                socketConnection.ReceiveData();
                break;
            default:
                throw new Exception();
        }
    }
}
