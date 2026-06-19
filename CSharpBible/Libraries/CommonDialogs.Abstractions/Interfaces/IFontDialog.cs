namespace CommonDialogs.Interfaces;

using CommonDialogs.Models;

public interface IFontDialog
{
    FontDialogSelection Font { get; set; }
    bool? ShowDialog();
    bool? ShowDialog(object owner);
}