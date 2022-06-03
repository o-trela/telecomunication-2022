using NAudio.Wave;

namespace AudioConverter.Converters;

internal static class Player
{
    public static void Play(string fileName, int deviceNr)
    {
        using var audioFileReader = new WaveFileReader(fileName);
        using var outputDevice = new WaveOutEvent()
        {
            DeviceNumber = deviceNr,
        };
        outputDevice.Init(audioFileReader);
        outputDevice.Play();
        while (outputDevice.PlaybackState == PlaybackState.Playing)
        {
            Thread.Sleep(100);
        }
    }
}