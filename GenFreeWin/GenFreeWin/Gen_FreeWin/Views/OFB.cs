using GenFree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Views;

namespace Gen_FreeWin.Views;

public partial class OFB : Form
{
    private static readonly List<WeakReference> __ENCList = new();

    private IOFBViewModel _viewModel;

    public IOFBViewModel ViewModel => _viewModel;

    [DebuggerNonUserCode]
    public OFB(IOFBViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.View = this;
        _viewModel.DoClose = Close;
        _viewModel.InitView = VmInitView;
        _viewModel.SetFocus = VmSetFocus;
        Load += _viewModel.OFB_Load;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }

        InitializeComponent();

        TextBindingAttribute.Commit(this, _viewModel);
        CommandBindingAttribute.Commit(this, _viewModel);
        KeyBindingAttribute.Commit(this, _viewModel);
        CheckedBindingAttribute.Commit(this, _viewModel);
        DblClickBindingAttribute.Commit(this, _viewModel);
        ListBindingAttribute.Commit(this, _viewModel);
    }

    private void VmSetFocus(string obj)
    {
        switch (obj)
        {
            case nameof(IOFBViewModel.Text2_0_Text):
                _Text2_0.Focus();
                break;
            case nameof(IOFBViewModel.Text2_1_Text):
                _Text2_1.Focus();
                break;
            case nameof(IOFBViewModel.Text2_2_Text):
                _Text2_2.Focus();
                break;
        }
    }

    private void VmInitView(float _)
    {
        var Modul1 = _Modul1.Instance;
        if (Modul1.FontSize > 0f)
        {
            Font _regularFont = new("Arial", Modul1.FontSize, FontStyle.Regular);
            Font = _regularFont;
            _Text2_0.Font = _regularFont;
            _Text2_1.Font = _regularFont;
            _Text2_2.Font = _regularFont;
            _List5_0.Font = _regularFont;
            _List5_1.Font = _regularFont;
            _List5_2.Font = _regularFont;
            List1.Font = _regularFont;
            List2.Font = _regularFont;
            List3.Font = _regularFont;
            List4.Font = _regularFont;
            _Label1_0.Font = _regularFont;
            _Label1_1.Font = _regularFont;
            _Label1_2.Font = _regularFont;
            _Label1_3.Font = _regularFont;
            Check1.Font = _regularFont;
            Text1.Font = _regularFont;
            btnApply.Font = _regularFont;
        }
        Check1.Text = Modul1.IText[EUserText.t402];
        _Label1_0.Text = Modul1.IText[EUserText.t406];
        _Label1_1.Text = Modul1.IText[EUserText.t404];
        _Label1_2.Text = Modul1.IText[EUserText.t405];
        _Label1_3.Text = Modul1.IText[EUserText.t403] + ":";
        Label2.Text = Modul1.IText[EUserText.t407];
        btnApply.Text = Modul1.IText[EUserText.t113];
        Top = Personen.Default.Top + 200;
        Left = Personen.Default.Left + 50;
    }

}
