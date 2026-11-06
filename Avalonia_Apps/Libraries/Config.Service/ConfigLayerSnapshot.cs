using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Config.Service;

/// <summary>
/// An immutable snapshot of the explicit overrides stored in one configuration layer.
/// </summary>
public sealed class ConfigLayerSnapshot
{
    public ConfigLayerSnapshot(
        ConfigLayer layer,
        IEnumerable<KeyValuePair<ConfigPropertyKey, ConfigLayerValue>> values,
        string? storageVersion = null)
    {
        if (!Enum.IsDefined(layer))
        {
            throw new ArgumentOutOfRangeException(nameof(layer));
        }

        ArgumentNullException.ThrowIfNull(values);
        Layer = layer;
        StorageVersion = storageVersion;
        Dictionary<ConfigPropertyKey, ConfigLayerValue> copy = [];
        foreach (KeyValuePair<ConfigPropertyKey, ConfigLayerValue> entry in values)
        {
            if (string.IsNullOrWhiteSpace(entry.Key.SectionName)
                || string.IsNullOrWhiteSpace(entry.Key.PropertyName))
            {
                throw new ArgumentException("Configuration property keys must include a section and property name.", nameof(values));
            }

            ArgumentNullException.ThrowIfNull(entry.Value);
            if (entry.Value.IsEnforced && layer != ConfigLayer.System)
            {
                throw new ArgumentException("Only a system-layer value can be marked as an enforced policy.", nameof(values));
            }

            if (!copy.TryAdd(entry.Key, entry.Value))
            {
                throw new ArgumentException(
                    $"Configuration property '{entry.Key.SectionName}.{entry.Key.PropertyName}' occurs more than once.",
                    nameof(values));
            }
        }

        Values = new ReadOnlyDictionary<ConfigPropertyKey, ConfigLayerValue>(copy);
    }

    public ConfigLayer Layer { get; }

    /// <summary>
    /// Gets the opaque storage version observed when this snapshot was loaded.
    /// Null indicates that the layer file did not exist when loaded.
    /// </summary>
    public string? StorageVersion { get; }

    public IReadOnlyDictionary<ConfigPropertyKey, ConfigLayerValue> Values { get; }

    public bool TryGetValue(ConfigPropertyKey key, [NotNullWhen(true)] out ConfigLayerValue? value)
    {
        return Values.TryGetValue(key, out value);
    }

    /// <summary>
    /// Returns a new draft snapshot containing an explicit override for <paramref name="key"/>.
    /// The observed storage version is preserved so the store can detect stale drafts.
    /// </summary>
    public ConfigLayerSnapshot WithValue(ConfigPropertyKey key, ConfigLayerValue value)
    {
        ValidateKey(key);
        ArgumentNullException.ThrowIfNull(value);

        Dictionary<ConfigPropertyKey, ConfigLayerValue> updated = new(Values)
        {
            [key] = value,
        };
        return new ConfigLayerSnapshot(Layer, updated, StorageVersion);
    }

    /// <summary>
    /// Returns a new draft snapshot with the override removed, allowing a lower layer to become effective.
    /// The observed storage version is preserved so the store can detect stale drafts.
    /// </summary>
    public ConfigLayerSnapshot WithoutValue(ConfigPropertyKey key)
    {
        ValidateKey(key);
        if (!Values.ContainsKey(key))
        {
            return this;
        }

        Dictionary<ConfigPropertyKey, ConfigLayerValue> updated = new(Values);
        updated.Remove(key);
        return new ConfigLayerSnapshot(Layer, updated, StorageVersion);
    }

    private static void ValidateKey(ConfigPropertyKey key)
    {
        if (string.IsNullOrWhiteSpace(key.SectionName)
            || string.IsNullOrWhiteSpace(key.PropertyName))
        {
            throw new ArgumentException("Configuration property keys must include a section and property name.", nameof(key));
        }
    }
}
