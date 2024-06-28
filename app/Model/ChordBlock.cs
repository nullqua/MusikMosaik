using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Collections.Generic;

namespace app.Model
{
    public class ChordBlock : MusicBlock
    {
        public int Velocity;
        public MusicalTimeSpan MusicalTimeSpan;
        public string Basstone;
        public string Overtone;
        public int Pitch;
        private string[] AllNotes = {
            "c", "c#", "d", "d#", "e", "f", "f#", "g", "g#", "a", "a#", "b"
        };
        public string Mode;
        /// <summary>
        /// GrundTon1, Terz, Quinte, GrundTon2, MelodieTon
        /// </summary>
        public string[] Notenames = new string[5];
        /// <summary>
        /// GrundTon1, Terz, Quinte, GrundTon2, MelodieTon
        /// </summary>
        public int[] Notepitches = new int[5];
        public string[] NotenamesFinal;
        public ChordBlock(Guid id, string basstone, string overtone, int pitch, string mode, MusicalTimeSpan musicalTimeSpan, int velocity)
        {
            Id = id;
            this.Basstone = basstone;
            this.Overtone = overtone;
            this.Pitch = pitch;
            this.Mode = mode;
            MusicalTimeSpan = musicalTimeSpan;
            this.Velocity = velocity;
            for (int i = 0; i < Notepitches.Length; i++)
            {
                Notepitches[i] = pitch;
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
            Notenames[4] = Overtone + Pitch.ToString();
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
                throw new Exception("Basstone nicht richtig angegeben");
            }
            if (Mode == "Major")
            {
                Notenames[0] = Basstone;
                ChordTerz = (RootNoteInt + 4) % 12;
                Notenames[1] = AllNotes[ChordTerz % 12];
                ChordQuint = (RootNoteInt + 7) % 12;
                Notenames[2] = AllNotes[ChordQuint % 12];
                ChordOktav = (RootNoteInt);
                Notenames[3] = AllNotes[ChordOktav % 12];
                Notenames[4] = Overtone;
            }
            else if (Mode == "Minor")
            {
                Notenames[0] = Basstone;
                ChordTerz = (RootNoteInt + 3) % 12;
                Notenames[1] = AllNotes[ChordTerz % 12];
                ChordQuint = (RootNoteInt + 7) % 12;
                Notenames[2] = AllNotes[ChordQuint % 12];
                ChordOktav = (RootNoteInt);
                Notenames[3] = AllNotes[ChordOktav % 12];
                Notenames[4] = Overtone;
            }
            else
            {
                throw new Exception("Mode wrong falue different to Major/Minor");
            }
            RootNoteInt -= 13;
            Notepitches[0] = Notepitches[0] - 1;
            if (ChordTerz > OvertoneInt)
            {
                ChordTerz -= 13;
                Notepitches[1] = Notepitches[1] - 1;
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
                Notepitches[2] = Notepitches[2] - 1;
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
                Notepitches[3] = Notepitches[3] - 1;
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
                Notepitches[0] = Notepitches[0] - 1;
            }
            for (int i = 0; i < Notepitches.Length; i++)
            {
                Notenames[i] = Notenames[i] + Notepitches[i].ToString();
            }
        }

        private void IsNotenamestwice()
        {
            HashSet<string> set = new HashSet<string>();
            List<string> uniqueElements = new List<string>();
            foreach (string value in Notenames)
            {
                if (!set.Add(value))
                {
                }
                else
                {
                    uniqueElements.Add(value);
                }
            }
            NotenamesFinal = uniqueElements.ToArray();
        }
        public void printNotenames()
        {
            for (int i = 0; i < NotenamesFinal.Length; i++)
            {
                Debug.WriteLine(NotenamesFinal[i]);
            }
        }
    }
}
