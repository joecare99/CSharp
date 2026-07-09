using AppKomponentBaseLib.Components;
using System;
using System.Collections.Generic;

namespace AppKomponentBaseLib.Context;

/// <summary>
/// Provides shared application-component context information for context-sensitive interactions.
/// </summary>
public interface IAppContext
{
    /// <summary>
    /// Gets the active component identifier when a component owns the current focus.
    /// </summary>
    string? ActiveComponentId { get; }

    /// <summary>
    /// Gets the active component descriptor when available.
    /// </summary>
    AppComponentDescriptor? ActiveComponent { get; }

    /// <summary>
    /// Gets the current context targets that should participate in command evaluation.
    /// </summary>
    IReadOnlyList<AppContextTarget> Targets { get; }

    /// <summary>
    /// Gets the optional service provider exposed by the current host.
    /// </summary>
    IServiceProvider? Services { get; }
}
