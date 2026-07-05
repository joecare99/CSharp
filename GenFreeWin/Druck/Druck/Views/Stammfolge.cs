using Druck.My;
using GenFree.Helper;
using GenFree.Data;
using GenFree.Interfaces.DB;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Compatibility.VB6;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using GenFree;
using BaseLib.Helper;
using System.Runtime.Serialization;

namespace Druck.Views;

[DesignerGenerated]
public partial class Stammfolge : Form
{

    private int[] GLauf;

    private string Aus_Nr;

    private string Aus_Text;

    private byte ZZ;

    private byte Tz;

    private bool Hei;

    private short Abst;

    private string PatText;

    private bool DGSchalt;

    private bool LSchalt;

    private int Perstatic;

    private short IQ;

    private int Lauf;

    private short Gen;

    private string SKennz;

    private int PerSp1;

    private IRecordset BiTable;

    private byte Ri;

    private byte MaxGen;

    [SpecialName]
    private string _0024STATIC_0024Partles_002420118_0024Namen1;

    [SpecialName]
    private byte _Weitehen_Fa;

    [SpecialName]
    private byte _0024STATIC_0024Kisort_00242001_0024I;


    [SpecialName]
    private byte _0024STATIC_0024Dat_00242001_0024I;

    [SpecialName]
    private byte _WeitEhen_I4;

    [SpecialName]
    private int _0024STATIC_0024Dat_00242001_0024PerSp1;

    [SpecialName]
    private int _0024STATIC_0024Dat_00242001_0024Ke;

    [SpecialName]
    private string _0024STATIC_0024Kisort_00242001_0024Datum;

    [SpecialName]
    private byte _0024STATIC_0024Partles_002420118_0024K;

    [SpecialName]
    private ELinkKennz _0024STATIC_0024Dat_00242001_0024Kennz1;

    [SpecialName]
    private string _0024STATIC_0024Partles_002420118_0024VName;

    [SpecialName]
    private byte _0024STATIC_0024Weitehen_00242001_0024a;

    [SpecialName]
    private short _0024STATIC_0024Weitehen_00242001_0024D;

    [SpecialName]
    private string _0024STATIC_0024Partles_002420118_0024Namen2;
    private bool M1_Ki;
    private int M1_J;
    private float M_Sgg = 1f;
    private string _M_ind;
    private string M_Namen;

    [DebuggerNonUserCode]
    public Stammfolge()
    {
        base.Load += Stammfolge_Load;
        base.FormClosing += Stammfolge_FormClosing;
        GLauf = new int[91];
        Aus_Nr = "";
        Aus_Text = "";
        this.Anz = new Microsoft.VisualBasic.Compatibility.VB6.RichTextBoxArray(this.components);
        this.List1 = new Microsoft.VisualBasic.Compatibility.VB6.ListBoxArray(this.components);
        this.Command_Renamed = new ControlArray<System.Windows.Forms.Button>();
        this.Command1 = new ControlArray<System.Windows.Forms.Button>();
        this.Command2 = new ControlArray<System.Windows.Forms.Button>();
        this.Label1 = new ControlArray<System.Windows.Forms.Label>();
        InitializeComponent();
        this.List1.SetIndex(this._List1_6, 6);
        this.Command1.SetIndex(this._Command1_8, 8);
        this.Command1.SetIndex(this._Command1_7, 7);
        this.Command2.SetIndex(this._Command2_4, 4);
        this.Command2.SetIndex(this._Command2_2, 2);
        this.Command2.SetIndex(this._Command2_1, 1);
        this.Command2.SetIndex(this._Command2_0, 0);
        this.Command2.SetIndex(this._Command2_3, 3);
        this.List1.SetIndex(this._List1_5, 5);
        this.List1.SetIndex(this._List1_4, 4);
        this.List1.SetIndex(this._List1_3, 3);
        this.Anz.SetIndex(this._Anz_1, 1);
        this.Anz.SetIndex(this._Anz_0, 0);
        this.Command_Renamed.SetIndex(this._Command_1, 1);
        this.Command_Renamed.SetIndex(this._Command_0, 0);
        this.Label1.SetIndex(this._Label1_5, 5);
        this.Label1.SetIndex(this._Label1_4, 4);
        this.Label1.SetIndex(this._Label1_3, 3);
        this.Label1.SetIndex(this._Label1_2, 2);
        this.Label1.SetIndex(this._Label1_1, 1);
        this.Label1.SetIndex(this._Label1_0, 0);
        this.List1.SetIndex(this._List1_2, 2);
        this.List1.SetIndex(this._List1_1, 1);
        this.List1.SetIndex(this._List1_0, 0);
        this.Anz.SetIndex(this._Anz_2, 2);

        Command_Renamed.AddClick(Command_Renamed_Click);
        Command1.AddClick(Command1_Click);
        Command2.AddClick(Command2_Click);

    }

