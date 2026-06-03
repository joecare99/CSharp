namespace AA98_AvlnCodeStudio.Editor.Services;

/// <summary>
/// Indicates whether a workflow operation completed or was canceled.
/// </summary>
public enum EditorOperationStatus
{
    /// <summary>
    /// The workflow operation completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// The workflow operation was canceled.
    /// </summary>
    Canceled,
}