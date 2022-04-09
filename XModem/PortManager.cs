using System;
using System.Text;
using System.IO.Ports;

namespace XModem;

public class PortManager
{
    private SerialPort _serialPort;
    private Action<object> _printer;

    public PortManager(int portNumber, Action<object> printer)
    {
        _serialPort = new SerialPort()
        {
            PortName = $"COM{portNumber}",
            BaudRate = 115200,
            Parity = Parity.None,
            DataBits = 8,
            StopBits = StopBits.One,
        };

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

    public void Write(string? message)
    {
        if (String.IsNullOrWhiteSpace(message)) return;
        _serialPort.WriteLine(message);
    }

    public string Read()
    {
        string content = _serialPort.ReadLine();
        _printer(content + '\n');
        return content;
    }

    public static string[] GetPorts()
    {
        return SerialPort.GetPortNames();
    }
}