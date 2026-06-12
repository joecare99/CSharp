using System;
using AA40_Wizzard.Model;
using NSubstitute;

namespace AA40_Wizzard.Tests.Models;

[TestClass]
public class WizzardModelTests
{
    [TestMethod]
    public void ConstructorInitializesDefaultsAndLogsStartupTest()
    {
        var clock = Substitute.For<ISystemClock>();
        clock.Now.Returns(new DateTime(2025, 1, 2, 3, 4, 5));
        var log = Substitute.For<ILogSink>();

        using var model = new WizzardModel(clock, log);

        Assert.AreEqual(-1, model.MainSelection);
        Assert.AreEqual(-1, model.SubSelection);
        Assert.AreEqual(-1, model.Additional1);
        Assert.AreEqual(8, model.MainOptions.Count);
        Assert.AreEqual(8, model.SubOptions.Count);
        Assert.AreEqual(8, model.AdditOptions.Count);
        Assert.AreEqual(new DateTime(2025, 1, 2, 3, 4, 5), model.Now);
        log.Received().Log("WizzardModel created");
    }

    [TestMethod]
    public void MainSelectionResetsAdditionalSelectionsTest()
    {
        var model = new WizzardModel(Substitute.For<ISystemClock>(), Substitute.For<ILogSink>())
        {
            Additional1 = 1,
            Additional2 = 2,
            Additional3 = 3,
        };

        model.MainSelection = 5;

        Assert.AreEqual(-1, model.Additional1);
        Assert.AreEqual(-1, model.Additional2);
        Assert.AreEqual(-1, model.Additional3);
    }

    [TestMethod]
    public void DisposeLogsShutdownTest()
    {
        var log = Substitute.For<ILogSink>();
        var model = new WizzardModel(Substitute.For<ISystemClock>(), log);

        model.Dispose();

        log.Received().Log("WizzardModel stopped");
    }
}
