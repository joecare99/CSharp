using Microsoft.VisualBasic;
using System.Windows.Forms;

namespace GenFree.ViewModels.Interfaces;

public interface IInteraction
{
    bool Choose(double v1, string v2, string v3, string v4, string v5, string v6);
    string? InputBox(string v,string title ="");
    DialogResult MsgBox(string prompt,string title="",MessageBoxButtons mb=MessageBoxButtons.OK,MessageBoxIcon icon=MessageBoxIcon.None);
    int Shell(string v, int winStyle = 1);
}