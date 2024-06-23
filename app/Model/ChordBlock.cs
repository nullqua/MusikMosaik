using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

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
            "C", "CSharp", "D", "DSharp", "E", "F", "FSharp", "G", "GSharp", "A", "ASharp", "H"
        };
        string Mode;
        public string[] Notenames = new string[5];
        public ChordBlock(Guid id, string basstone, string overtone, int pitch, string mode, int timenumerator, int timedenominatorint, int velocity)
        {
            Id = id;
            this.Basstone = basstone;
            this.Overtone = overtone;
            this.Pitch = pitch;
            this.Mode = mode;
            MusicalTimeSpan = new MusicalTimeSpan(timenumerator, timedenominatorint);
            this.Velocity = velocity;

        }

        private void FillNotenames()
        {
            int RootNoteInt = int.MaxValue;
            int OvertoneInt = int.MaxValue;
            int ChordTerz;
            int ChordQuint;
            Notenames[5] = Overtone + Pitch.ToString();
            for (int i = 0; i < AllNotes.Length; i++)
            {
                if (Basstone == AllNotes[i])
                {
                    RootNoteInt = i;
                    break;
                }
                if (Overtone == AllNotes[i])
                {
                    OvertoneInt = i;
                    break;
                }
            }
            if (RootNoteInt == int.MaxValue || RootNoteInt == int.MaxValue)
            {
                throw new Exception("Basstone nicht richtig angegeben");
            }
            if (Mode == "Major")
            {
                ChordTerz = (RootNoteInt + 4);
                ChordQuint = (RootNoteInt + 7);
            }
            else if (Mode == "Minor")
            {
                ChordTerz = (RootNoteInt + 3);
                ChordQuint = (RootNoteInt + 7);
            }
            else
            {
                throw new Exception("Mode wrong falue different to Major/Minor");
            }
            if (RootNoteInt < OvertoneInt && OvertoneInt <= ChordTerz)
            {
                if (true)
                {

                }
            }
            
        }
        private void MoveScale()
        {
            Boolean found = false;
            string[] AllNotesMoveToBase = AllNotes;
            int j = 0;
            int BasstoneInt = int.MaxValue;
            for (int i = 0; i < AllNotes.Length; i++)
            {
                if(Basstone == AllNotes[i])
                {
                    BasstoneInt = i;
                    found = true;
                }
                if(found)
                {
                    AllNotesMoveToBase[j] = AllNotes[i];
                    j++;
                }
            }
            if(!found)
            {
                throw new Exception("BassTon Falsch");
            }
            for(int i = 0;i < BasstoneInt; i++)
            {
                AllNotesMoveToBase[j] = AllNotes[i];
                j++;
            }
        }
    }
}
