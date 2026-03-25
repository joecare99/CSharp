using Galaxia.Helper;
using BaseLib.Models.Interfaces;
using NSubstitute;
using System.Diagnostics;

namespace Galaxia.Models.Tests;

[TestClass()]
public class SpaceTests
{
    private IRandom rnd;
    private int _rnd = 0;

    [TestMethod()]
    public void InitializeTest()
    {
        var space = new Space();
        _rnd = 0;
        NamingHelper.SetRandom(rnd = Substitute.For<IRandom>());
        rnd.Next(Arg.Any<int>()).Returns(x => _rnd++ % x.ArgAt<int>(0));
        space.Initialize();
        Assert.IsNotNull(space.Sectors);
        Assert.HasCount(24, space.Sectors);
        foreach (var sector in space.Sectors.Values)
        {
            Assert.IsNotNull(sector);
            Assert.IsNotNull(sector.Starsystems);
            Assert.HasCount(2, sector.Starsystems);
            foreach (var star in sector.Starsystems)
            {
                Assert.IsNotNull(star);
                Assert.IsFalse(string.IsNullOrEmpty(star.Name));
                Assert.Contains(star.Name, NamingHelper.existingNames);
                Debug.WriteLine($"Star System: {star.Name} at {star.Position.X},{star.Position.Y},{star.Position.Z} pop:{star.Population} res:{star.Resources}");
            }
        }
    }
}