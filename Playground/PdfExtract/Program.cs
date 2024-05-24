using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Drawing;

namespace PdfExtract
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (PdfDocument doc = PdfDocument.Open("C:\\Users\\Florian\\Desktop\\Alle meine Entchen\\Alle meine Entchen.pdf"))
            {
                Console.WriteLine("read");
                var idx = 0;

                foreach (var page in doc.GetPages())
                {
                    var text = page.Text;

                    Console.WriteLine(text);

                    foreach (IPdfImage img in page.GetImages())
                    {
                        Console.WriteLine("image");

                        img.TryGetPng(out byte[] bytes);

                        if ( bytes == null)
                        {
                            continue;
                        }

                        Image pngImg = Image.FromStream(new MemoryStream(bytes));

                        pngImg.Save($"C:\\Users\\Florian\\Desktop\\Alle meine Entchen\\img_{idx}.png");
                        idx++;
                    }   
                }
            }
        }
    }
}
