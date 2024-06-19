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
        public int Duration { get; set; }

        public string Notename { get; set; }
    }
}
