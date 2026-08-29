using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Posix;

/// <summary>Renders a deterministic ConsoleLib frame through ANSI output.</summary>
public sealed class AnsiFrameRenderer
{
    private readonly IAnsiOutput _output;

    public AnsiFrameRenderer(IAnsiOutput output) =>
        _output = output ?? throw new ArgumentNullException(nameof(output));

    public async Task RenderAsync(IRenderSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        if (snapshot is null)
            throw new ArgumentNullException(nameof(snapshot));

        ConsoleColor? foreground = null;
        ConsoleColor? background = null;
        for (var y = 0; y < snapshot.Size.Height; y++)
        {
            await _output.MoveCursorAsync(1, y + 1, cancellationToken).ConfigureAwait(false);
            for (var x = 0; x < snapshot.Size.Width; x++)
            {
                var cell = snapshot.GetCell(x, y);
                if (foreground != cell.Foreground)
                {
                    await _output.SetForegroundAsync(cell.Foreground, cancellationToken).ConfigureAwait(false);
                    foreground = cell.Foreground;
                }
                if (background != cell.Background)
                {
                    await _output.SetBackgroundAsync(cell.Background, cancellationToken).ConfigureAwait(false);
                    background = cell.Background;
                }
                await _output.WriteAsync(cell.Character.ToString(), cancellationToken).ConfigureAwait(false);
            }
        }
        await _output.ResetAsync(cancellationToken).ConfigureAwait(false);
    }
}
