using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.UI.Components;
using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AvlnCodeStudio.UI.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
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
        CheckBox dragMode = tab.View.GetVisualDescendants().OfType<CheckBox>()
            .Single(element => element.Name == "DragCropMode");
        Check(dragMode.IsChecked != true && dragMode.IsEffectivelyEnabled,
            "The shared drag crop mode was not available and initially off.");
        dragMode.IsChecked = true;
        Check(!tab.IsDirty && ReferenceEquals(image.ActiveDocument, originalDocument)
            && image.CapturePngSnapshot().Bytes.SequenceEqual(initial) && previewChanges == 0,
            "Enabling drag crop changed image content.");
        dragMode.IsChecked = false;
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
        Button fit = imageButtons.Single(button => button.Name == "FitImageButton");
        Check(fit.IsEffectivelyEnabled && imageScroll.Viewport.Width > 0 && imageScroll.Viewport.Height > 0,
            "The image fit button or its viewport was unavailable.");
        double expectedFit = Math.Clamp(Math.Min(imageScroll.Viewport.Width / 2, imageScroll.Viewport.Height / 3) * 100,
            ImageEditorViewModel.MinimumZoomPercentage, ImageEditorViewModel.MaximumZoomPercentage);
        Invoke(tab.View, "Fit (min 10%)");
        await WaitUntilAsync(() => Math.Abs(image.ZoomPercentage - expectedFit) < 0.000001
            && Math.Abs(cropPreview.Zoom - image.ZoomFactor) < 0.000001,
            "fit image zoom and crop preview");
        Invoke(tab.View, "Fit (min 10%)");
        Check(Math.Abs(image.ZoomPercentage - expectedFit) < 0.000001,
            "Repeated fit changed the zoom without a viewport change.");
        Check(cropPreview.IsVisible && cropPreview.CropRectangle == new Avalonia.Rect(1, 1, 1, 1)
            && !tab.IsDirty && ReferenceEquals(image.ActiveDocument, originalDocument)
            && image.RedoCommand.CanExecute(null) && !image.UndoCommand.CanExecute(null)
            && image.CapturePngSnapshot().Bytes.SequenceEqual(initial) && previewChanges == 0,
            "Fit image changed crop state, pixels, history or dirty state.");
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
                CheckBox sampleMode = view.GetVisualDescendants().OfType<CheckBox>().Single(c => c.Name == "SampleColorMode");
                CheckBox cropMode = view.GetVisualDescendants().OfType<CheckBox>().Single(c => c.Name == "DragCropMode");
                TextBox hex = view.GetVisualDescendants().OfType<TextBox>().Single(c => c.Name == "SampledColorHexInput");
                TextBox colorDraft = view.GetVisualDescendants().OfType<TextBox>().Single(c => c.Name == "ManualColorHexInput");
                Button applyColor = view.GetVisualDescendants().OfType<Button>().Single(c => c.Name == "ApplyManualColorButton");
                Check(sampleMode.IsChecked != true && sampleMode.IsEffectivelyEnabled,
                    "The image sampling mode was not available and initially off.");
                cropMode.IsChecked = true;
                sampleMode.IsChecked = true;
                Check(cropMode.IsChecked != true, "Sampling did not disable drag crop.");
                var state = editor.CaptureDocumentState();
                byte[] beforeSampling = editor.CapturePngSnapshot().Bytes;
                int changing = 0;
                EventHandler contentChanging = (_, _) => changing++;
                editor.ContentChanging += contentChanging;
                colorDraft.Text = "#80402010";
                applyColor.Command!.Execute(null);
                Check(editor.SampledColorHex == "#80402010" && hex.Text == "#80402010",
                    "Manual color was not applied in the image tab.");
                colorDraft.Text = "#bad";
                applyColor.Command.Execute(null);
                Check(editor.SampledColorHex == "#80402010" && editor.ManualColorError.Length > 0,
                    "Invalid image-tab color did not retain the selection.");
                void SampleAt(double x, double y, string expected)
                {
                    Point windowPoint = surface.TranslatePoint(new Point(x, y), window)
                        ?? throw new InvalidOperationException("No image sampling coordinates.");
                    IInputElement target = window.InputHitTest(windowPoint)
                        ?? throw new InvalidOperationException("No image sampling input target.");
                    Check(target is Visual visual && (ReferenceEquals(visual, surface)
                        || visual.GetVisualAncestors().Contains(surface)), "Image sampling hit another control.");
                    PointerPressedEventArgs press = new(target, new Pointer(7, PointerType.Mouse, true), window,
                        windowPoint, 0,
                        new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed),
                        KeyModifiers.None, 1)
                    {
                        RoutedEvent = InputElement.PointerPressedEvent,
                    };
                    ((Interactive)target).RaiseEvent(press);
                    Check(press.Handled && editor.SampledColorHex == expected && hex.Text == expected &&
                        colorDraft.Text == expected && hex.IsReadOnly,
                        $"Image sampling did not report {expected} at {x},{y}.");
                }
                SampleAt(28.5, 4.5, "#80FF0000");
                Point otherPoint = surface.TranslatePoint(new Point(44.5, 36.5), window)
                    ?? throw new InvalidOperationException("No image sampling coordinates.");
                IInputElement otherTarget = window.InputHitTest(otherPoint)
                    ?? throw new InvalidOperationException("No image sampling input target.");
                Check(otherTarget is Visual otherVisual && (ReferenceEquals(otherVisual, surface)
                    || otherVisual.GetVisualAncestors().Contains(surface)), "Image sampling hit another control.");
                foreach (var (kind, button) in new[]
                {
                    (PointerUpdateKind.RightButtonPressed, RawInputModifiers.RightMouseButton),
                    (PointerUpdateKind.MiddleButtonPressed, RawInputModifiers.MiddleMouseButton),
                })
                {
                    var press = new PointerPressedEventArgs(otherTarget, new Pointer(7, PointerType.Mouse, true), window,
                        otherPoint, 0, new PointerPointProperties(RawInputModifiers.LeftMouseButton | button, kind),
                        KeyModifiers.None, 1)
                    {
                        RoutedEvent = InputElement.PointerPressedEvent,
                    };
                    ((Interactive)otherTarget).RaiseEvent(press);
                    Check(!press.Handled && editor.SampledColorHex == "#80FF0000",
                        "A non-left press while left was held changed the sampled color.");
                }
                SampleAt(4.5, 4.5, "#00000000");
                SampleAt(44.5, 36.5, "#FF00FF00");
                editor.ContentChanging -= contentChanging;
                Check(editor.IsCurrentDocumentState(state) && !editor.IsDirty && changing == 0
                    && editor.CapturePngSnapshot().Bytes.SequenceEqual(beforeSampling),
                    "Image sampling changed pixels, history or dirty state.");
                sampleMode.IsChecked = false;
                byte[] SnapshotPixel(Bitmap image, int x, int y)
                {
                    using var copy = new WriteableBitmap(image.PixelSize, new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
                    using var buffer = copy.Lock();
                    image.CopyPixels(buffer);
                    byte[] value = new byte[4];
                    Marshal.Copy(IntPtr.Add(buffer.Address, y * buffer.RowBytes + x * 4), value, 0, 4);
                    return value;
                }
                CheckBox brushMode = view.GetVisualDescendants().OfType<CheckBox>().Single(c => c.Name == "BrushMode");
                Check(brushMode.IsChecked != true && brushMode.IsEffectivelyEnabled,
                    "The image brush was not available and initially off.");
                sampleMode.IsChecked = true;
                brushMode.IsChecked = true;
                Check(sampleMode.IsChecked != true && cropMode.IsChecked != true,
                    "The image brush did not exclude sampling and crop.");
                Point BrushPoint(double x)
                {
                    Point point = surface.TranslatePoint(new Point(x, 4.5), window)
                        ?? throw new InvalidOperationException("No image brush coordinates.");
                    IInputElement target = window.InputHitTest(point)
                        ?? throw new InvalidOperationException("No image brush input target.");
                    Check(target is Visual visual && (ReferenceEquals(visual, surface)
                        || visual.GetVisualAncestors().Contains(surface)), "Image brush hit another control.");
                    return point;
                }
                Point start = BrushPoint(4.5);
                Point middle = BrushPoint(6.5);
                Point end = BrushPoint(8.5);
                IInputElement brushTarget = window.InputHitTest(start)!;
                Pointer brushPointer = new(17, PointerType.Mouse, true);
                int brushChanges = 0;
                EventHandler brushChanging = (_, _) => brushChanges++;
                editor.ContentChanging += brushChanging;
                PointerPressedEventArgs brushPress = new(brushTarget, brushPointer, window, start, 0,
                    new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed),
                    KeyModifiers.None, 1)
                {
                    RoutedEvent = InputElement.PointerPressedEvent,
                };
                ((Interactive)brushTarget).RaiseEvent(brushPress);
                Check(brushPress.Handled && ReferenceEquals(brushPointer.Captured, surface),
                    "The image brush did not capture the left press.");
                PointerEventArgs brushMove = new(InputElement.PointerMovedEvent, surface, brushPointer, window, middle, 0,
                    new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.Other), KeyModifiers.None);
                surface.RaiseEvent(brushMove);
                Check(brushMove.Handled && brushChanges == 0 && editor.IsCurrentDocumentState(state) && !editor.IsDirty
                    && editor.CapturePngSnapshot().Bytes.SequenceEqual(beforeSampling),
                    "The pending brush gesture changed the document.");
                PointerReleasedEventArgs brushRelease = new(surface, brushPointer, window, end, 0,
                    new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonReleased),
                    KeyModifiers.None, MouseButton.Left)
                {
                    RoutedEvent = InputElement.PointerReleasedEvent,
                };
                surface.RaiseEvent(brushRelease);
                Check(brushRelease.Handled && brushPointer.Captured is null && brushChanges == 1 && editor.IsDirty
                    && editor.UndoCommand.CanExecute(null), "The image brush did not commit on release.");
                byte[] painted = editor.CapturePngSnapshot().Bytes;
                using (var stream = new MemoryStream(painted))
                using (var decoded = new Bitmap(stream))
                {
                    for (int x = 4; x <= 8; x++)
                        Check(SnapshotPixel(decoded, x, 4).SequenceEqual(new byte[] { 0, 255, 0, 255 }),
                            $"The image brush missed pixel {x},4.");
                    Check(SnapshotPixel(decoded, 3, 4).SequenceEqual(new byte[] { 0, 0, 0, 0 })
                        && SnapshotPixel(decoded, 9, 4).SequenceEqual(new byte[] { 0, 0, 0, 0 }),
                        "The image brush painted beyond the stroke.");
                }
                editor.Undo();
                Check(editor.IsCurrentDocumentState(state) && !editor.IsDirty && !editor.UndoCommand.CanExecute(null)
                    && editor.CapturePngSnapshot().Bytes.SequenceEqual(beforeSampling),
                    "One undo did not restore the image before the brush stroke.");
                editor.Redo();
                Check(editor.IsDirty && editor.CapturePngSnapshot().Bytes.SequenceEqual(painted),
                    "Redo did not restore the image brush stroke.");
                editor.Undo();
                Check(editor.IsCurrentDocumentState(state) && !editor.IsDirty && brushChanges == 4,
                    "Undo did not restore the clean image after brush redo.");
                editor.ContentChanging -= brushChanging;

                TextBox sizeInput = view.GetVisualDescendants().OfType<TextBox>().Single(c => c.Name == "BrushSizeInput");
                Check(sizeInput.Focus(), "The brush size input did not receive focus.");
                sizeInput.SelectAll();
                sizeInput.RaiseEvent(new TextInputEventArgs { RoutedEvent = InputElement.TextInputEvent, Text = "3" });
                Check(editor.BrushSizeText == "3" && string.IsNullOrEmpty(editor.BrushSizeError),
                    "The brush size input did not update its binding.");
                brushMode.IsChecked = true;
                window.UpdateLayout();
                start = BrushPoint(4.5); end = BrushPoint(8.5);
                brushTarget = window.InputHitTest(start)!;
                brushPointer = new Pointer(22, PointerType.Mouse, true);
                var sizedPress = new PointerPressedEventArgs(brushTarget, brushPointer, window, start, 0,
                    new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed),
                    KeyModifiers.None, 1)
                { RoutedEvent = InputElement.PointerPressedEvent };
                ((Interactive)brushTarget).RaiseEvent(sizedPress);
                Check(sizedPress.Handled && ReferenceEquals(brushPointer.Captured, surface),
                    "The sized brush did not capture its press.");
                surface.RaiseEvent(new PointerEventArgs(InputElement.PointerMovedEvent, surface, brushPointer, window, end, 0,
                    new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.Other), KeyModifiers.None));
                surface.RaiseEvent(new PointerReleasedEventArgs(surface, brushPointer, window, end, 0,
                    new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonReleased),
                    KeyModifiers.None, MouseButton.Left)
                { RoutedEvent = InputElement.PointerReleasedEvent });
                byte[] sizedPaint = editor.CapturePngSnapshot().Bytes;
                using (var stream = new MemoryStream(sizedPaint))
                using (var decoded = new Bitmap(stream))
                    for (int y = 3; y <= 5; y++)
                        for (int x = 3; x <= 9; x++)
                            Check(SnapshotPixel(decoded, x, y).SequenceEqual(new byte[] { 0, 255, 0, 255 }),
                                $"The sized brush missed pixel {x},{y}.");
                editor.Undo();
                Check(editor.IsCurrentDocumentState(state) && editor.CapturePngSnapshot().Bytes.SequenceEqual(beforeSampling),
                    "Undo did not restore the sized brush stroke.");
                editor.Redo();
                Check(editor.CapturePngSnapshot().Bytes.SequenceEqual(sizedPaint), "Redo did not restore the sized brush stroke.");
                editor.Undo();
                CheckBox lineMode = view.GetVisualDescendants().OfType<CheckBox>().Single(c => c.Name == "LineMode");
                lineMode.IsChecked = true;
                Check(editor.SelectedTool == ImageEditorTool.Line && brushMode.IsChecked != true,
                    "The line tool was not selected exclusively.");
                brushPointer = new Pointer(27, PointerType.Mouse, true);
                var linePress = new PointerPressedEventArgs(brushTarget, brushPointer, window, start, 0,
                    new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed),
                    KeyModifiers.None, 1)
                { RoutedEvent = InputElement.PointerPressedEvent };
                ((Interactive)brushTarget).RaiseEvent(linePress);
                Check(linePress.Handled && ReferenceEquals(brushPointer.Captured, surface),
                    "The line did not capture its press.");
                Point offLine = surface.TranslatePoint(new Point(6.5, 10.5), window)!.Value;
                surface.RaiseEvent(new PointerEventArgs(InputElement.PointerMovedEvent, surface, brushPointer, window, offLine, 0,
                    new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.Other), KeyModifiers.None));
                Check(editor.IsCurrentDocumentState(state), "The pending line changed the image.");
                surface.RaiseEvent(new PointerReleasedEventArgs(surface, brushPointer, window, end, 0,
                    new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonReleased),
                    KeyModifiers.None, MouseButton.Left)
                { RoutedEvent = InputElement.PointerReleasedEvent });
                byte[] linePaint = editor.CapturePngSnapshot().Bytes;
                Check(linePaint.SequenceEqual(sizedPaint), "The line did not use only its endpoints and selected size.");
                editor.Undo();
                Check(editor.IsCurrentDocumentState(state), "Undo did not restore the line source.");
                editor.Redo();
                Check(editor.CapturePngSnapshot().Bytes.SequenceEqual(linePaint), "Redo did not restore the line.");
                editor.Undo();
                sizeInput.SetCurrentValue(TextBox.TextProperty, "1");

                void EnterManualColor(string value)
                {
                    Check(colorDraft.Focus(), "The image color field did not receive focus.");
                    colorDraft.SelectAll();
                    var input = new TextInputEventArgs { RoutedEvent = InputElement.TextInputEvent, Text = value };
                    colorDraft.RaiseEvent(input);
                    Check(colorDraft.Text == value && editor.ManualColorHex == value,
                        "The image color field did not receive text input through its binding.");
                    var enter = new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Enter };
                    colorDraft.RaiseEvent(enter);
                    Check(enter.Handled && editor.SampledColorHex == value && hex.Text == value,
                        "Enter did not apply the entered image color.");
                }

                EnterManualColor("#FF3157B9");
                window.UpdateLayout();
                brushMode.IsChecked = true;
                start = BrushPoint(4.5);
                end = BrushPoint(8.5);
                brushTarget = window.InputHitTest(start)!;
                brushPointer = new Pointer(19, PointerType.Mouse, true);
                var manualPress = new PointerPressedEventArgs(brushTarget, brushPointer, window, start, 0,
                    new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed),
                    KeyModifiers.None, 1)
                { RoutedEvent = InputElement.PointerPressedEvent };
                ((Interactive)brushTarget).RaiseEvent(manualPress);
                Check(manualPress.Handled && ReferenceEquals(brushPointer.Captured, surface),
                    "The manual-color brush did not capture its hit-tested press.");
                var manualMove = new PointerEventArgs(InputElement.PointerMovedEvent, surface, brushPointer, window, end, 0,
                    new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.Other), KeyModifiers.None);
                surface.RaiseEvent(manualMove);
                Check(editor.IsCurrentDocumentState(state) && !editor.IsDirty &&
                    editor.CapturePngSnapshot().Bytes.SequenceEqual(beforeSampling),
                    "The pending manual-color stroke changed the image.");
                var manualRelease = new PointerReleasedEventArgs(surface, brushPointer, window, end, 0,
                    new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonReleased),
                    KeyModifiers.None, MouseButton.Left)
                { RoutedEvent = InputElement.PointerReleasedEvent };
                surface.RaiseEvent(manualRelease);
                Check(manualRelease.Handled && editor.IsDirty && editor.UndoCommand.CanExecute(null),
                    "The manual-color stroke did not commit on release.");
                byte[] manualPainted = editor.CapturePngSnapshot().Bytes;
                using (var stream = new MemoryStream(manualPainted))
                using (var decoded = new Bitmap(stream))
                    for (int x = 4; x <= 8; x++)
                        Check(SnapshotPixel(decoded, x, 4).SequenceEqual(new byte[] { 185, 87, 49, 255 }),
                            $"The manual-color brush missed pixel {x},4.");
                editor.Undo();
                Check(editor.IsCurrentDocumentState(state) && !editor.IsDirty &&
                    editor.CapturePngSnapshot().Bytes.SequenceEqual(beforeSampling),
                    "Undo did not restore the image before manual-color painting.");
                editor.Redo();
                Check(editor.IsDirty && editor.CapturePngSnapshot().Bytes.SequenceEqual(manualPainted),
                    "Redo did not restore the manual-color stroke.");
                editor.Undo();

                brushMode.IsChecked = true;
                window.UpdateLayout();
                start = BrushPoint(4.5);
                brushTarget = window.InputHitTest(start)!;
                brushPointer = new Pointer(20, PointerType.Mouse, true);
                var cancelledPress = new PointerPressedEventArgs(brushTarget, brushPointer, window, start, 0,
                    new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed),
                    KeyModifiers.None, 1)
                { RoutedEvent = InputElement.PointerPressedEvent };
                ((Interactive)brushTarget).RaiseEvent(cancelledPress);
                Check(cancelledPress.Handled && ReferenceEquals(brushPointer.Captured, surface),
                    "The pending manual-color stroke did not capture its press.");
                EnterManualColor("#FF3A4B5C");
                Check(brushPointer.Captured is null && editor.IsCurrentDocumentState(state) && !editor.IsDirty &&
                    editor.RedoCommand.CanExecute(null) && editor.CapturePngSnapshot().Bytes.SequenceEqual(beforeSampling),
                    "Changing the manual color did not cancel the pending stroke safely.");
                var cancelledRelease = new PointerReleasedEventArgs(surface, brushPointer, window, start, 0,
                    new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonReleased),
                    KeyModifiers.None, MouseButton.Left)
                { RoutedEvent = InputElement.PointerReleasedEvent };
                surface.RaiseEvent(cancelledRelease);
                Check(editor.IsCurrentDocumentState(state) && editor.RedoCommand.CanExecute(null) &&
                    editor.CapturePngSnapshot().Bytes.SequenceEqual(beforeSampling),
                    "A cancelled manual-color stroke changed the image on release.");

                CheckBox fillMode = view.GetVisualDescendants().OfType<CheckBox>().Single(c => c.Name == "FillMode");
                Check(fillMode.IsChecked != true && fillMode.IsEffectivelyEnabled,
                    "The image fill was not available and initially off.");
                EnterManualColor("#80402010");
                fillMode.IsChecked = true;
                Check(brushMode.IsChecked != true && sampleMode.IsChecked != true && cropMode.IsChecked != true,
                    "The fill did not exclude other modes.");
                window.UpdateLayout();
                Point fillPoint = BrushPoint(4.5);
                IInputElement fillTarget = window.InputHitTest(fillPoint)!;
                Pointer fillPointer = new(21, PointerType.Mouse, true);
                var fillPress = new PointerPressedEventArgs(fillTarget, fillPointer, window, fillPoint, 0,
                    new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed),
                    KeyModifiers.None, 1)
                { RoutedEvent = InputElement.PointerPressedEvent };
                ((Interactive)fillTarget).RaiseEvent(fillPress);
                Check(fillPress.Handled && ReferenceEquals(fillPointer.Captured, surface) &&
                    editor.IsCurrentDocumentState(state), "Fill press did not capture or changed pixels prematurely.");
                var fillRelease = new PointerReleasedEventArgs(surface, fillPointer, window, fillPoint, 0,
                    new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonReleased),
                    KeyModifiers.None, MouseButton.Left)
                { RoutedEvent = InputElement.PointerReleasedEvent };
                surface.RaiseEvent(fillRelease);
                Check(fillRelease.Handled && fillPointer.Captured is null,
                    "Fill release did not end the gesture.");
                await WaitUntilAsync(() => editor.IsDirty, "image fill completion");
                Check(editor.IsDirty &&
                    editor.UndoCommand.CanExecute(null), "Fill did not commit after release.");
                byte[] filled = editor.CapturePngSnapshot().Bytes;
                using (var stream = new MemoryStream(filled))
                using (var decoded = new Bitmap(stream))
                    Check(SnapshotPixel(decoded, 4, 4).SequenceEqual(new byte[] { 8, 16, 32, 128 }) &&
                        SnapshotPixel(decoded, 23, 23).SequenceEqual(new byte[] { 8, 16, 32, 128 }) &&
                        SnapshotPixel(decoded, 24, 4).SequenceEqual(new byte[] { 0, 0, 128, 128 }) &&
                        SnapshotPixel(decoded, 4, 24).SequenceEqual(new byte[] { 0, 255, 0, 255 }),
                        "Fill PNG pixels did not match the connected region.");
                editor.Undo();
                Check(editor.IsCurrentDocumentState(state) && editor.CapturePngSnapshot().Bytes.SequenceEqual(beforeSampling),
                    "Fill undo did not restore the original image.");
                editor.Redo();
                Check(editor.IsDirty && editor.CapturePngSnapshot().Bytes.SequenceEqual(filled),
                    "Fill redo did not restore its PNG.");
                editor.Undo();

                CheckBox eraserMode = view.GetVisualDescendants().OfType<CheckBox>().Single(c => c.Name == "EraserMode");
                Check(eraserMode.IsChecked != true && eraserMode.IsEffectivelyEnabled,
                    "The image eraser was not available and initially off.");
                eraserMode.IsChecked = true;
                Check(brushMode.IsChecked != true && sampleMode.IsChecked != true && cropMode.IsChecked != true,
                    "The eraser did not exclude other modes.");
                Point eraseStart = BrushPoint(28.5);
                Point eraseEnd = BrushPoint(30.5);
                IInputElement eraseTarget = window.InputHitTest(eraseStart)!;
                Pointer erasePointer = new(18, PointerType.Mouse, true);
                PointerPressedEventArgs erasePress = new(eraseTarget, erasePointer, window, eraseStart, 0,
                    new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed),
                    KeyModifiers.None, 1)
                { RoutedEvent = InputElement.PointerPressedEvent };
                ((Interactive)eraseTarget).RaiseEvent(erasePress);
                Check(erasePress.Handled && ReferenceEquals(erasePointer.Captured, surface),
                    "The eraser did not capture the hit-tested press.");
                PointerEventArgs eraseMove = new(InputElement.PointerMovedEvent, surface, erasePointer, window, eraseEnd, 0,
                    new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.Other), KeyModifiers.None);
                surface.RaiseEvent(eraseMove);
                Check(editor.IsCurrentDocumentState(state) && !editor.IsDirty
                    && editor.CapturePngSnapshot().Bytes.SequenceEqual(beforeSampling),
                    "The pending eraser changed the image.");
                PointerReleasedEventArgs eraseRelease = new(surface, erasePointer, window, eraseEnd, 0,
                    new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonReleased),
                    KeyModifiers.None, MouseButton.Left)
                { RoutedEvent = InputElement.PointerReleasedEvent };
                surface.RaiseEvent(eraseRelease);
                Check(eraseRelease.Handled && erasePointer.Captured is null && editor.IsDirty,
                    "The eraser did not commit on release.");
                byte[] erased = editor.CapturePngSnapshot().Bytes;
                using (var stream = new MemoryStream(erased))
                using (var decoded = new Bitmap(stream))
                {
                    for (int x = 28; x <= 30; x++)
                        Check(SnapshotPixel(decoded, x, 4).SequenceEqual(new byte[] { 0, 0, 0, 0 }),
                            $"The eraser missed pixel {x},4.");
                    Check(SnapshotPixel(decoded, 27, 4).SequenceEqual(new byte[] { 0, 0, 128, 128 }),
                        "The eraser changed an adjacent pixel.");
                }
                editor.Undo();
                Check(editor.IsCurrentDocumentState(state) && !editor.IsDirty
                    && editor.CapturePngSnapshot().Bytes.SequenceEqual(beforeSampling),
                    "Undo did not restore the image before erasing.");
                editor.Redo();
                Check(editor.CapturePngSnapshot().Bytes.SequenceEqual(erased), "Redo did not restore the erased image.");
                editor.Undo();
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
