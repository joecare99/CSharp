using GenFree;
using GenFree.Data;
using GenFree.Interfaces.Sys;
using System;
using System.Windows.Forms;

namespace GenFreeWin.Views;

public partial class FraSelPrintPrivacy : UserControl
{
    IModul1 Modul1 => _Modul1.Instance;

    public FraSelPrintPrivacy()
    {
        InitializeComponent();
    }

    public EExportPrivacy ePrintPrivacy
    {
        get
        {
            return 0 switch
            {
                _ when rbtSelPrvLocked.Checked => EExportPrivacy.Locked,
                _ when rbtSelPrvPrivate.Checked => EExportPrivacy.Private,
                _ when rbtSelPrvFree.Checked => EExportPrivacy.Public,
                _ => EExportPrivacy.None
            };
        }
    }

    public event EventHandler OnPrintPrivacyChanged;

    private void FraSelPrintPrivacy_Load(object sender, EventArgs e)
    {
        if (this.DesignMode) return;
        grpSelPrivacy.Text = Modul1.IText[EUserText.t290];
        rbtSelPrvLocked.Text = Modul1.IText[EUserText.t279];
        rbtSelPrvPrivate.Text = Modul1.IText[EUserText.t280];
        rbtSelPrvFree.Text = Modul1.IText[EUserText.t281];
    }

    private void rbtSeletion_CheckedChanged(object sender, EventArgs e)
    {
        if (sender is RadioButton r && r.Checked)
            OnPrintPrivacyChanged?.Invoke(sender, e);
    }
}
