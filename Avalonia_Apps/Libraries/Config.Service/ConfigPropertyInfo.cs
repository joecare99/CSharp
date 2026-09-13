using System;
using System.Collections.Generic;

namespace Config.Service;

/// <summary>
/// Immutable descriptor for one editable property of a configuration model.
/// UIs use <see cref="Kind"/> to pick a control and <see cref="EnumOptions"/>
/// to populate closed-choice editors.
/// </summary>
public sealed class ConfigPropertyInfo
{
    /// <summary>Creates a property descriptor with an optional enum option list.</summary>
    /// <param name="name">Stable property name used for persistence and lookup.</param>
    /// <param name="kind">Editor kind that describes how the value should be edited.</param>
    /// <param name="isSensitive">Whether the value is sensitive and must be masked in UIs.</param>
    /// <param name="enumOptions">Names of the enum members when <paramref name="kind"/> is <see cref="ConfigPropertyKind.Enum"/>.</param>
    public ConfigPropertyInfo(
        string name,
        ConfigPropertyKind kind,
        bool isSensitive,
        IReadOnlyCollection<string>? enumOptions = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Kind = kind;
        IsSensitive = isSensitive;
        EnumOptions = enumOptions ?? Array.Empty<string>();
    }

    /// <summary>Stable property name used for persistence and lookup.</summary>
    public string Name { get; }

    /// <summary>Editor kind for the property.</summary>
    public ConfigPropertyKind Kind { get; }

    /// <summary>True when the value is sensitive and must be masked in configuration UIs.</summary>
    public bool IsSensitive { get; }

    /// <summary>Enum member names for closed-choice editors; empty for all other kinds.</summary>
    public IReadOnlyCollection<string> EnumOptions { get; }
}
