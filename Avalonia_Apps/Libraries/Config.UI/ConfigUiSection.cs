using System;

namespace Config.UI;

/// <summary>
/// Describes a localized configuration section at an application host boundary.
/// </summary>
public sealed class ConfigUiSection
{
    /// <summary>
    /// Initializes a configuration UI section descriptor.
    /// </summary>
    public ConfigUiSection(string name, string displayName, string? description = null, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A section name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("A section display name is required.", nameof(displayName));
        }

        Name = name;
        DisplayName = displayName;
        Description = description;
        SortOrder = sortOrder;
    }

    /// <summary>Gets the stable section identifier.</summary>
    public string Name { get; }

    /// <summary>Gets the localized section display text.</summary>
    public string DisplayName { get; }

    /// <summary>Gets an optional localized section explanation.</summary>
    public string? Description { get; }

    /// <summary>Gets the deterministic section order.</summary>
    public int SortOrder { get; }
}
