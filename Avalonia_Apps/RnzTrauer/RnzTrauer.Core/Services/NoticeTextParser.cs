using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using RnzTrauer.Core.Domain;

namespace RnzTrauer.Core.Services;

/// <summary>
/// Deliberately conservative port of <c>TAnzTextParser</c>: normalize OCR text,
/// recognize German date/death/burial markers, and return facts for user review.
/// </summary>
public sealed class NoticeTextParser : INoticeTextParser
{
    private static readonly string[] BirthMarkers = ["*", "geb. am", "geb.", "geboren am", "geboren"];
    private static readonly string[] DeathMarkers = ["†", "✝", "verst. am", "gest. am", "gest.", "verstarb am", "verstorben am", "gestorben am", "+"];
    private static readonly string[] BurialMarkers = ["Bestattungstermin", "Trauerfeier", "Requiem", "Begräbnis", "Beerdigung", "Beisetzung", "Urnenbeisetzung"];
    private static readonly Regex DatePattern = new(@"\b(?:[0-3]?\d\s*[.]\s*)?(?:[01]?\d|Jan(?:uar)?|Feb(?:ruar)?|März?|Marz|Apr(?:il)?|Mai|Jun(?:i)?|Jul(?:i)?|Aug(?:ust)?|Sep(?:t(?:ember)?)?|Okt(?:ober)?|Nov(?:ember)?|Dez(?:ember)?)\s*[.]?\s*\d{4}\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public ParsedNoticeFacts Parse(DeathNotice notice, string text, IReadOnlyCollection<string> placeNames)
    {
        ArgumentNullException.ThrowIfNull(notice);
        text ??= string.Empty;
        var normalized = Regex.Replace(text.Replace("\t", " "), @"-\r?\n", string.Empty);
        normalized = Regex.Replace(normalized, @"\s+", " ").Trim();
        var birth = FindDateAfter(normalized, BirthMarkers);
        var death = FindDateAfter(normalized, DeathMarkers);
        death ??= FindDateBeforeMarker(normalized, "verstorben");
        death ??= FindDateBeforeMarker(normalized, "verstarb");
        death ??= FindDateBeforeMarker(normalized, "geschlossen");
        if (birth is null || death is null)
        {
            var unmarkedDates = DatePattern.Matches(normalized).Select(match => ParseDate(match.Value)).Where(date => date.HasValue).ToArray();
            if (unmarkedDates.Length >= 2)
            {
                birth ??= unmarkedDates[0];
                death ??= unmarkedDates[1];
            }
        }
        var burial = FindDateAfter(normalized, BurialMarkers);
        var maiden = FindMaidenName(normalized, notice.FamilyName);
        var place = placeNames.OrderByDescending(static p => p.Length).FirstOrDefault(p => ContainsWord(normalized, p));
        var age = FindAge(normalized);
        AdvertisementCategory? category = notice.Category == AdvertisementCategory.DeathNotice && burial is null && normalized.Contains("in aller Stille", StringComparison.OrdinalIgnoreCase)
            ? AdvertisementCategory.DeathNoticeWithoutBurial : null;
        return new ParsedNoticeFacts(birth, death, burial, maiden, place, age, category);
    }

    private static DateTime? FindDateAfter(string text, IEnumerable<string> markers)
    {
        var start = markers.Select(marker => text.IndexOf(marker, StringComparison.OrdinalIgnoreCase)).Where(index => index >= 0).DefaultIfEmpty(-1).Min();
        return start < 0 ? null : ParseDate(DatePattern.Match(text, start).Value);
    }

    private static DateTime? FindDateBeforeMarker(string text, string marker)
    {
        var markerIndex = text.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (markerIndex < 0)
            return null;
        var start = Math.Max(0, markerIndex - 100);
        return ParseDate(DatePattern.Match(text[start..markerIndex]).Value);
    }

    private static DateTime? ParseDate(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = value
            .Replace("Januar", "01").Replace("Februar", "02").Replace("März", "03")
            .Replace("Marz", "03").Replace("April", "04").Replace("Juni", "06")
            .Replace("Juli", "07").Replace("August", "08").Replace("September", "09")
            .Replace("Oktober", "10").Replace("November", "11").Replace("Dezember", "12")
            .Replace("Mär", "03").Replace("Jan", "01").Replace("Feb", "02")
            .Replace("Apr", "04").Replace("Jun", "06").Replace("Jul", "07")
            .Replace("Aug", "08").Replace("Sep", "09").Replace("Okt", "10")
            .Replace("Nov", "11").Replace("Dez", "12");
        return DateTime.TryParse(normalized, CultureInfo.GetCultureInfo("de-DE"), DateTimeStyles.AllowWhiteSpaces, out var date) ? date : null;
    }

    private static string? FindMaidenName(string text, string? familyName)
    {
        var match = Regex.Match(text, @"\b(?:geborene?|geb\.)\s+((?:(?:von|van|de|der)\s+){1,2}[A-ZÄÖÜ][\p{L}-]*|[A-ZÄÖÜ][\p{L}-]*)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!match.Success || string.Equals(match.Groups[1].Value, familyName, StringComparison.OrdinalIgnoreCase))
            return null;
        var titleCased = CultureInfo.GetCultureInfo("de-DE").TextInfo.ToTitleCase(match.Groups[1].Value.ToLower(CultureInfo.GetCultureInfo("de-DE")));
        return Regex.Replace(titleCased, @"\b(Von|Van|De|Der)\b", static particle => particle.Value.ToLowerInvariant());
    }

    private static int? FindAge(string text)
    {
        var match = Regex.Match(text, @"(?:im Alter von|mit|seinem|ihrem)\s+(\d{1,3})\s*[.]?\s+(?:Jahren|Lebensjahr)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        return match.Success && int.TryParse(match.Groups[1].Value, out var age) ? age : null;
    }

    private static bool ContainsWord(string text, string candidate)
    {
        if (candidate.Length <= 1)
            return false;
        if (text.Contains(candidate, StringComparison.OrdinalIgnoreCase))
            return true;

        var compactText = CompactPlaceText(text);
        var compactCandidate = CompactPlaceText(candidate);
        return compactCandidate.Length > 1 &&
               compactText.Contains(compactCandidate, StringComparison.OrdinalIgnoreCase);
    }

    private static string CompactPlaceText(string value) =>
        string.Concat(value.Where(static character => !char.IsWhiteSpace(character)));
}
