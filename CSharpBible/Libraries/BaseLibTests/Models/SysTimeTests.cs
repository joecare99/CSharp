using BaseLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BaseLib.Models.Tests;

[TestClass]
public class SysTimeTests
{
    private SysTime _sut = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _sut = new SysTime();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        SysTime.GetNow = () => DateTime.Now;
    }

    [TestMethod]
    public void Now_ReturnsCurrentDateTime()
    {
        // Arrange
        var expectedDate = new DateTime(2024, 6, 15, 10, 30, 45);
        SysTime.GetNow = () => expectedDate;

        // Act
        var result = _sut.Now;

        // Assert
        Assert.AreEqual(expectedDate, result);
    }

    [TestMethod]
    public void Today_ReturnsDatePartOnly()
    {
        // Arrange
        var dateWithTime = new DateTime(2024, 6, 15, 10, 30, 45);
        var expectedDate = new DateTime(2024, 6, 15);
        SysTime.GetNow = () => dateWithTime;

        // Act
        var result = _sut.Today;

        // Assert
        Assert.AreEqual(expectedDate, result);
    }

    [TestMethod]
    public void GetNow_DefaultValue_ReturnsDateTimeNow()
    {
        // Arrange
        SysTime.GetNow = () => DateTime.Now;

        // Act
        var before = DateTime.Now;
        var result = _sut.Now;
        var after = DateTime.Now;

        // Assert
        Assert.IsTrue(result >= before && result <= after);
    }

    [TestMethod]
    public void GetNow_CanBeOverridden()
    {
        // Arrange
        var customDate = new DateTime(2000, 1, 1);
        SysTime.GetNow = () => customDate;

        // Act
        var result = SysTime.GetNow();

        // Assert
        Assert.AreEqual(customDate, result);
    }
}