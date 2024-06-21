using Melanchall.DryWetMidi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.Model
{
    public class NoteBlock : MusicBlock
    {
        public int Pitch { get; set; }
        public int Velocity { get; set; }
        public int DurationNominator { get; set; }

        public string Notename { get; set; }
        public string Note = Notename + Pitch.toString();
    }
}
