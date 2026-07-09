using AA98.DevOpsPlanning.Host.Services;
using AA98_AvlnCodeStudio.Base.Components.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.DevOpsPlanning.Host.Commands;

/// <summary>
/// Opens a planning project root and reloads planning data from that path.
/// </summary>
public sealed class OpenPlanningProjectCommandContribution : IWorkbenchCommandContribution
{
    /// <inheritdoc/>
    public WorkbenchCommandDescriptor Descriptor { get; } = new(
        commandId: "Planning.File.OpenPlanningProject",
        displayTitle: "DevOps-Projekt öffnen...",
        placements: new[]
        {
            new WorkbenchCommandPlacement(WorkbenchCommandSurface.Menu, "Datei", 20),
            new WorkbenchCommandPlacement(WorkbenchCommandSurface.Toolbar, "Datei", 20),
        },
        description: "Wählt einen DevOps-Planungsprojektordner und lädt dessen Inhalte.");

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

        string? selectedFolder = await folderPicker.PickFolderAsync("DevOps-Projektordner wählen", cancellationToken);
        if (string.IsNullOrWhiteSpace(selectedFolder))
        {
            planningContext.SetStatus("Open planning project canceled.");
            return;
        }

        planningContext.SetPathSelection(selectedFolder, pathSelectionIsPlanningProject: true);
        await planningContext.ReloadAsync(cancellationToken);
    }
}
