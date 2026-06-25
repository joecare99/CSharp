using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terminal.Core;

namespace Terminal.Backends.Windows;

/// <summary>
/// Hosts a Windows console process inside a ConPTY pseudoconsole session.
/// </summary>
public sealed class WindowsConPtyTerminalSession : ITerminalSession
{
    private SafeFileHandle? _inputWriteHandle;
    private SafeFileHandle? _outputReadHandle;
    private Stream? _inputStream;
    private StreamReader? _outputReader;
    private CancellationTokenSource? _readerCancellationTokenSource;
    private Task? _readerTask;
    private IntPtr _pseudoConsole;
    private uint _processId;

    /// <inheritdoc/>
    public event EventHandler<string>? OutputReceived;

    /// <inheritdoc/>
    public bool IsRunning { get; private set; }

    /// <inheritdoc/>
    public TerminalSize Size { get; private set; } = new(80, 25);

    /// <inheritdoc/>
    public async Task StartAsync(TerminalSessionOptions options, CancellationToken cancellationToken = default)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("ConPTY is only available on Windows.");
        }

        if (IsRunning)
        {
            throw new InvalidOperationException("The terminal session is already running.");
        }

        if (string.IsNullOrWhiteSpace(options.FileName))
        {
            throw new ArgumentException("A terminal executable is required.", nameof(options));
        }

        cancellationToken.ThrowIfCancellationRequested();
        Size = options.InitialSize.Normalize();

        IntPtr inputReadPipe = IntPtr.Zero;
        IntPtr inputWritePipe = IntPtr.Zero;
        IntPtr outputReadPipe = IntPtr.Zero;
        IntPtr outputWritePipe = IntPtr.Zero;
        IntPtr attributeList = IntPtr.Zero;
        IntPtr attributeListSize = IntPtr.Zero;
        WindowsNativeMethods.PROCESS_INFORMATION processInformation = default;

        try
        {
            CreatePipePair(out inputReadPipe, out inputWritePipe);
            CreatePipePair(out outputReadPipe, out outputWritePipe);

            var createResult = WindowsNativeMethods.CreatePseudoConsole(
                new WindowsNativeMethods.COORD((short)Size.Columns, (short)Size.Rows),
                inputReadPipe,
                outputWritePipe,
                0,
                out _pseudoConsole);

            if (createResult != 0)
            {
                Marshal.ThrowExceptionForHR(createResult);
            }

            WindowsNativeMethods.InitializeProcThreadAttributeList(IntPtr.Zero, 1, 0, ref attributeListSize);
            attributeList = Marshal.AllocHGlobal(attributeListSize);
            if (!WindowsNativeMethods.InitializeProcThreadAttributeList(attributeList, 1, 0, ref attributeListSize))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if (!WindowsNativeMethods.UpdateProcThreadAttribute(
                attributeList,
                0,
                (IntPtr)WindowsNativeMethods.PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE,
                _pseudoConsole,
                (IntPtr)IntPtr.Size,
                IntPtr.Zero,
                IntPtr.Zero))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var startupInfo = new WindowsNativeMethods.STARTUPINFOEX();
            startupInfo.StartupInfo.cb = Marshal.SizeOf<WindowsNativeMethods.STARTUPINFOEX>();
            startupInfo.lpAttributeList = attributeList;

            var commandLine = BuildCommandLine(options);
            if (!WindowsNativeMethods.CreateProcessW(
                null,
                commandLine,
                IntPtr.Zero,
                IntPtr.Zero,
                false,
                WindowsNativeMethods.EXTENDED_STARTUPINFO_PRESENT,
                IntPtr.Zero,
                string.IsNullOrWhiteSpace(options.WorkingDirectory) ? null : options.WorkingDirectory,
                ref startupInfo,
                out processInformation))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            _processId = processInformation.dwProcessId;
            _inputWriteHandle = new SafeFileHandle(inputWritePipe, ownsHandle: true);
            _outputReadHandle = new SafeFileHandle(outputReadPipe, ownsHandle: true);
            _inputStream = new FileStream(_inputWriteHandle, FileAccess.Write, 0x1000, isAsync: false);
            _outputReader = new StreamReader(new FileStream(_outputReadHandle, FileAccess.Read, 0x1000, isAsync: false), Encoding.UTF8, false, 0x1000, leaveOpen: false);
            _readerCancellationTokenSource = new CancellationTokenSource();
            _readerTask = Task.Run(() => PumpOutputAsync(_readerCancellationTokenSource.Token));
            IsRunning = true;
        }
        finally
        {
            if (attributeList != IntPtr.Zero)
            {
                WindowsNativeMethods.DeleteProcThreadAttributeList(attributeList);
                Marshal.FreeHGlobal(attributeList);
            }

            if (processInformation.hThread != IntPtr.Zero)
            {
                WindowsNativeMethods.CloseHandle(processInformation.hThread);
            }

            if (processInformation.hProcess != IntPtr.Zero)
            {
                WindowsNativeMethods.CloseHandle(processInformation.hProcess);
            }

            if (inputReadPipe != IntPtr.Zero)
            {
                WindowsNativeMethods.CloseHandle(inputReadPipe);
            }

            if (outputWritePipe != IntPtr.Zero)
            {
                WindowsNativeMethods.CloseHandle(outputWritePipe);
            }
        }
    }

    /// <inheritdoc/>
    public async Task WriteAsync(string input, CancellationToken cancellationToken = default)
    {
        if (!IsRunning || _inputStream is null || string.IsNullOrEmpty(input))
        {
            return;
        }

        var bytes = Encoding.UTF8.GetBytes(input);
        await _inputStream.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
        await _inputStream.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task ResizeAsync(TerminalSize size, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Size = size.Normalize();
        if (_pseudoConsole != IntPtr.Zero)
        {
            var result = WindowsNativeMethods.ResizePseudoConsole(_pseudoConsole, new WindowsNativeMethods.COORD((short)Size.Columns, (short)Size.Rows));
            if (result != 0)
            {
                Marshal.ThrowExceptionForHR(result);
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!IsRunning)
        {
            return;
        }

        IsRunning = false;

        try
        {
            if (_processId != 0)
            {
                using var process = Process.GetProcessById((int)_processId);
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                    await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
                }
            }
        }
        catch
        {
        }

        if (_readerCancellationTokenSource is not null)
        {
            _readerCancellationTokenSource.Cancel();
        }

        if (_readerTask is not null)
        {
            try
            {
                await _readerTask.ConfigureAwait(false);
            }
            catch
            {
            }
        }

        _outputReader?.Dispose();
        _outputReader = null;
        _inputStream?.Dispose();
        _inputStream = null;
        _inputWriteHandle?.Dispose();
        _inputWriteHandle = null;
        _outputReadHandle?.Dispose();
        _outputReadHandle = null;
        _readerCancellationTokenSource?.Dispose();
        _readerCancellationTokenSource = null;

        if (_pseudoConsole != IntPtr.Zero)
        {
            WindowsNativeMethods.ClosePseudoConsole(_pseudoConsole);
            _pseudoConsole = IntPtr.Zero;
        }

        _processId = 0;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await StopAsync().ConfigureAwait(false);
    }

    private static void CreatePipePair(out IntPtr readPipe, out IntPtr writePipe)
    {
        if (!WindowsNativeMethods.CreatePipe(out readPipe, out writePipe, IntPtr.Zero, 0))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    private async Task PumpOutputAsync(CancellationToken cancellationToken)
    {
        if (_outputReader is null)
        {
            return;
        }

        var buffer = new char[1024];
        while (!cancellationToken.IsCancellationRequested)
        {
            var read = await _outputReader.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken).ConfigureAwait(false);
            if (read <= 0)
            {
                break;
            }

            OutputReceived?.Invoke(this, new string(buffer, 0, read));
        }
    }

    private static string BuildCommandLine(TerminalSessionOptions options)
    {
        var fileName = QuoteIfNeeded(options.FileName);
        if (string.IsNullOrWhiteSpace(options.Arguments))
        {
            return fileName;
        }

        return string.Concat(fileName, " ", options.Arguments);
    }

    private static string QuoteIfNeeded(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Contains(" ", StringComparison.Ordinal) && !value.StartsWith("\"", StringComparison.Ordinal)
            ? string.Concat('"', value, '"')
            : value;
    }
}
