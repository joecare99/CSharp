using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.Model.Workspaces;
using Avln_ImageEditor.Controls.ViewModels;
using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Components;

internal sealed class ImageDocumentSession : IDocumentEditorSession, IDocumentSavePathPolicy
{
    private readonly ImageEditorViewModel _image;
    private readonly Func<string, byte[], byte[]?, CancellationToken, Task> _writeAsync;
    private readonly SemaphoreSlim _fileOperation = new(1, 1);
    private byte[]? _ownedOutputHash;
    private bool _disposed;

    public ImageDocumentSession(
        ImageEditorViewModel image,
        Func<string, byte[], byte[]?, CancellationToken, Task>? writeAsync = null)
    {
        _image = image ?? throw new ArgumentNullException(nameof(image));
        _writeAsync = writeAsync ?? ImageDocumentFileStorage.WriteAsync;
        _image.ContentChanging += OnContentChanging;
        _image.PropertyChanged += OnImagePropertyChanged;
    }

    public string? FilePath { get; private set; }

    public string DisplayName => Path.GetFileName(FilePath) ?? "Image";

    public bool IsDirty => _image.IsDirty;

    public bool RequiresSaveAs => _ownedOutputHash is null;

    public event EventHandler? ContentChanging;

    public event EventHandler? StateChanged;

    public async Task OpenAsync(string filePath, CancellationToken cancellationToken = default)
    {
        await _fileOperation.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_image.IsDirty)
            {
                throw new InvalidOperationException("Save or discard the current image before opening another.");
            }

            string path = WorkspacePathIdentity.Normalize(filePath);
            byte[] encoded = await ImageDocumentFileStorage.ReadAsync(path, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_image.IsDirty)
            {
                throw new InvalidOperationException("The image changed while another image was opening.");
            }

            try
            {
                _image.LoadDocument(encoded, Path.GetFileName(path));
            }
            catch (InvalidDataException)
            {
                throw;
            }
            catch (Exception exception) when (exception is IOException or ArgumentException or InvalidOperationException or NotSupportedException)
            {
                throw new InvalidDataException("The image could not be decoded safely.", exception);
            }

            FilePath = path;
            _ownedOutputHash = null;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            _fileOperation.Release();
        }
    }

    public async Task SaveAsync(string filePath, CancellationToken cancellationToken = default)
    {
        await _fileOperation.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            string path = WorkspacePathIdentity.Normalize(filePath);
            if (!string.Equals(Path.GetExtension(path), ".png", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Images can only be saved to a PNG file.");
            }

            ImageEditorSaveSnapshot snapshot = _image.CapturePngSnapshot();
            byte[] bytes = (byte[])snapshot.Bytes.Clone();
            byte[]? expectedHash = FilePath is not null && WorkspacePathIdentity.AreEqual(path, FilePath)
                ? _ownedOutputHash
                : null;
            await _writeAsync(path, bytes, expectedHash, cancellationToken);
            if (_disposed || !_image.IsCurrentDocument(snapshot))
            {
                throw new InvalidOperationException(
                    "The image was saved, but the open document changed; its path and saved state were not updated.");
            }

            FilePath = path;
            _ownedOutputHash = SHA256.HashData(bytes);
            _image.MarkSaved(snapshot);
            _image.RenameDocument(snapshot, Path.GetFileName(path));
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            _fileOperation.Release();
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _image.ContentChanging -= OnContentChanging;
        _image.PropertyChanged -= OnImagePropertyChanged;
        _image.Dispose();
    }

    private void OnContentChanging(object? sender, EventArgs e) => ContentChanging?.Invoke(this, e);

    private void OnImagePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is null or nameof(ImageEditorViewModel.IsDirty))
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
