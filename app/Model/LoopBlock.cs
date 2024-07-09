namespace app.Model
{
    /// <summary>
    /// Represents a counting (for) loop of music blocks
    /// </summary>
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

        /// <summary>
        /// Builds a MIDI sequence from the loop's music blocks and repeats it according to the loop's repeat count.
        /// </summary>
        /// <param name="midiBuilder">The MIDI builder instance used to build the sequence.</param>
        public void BuildMidiSequence(MidiBuilder midiBuilder)
        {
            foreach (MusicBlock musicBlock in Blocks)
            {
                if (musicBlock == null)
                {
                    throw new Exception("MusicBlock is empty");
                }
                else
                {
                    if (musicBlock is NoteBlock noteBlock)
                    {
                        midiBuilder.AddNote(noteBlock.NoteName, noteBlock.TimeSpan);
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
