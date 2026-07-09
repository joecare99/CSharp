using AA98_AvlnCodeStudio.Base.Components.Commands;
using AppKomponentBaseLib.Context;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.DevOpsPlanning.Host.Commands;

/// <summary>
/// Represents the mutable workbench command context used by the DevOps planning host.
/// </summary>
public sealed class DevOpsPlanningCommandContext : IDevOpsPlanningCommandContext
{
    private readonly Func<string> _getPathSelection;
    private readonly Func<bool> _getPathSelectionIsPlanningProject;
    private readonly Action<string, bool> _setPathSelection;
    private readonly Func<CancellationToken, Task> _reloadAsync;
    private readonly Action<string> _setStatus;

    /// <summary>
    /// Initializes a new instance of the <see cref="DevOpsPlanningCommandContext"/> class.
    /// </summary>
    /// <param name="services">The optional host service provider.</param>
    /// <param name="getPathSelection">The path selection getter.</param>
    /// <param name="getPathSelectionIsPlanningProject">The path mode getter.</param>
    /// <param name="setPathSelection">The path selection updater.</param>
    /// <param name="reloadAsync">The reload callback.</param>
    /// <param name="setStatus">The status callback.</param>
    public DevOpsPlanningCommandContext(
        IServiceProvider? services,
        Func<string> getPathSelection,
        Func<bool> getPathSelectionIsPlanningProject,
        Action<string, bool> setPathSelection,
        Func<CancellationToken, Task> reloadAsync,
        Action<string> setStatus)
    {
        Services = services;
        _getPathSelection = getPathSelection ?? throw new ArgumentNullException(nameof(getPathSelection));
        _getPathSelectionIsPlanningProject = getPathSelectionIsPlanningProject ?? throw new ArgumentNullException(nameof(getPathSelectionIsPlanningProject));
        _setPathSelection = setPathSelection ?? throw new ArgumentNullException(nameof(setPathSelection));
        _reloadAsync = reloadAsync ?? throw new ArgumentNullException(nameof(reloadAsync));
        _setStatus = setStatus ?? throw new ArgumentNullException(nameof(setStatus));
    }

    /// <inheritdoc/>
    public string? ActiveComponentId => "AA98.DevOpsPlanning.Host";

    /// <inheritdoc/>
    public string? ActiveDocumentId => null;

    /// <inheritdoc/>
    public IReadOnlyList<AppContextTarget> Targets { get; } = [];

    /// <inheritdoc/>
    public IServiceProvider? Services { get; }

    /// <inheritdoc/>
    public string PathSelection => _getPathSelection();

    /// <inheritdoc/>
    public bool PathSelectionIsPlanningProject => _getPathSelectionIsPlanningProject();

    /// <inheritdoc/>
    public void SetPathSelection(string pathSelection, bool pathSelectionIsPlanningProject)
    {
        _setPathSelection(pathSelection, pathSelectionIsPlanningProject);
    }

    /// <inheritdoc/>
    public Task ReloadAsync(CancellationToken cancellationToken = default)
    {
        return _reloadAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public void SetStatus(string statusText)
    {
        _setStatus(statusText);
    }
}
