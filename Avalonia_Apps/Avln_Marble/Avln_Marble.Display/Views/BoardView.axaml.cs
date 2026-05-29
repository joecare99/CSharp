using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Avln_Marble.Display.ViewModels;
using MarbleBoard.Engine.Models;

namespace Avln_Marble.Display.Views;

/// <summary>
/// Renders the interactive marble board and translates pointer interaction to the board view model.
/// </summary>
public partial class BoardView : UserControl
{
    private bool _isPointerCaptured;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoardView"/> class.
    /// </summary>
    public BoardView()
    {
        InitializeComponent();
        AttachedToVisualTree += (_, _) => BoardSurface.Focus();
    }

    private BoardViewModel Board => (BoardViewModel)DataContext!;

    private void BoardSurface_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (TryGetMarbleViewModel(e.Source as Visual, out MarbleViewModel marble))
        {
            if (Board.BeginPointerDrag(marble.Coordinate))
            {
                e.Pointer.Capture(BoardSurface);
                _isPointerCaptured = true;
                e.Handled = true;
            }

            return;
        }

        Point point = e.GetPosition(BoardSurface);
        if (Board.TryGetCoordinate(point, out BoardCoordinate coordinate))
        {
            Board.Select(coordinate);
            BoardSurface.Focus();
            e.Handled = true;
        }
    }

    private void BoardSurface_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isPointerCaptured)
        {
            return;
        }

        Board.UpdatePointer(e.GetPosition(BoardSurface));
        e.Handled = true;
    }

    private void BoardSurface_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_isPointerCaptured)
        {
            return;
        }

        Board.CompletePointerDrag(e.GetPosition(BoardSurface));
        e.Pointer.Capture(null);
        _isPointerCaptured = false;
        BoardSurface.Focus();
        e.Handled = true;
    }

    private static bool TryGetMarbleViewModel(Visual? source, out MarbleViewModel marble)
    {
        Visual? current = source;
        while (current is not null)
        {
            if (current is StyledElement element && element.DataContext is MarbleViewModel marbleViewModel)
            {
                marble = marbleViewModel;
                return true;
            }

            current = current.GetVisualParent();
        }

        marble = default!;
        return false;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
