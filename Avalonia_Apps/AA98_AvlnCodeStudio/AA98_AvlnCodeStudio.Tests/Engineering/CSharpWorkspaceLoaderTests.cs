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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.IO;
using System.Linq;
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
