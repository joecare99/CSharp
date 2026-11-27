using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avln_ImageEditor.Controls.Models;
using Avln_ImageEditor.Controls.ViewModels;

namespace Avln_ImageEditor.Controls.Views;

/// <summary>
/// Provides a reusable OS-agnostic image editing surface for Avalonia hosts.
/// </summary>
public partial class ImageEditorControl : UserControl
{
    private ImageEditorViewModel? _editor;
    private ImageEditorDocument? _dragDocument;
    private IPointer? _dragPointer;
    private Point _dragStart;
    private (string X, string Y, string Width, string Height) _beforeDrag;
    private bool _updatingCrop;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageEditorControl"/> class.
    /// </summary>
    public ImageEditorControl()
    {
        InitializeComponent();
        ImageSurface.PointerPressed += ImagePointerPressed;
        ImageSurface.PointerMoved += ImagePointerMoved;
        ImageSurface.PointerReleased += ImagePointerReleased;
        ImageSurface.PointerCaptureLost += ImagePointerCaptureLost;
        DragCropMode.PropertyChanged += (_, e) =>
        {
            if (e.Property == CheckBox.IsCheckedProperty && DragCropMode.IsChecked != true)
                CancelDrag(restore: true);
        };
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ObserveEditor(DataContext as ImageEditorViewModel);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        CancelDrag(restore: true);
        ObserveEditor(null);
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (this.IsAttachedToVisualTree()) ObserveEditor(DataContext as ImageEditorViewModel);
    }

    private void ObserveEditor(ImageEditorViewModel? editor)
    {
        if (ReferenceEquals(_editor, editor)) return;
        CancelDrag(restore: true);
        if (_editor is not null)
        {
            _editor.PropertyChanged -= EditorPropertyChanged;
            _editor.ContentChanging -= EditorContentChanging;
        }
        _editor = editor;
        if (_editor is not null)
        {
            _editor.PropertyChanged += EditorPropertyChanged;
            _editor.ContentChanging += EditorContentChanging;
        }
        DragCropMode.IsChecked = false;
    }

    private void EditorContentChanging(object? sender, EventArgs e) => CancelDrag(restore: true);

