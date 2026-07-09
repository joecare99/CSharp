using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using Gen_FreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Gen_FreeWin.ViewModels;

public partial class DubViewModel : ObservableObject, IDubViewModel
{
    IContainerControl? IDubViewModel.View { get; set; }
    private Dub View => (Dub)((IDubViewModel)this).View!;
    private IModul1 Modul1 => _Modul1.Instance;
    IInteraction Interaction => Menue.Default;
    IVBInformation Information => Modul1.Information;
    IProjectData ProjectData => Modul1.ProjectData;
    IStrings Strings => Modul1.Strings;

    private int F1;
    private int P1;
    private int P2;
    private int F2;
    private byte Loe;
    private float Modul1_Dub_Trans1;
    //   private string Modul1_Kont20;
    private string[] Modul1_Temp = new string[2];

    public void Command1_Click(object eventSender, EventArgs eventArgs)
    {
        ProjectData.ClearProjectError();
        short index = (short)View.ACommand1.GetIndex((Button)eventSender);
        View._Command1_5.Visible = false;
        View._Command1_7.Visible = false;
        View._Frame1_3.Text = "Person";
        View._Frame1_2.Text = "Person";
        View.Button2.Visible = false;
        View.Button3.Visible = false;
        View._Command1_9.Visible = true;
        View._Command1_10.Visible = true;
        switch (index)
        {
            case 0:
            case 3:
            case 4:
            case 9:
            case 10:
                break;
            default:
                if (Modul1.Typ == DriveType.CDRom)
                {
                    _ = Interaction.MsgBox(Modul1.Message_sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
                    return;
                }
                break;
        }
        if (Modul1.Trans != 0)
        {
            Frag();
        }
        switch (index)
        {
            case 0:
                Command1_0_Click(eventSender, eventArgs);
                break;
            case 1:
                Command1_1_Click(eventSender, eventArgs);
                break;
            case 2:
                Command1_2_Click(eventSender, eventArgs);
                break;
            case 3:
                Command1_3_Click(eventSender, eventArgs);
                break;
            case 4:
                Command1_4_Click(eventSender, eventArgs);
                break;
            case 5:
                Command1_5_Click(eventSender, eventArgs);
                break;
            case 7:
                Command1_7_Click(eventSender, eventArgs);
                break;
            case 9:
                Command1_9_Click(eventSender, eventArgs);
                break;
            case 10:
                Command1_10_Click(eventSender, eventArgs);
                break;
            default:
                _ = Interaction.MsgBox(index.AsString());
                break;
        }
    }

    private void HandleCommandErr(out int num, out int M1_Iter)
    {
        num = 172;
        View._Command1_1.Visible = false;
        View._Command1_2.Visible = false;
        int Fam = View._Label1_10.Tag.AsInt();
        int Fam2 = View._Label1_30.Tag.AsInt();
        M1_Iter = 0;
        num = 177;
        while (M1_Iter <= 18)
        {
            View.ALabel1[(short)M1_Iter].Text = "";
            View.ALabel1[(short)M1_Iter].Tag = 0;
            View.ALabel1[(short)M1_Iter].BackColor = ColorTranslator.FromOle(16777152);
            M1_Iter++;
        }
        M1_Iter = 20;
        if (M1_Iter <= 38)
        {
            View.ALabel1[(short)M1_Iter].Text = "";
            View.ALabel1[(short)M1_Iter].Tag = 0;
            View.ALabel1[(short)M1_Iter].BackColor = ColorTranslator.FromOle(16777152);
            M1_Iter++;
        }
        View._Label1_0.Text = Fam.AsString();
        View._Label1_0.Tag = Fam;
        View._Label1_20.Text = Fam2.AsString();
        View._Label1_20.Tag = Fam2;
        Modul1.FamInArb = Fam;
        // Todo: Check
        // Modul1.o01_Person.Prefix = "";
        var s = FamDat(Modul1.FamInArb);
        // Todo: Check
        if (s != "")
        {
            // Todo: Check
            View._Label1_1.Text = s;
        }
        Modul1.FamInArb = Fam2;
        s = FamDat(Modul1.FamInArb);
        if (s != "")
        {
            View._Label1_21.Text = s;
        }

        if (View._Label1_0.Tag.AsInt() != View._Label1_20.Tag.AsInt())
        {
            View._Frame1_3.Text = "Familie";
            View._Frame1_2.Text = "Familie";
            View.Button2.Visible = true;
            View.Button3.Visible = true;
            View._Command1_9.Visible = false;
            View._Command1_10.Visible = false;
            View._Command1_5.Visible = true;
            View._Command1_7.Visible = true;
            View._Command1_5.Text = $"Familie {View._Label1_0.Tag} löschen";
            View._Command1_7.Text = $"Familie {View._Label1_20.Tag} löschen";
        }
    }

    private void Command1_10_Click(object eventSender, EventArgs eventArgs)
    {
        Modul1.PersInArb = View._Label1_20.Tag.AsInt();
        Familie.Default.btnMainmenue.Text = Modul1.IText[EUserText.tNMBack];
        Personen.Default.Close();
        Modul1.Aend = 0f;
        Modul1.Ad = false;
        Personen.Default.Show(Modul1.PersInArb, EUserText.tNMBack);
    }

    private void Command1_9_Click(object eventSender, EventArgs eventArgs)
    {
        Modul1.PersInArb = View._Label1_0.Tag.AsInt();
        Personen.Default.Close();
        Familie.Default.btnMainmenue.Text = Modul1.IText[EUserText.tNMBack];
        Modul1.Aend = 0f;
        Modul1.Ad = false;
        Personen.Default.Show(Modul1.PersInArb, EUserText.tNMBack);
    }

    private void Command1_7_Click(object eventSender, EventArgs eventArgs)
    {
        var Fam = View._Label1_20.Tag.AsInt();
        var Fam2 = View._Label1_0.Tag.AsInt();
        Famweg(Fam, Fam2);
    }

    private void Command1_5_Click(object eventSender, EventArgs eventArgs)
    {
        var Fam = View._Label1_0.Tag.AsInt();
        var Fam2 = View._Label1_20.Tag.AsInt();
        Famweg(Fam, Fam2);
    }

    private void Command1_4_Click(object eventSender, EventArgs eventArgs)
    {
        checked
        {
            View.TextBox1.Text = "";
            View.TextBox2.Text = "";
            View._List1_0.Items.Clear();
            View._List1_1.Items.Clear();
            View.Close();
            Menue.Default.Show();
        }
    }

    private void Command1_3_Click(object eventSender, EventArgs eventArgs)
    {
        if (ColorTranslator.ToOle(View._Label1_20.BackColor) == 8454143)
        {
            Frm_Dub_Clear(0);
        }
        if (Modul1.Trans > 0)
        {
            Frag();
        }
        Frm_Dub_Clear(1);
        View._Command1_1.Visible = false;
        View._Command1_2.Visible = false;
        if (View.TextBox2.Text == "")
        {
            View.Button4.PerformClick();
        }
        else
        {
            Modul1_Temp[1] = View.TextBox2.Text;
            View.TextBox2.Text = "";
            View.TextBox2.Text = Modul1_Temp[1];
        }
    }

    private void Command1_2_Click(object eventSender, EventArgs eventArgs)
    {
        if (Modul1.Trans == 0)
        {
            DataModul.wrkDefault!.Begin();
            Modul1_Dub_Trans1 += 1f;
            Modul1.Trans = 1;
        }
        P2 = View._Label1_0.Tag.AsInt();
        P1 = View._Label1_20.Tag.AsInt();
        F1 = View._Label1_11.Tag.AsInt();
        F2 = View._Label1_31.Tag.AsInt();
        Tausch(P1, F1, P2, F2);
    }

    private void Command1_1_Click(object eventSender, EventArgs eventArgs)
    {
        if (Modul1.Trans == 0)
        {
            DataModul.wrkDefault!.Begin();
            Modul1_Dub_Trans1 += 1f;
            Modul1.Trans = 1;
        }
        P1 = View._Label1_0.Tag.AsInt();
        P2 = View._Label1_20.Tag.AsInt();
        F1 = View._Label1_11.Tag.AsInt();
        F2 = View._Label1_31.Tag.AsInt();
        Tausch(P1, F1, P2, F2);
    }

    private void Command1_0_Click(object eventSender, EventArgs eventArgs)
    {
        if (System.Drawing.ColorTranslator.ToOle(View._Label1_20.BackColor) == 8454143)
        {
            Frm_Dub_Clear(0);
        }
        if (Modul1.Trans > 0)
        {
            Frag();
        }
        Frm_Dub_Clear(1);
        View._Command1_1.Visible = false;
        View._Command1_2.Visible = false;
        if (View.TextBox1.Text == "" & View.Button4.Enabled)
        {
            View.Button4.PerformClick();
        }
        else
        {
            Modul1_Temp[0] = View.TextBox1.Text;
            View.TextBox1.Text = "";
            View.TextBox1.Text = Modul1_Temp[0];
        }
    }

    private void Frm_Dub_Clear(int iIndex)
    {
        int M1_Iter = 0 + iIndex * 20;
        while (M1_Iter <= 18 + iIndex * 20)
        {
            View.ALabel1[(short)M1_Iter].Text = "";
            View.ALabel1[(short)M1_Iter].Tag = 0;
            View.ALabel1[(short)M1_Iter].BackColor = System.Drawing.ColorTranslator.FromOle(16777215);
            M1_Iter++;
        }
        M1_Iter = 43 - iIndex * 2;
        while (M1_Iter <= 44 - iIndex * 2)
        {
            View.ALabel1[(short)M1_Iter].Text = "";
            View.ALabel1[(short)M1_Iter].Tag = 0;
            View.ALabel1[(short)M1_Iter].BackColor = System.Drawing.ColorTranslator.FromOle(16777215);
            M1_Iter++;
        }
        View.AFrame1[(short)(2 + iIndex)].Visible = false;
        View.AFrame1[(short)(0 + iIndex)].Visible = true;
        TextBox textBox = iIndex == 0 ? View.TextBox1 : View.TextBox2;
        Modul1_Temp[0] = textBox.Text;
        textBox.Text = "";
        textBox.Text = Modul1_Temp[0];
    }

    public void Dub_Load(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_02b7
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        int num4;
        var M1_Iter = 0;
        switch (try0001_dispatch)
        {
            default:
                ProjectData.ClearProjectError();
                num3 = 2;
                View.BackColor = Modul1.HintFarb;
                View._Command1_4.Text = Modul1.IText[EUserText.t158];
                if (Modul1.FontSize > 0f)
                {
                    float fS = Modul1.FontSize;
                    View.Font = new Font("Arial", fS, FontStyle.Regular);
                    View.List1[0].Font = new Font("courier new", Modul1.FontSize - 2f, FontStyle.Regular);
                    View.List1[1].Font = new Font("courier new", Modul1.FontSize - 2f, FontStyle.Regular);
                    M1_Iter = 0;
                    while (M1_Iter <= 38)
                    {
                        if (M1_Iter != 19)
                        {
                            View.ALabel1[(short)M1_Iter].Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                        }
                        lErl = 3;
                        M1_Iter++;
                    }
                    View._Label1_41.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                    View._Label1_42.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                    View._Label1_43.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                    View._Label1_44.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                }
                View.WindowState = Menue.Default.WindowState;
                var aiPos = Modul1.Persistence.ReadIntsMand("maspos.dat", 2);
                View.Left = aiPos[0];
                View.Top = aiPos[1];
                View.Show();
                _ = View.TextBox1.Focus();
                goto end_IL_0001_2;
            IL_0307:
                num4 = unchecked(num2 + 1);
                goto IL_030a;
            IL_030a:
                num2 = 0;
                switch (num4)
                {
                    case 1:
                        break;

                }
                goto default;
        }
    end_IL_0001_2:
        return;
    }


    public void Dub_FormClosing(object eventSender, FormClosingEventArgs eventArgs)
    {
        //Discarded unreachable code: IL_0056, IL_00cc
        int try0001_dispatch = -1;
        int num = default;
        bool cancel = default;
        int num2 = default;
        int num3 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                CloseReason closeReason;
                int num4;
                switch (try0001_dispatch)
                {
                    default:
                        num = 1;
                        goto IL_000b;
                    case 287:
                        {
                            num2 = num;
                            switch (num3 <= -2 ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_00cf;
                                default:
                                    goto end_IL_0001;
                            }
                            if (Information.Err().Number == 91)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                            }
                            else
                            {
                                if (Information.Err().Number != 3420)
                                {
                                    break;
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                            }
                            goto IL_00cf;
                        }
                    end_IL_0001_3:
                        break;
                    IL_000b:
                        num = 2;
                        cancel = eventArgs.Cancel;
                        closeReason = eventArgs.CloseReason;
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        View.List1[0].Items.Clear();
                        View.List1[1].Items.Clear();
                        goto end_IL_0001_2;
                    IL_00cf:
                        num4 = num2 + 1;
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 13:
                            case 15:
                                goto end_IL_0001_3;
                        }
                        goto default;
                }
                num = 15;
                eventArgs.Cancel = cancel;
                break;
            end_IL_0001:
                ;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 287;
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
    public void List1_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_1d2d
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        short num5 = default;
        int num7 = default;
        int lErl = default;
        int M1_Iter = default;
        int num4;
        short Schalt;
        short index;
        switch (try0001_dispatch)
        {
            default:
                num = 1;
                index = (short)View.List1.GetIndex((ListBox)eventSender);
                Modul1.PersInArb = (int)Math.Round(View.List1[index].Text.Right(10).AsDouble());
                View.AFrame1[index].Visible = false;
                View.AFrame1[(short)(index + 2)].Visible = true;

                var pt = DataModul.Person.Seek(Modul1.PersInArb);

                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out int iAhn, out string Modul1_Kont20);
                Modul1.Kont[97] = iAhn.AsString();
                if (View.List1[0].SelectedIndex > 10)
                {
                    View.List1[0].TopIndex = View.List1[0].SelectedIndex - 5;
                }
                GetALabel1_2d(index, 0).Text = $"{Modul1.PersInArb,20}{pt.Fields[PersonFields.Sex].AsString()} {Modul1.Person.Stat.Trim()} {Modul1.Person.Clan.Trim()}";
                GetALabel1_2d(index, 0).Tag = Modul1.PersInArb;
                Modul1_Temp[index] = Modul1.Person.SurName.Trim() + "," + Modul1.Person.Givennames.Trim();
                Modul1.Person.SetFullSurname(Modul1.Person.SurName);
                if (Modul1.Person.Prefix != "")
                {
                    Modul1.Person.SetFullSurname(Modul1.Person.Prefix + " " + Modul1.Person.FullSurName);
                }
                if (Modul1.Person.Suffix != "")
                {
                    Modul1.Person.SetFullSurname(Modul1.Person.FullSurName + " " + Modul1.Person.Suffix);
                }
                if (Modul1.Person.Alias != "")
                {
                    Modul1.Person.SetFullSurname(Modul1.Person.FullSurName + Modul1.Person.Alias.FrameIfNEoW(" (", ")"));
                }
                GetALabel1_2d(index, 1).Text = Modul1.Person.Givennames + " " + Modul1.Person.FullSurName;
                Modul1.Datles(Modul1.PersInArb, Modul1.Person);
                M1_Iter = 1;
                if (M1_Iter <= 4)
                {
                    if (Modul1.Kont[M1_Iter + 15] != "")
                    {
                        Modul1.Kont[M1_Iter + 10] = "B" + Modul1.Kont[M1_Iter + 10];
                    }
                    M1_Iter++;
                }
                GetALabel1_2d(index, 2).Text = Modul1.IText[EUserText.tBirthdaySh] + " " + Modul1.Person.Birthday;
                GetALabel1_2d(index, 3).Text = Modul1.IText[EUserText.tBaptismSh] + " " + Modul1.Person.Baptised;
                GetALabel1_2d(index, 4).Text = Modul1.IText[EUserText.tDeathSh] + " " + Modul1.Person.Death;
                GetALabel1_2d(index, 5).Text = Modul1.IText[EUserText.tBurialSh] + " " + Modul1.Person.Burial;
                GetALabel1_2d(index, -2).Text = Modul1.IText[EUserText.tOccupation];
                GetALabel1_2d(index, -1).Text = Modul1.IText[EUserText.tResidence];
                M1_Iter = 41;
                while (M1_Iter <= 44)
                {
                    View.ALabel1[(short)M1_Iter].BackColor = ColorTranslator.FromOle(16777215);
                    M1_Iter++;
                }

                if (DataModul.Event.ReadBeSu(EEventArt.eA_300, Modul1.PersInArb, out var cEv)
                    && cEv.iKBem > 0
                    && !string.IsNullOrEmpty(cEv.sKBem))
                {
                    GetALabel1_2d(index, -2).Text = $"{Modul1.IText[EUserText.tOccupation]} {cEv.sKBem.Trim()}";
                }
                GetALabel1_2d(index, -1).Text = Modul1.IText[EUserText.tResidence];
                var ubgT = "";

                if (DataModul.Event.ReadBeSu(EEventArt.eA_302, Modul1.PersInArb, out cEv))
                {
                    ubgT = "";
                    if (cEv.iOrt > 0 && DataModul.Place.ReadData(cEv.iOrt, out var cPlace))
                    {
                        ubgT = DataModul.Place.FullName(cPlace, true, true);
                    }
                    GetALabel1_2d(index, -1).Text = "Wohnort: " + ubgT.Trim();
                }
                var ubg = Modul1.Eltsuch(Modul1.PersInArb);
                GetALabel1_2d(index, 6).Text = "Eltern Ehe Nr.:             ".Left(15) + ubg.AsString();
                GetALabel1_2d(index, 6).Tag = ubg;
                Modul1.FamInArb = GetALabel1_2d(index, 6).Tag.AsInt();
                if (Modul1.FamInArb > 0)
                {
                    DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                    Modul1.PersInArb = Modul1.Family.Mann;
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out iAhn, out Modul1_Kont20);
                    Modul1.Kont[97] = iAhn.AsString();
                    GetALabel1_2d(index, 7).Text = Modul1.PersInArb.AsString() + " " + Modul1.Person.Givennames + " " + Modul1.Person.SurName;
                    Modul1.PersInArb = Modul1.Family.Frau;
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out iAhn, out Modul1_Kont20);
                    Modul1.Kont[97] = iAhn.AsString();
                    GetALabel1_2d(index, 8).Text = Modul1.PersInArb.AsString() + " " + Modul1.Person.Givennames + " " + Modul1.Person.SurName;
                }
                Modul1.PersInArb = (int)Math.Round(View.ALabel1[(short)(index * 20)].Text.AsDouble());
                Modul1.eLKennz = ELinkKennz.lkFather;
                ubgT = "";
                if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                {
                    Modul1.eLKennz = ELinkKennz.lkMother;
                }

                M1_Iter = 0;
                List<int> aiFam = new();
                foreach (var link in DataModul.Link.ReadAllPers(Modul1.PersInArb, Modul1.eLKennz))
                {
                    if (M1_Iter++ > 100) break;
                    aiFam.Add(link.iFamNr);
                }
                if (aiFam.Count > 0)
                {
                    GetALabel1_2d(index, 9).Text = "Eigene Ehe Nr.: " + string.Join(", ", aiFam);
                    num5 = 0;
                    num7 = aiFam.Count;
                    M1_Iter = 1;
                    while (M1_Iter <= num7
                        && !(index == 0 & 9 + M1_Iter + num5 > 18)
                        && !(index == 1 & 9 + M1_Iter + num5 > 38)
                        && M1_Iter != 4
                        && num5 <= 5)
                    {
                        Modul1.FamInArb = aiFam[M1_Iter - 1];

                        var s = FamDat(aiFam[M1_Iter - 1]);
                        if (s != "")
                        {
                            GetALabel1_2d(index, 9 + M1_Iter + num5).Text = s;
                        }
                        num5++;

                        DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                        Modul1.PersInArb = Modul1.eLKennz.AsDouble() == 1.0 ? Modul1.Family.Frau : Modul1.Family.Mann;
                        if (Modul1.PersInArb > 0)
                        {
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out iAhn, out Modul1_Kont20);
                            Modul1.Kont[97] = iAhn.AsString();
                            Modul1.Person.SetFullSurname(Modul1.Person.SurName);
                            if (Modul1.Person.Prefix != "")
                            {
                                Modul1.Person.SetFullSurname(Modul1.Person.Prefix + " " + Modul1.Person.FullSurName);
                            }
                            if (Modul1.Person.Suffix != "")
                            {
                                Modul1.Person.SetFullSurname(Modul1.Person.SurName + " " + Modul1.Person.Suffix);
                            }
                            GetALabel1_2d(index, 9 + M1_Iter + num5).Text = Modul1.PersInArb.AsString().Trim() + " " + Modul1.Person.Givennames + " " + Modul1.Person.FullSurName;
                            num5++;
                        }
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        if (Modul1.Family.Kind[1] != 0)
                        {
                            Modul1.PersInArb = Modul1.Family.Kind[1];
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out iAhn, out Modul1_Kont20);
                            Modul1.Kont[97] = iAhn.AsString();
                            if (Modul1.Person.Prefix != "")
                            {
                                Modul1.Person.SetFullSurname(Modul1.Person.Prefix + " " + Modul1.Person.SurName);
                            }
                            if (Modul1.Person.Suffix != "")
                            {
                                Modul1.Person.SetFullSurname(Modul1.Person.SurName + " " + Modul1.Person.Suffix);
                            }
                            GetALabel1_2d(index, 9 + M1_Iter + num5).Text = "Kind: " + Modul1.Person.Givennames + " " + Modul1.Person.FullSurName;
                            num5++;
                            Modul1.Datles(Modul1.PersInArb, Modul1.Person);

                            var sDate = "";
                            sDate = Modul1.IText[EUserText.tBirthdaySh] + " " + Modul1.Person.Birthday;

                            if (sDate.Trim() == "")
                            {
                                sDate = Modul1.IText[EUserText.tBaptismSh] + " " + Modul1.Person.Baptised;
                            }
                            if (sDate.Trim() == "")
                            {
                                sDate = Modul1.IText[EUserText.tDeathSh] + " " + Modul1.Person.Death;
                            }
                            if (sDate.Trim() == "")
                            {
                                sDate = Modul1.IText[EUserText.tBurialSh] + " " + Modul1.Person.Burial;
                            }
                            if (index == 0 & 9 + M1_Iter + num5 > 18 || index == 1 & 29 + M1_Iter + num5 > 38)
                            {
                                break;
                            }
                            GetALabel1_2d(index, 9 + M1_Iter + num5).Text = sDate;
                            num5++;
                        }
                        M1_Iter++;
                    }
                }
                lErl = 3;
                if (View._Frame1_2.Visible & View._Frame1_3.Visible)
                {
                    if (View._Label1_0.Tag.AsInt() == View._Label1_20.Tag.AsInt()
                          & View._Label1_0.Text.Left(20) != View._Label1_20.Text.Left(20))
                    {
                        if (View._Label1_6.Tag.AsInt() == View._Label1_26.Tag.AsInt()
                          | View._Label1_6.Tag.AsInt() == 0
                          | View._Label1_26.Tag.AsInt() == 0)
                        {
                            View._Command1_1.Visible = true;
                            View._Command1_2.Visible = true;
                            View._Command1_1.BackColor = View._Command1_3.BackColor;
                            View._Command1_2.BackColor = View._Command1_3.BackColor;
                            View._Command1_1.Text = $"Person löschen und alle Daten und Verknüpfungen nach Person {View._Label1_20.Tag.AsInt()} übertragen.\n(Ausnahme: Vornamen)";
                            View._Command1_2.Text = $"Person löschen und alle Daten und Verknüpfungen nach Person {View._Label1_0.Tag.AsInt()} übertragen.\n(Ausnahme: Vornamen)";
                        }
                        else
                        {
                            View._Command1_1.Visible = true;
                            View._Command1_2.Visible = true;
                            View._Command1_1.BackColor = Color.Red;
                            View._Command1_2.BackColor = Color.Red;
                            View._Command1_1.Text = $"Person löschen und alle Daten und Verknüpfungen nach Person {View._Label1_20.Tag.AsInt()} übertragen.\n(Ausnahme: Vornamen)\nDiese Eltern-Kind-Verbindung wird gelöscht";
                            View._Command1_2.Text = $"Person löschen und alle Daten und Verknüpfungen nach Person {View._Label1_0.Tag.AsInt()} übertragen.\n(Ausnahme: Vornamen)\nDiese Eltern-Kind-Verbindung wird gelöscht";
                        }
                    }
                }
                Modul1.PersInArb = (int)Math.Round(GetALabel1_2d(index, 0).Text.AsDouble());
                if (DataModul.Link.Count <= 0)
                {
                    goto end_IL_0001_2;
                }
                M1_Iter = (int)ELinkKennz.lkGodparent;
                M1_Iter++;
                while (M1_Iter <= (int)ELinkKennz.lkWitnOfMarr)
                {

                    foreach (var link in DataModul.Link.ReadAllPers(Modul1.PersInArb, M1_Iter.AsEnum<ELinkKennz>()))
                    {
                        if (M1_Iter == (int)ELinkKennz.lkGodparent)
                        {

                            var cEvt = DataModul.Event.ReadDataPl(EEventArt.eA_Baptism, link.iFamNr, out bool xBreak);
                            if (xBreak || cEvt.dDatumV == default)
                            {
                                cEvt = DataModul.Event.ReadDataPl(EEventArt.eA_Baptism, link.iFamNr, out xBreak);
                            }
                            Modul1.sDatu = !xBreak && cEvt.dDatumV != default ? cEvt.dDatumV.Year.AsString() : "          ";
                            GetALabel1_2d(index, 18).Text = Modul1.sDatu + " " + Modul1.IText[EUserText.tGodparents] + ": ";
                        }
                        else
                        {
                            var cEvt = DataModul.Event.ReadDataPl(EEventArt.eA_Marriage, link.iFamNr, out bool xBreak);
                            if (xBreak)
                            {
                                cEvt = DataModul.Event.ReadDataPl(EEventArt.eA_MarrReligious, link.iFamNr, out xBreak);
                            }
                            Modul1.sDatu = !xBreak && cEvt.dDatumV != default ? cEvt.dDatumV.Year.AsString() : "          ";
                            GetALabel1_2d(index, 18).Text = Modul1.sDatu + " " + Modul1.IText[EUserText.t128];
                        }

                    }

                }


                goto end_IL_0001_2;


            IL_1d85:
                num4 = unchecked(num2 + 1);
                goto IL_1d89;
            IL_1d89:
                num2 = 0;
                switch (num4)
                {
                    case 1:
                        break;
                    case 51:
                    case 76:
                    case 255:
                    case 269:
                    case 275:
                    case 276:
                    case 281:
                        goto end_IL_0001_2;
                }
                goto default;
        }
    end_IL_0001_2: // <========== 6
        return;
    }

    private Label GetALabel1_2d(short index, int iSub)
    {
        if (iSub >= 0)
            return View.ALabel1[(short)(index * 20 + iSub)];
        else
            return View.ALabel1[(short)(45 - index * -2 + iSub)];
    }
    public void Text1_TextChanged(object eventSender, EventArgs eventArgs)
    {
        short Index = (short)View.Text1.GetIndex((TextBox)eventSender);
        //    goto IL_0015;
        //case 440:
        //    {
        //        num2 = num;
        //        switch (num3 <= -2 ? 1 : num3)
        //        {
        //            case 2:
        //                break;
        //            case 1:
        //                goto IL_0160;
        //            default:
        //                goto end_IL_0001;
        //        }
        //        if (Information.Err().Number != 94)
        //        {
        //            goto end_IL_0001_2;
        //        }
        //        DataModul.DSB_SearchTable.Edit();
        //        DataModul.DSB_SearchTable.Fields["iKenn"].Value = " ";
        //        DataModul.DSB_SearchTable.Update();
        //        ProjectData.ClearProjectError();
        //        if (num2 == 0)
        //        {
        //            throw ProjectData.CreateProjectError(-2146828268);
        //        }
        //        num4 = num2;
        //        goto IL_0163;
        //    }
        //end_IL_0001:
        //    break;
        //IL_0015:
        if (View.Text1[Index].Text != "")
        {

            DataModul.DSB_SearchTable.Index = "Persuch";
            DataModul.DSB_SearchTable.Seek(">=", View.Text1[Index].Text, 0);
            View.List1[Index].Items.Clear();
            Zeig(ref Index);
        }
    }
    public void Ertausch(ref int P1, ref int P2)
    {
        ProjectData.ClearProjectError();
        Modul1.PersInArb = P1;
        var dB_EventTable = DataModul.DB_EventTable;
        int num4;
        EEventArt _Iter;
        for (_Iter = EEventArt.eA_Birth; _Iter <= EEventArt.eA_Death; _Iter++)
        {
            Loe = Vergl(_Iter, P1, P2);
            if (Loe != 100)
            {
                if (Loe == 0)
                {
                    dB_EventTable.Seek("=", _Iter, P1);
                    dB_EventTable.Delete();
                    DataModul.DB_SourceLinkTable.Index = "Tab22";
                    DataModul.DB_SourceLinkTable.Seek("=", 3, P1, _Iter, Modul1.LfNR);
                    while (!DataModul.DB_SourceLinkTable.NoMatch && !DataModul.DB_SourceLinkTable.NoMatch)
                    {
                        DataModul.DB_SourceLinkTable.Edit();
                        DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].Value = P2;
                        DataModul.DB_SourceLinkTable.Update();
                        DataModul.DB_SourceLinkTable.Seek("=", 3, P1, _Iter, Modul1.LfNR);
                    }

                    dB_EventTable.MoveNext();
                    _Iter++;
                    continue;

                }
                else
                {
                    if (Loe > 1)
                    {
                        dB_EventTable.Seek("=", _Iter.AsString(), P2.AsString());
                        dB_EventTable.Edit();
                        if (Modul1.Kont[10] != "")
                        {
                            dB_EventTable.Fields[EventFields.Bem1].Value = Strings.Trim(dB_EventTable.Fields[EventFields.Bem1].AsString().AsString()) != ""
                                ? (dB_EventTable.Fields[EventFields.Bem1].AsString() + " *** " + Modul1.Kont[10])
                                : Modul1.Kont[10];
                        }
                        if (Modul1.Person.Birthday != "")
                        {
                            dB_EventTable.Fields[EventFields.Bem2].Value = Strings.Trim(dB_EventTable.Fields[EventFields.Bem2].AsString().AsString()) != ""
                                ? (dB_EventTable.Fields[EventFields.Bem2].AsString() + " *** " + Modul1.Person.Birthday)
                                : Modul1.Person.Birthday;
                        }
                        if (Modul1.Kont[15] != "")
                        {
                            if (null == dB_EventTable.Fields[EventFields.Bem3].AsString())
                            {
                                dB_EventTable.Fields[EventFields.Bem3].Value = " ";
                            }
                            dB_EventTable.Fields[EventFields.Bem3].Value = Strings.Trim(dB_EventTable.Fields[EventFields.Bem3].AsString().AsString()) != ""
                                ? (dB_EventTable.Fields[EventFields.Bem3].AsString() + " *** " + Modul1.Kont[15])
                                : Modul1.Kont[15];
                        }
                        if (Modul1.Kont[17] != "")
                        {
                            if (null != dB_EventTable.Fields[17].AsString())
                            {
                                dB_EventTable.Fields[17].Value = dB_EventTable.Fields[17].AsString() != ""
                                    ? (dB_EventTable.Fields[17].AsString() + " *** " + Modul1.Kont[17])
                                    : Modul1.Kont[17];
                            }
                        }
                        dB_EventTable.Update();
                        dB_EventTable.Seek("=", _Iter.AsString(), P1.AsString());
                        dB_EventTable.Delete();
                    }
                    else if (Loe != 1)
                    {
                    }

                }
            }

            int num6 = 1;
            dB_EventTable.Index = nameof(EventIndex.BeSu);
            dB_EventTable.Seek("=", _Iter.AsString(), P2.AsString());
            if (dB_EventTable.NoMatch)
            {
                num6 = 0;
            }
            else if (
                dB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != _Iter ||
                dB_EventTable.Fields[EventFields.PerFamNr].AsInt() != P2)
            {
                num6 = 0;
            }

            num4 = 0;
            dB_EventTable.Index = nameof(EventIndex.BeSu);
            dB_EventTable.Seek("=", 105.AsString(), P2.AsString());
            while (!dB_EventTable.EOF
                && !dB_EventTable.NoMatch
                && dB_EventTable.Fields[EventFields.Art].AsInt() == 105
                && dB_EventTable.Fields[EventFields.PerFamNr].AsInt() == P2)
            {
                if (dB_EventTable.Fields[EventFields.LfNr].AsInt() > num4)
                {
                    num4 = dB_EventTable.Fields[EventFields.LfNr].AsInt().AsInt();
                }
                dB_EventTable.MoveNext();
            }
            dB_EventTable.Index = nameof(EventIndex.BeSu);
            dB_EventTable.Seek("=", _Iter.AsString(), Modul1.PersInArb.AsString());
            if (!dB_EventTable.NoMatch)
            {
                if (dB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() == _Iter
                    && dB_EventTable.Fields[EventFields.PerFamNr].AsInt() == Modul1.PersInArb)
                {
                    if (num6 == 0)
                    {
                        dB_EventTable.Edit();
                        dB_EventTable.Fields[EventFields.PerFamNr].Value = P2;
                        dB_EventTable.Update();
                        DataModul.DB_SourceLinkTable.Index = "Tab22";
                        DataModul.DB_SourceLinkTable.Seek("=", 3, Modul1.PersInArb, _Iter, Modul1.LfNR);
                        if (!DataModul.DB_SourceLinkTable.NoMatch)
                        {
                            DataModul.DB_SourceLinkTable.Edit();
                            DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].Value = P2;
                            DataModul.DB_SourceLinkTable.Update();
                        }

                        _Iter++;
                        continue;
                    }
                    else
                    {
                        num4++;
                        dB_EventTable.Seek("=", _Iter.AsString(), P1.AsString());
                        short lfNR = Modul1.LfNR;
                        int persInArb = Modul1.PersInArb;
                        lfNR = (short)dB_EventTable.Fields[EventFields.LfNr].AsInt();
                        DataModul.DB_SourceLinkTable.Index = "Tab22";
                        DataModul.DB_SourceLinkTable.Seek("=", 3, persInArb, _Iter, lfNR);
                        if (!DataModul.DB_SourceLinkTable.NoMatch)
                        {
                            DataModul.DB_SourceLinkTable.Edit();
                            DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].Value = P2;
                            DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].Value = 105;
                            DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.LfNr].Value = num4;
                            DataModul.DB_SourceLinkTable.Update();
                        }

                        dB_EventTable.Edit();
                        dB_EventTable.Fields[EventFields.LfNr].Value = num4;
                        Modul1.UbgT = _Iter switch
                        {// Todo i18N
                            EEventArt.eA_Birth => "Anderes Geburtsdatum",
                            EEventArt.eA_Baptism => "Anderes Taufdatum",
                            EEventArt.eA_Death => "Anderes Sterbedatum",
                            EEventArt.eA_Burial => "Anderes Begräbnisdatum",
                            _ => "",
                        };
                        Modul1.eTKennz = ETextKennz.T_;
                        var ubg = Modul1.TextSpeich(Modul1.UbgT, Modul1.UbgT1, Modul1.eTKennz, persInArb, lfNR);
                        dB_EventTable.Fields[EventFields.ArtText].Value = ubg;
                        dB_EventTable.Fields[EventFields.Art].Value = 105;
                        dB_EventTable.Fields[EventFields.PerFamNr].Value = P2;
                        dB_EventTable.Update();
                    }

                    dB_EventTable.MoveNext();
                }
            }
        }

        short num8 = 1;
        while (num8 <= 8)
        {

            _Iter = num8 switch
            {
                1 => EEventArt.eA_105,
                2 => EEventArt.eA_106,
                3 => EEventArt.eA_300,
                4 => EEventArt.eA_301,
                5 => EEventArt.eA_302,
                _ => _Iter,
            };
            Loe = Vergl(_Iter, P1, P2);
            if (Loe != 100)
            {
                if (Loe == 0)
                {
                    dB_EventTable.Seek("=", _Iter.AsString(), P1.AsString());
                    dB_EventTable.Delete();
                }
                else
                {
                    if (Loe > 1)
                    {
                        dB_EventTable.Seek("=", _Iter.AsString(), P2.AsString());
                        dB_EventTable.Edit();
                        if (Modul1.Kont[10] != "")
                        {
                            dB_EventTable.Fields[EventFields.Bem1].Value = dB_EventTable.Fields[EventFields.Bem1].AsString() != ""
                                ? (dB_EventTable.Fields[EventFields.Bem1].AsString() + " *** " + Modul1.Kont[10])
                                : Modul1.Kont[10];
                        }
                        if (Modul1.Person.Birthday != "")
                        {
                            dB_EventTable.Fields[EventFields.Bem2].Value = dB_EventTable.Fields[EventFields.Bem2].AsString() != ""
                                ? (dB_EventTable.Fields[EventFields.Bem2].AsString() + " *** " + Modul1.Person.Birthday)
                                : Modul1.Kont[10];
                        }
                        if (Modul1.Kont[16] != "")
                        {
                            if (null != dB_EventTable.Fields[EventFields.Bem3].AsString())
                            {
                                dB_EventTable.Fields[EventFields.Bem3].Value = dB_EventTable.Fields[EventFields.Bem3].AsString() != ""
                                    ? (dB_EventTable.Fields[EventFields.Bem3].AsString() + " *** " + Modul1.Kont[16])
                                    : Modul1.Kont[16];
                            }
                        }
                        if (Modul1.Kont[17] != "")
                        {
                            if (null != dB_EventTable.Fields[EventFields.Bem4].AsString())
                            {
                                dB_EventTable.Fields[EventFields.Bem4].Value = dB_EventTable.Fields[EventFields.Bem4].AsString() != ""
                                    ? (dB_EventTable.Fields[EventFields.Bem4].AsString() + " *** " + Modul1.Kont[17])
                                    : Modul1.Kont[17];
                            }
                        }
                        dB_EventTable.Update();
                        dB_EventTable.Seek("=", _Iter.AsString(), P1.AsString());
                        dB_EventTable.Delete();
                    }
                    else if (Loe != 1)
                    {
                    }

                }
            }

            num4 = 0;
            dB_EventTable.Index = nameof(EventIndex.BeSu);
            dB_EventTable.Seek("=", _Iter.AsString(), P2.AsString());
            while (!dB_EventTable.EOF
                && !dB_EventTable.NoMatch
                && dB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() == _Iter
                    && dB_EventTable.Fields[EventFields.PerFamNr].AsInt() == P2)
            {
                if (dB_EventTable.Fields[EventFields.LfNr].AsInt() > num4)
                {
                    num4 = dB_EventTable.Fields[EventFields.LfNr].AsInt().AsInt();
                }
                dB_EventTable.MoveNext();

            }
            dB_EventTable.Index = nameof(EventIndex.BeSu);
            dB_EventTable.Seek("=", _Iter.AsString(), Modul1.PersInArb.AsString());
            while (!dB_EventTable.EOF
                && !dB_EventTable.NoMatch
                && dB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() == _Iter
                && dB_EventTable.Fields[EventFields.PerFamNr].AsInt() == Modul1.PersInArb)
            {
                num4 = (short)(num4 + 1);
                SourceLink_Ref_Replace(dB_EventTable.Fields[EventFields.LfNr].AsInt(), P2, _Iter, num4);
                dB_EventTable.Edit();
                dB_EventTable.Fields[EventFields.LfNr].Value = num4;
                dB_EventTable.Fields[EventFields.PerFamNr].Value = P2;
                dB_EventTable.Update();
                dB_EventTable.MoveNext();
            }

            num8 = (short)unchecked(num8 + 1);
        }



    }

    private void SourceLink_Ref_Replace(int P1_, int P2, EEventArt eArt, int iLfdNr)
    {
        DataModul.DB_SourceLinkTable.Index = "Tab22";
        DataModul.DB_SourceLinkTable.Seek("=", 3, Modul1.PersInArb, eArt, P1_);
        if (!DataModul.DB_SourceLinkTable.NoMatch)
        {
            DataModul.DB_SourceLinkTable.Edit();
            DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].Value = P2;
            DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.LfNr].Value = iLfdNr;
            DataModul.DB_SourceLinkTable.Update();
        }
    }

    public void Namen(int P1, int P2)
    {
        short num = 0;
        DataModul.DB_PersonTable.Seek("=", P1);

        string Person_P1_sBem1 = DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString();
        if ("" == DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString())
        {
            DataModul.DB_PersonTable.Edit();
            DataModul.DB_PersonTable.Fields[PersonFields.Bem2].Value = " ";
            DataModul.DB_PersonTable.Update();
        }
        string Person_P1_sBem2 = DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString();
        if ("" == DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString())
        {
            DataModul.DB_PersonTable.Edit();
            DataModul.DB_PersonTable.Fields[PersonFields.Bem3].Value = " ";
            DataModul.DB_PersonTable.Update();
        }
        string Person_P1_sBem3 = DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString();

        if (Person_P1_sBem1.Trim() != "" | Person_P1_sBem3.Trim() != "" | Person_P1_sBem2.Trim() != "")
        {
            DataModul.DB_PersonTable.Seek("=", P2);
            if (Person_P1_sBem1.Trim() != "")
            {
                DataModul.DB_PersonTable.Edit();
                DataModul.DB_PersonTable.Fields[PersonFields.Bem1].Value =
                    DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString().Trim() == ""
                    ? Person_P1_sBem1
                    : (DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString() + "*** " + Person_P1_sBem1.Trim());
                DataModul.DB_PersonTable.Update();
            }
            if ("" == DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString())
            {
                DataModul.DB_PersonTable.Edit();
                DataModul.DB_PersonTable.Fields[PersonFields.Bem2].Value = " ";
                DataModul.DB_PersonTable.Update();
            }
            if (Person_P1_sBem2.Trim() != "")
            {
                DataModul.DB_PersonTable.Edit();
                DataModul.DB_PersonTable.Fields[PersonFields.Bem2].Value = Strings.Trim(DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString().AsString()) == ""
                    ? Person_P1_sBem2
                    : (DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString() + "*** " + Person_P1_sBem2.Trim());
                DataModul.DB_PersonTable.Update();
            }
            if (null == DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString())
            {
                DataModul.DB_PersonTable.Edit();
                DataModul.DB_PersonTable.Fields[PersonFields.Bem3].Value = " ";
                DataModul.DB_PersonTable.Update();
            }
            if (Person_P1_sBem3.Trim() != "")
            {
                DataModul.DB_PersonTable.Edit();
                DataModul.DB_PersonTable.Fields[PersonFields.Bem3].Value = Strings.Trim(DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString().AsString()) == ""
                    ? Person_P1_sBem3
                    : (DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString() + "*** " + Person_P1_sBem3.Trim());
                DataModul.DB_PersonTable.Update();
            }
        }
        Modul1.PersInArb = P1;
        Modul1.Person_ReadNames(P1, Modul1.Person);
        string text4 = (Modul1.Person.Prefix + " " + Modul1.Person.SurName + " " + Modul1.Person.Suffix).Trim();
        string text5 = Modul1.Person.Alias;

        Modul1.PersInArb = P2;
        Modul1.Person_ReadNames(P2, Modul1.Person);
        Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out int iAhn, out _);
        Modul1.Kont[97] = iAhn.AsString();

        string right = (Modul1.Person.Prefix + " " + Modul1.Person.SurName + " " + Modul1.Person.Suffix).Trim();
        string text6 = Modul1.Person.Alias;
        if (text6.Trim() != "" && text5 != text6)
        {
            num = 1;
            if (text6 != "")
            {
                text6 += ";";
            }
            text6 += text5;
        }
        if (text4 != right)
        {
            num = 1;
            if (text6 != "")
            {
                text6 += ";";
            }
            text6 += text4;
        }
        if (text5 != text6)
        {
            num = 1;
            if (text6 != "")
            {
                text6 += ";";
            }
            text6 += text5;
        }
        int Satz;
        if (num > 0)
        {
            Modul1.UbgT = text6;
            Modul1.eNKennz = ETextKennz.C_;
            Satz = DataModul.Texte_Schreib(text6, Modul1.UbgT1, ETextKennz.C_);
            DataModul_NameTable_SetData(Satz, Modul1.PersInArb, ETextKennz.C_);
        }

        string? text8 = "";
        string? text7;
        text7 = DataModul_Names_Get(P1, ETextKennz.U_);
        if (text7 == null)
        {
            return;
        }

        text8 = DataModul_Names_Get(P1, ETextKennz.U_);
        if (text8 != null)
        {
            DataModul.DB_NameTable.Delete();
        }
        if ((text8 + " " + text7).Trim() != "")
        {
            Modul1.UbgT = (text8 + " " + text7).Trim();
            ETextKennz eTKennz = ETextKennz.U_;
            Satz = DataModul.Texte_Schreib(Modul1.UbgT, Modul1.UbgT1, eTKennz);
            DataModul_Names_Append(P2, Satz, eTKennz);
        }
    }

    private static void DataModul_Names_Append(int P2, int Satz, ETextKennz eTKennz)
    {
        DataModul.DB_NameTable.AddNew();
        DataModul.DB_NameTable.Fields[NameFields.PersNr].Value = P2;
        DataModul.DB_NameTable.Fields[NameFields.Kennz].Value = eTKennz;
        DataModul.DB_NameTable.Fields[NameFields.Text].Value = Satz;
        DataModul.DB_NameTable.Fields[NameFields.LfNr].Value = 0;
        DataModul.DB_NameTable.Fields[NameFields.Ruf].Value = 0;
        DataModul.DB_NameTable.Update();
    }

    private string? DataModul_Names_Get(int persInArb, ETextKennz eTKennz)
    {
        string? text7 = null;
        DataModul.DB_NameTable.Index = nameof(NameIndex.NamKenn);
        DataModul.DB_NameTable.Seek("=", persInArb, (char)eTKennz);
        if (!DataModul.DB_NameTable.NoMatch)
        {
            text7 = DataModul.TextLese1(DataModul.DB_NameTable.Fields[NameFields.Text].AsInt());
        }
        return text7;
    }

    private void DataModul_NameTable_SetData(int Satz, int persInArb, ETextKennz eNKennz)
    {
        DataModul.DB_NameTable.Index = nameof(NameIndex.NamKenn);
        DataModul.DB_NameTable.Seek("=", persInArb, (char)eNKennz);
        if (!DataModul.DB_NameTable.NoMatch)
        {
            DataModul.DB_NameTable.Edit();
            DataModul.DB_NameTable.Fields[NameFields.Text].Value = Satz;
        }
        else
        {
            DataModul.DB_NameTable.AddNew();
            DataModul.DB_NameTable.Fields[NameFields.PersNr].Value = persInArb;
            DataModul.DB_NameTable.Fields[NameFields.Kennz].Value = eNKennz;
            DataModul.DB_NameTable.Fields[NameFields.Text].Value = Satz;
            DataModul.DB_NameTable.Fields[NameFields.LfNr].Value = 0;
            DataModul.DB_NameTable.Fields[NameFields.Ruf].Value = 0;
        }
        DataModul.DB_NameTable.Update();
    }

    public void Frag()
    {
        var M1_Iter = 0;
        var Modul1_Value = (float)Interaction.MsgBox("Änderungen speichern?", title: "", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Question);
        if (Modul1_Value == 1f)
        {
            var num5 = (int)Math.Round(Modul1_Dub_Trans1);
            M1_Iter = 1;
            while (M1_Iter <= num5)
            {
                DataModul.wrkDefault.Commit();
                M1_Iter++;
            }
        }
        else
        {
            var num7 = (int)Math.Round(Modul1_Dub_Trans1);
            M1_Iter = 1;
            while (M1_Iter <= num7)
            {
                DataModul.wrkDefault.Rollback();
                M1_Iter++;
            }
        }
        Modul1_Dub_Trans1 = 0f;
        Modul1.Trans = 0;
    }

    public void Listneu()
    {
        Modul1_Temp[0] = View.TextBox1.Text;
        View.TextBox1.Text = "";
        View.TextBox1.Text = Modul1_Temp[0];
        View._Frame1_2.Visible = false;
        View._Frame1_0.Visible = true;
        View._Command1_5.BackColor = View._Command1_3.BackColor;
        Modul1_Temp[1] = View.TextBox2.Text;
        View.TextBox2.Text = "";
        View.TextBox2.Text = Modul1_Temp[1];
        var M1_Iter = 0;
        checked
        {

            int i;
            int num;
            do
            {
                View.ALabel1[(short)M1_Iter].Text = "";
                View.ALabel1[(short)M1_Iter].Tag = 0;
                View.ALabel1[(short)M1_Iter].BackColor = ColorTranslator.FromOle(16777215);
                M1_Iter++;
                i = M1_Iter;
                num = 18;
            }
            while (i <= num);
            View._Frame1_2.Visible = false;
            View._Frame1_0.Visible = true;
            View._Command1_5.BackColor = View._Command1_3.BackColor;
            M1_Iter = 20;
            int i2;
            do
            {
                View.ALabel1[(short)M1_Iter].Text = "";
                View.ALabel1[(short)M1_Iter].Tag = 0;
                View.ALabel1[(short)M1_Iter].BackColor = ColorTranslator.FromOle(16777215);
                M1_Iter++;
                i2 = M1_Iter;
                num = 38;
            }
            while (i2 <= num);
            View._Frame1_3.Visible = false;
            View._Frame1_1.Visible = true;
            View._Command1_7.BackColor = View._Command1_3.BackColor;
            View._Command1_1.Visible = false;
            View._Command1_2.Visible = false;
            View._Command1_5.Visible = false;
            View._Command1_7.Visible = false;
            if (View.TextBox1.Text == "" & View.Button4.Enabled)
            {
                View.Button4.PerformClick();
            }
        }
    }

    public void Tausch(int P1, int F1, int P2, int F2)
    {
        //Discarded unreachable code: IL_1dfa, IL_2360
        int try0001_dispatch = -1;
        int num2 = default;
        int num11 = default;
        int num12 = default;
        int num6;
        switch (try0001_dispatch)
        {
            default:
                ProjectData.ClearProjectError();
                int num3 = 2;
                Namen(P1, P2);

                if (DataModul.Link.ExistE(P2, ELinkKennz.lkChild))
                {
                    _ = DataModul.Link.DeleteAllE(P1, ELinkKennz.lkChild);
                }

                foreach (var link in DataModul.Link.ReadAllPers(P1))
                {
                    link.SetPers(P2);
                }

                int M1_Iter = 4;
                while (M1_Iter <= 9)
                {
                    Modul1.eLKennz = M1_Iter.AsEnum<ELinkKennz>();
                    foreach (var link in DataModul.Link.ReadAllFams(P1, Modul1.eLKennz))
                    {
                        link.SetFam(P2);
                    }
                    M1_Iter++;
                }

                Witnes_Ref_Replace1(P1, P2);

                Witness_Ref_Replace2(P1, P2);
                Sourcelink_ref_Replace(P1, P2);

                Picture_Ref_Replace(P1, P2);

                Modul1.PersInArb = P1;

                Ertausch(ref P1, ref P2);
                DataModul.Person_Sichlöschloesch(Modul1.PersInArb);
                View._Label1_41.Text = "Beruf:";
                View._Label1_43.Text = "Beruf:";
                View._Label1_42.Text = "Wohnort:";
                View._Label1_44.Text = "Wohnort:";
                if (ColorTranslator.ToOle(View._Label1_0.BackColor) != 8454143)
                {
                    View._Command1_1.Visible = false;
                    View._Command1_2.Visible = false;
                    short num7 = 1;
                    while (num7 <= 2)
                    {
                        short num4;
                        if (num7 == 1)
                        {
                            M1_Iter = 0;
                            while (M1_Iter <= 18)
                            {
                                View.ALabel1[(short)M1_Iter].Text = "";
                                View.ALabel1[(short)M1_Iter].Tag = 0;
                                View.ALabel1[(short)M1_Iter].BackColor = ColorTranslator.FromOle(8454143);
                                M1_Iter++;
                            }
                            M1_Iter = 41;
                            while (M1_Iter <= 42)
                            {
                                View.ALabel1[(short)M1_Iter].BackColor = ColorTranslator.FromOle(8454143);
                                M1_Iter++;
                            }
                            num4 = 0;
                            Modul1.PersInArb = F1;
                        }
                        else
                        {
                            M1_Iter = 20;
                            while (M1_Iter <= 38)
                            {
                                View.ALabel1[(short)M1_Iter].Text = "";
                                View.ALabel1[(short)M1_Iter].Tag = 0;
                                View.ALabel1[(short)M1_Iter].BackColor = ColorTranslator.FromOle(8454143);
                                M1_Iter++;
                            }
                            M1_Iter = 43;
                            while (M1_Iter <= 44)
                            {
                                View.ALabel1[(short)M1_Iter].BackColor = ColorTranslator.FromOle(8454143);
                                M1_Iter++;
                            }
                            num4 = 1;
                            Modul1.PersInArb = F2;
                        }
                        if (Modul1.PersInArb > 0)
                        {
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out int iAhn, out _);
                            Modul1.Kont[97] = iAhn.AsString();
                            GetALabel1_2d(num4, 0).Text = $"{Modul1.PersInArb,20}{DataModul.Person.GetSex(Modul1.PersInArb)}";
                            GetALabel1_2d(num4, 0).Tag = Modul1.PersInArb;
                            Modul1.Person.SetFullSurname(Modul1.Person.SurName);
                            if (Modul1.Person.Prefix != "")
                            {
                                Modul1.Person.SetFullSurname(Modul1.Person.Prefix + " " + Modul1.Person.FullSurName);
                            }
                            if (Modul1.Person.Suffix != "")
                            {
                                Modul1.Person.SetFullSurname(Modul1.Person.FullSurName + " " + Modul1.Person.Suffix);
                            }
                            GetALabel1_2d(num4, 1).Text = Modul1.Person.Givennames + " " + Modul1.Person.FullSurName;
                            Modul1.Datles(Modul1.PersInArb, Modul1.Person);
                            M1_Iter = 1;
                            while (M1_Iter <= 4)
                            {
                                if (Modul1.Kont[M1_Iter + 15] != "")
                                {
                                    Modul1.Kont[M1_Iter + 10] = "B" + Modul1.Kont[M1_Iter + 10];
                                }
                                M1_Iter++;
                            }
                            GetALabel1_2d(num4, 2).Text = Modul1.IText[EUserText.tBirthdaySh] + " " + Modul1.Person.Birthday;
                            GetALabel1_2d(num4, 3).Text = Modul1.IText[EUserText.tBaptismSh] + " " + Modul1.Person.Baptised;
                            GetALabel1_2d(num4, 4).Text = Modul1.IText[EUserText.tDeathSh] + " " + Modul1.Person.Death;
                            GetALabel1_2d(num4, 5).Text = Modul1.IText[EUserText.tBurialSh] + " " + Modul1.Person.Burial;

                            if (DataModul.Event.ReadData(EEventArt.eA_300, Modul1.PersInArb, out var cEv))
                            {
                                if (cEv.iKBem > 0)
                                {
                                    if (num7 == 2)
                                    {
                                        View._Label1_41.Text = $"{Modul1.IText[EUserText.tOccupation]} {cEv.sKBem.Trim()}";
                                    }
                                    if (num7 == 1)
                                    {
                                        View._Label1_43.Text = $"{Modul1.IText[EUserText.tOccupation]} {cEv.sKBem.Trim()}";
                                    }
                                }

                            }
                            if (num7 == 1)
                            {
                                View._Label1_44.Text = Modul1.IText[EUserText.tResidence];
                            }
                            if (num7 == 2)
                            {
                                View._Label1_42.Text = Modul1.IText[EUserText.tResidence];
                            }
                            if (!DataModul.Event.ReadData(EEventArt.eA_302, Modul1.PersInArb, out var cEvt))
                            {
                                Modul1.UbgT = "";
                                if (cEvt.iOrt > 0 && DataModul.Place.ReadData(cEvt.iOrt, out var cPlace))
                                {
                                    Modul1.UbgT = DataModul.Place.FullName(cPlace, true, true);
                                }
                                if (num7 == 2)
                                {
                                    View._Label1_42.Text = "Wohnort: " + Modul1.UbgT.Trim();
                                }
                                if (num7 == 1)
                                {
                                    View._Label1_44.Text = "Wohnort: " + Modul1.UbgT.Trim();
                                }
                            }
                            var ubg = Modul1.Eltsuch(Modul1.PersInArb);
                            GetALabel1_2d(num4, 6).Text = "Eltern Ehe Nr.:             ".Left(15) + ubg.AsString();
                            GetALabel1_2d(num4, 6).Tag = ubg;
                            Modul1.PersInArb = View.ALabel1[(short)(num4 * 20)].Tag.AsInt();
                            Modul1.eLKennz = DataModul.Person.GetSex(Modul1.PersInArb) == "F" ? ELinkKennz.lkMother : ELinkKennz.lkFather;

                            var aiFam = new List<int>();
                            foreach (var link in DataModul.Link.ReadAllPers(Modul1.PersInArb, Modul1.eLKennz))
                            {
                                aiFam.Add(link.iFamNr);
                                if (aiFam.Count > 99) break;
                            }
                            if (aiFam.Count == 0)
                            {
                                goto end_IL_0001_2;
                            }
                            GetALabel1_2d(num4, 10).Text = "Eigene Ehe Nr.: " + aiFam[0].AsString();
                            GetALabel1_2d(num4, 10).Tag = aiFam[0];
                            short num5 = 0;
                            int num13 = aiFam.Count;
                            M1_Iter = 1;
                            while (M1_Iter <= aiFam.Count && M1_Iter <= 4)
                            {
                                Modul1.FamInArb = aiFam[M1_Iter];
                                var s = FamDat(aiFam[M1_Iter]);
                                if (s != "")
                                {
                                    GetALabel1_2d(num4, 10 + M1_Iter + num5).Text = s;
                                    GetALabel1_2d(num4, 10 + M1_Iter + num5).Tag = aiFam[M1_Iter];
                                    num5++;
                                }
                                DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                                Modul1.PersInArb = Modul1.eLKennz.AsDouble() == 1.0 ? Modul1.Family.Frau : Modul1.Family.Mann;
                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out iAhn, out _);
                                Modul1.Kont[97] = iAhn.AsString();
                                if (Modul1.Person.Prefix != "")
                                {
                                    Modul1.Person.SetFullSurname(Modul1.Person.Prefix + " " + Modul1.Person.FullSurName);
                                }
                                Modul1.Person.SetFullSurname(Modul1.Person.SurName);
                                if (Modul1.Person.Suffix != "")
                                {
                                    Modul1.Person.SetFullSurname(Modul1.Person.FullSurName + " " + Modul1.Person.Suffix);
                                }
                                if (M1_Iter < 5)
                                {
                                    GetALabel1_2d(num4, 10 + M1_Iter + num5).Text = Modul1.PersInArb.AsString().Trim() + " " + Modul1.Person.Givennames + " " + Modul1.Person.FullSurName;
                                }
                                M1_Iter++;
                            }
                            if (View._Frame1_2.Visible & View._Frame1_3.Visible)
                            {
                                if (Strings.Mid(View._Label1_0.Text, 20, 10) == Strings.Mid(View._Label1_20.Text, 20, 10)
                                        && View._Label1_0.Text.Left(20) != View._Label1_20.Text.Left(20))
                                {
                                    if (View._Label1_6.Tag.AsInt() == View._Label1_26.Tag.AsInt()
                                      | View._Label1_6.Tag.AsInt() == 0
                                      | View._Label1_26.Tag.AsInt() == 0)
                                    {
                                        View._Command1_1.Visible = true;
                                        View._Command1_2.Visible = true;
                                        View._Command1_1.BackColor = View._Command1_3.BackColor;
                                        View._Command1_2.BackColor = View._Command1_3.BackColor;
                                        View._Command1_1.Text = $"Person löschen und alle Daten und Verknüpfungen nach Person {View._Label1_20.Tag.AsInt()} übertragen.\n(Ausnahme: Vornamen)";
                                        View._Command1_2.Text = $"Person löschen und alle Daten und Verknüpfungen nach Person {View._Label1_0.Tag.AsInt()} übertragen.\n(Ausnahme: Vornamen)";
                                    }
                                    else
                                    {
                                        View._Command1_1.Visible = true;
                                        View._Command1_2.Visible = true;
                                        View._Command1_1.BackColor = Color.Red;
                                        View._Command1_2.BackColor = Color.Red;
                                        View._Command1_1.Text = $"Person löschen und alle Daten und Verknüpfungen nach Person {View._Label1_20.Tag.AsInt()} übertragen.\n(Ausnahme: Vornamen)\nDiese Eltern-Kind-Verbindung wird gelöscht";
                                        View._Command1_2.Text = $"Person löschen und alle Daten und Verknüpfungen nach Person {View._Label1_0.Tag.AsInt()} übertragen.\n(Ausnahme: Vornamen)\nDiese Eltern-Kind-Verbindung wird gelöscht";
                                    }
                                }
                            }
                        }
                        num7++;
                    }
                }
                else
                {
                    View._Command1_1.Visible = false;
                    View._Command1_2.Visible = false;
                    num11 = View._Label1_10.Tag.AsInt();
                    num12 = View._Label1_30.Tag.AsInt();
                    M1_Iter = 0;
                    while (M1_Iter <= 18)
                    {
                        View.ALabel1[(short)M1_Iter].Text = "";
                        View.ALabel1[(short)M1_Iter].Tag = 0;
                        View.ALabel1[(short)M1_Iter].BackColor = ColorTranslator.FromOle(16777152);
                        M1_Iter++;
                    }
                    M1_Iter = 20;
                    while (M1_Iter <= 38)
                    {
                        View.ALabel1[(short)M1_Iter].Text = "";
                        View.ALabel1[(short)M1_Iter].Tag = 0;
                        View.ALabel1[(short)M1_Iter].BackColor = ColorTranslator.FromOle(16777152);
                        M1_Iter++;
                    }
                    M1_Iter = 41;
                    while (M1_Iter <= 44)
                    {
                        View.ALabel1[(short)M1_Iter].Text = "";
                        View.ALabel1[(short)M1_Iter].Tag = 0;
                        View.ALabel1[(short)M1_Iter].BackColor = ColorTranslator.FromOle(16777152);
                        M1_Iter++;
                    }
                    View._Label1_0.Text = num11.AsString();
                    View._Label1_0.Tag = num11;
                    View._Label1_20.Text = num12.AsString();
                    View._Label1_20.Tag = num12;
                    Modul1.FamInArb = num11;
                    //  Modul1.o01_Person.Prefix = "";
                    var s = FamDat(Modul1.FamInArb);
                    if (s != "")
                    {
                        View._Label1_1.Text = s;
                    }
                    Modul1.FamInArb = num12;
                    // Modul1.o01_Person.Prefix = "";
                    s = FamDat(Modul1.FamInArb);
                    if (s != "")
                    {
                        View._Label1_21.Text = s;
                    }
                    if (View._Label1_0.Tag.AsInt() == View._Label1_20.Tag.AsInt())
                    {
                        goto end_IL_0001_2;
                    }
                    if (View._Label1_1.Text.Trim() != "")
                    {
                        View._Command1_5.BackColor = Color.Red;
                    }
                    if (View._Label1_21.Text.Trim() != "")
                    {
                        View._Command1_7.BackColor = Color.Red;
                    }
                    View._Frame1_3.Text = "Familie";
                    View._Frame1_2.Text = "Familie";
                    View.Button2.Visible = true;
                    View.Button3.Visible = true;
                    View._Command1_9.Visible = false;
                    View._Command1_10.Visible = false;
                    View._Command1_5.Visible = true;
                    View._Command1_7.Visible = true;
                    View._Command1_5.Text = $"Familie {View._Label1_0.Tag} löschen";
                    View._Command1_7.Text = $"Familie {View._Label1_20.Tag} löschen";
                }
                goto end_IL_0001_2;

            IL_23f3:
                num6 = unchecked(num2 + 1);
                goto IL_23f7;
            IL_23f7:
                num2 = 0;
                switch (num6)
                {
                    case 1:
                        break;

                    case 193:
                    case 218:
                    case 242:
                    case 310:
                    case 358:
                    case 359:
                    case 360:
                    case 369:
                        goto end_IL_0001_2;
                }
                goto default;
        }
    end_IL_0001_2: // <========== 7
        return;
    }

    private void Witnes_Ref_Replace1(int P1, int P2)
    {
        Modul1.eWKennz = 10;
        IRecordset dB_WitnessTable = DataModul.DB_WitnessTable;
        dB_WitnessTable.Index = nameof(WitnessIndex.FamSu);
        dB_WitnessTable.Seek("=", P1, Modul1.eWKennz);
        while (!dB_WitnessTable.EOF
            && !dB_WitnessTable.NoMatch
            && !dB_WitnessTable.EOF
            && !(dB_WitnessTable.Fields[WitnessFields.FamNr].AsInt() != P1))
        {
            dB_WitnessTable.Edit();
            dB_WitnessTable.Fields[WitnessFields.FamNr].Value = P2;
            dB_WitnessTable.Update();
            dB_WitnessTable.MoveNext();
        }
    }

    private void Witness_Ref_Replace2(int P1, int P2)
    {
        Modul1.eWKennz = 10;
        IRecordset dB_WitnessTable = DataModul.DB_WitnessTable;
        dB_WitnessTable.Index = nameof(WitnessIndex.ElSu);
        dB_WitnessTable.Seek("=", P1, Modul1.eWKennz);
        while (!dB_WitnessTable.EOF && !dB_WitnessTable.NoMatch && !dB_WitnessTable.EOF && !(dB_WitnessTable.Fields[WitnessFields.PerNr].AsInt() != P1))
        {
            dB_WitnessTable.Edit();
            dB_WitnessTable.Fields[WitnessFields.PerNr].Value = P2;
            dB_WitnessTable.Update();
            dB_WitnessTable.MoveNext();
        }
    }

    private void Sourcelink_ref_Replace(int P1, int P2)
    {
        Modul1.Qkenn = 1;
        IRecordset dB_SourceLinkTable = DataModul.DB_SourceLinkTable;
        dB_SourceLinkTable.Index = "Tab";
        dB_SourceLinkTable.Seek("=", Modul1.Qkenn, P1);
        while (!dB_SourceLinkTable.EOF
            && !dB_SourceLinkTable.NoMatch
            && dB_SourceLinkTable.Fields[0].AsInt() == Modul1.Qkenn
            && dB_SourceLinkTable.Fields[1].AsInt() <= P1)
        {
            dB_SourceLinkTable.Edit();
            dB_SourceLinkTable.Fields[SourceLinkFields._2].Value = P2;
            dB_SourceLinkTable.Update();
            dB_SourceLinkTable.MoveNext();
        }
    }

    private static void Picture_Ref_Replace(int P1, int P2)
    {
        IRecordset dB_PictureTable = DataModul.DB_PictureTable;
        dB_PictureTable.Index = nameof(PictureIndex.PerKenn);
        dB_PictureTable.Seek("=", "P", P1);
        while (!dB_PictureTable.EOF && !dB_PictureTable.NoMatch && !(dB_PictureTable.Fields[PictureFields.ZuNr].AsInt() != P1))
        {
            dB_PictureTable.Edit();
            dB_PictureTable.Fields[PictureFields.ZuNr].Value = P2;
            dB_PictureTable.Update();
            dB_PictureTable.MoveNext();

        }
    }

    public void Zeig(ref short Index)
    {
        ELinkKennz kennz = default;

        ProjectData.ClearProjectError();
        int num5 = 0;
        var aiFam = new List<int>();
        while (!DataModul.DSB_SearchTable.EOF &&
            View.List1[Index].Items.Count <= Modul1.Aus[12].AsInt())
        {
            num5 = checked(num5 + 1);
            int Search_iNummer = DataModul.DSB_SearchTable.Fields["Nummer"].AsInt();
            string Search_iKenn = DataModul.DSB_SearchTable.Fields["iKenn"].AsString();
            DateTime Search_dDatum = DataModul.DSB_SearchTable.Fields["Datum"].AsDate();
            string Search_sName = DataModul.DSB_SearchTable.Fields["Name"].AsString();
            string Search_sSich = DataModul.DSB_SearchTable.Fields["Sich"].AsString();

            int Search_iPersNr = Search_iNummer;
            if (Search_iPersNr != 0)
            {

                string Person_sSex = DataModul.Person.GetSex(Search_iPersNr);

                if (!View.Option1[1].Checked && !View.Option1[2].Checked
                    || !View.Option1[2].Checked && Person_sSex == "F"
                    || !View.Option1[1].Checked && Person_sSex == "M")
                {
                    string text2 = Search_dDatum.Year == 0 ? "       " : $"{Search_iKenn,2}{Search_dDatum.Year,-4}";

                    if (DataModul.DSB_SearchTable.EOF)
                    {
                        break;
                    }

                    if (Search_iKenn.AsInt() == 9)
                    {
                        DataModul.DSB_SearchTable.Edit();
                        DataModul.DSB_SearchTable.Fields["iKenn"].Value = Search_iKenn = "U";
                        DataModul.DSB_SearchTable.Update();
                    }

                    text2 = Search_dDatum.Year == 0 ? "       " : $"{Search_iKenn,2}{Search_dDatum.Year,-4}{Search_sSich}";

                    string text = $"{Search_sName,40}";
                    if (View.CheckBox1.Checked)
                    {
                        // Modul1.eLKennz
                        kennz = Person_sSex.ToUpper() == "M" ? ELinkKennz.lkFather : ELinkKennz.lkMother;
                        if (DataModul.Link.GetPersonFam(Search_iPersNr, kennz, out int iFamNr))
                        {
                            aiFam.Add(iFamNr);
                            DataModul.Link.ReadFamily(iFamNr, Modul1.Family);
                            Search_iPersNr = kennz switch
                            {
                                ELinkKennz.lkFather => Modul1.Family.Frau,
                                ELinkKennz.lkMother => Modul1.Family.Mann,
                                _ => Search_iPersNr,
                            };

                            if (Search_iPersNr > 0)
                            {
                                Modul1.Person_ReadNames(Search_iPersNr, Modul1.Person);
                                //Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.o01_Person.iPerFamNr, out int iAhn, out Modul1.Kont[20]);
                                //Modul1.Kont[97] = iAhn.AsString();
                                text = $"{Search_sName,30} /{Modul1.Person.SurName,10}".Left(40);
                            }
                        }
                    }
                    if (View.CheckBox2.Checked)
                    {
                        Modul1.eLKennz = ELinkKennz.lkChild;
                        if (DataModul.Link.GetPersonFam(Search_iPersNr, Modul1.eLKennz, out var iChildFam))
                        {
                            Modul1.FamInArb = iChildFam;
                            aiFam.Add(iChildFam);
                            DataModul.Link.ReadFamily(iChildFam, Modul1.Family);
                            if (Modul1.Family.Mann > 0)
                            {
                                Modul1.Person_ReadNames(Modul1.Family.Mann, Modul1.Person);
                                //Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.o04_Family.Mann, out int iAhn, out Modul1.Kont[20]);
                                //Modul1.Kont[97] = iAhn.AsString();
                                text = $"{Search_sName,20} /{Modul1.Person.SurName,9} /".Left(31);
                            }
                            if (Modul1.Family.Frau > 0)
                            {
                                Modul1.Person_ReadNames(Modul1.Family.Frau, Modul1.Person);
                                //Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.o04_Family.Modul1.Family.Frau, out var iAhn, out Modul1.Kont[20]);
                                //Modul1.Kont[97] = iAhn.AsString();
                                text = $"{text.Left(30)}{Modul1.Person.SurName}".Left(40);
                            }
                            else
                                text = $"{text.Left(31)}{"",9}";
                        }
                    }

                    _ = View.List1[Index].Items.Add(new ListItem($"{text.Left(40),40}{text2}{Search_iPersNr,-10}", Search_iPersNr));
                }
            }
            else
            {
                num5--;
            }
            DataModul.DSB_SearchTable.MoveNext();
        }

        goto end_IL_0001_2;

    end_IL_0001_2: // <========== 5
        return;
    }
    public void ZeigNaNum(ref short Index)
    {
        string text = "";
        string item = "";
        string right = "";
        ProjectData.ClearProjectError();
        View.Cursor = Cursors.WaitCursor;
        View.List1[Index].Items.Clear();
        int num5 = 0;
        DataModul.DB_PersonTable.Index = nameof(PersonIndex.Puid);
        DataModul.DB_PersonTable.MoveFirst();

        while (!DataModul.DB_PersonTable.EOF)
        {
            num5 = checked(num5 + 1);
            Modul1.PersInArb = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
            if (Modul1.PersInArb == 0)
            {
                num5 = checked(num5 - 1);
            }
            else
            {
                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out int iAhn, out _);
                Modul1.Kont[97] = iAhn.AsString();
                string text2 = Modul1.Person.SurName + ", " + Modul1.Person.Givennames;
                if (DataModul.DB_PersonTable.Fields[PersonFields.PUid].AsGUID() == right.AsGUID())
                {
                    _ = View.List1[Index].Items.Add(item);
                    item = "";
                    _ = View.List1[Index].Items.Add(text2 + "                                                ".Left(40) + text + "           " + Modul1.PersInArb.AsString().Right(10));
                    text2 = "";
                }
                item = text2 + "                                                ".Left(40) + text + "           " + Modul1.PersInArb.AsString().Right(10);
                right = DataModul.DB_PersonTable.Fields[PersonFields.PUid].AsString();
                if (View.List1[Index].Items.Count > Modul1.Aus[12].AsDouble())
                {

                    break;
                }
            }

            DataModul.DB_PersonTable.MoveNext();
        }

        View.Cursor = Cursors.Arrow;
        if (View.List1[Index].Items.Count == 0)
            _ = View.List1[Index].Items.Add("Keine an Hand der UID erkennbaren Dubletten vorhanden.");


        return;
    }
    public string FamDat(int famInArb)
    {
        //Discarded unreachable code: IL_033c
        DataModul.DB_EventTable.Index = nameof(EventIndex.ArtNr);
        EEventArt num = EEventArt.eA_500;

        IEventData? cEvt = null;
        while (num <= EEventArt.eA_504)
        {
            cEvt = DataModul.Event.ReadDataPl(num, famInArb, out var xBreak);
            if (!xBreak)
                break;

            num++;
        }
        if (cEvt == null) return "";

        if (cEvt.dDatumV != default)
        {
            Modul1.Kont1[1] = cEvt.dDatumV.ToString();
        }
        string sOrt = "";
        if (cEvt.iOrt > 0 && DataModul.Place.ReadData(cEvt.iOrt, out var cPlace))
        {
            sOrt = DataModul.Place.FullName(cPlace, true, true);
        }
        Modul1.Kont1[4] = cEvt.sDatumB_S;


        string text = Modul1.Event_PreDisplay(xCitation: cEvt.sBem[3].TrimEnd() != "", xWitness: cEvt.sBem[4].TrimEnd() != "", xAnnotation: cEvt.sBem[1].TrimEnd() != ""
            || cEvt.sBem[2].TrimEnd() != "");

        return text + Modul1.Kont1[1] + " " + sOrt;
    }

    public void Famweg(int Fam1, int Fam2)
    {
        //Discarded unreachable code: IL_2278
        string? text = default;
        short num6 = default;
        int num7 = default;
        int num8 = default;
        EEventArt eArt = default;

        ProjectData.ClearProjectError();
        Pictures_UpdateReplaceAllFam(Fam1, Fam2);

        var M1_Iter_ = 1;
        foreach (var link in DataModul.Link.ReadAllFams(Fam1))
        {
            if (link.eKennz == ELinkKennz.lkChild
                || link.eKennz == ELinkKennz.lkAdoptedChild)
            {
                link.SetFam(Fam2);
            }
            else if (link.eKennz == ELinkKennz.lkFather
                || link.eKennz == ELinkKennz.lkMother)
            {
                link.Delete();
            }

            M1_Iter_++;
        }

        SourceLink_UpdateReplAll(2, Fam1, Fam2);

        DataModul.Family_DeleteA(Fam1, FamilyFields.Bem3, (o) => text = o != null ? o.AsString() : text);

        if (!string.IsNullOrWhiteSpace(text))
        {
            DataModul.DB_FamilyTable.Seek("=", Fam2);
            DataModul.DB_FamilyTable.Edit();
            if (Strings.Trim(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].AsString()) == "")
            {
                DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].Value = text!;
            }
            else
            {
                DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].Value = DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].Value + "*** " + text!.Trim();
                text = "";
            }
            DataModul.DB_FamilyTable.Update();
        }

        View._Command1_5.Visible = false;
        View._Command1_7.Visible = false;
        eArt = EEventArt.eA_500;
        while (eArt <= EEventArt.eA_601)
        {
            if (eArt > EEventArt.eA_507)
            {
                eArt = EEventArt.eA_601;
            }
            if (DataModul.Event.ReadData((eArt, Fam1, 0), out var cEv))
            {
                if (DataModul.Event.Exists(eArt, Fam2, 0))
                {
                    var eArt2 = EEventArt.eA_603;
                    num6 = Event_MaxLfdNr(Fam2, eArt2);
                    if (num6 == -1)
                        break;
                    else
                        num6++;

                    SourceLink_UpdateData(3, Fam1, eArt, Fam2, num6, eArt2);

                    DataModul.Witness.UpdateAllReplFams(Fam1, Fam2, num6, eArt2);

                    var sArtText = eArt switch
                    {
                        EEventArt.eA_500 => "anderes Proklamationsdatum",
                        EEventArt.eA_501 => "anderes Verlobungsdatum",
                        EEventArt.eA_Marriage => "anderes Heiratsdatum",
                        EEventArt.eA_MarrReligious => "anderes kirchl. Heiratsdatum",
                        EEventArt.eA_504 => "anderes Scheidungsdatum",
                        EEventArt.eA_505 => "anderes Datum eheähnl. Beziehung",
                        EEventArt.eA_507 => "anderes Dimitierungsdatum",
                        EEventArt.eA_601 => "anderes fikt. Heiratsdatum",
                        _ => "",
                    };
                    var iArtText = Modul1.TextSpeich(sArtText, "", ETextKennz.M_, Fam2, num6);
                    cEv!.SetPropValue(EEventProp.eArt, eArt2);
                    cEv.SetPropValue(EEventProp.iArtText, iArtText);
                    cEv.SetPropValue(EEventProp.iLfNr, num6);
                    cEv.SetPropValue(EEventProp.iPerFamNr, Fam2);
                    cEv.Update();
                }
                else
                {
                    SourceLink_UpdateData(3, Fam1, eArt, Fam2, 0, eArt);
                    DataModul.Witness.UpdateAllReplFams(Fam1, Fam2);
                    DataModul.Event.UpdateReplFams(Fam1, Fam2, eArt);
                }
            }
            eArt++;
        }
        eArt = EEventArt.eA_602;
        while (eArt <= EEventArt.eA_603)
        {
            num7 = 1;
            while (num7 <= 99)
            {
                if (DataModul.Event.ReadData((eArt, Fam1, 0), out var cEv))
                {
                    if (DataModul.Event.Exists(eArt, Fam2, 0))
                    {
                        num8 = Event_MaxLfdNr(Fam2, eArt);
                        if (num8 == -1)
                            break;
                        else
                            num8++;

                        cEv!.SetPropValue(EEventProp.eArt, eArt);
                        cEv.SetPropValue(EEventProp.iArtText, "");
                        cEv.SetPropValue(EEventProp.iLfNr, num8);
                        cEv.SetPropValue(EEventProp.iPerFamNr, Fam2);
                        cEv.Update();
                    }
                    else
                    {
                        DataModul.Event.UpdateReplFams(Fam1, Fam2, eArt);
                    }
                }
                num7++;
            }
            eArt++;
        }

        // Cleanup
        eArt = EEventArt.eA_500;
        while (eArt <= EEventArt.eA_510)
        {
            DataModul.Event.DeleteAll(eArt, Fam1);
            eArt++;
        }

        eArt = EEventArt.eA_601;
        while (eArt <= EEventArt.eA_603)
        {
            DataModul.Event.DeleteAll(eArt, Fam1);
            eArt++;
        }

        DataModul.Link.DeleteFamWhere(Fam1,
            (l) => l.eKennz is not (ELinkKennz.lk9 or ELinkKennz.lkGodparent));

        DataModul.SourceLink_DeleteAllWhere(Fam1, 2, (eA) => true);
        DataModul.SourceLink_DeleteAllWhere(Fam1, 3, (eA) => (int)eA > 499);

        DataModul.Witness.DeleteAllF(Fam1, 10);


        if (Modul1.Trans > 0)
        {
            Frag();
        }
        Listneu();
        goto end_IL_0001_2;
    end_IL_0001_2:
        return;
    }

    private static void Pictures_UpdateReplaceAllFam(int Fam1, int Fam2)
    {
        DataModul.DB_PictureTable.Index = nameof(PictureIndex.PerKenn);
        DataModul.DB_PictureTable.Seek("=", "F", Fam1);
        while (!DataModul.DB_PictureTable.EOF && !DataModul.DB_PictureTable.NoMatch && !(DataModul.DB_PictureTable.Fields[PictureFields.ZuNr].AsInt() != Fam1))
        {
            DataModul.DB_PictureTable.Edit();
            DataModul.DB_PictureTable.Fields[PictureFields.ZuNr].Value = Fam2;
            DataModul.DB_PictureTable.Update();
            DataModul.DB_PictureTable.MoveNext();
        }
    }

    private static void SourceLink_UpdateReplAll(int iLinkType, int Fam1, int Fam2)
    {
        DataModul.DB_SourceLinkTable.Index = "Tab";
        DataModul.DB_SourceLinkTable.Seek("=", iLinkType, Fam1);
        while (!DataModul.DB_SourceLinkTable.EOF
            && !DataModul.DB_SourceLinkTable.NoMatch
            && DataModul.DB_SourceLinkTable.Fields[0].AsInt() == iLinkType
            && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].AsInt() == Fam1)
        {
            DataModul.DB_SourceLinkTable.Edit();
            DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].Value = Fam2;
            DataModul.DB_SourceLinkTable.Update();
            DataModul.DB_SourceLinkTable.MoveNext();
        }
    }

    private static short Event_MaxLfdNr(int iFam, EEventArt eArt)
    {
        short num8;
        // Search max. LfdNr for Event
        IRecordset dB_EventTable = DataModul.DB_EventTable;
        dB_EventTable.Index = nameof(EventIndex.BeSu);
        dB_EventTable.Seek("=", eArt, iFam);
        num8 = 0;
        if (!dB_EventTable.EOF)
        {
            while (!dB_EventTable.EOF
                && !dB_EventTable.NoMatch
                && !(dB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != eArt)
                && !(dB_EventTable.Fields[EventFields.PerFamNr].AsInt() != iFam))
            {
                short Event_iLfdNr = (short)dB_EventTable.Fields[EventFields.LfNr].AsInt();
                if (Event_iLfdNr > num8)
                {
                    num8 = Event_iLfdNr;
                }
                dB_EventTable.MoveNext();
            }
            return num8;
        }
        else
            return -1;

    }
    private static void SourceLink_UpdateData(int iLinkType, int Fam1, EEventArt eArt, int iFam2, int iLfdNr2, EEventArt eArt2)
    {
        IRecordset dB_SourceLinkTable = DataModul.DB_SourceLinkTable;
        dB_SourceLinkTable.Index = "Tab22";
        dB_SourceLinkTable.Seek("=", iLinkType, Fam1, eArt, 0);
        if (!dB_SourceLinkTable.NoMatch)
        {
            dB_SourceLinkTable.Edit();
            dB_SourceLinkTable.Fields["2"].Value = iFam2;
            if (eArt2 != eArt)
                dB_SourceLinkTable.Fields["Art"].Value = eArt2;
            if (iLfdNr2 != 0)
                dB_SourceLinkTable.Fields["LfNr"].Value = iLfdNr2;
            dB_SourceLinkTable.Update();
        }
    }



    public void Button1_Click(object sender, EventArgs e)
    {
        if (View.TextBox1.Text != "")
        {
            DataModul.DSB_SearchTable.Index = "Persuch";
            DataModul.DSB_SearchTable.Seek(">=", View.TextBox1.Text, 0);
            View._List1_0.Items.Clear();
            short Index = 0;
            Zeig(ref Index);
        }
        if (View.TextBox2.Text != "")
        {
            DataModul.DSB_SearchTable.Index = "Persuch";
            DataModul.DSB_SearchTable.Seek(">=", View.TextBox2.Text, 0);
            View._List1_1.Items.Clear();
            short Index = 1;
            Zeig(ref Index);
        }
    }

    public void _Option1_0_CheckedChanged(object sender, EventArgs e)
    {
        View.Button1.PerformClick();
    }

    public void TextBox1_TextChanged(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_022a
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string name = default;
        int num4;
        short Index;
        switch (try0001_dispatch)
        {
            default:
                ProjectData.ClearProjectError();
                num3 = 2;
                //DataModul.DSB_SearchTable.Edit();
                //DataModul.DSB_SearchTable.Fields["iKenn"].Value = " ";
                //DataModul.DSB_SearchTable.Update();kd
                View.Button2.Visible = false;
                View.Button3.Visible = false;
                name = ((TextBox)sender).Name;
                if (name == View.TextBox1.Name)
                {
                    if (View.TextBox1.Text == "" & View.Button4.Enabled)
                    {
                        View.Button4.PerformClick();
                    }
                    else
                    {
                        DataModul.DSB_SearchTable.Index = "Persuch";
                        DataModul.DSB_SearchTable.Seek(">=", View.TextBox1.Text, 0);
                        View.List1[0].Items.Clear();
                        Index = 0;
                        Zeig(ref Index);
                    }
                }
                else
                {
                    if (name != View.TextBox2.Name)
                    {
                        goto end_IL_0001_2;
                    }
                    if (View.TextBox2.Text == "")
                    {
                        goto end_IL_0001_2;
                    }
                    DataModul.DSB_SearchTable.Index = "Persuch";
                    DataModul.DSB_SearchTable.Seek(">=", View.TextBox2.Text, 0);
                    View.List1[1].Items.Clear();
                    Index = 1;
                    Zeig(ref Index);
                }
                goto end_IL_0001_2;
        }
    end_IL_0001_2: // <========== 4
        return;
    }
    public byte Vergl(EEventArt eEvtArt, int P1, int P2)
    {
        byte Loe;
        if (DataModul.Event.ReadData(eEvtArt, P1, out var cEv1))
        {
            if (DataModul.Event.ReadData(eEvtArt, P2, out var cEv2))
            {
                Loe = (byte)Event_Compare(cEv1!, cEv2!);
            }
            else
            {
                Loe = 100;
            }
        }
        else
        {
            Loe = 100;
        }
        return Loe;
    }

    private static int Compare(string[] kont1, string[] kont2)
    {
        var Loe = 0;
        if (kont1[2].Trim() != kont2[2].Trim()
            || kont1[3].Trim() != kont2[1].Trim()
            || kont1[4].Trim() != kont2[4].Trim()
            || kont1[5].Trim() != kont2[5].Trim()
            || kont1[6].Trim() != kont2[6].Trim()
            || kont1[7].Trim() != kont2[7].Trim()
            || kont1[8].Trim() != kont2[8].Trim()
            || kont1[9].Trim() != kont2[9].Trim()
            || kont1[12].Trim() != kont2[12].Trim()
                || kont1[13].Trim() != kont2[13].Trim()
                || kont1[14].Trim() != kont2[14].Trim()
                || kont1[16].Trim() != kont2[16].Trim())
        {
            Loe = 1;
        }
        else if (kont1[17].Trim() != kont2[17].Trim())
        {
            Loe = 12;
        }
        else if (kont1[15].Trim() != kont2[15].Trim())
        {
            Loe = 6;
        }
        else if (kont1[11].Trim() != kont2[11].Trim())
        {
            Loe = 3;
        }
        else if (kont1[10].Trim() != kont2[10].Trim())
        {
            Loe = 2;
        }
        return Loe;
    }

    private static int Event_Compare(IEventData kont1, IEventData kont2)
    {
        var Loe = 0;
        (EEventProp, byte)[] atList = { (EEventProp.dDatumV, 1), (EEventProp.sDatumV_S, 1), (EEventProp.dDatumB, 1),
            (EEventProp.sDatumV_S, 1), (EEventProp.iKBem, 1), (EEventProp.iOrt, 1),(EEventProp.sOrt_S, 1),
          (EEventProp.sReg, 1), (EEventProp.iPlatz, 1),(EEventProp.sVChr, 1),(EEventProp.iLfNr, 1),
          (EEventProp.iArtText, 1),(EEventProp.sZusatz, 12),(EEventProp.sBem, 3) };

        foreach (var (prop, i) in atList)
        {
            if (kont1.GetPropValue(prop) != kont2.GetPropValue(prop))
            {
                Loe = i;
                break;
            }
        }
        return Loe;
    }


    private static string[] ReadDataRow(IRecordset dB_EventTable)
    {
        string[] kont = new string[20];
        kont.Initialize();
        for (EventFields i = default; i < (EventFields)18; i++)
        {
            kont[(int)i] = dB_EventTable.Fields[$"{i}"].AsString();
        }
        kont[18] = dB_EventTable.Fields[22].AsString();
        kont[19] = dB_EventTable.Fields[23].AsString();
        if (kont[15] == "0")
        {
            kont[15] = "";
        }
        if (kont[16] == "0")
        {
            kont[16] = "";
        }
        if (kont[17] == "0")
        {
            kont[17] = "";
        }
        return kont;
    }

    public void CheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        View.Button1.PerformClick();
    }

    public void CheckBox2_CheckedChanged(object sender, EventArgs e)
    {
        if (View.CheckBox2.Checked)
        {
            View.CheckBox1.Checked = false;
        }
    }

    public void CheckBox1_Click(object sender, EventArgs e)
    {
        if (View.CheckBox1.Checked)
        {
            View.CheckBox2.Checked = false;
        }
        View.Button1.PerformClick();
    }

    public void CheckBox2_Click(object sender, EventArgs e)
    {
        if (View.CheckBox2.Checked)
        {
            View.CheckBox1.Checked = false;
        }
        View.Button1.PerformClick();
    }

    public void Button2_Click(object sender, EventArgs e)
    {
        if (Modul1.Trans != 0)
        {
            Frag();
        }
        Modul1.FamInArb = checked(View._Label1_0.Tag.AsInt());
        Modul1.Aend = 0f;
        Modul1.Ad = false;
        Familie.Default.Show(Modul1.FamInArb, EUserText.tNMBack);
    }

    public void Button3_Click(object sender, EventArgs e)
    {
        if (Modul1.Trans != 0)
        {
            Frag();
        }
        Modul1.FamInArb = checked(View._Label1_20.Tag.AsInt());
        Modul1.Aend = 0f;
        Modul1.Ad = false;
        Familie.Default.Show(Modul1.FamInArb, EUserText.tNMBack);
    }

    public void Button4_Click(object sender, EventArgs e)
    {
        View.TextBox1.Text = "";
        short Index = 0;
        ZeigNaNum(ref Index);
        if (View._List1_0.Items.Count > 1)
        {
            Index = 1;
            ZeigNaNum(ref Index);
        }
    }

    public void RadioButton2_CheckedChanged(object sender, EventArgs e)
    {
        View.Button4.Enabled = View.RadioButton2.Checked;
    }

    public void List1_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right)
        {
            return;
        }
        short index = (short)View.List1.GetIndex((ListBox)sender);
        if (Strings.Trim(View.List1[index].SelectedItem.AsString().Left(41)) != "")
        {
            if (index == 0)
            {
                View.TextBox1.Text = Strings.Trim(View.List1[index].SelectedItem.AsString().Left(41));
                View.TextBox1.Tag = View.List1[index].ItemData<int>();
            }
            else
            {
                View.TextBox2.Text = Strings.Trim(View.List1[index].SelectedItem.AsString().Left(41));
                View.TextBox2.Tag = View.List1[index].ItemData<int>();
            }
        }
    }

}
