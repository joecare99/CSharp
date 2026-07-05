using BaseLib.Helper;
using Druck.My;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
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

namespace Druck;

public partial class AhnenST : Form
{
    private string Ahne;

    private long IZahl;

    private string T;

    [SpecialName]
    private string _DaT;

    [SpecialName]
    private string _0024STATIC_0024Kinder_00242001_0024Vor;

    [SpecialName]
    private byte I1;

    [SpecialName]
    private byte _0024STATIC_0024Kinder_00242001_0024I;
    private short M1_J;
    private string M_Namen;
    private byte M_KonLen;

    [DebuggerNonUserCode]
    public AhnenST()
    {
        this.Befehl = new ControlArray<Button>();
        this.Bezeichnung1 = new ControlArray<Label>();
        this.Command_Renamed = new ControlArray<Button>();
        this.Command2 = new ControlArray<Button>();
        this.Label1 = new ControlArray<Label>();
        this.RichTextBox1 = new ControlArray<RichTextBox>();
        base.Load += AhnenST_Load;
        base.FormClosing += AhnenST_FormClosing;
        InitializeComponent();
        this.Command2.SetIndex(this._Command2_3, 3);
        this.Command2.SetIndex(this._Command2_0, 0);
        this.Command2.SetIndex(this._Command2_1, 1);
        this.Command2.SetIndex(this._Command2_2, 2);
        this.RichTextBox1.SetIndex(this._RichTextBox1_1, 1);
        this.Befehl.SetIndex(this._Befehl_6, 6);
        this.Befehl.SetIndex(this._Befehl_5, 5);
        this.Befehl.SetIndex(this._Befehl_4, 4);
        this.Befehl.SetIndex(this._Befehl_0, 0);
        this.RichTextBox1.SetIndex(this._RichTextBox1_0, 0);
        this.Befehl.SetIndex(this._Befehl_1, 1);
        this.Befehl.SetIndex(this._Befehl_2, 2);
        this.Befehl.SetIndex(this._Befehl_3, 3);
        this.RichTextBox1.SetIndex(this._RichTextBox1_2, 2);
        this.RichTextBox1.SetIndex(this._RichTextBox1_3, 3);
        this.Command_Renamed.SetIndex(this._Command_1, 1);
        this.Command_Renamed.SetIndex(this._Command_0, 0);
        this.Label1.SetIndex(this._Label1_5, 5);
        this.Label1.SetIndex(this._Label1_4, 4);
        this.Label1.SetIndex(this._Label1_3, 3);
        this.Label1.SetIndex(this._Label1_2, 2);
        this.Label1.SetIndex(this._Label1_1, 1);
        this.Label1.SetIndex(this._Label1_0, 0);
        this.Bezeichnung1.SetIndex(this._Bezeichnung1_2, 2);
        this.Bezeichnung1.SetIndex(this._Bezeichnung1_1, 1);
        this.Bezeichnung1.SetIndex(this._Bezeichnung1_0, 0);
        Befehl.AddClick(Befehl_Click);
        Command_Renamed.AddClick(Command_Renamed_Click);
        Command2.AddClick(Command2_Click);
    }

