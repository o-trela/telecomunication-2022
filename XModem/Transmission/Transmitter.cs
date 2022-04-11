using System.Diagnostics;
using XModem.Helpers;
using XModem.Logging;

namespace XModem.Transmission;

public class Transmitter : PortManager
{
    private readonly byte[] _data;

    public Transmitter(string portName, byte[] data, VerificationMethod method, ILogger logger)
        : base(portName, method, logger)
    {
        _data = data;
    }

    public override void Process()
    {
        if (!StartTransmission()) return;

        int dataLength = _data.Length;
        int packets = dataLength <= 0 ? 0 : (dataLength - 1) / Global.BlockSize + 1;
        ;
        for (var i = 0; i < packets; i++)
        {
            _logger.Log($"Packet #{i + 1}");
            byte[] packet = PreparePacket(i);
            Write(packet.SimulateNoise(Global.Interference));
            while (true)
            {
                char signal = ReadSignal();
                if (signal == Global.ACK || signal == Global.C)
                {
                    _logger.Log("\tACK signal received.");
                    break;
                }

                if (signal == Global.NAK)
                {
                    _logger.LogWarning($"\tNAK signal received. Resending packet.");
                    Write(packet.SimulateNoise(Global.Interference));
                }
            }
        }

        EndOfTransmission(packets);
    }

    private bool StartTransmission()
    {
        var (signal, printableSignal) = _method switch
        {
            VerificationMethod.CheckSum => (Global.NAK, "NAK"),
            VerificationMethod.Crc => (Global.C, "C"),
            _ => throw new ArgumentOutOfRangeException(),
        };

        _logger.Log($"Waiting for {printableSignal} signal...");

        var startTime = Stopwatch.StartNew();
        while (startTime.Elapsed.Seconds < 60)
        {
            if (_serialPort.BytesToRead > 0)
            {
                if (ReadSignal() == signal)
                {
                    _serialPort.DiscardInBuffer();
                    _logger.LogProgress($"{printableSignal} received. Starting transmission.");
                    return true;
                }
            }
        }

        return false;
    }

    private void EndOfTransmission(int packetNumber)
    {
        _logger.Log($"Packet #{packetNumber + 1}");
        WriteSignal(Global.EOT);
        _logger.Log("\tEOT signal sent.");
        while (true)
        {
            char signal = ReadSignal();
            if (signal == Global.ACK)
            {
                _logger.Log("\tACK signal received.");
                break;
            }

            if (signal == Global.NAK)
            {
                _logger.LogWarning("\tNAK signal received. Resending EOT signal.");
                WriteSignal(Global.EOT);
            }
        }

        _logger.LogSuccess("Transmission ended successfully.");
        _serialPort.Dispose();
    }

    private byte[] PreparePacket(int i)
    {
        int blockSize = Global.BlockSize;
        int dataLength = _data.Length;
        int verificationSize = (int)_method;
        byte[] packet = new byte[3 + blockSize + verificationSize];
        int offset = i * blockSize;
        int counter = (i + 1) % 255;

        packet[0] = (byte)Global.SOH;
        packet[1] = (byte)counter;
        packet[2] = (byte)(255 - counter);

        for (var j = 0; j < blockSize; j++)
        {
            if (dataLength <= offset + j)
            {
                packet[3 + blockSize - 1] = (byte)(offset + blockSize - dataLength - 1);
                break;
            }
            packet[3 + j] = _data[offset + j];
        }

        byte[] verificationBytes = VerificationCode(packet);
        for (var j = 0; j < verificationBytes.Length; j++)
        {
            packet[3 + blockSize + j] = verificationBytes[j];
        }

        return packet;
    }
}