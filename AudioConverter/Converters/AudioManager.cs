using System;
using System.Runtime.InteropServices;
using System.Text;
using NAudio;
using NAudio.Wave;

namespace AudioConverter.Converters;

public static class AudioManager
{
    public static int RecordingDevice { get; set; }
    public static int PlaybackDevice { get; set; }
    public static int SampleRate { get; set; }
    public static int Bits { get; set; }
    public static int Channels { get; set; }

    private static WaveFormat? waveFormat;

    public static WaveInCapabilities[] GetRecordingDevices()
    {
        int inCount = WaveInEvent.DeviceCount;
        WaveInCapabilities[] devices = new WaveInCapabilities[inCount];
        for (int i = 0; i < inCount; i++)
        {
            devices[i] = WaveInEvent.GetCapabilities(i);
        }
        return devices;
    }
    
    public static WaveOutCapabilities[] GetPlaybackDevices()
    {
        int outCount = WaveInterop.waveOutGetNumDevs();
        WaveOutCapabilities[] devices = new WaveOutCapabilities[outCount];
        WaveOutCapabilities caps = new();
        int structSize = Marshal.SizeOf(caps);
        for (int i = 0; i < outCount; i++)
        {
            MmException.Try(WaveInterop.waveOutGetDevCaps((IntPtr)i, out caps, structSize), "waveOutGetDevCaps");
            devices[i] = caps;
        }
        return devices;
    }

    public static void Play(string filename)
    {
        Player.Play(filename, PlaybackDevice);
    }
    
    public static void Record(string filename)
    {
        waveFormat = new WaveFormat(SampleRate, Bits, Channels);
        Recorder.StartRecording(filename, RecordingDevice, waveFormat);
        Console.Write("Press Enter to stop recording");
        Console.Read();
        Recorder.StopRecording();
    }
}
