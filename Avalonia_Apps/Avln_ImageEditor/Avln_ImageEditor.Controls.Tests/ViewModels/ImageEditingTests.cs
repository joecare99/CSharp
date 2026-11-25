using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avln_ImageEditor.Controls.Models;
using Avln_ImageEditor.Controls.ViewModels;
using Avln_ImageEditor.Host.ViewModels;
using Avln_ImageEditor.Host.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avln_ImageEditor.Controls.Tests.ViewModels;

[TestClass]
public sealed class ImageEditingTests
{
    private static HeadlessUnitTestSession _session = null!;

    [AssemblyInitialize]
    public static void Initialize(TestContext context)
    {
        _session = HeadlessUnitTestSession.StartNew(typeof(HeadlessTestEntry), AvaloniaTestIsolationLevel.PerAssembly);
    }

    private static readonly byte[] Pixels =
    {
        0, 0, 255, 255, 0, 255, 0, 255, 0, 0, 0, 0,
        255, 0, 0, 255, 0, 128, 128, 128, 255, 255, 255, 255
    };

    private static readonly byte[] FlippedPixels =
    {
        0, 0, 0, 0, 0, 255, 0, 255, 0, 0, 255, 255,
        255, 255, 255, 255, 0, 128, 128, 128, 255, 0, 0, 255
    };

    private static readonly byte[] VerticallyFlippedPixels =
    {
        255, 0, 0, 255, 0, 128, 128, 128, 255, 255, 255, 255,
        0, 0, 255, 255, 0, 255, 0, 255, 0, 0, 0, 0
    };

    private static readonly byte[] LeftRotatedPixels =
    {
        0, 0, 0, 0, 255, 255, 255, 255,
        0, 255, 0, 255, 0, 128, 128, 128,
        0, 0, 255, 255, 255, 0, 0, 255
    };

    private static readonly byte[] Rotated180Pixels =
    {
        255, 255, 255, 255, 0, 128, 128, 128, 255, 0, 0, 255,
        0, 0, 0, 0, 0, 255, 0, 255, 0, 0, 255, 255
    };

    private static ImageEditorDocument MakeDocument()
    {
        var bitmap = new WriteableBitmap(new PixelSize(3, 2), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
        using (var buffer = bitmap.Lock())
            for (int y = 0; y < 2; y++)
                Marshal.Copy(Pixels, y * 12, IntPtr.Add(buffer.Address, y * buffer.RowBytes), 12);
        return new ImageEditorDocument("original.png", bitmap);
    }

    private static byte[] ReadPixels(ImageEditorDocument document)
    {
        var size = document.PixelSize;
        using var bitmap = new WriteableBitmap(size, new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
        byte[] result = new byte[size.Width * size.Height * 4];
        using var buffer = bitmap.Lock();
        ((Bitmap)document.Image).CopyPixels(buffer);
        for (int y = 0; y < size.Height; y++)
            Marshal.Copy(IntPtr.Add(buffer.Address, y * buffer.RowBytes), result, y * size.Width * 4, size.Width * 4);
        return result;
    }

    [TestMethod]
    public async Task CropPreservesPixelsAlphaHistoryAndPngSnapshot() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        editor.LoadDocument(MakeDocument());
        int changes = 0;
        editor.ContentChanging += (_, _) => changes++;
        editor.CropX = "1";
        editor.CropY = "1";
        editor.CropWidth = "2";
        editor.CropHeight = "1";
        Assert.IsTrue(editor.ApplyCropCommand.CanExecute(null));
        editor.ApplyCropCommand.Execute(null);
        byte[] expected = { 0, 128, 128, 128, 255, 255, 255, 255 };
        Assert.AreEqual(new PixelSize(2, 1), editor.ActiveDocument!.PixelSize);
        CollectionAssert.AreEqual(expected, ReadPixels(editor.ActiveDocument));
        Assert.AreEqual("2", editor.CropWidth);
        Assert.AreEqual("1", editor.CropHeight);
        Assert.IsTrue(editor.IsDirty);
        var snapshot = editor.CapturePngSnapshot();
        using (var decoded = new ImageEditorViewModel())
        {
            decoded.LoadDocument(snapshot.Bytes, "crop.png");
            Assert.AreEqual(new PixelSize(2, 1), decoded.ActiveDocument!.PixelSize);
            CollectionAssert.AreEqual(expected, ReadPixels(decoded.ActiveDocument));
        }
        editor.MarkSaved(snapshot);
        Assert.IsFalse(editor.IsDirty);
        editor.Undo();
        Assert.IsTrue(editor.IsDirty);
        Assert.AreEqual("3", editor.CropWidth);
        CollectionAssert.AreEqual(Pixels, ReadPixels(editor.ActiveDocument!));
        Assert.IsFalse(editor.UndoCommand.CanExecute(null));
        editor.Redo();
        Assert.IsFalse(editor.IsDirty);
        CollectionAssert.AreEqual(expected, ReadPixels(editor.ActiveDocument!));
        Assert.AreEqual(3, changes);
    }, CancellationToken.None);

