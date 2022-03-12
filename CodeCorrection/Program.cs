using System;
namespace CodeCorrection;

public class Program {
    
    static public void Main(String[] args)
    {
        List<int> msg = new List<int>
        {
            0, 0, 0, 1, 1, 0, 1, 1
        };
        Console.Write("Message: ");
        msg.ForEach(i => Console.Write(i));
        
        var encodedList = CodeCorrection.Encode(msg);
        Console.Write("\nEncoded: ");
        encodedList.ForEach(i => Console.Write(i));
        
        Console.Write("\nNoise: ");
        // making noise
        List<int> noiseList = new List<int>();
        int index = 0;
        foreach (var bit in encodedList)
        {
            if (index == 3)
            {
                noiseList.Add((bit + 1) % 2);
                index++;
                continue;
            }
            noiseList.Add(bit);
            index++;
        }
        noiseList.ForEach(i => Console.Write(i));

        Console.Write("\nDecoded: ");
        CodeCorrection.Decode(msg).ForEach(i => Console.Write(i));
    }
}