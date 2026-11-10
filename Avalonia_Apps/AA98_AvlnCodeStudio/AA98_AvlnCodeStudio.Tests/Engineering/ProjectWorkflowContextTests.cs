using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class ProjectWorkflowContextTests
{
    [TestMethod]
    public void Context_RequiresAnExistingWorkspaceProjectAndSafePropertyValues()
    {
        WithTemporaryDirectory(root =>
        {
            string projectPath = WriteProject(root, "App.csproj", "<TargetFramework>net8.0</TargetFramework>");
            string solutionPath = Path.Combine(root, "Projects.slnx");
            File.WriteAllText(solutionPath, "<Solution><Project Path=\"App.csproj\" /><Project Path=\"Missing.csproj\" /></Solution>");
            CSharpWorkspace workspace = new CSharpWorkspaceLoader().Load(solutionPath);

            ProjectWorkflowContext context = ProjectWorkflowContext.Create(workspace, projectPath, " Debug ", "net8.0");
            Assert.AreEqual("Debug", context.Configuration);
            Assert.AreEqual("net8.0", context.TargetFramework);
            Assert.ThrowsExactly<ArgumentException>(() =>
                ProjectWorkflowContext.Create(workspace, Path.Combine(root, "Other.csproj"), "Debug"));
            Assert.ThrowsExactly<FileNotFoundException>(() =>
                ProjectWorkflowContext.Create(workspace, Path.Combine(root, "Missing.csproj"), "Debug"));
            Assert.ThrowsExactly<ArgumentException>(() =>
                ProjectWorkflowContext.Create(workspace, projectPath, "Debug;Other=Injected"));
        });
    }

    [TestMethod]
    public async Task Inspection_UsesEvaluatedConfigurationFrameworkAndCustomOutput()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            File.WriteAllText(
                Path.Combine(root, "Directory.Build.props"),
                "<Project><PropertyGroup><BaseOutputPath>custom-output\\</BaseOutputPath></PropertyGroup></Project>");
            string path = WriteProject(root, "App.csproj",
                "<TargetFramework>net8.0</TargetFramework><OutputType>Exe</OutputType>"
                + "<AssemblyName>CustomApp</AssemblyName><Configurations>Debug;Release;Profile</Configurations>");
            CSharpWorkspace workspace = new CSharpWorkspaceLoader().Load(path);
            ProjectWorkflowContext context = ProjectWorkflowContext.Create(workspace, path, "Profile");

            ProjectTargetInspection result = await new DotNetProjectTargetResolver().InspectAsync(context);

            Assert.IsTrue(result.CanDebug, result.DebugUnavailableReason);
            Assert.AreEqual("net8.0", result.TargetFramework);
            Assert.IsTrue(result.Configurations.Contains("Profile"));
            Assert.AreEqual(
                Path.Combine(root, "custom-output", "Profile", "net8.0", "CustomApp.dll"),
                result.TargetPath);
            Assert.IsFalse(File.Exists(result.TargetPath));
        });
    }

    [TestMethod]
    public async Task Inspection_RequiresAnExplicitChoiceForMultipleFrameworks()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string path = WriteProject(root, "App.csproj",
                "<TargetFrameworks>net8.0;net9.0</TargetFrameworks><OutputType>Exe</OutputType>");
            CSharpWorkspace workspace = new CSharpWorkspaceLoader().Load(path);
            DotNetProjectTargetResolver resolver = new();

            ProjectTargetInspection ambiguous = await resolver.InspectAsync(
                ProjectWorkflowContext.Create(workspace, path, "Debug"));
            Assert.IsFalse(ambiguous.CanDebug);
            Assert.AreEqual(2, ambiguous.TargetFrameworks.Count);
            Assert.IsNull(ambiguous.TargetPath);

            ProjectTargetInspection selected = await resolver.InspectAsync(
                ProjectWorkflowContext.Create(workspace, path, "Debug", "net9.0"));
            Assert.IsTrue(selected.CanDebug, selected.DebugUnavailableReason);
            Assert.AreEqual("net9.0", selected.TargetFramework);
            Assert.AreEqual(Path.Combine(root, "bin", "Debug", "net9.0", "App.dll"), selected.TargetPath);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () =>
                await resolver.InspectAsync(ProjectWorkflowContext.Create(workspace, path, "Debug", "net10.0")));
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () =>
                await resolver.InspectAsync(ProjectWorkflowContext.Create(workspace, path, "Invalid")));
        });
    }

    [TestMethod]
    public async Task Inspection_ReportsLibraryAndHonorsCancellation()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string path = WriteProject(root, "Library.csproj", "<TargetFramework>net8.0</TargetFramework>");
            ProjectWorkflowContext context = ProjectWorkflowContext.Create(new CSharpWorkspaceLoader().Load(path), path, "Debug");
            DotNetProjectTargetResolver resolver = new();

            ProjectTargetInspection result = await resolver.InspectAsync(context);
            Assert.IsFalse(result.CanDebug);
            StringAssert.Contains(result.DebugUnavailableReason, "not executable");
            using CancellationTokenSource cancellation = new();
            cancellation.Cancel();
            await Assert.ThrowsExactlyAsync<OperationCanceledException>(async () =>
                await resolver.InspectAsync(context, cancellation.Token));
        });
    }

    private static string WriteProject(string root, string name, string properties)
    {
        string path = Path.Combine(root, name);
        File.WriteAllText(path, $"<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup>{properties}</PropertyGroup></Project>");
        return path;
    }

    private static void WithTemporaryDirectory(Action<string> action)
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98 Workflow {Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            action(root);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static async Task WithTemporaryDirectoryAsync(Func<string, Task> action)
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98 Workflow {Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            await action(root);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
