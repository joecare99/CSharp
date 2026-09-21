using System;

namespace Osb.Core.Formatting;

public sealed class LegacyDateFormattingRequest
{
    public LegacyDateFormattingRequest(
        string rawDate,
        string qualifier,
        LegacyFormattingOptions options,
        byte nonul = 0,
        string qualifierValue = "",
        string contextPrefix = "")
    {
        RawDate = rawDate ?? throw new ArgumentNullException(nameof(rawDate));
        Qualifier = qualifier ?? throw new ArgumentNullException(nameof(qualifier));
        Options = options ?? throw new ArgumentNullException(nameof(options));
        QualifierValue = qualifierValue ?? throw new ArgumentNullException(nameof(qualifierValue));
        ContextPrefix = contextPrefix ?? throw new ArgumentNullException(nameof(contextPrefix));
        Nonul = nonul;
    }

    public string RawDate { get; }
    public string Qualifier { get; }
    public LegacyFormattingOptions Options { get; }
    public byte Nonul { get; }
    public string QualifierValue { get; }
    public string ContextPrefix { get; }
}

public sealed class LegacyDateFormattingResult
{
    public LegacyDateFormattingResult(string text, bool wasSuppressed)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        WasSuppressed = wasSuppressed;
    }

    public string Text { get; }
    public bool WasSuppressed { get; }
}

public static class LegacyDateFormatting
{
    public static LegacyDateFormattingResult Format(LegacyDateFormattingRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (request.Options.SuppressDatesAfterCutoff &&
            string.CompareOrdinal(request.RawDate, request.Options.DateCutoff) > 0)
        {
            return new LegacyDateFormattingResult(string.Empty, true);
        }

        var text = LegacyTextFormatting.FormatLegacyDate(request.RawDate);
        if (request.Nonul == 0 && request.Options.UseReplacementDateMarker)
        {
            text = ApplyReplacementMarker(text);
        }

        text = ApplyQualifier(text, request);
        if (request.Options.QualifyDatesWithAm &&
            text.Split(new[] { '.' }, StringSplitOptions.None).Length == 3 &&
            request.Qualifier.Length == 0 &&
            request.ContextPrefix.Length == 0)
        {
            text = "am " + text;
        }

        return new LegacyDateFormattingResult(text, false);
    }

    private static string ApplyReplacementMarker(string text)
    {
        if (text.Length >= 4 && text.Substring(2, 2) == ".0")
        {
            text = text.Substring(0, 2) + ".R" + text.Substring(4);
        }

        if (text.StartsWith("0", StringComparison.Ordinal))
        {
            text = "R" + text.Substring(1);
        }

        return text.Replace("R", string.Empty);
    }

    private static string ApplyQualifier(string text, LegacyDateFormattingRequest request)
    {
        switch (request.Qualifier.ToUpperInvariant())
        {
            case "U":
                return "um " + text;
            case "V":
                return "vor " + text;
            case "N":
                return "nach " + text;
            case "R":
                return "errech. " + text;
            case "Z":
                return "zwischen " + text;
            case "A":
                return " und " + text;
            case "B":
                return " bis " + text;
            case "C":
                return "calc. " + text;
            case "?":
                return text + request.QualifierValue;
            case "":
                return request.ContextPrefix.Length == 0 ? text : request.ContextPrefix + " " + text;
            default:
                return text;
        }
    }
}
