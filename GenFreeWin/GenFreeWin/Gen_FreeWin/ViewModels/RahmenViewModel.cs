using BaseLib.Helper;
using GenFreeWin.Main;
using GenFreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using MVVM.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GenFreeWin.ViewModels;

public partial class RahmenViewModel : BaseViewModelCT, IRahmenViewModel
{
    IContainerControl IRahmenViewModel.View { get; set; }
    
    private Rahmen View => (Rahmen)(this as IRahmenViewModel).View;

    private const string sConnectedPersonInsert = "Verbundene Personen Einfügen";
    private string sNoChangesOnCD => Modul1.Message_sNoChangesOnCD;
    private const string sPersonNotExists = "Person existiert nicht";
    private const string sNumberOfPerson = "Nummer der gewünschten Person\r ";

    public IList<int>? _aiPers { get; set; }
    private IList<(int iFamily, ELinkKennz eEKennz)> _atFam;
    public IList<(EEventArt eArt, short iKnz, int iFamily)>? _atEKFam { get; set; }
    private string Modul1_Kont20;
    private string Modul1_LiText;
    [Obsolete]
    IProjectData ProjectData =>Modul1.ProjectData;
    IInteraction Interaction => Menue.Default;
    [Obsolete]
    IVBInformation Information => Modul1.Information;
    [Obsolete]
    IVBConversions Conversion => Modul1.Conversions;
    [Obsolete]
    IStrings Strings => Modul1.Strings;
    [Obsolete]
    IStrings StringType => Modul1.Strings;
    private int Modul1_PerZeug;
    private EEventArt Modul1_ErArt;
    private int Modul1_PerfamNr;
    private int Modul1_PersInArbsp;
    private List<int> Modul1_Per1 = new();
    private int Modul1_Tast;
    IModul1 Modul1 => _Modul1.Instance;

    IList List4_Items => View.List4.Items;

