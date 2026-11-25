using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class VSTestCancellationIntegrationTests
{
    [TestMethod]
    public async Task OwnedWorker_CancelsActiveCaseAndCleansUpAfterMissingStartSignal()
    {
        if (Environment.Version.Major != 10)
        {
            Assert.Inconclusive("The controlled fixture targets .NET 10.");
        }
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux())
        {
            Assert.Inconclusive("The fixture requires a Windows job or Linux PID namespace.");
        }
        if (OperatingSystem.IsLinux())
        {
            await RequireLinuxIsolationAsync();
        }

        string repository = FindRepository();
        DirectoryInfo? bin = new(AppContext.BaseDirectory);
        while (bin is not null && !string.Equals(bin.Name, "bin", StringComparison.OrdinalIgnoreCase))
        {
            bin = bin.Parent;
        }
        string root = Path.Combine(bin?.Parent?.FullName ?? AppContext.BaseDirectory, $"vc-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string adapter = Path.Combine(root, "a");
            string worker = Path.Combine(root, "w");
            Directory.CreateDirectory(adapter);
            Directory.CreateDirectory(worker);
            await File.WriteAllTextAsync(Path.Combine(adapter, "C.TestAdapter.csproj"), AdapterProject);
            await File.WriteAllTextAsync(Path.Combine(adapter, "Adapter.cs"), AdapterSource);
            string baseProject = Path.Combine(repository, "AA98_AvlnCodeStudio.Base.Testing", "AA98_AvlnCodeStudio.Base.Testing.csproj");
            await File.WriteAllTextAsync(Path.Combine(worker, "W.csproj"), WorkerProject(baseProject));
            await File.WriteAllTextAsync(Path.Combine(worker, "Program.cs"), WorkerSource);

            string artifacts = Path.Combine(root, "o");
            string props = Path.Combine(repository, "tools", "build", "IsolatedArtifacts.props");
            await BuildAsync(Path.Combine(adapter, "C.TestAdapter.csproj"), artifacts, props);
            await BuildAsync(Path.Combine(worker, "W.csproj"), artifacts, props);
            Assert.IsTrue(File.Exists(Path.Combine(artifacts, "obj", "AA98_AvlnCodeStudio.Base.Testing", "project.assets.json")));
            Assert.IsTrue(File.Exists(Path.Combine(artifacts, "obj", "AA98_AvlnCodeStudio.Base", "project.assets.json")));
            string output = Path.Combine(artifacts, "bin", "C.TestAdapter", "Debug", "net10.0");
            string assembly = Path.Combine(output, "C.TestAdapter.dll");
            string workerAssembly = Path.Combine(artifacts, "bin", "W", "Debug", "net10.0", "W.dll");

            await RunScenarioAsync(workerAssembly, assembly, suppressStart: false, beforeGate: false, lateResult: false);
            await RunScenarioAsync(workerAssembly, assembly, suppressStart: false, beforeGate: false, lateResult: true);
            await RunScenarioAsync(workerAssembly, assembly, suppressStart: false, beforeGate: false, lateResult: false, debug: true);
            await RunScenarioAsync(workerAssembly, assembly, suppressStart: true, beforeGate: false, lateResult: false);
            await RunScenarioAsync(workerAssembly, assembly, suppressStart: false, beforeGate: true, lateResult: false);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static async Task RunScenarioAsync(string workerAssembly, string assembly, bool suppressStart, bool beforeGate, bool lateResult, bool debug = false)
    {
        string directory = Path.GetDirectoryName(assembly)!;
        foreach (string name in new[] { "boot", "gate", "host", "ready", "cancel", "result", "selected", "no-start", "adapter-canceled", "late-mode", "late-result", "debug-mode", "attached", "detached", "partial-result" })
        {
            File.Delete(Path.Combine(directory, name));
        }
        if (suppressStart)
        {
            await File.WriteAllTextAsync(Path.Combine(directory, "no-start"), "1");
        }
        if (lateResult)
        {
            await File.WriteAllTextAsync(Path.Combine(directory, "late-mode"), "1");
        }
        if (debug)
        {
            await File.WriteAllTextAsync(Path.Combine(directory, "debug-mode"), "1");
        }

        await using OwnedWorker worker = await OwnedWorker.StartAsync(workerAssembly, assembly);
        await WaitForFileAsync(Path.Combine(directory, "boot"), worker.Process, TimeSpan.FromSeconds(60));
        await worker.IdentifyAsync();
        if (beforeGate)
        {
            await worker.TerminateAsync();
            await worker.AssertStoppedAsync();
            return;
        }
        await File.WriteAllTextAsync(Path.Combine(directory, "gate"), "1");
        await WaitForFileAsync(Path.Combine(directory, "host"), worker.Process, TimeSpan.FromSeconds(90));
        string[] marker = (await File.ReadAllTextAsync(Path.Combine(directory, "host"))).Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.AreEqual(5, marker.Length);
        int innerHostPid = int.Parse(marker[0]);
        Assert.AreEqual(await File.ReadAllTextAsync(Path.Combine(directory, "selected")), string.Join('\n', marker.Skip(1)), "The adapter ran a different case.");
        if (debug)
        {
            Assert.AreEqual(innerHostPid.ToString(), await File.ReadAllTextAsync(Path.Combine(directory, "attached")));
            Assert.AreEqual("recorded", await File.ReadAllTextAsync(Path.Combine(directory, "partial-result")));
        }
        await worker.TrackHostAndRunnerAsync(innerHostPid);

        if (suppressStart)
        {
            await Assert.ThrowsExactlyAsync<TimeoutException>(() =>
                WaitForFileAsync(Path.Combine(directory, "ready"), worker.Process, TimeSpan.FromSeconds(2)));
            await worker.TerminateAsync();
        }
        else
        {
            await WaitForFileAsync(Path.Combine(directory, "ready"), worker.Process, TimeSpan.FromSeconds(10));
            await File.WriteAllTextAsync(Path.Combine(directory, "cancel"), "1");
            await worker.WaitForExitAsync(TimeSpan.FromSeconds(60));
            if (worker.Process.ExitCode != 0)
            {
                Assert.Fail($"Worker failed: {await worker.DiagnosticsAsync().WaitAsync(TimeSpan.FromSeconds(10))}");
            }
            Assert.AreEqual("canceled", await File.ReadAllTextAsync(Path.Combine(directory, "result")));
            if (debug)
            {
                Assert.AreEqual("detached\n", await File.ReadAllTextAsync(Path.Combine(directory, "detached")));
            }
            else
            {
                Assert.IsTrue(File.Exists(Path.Combine(directory, "adapter-canceled")), "The adapter did not receive Cancel.");
            }
            if (lateResult)
            {
                Assert.AreEqual("recorded", await File.ReadAllTextAsync(Path.Combine(directory, "late-result")));
            }
        }
        await worker.AssertStoppedAsync();
    }

    private static async Task RequireLinuxIsolationAsync()
    {
        const string unshare = "/usr/bin/unshare";
        if (!File.Exists(unshare)
            || !Encoding.ASCII.GetString(await File.ReadAllBytesAsync(unshare)).Contains("pidfd_open", StringComparison.Ordinal))
        {
            Assert.Inconclusive("Linux isolation needs util-linux unshare built with pidfd_open.");
        }
        try
        {
            using SafeFileHandle pidfd = Native.OpenPidFd(Environment.ProcessId);
        }
        catch (InvalidOperationException exception)
        {
            Assert.Inconclusive($"Linux pidfds are unavailable: {exception.Message}");
        }
        ProcessStartInfo versionStart = new(unshare) { RedirectStandardOutput = true, RedirectStandardError = true };
        versionStart.ArgumentList.Add("--version");
        using Process versionProcess = Process.Start(versionStart)!;
        using SafeFileHandle? versionPidFd = OpenPidFdUnlessExited(versionProcess);
        Task<string> versionOutput = versionProcess.StandardOutput.ReadToEndAsync();
        try
        {
            await versionProcess.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(10));
        }
        catch (TimeoutException)
        {
            Native.KillPidFd(versionPidFd ?? throw new InvalidOperationException("Version probe PID was not retained."));
            await versionProcess.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
            Assert.Inconclusive("The unshare version probe timed out.");
        }
        string versionText = await versionOutput;
        if (versionProcess.ExitCode != 0
            || !Version.TryParse(versionText.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).LastOrDefault(), out Version? version)
            || version < new Version(2, 41, 3))
        {
            Assert.Inconclusive("Linux isolation needs util-linux unshare 2.41.3 or newer.");
        }

        ProcessStartInfo probe = new(unshare) { RedirectStandardOutput = true, RedirectStandardError = true };
        foreach (string argument in new[] { "--user", "--map-root-user", "--mount", "--pid", "--fork", "--mount-proc", "--kill-child=KILL", "/usr/bin/true" })
        {
            probe.ArgumentList.Add(argument);
        }
        using Process process = Process.Start(probe)!;
        using SafeFileHandle? probePidFd = OpenPidFdUnlessExited(process);
        try
        {
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
        }
        catch (TimeoutException)
        {
            Native.KillPidFd(probePidFd ?? throw new InvalidOperationException("Namespace probe PID was not retained."));
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
            Assert.Inconclusive("The PID namespace availability probe timed out.");
        }
        if (process.ExitCode != 0)
        {
            Assert.Inconclusive($"PID namespaces are unavailable: {await process.StandardError.ReadToEndAsync()}");
        }
    }

    private static SafeFileHandle? OpenPidFdUnlessExited(Process process)
    {
        try
        {
            return Native.OpenPidFd(process.Id);
        }
        catch (InvalidOperationException) when (process.HasExited)
        {
            return null;
        }
    }

    private static async Task WaitForFileAsync(string path, Process process, TimeSpan timeout)
    {
        Stopwatch timer = Stopwatch.StartNew();
        while (!File.Exists(path))
        {
            if (process.HasExited)
            {
                throw new InvalidOperationException($"Worker exited before creating {Path.GetFileName(path)} (exit {process.ExitCode}).");
            }
            if (timer.Elapsed >= timeout)
            {
                throw new TimeoutException($"Worker did not create {Path.GetFileName(path)}.");
            }
            await Task.Delay(50);
        }
    }

    private static async Task BuildAsync(string project, string root, string props)
    {
        ProcessStartInfo start = new("dotnet") { RedirectStandardOutput = true, RedirectStandardError = true };
        start.Environment["MSBUILDDISABLENODEREUSE"] = "1";
        start.Environment["DOTNET_CLI_USE_MSBUILD_SERVER"] = "0";
        foreach (string key in new[] { "HostArtifactRoot", "CustomAfterDirectoryBuildProps", "DirectoryBuildPropsPath" })
        {
            start.Environment.Remove(key);
        }
        foreach (string argument in new[] { "build", project, "-f", "net10.0", "--nologo", "-v", "quiet", "-nodeReuse:false", "-p:UseSharedCompilation=false", $"-p:HostArtifactRoot={root}", $"-p:CustomAfterDirectoryBuildProps={props}" })
        {
            start.ArgumentList.Add(argument);
        }
        using Process process = Process.Start(start) ?? throw new InvalidOperationException("Could not start fixture build.");
        Task<string> stdout = process.StandardOutput.ReadToEndAsync();
        Task<string> stderr = process.StandardError.ReadToEndAsync();
        try
        {
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromMinutes(3));
        }
        catch (TimeoutException)
        {
            process.Kill(entireProcessTree: true);
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
            throw;
        }
        Assert.AreEqual(0, process.ExitCode, $"{await stdout}\n{await stderr}");
    }

    private static string FindRepository([CallerFilePath] string source = "")
    {
        DirectoryInfo? directory = new(Path.GetDirectoryName(source)!);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "tools", "build", "IsolatedArtifacts.props")))
        {
            directory = directory.Parent;
        }
        return directory?.FullName ?? throw new InvalidOperationException("Cannot find fixture isolation props.");
    }

    private sealed class OwnedWorker : IAsyncDisposable
    {
        private readonly SafeFileHandle? _job;
        private readonly List<Process> _tracked = [];
        private readonly List<SafeFileHandle> _pidfds = [];
        private readonly Task<string> _stdout;
        private readonly Task<string> _stderr;
        private readonly SafeFileHandle? _rootPidFd;
        private FileStream? _namespaceHandle;
        private string? _namespace;
        private int _workerPid;

        private OwnedWorker(Process process, SafeFileHandle? job)
        {
            Process = process;
            _job = job;
            _rootPidFd = OperatingSystem.IsLinux() ? Native.OpenPidFd(process.Id) : null;
            _stdout = process.StandardOutput.ReadToEndAsync();
            _stderr = process.StandardError.ReadToEndAsync();
        }

        public Process Process { get; }

        public static async Task<OwnedWorker> StartAsync(string workerAssembly, string assembly)
        {
            SafeFileHandle? job = OperatingSystem.IsWindows() ? Native.CreateJob() : null;
            ProcessStartInfo start = new(OperatingSystem.IsWindows() ? "dotnet" : "/usr/bin/unshare")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            if (OperatingSystem.IsLinux())
            {
                // The namespace's PID 1 is the gated worker; its death also ends its descendants.
                foreach (string argument in new[] { "--user", "--map-root-user", "--mount", "--pid", "--fork", "--mount-proc", "--kill-child=KILL", "dotnet" })
                {
                    start.ArgumentList.Add(argument);
                }
            }
            start.ArgumentList.Add(workerAssembly);
            start.ArgumentList.Add(assembly);
            Process? process = null;
            try
            {
                process = Process.Start(start) ?? throw new InvalidOperationException("Could not start fixture worker.");
                OwnedWorker owned = new(process, job);
                if (job is not null)
                {
                    Native.Assign(job, process);
                    owned._workerPid = process.Id;
                }
                return owned;
            }
            catch
            {
                if (process is not null)
                {
                    if (!process.HasExited)
                    {
                        process.Kill(entireProcessTree: true);
                        await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
                    }
                    process.Dispose();
                }
                job?.Dispose();
                throw;
            }
        }

        public async Task IdentifyAsync()
        {
            if (OperatingSystem.IsLinux())
            {
                string childrenPath = $"/proc/{Process.Id}/task/{Process.Id}/children";
                string children = await File.ReadAllTextAsync(childrenPath);
                _workerPid = int.Parse(children.Split(' ', StringSplitOptions.RemoveEmptyEntries).Single());
                string path = $"/proc/{_workerPid}/ns/pid";
                _namespace = new FileInfo(path).LinkTarget ?? throw new InvalidOperationException("Cannot identify worker PID namespace.");
                _namespaceHandle = File.OpenRead(path);
            }
            await TrackAsync(_workerPid);
        }

        public async Task TrackHostAndRunnerAsync(int innerPid)
        {
            int hostPid = OperatingSystem.IsWindows() ? innerPid : FindNamespacePid(innerPid);
            using Process host = Process.GetProcessById(hostPid);
            if (_job is not null)
            {
                Assert.IsTrue(Native.InJob(_job, host));
            }
            int runnerPid = Native.ParentPid(hostPid, host);
            using Process runner = Process.GetProcessById(runnerPid);
            if (_job is not null)
            {
                Assert.IsTrue(Native.InJob(_job, runner));
            }
            Assert.AreEqual(_workerPid, Native.ParentPid(runnerPid, runner), "VSTest runner is not a direct worker child.");
            await TrackAsync(runnerPid);
            await TrackAsync(hostPid);
        }

        private int FindNamespacePid(int innerPid)
        {
            foreach (string path in Directory.EnumerateDirectories("/proc"))
            {
                if (!int.TryParse(Path.GetFileName(path), out int pid))
                {
                    continue;
                }
                try
                {
                    if (new FileInfo(Path.Combine(path, "ns", "pid")).LinkTarget != _namespace)
                    {
                        continue;
                    }
                    string? line = File.ReadLines(Path.Combine(path, "status")).FirstOrDefault(line => line.StartsWith("NSpid:", StringComparison.Ordinal));
                    if (line is not null && int.Parse(line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Last()) == innerPid)
                    {
                        return pid;
                    }
                }
                catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
                {
                }
            }
            throw new InvalidOperationException("Host PID not found in the owned PID namespace.");
        }

        private Task TrackAsync(int pid)
        {
            if (OperatingSystem.IsLinux())
            {
                _pidfds.Add(Native.OpenPidFd(pid));
            }
            else
            {
                _tracked.Add(Process.GetProcessById(pid));
            }
            return Task.CompletedTask;
        }

        public async Task WaitForExitAsync(TimeSpan timeout) => await Process.WaitForExitAsync().WaitAsync(timeout);

        public async Task TerminateAsync()
        {
            if (_job is not null)
            {
                Native.Terminate(_job);
            }
            else if (!Process.HasExited)
            {
                Native.KillPidFd(_rootPidFd!);
            }
            await WaitForExitAsync(TimeSpan.FromSeconds(30));
        }

        public async Task AssertStoppedAsync()
        {
            foreach (Process process in _tracked)
            {
                await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
            }
            foreach (SafeFileHandle pidfd in _pidfds)
            {
                Assert.IsTrue(Native.WaitPidFd(pidfd, 30000), "An identified namespace process survived.");
            }
            if (_namespace is not null)
            {
                Stopwatch timer = Stopwatch.StartNew();
                while (NamespaceHasProcesses())
                {
                    if (timer.Elapsed >= TimeSpan.FromSeconds(30))
                    {
                        Assert.Fail("The owned PID namespace still contains processes.");
                    }
                    await Task.Delay(50);
                }
            }
            if (_job is not null)
            {
                Assert.AreEqual(0u, Native.ActiveProcesses(_job), "The worker job still contains processes.");
            }
        }

        private bool NamespaceHasProcesses()
        {
            foreach (string path in Directory.EnumerateDirectories("/proc"))
            {
                if (!int.TryParse(Path.GetFileName(path), out _))
                {
                    continue;
                }
                try
                {
                    if (new FileInfo(Path.Combine(path, "ns", "pid")).LinkTarget == _namespace)
                    {
                        return true;
                    }
                }
                catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
                {
                }
            }
            return false;
        }

        public async Task<string> DiagnosticsAsync() => $"{await _stdout}\n{await _stderr}";

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (!Process.HasExited || (_job is not null && Native.ActiveProcesses(_job) != 0))
                {
                    await TerminateAsync();
                }
                await AssertStoppedAsync();
            }
            finally
            {
                _namespaceHandle?.Dispose();
                _rootPidFd?.Dispose();
                foreach (SafeFileHandle handle in _pidfds) { handle.Dispose(); }
                foreach (Process process in _tracked) { process.Dispose(); }
                _job?.Dispose();
                Process.Dispose();
            }
        }
    }

    private static class Native
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct BasicLimit
        {
            public long PerProcess, PerJob;
            public uint Flags;
            public UIntPtr Minimum, Maximum;
            public uint ActiveLimit;
            public UIntPtr Affinity;
            public uint Priority, Scheduling;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IoCounters { public ulong A, B, C, D, E, F; }

        [StructLayout(LayoutKind.Sequential)]
        private struct ExtendedLimit
        {
            public BasicLimit Basic;
            public IoCounters Io;
            public UIntPtr ProcessMemory, JobMemory, PeakProcessMemory, PeakJobMemory;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Accounting
        {
            public long A, B, C, D;
            public uint Faults, Total, Active, Terminated;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ProcessBasic
        {
            public IntPtr A, Peb, C, D, Id, Parent;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PollFd { public int Fd; public short Events, Returned; }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeFileHandle CreateJobObject(IntPtr attributes, string? name);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetInformationJobObject(SafeFileHandle job, int type, ref ExtendedLimit info, int size);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AssignProcessToJobObject(SafeFileHandle job, IntPtr process);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool IsProcessInJob(IntPtr process, SafeFileHandle job, out bool inJob);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool TerminateJobObject(SafeFileHandle job, uint exitCode);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool QueryInformationJobObject(SafeFileHandle job, int type, out Accounting info, int size, IntPtr returned);
        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr process, int infoClass, out ProcessBasic info, int size, out int returned);
        [DllImport("libc", SetLastError = true)]
        private static extern long syscall(long number, int pid, uint flags);
        [DllImport("libc", EntryPoint = "syscall", SetLastError = true)]
        private static extern long SignalPidFd(long number, int fd, int signal, IntPtr info, uint flags);
        [DllImport("libc", SetLastError = true)]
        private static extern int poll(ref PollFd fds, uint count, int timeout);

        public static SafeFileHandle CreateJob()
        {
            SafeFileHandle job = CreateJobObject(IntPtr.Zero, null);
            if (job.IsInvalid) { throw new InvalidOperationException($"CreateJobObject: {Marshal.GetLastWin32Error()}"); }
            ExtendedLimit info = new() { Basic = new BasicLimit { Flags = 0x2000 } };
            if (!SetInformationJobObject(job, 9, ref info, Marshal.SizeOf<ExtendedLimit>()))
            {
                job.Dispose();
                throw new InvalidOperationException($"SetInformationJobObject: {Marshal.GetLastWin32Error()}");
            }
            return job;
        }

        public static void Assign(SafeFileHandle job, Process process)
        {
            if (!AssignProcessToJobObject(job, process.Handle))
            {
                throw new InvalidOperationException($"AssignProcessToJobObject: {Marshal.GetLastWin32Error()}");
            }
        }

        public static bool InJob(SafeFileHandle job, Process process) =>
            IsProcessInJob(process.Handle, job, out bool inJob) && inJob;

        public static void Terminate(SafeFileHandle job)
        {
            if (!TerminateJobObject(job, 1)) { throw new InvalidOperationException($"TerminateJobObject: {Marshal.GetLastWin32Error()}"); }
        }

        public static uint ActiveProcesses(SafeFileHandle job)
        {
            if (!QueryInformationJobObject(job, 1, out Accounting info, Marshal.SizeOf<Accounting>(), IntPtr.Zero))
            {
                throw new InvalidOperationException($"QueryInformationJobObject: {Marshal.GetLastWin32Error()}");
            }
            return info.Active;
        }

        public static int ParentPid(int pid, Process process)
        {
            if (OperatingSystem.IsWindows())
            {
                int status = NtQueryInformationProcess(process.Handle, 0, out ProcessBasic info, Marshal.SizeOf<ProcessBasic>(), out _);
                if (status != 0) { throw new InvalidOperationException($"NtQueryInformationProcess: {status}"); }
                return checked((int)info.Parent);
            }
            string stat = File.ReadAllText($"/proc/{pid}/stat");
            return int.Parse(stat[(stat.LastIndexOf(')') + 2)..].Split(' ')[1]);
        }

        public static SafeFileHandle OpenPidFd(int pid)
        {
            int fd = checked((int)syscall(434, pid, 0));
            if (fd < 0) { throw new InvalidOperationException($"pidfd_open: {Marshal.GetLastWin32Error()}"); }
            return new SafeFileHandle((IntPtr)fd, ownsHandle: true);
        }

        public static void KillPidFd(SafeFileHandle pidfd)
        {
            long result = SignalPidFd(424, checked((int)pidfd.DangerousGetHandle()), 9, IntPtr.Zero, 0);
            int error = Marshal.GetLastWin32Error();
            if (result != 0 && error != 3)
            {
                throw new InvalidOperationException($"pidfd_send_signal: {error}");
            }
        }

        public static bool WaitPidFd(SafeFileHandle pidfd, int timeout)
        {
            PollFd fd = new() { Fd = checked((int)pidfd.DangerousGetHandle()), Events = 1 };
            Stopwatch timer = Stopwatch.StartNew();
            do
            {
                int result = poll(ref fd, 1, Math.Max(0, timeout - (int)timer.ElapsedMilliseconds));
                if (result >= 0)
                {
                    return result > 0 && (fd.Returned & 0x11) != 0;
                }
                if (Marshal.GetLastWin32Error() != 4)
                {
                    throw new InvalidOperationException($"pidfd poll: {Marshal.GetLastWin32Error()}");
                }
            }
            while (timer.ElapsedMilliseconds < timeout);
            return false;
        }
    }

    private const string AdapterProject = """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
            <IsTestProject>true</IsTestProject>
            <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
          </PropertyGroup>
          <ItemGroup><PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.12.0" /></ItemGroup>
        </Project>
        """;

    private static string WorkerProject(string baseProject) => $"""
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup>
          <ItemGroup><ProjectReference Include="{SecurityElement.Escape(baseProject)}" /></ItemGroup>
        </Project>
        """;

    private const string AdapterSource = """
        using System;
        using System.Collections.Generic;
        using System.IO;
        using System.Threading;
        using Microsoft.VisualStudio.TestPlatform.ObjectModel;
        using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
        using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

        [FileExtension(".dll")]
        [DefaultExecutorUri("executor://controlled-cancel/v1")]
        public sealed class Discoverer : ITestDiscoverer
        {
            public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext context, IMessageLogger logger, ITestCaseDiscoverySink sink)
            {
                foreach (string source in sources)
                    sink.SendTestCase(new TestCase("Controlled.Example", new Uri("executor://controlled-cancel/v1"), source));
            }
        }

        [ExtensionUri("executor://controlled-cancel/v1")]
        public sealed class Executor : ITestExecutor
        {
            private readonly ManualResetEventSlim canceled = new(false);
            public void Cancel() => canceled.Set();
            public void RunTests(IEnumerable<string> sources, IRunContext context, IFrameworkHandle handle) => throw new NotSupportedException();
            public void RunTests(IEnumerable<TestCase> tests, IRunContext context, IFrameworkHandle handle)
            {
                foreach (TestCase test in tests)
                {
                    string directory = Path.GetDirectoryName(test.Source);
                    bool late = File.Exists(Path.Combine(directory, "late-mode"));
                    if (!late)
                    {
                        handle.RecordResult(new TestResult(test) { Outcome = TestOutcome.Passed });
                        File.WriteAllText(Path.Combine(directory, "partial-result"), "recorded");
                    }
                    string pending = Path.Combine(directory, "host.pending");
                    File.WriteAllText(pending, Environment.ProcessId + "\n" + test.Id + "\n" + test.ExecutorUri + "\n" + test.Source + "\n" + test.FullyQualifiedName);
                    File.Move(pending, Path.Combine(directory, "host"));
                    if (!File.Exists(Path.Combine(directory, "no-start")))
                        File.WriteAllText(Path.Combine(directory, "ready"), "1");
                    if (canceled.Wait(TimeSpan.FromSeconds(120)))
                    {
                        File.WriteAllText(Path.Combine(directory, "adapter-canceled"), "1");
                        if (late)
                        {
                            handle.RecordResult(new TestResult(test) { Outcome = TestOutcome.Passed });
                            File.WriteAllText(Path.Combine(directory, "late-result"), "recorded");
                        }
                    }
                }
            }
        }
        """;

    private const string WorkerSource = """
        using System;
        using System.IO;
        using System.Linq;
        using System.Threading;
        using System.Threading.Tasks;
        using AA98_AvlnCodeStudio.Base.Testing.Services;

        string assembly = Path.GetFullPath(args[0]);
        string directory = Path.GetDirectoryName(assembly);
        File.WriteAllText(Path.Combine(directory, "boot"), "1");
        DateTime deadline = DateTime.UtcNow.AddSeconds(90);
        while (!File.Exists(Path.Combine(directory, "gate")) && DateTime.UtcNow < deadline)
            await Task.Delay(50);
        if (!File.Exists(Path.Combine(directory, "gate")))
            throw new TimeoutException("Worker gate was not opened.");
        DotNetTestDiscoveryService service = new();
        var discovery = await service.DiscoverAsync(assembly, directory);
        var selected = discovery.Cases.Single(test => test.FullyQualifiedName == "Controlled.Example");
        File.WriteAllText(Path.Combine(directory, "selected"), selected.Id + "\n" + selected.ExecutorUri + "\n" + selected.Source + "\n" + selected.FullyQualifiedName);
        using CancellationTokenSource cancellation = new();
        Task run = File.Exists(Path.Combine(directory, "debug-mode"))
            ? service.DebugCaseAsync(assembly, directory, selected, (pid, token) =>
            {
                token.ThrowIfCancellationRequested();
                File.WriteAllText(Path.Combine(directory, "attached"), pid.ToString());
                return Task.FromResult<IAsyncDisposable>(new Lease(directory));
            }, cancellation.Token)
            : service.RunCaseAsync(assembly, directory, selected, cancellation.Token);
        while (!File.Exists(Path.Combine(directory, "cancel")) && !run.IsCompleted)
            await Task.Delay(50);
        cancellation.Cancel();
        try
        {
            await run;
            throw new InvalidOperationException("Canceled run returned a result.");
        }
        catch (OperationCanceledException error) when (error.CancellationToken == cancellation.Token)
        {
            File.WriteAllText(Path.Combine(directory, "result"), "canceled");
        }

        sealed class Lease(string directory) : IAsyncDisposable
        {
            public ValueTask DisposeAsync()
            {
                File.AppendAllText(Path.Combine(directory, "detached"), "detached\n");
                return ValueTask.CompletedTask;
            }
        }
        """;
}
