using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avln_ImageEditor.Controls.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avln_ImageEditor.Controls.ViewModels;

/// <summary>Coordinates the host-independent image editor state. Use on the UI thread.</summary>
public sealed partial class ImageEditorViewModel : ViewModelBase, IDisposable
{
    private const int MaxHistoryStates = 9;
    private readonly List<ImageEditorDocument> _history = new();
    private int _index;
    private object _documentIdentity = new();
    private ImageEditorDocument? _savedRevision;
    private ImageEditorDocument? _activeDocument;

    /// <summary>Raised immediately before a successful pixel mutation, including undo and redo.</summary>
    public event EventHandler? ContentChanging;

    public ImageEditorDocument? ActiveDocument
    {
        get => _activeDocument;
        set
        {
            if (ReferenceEquals(value, _activeDocument)) return;
            if (value is null) ClearDocument();
            else LoadDocument(value);
        }
    }

    private void SetActiveDocument(ImageEditorDocument? value)
    {
        if (SetProperty(ref _activeDocument, value, nameof(ActiveDocument)))
        {
            OnPropertyChanged(nameof(HasDocument));
            OnPropertyChanged(nameof(DocumentSummary));
        }
    }

    [ObservableProperty]
    private ImageEditorTool _selectedTool = ImageEditorTool.Select;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ZoomFactor))]
    [NotifyPropertyChangedFor(nameof(ZoomSummary))]
    private double _zoomPercentage = 100.0;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ClearDocumentCommand))]
    private bool _allowClear = true;

    public bool HasDocument => ActiveDocument is not null;
    public bool IsDirty => ActiveDocument is not null && !ReferenceEquals(ActiveDocument, _savedRevision);
    public double ZoomFactor => ZoomPercentage / 100.0;
    public string ZoomSummary => $"{ZoomPercentage:0}%";
    public string DocumentSummary => ActiveDocument?.Summary ?? "No image loaded";

    public void LoadDocument(byte[] imageBytes, string documentName)
    {
        ArgumentNullException.ThrowIfNull(imageBytes);
        ArgumentException.ThrowIfNullOrWhiteSpace(documentName);
        ImageDecodeLimits.ValidateEncoded(imageBytes);
        using var stream = new MemoryStream(imageBytes, writable: false);
        var bitmap = new Bitmap(stream);
        try { LoadDocument(new ImageEditorDocument(documentName, bitmap)); }
        catch { bitmap.Dispose(); throw; }
    }

    /// <summary>Transfers ownership of the supplied document to this editor on success.</summary>
    public void LoadDocument(ImageEditorDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        if (_history.Contains(document))
            throw new ArgumentException("The document is already owned by this editor.", nameof(document));
        ImageDecodeLimits.ValidateSize(document.PixelSize.Width, document.PixelSize.Height);
        var old = _history.ToArray();
        _history.Clear();
        _history.Add(document);
        _index = 0;
        _documentIdentity = new object();
        _savedRevision = document;
        SetActiveDocument(document);
        ZoomPercentage = 100;
        RefreshState();
        foreach (var item in old) item.Dispose();
    }

    /// <summary>Always encodes the displayed pixels to PNG, even if the image is unmodified.</summary>
    public ImageEditorSaveSnapshot CapturePngSnapshot()
    {
        var document = ActiveDocument ?? throw new InvalidOperationException("No image is loaded.");
        using var stream = new MemoryStream();
        document.Bitmap.Save(stream, new PngBitmapEncoderOptions());
        return new ImageEditorSaveSnapshot(stream.ToArray(), _documentIdentity, document);
    }

    /// <summary>Returns whether the snapshot belongs to the currently loaded document, irrespective of later edits.</summary>
    public bool IsCurrentDocument(ImageEditorSaveSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        return HasDocument && ReferenceEquals(snapshot.Document, _documentIdentity);
    }

    /// <summary>Records the saved history state only if the snapshot belongs to the current document.</summary>
    public void MarkSaved(ImageEditorSaveSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        if (!IsCurrentDocument(snapshot)) return;
        _savedRevision = (ImageEditorDocument)snapshot.Revision;
        RefreshState();
    }

    /// <summary>Renames the current document only if the snapshot still belongs to it.</summary>
    public void RenameDocument(ImageEditorSaveSnapshot snapshot, string name)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (!IsCurrentDocument(snapshot)) return;
        foreach (var document in _history) document.Rename(name);
        OnPropertyChanged(nameof(DocumentSummary));
    }

    [RelayCommand(CanExecute = nameof(CanRotate))]
    public void RotateRight()
    {
        var source = ActiveDocument;
        if (source is null) return;
        int width = source.PixelSize.Width, height = source.PixelSize.Height;
        using var pixels = new WriteableBitmap(source.PixelSize, new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
        byte[] before = new byte[checked(width * height * 4)];
        using (var buffer = pixels.Lock())
        {
            source.Bitmap.CopyPixels(buffer);
            for (int y = 0; y < height; y++)
                Marshal.Copy(IntPtr.Add(buffer.Address, y * buffer.RowBytes), before, y * width * 4, width * 4);
        }
        byte[] after = new byte[before.Length];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                Buffer.BlockCopy(before, (y * width + x) * 4, after, (x * height + height - 1 - y) * 4, 4);
        if (width == height && before.AsSpan().SequenceEqual(after)) return;

        var rotated = new WriteableBitmap(new PixelSize(height, width), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
        try
        {
            using (var buffer = rotated.Lock())
                for (int y = 0; y < width; y++)
                    Marshal.Copy(after, y * height * 4, IntPtr.Add(buffer.Address, y * buffer.RowBytes), height * 4);
            var next = new ImageEditorDocument(source.Name, rotated);
            ContentChanging?.Invoke(this, EventArgs.Empty);
            for (int i = _history.Count - 1; i > _index; i--) { _history[i].Dispose(); _history.RemoveAt(i); }
            _history.Add(next);
            _index++;
            if (_history.Count > MaxHistoryStates) { _history[0].Dispose(); _history.RemoveAt(0); _index--; }
            SetActiveDocument(next);
            RefreshState();
        }
        catch { rotated.Dispose(); throw; }
    }

    private bool CanRotate() => HasDocument;
    private bool CanUndo() => HasDocument && _index > 0;
    private bool CanRedo() => HasDocument && _index + 1 < _history.Count;
    private bool CanClear() => AllowClear && HasDocument;

    [RelayCommand(CanExecute = nameof(CanUndo))]
    public void Undo()
    {
        if (!CanUndo()) return;
        ContentChanging?.Invoke(this, EventArgs.Empty);
        SetActiveDocument(_history[--_index]);
        RefreshState();
    }

    [RelayCommand(CanExecute = nameof(CanRedo))]
    public void Redo()
    {
        if (!CanRedo()) return;
        ContentChanging?.Invoke(this, EventArgs.Empty);
        SetActiveDocument(_history[++_index]);
        RefreshState();
    }

    [RelayCommand(CanExecute = nameof(CanClear))]
    public void ClearDocument()
    {
        if (!AllowClear) return;
        ClearCore();
    }

    private void ClearCore()
    {
        var old = _history.ToArray();
        _history.Clear();
        _documentIdentity = new object();
        _savedRevision = null;
        SetActiveDocument(null);
        RefreshState();
        foreach (var item in old) item.Dispose();
    }

    private void RefreshState()
    {
        OnPropertyChanged(nameof(IsDirty));
        RotateRightCommand.NotifyCanExecuteChanged();
        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
        ClearDocumentCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    public void SelectTool(ImageEditorTool tool) => SelectedTool = tool;
    [RelayCommand]
    public void ZoomIn() => ZoomPercentage = Math.Min(800, ZoomPercentage + 25);
    [RelayCommand]
    public void ZoomOut() => ZoomPercentage = Math.Max(10, ZoomPercentage - 25);
    [RelayCommand]
    public void ResetZoom() => ZoomPercentage = 100;
    public void Dispose() => ClearCore();
}
