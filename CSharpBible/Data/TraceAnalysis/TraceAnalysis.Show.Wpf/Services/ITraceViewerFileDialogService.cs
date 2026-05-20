namespace TraceAnalysis.Show.Wpf.Services;

/// <summary>
/// Provides simple file-dialog interactions for the quick trace viewer.
/// </summary>
public interface ITraceViewerFileDialogService
{
    /// <summary>
    /// Gets a trace file path to open, or <c>null</c> when the user canceled.
    /// </summary>
    string? ShowOpenTraceFileDialog();
}
