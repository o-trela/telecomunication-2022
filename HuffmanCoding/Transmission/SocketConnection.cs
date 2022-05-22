using HuffmanCoding.Util;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HuffmanCoding.Transmission;

public class SocketConnection
{
    private readonly Encoding encoding = Encoding.UTF8;
    private readonly string _ipAddress;
    private readonly int _port;

    public SocketConnection(string ipAddress, int port)
    {
        _ipAddress = ipAddress;
        _port = port;
    }

    public void SendData(string message, Dictionary<char, string> dictionary)
    {
        try
        {
            byte[] dictionaryData = Serializer.Serialize(dictionary);
            byte[] messageData = encoding.GetBytes(message);

            using var client = new TcpClient(_ipAddress, _port);
            using NetworkStream stream = client.GetStream();

            byte[] length = dictionaryData.Length.ToBytes();
            stream.Write(length, 0, length.Length);
            stream.Write(dictionaryData, 0, dictionaryData.Length);
            Console.WriteLine($"Sent: {String.Join(" ", dictionary)}");

            length = messageData.Length.ToBytes();
            stream.Write(length, 0, length.Length);
            stream.Write(messageData, 0, messageData.Length);
            Console.WriteLine($"Sent: {message}");
        }
        catch (Exception e)
        {
            if (e is ArgumentNullException or SocketException)
            {
                Console.WriteLine($"Exception: {e}");
            }
            throw;
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

            Console.Write("Waiting for a connection... ");

            using TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Connected!");
            using NetworkStream stream = client.GetStream();

            byte[] sizeBuffer = new byte[4];
            byte[] buffer = new byte[1024];
            List<byte> allBytes = new();
            int count = 0;
            int dataSize = 0;

            while (dataSize <= 0)
            {
                count = stream.Read(sizeBuffer, 0, sizeBuffer.Length);
                dataSize = sizeBuffer.ToInt32();
                Thread.Sleep(10);
            }

            if (count != 4) throw new Exception("Received Data should have been 4 bytes long!");

            while (dataSize > 0)
            {
                int toRead = (dataSize, buffer.Length).Min();
                stream.Read(buffer, 0, toRead);
                allBytes.AddRange(new ArraySegment<byte>(buffer, 0, dataSize));
                dataSize -= toRead;
            }
            Dictionary<char, string> dictionary = Serializer.Deserialize<Dictionary<char, string>>(allBytes.ToArray());

            count = stream.Read(sizeBuffer, 0, sizeBuffer.Length);
            if (count != 4) throw new Exception("Received Data should have been 4 bytes long!");

            allBytes.Clear();
            dataSize = sizeBuffer.ToInt32();
            while (dataSize > 0)
            {
                int toRead = (dataSize, buffer.Length).Min();
                stream.Read(buffer, 0, toRead);
                allBytes.AddRange(new ArraySegment<byte>(buffer, 0, dataSize));
                dataSize -= toRead;
            }

            string message = encoding.GetString(allBytes.ToArray());
            Console.WriteLine($"Receive message: {message}");
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