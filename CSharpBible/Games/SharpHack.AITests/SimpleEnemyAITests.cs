using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpHack.AI;
using SharpHack.Base.Model;

namespace SharpHack.AITests;

[TestClass]
public class SimpleEnemyAITests
{
    [TestMethod]
    public void GetNextMove_MovesTowardsTarget_XAxis()
    {
        var ai = new SimpleEnemyAI();
        var enemy = new Creature { Position = new Point(1, 1) };
        var target = new Creature { Position = new Point(3, 1) };
        var map = new Map(10, 10);
        map[2, 1].Type = TileType.Floor; // Path is clear

        var nextPos = ai.GetNextMove(enemy, target, map);

        Assert.AreEqual(new Point(2, 1), nextPos);
    }

    [TestMethod]
    public void GetNextMove_MovesTowardsTarget_YAxis()
    {
        var ai = new SimpleEnemyAI();
        var enemy = new Creature { Position = new Point(1, 1) };
        var target = new Creature { Position = new Point(1, 3) };
        var map = new Map(10, 10);
        map[1, 2].Type = TileType.Floor; // Path is clear

        var nextPos = ai.GetNextMove(enemy, target, map);

        Assert.AreEqual(new Point(1, 2), nextPos);
    }

    [TestMethod]
    public void GetNextMove_StaysPut_IfBlocked()
    {
        var ai = new SimpleEnemyAI();
        var enemy = new Creature { Position = new Point(1, 1) };
        var target = new Creature { Position = new Point(3, 1) };
        var map = new Map(10, 10);
        map[2, 1].Type = TileType.Wall; // Path is blocked

        var nextPos = ai.GetNextMove(enemy, target, map);

        Assert.AreEqual(new Point(1, 1), nextPos);
    }
}
