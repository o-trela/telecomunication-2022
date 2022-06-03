using NAudio.Wave;

namespace AudioConverter.Converters;

internal static class Recorder
{
    private static bool isRecording = false;
    private static WaveInEvent? audioRecorder;
    private static WaveFileWriter? writer;

    public static void StartRecording(string filename, int deviceNum, WaveFormat? waveFormat = default)
    {
        if (isRecording) return;
        
        audioRecorder = new WaveInEvent()
        {
            WaveFormat = waveFormat ?? new WaveFormat(4100, 24, 1),
            DeviceNumber = deviceNum,
        };
        writer = new WaveFileWriter(filename, audioRecorder.WaveFormat);
        audioRecorder.DataAvailable += WriteWave;

        audioRecorder.StartRecording();
        isRecording = true;
    }
    
    public static void StopRecording()
    {
        if (!isRecording) return;
        
        audioRecorder?.StopRecording();
        isRecording = false;
    }

    private static void WriteWave(object? sender, WaveInEventArgs args)
    {
        writer?.Write(args.Buffer, 0, args.BytesRecorded);
    }

    public static void Dispose()
    {
        audioRecorder?.Dispose();
        audioRecorder = null;
        writer?.Dispose();
        writer = null;
    }
}
