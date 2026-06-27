using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avln_ImageEditor.Controls.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avln_ImageEditor.Host.ViewModels;

/// <summary>
/// Coordinates the desktop host shell around the reusable image editor control.
/// </summary>
public sealed partial class MainWindowViewModel : ObservableObject, IDisposable
{
    private static readonly FilePickerFileType PngFileType = new("PNG image")
    {
        Patterns = new[] { "*.png" },
        MimeTypes = new[] { "image/png" }
    };

    private static readonly FilePickerFileType JpegFileType = new("JPEG image")
    {
        Patterns = new[] { "*.jpg", "*.jpeg" },
        MimeTypes = new[] { "image/jpeg" }
    };

    private static readonly FilePickerFileType BitmapFileType = new("Bitmap image")
    {
        Patterns = new[] { "*.bmp" },
        MimeTypes = new[] { "image/bmp" }
    };

    private static readonly IReadOnlyList<FilePickerFileType> ImageFileTypes = new[]
    {
        PngFileType,
        JpegFileType,
        BitmapFileType
    };

    [ObservableProperty]
    private string _statusText = "Open an image to test the control.";

    /// <summary>
    /// Gets the window title.
    /// </summary>
    public string Title => "Image Editor Host";

    /// <summary>
    /// Gets the reusable image editor ViewModel.
    /// </summary>
    public ImageEditorViewModel Editor { get; } = new();

    /// <summary>
    /// Opens an image file through the host-provided storage provider.
    /// </summary>
    /// <param name="storageProvider">The Avalonia storage provider for the current host.</param>
    public async Task OpenImageAsync(IStorageProvider storageProvider)
    {
        ArgumentNullException.ThrowIfNull(storageProvider);

        if (!storageProvider.CanOpen)
        {
            StatusText = "The current host cannot open files.";
            return;
        }

        var selectedFiles = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open image",
            AllowMultiple = false,
            FileTypeFilter = ImageFileTypes
        });

        if (selectedFiles.Count == 0)
        {
            StatusText = "Open image canceled.";
            return;
        }

        var selectedFile = selectedFiles[0];
        await using var stream = await selectedFile.OpenReadAsync();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);

        Editor.LoadDocument(memoryStream.ToArray(), selectedFile.Name);
        StatusText = $"Loaded {Editor.DocumentSummary}";
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Editor.Dispose();
    }
}
