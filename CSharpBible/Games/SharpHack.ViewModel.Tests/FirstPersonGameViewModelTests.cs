using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SharpHack.Base.Model;
using SharpHack.Base.Interfaces;
using SharpHack.Engine;
using SharpHack.ViewModel;
using BaseLib.Models.Interfaces;

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

        var map = Substitute.For<IMap>();
        map.Width.Returns(10);
        map.Height.Returns(10);
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
        _vm.Player.Position.Returns(playerPos);
        
        var targetTile = Substitute.For<ITile>();
        _vm.Map.IsValid(5, 4).Returns(true);
        _vm.Map[5, 4].Returns(targetTile);

        // Act
        var result = _vm.GetRelativeTile(1, 0); // 1 block forward

        // Assert
        Assert.AreSame(targetTile, result);
    }
}
