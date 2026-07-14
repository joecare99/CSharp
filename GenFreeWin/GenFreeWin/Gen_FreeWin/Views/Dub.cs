using BaseLib.Helper;
using GenFree.Helper;
using GenFree.ViewModels.Interfaces;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Views;

namespace Gen_FreeWin.Views;

public partial class Dub : Form
{

    private IDubViewModel _viewModel;


    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams createParams = base.CreateParams;
            createParams.ClassStyle |= 512;
            return createParams;
        }
    }

    [DebuggerNonUserCode]
    public Dub(IDubViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.View = this;
        Load += _viewModel.Dub_Load;
        FormClosing += _viewModel.Dub_FormClosing;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        components = new System.ComponentModel.Container();

        ACommand1 = new ControlArray<Button>();
        AFrame1 = new ControlArray<GroupBox>();
        List1 = new ControlArray<ListBox>();
        Option1 = new ControlArray<RadioButton>();
        Text1 = new ControlArray<TextBox>();
        ALabel1 = new();

        InitializeComponent();

        ACommand1.SetIndex(_Command1_0, 0);
        ACommand1.SetIndex(_Command1_1, 1);
        ACommand1.SetIndex(_Command1_2, 2);
        ACommand1.SetIndex(_Command1_3, 3);
        ACommand1.SetIndex(_Command1_4, 4);
        ACommand1.SetIndex(_Command1_5, 5);
        ACommand1.SetIndex(_Command1_7, 7);
        ACommand1.SetIndex(_Command1_9, 9);
        ACommand1.SetIndex(_Command1_10, 10);

        List1.SetIndex(_List1_0, 0);
        List1.SetIndex(_List1_1, 1);

        AFrame1.SetIndex(_Frame1_0, 0);
        AFrame1.SetIndex(_Frame1_1, 1);
        AFrame1.SetIndex(_Frame1_2, 2);
        AFrame1.SetIndex(_Frame1_3, 3);

        ALabel1.SetIndex(_Label1_0, 0);
        ALabel1.SetIndex(_Label1_1, 1);
        ALabel1.SetIndex(_Label1_2, 2);
        ALabel1.SetIndex(_Label1_3, 3);
        ALabel1.SetIndex(_Label1_4, 4);
        ALabel1.SetIndex(_Label1_5, 5);
        ALabel1.SetIndex(_Label1_6, 6);
        ALabel1.SetIndex(_Label1_7, 7);
        ALabel1.SetIndex(_Label1_8, 8);
        ALabel1.SetIndex(_Label1_9, 9);
        ALabel1.SetIndex(_Label1_10, 10);
        ALabel1.SetIndex(_Label1_11, 11);
        ALabel1.SetIndex(_Label1_12, 12);
        ALabel1.SetIndex(_Label1_13, 13);
        ALabel1.SetIndex(_Label1_14, 14);
        ALabel1.SetIndex(_Label1_15, 15);
        ALabel1.SetIndex(_Label1_16, 16);
        ALabel1.SetIndex(_Label1_17, 17);
        ALabel1.SetIndex(_Label1_18, 18);
        ALabel1.SetIndex(_Label1_20, 20);
        ALabel1.SetIndex(_Label1_21, 21);
        ALabel1.SetIndex(_Label1_22, 22);
        ALabel1.SetIndex(_Label1_23, 23);
        ALabel1.SetIndex(_Label1_24, 24);
        ALabel1.SetIndex(_Label1_25, 25);
        ALabel1.SetIndex(_Label1_26, 26);
        ALabel1.SetIndex(_Label1_27, 27);
        ALabel1.SetIndex(_Label1_28, 28);
        ALabel1.SetIndex(_Label1_29, 29);
        ALabel1.SetIndex(_Label1_30, 30);
        ALabel1.SetIndex(_Label1_31, 31);
        ALabel1.SetIndex(_Label1_32, 32);
        ALabel1.SetIndex(_Label1_33, 33);
        ALabel1.SetIndex(_Label1_34, 34);
        ALabel1.SetIndex(_Label1_35, 35);
        ALabel1.SetIndex(_Label1_36, 36);
        ALabel1.SetIndex(_Label1_37, 37);
        ALabel1.SetIndex(_Label1_38, 38);

        ALabel1.SetIndex(_Label1_41, 41);
        ALabel1.SetIndex(_Label1_42, 42);
        ALabel1.SetIndex(_Label1_43, 43);
        ALabel1.SetIndex(_Label1_44, 44);

        Option1.SetIndex(_Option1_0, 0);
        Option1.SetIndex(_Option1_2, 2);
        Option1.SetIndex(_Option1_1, 1);

        List1.AddDoubleClick(_viewModel.List1_DoubleClick);
        List1.AddMouseDown(_viewModel.List1_MouseDown);
        Text1.AddTextChanged(_viewModel.Text1_TextChanged);

        CommandBindingAttribute.Commit(this, _viewModel);
    }
    public EventHandler Button1_Click => _viewModel.Button1_Click;
    public EventHandler Button2_Click => _viewModel.Button2_Click;
    public EventHandler Button3_Click => _viewModel.Button3_Click;
    public EventHandler Button4_Click => _viewModel.Button4_Click;
    public EventHandler CheckBox2_CheckedChanged => _viewModel.CheckBox2_CheckedChanged;
    public EventHandler CheckBox2_Click => _viewModel.CheckBox2_Click;
    public EventHandler CheckBox1_CheckedChanged => _viewModel.CheckBox1_CheckedChanged;
    public EventHandler CheckBox1_Click => _viewModel.CheckBox1_Click;
    public EventHandler TextBox1_TextChanged => _viewModel.TextBox1_TextChanged;
    public EventHandler RadioButton2_CheckedChanged => _viewModel.RadioButton2_CheckedChanged;
    private EventHandler _Option1_0_CheckedChanged => _viewModel._Option1_0_CheckedChanged;
}
