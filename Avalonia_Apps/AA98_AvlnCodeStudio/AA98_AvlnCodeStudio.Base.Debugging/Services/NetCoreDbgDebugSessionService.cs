using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Debugging.Models;

namespace AA98_AvlnCodeStudio.Base.Debugging.Services;

/// <summary>
/// Runs .NET debug sessions through the cross-platform NetCoreDbg Debug Adapter Protocol implementation.
/// </summary>
public sealed class NetCoreDbgDebugSessionService : IDebugSessionService
{
    private const int MaximumMessageLength = 16 * 1024 * 1024;
    private readonly ConcurrentDictionary<string, DebugSession> _sessions = new(StringComparer.Ordinal);
    private readonly string _adapterPath;

    /// <inheritdoc/>
    public event EventHandler<DebugSessionEventArgs>? SessionEventReceived;

    public NetCoreDbgDebugSessionService(string? adapterPath = null)
    {
        string? configuredPath = string.IsNullOrWhiteSpace(adapterPath)
            ? Environment.GetEnvironmentVariable("AVLN_NETCOREDBG_PATH")
            : adapterPath;
        _adapterPath = string.IsNullOrWhiteSpace(configuredPath) ? "netcoredbg" : configuredPath;
    }

    /// <inheritdoc/>
    public async Task<DebugSessionInfo> StartSessionAsync(
        DebugLaunchRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        if (request.AttachToExistingProcess)
        {
            throw new NotSupportedException("Attaching to an existing process is not supported by this launch request.");
        }

        if (string.IsNullOrWhiteSpace(request.Target))
        {
            throw new ArgumentException("A .NET application DLL is required as the debug target.", nameof(request));
        }

        string targetPath = Path.GetFullPath(request.Target);
        if (!File.Exists(targetPath))
        {
            throw new FileNotFoundException("The .NET debug target does not exist.", targetPath);
        }

        string workingDirectory = string.IsNullOrWhiteSpace(request.WorkingDirectory)
            ? Path.GetDirectoryName(targetPath)!
            : Path.GetFullPath(request.WorkingDirectory);
        if (!Directory.Exists(workingDirectory))
        {
            throw new DirectoryNotFoundException($"The debug working directory '{workingDirectory}' does not exist.");
        }

        ProcessStartInfo startInfo = new(_adapterPath)
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        startInfo.ArgumentList.Add("--interpreter=vscode");

        Process process = new() { StartInfo = startInfo };
        try
        {
            if (!process.Start())
            {
                throw new InvalidOperationException("The NetCoreDbg adapter process could not be started.");
            }
        }
        catch (Win32Exception exception)
        {
            process.Dispose();
            throw new InvalidOperationException(
                "Could not start NetCoreDbg. Install it and add it to PATH, or set AVLN_NETCOREDBG_PATH to its executable.",
                exception);
        }

        string sessionId = Guid.NewGuid().ToString("N");
        DapConnection connection = new(process.StandardInput.BaseStream, process.StandardOutput.BaseStream);
        DebugSession session = new(sessionId, process, connection);
        connection.EventReceived += message => OnAdapterEvent(session, message);
        Task<string> standardErrorTask = process.StandardError.ReadToEndAsync();
        try
        {
            await connection.SendRequestAsync("initialize", new
            {
                clientID = "avlncodestudio",
                clientName = "AvlnCodeStudio",
                adapterID = "coreclr",
                locale = CultureInfo.CurrentUICulture.Name,
                linesStartAt1 = true,
                columnsStartAt1 = true,
                pathFormat = "path",
                supportsRunInTerminalRequest = false,
                supportsConfigurationDoneRequest = true,
            }, cancellationToken).ConfigureAwait(false);

            await connection.SendRequestAsync("launch", new
            {
                type = "coreclr",
                request = "launch",
                name = Path.GetFileNameWithoutExtension(targetPath),
                program = targetPath,
                cwd = workingDirectory,
                args = request.Arguments.ToArray(),
                stopAtEntry = request.StopAtEntry,
                noDebug = false,
                console = "internalConsole",
            }, cancellationToken).ConfigureAwait(false);

            await connection.SendRequestAsync("configurationDone", new { }, cancellationToken).ConfigureAwait(false);
            if (request.StopAtEntry)
            {
                await session.InitialStop.Task
                    .WaitAsync(TimeSpan.FromSeconds(10), cancellationToken)
                    .ConfigureAwait(false);
            }

            if (session.State == DebugSessionState.Starting)
            {
                session.State = request.StopAtEntry ? DebugSessionState.Paused : DebugSessionState.Running;
            }

            if (!_sessions.TryAdd(sessionId, session))
            {
                throw new InvalidOperationException("A unique debugger session identifier could not be allocated.");
            }

            return new DebugSessionInfo
            {
                SessionId = sessionId,
                DisplayName = Path.GetFileName(targetPath),
                State = session.State,
            };
        }
        catch (Exception exception)
        {
            connection.Dispose();
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }

            await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
            process.Dispose();
            string standardError = (await standardErrorTask.ConfigureAwait(false)).Trim();
            if (!string.IsNullOrWhiteSpace(standardError))
            {
                throw new InvalidOperationException(
                    $"{exception.Message}{Environment.NewLine}{standardError}",
                    exception);
            }

            throw;
        }
    }

    /// <inheritdoc/>
    public async Task StopSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw new ArgumentException("A debugger session identifier is required.", nameof(sessionId));
        }

        if (!_sessions.TryRemove(sessionId, out DebugSession? session))
        {
            throw new KeyNotFoundException($"Debugger session '{sessionId}' was not found.");
        }

        try
        {
            if (!session.Process.HasExited)
            {
                await session.Connection.SendRequestAsync("disconnect", new
                {
                    terminateDebuggee = true,
                    restart = false,
                }, cancellationToken).ConfigureAwait(false);

                try
                {
                    await session.Process.WaitForExitAsync(cancellationToken)
                        .WaitAsync(TimeSpan.FromSeconds(2), cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (TimeoutException)
                {
                    session.Process.Kill(entireProcessTree: true);
                    await session.Process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            if (!session.Process.HasExited)
            {
                session.Process.Kill(entireProcessTree: true);
                await session.Process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
            }

            session.Connection.Dispose();
            session.Process.Dispose();
        }
    }

    /// <inheritdoc/>
    public async Task SetBreakpointsAsync(
        string sessionId,
        string sourcePath,
        IReadOnlyList<int> lines,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePath);
        ArgumentNullException.ThrowIfNull(lines);
        if (lines.Any(static line => line < 1))
        {
            throw new ArgumentOutOfRangeException(nameof(lines), "Breakpoint lines must be one-based positive numbers.");
        }

        string fullSourcePath = Path.GetFullPath(sourcePath);
        int[] distinctLines = lines.Distinct().Order().ToArray();
        DebugSession session = GetSession(sessionId);
        await session.Connection.SendRequestAsync("setBreakpoints", new
        {
            source = new { path = fullSourcePath },
            breakpoints = distinctLines.Select(static line => new { line }).ToArray(),
            sourceModified = false,
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task ContinueSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        DebugSession session = GetSession(sessionId);
        int threadId = await GetThreadIdAsync(session.Connection, cancellationToken).ConfigureAwait(false);
        await session.Connection.SendRequestAsync("continue", new
        {
            threadId,
            singleThread = false,
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task PauseSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        DebugSession session = GetSession(sessionId);
        int threadId = await GetThreadIdAsync(session.Connection, cancellationToken).ConfigureAwait(false);
        await session.Connection.SendRequestAsync("pause", new { threadId }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task StepSessionAsync(
        string sessionId,
        DebugStepKind stepKind,
        CancellationToken cancellationToken = default)
    {
        string command = stepKind switch
        {
            DebugStepKind.Into => "stepIn",
            DebugStepKind.Over => "next",
            DebugStepKind.Out => "stepOut",
            _ => throw new ArgumentOutOfRangeException(nameof(stepKind), stepKind, "Unknown debugger step operation."),
        };

        DebugSession session = GetSession(sessionId);
        int threadId = await GetThreadIdAsync(session.Connection, cancellationToken).ConfigureAwait(false);
        await session.Connection.SendRequestAsync(command, new
        {
            threadId,
            singleThread = false,
            granularity = "line",
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<DebugStackFrame>> GetStackTraceAsync(
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        DebugSession session = GetSession(sessionId);
        int threadId = await GetThreadIdAsync(session.Connection, cancellationToken).ConfigureAwait(false);
        JsonElement body = await session.Connection.SendRequestAsync("stackTrace", new
        {
            threadId,
            startFrame = 0,
            levels = 100,
        }, cancellationToken).ConfigureAwait(false);

        if (!body.TryGetProperty("stackFrames", out JsonElement stackFrames)
            || stackFrames.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        List<DebugStackFrame> result = new(stackFrames.GetArrayLength());
        foreach (JsonElement frame in stackFrames.EnumerateArray())
        {
            if (!frame.TryGetProperty("id", out JsonElement id)
                || !id.TryGetInt32(out int frameId)
                || !frame.TryGetProperty("name", out JsonElement name))
            {
                throw new InvalidDataException("The debugger adapter returned an invalid stack frame.");
            }

            string? sourcePath = frame.TryGetProperty("source", out JsonElement source)
                && source.TryGetProperty("path", out JsonElement path)
                ? path.GetString()
                : null;
            result.Add(new DebugStackFrame
            {
                Id = frameId,
                Name = name.GetString() ?? string.Empty,
                SourcePath = sourcePath,
                Line = ReadOptionalInt32(frame, "line"),
                Column = ReadOptionalInt32(frame, "column"),
            });
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<DebugVariable>> GetVariablesAsync(
        string sessionId,
        int frameId,
        CancellationToken cancellationToken = default)
    {
        if (frameId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(frameId), "A stack frame identifier cannot be negative.");
        }

        DebugSession session = GetSession(sessionId);
        JsonElement scopesBody = await session.Connection.SendRequestAsync("scopes", new { frameId }, cancellationToken)
            .ConfigureAwait(false);
        if (!scopesBody.TryGetProperty("scopes", out JsonElement scopes)
            || scopes.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        List<DebugVariable> result = [];
        foreach (JsonElement scope in scopes.EnumerateArray())
        {
            string scopeName = scope.TryGetProperty("name", out JsonElement scopeNameElement)
                ? scopeNameElement.GetString() ?? string.Empty
                : string.Empty;
            if (!scope.TryGetProperty("variablesReference", out JsonElement variablesReference)
                || !variablesReference.TryGetInt32(out int reference)
                || reference <= 0)
            {
                continue;
            }

            JsonElement variablesBody = await session.Connection.SendRequestAsync("variables", new
            {
                variablesReference = reference,
            }, cancellationToken).ConfigureAwait(false);
            if (!variablesBody.TryGetProperty("variables", out JsonElement variables)
                || variables.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (JsonElement variable in variables.EnumerateArray())
            {
                if (!variable.TryGetProperty("name", out JsonElement variableName)
                    || !variable.TryGetProperty("value", out JsonElement value))
                {
                    throw new InvalidDataException("The debugger adapter returned an invalid variable.");
                }

                result.Add(new DebugVariable
                {
                    Scope = scopeName,
                    Name = variableName.GetString() ?? string.Empty,
                    Value = value.GetString() ?? string.Empty,
                    Type = variable.TryGetProperty("type", out JsonElement type) ? type.GetString() : null,
                    EvaluateName = variable.TryGetProperty("evaluateName", out JsonElement evaluateName)
                        ? evaluateName.GetString()
                        : null,
                    VariablesReference = ReadOptionalInt32(variable, "variablesReference"),
                });
            }
        }

        return result;
    }

    private static int ReadOptionalInt32(JsonElement element, string propertyName)
    {
        return element.ValueKind == JsonValueKind.Object
            && element.TryGetProperty(propertyName, out JsonElement value)
            && value.TryGetInt32(out int parsedValue)
            ? parsedValue
            : 0;
    }

    private void OnAdapterEvent(DebugSession session, JsonElement message)
    {
        string eventName = message.GetProperty("event").GetString()
            ?? throw new InvalidDataException("The debugger adapter sent an event without a name.");
        JsonElement body = message.TryGetProperty("body", out JsonElement eventBody)
            ? eventBody
            : default;
        DebugSessionState? state = eventName switch
        {
            "stopped" => DebugSessionState.Paused,
            "continued" => DebugSessionState.Running,
            "terminated" or "exited" => DebugSessionState.Stopped,
            _ => null,
        };
        if (state is not null)
        {
            session.State = state.Value;
        }

        if (eventName == "stopped")
        {
            session.InitialStop.TrySetResult();
        }

        SessionEventReceived?.Invoke(this, new DebugSessionEventArgs
        {
            SessionId = session.SessionId,
            EventName = eventName,
            State = state,
            Reason = ReadOptionalString(body, "reason"),
            Description = ReadOptionalString(body, "description"),
            ThreadId = ReadOptionalInt32(body, "threadId") is int threadId && threadId > 0
                ? threadId
                : null,
            Output = ReadOptionalString(body, "output"),
        });
    }

    private static string? ReadOptionalString(JsonElement element, string propertyName)
    {
        return element.ValueKind == JsonValueKind.Object
            && element.TryGetProperty(propertyName, out JsonElement value)
                ? value.GetString()
                : null;
    }

    private DebugSession GetSession(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw new ArgumentException("A debugger session identifier is required.", nameof(sessionId));
        }

        if (!_sessions.TryGetValue(sessionId, out DebugSession? session))
        {
            throw new KeyNotFoundException($"Debugger session '{sessionId}' was not found.");
        }

        if (session.Process.HasExited)
        {
            throw new InvalidOperationException($"Debugger session '{sessionId}' has already exited.");
        }

        return session;
    }

    private static async Task<int> GetThreadIdAsync(DapConnection connection, CancellationToken cancellationToken)
    {
        JsonElement body = await connection.SendRequestAsync("threads", new { }, cancellationToken)
            .ConfigureAwait(false);
        if (!body.TryGetProperty("threads", out JsonElement threads)
            || threads.ValueKind != JsonValueKind.Array
            || threads.GetArrayLength() == 0
            || !threads[0].TryGetProperty("id", out JsonElement threadId)
            || !threadId.TryGetInt32(out int parsedThreadId))
        {
            throw new InvalidOperationException("The debugger adapter did not report an active thread.");
        }

        return parsedThreadId;
    }

    private sealed class DebugSession(string sessionId, Process process, DapConnection connection)
    {
        public string SessionId { get; } = sessionId;

        public Process Process { get; } = process;

        public DapConnection Connection { get; } = connection;

        public TaskCompletionSource InitialStop { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public DebugSessionState State { get; set; } = DebugSessionState.Starting;
    }

    private sealed class DapConnection : IDisposable
    {
        private static readonly JsonSerializerOptions s_jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private readonly Stream _input;
        private readonly Stream _output;
        private readonly SemaphoreSlim _requestLock = new(1, 1);
        private readonly ConcurrentDictionary<int, TaskCompletionSource<JsonElement>> _pendingResponses = new();
        private readonly CancellationTokenSource _shutdown = new();
        private int _sequence;
        private Exception? _readFailure;

        public DapConnection(Stream input, Stream output)
        {
            _input = input;
            _output = output;
            _ = ReadMessagesAsync();
        }

        public event Action<JsonElement>? EventReceived;

        public async Task<JsonElement> SendRequestAsync(
            string command,
            object arguments,
            CancellationToken cancellationToken)
        {
            await _requestLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            Exception? readFailure = Volatile.Read(ref _readFailure);
            if (readFailure is not null)
            {
                _requestLock.Release();
                throw new InvalidOperationException("The debugger adapter connection has failed.", readFailure);
            }

            int sequence = Interlocked.Increment(ref _sequence);
            TaskCompletionSource<JsonElement> response = new(TaskCreationOptions.RunContinuationsAsynchronously);
            if (!_pendingResponses.TryAdd(sequence, response))
            {
                _requestLock.Release();
                throw new InvalidOperationException("A unique DAP request identifier could not be allocated.");
            }

            try
            {
                byte[] content = JsonSerializer.SerializeToUtf8Bytes(new
                {
                    seq = sequence,
                    type = "request",
                    command,
                    arguments,
                }, s_jsonOptions);
                byte[] header = Encoding.ASCII.GetBytes($"Content-Length: {content.Length}\r\n\r\n");
                await _input.WriteAsync(header, cancellationToken).ConfigureAwait(false);
                await _input.WriteAsync(content, cancellationToken).ConfigureAwait(false);
                await _input.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                _pendingResponses.TryRemove(sequence, out _);
                throw;
            }
            finally
            {
                _requestLock.Release();
            }

            try
            {
                JsonElement root = await response.Task
                    .WaitAsync(TimeSpan.FromSeconds(10), cancellationToken)
                    .ConfigureAwait(false);
                if (!root.GetProperty("success").GetBoolean())
                {
                    string error = root.TryGetProperty("message", out JsonElement messageElement)
                        ? messageElement.GetString() ?? "The debugger adapter rejected the request."
                        : "The debugger adapter rejected the request.";
                    throw new InvalidOperationException($"NetCoreDbg {command} request failed: {error}");
                }

                return root.TryGetProperty("body", out JsonElement body) ? body : default;
            }
            finally
            {
                _pendingResponses.TryRemove(sequence, out _);
            }
        }

        private async Task ReadMessagesAsync()
        {
            try
            {
                while (!_shutdown.IsCancellationRequested)
                {
                    using JsonDocument message = await ReadMessageAsync(_shutdown.Token).ConfigureAwait(false);
                    JsonElement root = message.RootElement;
                    string? type = ReadOptionalString(root, "type");
                    if (string.Equals(type, "response", StringComparison.Ordinal))
                    {
                        if (root.TryGetProperty("request_seq", out JsonElement requestSequence)
                            && requestSequence.TryGetInt32(out int sequence)
                            && _pendingResponses.TryGetValue(sequence, out TaskCompletionSource<JsonElement>? response))
                        {
                            response.TrySetResult(root.Clone());
                        }
                    }
                    else if (string.Equals(type, "event", StringComparison.Ordinal))
                    {
                        EventReceived?.Invoke(root.Clone());
                    }
                    else
                    {
                        throw new InvalidDataException($"The debugger adapter sent an unsupported DAP message type '{type}'.");
                    }
                }
            }
            catch (OperationCanceledException) when (_shutdown.IsCancellationRequested)
            {
            }
            catch (Exception exception)
            {
                Volatile.Write(ref _readFailure, exception);
                foreach (TaskCompletionSource<JsonElement> response in _pendingResponses.Values)
                {
                    response.TrySetException(exception);
                }
            }
        }

        private async Task<JsonDocument> ReadMessageAsync(CancellationToken cancellationToken)
        {
            int? contentLength = null;
            int headerLength = 0;
            while (true)
            {
                string line = await ReadLineAsync(cancellationToken).ConfigureAwait(false);
                headerLength += line.Length + 2;
                if (headerLength > 8192)
                {
                    throw new InvalidDataException("The debugger adapter sent an oversized DAP header.");
                }

                if (line.Length == 0)
                {
                    break;
                }

                const string contentLengthPrefix = "Content-Length:";
                if (line.StartsWith(contentLengthPrefix, StringComparison.OrdinalIgnoreCase)
                    && int.TryParse(line.AsSpan(contentLengthPrefix.Length).Trim(), out int parsedLength))
                {
                    contentLength = parsedLength;
                }
            }

            if (contentLength is null or < 0 or > MaximumMessageLength)
            {
                throw new InvalidDataException("The debugger adapter sent an invalid DAP content length.");
            }

            byte[] content = new byte[contentLength.Value];
            await _output.ReadExactlyAsync(content, cancellationToken).ConfigureAwait(false);
            return JsonDocument.Parse(content);
        }

        private async Task<string> ReadLineAsync(CancellationToken cancellationToken)
        {
            using MemoryStream line = new();
            byte[] buffer = new byte[1];
            while (true)
            {
                int bytesRead = await _output.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
                if (bytesRead == 0)
                {
                    throw new EndOfStreamException("The debugger adapter closed its output stream.");
                }

                if (buffer[0] == (byte)'\n')
                {
                    byte[] bytes = line.ToArray();
                    int length = bytes.Length > 0 && bytes[^1] == (byte)'\r' ? bytes.Length - 1 : bytes.Length;
                    return Encoding.ASCII.GetString(bytes, 0, length);
                }

                line.WriteByte(buffer[0]);
                if (line.Length > 8192)
                {
                    throw new InvalidDataException("The debugger adapter sent an oversized DAP header line.");
                }
            }
        }

        public void Dispose()
        {
            _shutdown.Cancel();
            _input.Dispose();
            _output.Dispose();
            _requestLock.Dispose();
            _shutdown.Dispose();
        }
    }
}
