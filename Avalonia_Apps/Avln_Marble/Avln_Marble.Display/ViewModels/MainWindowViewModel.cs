namespace Avln_Marble.Display.ViewModels;

/// <summary>
/// Represents the root view model of the marble board sample.
/// </summary>
public sealed class MainWindowViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    /// <param name="board">The interactive board view model.</param>
    public MainWindowViewModel(BoardViewModel board)
    {
        Board = board;
    }

    /// <summary>
    /// Gets the interactive board view model.
    /// </summary>
    public BoardViewModel Board { get; }
}
