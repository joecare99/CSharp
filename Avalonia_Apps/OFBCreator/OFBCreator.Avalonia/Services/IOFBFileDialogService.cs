using System.Threading.Tasks;

namespace OFBCreator.Avalonia.Services;

/// <summary>
/// Provides native file and folder selection for the Avalonia workspace.
/// </summary>
public interface IOFBFileDialogService
{
    /// <summary>Prompts the user to select an existing portable project file.</summary>
    /// <param name="initialPath">A project path or folder used to choose the initial location.</param>
    /// <returns>The selected project path, or <see langword="null"/> when the dialog is canceled.</returns>
    Task<string?> OpenProjectAsync(string? initialPath);

    /// <summary>Prompts the user to choose a destination for a portable project file.</summary>
    /// <param name="suggestedPath">The default project path displayed by the save dialog.</param>
    /// <returns>The selected destination path, or <see langword="null"/> when canceled.</returns>
    Task<string?> SaveProjectAsAsync(string suggestedPath);

    /// <summary>Prompts the user to select a directory.</summary>
    /// <param name="initialPath">A file or directory path used to choose the initial location.</param>
    /// <returns>The selected directory path, or <see langword="null"/> when canceled.</returns>
    Task<string?> PickDirectoryAsync(string? initialPath);

    /// <summary>Prompts the user to select a GEDCOM input file.</summary>
    /// <param name="initialPath">A GEDCOM path or folder used to choose the initial location.</param>
    /// <returns>The selected GEDCOM path, or <see langword="null"/> when canceled.</returns>
    Task<string?> OpenGedcomAsync(string? initialPath);

    /// <summary>Prompts the user to choose a DOCX output destination.</summary>
    /// <param name="suggestedPath">The default DOCX path displayed by the save dialog.</param>
    /// <returns>The selected DOCX destination path, or <see langword="null"/> when canceled.</returns>
    Task<string?> SaveDocxAsAsync(string suggestedPath);
}
