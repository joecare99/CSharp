using System;
using ConsoleLib.Showcase.Apps;
using ConsoleLib.Showcase.Apps.TicTacToe;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Showcase.Apps.TicTacToe.Tests;

[TestClass]
public sealed class TicTacToeGameTests
{
    [TestMethod]
    public void PlayTransitionsPlayersAndDetectsWinner()
    {
        var game = new TicTacToeGame();

        Assert.AreEqual(TicTacToePlayer.X, game.CurrentPlayer);
        Assert.AreEqual(TicTacToeMoveResult.Played, game.Play(0));
        Assert.AreEqual(TicTacToePlayer.O, game.CurrentPlayer);
        Assert.AreEqual(TicTacToeMoveResult.Invalid, game.Play(0));
        Assert.AreEqual(TicTacToeMoveResult.Played, game.Play(3));
        Assert.AreEqual(TicTacToeMoveResult.Played, game.Play(1));
        Assert.AreEqual(TicTacToeMoveResult.Played, game.Play(4));

        Assert.AreEqual(TicTacToeMoveResult.Won, game.Play(2));
        Assert.AreEqual(TicTacToePlayer.X, game.Winner);
        Assert.IsTrue(game.IsGameOver);
        Assert.AreEqual(TicTacToeMoveResult.Invalid, game.Play(5));
    }

    [TestMethod]
    public void FullBoardWithoutWinnerIsDrawAndRestartClearsState()
    {
        var game = new TicTacToeGame();

        foreach (var index in new[] { 0, 1, 2, 4, 3, 5, 7, 6 })
            Assert.AreEqual(TicTacToeMoveResult.Played, game.Play(index));

        Assert.AreEqual(TicTacToeMoveResult.Draw, game.Play(8));
        Assert.IsTrue(game.IsDraw);
        Assert.AreEqual(TicTacToePlayer.None, game.Winner);

        game.Restart();

        Assert.AreEqual(TicTacToePlayer.X, game.CurrentPlayer);
        Assert.AreEqual(0, game.TurnCount);
        Assert.IsFalse(game.IsGameOver);
        foreach (var cell in game.Board)
            Assert.AreEqual(TicTacToePlayer.None, cell);
    }

    [TestMethod]
    public void ModuleRegistersAndLoadsItsEmbeddedPage()
    {
        var context = new ShowcaseAppRegistrationContext(new EmptyServiceProvider());
        new TicTacToeAppModule().Register(context);

        var descriptor = context.Apps["TicTacToe"];
        var viewModel = descriptor.CreateViewModel(context.Services);
        var page = descriptor.LoadPage(context.Services, viewModel);

        Assert.IsInstanceOfType<TicTacToeViewModel>(viewModel);
        Assert.AreEqual("Tic-Tac-Toe", page.Root.Text);
        Assert.IsTrue(page.NamedControls.ContainsKey("Cell0"));
        Assert.IsTrue(page.NamedControls.ContainsKey("Restart"));
    }

    private sealed class EmptyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }
}
