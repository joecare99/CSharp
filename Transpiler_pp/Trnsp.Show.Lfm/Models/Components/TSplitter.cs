using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TSplitter component.
/// </summary>
public partial class TSplitter : LfmComponentBase
{
    [ObservableProperty]
    private int _minSize = 30;

    [ObservableProperty]
    private bool _beveled = true;

    public TSplitter()
    {
        Width = 5;
        Height = 100;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "minsize":
                MinSize = ConvertToInt(value, 30);
                break;
            case "beveled":
                Beveled = ConvertToBool(value, true);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}
