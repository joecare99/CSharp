using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Editor.Components;
using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.UI.Components;
using AA98_AvlnCodeStudio.UI.Services;
using AA98_AvlnCodeStudio.UI.ViewModels;
using Avalonia.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class EditorDocumentWorkspaceTests
{
    [TestMethod]
    public async Task OpenOrActivateAsync_CreatesIndependentTabsAndReusesAlreadyOpenFile()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_EditorTabs_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string firstPath = Path.Combine(root, "First.cs");
            string secondPath = Path.Combine(root, "Second.cs");
            await File.WriteAllTextAsync(firstPath, "class First {}");
            await File.WriteAllTextAsync(secondPath, "class Second {}");

            var factory = new TestEditorComponentFactory();
            var workspace = new EditorDocumentWorkspace(factory.Create(), factory);

            await workspace.OpenOrActivateAsync(firstPath);
            EditorTabViewModel firstTab = workspace.ActiveTab;
            Assert.IsTrue(workspace.IsDocumentOpen(firstPath));
            await workspace.OpenOrActivateAsync(secondPath);
            EditorTabViewModel secondTab = workspace.ActiveTab;
            Assert.IsTrue(workspace.IsDocumentOpen(secondPath));

            firstTab.Editor.Text = "class First { int Value; }";
            await workspace.OpenOrActivateAsync(firstPath);

            Assert.AreEqual(3, workspace.Tabs.Count);
            Assert.AreSame(firstTab, workspace.ActiveTab);
            Assert.AreNotSame(firstTab.Editor, secondTab.Editor);
            Assert.AreEqual("class First { int Value; }", firstTab.Editor.Text);
            Assert.AreEqual("class Second {}", secondTab.Editor.Text);
            Assert.IsTrue(workspace.IsDocumentOpen(Path.GetFullPath(firstPath)));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task WorkspaceChange_UpdatesCleanOpenDocumentAndRejectsDirtyDocument()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_EditorChange_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "View.axaml");
            const string original = "<Window />";
            await File.WriteAllTextAsync(path, original);
            TestEditorComponentFactory factory = new();
            EditorDocumentWorkspace documents = new(factory.Create(), factory);
            await documents.OpenOrActivateAsync(path);
            WorkspaceFileChangeService changes = new(documents);
            WorkspaceFileChangeProposal proposal = changes.Prepare(root,
            [
                WorkspaceFileChangeRequest.ReplaceExisting(
                    "View.axaml",
                    original,
                    "<Window Title=\"Updated\" />"),
            ]);

            changes.Apply(proposal, confirmed: true);

            Assert.AreEqual("<Window Title=\"Updated\" />", documents.ActiveTab.Editor.Text);
            Assert.IsFalse(documents.ActiveTab.Editor.IsDirty);
            Assert.AreEqual(documents.ActiveTab.Editor.Text, await File.ReadAllTextAsync(path));

            documents.ActiveTab.Editor.Text = "<Window Title=\"Unsaved\" />";
            Assert.ThrowsExactly<WorkspaceOpenDocumentException>(() => changes.Prepare(root,
            [
                WorkspaceFileChangeRequest.ReplaceExisting(
                    "View.axaml",
                    "<Window Title=\"Updated\" />",
                    "<Window Title=\"Generated\" />"),
            ]));
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
                new FileEditorDocument(),
                new DesignEditorFileDialogService(),
                new FileSystemTextDocumentStorageService());
            return new AvaloniaEditorComponent(workflow, new EditorViewModel(workflow), new TextBlock());
        }

        IEditorComponent IEditorComponentFactory.Create() => Create();
    }
}
