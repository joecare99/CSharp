using BaseLib.Helper;
using GenFree.Helper;
using GenFree.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Views;

namespace Gen_FreeWin;

public partial class Regsuch : Form
{
    private static List<WeakReference> __ENCList = new();

    private IRegSuchViewModel _viewModel;

    public byte Suchschalt { get; internal set; }
    public int Suchfam { get; internal set; }

    [DebuggerNonUserCode]
    public Regsuch(IRegSuchViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.View = this;
        Load += _viewModel.Form_Load;
        FormClosed += _viewModel.Regsuch_FormClosed;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        ACheck2 = new ControlArray<CheckBox>();
        ACommand1 = new ControlArray<Button>();
        ALabel1 = new ControlArray<Label>();
        ALabel5 = new ControlArray<Label>();
        ALabel6 = new ControlArray<Label>();
        ALabel7 = new ControlArray<Label>();
        ALabel8 = new ControlArray<Label>();
        ALine1 = new ControlArray<Label>();
        AOption1 = new ControlArray<RadioButton>();
        InitializeComponent();

        AOption1.SetIndex(_Option1_0, 0);
        AOption1.SetIndex(_Option1_1, 1);
        AOption1.SetIndex(_Option1_2, 2);
        AOption1.SetIndex(_Option1_3, 3);
        AOption1.SetIndex(_Option1_4, 4);
        AOption1.SetIndex(_Option1_5, 5);
        AOption1.SetIndex(_Option1_6, 6);
        AOption1.SetIndex(_Option1_7, 7);
        AOption1.SetIndex(_Option1_8, 8);
        AOption1.SetIndex(_Option1_9, 9);
        AOption1.SetIndex(_Option1_10, 10);
        AOption1.SetIndex(_Option1_11, 11);
        AOption1.SetIndex(_Option1_12, 12);
        AOption1.SetIndex(_Option1_13, 13);

        ACheck2.SetIndex(_Check2_3, 3);

        ACommand1.SetIndex(_Command1_0, 0);
        ACommand1.SetIndex(_Command1_3, 3);
        ACommand1.SetIndex(_Command1_7, 7);

        ALabel1.SetIndex(_Label1_1, 1);

        ACommand1.AddClick(_viewModel.Command1_Click);
        ALabel5.AddDoubleClick(_viewModel.Label5_DoubleClick);
        AOption1.AddCheckedChangedRB(_viewModel.Option1_CheckedChanged);

        TextBindingAttribute.Commit(this, _viewModel);
        CommandBindingAttribute.Commit(this, _viewModel);
        CheckedBindingAttribute.Commit(this, _viewModel);
        DblClickBindingAttribute.Commit(this, _viewModel);
    }
    private void ListBox1_Click(object s, EventArgs e) => _viewModel.ListBox1_Click(s, e);
    private void ListBox1_DoubleClick(object s, EventArgs e) => _viewModel.ListBox1_DoubleClick(s, e);
    private void Combo1_KeyUp(object s, KeyEventArgs e) => _viewModel.Combo1_KeyUp(s, e);

    internal DialogResult ShowDialog(byte v1, byte bSuchschalt, int iSuchpers, int iSuchfam)
    {

        return ShowDialog();
    }
}
