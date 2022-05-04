﻿namespace HuffmanCoding.Model;

public class HuffmanNode
{
    public HuffmanNode(int freq, char character, HuffmanNode? left, HuffmanNode? right)
    {
        Freq = freq;
        Character = character;
        Left = left;
        Right = right;
    }

    public int Freq { get; set; }
    public char Character { get; set; }

    public HuffmanNode? Left { get; set; }
    public HuffmanNode? Right { get; set; }
    
}