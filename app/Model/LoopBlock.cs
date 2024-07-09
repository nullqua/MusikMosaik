namespace app.Model
{
    public class LoopBlock : MusicBlock
    {
        public List<MusicBlock> Blocks { get; set; }
        public int RepeatCount { get; set; }

        public LoopBlock(Guid id, int repeatCount)
        {
            Id = id;
            Blocks = [];
            RepeatCount = repeatCount;
        }

        public void AddLoopblock(MidiBuilder midiBuilder)
        {
            foreach (MusicBlock musicBlock in Blocks)
            {
                if (musicBlock == null)
                {
                    throw new Exception("MusicBlock is Leer");
                }
                else
                {
                    if (musicBlock is NoteBlock noteBlock)
                    {
                        midiBuilder.AddNote(noteBlock.Notename, noteBlock.MusicalTimeSpan);
                    }
                    else if (musicBlock is ChordBlock chordBlock)
                    {
                        midiBuilder.AddChord(chordBlock.NoteNames, chordBlock.TimeSpan);
                    }
                }
            }
            midiBuilder.Repeat(Blocks.Count, RepeatCount);
        }
    }
}
