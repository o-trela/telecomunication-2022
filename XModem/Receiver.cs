using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml;

namespace XModem;

public class Receiver : PortManager
{
    private byte[]? _data;
    private readonly Action<byte[]> _writer;
    private readonly List<byte> _received;

    public Receiver(string portName, Action<byte[]> writer, VerificationMethod method, Action<object> printer) 
        : base(portName, method, printer)
    {
        _received = new();
        _writer = writer;
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
                Global.Wait();
                _data = Read();
            }

            if (IsLastPacket())
            {
                _printer($"EOT signal received.\n");
                Acknowledged();
                break;
            }

            SplitData(out char signal,
                      out int packetNumber,
                      out int invPacketNumber,
                      out byte[] contentData,
                      out byte[] verification);

            if (signal != Global.SOH
                || packetNumber != counter
                || packetNumber + invPacketNumber != 255
                || !VerificationCode(_data).SequenceEqual(verification))
            {
                _printer($"Noise detected.\n");
                NotAcknowledged();
                continue;
            }

            contentData = CheckForComplement(contentData);

            AddReceived(contentData);
            Acknowledged();
            counter++;
            counter %= 255;
        }

        _writer(GetReceived());
        _printer("Transmission ended successfully.\n");
    }

    private bool StartTransmission()
    {
        var (signal, timeout, printableSignal) = _method switch
        {
            VerificationMethod.CheckSum => (Global.NAK, 10, "NAK"),
            VerificationMethod.CRC => (Global.C, 3, "C"),
            _ => throw new ArgumentOutOfRangeException(),
        };

        var startTime = Stopwatch.StartNew();
        var responseTime = Stopwatch.StartNew();
        while (startTime.Elapsed.Seconds < 60)
        {
            WriteSignal(signal);
            _printer($"{printableSignal} sent.\n");
            responseTime.Restart();
            while (responseTime.Elapsed.Seconds < timeout)
            {
                if (_serialPort.BytesToRead > 0)
                {
                    _printer("First packet discovered. Starting transmission.\n");
                    return true;
                }
                Global.Wait();
            }
            _printer("Timeout.\n");
        }

        return false;
    }

    private void SplitData(out char signal, out int packetNumber, out int invPacketNumber, out byte[] contentData, out byte[] verification)
    {
        if (_data is null) throw new NullReferenceException("Data cannot be null!");
        
        signal = (char)_data[0];
        packetNumber = _data[1];
        invPacketNumber = _data[2];
        contentData = _data[3..131];
        verification = _data[131..];
    }
    
    private byte[] CheckForComplement(byte[] contentData)
    {
        byte lastByte = contentData[^1];
        int length = contentData.Length;
        bool validFlag = true;
        
        for (var i = length - 2; i >= length - 1 - lastByte; i--)
        {
            if (contentData[i] == 0) continue;
            validFlag = false;
            break;
        }

        if (validFlag)
        {
            int newLength = length - (lastByte + 1);
            var clearedData = new byte[newLength];
            
            for (var i = 0; i < newLength; i++)
            {
                clearedData[i] = contentData[i];
            }

            return clearedData;
        }

        return contentData;
    }

    private bool IsLastPacket()
    {
        if (_data is null) throw new NullReferenceException("Data cannot be null!");
        return _data[0] == Global.EOT;
    }

    private void NotAcknowledged()
    {
        WriteSignal(Global.NAK);
        _printer("Packet not acknowledged! (NAK)\n");
    }

    private void Acknowledged()
    {
        WriteSignal(Global.ACK);
        _printer("Packet acknowledged! (ACK)\n");
    }

    private void AddReceived(byte[] contentData)
    {
        _received.AddRange(contentData);
        _printer("Data received.\n");
    }

    private byte[] GetReceived()
    {
        return _received.ToArray();
    }
}