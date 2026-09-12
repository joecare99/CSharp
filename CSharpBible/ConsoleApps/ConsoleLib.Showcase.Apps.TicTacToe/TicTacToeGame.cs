using System;
using System.Collections.Generic;

namespace ConsoleLib.Showcase.Apps.TicTacToe;

public sealed class TicTacToeGame
{
    private static readonly int[][] WinningLines =
    [
        [0, 1, 2], [3, 4, 5], [6, 7, 8],
        [0, 3, 6], [1, 4, 7], [2, 5, 8],
        [0, 4, 8], [2, 4, 6]
    ];

    private readonly TicTacToePlayer[] _board = new TicTacToePlayer[9];
    private readonly IReadOnlyList<TicTacToePlayer> _boardView;

    public TicTacToeGame()
    {
        _boardView = Array.AsReadOnly(_board);
    }

    public IReadOnlyList<TicTacToePlayer> Board => _boardView;
    public TicTacToePlayer CurrentPlayer { get; private set; } = TicTacToePlayer.X;
    public TicTacToePlayer Winner { get; private set; }
    public bool IsDraw { get; private set; }
    public bool IsGameOver => Winner != TicTacToePlayer.None || IsDraw;
    public int TurnCount { get; private set; }

    public TicTacToePlayer GetCell(int index)
    {
        ValidateIndex(index);
        return _board[index];
    }

    public TicTacToeMoveResult Play(int index)
    {
        ValidateIndex(index);

        if (IsGameOver || _board[index] != TicTacToePlayer.None)
            return TicTacToeMoveResult.Invalid;

        var player = CurrentPlayer;
        _board[index] = player;
        TurnCount++;

        if (HasWinningLine(player))
        {
            Winner = player;
            return TicTacToeMoveResult.Won;
        }

        if (TurnCount == _board.Length)
        {
            IsDraw = true;
            return TicTacToeMoveResult.Draw;
        }

        CurrentPlayer = player == TicTacToePlayer.X ? TicTacToePlayer.O : TicTacToePlayer.X;
        return TicTacToeMoveResult.Played;
    }

    public void Restart()
    {
        Array.Fill(_board, TicTacToePlayer.None);
        CurrentPlayer = TicTacToePlayer.X;
        Winner = TicTacToePlayer.None;
        IsDraw = false;
        TurnCount = 0;
    }

    private bool HasWinningLine(TicTacToePlayer player)
    {
        foreach (var line in WinningLines)
        {
            if (_board[line[0]] == player && _board[line[1]] == player && _board[line[2]] == player)
                return true;
        }

        return false;
    }

    private static void ValidateIndex(int index)
    {
        if (index is < 0 or > 8)
            throw new ArgumentOutOfRangeException(nameof(index));
    }
}
