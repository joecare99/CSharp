using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using GenFree;
using GenFree.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.UI;
using GenFree.Helper;
//using DAO;
using Microsoft.VisualBasic;
using BaseLib.Helper;
using Views;
using GenFree.ViewModels.Interfaces;


namespace Gen_FreeWin.Views;

public partial class Menue : Form, IInteraction
{
    private static readonly List<WeakReference> __ENCList = new();

    public static Menue Default => IoC.GetRequiredService<Menue>();

    IMenu1ViewModel _menu1ViewModel;
    IApplUserTexts Modul1_IText;
    public Menue(IMenu1ViewModel viewModel, IApplUserTexts strings)
    {
        Load += _Form1_Load;
        FormClosing += _Menue_FormClosing;
        _menu1ViewModel = viewModel;
        Modul1_IText = strings;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        components = new System.ComponentModel.Container();
        InitializeComponent();

        _menu1ViewModel.Interaction = this;
        _menu1ViewModel.SetWindowState = (state) => { WindowState = (FormWindowState)state; };
        _menu1ViewModel.GetWindowState = () => WindowState;
        _menu1ViewModel.Grossaend = Grossaend;
        _menu1ViewModel.AdresseType = typeof(Adresse);

        CommandBindingAttribute.Commit(this, viewModel);
        TextBindingAttribute.Commit(this, viewModel);
        VisibilityBindingAttribute.Commit(this, viewModel);
    }

    private void _Menue_FormClosing(object sender, FormClosingEventArgs e)
    {
        _menu1ViewModel.FormClosingCommand?.Execute(e);
    }

    private void _Form1_Load(object sender, EventArgs e)
    {
        _menu1ViewModel.FormLoadCommand.Execute(e);
        SetLabels();
        Text = "Gen_Free";
        Resize += Menue_Resize;
        Menue_Resize(sender, e);

    }

    private void Menue_Resize(object? sender, EventArgs e)
    {
        lblHdrProgName.Width = Width;
        lblHdrCopyright.Width = Width;
        lblHdrAdt.Width = Width;
    }

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
    protected override void Dispose(bool disposing)
    {

        base.Dispose(disposing);
    }

    public void Grossaend(float fS)
    {
        this.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnFamilies.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnSources.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnPersons.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnPlaces.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnMandants.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnManageTexts.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnPrint.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnImportExport.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnAddress.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnEndProgram.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnCalculations.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnFunctionKeys.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnConfig.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnCheckFamilies.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnCheckMissing.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnCheckPersons.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnDuplettes.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnNotes.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnEnterLizenz.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnReorg.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnBachupRead.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnBackupWrite.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnSendData.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnMerging.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnHelpMain.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnRemoteDiag.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnProperty.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        frmStatictics1.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        //lblHdrPersons.Font = ;
        //lblHdrFamilies.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        //lblHdrPlaces.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        //lblHdrDates.Font = new Font("Arial", fS, FontStyle.Regular);
        //lblHdrTexts.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        //lblPersons.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        //lblFamilies.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        //lblPlaces.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        //lblDates.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        //lblTexts.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        lblHdrProgName.Font = new Font("Arial", fS * 0.9f, FontStyle.Bold);
        lblHdrCopyright.Font = new Font("Arial", fS * 0.9f, FontStyle.Bold);
        lblHdrAdt.Font = new Font("Arial", fS * 0.9f, FontStyle.Bold);
        lblMenue18.Font = new Font("Arial", fS, FontStyle.Regular);
        lblWarning.Font = new Font("Arial", fS, FontStyle.Regular);
        lblMandant.Font = new Font("Arial", fS, FontStyle.Bold);
        btnTodayBirth.Font = new Font("Arial", fS, FontStyle.Regular);
        btnTodayDeath.Font = new Font("Arial", fS, FontStyle.Regular);
        btnTodayMarriage.Font = new Font("Arial", fS, FontStyle.Regular);
        btnTodayMarrRel.Font = new Font("Arial", fS, FontStyle.Regular);
        btnProperty.Font = new Font("Arial", fS, FontStyle.Regular);
        btnCheckUpdate.Font = new Font("Arial", fS, FontStyle.Regular);
        btnTodayBapt.Font = new Font("Arial", fS, FontStyle.Regular);
        btnTodayBurial.Font = new Font("Arial", fS, FontStyle.Regular);
        pbxCodeOfArms.Font = new Font("Arial", fS, FontStyle.Regular, GraphicsUnit.Point, 0);
        Menue_Resize(this, new());
    }


