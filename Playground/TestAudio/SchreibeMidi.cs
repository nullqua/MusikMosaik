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
        .SetNoteLength(new MidiTimeSpan(100))
        //.SetNoteLength(MusicalTimeSpan.Quarter)
        .SetVelocity((SevenBitNumber)100)
        .Note(Octave.Get(2).C)
        .Repeat(1,10)
        .SetVelocity((SevenBitNumber)50)
        .Note(Octave.Get(2).C, new MidiTimeSpan(50))
        .Repeat(1, 10)
        .Build();
        
        midiFile = pattern.ToFile(TempoMap.Create(Tempo.FromBeatsPerMinute(100)));
        midiFile.TimeDivision = new TicksPerQuarterNoteTimeDivision(100);
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
}
