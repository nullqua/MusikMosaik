using MeltySynth;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace app
{
    public class MidiPlayer
    {
        public static async void MididateiAbspielen(string mididatei)
        {
            var player = new MidiSampleProvider("TimGM6mb.sf2");

            using (var waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
            {
                waveOut.Init(player);
                waveOut.Play();

                // Load the MIDI file.
                var midiFile = new MeltySynth.MidiFile(mididatei);

                // Play the MIDI file.
                player.Play(midiFile);

                // Wait until any key is pressed.
                await Task.Run(() =>
                {
                    while (!player.IsPlaying())
                    {
                        Thread.Sleep(100); // Sleep to prevent busy-waiting
                    }
                });
            }
        }
    }
    public class MidiSampleProvider : ISampleProvider
    {
        private static WaveFormat format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

        private Synthesizer synthesizer;
        private MidiFileSequencer sequencer;
        //private static MidiSampleProvider instance;

        private object mutex;

        public MidiSampleProvider(string soundFontPath)
        {
            //synthesizer = new Synthesizer(soundFontPath, format.SampleRate);
            sequencer = new MidiFileSequencer(new Synthesizer(soundFontPath, format.SampleRate));

            mutex = new object();
        }
        //public static MidiSampleProvider GetInstance(string soundFontPath)
        //{
        //    if(instance == null)
        //    {
        //        return instance = new MidiSampleProvider(soundFontPath);
        //    }
        //    return instance;
        //}

        public void Play(MidiFile midiFile)
        {
            lock (mutex)
            {
                sequencer.Play(midiFile, false);
            }
        }

        public void Stop()
        {
            lock (mutex)
            {
                sequencer.Stop();
            }
        }
        public bool IsPlaying()
        {
            return sequencer.EndOfSequence;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            lock (mutex)
            {
                sequencer.RenderInterleaved(buffer.AsSpan(offset, count));
            }

            return count;
        }

        public WaveFormat WaveFormat => format;
    }
}