    private void SetLabels()
    {
        btnUpdNo.Text = Modul1_IText[EUserText.tNo];
        btnUpdYes.Text = Modul1_IText[EUserText.tYes];
        btnPlaces.Text = Modul1_IText[EUserText.t85_Places];
        btnMandants.Text = Modul1_IText[EUserText.t86_Mandants];
        btnManageTexts.Text = Modul1_IText[EUserText.t87];
        btnPrint.Text = Modul1_IText[EUserText.t88_Print];
        btnImportExport.Text = Modul1_IText[EUserText.t89];
        btnAddress.Text = Modul1_IText[EUserText.t90_Address];
        btnCalculations.Text = Modul1_IText[EUserText.t91];
        btnFunctionKeys.Text = Modul1_IText[EUserText.t92];
        btnConfig.Text = Modul1_IText[EUserText.t93_Config];
        btnCheckPersons.Text = Modul1_IText[EUserText.t94];
        btnCheckFamilies.Text = Modul1_IText[EUserText.t95];
        btnCheckMissing.Text = Modul1_IText[EUserText.t96];
        btnEndProgram.Text = Modul1_IText[EUserText.t97_EndProg];
        btnFamilies.Text = Modul1_IText[EUserText.t236_Families];
        btnPersons.Text = Modul1_IText[EUserText.t237_Persons];
        btnSources.Text = Modul1_IText[EUserText.t244];
        btnRemoteDiag.Text = Modul1_IText[EUserText.t247];
        btnDuplettes.Text = Modul1_IText[EUserText.t248];
        btnProperty.Text = Modul1_IText[EUserText.t249_Property]; // "Grund und Hof-Akten";

        btnMerging.Text = Modul1_IText[EUserText.t249_Property];
        btnNotes.Text = Modul1_IText[EUserText.t249_Property]; // "Bemerkungen durchsuch.";
        btnCardMode.Text = Modul1_IText[EUserText.t249_Property];

        btnCheckUpdate.Text = Modul1_IText[EUserText.t250];
        btnReorg.Text = Modul1_IText[EUserText.t251];
        btnBackupWrite.Text = Modul1_IText[EUserText.t252];
        btnBachupRead.Text = Modul1_IText[EUserText.t253];
        btnSendData.Text = Modul1_IText[EUserText.t254];
        btnHelpMain.Text = Modul1_IText[EUserText.t255];
        btnEnterLizenz.Text = Modul1_IText[EUserText.t278];

        btnTodayBirth.Text = Modul1_IText[EUserText.t435];
        btnTodayDeath.Text = Modul1_IText[EUserText.t436];
        btnTodayMarriage.Text = Modul1_IText[EUserText.t437];
        btnTodayMarrRel.Text = Modul1_IText[EUserText.t438];
        frmStatictics1.SetDefaultTexts();
    }

    public DialogResult MsgBox(string prompt, string title = "", MessageBoxButtons mb = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
    {
        return MsgBox(prompt, title, mb, icon);
    }

    public int Shell(string v)
    {
#if NET5_0_OR_GREATER
        return Interaction.Shell(v);
#else
        return 0;
#endif
    }

    public string? InputBox(string v, string title = "")
    {
#if NET5_0_OR_GREATER
        return Interaction.InputBox(v, title);
#else
        return "";
#endif
    }

    public int Shell(string v, int winStyle = 1)
    {
        throw new NotImplementedException();
    }
}
