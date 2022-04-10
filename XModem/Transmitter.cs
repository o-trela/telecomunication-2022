using System.Diagnostics;

namespace XModem;

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
            byte[] packet = PreparePacket(i);
            Write(packet.SimulateNoise(0.5));
            _logger.Log($"Packet #{i + 1} sent.");
            while (true)
            {
                char signal = ReadSignal();
                if (signal == Global.ACK)
                {
                    _logger.Log("ACK signal received.");
                    break;
                }

                if (signal == Global.NAK)
                {
                    _logger.LogWarning($"NAK signal received. Resending packet #{i + 1}.");
                    Write(packet.SimulateNoise(0.3));
                }
                Global.Wait();
            }
        }

        EndOfTransmission();

        _logger.LogSuccess("Transmission ended successfully.");
    }

    private void EndOfTransmission()
    {
        WriteSignal(Global.EOT);
        _logger.Log("EOT signal sent.");
        while (true)
        {
            char signal = ReadSignal();
            if (signal == Global.ACK)
            {
                _logger.Log("ACK signal received.");
                break;
            }

            if (signal == Global.NAK)
            {
                _logger.LogWarning($"NAK signal received. Resending EOT signal.");
                WriteSignal(Global.EOT);
            }
            Global.Wait();
        }
    }

    private byte[] PreparePacket(int i)
    {
        int blockSize = Global.BlockSize;
        int verificationSize = (int)_method;
        byte[] packet = new byte[3 + blockSize + verificationSize];
        int offset = i * blockSize;
        int counter = (i + 1) % 255; // idk if this should work like that, now its working for more than 255 packets sent.

        packet[0] = (byte)Global.SOH;
        packet[1] = (byte)counter;
        packet[2] = (byte)(255 - counter);

        for (var j = 0; j < blockSize; j++)
        {
            if (_data.Length <= offset + j)
            {
                packet[3 + blockSize - 1] = (byte)(offset + blockSize - _data.Length);
                break;
            }
            byte b = _data[offset + j];
            packet[3 + j] = b;
        }

        byte[] verificationBytes = VerificationCode(packet);
        for (var j = 0; j < verificationBytes.Length; j++)
        {
            packet[3 + blockSize + j] = verificationBytes[j];
        }

        return packet;
    }

    private bool StartTransmission()
    {
        var (signal, printableSignal) = _method switch
        {
            VerificationMethod.CheckSum => (Global.NAK, "NAK"),
            VerificationMethod.CRC => (Global.C, "C"),
            _ => throw new ArgumentOutOfRangeException(),
        };

        var startTime = Stopwatch.StartNew();
        while (startTime.Elapsed.Seconds < 60)
        {
            _logger.Log($"Waiting for {printableSignal} signal.");
            if (_serialPort.BytesToRead > 0)
            {
                if (ReadSignal() == signal)
                {
                    _logger.LogProgress($"{printableSignal} received. Starting transmission.");
                    return true;
                }
            }
            Global.Wait();
        }

        return false;
    }
}