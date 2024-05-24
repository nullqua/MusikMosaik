//using Melanchall.DryWetMidi.Composing;
//using Melanchall.DryWetMidi.Core;
//using Melanchall.DryWetMidi.Interaction;
//using Melanchall.DryWetMidi.MusicTheory;
//using NAudio.Wave;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//class MidierstellenTest
//{

//    static void Main()
//    {
//        var midiFile = new MidiFile();

//        // Define a chord for bass part which is just an octave
//        var bassChord = new[] { Interval.Twelve };

//        // Build the composition
//        var pattern = new PatternBuilder()

//            // The length of all main theme's notes within four first bars is
//            // triplet eight so set it which will free us from necessity to specify
//            // the length of each note explicitly

//            .SetNoteLength(MusicalTimeSpan.Eighth.Triplet())

//            // Anchor current time (start of the pattern) to jump to it
//            // when we'll start to program bass part

//            .Anchor()

//            // We will add notes relative to G#3.
//            // Instead of Octave.Get(3).GSharp it is possible to use Note.Get(NoteName.GSharp, 3)

//            .SetRootNote(Octave.Get(3).GSharp)

//            // Add first three notes and repeat them seven times which will
//            // give us two bars of the main theme

//            // G#3
//            .Note(Interval.Zero)   // +0  (G#3)
//            .Note(Interval.Five)   // +5  (C#4)
//            .Note(Interval.Eight)  // +8  (E4)

//            .Repeat(3, 7)          // repeat three previous notes seven times

//            // Add notes of the next two bars

//            // G#3
//            .Note(Interval.One)    // +1  (A3)
//            .Note(Interval.Five)   // +5  (C#4)
//            .Note(Interval.Eight)  // +8  (E4)

//            .Repeat(3, 1)          // repeat three previous notes

//            .Note(Interval.One)    // +1  (A3)
//            .Note(Interval.Six)    // +6  (D4)
//            .Note(Interval.Ten)    // +10 (F#4)

//            .Repeat(3, 1)          // repeat three previous notes
//                                   // reaching the end of third bar

//            .Note(Interval.Zero)   // +0  (G#3)
//            .Note(Interval.Four)   // +4  (C4)
//            .Note(Interval.Ten)    // +10 (F#4)
//            .Note(Interval.Zero)   // +0  (G#3)
//            .Note(Interval.Five)   // +5  (C#4)
//            .Note(Interval.Eight)  // +8  (E4)
//            .Note(Interval.Zero)   // +0  (G#3)
//            .Note(Interval.Five)   // +5  (C#4)
//            .Note(Interval.Seven) // +7  (D#4)
//            .Note(-Interval.Two)   // -2  (F#3)
//            .Note(Interval.Four)   // +4  (C4)
//            .Note(Interval.Seven)  // +7  (D#4)

//            // Now we will program bass part. To start adding notes from the
//            // beginning of the pattern we need to move to the anchor we set
//            // above

//            .MoveToFirstAnchor()

//            // First two chords have whole length

//            .SetNoteLength(MusicalTimeSpan.Whole)

//            // insert a chord relative to
//            .Chord(bassChord, Octave.Get(2).CSharp) // C#2 (C#2, C#3)
//            .Chord(bassChord, Octave.Get(1).B)      // B1  (B1, B2)

//            // Remaining four chords has half length

//            .SetNoteLength(MusicalTimeSpan.Half)

//            .Chord(bassChord, Octave.Get(1).A)      // A1  (A1, A2)
//            .Chord(bassChord, Octave.Get(1).FSharp) // F#1 (F#1, F#2)
//            .Chord(bassChord, Octave.Get(1).GSharp) // G#1 (G#1, G#2)

//            .Repeat()                               // repeat the previous chord

//            // Build a pattern that can be then saved to a MIDI file

//            .Build();

//        midiFile = pattern.ToFile(TempoMap.Default);
//        midiFile.Write("Some great song.mid", overwriteFile: true);
//    }
//}
