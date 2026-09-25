using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osb.Core.Profiles;

public enum PlaceSelectionMode
{
    IncludeOnly,
    ExcludeSelected
}

public sealed record PublicationProfile
{
    public PublicationProfile(
        int schemaVersion,
        string name,
        PlaceSelectionMode inclusionMode,
        IReadOnlyList<string> selectedPlaceIds,
        string titleTemplate,
        string footerTemplate)
    {
        if (schemaVersion <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(schemaVersion));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A publication profile requires a non-empty name.", nameof(name));
        }

        if (selectedPlaceIds == null)
        {
            throw new ArgumentNullException(nameof(selectedPlaceIds));
        }

        if (string.IsNullOrWhiteSpace(titleTemplate))
        {
            throw new ArgumentException("A publication profile requires a title template.", nameof(titleTemplate));
        }

        if (string.IsNullOrWhiteSpace(footerTemplate))
        {
            throw new ArgumentException("A publication profile requires a footer template.", nameof(footerTemplate));
        }

        var normalizedPlaceIds = new List<string>();
        foreach (var placeId in selectedPlaceIds)
        {
            if (string.IsNullOrWhiteSpace(placeId))
            {
                throw new ArgumentException("Selected place IDs cannot be null or empty.", nameof(selectedPlaceIds));
            }

            normalizedPlaceIds.Add(placeId.Trim());
        }

        SchemaVersion = schemaVersion;
        Name = name.Trim();
        InclusionMode = inclusionMode;
        SelectedPlaceIds = normalizedPlaceIds;
        TitleTemplate = titleTemplate.Trim();
        FooterTemplate = footerTemplate.Trim();
    }

    public int SchemaVersion { get; }

    public string Name { get; }

    public PlaceSelectionMode InclusionMode { get; }

    public IReadOnlyList<string> SelectedPlaceIds { get; }

    public string TitleTemplate { get; }

    public string FooterTemplate { get; }
}

public static class PublicationProfileCodec
{
    public static string Serialize(PublicationProfile profile)
    {
        if (profile == null)
        {
            throw new ArgumentNullException(nameof(profile));
        }

        var builder = new StringBuilder();
        builder.AppendLine($"schemaVersion={profile.SchemaVersion}");
        builder.AppendLine($"name={Escape(profile.Name)}");
        builder.AppendLine($"mode={profile.InclusionMode}");
        builder.AppendLine($"title={Escape(profile.TitleTemplate)}");
        builder.AppendLine($"footer={Escape(profile.FooterTemplate)}");
        builder.AppendLine($"places={string.Join("|", profile.SelectedPlaceIds.Select(Escape))}");
        return builder.ToString();
    }

    public static PublicationProfile Deserialize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("The profile payload is empty.", nameof(text));
        }

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries))
        {
            var index = line.IndexOf('=');
            if (index <= 0)
            {
                throw new FormatException($"Invalid profile line '{line}'.");
            }

            var key = line.Substring(0, index).Trim();
            var value = Unescape(line.Substring(index + 1));
            values[key] = value;
        }

        if (!values.TryGetValue("schemaVersion", out var schemaVersionText) ||
            !int.TryParse(schemaVersionText, out var schemaVersion))
        {
            throw new FormatException("The profile payload is missing a valid schemaVersion.");
        }

        if (!values.TryGetValue("name", out var name) || string.IsNullOrWhiteSpace(name))
        {
            throw new FormatException("The profile payload is missing a valid name.");
        }

        if (!values.TryGetValue("mode", out var modeText) || !Enum.TryParse<PlaceSelectionMode>(modeText, true, out var mode))
        {
            throw new FormatException("The profile payload is missing a valid inclusion mode.");
        }

        if (!values.TryGetValue("title", out var title) || string.IsNullOrWhiteSpace(title))
        {
            throw new FormatException("The profile payload is missing a valid title template.");
        }

        if (!values.TryGetValue("footer", out var footer) || string.IsNullOrWhiteSpace(footer))
        {
            throw new FormatException("The profile payload is missing a valid footer template.");
        }

        var placeIds = Array.Empty<string>();
        if (values.TryGetValue("places", out var placeListText) && !string.IsNullOrWhiteSpace(placeListText))
        {
            placeIds = placeListText
                .Split('|')
                .Select(Unescape)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToArray();
        }

        return new PublicationProfile(
            schemaVersion,
            name,
            mode,
            placeIds,
            title,
            footer);
    }

    private static string Escape(string value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        return value
            .Replace("\\", "\\\\")
            .Replace("|", "\\|")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n");
    }

    private static string Unescape(string value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        return value
            .Replace("\\|", "|")
            .Replace("\\r", "\r")
            .Replace("\\n", "\n")
            .Replace("\\\\", "\\");
    }
}
