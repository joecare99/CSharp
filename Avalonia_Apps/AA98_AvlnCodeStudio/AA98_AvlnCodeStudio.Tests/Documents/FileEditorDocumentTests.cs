using AA98_AvlnCodeStudio.Model.Documents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AA98_AvlnCodeStudio.Tests.Documents;

/// <summary>
/// Verifies the state transitions of <see cref="FileEditorDocument"/>.
/// </summary>
[TestClass]
public class FileEditorDocumentTests
{
    /// <summary>
    /// Verifies that a new instance starts as a clean untitled document.
    /// </summary>
    [TestMethod]
    public void NewDocumentStartsClean()
    {
        var document = new FileEditorDocument();

        Assert.AreEqual("Untitled.txt", document.DisplayName);
        Assert.AreEqual(string.Empty, document.Content);
        Assert.IsFalse(document.IsDirty);
        Assert.IsNull(document.FilePath);
    }

    /// <summary>
    /// Verifies that loading a file updates file name, content, and dirty state.
    /// </summary>
    [TestMethod]
    public void LoadSetsContentAndClearsDirtyFlag()
    {
        var document = new FileEditorDocument();

        document.Load(@"C:\Temp\sample.cs", "class C {}\n");

        Assert.AreEqual(@"C:\Temp\sample.cs", document.FilePath);
        Assert.AreEqual("sample.cs", document.DisplayName);
        Assert.AreEqual("class C {}\n", document.Content);
        Assert.IsFalse(document.IsDirty);
    }

    /// <summary>
    /// Verifies that content changes mark the document as dirty.
    /// </summary>
    [TestMethod]
    public void UpdateContentMarksDocumentDirty()
    {
        var document = new FileEditorDocument();

        document.UpdateContent("Hello editor");

        Assert.AreEqual("Hello editor", document.Content);
        Assert.IsTrue(document.IsDirty);
    }

    /// <summary>
    /// Verifies that saving stores the file path and clears the dirty flag.
    /// </summary>
    [TestMethod]
    public void MarkSavedClearsDirtyState()
    {
        var document = new FileEditorDocument();
        document.UpdateContent("Draft");

        document.MarkSaved(@"C:\Temp\draft.txt");

        Assert.AreEqual(@"C:\Temp\draft.txt", document.FilePath);
        Assert.IsFalse(document.IsDirty);
    }

    /// <summary>
    /// Verifies that invalid load arguments are rejected.
    /// </summary>
    [TestMethod]
    public void LoadRejectsMissingFilePath()
    {
        var document = new FileEditorDocument();

        Assert.Throws<ArgumentException>(() => document.Load(string.Empty, "content"));
    }
}