    private void Befehl_Click(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int index = default;
        int num2 = default;
        int num3 = default;
        int number = default;
        string text = default;
        string right = default;
        string text2 = default;
        string text3 = default;
        short num5 = default;
        int num6 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    string text4;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            index = Befehl.GetIndex((Button)eventSender);
                            goto IL_0015;
                        case 4647:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0f11;
                                    default:
                                        goto end_IL_0000;
                                }
                                number = Information.Err().Number;
                                if (number == 55)
                                {
                                    FileSystem.FileClose();
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0f0d;
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
                                goto IL_0f0d;
                            }
                        end_IL_0000:
                            break;
                        IL_0015:
                            num = 2;
                            text = "";
                            right = "";
                            text2 = "";
                            text3 = "";
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            text4 = "Datum " + DateTime.Today.Month.ToString() + "." + DateTime.Today.Day.ToString() + "." + DateTime.Today.Year.ToString();
                            switch (index)
                            {
                                case 0:
                                    break;
                                case 1:
                                    goto IL_068d;
                                case 2:
                                    goto IL_0737;
                                case 3:
                                    goto IL_075e;
                                case 4:
                                    goto IL_08b9;
                                case 5:
                                    goto IL_0941;
                                case 6:
                                    goto IL_0df2;
                                default:
                                    goto IL_0e57;
                            }
                            num6 = 0;
                            while (num6 <= 6)
                            {
                                Befehl[(short)num6].Visible = true;
                                num6++;
                            }
                            Befehl[0].Visible = false;
                            num5 = 0;
                            while (num5 <= 3)
                            {
                                RichTextBox1[num5].Visible = false;
                                num5 = (short)unchecked(num5 + 1);
                            }
                            RichTextBox1[2].Visible = true;
                            if (RichTextBox1[2].Text == "")
                            {
                                DataModul.DSB_NamIdxTable.Index = "Langi";
                                RichTextBox1[2].SelectionIndent = 50;
                                RichTextBox1[2].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                RichTextBox1[2].SelectedText = "Namen-Index (Langform)\n\n";
                                RichTextBox1[2].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                DataModul.DSB_NamIdxTable.Seek(">=", " ", " ", 0);
                                M_Namen = "";
                                goto IL_05ab;
                            }
                            goto IL_063e;
                        IL_05ab: // <========== 3
                                 // <========== 3
                            num = 29;
                            if (!DataModul.DSB_NamIdxTable.EOF)
                            {
                                RichTextBox1[2].SelectionIndent = 50;
                                if (M_Namen != DataModul.DSB_NamIdxTable.Fields["Name1"].AsString())
                                {
                                    if (text3 != "")
                                    {
                                        RichTextBox1[2].SelectionIndent = 70;
                                        RichTextBox1[2].SelectedText = text3 + "; " + Strings.Mid(text2, 2, text2.Length) + ".\n";
                                        text2 = "";
                                        right = "";
                                        text3 = "";
                                    }
                                    RichTextBox1[2].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                    RichTextBox1[2].SelectionIndent = 50;
                                    RichTextBox1[2].SelectedText = DataModul.DSB_NamIdxTable.Fields["Name1"].AsString() + '\n';
                                    RichTextBox1[2].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                    text2 = "";
                                    right = "";
                                    M_Namen = DataModul.DSB_NamIdxTable.Fields["Name1"].AsString();
                                }
                                if (text3 == "")
                                {
                                    text3 = DataModul.DSB_NamIdxTable.Fields["Name"].AsString();
                                }
                                if (text3 != DataModul.DSB_NamIdxTable.Fields["Name"].AsString())
                                {
                                    RichTextBox1[2].SelectionIndent = 70;
                                    RichTextBox1[2].SelectedText = text3 + ": " + Strings.Mid(text2, 2, text2.Length) + ".\n";
                                    text2 = "";
                                    right = 0.AsString();
                                    text3 = DataModul.DSB_NamIdxTable.Fields["Name"].AsString();
                                }
                                if (DataModul.DSB_NamIdxTable.Fields["Nr"].AsInt() != right.AsInt())
                                {
                                    text2 = text2 + ", " + DataModul.DSB_NamIdxTable.Fields["Nr"].AsString().Trim();
                                    right = DataModul.DSB_NamIdxTable.Fields["Nr"].AsString();
                                }
                                DataModul.DSB_NamIdxTable.MoveNext();
                                goto IL_05ab;
                            }
                            if (text3 != "")
                            {
                                RichTextBox1[2].SelectionIndent = 70;
                                RichTextBox1[2].SelectedText = text3 + "; " + Strings.Mid(text2, 2, text2.Length) + ".\n";
                                text2 = "";
                                right = "";
                                text3 = "";
                            }
                            goto IL_063e;
                        IL_063e: // <========== 3
                                 // <========== 3
                            num = 71;
                            RichTextBox1[2].SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text3.RTF", RichTextBoxStreamType.RichText);
                            RichTextBox1[2].LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text3.RTF", RichTextBoxStreamType.RichText);
                            goto end_IL_0000_2;
                        IL_068d:
                            num = 75;
                            num5 = 0;
                            goto IL_0694;
                        IL_0694: // <========== 3
                                 // <========== 3
                            num = 76;
                            if (!RichTextBox1[num5].Visible)
                            {
                                num5 = (short)unchecked(num5 + 1);
                                if (num5 <= 2)
                                {
                                    goto IL_0694;
                                }
                            }
                            RichTextBox1[num5].SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                            RichTextBox1[num5].LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Temp\\Text2.RTF", AppWinStyle.MaximizedFocus);
                            goto end_IL_0000_2;
                        IL_0737:
                            num = 85;
                            Close();
                            MyProject.Forms.Druck.Show();
                            goto end_IL_0000_2;
                        IL_075e:
                            num = 90;
                            MyProject.Forms.Hinter.CommonDialog1Save.Filter = "Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF";
                            num5 = 0;
                            goto IL_0782;
                        IL_0782: // <========== 3
                                 // <========== 3
                            num = 92;
                            if (!RichTextBox1[num5].Visible)
                            {
                                num5 = (short)unchecked(num5 + 1);
                                if (num5 <= 2)
                                {
                                    goto IL_0782;
                                }
                            }
                            MyProject.Forms.Hinter.CommonDialog1Save.InitialDirectory = _Modul1.Instance.GenFreeDir + "list\\";
                            MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex = 2;
                            MyProject.Forms.Hinter.CommonDialog1Save.ShowDialog();
                            if (MyProject.Forms.Hinter.CommonDialog1Save.FileName == "")
                            {
                                goto end_IL_0000_2;
                            }
                            switch (MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_0884;
                                default:
                                    goto end_IL_0000_2;
                            }
                            RichTextBox1[num5].SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);
                            goto end_IL_0000_2;
                        IL_0884:
                            num = 107;
                            RichTextBox1[num5].SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.RichText);
                            goto end_IL_0000_2;
                        IL_08b9:
                            num = 113;
                            num6 = 0;
                            while (num6 <= 6)
                            {
                                Befehl[(short)num6].Visible = true;
                                num6++;
                            }
                            Befehl[4].Visible = false;
                            num5 = 0;
                            while (num5 <= 3)
                            {
                                RichTextBox1[num5].Visible = false;
                                num5 = (short)unchecked(num5 + 1);
                            }
                            RichTextBox1[0].Visible = true;
                            goto end_IL_0000_2;
                        IL_0941:
                            num = 123;
                            num6 = 0;
                            while (num6 <= 6)
                            {
                                Befehl[(short)num6].Visible = true;
                                num6++;
                            }
                            Befehl[5].Visible = false;
                            num5 = 0;
                            while (num5 <= 3)
                            {
                                RichTextBox1[num5].Visible = false;
                                num5 = (short)unchecked(num5 + 1);
                            }
                            RichTextBox1[1].Visible = true;
                            if (RichTextBox1[1].Text == "")
                            {
                                DataModul.DSB_NamIdxTable.Index = "Kurzname";
                                RichTextBox1[1].SelectionIndent = 50;
                                RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                RichTextBox1[1].SelectedText = "Namen-Index (Kurzform)\n\n";
                                RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                DataModul.DSB_NamIdxTable.Seek(">=", " ", 0);
                                goto IL_0ccb;
                            }
                            goto IL_0da0;
                        IL_0ccb: // <========== 3
                                 // <========== 3
                            num = 139;
                            if (!DataModul.DSB_NamIdxTable.EOF)
                            {
                                if (text != DataModul.DSB_NamIdxTable.Fields["Name1"].AsString())
                                {
                                    if (text != "")
                                    {
                                        RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                        RichTextBox1[1].SelectedText = text;
                                        RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                        RichTextBox1[1].SelectedText = text2 + "\n\n";
                                        text2 = "";
                                        right = "";
                                    }
                                    text = DataModul.DSB_NamIdxTable.Fields["Name1"].AsString();
                                }
                                if (DataModul.DSB_NamIdxTable.Fields["Nr"].AsInt() != right.AsInt())
                                {
                                    text2 = text2 + ", " + DataModul.DSB_NamIdxTable.Fields["Nr"].AsString().Trim();
                                    right = DataModul.DSB_NamIdxTable.Fields["Nr"].AsString();
                                }
                                DataModul.DSB_NamIdxTable.MoveNext();
                                goto IL_0ccb;
                            }
                            RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                            RichTextBox1[1].SelectedText = text;
                            RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            RichTextBox1[1].SelectedText = text2 + "\n\n";
                            text2 = "";
                            right = "";
                            goto IL_0da0;
                        IL_0da0: // <========== 3
                                 // <========== 3
                            num = 164;
                            RichTextBox1[1].SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text3.RTF", RichTextBoxStreamType.RichText);
                            RichTextBox1[1].LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text3.RTF", RichTextBoxStreamType.RichText);
                            goto end_IL_0000_2;
                        IL_0df2:
                            num = 168;
                            num5 = 0;
                            while (num5 <= 3)
                            {
                                RichTextBox1[num5].Visible = false;
                                num5 = (short)unchecked(num5 + 1);
                            }
                            RichTextBox1[3].Visible = true;
                            Frame2.Visible = true;
                            goto end_IL_0000_2;
                        IL_0e57:
                            num = 175;
                            Interaction.MsgBox(index);
                            goto end_IL_0000_2;
                        IL_0f0d:
                            num4 = num2;
                            goto IL_0f15;
                        IL_0f11:
                            num4 = unchecked(num2 + 1);
                            goto IL_0f15;
                        IL_0f15:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 28:
                                case 29:
                                case 62:
                                    goto IL_05ab;
                                case 69:
                                case 70:
                                case 71:
                                    goto IL_063e;
                                case 76:
                                    goto IL_0694;
                                case 92:
                                    goto IL_0782;
                                case 138:
                                case 139:
                                case 156:
                                    goto IL_0ccb;
                                case 163:
                                case 164:
                                    goto IL_0da0;
                                case 9:
                                case 73:
                                case 83:
                                case 87:
                                case 88:
                                case 101:
                                case 105:
                                case 108:
                                case 109:
                                case 110:
                                case 111:
                                case 121:
                                case 166:
                                case 173:
                                case 176:
                                case 177:
                                case 179:
                                case 184:
                                case 185:
                                case 191:
                                case 192:
                                case 193:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 4647;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 12
            // <========== 12
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    private void Command_Renamed_Click(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int index = default;
        int left = default;
        string text = default;
        string left2 = default;
        byte b = default;
        int lErl = default;
        string str = default;
        byte b2 = default;
        string text2 = default;
        short num5 = default;
        short num6 = default;
        string text3 = default;
        object CounterResult = default;
        object LoopForResult = default;
        byte b3 = default;
        object LoopForResult2 = default;
        object LoopForResult3 = default;
        string text5 = default;
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
                    object obj;
                    int num7;
                    string text4;
                    short Listart;
                    bool neb;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 10520:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                    case 3:
                                        break;
                                    case 1:
                                        goto IL_234a;
                                    default:
                                        goto end_IL_0000;
                                }
                                goto IL_22a5;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            index = Command_Renamed.GetIndex((Button)eventSender);
                            left = 0;
                            text = "";
                            ProjectData.ClearProjectError();
                            num3 = 3;
                            left2 = "";
                            switch (index)
                            {
                                case 0:
                                    break;
                                case 1:
                                    goto IL_0082;
                                default:
                                    goto IL_22a5;
                            }
                            Close();
                            MyProject.Forms.Druck.Show();
                            goto end_IL_0000_2;
                        IL_0082:
                            num = 16;
                            Frame1.Visible = false;
                            if (Strings.Mid(Label1[3].Text, 16, 10).AsDouble() < 1.0)
                            {
                                Close();
                                MyProject.Forms.Druck.Show();
                                goto end_IL_0000_2;
                            }
                            RichTextBox1[0].Visible = true;
                            DataModul.DT_AncesterTable.Index = "Ahnen";
                            DataModul.DT_AncesterTable.MoveLast();
                            M_KonLen = (byte)Strings.Len(DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim());
                            RichTextBox1[0].Font = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            RichTextBox1[0].SelectionAlignment = HorizontalAlignment.Center;
                            if (_Modul1.Instance.Druck_Tast == 0)
                            {
                                RichTextBox1[0].SelectedText = "Ahnenliste nach Stämmen für:";
                                goto IL_01d8;
                            }
                            RichTextBox1[0].SelectedText = "Erweiterte Ahnenliste nach Stämmen für:";
                            goto IL_01d8;
                        IL_01d8: // <========== 3
                                 // <========== 3
                            num = 34;
                            RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            Bezeichnung1[0].Text = "Ahnenliste nach Stämmen" + DataModul.DT_AncesterTable.Fields["Gen"].AsString() + " Generationen für " + _Modul1.Instance.Person.Prae + _Modul1.Instance.Person.Givennames + " " + _Modul1.Instance.Kont[0].ToUpper();
                            Bezeichnung1[0].Refresh();
                            RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Person.Givennames.TrimEnd() + " ";
                            if (_Modul1.Instance.Kont[1].Trim() != "")
                            {
                                RichTextBox1[0].SelectedText = _Modul1.Instance.Kont[1].Trim() + " ";
                            }
                            M_Namen = _Modul1.Instance.Kont[0];
                            RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                            RichTextBox1[0].SelectedText = M_Namen.ToUpper() + "\n";
                            RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            RichTextBox1[0].SelectedText = "Erstellt am " + DateTime.Today.AsString();
                            RichTextBox1[0].SelectedText = " von " + _Modul1.Instance.User.Owner.Trim() + " mit Gen_Plus aus Mandant: " + _Modul1.Instance.Verz + "\n";
                            RichTextBox1[0].SelectionAlignment = HorizontalAlignment.Left;
                            RichTextBox1[0].SelectedText = "\n";
                            RichTextBox1[0].SelectionHangingIndent = unchecked(M_KonLen) * 5;
                            DataModul.DT_AncesterTable.Index = "Namen";
                            DataModul.DT_AncesterTable.MoveFirst();
                            b = 1;
                            goto IL_04b7;
                        IL_04b7: // <========== 3
                                 // <========== 3
                            num = 54;
                            lErl = 1;
                            obj = 0;
                            if (DataModul.DT_AncesterTable.EOF)
                            {
                                RichTextBox1[0].SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                                RichTextBox1[0].LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                                goto end_IL_0000_2;
                            }
                            str = Strings.Right("00000000" + DataModul.DT_AncesterTable.Fields["Ahn"].AsDouble().AsString().Trim(), 8);
                            b2 = (Byte)DataModul.DT_AncesterTable.Fields["Gen"].AsInt();
                            b = (byte)(unchecked(b) + 1);
                            if (b == 12)
                            {
                                b = 10;
                            }
                            if (!DataModul.DT_AncesterTable.NoMatch)
                            {
                                text += ">";
                                Bezeichnung1[2].Text = text;
                                Bezeichnung1[2].Refresh();
                                if (text.Length == 50)
                                {
                                    text = "";
                                }
                                if (_Modul1.Instance.Schalt == 0)
                                {
                                    if (DataModul.DT_AncesterTable.Fields["Ahn"].AsInt() == (left + 1))
                                    {
                                        if (Conversions.ToBoolean(Operators.CompareString(left2, "M", TextCompare: false) == 0 & (left.AsInt() != 1)))
                                        {
                                            text2 = "";
                                            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                            if (_Modul1.Instance.UbgT.Length > 10)
                                            {
                                                text2 = _Modul1.Instance.UbgT;
                                                _Modul1.Instance.PersInArb = DataModul.DT_AncesterTable.Fields["PerNr"].AsInt();
                                                _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                                                aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                                if (_Modul1.Instance.UbgT.Length > 10)
                                                {
                                                    num5 = (short)Math.Round(text2.Length / 10.0);
                                                    num6 = 1;
                                                    goto IL_0817;
                                                }
                                                _Modul1.Instance.FamInArb = (int)Math.Round(_Modul1.Instance.UbgT.AsDouble());
                                                goto IL_085e;
                                            }
                                            _Modul1.Instance.FamInArb = (int)Math.Round(_Modul1.Instance.UbgT.AsDouble());
                                            goto IL_085e;
                                        }
                                        goto IL_0ad6;
                                    }
                                }
                                goto IL_0ad6;
                            }
                            goto IL_2256;
                        IL_07fe: // <========== 3
                                 // <========== 3
                            num = 97;
                            if (_Modul1.Instance.Schalt != 1)
                            {
                                num6 = (short)unchecked(num6 + 1);
                                goto IL_0817;
                            }
                            goto IL_085e;
                        IL_0817:
                            if (num6 <= num5)
                            {
                                text3 = Strings.Mid(text2, num6 * 10 - 9, 10);
                                if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 1, _Modul1.Instance.UbgT.Length / 10.0, 1, ref LoopForResult, ref CounterResult))
                                {
                                    while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult, ref CounterResult))
                                    {
                                        if (Operators.CompareString(text3, Strings.Mid(_Modul1.Instance.UbgT, Conversions.ToInteger(Operators.SubtractObject(Operators.MultiplyObject(CounterResult, 10), 9)), 10), TextCompare: false) == 0)
                                        {
                                            _Modul1.Instance.FamInArb = text3.AsInt();
                                            _Modul1.Instance.Schalt = 1;
                                            goto IL_07fe;
                                        }
                                    }
                                }
                                goto IL_07fe;
                            }
                            goto IL_085e;
                        IL_085e: // <========== 5
                                 // <========== 5
                            num = 109;
                            RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            _Modul1.Instance.Schalt = 0;
                            _Modul1.Instance.Famdatles2();
                            if (_Modul1.Instance.Kont[0].Trim() != "")
                            {
                                RichTextBox1[0].SelectedText = "                      " + _Modul1.Instance.DTxt[5];
                                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[0] + "\n";
                            }
                            if (_Modul1.Instance.Kont[1].Trim() != "")
                            {
                                RichTextBox1[0].SelectedText = "                      " + _Modul1.Instance.DTxt[6];
                                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[1] + "\n";
                            }
                            if (_Modul1.Instance.Kont[2].Trim() != "")
                            {
                                RichTextBox1[0].SelectedText = "                      " + _Modul1.Instance.DTxt[7];
                                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[2] + "\n";
                            }
                            if (_Modul1.Instance.Kont[3].Trim() != "")
                            {
                                RichTextBox1[0].SelectedText = "                      " + _Modul1.Instance.DTxt[8];
                                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[3] + "\n";
                            }
                            if (_Modul1.Instance.Kont[4].Trim() != "")
                            {
                                RichTextBox1[0].SelectedText = "                      " + _Modul1.Instance.DTxt[9];
                                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[4] + "\n";
                            }
                            goto IL_0ad6;
                        IL_0ad6: // <========== 4
                                 // <========== 4
                            num = 135;
                            if (DataModul.DT_AncesterTable.Fields["Ehe"].AsInt() != 0)
                            {
                                num7 = DataModul.DT_AncesterTable.Fields["Ehe"].AsInt();
                            }
                            _Modul1.Instance.Schalt = 0;
                            left = DataModul.DT_AncesterTable.Fields["Ahn"].AsInt();
                            _Modul1.Instance.PersInArb = DataModul.DT_AncesterTable.Fields["PerNr"].AsInt();
                            if (_Modul1.Instance.PersInArb == 0)
                            {
                                _Modul1.Instance.FamInArb = DataModul.DT_AncesterTable.Fields["Ehe"].AsInt();
                                goto IL_2256;
                            }
                            DataModul.DB_PersonTable.Seek("=", _Modul1.Instance.PersInArb.AsString());
                            left2 = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString();
                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            if (unchecked(b2 > (uint)b3))
                            {
                                RichTextBox1[0].SelectedText = "\n";
                                RichTextBox1[0].Visible = true;
                                Bezeichnung1[1].Text = "Bearbeite Stamm " + _Modul1.Instance.AltName;
                                Bezeichnung1[1].Refresh();
                                text4 = "Generation " + b2.AsString() + "\n";
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            }
                            lErl = 3;
                            this.Ahne = Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsDouble().AsString().Trim(), M_KonLen);
                            RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            if ((List1.Items.Count == 0) | (Operators.CompareString(Strings.Trim(List1.Items[0].AsString().Left(15)), this.Ahne.Trim(), TextCompare: false) == 0))
                            {
                                if (List1.Items.Count > 0)
                                {
                                    if (DataModul.DT_AncesterTable.Fields["Ahn2"].AsInt() > 0)
                                    {
                                        str = Strings.Right("00000000" + DataModul.DT_AncesterTable.Fields["Ahn1"].AsString().Trim(), 8);
                                        this.Ahne = Strings.Right("           " + DataModul.DT_AncesterTable.Fields["Ahn2"].AsString().Trim() + str.Trim(), M_KonLen);
                                        RichTextBox1[0].SelectedText = "           " + this.Ahne.Right(10);
                                        goto IL_0fb2;
                                    }
                                    RichTextBox1[0].SelectedText = Strings.Right("           " + DataModul.DT_AncesterTable.Fields["Ahn1"].AsString(), M_KonLen) + " = ";
                                    goto IL_0fb2;
                                }
                                RichTextBox1[0].SelectionIndent = 0;
                                if (_Modul1.Instance.AltName != _Modul1.Instance.Kont[0])
                                {
                                    RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                    RichTextBox1[0].SelectedText = "  " + _Modul1.Instance.Kont[0].ToUpper() + "\n";
                                    _Modul1.Instance.AltName = _Modul1.Instance.Kont[0];
                                    RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                }
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                RichTextBox1[0].SelectedText = Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsDouble().AsString(), M_KonLen);
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                goto IL_1256;
                            }
                            if (List1.Items.Count > 0)
                            {
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.Font2, (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                if (DataModul.DT_AncesterTable.Fields["Ahn2"].AsInt() > 0)
                                {
                                    str = Strings.Right("00000000" + DataModul.DT_AncesterTable.Fields["Ahn1"].AsString().Trim(), 8);
                                    this.Ahne = Strings.Right("           " + DataModul.DT_AncesterTable.Fields["Ahn2"].AsString().Trim() + str.Trim(), 10);
                                    goto IL_17d4;
                                }
                                this.Ahne = Strings.Right("           " + DataModul.DT_AncesterTable.Fields["Ahn1"].AsString().Trim(), 10);
                                goto IL_17d4;
                            }
                            RichTextBox1[0].SelectedText = "        " + Strings.Right("          " + DataModul.DT_AncesterTable.Fields["Ahn1"].AsString(), 10);
                            goto IL_19c0;
                        IL_0fb2: // <========== 3
                                 // <========== 3
                            num = 170;
                            RichTextBox1[0].SelectedText = Strings.Trim(List1.Items[1].AsString());
                            if (List1.Items.Count > 2)
                            {
                                RichTextBox1[0].SelectedText = " >> ";
                            }
                            RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, List1.Items.Count - 1, 1, ref LoopForResult2, ref CounterResult))
                            {
                                while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult2, ref CounterResult))
                                {
                                    List1.Items.RemoveAt(0);
                                }
                            }
                            goto IL_1256;
                        IL_1256: // <========== 3
                                 // <========== 3
                            num = 191;
                            Namenindex(_Modul1.Instance.Kont[0]);
                            RichTextBox1[0].SelectedText = "   " + _Modul1.Instance.Person.Prae + _Modul1.Instance.Person.Givennames + " ";
                            if (_Modul1.Instance.Kont[1] != "")
                            {
                                RichTextBox1[0].SelectedText = _Modul1.Instance.Kont[1] + " ";
                            }
                            RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                            RichTextBox1[0].SelectedText = _Modul1.Instance.Kont[0].ToUpper();
                            RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            if (_Modul1.Instance.Kont[2].Trim() != "")
                            {
                                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[2].TrimEnd();
                            }
                            if (_Modul1.Instance.Kont[5].Trim() != "")
                            {
                                RichTextBox1[0].SelectedText = ", Sippe " + _Modul1.Instance.Kont[5].TrimEnd();
                            }
                            if (_Modul1.Instance.Kont[4].Trim() != "")
                            {
                                RichTextBox1[0].SelectedText = " (" + _Modul1.Instance.Kont[4].ToUpper() + ")";
                            }
                            Berufe((EEventArt)300);
                            Berufe((EEventArt)301);
                            RichTextBox1[0].SelectedText = "\n";
                            Listart = 0;
                            neb = false;
                            _Modul1.Instance.Datles3(Listart, default, default, ref neb);
                            RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            RichTextBox1[0].SelectionIndent = unchecked(M_KonLen) * 5;
                            if (_Modul1.Instance.Kont[11] != "")
                            {
                                RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[1] + " " + _Modul1.Instance.Kont[11] + "\n";
                            }
                            if (_Modul1.Instance.Kont[12] != "")
                            {
                                RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[2] + " " + _Modul1.Instance.Kont[12] + "\n";
                            }
                            if (_Modul1.Instance.Kont[13] != "")
                            {
                                RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[3] + " " + _Modul1.Instance.Kont[13] + "\n";
                            }
                            if (_Modul1.Instance.Kont[14] != "")
                            {
                                RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[4] + " " + _Modul1.Instance.Kont[14] + "\n";
                            }
                            goto IL_1b0d;
                        IL_17d4: // <========== 3
                                 // <========== 3
                            num = 239;
                            RichTextBox1[0].SelectedText = this.Ahne + " = ";
                            _Modul1.Instance.Schalt = 1;
                            if (Operators.CompareString(Strings.Trim(List1.Items[0].AsString().Left(15)), this.Ahne.Trim(), TextCompare: false) != 0)
                            {
                                RichTextBox1[0].SelectedText = Strings.Trim(List1.Items[0].AsString().Left(15)) + "; " + Strings.Trim(Strings.Mid(List1.Items[0].AsString(), 16, 3));
                                goto IL_18cb;
                            }
                            RichTextBox1[0].SelectedText = Strings.Trim(List1.Items[1].AsString().Left(15));
                            goto IL_18cb;
                        IL_18cb: // <========== 3
                                 // <========== 3
                            num = 247;
                            RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, List1.Items.Count - 1, 1, ref LoopForResult3, ref CounterResult))
                            {
                                while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult3, ref CounterResult))
                                {
                                    List1.Items.RemoveAt(0);
                                }
                            }
                            goto IL_19c0;
                        IL_19c0: // <========== 3
                                 // <========== 3
                            num = 255;
                            RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            RichTextBox1[0].SelectedText = "   " + _Modul1.Instance.Kont[3] + " ";
                            if (_Modul1.Instance.Kont[1] != "")
                            {
                                RichTextBox1[0].SelectedText = _Modul1.Instance.Kont[1] + " ";
                            }
                            RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                            RichTextBox1[0].SelectedText = _Modul1.Instance.Kont[0].ToUpper();
                            RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            RichTextBox1[0].SelectedText = "\n";
                            goto IL_1b0d;
                        IL_1b0d: // <========== 3
                                 // <========== 3
                            num = 265;
                            if (this.Ahne.AsDouble() != 1.0)
                            {
                                _Modul1.Instance.FamInArb = DataModul.DT_AncesterTable.Fields["Ehe"].AsInt();
                                if (_Modul1.Instance.Druck_Tast > 0)
                                {
                                    if (DataModul.DT_AncesterTable.Fields["Ehe"].AsInt() != 0)
                                    {
                                        _Modul1.Instance.FamInArb = DataModul.DT_AncesterTable.Fields["Ehe"].AsInt();
                                        _Modul1.Instance.Family_Les(_Modul1.Instance.FamInArb, _Modul1.Instance.Family);
                                        if (_Modul1.Instance.Family.Frau == _Modul1.Instance.PersInArb)
                                        {
                                            if (_Modul1.Instance.Family.Mann > 0)
                                            {
                                                _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                                text5 = _Modul1.Instance.Person.Givennames + " " + _Modul1.Instance.Kont[0].ToUpper();
                                                RichTextBox1[0].SelectionIndent = unchecked(M_KonLen) * 5;
                                                _Modul1.Instance.Schalt = 0;
                                                _Modul1.Instance.Famdatles2();
                                                if (_Modul1.Instance.Kont[0].Trim() != "")
                                                {
                                                    RichTextBox1[0].SelectedText = "  " + _Modul1.Instance.DTxt[5];
                                                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[0] + "\n";
                                                }
                                                if (_Modul1.Instance.Kont[1].Trim() != "")
                                                {
                                                    RichTextBox1[0].SelectedText = "  " + _Modul1.Instance.DTxt[6];
                                                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[1] + "\n";
                                                }
                                                if (_Modul1.Instance.Kont[2].Trim() != "")
                                                {
                                                    RichTextBox1[0].SelectedText = "  " + _Modul1.Instance.DTxt[7];
                                                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[2] + "\n";
                                                }
                                                if (_Modul1.Instance.Kont[3].Trim() != "")
                                                {
                                                    RichTextBox1[0].SelectedText = "  " + _Modul1.Instance.DTxt[8];
                                                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[3] + "\n";
                                                }
                                                if (_Modul1.Instance.Kont[4].Trim() != "")
                                                {
                                                    RichTextBox1[0].SelectedText = "  " + _Modul1.Instance.DTxt[9];
                                                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[4] + "\n";
                                                }
                                                RichTextBox1[0].SelectedText = " mit " + text5 + "\n";
                                                text5 = "";
                                            }
                                            goto IL_2256;
                                        }
                                        if (_Modul1.Instance.Family.Frau > 0)
                                        {
                                            _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau;
                                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                            text5 = _Modul1.Instance.Person.Givennames + " " + _Modul1.Instance.Kont[0].ToUpper();
                                            RichTextBox1[0].SelectionIndent = unchecked(M_KonLen) * 5;
                                            _Modul1.Instance.Schalt = 0;
                                            _Modul1.Instance.Famdatles2();
                                            if (_Modul1.Instance.Kont[0].Trim() != "")
                                            {
                                                RichTextBox1[0].SelectedText = "  " + _Modul1.Instance.DTxt[5];
                                                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[0].ToUpper() + "\n";
                                            }
                                            if (_Modul1.Instance.Kont[1].Trim() != "")
                                            {
                                                RichTextBox1[0].SelectedText = "  " + _Modul1.Instance.DTxt[6];
                                                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[1] + "\n";
                                            }
                                            if (_Modul1.Instance.Kont[2].Trim() != "")
                                            {
                                                RichTextBox1[0].SelectedText = "  " + _Modul1.Instance.DTxt[7];
                                                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[2] + "\n";
                                            }
                                            if (_Modul1.Instance.Kont[3].Trim() != "")
                                            {
                                                RichTextBox1[0].SelectedText = "  " + _Modul1.Instance.DTxt[8];
                                                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[3] + "\n";
                                            }
                                            if (_Modul1.Instance.Kont[4].Trim() != "")
                                            {
                                                RichTextBox1[0].SelectedText = "  " + _Modul1.Instance.DTxt[9];
                                                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[4] + "\n";
                                            }
                                            RichTextBox1[0].SelectedText = " mit " + text5 + "\n";
                                            text5 = "";
                                            Kinder();
                                            goto IL_2256;
                                        }
                                        Kinder();
                                    }
                                    goto IL_2256;
                                }
                            }
                            goto IL_2256;
                        IL_2256: // <========== 7
                                 // <========== 7
                            num = 343;
                            while (DataModul.DT_AncesterTable.NoMatch)
                            {
                                lErl = 4;
                                DataModul.DT_AncesterTable.MoveNext();
                                if (DataModul.DT_AncesterTable.EOF)
                                {
                                    goto end_IL_0000_2;
                                }
                            }
                            goto IL_04b7;
                        IL_22a5: // <========== 3
                                 // <========== 3
                            num = 355;
                            if (Information.Err().Number == 3021)
                            {
                                Frame1.Visible = false;
                                Kinder();
                                goto end_IL_0000_2;
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
                            goto IL_234e;
                        IL_234a:
                            num4 = unchecked(num2 + 1);
                            goto IL_234e;
                        IL_234e:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 30:
                                case 33:
                                case 34:
                                    goto IL_01d8;
                                case 54:
                                case 350:
                                case 351:
                                    goto IL_04b7;
                                case 94:
                                case 97:
                                    goto IL_07fe;
                                case 98:
                                case 101:
                                case 104:
                                case 105:
                                case 108:
                                case 109:
                                    goto IL_085e;
                                case 131:
                                case 132:
                                case 133:
                                case 134:
                                case 135:
                                    goto IL_0ad6;
                                case 166:
                                case 169:
                                case 170:
                                    goto IL_0fb2;
                                case 178:
                                case 190:
                                case 191:
                                    goto IL_1256;
                                case 235:
                                case 238:
                                case 239:
                                    goto IL_17d4;
                                case 243:
                                case 246:
                                case 247:
                                    goto IL_18cb;
                                case 251:
                                case 254:
                                case 255:
                                    goto IL_19c0;
                                case 227:
                                case 228:
                                case 264:
                                case 265:
                                    goto IL_1b0d;
                                case 68:
                                case 143:
                                case 266:
                                case 303:
                                case 304:
                                case 336:
                                case 339:
                                case 340:
                                case 341:
                                case 342:
                                case 343:
                                case 349:
                                    goto IL_2256;
                                case 8:
                                case 14:
                                case 352:
                                case 353:
                                case 354:
                                case 355:
                                    goto IL_22a5;
                                case 13:
                                case 20:
                                case 59:
                                case 346:
                                case 358:
                                case 361:
                                case 367:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj2) when (obj2 is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj2, lErl);
                try0000_dispatch = 10520;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 7
            // <========== 7
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    private void Command2_Click(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int index = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                int ortNr;
                int ortNr2;
                int ortNr3;
                byte Schalt;
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0007;
                    case 4400:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0ee2;
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
                            goto IL_0ee6;
                        }
                    end_IL_0000:
                        break;
                    IL_0007:
                        num = 2;
                        index = Command2.GetIndex((Button)eventSender);
                        Frame2.Visible = false;
                        _Modul1.Instance.Ind1 = "";
                        RichTextBox1[3].Text = "";
                        RichTextBox1[3].SelectionHangingIndent = 0;
                        RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        switch (index)
                        {
                            case 0:
                                break;
                            case 1:
                                goto IL_03e5;
                            case 2:
                                goto IL_089d;
                            case 3:
                                goto IL_0cdf;
                            default:
                                goto IL_0e79;
                        }
                        RichTextBox1[3].SelectionAlignment = HorizontalAlignment.Center;
                        RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                        RichTextBox1[3].SelectedText = "Ortsindex";
                        RichTextBox1[3].SelectedText = "\n";
                        DataModul.DSB_OrtIdxTable.Index = "Ort";
                        DataModul.DSB_OrtIdxTable.Seek(">=", " ");
                        while (!DataModul.DSB_OrtIdxTable.EOF)
                        {
                            if (DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt() != _Modul1.Instance.AltNr)
                            {
                                RichTextBox1[3].SelectionAlignment = HorizontalAlignment.Left;
                                RichTextBox1[3].SelectedText = "\n";
                                RichTextBox1[3].SelectionIndent = 0;
                                _Modul1.Instance.UbgT = _Modul1.Instance.ortles(DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt(), 21);
                                RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                RichTextBox1[3].SelectedText = _Modul1.Instance.UbgT;
                                _Modul1.Instance.AltNr = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                                RichTextBox1[3].SelectedText = "\n";
                                RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                RichTextBox1[3].SelectionIndent = 40;
                            }
                            RichTextBox1[3].SelectedText = DataModul.DSB_OrtIdxTable.Fields["Ind"].AsString().Trim() + "; ";
                            DataModul.DSB_OrtIdxTable.MoveNext();
                        }
                        Befehl[4].Visible = true;
                        RichTextBox1[3].SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text3.RTF", RichTextBoxStreamType.RichText);
                        RichTextBox1[3].LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text3.RTF", RichTextBoxStreamType.RichText);
                        goto end_IL_0000_2;
                    IL_03e5:
                        num = 41;
                        RichTextBox1[3].SelectionAlignment = HorizontalAlignment.Center;
                        RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                        RichTextBox1[3].SelectedText = "Index Orte-Namen";
                        RichTextBox1[3].SelectedText = "\n";
                        RichTextBox1[3].SelectionHangingIndent = 0;
                        DataModul.DSB_OrtIdxTable.Index = "ortnam";
                        DataModul.DSB_OrtIdxTable.Seek(">=", " ");
                        while (!DataModul.DSB_OrtIdxTable.EOF)
                        {
                            if (DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt() != _Modul1.Instance.AltNr)
                            {
                                RichTextBox1[3].SelectedText = "\n";
                                RichTextBox1[3].SelectionAlignment = HorizontalAlignment.Left;
                                RichTextBox1[3].SelectionIndent = 0;
                                _Modul1.Instance.AltName = "";
                                _Modul1.Instance.AltNr = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                                RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                _Modul1.Instance.UbgT = _Modul1.Instance.ortles(DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt(), 21);
                                RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                RichTextBox1[3].SelectedText = _Modul1.Instance.UbgT;
                                RichTextBox1[3].SelectedText = "\n";
                                RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                RichTextBox1[3].SelectionIndent = 40;
                            }
                            if (Operators.CompareString(DataModul.DSB_OrtIdxTable.Fields["Name"].AsString().Trim(), _Modul1.Instance.AltName.Trim(), TextCompare: false) != 0)
                            {
                                if (_Modul1.Instance.AltName != "")
                                {
                                    RichTextBox1[3].SelectedText = "\n";
                                }
                                RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                RichTextBox1[3].SelectedText = (DataModul.DSB_OrtIdxTable.Fields["Name"].Value + "  ").AsString();
                                RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                _Modul1.Instance.AltName = DataModul.DSB_OrtIdxTable.Fields["Name"].AsString();
                            }
                            RichTextBox1[3].SelectedText = DataModul.DSB_OrtIdxTable.Fields["Ind"].AsString().Trim() + "; ";
                            DataModul.DSB_OrtIdxTable.MoveNext();
                        }
                        Befehl[4].Visible = true;
                        RichTextBox1[3].SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text3.RTF", RichTextBoxStreamType.RichText);
                        RichTextBox1[3].LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text3.RTF", RichTextBoxStreamType.RichText);
                        goto end_IL_0000_2;
                    IL_089d:
                        num = 82;
                        RichTextBox1[3].SelectionAlignment = HorizontalAlignment.Center;
                        RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                        RichTextBox1[3].SelectedText = "Index Namen-Orte";
                        RichTextBox1[3].SelectedText = "\n";
                        DataModul.DSB_OrtIdxTable.Index = "NameOrt";
                        DataModul.DSB_OrtIdxTable.Seek(">=", " ");
                        while (!DataModul.DSB_OrtIdxTable.EOF)
                        {
                            if (Operators.CompareString(DataModul.DSB_OrtIdxTable.Fields["Name"].AsString().Trim(), _Modul1.Instance.AltName.Trim(), TextCompare: false) != 0)
                            {
                                RichTextBox1[3].SelectionAlignment = HorizontalAlignment.Left;
                                RichTextBox1[3].SelectedText = "\n";
                                RichTextBox1[3].SelectionIndent = 0;
                                _Modul1.Instance.AltNr = 0;
                                RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                RichTextBox1[3].SelectedText = DataModul.DSB_OrtIdxTable.Fields["Name"].AsString();
                                _Modul1.Instance.AltName = DataModul.DSB_OrtIdxTable.Fields["Name"].AsString();
                                RichTextBox1[3].SelectedText = "\n";
                                RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                RichTextBox1[3].SelectionIndent = 40;
                            }
                            if (DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt() != _Modul1.Instance.AltNr)
                            {
                                if (_Modul1.Instance.AltNr > 0)
                                {
                                    RichTextBox1[3].SelectedText = "\n";
                                }
                                RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                RichTextBox1[3].SelectedText = (DataModul.DSB_OrtIdxTable.Fields["Ort"].Value + "  ").AsString();
                                RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                _Modul1.Instance.AltNr = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                            }
                            RichTextBox1[3].SelectedText = DataModul.DSB_OrtIdxTable.Fields["Ind"].AsString().Trim() + "; ";
                            DataModul.DSB_OrtIdxTable.MoveNext();
                        }
                        Befehl[4].Visible = true;
                        RichTextBox1[3].SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text3.RTF", RichTextBoxStreamType.RichText);
                        RichTextBox1[3].LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text3.RTF", RichTextBoxStreamType.RichText);
                        goto end_IL_0000_2;
                    IL_0cdf:
                        num = 119;
                        _Modul1.Instance.PrintDat.Flagsch = 1;
                        DataModul.DSB_OrtIdxTable.Index = "Ort";
                        DataModul.DSB_OrtIdxTable.Seek(">=", " ");
                        goto IL_0e64;
                    IL_0e64: // <========== 3
                             // <========== 3
                        num = 123;
                        if (DataModul.DSB_OrtIdxTable.EOF)
                        {
                            goto end_IL_0000_2;
                        }
                        if (DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt() != _Modul1.Instance.AltNr)
                        {
                            RichTextBox1[3].SelectionIndent = 0;
                            _Modul1.Instance.UbgT = _Modul1.Instance.ortles(DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt(), 2);
                            RichTextBox1[3].SelectedText = _Modul1.Instance.UbgT;
                            _Modul1.Instance.AltNr = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                            RichTextBox1[3].SelectedText = "\n";
                        }
                        DataModul.DSB_OrtIdxTable.MoveNext();
                        goto IL_0e64;
                    IL_0e79:
                        num = 136;
                        Interaction.MsgBox(index);
                        goto end_IL_0000_2;
                    IL_0ee2:
                        num4 = num2 + 1;
                        goto IL_0ee6;
                    IL_0ee6:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 122:
                            case 123:
                            case 133:
                                goto IL_0e64;
                            case 9:
                            case 39:
                            case 80:
                            case 117:
                            case 134:
                            case 137:
                            case 138:
                            case 143:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 4400;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 6
            // <========== 6
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    private void AhnenST_Load(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int num6 = default;
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
                    int num7;
                    short Listart;
                    string Ahne;
                    bool neb;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            _Modul1.Instance.DAus[101] = _Modul1.Instance.Font1;
                            goto IL_0010;
                        case 2363:
                            short num5;
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0755;
                                    default:
                                        goto end_IL_0000;
                                }
                                int number = Information.Err().Number;
                                if (number == 55)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0755;
                                }
                                if (number == 3021)
                                {
                                    num5 = 11;
                                    while (num5 <= 14)
                                    {
                                        _Modul1.Instance.Kont[num5] = "";
                                        num5 = (short)unchecked(num5 + 1);
                                    }
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num2 = 0;
                                    goto IL_059d;
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
                                goto IL_0759;
                            }
                        end_IL_0000:
                            break;
                        IL_0010:
                            num = 2;
                            _Modul1.Instance.DAus[102] = "10";
                            num5 = 0;
                            while (num5 <= 3)
                            {
                                RichTextBox1[num5].Font = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                num5 = (short)unchecked(num5 + 1);
                            }
                            BackColor = _Modul1.Instance.HintFarb;
                            Befehl[3].Text = _Modul1.Instance.IText[47];
                            _Modul1.Instance.Dateienopen();
                            num5 = 0;
                            while (num5 <= 3)
                            {
                                RichTextBox1[num5].Width = 1000;
                                RichTextBox1[num5].Height = 600;
                                RichTextBox1[num5].RightMargin = RichTextBox1[num5].Width - 30;
                                num5 = (short)unchecked(num5 + 1);
                            }
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
                            }
                            ;
                            Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            Show();
                            _Modul1.Instance.eWindowState = _Modul1.Instance.Persistence.ReadEnumInit<Enum>("Windowstate");
                            WindowState = _Modul1.Instance.eWindowState.AsEnum<FormWindowState>();
                            Bezeichnung1[0].Width = Width;
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            num7 = num6;
                            num6 = 0;
                            Show();
                            FileSystem.FileClose(30);
                            Label1[3].Text = "Keine Berechnung vorhanden";
                            Label1[0].Text = "Sie müssen erst die Ahnen berechnen.";
                            DataModul.DT_AncesterTable.Index = "Ahnen";
                            DataModul.DT_AncesterTable.Seek("=", 1);
                            DataModul.DT_AncesterTable.MoveFirst();
                            if (!DataModul.DT_AncesterTable.EOF)
                            {
                                if (!DataModul.DT_AncesterTable.NoMatch)
                                {
                                    string text = DataModul.DT_AncesterTable.Fields["Ahn"].AsString();
                                    DataModul.DT_AncesterTable.MoveLast();
                                    Label1[3].Text = "Ahnenberechnung " + DataModul.DT_AncesterTable.Fields["Gen"].AsString() + " Generationen vorhanden für Ahnenziffer " + text;
                                    DataModul.DT_AncesterTable.MoveFirst();
                                    _Modul1.Instance.PersInArb = DataModul.DT_AncesterTable.Fields["PerNr"].AsInt();
                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                    Label1[0].Text = _Modul1.Instance.Person.Prae + _Modul1.Instance.Person.Givennames + " " + _Modul1.Instance.Kont[0].ToUpper();
                                    Listart = 0;
                                    Ahne = 0.AsString();
                                    neb = false;
                                    _Modul1.Instance.Datles3(Listart, default, default, ref neb);
                                }
                            }
                            goto IL_059d;
                        IL_059d: // <========== 3
                                 // <========== 4
                            num = 88;
                            lErl = 1;
                            Label1[5].Text = _Modul1.Instance.IText[3] + " " + _Modul1.Instance.Kont[11];
                            Label1[2].Text = _Modul1.Instance.IText[5] + " " + _Modul1.Instance.Kont[13];
                            Label1[1].Text = _Modul1.Instance.IText[4] + " " + _Modul1.Instance.Kont[12];
                            Label1[4].Text = _Modul1.Instance.DTxt[4] + " " + _Modul1.Instance.Kont[14];
                            Frame1.Visible = true;
                            Visible = true;
                            goto end_IL_0000_2;
                        IL_0755:
                            num4 = unchecked(num2 + 1);
                            goto IL_0759;
                        IL_0759:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 75:
                                case 78:
                                case 88:
                                case 108:
                                    goto IL_059d;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 2363;
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

    private void AhnenST_FormClosing(object eventSender, FormClosingEventArgs eventArgs)
    {
        //Discarded unreachable code: IL_00ef
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
                    case 341:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_00f3;
                                default:
                                    goto end_IL_0000;
                            }
                            if (Information.Err().Number != 91)
                            {
                                break;
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_00f3;
                        }
                    end_IL_0000_3:
                        break;
                    IL_000a:
                        num = 2;
                        closeReason = eventArgs.CloseReason;
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        FileSystem.FileClose(6);
                        FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\Windowstate", OpenMode.Output);
                        FileSystem.PrintLine(6, WindowState);
                        if (closeReason != 0)
                        {
                            goto end_IL_0000_2;
                        }
                        DataModul.MandDB.Close();
                        DataModul.DOSB.Close();
                        DataModul.TempDB.Close();
                        DataModul.DSB.Close();
                        ProjectData.EndApp();
                        goto end_IL_0000_2;
                    IL_00f3:
                        num4 = num2 + 1;
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 17:
                            case 19:
                                goto end_IL_0000_3;
                            case 12:
                            case 13:
                            case 14:
                            case 20:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
                num = 19;
                eventArgs.Cancel = cancel;
                break;
            end_IL_0000:
                ;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 341;
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

    public void Namenindex(string sName)
    {
        ProjectData.ClearProjectError();
        if (sName == "" || sName == "NN" || sName == "N.N.") return;

        DataModul.DSB_NamIdxTable.AddNew();
        if (_Modul1.Instance.Person.Givennames == "")
        {
            DataModul.DSB_NamIdxTable.Fields["Name"].Value = "?";
        }
        else
            DataModul.DSB_NamIdxTable.Fields["Name"].Value = _Modul1.Instance.Person.Givennames;
        DataModul.DSB_NamIdxTable.Fields["Name1"].Value = sName;
        M_Namen = sName;
        IZahl = checked((long)Math.Round(Ahne.AsDouble()));
        DataModul.DSB_NamIdxTable.Fields["Nr"].Value = Ahne;
        _Modul1.Instance.Ind1 = IZahl.AsString().PadLeft(20);
        DataModul.DSB_NamIdxTable.Update();
    }

    public void Berufe(EEventArt Modul1_Beruf)
    {
        byte b = default;
        int AAA;
        List2.Items.Clear();

        GenFree.Interfaces.DB.IRecordset dB_EventTable = DataModul.DB_EventTable;
        dB_EventTable.Index = "Besu";
        dB_EventTable.Seek("=", Modul1_Beruf.AsString(), _Modul1.Instance.PersInArb.AsString());
        if (dB_EventTable.NoMatch)
        {
            dB_EventTable.Index = "ArtNr";
            goto end_IL_0000_2;
        }
        string text2 = "";
        I1 = 1;
        while (I1 <= 70u && !dB_EventTable.EOF)
        {
            if (dB_EventTable.Fields[EventFields.LfNr].AsInt() >= 1)
            {
                M1_J = 0;
                while (unchecked(M1_J) <= 15u)
                {
                    _Modul1.Instance.Kont[M1_J] = "";
                    M1_J++;
                }
                _DaT = "        ";
                if (!Conversions.ToBoolean(dB_EventTable.NoMatch | dB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb
                       | dB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != Modul1_Beruf))
                {
                    _Modul1.Instance.UbgT = "";
                    _Modul1.Instance.sDatu = dB_EventTable.Fields[EventFields.DatumV].AsString();
                    if (_Modul1.Instance.sDatu.AsDouble() == 0.0)
                    {
                        _Modul1.Instance.sDatu = dB_EventTable.Fields[EventFields.DatumB].AsString();
                    }
                    _DaT = Strings.Right("        " + _Modul1.Instance.sDatu.AsDouble().AsString(), 8);
                    if (dB_EventTable.Fields[EventFields.KBem].Value.AsDouble() > 0.0)
                    {
                        AAA = dB_EventTable.Fields[EventFields.KBem].AsInt();
                        _Modul1.Instance.Kont[0] = DataModul.TextLese1(AAA);
                        if (_Modul1.Instance.Kont[0] != "")
                        {
                            _Modul1.Instance.Kont[8] = " " + _Modul1.Instance.Kont[0].Trim() + " ";
                        }
                    }
                    if ((_Modul1.Instance.Kont[3] != "") | (_Modul1.Instance.Kont[4] != ""))
                    {
                        _Modul1.Instance.Kont[2] = " {" + _Modul1.Instance.Kont[3].Trim();
                        if (_Modul1.Instance.Kont[3] == "")
                        {
                            _Modul1.Instance.Kont[2] = " {" + _Modul1.Instance.Kont[4].Trim();
                        }
                        else
                        {
                            _Modul1.Instance.Kont[2] = _Modul1.Instance.Kont[2] + " " + _Modul1.Instance.Kont[4].Trim();
                        }
                        _Modul1.Instance.Kont[2] = _Modul1.Instance.Kont[2] + "} ";
                    }
                    if (dB_EventTable.Fields[EventFields.Reg].AsString() != " ")
                    {
                        text2 = _Modul1.Instance.Kont[8].Trim();
                        List2.Items.Add("");
                    }
                    else
                    {
                        List2.Items.Add(_DaT + _Modul1.Instance.Kont[8].Trim());
                    }
                }
                else
                {
                    dB_EventTable.Index = "ArtNr";
                    break;
                }
            }
            dB_EventTable.MoveNext();
            I1++;
        }

        switch (Modul1_Beruf)
        {
            case GenFree.Data.EEventArt.eA_300:
                if (List2.Items.Count == 0)
                {
                    goto end_IL_0000_2;
                }
                if (List2.Items.Count == 1)
                {
                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.IText[7] + " ";
                }
                if (List2.Items.Count > 1)
                {
                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.IText[111] + " ";
                }
                break;
            case GenFree.Data.EEventArt.eA_301:
                if (List2.Items.Count == 0)
                {
                    goto end_IL_0000_2;
                }
                if (List2.Items.Count > 0)
                {
                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.IText[70] + " ";
                }
                break;
            case GenFree.Data.EEventArt.eA_302:
                if (List2.Items.Count == 0)
                {
                    goto end_IL_0000_2;
                }
                if (List2.Items.Count == 1)
                {
                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.IText[8];
                }
                if (List2.Items.Count > 1)
                {
                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.IText[8];
                }
                break;
            default:
                break;
        }
        RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
        string text = "";
        b = (byte)(List2.Items.Count - 1);
        I1 = 0;
        while (unchecked(I1 <= (uint)b))
        {
            if ((Operators.CompareString(List2.Items[I1].AsString(), List2.Items[unchecked(I1) + 1].AsString(), TextCompare: false) != 0) & (List2.Items[I1].AsString() != text2))
            {
                if (List2.Items[I1].AsString() != "")
                {
                    if ((text == "") & (text2 == ""))
                    {
                        text = Strings.Mid(List2.Items[I1].AsString(), 9, List2.Items[I1].AsString().Length);
                    }
                    else text = text + "; " + Strings.Mid(List2.Items[I1].AsString(), 9, List2.Items[I1].AsString().Length);
                }
            }
            I1++;
        }
        text = text2.Trim() + text + ".";
        if (text.Trim() != "")
        {
            RichTextBox1[0].SelectedText = text.Trim();
        }
        List2.Items.Clear();
        goto end_IL_0000_2;
    end_IL_0000_2:; // <========== 6
    }
    public void Kinder()
    {
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
                checked
                {
                    int num4;
                    short Listart;
                    bool neb;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 2660:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_08c2;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 5)
                                {
                                    goto end_IL_0000_2;
                                }
                                if (Information.Err().Number == 3159)
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
                                goto IL_08c6;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            if (_Modul1.Instance.Druck_Tast == 0)
                            {
                                goto end_IL_0000_2;
                            }
                            RichTextBox1[0].SelectionIndent = 20;
                            RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                            _Modul1.Instance.Famles();
                            List1.Items.Clear();
                            _0024STATIC_0024Kinder_00242001_0024I = 1;
                            goto IL_008e;
                        IL_008e: // <========== 3
                            num = 10;
                            if (_Modul1.Instance.Family.Kind[_0024STATIC_0024Kinder_00242001_0024I] != 0)
                            {
                                DataModul.DB_EventTable.Seek("=", 101, _Modul1.Instance.Family.Kind[_0024STATIC_0024Kinder_00242001_0024I].AsString(), "0");
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    _Modul1.Instance.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                    goto IL_020f;
                                }
                                DataModul.DB_EventTable.Seek("=", 102.AsString(), _Modul1.Instance.Family.Kind[_0024STATIC_0024Kinder_00242001_0024I].AsString(), "0");
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    _Modul1.Instance.sDatu = 0.AsString();
                                }
                                goto IL_01d8;
                            }
                            goto IL_0283;
                        IL_01d8:
                            num = 22;
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                _Modul1.Instance.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                            }
                            goto IL_020f;
                        IL_020f: // <========== 3
                            num = 26;
                            List1.Items.Add(Strings.Right("        " + _Modul1.Instance.sDatu.AsDouble().AsString(), 8) + _Modul1.Instance.Family.Kind[_0024STATIC_0024Kinder_00242001_0024I].AsString());
                            _0024STATIC_0024Kinder_00242001_0024I = (byte)unchecked((uint)(_0024STATIC_0024Kinder_00242001_0024I + 1));
                            if (unchecked(_0024STATIC_0024Kinder_00242001_0024I) <= 99u)
                            {
                                goto IL_008e;
                            }
                            goto IL_0283;
                        IL_0283: // <========== 3
                            num = 28;
                            RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            RichTextBox1[0].SelectionIndent = unchecked(M_KonLen) * 5 + 10;
                            if (_0024STATIC_0024Kinder_00242001_0024I > 2)
                            {
                                RichTextBox1[0].SelectedText = "Kinder:  \n";
                                goto IL_0322;
                            }
                            if (_0024STATIC_0024Kinder_00242001_0024I == 2)
                            {
                                RichTextBox1[0].SelectedText = "Kind:  \n";
                            }
                            goto IL_0322;
                        IL_0322: // <========== 3
                            num = 36;
                            if (List1.Items.Count > 0)
                            {
                                b = (byte)(List1.Items.Count - 1);
                                _0024STATIC_0024Kinder_00242001_0024I = 0;
                                goto IL_0824;
                            }
                            goto IL_0830;
                        IL_04fb: // <========== 4
                            num = 55;
                            RichTextBox1[0].SelectedText = _0024STATIC_0024Kinder_00242001_0024Vor + _Modul1.Instance.Kont[3];
                            if (_Modul1.Instance.Kont[2].Trim() != "")
                            {
                                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[2];
                            }
                            goto IL_0566;
                        IL_0566:
                            num = 59;
                            if (_Modul1.Instance.Kont[4].Trim() != "")
                            {
                                RichTextBox1[0].SelectedText = " (" + _Modul1.Instance.Kont[4] + ")";
                            }
                            goto IL_05af;
                        IL_05af:
                            num = 62;
                            if (_Modul1.Instance.Kont[5].Trim() != "")
                            {
                                RichTextBox1[0].SelectedText = ", Sippe " + _Modul1.Instance.Kont[5];
                            }
                            goto IL_05f3;
                        IL_05f3:
                            num = 65;
                            if (_0024STATIC_0024Kinder_00242001_0024Vor == "                         ")
                            {
                                Berufe((EEventArt)300);
                                Berufe((EEventArt)301);
                                Listart = 0;
                                neb = false;
                                _Modul1.Instance.Datles3(Listart, default, default, ref neb);
                                if (_Modul1.Instance.Kont[11].Trim() != "")
                                {
                                    _Modul1.Instance.Kont[11] = _Modul1.Instance.DTxt[1] + " " + _Modul1.Instance.Kont[11].Trim();
                                }
                                goto IL_06a6;
                            }
                            goto IL_07f7;
                        IL_06a6:
                            num = 74;
                            if ((_Modul1.Instance.Kont[11].Trim() == "") & (_Modul1.Instance.Kont[12].Trim() != ""))
                            {
                                _Modul1.Instance.Kont[11] = _Modul1.Instance.DTxt[2] + " " + _Modul1.Instance.Kont[12].Trim();
                            }
                            goto IL_0710;
                        IL_0710:
                            num = 77;
                            if (_Modul1.Instance.Kont[13].Trim() != "")
                            {
                                _Modul1.Instance.Kont[13] = _Modul1.Instance.DTxt[3] + " " + _Modul1.Instance.Kont[13].Trim();
                            }
                            goto IL_0759;
                        IL_0759:
                            num = 80;
                            if ((_Modul1.Instance.Kont[13].Trim() == "") & (_Modul1.Instance.Kont[14].Trim() != ""))
                            {
                                _Modul1.Instance.Kont[13] = _Modul1.Instance.DTxt[4] + " " + _Modul1.Instance.Kont[14].Trim();
                            }
                            goto IL_07c3;
                        IL_07c3:
                            num = 83;
                            RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[11] + " " + _Modul1.Instance.Kont[13];
                            goto IL_07f7;
                        IL_07f7: // <========== 3
                            num = 85;
                            RichTextBox1[0].SelectedText = "\n";
                            _0024STATIC_0024Kinder_00242001_0024I = (byte)unchecked((uint)(_0024STATIC_0024Kinder_00242001_0024I + 1));
                            goto IL_0824;
                        IL_0824:
                            if (unchecked(_0024STATIC_0024Kinder_00242001_0024I <= (uint)b))
                            {
                                _Modul1.Instance.PersInArb = Conversions.ToInteger(Strings.Mid(List1.Items[_0024STATIC_0024Kinder_00242001_0024I].AsString(), 9, List1.Items[_0024STATIC_0024Kinder_00242001_0024I].AsString().Length));
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                Namenindex(_Modul1.Instance.Kont[0]);
                                DataModul.DT_KindAhnTable.Index = "Kind";
                                DataModul.DT_KindAhnTable.Seek("=", _Modul1.Instance.PersInArb);
                                _0024STATIC_0024Kinder_00242001_0024Vor = "                  ";
                                if (!DataModul.DT_KindAhnTable.NoMatch)
                                {
                                    if (DataModul.DT_KindAhnTable.Fields["Ahn"].AsInt() == 0)
                                    {
                                        goto IL_04fb;
                                    }
                                    _0024STATIC_0024Kinder_00242001_0024Vor = "Ahn".Right(10) + Strings.Right(new string(' ', 40) + DataModul.DT_KindAhnTable.Fields["Ahn"].AsDouble().AsString().Trim(), unchecked(M_KonLen) + 1) + " ";
                                    goto IL_04fb;
                                }
                                goto IL_04fb;
                            }
                            goto IL_0830;
                        IL_0830: // <========== 3
                            num = 88;
                            List1.Items.Clear();
                            goto end_IL_0000_2;
                        IL_08c2:
                            num4 = unchecked(num2 + 1);
                            goto IL_08c6;
                        IL_08c6:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 10:
                                    goto IL_008e;
                                case 21:
                                case 22:
                                    goto IL_01d8;
                                case 16:
                                case 24:
                                case 25:
                                case 26:
                                    goto IL_020f;
                                case 11:
                                case 28:
                                    goto IL_0283;
                                case 32:
                                case 35:
                                case 36:
                                    goto IL_0322;
                                case 47:
                                case 50:
                                case 51:
                                case 54:
                                case 55:
                                    goto IL_04fb;
                                case 58:
                                case 59:
                                    goto IL_0566;
                                case 61:
                                case 62:
                                    goto IL_05af;
                                case 64:
                                case 65:
                                    goto IL_05f3;
                                case 73:
                                case 74:
                                    goto IL_06a6;
                                case 76:
                                case 77:
                                    goto IL_0710;
                                case 79:
                                case 80:
                                    goto IL_0759;
                                case 82:
                                case 83:
                                    goto IL_07c3;
                                case 84:
                                case 85:
                                    goto IL_07f7;
                                case 87:
                                case 88:
                                    goto IL_0830;
                                case 3:
                                case 89:
                                case 91:
                                case 94:
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
                try0000_dispatch = 2660;
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
}
