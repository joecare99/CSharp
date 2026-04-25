namespace TraceAnalysis.Workbench.Wpf.Services;

/// <summary>
/// Provides simple file-dialog interactions for the workbench UI.
/// </summary>
public interface IFileDialogService
{
    /// <summary>
    /// Gets a trace file path to open, or <c>null</c> when the user canceled.
    /// </summary>
    string? ShowOpenTraceFileDialog();

    /// <summary>
    /// Gets a file path to open, or <c>null</c> when the user canceled.
    /// </summary>
    string? ShowOpenFileDialog();

    /// <summary>
    /// Gets a file path to save, or <c>null</c> when the user canceled.
    /// </summary>
    string? ShowSaveFileDialog(string? currentFilePath);
}
