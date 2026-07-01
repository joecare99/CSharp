using AA98_AvlnCodeStudio.Base.Components.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.DevOpsPlanning.Host.Commands;

/// <summary>
/// Extends the generic workbench command context with DevOps planning host path operations.
/// </summary>
public interface IDevOpsPlanningCommandContext : IWorkbenchCommandContext
{
    /// <summary>
    /// Gets the currently selected path value.
    /// </summary>
    string PathSelection { get; }

    /// <summary>
    /// Gets a value indicating whether the selected path points to a planning project root.
    /// </summary>
    bool PathSelectionIsPlanningProject { get; }

    /// <summary>
    /// Updates the current path selection mode.
    /// </summary>
    /// <param name="pathSelection">The selected path.</param>
    /// <param name="pathSelectionIsPlanningProject">When <see langword="true"/>, the path is treated as planning project root.</param>
    void SetPathSelection(string pathSelection, bool pathSelectionIsPlanningProject);

    /// <summary>
    /// Reloads planning data with the current path selection.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that completes when reloading finishes.</returns>
    Task ReloadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a host status message.
    /// </summary>
    /// <param name="statusText">The status message.</param>
    void SetStatus(string statusText);
}
