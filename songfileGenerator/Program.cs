using System.Xml.Linq;

namespace songfileGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string inputFilePath;
            int barsPerSection;
            string outputFilePath;

            if (args.Length == 3)
            {
                inputFilePath = args[0];
                barsPerSection = int.Parse(args[1]);
                outputFilePath = args[2];
            }
            else
            {
                Console.Write("Bitte geben Sie den Pfad zur MusicXML-Datei ein: ");
                inputFilePath = Console.ReadLine();

                Console.Write("Wie viele Takte soll jeder Abschnitt haben? ");
                barsPerSection = int.Parse(Console.ReadLine());

                Console.Write("Bitte geben Sie den Zielpfad für den Output ein: ");
                outputFilePath = Console.ReadLine();
            }

            //TODO: Fehlerbehandlung

            XElement musicXml = XElement.Load(inputFilePath);
            SplitIntoSections(musicXml, barsPerSection);

            //TODO: mczip Archiv erstellen

            Console.WriteLine("Die mczip Datei wurde erfolgreich erstellt.");
        }

        private static void SplitIntoSections(XElement musicXml, int barsPerSection)
        {
            
        }
    }
}
