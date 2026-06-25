using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading;
using System.Threading.Tasks;
using Terminal.Core;

namespace Terminal.Wpf.ViewModels;

/// <summary>
/// Coordinates terminal session lifecycle and exposes the current viewport snapshot.
/// </summary>
public class TerminalControlViewModel : ObservableObject, IAsyncDisposable
{
    private readonly TerminalDocument _document;
    private readonly ITerminalSessionFactory _sessionFactory;
    private ITerminalSession? _session;
    private bool _isDisposed;
    private bool _isRunning;
    private TerminalMouseProtocol _mouseProtocol;
    private TerminalMouseTrackingMode _mouseTrackingMode;
    private string _title = string.Empty;
    private TerminalSnapshot _snapshot = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalControlViewModel"/> class.
    /// </summary>
    public TerminalControlViewModel()
        : this(TerminalBackendCatalog.CreateDefaultFactory(), new TerminalSize(80, 25))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalControlViewModel"/> class.
    /// </summary>
    /// <param name="sessionFactory">The session factory.</param>
    /// <param name="initialSize">The initial terminal size.</param>
    public TerminalControlViewModel(ITerminalSessionFactory sessionFactory, TerminalSize initialSize)
    {
        _sessionFactory = sessionFactory ?? throw new ArgumentNullException(nameof(sessionFactory));
        _document = new TerminalDocument(initialSize);
        _document.Changed += HandleDocumentChanged;
        Snapshot = _document.CreateSnapshot();
        Title = _document.Title;
        MouseProtocol = _document.MouseProtocol;
        MouseTrackingMode = _document.MouseTrackingMode;
    }

    /// <summary>
    /// Gets the document buffer size.
    /// </summary>
    public TerminalSize Size => _document.Buffer.Size;

    /// <summary>
    /// Gets a value indicating whether a session is currently running.
    /// </summary>
    public bool IsRunning
    {
        get => _isRunning;
        private set => SetProperty(ref _isRunning, value);
    }

    /// <summary>
    /// Gets the current terminal snapshot.
    /// </summary>
    public TerminalSnapshot Snapshot
    {
        get => _snapshot;
        private set => SetProperty(ref _snapshot, value);
    }

    /// <summary>
    /// Gets the current terminal window title.
    /// </summary>
    public string Title
    {
        get => _title;
        private set => SetProperty(ref _title, value);
    }

    /// <summary>
    /// Gets the current mouse reporting protocol.
    /// </summary>
    public TerminalMouseProtocol MouseProtocol
    {
        get => _mouseProtocol;
        private set => SetProperty(ref _mouseProtocol, value);
    }

    /// <summary>
    /// Gets the current mouse tracking mode.
    /// </summary>
    public TerminalMouseTrackingMode MouseTrackingMode
    {
        get => _mouseTrackingMode;
        private set => SetProperty(ref _mouseTrackingMode, value);
    }

    /// <summary>
    /// Starts the terminal session.
    /// </summary>
    /// <param name="options">The session options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task StartAsync(TerminalSessionOptions options, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        if (IsRunning)
        {
            return;
        }

        _document.Resize(options.InitialSize.Normalize());
        _session = _sessionFactory.CreateSession();
        _session.OutputReceived += HandleOutputReceived;
        await _session.StartAsync(options, cancellationToken).ConfigureAwait(false);
        IsRunning = true;
    }

    /// <summary>
    /// Stops the terminal session.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_session is null)
        {
            IsRunning = false;
            return;
        }

        _session.OutputReceived -= HandleOutputReceived;
        await _session.StopAsync(cancellationToken).ConfigureAwait(false);
        await _session.DisposeAsync().ConfigureAwait(false);
        _session = null;
        IsRunning = false;
    }

    /// <summary>
    /// Sends text input to the session.
    /// </summary>
    public Task SendTextAsync(string? text, CancellationToken cancellationToken = default)
    {
        return _session?.WriteAsync(TerminalInputEncoder.EncodeText(text), cancellationToken) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Sends Enter to the session.
    /// </summary>
    public Task SendEnterAsync(CancellationToken cancellationToken = default)
    {
        return _session?.WriteAsync(TerminalInputEncoder.EncodeEnter(), cancellationToken) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Sends Backspace to the session.
    /// </summary>
    public Task SendBackspaceAsync(CancellationToken cancellationToken = default)
    {
        return _session?.WriteAsync(TerminalInputEncoder.EncodeBackspace(), cancellationToken) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Sends an arrow-key VT sequence to the session.
    /// </summary>
    public Task SendArrowAsync(TerminalArrowKey arrowKey, CancellationToken cancellationToken = default)
    {
        var payload = arrowKey switch
        {
            TerminalArrowKey.Up => TerminalInputEncoder.EncodeArrowUp(),
            TerminalArrowKey.Down => TerminalInputEncoder.EncodeArrowDown(),
            TerminalArrowKey.Left => TerminalInputEncoder.EncodeArrowLeft(),
            TerminalArrowKey.Right => TerminalInputEncoder.EncodeArrowRight(),
            _ => string.Empty
        };

        return _session?.WriteAsync(payload, cancellationToken) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Sends mouse input to the session.
    /// </summary>
    public Task SendMouseAsync(string payload, CancellationToken cancellationToken = default)
    {
        return _session?.WriteAsync(payload, cancellationToken) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Resizes the terminal viewport.
    /// </summary>
    public async Task ResizeAsync(TerminalSize size, CancellationToken cancellationToken = default)
    {
        var normalized = size.Normalize();
        _document.Resize(normalized);
        if (_session is not null)
        {
            await _session.ResizeAsync(normalized, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _document.Changed -= HandleDocumentChanged;
        await StopAsync().ConfigureAwait(false);
    }

    private void HandleDocumentChanged(object? sender, EventArgs e)
    {
        Title = _document.Title;
        MouseProtocol = _document.MouseProtocol;
        MouseTrackingMode = _document.MouseTrackingMode;
        Snapshot = _document.CreateSnapshot();
    }

    private void HandleOutputReceived(object? sender, string output)
    {
        _document.ApplyOutput(output);
    }
}
