using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Model.Documents;
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
            await workspace.OpenOrActivateAsync(secondPath);
            EditorTabViewModel secondTab = workspace.ActiveTab;

            firstTab.Editor.Text = "class First { int Value; }";
            await workspace.OpenOrActivateAsync(firstPath);

            Assert.AreEqual(3, workspace.Tabs.Count);
            Assert.AreSame(firstTab, workspace.ActiveTab);
            Assert.AreNotSame(firstTab.Editor, secondTab.Editor);
            Assert.AreEqual("class First { int Value; }", firstTab.Editor.Text);
            Assert.AreEqual("class Second {}", secondTab.Editor.Text);
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
