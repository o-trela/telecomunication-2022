
using HuffmanCoding.Model;

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
    }
    
}
