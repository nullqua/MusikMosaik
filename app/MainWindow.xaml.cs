using app.Components;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

        private void CodeBlocksPlacement_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(Border)) is Border codeBlock)
            {
                if (codeBlocksPlacement.Children.Contains(placementHint))
                {
                    codeBlocksPlacement.Children.Remove(placementHint);
                }

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
                }
                
                codeBlocksPlacement.Children.Add(newCodeBlock);
            }
        }
    }
}