using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using UglyToad.PdfPig;

namespace RnzTrauer.Core;

/// <summary>
/// Contains helper methods ported from the original Python implementation.
/// </summary>
public static class PortedHelpers
{
    /// <summary>
    /// Gets the shared JSON serializer options used by the ported tools.
    /// </summary>
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Converts a RNZ date string in <c>dd.MM.yyyy</c> format into a <see cref="DateOnly"/> value.
    /// </summary>
    public static DateOnly? Str2Date(string? sValue)
    {
        if (string.IsNullOrWhiteSpace(sValue))
        {
            return null;
        }

        var arrParts = sValue.Split('.');
        if (arrParts.Length != 3)
        {
            return null;
        }

        if (int.TryParse(arrParts[2], out var iYear) &&
            int.TryParse(arrParts[1], out var iMonth) &&
            int.TryParse(arrParts[0], out var iDay))
        {
            try
            {
                return new DateOnly(iYear, iMonth, iDay);
            }
            catch
            {
                return null;
            }
        }

        return null;
    }

    /// <summary>
    /// Returns a trimmed string value from a loosely typed dictionary.
    /// </summary>
    public static string Cond(this IReadOnlyDictionary<string, object?> dValues, string sKey) => dValues.TryGetValue(sKey, out var xValue)
            ? Convert.ToString(xValue, CultureInfo.InvariantCulture)?.Trim(' ') ?? string.Empty
            : string.Empty;

    /// <summary>
    /// Crops the input string at the first occurrence of the specified separator.
    /// </summary>
    public static string LCropStr(this string sOriginal, string sSeparator)
    {
        var iFound = sOriginal.IndexOf(sSeparator, StringComparison.Ordinal);
        return iFound >= 0 ? sOriginal[..iFound] : sOriginal;
    }

    /// <summary>
    /// Splits a full name into last name and first name using the original Python rules.
    /// </summary>
    public static (string LastName, string FirstName) SplitName(this string sName)
    {
        var arrNames = sName.Trim(' ').Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();
        if (arrNames.Length == 0)
        {
            return (string.Empty, string.Empty);
        }

        for (var iIndex = arrNames.Length - 1; iIndex >= 0; iIndex--)
        {
            if (arrNames[iIndex].Equals("von", StringComparison.OrdinalIgnoreCase) || arrNames[iIndex].Equals("van", StringComparison.OrdinalIgnoreCase))
            {
                arrNames[iIndex] = arrNames[iIndex].ToLowerInvariant();
                if (iIndex < arrNames.Length - 1)
                {
                    arrNames[iIndex + 1] = $"{arrNames[iIndex]} {arrNames[iIndex + 1]}";
                    arrNames[iIndex] = string.Empty;
                }
            }
            else if (arrNames[iIndex].Contains('-', StringComparison.Ordinal))
            {
                var arrParts = arrNames[iIndex].Split('-', 2);
                arrNames[iIndex] = $"{arrParts[0].Capitalize()}-{(arrParts.Length > 1 ? arrParts[1].Capitalize() : string.Empty)}";
            }
            else
            {
                arrNames[iIndex] = arrNames[iIndex].Capitalize();
            }
        }

        var sLastName = arrNames[^1];
        var sFirstName = string.Join(" ", arrNames[..^1]).Trim(' ');
        return (sLastName, sFirstName);
    }

    /// <summary>
    /// Rewrites absolute page references into relative local paths.
    /// </summary>
    public static string MakeLocal(string sSource, string sReference)
    {
        var iSlashCount = sReference.Count(c => c == '/');
        const string sPrefix = "../../../../../";
        var sRelative = sPrefix[..Math.Max(0, Math.Min(sPrefix.Length, (iSlashCount - 3) * 3))];
        sSource = sSource.Replace("src=\"/", $"src=\"{sRelative}", StringComparison.Ordinal);
        sSource = sSource.Replace("href=\"/", $"href=\"{sRelative}", StringComparison.Ordinal);
        var sRest = sReference.Length > 9 ? sReference[9..] : string.Empty;
        var iIndex = sRest.IndexOf('/', StringComparison.Ordinal);
        var sRoot = iIndex >= 0 && sReference.Length >= iIndex + 10 ? sReference[..(iIndex + 10)] : sReference;
        sSource = sSource.Replace(sRoot, sRelative, StringComparison.Ordinal);
        sSource = sSource.Replace(".jpg", ".png", StringComparison.OrdinalIgnoreCase);
        return sSource;
    }

