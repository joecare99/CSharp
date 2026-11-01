using AA98_AvlnCodeStudio.Base.Debugging.Models;
using AA98_AvlnCodeStudio.Base.Debugging.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class NetCoreDbgDebugSessionServiceTests
{
    [TestMethod]
    public async Task StartAndStopSession_UsesDapLaunchAndDisconnectRequests()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_NetCoreDbg_{Guid.NewGuid():N}");
        string projectDirectory = Path.Combine(root, "adapter");
        string outputDirectory = Path.Combine(root, "output");
        Directory.CreateDirectory(projectDirectory);
        Directory.CreateDirectory(outputDirectory);

        try
        {
            string projectPath = Path.Combine(projectDirectory, "FakeAdapter.csproj");
            File.WriteAllText(projectPath,
                $"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <OutputType>Exe</OutputType>
                    <TargetFramework>net{Environment.Version.Major}.0</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                  </PropertyGroup>
                </Project>
                """);
            File.WriteAllText(Path.Combine(projectDirectory, "Program.cs"), FakeAdapterSource);
            await BuildFakeAdapterAsync(projectPath, outputDirectory);

            string adapterPath = Path.Combine(
                outputDirectory,
                OperatingSystem.IsWindows() ? "FakeAdapter.exe" : "FakeAdapter");
            var service = new NetCoreDbgDebugSessionService(adapterPath);
            string targetPath = typeof(NetCoreDbgDebugSessionService).Assembly.Location;
            TaskCompletionSource<DebugSessionEventArgs> continuedEvent = new(
                TaskCreationOptions.RunContinuationsAsynchronously);
            TaskCompletionSource<DebugSessionEventArgs> stoppedEvent = new(
                TaskCreationOptions.RunContinuationsAsynchronously);
            service.SessionEventReceived += (_, eventArgs) =>
            {
                if (eventArgs.EventName == "continued")
                {
                    continuedEvent.TrySetResult(eventArgs);
                }
                else if (eventArgs.EventName == "stopped")
                {
                    stoppedEvent.TrySetResult(eventArgs);
                }
            };

            DebugSessionInfo session = await service.StartSessionAsync(new DebugLaunchRequest
            {
                Target = targetPath,
                WorkingDirectory = root,
                StopAtEntry = true,
            });

            Assert.AreEqual(DebugSessionState.Paused, session.State);
            Assert.AreEqual(Path.GetFileName(targetPath), session.DisplayName);
            Assert.IsFalse(string.IsNullOrWhiteSpace(session.SessionId));

            await service.SetBreakpointsAsync(session.SessionId, targetPath, [1, 2]);
            await service.ContinueSessionAsync(session.SessionId);
            DebugSessionEventArgs continued = await continuedEvent.Task.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.AreEqual(DebugSessionState.Running, continued.State);
            await service.PauseSessionAsync(session.SessionId);
            DebugSessionEventArgs stopped = await stoppedEvent.Task.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.AreEqual(DebugSessionState.Paused, stopped.State);
            Assert.AreEqual("breakpoint", stopped.Reason);
            Assert.AreEqual(1, stopped.ThreadId);
            await service.StepSessionAsync(session.SessionId, DebugStepKind.Into);
            await service.StepSessionAsync(session.SessionId, DebugStepKind.Over);
            await service.StepSessionAsync(session.SessionId, DebugStepKind.Out);
            IReadOnlyList<DebugStackFrame> frames = await service.GetStackTraceAsync(session.SessionId);
            Assert.AreEqual(1, frames.Count);
            Assert.AreEqual("Main", frames[0].Name);
            Assert.AreEqual(2, frames[0].Line);
            IReadOnlyList<DebugVariable> variables = await service.GetVariablesAsync(session.SessionId, frames[0].Id);
            Assert.AreEqual(1, variables.Count);
            Assert.AreEqual("counter", variables[0].Name);
            Assert.AreEqual("3", variables[0].Value);
            await service.StopSessionAsync(session.SessionId);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task StartSession_RejectsMissingTargetsBeforeStartingAdapter()
    {
        var service = new NetCoreDbgDebugSessionService("missing-debug-adapter");

        await Assert.ThrowsExactlyAsync<FileNotFoundException>(() =>
            service.StartSessionAsync(new DebugLaunchRequest
            {
                Target = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".dll"),
            }));
    }

    [TestMethod]
    public async Task StartSession_ReportsUnsupportedAttachRequests()
    {
        var service = new NetCoreDbgDebugSessionService("missing-debug-adapter");

        await Assert.ThrowsExactlyAsync<NotSupportedException>(() =>
            service.StartSessionAsync(new DebugLaunchRequest
            {
                AttachToExistingProcess = true,
            }));
    }

    [TestMethod]
    public async Task SetBreakpoints_RejectsNonPositiveLines()
    {
        var service = new NetCoreDbgDebugSessionService("missing-debug-adapter");

        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() =>
            service.SetBreakpointsAsync("session", "source.cs", [0]));
    }

    private static async Task BuildFakeAdapterAsync(string projectPath, string outputDirectory)
    {
        ProcessStartInfo startInfo = new("dotnet")
        {
            WorkingDirectory = Path.GetDirectoryName(projectPath)!,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        startInfo.ArgumentList.Add("build");
        startInfo.ArgumentList.Add(projectPath);
        startInfo.ArgumentList.Add("--output");
        startInfo.ArgumentList.Add(outputDirectory);
        startInfo.ArgumentList.Add("--nologo");
        startInfo.Environment.Remove("MSBuildSDKsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath32");
        startInfo.Environment.Remove("MSBuildToolsPath");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR");

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("The fake DAP adapter build process could not be started.");
        Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> errorTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        string output = await outputTask;
        string error = await errorTask;
        Assert.AreEqual(0, process.ExitCode, $"{output}{Environment.NewLine}{error}");
    }

    private const string FakeAdapterSource =
        """
        using System.Text;
        using System.Text.Json;

        Stream input = Console.OpenStandardInput();
        Stream output = Console.OpenStandardOutput();
        int responseSequence = 0;

        while (true)
        {
            string? line = await ReadLineAsync(input);
            if (line is null)
            {
                break;
            }

            int contentLength = 0;
            while (line.Length > 0)
            {
                if (line.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
                {
                    contentLength = int.Parse(line["Content-Length:".Length..].Trim());
                }

                line = await ReadLineAsync(input) ?? throw new EndOfStreamException();
            }

            byte[] content = new byte[contentLength];
            await input.ReadExactlyAsync(content);
            using JsonDocument request = JsonDocument.Parse(content);
            JsonElement root = request.RootElement;
            string command = root.GetProperty("command").GetString()!;
            object? eventBody = command switch
            {
                "continue" => new { threadId = 1, allThreadsContinued = true },
                "pause" => new { reason = "breakpoint", threadId = 1, allThreadsStopped = true },
                _ => null,
            };
            if (eventBody is not null)
            {
                string eventName = command == "continue" ? "continued" : "stopped";
                byte[] eventMessage = JsonSerializer.SerializeToUtf8Bytes(new
                {
                    seq = ++responseSequence,
                    type = "event",
                    @event = eventName,
                    body = eventBody,
                });
                byte[] eventHeader = Encoding.ASCII.GetBytes($"Content-Length: {eventMessage.Length}\r\n\r\n");
                await output.WriteAsync(eventHeader);
                await output.WriteAsync(eventMessage);
                await output.FlushAsync();
            }

            if (command == "launch")
            {
                string program = root.GetProperty("arguments").GetProperty("program").GetString()!;
                if (!File.Exists(program))
                {
                    throw new FileNotFoundException("The launch request did not contain an existing program.", program);
                }

                if (!root.GetProperty("arguments").GetProperty("stopAtEntry").GetBoolean())
                {
                    throw new InvalidOperationException("The launch request did not stop at entry.");
                }
            }

            int requestSequence = root.GetProperty("seq").GetInt32();
            object? body = command switch
            {
                "threads" => new { threads = new[] { new { id = 1, name = "main" } } },
                "stackTrace" => new
                {
                    stackFrames = new[]
                    {
                        new
                        {
                            id = 42,
                            name = "Main",
                            line = 2,
                            column = 1,
                            source = new { path = "Fake.cs" },
                        },
                    },
                },
                "scopes" => new { scopes = new[] { new { name = "Locals", variablesReference = 99 } } },
                "variables" => new
                {
                    variables = new[]
                    {
                        new { name = "counter", value = "3", type = "int", evaluateName = "counter", variablesReference = 0 },
                    },
                },
                _ => null,
            };
            byte[] response = JsonSerializer.SerializeToUtf8Bytes(new
            {
                seq = ++responseSequence,
                type = "response",
                request_seq = requestSequence,
                success = true,
                command,
                body,
            });
            byte[] header = Encoding.ASCII.GetBytes($"Content-Length: {response.Length}\r\n\r\n");
            await output.WriteAsync(header);
            await output.WriteAsync(response);
            await output.FlushAsync();

            if (command == "disconnect")
            {
                break;
            }
        }

        static async Task<string?> ReadLineAsync(Stream stream)
        {
            using MemoryStream line = new();
            byte[] buffer = new byte[1];
            while (await stream.ReadAsync(buffer) > 0)
            {
                if (buffer[0] == (byte)'\n')
                {
                    byte[] bytes = line.ToArray();
                    int length = bytes.Length > 0 && bytes[^1] == (byte)'\r' ? bytes.Length - 1 : bytes.Length;
                    return Encoding.ASCII.GetString(bytes, 0, length);
                }

                line.WriteByte(buffer[0]);
            }

            return line.Length == 0 ? null : Encoding.ASCII.GetString(line.ToArray());
        }
        """;
}
