using GenFree.ViewModels.Interfaces;
using GenFree;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Views;

namespace GenFreeWin.Views;

public partial class Adresse : Form
{
    private static readonly List<WeakReference> __ENCList = new();

    private IAdresseViewModel _adresseViewModel;
    [DebuggerNonUserCode]
    public Adresse(IAdresseViewModel viewModel)
    {
        Load += Adresse_Load;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        _adresseViewModel = viewModel;
        _adresseViewModel.OnClose += viewModel_OnClose;
        InitializeComponent();

        CommandBindingAttribute.Commit(this, viewModel);
        TextBindingAttribute.Commit(this, viewModel);
    }

    private void viewModel_OnClose(object? sender, EventArgs e)
    {
        Hide();
    }

    [DebuggerNonUserCode]
    protected override void Dispose(bool disposing)
    {

        base.Dispose(disposing);
    }

    private void Adresse_Load(object sender, EventArgs e)
    {
        _adresseViewModel?.FormLoadCommand.Execute(e);
    }

}
