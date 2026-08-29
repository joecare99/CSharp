using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls;

/// <summary>Focusable popup panel that can be shown or hidden by a modal host.</summary>
public sealed class Dialog : Panel, IPopup
{
    public Dialog()
    {
        Visible = false;
    }

    public void Show()
    {
        Visible = true;
        Active = true;
    }

    public void Hide()
    {
        Active = false;
        Visible = false;
    }
}
