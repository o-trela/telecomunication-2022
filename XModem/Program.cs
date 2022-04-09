﻿using System;

// Bardzo basic program do testowania tych symulatorów portów szeregowych.
//
// Najpierw zainstaluj sobie bibliotekę System.IO.Ports (nie wiem jak, po prostu to zrób)
// Na razie korzystam z Virtual Null Modem i jest git (https://www.virtual-null-modem.com/)
// Tam trza stworzyć nowy modem, wybrać COM2 i COM3 (znaczy można inne, ale po co) i handshake chyba nie ma znaczenia na razie.
// Na razie symulowanie zakłóceń coś mi nie działa, ale to już w kilku programach, plus nie tylko w moim xmodemie, w tych na dysku też coś sie klepie jak się daje zakłócenie.
// Jak już sobie stworzysz te porty i Zastosujesz to se zbuduj ten programik i odpal 2 instancje po prostu z pliku exe
// W nadajniku wybierasz powiedzmy COM2, w odbiorniku COM3, i jak w nadajniku coś napiszesz to to się wyświetli w odbiorniku (a zakończyć można wpisując 'q')

namespace XModem;
using Color = ConsoleColor;

public class Program
{
    public static void Main()
    {
        /*
        int mode; // 1 - Nadajnik, 2 - Odbiornik
        int port;
        //string filePath;

        Println("Telekomunikacja i Przetwarzanie Sygnałów 2022 - XModem\n", Color.Cyan, Color.Magenta);
        Println("Wybierz funkcje programu:\n" +
            "1. Nadajnik\n" +
            "2. Odbiornik");
        Print("Wybor: ");
        mode = Utils.ReadInt32(1, 2);

        string[] ports = PortManager.GetPorts();

        Println("Wybierz numer portu szeregowego:");
        for (int i = 0; i < ports.Length; i++)
        {
            Println($"{i+1}. {ports[i]}");
        }
        Print("Wybor: ");
        port = Utils.ReadInt32(1, ports.Length);

        var portManager = new PortManager(port, (o) => Print(o));
        portManager.Open();

        bool cont = true;
        switch (mode)
        {
            case 1:
                while (cont)
                {
                    var keyInfo = Console.ReadKey();
                    string message = Char.ToString(keyInfo.KeyChar);
                    portManager.Write(message);
                    cont = !message.Equals("q", StringComparison.OrdinalIgnoreCase);
                }
                break;
            case 2:
                while (cont)
                {
                    string message = portManager.Read();
                    cont = !message.Equals("q", StringComparison.OrdinalIgnoreCase);
                }
                break;
        }
        */

        Receiver receiver = new Receiver(1, PortManager.VerificationMethod.CheckSum, o => Print(o));
        receiver.StartTransmission();


        //Console.ReadLine();
    }

    static void Println(object? text, Color font = Color.White, Color bg = Color.Black) => Print($"{text}\n", font, bg);
    static void Print(object? text, Color font = Color.White, Color bg = Color.Black)
    {
        Console.ForegroundColor = font;
        Console.BackgroundColor = bg;

        Console.Write(text);

        Console.ForegroundColor = Color.White;
        Console.BackgroundColor = Color.Black;
    }
}