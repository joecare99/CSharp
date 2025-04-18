using Microsoft.VisualBasic;
using System.Windows.Forms;

namespace GenFree.ViewModels.Interfaces;

public interface IInteraction
{
    string? InputBox(string v,string title ="");
    DialogResult MsgBox(string prompt,string title="",MessageBoxButtons mb=MessageBoxButtons.OK,MessageBoxIcon icon=MessageBoxIcon.None);
    int Shell(string v, int winStyle = 1);
}