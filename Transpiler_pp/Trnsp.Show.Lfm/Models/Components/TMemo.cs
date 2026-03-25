using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TMemo component (multi-line text input).
/// </summary>
public partial class TMemo : TEdit
{
    [ObservableProperty]
    private bool _wordWrap = true;

    [ObservableProperty]
    private ScrollStyle _scrollBars = ScrollStyle.None;

    public TMemo()
    {
        Height = 89;
        Width = 185;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "wordwrap":
                WordWrap = ConvertToBool(value, true);
                break;
            case "scrollbars":
                ScrollBars = ParseScrollBars(value?.ToString());
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static ScrollStyle ParseScrollBars(string? value) => value?.ToLower() switch
    {
        "ssnone" => ScrollStyle.None,
        "sshorizontal" => ScrollStyle.Horizontal,
        "ssvertical" => ScrollStyle.Vertical,
        "ssboth" => ScrollStyle.Both,
        _ => ScrollStyle.None
    };
}
