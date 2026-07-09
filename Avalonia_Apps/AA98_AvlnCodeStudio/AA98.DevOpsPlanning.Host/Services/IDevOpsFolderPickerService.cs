using System.Threading;
using System.Threading.Tasks;

namespace AA98.DevOpsPlanning.Host.Services;

/// <summary>
/// Provides folder selection dialogs for the DevOps planning host.
/// </summary>
public interface IDevOpsFolderPickerService
{
    /// <summary>
    /// Opens a folder selection dialog.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The selected local folder path, or <see langword="null"/> when canceled.</returns>
    Task<string?> PickFolderAsync(string title, CancellationToken cancellationToken = default);
}
