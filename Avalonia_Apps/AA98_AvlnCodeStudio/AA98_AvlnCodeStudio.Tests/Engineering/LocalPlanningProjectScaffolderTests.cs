using AA98_AvlnCodeStudio.Planning.Local.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class LocalPlanningProjectScaffolderTests
{
    [TestMethod]
    public void Create_GeneratesPortableSdkProjectAndPreservesMarkdownItems()
    {
        string projectRoot = Path.Combine(
            ResolveWorkspaceRoot(),
            "artifacts",
            OperatingSystem.IsWindows() ? "windows" : "linux",
            $"planning-scaffolder-test-{Guid.NewGuid():N}");

        try
        {
            LocalPlanningProjectScaffoldResult result = new LocalPlanningProjectScaffolder().Create(projectRoot);

            Assert.IsTrue(result.IsSuccessful, result.Message);
            Assert.AreEqual(Path.Combine(projectRoot, Path.GetFileName(projectRoot) + ".csproj"), result.ProjectFilePath);
            Assert.AreEqual(Path.Combine(projectRoot, Path.GetFileNameWithoutExtension(result.ProjectFilePath) + ".projitems"), result.ProjectItemsFilePath);
            Assert.IsTrue(File.Exists(result.ProjectFilePath));
            Assert.IsTrue(File.Exists(result.ProjectItemsFilePath));

            string project = File.ReadAllText(result.ProjectFilePath!);
            StringAssert.Contains(project, "<Project Sdk=\"Microsoft.NET.Sdk\">");
            StringAssert.Contains(project, "<TargetFramework>net8.0</TargetFramework>");
            StringAssert.Contains(project, "<EnableDefaultCompileItems>false</EnableDefaultCompileItems>");
            StringAssert.Contains(project, $"<Import Project=\"{Path.GetFileName(result.ProjectItemsFilePath)}\" Label=\"Shared\" />");
            Assert.IsFalse(project.Contains("CodeSharing", StringComparison.Ordinal));
            Assert.IsFalse(project.Contains("MSBuildExtensionsPath32", StringComparison.Ordinal));

            string projectItems = File.ReadAllText(result.ProjectItemsFilePath!);
            StringAssert.Contains(projectItems, "Include=\"$(MSBuildThisFileDirectory)**/*.md\"");
            StringAssert.Contains(projectItems, "bin/**;$(MSBuildThisFileDirectory)obj/**;$(MSBuildThisFileDirectory).vs/**");
            Assert.IsFalse(projectItems.Contains("CodeSharing", StringComparison.Ordinal));

            string fixturePath = Path.Combine(projectRoot, "Epics", "fixture.md");
            Directory.CreateDirectory(Path.Combine(projectRoot, "bin"));
            Directory.CreateDirectory(Path.Combine(projectRoot, "obj"));
            Directory.CreateDirectory(Path.Combine(projectRoot, ".vs"));
            File.WriteAllText(fixturePath, "# Included planning document");
            File.WriteAllText(Path.Combine(projectRoot, "bin", "ignored.md"), "# Ignored build output");
            File.WriteAllText(Path.Combine(projectRoot, "obj", "ignored.md"), "# Ignored intermediate output");
            File.WriteAllText(Path.Combine(projectRoot, ".vs", "ignored.md"), "# Ignored IDE output");

            string evaluatedItems = RunDotNet(projectRoot, "msbuild", result.ProjectFilePath!, "-getItem:None", "-property:TargetFramework=net10.0");
            StringAssert.Contains(evaluatedItems, "fixture.md");
            Assert.IsFalse(evaluatedItems.Contains("ignored.md", StringComparison.Ordinal));

            RunDotNet(projectRoot, "restore", result.ProjectFilePath!, "--verbosity", "quiet", "-p:TargetFramework=net10.0");
            RunDotNet(projectRoot, "build", result.ProjectFilePath!, "--no-restore", "--nologo", "--verbosity", "quiet", "-p:TargetFramework=net10.0");

            foreach (string folder in new[] { "Epics", "Features", "BacklogItems", "Tasks" })
            {
                Assert.IsTrue(Directory.Exists(Path.Combine(projectRoot, folder)));
            }

            string updatedProject = project + Environment.NewLine + "<!-- preserved -->";
            File.WriteAllText(result.ProjectFilePath!, updatedProject);
            LocalPlanningProjectScaffoldResult repeatedResult = new LocalPlanningProjectScaffolder().Create(projectRoot);
            Assert.AreEqual(updatedProject, File.ReadAllText(repeatedResult.ProjectFilePath!));
        }
        finally
        {
            if (Directory.Exists(projectRoot))
            {
                Directory.Delete(projectRoot, recursive: true);
            }
        }
    }

    private static string RunDotNet(string workingDirectory, params string[] arguments)
    {
        ProcessStartInfo startInfo = new("dotnet")
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("The .NET SDK process could not be started.");
        Task<string> standardOutput = process.StandardOutput.ReadToEndAsync();
        Task<string> standardError = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(180_000))
        {
            process.Kill(entireProcessTree: true);
            Assert.Fail($"The .NET SDK command timed out: dotnet {string.Join(" ", arguments)}");
        }

        string output = standardOutput.GetAwaiter().GetResult();
        string error = standardError.GetAwaiter().GetResult();
        Assert.AreEqual(0, process.ExitCode, $"dotnet {string.Join(" ", arguments)} failed.{Environment.NewLine}{output}{Environment.NewLine}{error}");
        return output + error;
    }

    private static string ResolveWorkspaceRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "AA98_AvlnCodeStudio.Tests", "AA98_AvlnCodeStudio.Tests.csproj")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("The AA98_AvlnCodeStudio workspace root could not be located.");
    }
}
