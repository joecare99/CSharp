using DetectiveGame.Engine.Game;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DetectiveGame.Tests;

[TestClass]
public class SetupTests
{
    [TestMethod]
    public void Distribution_Is_Fair()
    {
        var svc = new GameService();
        var state = svc.CreateNew(new[] { "A", "B", "C", "D" });
        var counts = state.Players.Select(p => p.Hand.Count).ToList();
        Assert.IsTrue(counts.Max() - counts.Min() <= 1);
        // total cards should be 27 - 3 = 24
        Assert.AreEqual(24, counts.Sum());
    }

    [TestMethod]
    public void Solution_Cards_Not_In_Hands()
    {
        var svc = new GameService();
        var state = svc.CreateNew(new[] { "A", "B", "C" });
        foreach (var p in state.Players)
        {
            Assert.IsFalse(p.Hand.Contains(state.Solution.Person));
            Assert.IsFalse(p.Hand.Contains(state.Solution.Weapon));
            Assert.IsFalse(p.Hand.Contains(state.Solution.Room));
        }
    }
}