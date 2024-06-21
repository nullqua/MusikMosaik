using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.MusicTheory;
using NAudio.Wave;
using Melanchall.DryWetMidi.Common;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

public class SchreibeMidi
{
    public static string erstellespezifischMidi(string midiname)
    {
        var MidiFile = new MidiFile();
        var patternBuilder = new PatternBuilder();
        addChord(new string[] { "C3", "C4", "E4", "G5" }, new MusicalTimeSpan(1, 4), patternBuilder);
        addChord(new string[] { "F3", "C4", "F4", "A4" }, new MusicalTimeSpan(1, 4), patternBuilder);
        addChord(new string[] { "G3", "B3", "D4", "G4" }, new MusicalTimeSpan(1, 4), patternBuilder);
        addChord(new string[] { "C3", "C4", "E4", "G4" }, new MusicalTimeSpan(1, 4), patternBuilder);
        patternBuilder.Repeat(2,5);
        var pattern = patternBuilder.Build();
        midiFile = pattern.ToFile(TempoMap.Create(new TicksPerQuarterNoteTimeDivision(480), Tempo.FromBeatsPerMinute(60)));

        midiFile.Write(midiname+ ".mid", overwriteFile: true);

        return midiname+".mid";
    }
    public static string erstelleMidi(string midiname, int[] array)
    {
        var midiFile = new MidiFile();
        var patternBuilder = new PatternBuilder();
        for(int i = 0; i < array.Length; i++)
        {
            switch (array[i])
            {
                case 1:
                    patternBuilder.Note(Octave.Get(2).C);
                    break;
            }
        }
        var pattern = patternBuilder.Build();
        midiFile = pattern.ToFile(TempoMap.Create(Tempo.FromBeatsPerMinute(100)));
        midiFile.TimeDivision = new TicksPerQuarterNoteTimeDivision(100);
        midiFile.Write(midiname + ".mid", overwriteFile: true);
        return midiname + ".mid";
    }
    public static MidiFile buildMidi(string midiName, PatternBuilder patternBuilder, short bpm)
    {
        var midiFile = new MidiFile();
        var pattern = patternBuilder.Build();
        midiFile = pattern.ToFile(TempoMap.Create(new TicksPerQuarterNoteTimeDivision(480), Tempo.FromBeatsPerMinute(bpm)));
        midiFile.Write(midiName + ".mid", overwriteFile: true);
        return midiFile;
    }
    public static PatternBuilder newPatternBuild() 
    { 
        return new PatternBuilder();
    }
    public static PatternBuilder addNoteToPattern(PatternBuilder patternBuilder, string note, int beatNumerator, int beatdenumerator)
    {
        patternBuilder.Note(note, new MusicalTimeSpan(beatNumerator, beatdenumerator));


        return patternBuilder;
    }
    public static PatternBuilder addChord(string[] noteNames, MusicalTimeSpan musicalTimeSpan, PatternBuilder patternBuilder)
    {
        List<Melanchall.DryWetMidi.MusicTheory.Note> notes = new List<Melanchall.DryWetMidi.MusicTheory.Note>();
        for (int i = 0; i < noteNames.Length; i++)
        {
            //Melanchall.DryWetMidi.MusicTheory.Note note = Melanchall.DryWetMidi.MusicTheory.Note.Parse(noteNames[i]);
            notes.Add(Melanchall.DryWetMidi.MusicTheory.Note.Parse(noteNames[i]));
        }
        return patternBuilder.Chord(notes, musicalTimeSpan);
    }


}
