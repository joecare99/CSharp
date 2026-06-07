using System.Linq;
using Avln_Bubbles.View.Services;
using Avln_Bubbles.View.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Avln_Bubbles.View.Tests;

/// <summary>
/// Tests the main shell view model.
/// </summary>
[TestClass]
public sealed class MainWindowViewModelTests
{
    /// <summary>
    /// Verifies that the shell creates a board immediately and exposes host information.
    /// </summary>
    [TestMethod]
    public void ConstructorCreatesBoardAndHostSpecificStatus()
    {
        var gameSessionFactory = Substitute.For<IGameSessionFactory>();
        var hostDescriptor = Substitute.For<IHostFeatureDescriptor>();
        hostDescriptor.HostLabel.Returns("Browser host");
        hostDescriptor.SupportsBrowserHosting.Returns(true);
        hostDescriptor.SupportsDesktopWindowing.Returns(false);
        hostDescriptor.SupportsRemoteHostingPreparation.Returns(true);
        hostDescriptor.RemoteHostingSummary.Returns("Prepared for remote composition.");

        var board = new BubblesBoardViewModel(new Model.BubbleTable(4, 4, 123));
        gameSessionFactory.CreateBoard(11, 11, default).ReturnsForAnyArgs(board);

        var viewModel = new MainWindowViewModel(gameSessionFactory, hostDescriptor);

        Assert.IsNotNull(viewModel.Board);
        Assert.AreEqual("Bubbles on Avalonia", viewModel.Title);
        Assert.IsTrue(viewModel.StatusText.Contains("Browser host"));
        Assert.IsTrue(viewModel.IsBrowserHost);
        Assert.IsFalse(viewModel.IsDesktopHost);
        Assert.AreEqual(16, viewModel.Board!.Balls.Count);
        Assert.IsTrue(viewModel.Board.Balls.Any());
    }
}
