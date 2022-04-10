using System;

namespace XModem;
using Color = ConsoleColor;

public class Program
{
    // Przykład wywołania: ./XModem.exe 1 COM3 text.txt
    //                       (nadajnik) (nazwa portu) (nazwa pliku))
    // Przykład wywołania: ./XModem.exe 2 COM4 text.out.txt
    //                       (odbrionik) (nazwa portu) (nazwa pliku))
    public static void Main(string[] args)
    {
        FileManager.EnsureDirectoryIsValid();
        Println("Telekomunikacja i Przetwarzanie Sygnałów 2022 - XModem\n", Color.Cyan, Color.Magenta);

        if (args.Length == 3)
        {
            Run(Int32.Parse(args[0]), args[1], args[2]);
        }
        else
        {
            Println("Wybierz funkcje programu:\n" +
                "1. Nadajnik\n" +
                "2. Odbiornik");
            Print("Wybor: ");
            int mode = Utils.ReadInt32(1, 2);
            string[] ports = PortManager.GetPorts();
            Println("Wybierz numer portu szeregowego:");
            for (var i = 0; i < ports.Length; i++) Println($"{i + 1}. {ports[i]}");
            Print("Wybor: ");
            int port = Utils.ReadInt32(1, ports.Length);

            Println("Podaj ścieżkę do pliku docelowego:");
            string filePath = Console.ReadLine() ?? "null";

            Run(mode, ports[port - 1], filePath);
        }

        Console.ReadLine();
    }

    private static void Run(int mode, string portName, string filePath)
    {
        var fileManager = new FileManager(filePath);

        PortManager manager = mode switch
        {
            1 => new Transmitter(portName, fileManager.Read(), PortManager.VerificationMethod.CheckSum, o => Print(o)),
            2 => new Receiver(portName, fileManager.Write, PortManager.VerificationMethod.CheckSum, o => Print(o)),
            _ => throw new ArgumentException($"Mode {mode} is not correct!"),
        };
        manager.Open();
        manager.Process();

        Console.ReadLine();
        return;
    }

    private static void Println(object? text, Color font = Color.White, Color bg = Color.Black) => Print($"{text}\n", font, bg);
    private static void Print(object? text, Color font = Color.White, Color bg = Color.Black)
    {
        Console.ForegroundColor = font;
        Console.BackgroundColor = bg;

        Console.Write(text);

        Console.ForegroundColor = Color.White;
        Console.BackgroundColor = Color.Black;
    }
}