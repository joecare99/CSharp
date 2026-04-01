namespace RepoMigrator.App.Wpf.ViewModels;

/// <summary>
/// Describes the currently active workflow stage of the migration UI.
/// </summary>
public enum WorkflowStage
{
    Source,
    Target,
    Options,
    Execution
}
