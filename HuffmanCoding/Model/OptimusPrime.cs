using System.Text;

namespace HuffmanCoding.Model;

public class OptimusPrime
{
    public string Encode(string letterChain, Dictionary<char, string> dictionary)
    {
        StringBuilder stringBuilder = new StringBuilder();

        foreach (var letter in letterChain.ToArray())
        {
            if (dictionary.TryGetValue(letter, out string codedLetter))
            {
                stringBuilder.Append(codedLetter);
            }
        }

        return stringBuilder.ToString();
    }

    public string Decode(string codedChain, Dictionary<char, string> dictionary)
    {
        StringBuilder helper = new StringBuilder();
        StringBuilder stringBuilder = new StringBuilder();

        foreach (var bit in codedChain)
        {
            helper.Append(bit);
            if (dictionary.ContainsValue(helper.ToString()))
            {
                var key = dictionary.FirstOrDefault(x => x.Value == helper.ToString()).Key;
                stringBuilder.Append(key);
                helper.Clear();
            }
        }

        return stringBuilder.ToString();
    }
}