using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DragDrop2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isDragging = false;
        private Point clickPosition;

        private bool selectActive = true;
        private UIElement selected;

        public MainWindow()
        {
            InitializeComponent();

            this.Focusable = true;
            this.Focus();

            selectBtn.IsEnabled = selectActive;
            penBtn.IsEnabled = !selectActive;

            foreach (UIElement child in panel.Children)
            {
                if (child is Border border)
                {
                    border.MouseMove += Border_MouseMove;
                }
            }
            canvas.Drop += Canvas_Drop;
            canvas.DragOver += Canvas_DragOver;
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var border = sender as Border;

                if (border != null)
                {
                    var data = new DataObject(typeof(Border), border);
                    DragDrop.DoDragDrop(border, border, DragDropEffects.Copy);
                }
            }
        }

        private void Canvas_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(Border)))
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.Copy;
            }
            e.Handled = true;
        }

        private void Canvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Border)))
            {
                var originalBorder = e.Data.GetData(typeof(Border)) as Border;
                if (originalBorder != null)
                {
                    if (originalBorder.Tag?.ToString() == "start" && CanvasContainsStartItem())
                    {
                        MessageBox.Show("Only one start item is allowed", "Error");
                        return;
                    }

                    var cloneBorder = new Border
                    {
                        Background = originalBorder.Background,
                        BorderBrush = originalBorder.BorderBrush,
                        BorderThickness = originalBorder.BorderThickness,
                        Height = originalBorder.Height,
                        Child = new TextBlock
                        {
                            Text = ((TextBlock)originalBorder.Child).Text,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        },
                        Tag = originalBorder.Tag
                    };

                    Canvas.SetLeft(cloneBorder, e.GetPosition(canvas).X);
                    Canvas.SetTop(cloneBorder, e.GetPosition(canvas).Y);
                    canvas.Children.Add(cloneBorder);

                    cloneBorder.MouseMove += CloneBorder_MouseMove;
                    cloneBorder.MouseLeftButtonDown += CloneBorder_MouseLeftButtonDown;
                    cloneBorder.MouseLeftButtonUp += CloneBorder_MouseLeftButtonUp;
                    cloneBorder.MouseLeave += CloneBorder_MouseLeave;
                    cloneBorder.MouseRightButtonDown += CloneBorder_MouseRightButtonDown;
                }
            }
        }

        private void CloneBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && sender is Border border)
            {
                var currentPosition = e.GetPosition(canvas);
                var left = currentPosition.X - clickPosition.X;
                var top = currentPosition.Y - clickPosition.Y;

                // Constrain the item within the canvas bounds
                left = Math.Max(left, 0);
                top = Math.Max(top, 0);
                left = Math.Min(left, canvas.ActualWidth - border.ActualWidth);
                top = Math.Min(top, canvas.ActualHeight - border.ActualHeight);

                Canvas.SetLeft(border, left);
                Canvas.SetTop(border, top);
            }
        }

        private void CloneBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;

            if (border != null)
            {
                if (selected is Border previous)
                {
                    previous.BorderBrush = Brushes.Black;
                    previous.BorderThickness = new Thickness(1);
                }

                selected = border;
                border.BorderBrush = Brushes.Red;
                border.BorderThickness = new Thickness(2);

                isDragging = true;
                clickPosition = e.GetPosition(border);
                border.CaptureMouse();
                e.Handled = true;
            }
        }

        private void CloneBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                var border = sender as Border;
                border?.ReleaseMouseCapture();
            }
        }

        private void CloneBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            isDragging = false;

            var border = sender as Border;
            border?.ReleaseMouseCapture();
        }

        private void CloneBorder_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("tbc", "Options");
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Delete || e.Key == Key.Back) && selected != null && canvas.Children.Contains(selected))
            {
                canvas.Children.Remove(selected);
                selected = null;
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selected is Border border)
            {
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new Thickness(1);
                selected = null;
            }
        }

        private bool CanvasContainsStartItem()
        {
            foreach (UIElement child in canvas.Children)
            {
                if (child is Border border && border.Tag?.ToString() == "start")
                {
                    return true;
                }
            }
            return false;
        }

        private void SelectPenToggle_Click(object sender, RoutedEventArgs e)
        {
            selectActive = !selectActive;

            selectBtn.IsEnabled = selectActive;
            penBtn.IsEnabled = !selectActive;
        }
    }
}