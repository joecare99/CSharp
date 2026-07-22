using BaseLib.Helper;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace GenFreeWin.Views;

internal partial class Ahnenneu : Form
{
    private static List<WeakReference> __ENCList = new List<WeakReference>();
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

    public ControlArray<Label> Label1;
    public ControlArray<Label> Label3;

    private int _gen;
    private short _t;
    private int _z;
    private string _liText;
    private short _frauenkek1;
    private short _frauenkek2;


    [DebuggerNonUserCode]
    public Ahnenneu()
    {
        Load += Ahnenneu_Load;
        MouseMove += Ahnenneu_MouseMove;
        Resize += Ahnenneu_Resize;
        FormClosed += Ahnenneu_FormClosed;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
#pragma warning disable CS0618 // Typ oder Element ist veraltet

        Label1 = new ControlArray<Label>();
        Label3 = new ControlArray<Label>();
#pragma warning restore CS0618 // Typ oder Element ist veraltet
        ((ISupportInitialize)Label1).BeginInit();
        ((ISupportInitialize)Label3).BeginInit();


        InitializeComponent();

        Label3.SetIndex(_Label3_32, 32);
        Label3.SetIndex(_Label3_31, 31);
        Label3.SetIndex(_Label3_30, 30);
        Label3.SetIndex(_Label3_29, 29);
        Label3.SetIndex(_Label3_28, 28);
        Label3.SetIndex(_Label3_27, 27);
        Label3.SetIndex(_Label3_26, 26);
        Label3.SetIndex(_Label3_25, 25);
        Label3.SetIndex(_Label3_24, 24);
        Label3.SetIndex(_Label3_23, 23);
        Label3.SetIndex(_Label3_22, 22);
        Label3.SetIndex(_Label3_21, 21);
        Label3.SetIndex(_Label3_20, 20);
        Label3.SetIndex(_Label3_19, 19);
        Label3.SetIndex(_Label3_18, 18);
        Label3.SetIndex(_Label3_17, 17);
        Label3.SetIndex(_Label3_16, 16);
        Label3.SetIndex(_Label3_15, 15);
        Label3.SetIndex(_Label3_14, 14);
        Label3.SetIndex(_Label3_13, 13);
        Label3.SetIndex(_Label3_12, 12);
        Label3.SetIndex(_Label3_11, 11);
        Label3.SetIndex(_Label3_10, 10);
        Label3.SetIndex(_Label3_9, 9);
        Label3.SetIndex(_Label3_8, 8);
        Label3.SetIndex(_Label3_7, 7);
        Label3.SetIndex(_Label3_6, 6);
        Label3.SetIndex(_Label3_5, 5);
        Label3.SetIndex(_Label3_4, 4);
        Label3.SetIndex(_Label3_3, 3);
        Label3.SetIndex(_Label3_2, 2);
        Label3.SetIndex(_Label3_1, 1);

        Label1.SetIndex(_Label1_49, 49);
        Label1.SetIndex(_Label1_50, 50);
        Label1.SetIndex(_Label1_51, 51);
        Label1.SetIndex(_Label1_52, 52);
        Label1.SetIndex(_Label1_53, 53);
        Label1.SetIndex(_Label1_54, 54);
        Label1.SetIndex(_Label1_55, 55);
        Label1.SetIndex(_Label1_56, 56);
        Label1.SetIndex(_Label1_57, 57);
        Label1.SetIndex(_Label1_58, 58);
        Label1.SetIndex(_Label1_59, 59);
        Label1.SetIndex(_Label1_60, 60);
        Label1.SetIndex(_Label1_62, 62);
        Label1.SetIndex(_Label1_61, 61);
        Label1.SetIndex(_Label1_63, 63);
        Label1.SetIndex(_Label1_64, 64);
        Label1.SetIndex(_Label1_34, 34);
        Label1.SetIndex(_Label1_33, 33);
        Label1.SetIndex(_Label1_35, 35);
        Label1.SetIndex(_Label1_36, 36);
        Label1.SetIndex(_Label1_37, 37);
        Label1.SetIndex(_Label1_38, 38);
        Label1.SetIndex(_Label1_39, 39);
        Label1.SetIndex(_Label1_40, 40);
        Label1.SetIndex(_Label1_42, 42);
        Label1.SetIndex(_Label1_41, 41);
        Label1.SetIndex(_Label1_43, 43);
        Label1.SetIndex(_Label1_44, 44);
        Label1.SetIndex(_Label1_45, 45);
        Label1.SetIndex(_Label1_46, 46);
        Label1.SetIndex(_Label1_47, 47);
        Label1.SetIndex(_Label1_48, 48);
        Label1.SetIndex(_Label1_32, 32);
        Label1.SetIndex(_Label1_31, 31);
        Label1.SetIndex(_Label1_30, 30);
        Label1.SetIndex(_Label1_29, 29);
        Label1.SetIndex(_Label1_28, 28);
        Label1.SetIndex(_Label1_27, 27);
        Label1.SetIndex(_Label1_26, 26);
        Label1.SetIndex(_Label1_25, 25);
        Label1.SetIndex(_Label1_24, 24);
        Label1.SetIndex(_Label1_23, 23);
        Label1.SetIndex(_Label1_22, 22);
        Label1.SetIndex(_Label1_21, 21);
        Label1.SetIndex(_Label1_20, 20);
        Label1.SetIndex(_Label1_19, 19);
        Label1.SetIndex(_Label1_18, 18);
        Label1.SetIndex(_Label1_17, 17);
        Label1.SetIndex(_Label1_16, 16);
        Label1.SetIndex(_Label1_15, 15);
        Label1.SetIndex(_Label1_14, 14);
        Label1.SetIndex(_Label1_13, 13);
        Label1.SetIndex(_Label1_12, 12);
        Label1.SetIndex(_Label1_11, 11);
        Label1.SetIndex(_Label1_10, 10);
        Label1.SetIndex(_Label1_9, 9);
        Label1.SetIndex(_Label1_8, 8);
        Label1.SetIndex(_Label1_7, 7);
        Label1.SetIndex(_Label1_6, 6);
        Label1.SetIndex(_Label1_5, 5);
        Label1.SetIndex(_Label1_4, 4);
        Label1.SetIndex(_Label1_3, 3);
        Label1.SetIndex(_Label1_2, 2);
        Label1.AddMouseDown(Label1_MouseDown); // Add MouseDown event handler for Label1 
        Label1.AddClick(Label1_Click);
        Label3.AddClick(Label3_Click);
        ((ISupportInitialize)Label1).EndInit();
        ((ISupportInitialize)Label3).EndInit();

    }

