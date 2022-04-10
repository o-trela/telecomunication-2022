using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml;

namespace XModem;

public class Receiver : PortManager
{
    private byte[]? _data;
    private List<byte> received;

    public Receiver(int portNumber, VerificationMethod method, Action<object> printer) 
        : base(portNumber, method, printer)
    {
        received = new();
    }

    public override void Process()
    {
        if (!StartTransmission()) return;

        int counter = 1;
        while (true)
        {
            _data = null;
            while (_data is null)
            {
                _data = Read();
                Thread.Sleep(10);
            }

            if (IsLastPacket())
            {
                Acknowledged();
                break;
            }

            SplitData(out char signal,
                      out int packetNumber,
                      out int invPacketNumber,
                      out byte[] contentData,
                      out byte[] verification);

            if (signal != Global.SOH
                || packetNumber == counter
                || packetNumber + invPacketNumber != 255
                || !VerificationCode(contentData).SequenceEqual(verification))
            {
                NotAcknowledged();
                continue;
            }

            AddReceived(contentData);
            Acknowledged();
            counter++;
        }

        _printer("Transmition ended succesfully");
    }

    private bool IsLastPacket()
    {
        if (_data is null) throw new NullReferenceException("Data cannot be null!");
        return _data[0] == Global.EOT;
    }

    private void NotAcknowledged()
    {
        WriteSignal(Global.NAK);
        _printer("Packet not acknowledged!\n");
    }

    private void Acknowledged()
    {
        WriteSignal(Global.ACK);
        _printer("Packet acknowledged!\n");
    }

    private void AddReceived(byte[] contentData)
    {
        received.AddRange(contentData);
        _printer("Data received.\n");
    }

    public bool StartTransmission()
    {
        var (signal, timeout) = _method switch
        {
            VerificationMethod.CheckSum => (Global.NAK, 10),
            VerificationMethod.CRC => (Global.C, 3),
            _ => throw new ArgumentOutOfRangeException(),
        };

        var startTime = Stopwatch.StartNew();
        var responseTime = Stopwatch.StartNew();
        while (startTime.Elapsed.Seconds < 60)
        {
            WriteSignal(signal);
            responseTime.Restart();
            while (responseTime.Elapsed.Seconds < timeout)
            {
                if (_serialPort.BytesToRead > 0) return true;
                Thread.Sleep(10);
            }
        }

        return false;
    }

    private void SplitData(out char signal, out int packetNumber, out int invPacketNumber, out byte[] contentData, out byte[] verification)
    {
        if (_data is null) throw new NullReferenceException("Data cannot be null!");
        
        signal = (char) _data[0];
        packetNumber = _data[1];
        invPacketNumber = _data[2];
        contentData = _data[3..131];
        verification = _data[131..];
    }
}