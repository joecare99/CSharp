using System.Numerics;
using AsteroidsModern.Engine.Abstractions;
using AsteroidsModern.Engine.Services;
using AsteroidsModern.Engine.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AsteroidsModernEngine.Tests.Services;

[TestClass]
public class GameWorldTests
{
    private IRandom _rand = null!;
    private IGameInput _input = null!;
    private ITimeProvider _time = null!;
    private ISound _sound = null!;
    private int __rand = 0;

    [TestInitialize]
    public void Init()
    {
        _rand = Substitute.For<IRandom>();
        __rand = 0;
        _rand.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(x  => __rand++ % (int)x[1] ); 
        _rand.NextSingle().Returns(x  => (float)Math.Exp(__rand++) * 1009f / 367f % 1f ); 
        _input = Substitute.For<IGameInput>();
        _time = Substitute.For<ITimeProvider>();
        _sound = Substitute.For<ISound>();
    }

    private GameWorld CreateWorld(Vector2 size)
    {
        var gw = new GameWorld(_rand) { WorldSize = size };
        gw.Reset();
        return gw;
    }

    [TestMethod]
    public void Reset_ShouldPlaceShipCenterAndSpawnAsteroids()
    {
        var gw = CreateWorld(new(100, 80));
        Assert.AreEqual(new Vector2(50, 40), gw.Ship.Position);
        Assert.IsNotEmpty(gw.Asteroids);
        Assert.IsEmpty(gw.Bullets);
    }

    [TestMethod]
    public void Update_Thrust_IncreasesVelocityAndCallsSound()
    {
        var gw = CreateWorld(new(200, 200));

        _input.IsDown(GameKey.Thrust).Returns(true);
        _time.DeltaTime.Returns(0.5);

        var velBefore = gw.Ship.Velocity;
        gw.Update(_input, _time, _sound);
        Assert.IsGreaterThan(velBefore.Length(), gw.Ship.Velocity.Length());
        _sound.Received().PlayThrust();
    }

    [TestMethod]
    public void Update_Rotate_LeftRightChangesRotation()
    {
        var gw = CreateWorld(new(200, 200));
        _time.DeltaTime.Returns(1.0);

        _input.IsDown(GameKey.Left).Returns(true);
        gw.Update(_input, _time, _sound);
        var rotAfterLeft = gw.Ship.Rotation;

        var input2 = Substitute.For<IGameInput>();
        input2.IsDown(GameKey.Right).Returns(true);
        gw.Update(input2, _time, _sound);
        Assert.AreNotEqual(rotAfterLeft, gw.Ship.Rotation);
    }

    [TestMethod]
    public void Fire_SpawnsBullet_WithCooldown()
    {
        var gw = CreateWorld(new(300, 300));

        _input.IsDown(GameKey.Fire).Returns(true);
        _time.DeltaTime.Returns(0.016);

        // t=0, first shot
        _time.TotalTime.Returns(0.0);
        gw.Update(_input, _time, _sound);
        Assert.HasCount(1, gw.Bullets);
        _sound.Received().PlayShoot();

        // t=0.1, still cooldown, no extra bullet
        _time.TotalTime.Returns(0.1);
        gw.Update(_input, _time, _sound);
        Assert.HasCount(1, gw.Bullets);

        // t=0.2, second shot
        _time.TotalTime.Returns(0.2);
        gw.Update(_input, _time, _sound);
        Assert.HasCount(2, gw.Bullets);
    }

    [TestMethod]
    public void Bullets_Expire_ByLife()
    {
        var gw = CreateWorld(new(300, 300));

        _input.IsDown(GameKey.Fire).Returns(true);
        _time.DeltaTime.Returns(0.016);
        _time.TotalTime.Returns(0.0);
        gw.Update(_input, _time, _sound);
        Assert.HasCount(1, gw.Bullets);

        // Advance life beyond bullet life
        for (int i = 0; i < 100; i++)
        {
            _time.TotalTime.Returns(i * 0.05);
            gw.Update(_input, _time, _sound);
        }
        Assert.IsEmpty(gw.Bullets);
    }

