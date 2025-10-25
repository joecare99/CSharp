using Avalonia.Controls;
using AA15_Labyrinth.ViewModels;

namespace AA15_Labyrinth.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnNewClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is ILabyrinthViewModel vm)
        {
            vm.Randomize();
            // Force redraw of content
            if (this.Content is Avalonia.Visual visual)
            {
                visual.InvalidateVisual();
            }
        }
    }
}
