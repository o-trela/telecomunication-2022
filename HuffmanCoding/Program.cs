
using HuffmanCoding.Model;

namespace HuffmanCoding;
class Program
{
    public static void Main(string[] args)
    {
        Huffman huffman = new Huffman("aa   ssssbbbscs ss");
        
        foreach (var keyPair in huffman.Dictionary)
        {
            Console.WriteLine(keyPair.Key + " : " + keyPair.Value);
        }
    }
    
}
