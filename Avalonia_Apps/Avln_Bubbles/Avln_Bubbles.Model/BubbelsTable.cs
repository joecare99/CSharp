using System;
using System.Collections.Generic;

namespace Avln_Bubbles.Model;

/// <summary>
/// Provides the rules and state transitions for the bubble board.
/// </summary>
public sealed class BubbleTable
{
    private readonly int[,] _balls;

    /// <summary>
    /// Initializes a new instance of the <see cref="BubbleTable"/> class.
    /// </summary>
    /// <param name="width">The column count.</param>
    /// <param name="height">The row count.</param>
    /// <param name="seed">The random seed.</param>
    public BubbleTable(int width, int height, int seed)
    {
        Column = width;
        Row = height;
        Seed = seed;
        _balls = new int[height, width];
        Random = new Random(seed);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BubbleTable"/> class.
    /// </summary>
    /// <param name="width">The column count.</param>
    /// <param name="height">The row count.</param>
    public BubbleTable(int width, int height)
        : this(width, height, Environment.TickCount)
    {
    }

    /// <summary>
    /// Occurs when a bubble is removed.
    /// </summary>
    public event Action<int, int> BallCleared = delegate { };

    /// <summary>
    /// Occurs when a bubble moves horizontally.
    /// </summary>
    public event Action<int, int, int> BallMovedHorizontal = delegate { };

    /// <summary>
    /// Occurs when a bubble moves vertically.
    /// </summary>
    public event Action<int, int, int> BallMovedVertical = delegate { };

    /// <summary>
    /// Occurs when a new bubble is generated.
    /// </summary>
    public event Action<Ball> NewBallCreated = delegate { };

    /// <summary>
    /// Gets the column count.
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Gets the row count.
    /// </summary>
    public int Row { get; }

    /// <summary>
    /// Gets the seed used for this board.
    /// </summary>
    public int Seed { get; }

    /// <summary>
    /// Gets the random instance.
    /// </summary>
    public Random Random { get; }

    /// <summary>
    /// Gets or sets the number of distinct ball types.
    /// </summary>
    public int BallTypeCount { get; set; } = 5;

    /// <summary>
    /// Clears the board.
    /// </summary>
    public void ClearTable()
    {
        for (var i = 0; i < Row; i++)
        {
            for (var j = 0; j < Column; j++)
            {
                _balls[i, j] = 0;
            }
        }
    }

    /// <summary>
    /// Fills the board with random bubbles.
    /// </summary>
    public void FillTable()
    {
        for (var row = 0; row < Row; row++)
        {
            for (var col = 0; col < Column; col++)
            {
                _balls[row, col] = NextBall();
                NewBallCreated(new Ball(col, row, _balls[row, col]));
            }
        }
    }

    /// <summary>
    /// Sets a cell value.
    /// </summary>
    public void FillCell(int col, int row, int value) => _balls[row, col] = value;

    /// <summary>
    /// Copies the provided values into a row.
    /// </summary>
    public void FillRow(int[] range, int row, int count)
    {
        for (var i = 0; i < count && i < Column; i++)
        {
            _balls[row, i] = range[i];
        }
    }

    /// <summary>
    /// Gets the raw cell value.
    /// </summary>
    public int GetBall(int col, int row) => _balls[row, col];

    /// <summary>
    /// Gets all connected bubbles with the same type.
    /// </summary>
    public List<Ball> GetUnion(int x, int y)
    {
        if (!HasBall(x, y, _balls))
        {
            return [];
        }

        return GetUnion(x, y, (int[,])_balls.Clone(), _balls[y, x]);
    }

    /// <summary>
    /// Clears a connected bubble group and applies gravity plus refill.
    /// </summary>
    public List<Ball> ClearUnion(int x, int y)
    {
        var removedBalls = GetUnion(x, y, _balls, _balls[y, x]);
        foreach (var item in removedBalls)
        {
            BallCleared(item.Y, item.X);
        }

        removedBalls.Sort((first, second) => second.Y - first.Y);
        foreach (var item in removedBalls)
        {
            for (var i = item.Y; i < Row; i++)
            {
                if (i + 1 >= Row || _balls[i + 1, item.X] == 0)
                {
                    break;
                }

                _balls[i, item.X] = _balls[i + 1, item.X];
                _balls[i + 1, item.X] = 0;
                BallMovedVertical(i + 1, item.X, -1);
            }
        }

        RemoveEmptyColumnAndFill();
        return removedBalls;
    }

    /// <summary>
    /// Determines whether the board is in a terminal state.
    /// </summary>
    public bool IsGameOver()
    {
        for (var col = 0; col < Column - 1; col++)
        {
            for (var row = 0; row < Row - 1; row++)
            {
                if (_balls[row, col] != 0 && (_balls[row, col] == _balls[row, col + 1] || _balls[row, col] == _balls[row + 1, col]))
                {
                    return false;
                }
            }
        }

        for (var i = 0; i < Column - 1; i++)
        {
            if (_balls[Row - 1, i] == _balls[Row - 1, i + 1] && _balls[Row - 1, i] != 0)
            {
                return false;
            }
        }

        for (var i = 0; i < Row - 1; i++)
        {
            if (_balls[i, Column - 1] == _balls[i + 1, Column - 1] && _balls[i, Column - 1] != 0)
            {
                return false;
            }
        }

        return true;
    }

    private List<Ball> GetUnion(int x, int y, int[,] data, int ball)
    {
        if (!HasBall(x, y, data) || data[y, x] != ball)
        {
            return [];
        }

        data[y, x] = 0;
        var result = new List<Ball> { new(x, y) };
        result.AddRange(GetUnion(x - 1, y, data, ball));
        result.AddRange(GetUnion(x + 1, y, data, ball));
        result.AddRange(GetUnion(x, y - 1, data, ball));
        result.AddRange(GetUnion(x, y + 1, data, ball));
        return result;
    }

    private List<int> RemoveEmptyColumnAndFill()
    {
        var result = new List<int>();
        var emptyColumnCount = 0;
        for (var i = 0; i < Column; i++)
        {
            if (_balls[0, i] == 0)
            {
                emptyColumnCount++;
                result.Add(i);
            }
        }

        while (CountLeftEmptyColumns() < emptyColumnCount)
        {
            for (var i = 0; i < Column - 1; i++)
            {
                if (IsColumnEmpty(Column - 1 - i))
                {
                    MoveColumnRight(Column - 1 - i);
                }
            }
        }

        for (var i = emptyColumnCount - 1; i >= 0; i--)
        {
            FillLeftColumn(i);
        }

        return result;
    }

    private int CountLeftEmptyColumns()
    {
        var count = 0;
        while (count < Column && _balls[0, count] == 0)
        {
            count++;
        }

        return count;
    }

    private bool ValidatePoint(int x, int y) => x >= 0 && y >= 0 && x < Column && y < Row;

    private bool HasBall(int x, int y, int[,] data) => ValidatePoint(x, y) && data[y, x] != 0;

    private bool IsColumnEmpty(int column)
    {
        if (column < 0 || column >= Column)
        {
            throw new IndexOutOfRangeException("The column is not visible.");
        }

        return _balls[0, column] == 0;
    }

    private void MoveColumnRight(int emptyColumn)
    {
        for (var i = emptyColumn; i > 0; i--)
        {
            for (var j = 0; j < Row; j++)
            {
                _balls[j, i] = _balls[j, i - 1];
                _balls[j, i - 1] = 0;
                if (_balls[j, i] != 0)
                {
                    BallMovedHorizontal(j, i - 1, 1);
                }
            }
        }
    }

    private int NextBall() => Random.Next() % BallTypeCount + 1;

    private void FillLeftColumn(int col)
    {
        var ballCount = Random.Next() % Row + 1;
        for (var i = 0; i < ballCount; i++)
        {
            _balls[i, col] = NextBall();
            NewBallCreated(new Ball(col, i, _balls[i, col]));
        }
    }
}
