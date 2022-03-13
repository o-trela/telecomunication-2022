using System;
namespace CodeCorrection;

public class Program {
    
    static public void Main(String[] args)
    {
        var msg = new List<int>
        {
            0, 1, 0, 1, 1, 0, 1, 1
        };
        Console.Write("Message:\t");
        msg.ForEach(Console.Write);
        
        var encodedList = CodeCorrection.Encode(msg);
        Console.Write("\nEncoded:\t");
        encodedList.ForEach(Console.Write);
        
        Console.Write("\nNoise:\t \t");
        // making noise
        var noiseList = new List<int>();
        var index = 0;
        foreach (var bit in encodedList)
        {
            if (index is 7 or 3)
            {
                noiseList.Add((bit + 1) % 2);
                index++;
                continue;
            }
            noiseList.Add(bit);
            index++;
        }
        noiseList.ForEach(Console.Write);

        Console.Write("\nDecoded:\t");
        CodeCorrection.Decode(noiseList).ForEach(Console.Write);
    }
}