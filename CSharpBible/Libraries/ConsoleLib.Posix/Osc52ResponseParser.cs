using System;
using System.Text;

namespace ConsoleLib.Posix;

/// <summary>Decodes one OSC-52 clipboard response with a configurable payload limit.</summary>
public sealed class Osc52ResponseParser
{
    private readonly int _maximumPayloadLength;

    public Osc52ResponseParser(int maximumPayloadLength = 4096)
    {
        if (maximumPayloadLength <= 0)
            throw new ArgumentOutOfRangeException(nameof(maximumPayloadLength));
        _maximumPayloadLength = maximumPayloadLength;
    }

    public string? Parse(string response)
    {
        if (response is null)
            throw new ArgumentNullException(nameof(response));
        const string prefix = "\u001b]52;";
        var start = response.IndexOf(prefix, StringComparison.Ordinal);
        if (start < 0)
            return null;
        var separator = response.IndexOf(';', start + prefix.Length);
        var terminator = response.IndexOf('\a', separator + 1);
        if (separator < 0 || terminator < 0)
            return null;
        var encoded = response.Substring(separator + 1, terminator - separator - 1);
        if (encoded.Length > _maximumPayloadLength * 2)
            return null;
        try
        {
            var bytes = Convert.FromBase64String(encoded);
            if (bytes.Length > _maximumPayloadLength)
                return null;
            return Encoding.UTF8.GetString(bytes);
        }
        catch (FormatException)
        {
            return null;
        }
    }
}
