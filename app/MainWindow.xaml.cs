using app.Components;
using app.Model;
using MeltySynth;
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
using System.Windows.Threading;
using Melanchall.DryWetMidi.Interaction;

namespace app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UIElement selected;
        private UIElement lastClickedBlock;

        private readonly double clickDelay = 200;
        private DateTime lastClickTime = DateTime.MinValue;

        private DispatcherTimer clickTimer = new DispatcherTimer();

        private JObject metadata;

        private List<List<MusicBlock>> blocks = [];

        private string fullMidiPath;
        private List<string> sectionsMidiPath = [];
        
        private int sectionCount = 0;

        public MainWindow()
        {
            InitializeComponent();

            clickTimer.Interval = TimeSpan.FromMilliseconds(clickDelay);
            clickTimer.Tick += CodeBlock_ClickTimer_Tick;

            string[] files = Directory.GetFiles(@"..\..\..\examples");

            var tempDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));

            foreach (string file in files)
            {
                try 
                {
                    var extractPath = Path.Combine(tempDir.FullName, Path.GetFileNameWithoutExtension(file));
                    Directory.CreateDirectory(extractPath);

                    ZipFile.ExtractToDirectory(file, extractPath);

                    Button button = new Button
                    {
                        Content = Path.GetFileNameWithoutExtension(file)
                    };

                    button.Click += (sender, e) =>
                    {
                        mainPanel.Children.Clear();
                        ProcessArchive(Path.Combine(tempDir.FullName, Path.GetFileNameWithoutExtension(file)));

                        for (var idx = 0; idx < sectionsMidiPath.Count; idx++)
                        {
                            blocks.Add([]);
                        };

                        foreach (var button in songPanel.Children.OfType<Button>())
                        {
                            button.IsEnabled = true;
                        }

                        var clickedButton = (Button)sender;
                        clickedButton.IsEnabled = false;
                    };
                    songPanel.Children.Add(button);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
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

        private void ProcessArchive(string directoryPath)
        {
            try
            {
                var metadataPath = Path.Combine(directoryPath, "metadata.json");
                if (File.Exists(metadataPath))
                {
                    string json = File.ReadAllText(metadataPath);
                    metadata = JObject.Parse(json);
                }

                fullMidiPath = Path.Combine(directoryPath, "score.mid");

                var sectionDirectories = Directory.GetDirectories(Path.Combine(directoryPath, "sections"));

                foreach (var directory in sectionDirectories)
                {
                    var scorePngPath = Path.Combine(directory, "score.png");
                    var scoreMidiPath = Path.Combine(directory, "score.mid");

                    if (File.Exists(scorePngPath))
                    {
                        var bitmap = new BitmapImage();
                        using (FileStream stream = File.OpenRead(scorePngPath))
                        {
                            bitmap.BeginInit();
                            bitmap.StreamSource = stream;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                        }

                        bitmap.Freeze();

                        sectionsMidiPath.Add(Path.Combine(directoryPath, scoreMidiPath));

                        AddRowToMainPanel(Path.GetFileName(directory), bitmap);
                        sectionCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void AddRowToMainPanel(string directoryName, BitmapImage image)
        {
            var grid = new Grid
            {
                Margin = new Thickness(10)
            };
            var column1 = new ColumnDefinition
            {
                Width = new GridLength(100)
            };
            var column2 = new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            };
            grid.ColumnDefinitions.Add(column1);
            grid.ColumnDefinitions.Add(column2);
            var row1 = new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star)
            };
            var row2 = new RowDefinition
            {
                Height = new GridLength(3, GridUnitType.Star)
            };
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);

            var button1 = new Button { Content = "Play" };
            button1.Tag = sectionCount;
            Grid.SetColumn(button1, 0);
            Grid.SetRow(button1, 0);
            button1.Click += ScorePlay_Click;

            var imageView = new Image { Source = image };
            Grid.SetColumn(imageView, 1);
            Grid.SetRow(imageView, 0);

            var innerGrid = new Grid();
            innerGrid.RowDefinitions.Add(new RowDefinition());
            innerGrid.RowDefinitions.Add(new RowDefinition());
            var button2 = new Button { Content = "Play" };
            button2.Tag = sectionCount;
            var button3 = new Button { Content = "Remove all" };
            button3.Tag = sectionCount;
            Grid.SetRow(button2, 0);
            Grid.SetRow(button3, 1);
            innerGrid.Children.Add(button2);
            innerGrid.Children.Add(button3);
            Grid.SetColumn(innerGrid, 0);
            Grid.SetRow(innerGrid, 1);
            button2.Click += CodeBlockPlay_Click;
            button3.Click += DeleteAll_Click;

            var scrollViewer = new ScrollViewer()
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                MinHeight = 70
            };
            stackPanel.Drop += CodeBlocksPlacement_Drop;
            stackPanel.AllowDrop = true;
            stackPanel.Tag = sectionCount;
            Grid.SetColumn(scrollViewer, 1);
            Grid.SetRow(scrollViewer, 1);
            scrollViewer.Content = stackPanel;

            var border = new Border
            {
                AllowDrop = true,
                Width = 100,
                Margin = new Thickness(5),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(2),
                Background = Brushes.Transparent
            };
            border.Drop += CodeBlocksPlacement_Drop;
            var textBlock = new TextBlock
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
            MidiPlayer.PlayMidiFile(sectionsMidiPath[Convert.ToInt32((sender as Button).Tag)]);
            //MessageBox.Show("Playing song");
        }

        private void CodeBlockPlay_Click(object sender, RoutedEventArgs e)
        {
            var section = Convert.ToInt32((sender as Button).Tag);

            Debug.WriteLine(section.ToString());

            MidiBuilder midiBuilder = new();

            foreach (MusicBlock musicBlock in blocks[section])
            {
                if (musicBlock == null)
                {
                    throw new Exception("MusicBlock is Leer");
                }
                else
                {
                    if (musicBlock is NoteBlock noteBlock)
                    {
                        midiBuilder.addNote(noteBlock.Notename, noteBlock.MusicalTimeSpan);
                    }
                    else if (musicBlock is ChordBlock chordBlock)
                    {
                        midiBuilder.addChord(chordBlock.Notenames, chordBlock.MusicalTimeSpan);
                    }
                    else if (musicBlock is LoopBlock loopBlock)
                    {
                        loopBlock.addLoopblock(midiBuilder);
                    }
                    else
                    {
                        throw new Exception("Unknown type: " + musicBlock.GetType);
                    }
                }
            }

            midiBuilder.buildMidi("test", 100);
            MidiPlayer.PlayMidiFile("test.mid");
        }

        private void CodeBlocksPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border codeBlock)
            {
                DragDrop.DoDragDrop(codeBlock, codeBlock, DragDropEffects.Copy);
            }
        }

        private void CodeBlock_ClickTimer_Tick(object sender, EventArgs e)
        {
            clickTimer.Stop();

            var elem = lastClickedBlock as Border;

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

        private void CodeBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var section = Convert.ToInt32(((sender as Border).Parent as StackPanel).Tag);
            
            var currentClickTime = DateTime.Now;
            var clickSpan = currentClickTime - lastClickTime;

            if (clickSpan.TotalMilliseconds < 500)
            {
                clickTimer.Stop();

                var res = blocks[section].Find(x => x.Id == ((sender as Border).Tag as TagData).Id);
            
                var optionWindow = new CodeBlockOptionWindow(ref res);
                optionWindow.ShowDialog();
            }
            else
            {
                clickTimer.Stop();

                lastClickedBlock = sender as UIElement;

                clickTimer.Start();
            }

            lastClickTime = currentClickTime;
            e.Handled = true;
        }

        private void CodeBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender as Border == selected)
            {
                var section = Convert.ToInt32(((sender as Border).Parent as StackPanel).Tag);

                var res = blocks[section].Find(x => x.Id == ((sender as Border).Tag as TagData).Id);
                var parent = (sender as Border).Parent as StackPanel;

                blocks[section].Remove(res);
                parent.Children.Remove(sender as Border);

                selected = null;
            }
            e.Handled = true;
        }

        private void CodeBlocksPlacement_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(Border)) is Border codeBlock)
            {
                var section = Convert.ToInt32(((sender as Border).Parent as StackPanel).Tag);
                
                var type = codeBlock.Tag as string;
                var guid = Guid.NewGuid();

                if (type == "Loop")
                {
                    var newCodeBlock = new NestedCodeBlock(guid, section, ref blocks, ref selected)
                    {
                        Width = 140,
                        Height = codeBlock.Height,
                        Count = "5"
                    };

                    blocks[section].Add(new LoopBlock(guid, 5));

                    var stackPanel = (sender as Border).Parent as StackPanel;
                    stackPanel.Children.Insert(stackPanel.Children.Count - 1, newCodeBlock);
                }
                else
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
                        Tag = new TagData(guid, type),
                    };

                    newCodeBlock.BorderBrush = Brushes.Black;
                    newCodeBlock.BorderThickness = new Thickness(2);

                    switch (type)
                    {
                        case "Note":
                            blocks[section].Add(new NoteBlock(guid, "c", 4, new MusicalTimeSpan(1,1), 100));
                            break;
                        case "Chord":
                            blocks[section].Add(new ChordBlock(guid, "c", "b", 4, "Major", new MusicalTimeSpan(1,1), 100));
                            break;
                    }

                    newCodeBlock.Focusable = true;
                    newCodeBlock.MouseLeftButtonDown += CodeBlock_MouseLeftButtonDown;
                    newCodeBlock.MouseRightButtonDown += CodeBlock_MouseRightButtonDown;

                    var stackPanel = (sender as Border).Parent as StackPanel;
                    stackPanel.Children.Insert(stackPanel.Children.Count - 1, newCodeBlock);
                }

                e.Handled = true;
            }
        }

        private void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            var section = Convert.ToInt32((sender as Button).Tag);

            var panel = mainPanel
                .Children.OfType<Grid>()
                .SelectMany(g => g.Children.OfType<ScrollViewer>())
                .Select(sv => sv.Content)
                .OfType<StackPanel>()
                .FirstOrDefault(sp => Convert.ToInt32(sp.Tag) == section);

            var toRemove = panel.Children.Count - 1;

            for (var idx = 0; idx < toRemove; idx++)
            {
                panel.Children.RemoveAt(0);
            }

            blocks[section].Clear();
        }
    }
}