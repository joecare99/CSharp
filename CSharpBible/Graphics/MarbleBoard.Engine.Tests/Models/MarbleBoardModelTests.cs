using MarbleBoard.Engine.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarbleBoard.Engine.Tests.Models;

/// <summary>
/// Validates the marble board engine behavior.
/// </summary>
[TestClass]
public sealed class MarbleBoardModelTests
{
    /// <summary>
    /// Verifies that the sample board contains marbles and an initial selection.
    /// </summary>
    [TestMethod]
    public void CreateSampleBoard_InitializesBoardWithSelection()
    {
        MarbleBoardModel board = MarbleBoardModel.CreateSampleBoard();

        Assert.AreEqual(7, board.ColumnCount);
        Assert.AreEqual(7, board.RowCount);
        Assert.IsTrue(board.GetOccupiedPositions().Count > 0);
        Assert.AreEqual(new BoardCoordinate(2, 1), board.SelectedCoordinate);
        Assert.IsNotNull(board.GetMarble(new BoardCoordinate(2, 1)));
    }

    /// <summary>
    /// Verifies that arrow-style selection finds the next occupied marble.
    /// </summary>
    [TestMethod]
    public void SelectNext_MovesSelectionToNextOccupiedMarble()
    {
        MarbleBoardModel board = MarbleBoardModel.CreateSampleBoard();

        bool changed = board.SelectNext(1, 0);

        Assert.IsTrue(changed);
        Assert.AreEqual(new BoardCoordinate(3, 1), board.SelectedCoordinate);
    }

    /// <summary>
    /// Verifies that keyboard dragging moves the selected marble into an empty neighbor slot.
    /// </summary>
    [TestMethod]
    public void TryMoveSelectedBy_MovesSelectedMarbleIntoFreeNeighborSlot()
    {
        MarbleBoardModel board = MarbleBoardModel.CreateSampleBoard();
        bool selected = board.Select(new BoardCoordinate(2, 3));

        bool moved = board.TryMoveSelectedBy(1, 0);

        Assert.IsTrue(selected);
        Assert.IsTrue(moved);
        Assert.IsNull(board.GetMarble(new BoardCoordinate(2, 3)));
        Assert.IsNotNull(board.GetMarble(new BoardCoordinate(3, 3)));
        Assert.AreEqual(new BoardCoordinate(3, 3), board.SelectedCoordinate);
    }

    /// <summary>
    /// Verifies that occupied target slots reject moves.
    /// </summary>
    [TestMethod]
    public void TryMoveSelectedTo_DoesNotOverwriteOccupiedTarget()
    {
        MarbleBoardModel board = MarbleBoardModel.CreateSampleBoard();
        bool selected = board.Select(new BoardCoordinate(2, 1));

        bool moved = board.TryMoveSelectedTo(new BoardCoordinate(3, 1));

        Assert.IsTrue(selected);
        Assert.IsFalse(moved);
        Assert.IsNotNull(board.GetMarble(new BoardCoordinate(2, 1)));
        Assert.IsNotNull(board.GetMarble(new BoardCoordinate(3, 1)));
        Assert.AreEqual(new BoardCoordinate(2, 1), board.SelectedCoordinate);
    }
}
