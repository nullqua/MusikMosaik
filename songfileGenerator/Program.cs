using System.Diagnostics;
using System.Drawing;
using System.IO.Compression;
using System.Xml.Linq;

namespace songfileGenerator
{
    internal class Program
    {
        private static readonly string MUSE_SCORE_PATH = "C:\\Program Files\\MuseScore 4\\bin\\MuseScore4.exe";

        static void Main(string[] args)
        {
            string inputFilePath;
            int measuresPerFile;
            string outputFilePath;

            if (args.Length == 3)
            {
                inputFilePath = args[0];
                measuresPerFile = int.Parse(args[1]);
                outputFilePath = args[2];
            }
            else
            {
                Console.WriteLine("Bitte geben Sie den Pfad zur Eingabedatei (.musicxml) an: ");
                inputFilePath = Console.ReadLine();

                Console.WriteLine("Bitte geben Sie die Anzahl der Takte pro Abschnitt an: ");
                measuresPerFile = int.Parse(Console.ReadLine());

                Console.WriteLine("Bitte geben Sie den Pfad und Namen der Ausgabedatei an: ");
                outputFilePath = Console.ReadLine();
            }

            if (inputFilePath.Length == 0 || measuresPerFile == 0 || outputFilePath.Length == 0)
            {
                Console.Error.WriteLine("Ungültige Eingabe.");
                return;
            }

            Console.WriteLine("Verarbeitung der Eingabedatei...");

            try
            {
                var doc = XDocument.Load(inputFilePath);

                doc.Descendants("lyric").Remove();

                string fullMidiPath = "score.mid";
                ConvertToMidi(doc, fullMidiPath);

                var measureElementsByPart = doc.Descendants("part").ToDictionary(
                    part => (string)part.Attribute("id"),
                    part => part.Elements("measure").ToList()
                );

                int totalMeasures = measureElementsByPart.First().Value.Count;
                int fileCount = (int)Math.Ceiling((double)totalMeasures / measuresPerFile);

                Console.WriteLine($"Erstelle {fileCount} Abschnitte mit je {measuresPerFile} Takten.");

                string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                Console.WriteLine($"Temporären Ordner erstellt ({tempDir}).");

                string sectionsDir = Path.Combine(tempDir, "sections");
                Directory.CreateDirectory("sections");

                Console.WriteLine("Erstelle Abschnitte...");

                for (int fileIndex = 0; fileIndex < fileCount; fileIndex++)
                {
                    var sectionDoc = new XDocument(new XElement("score-partwise"));

                    foreach (var headerElement in doc.Root.Elements().Where(e => e.Name != "part"))
                    {
                        sectionDoc.Root.Add(new XElement(headerElement));
                    }

                    foreach (var partElement in doc.Descendants("part"))
                    {
                        var newPart = new XElement(partElement.Name, partElement.Attributes());

                        var measuresToAdd = measureElementsByPart[(string)partElement.Attribute("id")]
                            .Skip(fileIndex * measuresPerFile)
                            .Take(measuresPerFile);

                        newPart.Add(measuresToAdd);
                        sectionDoc.Root.Add(newPart);
                    }

                    string sectionDir = Path.Combine(sectionsDir, $"section_{fileIndex + 1}");
                    Directory.CreateDirectory(sectionDir);

                    string sectionMidiPath = Path.Combine(sectionDir, "score.mid");
                    string sectionPngPathTemplate = Path.Combine(sectionDir, "score.png");

                    Console.WriteLine($"MIDI-Datei für Abschnitt {fileIndex + 1} erstellen...");

                    ConvertToMidi(sectionDoc, sectionMidiPath);

                    Console.WriteLine($"PNG-Datei für Abschnitt {fileIndex + 1} erstellen...");

                    ConvertToPng(sectionDoc, sectionPngPathTemplate);

                    string sectionPngPath = Directory.GetFiles(sectionDir, "score*.png").FirstOrDefault();
                    CropImage(sectionPngPath);
                }

                Console.WriteLine("Erstelle Archiv...");

                if (File.Exists(outputFilePath + ".mczip"))
                {
                    File.Delete(outputFilePath + ".mczip");
                }

                using (ZipArchive archive = ZipFile.Open(outputFilePath + ".mczip", ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(fullMidiPath, "score.mid");

                    foreach (var sectionPath in Directory.GetDirectories(sectionsDir))
                    {
                        string sectionName = new DirectoryInfo(sectionPath).Name;
                        string entryPath = $"sections/{sectionName}/";

                        archive.CreateEntryFromFile(Path.Combine(sectionPath, "score.mid"), $"{entryPath}score.mid");

                        string pngFile = Directory.GetFiles(sectionPath, "score*.png").FirstOrDefault();

                        archive.CreateEntryFromFile(pngFile, $"{entryPath}score.png");
                    }

                    // Metadata
                }

                Console.WriteLine("Temporäre Dateien werden bereinigt...");
                Directory.Delete(tempDir, true);
                File.Delete(fullMidiPath);

                Console.WriteLine("Die mczip-Datei wurde erfolgreich erstellt.");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Ein Fehler ist aufgetreten: " + e.Message);
            }   
        }

        private static void ConvertToMidi(XDocument xmlDoc, string outputPath)
        {
            string tempXmlPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".musicxml");
            xmlDoc.Save(tempXmlPath);

            Process process = new Process();
            process.StartInfo.FileName = MUSE_SCORE_PATH;
            process.StartInfo.Arguments = $"\"{tempXmlPath}\" -o \"{outputPath}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            
            try
            {
                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Fehler beim Konvertieren zu MIDI mit MuseScore. Ist MuseScore korrekt installiert?");
                Console.Error.WriteLine(e.Message);
            }

            File.Delete(tempXmlPath);
        }

        private static void ConvertToPng(XDocument xmlDoc, string outputPathTemplate)
        {
            string tempXmlPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".musicxml");
            xmlDoc.Save(tempXmlPath);

            Process process = new Process();
            process.StartInfo.FileName = MUSE_SCORE_PATH;
            process.StartInfo.Arguments = $"\"{tempXmlPath}\" -o \"{outputPathTemplate}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            try
            {
                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Fehler beim Konvertieren zu PNG mit MuseScore. Ist MuseScore korrekt installiert?");
                Console.Error.WriteLine(e.Message);
            }

            File.Delete(tempXmlPath);
        }

        private static void CropImage(string imagePath)
        {
            string tempPath = Path.GetTempFileName();

            using (var img = new Bitmap(imagePath))
            {
                var cropArea = new Rectangle(250, 450, 2600, 300);

                using (var croppedImg = img.Clone(cropArea, img.PixelFormat))
                {
                    croppedImg.Save(tempPath);
                }
            }

            File.Delete(imagePath);
            File.Move(tempPath, imagePath);
        }
    }
}
