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
using System.Collections.ObjectModel;

public class MidiBuilder
{
    MidiFile midiFile;
    PatternBuilder patternBuilder;

    public MidiBuilder(){
        patternBuilder = new PatternBuilder();
	}

    public void addNote(string note, MusicalTimeSpan musicalTimeSpan)
    {
        patternBuilder.Note(note, musicalTimeSpan);
    }

    public void addChord(string[] noteNames, MusicalTimeSpan musicalTimeSpan)
    {
        List<Melanchall.DryWetMidi.MusicTheory.Note> notes = new List<Melanchall.DryWetMidi.MusicTheory.Note>();
        for (int i = 0; i < noteNames.Length; i++)
        {
            notes.Add(Melanchall.DryWetMidi.MusicTheory.Note.Parse(noteNames[i]));
        }
        patternBuilder.Chord(notes, musicalTimeSpan);
    }

    public MidiFile buildMidi(string midiName, PatternBuilder patternBuilder, short bpm)
    {
        
        var pattern = patternBuilder.Build();
        var midiFile = pattern.ToFile(TempoMap.Create(new TicksPerQuarterNoteTimeDivision(480), Tempo.FromBeatsPerMinute(bpm)));
        midiFile.Write(midiName + ".mid", overwriteFile: true);
        return midiFile;
    }




}
