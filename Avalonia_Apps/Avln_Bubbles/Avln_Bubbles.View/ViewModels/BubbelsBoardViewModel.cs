using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avln_Bubbles.Model;

namespace Avln_Bubbles.View.ViewModels;

/// <summary>
/// Represents the board state for rendering and interaction.
/// </summary>
public sealed class BubblesBoardViewModel : ViewModelBase
{
    private readonly BubbleTable _model;
    private int _score;
    private int _unionScore;
    private bool _gameOver;

    /// <summary>
    /// Initializes a new instance of the <see cref="BubblesBoardViewModel"/> class.
    /// </summary>
    /// <param name="model">The underlying game table.</param>
    public BubblesBoardViewModel(BubbleTable model)
    {
        _model = model;
        Balls = [];
        Column = model.Column;
        Row = model.Row;
        Seed = model.Seed;

        _model.BallCleared += (rowIndex, colIndex) =>
        {
            var ball = Balls.Single(vm => vm.IsAliveAt(rowIndex, colIndex));
            ball.Removed = true;
        };

        _model.BallMovedVertical += (rowIndex, colIndex, delta) =>
            Balls.Single(vm => vm.IsAliveAt(rowIndex, colIndex)).MoveVertical(delta);

        _model.BallMovedHorizontal += (rowIndex, colIndex, delta) =>
        {
            var ball = Balls.Single(vm => vm.IsAliveAt(rowIndex, colIndex));
            ball.MoveHorizontal(delta);
        };

        _model.NewBallCreated += ball =>
        {
            var vm = new BallViewModel
            {
                Column = ball.X,
                Row = ball.Y,
                BallType = (BallType)ball.Type,
                PointerEnteredAction = HighlightUnion,
                PointerReleasedAction = ClearSelectedUnion
            };

            Balls.Add(vm);
        };
    }

    /// <summary>
    /// Occurs when the board reaches a terminal state.
    /// </summary>
    public event Action? GameOverReached;

    /// <summary>
    /// Gets the column count.
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Gets the row count.
    /// </summary>
    public int Row { get; }

    /// <summary>
    /// Gets the random seed for the current board.
    /// </summary>
    public int Seed { get; }

    /// <summary>
    /// Gets the rendered balls.
    /// </summary>
    public ObservableCollection<BallViewModel> Balls { get; }

    /// <summary>
    /// Gets or sets the total score.
    /// </summary>
    public int Score
    {
        get => _score;
        private set => SetProperty(ref _score, value);
    }

    /// <summary>
    /// Gets or sets the currently highlighted union score.
    /// </summary>
    public int UnionScore
    {
        get => _unionScore;
        private set => SetProperty(ref _unionScore, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the game is over.
    /// </summary>
    public bool GameOver
    {
        get => _gameOver;
        private set => SetProperty(ref _gameOver, value);
    }

    /// <summary>
    /// Populates the board from the model.
    /// </summary>
    public void FillTable() => _model.FillTable();

    /// <summary>
    /// Returns the currently connected group for the given location.
    /// </summary>
    public System.Collections.Generic.List<Ball> GetUnion(int col, int row) => _model.GetUnion(col, row);

    /// <summary>
    /// Resets movement deltas after the last board mutation.
    /// </summary>
    public void CommitAnimationStep()
    {
        for (var i = Balls.Count - 1; i >= 0; i--)
        {
            if (Balls[i].Removed)
            {
                Balls.RemoveAt(i);
            }
        }

        foreach (var ball in Balls)
        {
            ball.ClearDelta();
        }
    }

    private void HighlightUnion(BallViewModel ball)
    {
        foreach (var item in Balls)
        {
            item.Selected = false;
        }

        var union = GetUnion(ball.ColumnWithDelta, ball.RowWithDelta);
        if (union.Count <= 1)
        {
            UnionScore = 0;
            return;
        }

        UnionScore = union.Count * union.Count;
        foreach (var item in union)
        {
            Balls.Single(ballVm => ballVm.IsAliveAt(item.Y, item.X)).Selected = true;
        }
    }

    private void ClearSelectedUnion(BallViewModel ball)
    {
        UnionScore = 0;
        var union = GetUnion(ball.ColumnWithDelta, ball.RowWithDelta);
        if (union.Count <= 1)
        {
            return;
        }

        var removedBalls = _model.ClearUnion(ball.ColumnWithDelta, ball.RowWithDelta);
        if (removedBalls.Count > 1)
        {
            Score += removedBalls.Count * removedBalls.Count;
        }

        CommitAnimationStep();
        if (_model.IsGameOver())
        {
            GameOver = true;
            GameOverReached?.Invoke();
        }

        OnPropertyChanged(nameof(Score));
    }
}
