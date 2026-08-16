using System;

namespace Config.Service;

/// <summary>
/// Wraps a section provider and allows the UI/application layer to provide a localized description
/// for the section without modifying the provider's core implementation.
/// </summary>
public sealed class ConfigSectionRegistration
{
    public ConfigSectionRegistration(IConfigSectionProvider provider, string? description = null)
    {
        Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        Description = string.IsNullOrWhiteSpace(description) ? provider.Description : description;
    }

    public IConfigSectionProvider Provider { get; }

    public string Name => Provider.Name;

    public string DisplayName => Provider.DisplayName;

    public string? Description { get; }

    public int Order => Provider.Order;

    public Type ModelType => Provider.ModelType;

    public object CreateModel() => Provider.CreateModel();
}
