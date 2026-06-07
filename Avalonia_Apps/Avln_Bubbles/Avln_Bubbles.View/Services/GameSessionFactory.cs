using System;
using Avln_Bubbles.Model;
using Avln_Bubbles.View.ViewModels;

namespace Avln_Bubbles.View.Services;

/// <summary>
/// Default factory that creates a new game table and matching board view model.
/// </summary>
public sealed class GameSessionFactory : IGameSessionFactory
{
    /// <inheritdoc/>
    public BubblesBoardViewModel CreateBoard(int columnCount, int rowCount, int? seed = null)
    {
        var effectiveSeed = seed ?? Environment.TickCount;
        var table = new BubbleTable(columnCount, rowCount, effectiveSeed);
        return new BubblesBoardViewModel(table);
    }
}
