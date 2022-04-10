using System;

namespace XModem;
using Color = ConsoleColor;

public class Program
{
    public static void Main(string[] args)
    {
        Println("Telekomunikacja i Przetwarzanie Sygnałów 2022 - XModem\n", Color.Cyan, Color.Magenta);

        if (args.Length == 3)
        {
            Run(Int32.Parse(args[0]), Int32.Parse(args[1]), args[2]);
        }
        else
        {
            Println("Wybierz funkcje programu:\n" +
                "1. Nadajnik\n" +
                "2. Odbiornik");
            Print("Wybor: ");
            int mode = Utils.ReadInt32(1, 2); // 1 - Nadajnik, 2 - Odbiornik
            string[] ports = PortManager.GetPorts();
            Println("Wybierz numer portu szeregowego:");
            for (var i = 0; i < ports.Length; i++)
            {
                Println($"{i + 1}. {ports[i]}");
            }
            Print("Wybor: ");
            int port = Utils.ReadInt32(1, ports.Length);

            Println("Podaj ścieżkę do pliku docelowego:");
            string filePath = Console.ReadLine() ?? "null";

            Run(mode, port, filePath);
        }

        Console.ReadLine();
    }

    private static void Run(int mode, int portNumber, string filePath)
    {
        PortManager manager = mode switch
        {
            1 => new Transmitter(portNumber, new byte[] { 69, 42, 0 }, PortManager.VerificationMethod.CheckSum, o => Print(o)),
            2 => new Receiver(portNumber, PortManager.VerificationMethod.CheckSum, o => Print(o)),
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