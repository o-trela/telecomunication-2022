using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml;

namespace XModem;

public class Receiver : PortManager
{
    private byte[]? _data;
    public Receiver(int portNumber, VerificationMethod method, Action<object> printer) : base(portNumber, method, printer) {}

    public void Process()
    {
        int counter = 1;
        if (!StartTransmission())
        {
             return;           
        }
        

    }

    public bool StartTransmission()
    {
        var startTime = new Stopwatch();
        var responseTime = new Stopwatch();
        var (signal, timeout) = _method switch
        {
            VerificationMethod.CheckSum => (Global.NAK, 10),
            VerificationMethod.CRC => (Global.C, 3),
            _ => throw new ArgumentOutOfRangeException()
        };

        startTime.Start();
        while (startTime.Elapsed.Seconds < 60)
        {
            WriteSignal(signal);
            responseTime.Start();
            while (responseTime.Elapsed.Seconds < timeout)
            {
                _data = Read();
                if (_data is not null)
                {
                    return true;
                }
                Thread.Sleep(10);
            }
        }

        return false;
    }

    private void SplitData(out char signal, out int packetNumber, out int invPacketNumber, out byte[] contentData,
        out byte[] verification)
    {
        if (_data is null) throw new NullReferenceException("Data cannot be null!");
        
        signal = (char) _data[0];
        packetNumber = _data[1];
        invPacketNumber = _data[2];
        contentData = _data[3..132];
        verification = _data[132..];
    }
}