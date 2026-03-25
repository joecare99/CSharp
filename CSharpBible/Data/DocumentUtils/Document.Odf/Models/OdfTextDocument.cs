using Document.Base.Models.Interfaces;
using Document.Odf.Parsing;
using Document.Odf.Writing;

namespace Document.Odf.Models;

public class OdfTextDocument : IUserDocument
{
    private readonly OdfSection _root = new();
    private bool _isModified;

    public IDocElement Root => _root;

    public bool IsModified => _isModified;

    public static IUserDocument CreateUserDocument() => new OdfTextDocument();

    public IDocHeadline AddHeadline(int nLevel, string? Id = null)
    {
        var id = Id ?? Guid.NewGuid().ToString("N");
        var headline = new OdfHeadline(nLevel, id);
        _root.AddChild(headline);
        _isModified = true;
        return headline;
    }

    public IDocParagraph AddParagraph(string cStylename)
    {
        var paragraph = new OdfParagraph(cStylename);
        _root.AddChild(paragraph);
        _isModified = true;
        return paragraph;
    }

    public IDocTOC AddTOC(string cName, int nLevel)
    {
        var toc = new OdfTOC(cName, nLevel);
        _root.AddChild(toc);
        _isModified = true;
        return toc;
    }

    public IEnumerable<IDocElement> Enumerate() => _root.Enumerate();

    public bool LoadFrom(string cInputPath)
    {
        try
        {
            using var fs = File.OpenRead(cInputPath);
            return LoadFrom(fs);
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
            // Stream in temporäre Datei kopieren, da ZipArchive seekable Stream benötigt
            using var ms = new MemoryStream();
            sInputStream.CopyTo(ms);
            ms.Position = 0;

            using var zip = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Read);
            var contentEntry = zip.GetEntry("content.xml");
            if (contentEntry == null)
                return false;

            using var contentStream = contentEntry.Open();
            var contentXml = System.Xml.Linq.XDocument.Load(contentStream, 
                System.Xml.Linq.LoadOptions.PreserveWhitespace | System.Xml.Linq.LoadOptions.SetLineInfo);

            // Root-Nodes löschen und neu befüllen
            _root.Nodes.Clear();
            OdfBlockParser.Populate(this, contentXml);
            _isModified = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

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
            using var zip = new System.IO.Compression.ZipArchive(sOutputStream, 
                System.IO.Compression.ZipArchiveMode.Create, leaveOpen: true);

            // mimetype (muss erste Entry sein, unkomprimiert)
            var mimetypeEntry = zip.CreateEntry("mimetype", System.IO.Compression.CompressionLevel.NoCompression);
            using (var s = mimetypeEntry.Open())
            using (var w = new StreamWriter(s, new System.Text.UTF8Encoding(false)))
            {
                w.Write("application/vnd.oasis.opendocument.text");
            }

            // content.xml
            var contentXml = OdfXmlWriter.CreateContentXml(_root);
            var contentEntry = zip.CreateEntry("content.xml", System.IO.Compression.CompressionLevel.Optimal);
            using (var s = contentEntry.Open())
            {
                contentXml.Save(s);
            }

            // styles.xml
            var stylesXml = OdfXmlWriter.CreateStylesXml();
            var stylesEntry = zip.CreateEntry("styles.xml", System.IO.Compression.CompressionLevel.Optimal);
            using (var s = stylesEntry.Open())
            {
                stylesXml.Save(s);
            }

            // meta.xml
            var metaXml = OdfXmlWriter.CreateMetaXml();
            var metaEntry = zip.CreateEntry("meta.xml", System.IO.Compression.CompressionLevel.Optimal);
            using (var s = metaEntry.Open())
            {
                metaXml.Save(s);
            }

            // META-INF/manifest.xml
            WriteManifest(zip);

            _isModified = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void WriteManifest(System.IO.Compression.ZipArchive zip)
    {
        var manifestEntry = zip.CreateEntry("META-INF/manifest.xml", System.IO.Compression.CompressionLevel.Optimal);
        using var s = manifestEntry.Open();

        var manifest = new System.Xml.Linq.XDocument(
            new System.Xml.Linq.XDeclaration("1.0", "UTF-8", null),
            new System.Xml.Linq.XElement(OdfNamespaces.Manifest + "manifest",
                new System.Xml.Linq.XAttribute(System.Xml.Linq.XNamespace.Xmlns + "manifest", OdfNamespaces.Manifest),
                new System.Xml.Linq.XElement(OdfNamespaces.Manifest + "file-entry",
                    new System.Xml.Linq.XAttribute(OdfNamespaces.Manifest + "full-path", "/"),
                    new System.Xml.Linq.XAttribute(OdfNamespaces.Manifest + "media-type", "application/vnd.oasis.opendocument.text")),
                new System.Xml.Linq.XElement(OdfNamespaces.Manifest + "file-entry",
                    new System.Xml.Linq.XAttribute(OdfNamespaces.Manifest + "full-path", "content.xml"),
                    new System.Xml.Linq.XAttribute(OdfNamespaces.Manifest + "media-type", "text/xml")),
                new System.Xml.Linq.XElement(OdfNamespaces.Manifest + "file-entry",
                    new System.Xml.Linq.XAttribute(OdfNamespaces.Manifest + "full-path", "styles.xml"),
                    new System.Xml.Linq.XAttribute(OdfNamespaces.Manifest + "media-type", "text/xml")),
                new System.Xml.Linq.XElement(OdfNamespaces.Manifest + "file-entry",
                    new System.Xml.Linq.XAttribute(OdfNamespaces.Manifest + "full-path", "meta.xml"),
                    new System.Xml.Linq.XAttribute(OdfNamespaces.Manifest + "media-type", "text/xml"))
            )
        );
        manifest.Save(s);
    }
}
