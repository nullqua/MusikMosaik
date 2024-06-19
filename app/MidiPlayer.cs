using MeltySynth;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app
{
    internal class MidiPlayer
    {
        public static void PlayMidi(string mididatei, string soundfont)
        {
            var player = new MidiSampleProvider(@"..\..\..\" + soundfont);

            using (var waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
            {
                waveOut.Init(player);
                waveOut.Play();

                // Load the MIDI file.
                var midiFile = new MeltySynth.MidiFile(@"..\..\..\" + mididatei);

                // Play the MIDI file.
                player.Play(midiFile, true);

                // Wait until any key is pressed.
                Console.ReadKey();
            }
        }
    }
    class MidiSampleProvider : ISampleProvider
    {
        private static WaveFormat format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

        private Synthesizer synthesizer;
        private MidiFileSequencer sequencer;

        private object mutex;

        public MidiSampleProvider(string soundFontPath)
        {
            synthesizer = new Synthesizer(soundFontPath, format.SampleRate);
            sequencer = new MidiFileSequencer(synthesizer);

            mutex = new object();
        }

        public void Play(MidiFile midiFile, bool loop)
        {
            lock (mutex)
            {
                sequencer.Play(midiFile, loop);
            }
        }

        public void Stop()
        {
            lock (mutex)
            {
                sequencer.Stop();
            }
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
