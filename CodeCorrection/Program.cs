namespace CodeCorrection;

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

public class Program {
    
    static public void Main(String[] args)
    {
    string baseDataDirPath = 
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "telekomunikacja_2022");
        Util.CreateDirectory(baseDataDirPath);

        string filename;

        Console.WriteLine("Choose option:");
        Console.WriteLine("1. Encode file");
        Console.WriteLine("2. Decode file");
        Console.Write("Choice: ");

        var choice = Util.ReadInt32();
        switch (choice)
        {
            case 1:
                Console.Write("Enter file name to encode: ");
                filename = Console.ReadLine() ?? throw new ArgumentException();
                Reader.EncodeFile(filename);
                break;
            case 2:
                Console.Write("Enter file name to decode: ");
                filename = Console.ReadLine() ?? throw new ArgumentException();
                Reader.DecodeFile(filename);
                break;
            default:
                Console.WriteLine("Incorrect choice.");
                break;
        }
        
        Console.ReadLine();
    }
}