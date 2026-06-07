using Avalonia.Controls;
using Avalonia.Input;
using Avln_Bubbles.View.ViewModels;

namespace Avln_Bubbles.View.Views;

/// <summary>
/// Displays the interactive bubble board.
/// </summary>
public partial class BoardView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoardView"/> class.
    /// </summary>
    public BoardView()
    {
        InitializeComponent();
    }

    private void BallBorder_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (sender is Control { DataContext: BallViewModel ballViewModel })
        {
            ballViewModel.NotifyPointerEntered();
        }
    }

    private void BallBorder_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Control { DataContext: BallViewModel ballViewModel })
        {
            ballViewModel.NotifyPointerReleased();
        }
    }
}
