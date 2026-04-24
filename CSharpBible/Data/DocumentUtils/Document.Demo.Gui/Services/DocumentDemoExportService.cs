using Document.Base.Factories;
using Document.Base.Models;
using Document.Base.Models.Interfaces;
using Document.Demo.Gui.Models;

namespace Document.Demo.Gui.Services;

public sealed class DocumentDemoExportService
{
    private static readonly DemoFontStyle DefaultStyle = new();

    public DocumentExportResult Export(DemoExportRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OutputPath))
        {
            return new DocumentExportResult(DocumentExportStatus.InvalidPath);
        }

        try
        {
            DocumentProviderRegistry.EnsureRegistered();

            if (!UserDocumentFactory.TryCreate(request.FormatKey, out var document) || document is null)
            {
                return new DocumentExportResult(DocumentExportStatus.UnknownFormat);
            }

            BuildDemoContent(document, request.SelectedFeatures);

            if (!document.SaveTo(request.OutputPath))
            {
                return new DocumentExportResult(DocumentExportStatus.SaveFailed);
            }

            return new DocumentExportResult(DocumentExportStatus.Success);
        }
        catch (Exception exception)
        {
            return new DocumentExportResult(DocumentExportStatus.Error, exception);
        }
    }

    private static void BuildDemoContent(IUserDocument document, IReadOnlyCollection<DemoFeature> selectedFeatures)
    {
        document.AddHeadline(1, "demo-title").TextContent = "Dokumentausgabe-Demo";

        IDocTOC? toc = null;
        if (selectedFeatures.Contains(DemoFeature.TableOfContents))
        {
            toc = document.AddTOC("Inhaltsverzeichnis", 2);
        }

        if (selectedFeatures.Contains(DemoFeature.Headings))
        {
            BuildHeadingDemo(document);
        }

        if (selectedFeatures.Contains(DemoFeature.FontStyles))
        {
            BuildFontStylesDemo(document);
        }

        if (selectedFeatures.Contains(DemoFeature.FontFamiliesAndColors))
        {
            BuildFontsAndColorsDemo(document);
        }

        if (selectedFeatures.Contains(DemoFeature.Bookmarks))
        {
            BuildBookmarksDemo(document);
        }

        if (selectedFeatures.Contains(DemoFeature.Columns))
        {
            BuildColumnsDemo(document);
        }

        if (toc is not null && document.Root is IDocSection section)
        {
            toc.RebuildFrom(section);
        }
    }

    private static void BuildHeadingDemo(IUserDocument document)
    {
        document.AddHeadline(1, "kapitel-1").TextContent = "Kapitel 1";
        document.AddParagraph("Body").TextContent = "Dies ist ein Beispieltext unter einer Überschrift erster Ebene.";
        document.AddHeadline(2, "kapitel-1-1").TextContent = "Kapitel 1.1";
        document.AddParagraph("Body").TextContent = "Dies ist ein Beispieltext unter einer Überschrift zweiter Ebene.";
    }

    private static void BuildFontStylesDemo(IUserDocument document)
    {
        document.AddHeadline(2, "schriftstile").TextContent = "Schriftformate";

        var paragraph = document.AddParagraph("Body");
        paragraph.AppendText("Normal, ");
        paragraph.AddSpan("fett", EFontStyle.Bold);
        paragraph.AppendText(", ");
        paragraph.AddSpan("kursiv", EFontStyle.Italic);
        paragraph.AppendText(", ");
        paragraph.AddSpan("unterstrichen", EFontStyle.Underline);
        paragraph.AppendText(" und ");
        paragraph.AddSpan("durchgestrichen", EFontStyle.Strikeout);
        paragraph.AppendText(".");
    }

    private static void BuildFontsAndColorsDemo(IUserDocument document)
    {
        document.AddHeadline(2, "farben").TextContent = "Schriftarten und Farben";

        var paragraph = document.AddParagraph("Body");
        paragraph.AddSpan("Serif Rot", new DemoFontStyle(fontFamily: "Times New Roman", color: "#AA0000", fontSizePt: 12));
        paragraph.AppendText(" | ");
        paragraph.AddSpan("Sans Blau", new DemoFontStyle(fontFamily: "Arial", color: "#0033AA", bold: true, fontSizePt: 11));
        paragraph.AppendText(" | ");
        paragraph.AddSpan("Monospace Grün", new DemoFontStyle(fontFamily: "Consolas", color: "#0A7A0A", italic: true, fontSizePt: 10));
    }

    private static void BuildBookmarksDemo(IUserDocument document)
    {
        document.AddHeadline(2, "sprungmarken").TextContent = "Sprungmarken";

        var intro = document.AddParagraph("Body");
        intro.AddLink("#ziel-abschnitt", DefaultStyle).TextContent = "Zum Zielabschnitt springen";

        document.AddParagraph("Body").TextContent = "Zwischentext 1";
        document.AddParagraph("Body").TextContent = "Zwischentext 2";

        var targetParagraph = document.AddParagraph("Body");
        targetParagraph.AddBookmark("ziel-abschnitt", DefaultStyle).TextContent = "Zielabschnitt (Bookmark)";
    }

    private static void BuildColumnsDemo(IUserDocument document)
    {
        document.AddHeadline(2, "spalten").TextContent = "Spalten (Tabulator-basiert)";

        var header = document.AddParagraph("Body");
        header.AppendText("Name");
        header.AddTab(DefaultStyle);
        header.AppendText("Rolle");
        header.AddTab(DefaultStyle);
        header.AppendText("Status");

        AddColumnRow(document, "Anna", "Autorin", "Aktiv");
        AddColumnRow(document, "Ben", "Lektor", "Aktiv");
        AddColumnRow(document, "Chris", "Illustrator", "In Prüfung");
    }

    private static void AddColumnRow(IUserDocument document, string name, string role, string status)
    {
        var row = document.AddParagraph("Body");
        row.AppendText(name);
        row.AddTab(DefaultStyle);
        row.AppendText(role);
        row.AddTab(DefaultStyle);
        row.AppendText(status);
    }
}
