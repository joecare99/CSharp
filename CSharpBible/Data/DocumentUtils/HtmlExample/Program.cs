using Document.Base.Models.Interfaces;
using System.Diagnostics;
using System.Reflection.Metadata;
using System;
using System.IO;
using Document.Html;

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
        var doc = new HtmlDocument();

        // Überschrift
        IDocContent h1 = doc.AddHeadline(1);
        h1.TextContent = "Mein erstes HTML-Dokument";

        // Absatz mit Text
        IDocParagraph p = doc.AddParagraph("Body");
        p.AppendText("Hallo Welt! ");
        p.AppendText("Dies ist ein einfacher Absatz.");

        // Optional: weitere Elemente
        p.AddLineBreak();
        p.AddSpan("Fetter Text", Document.Html.Model.HtmlFontStyle.Default);

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
        IDocSection root = new Document.Html.Model.HtmlSection();
        var h1 = (Document.Html.Model.HtmlHeadline)root.AddHeadline(1);
        h1.TextContent = "Titel";
        var p = root.AddParagraph("Body");
        p.AppendText("Hallo ");
        p.AddSpan("Welt", Document.Html.Model.HtmlFontStyle.Default);
        p.AddLineBreak();
        var toc = (Document.Html.Model.HtmlTOC)root.AddTOC("Inhalt", 2);
        toc.RebuildFrom(root);

        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "test.html");
        Document.Html.IO.HtmlDocumentIO.SaveAsync(root, path);
        var loaded = Document.Html.IO.HtmlDocumentIO.LoadAsync(path).Result;
        Debug.WriteLine(loaded);
    }
}
