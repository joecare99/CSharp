namespace MarbleBoard.Wpf.ViewModels;

/// <summary>
/// Represents the root view model of the WPF prototype.
/// </summary>
public sealed class MainWindowViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    /// <param name="board">The board view model.</param>
    public MainWindowViewModel(BoardViewModel board)
    {
        Board = board;
    }

    /// <summary>
    /// Gets the interactive marble board view model.
    /// </summary>
    public BoardViewModel Board { get; }
}
