using Microsoft.Win32;

namespace TraceAnalysis.Workbench.Wpf.Services;

/// <summary>
/// Default file-dialog service for the WPF workbench baseline.
/// </summary>
public sealed class FileDialogService : IFileDialogService
{
    /// <inheritdoc/>
    public string? ShowOpenTraceFileDialog()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Trace files (*.csv;*.json;*.txt;*.trace)|*.csv;*.json;*.txt;*.trace|All files (*.*)|*.*"
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    /// <inheritdoc/>
    public string? ShowOpenFileDialog()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Processing configuration (*.json)|*.json|All files (*.*)|*.*"
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    /// <inheritdoc/>
    public string? ShowSaveFileDialog(string? currentFilePath)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "Processing configuration (*.json)|*.json|All files (*.*)|*.*",
            FileName = currentFilePath
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
