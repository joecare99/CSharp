using Microsoft.Win32;

namespace TraceAnalysis.Show.Wpf.Services;

/// <summary>
/// Default file-dialog service for the quick trace viewer.
/// </summary>
public sealed class TraceViewerFileDialogService : ITraceViewerFileDialogService
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
}
