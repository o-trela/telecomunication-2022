using System.Diagnostics;
using XModem.Helpers;
using XModem.Logging;

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
        bool newPacket = true;
        int packet = 1;
        while (true)
        {
            if (newPacket) _logger.Log($"Packet #{packet}");

            _data = null;
            while (_data is null)
            {
                _data = Read();
            }

            if (IsLastPacket())
            {
                _logger.Log($"\tEOT signal received.");
                Acknowledged();
                break;
            }

            SplitData(out char signal,
                      out int packetNumber,
                      out int invPacketNumber,
                      out byte[] contentData,
                      out byte[] verification);

            if (signal != Global.SOH
                || packetNumber != packet % 255
                || packetNumber + invPacketNumber != 255
                || !VerificationCode(_data).SequenceEqual(verification))
            {
                NotAcknowledged();
                newPacket = false;
                continue;
            }

            contentData = CheckForComplement(contentData);

            Acknowledged();
            AddReceived(contentData);
            packet++;
            newPacket = true;
        }

        EndTransmission();
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
                    _logger.LogProgress("\nFirst packet discovered. Starting transmission.");
                    return true;
                }
            }
            _logger.Log("Timeout.");
        }

        _serialPort.DiscardOutBuffer();
        return false;
    }

    private void EndTransmission()
    {
        _writer(GetReceived());
        _logger.LogSuccess("Transmission ended successfully.");
        _serialPort.Dispose();
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

    private static byte[] CheckForComplement(byte[] contentData)
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
        _logger.LogWarning("\tPacket not acknowledged! (NAK)");
    }

    private void Acknowledged()
    {
        WriteSignal(Global.ACK);
        _logger.Log("\tPacket acknowledged! (ACK)");
    }

    private void AddReceived(byte[] contentData)
    {
        _received.AddRange(contentData);
        _logger.Log("\tData saved.");
    }

    private byte[] GetReceived()
    {
        return _received.ToArray();
    }
}