using System.IO.Compression;
using System.Text;
using System.Xml.Linq;
using Document.Odf.Models;

namespace Document.Odf;

public sealed class OdfDocument : IAsyncDisposable, IDisposable
{
    private readonly ZipArchive _zip;
    private readonly bool _isReadOnly;
    private bool _disposed;

    private OdfDocument(ZipArchive zip, bool isReadOnly)
    {
        _zip = zip;
        _isReadOnly = isReadOnly;
    }

    public static OdfDocument Open(string path)
    {
        var fs = File.OpenRead(path);
        var zip = new ZipArchive(fs, ZipArchiveMode.Read, leaveOpen: false);
        return new OdfDocument(zip, isReadOnly: true);
    }

    public static OdfDocument OpenForUpdate(string path)
    {
        var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        var zip = new ZipArchive(fs, ZipArchiveMode.Update, leaveOpen: false);
        return new OdfDocument(zip, isReadOnly: false);
    }

    public static OdfDocument Create(string path)
    {
        var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
        var zip = new ZipArchive(fs, ZipArchiveMode.Create, leaveOpen: false);
        var doc = new OdfDocument(zip, isReadOnly: false);
        doc.WriteMinimalStructure();
        return doc;
    }

    public static OdfDocument Create(Stream stream, bool leaveOpen = false)
    {
        var zip = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen);
        var doc = new OdfDocument(zip, isReadOnly: false);
        doc.WriteMinimalStructure();
        return doc;
    }

    public static async Task<OdfDocument> OpenAsync(string path, CancellationToken ct = default)
    {
        var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 64 * 1024, useAsync: true);
        var zip = new ZipArchive(fs, ZipArchiveMode.Read, leaveOpen: false);
        await Task.CompletedTask;
        return new OdfDocument(zip, isReadOnly: true);
    }

    public XDocument ReadContentXml()
    {
        var entry = _zip.GetEntry("content.xml") ?? throw new FileNotFoundException("content.xml nicht gefunden");
        using var s = entry.Open();
        return XDocument.Load(s, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
    }

    public XDocument? ReadMetaXmlOrNull()
    {
        var entry = _zip.GetEntry("meta.xml");
        if (entry is null) return null;
        using var s = entry.Open();
        return XDocument.Load(s, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
    }

    public XDocument? ReadStylesXmlOrNull()
    {
        var entry = _zip.GetEntry("styles.xml");
        if (entry is null) return null;
        using var s = entry.Open();
        return XDocument.Load(s, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
    }

    public Stream? OpenBinary(string pathInPackage)
    {
        var entry = _zip.GetEntry(pathInPackage);
        return entry?.Open();
    }

    public void WriteContentXml(XDocument contentXml)
    {
        if (_isReadOnly) throw new InvalidOperationException("Dokument ist schreibgeschützt");

        var entry = _zip.GetEntry("content.xml");
        entry?.Delete();

        entry = _zip.CreateEntry("content.xml", CompressionLevel.Optimal);
        using var s = entry.Open();
        contentXml.Save(s);
    }

    public void WriteMetaXml(XDocument metaXml)
    {
        if (_isReadOnly) throw new InvalidOperationException("Dokument ist schreibgeschützt");

        var entry = _zip.GetEntry("meta.xml");
        entry?.Delete();

        entry = _zip.CreateEntry("meta.xml", CompressionLevel.Optimal);
        using var s = entry.Open();
        metaXml.Save(s);
    }

    public void WriteStylesXml(XDocument stylesXml)
    {
        if (_isReadOnly) throw new InvalidOperationException("Dokument ist schreibgeschützt");

        var entry = _zip.GetEntry("styles.xml");
        entry?.Delete();

        entry = _zip.CreateEntry("styles.xml", CompressionLevel.Optimal);
        using var s = entry.Open();
        stylesXml.Save(s);
    }

    public void WriteBinary(string pathInPackage, Stream data)
    {
        if (_isReadOnly) throw new InvalidOperationException("Dokument ist schreibgeschützt");

        var entry = _zip.GetEntry(pathInPackage);
        entry?.Delete();

        entry = _zip.CreateEntry(pathInPackage, CompressionLevel.Optimal);
        using var s = entry.Open();
        data.CopyTo(s);
    }

    private void WriteMinimalStructure()
    {
        // mimetype (uncompressed, must be first)
        var mimetypeEntry = _zip.CreateEntry("mimetype", CompressionLevel.NoCompression);
        using (var s = mimetypeEntry.Open())
        using (var w = new StreamWriter(s, new UTF8Encoding(false)))
        {
            w.Write("application/vnd.oasis.opendocument.text");
        }

        // META-INF/manifest.xml
        WriteManifest();
    }

    private void WriteManifest()
    {
        var manifestEntry = _zip.CreateEntry("META-INF/manifest.xml", CompressionLevel.Optimal);
        using var s = manifestEntry.Open();

        var manifest = new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            new XElement(OdfNamespaces.Manifest + "manifest",
                new XAttribute(XNamespace.Xmlns + "manifest", OdfNamespaces.Manifest),
                new XElement(OdfNamespaces.Manifest + "file-entry",
                    new XAttribute(OdfNamespaces.Manifest + "full-path", "/"),
                    new XAttribute(OdfNamespaces.Manifest + "media-type", "application/vnd.oasis.opendocument.text")),
                new XElement(OdfNamespaces.Manifest + "file-entry",
                    new XAttribute(OdfNamespaces.Manifest + "full-path", "content.xml"),
                    new XAttribute(OdfNamespaces.Manifest + "media-type", "text/xml")),
                new XElement(OdfNamespaces.Manifest + "file-entry",
                    new XAttribute(OdfNamespaces.Manifest + "full-path", "styles.xml"),
                    new XAttribute(OdfNamespaces.Manifest + "media-type", "text/xml")),
                new XElement(OdfNamespaces.Manifest + "file-entry",
                    new XAttribute(OdfNamespaces.Manifest + "full-path", "meta.xml"),
                    new XAttribute(OdfNamespaces.Manifest + "media-type", "text/xml"))
            )
        );
        manifest.Save(s);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _zip.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}