using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GenFreeWin.Views;

public interface IOrtsVerViewModel : INotifyPropertyChanged
{
    string edtPlace_Text { get; set; }
    string edtSuburb_Text { get; set; }
    string edtCounty_Text { get; set; }
    string edtCountry_Text { get; set; }
    string edtState_Text { get; set; }
    string edtLocator_Text { get; set; }
    string edtLat1_Text { get; set; }
    string edtLong1_Text { get; set; }
    string edtGOV_Text { get; set; }
    string edtLat2_Text { get; set; }
    string edtLat3_Text { get; set; }
    string edtLong2_Text { get; set; }
    string edtLong3_Text { get; set; }
    string edtZIP_Text { get; set; }
    string edtAdditional_Text { get; set; }
    string edtPolName_Text { get; set; }
    string TextBox17_Text { get; set; }
    string TextBox18_Text { get; set; }
    string TextBox22_Text { get; set; }
    string TextBox23_Text { get; set; }
    string TextBox24_Text { get; set; }
    string TextBox25_Text { get; set; }
    string TextBox26_Text { get; set; }
    string TextBox27_Text { get; set; }
    string TextBox28_Text { get; set; }
    string TextBox29_Text { get; set; }
    string TextBox30_Text { get; set; }
    string TextBox31_Text { get; set; }
    string TextBox32_Text { get; set; }

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