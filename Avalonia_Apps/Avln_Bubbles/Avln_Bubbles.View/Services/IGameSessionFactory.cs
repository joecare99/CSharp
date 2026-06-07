using Avln_Bubbles.View.ViewModels;

namespace Avln_Bubbles.View.Services;

/// <summary>
/// Creates game board sessions for the Bubbles application.
/// </summary>
public interface IGameSessionFactory
{
    /// <summary>
    /// Creates a new board view model instance.
    /// </summary>
    /// <param name="columnCount">The number of columns.</param>
    /// <param name="rowCount">The number of rows.</param>
    /// <param name="seed">The optional random seed.</param>
    /// <returns>A configured board view model.</returns>
    BubblesBoardViewModel CreateBoard(int columnCount, int rowCount, int? seed = null);
}
