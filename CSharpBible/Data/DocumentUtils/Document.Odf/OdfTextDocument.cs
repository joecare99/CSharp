using Document.Base.Models.Interfaces;

namespace Document.Odf;

public sealed class OdfTextDocument : IUserDocument
{
    private bool _isModified;

    public OdfTextDocument()
    {
        Root = new Document.Odf.Models.OdfSection();
    }

    public OdfTextDocument(IDocElement root)
    {
        Root = root is Document.Odf.Models.OdfSection
            ? root
            : throw new ArgumentException("Root muss vom Typ OdfSection sein.", nameof(root));
    }

    public IDocElement Root { get; private set; }

    public bool IsModified => _isModified;

    public IDocParagraph AddParagraph(string cStylename)
    {
        var section = EnsureRoot();
        _isModified = true;
        return section.AddParagraph(cStylename);
    }

    public IDocHeadline AddHeadline(int nLevel, string? Id = null)
    {
        var section = EnsureRoot();
        _isModified = true;
        return section.AddHeadline(nLevel, Id ?? Guid.NewGuid().ToString("N"));
    }

    public IDocTOC AddTOC(string cName, int nLevel)
    {
        var section = EnsureRoot();
        _isModified = true;
        return section.AddTOC(cName, nLevel);
    }

    public IEnumerable<IDocElement> Enumerate()
        => Root.Enumerate();

    public bool SaveTo(string cOutputPath)
    {
        try
        {
            using var fs = new FileStream(cOutputPath, FileMode.Create, FileAccess.Write);
            return SaveTo(fs);
        }
        catch
        {
            return false;
        }
    }

    public bool SaveTo(Stream sOutputStream, object? options = null)
    {
        try
        {
            using var zip = new System.IO.Compression.ZipArchive(sOutputStream, System.IO.Compression.ZipArchiveMode.Create, leaveOpen: true);

            // mimetype (uncompressed, first entry)
            var mimeEntry = zip.CreateEntry("mimetype", System.IO.Compression.CompressionLevel.NoCompression);
            using (var writer = new StreamWriter(mimeEntry.Open()))
                writer.Write("application/vnd.oasis.opendocument.text");

            // content.xml
            var contentEntry = zip.CreateEntry("content.xml");
            using (var stream = contentEntry.Open())
            {
                var xdoc = CreateContentXDocument();
                xdoc.Save(stream);
            }

            // META-INF/manifest.xml
            var manifestEntry = zip.CreateEntry("META-INF/manifest.xml");
            using (var stream = manifestEntry.Open())
            {
                var manifest = CreateManifest();
                manifest.Save(stream);
            }

            _isModified = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool LoadFrom(string cInputPath)
    {
        try
        {
            using var odf = OdfDocument.Open(cInputPath);
            var xdoc = odf.ReadContentXml();
            Document.Odf.Parsing.OdfBlockParser.Populate(this, xdoc);
            _isModified = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool LoadFrom(Stream sInputStream, object? options = null)
    {
        try
        {
            using var zip = new System.IO.Compression.ZipArchive(sInputStream, System.IO.Compression.ZipArchiveMode.Read, leaveOpen: true);
            var entry = zip.GetEntry("content.xml") ?? throw new FileNotFoundException("content.xml not found");
            using var stream = entry.Open();
            var xdoc = System.Xml.Linq.XDocument.Load(stream);
            Document.Odf.Parsing.OdfBlockParser.Populate(this, xdoc);
            _isModified = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private Models.OdfSection EnsureRoot()
        => Root as Models.OdfSection ?? throw new InvalidOperationException("Root ist nicht OdfSection");

    private static System.Xml.Linq.XDocument CreateManifest()
    {
        System.Xml.Linq.XNamespace manifestNs = "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0";
        return new System.Xml.Linq.XDocument(
            new System.Xml.Linq.XDeclaration("1.0", "UTF-8", null),
            new System.Xml.Linq.XElement(manifestNs + "manifest",
                new System.Xml.Linq.XAttribute(System.Xml.Linq.XNamespace.Xmlns + "manifest", manifestNs),
                new System.Xml.Linq.XElement(manifestNs + "file-entry",
                    new System.Xml.Linq.XAttribute(manifestNs + "full-path", "/"),
                    new System.Xml.Linq.XAttribute(manifestNs + "media-type", "application/vnd.oasis.opendocument.text")),
                new System.Xml.Linq.XElement(manifestNs + "file-entry",
                    new System.Xml.Linq.XAttribute(manifestNs + "full-path", "content.xml"),
                    new System.Xml.Linq.XAttribute(manifestNs + "media-type", "text/xml"))
            )
        );
    }

    private System.Xml.Linq.XDocument CreateContentXDocument()
    {
        // TODO: Anpassung an tatsächliches ODF-Modell der Anwendung
        var officeNs = System.Xml.Linq.XNamespace.Get("urn:oasis:names:tc:opendocument:xmlns:office:1.0");
        var textNs = System.Xml.Linq.XNamespace.Get("urn:oasis:names:tc:opendocument:xmlns:text:1.0");

        var rootSection = EnsureRoot();

        var bodyElement = new System.Xml.Linq.XElement(officeNs + "body");
        var textElement = new System.Xml.Linq.XElement(officeNs + "text");
        bodyElement.Add(textElement);

        foreach (var element in rootSection.Enumerate())
        {
            if (element is IDocHeadline headline)
            {
                var h = new System.Xml.Linq.XElement(textNs + "h",
                    new System.Xml.Linq.XAttribute(textNs + "outline-level", headline.Level));

                if (!string.IsNullOrEmpty(headline.Id))
                    h.Add(new System.Xml.Linq.XAttribute("xml:id", headline.Id));

                // headline.Text gibt es nicht, stattdessen TextContent verwenden
                if (!string.IsNullOrEmpty(headline.TextContent))
                    h.Add(new System.Xml.Linq.XText(headline.TextContent));

                textElement.Add(h);
            }
            else if (element is IDocParagraph paragraph)
            {
                var p = new System.Xml.Linq.XElement(textNs + "p");

                // StyleName gibt es nicht im Interface, daher entfernt
                // if (!string.IsNullOrEmpty(paragraph.StyleName))
                //     p.Add(new System.Xml.Linq.XAttribute(textNs + "style-name", paragraph.StyleName));

                if (!string.IsNullOrEmpty(paragraph.TextContent))
                    p.Add(new System.Xml.Linq.XText(paragraph.TextContent));

                textElement.Add(p);
            }
            // Weitere Elementtypen hier bei Bedarf abbilden
        }

        var doc = new System.Xml.Linq.XDocument(
            new System.Xml.Linq.XDeclaration("1.0", "UTF-8", null),
            new System.Xml.Linq.XElement(officeNs + "document-content",
                new System.Xml.Linq.XAttribute(System.Xml.Linq.XNamespace.Xmlns + "office", officeNs),
                new System.Xml.Linq.XAttribute(System.Xml.Linq.XNamespace.Xmlns + "text", textNs),
                bodyElement)
        );

        return doc;
    }
}
