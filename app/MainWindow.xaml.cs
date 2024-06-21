using app.Components;
using app.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UIElement selected;

        private readonly double clickDelay = 200;
        private DateTime lastClickTime = DateTime.MinValue;

        private JObject metadata;

        private List<MusicBlock> blocks = new List<MusicBlock>();

        public MainWindow()
        {
            InitializeComponent();

            string[] files = Directory.GetFiles(@"..\..\..\examples");

            foreach (string file in files)
            {
                Button button = new Button
                {
                    Content = Path.GetFileNameWithoutExtension(file)
                };

                button.Click += (sender, e) => {
                    mainPanel.Children.Clear();
                    OpenFile(file);

                    foreach (var button in songPanel.Children.OfType<Button>())
                    {
                        button.IsEnabled = true;
                    }

                    var clickedButton = (Button)sender;
                    clickedButton.IsEnabled = false;
                };
                songPanel.Children.Add(button);
            }

            TextBlock placeholder = new TextBlock
            {
                Text = "Click a song to load",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(20)
            };
            mainPanel.Children.Add(placeholder);


            foreach (UIElement child in codeBlocksPanel.Children)
            {
                if (child is Border)
                {
                    child.PreviewMouseLeftButtonDown += CodeBlocksPanel_PreviewMouseLeftButtonDown;
                }
            }
        }

        private void OpenFile(string filePath)
        {
            using (ZipArchive archive = ZipFile.OpenRead(filePath))
            {
                var metadataEntry = archive.Entries.FirstOrDefault(e => e.FullName == "metadata.json");
                if (metadataEntry != null)
                {
                    using (Stream stream = metadataEntry.Open())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string json = reader.ReadToEnd();

                            metadata = JObject.Parse(json);
                            Debug.WriteLine(metadata);
                        }
                    }
                }

                var sectionDireories = archive.Entries
                    .Where(e => e.FullName.StartsWith("sections/") && e.FullName.EndsWith("/"))
                    .Select(e => e.FullName.Split('/')[1])
                    .Distinct()
                    .ToList();


                foreach (var directory in sectionDireories)
                {
                    var scorePngEntry = archive.Entries.FirstOrDefault(e => e.FullName == $"sections/{directory}/score.png");
                    if (scorePngEntry != null)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        using (Stream stream = scorePngEntry.Open())
                        {
                            MemoryStream ms = new MemoryStream();
                            stream.CopyTo(ms);
                            ms.Position = 0;

                            bitmap.BeginInit();
                            bitmap.StreamSource = ms;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                        }
                        AddRowToMainPanel(directory, bitmap);
                    }
                }
            }   
        }

        private void AddRowToMainPanel(string directoryName, BitmapImage image)
        {
            Grid grid = new Grid
            {
                Margin = new Thickness(10)
            };
            ColumnDefinition column1 = new ColumnDefinition
            {
                Width = new GridLength(100)
            };
            ColumnDefinition column2 = new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            };
            grid.ColumnDefinitions.Add(column1);
            grid.ColumnDefinitions.Add(column2);
            RowDefinition row1 = new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star)
            };
            RowDefinition row2 = new RowDefinition
            {
                Height = new GridLength(3, GridUnitType.Star)
            };
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);

            Button button1 = new Button { Content = "Play" };
            Grid.SetColumn(button1, 0);
            Grid.SetRow(button1, 0);
            button1.Click += ScorePlay_Click;

            Image imageView = new Image { Source = image };
            Grid.SetColumn(imageView, 1);
            Grid.SetRow(imageView, 0);

            Grid innerGrid = new Grid();
            innerGrid.RowDefinitions.Add(new RowDefinition());
            innerGrid.RowDefinitions.Add(new RowDefinition());
            Button button2 = new Button { Content = "Play" };
            Button button3 = new Button { Content = "Remove all" };
            Grid.SetRow(button2, 0);
            Grid.SetRow(button3, 1);
            innerGrid.Children.Add(button2);
            innerGrid.Children.Add(button3);
            Grid.SetColumn(innerGrid, 0);
            Grid.SetRow(innerGrid, 1);
            button2.Click += CodeBlockPlay_Click;

            ScrollViewer scrollViewer = new ScrollViewer 
            { 
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                MinHeight = 70
            };
            stackPanel.Drop += CodeBlocksPlacement_Drop;
            stackPanel.AllowDrop = true;
            Grid.SetColumn(scrollViewer, 1);
            Grid.SetRow(scrollViewer, 1);
            scrollViewer.Content = stackPanel;

            Border border = new Border
            {
                AllowDrop = true,
                Width = 100,
                Margin = new Thickness(5),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(2),
                Background = Brushes.Transparent
            };
            border.Drop += CodeBlocksPlacement_Drop;
            TextBlock textBlock = new TextBlock 
            {
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = "+"
            };
            stackPanel.Children.Add(border);
            border.Child = textBlock;

            grid.Children.Add(button1);
            grid.Children.Add(imageView);
            grid.Children.Add(innerGrid);
            grid.Children.Add(scrollViewer);

            mainPanel.Children.Add(grid);
        }

        private void ScorePlay_Click(object sender, RoutedEventArgs e)
        {
            foreach(MusicBlock musicBlock in blocks)
            {
                if (musicBlock != null)
                {
                    throw new Exception("MusicBlock is Leer");
                }
                else
                {
                    MidiBuilder midiBuilder = new MidiBuilder();
                    if (musicBlock is NoteBlock)
                    {
                        string notename = ((NoteBlock)musicBlock).Notename;
                        string pitch = $"{((NoteBlock)musicBlock).Pitch}";


                        midiBuilder.addNote(notename + pitch, );
                    }
                    else if (musicBlock is ChordBlock)
                    {

                    }
                    else if (musicBlock is LoopBlock)
                    {

                    }
                    else if (musicBlock is MusicBlock)
                    {

                    }
                    else
                    {
                        throw new Exception("Unknown type: " + musicBlock.GetType);
                    }
                }
            MessageBox.Show("Playing song");
        }

        private void CodeBlockPlay_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Playing code block");
        }

        private void StackPanel_Drop(object sender, DragEventArgs e)
        {
            throw new NotImplementedException();
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
            TimeSpan timeSinceLastClick = DateTime.Now - lastClickTime;

            if (timeSinceLastClick.TotalMilliseconds <= clickDelay)
            {
                var res = blocks.Find(x => x.id == (Guid)(sender as Border).Tag);
                var optionWindow = new CodeBlockOptionWindow(res);
                optionWindow.ShowDialog();

            }
            else
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
                }
            }

            lastClickTime = DateTime.Now;
        }

        internal void CodeBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // codeBlocksPlacement.Children.Remove(selected);
            selected = null;
        }

        internal void CodeBlocksPlacement_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(Border)) is Border codeBlock)
            {
                var senderBlock = (sender as Border).Tag as MusicBlock;

                MusicBlock newMusicBlock = null;
                
                var guid = Guid.NewGuid();
                var type = codeBlock.Tag as string;

                switch (type)
                {
                    case "Note":
                        newMusicBlock = new NoteBlock
                        {
                            id = guid,
                            Pitch = 60,
                            Velocity = 100,
                            Duration = 1
                        };
                        break;
                    case "Chord":
                        newMusicBlock = new ChordBlock
                        {
                            id = guid,
                            Notes = new List<NoteBlock>()
                        };
                        break;
                    case "Loop":
                        newMusicBlock = new LoopBlock
                        {
                            id = guid,
                            Blocks = new List<MusicBlock>(),
                            RepeatCount = 5
                        };
                        break;
                    default:
                        throw new Exception("Unknown type: " + type);
                }

                if (type == "Loop")
                {
                    var newCodeBlock = new NestedCodeBlock
                    {
                        Width = 140,
                        Height = codeBlock.Height,
                        Count = "5",
                        Id = guid
                    };

                    blocks.Add(newMusicBlock);

                    var stackPanel = (sender as Border).Parent as StackPanel;
                    stackPanel.Children.Insert(stackPanel.Children.Count - 1, newCodeBlock);
                }
                else
                {
                    if (senderBlock is not LoopBlock)
                    {
                        var newCodeBlock = new Border
                        {
                            Width = 70,
                            Height = 70,
                            Background = codeBlock.Background,
                            Child = new TextBlock
                            {
                                Text = (codeBlock.Child as TextBlock)?.Text,
                                FontSize = (codeBlock.Child as TextBlock)?.FontSize ?? 12,
                                FontWeight = (codeBlock.Child as TextBlock)?.FontWeight ?? FontWeights.Normal,
                                HorizontalAlignment = (codeBlock.Child as TextBlock)?.HorizontalAlignment ?? HorizontalAlignment.Left,
                                VerticalAlignment = (codeBlock.Child as TextBlock)?.VerticalAlignment ?? VerticalAlignment.Top
                            },
                            Tag = guid
                        };

                        blocks.Add(newMusicBlock);

                        newCodeBlock.Focusable = true;
                        newCodeBlock.MouseLeftButtonDown += CodeBlock_MouseLeftButtonDown;
                        newCodeBlock.MouseRightButtonDown += CodeBlock_MouseRightButtonDown;

                        var stackPanel = (sender as Border).Parent as StackPanel;
                        stackPanel.Children.Insert(stackPanel.Children.Count - 1, newCodeBlock);
                    }
                }

                e.Handled = true;
            }
        }

        private void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}