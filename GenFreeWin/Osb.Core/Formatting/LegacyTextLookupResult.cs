using System;

namespace Osb.Core.Formatting;

public sealed class LegacyTextLookupResult
{
    private LegacyTextLookupResult(string text, string? leadName)
    {
        Text = text;
        LeadName = leadName;
    }

    public string Text { get; }

    public string? LeadName { get; }

    public static LegacyTextLookupResult FromRawValues(string? text, string? leadName)
    {
        var normalizedText = LegacyTextFormatting.DecodeLegacyText((text ?? string.Empty).Trim());
        var normalizedLeadName = leadName == null
            ? null
            : LegacyTextFormatting.DecodeLegacyText(leadName.Trim());

        return new LegacyTextLookupResult(normalizedText, normalizedLeadName);
    }
}
