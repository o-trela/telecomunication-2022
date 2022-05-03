namespace HuffmanCoding.Model;

public class Huffman
{
    public HuffmanNode? CreateDictionary(Dictionary<char, int> baseDictionary)
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
}