using AA98.DevOpsPlanning.Host.Services;
using AA98_AvlnCodeStudio.Base.Components.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.DevOpsPlanning.Host.Commands;

/// <summary>
/// Opens a solution root path and reloads planning data from its <c>DevOps</c> folder.
/// </summary>
public sealed class OpenSolutionRootCommandContribution : IWorkbenchCommandContribution
{
    /// <inheritdoc/>
    public WorkbenchCommandDescriptor Descriptor { get; } = new(
        commandId: "Planning.File.OpenSolutionRoot",
        displayTitle: "Solution-Root öffnen...",
        placements: new[]
        {
            new WorkbenchCommandPlacement(WorkbenchCommandSurface.Menu, "Datei", 10),
            new WorkbenchCommandPlacement(WorkbenchCommandSurface.Toolbar, "Datei", 10),
        },
        description: "Wählt einen Solution-Root-Ordner und lädt daraus den DevOps-Planungsbaum.");

    /// <inheritdoc/>
    public bool CanExecute(IWorkbenchCommandContext context)
    {
        return context is IDevOpsPlanningCommandContext;
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync(IWorkbenchCommandContext context, CancellationToken cancellationToken = default)
    {
        if (context is not IDevOpsPlanningCommandContext planningContext)
        {
            return;
        }

        IDevOpsFolderPickerService? folderPicker = context.Services?.GetService<IDevOpsFolderPickerService>();
        if (folderPicker is null)
        {
            planningContext.SetStatus("Folder picker service is not available.");
            return;
        }

        string? selectedFolder = await folderPicker.PickFolderAsync("Solution-Root wählen", cancellationToken);
        if (string.IsNullOrWhiteSpace(selectedFolder))
        {
            planningContext.SetStatus("Open solution root canceled.");
            return;
        }

        planningContext.SetPathSelection(selectedFolder, pathSelectionIsPlanningProject: false);
        await planningContext.ReloadAsync(cancellationToken);
    }
}
