using BaseLib.Helper;
using GenFree.Helper;
using GenFree.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace GenFreeWin.Views;

public partial class RechText : Form
{
    private static List<WeakReference> __ENCList = new List<WeakReference>();
    private IRechTextViewModel _viewModel;

    public int FamPerSchalt => _viewModel.FamPerSchalt;
    public int PersInArb => _viewModel.PersInArb;

    public int FamInArb { get; internal set; }

    [DebuggerNonUserCode]
    public RechText(IRechTextViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.View = this;
        Load += _viewModel.RechText_Load;
        FormClosing += _viewModel.RechText_FormClosing;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        ABef = new ControlArray<Button>();
        ABez = new ControlArray<Label>();
        ACommand1 = new ControlArray<Button>();

        InitializeComponent();
        ACommand1.SetIndex(_Command1_2, 2);
        ACommand1.SetIndex(_Command1_1, 1);
        ACommand1.SetIndex(_Command1_0, 0);
        ABef.SetIndex(_Bef_4, 4);
        ABef.SetIndex(_Bef_0, 0);
        ABef.SetIndex(_Bef_1, 1);
        ABef.SetIndex(_Bef_2, 2);
        ABef.SetIndex(_Bef_3, 3);
        ABef.AddClick(_viewModel.Bef_Click);
        ACommand1.AddClick(_viewModel.Command1_Click);

    }

    private void _Bef_3_Click(object s, EventArgs e) => _viewModel._Bef_3_Click(s, e);
    private void Liste1_DoubleClick(object s, EventArgs e) => _viewModel.Liste1_DoubleClick(s, e);
    private void List3_DoubleClick(object s, EventArgs e) => _viewModel.List3_DoubleClick(s, e);
    private void Label5_Click(object s, EventArgs e) => _viewModel.Label5_Click(s, e);

}
