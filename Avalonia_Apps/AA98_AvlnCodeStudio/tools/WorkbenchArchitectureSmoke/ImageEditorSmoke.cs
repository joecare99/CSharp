using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.UI.Components;
using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AvlnCodeStudio.UI.Views;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using Avln_ImageEditor.Controls.ViewModels;
using Avln_ImageEditor.Controls.Views;
using Microsoft.Extensions.DependencyInjection;
using Project.Explorer.Avalonia.ViewModels;
using System;
using System.IO;
using System.Linq;
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
            && tab.View.GetVisualDescendants().OfType<Image>().Any(element => element.Source is Bitmap)
            && tab.View.GetVisualDescendants().OfType<Button>()
                .Any(button => ReferenceEquals(button.Command, image.RotateRightCommand)),
            "the rendered image editor");
        Button[] imageButtons = tab.View.GetVisualDescendants().OfType<Button>().ToArray();
        Check(imageButtons.Any(button => ReferenceEquals(button.Command, image.UndoCommand))
            && imageButtons.Any(button => ReferenceEquals(button.Command, image.RedoCommand)),
            "The image editor did not expose its undo and redo commands.");
        Button flip = imageButtons.Single(button => ReferenceEquals(button.Command, image.FlipHorizontalCommand));
        Check(!image.AllowClear && !tab.IsDirty && ImageSize(image) == (2, 3),
            "The shared editor did not open the original image as a clean document.");

        byte[] initial = image.CapturePngSnapshot().Bytes;
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
