using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.Model
{
    public class ChordBlock : MusicBlock
    {
        public int Velocity;
        public MusicalTimeSpan MusicalTimeSpan;
        public string Basstone;
        public string Overtone;
        public int Pitch;
        enum Mode{ Major, Minor};
        public string[] notenames;
        ChordBlock(string basstone, string overtone, int pitch, Enum mode, int timenumerator, int timedenominatorint,int velocity)
        {

        }
    }
}
