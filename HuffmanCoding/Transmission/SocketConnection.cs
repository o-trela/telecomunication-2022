using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HuffmanCoding.Transmission;

public class SocketConnection
{
    private readonly Encoding asciiEndocing = Encoding.ASCII;
    private readonly string _ipAddress;
    private readonly int _port;

    public SocketConnection(string ipAddress, int port)
    {
        _ipAddress = ipAddress;
        _port = port;
    }

    public void SendData(String message)
    {
        try
        {
            byte[] data = asciiEndocing.GetBytes(message);

            using var client = new TcpClient(_ipAddress, _port);
            using NetworkStream stream = client.GetStream();

            stream.Write(data, 0, data.Length);
            Console.WriteLine($"Sent: {message}");
        }
        catch (Exception e)
        {
            if (e is ArgumentNullException or SocketException)
            {
                Console.WriteLine($"Exception: {e}");
            }
        }

        Console.WriteLine("\nPress anything to continue...");
        Console.Read();
    }

    public void ReceiveData()
    {
        TcpListener server = null!;

        try
        {
            IPAddress localAddr = IPAddress.Parse(_ipAddress);
            server = new TcpListener(localAddr, _port);
            server.Start();

            var bytes = new byte[256];

            while (true)
            {
                Console.Write("Waiting for a connection... ");

                using TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");
                using NetworkStream stream = client.GetStream();

                int count;
                while ((count = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string data = asciiEndocing.GetString(bytes, 0, count);
                    Console.WriteLine($"Received: {data}");
                }
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine($"SocketException: {e}");
        }
        finally
        {
            server.Stop();
        }

        Console.WriteLine("\nPress anything to continue...");
        Console.Read();
    }
}