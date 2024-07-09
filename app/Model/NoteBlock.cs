using Melanchall.DryWetMidi.Interaction;

namespace app.Model
{
    /// <summary>
    /// Represent a musical note block.
    /// </summary>
    public class NoteBlock : MusicBlock
    {
        public string Note { get; set; }
        public int Pitch { get; set; }
        public MusicalTimeSpan TimeSpan { get; set; }
        public int Velocity { get; set; }
        public string NoteName { get; set; }

        /// <summary>
        /// Creates a new instance of a note block.
        /// </summary>
        /// <param name="id">Unique identifier of code block.</param>
        /// <param name="note">The note.</param>
        /// <param name="pitch">The pitch.</param>
        /// <param name="timeSpan">The time span as fraction.</param>
        /// <param name="velocity">The velocity.</param>
        public NoteBlock(Guid id, string note, int pitch, MusicalTimeSpan timeSpan, int velocity)
        {
            Id = id;
            Note = note;
            Pitch = pitch;
            TimeSpan = timeSpan;
            Velocity = velocity;
            NoteName = note + pitch.ToString();
        }

        /// <summary>
        /// Builds the note for MIDI building.
        /// </summary>
        public void BuildNote()
        {
           NoteName = Note + Pitch.ToString();
        }
    }
}
