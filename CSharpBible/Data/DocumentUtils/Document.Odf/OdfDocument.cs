using System.IO.Compression;
using System.Xml.Linq;

namespace Document.Odf;

public sealed class OdfDocument : IAsyncDisposable, IDisposable
{
    private readonly ZipArchive _zip; private bool _disposed;
    private OdfDocument(ZipArchive zip)
    {
        _zip = zip;
    }

    public static OdfDocument Open(string path)
    {
        var fs = File.OpenRead(path);
        var zip = new ZipArchive(fs, ZipArchiveMode.Read, leaveOpen: false);
        return new OdfDocument(zip);
    }

    public static async Task<OdfDocument> OpenAsync(string path, CancellationToken ct = default)
    {
        // ZipArchive hat kein echtes Async-Open; Datei async öffnen für IO.
        var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 64 * 1024, useAsync: true);
        // trotzdem synchrones Zip-Parsing
        var zip = new ZipArchive(fs, ZipArchiveMode.Read, leaveOpen: false);
        await Task.CompletedTask;
        return new OdfDocument(zip);
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

    public Stream? OpenBinary(string pathInPackage)
    {
        var entry = _zip.GetEntry(pathInPackage);
        return entry?.Open();
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