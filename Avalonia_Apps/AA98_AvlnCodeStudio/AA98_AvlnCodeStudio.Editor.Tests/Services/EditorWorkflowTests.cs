using AA98_AvlnCodeStudio.Base.OS.Services;
using AA98_AvlnCodeStudio.Base.UI.Services;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Model.Documents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Editor.Tests.Services;

/// <summary>
/// Verifies the UI-agnostic editor workflow behavior.
/// </summary>
[TestClass]
public class EditorWorkflowTests
{
    /// <summary>
    /// Verifies that a new document resets the state and reports completion.
    /// </summary>
    [TestMethod]
    public async Task NewDocumentAsync_ResetsDocument()
    {
        var document = new FileEditorDocument();
        document.Load(@"C:\Temp\sample.cs", "class C {}\n");
        var dialogService = Substitute.For<IEditorFileDialogService>();
        var storageService = Substitute.For<ITextDocumentStorageService>();
        var workflow = new EditorWorkflow(document, dialogService, storageService);

        var result = await workflow.NewDocumentAsync();

        Assert.IsTrue(result.IsCompleted);
        Assert.AreEqual(EditorOperationKind.NewDocument, result.OperationKind);
        Assert.AreEqual("Untitled.txt", document.DisplayName);
        Assert.AreEqual(string.Empty, document.Content);
        Assert.IsFalse(document.IsDirty);
    }

    /// <summary>
    /// Verifies that opening a document loads the selected content.
    /// </summary>
    [TestMethod]
    public async Task OpenAsync_LoadsSelectedDocument()
    {
        var document = new FileEditorDocument();
        var dialogService = Substitute.For<IEditorFileDialogService>();
        var storageService = Substitute.For<ITextDocumentStorageService>();
        dialogService.ShowOpenFileDialogAsync(Arg.Any<string?>(), default).Returns(@"C:\Temp\sample.md");
        storageService.ReadAllTextAsync(@"C:\Temp\sample.md", default).Returns("# sample");
        var workflow = new EditorWorkflow(document, dialogService, storageService);

        var result = await workflow.OpenAsync();

        Assert.IsTrue(result.IsCompleted);
        Assert.AreEqual(EditorOperationKind.Open, result.OperationKind);
        Assert.AreEqual(@"C:\Temp\sample.md", document.FilePath);
        Assert.AreEqual("# sample", document.Content);
        Assert.IsFalse(document.IsDirty);
    }

    /// <summary>
    /// Verifies that save uses the existing file path without prompting.
    /// </summary>
    [TestMethod]
    public async Task SaveAsync_UsesExistingFilePath()
    {
        var document = new FileEditorDocument();
        document.Load(@"C:\Temp\sample.cs", "class C {}\n");
        document.UpdateContent("class C { }\n");
        var dialogService = Substitute.For<IEditorFileDialogService>();
        var storageService = Substitute.For<ITextDocumentStorageService>();
        var workflow = new EditorWorkflow(document, dialogService, storageService);

        var result = await workflow.SaveAsync();

        Assert.IsTrue(result.IsCompleted);
        Assert.AreEqual(EditorOperationKind.Save, result.OperationKind);
        await storageService.Received(1).SaveAllTextAsync(@"C:\Temp\sample.cs", "class C { }\n", default);
        Assert.IsFalse(document.IsDirty);
    }

    /// <summary>
    /// Verifies that save as prompts for a file path and persists the document.
    /// </summary>
    [TestMethod]
    public async Task SaveAsAsync_PromptsAndSavesDocument()
    {
        var document = new FileEditorDocument();
        document.UpdateContent("notes");
        var dialogService = Substitute.For<IEditorFileDialogService>();
        var storageService = Substitute.For<ITextDocumentStorageService>();
        dialogService.ShowSaveFileDialogAsync(Arg.Any<string?>(), "Untitled.txt", default).Returns(@"C:\Temp\notes.txt");
        var workflow = new EditorWorkflow(document, dialogService, storageService);

        var result = await workflow.SaveAsAsync();

        Assert.IsTrue(result.IsCompleted);
        Assert.AreEqual(EditorOperationKind.SaveAs, result.OperationKind);
        Assert.AreEqual(@"C:\Temp\notes.txt", document.FilePath);
        await storageService.Received(1).SaveAllTextAsync(@"C:\Temp\notes.txt", "notes", default);
    }

    /// <summary>
    /// Verifies that canceling open returns a canceled result.
    /// </summary>
    [TestMethod]
    public async Task OpenAsync_ReturnsCanceledWhenDialogReturnsNoPath()
    {
        var document = new FileEditorDocument();
        var dialogService = Substitute.For<IEditorFileDialogService>();
        var storageService = Substitute.For<ITextDocumentStorageService>();
        dialogService.ShowOpenFileDialogAsync(Arg.Any<string?>(), default).Returns((string?)null);
        var workflow = new EditorWorkflow(document, dialogService, storageService);

        var result = await workflow.OpenAsync();

        Assert.IsFalse(result.IsCompleted);
        Assert.AreEqual(EditorOperationStatus.Canceled, result.Status);
        await storageService.DidNotReceive().ReadAllTextAsync(Arg.Any<string>(), default);
    }
}