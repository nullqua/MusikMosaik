﻿using app.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Melanchall.DryWetMidi.Interaction;

namespace app.Components
{
    public partial class NestedCodeBlock : UserControl
    {
        private List<List<MusicBlock>> blocks;

        private readonly Guid id;
        private readonly int sectionCount;

        private UIElement selected;
        private UIElement lastClickedInnerBlock;

        private DateTime lastClickTime = DateTime.MinValue;
        private readonly DispatcherTimer clickTimer = new();

        public NestedCodeBlock(Guid id, int sectionCount, ref List<List<MusicBlock>> blocks, ref UIElement selected)
        {
            InitializeComponent();

            this.id = id;
            this.sectionCount = sectionCount;
            this.blocks = blocks;
            this.selected = selected;

            outerBorder.BorderBrush = Brushes.Black;
            outerBorder.BorderThickness = new Thickness(2);
            outerBorder.MouseLeftButtonDown += OuterBorder_MouseLeftButtonDown;
            outerBorder.MouseRightButtonDown += OuterBorder_MouseRightButtonDown;
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

        /// <summary>
        /// Adds a code block to the stack panel and data structure when it is dropped.
        /// </summary>
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
                newCodeBlock.MouseRightButtonDown += InnerBlock_MouseRightButtonDown;
                newCodeBlock.BorderBrush = Brushes.Black;
                newCodeBlock.BorderThickness = new Thickness(2);

                var selfBlock = blocks[sectionCount].Find(x => x.Id == id) as LoopBlock;

                switch (type)
                {
                    case "Note":
                        selfBlock.Blocks.Add(new NoteBlock(guid, "c", 4,new MusicalTimeSpan(1,1), 100));
                        break;
                    case "Chord":
                        selfBlock.Blocks.Add(new ChordBlock(guid, "c", "e", 4, "Major", new MusicalTimeSpan(1, 1), 100));
                        break;
                }

                container.Children.Insert(container.Children.Count - 1, newCodeBlock);
                container.Width += 70;

                e.Handled = true;
            }
        }

        /// <summary>
        /// Ensures that double-clicking on a code block works.
        /// </summary>
        private void InnerBlock_ClickTimer_Click(object sender, EventArgs e)
        {
            clickTimer.Stop();

            clickTimer.Tick -= InnerBlock_ClickTimer_Click;
            clickTimer.Tick += OuterBorder_ClickTimer_Click;

            var elem = lastClickedInnerBlock as Border;

            if (selected != null)
            {
                elem.BorderBrush = Brushes.Black;
                elem.BorderThickness = new Thickness(2);

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

            if (selected != null)
            {
                outerBorder.BorderBrush = Brushes.Black;
                outerBorder.BorderThickness = new Thickness(2);

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

                var selfBlock = blocks[sectionCount].Find(x => x.Id == id) as LoopBlock;

                var res = selfBlock.Blocks.Find(x => x.Id == type.Id);

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

                var res = blocks[sectionCount].Find(x => x.Id == id) as MusicBlock;
                
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

        private void InnerBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender as Border == selected)
            {
                var type = (sender as Border).Tag as TagData;

                var selfBlock = blocks[sectionCount].Find(x => x.Id == id) as LoopBlock;

                selfBlock.Blocks.Remove(selfBlock.Blocks.Find(x => x.Id == type.Id));

                container.Children.Remove(sender as UIElement);
                container.Width -= 70;
            }
            
            e.Handled = true;
        }

        private void OuterBorder_MouseRightButtonDown(object sender, MouseButtonEventArgs e) => (Application.Current.MainWindow as MainWindow).CodeBlock_MouseRightButtonDown(outerBorder, e);

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