    [TestMethod]
    public async Task HostExportsCropAndTracksSavedRevision() =>
        await _session.Dispatch(async () =>
    {
        string directory = Path.Combine(AppContext.BaseDirectory, "editor-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        using var host = new MainWindowViewModel();
        try
        {
            host.Editor.LoadDocument(MakeDocument());
            host.Editor.CropX = "1";
            host.Editor.CropY = "1";
            host.Editor.CropWidth = "2";
            host.Editor.CropHeight = "1";
            host.Editor.ApplyCrop();
            string path = Path.Combine(directory, "crop.png");
            await host.ExportPngToNewFileAsync(path);
            Assert.IsFalse(host.Editor.IsDirty);
            using var decoded = new ImageEditorViewModel();
            decoded.LoadDocument(File.ReadAllBytes(path), "crop.png");
            CollectionAssert.AreEqual(new byte[] { 0, 128, 128, 128, 255, 255, 255, 255 }, ReadPixels(decoded.ActiveDocument!));
            host.Editor.Undo();
            Assert.IsTrue(host.Editor.IsDirty);
            host.Editor.Redo();
            Assert.IsFalse(host.Editor.IsDirty);
        }
        finally { Directory.Delete(directory, recursive: true); }
        return true;
    }, CancellationToken.None);

    [TestMethod]
    public async Task CropRejectsInvalidInputsAndWholeImagePreservesRedo() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        Assert.IsFalse(editor.ApplyCropCommand.CanExecute(null));
        editor.LoadDocument(MakeDocument());
        editor.FlipHorizontal();
        editor.Undo();
        var original = editor.ActiveDocument;
        int changes = 0;
        editor.ContentChanging += (_, _) => changes++;
        var cases = new[] {
            ("-1", "0", "1", "1"), ("0", "-1", "1", "1"),
            ("0", "0", "0", "1"), ("0", "0", "1", "-1"),
            ("3", "0", "1", "1"), ("0", "2", "1", "1"),
            ("2", "0", "2", "1"), ("0", "1", "1", "2"),
            ("2147483647", "0", "2147483647", "1"),
            ("0", "0", "2147483648", "1"), ("", "0", "1", "1"),
            ("1.5", "0", "1", "1")
        };
        foreach (var (x, y, width, height) in cases)
        {
            editor.CropX = x; editor.CropY = y; editor.CropWidth = width; editor.CropHeight = height;
            Assert.IsFalse(editor.ApplyCropCommand.CanExecute(null), $"{x},{y},{width},{height}");
            Assert.IsFalse(string.IsNullOrEmpty(editor.CropError));
            editor.ApplyCrop();
            Assert.AreSame(original, editor.ActiveDocument);
            Assert.IsFalse(editor.IsDirty);
            Assert.IsTrue(editor.RedoCommand.CanExecute(null));
        }
        editor.CropX = "0"; editor.CropY = "0"; editor.CropWidth = "3"; editor.CropHeight = "2";
        Assert.IsTrue(editor.ApplyCropCommand.CanExecute(null));
        editor.ApplyCrop();
        Assert.AreSame(original, editor.ActiveDocument);
        Assert.IsTrue(editor.RedoCommand.CanExecute(null));
        Assert.AreEqual(0, changes);
        editor.CropX = "999";
        editor.LoadDocument(MakeDocument());
        Assert.AreEqual("0", editor.CropX);
        Assert.AreEqual("3", editor.CropWidth);
        editor.CropX = "999";
        editor.RotateRight();
        Assert.AreEqual("0", editor.CropX);
        Assert.AreEqual("2", editor.CropWidth);
        Assert.AreEqual("3", editor.CropHeight);
        editor.CropX = "2"; editor.CropY = "0"; editor.CropWidth = "1"; editor.CropHeight = "1";
        editor.Undo();
        editor.CropX = "2"; editor.CropY = "0"; editor.CropWidth = "1"; editor.CropHeight = "1";
        editor.ApplyCrop();
        CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, ReadPixels(editor.ActiveDocument!));
    }, CancellationToken.None);

    [TestMethod]
    public async Task StandaloneCropInputsAndApplyAreBoundToEditor() =>
        await _session.Dispatch(() =>
    {
        var host = new MainWindowViewModel();
        var window = new MainWindow { DataContext = host, Width = 1500, Height = 900 };
        try
        {
            host.Editor.LoadDocument(MakeDocument());
            window.Show();
            window.UpdateLayout();
            var inputs = window.GetVisualDescendants().OfType<TextBox>().Where(t => t.Name?.StartsWith("Crop", StringComparison.Ordinal) == true).ToArray();
            Assert.AreEqual(4, inputs.Length);
            var button = window.GetVisualDescendants().OfType<Button>().Single(b => Equals(b.Content, "Apply crop"));
            inputs.Single(t => t.Name == "CropXInput").SetCurrentValue(TextBox.TextProperty, "-1");
            Dispatcher.UIThread.RunJobs();
            Assert.AreEqual("-1", host.Editor.CropX);
            Assert.IsFalse(host.Editor.ApplyCropCommand.CanExecute(null));
            Assert.IsFalse(button.IsEffectivelyEnabled);
            Assert.IsFalse(string.IsNullOrEmpty(host.Editor.CropError));
            Assert.IsTrue(window.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == host.Editor.CropError && t.IsVisible));
            inputs.Single(t => t.Name == "CropXInput").SetCurrentValue(TextBox.TextProperty, "1");
            inputs.Single(t => t.Name == "CropYInput").SetCurrentValue(TextBox.TextProperty, "1");
            inputs.Single(t => t.Name == "CropWidthInput").SetCurrentValue(TextBox.TextProperty, "1");
            inputs.Single(t => t.Name == "CropHeightInput").SetCurrentValue(TextBox.TextProperty, "1");
            Dispatcher.UIThread.RunJobs();
            Assert.IsTrue(button.IsEffectivelyEnabled);
            Click(window, "Apply crop");
            CollectionAssert.AreEqual(new byte[] { 0, 128, 128, 128 }, ReadPixels(host.Editor.ActiveDocument!));
            Click(window, "Undo");
            CollectionAssert.AreEqual(Pixels, ReadPixels(host.Editor.ActiveDocument!));
        }
        finally
        {
            host.Editor.MarkSaved(host.Editor.CapturePngSnapshot());
            window.Close();
            host.Editor.Dispose();
        }
    }, CancellationToken.None);

    [TestMethod]
    public async Task RotateUndoRedoPreservePixelsAndNotifyBeforeMutation() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        editor.LoadDocument(MakeDocument());
        int events = 0;
        editor.ContentChanging += (_, _) => { events++; Assert.AreEqual(events == 2 ? 2 : 3, editor.ActiveDocument!.PixelSize.Width); };
        editor.RotateRight();
        Assert.AreEqual(new PixelSize(2, 3), editor.ActiveDocument!.PixelSize);
        CollectionAssert.AreEqual(new byte[] {
            255,0,0,255, 0,0,255,255,
            0,128,128,128, 0,255,0,255,
            255,255,255,255, 0,0,0,0
        }, ReadPixels(editor.ActiveDocument));
        Assert.IsTrue(editor.IsDirty);
        editor.Undo();
        Assert.IsFalse(editor.IsDirty);
        CollectionAssert.AreEqual(Pixels, ReadPixels(editor.ActiveDocument!));
        editor.Redo();
        Assert.IsTrue(editor.IsDirty);
        Assert.AreEqual(3, events);
    }, CancellationToken.None);

    [TestMethod]
    public async Task Rotate180IsOneHistoryStepAndExportsTransparentPixels() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        int commandChanges = 0;
        editor.Rotate180Command.CanExecuteChanged += (_, _) => commandChanges++;
        Assert.IsFalse(editor.Rotate180Command.CanExecute(null));
        editor.Rotate180();
        editor.LoadDocument(MakeDocument());
        Assert.IsTrue(editor.Rotate180Command.CanExecute(null));
        Assert.IsTrue(commandChanges > 0);
        var original = editor.ActiveDocument;
        int events = 0;
        editor.ContentChanging += (_, _) =>
        {
            CollectionAssert.AreEqual(events == 1 ? Rotated180Pixels : Pixels, ReadPixels(editor.ActiveDocument!));
            events++;
        };
        editor.Rotate180Command.Execute(null);
        Assert.AreEqual(new PixelSize(3, 2), editor.ActiveDocument!.PixelSize);
        CollectionAssert.AreEqual(Rotated180Pixels, ReadPixels(editor.ActiveDocument));
        Assert.IsTrue(editor.IsDirty);
        using (var decoded = new ImageEditorViewModel())
        {
            decoded.LoadDocument(editor.CapturePngSnapshot().Bytes, "rotated.png");
            Assert.AreEqual(new PixelSize(3, 2), decoded.ActiveDocument!.PixelSize);
            CollectionAssert.AreEqual(Rotated180Pixels, ReadPixels(decoded.ActiveDocument));
        }
        editor.Undo();
        Assert.AreSame(original, editor.ActiveDocument);
        Assert.IsFalse(editor.IsDirty);
        Assert.IsFalse(editor.UndoCommand.CanExecute(null));
        CollectionAssert.AreEqual(Pixels, ReadPixels(editor.ActiveDocument!));
        editor.Redo();
        Assert.IsTrue(editor.IsDirty);
        CollectionAssert.AreEqual(Rotated180Pixels, ReadPixels(editor.ActiveDocument!));
        Assert.AreEqual(3, events);
        editor.ClearDocument();
        Assert.IsFalse(editor.Rotate180Command.CanExecute(null));
    }, CancellationToken.None);

    [TestMethod]
    public async Task HostExportsRotated180PngAndTracksSavedRevision() =>
        await _session.Dispatch(async () =>
    {
        string directory = Path.Combine(AppContext.BaseDirectory, "editor-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var host = new MainWindowViewModel();
        try
        {
            host.Editor.LoadDocument(MakeDocument());
            host.Editor.Rotate180();
            string path = Path.Combine(directory, "rotated.png");
            await host.ExportPngToNewFileAsync(path);
            Assert.IsFalse(host.Editor.IsDirty);
            using var decoded = new ImageEditorViewModel();
            decoded.LoadDocument(File.ReadAllBytes(path), "rotated.png");
            Assert.AreEqual(new PixelSize(3, 2), decoded.ActiveDocument!.PixelSize);
            CollectionAssert.AreEqual(Rotated180Pixels, ReadPixels(decoded.ActiveDocument));
            host.Editor.Undo();
            Assert.IsTrue(host.Editor.IsDirty);
            host.Editor.Redo();
            Assert.IsFalse(host.Editor.IsDirty);
        }
        finally
        {
            host.Editor.Dispose();
            Directory.Delete(directory, recursive: true);
        }
        return true;
    }, CancellationToken.None);

    [TestMethod]
    public async Task Rotate180SymmetricNoOpPreservesDirtyStateAndRedo() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        var image = new WriteableBitmap(new PixelSize(3, 2), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
        byte[] symmetric = {
            0, 0, 255, 255, 0, 128, 128, 128, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 128, 128, 128, 0, 0, 255, 255
        };
        using (var buffer = image.Lock())
            for (int y = 0; y < 2; y++)
                Marshal.Copy(symmetric, y * 12, IntPtr.Add(buffer.Address, y * buffer.RowBytes), 12);
        editor.LoadDocument(new ImageEditorDocument("symmetric.png", image));
        int events = 0;
        editor.ContentChanging += (_, _) => events++;
        var original = editor.ActiveDocument;
        editor.Rotate180();
        Assert.AreSame(original, editor.ActiveDocument);
        Assert.IsFalse(editor.IsDirty);
        Assert.IsFalse(editor.UndoCommand.CanExecute(null));
        Assert.AreEqual(0, events);
        editor.FlipHorizontal();
        var flipped = editor.ActiveDocument;
        var flippedPixels = ReadPixels(flipped!);
        editor.Rotate180();
        Assert.AreSame(flipped, editor.ActiveDocument);
        Assert.IsTrue(editor.IsDirty);
        Assert.AreEqual(1, events);
        editor.Undo();
        editor.Rotate180Command.Execute(null);
        Assert.AreSame(original, editor.ActiveDocument);
        Assert.IsFalse(editor.IsDirty);
        Assert.IsTrue(editor.RedoCommand.CanExecute(null));
        Assert.AreEqual(2, events);
        CollectionAssert.AreEqual(symmetric, ReadPixels(editor.ActiveDocument!));
        editor.Redo();
        Assert.AreSame(flipped, editor.ActiveDocument);
        Assert.IsTrue(editor.IsDirty);
        CollectionAssert.AreEqual(flippedPixels, ReadPixels(editor.ActiveDocument!));
        Assert.AreEqual(3, events);
    }, CancellationToken.None);

    [TestMethod]
    public async Task RotateLeftPreservesPixelsHistoryAndSnapshotRevision() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        Assert.IsFalse(editor.RotateLeftCommand.CanExecute(null));
        editor.RotateLeft();
        editor.LoadDocument(MakeDocument());
        Assert.IsTrue(editor.RotateLeftCommand.CanExecute(null));
        var original = editor.CapturePngSnapshot();
        int events = 0;
        editor.ContentChanging += (_, _) =>
        {
            CollectionAssert.AreEqual(events == 1 ? LeftRotatedPixels : Pixels, ReadPixels(editor.ActiveDocument!));
            events++;
        };
        editor.RotateLeftCommand.Execute(null);
        Assert.AreEqual(new PixelSize(2, 3), editor.ActiveDocument!.PixelSize);
        CollectionAssert.AreEqual(LeftRotatedPixels, ReadPixels(editor.ActiveDocument));
        Assert.IsTrue(editor.IsDirty);
        var rotated = editor.CapturePngSnapshot();
        using (var decoded = new ImageEditorViewModel())
        {
            decoded.LoadDocument(rotated.Bytes, "left.png");
            Assert.AreEqual(new PixelSize(2, 3), decoded.ActiveDocument!.PixelSize);
            CollectionAssert.AreEqual(LeftRotatedPixels, ReadPixels(decoded.ActiveDocument));
        }
        editor.MarkSaved(original);
        Assert.IsTrue(editor.IsDirty);
        editor.Undo();
        Assert.IsFalse(editor.IsDirty);
        editor.MarkSaved(rotated);
        Assert.IsTrue(editor.IsDirty);
        editor.Redo();
        Assert.IsFalse(editor.IsDirty);
        CollectionAssert.AreEqual(LeftRotatedPixels, ReadPixels(editor.ActiveDocument!));
        Assert.AreEqual(3, events);
        editor.ClearDocument();
        Assert.IsFalse(editor.RotateLeftCommand.CanExecute(null));
    }, CancellationToken.None);

    [TestMethod]
    public async Task LeftRotationInverseAndSymmetricNoOpPreserveRedo() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        editor.LoadDocument(MakeDocument());
        editor.RotateLeft();
        editor.RotateRight();
        Assert.AreEqual(new PixelSize(3, 2), editor.ActiveDocument!.PixelSize);
        CollectionAssert.AreEqual(Pixels, ReadPixels(editor.ActiveDocument));

        using var symmetric = new ImageEditorViewModel();
        var image = new WriteableBitmap(new PixelSize(4, 4), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
        byte[] pixels = new byte[4 * 4 * 4];
        foreach (int i in new[] { 1, 7, 14, 8 })
        {
            pixels[i * 4 + 2] = 255;
            pixels[i * 4 + 3] = 255;
        }
        using (var buffer = image.Lock())
            for (int y = 0; y < 4; y++)
                Marshal.Copy(pixels, y * 16, IntPtr.Add(buffer.Address, y * buffer.RowBytes), 16);
        symmetric.LoadDocument(new ImageEditorDocument("symmetric.png", image));
        symmetric.FlipHorizontal();
        symmetric.Undo();
        int events = 0;
        symmetric.ContentChanging += (_, _) => events++;
        symmetric.RotateLeft();
        Assert.IsFalse(symmetric.IsDirty);
        Assert.IsFalse(symmetric.UndoCommand.CanExecute(null));
        Assert.IsTrue(symmetric.RedoCommand.CanExecute(null));
        CollectionAssert.AreEqual(pixels, ReadPixels(symmetric.ActiveDocument!));
        Assert.AreEqual(0, events);
    }, CancellationToken.None);

    [TestMethod]
    public async Task FlipPreservesPixelsHistoryAndSnapshotRevision() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        Assert.IsFalse(editor.FlipHorizontalCommand.CanExecute(null));
        editor.FlipHorizontal();
        editor.LoadDocument(MakeDocument());
        Assert.IsTrue(editor.FlipHorizontalCommand.CanExecute(null));
        var original = editor.CapturePngSnapshot();
        int events = 0;
        editor.ContentChanging += (_, _) =>
        {
            CollectionAssert.AreEqual(events == 1 ? FlippedPixels : Pixels, ReadPixels(editor.ActiveDocument!));
            events++;
        };
        editor.FlipHorizontalCommand.Execute(null);
        Assert.AreEqual(new PixelSize(3, 2), editor.ActiveDocument!.PixelSize);
        CollectionAssert.AreEqual(FlippedPixels, ReadPixels(editor.ActiveDocument));
        Assert.IsTrue(editor.IsDirty);
        var flipped = editor.CapturePngSnapshot();
        using (var decoded = new ImageEditorViewModel())
        {
            decoded.LoadDocument(flipped.Bytes, "flipped.png");
            CollectionAssert.AreEqual(FlippedPixels, ReadPixels(decoded.ActiveDocument!));
        }
        editor.MarkSaved(original);
        Assert.IsTrue(editor.IsDirty);
        editor.Undo();
        Assert.IsFalse(editor.IsDirty);
        CollectionAssert.AreEqual(Pixels, ReadPixels(editor.ActiveDocument!));
        editor.MarkSaved(flipped);
        Assert.IsTrue(editor.IsDirty);
        editor.Redo();
        Assert.IsFalse(editor.IsDirty);
        CollectionAssert.AreEqual(FlippedPixels, ReadPixels(editor.ActiveDocument!));
        Assert.AreEqual(3, events);
    }, CancellationToken.None);

    [TestMethod]
    public async Task VerticalFlipPreservesPixelsHistoryAndSnapshotIdentity() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        Assert.IsFalse(editor.FlipVerticalCommand.CanExecute(null));
        editor.FlipVertical();
        editor.LoadDocument(MakeDocument());
        Assert.IsTrue(editor.FlipVerticalCommand.CanExecute(null));
        var original = editor.CapturePngSnapshot();
        int events = 0;
        editor.ContentChanging += (_, _) =>
        {
            CollectionAssert.AreEqual(events == 1 ? VerticallyFlippedPixels : Pixels, ReadPixels(editor.ActiveDocument!));
            events++;
        };
        editor.FlipVerticalCommand.Execute(null);
        Assert.AreEqual(new PixelSize(3, 2), editor.ActiveDocument!.PixelSize);
        CollectionAssert.AreEqual(VerticallyFlippedPixels, ReadPixels(editor.ActiveDocument));
        Assert.IsTrue(editor.IsDirty);
        Assert.IsTrue(editor.IsCurrentDocument(original));
        var flipped = editor.CapturePngSnapshot();
        using (var decoded = new ImageEditorViewModel())
        {
            decoded.LoadDocument(flipped.Bytes, "flipped.png");
            CollectionAssert.AreEqual(VerticallyFlippedPixels, ReadPixels(decoded.ActiveDocument!));
        }
        editor.MarkSaved(original);
        Assert.IsTrue(editor.IsDirty);
        editor.Undo();
        Assert.IsFalse(editor.IsDirty);
        editor.MarkSaved(flipped);
        Assert.IsTrue(editor.IsDirty);
        editor.Redo();
        Assert.IsFalse(editor.IsDirty);
        CollectionAssert.AreEqual(VerticallyFlippedPixels, ReadPixels(editor.ActiveDocument!));
        Assert.AreEqual(3, events);
        editor.LoadDocument(MakeDocument());
        Assert.IsFalse(editor.IsCurrentDocument(flipped));
        editor.MarkSaved(flipped);
        Assert.IsFalse(editor.IsDirty);
        editor.ClearDocument();
        Assert.IsFalse(editor.FlipVerticalCommand.CanExecute(null));
    }, CancellationToken.None);

    [TestMethod]
    public async Task SymmetricVerticalFlipIsNoOpAndPreservesRedo() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        var image = new WriteableBitmap(new PixelSize(3, 3), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
        byte[] symmetric = {
            0,0,255,255, 0,255,0,255, 0,0,0,0,
            255,0,0,255, 0,128,128,128, 255,255,255,255,
            0,0,255,255, 0,255,0,255, 0,0,0,0
        };
        using (var buffer = image.Lock())
            for (int y = 0; y < 3; y++)
                Marshal.Copy(symmetric, y * 12, IntPtr.Add(buffer.Address, y * buffer.RowBytes), 12);
        editor.LoadDocument(new ImageEditorDocument("symmetric.png", image));
        editor.FlipHorizontal();
        editor.Undo();
        int events = 0;
        editor.ContentChanging += (_, _) => events++;
        editor.FlipVertical();
        Assert.AreEqual(0, events);
        Assert.IsFalse(editor.IsDirty);
        Assert.IsTrue(editor.RedoCommand.CanExecute(null));
        CollectionAssert.AreEqual(symmetric, ReadPixels(editor.ActiveDocument!));

        using var row = new ImageEditorViewModel();
        row.LoadDocument(new ImageEditorDocument("row.png",
            new WriteableBitmap(new PixelSize(2, 1), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul)));
        row.FlipVertical();
        Assert.IsFalse(row.IsDirty);
        Assert.IsFalse(row.UndoCommand.CanExecute(null));
    }, CancellationToken.None);

    [TestMethod]
    public async Task SymmetricFlipIsNoOpAndPreservesRedo() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        var image = new WriteableBitmap(new PixelSize(3, 2), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
        byte[] symmetric = {
            0,0,255,255, 0,255,0,255, 0,0,255,255,
            255,0,0,255, 0,0,0,0, 255,0,0,255
        };
        using (var buffer = image.Lock())
            for (int y = 0; y < 2; y++)
                Marshal.Copy(symmetric, y * 12, IntPtr.Add(buffer.Address, y * buffer.RowBytes), 12);
        editor.LoadDocument(new ImageEditorDocument("symmetric.png", image));
        editor.RotateRight();
        editor.Undo();
        int events = 0;
        editor.ContentChanging += (_, _) => events++;
        editor.FlipHorizontal();
        Assert.AreEqual(0, events);
        Assert.IsFalse(editor.IsDirty);
        Assert.IsTrue(editor.RedoCommand.CanExecute(null));
        CollectionAssert.AreEqual(symmetric, ReadPixels(editor.ActiveDocument!));
        editor.Redo();
        editor.FlipHorizontal();
        editor.Undo();
        Assert.IsTrue(editor.RedoCommand.CanExecute(null));
        editor.FlipHorizontal();
        Assert.IsFalse(editor.RedoCommand.CanExecute(null));

        using var column = new ImageEditorViewModel();
        column.LoadDocument(new ImageEditorDocument("column.png",
            new WriteableBitmap(new PixelSize(1, 2), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul)));
        int columnEvents = 0;
        column.ContentChanging += (_, _) => columnEvents++;
        column.FlipHorizontal();
        Assert.IsFalse(column.IsDirty);
        Assert.IsFalse(column.UndoCommand.CanExecute(null));
        Assert.AreEqual(0, columnEvents);
    }, CancellationToken.None);

    [TestMethod]
    public async Task SnapshotsTrackRevisionAndDocumentAndRenameSafely() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        editor.LoadDocument(MakeDocument());
        var original = editor.CapturePngSnapshot();
        Assert.IsTrue(editor.IsCurrentDocument(original));
        byte[] copy = original.Bytes;
        copy[0] = 0;
        Assert.AreEqual((byte)137, original.Bytes[0]);
        editor.RotateRight();
        Assert.IsTrue(editor.IsCurrentDocument(original));
        var rotated = editor.CapturePngSnapshot();
        using (var decoded = new ImageEditorViewModel())
        {
            decoded.LoadDocument(rotated.Bytes, "rotated.png");
            CollectionAssert.AreEqual(ReadPixels(editor.ActiveDocument!), ReadPixels(decoded.ActiveDocument!));
        }
        editor.RotateRight();
        editor.MarkSaved(rotated);
        Assert.IsTrue(editor.IsDirty);
        editor.Undo();
        Assert.IsFalse(editor.IsDirty);
        editor.RenameDocument(rotated, "export.png");
        Assert.IsTrue(editor.DocumentSummary.StartsWith("export.png", StringComparison.Ordinal));
        editor.Undo();
        Assert.IsTrue(editor.IsDirty);
        editor.MarkSaved(original);
        Assert.IsFalse(editor.IsDirty);
        editor.LoadDocument(MakeDocument());
        Assert.IsFalse(editor.IsCurrentDocument(rotated));
        editor.RotateRight();
        editor.MarkSaved(rotated);
        editor.RenameDocument(rotated, "wrong.png");
        Assert.IsTrue(editor.IsDirty);
        Assert.IsTrue(editor.DocumentSummary.StartsWith("original.png", StringComparison.Ordinal));
        editor.AllowClear = false;
        editor.ClearDocument();
        editor.ActiveDocument = null;
        Assert.IsTrue(editor.HasDocument);
        var current = editor.CapturePngSnapshot();
        editor.AllowClear = true;
        editor.ClearDocument();
        Assert.IsFalse(editor.IsCurrentDocument(current));
        editor.LoadDocument(MakeDocument());
        current = editor.CapturePngSnapshot();
        editor.Dispose();
        Assert.IsFalse(editor.IsCurrentDocument(current));
        Assert.ThrowsExactly<ArgumentNullException>(() => editor.IsCurrentDocument(null!));
    }, CancellationToken.None);

    [TestMethod]
    public async Task InvalidInputAndBoundsDoNotReplaceDocument() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        editor.LoadDocument(MakeDocument());
        Assert.ThrowsExactly<InvalidDataException>(() => editor.LoadDocument(new byte[] { 1, 2, 3 }, "bad.png"));
        Assert.ThrowsExactly<InvalidDataException>(() => editor.LoadDocument(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 32, 1, 0, 0, 0, 1 }, "huge.png"));
        byte[] truncated = editor.CapturePngSnapshot().Bytes.AsSpan(0, 24).ToArray();
        try
        {
            editor.LoadDocument(truncated, "truncated.png");
            Assert.Fail("The decoder accepted a truncated PNG.");
        }
        catch (Exception ex) when (ex is not AssertFailedException) { }
        Assert.ThrowsExactly<InvalidDataException>(() => ImageDecodeLimits.ValidateSize(8193, 1));
        Assert.ThrowsExactly<InvalidDataException>(() => ImageDecodeLimits.ValidateSize(2049, 2048));
        Assert.ThrowsExactly<InvalidDataException>(() => ImageDecodeLimits.ValidateSize(0, 1));
        Assert.AreEqual("original.png", editor.ActiveDocument!.Name);
        using var empty = new ImageEditorViewModel();
        Assert.ThrowsExactly<InvalidOperationException>(() => empty.CapturePngSnapshot());
    }, CancellationToken.None);

    [TestMethod]
    public async Task SymmetricRotationIsNotAChange() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        var image = new WriteableBitmap(new PixelSize(1, 1), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
        editor.LoadDocument(new ImageEditorDocument("single.png", image));
        int events = 0;
        editor.ContentChanging += (_, _) => events++;
        editor.RotateRight();
        editor.Undo();
        editor.Redo();
        Assert.IsFalse(editor.IsDirty);
        Assert.AreEqual(0, events);
    }, CancellationToken.None);

    [TestMethod]
    public async Task PngJpegAndBmpOpen() =>
        await _session.Dispatch(() =>
    {
        using var doc = MakeDocument();
        using var editor = new ImageEditorViewModel();
        using var png = new MemoryStream();
        ((Bitmap)doc.Image).Save(png, new PngBitmapEncoderOptions());
        editor.LoadDocument(png.ToArray(), "a.png");
        CollectionAssert.AreEqual(Pixels, ReadPixels(editor.ActiveDocument!));
        using var jpeg = new MemoryStream();
        ((Bitmap)doc.Image).Save(jpeg, new JpegBitmapEncoderOptions());
        editor.LoadDocument(jpeg.ToArray(), "a.jpg");
        Assert.AreEqual(new PixelSize(3, 2), editor.ActiveDocument!.PixelSize);
        byte[] bmp = {
            66,77,70,0,0,0,0,0,0,0,54,0,0,0,40,0,0,0,2,0,0,0,2,0,0,0,
            1,0,24,0,0,0,0,0,16,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,255,0,255,0,0,0,255,0,0,255,255,255,0,0
        };
        editor.LoadDocument(bmp, "a.bmp");
        Assert.AreEqual(new PixelSize(2, 2), editor.ActiveDocument!.PixelSize);
    }, CancellationToken.None);

    [TestMethod]
    public async Task HostExportCreatesOnlyNewFilesAndMarksSaved()
    {
        await _session.Dispatch(async () =>
        {
            string directory = Path.Combine(AppContext.BaseDirectory, "editor-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                using var host = new MainWindowViewModel();
                host.Editor.LoadDocument(MakeDocument());
                host.Editor.RotateLeft();
                string path = Path.Combine(directory, "export.png");
                await host.ExportPngToNewFileAsync(path);
                Assert.IsFalse(host.Editor.IsDirty);
                Assert.IsTrue(host.Editor.DocumentSummary.StartsWith("export.png", StringComparison.Ordinal));
                byte[] written = File.ReadAllBytes(path);
                using (var decoded = new ImageEditorViewModel())
                {
                    decoded.LoadDocument(written, "export.png");
                    Assert.AreEqual(new PixelSize(2, 3), decoded.ActiveDocument!.PixelSize);
                    CollectionAssert.AreEqual(LeftRotatedPixels, ReadPixels(decoded.ActiveDocument));
                }
                host.Editor.RotateRight();
                await Assert.ThrowsAsync<IOException>(() => host.ExportPngToNewFileAsync(path));
                CollectionAssert.AreEqual(written, File.ReadAllBytes(path));
                Assert.IsTrue(host.Editor.IsDirty);
                Assert.AreEqual(1, Directory.GetFiles(directory).Length);
                await Assert.ThrowsAsync<InvalidDataException>(() => host.ExportPngToNewFileAsync(Path.Combine(directory, "bad.jpg")));
                await Assert.ThrowsAsync<DirectoryNotFoundException>(() => host.ExportPngToNewFileAsync(Path.Combine(directory, "missing", "image.png")));
                Assert.IsTrue(host.Editor.IsDirty);
            }
            finally { Directory.Delete(directory, recursive: true); }
            return true;
        }, CancellationToken.None);
    }

    [TestMethod]
    public async Task HostInputLimitChecksLengthFirstAndCapsNonSeekableReads()
    {
        using var seekable = new CountingStream(ImageDecodeLimits.MaxEncodedBytes + 1L, true);
        await Assert.ThrowsAsync<InvalidDataException>(() => MainWindowViewModel.ReadImageBytesAsync(seekable));
        Assert.AreEqual(0L, seekable.BytesRead);
        using var nonSeekable = new CountingStream(ImageDecodeLimits.MaxEncodedBytes + 100000L, false);
        await Assert.ThrowsAsync<InvalidDataException>(() => MainWindowViewModel.ReadImageBytesAsync(nonSeekable));
        Assert.AreEqual(ImageDecodeLimits.MaxEncodedBytes + 1L, nonSeekable.BytesRead);
    }

    [TestMethod]
    public async Task FailedAndCanceledWritesRemoveOnlyTheirTemporarySibling()
    {
        string directory = Path.Combine(AppContext.BaseDirectory, "editor-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            string path = Path.Combine(directory, "export.png");
            string unrelated = Path.Combine(directory, "unrelated.tmp");
            File.WriteAllText(unrelated, "keep");
            await Assert.ThrowsAsync<IOException>(() => PngExportFile.WriteNewAsync(path, async (stream, token) =>
            {
                await stream.WriteAsync(new byte[] { 1, 2, 3 }, 0, 3, token);
                throw new IOException("Simulated write error.");
            }));
            Assert.IsFalse(File.Exists(path));
            Assert.AreEqual(1, Directory.GetFiles(directory).Length);
            using var cancel = new CancellationTokenSource();
            await Assert.ThrowsAsync<OperationCanceledException>(() => PngExportFile.WriteNewAsync(path, async (stream, token) =>
            {
                await stream.WriteAsync(new byte[] { 1, 2, 3 }, 0, 3, token);
                cancel.Cancel();
            }, cancel.Token));
            Assert.IsFalse(File.Exists(path));
            Assert.AreEqual(1, Directory.GetFiles(directory).Length);
            Assert.AreEqual("keep", File.ReadAllText(unrelated));
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

    [TestMethod]
    public async Task StandaloneWindowCommandsAndDirtyClose()
    {
        await _session.Dispatch(async () =>
        {
            string directory = Path.Combine(AppContext.BaseDirectory, "editor-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            MainWindow? window = null;
            try
            {
                using var fixture = MakeDocument();
                using var png = new MemoryStream();
                ((Bitmap)fixture.Image).Save(png, new PngBitmapEncoderOptions());
                string sourcePath = Path.Combine(directory, "fixture.png");
                byte[] sourceBytes = png.ToArray();
                File.WriteAllBytes(sourcePath, sourceBytes);
                var host = new MainWindowViewModel();
                host.Editor.LoadDocument(File.ReadAllBytes(sourcePath), "fixture.png");
                window = new MainWindow { DataContext = host, Width = 1500 };
                window.Show();
                window.UpdateLayout();
                Click(window, "Rotate 180°");
                Assert.AreEqual(new PixelSize(3, 2), host.Editor.ActiveDocument!.PixelSize);
                CollectionAssert.AreEqual(Rotated180Pixels, ReadPixels(host.Editor.ActiveDocument));
                Assert.IsTrue(host.Editor.IsDirty);
                Click(window, "Undo");
                Assert.IsFalse(host.Editor.IsDirty);
                Click(window, "Redo");
                CollectionAssert.AreEqual(Rotated180Pixels, ReadPixels(host.Editor.ActiveDocument!));
                Click(window, "Undo");
                Click(window, "Rotate left");
                Assert.AreEqual(new PixelSize(2, 3), host.Editor.ActiveDocument!.PixelSize);
                CollectionAssert.AreEqual(LeftRotatedPixels, ReadPixels(host.Editor.ActiveDocument));
                Click(window, "Undo");
                Assert.IsFalse(host.Editor.IsDirty);
                Click(window, "Rotate 90° right");
                Assert.AreEqual(new PixelSize(2, 3), host.Editor.ActiveDocument!.PixelSize);
                byte[] expected = {
                255,0,0,255, 0,0,255,255,
                0,128,128,128, 0,255,0,255,
                255,255,255,255, 0,0,0,0
            };
                CollectionAssert.AreEqual(expected, ReadPixels(host.Editor.ActiveDocument));
                Assert.IsTrue(host.Editor.IsDirty);
                Click(window, "Undo");
                Assert.AreEqual(new PixelSize(3, 2), host.Editor.ActiveDocument!.PixelSize);
                Assert.IsFalse(host.Editor.IsDirty);
                Click(window, "Redo");
                Assert.IsTrue(host.Editor.IsDirty);
                Click(window, "Flip horizontal");
                expected = new byte[] {
                    0,0,255,255, 255,0,0,255,
                    0,255,0,255, 0,128,128,128,
                    0,0,0,0, 255,255,255,255
                };
                CollectionAssert.AreEqual(expected, ReadPixels(host.Editor.ActiveDocument!));
                Click(window, "Undo");
                Click(window, "Redo");
                CollectionAssert.AreEqual(expected, ReadPixels(host.Editor.ActiveDocument!));
                Click(window, "Flip vertical");
                expected = new byte[] {
                    0,0,0,0, 255,255,255,255,
                    0,255,0,255, 0,128,128,128,
                    0,0,255,255, 255,0,0,255
                };
                CollectionAssert.AreEqual(expected, ReadPixels(host.Editor.ActiveDocument!));
                Click(window, "Undo");
                Click(window, "Redo");
                CollectionAssert.AreEqual(expected, ReadPixels(host.Editor.ActiveDocument!));
                window.Close();
                Assert.IsTrue(window.IsVisible);
                var dialog = window.OwnedWindows.Single();
                Assert.IsTrue(dialog.IsVisible);
                dialog.Close(false);
                Dispatcher.UIThread.RunJobs();
                Assert.IsTrue(window.IsVisible);
                Assert.IsTrue(host.Editor.IsDirty);
                string path = Path.Combine(directory, "export.png");
                await host.ExportPngToNewFileAsync(path);
                Assert.IsTrue(File.Exists(path));
                CollectionAssert.AreEqual(sourceBytes, File.ReadAllBytes(sourcePath));
                Assert.IsFalse(host.Editor.IsDirty);
                using (var exported = new ImageEditorViewModel())
                {
                    exported.LoadDocument(File.ReadAllBytes(path), "export.png");
                    Assert.AreEqual(new PixelSize(2, 3), exported.ActiveDocument!.PixelSize);
                    CollectionAssert.AreEqual(expected, ReadPixels(exported.ActiveDocument));
                }
                window.Close();
                Assert.IsFalse(window.IsVisible);
            }
            finally
            {
                if (window?.IsVisible == true)
                {
                    foreach (var dialog in window.OwnedWindows.ToArray()) dialog.Close(false);
                    if (window.DataContext is MainWindowViewModel host && host.Editor.HasDocument)
                        host.Editor.MarkSaved(host.Editor.CapturePngSnapshot());
                    window.Close();
                }
                Directory.Delete(directory, recursive: true);
            }
            return true;
        }, CancellationToken.None);
    }

    private static void Click(Window window, string label)
    {
        var button = window.GetVisualDescendants().OfType<Button>().Single(b => Equals(b.Content, label));
        Assert.IsTrue(button.IsEnabled);
        var point = button.TranslatePoint(new Point(button.Bounds.Width / 2, button.Bounds.Height / 2), window);
        Assert.IsNotNull(point);
        window.MouseDown(point.Value, MouseButton.Left, RawInputModifiers.None);
        window.MouseUp(point.Value, MouseButton.Left, RawInputModifiers.None);
        Dispatcher.UIThread.RunJobs();
    }

    public static class HeadlessTestEntry
    {
        public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<Avln_ImageEditor.Host.App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false })
            .UseSkia();
    }

    private sealed class CountingStream(long length, bool canSeek) : Stream
    {
        public long BytesRead { get; private set; }
        public override bool CanRead => true;
        public override bool CanSeek => canSeek;
        public override bool CanWrite => false;
        public override long Length => length;
        public override long Position { get => BytesRead; set => throw new NotSupportedException(); }
        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = (int)Math.Min(count, length - BytesRead);
            BytesRead += read;
            return read;
        }
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            Task.FromResult(Read(buffer, offset, count));
        public override void Flush() => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
