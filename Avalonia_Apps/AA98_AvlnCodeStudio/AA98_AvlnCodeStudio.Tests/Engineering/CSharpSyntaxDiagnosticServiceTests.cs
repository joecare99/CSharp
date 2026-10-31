using AA98_AvlnCodeStudio.UI.Services;
using AppKomponentBaseLib.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class CSharpSyntaxDiagnosticServiceTests
{
    [TestMethod]
    public void Analyze_InvalidCSharp_ReportsRoslynDiagnosticWithSourceLocation()
    {
        const string filePath = @"C:\workspace\Sample.cs";
        const string source = "class Sample\n{\n    void Run() { int value = 1 }\n}";

        Diagnostic diagnostic = new CSharpSyntaxDiagnosticService()
            .Analyze(source, filePath)
            .Single(item => item.Code == "CS1002");

        Assert.AreEqual(DiagnosticSeverity.Error, diagnostic.Severity);
        Assert.AreEqual(filePath, diagnostic.SourcePath);
        Assert.AreEqual(3, diagnostic.LineNumber);
        Assert.IsNotNull(diagnostic.ColumnNumber);
    }

    [TestMethod]
    public void Analyze_ValidCSharp_ReturnsNoSyntaxDiagnostics()
    {
        var diagnostics = new CSharpSyntaxDiagnosticService()
            .Analyze("class Sample { void Run() { int value = 1; } }", null);

        Assert.AreEqual(0, diagnostics.Count);
    }

    [TestMethod]
    public async Task AnalyzeAsync_ProjectWorkspaceReportsSemanticDiagnosticsForEditedFile()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_RoslynWorkspace_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string projectPath = Path.Combine(root, "Sample.csproj");
            string sourcePath = Path.Combine(root, "Program.cs");
            await File.WriteAllTextAsync(projectPath,
                $"<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net{Environment.Version.Major}.0</TargetFramework><ImplicitUsings>disable</ImplicitUsings></PropertyGroup></Project>");
            await File.WriteAllTextAsync(sourcePath,
                "public static class ReferencedType { public static int Value => 1; }");

            ProcessStartInfo restoreStartInfo = new("dotnet")
            {
                WorkingDirectory = AppContext.BaseDirectory,
                ArgumentList = { "restore", projectPath, "--ignore-failed-sources", "--verbosity", "quiet" },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            ClearInheritedMsBuildEnvironment(restoreStartInfo);
            using Process restore = Process.Start(restoreStartInfo)
                ?? throw new InvalidOperationException("Could not start dotnet restore for the semantic diagnostics test.");
            Task<string> standardOutput = restore.StandardOutput.ReadToEndAsync();
            Task<string> standardError = restore.StandardError.ReadToEndAsync();
            await restore.WaitForExitAsync();
            if (restore.ExitCode != 0)
            {
                Assert.Fail($"Temporary C# project restore failed: {await standardOutput}{await standardError}");
            }

            AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspace workspace =
                new AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspaceLoader().Load(projectPath);
            const string editedSource = """
                public static class ReferencedType { public static int Value => 1; }
                public class Consumer
                {
                    public string Read() => ReferencedType.Value;
                }
                """;

            var diagnostics = await new CSharpSyntaxDiagnosticService()
                .AnalyzeAsync(editedSource, sourcePath, workspace);

            Assert.IsTrue(diagnostics.Any(diagnostic =>
                diagnostic.Code == "CS0029"
                && diagnostic.SourcePath == sourcePath
                && diagnostic.Severity == DiagnosticSeverity.Error));
            Assert.IsFalse(diagnostics.Any(diagnostic => diagnostic.Code == "CS0103"));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task GetCompletionsAsync_ProjectWorkspaceProvidesMemberCompletions()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_RoslynCompletion_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string projectPath = Path.Combine(root, "Sample.csproj");
            string sourcePath = Path.Combine(root, "Program.cs");
            await File.WriteAllTextAsync(projectPath,
                $"<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net{Environment.Version.Major}.0</TargetFramework><ImplicitUsings>disable</ImplicitUsings></PropertyGroup></Project>");
            await File.WriteAllTextAsync(sourcePath, "public class Sample { }");

            ProcessStartInfo restoreStartInfo = new("dotnet")
            {
                WorkingDirectory = AppContext.BaseDirectory,
                ArgumentList = { "restore", projectPath, "--ignore-failed-sources", "--verbosity", "quiet" },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            ClearInheritedMsBuildEnvironment(restoreStartInfo);
            using Process restore = Process.Start(restoreStartInfo)
                ?? throw new InvalidOperationException("Could not start dotnet restore for the completion test.");
            Task<string> standardOutput = restore.StandardOutput.ReadToEndAsync();
            Task<string> standardError = restore.StandardError.ReadToEndAsync();
            await restore.WaitForExitAsync();
            if (restore.ExitCode != 0)
            {
                Assert.Fail($"Temporary C# project restore failed: {await standardOutput}{await standardError}");
            }

            AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspace workspace =
                new AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspaceLoader().Load(projectPath);
            const string source = "public class Sample { void Run() { System.Console.W } }";
            int position = source.IndexOf("System.Console.W", StringComparison.Ordinal) + "System.Console.W".Length;
            using CSharpSyntaxDiagnosticService service = new();

            IReadOnlyList<string> completions = await service.GetCompletionsAsync(
                source,
                sourcePath,
                position,
                workspace);

            CollectionAssert.Contains(completions.ToArray(), "WriteLine");
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static void ClearInheritedMsBuildEnvironment(ProcessStartInfo startInfo)
    {
        startInfo.Environment.Remove("MSBuildSDKsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath32");
        startInfo.Environment.Remove("MSBuildToolsPath");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR");
    }
}
