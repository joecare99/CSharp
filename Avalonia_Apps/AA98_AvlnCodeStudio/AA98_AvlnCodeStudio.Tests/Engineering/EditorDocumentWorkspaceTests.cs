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

    [TestMethod]
    public async Task WorkspaceChange_ExplicitlyReviewsDirtyDocumentAndRestoresItOnUndo()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_EditorDirtyChange_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "View.axaml");
            const string saved = "<Window />";
            const string dirty = "<Window Title=\"Unsaved\" />";
            const string proposed = "<Window Title=\"Reviewed\" />";
            await File.WriteAllTextAsync(path, saved);
            TestEditorComponentFactory factory = new();
            EditorDocumentWorkspace documents = new(factory.Create(), factory);
            await documents.OpenOrActivateAsync(path);
            documents.ActiveTab.Editor.Text = dirty;

            WorkspaceFileChangeService changes = new(documents);
            WorkspaceFileChangeProposal proposal = changes.Prepare(root,
            [
                WorkspaceFileChangeRequest.ReplaceExisting(
                    "View.axaml",
                    saved,
                    proposed,
                    expectedOpenDocumentContent: dirty),
            ]);
            changes.Apply(proposal, confirmed: true);

            Assert.AreEqual(proposed, documents.ActiveTab.Editor.Text);
            Assert.IsFalse(documents.ActiveTab.Editor.IsDirty);
            Assert.AreEqual(proposed, await File.ReadAllTextAsync(path));

            Assert.IsTrue(changes.Undo());
            Assert.AreEqual(dirty, documents.ActiveTab.Editor.Text);
            Assert.IsTrue(documents.ActiveTab.Editor.IsDirty);
            Assert.AreEqual(saved, await File.ReadAllTextAsync(path));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task WorkspaceChange_RejectsDocumentThatBecameDirtyAfterPreparation()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_EditorStaleChange_{Guid.NewGuid():N}");
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
                WorkspaceFileChangeRequest.ReplaceExisting("View.axaml", original, "<Window Title=\"Generated\" />"),
            ]);

            documents.ActiveTab.Editor.Text = "<Window Title=\"User edit\" />";
            Assert.ThrowsExactly<WorkspaceOpenDocumentException>(
                () => changes.Apply(proposal, confirmed: true));

            Assert.AreEqual(original, await File.ReadAllTextAsync(path));
            Assert.AreEqual("<Window Title=\"User edit\" />", documents.ActiveTab.Editor.Text);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task WorkspaceChange_UndoSynchronizesCleanDocumentAndRejectsDirtyDocument()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_EditorUndo_{Guid.NewGuid():N}");
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
                WorkspaceFileChangeRequest.ReplaceExisting("View.axaml", original, "<Window Title=\"Generated\" />"),
            ]);
            changes.Apply(proposal, confirmed: true);

            Assert.IsTrue(changes.Undo());
            Assert.AreEqual(original, documents.ActiveTab.Editor.Text);
            Assert.IsFalse(documents.ActiveTab.Editor.IsDirty);

            Assert.IsTrue(changes.Redo());
            documents.ActiveTab.Editor.Text = "<Window Title=\"Unsaved\" />";
            Assert.ThrowsExactly<WorkspaceOpenDocumentException>(() => changes.Undo());
            Assert.AreEqual("<Window Title=\"Generated\" />", await File.ReadAllTextAsync(path));
            Assert.AreEqual("<Window Title=\"Unsaved\" />", documents.ActiveTab.Editor.Text);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task OpenOrActivateAsync_UsesPlatformPathCaseRules()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_EditorCase_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string upperPath = Path.Combine(root, "View.axaml");
            string lowerPath = Path.Combine(root, "view.axaml");
            await File.WriteAllTextAsync(upperPath, "upper");
            if (!OperatingSystem.IsWindows())
            {
                await File.WriteAllTextAsync(lowerPath, "lower");
            }

            TestEditorComponentFactory factory = new();
            EditorDocumentWorkspace documents = new(factory.Create(), factory);
            await documents.OpenOrActivateAsync(upperPath);
            EditorTabViewModel upperTab = documents.ActiveTab;
            await documents.OpenOrActivateAsync(lowerPath);
            EditorTabViewModel lowerTab = documents.ActiveTab;

            if (OperatingSystem.IsWindows())
            {
                Assert.AreSame(upperTab, lowerTab);
                Assert.AreSame(upperTab, documents.FindByPath(lowerPath));
            }
            else
            {
                Assert.AreNotSame(upperTab, lowerTab);
                Assert.AreEqual("upper", upperTab.Editor.Text);
                Assert.AreEqual("lower", lowerTab.Editor.Text);
                Assert.AreSame(upperTab, documents.FindByPath(upperPath));
                Assert.AreSame(lowerTab, documents.FindByPath(lowerPath));
            }
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
