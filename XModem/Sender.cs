namespace XModem;

public class Sender
{
    private readonly PortManager _portManager;

    public Sender(PortManager portManager)
    {
        _portManager = portManager;
    }

    public void SendByteStream(byte[] data)
    {
        
    }
}