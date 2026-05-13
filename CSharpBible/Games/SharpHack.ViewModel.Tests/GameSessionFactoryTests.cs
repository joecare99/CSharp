using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SharpHack.Base.Interfaces;
using SharpHack.Base.Model;
using SharpHack.Engine;
using SharpHack.Base.Data;
using SharpHack.ViewModel;
using BaseLib.Models.Interfaces;
using System.Reflection;

namespace SharpHack.ViewModel.Tests;

[TestClass]
public class GameSessionFactoryTests
{
    [TestMethod]
    public void CreateSession_UsesProvidedSubstitutes()
    {
        var random = Substitute.For<IRandom>();
        var mapGenerator = Substitute.For<IMapGenerator>();
        var gamePersist = Substitute.For<IGamePersist>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var enemyAI = Substitute.For<IEnemyAI>();
        var map = new Map(10, 10);

        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Point?>()).Returns(map);

        var session = SharpHack.ViewModel.GameSessionFactory.CreateSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);

        Assert.IsNotNull(session);
        Assert.AreEqual(GameRunState.Running, session.RunState); // Ensure session is running
    }

    [TestMethod]
    public void CreateGameViewModel_ReturnsRunningViewModel()
    {
        var random = Substitute.For<IRandom>();
        var mapGenerator = Substitute.For<IMapGenerator>();
        var gamePersist = Substitute.For<IGamePersist>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var enemyAI = Substitute.For<IEnemyAI>();
        var map = new Map(10, 10);

        map[1, 1].Type = TileType.Floor;
        map[5, 5].Type = TileType.Floor;
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Point?>()).Returns(map);
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(5);

        var session = SharpHack.ViewModel.GameSessionFactory.CreateSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        var vm = SharpHack.ViewModel.GameSessionFactory.CreateGameViewModel(session);

        Assert.IsNotNull(vm);
        Assert.AreEqual(GameRunState.Running, vm.RunState);
        Assert.IsTrue(vm.IsRunning);
    }

    [TestMethod]
    public void CreateLayeredGameViewModel_ReturnsRunningViewModel()
    {
        var random = Substitute.For<IRandom>();
        var mapGenerator = Substitute.For<IMapGenerator>();
        var gamePersist = Substitute.For<IGamePersist>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var enemyAI = Substitute.For<IEnemyAI>();
        var map = new Map(10, 10);

        map[1, 1].Type = TileType.Floor;
        map[5, 5].Type = TileType.Floor;
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Point?>()).Returns(map);
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(map);
        random.Next(Arg.Any<int>()).Returns(5);

        var session = SharpHack.ViewModel.GameSessionFactory.CreateSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        var vm = SharpHack.ViewModel.GameSessionFactory.CreateLayeredGameViewModel(session);

        Assert.IsNotNull(vm);
        Assert.AreEqual(GameRunState.Running, vm.RunState);
        Assert.IsTrue(vm.IsRunning);
    }

    [DataTestMethod]
    [DataRow(GameRunState.PlayerDead)]
    [DataRow(GameRunState.Victory)]
    public void CreateSession_AfterTerminalRun_ReturnsFreshRunningSession(GameRunState terminalState)
    {
        var random = Substitute.For<IRandom>();
        var mapGenerator = Substitute.For<IMapGenerator>();
        var gamePersist = Substitute.For<IGamePersist>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var enemyAI = Substitute.For<IEnemyAI>();

        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Point?>()).Returns(_ => CreateFloorMap());
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(_ => CreateFloorMap());

        var previousSession = SharpHack.ViewModel.GameSessionFactory.CreateSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        typeof(GameSession).GetProperty(nameof(GameSession.RunState), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(previousSession, terminalState);

        var freshSession = SharpHack.ViewModel.GameSessionFactory.CreateSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        var freshViewModel = SharpHack.ViewModel.GameSessionFactory.CreateGameViewModel(freshSession);

        Assert.AreEqual(terminalState, previousSession.RunState);
        Assert.AreEqual(GameRunState.Running, freshSession.RunState);
        Assert.IsTrue(freshSession.IsRunning);
        Assert.AreEqual(GameRunState.Running, freshViewModel.RunState);
        Assert.IsTrue(freshViewModel.IsRunning);
    }

    [TestMethod]
    public void CreateSession_ReturnsIndependentStateForNewRun()
    {
        var random = Substitute.For<IRandom>();
        var mapGenerator = Substitute.For<IMapGenerator>();
        var gamePersist = Substitute.For<IGamePersist>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var enemyAI = Substitute.For<IEnemyAI>();
        var oldInventoryItem = Substitute.For<IItem>();

        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Point?>()).Returns(_ => CreateFloorMap());
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(_ => CreateFloorMap());

        var previousSession = SharpHack.ViewModel.GameSessionFactory.CreateSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        previousSession.Player.Inventory.Add(oldInventoryItem);
        previousSession.Player.Position = new Point(4, 4);

        var freshSession = SharpHack.ViewModel.GameSessionFactory.CreateSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);

        Assert.AreNotSame(previousSession, freshSession);
        Assert.AreNotSame(previousSession.Player, freshSession.Player);
        Assert.AreNotSame(previousSession.Map, freshSession.Map);
        Assert.AreEqual(1, previousSession.Player.Inventory.Count);
        Assert.AreEqual(0, freshSession.Player.Inventory.Count);
        Assert.AreEqual(0, freshSession.TurnsTaken);
    }

    [TestMethod]
    public void CreateGameViewModel_ForFreshSession_DoesNotReuseMessagesOrInventory()
    {
        var random = Substitute.For<IRandom>();
        var mapGenerator = Substitute.For<IMapGenerator>();
        var gamePersist = Substitute.For<IGamePersist>();
        var combatSystem = Substitute.For<ICombatSystem>();
        var enemyAI = Substitute.For<IEnemyAI>();
        var oldInventoryItem = Substitute.For<IItem>();

        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Point?>()).Returns(_ => CreateFloorMap());
        mapGenerator.Generate(Arg.Any<int>(), Arg.Any<int>()).Returns(_ => CreateFloorMap());

        var previousSession = SharpHack.ViewModel.GameSessionFactory.CreateSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        previousSession.Player.Inventory.Add(oldInventoryItem);
        var previousViewModel = SharpHack.ViewModel.GameSessionFactory.CreateGameViewModel(previousSession);
        previousViewModel.Messages.Add("stale");

        var freshSession = SharpHack.ViewModel.GameSessionFactory.CreateSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        var freshViewModel = SharpHack.ViewModel.GameSessionFactory.CreateGameViewModel(freshSession);

        Assert.AreEqual(1, previousViewModel.Inventory.Count);
        Assert.AreEqual(1, previousViewModel.Messages.Count);
        Assert.AreEqual(0, freshViewModel.Inventory.Count);
        Assert.AreEqual(0, freshViewModel.Messages.Count);
        Assert.AreEqual(GameRunState.Running, freshViewModel.RunState);
    }

    private static Map CreateFloorMap()
    {
        var map = new Map(10, 10);

        for (var x = 0; x < map.Width; x++)
        {
            for (var y = 0; y < map.Height; y++)
            {
                map[x, y].Type = TileType.Floor;
            }
        }

        return map;
    }
}
