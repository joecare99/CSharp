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

namespace ConsoleLib.Showcase.Terminal.Core;

/// <summary>
/// Provides the showcase-only Windows ConPTY implementation.
/// The provider-neutral terminal contracts remain in <see cref="Terminal.Core"/>.
/// </summary>
public sealed class WindowsConPtyTerminalBridge : ITerminalSession
{
    private SafeFileHandle? _inputWriteHandle;
    private SafeFileHandle? _outputReadHandle;
    private Stream? _inputStream;
    private StreamReader? _outputReader;
    private CancellationTokenSource? _readerCancellation;
    private Task? _readerTask;
    private IntPtr _pseudoConsole;
    private uint _processId;

    /// <summary>Gets a value indicating whether this bridge can run on the current host.</summary>
    public static bool IsSupported => OperatingSystem.IsWindows();

    /// <summary>Gets the parsed terminal document produced by this session.</summary>
    public TerminalDocument? Document { get; private set; }

    /// <inheritdoc />
    public event EventHandler<string>? OutputReceived;

    /// <summary>Occurs when the parsed terminal viewport changes.</summary>
    public event EventHandler<TerminalSnapshot>? SnapshotChanged;

    /// <inheritdoc />
    public bool IsRunning { get; private set; }

    /// <inheritdoc />
    public TerminalSize Size { get; private set; } = new(80, 25);

    /// <inheritdoc />
    public async Task StartAsync(TerminalSessionOptions options, CancellationToken cancellationToken = default)
    {
        if (!IsSupported)
            throw new PlatformNotSupportedException("The showcase ConPTY bridge requires Windows.");
        if (IsRunning)
            throw new InvalidOperationException("The terminal session is already running.");
        if (options is null)
            throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(options.FileName))
            throw new ArgumentException("A terminal executable is required.", nameof(options));

        cancellationToken.ThrowIfCancellationRequested();
        Size = options.InitialSize.Normalize();
        Document = new TerminalDocument(Size);

        IntPtr inputRead = IntPtr.Zero;
        IntPtr inputWrite = IntPtr.Zero;
        IntPtr outputRead = IntPtr.Zero;
        IntPtr outputWrite = IntPtr.Zero;
        IntPtr attributeList = IntPtr.Zero;
        IntPtr attributeListSize = IntPtr.Zero;
        NativeMethods.PROCESS_INFORMATION process = default;

