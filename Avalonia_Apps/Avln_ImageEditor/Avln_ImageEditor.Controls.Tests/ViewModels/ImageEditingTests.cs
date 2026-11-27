using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avln_ImageEditor.Controls.Models;
using Avln_ImageEditor.Controls.ViewModels;
using Avln_ImageEditor.Controls.Views;
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
    public async Task GrayscalePreservesAlphaDimensionsHistoryAndPng() =>
        await _session.Dispatch(() =>
        {
            byte[] original =
            {
                17, 91, 203, 255, 16, 64, 96, 128, 0, 0, 0, 0,
                0, 0, 255, 255, 0, 255, 0, 255, 255, 0, 0, 255
            };
            byte[] grayscale =
            {
                116, 116, 116, 255, 68, 68, 68, 128, 0, 0, 0, 0,
                76, 76, 76, 255, 150, 150, 150, 255, 29, 29, 29, 255
            };
            using var editor = new ImageEditorViewModel();
            int commandChanges = 0, events = 0;
            editor.ConvertToGrayscaleCommand.CanExecuteChanged += (_, _) => commandChanges++;
            Assert.IsFalse(editor.ConvertToGrayscaleCommand.CanExecute(null));
            editor.ConvertToGrayscale();
            var bitmap = new WriteableBitmap(new PixelSize(3, 2), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
            using (var buffer = bitmap.Lock()) Marshal.Copy(original, 0, buffer.Address, original.Length);
            editor.LoadDocument(new ImageEditorDocument("colors.png", bitmap));
            Assert.IsTrue(editor.ConvertToGrayscaleCommand.CanExecute(null));
            Assert.IsTrue(commandChanges > 0);
            editor.CropX = "1";
            editor.CropWidth = "2";
            editor.ContentChanging += (_, _) => events++;
            editor.ConvertToGrayscaleCommand.Execute(null);
            Assert.AreEqual(1, events);
            Assert.AreEqual(new PixelSize(3, 2), editor.ActiveDocument!.PixelSize);
            Assert.AreEqual("1", editor.CropX);
            Assert.AreEqual("2", editor.CropWidth);
            CollectionAssert.AreEqual(grayscale, ReadPixels(editor.ActiveDocument));
            Assert.IsTrue(editor.IsDirty);
            using (var decoded = new ImageEditorViewModel())
            {
                decoded.LoadDocument(editor.CapturePngSnapshot().Bytes, "grayscale.png");
                CollectionAssert.AreEqual(grayscale, ReadPixels(decoded.ActiveDocument!));
            }
            editor.Undo();
            Assert.IsFalse(editor.IsDirty);
            Assert.IsFalse(editor.UndoCommand.CanExecute(null));
            CollectionAssert.AreEqual(original, ReadPixels(editor.ActiveDocument!));
            editor.Redo();
            CollectionAssert.AreEqual(grayscale, ReadPixels(editor.ActiveDocument!));
            Assert.AreEqual(3, events);
            editor.Undo();
            editor.ConvertToGrayscale();
            Assert.IsFalse(editor.RedoCommand.CanExecute(null));
            CollectionAssert.AreEqual(grayscale, ReadPixels(editor.ActiveDocument!));
            editor.ClearDocument();
            Assert.IsFalse(editor.ConvertToGrayscaleCommand.CanExecute(null));
            return true;
        }, CancellationToken.None);

    [TestMethod]
    public async Task GrayscaleNoOpPreservesRedoAndNotifications() =>
        await _session.Dispatch(() =>
        {
            byte[] pixels = { 64, 64, 64, 128, 0, 0, 0, 0 };
            using var editor = new ImageEditorViewModel();
            var bitmap = new WriteableBitmap(new PixelSize(2, 1), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
            using (var buffer = bitmap.Lock()) Marshal.Copy(pixels, 0, buffer.Address, pixels.Length);
            editor.LoadDocument(new ImageEditorDocument("gray.png", bitmap));
            editor.FlipHorizontal();
            editor.Undo();
            var document = editor.ActiveDocument;
            byte[] png = editor.CapturePngSnapshot().Bytes;
            int events = 0;
            editor.ContentChanging += (_, _) => events++;
            editor.ConvertToGrayscale();
            Assert.AreSame(document, editor.ActiveDocument);
            Assert.AreEqual(0, events);
            Assert.IsFalse(editor.IsDirty);
            Assert.IsTrue(editor.RedoCommand.CanExecute(null));
            Assert.IsFalse(editor.UndoCommand.CanExecute(null));
            CollectionAssert.AreEqual(png, editor.CapturePngSnapshot().Bytes);
            editor.Redo();
            Assert.IsTrue(editor.IsDirty);
            return true;
        }, CancellationToken.None);

    [TestMethod]
    public async Task InvertPreservesAlphaDimensionsHistoryAndPng() =>
        await _session.Dispatch(() =>
        {
            byte[] original = { 17, 91, 203, 255, 16, 64, 96, 128, 0, 0, 0, 0 };
            byte[] inverted = { 238, 164, 52, 255, 112, 64, 32, 128, 0, 0, 0, 0 };
            using var editor = new ImageEditorViewModel();
            int commandChanges = 0, events = 0;
            editor.InvertColorsCommand.CanExecuteChanged += (_, _) => commandChanges++;
            Assert.IsFalse(editor.InvertColorsCommand.CanExecute(null));
            editor.InvertColors();
            var bitmap = new WriteableBitmap(new PixelSize(3, 1), new Vector(192, 192), PixelFormats.Bgra8888, AlphaFormat.Premul);
            using (var buffer = bitmap.Lock()) Marshal.Copy(original, 0, buffer.Address, original.Length);
            editor.LoadDocument(new ImageEditorDocument("colors.png", bitmap));
            Assert.IsTrue(editor.InvertColorsCommand.CanExecute(null));
            Assert.IsTrue(commandChanges > 0);
            editor.ContentChanging += (_, _) =>
            {
                events++;
                if (events == 1) CollectionAssert.AreEqual(original, ReadPixels(editor.ActiveDocument!));
            };
            editor.CropX = "1";
            editor.CropWidth = "2";
            editor.InvertColorsCommand.Execute(null);
            Assert.AreEqual(1, events);
            Assert.AreEqual(new PixelSize(3, 1), editor.ActiveDocument!.PixelSize);
            Assert.AreEqual("1", editor.CropX);
            Assert.AreEqual("2", editor.CropWidth);
            CollectionAssert.AreEqual(inverted, ReadPixels(editor.ActiveDocument));
            Assert.IsTrue(editor.IsDirty);
            using (var decoded = new ImageEditorViewModel())
            {
                decoded.LoadDocument(editor.CapturePngSnapshot().Bytes, "inverted.png");
                CollectionAssert.AreEqual(inverted, ReadPixels(decoded.ActiveDocument!));
            }
            editor.Undo();
            Assert.IsFalse(editor.IsDirty);
            CollectionAssert.AreEqual(original, ReadPixels(editor.ActiveDocument!));
            Assert.IsFalse(editor.UndoCommand.CanExecute(null));
            editor.Redo();
            CollectionAssert.AreEqual(inverted, ReadPixels(editor.ActiveDocument!));
            editor.Undo();
            editor.InvertColors();
            Assert.IsFalse(editor.RedoCommand.CanExecute(null));
            CollectionAssert.AreEqual(inverted, ReadPixels(editor.ActiveDocument!));
            editor.InvertColors();
            CollectionAssert.AreEqual(original, ReadPixels(editor.ActiveDocument!));
            editor.ClearDocument();
            Assert.IsFalse(editor.InvertColorsCommand.CanExecute(null));
            return true;
        }, CancellationToken.None);

    [TestMethod]
    public async Task InvertNoOpPreservesRedoAndNotifications() =>
        await _session.Dispatch(() =>
        {
            byte[] pixels = { 64, 64, 64, 128, 0, 0, 0, 0 };
            using var editor = new ImageEditorViewModel();
            var bitmap = new WriteableBitmap(new PixelSize(2, 1), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
            using (var buffer = bitmap.Lock()) Marshal.Copy(pixels, 0, buffer.Address, pixels.Length);
            editor.LoadDocument(new ImageEditorDocument("gray.png", bitmap));
            editor.FlipHorizontal();
            editor.Undo();
            var document = editor.ActiveDocument;
            byte[] png = editor.CapturePngSnapshot().Bytes;
            int events = 0;
            editor.ContentChanging += (_, _) => events++;
            editor.InvertColors();
            Assert.AreSame(document, editor.ActiveDocument);
            Assert.AreEqual(0, events);
            Assert.IsFalse(editor.IsDirty);
            Assert.IsTrue(editor.RedoCommand.CanExecute(null));
            Assert.IsFalse(editor.UndoCommand.CanExecute(null));
            CollectionAssert.AreEqual(png, editor.CapturePngSnapshot().Bytes);
            editor.Redo();
            Assert.IsTrue(editor.IsDirty);
            return true;
        }, CancellationToken.None);

    [TestMethod]
    public async Task TransparencyBackgroundRendersOnlyBehindImageAndPreservesState() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        var window = new Window { Width = 800, Height = 650, Content = new ImageEditorControl { DataContext = editor } };
        try
        {
            window.Show();
            window.UpdateLayout();
            var surface = window.GetVisualDescendants().OfType<Grid>().Single(c => c.Name == "ImageSurface");
            var background = window.GetVisualDescendants().OfType<Border>().Single(c => c.Name == "TransparencyBackground");
            var image = window.GetVisualDescendants().OfType<Image>().Single(c => c.Name == "EditorImage");
            var outline = window.GetVisualDescendants().OfType<CropPreviewOverlay>().Single(c => c.Name == "CropPreviewOutline");
            var scroll = window.GetVisualDescendants().OfType<ScrollViewer>().Single(c => c.Name == "ImageScrollViewer");
            Assert.IsFalse(background.IsVisible);
            Assert.IsFalse(background.IsHitTestVisible);

            var bitmap = new WriteableBitmap(new PixelSize(64, 48), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
            using (var buffer = bitmap.Lock())
            {
                var row = new byte[64 * 4];
                for (int x = 24; x < 64; x++)
                {
                    row[x * 4] = x >= 40 ? (byte)255 : (byte)0;
                    row[x * 4 + 2] = x < 40 ? (byte)128 : (byte)0;
                    row[x * 4 + 3] = x < 40 ? (byte)128 : (byte)255;
                }
                for (int y = 0; y < 48; y++)
                    Marshal.Copy(row, 0, IntPtr.Add(buffer.Address, y * buffer.RowBytes), row.Length);
            }
            editor.LoadDocument(new ImageEditorDocument("alpha.png", bitmap));
            var original = editor.ActiveDocument;
            var pixels = ReadPixels(original!);
            CollectionAssert.AreEqual(new byte[] { 0, 0, 128, 128 }, Pixel(pixels, 64, 28, 4));
            CollectionAssert.AreEqual(new byte[] { 255, 0, 0, 255 }, Pixel(pixels, 64, 44, 4));
            var png = editor.CapturePngSnapshot().Bytes;
            int changes = 0;
            editor.ContentChanging += (_, _) => changes++;
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.IsTrue(background.IsVisible);
            Assert.AreSame(original!.DisplayImage, image.Source);
            Assert.AreEqual(image.Bounds.Size, background.Bounds.Size);
            Assert.AreEqual(new Size(64, 48), surface.Bounds.Size);
            var origin = surface.TranslatePoint(new Point(0, 0), window)!.Value;
            using var rendered = window.CaptureRenderedFrame();
            Assert.IsNotNull(rendered);
            using var renderedCopy = new WriteableBitmap(rendered.PixelSize, new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
            using var frameBuffer = renderedCopy.Lock();
            rendered.CopyPixels(frameBuffer);
            var frame = new byte[rendered.PixelSize.Width * rendered.PixelSize.Height * 4];
            for (int y = 0; y < rendered.PixelSize.Height; y++)
                Marshal.Copy(IntPtr.Add(frameBuffer.Address, y * frameBuffer.RowBytes), frame, y * rendered.PixelSize.Width * 4, rendered.PixelSize.Width * 4);
            byte[] Sample(int x, int y) => Pixel(frame, rendered.PixelSize.Width, (int)origin.X + x, (int)origin.Y + y);
            byte[] light = Sample(4, 4);
            byte[] dark = Sample(12, 4);
            CollectionAssert.AreNotEqual(light, Sample(-4, 4));
            CollectionAssert.AreNotEqual(dark, Sample(68, 4));
            CollectionAssert.AreNotEqual(light, dark);
            CollectionAssert.AreEqual(light, Sample(20, 4));
            CollectionAssert.AreEqual(dark, Sample(4, 12));
            Assert.AreEqual((byte)255, light[3]);
            Assert.AreEqual((byte)255, dark[3]);
            var partial = Sample(28, 4);
            Assert.AreEqual((byte)255, partial[3]);
            Assert.AreEqual(dark[0] / 2, partial[0], 2);
            Assert.AreEqual(128 + dark[2] / 2, partial[2], 2);
            CollectionAssert.AreEqual(new byte[] { 255, 0, 0, 255 }, Sample(44, 4));

            editor.CropWidth = "63";
            editor.ZoomPercentage = 175;
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.IsTrue(outline.IsVisible);
            var imageOrigin = image.TranslatePoint(new Point(0, 0), scroll)!.Value;
            var backgroundOrigin = background.TranslatePoint(new Point(0, 0), scroll)!.Value;
            var backgroundEnd = background.TranslatePoint(new Point(64, 48), scroll)!.Value;
            Assert.AreEqual(imageOrigin, backgroundOrigin);
            Assert.AreEqual(112, backgroundEnd.X - backgroundOrigin.X, 0.01);
            Assert.AreEqual(84, backgroundEnd.Y - backgroundOrigin.Y, 0.01);
            Assert.AreSame(original, editor.ActiveDocument);
            Assert.IsFalse(editor.IsDirty);
            Assert.IsFalse(editor.UndoCommand.CanExecute(null));
            Assert.AreEqual(0, changes);
            CollectionAssert.AreEqual(pixels, ReadPixels(editor.ActiveDocument!));
            CollectionAssert.AreEqual(png, editor.CapturePngSnapshot().Bytes);

            editor.ApplyCrop();
            Assert.IsTrue(editor.IsDirty);
            Assert.AreEqual(63, editor.ActiveDocument!.PixelSize.Width);
            editor.Undo();
            Assert.IsFalse(editor.IsDirty);
            CollectionAssert.AreEqual(pixels, ReadPixels(editor.ActiveDocument!));
            CollectionAssert.AreEqual(png, editor.CapturePngSnapshot().Bytes);
            editor.Redo();
            using var decoded = new ImageEditorViewModel();
            decoded.LoadDocument(editor.CapturePngSnapshot().Bytes, "export.png");
            CollectionAssert.AreEqual(ReadPixels(editor.ActiveDocument!), ReadPixels(decoded.ActiveDocument!));
            Assert.AreEqual(3, changes);
            editor.ClearDocument();
            Assert.IsFalse(background.IsVisible);
        }
        finally { window.Close(); }
    }, CancellationToken.None);

    private static byte[] Pixel(byte[] pixels, int width, int x, int y) =>
        pixels.Skip((y * width + x) * 4).Take(4).ToArray();

    [TestMethod]
    [DataRow(96)]
    [DataRow(192)]
    public async Task ImageDpiPixelMapping(int dpi) =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        var bitmap = new WriteableBitmap(new PixelSize(64, 48), new Vector(dpi, dpi), PixelFormats.Bgra8888, AlphaFormat.Premul);
        using (var buffer = bitmap.Lock())
        {
            for (int y = 0; y < 48; y++)
            {
                var row = new byte[64 * 4];
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
        var originalPixels = ReadPixels(editor.ActiveDocument!);
        using var exported = new ImageEditorViewModel();
        exported.LoadDocument(editor.CapturePngSnapshot().Bytes, "export.png");
        CollectionAssert.AreEqual(originalPixels, ReadPixels(exported.ActiveDocument!));
        var window = new Window { Width = 800, Height = 650, Content = new ImageEditorControl { DataContext = editor } };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            var surface = window.GetVisualDescendants().OfType<Grid>().Single(c => c.Name == "ImageSurface");
            var background = window.GetVisualDescendants().OfType<Border>().Single(c => c.Name == "TransparencyBackground");
            var image = window.GetVisualDescendants().OfType<Image>().Single(c => c.Name == "EditorImage");
            var withChecker = RenderPixels(surface, 64, 48);
            background.IsVisible = false;
            var withoutChecker = RenderPixels(surface, 64, 48);
            var plain = RenderPixels(image, 64, 48);
            CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, Pixel(withoutChecker, 64, 4, 4));
            CollectionAssert.AreEqual(new byte[] { 0, 0, 128, 128 }, Pixel(withoutChecker, 64, 28, 4));
            CollectionAssert.AreEqual(new byte[] { 255, 0, 0, 255 }, Pixel(withoutChecker, 64, 44, 4));
            CollectionAssert.AreEqual(new byte[] { 0, 255, 0, 255 }, Pixel(withoutChecker, 64, 44, 36));
            CollectionAssert.AreEqual(withoutChecker, plain);
            var dark = Pixel(withChecker, 64, 12, 4);
            var partial = Pixel(withChecker, 64, 28, 4);
            Assert.AreEqual(dark[0] / 2, partial[0], 2);
            Assert.AreEqual(128 + dark[2] / 2, partial[2], 2);
            CollectionAssert.AreEqual(new byte[] { 255, 0, 0, 255 }, Pixel(withChecker, 64, 44, 4));
            CollectionAssert.AreEqual(new byte[] { 0, 255, 0, 255 }, Pixel(withChecker, 64, 44, 36));

            background.IsVisible = true;
            editor.ZoomPercentage = 200;
            editor.CropX = "16"; editor.CropY = "8"; editor.CropWidth = "40"; editor.CropHeight = "32";
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            var overlay = window.GetVisualDescendants().OfType<CropPreviewOverlay>().Single();
            Assert.IsTrue(overlay.IsVisible);
            CollectionAssert.AreEqual(Pixel(withChecker, 64, 4, 4), FramePixel(window, surface, 4, 4));
            CollectionAssert.AreEqual(partial, FramePixel(window, surface, 28, 4));
            CollectionAssert.AreEqual(new byte[] { 255, 0, 0, 255 }, FramePixel(window, surface, 44, 4));
            CollectionAssert.AreEqual(new byte[] { 0, 255, 0, 255 }, FramePixel(window, surface, 44, 36));
            Assert.IsFalse(editor.IsDirty);
            Assert.IsFalse(editor.UndoCommand.CanExecute(null));
            CollectionAssert.AreEqual(originalPixels, ReadPixels(editor.ActiveDocument!));

            editor.ApplyCrop();
            Assert.IsTrue(editor.IsDirty);
            Assert.AreEqual(new PixelSize(40, 32), editor.ActiveDocument!.PixelSize);
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            CollectionAssert.AreEqual(Pixel(withChecker, 64, 4, 4), FramePixel(window, surface, 4, 4));
            CollectionAssert.AreEqual(partial, FramePixel(window, surface, 12, 4));
            CollectionAssert.AreEqual(new byte[] { 255, 0, 0, 255 }, FramePixel(window, surface, 28, 4));
            CollectionAssert.AreEqual(new byte[] { 0, 255, 0, 255 }, FramePixel(window, surface, 28, 28));
            using (var croppedPng = new ImageEditorViewModel())
            {
                croppedPng.LoadDocument(editor.CapturePngSnapshot().Bytes, "crop.png");
                CollectionAssert.AreEqual(ReadPixels(editor.ActiveDocument), ReadPixels(croppedPng.ActiveDocument!));
            }
            editor.Undo();
            Assert.IsFalse(editor.IsDirty);
            Assert.IsTrue(editor.RedoCommand.CanExecute(null));
            CollectionAssert.AreEqual(originalPixels, ReadPixels(editor.ActiveDocument!));
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            CollectionAssert.AreEqual(partial, FramePixel(window, surface, 28, 4));
            CollectionAssert.AreEqual(new byte[] { 255, 0, 0, 255 }, FramePixel(window, surface, 44, 4));
            editor.Redo();
            Assert.IsTrue(editor.IsDirty);
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            CollectionAssert.AreEqual(partial, FramePixel(window, surface, 12, 4));
            CollectionAssert.AreEqual(new byte[] { 0, 255, 0, 255 }, FramePixel(window, surface, 28, 28));
        }
        finally { window.Close(); }
    }, CancellationToken.None);

    private static byte[] RenderPixels(Control control, int width, int height)
    {
        using var rendered = new RenderTargetBitmap(new PixelSize(width, height));
        using var copy = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
        rendered.Render(control);
        using var buffer = copy.Lock();
        rendered.CopyPixels(buffer);
        var pixels = new byte[width * height * 4];
        for (int y = 0; y < height; y++)
            Marshal.Copy(IntPtr.Add(buffer.Address, y * buffer.RowBytes), pixels, y * width * 4, width * 4);
        return pixels;
    }

    private static byte[] FramePixel(Window window, Control control, double x, double y)
    {
        var point = control.TranslatePoint(new Point(x + 0.5, y + 0.5), window)!.Value;
        using var rendered = window.CaptureRenderedFrame();
        Assert.IsNotNull(rendered);
        using var copy = new WriteableBitmap(rendered.PixelSize, new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);
        using var buffer = copy.Lock();
        rendered.CopyPixels(buffer);
        var pixel = new byte[4];
        int px = (int)(point.X * window.RenderScaling);
        int py = (int)(point.Y * window.RenderScaling);
        Marshal.Copy(IntPtr.Add(buffer.Address, py * buffer.RowBytes + px * 4), pixel, 0, 4);
        return pixel;
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
    public async Task ResetCropCoordinatesPreservesDocumentAndHistory() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        editor.CropX = "invalid";
        Assert.IsFalse(editor.ResetCropCoordinatesCommand.CanExecute(null));
        editor.ResetCropCoordinates();
        Assert.AreEqual("invalid", editor.CropX);
        editor.LoadDocument(MakeDocument());
        editor.RotateRight();
        var rotated = editor.ActiveDocument;
        var png = editor.CapturePngSnapshot().Bytes;
        int changes = 0;
        editor.ContentChanging += (_, _) => changes++;
        editor.CropX = "-1"; editor.CropY = "invalid"; editor.CropWidth = ""; editor.CropHeight = "999";
        Assert.IsTrue(editor.ResetCropCoordinatesCommand.CanExecute(null));
        Assert.IsFalse(editor.ApplyCropCommand.CanExecute(null));
        editor.ResetCropCoordinatesCommand.Execute(null);
        Assert.AreEqual("0", editor.CropX);
        Assert.AreEqual("0", editor.CropY);
        Assert.AreEqual("2", editor.CropWidth);
        Assert.AreEqual("3", editor.CropHeight);
        Assert.AreEqual("", editor.CropError);
        Assert.IsTrue(editor.ApplyCropCommand.CanExecute(null));
        Assert.IsFalse(editor.ShowCropPreview);
        Assert.AreSame(rotated, editor.ActiveDocument);
        Assert.IsTrue(editor.IsDirty);
        Assert.IsTrue(editor.UndoCommand.CanExecute(null));
        Assert.IsFalse(editor.RedoCommand.CanExecute(null));
        CollectionAssert.AreEqual(png, editor.CapturePngSnapshot().Bytes);
        Assert.AreEqual(0, changes);

        editor.Undo();
        var original = editor.ActiveDocument;
        editor.CropX = "1"; editor.CropY = "1"; editor.CropWidth = "1"; editor.CropHeight = "1";
        Assert.IsTrue(editor.ShowCropPreview);
        editor.ResetCropCoordinates();
        Assert.AreEqual("3", editor.CropWidth);
        Assert.AreEqual("2", editor.CropHeight);
        Assert.IsFalse(editor.ShowCropPreview);
        Assert.AreSame(original, editor.ActiveDocument);
        Assert.IsFalse(editor.IsDirty);
        Assert.IsFalse(editor.UndoCommand.CanExecute(null));
        Assert.IsTrue(editor.RedoCommand.CanExecute(null));
        Assert.AreEqual(1, changes);
        CollectionAssert.AreEqual(Pixels, ReadPixels(editor.ActiveDocument!));
        editor.Redo();
        Assert.AreSame(rotated, editor.ActiveDocument);
        editor.ClearDocument();
        Assert.IsFalse(editor.ResetCropCoordinatesCommand.CanExecute(null));
    }, CancellationToken.None);

    [TestMethod]
    public async Task DragCropUsesImageCoordinatesAndDoesNotMutateUntilApply() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        editor.LoadDocument(MakeDocument());
        editor.FlipHorizontal();
        editor.Undo();
        byte[] png = editor.CapturePngSnapshot().Bytes;
        var document = editor.ActiveDocument;
        var window = new Window { Width = 800, Height = 650, Content = new ImageEditorControl { DataContext = editor } };
        try
        {
            window.Show();
            editor.ZoomPercentage = 800;
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            var surface = window.GetVisualDescendants().OfType<Grid>().Single(c => c.Name == "ImageSurface");
            var toggle = window.GetVisualDescendants().OfType<CheckBox>().Single(c => c.Name == "DragCropMode");
            Point At(double x, double y) => surface.TranslatePoint(new Point(x, y), window)!.Value;
            void Down(double x, double y) => window.MouseDown(At(x, y), MouseButton.Left, RawInputModifiers.None);
            void Move(double x, double y) => window.MouseMove(At(x, y), RawInputModifiers.LeftMouseButton);
            void Up(double x, double y) => window.MouseUp(At(x, y), MouseButton.Left, RawInputModifiers.None);

            Down(2.8, 1.8); Move(0.2, 0.2); Up(0.2, 0.2);
            Assert.AreEqual("3", editor.CropWidth, "Mode must be off by default.");
            toggle.IsChecked = true;
            Down(2.8, 1.8);
            Move(1.2, 0.2);
            Assert.IsFalse(editor.ApplyCropCommand.CanExecute(null));
            editor.ApplyCrop();
            Assert.AreSame(document, editor.ActiveDocument);
            Up(1.2, 0.2);
            Assert.AreEqual(new Rect(1, 0, 2, 2), editor.CropPreview);
            Assert.IsTrue(editor.ShowCropPreview);
            Assert.IsFalse(editor.IsDirty);
            Assert.IsFalse(editor.UndoCommand.CanExecute(null));
            Assert.IsTrue(editor.RedoCommand.CanExecute(null));
            CollectionAssert.AreEqual(Pixels, ReadPixels(editor.ActiveDocument!));
            CollectionAssert.AreEqual(png, editor.CapturePngSnapshot().Bytes);

            editor.ApplyCrop();
            Assert.AreEqual(new PixelSize(2, 2), editor.ActiveDocument!.PixelSize);
            Assert.IsTrue(editor.IsDirty);
            editor.Undo();
            Assert.AreSame(document, editor.ActiveDocument);
            CollectionAssert.AreEqual(Pixels, ReadPixels(editor.ActiveDocument!));
            Assert.IsFalse(editor.IsDirty);

            toggle.IsChecked = true;
            Down(1.2, 0.5); Move(-2, 4); Up(-2, 4);
            Assert.AreEqual(new Rect(0, 0, 2, 2), editor.CropPreview);
            Down(1.2, 0.5); Up(1.2, 0.5);
            Assert.IsFalse(editor.ApplyCropCommand.CanExecute(null));
            Assert.AreEqual("0", editor.CropWidth);
        }
        finally { window.Close(); }
    }, CancellationToken.None);

    [TestMethod]
    public async Task DragCropScrollCancellationCaptureAndDocumentReplacement() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        editor.LoadDocument(new ImageEditorDocument("large.png",
            new WriteableBitmap(new PixelSize(1000, 800), new Vector(192, 192), PixelFormats.Bgra8888, AlphaFormat.Premul)));
        var window = new Window { Width = 800, Height = 650, Content = new ImageEditorControl { DataContext = editor } };
        try
        {
            window.Show();
            window.SetRenderScaling(1.25);
            editor.ZoomPercentage = 125;
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            var surface = window.GetVisualDescendants().OfType<Grid>().Single(c => c.Name == "ImageSurface");
            var toggle = window.GetVisualDescendants().OfType<CheckBox>().Single(c => c.Name == "DragCropMode");
            var scroll = window.GetVisualDescendants().OfType<ScrollViewer>().Single(c => c.Name == "ImageScrollViewer");
            scroll.Offset = new Vector(150, 100);
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            toggle.IsChecked = true;
            Point At(double x, double y) => surface.TranslatePoint(new Point(x, y), window)!.Value;
            void Down(double x, double y) => window.MouseDown(At(x, y), MouseButton.Left, RawInputModifiers.None);
            void Move(double x, double y) => window.MouseMove(At(x, y), RawInputModifiers.LeftMouseButton);
            void Up(double x, double y) => window.MouseUp(At(x, y), MouseButton.Left, RawInputModifiers.None);
            IPointer? pointer = null;
            surface.AddHandler(InputElement.PointerPressedEvent, (_, e) => pointer = e.Pointer,
                RoutingStrategies.Bubble, handledEventsToo: true);
            Down(200.2, 130.2); Move(300.6, 200.8); Up(300.6, 200.8);
            Assert.AreEqual(new Rect(200, 130, 101, 71), editor.CropPreview);

            Down(220, 140); Move(250, 180);
            Assert.AreEqual(new Rect(220, 140, 30, 40), editor.CropPreview);
            window.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            Assert.AreEqual(new Rect(200, 130, 101, 71), editor.CropPreview);
            Up(250, 180);
            Down(220, 140); Move(250, 180);
            Assert.IsNotNull(pointer);
            pointer.Capture(null);
            Assert.AreEqual(new Rect(200, 130, 101, 71), editor.CropPreview);
            Up(250, 180);

            Down(220, 140); Move(250, 180);
            toggle.IsChecked = false;
            Assert.AreEqual(new Rect(200, 130, 101, 71), editor.CropPreview);
            Up(250, 180);
            toggle.IsChecked = true;
            Down(220, 140); Move(250, 180);
            editor.CropX = "10";
            Assert.IsFalse(editor.ApplyCropCommand.CanExecute(null));
            Up(250, 180);
            Assert.IsFalse(editor.ApplyCropCommand.CanExecute(null));

            Down(220, 140); Move(250, 180);
            editor.LoadDocument(new ImageEditorDocument("new.png",
                new WriteableBitmap(new PixelSize(1000, 800), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul)));
            Up(250, 180);
            Assert.AreEqual(new Rect(0, 0, 1000, 800), editor.CropPreview);
            Assert.AreEqual(false, toggle.IsChecked);
            Assert.IsFalse(editor.IsDirty);
            Assert.IsFalse(editor.UndoCommand.CanExecute(null));
        }
        finally { window.Close(); }
    }, CancellationToken.None);

    [TestMethod]
    public async Task CropPreviewTracksImageGeometryZoomScrollAndDimensions() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        var window = new Window { Width = 800, Height = 650, Content = new ImageEditorControl { DataContext = editor } };
        try
        {
            window.Show();
            window.UpdateLayout();
            var outline = window.GetVisualDescendants().OfType<CropPreviewOverlay>().Single(c => c.Name == "CropPreviewOutline");
            var surface = window.GetVisualDescendants().OfType<Grid>().Single(c => c.Name == "ImageSurface");
            var image = window.GetVisualDescendants().OfType<Image>().Single(c => c.Name == "EditorImage");
            var background = window.GetVisualDescendants().OfType<Border>().Single(c => c.Name == "TransparencyBackground");
            var scroll = window.GetVisualDescendants().OfType<ScrollViewer>().Single(c => c.Name == "ImageScrollViewer");
            Assert.IsFalse(outline.IsVisible);
            Assert.IsFalse(outline.IsHitTestVisible);
            Assert.IsFalse(background.IsVisible);
            Assert.IsFalse(background.IsHitTestVisible);

            var bitmap = new WriteableBitmap(new PixelSize(1000, 800), new Vector(192, 192), PixelFormats.Bgra8888, AlphaFormat.Premul);
            editor.LoadDocument(new ImageEditorDocument("large.png", bitmap));
            editor.ZoomPercentage = 125;
            editor.CropX = "999"; editor.CropY = "799"; editor.CropWidth = "1"; editor.CropHeight = "1";
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.IsTrue(outline.IsVisible);
            Assert.AreEqual(new Rect(999, 799, 1, 1), outline.CropRectangle);
            Assert.AreEqual(new Size(1000, 800), image.Bounds.Size);
            var imageOrigin = image.TranslatePoint(new Point(0, 0), scroll)!.Value;
            var imageEnd = image.TranslatePoint(new Point(1000, 800), scroll)!.Value;
            Assert.IsTrue(background.IsVisible);
            Assert.AreEqual(image.Bounds.Size, background.Bounds.Size);
            Assert.AreEqual(imageOrigin, background.TranslatePoint(new Point(0, 0), scroll)!.Value);
            Assert.AreEqual(imageEnd, background.TranslatePoint(new Point(1000, 800), scroll)!.Value);
            Assert.AreEqual(1250, imageEnd.X - imageOrigin.X, 0.01);
            Assert.AreEqual(1000, imageEnd.Y - imageOrigin.Y, 0.01);
            var beforeScroll = outline.TranslatePoint(outline.CropRectangle.Position, scroll)!.Value;
            Assert.AreEqual(999 * 1.25, beforeScroll.X - imageOrigin.X, 0.01);
            Assert.AreEqual(799 * 1.25, beforeScroll.Y - imageOrigin.Y, 0.01);
            scroll.Offset = new Vector(150, 100);
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            var afterScroll = outline.TranslatePoint(outline.CropRectangle.Position, scroll)!.Value;
            var backgroundAfterScroll = background.TranslatePoint(new Point(0, 0), scroll)!.Value;
            Assert.AreEqual(150, imageOrigin.X - backgroundAfterScroll.X, 0.01);
            Assert.AreEqual(100, imageOrigin.Y - backgroundAfterScroll.Y, 0.01);
            Assert.AreEqual(150, beforeScroll.X - afterScroll.X, 0.01);
            Assert.AreEqual(100, beforeScroll.Y - afterScroll.Y, 0.01);
            var outlineEnd = outline.TranslatePoint(new Point(1000, 800), scroll)!.Value;
            Assert.AreEqual(1.25, outlineEnd.X - afterScroll.X, 0.01);
            Assert.AreEqual(1.25, outlineEnd.Y - afterScroll.Y, 0.01);

            editor.CropWidth = "2";
            Assert.IsFalse(outline.IsVisible);
            editor.CropX = "0"; editor.CropY = "0"; editor.CropWidth = "1000"; editor.CropHeight = "800";
            Assert.IsFalse(outline.IsVisible);
            editor.CropWidth = "999";
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.IsTrue(outline.IsVisible);
            Assert.AreEqual(new Rect(0, 0, 999, 800), outline.CropRectangle);
            Assert.AreEqual(new Size(1000, 800), surface.Bounds.Size);
            using (var rendered = new RenderTargetBitmap(new PixelSize(1000, 800)))
            using (var copy = new WriteableBitmap(new PixelSize(1000, 800), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul))
            {
                rendered.Render(outline);
                using var buffer = copy.Lock();
                rendered.CopyPixels(buffer);
                Assert.AreNotEqual(0, Marshal.ReadByte(buffer.Address, 500 * 4 + 3));
                Assert.AreEqual(0, Marshal.ReadByte(buffer.Address, 400 * buffer.RowBytes + 500 * 4 + 3));
            }
            editor.ZoomPercentage = 10;
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            var smallOrigin = outline.TranslatePoint(new Point(0, 0), scroll)!.Value;
            var smallEnd = outline.TranslatePoint(new Point(999, 800), scroll)!.Value;
            Assert.AreEqual(99.9, smallEnd.X - smallOrigin.X, 0.01);
            Assert.AreEqual(80, smallEnd.Y - smallOrigin.Y, 0.01);
            Assert.AreEqual(0.1, outline.Zoom, 0.0001);
            editor.ZoomPercentage = 125;
            editor.RotateRight();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.IsFalse(outline.IsVisible);
            Assert.AreEqual(new Size(800, 1000), image.Bounds.Size);
            editor.CropX = "799"; editor.CropY = "999"; editor.CropWidth = "1"; editor.CropHeight = "1";
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.IsTrue(outline.IsVisible);
            Assert.AreEqual(new Rect(799, 999, 1, 1), outline.CropRectangle);
            editor.ClearDocument();
            Assert.IsFalse(outline.IsVisible);
            Assert.IsFalse(background.IsVisible);
        }
        finally { window.Close(); }
    }, CancellationToken.None);

    [TestMethod]
    public async Task CropPreviewDoesNotChangePngPixelsDirtyOrHistory() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        editor.LoadDocument(MakeDocument());
        editor.FlipHorizontal();
        editor.Undo();
        var document = editor.ActiveDocument;
        var png = editor.CapturePngSnapshot().Bytes;
        int changes = 0;
        editor.ContentChanging += (_, _) => changes++;
        var window = new Window { Width = 800, Height = 650, Content = new ImageEditorControl { DataContext = editor } };
        try
        {
            window.Show();
            editor.CropX = "-1";
            editor.CropX = "1"; editor.CropY = "1"; editor.CropWidth = "2"; editor.CropHeight = "1";
            editor.ZoomPercentage = 175;
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            var outline = window.GetVisualDescendants().OfType<CropPreviewOverlay>().Single(c => c.Name == "CropPreviewOutline");
            Assert.IsTrue(outline.IsVisible);
            Assert.AreSame(document, editor.ActiveDocument);
            Assert.IsFalse(editor.IsDirty);
            Assert.IsFalse(editor.UndoCommand.CanExecute(null));
            Assert.IsTrue(editor.RedoCommand.CanExecute(null));
            Assert.AreEqual(0, changes);
            CollectionAssert.AreEqual(Pixels, ReadPixels(editor.ActiveDocument!));
            CollectionAssert.AreEqual(png, editor.CapturePngSnapshot().Bytes);
            editor.Redo();
            CollectionAssert.AreEqual(FlippedPixels, ReadPixels(editor.ActiveDocument!));
        }
        finally { window.Close(); }
    }, CancellationToken.None);

    [TestMethod]
    public async Task FitIsStableWithSpaceConsumingScrollbars() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        var window = new Window { Width = 1000, Height = 750, Content = new ImageEditorControl { DataContext = editor } };
        try
        {
            window.Show();
            editor.LoadDocument(new ImageEditorDocument("wide.png",
                new WriteableBitmap(new PixelSize(1400, 300), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul)));
            editor.ZoomPercentage = 200;
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            var scroll = window.GetVisualDescendants().OfType<ScrollViewer>().Single(c => c.Name == "ImageScrollViewer");
            scroll.AllowAutoHide = false;
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            var fit = window.GetVisualDescendants().OfType<Button>().Single(c => c.Name == "FitImageButton");
            var before = scroll.Viewport;
            fit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            double first = editor.ZoomPercentage;
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            var after = scroll.Viewport;
            fit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.AreEqual(first, editor.ZoomPercentage, 0.000001,
                $"Repeated fit changed zoom: {first} -> {editor.ZoomPercentage}; viewport {before} -> {after}");
            Assert.AreEqual(before, after, $"Scrollbar layout changed viewport: {before} -> {after}");
            Assert.AreEqual(Math.Clamp(Math.Min(after.Width / 1400, after.Height / 300) * 100, 10, 800),
                first, 0.000001);
            foreach (double scaling in new[] { 1.25, 2.0 })
            {
                window.SetRenderScaling(scaling);
                editor.ZoomPercentage = 200;
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();
                before = scroll.Viewport;
                fit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                first = editor.ZoomPercentage;
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();
                after = scroll.Viewport;
                fit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                Assert.AreEqual(before, after, $"Viewport changed at {scaling} scaling.");
                Assert.AreEqual(first, editor.ZoomPercentage, 0.000001);
                Assert.AreEqual(Math.Clamp(Math.Min(after.Width / 1400, after.Height / 300) * 100, 10, 800),
                    first, 0.000001);
            }
            editor.LoadDocument(MakeDocument());
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.AreEqual(100, editor.ZoomPercentage);
            window.Content = new Border();
            editor.ZoomPercentage = 123;
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.AreEqual(123, editor.ZoomPercentage);
        }
        finally { window.Close(); }
    }, CancellationToken.None);

    [TestMethod]
    public async Task FitUsesCurrentViewportAndPixelDimensionsWithoutFollowingResize() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        var control = new ImageEditorControl { DataContext = editor };
        var window = new Window { Width = 1000, Height = 750, Content = control };
        try
        {
            window.Show();
            window.UpdateLayout();
            var scroll = window.GetVisualDescendants().OfType<ScrollViewer>().Single(c => c.Name == "ImageScrollViewer");
            var image = window.GetVisualDescendants().OfType<Image>().Single(c => c.Name == "EditorImage");
            var fit = window.GetVisualDescendants().OfType<Button>().Single(c => c.Name == "FitImageButton");

            void Layout()
            {
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();
            }

            double? landscapeZoom = null;
            foreach (var (width, height, dpi) in new[] { (1400, 300, 96), (1400, 300, 192), (300, 1200, 192) })
            {
                editor.LoadDocument(new ImageEditorDocument("aspect.png",
                    new WriteableBitmap(new PixelSize(width, height), new Vector(dpi, dpi), PixelFormats.Bgra8888, AlphaFormat.Premul)));
                editor.ZoomPercentage = 200;
                Layout();
                scroll.Offset = new Vector(300, 250);
                Layout();
                Size viewport = scroll.Viewport;
                double expected = Math.Clamp(Math.Min(viewport.Width / width, viewport.Height / height) * 100, 10, 800);
                fit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                Assert.AreEqual(expected, editor.ZoomPercentage, 0.000001);
                if (width == 1400 && dpi == 96) landscapeZoom = editor.ZoomPercentage;
                if (width == 1400 && dpi == 192) Assert.AreEqual(landscapeZoom!.Value, editor.ZoomPercentage, 0.000001);
                Layout();
                var start = image.TranslatePoint(new Point(0, 0), scroll)!.Value;
                var end = image.TranslatePoint(new Point(width, height), scroll)!.Value;
                Assert.IsTrue(start.X >= -0.02 && start.Y >= -0.02, $"Image starts outside viewport: {start}");
                Assert.IsTrue(end.X <= scroll.Viewport.Width + 0.02 && end.Y <= scroll.Viewport.Height + 0.02,
                    $"Image ends outside viewport: {end}, viewport {scroll.Viewport}");
                Assert.AreEqual(width * editor.ZoomFactor, end.X - start.X, 0.02);
                Assert.AreEqual(height * editor.ZoomFactor, end.Y - start.Y, 0.02);
                if (width == 1400 && dpi == 96)
                {
                    var expandedViewport = scroll.Viewport;
                    double repeatedFit = Math.Clamp(Math.Min(expandedViewport.Width / width, expandedViewport.Height / height) * 100, 10, 800);
                    fit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    Assert.AreEqual(repeatedFit, editor.ZoomPercentage, 0.000001);
                    Assert.AreEqual(expected, editor.ZoomPercentage, 0.000001);
                }
            }

            double beforeResize = editor.ZoomPercentage;
            window.Width = 800;
            window.Height = 600;
            Layout();
            Assert.AreEqual(beforeResize, editor.ZoomPercentage);
            Size resizedViewport = scroll.Viewport;
            fit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.AreEqual(Math.Min(resizedViewport.Width / 300, resizedViewport.Height / 1200) * 100,
                editor.ZoomPercentage, 0.000001);
            editor.LoadDocument(new ImageEditorDocument("narrow.png",
                new WriteableBitmap(new PixelSize(1400, 300), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul)));
            window.Width = 380;
            window.Height = 550;
            editor.ZoomPercentage = 200;
            Layout();
            Assert.IsTrue(scroll.Viewport.Width < 640);
            scroll.Offset = new Vector(400, 0);
            Layout();
            fit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Layout();
            var narrowStart = image.TranslatePoint(new Point(0, 0), scroll)!.Value;
            var narrowEnd = image.TranslatePoint(new Point(1400, 300), scroll)!.Value;
            Assert.IsTrue(narrowStart.X >= -0.02 && narrowStart.Y >= -0.02,
                $"start {narrowStart}, end {narrowEnd}, viewport {scroll.Viewport}, extent {scroll.Extent}, offset {scroll.Offset}, zoom {editor.ZoomPercentage}");
            Assert.IsTrue(narrowEnd.X <= scroll.Viewport.Width + 0.02 && narrowEnd.Y <= scroll.Viewport.Height + 0.02,
                $"start {narrowStart}, end {narrowEnd}, viewport {scroll.Viewport}, extent {scroll.Extent}, offset {scroll.Offset}, zoom {editor.ZoomPercentage}");
            foreach (double scaling in new[] { 1.25, 2.0 })
            {
                window.SetRenderScaling(scaling);
                editor.ZoomPercentage = 200;
                Layout();
                var scaledViewport = scroll.Viewport;
                fit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                Assert.AreEqual(Math.Clamp(Math.Min(scaledViewport.Width / 1400, scaledViewport.Height / 300) * 100, 10, 800),
                    editor.ZoomPercentage, 0.000001);
                Layout();
                var scaledStart = image.TranslatePoint(new Point(0, 0), scroll)!.Value;
                var scaledEnd = image.TranslatePoint(new Point(1400, 300), scroll)!.Value;
                Assert.IsTrue(scaledStart.X >= -0.02 && scaledStart.Y >= -0.02);
                Assert.IsTrue(scaledEnd.X <= scroll.Viewport.Width + 0.02 && scaledEnd.Y <= scroll.Viewport.Height + 0.02);
            }
        }
        finally { window.Close(); }
    }, CancellationToken.None);

    [TestMethod]
    public async Task FitHonorsLimitsAndIgnoresMissingOrUnrealizedViewport() =>
        await _session.Dispatch(() =>
    {
        using var editor = new ImageEditorViewModel();
        var control = new ImageEditorControl { DataContext = editor };
        var fit = control.FindControl<Button>("FitImageButton")!;
        Assert.IsFalse(fit.IsEffectivelyEnabled);
        fit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Assert.AreEqual(100, editor.ZoomPercentage);
        editor.LoadDocument(MakeDocument());
        fit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Assert.AreEqual(100, editor.ZoomPercentage, "An unrealized viewport must not change zoom.");

        var window = new Window { Width = 700, Height = 600, Content = control };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            fit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.AreEqual(800, editor.ZoomPercentage);
            editor.LoadDocument(new ImageEditorDocument("large.png",
                new WriteableBitmap(new PixelSize(8192, 512), new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul)));
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            var scroll = window.GetVisualDescendants().OfType<ScrollViewer>().Single(c => c.Name == "ImageScrollViewer");
            Assert.IsTrue(scroll.Viewport.Width < 819.2);
            fit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.AreEqual(10, editor.ZoomPercentage);
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.IsTrue(editor.ActiveDocument!.DisplayImage.Size.Width * editor.ZoomFactor > scroll.Viewport.Width);

            scroll.Height = 0;
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.AreEqual(0, scroll.Bounds.Height, 0.001);
            editor.ZoomPercentage = 175;
            fit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.AreEqual(175, editor.ZoomPercentage, "A zero-sized viewport must not change zoom.");
            scroll.Height = double.NaN;
            control.IsVisible = false;
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            fit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.AreEqual(175, editor.ZoomPercentage, "A hidden viewport must not change zoom.");
            control.IsVisible = true;
            editor.ClearDocument();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.IsFalse(fit.IsEffectivelyEnabled);
            fit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.AreEqual(175, editor.ZoomPercentage);
        }
        finally { window.Close(); }
    }, CancellationToken.None);

    [TestMethod]
    public async Task StandaloneDragCropSelectsWithoutEditing() =>
        await _session.Dispatch(() =>
    {
        var host = new MainWindowViewModel();
        var window = new MainWindow { DataContext = host, Width = 1500, Height = 900 };
        try
        {
            host.Editor.LoadDocument(MakeDocument());
            host.Editor.ZoomPercentage = 800;
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            var surface = window.GetVisualDescendants().OfType<Grid>().Single(c => c.Name == "ImageSurface");
            var toggle = window.GetVisualDescendants().OfType<CheckBox>().Single(c => c.Name == "DragCropMode");
            var png = host.Editor.CapturePngSnapshot().Bytes;
            toggle.IsChecked = true;
            var start = surface.TranslatePoint(new Point(1.1, 0.1), window)!.Value;
            var end = surface.TranslatePoint(new Point(2.9, 1.9), window)!.Value;
            window.MouseDown(start, MouseButton.Left, RawInputModifiers.None);
            window.MouseMove(end, RawInputModifiers.LeftMouseButton);
            window.MouseUp(end, MouseButton.Left, RawInputModifiers.None);
            Assert.AreEqual(new Rect(1, 0, 2, 2), host.Editor.CropPreview);
            Assert.IsFalse(host.Editor.IsDirty);
            CollectionAssert.AreEqual(png, host.Editor.CapturePngSnapshot().Bytes);
            Click(window, "Apply crop");
            Assert.AreEqual(new PixelSize(2, 2), host.Editor.ActiveDocument!.PixelSize);
            host.Editor.Undo();
            CollectionAssert.AreEqual(Pixels, ReadPixels(host.Editor.ActiveDocument!));
        }
        finally { host.Editor.Dispose(); window.Close(); }
    }, CancellationToken.None);

    [TestMethod]
    public async Task StandaloneFitPreservesCropPixelsDirtyAndRedo() =>
        await _session.Dispatch(() =>
    {
        var host = new MainWindowViewModel();
        var window = new MainWindow { DataContext = host, Width = 1500, Height = 900 };
        try
        {
            window.Show();
            window.UpdateLayout();
            var fit = window.GetVisualDescendants().OfType<Button>().Single(c => c.Name == "FitImageButton");
            Assert.IsFalse(fit.IsEffectivelyEnabled);
            host.Editor.LoadDocument(MakeDocument());
            host.Editor.FlipHorizontal();
            host.Editor.Undo();
            host.Editor.CropX = "1";
            host.Editor.CropWidth = "1";
            var original = host.Editor.ActiveDocument;
            var state = host.Editor.CaptureDocumentState();
            var png = host.Editor.CapturePngSnapshot().Bytes;
            int changes = 0;
            host.Editor.ContentChanging += (_, _) => changes++;
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.IsTrue(fit.IsEffectivelyEnabled);
            Click(window, "Fit (min 10%)");
            Assert.AreEqual(800, host.Editor.ZoomPercentage);
            Assert.AreSame(original, host.Editor.ActiveDocument);
            Assert.IsTrue(host.Editor.IsCurrentDocumentState(state));
            Assert.IsFalse(host.Editor.IsDirty);
            Assert.IsFalse(host.Editor.UndoCommand.CanExecute(null));
            Assert.IsTrue(host.Editor.RedoCommand.CanExecute(null));
            Assert.AreEqual(0, changes);
            Assert.AreEqual(new Rect(1, 0, 1, 2), host.Editor.CropPreview);
            Assert.IsTrue(host.Editor.ShowCropPreview);
            var overlay = window.GetVisualDescendants().OfType<CropPreviewOverlay>().Single();
            Assert.IsTrue(overlay.IsVisible);
            Assert.AreEqual(8, overlay.Zoom, 0.000001);
            CollectionAssert.AreEqual(Pixels, ReadPixels(host.Editor.ActiveDocument!));
            CollectionAssert.AreEqual(png, host.Editor.CapturePngSnapshot().Bytes);
            host.Editor.Redo();
            CollectionAssert.AreEqual(FlippedPixels, ReadPixels(host.Editor.ActiveDocument!));
            var dirtyState = host.Editor.CaptureDocumentState();
            int dirtyChanges = changes;
            Click(window, "Fit (min 10%)");
            Assert.IsTrue(host.Editor.IsCurrentDocumentState(dirtyState));
            Assert.IsTrue(host.Editor.IsDirty);
            Assert.AreEqual(dirtyChanges, changes);
            host.Editor.Undo();
            CollectionAssert.AreEqual(Pixels, ReadPixels(host.Editor.ActiveDocument!));
        }
        finally { host.Editor.Dispose(); window.Close(); }
    }, CancellationToken.None);

    [TestMethod]
    public async Task StandaloneCropInputsAndApplyAreBoundToEditor() =>
        await _session.Dispatch(() =>
    {
        var host = new MainWindowViewModel();
        var window = new MainWindow { DataContext = host, Width = 1500, Height = 900 };
        try
        {
            window.Show();
            window.UpdateLayout();
            var inputs = window.GetVisualDescendants().OfType<TextBox>().Where(t => t.Name?.StartsWith("Crop", StringComparison.Ordinal) == true).ToArray();
            Assert.AreEqual(4, inputs.Length);
            var button = window.GetVisualDescendants().OfType<Button>().Single(b => Equals(b.Content, "Apply crop"));
            var reset = window.GetVisualDescendants().OfType<Button>().Single(b => b.Name == "ResetCropCoordinatesButton");
            Assert.IsFalse(reset.IsEffectivelyEnabled);
            host.Editor.LoadDocument(MakeDocument());
            Dispatcher.UIThread.RunJobs();
            Assert.IsTrue(reset.IsEffectivelyEnabled);
            inputs.Single(t => t.Name == "CropXInput").SetCurrentValue(TextBox.TextProperty, "-1");
            Dispatcher.UIThread.RunJobs();
            Assert.AreEqual("-1", host.Editor.CropX);
            Assert.IsFalse(host.Editor.ApplyCropCommand.CanExecute(null));
            Assert.IsFalse(button.IsEffectivelyEnabled);
            Assert.IsFalse(string.IsNullOrEmpty(host.Editor.CropError));
            Assert.IsTrue(window.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == host.Editor.CropError && t.IsVisible));
            Click(window, "Reset crop coordinates");
            Assert.AreEqual("0", inputs.Single(t => t.Name == "CropXInput").Text);
            Assert.IsTrue(button.IsEffectivelyEnabled);
            Assert.AreEqual("", host.Editor.CropError);
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
    public async Task ExportPickerKeepsOriginalDocumentWhenAnotherIsLoaded()
    {
        await _session.Dispatch(async () =>
        {
            string directory = Path.Combine(AppContext.BaseDirectory, "editor-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                string source = Path.Combine(directory, "source.png");
                string otherSource = Path.Combine(directory, "other.png");
                string destination = Path.Combine(directory, "export.png");
                using var host = new MainWindowViewModel();
                host.Editor.LoadDocument(MakeDocument());
                byte[] sourceBytes = host.Editor.CapturePngSnapshot().Bytes;
                File.WriteAllBytes(source, sourceBytes);
                host.Editor.RotateLeft();
                await Assert.ThrowsAsync<IOException>(() => host.ExportPngToNewFileAsync(source));
                CollectionAssert.AreEqual(sourceBytes, File.ReadAllBytes(source));
                Assert.IsTrue(host.Editor.IsDirty);
                var picker = new TaskCompletionSource<IStorageFile?>();
                var provider = DispatchProxy.Create<IStorageProvider, StorageProxy>();
                ((StorageProxy)(object)provider).Handler = method => method.Name switch
                {
                    "get_CanSave" => true,
                    "SaveFilePickerAsync" => picker.Task,
                    _ => throw new NotSupportedException(method.Name)
                };
                var file = DispatchProxy.Create<IStorageFile, StorageProxy>();
                ((StorageProxy)(object)file).Handler = method => method.Name switch
                {
                    "get_Path" => new Uri(destination),
                    _ => throw new NotSupportedException(method.Name)
                };
                Task export = host.ExportPngAsync(provider);
                host.Editor.LoadDocument(MakeDocument());
                host.Editor.FlipHorizontal();
                byte[] otherBytes = host.Editor.CapturePngSnapshot().Bytes;
                File.WriteAllBytes(otherSource, otherBytes);
                picker.SetResult(file);
                await export;
                Assert.IsTrue(host.Editor.IsDirty);
                Assert.IsTrue(host.Editor.DocumentSummary.StartsWith("original.png", StringComparison.Ordinal));
                CollectionAssert.AreEqual(otherBytes, host.Editor.CapturePngSnapshot().Bytes);
                CollectionAssert.AreEqual(otherBytes, File.ReadAllBytes(otherSource));
                CollectionAssert.AreEqual(sourceBytes, File.ReadAllBytes(source));
                using var decoded = new ImageEditorViewModel();
                decoded.LoadDocument(File.ReadAllBytes(destination), "export.png");
                CollectionAssert.AreEqual(LeftRotatedPixels, ReadPixels(decoded.ActiveDocument!));
                using var sourceEditor = new ImageEditorViewModel();
                sourceEditor.LoadDocument(File.ReadAllBytes(source), "source.png");
                CollectionAssert.AreEqual(Pixels, ReadPixels(sourceEditor.ActiveDocument!));
            }
            finally { Directory.Delete(directory, recursive: true); }
            return true;
        }, CancellationToken.None);
    }

    public class StorageProxy : DispatchProxy
    {
        public Func<MethodInfo, object?> Handler { get; set; } = null!;
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => Handler(targetMethod!);
    }

    private sealed class DelayedReadStream : MemoryStream
    {
        public readonly TaskCompletionSource<bool> Started = new();
        public readonly TaskCompletionSource<bool> Release = new();

        public DelayedReadStream(byte[] bytes) : base(bytes) { }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Started.TrySetResult(true);
            await Release.Task;
            return await base.ReadAsync(buffer, offset, count, cancellationToken);
        }
    }

    private static IStorageFile OpenFile(string path, Func<Task<Stream>> open)
    {
        var file = DispatchProxy.Create<IStorageFile, StorageProxy>();
        ((StorageProxy)(object)file).Handler = method => method.Name switch
        {
            "get_Name" => Path.GetFileName(path),
            "OpenReadAsync" => open(),
            _ => throw new NotSupportedException(method.Name)
        };
        return file;
    }

    private static IStorageProvider OpenProvider(Task<IReadOnlyList<IStorageFile>> picker)
    {
        var provider = DispatchProxy.Create<IStorageProvider, StorageProxy>();
        ((StorageProxy)(object)provider).Handler = method => method.Name switch
        {
            "get_CanOpen" => true,
            "OpenFilePickerAsync" => picker,
            _ => throw new NotSupportedException(method.Name)
        };
        return provider;
    }

    [TestMethod]
    [DataRow("picker")]
    [DataRow("read")]
    [DataRow("confirmation")]
    [DataRow("confirmation-replace")]
    [DataRow("confirmation-replace-clean")]
    [DataRow("confirmation-save")]
    [DataRow("empty-picker")]
    public async Task OpenDoesNotReplaceNewDocumentOrApplyStaleDiscardConfirmation(string stage)
    {
        await _session.Dispatch(async () =>
        {
            string directory = Path.Combine(AppContext.BaseDirectory, "open-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                string path = Path.Combine(directory, "selected.png");
                using var host = new MainWindowViewModel();
                host.Editor.LoadDocument(MakeDocument());
                byte[] source = host.Editor.CapturePngSnapshot().Bytes;
                File.WriteAllBytes(path, source);
                if (stage == "empty-picker")
                {
                    host.Editor.AllowClear = true;
                    host.Editor.ClearDocument();
                }
                var file = OpenFile(path, () => Task.FromResult<Stream>(File.OpenRead(path)));
                var picker = new TaskCompletionSource<IReadOnlyList<IStorageFile>>();
                var delayed = new DelayedReadStream(source);
                var confirmation = new TaskCompletionSource<bool>();
                var confirming = new TaskCompletionSource<bool>();
                if (stage == "read") file = OpenFile(path, () => Task.FromResult<Stream>(delayed));
                bool confirmationStage = stage.StartsWith("confirmation", StringComparison.Ordinal);
                if (confirmationStage) host.Editor.RotateLeft();
                if (stage != "picker" && stage != "empty-picker") picker.SetResult(new[] { file });
                Task opening = host.OpenImageAsync(OpenProvider(picker.Task), () =>
                {
                    confirming.TrySetResult(true);
                    return confirmationStage ? confirmation.Task : Task.FromResult(true);
                });
                if (stage == "read") await delayed.Started.Task;
                if (confirmationStage) await confirming.Task;
                if (!confirmationStage || stage.StartsWith("confirmation-replace", StringComparison.Ordinal)) host.Editor.LoadDocument(MakeDocument());
                if (stage == "confirmation-save") host.Editor.MarkSaved(host.Editor.CapturePngSnapshot());
                else if (stage != "confirmation-replace-clean") host.Editor.FlipHorizontal();
                byte[] other = host.Editor.CapturePngSnapshot().Bytes;
                if (stage == "picker" || stage == "empty-picker") picker.SetResult(new[] { file });
                if (stage == "read") delayed.Release.SetResult(true);
                if (confirmationStage) confirmation.SetResult(true);
                await opening;
                Assert.AreEqual(stage != "confirmation-save" && stage != "confirmation-replace-clean", host.Editor.IsDirty);
                CollectionAssert.AreEqual(other, host.Editor.CapturePngSnapshot().Bytes);
                CollectionAssert.AreEqual(source, File.ReadAllBytes(path));
                Assert.AreEqual("Open canceled; the document changed.", host.StatusText);
            }
            finally { Directory.Delete(directory, recursive: true); }
            return true;
        }, CancellationToken.None);
    }

    [TestMethod]
    [DataRow("clean")]
    [DataRow("dirty-confirm")]
    [DataRow("dirty-decline")]
    [DataRow("confirmation-canceled")]
    [DataRow("default-decline")]
    [DataRow("picker-cancel")]
    [DataRow("picker-task-canceled")]
    [DataRow("read-failure")]
    [DataRow("invalid")]
    [DataRow("edit-during-read")]
    public async Task OpenPositiveAndCanceledCasesPreserveFilesAndDirtyState(string scenario)
    {
        await _session.Dispatch(async () =>
        {
            string directory = Path.Combine(AppContext.BaseDirectory, "open-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                string path = Path.Combine(directory, "selected.png");
                using var host = new MainWindowViewModel();
                host.Editor.LoadDocument(MakeDocument());
                byte[] source = host.Editor.CapturePngSnapshot().Bytes;
                byte[] input = scenario == "invalid" ? new byte[] { 1, 2, 3 } : source;
                File.WriteAllBytes(path, input);
                if (scenario.StartsWith("dirty", StringComparison.Ordinal) || scenario == "confirmation-canceled" ||
                    scenario == "default-decline" || scenario == "read-failure" || scenario == "picker-task-canceled") host.Editor.RotateLeft();
                var delayed = new DelayedReadStream(input);
                var file = OpenFile(path, () => scenario == "read-failure"
                    ? Task.FromException<Stream>(new IOException("Read failed."))
                    : scenario == "edit-during-read" ? Task.FromResult<Stream>(delayed) : Task.FromResult<Stream>(File.OpenRead(path)));
                var files = scenario == "picker-cancel" ? Array.Empty<IStorageFile>() : new[] { file };
                int confirmations = 0;
                var provider = OpenProvider(scenario == "picker-task-canceled"
                    ? Task.FromCanceled<IReadOnlyList<IStorageFile>>(new CancellationToken(true))
                    : Task.FromResult<IReadOnlyList<IStorageFile>>(files));
                Task opening = scenario == "default-decline" ? host.OpenImageAsync(provider) : host.OpenImageAsync(provider, () =>
                {
                    confirmations++;
                    if (scenario == "confirmation-canceled") return Task.FromCanceled<bool>(new CancellationToken(true));
                    return Task.FromResult(scenario != "dirty-decline");
                });
                if (scenario == "edit-during-read")
                {
                    await delayed.Started.Task;
                    host.Editor.RotateLeft();
                    delayed.Release.SetResult(true);
                }
                byte[] before = host.Editor.CapturePngSnapshot().Bytes;
                if (scenario == "invalid")
                    await Assert.ThrowsAsync<InvalidDataException>(() => opening);
                else if (scenario == "read-failure")
                    await Assert.ThrowsAsync<IOException>(() => opening);
                else if (scenario == "confirmation-canceled" || scenario == "picker-task-canceled")
                {
                    try { await opening; Assert.Fail("Expected cancellation."); }
                    catch (OperationCanceledException) { }
                }
                else
                    await opening;
                bool loaded = scenario == "clean" || scenario == "dirty-confirm" || scenario == "edit-during-read";
                Assert.AreEqual(scenario == "dirty-decline" || scenario == "confirmation-canceled" || scenario == "default-decline" ||
                    scenario == "read-failure" || scenario == "picker-task-canceled", host.Editor.IsDirty);
                if (loaded)
                {
                    using var expected = new ImageEditorViewModel();
                    expected.LoadDocument(source, "expected.png");
                    CollectionAssert.AreEqual(ReadPixels(expected.ActiveDocument!), ReadPixels(host.Editor.ActiveDocument!));
                }
                else CollectionAssert.AreEqual(before, host.Editor.CapturePngSnapshot().Bytes);
                Assert.AreEqual(loaded ? "selected.png" : "original.png", host.Editor.ActiveDocument!.Name);
                Assert.AreEqual(scenario.StartsWith("dirty", StringComparison.Ordinal) || scenario == "edit-during-read" || scenario == "confirmation-canceled" ? 1 : 0, confirmations);
                CollectionAssert.AreEqual(input, File.ReadAllBytes(path));
            }
            finally { Directory.Delete(directory, recursive: true); }
            return true;
        }, CancellationToken.None);
    }

    [TestMethod]
    public async Task CanceledPickerDoesNotSaveOrRename()
    {
        await _session.Dispatch(async () =>
        {
            using var host = new MainWindowViewModel();
            host.Editor.LoadDocument(MakeDocument());
            host.Editor.RotateLeft();
            var provider = DispatchProxy.Create<IStorageProvider, StorageProxy>();
            ((StorageProxy)(object)provider).Handler = method => method.Name switch
            {
                "get_CanSave" => true,
                "SaveFilePickerAsync" => Task.FromResult<IStorageFile?>(null),
                _ => throw new NotSupportedException(method.Name)
            };
            byte[] before = host.Editor.CapturePngSnapshot().Bytes;
            await host.ExportPngAsync(provider);
            Assert.IsTrue(host.Editor.IsDirty);
            Assert.IsTrue(host.Editor.DocumentSummary.StartsWith("original.png", StringComparison.Ordinal));
            CollectionAssert.AreEqual(before, host.Editor.CapturePngSnapshot().Bytes);
            Assert.AreEqual("Export canceled.", host.StatusText);
            return true;
        }, CancellationToken.None);
    }

    [TestMethod]
    public async Task DelayedExportDoesNotMarkOrRenameAnotherDocument()
    {
        await _session.Dispatch(async () =>
        {
            string directory = Path.Combine(AppContext.BaseDirectory, "editor-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                var started = new TaskCompletionSource<bool>();
                var release = new TaskCompletionSource<bool>();
                using var host = new MainWindowViewModel((path, write) =>
                    PngExportFile.WriteNewAsync(path, async (stream, token) =>
                    {
                        started.SetResult(true);
                        await release.Task;
                        await write(stream, token);
                    }));
                host.Editor.LoadDocument(MakeDocument());
                string source = Path.Combine(directory, "source.png");
                byte[] sourceBytes = host.Editor.CapturePngSnapshot().Bytes;
                File.WriteAllBytes(source, sourceBytes);
                host.Editor.RotateLeft();
                string destination = Path.Combine(directory, "export.png");
                Task export = host.ExportPngToNewFileAsync(destination);
                await started.Task;
                host.Editor.LoadDocument(MakeDocument());
                host.Editor.FlipHorizontal();
                byte[] otherBytes = host.Editor.CapturePngSnapshot().Bytes;
                string other = Path.Combine(directory, "other.png");
                File.WriteAllBytes(other, otherBytes);
                release.SetResult(true);
                await export;
                Assert.IsTrue(host.Editor.IsDirty);
                Assert.IsTrue(host.Editor.DocumentSummary.StartsWith("original.png", StringComparison.Ordinal));
                CollectionAssert.AreEqual(otherBytes, host.Editor.CapturePngSnapshot().Bytes);
                CollectionAssert.AreEqual(otherBytes, File.ReadAllBytes(other));
                CollectionAssert.AreEqual(sourceBytes, File.ReadAllBytes(source));
                using var decoded = new ImageEditorViewModel();
                decoded.LoadDocument(File.ReadAllBytes(destination), "export.png");
                CollectionAssert.AreEqual(LeftRotatedPixels, ReadPixels(decoded.ActiveDocument!));
            }
            finally { Directory.Delete(directory, recursive: true); }
            return true;
        }, CancellationToken.None);
    }

    [TestMethod]
    public async Task ConcurrentDestinationCreationIsNotOverwritten()
    {
        string directory = Path.Combine(AppContext.BaseDirectory, "editor-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            string path = Path.Combine(directory, "export.png");
            byte[] competingBytes = { 7, 8, 9 };
            await Assert.ThrowsAsync<IOException>(() => PngExportFile.WriteNewAsync(path, async (stream, token) =>
            {
                await stream.WriteAsync(new byte[] { 1, 2, 3 }, 0, 3, token);
                File.WriteAllBytes(path, competingBytes);
            }));
            CollectionAssert.AreEqual(competingBytes, File.ReadAllBytes(path));
            Assert.AreEqual(1, Directory.GetFiles(directory).Length);
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

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
                Click(window, "Convert to grayscale");
                Assert.IsTrue(host.Editor.IsDirty);
                byte[] gray = ReadPixels(host.Editor.ActiveDocument!);
                Click(window, "Undo");
                Assert.IsFalse(host.Editor.IsDirty);
                Click(window, "Redo");
                CollectionAssert.AreEqual(gray, ReadPixels(host.Editor.ActiveDocument!));
                Click(window, "Undo");
                Click(window, "Invert colors");
                Assert.IsTrue(host.Editor.IsDirty);
                byte[] inverted = ReadPixels(host.Editor.ActiveDocument!);
                Click(window, "Undo");
                Assert.IsFalse(host.Editor.IsDirty);
                Click(window, "Redo");
                CollectionAssert.AreEqual(inverted, ReadPixels(host.Editor.ActiveDocument!));
                Click(window, "Undo");
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
