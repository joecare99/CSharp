using System;
using System.Collections.Generic;
using System.Linq;

namespace Config.Service;

/// <summary>
/// Implements <see cref="IConfigSectionRegistry"/> by collecting and maintaining
/// the section list from all registered providers. Emits a single event on changes.
/// </summary>
public sealed class ConfigSectionRegistry : IConfigSectionRegistry
{
    private readonly List<IConfigSectionProvider> _sections = new();
    private readonly object _lock = new();

    /// <inheritdoc/>
    public IReadOnlyCollection<IConfigSectionProvider> Sections =>
        _sections
            .OrderBy(s => s.Order)
            .ThenBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
            .ToList()
            .AsReadOnly();

    /// <inheritdoc/>
    public event Action? SectionsChanged;

    /// <summary>Registers a section provider atomically and emits an event if the list changed.</summary>
    public void AddSection(IConfigSectionProvider section)
    {
        ArgumentNullException.ThrowIfNull(section);

        lock (_lock)
        {
            var existing = _sections.Find(s => string.Equals(s.Name, section.Name, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                return;
            }

            _sections.Add(section);
            SectionsChanged?.Invoke();
        }
    }

    /// <summary>Unregisters a section provider and emits an event if the list changed.</summary>
    public bool RemoveSection(string sectionName)
    {
        ArgumentNullException.ThrowIfNull(sectionName);

        lock (_lock)
        {
            var toRemove = _sections.RemoveAll(s => string.Equals(s.Name, sectionName, StringComparison.OrdinalIgnoreCase));
            if (toRemove > 0)
            {
                SectionsChanged?.Invoke();
                return true;
            }

            return false;
        }
    }

    /// <inheritdoc/>
    public bool TryGetSection(string sectionName, out IConfigSectionProvider? section)
    {
        ArgumentNullException.ThrowIfNull(sectionName);

        lock (_lock)
        {
            section = _sections.FirstOrDefault(s => string.Equals(s.Name, sectionName, StringComparison.OrdinalIgnoreCase));
            return section is not null;
        }
    }
}
