using Document.Base.Models;
using Document.Odf;

namespace DocOdfTest;

internal class Program
{
    static void Main(string[] args)
    {
        // Neues Dokument erstellen
        var doc = new OdfTextDocument();

        // Überschrift hinzufügen
        var h1 = doc.AddHeadline(1);
        h1.AppendText("Mein Dokument");

        // Absatz hinzufügen
        var p = doc.AddParagraph("");
        p.AppendText("Dies ist ein ");
        p.AddSpan("fetter", EFontStyle.Bold);
        p.AppendText(" Text.");

        // Speichern
        doc.SaveTo("test.odt");

        // Laden
        doc.LoadFrom("existing.odt");
    }
}
