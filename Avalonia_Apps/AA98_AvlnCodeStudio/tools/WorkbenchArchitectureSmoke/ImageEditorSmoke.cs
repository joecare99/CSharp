using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.UI.Components;
using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AvlnCodeStudio.UI.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.VisualTree;
using Avln_ImageEditor.Controls.Models;
using Avln_ImageEditor.Controls.ViewModels;
using Avln_ImageEditor.Controls.Views;
using Microsoft.Extensions.DependencyInjection;
using Project.Explorer.Avalonia.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

internal static partial class Program
{
    private static async Task VerifyImageEditorAsync(IClassicDesktopStyleApplicationLifetime desktop, string fixture)
    {
        MainWindowViewModel workbench = SmokeApplication.Services!.GetRequiredService<MainWindowViewModel>();
        Window window = desktop.MainWindow!;
        string source = Path.Combine(fixture, "Editable.png");
        byte[] sourceBytes = await File.ReadAllBytesAsync(source);
        await workbench.OpenWorkspaceFileAsync(Path.Combine(fixture, "Sample.csproj"));

        ProjectExplorerViewModel explorer = workbench.ProjectExplorer!;
        explorer.SelectedItem = FindItem(explorer.RootItems, source);
        await WaitForActiveAsync(workbench, source);
        EditorTabViewModel tab = workbench.ActiveEditorTab;
        Check(tab.IsPreview && !tab.HasTextEditor && !tab.IsDiagnostic && tab.View is ImageEditorControl,
            "The image was not opened as an image-editor preview.");
        ImageEditorViewModel image = (ImageEditorViewModel)tab.View.DataContext!;
        await WaitUntilAsync(() => window.GetVisualDescendants().Contains(tab.View)
            && tab.View.GetVisualDescendants().OfType<Image>().Any(element => element.Name == "EditorImage" && element.Source is not null)
            && tab.View.GetVisualDescendants().OfType<Button>()
                .Any(button => ReferenceEquals(button.Command, image.RotateRightCommand)),
            "the rendered image editor");
        Button[] imageButtons = tab.View.GetVisualDescendants().OfType<Button>().ToArray();
        Check(imageButtons.Any(button => ReferenceEquals(button.Command, image.UndoCommand))
            && imageButtons.Any(button => ReferenceEquals(button.Command, image.RedoCommand)),
            "The image editor did not expose its undo and redo commands.");
        Button rotateLeft = imageButtons.Single(button => ReferenceEquals(button.Command, image.RotateLeftCommand));
        Button rotate180 = imageButtons.Single(button => ReferenceEquals(button.Command, image.Rotate180Command));
        Button crop = imageButtons.Single(button => ReferenceEquals(button.Command, image.ApplyCropCommand));
        Button resetCrop = imageButtons.Single(button => ReferenceEquals(button.Command, image.ResetCropCoordinatesCommand));
        Button flip = imageButtons.Single(button => ReferenceEquals(button.Command, image.FlipHorizontalCommand));
        Button flipVertical = imageButtons.Single(button => ReferenceEquals(button.Command, image.FlipVerticalCommand));
        Button invert = imageButtons.Single(button => ReferenceEquals(button.Command, image.InvertColorsCommand));
        Button grayscale = imageButtons.Single(button => ReferenceEquals(button.Command, image.ConvertToGrayscaleCommand));
        Check(!image.AllowClear && !tab.IsDirty && ImageSize(image) == (2, 3),
            "The shared editor did not open the original image as a clean document.");

        byte[] initial = image.CapturePngSnapshot().Bytes;
        await VerifyImageDpiRenderingAsync(window, tab.View, image, initial);
        Check(!tab.IsDirty && image.CapturePngSnapshot().Bytes.SequenceEqual(initial),
            "The DPI rendering probe did not restore the clean image.");
        Check(grayscale.IsEnabled, "The grayscale button is not available.");
        Invoke(tab.View, "Convert to grayscale");
        byte[] gray = image.CapturePngSnapshot().Bytes;
        Check(tab.IsDirty && ImageSize(image) == (2, 3) && !gray.SequenceEqual(initial),
            "Grayscale did not update the image.");
        image.UndoCommand.Execute(null);
        Check(!tab.IsDirty && image.CapturePngSnapshot().Bytes.SequenceEqual(initial),
            "Undo did not restore the image before grayscale.");
        image.RedoCommand.Execute(null);
        Check(tab.IsDirty && image.CapturePngSnapshot().Bytes.SequenceEqual(gray),
            "Redo did not restore the grayscale image.");
        image.UndoCommand.Execute(null);
        Check(invert.IsEnabled, "The invert colors button is not available.");
        Invoke(tab.View, "Invert colors");
        byte[] inverted = image.CapturePngSnapshot().Bytes;
        Check(tab.IsDirty && ImageSize(image) == (2, 3) && !inverted.SequenceEqual(initial),
            "Invert colors did not update the image.");
        image.UndoCommand.Execute(null);
        Check(!tab.IsDirty && image.CapturePngSnapshot().Bytes.SequenceEqual(initial),
            "Undo did not restore the image before inversion.");
        image.RedoCommand.Execute(null);
        Check(tab.IsDirty && image.CapturePngSnapshot().Bytes.SequenceEqual(inverted),
            "Redo did not restore the inverted image.");
        image.UndoCommand.Execute(null);
        Check(rotateLeft.IsEnabled, "The counterclockwise rotation button is not available.");
        Invoke(tab.View, "Rotate left");
        byte[] leftRotated = image.CapturePngSnapshot().Bytes;
        Check(!tab.IsPreview && tab.IsDirty && ImageSize(image) == (3, 2)
            && !leftRotated.SequenceEqual(initial), "Counterclockwise rotation did not update the image.");
        image.UndoCommand.Execute(null);
        Check(!tab.IsDirty && image.CapturePngSnapshot().Bytes.SequenceEqual(initial),
            "Undo did not restore the image before counterclockwise rotation.");
        image.RedoCommand.Execute(null);
        Check(tab.IsDirty && image.CapturePngSnapshot().Bytes.SequenceEqual(leftRotated),
            "Redo did not restore the counterclockwise rotation.");
        image.UndoCommand.Execute(null);

        Check(rotate180.IsEnabled, "The 180-degree rotation button is not available.");
        Invoke(tab.View, "Rotate 180°");
        byte[] halfTurn = image.CapturePngSnapshot().Bytes;
        Check(tab.IsDirty && ImageSize(image) == (2, 3) && !halfTurn.SequenceEqual(initial),
            "The 180-degree rotation did not update the image.");
        image.UndoCommand.Execute(null);
        Check(!tab.IsDirty && image.CapturePngSnapshot().Bytes.SequenceEqual(initial),
            "One undo did not restore the image before the 180-degree rotation.");
        image.RedoCommand.Execute(null);
        Check(tab.IsDirty && image.CapturePngSnapshot().Bytes.SequenceEqual(halfTurn),
            "Redo did not restore the 180-degree rotation.");
        image.UndoCommand.Execute(null);

        TextBox[] cropInputs = tab.View.GetVisualDescendants().OfType<TextBox>()
            .Where(input => input.Name?.StartsWith("Crop", StringComparison.Ordinal) == true).ToArray();
        CropPreviewOverlay cropPreview = tab.View.GetVisualDescendants().OfType<CropPreviewOverlay>().Single();
        ScrollViewer imageScroll = tab.View.GetVisualDescendants().OfType<ScrollViewer>()
            .Single(element => element.Name == "ImageScrollViewer");
        Image renderedImage = tab.View.GetVisualDescendants().OfType<Image>()
            .Single(element => element.Name == "EditorImage");
        var originalDocument = image.ActiveDocument;
        int previewChanges = 0;
        EventHandler previewChanging = (_, _) => previewChanges++;
        image.ContentChanging += previewChanging;
        Check(cropInputs.Length == 4, "The image crop fields were not available.");
        cropInputs.Single(input => input.Name == "CropXInput").SetCurrentValue(TextBox.TextProperty, "-1");
        await WaitUntilAsync(() => image.CropX == "-1" && !crop.IsEffectivelyEnabled
            && !cropPreview.IsVisible, "invalid image crop");
        Check(!string.IsNullOrEmpty(image.CropError) && !tab.IsDirty
            && ReferenceEquals(image.ActiveDocument, originalDocument)
            && image.RedoCommand.CanExecute(null) && !image.UndoCommand.CanExecute(null)
            && image.CapturePngSnapshot().Bytes.SequenceEqual(initial) && previewChanges == 0,
            "Invalid image crop input was not rejected.");
        async Task ResetCropAndCheckAsync(string scenario)
        {
            Check(resetCrop.IsEffectivelyEnabled, $"Reset crop was disabled after {scenario}.");
            Invoke(tab.View, "Reset crop coordinates");
            await WaitUntilAsync(() => image.CropX == "0" && image.CropY == "0"
                && image.CropWidth == "2" && image.CropHeight == "3"
                && cropInputs.Single(input => input.Name == "CropXInput").Text == "0"
                && cropInputs.Single(input => input.Name == "CropYInput").Text == "0"
                && cropInputs.Single(input => input.Name == "CropWidthInput").Text == "2"
                && cropInputs.Single(input => input.Name == "CropHeightInput").Text == "3"
                && !cropPreview.IsVisible && crop.IsEffectivelyEnabled, $"reset crop after {scenario}");
            Check(string.IsNullOrEmpty(image.CropError) && !image.ShowCropPreview && !image.IsDirty && !tab.IsDirty
                && ReferenceEquals(image.ActiveDocument, originalDocument)
                && image.RedoCommand.CanExecute(null) && !image.UndoCommand.CanExecute(null)
                && image.CapturePngSnapshot().Bytes.SequenceEqual(initial) && previewChanges == 0,
                $"Reset crop after {scenario} changed pixels, history or dirty state.");
        }

        await ResetCropAndCheckAsync("invalid input");
        cropInputs.Single(input => input.Name == "CropXInput").SetCurrentValue(TextBox.TextProperty, "1");
        cropInputs.Single(input => input.Name == "CropYInput").SetCurrentValue(TextBox.TextProperty, "1");
        cropInputs.Single(input => input.Name == "CropWidthInput").SetCurrentValue(TextBox.TextProperty, "1");
        cropInputs.Single(input => input.Name == "CropHeightInput").SetCurrentValue(TextBox.TextProperty, "1");
        await WaitUntilAsync(() => image.CropX == "1" && image.CropY == "1"
            && image.CropWidth == "1" && image.CropHeight == "1" && crop.IsEffectivelyEnabled
            && cropPreview.IsVisible && cropPreview.CropRectangle == new Avalonia.Rect(1, 1, 1, 1), "valid image crop");
        Check(!cropPreview.IsHitTestVisible && !tab.IsDirty && image.RedoCommand.CanExecute(null)
            && image.CapturePngSnapshot().Bytes.SequenceEqual(initial),
            "The crop preview changed the image or its history.");
        await ResetCropAndCheckAsync("valid input");
        cropInputs.Single(input => input.Name == "CropXInput").SetCurrentValue(TextBox.TextProperty, "1");
        cropInputs.Single(input => input.Name == "CropYInput").SetCurrentValue(TextBox.TextProperty, "1");
        cropInputs.Single(input => input.Name == "CropWidthInput").SetCurrentValue(TextBox.TextProperty, "1");
        cropInputs.Single(input => input.Name == "CropHeightInput").SetCurrentValue(TextBox.TextProperty, "1");
        await WaitUntilAsync(() => cropPreview.IsVisible && cropPreview.CropRectangle == new Avalonia.Rect(1, 1, 1, 1),
            "restored image crop preview");
        image.ZoomPercentage = 175;
        await WaitUntilAsync(() =>
        {
            Avalonia.Point? start = cropPreview.TranslatePoint(new Avalonia.Point(1, 1), imageScroll);
            Avalonia.Point? end = cropPreview.TranslatePoint(new Avalonia.Point(2, 2), imageScroll);
            Avalonia.Point? origin = renderedImage.TranslatePoint(new Avalonia.Point(0, 0), imageScroll);
            return cropPreview.Zoom == 1.75 && start.HasValue && end.HasValue && origin.HasValue
                && Math.Abs(start.Value.X - origin.Value.X - 1.75) < 0.01
                && Math.Abs(start.Value.Y - origin.Value.Y - 1.75) < 0.01
                && Math.Abs(end.Value.X - start.Value.X - 1.75) < 0.01
                && Math.Abs(end.Value.Y - start.Value.Y - 1.75) < 0.01;
        }, "zoomed image crop preview geometry");
        Check(!tab.IsDirty && ReferenceEquals(image.ActiveDocument, originalDocument)
            && image.RedoCommand.CanExecute(null) && !image.UndoCommand.CanExecute(null)
            && image.CapturePngSnapshot().Bytes.SequenceEqual(initial) && previewChanges == 0,
            "The zoomed crop preview changed pixels, history or dirty state.");
        image.ZoomPercentage = 100;
        image.ContentChanging -= previewChanging;
        Invoke(tab.View, "Apply crop");
        byte[] cropped = image.CapturePngSnapshot().Bytes;
        Check(tab.IsDirty && ImageSize(image) == (1, 1), "Crop did not update the image.");
        image.UndoCommand.Execute(null);
        Check(!tab.IsDirty && image.CapturePngSnapshot().Bytes.SequenceEqual(initial),
            "One undo did not restore the image before crop.");
        image.RedoCommand.Execute(null);
        Check(tab.IsDirty && image.CapturePngSnapshot().Bytes.SequenceEqual(cropped),
            "Redo did not restore the cropped image.");
        image.UndoCommand.Execute(null);
        image.RotateRightCommand.Execute(null);
        Check(!tab.IsPreview && tab.IsDirty && ImageSize(image) == (3, 2),
            "Rotation did not pin the preview and change its dimensions.");
        image.UndoCommand.Execute(null);
        Check(!tab.IsDirty && image.CapturePngSnapshot().Bytes.SequenceEqual(initial),
            "Undo did not restore the clean original image.");
        image.RedoCommand.Execute(null);
        Check(tab.IsDirty && ImageSize(image) == (3, 2), "Redo did not restore the image edit.");

        byte[] rotated = image.CapturePngSnapshot().Bytes;
        Check(flip.IsEnabled, "The image flip button is not available.");
        Invoke(tab.View, "Flip horizontal");
        byte[] flipped = image.CapturePngSnapshot().Bytes;
        Check(tab.IsDirty && ImageSize(image) == (3, 2) && !flipped.SequenceEqual(rotated),
            "The image flip button did not change the image.");
        image.UndoCommand.Execute(null);
        Check(image.CapturePngSnapshot().Bytes.SequenceEqual(rotated),
            "Undo did not restore the image before flipping.");
        image.RedoCommand.Execute(null);
        Check(image.CapturePngSnapshot().Bytes.SequenceEqual(flipped),
            "Redo did not restore the flipped image.");
        image.UndoCommand.Execute(null);

        Check(flipVertical.IsEnabled, "The vertical image flip button is not available.");
        Invoke(tab.View, "Flip vertical");
        byte[] verticallyFlipped = image.CapturePngSnapshot().Bytes;
        Check(tab.IsDirty && ImageSize(image) == (3, 2) && !verticallyFlipped.SequenceEqual(rotated),
            "The vertical image flip button did not change the image.");
        image.UndoCommand.Execute(null);
        Check(image.CapturePngSnapshot().Bytes.SequenceEqual(rotated),
            "Undo did not restore the image before vertical flipping.");
        image.RedoCommand.Execute(null);
        Check(image.CapturePngSnapshot().Bytes.SequenceEqual(verticallyFlipped),
            "Redo did not restore the vertically flipped image.");

        Button close = window.GetVisualDescendants().OfType<Button>()
            .Single(button => ReferenceEquals(button.DataContext, tab)
                && string.Equals(button.Content?.ToString(), "x", StringComparison.Ordinal));
        close.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        await WaitUntilAsync(() => desktop.Windows.OfType<DiscardWorkbenchChangesDialog>().Any(),
            "the dirty image close prompt");
        Click(desktop.Windows.OfType<DiscardWorkbenchChangesDialog>().Single(), "Cancel");
        await WaitUntilAsync(() => !desktop.Windows.OfType<DiscardWorkbenchChangesDialog>().Any(),
            "the cancelled image close prompt");
        Check(workbench.EditorWorkspace.Tabs.Contains(tab) && tab.IsDirty,
            "Cancelling image close lost the document.");

        s_imageSavePath = null;
        await tab.SaveCommand.ExecuteAsync(null);
        Check(tab.IsDirty && WorkspacePathIdentity.AreEqual(tab.FilePath!, source),
            "Cancelling the first save changed the image state.");

        s_imageSavePath = source;
        await tab.SaveAsCommand.ExecuteAsync(null);
        Check(tab.IsDirty && (await File.ReadAllBytesAsync(source)).SequenceEqual(sourceBytes),
            "Save As overwrote the source image.");

        string first = Path.Combine(fixture, "Export.png");
        s_imageSavePath = first;
        byte[] firstSnapshot = image.CapturePngSnapshot().Bytes;
        await tab.SaveCommand.ExecuteAsync(null);
        Check(!tab.IsDirty && WorkspacePathIdentity.AreEqual(tab.FilePath!, first)
            && image.ActiveDocument?.Name == Path.GetFileName(first)
            && (await File.ReadAllBytesAsync(first)).SequenceEqual(firstSnapshot)
            && (await File.ReadAllBytesAsync(source)).SequenceEqual(sourceBytes),
            "The first PNG export did not preserve the source or the saved snapshot.");

        image.RotateRightCommand.Execute(null);
        byte[] secondSnapshot = image.CapturePngSnapshot().Bytes;
        await tab.SaveCommand.ExecuteAsync(null);
        Check(!tab.IsDirty && (await File.ReadAllBytesAsync(first)).SequenceEqual(secondSnapshot),
            "A subsequent save did not update the unchanged session-owned PNG.");

        image.RotateRightCommand.Execute(null);
        byte[] external = [0x51, 0x52, 0x53];
        await File.WriteAllBytesAsync(first, external);
        await tab.SaveCommand.ExecuteAsync(null);
        Check(tab.IsDirty && (await File.ReadAllBytesAsync(first)).SequenceEqual(external)
            && tab.Editor.NotificationText.Contains("changed externally", StringComparison.Ordinal),
            "An externally changed output was overwritten or its conflict was not shown.");

        s_imageSavePath = first;
        await tab.SaveAsCommand.ExecuteAsync(null);
        Check(tab.IsDirty && (await File.ReadAllBytesAsync(first)).SequenceEqual(external),
            "Save As overwrote an existing session output.");

        string second = Path.Combine(fixture, "Export-copy.png");
        s_imageSavePath = second;
        byte[] finalSnapshot = image.CapturePngSnapshot().Bytes;
        await tab.SaveAsCommand.ExecuteAsync(null);
        Check(!tab.IsDirty && WorkspacePathIdentity.AreEqual(tab.FilePath!, second)
            && image.ActiveDocument?.Name == Path.GetFileName(second)
            && (await File.ReadAllBytesAsync(second)).SequenceEqual(finalSnapshot)
            && (await File.ReadAllBytesAsync(first)).SequenceEqual(external)
            && (await File.ReadAllBytesAsync(source)).SequenceEqual(sourceBytes),
            "Save As did not create an independent PNG or preserve existing files.");

        image.RotateRightCommand.Execute(null);
        close.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        await WaitUntilAsync(() => desktop.Windows.OfType<DiscardWorkbenchChangesDialog>().Any(),
            "the repeated dirty image close prompt");
        Click(desktop.Windows.OfType<DiscardWorkbenchChangesDialog>().Single(), "Discard changes");
        await WaitUntilAsync(() => !workbench.EditorWorkspace.Tabs.Contains(tab), "discarding the image tab");
        Check((await File.ReadAllBytesAsync(second)).SequenceEqual(finalSnapshot),
            "Discarding unsaved image edits changed the saved output.");

        foreach (string extension in new[] { "jpg", "bmp" })
        {
            string input = Path.Combine(fixture, $"Editable.{extension}");
            byte[] original = await File.ReadAllBytesAsync(input);
            explorer.SelectedItem = FindItem(explorer.RootItems, input);
            await WaitForActiveAsync(workbench, input);
            EditorTabViewModel importedTab = workbench.ActiveEditorTab;
            Check(importedTab.View is ImageEditorControl && !importedTab.IsDiagnostic,
                $"The {extension} input did not open in the shared image editor: {(importedTab.View as TextBlock)?.Text}");
            ImageEditorViewModel imported = (ImageEditorViewModel)importedTab.View.DataContext!;
            Check(ImageSize(imported) == (2, 3), $"The {extension} input has incorrect dimensions.");
            imported.RotateRightCommand.Execute(null);
            Check(importedTab.IsDirty && ImageSize(imported) == (3, 2),
                $"The {extension} input did not rotate.");
            string png = Path.Combine(fixture, $"{extension}-export.png");
            s_imageSavePath = png;
            byte[] exported = imported.CapturePngSnapshot().Bytes;
            await importedTab.SaveCommand.ExecuteAsync(null);
            Check(!importedTab.IsDirty && File.Exists(png) && WorkspacePathIdentity.AreEqual(importedTab.FilePath!, png)
                && (await File.ReadAllBytesAsync(png)).SequenceEqual(exported)
                && (await File.ReadAllBytesAsync(input)).SequenceEqual(original),
                $"The {extension} import did not produce a separate PNG or preserve its source.");
            Check(workbench.EditorWorkspace.TryClose(importedTab), $"The saved {extension} tab did not close.");
        }

        string corrupt = Path.Combine(fixture, "Image.png");
        byte[] corruptBytes = await File.ReadAllBytesAsync(corrupt);
        explorer.SelectedItem = FindItem(explorer.RootItems, corrupt);
        await WaitForActiveAsync(workbench, corrupt);
        Check(workbench.ActiveEditorTab.IsDiagnostic && !workbench.CanEditActiveDocument
            && (await File.ReadAllBytesAsync(corrupt)).SequenceEqual(corruptBytes),
            "A corrupt image was not safely reported or its source changed.");

        await VerifyImageSaveRacesAsync(source, sourceBytes, fixture);
        Console.WriteLine("Image: Workbench PNG/JPEG/BMP import, editing, PNG export, close and conflict protection verified.");
    }

