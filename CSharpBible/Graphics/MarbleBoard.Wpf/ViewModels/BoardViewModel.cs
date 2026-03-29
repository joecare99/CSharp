using MarbleBoard.Engine.Models;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MarbleBoard.Wpf.ViewModels;

/// <summary>
/// Coordinates marble rendering and user interaction for the WPF prototype.
/// </summary>
public sealed class BoardViewModel : BaseViewModelCT
{
    private readonly MarbleBoardModel _boardModel;
    private string _statusText = string.Empty;
    private double _dropTargetLeft;
    private double _dropTargetTop;
    private bool _isDropTargetVisible;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoardViewModel"/> class.
    /// </summary>
    /// <param name="boardModel">The underlying board model.</param>
    public BoardViewModel(MarbleBoardModel boardModel)
    {
        _boardModel = boardModel;
        _boardModel.StateChanged += OnBoardModelStateChanged;

        ColumnCount = boardModel.ColumnCount;
        RowCount = boardModel.RowCount;
        Slots = new ObservableCollection<BoardSlotViewModel>(
            Enumerable.Range(0, RowCount)
                .SelectMany(row => Enumerable.Range(0, ColumnCount).Select(column => new BoardSlotViewModel(column, row))));
        Marbles = new ObservableCollection<MarbleViewModel>();

        RefreshBoard();
        UpdateStatus();
    }

    /// <summary>
    /// Gets the number of board columns.
    /// </summary>
    public int ColumnCount { get; }

    /// <summary>
    /// Gets the number of board rows.
    /// </summary>
    public int RowCount { get; }

    /// <summary>
    /// Gets the constant board cell size.
    /// </summary>
    public double CellSize => 74.0;

    /// <summary>
    /// Gets the marble diameter.
    /// </summary>
    public double MarbleDiameter => 56.0;

    /// <summary>
    /// Gets the board width.
    /// </summary>
    public double BoardWidth => ColumnCount * CellSize;

    /// <summary>
    /// Gets the board height.
    /// </summary>
    public double BoardHeight => RowCount * CellSize;

    /// <summary>
    /// Gets the rendered slot collection.
    /// </summary>
    public ObservableCollection<BoardSlotViewModel> Slots { get; }

    /// <summary>
    /// Gets the rendered marble collection.
    /// </summary>
    public ObservableCollection<MarbleViewModel> Marbles { get; }

    /// <summary>
    /// Gets the current board status text.
    /// </summary>
    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    /// <summary>
    /// Gets the left position of the current drop preview.
    /// </summary>
    public double DropTargetLeft
    {
        get => _dropTargetLeft;
        private set => SetProperty(ref _dropTargetLeft, value);
    }

    /// <summary>
    /// Gets the top position of the current drop preview.
    /// </summary>
    public double DropTargetTop
    {
        get => _dropTargetTop;
        private set => SetProperty(ref _dropTargetTop, value);
    }

    /// <summary>
    /// Gets a value indicating whether the drop preview is visible.
    /// </summary>
    public bool IsDropTargetVisible
    {
        get => _isDropTargetVisible;
        private set => SetProperty(ref _isDropTargetVisible, value);
    }

    /// <summary>
    /// Selects a marble at the specified coordinate.
    /// </summary>
    /// <param name="coordinate">The coordinate to select.</param>
    /// <returns><see langword="true"/> if a marble was selected; otherwise, <see langword="false"/>.</returns>
    public bool Select(BoardCoordinate coordinate)
    {
        bool changed = _boardModel.Select(coordinate);
        if (!changed)
        {
            UpdateStatus("Select a marble before moving it.");
        }

        return changed;
    }

    /// <summary>
    /// Starts dragging a marble with the pointer.
    /// </summary>
    /// <param name="coordinate">The marble coordinate.</param>
    /// <returns><see langword="true"/> if dragging started; otherwise, <see langword="false"/>.</returns>
    public bool BeginPointerDrag(BoardCoordinate coordinate)
    {
        bool changed = _boardModel.BeginDrag(coordinate);
        if (changed)
        {
            UpdateStatus("Drag the selected marble to an empty slot.");
        }

        return changed;
    }

    /// <summary>
    /// Updates the current drop preview from a pointer location.
    /// </summary>
    /// <param name="point">The pointer position within the board surface.</param>
    public void UpdatePointer(Point point)
    {
        if (_boardModel.DragOrigin is null)
        {
            ClearDropTarget();
            return;
        }

        if (!TryGetCoordinate(point, out BoardCoordinate coordinate) || _boardModel.GetMarble(coordinate) is not null)
        {
            ClearDropTarget();
            return;
        }

        DropTargetLeft = GetMarbleLeft(coordinate.Column);
        DropTargetTop = GetMarbleTop(coordinate.Row);
        IsDropTargetVisible = true;
    }

    /// <summary>
    /// Completes a pointer drag operation.
    /// </summary>
    /// <param name="point">The pointer position within the board surface.</param>
    public void CompletePointerDrag(Point point)
    {
        bool moved = false;

        if (_boardModel.DragOrigin is not null && TryGetCoordinate(point, out BoardCoordinate coordinate))
        {
            moved = _boardModel.TryMoveSelectedTo(coordinate);
        }

        if (!moved)
        {
            _boardModel.CancelDrag();
            UpdateStatus("Drag cancelled. Drop on an empty slot to move the marble.");
        }

        ClearDropTarget();
    }

