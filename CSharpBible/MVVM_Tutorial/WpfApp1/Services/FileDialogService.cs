using System.IO;
using Microsoft.Win32;

namespace WpfApp.Services;

/// <summary>
/// Implements WPF file dialogs for text document operations.
/// </summary>
public sealed class FileDialogService : IFileDialogService
{
    /// <inheritdoc />
    public string? PickOpenFilePath()
    {
        var oDialog = new OpenFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            Multiselect = false
        };

        return oDialog.ShowDialog() == true ? oDialog.FileName : null;
    }

    /// <inheritdoc />
    public string? PickSaveFilePath(string? initialFilePath)
    {
        var oDialog = new SaveFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            FileName = string.IsNullOrWhiteSpace(initialFilePath) ? "Document.txt" : Path.GetFileName(initialFilePath)
        };

        if (!string.IsNullOrWhiteSpace(initialFilePath))
        {
            oDialog.InitialDirectory = Path.GetDirectoryName(initialFilePath);
        }

        return oDialog.ShowDialog() == true ? oDialog.FileName : null;
    }
}
