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

        private DispatcherTimer clickTimer = new();

        private JObject metadata;

        private List<MusicBlock> blocks = [];
        private int countButtons = 0;

        private DirectoryInfo temporaryDirectory;

        private string fullMidiPath;

        private List<string> sectionMidiPaths = [];

        public MainWindow()
        {
            InitializeComponent();

            string[] files = Directory.GetFiles(@"..\..\..\examples");

            temporaryDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));

            foreach (string file in files)
            {
                try
                {
                    var extractPath = Path.Combine(temporaryDirectory.FullName, Path.GetFileNameWithoutExtension(file));
                    Directory.CreateDirectory(extractPath);

                    ZipFile.ExtractToDirectory(file, extractPath);

                    Button button = new Button
                    {
                        Content = Path.GetFileNameWithoutExtension(file)
                    };

                    button.Click += (sender, e) =>
                    {
                        mainPanel.Children.Clear();
                        ProcessArchive(Path.Combine(temporaryDirectory.FullName, Path.GetFileNameWithoutExtension(file)));

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

            clickTimer.Interval = TimeSpan.FromMilliseconds(System.Windows.Forms.SystemInformation.DoubleClickTime);
            clickTimer.Tick += ClickTimer_Tick;
        }

        private void ClickTimer_Tick(object sender, EventArgs e)
        {
            clickTimer.Stop();

            HandleSingleClick(sender, e);
        }

        private void HandleSingleClick(object sender, EventArgs e)
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

        private void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is Guid guid)
            {
                var block = blocks.Find(x => x.Id == guid);

                if (block != null)
                {
                    var optionWindow = new CodeBlockOptionWindow(ref block);
                    optionWindow.ShowDialog();
                }
            }
        }

        private void ProcessArchive(string directoryPath)
        {
            try
            {
                var metadataEntry = Path.Combine(directoryPath, "metadata.json");

                if (File.Exists(metadataEntry))
                {
                    string json = File.ReadAllText(metadataEntry);
                    metadata = JObject.Parse(json);
                }

                fullMidiPath = Path.Combine(directoryPath, "score.mid");

                var sectionDirectories = Directory.GetDirectories(Path.Combine(directoryPath, "sections"));
                
                foreach (var directory in sectionDirectories)
                {
                    var scorePng = Path.Combine(directoryPath, "sections", directory, "score.png");
                    var scoreMidi = Path.Combine(directoryPath, "sections", directory, "score.mid");

                    if (File.Exists(scorePng))
                    {
                        var bitmap = new BitmapImage();
                        using (FileStream stream = File.OpenRead(scorePng))
                        {
                            bitmap.BeginInit();
                            bitmap.StreamSource = stream;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                        }
                        bitmap.Freeze();

                        sectionMidiPaths.Add(scoreMidi);

                        AddRowToMainPanel(Path.GetFileName(directory), bitmap);
                        countButtons++;
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
            button1.Tag = countButtons;
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
            button2.Tag = countButtons;
            Button button3 = new Button { Content = "Remove all" };
            button3.Tag = countButtons;
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
            MessageBox.Show("Playing song");
        }

        private void CodeBlockPlay_Click(object sender, RoutedEventArgs e)
        {
            var buttoncount =  (sender as Button).Tag;
            Debug.WriteLine(buttoncount.ToString());
            MidiBuilder midiBuilder = new MidiBuilder();
            foreach (MusicBlock musicBlock in blocks)
            {
                if (musicBlock == null)
                {
                    throw new Exception("MusicBlock is Leer");
                }
                else
                {
                    if (musicBlock is NoteBlock)
                    {
                        NoteBlock noteBlock = (NoteBlock)musicBlock;
                        midiBuilder.addNote(noteBlock.Notename, noteBlock.MusicalTimeSpan);
                    }
                    else if (musicBlock is ChordBlock)
                    {
                        ChordBlock chordBlock = (ChordBlock)musicBlock;
                        midiBuilder.addChord(chordBlock.Notenames, chordBlock.MusicalTimeSpan);
                    }
                    else if (musicBlock is LoopBlock)
                    {
                        LoopBlock loopBlock = (LoopBlock)musicBlock;
                        loopBlock.addLoopblock(midiBuilder);
                    }
                    else
                    {
                        throw new Exception("Unknown type: " + musicBlock.GetType);
                    }
                }
            }
            midiBuilder.buildMidi("test", 100);
            MidiPlayer.MididateiAbspielen("test.mid");
            //MidiFileSequencer sequencer
            //MessageBox.Show("test.mid");
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
            if ((sender as FrameworkElement).Name == "outerBorder")
            {

            }
            //if (clickTimer.IsEnabled)
            //{
            //    clickTimer.Stop();

            //    HandleDoubleClick(sender, e);
            //}
            //else
            //{
            //    clickTimer.Start();
            //}
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
                var type = codeBlock.Tag as string;
                var guid = Guid.NewGuid();

                if (type == "Loop")
                {
                    var newCodeBlock = new NestedCodeBlock(guid, ref blocks, ref selected)
                    {
                        Width = 140,
                        Height = codeBlock.Height,
                        Count = "5"
                    };

                    blocks.Add(new LoopBlock(guid, 5));

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

                    switch (type)
                    {
                        case "Note":
                            blocks.Add(new NoteBlock(guid, "c", 60, 1, 4, 100));
                            break;
                        case "Chord":
                            blocks.Add(new ChordBlock(guid, "c", "h", 60, "major", 1, 4, 100));
                            break;
                    }

                    newCodeBlock.Focusable = true;
                    newCodeBlock.MouseLeftButtonDown += CodeBlock_MouseLeftButtonDown;
                    newCodeBlock.MouseRightButtonDown += CodeBlock_MouseRightButtonDown;

                    var stackPanel = (sender as Border).Parent as StackPanel;
                    stackPanel.Children.Insert(stackPanel.Children.Count - 1, newCodeBlock);
                }

                //var senderBlock = (sender as Border).Tag as MusicBlock;

                //MusicBlock newMusicBlock = null;

                //switch (type)
                //{
                //    case "Note":
                //        newMusicBlock = new NoteBlock(guid, "c", 60, 1, 4, 100);
                //        break;
                //    case "Chord":
                //        newMusicBlock = new ChordBlock(guid, "c", "h", 60, "major", 1, 4, 100);
                //        break;
                //    case "Loop":
                //        newMusicBlock = new LoopBlock
                //        {
                //            Id = guid,
                //            Blocks = [],
                //            RepeatCount = 5
                //        };
                //        break;
                //    default:
                //        throw new Exception("Unknown type: " + type);
                //}

                e.Handled = true;
            }
        }

        private void DeleteAll_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}