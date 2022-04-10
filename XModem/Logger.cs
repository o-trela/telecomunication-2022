namespace XModem;
using Color = ConsoleColor;

public class Logger : ILogger, ICommunicator
{
    public void Log(object message)
    {
        WriteLine(message, Color.Gray);
    }

    public void LogWarning(object message)
    {
        WriteLine(message, Color.Yellow);
    }

    public void LogError(object message)
    {
        WriteLine(message, Color.Red);
    }

    public void LogProgress(object message)
    {
        WriteLine(message, Color.Blue);
    }

    public void LogSuccess(object message)
    {
        WriteLine(message, Color.Green);
    }

    public void Print(object message)
    {
        WriteLine(message);
    }

    public void PrintHeader(object message)
    {
        WriteLine($"{message}\n", Color.Cyan, Color.Magenta);
    }

    public void Prompt(object message)
    {
        Write(message);
    }


    private static void Write(object message, Color fg = Color.White, Color bg = Color.Black)
    {
        Console.ForegroundColor = fg;
        Console.BackgroundColor = bg;
        Console.Write(message);
        Reset();
    }

    private static void WriteLine(object message, Color fg = Color.White, Color bg = Color.Black)
    {
        Console.ForegroundColor = fg;
        Console.BackgroundColor = bg;
        Console.WriteLine(message);
        Reset();
    }

    private static void Reset()
    {
        Console.ForegroundColor = Color.White;
        Console.BackgroundColor = Color.Black;
    }
}
