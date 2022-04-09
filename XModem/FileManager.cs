namespace XModem;

public class FileManager
{
    public static readonly string BaseDataDirPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
        "Studia_2022",
        "telekomunikacja_2022"
    );

    private string _fullFilePath;

    public FileManager(string filename)
    {
        _fullFilePath = Path.Combine(BaseDataDirPath, filename);
    }

    public void Write(byte[] data) => File.WriteAllBytes(_fullFilePath, data);

    public byte[] Read() => File.ReadAllBytes(_fullFilePath);
}