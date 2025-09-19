using System.Text;
using System.Text.Json;
using Document.Base.Factories;
using Document.Base.Models.Interfaces;
using Document.Html;

namespace NebelEbook;

class Program
{
    static int Main(string[] args)
    {
        var t= typeof(HtmlDocument);


        try
        {
            var key = (args.Length > 0 ? args[0] : "html").Trim().ToLowerInvariant();
            var storyPath = Path.Combine(AppContext.BaseDirectory, "story.json");

            // Ausgabedatei bestimmen
            var output = args.Length > 1
                ? args[1]
                : Path.Combine(
                    AppContext.BaseDirectory,
                    key switch
                    {
                        "html" => "Nebel_ueber_Bretten_Interaktiv.html",
                        "pdf" => "Nebel_ueber_Bretten_Interaktiv.pdf",
                        _ when key.EndsWith(".html", StringComparison.OrdinalIgnoreCase) => Path.GetFileName(key),
                        _ when key.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) => Path.GetFileName(key),
                        _ => $"Nebel_ueber_Bretten_Interaktiv.{(key.Contains('/') ? "out" : key)}"
                    });

            // Dokument über Factory erstellen (per Key 'pdf'/'html' oder per Dateiendung)
            IUserDocument doc = UserDocumentFactory.Create(key);

            var story = LoadOrCreateSampleStory(storyPath);
            BuildDocument(doc, story);

            if (!doc.SaveTo(output))
            {
                Console.Error.WriteLine($"Speichern fehlgeschlagen: {output}");
                return 2;
            }

            Console.WriteLine($"Erstellt: {Path.GetFullPath(output)}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static IUserDocument CreateUserDocument(string key, string output)
    {
        // Versuch nach Key/MIME/Extension/Pfad
        if (UserDocumentFactory.TryCreate(key, out var byKey) && byKey is not null)
            return byKey;

        var byExt = UserDocumentFactory.CreateForPath(output)
                   ?? UserDocumentFactory.CreateForExtension(Path.GetExtension(output));
        if (byExt is not null)
            return byExt;

        // Fallback: pdf
        return UserDocumentFactory.Create("html");
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
                var link = p.AddLink(font);
                link.TextContent = TextTemplate.Render(n.Title, story.Variables);
                // Formatneutrale Link-Zielangabe
                link.SetStyle(doc, font); // optional, falls Engines Styles nutzen
                // über IDOMElement.Attributes:
                // href für HTML; PDF-Engine kann diese Information interpretieren
                (link as IDOMElement).Attributes["href"] = $"#{n.Id}";
            }
        }

        // Kapitel
        foreach (var node in story.Nodes)
        {
            var h = doc.AddHeadline(1);
            h.TextContent = TextTemplate.Render(node.Title, story.Variables);

            // Anker-Span mit id = node.Id
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
                    var l = cp.AddLink(font);
                    l.TextContent = "→ " + TextTemplate.Render(c.Label, story.Variables);
                    (l as IDOMElement).Attributes["href"] = $"#{c.TargetId}";
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

    // Minimaler Fallback – vollständige story.json wird bereits mitkopiert.
    static Story CreateSampleStory() => new Story
    {
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