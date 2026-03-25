using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TCheckGroup component (group of checkboxes).
/// </summary>
public partial class TCheckGroup : TGroupBox
{
    [ObservableProperty]
    private int _columns = 1;

    public List<string> Items { get; } = [];
    public List<bool> Checked { get; } = [];

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "columns":
                Columns = ConvertToInt(value, 1);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}
