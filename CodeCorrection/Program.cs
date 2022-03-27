using Zadanie_1.Dao;

namespace Zadanie_1;

/// <instruction>
/// Coding algorythm
///
/// Program creates "telekomunikacja_2022" folder in your documents folder.
/// That will be the working space for all the files you want to encode or decode.
///
/// You will be asked for the file name.
/// The program will search for it in "telekomunikacja_2022" folder in your documents.
/// Encoded and decoded files will be created in that folder ass well.
/// </instruction>

public static class Program {
    
    public static void Main(string[] args)
    {
        string baseDataDirPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                "telekomunikacja_2022"
                );

        Utils.CreateDirectory(baseDataDirPath);

        string filename;

        Console.WriteLine("Choose option:");
        Console.WriteLine("1. Encode file");
        Console.WriteLine("2. Decode file");
        Console.Write("Choice: ");

        var choice = Utils.ReadInt32(min: 1, max: 2);
        switch (choice)
        {
            case 1:
                Console.Write("Enter name of a file to encode: ");
                filename = Console.ReadLine() ?? throw new ArgumentException("Input can not be empty!", nameof(filename));
                try 
                {
                    FileManager.EncodeFile(filename);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);   
                }
                break;
            case 2:
                Console.Write("Enter name of a file to decode: ");
                filename = Console.ReadLine() ?? throw new ArgumentException("Input can not be empty!", nameof(filename));
                try
                {
                    FileManager.DecodeFile(filename);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);   
                }
                break;
            default:
                Console.WriteLine("Incorrect choice.");
                break;
        }
        
        Console.ReadLine();
    }
}