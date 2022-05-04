using System.Text;

namespace HuffmanCoding.Model;

public class Huffman
{
    public Huffman()
    {
        Dictionary = new Dictionary<char, string>();
    }

    public Dictionary<char, string> Dictionary { get; }
    public HuffmanNode? CreateTree(Dictionary<char, int> baseDictionary)
    {
        PriorityQueue<HuffmanNode?, int> queue 
            = new PriorityQueue<HuffmanNode?, int>();

        foreach (var pair in baseDictionary)
        {
            HuffmanNode? huffmanNode 
                = new HuffmanNode(pair.Value, pair.Key, null, null);
            
            queue.Enqueue(huffmanNode, huffmanNode.Freq);
        }

        HuffmanNode? root = null;

        while (queue.Count > 1)
        {
            HuffmanNode? left = queue.Dequeue();
            HuffmanNode? right = queue.Dequeue();

            int freq = left.Freq + right.Freq;
            
            HuffmanNode? babyRoot = new HuffmanNode(freq, '-', left, right);

            root = babyRoot;
            
            queue.Enqueue(babyRoot, babyRoot.Freq);
        }

        return root;
    }

    public void CreateDictionary(HuffmanNode? node, string word)
    { 
        if (node.Left == null
            && node.Right == null
            && Char.IsLetter(node.Character))
        {
            Dictionary.Add(node.Character, word);
            return;
        }

        CreateDictionary(node.Left, word + '0');
        CreateDictionary(node.Right, word + '1');
    }
}