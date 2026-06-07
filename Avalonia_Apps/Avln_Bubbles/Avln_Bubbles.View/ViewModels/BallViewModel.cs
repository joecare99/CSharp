using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avln_Bubbles.View.ViewModels;

/// <summary>
/// Represents a single bubble on the rendered board.
/// </summary>
public partial class BallViewModel : ViewModelBase
{
    private const double BallPixelSize = 60d;
    private const double BallCellSize = 66d;

    private int _columnDelta;
    private int _rowDelta;

    /// <summary>
    /// Gets or sets the zero-based row.
    /// </summary>
    [ObservableProperty]
    private int _row;

    /// <summary>
    /// Gets or sets the zero-based column.
    /// </summary>
    [ObservableProperty]
    private int _column;

    /// <summary>
    /// Gets or sets the visual bubble type.
    /// </summary>
    [ObservableProperty]
    private BallType _ballType = BallType.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the bubble is selected.
    /// </summary>
    [ObservableProperty]
    private bool _selected;

    /// <summary>
    /// Gets or sets a value indicating whether the bubble has been removed.
    /// </summary>
    [ObservableProperty]
    private bool _removed;

    /// <summary>
    /// Gets the row including animation delta.
    /// </summary>
    public int RowWithDelta => Row + _rowDelta;

    /// <summary>
    /// Gets the vertical pixel position for rendering.
    /// </summary>
    public double PixelTop => RowWithDelta * BallCellSize;

    /// <summary>
    /// Gets the column including animation delta.
    /// </summary>
    public int ColumnWithDelta => Column + _columnDelta;

    /// <summary>
    /// Gets the horizontal pixel position for rendering.
    /// </summary>
    public double PixelLeft => ColumnWithDelta * BallCellSize;

    /// <summary>
    /// Gets the rendered ball size.
    /// </summary>
    public double Size => BallPixelSize;

    /// <summary>
    /// Gets the corner radius for the circular bubble shape.
    /// </summary>
    public double CornerRadius => BallPixelSize / 2d;

    /// <summary>
    /// Gets the additional highlight offset for selected bubbles.
    /// </summary>
    public double HighlightOffset => Selected ? -4d : 0d;

    /// <summary>
    /// Gets or sets the action invoked when the pointer hovers the bubble.
    /// </summary>
    public Action<BallViewModel>? PointerEnteredAction { get; set; }

    /// <summary>
    /// Gets or sets the action invoked when the bubble is clicked.
    /// </summary>
    public Action<BallViewModel>? PointerReleasedAction { get; set; }

    /// <summary>
    /// Gets a value indicating whether the bubble is visible.
    /// </summary>
    public bool IsVisible => !Removed;

    /// <summary>
    /// Notifies the view model that the pointer entered the bubble.
    /// </summary>
    public void NotifyPointerEntered()
    {
        if (!Removed)
        {
            PointerEnteredAction?.Invoke(this);
        }
    }

    /// <summary>
    /// Notifies the view model that the bubble was clicked.
    /// </summary>
    public void NotifyPointerReleased()
    {
        if (!Removed)
        {
            PointerReleasedAction?.Invoke(this);
        }
    }

    /// <summary>
    /// Applies a horizontal delta.
    /// </summary>
    public void MoveHorizontal(int delta) => _columnDelta += delta;

    /// <summary>
    /// Applies a vertical delta.
    /// </summary>
    public void MoveVertical(int delta) => _rowDelta += delta;

    /// <summary>
    /// Commits pending deltas into the absolute coordinates.
    /// </summary>
    public void ClearDelta()
    {
        Row += _rowDelta;
        Column += _columnDelta;
        _rowDelta = 0;
        _columnDelta = 0;
        OnPropertyChanged(nameof(RowWithDelta));
        OnPropertyChanged(nameof(ColumnWithDelta));
        OnPropertyChanged(nameof(PixelTop));
        OnPropertyChanged(nameof(PixelLeft));
    }

    /// <summary>
    /// Determines whether the bubble is alive at the provided position.
    /// </summary>
    public bool IsAliveAt(int rowIndex, int colIndex) => RowWithDelta == rowIndex && ColumnWithDelta == colIndex && !Removed;

    partial void OnRowChanged(int value)
    {
        OnPropertyChanged(nameof(RowWithDelta));
        OnPropertyChanged(nameof(PixelTop));
    }

    partial void OnColumnChanged(int value)
    {
        OnPropertyChanged(nameof(ColumnWithDelta));
        OnPropertyChanged(nameof(PixelLeft));
    }
    partial void OnRemovedChanged(bool value) => OnPropertyChanged(nameof(IsVisible));

    partial void OnSelectedChanged(bool value) => OnPropertyChanged(nameof(HighlightOffset));
}
