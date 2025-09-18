using Document.Base.Models.Interfaces;
using System.Reflection.Metadata;

namespace PdfExample;

internal class Program
{
    static void Main(string[] args)
    {
        IUserDocument pdf = new Document.Pdf.PdfDocument();
        var h1 = pdf.AddHeadline(1);
        h1.TextContent = "PDF Titel";
        var p = pdf.AddParagraph("Body");
        p.AppendText("Hallo PDF-Welt!");
        pdf.SaveTo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "example.pdf"));
    }
}
