namespace XModem.Helpers;

public static class Global
{
    public static readonly int HeaderSize = 3;
    public static readonly int BlockSize = 128;

    public static readonly char SOH = (char)0x01;
    public static readonly char EOT = (char)0x04;
    public static readonly char ACK = (char)0x06;
    public static readonly char NAK = (char)0x15;
    public static readonly char CAN = (char)0x18;
    public static readonly char BEL = (char)0x07; // Just For Fun
    public static readonly char C = 'C';

    public static readonly double Interference = 0.4;

    public static void Wait(int miliseconds = 100)
    {
        Thread.Sleep(miliseconds);
    }
}
