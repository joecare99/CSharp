using AppKomponentBaseLib.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppKomponentBaseLib.Context;

/// <summary>
/// Provides an immutable shared application-component context snapshot.
/// </summary>
public sealed class AppContextSnapshot : IAppContext
{
    private readonly AppContextTarget[] _targets;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppContextSnapshot"/> class.
    /// </summary>
    /// <param name="activeComponentId">The active component identifier.</param>
    /// <param name="activeComponent">The active component descriptor.</param>
    /// <param name="targets">The current context targets.</param>
    /// <param name="services">The optional service provider.</param>
    public AppContextSnapshot(
        string? activeComponentId = null,
        AppComponentDescriptor? activeComponent = null,
        IEnumerable<AppContextTarget>? targets = null,
        IServiceProvider? services = null)
    {
        var normalizedTargets = targets?.ToArray() ?? Array.Empty<AppContextTarget>();
        if (normalizedTargets.Any(static target => target is null))
        {
            throw new ArgumentException("Targets cannot contain null entries.", nameof(targets));
        }

        ActiveComponentId = string.IsNullOrWhiteSpace(activeComponentId) ? null : activeComponentId.Trim();
        ActiveComponent = activeComponent;
        Services = services;
        _targets = normalizedTargets;
    }

    /// <summary>
    /// Gets the active component identifier when a component owns the current focus.
    /// </summary>
    public string? ActiveComponentId { get; }

    /// <summary>
    /// Gets the active component descriptor when available.
    /// </summary>
    public AppComponentDescriptor? ActiveComponent { get; }

    /// <summary>
    /// Gets the current context targets that should participate in command evaluation.
    /// </summary>
    public IReadOnlyList<AppContextTarget> Targets => _targets;

    /// <summary>
    /// Gets the optional service provider exposed by the current host.
    /// </summary>
    public IServiceProvider? Services { get; }
}