    /// <summary>
    /// Maps a remote RNZ URL to the local file system path used by the Python scripts.
    /// </summary>
    public static string GetLocalPath(string sUrl, string sLocalPathRoot, DateOnly? dtReference = null)
    {
        var dtCurrent = dtReference ?? DateOnly.FromDateTime(DateTime.Today);
        var sLocalPath = sUrl.Replace("https://trauer.rnz.de", string.Empty, StringComparison.Ordinal)
            .Replace("/", "\\", StringComparison.Ordinal);

        var iQueryIndex = sLocalPath.IndexOf('?', StringComparison.Ordinal);
        if (iQueryIndex >= 0)
        {
            sLocalPath = sLocalPath[..iQueryIndex];
        }

        if (sLocalPath.Contains("aktuelle-ausgabe", StringComparison.Ordinal))
        {
            sLocalPath = sLocalPath.Replace(
                "suche\\aktuelle-ausgabe",
                $"suche\\erscheinungstag-{dtCurrent.Day}-{dtCurrent.Month}-{dtCurrent.Year}",
                StringComparison.Ordinal);
        }

        var iFound = sLocalPath.IndexOf("erscheinungstag", StringComparison.Ordinal);
        if (iFound >= 0)
        {
            sLocalPath = sLocalPath.Replace("\\seite", "-pg", StringComparison.Ordinal);
            foreach (var sAnnouncementType in new[] { "nachrufe", "danksagungen", "todesanzeigen", "_" })
            {
                sLocalPath = sLocalPath.Replace($"\\anzeigenart-{sAnnouncementType}", $"-{sAnnouncementType[..1]}", StringComparison.Ordinal);
            }

            if (sLocalPath.Contains("tag-heute", StringComparison.Ordinal))
            {
                sLocalPath = sLocalPath.Replace(
                    "traueranzeigen-suche\\erscheinungstag",
                    $"{dtCurrent.Year}\\{dtCurrent:yyyy-MM-dd}\\liste",
                    StringComparison.Ordinal);
            }
            else
            {
                var iStartIndex = Math.Min(sLocalPath.Length, iFound + 16);
                var sDateFragment = sLocalPath.Substring(iStartIndex, Math.Min(10, Math.Max(0, sLocalPath.Length - iStartIndex))).LCropStr("\\");
                var arrSplit = sDateFragment.Split('-');
                if (arrSplit.Length >= 3)
                {
                    sLocalPath = sLocalPath.Replace(
                        "traueranzeigen-suche\\erscheinungstag",
                        $"{arrSplit[2]}\\{arrSplit[2]}-{arrSplit[1].PadLeft(2, '0')}-{arrSplit[0].PadLeft(2, '0')}\\liste",
                        StringComparison.Ordinal);
                }
            }
        }

        const string sMarker = "traueranzeige\\";
        iFound = sLocalPath.IndexOf(sMarker, StringComparison.Ordinal);
        if (iFound >= 0)
        {
            var sPathPart = sLocalPath[(iFound + sMarker.Length)..].LCropStr("\\");
            using var xMd5 = MD5.Create();
            var arrDigest = xMd5.ComputeHash(Encoding.UTF8.GetBytes(sPathPart));
            var iCrc = (((int)arrDigest[^2] << 8) | arrDigest[^1]) & 1023;
            sLocalPath = sLocalPath.Replace(sMarker, $"{sMarker}{iCrc:X3}\\", StringComparison.Ordinal);
        }

        return sLocalPathRoot + sLocalPath;
    }

    /// <summary>
    /// Extracts text from a PDF byte array.
    /// </summary>
    public static string PdfText(byte[] arrBytes)
    {
        try
        {
            using var xStream = new MemoryStream(arrBytes);
            using var xDocument = PdfDocument.Open(xStream);
            var sBuilder = new StringBuilder();
            foreach (var xPage in xDocument.GetPages())
            {
                string sText = xPage.Text;
                if (!sText.Contains("\r\n"))
                {
                    sText = "";
                    double rFontSize = 0d;
                    string FontName = "";
                    foreach (var letter in xPage.Letters)
                    {
                        // Todo: Space and double punch - detection
                        if (letter.FontSize != rFontSize || letter.FontName != FontName)
                        {
                            rFontSize = letter.FontSize;
                            FontName = letter.FontName;
                            if (!string.IsNullOrEmpty(sText))
                                sText += $"\r\n";
                        }
                        sText += letter.Value;
                    }
                }
                sBuilder.AppendLine(sText);
            }

            return sBuilder.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Converts a supported CLR value into a <see cref="JsonNode"/>.
    /// </summary>
    public static JsonNode? ToJsonNode(this object? xValue) => xValue switch
    {
        null => null,
        JsonNode xNode => xNode.DeepClone(),
        string sValue => JsonValue.Create(sValue),
        bool xValue1 => JsonValue.Create(xValue1),
        int iValue => JsonValue.Create(iValue),
        long iValue => JsonValue.Create(iValue),
        double fValue => JsonValue.Create(fValue),
        decimal fValue => JsonValue.Create(fValue),
        DateOnly dtValue => JsonValue.Create(dtValue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
        DateTime dtValue => JsonValue.Create(dtValue.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)),
        byte[] arrBytes => JsonValue.Create(Convert.ToBase64String(arrBytes)),
        IDictionary<string, object?> dValues => ToJsonObject(new Dictionary<string, object?>(dValues, StringComparer.Ordinal)),
        IDictionary<string, string> dValues => ToJsonObject(dValues.ToDictionary(k => k.Key, v => (object?)v.Value)),
        IEnumerable<Dictionary<string, object?>> arrItems => ToJsonArray(arrItems.Cast<object?>()),
        IEnumerable<object?> arrItems when xValue is not string => ToJsonArray(arrItems),
        _ => JsonSerializer.SerializeToNode(xValue, JsonOptions)
    };

    /// <summary>
    /// Converts a dictionary into a JSON object.
    /// </summary>
    public static JsonObject ToJsonObject(this IReadOnlyDictionary<string, object?> dValues)
    {
        var xObject = new JsonObject();
        foreach (var kvValue in dValues)
        {
            xObject[kvValue.Key] = kvValue.Value.ToJsonNode();
        }

        return xObject;
    }

    private static JsonArray ToJsonArray(IEnumerable<object?> arrItems)
    {
        var xArray = new JsonArray();
        foreach (var xItem in arrItems)
        {
            xArray.Add(xItem.ToJsonNode());
        }

        return xArray;
    }

    public static string Capitalize(this string sValue)
    {
        if (string.IsNullOrEmpty(sValue))
        {
            return sValue;
        }

        if (sValue.Length == 1)
        {
            return sValue.ToUpperInvariant();
        }

        return char.ToUpperInvariant(sValue[0]) + sValue[1..].ToLowerInvariant();
    }
}
