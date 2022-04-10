using System.Collections;
using System.Globalization;
using System.Text;

namespace XModem.Helpers;

public static class Utils
{
    public static int ReadInt32(int min = Int32.MinValue, int max = Int32.MaxValue, int def = 0)
    {
        if (!def.Between(min, max)) def = min;

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

    public static string Print(this IEnumerable arr, string separator = " ")
    {
        var sb = new StringBuilder();
        foreach (var item in arr) sb.Append(item).Append(separator);
        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }

    public static T?[] SimulateNoise<T>(this T?[] data, double probability = 0.5d)
    {
        if (probability <= 0.0 || probability > 1.0) return data;

        var rng = Random.Shared;
        if (!(rng.NextDouble() < probability)) return data;

        int dataLength = data.Length;
        var dirtyData = new T?[dataLength];
        Array.Copy(data, dirtyData, dataLength);
        int index = rng.Next(dataLength);
        dirtyData[index] = default;
        return dirtyData;
    }
}