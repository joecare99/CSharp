using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GenFree.ViewModels.Interfaces;

public interface IDubViewModel: INotifyPropertyChanged
{
    IContainerControl? View { get; set; }
    void Button1_Click(object sender, EventArgs e);
    void Button2_Click(object sender, EventArgs e);
    void Button3_Click(object sender, EventArgs e);
    void Button4_Click(object sender, EventArgs e);
    void CheckBox1_CheckedChanged(object sender, EventArgs e);
    void CheckBox1_Click(object sender, EventArgs e);
    void CheckBox2_CheckedChanged(object sender, EventArgs e);
    void CheckBox2_Click(object sender, EventArgs e);
    void Command1_Click(object sender, EventArgs e);
    void Dub_FormClosing(object sender, FormClosingEventArgs e);
    void Dub_Load(object sender, EventArgs e);
    void List1_DoubleClick(object sender, EventArgs e);
    void List1_MouseDown(object sender, MouseEventArgs e);
    void RadioButton2_CheckedChanged(object sender, EventArgs e);
    void Text1_TextChanged(object sender, EventArgs e);
    void TextBox1_TextChanged(object sender, EventArgs e);
    void _Option1_0_CheckedChanged(object sender, EventArgs e);
}