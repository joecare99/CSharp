using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SharpHack.Base.Data;
using SharpHack.Base.Interfaces;
using SharpHack.Base.Model;
using SharpHack.Engine;
using BaseLib.Models.Interfaces;

namespace SharpHack.EngineTests;

[TestClass]
public class GameSessionPrevLevelTests
{
    [TestMethod]
    public void MovePlayer_IntoStairsUp_DecrementsLevel_AndPlacesStairsUpAtEntryPosition()
    {
        var mapGenerator = Substitute.For<IMapGenerator>();
        var random = Substitute.For<IRandom>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var enemyAI = Substitute.For<IEnemyAI>();
        var gamePersist = Substitute.For<IGamePersist>();

        var map1 = new Map(10, 10);
        map1[1, 1].Type = TileType.StairsUp;
        map1[0, 1].Type = TileType.Floor;

        var map0 = new Map(10, 10);
        map0[1, 1].Type = TileType.Wall;

        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map1, map0);
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Point>()).Returns(map1, map0);
        random.Next(Arg.Any<int>()).Returns(0);

        var session = new GameSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        session.Player.Position = new Point(0, 1);

        session.MovePlayer(Direction.East);

        Assert.AreEqual(0, session.Level);
        Assert.AreEqual(new Point(1, 1), session.Player.Position);
        Assert.AreEqual(TileType.StairsUp, session.Map[1, 1].Type);
        Assert.IsTrue(session.Map[1, 1].IsWalkable);
    }

    [TestMethod]
    public void MovePlayer_IntoStairsUp_LogsAscendMessage()
    {
        var mapGenerator = Substitute.For<IMapGenerator>();
        var random = Substitute.For<IRandom>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var enemyAI = Substitute.For<IEnemyAI>();
        var gamePersist = Substitute.For<IGamePersist>();

        var map1 = new Map(10, 10);
        map1[1, 1].Type = TileType.StairsUp;
        map1[0, 1].Type = TileType.Floor;

        var map0 = new Map(10, 10);
        map0[1, 1].Type = TileType.Floor;

        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map1, map0);
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Point>()).Returns(map1, map0);
        random.Next(Arg.Any<int>()).Returns(0);

        var session = new GameSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        session.Player.Position = new Point(0, 1);

        string? lastMessage = null;
        session.OnMessage += m => lastMessage = m;

        session.MovePlayer(Direction.East);

        Assert.AreEqual("You ascend to level 0.", lastMessage);
    }
}
