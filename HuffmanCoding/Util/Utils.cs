using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCoding.Util;

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
}
