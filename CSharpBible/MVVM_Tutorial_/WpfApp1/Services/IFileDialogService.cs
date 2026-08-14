namespace WpfApp.Services;

/// <summary>
/// Provides file dialog interaction for opening and saving text documents.
/// </summary>
public interface IFileDialogService
{
    /// <summary>
    /// Shows the open-file dialog and returns the selected file path.
    /// </summary>
    string? PickOpenFilePath();

    /// <summary>
    /// Shows the save-file dialog and returns the selected file path.
    /// </summary>
    /// <param name="initialFilePath">The initial file path, if any.</param>
    string? PickSaveFilePath(string? initialFilePath);
}
