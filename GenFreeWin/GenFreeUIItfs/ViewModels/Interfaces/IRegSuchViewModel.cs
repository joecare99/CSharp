using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Forms;

namespace GenFree.ViewModels.Interfaces;

public interface IRegSuchViewModel
{
    IContainerControl View { get; set; }
    string Text1_Text { get; set; }
    IRelayCommand<object> Label9_DoubleClickCommand { get; }
    IRelayCommand<object> Label70_DoubleClickCommand { get; }

    void Form_Load(object sender, EventArgs e);
    
    void Combo1_KeyUp(object s, KeyEventArgs e);
    void Command1_Click(object sender, EventArgs e);
    void Label5_DoubleClick(object sender, EventArgs e);
    void ListBox1_Click(object s, EventArgs e);
    void ListBox1_DoubleClick(object s, EventArgs e);
    void Option1_CheckedChanged(object sender, EventArgs e);
    void Regsuch_FormClosed(object sender, FormClosedEventArgs e);
}