using app.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        public CodeBlockOptionWindow(MusicBlock musicBlock)
        {
            InitializeComponent();

            var stackPanel = new StackPanel
            {
                Margin = new Thickness(10)
            };

            if (musicBlock is LoopBlock)
            {
                var label = new Label { Content = "Anzahl Durchläufe:" };
                var textBox = new TextBox { Text = "1" };
                stackPanel.Children.Add(label);
                stackPanel.Children.Add(textBox);
            }
            else if (musicBlock is NoteBlock)
            {
                stackPanel.Children.Add(new Label { Content = "Tone:" });
                var toneComboBox = new ComboBox();
                toneComboBox.Items.Add("Low");
                toneComboBox.Items.Add("Medium");
                toneComboBox.Items.Add("High");
                stackPanel.Children.Add(toneComboBox);

                stackPanel.Children.Add(new Label { Content = "Note Length:" });
                var noteLengthComboBox = new ComboBox();
                noteLengthComboBox.Items.Add("Short");
                noteLengthComboBox.Items.Add("Medium");
                noteLengthComboBox.Items.Add("Long");
                stackPanel.Children.Add(noteLengthComboBox);
            }
        }
    }
}
