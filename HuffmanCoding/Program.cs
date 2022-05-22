using HuffmanCoding.Dao;
using HuffmanCoding.Model;
using HuffmanCoding.Transmission;
using HuffmanCoding.Util;

namespace HuffmanCoding;

class Program
{
    public static void Main()
    {
        const string ipAddress = "127.0.0.1";
        const int port = 8080;
        const string sampleText = "Ala ma kota, komputer, dzbanek i wiele innych bardzo ciekawym rzeczy, ale nie zaprezentuje nam ich, bo jest bardzo wstydliwa i nie ma na to ochoty";

        FileManager.EnsureDirectoryIsValid();

        int medium;
        int action;
        string data;
        FileManager? fileManager;
        Huffman? huffman;
        Action<string> writer;


        Console.WriteLine("Want do you want to do?\n" +
            "1. Send\n" +
            "2. Receive");
        Console.Write("Choice: ");
        action = Utils.ReadInt32(1, 2, 1);
        Console.WriteLine("1. Text\n" +
             "2. File");
        Console.Write("Choice: ");
        medium = Utils.ReadInt32(1, 2, 1) + 2;

        int result = action * medium;
        var socketConnection = new SocketConnection(ipAddress, port);

        switch (result) // send text = 3, send file = 4, receive text = 6, receive file = 8
        {
            case 3:
                Console.Write("Text: ");
                string? text = Console.ReadLine();
                data = String.IsNullOrWhiteSpace(text) ? sampleText : text;
                goto sending;
            case 4:
                fileManager = PromptFilename(true);
                data = fileManager.ReadString();
                goto sending;

            sending:
                huffman = new Huffman(data);

                foreach (var keyPair in huffman.Dictionary)
                {
                    Console.WriteLine(keyPair.Key + " --> " + keyPair.Value);
                }

                string encoded = OptimusPrime.Encode(data, huffman.Dictionary);
                string decoded = OptimusPrime.Decode(encoded, huffman.Dictionary);
                if (!String.Equals(data, decoded)) throw new Exception("Compression or decompression went wrong!");
                Console.WriteLine($"Compressed file is {100.0 * encoded.Length / (data.Length * 8.0):n2}% of the original size.");

                var dictionary = huffman?.Dictionary ?? throw new Exception("");
                socketConnection.SendData(data, dictionary);
                break;
            case 6:
                writer = Console.WriteLine;
                goto receiving;
            case 8:
                fileManager = PromptFilename(true);
                writer = fileManager.Write;
                goto receiving;
            receiving:
                var message = socketConnection.ReceiveData();
                writer(message);
                break;
            default:
                break;
        }

        FileManager PromptFilename(bool receiver)
        {
            Console.Write("Filename: ");
            string? filename = Console.ReadLine();
            filename = String.IsNullOrWhiteSpace(filename) ? (receiver ? "result.txt" : "sampletext.txt") : filename;
            return new FileManager(filename);
        }
    }
}
