using System.Text;
using System.Text.Json;
using System.Globalization;
using Document.Base.Factories;
using Document.Base.Models.Interfaces;
using Document.Html;
using Story_Base.Data;
using Document.Xaml;

namespace NebelEbook;

class Program
{
    // Caches für häufig genutzte Daten
    private static readonly HashSet<char> s_invalidFileNameChars = new(Path.GetInvalidFileNameChars());
    private static readonly HashSet<string> s_reservedNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "CON","PRN","AUX","NUL",
        "COM1","COM2","COM3","COM4","COM5","COM6","COM7","COM8","COM9",
        "LPT1","LPT2","LPT3","LPT4","LPT5","LPT6","LPT7","LPT8","LPT9"
    };

    static int Main(string[] args)
    {
        try
        {

            Init(args, out Story story, out string output, out IUserDocument doc);

            return Execute(story, output, doc);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static int Execute(Story story, string output, IUserDocument doc)
    {
        BuildDocument(doc, story);

        if (!doc.SaveTo(output))
        {
            Console.Error.WriteLine($"Speichern fehlgeschlagen: {output}");
            return 2;
        }

        Console.WriteLine($"Erstellt: {Path.GetFullPath(output)}");
        return 0;
    }

    private static void Init(string[] args, out Story story, out string output, out IUserDocument doc)
    {
        UserDocumentFactory.AutoScanOnFirstUse = false;

        // Manuelle Registrierung für HTML
        UserDocumentFactory.Register<HtmlDocument>("html", [".htm",".html"], "text/html");
        UserDocumentFactory.Register<XamlDocument>("xaml", [".xaml",".xml"], "text/xaml");
    //    UserDocumentFactory.Register<PdfDocument>("pdf", [".pdf" ], "application/pdf");

        var key = (args.Length > 0 ? args[0] : "html").Trim().ToLowerInvariant();
        var storyPath = Path.Combine(AppContext.BaseDirectory,"Resources", "story2.json");

        // Story laden (oder Beispiel erzeugen) -> liefert Titel
        story = LoadOrCreateSampleStory(storyPath);

        // Aus Titel der Story einen gültigen Dateinamen erzeugen
        var fileName = BuildOutputFileNameFromTitle(story?.Title, key);
        output = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

        // Dokument über Factory erstellen (per Key 'pdf'/'html' oder per Dateiendung)
        doc = UserDocumentFactory.Create(key);
    }

    private static string BuildOutputFileNameFromTitle(string? title, string key)
    {
        // Fallback-Titel
        title = string.IsNullOrWhiteSpace(title) ? "output" : title;

        // Ein-Pass-Aufbereitung: Diakritika entfernen, ungültige Zeichen ersetzen, Separatoren verdichten
        var sb = new StringBuilder(title.Length);
        bool lastWasSep = false;

        foreach (var c in title.Normalize(NormalizationForm.FormD))
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc == UnicodeCategory.NonSpacingMark)
                continue; // Diakritika überspringen

            char mapped;
            if (char.IsWhiteSpace(c) || c == '-' || c == '_' || c == '.')
            {
                mapped = '_';
            }
            else if (s_invalidFileNameChars.Contains(c))
            {
                mapped = '-';
            }
            else
            {
                mapped = c;
            }

            if (mapped == '_')
            {
                if (!lastWasSep)
                {
                    sb.Append('_');
                    lastWasSep = true;
                }
            }
            else
            {
                sb.Append(mapped);
                lastWasSep = false;
            }
        }

        var baseName = sb.ToString().Normalize(NormalizationForm.FormC).Trim('_');
        if (string.IsNullOrEmpty(baseName))
            baseName = "output";

        // Windows-reservierte Namen vermeiden
        if (s_reservedNames.Contains(baseName))
        {
            baseName += "_";
        }

        // Extension aus key bestimmen (Fallback .html)
        var ext = key?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(ext)) ext = "html";
        if (!ext.StartsWith('.')) ext = "." + ext;

        return baseName + ext;
    }

    private static void BuildDocument(IUserDocument doc, Story story)
    {
        var font = BasicFontStyle.Instance;

        // Inhaltsverzeichnis
        {
            var hToc = doc.AddHeadline(1);
            hToc.TextContent = "Inhaltsverzeichnis";

            foreach (var n in story.Nodes)
            {
                var p = doc.AddParagraph("TOCEntry");
                var link = p.AddLink("#"+n.Id,font);
                link.TextContent = TextTemplate.Render(n.Title, story.Variables);
                link.SetStyle(doc, font);
            }
        }

        // Kapitel
        foreach (var node in story.Nodes)
        {
            var h = doc.AddHeadline(1,node.Id);
            h.TextContent = TextTemplate.Render(node.Title, story.Variables);

            var anchor = h.AddSpan(font);
            (anchor as IDOMElement).Attributes["id"] = node.Id;

            foreach (var para in node.Paragraphs)
            {
                var p = doc.AddParagraph("Body");
                p.TextContent = TextTemplate.Render(para, story.Variables);
            }

            if (node.Choices is { Count: > 0 })
            {
                var ph = doc.AddParagraph("ChoiceHeader");
                ph.TextContent = "Deine Wahl:";

                foreach (var c in node.Choices)
                {
                    var cp = doc.AddParagraph("Choice");
                    var l = cp.AddLink("#"+c.TargetId,font);
                    l.TextContent = "→ " + TextTemplate.Render(c.Label, story.Variables);
                }
            }
        }
    }

    static Story LoadOrCreateSampleStory(string path)
    {
        if (File.Exists(path))
        {
            using var fs = File.OpenRead(path);
            return JsonSerializer.Deserialize<Story>(fs, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new Story();
        }

        var sample = CreateSampleStory();
        var json = JsonSerializer.Serialize(sample, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json, new UTF8Encoding(false));
        return sample;
    }

    static Story CreateSampleStory() => new Story
    {
        Title = "Nebel über Bretten",
        Variables = new Dictionary<string, string>
        {
            ["strasse"] = "Melanchthonstraße",
            ["opfer"] = "Friedrich von Hohenberg",
            ["detektiv"] = "Kommissar Keller",
            ["stadt"] = "Bretten"
        },
        Nodes = new()
        {
            new StoryNode(
                "kap1",
                "Kapitel 1 – Fund im Nebel",
                new[]
                {
                    "Der Nebel hing schwer über der {{strasse}}.",
                    "Im Salon: {{opfer}}, tot im Sessel – der Rotwein noch warm."
                },
                new List<Choice>
                {
                    new("Clara sofort befragen", "kap2_clara"),
                    new("Tatort gründlich untersuchen", "kap2_tatort")
                })
        }
    };
}