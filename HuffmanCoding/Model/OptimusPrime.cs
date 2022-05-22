using System.Text;

namespace HuffmanCoding.Model;

public static class OptimusPrime
{
    public static string Encode(string letterChain, Dictionary<char, string> dictionary)
    {
        var sb = new StringBuilder(letterChain.Length);

        foreach (var letter in letterChain.ToArray())
        {
            if (dictionary.TryGetValue(letter, out string? codedLetter))
            {
                sb.Append(codedLetter);
            }
        }

        return sb.ToString();
    }

    public static string Decode(string codedChain, Dictionary<char, string> dictionary)
    {
        var stringBuilder = new StringBuilder(codedChain.Length);

        string word = String.Empty;
        foreach (var bit in codedChain)
        {
            word += bit;
            if (dictionary.ContainsValue(word))
            {
                var key = dictionary.FirstOrDefault(x => x.Value == word).Key;
                stringBuilder.Append(key);
                word = String.Empty;
            }
        }

        return stringBuilder.ToString();
    }
}