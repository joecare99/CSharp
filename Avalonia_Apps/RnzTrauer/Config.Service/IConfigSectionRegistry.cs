using System;
using System.Collections.Generic;

namespace Config.Service;

/// <summary>
/// Registry for all configuration sections contributed by application parts.
/// Emits events when the section list changes (new part added, existing removed).
/// </summary>
public interface IConfigSectionRegistry
{
    /// <summary>All registered configuration sections, sorted by <see cref="IConfigSectionProvider.Order"/>.</summary>
    IReadOnlyCollection<IConfigSectionProvider> Sections { get; }

    /// <summary>Registers a configuration section.</summary>
    void AddSection(IConfigSectionProvider section);

    /// <summary>Removes a configuration section by name.</summary>
    bool RemoveSection(string sectionName);

    /// <summary>Gets a section by its stable name.</summary>
    bool TryGetSection(string sectionName, out IConfigSectionProvider? section);

    /// <summary>Event raised when the section list changes.</summary>
    event Action? SectionsChanged;
}
