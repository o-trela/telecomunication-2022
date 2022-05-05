using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HuffmanCoding.Transmission;

public class SocketConnection
{
    private readonly string _ipAddress;
    private readonly int _port;

    public SocketConnection(string ipAddress, int port)
    {
        _ipAddress = ipAddress;
        _port = port;
    }

    public void SendData(String message)
    {
        TcpClient client = null!;
        NetworkStream stream = null!;
        
        try
        {
            client = new TcpClient(_ipAddress, _port);

            Byte[] data = Encoding.ASCII.GetBytes(message);

            stream = client.GetStream();
            stream.Write(data, 0, data.Length);

            Console.WriteLine("Sent: {0}", message);
        }
        catch (Exception e)
        {
            if (e is ArgumentNullException or SocketException)
            {
                Console.WriteLine("Exception: {0}", e);
            }
        }
        finally
        {
            client.Close();
            stream.Close();
        }

        Console.WriteLine("\n Press anything to continue...");
        Console.Read();
    }
    
    public void ReceiveData()
    {
        TcpListener server = null!;
        TcpClient client = null!;
        NetworkStream stream = null!;
        
        try
        {
            IPAddress localAddr = IPAddress.Parse(_ipAddress);
            server = new TcpListener(localAddr, _port);
            server.Start();
            
            Byte[] bytes = new Byte[256];
            String data;
            
            while(true)
            {
                Console.Write("Waiting for a connection... ");
                
                client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");
                
                stream = client.GetStream();

                int i;
                while((i = stream.Read(bytes, 0, bytes.Length))!=0)
                {
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Received: {0}", data);
                }
            }
        }
        catch(SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            server.Stop();
            client.Close();
            stream.Close();
        }

        Console.WriteLine("\nHit anything to continue...");
        Console.Read();
    }
}