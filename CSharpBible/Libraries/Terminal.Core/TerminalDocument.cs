using System;

namespace Terminal.Core;

/// <summary>
/// Combines a terminal buffer and ANSI parser into a change-notifying document model.
/// </summary>
public sealed class TerminalDocument
{
    private readonly TerminalAnsiParser _parser;
    private TerminalMouseProtocol _mouseProtocol;
    private TerminalMouseTrackingMode _mouseTrackingMode;
    private string _title = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalDocument"/> class.
    /// </summary>
    public TerminalDocument(TerminalSize size, int scrollbackLimit = 1000)
    {
        Buffer = new TerminalBuffer(size, scrollbackLimit);
        _parser = new TerminalAnsiParser(Buffer, HandleWindowTitleChanged, HandleMouseTrackingChanged);
    }

    /// <summary>
    /// Occurs when the document content changed.
    /// </summary>
    public event EventHandler? Changed;

    /// <summary>
    /// Gets the underlying terminal buffer.
    /// </summary>
    public ITerminalBuffer Buffer { get; }

    /// <summary>
    /// Gets the current mouse reporting protocol reported by the session.
    /// </summary>
    public TerminalMouseProtocol MouseProtocol => _mouseProtocol;

    /// <summary>
    /// Gets the current mouse tracking mode reported by the session.
    /// </summary>
    public TerminalMouseTrackingMode MouseTrackingMode => _mouseTrackingMode;

    /// <summary>
    /// Gets the current terminal window title reported by the session.
    /// </summary>
    public string Title => _title;

    /// <summary>
    /// Parses terminal output and updates the document.
    /// </summary>
    public void ApplyOutput(string? text)
    {
        _parser.Parse(text);
        OnChanged();
    }

    /// <summary>
    /// Resizes the document viewport.
    /// </summary>
    public void Resize(TerminalSize size)
    {
        Buffer.Resize(size);
        OnChanged();
    }

    /// <summary>
    /// Creates a snapshot of the visible viewport.
    /// </summary>
    public TerminalSnapshot CreateSnapshot() => Buffer.CreateSnapshot();

    private void OnChanged()
    {
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private void HandleWindowTitleChanged(string title)
    {
        _title = title;
    }

    private void HandleMouseTrackingChanged(TerminalMouseTrackingMode mode, TerminalMouseProtocol protocol)
    {
        _mouseTrackingMode = mode;
        _mouseProtocol = protocol;
    }
}
