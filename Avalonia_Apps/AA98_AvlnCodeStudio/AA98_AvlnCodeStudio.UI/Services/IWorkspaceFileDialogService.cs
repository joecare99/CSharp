using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Services;

/// <summary>
/// Provides a dialog for selecting a C# workspace entry file.
/// </summary>
public interface IWorkspaceFileDialogService
{
    /// <summary>
    /// Shows a dialog for selecting a solution or C# project.
    /// </summary>
    Task<string?> ShowOpenWorkspaceDialogAsync(string? initialDirectory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows a dialog for choosing the destination of a new C# project.
    /// </summary>
    Task<string?> ShowSaveCSharpProjectDialogAsync(
        string? initialDirectory,
        string? initialFileName,
        CancellationToken cancellationToken = default);
}
