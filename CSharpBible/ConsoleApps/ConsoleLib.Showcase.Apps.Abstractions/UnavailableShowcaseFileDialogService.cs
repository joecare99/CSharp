using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleLib.Showcase.Apps;

/// <summary>Reports unavailable host functionality without pretending a selection succeeded.</summary>
public sealed class UnavailableShowcaseFileDialogService : IShowcaseFileDialogService
{
    private readonly string _message;

    public UnavailableShowcaseFileDialogService(string message = "File dialogs are not available on this host.")
        => _message = string.IsNullOrWhiteSpace(message) ? throw new ArgumentException("A message is required.", nameof(message)) : message;

    public Task<FileDialogResult> OpenFileAsync(FileDialogRequest request, CancellationToken cancellationToken = default) =>
        UnavailableAsync(cancellationToken);
    public Task<FileDialogResult> SaveFileAsync(FileDialogRequest request, CancellationToken cancellationToken = default) =>
        UnavailableAsync(cancellationToken);
    public Task<FileDialogResult> PickFolderAsync(FileDialogRequest request, CancellationToken cancellationToken = default) =>
        UnavailableAsync(cancellationToken);

    private Task<FileDialogResult> UnavailableAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(FileDialogResult.Unavailable(_message));
    }
}
