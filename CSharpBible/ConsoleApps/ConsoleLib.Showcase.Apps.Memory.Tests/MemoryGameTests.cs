using System;
using System.Linq;
using ConsoleLib.Showcase.Apps;
using ConsoleLib.Showcase.Apps.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Showcase.Apps.Memory.Tests;

[TestClass]
public sealed class MemoryGameTests
{
    [TestMethod]
    public void SeededOrInjectedRandomProducesDeterministicCardLayout()
    {
        var first = new MemoryGame(4, 1729);
        var second = new MemoryGame(4, 1729);
        var randomFirst = new MemoryGame(new Random(1729));
        var randomSecond = new MemoryGame(new Random(1729));

        CollectionAssert.AreEqual(
            first.Cards.Select(card => card.Value).ToArray(),
            second.Cards.Select(card => card.Value).ToArray());
        CollectionAssert.AreEqual(
            randomFirst.Cards.Select(card => card.Value).ToArray(),
            randomSecond.Cards.Select(card => card.Value).ToArray());
    }

    [TestMethod]
    public void SelectingPairCountsMoveAndCompletesGame()
    {
        var game = new MemoryGame(1, 42);

        Assert.AreEqual(MemorySelectionResult.FirstSelected, game.SelectCard(0));
        Assert.AreEqual(MemorySelectionResult.Completed, game.SelectCard(1));
        Assert.AreEqual(1, game.Moves);
        Assert.AreEqual(1, game.MatchedPairs);
        Assert.IsTrue(game.IsCompleted);
        Assert.IsTrue(game.Cards.All(card => card.IsMatched));
    }

    [TestMethod]
    public void MismatchHidesCardsAndRestartResetsProgress()
    {
        var game = new MemoryGame(2, 42);
        var firstIndex = 0;
        var differentIndex = game.Cards
            .Select((card, index) => (card, index))
            .First(candidate => candidate.card.Value != game.Cards[firstIndex].Value)
            .index;

        game.SelectCard(firstIndex);
        Assert.AreEqual(MemorySelectionResult.Mismatch, game.SelectCard(differentIndex));
        Assert.AreEqual(1, game.Moves);
        Assert.IsFalse(game.Cards[firstIndex].IsFaceUp);
        Assert.IsFalse(game.Cards[differentIndex].IsFaceUp);

        game.Restart();

        Assert.AreEqual(0, game.Moves);
        Assert.AreEqual(0, game.MatchedPairs);
        Assert.IsFalse(game.IsCompleted);
        Assert.IsTrue(game.Cards.All(card => !card.IsFaceUp && !card.IsMatched));
    }

    [TestMethod]
    public void ModuleRegistersAndLoadsItsEmbeddedPage()
    {
        var context = new ShowcaseAppRegistrationContext(new EmptyServiceProvider());
        new MemoryAppModule().Register(context);

        var descriptor = context.Apps["Memory"];
        var viewModel = descriptor.CreateViewModel(context.Services);
        var page = descriptor.LoadPage(context.Services, viewModel);

        Assert.IsInstanceOfType<MemoryViewModel>(viewModel);
        Assert.AreEqual("Memory", page.Root.Text);
        Assert.IsTrue(page.NamedControls.ContainsKey("Card0"));
        Assert.IsTrue(page.NamedControls.ContainsKey("Restart"));
    }

    private sealed class EmptyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }
}
