using System;
using System.Globalization;
using System.Text;
using System.IO.Ports;

namespace XModem;

public abstract class PortManager
{
    protected SerialPort _serialPort;
    protected Action<object> _printer;
    protected VerificationMethod _method;

    public PortManager(int portNumber, VerificationMethod method, Action<object> printer)
    {
        _serialPort = new SerialPort()
        {
            PortName = $"COM{portNumber}",
            BaudRate = 115200,
            Parity = Parity.None,
            DataBits = 8,
            StopBits = StopBits.One,
        };
        
        _method = method;
        _printer = printer;
    }

    public abstract void Process();

    public void Open()
    {
        try
        {
            _serialPort.Open();
            _printer($"Otworzenie portu {_serialPort.PortName} zakończone sukcesem\n\n");
        }
        catch (Exception e)
        {
            _printer($"Otworzenie portu {_serialPort.PortName} nie powiodło się.\n{e.Message}");
            throw;
        }
    }

    public void WriteSignal(char signal) => _serialPort.Write(new[] { (byte) signal }, 0, 1);
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

    private static byte[] CheckSum(byte[] data)
    {
        byte sum = 0;
        foreach (byte b in data)
        {
            sum += b;
        }
        return new[] { sum };
    }

    private static byte[] CRC(byte[] data)
    {
        throw new NotImplementedException();
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