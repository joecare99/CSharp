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
                host.Editor.RotateRight();
                string path = Path.Combine(directory, "export.png");
                await host.ExportPngToNewFileAsync(path);
                Assert.IsFalse(host.Editor.IsDirty);
                Assert.IsTrue(host.Editor.DocumentSummary.StartsWith("export.png", StringComparison.Ordinal));
                byte[] written = File.ReadAllBytes(path);
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
                var host = new MainWindowViewModel();
                host.Editor.LoadDocument(png.ToArray(), "fixture.png");
                window = new MainWindow { DataContext = host };
                window.Show();
                window.UpdateLayout();
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
