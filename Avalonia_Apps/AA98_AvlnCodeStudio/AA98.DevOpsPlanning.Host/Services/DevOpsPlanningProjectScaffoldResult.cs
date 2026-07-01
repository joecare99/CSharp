namespace AA98.DevOpsPlanning.Host.Services;

/// <summary>
/// Represents the outcome of a DevOps planning project scaffolding operation.
/// </summary>
public sealed class DevOpsPlanningProjectScaffoldResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DevOpsPlanningProjectScaffoldResult"/> class.
    /// </summary>
    /// <param name="isSuccessful">A value indicating whether scaffolding succeeded.</param>
    /// <param name="message">The user-facing status message.</param>
    /// <param name="projectRootPath">The created or selected project root path.</param>
    /// <param name="projectFilePath">The shared project file path.</param>
    /// <param name="projectItemsFilePath">The shared project items file path.</param>
    public DevOpsPlanningProjectScaffoldResult(
        bool isSuccessful,
        string message,
        string? projectRootPath = null,
        string? projectFilePath = null,
        string? projectItemsFilePath = null)
    {
        IsSuccessful = isSuccessful;
        Message = message;
        ProjectRootPath = projectRootPath;
        ProjectFilePath = projectFilePath;
        ProjectItemsFilePath = projectItemsFilePath;
    }

    /// <summary>
    /// Gets a value indicating whether scaffolding succeeded.
    /// </summary>
    public bool IsSuccessful { get; }

    /// <summary>
    /// Gets the status message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the root path of the planning project.
    /// </summary>
    public string? ProjectRootPath { get; }

    /// <summary>
    /// Gets the path of the created shared project file.
    /// </summary>
    public string? ProjectFilePath { get; }

    /// <summary>
    /// Gets the path of the created project items file.
    /// </summary>
    public string? ProjectItemsFilePath { get; }
}
