using System.Collections;
using System.Text;

namespace CodeCorrection;

public class Reader
{
    private static List<int> ReadFile(string filename)
    {
        List<int> bitsList = new();

        var bytes = File.ReadAllBytes(filename);
        var bits = new BitArray(bytes);
        
        foreach (bool bit in bits)
        {
            var converted = bit ? 1 : 0;
            bitsList.Add(converted);
        }

        return bitsList;
    }

    public static void EncodeFile(string filename)
    {
        var bitsList = ReadFile(filename);
        var buffer = new List<int>();
        var stringBuilder = new StringBuilder();
        using var writer =
            new StreamWriter(
                @"C:\Users\oskar\OneDrive\Desktop\Telekomunikacja\telecomunication-2022\CodeCorrection\encoded_text.txt");
        
        for (var i = 0; i < bitsList.Count; i++)
        {
            if (i % 8 == 0 && i != 0)
            {
                var encodedBuffer = CodeCorrection.Encode(buffer);
                encodedBuffer.ForEach(value => stringBuilder.Append(value));
                stringBuilder.AppendLine();
                buffer.Clear();
                encodedBuffer.Clear();
            }
            buffer.Add(bitsList.ElementAt(i));
        }
        
        writer.WriteLine(stringBuilder);
    }
}