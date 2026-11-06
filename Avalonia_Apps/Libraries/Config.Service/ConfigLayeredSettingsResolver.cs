using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Config.Service;

/// <summary>
/// Resolves registered section properties against explicitly supplied layer stores.
/// </summary>
public sealed class ConfigLayeredSettingsResolver
{
    private readonly IConfigSectionRegistry _sectionRegistry;
    private readonly IConfigLayerStore[] _layerStores;
    private readonly bool _trustEnforcedSystemValues;

    public ConfigLayeredSettingsResolver(
        IConfigSectionRegistry sectionRegistry,
        IEnumerable<IConfigLayerStore> layerStores,
        bool trustEnforcedSystemValues = false)
    {
        _sectionRegistry = sectionRegistry ?? throw new ArgumentNullException(nameof(sectionRegistry));
        ArgumentNullException.ThrowIfNull(layerStores);
        _layerStores = layerStores.ToArray();
        if (_layerStores.Any(static store => store is null))
        {
            throw new ArgumentException("Layer store collection cannot contain null entries.", nameof(layerStores));
        }

        if (_layerStores.Select(static store => store.Layer).Distinct().Count() != _layerStores.Length)
        {
            throw new ArgumentException("Only one store may be registered for each configuration layer.", nameof(layerStores));
        }

        _trustEnforcedSystemValues = trustEnforcedSystemValues;
    }

    /// <summary>
    /// Loads the product default and registered snapshots, then resolves one public property.
    /// Store failures are surfaced to the caller; missing files are represented by empty snapshots.
    /// </summary>
    public async Task<ConfigValueResolution> ResolveAsync(
        string sectionName,
        string propertyName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        cancellationToken.ThrowIfCancellationRequested();

        if (!_sectionRegistry.TryGetSection(sectionName, out IConfigSectionProvider? section)
            || section is null)
        {
            throw new KeyNotFoundException($"Configuration section '{sectionName}' is not registered.");
        }

        object productDefaultModel = section.CreateModel()
            ?? throw new InvalidOperationException($"Configuration section '{section.Name}' returned no default model.");
        PropertyInfo property = section.ModelType.GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public)
            ?? throw new KeyNotFoundException(
                $"Configuration property '{section.Name}.{propertyName}' is not a public instance property.");
        if (property.GetIndexParameters().Length != 0 || property.GetMethod is null)
        {
            throw new InvalidOperationException(
                $"Configuration property '{section.Name}.{propertyName}' cannot be used as a setting.");
        }

        object? productDefault = property.GetValue(productDefaultModel);
        ConfigLayerSnapshot[] snapshots = await Task.WhenAll(
                _layerStores.Select(store => store.LoadAsync(cancellationToken)))
            .ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        ConfigLayerSnapshot? system = snapshots.SingleOrDefault(static snapshot => snapshot.Layer == ConfigLayer.System);
        ConfigLayerSnapshot? group = snapshots.SingleOrDefault(static snapshot => snapshot.Layer == ConfigLayer.Group);
        ConfigLayerSnapshot? user = snapshots.SingleOrDefault(static snapshot => snapshot.Layer == ConfigLayer.User);

        return ConfigLayerResolver.Resolve(
            new ConfigPropertyKey(section.Name, property.Name),
            property.PropertyType,
            productDefault,
            system,
            group,
            user,
            _trustEnforcedSystemValues);
    }

    /// <summary>
    /// Resolves every readable public property in a registered section from one consistent set of layer snapshots.
    /// An optional draft snapshot replaces the loaded snapshot for its layer and must correspond to a registered store.
    /// </summary>
    public async Task<IReadOnlyDictionary<string, ConfigValueResolution>> ResolveSectionAsync(
        string sectionName,
        ConfigLayerSnapshot? draftSnapshot = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);
        cancellationToken.ThrowIfCancellationRequested();

        if (!_sectionRegistry.TryGetSection(sectionName, out IConfigSectionProvider? section)
            || section is null)
        {
            throw new KeyNotFoundException($"Configuration section '{sectionName}' is not registered.");
        }

        object productDefaultModel = section.CreateModel()
            ?? throw new InvalidOperationException($"Configuration section '{section.Name}' returned no default model.");
        ConfigLayerSnapshot[] snapshots = await Task.WhenAll(
                _layerStores.Select(store => store.LoadAsync(cancellationToken)))
            .ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();

        if (draftSnapshot is not null)
        {
            int index = Array.FindIndex(snapshots, snapshot => snapshot.Layer == draftSnapshot.Layer);
            if (index < 0)
            {
                throw new InvalidOperationException(
                    $"No store is registered for the {draftSnapshot.Layer} layer draft.");
            }

            snapshots[index] = draftSnapshot;
        }

        ConfigLayerSnapshot? system = snapshots.SingleOrDefault(static snapshot => snapshot.Layer == ConfigLayer.System);
        ConfigLayerSnapshot? group = snapshots.SingleOrDefault(static snapshot => snapshot.Layer == ConfigLayer.Group);
        ConfigLayerSnapshot? user = snapshots.SingleOrDefault(static snapshot => snapshot.Layer == ConfigLayer.User);
        Dictionary<string, ConfigValueResolution> resolutions = new(StringComparer.Ordinal);

        foreach (PropertyInfo property in section.ModelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.GetIndexParameters().Length != 0 || property.GetMethod is null)
            {
                continue;
            }

            ConfigPropertyKey key = new(section.Name, property.Name);
            resolutions.Add(
                property.Name,
                ConfigLayerResolver.Resolve(
                    key,
                    property.PropertyType,
                    property.GetValue(productDefaultModel),
                    system,
                    group,
                    user,
                    _trustEnforcedSystemValues));
        }

        return new ReadOnlyDictionary<string, ConfigValueResolution>(resolutions);
    }
}
