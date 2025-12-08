using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TImage/TPaintBox component.
/// </summary>
public partial class TImage : LfmComponentBase
{
    [ObservableProperty]
    private bool _stretch;

    [ObservableProperty]
    private bool _proportional;

    [ObservableProperty]
    private bool _center;

    /// <summary>
    /// Gets the ImageSource for rendering, derived from the Picture property.
    /// </summary>
    public ImageSource? ImageSource => Picture?.ImageSource;

    public TImage()
    {
        Height = 105;
        Width = 105;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "stretch":
                Stretch = ConvertToBool(value);
                break;
            case "proportional":
                Proportional = ConvertToBool(value);
                break;
            case "center":
                Center = ConvertToBool(value);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}
