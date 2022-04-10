using System.Diagnostics;
using XModem.Logging;
using XModem.Helpers;

namespace XModem.Transmission;

public class Receiver : PortManager
{
    private byte[]? _data;
    private readonly Action<byte[]> _writer;
    private readonly List<byte> _received;

    public Receiver(string portName, Action<byte[]> writer, VerificationMethod method, ILogger logger)
        : base(portName, method, logger)
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
                _logger.Log($"EOT signal received.");
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
                _logger.LogWarning($"Noise detected.");
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
        _logger.LogSuccess("Transmission ended successfully.");
    }

    private bool StartTransmission()
    {
        var (signal, timeout, printableSignal) = _method switch
        {
            VerificationMethod.CheckSum => (Global.NAK, 10, "NAK"),
            VerificationMethod.Crc => (Global.C, 3, "C"),
            _ => throw new ArgumentOutOfRangeException(),
        };

        var startTime = Stopwatch.StartNew();
        var responseTime = Stopwatch.StartNew();
        while (startTime.Elapsed.Seconds < 60)
        {
            WriteSignal(signal);
            _logger.Log($"{printableSignal} sent.");
            responseTime.Restart();
            while (responseTime.Elapsed.Seconds < timeout)
            {
                if (_serialPort.BytesToRead > 0)
                {
                    _logger.LogProgress("First packet discovered. Starting transmission.");
                    return true;
                }
                Global.Wait();
            }
            _logger.Log("Timeout.");
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
        bool validFlag = lastByte > 0;

        for (var i = length - 2; i > length - 2 - lastByte; i--)
        {
            if (contentData[i] == 0) continue;
            validFlag = false;
            break;
        }

        if (validFlag)
        {
            int newLength = length - (lastByte + 1); // new length is reduced by last byte value (zero counter) and this number of zeros
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
        _logger.LogWarning("Packet not acknowledged! (NAK)");
    }

    private void Acknowledged()
    {
        WriteSignal(Global.ACK);
        _logger.Log("Packet acknowledged! (ACK)");
    }

    private void AddReceived(byte[] contentData)
    {
        _received.AddRange(contentData);
        _logger.Log("Data received.");
    }

    private byte[] GetReceived()
    {
        return _received.ToArray();
    }
}