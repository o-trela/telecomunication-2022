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

    }
}