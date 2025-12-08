using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TRadioButton component.
/// </summary>
public partial class TRadioButton : LfmComponentBase
{
    [ObservableProperty]
    private bool _checked;

    public TRadioButton()
    {
        Height = 19;
        Width = 97;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "checked":
                Checked = ConvertToBool(value);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}
