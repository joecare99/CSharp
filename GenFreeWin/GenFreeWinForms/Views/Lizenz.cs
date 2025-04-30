using BaseLib.Helper;
using Gen_FreeWin.Views;
using GenFree;
using GenFree.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.ComponentModel;
using GenFree.ViewModels.Interfaces;
using GenFree.Interfaces.UI;
using Gen_FreeWin;

namespace GenFreeWin.Views;

public partial class Lizenz : Form
{
    private ILizenzViewModel _viewModel;
    private IApplUserTexts _strings;

    public Lizenz(ILizenzViewModel viewModel, IApplUserTexts strings)
    {
        Load += _Lizenz_Load;
        _viewModel = viewModel;
        _strings = strings;
        InitializeComponent();


    }

    private void _Lizenz_Load(object eventSender, EventArgs eventArgs)
    {
        lblEnterLicence.Text = _strings[EUserText.t112];
    //    lblDisplayHint.Text = 
        btnVerify.Text = _strings[EUserText.t113];
        btnCancel.Text = _strings[EUserText.tNMCancel];
      //  btnReqHint.Text = 
    }


    private void Text_KeyPress(object eventSender, KeyPressEventArgs eventArgs)
    {
        short num = checked((short)eventArgs.KeyChar);
        switch (((TextBox)eventSender).Tag.AsInt())
        {
            case 0:
                if (num == 189)
                {
                    num = 0;
                }
                if (num is not 8 and < 48)
                {
                    num = 0;
                }
                break;
            case 1:
            case 3:
                if (num > 57)
                {
                    num = 0;
                }
                if (num is not 8 and < 48)
                {
                    num = 0;
                }
                break;
        }
        eventArgs.KeyChar = (char)(num);
        if (num == 0)
        {
            eventArgs.Handled = true;
        }
    }

    private void Text_KeyUp(object eventSender, KeyEventArgs eventArgs)
    {
        switch (((TextBox)eventSender).Tag.AsInt())
        {
            case 0:
                if (txtLicPart1.Text.Length == 10)
                {
                    _ = txtLicPart2.Focus();
                }
                break;
            case 1:
                if (txtLicPart2.Text.Length == 10)
                {
                    _ = txtLicPart3.Focus();
                }
                break;
            case 2:
                if (txtLicPart3.Text.Length >= 5)
                {
                    _ = btnVerify.Focus();
                }
                break;
        }
    }

}