    private static (int Width, int Height) ImageSize(ImageEditorViewModel image)
    {
        using MemoryStream stream = new(image.CapturePngSnapshot().Bytes);
        using Bitmap bitmap = new(stream);
        return (bitmap.PixelSize.Width, bitmap.PixelSize.Height);
    }

    private static async Task VerifyImageDpiRenderingAsync(Window window, Control view, ImageEditorViewModel editor, byte[] original)
    {
        try
        {
            foreach (int dpi in new[] { 96, 192 })
            {
                var bitmap = new WriteableBitmap(new PixelSize(64, 48), new Vector(dpi, dpi), PixelFormats.Bgra8888, AlphaFormat.Premul);
                using (var buffer = bitmap.Lock())
                {
                    for (int y = 0; y < 48; y++)
                    {
                        byte[] row = new byte[64 * 4];
                        for (int x = 0; x < 64; x++)
                        {
                            if (y >= 24) { row[x * 4 + 1] = 255; row[x * 4 + 3] = 255; }
                            else if (x >= 40) { row[x * 4] = 255; row[x * 4 + 3] = 255; }
                            else if (x >= 24) { row[x * 4 + 2] = 128; row[x * 4 + 3] = 128; }
                        }
                        Marshal.Copy(row, 0, IntPtr.Add(buffer.Address, y * buffer.RowBytes), row.Length);
                    }
                }
                editor.LoadDocument(new ImageEditorDocument("dpi.png", bitmap));
                Grid surface = view.GetVisualDescendants().OfType<Grid>().Single(c => c.Name == "ImageSurface");
                Border background = view.GetVisualDescendants().OfType<Border>().Single(c => c.Name == "TransparencyBackground");
                await WaitUntilAsync(() => surface.Bounds.Size == new Size(64, 48) && background.IsVisible,
                    $"{dpi}-DPI image layout");
                window.UpdateLayout();
                byte[] SnapshotPixel(Bitmap image, int x, int y)
                {
                    using var copy = new WriteableBitmap(image.PixelSize, new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
                    using var buffer = copy.Lock();
                    image.CopyPixels(buffer);
                    byte[] value = new byte[4];
                    Marshal.Copy(IntPtr.Add(buffer.Address, y * buffer.RowBytes + x * 4), value, 0, 4);
                    return value;
                }
                using (var stream = new MemoryStream(editor.CapturePngSnapshot().Bytes))
                using (var decoded = new Bitmap(stream))
                {
                    Check(decoded.PixelSize == new PixelSize(64, 48)
                        && SnapshotPixel(decoded, 28, 4).SequenceEqual(new byte[] { 0, 0, 128, 128 })
                        && SnapshotPixel(decoded, 44, 36).SequenceEqual(new byte[] { 0, 255, 0, 255 }),
                        $"{dpi}-DPI PNG pixels were not retained.");
                }
                using (var rendered = new RenderTargetBitmap(new PixelSize(64, 48)))
                {
                    rendered.Render(surface);
                    Check(SnapshotPixel(rendered, 44, 4).SequenceEqual(new byte[] { 255, 0, 0, 255 })
                        && SnapshotPixel(rendered, 44, 36).SequenceEqual(new byte[] { 0, 255, 0, 255 }),
                        $"{dpi}-DPI image did not render its right and bottom pixels.");
                }
                background.SetCurrentValue(Visual.IsVisibleProperty, false);
                using (var rendered = new RenderTargetBitmap(new PixelSize(64, 48)))
                {
                    rendered.Render(surface);
                    Check(SnapshotPixel(rendered, 4, 4).SequenceEqual(new byte[] { 0, 0, 0, 0 })
                        && SnapshotPixel(rendered, 28, 4).SequenceEqual(new byte[] { 0, 0, 128, 128 }),
                        $"{dpi}-DPI image alpha was not rendered without the checkerboard.");
                }
                background.SetCurrentValue(Visual.IsVisibleProperty, true);
            }
        }
        finally
        {
            editor.LoadDocument(original, "Editable.png");
        }
    }

    private static async Task VerifyImageSaveRacesAsync(string source, byte[] sourceBytes, string fixture)
    {
        TaskCompletionSource started = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        ImageEditorViewModel image = new() { AllowClear = false };
        using (ImageDocumentSession session = new(image, async (path, png, expectedHash, token) =>
        {
            started.TrySetResult();
            await release.Task.WaitAsync(token);
            await ImageDocumentFileStorage.WriteAsync(path, png, expectedHash, token);
        }))
        {
            await session.OpenAsync(source);
            image.RotateRightCommand.Execute(null);
            byte[] snapshot = image.CapturePngSnapshot().Bytes;
            string destination = Path.Combine(fixture, "Delayed.png");
            Task saving = session.SaveAsync(destination);
            try
            {
                await started.Task.WaitAsync(TimeSpan.FromSeconds(10));
                image.RotateRightCommand.Execute(null);
            }
            finally
            {
                release.TrySetResult();
            }

            await saving;
            Check(session.IsDirty && (await File.ReadAllBytesAsync(destination)).SequenceEqual(snapshot)
                && WorkspacePathIdentity.AreEqual(session.FilePath!, destination),
                "A later image edit was incorrectly marked saved or changed the exported snapshot.");
            image.UndoCommand.Execute(null);
            Check(!session.IsDirty && image.CapturePngSnapshot().Bytes.SequenceEqual(snapshot),
                "Undo did not return to the image snapshot saved during the delayed write.");
            image.RedoCommand.Execute(null);
            Check(session.IsDirty, "Redo did not restore the later unsaved image edit.");
        }

        started = new(TaskCreationOptions.RunContinuationsAsynchronously);
        release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        image = new ImageEditorViewModel { AllowClear = false };
        using (ImageDocumentSession session = new(image, async (path, png, expectedHash, token) =>
        {
            started.TrySetResult();
            await release.Task.WaitAsync(token);
            await ImageDocumentFileStorage.WriteAsync(path, png, expectedHash, token);
        }))
        {
            await session.OpenAsync(source);
            image.RotateRightCommand.Execute(null);
            string destination = Path.Combine(fixture, "Replaced-document.png");
            Task saving = session.SaveAsync(destination);
            try
            {
                await started.Task.WaitAsync(TimeSpan.FromSeconds(10));
                image.LoadDocument(sourceBytes, "Replacement.png");
            }
            finally
            {
                release.TrySetResult();
            }

            bool rejected = false;
            try
            {
                await saving;
            }
            catch (InvalidOperationException exception) when (exception.Message.Contains("document changed", StringComparison.Ordinal))
            {
                rejected = true;
            }

            Check(rejected && File.Exists(destination) && WorkspacePathIdentity.AreEqual(session.FilePath!, source)
                && !session.IsDirty && ImageSize(image) == (2, 3),
                "A completed save adopted the path or saved state of a different image document.");
        }
    }
}
