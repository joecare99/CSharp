using AA98_AvlnCodeStudio.Editor.Navigation;
using Code.Navigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Editor.Tests.Navigation;

/// <summary>
/// Verifies the editor-host bridge for neutral code locations.
/// </summary>
[TestClass]
public sealed class EditorCodeNavigatorTests
{
    [TestMethod]
    public async Task NavigateAsync_OpensDocumentAndMovesCaret()
    {
        var documentHost = Substitute.For<IEditorDocumentHost>();
        var caretService = Substitute.For<IEditorCaretService>();
        var document = Substitute.For<IEditorDocument>();
        var location = new CodeLocation("source\file.cs", 4, 7);
        documentHost.OpenOrActivateAsync(location.Path, Arg.Any<CancellationToken>()).Returns(document);
        var navigator = new EditorCodeNavigator(documentHost, caretService);

        await navigator.NavigateAsync(location);

        await documentHost.Received(1).OpenOrActivateAsync(location.Path, CancellationToken.None);
        await caretService.Received(1).FocusAndMoveCaretAsync(document, location, CancellationToken.None);
    }

    [TestMethod]
    public async Task NavigateAsync_MissingDocumentThrowsFileNotFoundException()
    {
        var documentHost = Substitute.For<IEditorDocumentHost>();
        var caretService = Substitute.For<IEditorCaretService>();
        var location = new CodeLocation("missing.cs");
        documentHost.OpenOrActivateAsync(location.Path, Arg.Any<CancellationToken>()).Returns((IEditorDocument?)null);
        var navigator = new EditorCodeNavigator(documentHost, caretService);

        await Assert.ThrowsExactlyAsync<FileNotFoundException>(() => navigator.NavigateAsync(location));

        await caretService.DidNotReceiveWithAnyArgs().FocusAndMoveCaretAsync(default!, default!);
    }

    [TestMethod]
    public async Task NavigateAsync_ForwardsOffsetAndCoordinatesWithoutChangingPolicy()
    {
        var documentHost = Substitute.For<IEditorDocumentHost>();
        var caretService = Substitute.For<IEditorCaretService>();
        var document = Substitute.For<IEditorDocument>();
        var location = new CodeLocation("source\file.cs", 2, 3, 12);
        documentHost.OpenOrActivateAsync(location.Path, Arg.Any<CancellationToken>()).Returns(document);
        var navigator = new EditorCodeNavigator(documentHost, caretService);

        await navigator.NavigateAsync(location);

        await caretService.Received(1).FocusAndMoveCaretAsync(document, location, CancellationToken.None);
    }

    [TestMethod]
    public async Task NavigateAsync_RepeatedRequestsReuseHostAndCaretContract()
    {
        var documentHost = Substitute.For<IEditorDocumentHost>();
        var caretService = Substitute.For<IEditorCaretService>();
        var document = Substitute.For<IEditorDocument>();
        var location = new CodeLocation("source\file.cs", 3, 2);
        documentHost.OpenOrActivateAsync(location.Path, Arg.Any<CancellationToken>()).Returns(document);
        var navigator = new EditorCodeNavigator(documentHost, caretService);

        await navigator.NavigateAsync(location);
        await navigator.NavigateAsync(location);

        await documentHost.Received(2).OpenOrActivateAsync(location.Path, CancellationToken.None);
        await caretService.Received(2).FocusAndMoveCaretAsync(document, location, CancellationToken.None);
    }

    [TestMethod]
    public async Task NavigateAsync_CanceledRequestDoesNotContactHost()
    {
        var documentHost = Substitute.For<IEditorDocumentHost>();
        var caretService = Substitute.For<IEditorCaretService>();
        var navigator = new EditorCodeNavigator(documentHost, caretService);
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(
            () => navigator.NavigateAsync(new CodeLocation("source\file.cs"), cancellationSource.Token));

        await documentHost.DidNotReceiveWithAnyArgs().OpenOrActivateAsync(default!, default);
    }

    [TestMethod]
    public async Task NavigateAsync_PropagatesCaretFailureAfterHostActivation()
    {
        var documentHost = Substitute.For<IEditorDocumentHost>();
        var caretService = Substitute.For<IEditorCaretService>();
        var document = Substitute.For<IEditorDocument>();
        var location = new CodeLocation("source\\file.cs", 5, 1);
        var expected = new InvalidOperationException("Caret operation failed.");
        documentHost.OpenOrActivateAsync(location.Path, Arg.Any<CancellationToken>()).Returns(document);
        caretService
            .FocusAndMoveCaretAsync(document, location, Arg.Any<CancellationToken>())
            .Returns(Task.FromException(expected));
        var navigator = new EditorCodeNavigator(documentHost, caretService);

        var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => navigator.NavigateAsync(location));

        Assert.AreSame(expected, actual);
        await documentHost.Received(1).OpenOrActivateAsync(location.Path, CancellationToken.None);
        await caretService.Received(1).FocusAndMoveCaretAsync(document, location, CancellationToken.None);
    }
}