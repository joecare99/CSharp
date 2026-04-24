using System;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TButton component (standard button).
/// </summary>
public partial class TButton : LfmComponentBase
{
    [ObservableProperty]
    private bool _default;

    [ObservableProperty]
    private bool _cancel;

    [ObservableProperty]
    private ModalResult _modalResult = ModalResult.None;

    public TButton()
    {
        Height = 25;
        Width = 75;
        Color = Color.FromRgb(240, 240, 240);
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "default":
                Default = ConvertToBool(value);
                break;
            case "cancel":
                Cancel = ConvertToBool(value);
                break;
            case "modalresult":
                ModalResult = ParseModalResult(value?.ToString());
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static ModalResult ParseModalResult(string? value) => value?.ToLower() switch
    {
        "mrnone" => ModalResult.None,
        "mrok" => ModalResult.OK,
        "mrcancel" => ModalResult.Cancel,
        "mrabort" => ModalResult.Abort,
        "mrretry" => ModalResult.Retry,
        "mrignore" => ModalResult.Ignore,
        "mryes" => ModalResult.Yes,
        "mrno" => ModalResult.No,
        "mrclose" => ModalResult.Close,
        _ => ModalResult.None
    };
}
