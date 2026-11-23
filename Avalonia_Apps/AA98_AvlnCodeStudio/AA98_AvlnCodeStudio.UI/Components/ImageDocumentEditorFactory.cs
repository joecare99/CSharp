using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.UI.Resources;
using AA98_AvlnCodeStudio.UI.ViewModels;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avln_ImageEditor.Controls.ViewModels;
using Avln_ImageEditor.Controls.Views;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Components;

internal sealed class ImageDocumentEditorFactory(
    Func<TopLevel?> topLevelProvider,
    Func<CancellationToken, Task<string?>>? selectDestination = null)
    : IDocumentEditorFactory<EditorTabViewModel>
{
    public string Id => "image";

    public int Priority => 100;

    public bool CanOpen(string filePath)
    {
        string extension = Path.GetExtension(filePath);
        return extension.Equals(".png", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase);
    }

    public EditorTabViewModel Create(string filePath)
    {
        ImageEditorViewModel image = new() { AllowClear = false };
        ImageDocumentSession session = new(image);
        ImageEditorControl view = new() { DataContext = image };
        return new EditorTabViewModel(session, view, token => SelectDestinationAsync(session, token), Id);
    }

    private async Task<string?> SelectDestinationAsync(ImageDocumentSession session, CancellationToken cancellationToken)
    {
        string? path = selectDestination is null
            ? await SelectSavePathAsync(session, cancellationToken)
            : await selectDestination(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        if (path is not null && (File.Exists(path) || Directory.Exists(path)))
        {
            throw new IOException(UiStrings.ImageSaveExistingText);
        }

        return path;
    }

    private async Task<string?> SelectSavePathAsync(ImageDocumentSession session, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        TopLevel topLevel = topLevelProvider()
            ?? throw new InvalidOperationException(UiStrings.ImageSaveUnavailableText);
        string name = Path.GetFileNameWithoutExtension(session.FilePath) ?? "Image";
        IStorageFile? file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = UiStrings.ImageSavePickerTitle,
            SuggestedFileName = $"{name}-edited.png",
            DefaultExtension = "png",
            FileTypeChoices = [new FilePickerFileType(UiStrings.ImageSavePickerFilter) { Patterns = ["*.png"] }],
        });
        cancellationToken.ThrowIfCancellationRequested();
        if (file is null)
        {
            return null;
        }

        return file.TryGetLocalPath() ?? throw new InvalidOperationException(UiStrings.ImageSaveLocalOnlyText);
    }
}
