using BaseLib.Helper;
using CommunityToolkit.Mvvm.Input;
using GenFreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using Microsoft.VisualBasic;
using MVVM.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GenFreeWin.ViewModels;

public partial class FamilieViewModel : BaseViewModelCT, IFamilieViewModel
{
    IContainerControl IFamilieViewModel.View { get; set; }
    Familie View => (Familie)((IFamilieViewModel)this).View;
    private const int ErrUser14 = -2146828237;

    private IModul1 Modul1 => _Modul1.Instance;
    [Obsolete]
    IProjectData ProjectData => Modul1.ProjectData;
    IVBInformation Information => Modul1.Information;
    IStrings Strings => Modul1.Strings;
    IStrings StringType => Modul1.Strings;

    IVBConversions Conversion => Modul1.Conversions;
    IVBConversions Conversions => Modul1.Conversions;
    IInteraction Interaction => Menue.Default;

    public int iFamNr { get; set; }

    public IPersonRedViewModel Father { get; private set; }

    public IPersonRedViewModel Mother { get; private set; }

    private string Msg;

    public int iKindSich;
    private bool Ad;
    private int Datfehler_Abbruch;
    private string Datu;
    private int Kindneu;
    private string? LiText;
    private short[] Posi;
    private string Kont_3;
    private static (string, ETextKennz) Modul1_Bezeichnu;

    #region Public Methods
    public void Clear()
    {
        View.ComboBox1.Items.Clear();
        View.ComboBox2.Items.Clear();
        View.ComboBox1.Text = "";
        View.ComboBox2.Text = "";
        View.List2.Items.Clear();
        View.lstChildren.Items.Clear();
        View.iFamNr = 0;
        View.fraFather.Clear(EUserText.t162);
        View.fraMother.Clear(EUserText.t163);
        View.Label24.Text = "";
        View.Label25.Text = "";
        View.lblEstimatedMarr.Text = Modul1.IText[EUserText.t260];
        View.lblMarriage.Text = Modul1.IText[EUserText.tMarriage];
        View.lblMarriage.Refresh();
        View.lblDimissorale.Text = Modul1.IText[EUserText.t261];
        View.RichTextBox1.Text = "";
        View.btnSearchNumber.Text = Modul1.IText[EUserText.t272];
        View.btnSearchName.Text = Modul1.IText[EUserText.t273];
        View.btnReenter.Text = Modul1.IText[EUserText.t73];
        View.btnNext.Text = Modul1.IText[EUserText.t155];
        View.btnPrevious.Text = Modul1.IText[EUserText.t156];
        View.btnSearchPartner.Text = Modul1.IText[EUserText.t274];
        View.btnSearchRegister.Text = Modul1.IText[EUserText.t275];
        View.btnResarch.Text = Modul1.IText[EUserText.t276];
        View.btnFamilysheet.Text = Modul1.IText[EUserText.t277];
        View.btnEndTextinput.Text = Modul1.IText[EUserText.t159];
        View.btnDelete.Text = Modul1.IText[EUserText.tDelete];
        View.btnChildren.Text = "&" + Modul1.IText[EUserText.tChild_AS];
        View.btnResidence.Text = Modul1.IText[EUserText.tResidence];
        View.btnConfirmation.Text = Modul1.IText[EUserText.tConfirmation];
        View.TextBox4.Text = "";
        View.edtNameprefix.Text = "";
        View.edtNameSuffix.Text = "";
        View.frmAppendPerson.Visible = false;
        View.lblAppLabel44.Text = "";
        View.lblAppLabel44.Tag = 0;
        View.cbxIllegitRel.Text = Modul1.IText[EUserText.tIllegalRel];
        View.cbxIllegitRel.CheckState = CheckState.Unchecked;
    }

