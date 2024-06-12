using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.Model
{
    public class LoopBlock : MusicBlock
    {
        public List<MusicBlock> Blocks { get; set; }
        public int RepeatCount { get; set; }
    }
}
