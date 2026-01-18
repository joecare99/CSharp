using CommunityToolkit.Mvvm.ComponentModel;
using SharpHack.ViewModel;

namespace SharpHack.WPF2D.ViewModels;

/// <summary>
/// Main window view model.
/// </summary>
public sealed partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="game">The game view model.</param>
    public MainViewModel(LayeredGameViewModel game)
    {
        Game = game;
    }

    /// <summary>
    /// Gets the game view model.
    /// </summary>
    public LayeredGameViewModel Game { get; }
}
