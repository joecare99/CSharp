using System;
using System.Collections.Generic;
using System.Globalization;
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
        var oldSize = _activeDocument?.PixelSize;
        if (SetProperty(ref _activeDocument, value, nameof(ActiveDocument)))
        {
            OnPropertyChanged(nameof(HasDocument));
            OnPropertyChanged(nameof(DocumentSummary));
            if (oldSize != value?.PixelSize) ResetCrop();
            NotifyCropState();
            ResetCropCoordinatesCommand.NotifyCanExecuteChanged();
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

    [ObservableProperty]
    private string _cropX = "0";
    [ObservableProperty]
    private string _cropY = "0";
    [ObservableProperty]
    private string _cropWidth = "0";
    [ObservableProperty]
    private string _cropHeight = "0";

    partial void OnCropXChanged(string value) => NotifyCropState();
    partial void OnCropYChanged(string value) => NotifyCropState();
    partial void OnCropWidthChanged(string value) => NotifyCropState();
    partial void OnCropHeightChanged(string value) => NotifyCropState();

    /// <summary>Explains why the current coordinate inputs cannot be applied, or is empty when valid.</summary>
    public string CropError => !HasDocument ? "Load an image to crop." :
        TryGetCrop(out _, out _, out _, out _) ? "" :
        "Enter whole numbers: X and Y must be nonnegative; width and height must be positive and fit within the image.";

    /// <summary>Valid crop rectangle in image-pixel coordinates, or an empty rectangle.</summary>
    public Rect CropPreview => TryGetCrop(out int x, out int y, out int width, out int height)
        ? new Rect(x, y, width, height) : default;

    /// <summary>Whether the crop preview is shown. Whole-image crops are not outlined.</summary>
    public bool ShowCropPreview => TryGetCrop(out int x, out int y, out int width, out int height) &&
        (x != 0 || y != 0 || width != ActiveDocument!.PixelSize.Width || height != ActiveDocument.PixelSize.Height);

    private void ResetCrop()
    {
        CropX = "0";
        CropY = "0";
        CropWidth = (ActiveDocument?.PixelSize.Width ?? 0).ToString(CultureInfo.InvariantCulture);
        CropHeight = (ActiveDocument?.PixelSize.Height ?? 0).ToString(CultureInfo.InvariantCulture);
    }

    private void NotifyCropState()
    {
        OnPropertyChanged(nameof(CropError));
        OnPropertyChanged(nameof(CropPreview));
        OnPropertyChanged(nameof(ShowCropPreview));
        ApplyCropCommand.NotifyCanExecuteChanged();
    }

    private bool TryGetCrop(out int x, out int y, out int width, out int height)
    {
        x = y = width = height = 0;
        var size = ActiveDocument?.PixelSize;
        return size.HasValue &&
            int.TryParse(CropX, NumberStyles.Integer, CultureInfo.InvariantCulture, out x) &&
            int.TryParse(CropY, NumberStyles.Integer, CultureInfo.InvariantCulture, out y) &&
            int.TryParse(CropWidth, NumberStyles.Integer, CultureInfo.InvariantCulture, out width) &&
            int.TryParse(CropHeight, NumberStyles.Integer, CultureInfo.InvariantCulture, out height) &&
            x >= 0 && y >= 0 && width > 0 && height > 0 &&
            x < size.Value.Width && y < size.Value.Height &&
            width <= size.Value.Width - x && height <= size.Value.Height - y;
    }

    private bool CanApplyCrop() => TryGetCrop(out _, out _, out _, out _);

    /// <summary>Resets only the crop coordinates to the current image's full extent.</summary>
    [RelayCommand(CanExecute = nameof(CanTransform))]
    public void ResetCropCoordinates()
    {
        if (HasDocument) ResetCrop();
    }

    /// <summary>Crops to the validated pixel rectangle; invalid and whole-image rectangles do nothing.</summary>
    [RelayCommand(CanExecute = nameof(CanApplyCrop))]
    public void ApplyCrop()
    {
        if (!TryGetCrop(out int x, out int y, out int width, out int height)) return;
        var source = ActiveDocument!;
        if (x == 0 && y == 0 && width == source.PixelSize.Width && height == source.PixelSize.Height) return;

        using var pixels = new WriteableBitmap(source.PixelSize, new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
        var cropped = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
        try
        {
            using (var input = pixels.Lock())
            using (var output = cropped.Lock())
            {
                source.Bitmap.CopyPixels(input);
                byte[] row = new byte[checked(width * 4)];
                for (int rowIndex = 0; rowIndex < height; rowIndex++)
                {
                    Marshal.Copy(IntPtr.Add(input.Address, checked((y + rowIndex) * input.RowBytes + x * 4)), row, 0, row.Length);
                    Marshal.Copy(row, 0, IntPtr.Add(output.Address, rowIndex * output.RowBytes), row.Length);
                }
            }
            CommitPixels(source, cropped);
        }
        catch { cropped.Dispose(); throw; }
    }

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
        ResetCrop();
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

    [RelayCommand(CanExecute = nameof(CanTransform))]
    public void RotateRight() => TransformPixels(PixelTransform.RotateRight);

    [RelayCommand(CanExecute = nameof(CanTransform))]
    public void RotateLeft() => TransformPixels(PixelTransform.RotateLeft);

    [RelayCommand(CanExecute = nameof(CanTransform))]
    public void Rotate180() => TransformPixels(PixelTransform.Rotate180);

    [RelayCommand(CanExecute = nameof(CanTransform))]
    public void FlipHorizontal() => TransformPixels(PixelTransform.FlipHorizontal);

    [RelayCommand(CanExecute = nameof(CanTransform))]
    public void FlipVertical() => TransformPixels(PixelTransform.FlipVertical);

    /// <summary>Inverts visible RGB colors while preserving alpha.</summary>
    [RelayCommand(CanExecute = nameof(CanTransform))]
    public void InvertColors() => TransformPixels(PixelTransform.InvertColors);

    /// <summary>Converts to grayscale using BT.601 luma weights, preserving alpha.</summary>
    [RelayCommand(CanExecute = nameof(CanTransform))]
    public void ConvertToGrayscale() => TransformPixels(PixelTransform.Grayscale);

    private enum PixelTransform { RotateRight, RotateLeft, Rotate180, FlipHorizontal, FlipVertical, InvertColors, Grayscale }

    private void TransformPixels(PixelTransform transform)
    {
        var source = ActiveDocument;
        if (source is null) return;
        int width = source.PixelSize.Width, height = source.PixelSize.Height;
        bool rotate = transform is PixelTransform.RotateRight or PixelTransform.RotateLeft;
        int targetWidth = rotate ? height : width, targetHeight = rotate ? width : height;
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
            {
                int targetX = transform == PixelTransform.RotateRight ? height - 1 - y :
                    transform == PixelTransform.RotateLeft ? y : transform is PixelTransform.Rotate180 or PixelTransform.FlipHorizontal ? width - 1 - x : x;
                int targetY = transform == PixelTransform.RotateRight ? x :
                    transform == PixelTransform.RotateLeft ? width - 1 - x : transform is PixelTransform.Rotate180 or PixelTransform.FlipVertical ? height - 1 - y : y;
                int input = (y * width + x) * 4;
                int output = (targetY * targetWidth + targetX) * 4;
                if (transform == PixelTransform.InvertColors)
                {
                    byte alpha = before[input + 3];
                    for (int channel = 0; channel < 3; channel++)
                        after[output + channel] = (byte)(alpha - Math.Min(alpha, before[input + channel]));
                    after[output + 3] = alpha;
                }
                else if (transform == PixelTransform.Grayscale)
                {
                    byte alpha = before[input + 3];
                    int blue = Math.Min(alpha, before[input]);
                    int green = Math.Min(alpha, before[input + 1]);
                    int red = Math.Min(alpha, before[input + 2]);
                    byte gray = (byte)((299 * red + 587 * green + 114 * blue + 500) / 1000);
                    after[output] = after[output + 1] = after[output + 2] = gray;
                    after[output + 3] = alpha;
                }
                else
                    Buffer.BlockCopy(before, input, after, output, 4);
            }
        if (width == targetWidth && height == targetHeight && before.AsSpan().SequenceEqual(after)) return;

        var transformed = new WriteableBitmap(new PixelSize(targetWidth, targetHeight), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
        try
        {
            using (var buffer = transformed.Lock())
                for (int y = 0; y < targetHeight; y++)
                    Marshal.Copy(after, y * targetWidth * 4, IntPtr.Add(buffer.Address, y * buffer.RowBytes), targetWidth * 4);
            CommitPixels(source, transformed);
        }
        catch { transformed.Dispose(); throw; }
    }

    private void CommitPixels(ImageEditorDocument source, WriteableBitmap pixels)
    {
        var next = new ImageEditorDocument(source.Name, pixels);
        ContentChanging?.Invoke(this, EventArgs.Empty);
        for (int i = _history.Count - 1; i > _index; i--) { _history[i].Dispose(); _history.RemoveAt(i); }
        _history.Add(next);
        _index++;
        if (_history.Count > MaxHistoryStates) { _history[0].Dispose(); _history.RemoveAt(0); _index--; }
        SetActiveDocument(next);
        RefreshState();
    }

    private bool CanTransform() => HasDocument;
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
        RotateLeftCommand.NotifyCanExecuteChanged();
        Rotate180Command.NotifyCanExecuteChanged();
        FlipHorizontalCommand.NotifyCanExecuteChanged();
        FlipVerticalCommand.NotifyCanExecuteChanged();
        InvertColorsCommand.NotifyCanExecuteChanged();
        ConvertToGrayscaleCommand.NotifyCanExecuteChanged();
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
