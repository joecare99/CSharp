using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SharpHack.Base.Data;
using SharpHack.Base.Model;
using SharpHack.Base.Interfaces;
using SharpHack.Engine;
using SharpHack.ViewModel;
using BaseLib.Models.Interfaces;
using System.Reflection;

namespace SharpHack.ViewModel.Tests;

[TestClass]
public class FirstPersonGameViewModelTests
{
    private GameSession _session;
    private IRandom _random;
    private IMapGenerator _generator;
    private ICombatSystem _combat;
    private IEnemyAI _ai;
    private IGamePersist _persist;

    private FirstPersonGameViewModel _vm;

    [TestInitialize]
    public void Setup()
    {
        _random = Substitute.For<IRandom>();
        _generator = Substitute.For<IMapGenerator>();
        _combat = Substitute.For<ICombatSystem>();
        _ai = Substitute.For<IEnemyAI>();
        _persist = Substitute.For<IGamePersist>();
        var _mapfields = new Dictionary<(int, int), Tile>(); 
        var map = Substitute.For<IMap>();
        map.Width.Returns(10);
        map.Height.Returns(10);
        map[Arg.Any<int>(), Arg.Any<int>()].Returns((ci) => _mapfields.TryGetValue(((int)ci.Args()[0], (int)ci.Args()[1]), out var tile) ? tile : _mapfields[((int)ci.Args()[0], (int)ci.Args()[1])] = new Tile());
        map[Arg.Any<Point>()].Returns((ci) => _mapfields.TryGetValue((((Point)ci.Args()[0]).X, ((Point)ci.Args()[0]).Y), out var tile) ? tile : _mapfields[(((Point)ci.Args()[0]).X, ((Point)ci.Args()[0]).Y)] =  new Tile());
        _generator.Generate(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Point?>()).Returns(map);

        _session = new GameSession(_generator, _persist ,_random, _combat, _ai);
        _vm = new FirstPersonGameViewModel(_session);
    }

    [TestMethod]
    public void RotateLeft_ChangesDirectionCorrecty()
    {
        // Act
        _vm.FacingDirection = Direction.North;
        _vm.RotateLeft();

        // Assert
        Assert.AreEqual(Direction.West, _vm.FacingDirection);
    }

    [TestMethod]
    public void RotateRight_ChangesDirectionCorrecty()
    {
        // Act
        _vm.FacingDirection = Direction.North;
        _vm.RotateRight();

        // Assert
        Assert.AreEqual(Direction.East, _vm.FacingDirection);
    }

    [TestMethod]
    public void GetRelativeTile_NorthFacing_ReturnsCorrectTile()
    {
        // Arrange
        _vm.FacingDirection = Direction.North;
        var playerPos = new Point(5, 5);
        _vm.Player.Position=playerPos;
        
        var targetTile = _vm.Map[5, 4];
        _vm.Map.IsValid(5, 4).Returns(true);
        _vm.Map[5, 4].Returns(targetTile);

        // Act
        var result = _vm.GetRelativeTile(1, 0); // 1 block forward

        // Assert
        Assert.AreSame(targetTile, result);
    }

    [TestMethod]
    public void GameViewModels_ExposeRunningStateFromSession()
    {
        var gameViewModel = new GameViewModel(_session);
        var layeredViewModel = new LayeredGameViewModel(_session);

        Assert.AreEqual(GameRunState.Running, gameViewModel.RunState);
        Assert.IsTrue(gameViewModel.IsRunning);
        Assert.AreEqual(GameRunState.Running, layeredViewModel.RunState);
        Assert.IsTrue(layeredViewModel.IsRunning);
    }

