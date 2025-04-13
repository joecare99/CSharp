using Microsoft.VisualBasic;
using System.Windows.Forms;

namespace Gen_FreeWin.ViewModels.Interfaces
{
    public interface IInteraction
    {
        string? InputBox(string v);
        DialogResult MsgBox(string prompt,string title="",MessageBoxButtons mb=MessageBoxButtons.OK,MessageBoxIcon icon=MessageBoxIcon.None);
        int Shell(string v, int winStyle = 1);
    }
}