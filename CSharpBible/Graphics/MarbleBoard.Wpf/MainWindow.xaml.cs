using MarbleBoard.Engine.Models;
using MarbleBoard.Wpf.ViewModels;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MarbleBoard.Wpf;

/// <summary>
/// Hosts the interactive marble board prototype.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="viewModel">The root view model.</param>
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += OnLoaded;
    }

    private BoardViewModel Board => ((MainWindowViewModel)DataContext).Board;

    private void OnLoaded(object sender, RoutedEventArgs e)
        => BoardSurface.Focus();

    private void BoardSurface_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (TryGetMarbleViewModel(e.OriginalSource as DependencyObject, out MarbleViewModel marble))
        {
            if (Board.BeginPointerDrag(marble.Coordinate))
            {
                BoardSurface.CaptureMouse();
                e.Handled = true;
            }

            return;
        }

        if (Board.TryGetCoordinate(e.GetPosition(BoardSurface), out BoardCoordinate coordinate))
        {
            Board.Select(coordinate);
            BoardSurface.Focus();
            e.Handled = true;
        }
    }

    private void BoardSurface_OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (!BoardSurface.IsMouseCaptured)
        {
            return;
        }

        Board.UpdatePointer(e.GetPosition(BoardSurface));
        e.Handled = true;
    }

    private void BoardSurface_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!BoardSurface.IsMouseCaptured)
        {
            return;
        }

        Board.CompletePointerDrag(e.GetPosition(BoardSurface));
        BoardSurface.ReleaseMouseCapture();
        BoardSurface.Focus();
        e.Handled = true;
    }

    private void Window_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        bool shiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
        if (Board.HandleArrowKey(e.Key, shiftPressed))
        {
            e.Handled = true;
        }
    }

    private static bool TryGetMarbleViewModel(DependencyObject? source, out MarbleViewModel marble)
    {
        DependencyObject? current = source;
        while (current is not null)
        {
            if (current is FrameworkElement element && element.DataContext is MarbleViewModel marbleViewModel)
            {
                marble = marbleViewModel;
                return true;
            }

            current = VisualTreeHelper.GetParent(current);
        }

        marble = null!;
        return false;
    }
}
