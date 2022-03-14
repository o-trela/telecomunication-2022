namespace CodeCorrection;

public class Util
{
    public static List<int> Clone(List<int> original)
    {
        var cloned = new List<int>();
        original.ForEach(value => cloned.Add(value));
        
        return cloned;
    }
}