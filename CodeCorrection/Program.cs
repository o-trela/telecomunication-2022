using System;
namespace CodeCorrection;

public class Program {
    
    static public void Main(String[] args)
    {
        var msg = new List<int>
        {
            0, 0, 0, 1, 1, 0, 1, 1
        };
        Console.Write("Message: ");
        msg.ForEach(Console.Write);
        
        var encodedList = CodeCorrection.Encode(msg);
        Console.Write("\nEncoded: ");
        encodedList.ForEach(Console.Write);
        
        Console.Write("\nNoise: ");
        // making noise
        var noiseList = new List<int>();
        var index = 0;
        foreach (var bit in encodedList)
        {
            if (index is 4 or 1)
            {
                noiseList.Add((bit + 1) % 2);
                index++;
                continue;
            }
            noiseList.Add(bit);
            index++;
        }
        noiseList.ForEach(Console.Write);

        Console.Write("\nDecoded: ");
        CodeCorrection.Decode(noiseList).ForEach(Console.Write);
    }
}