using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AA98_AvlnCodeStudio.Editor.Navigation;
using AppKomponentBaseLib.Diagnostics;
using Code.Navigation;
using Diagnostics.Navigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class DiagnosticsNavigationIntegrationTests
{
    [TestMethod]
    public async Task DiagnosticActivation_MapsLocationAndMovesEditorCaret()
    {
        var documentHost = Substitute.For<IEditorDocumentHost>();
        var caretService = Substitute.For<IEditorCaretService>();
        var document = Substitute.For<IEditorDocument>();
        string sourcePath = Path.GetFullPath("planning.md");
        var diagnostic = new Diagnostic
        {
            Code = "PLN001",
            Severity = DiagnosticSeverity.Error,
            Message = "Planning root missing.",
            SourcePath = sourcePath,
            LineNumber = 12,
            ColumnNumber = 5,
        };
        var locationMapper = new DiagnosticLocationMapper();
        var navigator = new EditorCodeNavigator(documentHost, caretService);
        var viewModel = new DiagnosticCollectionViewModel(locationMapper, navigator);
        documentHost.OpenOrActivateAsync(sourcePath, Arg.Any<CancellationToken>()).Returns(document);

        await viewModel.ConsumeAsync(new[] { diagnostic });
        await viewModel.ActivateAsync(diagnostic);

        var expectedLocation = new CodeLocation(sourcePath, 12, 5);
        await documentHost.Received(1).OpenOrActivateAsync(sourcePath, CancellationToken.None);
        await caretService.Received(1).FocusAndMoveCaretAsync(document, expectedLocation, CancellationToken.None);
        Assert.AreSame(diagnostic, viewModel.Items[0]);
        Assert.AreEqual(string.Empty, viewModel.ActivationErrorText);
    }

    [TestMethod]
    public async Task DiagnosticActivation_FileOnlyLocationOpensDocumentWithoutCoordinates()
    {
        var documentHost = Substitute.For<IEditorDocumentHost>();
        var caretService = Substitute.For<IEditorCaretService>();
        var document = Substitute.For<IEditorDocument>();
        string sourcePath = Path.GetFullPath("planning.md");
        documentHost.OpenOrActivateAsync(sourcePath, Arg.Any<CancellationToken>()).Returns(document);
        var viewModel = new DiagnosticCollectionViewModel(
            new DiagnosticLocationMapper(),
            new EditorCodeNavigator(documentHost, caretService));

        await viewModel.ActivateAsync(new Diagnostic
        {
            Code = "PLN002",
            Severity = DiagnosticSeverity.Warning,
            Message = "File-level warning.",
            SourcePath = sourcePath,
        });

        await documentHost.Received(1).OpenOrActivateAsync(sourcePath, CancellationToken.None);
        await caretService.Received(1).FocusAndMoveCaretAsync(
            document,
            Arg.Is<CodeLocation>(location => location.Path == sourcePath && location.Line == null && location.Column == null),
            CancellationToken.None);
        Assert.AreEqual(string.Empty, viewModel.ActivationErrorText);
    }

    [TestMethod]
    public async Task DiagnosticActivation_InvalidCoordinatesDoesNotContactEditor()
    {
        var documentHost = Substitute.For<IEditorDocumentHost>();
        var caretService = Substitute.For<IEditorCaretService>();
        var viewModel = new DiagnosticCollectionViewModel(
            new DiagnosticLocationMapper(),
            new EditorCodeNavigator(documentHost, caretService));

        await viewModel.ActivateAsync(new Diagnostic
        {
            Code = "PLN003",
            Severity = DiagnosticSeverity.Error,
            Message = "Invalid line.",
            SourcePath = Path.GetFullPath("planning.md"),
            LineNumber = 0,
        });

        await documentHost.DidNotReceiveWithAnyArgs().OpenOrActivateAsync(default!, default);
        StringAssert.Contains(viewModel.ActivationErrorText, "coordinates");
    }

    [TestMethod]
    public async Task DiagnosticActivation_MissingFilePreservesDiagnosticAndReportsRecoverableError()
    {
        var documentHost = Substitute.For<IEditorDocumentHost>();
        var caretService = Substitute.For<IEditorCaretService>();
        var diagnostic = new Diagnostic
        {
            Code = "PLN004",
            Severity = DiagnosticSeverity.Error,
            Message = "Source file missing.",
            SourcePath = Path.GetFullPath("missing-planning.md"),
            LineNumber = 7,
        };
        documentHost.OpenOrActivateAsync(diagnostic.SourcePath!, Arg.Any<CancellationToken>())
            .Returns((IEditorDocument?)null);
        var viewModel = new DiagnosticCollectionViewModel(
            new DiagnosticLocationMapper(),
            new EditorCodeNavigator(documentHost, caretService));

        await viewModel.ConsumeAsync(new[] { diagnostic });
        await viewModel.ActivateAsync(diagnostic);

        Assert.AreEqual(1, viewModel.Items.Count);
        Assert.AreSame(diagnostic, viewModel.Items[0]);
        StringAssert.Contains(viewModel.ActivationErrorText, "could not open");
        await caretService.DidNotReceiveWithAnyArgs().FocusAndMoveCaretAsync(default!, default!);
    }
}