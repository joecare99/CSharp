using TraceAnalysis.Workbench.Wpf.ViewModels;

namespace TraceAnalysis.Workbench.Wpf.Services;

/// <summary>
/// Builds and updates the workbench main menu.
/// </summary>
public interface IWorkbenchMenuService
{
    /// <summary>
    /// Creates the menu model for the shell.
    /// </summary>
    MainMenuViewModel CreateMenu(MainWorkbenchViewModel shellViewModel);

    /// <summary>
    /// Applies context-sensitive menu behavior.
    /// </summary>
    void ApplyContext(MainMenuViewModel menu, WorkbenchContextKind contextKind);
}
