using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Gen_FreeWin.Views;

public interface IOrtsVerViewModel : INotifyPropertyChanged
{
    IContainerControl View { get; set; }
    IRelayCommand NextCommand { get; }
    IRelayCommand PrevCommand { get; }
    IRelayCommand ShowPlaceGECommand { get; }
    IRelayCommand ShowPlaceGMCommand { get; }
    IRelayCommand LinkGOVCommand { get; }
    IRelayCommand SearchGOVCommand { get; }
    IRelayCommand ConvertKoordsCommand { get; }
    IRelayCommand SearchNameCommand { get; }
    IRelayCommand SearchNumberCommand { get; }
    bool Frame1_Visible { get; }

    void Button10_Click(object s, EventArgs e);
    void Button11_Click(object s, EventArgs e);
    void Button12_Click(object s, EventArgs e);
    void Button13_Click(object s, EventArgs e);
    void Button14_Click(object s, EventArgs e);
    void Button15_Click(object s, EventArgs e);
    void Button16_Click(object s, EventArgs e);
    void Button17_Click(object s, EventArgs e);
    void Button18_Click(object s, EventArgs e);
    void Button19_Click(object s, EventArgs e);
    void Button20_Click(object s, EventArgs e);
    void Button21_Click(object s, EventArgs e);
    void Button22_Click(object s, EventArgs e);
    void Button23_Click(object s, EventArgs e);
    void Button24_Click(object s, EventArgs e);
    void Form_Load(object sender, EventArgs e);
    void Label27_TextChanged(object s, EventArgs e);
    void Label32_TextChanged(object s, EventArgs e);
    void ListBox1_DoubleClick(object sender, EventArgs e);
    void ListBox2_DoubleClick(object s, EventArgs e);
    void ListBox3_DoubleClick(object s, EventArgs e);
    void ListBox4_DoubleClick(object s, EventArgs e);
    void RTB1_KeyUp(object sender, KeyEventArgs e);
    void TextBox13_TextChanged(object sender, EventArgs e);
    void Textbox1_KeyPress(object eventSender, KeyPressEventArgs eventArgs);
    void TextBox1_KeyUp(object sender, KeyEventArgs e);
    void TextBox30_TextChanged(object s, EventArgs e);
    void TextBox31_TextChanged(object s, EventArgs e);
    void TextBox32_TextChanged(object s, EventArgs e);
}