namespace CodeCorrection;

public class CodeCorrection
{
    private static readonly int msgSize = 4;
    private static readonly int paritySize = 3;
    private static readonly int encodedMsgSize = msgSize + paritySize;

    
    private static readonly int[,] H =
    {
        {1, 1, 1, 0, 1, 0, 0},
        {0, 1, 1, 1, 0, 1, 0},
        {1, 1, 0, 1, 0, 0, 1}
    };

    public static List<int> Encode(List<int> msg)
    {
        for (var i = 0; i < 3; i++)
        {
            var parityBit = 0;
            for (var j = 0; j < 4; j++)
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
            msg.RemoveRange(msgSize, msg.Count - msgSize);
            return msg;
        }

        var correctedMsg = CorrectMsg(msg);
        correctedMsg.RemoveRange(msgSize, msg.Count - msgSize);
        return correctedMsg;
    }
    
    private static bool CheckForErrors(List<int> msg)
    {
        var numberSet = GetResult(msg);
        var valid = true;
        foreach (var number in numberSet.Where(number => number == 1))
        {
            valid = false;
        }

        return valid;
    }

    private static IReadOnlyCollection<int> GetResult(IReadOnlyCollection<int> msg)
    {
        var numberSet = new List<int>();
        for (var i = 0; i < paritySize; i++)
        {
            var check = 0;
            for (var j = 0; j < encodedMsgSize; j++)
            {
                check += H[i, j] * msg.ElementAt(j);
            }
            check %= 2;
            numberSet.Add(check);
        }
        return numberSet;
    }
    
    private static int ErrorBit(IReadOnlyCollection<int> numberSet)
    {
        int i;
        for (i = 0; i < encodedMsgSize; i++)
        {
            var valid = true;
            for (var j = 0; j < paritySize; j++)
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
    
    private static List<int> CorrectMsg(List<int> msg)
    {
        var position = ErrorBit(GetResult(msg));
        List<int> correctedMsg = new List<int>();
        var index = 0;
        foreach (var bit in msg)
        {
            if (index.Equals(position))
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

    /*public static int ErrorBit(List<int> list)
    {
        var parityList = list
            .Select((value, i) => (value, i))
            .Where((tuple, i) => tuple.value == 1)
            .ToList();

        int result = parityList.First().i;
        foreach (var (element, i) in parityList)
        {
            result ^= i;
        }

        return result;
    }*/
}