using System;
using System.IO;
using System.Text;

namespace AA98_AvlnCodeStudio.Model.Documents;

/// <summary>Decodes supported Unicode text strictly and retains its encoding and byte-order mark.</summary>
public static class TextDocumentEncoding
{
    private static readonly UTF8Encoding s_utf8 = new(false, true);
    private static readonly UTF8Encoding s_utf8Bom = new(true, true);
    private static readonly UnicodeEncoding s_utf16Le = new(false, true, true);
    private static readonly UnicodeEncoding s_utf16Be = new(true, true, true);
    private static readonly UTF32Encoding s_utf32Le = new(false, true, true);
    private static readonly UTF32Encoding s_utf32Be = new(true, true, true);

    public static (string Text, Encoding Encoding) Decode(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        ReadOnlySpan<byte> span = bytes;
        Encoding encoding = span.StartsWith(s_utf32Le.Preamble) ? s_utf32Le
            : span.StartsWith(s_utf32Be.Preamble) ? s_utf32Be
            : span.StartsWith(s_utf8Bom.Preamble) ? s_utf8Bom
            : span.StartsWith(s_utf16Le.Preamble) ? s_utf16Le
            : span.StartsWith(s_utf16Be.Preamble) ? s_utf16Be
            : s_utf8;
        try
        {
            string text = encoding.GetString(bytes, encoding.Preamble.Length, bytes.Length - encoding.Preamble.Length);
            ValidateText(text);
            return (text, encoding);
        }
        catch (DecoderFallbackException exception)
        {
            throw new InvalidDataException("The file is not valid supported Unicode text.", exception);
        }
    }

    public static void ValidateText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        foreach (char character in text)
        {
            if (char.IsControl(character) && character is not '\r' and not '\n' and not '\t' and not '\f')
            {
                throw new InvalidDataException("The file contains binary control characters and cannot be edited as text.");
            }
        }
    }

    public static byte[] Encode(string text, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(encoding);
        ValidateText(text);
        try
        {
            byte[] preamble = encoding.GetPreamble();
            byte[] content = encoding.GetBytes(text);
            byte[] result = new byte[preamble.Length + content.Length];
            preamble.CopyTo(result, 0);
            content.CopyTo(result, preamble.Length);
            return result;
        }
        catch (EncoderFallbackException exception)
        {
            throw new InvalidDataException("The document contains invalid Unicode text.", exception);
        }
    }
}
