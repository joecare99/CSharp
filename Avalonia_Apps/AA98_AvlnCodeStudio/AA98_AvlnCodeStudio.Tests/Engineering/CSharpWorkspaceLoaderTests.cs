using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.Editor.Components;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Editor.Navigation;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using AA98_AvlnCodeStudio.UI.Components;
using AA98_AvlnCodeStudio.UI.Navigation;
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
using Avalonia.Controls;
using Code.Navigation;
using Diagnostics.Navigation;

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
    public async Task MainWindowViewModel_CreateConsoleProject_CreatesAndOpensSelectedProject()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string projectPath = Path.Combine(root, "NewApp.csproj");
            IWorkspaceFileDialogService workspaceFileDialog = Substitute.For<IWorkspaceFileDialogService>();
            workspaceFileDialog
                .ShowSaveCSharpProjectDialogAsync(
                    Arg.Any<string?>(),
                    "ConsoleApp.csproj",
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<string?>(projectPath));
            var viewModel = new MainWindowViewModel(
                new AvaloniaEditorComponent(
                    new EditorWorkflow(
                        new AA98_AvlnCodeStudio.Model.Documents.FileEditorDocument(),
                        new DesignEditorFileDialogService(),
                        new DesignTextDocumentStorageService()),
                    new EditorViewModel(),
                    null!),
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(),
                null,
                null,
                workspaceFileDialogService: workspaceFileDialog);

            await viewModel.CreateConsoleProjectCommand.ExecuteAsync(null);

            Assert.AreEqual(Path.GetFullPath(projectPath), viewModel.Workspace?.EntryPath);
            Assert.IsTrue(File.Exists(projectPath));
            Assert.IsTrue(File.Exists(Path.Combine(root, "Program.cs")));
            StringAssert.Contains(viewModel.Editor.StatusText, "Created Console Application");
            await workspaceFileDialog.Received(1).ShowSaveCSharpProjectDialogAsync(
                Arg.Any<string?>(),
                "ConsoleApp.csproj",
                Arg.Any<CancellationToken>());
        });
    }

    [TestMethod]
    public async Task MainWindowViewModel_CreatesProjectFromReloadedJsonTemplate()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string templatesDirectory = Path.Combine(root, "templates");
            Directory.CreateDirectory(templatesDirectory);
            string projectPath = Path.Combine(root, "Custom.csproj");
            CSharpProjectTemplateService templateService = new([templatesDirectory]);
            templateService.SaveTemplate(new CSharpProjectTemplateDefinition
            {
                Id = "custom-workbench",
                Name = "Workbench Custom",
                Description = "Created from an external template.",
                DefaultProjectFileName = "Starter.csproj",
                ProjectFileContent = "<Project />",
            }, Path.Combine(templatesDirectory, "custom.json"));

            IWorkspaceFileDialogService workspaceFileDialog = Substitute.For<IWorkspaceFileDialogService>();
            workspaceFileDialog
                .ShowSaveCSharpProjectDialogAsync(
                    Arg.Any<string?>(),
                    "Starter.csproj",
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<string?>(projectPath));
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
                workspaceFileDialogService: workspaceFileDialog,
                projectTemplateService: templateService);

            Assert.IsTrue(viewModel.ProjectTemplates.Any(template => template.Id == "custom-workbench"));
            viewModel.SelectedProjectTemplate = viewModel.ProjectTemplates.Single(template => template.Id == "custom-workbench");
            await viewModel.CreateSelectedProjectCommand.ExecuteAsync(null);

            Assert.AreEqual(Path.GetFullPath(projectPath), viewModel.Workspace?.EntryPath);
            Assert.IsTrue(File.Exists(projectPath));
            StringAssert.Contains(viewModel.Editor.StatusText, "Created Workbench Custom");
        });
    }

    [TestMethod]
    public async Task MainWindowViewModel_ManagesNuGetPackageForSelectedWorkspaceProject()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string projectPath = Path.Combine(root, "App.csproj");
            string solutionPath = Path.Combine(root, "Sample.slnx");
            File.WriteAllText(projectPath, "<Project />");
            File.WriteAllText(solutionPath, "<Solution><Project Path=\"App.csproj\" /></Solution>");
            ICSharpProjectPackageService packageService = Substitute.For<ICSharpProjectPackageService>();
            packageService
                .AddPackageAsync(projectPath, "Example.Package", "2.0.0", Arg.Any<CancellationToken>())
                .Returns(Task.FromResult("Package added."));
            packageService
                .RemovePackageAsync(projectPath, "Example.Package", Arg.Any<CancellationToken>())
                .Returns(Task.FromResult("Package removed."));
            packageService
                .ListPackagesAsync(projectPath, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<CSharpProjectPackageReference>>([]));
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
                projectPackageService: packageService);
            await viewModel.OpenWorkspaceFileAsync(solutionPath);
            viewModel.PackageId = "Example.Package";
            viewModel.PackageVersion = "2.0.0";

            Assert.AreEqual("App", viewModel.SelectedPackageProject?.Name);
            Assert.IsTrue(viewModel.AddPackageCommand.CanExecute(null));
            await viewModel.AddPackageCommand.ExecuteAsync(null);

            await packageService.Received(1).AddPackageAsync(
                projectPath,
                "Example.Package",
                "2.0.0",
                Arg.Any<CancellationToken>());
            Assert.AreEqual("Package added.", viewModel.Editor.NotificationText);
            await viewModel.RemovePackageCommand.ExecuteAsync(null);
            await packageService.Received(1).RemovePackageAsync(
                projectPath,
                "Example.Package",
                Arg.Any<CancellationToken>());
            Assert.AreEqual("Package removed.", viewModel.Editor.NotificationText);
        });
    }

    [TestMethod]
    public async Task MainWindowViewModel_SearchesNuGetAndLoadsInstalledPackages()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string projectPath = Path.Combine(root, "App.csproj");
            string solutionPath = Path.Combine(root, "Sample.slnx");
            File.WriteAllText(projectPath, "<Project />");
            File.WriteAllText(solutionPath, "<Solution><Project Path=\"App.csproj\" /></Solution>");
            ICSharpProjectPackageService packageService = Substitute.For<ICSharpProjectPackageService>();
            packageService
                .SearchPackagesAsync("avalonia", 20, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<NuGetPackageSearchResult>>(
                    [new("Avalonia", "12.1.4", "Cross-platform UI framework.")]));
            packageService
                .ListPackagesAsync(projectPath, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<CSharpProjectPackageReference>>(
                    [new("Avalonia", "12.1.4", "12.1.4", "net10.0")]));
            var workflow = new EditorWorkflow(
                new AA98_AvlnCodeStudio.Model.Documents.FileEditorDocument(),
                new DesignEditorFileDialogService(),
                new DesignTextDocumentStorageService());
            MainWindowViewModel viewModel = new(
                new AvaloniaEditorComponent(workflow, new EditorViewModel(workflow), null!),
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(),
                null,
                null,
                projectPackageService: packageService);
            await viewModel.OpenWorkspaceFileAsync(solutionPath);
            viewModel.PackageSearchText = "avalonia";

            await viewModel.SearchPackagesCommand.ExecuteAsync(null);

            Assert.AreEqual(1, viewModel.PackageSearchResults.Count);
            Assert.AreEqual("Avalonia", viewModel.PackageId);
            Assert.AreEqual("12.1.4", viewModel.PackageVersion);
            Assert.AreEqual("Found 1 NuGet packages.", viewModel.Editor.StatusText);

            await viewModel.RefreshPackagesCommand.ExecuteAsync(null);

            Assert.AreEqual(1, viewModel.InstalledPackages.Count);
            Assert.AreEqual("Avalonia", viewModel.SelectedInstalledPackage?.Id);
            StringAssert.Contains(viewModel.Editor.NotificationText, "net10.0");
            await packageService.Received(1).SearchPackagesAsync(
                "avalonia",
                20,
                Arg.Any<CancellationToken>());
            await packageService.Received(1).ListPackagesAsync(projectPath, Arg.Any<CancellationToken>());
        });
    }

    [TestMethod]
    public async Task MainWindowViewModel_PreviewsActiveAxamlDocumentAndReportsMarkupErrors()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string axamlPath = Path.Combine(root, "View.axaml");
            File.WriteAllText(
                axamlPath,
                "<UserControl xmlns='https://github.com/avaloniaui'><StackPanel><TextBlock Text='Preview me' /></StackPanel></UserControl>");
            var workflow = new EditorWorkflow(
                new AA98_AvlnCodeStudio.Model.Documents.FileEditorDocument(),
                new DesignEditorFileDialogService(),
                new FileSystemTextDocumentStorageService());
            var viewModel = new MainWindowViewModel(
                new AvaloniaEditorComponent(workflow, new EditorViewModel(workflow), new Border()),
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(),
                null,
                null);

            await viewModel.Editor.OpenFileAsync(axamlPath);

            Assert.IsTrue(viewModel.HasAxamlPreview);
            Assert.AreEqual("StackPanel", viewModel.AxamlDesigner.PreviewRoot?.TypeName);
            Assert.AreEqual("Preview me", viewModel.AxamlDesigner.PreviewRoot?.Children[0].Text);

            viewModel.Editor.Text = "<StackPanel>";

            Assert.IsTrue(viewModel.HasAxamlPreview);
            Assert.IsNull(viewModel.AxamlDesigner.PreviewRoot);
            Assert.IsFalse(string.IsNullOrWhiteSpace(viewModel.AxamlDesigner.ErrorText));
        });
    }

    [TestMethod]
    public async Task MainWindowViewModel_AppliesSelectedPreviewTextBackToActiveAxamlDocument()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string axamlPath = Path.Combine(root, "View.axaml");
            File.WriteAllText(
                axamlPath,
                "<UserControl xmlns='https://github.com/avaloniaui'><StackPanel><TextBlock Text='Before' /></StackPanel></UserControl>");
            var workflow = new EditorWorkflow(
                new AA98_AvlnCodeStudio.Model.Documents.FileEditorDocument(),
                new DesignEditorFileDialogService(),
                new FileSystemTextDocumentStorageService());
            var viewModel = new MainWindowViewModel(
                new AvaloniaEditorComponent(workflow, new EditorViewModel(workflow), new Border()),
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(),
                null,
                null);

            await viewModel.Editor.OpenFileAsync(axamlPath);
            viewModel.AxamlDesigner.SelectedNode = viewModel.AxamlDesigner.PreviewRoot?.Children[0];
            viewModel.AxamlDesigner.SelectedProperty = "Text";
            viewModel.AxamlDesigner.SelectedValue = "After & updated";
            viewModel.AxamlDesigner.ApplySelectedValueCommand.Execute(null);

            StringAssert.Contains(viewModel.Editor.Text, "Text='After &amp; updated'");
            Assert.AreEqual("After & updated", viewModel.AxamlDesigner.PreviewRoot?.Children[0].Text);

            viewModel.AxamlDesigner.UndoCommand.Execute(null);
            Assert.IsTrue(viewModel.Editor.Text.Contains("Text='Before'", StringComparison.Ordinal));
            viewModel.AxamlDesigner.RedoCommand.Execute(null);
            Assert.IsTrue(viewModel.Editor.Text.Contains("Text='After &amp; updated'", StringComparison.Ordinal));
        });
    }

    [TestMethod]
    public async Task MainWindowViewModel_CreateLibraryAndTestProjects_OpensEachSelectedProject()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string libraryDirectory = Path.Combine(root, "library");
            string testDirectory = Path.Combine(root, "tests");
            Directory.CreateDirectory(libraryDirectory);
            Directory.CreateDirectory(testDirectory);
            string libraryPath = Path.Combine(libraryDirectory, "Shared.csproj");
            string testPath = Path.Combine(testDirectory, "Shared.Tests.csproj");
            IWorkspaceFileDialogService workspaceFileDialog = Substitute.For<IWorkspaceFileDialogService>();
            workspaceFileDialog
                .ShowSaveCSharpProjectDialogAsync(
                    Arg.Any<string?>(),
                    Arg.Any<string?>(),
                    Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromResult<string?>(libraryPath),
                    Task.FromResult<string?>(testPath));
            var viewModel = new MainWindowViewModel(
                new AvaloniaEditorComponent(
                    new EditorWorkflow(
                        new AA98_AvlnCodeStudio.Model.Documents.FileEditorDocument(),
                        new DesignEditorFileDialogService(),
                        new DesignTextDocumentStorageService()),
                    new EditorViewModel(),
                    null!),
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(),
                null,
                null,
                workspaceFileDialogService: workspaceFileDialog);

            await viewModel.CreateClassLibraryProjectCommand.ExecuteAsync(null);
            Assert.AreEqual(Path.GetFullPath(libraryPath), viewModel.Workspace?.EntryPath);
            Assert.IsTrue(File.Exists(Path.Combine(libraryDirectory, "Class1.cs")));

            await viewModel.CreateTestProjectCommand.ExecuteAsync(null);
            Assert.AreEqual(Path.GetFullPath(testPath), viewModel.Workspace?.EntryPath);
            Assert.IsTrue(File.Exists(Path.Combine(testDirectory, "SampleTests.cs")));
            StringAssert.Contains(File.ReadAllText(testPath), "<IsTestProject>true</IsTestProject>");
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

    [TestMethod]
    public async Task WorkbenchReferenceWorkflow_ConnectsWorkspaceEditorBuildTestsDiagnosticsGitAndDebugging()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string solutionPath = Path.Combine(root, "Sample.slnx");
            string projectPath = Path.Combine(root, "project", "App.csproj");
            string sourcePath = Path.Combine(root, "src", "Program.cs");
            string notesPath = Path.Combine(root, "notes.md");
            string targetPath = Path.Combine(root, "project", "bin", "Debug", "net10.0", "App.dll");
            const string originalSource = "class Program\n{\n    static void Main() {}\n}\n";
            Directory.CreateDirectory(Path.GetDirectoryName(projectPath)!);
            Directory.CreateDirectory(Path.GetDirectoryName(sourcePath)!);
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
            File.WriteAllText(solutionPath, "<Solution><Project Path=\"project\\App.csproj\" /></Solution>");
            File.WriteAllText(projectPath, "<Project />");
            File.WriteAllText(sourcePath, originalSource);
            File.WriteAllText(notesPath, "# Notes");
            File.WriteAllText(targetPath, string.Empty);

            var editorFactory = new TestEditorComponentFactory();
            var editorWorkspace = new EditorDocumentWorkspace(editorFactory.Create(), editorFactory);
            IEditorCaretService caretService = Substitute.For<IEditorCaretService>();
            ICodeNavigator navigator = new EditorCodeNavigator(
                new WorkbenchEditorDocumentHost(() => editorWorkspace),
                caretService);
            var diagnostics = new DiagnosticCollectionViewModel(new DiagnosticLocationMapper(), navigator);
            ICodeStudioBuilderService builderService = Substitute.For<ICodeStudioBuilderService>();
            builderService
                .BuildProjectAsync(Arg.Any<BuilderBuildRequest>(), Arg.Any<CancellationToken>())
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
                            FilePath = sourcePath,
                            LineNumber = 3,
                            ColumnNumber = 5,
                        },
                    },
                }));

            ITestExecutionService testExecutionService = Substitute.For<ITestExecutionService>();
            testExecutionService
                .RunTestsAsync(Arg.Any<TestRunRequest>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new TestRunSummary
                {
                    Outcome = TestRunOutcome.Passed,
                    TotalCount = 2,
                    PassedCount = 2,
                }));

            IVersionControlService versionControlService = Substitute.For<IVersionControlService>();
            versionControlService
                .GetStatusAsync(Arg.Any<VersionControlStatusRequest>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new VersionControlStatus
                {
                    RepositoryRootPath = root,
                    RepositoryName = "Sample",
                    ActiveReferenceName = "main",
                    IsRepositoryRootDiscovered = true,
                }));

            var debugSessionService = new FakeDebugSessionService(sourcePath);
            var viewModel = new MainWindowViewModel(
                editorWorkspace.ActiveTab.Component,
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                diagnostics,
                null,
                null,
                new CSharpWorkspaceLoader(),
                new DesignWorkspaceFileDialogService(),
                builderService,
                versionControlService,
                editorWorkspace,
                testExecutionService: testExecutionService,
                debugSessionService: debugSessionService);

            await viewModel.OpenWorkspaceFileAsync(solutionPath);
            Assert.AreEqual(Path.GetFullPath(solutionPath), viewModel.Workspace?.EntryPath);
            StringAssert.Contains(viewModel.VersionControlSummary, "main");
            await versionControlService.Received(1).GetStatusAsync(
                Arg.Is<VersionControlStatusRequest>(request => request.RepositoryContextPath == root),
                Arg.Any<CancellationToken>());

            await editorWorkspace.OpenOrActivateAsync(sourcePath);
            viewModel.Editor.Text = originalSource.Replace("Main() {}", "Main() { int value = 1; }", StringComparison.Ordinal);
            Assert.IsTrue(viewModel.Editor.IsDirty);

            await viewModel.BuildWorkspaceAsync();
            await builderService.Received(1).BuildProjectAsync(
                Arg.Is<BuilderBuildRequest>(request =>
                    request.WorkspaceRootPath == root
                    && request.ProjectPath == projectPath),
                Arg.Any<CancellationToken>());
            Assert.AreEqual(1, diagnostics.Items.Count);
            Assert.AreEqual("CS1002", diagnostics.Items[0].Code);
            await editorWorkspace.OpenOrActivateAsync(notesPath);
            Assert.AreEqual(Path.GetFullPath(notesPath), viewModel.ActiveEditorTab.FilePath);
            await diagnostics.ActivateAsync(diagnostics.Items[0]);
            await caretService.Received(1).FocusAndMoveCaretAsync(
                Arg.Is<IEditorDocument>(document => document.FilePath == Path.GetFullPath(sourcePath)),
                Arg.Is<CodeLocation>(location =>
                    location.Path == Path.GetFullPath(sourcePath)
                    && location.Line == 3
                    && location.Column == 5),
                Arg.Any<CancellationToken>());
            Assert.AreEqual(Path.GetFullPath(sourcePath), viewModel.ActiveEditorTab.FilePath);

            await viewModel.RunWorkspaceTestsCommand.ExecuteAsync(null);
            await testExecutionService.Received(1).RunTestsAsync(
                Arg.Is<TestRunRequest>(request =>
                    request.WorkspaceRootPath == root
                    && request.ProjectPath == Path.GetFullPath(solutionPath)),
                Arg.Any<CancellationToken>());
            StringAssert.Contains(viewModel.Editor.NotificationText, "2 passed");

            viewModel.DebugTargetPath = targetPath;
            Assert.IsTrue(viewModel.StartDebugSessionCommand.CanExecute(null));
            await viewModel.StartDebugSessionCommand.ExecuteAsync(null);
            Assert.AreEqual(targetPath, debugSessionService.LaunchRequest?.Target);
            Assert.AreEqual(root, debugSessionService.LaunchRequest?.WorkingDirectory);
            Assert.IsTrue(debugSessionService.LaunchRequest?.StopAtEntry);
            await viewModel.RefreshDebugStackCommand.ExecuteAsync(null);
            Assert.AreEqual(DebugSessionState.Paused, viewModel.ActiveDebugSessionState);
            Assert.AreEqual("Main", viewModel.SelectedDebugStackFrame?.Name);
            Assert.AreEqual("counter", viewModel.DebugVariables.Single().Name);
            await viewModel.ToggleBreakpointCommand.ExecuteAsync(null);
            Assert.AreEqual(Path.GetFullPath(sourcePath), debugSessionService.BreakpointSourcePath);
            CollectionAssert.AreEqual(new[] { 1 }, debugSessionService.BreakpointLines.ToArray());
            await viewModel.ContinueDebugSessionCommand.ExecuteAsync(null);
            Assert.AreEqual(1, debugSessionService.ContinueCount);
            await viewModel.PauseDebugSessionCommand.ExecuteAsync(null);
            Assert.AreEqual(1, debugSessionService.PauseCount);
            await viewModel.StopDebugSessionCommand.ExecuteAsync(null);
            Assert.AreEqual(1, debugSessionService.StopCount);
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

    private sealed class TestEditorComponentFactory : IAvaloniaEditorComponentFactory
    {
        public IAvaloniaEditorComponent Create()
        {
            var workflow = new EditorWorkflow(
                new AA98_AvlnCodeStudio.Model.Documents.FileEditorDocument(),
                new DesignEditorFileDialogService(),
                new FileSystemTextDocumentStorageService());
            var editorViewModel = new EditorViewModel(workflow);
            return new AvaloniaEditorComponent(
                workflow,
                editorViewModel,
                new TextBlock());
        }

        IEditorComponent IEditorComponentFactory.Create() => Create();
    }

    private sealed class FakeDebugSessionService : IDebugSessionService
    {
        private readonly string _sourcePath;

        public FakeDebugSessionService(string sourcePath)
        {
            _sourcePath = sourcePath;
        }

        public event EventHandler<DebugSessionEventArgs>? SessionEventReceived
        {
            add { }
            remove { }
        }

        public string? BreakpointSourcePath { get; private set; }

        public IReadOnlyList<int> BreakpointLines { get; private set; } = [];

        public int ContinueCount { get; private set; }

        public int PauseCount { get; private set; }

        public int StopCount { get; private set; }

        public DebugLaunchRequest? LaunchRequest { get; private set; }

        public Task<DebugSessionInfo> StartSessionAsync(
            DebugLaunchRequest request,
            CancellationToken cancellationToken = default)
        {
            LaunchRequest = request;
            return Task.FromResult(new DebugSessionInfo
            {
                SessionId = "reference-workflow-session",
                DisplayName = Path.GetFileName(request.Target),
                State = DebugSessionState.Paused,
            });
        }

        public Task SetBreakpointsAsync(
            string sessionId,
            string sourcePath,
            IReadOnlyList<int> lines,
            CancellationToken cancellationToken = default)
        {
            BreakpointSourcePath = sourcePath;
            BreakpointLines = lines;
            return Task.CompletedTask;
        }

        public Task ContinueSessionAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            ContinueCount++;
            return Task.CompletedTask;
        }

        public Task PauseSessionAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            PauseCount++;
            return Task.CompletedTask;
        }

        public Task StepSessionAsync(
            string sessionId,
            DebugStepKind stepKind,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<DebugStackFrame>> GetStackTraceAsync(
            string sessionId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<DebugStackFrame>>(
            [
                new DebugStackFrame
                {
                    Id = 42,
                    Name = "Main",
                    SourcePath = _sourcePath,
                    Line = 3,
                    Column = 5,
                },
            ]);
        }

        public Task<IReadOnlyList<DebugVariable>> GetVariablesAsync(
            string sessionId,
            int frameId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<DebugVariable>>(
            [
                new DebugVariable { Name = "counter", Value = "1", Type = "int" },
            ]);
        }

        public Task StopSessionAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            StopCount++;
            return Task.CompletedTask;
        }
    }
}
