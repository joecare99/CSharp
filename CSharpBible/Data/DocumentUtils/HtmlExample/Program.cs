using Document.Base.Models.Interfaces;
using System.Diagnostics;
using Document.Base.Factories;
using Document.Base.Models;
using Document.Html.Model;

namespace HtmlExample;

public class Program
{
    static void Main(string[] args)
    {
        CreateSampleDocument();

        CreateHtmlDocument2();
    }

    private static void CreateHtmlDocument2()
    {
        var doc = UserDocumentFactory.Create("html");

        // Überschrift
        IDocContent h1 = doc.AddHeadline(1);
        h1.TextContent = "Mein erstes HTML-Dokument";

        // Absatz mit Text
        IDocParagraph p = doc.AddParagraph("Body");
        p.AppendText("Hallo Welt! ");
        p.AppendText("Dies ist ein einfacher Absatz.");

        // Optional: weitere Elemente
        p.AddLineBreak();
//        p.AddSpan("Fetter Text");

        // Dokument speichern
        var outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "example.html");
        var ok = doc.SaveTo(outputPath);

        Console.WriteLine(ok
            ? $"Gespeichert: {outputPath}"
            : "Fehler beim Speichern.");
    }

    private static void CreateSampleDocument()
    {
        // Beispiel: Dokument erstellen, schreiben und wieder lesen
        var root = UserDocumentFactory.Create("html").Root as IDocSection;
        var h1 = root.AddHeadline(1, "kap1");
        h1.TextContent = "Titel";
        var p = root.AddParagraph("Body");
        p.AppendText("Hallo ");
        p.AddSpan("Welt", EFontStyle.Default);
        p.AddLineBreak();
        var toc = root.AddTOC("Inhalt", 2);
        toc.RebuildFrom(root);

        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "test.html");
        Document.Html.IO.HtmlDocumentIO.SaveAsync(root as HtmlSection, path);
        var loaded = Document.Html.IO.HtmlDocumentIO.LoadAsync(path).Result;
        Debug.WriteLine(loaded);
    }
}
