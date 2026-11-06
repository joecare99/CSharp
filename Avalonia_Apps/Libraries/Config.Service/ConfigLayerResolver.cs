using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Config.Service;

/// <summary>
/// Resolves an effective property value from product defaults and explicit layer overrides.
/// </summary>
public static class ConfigLayerResolver
{
    /// <summary>
    /// Resolves using the precedence: enforced system policy, user, group, system, product default.
    /// </summary>
    public static ConfigValueResolution Resolve(
        ConfigPropertyKey key,
        Type propertyType,
        object? productDefault,
        ConfigLayerSnapshot? system = null,
        ConfigLayerSnapshot? group = null,
        ConfigLayerSnapshot? user = null,
        bool trustEnforcedSystemValues = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key.SectionName);
        ArgumentException.ThrowIfNullOrWhiteSpace(key.PropertyName);
        ArgumentNullException.ThrowIfNull(propertyType);
        if (propertyType == typeof(void) || propertyType.IsByRef || propertyType.IsPointer)
        {
            throw new ArgumentException("Configuration property type must be a value or reference type.", nameof(propertyType));
        }

        ValidateLayer(system, ConfigLayer.System, nameof(system));
        ValidateLayer(group, ConfigLayer.Group, nameof(group));
        ValidateLayer(user, ConfigLayer.User, nameof(user));
        ValidateValue(propertyType, productDefault, "product default", key);

        ConfigLayerValue? systemValue;
        if (trustEnforcedSystemValues
            && TryGet(system, key, out systemValue)
            && systemValue.IsEnforced)
        {
            return ResolveLayerValue(propertyType, systemValue, ConfigValueSource.System, isEnforced: true, key);
        }

        if (TryGet(user, key, out ConfigLayerValue? userValue))
        {
            return ResolveLayerValue(propertyType, userValue, ConfigValueSource.User, isEnforced: false, key);
        }

        if (TryGet(group, key, out ConfigLayerValue? groupValue))
        {
            return ResolveLayerValue(propertyType, groupValue, ConfigValueSource.Group, isEnforced: false, key);
        }

        if (TryGet(system, key, out systemValue))
        {
            return ResolveLayerValue(propertyType, systemValue, ConfigValueSource.System, isEnforced: false, key);
        }

        return new ConfigValueResolution(productDefault, ConfigValueSource.ProductDefault, IsEnforced: false);
    }

    private static void ValidateLayer(ConfigLayerSnapshot? snapshot, ConfigLayer expectedLayer, string parameterName)
    {
        if (snapshot is not null && snapshot.Layer != expectedLayer)
        {
            throw new ArgumentException($"Expected a {expectedLayer} layer snapshot.", parameterName);
        }
    }

    private static bool TryGet(
        ConfigLayerSnapshot? snapshot,
        ConfigPropertyKey key,
        [NotNullWhen(true)] out ConfigLayerValue? value)
    {
        if (snapshot is not null && snapshot.TryGetValue(key, out ConfigLayerValue? found))
        {
            value = found;
            return true;
        }

        value = null!;
        return false;
    }

    private static ConfigValueResolution ResolveLayerValue(
        Type propertyType,
        ConfigLayerValue layerValue,
        ConfigValueSource source,
        bool isEnforced,
        ConfigPropertyKey key)
    {
        object? value = ValidateValue(propertyType, layerValue.Value, source.ToString(), key);
        return new ConfigValueResolution(value, source, isEnforced);
    }

    private static object? ValidateValue(Type propertyType, object? value, string source, ConfigPropertyKey key)
    {
        Type? nullableType = Nullable.GetUnderlyingType(propertyType);
        Type effectiveType = nullableType ?? propertyType;
        if (value is JsonElement jsonValue)
        {
            if (jsonValue.ValueKind == JsonValueKind.Null)
            {
                value = null;
            }
            else
            {
                try
                {
                    value = jsonValue.Deserialize(propertyType);
                }
                catch (JsonException exception)
                {
                    throw new ArgumentException(
                        $"The {source} value for '{key.SectionName}.{key.PropertyName}' is not compatible with '{propertyType}'.",
                        exception);
                }
            }
        }

        if (value is null)
        {
            if (!propertyType.IsValueType || nullableType is not null)
            {
                return null;
            }
        }
        else if (effectiveType.IsInstanceOfType(value))
        {
            return value;
        }

        throw new ArgumentException(
            $"The {source} value for '{key.SectionName}.{key.PropertyName}' is not compatible with '{propertyType}'.");
    }
}
