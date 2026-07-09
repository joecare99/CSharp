using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.Terminal.Host.Services;

/// <summary>
/// Reads a text stream and emits complete lines immediately plus partial chunks after a short inactivity timeout.
/// </summary>
public sealed class OutputChunkReader
{
    private const int ReadBufferSize = 256;
    private readonly TextReader _reader;
    private readonly TimeSpan _flushDelay;
    private readonly Action<string> _lineCallback;
    private readonly Action<string> _partialCallback;
    private readonly StringBuilder _buffer = new();
#if NET9_0_OR_GREATER
    private readonly Lock _syncRoot = new();
#else
    private readonly object _syncRoot = new();
#endif
    private CancellationTokenSource? _flushCancellationTokenSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutputChunkReader"/> class.
    /// </summary>
    /// <param name="reader">The text reader to consume.</param>
    /// <param name="flushDelay">The inactivity delay before partial output is flushed.</param>
    /// <param name="lineCallback">The callback for complete lines.</param>
    /// <param name="partialCallback">The callback for partial chunks.</param>
    public OutputChunkReader(TextReader reader, TimeSpan flushDelay, Action<string> lineCallback, Action<string> partialCallback)
    {
        _reader = reader;
        _flushDelay = flushDelay;
        _lineCallback = lineCallback;
        _partialCallback = partialCallback;
    }

    /// <summary>
    /// Runs the reader loop until the stream ends or cancellation is requested.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var readBuffer = new char[ReadBufferSize];
        while (!cancellationToken.IsCancellationRequested)
        {
            var charactersRead = await _reader.ReadAsync(readBuffer.AsMemory(0, readBuffer.Length), cancellationToken).ConfigureAwait(false);
            if (charactersRead == 0)
            {
                FlushPartialBuffer();
                return;
            }

            ProcessBuffer(readBuffer.AsSpan(0, charactersRead), cancellationToken);
        }
    }

    private void ProcessBuffer(ReadOnlySpan<char> buffer, CancellationToken cancellationToken)
    {
        var appendedPartial = false;
        foreach (var character in buffer)
        {
            if (character == '\r')
            {
                continue;
            }

            if (character == '\n')
            {
                CancelPendingFlush();
                var line = ExtractBufferedText();
                _lineCallback(line);
                appendedPartial = false;
                continue;
            }

            lock (_syncRoot)
            {
                _buffer.Append(character);
            }

            appendedPartial = true;
        }

        if (appendedPartial)
        {
            ScheduleFlush(cancellationToken);
        }
    }

    private void ScheduleFlush(CancellationToken cancellationToken)
    {
        CancelPendingFlush();

        CancellationTokenSource flushCancellationTokenSource;
        lock (_syncRoot)
        {
            _flushCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            flushCancellationTokenSource = _flushCancellationTokenSource;
        }

        _ = WaitForFlushAsync(flushCancellationTokenSource);
    }

    private async Task WaitForFlushAsync(CancellationTokenSource flushCancellationTokenSource)
    {
        try
        {
            await Task.Delay(_flushDelay, flushCancellationTokenSource.Token).ConfigureAwait(false);
            FlushPartialBuffer();
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            flushCancellationTokenSource.Dispose();
            lock (_syncRoot)
            {
                if (ReferenceEquals(_flushCancellationTokenSource, flushCancellationTokenSource))
                {
                    _flushCancellationTokenSource = null;
                }
            }
        }
    }

    private void CancelPendingFlush()
    {
        CancellationTokenSource? flushCancellationTokenSource;
        lock (_syncRoot)
        {
            flushCancellationTokenSource = _flushCancellationTokenSource;
            _flushCancellationTokenSource = null;
        }

        flushCancellationTokenSource?.Cancel();
    }

    private void FlushPartialBuffer()
    {
        CancelPendingFlush();
        var partial = ExtractBufferedText();
        if (partial.Length > 0)
        {
            _partialCallback(partial);
        }
    }

    private string ExtractBufferedText()
    {
        lock (_syncRoot)
        {
            if (_buffer.Length == 0)
            {
                return string.Empty;
            }

            var result = _buffer.ToString();
            _buffer.Clear();
            return result;
        }
    }
}