    private void EditorPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ImageEditorViewModel.ActiveDocument))
        {
            CancelDrag(restore: false);
            DragCropMode.IsChecked = false;
        }
        else if (_dragPointer is not null && !_updatingCrop &&
                 e.PropertyName is nameof(ImageEditorViewModel.CropX) or nameof(ImageEditorViewModel.CropY)
                 or nameof(ImageEditorViewModel.CropWidth) or nameof(ImageEditorViewModel.CropHeight))
        {
            CancelDrag(restore: false);
            if (_editor is not null) _editor.CropWidth = "0";
        }
    }

    private void ImagePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DragCropMode.IsChecked != true || _dragPointer is not null ||
            e.Pointer.Type != PointerType.Mouse || !e.GetCurrentPoint(ImageSurface).Properties.IsLeftButtonPressed ||
            _editor?.ActiveDocument is not { } document || _editor.IsCropSelectionInProgress)
            return;

        Point start = e.GetPosition(ImageSurface);
        if (!double.IsFinite(start.X) || !double.IsFinite(start.Y) ||
            start.X < 0 || start.Y < 0 || start.X >= document.PixelSize.Width || start.Y >= document.PixelSize.Height)
            return;

        _dragDocument = document;
        _dragPointer = e.Pointer;
        _dragStart = start;
        _beforeDrag = (_editor.CropX, _editor.CropY, _editor.CropWidth, _editor.CropHeight);
        _editor.IsCropSelectionInProgress = true;
        ImageSurface.Focus();
        e.Pointer.Capture(ImageSurface);
        if (!ReferenceEquals(e.Pointer.Captured, ImageSurface))
        {
            CancelDrag(restore: true);
            return;
        }
        UpdateDrag(start);
        e.Handled = true;
    }

    private void ImagePointerMoved(object? sender, PointerEventArgs e)
    {
        if (!ReferenceEquals(e.Pointer, _dragPointer)) return;
        UpdateDrag(e.GetPosition(ImageSurface));
        e.Handled = true;
    }

    private void ImagePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!ReferenceEquals(e.Pointer, _dragPointer) || e.InitialPressMouseButton != MouseButton.Left) return;
        UpdateDrag(e.GetPosition(ImageSurface));
        FinishDrag();
        e.Handled = true;
    }

    private void ImagePointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (ReferenceEquals(e.Pointer, _dragPointer)) CancelDrag(restore: true);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape && _dragPointer is not null)
        {
            CancelDrag(restore: true);
            e.Handled = true;
            return;
        }
        base.OnKeyDown(e);
    }

    private void UpdateDrag(Point end)
    {
        if (_editor is null || _dragDocument is null || !ReferenceEquals(_editor.ActiveDocument, _dragDocument) ||
            !double.IsFinite(end.X) || !double.IsFinite(end.Y))
        {
            CancelDrag(restore: false);
            return;
        }

        Point start = new(SnapPixelEdge(_dragStart.X), SnapPixelEdge(_dragStart.Y));
        end = new Point(SnapPixelEdge(Math.Clamp(end.X, 0, _dragDocument.PixelSize.Width)),
            SnapPixelEdge(Math.Clamp(end.Y, 0, _dragDocument.PixelSize.Height)));
        int left = (int)Math.Floor(Math.Min(start.X, end.X));
        int top = (int)Math.Floor(Math.Min(start.Y, end.Y));
        int right = (int)Math.Ceiling(Math.Max(start.X, end.X));
        int bottom = (int)Math.Ceiling(Math.Max(start.Y, end.Y));
        // A click or a line is empty even when it falls within a pixel.
        int width = end.X == start.X ? 0 : right - left;
        int height = end.Y == start.Y ? 0 : bottom - top;
        SetCrop(left.ToString(CultureInfo.InvariantCulture), top.ToString(CultureInfo.InvariantCulture),
            width.ToString(CultureInfo.InvariantCulture), height.ToString(CultureInfo.InvariantCulture));
    }

    private static double SnapPixelEdge(double value)
    {
        double nearest = Math.Round(value);
        return Math.Abs(value - nearest) < 1e-9 ? nearest : value;
    }

    private void SetCrop(string x, string y, string width, string height)
    {
        if (_editor is null) return;
        _updatingCrop = true;
        try
        {
            _editor.CropX = x;
            _editor.CropY = y;
            _editor.CropWidth = width;
            _editor.CropHeight = height;
        }
        finally { _updatingCrop = false; }
    }

    private void CancelDrag(bool restore)
    {
        if (_dragPointer is null) return;
        if (restore && ReferenceEquals(_editor?.ActiveDocument, _dragDocument))
            SetCrop(_beforeDrag.X, _beforeDrag.Y, _beforeDrag.Width, _beforeDrag.Height);
        FinishDrag();
    }

    private void FinishDrag()
    {
        var pointer = _dragPointer;
        _dragPointer = null;
        _dragDocument = null;
        if (_editor is not null) _editor.IsCropSelectionInProgress = false;
        if (pointer?.Captured == ImageSurface) pointer.Capture(null);
    }

    private void FitImageClick(object? sender, RoutedEventArgs e)
    {
        if (!this.IsAttachedToVisualTree() || !ImageScrollViewer.IsEffectivelyVisible ||
            ImageScrollViewer.Bounds.Width <= 0 ||
            ImageScrollViewer.Bounds.Height <= 0 || DataContext is not ImageEditorViewModel editor ||
            editor.ActiveDocument is not { } document)
            return;

        var viewport = ImageScrollViewer.Viewport;
        var image = document.DisplayImage.Size;
        if (!double.IsFinite(viewport.Width) || !double.IsFinite(viewport.Height) ||
            viewport.Width <= 0 || viewport.Height <= 0 ||
            viewport.Width > ImageScrollViewer.Bounds.Width + 0.01 || viewport.Height > ImageScrollViewer.Bounds.Height + 0.01 ||
            !double.IsFinite(image.Width) || !double.IsFinite(image.Height) ||
            image.Width <= 0 || image.Height <= 0)
            return;

        double percentage = Math.Min(viewport.Width / image.Width, viewport.Height / image.Height) * 100;
        editor.ZoomPercentage = Math.Clamp(percentage,
            ImageEditorViewModel.MinimumZoomPercentage, ImageEditorViewModel.MaximumZoomPercentage);
    }
}
