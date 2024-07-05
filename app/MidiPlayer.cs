using MeltySynth;
using NAudio.Wave;

namespace app
{

    public class MidiPlayer
    {
        public static async void PlayMidiFile(string mididatei)
        {
            var player = new MidiSampleProvider("TimGM6mb.sf2");

            using var waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());
            waveOut.Init(player);
            waveOut.Play();

            var midiFile = new MidiFile(mididatei);

            player.Play(midiFile);

            await Task.Run(() =>
            {
                while (!player.IsPlaying())
                {
                    // Prevents busy-waiting problem
                    Thread.Sleep(100);
                }
            });
        }
    }
    public class MidiSampleProvider(string soundFontPath) : ISampleProvider
    {
        private static readonly WaveFormat format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

        private MidiFileSequencer sequencer = new MidiFileSequencer(new Synthesizer(soundFontPath, format.SampleRate));

        private object mutex = new object();

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