        try
        {
            CreatePipePair(out inputRead, out inputWrite);
            CreatePipePair(out outputRead, out outputWrite);

            var result = NativeMethods.CreatePseudoConsole(
                new NativeMethods.COORD((short)Size.Columns, (short)Size.Rows),
                inputRead,
                outputWrite,
                0,
                out _pseudoConsole);
            if (result != 0)
                Marshal.ThrowExceptionForHR(result);

            NativeMethods.InitializeProcThreadAttributeList(IntPtr.Zero, 1, 0, ref attributeListSize);
            attributeList = Marshal.AllocHGlobal(attributeListSize);
            if (!NativeMethods.InitializeProcThreadAttributeList(attributeList, 1, 0, ref attributeListSize))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            if (!NativeMethods.UpdateProcThreadAttribute(
                attributeList,
                0,
                (IntPtr)NativeMethods.PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE,
                _pseudoConsole,
                (IntPtr)IntPtr.Size,
                IntPtr.Zero,
                IntPtr.Zero))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var startupInfo = new NativeMethods.STARTUPINFOEX
            {
                StartupInfo = new NativeMethods.STARTUPINFO
                {
                    cb = Marshal.SizeOf<NativeMethods.STARTUPINFOEX>()
                },
                lpAttributeList = attributeList
            };

            var commandLine = BuildCommandLine(options);
            if (!NativeMethods.CreateProcessW(
                null,
                commandLine,
                IntPtr.Zero,
                IntPtr.Zero,
                false,
                NativeMethods.EXTENDED_STARTUPINFO_PRESENT,
                IntPtr.Zero,
                string.IsNullOrWhiteSpace(options.WorkingDirectory) ? null : options.WorkingDirectory,
                ref startupInfo,
                out process))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            _processId = process.dwProcessId;
            _inputWriteHandle = new SafeFileHandle(inputWrite, ownsHandle: true);
            inputWrite = IntPtr.Zero;
            _outputReadHandle = new SafeFileHandle(outputRead, ownsHandle: true);
            outputRead = IntPtr.Zero;
            _inputStream = new FileStream(_inputWriteHandle, FileAccess.Write, 4096, isAsync: false);
            _outputReader = new StreamReader(
                new FileStream(_outputReadHandle, FileAccess.Read, 4096, isAsync: false),
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 4096,
                leaveOpen: false);
            _readerCancellation = new CancellationTokenSource();
            _readerTask = Task.Run(() => PumpOutputAsync(_readerCancellation.Token));
            IsRunning = true;
        }
        finally
        {
            if (attributeList != IntPtr.Zero)
            {
                NativeMethods.DeleteProcThreadAttributeList(attributeList);
                Marshal.FreeHGlobal(attributeList);
            }
            CloseHandle(ref process.hThread);
            CloseHandle(ref process.hProcess);
            CloseHandle(ref inputRead);
            CloseHandle(ref outputWrite);
            CloseHandle(ref inputWrite);
            CloseHandle(ref outputRead);
        }
    }

    /// <inheritdoc />
    public async Task WriteAsync(string input, CancellationToken cancellationToken = default)
    {
        if (!IsRunning || _inputStream is null || string.IsNullOrEmpty(input))
            return;

        var bytes = Encoding.UTF8.GetBytes(input);
        await _inputStream.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
        await _inputStream.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task ResizeAsync(TerminalSize size, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Size = size.Normalize();
        Document?.Resize(Size);
        if (_pseudoConsole != IntPtr.Zero)
        {
            var result = NativeMethods.ResizePseudoConsole(
                _pseudoConsole,
                new NativeMethods.COORD((short)Size.Columns, (short)Size.Rows));
            if (result != 0)
                Marshal.ThrowExceptionForHR(result);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!IsRunning && _pseudoConsole == IntPtr.Zero)
            return;

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
        catch (ArgumentException)
        {
        }
        finally
        {
            _readerCancellation?.Cancel();
            if (_readerTask is not null)
            {
                try { await _readerTask.ConfigureAwait(false); }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
            }

            _outputReader?.Dispose();
            _inputStream?.Dispose();
            _inputWriteHandle?.Dispose();
            _outputReadHandle?.Dispose();
            _readerCancellation?.Dispose();
            _outputReader = null;
            _inputStream = null;
            _inputWriteHandle = null;
            _outputReadHandle = null;
            _readerCancellation = null;
            _readerTask = null;

            if (_pseudoConsole != IntPtr.Zero)
            {
                NativeMethods.ClosePseudoConsole(_pseudoConsole);
                _pseudoConsole = IntPtr.Zero;
            }

            _processId = 0;
        }
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync() => new(StopAsync());

    private static void CreatePipePair(out IntPtr readPipe, out IntPtr writePipe)
    {
        if (!NativeMethods.CreatePipe(out readPipe, out writePipe, IntPtr.Zero, 0))
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    private async Task PumpOutputAsync(CancellationToken cancellationToken)
    {
        if (_outputReader is null)
            return;

        var buffer = new char[1024];
        while (!cancellationToken.IsCancellationRequested)
        {
            var count = await _outputReader.ReadAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false);
            if (count == 0)
                break;
            var text = new string(buffer, 0, count);
            Document?.ApplyOutput(text);
            OutputReceived?.Invoke(this, text);
            if (Document is not null)
                SnapshotChanged?.Invoke(this, Document.CreateSnapshot());
        }
    }

    private static string BuildCommandLine(TerminalSessionOptions options)
    {
        var fileName = QuoteIfNeeded(options.FileName);
        return string.IsNullOrWhiteSpace(options.Arguments)
            ? fileName
            : $"{fileName} {options.Arguments}";
    }

    private static string QuoteIfNeeded(string value) =>
        value.Contains(' ', StringComparison.Ordinal) && !value.StartsWith('"')
            ? $"\"{value}\""
            : value;

    private static void CloseHandle(ref IntPtr handle)
    {
        if (handle == IntPtr.Zero)
            return;
        NativeMethods.CloseHandle(handle);
        handle = IntPtr.Zero;
    }

    private static class NativeMethods
    {
        internal const int PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE = 0x00020016;
        internal const uint EXTENDED_STARTUPINFO_PRESENT = 0x00080000;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CreatePipe(out IntPtr readPipe, out IntPtr writePipe, IntPtr attributes, int size);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int CreatePseudoConsole(COORD size, IntPtr input, IntPtr output, uint flags, out IntPtr pseudoConsole);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int ResizePseudoConsole(IntPtr pseudoConsole, COORD size);

        [DllImport("kernel32.dll")]
        internal static extern void ClosePseudoConsole(IntPtr pseudoConsole);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool InitializeProcThreadAttributeList(IntPtr list, int count, int flags, ref IntPtr size);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UpdateProcThreadAttribute(IntPtr list, uint flags, IntPtr attribute, IntPtr value, IntPtr size, IntPtr previous, IntPtr returnSize);

        [DllImport("kernel32.dll")]
        internal static extern void DeleteProcThreadAttributeList(IntPtr list);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CreateProcessW(string? applicationName, string? commandLine, IntPtr processAttributes, IntPtr threadAttributes, bool inheritHandles, uint creationFlags, IntPtr environment, string? currentDirectory, ref STARTUPINFOEX startupInfo, out PROCESS_INFORMATION processInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr handle);

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct COORD(short x, short y)
        {
            internal readonly short X = x;
            internal readonly short Y = y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct STARTUPINFOEX
        {
            internal STARTUPINFO StartupInfo;
            internal IntPtr lpAttributeList;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct STARTUPINFO
        {
            internal int cb;
            internal string? lpReserved;
            internal string? lpDesktop;
            internal string? lpTitle;
            internal int dwX;
            internal int dwY;
            internal int dwXSize;
            internal int dwYSize;
            internal int dwXCountChars;
            internal int dwYCountChars;
            internal int dwFillAttribute;
            internal int dwFlags;
            internal short wShowWindow;
            internal short cbReserved2;
            internal IntPtr lpReserved2;
            internal IntPtr hStdInput;
            internal IntPtr hStdOutput;
            internal IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION
        {
            internal IntPtr hProcess;
            internal IntPtr hThread;
            internal uint dwProcessId;
            internal uint dwThreadId;
        }
    }
}
