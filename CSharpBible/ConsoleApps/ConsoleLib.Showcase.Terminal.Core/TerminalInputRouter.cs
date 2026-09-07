using System;
using System.Threading;
using System.Threading.Tasks;
using Terminal.Core;

namespace ConsoleLib.Showcase.Terminal.Core;

/// <summary>Maps native console key information to terminal input sequences.</summary>
public sealed class TerminalInputRouter
{
    public string Encode(ushort virtualKeyCode, char keyChar, uint controlKeyState, bool keyDown)
    {
        if (!keyDown)
            return string.Empty;

        var consoleKey = (ConsoleKey)virtualKeyCode;
        return Encode(new ConsoleKeyInfo(
            keyChar,
            consoleKey,
            (controlKeyState & 0x10) != 0,
            (controlKeyState & (0x02 | 0x01)) != 0,
            (controlKeyState & (0x08 | 0x04)) != 0));
    }

    public string Encode(ConsoleKeyInfo key) => key.Key switch
    {
        ConsoleKey.Enter => TerminalInputEncoder.EncodeEnter(),
        ConsoleKey.Backspace => TerminalInputEncoder.EncodeBackspace(),
        ConsoleKey.Tab => "\t",
        ConsoleKey.Escape => "\u001b",
        ConsoleKey.Delete => "\u001b[3~",
        ConsoleKey.Home => "\u001b[H",
        ConsoleKey.End => "\u001b[F",
        ConsoleKey.PageUp => "\u001b[5~",
        ConsoleKey.PageDown => "\u001b[6~",
        ConsoleKey.UpArrow => TerminalInputEncoder.EncodeArrowUp(),
        ConsoleKey.DownArrow => TerminalInputEncoder.EncodeArrowDown(),
        ConsoleKey.LeftArrow => TerminalInputEncoder.EncodeArrowLeft(),
        ConsoleKey.RightArrow => TerminalInputEncoder.EncodeArrowRight(),
        _ => TerminalInputEncoder.EncodeText(key.KeyChar == '\0' ? string.Empty : key.KeyChar.ToString())
    };

    public Task RouteAsync(ITerminalSession session, ConsoleKeyInfo key, CancellationToken cancellationToken = default)
    {
        if (session is null)
            throw new ArgumentNullException(nameof(session));
        return session.WriteAsync(Encode(key), cancellationToken);
    }
}
