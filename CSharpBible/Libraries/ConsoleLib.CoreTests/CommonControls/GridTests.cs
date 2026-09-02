using ConsoleLib.CommonControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace ConsoleLib.Tests;

[TestClass]
public class GridTests
{
    [TestMethod]
    public void Grid_UsesFirstCellForUnconfiguredChild()
    {
        var grid = CreateGrid(10, 6);
        var child = new Label { size = new Size(1, 1) };

        grid.Add(child);

        Assert.AreEqual(new Rectangle(0, 0, 5, 3), child.Dimension);
        Assert.AreEqual(0, Grid.GetRow(child));
        Assert.AreEqual(0, Grid.GetColumn(child));
        Assert.AreEqual(1, Grid.GetRowSpan(child));
        Assert.AreEqual(1, Grid.GetColumnSpan(child));
    }

    [TestMethod]
    public void Grid_AppliesAttachedPropertiesSetAfterAdd()
    {
        var grid = CreateGrid(10, 6);
        var child = new Label { size = new Size(1, 1) };
        grid.Add(child);

        Grid.SetRow(child, 1);
        Grid.SetColumn(child, 1);

        Assert.AreEqual(new Rectangle(5, 3, 5, 3), child.Dimension);
    }

    [TestMethod]
    public void Grid_SpansRemainingRowsAndColumns()
    {
        var grid = CreateGrid(12, 8);
        var child = new Label { size = new Size(1, 1) };
        Grid.SetRow(child, 0);
        Grid.SetColumn(child, 0);
        Grid.SetRowSpan(child, 2);
        Grid.SetColumnSpan(child, 2);

        grid.Add(child);

        Assert.AreEqual(new Rectangle(0, 0, 12, 8), child.Dimension);
    }

    [TestMethod]
    public void Grid_ClampsOutOfRangeAttachedProperties()
    {
        var grid = CreateGrid(10, 6);
        var child = new Label { size = new Size(1, 1) };
        Grid.SetRow(child, 99);
        Grid.SetColumn(child, 99);
        Grid.SetRowSpan(child, 0);
        Grid.SetColumnSpan(child, 0);

        grid.Add(child);

        Assert.AreEqual(99, Grid.GetRow(child));
        Assert.AreEqual(99, Grid.GetColumn(child));
        Assert.AreEqual(1, Grid.GetRowSpan(child));
        Assert.AreEqual(1, Grid.GetColumnSpan(child));
        Assert.AreEqual(new Rectangle(5, 3, 5, 3), child.Dimension);
    }

    [TestMethod]
    public void Grid_RemovingChildStopsFurtherLayoutNotifications()
    {
        var grid = CreateGrid(10, 6);
        var removed = new Label { size = new Size(1, 1) };
        var remaining = new Label { size = new Size(1, 1) };
        grid.Add(removed);
        grid.Add(remaining);
        grid.Remove(removed);
        var layoutAfterRemove = remaining.Dimension;

        removed.size = new Size(8, 8);

        Assert.AreEqual(layoutAfterRemove, remaining.Dimension);
        Assert.IsNull(removed.Parent);
    }

    [TestMethod]
    public void Grid_AlignsNonStretchingChildInsideItsCell()
    {
        var grid = CreateGrid(10, 6);
        grid.HorizontalContentAlignment = HorizontalAlignment.Center;
        grid.VerticalContentAlignment = VerticalAlignment.Bottom;
        var child = new Label { size = new Size(2, 1) };
        Grid.SetRow(child, 1);
        Grid.SetColumn(child, 1);

        grid.Add(child);

        Assert.AreEqual(new Rectangle(6, 5, 2, 1), child.Dimension);
    }

    [TestMethod]
    public void Grid_PreservesAttachedPlacementWhenChildrenAreAddedAndGridIsResized()
    {
        var grid = CreateGrid(10, 6);
        var first = new Label { Text = "A", size = new Size(1, 1) };
        var second = new Label { Text = "B", size = new Size(1, 1) };
        Grid.SetRow(first, 0);
        Grid.SetColumn(first, 1);
        Grid.SetRow(second, 1);
        Grid.SetColumn(second, 0);

        Assert.AreEqual(0, Grid.GetRow(first));
        Assert.AreEqual(1, Grid.GetRow(second));
        Assert.AreEqual(new Size(10, 6), grid.size);
        grid.Add(first);
        Assert.AreEqual(0, Grid.GetRow(first));
        Assert.AreEqual(1, Grid.GetColumn(first));
        Assert.AreEqual(new Rectangle(5, 0, 5, 3), first.Dimension, $"after first add={first.Dimension}");
        grid.Add(second);
        Assert.AreEqual(new Rectangle(5, 0, 5, 3), first.Dimension, $"after second add={first.Dimension}");
        grid.Dimension = new Rectangle(0, 0, 14, 8);

        Assert.AreEqual(0, Grid.GetRow(first));
        Assert.AreEqual(1, Grid.GetRow(second));
        Assert.AreEqual(GridUnitType.Star, grid.RowDefinitions[0].Height.GridUnitType);
        Assert.AreEqual(GridUnitType.Star, grid.RowDefinitions[1].Height.GridUnitType);
        Assert.AreEqual(new Rectangle(7, 0, 7, 4), first.Dimension, $"first={first.Dimension}, second={second.Dimension}");
        Assert.AreEqual(new Rectangle(0, 4, 7, 4), second.Dimension);
    }

    private static Grid CreateGrid(int width, int height)
    {
        var grid = new Grid { size = new Size(width, height) };
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        return grid;
    }
}
