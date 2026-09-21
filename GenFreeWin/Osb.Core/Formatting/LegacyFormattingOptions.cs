using System;
using System.Globalization;

namespace Osb.Core.Formatting;

public sealed class LegacyFormattingOptions
{
    public LegacyFormattingOptions(
        bool qualifyDatesWithAm,
        bool suppressDatesAfterCutoff,
        string dateCutoff,
        bool useReplacementDateMarker,
        bool preserveLineBreaks,
        bool includeSourceDate)
    {
        QualifyDatesWithAm = qualifyDatesWithAm;
        SuppressDatesAfterCutoff = suppressDatesAfterCutoff;
        DateCutoff = dateCutoff ?? throw new ArgumentNullException(nameof(dateCutoff));
        UseReplacementDateMarker = useReplacementDateMarker;
        PreserveLineBreaks = preserveLineBreaks;
        IncludeSourceDate = includeSourceDate;
    }

    public bool QualifyDatesWithAm { get; }

    public bool SuppressDatesAfterCutoff { get; }

    public string DateCutoff { get; }

    public bool UseReplacementDateMarker { get; }

    public bool PreserveLineBreaks { get; }

    public bool IncludeSourceDate { get; }

    public static LegacyFormattingOptions FromLegacySlots(string[] aus, string[] oAus)
    {
        if (aus == null)
        {
            throw new ArgumentNullException(nameof(aus));
        }

        if (oAus == null)
        {
            throw new ArgumentNullException(nameof(oAus));
        }

        return new LegacyFormattingOptions(
            IsEnabled(aus, 46),
            IsEnabled(aus, 108),
            GetSlot(aus, 173),
            IsEnabled(aus, 176),
            IsEnabled(aus, 185),
            IsEnabled(oAus, 5));
    }

    private static bool IsEnabled(string[] slots, int index)
    {
        return decimal.TryParse(GetSlot(slots, index), NumberStyles.Float, CultureInfo.InvariantCulture, out var value) &&
            value == 1m;
    }

    private static string GetSlot(string[] slots, int index)
    {
        return index >= 0 && index < slots.Length && slots[index] != null ? slots[index] : string.Empty;
    }
}
