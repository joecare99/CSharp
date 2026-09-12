using ConsoleLib.Interfaces;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleLib;

public sealed class RedirectedConsole : IExtendedConsole, IInputEndNotification, IInitialConsoleSize, IDisposable
{
    private static readonly Regex SgrMouse = new(
        "\u001b\\[<(?<button>\\d+);(?<x>\\d+);(?<y>\\d+)(?<release>[mM])",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex Resize = new(
        "\u001b\\[8;(?<rows>\\d+);(?<columns>\\d+)t",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private readonly TextReader _input;
    private readonly CancellationTokenSource _stop = new();
    private readonly Task _readerTask;
    private readonly int _width;
    private readonly int _height;

    public RedirectedConsole(TextReader? input = null, int? width = null, int? height = null)
    {
        _input = input ?? Console.In;
        _width = width ?? ReadDimension("COLUMNS", 80);
        _height = height ?? ReadDimension("LINES", 25);
        _readerTask = Task.Run(ReadInputAsync);
    }

    public event EventHandler<IMouseEvent>? MouseEvent;
    public event EventHandler<IKeyEvent>? KeyEvent;
    public event EventHandler<Point>? WindowBufferSizeEvent;
    public event EventHandler? EndOfInput;

    public int Width => _width;
    public int Height => _height;
    public Point InitialSize => new(_width, _height);

    public void Stop()
    {
        if (!_stop.IsCancellationRequested)
            _stop.Cancel();
    }

    public void Dispose()
    {
        Stop();
        try { _readerTask.Wait(TimeSpan.FromSeconds(1)); } catch (AggregateException) { }
        _stop.Dispose();
    }

    private async Task ReadInputAsync()
    {
        var buffer = new char[256];
        var pending = new StringBuilder();
        try
        {
            while (!_stop.IsCancellationRequested)
            {
                var read = await _input.ReadAsync(buffer.AsMemory(), _stop.Token).ConfigureAwait(false);
                if (read == 0)
                {
                    EndOfInput?.Invoke(this, EventArgs.Empty);
                    return;
                }

                pending.Append(buffer, 0, read);
                ParsePending(pending);
            }
        }
        catch (OperationCanceledException) when (_stop.IsCancellationRequested) { }
    }

    private void ParsePending(StringBuilder pending)
    {
        while (pending.Length > 0)
        {
            var value = pending.ToString();
            var resize = Resize.Match(value);
            if (resize.Success && resize.Index == 0)
            {
                WindowBufferSizeEvent?.Invoke(this, new Point(
                    int.Parse(resize.Groups["columns"].Value),
                    int.Parse(resize.Groups["rows"].Value)));
                pending.Remove(0, resize.Length);
                continue;
            }

            var mouse = SgrMouse.Match(value);
            if (mouse.Success && mouse.Index == 0)
            {
                RaiseMouse(mouse);
                pending.Remove(0, mouse.Length);
                continue;
            }

            if (value[0] == '\u001b')
            {
                if (value.Length == 1)
                    return;
                if (value[1] != '[')
                {
                    RaiseKey(ConsoleKey.Escape, '\u001b', 0);
                    pending.Remove(0, 1);
                    continue;
                }

                var sequenceEnd = FindCsiEnd(value);
                if (sequenceEnd < 0)
                    return;
                var sequence = value[..(sequenceEnd + 1)];
                if (!RaiseCsiKey(sequence))
                    RaiseKey(ConsoleKey.Escape, '\u001b', 0);
                pending.Remove(0, sequence.Length);
                continue;
            }

            var character = pending[0];
            pending.Remove(0, 1);
            RaiseCharacter(character);
        }
    }

    private static int FindCsiEnd(string value)
    {
        for (var index = 2; index < value.Length; index++)
            if (value[index] is >= '@' and <= '~')
                return index;
        return -1;
    }

    private bool RaiseCsiKey(string sequence)
    {
        var key = sequence switch
        {
            "\u001b[A" => ConsoleKey.UpArrow,
            "\u001b[B" => ConsoleKey.DownArrow,
            "\u001b[C" => ConsoleKey.RightArrow,
            "\u001b[D" => ConsoleKey.LeftArrow,
            "\u001b[H" => ConsoleKey.Home,
            "\u001b[F" => ConsoleKey.End,
            "\u001b[3~" => ConsoleKey.Delete,
            "\u001b[5~" => ConsoleKey.PageUp,
            "\u001b[6~" => ConsoleKey.PageDown,
            _ => (ConsoleKey?)null
        };
        if (key is null)
            return false;
        RaiseKey(key.Value, '\0', 0);
        return true;
    }

    private void RaiseCharacter(char character)
    {
        if (character == '\r' || character == '\n')
            RaiseKey(ConsoleKey.Enter, '\r', 0);
        else if (character == '\t')
            RaiseKey(ConsoleKey.Tab, '\t', 0);
        else if (character == '\b' || character == '\u007f')
            RaiseKey(ConsoleKey.Backspace, '\b', 0);
        else if (character == '\u001b')
            RaiseKey(ConsoleKey.Escape, character, 0);
        else if (Enum.TryParse<ConsoleKey>(character.ToString(), true, out var key))
            RaiseKey(key, character, 0);
        else
            RaiseKey(ConsoleKey.NoName, character, 0);
    }

    private void RaiseKey(ConsoleKey key, char character, uint modifiers)
        => KeyEvent?.Invoke(this, new SimulatedKeyEvent(key, character, modifiers));

    private void RaiseMouse(Match match)
    {
        var button = int.Parse(match.Groups["button"].Value);
        var motion = (button & 32) != 0;
        var wheel = (button & 64) != 0 ? ((button & 1) == 0 ? 1 : -1) : 0;
        var baseButton = button & 3;
        MouseEvent?.Invoke(this, new SimulatedMouseEvent(
            new Point(int.Parse(match.Groups["x"].Value) - 1, int.Parse(match.Groups["y"].Value) - 1),
            baseButton == 0 && match.Groups["release"].Value == "M",
            baseButton == 2 && match.Groups["release"].Value == "M",
            baseButton == 1 && match.Groups["release"].Value == "M",
            wheel,
            motion,
            button < 64));
    }

    private static int ReadDimension(string name, int fallback)
        => int.TryParse(Environment.GetEnvironmentVariable(name), out var value) && value > 0
            ? value
            : fallback;

    private sealed class SimulatedKeyEvent(ConsoleKey key, char keyChar, uint modifiers) : IKeyEvent
    {
        public bool bKeyDown => true;
        public char KeyChar => keyChar;
        public ushort usKeyCode => (ushort)key;
        public ushort usScanCode => 0;
        public uint dwControlKeyState => modifiers;
        public bool Handled { get; set; }
    }

    private sealed class SimulatedMouseEvent(
        Point position,
        bool left,
        bool right,
        bool middle,
        int wheel,
        bool moved,
        bool buttonEvent) : IMouseEvent
    {
        public Point MousePos => position;
        public bool MouseButtonLeft => left;
        public bool MouseButtonRight => right;
        public bool MouseButtonMiddle => middle;
        public int MouseWheel => wheel;
        public bool MouseMoved => moved;
        public bool ButtonEvent => buttonEvent;
        public bool Handled { get; set; }
    }
}
