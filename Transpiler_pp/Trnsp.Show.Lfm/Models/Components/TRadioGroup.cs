using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TRadioGroup component (group of radio buttons).
/// </summary>
public partial class TRadioGroup : TGroupBox
{
    [ObservableProperty]
    private int _columns = 1;

    [ObservableProperty]
    private int _itemIndex = -1;

    public List<string> Items { get; } = [];

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "columns":
                Columns = ConvertToInt(value, 1);
                break;
            case "itemindex":
                ItemIndex = ConvertToInt(value, -1);
                break;
            case "items.strings":
                if (value is IEnumerable<object> items)
                {
                    Items.Clear();
                    foreach (var item in items)
                    {
                        Items.Add(item?.ToString() ?? string.Empty);
                    }
                }
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}
