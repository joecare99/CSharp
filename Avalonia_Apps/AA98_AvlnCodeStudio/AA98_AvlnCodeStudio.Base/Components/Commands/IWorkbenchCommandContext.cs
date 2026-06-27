using AppKomponentBaseLib.Context;
using System;
using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Base.Components.Commands;

/// <summary>
/// Provides the neutral host and session context used for workbench command evaluation and execution.
/// </summary>
public interface IWorkbenchCommandContext
{
    /// <summary>
    /// Gets the active component identifier when a component owns the current focus.
    /// </summary>
    string? ActiveComponentId { get; }

    /// <summary>
    /// Gets the active document identifier when the host exposes one.
    /// </summary>
    string? ActiveDocumentId { get; }

    /// <summary>
    /// Gets the current shared application-context targets.
    /// </summary>
    IReadOnlyList<AppContextTarget> Targets { get; }

    /// <summary>
    /// Gets the optional service provider exposed by the current host.
    /// </summary>
    IServiceProvider? Services { get; }
}