    /// <summary>
    /// Handles arrow-key interaction for selection and keyboard dragging.
    /// </summary>
    /// <param name="key">The pressed key.</param>
    /// <param name="shiftPressed">A value indicating whether shift is pressed.</param>
    /// <returns><see langword="true"/> if the key was handled; otherwise, <see langword="false"/>.</returns>
    public bool HandleArrowKey(Key key, bool shiftPressed)
    {
        (int columnOffset, int rowOffset)? delta = key switch
        {
            Key.Left => (-1, 0),
            Key.Right => (1, 0),
            Key.Up => (0, -1),
            Key.Down => (0, 1),
            _ => null,
        };

        if (delta is null)
        {
            return false;
        }

        bool changed = shiftPressed
            ? _boardModel.TryMoveSelectedBy(delta.Value.columnOffset, delta.Value.rowOffset)
            : _boardModel.SelectNext(delta.Value.columnOffset, delta.Value.rowOffset);

        if (!changed)
        {
            UpdateStatus(shiftPressed
                ? "Keyboard dragging needs a selected marble and a free neighbor slot."
                : "No marble found in that direction.");
        }

        ClearDropTarget();
        return true;
    }

    /// <summary>
    /// Converts a point on the board surface to a board coordinate.
    /// </summary>
    /// <param name="point">The point to convert.</param>
    /// <param name="coordinate">The resulting coordinate.</param>
    /// <returns><see langword="true"/> if the point is inside the board; otherwise, <see langword="false"/>.</returns>
    public bool TryGetCoordinate(Point point, out BoardCoordinate coordinate)
    {
        int column = (int)(point.X / CellSize);
        int row = (int)(point.Y / CellSize);
        coordinate = new BoardCoordinate(column, row);
        return _boardModel.IsInside(coordinate);
    }

    private void OnBoardModelStateChanged(object? sender, EventArgs e)
    {
        RefreshBoard();
        UpdateStatus();
    }

    private void RefreshBoard()
    {
        Marbles.Clear();

        foreach ((BoardCoordinate coordinate, MarblePiece marble) in _boardModel.GetOccupiedPositions())
        {
            IReadOnlyList<(BoardCoordinate Coordinate, MarblePiece Piece)> neighbors = _boardModel.GetOccupiedNeighbors(coordinate);
            BoardCoordinate topCoordinate = coordinate.Offset(0, -1);
            BoardCoordinate rightCoordinate = coordinate.Offset(1, 0);
            BoardCoordinate bottomCoordinate = coordinate.Offset(0, 1);
            BoardCoordinate leftCoordinate = coordinate.Offset(-1, 0);

            MarbleColor? topNeighbor = neighbors.FirstOrDefault(item => item.Coordinate.Row < coordinate.Row).Piece?.Color;
            MarbleColor? rightNeighbor = neighbors.FirstOrDefault(item => item.Coordinate.Column > coordinate.Column).Piece?.Color;
            MarbleColor? bottomNeighbor = neighbors.FirstOrDefault(item => item.Coordinate.Row > coordinate.Row).Piece?.Color;
            MarbleColor? leftNeighbor = neighbors.FirstOrDefault(item => item.Coordinate.Column < coordinate.Column).Piece?.Color;

            Marbles.Add(new MarbleViewModel(
                coordinate,
                GetMarbleLeft(coordinate.Column),
                GetMarbleTop(coordinate.Row),
                MarbleDiameter,
                marble.Color,
                _boardModel.SelectedCoordinate == coordinate,
                _boardModel.IsInside(topCoordinate),
                _boardModel.IsInside(rightCoordinate),
                _boardModel.IsInside(bottomCoordinate),
                _boardModel.IsInside(leftCoordinate),
                topNeighbor,
                rightNeighbor,
                bottomNeighbor,
                leftNeighbor));
        }
    }

    private double GetMarbleLeft(int column)
        => (column * CellSize) + ((CellSize - MarbleDiameter) / 2.0);

    private double GetMarbleTop(int row)
        => (row * CellSize) + ((CellSize - MarbleDiameter) / 2.0);

    private void ClearDropTarget()
    {
        IsDropTargetVisible = false;
        DropTargetLeft = 0.0;
        DropTargetTop = 0.0;
    }

    private void UpdateStatus(string? overrideText = null)
    {
        if (!string.IsNullOrWhiteSpace(overrideText))
        {
            StatusText = overrideText;
            return;
        }

        int marbleCount = _boardModel.GetOccupiedPositions().Count;
        string selectedText = _boardModel.SelectedCoordinate is BoardCoordinate coordinate
            ? $"Selected marble at ({coordinate.Column}, {coordinate.Row})."
            : "No marble selected.";
        StatusText = $"{marbleCount} marbles on the board. {selectedText} Use arrows to select and Shift+Arrows to drag.";
    }
}
