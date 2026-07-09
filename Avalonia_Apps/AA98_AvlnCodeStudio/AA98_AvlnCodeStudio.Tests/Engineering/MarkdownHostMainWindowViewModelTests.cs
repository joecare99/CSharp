using AA98.MarkDown.Host.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

/// <summary>
/// Verifies tabbed markdown host workflows for create, open, save, and linked-file navigation.
/// </summary>
[TestClass]
public class MarkdownHostMainWindowViewModelTests
{
    /// <summary>
    /// Verifies that creating a new document appends and activates a fresh tab.
    /// </summary>
    [TestMethod]
    public void NewDocumentCommand_AddsAndSelectsTab()
    {
        MainWindowViewModel viewModel = new();
        int initialCount = viewModel.Documents.Count;

        viewModel.NewDocumentCommand.Execute(null);

        Assert.AreEqual(initialCount + 1, viewModel.Documents.Count);
        Assert.IsNotNull(viewModel.SelectedDocument);
        StringAssert.StartsWith(viewModel.SelectedDocument.Title, "Untitled-");
    }

    /// <summary>
    /// Verifies that SaveAs writes markdown content and synchronizes tab state.
    /// </summary>
    [TestMethod]
    public void SaveAsPathCommand_PersistsActiveDocument()
    {
        string tempRoot = Path.Combine(Path.GetTempPath(), $"md-host-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempRoot);

        try
        {
            string targetPath = Path.Combine(tempRoot, "saved.md");
            MainWindowViewModel viewModel = new();

            Assert.IsNotNull(viewModel.SelectedDocument);
            viewModel.SelectedDocument.Text = "# Saved";
            viewModel.DocumentPathInput = targetPath;

            viewModel.SaveAsPathCommand.Execute(null);

            Assert.IsTrue(File.Exists(targetPath));
            Assert.AreEqual("# Saved", File.ReadAllText(targetPath));
            Assert.AreEqual(targetPath, viewModel.SelectedDocument.FilePath);
            Assert.IsFalse(viewModel.SelectedDocument.IsDirty);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, true);
            }
        }
    }

    /// <summary>
    /// Verifies that linked markdown files can be opened relative to the active markdown file.
    /// </summary>
    [TestMethod]
    public void OpenLinkedDocument_ResolvesRelativeMarkdownLink()
    {
        string tempRoot = Path.Combine(Path.GetTempPath(), $"md-host-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempRoot);

        try
        {
            string parentPath = Path.Combine(tempRoot, "parent.md");
            string linkedPath = Path.Combine(tempRoot, "child.md");

            File.WriteAllText(parentPath, "[Child](child.md)");
            File.WriteAllText(linkedPath, "# Child");

            MainWindowViewModel viewModel = new();
            int initialCount = viewModel.Documents.Count;

            viewModel.DocumentPathInput = parentPath;
            viewModel.OpenPathCommand.Execute(null);
            viewModel.OpenLinkedDocument("child.md");

            Assert.IsTrue(viewModel.Documents.Count >= initialCount + 2);
            Assert.IsNotNull(viewModel.SelectedDocument);
            Assert.AreEqual(linkedPath, viewModel.SelectedDocument.FilePath);
            Assert.AreEqual("# Child", viewModel.SelectedDocument.Text);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, true);
            }
        }
    }

    /// <summary>
    /// Verifies that editing marks the active tab as dirty and appends the tab marker.
    /// </summary>
    [TestMethod]
    public void EditingDocument_MarksTabWithAsterisk()
    {
        MainWindowViewModel viewModel = new();
        Assert.IsNotNull(viewModel.SelectedDocument);

        viewModel.SelectedDocument.Text = "# Changed";

        Assert.IsTrue(viewModel.SelectedDocument.IsDirty);
        StringAssert.EndsWith(viewModel.SelectedDocument.DisplayTitle, "*");
    }

    /// <summary>
    /// Verifies that closing the last tab creates a new untitled tab to keep editing available.
    /// </summary>
    [TestMethod]
    public void CloseSelectedDocument_WhenLastTab_RecreatesUntitledTab()
    {
        MainWindowViewModel viewModel = new();
        Assert.AreEqual(1, viewModel.Documents.Count);

        viewModel.CloseSelectedDocument();

        Assert.AreEqual(1, viewModel.Documents.Count);
        Assert.IsNotNull(viewModel.SelectedDocument);
        StringAssert.StartsWith(viewModel.SelectedDocument.Title, "Untitled-");
    }
}
