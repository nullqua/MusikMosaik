using System;
using NAudio.Wave;
using MeltySynth;
using Microsoft.VisualBasic.ApplicationServices;

class Program
{
    static void Main()
    {
        var player = new MidiSampleProvider("C:\\Users\\wolfg\\Musikinformatik\\musikinformatik\\TestSound\\TimGM6mb.sf2");

        using (var waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
        {
            waveOut.Init(player);
            waveOut.Play();

            // Load the MIDI file.
            var midiFile = new MidiFile("C:\\Users\\wolfg\\Musikinformatik\\musikinformatik\\TestSound\\MIDI_sample.mid");

            // Play the MIDI file.
            player.Play(midiFile, true);

            // Wait until any key is pressed.
            Console.ReadKey();
        }
    }
}