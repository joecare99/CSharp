using Avalonia.Controls;

namespace Avalonia_App02.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnNewClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (this.FindControl<LabyrinthView>("Maze") is { } maze)
            {
                maze.Regenerate();
            }
        }
    }
}