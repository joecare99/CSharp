using Config.Service;
using Property.Editor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Config.UI.ConfigService;

/// <summary>
/// Projects registered configuration sections into the shared UI for one explicit target layer.
/// </summary>
public sealed class ConfigLayeredUiSectionRegistry : IConfigUiSectionRegistry
{
    /// <summary>
    /// Initializes a UI registry whose sections all edit the selected layer store.
    /// </summary>
    public ConfigLayeredUiSectionRegistry(
        IConfigSectionRegistry sectionRegistry,
        ConfigLayeredSettingsResolver resolver,
        IConfigLayerStore targetStore,
        ConfigLayer targetLayer,
        bool isReadOnly = false,
        string? storageLocation = null)
        : this(sectionRegistry, resolver, targetStore, targetLayer, null, isReadOnly, storageLocation)
    {
    }

    /// <summary>
    /// Initializes a registry with host-supplied property options for each section.
    /// The callback may return null to use the default options.
    /// </summary>
    public ConfigLayeredUiSectionRegistry(
        IConfigSectionRegistry sectionRegistry,
        ConfigLayeredSettingsResolver resolver,
        IConfigLayerStore targetStore,
        ConfigLayer targetLayer,
        Func<IConfigSectionProvider, System.Reflection.PropertyInfo, IReadOnlyList<PropertyOption>?>? optionsProvider,
        bool isReadOnly = false,
        string? storageLocation = null)
    {
        ArgumentNullException.ThrowIfNull(sectionRegistry);
        ArgumentNullException.ThrowIfNull(resolver);
        ArgumentNullException.ThrowIfNull(targetStore);
        if (!Enum.IsDefined(targetLayer))
        {
            throw new ArgumentOutOfRangeException(nameof(targetLayer));
        }

        if (targetStore.Layer != targetLayer)
        {
            throw new ArgumentException("The target store must match the selected configuration layer.", nameof(targetStore));
        }

        ConfigLayerDraftSession draftSession = new(targetStore);
        Sections = sectionRegistry.Sections
            .OrderBy(static section => section.Order)
            .ThenBy(static section => section.Name, StringComparer.OrdinalIgnoreCase)
            .Select(section => (IConfigUiSection)new ConfigLayeredUiSection(
                resolver,
                section,
                targetStore,
                targetLayer,
                draftSession,
                isReadOnly,
                storageLocation,
                optionsProvider))
            .ToArray();
    }

    /// <inheritdoc/>
    public IReadOnlyList<IConfigUiSection> Sections { get; }
}
