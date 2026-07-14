using BaseLib.Helper;
using GenFree;
using GenFree.Interfaces.Sys;
using GenFree.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Views;

namespace Gen_FreeWin.Views;

public partial class Familie : Form
{
    #region Properties
    private static readonly List<WeakReference> __ENCList = new();

    private IContainer components;
    private IModul1 Modul1 => _Modul1.Instance;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int iFamNr { get => lblFamNr.Tag.AsInt(); set => lblFamNr.Text = (lblFamNr.Tag = value).AsInt() == 0 ? "" : $"{Modul1.IText[EUserText.tFamilyNo]} {value.AsString()}"; }
    #endregion

    #region Constructors
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams createParams = base.CreateParams;
            createParams.ClassStyle |= 512;
            return createParams;
        }
    }

    public static Familie Default => MainProject.Forms.Familie;


    private IFamilieViewModel _viewModel;

    public Familie(IFamilieViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.View = this;
        FormClosed += _viewModel.Familie_FormClosed;
        Load += _viewModel.Familie_Load;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        InitializeComponent();
        Text = $"{_Modul1.Instance.AppName} Familienverwaltung";
        CommandBindingAttribute.Commit(this, _viewModel);
        TextBindingAttribute.Commit(this, _viewModel);
    }
    #endregion
    public void Family_ShowFamilyDlg(int famInArb)
    {
        iFamNr = famInArb;
        btnMainmenue.Text = Modul1.IText[EUserText.t158];
        Show();
        Hide();
        _viewModel.Fameinlesen(famInArb, out _);
        _ = ShowDialog();
    }

    public void Show(int iFamNr, EUserText tNMBack = EUserText.tNMBack)
    {
        Modul1.FamInArb = iFamNr;
        short Rich;
        Show();
        _viewModel.Fameinlesen(Modul1.FamInArb, out Rich);

        btnMainmenue.Text = Modul1.IText[tNMBack];
    }

    [Obsolete("Use _viewModel instead.")]
    public void Fameinlesen(int famInArb, out short rich)
        => _viewModel.Fameinlesen(famInArb, out rich);

    private void Listbox3_DoubleClick(object s, EventArgs e) => _viewModel.Listbox3_DoubleClick(s, e);
    private void RichTextBox1_Click(object s, EventArgs e) => _viewModel.RichTextBox1_Click(s, e);
    private void RichTextBox1_KeyDown(object s, KeyEventArgs e) => _viewModel.RichTextBox1_KeyDown(s, e);
    private void RichTextBox1_LinkClicked(object s, LinkClickedEventArgs e) => _viewModel.RichTextBox1_LinkClicked(s, e);
    private void TextBox4_KeyPress(object s, KeyPressEventArgs e) => _viewModel.TextBox4_KeyPress(s, e);
    private void ListBox1_DoubleClick(object s, EventArgs e) => _viewModel.ListBox1_DoubleClick(s, e);
    private void lblPicures_Click(object s, EventArgs e) => _viewModel.lblPicures_Click(s, e);
    private void lblSources_Click(object s, EventArgs e) => _viewModel.lblSources_Click(s, e);

    private void CheckBox1_CheckedChanged(object s, EventArgs e) => _viewModel.CheckBox1_CheckedChanged(s, e);
    private void edtNamePS_KeyUp(object s, KeyEventArgs e) => _viewModel.edtNamePS_KeyUp(s, e);
    private void cbxIllegitRel_Click(object s, EventArgs e) => _viewModel.cbxIllegitRel_Click(s, e);
    private void Button26_Click(object s, EventArgs e) => _viewModel.Button26_Click(s, e);
    private void Button24_Click(object s, EventArgs e) => _viewModel.Button24_Click(s, e);
    private void Label46_Click(object s, EventArgs e) => _viewModel.Label46_Click(s, e);
    private void Button23_Click(object s, EventArgs e) => _viewModel.Button23_Click(s, e);
    private void btnNew_Click(object s, EventArgs e) => _viewModel.btnNew_Click(s, e);
    private void lstMarriages_DoubleClick(object s, EventArgs e) => _viewModel.lstMarriages_DoubleClick(s, e);
    private void btnMarrClose_Click(object s, EventArgs e) => _viewModel.btnMarrClose_Click(s, e);
    private void Command1_Click(object s, EventArgs e) => _viewModel.Command1_Click(s, e);
    private void btnEdit_Click(object s, EventArgs e) => _viewModel.btnEdit_Click(s, e);
    private void TextBox1_KeyPress(object s, KeyPressEventArgs e) => _viewModel.TextBox1_KeyPress(s, e);
    private void btnConfirmation_Click(object s, EventArgs e) => _viewModel.btnConfirmation_Click(s, e);
    private void ComboBox2_TextChanged(object s, EventArgs e) => _viewModel.ComboBox2_TextChanged(s, e);
    private void ComboBox2_SelectedIndexChanged(object s, EventArgs e) => _viewModel.ComboBox2_SelectedIndexChanged(s, e);
    private void btnResidence_Click(object s, EventArgs e) => _viewModel.btnResidence_Click(s, e);
    private void ComboBox1_SelectedIndexChanged(object s, EventArgs e) => _viewModel.ComboBox1_SelectedIndexChanged(s, e);
    private void Button21_Click(object s, EventArgs e) => _viewModel.Button21_Click(s, e);
    private void Button18_Click_1(object s, EventArgs e) => _viewModel.Button18_Click_1(s, e);
    private void btnSearchNumber_Click(object s, EventArgs e) => _viewModel.btnSearchNumber_Click(s, e);
    private void btnSearchName_Click(object s, EventArgs e) => _viewModel.btnSearchName_Click(s, e);
    private void btnReenter_Click(object s, EventArgs e) => _viewModel.btnReenter_Click(s, e);
    private void btnNext_Click(object s, EventArgs e) => _viewModel.btnNext_Click(s, e);
    private void btnPrevious_Click(object s, EventArgs e) => _viewModel.btnPrevious_Click(s, e);
    private void btnEnableCheck_Click(object s, EventArgs e) => _viewModel.btnEnableCheck_Click(s, e);
    private void btnEndTextinput_Click(object s, EventArgs e) => _viewModel.btnEndTextinput_Click(s, e);
    private void btnSearchPartner_Click(object s, EventArgs e) => _viewModel.btnSearchPartner_Click(s, e);
    private void btnSearchRegister_Click(object s, EventArgs e) => _viewModel.btnSearchRegister_Click(s, e);
    private void btnMainmenue_Click(object s, EventArgs e) => _viewModel.btnMainmenue_Click(s, e);
    private void btnFamilysheet_Click(object s, EventArgs e) => _viewModel.btnFamilysheet_Click(s, e);
    private void btnResarch_Click(object s, EventArgs e) => _viewModel.btnResearch_Click(s, e);
    private void btnChildren_Click(object s, EventArgs e) => _viewModel.btnChildren_Click(s, e);
}
