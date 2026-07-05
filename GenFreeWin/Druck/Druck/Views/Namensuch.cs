using BaseLib.Helper;
using Druck.My;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.DB;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Compatibility.VB6;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Druck.Views;

[DesignerGenerated]
public partial class Namensuch : Form
{

    private static EventHandler IdleEvent;

    private string Per1;

    private string[] Kont2;

    private long ahn1;

    private long ahn2;

    private string KontU;

    private string Ds;

    private string WiS;

    private string V;

    private string LD;

    private string Job;

    private int Nr;

    private short EingCode;

    private int An;

    private short Struck;

    private ELinkKennz Kenn;

    private short BemSch;

    private short Num;

    private short ID;

    private string[] KontSP;

    private string[] KontSP1;

    private short LfNR;

    private float S;

    private int Fambehk;

    private int I1;

    private int A;

    private int Z;

    private int U;

    private int PersSp;

    private float Datsch;

    private string Namen;

    private string Kennzt;

    private int[] Vorn;

    private string[] Ruf;

    private string Datum4;

    private string Datum3;

    private IDatabase WB;

    private EEventArt Beruf;

    private string Datu;
    private string _M_LiText;

    public static event EventHandler Idle
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        add
        {
            IdleEvent = (EventHandler)Delegate.Combine(IdleEvent, value);
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        remove
        {
            IdleEvent = (EventHandler)Delegate.Remove(IdleEvent, value);
        }
    }

    [DebuggerNonUserCode]
    public Namensuch()
    {
        base.Load += Namensuch_Load;
        base.FormClosed += Namensuch_FormClosed;
        Kont2 = new string[21];
        KontSP = new string[102];
        KontSP1 = new string[102];
        Vorn = new int[16];
        Ruf = new string[16];
        this.Check1 = new ControlArray<CheckBox>();
        this.Check2 = new ControlArray<CheckBox>();
        this.Command1 = new ControlArray<Button>();
        this.Command2 = new ControlArray<Button>();
        this.Label1 = new ControlArray<Label>();
        this.Label5 = new ControlArray<Label>();
        this.Label6 = new ControlArray<Label>();
        this.Label7 = new ControlArray<Label>();
        this.Label8 = new ControlArray<Label>();
        this.Line1 = new ControlArray<Label>();
        this.Option1 = new ControlArray<RadioButton>();
        this.Anz = new Microsoft.VisualBasic.Compatibility.VB6.RichTextBoxArray(this.components);

        InitializeComponent();

        this.Command1.SetIndex(this._Command1_0, 0);
        this.Command1.SetIndex(this._Command1_1, 1);
        this.Command1.SetIndex(this._Command1_2, 2);
        this.Command1.SetIndex(this._Command1_3, 3);
        this.Command1.SetIndex(this._Command1_4, 4);
        this.Command1.SetIndex(this._Command1_5, 5);
        this.Command1.SetIndex(this._Command1_6, 6);
        this.Command1.SetIndex(this._Command1_7, 7);
        
        this.Command2.SetIndex(this._Command2_0, 0);
        this.Command2.SetIndex(this._Command2_1, 1);
        this.Command2.SetIndex(this._Command2_2, 2);
        
        this.Check1.SetIndex(this._Check1_4, 4);
        this.Check1.SetIndex(this._Check1_3, 3);
        this.Check1.SetIndex(this._Check1_2, 2);
        this.Check1.SetIndex(this._Check1_1, 1);
        this.Check1.SetIndex(this._Check1_0, 0);
        this.Anz.SetIndex(this._Anz_0, 0);
        this.Label6.SetIndex(this._Label6_7, 7);
        this.Label6.SetIndex(this._Label6_6, 6);
        this.Label6.SetIndex(this._Label6_5, 5);
        this.Label7.SetIndex(this._Label7_15, 15);
        this.Label7.SetIndex(this._Label7_14, 14);
        this.Label7.SetIndex(this._Label7_13, 13);
        this.Label7.SetIndex(this._Label7_12, 12);
        this.Label7.SetIndex(this._Label7_11, 11);
        this.Label7.SetIndex(this._Label7_10, 10);
        this.Label7.SetIndex(this._Label7_9, 9);
        this.Label7.SetIndex(this._Label7_8, 8);
        this.Label7.SetIndex(this._Label7_7, 7);
        this.Label7.SetIndex(this._Label7_6, 6);
        this.Label7.SetIndex(this._Label7_5, 5);
        this.Label7.SetIndex(this._Label7_4, 4);
        this.Label7.SetIndex(this._Label7_3, 3);
        this.Label7.SetIndex(this._Label7_2, 2);
        this.Label7.SetIndex(this._Label7_1, 1);
        this.Label7.SetIndex(this._Label7_0, 0);
        this.Line1.SetIndex(this._Line1_28, 28);
        this.Line1.SetIndex(this._Line1_23, 23);
        this.Label6.SetIndex(this._Label6_4, 4);
        this.Label6.SetIndex(this._Label6_3, 3);
        this.Label6.SetIndex(this._Label6_1, 1);
        this.Label6.SetIndex(this._Label6_2, 2);
        this.Line1.SetIndex(this._Line1_27, 27);
        this.Line1.SetIndex(this._Line1_26, 26);
        this.Line1.SetIndex(this._Line1_25, 25);
        this.Line1.SetIndex(this._Line1_24, 24);
        this.Line1.SetIndex(this._Line1_22, 22);
        this.Line1.SetIndex(this._Line1_21, 21);
        this.Line1.SetIndex(this._Line1_20, 20);
        this.Line1.SetIndex(this._Line1_19, 19);
        this.Line1.SetIndex(this._Line1_17, 17);
        this.Label6.SetIndex(this._Label6_0, 0);
        this.Line1.SetIndex(this._Line1_16, 16);
        this.Line1.SetIndex(this._Line1_15, 15);
        this.Line1.SetIndex(this._Line1_14, 14);
        this.Line1.SetIndex(this._Line1_13, 13);
        this.Line1.SetIndex(this._Line1_12, 12);
        this.Line1.SetIndex(this._Line1_11, 11);
        this.Line1.SetIndex(this._Line1_10, 10);
        this.Line1.SetIndex(this._Line1_9, 9);
        this.Line1.SetIndex(this._Line1_8, 8);
        this.Label5.SetIndex(this._Label5_14, 14);
        this.Label5.SetIndex(this._Label5_10, 10);
        this.Label5.SetIndex(this._Label5_15, 15);
        this.Line1.SetIndex(this._Line1_7, 7);
        this.Line1.SetIndex(this._Line1_6, 6);
        this.Line1.SetIndex(this._Line1_5, 5);
        this.Line1.SetIndex(this._Line1_4, 4);
        this.Line1.SetIndex(this._Line1_3, 3);
        this.Line1.SetIndex(this._Line1_2, 2);
        this.Line1.SetIndex(this._Line1_1, 1);
        this.Label5.SetIndex(this._Label5_8, 8);
        this.Label5.SetIndex(this._Label5_13, 13);
        this.Label5.SetIndex(this._Label5_7, 7);
        this.Label5.SetIndex(this._Label5_12, 12);
        this.Label5.SetIndex(this._Label5_9, 9);
        this.Label5.SetIndex(this._Label5_6, 6);
        this.Label5.SetIndex(this._Label5_5, 5);
        this.Label5.SetIndex(this._Label5_4, 4);
        this.Label5.SetIndex(this._Label5_3, 3);
        this.Label5.SetIndex(this._Label5_2, 2);
        this.Label5.SetIndex(this._Label5_11, 11);
        this.Label5.SetIndex(this._Label5_1, 1);
        this.Label5.SetIndex(this._Label5_0, 0);
        this.Line1.SetIndex(this._Line1_0, 0);
        this.Option1.SetIndex(this._Option1_7, 7);
        this.Option1.SetIndex(this._Option1_6, 6);
        this.Option1.SetIndex(this._Option1_5, 5);
        this.Option1.SetIndex(this._Option1_4, 4);
        this.Option1.SetIndex(this._Option1_3, 3);
        this.Option1.SetIndex(this._Option1_2, 2);
        this.Option1.SetIndex(this._Option1_1, 1);
        this.Option1.SetIndex(this._Option1_0, 0);
        this.Check2.SetIndex(this._Check2_0, 0);
        this.Check2.SetIndex(this._Check2_1, 1);
        this.Check2.SetIndex(this._Check2_2, 2);
        this.Check2.SetIndex(this._Check2_3, 3);
        this.Check2.SetIndex(this._Check2_5, 5);
        this.Check2.SetIndex(this._Check2_4, 4);
        this.Label8.SetIndex(this._Label8_7, 7);
        this.Label8.SetIndex(this._Label8_6, 6);
        this.Label8.SetIndex(this._Label8_5, 5);
        this.Label8.SetIndex(this._Label8_4, 4);
        this.Label8.SetIndex(this._Label8_3, 3);
        this.Label8.SetIndex(this._Label8_2, 2);
        this.Label8.SetIndex(this._Label8_1, 1);
        this.Label8.SetIndex(this._Label8_0, 0);
        this.Label1.SetIndex(this._Label1_1, 1);
        this.Label1.SetIndex(this._Label1_0, 0);

        Check2.AddCheckedChanged(Check2_CheckStateChanged);
        Check2.AddMouseUp(Check2_MouseUp);
        Check2.AddMouseDown(Check2_MouseDown);
        Command1.AddClick(Command1_Click);
        Command2.AddClick(Command2_Click);
        Label5.AddDoubleClick(Label5_DoubleClick);
        Label6.AddDoubleClick(Label6_DoubleClick);
        Option1.AddCheckedChangedRB(Option1_CheckedChanged);
    }


    private void Check2_CheckStateChanged(object eventSender, EventArgs eventArgs)
    {
        int Index = Check2.GetIndex((CheckBox)eventSender);
        if (Check2.GetIndex((CheckBox)eventSender) != 3)
        {
            if (Check2[2].Checked)
            {
                Check2[4].Visible = false;
                Check2[4].CheckState = CheckState.Unchecked;
                Check2[5].Visible = false;
                Check2[5].CheckState = CheckState.Unchecked;
                Check2[0].Visible = true;
                Check2[1].Visible = true;
            }
            else
            {
                Check2[0].Visible = false;
                Check2[0].CheckState = CheckState.Unchecked;
                Check2[1].Visible = false;
                Check2[1].CheckState = CheckState.Unchecked;
                Check2[4].Visible = true;
                Check2[5].Visible = true;
            }
            ComboBox1.Focus();
        }
    }

    private void Check2_MouseDown(object eventSender, MouseEventArgs eventArgs)
    {
        checked
        {
            short num = (short)unchecked((int)eventArgs.Button / 1048576);
            short num2 = (short)unchecked((int)Control.ModifierKeys / 65536);
            switch (Check2.GetIndex((CheckBox)eventSender))
            {
                case 0:
                    Check2[1].CheckState = CheckState.Unchecked;
                    Check2[0].CheckState = CheckState.Checked;
                    break;
                case 1:
                    Check2[0].CheckState = CheckState.Unchecked;
                    Check2[1].CheckState = CheckState.Checked;
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    Check2[5].CheckState = CheckState.Unchecked;
                    Check2[4].CheckState = CheckState.Checked;
                    break;
                case 5:
                    Check2[4].CheckState = CheckState.Unchecked;
                    Check2[5].CheckState = CheckState.Checked;
                    break;
            }
        }
    }

    private void Check3_CheckStateChanged(object eventSender, EventArgs eventArgs)
    {
        Check2[0].CheckState = CheckState.Unchecked;
        Check2[1].CheckState = CheckState.Unchecked;
        Check2[2].CheckState = CheckState.Unchecked;
        if (Check3.Checked)
        {
            Label3.Text = "Name,Vorname                        Datum JJJJ  Personennr.";
            Check2[0].Visible = false;
            Check2[1].Visible = false;
            Check2[2].Visible = false;
        }
        else
        {
            Label3.Text = "Name,Vorname                        Datum JJJJ  Personennr. Heirat       Partner";
            Check2[4].Visible = true;
            Check2[5].Visible = true;
            Check2[2].Visible = true;
        }
        if (ComboBox1.Text != "")
        {
            Command1[3].PerformClick();
        }
    }

    private void Combo1_KeyPress(object eventSender, KeyPressEventArgs eventArgs)
    {
        short num = checked((short)Strings.Asc(eventArgs.KeyChar));
        Interaction.MsgBox(num);
        if (num == 13)
        {
            Command1[3].PerformClick();
        }
        eventArgs.KeyChar = Strings.Chr(num);
        if (num != 0)
        {
        }
    }

    private void Command1_Click(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int Index = default;
        int num2 = default;
        int num3 = default;
        int num5 = default;
        string Value = default;
        string QuText = default;
        int lErl = default;
        int mann = default;
        int frau = default;
        string text2 = default;
        int FamSP = default;
        int Persp = default;
        short num6 = default;
        int num7 = default;
        int num8 = default;
        int num9 = default;
        int famInArb = default;
        string KText = default;
        string left = default;
        int num12 = default;
        int num13 = default;
        int num14 = default;
        int num15 = default;
        IList<int> aiFams;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    ListBox DL;
                    short A;
                    int AAA;
                    string LD;
                    float Idned;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            Index = Command1.GetIndex((Button)eventSender);
                            goto IL_0016;
                        case 27410:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 4:
                                        break;
                                    case 3:
                                        goto IL_5848;
                                    case 2:
                                        goto IL_595b;
                                    case 1:
                                        goto IL_5a14;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 5)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_5a14;
                                }
                                if (Information.Err().Number == 62)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_5a14;
                                }
                                if (Information.Err().Number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_5a14;
                                }
                                if (Information.Err().Number == 424)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_5a14;
                                }
                                if (Information.Err().Number == 3022)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_5a14;
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
                                goto IL_5a10;
                            }
                        end_IL_0000:
                            break;
                        IL_0016:
                            num = 2;
                            QuText = "";
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            ProjectData.ClearProjectError();
                            num3 = 3;
                            FileSystem.FileClose(6);
                            FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\List", OpenMode.Input);
                            ProjectData.ClearProjectError();
                            num3 = 4;
                            Anz[0].Text = "";
                            Command1[6].Visible = false;
                            Command1[5].Visible = false;
                            switch (Index)
                            {
                                case 0:
                                    break;
                                case 1:
                                    goto IL_01cd;
                                case 2:
                                    goto IL_24cd;
                                case 3:
                                    goto IL_5007;
                                case 5:
                                case 6:
                                    goto IL_5209;
                                case 7:
                                    goto IL_56c8;
                                default:
                                    goto end_IL_0000_2;
                            }
                            _Modul1.Instance.Ubg = 0;
                            if (Check2[3].Checked)
                            {
                                Hide();
                            }
                            if (Check2[3].CheckState == 0)
                            {
                                Check2[0].CheckState = CheckState.Unchecked;
                                Check2[1].CheckState = CheckState.Unchecked;
                                Check2[2].CheckState = CheckState.Unchecked;
                                Check2[4].CheckState = CheckState.Unchecked;
                                Check2[5].CheckState = CheckState.Unchecked;
                                Listleer();
                                ComboBox1.Text = "";
                                Close();
                            }
                            goto end_IL_0000_2;
                        IL_01cd:
                            num = 32;
                            FileSystem.FileClose(6);
                            FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\List", OpenMode.Input);
                            num5 = 0;
                            Value = 0.AsString();
                            while (!FileSystem.EOF(6))
                            {
                                FileSystem.Input(6, ref Value);
                                Check1[(short)num5].CheckState = unchecked((CheckState)checked((int)Math.Round(Value.AsDouble())));
                                num5++;
                            }
                            FileSystem.FileClose(6);
                            Struck = (short)Check1[4].CheckState;
                            Command1[6].Visible = true;
                            Anz[0].SelectionHangingIndent = 20;
                            Option1[3].Visible = false;
                            Option1[4].Visible = false;
                            Option1[6].Visible = false;
                            Frame1.Top = 0;
                            Frame1.Left = 0;
                            Frame1.Height = Height;
                            Frame1.Visible = true;
                            Frame1.Width = Width;
                            Anz[0].Top = 10;
                            Anz[0].Width = Frame1.Width - 40;
                            Anz[0].Left = 10;
                            Anz[0].Height = Command2[0].Top - 20;
                            Anz[0].RightMargin = Anz[0].Width - 20;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            ComboBox1.Visible = false;
                            Command1[3].Visible = false;
                            ComboBox1.Visible = false;
                            Command1[3].Visible = false;
                            _Modul1.Instance.Family.Mann = 0;
                            _Modul1.Instance.Family.Frau = 0;
                            Num = 0;
                            if (Check1[0].Checked)
                            {
                                Num = 6;
                            }
                            CheckBox1.CheckState = CheckState.Unchecked;
                            CheckBox2.CheckState = CheckState.Unchecked;
                            CheckBox3.CheckState = CheckState.Unchecked;
                            CheckBox4.CheckState = CheckState.Unchecked;
                            CheckBox5.CheckState = CheckState.Unchecked;
                            CheckBox6.CheckState = CheckState.Unchecked;
                            CheckBox7.CheckState = CheckState.Unchecked;
                            CheckBox8.CheckState = CheckState.Unchecked;
                            CheckBox9.CheckState = CheckState.Unchecked;
                            CheckBox10.CheckState = CheckState.Unchecked;
                            CheckBox11.CheckState = CheckState.Unchecked;
                            CheckBox12.CheckState = CheckState.Unchecked;
                            CheckBox13.CheckState = CheckState.Unchecked;
                            CheckBox14.CheckState = CheckState.Unchecked;
                            CheckBox15.CheckState = CheckState.Unchecked;
                            if (_Modul1.Instance.Aus[1] == "Y")
                            {
                                CheckBox1.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[2] == "Y")
                            {
                                CheckBox2.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[3] == "Y")
                            {
                                CheckBox3.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[4] == "Y")
                            {
                                CheckBox4.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[5] == "Y")
                            {
                                CheckBox5.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[6] == "Y")
                            {
                                CheckBox6.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[30] == "Y")
                            {
                                CheckBox7.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[31] == "Y")
                            {
                                CheckBox8.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[32] == "Y")
                            {
                                CheckBox9.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[33] == "Y")
                            {
                                CheckBox10.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[34] == "Y")
                            {
                                CheckBox11.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[35] == "Y")
                            {
                                CheckBox12.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[36] == "Y")
                            {
                                CheckBox13.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[37] == "Y")
                            {
                                CheckBox14.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[38] == "Y")
                            {
                                CheckBox15.CheckState = CheckState.Checked;
                            }
                            Frame2.Visible = true;
                            Command1[6].Focus();
                            if (An == 0)
                            {
                                goto end_IL_0000_2;
                            }
                            goto IL_0854;
                        IL_0854: // <========== 3
                            num = 135;
                            lErl = 77;
                            if (List7.Items.Count > 0)
                            {
                                List7.SelectedIndex = 0;
                                _Modul1.Instance.PersInArb = (int)Math.Round(List7.Text.AsDouble());
                                List7.Items.RemoveAt(0);
                                Label1[0].Text = _Modul1.Instance.PersInArb.AsString();
                            }
                            else
                            {
                                _Modul1.Instance.PersInArb = (int)Math.Round(Label1[0].Text.AsDouble());
                            }
                            goto IL_0919;
                        IL_0919: // <========== 3
                            num = 145;
                            Anz[0].SelectionFont = new Font("Arial", 16f, FontStyle.Bold);
                            Anz[0].SelectionAlignment = HorizontalAlignment.Center;
                            Anz[0].SelectedText = _Modul1.Instance.IText[61];
                            Anz[0].SelectionFont = new Font("Arial", 16f, FontStyle.Regular);
                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont[3].TrimEnd() + " ";
                            _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                            Namen = _Modul1.Instance.Kont[0].TrimEnd() + " ";
                            Anz[0].SelectionFont = new Font("Arial", 16f, FontStyle.Bold);
                            Anz[0].SelectedText = Namen;
                            Anz[0].SelectionFont = new Font("Arial", 13.01f, FontStyle.Regular);
                            if (Num == 6)
                            {
                                Anz[0].SelectedText = " <" + _Modul1.Instance.PersInArb.AsString().Trim() + ">";
                            }
                            Anz[0].SelectedText = "\n";
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            Anz[0].SelectedText = "Erstellt am " + DateTime.Today.AsString();
                            Anz[0].SelectedText = $" von {_Modul1.Instance.User.Name.Trim()} mit {_Modul1.Instance.AppName} aus Mandant: {_Modul1.Instance.Verz}\n";
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            Anz[0].SelectionAlignment = HorizontalAlignment.Left;
                            if (Check1[3].Checked)
                            {
                                DataModul.DB_PictureTable.Index = "Perkenn  ";
                                DataModul.DB_PictureTable.Seek("=", "P", _Modul1.Instance.PersInArb);
                                goto IL_0e34;
                            }
                            goto IL_0e4a;
                        IL_0dbb: // <========== 3
                            num = 180;
                            Anz[0].SelectedText = ("Bild: " + text2 + " " + DataModul.DB_PictureTable.Fields[PictureFields.Bem].Value + '\n').AsString();
                            DataModul.DB_PictureTable.MoveNext();
                            goto IL_0e34;
                        IL_0e34: // <========== 3
                            num = 169;
                            if (!DataModul.DB_PictureTable.EOF)
                            {
                                if (!DataModul.DB_PictureTable.NoMatch)
                                {
                                    if (!(DataModul.DB_PictureTable.Fields[PictureFields.ZuNr].AsInt() != _Modul1.Instance.PersInArb))
                                    {
                                        if (DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().StartsWith("#"))
                                        {
                                            text2 = Conversions.ToString(_Modul1.Instance.Verz + DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Left(DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Length) + DataModul.DB_PictureTable.Fields[PictureFields.Datei].Value);
                                        }
                                        else
                                        {
                                            text2 = DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString() + DataModul.DB_PictureTable.Fields[PictureFields.Datei].AsString();
                                        }
                                        goto IL_0dbb;
                                    }

                                }
                            }
                            goto IL_0e4a;
                        IL_0e4a: // <========== 4
                            num = 188;
                            Anz[0].SelectedText = "\n";
                            Anz[0].SelectionFont = new Font("Arial", 11f, FontStyle.Regular);
                            EPerles(Index, ref FamSP, ref Persp, Num);
                            string text4;
                            if (_Modul1.Instance.Family.Mann > 0)
                            {
                                _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                                _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                text4 = _Modul1.Instance.UbgT;
                            }
                            text4 = "";
                            if (_Modul1.Instance.Family.Frau > 0)
                            {
                                _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau;
                                _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                                aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                text4 += _Modul1.Instance.UbgT;
                            }
                            List3.Items.Clear();
                            int num10 = (int)Math.Round(text4.Length / 10.0);
                            num5 = 1;
                            while (num5 <= num10)
                            {
                                List3.Items.Add(text4.Left(10));
                                text4 = Strings.Mid(text4, 11, text4.Length);
                                num5++;
                            }
                            goto IL_0fdd;
                        IL_0fdd: // <========== 3
                            num = 209;
                            lErl = 12;
                            int num11 = List3.Items.Count - 2;
                            num5 = 0;
                            while (num5 <= num11)
                            {
                                if (List3.Items[num5] == List3.Items[num5 + 1])
                                {
                                    List3.Items.RemoveAt(num5 + 1);
                                    goto IL_0fdd;
                                }
                                num5++;
                            }
                            this.A = 0;
                            num5 = 1;
                            while (num5 <= 99)
                            {
                                _Modul1.Instance.Family.Kind[num5] = 0;
                                num5++;
                            }
                            num12 = List3.Items.Count - 1;
                            U = 0;
                            goto IL_129f;
                        IL_128a: // <========== 4
                            num = 243;
                            U++;
                            goto IL_129f;
                        IL_129f:
                            if (U <= num12)
                            {
                                _Modul1.Instance.FamInArb = (int)Math.Round(List3.Items[U].AsDouble());
                                num5 = 1;
                                foreach (var cLink in DataModul.Link.ReadAllFams(_Modul1.Instance.FamInArb, ELinkKennz.lkChild))
                                {
                                    this.A++;
                                    _Modul1.Instance.Family.Kind[this.A] = cLink.iPersNr;
                                    num5++;
                                }
                                goto IL_128a;
                            }
                            if (this.A > 1)
                            {
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                                if (U > 1)
                                {
                                    Anz[0].SelectedText = "\nGeschwister und Halbgeschwister:\n";
                                }
                                else
                                    Anz[0].SelectedText = "\n" + _Modul1.Instance.IText[60] + ":\n";
                                goto IL_134c;
                            }
                            goto IL_1852;
                        IL_134c: // <========== 3
                            num = 252;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            Kisort();
                            Anz[0].SelectionHangingIndent = 30;
                            num13 = List2.Items.Count - 1;
                            num5 = 0;
                            goto IL_1849;
                        IL_15d9: // <========== 3
                            num = 274;
                            _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont[0].Trim();
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            Anz[0].SelectedText = ", ";
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields["religi"].Value))
                            {
                                if (Operators.CompareString(DataModul.DB_PersonTable.Fields["religi"].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    _Modul1.Instance.Ubg = DataModul.DB_PersonTable.Fields["religi"].AsInt();
                                    LD = "";
                                    DataModul.Textlese(_Modul1.Instance.Ubg, out _Modul1.Instance.UbgT, out LD);
                                    if (_Modul1.Instance.UbgT.Trim() != "")
                                    {
                                        Anz[0].SelectedText = _Modul1.Instance.UbgT;
                                    }
                                }
                            }
                            goto IL_177d;
                        IL_177d: // <========== 3
                            num = 289;
                            Datsch = 1f;
                            Idned = 0f;
                            Datr1(ref Idned);
                            goto IL_183c;
                        IL_183c: // <========== 3
                            num = 297;
                            num5++;
                            goto IL_1849;
                        IL_1849:
                            if (num5 <= num13)
                            {
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                                _Modul1.Instance.PersInArb = Conversions.ToInteger(Strings.Mid(List2.Items[num5].AsString(), 9, 10));
                                if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                                {
                                    Anz[0].SelectedText = "\n";
                                }
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                                if (Struck == 1)
                                {
                                    Anz[0].SelectedText = "\n";
                                }
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                                if (_Modul1.Instance.PersInArb != Label1[0].Text.AsDouble())
                                {
                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                    if (Num == 6)
                                    {
                                        Anz[0].SelectedText = num5 + 1.AsString() + ". <" + _Modul1.Instance.PersInArb.AsString() + "> " + _Modul1.Instance.Kont[3].Trim();
                                    }
                                    else
                                    {
                                        Anz[0].SelectedText = num5 + 1.AsString() + ". " + _Modul1.Instance.Kont[3].Trim();
                                    }
                                    goto IL_15d9;
                                }
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                                Anz[0].SelectedText = num5 + 1.AsString() + _Modul1.Instance.IText[235] + "\n";
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                                goto IL_183c;
                            }
                            goto IL_1852;
                        IL_1852: // <========== 3
                            num = 299;
                            _Modul1.Instance.PersInArb = (int)Math.Round(Label1[0].Text.AsDouble());
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                            if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                            {
                                _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                            }
                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                            List8.Items.Clear();
                            num14 = (int)Math.Round(_Modul1.Instance.UbgT.Length / 10.0);
                            num5 = 1;
                            goto IL_1bbd;
                        IL_1977: // <========== 3
                            num = 311;
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", Z, _Modul1.Instance.FamInArb.AsString(), "0");
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                Datu = "        ";
                                goto IL_1a7d;
                            }
                            Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                            if (Z == 504)
                            {
                                goto IL_1a7d;
                            }
                            goto IL_1aa2;
                        IL_1a7d: // <========== 3
                            num = 322;
                            Z++;
                            if (Z <= 505)
                            {
                                goto IL_1977;
                            }
                            goto IL_1aa2;
                        IL_1aa2: // <========== 3
                            num = 323;
                            if (Datu == "        ")
                            {
                                DataModul.DB_EventTable.Seek("=", 601, _Modul1.Instance.FamInArb.AsString(), "0");
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);

                                }
                            }
                            goto IL_1b7e;
                        IL_1b7e: // <========== 3
                            num = 329;
                            List8.Items.Add(Datu + _Modul1.Instance.FamInArb.AsString());
                            num5++;
                            goto IL_1bbd;
                        IL_1bbd:
                            if (num5 <= num14)
                            {
                                _Modul1.Instance.FamInArb = _Modul1.Instance.UbgT.Left(10).AsInt();
                                _Modul1.Instance.UbgT = Strings.Mid(_Modul1.Instance.UbgT, 11, _Modul1.Instance.UbgT.Length);
                                Z = 500;
                                goto IL_1977;
                            }
                            Anz[0].SelectedText = "\n";
                            Anz[0].SelectionIndent = 0;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                            if (List8.Items.Count == 1)
                            {
                                Anz[0].SelectedText = "\nEigene Verbindung:";
                            }
                            if (List8.Items.Count > 1)
                            {
                                Anz[0].SelectedText = "\nEigene Verbindungen:";
                            }
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            Anz[0].SelectionIndent = 0;
                            num15 = List8.Items.Count - 1;
                            I1 = 0;
                            goto IL_244c;
                        IL_1de8:
                            num = 352;
                            Anz[0].SelectedText = I1 + 1.AsString() + ". ";
                            goto IL_1e1c;
                        IL_1e1c: // <========== 3
                            num = 354;
                            Heidat();
                            _Modul1.Instance.PersInArb = (int)Math.Round(Label1[0].Text.AsDouble());
                            string sPSex = DataModul.Person.GetSex(_Modul1.Instance.PersInArb);
                            var eLKennz = sPSex switch
                            {
                                "F" => ELinkKennz.lkFather,
                                "M" => ELinkKennz.lkMother,
                                _ => ELinkKennz.lkNone
                            };
                            if (DataModul.Link.GetFamPerson(_Modul1.Instance.FamInArb, eLKennz, out _Modul1.Instance.PersInArb))
                            {
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                                Anz[0].SelectedText = "\n";
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                                Anz[0].SelectionIndent = 0;
                                if (Num == 6)
                                {
                                    Anz[0].SelectedText = "Mit <" + _Modul1.Instance.PersInArb.AsString() + "> " + _Modul1.Instance.Kont[3].Trim();
                                }
                                else
                                    Anz[0].SelectedText = "Mit " + _Modul1.Instance.Kont[3].Trim();
                                goto IL_20c5;
                            }
                            Anz[0].SelectedText = "\n";
                            Anz[0].SelectionIndent = 0;
                            Anz[0].SelectedText = "Mit unbekanntem Partner\n";
                            goto IL_2371;
                        IL_20c5: // <========== 3
                            num = 377;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont[0];
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Italic);
                            if (_Modul1.Instance.Kont[4] != "")
                            {
                                Anz[0].SelectedText = " (" + _Modul1.Instance.Kont[4].TrimEnd() + ")";
                            }
                            else
                            {
                                Anz[0].SelectedText = "";
                            }
                            goto IL_21db;
                        IL_21db: // <========== 3
                            num = 387;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields["religi"].Value))
                            {
                                if (Operators.CompareString(DataModul.DB_PersonTable.Fields["religi"].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    _Modul1.Instance.Ubg = DataModul.DB_PersonTable.Fields["religi"].AsInt();
                                    LD = "";
                                    DataModul.Textlese(_Modul1.Instance.Ubg, out _Modul1.Instance.UbgT, out LD);
                                    if (_Modul1.Instance.UbgT.Trim() != "")
                                    {
                                        Anz[0].SelectedText = _Modul1.Instance.UbgT;
                                    }

                                }
                            }
                            goto IL_22ff;
                        IL_22ff: // <========== 3
                            num = 398;
                            Idned = 0f;
                            Datr1(ref Idned);
                            goto IL_2371;
                        IL_2371: // <========== 3
                            num = 405;
                            this.A = 0;
                            Fambehk = _Modul1.Instance.FamInArb;
                            KText = "Kinder:";
                            _Modul1.Instance.eLKennz = ELinkKennz.lkChild;
                            Kindles(ref KText);
                            _Modul1.Instance.FamInArb = Fambehk;
                            _Modul1.Instance.eLKennz = ELinkKennz.lkAdoptedChild;
                            KText = "Adoptivkinder:";
                            Kindles(ref KText);
                            Anz[0].SelectionIndent = 0;
                            Anz[0].SelectedText = "\n";
                            I1++;
                            goto IL_244c;
                        IL_244c:
                            if (I1 <= num15)
                            {
                                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List8.Items[I1].AsString(), 9, 10)));
                                if (_Modul1.Instance.FamInArb != 0)
                                {
                                    Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                                    if (List8.Items.Count > 1)
                                    {
                                        if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
                                        {
                                            Anz[0].SelectedText = "\n";
                                        }
                                        goto IL_1de8;
                                    }
                                    goto IL_1e1c;
                                }
                            }
                            if (List7.Items.Count > 0)
                            {
                                goto IL_0854;
                            }
                            Anz[0].SaveFile(_Modul1.Instance.GenFreeDir + "\\Temp\\Text2.RTF", RichTextBoxStreamType.RichText);
                            Anz[0].LoadFile(_Modul1.Instance.GenFreeDir + "\\Temp\\Text2.RTF", RichTextBoxStreamType.RichText);
                            goto end_IL_0000_2;
                        IL_24cd:
                            num = 424;
                            Option1[3].Visible = false;
                            Option1[4].Visible = false;
                            Option1[6].Visible = false;
                            Frame1.Top = 0;
                            Frame1.Left = 0;
                            Frame1.Height = Height;
                            Frame1.Visible = true;
                            Frame1.Width = Width;
                            Anz[0].Top = 10;
                            Anz[0].Width = Frame1.Width - 40;
                            Anz[0].Left = 10;
                            Anz[0].Height = Command2[0].Top - 20;
                            Anz[0].RightMargin = Anz[0].Width - 20;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            ComboBox1.Visible = false;
                            Command1[3].Visible = false;
                            Frame1.Visible = true;
                            Frame2.Visible = true;
                            Command1[5].Visible = true;
                            Command1[5].Focus();
                            FileSystem.FileClose(6);
                            FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\List", OpenMode.Input);
                            num5 = 0;
                            Value = 0.AsString();
                            while (!FileSystem.EOF(6))
                            {
                                FileSystem.Input(6, ref Value);
                                Check1[(short)num5].CheckState = unchecked((CheckState)checked((int)Math.Round(Value.AsDouble())));
                                num5++;
                            }
                            FileSystem.FileClose(99);
                            FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "\\Init\\Druck_ini.dat", OpenMode.Input);
                            num5 = 0;
                            while (!FileSystem.EOF(99))
                            {
                                num5++;
                                FileSystem.Input(99, ref _Modul1.Instance.Aus[num5]);
                            }
                            if (Check1[0].Checked)
                            {
                                Num = 6;
                            }
                            CheckBox1.CheckState = CheckState.Unchecked;
                            CheckBox2.CheckState = CheckState.Unchecked;
                            CheckBox3.CheckState = CheckState.Unchecked;
                            CheckBox4.CheckState = CheckState.Unchecked;
                            CheckBox5.CheckState = CheckState.Unchecked;
                            CheckBox6.CheckState = CheckState.Unchecked;
                            CheckBox7.CheckState = CheckState.Unchecked;
                            CheckBox8.CheckState = CheckState.Unchecked;
                            CheckBox9.CheckState = CheckState.Unchecked;
                            CheckBox10.CheckState = CheckState.Unchecked;
                            CheckBox11.CheckState = CheckState.Unchecked;
                            CheckBox12.CheckState = CheckState.Unchecked;
                            CheckBox13.CheckState = CheckState.Unchecked;
                            if (_Modul1.Instance.Aus[1] == "Y")
                            {
                                CheckBox1.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[2] == "Y")
                            {
                                CheckBox2.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[3] == "Y")
                            {
                                CheckBox3.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[4] == "Y")
                            {
                                CheckBox4.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[5] == "Y")
                            {
                                CheckBox5.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[6] == "Y")
                            {
                                CheckBox6.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[30] == "Y")
                            {
                                CheckBox7.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[31] == "Y")
                            {
                                CheckBox8.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[32] == "Y")
                            {
                                CheckBox9.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[33] == "Y")
                            {
                                CheckBox10.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[34] == "Y")
                            {
                                CheckBox11.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[35] == "Y")
                            {
                                CheckBox12.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[36] == "Y")
                            {
                                CheckBox13.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[37] == "Y")
                            {
                                CheckBox14.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[38] == "Y")
                            {
                                CheckBox15.CheckState = CheckState.Checked;
                            }
                            Frame2.Visible = true;
                            Command1[6].Focus();
                            FileSystem.FileClose(99);
                            if (_Modul1.Instance.Aus[24] != "1")
                            {
                                _Modul1.Instance.Aus[24] = "0";
                            }
                            Num = 0;
                            if (An == 0)
                            {
                                goto end_IL_0000_2;
                            }
                            Frame2.Visible = false;
                            if (Check1[0].Checked)
                            {
                                Num = 6;
                            }
                            Anz[0].Enabled = false;
                            Command2[0].Enabled = false;
                            Command2[1].Enabled = false;
                            Command2[2].Enabled = false;
                            Cursor = Cursors.WaitCursor;
                            if (List7.Items.Count > 0)
                            {
                                List7.SelectedIndex = 0;
                                _Modul1.Instance.FamInArb = (int)Math.Round(List7.Text.AsDouble());
                                List7.Items.RemoveAt(0);
                                Label1[1].Text = _Modul1.Instance.FamInArb.AsString();
                            }
                            else
                                _Modul1.Instance.FamInArb = (int)Math.Round(Label1[1].Text.AsDouble());
                            goto IL_2df9;
                        IL_2df9: // <========== 5
                            num = 551;
                            lErl = 66;
                            DataModul.DB_FamilyTable.Index = "Fam";
                            DataModul.DB_FamilyTable.Seek("=", _Modul1.Instance.FamInArb);
                            if (DataModul.DB_FamilyTable.NoMatch)
                            {
                                if (List7.Items.Count > 0)
                                {
                                    List7.SelectedIndex = 0;
                                    _Modul1.Instance.FamInArb = (int)Math.Round(List7.Text.AsDouble());
                                    List7.Items.RemoveAt(0);
                                    Label1[1].Text = _Modul1.Instance.FamInArb.AsString();
                                    Anz[0].SelectedText = "\n\n";
                                    goto IL_2df9;
                                }
                                goto end_IL_0000_2;
                            }
                            _Modul1.Instance.Famles();
                            Anz[0].SelectionFont = new Font("Arial", 16f, FontStyle.Bold);
                            Anz[0].SelectedText = "Familienblatt für: ";
                            Anz[0].SelectionFont = new Font("Arial", 16f, FontStyle.Regular);
                            if ((_Modul1.Instance.Schalt == 4) & (_Modul1.Instance.Family.Mann > 0))
                            {
                                Label1[0].Text = _Modul1.Instance.Family.Mann.AsString();
                            }
                            _Modul1.Instance.PersInArb = (int)Math.Round(Label1[0].Text.AsDouble());
                            if (_Modul1.Instance.Schalt == 4)
                            {
                                _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                            }
                            if (MyProject.Forms.Familie.Visible)
                            {
                                _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                            }
                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                            Namen = _Modul1.Instance.Kont[0].TrimEnd() + " ";
                            if (Namen.Trim() == "")
                            {
                                Namen = " unbekannt";
                            }
                            Anz[0].SelectionFont = new Font("Arial", 16f, FontStyle.Bold);
                            Anz[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " " + Namen.Trim();
                            Anz[0].SelectionFont = new Font("Arial", 16f, FontStyle.Regular);
                            Anz[0].SelectedText = " und ";
                            mann = _Modul1.Instance.Family.Mann;
                            frau = _Modul1.Instance.Family.Frau;
                            _Modul1.Instance.Family.Mann = 0;
                            _Modul1.Instance.Family.Frau = 0;
                            if (mann == Label1[0].Text.AsDouble())
                            {
                                _Modul1.Instance.PersInArb = frau;
                            }
                            else
                                _Modul1.Instance.PersInArb = mann;
                            goto IL_3204;
                        IL_3204: // <========== 3
                            num = 601;
                            _Modul1.Instance.FamInArb = (int)Math.Round(Label1[1].Text.AsDouble());
                            if (MyProject.Forms.Familie.Visible)
                            {
                                _Modul1.Instance.PersInArb = frau;
                            }
                            if (_Modul1.Instance.PersInArb > 0)
                            {
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                                Namen = _Modul1.Instance.Kont[0].TrimEnd() + " ";
                            }
                            else
                            {
                                _Modul1.Instance.Kont[3] = "";
                                Namen = _Modul1.Instance.IText[28];
                            }
                            goto IL_32ce;
                        IL_32ce: // <========== 3
                            num = 614;
                            Anz[0].SelectionFont = new Font("Arial", 16f, FontStyle.Bold);
                            Anz[0].SelectionAlignment = HorizontalAlignment.Center;
                            Anz[0].SelectedText = _Modul1.Instance.Kont[3] + " " + Namen.Trim();
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            if (Num == 6)
                            {
                                Anz[0].SelectedText = "[" + _Modul1.Instance.FamInArb.AsString() + "]\n";
                            }
                            else
                                Anz[0].SelectedText = "\n";
                            goto IL_33d7;
                        IL_33d7: // <========== 3
                            num = 624;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            Anz[0].SelectedText = "Erstellt am " + DateTime.Today.AsString();
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            Anz[0].SelectedText = $" von {_Modul1.Instance.User.Name.Trim()} mit {_Modul1.Instance.AppName} aus Mandant: {_Modul1.Instance.Verz}\n";
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            Anz[0].SelectionAlignment = HorizontalAlignment.Left;
                            if (Check1[3].Checked)
                            {
                                DataModul.DB_PictureTable.Index = "Perkenn  ";
                                DataModul.DB_PictureTable.Seek("=", "F", _Modul1.Instance.FamInArb);
                                goto IL_379a;
                            }
                            goto IL_37b0;
                        IL_36d3: // <========== 3
                            num = 645;
                            Anz[0].SelectedText = ("\nBild: " + text2 + " " + DataModul.DB_PictureTable.Fields[PictureFields.Bem].AsString());
                            if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            goto IL_3789;
                        IL_3789:
                            num = 653;
                            DataModul.DB_PictureTable.MoveNext();
                            goto IL_379a;
                        IL_379a: // <========== 3
                            num = 634;
                            if (!DataModul.DB_PictureTable.EOF && !DataModul.DB_PictureTable.NoMatch && !(DataModul.DB_PictureTable.Fields[PictureFields.ZuNr].AsInt() != _Modul1.Instance.FamInArb))
                            {
                                if (DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Left(1) == "#")
                                {
                                    text2 = Conversions.ToString(_Modul1.Instance.Verz + DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Left(DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Length) + DataModul.DB_PictureTable.Fields[PictureFields.Datei].Value);
                                }
                                else
                                {
                                    text2 = DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString() + DataModul.DB_PictureTable.Fields[PictureFields.Datei].AsString();
                                }
                                goto IL_36d3;
                            }
                            goto IL_37b0;
                        IL_37b0: // <========== 4
                            num = 656;
                            if (Strings.Mid(Anz[0].SelectedText, Anz[0].SelectionStart - 2, 2) != "\n\n")
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                            Anz[0].SelectedText = "Vater:\n";
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            if (mann > 0)
                            {
                                _Modul1.Instance.PersInArb = mann;
                                EPerles(Index, ref FamSP, ref Persp, Num);
                                Anz[0].SelectedText = "\n";
                            }
                            else
                            {
                                Anz[0].SelectedText = _Modul1.Instance.IText[28] + ".\n";
                            }
                            goto IL_38fc;
                        IL_38fc: // <========== 3
                            num = 670;
                            _Modul1.Instance.FamInArb = (int)Math.Round(Label1[1].Text.AsDouble());
                            Anz[0].SelectionIndent = 0;
                            S = 0f;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            Heidat();
                            S = 1f;
                            num5 = 1;
                            string text3;
                            foreach (var cLink in DataModul.Link.ReadAllFams(_Modul1.Instance.FamInArb, ELinkKennz.lkWitnOfEngage))
                            {
                                text3 = Strings.Right("          " + cLink.iPersNr.AsString(), 10);
                                Per1 += text3;
                                num5++;
                            }
                            goto IL_3b79;
                        IL_3b79: // <========== 4
                            num = 701;
                            if (num5 == 2)
                            {
                                Anz[0].SelectedText = "\nVerlobungszeuge: ";
                            }
                            if (num5 > 2)
                            {
                                Anz[0].SelectedText = "\nVerlobungseugen: ";
                            }
                            num6 = (short)Math.Round(Per1.Length / 10.0);
                            num7 = num6;
                            num5 = 1;
                            goto IL_3cef;
                        IL_3ce2: // <========== 3
                            num = 719;
                            num5++;
                            goto IL_3cef;
                        IL_3cef:
                            if (num5 <= num7)
                            {
                                _Modul1.Instance.PersInArb = (int)Math.Round(Per1.Left(10).AsDouble());
                                Per1 = Strings.Mid(Per1, 11, Per1.Length);
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                Anz[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                if (num5 < num6)
                                {
                                    Anz[0].SelectedText = "; ";
                                }
                                else
                                    Anz[0].SelectedText = ".";
                                goto IL_3ce2;
                            }
                            num5 = 1;
                            foreach (var cLink in DataModul.Link.ReadAllFams(_Modul1.Instance.FamInArb, ELinkKennz.lkMarrWitness))
                            {
                                text3 = Strings.Right("          " + cLink.iPersNr.AsString(), 10);
                                Per1 += text3;
                                num5++;
                            }
                            goto IL_3eab;
                        IL_3eab: // <========== 3
                            num = 739;
                            if (num5 == 2)
                            {
                                Anz[0].SelectedText = "\nTrauzeuge: ";
                            }
                            if (num5 > 2)
                            {
                                Anz[0].SelectedText = "\nTrauzeugen: ";
                            }
                            num6 = (short)Math.Round(Per1.Length / 10.0);
                            num8 = num6;
                            num5 = 1;
                            goto IL_404a;
                        IL_403d: // <========== 3
                            num = 757;
                            num5++;
                            goto IL_404a;
                        IL_404a:
                            if (num5 <= num8)
                            {
                                _Modul1.Instance.PersInArb = (int)Math.Round(Per1.Left(10).AsDouble());
                                Per1 = Strings.Mid(Per1, 11, Per1.Length);
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                Anz[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                if ((num5 < num6) | (DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].AsString().Length > 1))
                                {
                                    Anz[0].SelectedText = "; ";
                                }
                                else
                                {
                                    Anz[0].SelectedText = ".";
                                }
                                goto IL_403d;
                            }
                            if (!Information.IsDBNull(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].Value))
                            {
                                if (DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].AsString().Length > 1)
                                {
                                    if (num5 < 2)
                                    {
                                        Anz[0].SelectedText = "\nTrauzeugen:";
                                    }
                                    goto IL_40d8;
                                }
                            }
                            goto IL_4118;
                        IL_40d8:
                            num = 763;
                            Anz[0].SelectedText = (DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].Value + ".").AsString();
                            goto IL_4118;
                        IL_4118: // <========== 3
                            num = 766;
                            num5 = 1;
                            foreach (var cLink in DataModul.Link.ReadAllFams(_Modul1.Instance.FamInArb, ELinkKennz.lkWitnOfMarr))
                            {
                                text3 = Strings.Right("          " + cLink.iPersNr.AsString(), 10);
                                Per1 += text3;
                                num5++;
                            }
                            goto IL_42e3;
                        IL_42e3: // <========== 4
                            num = 788;
                            if (num5 == 2)
                            {
                                Anz[0].SelectedText = "\nKirchl. Trauzeuge: ";
                            }
                            if (num5 > 2)
                            {
                                Anz[0].SelectedText = "\nKirchl. Trauzeugen: ";
                            }
                            num6 = (short)Math.Round(Per1.Length / 10.0);
                            num9 = num6;
                            num5 = 1;
                            goto IL_4459;
                        IL_4427: // <========== 3
                            num = 805;
                            Per1 = Strings.Mid(Per1, 11, Per1.Length);
                            num5++;
                            goto IL_4459;
                        IL_4459:
                            if (num5 <= num9)
                            {
                                _Modul1.Instance.PersInArb = (int)Math.Round(Per1.Left(10).AsDouble());
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                Anz[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                if (num5 < num6)
                                {
                                    Anz[0].SelectedText = "; ";
                                }
                                else
                                {
                                    Anz[0].SelectedText = ".";
                                }
                                goto IL_4427;
                            }
                            famInArb = _Modul1.Instance.FamInArb;
                            Anz[0].SelectedText = "\nmit";
                            if (Struck == 1)
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            Anz[0].SelectionIndent = 0;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                            Anz[0].SelectedText = "\nMutter:\n";
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            if (frau > 0)
                            {
                                _Modul1.Instance.PersInArb = frau;
                                EPerles(Index, ref FamSP, ref Persp, Num);
                            }
                            else
                            {
                                Anz[0].SelectedText = _Modul1.Instance.IText[28];
                            }
                            goto IL_459c;
                        IL_459c: // <========== 3
                            num = 823;
                            _Modul1.Instance.FamInArb = famInArb;
                            _Modul1.Instance.Ubg = 602;
                            Berufe();
                            _Modul1.Instance.Ubg = 603;
                            Berufe();
                            Anz[0].SelectionIndent = 0;
                            DataModul.DB_FamilyTable.Index = "Fam";
                            DataModul.DB_FamilyTable.Seek("=", _Modul1.Instance.FamInArb);
                            if (DataModul.DB_FamilyTable.Fields[FamilyFields.Name].Value.AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_FamilyTable.Fields[FamilyFields.Name].AsInt();
                                LD = "";
                                DataModul.Textlese(AAA, out KontU, out LD);
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                                Anz[0].SelectedText = "\nGemeinsamer Familienname:";
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                                Anz[0].SelectedText = " " + KontU.Trim() + " ";
                            }
                            goto IL_4790;
                        IL_4790:
                            num = 838;
                            if (_Modul1.Instance.Aus[4] == "Y")
                            {
                                if (Operators.CompareString(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                                    Anz[0].SelectedText = "\nBemerkungen zur Familie:";
                                    Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                                    Anz[0].SelectedText = DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].AsString() + '\n';

                                }
                            }
                            goto IL_4899;
                        IL_4899: // <========== 3
                            num = 846;
                            DataModul.DB_SourceLinkTable.Index = "Tab";
                            DataModul.DB_SourceLinkTable.Seek("=", 2, _Modul1.Instance.FamInArb);
                            goto IL_4bf7;
                        IL_4aa4: // <========== 3
                            num = 863;
                            if (Operators.CompareString(DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim(), "", TextCompare: false) != 0)
                            {
                                if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value))
                                {
                                    if (Operators.CompareString(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        left = " " + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim() + " ";
                                        goto IL_4bb0;
                                    }
                                    left = " Seiten: ";
                                    goto IL_4bb0;
                                }
                                left = " Seiten: ";
                                goto IL_4bb0;
                            }
                            goto IL_4be6;
                        IL_4bb0: // <========== 4
                            num = 875;
                            QuText = (QuText + left + DataModul.DB_SourceLinkTable.Fields[3].AsString());
                            goto IL_4be6;
                        IL_4be6: // <========== 5
                            num = 879;
                            DataModul.DB_SourceLinkTable.MoveNext();
                            goto IL_4bf7;
                        IL_4bf7: // <========== 3
                            num = 849;
                            if (!DataModul.DB_SourceLinkTable.EOF)
                            {
                                if (!DataModul.DB_SourceLinkTable.NoMatch)
                                {
                                    if (!Conversions.ToBoolean((DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._1].AsInt() != 2)
                                        | (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].AsInt() != _Modul1.Instance.FamInArb)))
                                    {
                                        DataModul.DB_QuTable.Index = "NR";
                                        DataModul.DB_QuTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3]);
                                        if (!DataModul.DB_QuTable.NoMatch)
                                        {
                                            if (QuText.Trim().Length > 0)
                                            {
                                                QuText = (QuText + "; " + DataModul.DB_QuTable.Fields[QuFields._2].AsString());
                                            }
                                            else
                                            {
                                                QuText = (QuText + DataModul.DB_QuTable.Fields[QuFields._2].AsString());
                                            }
                                            goto IL_4aa4;
                                        }
                                        goto IL_4be6;
                                    }
                                    goto IL_4c0d;
                                }
                                goto IL_4be6;
                            }
                            goto IL_4c0d;
                        IL_4c0d: // <========== 3
                            num = 881;
                            if (!Information.IsDBNull(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].Value))
                            {
                                if (Strings.Len(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].AsString().Trim()) > 0)
                                {
                                    QuText = (QuText + "; " + DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].AsString());

                                }
                            }
                            goto IL_4ca3;
                        IL_4ca3: // <========== 3
                            num = 886;
                            if (QuText.Trim() != "")
                            {
                                Zeiweg(ref QuText);
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                                Anz[0].SelectedText = "\nQuellen: ";
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                                Anz[0].SelectedText = QuText.Trim() + ".";
                                QuText = "";
                                Anz[0].SelectedText = "\n";
                            }
                            _Modul1.Instance.FamInArb = (int)Math.Round(Label1[1].Text.AsDouble());
                            _Modul1.Instance.eLKennz = ELinkKennz.lkChild;
                            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            KText = "Kinder:";
                            Kindles(ref KText);
                            _Modul1.Instance.FamInArb = (int)Math.Round(Label1[1].Text.AsDouble());
                            _Modul1.Instance.eLKennz = ELinkKennz.lkAdoptedChild;
                            KText = "Adoptivkinder:";
                            Kindles(ref KText);
                            if (List7.Items.Count > 0)
                            {
                                List7.SelectedIndex = 0;
                                _Modul1.Instance.FamInArb = (int)Math.Round(List7.Text.AsDouble());
                                List7.Items.RemoveAt(0);
                                Label1[1].Text = _Modul1.Instance.FamInArb.AsString();
                                Anz[0].SelectedText = "\n\n";
                                goto IL_2df9;
                            }
                            Anz[0].Enabled = true;
                            Command2[0].Enabled = true;
                            Command2[1].Enabled = true;
                            Command2[2].Enabled = true;
                            Cursor = Cursors.Default;
                            Anz[0].SaveFile(_Modul1.Instance.GenFreeDir + "\\Temp\\Text2.RTF", RichTextBoxStreamType.RichText);
                            Anz[0].LoadFile(_Modul1.Instance.GenFreeDir + "\\Temp\\Text2.RTF", RichTextBoxStreamType.RichText);
                            goto end_IL_0000_2;
                        IL_5007:
                            num = 923;
                            List1.Visible = true;
                            Listleer();
                            if (ComboBox1.Text.Trim() == "")
                            {
                                ComboBox1.Text = ComboBox1.Items[0].AsString();
                            }
                            if (Text1.Text.Trim() != ComboBox1.Text.Trim())
                            {
                                Text1.Text = "";
                                Text1.Text = ComboBox1.Text.Trim();
                                if (Text1.Text.Trim() == "")
                                {
                                    Interaction.MsgBox("Suchbegriff muss angegeben werden");
                                    goto end_IL_0000_2;
                                }
                            }
                            if (Conversions.ToBoolean((ComboBox1.Items[0].AsString() != ComboBox1.Text) & ComboBox1.Text != ""))
                            {
                                ComboBox1.Items.Insert(0, ComboBox1.Text);
                                Suchspeich();
                            }
                            if (Text1.Text.Trim() == "")
                            {
                                Interaction.MsgBox("Suchbegriff muss angegeben werden");
                            }
                            else
                            {
                                Listfuell();
                                ComboBox1.Focus();
                            }
                            goto end_IL_0000_2;
                        IL_5209:
                            num = 950;
                            FileSystem.FileClose(6);
                            FileSystem.Kill(_Modul1.Instance.GenFreeDir + "\\Init\\List");
                            FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\List", OpenMode.Output);
                            num5 = 0;
                            while (num5 <= 4)
                            {
                                FileSystem.PrintLine(6, Check1[(short)num5].CheckState);
                                num5++;
                            }
                            num5 = 0;
                            while (num5 <= 6)
                            {
                                _Modul1.Instance.Aus[num5] = "";
                                num5++;
                            }
                            num5 = 28;
                            while (num5 <= 38)
                            {
                                _Modul1.Instance.Aus[num5] = "";
                                num5++;
                            }
                            if (CheckBox1.Checked)
                            {
                                _Modul1.Instance.Aus[1] = "Y";
                            }
                            if (CheckBox2.Checked)
                            {
                                _Modul1.Instance.Aus[2] = "Y";
                            }
                            if (CheckBox3.Checked)
                            {
                                _Modul1.Instance.Aus[3] = "Y";
                            }
                            if (CheckBox4.Checked)
                            {
                                _Modul1.Instance.Aus[4] = "Y";
                            }
                            if (CheckBox5.Checked)
                            {
                                _Modul1.Instance.Aus[5] = "Y";
                            }
                            if (CheckBox6.Checked)
                            {
                                _Modul1.Instance.Aus[6] = "Y";
                            }
                            if (CheckBox7.Checked)
                            {
                                _Modul1.Instance.Aus[30] = "Y";
                            }
                            if (CheckBox8.Checked)
                            {
                                _Modul1.Instance.Aus[31] = "Y";
                            }
                            if (CheckBox9.Checked)
                            {
                                _Modul1.Instance.Aus[32] = "Y";
                            }
                            if (CheckBox10.Checked)
                            {
                                _Modul1.Instance.Aus[33] = "Y";
                            }
                            if (CheckBox11.Checked)
                            {
                                _Modul1.Instance.Aus[34] = "Y";
                            }
                            if (CheckBox12.Checked)
                            {
                                _Modul1.Instance.Aus[35] = "Y";
                            }
                            if (CheckBox13.Checked)
                            {
                                _Modul1.Instance.Aus[36] = "Y";
                            }
                            if (CheckBox14.Checked)
                            {
                                _Modul1.Instance.Aus[37] = "Y";
                            }
                            if (CheckBox15.Checked)
                            {
                                _Modul1.Instance.Aus[38] = "Y";
                            }
                            Struck = 0;
                            if (Check1[4].Checked)
                            {
                                Struck = 1;
                            }
                            FileSystem.FileClose(6);
                            FileSystem.FileClose(99);
                            FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "\\Init\\Druck_ini.dat", OpenMode.Output);
                            num5 = 1;
                            while (num5 <= 50)
                            {
                                FileSystem.PrintLine(99, _Modul1.Instance.Aus[num5]);
                                num5++;
                            }
                            FileSystem.FileClose(99);
                            An = 1;
                            if (Index == 6)
                            {
                                Command1[1].PerformClick();
                            }
                            goto IL_568f;
                        IL_568f:
                            num = 1022;
                            if (Index == 5)
                            {
                                Command1[2].PerformClick();
                            }
                            goto IL_56b3;
                        IL_56b3:
                            num = 1025;
                            Frame2.Visible = false;
                            goto end_IL_0000_2;
                        IL_56c8:
                            num = 1028;
                            DL = List1;
                            A = 1;
                            Listbox3Clip(ref DL, ref A);
                            List1 = DL;
                            goto end_IL_0000_2;
                        IL_5848:
                            num = 1056;
                            if (Information.Err().Number == 53)
                            {
                                FileSystem.FileClose(6);
                                FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\List", OpenMode.Output);
                                num5 = 0;
                                while (num5 <= 2)
                                {
                                    FileSystem.PrintLine(6, "0");
                                    num5++;
                                }
                                FileSystem.FileClose(6);
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_5a10;
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
                            goto IL_5a10;
                        IL_595b:
                            num = 1071;
                            if (Information.Err().Number == 424)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_5a14;
                            }
                            if (Information.Err().Number == 91)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_5a14;
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
                            goto IL_5a10;
                        IL_5a10: // <========== 4
                            num4 = num2;
                            goto IL_5a18;
                        IL_5a14: // <========== 8
                            num4 = unchecked(num2 + 1);
                            goto IL_5a18;
                        IL_5a18:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 134:
                                case 135:
                                case 418:
                                    goto IL_0854;
                                case 141:
                                case 144:
                                case 145:
                                    goto IL_0919;
                                case 176:
                                case 179:
                                case 180:
                                    goto IL_0dbb;
                                case 168:
                                case 169:
                                case 186:
                                    goto IL_0e34;
                                case 172:
                                case 183:
                                case 187:
                                case 188:
                                    goto IL_0e4a;
                                case 209:
                                case 213:
                                    goto IL_0fdd;
                                case 226:
                                case 230:
                                case 233:
                                case 239:
                                case 243:
                                    goto IL_128a;
                                case 248:
                                case 251:
                                case 252:
                                    goto IL_134c;
                                case 270:
                                case 273:
                                case 274:
                                    goto IL_15d9;
                                case 286:
                                case 287:
                                case 288:
                                case 289:
                                    goto IL_177d;
                                case 291:
                                case 296:
                                case 297:
                                    goto IL_183c;
                                case 298:
                                case 299:
                                    goto IL_1852;
                                case 311:
                                    goto IL_1977;
                                case 315:
                                case 320:
                                case 321:
                                case 322:
                                    goto IL_1a7d;
                                case 319:
                                case 323:
                                    goto IL_1aa2;
                                case 327:
                                case 328:
                                case 329:
                                    goto IL_1b7e;
                                case 351:
                                case 352:
                                    goto IL_1de8;
                                case 353:
                                case 354:
                                    goto IL_1e1c;
                                case 373:
                                case 376:
                                case 377:
                                    goto IL_20c5;
                                case 383:
                                case 386:
                                case 387:
                                    goto IL_21db;
                                case 395:
                                case 396:
                                case 397:
                                case 398:
                                    goto IL_22ff;
                                case 399:
                                case 404:
                                case 405:
                                    goto IL_2371;
                                case 547:
                                case 550:
                                case 551:
                                case 561:
                                case 912:
                                    goto IL_2df9;
                                case 597:
                                case 600:
                                case 601:
                                    goto IL_3204;
                                case 609:
                                case 613:
                                case 614:
                                    goto IL_32ce;
                                case 620:
                                case 623:
                                case 624:
                                    goto IL_33d7;
                                case 641:
                                case 644:
                                case 645:
                                    goto IL_36d3;
                                case 648:
                                case 649:
                                case 652:
                                case 653:
                                    goto IL_3789;
                                case 633:
                                case 634:
                                case 654:
                                    goto IL_379a;
                                case 637:
                                case 651:
                                case 655:
                                case 656:
                                    goto IL_37b0;
                                case 666:
                                case 669:
                                case 670:
                                    goto IL_38fc;
                                case 680:
                                case 683:
                                case 686:
                                case 689:
                                case 701:
                                    goto IL_3b79;
                                case 715:
                                case 718:
                                case 719:
                                    goto IL_3ce2;
                                case 724:
                                case 727:
                                case 739:
                                    goto IL_3eab;
                                case 753:
                                case 756:
                                case 757:
                                    goto IL_403d;
                                case 762:
                                case 763:
                                    goto IL_40d8;
                                case 764:
                                case 765:
                                case 766:
                                    goto IL_4118;
                                case 770:
                                case 773:
                                case 776:
                                case 788:
                                    goto IL_42e3;
                                case 801:
                                case 804:
                                case 805:
                                    goto IL_4427;
                                case 819:
                                case 822:
                                case 823:
                                    goto IL_459c;
                                case 837:
                                case 838:
                                    goto IL_4790;
                                case 844:
                                case 845:
                                case 846:
                                    goto IL_4899;
                                case 859:
                                case 862:
                                case 863:
                                    goto IL_4aa4;
                                case 867:
                                case 870:
                                case 871:
                                case 874:
                                case 875:
                                    goto IL_4bb0;
                                case 876:
                                case 877:
                                case 878:
                                case 879:
                                    goto IL_4be6;
                                case 848:
                                case 849:
                                case 880:
                                    goto IL_4bf7;
                                case 852:
                                case 881:
                                    goto IL_4c0d;
                                case 884:
                                case 885:
                                case 886:
                                    goto IL_4ca3;
                                case 1021:
                                case 1022:
                                    goto IL_568f;
                                case 1024:
                                case 1025:
                                    goto IL_56b3;
                                case 13:
                                case 29:
                                case 30:
                                case 133:
                                case 422:
                                case 531:
                                case 564:
                                case 921:
                                case 933:
                                case 942:
                                case 946:
                                case 947:
                                case 948:
                                case 1026:
                                case 1029:
                                case 1030:
                                case 1083:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 27410;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 14
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    private void Command2_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_013c, IL_0145, IL_0156
        int try0000_dispatch = -1;
        int num2 = default;
        int num = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                switch (try0000_dispatch)
                {
                    default:
                        {
                            int index = Command2.GetIndex((Button)eventSender);
                            string drive = FileSystem.CurDir().Left(1);
                            switch (index)
                            {
                                case 0:
                                    Anz[0].SaveFile(_Modul1.Instance.GenFreeDir + "\\Temp\\Text2.RTF", RichTextBoxStreamType.RichText);
                                    Anz[0].LoadFile(_Modul1.Instance.GenFreeDir + "\\Temp\\Text2.RTF", RichTextBoxStreamType.RichText);
                                    ProjectData.ClearProjectError();
                                    num2 = 2;
                                    Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.GenFreeDir + "\\Temp\\Text2.RTF", AppWinStyle.NormalFocus);
                                    break;
                                case 1:
                                    An = 0;
                                    Option1[3].Visible = true;
                                    Option1[4].Visible = true;
                                    if (Option1[6].Enabled)
                                    {
                                        Option1[6].Visible = true;
                                    }
                                    Frame1.Visible = false;
                                    ComboBox1.Visible = true;
                                    Command1[3].Visible = true;
                                    Close();
                                    MyProject.Forms.Druck.Show();
                                    goto end_IL_0000;
                                case 2:
                                    CommonDialog1Save.Filter = "Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF";
                                    CommonDialog1Save.FilterIndex = 1;
                                    CommonDialog1Save.InitialDirectory = _Modul1.Instance.GenFreeDir + "\\list\\";
                                    CommonDialog1Save.ShowDialog();
                                    CommonDialog1Save.Filter = "Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF";
                                    if (CommonDialog1Save.FileName != "")
                                    {
                                        switch (CommonDialog1Save.FilterIndex)
                                        {
                                            case 1:
                                                Anz[0].SaveFile(CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);
                                                break;
                                            case 2:
                                                Anz[0].SaveFile(CommonDialog1Save.FileName, RichTextBoxStreamType.RichText);
                                                break;
                                        }
                                    }
                                    break;
                            }
                            FileSystem.ChDrive(drive);
                            goto end_IL_0000;
                        }
                    case 601:
                        num = -1;
                        switch (num2)
                        {
                            case 2:
                                if (Information.Err().Number == 53)
                                {
                                    Interaction.MsgBox("Die ausgewählte Textverarbeitung oder der Pfad der Textverarbeitung stimmt nicht. Bitte neu auswählen");
                                    Hide();
                                }
                                goto end_IL_0000;
                        }
                        break;
                }
                goto IL_0293;
            end_IL_0000:;
            }
            catch (Exception obj) when (obj is not null && num2 != 0 && num == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 601;
                continue;
            }
            break;
        IL_0293:
            throw ProjectData.CreateProjectError(-2146828237);
        }
        if (num != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void Namensuch_Load(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        short num4 = default;
        string text = default;
        short num6 = default;
        int lErl = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num5;
                    string nDrive;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            _Modul1.Instance.Suchfam = 0;
                            goto IL_0009;
                        case 3043:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 4:
                                    case 6:
                                        break;
                                    case 2:
                                        goto IL_07e8;
                                    case 3:
                                        goto IL_083a;
                                    case 5:
                                        goto IL_08b0;
                                    case 1:
                                        goto IL_0975;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 62)
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
                                goto IL_0971;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            num = 2;
                            _Modul1.Instance.Suchper = 0;
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            if (_Modul1.Instance.Fs > 0f)
                            {
                                Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                                List1.Font = new Font("Courier New", _Modul1.Instance.Fs, FontStyle.Regular);
                                Label3.Font = new Font("Courier New", _Modul1.Instance.Fs, FontStyle.Regular);
                                Option1[3].Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                                Option1[4].Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            }
                            Label6[6].Font = Font;
                            ComboBox1.Font = Font;
                            Text1.Font = Font;
                            WiS = "";
                            FileSystem.FileClose(6);
                            FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\Windowstate", OpenMode.Input);
                            FileSystem.Input(6, ref WiS);
                            FileSystem.FileClose(6);
                            WindowState = unchecked((FormWindowState)WiS.AsInt());
                            Frame1.Top = 0;
                            Frame1.Left = 0;
                            Frame1.Height = Height;
                            Frame1.Width = Width;
                            Frame1.Font = Font;
                            num4 = 0;
                            while (num4 <= 15)
                            {
                                Label7[num4].Visible = false;
                                num4 = (short)unchecked(num4 + 1);
                            }
                            List1.Visible = true;
                            Frame1.Height = Height;
                            Frame1.Width = Width;
                            BackColor = _Modul1.Instance.HintFarb;
                            Label1[0].Visible = false;
                            Label1[1].Visible = false;
                            Option1[6].Enabled = false;
                            ProjectData.ClearProjectError();
                            num3 = 3;
                            if (Check2[3].Checked)
                            {
                                FileSystem.FileClose(99);
                                FileSystem.FileOpen(99, _Modul1.Instance.Verz + "SUCH.DAT", OpenMode.Append);
                                FileSystem.FileClose(99);
                                FileSystem.FileOpen(99, _Modul1.Instance.Verz + "SUCH.DAT", OpenMode.Input);
                                ProjectData.ClearProjectError();
                                num3 = 4;
                                ComboBox1.Items.Clear();
                                num4 = 1;
                                while (num4 <= 10)
                                {
                                    text = FileSystem.LineInput(99);
                                    if (text.Trim() != "")
                                    {
                                        ComboBox1.Items.Add(text);
                                    }
                                    num4 = (short)unchecked(num4 + 1);
                                }
                                ComboBox1.Text = ComboBox1.Items[0].AsString();
                                ComboBox1.Focus();
                                ComboBox1.SelectionStart = ComboBox1.Text.Length;
                                goto end_IL_0000_2;
                            }
                            if (_Modul1.Instance.Suchschalt == 10)
                            {
                                Command1[4].Visible = true;
                                Label9.Visible = true;
                                List6.Items.Clear();
                                List6.Visible = true;
                                List1.Width = Width - 100;
                            }
                            num4 = 0;
                            while (num4 <= 15)
                            {
                                Label5[num4].Font = Font;
                                num4 = (short)unchecked(num4 + 1);
                            }
                            DataModul.DT_DescendentTable.MoveFirst();
                            if (DataModul.DT_DescendentTable.Fields["Gen"].AsString().Length > 0)
                            {
                                Option1[6].Enabled = true;
                            }
                            goto IL_04e9;
                        IL_04e9: // <========== 4
                            num = 68;
                            lErl = 2;
                            ProjectData.ClearProjectError();
                            num3 = 5;
                            DataModul.DT_AncesterTable.MoveFirst();
                            if (DataModul.DT_AncesterTable.RecordCount > 0)
                            {
                                Option1[7].Enabled = true;
                            }
                            goto IL_052d;
                        IL_052d: // <========== 4
                            num = 74;
                            lErl = 3;
                            ProjectData.ClearProjectError();
                            num3 = 0;
                            Command2[0].Text = _Modul1.Instance.IText[56];
                            Command2[1].Text = _Modul1.Instance.IText[57];
                            Command2[2].Text = _Modul1.Instance.IText[47];
                            num4 = 0;
                            goto IL_0599;
                        IL_0599: // <========== 3
                            num = 80;
                            if (!Option1[num4].Checked)
                            {
                                if (num4 == 7)
                                {
                                    Option1[0].Checked = true;
                                }
                                goto IL_05d0;
                            }
                            goto IL_05dd;
                        IL_05d0:
                            num = 86;
                            num4 = (short)unchecked(num4 + 1);
                            if (num4 <= 7)
                            {
                                goto IL_0599;
                            }
                            goto IL_05dd;
                        IL_05dd: // <========== 3
                            num = 87;
                            FileSystem.FileClose(99);
                            FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "\\Init\\Druck_ini.dat", OpenMode.Input);
                            num4 = 0;
                            while (!FileSystem.EOF(99))
                            {
                                num4 = (short)(num4 + 1);
                                FileSystem.Input(99, ref _Modul1.Instance.Aus[num4]);
                            }
                            FileSystem.FileClose(99);
                            if (_Modul1.Instance.Aus[24] != "1")
                            {
                                _Modul1.Instance.Aus[24] = "0";
                            }
                            nDrive = _Modul1.Instance.Verz1.Left(1) + ":\\";
                            num6 = (short)_Modul1.Instance.GetDriveType(ref nDrive);
                            if (num6 == 5)
                            {
                                goto end_IL_0000_2;
                            }
                            FileSystem.FileOpen(99, _Modul1.Instance.Verz + "SUCH.DAT", OpenMode.Append);
                            FileSystem.FileClose(99);
                            FileSystem.FileOpen(99, _Modul1.Instance.Verz + "SUCH.DAT", OpenMode.Input);
                            ProjectData.ClearProjectError();
                            num3 = 6;
                            ComboBox1.Items.Clear();
                            num4 = 1;
                            while (num4 <= 10)
                            {
                                text = FileSystem.LineInput(99);
                                if (text.Trim() != "")
                                {
                                    ComboBox1.Items.Add(text);
                                }
                                num4 = (short)unchecked(num4 + 1);
                            }
                            ProjectData.ClearProjectError();
                            num3 = 0;
                            goto end_IL_0000_2;
                        IL_07e8:
                            num = 125;
                            if (Information.Err().Number == 380)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0975;
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_0975;
                        IL_083a:
                            num = 131;
                            if (Information.Err().Number == 3021)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num2 = 0;
                                goto IL_04e9;
                            }
                            if (Information.Err().Number == 424)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num2 = 0;
                                goto IL_04e9;
                            }
                            goto IL_08b0;
                        IL_08b0: // <========== 3
                            num = 139;
                            if (Information.Err().Number == 3021)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num2 = 0;
                                goto IL_052d;
                            }
                            if (Information.Err().Number == 424)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num2 = 0;
                                goto IL_052d;
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
                            goto IL_0971;
                        IL_0971:
                            num5 = num2;
                            goto IL_0979;
                        IL_0975: // <========== 3
                            num5 = unchecked(num2 + 1);
                            goto IL_0979;
                        IL_0979:
                            num2 = 0;
                            switch (num5)
                            {
                                case 1:
                                    break;
                                case 67:
                                case 68:
                                case 133:
                                case 137:
                                    goto IL_04e9;
                                case 73:
                                case 74:
                                case 141:
                                case 145:
                                    goto IL_052d;
                                case 80:
                                    goto IL_0599;
                                case 85:
                                case 86:
                                    goto IL_05d0;
                                case 81:
                                case 87:
                                    goto IL_05dd;
                                case 138:
                                case 139:
                                    goto IL_08b0;
                                case 52:
                                case 101:
                                case 115:
                                case 117:
                                case 123:
                                case 124:
                                case 151:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 3043;
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
    private void Namensuch_FormClosed(object eventSender, FormClosedEventArgs eventArgs)
    {
        _Modul1.Instance.Suchschalt = 0;
        string nDrive = _Modul1.Instance.Verz1.Left(1) + ":\\";
        short num = checked((short)_Modul1.Instance.GetDriveType(ref nDrive));
        if (num != 5)
        {
            Suchspeich();
        }
    }

    private void Label5_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int index = default;
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
                        index = Label5.GetIndex((Label)eventSender);
                        goto IL_0014;
                    case 506:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0162;
                                default:
                                    goto end_IL_0000;
                            }
                            if (Information.Err().Number == 402)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0162;
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
                            goto IL_0165;
                        }
                    end_IL_0000:
                        break;
                    IL_0014:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        if (_Modul1.Instance.Suchschalt == 2)
                        {
                            goto end_IL_0000_2;
                        }
                        if (_Modul1.Instance.Suchschalt == 10)
                        {
                            goto end_IL_0000_2;
                        }
                        _Modul1.Instance.PersInArb = checked((int)Math.Round(Label7[index].Text.AsDouble()));
                        if (_Modul1.Instance.PersInArb <= 0)
                        {
                            goto end_IL_0000_2;
                        }
                        if (_Modul1.Instance.Suchschalt == 1)
                        {
                            Hide();
                            MyProject.Forms.Familie.Hide();
                            _Modul1.Instance.Ad = true;
                            MyProject.Forms.Personen.Show();
                            MyProject.Forms.Personen.Perzeig();
                            _Modul1.Instance.Ubg = _Modul1.Instance.PersInArb;

                        }
                        else
                        {
                            _Modul1.Instance.Suchper = _Modul1.Instance.PersInArb;
                            Hide();
                        }
                        goto end_IL_0000_2;
                    IL_0162:
                        num4 = num2 + 1;
                        goto IL_0165;
                    IL_0165:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 4:
                            case 7:
                            case 18:
                            case 22:
                            case 23:
                            case 24:
                            case 25:
                            case 34:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 506;
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
    private void Label6_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        int index = Label6.GetIndex((Label)eventSender);
        int iFam = 0;
        iFam = Label8[index].Tag.AsInt();
        if (_Modul1.Instance.Suchschalt == 2)
        {
            Hide();
        }
        if (_Modul1.Instance.Suchschalt == 0)
        {
            return;
        }
        _Modul1.Instance.Schalt = 0;
        if (iFam > 0)
        {
            if (Check2[3].Checked)
            {
                Hide();
            }
            if (Check2[3].CheckState == CheckState.Unchecked)
            {
                Hide();
            }
            MyProject.Forms.Personen.Hide();
            MyProject.Forms.Familie.Show();
            _Modul1.Instance.FamInArb = iFam;
        }
    }

    private void List1_SelectedIndexChanged(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        short num5 = default;
        int famInArb = default;
        int frau = default;
        short num6 = default;
        string text = default;
        IList<int> aiFams;
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
                        case 6197:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_140f;
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
                                goto IL_1413;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            Command1[1].Visible = true;
                            if (Option1[4].Checked | Option1[3].Checked)
                            {
                                Label1[0].Text = Strings.Mid(List1.Text, 75, 10);
                            }
                            else
                            {
                                Label1[0].Text = Strings.Mid(List1.Text, 48, 10);
                            }
                            goto IL_009c;
                        IL_009c: // <========== 3
                            num = 9;
                            lErl = 10;
                            if (Label1[0].Text.AsDouble() > 0.0)
                            {
                                Command1[1].Visible = true;
                            }
                            else
                            {
                                Command1[1].Visible = false;
                            }
                            goto IL_00fb;
                        IL_00fb: // <========== 3
                            num = 16;
                            Label1[1].Text = Strings.Mid(List1.Text, 135, 10);
                            if (Label1[1].Text.AsDouble() > 0.0)
                            {
                                Command1[2].Visible = true;
                            }
                            else
                            {
                                Command1[2].Visible = false;
                            }
                            goto IL_017e;
                        IL_017e: // <========== 3
                            num = 23;
                            _Modul1.Instance.FamInArb = 0;
                            if (List1.Text.Length > 131)
                            {
                                _Modul1.Instance.FamInArb = Strings.Mid(List1.Text, 135, 10).AsInt();
                            }
                            num5 = 0;
                            while (num5 <= 7)
                            {
                                Label8[num5].Text = "";
                                num5 = (short)unchecked(num5 + 1);
                            }
                            num5 = 0;
                            while (num5 <= 15)
                            {
                                Label5[num5].Text = "";
                                Label7[num5].Text = "";
                                num5 = (short)unchecked(num5 + 1);
                            }
                            if (_Modul1.Instance.FamInArb > 0)
                            {
                                _Modul1.Instance.Famles();
                                Label8[5].Text = _Modul1.Instance.FamInArb.AsString();
                                Label8[5].Tag = _Modul1.Instance.FamInArb;
                                famInArb = _Modul1.Instance.FamInArb;
                                frau = _Modul1.Instance.Family.Frau;
                                if (_Modul1.Instance.Family.Mann > 0)
                                {
                                    _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                                    Label7[4].Text = _Modul1.Instance.PersInArb.AsString();
                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                    {
                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0] + " " + _Modul1.Instance.Kont[2];
                                    }
                                    if (_Modul1.Instance.Kont[1] != "")
                                    {
                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1] + " " + _Modul1.Instance.Kont[0];
                                    }
                                    Label5[4].Text = _Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[0];
                                    Datr();
                                    Label5[4].Text = Datum4 + " " + Label5[4].Text + Datum3;
                                    _Modul1.Instance.eLKennz = ELinkKennz.lkChild;
                                    aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                    _Modul1.Instance.FamInArb = (int)Math.Round(_Modul1.Instance.UbgT.AsDouble());
                                    Label8[7].Text = _Modul1.Instance.FamInArb.AsString();
                                    Label8[7].Tag = _Modul1.Instance.FamInArb;
                                    _Modul1.Instance.Family.Mann = 0;
                                    frau = _Modul1.Instance.Family.Frau;
                                    _Modul1.Instance.Family.Frau = 0;
                                    _Modul1.Instance.Famles();
                                    if (_Modul1.Instance.Family.Mann > 0)
                                    {
                                        _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                                        Label7[0].Text = _Modul1.Instance.PersInArb.AsString();
                                        _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                        if (_Modul1.Instance.Kont[2].Trim() != "")
                                        {
                                            _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0] + " " + _Modul1.Instance.Kont[2];
                                        }
                                        if (_Modul1.Instance.Kont[1] != "")
                                        {
                                            _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1] + " " + _Modul1.Instance.Kont[0];
                                        }
                                        Label5[0].Text = _Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[0];
                                        Datr();
                                        Label5[0].Text = Datum4 + " " + Label5[0].Text + Datum3;
                                    }
                                    goto IL_0568;
                                }
                                goto IL_069d;
                            }
                            _Modul1.Instance.PersInArb = (int)Math.Round(Label1[0].Text.AsDouble());
                            Label7[4].Text = _Modul1.Instance.PersInArb.AsString();
                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            if (_Modul1.Instance.Kont[2] != "")
                            {
                                _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0] + " " + _Modul1.Instance.Kont[2];
                            }
                            if (_Modul1.Instance.Kont[1] != "")
                            {
                                _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1] + " " + _Modul1.Instance.Kont[0];
                            }
                            Label5[4].Text = _Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[0];
                            Datr();
                            Label5[4].Text = Datum4 + " " + Label5[4].Text + Datum3;
                            _Modul1.Instance.eLKennz = ELinkKennz.lkChild;
                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                            _Modul1.Instance.FamInArb = (int)Math.Round(_Modul1.Instance.UbgT.AsDouble());
                            _Modul1.Instance.Family.Mann = 0;
                            frau = _Modul1.Instance.Family.Frau;
                            _Modul1.Instance.Family.Frau = 0;
                            _Modul1.Instance.Famles();
                            if (_Modul1.Instance.Family.Mann > 0)
                            {
                                _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                                Label7[0].Text = _Modul1.Instance.PersInArb.AsString();
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                if (_Modul1.Instance.Kont[2].Trim() != "")
                                {
                                    _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0] + " " + _Modul1.Instance.Kont[2];
                                }
                                if (_Modul1.Instance.Kont[1] != "")
                                {
                                    _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1] + " " + _Modul1.Instance.Kont[0];
                                }
                                Label5[0].Text = _Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[0];
                                Datr();
                                Label5[0].Text = Datum4 + " " + Label5[0].Text + Datum3;
                            }
                            if (_Modul1.Instance.Family.Frau <= 0)
                            {
                                goto end_IL_0000_2;
                            }
                            _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau;
                            Label7[1].Text = _Modul1.Instance.PersInArb.AsString();
                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            if (_Modul1.Instance.Kont[2].Trim() != "")
                            {
                                _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0] + " " + _Modul1.Instance.Kont[2];
                            }
                            if (_Modul1.Instance.Kont[1] != "")
                            {
                                _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1] + " " + _Modul1.Instance.Kont[0];
                            }
                            Label5[1].Text = _Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[0];
                            Datr();
                            Label5[1].Text = Datum4 + " " + Label5[1].Text + Datum3;
                            goto end_IL_0000_2;
                        IL_0568:
                            num = 74;
                            if (_Modul1.Instance.Family.Frau > 0)
                            {
                                _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau;
                                Label7[1].Text = _Modul1.Instance.PersInArb.AsString();
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                if (_Modul1.Instance.Kont[2].Trim() != "")
                                {
                                    _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0] + " " + _Modul1.Instance.Kont[2];
                                }
                                if (_Modul1.Instance.Kont[1] != "")
                                {
                                    _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1] + " " + _Modul1.Instance.Kont[0];
                                }
                                Label5[1].Text = _Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[0];
                                Datr();
                                Label5[1].Text = Datum4 + " " + Label5[1].Text + Datum3;
                            }
                            goto IL_069d;
                        IL_069d: // <========== 3
                            num = 89;
                            if (frau > 0)
                            {
                                _Modul1.Instance.PersInArb = frau;
                                Label7[5].Text = _Modul1.Instance.PersInArb.AsString();
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                if (_Modul1.Instance.Kont[2].Trim() != "")
                                {
                                    _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0] + " " + _Modul1.Instance.Kont[2];
                                }
                                if (_Modul1.Instance.Kont[1] != "")
                                {
                                    _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1] + " " + _Modul1.Instance.Kont[0];
                                }
                                Label5[5].Text = _Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[0];
                                Datr();
                                Label5[5].Text = Datum4 + " " + Label5[5].Text + Datum3;
                                _Modul1.Instance.eLKennz = ELinkKennz.lkChild;
                                aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                _Modul1.Instance.FamInArb = (int)Math.Round(_Modul1.Instance.UbgT.AsDouble());
                                Label8[6].Text = _Modul1.Instance.FamInArb.AsString();
                                Label8[6].Tag = _Modul1.Instance.FamInArb;
                                _Modul1.Instance.Family.Mann = 0;
                                _Modul1.Instance.Family.Frau = 0;
                                _Modul1.Instance.Famles();
                                if (_Modul1.Instance.Family.Mann > 0)
                                {
                                    _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                                    Label7[2].Text = _Modul1.Instance.PersInArb.AsString();
                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                    {
                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0] + " " + _Modul1.Instance.Kont[2];
                                    }
                                    if (_Modul1.Instance.Kont[1] != "")
                                    {
                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1] + " " + _Modul1.Instance.Kont[0];
                                    }
                                    Label5[2].Text = _Modul1.Instance.Person.Givennames + " " + _Modul1.Instance.Kont[0];
                                    Datr();
                                    Label5[2].Text = Datum4 + " " + Label5[2].Text + Datum3;
                                }
                                goto IL_096b;
                            }
                            goto IL_0ab2;
                        IL_096b:
                            num = 123;
                            if (_Modul1.Instance.Family.Frau > 0)
                            {
                                _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau;
                                Label7[3].Text = _Modul1.Instance.PersInArb.AsString();
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                if (_Modul1.Instance.Kont[2].Trim() != "")
                                {
                                    _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0] + " " + _Modul1.Instance.Kont[2];
                                }
                                if (_Modul1.Instance.Kont[1] != "")
                                {
                                    _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1] + " " + _Modul1.Instance.Kont[0];
                                }
                                Label5[3].Text = _Modul1.Instance.Person.Givennames + " " + _Modul1.Instance.Kont[0];
                                Datr();
                                Label5[3].Text = Datum4 + " " + Label5[3].Text + Datum3;
                            }
                            goto IL_0ab2;
                        IL_0ab2: // <========== 3
                            num = 138;
                            List4.Items.Clear();
                            _Modul1.Instance.FamInArb = famInArb;
                            kindsuch();
                            if (List4.Items.Count <= 0)
                            {
                                goto end_IL_0000_2;
                            }
                            num6 = (short)(List4.Items.Count - 1);
                            num5 = 0;
                            goto IL_0f39;
                        IL_0e64: // <========== 3
                            num = 191;
                            Label7[(short)(num5 + 11)].Text = _Modul1.Instance.PersInArb.AsString();
                            if (_Modul1.Instance.PersInArb > 0)
                            {
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                Label5[(short)(num5 + 11)].Text = _Modul1.Instance.Person.Givennames + " " + _Modul1.Instance.Kont[0];
                            }
                            else
                            {
                                Label5[(short)(num5 + 11)].Text = " unbekannter Partner";
                            }
                            goto IL_0f2d;
                        IL_0f2d: // <========== 5
                            num = 204;
                            num5 = (short)unchecked(num5 + 1);
                            goto IL_0f39;
                        IL_0f39:
                            if (num5 > num6)
                            {
                                goto end_IL_0000_2;
                            }
                            if (num5 > 4)
                            {
                                goto end_IL_0000_2;
                            }
                            _Modul1.Instance.PersInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List4.Items[num5].AsString(), 9, 10)));
                            Label7[(short)(num5 + 6)].Text = _Modul1.Instance.PersInArb.AsString();
                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            text = List4.Items[num5].AsString().Left(4).AsString() + " ";
                            if (text.AsDouble() == 0.0)
                            {
                                text = "";
                            }
                            Label5[(short)(num5 + 6)].Text = text.Trim() + _Modul1.Instance.Person.Givennames + " " + _Modul1.Instance.Kont[0];
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                            if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                            {
                                _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                            }
                            _Modul1.Instance.UbgT = "";
                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                            if (_Modul1.Instance.UbgT.Length > 0)
                            {
                                if (_Modul1.Instance.UbgT.Length > 10)
                                {
                                    Label5[(short)(num5 + 11)].Text = "mehrere Ehen";
                                    goto IL_0f2d;
                                }
                                _Modul1.Instance.FamInArb = (int)Math.Round(_Modul1.Instance.UbgT.AsDouble());
                                if (num5 == 1)
                                {
                                    Label8[0].Text = _Modul1.Instance.FamInArb.AsString();
                                    Label8[0].Tag = _Modul1.Instance.FamInArb;
                                }
                                if (num5 == 2)
                                {
                                    Label8[1].Text = _Modul1.Instance.FamInArb.AsString();
                                    Label8[1].Tag = _Modul1.Instance.FamInArb;
                                }
                                if (num5 == 3)
                                {
                                    Label8[3].Text = _Modul1.Instance.FamInArb.AsString();
                                    Label8[3].Tag = _Modul1.Instance.FamInArb;
                                }
                                if (num5 == 4)
                                {
                                    Label8[4].Text = _Modul1.Instance.FamInArb.AsString();
                                    Label8[4].Tag = _Modul1.Instance.FamInArb;
                                }
                                if (num5 == 0)
                                {
                                    Label8[2].Text = _Modul1.Instance.FamInArb.AsString();
                                    Label8[2].Tag = _Modul1.Instance.FamInArb;
                                }
                                _Modul1.Instance.Family.Mann = 0;
                                _Modul1.Instance.Family.Frau = 0;
                                _Modul1.Instance.Famles();
                                if (_Modul1.Instance.eLKennz == ELinkKennz.lkFather)
                                {
                                    _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau;
                                }
                                else
                                {
                                    _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                                }
                                goto IL_0e64;
                            }
                            Label5[(short)(num5 + 11)].Text = "Keine Ehe";
                            goto IL_0f2d;
                        IL_140f:
                            num4 = unchecked(num2 + 1);
                            goto IL_1413;
                        IL_1413:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 5:
                                case 9:
                                    goto IL_009c;
                                case 12:
                                case 15:
                                case 16:
                                    goto IL_00fb;
                                case 19:
                                case 22:
                                case 23:
                                    goto IL_017e;
                                case 73:
                                case 74:
                                    goto IL_0568;
                                case 87:
                                case 88:
                                case 89:
                                    goto IL_069d;
                                case 122:
                                case 123:
                                    goto IL_096b;
                                case 136:
                                case 137:
                                case 138:
                                    goto IL_0ab2;
                                case 187:
                                case 190:
                                case 191:
                                    goto IL_0e64;
                                case 164:
                                case 195:
                                case 198:
                                case 199:
                                case 200:
                                case 203:
                                case 204:
                                    goto IL_0f2d;
                                case 6:
                                case 144:
                                case 205:
                                case 206:
                                case 254:
                                case 255:
                                case 256:
                                case 261:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 6197;
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
    private void List1_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        if (Check2[3].CheckState == CheckState.Unchecked)
        {
            Check2[0].CheckState = CheckState.Unchecked;
            Check2[1].CheckState = CheckState.Unchecked;
            Check2[2].CheckState = CheckState.Unchecked;
            Check2[4].CheckState = CheckState.Unchecked;
            Check2[5].CheckState = CheckState.Unchecked;
        }
        checked
        {
            if ((List1.Text.Length < 133) & (_Modul1.Instance.Schalt == 1))
            {
                if (_Modul1.Instance.Suchschalt == 2)
                {
                    Interaction.MsgBox("Für eine Person kann kein Familienblatt gedruckt werden");
                    return;
                }
                if (Option1[4].Checked | Option1[3].Checked)
                {
                    _Modul1.Instance.Suchper = (int)Math.Round(Strings.Mid(List1.Text, 75, 10).AsDouble());
                    _Modul1.Instance.Ubg = (int)Math.Round(Strings.Mid(List1.Text, 75, 10).AsDouble());
                    _Modul1.Instance.PersInArb = (int)Math.Round(Strings.Mid(List1.Text, 75, 10).AsDouble());
                }
                else
                {
                    _Modul1.Instance.PersInArb = (int)Math.Round(Strings.Mid(List1.Text, 48, 10).AsDouble());
                }
                if (_Modul1.Instance.PersInArb > 0)
                {
                    if (Check2[3].Checked)
                    {
                        Hide();
                    }
                    if (Check2[3].CheckState == CheckState.Unchecked)
                    {
                        Listleer();
                        Close();
                    }
                }
            }
            else if (Option1[4].Checked | Option1[3].Checked)
            {
                _Modul1.Instance.Suchper = (int)Math.Round(Strings.Mid(List1.Text, 75, 10).AsDouble());
                _Modul1.Instance.Ubg = (int)Math.Round(Strings.Mid(List1.Text, 75, 10).AsDouble());
                Close();
            }
            else
            {
                _Modul1.Instance.Suchfam = (int)Math.Round(Strings.Mid(List1.Text, 135, 10).AsDouble());
                _Modul1.Instance.Suchper = (int)Math.Round(Strings.Mid(List1.Text, 48, 10).AsDouble());
                if (_Modul1.Instance.Schalt == 1)
                {
                    _Modul1.Instance.Ubg = (int)Math.Round(Strings.Mid(List1.Text, 135, 10).AsDouble());
                }
                else
                {
                    _Modul1.Instance.Ubg = (int)Math.Round(Strings.Mid(List1.Text, 48, 10).AsDouble());
                }
                _Modul1.Instance.Schalt = 0;
                if (Check2[3].CheckState == CheckState.Unchecked)
                {
                    Listleer();
                    ComboBox1.Text = "";
                }
                Hide();
            }
        }
    }

    private void List6_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        if (Interaction.MsgBox("Eintrag entfernen?", MsgBoxStyle.YesNo, "") == MsgBoxResult.Yes)
        {
            List6.Items.RemoveAt(List6.SelectedIndex);
        }
    }

    private void Option1_CheckedChanged(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_048a
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int index = default;
        byte b = default;
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
                            num = 1;
                            if (!NewLateBinding.LateGet(eventSender, null, "Checked", new object[0], null, null, null).AsBool())
                            {
                                goto end_IL_0000;
                            }
                            goto IL_0022;
                        case 1500:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_048e;
                                    default:
                                        goto end_IL_0000_2;
                                }
                                if (Information.Err().Number != 5)
                                {
                                    goto end_IL_0000;
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_048e;
                            }
                        end_IL_0000_2:
                            break;
                        IL_0022:
                            num = 2;
                            index = Option1.GetIndex((RadioButton)eventSender);
                            b = 0;
                            while (unchecked(b) <= 15u)
                            {
                                Label5[b].Text = "";
                                Label7[b].Text = "";
                                b = (byte)unchecked((uint)(b + 1));
                            }
                            b = 0;
                            while (unchecked(b) <= 2u)
                            {
                                Check2[b].Enabled = true;
                                b = (byte)unchecked((uint)(b + 1));
                            }
                            List1.Items.Clear();
                            Check2[4].Visible = true;
                            Check2[5].Visible = true;
                            Check3.Enabled = true;
                            Check3.Visible = true;
                            Check2[2].Visible = false;
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            Label4.Visible = false;
                            Label10.Visible = false;
                            Text2.Visible = false;
                            switch (index)
                            {
                                case 0:
                                    Check2[0].Visible = false;
                                    Check2[1].Visible = false;
                                    Check2[4].Visible = true;
                                    Check2[5].Visible = true;
                                    Label3.Text = "Name,Vorname                        Datum JJJJ  Personennr. Heirat       Partner";
                                    Check2[2].Visible = true;
                                    break;
                                case 1:
                                    goto IL_0205;
                                case 2:
                                    goto IL_02b3;
                                case 3:
                                case 4:
                                    goto IL_0324;
                                case 5:
                                    goto IL_037d;
                                case 6:
                                    goto IL_0396;
                                case 7:
                                    goto IL_03af;
                                default:
                                    break;
                            }
                            goto IL_0431;
                        IL_0205:
                            num = 32;
                            Label10.Visible = true;
                            Text2.Visible = true;
                            Label3.Text = "Name,Vorname                        Datum JJJJ  Personennr. Heirat       Partner";
                            Check3.Enabled = false;
                            b = 0;
                            while (unchecked(b) <= 2u)
                            {
                                Check2[b].Enabled = false;
                                b = (byte)unchecked((uint)(b + 1));
                            }
                            Label4.Visible = true;
                            Check2[4].Visible = false;
                            Check2[5].Visible = false;
                            goto IL_0431;
                        IL_02b3:
                            num = 44;
                            Check2[0].Visible = false;
                            Check2[1].Visible = false;
                            Check2[4].Visible = false;
                            Check2[5].Visible = false;
                            Label3.Text = "Name,Vorname                        Datum JJJJ  Personennr. Heirat       Partner";
                            goto IL_0431;
                        IL_0324:
                            num = 51;
                            Label10.Visible = true;
                            Text2.Visible = true;
                            Label3.Text = "Name,Vorname                        Datum JJJJ-MM-TT Ort               Personennummer";
                            Label4.Visible = true;
                            Check3.Visible = false;
                            goto IL_0431;
                        IL_037d:
                            num = 58;
                            Label3.Text = "Name,Vorname                        Datum JJJJ-MM-TT      Personennummer";
                            goto IL_0431;
                        IL_0396:
                            num = 61;
                            Label3.Text = "Name,Vorname                          Personennummer Generation  Nachfahrennummer";
                            goto IL_0431;
                        IL_03af:
                            num = 64;
                            Check2[0].Visible = false;
                            Check2[1].Visible = false;
                            Check2[2].Visible = false;
                            Check2[4].Visible = false;
                            Check2[5].Visible = false;
                            Label3.Text = "Name,Vorname                               Personennummer Generation  Ahnennummer";
                            goto IL_0431;
                        IL_0431: // <========== 9
                            num = 71;
                            ComboBox1.Focus();
                            ComboBox1.Text = "";
                            goto end_IL_0000;
                        IL_048e:
                            num4 = unchecked(num2 + 1);
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 21:
                                case 30:
                                case 42:
                                case 49:
                                case 56:
                                case 59:
                                case 62:
                                case 70:
                                case 71:
                                    goto IL_0431;
                                case 73:
                                case 76:
                                case 78:
                                case 79:
                                    goto end_IL_0000;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 1500;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000: // <========== 4
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void Perles1()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string text = default;
        short num4 = default;
        string left = default;
        int num6 = default;
        int num7 = default;
        int num10 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num5;
                    int num8;
                    int num9;
                    string LD;
                    int AAA;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 1903:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_05e1;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 3021)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_05e1;
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
                                num5 = num2;
                                goto IL_05e5;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            num4 = 0;
                            while (num4 <= 10)
                            {
                                _Modul1.Instance.Kont[num4] = "";
                                _Modul1.Instance.Kont1[num4] = "";
                                Vorn[num4] = 0;
                                num4 = (short)unchecked(num4 + 1);
                            }
                            if (_Modul1.Instance.PersInArb == 0)
                            {
                                goto end_IL_0000_2;
                            }
                            DataModul.DB_NameTable.Index = "PNamen";
                            DataModul.DB_NameTable.Seek("=", _Modul1.Instance.PersInArb);
                            if (DataModul.DB_NameTable.NoMatch)
                            {
                                goto end_IL_0000_2;
                            }
                            num4 = 1;
                            goto IL_00db;
                        IL_00db: // <========== 3
                            num = 16;
                            if (!DataModul.DB_NameTable.EOF)
                            {
                                if (!DataModul.DB_NameTable.NoMatch)
                                {
                                    if (!(DataModul.DB_NameTable.Fields[NameFields.PersNr].AsInt() != _Modul1.Instance.PersInArb))
                                    {
                                        if (!DataModul.DB_NameTable.NoMatch)
                                        {
                                            _Modul1.Instance.Kont1[0] = DataModul.DB_NameTable.Fields[NameFields.Kennz].AsString();
                                            _Modul1.Instance.Kont1[1] = DataModul.DB_NameTable.Fields[NameFields.Text].AsString();
                                            _Modul1.Instance.Kont1[2] = DataModul.DB_NameTable.Fields[NameFields.LfNr].AsString();
                                            _Modul1.Instance.Kont1[3] = DataModul.DB_NameTable.Fields[NameFields.Ruf].AsString();
                                            left = _Modul1.Instance.Kont1[0];
                                            if (left == "A")
                                            {
                                                num6 = _Modul1.Instance.Kont1[1].AsInt();
                                                goto IL_033c;
                                            }
                                            if (left == "B")
                                            {
                                                num7 = _Modul1.Instance.Kont1[1].AsInt();
                                                goto IL_033c;
                                            }
                                            if (left == "C")
                                            {
                                                num8 = _Modul1.Instance.Kont1[1].AsInt();
                                                goto IL_033c;
                                            }
                                            if (left == "D")
                                            {
                                                num9 = _Modul1.Instance.Kont1[1].AsInt();
                                                goto IL_033c;
                                            }
                                            if (left == "N")
                                            {
                                                num10 = _Modul1.Instance.Kont1[1].AsInt();
                                                goto IL_033c;
                                            }
                                            if (left == "V" || left == "F")
                                            {
                                                Vorn[_Modul1.Instance.Kont1[2].AsInt()] = _Modul1.Instance.Kont1[1].AsInt();
                                                Ruf[_Modul1.Instance.Kont1[2].AsInt()] = _Modul1.Instance.Kont1[3];
                                            }
                                            goto IL_033c;
                                        }
                                    }
                                }
                            }
                            goto IL_035b;
                        IL_033c: // <========== 7
                            num = 56;
                            DataModul.DB_NameTable.MoveNext();
                            num4 = (short)unchecked(num4 + 1);
                            if (num4 <= 99)
                            {
                                goto IL_00db;
                            }
                            goto IL_035b;
                        IL_035b: // <========== 4
                            num = 58;
                            AAA = num10;
                            //bBB = ref COND.Kont[0];
                            LD = "";
                            DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                            num10 = 0;
                            AAA = num6;
                            //bBB2 = ref COND.Kont[1];
                            LD = "";
                            DataModul.Textlese(AAA, out _Modul1.Instance.Kont[1], out LD);
                            num6 = 0;
                            AAA = num7;
                            //bBB3 = ref COND.Kont[2];
                            LD = "";
                            DataModul.Textlese(AAA, out _Modul1.Instance.Kont[2], out LD);
                            num7 = 0;
                            text = "";
                            num4 = 1;
                            goto IL_0418;
                        IL_0418: // <========== 3
                            num = 66;
                            if (Vorn[num4] != 0)
                            {
                                AAA = Vorn[num4];
                                DataModul.Textlese(AAA, out V, out this.LD);
                                if (Ruf[num4] == "1")
                                {
                                    V = "\"" + V.TrimEnd() + "\"";
                                }
                                text = text + V.TrimEnd() + " ";
                                if (this.LD.Length > 0)
                                {
                                    if (_Modul1.Instance.Aus[20] == "Y")
                                    {
                                        text = text + ">" + this.LD.Trim() + "< ";
                                    }
                                }
                                goto IL_0506;
                            }
                            goto IL_0517;
                        IL_0506: // <========== 3
                            num = 79;
                            num4 = (short)unchecked(num4 + 1);
                            if (num4 <= 15)
                            {
                                goto IL_0418;
                            }
                            goto IL_0517;
                        IL_0517: // <========== 3
                            num = 80;
                            _Modul1.Instance.Kont[3] = text;
                            goto end_IL_0000_2;
                        IL_05e1:
                            num5 = unchecked(num2 + 1);
                            goto IL_05e5;
                        IL_05e5:
                            num2 = 0;
                            switch (num5)
                            {
                                case 1:
                                    break;
                                case 16:
                                    goto IL_00db;
                                case 31:
                                case 35:
                                case 38:
                                case 41:
                                case 44:
                                case 47:
                                case 51:
                                case 52:
                                case 55:
                                case 56:
                                    goto IL_033c;
                                case 17:
                                case 20:
                                case 23:
                                case 54:
                                case 58:
                                    goto IL_035b;
                                case 66:
                                    goto IL_0418;
                                case 77:
                                case 78:
                                case 79:
                                    goto IL_0506;
                                case 67:
                                case 80:
                                    goto IL_0517;
                                case 82:
                                    num = 82;
                                    if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                                    {
                                        ProjectData.EndApp();
                                    }
                                    goto case 83;
                                case 83:
                                case 85:
                                    num = 85;
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    break;
                                case 8:
                                case 13:
                                case 81:
                                case 95:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 1903;
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
    public IList<int> Ehesuch(int persInArb, string Persex)
    {
        List<int> text = new();
        string left = Persex;
        switch (left)
        {
            case "M":
                _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                break;
            case "F":
                _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                break;
            default:
                return text;
        }
        byte b = 1;
        foreach (var cLink in DataModul.Link.ReadAllPers(persInArb, _Modul1.Instance.eLKennz))
        {
            text.Add(cLink.iFamNr);
            if (b++ > 99) break;
        }
        return text;
    }
    public void Datwand1(ref string Datu, ref string Ds)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        byte b = default;
        float num5 = default;
        string left = default;
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
                        goto IL_0008;
                    case 1149:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0377;
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
                            goto IL_037b;
                        }
                    end_IL_0000:
                        break;
                    IL_0008:
                        num = 2;
                        if (Datu.Length < 8)
                        {
                            Datu = "        " + Datu.Right(8);
                        }
                        if (Strings.Mid(Datu, 7, 2).AsDouble() == 0.0)
                        {
                            StringType.MidStmtStr(ref Datu, 7, 2, "  ");
                        }
                        if (Strings.Mid(Datu, 5, 2).AsDouble() == 0.0)
                        {
                            StringType.MidStmtStr(ref Datu, 5, 2, "  ");
                        }
                        Datu = Strings.Mid(Datu, 7, 2) + " " + Strings.Mid(Datu, 5, 2) + " " + Datu.Left(4);
                        Datu = Datu.TrimStart().TrimEnd();
                        if (Datu.Length > 0)
                        {
                            b = 1;
                            goto IL_00f2;
                        }
                        goto IL_0136;
                    IL_00f2: // <========== 3
                        num = 15;
                        num5 = Strings.InStr(Datu, " ");
                        if (num5 > 0f)
                        {
                            StringType.MidStmtStr(ref Datu, checked((int)Math.Round(num5)), 1, ".");
                        }
                        goto IL_0129;
                    IL_0129:
                        num = 19;
                        b = checked((byte)unchecked((uint)(b + 1)));
                        if (b <= 2u)
                        {
                            goto IL_00f2;
                        }
                        goto IL_0136;
                    IL_0136: // <========== 3
                        num = 21;
                        left = Ds;
                        if (left == "U" || left == "u")
                        {
                            Datu = "um " + Datu;
                            goto IL_031e;
                        }
                        if (left == "V" || left == "v")
                        {
                            Datu = "vor " + Datu;
                            goto IL_031e;
                        }
                        if (left == "N" || left == "n")
                        {
                            Datu = "nach " + Datu;
                            goto IL_031e;
                        }
                        if (left == "?")
                        {
                            Datu += " ?";
                            goto IL_031e;
                        }
                        if (left == "R" || left == "r")
                        {
                            Datu = "errech. " + Datu;
                            goto IL_031e;
                        }
                        if (left == "Z" || left == "z")
                        {
                            Datu = "zwischen " + Datu;
                            goto IL_031e;
                        }
                        if (left == "a" || left == "A")
                        {
                            Datu = " und " + Datu;
                            goto IL_031e;
                        }
                        if (left == "b" || left == "B")
                        {
                            Datu = " bis " + Datu;
                            goto IL_031e;
                        }
                        if (Beruf == 0)
                        {
                            if (Datu.Length == 10)
                            {
                                Datu = "am " + Datu;
                            }
                        }
                        goto IL_031e;
                    IL_031e: // <========== 11
                        num = 55;
                        Ds = "";
                        goto end_IL_0000_2;
                    IL_0377:
                        num4 = num2 + 1;
                        goto IL_037b;
                    IL_037b:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 15:
                                goto IL_00f2;
                            case 18:
                            case 19:
                                goto IL_0129;
                            case 20:
                            case 21:
                                goto IL_0136;
                            case 22:
                            case 26:
                            case 29:
                            case 32:
                            case 35:
                            case 38:
                            case 41:
                            case 44:
                            case 47:
                            case 52:
                            case 53:
                            case 54:
                            case 55:
                                goto IL_031e;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 1149;
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
    public void Datles1()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string QuText = default;
        int number = default;
        float num5 = default;
        float num6 = default;
        short num7 = default;
        float num8 = default;
        float num9 = default;
        float num10 = default;
        int num11 = default;
        string Ds = default;
        float num12 = default;
        EEventArt Art = default;
        int lErl = default;
        string left = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    short Schalt;
                    int Ortnr;
                    string LD;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 6754:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_16a8;
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
                                goto IL_16ac;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            Beruf = 0;
                            _Modul1.Instance.Datum1 = "";
                            _Modul1.Instance.Datum2 = "";
                            num5 = 0f;
                            num6 = 0f;
                            num7 = 1;
                            while (num7 <= 50)
                            {
                                _Modul1.Instance.Kont[num7] = "";
                                num7 = (short)unchecked(num7 + 1);
                            }
                            num8 = 101f;
                            goto IL_0070;
                        IL_0070: // <========== 3
                                 // <========== 3
                            num = 11;
                            num9 = 0f;
                            while (num9 <= 20f)
                            {
                                _Modul1.Instance.Kont1[(int)Math.Round(num9)] = "";
                                num9 += 1f;
                            }
                            _Modul1.Instance.Ubg = (int)Math.Round(num8);
                            _Modul1.Instance.Art = (EEventArt)_Modul1.Instance.Ubg;
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", _Modul1.Instance.Ubg.AsString(), _Modul1.Instance.PersInArb.AsString(), "0");
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                if ((num10 == 2f) & (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() == num11))
                                {
                                    num10 = 3f;
                                    goto end_IL_0000_2;
                                }
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    Datu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                    if (num10 == 1f)
                                    {
                                        _Modul1.Instance.Kont[_Modul1.Instance.Ubg - 100] = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                        goto IL_15a5;
                                    }
                                    else if (num10 == 10f)
                                    {
                                        _Modul1.Instance.Kont[_Modul1.Instance.Ubg - 100] = DataModul.DB_EventTable.Fields[EventFields.Reg].AsString();
                                        _Modul1.Instance.Kont[_Modul1.Instance.Ubg - 90] = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                        goto IL_15a5;
                                    }
                                    else
                                    {
                                        Ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                        Datwand1(ref Datu, ref Ds);
                                        _Modul1.Instance.Kont1[1] = Datu;
                                        if (_Modul1.Instance.Ubg == 101)
                                        {
                                            _Modul1.Instance.Datum2 = "           " + Datu.Right(10);
                                            num5 = 1f;
                                        }
                                        if (unchecked(_Modul1.Instance.Ubg == 102 && num5 == 0f))
                                        {
                                            _Modul1.Instance.Datum2 = "           " + Datu.Right(10);
                                        }
                                        if (_Modul1.Instance.Ubg == 103)
                                        {
                                            _Modul1.Instance.Datum1 = "           " + Datu.Right(10);
                                            num6 = 1f;
                                        }
                                        if (unchecked(_Modul1.Instance.Ubg == 104 && num6 == 0f))
                                        {
                                            _Modul1.Instance.Datum1 = "           " + Datu.Right(10);
                                        }
                                    }

                                }
                                goto IL_03d5;
                            }
                            goto IL_15a5;
                        IL_03d5:
                            // <========== 3
                            num = 54;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                Datu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                Ds = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                Datwand1(ref Datu, ref Ds);
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString().Trim(), "", TextCompare: false) != 0)
                                    _Modul1.Instance.Kont1[3] = " " + Datu;
                                else
                                {
                                    _Modul1.Instance.Kont1[3] = " / " + Datu;
                                }
                            }
                            goto IL_04d2;
                        IL_04d2: // <========== 3
                                 // <========== 3
                            num = 65;
                            _Modul1.Instance.UbgT = "";
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                            {
                                Kont2[6] = "";
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Zusatz].Value))
                                {
                                    Kont2[6] = DataModul.DB_EventTable.Fields[EventFields.Zusatz].AsString();
                                }
                                Ortnr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble());
                                Schalt = 0;
                                ortles(ref Ortnr, ref Schalt);
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    _Modul1.Instance.UbgT = _Modul1.Instance.UbgT.TrimEnd() + " " + DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString().Trim();
                                }
                            }
                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].Value.AsDouble() > 0.0)
                            {
                                Ortnr = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                //kontU = ref KontU;
                                LD = "";
                                DataModul.Textlese(Ortnr, out KontU, out LD);
                                _Modul1.Instance.Kont1[6] = " " + KontU.Trim() + " ";
                            }
                            if (DataModul.DB_EventTable.Fields[EventFields.Platz].Value.AsDouble() > 0.0)
                            {
                                Ortnr = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                LD = "";
                                DataModul.Textlese(Ortnr, out KontU, out LD);
                                _Modul1.Instance.UbgT = _Modul1.Instance.UbgT + " " + KontU.Trim() + " ";
                            }
                            if (_Modul1.Instance.Aus[34] == "Y")
                            {
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Reg].Value))
                                {
                                    if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Reg].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        _Modul1.Instance.UbgT = _Modul1.Instance.UbgT + " (Urk.-Nr.: " + DataModul.DB_EventTable.Fields[EventFields.Reg].AsString().Trim() + ") ";
                                    }
                                }
                            }
                            if (DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().TrimEnd() == "" & DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().TrimEnd() == "")
                            {
                                _Modul1.Instance.Kont[_Modul1.Instance.Ubg - 90] = _Modul1.Instance.Kont1[1] + _Modul1.Instance.Kont1[2] + _Modul1.Instance.Kont1[3] + _Modul1.Instance.Kont1[4] + _Modul1.Instance.Kont1[5] + _Modul1.Instance.Kont1[6] + " " + _Modul1.Instance.UbgT;
                                _Modul1.Instance.UbgT = "";
                            }
                            else
                            {
                                _Modul1.Instance.Kont[_Modul1.Instance.Ubg - 90] = _Modul1.Instance.Kont1[1] + _Modul1.Instance.Kont1[2] + _Modul1.Instance.Kont1[3] + _Modul1.Instance.Kont1[4] + _Modul1.Instance.Kont1[5] + _Modul1.Instance.Kont1[6] + " " + _Modul1.Instance.UbgT;
                                _Modul1.Instance.UbgT = "";
                                if ((_Modul1.Instance.Aus[2] == "Y") & (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0))
                                {
                                    _Modul1.Instance.Kont[_Modul1.Instance.Ubg - 85] = "{" + DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().TrimEnd() + "}";
                                }
                                if ((_Modul1.Instance.Aus[3] == "Y") & (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0))
                                {
                                    _Modul1.Instance.Kont[_Modul1.Instance.Ubg - 80] = "{" + DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().TrimEnd() + "}";
                                }
                            }
                            DataModul.DB_SourceLinkTable.Index = "Tab22";
                            DataModul.DB_SourceLinkTable.Seek("=", 3, _Modul1.Instance.PersInArb, _Modul1.Instance.Ubg, 0);
                            QuText = "";
                            while (!DataModul.DB_SourceLinkTable.EOF && !DataModul.DB_SourceLinkTable.NoMatch && !Conversions.ToBoolean((DataModul.DB_SourceLinkTable.Fields[0].AsInt() != 3) | (DataModul.DB_SourceLinkTable.Fields[1].AsInt() > _Modul1.Instance.PersInArb) | (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsInt() != _Modul1.Instance.Ubg) | (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.LfNr].AsInt() != 0)))
                            {
                                DataModul.DB_QuTable.Index = "NR";
                                DataModul.DB_QuTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3]);
                                if (!DataModul.DB_QuTable.NoMatch)
                                {
                                    if (_Modul1.Instance.Kont[_Modul1.Instance.Ubg - 70] == "")
                                    {
                                        _Modul1.Instance.Kont[_Modul1.Instance.Ubg - 70] = "Quellen: ";
                                    }
                                    if (QuText == "") QuText = DataModul.DB_QuTable.Fields[QuFields._2].AsString();
                                    else
                                    {
                                        QuText = (QuText + "; " + DataModul.DB_QuTable.Fields[QuFields._2].AsString()).AsString();
                                    }
                                    if (Operators.CompareString(DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        if (Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString()))
                                            left = " Seiten: ";
                                        else
                                        {
                                            if (Operators.CompareString(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim(), "", TextCompare: false) == 0)
                                                left = " Seiten: ";
                                            else
                                            {
                                                left = " " + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim() + " ";
                                            }
                                        }
                                        QuText = (QuText + left + DataModul.DB_SourceLinkTable.Fields[3].AsInt()).AsString();
                                    }
                                    num = 140;
                                    if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString()))
                                    {
                                        if (Operators.ConditionalCompareObjectNotEqual(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString(), "", TextCompare: false))
                                        {
                                            QuText = (QuText + " >" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value + "<").AsString();
                                        }
                                    }
                                    if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value))
                                    {
                                        if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString() != "")
                                        {
                                            QuText = (QuText + " ==" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value + "==").AsString();
                                        }
                                    }
                                    Zeiweg(ref QuText);
                                }
                                DataModul.DB_SourceLinkTable.MoveNext();
                            }
                            if (QuText.Trim() != "")
                            {
                                Zeiweg(ref QuText);
                                _Modul1.Instance.Kont[_Modul1.Instance.Ubg - 70] = "Quellen: " + QuText.Trim();
                                QuText = "";
                            }
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString()))
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    _Modul1.Instance.Kont1[9] = DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim();
                                    Zeiweg(ref _Modul1.Instance.Kont1[9]);
                                }
                                if (_Modul1.Instance.Kont1[9].Trim() != "")
                                {
                                    if (_Modul1.Instance.Kont[_Modul1.Instance.Ubg - 70] != "")
                                        _Modul1.Instance.Kont[_Modul1.Instance.Ubg - 70] = _Modul1.Instance.Kont[_Modul1.Instance.Ubg - 70] + "; " + _Modul1.Instance.Kont1[9].Trim() + ".";
                                    else
                                    {
                                        _Modul1.Instance.Kont[_Modul1.Instance.Ubg - 70] = "Quellen: " + _Modul1.Instance.Kont1[9].Trim() + ".";
                                    }
                                }
                            }
                            goto IL_11c6;
                        IL_11c6: // <========== 3
                                 // <========== 4
                            num = 174;
                            PersSp = _Modul1.Instance.PersInArb;
                            num12 = 1f;
                            while (num12 <= 100f)
                            {
                                KontSP1[(int)Math.Round(num12)] = _Modul1.Instance.Kont1[(int)Math.Round(num12)];
                                KontSP[(int)Math.Round(num12)] = _Modul1.Instance.Kont[(int)Math.Round(num12)];
                                _Modul1.Instance.Kont[(int)Math.Round(num12)] = "";
                                _Modul1.Instance.Kont1[(int)Math.Round(num12)] = "";
                                num12 += 1f;
                            }
                            _Modul1.Instance.PersInArb = PersSp;
                            _Modul1.Instance.UbgT1 = "";
                            LfNR = 0;
                            Art = _Modul1.Instance.Art;
                            if (_Modul1.Instance.Aus[32] == "Y")
                            {
                                Zeugsu(Art);
                                _Modul1.Instance.PersInArb = PersSp;
                                _Modul1.Instance.Art = (EEventArt)Math.Round(num8);
                                DataModul.DB_EventTable.Seek("=", num8.AsString(), _Modul1.Instance.PersInArb.AsString(), "0");
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString()))
                                {
                                    if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        if (_Modul1.Instance.UbgT1.Trim() == "")
                                            _Modul1.Instance.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim();
                                        else
                                        {
                                            _Modul1.Instance.UbgT1 = _Modul1.Instance.UbgT1.Trim() + "; " + DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim();
                                        }
                                    }
                                }
                            }
                            goto IL_1486;
                        IL_1486: // <========== 3
                                 // <========== 4
                            num = 201;
                            _Modul1.Instance.PersInArb = PersSp;
                            _Modul1.Instance.Ubg = (int)_Modul1.Instance.Art;
                            num12 = 1f;
                            while (num12 <= 100f)
                            {
                                _Modul1.Instance.Kont1[(int)Math.Round(num12)] = KontSP1[(int)Math.Round(num12)];
                                _Modul1.Instance.Kont[(int)Math.Round(num12)] = KontSP[(int)Math.Round(num12)];
                                KontSP[(int)Math.Round(num12)] = "";
                                KontSP1[(int)Math.Round(num12)] = "";
                                num12 += 1f;
                            }
                            if (_Modul1.Instance.UbgT1.Trim() != "")
                            {
                                _Modul1.Instance.Kont[(int)_Modul1.Instance.Art - 60] = "Zeugen: " + _Modul1.Instance.UbgT1.Trim() + ".";
                            }
                            goto IL_15a5;
                        IL_15a5: // <========== 5
                                 // <========== 5
                            num = 212;
                            lErl = 2;
                            num8 += 1f;
                            if (!(num8 <= 107f))
                            {
                                goto end_IL_0000_2;
                            }
                            goto IL_0070;
                        IL_16a8:
                            num4 = unchecked(num2 + 1);
                            goto IL_16ac;
                        IL_16ac:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 11:
                                    goto IL_0070;
                                case 52:
                                case 53:
                                case 54:
                                    goto IL_03d5;
                                case 60:
                                case 63:
                                case 64:
                                case 65:
                                    goto IL_04d2;
                                case 99:
                                case 100:
                                case 104:
                                case 105:
                                case 130:
                                case 133:
                                case 134:
                                case 137:
                                case 138:
                                case 139:
                                case 140:
                                case 151:
                                case 152:
                                case 153:
                                case 108:
                                case 109:
                                case 154:
                                case 112:
                                case 155:
                                case 168:
                                case 171:
                                case 172:
                                case 173:
                                case 174:
                                    goto IL_11c6;
                                case 194:
                                case 197:
                                case 198:
                                case 199:
                                case 200:
                                case 201:
                                    goto IL_1486;
                                case 19:
                                case 29:
                                case 34:
                                case 211:
                                case 212:
                                    goto IL_15a5;
                                case 219:
                                case 220:
                                    num = 220;
                                    number = Information.Err().Number;
                                    goto case 222;
                                case 222:
                                case 223:
                                    num = 223;
                                    if (number == 94)
                                    {
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        goto IL_16a8;
                                    }
                                    goto case 227;
                                case 227:
                                case 228:
                                    num = 228;
                                    if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                                    {
                                        ProjectData.EndApp();
                                    }
                                    goto case 229;
                                case 229:
                                case 23:
                                case 214:
                                case 221:
                                case 225:
                                case 226:
                                case 232:
                                case 233:
                                case 234:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 6754;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 3
                       // <========== 3
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void ortles(ref int Ortnr, ref short Schalt)
    {
        //Discarded unreachable code: IL_04d4
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        byte b = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                int AAA;
                string LD;
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0008;
                    case 1438:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_04d8;
                                default:
                                    goto end_IL_0000;
                            }
                            if (Information.Err().Number != 94)
                            {
                                goto end_IL_0000_2;
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_04d8;
                        }
                    end_IL_0000:
                        break;
                    IL_0008:
                        num = 2;
                        b = 1;
                        while (b <= 5u)
                        {
                            Kont2[b] = "";
                            b = checked((byte)unchecked((uint)(b + 1)));
                        }
                        b = 7;
                        while (b <= 10u)
                        {
                            Kont2[b] = "";
                            b = checked((byte)unchecked((uint)(b + 1)));
                        }
                        DataModul.DB_PlaceTable.Index = "OrtNr";
                        DataModul.DB_PlaceTable.Seek("=", Ortnr);
                        if (!DataModul.DB_PlaceTable.NoMatch)
                        {
                            AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt();
                            LD = "";
                            DataModul.Textlese(AAA, out Kont2[1], out LD);
                            AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt();
                            //bBB2 = ref Kont2[2];
                            LD = "";
                            DataModul.Textlese(AAA, out Kont2[2], out LD);
                            if (!Information.IsDBNull(DataModul.DB_PlaceTable.Fields[PlaceFields.PolName].Value))
                            {
                                AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.PolName].AsInt();
                                //bBB3 = ref Kont2[7];
                                LD = "";
                                DataModul.Textlese(AAA, out Kont2[7], out LD);
                                if (Kont2[7] != "")
                                {
                                    Kont2[7] = " (" + Kont2[7] + ")";
                                }
                            }
                            goto IL_020a;
                        }
                        _Modul1.Instance.UbgT = "";
                        goto end_IL_0000_2;
                    IL_020a: // <========== 3
                        num = 19;
                        if (Kont2[6] == "")
                        {
                            if (!Information.IsDBNull(DataModul.DB_PlaceTable.Fields[PlaceFields.Zusatz].Value))
                            {
                                Kont2[6] = DataModul.DB_PlaceTable.Fields[PlaceFields.Zusatz].AsString();
                            }
                            goto IL_027a;
                        }
                        goto IL_02aa;
                    IL_027a:
                        num = 23;
                        if (Kont2[6].Trim() == "")
                        {
                            Kont2[6] = "in";
                        }
                        goto IL_02aa;
                    IL_02aa: // <========== 3
                        num = 27;
                        if (_Modul1.Instance.Aus[35] != "Y")
                        {
                            if (Schalt == 0)
                            {
                                AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Kreis].AsInt();
                                //bBB4 = ref Kont2[3];
                                LD = "";
                                DataModul.Textlese(AAA, out Kont2[3], out LD);
                                AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Land].AsInt();
                                //bBB5 = ref Kont2[4];
                                LD = "";
                                DataModul.Textlese(AAA, out Kont2[4], out LD);
                                AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Staat].AsInt();
                                //var bBB6 = ref Kont2[5];
                                LD = "";
                                DataModul.Textlese(AAA, out Kont2[5], out LD);
                            }
                        }
                        goto IL_03ae;
                    IL_03ae: // <========== 3
                        num = 34;
                        if (Kont2[2] != "")
                        {
                            Kont2[2] = "-" + Kont2[2];
                        }
                        goto IL_03e6;
                    IL_03e6:
                        num = 37;
                        _Modul1.Instance.UbgT = (Kont2[6] + " " + Kont2[1].TrimEnd() + Kont2[2].TrimEnd() + Kont2[7] + " " + Kont2[3].TrimEnd() + " " + Kont2[4].TrimEnd() + " " + Kont2[5].TrimEnd()).Trim();
                        goto end_IL_0000_2;
                    IL_04d8:
                        num4 = num2 + 1;
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 17:
                            case 18:
                            case 19:
                                goto IL_020a;
                            case 22:
                            case 23:
                                goto IL_027a;
                            case 25:
                            case 26:
                            case 27:
                                goto IL_02aa;
                            case 32:
                            case 33:
                            case 34:
                                goto IL_03ae;
                            case 36:
                            case 37:
                                goto IL_03e6;
                            case 38:
                            case 41:
                            case 42:
                            case 45:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 1438;
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
    //public void DataModul.Textlese(ref int AAA, ref string BBB, ref string LD)
    //{
    //    LD = "";
    //    if (AAA == 0)
    //    {
    //        BBB = "";
    //        return;
    //    }
    //    LD = "";
    //    DataModul.DB_TexteTable.Index = "TxNr1";
    //    DataModul.DB_TexteTable.Seek("=", AAA);
    //    if (!DataModul.DB_TexteTable.NoMatch)
    //    {
    //        BBB = DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString();
    //        if (!Information.IsDBNull(DataModul.DB_TexteTable.Fields[TexteFields.Leitname].Value))
    //        {
    //            LD = DataModul.DB_TexteTable.Fields[TexteFields.Leitname].AsString().Trim();
    //        }
    //        BBB = BBB.Replace("ssss", "ß");
    //        LD = LD.Replace("ssss", "ß");
    //    }
    //}

    public void Ahnles()
    {
        int try0000_dispatch = -1;
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
                int num4;
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0007;
                    case 1127:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_03ab;
                                default:
                                    goto end_IL_0000;
                            }
                            if (Information.Err().Number == 91)
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
                            goto IL_03ae;
                        }
                    end_IL_0000:
                        break;
                    IL_0007:
                        num = 2;
                        DataModul.DT_AncesterTable.Index = "PerNr";
                        _Modul1.Instance.Kont[20] = "";
                        _Modul1.Instance.Kont[10] = "";
                        _Modul1.Instance.Kont[11] = "";
                        DataModul.DT_AncesterTable.Seek("=", _Modul1.Instance.PersInArb);
                        goto IL_009c;
                    IL_009c: // <========== 3
                        num = 7;
                        lErl = 1;
                        if (!DataModul.DT_AncesterTable.NoMatch)
                        {
                            if (DataModul.DT_AncesterTable.Fields["Ahn"].AsInt() > 0)
                            {
                                if (!(DataModul.DT_AncesterTable.Fields["PerNr"].AsInt() != _Modul1.Instance.PersInArb))
                                {
                                    if (DataModul.DT_AncesterTable.Fields["Weiter"].AsInt() != 0)
                                    {
                                        _Modul1.Instance.Kont[20] = ">>";
                                    }
                                    if (_Modul1.Instance.Kont[11] == "")
                                        _Modul1.Instance.Kont[11] = DataModul.DT_AncesterTable.Fields["Ahn"].AsString();
                                    else
                                    {
                                        _Modul1.Instance.Kont[11] = _Modul1.Instance.Kont[11] + "; " + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim();
                                    }
                                    goto IL_01d2;
                                }
                            }
                        }
                        goto IL_0258;
                    IL_01d2: // <========== 3
                        num = 22;
                        _Modul1.Instance.Kont[10] = "Generation " + DataModul.DT_AncesterTable.Fields["Gen"].AsString() + " Ahn-Nr.: " + _Modul1.Instance.Kont[11].Trim();
                        if (DataModul.DT_AncesterTable.Fields["Weiter"].AsInt() != 0)
                        {
                            DataModul.DT_AncesterTable.MoveNext();
                            goto IL_009c;
                        }
                        goto IL_0258;
                    IL_0258: // <========== 4
                        num = 29;
                        lErl = 2;
                        DataModul.DT_DescendentTable.Index = "PerNr";
                        DataModul.DT_DescendentTable.Seek("=", _Modul1.Instance.PersInArb);
                        if (DataModul.DT_DescendentTable.NoMatch)
                        {

                        }
                        else
                        {
                            _Modul1.Instance.Kont[13] = Conversions.ToString(string.Concat(_Modul1.Instance.IText[239] + " ", DataModul.DT_DescendentTable.Fields["Gen"].AsString()) + "-" + DataModul.DT_DescendentTable.Fields["Nr"].Value);
                        }
                        goto end_IL_0000_2;
                    IL_03ab:
                        num4 = num2 + 1;
                        goto IL_03ae;
                    IL_03ae:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 7:
                            case 25:
                                goto IL_009c;
                            case 18:
                            case 21:
                            case 22:
                                goto IL_01d2;
                            case 11:
                            case 26:
                            case 27:
                            case 28:
                            case 29:
                                goto IL_0258;
                            case 34:
                            case 35:
                            case 37:
                            case 43:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 1127;
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
    public void kindsuch()
    {
        int try0000_dispatch = -1;
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
                int num4;
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0007;
                    case 1133:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_03b1;
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
                            goto IL_03b4;
                        }
                    end_IL_0000:
                        break;
                    IL_0007:
                        num = 2;
                        byte b = 1;
                        foreach (var cLink in DataModul.Link.ReadAllFams(_Modul1.Instance.FamInArb, ELinkKennz.lkChild))
                        {
                            string text = "";
                            _Modul1.Instance.PersInArb = cLink.iPersNr;
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", "101", _Modul1.Instance.PersInArb.AsString(), "0");
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                DataModul.DB_EventTable.Seek("=", "102", _Modul1.Instance.PersInArb.AsString(), "0");
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    text = "00000000";
                                }
                            }
                            else
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumV].AsInt() == 0)
                            {
                                DataModul.DB_EventTable.Seek("=", "102", _Modul1.Instance.PersInArb.AsString(), "0");
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    lErl = 1;
                                    text = "00000000";
                                }
                            }
                            else
                                text = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                            lErl = 2;
                            List4.Items.Add(text + _Modul1.Instance.PersInArb.AsString());
                            b++;
                        }
                        goto end_IL_0000_2;
                    IL_03b1:
                        num4 = num2 + 1;
                        goto IL_03b4;
                    IL_03b4:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 5:
                            case 9:
                            case 12:
                            case 38:
                            case 43:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 1133;
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
    public void Listfuell()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        int persInArb = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                int Ortnr;
                short Schalt;
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0008;
                    case 9551:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_1fad;
                                default:
                                    goto end_IL_0000;
                            }
                            int number = Information.Err().Number;
                            if (number == 9)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1fad;
                            }
                            if (number == 3021)
                            {
                                goto IL_1ceb;
                            }
                            if (number == 94)
                            {
                                DataModul.DSB_SearchTable.Edit();
                                DataModul.DSB_SearchTable.Fields["Kenn"].Value = "9";
                                DataModul.DSB_SearchTable.Update();
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1fa9;
                            }
                            if (number == 3421)
                            {
                                Option1[1].Checked = true;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num2 = 0;
                                goto IL_012f;
                            }
                            if (number != 3167)
                            {
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
                            goto IL_1fa9;
                        }
                    end_IL_0000:
                        break;
                    IL_0008:
                        num = 2;
                        if (_Modul1.Instance.Aus[12] == "")
                        {
                            _Modul1.Instance.Aus[12] = "200";
                        }
                        if (_Modul1.Instance.Aus[13] == "")
                        {
                            _Modul1.Instance.Aus[13] = "100";
                        }
                        if (ComboBox1.Text == "")
                        {
                            if (ComboBox1.Text[0].AsString() != "")
                            {
                                ComboBox1.Text = ComboBox1.Items[0].AsString();
                                Text1.Text = ComboBox1.Text[0].AsString();

                            }
                        }
                        goto IL_00f3;
                    IL_00f3: // <========== 3
                             // <========== 3
                        num = 15;
                        if (ComboBox1.Text != "")
                        {
                            ComboBox1.Text = Text1.Text;
                            goto IL_012f;
                        }
                        goto IL_1ceb;
                    IL_012f: // <========== 3
                             // <========== 3
                        num = 19;
                        lErl = 1;
                        if (Text1.Text != "")
                        {
                            if (0 - (Option1[2].Checked ? 1 : 0) == -1)
                            {
                                DataModul.DSB_SearchTable.Index = "Nummer";
                                if (Text1.Text == "")
                                {
                                    Text1.Text = "0";
                                }
                                DataModul.DSB_SearchTable.Seek(">=", Text1.Text.AsDouble());
                                if (Check3.CheckState == CheckState.Unchecked)
                                {
                                    Zeigfam();
                                }
                                else
                                {

                                    Zeig();
                                }
                                goto end_IL_0000_2;
                            }
                            if (0 - (RadioButton5.Checked ? 1 : 0) == -1)
                            {
                                DataModul.DSB_SearchTable.Index = "Leitsuch";
                                DataModul.DSB_SearchTable.Seek(">=", Text1.Text, 0);
                                if (Check3.CheckState == CheckState.Unchecked)
                                {
                                    Zeigfam();
                                }
                                else
                                {

                                    Zeig();
                                }
                                goto end_IL_0000_2;
                            }
                            if (0 - (RadioButton3.Checked ? 1 : 0) == -1)
                            {
                                DataModul.DSB_SearchTable.Index = "K_Phonsuch";
                                Module2.Koelner_Phonetic(Text1.Text);
                                DataModul.DSB_SearchTable.Seek(">=", Module2.Endwert, 0);
                                if (Check3.CheckState == CheckState.Unchecked)
                                {
                                    Zeigfam();
                                }
                                else
                                {

                                    Zeig();
                                }
                                goto end_IL_0000_2;
                            }
                            if (0 - (RadioButton4.Checked ? 1 : 0) == -1)
                            {
                                Module2.sCode = "";
                                Module2.GetSoundEx(Text1.Text);
                                DataModul.DSB_SearchTable.Index = "Soundsuch";
                                DataModul.DSB_SearchTable.Seek(">=", Module2.sCode, 0);
                                if (Check3.CheckState == CheckState.Unchecked)
                                {
                                    Zeigfam();
                                }
                                else
                                {

                                    Zeig();
                                }
                                goto end_IL_0000_2;
                            }
                            if (0 - (Option1[0].Checked ? 1 : 0) == -1)
                            {
                                DataModul.DSB_SearchTable.Index = "Persuch";
                                DataModul.DSB_SearchTable.Seek(">=", Text1.Text, 0);
                                if (Check3.CheckState == CheckState.Unchecked)
                                {
                                    Zeigfam();
                                }
                                else
                                {

                                    Zeig();
                                }
                                goto end_IL_0000_2;
                            }
                            if (0 - (Option1[1].Checked ? 1 : 0) == -1)
                            {
                                DataModul.DSB_SearchTable.Index = "Persuch";
                                DataModul.DSB_SearchTable.Seek(">=", Text1.Text, 0);
                                Zeigfamdat();
                                goto end_IL_0000_2;
                            }
                            int num6;
                            int num7;
                            string text;
                            string str;
                            string text2;
                            string item;
                            if (0 - (Option1[3].Checked ? 1 : 0) == -1)
                            {
                                Listleer();
                                num6 = checked((int)Math.Round((Text1.Text + "0000".Left(8)).AsDouble()));
                                if (num6 <= 0)
                                {
                                    num6 = 1;
                                }
                                DataModul.DB_EventTable.Index = "DatInd";
                                DataModul.DB_EventTable.Seek(">=", num6);
                                num7 = 0;
                                while (!DataModul.DB_EventTable.EOF
                                    && !DataModul.DB_EventTable.NoMatch
                                    && Conversions.ToBoolean(
                                              (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == 103)
                                              | (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == 104)))
                                {
                                    if (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == 103)
                                    {
                                        Kennzt = "+";
                                    }
                                    if (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == 104)
                                    {
                                        Kennzt = "/";
                                    }
                                    _Modul1.Instance.PersInArb = DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt();
                                    _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                                    if (!(Check2[5].Checked
                                        & (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "M")))
                                    {
                                        if (!(Check2[4].Checked
                                        & (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")))
                                        {
                                            if (_Modul1.Instance.PersInArb != persInArb)
                                            {
                                                persInArb = _Modul1.Instance.PersInArb;
                                                _Modul1.Instance.UbgT = "";
                                                if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                                                {
                                                    Ortnr = checked((int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble()));
                                                    Schalt = 1;
                                                    ortles(ref Ortnr, ref Schalt);
                                                }
                                                text = " " + _Modul1.Instance.UbgT + "                    ".Left(17);
                                                str = Strings.Right("        " + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString(), 8);
                                                text2 = Kennzt + str.Left(4) + "-" + Strings.Mid(str, 5, 2) + "-" + Strings.Mid(str, 7, 2) + text;
                                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                                if (Text2.Text == "")
                                                {
                                                    if (_Modul1.Instance.Kont[1].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0];
                                                    }
                                                    item = Strings.Left(_Modul1.Instance.Kont[0] + "," + _Modul1.Instance.Kont[3] + "                                          ", 40) + text2 + "    " + Strings.Right("          " + DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsString(), 10);
                                                    num7 = checked(num7 + 1);
                                                    List1.Items.Add(item);
                                                    if (num7 == _Modul1.Instance.Aus[12].AsDouble())
                                                    {
                                                        break;
                                                    }
                                                }
                                                else
                                                if (Operators.CompareString(Strings.Left(_Modul1.Instance.Kont[0].ToUpper().Trim(), Text2.Text.Length), Text2.Text.ToUpper(), TextCompare: false) == 0)
                                                {
                                                    num7 = checked(num7 + 1);
                                                    if (_Modul1.Instance.Kont[1].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0];
                                                    }
                                                    item = Strings.Left(_Modul1.Instance.Kont[0] + "," + _Modul1.Instance.Kont[3] + "                                          ", 40) + text2 + "    " + Strings.Right("          " + DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsString(), 10);
                                                    num7 = checked(num7 + 1);
                                                    List1.Items.Add(item);
                                                    if (num7 == _Modul1.Instance.Aus[12].AsDouble())
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    DataModul.DB_EventTable.MoveNext();
                                }
                            }
                            if (0 - (Option1[4].Checked ? 1 : 0) == -1)
                            {
                                Listleer();
                                num6 = checked((int)Math.Round((Text1.Text + "0000".Left(8)).AsDouble()));
                                if (num6 <= 0)
                                {
                                    num6 = 1;
                                }
                                DataModul.DB_EventTable.Index = "DatInd";
                                DataModul.DB_EventTable.Seek(">=", num6);
                                num7 = 0;
                                while (!DataModul.DB_EventTable.EOF && !DataModul.DB_EventTable.NoMatch)
                                {
                                    if (
                                          (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == 101)
                                          | (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == 102))
                                    {
                                        if (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == 101)
                                        {
                                            Kennzt = "*";
                                        }
                                        if (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == 102)
                                        {
                                            Kennzt = "~";
                                        }
                                        _Modul1.Instance.PersInArb = DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt();
                                        _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                                        if (!(Check2[5].Checked
                                        & (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "M")))
                                        {
                                            if (!(Check2[4].Checked
                                        & (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")))
                                            {
                                                if (_Modul1.Instance.PersInArb != persInArb)
                                                {
                                                    persInArb = _Modul1.Instance.PersInArb;
                                                    _Modul1.Instance.UbgT = "";
                                                    if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                                                    {
                                                        Ortnr = checked((int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble()));
                                                        Schalt = 1;
                                                        ortles(ref Ortnr, ref Schalt);
                                                    }
                                                    text = " " + _Modul1.Instance.UbgT + "                    ".Left(17);
                                                    str = Strings.Right("         " + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString(), 9);
                                                    text2 = Kennzt + str.Left(5) + "-" + Strings.Mid(str, 6, 2) + "-" + Strings.Mid(str, 8, 2) + text;
                                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                                    if (Text2.Text == "")
                                                    {
                                                        num7 = checked(num7 + 1);
                                                        if (_Modul1.Instance.Kont[1].Trim() != "")
                                                        {
                                                            _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0];
                                                        }
                                                        item = Strings.Left(_Modul1.Instance.Kont[0] + "," + _Modul1.Instance.Kont[3] + "                                          ", 40) + text2 + "    " + Strings.Right("          " + DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsString(), 10);
                                                        List1.Items.Add(item);
                                                        if (num7 == _Modul1.Instance.Aus[12].AsDouble())
                                                        {
                                                            break;
                                                        }
                                                        item = "";
                                                        DataModul.DB_EventTable.MoveNext();
                                                        continue;
                                                    }
                                                    if (Operators.CompareString(Strings.Left(_Modul1.Instance.Kont[0].ToUpper().Trim(), Text2.Text.Length), Text2.Text.ToUpper(), TextCompare: false) == 0)
                                                    {
                                                        num7 = checked(num7 + 1);
                                                        if (_Modul1.Instance.Kont[1].Trim() != "")
                                                        {
                                                            _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0];
                                                        }
                                                        item = Strings.Left(_Modul1.Instance.Kont[0] + "," + _Modul1.Instance.Kont[3] + "                                          ", 40) + text2 + "    " + Strings.Right("          " + DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsString(), 10);
                                                        List1.Items.Add(item);
                                                        if (num7 != _Modul1.Instance.Aus[12].AsDouble())
                                                        {
                                                            item = "";
                                                            DataModul.DB_EventTable.MoveNext();
                                                            continue;
                                                        }
                                                        break;
                                                    }
                                                    item = "";
                                                }
                                            }
                                        }
                                    }
                                    DataModul.DB_EventTable.MoveNext();
                                }
                            }
                            if (0 - (Option1[5].Checked ? 1 : 0) == -1)
                            {
                                DataModul.DSB_SearchTable.Index = "Aliassuch";
                                DataModul.DSB_SearchTable.Seek(">=", Text1.Text, 0);
                                if (Check3.CheckState == CheckState.Unchecked)
                                {
                                    Zeigfam();
                                }
                                else
                                {

                                    Zeig();
                                }
                                goto end_IL_0000_2;
                            }

                            string item2;
                            if (0 - (Option1[6].Checked ? 1 : 0) == -1)
                            {
                                Listleer();
                                _Modul1.Instance.UbgT = " " + Text1.Text;
                                if (_Modul1.Instance.UbgT.Right(1) != ".")
                                {
                                    _Modul1.Instance.UbgT += ".";
                                }

                                string prompt = "Die Eingabe der Nachfahren-Nummer muss in der korrekten Form erfolgen.\nImmer in Blöcken von einem Leerzeichen, einer Ziffer und einem Punkt oder zwei Ziffern und ein Punkt.\n";
                                if (_Modul1.Instance.UbgT.Length / 3.0 != Conversion.Int(_Modul1.Instance.UbgT.Length / 3.0))
                                {
                                    Interaction.MsgBox(prompt);
                                    goto IL_1ceb;
                                }

                                int num8 = _Modul1.Instance.UbgT.Length;
                                num7 = 3;
                                while (num7 <= num8)
                                {
                                    if (Strings.Mid(_Modul1.Instance.UbgT, num7, 1) != ".")
                                    {
                                        Interaction.MsgBox(prompt);
                                        goto IL_1ceb;
                                    }
                                    num7 = checked(num7 + 3);
                                }
                                if (_Modul1.Instance.UbgT == " 1.")
                                {
                                    _Modul1.Instance.UbgT = " 1";
                                }
                                DataModul.DT_DescendentTable.Index = "nr";
                                DataModul.DT_DescendentTable.Seek(">=", _Modul1.Instance.UbgT);
                                _Modul1.Instance.PersInArb = DataModul.DT_DescendentTable.Fields["Pr"].AsInt();
                                Perles1();
                                item2 = Conversions.ToString(Operators.ConcatenateObject(string.Concat(string.Concat(Strings.Left(_Modul1.Instance.Kont[0].TrimEnd() + "," + _Modul1.Instance.Kont[3] + new string(' ', 80), 42) + Strings.Right("          " + DataModul.DT_DescendentTable.Fields["Pr"].AsString(), 10), "         "), Strings.Right("  " + DataModul.DT_DescendentTable.Fields["gen"].AsString().Trim(), 2)), "-" + DataModul.DT_DescendentTable.Fields["Nr"].Value));
                                List1.Items.Add(item2);
                                if (_Modul1.Instance.UbgT == " 1")
                                {
                                    _Modul1.Instance.UbgT = " 1.";
                                    DataModul.DT_DescendentTable.Seek(">=", _Modul1.Instance.UbgT);
                                }
                                else
                                    DataModul.DT_DescendentTable.MoveNext();
                                int num9 = checked((int)Math.Round(_Modul1.Instance.Aus[13].AsDouble()));
                                num7 = 1;
                                while (num7 <= num9)
                                {
                                    _Modul1.Instance.PersInArb = DataModul.DT_DescendentTable.Fields["Pr"].AsInt();
                                    Perles1();
                                    item2 = Conversions.ToString(Operators.ConcatenateObject(string.Concat(string.Concat(Strings.Left(_Modul1.Instance.Kont[0].TrimEnd() + "," + _Modul1.Instance.Kont[3] + new string(' ', 80), 42) + Strings.Right("          " + DataModul.DT_DescendentTable.Fields["Pr"].AsString(), 10), "         "), Strings.Right("  " + DataModul.DT_DescendentTable.Fields["gen"].AsString().Trim(), 2)), "-" + DataModul.DT_DescendentTable.Fields["Nr"].Value));
                                    List1.Items.Add(item2);
                                    DataModul.DT_DescendentTable.MoveNext();
                                    num7 = checked(num7 + 1);
                                }
                            }
                            if (0 - (Option1[7].Checked ? 1 : 0) == -1)
                            {
                                Listleer();
                                _Modul1.Instance.UbgT = new string(' ', 40) + Text1.Text.Right(40);
                                DataModul.DT_AncesterTable.MoveFirst();
                                DataModul.DT_AncesterTable.Index = "Ahnen";
                                DataModul.DT_AncesterTable.Seek(">=", _Modul1.Instance.UbgT);
                                while (!DataModul.DT_AncesterTable.EOF)
                                {
                                    _Modul1.Instance.PersInArb = DataModul.DT_AncesterTable.Fields["Pernr"].AsInt();
                                    Perles1();
                                    item2 = Strings.Left(_Modul1.Instance.Kont[0].TrimEnd() + "," + _Modul1.Instance.Kont[3] + new string(' ', 80), 44) + Strings.Right("          " + DataModul.DT_AncesterTable.Fields["PerNr"].AsString(), 10) + "             " + DataModul.DT_AncesterTable.Fields["gen"].AsString() + "   " + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim();
                                    List1.Items.Add(item2);
                                    DataModul.DT_AncesterTable.MoveNext();
                                }
                            }
                            if (RadioButton1.Checked)
                            {
                                if (Text1.Text == "")
                                {
                                    Text1.Text = "0";
                                }
                                Zeigfamanl((int)Math.Round((Text1.Text + "00000000".Left(8)).AsDouble()));
                                goto end_IL_0000_2;
                            }
                            if (RadioButton2.Checked)
                            {
                                DataModul.DB_FamilyTable.Index = "BeaDat";
                                if (Text1.Text == "")
                                {
                                    Text1.Text = "0";
                                }
                                DataModul.DB_PersonTable.Index = "PerNr";
                                DataModul.DB_PersonTable.Seek(">=", Text1.Text.AsDouble());
                                Zeigfamanl2();
                                goto end_IL_0000_2;
                            }
                        }
                        goto IL_1ceb;
                    IL_1ceb: // <========== 8
                             // <========== 8
                        num = 310;
                        List1.Items.Add("Ende der Liste");
                        goto end_IL_0000_2;
                    IL_1fa9: // <========== 4
                             // <========== 4
                        num4 = num2;
                        goto IL_1fb1;
                    IL_1fad:
                        num4 = num2 + 1;
                        goto IL_1fb1;
                    IL_1fb1:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 13:
                            case 14:
                            case 15:
                                goto IL_00f3;
                            case 19:
                            case 332:
                                goto IL_012f;
                            case 16:
                            case 21:
                            case 246:
                            case 251:
                            case 309:
                            case 310:
                            case 320:
                            case 352:
                                goto IL_1ceb;
                            case 340:
                            case 341:
                                num = 341;
                                lErl = 94;
                                if (Information.Err().Number == 94)
                                {
                                    goto case 343;
                                }
                                goto case 347;
                            case 343:
                                num = 343;
                                DataModul.DSB_SearchTable.Edit();
                                DataModul.DSB_SearchTable.Fields["Kenn"].Value = "9";
                                DataModul.DSB_SearchTable.Update();
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1fa9;
                            case 347:
                            case 348:
                            case 349:
                                num = 349;
                                if (Information.Err().Number == 3021)
                                {
                                    goto case 350;
                                }
                                goto case 353;
                            case 350:
                                num = 350;
                                List1.Items.Add("Ende der Liste");
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num2 = 0;
                                goto IL_1ceb;
                            case 353:
                            case 354:
                                num = 354;
                                Interaction.MsgBox(Information.Err().Number);
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1fa9;
                            case 31:
                            case 34:
                            case 35:
                            case 42:
                            case 45:
                            case 46:
                            case 54:
                            case 57:
                            case 58:
                            case 67:
                            case 70:
                            case 71:
                            case 78:
                            case 81:
                            case 82:
                            case 88:
                            case 231:
                            case 234:
                            case 235:
                            case 298:
                            case 308:
                            case 311:
                            case 356:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 9551;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 17
                       // <========== 17
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void Famzeig(int Fam, ref string LiText, ELinkKennz Kenn)
    {
        DataModul.DB_EventTable.Index = "BeSu";
        float num = 502f;
        float num2;
        do
        {
            num2 = num;
            DataModul.DB_EventTable.Seek("=", num, Fam.AsString());
            if (DataModul.DB_EventTable.NoMatch)
            {
                Datu = "      ";
            }
            else
            {
                Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                Datu = Datu.Left(4) + Strings.Right((" " + DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString()), 1);
                if (Datu.AsDouble() == 0.0)
                {
                    Datu = "      ";
                }
                if (Datu.Trim() != "")
                {
                    break;
                }
            }
            num += 1f;
        }
        while (num <= 507f);
        if (Datu.Trim() == "")
        {
            num = 500f;
            do
            {
                DataModul.DB_EventTable.Seek("=", num, Fam.AsString());
                num2 = num;
                if (DataModul.DB_EventTable.NoMatch)
                {
                    Datu = "      ";
                }
                else
                {
                    Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                    Datu = Datu.Left(4) + Strings.Right((" " + DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString()), 1);
                    if (Datu.AsDouble() == 0.0)
                    {
                        Datu = "      ";
                    }
                    if (Datu.Trim() != "")
                    {
                        break;
                    }
                }
                num += 1f;
            }
            while (num <= 501f);
        }
        if (Datu.Trim() == "")
        {
            DataModul.DB_EventTable.Seek("=", 601, Fam.AsString());
            if (DataModul.DB_EventTable.NoMatch)
            {
                Datu = "      ";
            }
            else
            {
                Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                Datu = Datu.Left(4) + Strings.Right((" " + DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString()), 1);
                num2 = 601f;
                if (Datu.AsDouble() == 0.0)
                {
                    Datu = "      ";
                }
            }
        }
        string text = "";
        float num3 = num2;
        if (num3 == 500f)
        {
            text = "Prok.";
        }
        else if (num3 == 501f)
        {
            text = "Verl.";
        }
        else if (num3 == 502f)
        {
            text = "Heir.";
        }
        else if (num3 == 503f)
        {
            text = "kirH.";
        }
        else if (num3 == 505f)
        {
            text = "Eheä.";
        }
        else if (num3 == 504f)
        {
            text = "Scheid.";
        }
        else if (num3 == 507f)
        {
            text = "Dim.";
        }
        else if (num3 == 601f)
        {
            text = "FiHr.";
        }
        if (Datu.Trim() == "")
        {
            text = "    ";
        }
        LiText = new string(' ', 80);
        if (DataModul.Link.GetFamPerson(Fam, Kenn, out _Modul1.Instance.PersInArb))
        {
            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
            StringType.MidStmtStr(ref LiText, 1, 80, "              " + text + Datu.Right(12) + " mit " + _Modul1.Instance.Kont[0].ToUpper().TrimEnd() + "," + _Modul1.Instance.Kont[3]);
        }
        else
        {
            StringType.MidStmtStr(ref LiText, 1, 80, "              " + text + Datu.Right(12) + " mit " + _Modul1.Instance.IText[28]);
        }
        if (Datu.Trim() == "")
        {
            DataModul.DB_FamilyTable.Index = "Fam";
            DataModul.DB_FamilyTable.Seek("=", Fam);
            if (!DataModul.DB_FamilyTable.NoMatch && (DataModul.DB_FamilyTable.Fields[FamilyFields.Aeb].AsInt() == -1))
            {
                StringType.MidStmtStr(ref LiText, 2, 13, "Ausserehel. ");
            }
        }
    }

    public void Datr()
    {
        Datl();
        if (Datum3 != "")
        {
            Datum3 = "+" + Datum3.Right(4);
        }
        if (Datum4 != "")
        {
            Datum4 = "*" + Datum4.Right(4);
        }
    }

    public void Datr1(ref float Idned)
    {
        //Discarded unreachable code: IL_1460
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        string QuText = default;
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
                        num = 1;
                        Datles1();
                        goto IL_0009;
                    case 6362:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_1464;
                                default:
                                    goto end_IL_0000;
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_1464;
                        }
                    end_IL_0000:
                        break;
                    IL_0009:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        float num5 = 1f;
                        if (_Modul1.Instance.Kont[11].Trim() != "")
                        {
                            if (Struck == 1)
                            {
                                Anz[0].SelectedText = "\n";
                                if (Idned != 0f)
                                {
                                    if (Idned != 1f)
                                    {
                                        if (Idned != 3f)
                                        {
                                        }
                                    }
                                }
                                Anz[0].SelectedText = _Modul1.Instance.DTxt[1] + " " + _Modul1.Instance.Kont[11].Trim() + ". ";
                                num5 = 0f;
                            }
                            else
                            {
                                Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[1] + " " + _Modul1.Instance.Kont[11].Trim() + ". ";
                                num5 = 0f;
                            }
                            if (BemSch == 1)
                            {
                                if (_Modul1.Instance.Aus[2] == "Y" && _Modul1.Instance.Kont[16].Length > 0)
                                {
                                    if (num5 == 1f)
                                    {
                                        Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[1];
                                        num5 = 0f;
                                    }
                                    QuText = _Modul1.Instance.Kont[16];
                                    Zeiweg(ref QuText);
                                    Anz[0].SelectedText = " " + QuText;
                                    QuText = "";
                                }
                                if (_Modul1.Instance.Aus[3] == "Y" && _Modul1.Instance.Kont[21].Length > 0)
                                {
                                    if (num5 == 1f)
                                    {
                                        Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[1];
                                        num5 = 0f;
                                    }
                                    QuText = _Modul1.Instance.Kont[21];
                                    Zeiweg(ref QuText);
                                    Anz[0].SelectedText = " " + QuText;
                                    QuText = "";
                                }
                            }
                        }
                        if (_Modul1.Instance.Kont[41] != "")
                        {
                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont[41];
                        }
                        if (_Modul1.Instance.Kont[31] != "")
                        {
                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont[31];
                        }
                        num5 = 1f;
                        if (_Modul1.Instance.Kont[12].Trim() != "")
                        {
                            if (Struck == 1)
                            {
                                Anz[0].SelectedText = "\n";
                                if (Idned != 0f)
                                {
                                    if (Idned != 1f)
                                    {
                                    }
                                }
                                Anz[0].SelectedText = _Modul1.Instance.DTxt[2] + " " + _Modul1.Instance.Kont[12].Trim() + ". ";
                                num5 = 0f;
                            }
                            else
                            {
                                Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[2] + " " + _Modul1.Instance.Kont[12].Trim() + ". ";
                                num5 = 0f;
                            }
                        }
                        if (BemSch == 1)
                        {
                            if (_Modul1.Instance.Aus[2] == "Y" && _Modul1.Instance.Kont[17].Length > 0)
                            {
                                if (num5 == 1f)
                                {
                                    Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[2];
                                    num5 = 0f;
                                }
                                QuText = _Modul1.Instance.Kont[17];
                                Zeiweg(ref QuText);
                                Anz[0].SelectedText = " " + QuText;
                                QuText = "";
                            }
                            if (_Modul1.Instance.Aus[3] == "Y" && _Modul1.Instance.Kont[22].Length > 0)
                            {
                                if (num5 == 1f)
                                {
                                    Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[2];
                                    num5 = 0f;
                                }
                                QuText = _Modul1.Instance.Kont[22];
                                Zeiweg(ref QuText);
                                Anz[0].SelectedText = " " + QuText;
                                QuText = "";
                            }
                        }
                        if (_Modul1.Instance.Kont[42] != "")
                        {
                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont[42];
                        }
                        if (_Modul1.Instance.Kont[32] != "")
                        {
                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont[32];
                        }
                        if (_Modul1.Instance.Aus[30] == "Y")
                        {
                            int persInArb = _Modul1.Instance.PersInArb;
                            int num6 = 0;
                            foreach (var cLink in DataModul.Link.ReadAllFams(_Modul1.Instance.PersInArb, ELinkKennz.lkGodparent))
                            {
                                _Modul1.Instance.PersInArb = cLink.iPersNr;
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                                if (num6 == 0)
                                {
                                    Anz[0].SelectedText = "Paten: " + _Modul1.Instance.Kont[3].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                    num6 = 1;
                                }
                                else Anz[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                if (_Modul1.Instance.Aus[31] != "Y")
                                {
                                    Datles1();
                                    if (_Modul1.Instance.Kont[11] == "")
                                    {
                                        _Modul1.Instance.Kont[11] = _Modul1.Instance.Kont[12];
                                    }
                                    if (_Modul1.Instance.Kont[11].Trim() != "")
                                    {
                                        Anz[0].SelectedText = " * " + _Modul1.Instance.Kont[11].Trim();
                                    }
                                    if (_Modul1.Instance.Kont[13] == "")
                                    {
                                        _Modul1.Instance.Kont[13] = _Modul1.Instance.Kont[14];
                                    }
                                    if ((_Modul1.Instance.Kont[14].Trim() != "") & (_Modul1.Instance.Kont[11].Trim() != ""))
                                    {
                                        Anz[0].SelectedText = ",";
                                    }
                                    if (_Modul1.Instance.Kont[14].Trim() != "")
                                    {
                                        Anz[0].SelectedText = " + " + _Modul1.Instance.Kont[13].Trim();
                                    }
                                    Anz[0].SelectedText = "; ";
                                }
                                else Anz[0].SelectedText = "; ";
                                lErl = 55;
                                _Modul1.Instance.PersInArb = persInArb;
                            }
                            _Modul1.Instance.PersInArb = persInArb;
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields[PersonFields.Bem2].Value) && Operators.CompareString(DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                            {
                                if (num6 == 0)
                                {
                                    Anz[0].SelectedText = " Paten: ";
                                }
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                                Anz[0].SelectedText = DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString().Trim();
                                Retweg3();
                            }
                            _Modul1.Instance.PersInArb = persInArb;
                        }
                        Retweg3();
                        if (Anz[0].SelectionStart > 0)
                        {
                            if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == ";")
                            {
                                Anz[0].SelectionStart = checked(Anz[0].SelectionStart - 1);
                                Anz[0].SelectionLength = 2;
                                Anz[0].SelectedText = "";
                            }
                            if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != ".")
                            {
                                Anz[0].SelectedText = ".";
                            }
                        }
                        lErl = 66;
                        int famInArb = _Modul1.Instance.FamInArb;
                        int persInArb2 = _Modul1.Instance.PersInArb;
                        _Modul1.Instance.PersInArb = persInArb2;
                        persInArb2 = 0;
                        _Modul1.Instance.FamInArb = famInArb;
                        famInArb = 0;
                        num5 = 1f;
                        Datles1();
                        if (_Modul1.Instance.Kont[13].Trim() != "")
                        {
                            if (Struck == 1)
                            {
                                Anz[0].SelectedText = "\n";
                                if (Idned != 0f)
                                {
                                    if (Idned != 1f)
                                    {
                                    }
                                }
                                Anz[0].SelectedText = _Modul1.Instance.DTxt[3] + " " + _Modul1.Instance.Kont[13].Trim() + ". ";
                                num5 = 0f;
                            }
                            else
                            {
                                Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[3] + " " + _Modul1.Instance.Kont[13].Trim() + ". ";
                                num5 = 0f;
                            }
                            if (BemSch == 1)
                            {
                                if (_Modul1.Instance.Aus[2] == "Y" && _Modul1.Instance.Kont[18].Length > 0)
                                {
                                    if (num5 == 1f)
                                    {
                                        Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[3];
                                        num5 = 0f;
                                    }
                                    QuText = _Modul1.Instance.Kont[18];
                                    Zeiweg(ref QuText);
                                    Anz[0].SelectedText = " " + QuText + " ";
                                    QuText = "";
                                }
                                if (_Modul1.Instance.Aus[3] == "Y" && _Modul1.Instance.Kont[23].Length > 0)
                                {
                                    if (num5 == 1f)
                                    {
                                        Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[3];
                                        num5 = 0f;
                                    }
                                    QuText = _Modul1.Instance.Kont[23];
                                    Zeiweg(ref QuText);
                                    Anz[0].SelectedText = " " + QuText;
                                    QuText = "";
                                }
                            }
                        }
                        if (_Modul1.Instance.Kont[43] != "")
                        {
                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont[43];
                        }
                        if (_Modul1.Instance.Kont[33] != "")
                        {
                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont[33];
                        }
                        num5 = 1f;
                        if (_Modul1.Instance.Kont[14].Trim() != "")
                        {
                            if (Struck == 1)
                            {
                                Anz[0].SelectedText = "\n";
                                if (Idned != 0f)
                                {
                                    if (Idned != 1f)
                                    {
                                    }
                                }
                                Anz[0].SelectedText = _Modul1.Instance.DTxt[4] + " " + _Modul1.Instance.Kont[14].Trim() + ". ";
                                num5 = 0f;
                            }
                            else
                            {
                                Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[4] + " " + _Modul1.Instance.Kont[14].Trim() + ". ";
                                num5 = 0f;
                            }
                            if (BemSch == 1)
                            {
                                if (_Modul1.Instance.Aus[2] == "Y" && _Modul1.Instance.Kont[19].Length > 0)
                                {
                                    if (num5 == 1f)
                                    {
                                        Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[4];
                                        num5 = 0f;
                                    }
                                    Anz[0].SelectedText = " " + _Modul1.Instance.Kont[19];
                                }
                                if (_Modul1.Instance.Aus[3] == "Y" && _Modul1.Instance.Kont[24].Length > 0)
                                {
                                    if (num5 == 1f)
                                    {
                                        Anz[0].SelectedText = " " + _Modul1.Instance.DTxt[4];
                                        num5 = 0f;
                                    }
                                    Anz[0].SelectedText = " " + _Modul1.Instance.Kont[24];
                                }
                            }
                        }
                        if (_Modul1.Instance.Kont[44] != "")
                        {
                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont[44];
                        }
                        if (_Modul1.Instance.Kont[34] != "")
                        {
                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont[34];
                        }
                        if (Struck == 1 && Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                        {
                            Anz[0].SelectedText = "\n";
                        }
                        Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                        goto end_IL_0000_2;
                    IL_1464:
                        num4 = num2 + 1;
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
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 6362;
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
    public void Zeiweg(ref string QuText)
    {
        if (_Modul1.Instance.Aus[36] != "Y")
        {
            QuText = QuText.Replace("\t", " ");
            QuText = QuText.Replace("\n", " ");
            QuText = QuText.Replace("\r", " ");
            QuText = QuText.Replace("  ", " ");
            QuText = QuText.Trim();
        }
    }

    public void Berufe()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        int num5 = default;
        short num7 = default;
        short num8 = default;
        string QuText = default;
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
                    short Schalt;
                    int AAA;
                    string LD;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 10861:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_2453;
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
                                goto IL_2457;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            Job = "";
                            List3.Items.Clear();
                            Beruf = (EEventArt)_Modul1.Instance.Ubg;
                            _Modul1.Instance.Art = (EEventArt)_Modul1.Instance.Ubg;
                            num5 = _Modul1.Instance.PersInArb;
                            if (_Modul1.Instance.Art > EEventArt.eA_601)
                            {
                                num5 = _Modul1.Instance.FamInArb;
                            }
                            DataModul.DB_EventTable.Index = "Besu";
                            DataModul.DB_EventTable.Seek("=", Beruf.AsString(), num5.AsString());
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                DataModul.DB_EventTable.Index = "ArtNr";
                                goto end_IL_0000_2;
                            }
                            goto IL_0409;
                        IL_03d9:
                            num = 46;
                            List3.Items.Add(Job);
                            goto IL_03f4;
                        IL_03f4: // <========== 3
                            num = 47;
                            lErl = 2;
                            DataModul.DB_EventTable.MoveNext();
                            goto IL_0409;
                        IL_0409: // <========== 3
                            num = 17;
                            float num6;
                            if (!DataModul.DB_EventTable.EOF)
                            {
                                if (!(DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != _Modul1.Instance.Art))
                                {
                                    if (!Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                                    {
                                        num6 = 0f;
                                        while (num6 <= 15f)
                                        {
                                            _Modul1.Instance.Kont1[(int)Math.Round(num6)] = "";
                                            num6 += 1f;
                                        }
                                        _Modul1.Instance.Ubg = num7;
                                        Datu = "";
                                        if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != num5)))
                                        {
                                            DataModul.DB_EventTable.Index = "ArtNr";
                                            goto IL_041c;
                                        }
                                        if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                        {
                                            Datu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                            _Modul1.Instance.Kont1[1] = Datu;
                                        }
                                        _Modul1.Instance.UbgT = "";
                                        if (DataModul.DB_EventTable.Fields[EventFields.KBem].Value.AsDouble() > 0.0)
                                        {
                                            AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                            LD = "";
                                            DataModul.Textlese(AAA, out KontU, out LD);
                                            _Modul1.Instance.Kont1[7] = " " + KontU.Trim() + " ";
                                        }
                                        Job = _Modul1.Instance.Kont1[1] + _Modul1.Instance.Kont1[3] + _Modul1.Instance.Kont1[7] + new string(' ', 240).Left(240) + DataModul.DB_EventTable.Fields[EventFields.LfNr].AsString();
                                        if (DataModul.DB_EventTable.Fields[EventFields.Reg].AsString() != " ")
                                        {
                                            Job = "+" + Job;
                                        }
                                        goto IL_03d9;
                                    }
                                    goto IL_03f4;
                                }
                            }
                            goto IL_041c;
                        IL_041c: // <========== 3
                            num = 50;
                            lErl = 13;
                            EEventArt beruf = Beruf;
                            if (beruf == EEventArt.eA_300)
                            {
                                if (List3.Items.Count == 0)
                                {
                                    goto end_IL_0000_2;
                                }
                                if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                                {
                                    Anz[0].SelectedText = "\n";
                                }
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                                if (List3.Items.Count == 1)
                                {
                                    Anz[0].SelectedText = "Beruf: ";
                                }
                                goto IL_04fa;
                            }
                            if (beruf == EEventArt.eA_301)
                            {
                                if (List3.Items.Count == 0)
                                {
                                    goto end_IL_0000_2;
                                }
                                if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                                {
                                    Anz[0].SelectedText = "\n";
                                }
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                                if (List3.Items.Count >= 1)
                                {
                                    Anz[0].SelectedText = "Titel: ";
                                }
                                goto IL_061f;
                            }
                            if (beruf == EEventArt.eA_302)
                            {
                                if (List3.Items.Count == 0)
                                {
                                    goto end_IL_0000_2;
                                }
                                if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                                {
                                    Anz[0].SelectedText = "\n";
                                }
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                                if (List3.Items.Count == 1)
                                {
                                    Anz[0].SelectedText = "Wohnung: ";
                                }
                                goto IL_0713;
                            }
                            if (beruf == EEventArt.eA_602)
                            {
                                if (List3.Items.Count == 0)
                                {
                                    goto end_IL_0000_2;
                                }
                                Retweg3();
                                Anz[0].SelectedText = "\n\n";
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                                if (List3.Items.Count == 1)
                                {
                                    Anz[0].SelectedText = "Wohnung der Familie: ";
                                }
                                goto IL_0814;
                            }
                            goto IL_086a;
                        IL_04fa:
                            num = 65;
                            if (List3.Items.Count > 1)
                            {
                                Anz[0].SelectedText = "Berufe: ";
                            }
                            goto IL_052b;
                        IL_052b:
                            num = 68;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            goto IL_086a;
                        IL_061f:
                            num = 81;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            goto IL_086a;
                        IL_0713:
                            num = 94;
                            if (List3.Items.Count > 1)
                            {
                                Anz[0].SelectedText = "Wohnungen: ";
                            }
                            goto IL_0744;
                        IL_0744:
                            num = 97;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            goto IL_086a;
                        IL_0814:
                            num = 109;
                            if (List3.Items.Count > 1)
                            {
                                Anz[0].SelectedText = "Wohnungen der Familie: ";
                            }
                            goto IL_0845;
                        IL_0845:
                            num = 112;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            goto IL_086a;
                        IL_086a: // <========== 6
                            num = 114;
                            num8 = (short)(List3.Items.Count - 1);
                            num7 = 0;
                            goto IL_23e9;
                        IL_0b10:
                            num = 138;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                Datu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                Datu = "00000000" + Datu.Right(8);
                                Ds = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                Datwand1(ref Datu, ref Ds);
                                if (Datu.Left(2) == "am")
                                {
                                    Datu = Strings.Mid(Datu, 4, Datu.Length);
                                }
                                goto IL_0c18;
                            }
                            goto IL_0dfc;
                        IL_0c18:
                            num = 146;
                            if ((Datu != "")
                                        & (DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString() == ""))
                            {
                                Datu = " bis " + Datu;
                            }
                            goto IL_0c88;
                        IL_0c88:
                            num = 149;
                            if (_Modul1.Instance.Kont1[1].Left(2) == "am")
                            {
                                _Modul1.Instance.Kont1[1] = Strings.Mid(_Modul1.Instance.Kont1[1], 4, _Modul1.Instance.Kont1[1].Length);
                            }
                            goto IL_0cd1;
                        IL_0cd1:
                            num = 152;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString() == "")
                            {
                                if (_Modul1.Instance.Kont1[1].Trim() != "")
                                {
                                    if (_Modul1.Instance.Kont1[1].Length < 10)
                                    {
                                        _Modul1.Instance.Kont1[1] = "von " + _Modul1.Instance.Kont1[1];
                                        goto IL_0d80;
                                    }
                                    _Modul1.Instance.Kont1[1] = "vom " + _Modul1.Instance.Kont1[1];
                                }
                                goto IL_0d80;
                            }
                            goto IL_0da3;
                        IL_0d80: // <========== 3
                            num = 161;
                            _Modul1.Instance.Kont1[3] = " " + Datu.Trim();
                            goto IL_0da3;
                        IL_0da3: // <========== 3
                            num = 163;
                            if ((_Modul1.Instance.Kont1[3] == "") & (Datu != ""))
                            {
                                _Modul1.Instance.Kont1[3] = " " + Datu.Trim();
                            }
                            goto IL_0dfc;
                        IL_0dfc: // <========== 3
                            num = 167;
                            _Modul1.Instance.UbgT = "";
                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].Value.AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                LD = "";
                                DataModul.Textlese(AAA, out KontU, out LD);
                                _Modul1.Instance.Kont1[7] = " " + KontU.Trim() + " ";
                            }
                            goto IL_0eb2;
                        IL_0eb2:
                            num = 172;
                            if (Beruf == EEventArt.eA_603)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                                LD = "";
                                DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                                if (_Modul1.Instance.Kont[0] != "")
                                {
                                    _Modul1.Instance.Kont1[9] = _Modul1.Instance.Kont[0].Trim() + ": ";

                                }
                            }
                            goto IL_0f57;
                        IL_0f57: // <========== 3
                            num = 178;
                            if (Beruf == EEventArt.eA_105)
                            {
                                if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                                {
                                    Anz[0].SelectedText = "\n";
                                }
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value))
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                                    LD = "";
                                    DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                                }
                                else
                                {

                                    _Modul1.Instance.Kont[0] = "";
                                }
                                goto IL_104e;
                            }
                            goto IL_10cc;
                        IL_104e: // <========== 3
                            num = 188;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                            Anz[0].SelectedText = _Modul1.Instance.Kont[0].Trim() + ": ";
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            goto IL_10cc;
                        IL_10cc: // <========== 3
                            num = 192;
                            if (Beruf == EEventArt.eA_603)
                            {
                                if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                                {
                                    Anz[0].SelectedText = "\n";
                                }
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                                Anz[0].SelectedText = _Modul1.Instance.Kont[0].Trim() + ": ";
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            }
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                            {
                                Kont2[6] = "";
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Zusatz].Value))
                                {
                                    Kont2[6] = DataModul.DB_EventTable.Fields[EventFields.Zusatz].AsString();
                                }
                                AAA = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble());
                                Schalt = 0;
                                ortles(ref AAA, ref Schalt);
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    _Modul1.Instance.UbgT = _Modul1.Instance.UbgT.TrimEnd() + " " + DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString().Trim();
                                }
                                _Modul1.Instance.Kont1[5] = " " + _Modul1.Instance.UbgT.Trim();
                                _Modul1.Instance.UbgT = "";
                            }
                            _Modul1.Instance.Kont1[4] = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                            if (DataModul.DB_EventTable.Fields[EventFields.Platz].Value.AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                LD = "";
                                DataModul.Textlese(AAA, out KontU, out LD);
                                _Modul1.Instance.Kont1[8] = " " + KontU.Trim() + " ";
                            }
                            _Modul1.Instance.Kont1[4] = "";
                            if (Beruf < EEventArt.eA_400)
                            {
                                if (_Modul1.Instance.Aus[2] == "Y")
                                {
                                    if (DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString() != " ")
                                    {
                                        _Modul1.Instance.Kont1[2] = "{" + DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim() + "}";

                                    }
                                }
                                goto IL_14c5;
                            }
                            if (_Modul1.Instance.Aus[2] == "Y")
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString() != " ")
                                {
                                    _Modul1.Instance.Kont1[2] = "{" + DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim() + "}";

                                }
                            }
                            goto IL_15e3;
                        IL_14c5: // <========== 3
                            num = 224;
                            if (_Modul1.Instance.Aus[3] == "Y")
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString() != " ")
                                {
                                    _Modul1.Instance.Kont1[4] = "{" + DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim() + "}";

                                }
                            }
                            goto IL_166c;
                        IL_15e3: // <========== 3
                            num = 236;
                            if (_Modul1.Instance.Aus[3] == "Y")
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString() != " ")
                                {
                                    _Modul1.Instance.Kont1[4] = "{" + DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim() + "}";

                                }
                            }
                            goto IL_166c;
                        IL_166c: // <========== 5
                            num = 242;
                            if ((_Modul1.Instance.Kont1[2].Trim() != "") | (_Modul1.Instance.Kont1[4].Trim() != ""))
                            {
                                _Modul1.Instance.Kont1[6] = " " + _Modul1.Instance.Kont1[2].Trim() + " " + _Modul1.Instance.Kont1[4].Trim();
                            }
                            Job = _Modul1.Instance.Kont1[1] + _Modul1.Instance.Kont1[3] + _Modul1.Instance.Kont1[7] + _Modul1.Instance.Kont1[5] + _Modul1.Instance.Kont1[8] + _Modul1.Instance.UbgT + _Modul1.Instance.Kont1[6];
                            if (Beruf == EEventArt.eA_603)
                            {
                                Job = _Modul1.Instance.Kont1[1] + _Modul1.Instance.Kont1[3] + _Modul1.Instance.Kont1[7] + _Modul1.Instance.Kont1[5] + _Modul1.Instance.Kont1[8] + _Modul1.Instance.UbgT + _Modul1.Instance.Kont1[6];
                            }
                            DataModul.DB_SourceLinkTable.Index = "Tab22";
                            DataModul.DB_SourceLinkTable.Seek("=", 3, num5, Beruf, LfNR);
                            QuText = "";
                            goto IL_1d2d;
                        IL_1a6f: // <========== 3
                            num = 270;
                            if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[3].Value) & (Operators.CompareString(DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim(), "", TextCompare: false) != 0))
                            {
                                if (Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value))
                                {
                                    QuText = QuText + " Seiten: " + DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim();
                                    goto IL_1bc8;
                                }
                                QuText = QuText + ", " + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim() + " " + DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim();
                            }
                            goto IL_1bc8;
                        IL_1bc8: // <========== 3
                            num = 278;
                            if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value))
                            {
                                if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString() != "")
                                {
                                    QuText = (QuText + " >" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value + "<").AsString();
                                }
                                goto IL_1c63;
                            }
                            goto IL_1c72;
                        IL_1c63:
                            num = 282;
                            Zeiweg(ref QuText);
                            goto IL_1c72;
                        IL_1c72: // <========== 3
                            num = 284;
                            if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value))
                            {
                                if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString() != "")
                                {
                                    QuText = (QuText + " ==" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value + "==").AsString();
                                }
                                goto IL_1d0d;
                            }
                            goto IL_1d1c;
                        IL_1d0d:
                            num = 288;
                            Zeiweg(ref QuText);
                            goto IL_1d1c;
                        IL_1d1c: // <========== 5
                            num = 292;
                            DataModul.DB_SourceLinkTable.MoveNext();
                            goto IL_1d2d;
                        IL_1d2d: // <========== 3
                            num = 253;
                            if (!DataModul.DB_SourceLinkTable.EOF)
                            {
                                if (!DataModul.DB_SourceLinkTable.NoMatch)
                                {
                                    if (!((DataModul.DB_SourceLinkTable.Fields[0].AsInt() != 3)
                                        | (DataModul.DB_SourceLinkTable.Fields[1].AsInt() > _Modul1.Instance.PersInArb)
                                        | (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsEnum<EEventArt>() != Beruf)
                                        | (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.LfNr].AsInt() != LfNR)))
                                    {
                                        DataModul.DB_QuTable.Index = "NR";
                                        DataModul.DB_QuTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3].AsInt());
                                        if (!DataModul.DB_QuTable.NoMatch)
                                        {
                                            if (QuText == "")
                                            {
                                                QuText = ". Quellen: ";
                                            }
                                            if (QuText.Trim().Length > 10)
                                            {
                                                QuText = (QuText + "; " + DataModul.DB_QuTable.Fields[QuFields._2].AsString());
                                            }
                                            else
                                            {

                                                QuText = (QuText + DataModul.DB_QuTable.Fields[QuFields._2].AsString());
                                            }
                                            goto IL_1a6f;
                                        }
                                    }
                                    else goto IL_1d43;
                                }
                                goto IL_1d1c;
                            }
                            goto IL_1d43;
                        IL_1d43: // <========== 3
                            num = 294;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem3].Value))
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    _Modul1.Instance.Kont1[9] = DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim();
                                }
                                goto IL_1ddc;
                            }
                            goto IL_1f0d;
                        IL_1ddc:
                            num = 298;
                            if (_Modul1.Instance.Kont1[9].Trim() != "")
                            {
                                if (QuText == "")
                                {
                                    QuText = ". Quellen: " + _Modul1.Instance.Kont1[9].Trim();
                                }
                                else
                                {

                                    QuText = QuText + "; " + _Modul1.Instance.Kont1[9].Trim();
                                    _Modul1.Instance.Kont1[9] = "";
                                }
                                goto IL_1e77;
                            }
                            goto IL_1f0d;
                        IL_1e77: // <========== 3
                            num = 306;
                            if (QuText.Trim() != "")
                            {
                                Zeiweg(ref QuText);
                                QuText = QuText.Trim();
                                if (QuText.Right(1) == ";")
                                {
                                    QuText = Strings.Trim(QuText.Left(QuText.Trim().Length - 1));
                                }
                                goto IL_1ef3;
                            }
                            goto IL_1f0d;
                        IL_1ef3:
                            num = 312;
                            QuText = QuText.Trim() + ".";
                            goto IL_1f0d;
                        IL_1f0d: // <========== 5
                            num = 316;
                            float num9;
                            if (Beruf <= EEventArt.eA_499)
                            {
                                persInArb = _Modul1.Instance.PersInArb;
                                num9 = 1f;
                                while (num9 <= 100f)
                                {
                                    KontSP[(int)Math.Round(num9)] = _Modul1.Instance.Kont[(int)Math.Round(num9)];
                                    _Modul1.Instance.Kont[(int)Math.Round(num9)] = "";
                                    num9 += 1f;
                                }

                                EEventArt Art = _Modul1.Instance.Art;
                                if (_Modul1.Instance.Aus[32] == "Y")
                                {
                                    Zeugsu(Art);
                                    _Modul1.Instance.PersInArb = persInArb;
                                    DataModul.DB_EventTable.Seek("=", Art.AsString(), _Modul1.Instance.PersInArb.AsString(), LfNR);
                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem4].Value))
                                    {
                                        if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            if (_Modul1.Instance.UbgT1.Trim() != "")
                                            {
                                                _Modul1.Instance.UbgT1 = _Modul1.Instance.UbgT1 + "; " + DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim();
                                                goto IL_2152;
                                            }
                                            _Modul1.Instance.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim();
                                        }

                                    }
                                }
                                goto IL_2152;
                            }
                            goto IL_222b;
                        IL_2152: // <========== 4
                            num = 340;
                            _Modul1.Instance.PersInArb = persInArb;
                            _Modul1.Instance.Ubg = (int)_Modul1.Instance.Art;
                            num9 = 1f;
                            while (num9 <= 100f)
                            {
                                _Modul1.Instance.Kont[(int)Math.Round(num9)] = KontSP[(int)Math.Round(num9)];
                                KontSP[(int)Math.Round(num9)] = "";
                                num9 += 1f;
                            }
                            _Modul1.Instance.PersInArb = persInArb;
                            if (_Modul1.Instance.UbgT1.Trim() != "")
                            {
                                _Modul1.Instance.UbgT1 = " Zeugen: " + _Modul1.Instance.UbgT1.Trim() + ".";
                            }
                            goto IL_222b;
                        IL_222b: // <========== 3
                            num = 350;
                            lErl = 10;
                            Job = Job.Trim() + QuText + _Modul1.Instance.UbgT1;
                            QuText = "";
                            _Modul1.Instance.UbgT1 = "";
                            if (Beruf == EEventArt.eA_105)
                            {
                                if (Job.Trim() != "")
                                {
                                    Anz[0].SelectedText = Job;
                                }
                                goto IL_23cc;
                            }
                            if (num7 == 0)
                            {
                                if (Job.Trim() != "")
                                {
                                    Anz[0].SelectedText = Job;
                                }
                                goto IL_23cc;
                            }
                            if (Beruf != EEventArt.eA_603)
                            {
                                if (Job.Trim() != "")
                                {
                                    Anz[0].SelectedText = "\n" + Job;
                                }
                                goto IL_23cc;
                            }
                            if (Job.Trim() != "")
                            {
                                Anz[0].SelectedText = Job;
                            }
                            goto IL_23cc;
                        IL_23cc: // <========== 5
                            num = 378;
                            DataModul.DB_EventTable.MoveNext();
                            num7 = (short)unchecked(num7 + 1);
                            goto IL_23e9;
                        IL_23e9:
                            if (num7 <= num8)
                            {
                                LfNR = (short)Math.Round(Conversion.Val(Strings.Mid(List3.Items[num7].AsString(), 240, 10)));
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", Beruf.AsString(), num5.AsString(), LfNR);
                                if (Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                                {
                                    Interaction.MsgBox("7");
                                    Debugger.Break();
                                }
                                num6 = 0f;
                                while (num6 <= 15f)
                                {
                                    _Modul1.Instance.Kont1[(int)Math.Round(num6)] = "";
                                    num6 += 1f;
                                }
                                _Modul1.Instance.Ubg = num7;
                                _Modul1.Instance.Art = Beruf;
                                Datu = "";
                                if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != num5)))
                                {
                                    DataModul.DB_EventTable.Index = "ArtNr";
                                    goto end_IL_0000_2;
                                }
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    Datu = Strings.Right(("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()).AsString(), 8);
                                    Ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                    Datwand1(ref Datu, ref Ds);
                                    _Modul1.Instance.Kont1[1] = Datu;
                                }
                                goto IL_0b10;
                            }
                            Beruf = 0;
                            goto end_IL_0000_2;
                        IL_2453:
                            num4 = unchecked(num2 + 1);
                            goto IL_2457;
                        IL_2457:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 45:
                                case 46:
                                    goto IL_03d9;
                                case 22:
                                case 47:
                                    goto IL_03f4;
                                case 15:
                                case 16:
                                case 17:
                                case 49:
                                    goto IL_0409;
                                case 19:
                                case 31:
                                case 50:
                                    goto IL_041c;
                                case 64:
                                case 65:
                                    goto IL_04fa;
                                case 67:
                                case 68:
                                    goto IL_052b;
                                case 80:
                                case 81:
                                    goto IL_061f;
                                case 93:
                                case 94:
                                    goto IL_0713;
                                case 96:
                                case 97:
                                    goto IL_0744;
                                case 108:
                                case 109:
                                    goto IL_0814;
                                case 111:
                                case 112:
                                    goto IL_0845;
                                case 52:
                                case 69:
                                case 82:
                                case 98:
                                case 113:
                                case 114:
                                    goto IL_086a;
                                case 137:
                                case 138:
                                    goto IL_0b10;
                                case 145:
                                case 146:
                                    goto IL_0c18;
                                case 148:
                                case 149:
                                    goto IL_0c88;
                                case 151:
                                case 152:
                                    goto IL_0cd1;
                                case 156:
                                case 159:
                                case 160:
                                case 161:
                                    goto IL_0d80;
                                case 162:
                                case 163:
                                    goto IL_0da3;
                                case 165:
                                case 166:
                                case 167:
                                    goto IL_0dfc;
                                case 171:
                                case 172:
                                    goto IL_0eb2;
                                case 176:
                                case 177:
                                case 178:
                                    goto IL_0f57;
                                case 184:
                                case 187:
                                case 188:
                                    goto IL_104e;
                                case 191:
                                case 192:
                                    goto IL_10cc;
                                case 222:
                                case 224:
                                    goto IL_14c5;
                                case 234:
                                case 236:
                                    goto IL_15e3;
                                case 227:
                                case 229:
                                case 239:
                                case 241:
                                case 242:
                                    goto IL_166c;
                                case 266:
                                case 269:
                                case 270:
                                    goto IL_1a6f;
                                case 273:
                                case 276:
                                case 277:
                                case 278:
                                    goto IL_1bc8;
                                case 281:
                                case 282:
                                    goto IL_1c63;
                                case 283:
                                case 284:
                                    goto IL_1c72;
                                case 287:
                                case 288:
                                    goto IL_1d0d;
                                case 289:
                                case 290:
                                case 291:
                                case 292:
                                    goto IL_1d1c;
                                case 252:
                                case 253:
                                case 293:
                                    goto IL_1d2d;
                                case 256:
                                case 294:
                                    goto IL_1d43;
                                case 297:
                                case 298:
                                    goto IL_1ddc;
                                case 301:
                                case 305:
                                case 306:
                                    goto IL_1e77;
                                case 311:
                                case 312:
                                    goto IL_1ef3;
                                case 313:
                                case 314:
                                case 315:
                                case 316:
                                    goto IL_1f0d;
                                case 333:
                                case 336:
                                case 337:
                                case 338:
                                case 339:
                                case 340:
                                    goto IL_2152;
                                case 317:
                                case 349:
                                case 350:
                                    goto IL_222b;
                                case 357:
                                case 358:
                                case 363:
                                case 364:
                                case 369:
                                case 370:
                                case 374:
                                case 375:
                                case 376:
                                case 377:
                                case 378:
                                    goto IL_23cc;
                                case 14:
                                case 56:
                                case 72:
                                case 85:
                                case 101:
                                case 130:
                                case 381:
                                case 386:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 10861;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 8
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    public void Kindles(ref string KText)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        short num5 = default;
        float num6 = default;
        short num8 = default;
        string QuText = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    string LD;
                    float Idned;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 2390:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_07b4;
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
                                goto IL_07b8;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            num6 = 0f;
                            short num7 = 1;
                            while (num7 <= 99)
                            {
                                _Modul1.Instance.Family.Kind[num7] = 0;
                                num7 = (short)unchecked(num7 + 1);
                            }
                            num7 = 1;
                            foreach (var cLink in DataModul.Link.ReadAllFams(_Modul1.Instance.FamInArb, _Modul1.Instance.eLKennz))
                            {
                                num6 += 1f;
                                _Modul1.Instance.Family.Kind[(int)Math.Round(num6)] = cLink.iPersNr;
                                num7++;
                            }
                            goto IL_0188;
                        IL_0188: // <========== 4
                            num = 27;
                            Anz[0].SelectionIndent = 0;
                            if (!(num6 > 0f))
                            {
                                goto end_IL_0000_2;
                            }
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                            Anz[0].SelectedText = "\n" + KText;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            Kisort();
                            num8 = (short)(List2.Items.Count - 1);
                            num5 = 0;
                            goto IL_075e;
                        IL_0384: // <========== 3
                            num = 45;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                            _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont[0].Trim();
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            if (_Modul1.Instance.Kont[4] != "")
                            {
                                Anz[0].SelectedText = "(" + _Modul1.Instance.Kont[4].TrimEnd() + ") ";
                            }
                            else
                            {
                                Anz[0].SelectedText = "";
                            }
                            goto IL_049a;
                        IL_049a: // <========== 3
                            num = 57;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields["religi"].Value) && Operators.CompareString(DataModul.DB_PersonTable.Fields["religi"].AsString().Trim(), "", TextCompare: false) != 0)
                            {
                                _Modul1.Instance.Ubg = DataModul.DB_PersonTable.Fields["religi"].AsInt();
                                LD = "";
                                DataModul.Textlese(_Modul1.Instance.Ubg, out _Modul1.Instance.UbgT, out LD);
                                if (_Modul1.Instance.UbgT.Trim() != "")
                                {
                                    Anz[0].SelectedText = _Modul1.Instance.UbgT;
                                }

                            }
                            goto IL_05a3;
                        IL_05a3: // <========== 3
                            num = 68;
                            nachnr();
                            BemSch = 1;
                            ID = 0;
                            Idned = 0f;
                            Datr1(ref Idned);
                            if (_Modul1.Instance.Aus[1] == "Y")
                            {
                                if (DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString() != " ")
                                {
                                    QuText = DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString();
                                    Zeiweg(ref QuText);
                                    Anz[0].SelectedText = " " + QuText + " ";
                                    QuText = "";

                                }
                            }
                            goto IL_067a;
                        IL_067a: // <========== 3
                            num = 80;
                            BemSch = 0;
                            ID = 0;
                            Kindhei();
                            if (Struck == 1)
                            {
                                Anz[0].SelectionIndent = 20 + ID;
                            }
                            goto IL_06c5;
                        IL_06c5:
                            num = 86;
                            if (Struck == 1)
                            {
                                if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                                {
                                    Anz[0].SelectedText = "\n";

                                }
                            }
                            goto IL_071a;
                        IL_071a: // <========== 3
                            num = 91;
                            Anz[0].SelectionIndent = 0;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            num5 = (short)unchecked(num5 + 1);
                            goto IL_075e;
                        IL_075e:
                            if (num5 > num8)
                            {
                                goto end_IL_0000_2;
                            }
                            Anz[0].SelectedText = "\n";
                            Anz[0].SelectionHangingIndent = 30;
                            _Modul1.Instance.PersInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List2.Items[num5].AsString(), 9, 10)));
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            if (Num == 6)
                            {
                                Anz[0].SelectedText = num5 + 1.AsString() + ". <" + _Modul1.Instance.PersInArb.AsString() + "> " + _Modul1.Instance.Kont[3];
                            }
                            else
                            {
                                Anz[0].SelectedText = num5 + 1.AsString() + ". " + _Modul1.Instance.Kont[3].Trim();
                            }
                            goto IL_0384;
                        IL_07b4:
                            num4 = unchecked(num2 + 1);
                            goto IL_07b8;
                        IL_07b8:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 10:
                                case 14:
                                case 17:
                                case 23:
                                case 27:
                                    goto IL_0188;
                                case 41:
                                case 44:
                                case 45:
                                    goto IL_0384;
                                case 53:
                                case 56:
                                case 57:
                                    goto IL_049a;
                                case 65:
                                case 66:
                                case 67:
                                case 68:
                                    goto IL_05a3;
                                case 78:
                                case 79:
                                case 80:
                                    goto IL_067a;
                                case 85:
                                case 86:
                                    goto IL_06c5;
                                case 89:
                                case 90:
                                case 91:
                                    goto IL_071a;
                                case 94:
                                case 95:
                                case 100:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 2390;
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
    public void Kisort()
    {
        List2.Items.Clear();
        short num = 1;
        checked
        {
            while (_Modul1.Instance.Family.Kind[num] != 0)
            {
                _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Kind[num];
                DataModul.DB_EventTable.Index = "ArtNr";
                DataModul.DB_EventTable.Seek("=", "101", _Modul1.Instance.PersInArb.AsString(), "0");
                string text;
                if (DataModul.DB_EventTable.NoMatch)
                {
                    DataModul.DB_EventTable.Seek("=", "102", _Modul1.Instance.PersInArb.AsString(), "0");
                    if (DataModul.DB_EventTable.NoMatch)
                    {
                        text = "00000000";
                        goto IL_01e6;
                    }
                }
                else if (DataModul.DB_EventTable.Fields[EventFields.DatumV].AsInt() == 0)
                {
                    DataModul.DB_EventTable.Seek("=", "102", _Modul1.Instance.PersInArb.AsString(), "0");
                    if (DataModul.DB_EventTable.NoMatch)
                    {
                        text = "00000000";

                    }
                }
                else
                {
                    text = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                }
                goto IL_01e6;
            IL_01e6:
                List2.Items.Add(text + _Modul1.Instance.PersInArb.AsString());
                num = (short)unchecked(num + 1);
                if (num > 99)
                {
                    break;
                }
            }
        }
    }

    public void EPerles(int Index, ref int FamSP1, ref int Persp1, short Num)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int persInArb = default;
        string QuText = default;
        int num5 = default;
        short num6 = default;
        short num7 = default;
        int num8 = default;
        int num9 = default;
        int num10 = default;
        int num11 = default;
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
                    int persInArb2;
                    int persInArb3;
                    int Ortnr;
                    short Schalt;
                    string LD;
                    float Art;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 19601:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_41b3;
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
                                goto IL_41b7;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            persInArb = _Modul1.Instance.PersInArb;
                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            if (Num == 6)
                            {
                                Anz[0].SelectedText = "<" + _Modul1.Instance.PersInArb.AsString() + "> ";
                            }
                            Anz[0].SelectedText = _Modul1.Instance.Kont[3].TrimEnd() + " ";
                            _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                            string selectedText = _Modul1.Instance.Kont[0].TrimEnd();
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                            Anz[0].SelectedText = selectedText;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            if (_Modul1.Instance.Kont[5] != "")
                            {
                                Anz[0].SelectedText = " Sippe " + _Modul1.Instance.Kont[5].TrimEnd();
                            }
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Italic);
                            if (_Modul1.Instance.Kont[4] != "")
                            {
                                Anz[0].SelectedText = "(" + _Modul1.Instance.Kont[4].TrimEnd() + ") ";
                            }
                            else
                            {

                                Anz[0].SelectedText = "";
                            }
                            goto IL_01d6;
                        IL_01d6: // <========== 3
                            num = 24;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields["religi"].Value))
                            {
                                if (Operators.CompareString(DataModul.DB_PersonTable.Fields["religi"].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    _Modul1.Instance.Ubg = DataModul.DB_PersonTable.Fields["religi"].AsInt();
                                    LD = "";
                                    DataModul.Textlese(_Modul1.Instance.Ubg, out _Modul1.Instance.UbgT, out LD);
                                    if (_Modul1.Instance.UbgT.Trim() != "")
                                    {
                                        Anz[0].SelectedText = _Modul1.Instance.UbgT;
                                    }

                                }
                            }
                            goto IL_02d6;
                        IL_02d6: // <========== 3
                            num = 34;
                            if (_Modul1.Instance.Kont[6].Trim() != "")
                            {
                                Anz[0].SelectedText = " " + _Modul1.Instance.Kont[6].TrimEnd() + " ";
                            }
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            nachnr();
                            Datles1();
                            Anz[0].SelectedText = _Modul1.Instance.Kont[25];
                            Anz[0].SelectionHangingIndent = 40;
                            Anz[0].SelectedText = "\n";
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            Anz[0].SelectionIndent = 0;
                            Datles1();
                            Anz[0].SelectionIndent = 20;
                            Anz[0].SelectedText = _Modul1.Instance.DTxt[1] + " " + _Modul1.Instance.Kont[11].Trim() + ".";
                            if (_Modul1.Instance.Aus[2] == "Y")
                            {
                                if (_Modul1.Instance.Kont[16].Trim() != "")
                                {
                                    QuText = _Modul1.Instance.Kont[16];
                                    Zeiweg(ref QuText);
                                    Anz[0].SelectedText = " " + QuText;
                                    QuText = "";

                                }
                            }
                            goto IL_04bd;
                        IL_04bd: // <========== 3
                            num = 56;
                            if (_Modul1.Instance.Aus[3] == "Y")
                            {
                                if (_Modul1.Instance.Kont[21].Trim() != "")
                                {
                                    QuText = _Modul1.Instance.Kont[21];
                                    Zeiweg(ref QuText);
                                    Anz[0].SelectedText = " " + QuText;
                                    QuText = "";

                                }
                            }
                            goto IL_053b;
                        IL_053b: // <========== 3
                            num = 64;
                            if (_Modul1.Instance.Kont[31].Trim() != "")
                            {
                                QuText = _Modul1.Instance.Kont[31];
                                Zeiweg(ref QuText);
                                Anz[0].SelectedText = " " + QuText + " ";
                                QuText = "";
                            }
                            FamSP1 = _Modul1.Instance.FamInArb;
                            if (_Modul1.Instance.Kont[41] != "")
                            {
                                Anz[0].SelectedText = " " + _Modul1.Instance.Kont[41];
                            }
                            Anz[0].SelectedText = "\n" + _Modul1.Instance.DTxt[2] + " " + _Modul1.Instance.Kont[12].Trim() + ".";
                            if (_Modul1.Instance.Aus[2] == "Y")
                            {
                                if (_Modul1.Instance.Kont[17].Length > 0)
                                {
                                    QuText = _Modul1.Instance.Kont[17];
                                    Zeiweg(ref QuText);
                                    Anz[0].SelectedText = " " + QuText;
                                    QuText = "";

                                }
                            }
                            goto IL_06bf;
                        IL_06bf: // <========== 3
                            num = 83;
                            if (_Modul1.Instance.Aus[3] == "Y")
                            {
                                if (_Modul1.Instance.Kont[22].Length > 0)
                                {
                                    QuText = _Modul1.Instance.Kont[22];
                                    Zeiweg(ref QuText);
                                    Anz[0].SelectedText = " " + QuText;
                                    QuText = "";

                                }
                            }
                            goto IL_0732;
                        IL_0732: // <========== 3
                            num = 91;
                            if (_Modul1.Instance.Kont[32].Trim() != "")
                            {
                                QuText = _Modul1.Instance.Kont[32];
                                Zeiweg(ref QuText);
                                Anz[0].SelectedText = " " + QuText + ".";
                                QuText = "";
                            }
                            goto IL_079c;
                        IL_079c:
                            num = 97;
                            if (_Modul1.Instance.Kont[42] != "")
                            {
                                Anz[0].SelectedText = " " + _Modul1.Instance.Kont[42];
                            }
                            goto IL_07dd;
                        IL_07dd:
                            num = 100;
                            if ((Index == 1) | (Index == 2))
                            {
                                if (_Modul1.Instance.Aus[30] == "Y")
                                {
                                    persInArb2 = _Modul1.Instance.PersInArb;
                                    num5 = 0;
                                    goto IL_0c3f;
                                }
                                goto IL_0d45;
                            }
                            goto IL_27d8;
                        IL_0c3f: // <========== 3
                            num = 107;
                            foreach (var cLink in DataModul.Link.ReadAllFams(_Modul1.Instance.PersInArb, ELinkKennz.lkGodparent))
                            {
                                _Modul1.Instance.PersInArb = cLink.iPersNr;
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                                Anz[0].SelectionIndent = 20;
                                if (num5 == 0)
                                {
                                    Anz[0].SelectedText = "\n";
                                    Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                                    Anz[0].SelectedText = "Paten: " + _Modul1.Instance.Kont[3].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                    num5 = 1;
                                }
                                else
                                {

                                    Anz[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                }
                                if (_Modul1.Instance.Aus[31] != "Y")
                                {
                                    Datles1();
                                    if (_Modul1.Instance.Kont[11] == "")
                                    {
                                        _Modul1.Instance.Kont[11] = _Modul1.Instance.Kont[12];
                                    }
                                    if (_Modul1.Instance.Kont[11].Trim() != "")
                                    {
                                        Anz[0].SelectedText = " * " + _Modul1.Instance.Kont[11].Trim();
                                    }
                                    if (_Modul1.Instance.Kont[13] == "")
                                    {
                                        _Modul1.Instance.Kont[13] = _Modul1.Instance.Kont[14];
                                    }
                                    if ((_Modul1.Instance.Kont[14].Trim() != "") & (_Modul1.Instance.Kont[11].Trim() != ""))
                                    {
                                        Anz[0].SelectedText = ",";
                                    }
                                    if (_Modul1.Instance.Kont[14].Trim() != "")
                                    {
                                        Anz[0].SelectedText = " + " + _Modul1.Instance.Kont[13].Trim();
                                    }
                                    Anz[0].SelectedText = "; ";
                                }
                                else
                                    Anz[0].SelectedText = "; ";
                            }
                            _Modul1.Instance.PersInArb = persInArb;
                            goto IL_0c52;
                        IL_0c52: // <========== 3
                            num = 152;
                            _Modul1.Instance.PersInArb = persInArb;
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields[PersonFields.Bem2].Value) && Operators.CompareString(DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                            {
                                if (num5 == 0)
                                {
                                    Anz[0].SelectedText = " Paten: ";
                                }
                                goto IL_0cfc;
                            }
                            goto IL_0d37;
                        IL_0cfc:
                            num = 159;
                            Anz[0].SelectedText = DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString().Trim();
                            goto IL_0d37;
                        IL_0d37: // <========== 3
                            num = 162;
                            _Modul1.Instance.PersInArb = persInArb;
                            goto IL_0d45;
                        IL_0d45: // <========== 3
                            num = 164;
                            leerweg();
                            if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == ";")
                            {
                                Anz[0].SelectionStart = Anz[0].SelectionStart - 1;
                                Anz[0].SelectionLength = 2;
                                Anz[0].SelectedText = "";
                            }
                            goto IL_0df0;
                        IL_0df0:
                            num = 170;
                            if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != ".")
                            {
                                Anz[0].SelectedText = ".";
                            }
                            goto IL_0e4a;
                        IL_0e4a:
                            num = 173;
                            if (_Modul1.Instance.Aus[38] == "Y")
                            {
                                List5.Items.Clear();
                                Label1[0].Text = _Modul1.Instance.PersInArb.AsString();
                                foreach (var cLink in DataModul.Link.ReadAllPers(_Modul1.Instance.PersInArb, ELinkKennz.lkGodparent))
                                {
                                    _Modul1.Instance.PersInArb = cLink.iFamNr;
                                    DataModul.DB_EventTable.Seek("=", 102.AsString(), cLink.iFamNr.AsString(), 0);
                                    if (DataModul.DB_EventTable.NoMatch)
                                    {
                                        DataModul.DB_EventTable.Seek("=", 101.AsString(), cLink.iFamNr.AsString(), 0);
                                    }
                                    if (DataModul.DB_EventTable.NoMatch)
                                    {
                                        Datu = "          ";
                                    }
                                    else
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                    {
                                        Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                    }
                                    else
                                    {

                                        Datu = "          ";
                                    }
                                    List5.Items.Add(Datu + "                    " + _Modul1.Instance.PersInArb.AsString());
                                }
                                _Modul1.Instance.PersInArb = (int)Math.Round(Label1[0].Text.AsDouble());
                                goto IL_1225;
                            }
                            goto IL_152a;
                        IL_1225: // <========== 3
                            num = 208;
                            num6 = (short)(List5.Items.Count - 1);
                            num7 = 0;
                            goto IL_1522;
                        IL_1482: // <========== 4
                            num = 229;
                            Anz[0].SelectedText = "\nPate: " + Datu + " bei " + _Modul1.Instance.Kont[3].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                            _Modul1.Instance.PersInArb = (int)Math.Round(Label1[0].Text.AsDouble());
                            num7 = (short)unchecked(num7 + 1);
                            goto IL_1522;
                        IL_1522:
                            if (num7 <= num6)
                            {
                                _Modul1.Instance.PersInArb = (int)Math.Round(Conversion.Val(List5.Items[num7].AsString().Right(10)));
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                                DataModul.DB_EventTable.Seek("=", 102.AsString(), _Modul1.Instance.PersInArb, 0);
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    DataModul.DB_EventTable.Seek("=", 101.AsString(), _Modul1.Instance.PersInArb.AsString(), 0);
                                }
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    Datu = "          ";
                                    goto IL_1482;
                                }
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                    Ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                    Datwand1(ref Datu, ref Ds);
                                }
                                else
                                {

                                    Datu = "          ";
                                }
                                goto IL_1482;
                            }
                            goto IL_152a;
                        IL_152a: // <========== 3
                            num = 233;
                            _Modul1.Instance.PersInArb = persInArb;
                            if (_Modul1.Instance.Aus[37] == "Y")
                            {
                                Zeuge_Bei();
                            }
                            _Modul1.Instance.PersInArb = persInArb;
                            persInArb3 = _Modul1.Instance.PersInArb;
                            foreach (var cLink in DataModul.Link.ReadAllFams(_Modul1.Instance.PersInArb, ELinkKennz.lk9))
                            {
                                _Modul1.Instance.PersInArb = cLink.iPersNr;
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                                Anz[0].SelectedText = "\n";
                                Anz[0].SelectedText = "Verbundene Person: ";
                                Anz[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                Datles1();
                                if (_Modul1.Instance.Kont[11] == "")
                                {
                                    _Modul1.Instance.Kont[11] = _Modul1.Instance.Kont[12];
                                }
                                if (_Modul1.Instance.Kont[11].Trim() != "")
                                {
                                    Anz[0].SelectedText = " * " + _Modul1.Instance.Kont[11].Trim() + ".";
                                }
                                if (_Modul1.Instance.Kont[13] == "")
                                {
                                    _Modul1.Instance.Kont[13] = _Modul1.Instance.Kont[14];
                                }
                                if (_Modul1.Instance.Kont[14].Trim() != "")
                                {
                                    Anz[0].SelectedText = " + " + _Modul1.Instance.Kont[13].Trim() + ".";
                                }
                            }
                            goto IL_188a;
                        IL_188a: // <========== 3
                            num = 273;
                            _Modul1.Instance.PersInArb = persInArb;
                            foreach (var cLink in DataModul.Link.ReadAllPers(_Modul1.Instance.PersInArb, ELinkKennz.lk9))
                            {
                                _Modul1.Instance.PersInArb = cLink.iFamNr;
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                                DataModul.DB_EventTable.Seek("=", 102.AsString(), cLink.iFamNr, 0);
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    DataModul.DB_EventTable.Seek("=", 101.AsString(), cLink.iFamNr, 0);
                                }
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    Datu = "          ";
                                }
                                else
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                    Ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                    Datwand1(ref Datu, ref Ds);
                                }
                                else
                                {
                                    Datu = "          ";
                                }
                                Anz[0].SelectedText = "\nVerbunden " + Datu + " mit " + _Modul1.Instance.Kont[3].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                _Modul1.Instance.PersInArb = persInArb;
                            }
                            goto IL_1c95;
                        IL_1c95: // <========== 3
                            num = 310;
                            Per1 = "";
                            string text = "";
                            _Modul1.Instance.eLKennz = ELinkKennz.lkWitnOfEngage;
                            foreach (var cLink in DataModul.Link.ReadAllPers(_Modul1.Instance.PersInArb, ELinkKennz.lkWitnOfEngage))
                            {
                                text = Strings.Right("          " + cLink.iFamNr.AsString(), 10) + _Modul1.Instance.eLKennz;
                                Per1 += text;
                            }
                            goto IL_1e42;
                        IL_1e42: // <========== 3
                            num = 327;
                            List5.Items.Clear();
                            num8 = (int)Math.Round(Per1.Length / 11.0);
                            num9 = 1;
                            goto IL_2021;
                        IL_1fdf: // <========== 3
                            num = 340;
                            List5.Items.Add(Datu + "          " + _Modul1.Instance.FamInArb.AsString());
                            num9++;
                            goto IL_2021;
                        IL_2021:
                            if (num9 <= num8)
                            {
                                _Modul1.Instance.FamInArb = (int)Math.Round(Per1.Left(10).AsDouble());
                                Per1 = Strings.Mid(Per1, 12, Per1.Length);
                                DataModul.DB_EventTable.Seek("=", 503, _Modul1.Instance.FamInArb.AsString(), "0");
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    Datu = "    ";
                                }
                                else
                                {

                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                    {
                                        Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                    }
                                }
                                goto IL_1fdf;
                            }
                            LD = "Verlobungszeuge";
                            Art = 501f;
                            Zeugenaus(ref LD, ref Art);
                            _Modul1.Instance.PersInArb = persInArb;
                            Per1 = "";
                            text = "";
                            _Modul1.Instance.eLKennz = ELinkKennz.lkMarrWitness;
                            foreach (var cLink in DataModul.Link.ReadAllPers(_Modul1.Instance.PersInArb, ELinkKennz.lkMarrWitness))
                            {
                                text = Strings.Right("          " + cLink.iFamNr.AsString(), 10) + _Modul1.Instance.eLKennz;
                                Per1 += text;
                            }
                            goto IL_2203;
                        IL_2203: // <========== 3
                            num = 361;
                            List5.Items.Clear();
                            num10 = (int)Math.Round(Per1.Length / 11.0);
                            num9 = 1;
                            goto IL_23e2;
                        IL_23a0: // <========== 3
                            num = 374;
                            List5.Items.Add(Datu + "          " + _Modul1.Instance.FamInArb.AsString());
                            num9++;
                            goto IL_23e2;
                        IL_23e2:
                            if (num9 <= num10)
                            {
                                _Modul1.Instance.FamInArb = (int)Math.Round(Per1.Left(10).AsDouble());
                                Per1 = Strings.Mid(Per1, 12, Per1.Length);
                                DataModul.DB_EventTable.Seek("=", 503, _Modul1.Instance.FamInArb.AsString(), "0");
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    Datu = "    ";
                                }
                                else
                                {

                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                    {
                                        Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                    }
                                }
                                goto IL_23a0;
                            }
                            LD = "Trauzeuge";
                            Art = 502f;
                            Zeugenaus(ref LD, ref Art);
                            _Modul1.Instance.PersInArb = persInArb;
                            Per1 = "";
                            text = "";
                            _Modul1.Instance.eLKennz = ELinkKennz.lkWitnOfMarr;
                            foreach (var cLink in DataModul.Link.ReadAllPers(_Modul1.Instance.PersInArb, ELinkKennz.lkWitnOfMarr))
                            {
                                text = Strings.Right("          " + cLink.iFamNr.AsString(), 10) + _Modul1.Instance.eLKennz;
                                Per1 += text;
                            }
                            List5.Items.Clear();
                            num11 = (int)Math.Round(Per1.Length / 11.0);
                            num9 = 1;
                            goto IL_27a3;
                        IL_2761: // <========== 3
                            num = 408;
                            List5.Items.Add(Datu + "          " + _Modul1.Instance.FamInArb.AsString());
                            num9++;
                            goto IL_27a3;
                        IL_27a3:
                            if (num9 <= num11)
                            {
                                _Modul1.Instance.FamInArb = (int)Math.Round(Per1.Left(10).AsDouble());
                                Per1 = Strings.Mid(Per1, 12, Per1.Length);
                                DataModul.DB_EventTable.Seek("=", 503, _Modul1.Instance.FamInArb.AsString(), "0");
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    Datu = "    ";
                                }
                                else
                                {

                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                    {
                                        Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                    }
                                }
                                goto IL_2761;
                            }
                            LD = "kirchl. Trauzeuge";
                            Art = 503f;
                            Zeugenaus(ref LD, ref Art);
                            _Modul1.Instance.PersInArb = persInArb;
                            goto IL_27d8;
                        IL_27d8: // <========== 3
                            num = 413;
                            _Modul1.Instance.Ubg = 300;
                            Berufe();
                            _Modul1.Instance.Ubg = 301;
                            Berufe();
                            _Modul1.Instance.Ubg = 302;
                            Berufe();
                            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            _Modul1.Instance.Ubg = 105;
                            Berufe();
                            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            Datles1();
                            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            Anz[0].SelectedText = _Modul1.Instance.DTxt[3] + " " + _Modul1.Instance.Kont[13].Trim() + ".";
                            if (_Modul1.Instance.Aus[2] == "Y" && _Modul1.Instance.Kont[18].Length > 0)
                            {
                                QuText = _Modul1.Instance.Kont[18];
                                Zeiweg(ref QuText);
                                Anz[0].SelectedText = " " + QuText;
                                QuText = "";

                            }
                            goto IL_2a54;
                        IL_2a54: // <========== 3
                            num = 442;
                            if (_Modul1.Instance.Aus[3] == "Y" && _Modul1.Instance.Kont[23].Length > 0)
                            {
                                QuText = _Modul1.Instance.Kont[23];
                                Zeiweg(ref QuText);
                                Anz[0].SelectedText = " " + QuText;
                                QuText = "";

                            }
                            goto IL_2ad9;
                        IL_2ad9: // <========== 3
                            num = 450;
                            if (_Modul1.Instance.Kont[33].Trim() != "")
                            {
                                QuText = _Modul1.Instance.Kont[33];
                                Zeiweg(ref QuText);
                                Anz[0].SelectedText = " " + QuText + ".";
                                QuText = "";
                            }
                            if (_Modul1.Instance.Kont[43] != "")
                            {
                                Anz[0].SelectedText = " " + _Modul1.Instance.Kont[43];
                            }
                            Anz[0].SelectedText = "\n" + _Modul1.Instance.DTxt[4] + " " + _Modul1.Instance.Kont[14].Trim() + ".";
                            if (_Modul1.Instance.Aus[2] == "Y")
                            {
                                if (_Modul1.Instance.Kont[19].Length > 0)
                                {
                                    QuText = _Modul1.Instance.Kont[19];
                                    Zeiweg(ref QuText);
                                    Anz[0].SelectedText = " " + QuText;
                                    QuText = "";

                                }
                            }
                            goto IL_2c7c;
                        IL_2c7c: // <========== 3
                            num = 468;
                            if (_Modul1.Instance.Aus[3] == "Y")
                            {
                                if (_Modul1.Instance.Kont[24].Length > 0)
                                {
                                    QuText = _Modul1.Instance.Kont[24];
                                    Zeiweg(ref QuText);
                                    Anz[0].SelectedText = " " + QuText;
                                    QuText = "";

                                }
                            }
                            goto IL_2d01;
                        IL_2d01: // <========== 3
                            num = 476;
                            if (_Modul1.Instance.Kont[34].Trim() != "")
                            {
                                QuText = _Modul1.Instance.Kont[34];
                                Zeiweg(ref QuText);
                                Anz[0].SelectedText = " " + QuText + ".";
                                QuText = "";
                            }
                            goto IL_2d7a;
                        IL_2d7a:
                            num = 482;
                            if (_Modul1.Instance.Kont[44] != "")
                            {
                                Anz[0].SelectedText = " " + _Modul1.Instance.Kont[44];
                            }
                            goto IL_2dc1;
                        IL_2dc1:
                            num = 485;
                            if (Anz[0].Text.Right(1) != "\n")
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            goto IL_2e0a;
                        IL_2e0a:
                            num = 488;
                            if (_Modul1.Instance.Aus[1] == "Y")
                            {
                                if (Operators.CompareString(DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    QuText = DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString().Trim();
                                    Zeiweg(ref QuText);
                                    Anz[0].SelectedText = "{" + QuText + "}\n";
                                    QuText = "";

                                }
                            }
                            goto IL_2ed3;
                        IL_2ed3: // <========== 3
                            num = 496;
                            Quellen();
                            Persp1 = _Modul1.Instance.PersInArb;
                            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                            if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                            {
                                _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                            }
                            if (Index == 2)
                            {
                                Weitehen(ref FamSP1);
                            }
                            _Modul1.Instance.PersInArb = persInArb;
                            _Modul1.Instance.Family.Mann = 0;
                            _Modul1.Instance.Family.Frau = 0;
                            _Modul1.Instance.Ubg = Adoeltsuch(_Modul1.Instance.PersInArb);
                            Adoelt(_Modul1.Instance.Ubg);
                            _Modul1.Instance.PersInArb = persInArb;
                            _Modul1.Instance.Family.Mann = 0;
                            _Modul1.Instance.Family.Frau = 0;
                            DataModul.Link.GetPersonFam(_Modul1.Instance.PersInArb, ELinkKennz.lkChild, out _Modul1.Instance.Ubg);
                            if (_Modul1.Instance.Ubg > 0)
                            {
                                Anz[0].SelectedText = "\n";
                                Anz[0].SelectionIndent = 40;
                                ID = 200;
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                                Anz[0].SelectedText = "Eltern: \n";
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                                _Modul1.Instance.FamInArb = _Modul1.Instance.Ubg;
                                if (DataModul.Link.GetFamPerson(_Modul1.Instance.FamInArb, ELinkKennz.lkFather, out _Modul1.Instance.PersInArb))
                                {
                                    _Modul1.Instance.Family.Mann = _Modul1.Instance.PersInArb;
                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                    _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                                    if (Num == 6)
                                    {
                                        Anz[0].SelectedText = "<" + _Modul1.Instance.PersInArb.AsString() + "> " + _Modul1.Instance.Kont[3].Trim();
                                    }
                                    else
                                    {

                                        Anz[0].SelectedText = _Modul1.Instance.Kont[3].Trim();
                                    }
                                    goto IL_31fe;
                                }
                                Anz[0].SelectedText = _Modul1.Instance.IText[58] + "\n";
                                _Modul1.Instance.PersInArb = 0;
                                goto IL_3485;
                            }
                            _Modul1.Instance.FamInArb = 0;
                            goto end_IL_0000_2;
                        IL_31fe: // <========== 3
                            num = 535;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont[0];
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            if (_Modul1.Instance.Kont[4] != "")
                            {
                                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Italic);
                                Anz[0].SelectedText = " (" + _Modul1.Instance.Kont[4].TrimEnd() + ")";
                            }
                            else
                            {

                                Anz[0].SelectedText = "";
                            }
                            goto IL_3314;
                        IL_3314: // <========== 3
                            num = 545;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields["religi"].Value))
                            {
                                if (Operators.CompareString(DataModul.DB_PersonTable.Fields["religi"].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    _Modul1.Instance.Ubg = DataModul.DB_PersonTable.Fields["religi"].AsInt();
                                    LD = "";
                                    DataModul.Textlese(_Modul1.Instance.Ubg, out _Modul1.Instance.UbgT, out LD);
                                    if (_Modul1.Instance.UbgT.Trim() != "")
                                    {
                                        Anz[0].SelectedText = _Modul1.Instance.UbgT;
                                    }

                                }
                            }
                            goto IL_3438;
                        IL_3438: // <========== 3
                            num = 556;
                            nachnr();
                            goto IL_3485;
                        IL_3485: // <========== 3
                            num = 562;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            BemSch = 0;
                            if (_Modul1.Instance.PersInArb > 0)
                            {
                                Art = 0f;
                                Datr1(ref Art);
                            }
                            DataModul.DB_EventTable.Seek("=", 502, _Modul1.Instance.FamInArb.AsString(), "0");
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                DataModul.DB_EventTable.Seek("=", 503, _Modul1.Instance.FamInArb.AsString(), "0");
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                }
                                else goto IL_3c1e;
                            }
                            goto IL_35d7;
                        IL_35d7: // <========== 3
                            num = 574;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                            {
                                Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                Ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                Datwand1(ref Datu, ref Ds);
                                _Modul1.Instance.Kont1[1] = Datu;
                            }
                            goto IL_36ac;
                        IL_36ac:
                            num = 580;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                Datu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                Ds = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                Datwand1(ref Datu, ref Ds);
                                if (_Modul1.Instance.Kont1[1] != "")
                                {
                                    Datu = " / " + Datu;
                                }
                                goto IL_3785;
                            }
                            goto IL_3799;
                        IL_3785:
                            num = 587;
                            _Modul1.Instance.Kont1[3] = Datu;
                            goto IL_3799;
                        IL_3799: // <========== 3
                            num = 589;
                            _Modul1.Instance.UbgT = "";
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                            {
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Zusatz].Value))
                                {
                                    Kont2[6] = DataModul.DB_EventTable.Fields[EventFields.Zusatz].AsString();
                                }
                                Ortnr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble());
                                Schalt = 1;
                                ortles(ref Ortnr, ref Schalt);
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    _Modul1.Instance.UbgT = _Modul1.Instance.UbgT.TrimEnd() + " " + DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString().Trim();

                                }
                            }
                            goto IL_38f5;
                        IL_38f5: // <========== 3
                            num = 599;
                            num9 = 1;
                            while (num9 <= 6)
                            {
                                if (_Modul1.Instance.Kont1[num9] == "0")
                                {
                                    _Modul1.Instance.Kont1[num9] = "";
                                }
                                num9++;
                            }
                            Anz[0].SelectionIndent = 50;
                            if (Struck == 1)
                            {
                                Anz[0].SelectionIndent = 70;
                            }
                            if (Anz[0].SelectedText.Right(1) != " ")
                            {
                                Anz[0].SelectedText = " ";
                            }
                            string text2 = "";
                            if (DataModul.DB_EventTable.Fields[EventFields.Art].AsString() == "502")
                            {
                                text2 = _Modul1.Instance.DTxt[7];
                            }
                            if (DataModul.DB_EventTable.Fields[EventFields.Art].AsString() == "503")
                            {
                                text2 = _Modul1.Instance.DTxt[8];
                            }
                            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            if (Num == 6)
                            {
                                Anz[0].SelectedText = text2 + " [" + _Modul1.Instance.FamInArb.AsString() + "] " + _Modul1.Instance.Kont1[1] + " " + _Modul1.Instance.Kont1[2] + _Modul1.Instance.Kont1[3] + _Modul1.Instance.Kont1[5] + _Modul1.Instance.Kont1[6] + " " + _Modul1.Instance.UbgT + " mit\n";
                                _Modul1.Instance.UbgT = "";
                            }
                            else
                            {

                                Anz[0].SelectedText = text2 + " " + _Modul1.Instance.Kont1[1] + " " + _Modul1.Instance.Kont1[2] + _Modul1.Instance.Kont1[3] + _Modul1.Instance.Kont1[5] + _Modul1.Instance.Kont1[6] + " " + _Modul1.Instance.UbgT + " mit\n";
                                _Modul1.Instance.UbgT = "";
                            }
                            goto IL_3c1e;
                        IL_3c1e: // <========== 4
                            num = 629;
                            lErl = 33;
                            Anz[0].SelectionIndent = 40;
                            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            if (DataModul.Link.GetFamPerson(_Modul1.Instance.FamInArb, ELinkKennz.lkMother, out _Modul1.Instance.PersInArb))
                            {
                                _Modul1.Instance.Family.Frau = _Modul1.Instance.PersInArb;
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                                if (Num == 6)
                                {
                                    Anz[0].SelectedText = "<" + _Modul1.Instance.PersInArb.AsString() + "> " + _Modul1.Instance.Kont[3].Trim();
                                }
                                else
                                {

                                    Anz[0].SelectedText = _Modul1.Instance.Kont[3].Trim();
                                }
                                goto IL_3e11;
                            }
                            Anz[0].SelectedText = _Modul1.Instance.IText[58] + "\n";
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            goto end_IL_0000_2;
                        IL_3e11: // <========== 3
                            num = 648;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont[0];
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Italic);
                            if (_Modul1.Instance.Kont[4] != "")
                            {
                                Anz[0].SelectedText = " (" + _Modul1.Instance.Kont[4].TrimEnd() + ")";
                            }
                            else
                            {

                                Anz[0].SelectedText = "";
                            }
                            goto IL_3f27;
                        IL_3f27: // <========== 3
                            num = 658;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields["religi"].Value))
                            {
                                if (Operators.CompareString(DataModul.DB_PersonTable.Fields["religi"].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    _Modul1.Instance.Ubg = DataModul.DB_PersonTable.Fields["religi"].AsInt();
                                    LD = "";
                                    DataModul.Textlese(_Modul1.Instance.Ubg, out _Modul1.Instance.UbgT, out LD);
                                    if (_Modul1.Instance.UbgT.Trim() != "")
                                    {
                                        Anz[0].SelectedText = _Modul1.Instance.UbgT;
                                    }

                                }
                            }
                            goto IL_404b;
                        IL_404b: // <========== 3
                            num = 669;
                            nachnr();
                            Art = 0f;
                            Datr1(ref Art);
                            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            goto IL_4137;
                        IL_4137:
                            num = 685;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            goto end_IL_0000_2;
                        IL_41b3:
                            num4 = unchecked(num2 + 1);
                            goto IL_41b7;
                        IL_41b7:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 20:
                                case 23:
                                case 24:
                                    goto IL_01d6;
                                case 31:
                                case 32:
                                case 33:
                                case 34:
                                    goto IL_02d6;
                                case 54:
                                case 55:
                                case 56:
                                    goto IL_04bd;
                                case 62:
                                case 63:
                                case 64:
                                    goto IL_053b;
                                case 81:
                                case 82:
                                case 83:
                                    goto IL_06bf;
                                case 89:
                                case 90:
                                case 91:
                                    goto IL_0732;
                                case 96:
                                case 97:
                                    goto IL_079c;
                                case 99:
                                case 100:
                                    goto IL_07dd;
                                case 106:
                                case 107:
                                case 151:
                                    goto IL_0c3f;
                                case 110:
                                case 152:
                                    goto IL_0c52;
                                case 158:
                                case 159:
                                    goto IL_0cfc;
                                case 160:
                                case 161:
                                case 162:
                                    goto IL_0d37;
                                case 163:
                                case 164:
                                    goto IL_0d45;
                                case 169:
                                case 170:
                                    goto IL_0df0;
                                case 172:
                                case 173:
                                    goto IL_0e4a;
                                case 182:
                                case 203:
                                case 208:
                                    goto IL_1225;
                                case 218:
                                case 224:
                                case 227:
                                case 228:
                                case 229:
                                    goto IL_1482;
                                case 245:
                                case 268:
                                case 273:
                                    goto IL_188a;
                                case 280:
                                case 305:
                                case 310:
                                    goto IL_1c95;
                                case 318:
                                case 321:
                                case 327:
                                    goto IL_1e42;
                                case 334:
                                case 338:
                                case 339:
                                case 340:
                                    goto IL_1fdf;
                                case 352:
                                case 355:
                                case 361:
                                    goto IL_2203;
                                case 368:
                                case 372:
                                case 373:
                                case 374:
                                    goto IL_23a0;
                                case 402:
                                case 406:
                                case 407:
                                case 408:
                                    goto IL_2761;
                                case 412:
                                case 413:
                                    goto IL_27d8;
                                case 440:
                                case 441:
                                case 442:
                                    goto IL_2a54;
                                case 448:
                                case 449:
                                case 450:
                                    goto IL_2ad9;
                                case 466:
                                case 467:
                                case 468:
                                    goto IL_2c7c;
                                case 474:
                                case 475:
                                case 476:
                                    goto IL_2d01;
                                case 481:
                                case 482:
                                    goto IL_2d7a;
                                case 484:
                                case 485:
                                    goto IL_2dc1;
                                case 487:
                                case 488:
                                    goto IL_2e0a;
                                case 494:
                                case 495:
                                case 496:
                                    goto IL_2ed3;
                                case 531:
                                case 534:
                                case 535:
                                    goto IL_31fe;
                                case 541:
                                case 544:
                                case 545:
                                    goto IL_3314;
                                case 553:
                                case 554:
                                case 555:
                                case 556:
                                    goto IL_3438;
                                case 557:
                                case 561:
                                case 562:
                                    goto IL_3485;
                                case 572:
                                case 573:
                                case 574:
                                    goto IL_35d7;
                                case 579:
                                case 580:
                                    goto IL_36ac;
                                case 586:
                                case 587:
                                    goto IL_3785;
                                case 588:
                                case 589:
                                    goto IL_3799;
                                case 597:
                                case 598:
                                case 599:
                                    goto IL_38f5;
                                case 571:
                                case 624:
                                case 628:
                                case 629:
                                    goto IL_3c1e;
                                case 644:
                                case 647:
                                case 648:
                                    goto IL_3e11;
                                case 654:
                                case 657:
                                case 658:
                                    goto IL_3f27;
                                case 666:
                                case 667:
                                case 668:
                                case 669:
                                    goto IL_404b;
                                case 684:
                                case 685:
                                    goto IL_4137;
                                case 674:
                                case 679:
                                case 686:
                                case 691:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 19601;
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
    public void nachnr()
    {
        if ((Check1[1].Checked) | (Check1[2].Checked))
        {
            Ahnles();
            if (Check1[1].Checked && _Modul1.Instance.Kont[11].Trim() != "")
            {
                Anz[0].SelectedText = " ";
                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Underline);
                Anz[0].SelectedText = "Ahnen-Nr.: " + _Modul1.Instance.Kont[11].Trim() + ".";
                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
            }
            if (Check1[2].Checked && _Modul1.Instance.Kont[13].Trim() != "")
            {
                Anz[0].SelectedText = " ";
                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Underline);
                Anz[0].SelectedText = _Modul1.Instance.Kont[13];
                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
            }
            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
        }
    }

    public void Kindhei()
    {
        //Discarded unreachable code: IL_096c
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        short num4 = default;
        int persInArb = default;
        short num6 = default;
        short num7 = default;
        short num8 = default;
        float num9 = default;
        IList<int> aiFams;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num5;
                    string LD;
                    float Idned;
                    ELinkKennz eLKennz = default;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 2874:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0970;
                                    default:
                                        goto end_IL_0000;
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0970;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            string sPSex = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString();
                            _Modul1.Instance.eLKennz = sPSex == "F" ? ELinkKennz.lkMother : ELinkKennz.lkFather;
                            persInArb = _Modul1.Instance.PersInArb;
                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                            List5.Items.Clear();
                            num6 = (short)Math.Round(_Modul1.Instance.UbgT.Length / 10.0);
                            num4 = 1;
                            goto IL_02d1;
                        IL_00dc: // <========== 3
                            num = 13;
                            DataModul.DB_EventTable.Seek("=", num9, _Modul1.Instance.FamInArb.AsString(), "0");
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                Datu = "00000000";
                                goto IL_01b0;
                            }
                            Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                            if (num9 == 504f)
                            {
                                goto IL_01b0;
                            }
                            goto IL_01c7;
                        IL_01b0: // <========== 3
                            num = 23;
                            num9 += 1f;
                            if (num9 <= 505f)
                            {
                                goto IL_00dc;
                            }
                            goto IL_01c7;
                        IL_01c7: // <========== 3
                            num = 24;
                            if (Datu.AsDouble() == 0.0)
                            {
                                DataModul.DB_EventTable.Seek("=", 601, _Modul1.Instance.FamInArb.AsString(), "0");
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);

                                }
                            }
                            goto IL_0299;
                        IL_0299: // <========== 3
                            num = 30;
                            List5.Items.Add(Datu + _Modul1.Instance.FamInArb.AsString());
                            num4 = (short)unchecked(num4 + 1);
                            goto IL_02d1;
                        IL_02d1:
                            if (num4 <= num6)
                            {
                                _Modul1.Instance.FamInArb = _Modul1.Instance.UbgT.Left(10).AsInt();
                                _Modul1.Instance.UbgT = Strings.Mid(_Modul1.Instance.UbgT, 11, _Modul1.Instance.UbgT.Length);
                                num9 = 500f;
                                goto IL_00dc;
                            }
                            if (Struck == 1)
                            {
                                if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                                {
                                    Anz[0].SelectedText = "\n";
                                }
                                goto IL_032e;
                            }
                            goto IL_0345;
                        IL_032e:
                            num = 36;
                            Anz[0].SelectionIndent = 90;
                            goto IL_0345;
                        IL_0345: // <========== 3
                            num = 38;
                            Anz[0].SelectionIndent = 0;
                            if (Anz[0].SelectedText.Right(1) != " ")
                            {
                                Anz[0].SelectedText = " ";
                            }
                            goto IL_039e;
                        IL_039e:
                            num = 42;
                            if (List5.Items.Count == 1)
                            {
                                Anz[0].SelectedText = "Verbindung:";
                            }
                            goto IL_03cf;
                        IL_03cf:
                            num = 45;
                            if (List5.Items.Count > 1)
                            {
                                Anz[0].SelectedText = "Verbindungen:";
                            }
                            goto IL_0400;
                        IL_0400:
                            num = 48;
                            num7 = (short)(List5.Items.Count - 1);
                            num8 = 0;
                            goto IL_092a;
                        IL_04d5: // <========== 3
                            num = 59;
                            sPSex = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString();
                            eLKennz = sPSex switch
                            {
                                "F" => ELinkKennz.lkFather,
                                "M" => ELinkKennz.lkMother,
                                _ => ELinkKennz.lkNone
                            };
                            if (DataModul.Link.GetFamPerson(_Modul1.Instance.FamInArb, eLKennz, out _Modul1.Instance.PersInArb))
                            {
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                                if (Num == 6)
                                {
                                    Anz[0].SelectedText = "<" + _Modul1.Instance.PersInArb.AsString() + "> " + _Modul1.Instance.Kont[3].Trim();
                                }
                                else
                                {
                                    Anz[0].SelectedText = _Modul1.Instance.Kont[3].Trim();
                                }
                                goto IL_06ae;
                            }
                            Anz[0].SelectedText = _Modul1.Instance.IText[58] + "\n";
                            goto IL_090e;
                        IL_06ae: // <========== 3
                            num = 76;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont[0];
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Italic);
                            if (_Modul1.Instance.Kont[4] != "")
                            {
                                Anz[0].SelectedText = "(" + _Modul1.Instance.Kont[4].TrimEnd() + ") ";
                            }
                            else
                            {
                                Anz[0].SelectedText = "";
                            }
                            goto IL_07ac;
                        IL_07ac: // <========== 3
                            num = 86;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields["religi"].Value))
                            {
                                if (Operators.CompareString(DataModul.DB_PersonTable.Fields["religi"].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    _Modul1.Instance.Ubg = DataModul.DB_PersonTable.Fields["religi"].AsInt();
                                    LD = "";
                                    DataModul.Textlese(_Modul1.Instance.Ubg, out _Modul1.Instance.UbgT, out LD);
                                    if (_Modul1.Instance.UbgT.Trim() != "")
                                    {
                                        Anz[0].SelectedText = _Modul1.Instance.UbgT;
                                    }

                                }
                            }
                            goto IL_08b5;
                        IL_08b5: // <========== 3
                            num = 97;
                            nachnr();
                            ID = 600;
                            Idned = 0f;
                            Datr1(ref Idned);
                            goto IL_090e;
                        IL_090e: // <========== 3
                            num = 104;
                            _Modul1.Instance.PersInArb = persInArb;
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            num8 = (short)unchecked(num8 + 1);
                            goto IL_092a;
                        IL_092a:
                            if (num8 <= num7)
                            {
                                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List5.Items[num8].AsString(), 9, 10)));
                                Heidat();
                                if (Struck == 1)
                                {
                                    Anz[0].SelectedText = "\n";
                                    Anz[0].SelectionIndent = 90;
                                    Anz[0].SelectedText = "Mit ";
                                }
                                else
                                {
                                    Anz[0].SelectedText = " Mit ";
                                }
                                goto IL_04d5;
                            }
                            Anz[0].SelectionIndent = 0;
                            goto end_IL_0000_2;
                        IL_0970:
                            num5 = unchecked(num2 + 1);
                            num2 = 0;
                            switch (num5)
                            {
                                case 1:
                                    break;
                                case 13:
                                    goto IL_00dc;
                                case 16:
                                case 21:
                                case 22:
                                case 23:
                                    goto IL_01b0;
                                case 20:
                                case 24:
                                    goto IL_01c7;
                                case 28:
                                case 29:
                                case 30:
                                    goto IL_0299;
                                case 35:
                                case 36:
                                    goto IL_032e;
                                case 37:
                                case 38:
                                    goto IL_0345;
                                case 41:
                                case 42:
                                    goto IL_039e;
                                case 44:
                                case 45:
                                    goto IL_03cf;
                                case 47:
                                case 48:
                                    goto IL_0400;
                                case 55:
                                case 58:
                                case 59:
                                    goto IL_04d5;
                                case 72:
                                case 75:
                                case 76:
                                    goto IL_06ae;
                                case 82:
                                case 85:
                                case 86:
                                    goto IL_07ac;
                                case 94:
                                case 95:
                                case 96:
                                case 97:
                                    goto IL_08b5;
                                case 100:
                                case 103:
                                case 104:
                                    goto IL_090e;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 2874;
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
    public void Heidat()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int num4 = default;
        EEventArt num6 = default;
        EEventArt Art = default;
        bool flag = default;
        string text = default;
        string QuText = default;
        int lErl = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num5;
                    short Schalt;
                    int Ortnr;
                    string LD;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 7713:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_19af;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_19af;
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
                                num5 = num2;
                                goto IL_19b3;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            if (Num == 6)
                            {
                                Anz[0].SelectedText = "[" + _Modul1.Instance.FamInArb.AsString() + "]";
                            }
                            DataModul.DB_FamilyTable.Index = "Fam";
                            DataModul.DB_FamilyTable.Seek("=", _Modul1.Instance.FamInArb);
                            if (DataModul.DB_FamilyTable.NoMatch)
                            {
                                goto end_IL_0000_2;
                            }
                            if (DataModul.DB_FamilyTable.Fields[FamilyFields.Aeb].AsInt() == -1)
                            {
                                Anz[0].SelectedText = "Aussereheliche Verbindung";
                            }
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            num6 = EEventArt.eA_500;
                            goto IL_015c;
                        IL_015c: // <========== 3
                            num = 16;
                            if (num6 == EEventArt.eA_500)
                            {
                                Art = EEventArt.eA_501;
                            }
                            else
                            if (num6 == EEventArt.eA_501)
                            {
                                Art = EEventArt.eA_500;
                            }
                            else
                            if (num6 == EEventArt.eA_Marriage)
                            {
                                Art = EEventArt.eA_507;
                            }
                            else
                            if (num6 == EEventArt.eA_MarrReligious)
                            {
                                Art = EEventArt.eA_505;
                            }
                            else
                            if (num6 == EEventArt.eA_504)
                            {
                                Art = EEventArt.eA_Marriage;
                            }
                            else
                            if (num6 == EEventArt.eA_505)
                            {
                                Art = EEventArt.eA_MarrReligious;
                            }
                            else
                            if (num6 == EEventArt.eA_506)
                            {
                                Art = EEventArt.eA_504;
                            }
                            else
                            if (num6 == EEventArt.eA_507)
                            {
                                Art = EEventArt.eA_506;
                            }
                            Datu = "";
                            short num7 = 1;
                            while (num7 <= 6)
                            {
                                _Modul1.Instance.Kont1[num7] = "";
                                num7 = (short)unchecked(num7 + 1);
                            }
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", Art, _Modul1.Instance.FamInArb.AsString(), "0");
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                Ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                if (Datu.AsDouble() > 0.0)
                                {
                                    Datwand1(ref Datu, ref Ds);
                                    _Modul1.Instance.Kont1[1] = Datu;
                                }
                                else
                                {

                                    _Modul1.Instance.Kont1[1] = "";
                                }
                                goto IL_038f;
                            }
                            goto IL_18af;
                        IL_038f: // <========== 3
                            num = 58;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                Ds = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                Datu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                Datwand1(ref Datu, ref Ds);
                                if (_Modul1.Instance.Kont1[1] != "")
                                {
                                    Datu = " / " + Datu;
                                }
                                goto IL_0456;
                            }
                            goto IL_0467;
                        IL_0456:
                            num = 65;
                            _Modul1.Instance.Kont1[3] = Datu;
                            goto IL_0467;
                        IL_0467: // <========== 3
                            num = 67;
                            _Modul1.Instance.UbgT = "";
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                            {
                                Kont2[6] = "";
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Zusatz].Value))
                                {
                                    Kont2[6] = DataModul.DB_EventTable.Fields[EventFields.Zusatz].AsString();
                                }
                                Ortnr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble());
                                Schalt = 0;
                                ortles(ref Ortnr, ref Schalt);
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    _Modul1.Instance.UbgT = _Modul1.Instance.UbgT.TrimEnd() + " " + DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString().Trim();

                                }
                            }
                            goto IL_05bf;
                        IL_05bf: // <========== 3
                            num = 78;
                            num7 = 1;
                            while (num7 <= 6)
                            {
                                if (_Modul1.Instance.Kont1[num7] == "0")
                                {
                                    _Modul1.Instance.Kont1[num7] = "";
                                }
                                num7 = (short)unchecked(num7 + 1);
                            }
                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].Value.AsDouble() > 0.0)
                            {
                                Ortnr = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                LD = "";
                                DataModul.Textlese(Ortnr, out KontU, out LD);
                                _Modul1.Instance.UbgT = " " + KontU.Trim() + " " + _Modul1.Instance.UbgT;
                            }
                            goto IL_069a;
                        IL_069a:
                            num = 87;
                            if (DataModul.DB_EventTable.Fields[EventFields.Platz].Value.AsDouble() > 0.0)
                            {
                                Ortnr = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                LD = "";
                                DataModul.Textlese(Ortnr, out KontU, out LD);
                                _Modul1.Instance.UbgT = _Modul1.Instance.UbgT + " " + KontU.Trim() + " ";
                            }
                            goto IL_0739;
                        IL_0739:
                            num = 91;
                            if (_Modul1.Instance.Aus[34] == "Y")
                            {
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Reg].Value))
                                {
                                    if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Reg].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        _Modul1.Instance.UbgT = _Modul1.Instance.UbgT + " (Urk.-Nr.: " + DataModul.DB_EventTable.Fields[EventFields.Reg].AsString().Trim() + ") ";
                                    }

                                }
                            }
                            goto IL_07f4;
                        IL_07f4: // <========== 3
                            num = 98;
                            flag = false;
                            DataModul.DB_SourceLinkTable.Index = "Tab22";
                            DataModul.DB_SourceLinkTable.Seek("=", 3, _Modul1.Instance.FamInArb, DataModul.DB_EventTable.Fields[EventFields.Art], 0);
                            if (!DataModul.DB_SourceLinkTable.NoMatch)
                            {
                                flag = true;
                            }
                            goto IL_088d;
                        IL_088d:
                            num = 104;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem3].Value))
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    flag = true;

                                }
                            }
                            goto IL_08f1;
                        IL_08f1: // <========== 3
                            num = 109;
                            if ((_Modul1.Instance.Aus[5] == "Y") & (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0))
                            {
                                flag = true;
                            }
                            goto IL_0947;
                        IL_0947:
                            num = 112;
                            if ((_Modul1.Instance.Aus[6] == "Y") & (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0))
                            {
                                flag = true;
                            }
                            goto IL_099d;
                        IL_099d:
                            num = 115;
                            if ((_Modul1.Instance.Kont1[1].Trim() != "") | (_Modul1.Instance.Kont1[2].Trim() != "") | (_Modul1.Instance.Kont1[3].Trim() != "") | (_Modul1.Instance.Kont1[5].Trim() != "") | (_Modul1.Instance.Kont1[6].Trim() != "") | (_Modul1.Instance.UbgT.Trim() != ""))
                            {
                                flag = true;
                            }
                            goto IL_0a5a;
                        IL_0a5a:
                            num = 118;
                            if (flag)
                            {
                                text = "";
                                EEventArt num8 = Art;
                                switch (num8)
                                {
                                    case EEventArt.eA_500:
                                        text = _Modul1.Instance.DTxt[5];
                                        break;
                                    case EEventArt.eA_501:
                                        text = _Modul1.Instance.DTxt[6];
                                        break;
                                    case EEventArt.eA_Marriage:
                                        text = _Modul1.Instance.DTxt[7];
                                        break;
                                    case EEventArt.eA_MarrReligious:
                                        text = _Modul1.Instance.DTxt[8];
                                        break;
                                    case EEventArt.eA_504:
                                        text = _Modul1.Instance.DTxt[9];
                                        break;
                                    case EEventArt.eA_505:
                                        text = _Modul1.Instance.DTxt[10];
                                        break;
                                    case EEventArt.eA_507:
                                        text = _Modul1.Instance.DTxt[15];
                                        break;
                                }
                                goto IL_0b5b;
                            }
                            goto IL_15ba;
                        IL_0b5b: // <========== 8
                            num = 144;
                            if (Anz[0].SelectedText.Left(1) != " ")
                            {
                                Anz[0].SelectedText = " ";
                            }
                            goto IL_0ba4;
                        IL_0ba4:
                            num = 147;
                            if (Struck == 1)
                            {
                                Anz[0].SelectedText = "\n";
                                if (Anz[0].SelectionIndent == 0)
                                {
                                    Anz[0].SelectionIndent = 20;

                                }
                            }
                            goto IL_0c06;
                        IL_0c06: // <========== 3
                            num = 153;
                            Anz[0].SelectedText = text + " " + _Modul1.Instance.Kont1[1] + _Modul1.Instance.Kont1[2] + _Modul1.Instance.Kont1[3] + _Modul1.Instance.Kont1[5] + _Modul1.Instance.Kont1[6].TrimEnd() + " " + _Modul1.Instance.UbgT + ".";
                            _Modul1.Instance.UbgT = "";
                            LfNR = 0;
                            _Modul1.Instance.Kont1[20] = "";
                            if (DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().TrimEnd() != "")
                            {
                                if (_Modul1.Instance.Aus[5] == "Y")
                                {
                                    Anz[0].SelectedText = " {" + DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().TrimEnd() + "}";

                                }
                            }
                            goto IL_0d76;
                        IL_0d76: // <========== 3
                            num = 162;
                            if (DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().TrimEnd() != "")
                            {
                                if (_Modul1.Instance.Aus[6] == "Y")
                                {
                                    Anz[0].SelectedText = " {" + DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().TrimEnd() + "}";

                                }
                            }
                            goto IL_0e14;
                        IL_0e14: // <========== 3
                            num = 167;
                            QuText = "";
                            DataModul.DB_SourceLinkTable.Index = "Tab22";
                            DataModul.DB_SourceLinkTable.Seek("=", 3, _Modul1.Instance.FamInArb, DataModul.DB_EventTable.Fields[EventFields.Art], 0);
                            if (!DataModul.DB_SourceLinkTable.NoMatch)
                            {
                                DataModul.DB_SourceLinkTable.Seek("=", 3, _Modul1.Instance.FamInArb, DataModul.DB_EventTable.Fields[EventFields.Art], 0);
                                if (!DataModul.DB_SourceLinkTable.NoMatch)
                                {
                                    if (Operators.CompareString(Anz[0].Text.Trim().Right(1), ".", TextCompare: false) != 0)
                                    {
                                        Anz[0].SelectedText = ".";
                                    }
                                    Anz[0].SelectedText = " Quellen: ";
                                    goto IL_1427;
                                }
                            }
                            goto IL_143d;
                        IL_11a5: // <========== 3
                            num = 195;
                            if (Operators.CompareString(DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim(), "", TextCompare: false) != 0)
                            {
                                if (Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value))
                                {
                                    QuText = QuText + ", Seiten: " + DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim();
                                    goto IL_12d1;
                                }
                                QuText = QuText + ", " + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim() + " " + DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim();
                            }
                            goto IL_12d1;
                        IL_12d1: // <========== 3
                            num = 203;
                            if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value))
                            {
                                if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString() != "")
                                {
                                    QuText = (QuText + " >" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value + "<").AsString();

                                }
                            }
                            goto IL_136c;
                        IL_136c: // <========== 3
                            num = 208;
                            if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value))
                            {
                                if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString() != "")
                                {
                                    QuText = (QuText + " ==" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value + "==").AsString();

                                }
                            }
                            goto IL_1407;
                        IL_1407: // <========== 3
                            num = 213;
                            Zeiweg(ref QuText);
                            DataModul.DB_SourceLinkTable.MoveNext();
                            goto IL_1427;
                        IL_1427: // <========== 3
                            num = 180;
                            if (!DataModul.DB_SourceLinkTable.EOF)
                            {
                                if (!DataModul.DB_SourceLinkTable.NoMatch)
                                {
                                    if (!Conversions.ToBoolean((DataModul.DB_SourceLinkTable.Fields[0].AsInt() != 3)
                                        | (DataModul.DB_SourceLinkTable.Fields[1].AsInt() > _Modul1.Instance.FamInArb)
                                        | (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsInt() != DataModul.DB_EventTable.Fields[EventFields.Art].AsInt())
                                        | (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.LfNr].AsInt() != 0)))
                                    {
                                        DataModul.DB_QuTable.Index = "Nr";
                                        DataModul.DB_QuTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[2]);
                                        if (QuText != "")
                                        {
                                            QuText = (QuText + "; " + DataModul.DB_QuTable.Fields[QuFields._2].AsString());
                                        }
                                        else
                                        {

                                            QuText = DataModul.DB_QuTable.Fields[QuFields._2].AsString();
                                        }
                                        goto IL_11a5;
                                    }

                                }
                            }
                            goto IL_143d;
                        IL_143d: // <========== 4
                            num = 218;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem3].Value))
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    _Modul1.Instance.UbgT = DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().TrimEnd();
                                    Zeiweg(ref _Modul1.Instance.UbgT);
                                    if (QuText == "")
                                    {
                                        QuText = ". Quellen: " + _Modul1.Instance.UbgT.TrimEnd();
                                        goto IL_1549;
                                    }
                                    QuText = QuText.Trim() + "; " + _Modul1.Instance.UbgT.TrimEnd();
                                }
                                goto IL_1549;
                            }
                            goto IL_15ba;
                        IL_1549: // <========== 3
                            num = 229;
                            Anz[0].SelectedText = QuText;
                            QuText = "";
                            if (Anz[0].Text.Right(1) != ".")
                            {
                                Anz[0].SelectedText = ".";
                            }
                            goto IL_15ba;
                        IL_15ba: // <========== 4
                            num = 236;
                            if (_Modul1.Instance.Aus[32] == "Y")
                            {
                                Zeugsu(Art);
                                _Modul1.Instance.Kont1[20] = _Modul1.Instance.UbgT1;
                                DataModul.DB_EventTable.Seek("=", Art, _Modul1.Instance.FamInArb.AsString(), "0");
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem4].Value))
                                {
                                    if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        if (_Modul1.Instance.Kont1[20].Trim() != "")
                                        {
                                            _Modul1.Instance.Kont1[20] = _Modul1.Instance.Kont1[20] + "; " + DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim();
                                        }
                                        else
                                        {

                                            _Modul1.Instance.Kont1[20] = DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim();
                                        }

                                    }
                                }
                                goto IL_176b;
                            }
                            goto IL_18af;
                        IL_176b: // <========== 4
                            num = 250;
                            if (_Modul1.Instance.Kont1[20].Trim() != "")
                            {
                                if (Operators.CompareString(Anz[0].Text.Trim().Right(1), ".", TextCompare: false) != 0)
                                {
                                    Anz[0].SelectedText = ".";
                                }
                                if (unchecked(Art == EEventArt.eA_Marriage || Art == EEventArt.eA_MarrReligious))
                                {
                                    Anz[0].SelectedText = " Trauzeugen: " + _Modul1.Instance.Kont1[20].Trim();
                                    goto IL_1861;
                                }
                                Anz[0].SelectedText = " Zeugen: " + _Modul1.Instance.Kont1[20].Trim();
                                goto IL_1861;
                            }
                            goto IL_18af;
                        IL_1861: // <========== 3
                            num = 260;
                            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), ".", TextCompare: false) != 0)
                            {
                                Anz[0].SelectedText = ".";
                            }
                            goto IL_18af;
                        IL_18af: // <========== 5
                            num = 265;
                            lErl = 22;
                            num6++;
                            if (num6 <= EEventArt.eA_507)
                            {
                                goto IL_015c;
                            }
                            if (num4 == 1f)
                            {
                                Anz[0].SelectedText = " mit\n";
                            }
                            goto IL_1901;
                        IL_1901:
                            num = 270;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            goto end_IL_0000_2;
                        IL_19af:
                            num5 = unchecked(num2 + 1);
                            goto IL_19b3;
                        IL_19b3:
                            num2 = 0;
                            switch (num5)
                            {
                                case 1:
                                    break;
                                case 16:
                                    goto IL_015c;
                                case 54:
                                case 57:
                                case 58:
                                    goto IL_038f;
                                case 64:
                                case 65:
                                    goto IL_0456;
                                case 66:
                                case 67:
                                    goto IL_0467;
                                case 76:
                                case 77:
                                case 78:
                                    goto IL_05bf;
                                case 86:
                                case 87:
                                    goto IL_069a;
                                case 90:
                                case 91:
                                    goto IL_0739;
                                case 95:
                                case 96:
                                case 97:
                                case 98:
                                    goto IL_07f4;
                                case 103:
                                case 104:
                                    goto IL_088d;
                                case 107:
                                case 109:
                                    goto IL_08f1;
                                case 111:
                                case 112:
                                    goto IL_0947;
                                case 114:
                                case 115:
                                    goto IL_099d;
                                case 117:
                                case 118:
                                    goto IL_0a5a;
                                case 121:
                                case 125:
                                case 128:
                                case 131:
                                case 134:
                                case 137:
                                case 140:
                                case 143:
                                case 144:
                                    goto IL_0b5b;
                                case 146:
                                case 147:
                                    goto IL_0ba4;
                                case 151:
                                case 152:
                                case 153:
                                    goto IL_0c06;
                                case 160:
                                case 161:
                                case 162:
                                    goto IL_0d76;
                                case 165:
                                case 166:
                                case 167:
                                    goto IL_0e14;
                                case 191:
                                case 194:
                                case 195:
                                    goto IL_11a5;
                                case 198:
                                case 201:
                                case 202:
                                case 203:
                                    goto IL_12d1;
                                case 206:
                                case 207:
                                case 208:
                                    goto IL_136c;
                                case 211:
                                case 212:
                                case 213:
                                    goto IL_1407;
                                case 179:
                                case 180:
                                case 215:
                                    goto IL_1427;
                                case 182:
                                case 185:
                                case 216:
                                case 217:
                                case 218:
                                    goto IL_143d;
                                case 224:
                                case 227:
                                case 228:
                                case 229:
                                    goto IL_1549;
                                case 233:
                                case 234:
                                case 235:
                                case 236:
                                    goto IL_15ba;
                                case 244:
                                case 247:
                                case 248:
                                case 249:
                                case 250:
                                    goto IL_176b;
                                case 256:
                                case 259:
                                case 260:
                                    goto IL_1861;
                                case 47:
                                case 262:
                                case 263:
                                case 264:
                                case 265:
                                    goto IL_18af;
                                case 269:
                                case 270:
                                    goto IL_1901;
                                case 9:
                                case 271:
                                case 280:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 7713;
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

    public void Weitehen(ref int FamSP1)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        short num5 = default;
        short num6 = default;
        short num7 = default;
        short num8 = default;
        int persInArb = default;
        string Ds = default;
        IList<int> aiFams;
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
                        case 2438:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_07c4;
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
                                goto IL_07c8;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                            List3.Items.Clear();
                            if (_Modul1.Instance.UbgT.Length == 10)
                            {
                                goto end_IL_0000_2;
                            }
                            if (_Modul1.Instance.UbgT.Length < 21)
                            {
                                Anz[0].SelectedText = "\nWeitere Verbindung: ";
                                goto IL_0083;
                            }
                            Anz[0].SelectedText = "\nWeitere Verbindungen: ";
                            goto IL_0083;
                        IL_0083: // <========== 3
                            num = 13;
                            num7 = (short)Math.Round(_Modul1.Instance.UbgT.Length / 10.0);
                            num6 = 1;
                            goto IL_0255;
                        IL_0105: // <========== 3
                            num = 18;
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", num5, _Modul1.Instance.FamInArb.AsString(), "0");
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                Datu = "        ";
                                goto IL_0206;
                            }
                            Datu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                            {
                                if (num5 == 504)
                                {
                                    goto IL_0206;
                                }
                                goto IL_021d;
                            }
                            goto IL_0206;
                        IL_0206: // <========== 4
                            num = 31;
                            num5 = (short)unchecked(num5 + 1);
                            if (num5 <= 505)
                            {
                                goto IL_0105;
                            }
                            goto IL_021d;
                        IL_021d: // <========== 3
                            num = 32;
                            List3.Items.Add(Datu + _Modul1.Instance.FamInArb.AsString());
                            goto IL_024c;
                        IL_024c: // <========== 3
                            num = 34;
                            num6 = (short)unchecked(num6 + 1);
                            goto IL_0255;
                        IL_0255:
                            if (num6 <= num7)
                            {
                                _Modul1.Instance.FamInArb = (int)Math.Round(_Modul1.Instance.UbgT.Left(10).AsDouble());
                                _Modul1.Instance.UbgT = Strings.Mid(_Modul1.Instance.UbgT, 11, _Modul1.Instance.UbgT.Length);
                                if (_Modul1.Instance.FamInArb != FamSP1)
                                {
                                    num5 = 500;
                                    goto IL_0105;
                                }
                                goto IL_024c;
                            }
                            if (_Modul1.Instance.eLKennz == ELinkKennz.lkFather)
                            {
                                _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                            }
                            else
                            {
                                _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                            }
                            goto IL_0296;
                        IL_0296: // <========== 3
                            num = 41;
                            Datu = "";
                            num8 = (short)(List3.Items.Count - 1);
                            num6 = 0;
                            goto IL_073f;
                        IL_0359: // <========== 3
                            num = 52;
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", num5, _Modul1.Instance.FamInArb.AsString(), "0");
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                Datu = "        ";
                                goto IL_045a;
                            }
                            Datu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                            {
                                if (num5 == 504)
                                {
                                    goto IL_045a;
                                }
                                goto IL_0471;
                            }
                            goto IL_045a;
                        IL_045a: // <========== 4
                            num = 65;
                            num5 = (short)unchecked(num5 + 1);
                            if (num5 <= 505)
                            {
                                goto IL_0359;
                            }
                            goto IL_0471;
                        IL_0471: // <========== 3
                            num = 66;
                            Ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                            Datwand1(ref Datu, ref Ds);
                            goto IL_04a6;
                        IL_04a6: // <========== 3
                            num = 69;
                            if (Num == 6)
                            {
                                Anz[0].SelectedText = "[" + _Modul1.Instance.FamInArb.AsString() + "] " + Datu + " mit ";
                            }
                            else
                            {
                                Anz[0].SelectedText = Datu + " mit ";
                            }
                            goto IL_053a;
                        IL_053a: // <========== 3
                            num = 75;
                            if (DataModul.Link.GetFamPerson(_Modul1.Instance.FamInArb, _Modul1.Instance.eLKennz, out _Modul1.Instance.PersInArb))
                            {
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                                if (Num == 6)
                                {
                                    Anz[0].SelectedText = "<" + _Modul1.Instance.PersInArb.AsString() + "> " + _Modul1.Instance.Kont[3].Trim();
                                }
                                else
                                {
                                    Anz[0].SelectedText = _Modul1.Instance.Kont[3].Trim();
                                }
                                goto IL_0653;
                            }
                            Anz[0].SelectedText = " UNBEKANNT";
                            goto IL_06e3;
                        IL_0653: // <========== 3
                            num = 86;
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                            Anz[0].SelectedText = " " + _Modul1.Instance.Kont[0];
                            Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                            goto IL_06e3;
                        IL_06e3: // <========== 3
                            num = 93;
                            if (num6 < List3.Items.Count - 1)
                            {
                                Anz[0].SelectedText = "; ";
                            }
                            else
                            {
                                Anz[0].SelectedText = ".";
                            }
                            goto IL_0736;
                        IL_0736: // <========== 3
                            num = 99;
                            num6 = (short)unchecked(num6 + 1);
                            goto IL_073f;
                        IL_073f:
                            if (num6 <= num8)
                            {
                                List3.SelectedIndex = num6;
                                _Modul1.Instance.FamInArb = (int)Math.Round(Strings.Mid(List3.Text, 10, 10).AsDouble());
                                string left = List3.Text.Left(8);
                                if (_Modul1.Instance.FamInArb != 0)
                                {
                                    if (left != "        ")
                                    {
                                        num5 = 500;
                                        goto IL_0359;
                                    }
                                    goto IL_04a6;
                                }
                            }
                            _Modul1.Instance.FamInArb = FamSP1;
                            _Modul1.Instance.PersInArb = persInArb;
                            Anz[0].SelectedText = "\n";
                            goto end_IL_0000_2;
                        IL_07c4:
                            num4 = unchecked(num2 + 1);
                            goto IL_07c8;
                        IL_07c8:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 9:
                                case 12:
                                case 13:
                                    goto IL_0083;
                                case 18:
                                    goto IL_0105;
                                case 22:
                                case 28:
                                case 30:
                                case 31:
                                    goto IL_0206;
                                case 27:
                                case 32:
                                    goto IL_021d;
                                case 33:
                                case 34:
                                    goto IL_024c;
                                case 37:
                                case 40:
                                case 41:
                                    goto IL_0296;
                                case 52:
                                    goto IL_0359;
                                case 56:
                                case 62:
                                case 64:
                                case 65:
                                    goto IL_045a;
                                case 61:
                                case 66:
                                    goto IL_0471;
                                case 68:
                                case 69:
                                    goto IL_04a6;
                                case 71:
                                case 74:
                                case 75:
                                    goto IL_053a;
                                case 82:
                                case 85:
                                case 86:
                                    goto IL_0653;
                                case 89:
                                case 92:
                                case 93:
                                    goto IL_06e3;
                                case 95:
                                case 98:
                                case 99:
                                    goto IL_0736;
                                case 5:
                                case 103:
                                case 108:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 2438;
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

    public void Quellen()
    {
        var num = 2;
        DataModul.DB_SourceLinkTable.Index = "Tab";
        DataModul.DB_SourceLinkTable.Seek("=", 1, _Modul1.Instance.PersInArb);
        var QuText = "";
        while (!DataModul.DB_SourceLinkTable.EOF
            && !DataModul.DB_SourceLinkTable.NoMatch
            && !Conversions.ToBoolean((DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._1].AsInt() != 1)
            | (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].AsInt() != _Modul1.Instance.PersInArb)))
        {
            DataModul.DB_QuTable.Index = "NR";
            DataModul.DB_QuTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3]);
            if (!DataModul.DB_QuTable.NoMatch)
            {
                if (QuText.Trim().Length == 0) QuText = (QuText + DataModul.DB_QuTable.Fields[QuFields._2].AsString());
                else
                {
                    QuText = (QuText + "; " + DataModul.DB_QuTable.Fields[QuFields._2].AsString());
                }
                if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[3].Value) & (Operators.CompareString(DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim(), "", TextCompare: false) != 0))
                {
                    if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value)) QuText = QuText + ", " + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim() + " " + DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim();
                    else
                    {
                        QuText = QuText + ", Seiten: " + DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim();
                    }
                }
                if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value))
                {
                    if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString() != "")
                    {
                        QuText = (QuText + " >" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value + "<").AsString();
                    }
                }
                if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value))
                {
                    if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString() != "")
                    {
                        QuText = (QuText + " ==" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value + "==").AsString();
                    }
                }
                Zeiweg(ref QuText);
            }
            DataModul.DB_SourceLinkTable.MoveNext();
        }
        _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
        if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields[PersonFields.Bem3].Value))
        {
            if (DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString().Length > 0)
            {
                if (QuText.Trim() == "") QuText = DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString();
                else
                {
                    QuText = (QuText + ". " + DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString());
                }
            }
        }
        if (QuText.Trim() != "")
        {
            Zeiweg(ref QuText);
            Anz[0].SelectedText = "Quellen: " + QuText.Trim() + ".";
            QuText = "";
            Anz[0].SelectedText = "\n";
        }
    }
    public void Zeig()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        string text = default;
        float num4 = default;
        int num6 = default;
        string text2 = default;
        string left = default;
        string text3 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num5;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 2255:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                    case 3:
                                        break;
                                    case 1:
                                        goto IL_0745;
                                    default:
                                        goto end_IL_0000;
                                }
                                lErl = 94;
                                if (Information.Err().Number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0745;
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
                                num5 = num2;
                                goto IL_0749;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            num6 = 0;
                            _Modul1.Instance.PersInArb = DataModul.DSB_SearchTable.Fields["Nummer"].AsInt();
                            DataModul.DB_PersonTable.Index = "PerNr";
                            DataModul.DB_PersonTable.Seek("=", _Modul1.Instance.PersInArb.AsString());
                            left = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString();
                            if (Check2[4].Checked)
                            {
                                if (left != "F")
                                {
                                    goto IL_00f4;
                                }
                                goto IL_02d7;
                            }
                            goto IL_00f4;
                        IL_00f4: // <========== 3
                            num = 12;
                            if (Check2[5].Checked)
                            {
                                if (left != "M")
                                {
                                    goto IL_0125;
                                }
                                goto IL_02d7;
                            }
                            goto IL_0125;
                        IL_0125: // <========== 3
                            num = 17;
                            text3 = (" " + DataModul.DSB_SearchTable.Fields["Kenn"].AsString());
                            text2 = text3 + Strings.Right("      " + DataModul.DSB_SearchTable.Fields["Datum"].AsString(), 4) + " ";
                            if (text2.Right(4).AsDouble() == 0.0)
                            {
                                text2 = "       ";
                            }
                            text = Strings.Left((DataModul.DSB_SearchTable.Fields["Name"].Value + new string(' ', 40)).AsString(), 40);
                            num4 = Strings.InStr(text, ",");
                            if (num4 > 25f)
                            {
                                text = text.Left(25) + Strings.Mid(text, (int)Math.Round(num4), text.Length);
                            }
                            List1.Items.Add(text + new string(' ', 40).Left(40) + text2 + Strings.Right("           " + DataModul.DSB_SearchTable.Fields["Nummer"].AsString(), 10));
                            ProjectData.ClearProjectError();
                            num3 = 3;
                            if (_Modul1.Instance.Aus[12] == "")
                            {
                                _Modul1.Instance.Aus[12] = "200";
                            }
                            goto IL_02d1;
                        IL_02d1:
                            num = 32;
                            num6 = 0;
                            goto IL_02d7;
                        IL_02d7: // <========== 4
                            num = 33;
                            lErl = 92;
                            goto IL_06ab;
                        IL_02e4: // <========== 4
                            num = 36;
                            lErl = 93;
                            if (DataModul.DSB_SearchTable.EOF)
                            {
                                goto end_IL_0000_2;
                            }
                            DataModul.DSB_SearchTable.MoveNext();
                            if (DataModul.DSB_SearchTable.NoMatch)
                            {
                                goto end_IL_0000_2;
                            }
                            if (DataModul.DSB_SearchTable.EOF)
                            {
                                goto end_IL_0000_2;
                            }
                            _Modul1.Instance.PersInArb = DataModul.DSB_SearchTable.Fields["Nummer"].AsInt();
                            DataModul.DB_PersonTable.Index = "PerNr";
                            DataModul.DB_PersonTable.Seek("=", _Modul1.Instance.PersInArb.AsString());
                            left = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString();
                            if (Check2[4].Checked)
                            {
                                if (left == "F")
                                {
                                    goto IL_02e4;
                                }
                            }
                            if (Check2[5].Checked)
                            {
                                if (left == "M")
                                {
                                    goto IL_02e4;
                                }
                            }
                            if (Information.IsDBNull(DataModul.DSB_SearchTable.Fields["Kenn"].Value))
                            {
                                DataModul.DSB_SearchTable.Edit();
                                DataModul.DSB_SearchTable.Fields["Kenn"].Value = " ";
                                DataModul.DSB_SearchTable.Update();
                            }
                            if (DataModul.DSB_SearchTable.Fields["Kenn"].AsString() == "9")
                            {
                                DataModul.DSB_SearchTable.Edit();
                                DataModul.DSB_SearchTable.Fields["Kenn"].Value = " ";
                                DataModul.DSB_SearchTable.Update();
                            }
                            text3 = (" " + DataModul.DSB_SearchTable.Fields["Kenn"].AsString());
                            text2 = text3 + Strings.Right("      " + DataModul.DSB_SearchTable.Fields["Datum"].AsString(), 4) + " ";
                            if (Strings.Mid(text2, 4).AsDouble() == 0.0)
                            {
                                text2 = "       ";
                            }
                            text = Strings.Left((DataModul.DSB_SearchTable.Fields["Name"].Value + new string(' ', 40)).AsString(), 40);
                            num4 = Strings.InStr(text, ",");
                            if (num4 > 25f)
                            {
                                text = text.Left(25) + Strings.Mid(text, (int)Math.Round(num4), text.Length);
                            }
                            num6++;
                            List1.Items.Add(text + new string(' ', 40).Left(40) + text2 + Strings.Right("           " + DataModul.DSB_SearchTable.Fields["Nummer"].AsString(), 10));
                            goto IL_06ab;
                        IL_06ab: // <========== 3
                            num = 35;
                            if (num6 == _Modul1.Instance.Aus[12].AsDouble())
                            {
                                goto end_IL_0000_2;
                            }
                            goto IL_02e4;
                        IL_0745:
                            num5 = unchecked(num2 + 1);
                            goto IL_0749;
                        IL_0749:
                            num2 = 0;
                            switch (num5)
                            {
                                case 1:
                                    break;
                                case 10:
                                case 11:
                                case 12:
                                    goto IL_00f4;
                                case 15:
                                case 16:
                                case 17:
                                    goto IL_0125;
                                case 31:
                                case 32:
                                    goto IL_02d1;
                                case 9:
                                case 14:
                                case 33:
                                    goto IL_02d7;
                                case 36:
                                case 53:
                                case 58:
                                    goto IL_02e4;
                                case 34:
                                case 35:
                                case 83:
                                    goto IL_06ab;
                                case 38:
                                case 42:
                                case 45:
                                case 84:
                                case 94:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 2255;
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
    public void Zeigfam()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int number = default;
        short num5 = default;
        string text2 = default;
        string text3 = default;
        string Persex = default;
        int Fam = default;
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
                    string Persex2;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 3194:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0a60;
                                    default:
                                        goto end_IL_0000;
                                }
                                number = Information.Err().Number;
                                if (number == 3021)
                                {
                                    List1.Items.Add("------------ Ende der Liste-----------");
                                    goto end_IL_0000_3;
                                }
                                if (number == 94)
                                {
                                    DataModul.DSB_SearchTable.Edit();
                                    DataModul.DSB_SearchTable.Fields["Kenn"].Value = "9";
                                    DataModul.DSB_SearchTable.Update();
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num4 = num2;
                                    goto IL_0a64;
                                }
                                if (number != 3421)
                                {
                                    break;
                                }
                                Option1[1].Checked = true;
                                goto end_IL_0000_3;
                            }
                        end_IL_0000_2:
                            break;
                        IL_0008:
                            num = 2;
                            num5 = 0;
                            DataModul.DB_PersonTable.Index = "PerNr";
                            string text = (" " + DataModul.DSB_SearchTable.Fields["Kenn"].AsString());
                            text2 = text + Strings.Right("      " + DataModul.DSB_SearchTable.Fields["Datum"].AsString(), 6) + " ";
                            if (text2.Right(4).AsDouble() == 0.0)
                            {
                                text2 = "       ";
                            }
                            _Modul1.Instance.PersInArb = DataModul.DSB_SearchTable.Fields["Nummer"].AsInt();
                            Perles1();
                            if (_Modul1.Instance.Kont[1].Trim() != "")
                            {
                                _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0];
                            }
                            if (_Modul1.Instance.Kont[0].Length > 25)
                            {
                                _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].Left(25);
                            }
                            text3 = _Modul1.Instance.Kont[0].TrimEnd() + "," + _Modul1.Instance.Kont[3];
                            DataModul.DB_PersonTable.Index = "PerNr";
                            DataModul.DB_PersonTable.Seek("=", _Modul1.Instance.PersInArb.AsString());
                            Persex = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString();
                            var lFams = Ehesuch(_Modul1.Instance.PersInArb, Persex);
                            Kenn = Persex == "F" ? ELinkKennz.lkFather : ELinkKennz.lkMother;
                            foreach (var iFam in lFams)
                            {
                                Famzeig(iFam, ref _M_LiText, Kenn);
                                _M_LiText = "";
                            }
                            while (num5 != _Modul1.Instance.Aus[13].AsDouble())
                            {
                                text = (" " + DataModul.DSB_SearchTable.Fields["Kenn"].AsString());
                                text2 = text + Strings.Right("      " + DataModul.DSB_SearchTable.Fields["Datum"].AsString(), 4);
                                if (!Information.IsDBNull(DataModul.DSB_SearchTable.Fields["Sich"].Value))
                                {
                                    text2 = (text2 + DataModul.DSB_SearchTable.Fields["Sich"].AsString());
                                }
                                text2 = text2 + "   ".Left(7);
                                if (Conversion.Val(Strings.Mid(text2.Trim(), 2, 4)) == 0.0)
                                {
                                    text2 = "       ";
                                }
                                _Modul1.Instance.PersInArb = DataModul.DSB_SearchTable.Fields["Nummer"].AsInt();
                                Perles1();
                                if (_Modul1.Instance.Kont[1].Trim() != "")
                                {
                                    _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0];
                                }
                                text3 = _Modul1.Instance.Kont[0].TrimEnd() + "," + _Modul1.Instance.Kont[3];
                                DataModul.DB_PersonTable.Index = "PerNr";
                                DataModul.DB_PersonTable.Seek("=", _Modul1.Instance.PersInArb.AsString());
                                Persex = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString();
                                if (Check2[4].Checked)
                                {
                                    if (Persex == "F")
                                    {
                                        DataModul.DSB_SearchTable.MoveNext();
                                        continue;
                                    }
                                }
                                if (Check2[5].Checked && Persex == "M")
                                {
                                    DataModul.DSB_SearchTable.MoveNext();
                                    continue;
                                }
                                lFams = null;
                                if (Check2[0].Checked && Persex == "M")
                                {
                                    Persex2 = Persex;
                                    lFams = Ehesuch(_Modul1.Instance.PersInArb, Persex2);
                                }
                                if (Check2[1].Checked && Persex == "F")
                                {
                                    Persex2 = Persex;
                                    lFams = Ehesuch(_Modul1.Instance.PersInArb, Persex2);
                                }
                                if ((Check2[0].CheckState == CheckState.Unchecked) & (Check2[1].CheckState == CheckState.Unchecked))
                                {
                                    Persex2 = Persex;
                                    lFams = Ehesuch(_Modul1.Instance.PersInArb, Persex2);
                                }
                                Kenn = Persex == "F" ? ELinkKennz.lkFather : ELinkKennz.lkMother;
                                _M_LiText = "";
                                float num9;
                                if (lFams != null)
                                {
                                    foreach (var iFam in lFams)
                                    {
                                        Famzeig(iFam, ref _M_LiText, Kenn);
                                        num9 = Strings.InStr(text3, ",");
                                        if (num9 > 25f)
                                        {
                                            text3 = text3.Left(25) + Strings.Mid(text3, (int)Math.Round(num9), text3.Length);
                                        }
                                        List1.Items.Add(Strings.Left(text3 + new string(' ', 40).Left(40) + text2 + Strings.Right("              " + DataModul.DSB_SearchTable.Fields["Nummer"].AsString(), 10) + _M_LiText, 134) + new string(' ', 10) + Fam.AsString().Right(10));
                                        if (List1.Items.Count >= _Modul1.Instance.Aus[13].AsDouble())
                                        {
                                            goto end_IL_0000_3;
                                        }
                                    }
                                }
                                else
                                {
                                    num9 = Strings.InStr(text3, ",");
                                    if (num9 > 25f)
                                    {
                                        text3 = text3.Left(25) + Strings.Mid(text3, (int)Math.Round(num9), text3.Length);
                                    }
                                    if (Check2[2].CheckState == CheckState.Unchecked)
                                    {
                                        List1.Items.Add(text3 + new string(' ', 40).Left(40) + text2 + Strings.Right("           " + DataModul.DSB_SearchTable.Fields["Nummer"].AsString(), 10) + _M_LiText);
                                        S += 1f;
                                    }
                                    _M_LiText = "";
                                    DataModul.DSB_SearchTable.MoveNext();
                                }
                                DataModul.DSB_SearchTable.MoveNext();
                                continue;
                            }
                            goto end_IL_0000_3;
                        IL_0a60:
                            num4 = unchecked(num2 + 1);
                            goto IL_0a64;
                        IL_0a64:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 54:
                                case 59:
                                case 96:
                                case 107:
                                case 108:
                                case 32:
                                case 33:
                                case 110:
                                case 128:
                                    goto end_IL_0000_2;
                                case 91:
                                case 111:
                                case 113:
                                case 117:
                                case 123:
                                case 124:
                                case 127:
                                case 129:
                                case 130:
                                    goto end_IL_0000_3;
                            }
                            goto default;
                    }
                    num = 128;
                    if (number != 3167)
                    {
                    }
                    break;
                }
            end_IL_0000:
                ;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 3194;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_3: // <========== 5
                       // <========== 5
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void Zeigfamdat()
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int number = default;
        int lErl = default;
        float num5 = default;
        int num6 = default;
        float num8 = default;
        string text = default;
        string text2 = default;
        string text3 = default;
        int famInArb = default;
        string text4 = default;
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
                        Listleer();
                        goto IL_0009;
                    case 3102:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0a14;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_0914;
                        }
                    IL_0914:
                        num = 105;
                        number = Information.Err().Number;
                        goto IL_0924;
                    IL_0924:
                        num = 108;
                        if (number == 3021)
                        {
                            goto IL_0931;
                        }
                        goto IL_0953;
                    IL_0953:
                        num = 112;
                        if (number == 94)
                        {
                            goto IL_095d;
                        }
                        goto IL_09b9;
                    IL_095d:
                        num = 113;
                        DataModul.DSB_SearchTable.Edit();
                        goto IL_096b;
                    IL_096b:
                        num = 114;
                        DataModul.DSB_SearchTable.Fields["Kenn"].Value = "9";
                        goto IL_098d;
                    IL_098d:
                        num = 115;
                        DataModul.DSB_SearchTable.Update();
                        goto IL_099d;
                    IL_099d:
                        num = 116;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        num4 = num2;
                        goto IL_0a18;
                    IL_08c2:
                        num = 94;
                        _M_LiText = "";
                        goto IL_08d0;
                    IL_08d0:
                        num = 96;
                        lErl = 2;
                        goto IL_08d7;
                    IL_0a18:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_0009;
                            case 3:
                                goto IL_0011;
                            case 4:
                                goto IL_003c;
                            case 5:
                                goto IL_004e;
                            case 6:
                                goto IL_00a3;
                            case 9:
                                goto IL_00ae;
                            case 10:
                                goto IL_00c1;
                            case 11:
                                goto IL_0126;
                            case 12:
                                goto IL_0155;
                            case 13:
                            case 14:
                                goto IL_0164;
                            case 15:
                                goto IL_0193;
                            case 16:
                            case 17:
                                goto IL_01a2;
                            case 18:
                                goto IL_01d1;
                            case 19:
                            case 20:
                                goto IL_01e0;
                            case 21:
                                goto IL_020f;
                            case 22:
                            case 23:
                                goto IL_021e;
                            case 24:
                                goto IL_024d;
                            case 25:
                            case 26:
                                goto IL_025c;
                            case 27:
                                goto IL_028b;
                            case 28:
                            case 29:
                                goto IL_029a;
                            case 30:
                                goto IL_02c9;
                            case 31:
                            case 32:
                                goto IL_02d8;
                            case 33:
                                goto IL_0307;
                            case 34:
                            case 35:
                                goto IL_0316;
                            case 36:
                                goto IL_033d;
                            case 37:
                                goto IL_0346;
                            case 38:
                                goto IL_0350;
                            case 39:
                                goto IL_0370;
                            case 40:
                                goto IL_037e;
                            case 41:
                                goto IL_0388;
                            case 42:
                                goto IL_03cb;
                            case 43:
                            case 44:
                                goto IL_03d5;
                            case 45:
                                goto IL_03e3;
                            case 46:
                                goto IL_03ed;
                            case 47:
                                goto IL_0430;
                            case 50:
                                goto IL_043c;
                            case 51:
                                goto IL_0440;
                            case 48:
                            case 49:
                            case 52:
                            case 53:
                                goto IL_044a;
                            case 54:
                                goto IL_0459;
                            case 55:
                                goto IL_0465;
                            case 57:
                            case 58:
                                goto IL_04a8;
                            case 59:
                                goto IL_04b6;
                            case 60:
                                goto IL_04c0;
                            case 61:
                                goto IL_04de;
                            case 62:
                            case 63:
                                goto IL_0506;
                            case 64:
                                goto IL_051a;
                            case 65:
                            case 66:
                                goto IL_0533;
                            case 67:
                                goto IL_0556;
                            case 68:
                                goto IL_0569;
                            case 69:
                                goto IL_05c2;
                            case 70:
                                goto IL_05f0;
                            case 71:
                                goto IL_062f;
                            case 72:
                                goto IL_0658;
                            case 73:
                            case 74:
                                goto IL_0681;
                            case 75:
                                goto IL_0697;
                            case 76:
                                goto IL_06b2;
                            case 77:
                            case 78:
                                goto IL_06bc;
                            case 56:
                            case 79:
                                goto IL_0707;
                            case 80:
                                goto IL_070e;
                            case 81:
                                goto IL_0718;
                            case 82:
                                goto IL_0755;
                            case 83:
                                goto IL_07ac;
                            case 84:
                                goto IL_07cb;
                            case 85:
                                goto IL_07d9;
                            case 86:
                                goto IL_07e3;
                            case 87:
                                goto IL_0801;
                            case 88:
                            case 89:
                                goto IL_0829;
                            case 90:
                                goto IL_084c;
                            case 91:
                                goto IL_0861;
                            case 92:
                                goto IL_088a;
                            case 93:
                                goto IL_08b4;
                            case 94:
                                goto IL_08c2;
                            case 95:
                            case 96:
                            case 124:
                                goto IL_08d0;
                            case 97:
                                goto IL_08d7;
                            case 99:
                            case 100:
                            case 101:
                            case 102:
                                goto IL_08ef;
                            case 7:
                            case 8:
                            case 103:
                                goto IL_08fd;
                            case 105:
                                goto IL_0914;
                            case 107:
                            case 108:
                                goto IL_0924;
                            case 109:
                                goto IL_0931;
                            case 112:
                                goto IL_0953;
                            case 113:
                                goto IL_095d;
                            case 114:
                                goto IL_096b;
                            case 115:
                                goto IL_098d;
                            case 116:
                                goto IL_099d;
                            case 119:
                                goto IL_09b9;
                            case 120:
                                goto IL_09c6;
                            case 122:
                                goto IL_09de;
                            case 123:
                                goto IL_09eb;
                            default:
                                goto end_IL_0000;
                            case 98:
                            case 104:
                            case 106:
                            case 110:
                            case 111:
                            case 117:
                            case 118:
                            case 121:
                            case 125:
                            case 126:
                                goto end_IL_0000_2;
                        }
                        goto default;
                    IL_0931:
                        num = 109;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_0a14;
                    IL_08b4:
                        num = 93;
                        num5 += 1f;
                        goto IL_08c2;
                    IL_0a14:
                        num4 = num2 + 1;
                        goto IL_0a18;
                    IL_09b9:
                        num = 119;
                        if (number == 3421)
                        {
                            goto IL_09c6;
                        }
                        goto IL_09de;
                    IL_09c6:
                        num = 120;
                        Option1[1].Checked = true;
                        goto end_IL_0000_2;
                    IL_09de:
                        num = 122;
                        if (number != 3167)
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_09eb;
                    IL_09eb:
                        num = 123;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        num2 = 0;
                        goto IL_08d0;
                    IL_08d7:
                        num = 97;
                        if ((double)num5 >= _Modul1.Instance.Aus[13].AsDouble())
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_08ef;
                    IL_08ef:
                        num = 102;
                        DataModul.DB_EventTable.MoveNext();
                        goto IL_08fd;
                    IL_0009:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0011;
                    IL_0011:
                        num = 3;
                        num6 = checked((int)Math.Round((Text1.Text + "0000".Left(8)).AsDouble()));
                        goto IL_003c;
                    IL_003c:
                        num = 4;
                        DataModul.DB_EventTable.Index = "DatInd";
                        goto IL_004e;
                    IL_004e:
                        num = 5;
                        DataModul.DB_EventTable.Seek(">=", num6);
                        goto IL_00a3;
                    IL_00a3:
                        num = 6;
                        goto IL_08fd;
                    IL_08fd:
                        num = 8;
                        if (DataModul.DB_EventTable.EOF)
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_00ae;
                    IL_00ae:
                        num = 9;
                        if (!DataModul.DB_EventTable.NoMatch)
                        {
                            goto IL_00c1;
                        }
                        goto IL_08ef;
                    IL_00c1:
                        num = 10;
                        if (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() > 499
                            & (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() < 602))
                        {
                            goto IL_0126;
                        }
                        goto IL_08ef;
                    IL_0126:
                        num = 11;
                        if (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == 500)
                        {
                            goto IL_0155;
                        }
                        goto IL_0164;
                    IL_0155:
                        num = 12;
                        Kennzt = " P";
                        goto IL_0164;
                    IL_0164:
                        num = 14;
                        if (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == 501)
                        {
                            goto IL_0193;
                        }
                        goto IL_01a2;
                    IL_0193:
                        num = 15;
                        Kennzt = " V";
                        goto IL_01a2;
                    IL_01a2:
                        num = 17;
                        if (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == 502)
                        {
                            goto IL_01d1;
                        }
                        goto IL_01e0;
                    IL_01d1:
                        num = 18;
                        Kennzt = " H";
                        goto IL_01e0;
                    IL_01e0:
                        num = 20;
                        if (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == 503)
                        {
                            goto IL_020f;
                        }
                        goto IL_021e;
                    IL_020f:
                        num = 21;
                        Kennzt = " K";
                        goto IL_021e;
                    IL_021e:
                        num = 23;
                        if (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == 504)
                        {
                            goto IL_024d;
                        }
                        goto IL_025c;
                    IL_024d:
                        num = 24;
                        Kennzt = " S";
                        goto IL_025c;
                    IL_025c:
                        num = 26;
                        if (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == 505)
                        {
                            goto IL_028b;
                        }
                        goto IL_029a;
                    IL_028b:
                        num = 27;
                        Kennzt = " E";
                        goto IL_029a;
                    IL_029a:
                        num = 29;
                        if (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == 507)
                        {
                            goto IL_02c9;
                        }
                        goto IL_02d8;
                    IL_02c9:
                        num = 30;
                        Kennzt = " D";
                        goto IL_02d8;
                    IL_02d8:
                        num = 32;
                        if (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == 601)
                        {
                            goto IL_0307;
                        }
                        goto IL_0316;
                    IL_0307:
                        num = 33;
                        Kennzt = " F";
                        goto IL_0316;
                    IL_0316:
                        num = 35;
                        _Modul1.Instance.FamInArb = DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt();
                        goto IL_033d;
                    IL_033d:
                        num = 36;
                        _Modul1.Instance.Famles();
                        goto IL_0346;
                    IL_0346:
                        num = 37;
                        num8 = 0f;
                        goto IL_0350;
                    IL_0350:
                        num = 38;
                        if (Text2.Text != "")
                        {
                            goto IL_0370;
                        }
                        goto IL_043c;
                    IL_0370:
                        num = 39;
                        _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau;
                        goto IL_037e;
                    IL_037e:
                        num = 40;
                        Perles1();
                        goto IL_0388;
                    IL_0388:
                        num = 41;
                        if (Operators.CompareString(Strings.Left(_Modul1.Instance.Kont[0].ToUpper().Trim(), Text2.Text.Length), Text2.Text.ToUpper(), TextCompare: false) == 0)
                        {
                            goto IL_03cb;
                        }
                        goto IL_03d5;
                    IL_03cb:
                        num = 42;
                        num8 = 1f;
                        goto IL_03d5;
                    IL_03d5:
                        num = 44;
                        _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                        goto IL_03e3;
                    IL_03e3:
                        num = 45;
                        Perles1();
                        goto IL_03ed;
                    IL_03ed:
                        num = 46;
                        if (Operators.CompareString(Strings.Left(_Modul1.Instance.Kont[0].ToUpper().Trim(), Text2.Text.Length), Text2.Text.ToUpper(), TextCompare: false) == 0)
                        {
                            goto IL_0430;
                        }
                        goto IL_044a;
                    IL_0430:
                        num = 47;
                        num8 = 1f;
                        goto IL_044a;
                    IL_043c:
                        num = 50;
                        goto IL_0440;
                    IL_0440:
                        num = 51;
                        num8 = 1f;
                        goto IL_044a;
                    IL_044a:
                        num = 53;
                        if (num8 == 1f)
                        {
                            goto IL_0459;
                        }
                        goto IL_08d0;
                    IL_0459:
                        num = 54;
                        if (_Modul1.Instance.Family.Mann == 0)
                        {
                            goto IL_0465;
                        }
                        goto IL_04a8;
                    IL_0465:
                        num = 55;
                        _M_LiText = " " + new string(' ', 40).Left(40) + "       " + "              ".Right(10) + " ";
                        goto IL_0707;
                    IL_04a8:
                        num = 58;
                        _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                        goto IL_04b6;
                    IL_04b6:
                        num = 59;
                        Perles1();
                        goto IL_04c0;
                    IL_04c0:
                        num = 60;
                        if (_Modul1.Instance.Kont[1].Trim() != "")
                        {
                            goto IL_04de;
                        }
                        goto IL_0506;
                    IL_04de:
                        num = 61;
                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0];
                        goto IL_0506;
                    IL_0506:
                        num = 63;
                        if (_Modul1.Instance.Kont[0].Length > 25)
                        {
                            goto IL_051a;
                        }
                        goto IL_0533;
                    IL_051a:
                        num = 64;
                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].Left(25);
                        goto IL_0533;
                    IL_0533:
                        num = 66;
                        text = _Modul1.Instance.Kont[0].TrimEnd() + "," + _Modul1.Instance.Kont[3];
                        goto IL_0556;
                    IL_0556:
                        num = 67;
                        DataModul.DSB_SearchTable.Index = "Nummer";
                        goto IL_0569;
                    IL_0569:
                        num = 68;
                        DataModul.DSB_SearchTable.Seek("=", _Modul1.Instance.PersInArb);
                        goto IL_05c2;
                    IL_05c2:
                        num = 69;
                        text2 = (" " + DataModul.DSB_SearchTable.Fields["Kenn"].AsString());
                        goto IL_05f0;
                    IL_05f0:
                        num = 70;
                        text3 = text2 + Strings.Right("      " + DataModul.DSB_SearchTable.Fields["Datum"].AsString(), 4);
                        goto IL_062f;
                    IL_062f:
                        num = 71;
                        if (!Information.IsDBNull(DataModul.DSB_SearchTable.Fields["Sich"].Value))
                        {
                            goto IL_0658;
                        }
                        goto IL_0681;
                    IL_0658:
                        num = 72;
                        text3 = (text3 + DataModul.DSB_SearchTable.Fields["Sich"].AsString());
                        goto IL_0681;
                    IL_0681:
                        num = 74;
                        text3 = text3 + "   ".Left(7);
                        goto IL_0697;
                    IL_0697:
                        num = 75;
                        if (text3.Right(4).AsDouble() == 0.0)
                        {
                            goto IL_06b2;
                        }
                        goto IL_06bc;
                    IL_06b2:
                        num = 76;
                        text3 = "       ";
                        goto IL_06bc;
                    IL_06bc:
                        num = 78;
                        _M_LiText = text + new string(' ', 40).Left(40) + text3 + "              " + _Modul1.Instance.PersInArb.AsString().Right(10) + " ";
                        goto IL_0707;
                    IL_0707:
                        num = 79;
                        lErl = 4;
                        goto IL_070e;
                    IL_070e:
                        num = 80;
                        famInArb = _Modul1.Instance.FamInArb;
                        goto IL_0718;
                    IL_0718:
                        num = 81;
                        Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                        goto IL_0755;
                    IL_0755:
                        num = 82;
                        text4 = Datu.Right(2) + "." + Strings.Mid(Datu, 5, 2) + "." + Datu.Left(4);
                        goto IL_07ac;
                    IL_07ac:
                        num = 83;
                        _M_LiText = _M_LiText + text4 + Kennzt + " mit ";
                        goto IL_07cb;
                    IL_07cb:
                        num = 84;
                        _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau;
                        goto IL_07d9;
                    IL_07d9:
                        num = 85;
                        Perles1();
                        goto IL_07e3;
                    IL_07e3:
                        num = 86;
                        if (_Modul1.Instance.Kont[1].Trim() != "")
                        {
                            goto IL_0801;
                        }
                        goto IL_0829;
                    IL_0801:
                        num = 87;
                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0];
                        goto IL_0829;
                    IL_0829:
                        num = 89;
                        text = _Modul1.Instance.Kont[0].TrimEnd() + "," + _Modul1.Instance.Kont[3];
                        goto IL_084c;
                    IL_084c:
                        num = 90;
                        _M_LiText += text;
                        goto IL_0861;
                    IL_0861:
                        num = 91;
                        _M_LiText = _M_LiText + new string(' ', 135).Left(135);
                        goto IL_088a;
                    IL_088a:
                        num = 92;
                        List1.Items.Add(_M_LiText + famInArb.AsString());
                        goto IL_08b4;
                    end_IL_0000:
                        break;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 3102;
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

    public void Zeugenaus(ref string InText, ref float Art)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string text = default;
        short num5 = default;
        short num6 = default;
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
                        case 882:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_02e0;
                                    default:
                                        goto end_IL_0000;
                                }
                                goto IL_0297;
                            }
                        IL_02e0:
                            num4 = unchecked(num2 + 1);
                            goto IL_02e4;
                        IL_0297:
                            num = 28;
                            if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            goto IL_02bd;
                        IL_0231:
                            num = 24;
                            Anz[0].SelectedText = "\n" + InText + _M_LiText + " und " + text;
                            goto IL_0274;
                        IL_02bd:
                            num = 31;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_02e4;
                        IL_0274:
                            num = 25;
                            _M_LiText = "";
                            goto IL_0282;
                        IL_0282:
                            num = 26;
                            num5 = (short)unchecked(num5 + 1);
                            goto IL_028b;
                        IL_02e4:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_0008;
                                case 3:
                                    goto IL_0026;
                                case 4:
                                    goto IL_0056;
                                case 5:
                                    goto IL_00b5;
                                case 6:
                                    goto IL_00c4;
                                case 8:
                                    goto IL_00d4;
                                case 9:
                                    goto IL_00d7;
                                case 10:
                                    goto IL_0109;
                                case 7:
                                case 11:
                                case 12:
                                case 13:
                                    goto IL_014b;
                                case 14:
                                    goto IL_0161;
                                case 15:
                                    goto IL_017f;
                                case 16:
                                    goto IL_018e;
                                case 17:
                                    goto IL_0197;
                                case 18:
                                    goto IL_01a5;
                                case 19:
                                    goto IL_01ae;
                                case 20:
                                    goto IL_01da;
                                case 21:
                                    goto IL_01ee;
                                case 22:
                                    goto IL_01fc;
                                case 23:
                                    goto IL_0205;
                                case 24:
                                    goto IL_0231;
                                case 25:
                                    goto IL_0274;
                                case 26:
                                    goto IL_0282;
                                case 28:
                                    goto IL_0297;
                                case 29:
                                case 31:
                                    goto IL_02bd;
                                default:
                                    goto end_IL_0000;
                                case 27:
                                case 32:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                        IL_0008:
                            num = 2;
                            num6 = (short)(List5.Items.Count - 1);
                            num5 = 0;
                            goto IL_028b;
                        IL_028b:
                            if (num5 > num6)
                            {
                                goto end_IL_0000_2;
                            }
                            goto IL_0026;
                        IL_0026:
                            num = 3;
                            _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(List5.Items[num5].AsString().Right(10)));
                            goto IL_0056;
                        IL_0056:
                            num = 4;
                            DataModul.DB_EventTable.Seek("=", Art, _Modul1.Instance.FamInArb.AsString(), "0");
                            goto IL_00b5;
                        IL_00b5:
                            num = 5;
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                goto IL_00c4;
                            }
                            goto IL_00d4;
                        IL_00c4:
                            num = 6;
                            Datu = "    ";
                            goto IL_014b;
                        IL_00d4:
                            num = 8;
                            goto IL_00d7;
                        IL_00d7:
                            num = 9;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                            {
                                goto IL_0109;
                            }
                            goto IL_014b;
                        IL_0109:
                            num = 10;
                            Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                            goto IL_014b;
                        IL_014b:
                            num = 13;
                            Datwand1(ref Datu, ref Ds);
                            goto IL_0161;
                        IL_0161:
                            num = 14;
                            _M_LiText = " " + Datu + " bei ";
                            goto IL_017f;
                        IL_017f:
                            num = 15;
                            Datu = "";
                            goto IL_018e;
                        IL_018e:
                            num = 16;
                            _Modul1.Instance.Famles();
                            goto IL_0197;
                        IL_0197:
                            num = 17;
                            _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                            goto IL_01a5;
                        IL_01a5:
                            num = 18;
                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            goto IL_01ae;
                        IL_01ae:
                            num = 19;
                            text = _Modul1.Instance.Kont[3].Trim() + " " + _Modul1.Instance.Kont[0].Trim().ToUpper();
                            goto IL_01da;
                        IL_01da:
                            num = 20;
                            _M_LiText += text;
                            goto IL_01ee;
                        IL_01ee:
                            num = 21;
                            _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau;
                            goto IL_01fc;
                        IL_01fc:
                            num = 22;
                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            goto IL_0205;
                        IL_0205:
                            num = 23;
                            text = _Modul1.Instance.Kont[3].Trim() + " " + _Modul1.Instance.Kont[0].Trim().ToUpper();
                            goto IL_0231;
                        end_IL_0000:
                            break;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 882;
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

    public void Zeugsu(EEventArt Art)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string text3 = default;
        string text = default;
        string text2 = default;
        short num5 = default;
        short num6 = default;
        string text4 = default;
        short num7 = default;
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
                        case 2150:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0704;
                                    default:
                                        goto end_IL_0000;
                                }
                                goto IL_06bb;
                            }
                        IL_0704:
                            num4 = unchecked(num2 + 1);
                            goto IL_0708;
                        IL_06bb:
                            num = 80;
                            if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            goto IL_06e1;
                        IL_0633:
                            num = 74;
                            goto IL_0637;
                        IL_06e1:
                            num = 83;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_0708;
                        IL_0637:
                            num = 75;
                            text3 = " " + _Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[0].Trim().ToUpper() + " " + text + " " + text2;
                            goto IL_069a;
                        IL_069a:
                            num = 77;
                            num5 = (short)unchecked(num5 + 1);
                            goto IL_06a3;
                        IL_0708:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_0008;
                                case 3:
                                    goto IL_0016;
                                case 4:
                                    goto IL_0022;
                                case 6:
                                    goto IL_0032;
                                case 7:
                                    goto IL_0035;
                                case 5:
                                case 8:
                                case 9:
                                    goto IL_0043;
                                case 10:
                                    goto IL_0056;
                                case 11:
                                    goto IL_00b0;
                                case 12:
                                    goto IL_00c3;
                                case 13:
                                    goto IL_00d1;
                                case 14:
                                    goto IL_0133;
                                case 15:
                                    goto IL_0139;
                                case 17:
                                case 18:
                                    goto IL_014e;
                                case 20:
                                case 21:
                                    goto IL_0163;
                                case 23:
                                case 24:
                                    goto IL_01c6;
                                case 26:
                                case 27:
                                    goto IL_022b;
                                case 28:
                                    goto IL_023b;
                                case 29:
                                    goto IL_024c;
                                case 31:
                                    goto IL_0257;
                                case 32:
                                    goto IL_025b;
                                case 33:
                                    goto IL_02bd;
                                case 30:
                                case 34:
                                case 35:
                                    goto IL_02d4;
                                case 36:
                                    goto IL_02e2;
                                case 16:
                                case 19:
                                case 22:
                                case 25:
                                case 37:
                                    goto IL_02f3;
                                case 38:
                                    goto IL_0304;
                                case 39:
                                    goto IL_030f;
                                case 40:
                                    goto IL_0338;
                                case 41:
                                    goto IL_034b;
                                case 42:
                                    goto IL_036d;
                                case 43:
                                    goto IL_038b;
                                case 44:
                                    goto IL_0391;
                                case 45:
                                    goto IL_03a1;
                                case 46:
                                    goto IL_03af;
                                case 47:
                                    goto IL_03c9;
                                case 48:
                                case 49:
                                    goto IL_03d3;
                                case 50:
                                    goto IL_03dd;
                                case 51:
                                    goto IL_03e7;
                                case 52:
                                    goto IL_0406;
                                case 53:
                                case 54:
                                    goto IL_042b;
                                case 55:
                                    goto IL_046b;
                                case 56:
                                case 57:
                                    goto IL_0490;
                                case 58:
                                    goto IL_04af;
                                case 59:
                                case 60:
                                    goto IL_04c1;
                                case 61:
                                    goto IL_04e0;
                                case 62:
                                case 63:
                                    goto IL_0505;
                                case 64:
                                    goto IL_0545;
                                case 65:
                                case 66:
                                    goto IL_056a;
                                case 67:
                                    goto IL_0589;
                                case 68:
                                case 69:
                                    goto IL_059b;
                                case 70:
                                    goto IL_05a4;
                                case 71:
                                    goto IL_05ae;
                                case 72:
                                    goto IL_05c7;
                                case 74:
                                    goto IL_0633;
                                case 75:
                                    goto IL_0637;
                                case 73:
                                case 76:
                                case 77:
                                    goto IL_069a;
                                case 78:
                                    goto IL_06ab;
                                case 80:
                                    goto IL_06bb;
                                case 81:
                                case 83:
                                    goto IL_06e1;
                                default:
                                    goto end_IL_0000;
                                case 79:
                                case 84:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                        IL_0008:
                            num = 2;
                            Per1 = "";
                            goto IL_0016;
                        IL_0016:
                            num = 3;
                            if (Art < EEventArt.eA_499)
                            {
                                goto IL_0022;
                            }
                            goto IL_0032;
                        IL_0022:
                            num = 4;
                            Nr = _Modul1.Instance.PersInArb;
                            goto IL_0043;
                        IL_0032:
                            num = 6;
                            goto IL_0035;
                        IL_0035:
                            num = 7;
                            Nr = _Modul1.Instance.FamInArb;
                            goto IL_0043;
                        IL_0043:
                            num = 9;
                            DataModul.DB_WitnessTable.Index = "FamSu";
                            goto IL_0056;
                        IL_0056:
                            num = 10;
                            DataModul.DB_WitnessTable.Seek("=", Nr, "10");
                            goto IL_00b0;
                        IL_00b0:
                            num = 11;
                            DataModul.DB_WitnessTable.Index = "ZeugSu";
                            goto IL_00c3;
                        IL_00c3:
                            num = 12;
                            _Modul1.Instance.eWKennz = "10";
                            goto IL_00d1;
                        IL_00d1:
                            num = 13;
                            DataModul.DB_WitnessTable.Seek("=", Nr, _Modul1.Instance.eWKennz, Art, LfNR);
                            goto IL_0133;
                        IL_0133:
                            num = 14;
                            num6 = 1;
                            goto IL_0139;
                        IL_0139:
                            num = 15;
                            if (!DataModul.DB_WitnessTable.EOF)
                            {
                                goto IL_014e;
                            }
                            goto IL_02f3;
                        IL_014e:
                            num = 18;
                            if (!DataModul.DB_WitnessTable.NoMatch)
                            {
                                goto IL_0163;
                            }
                            goto IL_02f3;
                        IL_0163:
                            num = 21;
                            if (!Conversions.ToBoolean((DataModul.DB_WitnessTable.Fields[WitnessFields.FamNr].AsInt() != Nr)
                                | (DataModul.DB_WitnessTable.Fields[WitnessFields.Kennz].AsInt() != 10)))
                            {
                                goto IL_01c6;
                            }
                            goto IL_02f3;
                        IL_01c6:
                            num = 24;
                            if (!Conversions.ToBoolean((DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsEnum<EEventArt>() != Art)
                                | (DataModul.DB_WitnessTable.Fields[WitnessFields.LfNr].AsInt() != LfNR)))
                            {
                                goto IL_022b;
                            }
                            goto IL_02f3;
                        IL_022b:
                            num = 27;
                            if (DataModul.DB_WitnessTable.NoMatch)
                            {
                                goto IL_023b;
                            }
                            goto IL_0257;
                        IL_023b:
                            num = 28;
                            Interaction.MsgBox("F26");
                            goto IL_024c;
                        IL_024c:
                            num = 29;
                            Debugger.Break();
                            goto IL_02d4;
                        IL_0257:
                            num = 31;
                            goto IL_025b;
                        IL_025b:
                            num = 32;
                            text4 = DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsString() + Strings.Right("          " + DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsString(), 10);
                            goto IL_02bd;
                        IL_02bd:
                            num = 33;
                            Per1 += text4;
                            goto IL_02d4;
                        IL_02d4:
                            num = 35;
                            DataModul.DB_WitnessTable.MoveNext();
                            goto IL_02e2;
                        IL_02e2:
                            num = 36;
                            num6 = (short)unchecked(num6 + 1);
                            if (num6 <= 99)
                            {
                                goto IL_0139;
                            }
                            goto IL_02f3;
                        IL_02f3:
                            num = 37;
                            _Modul1.Instance.Kont1[20] = "";
                            goto IL_0304;
                        IL_0304:
                            num = 38;
                            text3 = "";
                            goto IL_030f;
                        IL_030f:
                            num = 39;
                            num7 = (short)Math.Round(Per1.Length / 14.0);
                            num5 = 1;
                            goto IL_06a3;
                        IL_06a3:
                            if (num5 <= num7)
                            {
                                goto IL_0338;
                            }
                            goto IL_06ab;
                        IL_06ab:
                            num = 78;
                            _Modul1.Instance.UbgT1 = text3;
                            goto end_IL_0000_2;
                        IL_0338:
                            num = 40;
                            text4 = Per1.Left(14);
                            goto IL_034b;
                        IL_034b:
                            num = 41;
                            Per1 = Strings.Mid(Per1, 15, Per1.Length);
                            goto IL_036d;
                        IL_036d:
                            num = 42;
                            _Modul1.Instance.PersInArb = (int)Math.Round(Strings.Mid(text4, 5, 10).AsDouble());
                            goto IL_038b;
                        IL_038b:
                            num = 43;
                            num6 = 1;
                            goto IL_0391;
                        IL_0391:
                            num = 44;
                            _Modul1.Instance.Kont[num6] = "";
                            goto IL_03a1;
                        IL_03a1:
                            num = 45;
                            num6 = (short)unchecked(num6 + 1);
                            if (num6 <= 99)
                            {
                                goto IL_0391;
                            }
                            goto IL_03af;
                        IL_03af:
                            num = 46;
                            if (_Modul1.Instance.Aus[33] != "Y")
                            {
                                goto IL_03c9;
                            }
                            goto IL_03d3;
                        IL_03c9:
                            num = 47;
                            Datles2();
                            goto IL_03d3;
                        IL_03d3:
                            num = 49;
                            text = "";
                            goto IL_03dd;
                        IL_03dd:
                            num = 50;
                            text2 = "";
                            goto IL_03e7;
                        IL_03e7:
                            num = 51;
                            if (_Modul1.Instance.Kont[11].Trim() != "")
                            {
                                goto IL_0406;
                            }
                            goto IL_042b;
                        IL_0406:
                            num = 52;
                            _Modul1.Instance.Kont[11] = _Modul1.Instance.DTxt[1] + " " + _Modul1.Instance.Kont[11];
                            goto IL_042b;
                        IL_042b:
                            num = 54;
                            if ((_Modul1.Instance.Kont[11].Trim() == "") & (_Modul1.Instance.Kont[12].Trim() != ""))
                            {
                                goto IL_046b;
                            }
                            goto IL_0490;
                        IL_046b:
                            num = 55;
                            _Modul1.Instance.Kont[11] = _Modul1.Instance.DTxt[2] + " " + _Modul1.Instance.Kont[12];
                            goto IL_0490;
                        IL_0490:
                            num = 57;
                            if (_Modul1.Instance.Kont[11].Trim() != "")
                            {
                                goto IL_04af;
                            }
                            goto IL_04c1;
                        IL_04af:
                            num = 58;
                            text = _Modul1.Instance.Kont[11].Trim();
                            goto IL_04c1;
                        IL_04c1:
                            num = 60;
                            if (_Modul1.Instance.Kont[13].Trim() != "")
                            {
                                goto IL_04e0;
                            }
                            goto IL_0505;
                        IL_04e0:
                            num = 61;
                            _Modul1.Instance.Kont[13] = _Modul1.Instance.DTxt[3] + " " + _Modul1.Instance.Kont[13];
                            goto IL_0505;
                        IL_0505:
                            num = 63;
                            if ((_Modul1.Instance.Kont[13].Trim() == "") & (_Modul1.Instance.Kont[14].Trim() != ""))
                            {
                                goto IL_0545;
                            }
                            goto IL_056a;
                        IL_0545:
                            num = 64;
                            _Modul1.Instance.Kont[13] = _Modul1.Instance.DTxt[4] + " " + _Modul1.Instance.Kont[14];
                            goto IL_056a;
                        IL_056a:
                            num = 66;
                            if (_Modul1.Instance.Kont[13].Trim() != "")
                            {
                                goto IL_0589;
                            }
                            goto IL_059b;
                        IL_0589:
                            num = 67;
                            text2 = _Modul1.Instance.Kont[13].Trim();
                            goto IL_059b;
                        IL_059b:
                            num = 69;
                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            goto IL_05a4;
                        IL_05a4:
                            num = 70;
                            _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                            goto IL_05ae;
                        IL_05ae:
                            num = 71;
                            if (text3.Trim() != "")
                            {
                                goto IL_05c7;
                            }
                            goto IL_0633;
                        IL_05c7:
                            num = 72;
                            text3 = text3 + "; " + _Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[0].Trim().ToUpper() + " " + text + " " + text2;
                            goto IL_069a;
                        end_IL_0000:
                            break;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 2150;
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

    public void Adoelt(int iFamNr)
    {
        ID = 40;
        checked
        {
            if (iFamNr > 0)
            {
                Anz[0].SelectionIndent = ID;
                Anz[0].SelectedText = "\n";
                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                Anz[0].SelectedText = "Adoptiveltern: \n";
                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                _Modul1.Instance.FamInArb = iFamNr;
                if (DataModul.Link.GetFamPerson(_Modul1.Instance.FamInArb, ELinkKennz.lkFather, out _Modul1.Instance.PersInArb))
                {
                    _Modul1.Instance.Family.Mann = _Modul1.Instance.PersInArb;
                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                    _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                    if (Num == 6)
                    {
                        Anz[0].SelectedText = "<" + _Modul1.Instance.PersInArb.AsString() + "> " + _Modul1.Instance.Kont[3].Trim();
                    }
                    else
                    {
                        Anz[0].SelectedText = _Modul1.Instance.Kont[3].Trim();
                    }
                    Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                    Anz[0].SelectedText = " " + _Modul1.Instance.Kont[0];
                    Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                    if (_Modul1.Instance.Kont[4] != "")
                    {
                        Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Italic);
                        Anz[0].SelectedText = " (" + _Modul1.Instance.Kont[4].TrimEnd() + ")";
                    }
                    else
                    {
                        Anz[0].SelectedText = "";
                    }
                    Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                    _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                    if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields["religi"].Value) && Operators.CompareString(DataModul.DB_PersonTable.Fields["religi"].AsString().Trim(), "", TextCompare: false) != 0)
                    {
                        iFamNr = DataModul.DB_PersonTable.Fields["religi"].AsInt();
                        string LD;
                        DataModul.Textlese(iFamNr, out _Modul1.Instance.UbgT, out LD);
                        if (_Modul1.Instance.UbgT.Trim() != "")
                        {
                            Anz[0].SelectedText = _Modul1.Instance.UbgT;
                        }
                    }
                    nachnr();
                }
                else
                {
                    Anz[0].SelectedText = _Modul1.Instance.IText[58] + "\n";
                    _Modul1.Instance.PersInArb = 0;
                }
                Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                BemSch = 0;
                float Idned = 0f;
                Datr1(ref Idned);
                DataModul.DB_EventTable.Seek("=", 502, _Modul1.Instance.FamInArb.AsString(), "0");
                if (DataModul.DB_EventTable.NoMatch)
                {
                    DataModul.DB_EventTable.Seek("=", 503, _Modul1.Instance.FamInArb.AsString(), "0");
                }
                if (!DataModul.DB_EventTable.NoMatch)
                {
                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                    {
                        Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                        Ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                        Datwand1(ref Datu, ref Ds);
                        _Modul1.Instance.Kont1[1] = Datu;
                    }
                    if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                    {
                        Datu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                        Ds = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                        Datwand1(ref Datu, ref Ds);
                        if (_Modul1.Instance.Kont1[1] != "")
                        {
                            Datu = " / " + Datu;
                        }
                        _Modul1.Instance.Kont1[3] = Datu;
                    }
                    _Modul1.Instance.UbgT = "";
                    if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                    {
                        int Ortnr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble());
                        short Schalt = 1;
                        ortles(ref Ortnr, ref Schalt);
                        if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString().Trim(), "", TextCompare: false) != 0)
                        {
                            _Modul1.Instance.UbgT = _Modul1.Instance.UbgT.TrimEnd() + " " + DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString().Trim();
                        }
                    }
                    short num = 1;
                    do
                    {
                        if (_Modul1.Instance.Kont1[num] == "0")
                        {
                            _Modul1.Instance.Kont1[num] = "";
                        }
                        num = (short)unchecked(num + 1);
                    }
                    while (num <= 6);
                    Anz[0].SelectionIndent = ID + 10;
                    if (Struck == 1)
                    {
                        Anz[0].SelectionIndent = ID + 50;
                    }
                    string text = _Modul1.Instance.DTxt[7];
                    if (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == 503)
                    {
                        text = _Modul1.Instance.DTxt[8];
                    }
                    if (Num == 6)
                    {
                        Anz[0].SelectedText = "\n" + text + " [" + _Modul1.Instance.FamInArb.AsString() + "] " + _Modul1.Instance.Kont1[1] + " " + _Modul1.Instance.Kont1[2] + _Modul1.Instance.Kont1[3] + _Modul1.Instance.Kont1[5] + _Modul1.Instance.Kont1[6] + " " + _Modul1.Instance.UbgT + " mit\n";
                        _Modul1.Instance.UbgT = "";
                    }
                    else
                    {
                        Anz[0].SelectedText = "\n" + text + " " + _Modul1.Instance.Kont1[1] + " " + _Modul1.Instance.Kont1[2] + _Modul1.Instance.Kont1[3] + _Modul1.Instance.Kont1[5] + _Modul1.Instance.Kont1[6] + " " + _Modul1.Instance.UbgT + " mit\n";
                        _Modul1.Instance.UbgT = "";
                    }
                }
                if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                {
                    Anz[0].SelectedText = "\n";
                }
                if (DataModul.Link.GetFamPerson(_Modul1.Instance.FamInArb, ELinkKennz.lkMother, out _Modul1.Instance.PersInArb))
                {
                    _Modul1.Instance.Family.Frau = _Modul1.Instance.PersInArb;
                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                    _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                    if (Num == 6)
                    {
                        Anz[0].SelectedText = "<" + _Modul1.Instance.PersInArb.AsString() + "> " + _Modul1.Instance.Kont[3].Trim();
                    }
                    else
                    {
                        Anz[0].SelectedText = _Modul1.Instance.Kont[3].Trim();
                    }
                    Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                    Anz[0].SelectedText = " " + _Modul1.Instance.Kont[0];
                    Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                    Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Italic);
                    if (_Modul1.Instance.Kont[4] != "")
                    {
                        Anz[0].SelectedText = " (" + _Modul1.Instance.Kont[4].TrimEnd() + ")";
                    }
                    else
                    {
                        Anz[0].SelectedText = "";
                    }
                    Anz[0].SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                    _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                    if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields["religi"].Value) && Operators.CompareString(DataModul.DB_PersonTable.Fields["religi"].AsString().Trim(), "", TextCompare: false) != 0)
                    {
                        iFamNr = DataModul.DB_PersonTable.Fields["religi"].AsInt();
                        string LD;
                        DataModul.Textlese(iFamNr, out _Modul1.Instance.UbgT, out LD);
                        if (_Modul1.Instance.UbgT.Trim() != "")
                        {
                            Anz[0].SelectedText = _Modul1.Instance.UbgT;
                        }
                    }
                    nachnr();
                    Idned = 0f;
                    Datr1(ref Idned);
                    if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                    {
                        Anz[0].SelectedText = "\n";
                    }
                }
                else
                {
                    Anz[0].SelectedText = _Modul1.Instance.IText[58] + "\n";
                }
            }
            else
            {
                _Modul1.Instance.FamInArb = 0;
            }
        }
    }

    public int Adoeltsuch(int persInArb)
    {
        string text = "";
        foreach (var cLink in DataModul.Link.ReadAllPers(persInArb, ELinkKennz.lkAdoptedChild))
        {
            text += Strings.Right("         " + cLink.iFamNr.AsString(), 10);
        }
        if (text.Length > 10)
        {
            string text2 = "Person " + persInArb.AsString() + " ist in den Familien " + text + " als Kind eingebunden. Eine Person kann aber nur in einer Familie als Kind sein.";
            text2 += "\nBitte diesen Fehler zuerst korrigieren.";
            Interaction.MsgBox(text2, MsgBoxStyle.Exclamation, "Schwerer Datenfehler");
            return 0;
        }
        return text.AsInt();
    }


    private void Text1_TextChanged(object eventSender, EventArgs eventArgs)
    {
        Listleer();
    }

    public void Listbox3Clip(ref ListBox DL, ref short A)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string text = default;
        string text2 = default;
        short num5 = default;
        short num6 = default;
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
                        case 486:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0178;
                                    default:
                                        goto end_IL_0000;
                                }
                                goto IL_012f;
                            }
                        IL_0178:
                            num4 = unchecked(num2 + 1);
                            goto IL_017c;
                        IL_012f:
                            num = 19;
                            if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            goto IL_0155;
                        IL_0060:
                            num = 10;
                            text += DL.Text.Left(100);
                            goto IL_007a;
                        IL_0155:
                            num = 22;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_017c;
                        IL_007a:
                            num = 11;
                            text2 = text2 + text + "\r\n";
                            goto IL_008b;
                        IL_008b:
                            num = 12;
                            num5 = (short)unchecked(num5 + 1);
                            goto IL_0094;
                        IL_017c:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_0008;
                                case 3:
                                    goto IL_0011;
                                case 4:
                                    goto IL_0023;
                                case 5:
                                    goto IL_002b;
                                case 6:
                                case 7:
                                    goto IL_0034;
                                case 8:
                                    goto IL_004b;
                                case 9:
                                    goto IL_0056;
                                case 10:
                                    goto IL_0060;
                                case 11:
                                    goto IL_007a;
                                case 12:
                                    goto IL_008b;
                                case 13:
                                    goto IL_0098;
                                case 14:
                                    goto IL_00b1;
                                case 15:
                                    goto IL_00cf;
                                case 16:
                                    goto IL_00e9;
                                case 17:
                                    goto IL_0102;
                                case 19:
                                    goto IL_012f;
                                case 20:
                                case 22:
                                    goto IL_0155;
                                default:
                                    goto end_IL_0000;
                                case 18:
                                case 23:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                        IL_0008:
                            num = 2;
                            text2 = "";
                            goto IL_0011;
                        IL_0011:
                            num = 3;
                            MyProject.Computer.Clipboard.Clear();
                            goto IL_0023;
                        IL_0023:
                            num = 4;
                            if (A == 1)
                            {
                                goto IL_002b;
                            }
                            goto IL_0034;
                        IL_002b:
                            num = 5;
                            text2 = "Seiteneinstellung auf A4 Querformat einstellen\n\n";
                            goto IL_0034;
                        IL_0034:
                            num = 7;
                            num6 = (short)(DL.Items.Count - 1);
                            num5 = 0;
                            goto IL_0094;
                        IL_0094:
                            if (num5 <= num6)
                            {
                                goto IL_004b;
                            }
                            goto IL_0098;
                        IL_0098:
                            num = 13;
                            FileSystem.FileClose(99);
                            goto IL_00b1;
                        IL_00b1:
                            num = 14;
                            FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "\\Temp\\Text4.Txt", OpenMode.Output);
                            goto IL_00cf;
                        IL_00cf:
                            num = 15;
                            FileSystem.PrintLine(99, text2);
                            goto IL_00e9;
                        IL_00e9:
                            num = 16;
                            FileSystem.FileClose(99);
                            goto IL_0102;
                        IL_0102:
                            num = 17;
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.GenFreeDir + "\\Temp\\Text4.Txt", AppWinStyle.NormalFocus);
                            goto end_IL_0000_2;
                        IL_004b:
                            num = 8;
                            DL.SelectedIndex = num5;
                            goto IL_0056;
                        IL_0056:
                            num = 9;
                            text = "";
                            goto IL_0060;
                        end_IL_0000:
                            break;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 486;
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

    public void Zeuge_Bei()
    {
        //Discarded unreachable code: IL_051d
        Per1 = "";
        DataModul.DB_WitnessTable.Index = "ElSu";
        DataModul.DB_WitnessTable.Seek("=", _Modul1.Instance.PersInArb, "10");
        Per1 = "";
        List3.Items.Clear();
        short num = 1;
        checked
        {
            string text;
            do
            {
                if (DataModul.DB_WitnessTable.EOF
|| DataModul.DB_WitnessTable.NoMatch
|| Conversions.ToBoolean((DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsInt() != _Modul1.Instance.PersInArb)
| (DataModul.DB_WitnessTable.Fields[WitnessFields.Kennz].AsString() != "10")))
                {
                    break;
                }
                if (DataModul.DB_WitnessTable.NoMatch)
                {
                    Interaction.MsgBox("F26");
                    Debugger.Break();
                }
                else
                {
                    text = DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsString() + DataModul.DB_WitnessTable.Fields[WitnessFields.LfNr].AsString() + Strings.Right("          " + DataModul.DB_WitnessTable.Fields[WitnessFields.FamNr].AsString(), 10);
                    DataModul.DB_EventTable.Index = "ArtNr";
                    DataModul.DB_EventTable.Seek("=", DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsString(), DataModul.DB_WitnessTable.Fields[WitnessFields.FamNr], DataModul.DB_WitnessTable.Fields[WitnessFields.LfNr]);
                    string text2;
                    if (!DataModul.DB_EventTable.NoMatch)
                    {
                        text2 = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                        if (text2.AsDouble() == 0.0)
                        {
                            text2 = "        ";
                        }
                    }
                    else
                    {
                        text2 = "        ";
                    }
                    text = text2 + text;
                    List3.Items.Add(text);
                }
                DataModul.DB_WitnessTable.MoveNext();
                num = (short)unchecked(num + 1);
            }
            while (num <= 99);
            short num2 = (short)(List3.Items.Count - 1);
            for (num = 0; num <= num2; num = (short)unchecked(num + 1))
            {
                Per1 += Strings.Mid(List3.Items[num].AsString(), 9, 16);
            }
            short num3 = (short)Math.Round(Per1.Length / 16.0);
            for (short num4 = 1; num4 <= num3; num4 = (short)unchecked(num4 + 1))
            {
                text = Per1.Left(16);
                Per1 = Strings.Mid(Per1, 17, Per1.Length);
                string text3;
                if (text.Left(4).AsDouble() > 499.0)
                {
                    _Modul1.Instance.FamInArb = (int)Math.Round(Strings.Mid(text, 7, 10).AsDouble());
                    _Modul1.Instance.Famles();
                    string str = "";
                    if (_Modul1.Instance.Family.Mann > 0)
                    {
                        _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                        _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                        _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                        str = _Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[0];
                    }
                    string str2 = "";
                    if (_Modul1.Instance.Family.Frau > 0)
                    {
                        _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau;
                        _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                        _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                        str2 = _Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[0];
                    }
                    text3 = (!((str.Trim() != "") & (str2.Trim() != ""))) ? (str.Trim() + " " + str2.Trim()).Trim() : (str.Trim() + " und " + str2.Trim());
                    DataModul.DB_EventTable.Index = "ArtNr";
                    DataModul.DB_EventTable.Seek("=", text.Left(4), _Modul1.Instance.FamInArb, Strings.Mid(text, 5, 2).AsDouble());
                }
                else
                {
                    _Modul1.Instance.PersInArb = (int)Math.Round(Strings.Mid(text, 7, 10).AsDouble());
                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                    _Modul1.Instance.Person_FullSurname(_Modul1.Instance.Kont, false);
                    text3 = (_Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[0]).Trim();
                    DataModul.DB_EventTable.Index = "ArtNr";
                    DataModul.DB_EventTable.Seek("=", text.Left(4), _Modul1.Instance.PersInArb, Strings.Mid(text, 5, 2).AsDouble());
                }
                string text4 = "";
                if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                {
                    Datu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                    Ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                    Datwand1(ref Datu, ref Ds);
                    text4 = Datu + " ";
                }
                string text5 = "";
                string text6;
                switch (text.Left(4))
                {
                    case " 101":
                        text6 = " (" + text4 + "bei der Geburt)";
                        text5 = "Zeuge " + text4 + "bei der Geburt von ";
                        break;
                    case " 102":
                        text6 = " (" + text4 + "bei der Taufe)";
                        text5 = "Zeuge " + text4 + "bei der Taufe von ";
                        break;
                    case " 103":
                        text6 = " (" + text4 + "beim Tod)";
                        text5 = "Zeuge " + text4 + "beim Tod von ";
                        break;
                    case " 104":
                        text6 = " (" + text4 + "beim Begräbnis)";
                        text5 = "Zeuge " + text4 + "beim Begräbnis von ";
                        break;
                    case " 105":
                        {
                            int AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                            string LD;
                            DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                            if (_Modul1.Instance.Kont[0] != "")
                            {
                                text6 = " (" + text4 + "bei " + DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString().Trim() + ")";
                                text5 = "Zeuge " + text4 + "bei " + DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString().Trim() + "von ";
                            }
                            else
                            {
                                text6 = " (" + text4 + "bei der Sonst.Datum)";
                                text5 = "Zeuge " + text4 + "beim Sonst. Datum von ";
                            }
                            break;
                        }
                    case " 106":
                        text6 = " (" + text4 + "beim Heimatort)";
                        text5 = "Zeuge " + text4 + "beim Heimatort von ";
                        break;
                    case " 300":
                        text6 = " (" + text4 + "beim Beruf)";
                        text5 = "Zeuge " + text4 + "beim Beruf von ";
                        break;
                    case " 301":
                        text6 = " (" + text4 + "beim Titel)";
                        text5 = "Zeuge " + text4 + "beim Titel von ";
                        break;
                    case " 302":
                        text6 = " (" + text4 + "beim Wohnort)";
                        text5 = "Zeuge " + text4 + "beim Wohnort von ";
                        break;
                    case " 500":
                        text6 = " (" + text4 + "bei der Proklamation)";
                        text5 = "Zeuge " + text4 + "bei bei der Proklamation von ";
                        break;
                    case " 501":
                        text6 = " (" + text4 + "bei der Verlobung)";
                        text5 = "Zeuge " + text4 + "bei der Verlobung von ";
                        break;
                    case " 502":
                        text6 = " (" + text4 + "bei der Heirat)";
                        text5 = "Trauzeuge " + text4 + "bei der Heirat von ";
                        break;
                    case " 503":
                        text6 = " (" + text4 + "bei der Kirchl. Heir.)";
                        text5 = "Trauzeuge " + text4 + "bei der Kirchl. Heir. von ";
                        break;
                    case " 504":
                        text6 = " (" + text4 + "bei der Scheidung)";
                        text5 = "Zeuge " + text4 + "bei der Scheidung von ";
                        break;
                    case " 505":
                        text6 = " (" + text4 + "bei der Eheänl. Beziehung)";
                        text5 = "Zeuge " + text4 + "bei der Eheänl. Beziehung von ";
                        break;
                    case " 506":
                        text6 = " (" + text4 + "Eheänl. Beziehung)";
                        break;
                    case " 507":
                        text6 = " (" + text4 + "Dimissoriale)";
                        text5 = "Zeuge " + text4 + "bei der Dimissoriale von ";
                        break;
                    case " 602":
                        text6 = " (" + text4 + "bei der Wohnung)";
                        text5 = "Zeuge " + text4 + "beim Wohnungseintag von ";
                        break;
                    case " 603":
                        {
                            int AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                            string LD;
                            DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                            if (_Modul1.Instance.Kont[0] != "")
                            {
                                text6 = " (" + text4 + "bei " + DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString().Trim() + ")";
                                text5 = "Zeuge " + text4 + "bei " + DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString().Trim() + "von ";
                            }
                            else
                            {
                                text6 = " (" + text4 + "bei der Sonst.Datum)";
                                text5 = "Zeuge " + text4 + "beim Sonst. Datum von ";
                            }
                            break;
                        }
                }
                Anz[0].SelectedText = "\n" + text5 + text3;
            }
        }
    }

    public void Datles2()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int number = default;
        int lErl = default;
        float num5 = default;
        float num6 = default;
        float num7 = default;
        short num8 = default;
        float num9 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    int Ortnr;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 1861:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_05fb;
                                    default:
                                        goto end_IL_0000;
                                }
                                goto IL_057c;
                            }
                        IL_057c:
                            num = 64;
                            number = Information.Err().Number;
                            goto IL_058c;
                        IL_058c:
                            num = 67;
                            if (number == 94)
                            {
                                goto IL_0596;
                            }
                            goto IL_05b2;
                        IL_05b2:
                            num = 72;
                            if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            goto IL_05d8;
                        IL_054b:
                            num = 60;
                            _Modul1.Instance.UbgT = "";
                            goto IL_0559;
                        IL_05d8:
                            num = 75;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_05ff;
                        IL_0559:
                            num = 61;
                            lErl = 2;
                            goto IL_0560;
                        IL_0560:
                            num = 62;
                            num5 += 1f;
                            if (!(num5 <= 104f))
                            {
                                goto end_IL_0000_2;
                            }
                            goto IL_005f;
                        IL_05ff:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_0008;
                                case 3:
                                    goto IL_0015;
                                case 4:
                                    goto IL_0022;
                                case 5:
                                    goto IL_002b;
                                case 6:
                                    goto IL_0034;
                                case 7:
                                    goto IL_0039;
                                case 8:
                                    goto IL_0048;
                                case 9:
                                    goto IL_0055;
                                case 10:
                                    goto IL_005f;
                                case 11:
                                    goto IL_006a;
                                case 12:
                                    goto IL_0082;
                                case 13:
                                    goto IL_0099;
                                case 14:
                                    goto IL_00aa;
                                case 15:
                                    goto IL_00b9;
                                case 16:
                                    goto IL_00cc;
                                case 17:
                                    goto IL_0134;
                                case 19:
                                case 20:
                                    goto IL_0149;
                                case 21:
                                    goto IL_017e;
                                case 22:
                                    goto IL_01a6;
                                case 23:
                                    goto IL_01ce;
                                case 24:
                                    goto IL_01e4;
                                case 25:
                                    goto IL_01f5;
                                case 26:
                                    goto IL_0202;
                                case 27:
                                    goto IL_0222;
                                case 28:
                                case 29:
                                    goto IL_022c;
                                case 30:
                                    goto IL_0244;
                                case 31:
                                case 32:
                                    goto IL_0264;
                                case 33:
                                    goto IL_0271;
                                case 34:
                                    goto IL_0291;
                                case 35:
                                case 36:
                                    goto IL_029b;
                                case 37:
                                    goto IL_02b3;
                                case 38:
                                case 39:
                                case 40:
                                    goto IL_02d3;
                                case 41:
                                    goto IL_0301;
                                case 42:
                                    goto IL_0329;
                                case 43:
                                    goto IL_0338;
                                case 44:
                                    goto IL_0360;
                                case 45:
                                    goto IL_0376;
                                case 46:
                                    goto IL_03ab;
                                case 48:
                                    goto IL_03c8;
                                case 49:
                                    goto IL_03cc;
                                case 47:
                                case 50:
                                case 51:
                                case 52:
                                    goto IL_03e7;
                                case 53:
                                    goto IL_03f5;
                                case 54:
                                    goto IL_042a;
                                case 55:
                                    goto IL_0461;
                                case 56:
                                    goto IL_0496;
                                case 57:
                                case 58:
                                case 59:
                                    goto IL_04d6;
                                case 60:
                                    goto IL_054b;
                                case 18:
                                case 61:
                                    goto IL_0559;
                                case 62:
                                    goto IL_0560;
                                case 64:
                                    goto IL_057c;
                                case 66:
                                case 67:
                                    goto IL_058c;
                                case 68:
                                    goto IL_0596;
                                case 71:
                                case 72:
                                    goto IL_05b2;
                                case 73:
                                case 75:
                                    goto IL_05d8;
                                default:
                                    goto end_IL_0000;
                                case 63:
                                case 65:
                                case 69:
                                case 70:
                                case 76:
                                case 77:
                                case 78:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                        IL_0596:
                            num = 68;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_05fb;
                        IL_04d6:
                            num = 59;
                            _Modul1.Instance.Kont[_Modul1.Instance.Ubg - 90] = _Modul1.Instance.Kont1[1] + _Modul1.Instance.Kont1[2] + _Modul1.Instance.Kont1[3] + _Modul1.Instance.Kont1[4] + _Modul1.Instance.Kont1[5] + _Modul1.Instance.Kont1[6] + " " + _Modul1.Instance.UbgT;
                            goto IL_054b;
                        IL_05fb:
                            num4 = unchecked(num2 + 1);
                            goto IL_05ff;
                        IL_0008:
                            num = 2;
                            _Modul1.Instance.Datum1 = "";
                            goto IL_0015;
                        IL_0015:
                            num = 3;
                            _Modul1.Instance.Datum2 = "";
                            goto IL_0022;
                        IL_0022:
                            num = 4;
                            num6 = 0f;
                            goto IL_002b;
                        IL_002b:
                            num = 5;
                            num7 = 0f;
                            goto IL_0034;
                        IL_0034:
                            num = 6;
                            num8 = 1;
                            goto IL_0039;
                        IL_0039:
                            num = 7;
                            _Modul1.Instance.Kont[num8] = "";
                            goto IL_0048;
                        IL_0048:
                            num = 8;
                            num8 = (short)unchecked(num8 + 1);
                            if (num8 <= 50)
                            {
                                goto IL_0039;
                            }
                            goto IL_0055;
                        IL_0055:
                            num = 9;
                            num5 = 101f;
                            goto IL_005f;
                        IL_005f:
                            num = 10;
                            num9 = 0f;
                            goto IL_006a;
                        IL_006a:
                            num = 11;
                            _Modul1.Instance.Kont1[(int)Math.Round(num9)] = "";
                            goto IL_0082;
                        IL_0082:
                            num = 12;
                            num9 += 1f;
                            if (num9 <= 20f)
                            {
                                goto IL_006a;
                            }
                            goto IL_0099;
                        IL_0099:
                            num = 13;
                            _Modul1.Instance.Ubg = (int)Math.Round(num5);
                            goto IL_00aa;
                        IL_00aa:
                            num = 14;
                            _Modul1.Instance.Art = (EEventArt)_Modul1.Instance.Ubg;
                            goto IL_00b9;
                        IL_00b9:
                            num = 15;
                            DataModul.DB_EventTable.Index = "ArtNr";
                            goto IL_00cc;
                        IL_00cc:
                            num = 16;
                            DataModul.DB_EventTable.Seek("=", _Modul1.Instance.Ubg.AsString(), _Modul1.Instance.PersInArb.AsString(), "0");
                            goto IL_0134;
                        IL_0134:
                            num = 17;
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                goto IL_0149;
                            }
                            goto IL_0559;
                        IL_0149:
                            num = 20;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                            {
                                goto IL_017e;
                            }
                            goto IL_02d3;
                        IL_017e:
                            num = 21;
                            Datu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                            goto IL_01a6;
                        IL_01a6:
                            num = 22;
                            Ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                            goto IL_01ce;
                        IL_01ce:
                            num = 23;
                            Datwand1(ref Datu, ref Ds);
                            goto IL_01e4;
                        IL_01e4:
                            num = 24;
                            _Modul1.Instance.Kont1[1] = Datu;
                            goto IL_01f5;
                        IL_01f5:
                            num = 25;
                            if (_Modul1.Instance.Ubg == 101)
                            {
                                goto IL_0202;
                            }
                            goto IL_022c;
                        IL_0202:
                            num = 26;
                            _Modul1.Instance.Datum2 = "           " + Datu.Right(10);
                            goto IL_0222;
                        IL_0222:
                            num = 27;
                            num6 = 1f;
                            goto IL_022c;
                        IL_022c:
                            num = 29;
                            if (unchecked(_Modul1.Instance.Ubg == 102 && num6 == 0f))
                            {
                                goto IL_0244;
                            }
                            goto IL_0264;
                        IL_0244:
                            num = 30;
                            _Modul1.Instance.Datum2 = "           " + Datu.Right(10);
                            goto IL_0264;
                        IL_0264:
                            num = 32;
                            if (_Modul1.Instance.Ubg == 103)
                            {
                                goto IL_0271;
                            }
                            goto IL_029b;
                        IL_0271:
                            num = 33;
                            _Modul1.Instance.Datum1 = "           " + Datu.Right(10);
                            goto IL_0291;
                        IL_0291:
                            num = 34;
                            num7 = 1f;
                            goto IL_029b;
                        IL_029b:
                            num = 36;
                            if (unchecked(_Modul1.Instance.Ubg == 104 && num7 == 0f))
                            {
                                goto IL_02b3;
                            }
                            goto IL_02d3;
                        IL_02b3:
                            num = 37;
                            _Modul1.Instance.Datum1 = "           " + Datu.Right(10);
                            goto IL_02d3;
                        IL_02d3:
                            num = 40;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                goto IL_0301;
                            }
                            goto IL_03e7;
                        IL_0301:
                            num = 41;
                            Datu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                            goto IL_0329;
                        IL_0329:
                            num = 42;
                            Ds = "";
                            goto IL_0338;
                        IL_0338:
                            num = 43;
                            Ds = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                            goto IL_0360;
                        IL_0360:
                            num = 44;
                            Datwand1(ref Datu, ref Ds);
                            goto IL_0376;
                        IL_0376:
                            num = 45;
                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString().Trim(), "", TextCompare: false) == 0)
                            {
                                goto IL_03ab;
                            }
                            goto IL_03c8;
                        IL_03ab:
                            num = 46;
                            _Modul1.Instance.Kont1[3] = " / " + Datu;
                            goto IL_03e7;
                        IL_03c8:
                            num = 48;
                            goto IL_03cc;
                        IL_03cc:
                            num = 49;
                            _Modul1.Instance.Kont1[3] = " " + Datu;
                            goto IL_03e7;
                        IL_03e7:
                            num = 52;
                            _Modul1.Instance.UbgT = "";
                            goto IL_03f5;
                        IL_03f5:
                            num = 53;
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                            {
                                goto IL_042a;
                            }
                            goto IL_04d6;
                        IL_042a:
                            num = 54;
                            Ortnr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble());
                            ortles2(ref Ortnr);
                            goto IL_0461;
                        IL_0461:
                            num = 55;
                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString().Trim(), "", TextCompare: false) != 0)
                            {
                                goto IL_0496;
                            }
                            goto IL_04d6;
                        IL_0496:
                            num = 56;
                            _Modul1.Instance.UbgT = _Modul1.Instance.UbgT.TrimEnd() + " " + DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString().Trim();
                            goto IL_04d6;
                        end_IL_0000:
                            break;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 1861;
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

    public void ortles2(ref int Ortnr)
    {
        int try0000_dispatch = -1;
        int num2 = default;
        int num = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                switch (try0000_dispatch)
                {
                    default:
                        {
                            float num3 = 1f;
                            do
                            {
                                Kont2[checked((int)Math.Round(num3))] = "";
                                num3 += 1f;
                            }
                            while (num3 <= 10f);
                            ProjectData.ClearProjectError();
                            num2 = 0;
                            DataModul.DB_PlaceTable.Index = "OrtNr";
                            DataModul.DB_PlaceTable.Seek("=", Ortnr);
                            if (!DataModul.DB_PlaceTable.NoMatch)
                            {
                                int AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt();
                                string LD = "";
                                DataModul.Textlese(AAA, out Kont2[1], out LD);
                                AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt();
                                LD = "";
                                DataModul.Textlese(AAA, out Kont2[2], out LD);
                                if (Kont2[2] != "")
                                {
                                    Kont2[2] = "-" + Kont2[2];
                                }
                                if (!Information.IsDBNull(DataModul.DB_PlaceTable.Fields[PlaceFields.Zusatz].Value))
                                {
                                    Kont2[3] = DataModul.DB_PlaceTable.Fields[PlaceFields.Zusatz].AsString();
                                }
                                if (Kont2[3].Trim() == "")
                                {
                                    Kont2[3] = "in";
                                }
                                _Modul1.Instance.UbgT = Kont2[3].Trim() + " " + Kont2[1].TrimEnd() + Kont2[2].TrimEnd();
                            }
                            else
                            {
                                _Modul1.Instance.UbgT = "";
                            }
                            goto end_IL_0000;
                        }
                    case 530:
                        num = -1;
                        break;
                }
                goto IL_0246;
            end_IL_0000:;
            }
            catch (Exception obj) when (obj is not null && num2 != 0 && num == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 530;
                continue;
            }
            break;
        IL_0246:
            throw ProjectData.CreateProjectError(-2146828237);
        }
        if (num != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    public void Datl()
    {
        Datum3 = "";
        Datum4 = "";
        short num = 101;
        checked
        {
            do
            {
                _Modul1.Instance.Ubg = num;
                DataModul.DB_EventTable.Index = "ArtNr";
                DataModul.DB_EventTable.Seek("=", _Modul1.Instance.Ubg.AsString(), _Modul1.Instance.PersInArb.AsString(), "0");
                if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                {
                    Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                    if (_Modul1.Instance.Ubg == 101)
                    {
                        Datum4 = Datu.Left(4);
                    }
                    if ((_Modul1.Instance.Ubg == 102) & (Datum4 == ""))
                    {
                        Datum4 = Datu.Left(4);
                    }
                    if (_Modul1.Instance.Ubg == 103)
                    {
                        Datum3 = Datu.Left(4);
                    }
                    if ((_Modul1.Instance.Ubg == 104) & (Datum3 == ""))
                    {
                        Datum3 = Datu.Left(4);
                    }
                }
                num = (short)unchecked(num + 1);
            }
            while (num <= 104);
        }
    }

    public void Retweg3()
    {
        //Discarded unreachable code: IL_014e
        int try0000_dispatch = -1;
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
                int num4;
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0007;
                    case 433:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0151;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_0131;
                        }
                    IL_0131:
                        num = 19;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_0151;
                    IL_010e:
                        num = 14;
                        Anz[0].SelectedText = "";
                        goto IL_0007;
                    IL_0151:
                        num4 = num2 + 1;
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                            case 9:
                            case 15:
                                goto IL_0007;
                            case 3:
                                goto IL_000b;
                            case 4:
                                goto IL_0013;
                            case 5:
                                goto IL_002c;
                            case 6:
                                goto IL_0053;
                            case 7:
                                goto IL_0079;
                            case 8:
                                goto IL_008d;
                            case 10:
                            case 11:
                                goto IL_00aa;
                            case 12:
                                goto IL_00d2;
                            case 13:
                                goto IL_00f9;
                            case 14:
                                goto IL_010e;
                            case 19:
                                goto IL_0131;
                            default:
                                goto end_IL_0000;
                            case 16:
                            case 17:
                            case 18:
                            case 20:
                                goto end_IL_0000_2;
                        }
                        goto default;
                    IL_0007:
                        num = 2;
                        lErl = 1;
                        goto IL_000b;
                    IL_000b:
                        num = 3;
                        leerweg();
                        goto IL_0013;
                    IL_0013:
                        num = 4;
                        if (Anz[0].SelectionStart <= 1)
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_002c;
                    IL_002c:
                        num = 5;
                        if (Anz[0].Text.Right(1) == "\n")
                        {
                            goto IL_0053;
                        }
                        goto IL_00aa;
                    IL_0053:
                        num = 6;
                        Anz[0].SelectionStart = checked(Anz[0].SelectionStart - 2);
                        goto IL_0079;
                    IL_0079:
                        num = 7;
                        Anz[0].SelectionLength = 2;
                        goto IL_008d;
                    IL_008d:
                        num = 8;
                        Anz[0].SelectedText = "";
                        goto IL_0007;
                    IL_00aa:
                        num = 11;
                        if (Anz[0].Text.Right(1) != "\r")
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_00d2;
                    IL_00d2:
                        num = 12;
                        Anz[0].SelectionStart = checked(Anz[0].SelectionStart - 2);
                        goto IL_00f9;
                    IL_00f9:
                        num = 13;
                        Anz[0].SelectionLength = 2;
                        goto IL_010e;
                    end_IL_0000:
                        break;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 433;
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
    }

    public void Listleer()
    {
        Command1[1].Visible = false;
        Command1[2].Visible = false;
        byte b = 0;
        do
        {
            Label5[b].Text = "";
            checked
            {
                b = (byte)unchecked((uint)(b + 1));
            }
        }
        while (b <= 15u);
        List1.Items.Clear();
    }

    private void ComboBox1_KeyUp(object sender, KeyEventArgs e)
    {
        if ((EingCode = checked((short)e.KeyCode)) == 13)
        {
            Command1[3].PerformClick();
        }
    }

    private void Check2_MouseUp(object sender, MouseEventArgs e)
    {
        int index = Check2.GetIndex((CheckBox)sender);
        if (index != 3)
        {
            Listleer();
        }
    }

    private void Check3_MouseUp(object sender, MouseEventArgs e)
    {
        Listleer();
        if (ComboBox1.Text != "")
        {
            Command1[3].PerformClick();
        }
    }

    private void Button1_Click(object sender, EventArgs e)
    {
        List1.BringToFront();
        Interaction.MsgBox(List1.Enabled);
        List1.Focus();
        Interaction.MsgBox(List1.Focused);
        Interaction.MsgBox(List1.Visible);
        List1.Visible = false;
        List1.Visible = true;
    }

    private void Button2_Click(object sender, EventArgs e)
    {
        Close();
    }

    public void Suchspeich()
    {
    }

    private void RadioButton1_CheckedChanged(object sender, EventArgs e)
    {
        if (RadioButton1.Checked | RadioButton2.Checked)
        {
            byte b = 0;
            do
            {
                Label5[b].Text = "";
                Label7[b].Text = "";
                checked
                {
                    b = (byte)unchecked((uint)(b + 1));
                }
            }
            while (b <= 15u);
            b = 0;
            do
            {
                Check2[b].Enabled = true;
                checked
                {
                    b = (byte)unchecked((uint)(b + 1));
                }
            }
            while (b <= 2u);
            List1.Items.Clear();
            Check2[4].Visible = true;
            Check2[5].Visible = true;
            Check3.Enabled = true;
            Check3.Visible = true;
            Check2[2].Visible = false;
            Label4.Visible = false;
            Label10.Visible = false;
            Text2.Visible = false;
            Label3.Text = "Name,Vorname                        Bearb.Datum Personennummer                       Partner";
            Label4.Visible = true;
            Check3.Visible = false;
            ComboBox1.Focus();
            ComboBox1.Text = "";
            if (RadioButton2.Checked)
            {
                Check2[4].Visible = false;
                Check2[5].Visible = false;
            }
        }
    }

    private void Zeigfamanl(int dBearb)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        string HT = default;
        string DDatum = default;
        int Fam = default;
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
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            Listleer();
                            goto IL_0009;
                        case 2291:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0731;
                                    default:
                                        goto end_IL_0000;
                                }
                                int number = Information.Err().Number;
                                if (number == 3021)
                                {
                                    List1.Items.Add("------------ Ende der Liste-----------");
                                    goto end_IL_0000_2;
                                }
                                if (number == 94)
                                {
                                    DataModul.DSB_SearchTable.Edit();
                                    DataModul.DSB_SearchTable.Fields["Kenn"].Value = "9";
                                    DataModul.DSB_SearchTable.Update();
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_072d;
                                }
                                if (number == 3421)
                                {
                                    Option1[1].Checked = true;
                                    goto end_IL_0000_2;
                                }
                                if (number == 3167)
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
                                goto IL_072d;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            short num5 = 0;
                            DataModul.DB_PersonTable.Index = "BeaDat";
                            int num6 = dBearb;
                            DataModul.DB_PersonTable.Seek(">=", num6);
                            HT = "          ";
                            while (!DataModul.DB_PersonTable.EOF
                                && num5 != _Modul1.Instance.Aus[13].AsDouble())
                            {
                                DDatum = DataModul.DB_PersonTable.Fields[PersonFields.EditDat].AsString();
                                Datwand(ref DDatum, ref HT);
                                if (HT.Trim() == "")
                                {
                                    HT = "          ";
                                }
                                _Modul1.Instance.PersInArb = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                                PersSp = _Modul1.Instance.PersInArb;
                                Perles1();
                                if (_Modul1.Instance.Kont[1].Trim() != "")
                                {
                                    _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0];
                                }
                                string text = _Modul1.Instance.Kont[0].TrimEnd() + "," + _Modul1.Instance.Kont[3];
                                string text2 = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString();
                                if ((Check2[4].CheckState != CheckState.Checked || text2 != "F")
                                    && (Check2[5].CheckState != CheckState.Checked || text2 != "M"))
                                {
                                    IList<int>? lFams = null;
                                    if (Check2[0].Checked && text2 == "M"
                                                                    || Check2[1].Checked && text2 == "F"
                                                                    || (Check2[0].CheckState == CheckState.Unchecked) & (Check2[1].CheckState == CheckState.Unchecked))
                                    {
                                        Persex = text2;
                                        lFams = Ehesuch(_Modul1.Instance.PersInArb, Persex);
                                    }
                                    Kenn = text2 == "F" ? ELinkKennz.lkFather : ELinkKennz.lkMother;
                                    _M_LiText = "";
                                    float num9;
                                    if (lFams != null)
                                    {
                                        foreach (var iFam in lFams)
                                        {
                                            Famzeig(iFam, ref _M_LiText, Kenn);
                                            num9 = Strings.InStr(text, ",");
                                            if (num9 > 25f)
                                            {
                                                text = text.Left(25) + Strings.Mid(text, (int)Math.Round(num9), text.Length);
                                            }
                                            List1.Items.Add(text + new string(' ', 40).Left(37) + HT + "              " + PersSp.AsString().Right(10) + _M_LiText + Fam.AsString());
                                            if (List1.Items.Count >= _Modul1.Instance.Aus[13].AsDouble())
                                            {
                                                goto end_IL_0000_2;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        num9 = Strings.InStr(text, ",");
                                        if (num9 > 25f)
                                        {
                                            text = text.Left(25) + Strings.Mid(text, (int)Math.Round(num9), text.Length);
                                        }
                                        if (Check2[2].CheckState == CheckState.Unchecked)
                                        {
                                            List1.Items.Add(text + new string(' ', 40).Left(37) + HT + Strings.Right("           " + PersSp.AsString().Trim(), 10) + _M_LiText);
                                            S += 1f;
                                        }
                                        _M_LiText = "";
                                    }
                                }
                                lErl = 2;
                                DataModul.DB_PersonTable.MoveNext();
                            }
                            goto end_IL_0000_2;
                        IL_072d:
                            num4 = num2;
                            goto IL_0735;
                        IL_0731:
                            num4 = unchecked(num2 + 1);
                            goto IL_0735;
                        IL_0735:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 62:
                                case 82:
                                case 84:
                                case 88:
                                case 94:
                                case 95:
                                case 98:
                                case 100:
                                case 106:
                                case 107:
                                case 108:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 2291;
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
    private void Zeigfamanl2()
    {
        int try0000_dispatch = -1;
        int num = default;
        string HT = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        float num5 = default;
        string LiText = default;
        string DDatum = default;
        float num8 = default;
        string text = default;
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
                        HT = "";
                        goto IL_000a;
                    case 2420:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_07be;
                                default:
                                    goto end_IL_0000;
                            }
                            int number = Information.Err().Number;
                            if (number == 3021)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_07be;
                            }
                            if (number == 94)
                            {
                                DataModul.DSB_SearchTable.Edit();
                                DataModul.DSB_SearchTable.Fields["Kenn"].Value = "9";
                                DataModul.DSB_SearchTable.Update();
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_07c2;
                            }
                            if (number == 3421)
                            {
                                Option1[1].Checked = true;
                                goto end_IL_0000_2;
                            }
                            if (number != 3167)
                            {
                                goto end_IL_0000_2;
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num2 = 0;
                            goto IL_0679;
                        }
                    end_IL_0000:
                        break;
                    IL_000a:
                        num = 2;
                        Listleer();
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        int num6 = checked((int)Math.Round((Text1.Text + "0000".Left(8)).AsDouble()));
                        DataModul.DB_FamilyTable.Index = "Beadat";
                        DataModul.DB_FamilyTable.Seek(">=", num6);
                        LiText = "          ";
                        goto IL_06a6;
                    IL_0274: // <========== 3
                        num = 36;
                        if (num8 == 1f)
                        {
                            if (_Modul1.Instance.Family.Mann == 0)
                            {
                                _M_LiText = " " + new string(' ', 40).Left(40) + "       " + "              ".Right(10) + " ";
                                goto IL_0551;
                            }
                            _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                            Perles1();
                            if (_Modul1.Instance.Kont[1].Trim() != "")
                            {
                                _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0];
                            }
                            if (_Modul1.Instance.Kont[0].Length > 25)
                            {
                                _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].Left(25);
                            }
                            text = _Modul1.Instance.Kont[0].TrimEnd() + "," + _Modul1.Instance.Kont[3];
                            DataModul.DSB_SearchTable.Index = "Nummer";
                            DataModul.DSB_SearchTable.Seek("=", _Modul1.Instance.PersInArb);
                            string text2 = (" " + DataModul.DSB_SearchTable.Fields["Kenn"].AsString());
                            DDatum = text2 + Strings.Right("      " + DataModul.DSB_SearchTable.Fields["Datum"].AsString(), 4);
                            if (!Information.IsDBNull(DataModul.DSB_SearchTable.Fields["Sich"].Value))
                            {
                                DDatum = (DDatum + DataModul.DSB_SearchTable.Fields["Sich"].AsString());
                            }
                            DDatum = DDatum + "   ".Left(7);
                            if (DDatum.Right(4).AsDouble() == 0.0)
                            {
                                DDatum = "       ";
                            }
                            goto IL_04e6;
                        }
                        goto IL_0698;
                    IL_04e6:
                        num = 61;
                        _M_LiText = text + new string(' ', 40).Left(37) + HT + "              " + _Modul1.Instance.PersInArb.AsString().Right(10) + " " + LiText;
                        goto IL_0551;
                    IL_0551: // <========== 3
                        num = 62;
                        lErl = 4;
                        _M_LiText += " mit ";
                        _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau;
                        Perles1();
                        if (_Modul1.Instance.Kont[1].Trim() != "")
                        {
                            _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0];
                        }
                        text = _Modul1.Instance.Kont[0].TrimEnd() + "," + _Modul1.Instance.Kont[3];
                        _M_LiText += text;
                        _M_LiText = _M_LiText + new string(' ', 135).Left(135);
                        List1.Items.Add(_M_LiText + _Modul1.Instance.FamInArb.AsString());
                        num5 += 1f;
                        _M_LiText = "";
                        goto IL_0679;
                    IL_0679: // <========== 3
                        num = 75;
                        lErl = 2;
                        if ((double)num5 >= _Modul1.Instance.Aus[13].AsDouble())
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_0698;
                    IL_0698: // <========== 4
                        num = 81;
                        DataModul.DB_FamilyTable.MoveNext();
                        goto IL_06a6;
                    IL_06a6: // <========== 3
                        num = 10;
                        if (DataModul.DB_FamilyTable.EOF)
                        {
                            goto end_IL_0000_2;
                        }
                        if (!DataModul.DB_FamilyTable.NoMatch)
                        {
                            _Modul1.Instance.FamInArb = DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();
                            DDatum = DataModul.DB_FamilyTable.Fields[FamilyFields.EditDat].AsString();
                            Datwand(ref DDatum, ref HT);
                            if (HT.Trim() == "")
                            {
                                HT = "          ";
                            }
                            FamzeigDat(ref _Modul1.Instance.FamInArb, ref LiText, Kenn);
                            _Modul1.Instance.Famles();
                            num8 = 0f;
                            if (Text2.Text != "")
                            {
                                _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau;
                                Perles1();
                                if (Operators.CompareString(Strings.Left(_Modul1.Instance.Kont[0].ToUpper().Trim(), Text2.Text.Length), Text2.Text.ToUpper(), TextCompare: false) == 0)
                                {
                                    num8 = 1f;
                                }
                                _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                                Perles1();
                                if (Operators.CompareString(Strings.Left(_Modul1.Instance.Kont[0].ToUpper().Trim(), Text2.Text.Length), Text2.Text.ToUpper(), TextCompare: false) == 0)
                                {
                                    num8 = 1f;
                                }

                            }
                            else
                            {
                                num8 = 1f;
                            }
                            goto IL_0274;
                        }
                        goto IL_0698;
                    IL_07be:
                        num4 = num2 + 1;
                        goto IL_07c2;
                    IL_07c2:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 31:
                            case 32:
                            case 35:
                            case 36:
                                goto IL_0274;
                            case 60:
                            case 61:
                                goto IL_04e6;
                            case 39:
                            case 62:
                                goto IL_0551;
                            case 75:
                            case 103:
                                goto IL_0679;
                            case 78:
                            case 79:
                            case 80:
                            case 81:
                                goto IL_0698;
                            case 9:
                            case 10:
                            case 82:
                                goto IL_06a6;
                            case 77:
                            case 83:
                            case 85:
                            case 89:
                            case 90:
                            case 96:
                            case 97:
                            case 100:
                            case 104:
                            case 105:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 2420;
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
    public void FamzeigDat(ref int Fam, ref string LiText, ELinkKennz Kenn)
    {
        DataModul.DB_EventTable.Index = "BeSu";
        EEventArt num = EEventArt.eA_Marriage;
        EEventArt num2;
        do
        {
            num2 = num;
            DataModul.DB_EventTable.Seek("=", num, Fam.AsString());
            if (DataModul.DB_EventTable.NoMatch)
            {
                Datu = "      ";
            }
            else
            {
                Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                Datu = Datu.Left(4) + Strings.Right((" " + DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString()), 1);
                if (Datu.AsDouble() == 0.0)
                {
                    Datu = "      ";
                }
                if (Datu.Trim() != "")
                {
                    break;
                }
            }
            num++;
        }
        while (num <= EEventArt.eA_507);
        if (Datu.Trim() == "")
        {
            num = EEventArt.eA_500;
            do
            {
                DataModul.DB_EventTable.Seek("=", num, Fam.AsString());
                num2 = num;
                if (DataModul.DB_EventTable.NoMatch)
                {
                    Datu = "      ";
                }
                else
                {
                    Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                    Datu = Datu.Left(4) + Strings.Right((" " + DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString()), 1);
                    if (Datu.AsDouble() == 0.0)
                    {
                        Datu = "      ";
                    }
                    if (Datu.Trim() != "")
                    {
                        break;
                    }
                }
                num++;
            }
            while (num <= EEventArt.eA_501);
        }
        if (Datu.Trim() == "")
        {
            DataModul.DB_EventTable.Seek("=", 601, Fam.AsString());
            if (DataModul.DB_EventTable.NoMatch)
            {
                Datu = "      ";
            }
            else
            {
                Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                Datu = Datu.Left(4) + Strings.Right((" " + DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString()), 1);
                num2 = EEventArt.eA_601;
                if (Datu.AsDouble() == 0.0)
                {
                    Datu = "      ";
                }
            }
        }
        string text = "";
        if (num2 == EEventArt.eA_500)
        {
            text = "Prok.";
        }
        else if (num2 == EEventArt.eA_501)
        {
            text = "Verl.";
        }
        else if (num2 == EEventArt.eA_Marriage)
        {
            text = "Heir.";
        }
        else if (num2 == EEventArt.eA_MarrReligious)
        {
            text = "kirH.";
        }
        else if (num2 == EEventArt.eA_505)
        {
            text = "Eheä.";
        }
        else if (num2 == EEventArt.eA_504)
        {
            text = "Scheid.";
        }
        else if (num2 == EEventArt.eA_507)
        {
            text = "Dim.";
        }
        else if (num2 == EEventArt.eA_601)
        {
            text = "FiHr.";
        }
        if (Datu.Trim() == "")
        {
            text = "    ";
        }
        LiText = new string(' ', 12);
        LiText = "              " + text + Datu.Right(12);
    }

    public void Datwand(ref string DDatum, ref string HT)
    {
        HT = "          ";
        if (DDatum.TrimStart().Length == 4)
        {
            DDatum = DDatum.TrimStart() + "00000000".Left(8);
        }
        if (DDatum.Trim().Length < 8)
        {
            DDatum = "0000000" + DDatum.Trim().Right(8);
        }
        if (DDatum.AsDouble() > 0.0)
        {
            StringType.MidStmtStr(ref HT, 1, 2, Strings.Mid(DDatum, 7, 2));
            if (HT.Left(2).AsDouble() == 0.0)
            {
                StringType.MidStmtStr(ref HT, 1, 2, "00");
            }
            StringType.MidStmtStr(ref HT, 4, 2, Strings.Mid(DDatum, 5, 2));
            if (Strings.Mid(HT, 4, 2).AsDouble() == 0.0)
            {
                StringType.MidStmtStr(ref HT, 4, 2, "00");
            }
            StringType.MidStmtStr(ref HT, 7, 4, DDatum.Left(4));
            StringType.MidStmtStr(ref HT, 3, 1, ".");
            StringType.MidStmtStr(ref HT, 6, 1, ".");
        }
        HT = HT.TrimEnd();
    }

    private void _Command1_5_Click(object sender, EventArgs e)
    {
    }
}
