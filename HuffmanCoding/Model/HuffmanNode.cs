namespace HuffmanCoding.Model;

public class HuffmanNode
{
    public HuffmanNode(int freq, char character, HuffmanNode? left, HuffmanNode? right)
    {
        Freq = freq;
        Character = character;
        Left = left;
        Right = right;
    }

    public int Freq { get; }
    public char Character { get; }

    public HuffmanNode? Left { get; }
    public HuffmanNode? Right { get; }
    
}