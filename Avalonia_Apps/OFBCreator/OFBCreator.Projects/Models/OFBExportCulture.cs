using System;
using System.Globalization;
using System.IO;

namespace OFBCreator.Projects.Models;

/// <summary>
/// Supported culture identifiers for generated OFB date presentation.
/// </summary>
public static class OFBExportCulture
{
    public const string Default = "de-DE";
    public const string German = "de-DE";
    public const string EnglishUnitedStates = "en-US";

    /// <summary>
    /// Resolves a supported project culture.
    /// </summary>
    public static CultureInfo GetCulture(string cultureName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cultureName);
        if (!string.Equals(cultureName, German, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(cultureName, EnglishUnitedStates, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException(
                $"Export culture must be '{German}' or '{EnglishUnitedStates}'.",
                nameof(cultureName));
        return CultureInfo.GetCultureInfo(cultureName);
    }

    public static void Validate(string cultureName)
    {
        try
        {
            _ = GetCulture(cultureName);
        }
        catch (ArgumentException exception)
        {
            throw new InvalidDataException(exception.Message, exception);
        }
    }
}
