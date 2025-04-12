using Gen_FreeWin.ViewModels.Interfaces;
using GenFree;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Gen_FreeWin.Views;

public partial class Adresse : Form
{
    private static readonly List<WeakReference> __ENCList = new();

    private IAdresseViewModel _adresseViewModel;
    [DebuggerNonUserCode]
    public Adresse()
    {
        Load += Adresse_Load;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }

        InitializeComponent();

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
