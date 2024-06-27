using app.Model;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace app.Components
{
    /// <summary>
    /// Interaction logic for NestedCodeBlock.xaml
    /// </summary>
    public partial class NestedCodeBlock : UserControl
    {
        private List<MusicBlock> blocks;

        private readonly Guid id;

        private UIElement selected;
        private UIElement lastClickedInnerBlock;

        private DateTime lastClickTime = DateTime.MinValue;
        private DispatcherTimer clickTimer = new DispatcherTimer();

        public NestedCodeBlock(Guid id, ref List<MusicBlock> blocks, ref UIElement selected)
        {
            InitializeComponent();

            this.id = id;
            this.blocks = blocks;
            this.selected = selected;

            outerBorder.MouseLeftButtonDown += OuterBorder_MouseLeftButtonDown;
            outerBorder.Tag = new TagData(id, "Loop");

            clickTimer.Interval = TimeSpan.FromMilliseconds(500);
            clickTimer.Tick += OuterBorder_ClickTimer_Click;
        }

        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(
            "Count", typeof(string), typeof(NestedCodeBlock), new PropertyMetadata(default(string)));

        public StackPanel CodeBlockContainer => container;

        public string Count
        {
            get => (string)GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }
        
        private void Container_Drop(object sender, DragEventArgs e)
        {
            if ((e.Data.GetData(typeof(Border)) as Border).Tag.Equals("Loop"))
            {
                MessageBox.Show("Geschachtelte Schleifen sind nicht erlaubt.", "Fehler");
                e.Handled = true;
                return;
            }

            if (e.Data.GetData(typeof(Border)) is Border codeBlock)
            {
                var type = codeBlock.Tag as string;
                var guid = Guid.NewGuid();

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
                    },
                    Tag = new TagData(guid, type)
                };
                newCodeBlock.MouseLeftButtonDown += InnerBlock_MouseLeftButtonDown;

                var selfBlock = blocks.Find(x => x.Id == id) as LoopBlock;

                switch (type)
                {
                    case "Note":
                        selfBlock.Blocks.Add(new NoteBlock(guid, "C", 0, 4, 4, 100));
                        break;
                    case "Chord":
                        selfBlock.Blocks.Add(new ChordBlock(guid, "C", "E", 0, "Major", 4, 4, 100));
                        break;
                }

                container.Children.Insert(container.Children.Count - 1, newCodeBlock);

                container.Width += 70;

                e.Handled = true;
            }
        }

        private void InnerBlock_ClickTimer_Click(object sender, EventArgs e)
        {
            clickTimer.Stop();

            clickTimer.Tick -= InnerBlock_ClickTimer_Click;
            clickTimer.Tick += OuterBorder_ClickTimer_Click;

            Debug.WriteLine("mark inner block as selected");

            var elem = lastClickedInnerBlock as Border;

            if (selected != null)
            {
                elem.BorderBrush = Brushes.Transparent;
                elem.BorderThickness = new Thickness(0);

                selected = null;
            }
            else
            {
                selected = elem;

                elem.BorderBrush = Brushes.Orange;
                elem.BorderThickness = new Thickness(3);
            }
        }

        private void OuterBorder_ClickTimer_Click(object sender, EventArgs e)
        {
            clickTimer.Stop();

            Debug.WriteLine("mark loop block as selected");

            if (selected != null)
            {
                outerBorder.BorderBrush = Brushes.Transparent;
                outerBorder.BorderThickness = new Thickness(0);

                selected = null;
            }
            else
            {
                selected = outerBorder;

                outerBorder.BorderBrush = Brushes.Orange;
                outerBorder.BorderThickness = new Thickness(3);
            }
        }   

        private void InnerBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clickSpan = DateTime.Now - lastClickTime;
            var type = (sender as Border).Tag as TagData;

            if (clickSpan.TotalMilliseconds < 500)
            {
                clickTimer.Stop();
                Debug.WriteLine("double click, open inner block option dialog");

                var selfBlock = blocks.Find(x => x.Id == id) as LoopBlock;

                var res = selfBlock.Blocks.Find(x => x.Id == type.Id) as MusicBlock;

                var optionWindow = new CodeBlockOptionWindow(ref res);
                optionWindow.ShowDialog();
            }
            else
            {
                clickTimer.Stop();

                lastClickedInnerBlock = sender as UIElement;

                clickTimer.Tick -= OuterBorder_ClickTimer_Click;
                clickTimer.Tick += InnerBlock_ClickTimer_Click;

                clickTimer.Start();
            }

            lastClickTime = DateTime.Now;
            e.Handled = true;
        }

        private void OuterBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clickSpan = DateTime.Now - lastClickTime;
            var type = (sender as Border).Tag as TagData;

            if (clickSpan.TotalMilliseconds < 500)
            {
                clickTimer.Stop();
                Debug.WriteLine("double click, open loop block option dialog");

                var res = blocks.Find(x => x.Id == id) as MusicBlock;
                
                var optionWindow = new CodeBlockOptionWindow(ref res);
                optionWindow.ShowDialog();
            }
            else
            {
                clickTimer.Stop();
                clickTimer.Start();
            }

            lastClickTime = DateTime.Now;
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
