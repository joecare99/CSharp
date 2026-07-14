using BaseLib.Helper;
using Gen_FreeWin.Main;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;

//using DAO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Gen_FreeWin.Views;
internal partial class Fehlerli : Form
{
    private static List<WeakReference> __ENCList = new();
    [Obsolete]
    IProjectData ProjectData;
    IInteraction Interaction;
    [Obsolete]
    IVBInformation Information;
    [Obsolete]
    IOperators Operators;
    [Obsolete]
    IVBConversions Conversion;
    [Obsolete]
    IStrings Strings;
    IStrings StringType;

    IModul1 Modul1 => _Modul1.Instance;


    private short T;

    private int I1;

    private DateTime transdat;

    private int A;


#pragma warning disable CS0618 // Typ oder Element ist veraltet

    public ControlArray<Label> Label1;
    private int Modul1_PersInArbsp;
    private string Modul1_LiText;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual ControlArray<Button> Command1
    {
        [DebuggerNonUserCode]
        get => _Command1;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler obj = Command1_Click;
            if (_Command1 != null)
            {
                _Command1.RemoveClick(obj);
            }
            _Command1 = value;
            if (_Command1 != null)
            {
                _Command1.AddClick(obj);
            }
        }
    }
#pragma warning restore CS0618 // Typ oder Element ist veraltet


    [DebuggerNonUserCode]
    public Fehlerli()
    {
        Load += Fehlerli_Load;
        FormClosing += Fehlerli_FormClosing;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }

#pragma warning disable CS0618 // Typ oder Element ist veraltet
        Command1 = new ControlArray<Button>();
        Label1 = new ControlArray<Label>();
