namespace CodeCorrection;

public class CodeCorrection
{
    private const int MsgSize = 8;
    private const int ParitySize = 4;
    private const int EncodedMsgSize = MsgSize + ParitySize;

    // TODO: Create matrix for two errors
    private static readonly int[,] H =
    {
        {1, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0},
        {0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 0, 0},
        {1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0},
        {0, 0, 1, 1, 0, 1, 0, 1, 0, 0, 0, 1}
    };

    public static List<int> Encode(List<int> msg)
    {
        for (var i = 0; i < ParitySize; i++)
        {
            var parityBit = 0;
            for (var j = 0; j < MsgSize; j++)
            {
                parityBit += H[i, j] * msg.ElementAt(j);
            }
            parityBit %= 2;
            msg.Add(parityBit);
        }
        return msg;
    }

    public static List<int> Decode(List<int> msg)
    {
        if (CheckForErrors(msg))
        {
            msg.RemoveRange(MsgSize, msg.Count - MsgSize);
            return msg;
        }

        var correctedMsg = CorrectMsg(msg);
        correctedMsg.RemoveRange(MsgSize, msg.Count - MsgSize);
        return correctedMsg;
    }
    
    private static bool CheckForErrors(IReadOnlyCollection<int> msg)
    {
        var numberSet = GetParityBits(msg);
        var valid = true;
        foreach (var number in numberSet.Where(number => number == 1))
        {
            valid = false;
        }

        return valid;
    }

    private static IReadOnlyCollection<int> GetParityBits(IReadOnlyCollection<int> msg)
    {
        var numberSet = new List<int>();
        for (var i = 0; i < ParitySize; i++)
        {
            var check = 0;
            for (var j = 0; j < EncodedMsgSize; j++)
            {
                check += H[i, j] * msg.ElementAt(j);
            }
            check %= 2;
            numberSet.Add(check);
        }
        return numberSet;
    }

    private static bool IfSingleColumn(IReadOnlyCollection<int> numberSet)
    {
        for (var i = 0; i < EncodedMsgSize; i++)
        {
            var valid = true;
            for (var j = 0; j < ParitySize; j++)
            {
                if (numberSet.ElementAt(j).Equals(H[j, i])) continue;
                valid = false;
                break;
            }
            if (valid)
            {
                return true;
            }
        }

        return false;
    }
    
    private static int ErrorBit(IReadOnlyCollection<int> numberSet)
    {
        int i;
        for (i = 0; i < EncodedMsgSize; i++)
        {
            var valid = true;
            for (var j = 0; j < ParitySize; j++)
            {
                if (numberSet.ElementAt(j).Equals(H[j, i])) continue;
                valid = false;
                break;
            }
            if (valid)
            {
                return i;
            }
        }
        return 0; // not suppose to happen
    }

    private static Tuple<int, int> ErrorBits(IReadOnlyCollection<int> numberSet)
    {
        for (var i = 0; i < EncodedMsgSize; i++)
        {
            for (var j = i + 1; j < EncodedMsgSize; j++)
            {
                for (var k = 0; k < ParitySize; k++)
                {
                    if ((H[k, i] + H[k, j]) % 2 
                        != numberSet.ElementAt(k)) break;
                    if (k >= ParitySize - 1)
                    {
                        return Tuple.Create(i, j);
                    }
                }
            }
        }

        return Tuple.Create(0, 0); // not suppose to happen
    }

    private static List<int> CorrectMsg(List<int> msg)
    {
        List<int> position = new List<int>();
        if (IfSingleColumn(GetParityBits(msg)))
        {
            position.Add(ErrorBit(GetParityBits(msg)));
        }
        else
        {
            position.Add(ErrorBits(GetParityBits(msg)).Item1);
            position.Add(ErrorBits(GetParityBits(msg)).Item2);
        }
        
        List<int> correctedMsg = new List<int>();
        var index = 0;
        foreach (var bit in msg)
        {
            if (position.Contains(index))
            {
                correctedMsg.Add((bit + 1) % 2);
                index++;
                continue;
            }
            correctedMsg.Add(bit);
            index++;
        }

        return correctedMsg;
    }
}