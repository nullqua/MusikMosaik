using app.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace app
{
    /// <summary>
    /// Interaction logic for CodeBlockOptionWindow.xaml
    /// </summary>
    public partial class CodeBlockOptionWindow : Window
    {
        private readonly MusicBlock musicBlock;

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
            var textBox = sender as TextBox;
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

            }
            else if (musicBlock is ChordBlock chordBlock)
            {

            }

            Close();
        }
    }
}
