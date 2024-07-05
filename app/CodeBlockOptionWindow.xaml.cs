using app.Model;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Melanchall.DryWetMidi.Interaction;

namespace app
{
    public partial class CodeBlockOptionWindow : Window
    {
        private MusicBlock musicBlock;

        private readonly TextBox loopCount;
        private readonly ComboBox tone1;
        private readonly ComboBox tone2;
        private readonly ComboBox pitch;
        private readonly ComboBox noteLength;
        private readonly ComboBox scale;

        public CodeBlockOptionWindow(ref MusicBlock musicBlock)
        {
            InitializeComponent();

            this.musicBlock = musicBlock;

            var stackPanel = new StackPanel
            {
                Margin = new Thickness(10, 5, 10, 20)
            };

            if (musicBlock is LoopBlock)
            {
                var label = new Label { Content = "Anzahl Durchläufe:" };
                loopCount = new TextBox { Text = "1" };

                loopCount.PreviewTextInput += TextBox_PreviewTextInput;
                stackPanel.Children.Add(label);
                stackPanel.Children.Add(loopCount);
            }
            else if (musicBlock is NoteBlock)
            {
                stackPanel.Children.Add(new Label { Content = "Ton:" });

                tone1 = new ComboBox();

                tone1.Items.Add("H#/C");
                tone1.Items.Add("C#/Db");
                tone1.Items.Add("D");
                tone1.Items.Add("D#/Eb");
                tone1.Items.Add("E/Fb");
                tone1.Items.Add("E#/F");
                tone1.Items.Add("F#/Gb");
                tone1.Items.Add("G");
                tone1.Items.Add("G#/Ab");
                tone1.Items.Add("A");
                tone1.Items.Add("A#/B");
                tone1.Items.Add("H/Cb");

                stackPanel.Children.Add(tone1);

                stackPanel.Children.Add(new Label { Content = "Tonhöhe:" });

                pitch = new ComboBox();

                pitch.Items.Add("1 (tiefste Oktave)");
                pitch.Items.Add("2");
                pitch.Items.Add("3");
                pitch.Items.Add("4 (Mitte der Klaviatur)");
                pitch.Items.Add("5");
                pitch.Items.Add("6");
                pitch.Items.Add("7");
                pitch.Items.Add("8 (höchste Oktave)");

                stackPanel.Children.Add(pitch);

                stackPanel.Children.Add(new Label { Content = "Tonlänge:" });

                noteLength = new ComboBox();
                noteLength.Items.Add("ganze Note");
                noteLength.Items.Add("halbe Note");
                noteLength.Items.Add("viertel Note");
                noteLength.Items.Add("punktierte viertel Note");
                noteLength.Items.Add("achtel Note");
                noteLength.Items.Add("punktierte achtel Note");
                noteLength.Items.Add("16tel Note");

                stackPanel.Children.Add(noteLength);
            }
            else if (musicBlock is ChordBlock)
            {
                stackPanel.Children.Add(new Label { Content = "Tonlänge:" });

                noteLength = new ComboBox();
                noteLength.Items.Add("ganze Note");
                noteLength.Items.Add("halbe Note");
                noteLength.Items.Add("viertel Note");
                noteLength.Items.Add("punktierte viertel Note");
                noteLength.Items.Add("achtel Note");
                noteLength.Items.Add("punktierte achtel Note");
                noteLength.Items.Add("16tel Note");

                stackPanel.Children.Add(noteLength);

                stackPanel.Children.Add(new Label { Content = "Grundton:" });

                tone1 = new ComboBox();

                tone1.Items.Add("H#/C");
                tone1.Items.Add("C#/Db");
                tone1.Items.Add("D");
                tone1.Items.Add("D#/Eb");
                tone1.Items.Add("E/Fb");
                tone1.Items.Add("E#/F");
                tone1.Items.Add("F#/Gb");
                tone1.Items.Add("G");
                tone1.Items.Add("G#/Ab");
                tone1.Items.Add("A");
                tone1.Items.Add("A#/B");
                tone1.Items.Add("H/Cb");

                stackPanel.Children.Add(tone1);

                stackPanel.Children.Add(new Label { Content = "Höchster Ton:" });

                tone2 = new ComboBox();

                tone2.Items.Add("H#/C");
                tone2.Items.Add("C#/Db");
                tone2.Items.Add("D");
                tone2.Items.Add("D#/Eb");
                tone2.Items.Add("E/Fb");
                tone2.Items.Add("E#/F");
                tone2.Items.Add("F#/Gb");
                tone2.Items.Add("G");
                tone2.Items.Add("G#/Ab");
                tone2.Items.Add("A");
                tone2.Items.Add("A#/B");
                tone2.Items.Add("H/Cb");

                stackPanel.Children.Add(tone2);

                stackPanel.Children.Add(new Label { Content = "Oktave (höchste Note):" });

                pitch = new ComboBox();

                pitch.Items.Add("1 (tiefste Oktave)");
                pitch.Items.Add("2");
                pitch.Items.Add("3");
                pitch.Items.Add("4 (Mitte der Klaviatur)");
                pitch.Items.Add("5");
                pitch.Items.Add("6");
                pitch.Items.Add("7");
                pitch.Items.Add("8 (höchste Oktave)");

                stackPanel.Children.Add(pitch);

                stackPanel.Children.Add(new Label { Content = "Akkord-Art:" });

                scale = new ComboBox();

                scale.Items.Add("Dur");
                scale.Items.Add("Moll");

                stackPanel.Children.Add(scale);
            }

            panel.Children.Add(stackPanel);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (musicBlock is LoopBlock loopBlock)
            {
                if (loopCount.Text != "")
                {
                    loopBlock.RepeatCount = int.Parse(loopCount.Text);
                }
            }
            else if (musicBlock is NoteBlock noteBlock)
            {
                if (tone1 != null)
                {
                    if (tone1.Text == "H#/C") { noteBlock.Note = "c"; }
                    else if (tone1.Text == "C#/Db") { noteBlock.Note = "c#"; }
                    else if (tone1.Text == "D") { noteBlock.Note = "d"; }
                    else if (tone1.Text == "D#/Eb") { noteBlock.Note = "d#"; }
                    else if (tone1.Text == "E/Fb") { noteBlock.Note = "e"; }
                    else if (tone1.Text == "E#/F") { noteBlock.Note = "f"; }
                    else if (tone1.Text == "F#/Gb") { noteBlock.Note = "f#"; }
                    else if (tone1.Text == "G") { noteBlock.Note = "g"; }
                    else if (tone1.Text == "G#/Ab") { noteBlock.Note = "g#"; }
                    else if (tone1.Text == "A") { noteBlock.Note = "a"; }
                    else if (tone1.Text == "A#/B") { noteBlock.Note = "a#"; }
                    else if (tone1.Text == "H/Cb") { noteBlock.Note = "b"; }
                }
                if (pitch != null)
                {
                    if (pitch.Text == "1 (tiefste Oktave)") { noteBlock.Pitch = 1; }
                    else if (pitch.Text == "2") { noteBlock.Pitch = 2; }
                    else if (pitch.Text == "3") { noteBlock.Pitch = 3; }
                    else if (pitch.Text == "4 (Mitte der Klaviatur)") { noteBlock.Pitch = 4; }
                    else if (pitch.Text == "5") { noteBlock.Pitch = 5; }
                    else if (pitch.Text == "6") { noteBlock.Pitch = 6; }
                    else if (pitch.Text == "7") { noteBlock.Pitch = 7; }
                    else if (pitch.Text == "8 (höchste Oktave)") { noteBlock.Pitch = 8; }
                }
                if (noteLength != null)
                {
                    if (noteLength.Text == "ganze Note") { noteBlock.MusicalTimeSpan = new MusicalTimeSpan(1, 1); }
                    else if (noteLength.Text == "halbe Note") { noteBlock.MusicalTimeSpan = new MusicalTimeSpan(1, 2); }
                    else if (noteLength.Text == "viertel Note") { noteBlock.MusicalTimeSpan = new MusicalTimeSpan(1, 4); }
                    else if (noteLength.Text == "punktierte viertel Note") { noteBlock.MusicalTimeSpan = new MusicalTimeSpan(3, 8); }
                    else if (noteLength.Text == "achtel Note") { noteBlock.MusicalTimeSpan = new MusicalTimeSpan(1, 8); }
                    else if (noteLength.Text == "punktierte achtel Note") { noteBlock.MusicalTimeSpan = new MusicalTimeSpan(3, 16); }
                    else if (noteLength.Text == "16tel Note") { noteBlock.MusicalTimeSpan = new MusicalTimeSpan(1, 16); }
                }
                noteBlock.DoNotename();
            }
            else if (musicBlock is ChordBlock chordBlock)
            {
                if (tone1 != null)
                {
                    if (tone1.Text == "H#/C") { chordBlock.basstone = "c"; }
                    else if (tone1.Text == "C#/Db") { chordBlock.basstone = "c#"; }
                    else if (tone1.Text == "D") { chordBlock.basstone = "d"; }
                    else if (tone1.Text == "D#/Eb") { chordBlock.basstone = "d#"; }
                    else if (tone1.Text == "E/Fb") { chordBlock.basstone = "e"; }
                    else if (tone1.Text == "E#/F") { chordBlock.basstone = "f"; }
                    else if (tone1.Text == "F#/Gb") { chordBlock.basstone = "f#"; }
                    else if (tone1.Text == "G") { chordBlock.basstone = "g"; }
                    else if (tone1.Text == "G#/Ab") { chordBlock.basstone = "g#"; }
                    else if (tone1.Text == "A") { chordBlock.basstone = "a"; }
                    else if (tone1.Text == "A#/B") { chordBlock.basstone = "a#"; }
                    else if (tone1.Text == "H/Cb") { chordBlock.basstone = "b"; }
                }
                if (tone2 != null)
                {
                    if (tone2.Text == "H#/C") { chordBlock.overtone = "c"; }
                    else if (tone2.Text == "C#/Db") { chordBlock.overtone = "c#"; }
                    else if (tone2.Text == "D") { chordBlock.overtone = "d"; }
                    else if (tone2.Text == "D#/Eb") { chordBlock.overtone = "d#"; }
                    else if (tone2.Text == "E/Fb") { chordBlock.overtone = "e"; }
                    else if (tone2.Text == "E#/F") { chordBlock.overtone = "f"; }
                    else if (tone2.Text == "F#/Gb") { chordBlock.overtone = "f#"; }
                    else if (tone2.Text == "G") { chordBlock.overtone = "g"; }
                    else if (tone2.Text == "G#/Ab") { chordBlock.overtone = "g#"; }
                    else if (tone2.Text == "A") { chordBlock.overtone = "a"; }
                    else if (tone2.Text == "A#/B") { chordBlock.overtone = "a#"; }
                    else if (tone2.Text == "H/Cb") { chordBlock.overtone = "b"; }
                }
                if (pitch != null)
                {
                    if (pitch.Text == "1 (tiefste Oktave)") { chordBlock.pitch = 1; }
                    else if (pitch.Text == "2") { chordBlock.pitch = 2; }
                    else if (pitch.Text == "3") { chordBlock.pitch = 3; }
                    else if (pitch.Text == "4 (Mitte der Klaviatur)") { chordBlock.pitch = 4; }
                    else if (pitch.Text == "5") { chordBlock.pitch = 5; }
                    else if (pitch.Text == "6") { chordBlock.pitch = 6; }
                    else if (pitch.Text == "7") { chordBlock.pitch = 7; }
                    else if (pitch.Text == "8 (höchste Oktave)") { chordBlock.pitch = 8; }
                }
                if (noteLength != null)
                {
                    if (noteLength.Text == "ganze Note") { chordBlock.musicalTimeSpan = new MusicalTimeSpan(1, 1); }
                    else if (noteLength.Text == "halbe Note") { chordBlock.musicalTimeSpan = new MusicalTimeSpan(1, 2); }
                    else if (noteLength.Text == "viertel Note") { chordBlock.musicalTimeSpan = new MusicalTimeSpan(1, 4); }
                    else if (noteLength.Text == "punktierte viertel Note") { chordBlock.musicalTimeSpan = new MusicalTimeSpan(3, 8); }
                    else if (noteLength.Text == "achtel Note") { chordBlock.musicalTimeSpan = new MusicalTimeSpan(1, 8); }
                    else if (noteLength.Text == "punktierte achtel Note") { chordBlock.musicalTimeSpan = new MusicalTimeSpan(3, 16); }
                    else if (noteLength.Text == "16tel Note") { chordBlock.musicalTimeSpan = new MusicalTimeSpan(1, 16); }
                }
                if (scale != null)
                {
                    if (scale.Text == "Dur") { chordBlock.mode = "Major"; }
                    else if (scale.Text == "Moll") { chordBlock.mode = "Minor"; }
                }
                chordBlock.FillNotenames();
            }
            Close();
        }
    }
}
