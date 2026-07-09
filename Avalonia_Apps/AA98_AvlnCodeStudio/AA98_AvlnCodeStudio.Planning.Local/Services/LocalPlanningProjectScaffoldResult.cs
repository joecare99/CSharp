namespace AA98_AvlnCodeStudio.Planning.Local.Services;

/// <summary>
/// Represents the outcome of scaffolding a local planning project.
/// </summary>
public sealed class LocalPlanningProjectScaffoldResult
{
    public LocalPlanningProjectScaffoldResult(bool isSuccessful, string message, string? projectRootPath = null, string? projectFilePath = null, string? projectItemsFilePath = null)
    {
        IsSuccessful = isSuccessful;
        Message = message;
        ProjectRootPath = projectRootPath;
        ProjectFilePath = projectFilePath;
        ProjectItemsFilePath = projectItemsFilePath;
    }

    public bool IsSuccessful { get; }

    public string Message { get; }

    public string? ProjectRootPath { get; }

    public string? ProjectFilePath { get; }

    public string? ProjectItemsFilePath { get; }
}