    [TestMethod]
    public void Wrap_ShouldWrapShipAndEntities()
    {
        var gw = CreateWorld(new(100, 100));
        _time.DeltaTime.Returns(1.0);

        gw.Ship.Position = new Vector2(-1, 50);
        gw.Update(_input, _time, _sound);
        Assert.IsTrue(gw.Ship.Position.X >= 0 && gw.Ship.Position.X <= 100);
    }

    private static (List<Asteroid> asteroids, List<Bullet> bullets) GetSimLists(GameWorld gw)
    {
        var simField = typeof(GameWorld).GetField("_simulation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var sim = simField!.GetValue(gw)!;
        var astField = sim.GetType().GetField("_asteroids", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var bulletField = sim.GetType().GetField("_bullets", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return ((List<Asteroid>)astField!.GetValue(sim)!, (List<Bullet>)bulletField!.GetValue(sim)!);
    }

    [TestMethod]
    public void Collision_ShipWithAsteroid_ResetsWorld()
    {
        var gw = CreateWorld(new(200, 200));
        _time.DeltaTime.Returns(0.016);

        // move ship away from center    
        gw.Ship.Position = new(104, 98);
        // place an asteroid at ship
        var (asteroids, _) = GetSimLists(gw);
        asteroids.Clear();
        asteroids.Add(new Asteroid { Position = gw.Ship.Position, Velocity = Vector2.Zero, Radius = 20 });

        gw.Update(_input, _time, _sound);
        _sound.Received().PlayBang();
        Assert.AreEqual(new Vector2(100, 100), gw.Ship.Position); // reset moved ship
    }

    [TestMethod]
    public void Collision_BulletWithAsteroid_SplitsOrRemoves()
    {
        var gw = CreateWorld(new(200, 200));
        _time.DeltaTime.Returns(0.016);

        // move ship away from center    
        gw.Ship.Position = new(1, 1);
        // prepare one large asteroid and one bullet colliding
        var (asteroids, bullets) = GetSimLists(gw);
        asteroids.Clear();
        bullets.Clear();
        asteroids.Add(new Asteroid { Position = new(100, 100), Velocity = Vector2.Zero, Radius = 30 });
        bullets.Add(new Bullet { Position = new(100, 100), Velocity = Vector2.Zero, Radius = 2 });

        gw.Update(_input, _time, _sound);
        _sound.Received().PlayBang();
        Assert.IsGreaterThanOrEqualTo(1, gw.Asteroids.Count); // split into 2 if large, or removed if small
        Assert.IsEmpty(gw.Bullets);
    }

    [TestMethod]
    [DataRow(0, 0,DisplayName ="Ship only")]
    [DataRow(3, 0, DisplayName = "3 Asteroids")]
    [DataRow(0, 5, DisplayName = "5 Bullets")]
    [DataRow(4, 7, DisplayName = "4 Asteroids, 7 Bullets")]
    public void Render_CallsContext(int nA, int nB)
    {
        // Arrange
        var gw = CreateWorld(new(200, 200));
        var ctx = Substitute.For<IRenderContext>();

        var (asteroids, bullets) = GetSimLists(gw);
        asteroids.Clear();
        bullets.Clear();
        for (int i = 0; i < nA; i++)
            asteroids.Add(new Asteroid { Position = new(10 + i * 20, 50), Velocity = Vector2.Zero, Radius = 10 + i * 5 });
        for (int i = 0; i < nB; i++)
            bullets.Add(new Bullet { Position = new(100, 100), Velocity = Vector2.Zero, Radius = 2 });

        gw.Render(ctx);
        ctx.Received().Clear(Arg.Any<Color>());
        ctx.Received(1).DrawPolygon(Arg.Any<Vector2[]>(), Arg.Any<Color>(), Arg.Any<float>());
        ctx.Received(nA+nB).DrawCircle(Arg.Any<Vector2>(), Arg.Any<float>(), Arg.Any<Color>(), Arg.Any<float>(), Arg.Any<int>());
    }
}
