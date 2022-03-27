namespace Zadanie_1;

public static class Code
{
    private const int MsgSize = 8;
    private const int ParitySize = 8;
    private const int EncodedMsgSize = MsgSize + ParitySize;
    private static readonly int[,] H =
    {
        {1, 1, 1, 1, 0, 0, 0, 0,  1, 0, 0, 0, 0, 0, 0, 0},
        {1, 1, 0, 0, 1, 1, 0, 0,  0, 1, 0, 0, 0, 0, 0, 0},
        {1, 0, 1, 0, 1, 0, 1, 0,  0, 0, 1, 0, 0, 0, 0, 0},
        {0, 1, 0, 1, 0, 1, 1, 0,  0, 0, 0, 1, 0, 0, 0, 0},
        {1, 1, 1, 0, 1, 0, 0, 1,  0, 0, 0, 0, 1, 0, 0, 0},
        {1, 0, 0, 1, 0, 1, 0, 1,  0, 0, 0, 0, 0, 1, 0, 0},
        {0, 1, 1, 1, 1, 0, 1, 1,  0, 0, 0, 0, 0, 0, 1, 0},
        {1, 1, 1, 0, 0, 1, 1, 1,  0, 0, 0, 0, 0, 0, 0, 1},
    };

    public static List<int> Encode(List<int> msg)
    {
        var encodedMsg = new List<int>(msg);
        for (var i = 0; i < ParitySize; i++)
        {
            int parityBit = 0;
            for (var j = 0; j < MsgSize; j++)
            {
                parityBit ^= H[i, j] & msg[j];
            }
            encodedMsg.Add(parityBit);
        }
        return encodedMsg;
    }

    public static List<int> Decode(List<int> msg)
    {
        List<int> correctedMsg;

        if (IsCorrect(msg)) correctedMsg = new List<int>(msg);
        else correctedMsg = CorrectMsg(msg);

        correctedMsg.RemoveRange(MsgSize, msg.Count - MsgSize);
        return correctedMsg;
    }
    
    private static bool IsCorrect(List<int> msg)
    {
        var numberSet = GetParityBits(msg);
        return !numberSet.Contains(1);
    }

    private static List<int> GetParityBits(List<int> msg)
    {
        var numberSet = new List<int>();
        for (var i = 0; i < ParitySize; i++)
        {
            int check = 0;
            for (var j = 0; j < EncodedMsgSize; j++)
            {
                check ^= H[i, j] & msg[j];
            }
            numberSet.Add(check);
        }
        return numberSet;
    }

    private static bool IfSingleColumn(List<int> numberSet)
    {
        for (var i = 0; i < EncodedMsgSize; i++)
        {
            bool valid = true;
            for (var j = 0; j < ParitySize; j++)
            {
                if (numberSet[j] != H[j, i])
                {
                    valid = false;
                    break;
                }
            }
            if (valid) return true;
        }
        return false;
    }
    
    private static int ErrorBit(List<int> numberSet)
    {
        for (var i = 0; i < EncodedMsgSize; i++)
        {
            bool valid = true;
            for (var j = 0; j < ParitySize; j++)
            {
                if (numberSet[j] != H[j, i])
                {
                    valid = false;
                    break;
                }
            }
            if (valid) return i;
        }
        return -1;
    }

    private static (int bit1, int bit2) ErrorBits(List<int> numberSet)
    {
        for (var i = 0; i < EncodedMsgSize; i++)
        {
            for (var j = i + 1; j < EncodedMsgSize; j++)
            {
                for (var k = 0; k < ParitySize; k++)
                {
                    if ((H[k, i] ^ H[k, j]) != numberSet[k]) break;

                    if (k >= ParitySize - 1) return (i, j);
                }
            }
        }
        return (-1, -1);
    }

    private static List<int> CorrectMsg(List<int> msg)
    {
        var positions = new List<int>();
        var parityBits = GetParityBits(msg);

        if (IfSingleColumn(parityBits))
        {
            int bit = ErrorBit(parityBits);
            positions.Add(bit);
        }
        else
        {
            (int bit1, int bit2) = ErrorBits(parityBits);
            positions.Add(bit1);
            positions.Add(bit2);
        }
        
        var correctedMsg = new List<int>(msg);
        foreach (var i in positions) correctedMsg[i] ^= 1;

        return correctedMsg;
    }
}