using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

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

        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(
            "Count", typeof(string), typeof(NestedCodeBlock), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty IdProperty = DependencyProperty.Register(
            "Id", typeof(Guid), typeof(NestedCodeBlock), new PropertyMetadata(default(Guid)));

        public StackPanel CodeBlockContainer => container;

        public string Count
        {
            get => (string)GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }

        public Guid Id
        {
            get => (Guid)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }
        
        private void Container_Drop(object sender, DragEventArgs e)
        {
            Debug.WriteLine((e.Data.GetData(typeof(Border)) as Border).Tag);

            if ((e.Data.GetData(typeof(Border)) as Border).Tag.Equals("Loop"))
            {
                MessageBox.Show("Geschachtelte Schleifen sind nicht erlaubt.", "Fehler");
                e.Handled = true;
                return;
            }

            if (e.Data.GetData(typeof(Border)) is Border codeBlock)
            {
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

                if (newCodeBlock is Border border)
                {
                    border.MouseLeftButtonDown += (Application.Current.MainWindow as MainWindow).CodeBlock_MouseLeftButtonDown;
                    border.MouseRightButtonDown += (Application.Current.MainWindow as MainWindow).CodeBlock_MouseRightButtonDown;
                }
                container.Children.Insert(container.Children.Count - 1, newCodeBlock);

                container.Width += 70;

                e.Handled = true;
            }
        }

        private void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is StackPanel stackPanel)
            {
                Width = stackPanel.Width;
                Height = stackPanel.Height;
            }
        }   
    }
}
