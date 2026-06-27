using System;
using System.Collections.Generic;
using System.Linq;

namespace AppKomponentBaseLib.Components;

/// <summary>
/// Describes a reusable application component that can participate in host composition.
/// </summary>
public sealed class AppComponentDescriptor
{
    private readonly string[] _capabilities;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppComponentDescriptor"/> class.
    /// </summary>
    /// <param name="componentId">The stable component identifier.</param>
    /// <param name="displayName">The host-facing display name.</param>
    /// <param name="capabilities">The optional capability identifiers exposed by the component.</param>
    /// <param name="description">The optional component description.</param>
    public AppComponentDescriptor(
        string componentId,
        string displayName,
        IEnumerable<string>? capabilities = null,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(componentId))
        {
            throw new ArgumentException("A stable component identifier is required.", nameof(componentId));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("A display name is required.", nameof(displayName));
        }

        var normalizedCapabilities = capabilities?
            .Where(static capability => !string.IsNullOrWhiteSpace(capability))
            .Select(static capability => capability.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray() ?? Array.Empty<string>();

        ComponentId = componentId;
        DisplayName = displayName;
        Description = description;
        _capabilities = normalizedCapabilities;
    }

    /// <summary>
    /// Gets the stable component identifier.
    /// </summary>
    public string ComponentId { get; }

    /// <summary>
    /// Gets the host-facing display name.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets the optional component description.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the capability identifiers exposed by the component.
    /// </summary>
    public IReadOnlyList<string> Capabilities => _capabilities;
}
