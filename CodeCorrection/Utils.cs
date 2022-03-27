using System.Globalization;
using System.Text;
using System.Collections;

namespace Zadanie_1;

public static class Utils
{   
    public static void CreateDirectory(in string dirPath)
    {
        try
        {
            if (!String.IsNullOrWhiteSpace(dirPath) && !Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public static int ReadInt32(int min = Int32.MinValue, int max = Int32.MaxValue, int def = 1)
    {
        while (true)
        {
            string? input = Console.ReadLine();

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

    public static string ToFormatted(this IEnumerable arr, string separator = " ")
    {
        var sb = new StringBuilder();
        foreach (var item in arr) sb.Append(item).Append(separator);
        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }
}