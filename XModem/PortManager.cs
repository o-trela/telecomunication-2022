using System;
using System.Globalization;
using System.Text;
using System.IO.Ports;

namespace XModem;

public class PortManager
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
        int charactersRead;
        var length = 131 + (int) _method;
        byte[] data = new byte[length];
        charactersRead = _serialPort.Read(data, 0, length);
        return charactersRead > 0 ? data : null;
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