using System;
using System.Collections.Generic;
using System.Linq;

namespace MarbleBoard.Engine.Models;

/// <summary>
/// Represents the mutable state of the marble board prototype.
/// </summary>
public sealed class MarbleBoardModel
{
    private static readonly (int ColumnOffset, int RowOffset)[] _cardinalOffsets =
    [
        (0, -1),
        (1, 0),
        (0, 1),
        (-1, 0),
    ];

    private readonly MarblePiece?[,] _marbles;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarbleBoardModel"/> class.
    /// </summary>
    /// <param name="columnCount">The number of board columns.</param>
    /// <param name="rowCount">The number of board rows.</param>
    public MarbleBoardModel(int columnCount, int rowCount)
    {
        if (columnCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(columnCount));
        }

        if (rowCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rowCount));
        }

        ColumnCount = columnCount;
        RowCount = rowCount;
        _marbles = new MarblePiece[columnCount, rowCount];
    }

    /// <summary>
    /// Occurs when the board state changes.
    /// </summary>
    public event EventHandler? StateChanged;

    /// <summary>
    /// Gets the number of columns.
    /// </summary>
    public int ColumnCount { get; }

    /// <summary>
    /// Gets the number of rows.
    /// </summary>
    public int RowCount { get; }

    /// <summary>
    /// Gets the currently selected coordinate.
    /// </summary>
    public BoardCoordinate? SelectedCoordinate { get; private set; }

    /// <summary>
    /// Gets the coordinate where a mouse drag started.
    /// </summary>
    public BoardCoordinate? DragOrigin { get; private set; }

    /// <summary>
    /// Creates a sample board arrangement for the prototype UI.
    /// </summary>
    /// <returns>A populated sample board.</returns>
    public static MarbleBoardModel CreateSampleBoard()
    {
        MarbleBoardModel board = new(7, 7);

        (BoardCoordinate Coordinate, MarbleColor Color)[] marbles =
        [
            (new BoardCoordinate(2, 1), MarbleColor.Ruby),
            (new BoardCoordinate(3, 1), MarbleColor.Sapphire),
            (new BoardCoordinate(4, 1), MarbleColor.Emerald),
            (new BoardCoordinate(1, 2), MarbleColor.Amber),
            (new BoardCoordinate(2, 2), MarbleColor.Pearl),
            (new BoardCoordinate(3, 2), MarbleColor.Violet),
            (new BoardCoordinate(4, 2), MarbleColor.Ruby),
            (new BoardCoordinate(5, 2), MarbleColor.Sapphire),
            (new BoardCoordinate(1, 3), MarbleColor.Emerald),
            (new BoardCoordinate(2, 3), MarbleColor.Amber),
            (new BoardCoordinate(4, 3), MarbleColor.Pearl),
            (new BoardCoordinate(5, 3), MarbleColor.Violet),
            (new BoardCoordinate(1, 4), MarbleColor.Ruby),
            (new BoardCoordinate(2, 4), MarbleColor.Sapphire),
            (new BoardCoordinate(3, 4), MarbleColor.Emerald),
            (new BoardCoordinate(4, 4), MarbleColor.Amber),
            (new BoardCoordinate(5, 4), MarbleColor.Pearl),
            (new BoardCoordinate(2, 5), MarbleColor.Violet),
            (new BoardCoordinate(3, 5), MarbleColor.Ruby),
            (new BoardCoordinate(4, 5), MarbleColor.Sapphire),
        ];

        foreach ((BoardCoordinate coordinate, MarbleColor color) in marbles)
        {
            board.SetMarble(coordinate, new MarblePiece(Guid.NewGuid(), color));
        }

        board.SelectedCoordinate = marbles[0].Coordinate;
        return board;
    }

    /// <summary>
    /// Determines whether the specified coordinate is inside the board.
    /// </summary>
    /// <param name="coordinate">The coordinate to test.</param>
    /// <returns><see langword="true"/> if the coordinate is inside the board; otherwise, <see langword="false"/>.</returns>
    public bool IsInside(BoardCoordinate coordinate)
        => coordinate.Column >= 0
           && coordinate.Column < ColumnCount
           && coordinate.Row >= 0
           && coordinate.Row < RowCount;

    /// <summary>
    /// Gets the marble at the specified coordinate.
    /// </summary>
    /// <param name="coordinate">The coordinate to inspect.</param>
    /// <returns>The marble at the specified coordinate, or <see langword="null"/>.</returns>
    public MarblePiece? GetMarble(BoardCoordinate coordinate)
        => !IsInside(coordinate) ? null : _marbles[coordinate.Column, coordinate.Row];

    /// <summary>
    /// Enumerates all occupied board positions.
    /// </summary>
    /// <returns>The occupied coordinates with their marbles.</returns>
    public IReadOnlyList<(BoardCoordinate Coordinate, MarblePiece Piece)> GetOccupiedPositions()
    {
        List<(BoardCoordinate Coordinate, MarblePiece Piece)> positions = new();

        for (int row = 0; row < RowCount; row++)
        {
            for (int column = 0; column < ColumnCount; column++)
            {
                MarblePiece? marble = _marbles[column, row];
                if (marble is not null)
                {
                    positions.Add((new BoardCoordinate(column, row), marble));
                }
            }
        }

        return positions;
    }

    /// <summary>
    /// Gets the occupied cardinal neighbors of the specified coordinate.
    /// </summary>
    /// <param name="coordinate">The source coordinate.</param>
    /// <returns>The occupied neighboring positions.</returns>
    public IReadOnlyList<(BoardCoordinate Coordinate, MarblePiece Piece)> GetOccupiedNeighbors(BoardCoordinate coordinate)
    {
        List<(BoardCoordinate Coordinate, MarblePiece Piece)> neighbors = new();

        foreach ((int columnOffset, int rowOffset) in _cardinalOffsets)
        {
            BoardCoordinate neighborCoordinate = coordinate.Offset(columnOffset, rowOffset);
            MarblePiece? marble = GetMarble(neighborCoordinate);
            if (marble is not null)
            {
                neighbors.Add((neighborCoordinate, marble));
            }
        }

        return neighbors;
    }

    /// <summary>
    /// Selects the marble at the specified coordinate.
    /// </summary>
    /// <param name="coordinate">The coordinate to select.</param>
    /// <returns><see langword="true"/> if the selection changed successfully; otherwise, <see langword="false"/>.</returns>
    public bool Select(BoardCoordinate coordinate)
    {
        if (GetMarble(coordinate) is null)
        {
            return false;
        }

        if (SelectedCoordinate == coordinate)
        {
            return true;
        }

        SelectedCoordinate = coordinate;
        DragOrigin = null;
        OnStateChanged();
        return true;
    }

    /// <summary>
    /// Moves the current selection to the next occupied position in the specified direction.
    /// </summary>
    /// <param name="columnDirection">The horizontal direction.</param>
    /// <param name="rowDirection">The vertical direction.</param>
    /// <returns><see langword="true"/> if a new selection was found; otherwise, <see langword="false"/>.</returns>
    public bool SelectNext(int columnDirection, int rowDirection)
    {
        if (columnDirection == 0 && rowDirection == 0)
        {
            return false;
        }

        BoardCoordinate? startCoordinate = SelectedCoordinate;
        if (startCoordinate is null)
        {
            BoardCoordinate? firstCoordinate = GetOccupiedPositions().Select(static item => (BoardCoordinate?)item.Coordinate).FirstOrDefault();
            if (firstCoordinate is null)
            {
                return false;
            }

            SelectedCoordinate = firstCoordinate;
            OnStateChanged();
            return true;
        }

        BoardCoordinate probe = startCoordinate.Value.Offset(columnDirection, rowDirection);
        while (IsInside(probe))
        {
            if (GetMarble(probe) is not null)
            {
                SelectedCoordinate = probe;
                DragOrigin = null;
                OnStateChanged();
                return true;
            }

            probe = probe.Offset(columnDirection, rowDirection);
        }

        return false;
    }

    /// <summary>
    /// Starts dragging from the specified coordinate.
    /// </summary>
    /// <param name="coordinate">The coordinate that owns the dragged marble.</param>
    /// <returns><see langword="true"/> if dragging started; otherwise, <see langword="false"/>.</returns>
    public bool BeginDrag(BoardCoordinate coordinate)
    {
        if (!Select(coordinate))
        {
            return false;
        }

        DragOrigin = coordinate;
        OnStateChanged();
        return true;
    }

    /// <summary>
    /// Cancels the current drag operation.
    /// </summary>
    public void CancelDrag()
    {
        if (DragOrigin is null)
        {
            return;
        }

        DragOrigin = null;
        OnStateChanged();
    }

    /// <summary>
    /// Moves the selected marble to the target coordinate when the slot is empty.
    /// </summary>
    /// <param name="targetCoordinate">The target coordinate.</param>
    /// <returns><see langword="true"/> if the move succeeded; otherwise, <see langword="false"/>.</returns>
    public bool TryMoveSelectedTo(BoardCoordinate targetCoordinate)
    {
        if (SelectedCoordinate is null || !IsInside(targetCoordinate))
        {
            return false;
        }

        BoardCoordinate sourceCoordinate = SelectedCoordinate.Value;
        if (sourceCoordinate == targetCoordinate || GetMarble(targetCoordinate) is not null)
        {
            return false;
        }

        MarblePiece? marble = GetMarble(sourceCoordinate);
        if (marble is null)
        {
            return false;
        }

        _marbles[sourceCoordinate.Column, sourceCoordinate.Row] = null;
        _marbles[targetCoordinate.Column, targetCoordinate.Row] = marble;
        SelectedCoordinate = targetCoordinate;
        DragOrigin = null;
        OnStateChanged();
        return true;
    }

    /// <summary>
    /// Moves the selected marble by one slot in the specified direction.
    /// </summary>
    /// <param name="columnOffset">The horizontal offset.</param>
    /// <param name="rowOffset">The vertical offset.</param>
    /// <returns><see langword="true"/> if the move succeeded; otherwise, <see langword="false"/>.</returns>
    public bool TryMoveSelectedBy(int columnOffset, int rowOffset)
    {
        if (SelectedCoordinate is null)
        {
            return false;
        }

        return TryMoveSelectedTo(SelectedCoordinate.Value.Offset(columnOffset, rowOffset));
    }

    private void SetMarble(BoardCoordinate coordinate, MarblePiece marble)
    {
        if (!IsInside(coordinate))
        {
            throw new ArgumentOutOfRangeException(nameof(coordinate));
        }

        _marbles[coordinate.Column, coordinate.Row] = marble;
    }

    private void OnStateChanged()
        => StateChanged?.Invoke(this, EventArgs.Empty);
}
