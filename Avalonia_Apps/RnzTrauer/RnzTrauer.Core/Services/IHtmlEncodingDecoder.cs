using System;

namespace RnzTrauer.Core.Services;

/// <summary>Converts legacy portal bytes into the UTF-16 strings used by Core.</summary>
public interface IHtmlEncodingDecoder
{
    /// <summary>Decodes BOM-marked, valid UTF-8, or CP1252 fallback input.</summary>
    DecodedHtml Decode(ReadOnlyMemory<byte> bytes);
}
