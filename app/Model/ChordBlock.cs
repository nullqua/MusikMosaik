using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public string[] AllNotes = new string[]
        {
            "C", "CSharp", "D", "DSharp", "E", "F", "FSharp", "G", "GSharp", "A", "ASharp", "H"
        };
        string Mode;
        public string[] notenames;
        ChordBlock(Guid id,string basstone, string overtone, int pitch, string mode, int timenumerator, int timedenominatorint,int velocity)
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
            int BasstoneInt = int.MaxValue;
            int ChordTerz;
            int ChordQuint;
            for (int i = 0; i < AllNotes.Length; i++)
            {
                if (Basstone == AllNotes[i])
                {
                    BasstoneInt = i;
                    break;
                }
            }
            if(BasstoneInt ==  int.MaxValue)
            {
                throw new Exception("Basstone nicht richtig angegeben");
            }
            if(Mode == "Major")
            {
                ChordTerz = (BasstoneInt + 4) % 12;
                ChordQuint = (BasstoneInt + 7) % 12;
            }else if (Mode == "Minor")
            {
                ChordTerz = (BasstoneInt + 3) % 12;
                ChordQuint = (BasstoneInt + 7) % 12;
            }
            else
            {
                throw new Exception("Mode wrong falue different to Major/Minor");
            }
            notenames
        }
    }
}
