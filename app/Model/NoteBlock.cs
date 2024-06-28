using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.Note = note;
            this.Pitch = pitch;
            this.MusicalTimeSpan = musicalTimeSpan;
            this.Velocity = velocity;
            this.Notename = note + pitch.ToString();
        }
        public void DoNotename()
        {
           this.Notename = this.Note + this.Pitch.ToString();
        }
    }
}
