using System;

namespace Config.Service;

/// <summary>
/// Identifies a configuration property independently of its storage layer.
/// </summary>
public readonly record struct ConfigPropertyKey
{
    public ConfigPropertyKey(string sectionName, string propertyName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        SectionName = sectionName;
        PropertyName = propertyName;
    }

    public string SectionName { get; }

    public string PropertyName { get; }
}
