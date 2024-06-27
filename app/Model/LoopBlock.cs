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

        public LoopBlock(Guid id, int repeatCount)
        {
            Id = id;
            Blocks = new List<MusicBlock>();
            RepeatCount = repeatCount;
        }

        public void addLoopblock(MidiBuilder midiBuilder)
        {
            foreach (MusicBlock musicBlock in Blocks)
            {
                if (musicBlock != null)
                {
                    throw new Exception("MusicBlock is Leer");
                }
                else
                {
                    if (musicBlock is NoteBlock)
                    {
                        NoteBlock noteBlock = (NoteBlock)musicBlock;
                        midiBuilder.addNote(noteBlock.Notename, noteBlock.MusicalTimeSpan);
                    }
                    else if (musicBlock is ChordBlock)
                    {
                        ChordBlock chordBlock = (ChordBlock)musicBlock;
                        midiBuilder.addChord(chordBlock.Notenames, chordBlock.MusicalTimeSpan);
                    }
                }
            }
            midiBuilder.Repeat(Blocks.Count, RepeatCount);
        }
    }
}
