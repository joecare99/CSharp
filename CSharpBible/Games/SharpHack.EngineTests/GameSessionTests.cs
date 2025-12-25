using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SharpHack.Engine;
using SharpHack.LevelGen;
using SharpHack.Base.Model;
using System.Collections.Generic;
using BaseLib.Models.Interfaces;
using SharpHack.Base.Interfaces; // Add using

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
        var combatSystem = Substitute.For<ICombatSystem>(); // Mock combat system
        var map = new Map(10, 10);
        map[1, 1].Type = TileType.Floor; 
        map[5, 5].Type = TileType.Floor;
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(5); 

        // Act
        var session = new GameSession(mapGenerator, random, combatSystem); // Pass mock

        // Assert
        Assert.IsNotNull(session.Enemies);
        Assert.IsTrue(session.Enemies.Count > 0, "Enemies should be spawned.");
    }

    [TestMethod]
    public void MovePlayer_BlockedByEnemy_DoesNotMove()
    {
        // Arrange
        var mapGenerator = Substitute.For<IMapGenerator>();
        var random = Substitute.For<IRandom>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var map = new Map(10, 10);
        
        map[1, 1].Type = TileType.Floor;
        map[2, 1].Type = TileType.Floor;
        
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(2, 1, 0, 0); 

        var session = new GameSession(mapGenerator, random, combatSystem);
        session.Player.Position = new Point(1, 1);
        
        var enemy = session.Enemies.Find(e => e.Position.X == 2 && e.Position.Y == 1);
        Assert.IsNotNull(enemy);

        // Act
        session.MovePlayer(Direction.East);

        // Assert
        Assert.AreEqual(new Point(1, 1), session.Player.Position, "Player should not move into enemy.");
        // Verify combat system was called
        combatSystem.Received().Attack(session.Player, enemy, Arg.Any<System.Action<string>>());
    }

    [TestMethod]
    public void MovePlayer_AttacksEnemy_DealsDamage()
    {
        // This test is now testing interaction with ICombatSystem, not the damage calculation itself
        // Arrange
        var mapGenerator = Substitute.For<IMapGenerator>();
        var random = Substitute.For<IRandom>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var map = new Map(10, 10);
        
        map[1, 1].Type = TileType.Floor;
        map[2, 1].Type = TileType.Floor;
        
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(2, 1, 0, 0);

        var session = new GameSession(mapGenerator, random, combatSystem);
        session.Player.Position = new Point(1, 1);
        
        var enemy = session.Enemies.Find(e => e.Position.X == 2 && e.Position.Y == 1);

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
        var map = new Map(10, 10);
        
        map[1, 1].Type = TileType.Floor;
        map[2, 1].Type = TileType.Floor;
        
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(2, 1, 0, 0);

        var session = new GameSession(mapGenerator, random, combatSystem);
        session.Player.Position = new Point(1, 1);
        
        var enemy = session.Enemies.Find(e => e.Position.X == 2 && e.Position.Y == 1);
        
        // Simulate combat system killing the enemy
        combatSystem.When(x => x.Attack(session.Player, enemy, Arg.Any<System.Action<string>>()))
                    .Do(x => enemy.HP = 0);

        // Act
        session.MovePlayer(Direction.East);

        // Assert
        Assert.IsFalse(session.Enemies.Contains(enemy), "Enemy should be removed from list.");
        Assert.IsNull(map[2, 1].Creature, "Enemy should be removed from map tile.");
    }
}
