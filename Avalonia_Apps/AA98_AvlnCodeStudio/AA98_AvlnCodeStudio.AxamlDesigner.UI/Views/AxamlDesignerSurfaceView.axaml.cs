using AA98_AvlnCodeStudio.AxamlDesigner.UI.Controls;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.AxamlDesigner.UI.Views;

public partial class AxamlDesignerSurfaceView : UserControl
{
    public AxamlDesignerSurfaceView()
    {
        InitializeComponent();
        PreviewControl.LayoutResizeRequested += OnLayoutResizeRequested;
    }

    public async Task<byte[]> CapturePreviewPngAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await Dispatcher.UIThread.InvokeAsync(CapturePreviewPng, DispatcherPriority.Render, cancellationToken);
    }

    private byte[] CapturePreviewPng()
    {
        if (PreviewControl.PreviewNode is null)
        {
            throw new InvalidOperationException("Open a valid AXAML document before capturing a screenshot.");
        }

        double width = PreviewControl.Bounds.Width;
        double height = PreviewControl.Bounds.Height;
        double scaling = (VisualRoot as TopLevel)?.RenderScaling ?? 1;
        int pixelWidth = (int)Math.Ceiling(width * scaling);
        int pixelHeight = (int)Math.Ceiling(height * scaling);
        if (pixelWidth <= 0 || pixelHeight <= 0 || pixelWidth > 4096 || pixelHeight > 4096
            || (long)pixelWidth * pixelHeight > 16_000_000)
        {
            throw new InvalidOperationException("The designer preview must be visible and within screenshot size limits.");
        }

        using RenderTargetBitmap bitmap = new(
            new PixelSize(pixelWidth, pixelHeight),
            new Vector(96 * scaling, 96 * scaling));
        bitmap.Render(PreviewControl);
        using MemoryStream stream = new();
        bitmap.Save(stream, PngBitmapEncoderOptions.Default);
        return stream.ToArray();
    }

    private async void OnScreenshotRequested(object? sender, RoutedEventArgs e)
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.StorageProvider is null)
        {
            SetStatus("Screenshot cannot be saved because file storage is unavailable.");
            return;
        }

        try
        {
            IStorageFile? file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save designer screenshot",
                SuggestedFileName = "axaml-preview.png",
                DefaultExtension = "png",
                FileTypeChoices = [new FilePickerFileType("PNG image") { Patterns = ["*.png"], MimeTypes = ["image/png"] }],
            });
            if (file is null)
            {
                return;
            }

            byte[] image = await CapturePreviewPngAsync();
            await using Stream output = await file.OpenWriteAsync();
            await output.WriteAsync(image);
            SetStatus($"Screenshot saved to {file.Name}.");
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            SetStatus($"Screenshot capture failed: {exception.Message}");
        }
    }

    private void SetStatus(string message)
    {
        if (DataContext is AxamlDesignerViewModel viewModel)
        {
            viewModel.StatusText = message;
        }
    }

    private void OnLayoutResizeRequested(object? sender, AxamlPreviewResizeEventArgs e)
    {
        if (DataContext is AxamlDesignerViewModel viewModel)
        {
            viewModel.ResizeElement(e.Node, e.Width, e.Height);
        }
    }
}
