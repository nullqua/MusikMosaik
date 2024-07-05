using Melanchall.DryWetMidi.Interaction;

namespace app.Model
{
    public class NoteBlock : MusicBlock
    {
        public string Note { get; set; }
        public int Pitch { get; set; }
        public MusicalTimeSpan MusicalTimeSpan { get; set; }
        public int Velocity { get; set; }
        public string Notename { get; set; }

        public NoteBlock(Guid id, string note, int pitch, MusicalTimeSpan musicalTimeSpan, int velocity)
        {
            Id = id;
            Note = note;
            Pitch = pitch;
            MusicalTimeSpan = musicalTimeSpan;
            Velocity = velocity;
            Notename = note + pitch.ToString();
        }
        public void DoNotename()
        {
           Notename = Note + Pitch.ToString();
        }
    }
}
