using Config.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Config.UI.ConfigService;

/// <summary>
/// Projects registered Config.Service sections into the Config.UI registry.
/// </summary>
public sealed class ConfigServiceUiSectionRegistry : IConfigUiSectionRegistry
{
    /// <summary>
    /// Initializes a registry from Config.Service registrations.
    /// </summary>
    public ConfigServiceUiSectionRegistry(
        global::Config.Service.ConfigService configService,
        IConfigSectionRegistry registry,
        bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(configService);
        ArgumentNullException.ThrowIfNull(registry);

        Sections = registry.Sections
            .OrderBy(static section => section.Order)
            .ThenBy(static section => section.Name, StringComparer.OrdinalIgnoreCase)
            .Select(section => (IConfigUiSection)new ConfigServiceUiSection(configService, section, isReadOnly))
            .ToArray();
    }

    /// <inheritdoc/>
    public IReadOnlyList<IConfigUiSection> Sections { get; }
}
