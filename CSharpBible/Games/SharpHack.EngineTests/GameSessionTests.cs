using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SharpHack.Engine;
using SharpHack.Base.Model;
using System.Collections.Generic;
using BaseLib.Models.Interfaces;
using SharpHack.Base.Interfaces;
using System.Linq;
using SharpHack.Base.Data;
using NSubstitute.Core.Arguments;

namespace SharpHack.EngineTests;

[TestClass]
public class GameSessionTests
{
    [TestMethod]
    public void Initialize_CreatesEnemies()
    {
        // Arrange
        var mapGenerator = Substitute.For<IMapGenerator>();
        var random = Substitute.For<IRandom>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var enemyAI = Substitute.For<IEnemyAI>();
        var gamePersist = Substitute.For<IGamePersist>();
        var map = new Map(10, 10);
        map[1, 1].Type = TileType.Floor;
        map[5, 5].Type = TileType.Floor;
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>(),Arg.Any<Point>()).Returns(map);
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(5);

        // Act
        var session = new GameSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);

        // Assert
        Assert.IsNotNull(session.Enemies);
        Assert.IsNotEmpty(session.Enemies, "Enemies should be spawned.");
    }

    [TestMethod]
    public void MovePlayer_BlockedByEnemy_DoesNotMove()
    {
        // Arrange
        var mapGenerator = Substitute.For<IMapGenerator>();
        var random = Substitute.For<IRandom>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var enemyAI = Substitute.For<IEnemyAI>();
        var gamePersist = Substitute.For<IGamePersist>();
        var map = new Map(10, 10);

        map[1, 1].Type = TileType.Floor;
        map[2, 1].Type = TileType.Floor;

        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(2, 1, 0, 0);

        var session = new GameSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        session.Player.Position = new Point(1, 1);

        var enemy = session.Enemies.FirstOrDefault(e => e.Position.X == 2 && e.Position.Y == 1);
        Assert.IsNotNull(enemy);

        // Act
        session.MovePlayer(Direction.East);

        // Assert
        Assert.AreEqual(new Point(1, 1), session.Player.Position, "Player should not move into enemy.");
        combatSystem.Received().Attack(session.Player, enemy, Arg.Any<System.Action<string>>());
    }

    [TestMethod]
    public void MovePlayer_AttacksEnemy_DealsDamage()
    {
        // Arrange
        var mapGenerator = Substitute.For<IMapGenerator>();
        var random = Substitute.For<IRandom>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var enemyAI = Substitute.For<IEnemyAI>();
        var gamePersist = Substitute.For<IGamePersist>();
        var map = new Map(10, 10);

        map[1, 1].Type = TileType.Floor;
        map[2, 1].Type = TileType.Floor;

        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(2, 1, 0, 0);

        var session = new GameSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        session.Player.Position = new Point(1, 1);

        var enemy = session.Enemies.FirstOrDefault(e => e.Position.X == 2 && e.Position.Y == 1);
        Assert.IsNotNull(enemy);

        // Act
        session.MovePlayer(Direction.East);

        // Assert
        combatSystem.Received().Attack(session.Player, enemy, Arg.Any<System.Action<string>>());
    }

    [TestMethod]
    public void MovePlayer_KillsEnemy_RemovesFromMap()
    {
        // Arrange
        var mapGenerator = Substitute.For<IMapGenerator>();
        var random = Substitute.For<IRandom>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var enemyAI = Substitute.For<IEnemyAI>();
        var gamePersist = Substitute.For<IGamePersist>();
        var map = new Map(10, 10);

        map[1, 1].Type = TileType.Floor;
        map[2, 1].Type = TileType.Floor;

        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(2, 1, 0, 0);

        var session = new GameSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        session.Player.Position = new Point(1, 1);

        var enemy = session.Enemies.FirstOrDefault(e => e.Position.X == 2 && e.Position.Y == 1);
        Assert.IsNotNull(enemy);

        combatSystem.When(x => x.Attack(session.Player, enemy, Arg.Any<System.Action<string>>() ))
                    .Do(_ => enemy.HP = 0);

        // Act
        session.MovePlayer(Direction.East);

        // Assert
        Assert.DoesNotContain(enemy, session.Enemies, "Enemy should be removed from list.");
        Assert.IsNull(map[2, 1].Creature, "Enemy should be removed from map tile.");
    }

    [TestMethod]
    public void Update_MovesEnemies()
    {
        // Arrange
        var mapGenerator = Substitute.For<IMapGenerator>();
        var random = Substitute.For<IRandom>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var enemyAI = Substitute.For<IEnemyAI>();
        var gamePersist = Substitute.For<IGamePersist>();
        var map = new Map(10, 10);

        map[1, 1].Type = TileType.Floor;
        map[5, 5].Type = TileType.Floor;
        map[5, 4].Type = TileType.Floor;

        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(5);

        var session = new GameSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        var enemy = session.Enemies.First();

        enemyAI.GetNextMove(enemy, session.Player, map).Returns(new Point(5, 4));

        // Act
        session.Update();

        // Assert
        Assert.AreEqual(new Point(5, 4), enemy.Position);
        Assert.IsNull(map[5, 5].Creature);
        Assert.AreEqual(enemy, map[5, 4].Creature);
    }

    [TestMethod]
    public void PickUpItem_AddsToInventory_AndEquips()
    {
        // Arrange
        var mapGenerator = Substitute.For<IMapGenerator>();
        var random = Substitute.For<IRandom>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var enemyAI = Substitute.For<IEnemyAI>();
        var gamePersist = Substitute.For<IGamePersist>();
        var map = new Map(10, 10);

        map[1, 1].Type = TileType.Floor;
        var sword = new Weapon { Name = "Sword", AttackBonus = 5, Position = new Point(1, 1) };
        map[1, 1].Items.Add(sword);

        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(0);

        var session = new GameSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        session.Player.Position = new Point(1, 1);

        // Act
        session.PickUpItem(session.Player, sword);

        // Assert
        Assert.Contains(sword, session.Player.Inventory);
        Assert.AreEqual(sword, session.Player.MainHand);
        Assert.DoesNotContain(sword, map[1, 1].Items);
    }

    [TestMethod]
    public void NextLevel_PlacesStairsUp_AtEntryPosition()
    {
        // Arrange
        var mapGenerator = Substitute.For<IMapGenerator>();
        var random = Substitute.For<IRandom>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var gamePersist = Substitute.For<IGamePersist>();
        var enemyAI = Substitute.For<IEnemyAI>();

        var map1 = new Map(10, 10);
        map1[1, 1].Type = TileType.StairsDown;

        var map2 = new Map(10, 10);
        map2[1, 1].Type = TileType.Wall;

        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map1, map2);
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>(),Arg.Any<Point>()).Returns(map1, map2);
        random.Next(Arg.Any<int>()).Returns(0);

        var session = new GameSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);

        session.Player.Position = new Point(0, 1);
        map1[0, 1].Type = TileType.Floor;

        // Act
        session.MovePlayer(Direction.East);

        // Assert
        Assert.AreEqual(2, session.Level);
        Assert.AreEqual(new Point(1, 1), session.Player.Position);
        Assert.AreEqual(TileType.StairsUp, session.Map[1, 1].Type);
    }
}
