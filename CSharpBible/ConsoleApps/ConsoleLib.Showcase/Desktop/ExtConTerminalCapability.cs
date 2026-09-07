using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLib.Showcase.Desktop.Capabilities;
using ConsoleLib.Showcase.Services;
using Terminal.Core;

namespace ConsoleLib.Showcase.DesktopHost;

/// <summary>Adapts the existing ConPTY-backed showcase service to the portable contract.</summary>
public sealed class ExtConTerminalCapability : IShowcaseTerminalCapability
{
    private readonly IShowcaseTerminalService _service;

    public ExtConTerminalCapability(IShowcaseTerminalService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _service.SnapshotChanged += Service_SnapshotChanged;
    }

    public bool IsAvailable => true;

    public bool IsRunning => _service.IsRunning;

    public event EventHandler<string>? OutputChanged;

    public Task StartAsync(Size size, CancellationToken cancellationToken = default) =>
        _service.StartAsync(new TerminalSize(Math.Max(1, size.Width), Math.Max(1, size.Height)), cancellationToken);

    public Task SendInputAsync(string input, CancellationToken cancellationToken = default) =>
        _service.SendInputAsync(input, cancellationToken);

    public Task ResizeAsync(Size size, CancellationToken cancellationToken = default) =>
        _service.ResizeAsync(new TerminalSize(Math.Max(1, size.Width), Math.Max(1, size.Height)), cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken = default) =>
        _service.StopAsync(cancellationToken);

    public async ValueTask DisposeAsync()
    {
        _service.SnapshotChanged -= Service_SnapshotChanged;
        await _service.StopAsync().ConfigureAwait(false);
    }

    private void Service_SnapshotChanged(object? sender, TerminalSnapshot snapshot)
    {
        var text = string.Join(
            Environment.NewLine,
            snapshot.Lines.Select(line => new string(line.Select(cell => cell.Character).ToArray())));
        OutputChanged?.Invoke(this, text);
    }
}
