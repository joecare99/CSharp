using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Config.Service;

/// <summary>
/// Reflection-based inspector that examines a configuration model type and produces
/// a list of <see cref="ConfigPropertyInfo"/> descriptors. Honors <see cref="ConfigIgnoreAttribute"/>
/// for exclusion and <see cref="SensitiveConfigPropertyAttribute"/> for masking in UIs.
/// Infers property kind from type: string → Text, numeric types → Number, bool → Boolean, enum → Enum.
/// </summary>
internal sealed class ConfigModelInspector
{
    /// <summary>
    /// Inspects a configuration model type and returns its supported properties as descriptors.
    /// Only public instance properties are inspected; private/internal ones are ignored.
    /// </summary>
    /// <param name="modelType">The configuration model type to inspect.</param>
    /// <returns>A read-only list of property descriptors, ordered by declaration order in the model type.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="modelType"/> is null.</exception>
    public static IReadOnlyCollection<ConfigPropertyInfo> Inspect(Type modelType)
    {
        ArgumentNullException.ThrowIfNull(modelType);

        if (!modelType.IsClass || modelType.IsAbstract)
        {
            throw new ArgumentException($"Model type must be a concrete class, got: {modelType}", nameof(modelType));
        }

        var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        return properties.Select(CreatePropertyInfo).Where(i => i.Name != null).OrderBy(i => i.Name).ToList().AsReadOnly();
    }

    /// <summary>Creates a property info from a single PropertyInfo, inspecting attributes and type.</summary>
    private static ConfigPropertyInfo CreatePropertyInfo(PropertyInfo property)
    {
        var name = property.Name;
        var isSensitive = property.GetCustomAttribute<SensitiveConfigPropertyAttribute>() != null;
        var isIgnored = property.GetCustomAttribute<ConfigIgnoreAttribute>() != null;

        if (isIgnored)
        {
            return new ConfigPropertyInfo(name, ConfigPropertyKind.Text, isSensitive);
        }

        var type = property.PropertyType;
        var kind = DetermineKind(type);
        var enumOptions = (kind == ConfigPropertyKind.Enum) ? Enum.GetNames(type) : Array.Empty<string>();

        return new ConfigPropertyInfo(name, kind, isSensitive, enumOptions);
    }

    /// <summary>Determines the editor kind from a property type.</summary>
    private static ConfigPropertyKind DetermineKind(Type type)
    {
        if (type == typeof(string))
        {
            return ConfigPropertyKind.Text;
        }

        if (type == typeof(bool))
        {
            return ConfigPropertyKind.Boolean;
        }

        if (typeof(IEquatable<>).IsAssignableFrom(type) && type.IsEnum)
        {
            return ConfigPropertyKind.Enum;
        }

        if (type.IsPrimitive || typeof(decimal).IsAssignableFrom(type))
        {
            return ConfigPropertyKind.Number;
        }

        // Unknown types default to Text for flexibility.
        return ConfigPropertyKind.Text;
    }
}
