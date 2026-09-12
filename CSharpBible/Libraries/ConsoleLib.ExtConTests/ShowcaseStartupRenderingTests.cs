using BaseLib.Interfaces;
using ConsoleLib.CommonControls;
using ConsoleLib.Data;
using ConsoleLib.ExtCon;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace ConsoleLibTests;

[TestClass]
public sealed class ShowcaseStartupRenderingTests
{
    [TestInitialize]
    public void SetUp()
    {
        ConsoleFramework.ExtendedConsole = null;
        ResetCanvas();
    }

    [TestCleanup]
    public void TearDown()
    {
        ConsoleFramework.ExtendedConsole = null;
        ResetCanvas();
    }

    [TestMethod]
    public void AttachControlWritesInitialCanonicalFrame()
    {
        var console = new RecordingConsole(10, 4);
        var widgetSet = new ConsoleWidgetSet(console, new RecordingExtendedConsole());
        var button = new Button { size = new Size(8, 1), Text = "OK" };

        widgetSet.AttachControl(button);

        Assert.AreEqual('O', console[3, 0]);
        Assert.AreEqual('K', console[4, 0]);
        Assert.IsTrue(console.Writes.Count > 0);
    }

    [TestMethod]
    public void ResizeRaisedDuringStartupUpdatesCanvasBeforeFirstFrame()
    {
        var console = new RecordingConsole(10, 4);
        var extendedConsole = new RecordingExtendedConsole();
        var widgetSet = new ConsoleWidgetSet(console, extendedConsole);

        extendedConsole.RaiseResize(new Point(6, 3));

        Assert.AreEqual(new Rectangle(0, 0, 6, 3), ConsoleFramework.Canvas.ClipRect);

        var button = new Button { size = new Size(6, 3), Text = "OK" };
        widgetSet.AttachControl(button);

        Assert.IsTrue(console.Writes.All(write => write.Position.X < 6 && write.Position.Y < 3));
    }

    [TestMethod]
    public void StartupRenderingDoesNotClearHost()
    {
        var console = new RecordingConsole(10, 4);
        var widgetSet = new ConsoleWidgetSet(console, new RecordingExtendedConsole());

        widgetSet.AttachControl(new Button { size = new Size(8, 1), Text = "OK" });

        Assert.AreEqual(0, console.ClearCount);
    }

    [TestMethod]
    public void InitialCanonicalFrameUsesBatchedRowWrites()
    {
        var console = new RecordingConsole(10, 4);
        var widgetSet = new ConsoleWidgetSet(console, new RecordingExtendedConsole());

        widgetSet.AttachControl(new Button { size = new Size(8, 3), Text = "OK" });

        Assert.IsTrue(console.StringWriteCount < console.Writes.Count);
        Assert.IsTrue(console.StringWriteCount <= 4);
        Assert.IsTrue(console.StringWriteRows.Distinct().Count() <= 3);
        Assert.AreEqual('O', console[3, 1]);
        Assert.AreEqual('K', console[4, 1]);
        Assert.AreEqual(' ', console[0, 0]);
        Assert.AreEqual(' ', console[7, 2]);
    }

    [TestMethod]
    public void ReattachingAfterDirectHostOverwriteRestoresCanonicalFrame()
    {
        var console = new RecordingConsole(10, 4);
        var widgetSet = new ConsoleWidgetSet(console, new RecordingExtendedConsole());
        var button = new Button { size = new Size(8, 1), Text = "OK" };

        widgetSet.AttachControl(button);
        console.SetCursorPosition(3, 0);
        console.Write('X');
        Assert.AreEqual('X', console[3, 0]);

        widgetSet.DetachControl(button);
        widgetSet.AttachControl(button);

        Assert.AreEqual('O', console[3, 0]);
        Assert.AreEqual('K', console[4, 0]);
    }

    [TestMethod]
    public void ReattachingPreviousRootAfterTransientDialogRootFlushesCompleteFrame()
    {
        var console = new RecordingConsole(20, 8);
        var widgetSet = new ConsoleWidgetSet(console, new RecordingExtendedConsole());
        var host = new Panel { size = new Size(20, 8) };
        host.Add(new Label { Text = "Desktop", size = new Size(10, 1) });
        var dialog = new Dialog
        {
            Text = "About",
            BorderStyle = BorderStyle.Double,
            size = new Size(10, 5),
            Position = new Point(5, 1)
        };
        dialog.Show();

        widgetSet.AttachControl(host);
        widgetSet.AttachControl(dialog);
        widgetSet.AttachControl(host);

        Assert.IsTrue(console.Writes.Any(write => write.Character == 'D'));
        Assert.IsTrue(console.Writes.Any(write => write.Character == '╔'));
    }

    private static void ResetCanvas()
    {
        var canvasField = typeof(ConsoleFramework).GetField("_canvas", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(canvasField);
        canvasField!.SetValue(null, null);
    }

    private sealed class RecordingExtendedConsole : IExtendedConsole
    {
        public event EventHandler<IMouseEvent>? MouseEvent;
        public event EventHandler<IKeyEvent>? KeyEvent;
        public event EventHandler<Point>? WindowBufferSizeEvent;

        public void Stop() { }

        public void RaiseResize(Point size) => WindowBufferSizeEvent?.Invoke(this, size);
    }

    private sealed class RecordingConsole : IConsole
    {
        private readonly Dictionary<Point, char> _cells = new();
        private Point _cursor;

        public RecordingConsole(int width, int height)
        {
            WindowWidth = width;
            WindowHeight = height;
        }

        public List<WriteRecord> Writes { get; } = new();
        public List<int> StringWriteRows { get; } = new();
        public int StringWriteCount { get; private set; }
        public int ClearCount { get; private set; }
        public char this[int x, int y] => _cells.TryGetValue(new Point(x, y), out var value) ? value : '\0';
        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Gray;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
        public bool IsOutputRedirected => false;
        public bool KeyAvailable => false;
        public int LargestWindowHeight => WindowHeight;
        public int LargestWindowWidth => WindowWidth;
        public string Title { get; set; } = string.Empty;
        public int WindowHeight { get; set; }
        public int WindowWidth { get; set; }
        public int WindowLeft { get; set; }
        public int WindowTop { get; set; }
        public bool CursorVisible { get; set; } = true;
        public int BufferWidth => WindowWidth;
        public int BufferHeight => WindowHeight;

        public void Beep(int freq, int len) { }
        public void Clear() { ClearCount++; _cells.Clear(); }
        public (int Left, int Top) GetCursorPosition() => (_cursor.X, _cursor.Y);
        public ConsoleKeyInfo? ReadKey() => null;
        public string ReadLine() => string.Empty;
        public void ResetColor() { ForegroundColor = ConsoleColor.Gray; BackgroundColor = ConsoleColor.Black; }
        public void SetCursorPosition(int left, int top) => _cursor = new Point(left, top);
        public void SetWindowPosition(int left, int top) { WindowLeft = left; WindowTop = top; }
        public void SetWindowSize(int width, int height) { WindowWidth = width; WindowHeight = height; }

        public void Write(char ch)
        {
            _cells[_cursor] = ch;
            Writes.Add(new WriteRecord(_cursor, ch));
            _cursor.X++;
        }

        public void Write(string? value)
        {
            StringWriteCount++;
            StringWriteRows.Add(_cursor.Y);
            foreach (var ch in value ?? string.Empty)
                Write(ch);
        }

        public void WriteLine(string? value = "") => Write(value + Environment.NewLine);
    }

    private readonly record struct WriteRecord(Point Position, char Character);
}
