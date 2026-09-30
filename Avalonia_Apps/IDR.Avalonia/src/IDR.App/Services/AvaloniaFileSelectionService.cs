using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.App.Services;

public sealed class AvaloniaFileSelectionService : IFileSelectionService
{
    private readonly Func<TopLevel?> _topLevelProvider;

    public AvaloniaFileSelectionService(Func<TopLevel?> topLevelProvider)
    {
        _topLevelProvider = topLevelProvider
            ?? throw new ArgumentNullException(nameof(topLevelProvider));
    }

    public async Task<string?> SelectPeImageAsync(CancellationToken cancellationToken)
    {
        TopLevel topLevel = _topLevelProvider()
            ?? throw new InvalidOperationException("The Avalonia main window is not available.");

        IReadOnlyList<IStorageFile> files = await topLevel.StorageProvider
            .OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false,
                Title = "Open Delphi executable or library"
            })
            .ConfigureAwait(true);

        cancellationToken.ThrowIfCancellationRequested();
        return files.Count == 0 ? null : files[0].TryGetLocalPath();
    }
}
