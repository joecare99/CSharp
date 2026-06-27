using System;

namespace AppKomponentBaseLib.Configuration;

/// <summary>
/// Describes a contributed application setting or user preference.
/// </summary>
public sealed class AppSettingDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppSettingDescriptor"/> class.
    /// </summary>
    /// <param name="settingKey">The stable setting key.</param>
    /// <param name="displayName">The host-facing display name.</param>
    /// <param name="sectionKey">The stable section key used for grouping.</param>
    /// <param name="scope">The intended persistence scope.</param>
    /// <param name="description">The optional setting description.</param>
    /// <param name="defaultValue">The optional default value.</param>
    public AppSettingDescriptor(
        string settingKey,
        string displayName,
        string sectionKey,
        AppSettingScope scope,
        string? description = null,
        object? defaultValue = null)
    {
        if (string.IsNullOrWhiteSpace(settingKey))
        {
            throw new ArgumentException("A stable setting key is required.", nameof(settingKey));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("A display name is required.", nameof(displayName));
        }

        if (string.IsNullOrWhiteSpace(sectionKey))
        {
            throw new ArgumentException("A section key is required.", nameof(sectionKey));
        }

        SettingKey = settingKey;
        DisplayName = displayName;
        SectionKey = sectionKey;
        Scope = scope;
        Description = description;
        DefaultValue = defaultValue;
    }

    /// <summary>
    /// Gets the stable setting key.
    /// </summary>
    public string SettingKey { get; }

    /// <summary>
    /// Gets the host-facing display name.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets the stable section key used for grouping.
    /// </summary>
    public string SectionKey { get; }

    /// <summary>
    /// Gets the intended persistence scope.
    /// </summary>
    public AppSettingScope Scope { get; }

    /// <summary>
    /// Gets the optional setting description.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the optional default value.
    /// </summary>
    public object? DefaultValue { get; }
}
