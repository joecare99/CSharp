using AA98.DevOpsPlanning.Host.Services;
using AA98_AvlnCodeStudio.Base.Components.Commands;
using AA98_AvlnCodeStudio.Planning.Local.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.DevOpsPlanning.Host.Commands;

/// <summary>
/// Creates a new DevOps planning project structure in a selected folder.
/// </summary>
public sealed class NewPlanningProjectCommandContribution : IWorkbenchCommandContribution
{
    /// <inheritdoc/>
    public WorkbenchCommandDescriptor Descriptor { get; } = new(
        commandId: "Planning.File.NewPlanningProject",
        displayTitle: "Neues DevOps-Projekt...",
        placements: new[]
        {
            new WorkbenchCommandPlacement(WorkbenchCommandSurface.Menu, "Datei", 30),
            new WorkbenchCommandPlacement(WorkbenchCommandSurface.Toolbar, "Datei", 30),
        },
        description: "Legt ein neues DevOps-Planungsprojekt mit Standardordnern an.");

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
        ILocalPlanningProjectScaffolder? scaffolder = context.Services?.GetService<ILocalPlanningProjectScaffolder>();
        if (folderPicker is null || scaffolder is null)
        {
            planningContext.SetStatus("Required project creation services are not available.");
            return;
        }

        string? selectedFolder = await folderPicker.PickFolderAsync("Ordner für neues DevOps-Projekt wählen", cancellationToken);
        if (string.IsNullOrWhiteSpace(selectedFolder))
        {
            planningContext.SetStatus("Create planning project canceled.");
            return;
        }

        LocalPlanningProjectScaffoldResult scaffoldResult = scaffolder.Create(selectedFolder);
        planningContext.SetStatus(scaffoldResult.Message);
        if (!scaffoldResult.IsSuccessful)
        {
            return;
        }

        planningContext.SetPathSelection(selectedFolder, pathSelectionIsPlanningProject: true);
        await planningContext.ReloadAsync(cancellationToken);
    }
}
