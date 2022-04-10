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
        for (var i = 0; i < packets; i++)                                       // 129 -> (129 - 1) / 128 + 1 = 2;
        {
            byte[] packet = PreparePacket(i);
            Write(packet.SimulateNoise(0.5));
            while (true)
            {
                char signal = ReadSignal();
                if (signal == Global.ACK) break;
                if (signal == Global.NAK) Write(packet.SimulateNoise(0.3));
                Global.Wait();
            }
        }

        EndOfTransmission();

        _printer("Transmission ended successfully");
    }

    private void EndOfTransmission()
    {
        WriteSignal(Global.EOT);
        while (true)
        {
            char signal = ReadSignal();
            if (signal == Global.ACK) break;
            if (signal == Global.NAK) WriteSignal(Global.EOT);
            Global.Wait();
        }
    }

    private byte[] PreparePacket(int i)
    {
        int blockSize = Global.BlockSize;
        int verificationSize = (int)_method;
        byte[] packet = new byte[3 + blockSize + verificationSize];
        int offset = i * blockSize;
        int counter = i + 1;
        
        packet[0] = (byte)Global.SOH;
        packet[1] = (byte)counter;
        packet[2] = (byte)(255 - counter);

        for (var j = 0; j < blockSize; j++)
        {
            byte b;
            if (_data.Length - 1 <= offset + j) b = 0;
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
            Global.Wait();
        }

        return false;
    }
}