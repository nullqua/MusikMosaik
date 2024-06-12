using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.Model
{
    public class ChordBlock : MusicBlock
    {
        public List<NoteBlock> Notes { get; set; }
    }
}
