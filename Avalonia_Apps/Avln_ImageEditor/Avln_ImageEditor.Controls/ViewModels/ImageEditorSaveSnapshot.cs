using System;

namespace Avln_ImageEditor.Controls.ViewModels;

/// <summary>A PNG encoding associated with one editor document and history state.</summary>
public sealed class ImageEditorSaveSnapshot
{
    private readonly byte[] _bytes;
    internal ImageEditorSaveSnapshot(byte[] bytes, object document, object revision)
    {
        _bytes = bytes;
        Document = document;
        Revision = revision;
    }

    /// <summary>Returns a copy of the encoded PNG bytes.</summary>
    public byte[] Bytes => (byte[])_bytes.Clone();
    internal object Document { get; }
    internal object Revision { get; }
}
