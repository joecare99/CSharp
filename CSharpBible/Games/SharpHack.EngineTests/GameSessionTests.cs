using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SharpHack.Engine;
using SharpHack.LevelGen;
using SharpHack.Base.Model;
using System.Collections.Generic;
using BaseLib.Models.Interfaces;
using SharpHack.Base.Interfaces;
using System.Linq; // Add using

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
        var enemyAI = Substitute.For<IEnemyAI>(); // Mock AI
        var map = new Map(10, 10);
        map[1, 1].Type = TileType.Floor; 
        map[5, 5].Type = TileType.Floor;
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(5); 

        // Act
        var session = new GameSession(mapGenerator, random, combatSystem, enemyAI); // Pass mock

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
        var enemyAI = Substitute.For<IEnemyAI>();
        var map = new Map(10, 10);
        
        map[1, 1].Type = TileType.Floor;
        map[2, 1].Type = TileType.Floor;
        
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(2, 1, 0, 0); 

        var session = new GameSession(mapGenerator, random, combatSystem, enemyAI);
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
        // Arrange
        var mapGenerator = Substitute.For<IMapGenerator>();
        var random = Substitute.For<IRandom>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var enemyAI = Substitute.For<IEnemyAI>();
        var map = new Map(10, 10);
        
        map[1, 1].Type = TileType.Floor;
        map[2, 1].Type = TileType.Floor;
        
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(2, 1, 0, 0);

        var session = new GameSession(mapGenerator, random, combatSystem, enemyAI);
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
        var enemyAI = Substitute.For<IEnemyAI>();
        var map = new Map(10, 10);
        
        map[1, 1].Type = TileType.Floor;
        map[2, 1].Type = TileType.Floor;
        
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(2, 1, 0, 0);

        var session = new GameSession(mapGenerator, random, combatSystem, enemyAI);
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

    [TestMethod]
    public void Update_MovesEnemies()
    {
        // Arrange
        var mapGenerator = Substitute.For<IMapGenerator>();
        var random = Substitute.For<IRandom>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var enemyAI = Substitute.For<IEnemyAI>();
        var map = new Map(10, 10);
        
        map[1, 1].Type = TileType.Floor;
        map[5, 5].Type = TileType.Floor; // Enemy start
        map[5, 4].Type = TileType.Floor; // Enemy dest
        
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(5); // Spawn at 5,5

        var session = new GameSession(mapGenerator, random, combatSystem, enemyAI);
        var enemy = session.Enemies.First();
        
        // Mock AI to move enemy North
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
        var map = new Map(10, 10);
        
        map[1, 1].Type = TileType.Floor;
        var sword = new Weapon { Name = "Sword", AttackBonus = 5, Position = new Point(1, 1) };
        map[1, 1].Items.Add(sword);
        
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(0); // No enemies spawned or items spawned by Initialize

        var session = new GameSession(mapGenerator, random, combatSystem, enemyAI);
        session.Player.Position = new Point(1, 1); // Player starts on item (or moves to it)

        // Act
        // Simulate move to 1,1 (or just call PickUp directly if player is already there)
        // Since Initialize puts player at 1,1, let's just call PickUp
        session.PickUpItem(session.Player, sword);

        // Assert
        Assert.IsTrue(session.Player.Inventory.Contains(sword));
        Assert.AreEqual(sword, session.Player.MainHand);
        Assert.IsFalse(map[1, 1].Items.Contains(sword));
    }

    [TestMethod]
    public void NextLevel_PlacesStairsUp_AtEntryPosition()
    {
        // Arrange
        var mapGenerator = Substitute.For<IMapGenerator>();
        var random = Substitute.For<IRandom>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var enemyAI = Substitute.For<IEnemyAI>();
        
        var map1 = new Map(10, 10);
        map1[1, 1].Type = TileType.StairsDown; // Player is here
        
        var map2 = new Map(10, 10);
        map2[1, 1].Type = TileType.Wall; // Initially a wall, should be forced to floor/stairs
        
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map1, map2); // Return map1 first, then map2
        random.Next(Arg.Any<int>()).Returns(0);

        var session = new GameSession(mapGenerator, random, combatSystem, enemyAI);
        session.Player.Position = new Point(1, 1);

        // Act
        // Trigger NextLevel via MovePlayer onto StairsDown
        session.MovePlayer(Direction.East); // Assuming player moves onto stairs? No, setup says player IS at 1,1.
        // Wait, MovePlayer checks destination. If player is at 0,1 and moves East to 1,1 (StairsDown).
        
        // Let's adjust setup: Player at 0,1. Stairs at 1,1.
        session.Player.Position = new Point(0, 1);
        map1[0, 1].Type = TileType.Floor;
        
        session.MovePlayer(Direction.East); // Move to 1,1

        // Assert
        Assert.AreEqual(2, session.Level);
        Assert.AreEqual(new Point(1, 1), session.Player.Position);
        Assert.AreEqual(TileType.StairsUp, session.Map[1, 1].Type);
    }
}
