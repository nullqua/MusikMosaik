using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Core;

public class MidiBuilder
{
    private readonly PatternBuilder patternBuilder = new();

    public void AddNote(string note, MusicalTimeSpan musicalTimeSpan)
    {
        patternBuilder.Note(note, musicalTimeSpan);
    }

    public void AddChord(string[] noteNames, MusicalTimeSpan musicalTimeSpan)
    {
        var notes = new List<Melanchall.DryWetMidi.MusicTheory.Note>();

        for (int i = 0; i < noteNames.Length; i++)
        {
            notes.Add(Melanchall.DryWetMidi.MusicTheory.Note.Parse(noteNames[i]));
        }
        patternBuilder.Chord(notes, musicalTimeSpan);
    }
    public void Repeat(int actionsCount, int repeat)
    {
        patternBuilder.Repeat(actionsCount, repeat);
    }

    public string BuildMidi(string midiName, short bpm)
    {
        var pattern = patternBuilder.Build();
        var midiFile = pattern.ToFile(TempoMap.Create(new TicksPerQuarterNoteTimeDivision(480), Tempo.FromBeatsPerMinute(bpm)));

        midiFile.Write(midiName + ".mid", overwriteFile: true);
        return midiName + ".mid";
    }
}
