
using HuffmanCoding.Model;

namespace HuffmanCoding;
class Program
{
    public static void Main(string[] args)
    {
        Huffman huffman = new Huffman();

        Dictionary<char, int> dictionary = new Dictionary<char, int>()
        {
            { 'a', 10 },
            { 'b', 11 },
            { 'c', 12 },
            { 'd', 17 },
            { 'e', 20 },
        };

        HuffmanNode? test = huffman.CreateTree(dictionary);
        huffman.CreateDictionary(test, "");
        foreach (var keyPair in huffman.Dictionary)
        {
            Console.WriteLine(keyPair.Key + " : " + keyPair.Value);
        }
    }
    
}
