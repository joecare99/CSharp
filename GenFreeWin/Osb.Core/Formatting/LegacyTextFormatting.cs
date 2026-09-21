using System;

namespace Osb.Core.Formatting;

public static class LegacyTextFormatting
{
    public static string FormatGeoCoordinate(string coordinate)
    {
        if (coordinate == null)
        {
            throw new ArgumentNullException(nameof(coordinate));
        }

        var separatorIndex = coordinate.IndexOf(',');
        if (separatorIndex < 0)
        {
            separatorIndex = coordinate.IndexOf('.');
        }

        var result = coordinate.Substring(0, separatorIndex) + "\u00b0 ";
        result += coordinate.Substring(separatorIndex + 1, Math.Min(2, coordinate.Length - separatorIndex - 1)) + "' ";

        var secondsStart = separatorIndex + 3;
        if (secondsStart < coordinate.Length)
        {
            result += coordinate.Substring(secondsStart, Math.Min(2, coordinate.Length - secondsStart));
        }

        return (result + "''").Trim();
    }

    public static string FormatName(string name, string prefix, string suffix)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (prefix == null)
        {
            throw new ArgumentNullException(nameof(prefix));
        }

        if (suffix == null)
        {
            throw new ArgumentNullException(nameof(suffix));
        }

        var result = name.ToUpperInvariant();
        if (prefix.Length != 0)
        {
            result = prefix + " " + result;
        }

        if (suffix.Length != 0)
        {
            result += " " + suffix;
        }

        return result;
    }

    public static string RemoveQuotes(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return value.Replace("\"", string.Empty);
    }

    public static string DecodeLegacyText(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return value.Replace("ssss", "ß");
    }

    public static string EncodeLegacyText(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return value.Replace("ß", "ssss");
    }

    public static string NormalizeText(string value, bool replaceLineBreaks)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (replaceLineBreaks)
        {
            value = value.Replace("\r", " ").Replace("\n", " ");
        }

        while (value.IndexOf("  ", StringComparison.Ordinal) >= 0)
        {
            value = value.Replace("  ", " ");
        }

        return value;
    }

    public static string NormalizeText(string value, LegacyFormattingOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return NormalizeText(value, !options.PreserveLineBreaks);
    }

    public static string FormatLegacyDate(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (value.Length < 8)
        {
            value = (value + "00000000").Substring(0, 8);
        }

        var day = value.Substring(6, 2);
        var month = value.Substring(4, 2);
        var year = value.Substring(0, 4);

        if (day == "00")
        {
            day = "  ";
        }

        if (month == "00")
        {
            month = "  ";
        }

        return (day + " " + month + " " + year).Trim().Replace(" ", ".");
    }

    public static string FormatRepositoryLocation(
        string name,
        string street,
        string postalCode,
        string place,
        string phone,
        string email)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (street == null)
        {
            throw new ArgumentNullException(nameof(street));
        }

        if (postalCode == null)
        {
            throw new ArgumentNullException(nameof(postalCode));
        }

        if (place == null)
        {
            throw new ArgumentNullException(nameof(place));
        }

        if (phone == null)
        {
            throw new ArgumentNullException(nameof(phone));
        }

        if (email == null)
        {
            throw new ArgumentNullException(nameof(email));
        }

        var result = string.Empty;
        AppendWithSuffix(ref result, name, ", ");
        AppendWithSuffix(ref result, street, ", ");
        AppendWithSuffix(ref result, postalCode, " ");
        AppendWithSuffix(ref result, place, ", ");
        AppendWithSuffix(ref result, phone, ", ");
        AppendWithSuffix(ref result, email, string.Empty);
        return "Standort: " + result.TrimEnd();
    }

    private static void AppendWithSuffix(ref string result, string value, string suffix)
    {
        var trimmed = value.Trim();
        if (trimmed.Length != 0)
        {
            result += trimmed + suffix;
        }
    }
}
