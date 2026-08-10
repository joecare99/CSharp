using BaseLib.Helper;
using GenFreeWin.Main;
using GenFree;
using GenFree.Data;
using GenFree.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace GenFreeWin.Views;

public enum ERahmenResult : int
{
    eRR_None = 0,
    eRR_OK = 30,
    eRR_Removed = 15,
    eRR25 = 25,
    eRR_Error,
}
public partial class Rahmen : Form
{
    private static List<WeakReference> __ENCList = new List<WeakReference>();

    private IRahmenViewModel _viewModel;

    public static Rahmen Default => MainProject.Forms.Rahmen;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public EUserText iHdrText { get => frmFrame1.Tag.AsEnum<EUserText>(); set => frmFrame1.Text = _Modul1.Instance.IText[frmFrame1.Tag = value]; }
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public (short LfNR, EEventArt Art, int iPerfam) DataHolder { get; set; } = (0, EEventArt.eA_Unknown, 0);
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ERahmenResult eResult { get; set; }

    //  public ControlArray<Button> arButtons => _arButtons;


    [DebuggerNonUserCode]
    public Rahmen(IRahmenViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.View = this;
        Load += _viewModel.Form_Load;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        //        Frame2 = new ControlArray<GroupBox>(components);
        //        ((ISupportInitialize)Frame2).BeginInit();
        InitializeComponent();
        /*
        Frame2.SetIndex(frmFrame2, 0);
        arButtons.SetIndex(btnReenter, 0);
        arButtons.SetIndex(btnFromFile, 1);
        arButtons.SetIndex(btnCancel, 8);
        arButtons.SetIndex(btnEnterNumber, 12);
        */
        //        ((ISupportInitialize)Frame2).EndInit();

    }

    private void btnAppend_Click(object s, EventArgs e) => _viewModel.btnAppend_Click(s,e);
    private void Command1_Click(object s, EventArgs e) => _viewModel.Command1_Click(s,e);
    private void btnClose_Click(object s, EventArgs e) => _viewModel.btnClose_Click(s,e);
    private void btnSelFromFile_Click(object s, EventArgs e) => _viewModel.btnSelFromFile_Click(s,e);
    private void btnEnterNumber_Click(object s, EventArgs e) => _viewModel.btnEnterNumber_Click(s,e);
    private void btnSelReenter_Click(object s, EventArgs e) => _viewModel.btnSelReenter_Click(s,e);
    private void btnSelCancel_Click(object s, EventArgs e) => _viewModel.btnSelCancel_Click(s,e);

    public void ShowDialog(int iLinkType, IList<int>? aiPer, EUserText iTag)
    {
        _viewModel._aiPers = aiPer;
        iHdrText = iTag;
        _Modul1.Instance.Ubg = iLinkType;
        _ = base.ShowDialog();
    }

    public void ShowRahmenDialog(string sHeader, EUserText iTag, int iLinkType, IList<int> aiPer)
    {
        Rahmen frmRahmen = this;
        frmRahmen.RTB.Visible = false;
        frmRahmen.lblAsText.Visible = false;
        frmRahmen.ShowDialog(iLinkType, aiPer, iTag);
    }
    public void ShowWittDialog(IList<(EEventArt, short, int)> aiPers, int iLinkType, EUserText iTag)
    {
        _viewModel._atEKFam = aiPers;
        ShowDialog(iLinkType, null, iTag);
    }

    public void DH_Clear()
    {
        DataHolder = (0, 0, 0);
        eResult = ERahmenResult.eRR_None;
    }
}
