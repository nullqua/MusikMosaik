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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace app.Components
{
    /// <summary>
    /// Interaction logic for NestedCodeBlock.xaml
    /// </summary>
    public partial class NestedCodeBlock : UserControl
    {
        public NestedCodeBlock()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type", typeof(string), typeof(NestedCodeBlock), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(
            "Count", typeof(string), typeof(NestedCodeBlock), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty LoopCodeBlockProperty = DependencyProperty.Register(
            "LoopCodeBlock", typeof(Border), typeof(NestedCodeBlock), new PropertyMetadata(default(Border)));

        public Border LoopCodeBlock
        {
            get => (Border)GetValue(LoopCodeBlockProperty);
            set => SetValue(LoopCodeBlockProperty, value);
        }

        public StackPanel CodeBlockContainer => container;

        public string Type
        {
            get => (string)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public string Count
        {
            get => (string)GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }

        private void Container_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(Border)) == LoopCodeBlock)
            {
                MessageBox.Show("Geschachtelte Schleifen sind nicht erlaubt.", "Fehler");
                e.Handled = true;
                return;
            }

            if (e.Data.GetData(typeof(Border)) is Border codeBlock)
            {
                if (container.Children.Contains(placementHint))
                {
                    container.Children.Remove(placementHint);
                }

                var newCodeBlock = new Border
                {
                    Width = 70,
                    Height = codeBlock.Height,
                    Background = codeBlock.Background,
                    Child = new TextBlock
                    {
                        Text = (codeBlock.Child as TextBlock)?.Text,
                        FontSize = (codeBlock.Child as TextBlock)?.FontSize ?? 12,
                        FontWeight = (codeBlock.Child as TextBlock)?.FontWeight ?? FontWeights.Normal,
                        HorizontalAlignment = (codeBlock.Child as TextBlock)?.HorizontalAlignment ?? HorizontalAlignment.Left,
                        VerticalAlignment = (codeBlock.Child as TextBlock)?.VerticalAlignment ?? VerticalAlignment.Top
                    }
                };

                container.Children.Insert(container.Children.Count - 1, newCodeBlock);

                // Update the width of the container to accommodate the new code block
                container.Width += 70;
                //outerBorder.Width += newCodeBlock.Width;
                //grid.Width += newCodeBlock.Width;
                //innerBorder.Width += newCodeBlock.Width;

                Debug.WriteLine(newCodeBlock.Width);
                e.Handled = true;
            }
        }

        private void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is StackPanel stackPanel)
            {
                this.Width = stackPanel.Width;
                this.Height = stackPanel.Height;
            }
        }   
    }
}
