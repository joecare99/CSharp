using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BaseLib.Helper.Tests;
[TestClass]
public class SysTimeTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private Func<DateTime> _gnfunc;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init()
    {
        _gnfunc = SysTime.GetNow;
        Assert.IsNotNull(_gnfunc?.Invoke());
        SysTime.GetNow = () => new DateTime(2021, 1, 1, 12, 01, 59);
    }

    [TestCleanup]
    public void CleanUp()
    {
        SysTime.GetNow = _gnfunc;
    }


    [TestMethod]
    public void NowTest()
    {
        // Arrange
        var sysTime = new SysTime();
        // Act
        var result = sysTime.Now;
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(new DateTime(2021, 1, 1, 12, 01, 59), result);
    }

    [TestMethod]
    public void TodayTest()
    {
        // Arrange
        var sysTime = new SysTime();
        // Act
        var result = sysTime.Today;
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(new DateTime(2021, 1, 1), result);
    }
}
