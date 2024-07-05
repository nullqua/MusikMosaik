using Melanchall.DryWetMidi.Interaction;
using System.Diagnostics;

namespace app.Model
{
    public class ChordBlock : MusicBlock
    {
        public int velocity;
        public MusicalTimeSpan musicalTimeSpan;
        public string basstone;
        public string overtone;
        public int pitch;
        private string[] allNotes = {
            "c", "c#", "d", "d#", "e", "f", "f#", "g", "g#", "a", "a#", "b"
        };
        public string mode;
        /// <summary>
        /// GrundTon1, Terz, Quinte, GrundTon2, MelodieTon
        /// </summary>
        public string[] notenames = new string[5];
        /// <summary>
        /// GrundTon1, Terz, Quinte, GrundTon2, MelodieTon
        /// </summary>
        public int[] notepitches = new int[5];
        public string[] notenamesFinal;

        public ChordBlock(Guid id, string basstone, string overtone, int pitch, string mode, MusicalTimeSpan musicalTimeSpan, int velocity)
        {
            Id = id;
            this.basstone = basstone;
            this.overtone = overtone;
            this.pitch = pitch;
            this.mode = mode;
            this.musicalTimeSpan = musicalTimeSpan;
            this.velocity = velocity;

            for (int i = 0; i < notepitches.Length; i++)
            {
                notepitches[i] = pitch;
            }

            FillNotenames();
            IsNotenamestwice();
        }

        public void FillNotenames()
        {
            int RootNoteInt = int.MaxValue;
            int OvertoneInt = int.MaxValue;
            int ChordTerz;
            int ChordQuint;
            int ChordOktav;
            bool RootNoteBool = false;

            notenames[4] = overtone + pitch.ToString();

            for (int i = 0; i < allNotes.Length; i++)
            {
                if (basstone == allNotes[i])
                {
                    RootNoteInt = i;
                }

                if (overtone == allNotes[i])
                {
                    OvertoneInt = i;
                }
            }

            if (RootNoteInt == int.MaxValue || OvertoneInt == int.MaxValue)
            {
                throw new Exception("Basstone nicht richtig angegeben");
            }

            if (mode == "Major")
            {
                notenames[0] = basstone;
                ChordTerz = (RootNoteInt + 4) % 12;
                notenames[1] = allNotes[ChordTerz % 12];
                ChordQuint = (RootNoteInt + 7) % 12;
                notenames[2] = allNotes[ChordQuint % 12];
                ChordOktav = (RootNoteInt);
                notenames[3] = allNotes[ChordOktav % 12];
                notenames[4] = overtone;
            }
            else if (mode == "Minor")
            {
                notenames[0] = basstone;
                ChordTerz = (RootNoteInt + 3) % 12;
                notenames[1] = allNotes[ChordTerz % 12];
                ChordQuint = (RootNoteInt + 7) % 12;
                notenames[2] = allNotes[ChordQuint % 12];
                ChordOktav = (RootNoteInt);
                notenames[3] = allNotes[ChordOktav % 12];
                notenames[4] = overtone;
            }
            else
            {
                throw new Exception("Mode wrong falue different to Major/Minor");
            }

            RootNoteInt -= 13;
            notepitches[0] = notepitches[0] - 1;

            if (ChordTerz > OvertoneInt)
            {
                ChordTerz -= 13;
                notepitches[1] = notepitches[1] - 1;
                RootNoteBool = true;
            }
            else if (ChordTerz == OvertoneInt)
            {
                ChordTerz = int.MaxValue;
            }
            else
            {

            }

            if (ChordQuint > OvertoneInt)
            {
                ChordQuint -= 13;
                notepitches[2] = notepitches[2] - 1;
                RootNoteBool = true;
            }
            else if (ChordQuint == OvertoneInt)
            {
                ChordQuint = int.MaxValue;
            }
            else
            {

            }

            if (ChordOktav > OvertoneInt)
            {
                ChordOktav -= 13;
                notepitches[3] = notepitches[3] - 1;
                RootNoteBool = true;
            }
            else if (ChordOktav == OvertoneInt)
            {
                ChordOktav = int.MaxValue;
            }
            else
            {

            }

            if (RootNoteBool)
            {
                RootNoteInt -= 13;
                notepitches[0] = notepitches[0] - 1;
            }

            for (int i = 0; i < notepitches.Length; i++)
            {
                notenames[i] = notenames[i] + notepitches[i].ToString();
            }
        }

        private void IsNotenamestwice()
        {
            var set = new HashSet<string>();
            var uniqueElements = new List<string>();

            foreach (string value in notenames)
            {
                if (!set.Add(value))
                {
                }
                else
                {
                    uniqueElements.Add(value);
                }
            }
            notenamesFinal = uniqueElements.ToArray();
        }

        public void PrintNotenames()
        {
            for (int i = 0; i < notenamesFinal.Length; i++)
            {
                Debug.WriteLine(notenamesFinal[i]);
            }
        }
    }
}
