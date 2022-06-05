using System;
using AudioConverter.Converters;
using AudioConverter.Dao;
using AudioConverter.Util;

FileManager.EnsureDirectoryIsValid();

const string defOutputFileName = "output.wav";
const string defInputFileName = "rafal.wav";

string filename;
int deviceNr;
int choice;
int sampleRate = 44100;
int bits = 24;
int channels = 1;

Console.WriteLine("Choose an action:\n" +
    "1. Record audio\n" +
    "2. Play audio");
Console.Write("Choice: ");
choice = Utils.ReadInt32(1, 2);

int i = 1;
if (choice == 1)
{
    Console.WriteLine("Wave format:");
    Console.Write($"Sample Rate (def: {sampleRate}): ");
    AudioManager.SampleRate = Utils.ReadInt32(1, 384000, def: sampleRate, predicate: x => x % 24000 == 0 || x % 44100 == 0);
    
    Console.Write($"Bits (def: {bits}): ");
    AudioManager.Bits = Utils.ReadInt32(1, 32, def: bits, predicate: x => x % 8 == 0);
    
    Console.Write($"channels (def: {channels}): ");
    AudioManager.Channels = Utils.ReadInt32(1, 2, def: channels);
    
    Console.Write("Output Filename: ");
    string? read = Console.ReadLine();
    if (String.IsNullOrWhiteSpace(read)) filename = defOutputFileName;
    else filename = read;
    
    Console.WriteLine("Choose recording device:");
    foreach (var device in AudioManager.GetRecordingDevices())
    {
        Console.WriteLine($"{i}. {device.ProductName}");
        i++;
    }
    Console.Write("Choice: ");
    deviceNr = Utils.ReadInt32(1, i) - 1;
    AudioManager.RecordingDevice = deviceNr;
    
    string filepath = FileManager.GetFilePath(filename);
    AudioManager.Record(filepath);
}
else
{
    Console.Write("Audio Filename: ");
    string? read = Console.ReadLine();
    if (String.IsNullOrWhiteSpace(read)) filename = defInputFileName;
    else filename = read;

    Console.WriteLine("Choose playback device:");
    foreach (var device in AudioManager.GetPlaybackDevices())
    {
        Console.WriteLine($"{i}. {device.ProductName}");
        i++;
    }
    Console.Write("Choice: ");
    deviceNr = Utils.ReadInt32(1, i) - 1;
    AudioManager.PlaybackDevice = deviceNr;
    
    string filepath = FileManager.GetFilePath(filename);
    AudioManager.Play(filepath);
}

Console.WriteLine("All Done!");
Console.ReadLine();
