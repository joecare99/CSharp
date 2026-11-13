using AA98_AvlnCodeStudio.Base.UI.Services;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AA98_AvlnCodeStudio.Editor.Components;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using AA98_AvlnCodeStudio.UI.Components;
using AA98_AvlnCodeStudio.UI.Services;
using AA98_AvlnCodeStudio.UI.ViewModels;
using Avalonia.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AxamlWorkbenchResourceTests
{
    [TestMethod]
    public async Task Workbench_ContextFollowsWorkspaceTabsAndSaveAsWithoutLosingDraftOrHistory()
    {
        string fixture = Path.Combine(Environment.CurrentDirectory, "workbench-resource-" + Guid.NewGuid().ToString("N"));
        try
        {
            string first = CreateProject(fixture, "First", "First");
            string second = CreateProject(fixture, "Second", "Second");
            string firstView = Path.Combine(first, "Views", "View.axaml");
            string secondView = Path.Combine(second, "Views", "View.axaml");
            string savedView = Path.Combine(second, "Views", "Saved.axaml");
            const string markup = """
                <StackPanel xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:p='clr-namespace:Sample.Properties'>
                  <Button x:Name='Target' Width='100' Content='{x:Static p:Resources.Caption}'/>
                </StackPanel>
                """;
            File.WriteAllText(firstView, markup);
            File.WriteAllText(secondView, markup);
            string solution = Path.Combine(fixture, "Workspace.slnx");
            File.WriteAllText(solution, """
                <Solution><Project Path="First\Sample.csproj"/><Project Path="Second\Sample.csproj"/></Solution>
                """);
            IEditorFileDialogService dialog = Substitute.For<IEditorFileDialogService>();
            dialog.ShowSaveFileDialogAsync(Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<string?>(savedView));
            TestFactory factory = new(dialog);
            IAvaloniaEditorComponent initial = factory.Create();
            using EditorDocumentWorkspace documents = new(initial, factory);
            MainWindowViewModel workbench = new(
                initial,
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(),
                null,
                null,
                editorWorkspace: documents);

            await documents.OpenOrActivateAsync(firstView);
            EditorTabViewModel firstTab = documents.ActiveTab;
            var designer = firstTab.Designer!;
            Assert.AreEqual("{x:Static p:Resources.Caption}", designer.PreviewRoot!.Children[0].Text);
            workbench.Workspace = new CSharpWorkspaceLoader().Load(solution);
            Assert.AreEqual("First", designer.PreviewRoot!.Children[0].Text);
            designer.SelectedNode = designer.PreviewRoot.Children[0];
            designer.SelectedProperty = "Width";
            designer.SelectedValue = "120";
            designer.ApplySelectedValueCommand.Execute(null);
            designer.SelectedValue = "140";
            designer.ApplySelectedValueCommand.Execute(null);
            designer.UndoCommand.Execute(null);
            Assert.IsTrue(designer.CanUndo);
            Assert.IsTrue(designer.CanRedo);
            var draft = designer.PropertyRows.Single(row => row.Name == "Width");
            Assert.IsFalse(draft.TrySetValue("-2"));
            string unchanged = firstTab.Editor.Text;

            await documents.OpenOrActivateAsync(secondView);
            EditorTabViewModel secondTab = documents.ActiveTab;
            Assert.AreEqual("Second", secondTab.Designer!.PreviewRoot!.Children[0].Text);
            workbench.Workspace = null;
            Assert.AreEqual("{x:Static p:Resources.Caption}", firstTab.Designer!.PreviewRoot!.Children[0].Text);
            Assert.AreEqual("-2", firstTab.Designer.PropertyRows.Single(row => row.Name == "Width").Value);
            Assert.AreEqual(unchanged, firstTab.Editor.Text);

            workbench.Workspace = new CSharpWorkspaceLoader().Load(solution);
            documents.Activate(firstTab);
            Assert.AreEqual("First", designer.PreviewRoot!.Children[0].Text);
            Assert.AreEqual("Target", designer.SelectedNode!.StableName);
            Assert.AreEqual("-2", designer.PropertyRows.Single(row => row.Name == "Width").Value);
            designer.DiscardAllPropertyDrafts();
            Assert.IsTrue(designer.CanUndo);
            Assert.IsTrue(designer.CanRedo);

            await firstTab.Editor.SaveAsCommand.ExecuteAsync(null);
            Assert.AreEqual(Path.GetFullPath(savedView), firstTab.Editor.CurrentFilePath);
            Assert.AreEqual("Second", designer.PreviewRoot!.Children[0].Text);
            Assert.IsTrue(designer.CanUndo);
            Assert.IsTrue(designer.CanRedo);
            designer.RedoCommand.Execute(null);
            Assert.AreEqual("140", designer.SelectedValue);
            Assert.AreEqual("Second", designer.PreviewRoot!.Children[0].Text);
            workbench.Workspace = null;
        }
        finally
        {
            if (Directory.Exists(fixture))
            {
                Directory.Delete(fixture, true);
            }
        }
    }

    [TestMethod]
    public async Task Workbench_AmbiguousProjectLeavesStaticReferenceUnresolved()
    {
        string fixture = Path.Combine(Environment.CurrentDirectory, "workbench-ambiguous-" + Guid.NewGuid().ToString("N"));
        try
        {
            string project = CreateProject(fixture, "Shared", "Must not resolve");
            File.WriteAllText(Path.Combine(project, "Other.csproj"), "<Project />");
            string view = Path.Combine(project, "Views", "View.axaml");
            File.WriteAllText(view,
                "<Button xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:p='clr-namespace:Sample.Properties' Content='{x:Static p:Resources.Caption}'/>");
            string solution = Path.Combine(fixture, "Workspace.slnx");
            File.WriteAllText(solution, "<Solution><Project Path='Shared\\Sample.csproj'/><Project Path='Shared\\Other.csproj'/></Solution>");
            TestFactory factory = new(new DesignEditorFileDialogService());
            IAvaloniaEditorComponent initial = factory.Create();
            using EditorDocumentWorkspace documents = new(initial, factory);
            MainWindowViewModel workbench = new(initial, new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(), null, null, editorWorkspace: documents);
            workbench.Workspace = new CSharpWorkspaceLoader().Load(solution);
            await documents.OpenOrActivateAsync(view);
            Assert.AreEqual("{x:Static p:Resources.Caption}", documents.ActiveTab.Designer!.PreviewRoot!.Text);
            workbench.Workspace = null;
        }
        finally
        {
            if (Directory.Exists(fixture))
            {
                Directory.Delete(fixture, true);
            }
        }
    }

    [TestMethod]
    public async Task Workbench_OpensReportedCalculatorUsingItsProjectContext()
    {
        const string root = @"C:\Projekte\CSharp\Avalonia_Apps\AA05_CommandParCalc\AA05_CommandParCalc";
        string path = Path.Combine(root, "Views", "CommandParCalculatorView.axaml");
        if (!File.Exists(path))
        {
            Assert.Inconclusive("The external read-only calculator sample is not present.");
        }

        TestFactory factory = new(new DesignEditorFileDialogService());
        IAvaloniaEditorComponent initial = factory.Create();
        using EditorDocumentWorkspace documents = new(initial, factory);
        MainWindowViewModel workbench = new(initial, new PlanningExplorerViewModel(new MarkdownPlanningReader()),
            new DiagnosticCollectionViewModel(), null, null, editorWorkspace: documents);
        try
        {
            workbench.Workspace = new CSharpWorkspaceLoader().Load(Path.Combine(root, "AA05_CommandParCalc.csproj"));
            await documents.OpenOrActivateAsync(path);
            AxamlPreviewNode button = Descendants(documents.ActiveTab.Designer!.PreviewRoot!)
                .Single(node => node.StableName == "btn7");
            Assert.AreEqual("7", button.Text);
            Assert.AreEqual("-5", button.RenderProperties["Margin"]);
            Assert.AreEqual("10", button.RenderProperties["Padding"]);
            Assert.IsFalse(documents.ActiveTab.IsDirty);
        }
        finally
        {
            workbench.Workspace = null;
        }
    }

    private static System.Collections.Generic.IEnumerable<AxamlPreviewNode> Descendants(AxamlPreviewNode node) =>
        new[] { node }.Concat(node.Children.SelectMany(Descendants));

    private static string CreateProject(string fixture, string folder, string caption)
    {
        string root = Path.Combine(fixture, folder);
        Directory.CreateDirectory(Path.Combine(root, "Properties"));
        Directory.CreateDirectory(Path.Combine(root, "Views"));
        File.WriteAllText(Path.Combine(root, "Sample.csproj"), "<Project />");
        File.WriteAllText(Path.Combine(root, "Properties", "Resources.resx"),
            $"<root><data name='Caption'><value>{caption}</value></data></root>");
        return root;
    }

    private sealed class TestFactory(IEditorFileDialogService dialog) : IAvaloniaEditorComponentFactory
    {
        public IAvaloniaEditorComponent Create()
        {
            EditorWorkflow workflow = new(new FileEditorDocument(), dialog, new FileSystemTextDocumentStorageService());
            return new AvaloniaEditorComponent(workflow, new EditorViewModel(workflow), new Border());
        }

        IEditorComponent IEditorComponentFactory.Create() => Create();
    }
}
