using NAudio.Wave;
using NAudio.Midi;

public class MididateiFunktionen
{
    public static void MididateiAbspielen(string mididatei, string soundfont)
    {
        var player = new MidiSampleProvider(@"..\..\..\" + soundfont);

        using (var waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
        {
            waveOut.Init(player);
            waveOut.Play();

            // Load the MIDI file.
            var midiFile = new MeltySynth.MidiFile(@"..\..\..\" + mididatei);

            // Play the MIDI file.
            player.Play(midiFile);

            // Wait until any key is pressed.
            while (!player.IsPlaying())
            {

            }
        }
    }
    public static void MididateiAuslesen(string mididatei)
    {
        var mf = new MidiFile(mididatei);
        var timeSignature = mf.Events[0].OfType<TimeSignatureEvent>().FirstOrDefault();
        Console.WriteLine("Format {0}, Tracks {1}, Delta Ticks Per Quarter Note {2}",
                mf.FileFormat, mf.Tracks, mf.DeltaTicksPerQuarterNote);
        for (int n = 0; n < mf.Tracks; n++)
        {
            foreach (var midiEvent in mf.Events[n])
            {
                if (!MidiEvent.IsNoteOff(midiEvent))
                {
                    Console.WriteLine("{0} {1}\r\n", ToMBT(midiEvent.AbsoluteTime, mf.DeltaTicksPerQuarterNote, timeSignature), midiEvent);
                }
            }
        }
    }
    private static string ToMBT(long eventTime, int ticksPerQuarterNote, TimeSignatureEvent timeSignature)
    {
        int beatsPerBar = timeSignature == null ? 4 : timeSignature.Numerator;
        int ticksPerBar = timeSignature == null ? ticksPerQuarterNote * 4 : (timeSignature.Numerator * ticksPerQuarterNote * 4) / (1 << timeSignature.Denominator);
        int ticksPerBeat = ticksPerBar / beatsPerBar;
        long bar = 1 + (eventTime / ticksPerBar);
        long beat = 1 + ((eventTime % ticksPerBar) / ticksPerBeat);
        long tick = eventTime % ticksPerBeat;
        return String.Format("{0}:{1}:{2}", bar, beat, tick);
    }
}
