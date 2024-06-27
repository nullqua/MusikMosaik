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
        private MusicBlock musicBlock;

        public CodeBlockOptionWindow(ref MusicBlock musicBlock)
        {
            InitializeComponent();

            this.musicBlock = musicBlock;

            var stackPanel = new StackPanel
            {
                Margin = new Thickness(10, 5, 10, 20)
            };

            if (musicBlock is LoopBlock block)
            {
                var label = new Label { Content = "Anzahl Durchläufe:" };
                var textBox = new TextBox { Text = "1" };

                textBox.PreviewTextInput += TextBox_PreviewTextInput;
                stackPanel.Children.Add(label);
                stackPanel.Children.Add(textBox);
            }
            else if (musicBlock is NoteBlock)
            {
                stackPanel.Children.Add(new Label { Content = "Ton:" });

                var noteComboBox = new ComboBox();

                noteComboBox.Items.Add("H#/C");
                noteComboBox.Items.Add("C#/Db");
                noteComboBox.Items.Add("D");
                noteComboBox.Items.Add("D#/Eb");
                noteComboBox.Items.Add("E/Fb");
                noteComboBox.Items.Add("E#/F");
                noteComboBox.Items.Add("F#/Gb");
                noteComboBox.Items.Add("G");
                noteComboBox.Items.Add("G#/Ab");
                noteComboBox.Items.Add("A");
                noteComboBox.Items.Add("A#/B");
                noteComboBox.Items.Add("H/Cb");

                stackPanel.Children.Add(noteComboBox);

                stackPanel.Children.Add(new Label { Content = "Tonhöhe:" });

                var pitchComboBox = new ComboBox();

                pitchComboBox.Items.Add("1 (tiefste Oktave)");
                pitchComboBox.Items.Add("2");
                pitchComboBox.Items.Add("3");
                pitchComboBox.Items.Add("4 (Mitte der Klaviatur)");
                pitchComboBox.Items.Add("5");
                pitchComboBox.Items.Add("6");
                pitchComboBox.Items.Add("7");
                pitchComboBox.Items.Add("8 (höchste Oktave)");

                stackPanel.Children.Add(pitchComboBox);

                stackPanel.Children.Add(new Label { Content = "Tonlänge:" });

                var noteLengthComboBox = new ComboBox();
                noteLengthComboBox.Items.Add("ganze Note");
                noteLengthComboBox.Items.Add("halbe Note");
                noteLengthComboBox.Items.Add("viertel Note");
                noteLengthComboBox.Items.Add("punktierte viertel Note");
                noteLengthComboBox.Items.Add("achtel Note");
                noteLengthComboBox.Items.Add("punktierte achtel Note");
                noteLengthComboBox.Items.Add("16tel Note");

                stackPanel.Children.Add(noteLengthComboBox);
            }
            else if (musicBlock is ChordBlock)
            {
                stackPanel.Children.Add(new Label { Content = "Tonlänge:" });

                var noteLengthComboBox = new ComboBox();
                noteLengthComboBox.Items.Add("ganze Note");
                noteLengthComboBox.Items.Add("halbe Note");
                noteLengthComboBox.Items.Add("viertel Note");
                noteLengthComboBox.Items.Add("punktierte viertel Note");
                noteLengthComboBox.Items.Add("achtel Note");
                noteLengthComboBox.Items.Add("punktierte achtel Note");
                noteLengthComboBox.Items.Add("16tel Note");

                stackPanel.Children.Add(noteLengthComboBox);

                stackPanel.Children.Add(new Label { Content = "Grundton:" });

                var baseTone = new ComboBox();

                baseTone.Items.Add("H#/C");
                baseTone.Items.Add("C#/Db");
                baseTone.Items.Add("D");
                baseTone.Items.Add("D#/Eb");
                baseTone.Items.Add("E/Fb");
                baseTone.Items.Add("E#/F");
                baseTone.Items.Add("F#/Gb");
                baseTone.Items.Add("G");
                baseTone.Items.Add("G#/Ab");
                baseTone.Items.Add("A");
                baseTone.Items.Add("A#/B");
                baseTone.Items.Add("H/Cb");

                stackPanel.Children.Add(baseTone);

                stackPanel.Children.Add(new Label { Content = "Höchster Ton:" });

                var overTone = new ComboBox();

                overTone.Items.Add("H#/C");
                overTone.Items.Add("C#/Db");
                overTone.Items.Add("D");
                overTone.Items.Add("D#/Eb");
                overTone.Items.Add("E/Fb");
                overTone.Items.Add("E#/F");
                overTone.Items.Add("F#/Gb");
                overTone.Items.Add("G");
                overTone.Items.Add("G#/Ab");
                overTone.Items.Add("A");
                overTone.Items.Add("A#/B");
                overTone.Items.Add("H/Cb");

                stackPanel.Children.Add(overTone);

                stackPanel.Children.Add(new Label { Content = "Oktave (höchste Note):" });

                var pitchComboBox = new ComboBox();

                pitchComboBox.Items.Add("1 (tiefste Oktave)");
                pitchComboBox.Items.Add("2");
                pitchComboBox.Items.Add("3");
                pitchComboBox.Items.Add("4 (Mitte der Klaviatur)");
                pitchComboBox.Items.Add("5");
                pitchComboBox.Items.Add("6");
                pitchComboBox.Items.Add("7");
                pitchComboBox.Items.Add("8 (höchste Oktave)");

                stackPanel.Children.Add(pitchComboBox);

                stackPanel.Children.Add(new Label { Content = "Akkord-Art:" });

                var scaleComboBox = new ComboBox();

                scaleComboBox.Items.Add("Dur");
                scaleComboBox.Items.Add("Moll");

                stackPanel.Children.Add(scaleComboBox);
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

        }
    }
}
