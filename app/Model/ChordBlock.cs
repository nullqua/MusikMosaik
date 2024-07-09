using Melanchall.DryWetMidi.Interaction;

namespace app.Model
{
    /// <summary>
    /// Represent a musical chord block.
    /// </summary>
    public class ChordBlock : MusicBlock
    {
        private static readonly string[] AllNotes = [
            "c", "c#", "d", "d#", "e", "f", "f#", "g", "g#", "a", "a#", "b"
        ];

        public int Pitch { get; set; }
        public int Velocity { get; set; }
        public MusicalTimeSpan TimeSpan { get; set; }
        public string Mode { get; set; }
        public string Basstone { get; set; }
        public string Overtone { get; set; }
        public int[] NotePitches { get; private set; } = new int[5];
        public string[] NoteNames { get; private set; } = new string[5];
        public string[] FinalNoteNames { get; private set; }

        /// <summary>
        /// Creates a new instance of a chord block.
        /// </summary>
        /// <param name="id">Unique identifier of code block.</param>
        /// <param name="basstone">The bass tone.</param>
        /// <param name="overtone">The over tone.</param>
        /// <param name="pitch">The pitch.</param>
        /// <param name="mode">The scale (major or minor).</param>
        /// <param name="musicalTimeSpan">The time span as fraction.</param>
        /// <param name="velocity">The velocity.</param>
        public ChordBlock(Guid id, string basstone, string overtone, int pitch, string mode, MusicalTimeSpan musicalTimeSpan, int velocity)
        {
            Id = id;
            Basstone = basstone;
            Overtone = overtone;
            Pitch = pitch;
            Mode = mode;
            TimeSpan = musicalTimeSpan;
            Velocity = velocity;

            for (int i = 0; i < NotePitches.Length; i++)
            {
                NotePitches[i] = pitch;
            }

            GenerateChordNotesAndPitches();
            FinalNoteNames = NoteNames.Distinct().ToArray();

        }

        /// <summary>
        /// Generates and adjusts the note names and pitches for the chord based on its characteristics.
        /// </summary>
        public void GenerateChordNotesAndPitches()
        {
            int RootNoteInt = int.MaxValue;
            int OvertoneInt = int.MaxValue;
            int ChordTerz;
            int ChordQuint;
            int ChordOktav;

            bool RootNoteBool = false;

            NoteNames[4] = Overtone + Pitch.ToString();

            for (int i = 0; i < AllNotes.Length; i++)
            {
                if (Basstone == AllNotes[i])
                {
                    RootNoteInt = i;
                }

                if (Overtone == AllNotes[i])
                {
                    OvertoneInt = i;
                }
            }

            if (RootNoteInt == int.MaxValue || OvertoneInt == int.MaxValue)
            {
                throw new Exception("Basstone is not correct");
            }

            if (Mode == "Major")
            {
                NoteNames[0] = Basstone;
                ChordTerz = (RootNoteInt + 4) % 12;
                NoteNames[1] = AllNotes[ChordTerz % 12];
                ChordQuint = (RootNoteInt + 7) % 12;
                NoteNames[2] = AllNotes[ChordQuint % 12];
                ChordOktav = RootNoteInt;
                NoteNames[3] = AllNotes[ChordOktav % 12];
                NoteNames[4] = Overtone;
            }
            else if (Mode == "Minor")
            {
                NoteNames[0] = Basstone;
                ChordTerz = (RootNoteInt + 3) % 12;
                NoteNames[1] = AllNotes[ChordTerz % 12];
                ChordQuint = (RootNoteInt + 7) % 12;
                NoteNames[2] = AllNotes[ChordQuint % 12];
                ChordOktav = RootNoteInt;
                NoteNames[3] = AllNotes[ChordOktav % 12];
                NoteNames[4] = Overtone;
            }
            else
            {
                throw new Exception("Mode wrong falue different to Major/Minor");
            }

            RootNoteInt -= 13;
            NotePitches[0] = NotePitches[0] - 1;

            if (ChordTerz > OvertoneInt)
            {
                ChordTerz -= 13;
                NotePitches[1] = NotePitches[1] - 1;
                RootNoteBool = true;
            }
            else if (ChordTerz == OvertoneInt)
            {
                ChordTerz = int.MaxValue;
            }

            if (ChordQuint > OvertoneInt)
            {
                ChordQuint -= 13;
                NotePitches[2] = NotePitches[2] - 1;
                RootNoteBool = true;
            }
            else if (ChordQuint == OvertoneInt)
            {
                ChordQuint = int.MaxValue;
            }

            if (ChordOktav > OvertoneInt)
            {
                ChordOktav -= 13;
                NotePitches[3] = NotePitches[3] - 1;
                RootNoteBool = true;
            }
            else if (ChordOktav == OvertoneInt)
            {
                ChordOktav = int.MaxValue;
            }

            if (RootNoteBool)
            {
                RootNoteInt -= 13;
                NotePitches[0] = NotePitches[0] - 1;
            }

            for (int i = 0; i < NotePitches.Length; i++)
            {
                NoteNames[i] = NoteNames[i] + NotePitches[i].ToString();
            }
        }
    }
}
