namespace CommonDialogs.Interfaces;

using CommonDialogs.Models;

public interface IColorDialog
{
    bool AllowFullOpen { get; set; }
    bool AnyColor { get; set; }
    DialogColor Color { get; set; }
    int[] CustomColors { get; set; }
    bool FullOpen { get; set; }
    bool ShowHelp { get; set; }
    bool SolidColorOnly { get; set; }
    object? Tag { get; set; }

    void Reset();
    bool? ShowDialog();
    bool? ShowDialog(object owner);
}