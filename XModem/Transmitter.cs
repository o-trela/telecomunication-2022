using System.Diagnostics;

namespace XModem;

public class Transmitter : PortManager
{
    private readonly byte[] _data;

    public Transmitter(string portName, byte[] data, VerificationMethod method, Action<object> printer) 
        : base(portName, method, printer)
    {
        _data = data;
    }

    public override void Process()
    {
        if (!StartTransmission()) return;

        int dataLength = _data.Length;
        int packets = dataLength <= 0 ? 0 : (dataLength - 1) / Global.BlockSize + 1; // 127 -> (127 - 1) / 128 + 1 = 1;
                                                                                     // 128 -> (128 - 1) / 128 + 1 = 1;
        for (var i = 0; i < packets; i++)                                            // 129 -> (129 - 1) / 128 + 1 = 2;
        {
            byte[] packet = PreparePacket(i);
            Write(packet/*.SimulateNoise(0.5)*/);
            _printer($"Packet #{i + 1} sent.\n");
            while (true)
            {
                char signal = ReadSignal();
                if (signal == Global.ACK)
                {
                    _printer("ACK signal received.\n");
                    break;
                }

                if (signal == Global.NAK)
                {
                    _printer($"NAK signal received. Resending packet #{i + 1}.\n");
                    Write(packet/*.SimulateNoise(0.3)*/);
                }
                Global.Wait();
            }
        }

        EndOfTransmission();

        _printer("Transmission ended successfully.\n");
    }

    private void EndOfTransmission()
    {
        WriteSignal(Global.EOT);
        _printer("EOT signal sent.\n");
        while (true)
        {
            char signal = ReadSignal();
            if (signal == Global.ACK)
            {
                _printer("ACK signal received.\n");
                break;
            }

            if (signal == Global.NAK)
            {
                _printer($"NAK signal received. Resending EOT signal.\n");
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
        int zeroCounter = 0;
        
        packet[0] = (byte)Global.SOH;
        packet[1] = (byte)counter;
        packet[2] = (byte)(255 - counter);

        for (var j = 0; j < blockSize; j++)
        {
            byte b;
            if (_data.Length <= offset + j) // TODO: packet is initially filled with zeros
                                            // TODO: so it can be just calculated how many zeros we left in packet and put count at the end
            {
                if (j == blockSize - 1) b = (byte) zeroCounter;
                else b = 0;
                zeroCounter++;
            }
            else b = _data[offset + j];

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
            _printer($"Waiting for {printableSignal} signal.\n");
            if (_serialPort.BytesToRead > 0)
            {
                if (ReadSignal() == signal)
                {
                    _printer($"{printableSignal} received. Starting transmission.\n");
                    return true;
                }
            }
            Global.Wait();
        }

        return false;
    }
}