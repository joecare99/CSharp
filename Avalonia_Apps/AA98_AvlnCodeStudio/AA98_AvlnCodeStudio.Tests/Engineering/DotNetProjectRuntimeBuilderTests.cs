using AA98_AvlnCodeStudio.Base.Building.Models;
using AA98_AvlnCodeStudio.Base.Building.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class DotNetProjectRuntimeBuilderTests
{
    [TestMethod]
    [TestCategory("Integration")]
    public async Task BuildAsync_RefreshesRuntimeMetadataAfterAddingProjectReference()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_RuntimeBuilder_{Guid.NewGuid():N}");
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
                    public const string Value = "dependency-loaded";
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
            File.WriteAllText(Path.Combine(appDirectory, "Program.cs"), "Console.WriteLine(Library.LibraryMarker.Value);");
            await RunDotNetAsync(appDirectory, "restore", "App.csproj");
            Assert.IsFalse(File.ReadAllText(depsPath).Contains("Library.dll", StringComparison.Ordinal));

            await new DotNetProjectRuntimeBuilder().BuildAsync(new BuilderBuildRequest
            {
                ProjectPath = appProjectPath,
                Configuration = "Debug",
                TargetFramework = targetFramework,
            });

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
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"dotnet {string.Join(' ', arguments)} failed (exit code {process.ExitCode}):{Environment.NewLine}{output}{Environment.NewLine}{error}");
        }

        return output;
    }
}
