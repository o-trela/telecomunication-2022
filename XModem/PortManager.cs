using System.IO.Ports;

namespace XModem;

public abstract class PortManager
{
    protected readonly SerialPort _serialPort;
    protected readonly ILogger _logger;
    protected readonly VerificationMethod _method;

    protected PortManager(string portName, VerificationMethod method, ILogger logger)
    {
        _serialPort = new SerialPort()
        {
            PortName = portName,
            BaudRate = 115200,
            Parity = Parity.None,
            DataBits = 8,
            StopBits = StopBits.One,
        };

        _method = method;
        _logger = logger;
    }

    public abstract void Process();

    public void Open()
    {
        try
        {
            _serialPort.Open();
            _logger.LogProgress($"Otworzenie portu {_serialPort.PortName} zakończone sukcesem\n");
        }
        catch (Exception e)
        {
            _logger.LogError($"Otworzenie portu {_serialPort.PortName} nie powiodło się.\n{e.Message}");
            throw;
        }
    }

    public void WriteSignal(char signal) => _serialPort.Write(new[] { (byte)signal }, 0, 1);

    public char ReadSignal() => (char)_serialPort.ReadChar();

    public void Write(byte[] data) => _serialPort.Write(data, 0, data.Length);

    public byte[]? Read()
    {
        int length = _serialPort.BytesToRead;

        if (length <= 0) return null;

        byte[] data = new byte[length];
        int charactersRead = _serialPort.Read(data, 0, length);
        return charactersRead > 0 ? data : null;
    }

    protected byte[] VerificationCode(byte[] data)
    {
        return _method switch
        {
            VerificationMethod.CheckSum => CheckSum(data),
            VerificationMethod.CRC => CRC(data),
            _ => throw new ArgumentException("Ale heca nie ma mnie"),
        };
    }

    protected static byte[] CheckSum(byte[] data)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (data.Length < Global.BlockSize + 1) throw new ArgumentException($"Data Length is too short ({data.Length})");

        int offset = Global.HeaderSize;

        byte sum = 0;
        for (int i = offset; i < Global.BlockSize + offset; i++)
        {
            byte b = data[i];
            sum += b;
        }
        return new[] { sum };
    }

    public static byte[] CRC(byte[] data)
    {
        const int Polynomial = 0x1021;
        const int Bits = 8;

        int offset = Global.HeaderSize;

        int crc = 0;
        for (int i = offset; i < Global.BlockSize + offset; i++)
        {
            crc ^= data[i] << Bits;
            for (int j = 0; j < Bits; j++)
            {
                crc <<= 1;
                if ((crc & 0x8000) != 0) crc ^= Polynomial;
            }
        }
        return new[] { (byte)crc, (byte)(crc >> 8) };
    }

    public static string[] GetPorts()
    {
        return SerialPort.GetPortNames();
    }

    public enum VerificationMethod
    {
        CheckSum = 1,
        CRC = 2,
    }
}