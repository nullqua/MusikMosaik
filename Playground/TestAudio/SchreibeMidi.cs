using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using NAudio.Wave;
using Melanchall.DryWetMidi.Common;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

public class SchreibeMidi
{
    public static string erstellespezifischMidi(string midiname)
    {
        var midiFile = new MidiFile();
        var pattern = new PatternBuilder()
        //.SetNoteLength(new MusicalTimeSpan(1,4,true))
        //.SetNoteLength(MusicalTimeSpan.Quarter)
        .SetVelocity((SevenBitNumber)100)
        .Note("C3", MusicalTimeSpan.Quarter)
        .Repeat(1,10)
        .Build();

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
        midiFile = pattern.ToFile(TempoMap.Create(Tempo.FromBeatsPerMinute(bpm)));
        midiFile.TimeDivision = new TicksPerQuarterNoteTimeDivision(bpm);
        midiFile.Write(midiName + ".mid", overwriteFile: true);
        return midiFile;
    }
    public static PatternBuilder newPatternBuild(string pattern) 
    { 
        return new PatternBuilder();
    }
    public static PatternBuilder addNoteToPattern(PatternBuilder patternBuilder, string note, int notenlange)
    {
        patternBuilder.SetNoteLength(new MidiTimeSpan());
        patternBuilder.Note(note);
        
        return patternBuilder;
    }


}