    public void Command1_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_0c9c
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_Start;
                        case 4082:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_ErrHndl;
                                    default:
                                        goto end_IL_0001;
                                }
                                goto IL_0c9e;
                            }
                        IL_0c9e:
                            num = 165;
                            if (Information.Err().Number == 94)
                            {
                                goto IL_0cb9;
                            }
                            else
                            {
                                goto IL_0cd6;
                            }
                        IL_0cd6:
                            num = 169;
                            if (Interaction.MsgBox(Conversion.ErrorToString(), mb: MessageBoxButtons.OKCancel, title: Information.Err().Number.AsString()) == DialogResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            goto IL_0d06;
                        IL_0d06:
                            num = 172;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_0d30;
                        IL_0d30:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_Start;
                                case 15:
                                case 16:
                                    goto IL_0088;
                                case 45:
                                case 46:
                                    goto IL_0284;
                                case 165:
                                    goto IL_0c9e;
                                case 166:
                                    goto IL_0cb9;
                                case 167:
                                case 169:
                                    goto IL_0cd6;
                                case 170:
                                case 172:
                                    goto IL_0d06;
                                default:
                                    goto end_IL_0001;
                                case 8:
                                case 13:
                                case 14:
                                case 18:
                                case 21:
                                case 26:
                                case 30:
                                case 34:
                                case 38:
                                case 43:
                                case 44:
                                case 48:
                                case 51:
                                case 61:
                                case 65:
                                case 74:
                                case 80:
                                case 89:
                                case 94:
                                case 103:
                                case 108:
                                case 123:
                                case 151:
                                case 152:
                                case 161:
                                case 162:
                                case 163:
                                case 164:
                                case 173:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                        IL_0cb9:
                            num = 166;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_ErrHndl;
                        IL_ErrHndl:
                            num4 = unchecked(num2 + 1);
                            goto IL_0d30;
                        IL_Start:
                            num = 2;
                            short index = 2;
                            if (Modul1.Typ != DriveType.CDRom)
                            {
                                Ramentextspeich();
                            }
                            lErl = 1;
                            switch (index)
                            {
                                case 0:
                                    btnClose_Click(eventSender, eventArgs);
                                    break;
                                case 1:
                                    goto IL_0088;
                                case 2:
                                    goto IL_0284;
                                default:
                                    break;
                            }
                            goto end_IL_0001_2;
                        IL_0088:
                            num = 16;
                            btnAppend_Click(eventSender, eventArgs);
                            goto end_IL_0001_2;
                        IL_0284:
                            num = 46;
                            btnDelete_Click(eventSender, eventArgs);
                            goto end_IL_0001_2;
                        end_IL_0001:
                            break;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 4082;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    public void btnDelete_Click(object eventSender, EventArgs eventArgs)
    {
        if (Modul1.Typ != DriveType.CDRom)
            Ramentextspeich();
        int left2 = View.frmFrame1.Tag.AsInt();
        int PersonNr = Personen.Default.PersonNr;
        int iFamNr = View.List4.SelectedItem.ItemData<int>();
        if (Modul1.Typ == DriveType.CDRom)
        {
            _ = Interaction.MsgBox(sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
        }
        else if (left2 == (int)EUserText.tGodparents
             || left2 == (int)EUserText.t348)
        {
            bool flag;
            if (flag = DataModul.Link.Delete(PersonNr, iFamNr, ELinkKennz.lkGodparent))
            {
                Modul1.PersInArb = PersonNr;
            }
            View.Visible = flag;
        }
        else if (left2 == (int)EUserText.tGodparentOf)
        {
            var flag = DataModul.Link.Delete(iFamNr, PersonNr, ELinkKennz.lkGodparent);
            if (flag)
            {
                Modul1.PersInArb = PersonNr;
                View.eResult = ERahmenResult.eRR_Removed;
                View.Close();
                View.Visible = false;
            }
        }
        else if (left2 == 63)
        {
            var flag = DataModul.Link.Delete(iFamNr, PersonNr, ELinkKennz.lk9);
            if (flag)
            {
                View.eResult = ERahmenResult.eRR_Removed;
                View.Close();
            }
        }
        else if (left2 == 62)
        {
            var flag = DataModul.Link.Delete(PersonNr, iFamNr, ELinkKennz.lk9);
            if (flag)
            {
                View.eResult = ERahmenResult.eRR_Removed;
                View.Close();
            }
        }
        else if (left2 == 64)
        {
            Modul1.eWKennz = 10;
            if (Modul1_ErArt > EEventArt.eA_499)
            {
                Modul1_PerfamNr = Familie.Default.iFamNr;
            }
            else
            {
                Debugger.Break();
            }
            DataModul.Witness.Delete((Modul1_PerfamNr, Modul1.PersInArb, Modul1.eWKennz, Modul1_ErArt, Modul1.LfNR));
            View.Close();
        }
        else
        {
            Modul1.PersInArb = (int)Math.Round(Strings.Mid(View.List4.Text, 50, 10).AsDouble());
            int iTag = View.frmFrame1.Tag.AsInt();
            if (iTag == (int)EUserText.tMarrWitness)
            {
                Modul1.eLKennz = ELinkKennz.lkMarrWitness;
            }
            else if (iTag == (int)EUserText.tWitnOfEngage)
            {
                Modul1.eLKennz = ELinkKennz.lkWitnOfEngage;
            }
            else if (iTag == (int)EUserText.tWitnOfMarr)
            {
                Modul1.eLKennz = ELinkKennz.lkWitnOfMarr;
            }
            Modul1.FamInArb = Familie.Default.iFamNr;
            if (iTag == 64) //Todo: Check tag instead of text
            {
                Modul1.eWKennz = 10;
                Modul1_PerfamNr = Modul1_ErArt > EEventArt.eA_499 ? Familie.Default.iFamNr : PersonNr;
                DataModul.Witness.Delete((Modul1_PerfamNr, Modul1.PersInArb, Modul1.eWKennz, Modul1_ErArt, Modul1.LfNR));
                View.Close();
                View.eResult = ERahmenResult.eRR_Removed;
            }
            else
            {
                _ = DataModul.Link.Delete(Modul1.FamInArb, Modul1.PersInArb, Modul1.eLKennz);
                View.Close();
                Familie.Default.Fameinlesen(Modul1.FamInArb, out short rich);
            }
        }
    }


    public void btnAppend_Click(object eventSender, EventArgs eventArgs)
    {
        if (Modul1.Typ != DriveType.CDRom)
            Ramentextspeich();
        int left = View.frmFrame1.Tag.AsInt();
        if (Modul1.Typ == DriveType.CDRom)
        {
            _ = Interaction.MsgBox(sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
        }
        else if (left == (int)EUserText.tGodparents
        || left == (int)EUserText.tGodparentOf
        || left == (int)EUserText.t348)
        {
            View.frmRahmenSelect.Text = "Pate für Person  Einfügen";
            View.frmRahmenSelect.Visible = true;
        }
        else if (left == 62)
        {
            View.frmRahmenSelect.Text = sConnectedPersonInsert;
            View.frmRahmenSelect.Visible = true;
        }
        else if (left == 63)
        {
            View.frmRahmenSelect.Text = "Verbunden mit Einfügen";
            View.frmRahmenSelect.Visible = true;
        }
        else
        {
            View.List4.Enabled = false;
            View.frmRahmenSelect.Text = View.frmFrame1.Text + " einfügen";
            View.frmRahmenSelect.Visible = true;
        }
    }

    public void btnClose_Click(object eventSender, EventArgs eventArgs)
    {
        if (Modul1.Typ != DriveType.CDRom)
            Ramentextspeich();
        View.Close();
        View.eResult = ERahmenResult.eRR_OK;
    }

    private void Command2_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_2ec6
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_Start;
                        case 14148:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_2f1e;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString(), mb: MessageBoxButtons.OKCancel, title: Information.Err().Number.AsString()) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_2f22;
                            }
                        end_IL_0001:
                            break;
                        IL_Start:
                            num = 2;
                            var index = 0;// arButtons.GetIndex((Button)eventSender);
                            if (View.frmFrame1.Tag.AsInt() != 64)
                            {
                                View.frmRahmenSelect.xVisReenter = true;
                            }
                            switch (index)
                            {
                                case 0:
                                    btnSelReenter_Click(eventSender, eventArgs);
                                    goto end_IL_0001_2;
                                case 1:
                                    // btnRemove
                                    goto IL_1151;
                                case 2:
                                    View.frmRahmenSelect.Visible = false;
                                    goto end_IL_0001_2;
                                case 5:
                                    View.frmRahmenSelect.Visible = false;
                                    goto end_IL_0001_2;
                                case 8:
                                    btnSelCancel_Click(eventSender, eventArgs);
                                    goto end_IL_0001_2;
                                case 9:
                                    View.frmRahmenSelect.Visible = false;
                                    goto end_IL_0001_2;
                                case 12:
                                    //btnEnterNumber
                                    goto IL_2431;
                                default:
                                    goto end_IL_0001_2;
                            }


                        IL_1151:
                            num = 172;
                            btnSelFromFile_Click(eventSender, eventArgs);
                            goto end_IL_0001_2;
                        IL_2431:
                            num = 379;
                            btnEnterNumber_Click(eventSender, eventArgs);
                            goto end_IL_0001_2;
                        IL_2f1e:
                            num4 = unchecked(num2 + 1);
                            goto IL_2f22;
                        IL_2f22:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;

                                case 7:
                                case 52:
                                case 58:
                                case 169:
                                case 170:
                                case 204:
                                case 232:
                                case 267:
                                case 302:
                                case 344:
                                case 364:
                                case 367:
                                case 370:
                                case 374:
                                case 377:
                                case 387:
                                case 394:
                                case 405:
                                case 412:
                                case 431:
                                case 438:
                                case 453:
                                case 472:
                                case 485:
                                case 503:
                                case 511:
                                case 512:
                                case 517:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 14148;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 26
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    public void btnEnterNumber_Click(object eventSender, EventArgs eventArgs)
    {
        int left = View.frmFrame1.Tag.AsInt();
        string prompt;
        switch (left)
        {
            case (int)EUserText.tGodparents:
                Modul1_PersInArbsp = Modul1.PersInArb;
                prompt = sNumberOfPerson;
                Modul1.SuchPer = Interaction.InputBox(prompt, "Personensuche").AsInt();
                if (Modul1.SuchPer == 0)
                {
                    return;
                }

                if (Modul1.SuchPer > 0)
                {
                    if (DataModul.Person.Exists(Modul1.SuchPer))
                    {
                        _ = Interaction.MsgBox(sPersonNotExists);
                        return;
                    }
                    Modul1.eLKennz = ELinkKennz.lkGodparent;
                    Pate1(Modul1.SuchPer, Modul1_PersInArbsp, Modul1.eLKennz);
                }
                break;
            case (int)EUserText.tGodparentOf:
                Modul1_PersInArbsp = Modul1.PersInArb;
                prompt = sNumberOfPerson;
                Modul1.SuchPer = (int)Math.Round(Interaction.InputBox(prompt, "Personensuche").AsDouble());
                if (Modul1.SuchPer == 0)
                {
                    return;
                }
                if (Modul1.SuchPer > 0)
                {

                    if (DataModul.Person.Exists(Modul1.SuchPer))
                    {
                        _ = Interaction.MsgBox(sPersonNotExists);
                        return;
                    }
                    DataModul.Link.Append(Modul1.SuchPer, Modul1_PersInArbsp, Modul1.eLKennz = ELinkKennz.lkGodparent);
                }
                break;
            case 348:
                Modul1_PersInArbsp = Modul1.PersInArb;
                prompt = sNumberOfPerson;
                Modul1.SuchPer = Interaction.InputBox(prompt, "Personensuche").AsInt();
                if (Modul1.SuchPer == 0)
                {
                    return;
                }

                if (Modul1.SuchPer > 0)
                {

                    if (DataModul.Person.Exists(Modul1.SuchPer))
                    {
                        _ = Interaction.MsgBox(sPersonNotExists);
                        return;
                    }
                    Modul1.eLKennz = ELinkKennz.lk9;
                    Pate1(Modul1.SuchPer, Modul1_PersInArbsp, Modul1.eLKennz);
                }
                break;
            case 63:
                Modul1_PersInArbsp = Modul1.PersInArb;
                prompt = sNumberOfPerson;
                Modul1.SuchPer = Interaction.InputBox(prompt, "Personensuche").AsInt();
                if (Modul1.SuchPer > 0)
                {

                    if (DataModul.Person.Exists(Modul1.SuchPer))
                    {
                        _ = Interaction.MsgBox(sPersonNotExists);
                        return;
                    }
                    Modul1.eLKennz = ELinkKennz.lk9;
                    DataModul.Link.Append(Modul1.SuchPer, Modul1_PersInArbsp, Modul1.eLKennz);
                }
                break;
            default:
                Modul1_PersInArbsp = Modul1.PersInArb;
                prompt = sNumberOfPerson;
                Modul1.SuchPer = (int)Math.Round(Interaction.InputBox(prompt, "Personensuche").AsDouble());
                if (Modul1.SuchPer == 0)
                {
                    return;
                }
                Modul1_PerfamNr = Modul1_ErArt > EEventArt.eA_499 ? Familie.Default.iFamNr : Personen.Default.PersonNr;

                if (Modul1.SuchPer > 0)
                {

                    if (DataModul.Person.Exists(Modul1.SuchPer))
                    {
                        _ = Interaction.MsgBox(sPersonNotExists);
                        return;
                    }
                    DataModul.Witness.Append(Modul1_PerfamNr, Modul1.SuchPer, Modul1.eWKennz, Modul1_ErArt, Modul1.LfNR);
                }
                break;
        }
        View.Close();
        View.frmRahmenSelect.Visible = false;
        View.eResult = ERahmenResult.eRR_Removed;
        View.Close();
    }

    public void btnSelFromFile_Click(object eventSender, EventArgs eventArgs)
    {
        ResetCheckBoxState();
        Namensuch frmNamensuch = MainProject.Forms.Namensuch;
        EUserText left2 = View.iHdrText;
        if (left2 == EUserText.tGodparents
             || left2 == EUserText.t348)
        {
            if (!Handle_GodParent_Reenter())
                return;
        }
        else if (left2 == EUserText.tGodparentOf)
        {
            frmNamensuch.Show();
            if (frmNamensuch.List1.SelectedIndex > 10)
            {
                frmNamensuch.List1.TopIndex = frmNamensuch.List1.SelectedIndex - 5;
            }
            frmNamensuch.ComboBox2.Text = Modul1.IText[EUserText.t314];
            Modul1.Schalt = 2;
            Modul1.Suchfam = 0;
            Modul1.SuchPer = 0;
            _ = frmNamensuch.ComboBox1.Focus();
            frmNamensuch.ComboBox1.SelectionStart = frmNamensuch.ComboBox1.Text.Length;
            if (frmNamensuch.chbSelection.CheckState == CheckState.Unchecked)
            {
                frmNamensuch.ComboBox1.Text = "";
            }
            frmNamensuch.Visible = false;
            _ = frmNamensuch.ShowDialog();
            if (Modul1.SuchPer > 0)
            {
                if (DataModul.Person.Exists(Modul1.SuchPer))
                {
                    _ = Interaction.MsgBox(sPersonNotExists);
                    return;
                }
                else
                {
                    DataModul.Link.Append(Modul1.SuchPer, Modul1_PersInArbsp, Modul1.eLKennz = ELinkKennz.lkGodparent);
                }
            }
        }
        else if (left2 == EUserText.t303)
        {
            frmNamensuch.Show();
            if (frmNamensuch.List1.SelectedIndex > 10)
            {
                frmNamensuch.List1.TopIndex = frmNamensuch.List1.SelectedIndex - 5;
            }
            frmNamensuch.ComboBox2.Text = Modul1.IText[EUserText.t314];
            Modul1.Schalt = 2;
            Modul1.Suchfam = 0;
            Modul1.SuchPer = 0;
            _ = frmNamensuch.ComboBox1.Focus();
            frmNamensuch.ComboBox1.SelectionStart = frmNamensuch.ComboBox1.Text.Length;
            if (frmNamensuch.chbSelection.CheckState == CheckState.Unchecked)
            {
                frmNamensuch.ComboBox1.Text = "";
            }
            frmNamensuch.Visible = false;
            _ = frmNamensuch.ShowDialog();
            if (Modul1.SuchPer > 0)
            {

                if (DataModul.Person.Exists(Modul1.SuchPer))
                {
                    _ = Interaction.MsgBox(sPersonNotExists);
                    return;
                }
                else
                {
                    DataModul.Link.Append(Modul1.SuchPer, Modul1_PersInArbsp, Modul1.eLKennz = ELinkKennz.lk9);
                }
            }
        }
        else if (left2 == EUserText.t304)
        {
            frmNamensuch.Show();
            frmNamensuch.ComboBox2.Text = Modul1.IText[EUserText.t314];
            if (frmNamensuch.List1.SelectedIndex > 10)
            {
                frmNamensuch.List1.TopIndex = frmNamensuch.List1.SelectedIndex - 5;
            }
            Modul1.Schalt = 2;
            Modul1.Suchfam = 0;
            Modul1.SuchPer = 0;
            _ = frmNamensuch.ComboBox1.Focus();
            frmNamensuch.ComboBox1.SelectionStart = frmNamensuch.ComboBox1.Text.Length;
            if (frmNamensuch.chbSelection.CheckState == CheckState.Unchecked)
            {
                frmNamensuch.ComboBox1.Text = "";
            }
            frmNamensuch.Visible = false;
            _ = frmNamensuch.ShowDialog();
            if (Modul1.SuchPer > 0)
            {

                if (DataModul.Person.Exists(Modul1.SuchPer))
                {
                    _ = Interaction.MsgBox(sPersonNotExists);
                    return;
                }
                else
                {
                    Modul1.eLKennz = ELinkKennz.lk9;
                    Pate1(Modul1.SuchPer, Modul1_PersInArbsp, Modul1.eLKennz);
                    DataModul.Link.Append(Modul1.SuchPer, Modul1_PersInArbsp, Modul1.eLKennz);
                }
            }
        }
        else
        {
            Modul1.Suchschalt = 0;
            frmNamensuch.Show();
            if (frmNamensuch.List1.SelectedIndex > 10)
            {
                frmNamensuch.List1.TopIndex = frmNamensuch.List1.SelectedIndex - 5;
            }
            Modul1.Schalt = 2;
            Modul1.Suchfam = 0;
            Modul1.SuchPer = 0;
            _ = frmNamensuch.ComboBox1.Focus();
            frmNamensuch.ComboBox1.SelectionStart = frmNamensuch.ComboBox1.Text.Length;
            if (frmNamensuch.chbSelection.CheckState == CheckState.Unchecked)
            {
                frmNamensuch.ComboBox1.Text = "";
            }
            frmNamensuch.Visible = false;
            _ = frmNamensuch.ShowDialog();
            Modul1_PerfamNr = Modul1_ErArt > EEventArt.eA_499 ? Familie.Default.iFamNr : Personen.Default.PersonNr;

            if (Modul1.SuchPer > 0)
            {

                if (DataModul.Person.Exists(Modul1.SuchPer))
                {
                    _ = Interaction.MsgBox(sPersonNotExists);
                    return;
                }
                else
                {
                    DataModul.Witness.Append(Modul1_PerfamNr, Modul1.SuchPer, Modul1.eWKennz = 10, Modul1_ErArt, Modul1.LfNR);
                }
            }
        }
        View.frmRahmenSelect.Visible = false;
        View.eResult = ERahmenResult.eRR_Removed;
        Modul1.PersInArb = Personen.Default.PersonNr;
        View.Close();
    }

    public void btnSelCancel_Click(object eventSender, EventArgs eventArgs)
    {
        View.frmRahmenSelect.xEnReenter = true;
        View.frmRahmenSelect.Visible = false;
    }

    private bool Handle_GodParent_Reenter()
    {
        Namensuch frmNamensuch = MainProject.Forms.Namensuch;
        Modul1_PersInArbsp = Modul1.PersInArb;
        frmNamensuch.Show();
        if (frmNamensuch.List1.SelectedIndex > 10)
        {
            frmNamensuch.List1.TopIndex = frmNamensuch.List1.SelectedIndex - 5;
        }
        frmNamensuch.ComboBox2.Text = Modul1.IText[EUserText.t314];
        frmNamensuch.Label4.Visible = false;
        Modul1.Schalt = 2;
        Modul1.Suchfam = 0;
        Modul1.SuchPer = 0;
        _ = frmNamensuch.ComboBox1.Focus();
        frmNamensuch.ComboBox1.SelectionStart = frmNamensuch.ComboBox1.Text.Length;
        if (frmNamensuch.chbSelection.CheckState == CheckState.Unchecked)
        {
            frmNamensuch.ComboBox1.Text = "";
        }
        frmNamensuch.Visible = false;
        _ = frmNamensuch.ShowDialog();
        if (Modul1.SuchPer > 0)
        {

            if (DataModul.Person.Exists(Modul1.SuchPer))
            {
                _ = Interaction.MsgBox(sPersonNotExists);
                return true;
            }
            Modul1.eLKennz = ELinkKennz.lkGodparent;
            Pate1(Modul1.SuchPer, Modul1_PersInArbsp, Modul1.eLKennz);
        }
        View.eResult = ERahmenResult.eRR_Removed;
        return false;

    }

    private static void ResetCheckBoxState()
    {
        Namensuch frmNamensuch = MainProject.Forms.Namensuch;
        if (frmNamensuch.chbSelection.CheckState == CheckState.Unchecked)
        {
            frmNamensuch.chbMale.Checked = false;
            frmNamensuch.chbFemales.Checked = false;
            frmNamensuch.chbMale2.Checked = false;
            frmNamensuch.chbFemale2.Checked = false;
        }

    }

    public void btnSelReenter_Click(object eventSender, EventArgs eventArgs)
    {
        var dh = Personen.Default.FrmPerson_Do(View.frmFrame1.Tag.AsInt(), View.btnClose.PerformClick, View.Close);
        if (dh.Art != EEventArt.eA_Unknown)
            View.DataHolder = dh;
        Modul1.Ubg = 30;
        Modul1.Schalt = 3;
        View.eResult = ERahmenResult.eRR_OK;
    }

    public void Form_Load(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_3359
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int num5 = default;
        string text = default;
        string text2 = default;
        int lErl = default;
        string text3 = default;
        int num14 = default;
        int num17 = default;
        string text4 = default;
        string text5 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    string Persex;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0009;
                        case 15253:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                    case 3:
                                        break;
                                    case 1:
                                        goto IL_33db;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Information.Err().Number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_33db;
                                }
                                else
                                {
                                    _ = Interaction.MsgBox(Information.Err().Number.AsString(), mb: MessageBoxButtons.OK, title: Conversion.ErrorToString());
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num4 = num2;
                                    goto IL_33df;
                                }
                            }
                        end_IL_0001:
                            break;
                        IL_0009:
                            num = 2;
                            bool flag = true;
                            if (Modul1.FontSize > 0f)
                            {
                                View.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                View.RTB.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                View.List4.Font = new Font("Courier new", Modul1.FontSize, FontStyle.Regular);
                                View.frmFrame1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                View.frmRahmenSelect.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                View.btnClose.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                View.btnAppend.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                View.btnDelete.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                            }
                            View.RTB.RightMargin = View.RTB.Width - 40;
                            List4_Items.Clear();
                            int ubg = Modul1.Ubg;
                            switch (ubg)
                            {
                                case 1:
                                    Do1();
                                    break;
                                case 2:
                                    Do2();
                                    flag = false;
                                    break;
                                case 3:
                                    Do3(text);
                                    break;
                                case 4:
                                    int num10;
                                    Do4(out num10, ref text2, out Persex);
                                    break;
                                case 5:

                                    Do5(ref text2, ubg);
                                    break;
                                case 6 or 7 or 8:

                                    Do678(ubg);
                                    flag = false;
                                    break;
                                case 9:
                                    if (Do9(ref text, ref text3, ref num14))
                                        flag = false;
                                    break;
                                case 10 or 11:
                                    short num15;
                                    Do10_11(out num15, ref num17, ref text4, ref text5);
                                    flag = false;
                                    break;
                                case 101 or 102 or 103 or 104 or 105 or 106 or 300 or 301 or 302 or 500 or 501 or 502 or 503 or 504 or 505 or 506 or 507 or 602 or 603:
                                    num3 = Do100pp(ref num5);
                                    flag = false;
                                    break;
                            }
                            if (flag)
                            {
                                Modul1.PersInArb = Personen.Default.PersonNr;
                                var pt = DataModul.Person.Seek(Modul1.PersInArb);
                                if (pt.Fields[PersonFields.Bem2].AsString().Length >= 0)
                                {
                                    View.RTB.SelectedText = pt.Fields[PersonFields.Bem2].AsString();
                                }
                                Modul1.Art = Modul1_ErArt;
                            }
                            goto end_IL_0001_2;
                        IL_33db:
                            num4 = unchecked(num2 + 1);
                            goto IL_33df;
                        IL_33df:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 62:
                                case 279:
                                case 286:
                                case 440:
                                case 441:
                                case 473:
                                case 483:
                                case 490:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 15253;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 8
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void Do1()
    {
        foreach (int persInArb in _aiPers)
        {
            Modul1.Person_ReadNames(persInArb, Modul1.Person);
            Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person));
            Modul1_LiText = Strings.Left(Modul1.Person.FullSurName.Trim() + ", " + Modul1.Person.Givennames + "                                                         ", 48) + "          " + persInArb.AsString().Right(10);
            _ = List4_Items.Add(Modul1_LiText);

        }
    }

    private void Do2()
    {
        foreach (int persInArb in _aiPers)
        {
            int iBpYear = DataModul.Event.GetPersonBirthOrBapt(persInArb, true).Year;
            Modul1.sDatu = $"{(iBpYear == 0 ? "" : iBpYear.AsString()),10}";
            Modul1.Person_ReadNames(persInArb, Modul1.Person);
            Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person));
            Modul1_LiText = Modul1.sDatu + " " + Strings.Left(Modul1.Person.FullSurName.Trim() + ", " + Modul1.Person.Givennames + "                                                         ", 37) + "          " + persInArb.AsString().Right(10);
            _ = List4_Items.Add(new ListItem(Modul1_LiText, persInArb));
        }
    }

    private void Do3(string text)
    {
        // Fill List4 with all selected families
        foreach (var item in _atFam)
        {
            View.btnAppend.Visible = false;
            View.btnDelete.Visible = false;
            text = Family_GetDescription(item.iFamily);
            _ = List4_Items.Add(new ListItem(text, item.iFamily));
        }
    }

    private string Family_GetDescription(int iFamily)
    {
        EEventArt[] aeConEvent = { EEventArt.eA_Marriage, EEventArt.eA_MarrReligious, EEventArt.eA_501, EEventArt.eA_500 };
        int iLkYear = default;
        string sResult = "";

        foreach (var e in aeConEvent)
            if (iLkYear != 0) break;
            else
                iLkYear = DataModul.Event.GetDate(e, iFamily).Year;
        sResult = iLkYear == 0 ? new string(' ', 10) : $"{iLkYear,10}";
        DataModul.Link.ReadFamily(iFamily, Modul1.Family);

        // o01_Person 1 description
        sResult += " " + Person_GetDescription(Modul1.Family.Mann);

        // o01_Person connector (if any) 
        // ToDo: I18N
        sResult += " und";

        // o01_Person 2 description
        sResult += " " + Person_GetDescription(Modul1.Family.Frau);

        sResult = $"{sResult}{"",60}{iFamily.AsString()}";
        return sResult;

        string Person_GetDescription(int iPersNr)
        {
            Modul1.Person_ReadNames(iPersNr, Modul1.Person);
            return $"{Modul1.Person.SurName.Trim().ToUpper()}, {Modul1.Person.Givennames}{null,18}".Left(20).Trim();
        }
    }

    private void Do4(out int num10, ref string text2, out string Persex)
    {
        EEventArt[] aeConEvent = {
            EEventArt.eA_500,  EEventArt.eA_501,  EEventArt.eA_Marriage,  EEventArt.eA_MarrReligious,
            EEventArt.eA_504,  EEventArt.eA_505,  EEventArt.eA_506,  EEventArt.eA_507, EEventArt.eA_601
        };

        checked
        {
            View.btnAppend.Visible = false;
            View.btnDelete.Visible = false;
            View.iHdrText = EUserText.tMarrTo;
            Persex = Personen.Default.edtSex.Text;
            var aiMarr = Modul1.Ehesuch(Personen.Default.PersonNr, Persex: Persex);
            List4_Items.Clear();
            Modul1.eLKennz = Personen.Default.edtSex.Text == "M" ? ELinkKennz.lkMother : ELinkKennz.lkFather;

            num10 = aiMarr.Count;

            foreach (int num11 in aiMarr)
            {
                int iLkYear = default;
                EEventArt eDateArt = default;
                foreach (var e in aeConEvent)
                    if (iLkYear != 0) break;
                    else
                    {
                        iLkYear = DataModul.Event.GetDate(e, num11).Year;
                        eDateArt = e;
                    }

                Modul1.sDatu = eDateArt == EEventArt.eA_601 ? $"{iLkYear,4} F" : $"{iLkYear,4}  ";
                if (DataModul.Link.GetFamPerson(num11, Modul1.eLKennz, out int iPersNr))
                {
                    Modul1.PersInArb = iPersNr;
                    Modul1.Person_ReadNames(iPersNr, Modul1.Person);
                    Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(iPersNr, out int iAhn, out Modul1_Kont20);
                    Modul1.Kont[97] = iAhn.AsString();
                    Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person));
                    Modul1_LiText = new string(' ', 80);
                    if (!string.IsNullOrWhiteSpace(Modul1.Person.FullSurName) || !string.IsNullOrWhiteSpace(Modul1.Person.Givennames))
                    {
                        StringType.MidStmtStr(ref Modul1_LiText, 1, 60, "" + Modul1.sDatu.Right(6) + " " + Modul1.Person.Givennames + " " + Modul1.Person.FullSurName.TrimEnd());
                    }
                    else
                    {
                        StringType.MidStmtStr(ref Modul1_LiText, 1, 60, "" + Modul1.sDatu.Right(6) + " namenlosem Partner");
                    }
                    StringType.MidStmtStr(ref Modul1_LiText, 61, 19, num11.AsString());
                    _ = List4_Items.Add(Modul1_LiText);
                }
                else
                {
                    text2 = " unbekanntem Partner";
                    Modul1_LiText = new string(' ', 80);
                    StringType.MidStmtStr(ref Modul1_LiText, 1, 60, "" + Modul1.sDatu.Right(6) + text2);
                    StringType.MidStmtStr(ref Modul1_LiText, 61, 19, num11.AsString());
                    _ = List4_Items.Add(Modul1_LiText);
                }
            }
        }
    }

    private void Do5(ref string text2, int ubg)
    {
        EEventArt[] aeConEvent = {
            EEventArt.eA_500,  EEventArt.eA_501,  EEventArt.eA_Marriage,  EEventArt.eA_MarrReligious,
            EEventArt.eA_504,  EEventArt.eA_505,  EEventArt.eA_506,  EEventArt.eA_507, EEventArt.eA_601
        };

        checked
        {
            View.iHdrText = EUserText.tMarrTo;
            View.btnAppend.Visible = false;
            View.btnDelete.Visible = false;
            foreach (var num11 in _aiPers)
            {
                int iLkYear = default;
                EEventArt eDateArt = default;
                foreach (var e in aeConEvent)
                    if (iLkYear != 0) break;
                    else
                    {
                        iLkYear = DataModul.Event.GetDate(e, num11).Year;
                        eDateArt = e;
                    }


                Modul1_LiText = new string(' ', 80);
                int persInArb = 0;
                Modul1.sDatu = eDateArt == EEventArt.eA_601 ? $"{iLkYear,4} F" : $"{iLkYear,4}  ";
                if (DataModul.Link.GetFamPerson(num11, Modul1.eLKennz, out int iPerNr))
                {
                    persInArb = iPerNr;
                    Modul1.Person_ReadNames(persInArb, Modul1.Person);
                    Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(iPerNr, out int iAhn, out Modul1_Kont20);
                    Modul1.Kont[97] = iAhn.AsString();
                    if (!string.IsNullOrWhiteSpace(Modul1.Person.SurName) || !string.IsNullOrWhiteSpace(Modul1.Person.Givennames))
                    {
                        StringType.MidStmtStr(ref Modul1_LiText, 1, 60, "" + Modul1.sDatu.Right(6) + " " + Modul1.Person.Givennames + " " + Modul1.Person.SurName.ToUpper().TrimEnd());
                    }
                    else
                    {
                        StringType.MidStmtStr(ref Modul1_LiText, 1, 60, "" + Modul1.sDatu.Right(6) + " namenlosem Partner");
                    }
                    StringType.MidStmtStr(ref Modul1_LiText, 61, 19, num11.AsString());
                    _ = List4_Items.Add(new ListItem(Modul1_LiText, persInArb));
                }
                else
                {
                    text2 = " Unbekannter Partner";
                    StringType.MidStmtStr(ref Modul1_LiText, 1, 60, "    " + Modul1.sDatu.Right(6) + text2);
                    StringType.MidStmtStr(ref Modul1_LiText, 61, 19, num11.AsString());
                    _ = List4_Items.Add(new ListItem(Modul1_LiText, -1));
                }
            }
        }
    }

    private void Do678(int eLKnz)
    {
        if (eLKnz == 6)
        {
            View.iHdrText = EUserText.tMarrWitness;
        }
        else if (eLKnz == 7)
        {
            View.iHdrText = EUserText.tWitnOfEngage;
            View.Height = 232;
        }
        else if (eLKnz == 8)
        {
            View.iHdrText = EUserText.tWitnOfMarr;
            View.Height = 232;
        }
        List4_Items.Clear();
        foreach (int persInArb in _aiPers)
        {
            Modul1.Person_ReadNames(persInArb, Modul1.Person);
            Modul1_LiText = Strings.Left(Modul1.Person.SurName.Trim().ToUpper() + ", " + Modul1.Person.Givennames + "                                                         ", 49) + "          " + persInArb.AsString().Right(10);
            _ = List4_Items.Add(new ListItem(Modul1_LiText, persInArb));
        }

    }

    private bool Do9(ref string text, ref string text3, ref int num14)
    {

        Modul1.PersInArb = Personen.Default.PersonNr;
        List4_Items.Clear();
        var _aiFams = DataModul.Link.GetPersonFams(Modul1.PersInArb, ELinkKennz.lkAdoptedChild);
        if (_aiFams.Count == 0)
            return true;
        string liText;
        int persInArb = 0;
        foreach (int famInArb in _aiFams)
        {
            liText = "";
            DataModul.Link.ReadFamily(famInArb, Modul1.Family);
            persInArb = Modul1.Family.Mann;
            Modul1.Person_ReadNames(persInArb, Modul1.Person);
            Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person));
            text = Strings.Trim(Strings.Left(Modul1.Person.FullSurName.Trim() + "," + Modul1.Person.Givennames + "                    ", 20));
            liText = liText + " " + text;
            persInArb = Modul1.Family.Frau;
            Modul1.Person_ReadNames(persInArb, Modul1.Person);
            Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person));
            text = Strings.Trim(Strings.Left(Modul1.Person.FullSurName.Trim() + "," + Modul1.Person.Givennames + "                    ", 20));
            liText = liText + " und " + text;
            liText = liText + "                                                                ".Left(60) + famInArb.AsString();
            _ = List4_Items.Add(new ListItem(liText, famInArb));
        }
        return false;

    }


    private void Do10_11(out short num15,
        ref int num17,
        ref string text4,
        ref string text5)
    {
        num15 = (short)Modul1.Ubg;
        Modul1_LiText = "";
        int i = 1;
        string text;
        foreach (var item in _atEKFam)
        {
            if (item.eArt > EEventArt.eA_499)
            {
                Modul1.FamInArb = item.iFamily;
                DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                Modul1.PersInArb = Modul1.Family.Mann;
                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                text = Strings.Trim(Strings.Left(Modul1.Person.SurName.Trim().ToUpper() + "," + Modul1.Person.Givennames + "                    ", 20));
                Modul1_LiText = Modul1_LiText + " " + text;
                Modul1.PersInArb = Modul1.Family.Frau;
                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                text = Strings.Trim(Strings.Left(Modul1.Person.SurName.Trim().ToUpper() + "," + Modul1.Person.Givennames + "                    ", 20));
                Modul1_LiText = Modul1_LiText + " und " + text;
            }
            else
            {
                Modul1.PersInArb = item.iFamily;
            }
            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
            Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out int iAhn, out Modul1_Kont20);
            Modul1.Kont[97] = iAhn.AsString();
            num17 = Personen.Default.PersonNr;
            (EEventArt eArt, int iLink, short iLfNR) key = default;
            if (num15 == 10)
            {
                key = ((EEventArt)Modul1_Per1[i * 3], num17, (short)Modul1_Per1[i * 3 + 1]);
            }
            else
            {
                if (num15 == 11)
                {
                    if (item.eArt > EEventArt.eA_499)
                    {
                        key = (item.eArt, Modul1.FamInArb, item.iKnz);
                    }
                    else
                    {
                        key = ((EEventArt)Modul1_Per1[i * 3], Modul1.PersInArb, (short)Modul1_Per1[i * 3 + 1]);
                    }
                }
            }
            text4 = DataModul.Event.GetDate(key.eArt, key.iLink).Year.AsString();
            if (text4.AsInt() == 0.0)
            {
                text4 = "";
            }
            text5 = "";
            var left = " " + item.eArt.AsString();
            text5 = item.eArt switch
            {
                EEventArt.eA_Birth => " " + text4 + "(" + Modul1.IText[EUserText.t264] + ")",
                EEventArt.eA_Baptism => " " + text4 + "(Taufe)",
                EEventArt.eA_Death => " " + text4 + "(Tod)",
                EEventArt.eA_Burial => " " + text4 + "(Begraben)",
                EEventArt.eA_105 => " " + text4 + "(Sonst.Datum)",
                EEventArt.eA_106 => " " + text4 + "(Heimatort)",
                EEventArt.eA_300 => " " + text4 + "(Beruf)",
                EEventArt.eA_301 => " " + text4 + "(Titel)",
                EEventArt.eA_302 => " " + text4 + "(Wohnort)",
                EEventArt.eA_500 => " " + text4 + "(Proklamation)",
                EEventArt.eA_501 => " " + text4 + "(Verlobung)",
                EEventArt.eA_Marriage => " " + text4 + "(Heirat)",
                EEventArt.eA_MarrReligious => " " + text4 + "(Kirchl. Heir.)",
                EEventArt.eA_504 => " " + text4 + "(Scheidung)",
                EEventArt.eA_505 => " " + text4 + "(Eheänl. Beziehung)",
                EEventArt.eA_506 => " " + text4 + "(Eheänl. Beziehung)",
                EEventArt.eA_507 => " " + text4 + "(Dimissiorale)",
                _ => "",
            };
            if (item.eArt < EEventArt.eA_499)
            {
                if (Modul1.Person.Prefix.Trim() != "")
                {
                    Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName.Trim());
                }
                else
                    Modul1.Person.SetFullSurname(Modul1.Person.SurName.Trim());
                Modul1_LiText = Strings.Left(text5 + " " + Modul1.Person.FullSurName.Trim().ToUpper() + ", " + Modul1.Person.Givennames.Trim() + Strings.Space(50), 48) + "          " + Modul1.PersInArb.AsString().Right(10);
            }
            else
            {
                Modul1_LiText = text5 + Modul1_LiText + Strings.Space(60).Left(58) + "          " + Modul1.FamInArb.AsString().Right(10);
            }
            _ = List4_Items.Add(new ListItem(Modul1_LiText, item));
            Modul1_LiText = "";
        }
        if (List4_Items.Count == 0)
        {
            View.Height = 376;
            View.RTB.Text = "Nur Textzeugen, Anzeige hier nicht möglich";
        }
    }

    private int Do100pp(ref int num5)
    {
        int num3;
        ProjectData.ClearProjectError();
        num3 = 3;
        foreach (var persInArb in _aiPers)
        {
            Modul1.Person_ReadNames(persInArb, Modul1.Person);
            Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out int iAhn, out Modul1_Kont20);
            Modul1.Kont[97] = iAhn.AsString();
            if (Modul1.Person.Prefix.Trim() != "")
            {
                Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName.Trim());
            }
            Modul1_LiText = Strings.Left(Modul1.Person.FullSurName.Trim().ToUpper() + ", " + Modul1.Person.Givennames + "                                                         ", 48) + "          " + persInArb.AsString().Right(10);
            _ = List4_Items.Add(new ListItem(Modul1_LiText, persInArb));
        }

        Modul1_PerfamNr = Modul1_ErArt > EEventArt.eA_499 ? Familie.Default.iFamNr : Personen.Default.PersonNr;
        View.RTB.Text = DataModul.Event.GetValue((Modul1_ErArt, Modul1_PerfamNr, Modul1.LfNR), EventFields.Bem4, View.RTB.Text);
        View.frmFrame1.Text = "Zeugen: " + MainProject.Forms.Ereignis.Label10.Text;
        View.frmFrame1.Tag = 64;
        return num3;
    }


    private void List4_SelectedIndexChanged(object eventSender, EventArgs eventArgs)
    {
        if (!Personen.Default.btnSaveNExit.Visible)
        {
            if (Personen.Default.btnSaveToFamily.Visible)
            {
                _ = Interaction.MsgBox(" Sie können den Paten nicht bearbeiten ohne die Person zu speichern.");
                return;
            }
            Modul1.PersInArb = View.List4.SelectedItem.ItemData<int>();
            View.btnDelete.Enabled = true;
        }
    }

    private void List4_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        int ubg2 = 1;
        Ramentextspeich();
        if (View.List4.SelectedIndex < 0)
        {
            goto end_IL_0001_2;
        }
        var forms = MainProject.Forms;
        Form Formtocheck = MainProject.Forms.Personen;
        var num7 = Modul1.IsFormloaded(Formtocheck);
        MainProject.Forms.Personen = (Personen)Formtocheck;
        if (num7 == -1)
        {
            if (Personen.Default.btnSaveNExit.Visible)
            {
                goto end_IL_0001_2;
            }
            Modul1.PersInArb = Personen.Default.PersonNr;
            _ = Personen.Default.AendPruef(Modul1.PersInArb, ubg2);
        }
        if (View.iHdrText == EUserText.tMarrTo
            || View.iHdrText == EUserText.t302
            || View.iHdrText == EUserText.t460) //"Adoptiert von"
        {
            Modul1.FamInArb = List4_Items[View.List4.SelectedIndex].ItemData<int>();
            View.frmFrame1.Visible = false;
            Personen.Default.Close();
            View.Close();
            if (Modul1_Tast == 5)
            {
                View.Close();
            }
            short Rich = 3;
            Familie.Default.Show(Modul1.FamInArb);
        }
        else
        {
            if (View.iHdrText == EUserText.tMarrWitness
                || View.iHdrText == EUserText.tWitnOfEngage
                || View.iHdrText == EUserText.tWitnOfMarr)
            {
                Modul1.PersInArb = List4_Items[View.List4.SelectedIndex].ItemData<int>();
                if (Modul1.PersInArb == 0)
                {
                    goto end_IL_0001_2;
                }
                View.Close();
                Modul1.Ad = false;
                Personen.Default.Show(Modul1.PersInArb, EUserText.t158);
                Modul1.Aend = 0f;
            }
            else
            {
                if (View.frmFrame1.Tag.AsInt() == 64)
                {
                    if (View.List4.SelectedItem.AsString().Trim().Length == 0)
                    {
                        goto end_IL_0001_2;
                    }
                    if (List4_Items[View.List4.SelectedIndex].AsString().Length <= 0)
                    {
                        Debugger.Break();
                    }
                    Modul1_PerZeug = 0;
                    Modul1_PerZeug = List4_Items[View.List4.SelectedIndex].ItemData<int>();
                    if (Modul1_PerZeug == 0)
                    {
                        goto end_IL_0001_2;
                    }
                    View.Close();
                    View.eResult = ERahmenResult.eRR25;
                    MainProject.Forms.Ereignis.Button5.PerformClick();
                    View.Close();
                    Modul1.PersInArb = 0;
                    Modul1.Ad = false;
                    Modul1.PersInArb = Modul1_PerZeug;
                    Familie.Default.Close();
                    Modul1.Aend = 0f;
                    Personen.Default.Show(Modul1.PersInArb, EUserText.t158);
                }
                else
                {
                    View.eResult = ERahmenResult.eRR_OK;
                    int num6 = 0;
                    int num4 = 0;
                    if (List4_Items[View.List4.SelectedIndex].AsString().Length < 60)
                    {
                        num6 = (int)Math.Round(Conversion.Val(Strings.Mid(List4_Items[View.List4.SelectedIndex].AsString(), 49, 10)));
                    }
                    else
                    {
                        num4 = (int)Math.Round(Conversion.Val(Strings.Mid(List4_Items[View.List4.SelectedIndex].AsString(), 59, 10)));
                    }
                    View.Close();
                    forms = MainProject.Forms;
                    Formtocheck = MainProject.Forms.Ereignis;
                    var num8 = Modul1.IsFormloaded(Formtocheck);
                    MainProject.Forms.Ereignis = (FrmEreignis)Formtocheck;
                    if (num8 == -1)
                    {
                        MainProject.Forms.Ereignis.Button5.PerformClick();
                    }
                    Familie.Default.Close();
                    if (num4 > 0)
                    {
                        Personen.Default.Close();
                        Modul1.FamInArb = num4;
                        short Rich = 3;
                        Familie.Default.Show(Modul1.FamInArb);
                    }
                    else
                    {
                        if (num6 != 0)
                        {
                            Modul1.PersInArb = num6;
                            Personen.Default.Show(Modul1.PersInArb, EUserText.tNone);
                        }
                        View.Close();
                        View.eResult = ERahmenResult.eRR_OK;
                        Modul1.Aend = 0f;
                    }
                }
            }
        }
    end_IL_0001_2:;
    }
    private void RTB_KeyDown(object eventSender, KeyEventArgs eventArgs)
    {
        //Discarded unreachable code: IL_00d9
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num5;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 457:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_014f;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Information.Err().Number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_014f;
                                }
                                else
                                {
                                    _ = Interaction.MsgBox(Information.Err().Number.AsString(), title: Conversion.ErrorToString(), mb: MessageBoxButtons.OK);
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num5 = num2;
                                    goto IL_0153;
                                }
                            }
                        end_IL_0001:
                            break;
                        IL_0008:
                            num = 2;
                            short num6 = (short)eventArgs.KeyCode;
                            short num4 = (short)unchecked((int)eventArgs.KeyData / 65536);
                            if (num4 != 0)
                            {
                                goto end_IL_0001_2;
                            }
                            if (Modul1.Trans == 0)
                            {
                                Modul1.Trans = 1;
                            }
                            switch (num6)
                            {
                                case 113:
                                case 114:
                                case 115:
                                case 116:
                                case 117:
                                case 118:
                                case 119:
                                case 120:
                                case 121:
                                case 122:
                                case 123:
                                    View.RTB.SelectedText = Modul1.Te[num6 - 113];
                                    View.RTB.SelectionStart = View.RTB.Text.Length;
                                    break;
                                default:
                                    break;
                            }
                            goto end_IL_0001_2;
                        IL_014f:
                            num5 = unchecked(num2 + 1);
                            goto IL_0153;
                        IL_0153:
                            num2 = 0;
                            switch (num5)
                            {
                                case 1:
                                    break;
                                case 9:
                                case 14:
                                case 15:
                                case 16:
                                case 17:
                                case 18:
                                case 19:
                                case 26:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 457;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 4
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    private void RTB_Leave(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_0445
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                MainProject.MyForms forms;
                Form Formtocheck;
                short num5;
                switch (try0001_dispatch)
                {
                    default:
                        num = 1;
                        if (Modul1.Typ == DriveType.CDRom)
                        {
                            goto end_IL_0001;
                        }
                        goto IL_0018;
                    case 1463:
                        {
                            num2 = num;
                            switch (num3 <= -2 ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_04c9;
                                default:
                                    goto end_IL_0001_2;
                            }
                            if (Information.Err().Number == 94)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_04c9;
                            }
                            else
                            {
                                if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_04cd;
                            }
                        }
                    end_IL_0001_2:
                        break;
                    IL_0018:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        forms = MainProject.Forms;
                        Formtocheck = forms.Ereignis;
                        num5 = Modul1.IsFormloaded(Formtocheck);
                        forms.Ereignis = (FrmEreignis)Formtocheck;
                        if (num5 != -1)
                        {
                            goto end_IL_0001;
                        }
                        Modul1_PerfamNr = Modul1_ErArt > EEventArt.eA_499 ? Familie.Default.iFamNr : Personen.Default.PersonNr;
                        DataModul.Event.SetValues((Modul1_ErArt, Modul1_PerfamNr, Modul1.LfNR), new[] { (EventFields.Bem4, (object)View.RTB.Text.Trim()) });
                        goto end_IL_0001;
                    IL_04c9:
                        num4 = num2 + 1;
                        goto IL_04cd;
                    IL_04cd:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                            case 18:
                            case 44:
                            case 45:
                            case 46:
                            case 55:
                                goto end_IL_0001;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 1463;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001: // <========== 4
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void Pate1(int iSuchPer, int iPerFam, ELinkKennz eLKennz)
    {
        //Discarded unreachable code: IL_0136
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                switch (try0001_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_Start;
                    case 527:
                        {
                            num2 = num;
                            switch (num3 <= -2 ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_01af;
                                default:
                                    goto end_IL_0001;
                            }
                            if (Information.Err().Number == 94)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_01af;
                            }
                            else
                            {
                                if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_01b2;
                            }
                        }
                    end_IL_0001:
                        break;
                    IL_Start:
                        num = 2;
                        DataModul.Link.Append(iPerFam, iSuchPer, eLKennz);
                        goto end_IL_0001_2;
                    IL_01af:
                        num4 = num2 + 1;
                        goto IL_01b2;
                    IL_01b2:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 527;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void Ramentextspeich()
    {
        //Discarded unreachable code: IL_034b
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 1153:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_03b9;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Information.Err().Number == 3021)
                                {
                                    goto end_IL_0001_2;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_03bc;
                            }
                        end_IL_0001:
                            break;
                        IL_0008:
                            num = 2;
                            if (Modul1.Typ == DriveType.CDRom)
                            {
                                goto end_IL_0001_2;
                            }
                            if (Personen.Default.Visible)
                            {
                                Modul1.PersInArb = Personen.Default.PersonNr;
                            }
                            Modul1.Art = Modul1_ErArt;
                            if (Familie.Default.Visible)
                            {
                                Modul1.PersInArb = Familie.Default.iFamNr;
                            }
                            if (MainProject.Forms.Ereignis.Visible)
                            {
                                if (!DataModul.Event.UpdateValues((Modul1.Art, Modul1.PersInArb, Modul1.LfNR), new[] { (EventFields.Bem4, (object)View.RTB.Text) }))
                                {
                                    goto end_IL_0001_2;
                                }
                            }
                            else
                            {
                                if (Personen.Default.Visible)
                                {
                                    Modul1.PersInArb = Personen.Default.PersonNr;
                                    if (View.frmFrame1.Tag.AsInt() == (int)EUserText.tGodparents)
                                    {
                                        DataModul.DB_PersonTable.Edit();
                                        DataModul.DB_PersonTable.Fields[PersonFields.Bem2].Value = View.RTB.Text;
                                        DataModul.DB_PersonTable.Update();
                                    }
                                }
                                if (Familie.Default.Visible)
                                {
                                    Modul1.FamInArb = Familie.Default.iFamNr;
                                    DataModul.DB_FamilyTable.Edit();
                                    if (View.Height != 365 && View.frmFrame1.Tag.AsInt() == (int)EUserText.tMarrWitness)
                                    {
                                        DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].Value = View.RTB.Text;
                                    }
                                    DataModul.DB_FamilyTable.Update();
                                }
                                View.frmRahmenSelect.Visible = false;
                            }
                            goto end_IL_0001_2;
                        IL_03b9:
                            num4 = unchecked(num2 + 1);
                            goto IL_03bc;
                        IL_03bc:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 13:
                                case 18:
                                case 37:
                                case 38:
                                case 40:
                                case 46:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 1153;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 5
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }



}
