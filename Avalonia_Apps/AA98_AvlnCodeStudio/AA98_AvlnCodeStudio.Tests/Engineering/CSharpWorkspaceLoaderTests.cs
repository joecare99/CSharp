using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using AA98_AvlnCodeStudio.UI.Components;
using AA98_AvlnCodeStudio.UI.Services;
using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AvlnCodeStudio.Base.Building.Models;
using AA98_AvlnCodeStudio.Base.Building.Services;
using AA98_AvlnCodeStudio.Base.Versioning.Models;
using AA98_AvlnCodeStudio.Base.Versioning.Services;
using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using AA98_AvlnCodeStudio.Base.Debugging.Models;
using AA98_AvlnCodeStudio.Base.Debugging.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class CSharpWorkspaceLoaderTests
{
    [TestMethod]
    public void Load_ClassicalSolution_ResolvesCSharpProjectsAndIgnoresOtherProjectTypes()
    {
        WithTemporaryDirectory(root =>
        {
            string solutionPath = Path.Combine(root, "Sample.sln");
            string projectPath = Path.Combine(root, "src", "App.csproj");
            File.WriteAllText(solutionPath, string.Join(Environment.NewLine,
                "Microsoft Visual Studio Solution File, Format Version 12.00",
                "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"App\", \"src\\App.csproj\", \"{A}\"",
                "EndProject",
                "Project(\"{F2A71F9B-5D33-465A-A702-920D77279786}\") = \"Other\", \"src\\Other.fsproj\", \"{B}\"",
                "EndProject"));

            CSharpWorkspace workspace = new CSharpWorkspaceLoader().Load(solutionPath);

            Assert.AreEqual(Path.GetFullPath(solutionPath), workspace.EntryPath);
            Assert.AreEqual(root, workspace.RootPath);
            Assert.AreEqual(1, workspace.Projects.Count);
            Assert.AreEqual(Path.GetFullPath(projectPath), workspace.Projects[0].ProjectPath);
            Assert.AreEqual("App", workspace.Projects[0].Name);
        });
    }

    [TestMethod]
    public void Load_Slnx_ResolvesCSharpProjectsAndDeduplicatesEntries()
    {
        WithTemporaryDirectory(root =>
        {
            string solutionPath = Path.Combine(root, "Sample.slnx");
            File.WriteAllText(solutionPath,
                "<Solution><Project Path=\"src\\App.csproj\" /><Project Path=\"src/App.csproj\" /><Project Path=\"readme.md\" /></Solution>");

            CSharpWorkspace workspace = new CSharpWorkspaceLoader().Load(solutionPath);

            Assert.AreEqual(1, workspace.Projects.Count);
            Assert.AreEqual(Path.GetFullPath(Path.Combine(root, "src", "App.csproj")), workspace.Projects[0].ProjectPath);
        });
    }

    [TestMethod]
    public void Load_CSharpProject_UsesProjectAsSingleWorkspaceEntry()
    {
        WithTemporaryDirectory(root =>
        {
            string projectPath = Path.Combine(root, "App.csproj");
            File.WriteAllText(projectPath, "<Project />");

            CSharpWorkspace workspace = new CSharpWorkspaceLoader().Load(projectPath);

            Assert.AreEqual(Path.GetFullPath(projectPath), workspace.EntryPath);
            Assert.AreEqual(1, workspace.Projects.Count);
            Assert.AreEqual("App", workspace.Projects.Single().Name);
        });
    }

    [TestMethod]
    public async Task MainWindowViewModel_OpenWorkspaceFile_LoadsWorkspaceMetadata()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string solutionPath = Path.Combine(root, "Sample.slnx");
            File.WriteAllText(solutionPath, "<Solution><Project Path=\"src\\App.csproj\" /></Solution>");
            var editorComponent = Substitute.For<IAvaloniaEditorComponent>();
            editorComponent.EditorViewModel.Returns(new EditorViewModel());
            IVersionControlService versionControlService = Substitute.For<IVersionControlService>();
            versionControlService
                .GetStatusAsync(Arg.Any<VersionControlStatusRequest>(), default)
                .Returns(Task.FromResult(new VersionControlStatus
                {
                    RepositoryRootPath = root,
                    RepositoryName = "Sample",
                    ActiveReferenceName = "main",
                    IsRepositoryRootDiscovered = true,
                }));
            var viewModel = new MainWindowViewModel(
                editorComponent,
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(),
                null,
                null,
                new CSharpWorkspaceLoader(),
                new DesignWorkspaceFileDialogService(),
                null,
                versionControlService);

            await viewModel.OpenWorkspaceFileAsync(solutionPath);

            Assert.AreEqual(Path.GetFullPath(solutionPath), viewModel.Workspace?.EntryPath);
            Assert.AreEqual(1, viewModel.Workspace?.Projects.Count);
            StringAssert.Contains(viewModel.WorkspaceTitle, solutionPath);
            StringAssert.Contains(viewModel.VersionControlSummary, "main");
            await versionControlService.Received(1).GetStatusAsync(
                Arg.Is<VersionControlStatusRequest>(request => request.RepositoryContextPath == root),
                default);
        });
    }

    [TestMethod]
    public async Task MainWindowViewModel_BuildWorkspace_ForwardsProjectAndCollectsDiagnostics()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string solutionPath = Path.Combine(root, "Sample.slnx");
            string projectPath = Path.Combine(root, "src", "App.csproj");
            File.WriteAllText(solutionPath, "<Solution><Project Path=\"src\\App.csproj\" /></Solution>");
            ICodeStudioBuilderService builderService = Substitute.For<ICodeStudioBuilderService>();
            builderService
                .BuildProjectAsync(Arg.Any<BuilderBuildRequest>(), default)
                .Returns(Task.FromResult(new BuilderBuildResult
                {
                    ProjectPath = projectPath,
                    Succeeded = false,
                    Diagnostics =
                    {
                        new BuilderDiagnostic
                        {
                            Code = "CS1002",
                            Message = "Expected semicolon.",
                            Severity = BuilderDiagnosticSeverity.Error,
                            FilePath = Path.Combine(root, "src", "Program.cs"),
                            LineNumber = 4,
                            ColumnNumber = 12,
                        },
                    },
                }));
            var workflow = new EditorWorkflow(
                new AA98_AvlnCodeStudio.Model.Documents.FileEditorDocument(),
                new DesignEditorFileDialogService(),
                new DesignTextDocumentStorageService());
            var editorComponent = new AvaloniaEditorComponent(workflow, new EditorViewModel(workflow), null!);
            var diagnostics = new DiagnosticCollectionViewModel();
            var viewModel = new MainWindowViewModel(
                editorComponent,
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                diagnostics,
                null,
                null,
                new CSharpWorkspaceLoader(),
                new DesignWorkspaceFileDialogService(),
                builderService);
            await viewModel.OpenWorkspaceFileAsync(solutionPath);

            await viewModel.BuildWorkspaceAsync();

            await builderService.Received(1).BuildProjectAsync(
                Arg.Is<BuilderBuildRequest>(request =>
                    request.WorkspaceRootPath == root
                    && request.ProjectPath == projectPath),
                default);
            Assert.AreEqual(1, diagnostics.Items.Count);
            Assert.AreEqual("CS1002", diagnostics.Items[0].Code);
            Assert.AreEqual(AppKomponentBaseLib.Diagnostics.DiagnosticSeverity.Error, diagnostics.Items[0].Severity);
            Assert.AreEqual(4, diagnostics.Items[0].LineNumber);
        });
    }

    [TestMethod]
    public async Task MainWindowViewModel_RunWorkspaceTests_UsesWorkspaceEntryAndReportsSummary()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string solutionPath = Path.Combine(root, "Sample.slnx");
            File.WriteAllText(solutionPath, "<Solution><Project Path=\"src\\App.csproj\" /></Solution>");
            ITestExecutionService testExecutionService = Substitute.For<ITestExecutionService>();
            testExecutionService
                .RunTestsAsync(Arg.Any<TestRunRequest>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new TestRunSummary
                {
                    Outcome = TestRunOutcome.Passed,
                    TotalCount = 5,
                    PassedCount = 5,
                }));
            var workflow = new EditorWorkflow(
                new AA98_AvlnCodeStudio.Model.Documents.FileEditorDocument(),
                new DesignEditorFileDialogService(),
                new DesignTextDocumentStorageService());
            var viewModel = new MainWindowViewModel(
                new AvaloniaEditorComponent(workflow, new EditorViewModel(workflow), null!),
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(),
                null,
                null,
                new CSharpWorkspaceLoader(),
                new DesignWorkspaceFileDialogService(),
                testExecutionService: testExecutionService);
            await viewModel.OpenWorkspaceFileAsync(solutionPath);

            await viewModel.RunWorkspaceTestsCommand.ExecuteAsync(null);

            await testExecutionService.Received(1).RunTestsAsync(
                Arg.Is<TestRunRequest>(request =>
                    request.WorkspaceRootPath == root
                    && request.ProjectPath == Path.GetFullPath(solutionPath)),
                Arg.Any<CancellationToken>());
            StringAssert.Contains(viewModel.Editor.StatusText, "Passed");
            StringAssert.Contains(viewModel.Editor.NotificationText, "5 passed");
        });
    }

    [TestMethod]
    public async Task MainWindowViewModel_StartAndStopDebugSession_UsesConfiguredTargetAndWorkspaceDirectory()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string solutionPath = Path.Combine(root, "Sample.slnx");
            string projectPath = Path.Combine(root, "App.csproj");
            string targetPath = Path.Combine(root, "bin", "Debug", "net8.0", "App.dll");
            string sourcePath = Path.Combine(root, "Program.cs");
            File.WriteAllText(solutionPath, "<Solution><Project Path=\"App.csproj\" /></Solution>");
            File.WriteAllText(projectPath, "<Project />");
            File.WriteAllText(sourcePath, "class Program\n{\n}");
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
            File.WriteAllText(targetPath, string.Empty);

            IDebugSessionService debugSessionService = Substitute.For<IDebugSessionService>();
            debugSessionService
                .StartSessionAsync(Arg.Any<DebugLaunchRequest>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new DebugSessionInfo
                {
                    SessionId = "debug-session",
                    DisplayName = "App.dll",
                    State = DebugSessionState.Running,
                }));
            debugSessionService
                .GetStackTraceAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<DebugStackFrame>>(
                [
                    new DebugStackFrame { Id = 42, Name = "Main", Line = 3 },
                ]));
            debugSessionService
                .GetVariablesAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<DebugVariable>>(
                [
                    new DebugVariable { Name = "counter", Value = "3", Type = "int" },
                ]));
            var workflow = new EditorWorkflow(
                new AA98_AvlnCodeStudio.Model.Documents.FileEditorDocument(),
                new DesignEditorFileDialogService(),
                new DesignTextDocumentStorageService());
            var viewModel = new MainWindowViewModel(
                new AvaloniaEditorComponent(workflow, new EditorViewModel(workflow), null!),
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(),
                null,
                null,
                new CSharpWorkspaceLoader(),
                new DesignWorkspaceFileDialogService(),
                debugSessionService: debugSessionService);
            await viewModel.OpenWorkspaceFileAsync(solutionPath);
            viewModel.DebugTargetPath = targetPath;

            Assert.IsTrue(viewModel.StartDebugSessionCommand.CanExecute(null));
            await viewModel.StartDebugSessionCommand.ExecuteAsync(null);

            await debugSessionService.Received(1).StartSessionAsync(
                Arg.Is<DebugLaunchRequest>(request =>
                    request.Target == targetPath
                    && request.WorkingDirectory == root
                    && request.StopAtEntry),
                Arg.Any<CancellationToken>());
            Assert.IsTrue(viewModel.StopDebugSessionCommand.CanExecute(null));
            Assert.IsFalse(viewModel.ContinueDebugSessionCommand.CanExecute(null));
            Assert.IsTrue(viewModel.PauseDebugSessionCommand.CanExecute(null));
            await viewModel.Editor.OpenFileAsync(sourcePath);
            Assert.IsTrue(viewModel.ToggleBreakpointCommand.CanExecute(null));
            await viewModel.ToggleBreakpointCommand.ExecuteAsync(null);
            await debugSessionService.Received(1).SetBreakpointsAsync(
                "debug-session",
                sourcePath,
                Arg.Is<IReadOnlyList<int>>(lines => lines.Count == 1 && lines[0] == 1),
                Arg.Any<CancellationToken>());
            await viewModel.ToggleBreakpointCommand.ExecuteAsync(null);
            await debugSessionService.Received(1).SetBreakpointsAsync(
                "debug-session",
                sourcePath,
                Arg.Is<IReadOnlyList<int>>(lines => lines.Count == 0),
                Arg.Any<CancellationToken>());
            await viewModel.ContinueDebugSessionCommand.ExecuteAsync(null);
            await debugSessionService.Received(1).ContinueSessionAsync("debug-session", Arg.Any<CancellationToken>());
            Assert.AreEqual(DebugSessionState.Running, viewModel.ActiveDebugSessionState);
            Assert.IsFalse(viewModel.StepIntoCommand.CanExecute(null));
            await viewModel.PauseDebugSessionCommand.ExecuteAsync(null);
            await debugSessionService.Received(1).PauseSessionAsync("debug-session", Arg.Any<CancellationToken>());
            viewModel.ActiveDebugSessionState = DebugSessionState.Paused;
            Assert.IsTrue(viewModel.StepIntoCommand.CanExecute(null));
            await viewModel.StepIntoCommand.ExecuteAsync(null);
            await debugSessionService.Received(1).StepSessionAsync(
                "debug-session",
                DebugStepKind.Into,
                Arg.Any<CancellationToken>());
            viewModel.ActiveDebugSessionState = DebugSessionState.Paused;
            await viewModel.RefreshDebugStackCommand.ExecuteAsync(null);
            await debugSessionService.Received(1).GetStackTraceAsync("debug-session", Arg.Any<CancellationToken>());
            await debugSessionService.Received(1).GetVariablesAsync(
                "debug-session",
                42,
                Arg.Any<CancellationToken>());
            Assert.AreEqual("Main", viewModel.SelectedDebugStackFrame?.Name);
            Assert.AreEqual("counter", viewModel.DebugVariables.Single().Name);
            await viewModel.StopDebugSessionCommand.ExecuteAsync(null);
            await debugSessionService.Received(1).StopSessionAsync("debug-session", Arg.Any<CancellationToken>());
            Assert.IsFalse(viewModel.StopDebugSessionCommand.CanExecute(null));
        });
    }

    private static void WithTemporaryDirectory(Action<string> action)
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_CSharpWorkspace_{Guid.NewGuid():N}");
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
        string root = Path.Combine(Path.GetTempPath(), $"AA98_CSharpWorkspace_{Guid.NewGuid():N}");
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
