using System;

namespace Config.Service;

/// <summary>
/// Describes one configuration section contributed by an application component.
/// Implementations stay UI-agnostic and OS-agnostic; the <see cref="Name"/> is
/// the stable storage key used by the configuration store, while
/// <see cref="DisplayName"/> and <see cref="Description"/> are localized by the
/// contributing component for human-facing configuration UIs.
/// </summary>
public interface IConfigSectionProvider
{
    /// <summary>Stable section key used as the JSON storage key, for example "RnzTrauer.Database".</summary>
    string Name { get; }

    /// <summary>Display name shown by configuration UIs; localized by the component.</summary>
    string DisplayName { get; }

    /// <summary>Optional explanation shown by configuration UIs; localized by the component.</summary>
    string? Description { get; }

    /// <summary>Sort order within a configuration UI. Lower values appear first.</summary>
    int Order { get; }

    /// <summary>Closed model type whose public, supported instance properties are persisted.</summary>
    Type ModelType { get; }

    /// <summary>
    /// Creates the default model instance. Missing or removed values in a stored
    /// document fall back to the values of this instance.
    /// </summary>
    object CreateModel();
}
