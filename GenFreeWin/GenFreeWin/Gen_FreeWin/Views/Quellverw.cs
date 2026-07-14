using BaseLib.Helper;
using GenFree.Helper;
using GenFree.ViewModels.Interfaces;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Gen_FreeWin;

public partial class Quellverw : Form
{
    private IQuellVerwViewModel _viewModel;

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams createParams = base.CreateParams;
            createParams.ClassStyle |= 512;
            return createParams;
        }
    }
    public IQuellVerwViewModel ViewModel => _viewModel;

    [DebuggerNonUserCode]
    public Quellverw(IQuellVerwViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.View = this;
        Load += _viewModel.Quellverw_Load;
        components = new System.ComponentModel.Container();
        ACommand1 = new ControlArray<Button>();
        ACommand3 = new ControlArray<Button>();
        ALabel1 = new ControlArray<Label>();
        AOption1 = new ControlArray<RadioButton>();
        AOption2 = new ControlArray<RadioButton>();
        AText1 = new ControlArray<TextBox>();

        //   ((System.ComponentModel.ISupportInitialize)AText1).BeginInit();

        InitializeComponent();
        ACommand1.SetIndex(_Command1_0, 0);
        ACommand1.SetIndex(_Command1_1, 1);
        ACommand1.SetIndex(_Command1_2, 2);
        ACommand1.SetIndex(_Command1_3, 3);
        ACommand1.SetIndex(_Command1_4, 4);
        ACommand1.SetIndex(_Command1_5, 5);
        ACommand1.SetIndex(_Command1_6, 6);
        ACommand1.SetIndex(_Command1_7, 7);
        ACommand1.SetIndex(_Command1_8, 8);
        ACommand1.SetIndex(_Command1_9, 9);
        ACommand1.SetIndex(_Command1_10, 10);
        ACommand1.SetIndex(_Command1_11, 11);
        ACommand1.SetIndex(_Command1_12, 12);
        ACommand1.SetIndex(_Command1_14, 14);
        ALabel1.SetIndex(_Label1_1, 1);
        ALabel1.SetIndex(_Label1_2, 2);
        ALabel1.SetIndex(_Label1_3, 3);
        ALabel1.SetIndex(_Label1_4, 4);
        ALabel1.SetIndex(_Label1_5, 5);
        ALabel1.SetIndex(_Label1_6, 6);
        ALabel1.SetIndex(_Label1_7, 7);
        ALabel1.SetIndex(_Label1_8, 8);
        ALabel1.SetIndex(_Label1_9, 9);
        ALabel1.SetIndex(_Label1_11, 11);
        ALabel1.SetIndex(_Label1_12, 12);
        ALabel1.SetIndex(_Label1_13, 13);
        AText1.SetIndex(_Text1_0, 0);
        AText1.SetIndex(_Text1_1, 1);
        AText1.SetIndex(_Text1_2, 2);
        AText1.SetIndex(_Text1_3, 3);
        AText1.SetIndex(_Text1_5, 5);
        AText1.SetIndex(_Text1_6, 6);
        AText1.SetIndex(_Text1_7, 7);
        AText1.SetIndex(_Text1_8, 8);
        AText1.SetIndex(_Text1_9, 9);
        AText1.SetIndex(_Text1_10, 10);
        AOption1.SetIndex(_Option1_1, 1);
        AOption1.SetIndex(_Option1_0, 0);

        AOption2.SetIndex(_Option2_1, 1);
        AOption2.SetIndex(_Option2_2, 2);
        AOption2.SetIndex(_Option2_0, 0);
        ACommand3.SetIndex(_Command3_2, 2);
        ACommand3.SetIndex(_Command3_1, 1);
        ACommand3.SetIndex(_Command3_0, 0);

        ACommand1.AddClick(_viewModel.Command1_Click);
        ACommand3.AddClick(_viewModel.Command3_Click);
        AOption2.AddCheckedChangedRB(_viewModel.Option2_CheckedChanged);
        //    ((System.ComponentModel.ISupportInitialize)AText1).EndInit();
    }

    public DialogResult ShowDialog(int ubg)
    {

        return base.ShowDialog(null);
    }

    private void ComboBox2_MouseDoubleClick(object s,MouseEventArgs e) => _viewModel.ComboBox2_MouseDoubleClick(s, e);
    private void ComboBox2_SelectedIndexChanged(object s,EventArgs e) => _viewModel.ComboBox2_SelectedIndexChanged(s, e);
    private void PictureBox1_Click(object s,EventArgs e) => _viewModel.PictureBox1_Click(s, e);
    private void Button6_Click(object s,EventArgs e) => _viewModel.Button6_Click(s, e);
    private void btnHometown_Click(object s,EventArgs e) => _viewModel.btnHometown_Click(s, e);
    private void Label7_Click(object s,EventArgs e) => _viewModel.Label7_Click(s, e);
    private void btnNewEntry_Click(object s,EventArgs e) => _viewModel.btnNewEntry_Click(s, e);
   
    private void frmSrch_btnDeleteEntry_Click(object s,EventArgs e) => _viewModel.frmSrch.btnDeleteEntry_Click(s, e);
    private void frmSrch_btnClose_Click(object s,EventArgs e) => _viewModel.frmSrch.btnClose_Click(s, e);
    private void frmSrch_ListBox1_DoubleClick(object s,EventArgs e) => _viewModel.frmSrch.ListBox1_DoubleClick(s, e);
    private void frmSrch_edtSearch_TextChanged(object s,EventArgs e) => _viewModel.frmSrch.edtSearch_TextChanged(s, e);
    
    private void ComboBox1_KeyUp(object s,KeyEventArgs e) => _viewModel.ComboBox1_KeyUp(s, e);
    private void ComboBox1_DoubleClick(object s,EventArgs e) => _viewModel.ComboBox1_DoubleClick(s, e);
    private void ComboBox1_SelectedIndexChanged(object s,EventArgs e) => _viewModel.ComboBox1_SelectedIndexChanged(s, e);
    private void Button1_Click(object s,EventArgs e) => _viewModel.Button1_Click(s, e);
    private void TextBox1_KeyDown(object s,KeyEventArgs e) => _viewModel.TextBox1_KeyDown(s, e);
    private void RTB1_GotFocus(object s,EventArgs e) => _viewModel.RTB1_GotFocus(s, e);
    private void List1_DoubleClick(object s,EventArgs e) => _viewModel.List1_DoubleClick(s, e);
    private void Text2_KeyDown(object s,KeyEventArgs e) => _viewModel.Text2_KeyDown(s, e);
    private void _Option1_0_CheckedChanged(object s,EventArgs e) => _viewModel._Option1_0_CheckedChanged(s, e);
    private void Text1_KeyDown(object s,KeyEventArgs e) => _viewModel.Text1_KeyDown(s, e);
}
