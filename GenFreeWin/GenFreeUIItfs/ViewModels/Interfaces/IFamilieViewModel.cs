using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GenFree.ViewModels.Interfaces;

public interface IFamilieViewModel: INotifyPropertyChanged
{
    IContainerControl View { get; set; }
    int iFamNr { get; set; }
    IPersonRedViewModel Father { get; }
    IPersonRedViewModel Mother { get; }
    IRelayCommand DeleteFamilyCommand { get; }
    // Add Events-Commands 
    IRelayCommand AddProklamationCommand { get; }
    IRelayCommand AddEngagementCommand { get; }
    IRelayCommand AddMarriageCommand { get; }
    IRelayCommand AddReligMarrCommand { get; }
    IRelayCommand AddDivorceCommand { get; }
    IRelayCommand AddPartnershipCommand { get; }
    IRelayCommand AddDimissoraleCommand { get; }
    IRelayCommand AddEstimatedMarrCommand { get; }

    void btnChildren_Click(object s, EventArgs e);
    void btnConfirmation_Click(object s, EventArgs e);
    void btnEdit_Click(object s, EventArgs e);
    void btnEnableCheck_Click(object s, EventArgs e);
    void btnEndTextinput_Click(object sender, EventArgs e);
    void btnFamilysheet_Click(object sender, EventArgs e);
    void btnMainmenue_Click(object s, EventArgs e);
    void btnMarrClose_Click(object s, EventArgs e);
    void btnNew_Click(object s, EventArgs e);
    void btnNext_Click(object s, EventArgs e);
    void btnPrevious_Click(object s, EventArgs e);
    void btnReenter_Click(object sender, EventArgs e);
    void btnResearch_Click(object sender, EventArgs e);
    void btnResidence_Click(object s, EventArgs e);
    void btnSearchName_Click(object sender, EventArgs e);
    void btnSearchNumber_Click(object s, EventArgs e);
    void btnSearchPartner_Click(object sender, EventArgs e);
    void btnSearchRegister_Click(object s, EventArgs e);
    void Button18_Click_1(object sender, EventArgs e);
    void Button21_Click(object sender, EventArgs e);
    void Button23_Click(object sender, EventArgs e);
    void Button24_Click(object sender, EventArgs e);
    void Button26_Click(object sender, EventArgs e);
    void cbxIllegitRel_Click(object sender, EventArgs e);
    void CheckBox1_CheckedChanged(object sender, EventArgs e);
    void ComboBox1_SelectedIndexChanged(object sender, EventArgs e);
    void ComboBox2_SelectedIndexChanged(object sender, EventArgs e);
    void ComboBox2_TextChanged(object sender, EventArgs e);
    void Command1_Click(object s, EventArgs e);
    void edtNamePS_KeyUp(object sender, KeyEventArgs e);
    void Fameinlesen(int famInArb, out short rich);
    void Familie_FormClosed(object sender, FormClosedEventArgs e);
    void Familie_Load(object sender, EventArgs e);
    void Label45_Click(object sender, EventArgs e);
    void Label46_Click(object sender, EventArgs e);
    void Label46_Click_(object sender, EventArgs e);
    void Label47_Click(object sender, EventArgs e);
    void lblPicures_Click(object sender, EventArgs e);
    void lblSources_Click(object sender, EventArgs e);
    void ListBox1_DoubleClick(object sender, EventArgs e);
    void Listbox3_DoubleClick(object eventSender, EventArgs eventArgs);
    void lstMarriages_DoubleClick(object s, EventArgs e);
    void RichTextBox1_Click(object sender, EventArgs e);
    void RichTextBox1_KeyDown(object sender, KeyEventArgs e);
    void RichTextBox1_LinkClicked(object sender, LinkClickedEventArgs e);
    void TextBox1_KeyPress(object sender, KeyPressEventArgs e);
    void TextBox1_KeyUp(object sender, KeyEventArgs e);
    void TextBox4_KeyPress(object sender, KeyPressEventArgs e);
}