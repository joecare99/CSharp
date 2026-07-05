using BaseLib.Helper;
using GenFree.Helper;
using GenFree.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace Gen_FreeWin.Views;

public partial class Vornam : Form
{
    private static List<WeakReference> __ENCList = new List<WeakReference>();


    public ToolTip ToolTip1;

    public ControlArray<Button> Befehl;

    public ControlArray<TextBox> Text_Renamed;

#pragma warning disable CS0618 // Typ oder Element ist veraltet

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#pragma warning restore CS0618 // Typ oder Element ist veraltet

#pragma warning disable CS0618 // Typ oder Element ist veraltet


    private void Liste1_DoubleClick(object s,EventArgs e) => _viewModel.Liste1_DoubleClick(s,e);
    private void List1_DoubleClick(object s, EventArgs e) => _viewModel.List1_DoubleClick(s, e);

    public ControlArray<TextBox> Text1;
    private IVornamViewModel _viewModel;
#pragma warning restore CS0618 // Typ oder Element ist veraltet

    [DebuggerNonUserCode]
    public Vornam(IVornamViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.View = this;
        Load += _viewModel.Form_Load;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
#pragma warning disable CS0618 // Typ oder Element ist veraltet
        Befehl = new ControlArray<Button>();
        Text_Renamed = new ControlArray<TextBox>();
        Text1 = new ControlArray<TextBox>();
#pragma warning restore CS0618 // Typ oder Element ist veraltet
        ((ISupportInitialize)Befehl).BeginInit();
        ((ISupportInitialize)Text_Renamed).BeginInit();
        ((ISupportInitialize)Text1).BeginInit();

        InitializeComponent();

        Text_Renamed.SetIndex(_Text_1, 1);
        Text_Renamed.SetIndex(_Text_2, 2);
        Text_Renamed.SetIndex(_Text_3, 3);
        Text_Renamed.SetIndex(_Text_4, 4);
        Text_Renamed.SetIndex(_Text_5, 5);
        Text_Renamed.SetIndex(_Text_6, 6);
        Text_Renamed.SetIndex(_Text_7, 7);
        Text_Renamed.SetIndex(_Text_8, 8);
        Text_Renamed.SetIndex(_Text_9, 9);
        Text_Renamed.SetIndex(_Text_10, 10);
        Text_Renamed.SetIndex(_Text_11, 11);
        Text_Renamed.SetIndex(_Text_12, 12);
        Text_Renamed.SetIndex(_Text_13, 13);
        Text_Renamed.SetIndex(_Text_14, 14);
        Text_Renamed.SetIndex(_Text_15, 15);
        Text_Renamed.SetIndex(_Text_16, 16);
        Text_Renamed.SetIndex(_Text_51, 51);
        Text_Renamed.SetIndex(_Text_52, 52);
        Text_Renamed.SetIndex(_Text_53, 53);
        Text_Renamed.SetIndex(_Text_54, 54);
        Text_Renamed.SetIndex(_Text_55, 55);
        Text_Renamed.SetIndex(_Text_56, 56);
        Text_Renamed.SetIndex(_Text_57, 57);
        Text_Renamed.SetIndex(_Text_58, 58);
        Text_Renamed.SetIndex(_Text_59, 59);
        Text_Renamed.SetIndex(_Text_60, 60);
        Text_Renamed.SetIndex(_Text_61, 61);
        Text_Renamed.SetIndex(_Text_62, 62);
        Text_Renamed.SetIndex(_Text_63, 63);
        Text_Renamed.SetIndex(_Text_64, 64);
        Text_Renamed.SetIndex(_Text_65, 65);
        Befehl.SetIndex(btnCancel, 1);
        Befehl.SetIndex(btnDone, 2);


        Befehl.AddClick(_viewModel.Befehl_Click);
        Text_Renamed.AddKeyPress(_viewModel.Text_Renamed_KeyPress);
        Text_Renamed.AddKeyUp(_viewModel.Text_Renamed_KeyUp);
        Text_Renamed.AddTextChanged(_viewModel.Text_Renamed_TextChanged);

        ((ISupportInitialize)Befehl).EndInit();
        ((ISupportInitialize)Text_Renamed).EndInit();
        ((ISupportInitialize)Text1).EndInit();

    }


    private void _Text_1_TextChanged(object sender, EventArgs e)
    {
    }

    private void _Befehl_2_Click(object sender, EventArgs e)
    {
    }
}