    private void Ahnenneu_Load(object eventSender, EventArgs eventArgs)
    {
        BackColor = Personen.Default.BackColor;
        if (Modul1.FontSize > 0f)
        {
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        }
        DesktopLocation = Personen.Default.DesktopLocation;
        Ahnfuell();
    }

    private void Ahnenneu_MouseMove(object eventSender, MouseEventArgs eventArgs)
    {
        checked
        {
            Frame1.Width = _Label1_7.Left - 10;
            Frame1.Height = 300;
        }
    }

    private void Ahnenneu_Resize(object eventSender, EventArgs eventArgs)
    {
        Width = Personen.Default.Width;
        Height = Personen.Default.Height;
    }

    private void Ahnenneu_FormClosed(object eventSender, FormClosedEventArgs eventArgs)
    {
        DataModul.NB.Close();
    }


    private void Label1_MouseDown(object eventSender, MouseEventArgs eventArgs)
    {
        int index = Label1.GetIndex((Label)eventSender);
        switch (eventArgs.Button)
        {
            case MouseButtons.Left:
                Label1_Click(eventSender, eventArgs);
                break;
            case MouseButtons.Right:

                if (Label1[index].Text == "")
                {
                    break;
                }
                Modul1.PersInArb = Label1[index].Tag.AsInt();
                if (Modul1.PersInArb == 0)
                {
                    break;
                }
                Frame1.Visible = true;
                Frame1.Width = 600;
                Frame1.Height = 530;
                Frame1.Visible = true;
                List1.Items.Clear();
                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);

                Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out int iAhn, out var Modul1_Kont20);
                Modul1.Kont[97] = iAhn.AsString();

