using AA98_AvlnCodeStudio.Base.Building.Models;
using AA98_AvlnCodeStudio.Base.Building.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#if NET10_0
using AA98.Builder.Host;
using NSubstitute;
#endif

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class DotNetProjectRuntimeBuilderTests
{
#if NET10_0
    [TestMethod]
    [TestCategory("Integration")]
    public async Task HostRefresh_WithRealSdkFailures_ReportsErrorWithoutSuccess()
    {
        string root = Path.Combine(AppContext.BaseDirectory, $"AA98_RuntimeFailure_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        string projectPath = Path.Combine(root, "Fixture.csproj");
        try
        {
            File.WriteAllText(projectPath, """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
                  <Target Name="FailFixture" BeforeTargets="CoreCompile">
                    <Error Text="OWNED_RUNTIME_REFRESH_FAILURE" />
                  </Target>
                </Project>
                """);
            File.WriteAllText(Path.Combine(root, "Marker.cs"), "public class Marker { }");

            ICodeStudioBuilderService builder = Substitute.For<ICodeStudioBuilderService>();
            builder.BuildProjectAsync(Arg.Any<BuilderBuildRequest>(), Arg.Any<CancellationToken>())
                .Returns(new BuilderBuildResult { ProjectPath = projectPath, Succeeded = true });
            using StringWriter missingOutput = new();
            using StringWriter missingError = new();
            int missingExit = await Program.RunAsync(
                ["build", projectPath, "--refresh-runtime"], builder, missingOutput, missingError);
            Assert.AreEqual(1, missingExit);
            Assert.AreEqual(string.Empty, missingOutput.ToString());
            StringAssert.Contains(missingError.ToString(), "AA98_BUILDER_HOST_ERROR");
            StringAssert.Contains(missingError.ToString(), "NETSDK1004");

            await RunDotNetAsync(root, "restore", projectPath);
            using StringWriter targetOutput = new();
            using StringWriter targetError = new();
            int targetExit = await Program.RunAsync(
                ["build", projectPath, "--refresh-runtime"], builder, targetOutput, targetError);
            Assert.AreEqual(1, targetExit);
            Assert.AreEqual(string.Empty, targetOutput.ToString());
            StringAssert.Contains(targetError.ToString(), "AA98_BUILDER_HOST_ERROR");
            StringAssert.Contains(targetError.ToString(), "OWNED_RUNTIME_REFRESH_FAILURE");
            StringAssert.Contains(targetError.ToString(), "exit code");

            string hostPath = Path.Combine(AppContext.BaseDirectory, "AA98.Builder.Host.dll");
            (int defaultExit, string defaultOutput, _) = await RunDotNetProcessAsync(root, hostPath, "build", projectPath);
            Assert.AreEqual(0, defaultExit);
            StringAssert.Contains(defaultOutput, "succeeded=True");
            Assert.IsFalse(defaultOutput.Contains("runtimeRefresh=Succeeded", StringComparison.Ordinal));

            (int refreshExit, string refreshOutput, string refreshError) =
                await RunDotNetProcessAsync(root, hostPath, "build", projectPath, "--refresh-runtime");
            Assert.AreEqual(1, refreshExit);
            Assert.AreEqual(string.Empty, refreshOutput);
            StringAssert.Contains(refreshError, "AA98_BUILDER_HOST_ERROR");
            StringAssert.Contains(refreshError, "OWNED_RUNTIME_REFRESH_FAILURE");
            Assert.IsFalse(refreshError.Contains("runtimeRefresh=Succeeded", StringComparison.Ordinal));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task HostRefresh_WhenRealSdkProcessIsCanceled_DoesNotReportSuccess()
    {
        if (!OperatingSystem.IsWindows())
        {
            Assert.Inconclusive("The owned cancellation fixture uses Windows ping.");
        }

        string root = Path.Combine(AppContext.BaseDirectory, $"AA98_RuntimeCancel_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        string projectPath = Path.Combine(root, "Fixture.csproj");
        string markerPath = Path.Combine(root, "started.txt");
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(45));
        Task<int>? run = null;
        try
        {
            File.WriteAllText(projectPath, """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
                  <Target Name="WaitFixture" BeforeTargets="CoreCompile">
                    <WriteLinesToFile File="started.txt" Lines="started" Overwrite="true" />
                    <Exec Command="ping 127.0.0.1 -n 31 &gt; nul" />
                  </Target>
                </Project>
                """);
            await RunDotNetAsync(root, "restore", projectPath);
            ICodeStudioBuilderService builder = Substitute.For<ICodeStudioBuilderService>();
            builder.BuildProjectAsync(Arg.Any<BuilderBuildRequest>(), Arg.Any<CancellationToken>())
                .Returns(new BuilderBuildResult { ProjectPath = projectPath, Succeeded = true });
            using StringWriter output = new();
            using StringWriter error = new();
            run = Program.RunAsync(["build", projectPath, "--refresh-runtime"], builder, output, error, cancellation.Token);
            while (!File.Exists(markerPath) && !run.IsCompleted && !cancellation.IsCancellationRequested)
            {
                await Task.Delay(100);
            }

            Assert.IsTrue(File.Exists(markerPath), "The SDK target did not start.");
            cancellation.Cancel();
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await run);
            Assert.AreEqual(string.Empty, output.ToString());
            Assert.AreEqual(string.Empty, error.ToString());
        }
        finally
        {
            cancellation.Cancel();
            if (run is not null)
            {
                try { await run; }
                catch (OperationCanceledException) { }
            }

            Directory.Delete(root, recursive: true);
        }
    }
#endif

    [TestMethod]
    [DataRow(false)]
#if NET10_0
    [DataRow(true)]
#endif
    [TestCategory("Integration")]
    public async Task BuildAsync_RefreshesRuntimeMetadataAfterAddingProjectReference(bool useHostRefresh)
    {
        string root = Path.Combine(AppContext.BaseDirectory, $"AA98_RuntimeBuilder_{Guid.NewGuid():N}");
        string appDirectory = Path.Combine(root, "App");
        string libraryDirectory = Path.Combine(root, "Library");
        Directory.CreateDirectory(appDirectory);
        Directory.CreateDirectory(libraryDirectory);

        try
        {
            string targetFramework = $"net{Environment.Version.Major}.0";
            string appProjectPath = Path.Combine(appDirectory, "App.csproj");
            string libraryProjectPath = Path.Combine(libraryDirectory, "Library.csproj");
            string appOutputDirectory = Path.Combine(appDirectory, "bin", "Debug", targetFramework);
            File.WriteAllText(
                appProjectPath,
                $"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <OutputType>Exe</OutputType>
                    <TargetFramework>{targetFramework}</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                  </PropertyGroup>
                </Project>
                """);
            File.WriteAllText(Path.Combine(appDirectory, "Program.cs"), "Console.WriteLine(\"before\");");

            await RunDotNetAsync(appDirectory, "restore", "App.csproj");
            await RunDotNetAsync(appDirectory, "build", "App.csproj", "--no-restore", "--configuration", "Debug");
            string depsPath = Path.Combine(appOutputDirectory, "App.deps.json");
            Assert.IsFalse(File.ReadAllText(depsPath).Contains("Library.dll", StringComparison.Ordinal));

            File.WriteAllText(
                libraryProjectPath,
                $"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <TargetFramework>{targetFramework}</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                  </PropertyGroup>
                </Project>
                """);
            File.WriteAllText(
                Path.Combine(libraryDirectory, "LibraryMarker.cs"),
                """
                namespace Library;
                public static class LibraryMarker
                {
                    public static string Value() => "dependency-loaded";
                }
                """);
            File.WriteAllText(
                appProjectPath,
                $"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <OutputType>Exe</OutputType>
                    <TargetFramework>{targetFramework}</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                  </PropertyGroup>
                  <ItemGroup>
                    <ProjectReference Include="..\Library\Library.csproj" />
                  </ItemGroup>
                </Project>
                """);
            File.WriteAllText(Path.Combine(appDirectory, "Program.cs"), "Console.WriteLine(Library.LibraryMarker.Value());");
            await RunDotNetAsync(appDirectory, "restore", "App.csproj");
            Assert.IsFalse(File.ReadAllText(depsPath).Contains("Library.dll", StringComparison.Ordinal));

#if NET10_0
            await RunDotNetAsync(libraryDirectory, "build", "Library.csproj", "--no-restore", "--configuration", "Debug");
            string hostPath = Path.Combine(AppContext.BaseDirectory, "AA98.Builder.Host.dll");
            if (!useHostRefresh)
            {
                string hostOutput = await RunDotNetAsync(appDirectory, hostPath, "build", appProjectPath, "--configuration", "Debug", "--framework", targetFramework);
                StringAssert.Contains(hostOutput, "succeeded=True");
            }
#endif

#if NET10_0
            if (useHostRefresh)
            {
                string refreshOutput = await RunDotNetAsync(
                    appDirectory, hostPath, "build", appProjectPath, "--configuration", "Debug",
                    "--framework", targetFramework, "--refresh-runtime");
                StringAssert.Contains(refreshOutput, "runtimeRefresh=Succeeded");
            }
            else
#endif
            {
                await new DotNetProjectRuntimeBuilder().BuildAsync(new BuilderBuildRequest
                {
                    ProjectPath = appProjectPath,
                    Configuration = "Debug",
                    TargetFramework = targetFramework,
                });
            }

            StringAssert.Contains(File.ReadAllText(depsPath), "Library.dll");
            string output = await RunDotNetAsync(appDirectory, Path.Combine(appOutputDirectory, "App.dll"));
            Assert.AreEqual("dependency-loaded", output.Trim());
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static async Task<string> RunDotNetAsync(string workingDirectory, params string[] arguments)
    {
        (int exitCode, string output, string error) = await RunDotNetProcessAsync(workingDirectory, arguments);
        if (exitCode != 0)
        {
            throw new InvalidOperationException(
                $"dotnet {string.Join(' ', arguments)} failed (exit code {exitCode}):{Environment.NewLine}{output}{Environment.NewLine}{error}");
        }

        return output;
    }

    private static async Task<(int ExitCode, string Output, string Error)> RunDotNetProcessAsync(
        string workingDirectory, params string[] arguments)
    {
        ProcessStartInfo startInfo = new("dotnet")
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        startInfo.Environment.Remove("MSBuildSDKsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath32");
        startInfo.Environment.Remove("MSBuildToolsPath");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR");
        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("The .NET SDK process could not be started.");
        Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> errorTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        string output = await outputTask;
        string error = await errorTask;
        return (process.ExitCode, output, error);
    }
}
