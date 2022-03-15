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
            // needs to be cleared
            if (i == bitsList.Count - 1)
            {
                buffer.Add(bitsList.ElementAt(i));
            }
            
            if (i % 8 == 0 && i != 0 || i == bitsList.Count - 1)
            {
                var encodedBuffer = CodeCorrection.Encode(buffer);
                encodedBuffer.ForEach(value => stringBuilder.Append(value));
                stringBuilder.AppendLine();
                buffer.Clear();
                encodedBuffer.Clear();
            }

            if (i != bitsList.Count - 1)
            {
                buffer.Add(bitsList.ElementAt(i));
            }
        }
        
        writer.WriteLine(stringBuilder);
    }

    public static void DecodeFile(string filename)
    {
        var chunk = new List<int>();
        var bits = new List<bool>();
        foreach (var readLine in File.ReadLines(filename))
        {
            for (var i = 0; i < readLine.Length; i++)
            {
                var bit = readLine.ElementAt(i);
                var bitInt = Int32.Parse(bit.ToString());
                chunk.Add(bitInt);
            }
            var decodedChunk = CodeCorrection.Decode(chunk);
            decodedChunk.ForEach(value => bits.Add(value == 1));
            
            chunk.Clear();
            decodedChunk.Clear();
        }

        var bitArray = new BitArray(bits.ToArray());
        byte[] bytes = new byte[bitArray.Length / 8];
        bitArray.CopyTo(bytes, 0);
        File.WriteAllBytes(@"C:\Users\oskar\OneDrive\Desktop\Telekomunikacja\telecomunication-2022\CodeCorrection\decoded_tele.png", bytes);
    }
}