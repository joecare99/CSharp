using DetectiveGame.Engine.Game;
using DetectiveGame.Engine.Cards;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DetectiveGame.Tests;

[TestClass]
public class SuggestionTests
{
    [TestMethod]
    public void Suggestion_Returns_First_Refuter()
    {
        var svc = new GameService();
        var state = svc.CreateNew(new[] { "A", "B", "C" }, seed: 123);
        var current = state.Players[state.CurrentPlayerIndex];
        // pick three cards from next player hand to ensure refutation
        var next = state.Players[(state.CurrentPlayerIndex + 1) % state.Players.Count];
        var card = next.Hand.First();
        Card person = card.Category == CardCategory.Person ? card : state.Players.SelectMany(p=>p.Hand).First(c=>c.Category==CardCategory.Person);
        Card weapon = card.Category == CardCategory.Weapon ? card : state.Players.SelectMany(p=>p.Hand).First(c=>c.Category==CardCategory.Weapon);
        Card room = card.Category == CardCategory.Room ? card : state.Players.SelectMany(p=>p.Hand).First(c=>c.Category==CardCategory.Room);
        var result = svc.MakeSuggestion(state, current.Id, person, weapon, room);
        Assert.IsNotNull(result.RefutingPlayerId);
    }
}