using System;
using System.IO;
using Avalonia.Media.Imaging;
using Avln_ImageEditor.Controls.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avln_ImageEditor.Controls.ViewModels;

/// <summary>
/// Coordinates the OS-agnostic state of the image editor control.
/// </summary>
public sealed partial class ImageEditorViewModel : ViewModelBase, IDisposable
{
    private const double MinimumZoomPercentage = 10.0;
    private const double MaximumZoomPercentage = 800.0;
    private const double ZoomStepPercentage = 25.0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasDocument))]
    [NotifyPropertyChangedFor(nameof(DocumentSummary))]
    private ImageEditorDocument? _activeDocument;

    [ObservableProperty]
    private ImageEditorTool _selectedTool = ImageEditorTool.Select;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ZoomFactor))]
    [NotifyPropertyChangedFor(nameof(ZoomSummary))]
    private double _zoomPercentage = 100.0;

    /// <summary>
    /// Gets a value indicating whether an image document is currently loaded.
    /// </summary>
    public bool HasDocument => ActiveDocument is not null;

    /// <summary>
    /// Gets the zoom factor used by the editor surface.
    /// </summary>
    public double ZoomFactor => ZoomPercentage / 100.0;

    /// <summary>
    /// Gets a compact zoom label.
    /// </summary>
    public string ZoomSummary => $"{ZoomPercentage:0}%";

    /// <summary>
    /// Gets a compact document label.
    /// </summary>
    public string DocumentSummary => ActiveDocument?.Summary ?? "No image loaded";

    /// <summary>
    /// Loads an image document from encoded image bytes.
    /// </summary>
    /// <param name="imageBytes">The encoded image bytes.</param>
    /// <param name="documentName">The display name of the document.</param>
    public void LoadDocument(byte[] imageBytes, string documentName)
    {
        ArgumentNullException.ThrowIfNull(imageBytes);
        ArgumentException.ThrowIfNullOrWhiteSpace(documentName);

        using var stream = new MemoryStream(imageBytes, writable: false);
        LoadDocument(new ImageEditorDocument(documentName, new Bitmap(stream)));
    }

    /// <summary>
    /// Loads an image document that was decoded by the caller.
    /// </summary>
    /// <param name="document">The image document to show.</param>
    public void LoadDocument(ImageEditorDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var previousDocument = ActiveDocument;
        ActiveDocument = document;
        ZoomPercentage = 100.0;
        previousDocument?.Dispose();
    }

    /// <summary>
    /// Clears the current document.
    /// </summary>
    [RelayCommand]
    public void ClearDocument()
    {
        var previousDocument = ActiveDocument;
        ActiveDocument = null;
        previousDocument?.Dispose();
    }

    /// <summary>
    /// Selects the active editor tool.
    /// </summary>
    /// <param name="tool">The tool to activate.</param>
    [RelayCommand]
    public void SelectTool(ImageEditorTool tool)
    {
        SelectedTool = tool;
    }

    /// <summary>
    /// Increases the editor zoom level.
    /// </summary>
    [RelayCommand]
    public void ZoomIn()
    {
        ZoomPercentage = Math.Min(MaximumZoomPercentage, ZoomPercentage + ZoomStepPercentage);
    }

    /// <summary>
    /// Decreases the editor zoom level.
    /// </summary>
    [RelayCommand]
    public void ZoomOut()
    {
        ZoomPercentage = Math.Max(MinimumZoomPercentage, ZoomPercentage - ZoomStepPercentage);
    }

    /// <summary>
    /// Resets the editor zoom level to the original image size.
    /// </summary>
    [RelayCommand]
    public void ResetZoom()
    {
        ZoomPercentage = 100.0;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ClearDocument();
    }
}
