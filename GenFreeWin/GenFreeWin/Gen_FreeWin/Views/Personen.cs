using GenFree;
using GenFree.Data;
using GenFree.Interfaces.Sys;
using GenFree.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Views;

namespace GenFreeWin.Views;

public partial class Personen : Form
{
    private static List<WeakReference> __ENCList = new List<WeakReference>();
    private IModul1 Modul1 => _Modul1.Instance;

    private IPersonenViewModel _viewModel;

    public static Personen Default => MainProject.Forms.Personen;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool xDestIsFam { get; private set; }
    public int PersonNr { get; internal set; }

    public Personen(IPersonenViewModel viewModel)
    {
        Load += Personen_Load;
        FormClosing += Personen_FormClosing;

        _viewModel = viewModel;
        _viewModel.SetFocus = VmSetFocus;
        _viewModel.DoClose = Close;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        components = new Container();
        ComponentResourceManager resources
            = new ComponentResourceManager(typeof(GenFreeWin.Views.Menue));

        InitializeComponent();

        //Text_Renamed.SetIndex(edtSex, 0);
        //Text_Renamed.SetIndex(edtReligion, 1);

        //Text_Renamed.SetIndex(edtPrefix, 3);
        //Text_Renamed.SetIndex(edtSuffix, 4);
        //Text_Renamed.SetIndex(edtGivennames, 5);

        //Text_Renamed.SetIndex(edtClan, 7);
        //Text_Renamed.SetIndex(edtStatus, 8);


        //Combo1.SetIndex(cbxOccupation, 0);
        //Combo1.SetIndex(cbxTitle, 1);
        //Combo1.SetIndex(cbxResidence, 2);
        //Combo1.SetIndex(cbxHome, 3);
        //Combo1.SetIndex(cbxAdditional, 10);

        /*
                btnEdit.SetIndex(btnLoadFromFile, 2);
        btnEdit.SetIndex(btnCancel3, 0);
                Frame2.SetIndex(frmImport, 1);
                btnEdit.SetIndex(btnReenter, 3);
        */
        //Command4.SetIndex(btnOccupation, 0);
        //Command4.SetIndex(btnResidence, 1);
        //Command4.SetIndex(btnAdditional, 2);
        //Command4.SetIndex(btnTitle, 3);

        //        lblEnterLicence.SetIndex(lblBirthDisp, 101);
        //lblEnterLicence.SetIndex(lblBaptDisp, 102);
        //lblEnterLicence.SetIndex(lblBurialDisp, 104);
        //lblEnterLicence.SetIndex(lblDeathDisp, 103);

        //arLabel.SetIndex(lblAge, 0);
        //arLabel.SetIndex(lblMandant, 1);
        //arLabel.SetIndex(lblSearch2, 2);
        //arLabel.SetIndex(lblSex, 3);
        //arLabel.SetIndex(lblReligion, 4);
        //arLabel.SetIndex(lblMarriages, 5);
        //arLabel.SetIndex(lblAncesterNr, 6);
        //arLabel.SetIndex(lblSurname, 7);
        //arLabel.SetIndex(lblPrefix, 8);
        //arLabel.SetIndex(lblSuffix, 9);
        //arLabel.SetIndex(lblGivennames, 10);
        //arLabel.SetIndex(lblAlias, 11);
        //arLabel.SetIndex(lblClan, 12);
        //arLabel.SetIndex(lblOccupation, 14);
        //arLabel.SetIndex(lblOther, 19);
        //arLabel.SetIndex(lblResidence, 21);
        //arLabel.SetIndex(lblPersonNr, 28);
        //arLabel.SetIndex(lblNachfNr, 29);

        CommandBindingAttribute.Commit(this, _viewModel);
        VisibilityBindingAttribute.Commit(this, _viewModel);
        TextBindingAttribute.Commit(this, _viewModel);
        ListBindingAttribute.Commit(this, _viewModel);

    }

    private void VmSetFocus(object obj)
    {
        switch (obj)
        {
            case nameof(IPersonenViewModel.Sex_Text):
                // Set focus to Sex field
                if (edtSex.Visible)
                    edtSex.Focus();
                break;
            case nameof(IPersonenViewModel.Religion_Text):
                if (edtReligion.Visible)
                    edtReligion.Focus();
                break;
            default:
                throw new ArgumentException("Unknown field", nameof(obj));
        }
    }

    private void Personen_FormClosing(object sender, FormClosingEventArgs e)
    {
        //      _viewModel.FormClosingCommand.Execute(e);
    }

    private void Personen_Load(object sender, EventArgs e)
    {
        _viewModel.FormLoadCommand.Execute(null);
    }

    public void Show(int PersInArb, EUserText eRetText)
    {
        if (eRetText != EUserText.tNone)
            btnReturn.Text = Modul1.IText[eRetText];
        _viewModel.PersonNr = Modul1.PersInArb = PersInArb;
        _viewModel.Perzeig(PersInArb);
        if (Visible)
            Hide();
        base.Show();
    }


    public void Formgross()
    {
        float fS = Modul1.FontSize;
        foreach (Control l in this.Controls)
        {
            l.Font = new Font("Arial", fS, FontStyle.Regular);
        }
        frmPicture.Left = btnLinkedPerson.Left + btnLinkedPerson.Width + 20;
        goto end_IL_0001_2;
    end_IL_0001_2:
        return;

    }

    public void FrmPerson_EventUpd(int PersInArb)
    {
        _viewModel.PersonNr = PersInArb;
    }

    internal void Perzeig(int persInArb)
    {
        throw new NotImplementedException();
    }

    internal void Clear()
    {
        throw new NotImplementedException();
    }

    internal void Zeigfam(string v)
    {
        throw new NotImplementedException();
    }

    internal void SetData(int persInArb, int iFamNr, string lblFamText, EUserText eHdrText, ELinkKennz lkLink)
    {
        throw new NotImplementedException();
    }

    public int AendPruef(int persInArb, int ubg2)
    {
        return _viewModel.AendPruef(persInArb, ubg2);
    }

    internal (short LfNR, EEventArt Art, int iPerfam) FrmPerson_Do(int v, Action performClick, Action close)
    {
        throw new NotImplementedException();
    }
    //#endregion
}
