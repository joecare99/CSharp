using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terminal.Core;

namespace ConsoleLib.Showcase.Services;

/// <summary>Owns a showcase terminal session and exposes its parsed viewport.</summary>
public sealed class ShowcaseTerminalService : IShowcaseTerminalService
{
    private readonly ITerminalSessionFactory _sessionFactory;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private ITerminalSession? _session;
    private TerminalDocument? _document;

    public ShowcaseTerminalService(ITerminalSessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory ?? throw new ArgumentNullException(nameof(sessionFactory));
    }

    public event EventHandler<TerminalSnapshot>? SnapshotChanged;

    public ITerminalSession? Session => _session;

    public TerminalDocument? Document => _document;

    public bool IsRunning => _session?.IsRunning == true;

    public async Task StartAsync(TerminalSize size, CancellationToken cancellationToken = default)
    {
        var normalizedSize = size.Normalize();
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_session?.IsRunning == true)
            {
                await ResizeCoreAsync(normalizedSize, cancellationToken).ConfigureAwait(false);
                return;
            }

            if (_session is not null)
            {
                var previousSession = _session;
                _session = null;
                _document = null;
                previousSession.OutputReceived -= HandleOutput;
                await previousSession.StopAsync(cancellationToken).ConfigureAwait(false);
                await previousSession.DisposeAsync().ConfigureAwait(false);
            }

            var session = _sessionFactory.CreateSession();
            var document = new TerminalDocument(normalizedSize);
            _session = session;
            _document = document;
            session.OutputReceived += HandleOutput;
            try
            {
                await session.StartAsync(CreateShellOptions(normalizedSize), cancellationToken).ConfigureAwait(false);
                SnapshotChanged?.Invoke(this, document.CreateSnapshot());
            }
            catch
            {
                _session = null;
                _document = null;
                session.OutputReceived -= HandleOutput;
                await session.DisposeAsync().ConfigureAwait(false);
                throw;
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task SendInputAsync(string input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(input))
            return;

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_session?.IsRunning == true)
                await _session.WriteAsync(input, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task ResizeAsync(TerminalSize size, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await ResizeCoreAsync(size.Normalize(), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_session is null)
                return;

            var session = _session;
            _session = null;
            _document = null;
            session.OutputReceived -= HandleOutput;
            await session.StopAsync(cancellationToken).ConfigureAwait(false);
            await session.DisposeAsync().ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<string> RunProbeAsync(CancellationToken cancellationToken = default)
    {
        await using var session = _sessionFactory.CreateSession();
        var output = new StringBuilder();
        var completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        session.OutputReceived += OnOutput;

        try
        {
            var options = CreateShellOptions(new TerminalSize(80, 25));
            options.Arguments = OperatingSystem.IsWindows()
                ? "/d /c echo ConsoleLib Showcase ConPTY bridge ready"
                : "-c \"printf 'ConsoleLib Showcase terminal bridge ready\\\\n'\"";
            await session.StartAsync(options, cancellationToken).ConfigureAwait(false);
            await Task.WhenAny(completed.Task, Task.Delay(TimeSpan.FromSeconds(2), cancellationToken)).ConfigureAwait(false);
            return output.Length == 0 ? "Terminal bridge started successfully." : output.ToString().Trim();
        }
        finally
        {
            session.OutputReceived -= OnOutput;
            await session.StopAsync(cancellationToken).ConfigureAwait(false);
        }

        void OnOutput(object? sender, string text)
        {
            output.Append(text);
            completed.TrySetResult();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync().ConfigureAwait(false);
        _gate.Dispose();
    }

    private async Task ResizeCoreAsync(TerminalSize size, CancellationToken cancellationToken)
    {
        _document?.Resize(size);
        if (_session?.IsRunning == true)
            await _session.ResizeAsync(size, cancellationToken).ConfigureAwait(false);
        if (_document is not null)
            SnapshotChanged?.Invoke(this, _document.CreateSnapshot());
    }

    private void HandleOutput(object? sender, string text)
    {
        var document = _document;
        if (document is null)
            return;

        document.ApplyOutput(text);
        SnapshotChanged?.Invoke(this, document.CreateSnapshot());
    }

    private static TerminalSessionOptions CreateShellOptions(TerminalSize size)
    {
        var options = TerminalShellOptions.CreateDefault();
        options.InitialSize = size;
        options.Arguments = OperatingSystem.IsWindows() ? "/d" : "-i";
        return options;
    }
}
