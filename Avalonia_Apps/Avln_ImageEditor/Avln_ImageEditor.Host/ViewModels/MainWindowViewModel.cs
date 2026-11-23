using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avln_ImageEditor.Controls.Models;
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
    public ImageEditorViewModel Editor { get; } = new() { AllowClear = false };

    /// <summary>
    /// Opens an image file through the host-provided storage provider.
    /// </summary>
    /// <param name="storageProvider">The Avalonia storage provider for the current host.</param>
    public Task OpenImageAsync(IStorageProvider storageProvider) =>
        OpenImageAsync(storageProvider, () => Task.FromResult(false));

    /// <summary>Opens an image after confirming any unsaved changes.</summary>
    public async Task OpenImageAsync(IStorageProvider storageProvider, Func<Task<bool>> confirmDiscard)
    {
        ArgumentNullException.ThrowIfNull(storageProvider);
        ArgumentNullException.ThrowIfNull(confirmDiscard);

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
        byte[] imageBytes = await ReadImageBytesAsync(stream);
        if (Editor.IsDirty && !await confirmDiscard())
        {
            StatusText = "Open canceled; edits retained.";
            return;
        }
        Editor.LoadDocument(imageBytes, selectedFile.Name);
        StatusText = $"Loaded {Editor.DocumentSummary}";
    }

    internal static async Task<byte[]> ReadImageBytesAsync(Stream stream)
    {
        if (stream.CanSeek && stream.Length - stream.Position > ImageDecodeLimits.MaxEncodedBytes)
            throw new InvalidDataException("Image exceeds the 32 MiB encoded limit.");
        using var memoryStream = new MemoryStream();
        var buffer = new byte[81920];
        int read;
        while ((read = await stream.ReadAsync(buffer, 0,
            (int)Math.Min(buffer.Length, ImageDecodeLimits.MaxEncodedBytes - memoryStream.Length + 1))) != 0)
        {
            if (memoryStream.Length + read > ImageDecodeLimits.MaxEncodedBytes)
                throw new InvalidDataException("Image exceeds the 32 MiB encoded limit.");
            memoryStream.Write(buffer, 0, read);
        }
        return memoryStream.ToArray();
    }

    /// <summary>Exports to a new local PNG without overwriting an existing file.</summary>
    public async Task ExportPngAsync(IStorageProvider storageProvider)
    {
        ArgumentNullException.ThrowIfNull(storageProvider);
        if (!Editor.HasDocument) { StatusText = "No image loaded."; return; }
        if (!storageProvider.CanSave) { StatusText = "This host cannot save files."; return; }
        var selectedFile = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export PNG to a new file (existing files are not overwritten)",
            DefaultExtension = "png",
            FileTypeChoices = new[] { PngFileType }
        });
        if (selectedFile is null) { StatusText = "Export canceled."; return; }
        string? path = selectedFile.TryGetLocalPath();
        if (path is null) throw new NotSupportedException("Export requires a local file path.");
        await ExportPngToNewFileAsync(path);
    }

    /// <summary>Writes a snapshot to a temporary sibling and moves it to a new destination.</summary>
    public async Task ExportPngToNewFileAsync(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (!string.Equals(Path.GetExtension(path), ".png", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Choose a .png filename.");
        var snapshot = Editor.CapturePngSnapshot();
        var bytes = snapshot.Bytes;
        await PngExportFile.WriteNewAsync(path, (stream, cancellationToken) =>
            stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken));
        Editor.MarkSaved(snapshot);
        Editor.RenameDocument(snapshot, Path.GetFileName(path));
        StatusText = $"Exported PNG to {path}. PNG re-encoding may discard metadata.";
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Editor.Dispose();
    }
}
