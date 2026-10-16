using System;
using System.Collections.Generic;
using Document.Base.Models.Interfaces;

namespace Document.Docx.Model;

/// <summary>
/// Represents encoded image content in a DOCX document tree.
/// </summary>
public sealed class DocxImage : DocxNodeBase, IDocImage
{
    public DocxImage(byte[] imageData, string contentType, string? altText)
    {
        ImageData = imageData;
        ContentType = contentType;
        AltText = altText;
    }

    public byte[] ImageData { get; }

    public string ContentType { get; }

    public string? AltText { get; }

    public IDocElement AppendDocElement(Enum type) => throw new NotSupportedException();

    public IDocElement AppendDocElement(Enum type, Type elementType) => throw new NotSupportedException();

    public IDocElement AppendDocElement(Enum type, Enum attribute, string value, Type elementType, string? id) =>
        throw new NotSupportedException();

    public IEnumerable<IDocElement> Enumerate()
    {
        yield return this;
    }
}
