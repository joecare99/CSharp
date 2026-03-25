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
        Assert.IsLessThanOrEqualTo(1, counts.Max() - counts.Min());
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
            Assert.DoesNotContain(state.Solution.Person, p.Hand);
            Assert.DoesNotContain(state.Solution.Weapon, p.Hand);
            Assert.DoesNotContain(state.Solution.Room, p.Hand);
        }
    }
}