                string itemString = Modul1.Person.SurName + ", " + Modul1.Person.Givennames;
                _ = List1.Items.Add(new ListItem(itemString, Modul1.PersInArb));
                Modul1.Datles(Modul1.PersInArb, Modul1.Person);
                List1.Items.SetString(1, "Geboren: " + Modul1.Person.Birthday);
                List1.Items.SetString(2, "Getauft: " + Modul1.Person.Baptised);
                List1.Items.SetString(3, "Gestorben: " + Modul1.Person.Death);
                List1.Items.SetString(4, "Begraben: " + Modul1.Person.Burial);
                List1.Items.SetString(5, "Verbindung(en): ");
                List2.Items.Clear();
                Personen.Default.List4.Items.Clear();
                Modul1.eLKennz = ELinkKennz.lkFather;
                DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb);
                string Persex = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString().AsString();
                int persInArb = Modul1.PersInArb;
                if (Persex == "M")
                {
                    Modul1.eLKennz = ELinkKennz.lkMother;
                }
                foreach (int iFamNr in Modul1.Ehesuch(Modul1.PersInArb, Persex))
                {
                    string datu = $"{Event_GetFamilyYear(iFamNr).Year,4}";

                    if (DataModul.Link.GetFamPerson(iFamNr, Modul1.eLKennz, out var link_iPerNr))
                    {
                        Modul1.Person_ReadNames(link_iPerNr, Modul1.Person);

                        Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out int iAhn2, out Modul1_Kont20);
                        Modul1.Kont[97] = iAhn2.AsString();

                        if (datu.Right(1) == "F")
                        {
                            datu = datu.Left(4) + " F";
                        }
                        _liText = new string(' ', 80);
                        if (Modul1.Person.SurName != "" | Modul1.Person.Givennames != "")
                        {
                            StringType.MidStmtStr(ref _liText, 1, 60, "" + datu.Right(6) + " " + Modul1.Person.Givennames + " " + Modul1.Person.SurName.ToUpper().TrimEnd());
                        }
                        else
                        {
                            StringType.MidStmtStr(ref _liText, 1, 60, "" + datu.Right(6) + " namenlosem Partner");
                        }
                        _ = List2.Items.Add(new ListItem(_liText, Modul1.PersInArb));
                    }
                    else
                    {
                        string text = " unbekanntem Partner";
                        if (datu.Right(1) == "F")
                        {
                            datu = datu.Left(4);
                            text = " F Unbekannter Partner";
                        }
                        _liText = new string(' ', 80);
                        StringType.MidStmtStr(ref _liText, 1, 60, "    " + datu.Right(6) + text);
                        _ = List2.Items.Add(new ListItem(_liText, 0));
                    }

                }
                int num9 = List2.Items.Count - 1;
                int M1_Iter = 0;
                while (true)
                {
                    int i2 = M1_Iter;
                    int num6 = num9;
                    if (i2 > num6)
                    {
                        break;
                    }
                    _ = List1.Items.Add(new ListItem(List2.Items.ItemString(M1_Iter), List2.Items.ItemData(M1_Iter)));
                    M1_Iter++;
                }
                Modul1.PersInArb = persInArb;
                Personen.Default.Sortlist.Items.Clear();
                foreach (var iFamNr in Modul1.Ehesuch(Modul1.PersInArb, Persex))
                {
                    foreach (var itm in Modul1.Family_Kindsuch(iFamNr))
                        Personen.Default.Sortlist.Items.Add(itm);
                }
                if (Personen.Default.Sortlist.Items.Count > 0)
                {
                    _ = List1.Items.Add(new ListItem(Modul1.IText[EUserText.t130], -1));
                    int num11 = Personen.Default.Sortlist.Items.Count - 1;
                    M1_Iter = 0;
                    while (true)
                    {
                        int i4 = M1_Iter;
                        int num6 = num11;
                        if (i4 <= num6)
                        {
                            Modul1.PersInArb = (int)Math.Round(Strings.Mid(Personen.Default.Sortlist.Items.ItemString(M1_Iter), 11, 10).AsDouble());
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out int iAhn2, out Modul1_Kont20);
                            Modul1.Kont[97] = iAhn2.AsString();
                            string DDatum = Personen.Default.Sortlist.Items.ItemString(M1_Iter).Left(10);
                            string HT = DDatum.Date2DotDateStr2();
                            if (HT == "")
                            {
                                HT = "          ";
                            }
                            if (Personen.Default.Sortlist.Items.ItemData<int>(M1_Iter) == 10)
                            {
                                HT += " (Adop.)";
                            }
                            var sGivennames = Modul1.Person.Givennames.Trim() == "" ? Modul1.Person.SurName.Trim() : Modul1.Person.Givennames.Trim();
                            itemString = new string(' ', 100);
                            StringType.MidStmtStr(ref itemString, 1, 40, HT + "   " + sGivennames);
                            _ = List1.Items.Add(new ListItem(itemString, Modul1.PersInArb));
                            M1_Iter++;
                            continue;
                        }
                        break;
                    }
                }
                else
                {
                    _ = List1.Items.Add(new ListItem(Modul1.IText[EUserText.t131], -1));
                }
                break;

        }

    }

    private void Label1_Click(object eventSender, EventArgs e)
    {
        int index = Label1.GetIndex((Label)eventSender);
        Modul1.PersInArb = Label1[index].Tag.AsInt();
        if (Modul1.PersInArb != 0)
        {
            MainProject.Forms.Namensuch.SetPerson(Modul1.PersInArb, 1, 9);
        }
    }

    private DateTime Event_GetFamilyYear(int iFamNr)
    {
        DateTime datu = default;
        DateTime dt = default;
        var _t = EEventArt.eA_500;
        while (_t <= EEventArt.eA_507)
        {
            if ((dt = DataModul.Event.GetDate(_t, iFamNr)) != default)
            {
                datu = dt;
                break;
            }
            _t++;
        }
        if (datu == default)
        {
            if ((dt = DataModul.Event.GetDate(EEventArt.eA_601, iFamNr)) != default)
            {
                datu = dt;
            }
        }

        return datu;
    }

    private void Label2_Click(object eventSender, EventArgs eventArgs)
    {
        Modul1.Schalt = 3;
        Personen.Default.lblSearch2.Text = "";
        Modul1.PersInArb = Label2.Tag.AsInt();
        Close();
        Personen.Default.Perzeig(Modul1.PersInArb);
        Modul1.Aend = 0f;
    }

    private void Label3_Click(object eventSender, EventArgs eventArgs)
    {
        int index = Label3.GetIndex((Label)eventSender);
        Label4.Visible = true;
        checked
        {
            Modul1.PersInArb = Label1[(short)(index + 32)].Tag.AsInt();
            AB1(GetPersInArb());
            var M1_Iter = 1;
            int i;
            int num;
            do
            {
                Label3[(short)M1_Iter].Visible = false;
                M1_Iter++;
                i = M1_Iter;
                num = 32;
            }
            while (i <= num);
            M1_Iter = 2;
            int i2;
            do
            {
                Label1[(short)M1_Iter].Text = "";
                M1_Iter++;
                i2 = M1_Iter;
                num = 64;
            }
            while (i2 <= num);
            M1_Iter = 1;
            int i3;
            do
            {
                Label3[(short)M1_Iter].Visible = false;
                M1_Iter++;
                i3 = M1_Iter;
                num = 32;
            }
            while (i3 <= num);
            DataModul.NB_Ahn1Table.Index = "Ahnen";
            short num2 = 1;
            short num3;
            short num4;
            do
            {
                DataModul.NB_Ahn1Table.Index = "Ahnen";
                DataModul.NB_Ahn1Table.Seek("=", 0, 0, num2.AsString());
                if (!DataModul.NB_Ahn1Table.NoMatch)
                {
                    Modul1.PersInArb = DataModul.NB_Ahn1Table.Fields["PerNr"].AsInt();
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out int iAhn, out var Modul1_Kont20);
                    Modul1.Kont[97] = iAhn.AsString();
                    Label1[(short)(num2 + 1)].Text = Modul1.Person.SurName + ", " + Modul1.Person.Givennames;
                    Label1[(short)(num2 + 1)].Tag = DataModul.NB_Ahn1Table.Fields["PerNr"].AsInt();
                }
                num2 = (short)unchecked(num2 + 1);
                num3 = num2;
                num4 = 63;
            }
            while (num3 <= num4);
            _z = 0;
            num2 = 64;
            short num5;
            do
            {
                _z++;
                DataModul.NB_Ahn1Table.Index = "Ahnen";
                DataModul.NB_Ahn1Table.Seek("=", 0, 0, num2.AsString());
                if (!DataModul.NB_Ahn1Table.NoMatch)
                {
                    Modul1.PersInArb = DataModul.NB_Ahn1Table.Fields["PerNr"].AsInt();
                    Label3[(short)_z].Visible = true;
                    Label3[(short)_z].Tag = DataModul.NB_Ahn1Table.Fields["PerNr"].AsInt();
                }
                else
                {
                    DataModul.NB_Ahn1Table.Seek("=", 0, 0, num2.AsString());
                    if (!DataModul.NB_Ahn1Table.NoMatch)
                    {
                        Modul1.PersInArb = DataModul.NB_Ahn1Table.Fields["PerNr"].AsInt();
                    }
                }
                num2 = (short)unchecked(num2 + 2);
                num5 = num2;
                num4 = 127;
            }
            while (num5 <= num4);
        }
    }

    private void Label4_Click(object eventSender, EventArgs eventArgs)
    {
        Label4.Visible = false;
        Ahnfuell();
    }

    private void List1_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        if (List1.Items.ItemData(List1.SelectedIndex) is int i && i > 0)
        {
            Modul1.PersInArb = i;
            Close();
            Modul1.Schalt = 3;
            Personen.Default.lblSearch2.Text = "";
            Personen.Default.Perzeig(Modul1.PersInArb);
            Modul1.Aend = 0f;
        }
    }

    private void List1_MouseMove(object eventSender, MouseEventArgs eventArgs)
    {
        checked
        {
            Frame1.Width = 600;
            Frame1.Height = 530;
        }
    }

    public int GetPersInArb()
    {
        return Modul1.PersInArb;
    }

    public void AB1(int persInArb)
    {
        //Discarded unreachable code: IL_0b87, IL_0d39
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        string source = default;
        string destination = default;
        string name = default;
        int lErl = default;
        int nr = default;
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
                    int M1_Iter = default;
                    int num4;
                    bool flag;
                    int Modul1_Nr1 = 0;
                    int Modul1_Gen1 = 0;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            goto IL_Start;
                        case 4316:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0e42;
                                    default:
                                        goto end_IL_0001;
                                }
                                goto IL_ErrHndl;
                            }

                        IL_0e46:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_Start;
                                case 33:
                                case 126:
                                case 129:
                                case 142:
                                    goto IL_024a;
                                case 111:
                                case 122:
                                case 123:
                                case 146:
                                    goto IL_ErrHndl;
                                default:
                                    goto end_IL_0001;
                                case 22:
                                case 143:
                                case 144:
                                case 145:
                                case 162:
                                    goto end_IL_0001_2;
                            }
                            goto default;

                        IL_0e42:
                            num4 = unchecked(num2 + 1);
                            goto IL_0e46;

                        IL_Start:
                            num = 2;

                            Modul1.Verz1 = Modul1.Verz.Left(15);
                            if (Modul1.System.VerSpecial == 1)
                            {
                                Modul1.Verz1 = Modul1.Verz.Left(20);
                            }

                            ProjectData.ClearProjectError();
                            num3 = 2;
                            DataModul.CreateNewNBDatabase(Modul1.Persistence);
                            lErl = 11;
                            _gen = 0;
                            Modul1.Schalt = 0;
                            if (persInArb == 0)
                            {
                                _ = Interaction.MsgBox("Stop", title: "4", mb: MessageBoxButtons.OK);
                                goto end_IL_0001_2;
                            }
                            nr = Modul1_Nr1 = 1;
                            _z = nr - 1;
                            Modul1_Gen1 = 6;
                            Modul1_Gen1 = Modul1_Gen1 + (_gen - 1);
                            num5 = 0;
                            num6 = Modul1_Nr1;
                            M1_Iter = Modul1_Nr1;
                            goto IL_024a;
                        IL_024a: //<===================
                            num = 33;
                            lErl = 5;
                            flag = true;
                            while (flag && persInArb > 0)
                            {
                                DataModul.NB_Ahnen.Commit(persInArb, Modul1.FamInArb, _gen, num6, num5, Modul1.Person.SurName);
                                Modul1.Schalt = 1;
                                Modul1.eLKennz = ELinkKennz.lkChild;
                                var aiFam = Modul1.Link_Famsuch(persInArb, Modul1.eLKennz);
                                if (aiFam.Count != 0)
                                    Modul1.FamInArb = aiFam[0];
                                _gen++;
                                if (!(_gen == 135 | _gen > Modul1_Gen1.AsDouble() + 1.0)
                                    && Modul1.FamInArb != 0)
                                {
                                    Modul1.Family.Mann = 0;
                                    Modul1.Family.Frau = 0;
                                    DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                                    persInArb = Modul1.Family.Mann;
                                    if (Modul1.Schalt < 2)
                                    {
                                        num6 *= 2;
                                    }
                                    if (num6 >= 1e10)
                                    {
                                        num7 = (int)(num6 / 1e10);
                                        num6 %= 1e10.AsInt();
                                        num5 += num7;
                                    }
                                    _frauenkek2 = (short)num5;
                                    _frauenkek1 = (short)(num6 + 1);
                                    if (Modul1.Family.Frau > 0)
                                    {
                                        DataModul_WriteNBFrau1Data(Modul1.Family.Frau, Modul1.FamInArb, Modul1_Nr1, _gen, _frauenkek1, _frauenkek2);
                                        Modul1_Nr1++;
                                    }
                                    Modul1.Family.Frau = 0;
                                    if (Modul1.Family.Mann <= 0)
                                    {
                                        M1_Iter++;
                                    }
                                }
                                else
                                {
                                    lErl = 6;
                                    ProjectData.ClearProjectError();
                                    num3 = 0;
                                    _z++;
                                    flag = DataModul.NB_Frau.ReadData(_z, out var iPers, out var famNr, out _gen, out num5, out var _num6);
                                    if (flag)
                                        (Modul1.PersInArb, Modul1.FamInArb) = (iPers, famNr);
                                    num6 = _num6;
                                }
                            }
                            return;


                        IL_ErrHndl:
                            num = 146;
                            if (Information.Err().Number == 91)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0e42;
                            }
                            else if (Information.Err().Number == 3420)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0e42;
                            }
                            else if (Information.Err().Number == 53)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0e42;
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
                                goto IL_0e46;
                            }



                        end_IL_0001:
                            break;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 4316;
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


    public void DataModul_WriteNBFrau1Data(int PersNr, int famNr, int Nr, int Gen, int Frauenkek1, int Frauenkek2)
    {
        if (DataModul.NB_Frau.PersonExists(PersNr))
        {
            DataModul.NB_Frau.AddRow(Nr, Gen, PersNr, famNr, Frauenkek1, Frauenkek2);
        }
        else
        {
            DataModul.NB_Frau.AddRow(Nr, Gen, PersNr, famNr, Frauenkek1, Frauenkek2);
        }
    }

    public void Ahnfuell()
    {
        //Discarded unreachable code: IL_065a, IL_0698
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
                    int iAhn;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 1983:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_069b;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Information.Err().Number != 53)
                                {
                                    goto end_IL_0001_2;
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_069b;
                            }
                        end_IL_0001:
                            break;
                        IL_0008:
                            num = 2;
                            Label4.Visible = false;
                            if (CheckBox1.Checked)
                            {
                                goto end_IL_0001_2;
                            }
                            List1.Items.Clear();
                            Frame1.Visible = false;
                            var M1_Iter = 1;
                            while (M1_Iter <= 32)
                            {
                                Label3[(short)M1_Iter].Visible = false;
                                M1_Iter++;
                            }
                            M1_Iter = 2;
                            while (M1_Iter <= 64)
                            {
                                Label1[(short)M1_Iter].Text = "";
                                Label1[(short)M1_Iter].Tag = "";
                                M1_Iter++;
                            }
                            Modul1.PersInArb = Personen.Default.PersonNr;
                            Modul1.Startpers = Personen.Default.PersonNr;
                            string Modul1_Kont20;
                            AB1(GetPersInArb());
                            DataModul.NB_Ahn1Table.Index = "Ahnen";
                            short num6 = 1;
                            while (num6 <= 63)
                            {
                                DataModul.NB_Ahn1Table.Index = "Ahnen";
                                DataModul.NB_Ahn1Table.Seek("=", 0, 0, num6.AsString());
                                if (!DataModul.NB_Ahn1Table.NoMatch)
                                {
                                    Modul1.PersInArb = DataModul.NB_Ahn1Table.Fields["PerNr"].AsInt();
                                    if (Modul1.PersInArb != 0)
                                    {
                                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                        Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out iAhn, out Modul1_Kont20);
                                        Modul1.Kont[97] = iAhn.AsString();
                                        Label1[(short)(num6 + 1)].Text = Modul1.Person.SurName + ", " + Modul1.Person.Givennames;
                                        Label1[(short)(num6 + 1)].Tag = DataModul.NB_Ahn1Table.Fields["PerNr"].AsInt();
                                    }
                                }
                                lErl = 4;
                                num6 = (short)unchecked(num6 + 1);
                            }
                            _z = 0;
                            num6 = 64;
                            while (num6 <= 127)
                            {
                                _z++;
                                DataModul.NB_Ahn1Table.Index = "Ahnen";
                                DataModul.NB_Ahn1Table.Seek("=", 0, 0, num6.AsString());
                                if (!DataModul.NB_Ahn1Table.NoMatch)
                                {
                                    Modul1.PersInArb = DataModul.NB_Ahn1Table.Fields["PerNr"].AsInt();
                                    Label3[(short)_z].Visible = true;
                                    Label3[(short)_z].Text = ">";
                                    Label3[(short)_z].Tag = DataModul.NB_Ahn1Table.Fields["PerNr"].AsInt();
                                }
                                else
                                {
                                    DataModul.NB_Ahn1Table.Seek("=", 0, 0, num6.AsString());
                                    if (!DataModul.NB_Ahn1Table.NoMatch)
                                    {
                                        Modul1.PersInArb = DataModul.NB_Ahn1Table.Fields["PerNr"].AsInt();
                                    }
                                }
                                num6 = (short)unchecked(num6 + 2);
                            }
                            Modul1.PersInArb = Modul1.Startpers;
                            if (Modul1.PersInArb == 0)
                            {
                                goto end_IL_0001_2;
                            }
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out iAhn, out Modul1_Kont20);
                            Modul1.Kont[97] = iAhn.AsString();
                            Label2.Text = "Startperson:                <zurück>\n" + Modul1.Person.SurName + ", " + Modul1.Person.Givennames + "\n";
                            Modul1.Datles(Modul1.PersInArb, Modul1.Person);
                            Label2.Text = Label2.Text + "Geboren: " + Modul1.Person.Birthday + "\n";
                            Label2.Text = Label2.Text + "Getauft: " + Modul1.Person.Baptised + "\n";
                            Label2.Text = Label2.Text + "Gestorben: " + Modul1.Person.Death + "\n";
                            Label2.Text = Label2.Text + "Begraben: " + Modul1.Person.Burial + "\n";
                            Label2.Tag = Modul1.Startpers;
                            goto end_IL_0001_2;
                        IL_069b:
                            num4 = unchecked(num2 + 1);
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 4:
                                case 53:
                                case 63:
                                case 66:
                                case 68:
                                case 69:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 1983;
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
