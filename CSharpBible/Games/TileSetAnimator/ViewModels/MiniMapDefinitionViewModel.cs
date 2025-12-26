using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TileSetAnimator.ViewModels;

/// <summary>
/// Represents a named mini map definition containing 5x5 slots.
/// </summary>
public partial class MiniMapDefinitionViewModel : ObservableObject
{
    public MiniMapDefinitionViewModel(Guid id, string name, ObservableCollection<MiniMapSlotViewModel>? slots = null)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        this.name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
        Slots = slots ?? new ObservableCollection<MiniMapSlotViewModel>();
    }

    /// <summary>
    /// Gets the unique identifier of the definition.
    /// </summary>
    public Guid Id { get; }

    [ObservableProperty]
    private string name = string.Empty;

    /// <summary>
    /// Gets the slot collection belonging to this mini map definition.
    /// </summary>
    public ObservableCollection<MiniMapSlotViewModel> Slots { get; }

    public override string ToString() => Name;
}