    public void Famles(int FamNr, IFamilyData family)
    {
        if (FamNr == 0)
        {
            return;
        }
        View.Label46.Text = "";
        View.Label46.Tag = 0;
        View.Label46.Visible = false;
        View.Label45.Text = "";
        View.Label45.Tag = 0;
        View.Label45.Visible = false;
        View.Label47.Text = "";
        View.Label47.Visible = false;
        View.Label47.Tag = 0;
        void HandleExtraEvents(ELinkKennz i, int p)
        {
            switch (i)
            {
                case ELinkKennz.lkMarrWitness:
                    View.Label46.Text = Modul1.IText[EUserText.tMarrWitness] + " " + Modul1.IText[EUserText.tYes];
                    View.Label46.Tag = p;
                    View.Label46.Visible = true;
                    break;
                case ELinkKennz.lkWitnOfEngage:
                    View.Label45.Text = Modul1.IText[EUserText.tWitnOfEngage] + " " + Modul1.IText[EUserText.tYes];
                    View.Label45.Tag = p;
                    View.Label45.Visible = true;
                    break;
                case ELinkKennz.lkWitnOfMarr:
                    View.Label47.Text = Modul1.IText[EUserText.tWitnOfMarr] + " " + Modul1.IText[EUserText.tYes];
                    View.Label47.Tag = p;
                    View.Label47.Visible = true;
                    break;
                default:
                    break;
            }
        }
        DataModul.Link.ReadFamily(FamNr, family, HandleExtraEvents);
    }
    public void FamdatPrüfen(int famInArb)
    {
        //Discarded unreachable code: IL_103e
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        string DDatum = default;
        int lErl = default;
        string HT = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    string DDatum2;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            if (Modul1.Typ == DriveType.CDRom)
                            {
                                goto end_IL_0001;
                            }
                            goto IL_0018;
                        case 5284:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_10ce;
                                    default:
                                        goto end_IL_0001_2;
                                }
                                if (Information.Err().Number == 13)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_10ce;
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
                                    goto IL_10d2;
                                }
                            }
                        end_IL_0001_2:
                            break;
                        IL_0018:
                            num = 4;
                            float[] array3 = new float[5];
                            Datfehler_Abbruch = 0;

                            if (View.btnEnableCheck.Tag.AsInt() == 1)
                            {
                                goto end_IL_0001;
                            }
                            array3[1] = 10f;
                            if (Modul1.Aus[(int)EOutCfg.o09] != "")
                            {
                                array3[1] = Modul1.Aus[(int)EOutCfg.o09].AsInt();
                            }
                            array3[1] *= 10000f;
                            array3[2] = 20f;
                            if (Modul1.Aus[(int)EOutCfg.o08] != "")
                            {
                                array3[2] = Modul1.Aus[(int)EOutCfg.o08].AsInt();
                            }
                            array3[2] *= 10000f;
                            array3[3] = 20f;
                            if (Modul1.Aus[(int)EOutCfg.o10_EmitIDs] != "")
                            {
                                array3[3] = Modul1.Aus[(int)EOutCfg.o10_EmitIDs].AsInt();
                            }
                            array3[3] *= 10000f;
                            array3[4] = 45f;
                            if (Modul1.Aus[(int)EOutCfg.o11] != "")
                            {
                                array3[4] = Modul1.Aus[(int)EOutCfg.o11].AsInt();
                            }
                            array3[4] *= 10000f;
                            int num6 = 0;
                            int[] array4 = new int[5];
                            int[] array2 = new int[5];
                            int[] array = new int[5];

                            if (
                              CheckPerson(array4, View.fraFather.iPNr)
                             && CheckPerson(array2, View.fraMother.iPNr))
                            {
                                float num8 = array4[1] - array2[1];
                                if (array2[1] > 0 & array4[1] > 0)
                                {
                                    if (num8 > array3[1] | num8 < 0f - array3[1])
                                    {
                                        Datfehler_Abbruch = (short)Interaction.MsgBox("Altersdifferenz Mann / Frau", title: "Plausibilitäts-Problem!", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Warning);
                                    }
                                    if (Datfehler_Abbruch == 2)
                                    {
                                        goto end_IL_0001;
                                    }
                                }

                                _ = DataModul.Event.ReadFamDates(famInArb).IntoString(Modul1.Kont1);
                                num6 = Modul1.Kont1[2].AsInt();
                                if (Modul1.Kont1[3].AsInt() < Modul1.Kont1[2].AsInt())
                                {
                                    num6 = Modul1.Kont1[3].AsInt();
                                }
                                if (Modul1.Kont1[3].AsInt() < Modul1.Kont1[5].AsInt())
                                {
                                    num6 = Modul1.Kont1[5].AsInt();
                                }
                                if (num6 == 0)
                                {
                                    num6 = Modul1.Kont1[3].AsInt();
                                }
                                if (num6 == 0)
                                {
                                    num6 = Modul1.Kont1[1].AsInt();
                                }
                                if (num6 == 0)
                                {
                                    num6 = Modul1.Kont1[0].AsInt();
                                }
                                if (num6 > 0)
                                {
                                    if (num6 - array3[2] < array4[1])
                                    {
                                        Datfehler_Abbruch = (short)Interaction.MsgBox("Vater bei Heirat zu jung !", title: "Plausibilitäts-Problem!", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Warning);
                                        if (Datfehler_Abbruch == 2)
                                        {
                                            goto end_IL_0001;
                                        }
                                    }
                                    if (num6 - array3[3] < array2[1])
                                    {
                                        Datfehler_Abbruch = (short)Interaction.MsgBox("Mutter bei Heirat zu jung !", title: "Plausibilitäts-Problem!", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Warning);
                                    }
                                    if (Datfehler_Abbruch == 2)
                                    {
                                        goto end_IL_0001;
                                    }
                                    if (num6 > array4[3] & array4[3] > 0)
                                    {
                                        Datfehler_Abbruch = (short)Interaction.MsgBox("Vater bei Heirat schon verstorben !", title: "Plausibilitäts-Problem!", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Warning);
                                    }
                                    if (Datfehler_Abbruch == 2)
                                    {
                                        goto end_IL_0001;
                                    }
                                    if (num6 > array2[3] & array2[3] > 0)
                                    {
                                        Datfehler_Abbruch = (short)Interaction.MsgBox("Mutter bei Heirat schon verstorben !", title: "Plausibilitäts-Problem!", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Warning);
                                    }
                                    if (Datfehler_Abbruch == 2)
                                    {
                                        goto end_IL_0001;
                                    }
                                }
                                DDatum = "";
                                short num12 = (short)(View.lstChildren.Items.Count - 1);
                                short num5 = 0;
                                while (num5 <= num12)
                                {
                                    if (Strings.Mid(View.lstChildren.Items[num5].AsString(), 101, 9) != "Adoptiert")
                                    {
                                        Modul1.PersInArb = (int)Math.Round(Conversion.Val(Strings.Mid(View.lstChildren.Items[num5].AsString(), 67, 8)));
                                        _ = DataModul.Event.GetPersonDates(Modul1.PersInArb, out bool VCHR).IntoString(Modul1.Kont1);
                                        if (VCHR)
                                        {
                                            goto end_IL_0001;
                                        }
                                        var num9 = 1;
                                        while (num9 <= 4)
                                        {
                                            array[num9] = Modul1.Kont1[num9].AsInt();
                                            num9++;
                                        }
                                        if (array[1] == 0)
                                        {
                                            array[1] = array[2];
                                        }
                                        if (num5 > 0)
                                        {
                                            HT = "";
                                            HT = DDatum.Date2DotDateStr2();
                                            if (HT.Left(2) == "00")
                                            {
                                                StringType.MidStmtStr(ref HT, 1, 2, "01");
                                            }
                                            if (Strings.Mid(HT, 4, 2) == "00")
                                            {
                                                StringType.MidStmtStr(ref HT, 4, 2, "01");
                                            }
                                            DDatum = HT;
                                            DDatum2 = array[1].AsString().Trim();
                                            HT = DDatum2.Date2DotDateStr2();
                                            if (HT.Left(2) == "00")
                                            {
                                                StringType.MidStmtStr(ref HT, 1, 2, "01");
                                            }
                                            if (Strings.Mid(HT, 4, 2) == "00")
                                            {
                                                StringType.MidStmtStr(ref HT, 4, 2, "01");
                                            }
                                            if (DDatum.AsInt() > 0.0 & HT.AsInt() > 0.0)
                                            {
                                                ProjectData.ClearProjectError();
                                                num3 = 2;
                                                if ((DDatum.AsDate() - HT.AsDate()).TotalDays > 0 &&
                                                    (DDatum.AsDate() - HT.AsDate()).TotalDays < 270)
                                                {
                                                    Datfehler_Abbruch = (short)Interaction.MsgBox("Kind " + num5.AsString() + " nur" + Conversion.Str(
                                                        (Conversions.ToDate(DDatum) - Conversions.ToDate(HT)).TotalDays) + " Tage vor Kind " + num5 + 1.AsString() + " geboren.", title: "Plausibilitätsproblem", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Warning);
                                                    if (Datfehler_Abbruch == 2)
                                                    {
                                                        goto end_IL_0001;
                                                    }
                                                }
                                            }
                                        }
                                        lErl = 1;
                                        if (array[3] == 0)
                                        {
                                            array[3] = array[4];
                                        }
                                        if (unchecked(array[1] > 0 && num6 > 0))
                                        {
                                            if (array[1] < num6 - (Modul1.Aus[(int)EOutCfg.o23].AsInt() + 1.0) * 10000.0)
                                            {
                                                Datfehler_Abbruch = (short)Interaction.MsgBox("Kind " + num5 + 1.AsString() + " vor der Heirat geboren!", title: "Plausibilitäts-Problem!", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Warning);
                                                if (Datfehler_Abbruch == 2)
                                                {
                                                    goto end_IL_0001;
                                                }
                                            }
                                        }
                                        if (array[1] > 0 & array4[1] > 0)
                                        {
                                            if (array[1] - 10000 < array4[1])
                                            {
                                                Datfehler_Abbruch = (short)Interaction.MsgBox("Kind " + num5 + 1.AsString() + " vor Vater geboren !", title: "Plausibilitäts-Problem!", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Warning);
                                                if (Datfehler_Abbruch == 2)
                                                {
                                                    goto end_IL_0001;
                                                }
                                            }
                                        }
                                        if (array[1] > 0 & array4[3] > 0)
                                        {
                                            if (array[1] - 10000 > array4[3])
                                            {
                                                Datfehler_Abbruch = (short)Interaction.MsgBox("Vater vor Zeugung Kind " + num5 + 1.AsString() + " verstorben !", title: "Plausibilitäts-Problem!", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Warning);
                                                if (Datfehler_Abbruch == 2)
                                                {
                                                    goto end_IL_0001;
                                                }
                                            }
                                        }
                                        if (array[1] > 0 & array2[1] > 0)
                                        {
                                            if (array[1] - 10000 < array2[1])
                                            {
                                                Datfehler_Abbruch = (short)Interaction.MsgBox("Kind " + num5 + 1.AsString() + " vor Mutter geboren !", title: "Plausibilitäts-Problem!", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Warning);
                                                if (Datfehler_Abbruch == 2)
                                                {
                                                    goto end_IL_0001;
                                                }
                                            }
                                        }
                                        if (array[1] > 0 & array2[3] > 0)
                                        {
                                            if (array[1] > array2[3])
                                            {
                                                Datfehler_Abbruch = (short)Interaction.MsgBox("Mutter vor Geburt Kind " + num5 + 1.AsString() + " verstorben !", title: "Plausibilitäts-Problem!", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Warning);
                                                if (Datfehler_Abbruch == 2)
                                                {
                                                    goto end_IL_0001;
                                                }
                                            }
                                        }
                                        if (array[1] > 0 & array2[1] > 0)
                                        {
                                            if (array[1] > array2[1] + array3[4])
                                            {
                                                Datfehler_Abbruch = (short)Interaction.MsgBox("Mutter bei Geburt Kind " + num5 + 1.AsString() + " zu alt !", title: "Plausibilitäts-Problem!", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Warning);
                                                if (Datfehler_Abbruch == 2)
                                                {
                                                    goto end_IL_0001;
                                                }
                                            }
                                        }
                                        DDatum = array[1].AsString().Trim();
                                    }
                                    num5 = (short)unchecked(num5 + 1);
                                }
                            }
                            lErl = 4;
                            FamSaveAend(Modul1.FamInArb = iFamNr);
                            goto end_IL_0001;
                        IL_10ce:
                            num4 = unchecked(num2 + 1);
                            goto IL_10d2;
                        IL_10d2:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                case 7:
                                case 74:
                                case 112:
                                case 119:
                                case 125:
                                case 131:
                                case 140:
                                case 170:
                                case 183:
                                case 191:
                                case 199:
                                case 207:
                                case 215:
                                case 223:
                                case 232:
                                case 241:
                                    goto end_IL_0001;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 5284;
                continue;
            }
            throw ProjectData.CreateProjectError(ErrUser14);
        end_IL_0001: // <========== 17
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }


    public void Mehrehezeig(IList<int> aiFam, ELinkKennz eLKennz)
    {
        View.lstMarriages.Items.Clear();
        View.frmMarriage.Visible = true;
        View.frmMarriage.Text = Modul1.IText[EUserText.tMarrTo];
        foreach (int iFamNr in aiFam)
        {
            EEventArt eArt = EEventArt.eA_500;
            string datu = "";
            while (eArt <= EEventArt.eA_507)
            {
                var dt = DataModul.Event.GetDate(eArt, iFamNr);
                if (dt == default)
                {
                    datu = "    ";
                }
                else
                {
                    datu = dt.Year.AsString();
                    break;
                }
                eArt++;
            }
            if (datu.Trim() == "")
            {
                var dt = DataModul.Event.GetDate(EEventArt.eA_601, iFamNr);
                datu = dt == default ? "    " : $"{dt.Year,4}   F";
            }
            Update_ParentFamilyDisplay(eLKennz, iFamNr, datu);
        }
    }

    /// <summary>
    /// Aktualisiert die Anzeige der Elternfamilie(n) in der Ehen-Liste (lstMarriages).
    /// Fügt für jede gefundene Familie einen formatierten Listeneintrag hinzu.
    /// Wenn keine passende Familie gefunden wird, wird ein Platzhalter für einen unbekannten Partner hinzugefügt.
    /// </summary>
    /// <param name="eLKennz">Der Elterntyp (z.B. Vater/Mutter).</param>
    /// <param name="iFamNr">Die Familiennummer.</param>
    /// <param name="datu">Das Datumsfeld (Jahr oder Jahr+F-Kennzeichnung).</param>
    /// <returns>Das ggf. angepasste Datumsfeld.</returns>
    private void Update_ParentFamilyDisplay(ELinkKennz eLKennz, int iFamNr, string datu)
    {
        // Initialisiere einen leeren Textpuffer für die Anzeige
        var liText = new string(' ', 80);
        bool found = false;

        // Durchlaufe alle Familien des gegebenen Typs für die Person
        foreach (var item in DataModul.Link.ReadAllFams(iFamNr, eLKennz))
        {
            found = true;
            Modul1.Person_ReadNames(item.iPersNr, Modul1.Person);

            // Falls das Datum auf "F" endet, formatiere es entsprechend
            if (datu.Length > 0 && datu[datu.Length - 1] == 'F')
            {
                datu = datu.Substring(0, 4) + " F";
            }

            // Erzeuge den Anzeigetext mit Namen oder Platzhalter
            string nameText = $"{(datu.Length >= 6 ? datu.Substring(datu.Length - 6) : datu),-6}";
            if (!string.IsNullOrWhiteSpace(Modul1.Person.SurName) || !string.IsNullOrWhiteSpace(Modul1.Person.Givennames))
            {
                nameText += $" {Modul1.Person.Givennames} {Modul1.Person.SurName.ToUpper().TrimEnd()}";
            }
            else
            {
                nameText += $" namenlosem Partner";
            }

            // Füge den Text in liText an die richtige Stelle ein
            liText = liText.Remove(0, Math.Min(60, liText.Length)).Insert(0, nameText.PadRight(60).Substring(0, 60)) + liText.Substring(Math.Min(60, liText.Length));
            // Füge die Familiennummer an
            string famNrText = iFamNr.AsString().PadRight(19).Substring(0, 19);
            liText = liText.Remove(60, Math.Min(19, liText.Length - 60)).Insert(60, famNrText) + (liText.Length > 79 ? liText.Substring(79) : "");

        }

        // Falls keine Familie gefunden wurde, füge einen Platzhalter für unbekannten Partner hinzu
        if (!found)
        {
            string text = " Unbekannter Partner";
            if (datu.Length > 0 && datu[datu.Length - 1] == 'F')
            {
                datu = datu.Substring(0, 4);
                text = " F Unbekannter Partner";
            }
            string nameText = $"{(datu.Length >= 6 ? datu.Substring(datu.Length - 6) : datu),-6}{text}";
            liText = liText.Remove(0, Math.Min(60, liText.Length)).Insert(0, nameText.PadRight(60).Substring(0, 60)) + liText.Substring(Math.Min(60, liText.Length));
            string famNrText = iFamNr.AsString().PadRight(19).Substring(0, 19);
            liText = liText.Remove(60, Math.Min(19, liText.Length - 60)).Insert(60, famNrText) + (liText.Length > 79 ? liText.Substring(79) : "");
        }
        // Füge den Eintrag der Liste hinzu
        View.lstMarriages.Items.Add(new ListItem(liText, iFamNr));
    }
    #endregion

    public void Familie_FormClosed(object sender, FormClosedEventArgs e)
    {
        FileSystem.FileClose(6);
        if (Modul1.Typ != DriveType.CDRom)
        {
            if (Modul1.cMandDrive.DriveType != DriveType.CDRom)
            {
                FileSystem.FileOpen(6, Modul1.InitDir + "Windowstate", OpenMode.Output);
            }
            FileSystem.PrintLine(6, View.WindowState);
        }
        FileSystem.FileClose(6);
    }
    public void Familie_Load(object sender, EventArgs e)
    {
        iKindSich = 0;

        //     Modul1.AddContextMenu(View.RichTextBox1);
        if (Modul1.FontSize > 0f)
        {
            View.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.lstChildren.Font = new Font("Courier New", Modul1.FontSize, FontStyle.Regular);
            View.lstMarriages.Font = new Font("Courier New", Modul1.FontSize, FontStyle.Regular);
            View.lblHdrList.Font = new Font("Courier New", Modul1.FontSize, FontStyle.Regular);
        }
        View.BackColor = Menue.Default.BackColor;

        Modul1.Persistence.ReadEnumInit<FormWindowState>("Windowstate", out var WiS);
        View.WindowState = WiS;

        View.lblCommonFamName.Text = Modul1.IText[EUserText.tCommonFamName];
        View.ToolTip1.SetToolTip(View.lblCommonFamName, "Nur wenn er vom Namen des Vaters abweicht; 'Neues Namensrecht'");
        View.ToolTip1.SetToolTip(View.btnChildren, "Kind zufügen oder entfernen");
        View.lblHdrBorn_Name.Text = Modul1.IText[EUserText.tBorn_Name];
        View.lblHdrList.Text = "   " + Modul1.IText[EUserText.tDeath_PNo_FNo_ANo];
        View.lblMandant.Text = Modul1.Verz;
        View.lblFamilynotes.Text = Modul1.IText[EUserText.t287];

        var ints = Modul1.Persistence.ReadIntsProg("maspos.dat", 2);
        View.Left = ints[0];
        View.Top = ints[1];
    }
    public void btnNext_Click(object sender, EventArgs e)
    {
        Kindneu = 0;
        Modul1.FamInArb = iFamNr;
        FamdatPrüfen(Modul1.FamInArb);
        if (Datfehler_Abbruch != 2)
        {
            Modul1.FamInArb = iFamNr + 1;
            short Rich = (short)(Modul1.FamInArb > iFamNr ? 1 : 0);
            Fameinlesen(Modul1.FamInArb, out Rich);
        }
    }
    public void btnPrevious_Click(object sender, EventArgs e)
    {
        Kindneu = 0;
        Modul1.FamInArb = iFamNr;
        FamdatPrüfen(Modul1.FamInArb);
        if (Datfehler_Abbruch != 2)
        {
            FamSaveAend(Modul1.FamInArb = iFamNr);
            Modul1.FamInArb = iFamNr - 1;
            short Rich = (short)(Modul1.FamInArb > iFamNr ? 1 : 0);
            Fameinlesen(Modul1.FamInArb, out Rich);
        }
    }
    public void btnSearchRegister_Click(object sender, EventArgs e)
    {
        Regsuch regsuch = MainProject.Forms.Regsuch;

        Kindneu = 0;
        checked
        {
            Modul1.FamInArb = iFamNr;
            FamdatPrüfen(Modul1.FamInArb);
            if (Datfehler_Abbruch == 2)
            {
                return;
            }
            _ = regsuch.ShowDialog((byte)1, (byte)1, 0, 0);
            Modul1.Suchschalt = regsuch.Suchschalt;
            Modul1.Suchfam = regsuch.Suchfam;
            if (Modul1.Suchschalt == 1)
            {
                if (Modul1.Suchfam == 0)
                {
                    Modul1.Suchfam = iFamNr;
                }
                Modul1.FamInArb = Modul1.Suchfam;
                short Rich;
                Fameinlesen(Modul1.FamInArb, out Rich);
            }
        }
    }

    public void btnSearchNumber_Click(object sender, EventArgs e)
    {
        Modul1.FamInArb = iFamNr;
        FamdatPrüfen(Modul1.FamInArb);
        if (Datfehler_Abbruch == 2)
        {
            return;
        }
        Kindneu = 0;
        string prompt = "Nummer der gesuchten Familie\rLeer,0 oder Abbrechen wechselt in die Suche nach Namen";
        string inputStr = Interaction.InputBox(prompt, "Familiensuche");
        Modul1.FamInArb = inputStr.AsInt();
        if (Modul1.FamInArb == 0)
        {
            Modul1.Schalt = 1;
            Modul1.Suchschalt = 1;
            Modul1.Suchfam = 0;
            Modul1.SuchPer = 0;
            MainProject.Forms.Namensuch.ShowDialog(title: "Familiensuche", personNr: 0, iFamNr: 0);
            if (Modul1.Suchfam == 0)
            {
                Modul1.Suchfam = iFamNr;
            }
            Modul1.FamInArb = Modul1.Suchfam;
        }
        short Rich;
        Fameinlesen(Modul1.FamInArb, out Rich);

    }
    public void lblSources_Click(object sender, EventArgs e)
    {
        Modul1.FamInArb = iFamNr;
        var dB_FamilyTable = DataModul.DB_FamilyTable;
        dB_FamilyTable.Seek("=", Modul1.FamInArb);
        var Qkenn = 2;
        Modul1.Ubg = 0;
        var dB_TTable = DataModul.DB_SourceLinkTable;
        dB_TTable.Index = "Tab";
        dB_TTable.Seek("=", Qkenn, Modul1.FamInArb);
        ProjectData.ClearProjectError();
        Quellen frmQuellen = MainProject.Forms.Quellen;
        while (!dB_TTable.EOF
            && !dB_TTable.NoMatch
            && dB_TTable.Fields[SourceLinkFields._1].AsInt() == Qkenn
            && dB_TTable.Fields[SourceLinkFields._2].AsInt() <= Modul1.FamInArb)
        {
            frmQuellen.Button3.Enabled = true;
            var dB_QuTable = DataModul.DB_QuTable;
            dB_QuTable.Index = "Nr";
            dB_QuTable.Seek("=", dB_TTable.Fields[SourceLinkFields._3]);
            if (!dB_QuTable.NoMatch)
            {
                _ = frmQuellen.ComboBox1.Items.Add(Strings.Left(((dB_QuTable.Fields["2"].Value) + (new string(' ', 240))).AsString(), 240) + dB_QuTable.Fields["1"].AsString());
                frmQuellen.ComboBox1.Text = frmQuellen.ComboBox1.Items[0].AsString();
            }
            dB_TTable.MoveNext();
        }
        Modul1.FamInArb = iFamNr;
        dB_FamilyTable.Seek("=", Modul1.FamInArb.AsString());
        if (null != dB_FamilyTable.Fields[FamilyFields.Bem3].Value)
        {
            if (Strings.Trim(dB_FamilyTable.Fields[FamilyFields.Bem3].AsString()) != "")
            {
                frmQuellen.RTB.Text = dB_FamilyTable.Fields[FamilyFields.Bem3].AsString();
            }
        }
        frmQuellen.Show();
        _ = frmQuellen.RTB.Focus();
        frmQuellen.StartPosition = FormStartPosition.CenterScreen;
        frmQuellen.Visible = false;
        _ = frmQuellen.ShowDialog(Modul1.Ubg);
    }
    public void btnResidence_Click(object sender, EventArgs e)
    {
        FamSaveAend(Modul1.FamInArb = iFamNr);
        Modul1.Ubg = 602;
        if (View.ComboBox1.Text.Trim() == "" & View.ComboBox1.BackColor != Color.FromArgb(0xC0FFFF))
        {
            View.frmFamilyresidence.Visible = true;
            View.btnNew.PerformClick();
        }
        else
        {
            View.frmFamilyresidence.Text = "Wohnort der Familie";
            View.frmFamilyresidence.Visible = true;
        }
    }
    public void btnConfirmation_Click(object sender, EventArgs e)
    {
        FamSaveAend(Modul1.FamInArb = iFamNr);
        Modul1.Ubg = 603;
        if (View.ComboBox2.Text.Trim() == "" & View.ComboBox2.Tag.AsInt() == 0.0 & View.ComboBox2.BackColor != Color.FromArgb(0xC0FFFF))
        {
            View.frmFamilyresidence.Visible = true;
            View.btnNew.PerformClick();
        }
        else
        {
            View.frmFamilyresidence.Text = "sonstiges Datum der Familie";
            View.frmFamilyresidence.Visible = true;
        }
    }
    public void btnMainmenue_Click(object sender, EventArgs e)
    {
        checked
        {
            Modul1.FamInArb = iFamNr;
            FamdatPrüfen(Modul1.FamInArb);
            if (Datfehler_Abbruch == 2)
            {
                return;
            }
            FamSaveAend(Modul1.FamInArb = iFamNr);
            if (View.btnMainmenue.Text == Modul1.IText[EUserText.tNMBack])
            {
                View.Close();
                return;
            }
            if (Modul1.Typ != DriveType.CDRom)
            {
                Modul1.Letzte = new() { iPerson = Modul1.Letzte.iPerson, iFamily = iFamNr };
                Modul1.Persistence.PutIntMand("Letzter.dat", Modul1.Letzte.iFamily, 2L);
            }
            Menue.Default.Show();
            View.Close();
        }
    }
    public void ListBox1_DoubleClick(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_0193
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
                short Rich;
                switch (try0001_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0008;
                    case 629:
                        {
                            num2 = num;
                            switch (num3 <= -2 ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_01e1;
                                default:
                                    goto end_IL_0001;
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
                            goto IL_01e4;
                        }
                    end_IL_0001:
                        break;
                    IL_0008:
                        num = 2;
                        Datfehler_Abbruch = 0;
                        Modul1.FamInArb = iFamNr;
                        FamdatPrüfen(Modul1.FamInArb);
                        if (Datfehler_Abbruch == 2)
                        {
                            goto end_IL_0001_2;
                        }
                        if (Modul1.FamInArb > 0)
                        {
                            Modul1.Aendf = true;
                        }
                        FamSaveAend(Modul1.FamInArb = iFamNr);
                        if (View.lstChildren.Text.Trim() == "")
                        {
                            goto end_IL_0001_2;
                        }
                        if (Strings.Mid(View.lstChildren.Text, 75, 8) != "        ")
                        {
                            Modul1.FamInArb = Strings.Mid(View.lstChildren.Text, 75, 8).AsInt();
                            Clear();
                            Rich = 3;
                            Fameinlesen(Modul1.FamInArb, out Rich);
                        }
                        else
                        {
                            //    Modul1.Ubg = Strings.Mid(lstUsageList.Text, 67, 8).AsInt();
                            Modul1.Ubg = View.lstChildren.SelectedItem.ItemData<int>();
                            if (Modul1.Ubg <= 0)
                            {
                                goto end_IL_0001_2;
                            }
                            Modul1.PersInArb = Modul1.Ubg;
                            View.Hide();
                            Personen.Default.Show(Modul1.PersInArb, EUserText.t158);
                        }
                        goto end_IL_0001_2;
                    IL_01e1:
                        num4 = num2 + 1;
                        goto IL_01e4;
                    IL_01e4:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 6:
                            case 17:
                            case 25:
                            case 26:
                            case 27:
                            case 28:
                            case 33:
                                goto end_IL_0001_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 629;
                continue;
            }
            throw ProjectData.CreateProjectError(ErrUser14);
        end_IL_0001_2: // <========== 5
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    private bool CheckPerson(int[] array4, int persInArb)
    {
        var asDts = DataModul.Event.GetPersonDates(persInArb, out bool VCHR);
        if (VCHR)
        {
            return false;
        }
        int i = 1;
        while (i <= 4)
        {
            array4[i] = asDts[i].AsInt();
            i++;
        }
        if (array4[1] == 0)
        {
            array4[1] = array4[2];
        }
        if (array4[3] == 0)
        {
            array4[3] = array4[4];
        }
        return true;
    }
    public void btnEnableCheck_Click(object sender, EventArgs e)
    {
        checked
        {
            Modul1.FamInArb = iFamNr;
            if (Modul1.Typ == DriveType.CDRom)
            {
                _ = Interaction.MsgBox(Modul1.Message_sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
            }
            else if (View.btnEnableCheck.Tag.AsInt() == 0)
            {
                View.btnEnableCheck.Text = Modul1.IText[EUserText.t151];
                View.btnEnableCheck.Tag = 1;
                View.btnEnableCheck.BackColor = Color.FromArgb(0xFF);

                DataModul_Families_UpdateCheck(Modul1.FamInArb, "N");
            }
            else
            {
                View.btnEnableCheck.Text = Modul1.IText[EUserText.t152];
                View.btnEnableCheck.Tag = 0;
                View.btnEnableCheck.BackColor = View.btnPrevious.BackColor;
                DataModul_Families_UpdateCheck(Modul1.FamInArb, " ");
            }
        }
    }

    private void DataModul_Families_UpdateCheck(int famInArb, string sValue)
    {
        DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
        DataModul.DB_FamilyTable.Seek("=", famInArb);
        if (!DataModul.DB_FamilyTable.NoMatch)
        {
            DataModul.DB_FamilyTable.Edit();
            DataModul.DB_FamilyTable.Fields[FamilyFields.Prüfen].Value = sValue;
            DataModul.DB_FamilyTable.Update();
        }
    }

    private void fraFather_LabelNrMarr_Click(object sender, EventArgs e)
        => Parent_MarriageUpdate(View.fraFather.iPNr, ELinkKennz.lkFather);
    private void fraMother_LabelNrMarr_Click(object sender, EventArgs e)
        => Parent_MarriageUpdate(View.fraMother.iPNr, ELinkKennz.lkMother);

    private void Parent_MarriageUpdate(int iPNr, ELinkKennz eLinkKnz) => Mehrehezeig(
            Modul1.Link_Famsuch(iPNr, eLinkKnz),
            eLinkKnz == ELinkKennz.lkFather ? ELinkKennz.lkMother : ELinkKennz.lkFather);


    public void btnMarrClose_Click(object sender, EventArgs e)
    {
        View.frmMarriage.Visible = false;
    }

    public void lstMarriages_DoubleClick(object sender, EventArgs e)
    {
        Kindneu = 0;
        checked
        {
            Modul1.FamInArb = iFamNr;
            FamdatPrüfen(Modul1.FamInArb);
            if (Datfehler_Abbruch != 2 && View.lstMarriages.Text.Trim() != "")
            {
                Modul1.FamInArb = View.lstMarriages.SelectedItem.ItemData<int>();
                if (Modul1.FamInArb != 0)
                {
                    View.frmMarriage.Visible = false;
                    short Rich;
                    Fameinlesen(Modul1.FamInArb, out Rich);
                }
            }
        }
    }
    private void fraFather_Label_Click(object sender, EventArgs e)
    {
        FamSaveAend(Modul1.FamInArb = iFamNr);
        ProjectData.ClearProjectError();
        Modul1.PersInArb = View.fraFather.iPNr;
        if (Modul1.PersInArb != 0)
        {
            View.Close();
            Personen.Default.Show(Modul1.PersInArb, EUserText.t158);
        }
        else
        {
            View.chbAppAsAdopted.Visible = false;
            View.btnAppDelete.Enabled = false;
            View.btnAppDelete.Text = "";
            View.frmAppendPerson.Visible = true;
            View.lblAppLabel44.Text = $"{Modul1.IText[EUserText.tFamilyNo]} {iFamNr} {Modul1.IText[EUserText.t136]}";
            View.lblAppLabel44.Tag = 1;
        }
    }
    private void fraMother_Label_Click(object sender, EventArgs e)
    {
        Modul1.PersInArb = 0;
        FamSaveAend(Modul1.FamInArb = iFamNr);
        Modul1.PersInArb = View.fraMother.iPNr;
        if (Modul1.PersInArb != 0)
        {
            View.Close();
            MainProject.MyForms forms = MainProject.Forms;
            Form Formtocheck = forms.Personen;
            var num5 = Modul1.IsFormloaded(Formtocheck);
            forms.Personen = (Personen)Formtocheck;
            Personen.Default.Close();
            Ad = false;
            Personen.Default.Show(Modul1.PersInArb, EUserText.t158);
        }
        else
        {
            View.chbAppAsAdopted.Visible = false;
            View.btnAppDelete.Enabled = false;
            View.btnAppDelete.Text = "";
            View.frmAppendPerson.Visible = true;
            View.lblAppLabel44.Text = $"{Modul1.IText[EUserText.tFamilyNo]} {iFamNr} {Modul1.IText[EUserText.t137]}";
            View.lblAppLabel44.Tag = 2;
        }
    }

    [RelayCommand]
    private void AddProklamation()
    {
        FamSaveAend(Modul1.FamInArb);
        _ = MainProject.Forms.Ereignis.ShowEventDialog(EEventArt.eA_500);
    }
    [RelayCommand]
    private void AddEngagement()
    {
        FamSaveAend(Modul1.FamInArb = iFamNr);
        _ = MainProject.Forms.Ereignis.ShowEventDialog(EEventArt.eA_501);
    }
    [RelayCommand]
    private void AddMarriage()
    {
        FamSaveAend(Modul1.FamInArb = iFamNr);
        _ = MainProject.Forms.Ereignis.ShowEventDialog(EEventArt.eA_Marriage);
    }
    [RelayCommand]
    private void AddReligMarr()
    {
        FamSaveAend(Modul1.FamInArb);
        _ = MainProject.Forms.Ereignis.ShowEventDialog(EEventArt.eA_MarrReligious);
    }
    [RelayCommand]
    private void AddDivorce()
    {
        FamSaveAend(Modul1.FamInArb = iFamNr);
        _ = MainProject.Forms.Ereignis.ShowEventDialog(EEventArt.eA_504);
    }
    [RelayCommand]
    private void AddPartnership()
    {
        FamSaveAend(Modul1.FamInArb);
        _ = MainProject.Forms.Ereignis.ShowEventDialog(EEventArt.eA_505);
    }
    [RelayCommand]
    private void AddDimissorale()
    {
        FamSaveAend(Modul1.FamInArb = iFamNr);
        _ = MainProject.Forms.Ereignis.ShowEventDialog(EEventArt.eA_507);
    }
    [RelayCommand]
    private void AddEstimatedMarr()
    {
        FamSaveAend(Modul1.FamInArb);
        _ = MainProject.Forms.Ereignis.ShowEventDialog(EEventArt.eA_601);
    }

    public void Famdatles(int FamInArb)
    {

        //   var dB_TTable = DataModul.DB_SourceLinkTable;

        Modul1.FamDatLes_int(FamInArb, DisableIllg, SetEventText);
        CheckDisableLabels();

        void DisableIllg()
        {
            View.cbxIllegitRel.Enabled = false;
            View.cbxIllegitRel.CheckState = CheckState.Unchecked;
        }
    }


    public void FamSaveAend(int famInArb)
    {
        if (Modul1.Typ == DriveType.CDRom)
        {
            return;
        }
        checked
        {
            var dB_FamilyTable = DataModul.DB_FamilyTable;
            dB_FamilyTable.Index = nameof(FamilyIndex.Fam);
            dB_FamilyTable.Seek("=", famInArb);

            string Fam_sBem1 = dB_FamilyTable.Fields[FamilyFields.Bem1].AsString();
            int Fam_iGgv = dB_FamilyTable.Fields[FamilyFields.ggv].AsInt(-1);
            int aByte = 0;

            if (Fam_sBem1 != View.RichTextBox1.Text)
            {
                Modul1.Aendf = true;
            }
            if (Modul1.FAendmerk)
            {
                Modul1.Aendf = true;
                Modul1.FAendmerk = false;
            }
            if (View.CheckBox2.Checked)
            {
                aByte = 1;
            }
            if (View.CheckBox2.CheckState == CheckState.Unchecked)
            {
                aByte = 0;
            }

            if (0 == Fam_iGgv)
            {
                Modul1.Aendf = true;
            }
            else if (Fam_iGgv != aByte)
            {
                Modul1.Aendf = true;
            }
            if (!Modul1.Aendf)
            {
                return;
            }
            int iName = View.TextBox4.Text.Trim() != "" ? Modul1.TextSpeich(View.TextBox4.Text.Trim(), "", ETextKennz.tkName, Fam_iGgv, Modul1.LfNR) : 0;
            int iPrae = View.edtNameprefix.Text.Trim() != "" ? Modul1.TextSpeich(View.edtNameprefix.Text.Trim(), "", ETextKennz.A_, Fam_iGgv, Modul1.LfNR) : 0;
            var iSuf = View.edtNameSuffix.Text.Trim() != "" ? Modul1.TextSpeich(View.edtNameSuffix.Text.Trim(), "", ETextKennz.B_, Fam_iGgv, Modul1.LfNR) : 0;

            if (View.RichTextBox1.Text.TrimEnd() == "")
            {
                View.RichTextBox1.Text = " ";
            }
            int M1_Iter = 1;
            while (View.RichTextBox1.Text.Length != 0 && Strings.Asc(View.RichTextBox1.Text.Right(1)) < 14)
            {
                View.RichTextBox1.Text = View.RichTextBox1.Text.Left(View.RichTextBox1.Text.Length - 1);
                M1_Iter++;
                int i = M1_Iter;
                int num = 20;
                if (i > num)
                {
                    break;
                }
            }
            if (View.RichTextBox1.Text.Length == 0)
            {
                View.RichTextBox1.Text = " ";
            }

            dB_FamilyTable.Edit();

            dB_FamilyTable.Fields[FamilyFields.Name].Value = iName;

            dB_FamilyTable.Fields[FamilyFields.Prae].Value = iPrae;

            dB_FamilyTable.Fields[FamilyFields.Suf].Value = iSuf;

            dB_FamilyTable.Fields[FamilyFields.Bem1].Value = View.RichTextBox1.Text;
            dB_FamilyTable.Fields[FamilyFields.Aeb].Value = View.cbxIllegitRel.Checked ? -1 : 0;
            if (View.CheckBox2.Checked)
            {
            }
            else
            {
            }
            if (Modul1.Trans > 0)
            {
                dB_FamilyTable.Fields[FamilyFields.EditDat].Value = DateTime.Today.ToString("yyyyMMdd");
            }
            Modul1.Trans = 0;
            dB_FamilyTable.Update();
            Modul1.Aendf = false;
        }
    }

    private void fraFather_Grandparent_Click(object sender, EventArgs e)
    {
        if (View.fraFather.iPNr == 0)
        {
            return;
        }
        FamSaveAend(Modul1.FamInArb);
        Modul1.FamInArb = View.fraFather.iFamNr;
        if (Modul1.FamInArb > 0)
        {
            short Rich;
            Fameinlesen(Modul1.FamInArb, out Rich);
        }
        else
        {
            if (Interaction.MsgBox("Wirklich eine neue Familie anlegen in der der Vater dieser Familie Kind ist?", title: "", mb: MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }
            Kindneu = View.fraFather.iPNr;
            iKindSich = View.fraFather.iPNr;
            View.btnReenter.PerformClick();
            if (Datfehler_Abbruch == 2 || Modul1.FamInArb == 0)
            {
                return;
            }
            Modul1.eLKennz = ELinkKennz.lkChild;
            Modul1.PersInArb = Kindneu;
            Modul1.eLKennz = ELinkKennz.lkChild;
            _ = DataModul.Link.DeleteQ(Modul1.FamInArb, Modul1.PersInArb, ELinkKennz.lkChild, DialogResult.Yes, (p, f) =>
            {
                string prompt = $"Kind Pers.-Nr. {p} aus Familie Nr. {f} entfernen?";
                return Interaction.MsgBox(prompt, icon: MessageBoxIcon.Exclamation, mb: MessageBoxButtons.YesNo, title: "");
            });
            if (!DataModul.Link.AppendE(Kindneu, Modul1.FamInArb, ELinkKennz.lkChild))
            {
                var mb = Interaction.MsgBox($"Person {Kindneu} ist bereits Kind in Familie {Modul1.FamInArb}");
                View.fraFather.iPNr = 0;
                return;
            }
            short Rich;
            Fameinlesen(Modul1.FamInArb, out Rich);
        }
    }

    private void fraMother_Grandparent_Click(object sender, EventArgs e)
    {
        if (View.fraMother.iPNr == 0)
        {
            return;
        }
        FamSaveAend(Modul1.FamInArb);
        Modul1.FamInArb = View.fraFather.iFamNr;
        if (Modul1.FamInArb > 0)
        {
            short Rich;
            Fameinlesen(Modul1.FamInArb, out Rich);
        }
        else
        {
            if (Interaction.MsgBox("Wirklich eine neue Familie anlegen in der die Mutter dieser Familie Kind ist?", title: "", mb: MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }
            Kindneu = View.fraMother.iPNr;
            iKindSich = View.fraMother.iPNr;
            View.btnReenter.PerformClick();
            if (Datfehler_Abbruch == 2 || Modul1.FamInArb == 0)
            {
                return;
            }
            Modul1.eLKennz = ELinkKennz.lkChild;
            Modul1.PersInArb = Kindneu;
            Modul1.eLKennz = ELinkKennz.lkChild;
            _ = DataModul.Link.DeleteQ(Modul1.FamInArb, Modul1.PersInArb, ELinkKennz.lkChild, DialogResult.Yes, (p, f) =>
            {
                string prompt = $"Kind Pers.-Nr. {p} aus Familie Nr. {f} entfernen?";
                return Interaction.MsgBox(prompt, icon: MessageBoxIcon.Exclamation, mb: MessageBoxButtons.YesNo, title: "");
            });
            if (!DataModul.Link.AppendE(Kindneu, Modul1.FamInArb, ELinkKennz.lkChild))
            {
                var mb = Interaction.MsgBox($"Person {Kindneu} ist bereits Kind in Familie {Modul1.FamInArb}");
                View.fraMother.iPNr = 0;
                return;
            }
            short Rich;
            Fameinlesen(Modul1.FamInArb, out Rich);
        }
    }

    public void btnReenter_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_0497
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
                short Rich;
                switch (try0001_dispatch)
                {
                    default:
                        num = 1;
                        if (Modul1.Typ == DriveType.CDRom)
                        {
                            _ = Interaction.MsgBox(Modul1.Message_sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
                            goto end_IL_0001_2;
                        }
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        int famInArb = iFamNr;

                        if (famInArb > 0)
                        {
                            FamdatPrüfen(famInArb);
                            if (Datfehler_Abbruch == 2)
                            {
                                goto end_IL_0001_2;
                            }
                        }
                        if (DataModul.DB_FamilyTable.RecordCount > 0)
                        {
                            famInArb = 0;
                            DataModul.DB_WDTable.MoveFirst();
                            if (DataModul.DB_WDTable.Fields[WDFields.Nr].AsInt() == 1)
                            {
                                if (DataModul.LeerTab_GetEmptyFam(out famInArb))
                                {
                                    DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
                                    DataModul.DB_FamilyTable.Seek("=", famInArb);
                                    if (!DataModul.DB_FamilyTable.NoMatch)
                                    {
                                        famInArb = 0;
                                    }
                                }
                            }
                            if (famInArb == 0)
                            {
                                DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
                                DataModul.DB_FamilyTable.MoveLast();
                                famInArb = Conversions.ToInteger(((DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt()) + (1)));
                            }
                        }
                        else
                        {
                            famInArb = 1;
                        }
                        if (famInArb <= 0)
                        {
                        }
                        else
                        {

                            View.fraFather.iFamNr = famInArb;
                            DataModul.DB_FamilyTable.AddNew();
                            DataModul.DB_FamilyTable.Fields[FamilyFields.AnlDatum].Value = DateTime.Today.Year.ToString() + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString();
                            DataModul.DB_FamilyTable.Fields[FamilyFields.EditDat].Value = DateTime.Today.Year.ToString() + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString();
                            DataModul.DB_FamilyTable.Fields[FamilyFields.Prüfen].Value = "1    ";
                            DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].Value = " ";
                            DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].Value = famInArb;
                            DataModul.DB_FamilyTable.Fields[FamilyFields.Name].Value = 0;
                            DataModul.DB_FamilyTable.Fields[FamilyFields.Aeb].Value = 0;
                            DataModul.DB_FamilyTable.Fields[FamilyFields.Fuid].Value = Guid.NewGuid();
                            DataModul.DB_FamilyTable.Update();
                            Modul1.Ubg = famInArb;
                            Clear();
                            Rich = 3;
                            Fameinlesen(famInArb, out Rich);

                        }
                        Modul1.FamInArb = famInArb;
                        goto end_IL_0001_2;
                    case 1505:
                        {
                            num2 = num;
                            switch (num3 <= -2 ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_04e5;
                                default:
                                    goto end_IL_0001;
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
                            goto IL_04e8;
                        }
                    end_IL_0001:
                        break;
                    IL_04e5:
                        num4 = num2 + 1;
                        goto IL_04e8;
                    IL_04e8:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 3:
                            case 10:
                            case 53:
                            case 54:
                            case 59:
                                goto end_IL_0001_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 1505;
                continue;
            }
            throw ProjectData.CreateProjectError(ErrUser14);
        end_IL_0001_2: // <========== 4
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void btnSearchName_Click(object sender, EventArgs e)
    {
        Namensuch namensuch = MainProject.Forms.Namensuch;

        Kindneu = 0;
        Modul1.FamInArb = iFamNr;
        FamdatPrüfen(Modul1.FamInArb);
        if (Datfehler_Abbruch == 2)
        {
            return;
        }



        _ = namensuch.ShowDialog(0, "", 0);
        var FamPerschalt = namensuch.FamPerSchalt;
        Modul1.Suchfam = namensuch.SuchFam;
        Modul1.SuchPer = namensuch.SuchPer;

        if (FamPerschalt == 1)
        {
            View.Close();
            Ad = true;
            Personen.Default.Show(Modul1.PersInArb, EUserText.t158);
            return;
        }
        if (Modul1.Suchfam == 0)
        {
            Modul1.Suchfam = iFamNr;
        }
        Modul1.FamInArb = Modul1.Suchfam;
        short Rich;
        Fameinlesen(Modul1.FamInArb, out Rich);

    }

    public void Command1_Click(object sender, EventArgs e)
    {
        View.frmFamilyresidence.Visible = false;
    }

    public void btnNew_Click(object sender, EventArgs e)
    {
        checked
        {
            Modul1.FamInArb = iFamNr;
            View.frmFamilyresidence.Visible = false;
            int M1_Iter = 1;
#warning ToDo: Parameter 
            EEventArt eEventArt = Modul1.Ubg.AsEnum<EEventArt>();
            while (M1_Iter <= 70)
            {
                if (DataModul.Event.Exists(eEventArt, Modul1.FamInArb, M1_Iter))
                {
                    Modul1.LfNR = (short)M1_Iter;
                    break;
                }
                M1_Iter++;
            }
            Modul1.ErSchalt = 2;
            MainProject.Forms.Ereignis.Show();

            if (eEventArt == EEventArt.eA_603)
            {
                _ = MainProject.Forms.Ereignis.TextBox1.Focus();
            }
            if (eEventArt == EEventArt.eA_602)
            {
                if (Modul1.Aus[(int)EOutCfg.o26] == "")
                {
                    Modul1.Aus[(int)EOutCfg.o26] = true.AsString();
                }
                _ = Modul1.Aus[(int)EOutCfg.o26].AsBool()
                    ? MainProject.Forms.Ereignis.TextBox6.Focus()
                    : MainProject.Forms.Ereignis.TextBox2.Focus();
            }
            MainProject.Forms.Ereignis.Visible = false;
            _ = MainProject.Forms.Ereignis.ShowEventDialog(eEventArt);
        }
    }

    public void btnEdit_Click(object sender, EventArgs e)
    {
        checked
        {
            EEventArt eEventArt = default;
            if (eEventArt == EEventArt.eA_602)
            {
                Modul1.Nr = iFamNr;
                if (View.ComboBox1.Text != "")
                {
                    Modul1.LfNR = (short)View.ComboBox1.Tag.AsInt();
                }
            }
            else if (eEventArt == EEventArt.eA_603)
            {
                Modul1.Nr = iFamNr;
                if (View.ComboBox2.Text != "")
                {
                    Modul1.LfNR = (short)View.ComboBox2.Tag.AsInt();
                }
                View.fraFather.PNr_Visible = true;
                View.lblMandant.Visible = true;
            }
            View.frmFamilyresidence.Visible = false;
            _ = MainProject.Forms.Ereignis.ShowEventDialog(eEventArt);
        }
    }

    public void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        View.ComboBox1.Tag = View.ComboBox1.Text.Right(10);
        _ = View.lstChildren.Focus();
    }

    public void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
    {
        //Discarded unreachable code: IL_07b7, IL_08a4, IL_08c9
        short num;
        checked
        {
            num = (short)Strings.Asc(e.KeyChar);
            if (num == 13)
            {
                if (num == 13)
                {
                    num = 0;
                }
                if (Modul1.Trans == 0)
                {
                    Modul1.Trans = 1;
                }
                Modul1.FamInArb = iFamNr;
                string name = ((TextBox)sender).Name;
                if (name == nameof(View.fraFather.edtParentPNr))
                {
                    fraFather_PNr_KeyPress(sender, e);
                }
                else if (name == nameof(View.fraMother.edtParentPNr))
                {
                    fraMother_PNr_KeyPress(sender, e);
                }
                else if (name == View.TextBox3.Name)
                {
                    Modul1.eLKennz = ELinkKennz.lkChild;
                    if (View.TextBox3.Tag.AsInt() != 0)
                    {
                        Modul1.PersInArb = View.TextBox3.Tag.AsInt();
                        if (!DataModul.Person.Exists(Modul1.PersInArb))
                        {
                            _ = Interaction.MsgBox("Person mit Nummer" + Modul1.PersInArb.AsString() + " ist nicht vorhanden");
                            View.TextBox3.Text = "";
                        }
                        else
                        {
                            Modul1.eLKennz = DataModul.Person.GetSex(Modul1.PersInArb) == "F" ? ELinkKennz.lkMother : ELinkKennz.lkFather;
                            if (!DataModul.Link.Exist(Modul1.FamInArb, Modul1.PersInArb, Modul1.eLKennz))
                            {

                                if (!DataModul.Link.DeleteQ(Modul1.FamInArb, Modul1.PersInArb, ELinkKennz.lkAdoptedChild, DialogResult.Yes, (p, f) =>
                                {
                                    string prompt = $"Adoptivkind Pers.-Nr. {p} aus Familie Nr. {f} entfernen?";
                                    return Interaction.MsgBox(prompt, icon: MessageBoxIcon.Exclamation, mb: MessageBoxButtons.YesNo, title: "");
                                }))
                                {
                                    if (!DataModul.Link.DeleteQ(Modul1.FamInArb, Modul1.PersInArb, ELinkKennz.lkChild, DialogResult.Yes, (p, f) =>
                                    {
                                        string prompt = $"Kind Pers.-Nr. {p} aus Familie Nr. {f} entfernen?";
                                        return Interaction.MsgBox(prompt, icon: MessageBoxIcon.Exclamation, mb: MessageBoxButtons.YesNo, title: "");
                                    }))
                                    {
                                        if (DataModul.Link.GetPersonFam(Modul1.PersInArb, ELinkKennz.lkChild, out int iFamNr))
                                        {
                                            string prompt = $"Person {Modul1.PersInArb} ist bereits Kind in Familie {iFamNr}";
                                            prompt += "\n\nSoll sie als Adoptivkind eingefügt werden?";
                                            var ftValue = (float)Interaction.MsgBox(prompt, title: "", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Question);
                                            if (ftValue == 2f)
                                            {
                                                View.TextBox3.Text = "";
                                                e.KeyChar = Strings.Chr(num);
                                                if (num == 0)
                                                {
                                                    e.Handled = true;
                                                }
                                                return;
                                            }
                                            Modul1.eLKennz = ELinkKennz.lkAdoptedChild;
                                        }

                                        new CLinkData(Modul1.eLKennz, Modul1.FamInArb, View.TextBox3.Tag.AsInt()).AppendDB();
                                    }
                                    View.TextBox3.Text = "";
                                }
                                Modul1.Ubg = checked(iFamNr);
                                short Rich;
                                Fameinlesen(Modul1.FamInArb, out Rich);
                            }
                            else
                            {
                                _ = Interaction.MsgBox("Person ist bereits Elternteil in dieser Familie");
                                View.TextBox3.Text = "";
                            }
                        }
                    }
                }
                else
                {
                    Modul1.Ubg = checked(iFamNr);
                    short Rich;
                    Fameinlesen(Modul1.FamInArb, out Rich);
                }
            }
        }
        e.KeyChar = Strings.Chr(num);
        if (num == 0)
        {
            e.Handled = true;
        }
    }

    private void fraMother_PNr_KeyPress(object sender, KeyPressEventArgs e)
    {
        if ((short)Strings.Asc(e.KeyChar) == 13)
        {
            if (View.fraMother.iPNr > 0)
            {
                AppendFamilyParent(Modul1.FamInArb, View.fraMother.iPNr, ELinkKennz.lkMother);
            }
            else
            {
                DataModul.Link.DeleteFam(Modul1.FamInArb, ELinkKennz.lkMother);
            }
        }
    }

    private void fraFather_PNr_KeyPress(object sender, KeyPressEventArgs e)
    {
        if ((short)Strings.Asc(e.KeyChar) == 13)
        {
            if (View.fraFather.iPNr > 0)
            {
                AppendFamilyParent(Modul1.FamInArb, View.fraFather.iPNr, ELinkKennz.lkFather);
            }
            else
            {
                DataModul.Link.DeleteFam(Modul1.FamInArb, ELinkKennz.lkFather);
            }
        }
    }

    public void AppendFamilyParent(int famInArb, int persInArb, ELinkKennz kennz)
    {
        switch (DataModul.Link.AppendFamilyParent(famInArb, persInArb, kennz, DataModul.Person.CheckID, View.CheckBox2.Checked))
        {
            case -1:
                _ = Interaction.MsgBox("Person mit dieser Nummer wurde noch nicht eingegeben");
                Modul1.Trans--;
                return;
            case -2:
                _ = Interaction.MsgBox("Person mit dieser Nummer ist nicht eingegeben");
                Modul1.Trans--;
                return;
            case -3:
                _ = Interaction.MsgBox("Person hat für diese Position das falsche Geschlecht");
                Modul1.Trans--;
                return;
        }
    }


    public void btnChildren_Click(object sender, EventArgs e)
    {
        FamSaveAend(Modul1.FamInArb);
        if (View.lstChildren.SelectedIndex > -1)
        {
            Modul1.PersInArb = View.lstChildren.SelectedItem.ItemData<int>();
            View.btnAppDelete.Enabled = true;
            View.btnAppDelete.Text = "Kind Pers.-Nr." + Modul1.PersInArb.AsString() + " aus Familie entfernen";
        }
        else
        {
            View.btnAppDelete.Enabled = false;
            View.btnAppDelete.Text = "";
        }
        View.chbAppAsAdopted.CheckState = CheckState.Unchecked;
        View.frmAppendPerson.Visible = true;
        View.chbAppAsAdopted.Visible = true;
        View.lblAppLabel44.Text = $"{Modul1.IText[EUserText.tFamilyNo]} {iFamNr} {Modul1.IText[EUserText.t138]}";
        View.lblAppLabel44.Tag = 3;
    }

    public void Button18_Click_1(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_0284, IL_0958
        string name = ((Button)sender).Name;
        if (name == View.btnAppCancel.Name)
        {
            btnAppCancel_Click(sender, e);
            return;
        }
        else if (name == View.btnAppRechPers.Name)
        {
            btnAppRechPers_Click(sender, e);
            return;
        }
        else if (name == View.btnAppFromFile.Name)
        {
            btnAppFromFile_Click(sender, e);

        }
        else if (name == View.btnAppNewPerson.Name)
        {
            btnAppNewPerson_Click(sender, e);
            return;
        }
        else
        {
            return;
        }
    }

    public void btnAppFromFile_Click(object sender, EventArgs e)
    {
        Kindneu = 0;
        View.chbAppAsAdopted.Visible = false;
        View.frmAppendPerson.Visible = false;

        Namensuch namensuch = MainProject.Forms.Namensuch;
        namensuch.Close();
        namensuch.Show();
        namensuch.ClearSelection();
        if (View.lblAppLabel44.Tag.AsInt() == 2)
        {
            namensuch.chbFemale2.CheckState = CheckState.Checked;
        }
        if (View.lblAppLabel44.Tag.AsInt() == 3)
        {
            namensuch.chbMale2.CheckState = CheckState.Checked;
        }
        if (View.lblAppLabel44.Tag.AsInt() == 3)
        {
            if (View.fraFather.iPNr != 0)
            {
                Modul1.PersInArb = View.fraFather.iPNr;
            }
            else if (View.fraMother.iPNr != 0)
            {
                Modul1.PersInArb = View.fraMother.iPNr;
            }
            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
            namensuch.Close();
            namensuch.Show();
            if (namensuch.List1.SelectedIndex > 10)
            {
                namensuch.List1.TopIndex = namensuch.List1.SelectedIndex - 5;
            }
            namensuch.ComboBox1.Text = Modul1.Person.SurName;
            namensuch.btnStartSearch.PerformClick();
            Modul1.Schalt = 2;
            Modul1.Suchfam = 0;
            Modul1.SuchPer = 0;
            ShowNameDialog();
        }
        else
        {
            Modul1.Schalt = 2;
            Modul1.Suchfam = 0;
            Modul1.SuchPer = 0;
            ShowNameDialog();
        }
        View.chbAppAsAdopted.Visible = false;
        View.frmAppendPerson.Visible = false;

        Modul1.PersInArb = Modul1.SuchPer;
        Modul1.FamInArb = iFamNr;
        if (Modul1.SuchPer <= 0)
        {
            return;
        }
        Modul1.eLKennz = DataModul.Person.GetSex(Modul1.PersInArb) == "F" ? ELinkKennz.lkMother : ELinkKennz.lkFather;
        if (!DataModul.Link.Exist(Modul1.FamInArb, Modul1.PersInArb, Modul1.eLKennz))
        {
            _ = Interaction.MsgBox("Person ist bereits Elternteil in diser Familie");
            return;
        }
        if (View.lblAppLabel44.Tag.AsInt() != 0)
        {
            Modul1.eLKennz = View.lblAppLabel44.Tag.AsEnum<ELinkKennz>();
        }
        if (View.chbAppAsAdopted.Checked)
        {
            Modul1.eLKennz = ELinkKennz.lkAdoptedChild;
        }
        short Rich;
        if (Modul1.eLKennz == ELinkKennz.lkChild)
        {

            if (DataModul.Link.GetPersonFam(Modul1.PersInArb, Modul1.eLKennz, out int iFamNr2))
            {
                string text = $"Person {Modul1.PersInArb} ist bereits Kind in Familie {iFamNr2}";
                text += "\n\nSoll sie als Adoptivkind eingefügt werden?";
                var ftValue = (float)Interaction.MsgBox(text, title: "", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Question);
                if (ftValue == 2f)
                {
                    View.TextBox3.Text = "";
                    return;
                }
                Modul1.eLKennz = ELinkKennz.lkAdoptedChild;
            }
            new CLinkData(ELinkKennz.lkAdoptedChild, Modul1.FamInArb, Modul1.PersInArb).AppendDB();
            Modul1.Ubg = iFamNr;
            Fameinlesen(Modul1.FamInArb, out Rich);
            return;
        }
        if (!DataModul.Link.SetVerknQ(Modul1.FamInArb, Modul1.PersInArb, Modul1.eLKennz, DialogResult.Yes,
            (p, f) =>
            {
                string text = $"Kind Pers.-Nr. {p} aus Familie Nr. {f} entfernen?";
                return Interaction.MsgBox(text, icon: MessageBoxIcon.Exclamation, mb: MessageBoxButtons.YesNo, title: "");
            }))
        {
            if (Modul1.Trans == 0)
            {
                Modul1.Trans = 1;
            }
        }
        Clear();
        Fameinlesen(Modul1.FamInArb, out Rich);
    }

    public void btnAppNewPerson_Click(object sender, EventArgs e)
    {
        Personen frmPerson = Personen.Default;
        frmPerson.Close();
        if (Modul1.Trans == 0)
        {
            Modul1.Trans = 1;
        }
        if (Modul1.Typ == DriveType.CDRom)
        {
            _ = Interaction.MsgBox(Modul1.Message_sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
            return;
        }
        if (Modul1.System.xDemo && DataModul.Person.Count > 100)
        {
            _ = Interaction.MsgBox(_Modul1.Instance.Message_sDemoVerNotPossibl);
            View.chbAppAsAdopted.Visible = false;
            View.frmAppendPerson.Visible = false;
            View.lblAppLabel44.Tag = 0;
            return;
        }
        View.chbAppAsAdopted.Visible = false;
        View.frmAppendPerson.Visible = false;
        Modul1.FamInArb = iFamNr;
        string lblFamText = frmPerson.lblFamPers.Text;
        ELinkKennz lkLink = ELinkKennz.lkNone;
        EUserText eHdrText = default;
        if (View.lblAppLabel44.Tag.AsInt() == 1)
        {
            eHdrText = EUserText.t162;
            lblFamText = $"Fam.: {Modul1.FamInArb,10} {Modul1.IText[eHdrText]} einfügen";
            lkLink = ELinkKennz.lkFather;
        }
        if (View.lblAppLabel44.Tag.AsInt() == 2)
        {
            eHdrText = EUserText.t163;
            lblFamText = $"Fam.: {Modul1.FamInArb,10} {Modul1.IText[eHdrText]} einfügen";
            lkLink = ELinkKennz.lkMother;
        }
        if (View.lblAppLabel44.Tag.AsInt() == 3)
        {
            if (!View.chbAppAsAdopted.Checked)
            {
                eHdrText = EUserText.tChild_AS;
                lblFamText = $"Fam.: {Modul1.FamInArb,10} {Modul1.IText[EUserText.tChild_AS]} hinzufügen";
                lkLink = ELinkKennz.lkChild;
            }
            else
            {
                eHdrText = EUserText.t452;
                lblFamText = $"Fam.: {Modul1.FamInArb,10} {Modul1.IText[eHdrText]} hinzufügen";
                lkLink = ELinkKennz.lkAdoptedChild;
            }
        }

        Modul1.PersInArb = DataModul.Person_NewPerson(frmPerson.Clear, _Modul1.Instance.Schalt);

        Ad = true;
        int persInArb = View.fraFather.iPNr;
        int persInArb2 = View.fraMother.iPNr;
        frmPerson.Show(persInArb, EUserText.t158);
        Ad = false;
        frmPerson.Clear();
        frmPerson.SetData(Modul1.PersInArb, iFamNr, lblFamText, eHdrText, lkLink);

        if (Strings.InStr(lblFamText.ToUpper(), Modul1.IText[EUserText.tChild_AS].ToUpper()) != 0)
        {
            var M1_Iter = 0;
            while (M1_Iter <= 100)
            {
                Modul1.Kont[M1_Iter] = "";
                M1_Iter++;
            }

            frmPerson.lblMarriages.Text = Modul1.IText[EUserText.tMarrCount] + " 0";
            frmPerson.edtSex.Text = " ";
            _ = frmPerson.edtSex.Focus();
            Modul1.PersInArb = persInArb;

            var dB_FamilyTable = DataModul.DB_FamilyTable;
            var dB_PersonTable = DataModul.DB_PersonTable;

            if (Modul1.PersInArb > 0
                || ((Modul1.PersInArb = persInArb2) > 0))
            {
                dB_PersonTable.Seek("=", Modul1.PersInArb);
                int Person_iReligi = dB_PersonTable.Fields[PersonFields.religi].AsInt();
                frmPerson.edtReligion.Text = Person_iReligi != 0 ? DataModul.TextLese1(Person_iReligi) : "";
            }
            dB_FamilyTable.Index = nameof(FamilyIndex.Fam);
            dB_FamilyTable.Seek("=", Modul1.FamInArb);
            int Family_iName = dB_FamilyTable.Fields[FamilyFields.Name].AsInt();
            int Family_iPrae = dB_FamilyTable.Fields[FamilyFields.Prae].AsInt();
            int Family_iSuf = dB_FamilyTable.Fields[FamilyFields.Suf].AsInt();

            string Family_sName = Family_iName != 0 ? DataModul.TextLese1(Family_iName) : "";
            string Family_sPrae = Family_iPrae != 0 ? DataModul.TextLese1(Family_iPrae) : "";
            string Family_sSuf = Family_iSuf != 0 ? DataModul.TextLese1(Family_iSuf) : "";

            if (Family_sName.Trim() != "")
            {
                frmPerson.edtSuffix.Text = Family_sSuf;
                frmPerson.edtPrefix.Text = Family_sPrae;
                frmPerson.edtSurnames.Text = Family_sName;
                return;
            }
            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
            if (View.TextBox3.Text.Trim() != "")
            {
                frmPerson.edtSurnames.Text = View.TextBox3.Text.Trim();
            }
            else
            {
                frmPerson.edtSurnames.Text = Modul1.Person.SurName;
                frmPerson.edtPrefix.Text = Modul1.Person.Prefix;
                frmPerson.edtSuffix.Text = Modul1.Person.Suffix;
            }

            if (Modul1.Aus[(int)EOutCfg.o24].AsInt() == 1)
            {
                frmPerson.frmDublicates.Height = 482;
                frmPerson.frmDublicates.Width = 790;
                frmPerson.frmDublicates.Location = new Point(0, 166);
                frmPerson.frmDublicates.Visible = true;
                frmPerson.Zeigfam(frmPerson.edtSurnames.Text.Trim());
            }
            return;
        }
        else
        {
            if (Strings.InStr(lblFamText, Modul1.IText[EUserText.t162]) == 0)
            {
                return;
            }
            frmPerson.lblMarriages.Text = Modul1.IText[EUserText.tMarrCount] + " 0";
            Famles(Modul1.FamInArb, Modul1.Family);
            Modul1.PersInArb = Modul1.Family.Kind[1];

            if (Modul1.PersInArb > 0)
            {
                var dB_PersonTable = DataModul.DB_PersonTable;
                dB_PersonTable.Seek("=", Modul1.PersInArb);
                int Person_iReligi = dB_PersonTable.Fields[PersonFields.religi].AsInt();
                frmPerson.edtReligion.Text = Person_iReligi != 0 ? DataModul.TextLese1(Person_iReligi) : "";

                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                frmPerson.edtSurnames.Text = Modul1.Person.SurName;
                frmPerson.edtPrefix.Text = Modul1.Person.Prefix;
                frmPerson.edtSuffix.Text = Modul1.Person.Suffix;
                _ = frmPerson.edtGivennames.Focus();
                if (Modul1.Aus[(int)EOutCfg.o24].AsInt() == 1.0)
                {
                    frmPerson.frmDublicates.Height = 482;
                    frmPerson.frmDublicates.Width = 790;
                    frmPerson.frmDublicates.Location = new Point(0, 166);
                    frmPerson.frmDublicates.Visible = true;
                    frmPerson.Zeigfam(frmPerson.edtSurnames.Text.Trim());
                }
            }
        }

    }


    public void btnAppCancel_Click(object sender, EventArgs e)
    {
        Kindneu = 0;
        View.chbAppAsAdopted.Visible = false;
        View.frmAppendPerson.Visible = false;
        View.lblAppLabel44.Tag = 0;
        _ = View.btnChildren.Focus();
    }

    public void btnAppRechPers_Click(object sender, EventArgs e)
    {
        RechText rechText = MainProject.Forms.RechText;

        Kindneu = 0;
        View.chbAppAsAdopted.Visible = false;
        View.frmAppendPerson.Visible = false;

        Modul1.SuchPer = 0;
        rechText.Show();
        if (View.lblAppLabel44.Tag.AsInt() == 2)
        {
            rechText._Bef_3.Enabled = false;
        }
        if (View.lblAppLabel44.Tag.AsInt() == 1)
        {
            rechText._Bef_3.Enabled = false;
        }
        if (View.lblAppLabel44.Tag.AsInt() == 3)
        {
            if (View.fraFather.iPNr != 0)
            {
                Modul1.PersInArb = View.fraFather.iPNr;
            }
            else if (View.fraMother.iPNr != 0)
            {
                Modul1.PersInArb = View.fraMother.iPNr;
            }
            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
            rechText.Text2.Text = Modul1.Person.SurName;
        }
        rechText.Hide();
        Modul1.PersInArb = 0;
        _ = rechText.ShowDialog();
        var FamPerschalt = rechText.FamPerSchalt;
        Modul1.PersInArb = rechText.PersInArb;

        if (FamPerschalt == 1)
        {
            View.Close();
            Ad = true;
            Personen.Default.Show(Modul1.PersInArb, EUserText.t158);
            return;
        }
        if (Modul1.PersInArb != 0)
            Modul1.SuchPer = Modul1.PersInArb;
    }

    private static void ShowNameDialog()
    {
        checked
        {
            Namensuch frmNamensuch = MainProject.Forms.Namensuch;
            ComboBox comboBox1 = frmNamensuch.ComboBox1;
            _ = comboBox1.Focus();
            comboBox1.SelectionStart = comboBox1.Text.Length;
            frmNamensuch.Visible = false;
            _ = frmNamensuch.ShowDialog();
        }
    }

    public void btnSearchPartner_Click(object sender, EventArgs e)
    {
        Kindneu = 0;
        checked
        {
            Modul1.FamInArb = iFamNr;
            FamdatPrüfen(Modul1.FamInArb);
            if (Datfehler_Abbruch != 2)
            {
                Modul1.Suchfam = 0;
                MainProject.Forms.Partnerrecherche.Show();
                _ = MainProject.Forms.Partnerrecherche._Text1_0.Focus();
                MainProject.Forms.Partnerrecherche.Hide();
                _ = MainProject.Forms.Partnerrecherche.ShowDialog();
                if (Modul1.Suchfam == 0)
                {
                    Modul1.Suchfam = iFamNr;
                }
                Modul1.FamInArb = Modul1.Suchfam;
                short Rich;
                Fameinlesen(Modul1.FamInArb, out Rich);
            }
        }
    }

    [RelayCommand]
    private void DeleteFamily()
    {
        Modul1.FamInArb = iFamNr;
        if (Modul1.Typ == DriveType.CDRom)
        {
            _ = Interaction.MsgBox(Modul1.Message_sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
            return;
        }

        if (View.fraFather.iPNr + View.fraMother.iPNr + View.lstChildren.Items.Count > 0)
        {
            _ = Interaction.MsgBox("Vor dem löschen müssen alle Personen aus der Familie entfernt werden", title: "Familie löschen nicht möglich", icon: MessageBoxIcon.Warning);
        }
        do
        {
            if (DeleteParent("Vater", View.fraFather.iPNr, ELinkKennz.lkFather)
                || DeleteParent("Mutter", View.fraMother.iPNr, ELinkKennz.lkMother))
                break;

            var ftValue = 6f;
            while ((View.lstChildren.Items.Count != 0) && (ftValue == 6f))
            {
                View.lstChildren.SelectedIndex = 0;
                ftValue = (float)DeleteChildren(Modul1.FamInArb, View.lstChildren.SelectedItem.ItemData<int>(), ELinkKennz.lkChild);
                if (ftValue == 6f)
                    View.lstChildren.Items.RemoveAt(0);
            }

            if (ftValue != 6f)
                break;

            ftValue = (float)Interaction.MsgBox("Alle Daten dieser Familie gehen verloren", icon: MessageBoxIcon.Exclamation, mb: MessageBoxButtons.YesNo, title: "Familie wirklich löschen?");
            if (ftValue != 7f)
            {
                Modul1.FamInArb = iFamNr;
                int M1_Iter = 500;
                while (M1_Iter <= 510)
                {
                    DataModul.Event.DeleteBeSu(M1_Iter.AsEnum<EEventArt>(), iFamNr);
                    M1_Iter++;
                }
                M1_Iter = 601;
                while (M1_Iter <= 603)
                {
                    DataModul.Event.DeleteBeSu(M1_Iter.AsEnum<EEventArt>(), iFamNr);
                    M1_Iter++;
                }

                DataModul.Link.DeleteFamWhere(iFamNr, (l) => l.eKennz != ELinkKennz.lkGodparent && l.eKennz != ELinkKennz.lk9);

                DataModul.SourceLink_DeleteAllPF(iFamNr, 2);
                DataModul.SourceLink_DeleteAllWhere(iFamNr, 3, (eA) => eA > EEventArt.eA_499);

                DataModul.Witness.DeleteAllF(iFamNr, 10);

                var dB_FamilyTable = DataModul.DB_FamilyTable;
                if (Modul1.FamInArb == 1)
                {
                    dB_FamilyTable.Index = nameof(FamilyIndex.Fam);
                    dB_FamilyTable.Seek("=", Modul1.FamInArb);
                    if (!dB_FamilyTable.NoMatch)
                    {
                        dB_FamilyTable.Delete();
                        _ = Interaction.MsgBox("Familie wurde gelöscht");
                        dB_FamilyTable.AddNew();
                        View.fraFather.iFamNr = Modul1.FamInArb;
                        dB_FamilyTable.Fields[FamilyFields.AnlDatum].Value = DateTime.Today.ToString("yyyyddMM");
                        dB_FamilyTable.Fields[FamilyFields.EditDat].Value = DateTime.Today.ToString("yyyyddMM");
                        dB_FamilyTable.Fields[FamilyFields.Prüfen].Value = "1    ";
                        dB_FamilyTable.Fields[FamilyFields.Bem1].Value = " ";
                        dB_FamilyTable.Fields[FamilyFields.FamNr].Value = Modul1.FamInArb;
                        dB_FamilyTable.Fields[FamilyFields.Name].Value = 0;
                        dB_FamilyTable.Fields[FamilyFields.Aeb].Value = 0;
                        dB_FamilyTable.Fields[FamilyFields.Fuid].Value = Guid.NewGuid();
                        dB_FamilyTable.Update();
                    }
                }
                else
                {
                    dB_FamilyTable.Index = nameof(FamilyIndex.Fam);
                    dB_FamilyTable.Seek("=", Modul1.FamInArb);
                    if (!dB_FamilyTable.NoMatch)
                    {
                        dB_FamilyTable.Delete();
                        _ = Interaction.MsgBox("Familie wurde gelöscht");
                        DataModul.LeerTab_AddRaw(Modul1.FamInArb, "F");
                    }
                }

                Clear();
                Modul1.FamInArb++;
            }
        }
        while (false);
        short Rich;
        Fameinlesen(Modul1.FamInArb, out Rich);

    }

    private DialogResult DeleteChildren(int famInArb, int iChild, ELinkKennz iKennz)
    {
        DialogResult UserQuery(int i)
        {
            string text;
            text = $"Kind Pers.-Nr. {i} aus Familie Nr. {famInArb} entfernen?";
            text += "\n\nDie Person bleibt erhalten, nur die Verknüpfung wird aufgelöst";
            return Interaction.MsgBox(text, icon: MessageBoxIcon.Exclamation, mb: MessageBoxButtons.YesNo, title: "");
        }
        Modul1.Ubg = 0;
        int persInArb = iChild;
        var mr = DataModul.Link.DeleteChildren(famInArb, persInArb, iKennz, DialogResult.Yes, UserQuery);
        return mr;
    }

    private bool DeleteParent(string sParName, int FamNr, ELinkKennz iKennz)
    {
        if (FamNr > 0)
        {
            string text = $"{sParName} aus der Familie entfernen?\n\nDie Person bleibt erhalten, nur die Verknüpfung wird aufgelöst";
            var mr = Interaction.MsgBox(text, icon: MessageBoxIcon.Exclamation, mb: MessageBoxButtons.YesNo, title: "Vorsicht!!!");
            if (mr == DialogResult.Yes)
            {
                DataModul.Link.DeleteFam(FamNr, iKennz);
            }
            else
                return true;
        }
        return false;
    }

    public void btnFamilysheet_Click(object sender, EventArgs e)
    {
        Modul1.Schalt = 9;
        Modul1.FamInArb = iFamNr;
        FamSaveAend(Modul1.FamInArb);
        MainProject.Forms.Namensuch.Show(Modul1.FamInArb);
    }

    public void CheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        Modul1.Aendf = true;
    }

    public void RichTextBox1_Click(object sender, EventArgs e)
    {
        checked
        {
            if (View.RichTextBox1.Top > View.lblFamilynotes.Top)
            {
                Posi[0] = (short)View.RichTextBox1.Top;
                Posi[1] = (short)View.RichTextBox1.Height;
            }
            View.RichTextBox1.Top = View.lblEngagement.Top;
            View.RichTextBox1.Height = 480;
            View.RichTextBox1.Height = View.btnEndTextinput.Top - View.lblEngagement.Top;
            if (View.RichTextBox1.Text.Trim() == "")
            {
                View.RichTextBox1.SelectionStart = 0;
            }
            HideButtons();
            View.btnEndTextinput.Visible = true;
            View.Button26.Visible = true;
        }
    }

    public void RichTextBox1_KeyDown(object sender, KeyEventArgs e)
    {
        if (View.btnSearchNumber.Visible)
        {
            View.RichTextBox1.Top = 200;
            View.RichTextBox1.Height = 480;
            if (View.RichTextBox1.Text.Trim() == "")
            {
                View.RichTextBox1.SelectionStart = 0;
            }
            HideButtons();
            View.btnEndTextinput.Visible = true;
        }
        checked
        {
            short num = (short)e.KeyCode;
            short num2 = (short)unchecked((int)e.KeyData / 65536);
            Modul1.Trans = 1;
            if (num2 == 0)
            {
                switch (num)
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
                        View.RichTextBox1.SelectedText = Modul1.Te[num - 113];
                        break;
                }
            }
        }
    }

    private void HideButtons()
    {
        View.btnSearchNumber.Visible = false;
        View.btnSearchName.Visible = false;
        View.btnReenter.Visible = false;
        View.btnNext.Visible = false;
        View.btnPrevious.Visible = false;
        View.btnEnableCheck.Visible = false;
        View.btnSearchPartner.Visible = false;
        View.btnSearchRegister.Visible = false;
        View.btnMainmenue.Visible = false;
        View.btnDelete.Visible = false;
        View.btnFamilysheet.Visible = false;
        View.btnSearchNumber2.Visible = false;
        View.btnResarch.Visible = false;
        View.btnChildren.Visible = false;
        View.btnResidence.Visible = false;
    }

    public void btnEndTextinput_Click(object sender, EventArgs e)
    {
        View.RichTextBox1.Top = Posi[0];
        View.RichTextBox1.Height = Posi[1];
        View.btnEndTextinput.Visible = false;
        View.Button26.Visible = false;

        ShowButtons();
        Modul1.Aendf = true;
        FamSaveAend(Modul1.FamInArb);
    }

    public void Button26_Click(object sender, EventArgs e)
    {
        View.RichTextBox1.Top = Posi[0];
        View.RichTextBox1.Height = Posi[1];
        View.btnEndTextinput.Visible = false;
        View.Button26.Visible = false;
        ShowButtons();
        Modul1.FamInArb = iFamNr;
        short Rich;
        Fameinlesen(Modul1.FamInArb, out Rich);
    }

    private void ShowButtons()
    {
        View.btnSearchNumber.Visible = true;
        View.btnSearchName.Visible = true;
        View.btnReenter.Visible = true;
        View.btnNext.Visible = true;
        View.btnPrevious.Visible = true;
        View.btnEnableCheck.Visible = true;
        View.btnSearchPartner.Visible = true;
        View.btnSearchRegister.Visible = true;
        View.btnMainmenue.Visible = true;
        View.btnDelete.Visible = true;
        View.btnFamilysheet.Visible = true;
        View.btnResarch.Visible = true;
        View.btnChildren.Visible = true;
        View.btnResidence.Visible = true;
    }

    private void fraFather_DeleteClick(object sender, EventArgs e)
    {
        Modul1.FamInArb = iFamNr;
        if (!(View.fraFather.iPNr > 0))
        {
            return;
        }
        Msg = "Vater aus der Familie entfernen?\n\nDie Person bleibt erhalten, nur die Verknüpfung wird aufgelöst";
        var ftValue = (float)Interaction.MsgBox(Msg, icon: MessageBoxIcon.Exclamation, mb: MessageBoxButtons.YesNo, title: "Vorsicht!!!");
        if (ftValue == 6f)
        {
            Clipboard.Clear();

            if (DataModul.Link.GetFamPerson(Modul1.FamInArb, ELinkKennz.lkFather, out var iPerNr))
            {
                Clipboard.SetText(iPerNr.AsString());
                _ = DataModul.Link.Delete(Modul1.FamInArb, iPerNr, ELinkKennz.lkFather);
            }
            Modul1.Ubg = iFamNr;
            Modul1.Aendf = true;
            Modul1.Trans = 1;
            FamSaveAend(Modul1.FamInArb);
            short Rich;
            Fameinlesen(Modul1.FamInArb, out Rich);
        }
    }

    private void fraMother_DeleteClick(object sender, EventArgs e)
    {
        Modul1.FamInArb = iFamNr;
        if (View.fraMother.iPNr == 0)
        {
            return;
        }
        Msg = "Mutter aus der Familie entfernen?\n\nDie Person bleibt erhalten, nur die Verknüpfung wird aufgelöst";
        var ftValue = (float)Interaction.MsgBox(Msg, icon: MessageBoxIcon.Exclamation, mb: MessageBoxButtons.YesNo, title: "Vorsicht!!!");
        if (ftValue != 6f)
        {
            return;
        }
        Clipboard.Clear();
        if (DataModul.Link.GetFamPerson(Modul1.FamInArb, ELinkKennz.lkMother, out var iPerNr))
        {
            Clipboard.SetText(iPerNr.AsString());
            _ = DataModul.Link.Delete(Modul1.FamInArb, iPerNr, ELinkKennz.lkMother);
        }
        Modul1.Ubg = checked(iFamNr);
        Modul1.Aendf = true;
        Modul1.Trans = 1;
        FamSaveAend(Modul1.FamInArb);
        short Rich;
        Fameinlesen(Modul1.FamInArb, out Rich);
    }

    public void Button21_Click(object sender, EventArgs e)
    {
        if (View.lstChildren.SelectedIndex > -1)
        {
            Modul1.PersInArb = View.lstChildren.SelectedItem.ItemData<int>();
            Modul1.eLKennz = ELinkKennz.lkChild;
            if (Strings.Mid(View.lstChildren.Text, 104, 9) == "Adoptiert")
            {
                Modul1.eLKennz = ELinkKennz.lkAdoptedChild;
            }
            Modul1.FamInArb = checked(iFamNr);

            DialogResult UserQuery(int i)
            {
                Msg = $"Kind Pers.-Nr. {i} aus Familie Nr. {Modul1.FamInArb} entfernen?";
                Msg += "\n\nDie Person bleibt erhalten, nur die Verküpfung wird aufgelöst";

                var mb = Interaction.MsgBox(Msg, icon: MessageBoxIcon.Exclamation, mb: MessageBoxButtons.YesNo, title: "");
                if (mb == DialogResult.Yes)
                {
                    Clipboard.SetText($"{i}");
                    View.lstChildren.Items.RemoveAt(View.lstChildren.SelectedIndex);
                }
                return mb;
            }
            if (DialogResult.Yes != DataModul.Link.DeleteChildren(Modul1.FamInArb, Modul1.PersInArb, Modul1.eLKennz, DialogResult.Yes, UserQuery))
                return;

            View.frmAppendPerson.Visible = false;
            Modul1.FAendmerk = true;
            Modul1.Trans = 1;
            FamSaveAend(Modul1.FamInArb);
            short Rich;
            Fameinlesen(Modul1.FamInArb, out Rich);
        }
    }

    public void btnResearch_Click(object sender, EventArgs e)
    {
        RechText rechText = MainProject.Forms.RechText;
        _ = rechText.ShowDialog();
        Modul1.FamInArb = rechText.FamInArb;
        var FamPerschalt = rechText.FamPerSchalt;
        if (FamPerschalt == 1)
        {
            View.Close();
            Ad = true;
            Personen.Default.Show(Modul1.PersInArb, EUserText.t158);
            return;
        }
        if (Modul1.FamInArb == 0)
        {
            Modul1.FamInArb = checked(iFamNr);
        }
        short Rich;
        Fameinlesen(Modul1.FamInArb, out Rich);
    }

    public void TextBox4_KeyPress(object sender, KeyPressEventArgs e)
    {
        short num = checked((short)Strings.Asc(e.KeyChar));
        Modul1.Aendf = true;
        if (num == 13)
        {
            num = 0;
        }
        if (num == 13)
        {
            View.ListBox3.Visible = false;
        }
        e.KeyChar = Strings.Chr(num);
        if (num == 0)
        {
            e.Handled = true;
        }
    }

    public void edtNamePS_KeyUp(object sender, KeyEventArgs e)
    {
        short num = checked((short)e.KeyCode);
        Modul1.Aendf = true;
        if (num == 13)
        {
            View.ListBox3.Visible = false;
            num = 0;
        }
        if (num <= 8)
        {
            return;
        }
        if (((TextBox)sender).Name == View.TextBox4.Name)
        {
            View.ListBox3.Visible = true;
            Textzeig1(ETextKennz.tkName, View.TextBox4.Text.TrimEnd(), View.ListBox3);
            Modul1.Aendf = true;
            Modul1.Trans = 1;
            num = 0;
            if (num == 13)
            {
                _ = Interaction.MsgBox("Stop", title: "23", mb: MessageBoxButtons.OK);
            }
            _ = View.TextBox4.Focus();
        }
        else if (((TextBox)sender).Name == View.edtNameprefix.Name)
        {
            View.ListBox3.Visible = true;
            Textzeig1(ETextKennz.A_, View.edtNameprefix.Text.TrimEnd(), View.ListBox3);
            Modul1.Aendf = true;
            Modul1.Trans = 1;
            num = 0;
            if (num == 13)
            {
                _ = Interaction.MsgBox("Stop", title: "23", mb: MessageBoxButtons.OK);
            }
            _ = View.edtNameprefix.Focus();
        }
        else if (((TextBox)sender).Name == View.edtNameSuffix.Name)
        {
            View.ListBox3.Visible = true;
            Textzeig1(ETextKennz.B_, View.edtNameSuffix.Text.TrimEnd(), View.ListBox3);
            Modul1.Aendf = true;
            Modul1.Trans = 1;
            num = 0;
            if (num == 13)
            {
                _ = Interaction.MsgBox("Stop", title: "23", mb: MessageBoxButtons.OK);
            }
            _ = View.edtNameSuffix.Focus();
        }
    }

    public static void Textzeig1(ETextKennz eTKennz, string ubgT, ListBox listBox3)
    {
        listBox3.Items.Clear();
        _Modul1.Instance.STextles($"{nameof(Familie)}.{listBox3.Name}", eTKennz, ubgT, listBox3.Items);
        listBox3.Visible = true;
        listBox3.Visible = true;
    }

    public void Listbox3_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        switch (Modul1.eNKennz)
        {
            case ETextKennz.tkName:
                View.TextBox4.Text = View.ListBox3.Text.Left(240);
                _ = View.edtNameprefix.Focus();
                break;
            case ETextKennz.A_:
                View.edtNameprefix.Text = View.ListBox3.Text.Left(240);
                _ = View.edtNameSuffix.Focus();
                break;
            case ETextKennz.B_:
                View.edtNameSuffix.Text = View.ListBox3.Text.Left(240);
                _ = View.lstChildren.Focus();
                break;
        }
        Modul1.Aendf = true;
        View.ListBox3.Visible = false;
    }

    public void TextBox1_KeyUp(object sender, KeyEventArgs e)
    {
        //Discarded unreachable code: IL_00c1, IL_01a2
        if (checked((short)e.KeyCode) is not (8 or 46))
        {
            return;
        }

        if (Modul1.Trans == 0)
        {
            Modul1.Trans = 1;
        }
        checked
        {
            Modul1.FamInArb = iFamNr;
            string name = ((TextBox)sender).Name;
            if (name == nameof(View.fraFather.edtParentPNr))
            {
                fraFather_PNr_KeyUp(sender, e);
            }
            else if (name == nameof(View.fraMother.edtParentPNr))
            {
                fraMother_PNr_KeyUp(sender, e);
            }

        }
    }

    private void fraFather_PNr_KeyUp(object sender, KeyEventArgs e)
    {
        if ((short)e.KeyCode is 8 or 46
            && View.fraFather.iPNr <= 0)
        {
            Modul1.eLKennz = ELinkKennz.lkFather;
            DataModul.Link.DeleteFam(Modul1.FamInArb, ELinkKennz.lkFather);
            Modul1.Ubg = iFamNr;
            Modul1.FAendmerk = true;
            FamSaveAend(Modul1.FamInArb);
            short Rich;
            Fameinlesen(Modul1.FamInArb, out Rich);
        }
    }

    private void fraMother_PNr_KeyUp(object sender, KeyEventArgs e)
    {
        if ((short)e.KeyCode is 8 or 46
            && View.fraMother.iPNr <= 0)
        {
            Modul1.eLKennz = ELinkKennz.lkMother;
            DataModul.Link.DeleteFam(Modul1.FamInArb, ELinkKennz.lkMother);
            Modul1.Ubg = iFamNr;
            Modul1.FAendmerk = true;
            FamSaveAend(Modul1.FamInArb);
            short Rich;
            Fameinlesen(Modul1.FamInArb, out Rich);
        }
    }

    public void lblPicures_Click(object sender, EventArgs e)
    {
        Modul1.FamInArb = iFamNr;
        Modul1.Ubg = Modul1.FamInArb;
        Modul1.sPKennz = "F";
        MainProject.Forms.Bilder.Show();
    }

    public void cbxIllegitRel_Click(object sender, EventArgs e)
    {
        if (View.cbxIllegitRel.Checked)
        {
            SetLabels(false);
        }
        else
        {
            SetLabels(true);
        }
    }


    public void Label46_Click_(object sender, EventArgs e)
    {
        checked
        {
            Modul1.FamInArb = iFamNr;
            string name = ((Label)sender).Name;
            if (name == View.Label46.Name)
            {
                Label46_Click2(sender, e);
            }
            else if (name == View.Label45.Name)
            {
                Label45_Click(sender, e);
            }
            else if (name == View.Label47.Name)
            {
                Label47_Click(sender, e);
            }
        }
    }

    public void Label45_Click(object sender, EventArgs e)
    {
        int M1_Iter = default;
        var aiPers = new List<int>();
        foreach (var link in DataModul.Link.ReadAllFams(iFamNr, ELinkKennz.lkWitnOfEngage))
        {
            if (M1_Iter++ > 99)
                break;
            aiPers.Add(link.iPersNr);
        }
        Rahmen.Default.btnDelete.Enabled = false;
        Rahmen.Default.ShowDialog(7, aiPers, EUserText.t302);

    }
    public void Label47_Click(object sender, EventArgs e)
    {
        int M1_Iter = default;
        var aiPers = new List<int>();
        foreach (var link in DataModul.Link.ReadAllFams(iFamNr, ELinkKennz.lkWitnOfMarr))
        {
            if (M1_Iter++ > 99)
                break;
            aiPers.Add(link.iPersNr);
        }
        Rahmen.Default.btnDelete.Enabled = false;
        Rahmen.Default.ShowDialog(8, aiPers, EUserText.t302);

    }

    public void Label46_Click2(object sender, EventArgs e)
    {
        int M1_Iter = default;
        var aiPers = new List<int>();
        foreach (var link in DataModul.Link.ReadAllFams(iFamNr, ELinkKennz.lkMarrWitness))
        {
            if (M1_Iter++ > 99)
                break;
            aiPers.Add(link.iPersNr);
        }
        DataModul.DB_FamilyTable.Seek("=", iFamNr);
        if (DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].AsString().Length > 1)
        {
            Rahmen.Default.RTB.Text = DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].AsString();
        }
        Rahmen.Default.btnDelete.Enabled = false;
        Rahmen.Default.ShowDialog(6, aiPers, EUserText.t302);

    }

    public void Label46_Click(object sender, EventArgs e)
    {
        int famInArb = iFamNr;
        IList<int> aiPers = new List<int>();
        string name = ((Label)sender).Name;
        Rahmen frmRahmen = Rahmen.Default;
        if (name == View.Label46.Name)
        {
            aiPers = AppendFamKnz(famInArb, ELinkKennz.lkMarrWitness);

            DataModul.DB_FamilyTable.Seek("=", famInArb);
            if (DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].AsString().Length > 1)
            {
                frmRahmen.RTB.Text = DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].AsString();
            }
        }
        else if (name == View.Label45.Name)
        {
            aiPers = AppendFamKnz(famInArb, ELinkKennz.lkWitnOfEngage);
        }
        else if (name == View.Label47.Name)
        {
            aiPers = AppendFamKnz(famInArb, ELinkKennz.lkWitnOfMarr);
        }

        Modul1.FamInArb = famInArb;
        frmRahmen.btnDelete.Enabled = false;
        frmRahmen.ShowDialog(Modul1.Ubg, aiPers, EUserText.t302);

        static IList<int> AppendFamKnz(int famInArb, ELinkKennz sKennz)
        {
            int M1_Iter = default;
            List<int> aiPers = new();
            foreach (var item in DataModul.Link.ReadAllFams(famInArb, sKennz))
            {
                aiPers.Add(item.iPersNr);
                if (M1_Iter++ > 99)
                    break;
            }

            return aiPers;
        }
    }
    public void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
    {
        View.ComboBox2.Tag = View.ComboBox2.Text.Right(10);
        _ = View.lstChildren.Focus();
    }

    public void ComboBox2_TextChanged(object sender, EventArgs e)
    {
        View.ComboBox2.Tag = View.ComboBox2.Text.Right(10);
    }

    public void RichTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
    {
        _ = Process.Start(e.LinkText);
    }

    public void Button23_Click(object sender, EventArgs e)
    {
        Modul1.FamInArb = iFamNr;
        DataModul.Event.ChgEvent(EEventArt.eA_Marriage, Modul1.FamInArb, EEventArt.eA_MarrReligious);

        DataModul.SourceLink_ChangeEvent(Modul1.FamInArb, EEventArt.eA_Marriage, EEventArt.eA_MarrReligious);

        DataModul.Witness_ChangeEventArt(Modul1.FamInArb, EEventArt.eA_Marriage, EEventArt.eA_MarrReligious);
        short Rich;
        Fameinlesen(Modul1.FamInArb, out Rich);
        _ = View.lblHdrBorn_Name.Focus();
    }

    public void Button24_Click(object sender, EventArgs e)
    {
        var dB_FamilyTable = DataModul.DB_FamilyTable;

        dB_FamilyTable.Index = nameof(FamilyIndex.Fam);
        dB_FamilyTable.MoveFirst();
        while (!dB_FamilyTable.EOF)
        {
            int famInArb = dB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();

            if (!DataModul.Event.Exists(EEventArt.eA_MarrReligious, famInArb))
            {
                DataModul.Event.ChgEvent(EEventArt.eA_Marriage, famInArb, EEventArt.eA_MarrReligious);

                DataModul.SourceLink_ChangeEvent(famInArb, EEventArt.eA_Marriage, EEventArt.eA_MarrReligious);

                DataModul.Witness_ChangeEventArt(famInArb, EEventArt.eA_Marriage, EEventArt.eA_MarrReligious);
            }
            dB_FamilyTable.MoveNext();
        }
        Debugger.Break();
        short Rich;

        Fameinlesen(Modul1.FamInArb, out Rich);
    }

    private void SetLabels(bool xSetVal)
    {
        View.lblProklamation.Enabled = xSetVal;
        View.lblMarriage.Enabled = xSetVal;
        View.lblDivorce.Enabled = xSetVal;
        View.lblDimissorale.Enabled = xSetVal;
        View.lblEngagement.Enabled = xSetVal;
        View.lblReligMarr.Enabled = xSetVal;
        View.lblPartnership.Enabled = xSetVal;
    }

    public void EnableLabels()
    {
        SetLabels(true);
    }

    public void CheckDisableLabels()
    {
        if (View.cbxIllegitRel.Checked)
        {
            SetLabels(false);
            //=================
        }
    }

    public void SetEventText(string text, int ubg, string text2)
    {
        Label lbl;
        string sHdr;
        (lbl, sHdr) = GetLbHdr(ubg);
        if (lbl != null)
        {
            lbl.Text = $"{sHdr}: {text} {text2}".Replace("  ", " ");
            Modul1.UbgT = "";
        }
#nullable enable
        (Label?, string) GetLbHdr(int ubg) => ubg switch
        {
            500 => (View.lblProklamation, Modul1.IText[EUserText.tProclamation]),
            501 => (View.lblEngagement, Modul1.IText[EUserText.tEngagement]),
            502 => (View.lblMarriage, Modul1.IText[EUserText.tMarriage]),
            503 => (View.lblReligMarr, Modul1.IText[EUserText.tMarriageRelig]),
            504 => (View.lblDivorce, Modul1.IText[EUserText.tDivorce]),
            505 => (View.lblPartnership, Modul1.IText[EUserText.tPartnership]),
            507 => (View.lblDimissorale, Modul1.IText[EUserText.t261]),
            _ => (null, ""),
        };
#nullable restore
    }

    public void SetData(int FamInArb, IFamilyData cFamily)
    {
        Familie frmFamilie = MainProject.Forms.Familie;

        DataModul.DB_SourceLinkTable.Index = "Tab";
        DataModul.DB_SourceLinkTable.Seek("=", 2, FamInArb);
        bool xFamExists = !DataModul.DB_SourceLinkTable.NoMatch;
        frmFamilie.lblSources.SetLabelTxt(xFamExists || !string.IsNullOrEmpty(cFamily.sBem[3]), Modul1.IText[EUserText.t244], frmFamilie.lblHdrList.BackColor, Modul1.IText);

        frmFamilie.CheckBox2.Checked = cFamily.iGgv > 0;
    }


    public void Fameinlesen(int FamInArb, out short Rich)
    {

        //int num4 = 0;
        //while (num4 <= 100)
        //{
        //    Kont[num4] = "";
        //    num4++;
        //}
        Rich = 0;

        Clear();
        EnableLabels();

        if (DataModul.Family.Count == 0)
        {
            View.Show();
            View.btnReenter.PerformClick();
            Modul1.Family.Mann = 0;
            Modul1.Family.Frau = 0;
            return;
            //=================
        }

        View.Button23.Visible = false;

        Rich = Modul1.Famsatzles(FamInArb, Rich, Modul1.Family);
        SetData(FamInArb, Modul1.Family);
        View.TextBox4.Text = Modul1.Family.sName;
        View.edtNameprefix.Text = Modul1.Family.sPrefix;
        View.edtNameSuffix.Text = Modul1.Family.sSuffix;
        View.cbxIllegitRel.Checked = Modul1.Family.xAeB;
        iFamNr = FamInArb;
        View.Label24.Text = Modul1.Family.sName;
        View.RichTextBox1.Text = Modul1.Family.sBem[1];
        View.Label25.Text = Modul1.Family.sSuffix;

        bool xMatch = DataModul.Picture_ExistsFam(FamInArb);

        View.lblPictures.SetLabelTxt(xMatch, Modul1.IText[EUserText.t129], View.lblHdrList.BackColor, Modul1.IText);

        if (Modul1.Family.sPruefen == "N")
        {
            View.btnEnableCheck.Text = Modul1.IText[EUserText.t151];
            View.btnEnableCheck.Tag = 1;
            View.btnEnableCheck.BackColor = Color.FromArgb(255, 0, 0, 0);
            //=================
        }
        else
        {
            View.btnEnableCheck.Text = Modul1.IText[EUserText.t152];
            View.btnEnableCheck.Tag = 0;
            View.btnEnableCheck.BackColor = View.btnPrevious.BackColor;
        }

        FamWohnles(Modul1.FamInArb, EEventArt.eA_602);
        FamWohnles(Modul1.FamInArb, EEventArt.eA_603);
        View.cbxIllegitRel.Enabled = true;
        Famdatles(Modul1.FamInArb);
        if (Modul1.EstDateLes(out var sEstMarrDat))
            View.lblEstimatedMarr.Text = Modul1.IText[EUserText.t260] + ": " + sEstMarrDat;

        View.Label46.Text = "";
        View.Label46.Visible = false;
        if (DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].AsString() != "")
        {
            View.Label46.Text = Modul1.IText[EUserText.tMarrWitness] + " " + Modul1.IText[EUserText.tYes];
            View.Label46.Visible = true;
        }
        Famles(FamInArb, _Modul1.Instance.Family);
        string str = "";
        if (Modul1.Family.Mann > 0)
        {
            var iPers = Modul1.Family.Mann;
            if (!DataModul.Person.ReadData(iPers, out var cPers))
            {
                _ = Interaction.MsgBox("Person mit dieser Nummer (" + iPers.AsString() + " ) ist nicht vorhanden!", title: "Family.Mann", mb: MessageBoxButtons.OK);
                View.fraFather.iPNr = 0;

                return;
                //=================
            }
            if (cPers.sSex == "U")
            {
                _ = Interaction.MsgBox("Geschlecht ist nicht identifizierbar, keine Ehe möglich");
            }

            if (!View.CheckBox2.Checked
                    && Strings.Trim((
                        DataModul.DB_FamilyTable.Fields[FamilyFields.Prüfen].AsString()).AsString()) != "N"
                        && (cPers.sSex != "M"))
            {
                _ = Interaction.MsgBox("Ehemann ist eine Frau", title: "", icon: MessageBoxIcon.Error);
            }
            Modul1.Person_ReadNames(iPers, Modul1.Person);
            Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person));
            str = Modul1.Person.FullSurName;
            View.lblFamName.Text = Modul1.Person.FullSurName.Trim();
        }

        View.List2.Items.Clear();
        var num4 = 1;
        while (num4 <= 99
            && num4 < Modul1.Family.Kinder.Count
            && Modul1.Family.Kinder[num4].nr != 0)
        {
            string text = Modul1.Family.Kinder[num4].aTxt == "A" ? "A" : "";
            var PersInArb = Modul1.Family.Kind[num4];

            var dt = DataModul.Event.GetPersonBirthOrBapt(PersInArb);
            string text2 = dt == default
        ? "00000000"
        : dt.ToString("yyyyMMdd");

            _ = View.List2.Items.Add(new ListItem(text2 + "             " + PersInArb.AsString().Right(10) + text, PersInArb));
            num4++;
        }
        int num10 = 0;
        while (View.List2.Items.Count != 0
            && !(num10 >= View.List2.Items.Count))
        {
            var iPersChild = View.List2.Items[num10].ItemData<int>();
            HandleChild(num10, str, iPersChild, this);
            num10 += 1;
        }
        View.lblChildCount.Text = Modul1.IText[EUserText.tChildCount] + num10.AsString();
        bool xIgnoreSex = View.CheckBox2.Checked;

        HandlePerson(Modul1.Family.Mann, View.fraFather);



        if (Modul1.Family.Frau > 0)
        {
            if (DataModul.Person.Exists(Modul1.Family.Frau))
            {
                _ = Interaction.MsgBox("Person mit dieser Nummer (" + Modul1.Family.Frau.AsString() + " ) ist nicht vorhanden!", title: "Family.Frau", mb: MessageBoxButtons.OK);
                View.fraMother.iPNr = 0;

                goto end_IL_0001_2;
                //=================
            }

            string Frau_sSex = DataModul.Person.GetSex(Modul1.Family.Frau);
            if (!xIgnoreSex
            && Strings.Trim(DataModul.DB_FamilyTable.Fields[FamilyFields.Prüfen].AsString()) != "N"
            && Frau_sSex != "F")
            {
                _ = Interaction.MsgBox("Ehefrau ist ein Mann", title: "", icon: MessageBoxIcon.Error);
            }
        }
        HandlePerson(Modul1.Family.Frau, View.fraMother);


        if (!View.Visible)
        {
            return;
        }

        if (Modul1.Typ != DriveType.CDRom)
        {
            Modul1.Letzte = new() { iPerson = Modul1.Letzte.iPerson, iFamily = iFamNr };
            Modul1.Persistence.PutIntMand("Letzter.dat", Modul1.Letzte.iFamily, 2L);
        }

        if (View.lblMarriage.Text.Trim() != Modul1.IText[EUserText.tMarriage] + ":" & View.lblReligMarr.Text.Trim() == Modul1.IText[EUserText.tMarriageRelig] + ":")
        {
            View.Button23.Visible = true;
            //=================
        }
        goto end_IL_0001_2;

    end_IL_0001_2: // <========== 5
        return;

    }

    public void FamWohnles(int FamInArb, EEventArt eArt)
    {

        List<IListItem<int>> lLi1 = new();
        List<IListItem<int>> lLi2 = new();
        string sText1 = "";
        int iTag1 = 0;
        string sText2 = "";
        int iTag2 = 0;
        foreach (var cEvent in DataModul.Event.ReadEventsBeSu(FamInArb, eArt))
        {
            {
                if (cEvent.iLfNr > 0)
                {
                    var Li = Modul1.Event_ToShortLine(cEvent);
                    if (cEvent.sReg.Trim() != "")
                        if (eArt == EEventArt.eA_602)
                        {
                            sText1 = Li.ToString() + cEvent.iLfNr.AsString();
                            iTag1 = Li.ItemData;
                            lLi1.Add(Li);
                        }
                    if (eArt == EEventArt.eA_603)
                    {
                        if (cEvent.sReg != " ")
                        {
                            sText2 = Li.ToString() + cEvent.iLfNr.AsString();
                            iTag2 = Li.ItemData;
                        }
                        lLi2.Add(Li);
                    }
                }
            }
        }

        ComboBox comboBox1 = View.ComboBox1;
        ComboBox comboBox2 = View.ComboBox2;

        if (eArt == EEventArt.eA_602)
        {
            comboBox1.BackColor = SystemColors.Window;
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(lLi1.ToArray());
            comboBox1.Tag = iTag1;
            comboBox1.Text = sText1;
        }
        else if (eArt == EEventArt.eA_603)
        {
            comboBox2.BackColor = ColorTranslator.FromOle(-2147483643);
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(lLi2.ToArray());
            comboBox2.Tag = iTag2;
            comboBox2.Text = sText2;
        }

        if (comboBox1.Items.Count > 0)
        {
            if (comboBox1.Text.Trim() == "")
            {
                comboBox1.SelectedIndex = 0;
                comboBox1.Tag = comboBox1.Items[0].AsString().Right(10);
            }
            comboBox1.BackColor = Color.FromArgb(-2147483643);
            if (comboBox1.Items.Count > 1)
            {
                comboBox1.BackColor = Color.FromArgb(12648447);
            }
        }
        if (comboBox2.Items.Count > 0)
        {
            if (comboBox2.Text.Trim() == "")
            {
                comboBox2.Text = comboBox2.Items[0].AsString().Left(240);
                comboBox2.Tag = comboBox2.Items[0].AsString().Right(10);
            }
            comboBox2.BackColor = ColorTranslator.FromOle(-2147483643);
            if (comboBox2.Items.Count > 1)
            {
                comboBox2.BackColor = Color.FromArgb(12648447);
            }
        }
    }
    private bool HandleChild(int iChNr, string sFamilyName, int iPersChild, FamilieViewModel frmFamily)
    {
        int FamInArb;
        if (!DataModul.Person.Exists(iPersChild))
        {
            _ = Interaction.MsgBox("Person mit dieser Nummer (" + iPersChild.AsString() + " ) ist nicht vorhanden!", title: Modul1.IText[EUserText.tChild_AS], mb: MessageBoxButtons.OK);
            frmFamily.View.TextBox3.Text = "";
            return false;
            //=================
        }

        var evts = Modul1.FamPerDatles(iPersChild, 2);

        var sDest = new string(' ', 200);
        //num4 = 11;
        //while (num4 <= 14)
        //{
        //    Kont[num4] = $"{Kont[num4].Trim(),10}";
        //    num4++;
        //}
        if (evts.TryGetValue(EEventArt.eA_Birth, out var edt) && edt.Item3 != default)
        {
            StringType.MidStmtStr(ref sDest, 1, 10, edt.Item3.AsString());
            //=================
        }
        else if (evts.TryGetValue(EEventArt.eA_Baptism, out var edt2) && edt2.Item3 != default)
        {
            StringType.MidStmtStr(ref sDest, 1, 10, edt.Item3.AsString());
        }

        if (Modul1.Person.xDead)
        {
            StringType.MidStmtStr(ref sDest, 53, 10, Modul1.Person.xDead ? "j" : "");
            //=================
        }
        else
        {
            StringType.MidStmtStr(ref sDest, 53, 10, Modul1.Person.sBurried);
        }

        var Persex = DataModul.Person.GetSex(iPersChild);
        if (Persex != "F")
        {
            Persex = "M";
        }
        ELinkKennz eLKennz;
        if (Persex == "M")
        {
            eLKennz = ELinkKennz.lkFather;
        }
        else if (Persex == "F")
        {
            eLKennz = ELinkKennz.lkMother;
        }
        else
        {
            _ = Interaction.MsgBox("Geschlecht ist nicht identifizierbar, keine Ehe möglich");
            return false;
        }
        //=================
        var aiFam = Modul1.Link_Famsuch(iPersChild, eLKennz);
        IList<ListItem<int>> items1 = [];
        items1.Clear();
        var num4 = 1;
        while (num4 <= aiFam.Count)
        {
            var Datu = "        ";
            int num6 = aiFam[num4 - 1];
            var num14 = EEventArt.eA_500;
            while (num14 <= EEventArt.eA_507)
            {
                var dt = DataModul.Event.GetDate(num14, num6);
                if (dt != default)
                {
                    Datu = dt.ToString("yyyyMMdd");
                    break;
                }
                num14++;
            }
            if (Datu.Trim() == "")
            {
                var dt = DataModul.Event.GetDate(EEventArt.eA_601, num6);
                if (dt != default)
                {
                    Datu = dt.ToString("yyyyMMdd");
                }
            }
            var LiText = new string(' ', 60);
            if ((Modul1.Person.SurName != "") | (Kont_3 != ""))
            {
                StringType.MidStmtStr(ref LiText, 1, 40, "" + Datu.Right(8));
            }
            StringType.MidStmtStr(ref LiText, 41, 19, new string(' ', 20) + num6.AsString().Right(19));
            items1.Add(new(LiText, num6));
            LiText = "";
            num4++;
        }

        int? iFamYear = null;
        FamInArb = 0;
        if (items1.Count > 0)
        {
            FamInArb = items1[0].ItemData<int>();
            iFamYear = Modul1.FamDatYear(FamInArb, 1);
            StringType.MidStmtStr(ref sDest, 72, 8, items1[0].ItemString.Right(8));
            StringType.MidStmtStr(ref sDest, 82, 8, $"   {iFamYear:D4}     ");
        }
        StringType.MidStmtStr(ref sDest, 64, 8, "        " + iPersChild.AsString().Right(8));

        Kont_3 = "";
        Modul1.Person_ReadNames(iPersChild, Modul1.Person);
        Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person));
        if (sFamilyName.Trim() != Modul1.Person.FullSurName.Trim())
        {
            Kont_3 = Kont_3.Trim() + " " + Modul1.Person.FullSurName.ToUpper();
        }
        StringType.MidStmtStr(ref sDest, 12, 40, Kont_3);
        _ = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out int iAhn, out var sData);
        //=================

        string sInsert = "";
        IList items = frmFamily.View.List2.Items;
        if (Strings.Mid(items[iChNr].AsString(), 19, 1) == "A")
        {
            sInsert = "Adoptiert";
        }
        StringType.MidStmtStr(ref sDest, 91, 9, $"{iAhn,7}{sData}");
        StringType.MidStmtStr(ref sDest, 101, 9, sInsert);

        _ = frmFamily.View.lstChildren.Items.Add(new ListItem<PersonFamily>(sDest, new(iPersChild, FamInArb)));
        //=================
        return true;
    }

    private void HandlePerson(int iPersonNr, FraParentView fraPerson)
    {
        string sAncestData;
        string text12;
        if (iPersonNr > 0)
        {
            fraPerson.iPNr = iPersonNr;
            Modul1.Person_ReadNames(iPersonNr, Modul1.Person);
            Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person));
            sAncestData = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out _, out _);
            bool xPicBemExist = DataModul.Picture_PersonDataExist(iPersonNr, PersonFields.Bem1);
            bool xPicExists = DataModul.Picture_Exists(iPersonNr, 'P');
            string sEvent300 = DataModul.Event_GetLabelText(iPersonNr, EEventArt.eA_300, Modul1.Event_PreDisplay);
            string sEventTitle = DataModul.Event_GetLabelText(iPersonNr, EEventArt.eA_301, Modul1.Event_PreDisplay);

            if (sEventTitle != "")
            {
                sEventTitle = sEventTitle.Length > Modul1.IText[EUserText.tOccupation].Length
                    ? sEvent300 + ", " + sEventTitle
                    : Modul1.IText[EUserText.t70] + " " + sEventTitle.Trim();
            }
            text12 = "";
            if (xPicBemExist)
                text12 += "(Bem.)   ";
            if (xPicExists)
                text12 += "(Foto)   ";
            text12 += sAncestData;
            sEvent300 = Modul1.IText[EUserText.tTitle] + ": " + sEvent300.Trim();


            fraPerson.PersText12 = text12.Trim();
            fraPerson.PersName = Modul1.IText[EUserText.tName] + " " + Modul1.Person.Prae + Modul1.Person.FullSurName;
            fraPerson.PersGivn = Modul1.IText[EUserText.tGivenname] + " " + Modul1.Person.Givennames;
            fraPerson.PersTitle = sEventTitle;
            fraPerson.PersText10 = Modul1.IText[EUserText.tResidence] + ": " + DataModul.Event_GetLabelText(iPersonNr, EEventArt.eA_302, Modul1.Event_PreDisplay);

            Modul1.FamPerDatles(iPersonNr, 0);
            sAncestData = "";
            if (Modul1.Person.Suffix.Trim() != "")
            {
                sAncestData = " / ";
            }

            fraPerson.PersText8 = Modul1.IText[EUserText.t257] + " " + Modul1.Person.Prefix.Trim() + sAncestData + Modul1.Person.Suffix.Trim().Left(62);
            sAncestData = "";
            if (Modul1.Person.Alias.Trim() != "")
            {
                sAncestData = " / ";
            }

            fraPerson.PersAka = Modul1.IText[EUserText.t258] + " " + Kont_3.Trim() + sAncestData + Modul1.Person.Alias.Trim().Left(62);

            var aiFam = Modul1.Link_Famsuch(iPersonNr, Modul1.eLKennz = ELinkKennz.lkFather);
            fraPerson.PersNrMarr = Modul1.IText[EUserText.tMarrCount] + " " + aiFam.Count.AsString();
            if (Modul1.Eltsuch(iPersonNr) is int iParentFam and > 0)
            {
                SetParentData(iParentFam, ELinkKennz.lkFather, fraPerson.lblGrandfather, Modul1.IText[EUserText.tGrandfather]);
                SetParentData(iParentFam, ELinkKennz.lkMother, fraPerson.lblGrandmother, Modul1.IText[EUserText.tGrandmother]);
            }
            else
            {
                fraPerson.lblGrandfather.Tag = 0;
                fraPerson.lblGrandfather.Text = Modul1.IText[EUserText.tGrandfather] + ": " + Modul1.IText[EUserText.tUnknown];
                fraPerson.lblGrandmother.Tag = 0;
                fraPerson.lblGrandmother.Text = Modul1.IText[EUserText.tGrandmother] + ": " + Modul1.IText[EUserText.tUnknown];
                //=================
            }
        }
        else
        {
            fraPerson.iPNr = 0;
        }
    }

    private void SetParentData(int famInArb, ELinkKennz iKennz, Label label, string sHeader)
    {
        label.Text = sHeader + ": " + Modul1.IText[EUserText.tUnknown];
        label.Tag = 0;
        if (DataModul.Link.GetFamPerson(famInArb, iKennz, out int iPers))
        {
            var person = new CPersonData();
            Modul1.Person_ReadNames(iPers, person);
            person.SetFullSurname(Modul1.BuildFullSurName(person));
            label.Text = $"{sHeader}: {iPers} {person.Givennames} {person.FullSurName}";
            label.Tag = iPers;
        }


    }

}