#pragma warning restore CS0618 // Typ oder Element ist veraltet

        ((ISupportInitialize)Command1).BeginInit();
        ((ISupportInitialize)Label1).BeginInit();

        InitializeComponent();

        Command1.SetIndex(_Command1_3, 3);
        Command1.SetIndex(_Command1_2, 2);
        Command1.SetIndex(_Command1_1, 1);
        Command1.SetIndex(_Command1_0, 0);
        Label1.SetIndex(_Label1_8, 8);
        Label1.SetIndex(_Label1_9, 9);
        Label1.SetIndex(_Label1_10, 10);
        Label1.SetIndex(_Label1_2, 2);
        Label1.SetIndex(_Label1_1, 1);
        Label1.SetIndex(_Label1_0, 0);
        Command1.SetIndex(_Command1_6, 6);
        Command1.SetIndex(_Command1_5, 5);
        Command1.SetIndex(_Command1_4, 4);
        Command1.SetIndex(_Command1_7, 7);


        ((ISupportInitialize)Command1).EndInit();
        ((ISupportInitialize)Label1).EndInit();

    }

    private void Command1_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_451b
        int try0001_dispatch = -1;
        int num = default;
        int index = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        int num6 = default;
        int num7 = default;
        int num8 = default;
        int num10 = default;
        string sDest2 = default;
        short num12 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int M1_Iter = default;
                    int num4;
                    int num5;
                    var famInArb = Modul1.FamInArb;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            index = Command1.GetIndex((Button)eventSender);
                            goto IL_0016;
                        case 21059:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_45b9;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Information.Err().Number == 6)
                                {
                                    num7 = 200000000;
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_45b9;
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
                                    goto IL_45bd;
                                }
                            }
                        end_IL_0001:
                            break;
                        IL_0016:
                            num = 2;
                            RadioButton1.Visible = true;
                            RadioButton2.Visible = true;
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            switch (index)
                            {
                                case 0:
                                case 1:
                                case 4:
                                case 5:
                                case 6:
                                    break;
                                case 7:
                                    goto IL_0094;
                                default:
                                    goto IL_00a6;
                            }
                            {
                                goto IL_006f;
                            }
                        IL_006f:
                            num = 9;
                            List2.Visible = false;
                            List1.Visible = true;
                            goto IL_00a6;
                        IL_0094:
                            num = 13;
                            List1.Visible = false;
                            goto IL_00a6;
                        IL_00a6: // <========== 4
                                 // <========== 4
                            num = 15;
                            Label4.Text = "";
                            if (Modul1.Aus[12] == "")
                            {
                                Modul1.Aus[12] = "200";
                            }
                            if (Modul1.Aus[13] == "")
                            {
                                Modul1.Aus[13] = "200";
                            }
                            List1.Visible = true;
                            List2.Visible = false;
                            Label2.Text = "";
                            if (RadioButton1.Checked)
                            {
                                List1.Sorted = true;
                            }
                            if (RadioButton2.Checked)
                            {
                                List1.Sorted = false;
                            }
                            switch (index)
                            {
                                case 0:
                                    if (!Fehlliste_Personen())
                                        goto end_IL_0001_2;
                                    break;
                                case 1:
                                    if (!Fehliste_Familien())
                                        goto end_IL_0001_2;
                                    break;
                                case 2:
                                    List1.Items.Clear();
                                    List2.Items.Clear();
                                    ProgressBar1.Minimum = 0;
                                    ProgressBar1.Maximum = 0;
                                    Close();
                                    goto end_IL_0001_2;
                                case 3:
                                    goto IL_1aae;
                                case 4:
                                    goto IL_1c00;
                                case 5:
                                    goto IL_2496;
                                case 6:
                                    goto IL_3406;
                                case 7:
                                    goto IL_3e4e;
                                default:
                                    break;
                            }
                            goto IL_448a;


                        IL_1aae:
                            num = 322;
                            Modul1.Persistence.WriteStringsTemp("Text4.Txt", [_Label1_2.Text, _Label1_1.Text, _Label1_0.Text]);
                            if (List2.Visible)
                            {
                                Modul1.Listbox3Clip(List2.Items, 2);
                            }
                            else
                            {
                                Modul1.Listbox3Clip(List1.Items, 2);
                            }
                            goto IL_448a;
                        IL_1c00:
                            num = 336;
                            //Sz = 0;
                            _ = FehlListe_FamWODates();
                            goto end_IL_0001_2;

                        IL_2496:
                            num = 439;
                            List1.Items.Clear();
                            _Label1_2.Text = "Personen ohne Datum";
                            _Label1_1.Text = Modul1.IText[EUserText.t166];
                            _Label1_0.Text = Modul1.IText[EUserText.t167];
                            I1 = 0;
                            while (I1 <= 2)
                            {
                                Label1[(short)I1].Refresh();
                                I1++;
                            }
                            num7 = 1;
                            num8 = DataModul.Person.MaxID;
                            if (num7 == 0 || num7 > num8)
                            {
                                if (num7 > num8)
                                    _ = Interaction.MsgBox(Modul1.IText[EUserText.t173] + " " + num7.AsString() + Modul1.IText[EUserText.t172]);
                                goto end_IL_0001_2;
                            }

                            ProgressBar1.Minimum = 0;
                            ProgressBar1.Maximum = 0;
                            ProgressBar1.Step = 1;
                            ProgressBar1.Maximum =
                            num10 = DataModul.Person.Count - 1;
                            I1 = num7;
                            goto IL_33b6;

                        IL_32fc: // <========== 4
                                 // <========== 4
                            num = 606;
                            lErl = 45;
                            Modul1_LiText += sDest2;
                            goto IL_3320;
                        IL_3320: // <========== 3
                                 // <========== 3
                            num = 609;
                            if (Modul1_LiText.Trim() != "")
                            {
                                _ = List1.Items.Add(new ListItem(Modul1_LiText + "          " + Modul1.PersInArb));
                            }
                            Modul1_LiText = "";
                            lErl = 95;
                            I1++;
                            goto IL_33b6;
                        IL_33b6:
                            while (I1 <= num10)
                            {
                                ProgressBar1.PerformStep();
                                Application.DoEvents();
                                Modul1.PersInArb = I1;
                                if (Modul1.PersInArb <= num8)
                                {
                                    if (DataModul.Person.Exists(Modul1.PersInArb))
                                    {
                                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                        Modul1_LiText = Strings.Left(Modul1.Person.SurName.Trim() + "," + Modul1.Person.Givennames.Trim() + "                                  ", 20);
                                        Modul1.Datles(Modul1.PersInArb, Modul1.Person, true);
                                        sDest2 = "                                                                            ";
                                        if (Modul1.Person.Birthday.Trim() == "" && Modul1.Person.Baptised.Trim() == "" && Modul1.Person.Death.Trim() == "" && Modul1.Person.Burial.Trim() == "")
                                        {
                                            num12 = 300;
                                            while (num12 <= 302)
                                            {
                                                if (DataModul.Event.ReadData((EEventArt)num12, Modul1.PersInArb, out var cEvt))
                                                {
                                                    int[] aiStrPos = num12 switch
                                                    {
                                                        300 => new[] { 34, 38 },
                                                        301 => new[] { 42, 46 },
                                                        302 => new[] { 50, 53 },
                                                        _ => Array.Empty<int>(),
                                                    };
                                                    if (aiStrPos.Length > 0)
                                                    {
                                                        if (cEvt.iKBem > 0)
                                                            StringType.MidStmtStr(ref sDest2, aiStrPos[0], 1, Modul1.IText[EUserText.t175]);

                                                        if (!(cEvt.dDatumV != default)
                                                            && !(cEvt.dDatumB != default))
                                                        {
                                                            if (cEvt.iOrt > 0)
                                                                StringType.MidStmtStr(ref sDest2, aiStrPos[1], 1, Modul1.IText[EUserText.t175]);
                                                        }
                                                        else
                                                        {
                                                            I1++;
                                                            goto IL_33b6;
                                                        }
                                                    }
                                                }
                                                lErl = 92;
                                                num12++;
                                            }
                                            goto IL_32fc;
                                        }
                                    }
                                    I1++;
                                    goto IL_33b6;
                                }
                            }
                            Label4.Text = $"{List1.Items.Count - 1} Einträge";

                            goto IL_448a;
                        IL_3406:
                            num = 618;
                            if (Fehlliste_LinkedPersons())
                                goto end_IL_0001_2;

                            goto IL_448a;
                        IL_3e4e:
                            num = 716;
                            num7 = Fehliste_Orte(List2.Items);
                            goto IL_448a;
                        IL_448a: // <========== 8
                                 // <========== 9
                            num = 781;
                            lErl = 4;
                            if (RadioButton2.Checked)
                            {
                                _ = List1.Items.Add("Ende der Liste");
                                M1_Iter = 1;
                                while (M1_Iter <= 17)
                                {
                                    _ = List1.Items.Add("");
                                    M1_Iter++;
                                }
                            }
                            goto end_IL_0001_2;
                        IL_45b9:
                            num4 = unchecked(num2 + 1);
                            goto IL_45bd;
                        IL_45bd:
                            num2 = 0;

                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 6:
                                case 11:
                                case 14:
                                case 15:
                                    goto IL_00a6;




                                case 550:
                                    num = 550;
                                    Modul1_PersInArbsp = Modul1.PersInArb;
                                    if (DataModul.Person.GetSex(Modul1.PersInArb) == "F")
                                    {
                                        Modul1.eLKennz = ELinkKennz.lkMother;
                                    }
                                    else
                                        Modul1.eLKennz = ELinkKennz.lkFather;
                                    if (DataModul.Link.GetPersonFam(Modul1.PersInArb, Modul1.eLKennz, out famInArb))
                                    {
                                        goto case 559;
                                    }
                                    if (Modul1_LiText.Trim() != "")
                                    {
                                        _ = List1.Items.Add(new ListItem(Modul1_LiText + "          " + Modul1.PersInArb));
                                    }
                                    Modul1_LiText = "";
                                    lErl = 95;
                                    I1++;
                                    goto IL_33b6;
                                case 559:
                                    num = 559;
                                    DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                                    if (Modul1.eLKennz == ELinkKennz.lkFather)
                                    {
                                        Modul1.PersInArb = Modul1.Family.Frau;
                                    }
                                    if (Modul1.eLKennz == ELinkKennz.lkMother)
                                    {
                                        Modul1.PersInArb = Modul1.Family.Mann;
                                    }
                                    M1_Iter = 101;
                                    while (M1_Iter <= 102)
                                    {
                                        Modul1.Ubg = M1_Iter;
                                        if (DataModul.Event.ReadData((EEventArt)Modul1.Ubg, Modul1.PersInArb, out var cEvt2)
                                            && cEvt2.dDatumV != default)
                                        {
                                            transdat = cEvt2.dDatumV;
                                            Event_Set(Modul1_PersInArbsp, EEventArt.eA_Birth, transdat, "C");
                                            I1++;
                                            goto IL_33b6;
                                        }
                                        lErl = 6;
                                        num = 583;
                                        M1_Iter++;
                                    }
                                    num6 = 30000000;
                                    A = 1;
                                    if (Modul1.Family.Kind[A] > 0)
                                    {
                                        goto case 587;
                                    }
                                    goto case 598;
                                case 586:
                                case 587:
                                case 588:
                                    num = 587;
                                    M1_Iter = 101;
                                    if (!DataModul.Event.ReadData((EEventArt)M1_Iter, Modul1.Family.Kind[A], out var cEvt)
                                       && cEvt.dDatumV != default
                                       && cEvt.dDatumV.AsInt() < num6)
                                    {
                                        num6 = cEvt.dDatumV.AsInt();
                                    }
                                    goto case 593;
                                case 593: // <========== 4
                                          // <========== 4
                                case 594:
                                case 595:
                                case 596:
                                    {
                                        num = 596;
                                        M1_Iter++;
                                        int i2 = M1_Iter;
                                        num5 = 102;
                                        if (i2 <= num5)
                                        {
                                            goto case 588;
                                        }
                                        goto case 597;
                                    }
                                case 598:
                                    num = 598;
                                    goto case 599;
                                case 597:
                                case 600:
                                case 601:
                                    {
                                        num = 601;
                                        A++;
                                        int a = A;
                                        num5 = 99;
                                        if (a <= num5)
                                        {
                                            goto case 586;
                                        }
                                        goto case 599;
                                    }
                                case 599:
                                case 602:
                                    num = 602;
                                    if (num6 < 30000000)
                                    {
                                        goto case 603;
                                    }
                                    goto IL_32fc;
                                case 603:
                                    num = 603;
                                    transdat = num6.AsDate().Subtract(new TimeSpan(365 * 25, 0, 0, 0, 0));
                                    Event_Set(Modul1_PersInArbsp, EEventArt.eA_Birth, transdat, "C");
                                    goto IL_32fc;
                                case 549:
                                case 605:
                                case 606:
                                    goto IL_32fc;
                                case 558:
                                case 608:
                                case 609:
                                    goto IL_3320;
                                case 32:
                                case 68:
                                case 157:
                                case 195:
                                case 311:
                                case 312:
                                case 320:
                                case 334:
                                case 437:
                                case 469:
                                case 616:
                                case 647:
                                case 714:
                                case 779:
                                case 780:
                                case 781:
                                    goto IL_448a;
                                case 46:
                                case 52:
                                case 56:
                                case 178:
                                case 184:
                                case 319:
                                case 355:
                                case 361:
                                case 436:
                                case 448:
                                case 454:
                                case 458:
                                case 623:
                                case 629:
                                case 633:
                                case 720:
                                case 787:
                                case 788:
                                case 798:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 21059;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 19
            // <========== 19
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private bool FehlListe_FamWODates()
    {
        List1.Items.Clear();
        _Label1_2.Text = "Familien ohne Datum";
        _Label1_1.Text = "Mann            Frau       Kinder  Pro  Verl. Hei   k.H. Schd. Eheä. Aus  Fikt.";
        _Label1_0.Text = "                                    DO   DO    DO    DO   DO    DO        DO";
        if (RadioButton2.Checked)
        {
            _ = List1.Items.Add("Familien ohne Datum");
        }
        if (RadioButton2.Checked)
        {
            _ = List1.Items.Add("Mann            Frau       Kinder  Pro  Verl. Hei   k.H. Schd. eheä. auß  Fikt.");
        }
        if (RadioButton2.Checked)
        {
            _ = List1.Items.Add("                                    DO   DO    DO    DO   DO    DO        DO");
        }
        var M1_Iter = 0;
        while (M1_Iter <= 2)
        {
            Label1[(short)M1_Iter].Refresh();
            M1_Iter++;
        }
        var num7 = 1;
        if (num7 <= 0)
        {
            return false;
        }
        DataModul.DB_FamilyTable.MoveLast();
        var iMaxFamily = DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();
        if (num7 > iMaxFamily)
        {
            _ = Interaction.MsgBox(Modul1.IText[EUserText.t174] + " " + num7.AsString() + Modul1.IText[EUserText.t172]);
            return false;
        }
        ProgressBar1.Minimum = 0;
        ProgressBar1.Maximum = 0;
        ProgressBar1.Step = 1;
        ProgressBar1.Maximum = Modul1.Aus[13].AsInt();
        I1 = num7;
        while (I1 <= iMaxFamily)
        {
            ProgressBar1.PerformStep();
            Application.DoEvents();
            var sDest2 = new string(' ', 90);
            Modul1.FamInArb = I1;
            if (Modul1.FamInArb <= iMaxFamily)
            {
                DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
                DataModul.DB_FamilyTable.Seek("=", Modul1.FamInArb.AsString());
                if (!DataModul.DB_FamilyTable.NoMatch)
                {
                    DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                    if (Modul1.Family.Mann > 0)
                    {
                        Modul1.PersInArb = Modul1.Family.Mann;
                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                        if (Modul1.Person.SurName != "")
                        {
                            StringType.MidStmtStr(ref sDest2, 1, 15, Modul1.Person.SurName);
                        }
                        else
                        {
                            StringType.MidStmtStr(ref sDest2, 1, 15, ">NN<");
                        }
                        StringType.MidStmtStr(ref sDest2, 16, 2, "/ ");
                    }
                    if (Modul1.Family.Frau > 0)
                    {
                        Modul1.PersInArb = Modul1.Family.Frau;
                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                        if (Modul1.Person.SurName != "")
                        {
                            StringType.MidStmtStr(ref sDest2, 17, 15, Modul1.Person.SurName);
                        }
                        else
                        {
                            StringType.MidStmtStr(ref sDest2, 17, 15, ">NN<");
                        }
                    }
                    if (Modul1.Family.Kind[1] != 0)
                    {
                        StringType.MidStmtStr(ref sDest2, 32, 1, Modul1.IText[EUserText.t175]);
                    }

                    DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
                    DataModul.DB_FamilyTable.Seek("=", Modul1.FamInArb);
                    if (!DataModul.DB_FamilyTable.NoMatch)
                    {
                        if (DataModul.DB_FamilyTable.Fields[FamilyFields.Aeb].AsBool())
                        {
                            StringType.MidStmtStr(ref sDest2, 70, 1, Modul1.IText[EUserText.t175]);
                        }
                        var num15 = 500;
                        while (num15 <= 507)
                        {
                            if (DataModul.Event.Exists((EEventArt)num15, Modul1.FamInArb, 0))
                            {
                                break;
                            }
                            num15++;
                        }
                        if (num15 <= 507)
                        {
                            I1++;
                            continue;

                        }

                    }
                }
                if (!DataModul.Event.Exists(EEventArt.eA_601, Modul1.FamInArb, 0)
                    && sDest2.Trim() != "")
                {
                    _ = List1.Items.Add(new ListItem(sDest2 + "          " + Modul1.FamInArb.AsString()));
                    //Sz++;
                }
            }
            I1++;

        }
        Label4.Text = $"{(List1.Items.Count - 2).AsString()} Einträge";
        return true;
    }

    private bool Fehliste_Familien()
    {
        List1.Items.Clear();
        _Label1_2.Text = "Fehlliste Familien";
        _Label1_1.Text = "Mann            Frau       Kinder  Pro  Verl. Hei   k.H. Schd. eheä. auß  Fikt.";
        _Label1_0.Text = "                                    DO   DO    DO    DO   DO    DO        DO";
        if (RadioButton2.Checked)
        {
            _ = List1.Items.Add("Fehlliste Familien");
        }
        if (RadioButton2.Checked)
        {
            _ = List1.Items.Add("Mann            Frau       Kinder  Pro  Verl. Hei   k.H. Schd. eheä. auß  Fikt.");
        }
        if (RadioButton2.Checked)
        {
            _ = List1.Items.Add("                                    DO   DO    DO    DO   DO    DO        DO");
        }
        var M1_Iter = 0;
        while (M1_Iter <= 2)
        {
            Label1[(short)M1_Iter].Refresh();
            M1_Iter++;
        }
        var num7 = 1;
        DataModul.DB_FamilyTable.MoveLast();
        var iMaxPers = DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();
        if (num7 > iMaxPers)
        {
            _ = Interaction.MsgBox("Die höchste Familiennummer ist " + DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt().AsString());
            return false;
        }
        ProgressBar1.Minimum = 0;
        ProgressBar1.Maximum = DataModul.DB_FamilyTable.RecordCount - 1;
        ProgressBar1.Step = 1;
        I1 = num7;
        while (I1 <= iMaxPers)
        {
            ProgressBar1.PerformStep();
            Application.DoEvents();
            var sDest2 = new string(' ', 90);
            DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
            DataModul.DB_FamilyTable.Seek("=", I1);
            if (!DataModul.DB_FamilyTable.NoMatch)
            {
                DataModul.Link.ReadFamily(I1, Modul1.Family);
                if (Modul1.Family.Mann > 0)
                {
                    Modul1.PersInArb = Modul1.Family.Mann;
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    if (Modul1.Person.SurName != "")
                    {
                        StringType.MidStmtStr(ref sDest2, 1, 15, Modul1.Person.SurName);
                    }
                    else
                    {
                        StringType.MidStmtStr(ref sDest2, 1, 15, ">NN<");
                    }
                    StringType.MidStmtStr(ref sDest2, 16, 2, "/ ");
                }
                if (Modul1.Family.Frau > 0)
                {
                    Modul1.PersInArb = Modul1.Family.Frau;
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    if (Modul1.Person.SurName != "")
                    {
                        StringType.MidStmtStr(ref sDest2, 17, 15, Modul1.Person.SurName);
                    }
                    else
                    {
                        StringType.MidStmtStr(ref sDest2, 17, 15, ">NN<");
                    }
                }
                if (Modul1.Family.Kind[1] != 0)
                {
                    StringType.MidStmtStr(ref sDest2, 32, 1, Modul1.IText[EUserText.t175]);
                }
                DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
                DataModul.DB_FamilyTable.Seek("=", I1);
                if (!DataModul.DB_FamilyTable.NoMatch)
                {
                    if (DataModul.DB_FamilyTable.Fields[FamilyFields.Aeb].AsBool())
                    {
                        StringType.MidStmtStr(ref sDest2, 70, 1, Modul1.IText[EUserText.t175]);
                    }
                    EEventProp[] aeFields = new[] { EEventProp.dDatumV, EEventProp.iOrt };
                    EEventArt eEventArt = EEventArt.eA_500;
                    while (eEventArt <= EEventArt.eA_505)
                    {
                        if (DataModul.Event.ReadData(eEventArt, I1, out var cEv))
                        {
                            int[] aiStrPos = eEventArt switch
                            {
                                EEventArt.eA_500 => new[] { 36, 37 },
                                EEventArt.eA_501 => new[] { 42, 43 },
                                EEventArt.eA_Marriage => new[] { 48, 49 },
                                EEventArt.eA_MarrReligious => new[] { 54, 55 },
                                EEventArt.eA_504 => new[] { 59, 60 },
                                EEventArt.eA_505 => new[] { 65, 66 },
                                _ => Array.Empty<int>(),
                            };
                            for (var i = 0; i < aiStrPos.Length; i++)
                                if (cEv.GetPropValue(aeFields[i]).AsInt() > 0)
                                    StringType.MidStmtStr(ref sDest2, aiStrPos[i], 1, Modul1.IText[EUserText.t175]);
                        }
                        eEventArt++;
                    }
                    if (DataModul.Event.ReadData(EEventArt.eA_601, I1, out var cEv2))
                    {
                        int[] aiStrPos = new[] { 75, 76 };
                        for (var i = 0; i < aiStrPos.Length; i++)
                            if (cEv2.GetPropValue(aeFields[i]).AsInt() > 0)
                                StringType.MidStmtStr(ref sDest2, aiStrPos[i], 1, Modul1.IText[EUserText.t175]);
                    }
                    _ = List1.Items.Add(new ListItem(sDest2 + "          " + I1.AsString()));
                }
            }
            I1++;
        }
        Label4.Text = $"{(List1.Items.Count - 3).AsString()} Familien";
        if (RadioButton1.Checked)
        {
            Label4.Text = List1.Items.Count.AsString() + " Familien";
        }
        return true;
    }

    private bool Fehlliste_LinkedPersons()
    {
        var aiStrPos = new[] { 0, 24, 24, 30, 36, 46, 46, 46, 55, 70 };
        List1.Items.Clear();
        _Label1_2.Text = Modul1.IText[EUserText.t84_Persons];
        _Label1_0.Text = "Name              Elternteil  Kind  Pate  Zeuge  Adoptivkind  verbunden";
        var num7 = 1;
        var num8 = DataModul.Person.MaxID;
        if (num7 == 0 || num7 > num8)
        {
            if (num7 > num8)
                _ = Interaction.MsgBox(Modul1.IText[EUserText.t173] + " " + num7.AsString() + Modul1.IText[EUserText.t172]);
            return false;
        }
        _Label1_1.Text = "unvollständig verknüpfte Personen ";
        Modul1.Schalt = 3;
        ProgressBar1.Minimum = 0;
        ProgressBar1.Maximum = 0;
        ProgressBar1.Step = 1;
        var num9 =
        ProgressBar1.Maximum =
            DataModul.Person.Count - 1;
        var I1 = num7;
        while (I1 <= num9 && I1 <= num8)
        {
            ProgressBar1.PerformStep();
            Application.DoEvents();
            var sDest = new string(' ', 80);
            if (DataModul.Person.Exists(I1))
            {
                bool[] axLinkEx = new bool[10];
                axLinkEx.Initialize();
                for (ELinkKennz lkI = ELinkKennz.lkFather; lkI <= ELinkKennz.lk9; lkI++)
                {
                    axLinkEx[(int)lkI] = DataModul.Link.ExistE(I1, lkI);
                }
                var LiEl = axLinkEx[1] && axLinkEx[2] ? 1 : 0;
                var LiKi = axLinkEx[3] ? 1 : 0;

                if (LiKi == 0 & LiEl == 0)
                {
                    Modul1.Person_ReadNames(I1, Modul1.Person);
                    StringType.MidStmtStr(ref sDest, 1, 20, $"{Modul1.Person.SurName.Trim() + "," + Modul1.Person.Givennames.Trim(),20}");
                    for (ELinkKennz lkI = ELinkKennz.lkFather; lkI <= ELinkKennz.lkChild; lkI++)
                        if (!axLinkEx[(int)lkI])
                            StringType.MidStmtStr(ref sDest, aiStrPos[(int)lkI], 1, "N");
                    for (ELinkKennz lkI = ELinkKennz.lkGodparent; lkI <= ELinkKennz.lk9; lkI++)
                        if (axLinkEx[(int)lkI])
                            StringType.MidStmtStr(ref sDest, aiStrPos[(int)lkI], 1, "J");

                    DataModul.DB_WitnessTable.Index = nameof(WitnessIndex.ElSu);
                    DataModul.DB_WitnessTable.Seek("=", I1, "10");
                    if (!DataModul.DB_WitnessTable.NoMatch)
                    {
                        StringType.MidStmtStr(ref sDest, aiStrPos[5], 1, "J");
                    }
                    _ = List1.Items.Add(new ListItem($"{sDest}          {I1}", I1));
                }
            }
            I1++;
        }
        Label4.Text = $"{(List1.Items.Count - 1).AsString()} Einträge";
        return true;
    }


    private int Fehliste_Orte(IList items)
    {
        int num7;
        List2.Visible = true;
        items.Clear();
        num7 = 1;
        _Label1_2.Text = "Fehlliste Orte";
        _Label1_1.Text = "      Nr    Ort Orsteil  Kreis Land Staat Loc  Länge  Breite PLZ Terr StKz";
        _Label1_0.Text = "  ";
        _ = items.Add("Fehlliste Orte");
        _ = items.Add("      Nr    Ort Ortsteil Kreis Land Staat Loc. Laenge Breite PLZ Terr. StKz");
        A = 1;
        foreach (var cPlace in DataModul.Place.ReadAll())
        {
            var Modul1_LiText = new string(' ', 80);
            StringType.MidStmtStr(ref Modul1_LiText, 1, 10, $"{cPlace.ID,10}");
            if (cPlace.iOrt == 0)
            {
                StringType.MidStmtStr(ref Modul1_LiText, 12, 2, "F ");
            }
            if (cPlace.iOrtsteil == 0)
            {
                StringType.MidStmtStr(ref Modul1_LiText, 20, 2, "F ");
            }
            if (cPlace.iKreis == 0)
            {
                StringType.MidStmtStr(ref Modul1_LiText, 28, 2, "F ");
            }
            if (cPlace.iLand == 0)
            {
                StringType.MidStmtStr(ref Modul1_LiText, 34, 2, "F ");
            }
            if (cPlace.iStaat == 0)
            {
                StringType.MidStmtStr(ref Modul1_LiText, 39, 2, "F ");
            }
            if (cPlace.sLoc.Trim() == "")
            {
                StringType.MidStmtStr(ref Modul1_LiText, 44, 2, "F ");
            }
            if (cPlace.sL.Trim() == "")
            {
                StringType.MidStmtStr(ref Modul1_LiText, 52, 2, "F ");
            }
            if (cPlace.sB.Trim() == "")
            {
                StringType.MidStmtStr(ref Modul1_LiText, 58, 2, "F ");
            }
            if (cPlace.sPLZ.Trim() == "")
            {
                StringType.MidStmtStr(ref Modul1_LiText, 63, 2, "F ");
            }
            if (cPlace.sTerr.Trim() == "")
            {
                StringType.MidStmtStr(ref Modul1_LiText, 68, 2, "F ");
            }
            if (cPlace.sStaatk.Trim() == "")
            {
                StringType.MidStmtStr(ref Modul1_LiText, 71, 2, "F ");
            }
            _ = items.Add(new ListItem(Modul1_LiText, cPlace.ID));
        }
        _ = items.Add("Ende der Liste");

        return num7;
    }

    private bool Fehlliste_Personen()
    {
        IList items = List1.Items;
        items.Clear();
        _Label1_2.Text = "Fehlliste Personen";
        _Label1_1.Text = Modul1.IText[EUserText.t166];
        _Label1_0.Text = Modul1.IText[EUserText.t167];
        _Label1_1.Text = _Label1_1.Text + " Quelle";
        _Label1_0.Text = _Label1_0.Text + "    T sDateV_S";
        I1 = 0;
        while (I1 <= 2)
        {
            Label1[(short)I1].Refresh();
            I1++;
        }
        var num7 = 1;
        var num8 = DataModul.Person.MaxID;
        if (num7 == 0 || num7 > num8)
        {
            if (num7 > num8)
            {
                _ = Interaction.MsgBox("Die höchste Personennummer ist " + num8.AsString());
            }
            return false;
        }
        ProgressBar1.Minimum = 0;
        ProgressBar1.Maximum = 0;
        ProgressBar1.Step = 1;
        ProgressBar1.Maximum = DataModul.Person.Count - 1;
        items.Clear();
        I1 = num7;
        while (I1 < num8)
        {
            Modul1.PersInArb = I1;
            ProgressBar1.PerformStep();
            Application.DoEvents();
            if (DataModul.Person.Exists(Modul1.PersInArb))
            {
                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                Modul1_LiText = Strings.Left(Modul1.Person.SurName.Trim() + "," + Modul1.Person.Givennames.Trim() + "                                  ", 20);
                Modul1.Datles(Modul1.PersInArb, Modul1.Person, true);
                var sDest2 = "                                                                            ";
                StringType.MidStmtStr(ref sDest2, 2, 2, Modul1.Person.Birthday + Modul1.Kont[21]);
                StringType.MidStmtStr(ref sDest2, 8, 2, Modul1.Person.Baptised + Modul1.Kont[22]);
                StringType.MidStmtStr(ref sDest2, 17, 2, Modul1.Person.Death + Modul1.Kont[23]);
                StringType.MidStmtStr(ref sDest2, 27, 2, Modul1.Person.Burial + Modul1.Kont[24]);
                var eArt = EEventArt.eA_300;
                while (eArt <= EEventArt.eA_302)
                {
                    if (DataModul.Event.ReadData(eArt, Modul1.PersInArb, out var cEv))
                    {
                        EEventProp[] aeFields = new[] { EEventProp.iKBem, EEventProp.dDatumV, EEventProp.dDatumB, EEventProp.iOrt };
                        var aiStrPos = eArt switch
                        {
                            EEventArt.eA_300 => new[] { 34, 36, 37, 38 },
                            EEventArt.eA_301 => new[] { 42, 44, 45, 46 },
                            EEventArt.eA_302 => new[] { 50, 51, 52, 53 },
                            _ => Array.Empty<int>(),
                        };
                        for (var i = 0; i < aiStrPos.Length; i++)
                            if (cEv.GetPropValue(aeFields[i]).AsInt() > 0)
                                StringType.MidStmtStr(ref sDest2, aiStrPos[i], 1, Modul1.IText[EUserText.t175]);
                    }
                    eArt++;
                }
                Modul1_LiText += sDest2;
                var pt = DataModul.Person.Seek(Modul1.PersInArb);
                if (pt.Fields[PersonFields.Bem3].AsString() != "")
                {
                    StringType.MidStmtStr(ref Modul1_LiText, 77, 1, "J");
                }

                if (DataModul.SourceLink.ExistsEnt(1, Modul1.PersInArb))
                {
                    StringType.MidStmtStr(ref Modul1_LiText, 79, 1, "J");
                }
                _ = items.Add(new ListItem(Modul1_LiText + "          " + Modul1.PersInArb));
                Modul1_LiText = "";
            }
            I1++;
        }
        Label4.Text = items.Count.AsString() + " Einträge";
        return true;
    }

    private void Fehlerli_Load(object eventSender, EventArgs eventArgs)
    {
        checked
        {
            if (Modul1.FontSize > 0f)
            {
                Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                _Label1_0.Font = new Font("courier new", Modul1.FontSize, FontStyle.Regular);
                _Label1_1.Font = new Font("courier new", Modul1.FontSize, FontStyle.Regular);
                _Label1_2.Font = new Font("courier new", Modul1.FontSize, FontStyle.Regular);
                List1.Font = new Font("courier new", Modul1.FontSize, FontStyle.Regular);
                List2.Font = new Font("courier new", Modul1.FontSize, FontStyle.Regular);
                _Label1_8.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                _Label1_9.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                _Label1_10.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                short num = 0;
                short num2;
                short num3;
                do
                {
                    Command1[num].Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                    num = (short)unchecked(num + 1);
                    num2 = num;
                    num3 = 7;
                }
                while (num2 <= num3);
                Button1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                Button2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                Button3.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                Button4.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                Button6.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                Button7.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                Button9.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                Button10.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                Button11.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                Button12.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                Button13.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                Button14.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                Button15.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                Label3.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            }
        }
        Modul1.Persistence.ReadEnumInit<FormWindowState>("Windowstate", out var Modul1_WiS);
        WindowState = Modul1_WiS;
        var Pos = Modul1.Persistence.ReadIntsProg("maspos.dat", 2);
        Left = Pos[0];
        Top = Pos[1];
        BackColor = Modul1.HintFarb;
        _Label1_10.Text = Modul1.VersionT;
        _Label1_9.Text = Modul1.Version1;
        _Label1_8.Text = Modul1.Version;
        _Label1_8.Width = Width;
        if (Modul1.System.VerSpecial == 1)
        {
            _Label1_8.Text = "Eingeschränkte Sonderversion";
        }
        Command1[3].Text = Modul1.IText[EUserText.tNMPrint];
        Command1[0].Text = Modul1.IText[EUserText.t237_Persons];
        Command1[1].Text = Modul1.IText[EUserText.t236_Families];
        Command1[2].Text = Modul1.IText[EUserText.t158];

        // MVVM-Integration: ViewModel initialisieren und an View binden
        try
        {
            // TODO: Diese Zeilen könnten später in einen separaten Initialisierungs-Hook
            // oder in den View-Constructor verschoben werden, wenn das Legacy-System es erlaubt.
            var viewModel = FehlerliViewModelFactory.CreateAndBindViewModel(this);
            // ViewModel wird zur späteren Verwendung in einem Tag oder Property gespeichert
            // (z.B.: this.Tag = viewModel;)
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Fehler bei MVVM-Initialisierung in Fehlerli_Load: {ex.Message}");
            // Kein Fehler werfen; die Form lädt weiterhin mit Legacy-Verhalten auf
        }
    }

    private void Fehlerli_FormClosing(object eventSender, FormClosingEventArgs eventArgs)
    {
        int num4;
        var cancel = eventArgs.Cancel;
        if (eventArgs.CloseReason == 0)
        {
            DataModul.MandDB?.Close();
            DataModul.TempDB?.Close();
            DataModul.DOSB?.Close();
            DataModul.DSB?.Close();
            ProjectData.EndApp();
        }
    }
    private void List1_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        Familie.Default.btnMainmenue.Text = Modul1.IText[EUserText.tNMBack];
        checked
        {
            if (Strings.InStr(_Label1_2.Text, Modul1.IText[EUserText.t84_Persons]) != 0)
            {
                Modul1.PersInArb = (int)Math.Round(List1.Text.Right(10).AsDouble());
                Personen.Default.lblSearch2.Text = "";
                Modul1.Aend = 0f;
                Personen.Default.Show(Modul1.PersInArb, EUserText.tNMBack);
            }
            else
            {
                Modul1.FamInArb = (int)Math.Round(List1.Text.Right(10).AsDouble());
                Familie.Default.Show();
                short Rich;
                Familie.Default.Fameinlesen(Modul1.FamInArb, out Rich);
            }
            Familie.Default.btnMainmenue.Text = Modul1.IText[EUserText.tNMBack];
        }
    }

    private void List2_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        Modul1.Schalt = (byte)-List2.Tag.AsInt();
        _ = MainProject.Forms.Ortsver.ShowDialog();
        MainProject.Forms.Ortsver.Close();
    }

    private void Button1_Click(object sender, EventArgs e)
    {
        RadioButton1.Visible = true;
        RadioButton2.Visible = true;
        if (RadioButton1.Checked)
        {
            List1.Sorted = true;
        }
        if (RadioButton2.Checked)
        {
            List1.Sorted = false;
        }
        Label4.Text = "";
        List2.Visible = false;
        List1.Items.Clear();
        List1.Visible = true;
        _Label1_2.Text = "Personen ohne Eltern";
        long num = 1L;
        if (num <= 0)
        {
            return;
        }
        long num2 = DataModul.Person.MaxID;
        if (num == 0)
        {
            return;
        }
        ProgressBar1.Minimum = 0;
        ProgressBar1.Maximum = 0;
        ProgressBar1.Step = 1;
        checked
        {
            int num3 =
                ProgressBar1.Maximum = DataModul.Person.Count - 1;
            if (num > DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt())
            {
                _ = Interaction.MsgBox(Modul1.IText[EUserText.t173] + " " + num.AsString() + Modul1.IText[EUserText.t172]);
                return;
            }
            _ = List1.Items.Add("Personen ohne Eltern (tote Punkte)");
            Modul1.Schalt = 3;
            int i = (int)num;
            I1 = i;
            while (I1 <= num3 && I1 <= num2)
            {
                ProgressBar1.PerformStep();
                Application.DoEvents();
                Modul1.PersInArb = I1;
                string sDest = new(' ', 80);

                if (DataModul.Person.Exists(Modul1.PersInArb))
                {
                    var LiKi = 1;
                    //LiPa = 1;
                    Modul1.eLKennz = ELinkKennz.lkChild;
                    if (DataModul.Link.ExistE(Modul1.PersInArb, Modul1.eLKennz))
                    {
                        LiKi = 0;
                    }
                    if (LiKi == 0)
                    {
                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                        StringType.MidStmtStr(ref sDest, 1, 30, Strings.Left(Modul1.Person.SurName.Trim() + "," + Modul1.Person.Givennames.Trim() + "                                  ", 30));
                        var (sDat_Birth, sDat_Death) = Modul1.Datles(Modul1.PersInArb, Modul1.Person);
                        if (sDat_Birth.Trim() != "")
                        {
                            StringType.MidStmtStr(ref sDest, 35, 20, Modul1.sBirthMark + sDat_Birth);
                        }
                        else if (sDat_Death.Trim() != "")
                        {
                            StringType.MidStmtStr(ref sDest, 35, 20, Modul1.sDeathMark + sDat_Death);
                        }
                        _ = List1.Items.Add(new ListItem(sDest + "          " + Modul1.PersInArb));
                    }
                }
                I1++;
            }
        }
    }

    private void Button2_Click(object sender, EventArgs e)
    {
        RadioButton1.Visible = true;
        RadioButton2.Visible = true;
        Label4.Text = "";
        List1.Sorted = false;
        List2.Visible = false;
        string text = "";
        int num = 1;
        if (num <= 0)
        {
            return;
        }
        _Label1_2.Text = "Liste unvollständiger Familien";
        Modul1.FamInArb = 0;
        List1.Items.Clear();
        List1.Visible = true;
        ProgressBar1.Minimum = 0;
        ProgressBar1.Maximum = 0;
        ProgressBar1.Step = 1;
        ProgressBar1.Maximum = DataModul.DB_FamilyTable.RecordCount;
        _Label1_0.Text = "Liste unvollständiger Familien (nur eine Person)";
        _Label1_1.Text = "Mann            Frau         Kind";
        num = 1;
        DataModul.DB_FamilyTable.MoveFirst();
        int i = num;
        checked
        {
            int num2 = num + DataModul.DB_FamilyTable.RecordCount;
            int M1_Iter = i;
            while (true)
            {
                int i2 = M1_Iter;
                int num3 = num2;
                if (i2 > num3 || DataModul.DB_FamilyTable.EOF)
                {
                    break;
                }
                ProgressBar1.PerformStep();
                Application.DoEvents();
                text = "                                                                                          ";
                Modul1.FamInArb++;
                DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
                DataModul.DB_FamilyTable.Seek("=", Modul1.FamInArb.AsString());
                if (!DataModul.DB_FamilyTable.NoMatch)
                {
                    DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                    A = 0;
                    Label2.Text = "Fam.:" + Modul1.FamInArb.AsString();
                    if (Modul1.Family.Mann > 0)
                    {
                        A++;
                    }
                    if (Modul1.Family.Frau > 0)
                    {
                        A++;
                    }
                    if (Modul1.Family.Kind[1] != 0)
                    {
                        StringType.MidStmtStr(ref text, 32, 1, Modul1.IText[EUserText.t175]);
                        A++;
                    }
                    if (A < 2)
                    {
                        StringType.MidStmtStr(ref text, 16, 2, "/ ");
                        if (Modul1.Family.Mann > 0)
                        {
                            Modul1.PersInArb = Modul1.Family.Mann;
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            A++;
                            if (Modul1.Person.SurName != "")
                            {
                                StringType.MidStmtStr(ref text, 1, 15, Modul1.Person.SurName);
                            }
                            else
                            {
                                StringType.MidStmtStr(ref text, 1, 15, ">NN<");
                            }
                            StringType.MidStmtStr(ref text, 16, 2, "/ ");
                        }
                        if (Modul1.Family.Frau > 0)
                        {
                            Modul1.PersInArb = Modul1.Family.Frau;
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            A++;
                            if (Modul1.Person.SurName != "")
                            {
                                StringType.MidStmtStr(ref text, 17, 15, Modul1.Person.SurName);
                            }
                            else
                            {
                                StringType.MidStmtStr(ref text, 17, 15, ">NN<");
                            }
                        }
                        Application.DoEvents();
                        _ = List1.Items.Add(new ListItem(text + "          " + Modul1.FamInArb.AsString()));
                    }
                }
                M1_Iter++;
            }
            Label4.Text = $"{(List1.Items.Count - 2).AsString()} Einträge";
        }
    }

    private void Button3_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_06ac
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        string sDest = default;
        int Satz = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int M1_Iter = default;
                    int num4;
                    string Wort;
                    ETextKennz tKennz;
                    int i3;
                    object[] array;
                    //Field field;
                    //Field field2;
                    //Field field3;
                    Type typeFromHandle;
                    Type typeFromHandle2;
                    Type typeFromHandle3;
                    object[] array2;
                    bool[] array3;
                    string left;
                    object left2;
                    object[] array4;
                    object[] array5;
                    bool[] array6;
                    string left3;
                    object[] array7;
                    object[] array8;
                    bool[] array9;
                    object obj;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0009;
                        case 2708:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_ResumeNext;
                                    default:
                                        goto end_IL_0001;
                                }
                                goto IL_ErrHndl;
                            }
                        IL_ResumeNext:
                            num4 = unchecked(num2 + 1);
                            goto IL_Restart;
                        IL_ErrHndl:
                            num = 70;
                            if (Information.Err().Number == 13)
                            {
                                Cursor.Current = Cursors.WaitCursor;
                                _ = Interaction.MsgBox("Die Religionseinträge werden jetzt bearbeitet. Vermutlich wurde der Mandant mit einer älteren Programmversion bearbeitet. Dieser Vorgang kann einige Minuten dauern.");
                                DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
                                DataModul.DB_PersonTable.MoveLast();
                                int num5 = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                                DataModul.DB_PersonTable.MoveFirst();
                                int num6 = num5;
                                M1_Iter = 1;
                                while (M1_Iter <= num6)
                                {
                                    DataModul.DB_PersonTable.Edit();
                                    if (Strings.Trim(DataModul.DB_PersonTable.Fields[PersonFields.Konv].AsString()) != "")
                                    {
                                        var field = DataModul.DB_PersonTable.Fields[PersonFields.Konv];
                                        Wort = field.AsInt().AsString();
                                        tKennz = ETextKennz.tk7_;
                                        Satz = DataModul.Texte_Schreib(Wort, Modul1.UbgT1, tKennz);
                                        field.Value = Wort;
                                        DataModul.DB_PersonTable.Fields[PersonFields.religi].Value = Satz;
                                        Satz = 0;
                                    }
                                    else
                                    {
                                        DataModul.DB_PersonTable.Fields[PersonFields.religi].Value = Satz;
                                        Satz = 0;
                                    }
                                    DataModul.DB_PersonTable.Update();
                                    DataModul.DB_PersonTable.MoveNext();
                                    M1_Iter++;
                                }
                                _ = Interaction.MsgBox("Fertig");
                                Modul1.Reli = false;
                                Cursor.Current = Cursors.Default;
                                goto end_IL_0001_2;
                            }
                            else
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_Restart;
                            }

                        IL_Restart:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_0009;
                                case 70:
                                    goto IL_ErrHndl;
                                case 95:
                                    goto IL_08b8;
                                default:
                                    goto end_IL_0001;
                                case 14:
                                case 20:
                                case 24:
                                case 69:
                                case 94:
                                case 99:
                                    goto end_IL_0001_2;
                            }
                            goto default;

                        IL_08b8:
                            num = 95;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_Restart;


                        IL_0009:
                            num = 2;
                            RadioButton1.Visible = true;
                            RadioButton2.Visible = true;
                            List1.Items.Clear();
                            _Label1_2.Text = "Fehlliste Personen";
                            _Label1_1.Text = Modul1.IText[EUserText.t166];
                            _Label1_0.Text = Modul1.IText[EUserText.t167];
                            List1.Items.Clear();
                            I1 = 0;
                            while (I1 <= 2)
                            {
                                Label1[(short)I1].Refresh();
                                I1++;
                            }
                            int num8 = 1;
                            if (num8 <= 0)
                            {
                                goto end_IL_0001_2;
                            }
                            DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
                            DataModul.DB_PersonTable.MoveLast();
                            int num9 = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                            if (num8 == 0)
                            {
                                goto end_IL_0001_2;
                            }
                            if (num8 <= num9)
                            {
                                _ = Interaction.MsgBox($"Die höchste Personennummer ist {num9}");
                                goto end_IL_0001_2;
                            }
                            ProgressBar1.Minimum = 0;
                            ProgressBar1.Maximum = 0;
                            ProgressBar1.Step = 1;
                            ProgressBar1.Maximum = DataModul.DB_PersonTable.RecordCount - 1;
                            List1.Items.Clear();
                            if (RadioButton2.Checked)
                            {
                                _ = List1.Items.Add("Fehlliste              Konfession und Geschlecht");
                            }
                            i3 = num8;
                            int num10 = DataModul.DB_PersonTable.RecordCount - 1;
                            I1 = i3;
                            while (I1 <= num10)
                            {
                                ProgressBar1.PerformStep();
                                Application.DoEvents();
                                Modul1.PersInArb = I1;
                                if (Modul1.PersInArb <= num9)
                                {
                                    DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb);
                                    if (!DataModul.DB_PersonTable.NoMatch)
                                    {
                                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                        Modul1_LiText = (Modul1.Person.SurName.Trim() + "," + Modul1.Person.Givennames.Trim()).PadRight(20, ' ');
                                        Modul1.Schalt = 20;
                                        T = 1;
                                        while (T <= 10)
                                        {
                                            Modul1.Kont[T + 10] = " ";
                                            Modul1.Kont[T + 20] = " ";
                                            T++;
                                        }
                                        sDest = "                                                                            ";
                                        array = new object[1];
                                        array[0] = DataModul.DB_PersonTable.Fields[PersonFields.Sex].Value;
                                        array2 = array;
                                        array3 = new bool[1] { true };
                                        left = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString().ToUpper();
                                        if (array3[0])
                                        {
                                            DataModul.DB_PersonTable.Fields[PersonFields.Sex].Value = array2[0];
                                        }
                                        left2 = left != "M";
                                        array4 = new object[1];
                                        array4[0] = DataModul.DB_PersonTable.Fields[PersonFields.Sex].Value;
                                        array5 = array4;
                                        array6 = new bool[1] { true };
                                        left3 = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString().ToUpper();
                                        if (array6[0])
                                        {
                                            DataModul.DB_PersonTable.Fields[PersonFields.Sex].Value = array5[0];
                                        }
                                        if ((bool)left2 & (left3 != "F"))
                                        {

                                            array7 = new object[1];
                                            var field = DataModul.DB_PersonTable.Fields[PersonFields.Sex];
                                            array7[0] = field.Value;
                                            array8 = array7;
                                            array9 = new bool[1] { true };
                                            obj = field.AsString().ToUpper();
                                            if (array9[0])
                                            {
                                                field.Value = array8[0];
                                            }
                                            sDest = obj.AsString();
                                        }
                                        if (DataModul.DB_PersonTable.Fields[PersonFields.religi].AsInt() == 0)
                                        {
                                            sDest = "F";
                                        }
                                        if (sDest.Trim() != "")
                                        {
                                            Modul1_LiText += sDest;
                                            if (Modul1_LiText.Trim() != "")
                                            {
                                                _ = List1.Items.Add(new ListItem(Modul1_LiText + "          " + Modul1.PersInArb, Modul1.PersInArb));
                                            }
                                        }
                                        Modul1_LiText = "";
                                    }
                                    lErl = 5;
                                    I1++;
                                }
                                else
                                    break;
                            }
                            lErl = 4;
                            goto end_IL_0001_2;
                        end_IL_0001:
                            break;
                    }
                }
            }
            catch (Exception obj2) when (obj2 is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj2, lErl);
                try0001_dispatch = 2708;
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

    private void Button4_Click(object sender, EventArgs e)
    {
        Button8.Visible = false;
        Button5.Visible = true;
        GroupBox4.Visible = false;
        GroupBox3.Visible = true;
        GroupBox2.Visible = true;
    }

    private void Button5_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_0739, IL_0993
        int try0001_dispatch = -1;
        int num2 = default;
        int lErl = default;
        int num = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int M1_Iter = default;
                    switch (try0001_dispatch)
                    {
                        default:
                            {
                                ProjectData.ClearProjectError();
                                num2 = 2;
                                //Sz = 0;
                                List1.Items.Clear();
                                if (RadioButton1.Checked)
                                {
                                    List1.Sorted = true;
                                }
                                if (RadioButton2.Checked)
                                {
                                    List1.Sorted = false;
                                }

                                _Label1_2.Text = "Familien ohne Datum";
                                _Label1_1.Text = "Mann            Frau       Kinder  ";
                                _Label1_0.Text = "                                  ";
                                M1_Iter = 0;
                                while (M1_Iter <= 2)
                                {
                                    Label1[(short)M1_Iter++].Refresh();
                                }
                                int num4 = 1;
                                if (num4 <= 0)
                                {
                                    break;
                                }
                                DataModul.DB_FamilyTable.MoveLast();
                                int iMaxFamNr = DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();
                                if (num4 > iMaxFamNr)
                                {
                                    _ = Interaction.MsgBox(Modul1.IText[EUserText.t174] + " " + num4.AsString() + Modul1.IText[EUserText.t172]);
                                    break;
                                }

                                ProgressBar1.Minimum = 0;
                                ProgressBar1.Maximum = 0;
                                ProgressBar1.Step = 1;
                                ProgressBar1.Maximum = Modul1.Aus[13].AsInt();
                                int num6 = num4 + iMaxFamNr;
                                var I1 = num4;
                                while (I1 <= num6)
                                {
                                    ProgressBar1.PerformStep();
                                    string sDest = new string(' ', 90);
                                    if (I1 <= iMaxFamNr)
                                    {
                                        DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
                                        DataModul.DB_FamilyTable.Seek("=", I1.AsString());
                                        if (!DataModul.DB_FamilyTable.NoMatch)
                                        {
                                            bool Family_xAeb = DataModul.DB_FamilyTable.Fields[FamilyFields.Aeb].AsBool();

                                            if (Family_xAeb)
                                            {
                                                StringType.MidStmtStr(ref sDest, 70, 1, Modul1.IText[EUserText.t175]);
                                            }
                                            int num7 = 499;
                                            IEventData Event = null;
                                            while (++num7 <= 507)
                                            {
                                                if (!(num7 == 500 & !CheckBox6.Checked)
                                                    && !(num7 == 501 & !CheckBox5.Checked)
                                                    && !(num7 == 502 & !CheckBox7.Checked)
                                                    && !(num7 == 503 & !CheckBox8.Checked)
                                                    && !(num7 == 504 & !CheckBox9.Checked)
                                                    && !(num7 == 505 & !CheckBox10.Checked)
                                                    && !(num7 == 507 & !CheckBox11.Checked)
                                                    && DataModul.Event.ReadData((EEventArt)num7, I1, out Event, 0))
                                                {
                                                    break;
                                                }
                                            }

                                            if (num7 > 507
                                                || (!CheckBox1.Checked
                                                || !(Event.dDatumV.AsDouble() > 0.0)
                                                || Event.dDatumV.AsString().Right(2) != "00"
                                                    && Event.dDatumV.AsString().Right(4) != "0000")
                                                    && (!CheckBox2.Checked || !(Event.iOrt == 0))
                                                    && (!CheckBox3.Checked || Event.sReg.AsString().Trim() != "") && (!CheckBox4.Checked || (null == Event.sBem[3]
                                                            || Event.sBem[3].Trim() != "")
                                                            && DataModul.SourceLink_Exists(3, Modul1.Nr, (EEventArt)Modul1.Ubg, Modul1.LfNR)))
                                            {
                                                ProgressBar1.PerformStep();
                                                Application.DoEvents();
                                                this.I1++;
                                                continue;
                                            }
                                        }
                                        lErl = 34;
                                        DataModul.Link.ReadFamily(I1, Modul1.Family);
                                        if (Modul1.Family.Mann > 0)
                                        {
                                            Modul1.Person_ReadNames(Modul1.Family.Mann, Modul1.Person);
                                            if (Modul1.Person.SurName != "")
                                            {
                                                StringType.MidStmtStr(ref sDest, 1, 15, Modul1.Person.SurName);
                                            }
                                            else
                                            {
                                                StringType.MidStmtStr(ref sDest, 1, 15, ">NN<");
                                            }
                                            StringType.MidStmtStr(ref sDest, 16, 2, "/ ");
                                        }
                                        if (Modul1.Family.Frau > 0)
                                        {
                                            Modul1.Person_ReadNames(Modul1.Family.Frau, Modul1.Person);
                                            if (Modul1.Person.SurName != "")
                                            {
                                                StringType.MidStmtStr(ref sDest, 17, 15, Modul1.Person.SurName);
                                            }
                                            else
                                            {
                                                StringType.MidStmtStr(ref sDest, 17, 15, ">NN<");
                                            }
                                        }
                                        if (Modul1.Family.Kind[1] != 0)
                                        {
                                            StringType.MidStmtStr(ref sDest, 32, 1, Modul1.IText[EUserText.t175]);
                                        }
                                        if (!DataModul.Event.Exists(EEventArt.eA_601, I1) && sDest.Trim() != "")
                                        {
                                            _ = List1.Items.Add(new ListItem(sDest + "          " + I1.AsString()));
                                            //Sz++;
                                        }
                                    }
                                    ProgressBar1.PerformStep();
                                    Application.DoEvents();
                                    this.I1++;
                                }
                                Label4.Text = List1.Items.Count.AsString() + " Einträge";
                                GroupBox2.Visible = false;
                                break;
                            }
                        case 2455:
                            num = -1;
                            switch (num2)
                            {
                                case 2:
                                    break;
                                default:
                                    goto IL_09d3;
                            }
                            break;
                    }
                }
            }
            catch (Exception obj) when (num2 != 0 && num == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 2455;
                continue;
            }
            break;
        IL_09d3:
            throw ProjectData.CreateProjectError(-2146828237);
        }
        if (num != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void Button6_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_040e, IL_0461, IL_04cc
        ListBox_SetTabStop(List1, 100, 230, 260, 300);
        Label4.Text = "";
        if (RadioButton1.Checked)
        {
            List1.Sorted = true;
        }
        if (RadioButton2.Checked)
        {
            List1.Sorted = false;
        }
        List1.Items.Clear();
        _Label1_2.Text = "lebende Personen";
        int num = 1;
        if (num <= 0)
        {
            return;
        }
        DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
        DataModul.DB_PersonTable.MoveLast();
        int num2 = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
        if (num == 0)
        {
            return;
        }
        if (num > DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt())
        {
            _ = Interaction.MsgBox(Modul1.IText[EUserText.t173] + " " + num.AsString() + Modul1.IText[EUserText.t172]);
            return;
        }
        ProgressBar1.Minimum = 0;
        ProgressBar1.Maximum = 0;
        ProgressBar1.Step = 1;
        checked
        {
            ProgressBar1.Maximum = DataModul.DB_PersonTable.RecordCount - 1;
            int num3 = num2;
            I1 = num;
            while (I1 <= num3)
            {
                ProgressBar1.PerformStep();
                Application.DoEvents();
                Modul1.PersInArb = I1;
                if (Modul1.PersInArb > num2)
                {
                    return;
                }
                DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb);
                if (!DataModul.DB_PersonTable.NoMatch)
                {
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    Modul1_LiText = Modul1.Person.SurName + " \t" + Modul1.Person.Givennames.Trim() + "\t";
                    T = 1;
                    while (T <= 10)
                    {
                        Modul1.Kont[T + 10] = "";
                        Modul1.Kont[T + 20] = "";
                        T++;
                    }
                    DataModul.Event.PersLebDatles(Modul1.PersInArb, Modul1.Person);
                    string inputStr = DateTime.Now.ToString("yyyyMMdd");
                    if (Modul1.Person.Death.Trim() == "" && Modul1.Person.Burial.Trim() == "")
                    {
                        if (Modul1.Person.Birthday.Trim() == "" && Modul1.Person.Baptised.Trim() == "")
                        {
                            Modul1_LiText += "          ";
                        }
                        else if (Modul1.Person.Birthday.Trim() != "")
                        {
                            if (Modul1.Person.Birthday.AsDouble() < inputStr.AsInt() - 1040000.0)
                            {
                                Modul1.sDatu = Modul1.Person.Birthday;
                                Modul1.sDatu = Modul1.sDatu.Date2DotDateStr();
                                var s = Modul1.sDatu;
                                Modul1_LiText = Modul1_LiText + " " + s.Trim();
                            }
                        }
                        else if (Modul1.Person.Baptised.Trim() != "")
                        {
                            if (Modul1.Person.Baptised.AsDouble() < inputStr.AsInt() - 1040000.0)
                            {
                                Modul1.sDatu = Modul1.Person.Baptised;
                                Modul1.sDatu = Modul1.sDatu.Date2DotDateStr();
                                var s = Modul1.sDatu;
                                Modul1_LiText = Modul1_LiText + s + "(t)";
                            }
                        }
                        else
                        {
                            Modul1_LiText = $"{Modul1_LiText}\t{$"          {Modul1.PersInArb}".Right(10)}";
                            if (Modul1_LiText.Trim() != "")
                            {
                                _ = List1.Items.Add(new ListItem(Modul1_LiText));
                            }
                        }
                    }
                }
                Modul1_LiText = "";
                I1++;
            }
            Label4.Text = $"{(List1.Items.Count - 1).AsString()} Einträge";
        }
    }

    public void ListBox_SetTabStop(ListBox oList, params int[] nTabs)
    {
        int wParam = checked(nTabs.Length + 1);
        _ = SendMessage(oList.Handle.ToInt32(), 402, wParam, ref nTabs[0]);
    }

    [DllImport("user32", CharSet = CharSet.Ansi, EntryPoint = "SendMessageA", ExactSpelling = true, SetLastError = true)]
    private static extern int SendMessage(int hwnd, int wMsg, int wParam, ref int lParam);

    private void Button8_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_09fd, IL_0b0d
        int try0001_dispatch = -1;
        int num2 = default;
        int lErl = default;
        int num = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int M1_Iter = default;
                    switch (try0001_dispatch)
                    {
                        default:
                            {
                                ProjectData.ClearProjectError();
                                num2 = 2;
                                //Sz = 0;
                                List1.Items.Clear();
                                if (RadioButton1.Checked)
                                {
                                    List1.Sorted = true;
                                }
                                if (RadioButton2.Checked)
                                {
                                    List1.Sorted = false;
                                }
                                _Label1_2.Text = "Personen mit unvollständigem Datum Datum";
                                _Label1_1.Text = "  ";
                                _Label1_0.Text = "                                  ";
                                M1_Iter = 0;
                                int i;
                                int num3;
                                do
                                {
                                    Label1[(short)M1_Iter].Refresh();
                                    M1_Iter++;
                                    i = M1_Iter;
                                    num3 = 2;
                                }
                                while (i <= num3);
                                int num4 = 1;
                                if (num4 <= 0)
                                {
                                    break;
                                }
                                DataModul.DB_PersonTable.MoveLast();
                                int num5 = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                                if (num4 > DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt())
                                {
                                    _ = Interaction.MsgBox(Modul1.IText[EUserText.t174] + " " + num4.AsString() + Modul1.IText[EUserText.t172]);
                                    break;
                                }
                                ProgressBar1.Minimum = 0;
                                ProgressBar1.Maximum = 0;
                                ProgressBar1.Step = 1;
                                ProgressBar1.Maximum = num5;
                                int num6 = num4 + num5;
                                I1 = num4;
                                while (true)
                                {
                                    int i2 = I1;
                                    num3 = num6;
                                    if (i2 > num3)
                                    {
                                        break;
                                    }
                                    ProgressBar1.PerformStep();
                                    string text = "                                                                                          ";
                                    Modul1.PersInArb = I1;
                                    if (Modul1.PersInArb > num5)
                                    {
                                        goto IL_0aa9;
                                    }
                                    DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
                                    DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb);
                                    int num7;
                                    if (!DataModul.DB_PersonTable.NoMatch)
                                    {
                                        DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
                                        DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb);
                                        if (!DataModul.DB_PersonTable.NoMatch)
                                        {
                                            IEventData cEvt2 = null;
                                            num7 = 101;
                                            while (num7 <= 105)
                                            {
                                                if (!(num7 == 101 & !CheckBox22.Checked) && !(num7 == 102 & !CheckBox21.Checked) && !(num7 == 103 & !CheckBox20.Checked) && !(num7 == 104 & !CheckBox19.Checked) && !(num7 == 105 & !CheckBox15.Checked))
                                                {
                                                    Modul1.Ubg = num7;
                                                    if (!DataModul.Event.ReadData((EEventArt)num7, Modul1.PersInArb, out cEvt2))
                                                    {
                                                        break;
                                                    }
                                                }
                                                lErl = 2;
                                                num7++;
                                                goto IL_068b;
                                            }
                                            if ((!CheckBox1.Checked
                                                || (cEvt2.dDatumV == default)
                                                || cEvt2.dDatumV.Day > 0
                                                  && cEvt2.dDatumV.Year > 0)
                                                && (!CheckBox2.Checked
                                                    || !(cEvt2.iOrt == 0))
                                                && (!CheckBox3.Checked
                                                    || cEvt2.sReg.Trim() != ""))
                                            {
                                                if (!CheckBox4.Checked)
                                                {
                                                    goto IL_0aa9;
                                                }
                                                if ("" == cEvt2.sBem[3]
                                                    || cEvt2.sBem[3].Trim() != "")
                                                {
                                                    DataModul.DB_SourceLinkTable.Index = "Tab22";
                                                    DataModul.DB_SourceLinkTable.Seek("=", 3, Modul1.PersInArb, Modul1.Ubg, Modul1.LfNR);
                                                    if (!DataModul.DB_SourceLinkTable.NoMatch)
                                                    {
                                                        goto IL_0aa9;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    goto IL_09fe;
                                IL_068b:
                                    IEventData cEvt = null;
                                    num7 = 300;
                                    while (num7 <= 302)
                                    {
                                        if (!(num7 == 300 & !CheckBox18.Checked)
                                            && !(num7 == 301 & !CheckBox17.Checked)
                                            && !(num7 == 302 & !CheckBox16.Checked))
                                        {
                                            Modul1.Ubg = num7;
                                            if (DataModul.Event.ReadData((EEventArt)num7, Modul1.PersInArb, out cEvt))
                                            {
                                                break;
                                            }
                                        }
                                        lErl = 3;
                                        num7++;
                                        goto IL_0aa9;
                                    }
                                    if (cEvt != null
                                        && (!CheckBox1.Checked
                                        || cEvt.dDatumV == default
                                        || cEvt.dDatumV.Day != 0
                                        && cEvt.dDatumV.Year != 0)
                                        && (!CheckBox2.Checked
                                        || !(cEvt.iOrt == 0))
                                        && (!CheckBox3.Checked
                                        || cEvt.sReg.Trim() != ""))
                                    {
                                        if (!CheckBox4.Checked)
                                        {
                                            goto IL_0aa9;
                                        }
                                        if ("" == cEvt.sBem[3]
                                            || cEvt.sBem[3].Trim() != "")
                                        {
                                            DataModul.DB_SourceLinkTable.Index = "Tab22";
                                            DataModul.DB_SourceLinkTable.Seek("=", 3, Modul1.Nr, Modul1.Ubg, Modul1.LfNR);
                                            if (!DataModul.DB_SourceLinkTable.NoMatch)
                                            {
                                                goto IL_0aa9;
                                            }
                                        }
                                    }
                                    goto IL_09fe;
                                IL_09fe:
                                    lErl = 34;
                                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                    if (Modul1.Person.SurName != "")
                                    {
                                        text = Strings.Left(Modul1.Person.SurName + ", " + Modul1.Person.Givennames + new string(' ', 80), 80);
                                    }
                                    if (text.Trim() != "")
                                    {
                                        _ = List1.Items.Add(new ListItem(text + Modul1.PersInArb));
                                        //Sz++;
                                    }
                                    goto IL_0aa9;
                                IL_0aa9:
                                    lErl = 35;
                                    I1++;
                                }
                                Label4.Text = List1.Items.Count.AsString() + " Einträge";
                                GroupBox2.Visible = false;
                                break;
                            }
                        case 2833:
                            num = -1;
                            switch (num2)
                            {
                                case 2:
                                    break;
                                default:
                                    goto IL_0b4d;
                            }
                            break;
                    }
                }
            }
            catch (Exception obj) when (num2 != 0 && num == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 2833;
                continue;
            }
            break;
        IL_0b4d:
            throw ProjectData.CreateProjectError(-2146828237);
        }
        if (num != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void Button7_Click(object sender, EventArgs e)
    {
        RadioButton1.Visible = true;
        RadioButton2.Visible = true;
        Button5.Visible = false;
        Button8.Visible = true;
        GroupBox4.Visible = true;
        GroupBox3.Visible = false;
        GroupBox2.Visible = true;
    }

    private void Event_Set(int persInArbsp, object eArt, DateTime transdat, string sDateV_S)
    {

        var dB_EventTable = DataModul.DB_EventTable;
        dB_EventTable.Index = nameof(EventIndex.ArtNr);
        dB_EventTable.Seek("=", eArt, persInArbsp, 0);
        if (dB_EventTable.NoMatch)
        {
            dB_EventTable.AddNew();
            dB_EventTable.Fields[EventFields.KBem].Value = 0;
            dB_EventTable.Fields[EventFields.Art].Value = eArt;
            dB_EventTable.Fields[EventFields.LfNr].Value = "0";
            dB_EventTable.Fields[EventFields.PerFamNr].Value = persInArbsp;
            dB_EventTable.Fields[EventFields.DatumV].Value = transdat.AsInt();
            dB_EventTable.Fields[EventFields.DatumV_S].Value = sDateV_S;
            dB_EventTable.Fields[EventFields.DatumB].Value = 0;
            dB_EventTable.Fields[EventFields.DatumB_S].Value = " ";
            dB_EventTable.Fields[EventFields.Ort].Value = 0;
            dB_EventTable.Fields[EventFields.Ort_S].Value = "";
            dB_EventTable.Fields[EventFields.Bem1].Value = " ";
            dB_EventTable.Fields[EventFields.Bem2].Value = " ";
            dB_EventTable.Fields[EventFields.Bem3].Value = " ";
            dB_EventTable.Fields[EventFields.Platz].Value = 0;
            dB_EventTable.Fields[EventFields.Reg].Value = " ";
            dB_EventTable.Fields[EventFields.Hausnr].Value = 0;
        }
        else
        {
            dB_EventTable.Edit();
            dB_EventTable.Fields[EventFields.DatumV].Value = transdat.AsInt();
            dB_EventTable.Fields[EventFields.DatumV_S].Value = sDateV_S;
        }
        dB_EventTable.Update();
    }

    /// <summary>
    /// Button9s the click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void Button9_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_0e31, IL_24dd
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        int num5 = default;
        bool flag = default;
        short num6 = default;
        string name = default;
        int num8 = default;
        int iMaxPersID = default;
        int num10 = default;
        string sDest = default;
        short num12 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    int i2;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0009;
                        case 11139:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_2535;
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
                                goto IL_2539;
                            }
                        end_IL_0001:
                            break;
                        IL_0009:
                            num = 2;
                            RadioButton1.Visible = false;
                            RadioButton2.Visible = false;
                            num5 = 30000000;
                            flag = false;
                            num6 = 0;
                            List1.Items.Clear();
                            name = ((Button)sender).Name;
                            if (name == nameof(Button9))
                            {
                                flag = true;
                                _Label1_2.Text = "Personen deren Daten nicht ergänzt werden können.";
                            }
                            else if (name == nameof(Button10))
                            {
                                num6 = 1;
                            }

                            List1.Items.Clear();
                            _Label1_1.Text = Modul1.IText[EUserText.t166];
                            _Label1_0.Text = Modul1.IText[EUserText.t167];
                            I1 = 0;
                            if (I1 <= 2)
                            {
                                Label1[(short)I1].Refresh();
                                I1++;
                            }
                            num8 = 1;
                            if (num8 <= 0)
                            {
                                goto end_IL_0001_2;
                            }
                            DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
                            DataModul.DB_PersonTable.MoveLast();
                            iMaxPersID = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                            if (num8 == 0)
                            {
                                goto end_IL_0001_2;
                            }
                            if (num8 > iMaxPersID)
                            {
                                _ = Interaction.MsgBox(Modul1.IText[EUserText.t173] + " " + num8.AsString() + Modul1.IText[EUserText.t172]);
                                goto end_IL_0001_2;
                            }
                            else
                            {
                                ProgressBar1.Minimum = 0;
                                ProgressBar1.Maximum = 0;
                                ProgressBar1.Step = 1;
                                ProgressBar1.Maximum = DataModul.DB_PersonTable.RecordCount - 1;
                                i2 = num8;
                                num10 = DataModul.DB_PersonTable.RecordCount - 1;
                                I1 = i2;
                                goto IL_24ae;
                            }

                        IL_0ebd: // <========== 3
                            num = 178;
                            lErl = InnerLoop(flag);
                            goto IL_248d;
                        //==========
                        IL_248d: // <========== 14
                            num = 391;
                            lErl = 95;
                            I1++;
                            goto IL_24ae;
                        IL_24ae:
                            while (I1 <= num10)
                            {
                                ProgressBar1.PerformStep();
                                Application.DoEvents();
                                Modul1.PersInArb = I1;
                                if (Modul1.PersInArb > iMaxPersID)
                                {
                                    goto end_IL_0001_2;
                                }
                                DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb);
                                if (!DataModul.DB_PersonTable.NoMatch)
                                {
                                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                    Modul1_LiText = Strings.Left(Modul1.Person.SurName.Trim() + "," + Modul1.Person.Givennames.Trim() + "                                  ", 20);
                                    var (sDat_Birth, sDat_Death) = Modul1.Datles(Modul1.PersInArb, Modul1.Person, true);
                                    if (num6 == 1)
                                    {
                                        if (sDat_Death == ""
                                            && sDat_Birth == "")
                                        {
                                            var pt = DataModul.Person.Seek(Modul1.PersInArb);
                                            pt.Edit();
                                            pt.Fields[PersonFields.OFB].Value = "J";
                                            pt.Update();
                                        }
                                    }
                                    else
                                    {
                                        sDest = "                                                                            ";
                                        if (Modul1.Person.Birthday.Trim() == ""
                                            && Modul1.Person.Baptised.Trim() == ""
                                            && Modul1.Person.Death.Trim() == ""
                                            && Modul1.Person.Burial.Trim() == "")
                                        {
                                            num12 = 300;
                                            while (num12 <= 302)
                                            {
                                                if (!DataModul.Event.ReadData((EEventArt)num12, Modul1.PersInArb, out var cEvt))
                                                {
                                                    int iBStrPos = 34;
                                                    TimeSpan tsV = new TimeSpan(365 * 30, 0, 0, 0, 0);
                                                    TimeSpan tsB = new TimeSpan(365 * 50, 0, 0, 0, 0);
                                                    int iPlStrPos = 38;

                                                    switch (num12)
                                                    {
                                                        case 105: //??

                                                            iBStrPos = 34;
                                                            tsV = new TimeSpan(365 * 15, 0, 0, 0, 0);
                                                            tsB = new TimeSpan(365 * 15, 0, 0, 0, 0);
                                                            iPlStrPos = 38;

                                                            break;
                                                        case 300:

                                                            iBStrPos = 34;
                                                            iPlStrPos = 38;


                                                            break;
                                                        case 301:

                                                            iBStrPos = 42;
                                                            iPlStrPos = 46;

                                                            break;
                                                        case 302:
                                                            iBStrPos = 50;
                                                            iPlStrPos = 53;
                                                            break;
                                                    }
                                                    if (cEvt.iKBem > 0)
                                                    {
                                                        StringType.MidStmtStr(ref sDest, iBStrPos, 1, Modul1.IText[EUserText.t175]);
                                                    }
                                                    if (cEvt.dDatumV != default)
                                                    {
                                                        transdat = cEvt.dDatumV.Subtract(tsV);
                                                    }
                                                    else if (cEvt.dDatumB != default)
                                                    {
                                                        transdat = cEvt.dDatumB.Subtract(tsB);
                                                    }
                                                    if (cEvt.dDatumV != default || cEvt.dDatumB != default)
                                                    {
                                                        lErl = 921;
                                                        if (cEvt.dDatumV != default)
                                                        {
                                                            Event_Set(Modul1.PersInArb, EEventArt.eA_Birth, transdat, "C");
                                                            I1++;
                                                            goto IL_24ae;
                                                        }
                                                        break;

                                                    }
                                                    if (cEvt.iOrt > 0)
                                                    {
                                                        StringType.MidStmtStr(ref sDest, iPlStrPos, 1, Modul1.IText[EUserText.t175]);
                                                    }

                                                }
                                                lErl = 92;
                                                num12++;
                                            }
                                            goto IL_0ebd;
                                        }
                                    }
                                }
                                lErl = 95;
                                I1++;

                            }

                            Label4.Text = "Fertig";

                            goto end_IL_0001_2;
                        IL_2535:
                            num4 = unchecked(num2 + 1);
                            goto IL_2539;
                        IL_2539:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;

                                case 170:
                                case 177:
                                case 178:
                                    goto IL_0ebd;
                                case 52:
                                case 66:
                                case 69:
                                case 77:
                                case 78:
                                case 79:
                                case 80:
                                case 84:
                                case 87:
                                case 90:
                                case 93:
                                case 176:
                                case 185:
                                case 218:
                                case 233:
                                case 259:
                                case 293:
                                case 318:
                                case 335:
                                case 352:
                                case 376:
                                case 383:
                                case 391:
                                    goto IL_248d;
                                case 27:
                                case 33:
                                case 37:
                                case 48:
                                case 394:
                                case 399:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 11139;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 6
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }

        int InnerLoop(bool flag)
        {
            int lErl = 923;
            DateTime dateTime;
            int famInArb = Modul1.FamInArb;
            if (flag)
            {
                var pt = DataModul.Person.Seek(Modul1.PersInArb);
                if (pt.Fields[PersonFields.OFB].AsString() != "J")
                {
                    Modul1_PersInArbsp = Modul1.PersInArb;
                    Modul1.eLKennz = pt.Fields[PersonFields.Sex].AsString().ToUpper() == "F" ? ELinkKennz.lkMother : ELinkKennz.lkFather;

                    if (DataModul.Link.GetPersonFam(Modul1.PersInArb, Modul1.eLKennz, out famInArb))
                    {
                        DataModul.Link.ReadFamily(famInArb, Modul1.Family);
                        if (Modul1.eLKennz == ELinkKennz.lkFather)
                        {
                            Modul1.PersInArb = Modul1.Family.Frau;
                        }
                        else if (Modul1.eLKennz == ELinkKennz.lkMother)
                        {
                            Modul1.PersInArb = Modul1.Family.Mann;
                        }

                        var M1_Iter = 101;
                        while (M1_Iter <= 104)
                        {
                            if ((dateTime = DataModul.Event.GetDate((EEventArt)M1_Iter, Modul1.PersInArb)) != default)
                            {
                                var tsV = M1_Iter is 103 or 104 ? new TimeSpan(365 * 50, 0, 0, 0, 0) : new TimeSpan(0, 0, 0, 0, 0);
                                transdat = dateTime.Subtract(tsV);
                                Event_Set(Modul1_PersInArbsp, EEventArt.eA_Birth, transdat, "C");
                                return lErl;
                            }
                            lErl = 6;
                            M1_Iter++;
                        }

                        // Schätze Geburtsdatum über Kinder
                        var num5 = 3000;
                        A = 1;
                        while (A <= 99
                            && Modul1.Family.Kind[A] > 0)
                        {
                            M1_Iter = 101;
                            while (M1_Iter <= 102)
                            {
                                if ((dateTime = DataModul.Event.GetDate((EEventArt)M1_Iter, Modul1.Family.Kind[A])) != default
                                    && dateTime.Year < num5)
                                {
                                    num5 = dateTime.Year;
                                }
                                M1_Iter++;
                            }
                            A++;
                        }
                        if (num5 < 3000)
                        {
                            transdat = new DateTime(num5 - 25, 1, 1);
                            Event_Set(Modul1_PersInArbsp, EEventArt.eA_Birth, transdat, "C");
                            return lErl;
                        }

                        // Schätze Geburtsdatum über Verlobungs und Hochzeit
                        num5 = 3000;
                        M1_Iter = 500;
                        while (M1_Iter <= 507)
                        {
                            if ((dateTime = DataModul.Event.GetDate((EEventArt)M1_Iter, famInArb)) != default
                                && dateTime.Year < num5)
                            {
                                num5 = dateTime.Year;
                            }
                            lErl = 54;
                            M1_Iter++;
                        }
                        if ((dateTime = DataModul.Event.GetDate(EEventArt.eA_601, famInArb)) != default
                            && dateTime.Year < num5)
                        {
                            num5 = dateTime.Year;
                        }
                        lErl = 53;
                        if (num5 < 3000)
                        {
                            transdat = new DateTime(num5 - 25, 1, 1);
                            Event_Set(Modul1_PersInArbsp, EEventArt.eA_Birth, transdat, "C");
                            return lErl;
                        }
                    }
                    lErl = 531;
                    if (DataModul.Link.GetPersonFam(Modul1.PersInArb, ELinkKennz.lkChild, out famInArb))
                    {
                        var num5 = 3000;
                        var M1_Iter = 500;
                        while (M1_Iter <= 507)
                        {
                            if ((dateTime = DataModul.Event.GetDate((EEventArt)M1_Iter, famInArb)) != default
                                && dateTime.Year < num5)
                            {
                                num5 = dateTime.Year;
                            }
                            lErl = 55;
                            M1_Iter++;
                        }
                        if (num5 < 3000)
                        {
                            transdat = new DateTime(num5 + 2, 1, 1);
                            Event_Set(Modul1_PersInArbsp, EEventArt.eA_Birth, transdat, "C");
                            return lErl;
                        }
                        DataModul.Link.ReadFamily(famInArb, Modul1.Family);
                        if (Modul1.Family.Mann > 0)
                        {
                            M1_Iter = 101;
                            while (M1_Iter <= 102)
                            {
                                if ((dateTime = DataModul.Event.GetDate((EEventArt)M1_Iter, Modul1.Family.Mann)) != default
                                    && dateTime.Year < num5)
                                {
                                    num5 = dateTime.Year;
                                }
                                M1_Iter++;
                            }
                            if (num5 < 3000)
                            {
                                transdat = new DateTime(num5 + 25, 1, 1);
                                Event_Set(Modul1_PersInArbsp, EEventArt.eA_Birth, transdat, "C");
                                return lErl;
                            }
                        }

                        if (Modul1.Family.Frau > 0)
                        {
                            M1_Iter = 101;
                            while (M1_Iter <= 102)
                            {
                                if ((dateTime = DataModul.Event.GetDate((EEventArt)M1_Iter, Modul1.Family.Frau)) != default
                                    && dateTime.Year < num5)
                                {
                                    num5 = dateTime.Year;
                                }
                                M1_Iter++;
                            }
                            if (num5 < 3000)
                            {
                                transdat = new DateTime(num5 + 25, 1, 1);
                                Event_Set(Modul1_PersInArbsp, EEventArt.eA_Birth, transdat, "C");
                            }
                        }
                    }
                    else
                    {
                        if (!DataModul.Link.GetPersonFam(Modul1.PersInArb, ELinkKennz.lkGodparent, out var iFam))
                        {
                            var num5 = 3000;
                            var M1_Iter = 101;
                            while (M1_Iter <= 102)
                            {
                                if ((dateTime = DataModul.Event.GetDate((EEventArt)M1_Iter, iFam)) != default
                                    && dateTime.Year < num5)
                                {
                                    num5 = dateTime.Year;
                                }
                                M1_Iter++;
                            }
                            if (num5 < 3000)
                            {
                                transdat = new DateTime(num5 + 25, 1, 1);
                                Event_Set(Modul1_PersInArbsp, EEventArt.eA_Birth, transdat, "C");
                            }
                            else
                            {
                                lErl = 45;
                                Modul1.PersInArb = Modul1_PersInArbsp;
                                var pt1 = DataModul.Person.Seek(Modul1.PersInArb);
                                if (null == pt1.Fields[PersonFields.OFB].Value
                                    || !(pt1.Fields[PersonFields.OFB].AsString() == "J"))
                                {
                                    if (Modul1_LiText.Trim() != "")
                                    {
                                        _ = List1.Items.Add(new ListItem(Modul1_LiText + "          " + Modul1_PersInArbsp.AsString()));
                                    }
                                    Modul1_LiText = "";
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Modul1.PersInArb = Modul1_PersInArbsp;
                var pt = DataModul.Person.Seek(Modul1.PersInArb);
                if (null == pt.Fields[PersonFields.OFB].Value
                    || !(pt.Fields[PersonFields.OFB].AsString() == "J"))
                {
                    if (Modul1_LiText.Trim() != "")
                    {
                        _ = List1.Items.Add(new ListItem(Modul1_LiText + "          " + Modul1_PersInArbsp.AsString()));
                    }
                    Modul1_LiText = "";
                }
            }

            return lErl;
        }

    }

    private void Button11_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_03dc
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int num5 = default;
        int num6 = default;
        int num7 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    int i;
                    switch (try0001_dispatch)
                    {
                        default:
                            {
                                num = 1;
                                RadioButton1.Visible = false;
                                RadioButton2.Visible = false;
                                List1.Items.Clear();
                                _Label1_2.Text = " Für OFB-Ausgabe gesperrte Personen";
                                num5 = 1;
                                ProjectData.ClearProjectError();
                                num3 = 2;
                                if (num5 <= 0)
                                {
                                    goto end_IL_0001_2;
                                }
                                DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
                                DataModul.DB_PersonTable.MoveLast();
                                num6 = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                                if (num5 == 0)
                                {
                                    goto end_IL_0001_2;
                                }
                                if (num5 > DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt())
                                {
                                    _ = Interaction.MsgBox(Modul1.IText[EUserText.t173] + " " + num5.AsString() + Modul1.IText[EUserText.t172]);
                                }
                                else
                                {
                                    ProgressBar1.Minimum = 0;
                                    ProgressBar1.Maximum = 0;
                                    ProgressBar1.Step = 1;
                                    ProgressBar1.Maximum = DataModul.DB_PersonTable.RecordCount - 1;
                                    i = num5;
                                    num7 = DataModul.DB_PersonTable.RecordCount - 1;
                                    I1 = i;
                                    while (I1 <= num7)
                                    {
                                        ProgressBar1.PerformStep();
                                        Application.DoEvents();
                                        Modul1.PersInArb = I1;
                                        if (Modul1.PersInArb > num6)
                                        {
                                            goto end_IL_0001_2;
                                        }
                                        DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb);
                                        if (!DataModul.DB_PersonTable.NoMatch)
                                        {
                                            if (null != DataModul.DB_PersonTable.Fields[PersonFields.OFB].Value)
                                            {
                                                if (DataModul.DB_PersonTable.Fields[PersonFields.OFB].AsString() == "J")
                                                {
                                                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                                    Modul1_LiText = Strings.Left(Modul1.Person.SurName.Trim() + "," + Modul1.Person.Givennames.Trim() + "                                  ", 30);
                                                    if (Modul1_LiText.Trim() != "")
                                                    {
                                                        _ = List1.Items.Add(new ListItem(Modul1_LiText + "          " + Modul1.PersInArb));
                                                    }
                                                    Modul1_LiText = "";
                                                }
                                            }
                                        }
                                        I1++;
                                    }
                                    Label4.Text = $"{(List1.Items.Count - 1).AsString()} Einträge";
                                }
                                goto end_IL_0001_2;
                            }
                        case 1296:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_042e;
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
                                goto IL_0432;
                            }
                        end_IL_0001:
                            break;
                        IL_042e:
                            num4 = unchecked(num2 + 1);
                            goto IL_0432;
                        IL_0432:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 9:
                                case 15:
                                case 19:
                                case 30:
                                case 47:
                                case 52:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 1296;
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
    private void Button13_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_10ac
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int num6 = default;
        int num7 = default;
        int num8 = default;
        int persInArb = default;
        string text2 = default;
        int lErl = default;
        string text3 = default;
        string text4 = default;
        string text5 = default;
        string left = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int M1_Iter = default;
                    int num4;
                    int i2;
                    int i3;
                    int num5;
                    var atList = new List<(EEventArt, int, short)>();
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0009;
                        case 5166:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_1104;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                {
                                    goto end_IL_0001_2;
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_1108;
                            }
                        end_IL_0001:
                            break;
                        IL_0009:
                            num = 2;
                            RadioButton1.Visible = true;
                            RadioButton2.Visible = true;
                            if (RadioButton1.Checked)
                            {
                                List1.Sorted = true;
                            }
                            if (RadioButton2.Checked)
                            {
                                List1.Sorted = false;
                            }
                            List1.Items.Clear();
                            _Label1_1.Text = "";
                            _Label1_2.Text = Modul1.IText[EUserText.t84_Persons];
                            _Label1_0.Text = "Person ist Zeuge bei";
                            I1 = 0;
                            while (I1 <= 2)
                            {
                                Label1[(short)I1].Refresh();
                                I1++;
                            }
                            num6 = 1;
                            if (num6 <= 0)
                            {
                                goto end_IL_0001_2;
                            }
                            DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
                            DataModul.DB_PersonTable.MoveLast();
                            num7 = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                            if (num6 == 0)
                            {
                                goto end_IL_0001_2;
                            }
                            if (num6 > DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt())
                            {
                                _ = Interaction.MsgBox("Die höchste Personennummer ist " + DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt().AsString());
                                goto end_IL_0001_2;
                            }
                            else
                            {
                                ProgressBar1.Minimum = 0;
                                ProgressBar1.Maximum = 0;
                                ProgressBar1.Step = 1;
                                ProgressBar1.Maximum = DataModul.DB_PersonTable.RecordCount - 1;
                                List1.Items.Clear();
                                i2 = num6;
                                num8 = num7;
                                I1 = i2;
                                goto IL_105e;
                            }
                        IL_05f8: // <========== 3
                            num = 58;
                            while (!DataModul.DB_WitnessTable.EOF
                                && !DataModul.DB_WitnessTable.EOF
                                && !DataModul.DB_WitnessTable.NoMatch
                                && !(DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsInt() != Modul1.PersInArb
                                || DataModul.DB_WitnessTable.Fields[WitnessFields.Kennz].AsString() != "10")
                                && !DataModul.DB_WitnessTable.NoMatch)
                            {
                                //                            text = DataModul.DB_WitnessTable.Fields[WitnessFields.Modul1.Art].AsInt().AsString() + DataModul.DB_WitnessTable.Fields[WitnessFields.Weiter].AsInt().AsString() + Strings.Right("          " + DataModul.DB_WitnessTable.Fields[WitnessFields.FamNr].AsInt().AsString(), 10);
                                atList.Add((DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsEnum<EEventArt>(),
                                    DataModul.DB_WitnessTable.Fields[WitnessFields.FamNr].AsInt(),
                                    (short)DataModul.DB_WitnessTable.Fields[WitnessFields.LfNr].AsInt()));
                                DataModul.DB_WitnessTable.MoveNext();
                            }
                            int num9 = 11;
                            Modul1_LiText = "";
                            int num10 = atList.Count;
                            M1_Iter = 1;
                            while (M1_Iter <= num10)
                            {
                                EEventArt eArt = atList[M1_Iter - 1].Item1;
                                if (eArt > EEventArt.eA_499)
                                {
                                    Modul1.FamInArb = atList[M1_Iter - 1].Item2;
                                    DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                                    Modul1.PersInArb = Modul1.Family.Mann;
                                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                    text3 = Strings.Trim(Strings.Left(Modul1.Person.SurName.Trim().ToUpper() + "," + Modul1.Person.Givennames + "                    ", 20));
                                    Modul1_LiText = Modul1_LiText + " " + text3;
                                    Modul1.PersInArb = Modul1.Family.Frau;
                                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                    text3 = Strings.Trim(Strings.Left(Modul1.Person.SurName.Trim().ToUpper() + "," + Modul1.Person.Givennames + "                    ", 20));
                                    Modul1_LiText = Modul1_LiText + " und " + text3;
                                }
                                else
                                {
                                    Modul1.PersInArb = atList[M1_Iter - 1].Item2;
                                }
                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);

                                int iLfNr = atList[M1_Iter - 1].Item3;
                                var iLink = num9 switch
                                {
                                    11 when eArt > EEventArt.eA_499 => Modul1.FamInArb,
                                    11 => Modul1.PersInArb,
                                    _ => persInArb,
                                };
                                DateTime dateTime = DataModul.Event.GetDate(eArt, iLfNr);
                                text4 = dateTime != default ? Strings.Left(dateTime.AsString(), 4) : "    ";
                                text5 = eArt switch
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
                                if (eArt.AsInt() < 499)
                                {
                                    if (Modul1.Person.Prefix.Trim() != "")
                                    {
                                        Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName.Trim());
                                    }
                                    Modul1_LiText = text2 + " Zeuge " + Strings.Left(text5 + " bei " + Modul1.Person.FullSurName.Trim().ToUpper() + ", " + Modul1.Person.Givennames.Trim() + Strings.Space(75), 63) + "          " + persInArb.AsString().Right(10);
                                }
                                else
                                {
                                    Modul1_LiText = text2 + " Zeuge " + text5 + " bei" + Modul1_LiText + Strings.Space(75).Left(63) + "          " + persInArb.AsString().Right(10);
                                }
                                _ = List1.Items.Add(Modul1_LiText);
                                Modul1_LiText = "";
                                M1_Iter++;
                            }
                            goto IL_103e;
                        IL_103e: // <========== 3
                            num = 190;
                            lErl = 5;
                            I1++;
                            goto IL_105e;
                        IL_105e:
                            i3 = I1;
                            num5 = num8;
                            if (i3 <= num5)
                            {
                                ProgressBar1.PerformStep();
                                Application.DoEvents();
                                Modul1.PersInArb = I1;
                                DataModul.DB_WitnessTable.Index = nameof(WitnessIndex.ElSu);
                                DataModul.DB_WitnessTable.Seek("=", Modul1.PersInArb, "10");
                                atList.Clear();
                                if (!DataModul.DB_WitnessTable.NoMatch)
                                {
                                    DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb);
                                    if (!DataModul.DB_PersonTable.NoMatch)
                                    {
                                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                        persInArb = Modul1.PersInArb;
                                        text2 = Strings.Left(Modul1.Person.SurName.Trim() + "," + Modul1.Person.Givennames.Trim() + "                                  ", 20);
                                        Modul1.Schalt = 20;
                                        DataModul.DB_WitnessTable.Index = nameof(WitnessIndex.ElSu);
                                        DataModul.DB_WitnessTable.Seek("=", Modul1.PersInArb, "10");
                                        atList.Clear();
                                        goto IL_05f8;
                                    }
                                }
                                goto IL_103e;
                            }
                            else
                            {
                                Label4.Text = List1.Items.Count.AsString() + " Einträge";
                            }
                            goto end_IL_0001_2;
                        IL_1104:
                            num4 = unchecked(num2 + 1);
                            goto IL_1108;
                        IL_1108:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 57:
                                case 58:
                                case 76:
                                    goto IL_05f8;
                                case 44:
                                case 48:
                                case 190:
                                    goto IL_103e;
                                case 19:
                                case 25:
                                case 29:
                                case 193:
                                case 195:
                                case 198:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 5166;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 6
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    private void Button12_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_09de
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        string text = default;
        int num6 = default;
        int num7 = default;
        int num8 = default;
        int persInArb = default;
        string text2 = default;
        int num10 = default;
        int lErl = default;
        string text3 = default;
        string text4 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int M1_Iter = default;
                    int num4;
                    int i2;
                    int i3;
                    int num5;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            RadioButton1.Visible = true;
                            goto IL_0011;
                        case 3110:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0a30;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                {
                                    goto end_IL_0001_2;
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_0a34;
                            }
                        end_IL_0001:
                            break;
                        IL_0011:
                            num = 2;
                            RadioButton2.Visible = true;
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            if (RadioButton1.Checked)
                            {
                                List1.Sorted = true;
                            }
                            if (RadioButton2.Checked)
                            {
                                List1.Sorted = false;
                            }
                            List1.Items.Clear();
                            _Label1_2.Text = Modul1.IText[EUserText.t84_Persons];
                            _Label1_1.Text = "";
                            _Label1_0.Text = "Person ist Pate bei";
                            I1 = 0;
                            while (I1 <= 2)
                            {
                                Label1[(short)I1].Refresh();
                                I1++;
                            }
                            num6 = 1;
                            if (num6 <= 0)
                            {
                                goto end_IL_0001_2;
                            }
                            DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
                            DataModul.DB_PersonTable.MoveLast();
                            num7 = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                            if (num6 == 0)
                            {
                                goto end_IL_0001_2;
                            }
                            if (num6 > DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt())
                            {
                                _ = Interaction.MsgBox("Die höchste Personennummer ist " + DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt().AsString());
                                goto end_IL_0001_2;
                            }
                            else
                            {
                                ProgressBar1.Minimum = 0;
                                ProgressBar1.Maximum = 0;
                                ProgressBar1.Step = 1;
                                ProgressBar1.Maximum = DataModul.DB_PersonTable.RecordCount - 1;
                                List1.Items.Clear();
                                i2 = num6;
                                num8 = num7;
                                I1 = i2;
                                goto IL_0993;
                            }
                        IL_0979: // <========== 3
                            num = 113;
                            lErl = 5;
                            I1++;
                            goto IL_0993;
                        IL_0993:
                            i3 = I1;
                            num5 = num8;
                            if (i3 <= num5)
                            {
                                ProgressBar1.PerformStep();
                                Application.DoEvents();
                                Modul1.PersInArb = I1;
                                var Modul1_Per1 = new List<int>();
                                if (!DataModul.Link.ExistE(Modul1.PersInArb, ELinkKennz.lkGodparent))
                                {
                                    DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb);
                                    if (!DataModul.DB_PersonTable.NoMatch)
                                    {
                                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                        persInArb = Modul1.PersInArb;
                                        text2 = Strings.Left(Modul1.Person.SurName.Trim() + "," + Modul1.Person.Givennames.Trim() + "                                  ", 20);
                                        Modul1.Schalt = 20;

                                        Modul1_Per1.AddRange(AppendParentNr(Modul1.PersInArb).Split(' ').Select((s) => s.AsInt()));

                                        Modul1_LiText = "";
                                        num10 = Modul1_Per1.Count;
                                        M1_Iter = 1;
                                        while (M1_Iter <= num10)
                                        {
                                            text = Modul1_Per1[M1_Iter - 1].AsString();
                                            if (text.AsInt() > 499.0)
                                            {
                                                Modul1.FamInArb = (int)Math.Round(Strings.Mid(text, 7, 10).AsDouble());
                                                DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                                                Modul1.PersInArb = Modul1.Family.Mann;
                                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                                text3 = Strings.Trim(Strings.Left(Modul1.Person.SurName.Trim().ToUpper() + "," + Modul1.Person.Givennames + "                    ", 20));
                                                Modul1_LiText = Modul1_LiText + " " + text3;
                                                Modul1.PersInArb = Modul1.Family.Frau;
                                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                                text3 = Strings.Trim(Strings.Left(Modul1.Person.SurName.Trim().ToUpper() + "," + Modul1.Person.Givennames + "                    ", 20));
                                                Modul1_LiText = Modul1_LiText + " und " + text3;
                                            }
                                            else
                                            {
                                                Modul1.PersInArb = (int)Math.Round(Strings.Mid(text, 7, 10).AsDouble());
                                            }
                                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                            DateTime dateTime = DataModul.Event.GetDate(EEventArt.eA_Baptism, Modul1.PersInArb);
                                            text4 = dateTime != default
                                                ? Strings.Left(dateTime.AsString(), 4)
                                                : "    ";
                                            if (Modul1.Person.Prefix.Trim() != "")
                                            {
                                                Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName.Trim());
                                            }
                                            Modul1_LiText = text2 + " Pate " + text4 + " bei " + Strings.Left(Modul1.Person.FullSurName.Trim().ToUpper() + ", " + Modul1.Person.Givennames.Trim() + Strings.Space(60), 58) + "          " + persInArb.AsString().Right(10);
                                            _ = List1.Items.Add(Modul1_LiText);
                                            Modul1_LiText = "";
                                            M1_Iter++;
                                        }

                                    }
                                }
                                goto IL_0979;
                            }
                            else
                            {
                                Label4.Text = List1.Items.Count.AsString() + " Einträge";
                            }
                            goto end_IL_0001_2;
                        IL_0a30:
                            num4 = unchecked(num2 + 1);
                            goto IL_0a34;
                        IL_0a34:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 44:
                                case 48:
                                case 113:
                                    goto IL_0979;
                                case 19:
                                case 25:
                                case 29:
                                case 116:
                                case 118:
                                case 121:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 3110;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 6
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }

        static string AppendParentNr(int persInArb1)
        {
            string text = string.Empty;
            foreach (var item in DataModul.Link.ReadAllPers(persInArb1, ELinkKennz.lkGodparent))
            {
                text += $"{item.iFamNr,10}";
            }

            return text;
        }

    }
    private void Button17_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_0a83
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
                        goto IL_0009;
                    case 3361:
                        {
                            num2 = num;
                            switch (num3 <= -2 ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0af3;
                                default:
                                    goto end_IL_0001;
                            }
                            if (Information.Err().Number != 3021)
                            {
                                break;
                            }
                            goto end_IL_0001_3;
                        }
                    end_IL_0001_2:
                        break;
                    IL_0009:
                        num = 2;
                        string text = "";
                        List1.Items.Clear();
                        if (RadioButton3.Checked)
                        {
                            text = "cEvt_iOrt";
                        }
                        else if (RadioButton4.Checked)
                        {
                            text = "?";
                        }
                        else if (RadioButton5.Checked)
                        {
                            text = "n";
                        }
                        else if (RadioButton6.Checked)
                        {
                            text = "u";
                        }
                        else if (RadioButton7.Checked)
                        {
                            text = "r";
                        }
                        else if (RadioButton8.Checked)
                        {
                            text = "a";
                        }
                        else if (RadioButton9.Checked)
                        {
                        }
                        else if (RadioButton10.Checked)
                        {
                            text = "c";
                        }
                        else if (RadioButton12.Checked)
                        {
                            text = "z";
                        }
                        else if (RadioButton13.Checked)
                        {
                            text = "b";
                        }
                        Label4.Text = " 0 Einträge";
                        var dB_EventTable = DataModul.DB_EventTable;
                        if (RadioButton16.Checked)
                        {
                            GroupBox8.Visible = false;
                            _Label1_2.Text = "Suche bei Personen Datumszusatz " + text;
                            if (RadioButton9.Checked)
                            {
                                dB_EventTable.MoveFirst();
                                while (!dB_EventTable.EOF)
                                {
                                    if (Strings.InStr(" VNURCZ?", dB_EventTable.Fields[EventFields.DatumV_S].AsString().AsString().ToUpper()) == 0)
                                    {
                                        Label4.Text = such(dB_EventTable, List1.Items);
                                    }

                                    if (Strings.InStr(" AB", dB_EventTable.Fields[EventFields.DatumB_S].AsString().AsString().ToUpper()) == 0)
                                    {
                                        Label4.Text = such(dB_EventTable, List1.Items);
                                    }
                                    dB_EventTable.MoveNext();
                                }
                            }
                            else if (RadioButton8.Checked | RadioButton13.Checked)
                            {
                                GroupBox8.Visible = false;
                                dB_EventTable.Index = nameof(EventIndex.Datbs);
                                dB_EventTable.MoveFirst();
                                dB_EventTable.Seek("=", text);
                                while (Operators.ConditionalCompareObjectEqual(dB_EventTable.Fields[EventFields.DatumB_S].AsString().AsString().ToUpper(), text.ToLower(), TextCompare: false))
                                {
                                    Label4.Text = such(dB_EventTable, List1.Items);
                                    dB_EventTable.MoveNext();
                                }
                            }
                            else
                            {
                                GroupBox8.Visible = false;
                                dB_EventTable.Index = nameof(EventIndex.Datvs);
                                dB_EventTable.MoveFirst();
                                dB_EventTable.Seek("=", text);
                                while (Operators.ConditionalCompareObjectEqual(dB_EventTable.Fields[EventFields.DatumV_S].AsString().AsString().ToUpper(), text.ToUpper(), TextCompare: false))
                                {
                                    _Label1_2.Text = "Suche bei Personen Datumszusatz " + text;
                                    Label4.Text = such(dB_EventTable, List1.Items);
                                    dB_EventTable.MoveNext();
                                }
                            }

                        }
                        else
                        {
                            _Label1_2.Text = "Suche bei Familien Datumszusatz " + text;
                            if (RadioButton9.Checked)
                            {
                                GroupBox8.Visible = false;
                                dB_EventTable.MoveFirst();
                                while (!dB_EventTable.EOF)
                                {
                                    string obj3 = dB_EventTable.Fields[EventFields.DatumV_S].AsString().AsString().ToUpper();
                                    if (~Strings.InStr(" VNURCZ?", obj3.AsString()) == 0)
                                    {
                                        Famsuch(dB_EventTable);
                                    }
                                    dB_EventTable.MoveNext();
                                    string obj4 = dB_EventTable.Fields[EventFields.DatumB_S].AsString().ToUpper();
                                    if (~Strings.InStr(" AB", obj4.AsString()) == 0)
                                    {
                                        Famsuch(dB_EventTable);
                                    }
                                    dB_EventTable.MoveNext();
                                }
                            }
                            else if (RadioButton8.Checked | RadioButton13.Checked)
                            {
                                GroupBox8.Visible = false;
                                dB_EventTable.Index = nameof(EventIndex.Datbs);
                                dB_EventTable.MoveFirst();
                                dB_EventTable.Seek("=", text);
                                while (Operators.ConditionalCompareObjectEqual(dB_EventTable.Fields[EventFields.DatumB_S].AsString().ToUpper(), text.ToUpper(), TextCompare: false))
                                {
                                    Famsuch(dB_EventTable);
                                    dB_EventTable.MoveNext();
                                }
                            }
                            else
                            {
                                GroupBox8.Visible = false;
                                dB_EventTable.Index = nameof(EventIndex.Datvs);
                                dB_EventTable.MoveFirst();
                                dB_EventTable.Seek("=", text);
                                while (Operators.ConditionalCompareObjectEqual(dB_EventTable.Fields[EventFields.DatumV_S].AsString().ToUpper(), text.ToUpper(), TextCompare: false))
                                {
                                    _Label1_2.Text = "Suche bei Familien Datumszusatz " + text;
                                    Famsuch(dB_EventTable);
                                    dB_EventTable.MoveNext();
                                }
                            }
                        }
                        goto end_IL_0001_3;

                    IL_0af3:
                        num4 = num2 + 1;
                        while (true)
                        {
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 131:
                                case 132:
                                    goto end_IL_0001_2;
                                case 134:
                                    goto IL_0acd;
                                case 53:
                                case 65:
                                case 66:
                                case 78:
                                case 79:
                                case 80:
                                case 101:
                                case 113:
                                case 114:
                                case 126:
                                case 127:
                                case 128:
                                case 130:
                                case 133:
                                case 135:
                                    goto end_IL_0001_3;
                            }
                            break;
                        IL_0acd:
                            num = 134;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                        }
                        goto default;
                }
                num = 132;
                _ = Interaction.MsgBox(Information.Err().Number.AsString());
                break;
            end_IL_0001:
                ;
            }
            catch (Exception obj5) when (obj5 is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj5);
                try0001_dispatch = 3361;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_3: // <========== 8
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void Button14_Click(object sender, EventArgs e)
    {
        RadioButton1.Visible = false;
        RadioButton2.Visible = false;
        if (!RadioButton16.Checked & !RadioButton15.Checked)
        {
            RadioButton16.Checked = true;
        }
        List1.Items.Clear();
        GroupBox8.Visible = true;
    }

    private string such(IRecordset dB_EventTable, ListBox.ObjectCollection items)
    {
        ////Discarded unreachable code: IL_0125
        //int try0001_dispatch = -1;
        //int num2 = default;
        //int num = default;
        //while (true)
        //{
        //    try
        //    {
        //        /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
        //        ;
        //        switch (try0001_dispatch)
        //        {
        //            default:
        ProjectData.ClearProjectError();
        int Event_iArt = dB_EventTable.Fields[EventFields.Art].AsInt().AsInt();
        int Event_iPerFamNr = dB_EventTable.Fields[EventFields.PerFamNr].AsInt();
        string liText;
        if (Event_iArt < 499)
        {
            Modul1.Person_ReadNames(Event_iPerFamNr, Modul1.Person);
            liText = Strings.Left(Modul1.Person.SurName.Trim() + "," + Modul1.Person.Givennames.Trim() + "                                  ", 20);
            if (liText.Trim() != "")
            {
                _ = items.Add(new ListItem($"{liText}          {Modul1.PersInArb}", Event_iPerFamNr));
            }
        }
        return items.Count.AsString() + " Einträge";
        //                break;
        //            case 297:
        //                num = -1;
        //                switch (num2)
        //                {
        //                    case 2:
        //                        break;
        //                    default:
        //                        goto IL_015f;
        //                }
        //                break;
        //        }
        //    }
        //    catch (Exception obj) when (obj is not null && num2 != 0 && num == 0)
        //    {
        //        ProjectData.SetProjectError(obj);
        //        try0001_dispatch = 297;
        //        continue;
        //    }
        //    break;
        //IL_015f:
        //    throw ProjectData.CreateProjectError(-2146828237);
        //}
        //if (num != 0)
        //{
        //    ProjectData.ClearProjectError();
        //}
    }

    private void Famsuch(IRecordset dB_EventTable)
    {
        int famInArb;
        if (dB_EventTable.Fields[EventFields.Art].AsInt() > 499)
        {
            {
                famInArb = dB_EventTable.Fields[EventFields.PerFamNr].AsInt();
                string sDest = "                                                                                          ";
                DataModul.Link.ReadFamily(famInArb, Modul1.Family);
                if (Modul1.Family.Mann > 0)
                {
                    Modul1.PersInArb = Modul1.Family.Mann;
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    if (Modul1.Person.SurName != "")
                    {
                        StringType.MidStmtStr(ref sDest, 1, 35, Modul1.Person.SurName + " " + Modul1.Person.Givennames);
                    }
                    else
                    {
                        StringType.MidStmtStr(ref sDest, 1, 15, ">NN<");
                    }
                    StringType.MidStmtStr(ref sDest, 36, 2, "/ ");
                }
                if (Modul1.Family.Frau > 0)
                {
                    Modul1.PersInArb = Modul1.Family.Frau;
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    if (Modul1.Person.SurName != "")
                    {
                        StringType.MidStmtStr(ref sDest, 37, 35, Modul1.Person.SurName + " " + Modul1.Person.Givennames);
                    }
                    else
                    {
                        StringType.MidStmtStr(ref sDest, 37, 35, ">NN<");
                    }
                }
                if (sDest.Trim() != "")
                {
                    _ = List1.Items.Add(new ListItem(sDest + "          " + famInArb.AsString()));
                    //Sz++;
                }
            }
            Label4.Text = List1.Items.Count.AsString() + " Einträge";
        }
    }

    private void Button15_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_027c
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
                    int num5;
                    int i;
                    int i2;
                    int num8;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 872:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_02ce;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                {
                                    goto end_IL_0001_2;
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_02d2;
                            }
                        end_IL_0001:
                            break;
                        IL_0008:
                            num = 2;
                            DataModul.DB_PersonTable.MoveLast();
                            num5 = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                            int num6 = 1;
                            if (num6 == 0)
                            {
                                goto end_IL_0001_2;
                            }

                            int num7;
                            if (num6 > DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt())
                            {
                                _ = Interaction.MsgBox(Modul1.IText[EUserText.t173] + " " + num6.AsString() + Modul1.IText[EUserText.t172]);
                                goto end_IL_0001_2;
                            }
                            else
                            {
                                ProgressBar1.Minimum = 0;
                                ProgressBar1.Maximum = 0;
                                ProgressBar1.Step = 1;
                                ProgressBar1.Maximum = DataModul.DB_PersonTable.RecordCount - 1;
                                i = num6;
                                num7 = DataModul.DB_PersonTable.RecordCount - 1;
                                I1 = i;
                                goto IL_0266;
                            }
                        IL_0266:
                            i2 = I1;
                            num8 = num7;
                            if (i2 > num8)
                            {
                                goto end_IL_0001_2;
                            }
                            ProgressBar1.PerformStep();
                            Modul1.PersInArb = I1;
                            var pt = DataModul.Person.Seek(I1);
                            if (null != pt.Fields[PersonFields.OFB].Value)
                            {
                                if (pt.Fields[PersonFields.OFB].AsString() == "J")
                                {
                                    pt.Edit();
                                    pt.Fields[PersonFields.OFB].Value = "";
                                    pt.Update();
                                }
                            }
                            I1++;
                            goto IL_0266;
                        IL_02ce:
                            num4 = unchecked(num2 + 1);
                            goto IL_02d2;
                        IL_02d2:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 6:
                                case 10:
                                case 29:
                                case 31:
                                case 34:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 872;
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
