using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media.Imaging;
using Treppen.Base;
using Treppen.WPF.Services;
using System.Drawing;
using System.Windows;

namespace Treppen.WPF.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private int _size = 21;

    [ObservableProperty]
    private BitmapSource? _previewImage;

    [ObservableProperty]
    private IHeightLabyrinth _labyrinth;

    private readonly IDrawingService _drawingService;

    public MainWindowViewModel(IHeightLabyrinth labyrinth, IDrawingService drawingService)
    {
        _labyrinth = labyrinth;
        _drawingService = drawingService;
        Generate();
    }

    [RelayCommand]
    private void Generate()
    {
        _labyrinth.Dimension = new Rectangle(0, 0, Size, Size);
        _labyrinth.Generate();
        PreviewImage = _drawingService.CreateLabyrinthPreview(_labyrinth);
        OnPropertyChanged(nameof(Labyrinth)); // Notify that the labyrinth has changed
    }

    [RelayCommand]
    private void Draw()
    {
        // The view is now automatically updated when the labyrinth changes.
        // This button could be used for a manual refresh if needed.
        OnPropertyChanged(nameof(Labyrinth)); 
    }

    [RelayCommand]
    private void Print()
    {
        // TODO: Implement Print logic
        MessageBox.Show("Printing not implemented yet.");
    }
}
