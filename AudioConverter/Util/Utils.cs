using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioConverter.Util;

public static class Utils
{
    public static int ReadInt32(int min = Int32.MinValue, int max = Int32.MaxValue, int def = 0, Predicate<int>? predicate = default)
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
                    if (predicate?.Invoke(output) is bool valid)
                    {
                        if (valid) return output;
                    }
                    else return output;
                }
            }

            Console.WriteLine($"Wrong input! It is supposed to be an Int32, ranging from {min} to {max}.");
        }
    }

    public static bool Between(this int val, int min, int max)
    {
        return val >= min && val <= max;
    }

    public static byte[] ToBytes(this int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        return bytes;
    }

    public static int ToInt32(this byte[] arr)
    {
        const int ByteCapacity = Byte.MaxValue + 1;
        int value = 0;
        int multiplier = 1;
        for (int i = arr.Length - 1; i >= 0; i--)
        {
            value += arr[i] * multiplier;
            multiplier *= ByteCapacity;
        }
        return value;
    }

    public static int Min(this (int, int) values)
    {
        var (one, two) = values;
        return one > two ? two : one;
    }
}
