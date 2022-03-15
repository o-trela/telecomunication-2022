using System.Globalization;

namespace CodeCorrection;

public static class Util
{
    public static List<int> Clone(List<int> original)
    {
        var cloned = new List<int>();
        original.ForEach(value => cloned.Add(value));
        
        return cloned;
    }
    
    public static void CreateDirectory(in string dirPath)
    {
        try
        {
            if (dirPath != null && !Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public static int ReadInt32(int min = Int32.MinValue, int max = Int32.MaxValue, int def = 0)
    {
        while (true)
        {
            string input = Console.ReadLine();

            if (input is not null)
            {
                if (String.Equals(input, String.Empty, StringComparison.OrdinalIgnoreCase))
                {
                    return def;
                }

                if (Int32.TryParse(input, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out int output) && output.Between(min, max))
                {
                    return output;
                }
            }

            Console.WriteLine($"Wrong input! It is supposed to be an Int32, ranging from {min} to {max}.");
        }
    }
    
    public static bool Between(this int val, int min, int max)
    {
        return val >= min && val <= max;
    }
}