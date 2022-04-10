using System.Diagnostics;

namespace XModem;

public class Transmitter : PortManager
{
    private byte[] _data;

    public Transmitter(int portNumber, byte[] data, VerificationMethod method, Action<object> printer) 
        : base(portNumber, method, printer)
    {
        _data = data;
    }

    public override void Process()
    {
        if (!StartTransmission()) return;

        int blockSize = Global.BlockSize;
        int dataLength = _data.Length;
        int packets = dataLength <= 0 ? 0 : (dataLength + 1) / 128;

        for (int i = 0; i < packets; i++)
        {
            int offset = i * blockSize;



        }

    }

    private bool StartTransmission()
    {
        char signal = _method switch
        {
            VerificationMethod.CheckSum => Global.NAK,
            VerificationMethod.CRC => Global.C,
            _ => throw new ArgumentOutOfRangeException(),
        };

        var startTime = Stopwatch.StartNew();
        while (startTime.Elapsed.Seconds < 60)
        {
            if (_serialPort.BytesToRead > 0)
            {
                if (ReadSignal() == signal) return true;
            }
            Thread.Sleep(10);
        }

        return false;
    }
}