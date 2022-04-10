using System;

namespace XModem;
using Color = ConsoleColor;
using Verification = PortManager.VerificationMethod;

public class Program
{
    // Przykład wywołania: ./XModem.exe 1 COM3 text.txt
    //                       (nadajnik) (nazwa portu) (nazwa pliku))
    // Przykład wywołania: ./XModem.exe 2 COM4 text.out.txt
    //                       (odbrionik) (nazwa portu) (nazwa pliku))
    public static void Main(string[] args)
    {
        FileManager.EnsureDirectoryIsValid();
        ICommunicator comm = new Logger();

        comm.PrintHeader("Telekomunikacja i Przetwarzanie Sygnałów 2022 - XModem");

        if (args.Length == 3)
        {
            Run(Int32.Parse(args[0]), args[1], args[2], Verification.CheckSum);
        }
        else if (args.Length == 4)
        {
            Run(Int32.Parse(args[0]), args[1], args[2], (Verification)Int32.Parse(args[3]));
        }
        else
        {
            comm.Print("Wybierz funkcje programu:\n" +
                "1. Nadajnik\n" +
                "2. Odbiornik");
            comm.Prompt("Wybor: ");
            int mode = Utils.ReadInt32(1, 2);

            string[] ports = PortManager.GetPorts();
            comm.Print("Wybierz numer portu szeregowego:");
            for (var i = 0; i < ports.Length; i++) comm.Print($"{i + 1}. {ports[i]}");
            comm.Prompt("Wybor: ");
            int port = Utils.ReadInt32(1, ports.Length);

            comm.Prompt("Podaj ścieżkę do pliku docelowego:");
            string filePath = Console.ReadLine() ?? "null";

            comm.Print("Wybierz metodę werydikacji poprawności zawartości:\n" +
                "1. Checksum\n" +
                "2. CRC");
            comm.Prompt("Wybor: ");
            Verification method = (Verification)Utils.ReadInt32(1, 2);


            Run(mode, ports[port - 1], filePath, method);
        }

        Console.ReadLine();
    }

    private static void Run(int mode, string portName, string fileName, Verification method, ILogger? logger = null)
    {
        if (logger is null) logger = new Logger();

        var fileManager = new FileManager(fileName);

        PortManager manager = mode switch
        {
            1 => new Transmitter(portName, fileManager.Read(), method, logger),
            2 => new Receiver(portName, fileManager.Write, method, logger),
            _ => throw new ArgumentException($"Mode {mode} is not correct!"),
        };
        manager.Open();
        manager.Process();
    }
}