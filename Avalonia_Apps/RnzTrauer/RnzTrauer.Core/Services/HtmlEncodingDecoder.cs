using System;
using System.Text;

namespace RnzTrauer.Core.Services;

/// <summary>
/// Explicit encoding boundary for the legacy <c>GuessEncoding</c> workflow.
/// UTF-8 is preferred when valid; Windows-1252 is the conservative portal fallback.
/// </summary>
public sealed class HtmlEncodingDecoder : IHtmlEncodingDecoder
{
    private static readonly Encoding StrictUtf8 = new UTF8Encoding(false, true);
    private static readonly Encoding Cp1252 = CreateCp1252();

    /// <inheritdoc />
    public DecodedHtml Decode(ReadOnlyMemory<byte> bytes)
    {
        var data = bytes.Span;
        if (StartsWith(data, 0xEF, 0xBB, 0xBF))
            return new DecodedHtml(Encoding.UTF8.GetString(data[3..]), "UTF-8 BOM");
        if (StartsWith(data, 0xFF, 0xFE))
            return new DecodedHtml(Encoding.Unicode.GetString(data[2..]), "UTF-16 LE BOM");
        if (StartsWith(data, 0xFE, 0xFF))
            return new DecodedHtml(Encoding.BigEndianUnicode.GetString(data[2..]), "UTF-16 BE BOM");

        try
        {
            return new DecodedHtml(StrictUtf8.GetString(data), "UTF-8");
        }
        catch (DecoderFallbackException)
        {
            return new DecodedHtml(Cp1252.GetString(data), "Windows-1252 fallback");
        }
    }

    private static bool StartsWith(ReadOnlySpan<byte> data, params byte[] prefix)
    {
        if (data.Length < prefix.Length)
            return false;
        for (var index = 0; index < prefix.Length; index++)
            if (data[index] != prefix[index])
                return false;
        return true;
    }

    private static Encoding CreateCp1252()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        return Encoding.GetEncoding(1252);
    }
}
