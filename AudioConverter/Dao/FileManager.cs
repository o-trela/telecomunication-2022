namespace AudioConverter.Dao;

public class FileManager
{
    public static readonly string BaseDataDirPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Studia_2022",
        "telekomunikacja_2022"
    );

    public string FilePath { get; init; }

    public FileManager(string filename)
    {
        FilePath = GetFilePath(filename);
    }

    public static void EnsureDirectoryIsValid(bool writePath = false)
    {
        if (!Directory.Exists(BaseDataDirPath))
        {
            Directory.CreateDirectory(BaseDataDirPath);
        }

        try
        {
            using var fs = File.Create(
                Path.Combine(BaseDataDirPath, Path.GetRandomFileName()),
                1,
                FileOptions.DeleteOnClose);
        }
        catch (Exception e)
        {
            throw new Exception("Base Directory has no write access right!", e);
        }

        if (writePath) Console.WriteLine($"Data path = {BaseDataDirPath}");
    }
    
    public static string GetFilePath(string filename)
    {
        return Path.Combine(BaseDataDirPath, filename);
    }

    public void Write(byte[] data) => File.WriteAllBytes(FilePath, data);

    public byte[] Read() => File.ReadAllBytes(FilePath);
}