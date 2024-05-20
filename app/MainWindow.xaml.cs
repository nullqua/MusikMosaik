using app.Components;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UIElement selected;

        public MainWindow()
        {
            InitializeComponent();

            foreach (UIElement child in codeBlocksPanel.Children)
            {
                if (child is Border)
                {
                    child.PreviewMouseLeftButtonDown += CodeBlocksPanel_PreviewMouseLeftButtonDown;
                }
            }
        }

        private void CodeBlocksPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (sender is Border codeBlock)
            {
                DragDrop.DoDragDrop(codeBlock, codeBlock, DragDropEffects.Copy);
            }
        }

        internal void CodeBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selected != null)
            {
                (selected as Border).BorderBrush = Brushes.Transparent;
                (selected as Border).BorderThickness = new Thickness(0);

                selected = null;
            }
            else 
            {
                selected = sender as UIElement;

                (selected as Border).BorderBrush = Brushes.Orange;
                (selected as Border).BorderThickness = new Thickness(2);

                selected.Focus();
                Keyboard.Focus(selected);
            }
        }

        internal void CodeBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            codeBlocksPlacement.Children.Remove(selected);
            selected = null;
        }

        private void CodeBlocksPlacement_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(Border)) is Border codeBlock)
            {
                dynamic newCodeBlock;

                if (codeBlock == loopCodeBlock)
                {
                    newCodeBlock = new NestedCodeBlock
                    {
                        Width = 140,
                        Height = codeBlock.Height,
                        Type = "Loop",
                        Count = "5",
                        LoopCodeBlock = loopCodeBlock
                    };
                }
                else
                {
                    newCodeBlock = new Border
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
                        border.Focusable = true;
                        border.MouseLeftButtonDown += CodeBlock_MouseLeftButtonDown;
                        border.MouseRightButtonDown += CodeBlock_MouseRightButtonDown;
                    }
                }
                
                codeBlocksPlacement.Children.Insert(codeBlocksPlacement.Children.Count - 1, newCodeBlock);
                
                e.Handled = true;
            }
        }
    }
}