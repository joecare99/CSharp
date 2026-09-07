using ConsoleLib.Showcase.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Showcase.Tests;

[TestClass]
public sealed class MonthGridTests
{
    [TestMethod]
    public void FebruaryLeapYear_HasTwentyNineDaysAndMondayFirstOffset()
    {
        var grid = MonthGrid.For(2024, 2);

        Assert.AreEqual(29, grid.DayCount);
        Assert.AreEqual(5, grid.WeekCount);
        Assert.AreEqual(1, grid.Cell(0, 3));
        Assert.AreEqual(29, grid.Cell(4, 3));
    }

    [TestMethod]
    public void MonthWithSixWeeks_IsPaddedToCompleteRows()
    {
        var grid = MonthGrid.For(2025, 3);

        Assert.AreEqual(6, grid.WeekCount);
        Assert.AreEqual(31, grid.Cell(5, 0));
        Assert.AreEqual(0, grid.Cells[grid.Cells.Length - 1]);
    }

    [TestMethod]
    public void MonthNavigation_WrapsYearBoundaries()
    {
        Assert.AreEqual((2025, 12), MonthGrid.PreviousMonth(2026, 1));
        Assert.AreEqual((2027, 1), MonthGrid.NextMonth(2026, 12));
    }
}
