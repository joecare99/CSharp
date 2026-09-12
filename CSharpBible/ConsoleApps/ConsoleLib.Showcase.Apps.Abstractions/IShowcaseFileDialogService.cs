using System.Threading;
using System.Threading.Tasks;

namespace ConsoleLib.Showcase.Apps;

/// <summary>Host-provided file and folder selection capability.</summary>
public interface IShowcaseFileDialogService
{
    Task<FileDialogResult> OpenFileAsync(FileDialogRequest request, CancellationToken cancellationToken = default);
    Task<FileDialogResult> SaveFileAsync(FileDialogRequest request, CancellationToken cancellationToken = default);
    Task<FileDialogResult> PickFolderAsync(FileDialogRequest request, CancellationToken cancellationToken = default);
}
