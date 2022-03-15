using System.Collections;
using System.Text;

namespace CodeCorrection;

public class Reader
{
    private static readonly string BaseDataDirPath = 
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "telekomunikacja_2022");
    
    private static List<int> ReadFile(string filename)
    {
        var filePath = Path.Combine(BaseDataDirPath, filename);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found!");
        }
        
        List<int> bitsList = new();

        var bytes = File.ReadAllBytes(filePath);
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
        var filePath = Path.Combine(BaseDataDirPath, filename);
        List<int> bitsList;
        try
        {
            bitsList = ReadFile(filePath);
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e.Message);
            return;
        }
        
        var buffer = new List<int>();
        var stringBuilder = new StringBuilder();
        using var writer =
            new StreamWriter(
                Path.Combine(BaseDataDirPath, "encoded_" + filename));
        
        for (var i = 0; i < bitsList.Count; i++)
        {
            // TODO: needs to be cleared
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

        // TODO: it can be done better
        stringBuilder.Remove(stringBuilder.Length - 2, 2);
        
        writer.WriteLine(stringBuilder);
        Console.WriteLine("File encoded as " + Path.Combine(BaseDataDirPath, "encoded_" + filename));
    }

    public static void DecodeFile(string filename)
    {
        var filePath = Path.Combine(BaseDataDirPath, filename);
        if (!File.Exists(filePath))
        {
            Console.WriteLine("File not found!");
            return;
        }
        
        var chunk = new List<int>();
        var bits = new List<bool>();
        foreach (var readLine in File.ReadLines(filePath))
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
        File.WriteAllBytes(Path.Combine(BaseDataDirPath, "decoded_" + filename), bytes);
        Console.WriteLine("File decoded as " + Path.Combine(BaseDataDirPath, "decoded_" + filename));
    }
}