    [TestMethod]
    public void GameViewModel_UsesTerminalRunStateAndBlocksMovement()
    {
        typeof(GameSession).GetProperty(nameof(GameSession.RunState), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(_session, GameRunState.PlayerDead);

        var viewModel = new GameViewModel(_session);
        var startPosition = _session.Player.Position;

        viewModel.Move(Direction.East);

        Assert.AreEqual(GameRunState.PlayerDead, viewModel.RunState);
        Assert.IsFalse(viewModel.IsRunning);
        Assert.AreEqual(startPosition, _session.Player.Position);
    }

    [TestMethod]
    public void GameViewModels_ExposeVictorySummaryFromSession()
    {
        typeof(GameSession).GetProperty(nameof(GameSession.RunState), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(_session, GameRunState.Victory);

        var gameViewModel = new GameViewModel(_session);
        var layeredViewModel = new LayeredGameViewModel(_session);

        Assert.AreEqual(GameRunState.Victory, gameViewModel.RunState);
        Assert.IsTrue(gameViewModel.HasWon);
        Assert.IsFalse(gameViewModel.IsRunning);
        Assert.IsFalse(string.IsNullOrWhiteSpace(gameViewModel.CompletionSummary));
        Assert.AreEqual(GameRunState.Victory, layeredViewModel.RunState);
        Assert.IsTrue(layeredViewModel.HasWon);
        Assert.IsFalse(layeredViewModel.IsRunning);
        Assert.IsFalse(string.IsNullOrWhiteSpace(layeredViewModel.CompletionSummary));
    }

    [TestMethod]
    public void GameViewModels_ExposeTurnCountFromSession()
    {
        _session.MovePlayer(Direction.East);

        var gameViewModel = new GameViewModel(_session);
        var layeredViewModel = new LayeredGameViewModel(_session);

        Assert.AreEqual(_session.TurnsTaken, gameViewModel.TurnsTaken);
        Assert.AreEqual(_session.TurnsTaken, layeredViewModel.TurnsTaken);
    }

    [TestMethod]
    public void DisposedGameViewModel_DetachesFromSessionMessages()
    {
        var viewModel = new GameViewModel(_session);
        _session.Player.HP = 0;

        viewModel.Dispose();
        _session.MovePlayer(Direction.East);

        Assert.AreEqual(0, viewModel.Messages.Count);
    }

    [TestMethod]
    public void DisposedLayeredGameViewModel_DetachesFromSessionMessages()
    {
        var viewModel = new LayeredGameViewModel(_session);
        _session.Player.HP = 0;

        viewModel.Dispose();
        _session.MovePlayer(Direction.East);

        Assert.AreEqual(0, viewModel.Messages.Count);
    }

    [TestMethod]
    public void GameViewModel_SaveAndLoadCommands_UseInjectedService()
    {
        var saveLoadService = Substitute.For<IGameSaveLoadService>();
        saveLoadService.CanSave.Returns(true);
        saveLoadService.CanLoad.Returns(true);
        var viewModel = new GameViewModel(_session, saveLoadService);

        Assert.IsTrue(viewModel.SaveGameCommand.CanExecute(null));
        Assert.IsTrue(viewModel.LoadGameCommand.CanExecute(null));

        viewModel.SaveGameCommand.Execute(null);
        viewModel.LoadGameCommand.Execute(null);

        saveLoadService.Received(1).Save();
        saveLoadService.Received(1).Load();
    }

    [TestMethod]
    public void LayeredGameViewModel_SaveAndLoadCommands_UseInjectedService()
    {
        var saveLoadService = Substitute.For<IGameSaveLoadService>();
        saveLoadService.CanSave.Returns(true);
        saveLoadService.CanLoad.Returns(true);
        var viewModel = new LayeredGameViewModel(_session, saveLoadService);

        Assert.IsTrue(viewModel.SaveGameCommand.CanExecute(null));
        Assert.IsTrue(viewModel.LoadGameCommand.CanExecute(null));

        viewModel.SaveGameCommand.Execute(null);
        viewModel.LoadGameCommand.Execute(null);

        saveLoadService.Received(1).Save();
        saveLoadService.Received(1).Load();
    }
}