    private void Command_Renamed_Click(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int index = default;
        int num2 = default;
        int num3 = default;
        short num5 = default;
        short num6 = default;
        string name = default;
        string source = default;
        string destination = default;
        short num7 = default;
        short num8 = default;
        string text3 = default;
        int lErl = default;
        short num9 = default;
        byte b = default;
        int selectionLength = default;
        short num10 = default;
        string text4 = default;
        short num11 = default;
        short num12 = default;
        short num13 = default;
        int num14 = default;
        int num15 = default;
        short num16 = default;
        short num17 = default;
        short num18 = default;
        short num19 = default;
        int num21 = default;
        int num22 = default;
        byte Tot = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    string text;
                    string text2;
                    string text5;
                    string text6;
                    string text7;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            index = Command_Renamed.GetIndex((Button)eventSender);
                            goto IL_0016;
                        case 14619:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_30ad;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 3022)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_30ad;
                                }
                                if (Information.Err().Number == 3420)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_30ad;
                                }
                                if (Information.Err().Number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_30ad;
                                }
                                if (Information.Err().Number == 5)
                                {
                                    goto end_IL_0000_2;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_30b1;
                            }
                        end_IL_0000:
                            break;
                        IL_0016:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            switch (index)
                            {
                                case 0:
                                    Close();
                                    MyProject.Forms.Druck.Show();
                                    break;
                                case 1:
                                    goto IL_0055;
                                default:
                                    break;
                            }
                            goto end_IL_0000_2;
                        IL_0055:
                            num = 11;
                            Frame1.Visible = false;
                            text = new string(' ', 80);
                            MyProject.Forms.AW.Show();
                            MyProject.Forms.AW.Kontroll[100].Visible = true;
                            MyProject.Forms.AW.Kontroll[97].Visible = false;
                            MyProject.Forms.AW.Kontroll[95].Visible = false;
                            MyProject.Forms.AW.Kontroll[91].Visible = true;
                            MyProject.Forms.AW.Kontroll[77].Visible = true;
                            MyProject.Forms.AW.Kontroll[81].Visible = true;
                            MyProject.Forms.AW.Kontroll[85].Visible = true;
                            MyProject.Forms.AW.Kontroll[88].Visible = true;
                            MyProject.Forms.AW.Kontroll[89].Visible = true;
                            MyProject.Forms.AW.Hide();
                            MyProject.Forms.AW.ShowDialog("STF");
                            _Modul1.Instance.PersSp = _Modul1.Instance.PersInArb;
                            DataModul.DB_PersonTable.Index = "PerNr";
                            DataModul.DB_PersonTable.MoveFirst();
                            goto IL_0304;
                        IL_02f6: // <========== 3
                                 // <========== 4
                            num = 46;
                            DataModul.DB_PersonTable.MoveNext();
                            goto IL_0304;
                        IL_0304: // <========== 3
                                 // <========== 3
                            num = 30;
                            if (!DataModul.DB_PersonTable.EOF)
                            {
                                _Modul1.Instance.PersInArb = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                                if (_Modul1.Instance.DAus[122].AsDouble() == 1.0)
                                {
                                    Module2.TotPrüf1(ref Tot);
                                    if (Tot == 2)
                                        goto IL_02f6;
                                }
                                if (_Modul1.Instance.DAus[120] == "1")
                                {
                                    Module2.Datcheck();
                                    if (_Modul1.Instance.Datum6.AsDouble() > _Modul1.Instance.DAus[121].AsDouble())
                                    {
                                        DataModul.DT_SperrTable.AddNew();
                                        DataModul.DT_SperrTable.Fields["Pernr"].Value = _Modul1.Instance.PersInArb;
                                        DataModul.DT_SperrTable.Update();
                                    }
                                }
                                goto IL_02f6;
                            }
                            _Modul1.Instance.PersInArb = _Modul1.Instance.PersSp;
                            Lauf = 0;
                            num5 = 0;
                            IQ = -1;
                            num6 = 1;
                            while (num6 <= 90)
                            {
                                GLauf[num6] = 0;
                                num6 = (short)unchecked(num6 + 1);
                            }
                            name = _Modul1.Instance.Verz1 + "TeMP\\NUMTemp.mdb";
                            source = _Modul1.Instance.Verz1 + "INIT\\NUMTEMP.mdb";
                            destination = _Modul1.Instance.Verz1 + "TEMP\\NumTemp.mdb";
                            FileSystem.FileCopy(source, destination);
                            DataModul.NB = UpgradeSupport.DAODBEngine_definst.OpenDatabase(name, false, false, "");
                            DataModul.NB_FrauTable = DataModul.NB.OpenRecordset(dbTables.Frauen, RecordsetTypeEnum.dbOpenTable);
                            DataModul.NB.Execute($"ALTER Table {dbTables.DgbIndex} add Column Geb Text(4);");
                            DataModul.NB.Execute($"DROP INDEX GES ON {dbTables.DgbIndex};");
                            DataModul.NB.Execute($"CREATE UNIQUE INDEX GES ON {dbTables.DgbIndex} ([Name],[Vornam],[Geb],[Ind]);");
                            DataModul.NB_DgbTable = DataModul.NB.OpenRecordset(dbTables.DgbIndex, RecordsetTypeEnum.dbOpenTable);
                            DataModul.NB_FrauTable.Index = "Nach";
                            DataModul.NB_FrauTable.AddNew();
                            DataModul.NB_FrauTable.Fields["Nr"].Value = _Modul1.Instance.PersInArb;
                            DataModul.NB_FrauTable.Fields["Gen"].Value = 1;
                            DataModul.NB_FrauTable.Fields["Kek2"].Value = 0;
                            DataModul.NB_FrauTable.Update();
                            Frame1.Visible = false;
                            Anz[0].Visible = true;
                            num7 = 3;
                            if (num7 == 0)
                            {
                                MyProject.Forms.Nachlist.Close();
                                MyProject.Forms.Druck.Show();
                                goto end_IL_0000_2;
                            }
                            Gen = 1;
                            num5 = 0;
                            num8 = 2;
                            text2 = Strings.Right("0" + Gen.AsString().Trim(), 2);
                            Anz[0].Enabled = false;
                            Anz[0].Font = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectionAlignment = HorizontalAlignment.Center;
                            Anz[0].SelectedText = "Stammfolgeliste für";
                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            _Modul1.Instance.Person.SetFullSurname(_Modul1.Instance.BuildFullSurName(_Modul1.Instance.Person, _Modul1.Instance.DAus[89] == "1"));
                            Anz[0].SelectedText = " " + (_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Kont[3].TrimEnd()).Trim() + " ";
                            text3 = _Modul1.Instance.Kont[0];
                            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                            Anz[0].SelectedText = text3 + "\n";
                            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectedText = $"Erstellt am {DateTime.Today.AsString()}";
                            Anz[0].SelectedText = $" von {_Modul1.Instance.User.Name.Trim()} mit {_Modul1.Instance.AppName} aus Mandant: {_Modul1.Instance.Verz}";
                            goto IL_0772;
                        IL_0772: // <========== 4
                                 // <========== 4
                            num = 96;
                            lErl = 1;
                            Anz[0].SelectionCharOffset = 0;
                            Anz[0].SelectedText = "\n";
                            Anz[0].SelectionAlignment = HorizontalAlignment.Left;
                            if (_Modul1.Instance.PrintDat.BemZahl > 0)
                            {
                                Anz[0].SelectionIndent = 35;
                                Anz[0].SelectionHangingIndent = 10;
                                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                Anz[0].SelectedText = "Quellen: ";
                                num9 = (short)_Modul1.Instance.PrintDat.BemZahl;
                                num6 = 1;
                                while (num6 <= num9)
                                {
                                    if ((_Modul1.Instance.DAus[71] == "1") & (num6 <= _Modul1.Instance.PrintDat.BemZahl))
                                    {
                                        Anz[0].SelectedText = "\n";
                                    }
                                    _Modul1.Instance.UbgT1 = _Modul1.Instance.KontBem[num6];
                                    _Modul1.Instance.UbgT1 = _Modul1.Instance.Retweg(_Modul1.Instance.UbgT1);
                                    Anz[0].SelectedText = " " + _Modul1.Instance.UbgT1;
                                    _Modul1.Instance.UbgT1 = "";
                                    num6 = (short)unchecked(num6 + 1);
                                }
                                Anz[0].SelectedText = "\n";
                            }
                            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            goto IL_0952;
                        IL_0952: // <========== 3
                                 // <========== 3
                            num = 117;
                            lErl = 2;
                            Anz[0].SelectionIndent = 0;
                            _Modul1.Instance.PrintDat.BemZahl = 0;
                            DataModul.NB_FrauTable.Seek(">=", Gen, num5);
                            if (num5 == 0)
                            {
                                num5 = 1;
                            }
                            if (DataModul.NB_FrauTable.NoMatch)
                            {
                                if (b == 1)
                                {
                                    Gen--;
                                    num5 = 0;
                                    num5 = (short)(num5 + 1);
                                    if (DataModul.NB_FrauTable.RecordCount == 0)
                                    {
                                        goto IL_2bbb;
                                    }
                                    goto IL_0952;
                                }
                                goto IL_0a65;
                            }
                            b = 1;
                            if (Anz[0].SelectionStart > 50000)
                            {
                                Anz[0].SelectedText = "\n";
                                Anz[0].SelectionIndent = 0;
                                Anz[0].SelectedText = "\n";
                                selectionLength = Anz[0].SelectionStart;
                                Anz[0].SelectionStart = 0;
                                Anz[0].SelectionLength = selectionLength;
                                num10 = (short)(num10 + 1);
                                FileSystem.FileClose(99);
                                text4 = _Modul1.Instance.GenFreeDir + "Temp\\Sptext" + num10.AsString().Trim() + ".rtf";
                                FileSystem.FileOpen(99, text4, OpenMode.Output);
                                FileSystem.PrintLine(99, Anz[0].SelectedRtf);
                                FileSystem.FileClose(99);
                                Anz[0].Text = "";
                                Anz[0].SelectionAlignment = HorizontalAlignment.Left;
                            }
                            Anz[0].SelectionAlignment = HorizontalAlignment.Left;
                            Anz[0].SelectedText = "\n";
                            _M_ind = DataModul.NB_FrauTable.Fields["Gen"].AsString().Trim() + "." + DataModul.NB_FrauTable.Fields["Kek2"].AsString().Trim();
                            if (Information.IsDBNull(DataModul.NB_FrauTable.Fields["alt"].Value))
                            {
                            }
                            Anz[0].SelectedText = DataModul.NB_FrauTable.Fields["Gen"].AsString().Trim() + "." + DataModul.NB_FrauTable.Fields["Kek2"].AsString().Trim() + " ";
                            Aus_Nr = DataModul.NB_FrauTable.Fields["Gen"].AsString().Trim() + "." + DataModul.NB_FrauTable.Fields["Kek2"].AsString().Trim();
                            Label2.Font = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            Label2.Text = "";
                            Label2.Text = DataModul.NB_FrauTable.Fields["Gen"].AsString().Trim() + "." + DataModul.NB_FrauTable.Fields["Kek2"].AsString().Trim() + " ";
                            Abst = (short)Label2.Width;
                            Gen = (short)DataModul.NB_FrauTable.Fields["Gen"].AsInt();
                            _Modul1.Instance.PersInArb = DataModul.NB_FrauTable.Fields["Nr"].AsInt();
                            Perstatic = _Modul1.Instance.PersInArb;
                            if (_Modul1.Instance.DAus[76] == "1")
                            {
                                Anz[0].SelectedText = " <" + _Modul1.Instance.PersInArb.AsString().Trim() + "> ";
                            }
                            Anz[0].SelectionHangingIndent = Abst;
                            if (!Information.IsDBNull(DataModul.NB_FrauTable.Fields["alt"].Value))
                            {
                                Aus_Text = Conversions.ToString(Operators.ConcatenateObject(" (aus ", DataModul.NB_FrauTable.Fields["alt"].AsString()) + ")");
                            }
                            EPerles();
                            if (Aus_Text != "")
                            {
                                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                Anz[0].SelectedText = Aus_Text;
                                Aus_Text = "";
                                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            }
                            DataModul.NB_FrauTable.Delete();
                            _Modul1.Instance.PersInArb = Perstatic;
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                            if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                            {
                                _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                            }
                            var aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                            List1[2].Items.Clear();
                            num13 = (short)aiFams.Count;
                            num6 = 1;
                            goto IL_1494;
                        IL_0a65: // <========== 3
                                 // <========== 3
                            num = 138;
                            if (Gen == MaxGen)
                            {
                                num8 = 2;
                                num5 = (short)(num5 + 1);
                            }
                            Gen = num8;
                            num8 = (short)(num8 + 1);
                            goto IL_0772;
                        IL_1486: // <========== 3
                                 // <========== 3
                            num = 238;
                            num6 = (short)unchecked(num6 + 1);
                            goto IL_1494;
                        IL_1494:
                            if (num6 <= num13)
                            {
                                _Modul1.Instance.FamInArb = _Modul1.Instance.UbgT.Left(10).AsInt();
                                _Modul1.Instance.Datschalt = 10;
                                _Modul1.Instance.Famdatles1(0, out var asFamDate);
                                _Modul1.Instance.Datschalt = 0;
                                if (asFamDate[12].Trim() == "")
                                {
                                    asFamDate[12] = asFamDate[13];
                                }
                                if (asFamDate[12].Trim() == "")
                                {
                                    asFamDate[12] = asFamDate[15];
                                }
                                if (asFamDate[12].Trim() == "")
                                {
                                    asFamDate[12] = asFamDate[17];
                                }
                                if (asFamDate[12].Trim() == "")
                                {
                                    asFamDate[12] = asFamDate[10];
                                }
                                if (asFamDate[12].Trim() == "")
                                {
                                    asFamDate[12] = asFamDate[18];
                                }
                                asFamDate[12] = "        " + asFamDate[12].Trim().Right(8);
                                _Modul1.Instance.UbgT = Strings.Mid(_Modul1.Instance.UbgT, 11, _Modul1.Instance.UbgT.Length);
                                num21 = 0;
                                num22 = 11;
                                while (num22 <= 18)
                                {
                                    if (asFamDate[num22].AsDouble() > num21)
                                    {
                                        num21 = (int)Math.Round(asFamDate[num22].AsDouble());
                                    }
                                    num22++;
                                }
                                if (_Modul1.Instance.DAus[124].AsDouble() == 1.0)
                                {
                                    if (num21 < _Modul1.Instance.DAus[125].AsDouble())
                                    {
                                        List1[2].Items.Add(asFamDate[12] + _Modul1.Instance.FamInArb.AsString());
                                    }

                                }
                                else
                                {
                                    List1[2].Items.Add(asFamDate[12] + _Modul1.Instance.FamInArb.AsString());
                                }
                                goto IL_1486;
                            }
                            num14 = List1[2].Items.Count - 1;
                            num15 = 0;
                            goto IL_2b92;
                        IL_2b85: // <========== 3
                                 // <========== 3
                            num = 469;
                            num15++;
                            goto IL_2b92;
                        IL_2b92:
                            if (num15 <= num14)
                            {
                                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1[2].Items[num15].AsString(), 9, 10)));
                                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                Anz[0].SelectedText = " \n";
                                Anz[0].SelectionIndent = 0;
                                Anz[0].SelectionHangingIndent = 0;
                                if (List1[2].Items.Count > 1)
                                {
                                    Anz[0].SelectedText = (num15 + 1.AsString()).Trim() + ". " + _Modul1.Instance.DTxt[11] + " ";
                                }
                                else
                                {
                                    Anz[0].SelectedText = _Modul1.Instance.DTxt[11] + " ";
                                }
                                M1_Ki = false;
                                Heidat();
                                if ((_Modul1.Instance.DAus[115] == "1") | (_Modul1.Instance.DAus[116] == "1"))
                                {
                                    Module2.Bildaus("F", "STF");
                                }
                                if (_Modul1.Instance.DAus[88] == "1")
                                {
                                    Bild("F", _Modul1.Instance.FamInArb);
                                }
                                if (_Modul1.Instance.DAus[62] == "1")
                                {
                                    FamQuell();
                                }
                                if (_Modul1.Instance.DAus[4] == "1")
                                {
                                    DataModul.DB_FamilyTable.Index = "Fam";
                                    DataModul.DB_FamilyTable.Seek("=", _Modul1.Instance.FamInArb);
                                    if (Operators.CompareString(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        _Modul1.Instance.UbgT1 = DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].AsString();
                                        Bemaus();
                                    }
                                }
                                _Modul1.Instance.PersInArb = Perstatic;
                                _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                                _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                                if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                                {
                                    _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                }
                                leerweg();
                                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                Anz[0].SelectedText = " mit ";
                                Anz[0].SelectedText = "\n";
                                DataModul.NB_FamilyTable.AddNew();
                                DataModul.NB_FamilyTable.Fields[IndexFields.Fam].Value = _Modul1.Instance.FamInArb;
                                DataModul.NB_FamilyTable.Update();
                                if (DataModul.Link.GetFamPerson(_Modul1.Instance.FamInArb, _Modul1.Instance.eLKennz, out _Modul1.Instance.PersInArb))
                                {
                                    DataModul.DT_SperrTable.Index = "Nr";
                                    DataModul.DT_SperrTable.Seek("=", _Modul1.Instance.PersInArb);
                                    if (!DataModul.DT_SperrTable.NoMatch)
                                    {
                                        Anz[0].SelectionIndent = 20;
                                        Anz[0].SelectionHangingIndent = num16;
                                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                        Anz[0].SelectedText = _Modul1.Instance.Datschuname;
                                    }
                                    else
                                    {
                                        EPerles();
                                    }
                                }
                                else
                                    Anz[0].SelectedText = "unbekanntem Partner";
                                lErl = 4;
                                num17 = 0;
                                num6 = 1;
                                while (num6 <= 99)
                                {
                                    _Modul1.Instance.Family.Kind[num6] = 0;
                                    num6 = (short)unchecked(num6 + 1);
                                }
                                DataModul.NB_FamilyTable.AddNew();
                                DataModul.NB_FamilyTable.Fields[IndexFields.Fam].Value = _Modul1.Instance.FamInArb;
                                DataModul.NB_FamilyTable.Update();
                                num6 = 1;
                                foreach (var cLink in DataModul.Link.ReadAllFams(_Modul1.Instance.FamInArb, ELinkKennz.lkChild))
                                {
                                    num17++;
                                    _Modul1.Instance.Family.Kind[num17] = cLink.iPersNr;
                                }
                                num16 = 0;
                                if (num17 > 0)
                                {
                                    num18 = num17;
                                    num6 = 1;
                                    while (num6 <= num18)
                                    {
                                        Label2.Font = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                        Label2.Text = "";
                                        Label2.Text = num6.AsString() + ". ";
                                        if (Label2.Width > num16)
                                        {
                                            num16 = (short)(Label2.Width + 10);
                                        }
                                        num6++;
                                    }
                                    Anz[0].SelectedText = "\n";
                                    Anz[0].SelectionIndent = 20;
                                    Anz[0].SelectionHangingIndent = num16;
                                    Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                    Anz[0].SelectedText = "Kinder:";
                                    Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                    Kisort();
                                    Lauf = GLauf[Gen + 1];
                                    num19 = (short)(List1[1].Items.Count - 1);
                                    IQ = 0;
                                    while (IQ <= num19)
                                    {
                                        leerweg();
                                        if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
                                        {
                                            Anz[0].SelectedText = "\n";
                                        }
                                        Anz[0].SelectionCharOffset = 0;
                                        DGSchalt = false;
                                        _Modul1.Instance.PersInArb = Conversions.ToInteger(Strings.Mid(List1[1].Items[IQ].AsString(), 9, 10));
                                        string sPSex = DataModul.Person.GetSex(_Modul1.Instance.PersInArb);
                                        if ((sPSex == "M") || (sPSex == "F"))
                                        {
                                            _Modul1.Instance.eLKennz = _Modul1.Instance.DAus[77] == "0" && sPSex == "F" ? ELinkKennz.lkMother : ELinkKennz.lkFather;
                                            SKennz = sPSex == "F" ? "2" : "1";
                                            var aiFams2 = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                            foreach (var iFam in aiFams2)
                                            {
                                                _Modul1.Instance.FamInArb = iFam;
                                                DataModul.NB_FamilyTable.AddNew();
                                                DataModul.NB_FamilyTable.Fields[IndexFields.Fam].Value = _Modul1.Instance.FamInArb;
                                                DataModul.NB_FamilyTable.Update();
                                                foreach (var cLink in DataModul.Link.ReadAllFams(_Modul1.Instance.FamInArb, ELinkKennz.lkChild))
                                                {
                                                    DataModul.DB_PersonTable.Seek("=", cLink.iPersNr);
                                                    if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "M"
                                                        || DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                                                    {
                                                        DGSchalt = true;
                                                        break; //243c
                                                    }
                                                }
                                                if (DGSchalt)
                                                    break;
                                            }
                                            lErl = 66;
                                        }
                                        _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                        DataModul.DT_SperrTable.Index = "Nr";
                                        DataModul.DT_SperrTable.Seek("=", _Modul1.Instance.PersInArb);
                                        if (!DataModul.DT_SperrTable.NoMatch)
                                        {
                                            Anz[0].SelectionIndent = 20;
                                            Anz[0].SelectionHangingIndent = num16;
                                            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                            if (_Modul1.Instance.DAus[76] == "1")
                                            {
                                                Anz[0].SelectedText = "  " + IQ + 1.AsString().Right(2) + ".  <" + _Modul1.Instance.PersInArb.AsString().Trim() + ">";
                                            }
                                            else
                                            {
                                                Anz[0].SelectedText = "   " + IQ + 1.AsString().Right(2) + ".";
                                            }
                                            Anz[0].SelectedText = _Modul1.Instance.Datschuname;
                                        }
                                        else
                                        {
                                            _Modul1.Instance.Ind1 = _M_ind + "." + (IQ + 1.AsString()).Trim();
                                            Namenindex();
                                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                            text3 = _Modul1.Instance.Kont[0];
                                            _Modul1.Instance.Person.SetFullSurname(_Modul1.Instance.BuildFullSurName(_Modul1.Instance.Person, _Modul1.Instance.DAus[89] == "1"));
                                            text5 = _Modul1.Instance.Kont[0];
                                            text6 = _Modul1.Instance.Kont[99];
                                            Anz[0].SelectionIndent = 20;
                                            Anz[0].SelectionHangingIndent = num16;
                                            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                            if (_Modul1.Instance.DAus[76] == "1")
                                            {
                                                Anz[0].SelectedText = "  " + IQ + 1.AsString().Right(2) + ".  <" + _Modul1.Instance.PersInArb.AsString().Trim() + ">";
                                            }
                                            else
                                            {
                                                Anz[0].SelectedText = "   " + IQ + 1.AsString().Right(2) + ".";
                                            }
                                            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                            Anz[0].SelectedText = " " + (_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Kont[3]).Trim();
                                            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont[0].Trim();
                                            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                            leerweg();
                                            if (_Modul1.Instance.Kont[4] != "")
                                            {
                                                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Italic);
                                                Anz[0].SelectedText = " (" + _Modul1.Instance.Kont[4].TrimEnd() + ")";
                                                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                                Anz[0].SelectedText = " ";
                                            }
                                            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                            leerweg();
                                            if (_Modul1.Instance.Kont[5].Trim() != "")
                                            {
                                                Anz[0].SelectedText = ", Sippe " + _Modul1.Instance.Kont[5];
                                            }
                                            leerweg();
                                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                                            text7 = "";
                                            if (Operators.CompareString(DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                text7 = DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString().Trim();
                                            }
                                            _Modul1.Instance.Ind1 = _M_ind + "." + (IQ + 1.AsString()).Trim();
                                            _Modul1.Instance.Aschalt = unchecked(_Modul1.Instance.Datschalt);
                                            _Modul1.Instance.Datschalt = (byte)Math.Round(_Modul1.Instance.Aschalt);
                                            Dat();
                                        }
                                        lErl = 3;
                                        IQ++;
                                    }
                                    IQ = -1;
                                }
                                goto IL_2b85;
                            }
                            Gen++;
                            goto IL_0772;
                        IL_2bbb: // <========== 3
                                 // <========== 3
                            num = 474;
                            Anz[0].SelectedText = "\n";
                            Anz[0].SelectionIndent = 0;
                            Anz[0].SelectedText = "\n";
                            selectionLength = Anz[0].SelectionStart;
                            Anz[0].SelectionStart = 0;
                            Anz[0].SelectionLength = selectionLength;
                            num10 = (short)(num10 + 1);
                            FileSystem.FileClose(99);
                            text4 = _Modul1.Instance.GenFreeDir + "Temp\\Sptext" + num10.AsString().Trim() + ".rtf";
                            FileSystem.FileOpen(99, text4, OpenMode.Output);
                            FileSystem.PrintLine(99, Anz[0].SelectedRtf);
                            FileSystem.FileClose(99);
                            Anz[0].Text = "";
                            FileSystem.FileClose(99);
                            num11 = num10;
                            FileSystem.FileClose(99);
                            Label3.Visible = true;
                            num12 = num11;
                            num6 = 1;
                            while (num6 <= num12)
                            {
                                text4 = _Modul1.Instance.GenFreeDir + "Temp\\Sptext" + num6.AsString().Trim() + ".rtf";
                                Anz[2].LoadFile(text4);
                                FileSystem.Kill(text4);
                                selectionLength = Anz[2].Text.Length;
                                Anz[2].SelectionStart = 0;
                                Anz[2].SelectionLength = selectionLength;
                                Anz[0].SelectedRtf = Anz[2].SelectedRtf;
                                Anz[2].Text = "";
                                num6 = (short)unchecked(num6 + 1);
                            }
                            Cursor = Cursors.Arrow;
                            Label3.Visible = false;
                            Anz[0].Visible = true;
                            Anz[0].Enabled = true;
                            Button1.Enabled = true;
                            Button2.Enabled = true;
                            Button3.Enabled = true;
                            Button4.Enabled = true;
                            Button5.Enabled = true;
                            Button6.Enabled = true;
                            Button7.Enabled = true;
                            if (DataModul.DSB_QuellIdxTable.RecordCount <= 0)
                            {
                            }
                            else
                            {
                                Button4.Visible = true;
                            }
                            goto end_IL_0000_2;
                        IL_30ad: // <========== 4
                                 // <========== 4
                            num4 = unchecked(num2 + 1);
                            goto IL_30b1;
                        IL_30b1:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 35:
                                case 44:
                                case 45:
                                case 46:
                                    goto IL_02f6;
                                case 29:
                                case 30:
                                case 47:
                                    goto IL_0304;
                                case 96:
                                case 144:
                                case 471:
                                    goto IL_0772;
                                case 117:
                                case 131:
                                case 132:
                                    goto IL_0952;
                                case 134:
                                    num = 134;
                                    if (DataModul.NB_FrauTable.RecordCount != 0)
                                    {
                                        goto IL_0a65;
                                    }
                                    goto IL_2bbb;
                                case 136:
                                case 137:
                                case 138:
                                    goto IL_0a65;
                                case 233:
                                case 234:
                                case 237:
                                case 238:
                                    goto IL_1486;
                                case 468:
                                case 469:
                                    goto IL_2b85;
                                case 130:
                                case 135:
                                case 474:
                                    goto IL_2bbb;
                                case 4:
                                case 9:
                                case 77:
                                case 133:
                                case 472:
                                case 473:
                                case 514:
                                case 515:
                                case 529:
                                case 535:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 14619;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 6
                       // <========== 7
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    private void Command1_Click(object eventSender, EventArgs eventArgs)
    {
        var index = Command1.GetIndex((Button)eventSender);
        RichTextBox richTextBox = Anz[1];
        _Modul1.Instance.PrintDat.Flagsch = 2;
        ProjectData.ClearProjectError();
        switch (index)
        {
            case 5:
                Command1_5_Click();
                break;
            case 7:
                Command1_7_Click(ichTextBox);
                break;
            case 8:
                Command1_8_Click(richTextBox);
                break;
            default:
                break;
        }
    }

    private void Command1_8_Click(RichTextBox richTextBox)
    {
        richTextBox.SelectionAlignment = HorizontalAlignment.Left;
        _Modul1.Instance.PrintDat.Flagsch = 1;
        Frame3.Visible = false;
        Anz[0].Visible = false;
        IRecordset? nB_DgbTable = DataModul.NB_DgbTable;
        richTextBox.Text = "";
        richTextBox.Top = 0;
        richTextBox.Left = 0;
        richTextBox.Width = checked((int)Math.Round(12000.0 / 1440.0 * DeviceDpi));
        richTextBox.Height = checked((int)Math.Round(7800.0 / 1440.0 * DeviceDpi));
        richTextBox.Font = richTextBox.SelectionFont.ChangeFBold(Bold: true);
        richTextBox.Font = richTextBox.SelectionFont.ChangeFBold(Bold: false);
        richTextBox.Visible = true;

        nB_DgbTable.Index = "Vollnam";
        nB_DgbTable.Seek(">=", " ", "");
        while (!nB_DgbTable.EOF)
        {
            if (Conversions.ToBoolean(nB_DgbTable.Fields["Name"].AsString() != "NN" & nB_DgbTable.Fields["Name"].AsString() != "N.N.") && nB_DgbTable.Fields["Name"].AsString() != _Modul1.Instance.AltName)
            {
                richTextBox.SelectionIndent = 0;
                richTextBox.SelectedText = (nB_DgbTable.Fields["Name"].AsString() + '\n').AsString();
                _Modul1.Instance.AltName = nB_DgbTable.Fields["Name"].AsString();
            }
            nB_DgbTable.MoveNext();
        }
    }

    private void Command1_7_Click(RichTextBox richTextBox)
    {
        Frame3.Visible = false;
        IRecordset? nB_DgbTable = DataModul.NB_DgbTable;
        Font fntregular = new(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
        Font fntBold = new(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);

        richTextBox.SelectionAlignment = HorizontalAlignment.Left;
        richTextBox.Text = "";
        richTextBox.SelectionFont = fntregular;
        richTextBox.SelectionAlignment = HorizontalAlignment.Left;
        richTextBox.SelectedText = "\n";
        richTextBox.SelectionTabs = [1, 200, 250, 650];

        Anz[0].Visible = false;
        richTextBox.Top = 0;
        richTextBox.Left = 0;
        richTextBox.Font = fntregular;
        richTextBox.SelectionFont = fntBold;
        richTextBox.SelectedText = "Personenindex\n";
        richTextBox.SelectionAlignment = HorizontalAlignment.Left;
        richTextBox.SelectionFont = fntregular;
        richTextBox.Visible = true;

        nB_DgbTable.Index = "Vollnam";
        nB_DgbTable.Seek(">=", " ", "");
        while (!nB_DgbTable.EOF)
        {
            if (nB_DgbTable.Fields["Name"].AsString() != _Modul1.Instance.AltName)
            {
                richTextBox.SelectedText = "\n";
                richTextBox.SelectionFont = fntregular;
                richTextBox.SelectionIndent = 0;
                richTextBox.SelectionFont = fntBold;
                if (Strings.InStr(nB_DgbTable.Fields["Name"].AsString(), " siehe ") != 0)
                {
                    richTextBox.SelectedText = nB_DgbTable.Fields["Name"].AsString();
                    _Modul1.Instance.AltName = nB_DgbTable.Fields["Name"].AsString();
                }
                else
                {
                    richTextBox.SelectedText = Strings.Left((nB_DgbTable.Fields["Name"].AsString() + "                            ").AsString(), 25);
                    richTextBox.SelectionFont = fntregular;
                    _Modul1.Instance.AltName = nB_DgbTable.Fields["Name"].AsString();
                    richTextBox.SelectedText = "\n";
                }
            }
            else
            {
                richTextBox.SelectionIndent = 20;
                richTextBox.SelectionHangingIndent = 10;
                if (Conversions.ToBoolean((left2 == nB_DgbTable.Fields["Vornam"].AsString()) & (left == nB_DgbTable.Fields["Geb"].AsString())))
                {
                    richTextBox.SelectedText = "  " + nB_DgbTable.Fields["Ind"].AsString().Trim();
                }
                else
                {
                    if (Strings.Mid(richTextBox.Text, richTextBox.SelectionStart, 1) != "\n")
                    {
                        richTextBox.SelectedText = "\n";
                    }
                    richTextBox.SelectedText = Conversions.ToString(nB_DgbTable.Fields["Vornam"].AsString() + '\t' + "* " + nB_DgbTable.Fields["Geb"].AsString() + '\t' + nB_DgbTable.Fields["Ind"].AsString().Trim());
                }
            }
            nB_DgbTable.MoveNext();
        }
        richTextBox.SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
        richTextBox.LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
    }

    private void Command1_5_Click()
    {
        Close();
        BiTable.Close();
        DataModul.NB_PersonTable.Close();
        DataModul.NB_FamilyTable.Close();
        DataModul.NB_FrauTable.Close();
        DataModul.NB_DgbTable.Close();
        DataModul.DSB_OrtIdxTable.Close();
        DataModul.NB.Close();
        DataModul.DSB_SortTable.Close();
        DataModul.DSB_NamIdxTable.Close();
        DataModul.MandDB.Close();
        DataModul.DOSB.Close();
        DataModul.TempDB.Close();
        DataModul.DSB.Close();
        _Modul1.Instance.GED = false;
    }

    private void Command2_Click(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int index = default;
        int num2 = default;
        int num3 = default;
        int number = default;
        string left = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                int AAA;
                string LD;
                int ortNr;
                int altNr;
                int ortNr2;
                byte Schalt;
                switch (try0000_dispatch)
                {
                    default:
                        num = 1;
                        index = Command2.GetIndex((Button)eventSender);
                        goto IL_0015;
                    case 6207:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_1509;
                                default:
                                    goto end_IL_0000;
                            }
                            number = Information.Err().Number;
                            if (number is 3022 or 3211 or 3010 or 3376)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1509;
                            }
                            if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_150d;
                        }
                    end_IL_0000:
                        break;
                    IL_0015:
                        num = 2;
                        left = "";
                        Frame2.Visible = false;
                        _Modul1.Instance.Ind1 = "";
                        Anz[1].SelectionHangingIndent = checked((int)Math.Round(0.0 * 1440.0 / DeviceDpi));
                        Anz[1].Visible = true;
                        switch (index)
                        {
                            case 0:
                                break;
                            case 1:
                                goto IL_04ca;
                            case 2:
                                goto IL_09a7;
                            case 3:
                                goto IL_0dce;
                            case 4:
                                goto IL_0f83;
                            default:
                                goto IL_1441;
                        }
                        Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        Anz[1].SelectionAlignment = HorizontalAlignment.Center;
                        Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                        Anz[1].SelectedText = "Ortsindex";
                        Anz[1].SelectedText = "\n";
                        DataModul.DSB_OrtIdxTable.Index = "Ort";
                        DataModul.DSB_OrtIdxTable.Seek(">=", " ");
                        goto IL_0468;
                    IL_03d4: // <========== 3
                        num = 40;
                        if (left != DataModul.DSB_OrtIdxTable.Fields["Ind"].AsString())
                        {
                            Anz[1].SelectedText = (DataModul.DSB_OrtIdxTable.Fields["Ind"].AsString() + "; ").AsString();
                            left = DataModul.DSB_OrtIdxTable.Fields["Ind"].AsString();
                        }
                        goto IL_045a;
                    IL_045a:
                        num = 44;
                        DataModul.DSB_OrtIdxTable.MoveNext();
                        goto IL_0468;
                    IL_0468: // <========== 3
                        num = 19;
                        if (!DataModul.DSB_OrtIdxTable.EOF)
                        {
                            if (DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt() != _Modul1.Instance.AltNr)
                            {
                                Anz[1].SelectionAlignment = HorizontalAlignment.Left;
                                Anz[1].SelectedText = "\n";
                                Anz[1].SelectionIndent = 0;
                                _Modul1.Instance.UbgT = _Modul1.Instance.ortles(DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt(), 21);
                                Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                Anz[1].SelectedText = _Modul1.Instance.UbgT;
                                left = "";
                                _Modul1.Instance.AltNr = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                                Anz[1].SelectedText = "\n";
                                Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                Anz[1].SelectionIndent = 40;
                                if (Check1.Checked)
                                {
                                    if (Operators.CompareString(DataModul.DB_PlaceTable.Fields[PlaceFields.Bem].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        Anz[1].SelectedText = DataModul.DB_PlaceTable.Fields[PlaceFields.Bem].AsString();
                                        Anz[1].SelectedText = "\n";
                                    }
                                }
                            }
                            goto IL_03d4;
                        }
                        Anz[1].SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                        Anz[1].LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                        goto end_IL_0000_2;
                    IL_04ca:
                        num = 50;
                        Anz[1].Font = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        Anz[1].SelectionAlignment = HorizontalAlignment.Center;
                        Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                        Anz[1].SelectedText = "Index Orte-Namen";
                        Anz[1].SelectedText = "\n";
                        Anz[1].SelectionHangingIndent = 0;
                        DataModul.DSB_OrtIdxTable.Index = "ortnam";
                        DataModul.DSB_OrtIdxTable.Seek(">=", " ");
                        goto IL_0945;
                    IL_07f8: // <========== 3
                        num = 79;
                        if (Operators.CompareString(DataModul.DSB_OrtIdxTable.Fields["Name"].AsString().Trim(), _Modul1.Instance.AltName.Trim(), TextCompare: false) != 0)
                        {
                            if (_Modul1.Instance.AltName != "")
                            {
                                Anz[1].SelectedText = "\n";
                            }
                            Anz[1].SelectedText = (DataModul.DSB_OrtIdxTable.Fields["Name"].AsString() + "  ").AsString();
                            Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            _Modul1.Instance.AltName = DataModul.DSB_OrtIdxTable.Fields["Name"].AsString();
                        }
                        Anz[1].SelectedText = (DataModul.DSB_OrtIdxTable.Fields["Ind"].AsString() + "; ").AsString();
                        DataModul.DSB_OrtIdxTable.MoveNext();
                        goto IL_0945;
                    IL_0945: // <========== 3
                        num = 59;
                        if (!DataModul.DSB_OrtIdxTable.EOF)
                        {
                            if (DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt() != _Modul1.Instance.AltNr)
                            {
                                Anz[1].SelectedText = "\n";
                                Anz[1].SelectionAlignment = HorizontalAlignment.Left;
                                Anz[1].SelectionIndent = 0;
                                _Modul1.Instance.AltName = "";
                                _Modul1.Instance.AltNr = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                                altNr = _Modul1.Instance.AltNr;
                                _Modul1.Instance.UbgT = _Modul1.Instance.ortles(altNr, 21);
                                Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                Anz[1].SelectedText = _Modul1.Instance.UbgT;
                                Anz[1].SelectedText = "\n";
                                Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                Anz[1].SelectionIndent = 40;
                                if (Check1.Checked)
                                {
                                    if (Operators.CompareString(DataModul.DB_PlaceTable.Fields[PlaceFields.Bem].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        Anz[1].SelectedText = DataModul.DB_PlaceTable.Fields[PlaceFields.Bem].AsString();
                                        Anz[1].SelectedText = "\n";
                                    }
                                }
                            }
                            goto IL_07f8;
                        }
                        Anz[1].SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                        Anz[1].LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                        goto end_IL_0000_2;
                    IL_09a7:
                        num = 94;
                        Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        Anz[1].SelectionAlignment = HorizontalAlignment.Center;
                        Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                        Anz[1].SelectedText = "Index Namen-Orte";
                        Anz[1].SelectedText = "\n";
                        DataModul.DSB_OrtIdxTable.Index = "NameOrt";
                        DataModul.DSB_OrtIdxTable.Seek(">=", " ");
                        while (!DataModul.DSB_OrtIdxTable.EOF)
                        {
                            if (Operators.CompareString(DataModul.DSB_OrtIdxTable.Fields["Name"].AsString().Trim(), _Modul1.Instance.AltName.Trim(), TextCompare: false) != 0)
                            {
                                Anz[1].SelectionAlignment = HorizontalAlignment.Left;
                                Anz[1].SelectedText = "\n";
                                Anz[1].SelectionIndent = 0;
                                _Modul1.Instance.AltNr = 0;
                                Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                Anz[1].SelectedText = DataModul.DSB_OrtIdxTable.Fields["Name"].AsString();
                                _Modul1.Instance.AltName = DataModul.DSB_OrtIdxTable.Fields["Name"].AsString();
                                Anz[1].SelectedText = "\n";
                                Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                Anz[1].SelectionIndent = 40;
                            }
                            if (DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt() != _Modul1.Instance.AltNr)
                            {
                                if (_Modul1.Instance.AltNr > 0)
                                {
                                    Anz[1].SelectedText = "\n";
                                }
                                Anz[1].SelectedText = (DataModul.DSB_OrtIdxTable.Fields["Ort"].AsString() + "  ").AsString();
                                Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                _Modul1.Instance.AltNr = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                            }
                            Anz[1].SelectedText = (DataModul.DSB_OrtIdxTable.Fields["Ind"].AsString() + "; ").AsString();
                            DataModul.DSB_OrtIdxTable.MoveNext();
                        }
                        Anz[1].SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                        Anz[1].LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                        goto end_IL_0000_2;
                    IL_0dce:
                        num = 130;
                        _Modul1.Instance.PrintDat.Flagsch = 1;
                        DataModul.DSB_OrtIdxTable.Index = "Ort";
                        DataModul.DSB_OrtIdxTable.Seek(">=", " ");
                        goto IL_0f68;
                    IL_0f57:
                        num = 143;
                        DataModul.DSB_OrtIdxTable.MoveNext();
                        goto IL_0f68;
                    IL_0f68: // <========== 3
                        num = 134;
                        if (DataModul.DSB_OrtIdxTable.EOF)
                        {
                            goto end_IL_0000_2;
                        }
                        if (DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt() != _Modul1.Instance.AltNr)
                        {
                            Anz[1].SelectionIndent = 0;
                            _Modul1.Instance.UbgT = _Modul1.Instance.ortles(DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt(), 2);
                            Anz[1].SelectedText = _Modul1.Instance.UbgT;
                            _Modul1.Instance.AltNr = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                            Anz[1].SelectedText = "\n";
                        }
                        goto IL_0f57;
                    IL_0f83:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        Anz[1].SelectionAlignment = HorizontalAlignment.Left;
                        DataModul.TempDB.Execute($"DROP Table {dbTables.OT};");
                        DataModul.TempDB.Execute($"CREATE Table {dbTables.OT} (OT TEXT(240)Not NULL,OrT TEXT(240)Not NULL);");
                        DataModul.TempDB.Execute($"CREATE UNIQUE INDEX Name ON {dbTables.OT} ([OT],[Ort]);");
                        DataModul.DT_OTTable = DataModul.TempDB.OpenRecordset(dbTables.OT, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DSB_OrtIdxTable.MoveFirst();
                        goto IL_1205;
                    IL_11f4: // <========== 3
                        num = 167;
                        DataModul.DSB_OrtIdxTable.MoveNext();
                        goto IL_1205;
                    IL_1205: // <========== 3
                        num = 155;
                        if (!DataModul.DSB_OrtIdxTable.EOF)
                        {
                            DataModul.DB_PlaceTable.Seek("=", DataModul.DSB_OrtIdxTable.Fields["OrtNr"]);
                            if (!DataModul.DB_PlaceTable.NoMatch)
                            {
                                if (DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].Value.AsDouble() > 0.0)
                                {
                                    AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt();
                                    LD = "";
                                    DataModul.Textlese(AAA, out _Modul1.Instance.Kont[1], out LD);
                                    AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt();
                                    LD = "";
                                    DataModul.Textlese(AAA, out _Modul1.Instance.Kont[2], out LD);
                                    DataModul.DT_OTTable.AddNew();
                                    DataModul.DT_OTTable.Fields["OT"].Value = _Modul1.Instance.Kont[1];
                                    DataModul.DT_OTTable.Fields["OrT"].Value = _Modul1.Instance.Kont[2];
                                    DataModul.DT_OTTable.Update();
                                }
                            }
                            goto IL_11f4;
                        }
                        Anz[1].Font = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                        Anz[1].SelectedText = "\nOrtsteil-Liste\n\n";
                        DataModul.DT_OTTable.Index = "Name";
                        DataModul.DT_OTTable.MoveFirst();
                        while (!DataModul.DT_OTTable.EOF)
                        {
                            _Modul1.Instance.Kont[1] = DataModul.DT_OTTable.Fields["OT"].AsString();
                            _Modul1.Instance.Kont[2] = DataModul.DT_OTTable.Fields["OrT"].AsString();
                            Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                            Anz[1].SelectedText = _Modul1.Instance.Kont[1];
                            Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[1].SelectedText = " siehe " + _Modul1.Instance.Kont[2] + "-" + _Modul1.Instance.Kont[1] + "\n\n";
                            DataModul.DT_OTTable.MoveNext();
                        }
                        Anz[1].Visible = true;
                        goto end_IL_0000_2;
                    IL_1441:
                        num = 187;
                        Interaction.MsgBox(index);
                        goto end_IL_0000_2;
                    IL_1509:
                        num4 = num2 + 1;
                        goto IL_150d;
                    IL_150d:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 37:
                            case 38:
                            case 39:
                            case 40:
                                goto IL_03d4;
                            case 43:
                            case 44:
                                goto IL_045a;
                            case 18:
                            case 19:
                            case 45:
                                goto IL_0468;
                            case 76:
                            case 77:
                            case 78:
                            case 79:
                                goto IL_07f8;
                            case 58:
                            case 59:
                            case 89:
                                goto IL_0945;
                            case 142:
                            case 143:
                                goto IL_0f57;
                            case 133:
                            case 134:
                            case 144:
                                goto IL_0f68;
                            case 165:
                            case 166:
                            case 167:
                                goto IL_11f4;
                            case 154:
                            case 155:
                            case 168:
                                goto IL_1205;
                            case 8:
                            case 48:
                            case 92:
                            case 128:
                            case 145:
                            case 185:
                            case 188:
                            case 189:
                            case 201:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 6207;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 7
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    private void Stammfolge_Load(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        string text = default;
        string source = default;
        string destination = default;
        string name = default;
        object CounterResult = default;
        object LoopForResult = default;
        int persInArb = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    byte PerPos;
                    Hinter hinter;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            BackColor = _Modul1.Instance.HintFarb;
                            goto IL_0013;
                        case 2994:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0978;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 55)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0978;
                                }
                                if (Information.Err().Number == 91)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0978;
                                }
                                if (Information.Err().Number == 75)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0978;
                                }
                                if (Information.Err().Number == 70)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0978;
                                }
                                if (Information.Err().Number == 53)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0978;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_097c;
                            }
                        end_IL_0000:
                            break;
                        IL_0013:
                            num = 2;
                            WindowState = MyProject.Forms.Druck.WindowState;
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            _Modul1.Instance.Feg = (short)_Modul1.Instance.Persistence.ReadIntInit("state");
                            _Modul1.Instance.Fs = _Modul1.Instance.Feg switch
                            {
                                0 => 7.8f,
                                1 => 8.7f,
                                2 => 9.5f,
                                3 => 10.3f,
                                4 => 11f,
                                5 => 11.7f,
                                6 => 12.4f,
                                7 => 13.2f,
                                8 => 14.9f,
                                9 => 16.5f,
                                _ => _Modul1.Instance.Fs,
                            };
                            goto IL_01bd;
                        IL_01bd: // <========== 12
                            num = 44;
                            Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            FileSystem.MkDir(_Modul1.Instance.Verz + "Stamm");
                            hinter = MyProject.Forms.Hinter;
                            hinter.Att(_Modul1.Instance.Verz + "Stamm");
                            FileSystem.Kill(_Modul1.Instance.Verz + "Stamm\\*.*");
                            _Modul1.Instance.Verz1 = _Modul1.Instance.Verz.Left(15);
                            source = _Modul1.Instance.GenFreeDir + "INIT\\GedAUS.mdb";
                            destination = _Modul1.Instance.Verz + "Stamm\\GEDAUS.mdb";
                            FileSystem.FileCopy(source, destination);
                            name = _Modul1.Instance.Verz + "Stamm\\GEDAUS.mdb";
                            DataModul.NB = UpgradeSupport.DAODBEngine_definst.OpenDatabase(name, false, false, "");
                            BiTable = DataModul.NB.OpenRecordset(dbTables.Bilder, RecordsetTypeEnum.dbOpenTable);
                            DataModul.NB_PersonTable = DataModul.NB.OpenRecordset(dbTables.Personen1, RecordsetTypeEnum.dbOpenTable);
                            DataModul.NB_FamilyTable = DataModul.NB.OpenRecordset(dbTables.Familie1, RecordsetTypeEnum.dbOpenTable);
                            Show();
                            MyProject.Forms.Hinter.Visible = false;
                            _Modul1.Instance.Ind1 = "";
                            _Modul1.Instance.Verz1 = _Modul1.Instance.Verz.Left(15);
                            if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, 2, 1, ref LoopForResult, ref CounterResult))
                            {
                                while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult, ref CounterResult))
                                {
                                    Anz[(short)CounterResult.AsInt()].Width = Width - 50;
                                    Anz[(short)CounterResult.AsInt()].Height = Height - 100;
                                    Anz[(short)CounterResult.AsInt()].RightMargin = Anz[(short)CounterResult.AsInt()].Width - 20;
                                    Anz[(short)CounterResult.AsInt()].Top = 0;
                                    Anz[(short)CounterResult.AsInt()].Left = 0;
                                }
                            }
                            Button1.Top = Anz[0].Height + 10;
                            Button2.Top = Anz[0].Height + 10;
                            Button3.Top = Anz[0].Height + 10;
                            Button4.Top = Anz[0].Height + 10;
                            Button5.Top = Anz[0].Height + 10;
                            Button6.Top = Anz[0].Height + 10;
                            Button7.Top = Anz[0].Height + 10;
                            Button8.Top = Anz[0].Height + 10;
                            _Modul1.Instance.Dateienopen();
                            ProjectData.ClearProjectError();
                            num3 = 0;
                            persInArb = (int)Math.Round(Interaction.InputBox("Nummer der Person, Leer/Abbruch = Suche nach Namen").AsDouble());
                            _Modul1.Instance.PersInArb = persInArb;
                            if (_Modul1.Instance.PersInArb == 0)
                            {
                                _Modul1.Instance.Schalt = 0;
                                _Modul1.Instance.Schalt = 3;
                                MyProject.Forms.Namensuch.ShowDialog();
                                _Modul1.Instance.PersInArb = _Modul1.Instance.Suchper;
                                if (_Modul1.Instance.Suchper == 0)
                                {
                                    MyProject.Forms.Druck.Show();
                                    goto end_IL_0000_2;
                                }
                                _Modul1.Instance.Schalt = 0;
                            }
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            if (_Modul1.Instance.PersInArb == 0)
                            {
                                Close();
                                MyProject.Forms.Druck.Show();
                                goto end_IL_0000_2;
                            }
                            MaxGen = (byte)Math.Round(Interaction.InputBox("Wieviele Generationen maximal").AsDouble());
                            if (MaxGen == 0)
                            {
                                MaxGen = 100;
                            }
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            text = (_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Kont[3].TrimEnd() + " " + _Modul1.Instance.Kont[0].TrimEnd().ToUpper() + " " + _Modul1.Instance.Kont[2].TrimEnd()).Trim();
                            Frame1.Visible = true;
                            Label1[0].Text = text;
                            Label1[3].Text = "Stammfolgeliste für " + text;
                            PerPos = 0;
                            _Modul1.Instance.Datles(_Modul1.Instance.PersInArb, out var asPersDates);
                            Label1[5].Text = _Modul1.Instance.IText[3] + " " + _Modul1.Instance.Kont[11];
                            Label1[1].Text = _Modul1.Instance.IText[4] + " " + _Modul1.Instance.Kont[12];
                            Label1[2].Text = _Modul1.Instance.IText[5] + " " + _Modul1.Instance.Kont[13];
                            Label1[4].Text = _Modul1.Instance.IText[6] + " " + _Modul1.Instance.Kont[14];
                            goto end_IL_0000_2;
                        IL_0978: // <========== 6
                            num4 = unchecked(num2 + 1);
                            goto IL_097c;
                        IL_097c:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 12:
                                case 16:
                                case 19:
                                case 22:
                                case 25:
                                case 28:
                                case 31:
                                case 34:
                                case 37:
                                case 40:
                                case 43:
                                case 44:
                                    goto IL_01bd;
                                case 88:
                                case 96:
                                case 113:
                                case 138:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 2994;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 4
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void Bemaus()
    {
        leerweg();
        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
        if (_Modul1.Instance.DAus[72] == "1")
        {
            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
        }
        Anz[0].SelectedText = " {";
        if (_Modul1.Instance.DAus[70] == "0")
        {
            _Modul1.Instance.UbgT1 = _Modul1.Instance.Retweg(_Modul1.Instance.UbgT1);
        }
        Anz[0].SelectedText = _Modul1.Instance.UbgT1;
        Anz[0].SelectedText = "}";
        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
    }

    public void Sonstdat()
    {
        int try0000_dispatch = -1;
        int num = default;
        string text = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        int num5 = default;
        string Job = default;
        int num6 = default;
        string text2 = default;
        string value = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    object[] array;
                    int ortNr;
                    byte Schalt;
                    int Nr;
                    EEventArt _eArt;
                    short LfNR;
                    int AAA;
                    string LD;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            text = "";
                            goto IL_0009;
                        case 4238:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0e48;
                                    default:
                                        goto end_IL_0000;
                                }
                                lErl = 200;
                                if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_0e4c;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            num = 2;
                            Job = "";
                            array = new object[6];
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            num6 = List3.Items.Count - 1;
                            num5 = 0;
                            goto IL_0ddd;
                        IL_00b2: // <========== 3
                            num = 13;
                            _Modul1.Instance.LfNR = Conversions.ToByte(List3.Items[num5].AsString().Right(10));
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", "105", _Modul1.Instance.PersInArb.AsString(), _Modul1.Instance.LfNR);
                            if (Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                            {
                                Interaction.MsgBox("Stop 14");
                            }
                            M1_J = 0;
                            while (unchecked(M1_J) <= 15u)
                            {
                                _Modul1.Instance.Kont1[M1_J] = "";
                                M1_J = (byte)unchecked((uint)(M1_J + 1));
                            }
                            _Modul1.Instance.sDatu = "";
                            if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch
                                      | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb)
                                      | (DataModul.DB_EventTable.Fields[EventFields.Art].AsString() != "105")))
                            {
                                DataModul.DB_EventTable.Index = "ArtNr";
                                goto end_IL_0000_2;
                            }
                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].Value.AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                LD = "";
                                DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                                if (_Modul1.Instance.Kont[0] != "")
                                {
                                    _Modul1.Instance.Kont1[7] = " " + _Modul1.Instance.Kont[0].Trim();
                                }
                            }
                            goto IL_030d;
                        IL_030d: // <========== 3
                            num = 33;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                            {
                                _Modul1.Instance.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                _Modul1.Instance.sDatu = "00000000" + _Modul1.Instance.sDatu.Right(8);
                                text2 = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                if ((text2.Trim() == "")
                                      & DataModul.DB_EventTable.Fields[EventFields.DatumB].AsDate() != default)
                                {
                                    text2 = " von";
                                }
                                _Modul1.Instance.Datwand1(ref _Modul1.Instance.sDatu, text2);
                                _Modul1.Instance.Kont1[1] = _Modul1.Instance.sDatu;
                            }
                            goto IL_0421;
                        IL_0421:
                            num = 43;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                _Modul1.Instance.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                _Modul1.Instance.sDatu = "00000000" + _Modul1.Instance.sDatu.Right(8);
                                text2 = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                _Modul1.Instance.Datwand1(ref _Modul1.Instance.sDatu, text2);
                                if (_Modul1.Instance.sDatu.Trim() != "")
                                {
                                    _Modul1.Instance.sDatu = " bis " + _Modul1.Instance.sDatu.Trim();
                                }
                                goto IL_04ff;
                            }
                            goto IL_054e;
                        IL_04ff:
                            num = 51;
                            if (_Modul1.Instance.Kont1[1].Trim() != "")
                            {
                                _Modul1.Instance.Kont1[1] = " von " + _Modul1.Instance.Kont1[1].Trim();
                            }
                            goto IL_053e;
                        IL_053e:
                            num = 54;
                            _Modul1.Instance.Kont1[3] = _Modul1.Instance.sDatu;
                            goto IL_054e;
                        IL_054e: // <========== 3
                            num = 56;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.DatumText].Value))
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt() > 0)
                                {
                                    value = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsString();
                                    AAA = value.AsInt();
                                    LD = "";
                                    DataModul.Textlese(AAA, out _Modul1.Instance.UbgT, out LD);
                                    value = AAA.AsString();
                                    if (_Modul1.Instance.UbgT.Trim() != "")
                                    {
                                        _Modul1.Instance.Kont1[3] = _Modul1.Instance.Kont1[3] + " (" + _Modul1.Instance.UbgT.Trim() + ")";
                                        _Modul1.Instance.UbgT = "";
                                    }
                                }
                            }
                            goto IL_0649;
                        IL_0649: // <========== 3
                            num = 66;
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                            {
                                ortNr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble());
                                _Modul1.Instance.UbgT = _Modul1.Instance.ortles(ortNr, 1);
                                _Modul1.Instance.Kont1[5] = " " + _Modul1.Instance.UbgT.Trim();
                                _Modul1.Instance.UbgT = "";
                            }
                            goto IL_06df;
                        IL_06df:
                            num = 71;
                            if (DataModul.DB_EventTable.Fields[EventFields.Platz].Value.AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                LD = "";
                                DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                                if (_Modul1.Instance.Kont[0] != "")
                                {
                                    _Modul1.Instance.Kont1[6] = " " + _Modul1.Instance.Kont[0].Trim();
                                }
                            }
                            goto IL_0797;
                        IL_0797: // <========== 3
                            num = 77;
                            Nr = _Modul1.Instance.PersInArb;
                            _eArt = EEventArt.eA_105;
                            LfNR = _Modul1.Instance.LfNR;
                            _Modul1.Instance.QuellenDatum(ref Nr, _eArt, ref LfNR);
                            _Modul1.Instance.LfNR = Conversions.ToByte(LfNR);
                            _Modul1.Instance.PersInArb = Nr.AsInt();
                            if (((_Modul1.Instance.DAus[38] == "1") & !M1_Ki) | ((_Modul1.Instance.DAus[42] == "1") & M1_Ki))
                            {
                                Job = _Modul1.Instance.Kont1[7].TrimEnd();
                                goto IL_089c;
                            }
                            if (((_Modul1.Instance.DAus[39] == "1") & !M1_Ki) | ((_Modul1.Instance.DAus[43] == "1") & M1_Ki))
                            {
                                Job = "";
                                Job = Module2.Jobdreh(Job);
                                Job += text;
                                text = "";
                            }
                            goto IL_089c;
                        IL_089c: // <========== 3
                            num = 87;
                            leerweg();
                            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectionCharOffset = 0;
                            if (_Modul1.Instance.DAus[106].AsDouble() == 1.0)
                            {
                                if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
                                {
                                    Anz[0].SelectedText = "\n";
                                }
                                goto IL_095c;
                            }
                            Anz[0].SelectedText = " ";
                            goto IL_0997;
                        IL_095c:
                            num = 94;
                            Anz[0].SelectionIndent = Abst;
                            goto IL_0997;
                        IL_0997: // <========== 3
                            num = 99;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value))
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.ArtText].Value.AsDouble() > 0.0)
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                                    LD = "";
                                    DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                                    if (_Modul1.Instance.Kont[0] != "")
                                    {
                                        _Modul1.Instance.Kont1[10] = " " + _Modul1.Instance.Kont[0].Trim() + ": ";
                                    }
                                }
                            }
                            goto IL_0a81;
                        IL_0a81: // <========== 3
                            num = 107;
                            if (_Modul1.Instance.Kont1[10].Trim() != "")
                            {
                                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Underline);
                                Anz[0].SelectedText = _Modul1.Instance.Kont1[10].Trim();
                                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            }
                            Anz[0].SelectedText = " " + Job.Trim() + ". ";
                            if (((_Modul1.Instance.DAus[40] == "1") & !M1_Ki) | ((_Modul1.Instance.DAus[44] == "1") & M1_Ki))
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    _Modul1.Instance.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim();
                                    Bemaus();
                                }
                            }
                            goto IL_0bfc;
                        IL_0bfc: // <========== 3
                            num = 119;
                            if (((_Modul1.Instance.DAus[41] == "1") & !M1_Ki) | ((_Modul1.Instance.DAus[45] == "1") & M1_Ki))
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    _Modul1.Instance.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim();
                                    Bemaus();
                                }
                            }
                            goto IL_0ca9;
                        IL_0ca9: // <========== 3
                            num = 125;
                            if (_Modul1.Instance.Kont1[9].Trim() != "")
                            {
                                Anz[0].SelectionCharOffset = _Modul1.Instance.PrintDat.Hoch;
                                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                Anz[0].SelectedText = _Modul1.Instance.Kont1[9];
                                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            }
                            Anz[0].SelectionCharOffset = 0;
                            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            DataModul.DB_EventTable.MoveNext();
                            num5++;
                            goto IL_0ddd;
                        IL_0ddd:
                            if (num5 > num6)
                            {
                                goto end_IL_0000_2;
                            }
                            if (_Modul1.Instance.DAus[106].AsDouble() == 1.0)
                            {
                                Anz[0].SelectedText = "\n";
                                Anz[0].SelectionIndent = Abst;

                            }
                            else
                            {
                                Anz[0].SelectedText = " ";
                            }
                            goto IL_00b2;
                        IL_0e48:
                            num4 = unchecked(num2 + 1);
                            goto IL_0e4c;
                        IL_0e4c:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 9:
                                case 12:
                                case 13:
                                    goto IL_00b2;
                                case 31:
                                case 32:
                                case 33:
                                    goto IL_030d;
                                case 42:
                                case 43:
                                    goto IL_0421;
                                case 50:
                                case 51:
                                    goto IL_04ff;
                                case 53:
                                case 54:
                                    goto IL_053e;
                                case 55:
                                case 56:
                                    goto IL_054e;
                                case 63:
                                case 64:
                                case 65:
                                case 66:
                                    goto IL_0649;
                                case 70:
                                case 71:
                                    goto IL_06df;
                                case 75:
                                case 76:
                                case 77:
                                    goto IL_0797;
                                case 80:
                                case 86:
                                case 87:
                                    goto IL_089c;
                                case 93:
                                case 94:
                                    goto IL_095c;
                                case 95:
                                case 98:
                                case 99:
                                    goto IL_0997;
                                case 104:
                                case 105:
                                case 106:
                                case 107:
                                    goto IL_0a81;
                                case 117:
                                case 118:
                                case 119:
                                    goto IL_0bfc;
                                case 123:
                                case 124:
                                case 125:
                                    goto IL_0ca9;
                                case 25:
                                case 135:
                                case 141:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 4238;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 3
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    private void Stammfolge_FormClosing(object eventSender, FormClosingEventArgs eventArgs)
    {
        //Discarded unreachable code: IL_0134
        int try0000_dispatch = -1;
        int num = default;
        bool cancel = default;
        int num2 = default;
        int num3 = default;
        CloseReason closeReason = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                switch (try0000_dispatch)
                {
                    default:
                        num = 1;
                        cancel = eventArgs.Cancel;
                        goto IL_000a;
                    case 438:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0138;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_00f8;
                        }
                    IL_00f8:
                        num = 22;
                        if (Information.Err().Number != 91)
                        {
                            break;
                        }
                        goto IL_010a;
                    IL_010a:
                        num = 23;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_0138;
                    IL_009b:
                        num = 11;
                        DataModul.DSB.Close();
                        ProjectData.EndApp();
                        goto end_IL_0000_2;
                    IL_0138:
                        num4 = num2 + 1;
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_000a;
                            case 3:
                                goto IL_0014;
                            case 4:
                                goto IL_001c;
                            case 5:
                                goto IL_0030;
                            case 6:
                                goto IL_004c;
                            case 7:
                                goto IL_006b;
                            case 8:
                                goto IL_0072;
                            case 9:
                                goto IL_007f;
                            case 10:
                                goto IL_008d;
                            case 11:
                                goto IL_009b;
                            case 15:
                                num = 15;
                                if (closeReason == CloseReason.None)
                                {
                                    goto case 16;
                                }
                                goto IL_00f8;
                            case 16:
                                num = 16;
                                DataModul.MandDB.Close();
                                DataModul.DOSB.Close();
                                DataModul.TempDB.Close();
                                DataModul.DSB.Close();
                                ProjectData.EndApp();
                                goto IL_00f8;
                            case 20:
                            case 21:
                            case 22:
                                goto IL_00f8;
                            case 23:
                                goto IL_010a;
                            case 24:
                            case 26:
                                goto end_IL_0000_3;
                            default:
                                goto end_IL_0000;
                            case 12:
                            case 13:
                            case 14:
                            case 27:
                                goto end_IL_0000_2;
                        }
                        goto default;
                    IL_000a:
                        num = 2;
                        closeReason = eventArgs.CloseReason;
                        goto IL_0014;
                    IL_0014:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_001c;
                    IL_001c:
                        num = 4;
                        FileSystem.FileClose(6);
                        goto IL_0030;
                    IL_0030:
                        num = 5;
                        FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\Windowstate", OpenMode.Output);
                        goto IL_004c;
                    IL_004c:
                        num = 6;
                        FileSystem.PrintLine(6, WindowState);
                        goto IL_006b;
                    IL_006b:
                        num = 7;
                        if (closeReason != 0)
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_0072;
                    IL_0072:
                        num = 8;
                        DataModul.MandDB.Close();
                        goto IL_007f;
                    IL_007f:
                        num = 9;
                        DataModul.DOSB.Close();
                        goto IL_008d;
                    IL_008d:
                        num = 10;
                        DataModul.TempDB.Close();
                        goto IL_009b;
                    end_IL_0000_3:
                        break;
                }
                num = 26;
                eventArgs.Cancel = cancel;
                break;
            end_IL_0000:;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 438;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    public void leerweg()
    {
        while (Anz[0].SelectionStart > 0 && Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == " ")
        {
            Anz[0].SelectionStart = checked(Anz[0].SelectionStart - 1);
            Anz[0].SelectionLength = 1;
            Anz[0].SelectedText = "";
        }
        Anz[0].SelectionCharOffset = 0;
    }

    public void SchwiegDat()
    {
        leerweg();
        _Modul1.Instance.Datschalt = 0;
        short Listart = 0;
        string Ahne = 0.AsString();
        bool neb = false;
        _Modul1.Instance.Datles3(Listart, default, default, ref neb);
        if ((_Modul1.Instance.Kont[11].Trim() != "") | (_Modul1.Instance.Kont[12].Trim() != ""))
        {
            if (_Modul1.Instance.Kont[11].Trim() != "")
            {
                Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[1];
                Anz[0].SelectedText = " " + _Modul1.Instance.Kont[11].Trim() + " ";
            }
            else
            {
                Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[2];
                Anz[0].SelectedText = " " + _Modul1.Instance.Kont[12].Trim() + " ";
            }
        }
        leerweg();
        if ((_Modul1.Instance.Kont[13].Trim() != "") | (_Modul1.Instance.Kont[14].Trim() != ""))
        {
            if (_Modul1.Instance.Kont[13].Trim() != "")
            {
                Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[3];
                Anz[0].SelectedText = " " + _Modul1.Instance.Kont[13].Trim() + " ";
            }
            else
            {
                Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[4];
                Anz[0].SelectedText = " " + _Modul1.Instance.Kont[14].Trim() + " ";
            }
        }
        _Modul1.Instance.Datschalt = 1;
    }

    public void Eltles(int persInArb, int iFam)
    {

        ProjectData.ClearProjectError();
        var Pattext = "";
        var _eArt = EEventArt.eA_Unknown;
        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
        _Modul1.Instance.eLKennz = DataModul.Person.GetSex(persInArb) == "F" ? ELinkKennz.lkFather : ELinkKennz.lkMother;
        if (_Modul1.Instance.eLKennz.AsDouble() == 2.0)
        {
            Anz[0].SelectedText = " (Sohn von ";
        }
        if (_Modul1.Instance.eLKennz.AsDouble() == 1.0)
        {
            Anz[0].SelectedText = " (Tochter von ";
        }
        _Modul1.Instance.FamInArb = iFam;
        DataModul.NB_FamilyTable.AddNew();
        DataModul.NB_FamilyTable.Fields[IndexFields.Fam].Value = _Modul1.Instance.FamInArb;
        DataModul.NB_FamilyTable.Update();
        if (DataModul.Link.GetFamPerson(_Modul1.Instance.FamInArb, ELinkKennz.lkFather, out persInArb))
        {
            _Modul1.Instance.Family.Mann = persInArb;
            _Modul1.Instance.Person_ReadNames(persInArb, _Modul1.Instance.Person);
            Namenindex();
            _Modul1.Instance.Person_ReadNames(persInArb, _Modul1.Instance.Person);
            _Modul1.Instance.Person.SetFullSurname(_Modul1.Instance.BuildFullSurName(_Modul1.Instance.Person, _Modul1.Instance.DAus[89] == "1"));
            _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].TrimEnd();
            Anz[0].SelectedText = _Modul1.Instance.Kont[3] + " ";
            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Bold);
            Anz[0].SelectedText = _Modul1.Instance.Kont[0];
            leerweg();
            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            Anz[0].SelectedText = " ";
            if (_Modul1.Instance.Kont[4] != "")
            {
                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Italic);
                Anz[0].SelectedText = "(" + _Modul1.Instance.Kont[4].TrimEnd() + ")";
                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                Anz[0].SelectedText = " ";
            }
            leerweg();
            if (_Modul1.Instance.DAus[78] == "1")
            {
                short Listart = 1;
                var neb = false;
                _Modul1.Instance.Datles3(Listart, default, _eArt, ref neb);
                Datschreib(ref Pattext, 1);
                leerweg();
                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble()), FontStyle.Regular);
                _eArt = EEventArt.eA_300;
                Berufe(_eArt, 0f);
                _eArt = EEventArt.eA_301;
                Berufe(_eArt, 0f);
            }
            _Modul1.Instance.Ind1 = _M_ind + "." + Strings.Trim(checked(IQ + 1).AsString());
            _Modul1.Instance.Aschalt = _Modul1.Instance.Datschalt;
            _Modul1.Instance.Datschalt = checked((byte)Math.Round(_Modul1.Instance.Aschalt));
            DGSchalt = false;
        }
        else
            Anz[0].SelectedText = "unbekanntem Vater ";
        DataModul.NB_FamilyTable.AddNew();
        DataModul.NB_FamilyTable.Fields[IndexFields.Fam].Value = _Modul1.Instance.FamInArb;
        DataModul.NB_FamilyTable.Update();
        if (DataModul.Link.GetFamPerson(_Modul1.Instance.FamInArb, ELinkKennz.lkMother, out persInArb))
        {
            _Modul1.Instance.Family.Frau = persInArb;
            _Modul1.Instance.Person_ReadNames(persInArb, _Modul1.Instance.Person);
            Namenindex();
            _Modul1.Instance.Person_ReadNames(persInArb, _Modul1.Instance.Person);
            _Modul1.Instance.Person.SetFullSurname(_Modul1.Instance.BuildFullSurName(_Modul1.Instance.Person, _Modul1.Instance.DAus[89] == "1"));
            _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].TrimEnd();
            leerweg();
            Anz[0].SelectedText = " ";
            if ((_Modul1.Instance.Kont[0].Trim() == "") & (_Modul1.Instance.Kont[3].Trim() == ""))
            {
                Anz[0].SelectedText = "und unbekannter Mutter";
            }
            else
            {
                leerweg();
                if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == ".")
                {
                    Anz[0].SelectionStart = checked(Anz[0].SelectionStart - 1);
                    Anz[0].SelectionLength = 1;
                    Anz[0].SelectedText = ",";
                }
                Anz[0].SelectedText = " und " + (_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Kont[3]).Trim();
                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Bold);
                Anz[0].SelectedText = " " + _Modul1.Instance.Kont[0];
                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                if (_Modul1.Instance.DAus[78] == "1")
                {
                    var lis = (short)1;
                    var neb = false;
                    _Modul1.Instance.Datles3(lis, default, default, ref neb);
                    Datschreib(ref Pattext, 0);
                    leerweg();
                    Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                    _eArt = EEventArt.eA_300;
                    Berufe(_eArt);
                    _eArt = EEventArt.eA_301;
                    Berufe(_eArt);
                }
                _Modul1.Instance.Ind1 = _M_ind + "." + Strings.Trim(checked(IQ + 1).AsString());
                _Modul1.Instance.Aschalt = _Modul1.Instance.Datschalt;
            }
        }
        else
        {
            leerweg();
            Anz[0].SelectedText = " ";
            Anz[0].SelectedText = "und unbekannter Mutter";
        }
        Anz[0].SelectedText = ".)";
    }
    public void Datschreib(ref string Pattext, byte umb)
    {
        int try0000_dispatch = -1;
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
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0007;
                    case 6767:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_1647;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_15fc;
                        }
                    IL_1647:
                        num4 = num2 + 1;
                        goto IL_164a;
                    IL_15fc:
                        num = 258;
                        if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                        {
                            ProjectData.EndApp();
                        }
                        goto IL_1624;
                    IL_1560:
                        num = 252;
                        Anz[0].SelectedText = " " + _Modul1.Instance.Kont[54].Trim() + ".";
                        goto IL_1593;
                    IL_1624:
                        num = 261;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        num4 = num2;
                        goto IL_164a;
                    IL_1593:
                        num = 253;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_15c5;
                    IL_15c5:
                        num = 256;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto end_IL_0000_2;
                    IL_164a:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_0007;
                            case 3:
                                goto IL_006a;
                            case 4:
                                goto IL_0084;
                            case 5:
                                goto IL_008a;
                            case 6:
                            case 7:
                                goto IL_00a2;
                            case 9:
                                goto IL_00bd;
                            case 10:
                                goto IL_00c0;
                            case 8:
                            case 11:
                            case 12:
                                goto IL_00d9;
                            case 13:
                                goto IL_00f4;
                            case 14:
                                goto IL_0112;
                            case 15:
                            case 16:
                                goto IL_0142;
                            case 17:
                                goto IL_0163;
                            case 18:
                                goto IL_017c;
                            case 19:
                                goto IL_01b6;
                            case 20:
                                goto IL_01d7;
                            case 21:
                                goto IL_01ec;
                            case 22:
                            case 23:
                                goto IL_021b;
                            case 24:
                                goto IL_0234;
                            case 25:
                                goto IL_0252;
                            case 26:
                            case 27:
                            case 28:
                                goto IL_027d;
                            case 29:
                                goto IL_0295;
                            case 30:
                                goto IL_02a8;
                            case 31:
                                goto IL_02b8;
                            case 32:
                            case 33:
                            case 34:
                                goto IL_02c1;
                            case 35:
                                goto IL_02d9;
                            case 36:
                                goto IL_02f7;
                            case 37:
                                goto IL_0307;
                            case 38:
                            case 39:
                            case 40:
                            case 41:
                                goto IL_0310;
                            case 42:
                                goto IL_0325;
                            case 43:
                                goto IL_0346;
                            case 44:
                                goto IL_034f;
                            case 45:
                                goto IL_0368;
                            case 46:
                                goto IL_0381;
                            case 48:
                                goto IL_03bd;
                            case 49:
                                goto IL_03c0;
                            case 47:
                            case 50:
                            case 51:
                                goto IL_03ef;
                            case 52:
                                goto IL_0408;
                            case 53:
                                goto IL_0421;
                            case 55:
                                goto IL_045d;
                            case 56:
                                goto IL_0460;
                            case 54:
                            case 57:
                            case 58:
                                goto IL_048f;
                            case 59:
                                goto IL_04bf;
                            case 60:
                            case 61:
                            case 62:
                                goto IL_04ee;
                            case 63:
                                goto IL_0552;
                            case 64:
                                goto IL_056d;
                            case 65:
                                goto IL_0574;
                            case 68:
                                goto IL_058f;
                            case 69:
                                goto IL_0592;
                            case 66:
                            case 67:
                            case 70:
                            case 71:
                                goto IL_05ab;
                            case 72:
                                goto IL_05c6;
                            case 73:
                                goto IL_05e4;
                            case 74:
                            case 75:
                                goto IL_0614;
                            case 76:
                                goto IL_0635;
                            case 77:
                                goto IL_064e;
                            case 78:
                                goto IL_0688;
                            case 79:
                                goto IL_06a9;
                            case 80:
                            case 81:
                                goto IL_06d8;
                            case 82:
                                goto IL_06f3;
                            case 83:
                                goto IL_0711;
                            case 84:
                            case 85:
                            case 86:
                                goto IL_073c;
                            case 87:
                                goto IL_0754;
                            case 88:
                                goto IL_0767;
                            case 89:
                                goto IL_0777;
                            case 90:
                            case 91:
                            case 92:
                                goto IL_0780;
                            case 93:
                                goto IL_0798;
                            case 94:
                                goto IL_07ab;
                            case 95:
                                goto IL_07bb;
                            case 96:
                            case 97:
                            case 98:
                            case 99:
                                goto IL_07c4;
                            case 100:
                                goto IL_07cd;
                            case 101:
                                goto IL_07e3;
                            case 102:
                                goto IL_07fc;
                            case 103:
                                goto IL_0817;
                            case 105:
                                goto IL_0853;
                            case 106:
                                goto IL_0856;
                            case 104:
                            case 107:
                            case 108:
                                goto IL_0885;
                            case 109:
                                goto IL_089e;
                            case 110:
                                goto IL_08b9;
                            case 112:
                                goto IL_08f5;
                            case 113:
                                goto IL_08f8;
                            case 111:
                            case 114:
                            case 115:
                                goto IL_0927;
                            case 116:
                                goto IL_0951;
                            case 117:
                                goto IL_095b;
                            case 118:
                            case 119:
                                goto IL_098a;
                            case 120:
                                goto IL_099f;
                            case 121:
                                goto IL_09c0;
                            case 122:
                                goto IL_09c9;
                            case 123:
                                goto IL_09e2;
                            case 124:
                                goto IL_09fb;
                            case 126:
                                goto IL_0a37;
                            case 127:
                                goto IL_0a3a;
                            case 125:
                            case 128:
                            case 129:
                                goto IL_0a69;
                            case 130:
                                goto IL_0a85;
                            case 131:
                                goto IL_0aa1;
                            case 133:
                                goto IL_0ae0;
                            case 134:
                                goto IL_0ae6;
                            case 132:
                            case 135:
                            case 136:
                                goto IL_0b18;
                            case 137:
                            case 138:
                            case 139:
                                goto IL_0b4b;
                            case 140:
                                goto IL_0bb2;
                            case 141:
                                goto IL_0bd0;
                            case 142:
                                goto IL_0bda;
                            case 145:
                                goto IL_0bf8;
                            case 146:
                                goto IL_0bfe;
                            case 143:
                            case 144:
                            case 147:
                            case 148:
                                goto IL_0c1a;
                            case 149:
                                goto IL_0c38;
                            case 150:
                                goto IL_0c59;
                            case 151:
                            case 152:
                                goto IL_0c8c;
                            case 153:
                                goto IL_0cb0;
                            case 154:
                                goto IL_0ccc;
                            case 155:
                                goto IL_0d09;
                            case 156:
                                goto IL_0d2d;
                            case 157:
                                goto IL_0d45;
                            case 158:
                            case 159:
                                goto IL_0d77;
                            case 160:
                                goto IL_0d95;
                            case 161:
                                goto IL_0db6;
                            case 162:
                            case 163:
                            case 164:
                                goto IL_0de4;
                            case 165:
                                goto IL_0dff;
                            case 166:
                                goto IL_0e15;
                            case 167:
                                goto IL_0e28;
                            case 168:
                            case 169:
                            case 170:
                                goto IL_0e34;
                            case 171:
                                goto IL_0e4f;
                            case 172:
                                goto IL_0e65;
                            case 173:
                                goto IL_0e78;
                            case 174:
                            case 175:
                            case 176:
                            case 177:
                                goto IL_0e84;
                            case 178:
                                goto IL_0e9c;
                            case 179:
                                goto IL_0ec0;
                            case 180:
                                goto IL_0ecc;
                            case 181:
                                goto IL_0ee8;
                            case 182:
                                goto IL_0f04;
                            case 184:
                                goto IL_0f43;
                            case 185:
                                goto IL_0f49;
                            case 183:
                            case 186:
                            case 187:
                                goto IL_0f7b;
                            case 188:
                                goto IL_0f97;
                            case 189:
                                goto IL_0fb3;
                            case 191:
                                goto IL_0ff2;
                            case 192:
                                goto IL_0ff8;
                            case 190:
                            case 193:
                            case 194:
                                goto IL_102a;
                            case 195:
                                goto IL_105d;
                            case 196:
                            case 197:
                            case 198:
                                goto IL_108f;
                            case 199:
                                goto IL_10f6;
                            case 200:
                                goto IL_1114;
                            case 201:
                                goto IL_111e;
                            case 204:
                                goto IL_113c;
                            case 205:
                                goto IL_1142;
                            case 202:
                            case 203:
                            case 206:
                            case 207:
                                goto IL_115e;
                            case 208:
                                goto IL_117c;
                            case 209:
                                goto IL_119d;
                            case 210:
                            case 211:
                                goto IL_11d0;
                            case 212:
                                goto IL_11f4;
                            case 213:
                                goto IL_1210;
                            case 214:
                                goto IL_124d;
                            case 215:
                                goto IL_127b;
                            case 216:
                            case 217:
                                goto IL_12ad;
                            case 218:
                                goto IL_12cb;
                            case 219:
                                goto IL_12ec;
                            case 220:
                            case 221:
                            case 222:
                                goto IL_131a;
                            case 223:
                                goto IL_1335;
                            case 224:
                                goto IL_134b;
                            case 225:
                                goto IL_135e;
                            case 226:
                            case 227:
                            case 228:
                                goto IL_136a;
                            case 229:
                                goto IL_1385;
                            case 230:
                                goto IL_139b;
                            case 231:
                                goto IL_13ae;
                            case 232:
                            case 233:
                            case 234:
                            case 235:
                                goto IL_13ba;
                            case 236:
                                goto IL_13d2;
                            case 237:
                                goto IL_13f6;
                            case 238:
                                goto IL_1402;
                            case 239:
                                goto IL_141e;
                            case 240:
                                goto IL_143a;
                            case 242:
                                goto IL_1479;
                            case 243:
                                goto IL_147f;
                            case 241:
                            case 244:
                            case 245:
                                goto IL_14b1;
                            case 246:
                                goto IL_14cd;
                            case 247:
                                goto IL_14e9;
                            case 249:
                                goto IL_1528;
                            case 250:
                                goto IL_152e;
                            case 248:
                            case 251:
                            case 252:
                                goto IL_1560;
                            case 253:
                                goto IL_1593;
                            case 254:
                            case 255:
                            case 256:
                                goto IL_15c5;
                            case 258:
                                goto IL_15fc;
                            case 259:
                            case 261:
                                goto IL_1624;
                            default:
                                goto end_IL_0000;
                            case 257:
                            case 262:
                                goto end_IL_0000_2;
                        }
                        goto default;
                    IL_0007:
                        num = 2;
                        if ((_Modul1.Instance.Kont[11].Trim() != "") | (_Modul1.Instance.Kont[16].Trim() != "") | (_Modul1.Instance.Kont[21].Trim() != ""))
                        {
                            goto IL_006a;
                        }
                        goto IL_0310;
                    IL_006a:
                        num = 3;
                        if (_Modul1.Instance.DAus[106].AsDouble() == 1.0)
                        {
                            goto IL_0084;
                        }
                        goto IL_00bd;
                    IL_0084:
                        num = 4;
                        if (umb == 0)
                        {
                            goto IL_008a;
                        }
                        goto IL_00a2;
                    IL_008a:
                        num = 5;
                        Anz[0].SelectedText = "\n";
                        goto IL_00a2;
                    IL_00a2:
                        num = 7;
                        Anz[0].SelectionIndent = Abst;
                        goto IL_00d9;
                    IL_00bd:
                        num = 9;
                        goto IL_00c0;
                    IL_00c0:
                        num = 10;
                        Anz[0].SelectedText = " ";
                        goto IL_00d9;
                    IL_00d9:
                        num = 12;
                        Anz[0].SelectedText = _Modul1.Instance.DTxt[1];
                        goto IL_00f4;
                    IL_00f4:
                        num = 13;
                        if (_Modul1.Instance.Kont[11].Trim() != "")
                        {
                            goto IL_0112;
                        }
                        goto IL_0142;
                    IL_0112:
                        num = 14;
                        Anz[0].SelectedText = " " + _Modul1.Instance.Kont[11].Trim() + ".";
                        goto IL_0142;
                    IL_0142:
                        num = 16;
                        if (_Modul1.Instance.Kont[31].Trim() != "")
                        {
                            goto IL_0163;
                        }
                        goto IL_021b;
                    IL_0163:
                        num = 17;
                        Anz[0].SelectionCharOffset = _Modul1.Instance.PrintDat.Hoch;
                        goto IL_017c;
                    IL_017c:
                        num = 18;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                        goto IL_01b6;
                    IL_01b6:
                        num = 19;
                        Anz[0].SelectedText = _Modul1.Instance.Kont[31].Trim();
                        goto IL_01d7;
                    IL_01d7:
                        num = 20;
                        Anz[0].SelectionCharOffset = 0;
                        goto IL_01ec;
                    IL_01ec:
                        num = 21;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_021b;
                    IL_021b:
                        num = 23;
                        if (_Modul1.Instance.DAus[85] == "1")
                        {
                            goto IL_0234;
                        }
                        goto IL_027d;
                    IL_0234:
                        num = 24;
                        if (_Modul1.Instance.Kont[41].Trim() != "")
                        {
                            goto IL_0252;
                        }
                        goto IL_027d;
                    IL_0252:
                        num = 25;
                        Anz[0].SelectedText = " Urkunde: " + _Modul1.Instance.Kont[41].Trim();
                        goto IL_027d;
                    IL_027d:
                        num = 28;
                        if (_Modul1.Instance.DAus[2] == "1")
                        {
                            goto IL_0295;
                        }
                        goto IL_02c1;
                    IL_0295:
                        num = 29;
                        if (_Modul1.Instance.Kont[16].Length > 0)
                        {
                            goto IL_02a8;
                        }
                        goto IL_02c1;
                    IL_02a8:
                        num = 30;
                        _Modul1.Instance.UbgT1 = _Modul1.Instance.Kont[16];
                        goto IL_02b8;
                    IL_02b8:
                        num = 31;
                        Bemaus();
                        goto IL_02c1;
                    IL_02c1:
                        num = 34;
                        if (_Modul1.Instance.DAus[3] == "1")
                        {
                            goto IL_02d9;
                        }
                        goto IL_0310;
                    IL_02d9:
                        num = 35;
                        if (_Modul1.Instance.Kont[21].Trim() != "")
                        {
                            goto IL_02f7;
                        }
                        goto IL_0310;
                    IL_02f7:
                        num = 36;
                        _Modul1.Instance.UbgT1 = _Modul1.Instance.Kont[21];
                        goto IL_0307;
                    IL_0307:
                        num = 37;
                        Bemaus();
                        goto IL_0310;
                    IL_0310:
                        num = 41;
                        if (_Modul1.Instance.DAus[96].AsBool())
                        {
                            goto IL_0325;
                        }
                        goto IL_04ee;
                    IL_0325:
                        num = 42;
                        if (_Modul1.Instance.Kont[51].Trim() != "")
                        {
                            goto IL_0346;
                        }
                        goto IL_04ee;
                    IL_0346:
                        num = 43;
                        leerweg();
                        goto IL_034f;
                    IL_034f:
                        num = 44;
                        Anz[0].SelectedText = " ";
                        goto IL_0368;
                    IL_0368:
                        num = 45;
                        if (_Modul1.Instance.DAus[72] == "1")
                        {
                            goto IL_0381;
                        }
                        goto IL_03bd;
                    IL_0381:
                        num = 46;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
                        goto IL_03ef;
                    IL_03bd:
                        num = 48;
                        goto IL_03c0;
                    IL_03c0:
                        num = 49;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Underline);
                        goto IL_03ef;
                    IL_03ef:
                        num = 51;
                        Anz[0].SelectedText = "Zeugen:";
                        goto IL_0408;
                    IL_0408:
                        num = 52;
                        if (_Modul1.Instance.DAus[72] == "1")
                        {
                            goto IL_0421;
                        }
                        goto IL_045d;
                    IL_0421:
                        num = 53;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                        goto IL_048f;
                    IL_045d:
                        num = 55;
                        goto IL_0460;
                    IL_0460:
                        num = 56;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_048f;
                    IL_048f:
                        num = 58;
                        Anz[0].SelectedText = " " + _Modul1.Instance.Kont[51].Trim() + ".";
                        goto IL_04bf;
                    IL_04bf:
                        num = 59;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_04ee;
                    IL_04ee:
                        num = 62;
                        if ((_Modul1.Instance.Kont[12].Trim() != "") | (_Modul1.Instance.Kont[17].Trim() != "") | (_Modul1.Instance.Kont[22].Trim() != ""))
                        {
                            goto IL_0552;
                        }
                        goto IL_07c4;
                    IL_0552:
                        num = 63;
                        if (_Modul1.Instance.DAus[106].AsDouble() == 1.0)
                        {
                            goto IL_056d;
                        }
                        goto IL_058f;
                    IL_056d:
                        num = 64;
                        if (umb == 0)
                        {
                            goto IL_0574;
                        }
                        goto IL_05ab;
                    IL_0574:
                        num = 65;
                        Anz[0].SelectedText = "\n";
                        goto IL_05ab;
                    IL_058f:
                        num = 68;
                        goto IL_0592;
                    IL_0592:
                        num = 69;
                        Anz[0].SelectedText = " ";
                        goto IL_05ab;
                    IL_05ab:
                        num = 71;
                        Anz[0].SelectedText = _Modul1.Instance.DTxt[2];
                        goto IL_05c6;
                    IL_05c6:
                        num = 72;
                        if (_Modul1.Instance.Kont[12].Trim() != "")
                        {
                            goto IL_05e4;
                        }
                        goto IL_0614;
                    IL_05e4:
                        num = 73;
                        Anz[0].SelectedText = " " + _Modul1.Instance.Kont[12].Trim() + ".";
                        goto IL_0614;
                    IL_0614:
                        num = 75;
                        if (_Modul1.Instance.Kont[32].Trim() != "")
                        {
                            goto IL_0635;
                        }
                        goto IL_06d8;
                    IL_0635:
                        num = 76;
                        Anz[0].SelectionCharOffset = _Modul1.Instance.PrintDat.Hoch;
                        goto IL_064e;
                    IL_064e:
                        num = 77;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                        goto IL_0688;
                    IL_0688:
                        num = 78;
                        Anz[0].SelectedText = _Modul1.Instance.Kont[32].Trim();
                        goto IL_06a9;
                    IL_06a9:
                        num = 79;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_06d8;
                    IL_06d8:
                        num = 81;
                        if (_Modul1.Instance.DAus[85].AsDouble() == 1.0)
                        {
                            goto IL_06f3;
                        }
                        goto IL_073c;
                    IL_06f3:
                        num = 82;
                        if (_Modul1.Instance.Kont[42].Trim() != "")
                        {
                            goto IL_0711;
                        }
                        goto IL_073c;
                    IL_0711:
                        num = 83;
                        Anz[0].SelectedText = " Urkunde: " + _Modul1.Instance.Kont[42].Trim();
                        goto IL_073c;
                    IL_073c:
                        num = 86;
                        if (_Modul1.Instance.DAus[2] == "1")
                        {
                            goto IL_0754;
                        }
                        goto IL_0780;
                    IL_0754:
                        num = 87;
                        if (_Modul1.Instance.Kont[17].Length > 0)
                        {
                            goto IL_0767;
                        }
                        goto IL_0780;
                    IL_0767:
                        num = 88;
                        _Modul1.Instance.UbgT1 = _Modul1.Instance.Kont[17];
                        goto IL_0777;
                    IL_0777:
                        num = 89;
                        Bemaus();
                        goto IL_0780;
                    IL_0780:
                        num = 92;
                        if (_Modul1.Instance.DAus[3] == "1")
                        {
                            goto IL_0798;
                        }
                        goto IL_07c4;
                    IL_0798:
                        num = 93;
                        if (_Modul1.Instance.Kont[22].Length > 0)
                        {
                            goto IL_07ab;
                        }
                        goto IL_07c4;
                    IL_07ab:
                        num = 94;
                        _Modul1.Instance.UbgT1 = _Modul1.Instance.Kont[22];
                        goto IL_07bb;
                    IL_07bb:
                        num = 95;
                        Bemaus();
                        goto IL_07c4;
                    IL_07c4:
                        num = 99;
                        leerweg();
                        goto IL_07cd;
                    IL_07cd:
                        num = 100;
                        if (Pattext != "")
                        {
                            goto IL_07e3;
                        }
                        goto IL_098a;
                    IL_07e3:
                        num = 101;
                        Anz[0].SelectedText = " ";
                        goto IL_07fc;
                    IL_07fc:
                        num = 102;
                        if (_Modul1.Instance.DAus[100].AsDouble() == 1.0)
                        {
                            goto IL_0817;
                        }
                        goto IL_0853;
                    IL_0817:
                        num = 103;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
                        goto IL_0885;
                    IL_0853:
                        num = 105;
                        goto IL_0856;
                    IL_0856:
                        num = 106;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Underline);
                        goto IL_0885;
                    IL_0885:
                        num = 108;
                        Anz[0].SelectedText = "Paten:";
                        goto IL_089e;
                    IL_089e:
                        num = 109;
                        if (_Modul1.Instance.DAus[100].AsDouble() == 1.0)
                        {
                            goto IL_08b9;
                        }
                        goto IL_08f5;
                    IL_08b9:
                        num = 110;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                        goto IL_0927;
                    IL_08f5:
                        num = 112;
                        goto IL_08f8;
                    IL_08f8:
                        num = 113;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_0927;
                    IL_0927:
                        num = 115;
                        Anz[0].SelectedText = " " + Pattext.Trim() + ".";
                        goto IL_0951;
                    IL_0951:
                        num = 116;
                        Pattext = "";
                        goto IL_095b;
                    IL_095b:
                        num = 117;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_098a;
                    IL_098a:
                        num = 119;
                        if (_Modul1.Instance.DAus[96].AsBool())
                        {
                            goto IL_099f;
                        }
                        goto IL_0b4b;
                    IL_099f:
                        num = 120;
                        if (_Modul1.Instance.Kont[52].Trim() != "")
                        {
                            goto IL_09c0;
                        }
                        goto IL_0b4b;
                    IL_09c0:
                        num = 121;
                        leerweg();
                        goto IL_09c9;
                    IL_09c9:
                        num = 122;
                        Anz[0].SelectedText = " ";
                        goto IL_09e2;
                    IL_09e2:
                        num = 123;
                        if (_Modul1.Instance.DAus[72] == "1")
                        {
                            goto IL_09fb;
                        }
                        goto IL_0a37;
                    IL_09fb:
                        num = 124;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
                        goto IL_0a69;
                    IL_0a37:
                        num = 126;
                        goto IL_0a3a;
                    IL_0a3a:
                        num = 127;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Underline);
                        goto IL_0a69;
                    IL_0a69:
                        num = 129;
                        Anz[0].SelectedText = "Zeugen:";
                        goto IL_0a85;
                    IL_0a85:
                        num = 130;
                        if (_Modul1.Instance.DAus[72] == "1")
                        {
                            goto IL_0aa1;
                        }
                        goto IL_0ae0;
                    IL_0aa1:
                        num = 131;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                        goto IL_0b18;
                    IL_0ae0:
                        num = 133;
                        goto IL_0ae6;
                    IL_0ae6:
                        num = 134;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_0b18;
                    IL_0b18:
                        num = 136;
                        Anz[0].SelectedText = " " + _Modul1.Instance.Kont[52].Trim() + ".";
                        goto IL_0b4b;
                    IL_0b4b:
                        num = 139;
                        if ((_Modul1.Instance.Kont[13].Trim() != "") | (_Modul1.Instance.Kont[18].Trim() != "") | (_Modul1.Instance.Kont[23].Trim() != ""))
                        {
                            goto IL_0bb2;
                        }
                        goto IL_0e84;
                    IL_0bb2:
                        num = 140;
                        if (_Modul1.Instance.DAus[106].AsDouble() == 1.0)
                        {
                            goto IL_0bd0;
                        }
                        goto IL_0bf8;
                    IL_0bd0:
                        num = 141;
                        if (umb == 0)
                        {
                            goto IL_0bda;
                        }
                        goto IL_0c1a;
                    IL_0bda:
                        num = 142;
                        Anz[0].SelectedText = "\n";
                        goto IL_0c1a;
                    IL_0bf8:
                        num = 145;
                        goto IL_0bfe;
                    IL_0bfe:
                        num = 146;
                        Anz[0].SelectedText = " ";
                        goto IL_0c1a;
                    IL_0c1a:
                        num = 148;
                        Anz[0].SelectedText = _Modul1.Instance.DTxt[3];
                        goto IL_0c38;
                    IL_0c38:
                        num = 149;
                        if (_Modul1.Instance.Kont[13].Trim() != "")
                        {
                            goto IL_0c59;
                        }
                        goto IL_0c8c;
                    IL_0c59:
                        num = 150;
                        Anz[0].SelectedText = " " + _Modul1.Instance.Kont[13].Trim() + ".";
                        goto IL_0c8c;
                    IL_0c8c:
                        num = 152;
                        if (_Modul1.Instance.Kont[33].Trim() != "")
                        {
                            goto IL_0cb0;
                        }
                        goto IL_0d77;
                    IL_0cb0:
                        num = 153;
                        Anz[0].SelectionCharOffset = _Modul1.Instance.PrintDat.Hoch;
                        goto IL_0ccc;
                    IL_0ccc:
                        num = 154;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                        goto IL_0d09;
                    IL_0d09:
                        num = 155;
                        Anz[0].SelectedText = _Modul1.Instance.Kont[33].Trim();
                        goto IL_0d2d;
                    IL_0d2d:
                        num = 156;
                        Anz[0].SelectionCharOffset = 0;
                        goto IL_0d45;
                    IL_0d45:
                        num = 157;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_0d77;
                    IL_0d77:
                        num = 159;
                        if (_Modul1.Instance.DAus[85].AsDouble() == 1.0)
                        {
                            goto IL_0d95;
                        }
                        goto IL_0de4;
                    IL_0d95:
                        num = 160;
                        if (_Modul1.Instance.Kont[43].Trim() != "")
                        {
                            goto IL_0db6;
                        }
                        goto IL_0de4;
                    IL_0db6:
                        num = 161;
                        Anz[0].SelectedText = " Urkunde: " + _Modul1.Instance.Kont[43].Trim();
                        goto IL_0de4;
                    IL_0de4:
                        num = 164;
                        if (_Modul1.Instance.DAus[2] == "1")
                        {
                            goto IL_0dff;
                        }
                        goto IL_0e34;
                    IL_0dff:
                        num = 165;
                        if (_Modul1.Instance.Kont[18].Length > 0)
                        {
                            goto IL_0e15;
                        }
                        goto IL_0e34;
                    IL_0e15:
                        num = 166;
                        _Modul1.Instance.UbgT1 = _Modul1.Instance.Kont[18];
                        goto IL_0e28;
                    IL_0e28:
                        num = 167;
                        Bemaus();
                        goto IL_0e34;
                    IL_0e34:
                        num = 170;
                        if (_Modul1.Instance.DAus[3] == "1")
                        {
                            goto IL_0e4f;
                        }
                        goto IL_0e84;
                    IL_0e4f:
                        num = 171;
                        if (_Modul1.Instance.Kont[23].Length > 0)
                        {
                            goto IL_0e65;
                        }
                        goto IL_0e84;
                    IL_0e65:
                        num = 172;
                        _Modul1.Instance.UbgT1 = _Modul1.Instance.Kont[23];
                        goto IL_0e78;
                    IL_0e78:
                        num = 173;
                        Bemaus();
                        goto IL_0e84;
                    IL_0e84:
                        num = 177;
                        if (_Modul1.Instance.DAus[96].AsBool())
                        {
                            goto IL_0e9c;
                        }
                        goto IL_108f;
                    IL_0e9c:
                        num = 178;
                        if (_Modul1.Instance.Kont[53].Trim() != "")
                        {
                            goto IL_0ec0;
                        }
                        goto IL_108f;
                    IL_0ec0:
                        num = 179;
                        leerweg();
                        goto IL_0ecc;
                    IL_0ecc:
                        num = 180;
                        Anz[0].SelectedText = " ";
                        goto IL_0ee8;
                    IL_0ee8:
                        num = 181;
                        if (_Modul1.Instance.DAus[72] == "1")
                        {
                            goto IL_0f04;
                        }
                        goto IL_0f43;
                    IL_0f04:
                        num = 182;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
                        goto IL_0f7b;
                    IL_0f43:
                        num = 184;
                        goto IL_0f49;
                    IL_0f49:
                        num = 185;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Underline);
                        goto IL_0f7b;
                    IL_0f7b:
                        num = 187;
                        Anz[0].SelectedText = "Zeugen:";
                        goto IL_0f97;
                    IL_0f97:
                        num = 188;
                        if (_Modul1.Instance.DAus[72] == "1")
                        {
                            goto IL_0fb3;
                        }
                        goto IL_0ff2;
                    IL_0fb3:
                        num = 189;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                        goto IL_102a;
                    IL_0ff2:
                        num = 191;
                        goto IL_0ff8;
                    IL_0ff8:
                        num = 192;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_102a;
                    IL_102a:
                        num = 194;
                        Anz[0].SelectedText = " " + _Modul1.Instance.Kont[53].Trim() + ".";
                        goto IL_105d;
                    IL_105d:
                        num = 195;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_108f;
                    IL_108f:
                        num = 198;
                        if ((_Modul1.Instance.Kont[14].Trim() != "") | (_Modul1.Instance.Kont[19].Trim() != "") | (_Modul1.Instance.Kont[24].Trim() != ""))
                        {
                            goto IL_10f6;
                        }
                        goto IL_13ba;
                    IL_10f6:
                        num = 199;
                        if (_Modul1.Instance.DAus[106].AsDouble() == 1.0)
                        {
                            goto IL_1114;
                        }
                        goto IL_113c;
                    IL_1114:
                        num = 200;
                        if (umb == 0)
                        {
                            goto IL_111e;
                        }
                        goto IL_115e;
                    IL_111e:
                        num = 201;
                        Anz[0].SelectedText = "\n";
                        goto IL_115e;
                    IL_113c:
                        num = 204;
                        goto IL_1142;
                    IL_1142:
                        num = 205;
                        Anz[0].SelectedText = " ";
                        goto IL_115e;
                    IL_115e:
                        num = 207;
                        Anz[0].SelectedText = _Modul1.Instance.DTxt[4];
                        goto IL_117c;
                    IL_117c:
                        num = 208;
                        if (_Modul1.Instance.Kont[14].Trim() != "")
                        {
                            goto IL_119d;
                        }
                        goto IL_11d0;
                    IL_119d:
                        num = 209;
                        Anz[0].SelectedText = " " + _Modul1.Instance.Kont[14].Trim() + ".";
                        goto IL_11d0;
                    IL_11d0:
                        num = 211;
                        if (_Modul1.Instance.Kont[34].Trim() != "")
                        {
                            goto IL_11f4;
                        }
                        goto IL_12ad;
                    IL_11f4:
                        num = 212;
                        Anz[0].SelectionCharOffset = _Modul1.Instance.PrintDat.Hoch;
                        goto IL_1210;
                    IL_1210:
                        num = 213;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                        goto IL_124d;
                    IL_124d:
                        num = 214;
                        Anz[0].SelectedText = _Modul1.Instance.Kont[34].Trim() + " ";
                        goto IL_127b;
                    IL_127b:
                        num = 215;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_12ad;
                    IL_12ad:
                        num = 217;
                        if (_Modul1.Instance.DAus[85].AsDouble() == 1.0)
                        {
                            goto IL_12cb;
                        }
                        goto IL_131a;
                    IL_12cb:
                        num = 218;
                        if (_Modul1.Instance.Kont[44].Trim() != "")
                        {
                            goto IL_12ec;
                        }
                        goto IL_131a;
                    IL_12ec:
                        num = 219;
                        Anz[0].SelectedText = " Urkunde: " + _Modul1.Instance.Kont[44].Trim();
                        goto IL_131a;
                    IL_131a:
                        num = 222;
                        if (_Modul1.Instance.DAus[2] == "1")
                        {
                            goto IL_1335;
                        }
                        goto IL_136a;
                    IL_1335:
                        num = 223;
                        if (_Modul1.Instance.Kont[19].Length > 0)
                        {
                            goto IL_134b;
                        }
                        goto IL_136a;
                    IL_134b:
                        num = 224;
                        _Modul1.Instance.UbgT1 = _Modul1.Instance.Kont[19];
                        goto IL_135e;
                    IL_135e:
                        num = 225;
                        Bemaus();
                        goto IL_136a;
                    IL_136a:
                        num = 228;
                        if (_Modul1.Instance.DAus[3] == "1")
                        {
                            goto IL_1385;
                        }
                        goto IL_13ba;
                    IL_1385:
                        num = 229;
                        if (_Modul1.Instance.Kont[24].Length > 0)
                        {
                            goto IL_139b;
                        }
                        goto IL_13ba;
                    IL_139b:
                        num = 230;
                        _Modul1.Instance.UbgT1 = _Modul1.Instance.Kont[24];
                        goto IL_13ae;
                    IL_13ae:
                        num = 231;
                        Bemaus();
                        goto IL_13ba;
                    IL_13ba:
                        num = 235;
                        if (_Modul1.Instance.DAus[96].AsBool())
                        {
                            goto IL_13d2;
                        }
                        goto IL_15c5;
                    IL_13d2:
                        num = 236;
                        if (_Modul1.Instance.Kont[54].Trim() != "")
                        {
                            goto IL_13f6;
                        }
                        goto IL_15c5;
                    IL_13f6:
                        num = 237;
                        leerweg();
                        goto IL_1402;
                    IL_1402:
                        num = 238;
                        Anz[0].SelectedText = " ";
                        goto IL_141e;
                    IL_141e:
                        num = 239;
                        if (_Modul1.Instance.DAus[72] == "1")
                        {
                            goto IL_143a;
                        }
                        goto IL_1479;
                    IL_143a:
                        num = 240;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
                        goto IL_14b1;
                    IL_1479:
                        num = 242;
                        goto IL_147f;
                    IL_147f:
                        num = 243;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Underline);
                        goto IL_14b1;
                    IL_14b1:
                        num = 245;
                        Anz[0].SelectedText = "Zeugen:";
                        goto IL_14cd;
                    IL_14cd:
                        num = 246;
                        if (_Modul1.Instance.DAus[72] == "1")
                        {
                            goto IL_14e9;
                        }
                        goto IL_1528;
                    IL_14e9:
                        num = 247;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                        goto IL_1560;
                    IL_1528:
                        num = 249;
                        goto IL_152e;
                    IL_152e:
                        num = 250;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_1560;
                    end_IL_0000:
                        break;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 6767;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    public float M_Sgg1 => 1f;

    public void Berufe(EEventArt Beruf, float M_Sgg = 1f)
    {
        int try0000_dispatch = -1;
        int num = default;
        string left = default;
        int num2 = default;
        int num3 = default;
        string Job = default;
        int lErl = default;
        short num5 = default;
        short num6 = default;
        string text2 = default;
        string value = default;
        int persInArb = default;
        string text3 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    string text;
                    int ortNr;
                    int ortNr2;
                    byte Schalt;
                    int Nr;
                    short LfNR;
                    short Listart;
                    int AAA;
                    string LD;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            left = "";
                            goto IL_0009;
                        case 8125:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_1b37;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_1b3b;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            num = 2;
                            List3.Items.Clear();
                            _Modul1.Instance.LfNR = 0;
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            DataModul.DB_EventTable.Index = "Besu";
                            DataModul.DB_EventTable.Seek("=", Beruf.AsString(), _Modul1.Instance.PersInArb.AsString());
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                DataModul.DB_EventTable.Index = "ArtNr";
                                goto end_IL_0000_2;
                            }
                            goto IL_04f0;
                        IL_033f: // <========== 3
                            num = 36;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value) && DataModul.DB_EventTable.Fields[EventFields.ArtText].Value.AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                                LD = "";
                                DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                                if (_Modul1.Instance.Kont[0] != "")
                                {
                                    _Modul1.Instance.Kont[10] = " " + _Modul1.Instance.Kont[0].Trim() + ": ";
                                }

                            }
                            goto IL_0429;
                        IL_0429: // <========== 3
                            num = 44;
                            Job = _Modul1.Instance.Kont1[1] + _Modul1.Instance.Kont1[3] + _Modul1.Instance.Kont1[7] + new string(' ', 240).Left(240) + DataModul.DB_EventTable.Fields[EventFields.LfNr].AsString();
                            if (DataModul.DB_EventTable.Fields[EventFields.Reg].AsString() != " ")
                            {
                                Job = "+" + Job;
                            }
                            goto IL_04c3;
                        IL_04c3:
                            num = 48;
                            List3.Items.Add(Job);
                            goto IL_04da;
                        IL_04da: // <========== 3
                            num = 49;
                            lErl = 12;
                            DataModul.DB_EventTable.MoveNext();
                            goto IL_04f0;
                        IL_04f0: // <========== 3
                            num = 12;
                            if (!DataModul.DB_EventTable.EOF)
                            {
                                if (!Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                                {
                                    M1_J = 0;
                                    while (unchecked(M1_J) <= 15u)
                                    {
                                        _Modul1.Instance.Kont1[M1_J] = "";
                                        M1_J = (byte)unchecked((uint)(M1_J + 1));
                                    }
                                    _Modul1.Instance.sDatu = "";
                                    if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch
                                      | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb)
                                      | (DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != Beruf)))
                                    {
                                        DataModul.DB_EventTable.Index = "ArtNr";
                                        goto IL_0503;
                                    }
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                    {
                                        _Modul1.Instance.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                                        _Modul1.Instance.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                        _Modul1.Instance.Kont1[1] = _Modul1.Instance.sDatu;
                                    }
                                    _Modul1.Instance.UbgT = "";
                                    if (DataModul.DB_EventTable.Fields[EventFields.KBem].Value.AsDouble() > 0.0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                        LD = "";
                                        DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                                        if (_Modul1.Instance.Kont[0] != "")
                                        {
                                            _Modul1.Instance.Kont1[7] = " " + _Modul1.Instance.Kont[0].Trim() + " ";

                                        }
                                    }
                                    else
                                    {
                                    }
                                    goto IL_033f;
                                }
                                goto IL_04da;
                            }
                            goto IL_0503;
                        IL_0503: // <========== 3
                            num = 52;
                            lErl = 13;
                            leerweg();
                            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * M_Sgg), FontStyle.Regular);
                            if (List3.Items.Count == 0)
                            {
                                goto end_IL_0000_2;
                            }
                            if (_Modul1.Instance.DAus[106].AsDouble() == 1.0)
                            {
                                Anz[0].SelectedText = "\n";
                                Anz[0].SelectionIndent = Abst;

                            }
                            else
                            {
                                Anz[0].SelectedText = " ";
                            }
                            goto IL_05da;
                        IL_05da: // <========== 3
                            num = 65;
                            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * M_Sgg), FontStyle.Underline);
                            switch (Beruf)
                            {
                                case EEventArt.eA_300:
                                    goto IL_0634;
                                case EEventArt.eA_301:
                                    goto IL_0698;
                                case EEventArt.eA_302:
                                    goto IL_06bc;
                                default:
                                    break;
                            }
                            goto IL_06dd;
                        IL_0634:
                            num = 70;
                            if (List3.Items.Count == 1)
                            {
                                Anz[0].SelectedText = "Beruf:";
                            }
                            goto IL_0665;
                        IL_0665:
                            num = 73;
                            if (List3.Items.Count > 1)
                            {
                                Anz[0].SelectedText = "Berufe:";
                            }
                            goto IL_06dd;
                        IL_0698:
                            num = 78;
                            Anz[0].SelectedText = _Modul1.Instance.IText[70].Trim();
                            goto IL_06dd;
                        IL_06bc:
                            num = 81;
                            Anz[0].SelectedText = _Modul1.Instance.IText[8].Trim();
                            goto IL_06dd;
                        IL_06dd: // <========== 5
                            num = 83;
                            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * M_Sgg), FontStyle.Regular);
                            Anz[0].SelectedText = " ";
                            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * M_Sgg), FontStyle.Regular);
                            num5 = (short)(List3.Items.Count - 1);
                            num6 = 0;
                            goto IL_1a30;
                        IL_0a68: // <========== 3
                            num = 117;
                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            M_Namen = _Modul1.Instance.Kont[0];
                            if (_Modul1.Instance.Kont[99] == "")
                            {
                                text = "?";

                            }
                            else
                            {
                                text = _Modul1.Instance.Kont[99].Trim();
                            }
                            goto IL_0abf;
                        IL_0abf: // <========== 3
                            num = 125;
                            _Modul1.Instance.Kont1[1] = "";
                            _Modul1.Instance.Kont1[3] = "";
                            if (left == "1")
                            {
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    _Modul1.Instance.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                                    text2 = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                    _Modul1.Instance.Datwand1(ref _Modul1.Instance.sDatu, text2);
                                    _Modul1.Instance.Kont1[1] = _Modul1.Instance.sDatu;
                                }
                                goto IL_0bb7;
                            }
                            goto IL_0f83;
                        IL_0bb7:
                            num = 134;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                if (_Modul1.Instance.Kont1[1] != "")
                                {
                                    _Modul1.Instance.Kont1[1] = "von " + _Modul1.Instance.Kont1[1];
                                }
                                _Modul1.Instance.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                _Modul1.Instance.sDatu = "00000000" + _Modul1.Instance.sDatu.Right(8);
                                text2 = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                _Modul1.Instance.Datwand1(ref _Modul1.Instance.sDatu, text2);
                                if ((_Modul1.Instance.sDatu != "") & (text2.Trim() == ""))
                                {
                                    _Modul1.Instance.sDatu = " bis " + _Modul1.Instance.sDatu.Trim();
                                }
                                goto IL_0cfa;
                            }
                            goto IL_0d0d;
                        IL_0cfa:
                            num = 145;
                            _Modul1.Instance.Kont1[3] = _Modul1.Instance.sDatu;
                            goto IL_0d0d;
                        IL_0d0d: // <========== 3
                            num = 147;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.DatumText].Value))
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt() > 0)
                                {
                                    value = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsString();
                                    AAA = value.AsInt();
                                    LD = "";
                                    DataModul.Textlese(AAA, out _Modul1.Instance.UbgT, out LD);
                                    value = AAA.AsString();
                                    if (_Modul1.Instance.UbgT.Trim() != "")
                                    {
                                        _Modul1.Instance.Kont1[3] = _Modul1.Instance.Kont1[3] + " (" + _Modul1.Instance.UbgT.Trim() + ")";
                                        _Modul1.Instance.UbgT = "";
                                    }

                                }
                            }
                            else
                            {
                            }
                            goto IL_0e1d;
                        IL_0e1d: // <========== 3
                            num = 157;
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                            {
                                ortNr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble());
                                _Modul1.Instance.UbgT = _Modul1.Instance.ortles1(ortNr, 1, (i, s) => _Modul1.Instance.ExportPlace(i, s, _Modul1.Instance.Ind1, M_Namen));
                                _Modul1.Instance.Kont1[5] = " " + _Modul1.Instance.UbgT.Trim();
                                _Modul1.Instance.UbgT = "";
                            }
                            goto IL_0ebf;
                        IL_0ebf:
                            num = 162;
                            if (DataModul.DB_EventTable.Fields[EventFields.Platz].Value.AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                LD = "";
                                DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                                if (_Modul1.Instance.Kont[0] != "")
                                {
                                    _Modul1.Instance.Kont1[6] = " " + _Modul1.Instance.Kont[0].Trim();

                                }
                            }
                            else
                            {
                            }
                            goto IL_0f83;
                        IL_0f83: // <========== 4
                            num = 169;
                            _Modul1.Instance.UbgT = "";
                            if (Beruf == EEventArt.eA_302)
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                                {
                                    ortNr2 = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble());
                                    _Modul1.Instance.UbgT = _Modul1.Instance.ortles1(ortNr2, 1, (i, s) => _Modul1.Instance.ExportPlace(i, s, _Modul1.Instance.Ind1, M_Namen));
                                    _Modul1.Instance.Kont1[5] = " " + _Modul1.Instance.UbgT.Trim();
                                    _Modul1.Instance.UbgT = "";
                                }
                                goto IL_1049;
                            }
                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].Value.AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                LD = "";
                                DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                                if (_Modul1.Instance.Kont[0] != "")
                                {
                                    _Modul1.Instance.Kont1[7] = " " + _Modul1.Instance.Kont[0].Trim();

                                }
                            }
                            else
                            {
                            }
                            goto IL_11f6;
                        IL_1049:
                            num = 176;
                            if (left == "1")
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.KBem].Value.AsDouble() > 0.0)
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                    LD = "";
                                    DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                                    if (_Modul1.Instance.Kont[0] != "")
                                    {
                                        _Modul1.Instance.Kont1[7] = " " + _Modul1.Instance.Kont[0].Trim();
                                    }

                                }
                            }
                            else
                            {
                            }
                            goto IL_11f6;
                        IL_11f6: // <========== 5
                            num = 193;
                            if (Beruf == EEventArt.eA_302)
                            {
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Hausnr].Value))
                                {
                                    if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Hausnr].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.Hausnr].AsInt();
                                        LD = "";
                                        DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                                        _Modul1.Instance.Kont1[7] = _Modul1.Instance.Kont1[7] + " " + _Modul1.Instance.Kont[0].Trim() + " ";
                                        _Modul1.Instance.Kont[0] = "";
                                    }

                                }
                            }
                            else
                            {
                            }
                            goto IL_1302;
                        IL_1302: // <========== 3
                            num = 202;
                            left = "0";
                            Job = (_Modul1.Instance.Kont1[1] + _Modul1.Instance.Kont1[3] + _Modul1.Instance.Kont1[7] + _Modul1.Instance.Kont1[6] + _Modul1.Instance.Kont1[5]).Trim();
                            Job = Module2.Jobdreh(Job);
                            if (Job.Trim() != "")
                            {
                                if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == ";")
                                {
                                    Anz[0].SelectedText = " ";
                                }
                                goto IL_13e7;
                            }
                            goto IL_1406;
                        IL_13e7:
                            num = 209;
                            Anz[0].SelectedText = Job.TrimEnd();
                            goto IL_1406;
                        IL_1406: // <========== 3
                            num = 211;
                            Nr = _Modul1.Instance.PersInArb;
                            LfNR = _Modul1.Instance.LfNR;
                            _Modul1.Instance.QuellenDatum(ref Nr, Beruf, ref LfNR);
                            _Modul1.Instance.LfNR = Conversions.ToByte(LfNR);
                            _Modul1.Instance.PersInArb = Nr.AsInt();
                            leerweg();
                            if (_Modul1.Instance.Kont1[9].Trim() != "")
                            {
                                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                Anz[0].SelectionCharOffset = _Modul1.Instance.PrintDat.Hoch;
                                Anz[0].SelectedText = " " + _Modul1.Instance.Kont1[9];
                                Anz[0].SelectionCharOffset = 0;
                                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            }
                            left = "0";
                            if (unchecked(0 - (M1_Ki ? 1 : 0)) == 1)
                            {
                                if (((Beruf == EEventArt.eA_300) & (_Modul1.Instance.DAus[26] == "1")) | ((Beruf == EEventArt.eA_301) & (_Modul1.Instance.DAus[30] == "1")) | ((Beruf == EEventArt.eA_302) & (_Modul1.Instance.DAus[34] == "1")))
                                {
                                    left = "1";
                                }
                                goto IL_1663;
                            }
                            if (((Beruf == EEventArt.eA_300) & (_Modul1.Instance.DAus[14] == "1")) | ((Beruf == EEventArt.eA_301) & (_Modul1.Instance.DAus[20] == "1")) | ((Beruf == EEventArt.eA_302) & (_Modul1.Instance.DAus[22] == "1")))
                            {
                                left = "1";
                            }
                            goto IL_1663;
                        IL_1663: // <========== 3
                            num = 231;
                            if (left == "1")
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString() != " ")
                                {
                                    _Modul1.Instance.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim();
                                    Bemaus();

                                }
                            }
                            else
                            {
                            }
                            goto IL_16e2;
                        IL_16e2: // <========== 3
                            num = 237;
                            left = "0";
                            if (unchecked(0 - (M1_Ki ? 1 : 0)) == 1)
                            {
                                if (((Beruf == EEventArt.eA_300) & (_Modul1.Instance.DAus[27] == "1")) | ((Beruf == EEventArt.eA_301) & (_Modul1.Instance.DAus[31] == "1")) | ((Beruf == EEventArt.eA_302) & (_Modul1.Instance.DAus[35] == "1")))
                                {
                                    left = "1";
                                }
                                goto IL_17fb;
                            }
                            if (((Beruf == EEventArt.eA_300) & (_Modul1.Instance.DAus[15] == "1")) | ((Beruf == EEventArt.eA_301) & (_Modul1.Instance.DAus[19] == "1")) | ((Beruf == EEventArt.eA_302) & (_Modul1.Instance.DAus[23] == "1")))
                            {
                                left = "1";
                            }
                            goto IL_17fb;
                        IL_17fb: // <========== 3
                            num = 248;
                            if (left == "1")
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString() != " ")
                                {
                                    _Modul1.Instance.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim();
                                    Bemaus();

                                }
                            }
                            else
                            {
                            }
                            goto IL_187a;
                        IL_187a: // <========== 3
                            num = 254;
                            persInArb = _Modul1.Instance.PersInArb;
                            LD = "";
                            _Modul1.Instance.Zeugsu(Beruf, _Modul1.Instance.LfNR, 1, 0L);
                            text3 = _Modul1.Instance.Kont1[20];
                            _Modul1.Instance.Kont1[20] = "";
                            _Modul1.Instance.PersInArb = persInArb;
                            if (_Modul1.Instance.DAus[96].AsDouble() == 1.0)
                            {
                                if (text3 != "")
                                {
                                    leerweg();
                                    if (_Modul1.Instance.DAus[100] == "1")
                                    {
                                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                    }
                                    Anz[0].SelectedText = " Zeugen: " + text3.Trim();
                                    text3 = "";
                                    Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);

                                }
                            }
                            else
                            {
                            }
                            goto IL_19e9;
                        IL_19e9: // <========== 3
                            num = 270;
                            left = "0";
                            Anz[0].SelectedText = ";";
                            DataModul.DB_EventTable.MoveNext();
                            num6 = (short)unchecked(num6 + 1);
                            goto IL_1a30;
                        IL_1a30:
                            if (num6 <= num5)
                            {
                                _Modul1.Instance.LfNR = (byte)Math.Round(Conversion.Val(Strings.Mid(List3.Items[num6].AsString(), 240, 10)));
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", Beruf.AsString(), _Modul1.Instance.PersInArb.AsString(), _Modul1.Instance.LfNR);
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    Debugger.Break();
                                    goto end_IL_0000_2;
                                }
                                if (Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                                {
                                    Interaction.MsgBox("7");
                                    Debugger.Break();
                                }
                                M1_J = 0;
                                while (unchecked(M1_J) <= 15u)
                                {
                                    _Modul1.Instance.Kont1[M1_J] = "";
                                    M1_J = (byte)unchecked((uint)(M1_J + 1));
                                }
                                _Modul1.Instance.Ubg = num6;
                                _Modul1.Instance.sDatu = "";
                                if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch
                                      | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb)
                                      | (DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != Beruf)))
                                {
                                    DataModul.DB_EventTable.Index = "ArtNr";
                                    goto end_IL_0000_2;
                                }
                                if (unchecked(0 - (M1_Ki ? 1 : 0)) == 1)
                                {
                                    if (((Beruf == EEventArt.eA_300) & (_Modul1.Instance.DAus[25] == "1")) | ((Beruf == EEventArt.eA_301) & (_Modul1.Instance.DAus[29] == "1")) | ((Beruf == EEventArt.eA_302) & (_Modul1.Instance.DAus[33] == "1")))
                                    {
                                        left = "1";
                                    }
                                    goto IL_0a68;
                                }
                                if (((Beruf == EEventArt.eA_300) & (_Modul1.Instance.DAus[13] == "1")) | ((Beruf == EEventArt.eA_301) & (_Modul1.Instance.DAus[17] == "1")) | ((Beruf == EEventArt.eA_302) & (_Modul1.Instance.DAus[21] == "1")))
                                {
                                    left = "1";
                                }
                                goto IL_0a68;
                            }
                            if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == ";")
                            {
                                Anz[0].SelectionStart = Anz[0].SelectionStart - 1;
                                Anz[0].SelectionLength = 1;
                                Anz[0].SelectedText = ".";
                            }
                            goto IL_1ad6;
                        IL_1ad6:
                            num = 279;
                            left = "0";
                            goto end_IL_0000_2;
                        IL_1b37:
                            num4 = unchecked(num2 + 1);
                            goto IL_1b3b;
                        IL_1b3b:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 34:
                                case 35:
                                case 36:
                                    goto IL_033f;
                                case 41:
                                case 42:
                                case 43:
                                case 44:
                                    goto IL_0429;
                                case 47:
                                case 48:
                                    goto IL_04c3;
                                case 14:
                                case 49:
                                    goto IL_04da;
                                case 10:
                                case 11:
                                case 12:
                                case 51:
                                    goto IL_04f0;
                                case 22:
                                case 52:
                                    goto IL_0503;
                                case 61:
                                case 64:
                                case 65:
                                    goto IL_05da;
                                case 72:
                                case 73:
                                    goto IL_0665;
                                case 67:
                                case 75:
                                case 76:
                                case 79:
                                case 82:
                                case 83:
                                    goto IL_06dd;
                                case 110:
                                case 111:
                                case 115:
                                case 116:
                                case 117:
                                    goto IL_0a68;
                                case 121:
                                case 124:
                                case 125:
                                    goto IL_0abf;
                                case 133:
                                case 134:
                                    goto IL_0bb7;
                                case 144:
                                case 145:
                                    goto IL_0cfa;
                                case 146:
                                case 147:
                                    goto IL_0d0d;
                                case 154:
                                case 155:
                                case 156:
                                case 157:
                                    goto IL_0e1d;
                                case 161:
                                case 162:
                                    goto IL_0ebf;
                                case 166:
                                case 167:
                                case 168:
                                case 169:
                                    goto IL_0f83;
                                case 175:
                                case 176:
                                    goto IL_1049;
                                case 181:
                                case 182:
                                case 183:
                                case 184:
                                case 190:
                                case 191:
                                case 192:
                                case 193:
                                    goto IL_11f6;
                                case 199:
                                case 200:
                                case 201:
                                case 202:
                                    goto IL_1302;
                                case 208:
                                case 209:
                                    goto IL_13e7;
                                case 210:
                                case 211:
                                    goto IL_1406;
                                case 224:
                                case 225:
                                case 229:
                                case 230:
                                case 231:
                                    goto IL_1663;
                                case 235:
                                case 236:
                                case 237:
                                    goto IL_16e2;
                                case 241:
                                case 242:
                                case 246:
                                case 247:
                                case 248:
                                    goto IL_17fb;
                                case 252:
                                case 253:
                                case 254:
                                    goto IL_187a;
                                case 268:
                                case 269:
                                case 270:
                                    goto IL_19e9;
                                case 278:
                                case 279:
                                    goto IL_1ad6;
                                case 9:
                                case 56:
                                case 92:
                                case 105:
                                case 280:
                                case 285:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 8125;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 6
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void Namenindex()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string text = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                short Listart;
                string Ahne;
                bool neb;
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0008;
                    case 1013:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_031f;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_02a7;
                        }
                    IL_02a7:
                        num = 41;
                        if (Information.Err().Number == 3022)
                        {
                            goto IL_02bc;
                        }
                        goto IL_02d6;
                    IL_02d6:
                        num = 45;
                        if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                        {
                            ProjectData.EndApp();
                        }
                        goto IL_02fc;
                    IL_0267:
                        num = 35;
                        goto IL_026b;
                    IL_02fc:
                        num = 48;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        num4 = num2;
                        goto IL_0323;
                    IL_026b:
                        num = 36;
                        DataModul.NB_DgbTable.Fields["Ind"].Value = _Modul1.Instance.Ind1.Trim();
                        goto IL_0292;
                    IL_0292:
                        num = 38;
                        DataModul.NB_DgbTable.Update();
                        goto end_IL_0000_2;
                    IL_0323:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_0008;
                            case 3:
                                goto IL_0020;
                            case 4:
                            case 5:
                                goto IL_0029;
                            case 6:
                                goto IL_0038;
                            case 7:
                                goto IL_0056;
                            case 9:
                                goto IL_0061;
                            case 10:
                                goto IL_0065;
                            case 8:
                            case 11:
                            case 12:
                                goto IL_0077;
                            case 13:
                                goto IL_0081;
                            case 14:
                                goto IL_00a0;
                            case 15:
                                goto IL_00aa;
                            case 16:
                                goto IL_00c3;
                            case 17:
                            case 18:
                                goto IL_00d5;
                            case 19:
                                goto IL_00ee;
                            case 20:
                            case 21:
                                goto IL_00fe;
                            case 22:
                                goto IL_011d;
                            case 23:
                                goto IL_012b;
                            case 24:
                                goto IL_0145;
                            case 25:
                            case 26:
                                goto IL_0163;
                            case 27:
                                goto IL_018a;
                            case 28:
                                goto IL_0198;
                            case 29:
                            case 30:
                                goto IL_01a6;
                            case 31:
                                goto IL_01c9;
                            case 32:
                                goto IL_01f3;
                            case 33:
                                goto IL_0215;
                            case 35:
                                goto IL_0267;
                            case 36:
                                goto IL_026b;
                            case 34:
                            case 37:
                            case 38:
                                goto IL_0292;
                            case 41:
                                goto IL_02a7;
                            case 42:
                                goto IL_02bc;
                            case 43:
                            case 45:
                                goto IL_02d6;
                            case 46:
                            case 48:
                                goto IL_02fc;
                            default:
                                goto end_IL_0000;
                            case 39:
                            case 40:
                            case 49:
                                goto end_IL_0000_2;
                        }
                        goto default;
                    IL_02bc:
                        num = 42;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_031f;
                    IL_0215:
                        num = 33;
                        DataModul.NB_DgbTable.Fields["Ind"].Value = Strings.Mid(_Modul1.Instance.Ind1.Trim(), 1, Strings.Len(Strings.Trim((_Modul1.Instance.Ind1.AsDouble() - 2.0).AsString())));
                        goto IL_0292;
                    IL_031f:
                        num4 = num2 + 1;
                        goto IL_0323;
                    IL_0008:
                        num = 2;
                        if (_Modul1.Instance.Kont[4] != "")
                        {
                            goto IL_0020;
                        }
                        goto IL_0029;
                    IL_0020:
                        num = 3;
                        Alias_Renamed();
                        goto IL_0029;
                    IL_0029:
                        num = 5;
                        M_Namen = _Modul1.Instance.Kont[0];
                        goto IL_0038;
                    IL_0038:
                        num = 6;
                        if (_Modul1.Instance.Kont[99].Trim() == "")
                        {
                            goto IL_0056;
                        }
                        goto IL_0061;
                    IL_0056:
                        num = 7;
                        text = "?";
                        goto IL_0077;
                    IL_0061:
                        num = 9;
                        goto IL_0065;
                    IL_0065:
                        num = 10;
                        text = _Modul1.Instance.Kont[99].Trim();
                        goto IL_0077;
                    IL_0077:
                        num = 12;
                        _Modul1.Instance.Datschalt = 1;
                        goto IL_0081;
                    IL_0081:
                        num = 13;
                        Listart = 1;
                        Ahne = 0.AsString();
                        neb = false;
                        _Modul1.Instance.Datles3(Listart, default, default, ref neb);
                        goto IL_00a0;
                    IL_00a0:
                        num = 14;
                        _Modul1.Instance.Datschalt = 0;
                        goto IL_00aa;
                    IL_00aa:
                        num = 15;
                        if (_Modul1.Instance.Kont[1] == "")
                        {
                            goto IL_00c3;
                        }
                        goto IL_00d5;
                    IL_00c3:
                        num = 16;
                        _Modul1.Instance.Kont[1] = _Modul1.Instance.Kont[2];
                        goto IL_00d5;
                    IL_00d5:
                        num = 18;
                        if (_Modul1.Instance.Kont[1] == "")
                        {
                            goto IL_00ee;
                        }
                        goto IL_00fe;
                    IL_00ee:
                        num = 19;
                        _Modul1.Instance.Kont[1] = "    ";
                        goto IL_00fe;
                    IL_00fe:
                        num = 21;
                        if (M_Namen.Trim() == "")
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_011d;
                    IL_011d:
                        num = 22;
                        DataModul.NB_DgbTable.AddNew();
                        goto IL_012b;
                    IL_012b:
                        num = 23;
                        if (M_Namen.Trim().Length > 150)
                        {
                            goto IL_0145;
                        }
                        goto IL_0163;
                    IL_0145:
                        num = 24;
                        M_Namen = M_Namen.Trim().Left(149);
                        goto IL_0163;
                    IL_0163:
                        num = 26;
                        DataModul.NB_DgbTable.Fields["Name"].Value = M_Namen.Trim();
                        goto IL_018a;
                    IL_018a:
                        num = 27;
                        if (text.Length > 23)
                        {
                            goto IL_0198;
                        }
                        goto IL_01a6;
                    IL_0198:
                        num = 28;
                        text = text.Left(23);
                        goto IL_01a6;
                    IL_01a6:
                        num = 30;
                        DataModul.NB_DgbTable.Fields["Vornam"].Value = text.Trim();
                        goto IL_01c9;
                    IL_01c9:
                        num = 31;
                        DataModul.NB_DgbTable.Fields["Geb"].Value = _Modul1.Instance.Kont[1].Left(4);
                        goto IL_01f3;
                    IL_01f3:
                        num = 32;
                        if (Operators.CompareString(_Modul1.Instance.Ind1.Trim().Right(2), ".0", TextCompare: false) == 0)
                        {
                            goto IL_0215;
                        }
                        goto IL_0267;
                    end_IL_0000:
                        break;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 1013;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    public void Alias_Renamed()
    {
        checked
        {
            short num;
            do
            {
                num = (short)Strings.InStr(_Modul1.Instance.Kont[4], ";");
                string text;
                if (num > 0)
                {
                    text = _Modul1.Instance.Kont[4].Left(num - 1);
                    _Modul1.Instance.Kont[4] = Strings.Mid(_Modul1.Instance.Kont[4], num + 1, _Modul1.Instance.Kont[4].Length);
                }
                else
                {
                    text = _Modul1.Instance.Kont[4];
                }
                if (text.Length > 50)
                {
                    text = Strings.Trim(text.Left(50));
                }
                DataModul.NB_DgbTable.AddNew();
                DataModul.NB_DgbTable.Fields["Name"].Value = text.Trim() + " siehe " + _Modul1.Instance.Kont[0].Trim();
                DataModul.NB_DgbTable.Fields["Vornam"].Value = " ";
                DataModul.NB_DgbTable.Fields["Geb"].Value = "";
                DataModul.NB_DgbTable.Fields["Ind"].Value = "";
                DataModul.NB_DgbTable.Update();
            }
            while (num != 0);
        }
    }

    public void EPerles()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int famInArb = default;
        int famInArb2 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                int FamPer;
                string LD;
                short Listart;
                bool neb;
                EEventArt Art;
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0008;
                    case 4011:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0d19;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_0c7c;
                        }
                    IL_0c7c:
                        num = 149;
                        if (Information.Err().Number == 3420)
                        {
                            goto IL_0c94;
                        }
                        goto IL_0cb1;
                    IL_0cb1:
                        num = 153;
                        if (Information.Err().Number == 5)
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_0cca;
                    IL_0cca:
                        num = 156;
                        if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                        {
                            ProjectData.EndApp();
                        }
                        goto IL_0cf3;
                    IL_0c25:
                        num = 143;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_0c58;
                    IL_0cf3:
                        num = 159;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        num4 = num2;
                        goto IL_0d1d;
                    IL_0c58:
                        num = 145;
                        _Modul1.Instance.FamInArb = famInArb;
                        goto IL_0c65;
                    IL_0c65:
                        num = 146;
                        _Modul1.Instance.PersInArb = PerSp1;
                        goto end_IL_0000_2;
                    IL_0d1d:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_0008;
                            case 3:
                                goto IL_0010;
                            case 4:
                                goto IL_001d;
                            case 5:
                                goto IL_0026;
                            case 6:
                                goto IL_002e;
                            case 7:
                                goto IL_0036;
                            case 8:
                                goto IL_007b;
                            case 9:
                                goto IL_00aa;
                            case 10:
                                goto IL_00cb;
                            case 11:
                                goto IL_00d5;
                            case 12:
                                goto IL_0105;
                            case 13:
                                goto IL_010e;
                            case 14:
                                goto IL_012a;
                            case 15:
                                goto IL_015a;
                            case 16:
                                goto IL_018a;
                            case 17:
                                goto IL_01ba;
                            case 18:
                            case 19:
                                goto IL_01d4;
                            case 20:
                                goto IL_01f2;
                            case 21:
                            case 22:
                                goto IL_0218;
                            case 23:
                                goto IL_0222;
                            case 24:
                                goto IL_023c;
                            case 25:
                            case 26:
                                goto IL_0249;
                            case 27:
                                goto IL_026b;
                            case 28:
                                goto IL_029b;
                            case 29:
                                goto IL_02b5;
                            case 30:
                                goto IL_02f0;
                            case 31:
                                goto IL_030d;
                            case 32:
                                goto IL_0323;
                            case 33:
                            case 34:
                                goto IL_0353;
                            case 35:
                                goto IL_035d;
                            case 36:
                                goto IL_038d;
                            case 37:
                                goto IL_03b9;
                            case 38:
                                goto IL_03f1;
                            case 39:
                                goto IL_0418;
                            case 40:
                                goto IL_0433;
                            case 41:
                                goto IL_044f;
                            case 42:
                            case 43:
                            case 44:
                            case 45:
                                goto IL_0473;
                            case 46:
                                goto IL_04a6;
                            case 47:
                            case 48:
                                goto IL_04b9;
                            case 49:
                                goto IL_04d3;
                            case 50:
                            case 51:
                                goto IL_04e7;
                            case 52:
                                goto IL_0523;
                            case 53:
                            case 54:
                                goto IL_0553;
                            case 55:
                                goto IL_055d;
                            case 56:
                                goto IL_057e;
                            case 57:
                                goto IL_0588;
                            case 58:
                                goto IL_05a2;
                            case 59:
                                goto IL_05bc;
                            case 60:
                            case 61:
                            case 62:
                                goto IL_05e8;
                            case 63:
                                goto IL_0607;
                            case 64:
                                goto IL_0622;
                            case 65:
                                goto IL_0652;
                            case 66:
                                goto IL_066d;
                            case 67:
                                goto IL_067c;
                            case 68:
                                goto IL_06ac;
                            case 69:
                            case 70:
                            case 71:
                                goto IL_06bb;
                            case 72:
                                goto IL_06ca;
                            case 73:
                                goto IL_06d9;
                            case 74:
                                goto IL_06e5;
                            case 75:
                                goto IL_06ff;
                            case 76:
                            case 77:
                            case 78:
                                goto IL_0709;
                            case 79:
                                goto IL_071e;
                            case 80:
                                goto IL_0728;
                            case 81:
                                goto IL_0749;
                            case 82:
                                goto IL_075a;
                            case 83:
                                goto IL_078a;
                            case 84:
                                goto IL_0799;
                            case 85:
                                goto IL_07cc;
                            case 86:
                            case 87:
                                goto IL_07d6;
                            case 88:
                                goto IL_0806;
                            case 89:
                                goto IL_0838;
                            case 90:
                            case 91:
                                goto IL_084b;
                            case 92:
                                goto IL_087b;
                            case 93:
                                goto IL_08ae;
                            case 94:
                                goto IL_08bc;
                            case 95:
                            case 96:
                                goto IL_08cf;
                            case 97:
                                goto IL_08ff;
                            case 98:
                                goto IL_0932;
                            case 99:
                                goto IL_0940;
                            case 100:
                            case 101:
                                goto IL_0953;
                            case 102:
                                goto IL_095d;
                            case 103:
                                goto IL_096c;
                            case 104:
                            case 105:
                                goto IL_099c;
                            case 106:
                                goto IL_09ab;
                            case 107:
                                goto IL_09db;
                            case 108:
                                goto IL_09ea;
                            case 109:
                                goto IL_0a04;
                            case 110:
                            case 111:
                                goto IL_0a0e;
                            case 112:
                                goto IL_0a3e;
                            case 113:
                                goto IL_0a4d;
                            case 114:
                                goto IL_0a5c;
                            case 115:
                                goto IL_0a66;
                            case 116:
                                goto IL_0a70;
                            case 117:
                                goto IL_0a7a;
                            case 118:
                                goto IL_0a84;
                            case 119:
                                goto IL_0a8e;
                            case 120:
                            case 121:
                                goto IL_0a9d;
                            case 122:
                                goto IL_0ab9;
                            case 123:
                                goto IL_0af1;
                            case 124:
                                goto IL_0b18;
                            case 125:
                                goto IL_0b34;
                            case 126:
                                goto IL_0b4e;
                            case 128:
                                goto IL_0b6b;
                            case 129:
                                goto IL_0b72;
                            case 127:
                            case 130:
                            case 131:
                                goto IL_0b8f;
                            case 132:
                            case 133:
                            case 134:
                                goto IL_0b9c;
                            case 135:
                                goto IL_0bb9;
                            case 136:
                                goto IL_0bdc;
                            case 137:
                            case 138:
                            case 139:
                                goto IL_0be9;
                            case 140:
                                goto IL_0bfd;
                            case 141:
                                goto IL_0c09;
                            case 142:
                                goto IL_0c18;
                            case 143:
                                goto IL_0c25;
                            case 144:
                            case 145:
                                goto IL_0c58;
                            case 146:
                                goto IL_0c65;
                            case 149:
                                goto IL_0c7c;
                            case 150:
                                goto IL_0c94;
                            case 151:
                            case 153:
                                goto IL_0cb1;
                            case 155:
                            case 156:
                                goto IL_0cca;
                            case 157:
                            case 159:
                                goto IL_0cf3;
                            default:
                                goto end_IL_0000;
                            case 147:
                            case 148:
                            case 154:
                            case 160:
                                goto end_IL_0000_2;
                        }
                        goto default;
                    IL_0c94:
                        num = 150;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_0d19;
                    IL_0c18:
                        num = 142;
                        Eltles(_Modul1.Instance.PersInArb, _Modul1.Instance.Ubg);
                        goto IL_0c25;
                    IL_0d19:
                        num4 = num2 + 1;
                        goto IL_0d1d;
                    IL_0008:
                        num = 2;
                        _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                        goto IL_0010;
                    IL_0010:
                        num = 3;
                        _Modul1.Instance.Ind1 = _M_ind;
                        goto IL_001d;
                    IL_001d:
                        num = 4;
                        Namenindex();
                        goto IL_0026;
                    IL_0026:
                        num = 5;
                        _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                        goto IL_002e;
                    IL_002e:
                        num = 6;
                        _Modul1.Instance.Person.SetFullSurname(_Modul1.Instance.BuildFullSurName(_Modul1.Instance.Person, _Modul1.Instance.DAus[89] == "1"));
                        goto IL_0036;
                    IL_0036:
                        num = 7;
                        Anz[0].SelectedText = (_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Kont[3].TrimEnd()).Trim() + " ";
                        goto IL_007b;
                    IL_007b:
                        num = 8;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                        goto IL_00aa;
                    IL_00aa:
                        num = 9;
                        Anz[0].SelectedText = _Modul1.Instance.Kont[0].Trim();
                        goto IL_00cb;
                    IL_00cb:
                        num = 10;
                        leerweg();
                        goto IL_00d5;
                    IL_00d5:
                        num = 11;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_0105;
                    IL_0105:
                        num = 12;
                        _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                        goto IL_010e;
                    IL_010e:
                        num = 13;
                        if (_Modul1.Instance.Kont[4] != "")
                        {
                            goto IL_012a;
                        }
                        goto IL_01d4;
                    IL_012a:
                        num = 14;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Italic);
                        goto IL_015a;
                    IL_015a:
                        num = 15;
                        Anz[0].SelectedText = " (" + _Modul1.Instance.Kont[4].TrimEnd() + ")";
                        goto IL_018a;
                    IL_018a:
                        num = 16;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_01ba;
                    IL_01ba:
                        num = 17;
                        Anz[0].SelectedText = " ";
                        goto IL_01d4;
                    IL_01d4:
                        num = 19;
                        if (_Modul1.Instance.Kont[5].Trim() != "")
                        {
                            goto IL_01f2;
                        }
                        goto IL_0218;
                    IL_01f2:
                        num = 20;
                        Anz[0].SelectedText = ", Sippe " + _Modul1.Instance.Kont[5];
                        goto IL_0218;
                    IL_0218:
                        num = 22;
                        leerweg();
                        goto IL_0222;
                    IL_0222:
                        num = 23;
                        if (_Modul1.Instance.DAus[62] == "1")
                        {
                            goto IL_023c;
                        }
                        goto IL_0249;
                    IL_023c:
                        num = 24;
                        FamPer = 1;
                        _Modul1.Instance.PerQu(ref FamPer);
                        goto IL_0249;
                    IL_0249:
                        num = 26;
                        if (_Modul1.Instance.Kont[30].Trim() != "")
                        {
                            goto IL_026b;
                        }
                        goto IL_0353;
                    IL_026b:
                        num = 27;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_029b;
                    IL_029b:
                        num = 28;
                        Anz[0].SelectionCharOffset = _Modul1.Instance.PrintDat.Hoch;
                        goto IL_02b5;
                    IL_02b5:
                        num = 29;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                        goto IL_02f0;
                    IL_02f0:
                        num = 30;
                        Anz[0].SelectedText = _Modul1.Instance.Kont[30];
                        goto IL_030d;
                    IL_030d:
                        num = 31;
                        Anz[0].SelectionCharOffset = 0;
                        goto IL_0323;
                    IL_0323:
                        num = 32;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_0353;
                    IL_0353:
                        num = 34;
                        leerweg();
                        goto IL_035d;
                    IL_035d:
                        num = 35;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_038d;
                    IL_038d:
                        num = 36;
                        if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields["religi"].Value))
                        {
                            goto IL_03b9;
                        }
                        goto IL_0473;
                    IL_03b9:
                        num = 37;
                        if (Operators.CompareString(DataModul.DB_PersonTable.Fields["religi"].AsString().Trim(), "", TextCompare: false) != 0)
                        {
                            goto IL_03f1;
                        }
                        goto IL_0473;
                    IL_03f1:
                        num = 38;
                        _Modul1.Instance.Ubg = DataModul.DB_PersonTable.Fields["religi"].AsInt();
                        goto IL_0418;
                    IL_0418:
                        num = 39;
                        LD = "";
                        DataModul.Textlese(_Modul1.Instance.Ubg, out _Modul1.Instance.UbgT, out LD);
                        goto IL_0433;
                    IL_0433:
                        num = 40;
                        if (_Modul1.Instance.UbgT.Trim() != "")
                        {
                            goto IL_044f;
                        }
                        goto IL_0473;
                    IL_044f:
                        num = 41;
                        Anz[0].SelectedText = " " + _Modul1.Instance.UbgT;
                        goto IL_0473;
                    IL_0473:
                        num = 45;
                        if ((_Modul1.Instance.DAus[115] == "1") | (_Modul1.Instance.DAus[116] == "1"))
                        {
                            goto IL_04a6;
                        }
                        goto IL_04b9;
                    IL_04a6:
                        num = 46;
                        Module2.Bildaus("P", "STF");
                        goto IL_04b9;
                    IL_04b9:
                        num = 48;
                        if (_Modul1.Instance.DAus[88] == "1")
                        {
                            goto IL_04d3;
                        }
                        goto IL_04e7;
                    IL_04d3:
                        num = 49;
                        Bild("P", _Modul1.Instance.PersInArb);
                        goto IL_04e7;
                    IL_04e7:
                        num = 51;
                        if ((_Modul1.Instance.DAus[99].AsDouble() == 1.0) & (_Modul1.Instance.Kont[6].Trim() != ""))
                        {
                            goto IL_0523;
                        }
                        goto IL_0553;
                    IL_0523:
                        num = 52;
                        Anz[0].SelectedText = " " + _Modul1.Instance.Kont[6].TrimEnd() + " ";
                        goto IL_0553;
                    IL_0553:
                        num = 54;
                        _Modul1.Instance.Datschalt = 1;
                        goto IL_055d;
                    IL_055d:
                        num = 55;
                        Listart = 1;
                        LD = 0.AsString();
                        Art = default;
                        neb = false;
                        _Modul1.Instance.Datles3(Listart, 0L, default, ref neb);
                        goto IL_057e;
                    IL_057e:
                        num = 56;
                        _Modul1.Instance.Datschalt = 0;
                        goto IL_0588;
                    IL_0588:
                        num = 57;
                        if (_Modul1.Instance.DAus[66] == "1")
                        {
                            goto IL_05a2;
                        }
                        goto IL_05e8;
                    IL_05a2:
                        num = 58;
                        if (_Modul1.Instance.Kont[25] != "")
                        {
                            goto IL_05bc;
                        }
                        goto IL_05e8;
                    IL_05bc:
                        num = 59;
                        Anz[0].SelectedText = " " + _Modul1.Instance.Kont[25].Trim();
                        goto IL_05e8;
                    IL_05e8:
                        num = 62;
                        if (_Modul1.Instance.DAus[106].AsDouble() == 1.0)
                        {
                            goto IL_0607;
                        }
                        goto IL_06bb;
                    IL_0607:
                        num = 63;
                        if (Aus_Text != "")
                        {
                            goto IL_0622;
                        }
                        goto IL_06bb;
                    IL_0622:
                        num = 64;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                        goto IL_0652;
                    IL_0652:
                        num = 65;
                        Anz[0].SelectedText = Aus_Text;
                        goto IL_066d;
                    IL_066d:
                        num = 66;
                        Aus_Text = "";
                        goto IL_067c;
                    IL_067c:
                        num = 67;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_06ac;
                    IL_06ac:
                        num = 68;
                        Aus_Text = "";
                        goto IL_06bb;
                    IL_06bb:
                        num = 71;
                        _Modul1.Instance.Aschalt = _Modul1.Instance.Datschalt;
                        goto IL_06ca;
                    IL_06ca:
                        num = 72;
                        PerSp1 = _Modul1.Instance.PersInArb;
                        goto IL_06d9;
                    IL_06d9:
                        num = 73;
                        if (!LSchalt)
                        {
                            goto IL_06e5;
                        }
                        goto IL_0709;
                    IL_06e5:
                        num = 74;
                        if (_Modul1.Instance.DAus[36] == "1")
                        {
                            goto IL_06ff;
                        }
                        goto IL_0709;
                    IL_06ff:
                        num = 75;
                        Paten2(_Modul1.Instance.PersInArb);
                        goto IL_0709;
                    IL_0709:
                        num = 78;
                        _Modul1.Instance.Datschalt = checked((byte)Math.Round(_Modul1.Instance.Aschalt));
                        goto IL_071e;
                    IL_071e:
                        num = 79;
                        M1_Ki = false;
                        goto IL_0728;
                    IL_0728:
                        num = 80;
                        LD = 0.AsString();
                        Listart = 1;
                        neb = false;
                        _Modul1.Instance.Datles3(Listart, 0L, default, ref neb);
                        goto IL_0749;
                    IL_0749:
                        num = 81;
                        Datschreib(ref PatText, 0);
                        goto IL_075a;
                    IL_075a:
                        num = 82;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_078a;
                    IL_078a:
                        num = 83;
                        if (!LSchalt)
                        {
                            goto IL_0799;
                        }
                        goto IL_099c;
                    IL_0799:
                        num = 84;
                        if ((_Modul1.Instance.DAus[38] == "1") | (_Modul1.Instance.DAus[39] == "1"))
                        {
                            goto IL_07cc;
                        }
                        goto IL_07d6;
                    IL_07cc:
                        num = 85;
                        Sonst();
                        goto IL_07d6;
                    IL_07d6:
                        num = 87;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_0806;
                    IL_0806:
                        num = 88;
                        if ((_Modul1.Instance.DAus[0] == "1") | (_Modul1.Instance.DAus[13] == "1"))
                        {
                            goto IL_0838;
                        }
                        goto IL_084b;
                    IL_0838:
                        num = 89;
                        Art = EEventArt.eA_300;
                        Berufe(Art);
                        goto IL_084b;
                    IL_084b:
                        num = 91;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_087b;
                    IL_087b:
                        num = 92;
                        if ((_Modul1.Instance.DAus[16] == "1") | (_Modul1.Instance.DAus[17] == "1"))
                        {
                            goto IL_08ae;
                        }
                        goto IL_08cf;
                    IL_08ae:
                        num = 93;
                        _Modul1.Instance.Ubg = 301;
                        goto IL_08bc;
                    IL_08bc:
                        num = 94;
                        Art = EEventArt.eA_301;
                        Berufe(Art);
                        goto IL_08cf;
                    IL_08cf:
                        num = 96;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_08ff;
                    IL_08ff:
                        num = 97;
                        if ((_Modul1.Instance.DAus[20] == "1") | (_Modul1.Instance.DAus[21] == "1"))
                        {
                            goto IL_0932;
                        }
                        goto IL_0953;
                    IL_0932:
                        num = 98;
                        _Modul1.Instance.Ubg = 302;
                        goto IL_0940;
                    IL_0940:
                        num = 99;
                        Art = EEventArt.eA_302;
                        Berufe(Art);
                        goto IL_0953;
                    IL_0953:
                        num = 101;
                        famInArb = _Modul1.Instance.FamInArb;
                        goto IL_095d;
                    IL_095d:
                        num = 102;
                        PerSp1 = _Modul1.Instance.PersInArb;
                        goto IL_096c;
                    IL_096c:
                        num = 103;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_099c;
                    IL_099c:
                        num = 105;
                        PerSp1 = _Modul1.Instance.PersInArb;
                        goto IL_09ab;
                    IL_09ab:
                        num = 106;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_09db;
                    IL_09db:
                        num = 107;
                        if (!LSchalt)
                        {
                            goto IL_09ea;
                        }
                        goto IL_0a9d;
                    IL_09ea:
                        num = 108;
                        if (_Modul1.Instance.DAus[37] == "1")
                        {
                            goto IL_0a04;
                        }
                        goto IL_0a0e;
                    IL_0a04:
                        num = 109;
                        Pate_bei(_Modul1.Instance.PersInArb);
                        goto IL_0a0e;
                    IL_0a0e:
                        num = 111;
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        goto IL_0a3e;
                    IL_0a3e:
                        num = 112;
                        _Modul1.Instance.PersInArb = PerSp1;
                        goto IL_0a4d;
                    IL_0a4d:
                        num = 113;
                        PerSp1 = _Modul1.Instance.PersInArb;
                        goto IL_0a5c;
                    IL_0a5c:
                        num = 114;
                        famInArb2 = _Modul1.Instance.FamInArb;
                        goto IL_0a66;
                    IL_0a66:
                        num = 115;
                        _Modul1.Instance.Trau = false;
                        goto IL_0a70;
                    IL_0a70:
                        num = 116;
                        _Modul1.Instance.Pat = false;
                        goto IL_0a7a;
                    IL_0a7a:
                        num = 117;
                        _Modul1.Instance.Trau = false;
                        goto IL_0a84;
                    IL_0a84:
                        num = 118;
                        _Modul1.Instance.FamInArb = famInArb2;
                        goto IL_0a8e;
                    IL_0a8e:
                        num = 119;
                        _Modul1.Instance.PersInArb = PerSp1;
                        goto IL_0a9d;
                    IL_0a9d:
                        num = 121;
                        if (_Modul1.Instance.DAus[1] == "1")
                        {
                            goto IL_0ab9;
                        }
                        goto IL_0b9c;
                    IL_0ab9:
                        num = 122;
                        if (Operators.CompareString(DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                        {
                            goto IL_0af1;
                        }
                        goto IL_0b9c;
                    IL_0af1:
                        num = 123;
                        _Modul1.Instance.UbgT1 = DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString();
                        goto IL_0b18;
                    IL_0b18:
                        num = 124;
                        if (_Modul1.Instance.DAus[106].AsDouble() == 1.0)
                        {
                            goto IL_0b34;
                        }
                        goto IL_0b6b;
                    IL_0b34:
                        num = 125;
                        Anz[0].SelectedText = "\n";
                        goto IL_0b4e;
                    IL_0b4e:
                        num = 126;
                        Anz[0].SelectionIndent = Abst;
                        goto IL_0b8f;
                    IL_0b6b:
                        num = 128;
                        goto IL_0b72;
                    IL_0b72:
                        num = 129;
                        Anz[0].SelectedText = " ";
                        goto IL_0b8f;
                    IL_0b8f:
                        num = 131;
                        Bemaus();
                        goto IL_0b9c;
                    IL_0b9c:
                        num = 134;
                        if (_Modul1.Instance.DAus[79] == "1")
                        {
                            goto IL_0bb9;
                        }
                        goto IL_0be9;
                    IL_0bb9:
                        num = 135;
                        if (!LSchalt & (_Modul1.Instance.PersInArb != Perstatic))
                        {
                            goto IL_0bdc;
                        }
                        goto IL_0be9;
                    IL_0bdc:
                        num = 136;
                        Weitehen();
                        goto IL_0be9;
                    IL_0be9:
                        num = 139;
                        if (Perstatic == _Modul1.Instance.PersInArb)
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_0bfd;
                    IL_0bfd:
                        num = 140;
                        DataModul.Link.GetPersonFam(_Modul1.Instance.PersInArb, ELinkKennz.lkChild, out _Modul1.Instance.Ubg);
                        goto IL_0c09;
                    IL_0c09:
                        num = 141;
                        if (_Modul1.Instance.Ubg > 0)
                        {
                            goto IL_0c18;
                        }
                        goto IL_0c58;
                    end_IL_0000:
                        break;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 4011;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    public void Pate_bei(int persInArb)
    {
        foreach (var cLink in DataModul.Link.ReadAllPers(persInArb, ELinkKennz.lkGodparent))
        {
            persInArb = cLink.iFamNr;
            _Modul1.Instance.Person_ReadNames(persInArb, _Modul1.Instance.Person);
            Namenindex();
            _Modul1.Instance.Person_ReadNames(persInArb, _Modul1.Instance.Person);
            _Modul1.Instance.Person.SetFullSurname(_Modul1.Instance.BuildFullSurName(_Modul1.Instance.Person, _Modul1.Instance.DAus[89] == "1"));
            DataModul.DB_EventTable.Index = "ArtNr";
            DataModul.DB_EventTable.Seek("=", 102.AsString(), persInArb.AsString(), "0");
            if (DataModul.DB_EventTable.NoMatch)
            {
                DataModul.DB_EventTable.Seek("=", 101.AsString(), persInArb.AsString(), "0");
            }
            if (DataModul.DB_EventTable.NoMatch)
            {
                _Modul1.Instance.sDatu = "          ";
            }
            else if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
            {
                _Modul1.Instance.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                string ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                _Modul1.Instance.Datwand1(ref _Modul1.Instance.sDatu, ds);
            }
            else
            {
                _Modul1.Instance.sDatu = "          ";
            }
            if (!_Modul1.Instance.Pat & (0 - (M1_Ki ? 1 : 0) == 0))
            {
                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            }
            else
            {
                leerweg();
                Anz[0].SelectedText = " ";
            }
            leerweg();
            if (_Modul1.Instance.DAus[106].AsDouble() == 1.0)
            {
                Anz[0].SelectedText = "\n";
                Anz[0].SelectionIndent = Abst;
            }
            else
            {
                Anz[0].SelectedText = " ";
            }
            if (0 - (M1_Ki ? 1 : 0) == 1)
            {
                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Underline);
            }
            Anz[0].SelectedText = "Pate:";
            if (_Modul1.Instance.DAus[100].AsDouble() == 1.0)
            {
                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            }
            Anz[0].SelectedText = " " + _Modul1.Instance.sDatu.Trim() + " bei " + Strings.Trim(_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Kont[3].Trim() + " " + _Modul1.Instance.Kont[0].Trim()) + ".";
            _Modul1.Instance.Pat = true;
        }
        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
    }

    public void Paten2(int persInArb)
    {
        _Modul1.Instance.Pat = false;
        PerSp1 = persInArb;
        foreach (var cLink in DataModul.Link.ReadAllFams(persInArb, ELinkKennz.lkGodparent))
        {
            persInArb = cLink.iPersNr;
            _Modul1.Instance.Person_ReadNames(persInArb, _Modul1.Instance.Person);
            Namenindex();
            _Modul1.Instance.Person_ReadNames(persInArb, _Modul1.Instance.Person);
            _Modul1.Instance.Person.SetFullSurname(_Modul1.Instance.BuildFullSurName(_Modul1.Instance.Person, _Modul1.Instance.DAus[89] == "1"));
            if (PatText == "")
            {
                PatText = Strings.Trim(_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Kont[3].Trim() + " " + _Modul1.Instance.Kont[0].Trim());
            }
            else
            {
                PatText = PatText + "; " + Strings.Trim(_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Kont[3].Trim() + " " + _Modul1.Instance.Kont[0].Trim());
            }
        }

        _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
        if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields[PersonFields.Bem2].Value) && Operators.CompareString(DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
        {
            _Modul1.Instance.UbgT1 = DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString();
            _Modul1.Instance.UbgT1 = _Modul1.Instance.Retweg(_Modul1.Instance.UbgT1);
            if (PatText == "")
            {
                PatText = _Modul1.Instance.UbgT1.Trim();
                _Modul1.Instance.UbgT1 = "";
            }
            else
            {
                PatText = PatText.Trim() + "; " + _Modul1.Instance.UbgT1.Trim();
                _Modul1.Instance.UbgT1 = "";
            }
        }
    }

    public void Sonst()
    {
        List3.Items.Clear();
        DataModul.DB_EventTable.Index = "Besu";
        DataModul.DB_EventTable.Seek("=", "105", _Modul1.Instance.PersInArb.AsString());
        if (DataModul.DB_EventTable.NoMatch)
        {
            DataModul.DB_EventTable.Index = "ArtNr";
            return;
        }
        short num = 1;
        checked
        {
            while (!DataModul.DB_EventTable.EOF)
            {
                if (DataModul.DB_EventTable.NoMatch)
                {
                    DataModul.DB_EventTable.Index = "ArtNr";
                    return;
                }
                if (!Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                {
                    M1_J = 0;
                    do
                    {
                        _Modul1.Instance.Kont1[M1_J] = "";
                        M1_J = (byte)unchecked((uint)(M1_J + 1));
                    }
                    while (unchecked(M1_J) <= 15u);
                    _Modul1.Instance.Ubg = num;
                    _Modul1.Instance.sDatu = "";
                    if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb) | (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() != 105)))
                    {
                        DataModul.DB_EventTable.Index = "ArtNr";
                        break;
                    }
                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                    {
                        _Modul1.Instance.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                        _Modul1.Instance.Kont1[1] = _Modul1.Instance.sDatu;
                    }
                    _Modul1.Instance.UbgT = "";
                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value) && DataModul.DB_EventTable.Fields[EventFields.ArtText].Value.AsDouble() > 0.0)
                    {
                        int AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                        string LD;
                        DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                        if (_Modul1.Instance.Kont[0] != "")
                        {
                            _Modul1.Instance.Kont1[7] = " " + _Modul1.Instance.Kont[0].Trim() + ": ";
                        }
                    }
                    string text = (!_Modul1.Instance.DAus[103].AsBool()) ? (_Modul1.Instance.Kont1[7] + _Modul1.Instance.Kont1[1] + new string(' ', 240).Left(240) + DataModul.DB_EventTable.Fields[EventFields.LfNr].AsString()) : ("!!!1" + _Modul1.Instance.Kont1[1] + _Modul1.Instance.Kont1[7] + new string(' ', 240).Left(240) + DataModul.DB_EventTable.Fields[EventFields.LfNr].AsString());
                    if (DataModul.DB_EventTable.Fields[EventFields.Reg].AsString() != " ")
                    {
                        text = "+" + text;
                    }
                    if (text.Trim() != "")
                    {
                        List3.Items.Add(text);
                    }
                }
                DataModul.DB_EventTable.MoveNext();
                num = (short)unchecked(num + 1);
                if (num > 70)
                {
                    break;
                }
            }
            Sonstdat();
        }
    }

    public void Heidat()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        EEventArt num5 = default;
        string Job = default;
        EEventArt eArt = default;
        string text = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                int ortNr;
                int AAA;
                int Nr;
                short LfNR;
                byte Schalt;
                short Listart;
                string LD;
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0008;
                    case 5962:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_1398;
                                default:
                                    goto end_IL_0000;
                            }
                            if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_139c;
                        }
                    end_IL_0000:
                        break;
                    IL_0008:
                        num = 2;
                        Job = "";
                        if (_Modul1.Instance.FamInArb == 0)
                        {
                            goto end_IL_0000_2;
                        }
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        if (_Modul1.Instance.DAus[76] == "1")
                        {
                            Anz[0].SelectedText = "[" + _Modul1.Instance.FamInArb.AsString() + "]";
                        }
                        DataModul.DB_FamilyTable.Index = "Fam";
                        DataModul.DB_FamilyTable.Seek("=", _Modul1.Instance.FamInArb);
                        if (DataModul.DB_FamilyTable.Fields[FamilyFields.Aeb].AsInt() == -1)
                        {
                            if (((0 - (M1_Ki ? 1 : 0) == 0) & (_Modul1.Instance.DAus[82].AsDouble() == 1.0)) | ((0 - (M1_Ki ? 1 : 0) == 1) & (_Modul1.Instance.DAus[83].AsDouble() == 1.0)))
                            {

                                Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[12] + " ";
                            }
                            goto end_IL_0000_2;
                        }
                        num5 = EEventArt.eA_500;
                        goto IL_01c3;
                    IL_01c3: // <========== 3
                        num = 19;
                        if (num5 == EEventArt.eA_500)
                        {
                            eArt = EEventArt.eA_501;
                            goto IL_0203;
                        }
                        if (num5 == EEventArt.eA_501)
                        {
                            eArt = EEventArt.eA_500;
                        }
                        else
                        {

                            eArt = num5;
                        }
                        goto IL_0203;
                    IL_0203: // <========== 4
                        num = 28;
                        _Modul1.Instance.sDatu = "";
                        short num6 = 1;
                        while (num6 <= 8)
                        {
                            _Modul1.Instance.Kont1[num6] = "";
                            num6 = checked((short)unchecked(num6 + 1));
                        }
                        DataModul.DB_EventTable.Index = "ArtNr";
                        DataModul.DB_EventTable.Seek("=", eArt, _Modul1.Instance.FamInArb.AsString(), "0");
                        string ds;
                        if (!DataModul.DB_EventTable.NoMatch)
                        {
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                            {
                                _Modul1.Instance.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                _Modul1.Instance.Datwand1(ref _Modul1.Instance.sDatu, ds);
                                _Modul1.Instance.Kont1[1] = _Modul1.Instance.sDatu;
                            }
                            goto IL_0374;
                        }
                        goto IL_1312;
                    IL_0374:
                        num = 43;
                        if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                        {
                            ds = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                            _Modul1.Instance.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                            _Modul1.Instance.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumB].AsDate().AsString().Trim(), 8);
                            _Modul1.Instance.Datwand1(ref _Modul1.Instance.sDatu, ds);
                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString().Trim(), "", TextCompare: false) == 0)
                            {
                                _Modul1.Instance.Kont1[3] = "/" + _Modul1.Instance.sDatu;
                                goto IL_04ab;
                            }
                            _Modul1.Instance.Kont1[3] = " " + _Modul1.Instance.sDatu;
                        }
                        goto IL_04ab;
                    IL_04ab: // <========== 3
                        num = 55;
                        _Modul1.Instance.UbgT = "";
                        if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.DatumText].Value))
                        {
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt() > 0)
                            {
                                string value = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsString();
                                AAA = value.AsInt();
                                LD = "";
                                _Modul1.Instance.UbgT = DataModul.TextLese1(AAA);
                                value = AAA.AsString();
                                if (_Modul1.Instance.UbgT.Trim() != "")
                                {
                                    _Modul1.Instance.Kont1[3] = _Modul1.Instance.Kont1[3] + " (" + _Modul1.Instance.UbgT.Trim() + ")";
                                    _Modul1.Instance.UbgT = "";
                                }

                            }
                        }
                        goto IL_05b4;
                    IL_05b4: // <========== 3
                        num = 66;
                        if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                        {
                            ortNr = checked((int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble()));
                            _Modul1.Instance.Kont1[5] = _Modul1.Instance.ortles1(ortNr, 1, (i, s) => _Modul1.Instance.ExportPlace(i, s, _Modul1.Instance.Ind1, M_Namen));
                        }
                        goto IL_063b;
                    IL_063b:
                        num = 71;
                        if (DataModul.DB_EventTable.Fields[EventFields.Platz].Value.AsDouble() > 0.0)
                        {
                            AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                            LD = "";
                            _Modul1.Instance.Kont[0] = DataModul.TextLese1(AAA);
                            if (_Modul1.Instance.Kont[0] != "")
                            {
                                _Modul1.Instance.Kont1[6] = " " + _Modul1.Instance.Kont[0].Trim();

                            }
                        }
                        goto IL_06f3;
                    IL_06f3: // <========== 3
                        num = 77;
                        if (DataModul.DB_EventTable.Fields[EventFields.KBem].Value.AsDouble() > 0.0)
                        {
                            AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                            LD = "";
                            _Modul1.Instance.Kont[0] = DataModul.TextLese1(AAA);
                            if (_Modul1.Instance.Kont[0] != "")
                            {
                                _Modul1.Instance.Kont1[7] = " " + _Modul1.Instance.Kont[0].Trim();

                            }
                        }
                        goto IL_07ab;
                    IL_07ab: // <========== 3
                        num = 83;
                        num6 = 1;
                        while (num6 <= 6)
                        {
                            if (_Modul1.Instance.Kont1[num6] == "0")
                            {
                                _Modul1.Instance.Kont1[num6] = "";
                            }
                            num6 = checked((short)unchecked(num6 + 1));
                        }
                        if ((_Modul1.Instance.Kont1[1].Trim() != "") | (_Modul1.Instance.Kont1[2].Trim() != "") | (_Modul1.Instance.Kont1[3].Trim() != "") | (_Modul1.Instance.Kont1[5].Trim() != "") | (_Modul1.Instance.Kont1[6].Trim() != "") | (_Modul1.Instance.UbgT.Trim() != "") | ((Strings.Len(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim()) > 0) & (_Modul1.Instance.DAus[5] == "1")) | ((Strings.Len(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim()) > 0) & (_Modul1.Instance.DAus[6] == "1")))
                        {
                            text = "";
                            switch (eArt)
                            {
                                case EEventArt.eA_500:
                                    if (!((0 - (M1_Ki ? 1 : 0) == 0) & (_Modul1.Instance.DAus[52] == "0")))
                                    {
                                        if (!((0 - (M1_Ki ? 1 : 0) == 1) & (_Modul1.Instance.DAus[57] == "0")))
                                        {
                                            text = _Modul1.Instance.DTxt[5];
                                            goto IL_0c76;
                                        }
                                    }
                                    goto IL_1312;
                                case EEventArt.eA_501:
                                    goto IL_09d2;
                                case EEventArt.eA_Marriage:
                                    goto IL_0a3f;
                                case EEventArt.eA_MarrReligious:
                                    goto IL_0aac;
                                case EEventArt.eA_504:
                                    goto IL_0b19;
                                case EEventArt.eA_505:
                                    goto IL_0b90;
                                case EEventArt.eA_507:
                                    goto IL_0c04;
                                default:
                                    break;
                            }
                            goto IL_0c76;
                        }
                        goto IL_0e3d;
                    IL_09d2:
                        num = 103;
                        if (!((0 - (M1_Ki ? 1 : 0) == 0) & (_Modul1.Instance.DAus[53] == "0")))
                        {
                            if (!((0 - (M1_Ki ? 1 : 0) == 1) & (_Modul1.Instance.DAus[58] == "0")))
                            {
                                text = _Modul1.Instance.DTxt[6];
                                goto IL_0c76;
                            }
                        }
                        goto IL_1312;
                    IL_0a3f:
                        num = 112;
                        if (!((0 - (M1_Ki ? 1 : 0) == 0) & (_Modul1.Instance.DAus[64] == "0")))
                        {
                            if (!((0 - (M1_Ki ? 1 : 0) == 1) & (_Modul1.Instance.DAus[65] == "0")))
                            {
                                text = _Modul1.Instance.DTxt[7];
                                goto IL_0c76;
                            }
                        }
                        goto IL_1312;
                    IL_0aac:
                        num = 121;
                        if (!((0 - (M1_Ki ? 1 : 0) == 0) & (_Modul1.Instance.DAus[54] == "0")))
                        {
                            if (!((0 - (M1_Ki ? 1 : 0) == 1) & (_Modul1.Instance.DAus[59] == "0")))
                            {
                                text = _Modul1.Instance.DTxt[8];
                                goto IL_0c76;
                            }
                        }
                        goto IL_1312;
                    IL_0b19:
                        num = 130;
                        if (!((0 - (M1_Ki ? 1 : 0) == 0) & (_Modul1.Instance.DAus[55] == "0")))
                        {
                            if (!((0 - (M1_Ki ? 1 : 0) == 1) & (_Modul1.Instance.DAus[60] == "0")))
                            {
                                text = _Modul1.Instance.DTxt[9];
                                goto IL_0c76;
                            }
                        }
                        goto IL_1312;
                    IL_0b90:
                        num = 139;
                        if (!((0 - (M1_Ki ? 1 : 0) == 0) & (_Modul1.Instance.DAus[56] == "0")))
                        {
                            if (!((0 - (M1_Ki ? 1 : 0) == 1) & (_Modul1.Instance.DAus[61] == "0")))
                            {
                                text = _Modul1.Instance.DTxt[10];
                                goto IL_0c76;
                            }
                        }
                        goto IL_1312;
                    IL_0c04:
                        num = 148;
                        if (!((0 - (M1_Ki ? 1 : 0) == 0) & (_Modul1.Instance.DAus[84] == "0")))
                        {
                            if (!((0 - (M1_Ki ? 1 : 0) == 1) & (_Modul1.Instance.DAus[86] == "0")))
                            {
                                text = _Modul1.Instance.DTxt[15];
                                goto IL_0c76;
                            }
                        }
                        goto IL_1312;
                    IL_0c76: // <========== 9
                        num = 156;
                        if ((_Modul1.Instance.Kont1[1].Trim() != "") | (_Modul1.Instance.Kont1[3].Trim() != "") | (_Modul1.Instance.Kont1[5].Trim() != "") | (_Modul1.Instance.Kont1[6].Trim() != "") | (_Modul1.Instance.Kont1[7].Trim() != "") | (_Modul1.Instance.Kont1[8].Trim() != "") | (_Modul1.Instance.UbgT.Trim() != ""))
                        {
                            leerweg();
                            if (_Modul1.Instance.DAus[106].AsDouble() == 1.0)
                            {
                                Anz[0].SelectedText = "\n";
                                Anz[0].SelectionIndent = Abst;
                            }
                            else
                            {

                                Anz[0].SelectedText = " ";
                            }
                            goto IL_0dde;
                        }
                        goto IL_0e3d;
                    IL_0dde: // <========== 3
                        num = 165;
                        Job = "";
                        Job = Module2.Jobdreh(Job);
                        Job = (text + " " + Job).Trim();
                        Anz[0].SelectedText = Job;
                        Hei = true;
                        goto IL_0e3d;
                    IL_0e3d: // <========== 4
                        num = 172;
                        if (Strings.Len(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim()) > 0)
                        {
                            if (_Modul1.Instance.DAus[5] == "1")
                            {
                                _Modul1.Instance.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString();
                                Bemaus();

                            }
                        }
                        goto IL_0ec2;
                    IL_0ec2: // <========== 3
                        num = 178;
                        if (Strings.Len(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim()) > 0)
                        {
                            if (_Modul1.Instance.DAus[6] == "1")
                            {
                                _Modul1.Instance.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString();
                                Bemaus();

                            }
                        }
                        goto IL_0f47;
                    IL_0f47: // <========== 3
                        num = 184;
                        Nr = _Modul1.Instance.FamInArb;
                        LfNR = 0;
                        _Modul1.Instance.QuellenDatum(ref Nr, eArt, ref LfNR);
                        _Modul1.Instance.FamInArb = Nr.AsInt();
                        if (_Modul1.Instance.Kont1[9].Trim() != "")
                        {
                            leerweg();
                            Anz[0].SelectionCharOffset = _Modul1.Instance.PrintDat.Hoch;
                            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont1[9].Trim() + " ";
                            Anz[0].SelectionCharOffset = 0;
                            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        }
                        goto IL_1086;
                    IL_1086:
                        num = 193;
                        if (_Modul1.Instance.DAus[96].AsBool())
                        {
                            _Modul1.Instance.PersSp = _Modul1.Instance.PersInArb;
                            byte b = 1;
                            while (b <= 100u)
                            {
                                _Modul1.Instance.KontSP1[b] = _Modul1.Instance.Kont1[b];
                                _Modul1.Instance.KontSP[b] = _Modul1.Instance.Kont[b];
                                _Modul1.Instance.Kont[b] = "";
                                _Modul1.Instance.Kont1[b] = "";
                                b = checked((byte)unchecked((uint)(b + 1)));
                            }
                            LD = "";
                            _Modul1.Instance.Zeugsu(eArt, 0, 1, 0L);
                            _Modul1.Instance.PersInArb = _Modul1.Instance.PersSp;
                            string text2 = _Modul1.Instance.Kont1[20];
                            b = 1;
                            while (b <= 100u)
                            {
                                _Modul1.Instance.Kont1[b] = _Modul1.Instance.KontSP1[b];
                                _Modul1.Instance.Kont[b] = _Modul1.Instance.KontSP[b];
                                _Modul1.Instance.KontSP[b] = "";
                                _Modul1.Instance.KontSP1[b] = "";
                                b = checked((byte)unchecked((uint)(b + 1)));
                            }
                            if (text2 != "")
                            {
                                leerweg();
                                if (text2.Trim().Right(1) != ";")
                                {
                                    text2 = text2.Trim() + ";";
                                }
                                if (_Modul1.Instance.DAus[100].AsDouble() == 1.0)
                                {
                                    Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                }
                                Anz[0].SelectedText = ", Zeugen: " + text2.Trim();
                                text2 = "";
                                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);

                            }
                        }
                        goto IL_1305;
                    IL_1305: // <========== 3
                        num = 223;
                        leerweg();
                        goto IL_1312;
                    IL_1312: // <========== 10
                        num = 224;
                        lErl = 22;
                        num5++;
                        if (num5 <= EEventArt.eA_507)
                        {
                            goto IL_01c3;
                        }
                        leerweg();
                        goto end_IL_0000_2;
                    IL_1398:
                        num4 = num2 + 1;
                        goto IL_139c;
                    IL_139c:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 19:
                                goto IL_01c3;
                            case 21:
                            case 24:
                            case 27:
                            case 28:
                                goto IL_0203;
                            case 42:
                            case 43:
                                goto IL_0374;
                            case 50:
                            case 53:
                            case 54:
                            case 55:
                                goto IL_04ab;
                            case 63:
                            case 64:
                            case 65:
                            case 66:
                                goto IL_05b4;
                            case 70:
                            case 71:
                                goto IL_063b;
                            case 75:
                            case 76:
                            case 77:
                                goto IL_06f3;
                            case 81:
                            case 82:
                            case 83:
                                goto IL_07ab;
                            case 91:
                            case 101:
                            case 110:
                            case 119:
                            case 128:
                            case 137:
                            case 146:
                            case 155:
                            case 156:
                                goto IL_0c76;
                            case 161:
                            case 164:
                            case 165:
                                goto IL_0dde;
                            case 170:
                            case 171:
                            case 172:
                                goto IL_0e3d;
                            case 176:
                            case 177:
                            case 178:
                                goto IL_0ec2;
                            case 182:
                            case 183:
                            case 184:
                                goto IL_0f47;
                            case 192:
                            case 193:
                                goto IL_1086;
                            case 221:
                            case 222:
                            case 223:
                                goto IL_1305;
                            case 35:
                            case 95:
                            case 98:
                            case 104:
                            case 107:
                            case 113:
                            case 116:
                            case 122:
                            case 125:
                            case 131:
                            case 134:
                            case 140:
                            case 143:
                            case 149:
                            case 152:
                            case 224:
                                goto IL_1312;
                            case 4:
                            case 15:
                            case 16:
                            case 227:
                            case 232:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 5962;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 5
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    public void Kisort()
    {
        List1[1].Items.Clear();
        _0024STATIC_0024Kisort_00242001_0024I = 1;
        while (_Modul1.Instance.Family.Kind[_0024STATIC_0024Kisort_00242001_0024I] != 0)
        {
            _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Kind[_0024STATIC_0024Kisort_00242001_0024I];
            DataModul.DB_EventTable.Seek("=", "101", _Modul1.Instance.PersInArb.AsString(), "0");
            if (DataModul.DB_EventTable.NoMatch)
            {
                DataModul.DB_EventTable.Seek("=", "102", _Modul1.Instance.PersInArb.AsString(), "0");
                if (DataModul.DB_EventTable.NoMatch)
                {
                    _0024STATIC_0024Kisort_00242001_0024Datum = "00000000";
                    goto IL_01fb;
                }
            }
            else if (DataModul.DB_EventTable.Fields[EventFields.DatumV].AsInt() == 0)
            {
                DataModul.DB_EventTable.Seek("=", "102", _Modul1.Instance.PersInArb.AsString(), "0");
                if (DataModul.DB_EventTable.NoMatch)
                {
                    _0024STATIC_0024Kisort_00242001_0024Datum = "00000000";

                }
            }
            else
            {
                _0024STATIC_0024Kisort_00242001_0024Datum = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
            }
            goto IL_01fb;
        IL_01fb:
            List1[1].Items.Add(_0024STATIC_0024Kisort_00242001_0024Datum + _Modul1.Instance.PersInArb.AsString());
            checked
            {
                _0024STATIC_0024Kisort_00242001_0024I = (byte)unchecked((uint)(_0024STATIC_0024Kisort_00242001_0024I + 1));
            }
            if (_0024STATIC_0024Kisort_00242001_0024I > 99u)
            {
                break;
            }
        }
    }

    public void Dat()
    {
        //Discarded unreachable code: IL_02ec
        _Modul1.Instance.Datschalt = 1;
        short Listart = 1;
        bool neb = false;
        _Modul1.Instance.Datles3(Listart, 0L, default, ref neb);
        if (_Modul1.Instance.DAus[73] == "0")
        {
            _Modul1.Instance.PrintDat.High = 0;
        }
        _Modul1.Instance.DAus[105] = "2020";
        checked
        {
            if (DGSchalt)
            {
                Lauf++;
                GLauf[Gen + 1] = Lauf;
                if (_Modul1.Instance.DAus[75] == "1")
                {
                    DataModul.DB_DoppelTable.Seek("=", _Modul1.Instance.PersInArb);
                    if (!DataModul.DB_DoppelTable.NoMatch)
                    {
                        Anz[0].SelectedText = (" " + "siehe " + DataModul.DB_DoppelTable.Fields[DoppelFields.Nr].AsString()).AsString();
                        return;
                    }
                    DataModul.DB_DoppelTable.AddNew();
                    DataModul.DB_DoppelTable.Fields[DoppelFields.Pr].Value = _Modul1.Instance.PersInArb;
                    DataModul.DB_DoppelTable.Fields[DoppelFields.Nr].Value = (Gen + 1.AsString()).Trim() + "." + Lauf.AsString().Trim();
                    DataModul.DB_DoppelTable.Update();
                }
                leerweg();
                if (Gen < MaxGen)
                {
                    Anz[0].SelectedText = " siehe " + (Gen + 1.AsString()).Trim() + "." + Lauf.AsString().Trim();
                    if (_Modul1.Instance.DAus[90] == "1")
                    {
                        Datschreib();
                    }
                    DataModul.NB_FrauTable.AddNew();
                    DataModul.NB_FrauTable.Fields["Nr"].Value = _Modul1.Instance.PersInArb;
                    DataModul.NB_FrauTable.Fields["Gen"].Value = Gen + 1;
                    DataModul.NB_FrauTable.Fields["Kek2"].Value = Lauf;
                    DataModul.NB_FrauTable.Fields["Alt"].Value = Aus_Nr;
                    DataModul.NB_FrauTable.Update();
                    return;
                }
            }
            if (_Modul1.Instance.DAus[67] == "1" && _Modul1.Instance.Kont[25] != "")
            {
                Anz[0].SelectedText = " " + _Modul1.Instance.Kont[25].Trim();
            }
            _0024STATIC_0024Dat_00242001_0024PerSp1 = _Modul1.Instance.PersInArb;
            if (_Modul1.Instance.DAus[48] == "1")
            {
                M1_Ki = true;
                Paten2(_Modul1.Instance.PersInArb);
                M1_Ki = false;
                _Modul1.Instance.PersInArb = _0024STATIC_0024Dat_00242001_0024PerSp1;
            }
            Datschreib();
            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
            Anz[0].SelectionCharOffset = 0;
            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
            _0024STATIC_0024Dat_00242001_0024PerSp1 = _Modul1.Instance.PersInArb;
            if (DataModul.DB_PersonTable.Fields["sex"].AsString() == "F")
            {
                _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
            }
            _0024STATIC_0024Dat_00242001_0024Kennz1 = _Modul1.Instance.eLKennz;
            _Modul1.Instance.PersInArb = _0024STATIC_0024Dat_00242001_0024PerSp1;
            _Modul1.Instance.PersInArb = _0024STATIC_0024Dat_00242001_0024PerSp1;
            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            if (_Modul1.Instance.DAus[49] == "1")
            {
                M1_Ki = true;
                Pate_bei(_Modul1.Instance.PersInArb);
                M1_Ki = false;
            }
            _Modul1.Instance.PersInArb = _0024STATIC_0024Dat_00242001_0024PerSp1;
            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
            if ((_Modul1.Instance.DAus[42] == "1") | (_Modul1.Instance.DAus[43] == "1"))
            {
                M1_Ki = true;
                Sonst();
                M1_Ki = false;
            }
            EEventArt Art;
            if ((_Modul1.Instance.DAus[24] == "1") | (_Modul1.Instance.DAus[25] == "1"))
            {
                M1_Ki = true;
                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                Art = EEventArt.eA_300;
                Berufe(Art);
                M1_Ki = false;
            }
            if ((_Modul1.Instance.DAus[28] == "1") | (_Modul1.Instance.DAus[29] == "1"))
            {
                M1_Ki = true;
                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                Art = EEventArt.eA_301;
                Berufe(Art);
                M1_Ki = false;
            }
            if ((_Modul1.Instance.DAus[32] == "1") | (_Modul1.Instance.DAus[33] == "1"))
            {
                M1_Ki = true;
                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                Art = EEventArt.eA_302;
                Berufe(Art);
                M1_Ki = false;
            }
            if (_Modul1.Instance.DAus[88] == "1")
            {
                Bild("P", _Modul1.Instance.PersInArb);
            }
            _0024STATIC_0024Dat_00242001_0024PerSp1 = _Modul1.Instance.PersInArb;
            _0024STATIC_0024Dat_00242001_0024PerSp1 = _Modul1.Instance.PersInArb;
            _Modul1.Instance.eLKennz = DataModul.Person.GetSex(_Modul1.Instance.PersInArb) == "F" ? ELinkKennz.lkMother : ELinkKennz.lkFather;
            var aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
            List1[0].Items.Clear();
            if (aiFams.Count > 1)
            {
                foreach (var iFam in aiFams)
                {
                    _Modul1.Instance.FamInArb = iFam;
                    _Modul1.Instance.Datschalt = 10;
                    _Modul1.Instance.Famdatles1(0, out var asFamDate);
                    _Modul1.Instance.Datschalt = 0;
                    if (asFamDate[12].Trim() == "")
                    {
                        asFamDate[12] = asFamDate[13];
                    }
                    if (asFamDate[12].Trim() == "")
                    {
                        asFamDate[12] = asFamDate[15];
                    }
                    if (asFamDate[12].Trim() == "")
                    {
                        asFamDate[12] = asFamDate[17];
                    }
                    if (asFamDate[12].Trim() == "")
                    {
                        asFamDate[12] = asFamDate[10];
                    }
                    if (asFamDate[12].Trim() == "")
                    {
                        asFamDate[12] = asFamDate[18];
                    }
                    asFamDate[12] = "        " + asFamDate[12].Trim().Right(8);
                    List1[0].Items.Add(asFamDate[12] + "          " + _Modul1.Instance.FamInArb.AsString().Right(10) + "          " + _Modul1.Instance.PersInArb.AsString().Right(10) + _0024STATIC_0024Dat_00242001_0024Kennz1);
                }
            }
            else if (aiFams.Count == 0)
            {
                List1[0].Items.Add(new string(' ', 20));
            }
            else
            {
                _Modul1.Instance.FamInArb = aiFams[0];
                List1[0].Items.Add(("          " + _Modul1.Instance.FamInArb.AsString()).Right(10) +
                    ("          " + _Modul1.Instance.PersInArb.AsString()).Right(10) + _0024STATIC_0024Dat_00242001_0024Kennz1);
            }
            int count = List1[0].Items.Count;
            for (_0024STATIC_0024Dat_00242001_0024Ke = 0; _0024STATIC_0024Dat_00242001_0024Ke <= count; _0024STATIC_0024Dat_00242001_0024Ke++)
            {
                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1[0].Items[_0024STATIC_0024Dat_00242001_0024Ke].AsString(), 9, 10)));
                if (_Modul1.Instance.FamInArb > 0)
                {
                    _0024STATIC_0024Dat_00242001_0024PerSp1 = (int)Math.Round(Conversion.Val(Strings.Mid(List1[0].Items[_0024STATIC_0024Dat_00242001_0024Ke].AsString(), 19, 10)));
                    _0024STATIC_0024Dat_00242001_0024Kennz1 = List1[0].Items[_0024STATIC_0024Dat_00242001_0024Ke].AsString().Right(1).AsEnum<ELinkKennz>();
                    _Modul1.Instance.Famdatles2();
                    if (_Modul1.Instance.Kont[2] == "")
                    {
                        _Modul1.Instance.Kont[2] = _Modul1.Instance.Kont[3];
                        _Modul1.Instance.Kont[3] = "";
                    }
                    if (_Modul1.Instance.DAus[106].AsDouble() == 1.0 && Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
                    {
                        Anz[0].SelectedText = "\n";
                    }
                    if (List1[0].Items.Count > 1)
                    {
                        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        if (_0024STATIC_0024Dat_00242001_0024Ke >= 1)
                        {
                            Anz[0].SelectedText = _0024STATIC_0024Dat_00242001_0024Ke + 1.AsString() + ". " + _Modul1.Instance.DTxt[11] + " ";
                        }
                        else
                        {
                            Anz[0].SelectedText = _0024STATIC_0024Dat_00242001_0024Ke + 1.AsString() + ". " + _Modul1.Instance.DTxt[11] + " ";
                        }
                    }
                    else
                    {
                        Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[11] + " ";
                    }
                    M1_Ki = true;
                    Heidat();
                    if (_Modul1.Instance.DAus[88] == "1")
                    {
                        Bild("F", _Modul1.Instance.FamInArb);
                    }
                    if (_Modul1.Instance.DAus[63] == "1")
                    {
                        FamQuell();
                    }
                    Anz[0].SelectionCharOffset = 0;
                    _Modul1.Instance.Famles();
                    Anz[0].SelectionCharOffset = 0;
                    List1[3].Items.Clear();
                    _0024STATIC_0024Dat_00242001_0024I = 1;
                    while (_Modul1.Instance.Family.Kind[_0024STATIC_0024Dat_00242001_0024I] != 0)
                    {
                        _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Kind[_0024STATIC_0024Dat_00242001_0024I];
                        _Modul1.Instance.Datschalt = 10;
                        Listart = 1;
                        neb = false;
                        _Modul1.Instance.Datles3(Listart, 0L, default, ref neb);
                        Anz[0].SelectedText = _Modul1.Instance.Kont[25];
                        _Modul1.Instance.Datschalt = 0;
                        if (_Modul1.Instance.Kont[11].Trim() == "")
                        {
                            _Modul1.Instance.Kont[11] = _Modul1.Instance.Kont[12];
                        }
                        _Modul1.Instance.Kont[11] = "        " + _Modul1.Instance.Kont[11].Right(8);
                        List1[3].Items.Add(_Modul1.Instance.Kont[11] + _Modul1.Instance.Family.Kind[_0024STATIC_0024Dat_00242001_0024I].AsString().Trim());
                        _0024STATIC_0024Dat_00242001_0024I = (byte)unchecked((uint)(_0024STATIC_0024Dat_00242001_0024I + 1));
                        if (unchecked(_0024STATIC_0024Dat_00242001_0024I) > 99u)
                        {
                            break;
                        }
                    }
                    _Modul1.Instance.eLKennz = _0024STATIC_0024Dat_00242001_0024Kennz1;
                    Partles(_0024STATIC_0024Dat_00242001_0024PerSp1);
                }
                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            }
        }
    }

    public void Datschreib()
    {
        if (_Modul1.Instance.DAus[63] == "1")
        {
            int FamPer = 1;
            _Modul1.Instance.PerQu(ref FamPer);
            Anz[0].SelectionCharOffset = _Modul1.Instance.PrintDat.Hoch;
            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            Anz[0].SelectedText = _Modul1.Instance.Kont[30];
            Anz[0].SelectionCharOffset = 0;
            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
        }
        if ((_Modul1.Instance.DAus[115] == "1") | (_Modul1.Instance.DAus[116] == "1"))
        {
            Module2.Bildaus("P", "STF");
        }
        if (_Modul1.Instance.DAus[88] == "1")
        {
            Bild("P", _Modul1.Instance.PersInArb);
        }
        M1_Ki = true;
        _Modul1.Instance.Datschalt = 0;
        short Listart = 1;
        string Ahne = 0.AsString();
        bool neb = false;
        _Modul1.Instance.Datles3(Listart, 0L, default, ref neb);
        Datschreib(ref PatText, 0);
        if (_Modul1.Instance.DAus[7] == "1")
        {
            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
            if (Operators.CompareString(DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
            {
                if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
                {
                    Anz[0].SelectedText = "\n";
                }
                _Modul1.Instance.UbgT1 = DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString();
                Bemaus();
            }
        }
        leerweg();
    }

    public void FamQuell()
    {
        int FamPer = 2;
        _Modul1.Instance.PerQu(ref FamPer);
        leerweg();
        Anz[0].SelectionCharOffset = _Modul1.Instance.PrintDat.Hoch;
        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
        Anz[0].SelectedText = _Modul1.Instance.Kont[30];
        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
        Anz[0].SelectionCharOffset = 0;
    }

    public void Partles(int PerSp1)
    {
        ELinkKennz kennz = _Modul1.Instance.eLKennz;
        if (PerSp1 == _Modul1.Instance.Family.Frau)
        {
            _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
            _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
        }
        else
        {
            _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau;
            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
        }
        if (kennz == ELinkKennz.lkMother)
        {
            _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
            _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
        }
        else
        {
            _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau;
            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
        }
        _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
        Namenindex();
        _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
        _0024STATIC_0024Partles_002420118_0024VName = _Modul1.Instance.Kont[99];
        M_Namen = _Modul1.Instance.Kont[0];
        leerweg();
        Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
        _Modul1.Instance.Ind1 = _M_ind + "." + Strings.Trim(checked(IQ + 1).AsString());
        if ((_Modul1.Instance.Kont[1].Trim() != "") | (_Modul1.Instance.Kont[3].Trim() != "") | (_Modul1.Instance.Kont[0].Trim() != ""))
        {
            Anz[0].SelectedText = " mit " + (_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Kont[3].TrimEnd()).Trim();
            _Modul1.Instance.Person.SetFullSurname(_Modul1.Instance.BuildFullSurName(_Modul1.Instance.Person, _Modul1.Instance.DAus[89] == "1"));
            _0024STATIC_0024Partles_002420118_0024Namen1 = _Modul1.Instance.Kont[0];
            leerweg();
            Anz[0].SelectionCharOffset = 0;
            Anz[0].SelectedText = " ";
            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
            Anz[0].SelectedText = _Modul1.Instance.Kont[0];
            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
        }
        else
        {
            Anz[0].SelectedText = " mit unbekanntem Partner";
        }
        _Modul1.Instance.Aschalt = _Modul1.Instance.Datschalt;
        leerweg();
        checked
        {
            _Modul1.Instance.Datschalt = (byte)Math.Round(_Modul1.Instance.Aschalt);
            _Modul1.Instance.Datschalt = 1;
            short Listart = 1;
            string Ahne = 0.AsString();
            EEventArt Art = default;
            bool neb = false;
            _Modul1.Instance.Datles3(Listart, 0L, Art, ref neb);
            _Modul1.Instance.Datschalt = 0;
            if (_Modul1.Instance.DAus[67] == "1" && _Modul1.Instance.Kont[25] != "")
            {
                Anz[0].SelectedText = " " + _Modul1.Instance.Kont[25].Trim();
            }
            Datschreib();
            if ((_Modul1.Instance.DAus[24] == "1") | (_Modul1.Instance.DAus[25] == "1"))
            {
                M1_Ki = true;
                Art = EEventArt.eA_300;
                Berufe(Art);
                M1_Ki = false;
            }
            if ((_Modul1.Instance.DAus[28] == "1") | (_Modul1.Instance.DAus[29] == "1"))
            {
                M1_Ki = true;
                Art = EEventArt.eA_301;
                Berufe(Art);
                M1_Ki = false;
            }
            if ((_Modul1.Instance.DAus[32] == "1") | (_Modul1.Instance.DAus[33] == "1"))
            {
                M1_Ki = true;
                Art = EEventArt.eA_302;
                Berufe(Art);
                M1_Ki = false;
            }
            M1_Ki = true;
            if (_Modul1.Instance.DAus[88] == "1")
            {
                Bild("P", _Modul1.Instance.PersInArb);
            }
            if (_Modul1.Instance.DAus[80] == "1")
            {
                Weitehen();
            }
            M1_Ki = false;
            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            DataModul.Link.GetPersonFam(_Modul1.Instance.PersInArb, ELinkKennz.lkChild, out int iFam);
            _Modul1.Instance.Ubg = iFam;
            if (_Modul1.Instance.Ubg > 0)
            {
                Eltles(_Modul1.Instance.PersInArb, _Modul1.Instance.Ubg);
                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            }
            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
            if (List1[3].Items.Count <= 0)
            {
                return;
            }
            Anz[0].SelectedText = "\n";
            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            Anz[0].SelectionIndent = 50;
            if (SKennz == "1")
            {
                if (List1[3].Items.Count == 1)
                {
                    Anz[0].SelectedText = "Kind " + _0024STATIC_0024Partles_002420118_0024Namen2;
                }
                else if (List1[3].Items.Count > 1)
                {
                    Anz[0].SelectedText = "Kinder " + _0024STATIC_0024Partles_002420118_0024Namen2;
                }
            }
            else if (List1[3].Items.Count == 1)
            {
                Anz[0].SelectedText = "Kind " + _0024STATIC_0024Partles_002420118_0024Namen1;
            }
            else if (List1[3].Items.Count > 1)
            {
                Anz[0].SelectedText = "Kinder " + _0024STATIC_0024Partles_002420118_0024Namen1;
            }
            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            int num = List1[3].Items.Count - 1;
            for (int i = 0; i <= num; i++)
            {
                if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
                {
                    Anz[0].SelectedText = "\n";
                }
                Anz[0].SelectedText = "(" + (i + 1.AsString()).Trim() + ") ";
                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                _Modul1.Instance.PersInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1[3].Items[i].AsString(), 9, 10)));
                _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                kennz = ELinkKennz.lkMother;
                if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                {
                    kennz = ELinkKennz.lkFather;
                }
                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                M_Namen = _Modul1.Instance.Kont[0];
                _0024STATIC_0024Partles_002420118_0024VName = _Modul1.Instance.Kont[99];
                _Modul1.Instance.Person.SetFullSurname(_Modul1.Instance.BuildFullSurName(_Modul1.Instance.Person, _Modul1.Instance.DAus[89] == "1"));
                Anz[0].SelectedText = (_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Kont[3].TrimEnd()).Trim() + " ";
                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                Anz[0].SelectedText = _Modul1.Instance.Kont[0];
                Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                _Modul1.Instance.Ind1 = _M_ind + "." + (IQ + 1.AsString()).Trim();
                _Modul1.Instance.Aschalt = unchecked(_Modul1.Instance.Datschalt);
                _Modul1.Instance.Datschalt = 1;
                Art = 0;
                Ahne = 0.AsString();
                Listart = 1;
                neb = false;
                _Modul1.Instance.Datles3(Listart, 0L, Art, ref neb);
                _Modul1.Instance.Datschalt = 0;
                if (_Modul1.Instance.DAus[67] == "1" && _Modul1.Instance.Kont[25] != "")
                {
                    Anz[0].SelectedText = " " + _Modul1.Instance.Kont[25].Trim();
                }
                if (_Modul1.Instance.Kont[1] == "")
                {
                    _Modul1.Instance.Kont[1] = _Modul1.Instance.Kont[2];
                }
                if (_Modul1.Instance.Kont[1] == "")
                {
                    _Modul1.Instance.Kont[1] = "    ";
                }
                DataModul.NB_DgbTable.AddNew();
                DataModul.NB_DgbTable.Fields["Name"].Value = M_Namen.Trim();
                if (_0024STATIC_0024Partles_002420118_0024VName.Length > 23)
                {
                    _0024STATIC_0024Partles_002420118_0024VName = _0024STATIC_0024Partles_002420118_0024VName.Left(23);
                }
                DataModul.NB_DgbTable.Fields["Vornam"].Value = _0024STATIC_0024Partles_002420118_0024VName.Trim() + "* " + _Modul1.Instance.Kont[1].Left(4);
                DataModul.NB_DgbTable.Fields["Geb"].Value = " ";
                DataModul.NB_DgbTable.Fields["Ind"].Value = _Modul1.Instance.Ind1.Trim();
                DataModul.NB_DgbTable.Update();
                Datschreib();
                _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                {
                    _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                }
                var aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                if (_Modul1.Instance.UbgT.Trim() == "")
                {
                    continue;
                }
                List1[4].Items.Clear();
                byte b2;
                if (_Modul1.Instance.UbgT.Length > 10)
                {
                    byte b = (byte)Math.Round(_Modul1.Instance.UbgT.Length / 10.0);
                    b2 = 1;
                    while (unchecked(b2 <= (uint)b))
                    {
                        _Modul1.Instance.FamInArb = _Modul1.Instance.UbgT.Left(10).AsInt();
                        _Modul1.Instance.UbgT = Strings.Mid(_Modul1.Instance.UbgT, 11, _Modul1.Instance.UbgT.Length);
                        short num2 = 502;
                        do
                        {
                            DataModul.DB_EventTable.Seek("=", num2, _Modul1.Instance.FamInArb.AsString(), "0");
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                _Modul1.Instance.sDatu = "        ";
                                num2 = (short)unchecked(num2 + 1);
                                continue;
                            }
                            _Modul1.Instance.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                            break;
                        }
                        while (num2 <= 503);
                        List1[4].Items.Add(_Modul1.Instance.sDatu + _Modul1.Instance.FamInArb.AsString());
                        b2 = (byte)unchecked((uint)(b2 + 1));
                    }
                }
                else
                {
                    List1[4].Items.Add("        " + _Modul1.Instance.UbgT);
                }
                byte b3 = (byte)(List1[4].Items.Count - 1);
                b2 = 0;
                while (unchecked(b2 <= (uint)b3))
                {
                    _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1[4].Items[b2].AsString(), 9, 10)));
                    Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                    Anz[0].SelectionAlignment = HorizontalAlignment.Left;
                    leerweg();
                    if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
                    {
                        Anz[0].SelectedText = "\n";
                    }
                    Hei = false;
                    Heidat();
                    if (!Hei)
                    {
                        Anz[0].SelectedText = _Modul1.Instance.DTxt[13];
                    }
                    Anz[0].SelectedText = " ";
                    if (DataModul.Link.GetFamPerson(_Modul1.Instance.FamInArb, kennz, out int iPers))
                    {
                        _Modul1.Instance.PersInArb = iPers;
                        Anz[0].SelectedText = "mit ";
                        EPerles();
                    }
                    else
                    {
                        Anz[0].SelectedText = _Modul1.Instance.IText[58];
                    }
                    Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                    _Modul1.Instance.Famles();
                    _0024STATIC_0024Partles_002420118_0024K = 1;
                    while (_Modul1.Instance.Family.Kind[_0024STATIC_0024Partles_002420118_0024K] != 0)
                    {
                        _0024STATIC_0024Partles_002420118_0024K = (byte)unchecked((uint)(_0024STATIC_0024Partles_002420118_0024K + 1));
                        if (unchecked(_0024STATIC_0024Partles_002420118_0024K) > 99u)
                        {
                            break;
                        }
                    }
                    if (_0024STATIC_0024Partles_002420118_0024K > 0)
                    {
                        leerweg();
                        if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
                        {
                            Anz[0].SelectedText = "\n";
                        }
                    }
                    if (_0024STATIC_0024Partles_002420118_0024K == 1)
                    {
                        Anz[0].SelectedText = "---- keine Kinder ----\n";
                    }
                    if (_0024STATIC_0024Partles_002420118_0024K > 2)
                    {
                        Anz[0].SelectedText = "----" + unchecked(_0024STATIC_0024Partles_002420118_0024K - 1).AsString() + " Kinder ---- \n";
                    }
                    if (_0024STATIC_0024Partles_002420118_0024K == 2)
                    {
                        Anz[0].SelectedText = "----" + unchecked(_0024STATIC_0024Partles_002420118_0024K - 1).AsString() + " Kind ---- \n";
                    }
                    Anz[0].SelectionAlignment = HorizontalAlignment.Left;
                    b2 = (byte)unchecked((uint)(b2 + 1));
                }
            }
            leerweg();
        }
    }

    public void Weitehen()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int persInArb = default;
        int famInArb = default;
        ELinkKennz eLKennz2 = default;
        byte b2 = default;
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
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 2181:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0713;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 3022)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0713;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_0717;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            persInArb = _Modul1.Instance.PersInArb;
                            famInArb = _Modul1.Instance.FamInArb;
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                            {
                                _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                                eLKennz2 = ELinkKennz.lkFather;
                            }
                            else
                            {
                                _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                eLKennz2 = ELinkKennz.lkMother;
                            }
                            List1[6].Items.Clear();
                            var aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                            if (aiFams.Count > 1)
                            {
                                _0024STATIC_0024Weitehen_00242001_0024a = (byte)Math.Round(_Modul1.Instance.UbgT.Length / 10.0);
                                b2 = _0024STATIC_0024Weitehen_00242001_0024a;
                                _WeitEhen_I4 = 1;
                                goto IL_0347;
                            }
                            goto IL_063d;
                        IL_0142: // <========== 3
                            num = 22;
                            DataModul.DB_EventTable.Seek("=", _0024STATIC_0024Weitehen_00242001_0024D, _Modul1.Instance.FamInArb.AsString(), "0");
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                _Modul1.Instance.sDatu = "        ";
                                _0024STATIC_0024Weitehen_00242001_0024D = (short)unchecked(_0024STATIC_0024Weitehen_00242001_0024D + 1);
                                if (_0024STATIC_0024Weitehen_00242001_0024D <= 505)
                                {
                                    goto IL_0142;
                                }

                            }
                            else
                            {
                                _Modul1.Instance.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                            }
                            goto IL_0230;
                        IL_0230: // <========== 3
                            num = 31;
                            if (_Modul1.Instance.sDatu.AsDouble() == 0.0)
                            {
                                DataModul.DB_EventTable.Seek("=", 601, _Modul1.Instance.FamInArb.AsString(), "0");
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    _Modul1.Instance.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);

                                }
                            }
                            goto IL_0300;
                        IL_0300: // <========== 3
                            num = 37;
                            List1[6].Items.Add(_Modul1.Instance.sDatu + _Modul1.Instance.FamInArb.AsString());
                            _WeitEhen_I4 = (byte)unchecked((uint)(_WeitEhen_I4 + 1));
                            goto IL_0347;
                        IL_0347:
                            if (unchecked(_WeitEhen_I4 <= (uint)b2))
                            {
                                if (_Modul1.Instance.UbgT.Length != 0)
                                {
                                    _Modul1.Instance.FamInArb = _Modul1.Instance.UbgT.Left(10).AsInt();
                                    _Modul1.Instance.UbgT = Strings.Mid(_Modul1.Instance.UbgT, 11, _Modul1.Instance.UbgT.Length);
                                    _0024STATIC_0024Weitehen_00242001_0024D = 502;
                                    goto IL_0142;
                                }
                            }
                            _Weitehen_Fa = 0;
                            byte b3 = (byte)(List1[6].Items.Count - 1);
                            _WeitEhen_I4 = 0;
                            while (unchecked(_WeitEhen_I4 <= (uint)b3))
                            {
                                _Weitehen_Fa++;
                                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1[6].Items[_WeitEhen_I4].AsString(), 9, 10)));
                                if (_Modul1.Instance.FamInArb != famInArb)
                                {
                                    if (_Modul1.Instance.eLKennz.AsDouble() == 1.0)
                                    {
                                        Anz[0].SelectedText = " (Er in" + _Weitehen_Fa.AsString() + ". Ehe";
                                    }
                                    if (_Modul1.Instance.eLKennz.AsDouble() == 2.0)
                                    {
                                        Anz[0].SelectedText = " (Sie in" + _Weitehen_Fa.AsString() + ". Ehe";
                                    }
                                    Heidat();
                                    DataModul.NB_FamilyTable.AddNew();
                                    DataModul.NB_FamilyTable.Fields[IndexFields.Fam].Value = _Modul1.Instance.FamInArb;
                                    DataModul.NB_FamilyTable.Update();
                                    Anz[0].SelectedText = " mit ";
                                    if (DataModul.Link.GetFamPerson(_Modul1.Instance.FamInArb, eLKennz2, out int iPers))
                                    {
                                        _Modul1.Instance.PersInArb = iPers;
                                        LSchalt = true;
                                        EPerles();
                                        LSchalt = false;
                                    }
                                    else
                                        Anz[0].SelectedText = "unbekannt.";
                                    Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                    Anz[0].SelectedText = ")";
                                }
                                lErl = 211;
                                _WeitEhen_I4 = (byte)unchecked((uint)(_WeitEhen_I4 + 1));
                            }
                            goto IL_063d;
                        IL_063d: // <========== 3
                            num = 73;
                            _Modul1.Instance.PersInArb = persInArb;
                            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            if (!(DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F"))
                            {
                            }
                            else
                            {
                                _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                            }
                            goto end_IL_0000_2;
                        IL_0713:
                            num4 = unchecked(num2 + 1);
                            goto IL_0717;
                        IL_0717:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 22:
                                    goto IL_0142;
                                case 28:
                                case 31:
                                    goto IL_0230;
                                case 35:
                                case 36:
                                case 37:
                                    goto IL_0300;
                                case 72:
                                case 73:
                                    goto IL_063d;
                                case 78:
                                case 79:
                                case 88:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 2181;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 3
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    private void Button1_Click(object sender, EventArgs e)
    {
        Ri = 0;
        while (!Anz[Ri].Visible)
        {
            checked
            {
                Ri = (byte)unchecked((uint)(Ri + 1));
            }
            if (Ri > 2u)
            {
                break;
            }
        }
        Anz[Ri].SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
        Anz[Ri].LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
        Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Temp\\Text2.RTF", AppWinStyle.MaximizedFocus);
    }

    private void Button2_Click(object sender, EventArgs e)
    {
        MyProject.Forms.Hinter.CommonDialog1Save.Filter = "Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF";
        Ri = 0;
        while (!Anz[Ri].Visible)
        {
            checked
            {
                Ri = (byte)unchecked((uint)(Ri + 1));
            }
            if (Ri > 2u)
            {
                break;
            }
        }
        MyProject.Forms.Hinter.CommonDialog1Save.InitialDirectory = _Modul1.Instance.GenFreeDir + "list\\";
        MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex = _Modul1.Instance.PrintDat.Flagsch;
        MyProject.Forms.Hinter.CommonDialog1Save.ShowDialog();
        if (MyProject.Forms.Hinter.CommonDialog1Save.FileName != "")
        {
            switch (MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex)
            {
                case 1:
                    Anz[Ri].SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);
                    break;
                case 2:
                    Anz[Ri].SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.RichText);
                    break;
            }
        }
    }

    private void Button3_Click(object sender, EventArgs e)
    {
        Anz[1].Visible = false;
        Anz[0].Visible = true;
    }

    private void Button4_Click(object sender, EventArgs e)
    {
        Anz[1].Text = "";
        Anz[0].Visible = false;
        Anz[1].Top = 0;
        Anz[1].Left = 0;
        Anz[1].Font = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
        Anz[1].Visible = true;
        Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
        DataModul.DSB_QuellIdxTable.Index = "Quelle";
        Anz[1].SelectedText = "Quellenverzeichnis\n";
        Anz[1].SelectionAlignment = HorizontalAlignment.Left;
        DataModul.DSB_QuellIdxTable.MoveFirst();
        int recordCount = DataModul.DSB_QuellIdxTable.RecordCount;
        for (int i = 1; i <= recordCount; i = checked(i + 1))
        {
            int num = DataModul.DSB_QuellIdxTable.Fields["Nr"].AsInt();
            if (num != _Modul1.Instance.AltNr)
            {
                Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                _Modul1.Instance.AltNr = num;
                DataModul.DB_QuTable.Index = "NR";
                DataModul.DB_QuTable.Seek("=", num);
                Anz[1].SelectionIndent = 4;
                Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                Anz[1].SelectedText = DataModul.DB_QuTable.Fields[QuFields._4].AsString() + '\n';
                Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                Anz[1].SelectionIndent = 40;
                if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._2].AsString().Trim(), "", TextCompare: false) != 0)
                {
                    Anz[1].SelectedText = ("Titel: " + DataModul.DB_QuTable.Fields[QuFields._2].Value + '\n').AsString();
                }
                if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._5].AsString().Trim(), "", TextCompare: false) != 0)
                {
                    Anz[1].SelectedText = ("Autor: " + DataModul.DB_QuTable.Fields[QuFields._5].Value + '\n').AsString();
                }
                _Modul1.Instance.UbgT = Module2.Repoles(DataModul.DB_QuTable.Fields[QuFields._1].AsInt());
                if (_Modul1.Instance.UbgT.Trim() != "")
                {
                    Anz[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                    Anz[1].SelectedText = _Modul1.Instance.UbgT;
                }
                _Modul1.Instance.UbgT = "";
                if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._7].AsString().Trim(), "", TextCompare: false) != 0)
                {
                    Anz[1].SelectedText = ("Herausgeber: " + DataModul.DB_QuTable.Fields[QuFields._7].Value + '\n').AsString();
                }
                if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._8].AsString().Trim(), "", TextCompare: false) != 0)
                {
                    Anz[1].SelectedText = ("Erscheinungsort: " + DataModul.DB_QuTable.Fields[QuFields._8].Value + '\n').AsString();
                }
                if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._9].AsString().Trim(), "", TextCompare: false) != 0)
                {
                    Anz[1].SelectedText = ("Erscheinungsdatum: " + DataModul.DB_QuTable.Fields[QuFields._9].Value + '\n').AsString();
                }
                if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._10].AsString().Trim(), "", TextCompare: false) != 0)
                {
                    Anz[1].SelectedText = ("in: " + DataModul.DB_QuTable.Fields[QuFields._10].Value + '\n').AsString();
                }
                if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._11].AsString().Trim(), "", TextCompare: false) != 0)
                {
                    Anz[1].SelectedText = ("Jahrgang: " + DataModul.DB_QuTable.Fields[QuFields._11].Value + " ").AsString();
                }
                if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._12].AsString().Trim(), "", TextCompare: false) != 0)
                {
                    Anz[1].SelectedText = ("Nr.: " + DataModul.DB_QuTable.Fields[QuFields._12].AsString()).AsString();
                }
                if ((Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._11].AsString().Trim(), "", TextCompare: false) != 0) | (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._12].AsString().Trim(), "", TextCompare: false) != 0))
                {
                    Anz[1].SelectedText = "\n";
                }
                if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._13].AsString().Trim(), "", TextCompare: false) != 0)
                {
                    Anz[1].SelectionIndent = 80;
                    Anz[1].SelectedText = ("Bemerkungen: " + DataModul.DB_QuTable.Fields[QuFields._13].Value + '\n').AsString();
                }
            }
            DataModul.DSB_QuellIdxTable.MoveNext();
        }
        Anz[1].SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
        Anz[1].LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
    }

    private void Button5_Click(object sender, EventArgs e)
    {
        Anz[0].Visible = false;
        Anz[1].Visible = false;
        Frame3.Visible = true;
    }

    private void Button6_Click(object sender, EventArgs e)
    {
        Anz[1].Text = "";
        Anz[0].Visible = false;
        Anz[1].Visible = false;
        Anz[2].Visible = false;
        Frame2.Visible = true;
    }

    private void Button7_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_0109
        int try0000_dispatch = -1;
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
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0007;
                    case 376:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_010c;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_00c1;
                        }
                    IL_00c1:
                        num = 18;
                        if (Information.Err().Number != 91)
                        {
                            break;
                        }
                        goto IL_00d2;
                    IL_00d2:
                        num = 19;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_010c;
                    IL_00b3:
                        num = 16;
                        _Modul1.Instance.GED = false;
                        goto end_IL_0000_2;
                    IL_010c:
                        num4 = num2 + 1;
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_0007;
                            case 3:
                                goto IL_000f;
                            case 4:
                                goto IL_001c;
                            case 5:
                                goto IL_0028;
                            case 6:
                                goto IL_0034;
                            case 7:
                                goto IL_0040;
                            case 8:
                                goto IL_004c;
                            case 9:
                                goto IL_0058;
                            case 10:
                                goto IL_0065;
                            case 11:
                                goto IL_0072;
                            case 12:
                                goto IL_007f;
                            case 13:
                                goto IL_008c;
                            case 14:
                                goto IL_0099;
                            case 15:
                                goto IL_00a6;
                            case 16:
                                goto IL_00b3;
                            case 18:
                                goto IL_00c1;
                            case 19:
                                goto IL_00d2;
                            case 20:
                            case 22:
                                goto end_IL_0000_3;
                            default:
                                goto end_IL_0000;
                            case 17:
                            case 23:
                                goto end_IL_0000_2;
                        }
                        goto default;
                    IL_0007:
                        num = 2;
                        Close();
                        goto IL_000f;
                    IL_000f:
                        num = 3;
                        BiTable.Close();
                        goto IL_001c;
                    IL_001c:
                        num = 4;
                        DataModul.NB_PersonTable.Close();
                        goto IL_0028;
                    IL_0028:
                        num = 5;
                        DataModul.NB_FamilyTable.Close();
                        goto IL_0034;
                    IL_0034:
                        num = 6;
                        DataModul.NB_FrauTable.Close();
                        goto IL_0040;
                    IL_0040:
                        num = 7;
                        DataModul.NB_DgbTable.Close();
                        goto IL_004c;
                    IL_004c:
                        num = 8;
                        DataModul.DSB_OrtIdxTable.Close();
                        goto IL_0058;
                    IL_0058:
                        num = 9;
                        DataModul.NB.Close();
                        goto IL_0065;
                    IL_0065:
                        num = 10;
                        DataModul.DSB_SortTable.Close();
                        goto IL_0072;
                    IL_0072:
                        num = 11;
                        DataModul.DSB_NamIdxTable.Close();
                        goto IL_007f;
                    IL_007f:
                        num = 12;
                        DataModul.MandDB.Close();
                        goto IL_008c;
                    IL_008c:
                        num = 13;
                        DataModul.DOSB.Close();
                        goto IL_0099;
                    IL_0099:
                        num = 14;
                        DataModul.TempDB.Close();
                        goto IL_00a6;
                    IL_00a6:
                        num = 15;
                        DataModul.DSB.Close();
                        goto IL_00b3;
                    end_IL_0000_3:
                        break;
                }
                num = 22;
                Interaction.MsgBox(Information.Err().Number);
                break;
            end_IL_0000:;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 376;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void Button8_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_007f
        int try0000_dispatch = -1;
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
                switch (try0000_dispatch)
                {
                    default:
                        num = 1;
                        DataModul.MandDB.Close();
                        goto IL_000c;
                    case 186:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0082;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_0056;
                        }
                    IL_000c:
                        num = 2;
                        DataModul.DOSB.Close();
                        goto IL_0018;
                    IL_0018:
                        num = 3;
                        DataModul.TempDB.Close();
                        goto IL_0024;
                    IL_0024:
                        num = 4;
                        DataModul.DSB.Close();
                        goto IL_0030;
                    IL_0030:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0037;
                    IL_0037:
                        num = 6;
                        Interaction.Shell(_Modul1.Instance.GenFreeDir + "Gen_Plus.exe", AppWinStyle.NormalFocus);
                        ProjectData.EndApp();
                        goto IL_0056;
                    IL_0056:
                        num = 8;
                        if (Information.Err().Number != 5)
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_0065;
                    IL_0065:
                        num = 9;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_0082;
                    IL_0082:
                        num4 = num2 + 1;
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_000c;
                            case 3:
                                goto IL_0018;
                            case 4:
                                goto IL_0024;
                            case 5:
                                goto IL_0030;
                            case 6:
                                goto IL_0037;
                            case 7:
                            case 8:
                                goto IL_0056;
                            case 9:
                                goto IL_0065;
                            default:
                                goto end_IL_0000;
                            case 10:
                                goto end_IL_0000_2;
                        }
                        goto default;
                    end_IL_0000:
                        break;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 186;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    public void Bild(string BKennz, int Nr)
    {
        if (_Modul1.Instance.DAus[88] != "1")
        {
            return;
        }

        DataModul.DB_PictureTable.Index = "Perkenn  ";
        DataModul.DB_PictureTable.Seek("=", BKennz, Nr);
        while (!DataModul.DB_PictureTable.EOF && !DataModul.DB_PictureTable.NoMatch && !(DataModul.DB_PictureTable.Fields[PictureFields.ZuNr].AsInt() != Nr)
            && !(DataModul.DB_PictureTable.Fields[PictureFields.Kennz].AsString() != BKennz))
        {
            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
            {
                Anz[0].SelectedText = "\n";
            }
            string text = (!DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().StartsWith("#")) ? (DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString()
                + DataModul.DB_PictureTable.Fields[PictureFields.Datei].AsString())
                : Conversions.ToString(_Modul1.Instance.Verz
                + Strings.Mid(DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString(), 2, DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Length)
                + DataModul.DB_PictureTable.Fields[PictureFields.Datei].AsString());
            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
            {
                Anz[0].SelectedText = "\n";
            }
            Anz[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            Anz[0].SelectionColor = Color.Purple;
            _Modul1.Instance.UbgT1 = "Bild: " + text;
            if (_Modul1.Instance.DAus[70] == "0")
            {
                _Modul1.Instance.UbgT1 = _Modul1.Instance.Retweg(_Modul1.Instance.UbgT1);
            }
            Anz[0].SelectedText = _Modul1.Instance.UbgT1 + "\n";
            _Modul1.Instance.UbgT1 = "";
            DataModul.DB_PictureTable.MoveNext();
        }
    }

    public void Retweg2()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        byte b = default;
        int lErl = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0007;
                    case 694:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0216;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_01d1;
                        }
                    IL_0216:
                        num4 = num2 + 1;
                        goto IL_0219;
                    IL_01d1:
                        num = 32;
                        if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                        {
                            ProjectData.EndApp();
                        }
                        goto IL_01f6;
                    IL_01a6:
                        num = 25;
                        if (b == 1)
                        {
                            goto IL_01ad;
                        }
                        goto IL_01b7;
                    IL_01f6:
                        num = 35;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        num4 = num2;
                        goto IL_0219;
                    IL_01ad:
                        num = 26;
                        b = 0;
                        goto IL_0031;
                    IL_01b7:
                        num = 30;
                        Anz[0].SelectionCharOffset = 0;
                        goto end_IL_0000_2;
                    IL_0219:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_0007;
                            case 3:
                                goto IL_0025;
                            case 4:
                                goto IL_002d;
                            case 5:
                            case 27:
                                goto IL_0031;
                            case 6:
                                goto IL_0036;
                            case 8:
                            case 9:
                                goto IL_0056;
                            case 10:
                                goto IL_008f;
                            case 11:
                                goto IL_00b6;
                            case 12:
                                goto IL_00cb;
                            case 13:
                                goto IL_00e4;
                            case 14:
                            case 15:
                                goto IL_00e9;
                            case 17:
                            case 18:
                                goto IL_010a;
                            case 19:
                                goto IL_0143;
                            case 20:
                                goto IL_016a;
                            case 21:
                                goto IL_017f;
                            case 22:
                                goto IL_0198;
                            case 23:
                            case 24:
                                goto IL_019d;
                            case 25:
                                goto IL_01a6;
                            case 26:
                                goto IL_01ad;
                            case 28:
                            case 29:
                            case 30:
                                goto IL_01b7;
                            case 32:
                                goto IL_01d1;
                            case 33:
                            case 35:
                                goto IL_01f6;
                            default:
                                goto end_IL_0000;
                            case 7:
                            case 16:
                            case 31:
                            case 36:
                                goto end_IL_0000_2;
                        }
                        goto default;
                    IL_0007:
                        num = 2;
                        if (Anz[0].Text.Length > 0)
                        {
                            goto IL_0025;
                        }
                        goto IL_01b7;
                    IL_0025:
                        num = 3;
                        leerweg();
                        goto IL_002d;
                    IL_002d:
                        num = 4;
                        b = 0;
                        goto IL_0031;
                    IL_0031:
                        num = 5;
                        lErl = 1;
                        goto IL_0036;
                    IL_0036:
                        num = 6;
                        if (Anz[0].Text.Length == 0)
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_0056;
                    IL_0056:
                        num = 9;
                        if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == "\n")
                        {
                            goto IL_008f;
                        }
                        goto IL_00e9;
                    IL_008f:
                        num = 10;
                        Anz[0].SelectionStart = checked(Anz[0].SelectionStart - 1);
                        goto IL_00b6;
                    IL_00b6:
                        num = 11;
                        Anz[0].SelectionLength = 1;
                        goto IL_00cb;
                    IL_00cb:
                        num = 12;
                        Anz[0].SelectedText = "";
                        goto IL_00e4;
                    IL_00e4:
                        num = 13;
                        b = 1;
                        goto IL_00e9;
                    IL_00e9:
                        num = 15;
                        if (Anz[0].Text.Length == 0)
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_010a;
                    IL_010a:
                        num = 18;
                        if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == "\r")
                        {
                            goto IL_0143;
                        }
                        goto IL_019d;
                    IL_0143:
                        num = 19;
                        Anz[0].SelectionStart = checked(Anz[0].SelectionStart - 1);
                        goto IL_016a;
                    IL_016a:
                        num = 20;
                        Anz[0].SelectionLength = 1;
                        goto IL_017f;
                    IL_017f:
                        num = 21;
                        Anz[0].SelectedText = "";
                        goto IL_0198;
                    IL_0198:
                        num = 22;
                        b = 1;
                        goto IL_019d;
                    IL_019d:
                        num = 24;
                        leerweg();
                        goto IL_01a6;
                    end_IL_0000:
                        break;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 694;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
}
