using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Config.Service;

/// <summary>
/// Facade for the configuration service that exposes a simple, type-safe API.
/// Clients use this to register sections and access the store via a stable base key.
/// </summary>
public sealed class ConfigService : IDisposable
{
    private readonly IConfigStore _store;
    private readonly IConfigSectionRegistry? _registry;
    private bool _disposed;

    public string BaseKey { get; }

    public string VendorName { get; }

    public string ApplicationName { get; }

    public ConfigService(
        IConfigStore store,
        string baseKey)
        : this(store, null, baseKey, "Vendor", "Application")
    {
    }

    public ConfigService(
        IConfigStore store,
        IConfigSectionRegistry? registry,
        string baseKey,
        string? vendorName = null,
        string? applicationName = null)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _registry = registry;
        BaseKey = baseKey ?? throw new ArgumentNullException(nameof(baseKey));
        VendorName = string.IsNullOrWhiteSpace(vendorName) ? "Vendor" : vendorName.Trim();
        ApplicationName = string.IsNullOrWhiteSpace(applicationName) ? "Application" : applicationName.Trim();
    }

    /// <summary>Gets a configuration model by its stable key, falling back to the default if missing.</summary>
    public Task<T?> LoadAsync<T>(string sectionKey) where T : notnull
    {
        var key = $"{BaseKey}.{sectionKey}";
        return _store.LoadAsync(key, default(T))!;
    }

    /// <summary>Saves a configuration model by its stable key.</summary>
    public Task SaveAsync<T>(string sectionKey, T value) where T : notnull
    {
        var key = $"{BaseKey}.{sectionKey}";
        return _store.SaveAsync(key, value);
    }

    /// <summary>Removes the stored configuration for a section.</summary>
    public Task ResetAsync(string sectionKey)
    {
        var key = $"{BaseKey}.{sectionKey}";
        return _store.ResetAsync(key);
    }

    /// <summary>Gets a single property from a stored configuration model.</summary>
    public async Task<TValue?> GetValueAsync<TModel, TValue>(string sectionKey, string propertyName, TValue? fallbackValue = default) where TModel : class, new()
    {
        var model = await LoadAsync<TModel>(sectionKey);
        return GetPropertyValue(model, propertyName, fallbackValue);
    }

    /// <summary>Looks up a string property value from a config model.</summary>
    public Task<string?> GetStringValueAsync<TModel>(string sectionKey, string propertyName, string? fallbackValue = null)
        where TModel : class, new()
    {
        return GetValueAsync<TModel, string>(sectionKey, propertyName, fallbackValue);
    }

    /// <summary>Looks up an enum property value from a config model.</summary>
    public async Task<TEnum?> GetEnumValueAsync<TModel, TEnum>(string sectionKey, string propertyName, TEnum? fallbackValue = default)
        where TModel : class, new()
        where TEnum : struct, Enum
    {
        var model = await LoadAsync<TModel>(sectionKey);
        if (model is null)
        {
            return fallbackValue;
        }

        var property = model.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        if (property is null)
        {
            return fallbackValue;
        }

        var rawValue = property.GetValue(model);
        if (rawValue is TEnum typedEnum)
        {
            return typedEnum;
        }

        if (rawValue is string stringValue && Enum.TryParse<TEnum>(stringValue, true, out var parsedFromString))
        {
            return parsedFromString;
        }

        if (rawValue is not null && Enum.TryParse(rawValue.ToString(), true, out TEnum parsedFromObject))
        {
            return parsedFromObject;
        }

        return fallbackValue;
    }

    /// <summary>Validates that a section exists and can be loaded into a model.</summary>
    public async Task<bool> ValidateSectionAsync<TModel>(string sectionKey) where TModel : class, new()
    {
        var model = await LoadAsync<TModel>(sectionKey);
        return model is not null;
    }

    private static TValue? GetPropertyValue<TValue>(object? model, string propertyName, TValue? fallbackValue)
    {
        if (model is null)
        {
            return fallbackValue;
        }

        var property = model.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        if (property is null)
        {
            return fallbackValue;
        }

        var rawValue = property.GetValue(model);
        if (rawValue is null)
        {
            return fallbackValue;
        }

        if (rawValue is TValue typedValue)
        {
            return typedValue;
        }

        if (typeof(TValue) == typeof(string))
        {
            var stringValue = rawValue switch
            {
                Enum enumValue => enumValue.ToString(),
                _ => rawValue.ToString()
            };

            return (TValue?)(object?)stringValue;
        }

        if (typeof(TValue).IsEnum && rawValue is string enumName && Enum.TryParse(typeof(TValue), enumName, true, out var enumValueFromName))
        {
            return (TValue?)enumValueFromName;
        }

        if (typeof(TValue).IsEnum && rawValue is not null && Enum.TryParse(typeof(TValue), rawValue.ToString(), true, out var parsedEnumValue))
        {
            return (TValue?)parsedEnumValue;
        }

        if (typeof(TValue).IsEnum && rawValue is TValue enumInstance)
        {
            return enumInstance;
        }

        return fallbackValue;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
    }
}
