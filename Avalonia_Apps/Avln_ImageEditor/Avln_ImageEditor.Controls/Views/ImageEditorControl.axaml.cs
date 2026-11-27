using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
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
    private IPointer? _brushPointer;
    private ImageEditorDocument? _brushDocument;
    private Color _brushColor;
    private bool _erasing;
    private Point _brushLast;
    private readonly HashSet<int> _brushPixels = new();
    private IPointer? _fillPointer;
    private ImageEditorDocument? _fillDocument;
    private Color _fillColor;
    private int _fillX, _fillY;

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
            if (e.Property != CheckBox.IsCheckedProperty) return;
            if (DragCropMode.IsChecked == true)
            {
                SampleColorMode.IsChecked = false;
                BrushMode.IsChecked = false;
                EraserMode.IsChecked = false;
                FillMode.IsChecked = false;
            }
            else CancelDrag(restore: true);
        };
        SampleColorMode.PropertyChanged += (_, e) =>
        {
            if (e.Property == CheckBox.IsCheckedProperty && SampleColorMode.IsChecked == true)
            {
                DragCropMode.IsChecked = false;
                BrushMode.IsChecked = false;
                EraserMode.IsChecked = false;
                FillMode.IsChecked = false;
            }
        };
        BrushMode.PropertyChanged += (_, e) =>
        {
            if (e.Property != CheckBox.IsCheckedProperty) return;
            if (BrushMode.IsChecked == true)
            {
                DragCropMode.IsChecked = false;
                SampleColorMode.IsChecked = false;
                EraserMode.IsChecked = false;
                FillMode.IsChecked = false;
                if (_editor is not null) _editor.SelectedTool = ImageEditorTool.Brush;
            }
            else
            {
                CancelBrush();
                if (_editor?.SelectedTool == ImageEditorTool.Brush) _editor.SelectedTool = ImageEditorTool.Select;
            }
        };
        EraserMode.PropertyChanged += (_, e) =>
        {
            if (e.Property != CheckBox.IsCheckedProperty) return;
            if (EraserMode.IsChecked == true)
            {
                DragCropMode.IsChecked = false;
                SampleColorMode.IsChecked = false;
                BrushMode.IsChecked = false;
                FillMode.IsChecked = false;
                if (_editor is not null) _editor.SelectedTool = ImageEditorTool.Eraser;
            }
            else
            {
                CancelBrush();
                if (_editor?.SelectedTool == ImageEditorTool.Eraser) _editor.SelectedTool = ImageEditorTool.Select;
            }
        };
        FillMode.PropertyChanged += (_, e) =>
        {
            if (e.Property != CheckBox.IsCheckedProperty) return;
            if (FillMode.IsChecked == true)
            {
                DragCropMode.IsChecked = false;
                SampleColorMode.IsChecked = false;
                BrushMode.IsChecked = false;
                EraserMode.IsChecked = false;
                if (_editor is not null) _editor.SelectedTool = ImageEditorTool.Fill;
            }
            else
            {
                CancelFill();
                if (_editor?.SelectedTool == ImageEditorTool.Fill) _editor.SelectedTool = ImageEditorTool.Select;
            }
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
        CancelBrush();
        CancelFill();
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
        CancelBrush();
        CancelFill();
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
        SampleColorMode.IsChecked = false;
        BrushMode.IsChecked = false;
        EraserMode.IsChecked = false;
        FillMode.IsChecked = false;
        if (_editor?.SelectedTool is ImageEditorTool.Brush or ImageEditorTool.Eraser or ImageEditorTool.Fill) _editor.SelectedTool = ImageEditorTool.Select;
    }

    private void EditorContentChanging(object? sender, EventArgs e)
    {
        CancelDrag(restore: true);
        CancelBrush();
        CancelFill();
    }

    private void EditorPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ImageEditorViewModel.ActiveDocument))
        {
            CancelDrag(restore: false);
            CancelBrush();
            CancelFill();
            if (_editor?.IsCommittingBrushRevision != true)
            {
                DragCropMode.IsChecked = false;
                SampleColorMode.IsChecked = false;
                BrushMode.IsChecked = false;
                EraserMode.IsChecked = false;
                FillMode.IsChecked = false;
            }
        }
        else if (e.PropertyName == nameof(ImageEditorViewModel.SelectedTool))
        {
            BrushMode.IsChecked = _editor?.SelectedTool == ImageEditorTool.Brush;
            EraserMode.IsChecked = _editor?.SelectedTool == ImageEditorTool.Eraser;
            FillMode.IsChecked = _editor?.SelectedTool == ImageEditorTool.Fill;
        }
        else if (e.PropertyName == nameof(ImageEditorViewModel.SampledColor))
        {
            CancelBrush();
            CancelFill();
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
        if (_fillPointer is not null)
        {
            CancelFill();
            e.Handled = true;
            return;
        }
        var fillProperties = e.GetCurrentPoint(ImageSurface).Properties;
        if (FillMode.IsChecked == true && _dragPointer is null && _brushPointer is null &&
            e.Pointer.Type == PointerType.Mouse &&
            fillProperties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed &&
            fillProperties.IsLeftButtonPressed && !fillProperties.IsRightButtonPressed && !fillProperties.IsMiddleButtonPressed &&
            _editor?.ActiveDocument is { } fillDocument && !_editor.IsCropSelectionInProgress &&
            _editor.SampledColor is { } fillColor)
        {
            Point point = e.GetPosition(ImageSurface);
            if (!TryFillPixel(point, fillDocument, out _fillX, out _fillY)) return;
            _fillDocument = fillDocument;
            _fillColor = fillColor;
            _fillPointer = e.Pointer;
            ImageSurface.Focus();
            e.Pointer.Capture(ImageSurface);
            if (!ReferenceEquals(e.Pointer.Captured, ImageSurface)) { CancelFill(); return; }
            e.Handled = true;
            return;
        }
        if (_erasing && ReferenceEquals(e.Pointer, _brushPointer))
        {
            CancelBrush();
            e.Handled = true;
            return;
        }
        if ((BrushMode.IsChecked == true || EraserMode.IsChecked == true) && _brushPointer is null &&
            e.Pointer.Type == PointerType.Mouse &&
            e.GetCurrentPoint(ImageSurface).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed &&
            (EraserMode.IsChecked != true ||
             (!e.GetCurrentPoint(ImageSurface).Properties.IsRightButtonPressed &&
              !e.GetCurrentPoint(ImageSurface).Properties.IsMiddleButtonPressed)) &&
            _editor?.ActiveDocument is { } brushDocument &&
            (EraserMode.IsChecked == true || _editor.SampledColor is not null))
        {
            Point brushStart = e.GetPosition(ImageSurface);
            if (!ValidBrushPoint(brushStart) || brushStart.X < 0 || brushStart.Y < 0 ||
                brushStart.X >= brushDocument.PixelSize.Width || brushStart.Y >= brushDocument.PixelSize.Height) return;
            _brushDocument = brushDocument;
            _erasing = EraserMode.IsChecked == true;
            _brushColor = _editor.SampledColor.GetValueOrDefault();
            _brushLast = brushStart;
            _brushPointer = e.Pointer;
            _brushPixels.Clear();
            AddBrushSegment(brushStart, brushStart);
            ImageSurface.Focus();
            e.Pointer.Capture(ImageSurface);
            if (!ReferenceEquals(e.Pointer.Captured, ImageSurface)) { CancelBrush(); return; }
            e.Handled = true;
            return;
        }
        if (SampleColorMode.IsChecked == true && _dragPointer is null &&
            e.Pointer.Type == PointerType.Mouse &&
            e.GetCurrentPoint(ImageSurface).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed &&
            _editor?.ActiveDocument is { } sampleDocument && !_editor.IsCropSelectionInProgress)
        {
            Point point = e.GetPosition(ImageSurface);
            if (double.IsFinite(point.X) && double.IsFinite(point.Y) &&
                point.X >= 0 && point.Y >= 0 && point.X < sampleDocument.PixelSize.Width &&
                point.Y < sampleDocument.PixelSize.Height &&
                _editor.SamplePixel((int)Math.Floor(point.X), (int)Math.Floor(point.Y)))
            {
                ImageSurface.Focus();
                e.Handled = true;
            }
            return;
        }
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
        if (ReferenceEquals(e.Pointer, _fillPointer))
        {
            var properties = e.GetCurrentPoint(ImageSurface).Properties;
            if (!properties.IsLeftButtonPressed || properties.IsRightButtonPressed || properties.IsMiddleButtonPressed ||
                properties.PointerUpdateKind != PointerUpdateKind.Other || _fillDocument is null ||
                !TryFillPixel(e.GetPosition(ImageSurface), _fillDocument, out int x, out int y) ||
                x != _fillX || y != _fillY) CancelFill();
            e.Handled = true;
            return;
        }
        if (ReferenceEquals(e.Pointer, _brushPointer))
        {
            var properties = e.GetCurrentPoint(ImageSurface).Properties;
            if (!properties.IsLeftButtonPressed || (_erasing &&
                (properties.IsRightButtonPressed || properties.IsMiddleButtonPressed ||
                 properties.PointerUpdateKind != PointerUpdateKind.Other))) CancelBrush();
            else ContinueBrush(e.GetPosition(ImageSurface));
            e.Handled = true;
            return;
        }
        if (!ReferenceEquals(e.Pointer, _dragPointer)) return;
        UpdateDrag(e.GetPosition(ImageSurface));
        e.Handled = true;
    }

    private void ImagePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (ReferenceEquals(e.Pointer, _fillPointer))
        {
            var properties = e.GetCurrentPoint(ImageSurface).Properties;
            var document = _fillDocument;
            var editor = _editor;
            var color = _fillColor;
            int x = _fillX, y = _fillY;
            bool valid = e.InitialPressMouseButton == MouseButton.Left &&
                properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased &&
                !properties.IsLeftButtonPressed && !properties.IsRightButtonPressed && !properties.IsMiddleButtonPressed &&
                document is not null && editor is not null && ReferenceEquals(editor.ActiveDocument, document) &&
                editor.SelectedTool == ImageEditorTool.Fill && editor.SampledColor == color &&
                TryFillPixel(e.GetPosition(ImageSurface), document, out int endX, out int endY) && endX == x && endY == y;
            CancelFill();
            if (valid) editor!.FloodFill(document!, color, x, y);
            e.Handled = true;
            return;
        }
        if (ReferenceEquals(e.Pointer, _brushPointer) && e.InitialPressMouseButton == MouseButton.Left &&
            e.GetCurrentPoint(ImageSurface).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased &&
            (!_erasing || (!e.GetCurrentPoint(ImageSurface).Properties.IsRightButtonPressed &&
                           !e.GetCurrentPoint(ImageSurface).Properties.IsMiddleButtonPressed)))
        {
            ContinueBrush(e.GetPosition(ImageSurface));
            if (_brushPointer is not null && _editor is { } editor && _brushDocument is { } document)
            {
                var pixels = new HashSet<int>(_brushPixels);
                var color = _brushColor;
                bool erasing = _erasing;
                CancelBrush();
                editor.PaintStroke(document, color, pixels, erasing);
            }
            e.Handled = true;
            return;
        }
        if (ReferenceEquals(e.Pointer, _brushPointer) && _erasing)
        {
            CancelBrush();
            e.Handled = true;
            return;
        }
        if (!ReferenceEquals(e.Pointer, _dragPointer) || e.InitialPressMouseButton != MouseButton.Left) return;
        UpdateDrag(e.GetPosition(ImageSurface));
        FinishDrag();
        e.Handled = true;
    }

    private void ImagePointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (ReferenceEquals(e.Pointer, _brushPointer)) CancelBrush();
        if (ReferenceEquals(e.Pointer, _fillPointer)) CancelFill();
        if (ReferenceEquals(e.Pointer, _dragPointer)) CancelDrag(restore: true);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape && _fillPointer is not null)
        {
            CancelFill();
            e.Handled = true;
            return;
        }
        if (e.Key == Key.Escape && _brushPointer is not null)
        {
            CancelBrush();
            e.Handled = true;
            return;
        }
        if (e.Key == Key.Escape && _dragPointer is not null)
        {
            CancelDrag(restore: true);
            e.Handled = true;
            return;
        }
        if (e.Key == Key.Escape && SampleColorMode.IsChecked == true)
        {
            SampleColorMode.IsChecked = false;
            e.Handled = true;
            return;
        }
        base.OnKeyDown(e);
    }

    private static bool ValidBrushPoint(Point p) =>
        double.IsFinite(p.X) && double.IsFinite(p.Y) && Math.Abs(p.X) <= 1e9 && Math.Abs(p.Y) <= 1e9;

    private static bool TryFillPixel(Point point, ImageEditorDocument document, out int x, out int y)
    {
        x = y = 0;
        if (!double.IsFinite(point.X) || !double.IsFinite(point.Y) || point.X < 0 || point.Y < 0 ||
            point.X >= document.PixelSize.Width || point.Y >= document.PixelSize.Height) return false;
        x = (int)Math.Floor(point.X);
        y = (int)Math.Floor(point.Y);
        return true;
    }

    private void CancelFill()
    {
        var pointer = _fillPointer;
        _fillPointer = null;
        _fillDocument = null;
        if (pointer?.Captured == ImageSurface) pointer.Capture(null);
    }

    private void ContinueBrush(Point point)
    {
        if (!ValidBrushPoint(point) || !ReferenceEquals(_editor?.ActiveDocument, _brushDocument))
        {
            CancelBrush();
            return;
        }
        AddBrushSegment(_brushLast, point);
        _brushLast = point;
    }

    private void AddBrushSegment(Point from, Point to)
    {
        if (_brushDocument is null) return;
        int width = _brushDocument.PixelSize.Width, height = _brushDocument.PixelSize.Height;
        double dx = to.X - from.X, dy = to.Y - from.Y, first = 0, last = 1;
        bool Clip(double p, double q)
        {
            if (p == 0) return q >= 0;
            double r = q / p;
            if (p < 0) { if (r > last) return false; first = Math.Max(first, r); }
            else { if (r < first) return false; last = Math.Min(last, r); }
            return true;
        }
        if (!Clip(-dx, from.X) || !Clip(dx, Math.BitDecrement((double)width) - from.X) ||
            !Clip(-dy, from.Y) || !Clip(dy, Math.BitDecrement((double)height) - from.Y)) return;
        int x0 = Math.Clamp((int)Math.Floor(from.X + first * dx), 0, width - 1);
        int y0 = Math.Clamp((int)Math.Floor(from.Y + first * dy), 0, height - 1);
        int x1 = Math.Clamp((int)Math.Floor(from.X + last * dx), 0, width - 1);
        int y1 = Math.Clamp((int)Math.Floor(from.Y + last * dy), 0, height - 1);
        if (x0 > x1 || (x0 == x1 && y0 > y1))
            (x0, y0, x1, y1) = (x1, y1, x0, y0);
        int stepX = x0 < x1 ? 1 : -1, stepY = y0 < y1 ? 1 : -1;
        int deltaX = Math.Abs(x1 - x0), deltaY = -Math.Abs(y1 - y0), error = deltaX + deltaY;
        while (true)
        {
            _brushPixels.Add(y0 * width + x0);
            if (x0 == x1 && y0 == y1) break;
            int twice = 2 * error;
            if (twice >= deltaY) { error += deltaY; x0 += stepX; }
            if (twice <= deltaX) { error += deltaX; y0 += stepY; }
        }
    }

    private void CancelBrush()
    {
        var pointer = _brushPointer;
        _brushPointer = null;
        _brushDocument = null;
        _brushPixels.Clear();
        if (pointer?.Captured == ImageSurface) pointer.Capture(null);
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

    private void ManualColorKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter || DataContext is not ImageEditorViewModel editor) return;
        editor.ApplyManualColor();
        e.Handled = true;
    }
}
