using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLib.Interfaces;
using ConsoleLib.Posix;
using ConsoleLib.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Showcase.Desktop.Tests;

[TestClass]
public sealed class PosixFrameOutputTests
{
    [TestMethod]
    public async Task DesktopPageSnapshotIsConsumedByAnsiFrameRenderer()
    {
        var viewModel = new DesktopViewModel { Status = "Posix frame ready" };
        var page = new DesktopPage().Load(viewModel);
        using var service = new AttachedRenderService();
        service.Attach(page.Root, new Size(80, 25));
        var output = new RecordingAnsiOutput();

        await new AnsiFrameRenderer(output).RenderAsync(service.GetSnapshot());

        StringAssert.Contains(output.Text, "Posix frame ready");
        StringAssert.EndsWith(output.Text, "\u001b[0m");
    }

    private sealed class RecordingAnsiOutput : IAnsiOutput
    {
        private readonly StringBuilder _text = new();

        public string Text => _text.ToString();

        public Task WriteAsync(string text, CancellationToken cancellationToken = default)
        {
            _text.Append(text);
            return Task.CompletedTask;
        }

        public Task ClearAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task MoveCursorAsync(int column, int row, CancellationToken cancellationToken = default) =>
            WriteAsync($"\u001b[{row};{column}H", cancellationToken);

        public Task SetForegroundAsync(ConsoleColor color, CancellationToken cancellationToken = default) =>
            WriteAsync($"<fg:{color}>", cancellationToken);

        public Task SetBackgroundAsync(ConsoleColor color, CancellationToken cancellationToken = default) =>
            WriteAsync($"<bg:{color}>", cancellationToken);

        public Task ResetAsync(CancellationToken cancellationToken = default) =>
            WriteAsync("\u001b[0m", cancellationToken);

        public Task EnableMouseTrackingAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DisableMouseTrackingAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
