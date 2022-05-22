using System.Text;

namespace HuffmanCoding.Model;

public class Huffman
{
    public Dictionary<char, string> Dictionary { get; init; }

    public Huffman(string letterChain)
    {
        Dictionary = new Dictionary<char, string>();
        Dictionary<char, int> letterFreq = LettersToOccurences(letterChain);
        Node? tree = CreateTree(letterFreq);
        CreateDictionary(tree);
    }

    private void CreateDictionary(Node? node, string word = "")
    { 
        if (node?.Left is null
            && node?.Right is null
            || Char.IsLetter(node.Character))
        {
            Dictionary.Add(node!.Character, word);
            return;
        }

        CreateDictionary(node.Left, word + '0');
        CreateDictionary(node.Right, word + '1');
    }

    private static Node? CreateTree(Dictionary<char, int> baseDictionary)
    {
        Node? root = null;
        var queue = new PriorityQueue<Node?, int>();

        foreach (var pair in baseDictionary)
        {
            var huffmanNode = new Node(pair.Value, pair.Key, null, null);
            queue.Enqueue(huffmanNode, huffmanNode.Freq);
        }

        while (queue.Count > 1)
        {
            Node? left = queue.Dequeue();
            Node? right = queue.Dequeue();
            int freq = left!.Freq + right!.Freq;

            var babyRoot = new Node(freq, '-', left, right);
            queue.Enqueue(babyRoot, babyRoot.Freq);

            root = babyRoot;
        }

        return root;
    }

    private static Dictionary<char, int> LettersToOccurences(string letterChain)
    {
        var dictionary = new Dictionary<char, int>();

        foreach (var letter in letterChain)
        {
            if (!dictionary.TryAdd(letter, 1)) dictionary[letter]++;
        }

        return dictionary;
    }

    public class Node
    {
        public int Freq { get; }
        public char Character { get; }
        public Node? Left { get; }
        public Node? Right { get; }

        public Node(int freq, char character, Node? left, Node? right)
        {
            Freq = freq;
            Character = character;
            Left = left;
            Right = right;
        }
    }
}