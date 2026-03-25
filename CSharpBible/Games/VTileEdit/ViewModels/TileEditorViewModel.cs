using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using VTileEdit.Models;

namespace VTileEdit.ViewModels;

/// <summary>
/// View-model for the tile editor. This implementation is UI-agnostic and can be hosted by different UIs.
/// </summary>
public sealed partial class TileEditorViewModel : ObservableObject
{
    private readonly IVTEModel _persistenceModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="TileEditorViewModel"/> class.
    /// </summary>
    /// <param name="persistenceModel">The persistence model used to store tile changes.</param>
    public TileEditorViewModel(IVTEModel persistenceModel)
    {
        _persistenceModel = persistenceModel ?? throw new ArgumentNullException(nameof(persistenceModel));
        Palette = new ObservableCollection<ConsoleColor>(Enum.GetValues<ConsoleColor>());
        CharacterPalette = new ObservableCollection<char>();

        ApplyForegroundCommand = new RelayCommand<ConsoleColor>(ApplyForeground);
        ApplyBackgroundCommand = new RelayCommand<ConsoleColor>(ApplyBackground);
        ApplyCharacterCommand = new RelayCommand<char>(ApplyCharacter);
    }

    /// <summary>
    /// Gets the available console colors.
    /// </summary>
    public ObservableCollection<ConsoleColor> Palette { get; }

    /// <summary>
    /// Gets the available characters for the character palette.
    /// </summary>
    public ObservableCollection<char> CharacterPalette { get; }

    /// <summary>
    /// Gets the command that applies a foreground color.
    /// </summary>
    public IRelayCommand<ConsoleColor> ApplyForegroundCommand { get; }

    /// <summary>
    /// Gets the command that applies a background color.
    /// </summary>
    public IRelayCommand<ConsoleColor> ApplyBackgroundCommand { get; }

    /// <summary>
    /// Gets the command that applies a character.
    /// </summary>
    public IRelayCommand<char> ApplyCharacterCommand { get; }

    private void ApplyForeground(ConsoleColor color)
        => _ = color;

    private void ApplyBackground(ConsoleColor color)
        => _ = color;

    private void ApplyCharacter(char character)
        => _ = character;
}
