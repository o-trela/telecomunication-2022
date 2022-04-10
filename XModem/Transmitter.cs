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
        
        int dataLength = _data.Length;
        int packets = dataLength <= 0 ? 0 : (dataLength - 1) / 128 + 1; // 127 -> (127 - 1) / 128 + 1 = 1;
                                                                        // 128 -> (128 - 1) / 128 + 1 = 1;
        for (var i = 0; i < packets; i++)                               // 129 -> (129 - 1) / 128 + 1 = 2;
        {
            byte[] packet = PreparePacket(i);
            Write(packet);
            while (true)
            {
                char signal = ReadSignal();
                if (signal == Global.ACK) break;
                if (signal == Global.NAK) Write(packet);
                Thread.Sleep(10);
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
            Thread.Sleep(10);
        }
    }

    private byte[] PreparePacket(int i)
    {
        int verificationSize = (int) _method;
        byte[] packet = new byte[3 + Global.BlockSize + verificationSize];
        int blockSize = Global.BlockSize;
        int offset = i * blockSize;
        int counter = i + 1;
        
        packet[0] = (byte) Global.SOH;
        packet[1] = (byte) counter;
        packet[2] = (byte)(255 - counter);

        for (var j = 0; j < Global.BlockSize; j++)
        {
            byte b = _data[offset + j];
            if (_data.Length - 1 <= offset + j)
            {
                b = 0;
            }
            packet[3 + j] = b;
        }

        byte[] verificationBytes = CalculateVerification(packet);
        for (int j = 0; j < verificationBytes.Length; j++)
        {
            packet[2 + Global.BlockSize + j] = verificationBytes[j];
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
            Thread.Sleep(10);
        }

        return false;
    }
    
    private byte[] CalculateVerification(byte[] packet)
    {
        byte[] bytes;
        switch (_method)
        {
            case VerificationMethod.CheckSum:
                bytes = CheckSum(packet[3..]);
                break;
            case VerificationMethod.CRC:
                bytes = CRC(packet[3..]);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return bytes;
    }
}