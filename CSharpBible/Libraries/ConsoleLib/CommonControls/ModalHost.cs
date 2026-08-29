using System;
using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls;

/// <summary>Hosts at most one active popup and restores the previous focus target.</summary>
public sealed class ModalHost : Control
{
    private IControl? _previousActive;

    public IPopup? ActivePopup { get; private set; }

    public void Show(IPopup popup)
    {
        if (popup is null)
            throw new ArgumentNullException(nameof(popup));
        if (ActivePopup is not null)
            Close();

        _previousActive = ActiveControl;
        Add(popup);
        ActivePopup = popup;
        popup.Show();
        ActiveControl = popup;
    }

    public void Close()
    {
        if (ActivePopup is null)
            return;

        var popup = ActivePopup;
        ActivePopup = null;
        popup.Hide();
        Remove(popup);
        if (_previousActive is not null && _previousActive.IsVisible && _previousActive.Enabled)
            _previousActive.Active = true;
        _previousActive = null;
    }

    public override void HandlePressKeyEvents(IKeyEvent e)
    {
        if (ActivePopup is null)
        {
            base.HandlePressKeyEvents(e);
            return;
        }

        if (e.bKeyDown && e.usKeyCode == (ushort)ConsoleKey.Escape)
        {
            Close();
            e.Handled = true;
            return;
        }

        ActivePopup.HandlePressKeyEvents(e);
        e.Handled = true;
    }

    public override void MouseClick(IMouseEvent e)
    {
        if (ActivePopup is not null)
        {
            ActivePopup.MouseClick(e);
            return;
        }

        base.MouseClick(e);
    }
}
