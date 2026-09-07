using System;
using CommunityToolkit.Mvvm.ComponentModel;
using ConsoleLib.CommonControls;

namespace ConsoleLib.Cxaml.Designer.ViewModels;

public sealed partial class GridDefinitionViewModel : ObservableObject
{
    public GridDefinitionViewModel(int index, bool isRow, GridLength length)
    {
        Index = index;
        IsRow = isRow;
        _unit = length.GridUnitType;
        _value = length.GridUnitType == GridUnitType.Auto ? 1 : length.Value;
    }

    public int Index { get; private set; }
    public bool IsRow { get; }

    [ObservableProperty]
    private GridUnitType _unit;

    [ObservableProperty]
    private double _value;

    public string DisplayName => (IsRow ? "Row " : "Column ") + (Index + 1);

    public void Reindex(int index)
    {
        Index = index;
        OnPropertyChanged(nameof(DisplayName));
    }

    public GridLength ToGridLength()
    {
        if (Unit == GridUnitType.Auto)
            return GridLength.Auto;
        return new GridLength(Math.Max(0, Value), Unit);
    }
}
