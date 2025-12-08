using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TValueListEditor component.
/// </summary>
public partial class TValueListEditor : TDrawGrid
{
    [ObservableProperty]
    private string _titleCaptions = string.Empty;

    public TValueListEditor()
    {
        ColCount = 2;
        FixedCols = 0;
    }
}
