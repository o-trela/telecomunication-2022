using System;

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
    public static void Main(string[] args)
    {
        if (args.Length == 3)
        {
            int mode = Int32.Parse(args[0]); // 1 - Nadajnik, 2 - Odbiornik
            int portNumber = Int32.Parse(args[1]); // Numer Portu COM
            string filepath = args[2]; // Ścieżka do pliku żródłowego lub docelowego 9zależne od 'mode'
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