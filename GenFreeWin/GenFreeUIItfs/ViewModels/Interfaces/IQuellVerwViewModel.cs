using System;
using System.Windows.Forms;

namespace GenFree.ViewModels.Interfaces;

public interface IQuellVerwViewModel
{
    IContainerControl View { get; set; }
    IFrmQuellSrchViewModel frmSrch { get; }
    IInteraction Interaction { get; }
    bool Frame1_Visible { get; }

    void btnHometown_Click(object s, EventArgs e);
    void btnNewEntry_Click(object s, EventArgs e);
    void Button1_Click(object s, EventArgs e);
    void Button6_Click(object s, EventArgs e);
    void ComboBox1_DoubleClick(object s, EventArgs e);
    void ComboBox1_KeyUp(object s, KeyEventArgs e);
    void ComboBox1_SelectedIndexChanged(object s, EventArgs e);
    void ComboBox2_MouseDoubleClick(object s, MouseEventArgs e);
    void ComboBox2_SelectedIndexChanged(object s, EventArgs e);
    void Command1_Click(object eventSender, EventArgs eventArgs);
    void Command2_Click(object eventSender, EventArgs eventArgs);
    void Command3_Click(object eventSender, EventArgs eventArgs);
    void Label7_Click(object s, EventArgs e);
    void List1_DoubleClick(object s, EventArgs e);
    void Option2_CheckedChanged(object eventSender, EventArgs eventArgs);
    void PictureBox1_Click(object s, EventArgs e);
    void Quellverw_Load(object sender, EventArgs e);
    void RTB1_GotFocus(object s, EventArgs e);
    void Text1_KeyDown(object s, KeyEventArgs e);
    void Text2_KeyDown(object s, KeyEventArgs e);
    void TextBox1_KeyDown(object s, KeyEventArgs e);
    void _Option1_0_CheckedChanged(object s, EventArgs e);
    void Les1(long Satznr, bool Rich);
}