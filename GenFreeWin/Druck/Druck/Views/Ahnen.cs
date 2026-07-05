using Druck.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Compatibility.VB6;
using Microsoft.VisualBasic.CompilerServices;
using System;
using GenFree;
using GenFree.Helper;
using GenFree.Interfaces.DB;
using GenFree.Data;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Collections.Generic;
using BaseLib.Helper;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.Data;

namespace Druck.Views;

[DesignerGenerated]
public partial class Ahnen : Form
{
    private IModul1 Modul1;


    private string Datu1;

    private bool Tz;

    private long Ahne;

    private int FamBeh;

    private string Pattext;

    private int PerSp1;

    private string TempFile;

    private bool Fb;

    private string[] Übersch;
    private int Modul1_J;
    private int Modul1_iPrivacy;
    private EEventArt Modul1_Beruf;
    private bool M1_Ki;
    private float M_Sgg = 1f;
    private string M_Namen;

    [DebuggerNonUserCode]
    public Ahnen()
    {
        base.FormClosing += Ahnen_FormClosing;
        base.Load += Ahnen_Load;
        Übersch = new string[3];
        this.Anz = new Microsoft.VisualBasic.Compatibility.VB6.RichTextBoxArray(this.components);
        this.Befehl = new ControlArray<Button>();
        this.Bezeichnung1 = new ControlArray<Label>();
        this.Command_Renamed = new ControlArray<Button>();
        this.Command2 = new ControlArray<Button>();
        this.Label1 = new ControlArray<Label>();

        InitializeComponent();

        this.Command_Renamed.SetIndex(this._Command_1, 1);
        this.Command_Renamed.SetIndex(this._Command_0, 0);
        this.Label1.SetIndex(this._Label1_5, 5);
        this.Label1.SetIndex(this._Label1_4, 4);
        this.Label1.SetIndex(this._Label1_3, 3);
        this.Label1.SetIndex(this._Label1_2, 2);
        this.Label1.SetIndex(this._Label1_1, 1);
        this.Label1.SetIndex(this._Label1_0, 0);
        this.Command2.SetIndex(this._Command2_3, 3);
        this.Command2.SetIndex(this._Command2_0, 0);
        this.Command2.SetIndex(this._Command2_1, 1);
        this.Command2.SetIndex(this._Command2_2, 2);
        this.Befehl.SetIndex(this._Befehl_7, 7);
        this.Anz.SetIndex(this._Anz_1, 1);
        this.Befehl.SetIndex(this._Befehl_6, 6);
        this.Befehl.SetIndex(this._Befehl_5, 5);
        this.Befehl.SetIndex(this._Befehl_4, 4);
        this.Befehl.SetIndex(this._Befehl_0, 0);
        this.Anz.SetIndex(this._Anz_0, 0);
        this.Befehl.SetIndex(this._Befehl_1, 1);
        this.Befehl.SetIndex(this._Befehl_2, 2);
        this.Befehl.SetIndex(this._Befehl_3, 3);
        this.Anz.SetIndex(this._Anz_2, 2);
        this.Anz.SetIndex(this._Anz_3, 3);
        this.Bezeichnung1.SetIndex(this._Bezeichnung1_2, 2);
        this.Bezeichnung1.SetIndex(this._Bezeichnung1_1, 1);
        this.Bezeichnung1.SetIndex(this._Bezeichnung1_0, 0);
        Befehl.AddClick(Befehl_Click);
        Command_Renamed.AddClick(Command_Renamed_Click);
        Command2.AddClick(Command2_Click);
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
    private void Befehl_Click(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        string right = default;
        string text = default;
        string text2 = default;
        string text3 = default;
        byte b = default;
        int recordCount = default;
        int num5 = default;
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
                    int index;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            index = Befehl.GetIndex((Button)eventSender);
                            goto IL_0016;
                        case 8523:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_1cc5;
                                    default:
                                        goto end_IL_0000;
                                }
                                int number = Information.Err().Number;
                                if (number == 55)
                                {
                                    FileSystem.FileClose();
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_1cc1;
                                }
                                if (number is 57 or 70)
                                {
                                    TempFile += "a";
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_1cc1;
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
                                goto IL_1cc1;
                            }
                        end_IL_0000:
                            break;
                        IL_0016:
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
                                    short b = Befehl_0_Click(ref right, ref text2, ref text3);
                                    break;
                                case 1:
                                    Befehl_1_Click();
                                    break;
                                case 2:
                                    Befehl_2_Click();
                                    break;
                                case 3:
                                    Befehl_3_Click();
                                    break;
                                case 4:
                                    Befehl_4_Click(out b, out num5);
                                    break;
                                case 5:
                                    Befehl_5_Click(ref right, ref text, ref text2);
                                    break;
                                case 6:
                                    Befehl_6_Click();
                                    break;
                                case 7:
                                    Befehl_7_Click(ref text2, ref recordCount, ref num5);
                                    break;
                                default:
                                    Interaction.MsgBox(index);
                                    break;
                            }
                            goto end_IL_0000_2;

                        IL_1cc1: // <========== 3
                                 // <========== 3
                            num4 = num2;
                            goto IL_1cc9;
                        IL_1cc5:
                            num4 = unchecked(num2 + 1);
                            goto IL_1cc9;
                        IL_1cc9:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 28:
                                case 29:
                                case 255:
                                case 256:
                                case 257:
                                case 9:
                                case 79:
                                case 90:
                                case 94:
                                case 95:
                                case 107:
                                case 110:
                                case 114:
                                case 117:
                                case 118:
                                case 119:
                                case 128:
                                case 176:
                                case 183:
                                case 259:
                                case 260:
                                case 263:
                                case 264:
                                case 266:
                                case 271:
                                case 272:
                                case 276:
                                case 277:
                                case 283:
                                case 284:
                                case 285:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 8523;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 13
                       // <========== 14
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private short Befehl_0_Click(ref string right, ref string text2, ref string text3)
    {
        checked
        {
            var b = (short)0;
            while (b <= 3u)
            {
                Anz[b].Visible = false;
                b++;
            }
            Anz[2].Visible = true;
            if (Anz[2].Text == "")
            {
                DataModul.DSB_NamIdxTable.Index = "Langi";
                Anz[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                Anz[2].SelectionAlignment = HorizontalAlignment.Center;
                Anz[2].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 1.3), FontStyle.Bold);
                Anz[2].SelectedText = "Namen-Index (Langform) " + Übersch[2] + " " + Übersch[0];
                Anz[2].SelectedText = Übersch[1] + "\n\n";
                Anz[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                Anz[2].SelectionAlignment = HorizontalAlignment.Left;
                Anz[2].SelectionIndent = 10;
                DataModul.DSB_NamIdxTable.Seek(">=", " ", " ", 0);
                M_Namen = "";
                while (!DataModul.DSB_NamIdxTable.EOF)
                {
                    Anz[2].SelectionIndent = 10;
                    if (M_Namen != DataModul.DSB_NamIdxTable.Fields["Name1"].AsString())
                    {
                        if (text3 != "")
                        {
                            Anz[2].SelectionIndent = 30;
                            Anz[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[2].SelectedText = text3 + "; " + Strings.Mid(text2, 2, text2.Length) + ".\n";
                            text2 = "";
                            right = "";
                            text3 = "";
                        }
                        Anz[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                        Anz[2].SelectionIndent = 20;
                        Anz[2].SelectedText = DataModul.DSB_NamIdxTable.Fields["Name1"].AsString() + '\n';
                        Anz[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
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
                        Anz[2].SelectionIndent = 30;
                        Anz[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        Anz[2].SelectedText = text3 + ": " + Strings.Mid(text2, 2, text2.Length) + ".\n";
                        text2 = "";
                        right = 0.AsString();
                        text3 = DataModul.DSB_NamIdxTable.Fields["Name"].AsString();
                    }
                    if (!Information.IsDBNull(DataModul.DSB_NamIdxTable.Fields["Nr"].Value))
                    {
                        if (DataModul.DSB_NamIdxTable.Fields["Nr"].AsInt() != right.AsInt())
                        {
                            text2 = text2 + ", " + DataModul.DSB_NamIdxTable.Fields["Nr"].AsString().Trim();
                            right = DataModul.DSB_NamIdxTable.Fields["Nr"].AsString();
                        }
                    }
                    DataModul.DSB_NamIdxTable.MoveNext();
                }
                if (text3 != "")
                {
                    Anz[2].SelectionIndent = 30;
                    Anz[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                    Anz[2].SelectedText = text3 + "; " + Strings.Mid(text2, 2, text2.Length) + ".\n";
                    text2 = "";
                    right = "";
                    text3 = "";
                }
            }
            TempFile = "Text3";
            Anz[2].SaveFile(Modul1.Verz1 + "TEMP\\" + TempFile + ".RTF", RichTextBoxStreamType.RichText);
            Anz[2].LoadFile(Modul1.Verz1 + "TEMP\\" + TempFile + ".RTF", RichTextBoxStreamType.RichText);
            return b;
        }
    }

    private int Befehl_5_Click(ref string right, ref string text, ref string text2)
    {
        checked
        {
            short b = 0;
            while (b <= 3u)
            {
                Anz[b].Visible = false;
                b++;
            }
            Anz[1].Text = "";
            Anz[1].Visible = true;
            if (Anz[1].Text == "")
            {
                DataModul.DSB_NamIdxTable.Index = "Kurzname";
                Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                Anz[1].SelectionAlignment = HorizontalAlignment.Center;
                Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 1.3), FontStyle.Bold);
                Anz[1].SelectedText = "Namen-Index (Kurzform) " + Übersch[2] + " " + Übersch[0];
                Anz[1].SelectedText = Übersch[1] + "\n\n";
                Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                Anz[1].SelectionAlignment = HorizontalAlignment.Left;
                Anz[1].SelectionIndent = 20;
                DataModul.DSB_NamIdxTable.Seek(">=", " ", 0);
                while (!DataModul.DSB_NamIdxTable.EOF)
                {
                    if (text != DataModul.DSB_NamIdxTable.Fields["Name1"].AsString())
                    {
                        if (text != "")
                        {
                            Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            Anz[1].SelectedText = text;
                            Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[1].SelectedText = text2 + "\n\n";
                            text2 = "";
                            right = "";
                        }
                        text = DataModul.DSB_NamIdxTable.Fields["Name1"].AsString();
                    }
                    if (!Information.IsDBNull(DataModul.DSB_NamIdxTable.Fields["Nr"].Value))
                    {
                        if (DataModul.DSB_NamIdxTable.Fields["Nr"].AsInt() != right.AsInt())
                        {
                            text2 = text2 + ", " + DataModul.DSB_NamIdxTable.Fields["Nr"].AsString().Trim();
                            right = DataModul.DSB_NamIdxTable.Fields["Nr"].AsString();
                        }
                    }
                    DataModul.DSB_NamIdxTable.MoveNext();
                }
                Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                Anz[1].SelectedText = text;
                Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                Anz[1].SelectedText = text2 + "\n\n";
                text2 = "";
                right = "";
            }
            Anz[1].SaveFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
            Anz[1].LoadFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
            return b;
        }
    }

    private int Befehl_1_Click()
    {
        checked
        {
            short b = 0;
            while (!Anz[b].Visible && unchecked(b) <= 2u)
            {
                b++;
            }
            TempFile = "Text2";
            Anz[b].SaveFile(Modul1.Verz1 + "TEMP\\" + TempFile + ".RTF", RichTextBoxStreamType.RichText);
            Anz[b].LoadFile(Modul1.Verz1 + "TEMP\\" + TempFile + ".RTF", RichTextBoxStreamType.RichText);
            Interaction.Shell(Modul1.Aus[7] + " " + Modul1.Verz1 + "TEMP\\" + TempFile + ".RTF", AppWinStyle.MaximizedFocus);
            return b;
        }
    }

    private void Befehl_2_Click()
    {
        checked
        {
            Close();
            MyProject.Forms.Druck.Show();
        }
    }

    private int Befehl_3_Click()
    {
        checked
        {
            short b = 0;
            while (!Anz[b].Visible
                && unchecked(b) <= 2u)
            {
                b++;
            }
            MyProject.Forms.Hinter.CommonDialog1Save.InitialDirectory = Modul1.GenFreeDir + "list\\";
            MyProject.Forms.Hinter.CommonDialog1Save.Filter = "Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF";
            MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex = 2;
            MyProject.Forms.Hinter.CommonDialog1Save.ShowDialog();
            if (MyProject.Forms.Hinter.CommonDialog1Save.FileName != "")
                switch (MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex)
                {
                    case 1:
                        Anz[b].SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);
                        break;
                    case 2:
                        Anz[b].SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.RichText);
                        break;
                    default:
                        break;
                }

            return b;
        }
    }

    private void Befehl_4_Click(out byte b, out int num5)
    {
        checked
        {
            num5 = 0;
            while (num5 <= 6)
            {
                Befehl[(short)num5].Visible = true;
                num5++;
            }
            b = 0;
            while (unchecked((byte)unchecked((uint)(b + 1))) <= 3u)
            {
                Anz[b].Visible = false;
            }
            Anz[0].Visible = true;
        }
    }

    private void Befehl_6_Click()
    {
        checked
        {
            short b = 0;
            while (unchecked((byte)unchecked((uint)(b + 1))) <= 3u)
            {
                Anz[b].Visible = false;
            }
            Anz[3].Visible = true;
            Frame2.Visible = true;
        }
    }

    private byte Befehl_7_Click(ref string text2, ref int recordCount, ref int num5)
    {
        checked
        {
            byte b;
            Anz[1].Text = "";
            b = 0;
            while (unchecked((byte)unchecked((uint)(b + 1))) <= 3u)
            {
                Anz[b].Visible = false;
            }
            Anz[1].Visible = true;
            Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            Anz[1].SelectionAlignment = HorizontalAlignment.Center;
            Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 1.3), FontStyle.Bold);
            Anz[1].SelectedText = "Quellenanhang " + Übersch[2] + " " + Übersch[0];
            Anz[1].SelectedText = Übersch[1] + "\n\n";
            Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            Anz[1].SelectionAlignment = HorizontalAlignment.Left;
            if (DataModul.DSB_QuellIdxTable.RecordCount <= 0)
            {
                goto end_IL_0000_2;
            }
            DataModul.DSB_QuellIdxTable.Index = "Quelle";
            DataModul.DSB_QuellIdxTable.MoveFirst();
            recordCount = DataModul.DSB_QuellIdxTable.RecordCount;
            num5 = 1;
            while (num5 <= recordCount)
            {
                text2 = DataModul.DSB_QuellIdxTable.Fields["Nr"].AsString();
                if (text2.AsDouble() != Modul1.AltNr)
                {
                    Modul1.AltNr = text2.AsInt();
                    DataModul.DB_QuTable.Index = "NR";
                    DataModul.DB_QuTable.Seek("=", text2);
                    Anz[1].SelectionIndent = 4;
                    Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                    Anz[1].SelectedText = DataModul.DB_QuTable.Fields[QuFields._4].AsString() + '\n';
                    Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                    Anz[1].SelectionIndent = 20;
                    if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._2].AsString().Trim(), "", TextCompare: false) != 0)
                    {
                        Anz[1].SelectedText = ("Titel: " + DataModul.DB_QuTable.Fields[QuFields._2].Value + '\n').AsString();
                    }
                    if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._5].AsString().Trim(), "", TextCompare: false) != 0)
                    {
                        Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        Anz[1].SelectedText = ("Autor: " + DataModul.DB_QuTable.Fields[QuFields._5].Value + '\n').AsString();
                    }
                    Modul1.UbgT = Module2.Repoles(DataModul.DB_QuTable.Fields[QuFields._1].AsInt());
                    if (Modul1.UbgT.Trim() != "")
                    {
                        Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        Anz[1].SelectedText = Modul1.UbgT;
                    }
                    Modul1.UbgT = "";
                    if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._7].AsString().Trim(), "", TextCompare: false) != 0)
                    {
                        Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        Anz[1].SelectedText = ("Herausgeber: " + DataModul.DB_QuTable.Fields[QuFields._7].Value + '\n').AsString();
                    }
                    if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._8].AsString().Trim(), "", TextCompare: false) != 0)
                    {
                        Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        Anz[1].SelectedText = ("Erscheinungsort: " + DataModul.DB_QuTable.Fields[QuFields._8].Value + '\n').AsString();
                    }
                    if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._9].AsString().Trim(), "", TextCompare: false) != 0)
                    {
                        Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        Anz[1].SelectedText = ("Erscheinungsdatum: " + DataModul.DB_QuTable.Fields[QuFields._9].Value + '\n').AsString();
                    }
                    if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._10].AsString().Trim(), "", TextCompare: false) != 0)
                    {
                        Anz[1].SelectedText = ("in: " + DataModul.DB_QuTable.Fields[QuFields._10].Value + '\n').AsString();
                    }
                    if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._11].AsString().Trim(), "", TextCompare: false) != 0)
                    {
                        Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        Anz[1].SelectedText = ("Jahrgang: " + DataModul.DB_QuTable.Fields[QuFields._11].Value + " ").AsString();
                    }
                    if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._12].AsString().Trim(), "", TextCompare: false) != 0)
                    {
                        Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        Anz[1].SelectedText = ("Nr.: " + DataModul.DB_QuTable.Fields[QuFields._12].AsString());
                    }
                    if ((Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._11].AsString().Trim(), "", TextCompare: false) != 0) | (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._12].AsString().Trim(), "", TextCompare: false) != 0))
                    {
                        Anz[1].SelectedText = "\n";
                    }
                    if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._13].AsString().Trim(), "", TextCompare: false) != 0)
                    {
                        Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        Anz[1].SelectionIndent = 30;
                        Anz[1].SelectedText = ("Bemerkungen: " + DataModul.DB_QuTable.Fields[QuFields._13].Value + '\n').AsString();
                    }
                }
                DataModul.DSB_QuellIdxTable.MoveNext();
                num5++;
            }

            return b;
        }
    }

    private void Command_Renamed_Click(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        string text = default;
        short num5 = default;
        short num8 = default;
        string text3 = default;
        bool flag = default;
        string left = default;
        int num9 = default;
        byte b = default;
        short num10 = default;
        short num11 = default;
        bool flag2 = default;
        int num12 = default;
        string text6 = default;
        short num13 = default;
        byte b3 = default;
        int num16 = default;
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
                    string text4;
                    string text5;
                    object obj;
                    int FamPer;
                    string LD;
                    short Listart;
                    bool neb;
                    int index;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            index = Command_Renamed.GetIndex((Button)eventSender);
                            goto IL_0016;
                        case 17215:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_3971;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 3021)
                                {
                                    if (Modul1.Druck_Tast > 0)
                                    {
                                        Kinder(text);
                                    }
                                    goto IL_3561;
                                }
                                if (Information.Err().Number == 5)
                                {
                                    goto end_IL_0000_2;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString() + "Command_Renamed", MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_3975;
                            }
                        end_IL_0000:
                            break;
                        IL_0016:
                            num = 2;
                            text3 = "";
                            text = "";
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            flag = false;
                            left = "";
                            num9 = 0;
                            Frame1.Visible = false;
                            switch (index)
                            {
                                case 0:
                                    break;
                                case 1:
                                    goto IL_0095;
                                default:
                                    goto IL_3516;
                            }
                            Close();
                            MyProject.Forms.Druck.Show();
                            goto end_IL_0000_2;
                        IL_0095:
                            num = 18;
                            Befehl[0].Enabled = false;
                            Befehl[1].Enabled = false;
                            Befehl[3].Enabled = false;
                            Befehl[4].Enabled = false;
                            Befehl[5].Enabled = false;
                            Befehl[6].Enabled = false;
                            Befehl[7].Enabled = false;
                            Anz[0].Enabled = false;
                            MyProject.Forms.AW.Close();
                            MyProject.Forms.AW.Kontroll[75].Visible = false;
                            MyProject.Forms.AW.Kontroll[65].Visible = false;
                            MyProject.Forms.AW.Kontroll[89].Visible = true;
                            MyProject.Forms.AW.Kontroll[97].Visible = true;
                            MyProject.Forms.AW.Kontroll[91].Visible = true;
                            MyProject.Forms.AW.Kontroll[92].Visible = true;
                            MyProject.Forms.AW.Kontroll[93].Visible = true;
                            MyProject.Forms.AW.Kontroll[63].Visible = false;
                            MyProject.Forms.AW.Kontroll[95].Visible = true;
                            num8 = 77;
                            while ((short)unchecked(num8 + 1) <= 80)
                            {
                                MyProject.Forms.AW.Kontroll[num8].Visible = false;
                            }
                            num8 = 10;
                            while ((short)unchecked(num8 + 1) <= 12)
                            {
                                MyProject.Forms.AW.Kontroll[num8].Visible = false;
                            }
                            num8 = 57;
                            while ((short)unchecked(num8 + 1) <= 61)
                            {
                                MyProject.Forms.AW.Kontroll[num8].Visible = false;
                            }
                            MyProject.Forms.AW.ShowDialog("Ahn");
                            Modul1.DAus[63] = "0";
                            Application.DoEvents();
                            if (Strings.Mid(Label1[3].Text, 16, 10).AsDouble() < 1.0)
                            {
                                Close();
                                MyProject.Forms.Druck.Show();
                                goto end_IL_0000_2;
                            }
                            num8 = 0;
                            while ((short)unchecked(num8 + 1) <= 3)
                            {
                                Anz[num8].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            }
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].Visible = true;
                            DataModul.NB_AhnTable.Index = "Ahnen";
                            DataModul.NB_AhnTable.MoveLast();
                            Modul1.PrintDat.KonLen = (byte)Strings.Len(DataModul.NB_AhnTable.Fields["Ahn"].AsString().Trim());
                            Anz[0].SelectionAlignment = HorizontalAlignment.Center;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 1.5), FontStyle.Regular);
                            if (Modul1.Druck_Tast == 0)
                            {
                                Anz[0].SelectedText = Modul1.IText[80];
                                Übersch[2] = "zur " + Modul1.IText[80];
                                goto IL_059b;
                            }
                            if (Modul1.Druck_Tast == 1)
                            {
                                Anz[0].SelectedText = "Ahnenliste mit Kindern für:";
                                Übersch[2] = "zur Ahnenliste mit Kindern für:";
                                goto IL_059b;
                            }
                            if (Modul1.Druck_Tast == 2)
                            {
                                Anz[0].SelectedText = "Spitzenahnen von: ";
                                Übersch[2] = "zur Spitzenahnenliste für:";
                            }
                            goto IL_059b;
                        IL_059b: // <========== 4
                            num = 76;
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            if (Modul1.Kont[1].Trim() != "")
                            {
                                Modul1.Kont[1] = Modul1.Kont[1].Trim() + " ";
                            }
                            goto IL_05e3;
                        IL_05e3:
                            num = 80;
                            if (Modul1.DAus[89] == "1")
                            {
                                Bezeichnung1[0].Text = "Ahnenliste " + DataModul.NB_AhnTable.Fields["Gen"].AsString() + " Generationen für " + Modul1.Person.Prae + " " + Modul1.Kont[3] + " " + Modul1.Kont[1] + Modul1.Kont[0].ToUpper();
                            }
                            else
                            {

                                Bezeichnung1[0].Text = "Ahnenliste " + DataModul.NB_AhnTable.Fields["Gen"].AsString() + " Generationen für " + Modul1.Person.Prae + " " + Modul1.Kont[3] + " " + Modul1.Kont[1] + Modul1.Kont[0].ToUpper();
                            }
                            goto IL_074b;
                        IL_074b: // <========== 3
                            num = 86;
                            Bezeichnung1[0].Refresh();
                            Anz[0].SelectedText = " " + Modul1.Person.Prae + Modul1.Kont[3].TrimEnd() + " ";
                            if (Modul1.Kont[1].Trim() != "")
                            {
                                Anz[0].SelectedText = Modul1.Kont[1].Trim() + " ";
                            }
                            goto IL_07e0;
                        IL_07e0:
                            num = 91;
                            if (Modul1.DAus[89] == "1")
                            {
                                M_Namen = Modul1.Kont[0].ToUpper();
                            }
                            else
                            {

                                M_Namen = Modul1.Kont[0];
                            }
                            goto IL_0825;
                        IL_0825: // <========== 3
                            num = 97;
                            Übersch[0] = " " + Modul1.Person.Prae + Modul1.Kont[3].TrimEnd() + " " + Modul1.Kont[0] + "\n";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 1.5), FontStyle.Bold);
                            Anz[0].SelectedText = M_Namen + "\n";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectedText = "Erstellt am " + DateTime.Today.AsString();
                            Anz[0].SelectedText = " von " + Modul1.User.Owner.Trim() + " mit Gen_Plus aus Mandant: " + Modul1.Verz + "\n";
                            Übersch[1] = "Erstellt am " + DateTime.Today.AsString() + " von " + Modul1.User.Owner.Trim() + " mit Gen_Plus aus Mandant: " + Modul1.Verz + "\n";
                            Anz[0].SelectedText = "\n";
                            Anz[0].SelectionAlignment = HorizontalAlignment.Left;
                            DataModul.NB_AhnTable.Index = "Ahnen";
                            DataModul.NB_AhnTable.MoveFirst();
                            if (Modul1.Druck_Tast == 2)
                            {
                                DataModul.NB_AhnTable.Index = "Spitz";
                                DataModul.NB_AhnTable.Seek(">=", "0", 1);
                                if (DataModul.NB_AhnTable.NoMatch)
                                {
                                    Interaction.MsgBox("In der vorhandenen Berechnung sind keine Spitzenahnen enthalten.");
                                    Close();
                                    MyProject.Forms.Druck.Show();
                                    goto end_IL_0000_2;
                                }
                                text4 = Strings.Right(new string(' ', 40) + Conversion.Str(DataModul.NB_AhnTable.Fields["Ahn"].AsInt() + 1), 40);
                            }
                            goto IL_0b4a;
                        IL_0b4a:
                            num = 119;
                            b = 1;
                            Modul1.FamInArb = 0;
                            goto IL_0b5b;
                        IL_0b5b: // <========== 3
                            num = 121;
                            lErl = 1;
                            obj = 0;
                            Application.DoEvents();
                            text5 = Strings.Right("000000000000000000000000000000000" + DataModul.NB_AhnTable.Fields["Ahn"].AsString().Trim(), 40);
                            byte b2 = (Byte)DataModul.NB_AhnTable.Fields["Gen"].AsInt();
                            b = (byte)(unchecked(b) + 1);
                            if (b == 12)
                            {
                                b = 10;
                            }
                            goto IL_0bf4;
                        IL_0bf4:
                            num = 130;
                            int selectionLength;
                            string text2;
                            if (Anz[0].SelectionStart > 50000)
                            {
                                Anz[0].SelectedText = "\n";
                                Anz[0].SelectionIndent = 0;
                                Anz[0].SelectedText = "\n";
                                selectionLength = Anz[0].SelectionStart;
                                Anz[0].SelectionStart = 0;
                                Anz[0].SelectionLength = selectionLength;
                                num5 = (short)(num5 + 1);
                                FileSystem.FileClose(99);
                                text2 = Modul1.GenFreeDir + "Temp\\sptext" + num5.AsString().Trim() + ".rtf";
                                FileSystem.FileOpen(99, text2, OpenMode.Output);
                                FileSystem.PrintLine(99, Anz[0].SelectedRtf);
                                FileSystem.FileClose(99);
                                Anz[0].Text = "";
                                Anz[0].SelectionAlignment = HorizontalAlignment.Left;
                            }
                            goto IL_0da1;
                        IL_0da1:
                            num = 146;
                            if (text.AsDouble() == 1.0)
                            {
                                Anz[0].SelectedText = "\n\n";
                                if (Modul1.PrintDat.BemZahl > 0)
                                {
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                                    Anz[0].SelectedText = "\nQuellen:";
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    num10 = (short)Modul1.PrintDat.BemZahl;
                                    num11 = 1;
                                    goto IL_0fac;
                                }
                            }
                            goto IL_0fdf;
                        IL_0ece:
                            num = 156;
                            Modul1.UbgT1 = Modul1.PrintDat.KontBem[num11];
                            Modul1.UbgT1 = Modul1.PrintDat.Retweg(Modul1.UbgT1);
                            Modul1.UbgT1 = Modul1.UbgT1.Left(Modul1.UbgT1.Length - 1) + Modul1.UbgT1.Right(1).Replace("\n", "");
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectedText = " " + Modul1.UbgT1;
                            Modul1.UbgT1 = "";
                            num11 = (short)unchecked(num11 + 1);
                            goto IL_0fac;
                        IL_0fac:
                            if (num11 <= num10)
                            {
                                if ((Modul1.DAus[71] == "1") & (num11 <= Modul1.PrintDat.BemZahl))
                                {
                                    Anz[0].SelectedText = "\n";
                                }
                                goto IL_0ece;
                            }
                            Modul1.PrintDat.BemZahl = 0;
                            Anz[0].SelectedText = "\n";
                            goto IL_0fdf;
                        IL_0fdf: // <========== 3
                            num = 167;
                            Fb = false;
                            if (!flag2)
                            {
                                if (num12 != 0)
                                {
                                    Modul1.FamInArb = num12;
                                }
                                goto IL_1015;
                            }
                            flag2 = false;
                            goto IL_1259;
                        IL_1015:
                            num = 172;
                            Modul1.Famles();
                            if (DataModul.NB_AhnTable.Fields["Ehe"].AsInt() != 0)
                            {
                                if ((b > 3) | ((Modul1.FamInArb > 0) & (Modul1.Druck_Tast == 2)))
                                {
                                    if (Modul1.Family.Frau == 0)
                                    {
                                        FamBeh = Modul1.FamInArb;
                                        if (Modul1.Druck_Tast > 0)
                                        {
                                            Kinder(text);
                                        }
                                        goto IL_10b3;
                                    }
                                    goto IL_10f0;
                                }
                                goto IL_1237;
                            }
                            goto IL_1259;
                        IL_10b3:
                            num = 180;
                            T2(text.AsLong());
                            Zeilenumbruch(Anz[0]);
                            Anz[0].SelectedText = "\n\n\n";
                            goto IL_10f0;
                        IL_10f0: // <========== 3
                            num = 184;
                            if (text.AsDouble() / 2.0 != Conversion.Int(text.AsDouble() / 2.0))
                            {
                                FamBeh = Modul1.FamInArb;
                                if (Modul1.Druck_Tast > 0)
                                {
                                    Kinder(text);
                                }
                                goto IL_1150;
                            }
                            if ((DataModul.NB_AhnTable.Fields["Ahn"].Value.AsDouble() - 1.0 != text.AsDouble()) & (Modul1.Family.Frau != 0))
                            {
                                if (Modul1.Druck_Tast == 2)
                                {
                                    if (Modul1.Druck_Tast > 0)
                                    {
                                        Kinder(text);
                                    }
                                    goto IL_1224;
                                }
                            }
                            goto IL_1237;
                        IL_1150:
                            num = 189;
                            Modul1.FamInArb = FamBeh;
                            T2(text.AsLong());
                            Zeilenumbruch(Anz[0]);
                            Anz[0].SelectedText = "\n\n\n";
                            goto IL_1237;
                        IL_1224:
                            num = 200;
                            T2(text.AsLong());
                            goto IL_1237;
                        IL_1237: // <========== 5
                            num = 205;
                            obj = 1;
                            goto IL_1259;
                        IL_1259: // <========== 4
                            num = 211;
                            if (!DataModul.NB_AhnTable.NoMatch)
                            {
                                text3 += ">";
                                Bezeichnung1[2].Text = text3;
                                Bezeichnung1[2].Refresh();
                                if (text3.Length == 50)
                                {
                                    text3 = "";
                                }
                                goto IL_12d8;
                            }
                            goto IL_34ea;
                        IL_12d8:
                            num = 220;
                            if (Modul1.Schalt == 0)
                            {
                                if (Conversions.ToDouble(DataModul.NB_AhnTable.Fields["Ahn"].AsString().Trim()) == text.AsDouble() + 1.0)
                                {
                                    if ((left == "M") & (text.AsDouble() != 1.0))
                                    {
                                        text6 = "";
                                        Modul1.eLKennz = ELinkKennz.lkFather;
                                        aiFams = DataModul.Link.GetPersonFams(Modul1.PersInArb, Modul1.eLKennz);
                                        if (Modul1.UbgT.Length != 10)
                                        {
                                            text6 = Modul1.UbgT;
                                            Modul1.PersInArb = DataModul.NB_AhnTable.Fields["PerNr"].AsInt();
                                            Modul1.eLKennz = ELinkKennz.lkMother;
                                            aiFams = DataModul.Link.GetPersonFams(Modul1.PersInArb, Modul1.eLKennz);
                                            if (Modul1.UbgT.Length > 10)
                                            {
                                                num13 = (short)Math.Round(text6.Length / 10.0);
                                                num8 = 1;
                                                goto IL_14f8;
                                            }
                                            Modul1.FamInArb = (int)Math.Round(Modul1.UbgT.AsDouble());

                                        }
                                        else
                                        {
                                            Modul1.FamInArb = (int)Math.Round(Modul1.UbgT.AsDouble());
                                        }
                                        goto IL_154b;
                                    }
                                    goto IL_191c;
                                }
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                if (Modul1.Druck_Tast == 2)
                                {
                                    if (Anz[0].SelectionStart > 0)
                                    {
                                        if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart - 2, 2) != "\n\n")
                                        {
                                            Anz[0].SelectedText = Modul1.Kont[30] + "\n";

                                        }
                                    }

                                }
                            }
                            goto IL_191c;
                        IL_14d9: // <========== 3
                            num = 241;
                            if (Modul1.Schalt != 1)
                            {
                                num8 = (short)unchecked(num8 + 1);
                                goto IL_14f8;
                            }
                            goto IL_154b;
                        IL_14f8:
                            if (num8 <= num13)
                            {
                                string text7 = Strings.Mid(text6, num8 * 10 - 9, 10);
                                short num14 = (short)Math.Round(Modul1.UbgT.Length / 10.0);
                                short num15 = 1;
                                while (num15 <= num14)
                                {
                                    if (text7 == Strings.Mid(Modul1.UbgT, num15 * 10 - 9, 10))
                                    {
                                        Modul1.FamInArb = text7.AsInt();
                                        Modul1.Schalt = 1;
                                        goto IL_14d9;
                                    }
                                    num15 = (short)unchecked(num15 + 1);
                                }
                                goto IL_14d9;
                            }
                            goto IL_154b;
                        IL_154b: // <========== 5
                            num = 253;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            M1_Ki = false;
                            DataModul.DB_FamilyTable.Index = "Fam";
                            DataModul.DB_FamilyTable.Seek("=", Modul1.FamInArb);
                            if (DataModul.DB_FamilyTable.Fields[FamilyFields.Aeb].AsInt() == -1)
                            {
                                if (unchecked(((0 - (M1_Ki ? 1 : 0) == 0) & (Modul1.DAus[82].AsDouble() == 1.0)) | ((0 - (M1_Ki ? 1 : 0) == 1) & (Modul1.DAus[83].AsDouble() == 1.0))))
                                {
                                    Anz[0].SelectedText = " " + Modul1.DTxt[12] + " mit\n";
                                }
                                goto IL_180a;
                            }
                            FamBeh = Modul1.FamInArb;
                            Heidat();
                            leerweg();
                            if ((Modul1.DAus[115] == "1") | (Modul1.DAus[116] == "1"))
                            {
                                Module2.Bildaus("F", "Ahnen");
                            }
                            goto IL_172e;
                        IL_172e:
                            num = 269;
                            if (Modul1.DAus[88] == "1")
                            {
                                Bild("F", FamBeh);
                                Anz[0].SelectedText = "\n";
                            }
                            goto IL_1780;
                        IL_1780:
                            num = 273;
                            if (Modul1.DAus[106] == "1")
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            goto IL_17ba;
                        IL_17ba:
                            num = 276;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectedText = " mit\n";
                            goto IL_180a;
                        IL_180a: // <========== 3
                            num = 278;
                            lErl = 6;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            goto IL_191c;
                        IL_191c: // <========== 6
                            num = 293;
                            if (DataModul.NB_AhnTable.Fields["Ehe"].AsInt() != 0)
                            {
                                num12 = DataModul.NB_AhnTable.Fields["Ehe"].AsInt();
                            }
                            goto IL_1971;
                        IL_1971:
                            num = 296;
                            Modul1.Schalt = 0;
                            text = DataModul.NB_AhnTable.Fields["Ahn"].AsString().Trim();
                            b2 = (Byte)DataModul.NB_AhnTable.Fields["Gen"].AsInt();
                            Modul1.PersInArb = DataModul.NB_AhnTable.Fields["PerNr"].AsInt();
                            if (Modul1.PersInArb != 0)
                            {
                                if (Modul1.PersInArb == 0)
                                {
                                    Modul1.FamInArb = DataModul.NB_AhnTable.Fields["Ehe"].AsInt();
                                    goto IL_34ea;
                                }
                                DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb);
                                left = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString();
                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                                if (unchecked(b2 > (uint)b3 || b2 == 0))
                                {
                                    Zeilenumbruch(Anz[0]);
                                    Anz[0].SelectedText = "\n\n\n";
                                    Bezeichnung1[1].Text = "Bearbeite Generation " + DataModul.NB_AhnTable.Fields["Gen"].AsString();
                                    Bezeichnung1[1].Refresh();
                                    Anz[0].SelectionAlignment = HorizontalAlignment.Left;
                                    string selectedText = "Generation " + b2.AsString() + "\n";
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() + 5.0), FontStyle.Underline);
                                    Anz[0].SelectionIndent = 0;
                                    Anz[0].SelectedText = selectedText;
                                    Anz[0].SelectedText = "\n";
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    b3 = (Byte)DataModul.NB_AhnTable.Fields["Gen"].AsInt();
                                }
                                goto IL_1cd7;
                            }
                            goto IL_34ea;
                        IL_1cd7:
                            num = 325;
                            if (DataModul.NB_AhnTable.Fields["Weiter"].AsString() != "0")
                            {
                                text = DataModul.NB_AhnTable.Fields["Ahn"].AsString();
                                DataModul.NB_AhnTable.Index = "PerNr";
                                DataModul.NB_AhnTable.Seek("=", Modul1.PersInArb);
                                goto IL_1d9f;
                            }
                            goto IL_1f7c;
                        IL_1d9f: // <========== 3
                            num = 329;
                            lErl = 2;
                            if (!DataModul.NB_AhnTable.NoMatch)
                            {
                                if (DataModul.NB_AhnTable.Fields["PerNr"].AsInt() != Modul1.PersInArb)
                                {
                                    DataModul.NB_AhnTable.Index = "Ahnen";
                                    DataModul.NB_AhnTable.Seek("=", text);
                                    goto IL_1f7c;
                                }
                                List1.Items.Add(Strings.Right("               " + DataModul.NB_AhnTable.Fields["Ahn"].AsString().Trim(), 15) + Strings.Right("   " + DataModul.NB_AhnTable.Fields["Gen"].AsString(), 3));
                                DataModul.NB_AhnTable.MoveNext();
                                if (!DataModul.NB_AhnTable.EOF)
                                {
                                    goto IL_1d9f;
                                }
                                DataModul.NB_AhnTable.Index = "Ahnen";
                                DataModul.NB_AhnTable.Seek("=", text);
                            }
                            goto IL_1f7c;
                        IL_1f7c: // <========== 4
                            num = 346;
                            lErl = 3;
                            Ahne = DataModul.NB_AhnTable.Fields["Ahn"].AsLong();
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectionIndent = 0;
                            if ((List1.Items.Count == 0) | (Operators.CompareString(Strings.Trim(List1.Items[0].AsString().Left(15)), Ahne.AsString().Trim(), TextCompare: false) == 0) | (Conversion.Val(Strings.Trim(List1.Items[0].AsString().Left(15))) + 1.0 == Ahne.AsString().Trim().AsDouble()) | (Modul1.DAus[95].AsDouble() == 1.0))
                            {
                                flag = false;
                                if (List1.Items.Count > 0)
                                {
                                    if (Modul1.DAus[97].AsDouble() == 0.0)
                                    {
                                        Anz[0].SelectedText = DataModul.NB_AhnTable.Fields["Ahn"].AsDouble().AsString() + " = ";
                                        if (Modul1.DAus[114].AsDouble() != 1.0)
                                        {
                                            if (Operators.CompareString(Strings.Trim(List1.Items[0].AsString().Left(15)), Ahne.AsString().Trim(), TextCompare: false) == 0)
                                            {
                                                Anz[0].SelectedText = Strings.Trim(List1.Items[1].AsString());
                                            }
                                            else
                                            {

                                                Anz[0].SelectedText = Strings.Trim(List1.Items[0].AsString());
                                            }
                                            goto IL_21f3;
                                        }
                                        goto IL_2315;
                                    }
                                    Anz[0].SelectedText = DataModul.NB_AhnTable.Fields["Ahn"].AsDouble().AsString() + " >>";
                                    goto IL_227d;
                                }
                                Anz[0].SelectedText = DataModul.NB_AhnTable.Fields["Ahn"].AsDouble().AsString().Trim();
                                goto IL_2315;
                            }
                            flag = true;
                            if (List1.Items.Count > 0)
                            {
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                Ahne = DataModul.NB_AhnTable.Fields["Ahn"].AsLong();
                                if (Modul1.DAus[97].AsDouble() == 0.0)
                                {
                                    Anz[0].SelectedText = Ahne.AsString() + " = ";
                                    Modul1.Schalt = 1;
                                    if (Operators.CompareString(Strings.Trim(List1.Items[0].AsString().Left(15)), Ahne.AsString().Trim(), TextCompare: false) != 0)
                                    {
                                        Anz[0].SelectedText = Strings.Trim(List1.Items[0].AsString().Left(15)) + "; " + Strings.Trim(Strings.Mid(List1.Items[0].AsString(), 16, 3));
                                        goto IL_3118;
                                    }
                                    Anz[0].SelectedText = Strings.Trim(List1.Items[1].AsString().Left(15));
                                }
                                goto IL_3118;
                            }
                            Anz[0].SelectedText = Strings.Right(new string(' ', 40) + DataModul.NB_AhnTable.Fields["Ahn"].AsDouble().AsString().Trim(), Modul1.PrintDat.KonLen);
                            goto IL_326e;
                        IL_21f3: // <========== 3
                            num = 364;
                            if (List1.Items.Count > 2)
                            {
                                Anz[0].SelectedText = " >> ";
                            }
                            goto IL_227d;
                        IL_227d: // <========== 3
                            num = 371;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            List1.Items.Clear();
                            goto IL_2315;
                        IL_2315: // <========== 4
                            num = 377;
                            if (Modul1.DAus[114].AsDouble() == 1.0)
                            {
                                if (DataModul.NB_AhnTable.Fields["weiter"].AsInt() > 0)
                                {
                                    num16 = 0;
                                    DataModul.NB_AhnTable.Index = "Implex";
                                    DataModul.NB_AhnTable.Seek("=", Modul1.PersInArb);
                                    goto IL_247b;
                                }
                            }
                            goto IL_2545;
                        IL_247b: // <========== 3
                            num = 383;
                            if (!DataModul.NB_AhnTable.EOF)
                            {
                                if (!(DataModul.NB_AhnTable.Fields["PerNr"].AsInt() != Modul1.PersInArb))
                                {
                                    Anz[0].SelectedText = DataModul.NB_AhnTable.Fields["Ahn"].AsDouble().AsString();
                                    num16++;
                                    DataModul.NB_AhnTable.MoveNext();
                                    goto IL_247b;
                                }
                            }
                            goto IL_2491;
                        IL_2491:
                            num = 391;
                            Anz[0].SelectedText = " *******" + num16.AsString() + " fach Ahne********** ";
                            DataModul.NB_AhnTable.Index = "Ahnen";
                            DataModul.NB_AhnTable.Seek("=", text);
                            if (num16 > num9)
                            {
                                num9 = num16;
                            }
                            goto IL_2545;
                        IL_2545: // <========== 3
                            num = 399;
                            Modul1.PrintDat.Namenindex(Ahne);
                            if (Modul1.DAus[76] == "1")
                            {
                                Anz[0].SelectedText = " [" + Modul1.PersInArb.AsString().Trim() + "]";
                            }
                            goto IL_25af;
                        IL_25af:
                            num = 403;
                            Anz[0].SelectionHangingIndent = 15;
                            Anz[0].SelectedText = " " + Modul1.Person.Prae + Modul1.Kont[3] + " ";
                            if (Modul1.Kont[1] != "")
                            {
                                Anz[0].SelectedText = Modul1.Kont[1] + " ";
                            }
                            goto IL_2643;
                        IL_2643:
                            num = 408;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            if (Modul1.DAus[89] == "1")
                            {
                                Anz[0].SelectedText = Modul1.Kont[0].ToUpper();
                            }
                            else
                            {

                                Anz[0].SelectedText = Modul1.Kont[0];
                            }
                            goto IL_26df;
                        IL_26df: // <========== 3
                            num = 415;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            if (Modul1.Kont[2].Trim() != "")
                            {
                                Anz[0].SelectedText = " " + Modul1.Kont[2].TrimEnd();
                            }
                            goto IL_2761;
                        IL_2761:
                            num = 419;
                            if (Modul1.Kont[5].Trim() != "")
                            {
                                Anz[0].SelectedText = ", Sippe " + Modul1.Kont[5].TrimEnd();
                            }
                            goto IL_27b0;
                        IL_27b0:
                            num = 422;
                            if (Modul1.DAus[89] == "1")
                            {
                                if (Modul1.Kont[4].Trim() != "")
                                {
                                    Anz[0].SelectedText = " (" + Modul1.Kont[4].ToUpper() + ")";
                                }
                            }
                            else
                            if (Modul1.Kont[4].Trim() != "")
                            {
                                Anz[0].SelectedText = " (" + Modul1.Kont[4] + ")";
                            }
                            goto IL_2879;
                        IL_2879: // <========== 3
                            num = 432;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Modul1.PerSatzLes(Modul1.PersInArb);
                            Anz[0].SelectionCharOffset = 0;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            if (Modul1.DAus[62] == "1")
                            {
                                FamPer = 1;
                                Modul1.PrintDat.PerQu(ref FamPer);
                            }
                            if (Modul1.Kont[30] != "")
                            {
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                                Anz[0].SelectedText = Modul1.Kont[30];
                                Anz[0].SelectionCharOffset = 0;
                            }
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields["religi"].Value) && Operators.CompareString(DataModul.DB_PersonTable.Fields["religi"].AsString().Trim(), "", TextCompare: false) != 0)
                            {
                                Modul1.Ubg = DataModul.DB_PersonTable.Fields["religi"].AsInt();
                                LD = "";
                                Modul1.UbgT = DataModul.TextLese1(Modul1.Ubg);
                                if (Modul1.UbgT.Trim() != "")
                                {
                                    Anz[0].SelectedText = " (" + Modul1.UbgT + ") ";
                                }

                            }
                            goto IL_2b17;
                        IL_2b17: // <========== 3
                            num = 455;
                            if ((Modul1.DAus[115] == "1") | (Modul1.DAus[116] == "1"))
                            {
                                Module2.Bildaus("P", "Ahnen");
                            }
                            goto IL_2b63;
                        IL_2b63:
                            num = 458;
                            if (Modul1.DAus[88] == "1")
                            {
                                Bild("P", Modul1.PersInArb);
                            }
                            goto IL_2b97;
                        IL_2b97:
                            num = 461;
                            if ((Modul1.DAus[99].AsDouble() == 1.0) & (Modul1.Kont[6].Trim() != ""))
                            {
                                Anz[0].SelectedText = " " + Modul1.Kont[6].TrimEnd() + " ";
                            }
                            goto IL_2c09;
                        IL_2c09:
                            num = 464;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Modul1.Datschalt = 1;
                            Listart = 0;
                            LD = 0.AsString();
                            neb = false;
                            Modul1.Datles3(Listart, default, default, ref neb);
                            Modul1.Datschalt = 0;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            if (Modul1.DAus[106] == "1")
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            goto IL_2ce8;
                        IL_2ce8:
                            num = 472;
                            Anz[0].SelectionIndent = 20;
                            M1_Ki = false;
                            Datschreib();
                            if (Modul1.DAus[66] == "1")
                            {
                                Modul1.Datschalt = 1;
                                LD = 0.AsString();
                                Listart = 0;
                                neb = false;
                                Modul1.Datles3(Listart, default, default, ref neb);
                                Modul1.Datschalt = 0;
                                if (Modul1.Kont[25].Trim() != "")
                                {
                                    Anz[0].SelectedText = " " + Modul1.Kont[25].Trim();

                                }
                            }
                            goto IL_2dcc;
                        IL_2dcc: // <========== 3
                            num = 483;
                            M1_Ki = false;
                            Modul1.PrintDat.Ja = 0;
                            if ((Modul1.DAus[38] == "1") | (Modul1.DAus[39] == "1"))
                            {
                                Sonst();
                            }
                            goto IL_2e29;
                        IL_2e29:
                            num = 488;
                            if ((Modul1.DAus[0] == "1") | (Modul1.DAus[13] == "1"))
                            {
                                Modul1.Ubg = 300;
                                neb = true;
                                Berufles1(ref neb, (EEventArt)Modul1.Ubg);
                            }
                            goto IL_2e81;
                        IL_2e81:
                            num = 492;
                            if ((Modul1.DAus[16] == "1") | (Modul1.DAus[17] == "1"))
                            {
                                Modul1.Ubg = 301;
                                neb = true;
                                Berufles1(ref neb, (EEventArt)Modul1.Ubg);
                            }
                            goto IL_2eda;
                        IL_2eda:
                            num = 496;
                            if ((Modul1.DAus[20] == "1") | (Modul1.DAus[21] == "1"))
                            {
                                Modul1.Ubg = 302;
                                neb = true;
                                Berufles1(ref neb, (EEventArt)Modul1.Ubg);
                            }
                            goto IL_2f33;
                        IL_2f33:
                            num = 500;
                            if (Modul1.PrintDat.Ja == 1)
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            goto IL_3400;
                        IL_3118: // <========== 3
                            num = 519;
                            List1.Items.Clear();
                            if (Modul1.DAus[97].AsDouble() == 1.0)
                            {
                                flag2 = true;
                                DataModul.NB_AhnTable.MoveNext();
                                if (!(DataModul.NB_AhnTable.Fields["Ahn"].AsInt() == Ahne.AsInt() + 1))
                                {
                                    goto IL_3505;
                                }
                                goto IL_34ea;
                            }
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            List1.Items.Clear();
                            goto IL_326e;
                        IL_326e: // <========== 3
                            num = 536;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectedText = "   " + Modul1.Kont[3] + " ";
                            if (Modul1.Kont[1] != "")
                            {
                                Anz[0].SelectedText = Modul1.Kont[1] + " ";
                            }
                            goto IL_3314;
                        IL_3314:
                            num = 541;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            if (Modul1.DAus[89] == "1")
                            {
                                Anz[0].SelectedText = Modul1.Kont[0].ToUpper();
                            }
                            else
                            {

                                Anz[0].SelectedText = Modul1.Kont[0];
                            }
                            goto IL_33b0;
                        IL_33b0: // <========== 3
                            num = 548;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectedText = "\n";
                            goto IL_3400;
                        IL_3400: // <========== 3
                            num = 551;
                            if (!flag)
                            {
                                Perabschluss(Modul1.PersInArb, Modul1.FamInArb);
                            }
                            goto IL_3418;
                        IL_3418:
                            num = 554;
                            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "*", TextCompare: false) == 0)
                            {
                                Anz[0].SelectionStart = Anz[0].SelectionStart - 1;
                                Anz[0].SelectionLength = 1;
                                Anz[0].SelectedText = "";
                            }
                            goto IL_34aa;
                        IL_34aa:
                            num = 559;
                            flag = false;
                            Modul1.FamInArb = DataModul.NB_AhnTable.Fields["Ehe"].AsInt();
                            Modul1.Famles();
                            goto IL_34ea;
                        IL_34ea: // <========== 6
                            num = 562;
                            lErl = 4;
                            DataModul.NB_AhnTable.MoveNext();
                            goto IL_3505;
                        IL_3505: // <========== 3
                            num = 564;
                            lErl = 5;
                            goto IL_0b5b;
                        IL_3516:
                            num = 569;
                            Kinder(text);
                            goto end_IL_0000_2;
                        IL_3561:
                            num = 575;
                            T2(text.AsLong());
                            if (DataModul.DSB_QuellIdxTable.RecordCount == 0)
                            {
                                Befehl[7].Enabled = false;
                            }
                            goto IL_35a1;
                        IL_35a1:
                            num = 579;
                            Anz[0].SelectedText = "\n";
                            selectionLength = Anz[0].SelectionStart;
                            Anz[0].SelectionStart = 0;
                            Anz[0].SelectionLength = selectionLength;
                            num5 = (short)(num5 + 1);
                            FileSystem.FileClose(99);
                            text2 = Modul1.GenFreeDir + "Temp\\Sptext" + num5.AsString().Trim() + ".rtf";
                            FileSystem.FileOpen(99, text2, OpenMode.Output);
                            FileSystem.PrintLine(99, Anz[0].SelectedRtf);
                            short num6 = num5;
                            FileSystem.FileClose(99);
                            Anz[0].Text = "";
                            short num7 = num6;
                            num8 = 1;
                            while (num8 <= num7)
                            {
                                text2 = Modul1.GenFreeDir + "Temp\\Sptext" + num8.AsString().Trim() + ".rtf";
                                Anz[3].LoadFile(text2);
                                FileSystem.Kill(text2);
                                selectionLength = Anz[3].Text.Length;
                                Anz[3].SelectionStart = 0;
                                Anz[3].SelectionLength = selectionLength;
                                Anz[0].SelectedRtf = Anz[3].SelectedRtf;
                                Anz[3].Text = "";
                                num8 = (short)unchecked(num8 + 1);
                            }
                            Befehl[0].Enabled = true;
                            Befehl[1].Enabled = true;
                            Befehl[3].Enabled = true;
                            Befehl[4].Enabled = true;
                            Befehl[5].Enabled = true;
                            Befehl[6].Enabled = true;
                            Befehl[7].Enabled = true;
                            if (DataModul.DSB_QuellIdxTable.RecordCount == 0)
                            {
                                Befehl[7].Enabled = false;
                            }
                            goto IL_38dc;
                        IL_38dc:
                            num = 611;
                            Anz[0].Enabled = true;
                            goto end_IL_0000_2;
                        IL_3971:
                            num4 = unchecked(num2 + 1);
                            goto IL_3975;
                        IL_3975:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 67:
                                case 71:
                                case 75:
                                case 76:
                                    goto IL_059b;
                                case 82:
                                case 85:
                                case 86:
                                    goto IL_074b;
                                case 90:
                                case 91:
                                    goto IL_07e0;
                                case 93:
                                case 96:
                                case 97:
                                    goto IL_0825;
                                case 118:
                                case 119:
                                    goto IL_0b4a;
                                case 121:
                                case 565:
                                    goto IL_0b5b;
                                case 129:
                                case 130:
                                    goto IL_0bf4;
                                case 145:
                                case 146:
                                    goto IL_0da1;
                                case 155:
                                case 156:
                                    goto IL_0ece;
                                case 165:
                                case 166:
                                case 167:
                                    goto IL_0fdf;
                                case 171:
                                case 172:
                                    goto IL_1015;
                                case 179:
                                case 180:
                                    goto IL_10b3;
                                case 183:
                                case 184:
                                    goto IL_10f0;
                                case 188:
                                case 189:
                                    goto IL_1150;
                                case 199:
                                case 200:
                                    goto IL_1224;
                                case 193:
                                case 201:
                                case 202:
                                case 203:
                                case 204:
                                case 205:
                                    goto IL_1237;
                                case 206:
                                case 207:
                                case 210:
                                case 211:
                                    goto IL_1259;
                                case 219:
                                case 220:
                                    goto IL_12d8;
                                case 238:
                                case 241:
                                    goto IL_14d9;
                                case 242:
                                case 245:
                                case 248:
                                case 249:
                                case 252:
                                case 253:
                                    goto IL_154b;
                                case 268:
                                case 269:
                                    goto IL_172e;
                                case 272:
                                case 273:
                                    goto IL_1780;
                                case 275:
                                case 276:
                                    goto IL_17ba;
                                case 260:
                                case 261:
                                case 278:
                                    goto IL_180a;
                                case 280:
                                case 281:
                                case 288:
                                case 289:
                                case 290:
                                case 291:
                                case 292:
                                case 293:
                                    goto IL_191c;
                                case 295:
                                case 296:
                                    goto IL_1971;
                                case 324:
                                case 325:
                                    goto IL_1cd7;
                                case 329:
                                case 342:
                                case 343:
                                    goto IL_1d9f;
                                case 334:
                                case 341:
                                case 344:
                                case 345:
                                case 346:
                                    goto IL_1f7c;
                                case 360:
                                case 363:
                                case 364:
                                    goto IL_21f3;
                                case 366:
                                case 367:
                                case 370:
                                case 371:
                                    goto IL_227d;
                                case 356:
                                case 373:
                                case 376:
                                case 377:
                                    goto IL_2315;
                                case 382:
                                case 383:
                                case 390:
                                    goto IL_247b;
                                case 385:
                                case 391:
                                    goto IL_2491;
                                case 396:
                                case 397:
                                case 398:
                                case 399:
                                    goto IL_2545;
                                case 402:
                                case 403:
                                    goto IL_25af;
                                case 407:
                                case 408:
                                    goto IL_2643;
                                case 411:
                                case 414:
                                case 415:
                                    goto IL_26df;
                                case 418:
                                case 419:
                                    goto IL_2761;
                                case 421:
                                case 422:
                                    goto IL_27b0;
                                case 425:
                                case 426:
                                case 430:
                                case 431:
                                case 432:
                                    goto IL_2879;
                                case 452:
                                case 453:
                                case 454:
                                case 455:
                                    goto IL_2b17;
                                case 457:
                                case 458:
                                    goto IL_2b63;
                                case 460:
                                case 461:
                                    goto IL_2b97;
                                case 463:
                                case 464:
                                    goto IL_2c09;
                                case 471:
                                case 472:
                                    goto IL_2ce8;
                                case 481:
                                case 482:
                                case 483:
                                    goto IL_2dcc;
                                case 487:
                                case 488:
                                    goto IL_2e29;
                                case 491:
                                case 492:
                                    goto IL_2e81;
                                case 495:
                                case 496:
                                    goto IL_2eda;
                                case 499:
                                case 500:
                                    goto IL_2f33;
                                case 514:
                                case 517:
                                case 518:
                                case 519:
                                    goto IL_3118;
                                case 532:
                                case 535:
                                case 536:
                                    goto IL_326e;
                                case 540:
                                case 541:
                                    goto IL_3314;
                                case 544:
                                case 547:
                                case 548:
                                    goto IL_33b0;
                                case 502:
                                case 503:
                                case 550:
                                case 551:
                                    goto IL_3400;
                                case 553:
                                case 554:
                                    goto IL_3418;
                                case 558:
                                case 559:
                                    goto IL_34aa;
                                case 212:
                                case 301:
                                case 305:
                                case 524:
                                case 562:
                                    goto IL_34ea;
                                case 527:
                                case 564:
                                    goto IL_3505;
                                case 574:
                                case 575:
                                    goto IL_3561;
                                case 578:
                                case 579:
                                    goto IL_35a1;
                                case 610:
                                case 611:
                                    goto IL_38dc;
                                case 15:
                                case 52:
                                case 115:
                                case 570:
                                case 612:
                                case 615:
                                case 621:
                                case 622:
                                case 623:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj2) when (obj2 is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj2, lErl);
                try0000_dispatch = 17215;
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
    public void Trauzeugen()
    {
        int try0000_dispatch = -1;
        int num = default;
        string text = default;
        int num2 = default;
        int num3 = default;
        byte b = default;
        byte b2 = default;
        byte b3 = default;
        byte b4 = default;
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
                            text = "";
                            goto IL_0009;
                        case 4133:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0d87;
                                    default:
                                        goto end_IL_0000;
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_0d8b;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            num = 2;
                            string text2 = "";
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            if (Modul1.FamInArb == 0)
                            {
                                goto end_IL_0000_2;
                            }
                            b = 1;
                            goto IL_009a;
                        IL_009a: // <========== 3
                            num = 10;
                            foreach (var cLink in DataModul.Link.ReadAllFams(Modul1.FamInArb, ELinkKennz.lkWitnOfEngage))
                            {
                                text2 = Strings.Right("          " + cLink.iPersNr.AsString(), 10);
                                text += text2;
                                b = (byte)unchecked((uint)(b + 1));
                            }
                            goto IL_0184;
                        IL_0184: // <========== 3
                            num = 23;
                            if (text != "")
                            {
                                if (!Tz)
                                {
                                    Anz[0].SelectedText = "\n";
                                    Tz = true;
                                    goto IL_01e8;
                                }
                                Anz[0].SelectedText = " ";
                            }
                            goto IL_01e8;
                        IL_01e8: // <========== 3
                            num = 32;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            if (b == 2)
                            {
                                Anz[0].SelectedText = "Verlobungszeuge: ";
                            }
                            goto IL_023a;
                        IL_023a:
                            num = 36;
                            if (b > 2)
                            {
                                Anz[0].SelectedText = "Verlobungszeugen: ";
                            }
                            goto IL_025c;
                        IL_025c:
                            num = 39;
                            b2 = (byte)Math.Round(text.Length / 10.0);
                            b = 1;
                            goto IL_03dc;
                        IL_03d3: // <========== 3
                            num = 53;
                            b = (byte)unchecked((uint)(b + 1));
                            goto IL_03dc;
                        IL_03dc:
                            if (unchecked(b <= (uint)b2))
                            {
                                Modul1.PersInArb = (int)Math.Round(text.Left(10).AsDouble());
                                text = Strings.Mid(text, 11, text.Length);
                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                Anz[0].SelectedText = Modul1.Kont[3].Trim() + "5%%%%%%%%%%%% " + Modul1.Kont[0].Trim();
                                if (unchecked(b) - 1 < text.Length / 10.0)
                                {
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    Anz[0].SelectedText = "; ";
                                }
                                else
                                {

                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    Anz[0].SelectedText = ".";
                                }
                                goto IL_03d3;
                            }
                            b = 1;
                            goto IL_0455;
                        IL_0455: // <========== 3
                            num = 57;
                            foreach (var cLink in DataModul.Link.ReadAllFams(Modul1.FamInArb, ELinkKennz.lkMarrWitness))
                            {
                                text2 = Strings.Right("          " + cLink.iPersNr.AsString(), 10);
                                text += text2;
                            }
                            goto IL_053f;
                        IL_053f: // <========== 3
                            num = 70;
                            DataModul.DB_FamilyTable.Index = "Fam";
                            DataModul.DB_FamilyTable.Seek("=", Modul1.FamInArb);
                            if (!Information.IsDBNull(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].Value))
                            {
                                if (DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].AsString().Length > 1)
                                {
                                    b = 3;

                                }
                            }
                            goto IL_0604;
                        IL_0604: // <========== 3
                            num = 77;
                            if (text != "")
                            {
                                if (!Tz)
                                {
                                    Anz[0].SelectedText = "\n";
                                    Tz = true;
                                    goto IL_0668;
                                }
                                Anz[0].SelectedText = " ";
                            }
                            goto IL_0668;
                        IL_0668: // <========== 3
                            num = 86;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            if (b == 2)
                            {
                                Anz[0].SelectedText = "Trauzeuge: ";
                            }
                            goto IL_06ba;
                        IL_06ba:
                            num = 90;
                            if (b > 2)
                            {
                                Anz[0].SelectedText = "Trauzeugen: ";
                            }
                            goto IL_06dc;
                        IL_06dc:
                            num = 93;
                            b3 = (byte)Math.Round(text.Length / 10.0);
                            b = 1;
                            goto IL_0886;
                        IL_087d: // <========== 3
                            num = 107;
                            b = (byte)unchecked((uint)(b + 1));
                            goto IL_0886;
                        IL_0886:
                            if (unchecked(b <= (uint)b3))
                            {
                                Modul1.PersInArb = (int)Math.Round(text.Left(10).AsDouble());
                                text = Strings.Mid(text, 11, text.Length);
                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                Anz[0].SelectedText = Modul1.Kont[3].Trim() + "6&%%%%%%%%%% " + Modul1.Kont[0].Trim();
                                if ((unchecked(b) - 1 < text.Length / 10.0) | (DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].AsString().Length > 1))
                                {
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    Anz[0].SelectedText = "; ";
                                }
                                else
                                {

                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    Anz[0].SelectedText = ".";
                                }
                                goto IL_087d;
                            }
                            if (!Information.IsDBNull(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].Value))
                            {
                                if (DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].AsString().Length > 1)
                                {
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    Anz[0].SelectedText = (DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].Value + ".").AsString();

                                }
                            }
                            goto IL_0951;
                        IL_0951: // <========== 3
                            num = 114;
                            b = 1;
                            goto IL_09c3;
                        IL_09c3: // <========== 3
                            num = 117;
                            foreach (var cLink in DataModul.Link.ReadAllFams(Modul1.FamInArb, ELinkKennz.lkWitnOfMarr))
                            {
                                text2 = Strings.Right("          " + cLink.iPersNr.AsString(), 10);
                                text += text2;
                            }
                            goto IL_0ab3;
                        IL_0ab3: // <========== 3
                            num = 130;
                            if (text != "")
                            {
                                if (!Tz)
                                {
                                    Anz[0].SelectedText = "\n";
                                    Tz = true;
                                    goto IL_0b29;
                                }
                                Anz[0].SelectedText = " ";
                            }
                            goto IL_0b29;
                        IL_0b29: // <========== 3
                            num = 139;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            if (b == 2)
                            {
                                Anz[0].SelectedText = "Kirchl. Trauzeuge: ";
                            }
                            goto IL_0b84;
                        IL_0b84:
                            num = 143;
                            if (b > 2)
                            {
                                Anz[0].SelectedText = "Kirchl. Trauzeugen: ";
                            }
                            goto IL_0bac;
                        IL_0bac:
                            num = 146;
                            b4 = (byte)Math.Round(text.Length / 10.0);
                            b = 1;
                            goto IL_0d54;
                        IL_0d48: // <========== 3
                            num = 160;
                            b = (byte)unchecked((uint)(b + 1));
                            goto IL_0d54;
                        IL_0d54:
                            if (unchecked(b > (uint)b4))
                            {
                                goto end_IL_0000_2;
                            }
                            Modul1.PersInArb = (int)Math.Round(text.Left(10).AsDouble());
                            text = Strings.Mid(text, 11, text.Length);
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectedText = Modul1.Kont[3].Trim() + "7&&&&&&&&&&&&&&&& " + Modul1.Kont[0].Trim();
                            if (unchecked(b) - 1 < text.Length / 10.0)
                            {
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                Anz[0].SelectedText = "; ";
                            }
                            else
                            {

                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                Anz[0].SelectedText = ".";
                            }
                            goto IL_0d48;
                        IL_0d87:
                            num4 = unchecked(num2 + 1);
                            goto IL_0d8b;
                        IL_0d8b:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 10:
                                    goto IL_009a;
                                case 11:
                                case 14:
                                case 17:
                                case 23:
                                    goto IL_0184;
                                case 27:
                                case 30:
                                case 31:
                                case 32:
                                    goto IL_01e8;
                                case 35:
                                case 36:
                                    goto IL_023a;
                                case 38:
                                case 39:
                                    goto IL_025c;
                                case 48:
                                case 52:
                                case 53:
                                    goto IL_03d3;
                                case 57:
                                    goto IL_0455;
                                case 58:
                                case 61:
                                case 64:
                                case 70:
                                    goto IL_053f;
                                case 75:
                                case 77:
                                    goto IL_0604;
                                case 81:
                                case 84:
                                case 85:
                                case 86:
                                    goto IL_0668;
                                case 89:
                                case 90:
                                    goto IL_06ba;
                                case 92:
                                case 93:
                                    goto IL_06dc;
                                case 102:
                                case 106:
                                case 107:
                                    goto IL_087d;
                                case 112:
                                case 113:
                                case 114:
                                    goto IL_0951;
                                case 117:
                                    goto IL_09c3;
                                case 118:
                                case 121:
                                case 124:
                                case 130:
                                    goto IL_0ab3;
                                case 134:
                                case 137:
                                case 138:
                                case 139:
                                    goto IL_0b29;
                                case 142:
                                case 143:
                                    goto IL_0b84;
                                case 145:
                                case 146:
                                    goto IL_0bac;
                                case 155:
                                case 159:
                                case 160:
                                    goto IL_0d48;
                                case 5:
                                case 161:
                                case 163:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 4133;
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

    public int GetPerSp1()
    {
        return this.PerSp1;
    }

    public void Paten2(int famInArb, int perSp1)
    {
        Modul1.PrintDat.Pat = false;
        foreach (var cLink in DataModul.Link.ReadAllFams(famInArb, ELinkKennz.lkGodparent))
        {
            Modul1.PersInArb = cLink.iPersNr;
            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
            if (Modul1.DAus[89] == "1")
            {
                Modul1.Kont[0] = Modul1.Kont[0].ToUpper();
            }
            if (Modul1.DAus[76] == "1")
            {
                Modul1.Kont[3] = " <" + Modul1.PersInArb.AsString().Trim() + ">" + Modul1.Kont[3];
            }
            if (Pattext == "")
            {
                Pattext = Modul1.Person.Prae + Modul1.Kont[3].Trim() + " " + Modul1.Kont[0].Trim();
            }
            else
            {
                Pattext = Pattext + "; " + Modul1.Person.Prae + Modul1.Kont[3].Trim() + " " + Modul1.Kont[0].Trim();
            }
        }
        string sBem2 = DataModul.Person.Seek(perSp1)?.Fields["Bem2"].AsString();
        if (sBem2.Trim() != "")
        {
            Modul1.UbgT1 = sBem2;
            if (Modul1.DAus[87] == "0")
            {
                Modul1.UbgT1 = Modul1.PrintDat.Retweg(Modul1.UbgT1);
            }
            Modul1.UbgT1 = Modul1.UbgT1.Left(checked(Modul1.UbgT1.Length - 1)) + Modul1.UbgT1.Right(1).Replace("\n", "");
            if (Pattext == "")
            {
                Pattext = Modul1.UbgT1.Trim();
                Modul1.UbgT1 = "";
            }
            else
            {
                Pattext = Pattext.Trim() + "; " + Modul1.UbgT1.Trim();
                Modul1.UbgT1 = "";
            }
        }
    }

    private void Command2_Click(object eventSender, EventArgs eventArgs)
    {
        int index = Command2.GetIndex((Button)eventSender);
        Frame2.Visible = false;
        Modul1.Ind1 = "";
        RichTextBox richTextBox = Anz[3];
        IRecordset dT_OrtIdxTable = DataModul.DSB_OrtIdxTable;

        richTextBox.Text = "";
        richTextBox.SelectionHangingIndent = 0;
        richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        switch (index)
        {
            case 0:
                {
                    richTextBox.SelectionAlignment = HorizontalAlignment.Center;
                    richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                    richTextBox.SelectedText = "Ortsindex";
                    richTextBox.SelectedText = "\n";
                    dT_OrtIdxTable.Index = "Ort";
                    dT_OrtIdxTable.Seek(">=", " ");
                    while (!dT_OrtIdxTable.EOF)
                    {
                        DataModul.DB_PlaceTable.Seek("=", dT_OrtIdxTable.Fields["OrtNr"]);
                        if (!DataModul.DB_PlaceTable.NoMatch)
                        {
                            int AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt();
                            Modul1.Kont[2] = DataModul.TextLese1(AAA);
                            AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt();
                            Modul1.Kont[1] = DataModul.TextLese1(AAA);
                        }
                        dT_OrtIdxTable.MoveNext();
                    }
                    dT_OrtIdxTable.Index = "Ort";
                    dT_OrtIdxTable.Seek(">=", " ");
                    int num = default;
                    while (!dT_OrtIdxTable.EOF)
                    {
                        if (dT_OrtIdxTable.Fields["OrtNr"].AsInt() != Modul1.AltNr)
                        {
                            num = 0;
                            richTextBox.SelectionAlignment = HorizontalAlignment.Left;
                            richTextBox.SelectedText = "\n";
                            richTextBox.SelectionIndent = 0;
                            var UbgT = Modul1.ortles1(dT_OrtIdxTable.Fields["OrtNr"].AsInt(), 21);

                            richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            richTextBox.SelectedText = UbgT;
                            Modul1.AltNr = dT_OrtIdxTable.Fields["OrtNr"].AsInt();
                            richTextBox.SelectedText = "\n";
                            richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            richTextBox.SelectionIndent = 20;
                        }
                        if (dT_OrtIdxTable.Fields["Ind"].AsInt() != num)
                        {
                            richTextBox.SelectedText = dT_OrtIdxTable.Fields["Ind"].AsString().Trim() + "; ";
                            num = dT_OrtIdxTable.Fields["Ind"].AsInt();
                        }
                        dT_OrtIdxTable.MoveNext();
                    }
                    Befehl[4].Visible = true;
                    richTextBox.SaveFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
                    richTextBox.LoadFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
                    break;
                }
            case 1:
                richTextBox.SelectionAlignment = HorizontalAlignment.Center;
                richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                richTextBox.SelectedText = "Index Orte-Namen";
                richTextBox.SelectedText = "\n";
                richTextBox.SelectionHangingIndent = 0;
                dT_OrtIdxTable.Index = "ortnam";
                dT_OrtIdxTable.Seek(">=", " ");
                while (!dT_OrtIdxTable.EOF)
                {
                    if (dT_OrtIdxTable.Fields["OrtNr"].AsInt() != Modul1.AltNr)
                    {
                        richTextBox.SelectedText = "\n";
                        richTextBox.SelectionAlignment = HorizontalAlignment.Left;
                        richTextBox.SelectionIndent = 0;
                        Modul1.AltName = "";
                        Modul1.AltNr = dT_OrtIdxTable.Fields["OrtNr"].AsInt();
                        richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                        Modul1.ortles(dT_OrtIdxTable.Fields["OrtNr"].AsInt(), 21);
                        richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                        richTextBox.SelectedText = Modul1.UbgT;
                        richTextBox.SelectedText = "\n";
                        richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        richTextBox.SelectionIndent = 20;
                    }
                    if (Operators.CompareString(dT_OrtIdxTable.Fields["Name"].AsString().Trim(), Modul1.AltName.Trim(), TextCompare: false) != 0)
                    {
                        if (Modul1.AltName != "")
                        {
                            richTextBox.SelectedText = "\n";
                        }
                        richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                        richTextBox.SelectedText = (dT_OrtIdxTable.Fields["Name"].Value + "  ").AsString();
                        richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        Modul1.AltName = dT_OrtIdxTable.Fields["Name"].AsString();
                    }
                    richTextBox.SelectedText = dT_OrtIdxTable.Fields["Ind"].AsString().Trim() + "; ";
                    dT_OrtIdxTable.MoveNext();
                }
                Befehl[4].Visible = true;
                richTextBox.SaveFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
                richTextBox.LoadFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
                break;
            case 2:
                richTextBox.SelectionAlignment = HorizontalAlignment.Center;
                richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                richTextBox.SelectedText = "Index Namen-Orte";
                richTextBox.SelectedText = "\n";
                dT_OrtIdxTable.Index = "NameOrt";
                dT_OrtIdxTable.Seek(">=", " ");
                while (!dT_OrtIdxTable.EOF)
                {
                    if (Operators.CompareString(dT_OrtIdxTable.Fields["Name"].AsString().Trim(), Modul1.AltName.Trim(), TextCompare: false) != 0)
                    {
                        richTextBox.SelectionAlignment = HorizontalAlignment.Left;
                        richTextBox.SelectedText = "\n";
                        richTextBox.SelectionIndent = 0;
                        Modul1.AltNr = 0;
                        richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                        richTextBox.SelectedText = dT_OrtIdxTable.Fields["Name"].AsString();
                        Modul1.AltName = dT_OrtIdxTable.Fields["Name"].AsString();
                        richTextBox.SelectedText = "\n";
                        richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        richTextBox.SelectionIndent = 20;
                    }
                    if (dT_OrtIdxTable.Fields["OrtNr"].AsInt() != Modul1.AltNr)
                    {
                        if (Modul1.AltNr > 0)
                        {
                            richTextBox.SelectedText = "\n";
                        }
                        richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                        richTextBox.SelectedText = (dT_OrtIdxTable.Fields["Ort"].Value + "  ").AsString();
                        richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        Modul1.AltNr = dT_OrtIdxTable.Fields["OrtNr"].AsInt();
                    }
                    richTextBox.SelectedText = dT_OrtIdxTable.Fields["Ind"].AsString().Trim() + "; ";
                    dT_OrtIdxTable.MoveNext();
                }
                Befehl[4].Visible = true;
                richTextBox.SaveFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
                richTextBox.LoadFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
                break;
            case 3:
                Modul1.PrintDat.Flagsch = 1;
                dT_OrtIdxTable.Index = "Ort";
                dT_OrtIdxTable.Seek(">=", " ");
                while (!dT_OrtIdxTable.EOF)
                {
                    if (dT_OrtIdxTable.Fields["OrtNr"].AsInt() != Modul1.AltNr)
                    {
                        richTextBox.SelectionIndent = 0;
                        Modul1.ortles(dT_OrtIdxTable.Fields["OrtNr"].AsInt(), 2);
                        richTextBox.SelectedText = Modul1.UbgT;
                        Modul1.AltNr = dT_OrtIdxTable.Fields["OrtNr"].AsInt();
                        richTextBox.SelectedText = "\n";
                    }
                    dT_OrtIdxTable.MoveNext();
                }
                break;
            default:
                Interaction.MsgBox(index);
                break;
        }
    }

    private void Ahnen_Load(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int number = default;
        int lErl = default;
        int num5 = default;
        string source = default;
        string destination = default;
        string name = default;
        IRecordset IRecordset = default;
        int num6 = default;
        string text = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                Hinter hinter;
                IRecordset Recordset2;
                IRecordset Recordset3;
                int num7;
                short Listart;
                long Fad;
                bool neb;
                switch (try0000_dispatch)
                {
                    default:
                        num = 1;
                        BackColor = Modul1.HintFarb;
                        goto IL_0013;
                    case 3403:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 3:
                                    break;
                                case 2:
                                    goto IL_0974;
                                case 1:
                                    goto IL_0acd;
                                default:
                                    goto end_IL_0000;
                            }
                            number = Information.Err().Number;
                            if (number == 55)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0acd;
                            }
                            if (number == 3021)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num2 = 0;
                                goto IL_0781;
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
                            goto IL_0ac9;
                        }
                    end_IL_0000:
                        break;
                    IL_0013:
                        num = 2;
                        Befehl[3].Text = Modul1.IText[47];
                        Modul1.Dateienopen();
                        Modul1.eWindowState = Modul1.Persistence.ReadEnumInit<FormWindowState>("Windowstate");
                        WindowState = Modul1.eWindowState.AsEnum<FormWindowState>();

                        Modul1.Feg = (short)Modul1.Persistence.ReadIntInit("state");
                        Modul1.Fs = Modul1.Feg switch
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
                            _ => Modul1.Fs,
                        };
                        goto IL_023a;
                    IL_023a: // <========== 12
                        num = 49;
                        Font = new Font("Arial", Modul1.Fs, FontStyle.Regular);
                        FileSystem.FileClose(6);
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        FileSystem.MkDir(Modul1.Verz + "Ahn");
                        DataModul.NB.Close();
                        hinter = MyProject.Forms.Hinter;
                        hinter.Att(Modul1.Verz + "Ahn");
                        FileSystem.Kill(Modul1.Verz + "Ahn\\*.*");
                        Modul1.Verz1 = Modul1.Verz.Left(15);
                        source = Modul1.GenFreeDir + "INIT\\GedAUS.mdb";
                        destination = Modul1.Verz + "Ahn\\GEDAUS.mdb";
                        FileSystem.FileCopy(source, destination);
                        name = Modul1.Verz + "Ahn\\GEDAUS.mdb";
                        DataModul.NB = UpgradeSupport.DAODBEngine_definst.OpenDatabase(name, false, false, "");
                        IRecordset = DataModul.NB.OpenRecordset(dbTables.Orte, RecordsetTypeEnum.dbOpenTable);
                        IRecordset.Index = "ORT";
                        Recordset2 = DataModul.NB.OpenRecordset(dbTables.QuellTemp, RecordsetTypeEnum.dbOpenTable);
                        Recordset3 = DataModul.NB.OpenRecordset(dbTables.Bilder, RecordsetTypeEnum.dbOpenTable);
                        DataModul.NB_PersonTable = DataModul.NB.OpenRecordset(dbTables.Personen1, RecordsetTypeEnum.dbOpenTable);
                        DataModul.NB_FamilyTable = DataModul.NB.OpenRecordset(dbTables.Familie1, RecordsetTypeEnum.dbOpenTable);
                        Show();
                        ProjectData.ClearProjectError();
                        num3 = 3;
                        num7 = num6;
                        num6 = 0;
                        FileSystem.FileClose(30);
                        Label1[3].Text = "Keine Berechnung vorhanden";
                        Label1[0].Text = "Sie müssen erst die Ahnen berechnen.";
                        DataModul.NB_AhnTable.Index = "Ahnen";
                        DataModul.NB_AhnTable.Seek("=", 1);
                        DataModul.NB_AhnTable.MoveFirst();
                        if (!DataModul.NB_AhnTable.EOF)
                        {
                            if (!DataModul.NB_AhnTable.NoMatch)
                            {
                                text = DataModul.NB_AhnTable.Fields["Ahn"].AsString();
                                DataModul.NB_AhnTable.MoveLast();
                                Label1[3].Text = "Ahnenberechnung " + DataModul.NB_AhnTable.Fields["Gen"].AsString() + " Generationen vorhanden für Ahnenziffer " + text;
                                DataModul.NB_AhnTable.MoveFirst();
                                Modul1.PersInArb = DataModul.NB_AhnTable.Fields["PerNr"].AsInt();
                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                if (Modul1.Kont[1].Trim() != "")
                                {
                                    Modul1.Kont[1] = Modul1.Kont[1] + " ";
                                }
                                goto IL_0642;
                            }
                        }
                        goto IL_0781;
                    IL_0642:
                        num = 94;
                        if (Modul1.DAus[89] == "1")
                        {
                            Label1[0].Text = (Modul1.Person.Prae + " " + Modul1.Kont[3] + " " + Modul1.Kont[1] + Modul1.Kont[0].ToUpper() + " " + Modul1.Kont[2]).Trim();
                        }
                        else
                        {

                            Label1[0].Text = (Modul1.Person.Prae + " " + Modul1.Kont[3] + " " + Modul1.Kont[1] + Modul1.Kont[0] + " " + Modul1.Kont[2]).Trim();
                        }
                        goto IL_0760;
                    IL_0760: // <========== 3
                        num = 100;
                        Listart = 0;
                        Fad = 0;
                        neb = false;
                        Modul1.Datles3(Listart, Fad, default, ref neb);
                        goto IL_0781;
                    IL_0781: // <========== 4
                        num = 101;
                        lErl = 1;
                        Label1[5].Text = Modul1.IText[3] + " " + Modul1.Kont[11];
                        Label1[2].Text = Modul1.IText[5] + " " + Modul1.Kont[13];
                        Label1[1].Text = Modul1.IText[4] + " " + Modul1.Kont[12];
                        Label1[4].Text = Modul1.DTxt[4] + " " + Modul1.Kont[14];
                        Anz[0].SelectionHangingIndent = 15;
                        Bezeichnung1[0].Width = Width;
                        num5 = 0;
                        do
                        {
                            num = 109;
                            num5 = checked(num5 + 1);
                        }
                        while (num5 <= 7);
                        if (Label1[3].Text != "Keine Berechnung vorhanden")
                        {
                        }
                        else
                        {

                            Command_Renamed[1].Enabled = false;
                        }
                        goto end_IL_0000_2;
                    IL_0974:
                        num = 132;
                        if (Information.Err().Number == 3420)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_0acd;
                        }
                        if (Information.Err().Number == 75)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_0acd;
                        }
                        if (Information.Err().Number == 53)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_0acd;
                        }
                        if (Information.Err().Number == 70)
                        {
                            MyProject.Forms.Druck.Bef[6].PerformClick();
                        }
                        goto IL_0a4c;
                    IL_0a4c:
                        num = 147;
                        if (Information.Err().Number == 91)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_0acd;
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
                        goto IL_0ac9;
                    IL_0ac9:
                        num4 = num2;
                        goto IL_0ad1;
                    IL_0acd: // <========== 6
                        num4 = num2 + 1;
                        goto IL_0ad1;
                    IL_0ad1:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 17:
                            case 21:
                            case 24:
                            case 27:
                            case 30:
                            case 33:
                            case 36:
                            case 39:
                            case 42:
                            case 45:
                            case 48:
                            case 49:
                                goto IL_023a;
                            case 93:
                            case 94:
                                goto IL_0642;
                            case 96:
                            case 99:
                            case 100:
                                goto IL_0760;
                            case 80:
                            case 83:
                            case 101:
                            case 123:
                                goto IL_0781;
                            case 146:
                            case 147:
                                goto IL_0a4c;
                            case 112:
                            case 113:
                            case 155:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 3403;
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
    public void Berufles1(ref bool Ki, EEventArt eArt)
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
        int num7 = default(short);
        int persInArb = default;
        string text2 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    int ortNr;
                    int ortNr2;
                    int ortNr3;
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
                        case 9934:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_2154;
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
                                goto IL_2158;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            List2.Items.Clear();
                            Modul1_Beruf = eArt;
                            foreach (var cEv in DataModul.Event.ReadEventsBeSu(Modul1.PersInArb, Modul1_Beruf))
                            {
                                if (!Information.IsDBNull(cEv.iPrivacy))
                                {
                                    Modul1_iPrivacy = cEv.iPrivacy;
                                }
                                else
                                {
                                    Modul1_iPrivacy = 0;
                                }

                                if (!(Modul1_iPrivacy.AsDouble() > Modul1.iPriv_aus))
                                {
                                    Modul1.PrintDat.Ja = 1;
                                    if (cEv.iLfNr >= 1)
                                    {
                                        Modul1_J = 0;
                                        while (unchecked((byte)unchecked((uint)(Modul1_J + 1))) <= 15u)
                                        {
                                            Modul1.Kont1[Modul1_J] = "";
                                        }
                                        Modul1.Ubg = num5;
                                        Modul1.sDatu = "";
                                        if ((cEv.iPerFamNr != Modul1.PersInArb)
                                            | (cEv.eArt != Modul1_Beruf))
                                        {
                                            break;
                                        }
                                        if (cEv.dDatumV != default)
                                        {
                                            Modul1.sDatu = Strings.Right("00000000" + cEv.dDatumV.AsInt(), 8);
                                            Modul1.sDatu = cEv.dDatumV.AsString();
                                            Modul1.Kont1[1] = Modul1.sDatu;
                                        }
                                        Modul1.UbgT = "";
                                        if (cEv.iKBem > 0)
                                        {
                                            if (cEv.sKBem.Trim() != "")
                                            {
                                                Modul1.Kont1[7] = " " + cEv.sKBem.Trim() + " ";

                                            }
                                        }
                                        if (Modul1_Beruf == EEventArt.eA_302
                                            && cEv.iHausNr != 0)
                                        {
                                            Modul1.Kont1[7] = Modul1.Kont1[7] + " " + cEv.sHausNr.Trim() + " ";
                                        }
                                        if (cEv.iArtText > 0 && cEv.sArtText != "")
                                        {
                                            Modul1.Kont[10] = " " + cEv.sArtText.Trim() + ": ";
                                        }
                                        Job = Modul1.Kont1[1] + Modul1.Kont1[3] + Modul1.Kont1[7] + new string(' ', 240).Left(240) + cEv.iLfNr.AsString();
                                        if (cEv.sReg != "")
                                        {
                                            Job = "+" + Job;
                                        }
                                        List2.Items.Add(new ListItem<int>(Job, cEv.iLfNr));
                                    }
                                }
                                lErl = 12;
                            }
                            goto IL_073d;
                        IL_073d: // <========== 4
                            num = 76;
                            lErl = 13;
                            switch (Modul1_Beruf)
                            {
                                case EEventArt.eA_300:
                                    if (List2.Items.Count == 0)
                                    {
                                        goto end_IL_0000_2;
                                    }
                                    if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                                    {
                                        if (Modul1.DAus[106] == "1")
                                        {
                                            Anz[0].SelectedText = "\n";
                                        }
                                        else
                                            Anz[0].SelectedText = " ";
                                    }
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                                    if (List2.Items.Count == 1)
                                    {
                                        Anz[0].SelectedText = "Beruf:";
                                    }
                                    if (List2.Items.Count > 1)
                                    {
                                        Anz[0].SelectedText = "Berufe:";
                                    }
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    Anz[0].SelectedText = " ";
                                    break;
                                case EEventArt.eA_301:
                                    if (List2.Items.Count == 0)
                                    {
                                        goto end_IL_0000_2;
                                    }
                                    if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                                    {
                                        if (Modul1.DAus[106] == "1")
                                        {
                                            Anz[0].SelectedText = "\n";
                                        }
                                        else
                                            Anz[0].SelectedText = " ";
                                    }
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                                    Anz[0].SelectedText = Modul1.IText[70].Trim();
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    Anz[0].SelectedText = " ";
                                    break;
                                case EEventArt.eA_302:
                                    if (List2.Items.Count == 0)
                                    {
                                        goto end_IL_0000_2;
                                    }
                                    if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                                    {
                                        if (Modul1.DAus[106] == "1")
                                        {
                                            Anz[0].SelectedText = "\n";
                                        }
                                        else
                                            Anz[0].SelectedText = " ";
                                    }
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                                    Anz[0].SelectedText = Modul1.IText[8].Trim();
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    Anz[0].SelectedText = " ";
                                    break;
                                default:
                                    break;
                            }
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            num6 = (short)(List2.Items.Count - 1);
                            num5 = 0;
                            while (num5 <= num6
                                && DataModul.Event.ReadData((Modul1_Beruf, Modul1.PersInArb, (short)List2.Items[num5].ItemData<int>()), out var cEv))
                            {
                                if (cEv.iLfNr < 1)
                                {
                                    Interaction.MsgBox("7");
                                    Debugger.Break();
                                }
                                Modul1_J = 0;
                                while (Modul1_J <= 15u)
                                {
                                    Modul1.Kont1[Modul1_J] = "";
                                    Modul1_J++;
                                }
                                Modul1.Ubg = num5;
                                Modul1.sDatu = "";
                                if (!Ki)
                                {
                                    if (((Modul1_Beruf == EEventArt.eA_300) & (Modul1.DAus[25] == "1"))
                                        | ((Modul1_Beruf == EEventArt.eA_301) & (Modul1.DAus[29] == "1"))
                                        | ((Modul1_Beruf == EEventArt.eA_302) & (Modul1.DAus[33] == "1")))
                                    {
                                        left = "1";
                                    }
                                }
                                else
                                if (((Modul1_Beruf == EEventArt.eA_300) & (Modul1.DAus[13] == "1"))
                                    | ((Modul1_Beruf == EEventArt.eA_301) & (Modul1.DAus[17] == "1"))
                                    | ((Modul1_Beruf == EEventArt.eA_302) & (Modul1.DAus[21] == "1")))
                                {
                                    left = "1";
                                }
                                if (left == "1")
                                {
                                    (Modul1.Kont1[1], Modul1.Kont1[3]) = Event_FullDate(cEv);
                                    if (cEv.iOrt > 0)
                                    {
                                        ortNr = (int)Math.Round(cEv.iOrt.AsDouble());
                                        Modul1.ortles(ortNr, 1);
                                        Modul1.Kont1[5] = " " + Modul1.UbgT.Trim();
                                        Modul1.UbgT = "";
                                        if (Operators.ConditionalCompareObjectNotEqual(cEv.sOrt_S, "", TextCompare: false))
                                        {
                                            Modul1.Kont1[5] = (Modul1.Kont1[5] + " " + cEv.sOrt_S).AsString();

                                        }
                                    }
                                    if (cEv.iPlatz > 0.0)
                                    {
                                        if (cEv.sPlatz != "")
                                        {
                                            Modul1.Kont1[6] = " " + cEv.sPlatz.Trim();

                                        }
                                    }
                                }
                                Modul1.UbgT = "";
                                if (cEv.iKBem > 0)
                                {
                                    if (cEv.sKBem != "")
                                    {
                                        Modul1.Kont1[7] = " " + cEv.sKBem.Trim();

                                    }
                                }
                                if (Modul1_Beruf == EEventArt.eA_302)
                                {
                                    if (cEv.iHausNr > 0)
                                    {
                                        Modul1.Kont1[7] = Modul1.Kont1[7] + " " + cEv.sHausNr.Trim() + " ";
                                    }
                                }
                                if (Modul1_Beruf == EEventArt.eA_302)
                                {
                                    if (((Modul1.DAus[20] == "1") & Ki) || (Modul1.DAus[32] == "1") & !Ki)
                                    {
                                        if (cEv.iOrt > 0)
                                        {
                                            ortNr2 = cEv.iOrt;
                                            Modul1.ortles(ortNr2, 1);
                                            Modul1.Kont1[7] = Modul1.Kont1[7] + " " + Modul1.UbgT.Trim();
                                            Modul1.UbgT = "";
                                            if (cEv.sOrt_S != "")
                                            {
                                                Modul1.Kont1[7] = Modul1.Kont1[7] + " " + cEv.sOrt_S;
                                            }

                                        }
                                    }
                                }
                                left = "0";
                                Job = "";
                                Job = Module2.Jobdreh(Job);
                                if (Job.Trim() != "")
                                {
                                    if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == ";")
                                    {
                                        Anz[0].SelectedText = " ";
                                    }
                                    Anz[0].SelectedText = Job.TrimEnd();
                                }
                                left = "0";
                                if (!Ki)
                                {
                                    if (((Modul1_Beruf == EEventArt.eA_300) & (Modul1.DAus[26] == "1"))
                                        | ((Modul1_Beruf == EEventArt.eA_301) & (Modul1.DAus[30] == "1"))
                                        | ((Modul1_Beruf == EEventArt.eA_302) & (Modul1.DAus[34] == "1")))
                                    {
                                        left = "1";
                                    }
                                }
                                else
                                if (((Modul1_Beruf == EEventArt.eA_300) & (Modul1.DAus[14] == "1"))
                                    | ((Modul1_Beruf == EEventArt.eA_301) & (Modul1.DAus[18] == "1"))
                                    | ((Modul1_Beruf == EEventArt.eA_302) & (Modul1.DAus[22] == "1")))
                                {
                                    left = "1";
                                }
                                if (left == "1")
                                {
                                    if (Operators.CompareString(cEv.sBem[1].Trim(), "", TextCompare: false) != 0)
                                    {
                                        Modul1.UbgT1 = cEv.sBem[1].Trim();
                                        Bemaus(Anz[0], Modul1.UbgT1);

                                    }
                                }
                                left = "0";
                                if (!Ki)
                                {
                                    if (((Modul1_Beruf == EEventArt.eA_300) & (Modul1.DAus[27] == "1"))
                                        | ((Modul1_Beruf == EEventArt.eA_301) & (Modul1.DAus[31] == "1"))
                                        | ((Modul1_Beruf == EEventArt.eA_302) & (Modul1.DAus[35] == "1")))
                                    {
                                        left = "1";
                                    }
                                }
                                else
                                if (((Modul1_Beruf == EEventArt.eA_300) & (Modul1.DAus[15] == "1"))
                                    | ((Modul1_Beruf == EEventArt.eA_301) & (Modul1.DAus[19] == "1"))
                                    | ((Modul1_Beruf == EEventArt.eA_302) & (Modul1.DAus[23] == "1")))
                                {
                                    left = "1";
                                }
                                if (left == "1")
                                {
                                    if (cEv.sBem[2] != " ")
                                    {
                                        Modul1.UbgT1 = cEv.sBem[2].Trim();
                                        Bemaus(Anz[0], Modul1.UbgT1);

                                    }
                                }
                                Nr = Modul1.PersInArb;
                                LfNR = Modul1.PrintDat.LfNR;
                                Modul1.PrintDat.QuelledotnDatum(ref Nr, Modul1_Beruf, ref LfNR);
                                Modul1.PrintDat.LfNR = Conversions.ToByte(LfNR);
                                Modul1.PersInArb = Nr.AsInt();
                                if (Modul1.Kont1[9].Trim() != "")
                                {
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                    Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                                    Anz[0].SelectedText = " " + Modul1.Kont1[9];
                                    Anz[0].SelectionCharOffset = 0;
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                }
                                left = "0";
                                if (Ki)
                                {
                                    num7 = Modul1.DAus[96].AsInt();
                                }
                                if (!Ki)
                                {
                                    num7 = Modul1.DAus[98].AsInt();
                                }
                                persInArb = Modul1.PersInArb;
                                if (num7 == 1)
                                {
                                    Modul1.Zeugsu(Modul1_Beruf, Modul1.PrintDat.LfNR, 2, Ahne);
                                }
                                text2 = Modul1.Kont1[20];
                                Modul1.Kont1[20] = "";
                                Modul1.PersInArb = persInArb;
                                if (num7 == 1)
                                {
                                    if (text2 != "")
                                    {
                                        Anz[0].SelectedText = " ";
                                        if (Modul1.DAus[100] == "1")
                                        {
                                            M_Sgg = 0.8f;
                                        }
                                        else
                                        {

                                            M_Sgg = 1f;
                                        }
                                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Underline);
                                        Anz[0].SelectedText = "Zeugen: ";
                                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Regular);
                                        Anz[0].SelectedText = text2.Trim();
                                        text2 = "\"";
                                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    }
                                }
                                Anz[0].SelectedText = ";";
                                num5++;
                            }
                            goto IL_2055;
                        IL_2055:
                            num = 335;
                            if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == ";")
                            {
                                Anz[0].SelectionStart = Anz[0].SelectionStart - 1;
                                Anz[0].SelectionLength = 1;
                                Anz[0].SelectedText = ".";
                            }
                            goto IL_20f3;
                        IL_20f3:
                            num = 340;
                            left = "0";
                            goto end_IL_0000_2;
                        IL_2154:
                            num4 = unchecked(num2 + 1);
                            goto IL_2158;
                        IL_2158:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 24:
                                case 37:
                                case 76:
                                    goto IL_073d;
                                case 142:
                                case 335:
                                    goto IL_2055;
                                case 339:
                                case 340:
                                    goto IL_20f3;
                                case 9:
                                case 82:
                                case 104:
                                case 121:
                                case 155:
                                case 341:
                                case 346:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 9934;
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

    private (string MK1, string MK3) Event_FullDate(IEventData cEv)
    {
        // Todo: I18N
        const string cFrom = "von";
        const string cUntil = "bis";

        string MK1 = "";
        string MK3 = "";
        if (cEv.dDatumV != default)
        {
            var MsD = Strings.Right("00000000" + cEv.dDatumV.AsString().Trim(), 8);
            MsD = Modul1.Datwand1(MsD, cEv.sDatumV_S);
            MK1 = MsD;
        }
        if (cEv.dDatumB != default)
        {
            if (MK1 != "")
            {
                MK1 = $"{cFrom} {MK1}";
            }
            var MsD = ("00000000" + cEv.dDatumB.AsString()).Right(8);
            MsD = Modul1.Datwand1(MsD, cEv.sDatumB_S);
            if ((MsD != "") & (cEv.sDatumB_S.Trim() == ""))
            {
                MsD = $" {cUntil} {MsD.Trim()}";
            }
            MK3 = MsD;
        }
        if (cEv.iDatumText > 0
            && cEv.sDatumText.Trim() != "")
        {
            MK3 = MK3 + " " + cEv.sDatumText.Trim().FrameIfNEoW(new[] { "(", ")" });
        }
        var r = (MK1, MK3);
        return r;
    }

    public void Bemaus(RichTextBox richTextBox, string ubgT1)
    {
        if (Modul1.DAus[70] == "0")
        {
            ubgT1 = Modul1.PrintDat.Retweg(ubgT1);
        }
        ubgT1 = ubgT1.Left(checked(ubgT1.Length - 1)) + ubgT1.Right(1).Replace("\n", "");
        if (Modul1.DAus[72] == "1")
        {
            richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
        }
        else
        {
            richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        }
        if (Strings.Mid(richTextBox.Text, richTextBox.SelectionStart, 1) != "\n")
        {
            richTextBox.SelectedText = " ";
        }
        richTextBox.SelectedText = "{";
        richTextBox.SelectedText = ubgT1;
        richTextBox.SelectedText = "}";
        richTextBox.SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
    }

    public void Sonst()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        byte b = default;
        string text = default;
        int lErl = default;
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
                    case 1659:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0555;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_050c;
                        }
                    IL_0555:
                        num4 = num2 + 1;
                        goto IL_0559;
                    IL_050c:
                        num = 65;
                        if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                        {
                            ProjectData.EndApp();
                        }
                        goto IL_0532;
                    IL_04de:
                        num = 61;
                        DataModul.DB_EventTable.MoveNext();
                        goto IL_04ec;
                    IL_0532:
                        num = 68;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        num4 = num2;
                        goto IL_0559;
                    IL_04ec:
                        num = 62;
                        b = checked((byte)unchecked((uint)(b + 1)));
                        if (b <= 70u)
                        {
                            goto IL_00c0;
                        }
                        goto IL_04fd;
                    IL_04fd:
                        num = 63;
                        Sonstdat();
                        goto end_IL_0000_2;
                    IL_0559:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_0008;
                            case 3:
                                goto IL_001b;
                            case 4:
                                goto IL_002d;
                            case 5:
                                goto IL_008a;
                            case 6:
                                goto IL_0099;
                            case 8:
                            case 9:
                                goto IL_00b0;
                            case 10:
                                goto IL_00ba;
                            case 11:
                                goto IL_00c0;
                            case 13:
                            case 14:
                                goto IL_00d5;
                            case 15:
                                goto IL_00e5;
                            case 17:
                            case 18:
                                goto IL_00fd;
                            case 19:
                                goto IL_0126;
                            case 21:
                                goto IL_014f;
                            case 22:
                                goto IL_0153;
                            case 20:
                            case 23:
                            case 24:
                                goto IL_0162;
                            case 26:
                            case 27:
                                goto IL_0181;
                            case 29:
                            case 30:
                                goto IL_01b1;
                            case 31:
                                goto IL_01bb;
                            case 32:
                                goto IL_01cf;
                            case 33:
                                goto IL_01e9;
                            case 34:
                                goto IL_01f7;
                            case 35:
                                goto IL_026a;
                            case 37:
                            case 38:
                                goto IL_0282;
                            case 39:
                                goto IL_02b4;
                            case 40:
                                goto IL_02db;
                            case 41:
                            case 42:
                                goto IL_02eb;
                            case 43:
                                goto IL_02f9;
                            case 44:
                                goto IL_0325;
                            case 45:
                                goto IL_035a;
                            case 46:
                                goto IL_03a1;
                            case 47:
                                goto IL_03ba;
                            case 48:
                            case 49:
                            case 50:
                            case 51:
                                goto IL_03e0;
                            case 52:
                                goto IL_03f3;
                            case 54:
                                goto IL_044b;
                            case 55:
                                goto IL_044f;
                            case 53:
                            case 56:
                            case 57:
                                goto IL_04a5;
                            case 58:
                                goto IL_04bd;
                            case 25:
                            case 28:
                            case 59:
                            case 60:
                                goto IL_04d3;
                            case 61:
                                goto IL_04de;
                            case 62:
                                goto IL_04ec;
                            case 12:
                            case 36:
                            case 63:
                                goto IL_04fd;
                            case 65:
                                goto IL_050c;
                            case 66:
                            case 68:
                                goto IL_0532;
                            default:
                                goto end_IL_0000;
                            case 7:
                            case 16:
                            case 64:
                            case 69:
                                goto end_IL_0000_2;
                        }
                        goto default;
                    IL_0008:
                        num = 2;
                        List2.Items.Clear();
                        goto IL_001b;
                    IL_001b:
                        num = 3;
                        DataModul.DB_EventTable.Index = "Besu";
                        goto IL_002d;
                    IL_002d:
                        num = 4;
                        DataModul.DB_EventTable.Seek("=", "105", Modul1.PersInArb.AsString());
                        goto IL_008a;
                    IL_008a:
                        num = 5;
                        if (DataModul.DB_EventTable.NoMatch)
                        {
                            goto IL_0099;
                        }
                        goto IL_00b0;
                    IL_0099:
                        num = 6;
                        DataModul.DB_EventTable.Index = "ArtNr";
                        goto end_IL_0000_2;
                    IL_00b0:
                        num = 9;
                        Modul1.PrintDat.Ja = 1;
                        goto IL_00ba;
                    IL_00ba:
                        num = 10;
                        b = 1;
                        goto IL_00c0;
                    IL_00c0:
                        num = 11;
                        if (!DataModul.DB_EventTable.EOF)
                        {
                            goto IL_00d5;
                        }
                        goto IL_04fd;
                    IL_00d5:
                        num = 14;
                        if (DataModul.DB_EventTable.NoMatch)
                        {
                            goto IL_00e5;
                        }
                        goto IL_00fd;
                    IL_00e5:
                        num = 15;
                        DataModul.DB_EventTable.Index = "ArtNr";
                        goto end_IL_0000_2;
                    IL_00fd:
                        num = 18;
                        if (!Information.IsDBNull(DataModul.DB_EventTable.Fields["priv"].Value))
                        {
                            goto IL_0126;
                        }
                        goto IL_014f;
                    IL_0126:
                        num = 19;
                        Modul1_iPrivacy = DataModul.DB_EventTable.Fields["priv"].AsInt();
                        goto IL_0162;
                    IL_014f:
                        num = 21;
                        goto IL_0153;
                    IL_0153:
                        num = 22;
                        Modul1_iPrivacy = 0;
                        goto IL_0162;
                    IL_0162:
                        num = 24;
                        if (!(Modul1_iPrivacy.AsDouble() > Modul1.iPriv_aus))
                        {
                            goto IL_0181;
                        }
                        goto IL_04d3;
                    IL_0181:
                        num = 27;
                        if (!Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                        {
                            goto IL_01b1;
                        }
                        goto IL_04d3;
                    IL_01b1:
                        num = 30;
                        Modul1_J = 0;
                        goto IL_01bb;
                    IL_01bb:
                        num = 31;
                        Modul1.Kont1[Modul1_J] = "";
                        goto IL_01cf;
                    IL_01cf:
                        num = 32;
                        Modul1_J = checked((byte)unchecked((uint)(Modul1_J + 1)));
                        if (Modul1_J <= 15u)
                        {
                            goto IL_01bb;
                        }
                        goto IL_01e9;
                    IL_01e9:
                        num = 33;
                        Modul1.sDatu = "";
                        goto IL_01f7;
                    IL_01f7:
                        num = 34;
                        if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != Modul1.PersInArb) | (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() != 105)))
                        {
                            goto IL_026a;
                        }
                        goto IL_0282;
                    IL_026a:
                        num = 35;
                        DataModul.DB_EventTable.Index = "ArtNr";
                        goto IL_04fd;
                    IL_0282:
                        num = 38;
                        if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                        {
                            goto IL_02b4;
                        }
                        goto IL_02eb;
                    IL_02b4:
                        num = 39;
                        Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                        goto IL_02db;
                    IL_02db:
                        num = 40;
                        Modul1.Kont1[1] = Modul1.sDatu;
                        goto IL_02eb;
                    IL_02eb:
                        num = 42;
                        Modul1.UbgT = "";
                        goto IL_02f9;
                    IL_02f9:
                        num = 43;
                        if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value))
                        {
                            goto IL_0325;
                        }
                        goto IL_03e0;
                    IL_0325:
                        num = 44;
                        if (DataModul.DB_EventTable.Fields[EventFields.ArtText].Value.AsDouble() > 0.0)
                        {
                            goto IL_035a;
                        }
                        goto IL_03e0;
                    IL_035a:
                        num = 45;
                        AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                        LD = "";
                        Modul1.Kont[0] = DataModul.TextLese1(AAA);
                        goto IL_03a1;
                    IL_03a1:
                        num = 46;
                        if (Modul1.Kont[0] != "")
                        {
                            goto IL_03ba;
                        }
                        goto IL_03e0;
                    IL_03ba:
                        num = 47;
                        Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim() + ": ";
                        goto IL_03e0;
                    IL_03e0:
                        num = 51;
                        if (Modul1.DAus[103].AsBool())
                        {
                            goto IL_03f3;
                        }
                        goto IL_044b;
                    IL_03f3:
                        num = 52;
                        text = Modul1.Kont1[1] + Modul1.Kont1[7] + new string(' ', 240).Left(240) + DataModul.DB_EventTable.Fields[EventFields.LfNr].AsString();
                        goto IL_04a5;
                    IL_044b:
                        num = 54;
                        goto IL_044f;
                    IL_044f:
                        num = 55;
                        text = Modul1.Kont1[7] + Modul1.Kont1[1] + new string(' ', 240).Left(240) + DataModul.DB_EventTable.Fields[EventFields.LfNr].AsString();
                        goto IL_04a5;
                    IL_04a5:
                        num = 57;
                        if (text.Trim() != "")
                        {
                            goto IL_04bd;
                        }
                        goto IL_04d3;
                    IL_04bd:
                        num = 58;
                        List2.Items.Add(text);
                        goto IL_04d3;
                    IL_04d3:
                        num = 60;
                        lErl = 299;
                        goto IL_04de;
                    end_IL_0000:
                        break;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 1659;
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
        string text2 = default;
        string value = default;
        int num6 = default;
        byte b = default;
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
                    object[] array;
                    int ortNr;
                    byte Schalt;
                    int Nr;
                    short LfNR;
                    int AAA;
                    string LD;
                    EEventArt _eArt;
                    short Listart;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            text = "";
                            goto IL_0009;
                        case 5129:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_1113;
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
                                goto IL_1117;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            num = 2;
                            Job = "";
                            text2 = "";
                            value = "";
                            array = new object[6];
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            num6 = List2.Items.Count - 1;
                            num5 = 0;
                            goto IL_10a8;
                        IL_0153: // <========== 3
                            num = 17;
                            if (!(Modul1_iPrivacy.AsDouble() > Modul1.iPriv_aus.AsDouble()))
                            {
                                if (Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                                {
                                    Interaction.MsgBox("Stop 14");
                                }
                                Modul1_J = 0;
                                while (unchecked(Modul1_J) <= 15u)
                                {
                                    Modul1.Kont1[Modul1_J] = "";
                                    Modul1_J = (byte)unchecked((uint)(Modul1_J + 1));
                                }
                                Modul1.sDatu = "";
                                if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != Modul1.PersInArb) | (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() != 105)))
                                {
                                    DataModul.DB_EventTable.Index = "ArtNr";
                                    goto end_IL_0000_2;
                                }
                                if (DataModul.DB_EventTable.Fields[EventFields.KBem].Value.AsDouble() > 0.0)
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                    LD = "";
                                    Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                    if (Modul1.Kont[0] != "")
                                    {
                                        Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim();
                                    }
                                }
                                goto IL_0337;
                            }
                            goto IL_1080;
                        IL_0337: // <========== 3
                            num = 37;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                            {
                                Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                Modul1.sDatu = "00000000" + Modul1.sDatu.Right(8);
                                text2 = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                if (Conversions.ToBoolean(Operators.CompareString(text2.Trim(), "", TextCompare: false) == 0
                                       & DataModul.DB_EventTable.Fields[EventFields.DatumB].AsDate() != default))
                                {
                                    text2 = " von";
                                }
                                Modul1.sDatu = Modul1.Datwand1(Modul1.sDatu, text2);
                                Modul1.Kont1[1] = Modul1.sDatu;
                                Modul1.sDatu = "";
                            }
                            goto IL_0459;
                        IL_0459:
                            num = 48;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                Modul1.sDatu = "00000000" + Modul1.sDatu.Right(8);
                                text2 = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                Modul1.Datwand1(ref Modul1.sDatu, text2);
                                if (Strings.InStr(Modul1.Kont1[1], "zwischen") != 0)
                                {
                                    if (Modul1.sDatu.Trim() != "")
                                    {
                                        Modul1.sDatu = " und " + Modul1.sDatu.Trim();
                                    }
                                }
                            }
                            goto IL_0556;
                        IL_0556: // <========== 3
                            num = 61;
                            Modul1.Kont1[3] = Modul1.sDatu;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.DatumText].Value))
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt() > 0)
                                {
                                    value = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsString();
                                    AAA = value.AsInt();
                                    LD = "";
                                    Modul1.UbgT = DataModul.TextLese1(AAA);
                                    value = AAA.AsString();
                                    if (Modul1.UbgT.Trim() != "")
                                    {
                                        Modul1.Kont1[3] = Modul1.Kont1[3] + " (" + Modul1.UbgT.Trim() + ")";
                                        Modul1.UbgT = "";
                                    }
                                }
                            }
                            goto IL_0661;
                        IL_0661: // <========== 3
                            num = 72;
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                            {
                                ortNr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble());
                                Modul1.ortles(ortNr, 1);
                                Modul1.Kont1[5] = " " + Modul1.UbgT.Trim();
                                Modul1.UbgT = "";
                                if (DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString() != "")
                                {
                                    Modul1.Kont1[5] = (Modul1.Kont1[5] + " " + DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString());
                                }
                            }
                            goto IL_0763;
                        IL_0763: // <========== 3
                            num = 80;
                            if (DataModul.DB_EventTable.Fields[EventFields.Platz].Value.AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                LD = "";
                                Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                if (Modul1.Kont[0] != "")
                                {
                                    Modul1.Kont1[6] = " " + Modul1.Kont[0].Trim();
                                }
                            }
                            goto IL_081b;
                        IL_081b: // <========== 3
                            num = 86;
                            if (Modul1.DAus[62] == "1")
                            {
                                Nr = Modul1.PersInArb;
                                _eArt = EEventArt.eA_105;
                                LfNR = Modul1.PrintDat.LfNR;
                                Modul1.PrintDat.QuellenDatum(ref Nr, _eArt, ref LfNR);
                                Modul1.PrintDat.LfNR = Conversions.ToByte(LfNR);
                                Modul1.PersInArb = Nr.AsInt();
                            }
                            if (((Modul1.DAus[38] == "1") & !M1_Ki) | ((Modul1.DAus[42] == "1") & M1_Ki))
                            {
                                Job = Modul1.Kont1[7].TrimEnd();
                                goto IL_093e;
                            }
                            if (((Modul1.DAus[39] == "1") & !M1_Ki) | ((Modul1.DAus[43] == "1") & M1_Ki))
                            {
                                Job = "";
                                Job = Module2.Jobdreh(Job);
                                Job += text;
                                text = "";
                            }
                            goto IL_093e;
                        IL_093e: // <========== 3
                            num = 98;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value))
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.ArtText].Value.AsDouble() > 0.0)
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                                    LD = "";
                                    Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                    if (Modul1.Kont[0] != "")
                                    {
                                        Modul1.Kont1[10] = Modul1.Kont[0].Trim() + ": ";
                                    }
                                }
                            }
                            goto IL_0a23;
                        IL_0a23: // <========== 3
                            num = 106;
                            if (Modul1.Kont1[10].Trim() != "")
                            {
                                if (M1_Ki & (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0))
                                {
                                    if (Modul1.DAus[106] == "1")
                                    {
                                        Anz[0].SelectedText = "\n";
                                    }
                                }
                                goto IL_0ab2;
                            }
                            goto IL_0bc0;
                        IL_0ab2: // <========== 3
                            num = 112;
                            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                            {
                                if (Modul1.DAus[106] == "1")
                                {
                                    Anz[0].SelectedText = "\n";
                                    goto IL_0b3e;
                                }
                                leerweg();
                                Anz[0].SelectedText = " ";
                            }
                            goto IL_0b3e;
                        IL_0b3e: // <========== 3
                            num = 121;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                            Anz[0].SelectedText = Modul1.Kont1[10].Trim();
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            goto IL_0bc0;
                        IL_0bc0: // <========== 3
                            num = 125;
                            leerweg();
                            Anz[0].SelectedText = " " + Job + ". ";
                            if (((Modul1.DAus[40] == "1") & !M1_Ki) | ((Modul1.DAus[44] == "1") & M1_Ki))
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim();
                                    Bemaus(Anz[0], Modul1.UbgT1);
                                }
                            }
                            goto IL_0ca6;
                        IL_0ca6: // <========== 3
                            num = 133;
                            if (((Modul1.DAus[41] == "1") & !M1_Ki) | ((Modul1.DAus[45] == "1") & M1_Ki))
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim();
                                    Bemaus(Anz[0], Modul1.UbgT1);
                                }
                            }
                            goto IL_0d5f;
                        IL_0d5f: // <========== 3
                            num = 139;
                            if (Modul1.Kont1[9].Trim() != "")
                            {
                                Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                Anz[0].SelectedText = Modul1.Kont1[9];
                                Anz[0].SelectionCharOffset = 0;
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            }
                            b = 0;
                            if (!M1_Ki)
                            {
                                b = Conversions.ToByte(Modul1.DAus[96]);
                            }
                            if (M1_Ki)
                            {
                                b = Conversions.ToByte(Modul1.DAus[98]);
                            }
                            Modul1.PersSp = Modul1.PersInArb;
                            if (b == 1)
                            {
                                _eArt = EEventArt.eA_105;
                                Modul1.Zeugsu(_eArt, Modul1.PrintDat.LfNR, 2, Ahne);
                            }
                            text3 = Modul1.Kont1[20];
                            Modul1.Kont1[20] = "";
                            Modul1.PersInArb = Modul1.PersSp;
                            if (b == 1)
                            {
                                if (text3 != "")
                                {
                                    if (Modul1.DAus[100] == "1")
                                    {
                                        M_Sgg = 0.8f;

                                    }
                                    else
                                    {
                                        M_Sgg = 1f;
                                    }
                                    goto IL_0f7e;
                                }
                            }
                            goto IL_1080;
                        IL_0f7e: // <========== 3
                            num = 168;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Underline);
                            Anz[0].SelectedText = "Zeugen:";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Regular);
                            Anz[0].SelectedText = " " + text3.Trim() + ".";
                            text3 = "";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            goto IL_1080;
                        IL_1080: // <========== 4
                            num = 176;
                            lErl = 2;
                            DataModul.DB_EventTable.MoveNext();
                            num5++;
                            goto IL_10a8;
                        IL_10a8:
                            if (num5 > num6)
                            {
                                goto end_IL_0000_2;
                            }
                            Modul1.PrintDat.LfNR = Conversions.ToByte(List2.Items[num5].AsString().Right(10));
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", "105", Modul1.PersInArb.AsString(), Modul1.PrintDat.LfNR);
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields["priv"].Value))
                            {
                                Modul1_iPrivacy = DataModul.DB_EventTable.Fields["priv"].AsInt();

                            }
                            else
                            {
                                Modul1_iPrivacy = 0;
                            }
                            goto IL_0153;
                        IL_1113:
                            num4 = unchecked(num2 + 1);
                            goto IL_1117;
                        IL_1117:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 13:
                                case 16:
                                case 17:
                                    goto IL_0153;
                                case 35:
                                case 36:
                                case 37:
                                    goto IL_0337;
                                case 47:
                                case 48:
                                    goto IL_0459;
                                case 54:
                                case 58:
                                case 59:
                                case 60:
                                case 61:
                                    goto IL_0556;
                                case 69:
                                case 70:
                                case 71:
                                case 72:
                                    goto IL_0661;
                                case 78:
                                case 79:
                                case 80:
                                    goto IL_0763;
                                case 84:
                                case 85:
                                case 86:
                                    goto IL_081b;
                                case 91:
                                case 97:
                                case 98:
                                    goto IL_093e;
                                case 103:
                                case 104:
                                case 105:
                                case 106:
                                    goto IL_0a23;
                                case 110:
                                case 111:
                                case 112:
                                    goto IL_0ab2;
                                case 115:
                                case 119:
                                case 120:
                                case 121:
                                    goto IL_0b3e;
                                case 124:
                                case 125:
                                    goto IL_0bc0;
                                case 131:
                                case 132:
                                case 133:
                                    goto IL_0ca6;
                                case 137:
                                case 138:
                                case 139:
                                    goto IL_0d5f;
                                case 164:
                                case 167:
                                case 168:
                                    goto IL_0f7e;
                                case 18:
                                case 174:
                                case 175:
                                case 176:
                                    goto IL_1080;
                                case 29:
                                case 179:
                                case 185:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 5129;
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
    private void Ahnen_FormClosing(object eventSender, FormClosingEventArgs eventArgs)
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
                            goto IL_00b3;
                        }
                    IL_00b3:
                        num = 15;
                        if (Information.Err().Number != 91)
                        {
                            break;
                        }
                        goto IL_00c5;
                    IL_00c5:
                        num = 16;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_00f3;
                    IL_009b:
                        num = 11;
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
                                goto IL_00b3;
                            case 16:
                                goto IL_00c5;
                            case 17:
                            case 19:
                                goto end_IL_0000_3;
                            default:
                                goto end_IL_0000;
                            case 12:
                            case 13:
                            case 14:
                            case 20:
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
                        FileSystem.FileOpen(6, Modul1.GenFreeDir + "\\Init\\Windowstate", OpenMode.Output);
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
                num = 19;
                eventArgs.Cancel = cancel;
                break;
            end_IL_0000:;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 341;
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
        PerSp1 = persInArb;
        List4.Items.Clear();
        foreach (var cLink in DataModul.Link.ReadAllPers(persInArb, ELinkKennz.lkGodparent))
        {
            persInArb = cLink.iFamNr;
            DataModul.DB_EventTable.Index = "ArtNr";
            DataModul.DB_EventTable.Seek("=", 102.AsString(), persInArb.AsString(), "0");
            if (DataModul.DB_EventTable.NoMatch)
            {
                DataModul.DB_EventTable.Seek("=", 101.AsString(), persInArb.AsString(), "0");
            }
            string text = DataModul.DB_EventTable.NoMatch ? "          " : ((!(Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)) ? "          " : Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8));
            List4.Items.Add(text + ("          " + persInArb.AsString()).Right(10));
        }
        checked
        {
            if (List4.Items.Count > 0)
            {
                if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                {
                    Anz[0].SelectedText = "\n";
                }
                int num = List4.Items.Count - 1;
                for (int i = 0; i <= num; i++)
                {
                    persInArb = (int)Math.Round(Conversion.Val(List4.Items[i].AsString().Right(10)));
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    DataModul.DB_EventTable.Index = "ArtNr";
                    DataModul.DB_EventTable.Seek("=", 102.AsString(), persInArb.AsString(), "0");
                    if (DataModul.DB_EventTable.NoMatch)
                    {
                        DataModul.DB_EventTable.Seek("=", 101.AsString(), persInArb.AsString(), "0");
                    }
                    string text;
                    if (DataModul.DB_EventTable.NoMatch)
                    {
                        text = "          ";
                    }
                    else if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                    {
                        text = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                        string ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                        text = Modul1.Datwand1(text, ds);
                    }
                    else
                    {
                        text = "          ";
                    }
                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                    if (!Modul1.PrintDat.Pat)
                    {
                        Anz[0].SelectedText = "Pate:";
                    }
                    else
                    {
                        if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == ".")
                        {
                            Anz[0].SelectionStart = Anz[0].SelectionStart - 1;
                            Anz[0].SelectionLength = 1;
                            Anz[0].SelectedText = ",";
                        }
                        Anz[0].SelectedText = " ";
                    }
                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                    if (Modul1.DAus[76] == "1")
                    {
                        Anz[0].SelectedText = " " + text.Trim() + " bei  <" + persInArb.AsString().Trim() + "> " + Modul1.Person.Prae + Modul1.Kont[3].Trim() + " " + Modul1.Kont[0].Trim() + ".";
                        Modul1.PrintDat.Pat = true;
                    }
                    else
                    {
                        Anz[0].SelectedText = " " + text.Trim() + " bei " + Modul1.Person.Prae + Modul1.Kont[3].Trim() + " " + Modul1.Kont[0].Trim() + ".";
                        Modul1.PrintDat.Pat = true;
                    }
                }
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
        }
    }

    public void Kinder(string Ahn)
    {
        M1_Ki = true;
        Modul1.UbgT1 = Modul1.PrintDat.Retweg(Modul1.UbgT1);
        Anz[0].SelectedText = "\n";
        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        Anz[0].SelectionIndent = 40;
        Anz[0].SelectionHangingIndent = 15;
        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        Modul1.Famles();
        List1.Items.Clear();
        byte b = 1;
        checked
        {
            while (Modul1.Family.Kind[b] != 0)
            {
                DataModul.DB_EventTable.Seek("=", 101, Modul1.Family.Kind[b].AsString(), "0");
                if (!DataModul.DB_EventTable.NoMatch)
                {
                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                    {
                        Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                    }
                    else
                    {
                        DataModul.DB_EventTable.Seek("=", 102.AsString(), Modul1.Family.Kind[b].AsString(), "0");
                        if (DataModul.DB_EventTable.NoMatch)
                        {
                            Modul1.sDatu = 0.AsString();
                        }
                        else
                        {
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields["priv"].Value))
                            {
                                Modul1_iPrivacy = DataModul.DB_EventTable.Fields["priv"].AsInt();
                            }
                            else
                            {
                                Modul1_iPrivacy = 0;
                            }
                            if (Modul1_iPrivacy.AsDouble() <= Modul1.iPriv_aus.AsDouble() && !DataModul.DB_EventTable.NoMatch)
                            {
                                Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                            }
                        }
                    }
                }
                else
                {
                    DataModul.DB_EventTable.Seek("=", 102.AsString(), Modul1.Family.Kind[b].AsString(), "0");
                    if (DataModul.DB_EventTable.NoMatch)
                    {
                        Modul1.sDatu = 0.AsString();
                    }
                    else if (!Information.IsDBNull(DataModul.DB_EventTable.Fields["priv"].Value))
                    {
                        Modul1_iPrivacy = DataModul.DB_EventTable.Fields["priv"].AsInt();
                    }
                    else
                    {
                        Modul1_iPrivacy = 0;
                    }
                    if (Modul1_iPrivacy.AsDouble() <= Modul1.iPriv_aus.AsDouble() && !DataModul.DB_EventTable.NoMatch)
                    {
                        Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                    }
                }
                List1.Items.Add(Strings.Right("        " + Modul1.sDatu.AsDouble().AsString(), 8) + Modul1.Family.Kind[b].AsString());
                b = (byte)unchecked((uint)(b + 1));
                if (unchecked(b) > 99u)
                {
                    break;
                }
            }
            if (Anz[0].Text != "" && Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
            {
                Anz[0].SelectedText = "\n";
            }
            Anz[0].SelectionIndent = 40;
            Anz[0].SelectionHangingIndent = 15;
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            if (b > 2)
            {
                Anz[0].SelectedText = "Kinder:  \n";
            }
            else
            {
                Anz[0].SelectedText = "Kind:  \n";
            }
            Anz[0].SelectionIndent = 40;
            Anz[0].SelectionHangingIndent = 15;
            if (List1.Items.Count <= 0)
            {
                return;
            }
            byte b2 = (byte)(List1.Items.Count - 1);
            b = 0;
            byte b3 = default;
            while (unchecked(b <= (uint)b2))
            {
                Anz[0].SelectionIndent = 40;
                Anz[0].SelectionHangingIndent = 15;
                Modul1.PersInArb = Conversions.ToInteger(Strings.Mid(List1.Items[b].AsString(), 9, List1.Items[b].AsString().Length));
                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                Modul1.PrintDat.Namenindex(this.Ahne);
                string text = "";
                List3.Items.Clear();
                DataModul.NB_AhnTable.Index = "PerNr";
                DataModul.NB_AhnTable.Seek("=", Modul1.PersInArb);
                while (!DataModul.NB_AhnTable.EOF)
                {
                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                    if (!DataModul.NB_AhnTable.NoMatch)
                    {
                        if (DataModul.NB_AhnTable.Fields["PerNr"].AsInt() != Modul1.PersInArb)
                        {
                            break;
                        }
                        if (DataModul.NB_AhnTable.Fields["Ahn"].AsInt() != 0)
                        {
                            List3.Items.Add(DataModul.NB_AhnTable.Fields["Ahn"].Value);
                            text = " (siehe Ahn " + DataModul.NB_AhnTable.Fields["Ahn"].AsDouble().AsString().Trim() + ") ";
                            b3 = 1;
                        }
                    }
                    DataModul.NB_AhnTable.MoveNext();
                }
                if (List3.Items.Count > 1)
                {
                    int count = List3.Items.Count;
                    for (int i = 0; i <= count; i++)
                    {
                        if (Strings.InStr(Strings.Trim(List3.Items[b].AsString()), "+") != 0)
                        {
                            Interaction.MsgBox(Strings.Trim(Conversion.Str(List3.Items[0].AsString().AsDouble())));
                        }
                        text = " (siehe Ahn " + Strings.Trim(Conversion.Str(List3.Items[0].AsString().AsDouble())) + " und weitere) ";
                        b3 = 1;
                    }
                }
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                if (Modul1.DAus[76] == "1")
                {
                    Anz[0].SelectedText = Conversion.Str((unchecked(b) + 1).AsDouble()) + ".) [" + Modul1.PersInArb.AsString().Trim() + "] " + Modul1.Person.Prae + Modul1.Kont[3];
                }
                else
                {
                    Anz[0].SelectedText = Conversion.Str((unchecked(b) + 1).AsDouble()) + ".) " + Modul1.Kont[3];
                }
                if (Modul1.Druck_Tast == 2)
                {
                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                    if (Modul1.DAus[89] == "1")
                    {
                        Anz[0].SelectedText = " " + Modul1.Kont[0].ToUpper();
                    }
                    else
                    {
                        Anz[0].SelectedText = " " + Modul1.Kont[0];
                    }
                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                }
                if (Modul1.Kont[2].Trim() != "")
                {
                    Anz[0].SelectedText = " " + Modul1.Kont[2];
                }
                if (Modul1.Kont[4].Trim() != "")
                {
                    Anz[0].SelectedText = " (" + Modul1.Kont[4] + ")";
                }
                if (Modul1.Kont[5].Trim() != "")
                {
                    Anz[0].SelectedText = ", Sippe " + Modul1.Kont[5];
                }
                Anz[0].SelectedText = text;
                if (Modul1.DAus[90].AsDouble() != 0.0)
                {
                    text = "";
                }
                if (text.Trim() == "")
                {
                    b3 = 0;
                    if ((Modul1.DAus[115] == "1") | (Modul1.DAus[116] == "1"))
                    {
                        Module2.Bildaus("P", "Ahnen");
                    }
                    if (Modul1.DAus[88] == "1")
                    {
                        Bild("P", Modul1.PersInArb);
                    }
                    if (Modul1.DAus[62] == "1")
                    {
                        int FamPer = 1;
                        Modul1.PrintDat.PerQu(ref FamPer);
                    }
                    if (Modul1.Kont[30] != "")
                    {
                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                        Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                        Anz[0].SelectedText = Modul1.Kont[30];
                        Anz[0].SelectionCharOffset = 0;
                    }
                    if (Modul1.DAus[106] == "1")
                    {
                        Anz[0].SelectedText = "\n";
                    }
                    Anz[0].SelectionIndent = 50;
                    M1_Ki = true;
                    PerSp1 = Modul1.PersInArb;
                    Datschreib();
                    if (Modul1.DAus[67] == "1")
                    {
                        Modul1.Datschalt = 1;
                        short Listart = 0;
                        string Ahne = 0.AsString();
                        bool neb = false;
                        Modul1.Datles3(Listart, default, default, ref neb);
                        Modul1.Datschalt = 0;
                        if (Modul1.Kont[25] != "")
                        {
                            Anz[0].SelectedText = " " + Modul1.Kont[25].Trim();
                        }
                    }
                    if ((Modul1.DAus[42] == "1") | (Modul1.DAus[43] == "1"))
                    {
                        Sonst();
                    }
                    if ((Modul1.DAus[24] == "1") | (Modul1.DAus[25] == "1"))
                    {
                        Modul1.Ubg = 300;
                        bool neb = false;
                        Berufles1(ref neb, (EEventArt)Modul1.Ubg);
                    }
                    if ((Modul1.DAus[28] == "1") | (Modul1.DAus[29] == "1"))
                    {
                        Modul1.Ubg = 301;
                        bool neb = false;
                        Berufles1(ref neb, (EEventArt)Modul1.Ubg);
                    }
                    if ((Modul1.DAus[32] == "1") | (Modul1.DAus[33] == "1"))
                    {
                        Modul1.Ubg = 302;
                        bool neb = false;
                        Berufles1(ref neb, (EEventArt)Modul1.Ubg);
                    }
                    M1_Ki = true;
                    Modul1.PersInArb = PerSp1;
                }
                Anz[0].SelectedText = "\n";
                Anz[0].SelectionIndent = 50;
                if (b3 != 1)
                {
                    Perabschluss(Modul1.PersInArb, Modul1.FamInArb);
                }
                M1_Ki = false;
                b3 = 0;
                if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                {
                    Anz[0].SelectedText = "\n";
                }
                Anz[0].SelectionCharOffset = 0;
                b = (byte)unchecked((uint)(b + 1));
            }
        }
    }

    public void T2(long Ahn)
    {
        M1_Ki = false;
        Anz[0].SelectionIndent = 20;
        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        Anz[0].SelectedText = "\n";
        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        if ((Modul1.DAus[20] == "1") | (Modul1.DAus[21] == "1"))
        {
            short Listart = 0;
            Modul1.PrintDat.FWohn(EEventArt.eA_602, ref Listart);
            Famwohn(EEventArt.eA_602);
        }
        if ((Modul1.DAus[20] == "1") | (Modul1.DAus[21] == "1"))
        {
            short Listart = 0;
            Modul1.PrintDat.FWohn(EEventArt.eA_603, ref Listart);
            Famwohn(EEventArt.eA_603);
        }
        if (Modul1.DAus[62] == "1")
        {
            int FamPer = 2;
            Modul1.PrintDat.PerQu(ref FamPer);
        }
        if (Modul1.Kont[30].Trim() != "")
        {
            Anz[0].SelectionIndent = 40;
            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
            {
                Anz[0].SelectedText = "\n";
            }
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            Anz[0].SelectionIndent = 40;
            Anz[0].SelectedText = "Quellen zur Familie: ";
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
            Anz[0].SelectedText = Modul1.Kont[30];
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        }
        if (FamBeh > 0 && Modul1.DAus[4] == "1")
        {
            DataModul.DB_FamilyTable.Index = "Fam";
            DataModul.DB_FamilyTable.Seek("=", FamBeh);
            if (Operators.CompareString(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
            {
                Modul1.UbgT1 = Modul1.PrintDat.Retweg(Modul1.UbgT1);
                if (Modul1.DAus[72] == "1")
                {
                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
                }
                else
                {
                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                }
                if (Modul1.DTxt[17].Trim() != "")
                {
                    Anz[0].SelectedText = Modul1.DTxt[17] + ": ";
                }
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                Anz[0].SelectedText = " ";
                Modul1.UbgT1 = DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].AsString();
                Bemaus(Anz[0], Modul1.UbgT1);
                Modul1.UbgT1 = "";
            }
        }
        FamBeh = 0;
        checked
        {
            if (Modul1.PrintDat.BemZahl > 0)
            {
                if (Operators.CompareString(Anz[0].Text.Trim().Right(2), "\n\n", TextCompare: false) != 0)
                {
                    Anz[0].SelectedText = "\n";
                }
                Anz[0].SelectionIndent = 0;
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                Anz[0].SelectedText = "Quellenverweise:";
                short num = (short)Modul1.PrintDat.BemZahl;
                for (short num2 = 1; num2 <= num; num2 = (short)unchecked(num2 + 1))
                {
                    if ((Modul1.DAus[71] == "1") & (num2 <= Modul1.PrintDat.BemZahl))
                    {
                        Anz[0].SelectedText = "\n";
                    }
                    Modul1.UbgT1 = Modul1.PrintDat.KontBem[num2];
                    Modul1.UbgT1 = Modul1.PrintDat.Retweg(Modul1.UbgT1);
                    Modul1.UbgT1 = Modul1.UbgT1.Left(Modul1.UbgT1.Length - 1) + Modul1.UbgT1.Right(1).Replace("\n", "");
                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                    Anz[0].SelectedText = " " + Modul1.UbgT1;
                    Modul1.UbgT1 = "";
                }
                Anz[0].SelectedText = "\n";
            }
            Modul1.PrintDat.BemZahl = 0;
            List1.Items.Clear();
            DataModul.NB_AhnTable.Index = "Ahnen";
            string text = Strings.Right(new string(' ', 40) + Conversion.Str(decimal.Add(new decimal(Ahn), 1m)), 40);
            DataModul.NB_AhnTable.Seek(">=", text);
            if (Modul1.Druck_Tast == 2)
            {
                DataModul.NB_AhnTable.Index = "Spitz";
                DataModul.NB_AhnTable.Seek(">=", "1", text);
            }
            if (Anz[0].SelectionStart > 2)
            {
                if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart - 2, 2) != "\n\n")
                {
                    Anz[0].SelectedText = "\n";
                }
            }
            else
            {
                Anz[0].SelectedText = "\n";
            }
            Anz[0].SelectionIndent = 0;
        }
    }

    public void Heidat()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string text = default;
        string text2 = default;
        bool flag = default;
        EEventArt num5 = default;
        EEventArt Art = default;
        string Job = default;
        byte b2 = default;
        short Listart = default;
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
                    int ortNr;
                    int AAA;
                    string LD;
                    int Nr;
                    short LfNR;
                    byte Schalt;
                    int famInArb;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 7465:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_1873;
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
                                goto IL_1877;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            string ds = "";
                            text = "";
                            text2 = "";
                            Zeilenumbruch(Anz[0]);
                            Anz[0].SelectionIndent = 20;
                            Anz[0].SelectedText = "\n";
                            Anz[0].SelectionIndent = 60;
                            Anz[0].SelectionHangingIndent = 15;
                            leerweg();
                            flag = true;
                            Zeilenumbruch(Anz[0]);
                            if (Anz[0].Text != "")
                            {
                                if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
                                {
                                    Anz[0].SelectedText = "\n";
                                }
                            }
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            if (Modul1.DAus[76].AsDouble() == 1.0)
                            {
                                Anz[0].SelectedText = " [" + Modul1.FamInArb.AsString().Trim() + "]";
                                flag = false;
                            }
                            num5 = EEventArt.eA_500;
                            goto IL_01b1;
                        IL_01b1: // <========== 3
                            // Loop
                            num = 24;
                            if (num5 == EEventArt.eA_500)
                            {
                                Art = EEventArt.eA_501;
                            }
                            else
                            if (num5 == EEventArt.eA_501)
                            {
                                Art = EEventArt.eA_500;
                            }
                            else
                                Art = num5;
                            Datu1 = "";
                            byte b = 1;
                            while (unchecked(b) <= 8u)
                            {
                                Modul1.Kont1[b] = "";
                                b = (byte)unchecked((uint)(b + 1));
                            }
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", Art, Modul1.FamInArb.AsString(), "0");
                            if (DataModul.Event.ReadData(Art, Modul1.FamInArb, out var cEv))
                            {
                                if (!Information.IsDBNull(cEv.iPrivacy))
                                {
                                    Modul1_iPrivacy = cEv.iPrivacy;
                                }
                                else
                                    Modul1_iPrivacy = 0;

                                if (!(Modul1_iPrivacy.AsDouble() > Modul1.iPriv_aus.AsDouble()))
                                {
                                    if (cEv.dDatumV != default)
                                    {
                                        Datu1 = Strings.Right("00000000" + cEv.dDatumV.AsString().Trim(), 8);
                                        ds = cEv.sDatumV_S;
                                        Datu1 = Modul1.Datwand1(Datu1, ds);
                                        Modul1.Kont1[1] = Datu1;
                                    }
                                    if (cEv.dDatumB != default)
                                    {
                                        ds = cEv.sDatumB_S;
                                        Datu1 = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                        Datu1 = Strings.Right("00000000" + cEv.dDatumB.AsString().Trim(), 8);
                                        Datu1 = Modul1.Datwand1(Datu1, ds);
                                        if (cEv.dDatumV.AsDouble() > 0.0)
                                        {
                                            Modul1.Kont1[3] = "/" + Datu1;
                                        }
                                        else
                                            Modul1.Kont1[3] = " " + Datu1;
                                    }
                                    Modul1.UbgT = "";
                                    if (cEv.iOrt.AsDouble() > 0.0)
                                    {
                                        ortNr = (int)Math.Round(cEv.iOrt.AsDouble());
                                        Modul1.ortles(ortNr, 1);
                                        Modul1.Kont1[5] = Modul1.UbgT;
                                        if (cEv.sOrt_S != "")
                                        {
                                            Modul1.Kont1[5] = Modul1.Kont1[5] + " " + cEv.sOrt_S;
                                        }
                                    }
                                    if (cEv.iPlatz.AsDouble() > 0.0)
                                    {
                                        AAA = cEv.iPlatz;
                                        LD = "";
                                        Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                        if (Modul1.Kont[0] != "")
                                        {
                                            Modul1.Kont1[6] = " " + Modul1.Kont[0].Trim();
                                        }
                                    }
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt()) > 0.0)
                                    {
                                        AAA = cEv.iKBem;
                                        LD = "";
                                        Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                        if (Modul1.Kont[0] != "")
                                        {
                                            Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim();
                                        }
                                    }
                                    b = 1;
                                    while (unchecked(b) <= 6u)
                                    {
                                        if (Modul1.Kont1[b] == "0")
                                        {
                                            Modul1.Kont1[b] = "";
                                        }
                                        b = (byte)unchecked((uint)(b + 1));
                                    }
                                    if ((Modul1.Kont1[1].Trim() != "")
                                        | (Modul1.Kont1[2].Trim() != "")
                                        | (Modul1.Kont1[3].Trim() != "")
                                        | (Modul1.Kont1[5].Trim() != "")
                                        | (Modul1.Kont1[6].Trim() != "")
                                        | (Modul1.UbgT.Trim() != "")
                                        | ((cEv.sBem[1].Trim().Length > 0) & (Modul1.DAus[5] == "1"))
                                        | ((cEv.sBem[2].Trim().Length > 0) & (Modul1.DAus[6] == "1")))
                                    {
                                        text = Art switch
                                        {
                                            EEventArt.eA_501 when M1_Ki | Modul1.DAus[53] != "0" && !M1_Ki | Modul1.DAus[58] != "0" => Modul1.DTxt[6],
                                            EEventArt.eA_500 when M1_Ki | Modul1.DAus[52] != "0" && !M1_Ki | Modul1.DAus[57] != "0" => Modul1.DTxt[5],
                                            EEventArt.eA_Marriage when M1_Ki | Modul1.DAus[64] != "0" && !M1_Ki | Modul1.DAus[65] != "0" => Modul1.DTxt[7],
                                            EEventArt.eA_MarrReligious when M1_Ki | Modul1.DAus[54] != "0" && !M1_Ki | Modul1.DAus[59] != "0" => Modul1.DTxt[8],
                                            EEventArt.eA_504 when M1_Ki | Modul1.DAus[55] != "0" && !M1_Ki | Modul1.DAus[60] != "0" => Modul1.DTxt[9],
                                            EEventArt.eA_505 when M1_Ki | Modul1.DAus[56] != "0" && !M1_Ki | Modul1.DAus[61] != "0" => Modul1.DTxt[10],
                                            EEventArt.eA_507 when M1_Ki | Modul1.DAus[84] != "0" && !M1_Ki | Modul1.DAus[88] != "0" => Modul1.DTxt[15],
                                            _ => "",
                                        };
                                        if (text != "")
                                        {
                                            if ((Modul1.Kont1[1].Trim() != "")
                                            | (Modul1.Kont1[3].Trim() != "")
                                            | (Modul1.Kont1[5].Trim() != "")
                                            | (Modul1.Kont1[6].Trim() != "")
                                            | (Modul1.Kont1[7].Trim() != "")
                                            | (Modul1.Kont1[8].Trim() != "")
                                            | (Modul1.UbgT.Trim() != ""))
                                            {
                                                if (Anz[0].SelectionStart > 0)
                                                {
                                                    if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != " ")
                                                    {
                                                        Anz[0].SelectedText = " ";
                                                    }
                                                }
                                                else
                                                    Anz[0].SelectedText = " ";
                                                if (unchecked(Modul1.DAus[106].AsDouble() == 1.0 && flag))
                                                {
                                                    Anz[0].SelectedText = "\n";
                                                }
                                                flag = true;
                                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                                Job = "";
                                                Job = Module2.Jobdreh(Job);
                                                Anz[0].SelectedText = (text + " " + Job).Trim();
                                                if (Modul1.DAus[85] == "1")
                                                {
                                                    if (cEv.sReg.Trim() != "")
                                                    {
                                                        Anz[0].SelectedText = " Urkunde: " + cEv.sReg.Trim() + ";";
                                                    }
                                                }
                                            }

                                        }
                                        else goto IL_1767;
                                    }
                                    if (cEv.sBem[1].Trim().Length > 0)
                                    {
                                        if (Modul1.DAus[5] == "1")
                                        {
                                            if (text.Trim() != "")
                                            {
                                                if (Anz[0].SelectionStart > 2 && Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != " ")
                                                {
                                                    Anz[0].SelectedText = " ";
                                                }
                                                Anz[0].SelectedText = text + " ";
                                                text = "";
                                            }
                                            if (cEv.sBem[1].Trim() != "")
                                            {
                                                Modul1.UbgT1 = cEv.sBem[1].Trim();
                                                Bemaus(Anz[0], Modul1.UbgT1);
                                            }
                                        }
                                    }
                                    if (cEv.sBem[2].Trim().Length != 0 && Modul1.DAus[6] == "1")
                                    {
                                        if (text.Trim() != "")
                                        {
                                            if (Anz[0].Text.Length > 0)
                                            {
                                                if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != " ")
                                                {
                                                    Anz[0].SelectedText = " ";
                                                }
                                                Anz[0].SelectedText = " ";
                                            }
                                            Anz[0].SelectedText = text + " ";
                                            text = "";
                                        }
                                        Modul1.UbgT1 = cEv.sBem[2].Trim();
                                        Bemaus(Anz[0], Modul1.UbgT1);
                                    }
                                    Modul1.Kont1[9] = "";
                                    Nr = Modul1.FamInArb;
                                    LfNR = 0;
                                    Modul1.PrintDat.QuellenDatum(ref Nr, Art, ref LfNR);
                                    Modul1.FamInArb = Nr.AsInt();
                                    if (Modul1.Kont1[9].Trim() != "")
                                    {
                                        if (Anz[0].SelectionStart > 1 && Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == " ")
                                        {
                                            Anz[0].SelectionStart = Anz[0].SelectionStart - 1;
                                            Anz[0].SelectionLength = 1;
                                            Anz[0].SelectedText = "";
                                        }
                                        Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                        Anz[0].SelectedText = Modul1.Kont1[9].Trim() + " ";
                                        Modul1.Kont1[9] = "";
                                        Anz[0].SelectionCharOffset = 0;
                                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    }
                                    if (!M1_Ki)
                                    {
                                        b2 = (byte)Modul1.DAus[96].AsInt();
                                    }
                                    if (M1_Ki)
                                    {
                                        b2 = (byte)Modul1.DAus[98].AsInt();
                                    }
                                    if (b2 == 1)
                                    {
                                        string namen = M_Namen;
                                        Modul1.PersSp = Modul1.PersInArb;
                                        byte b3 = 1;
                                        while (unchecked(b3) <= 100u)
                                        {
                                            Modul1.PrintDat.KontSP1[b3] = Modul1.Kont1[b3];
                                            Modul1.PrintDat.KontSP[b3] = Modul1.Kont[b3];
                                            Modul1.Kont[b3] = "";
                                            Modul1.Kont1[b3] = "";
                                            b3 = (byte)unchecked((uint)(b3 + 1));
                                        }
                                        Schalt = 0;
                                        Modul1.Zeugsu(Art, 0, Listart, Ahne);
                                        M_Namen = namen;
                                        Modul1.PersInArb = Modul1.PersSp;
                                        text2 = Modul1.Kont1[20];
                                        b3 = 1;
                                        while (unchecked(b3) <= 100u)
                                        {
                                            Modul1.Kont1[b3] = Modul1.PrintDat.KontSP1[b3];
                                            Modul1.Kont[b3] = Modul1.PrintDat.KontSP[b3];
                                            Modul1.PrintDat.KontSP[b3] = "";
                                            Modul1.PrintDat.KontSP1[b3] = "";
                                            b3 = (byte)unchecked((uint)(b3 + 1));
                                        }
                                    }
                                    if (text2.Trim() != "")
                                    {
                                        leerweg();
                                        Anz[0].SelectionCharOffset = 0;
                                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                        if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != ".")
                                        {
                                            Anz[0].SelectedText = ".";
                                        }
                                        if (Modul1.DAus[100] == "1")
                                        {
                                            M_Sgg = 0.8f;
                                        }
                                        else
                                            M_Sgg = 1f;
                                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Regular);
                                        Anz[0].SelectedText = " Zeugen: " + text2;
                                        text2 = "";
                                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    }

                                }
                            }
                            goto IL_1767;
                        IL_1767: // <========== 12
                            num = 281;
                            lErl = 22;
                            num5++;
                            if (num5 <= EEventArt.eA_507)
                            {
                                goto IL_01b1;
                            }
                            famInArb = Modul1.FamInArb;
                            if ((Modul1.DAus[46] == "1") & !M1_Ki)
                            {
                                Tz = false;
                                Trauzeugen();
                            }
                            if (!((Modul1.DAus[50] == "1") & !M1_Ki))
                            {
                            }
                            else
                            {

                                Tz = true;
                                Trauzeugen();
                            }
                            goto end_IL_0000_2;
                        IL_1873:
                            num4 = unchecked(num2 + 1);
                            goto IL_1877;
                        IL_1877:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 24:
                                    goto IL_01b1;
                                case 40:
                                case 49:
                                case 101:
                                case 104:
                                case 110:
                                case 113:
                                case 119:
                                case 122:
                                case 128:
                                case 131:
                                case 137:
                                case 140:
                                case 146:
                                case 149:
                                case 155:
                                case 158:
                                case 280:
                                case 281:
                                    goto IL_1767;
                                case 291:
                                case 292:
                                case 297:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 7465;
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
    public void Datschreib()
    {
        Modul1.Datschalt = 1;
        short Listart = 0;
        Modul1.Datles10(ref Listart, M1_Ki);
        if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
        {
            if (Modul1.DAus[106] == "1")
            {
                Anz[0].SelectedText = "\n";
            }
            else
            {
                Anz[0].SelectedText = " ";
            }
        }
        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        if ((Modul1.Kont[11].Trim() != "") | (Modul1.Kont[16].Trim().Length > 0) | (Modul1.Kont[52].Trim() != "") | (Modul1.Kont[21].Trim().Length > 0) | (Modul1.Kont[31].Trim().Length > 0))
        {
            Anz[0].SelectedText = Modul1.DTxt[1];
            Anz[0].SelectedText = " " + Modul1.Kont[11].Trim() + ".";
            if (Modul1.Kont[31].Trim() != "")
            {
                Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                Anz[0].SelectedText = Modul1.Kont[31].Trim();
                Anz[0].SelectionCharOffset = 0;
            }
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            if (Modul1.DAus[85] == "1" && Modul1.Kont[41].Trim() != "")
            {
                Anz[0].SelectedText = " Urkunde: " + Modul1.Kont[41].Trim();
            }
            if (Modul1.DAus[8] == "1" && Modul1.Kont[16].Trim().Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[16];
                Bemaus(Anz[0], Modul1.UbgT1);
            }
            if (Modul1.DAus[9] == "1" && Modul1.Kont[21].Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[21];
                Bemaus(Anz[0], Modul1.UbgT1);
            }
            if (Modul1.Kont[51].Trim() != "")
            {
                Anz[0].SelectedText = " ";
                if (Modul1.DAus[100] == "1")
                {
                    M_Sgg = 0.8f;
                }
                else
                {
                    M_Sgg = 1f;
                }
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Underline);
                Anz[0].SelectedText = "Zeugen:";
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Regular);
                Anz[0].SelectedText = " " + Modul1.Kont[51].Trim() + ".";
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
        }
        if ((Modul1.Kont[12].Trim() != "") | (Modul1.Kont[17].Trim().Length > 0) | (Modul1.Kont[52].Trim() != "") | (Modul1.Kont[22].Trim().Length > 0) | (Modul1.Kont[32].Trim().Length > 0))
        {
            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
            {
                if (Modul1.DAus[106] == "1")
                {
                    Anz[0].SelectedText = "\n";
                }
                else
                {
                    Anz[0].SelectedText = " ";
                }
            }
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            Anz[0].SelectedText = Modul1.DTxt[2];
            Anz[0].SelectedText = " " + Modul1.Kont[12].Trim() + ".";
            if (Modul1.Kont[32].Trim() != "")
            {
                Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                Anz[0].SelectedText = Modul1.Kont[32].Trim();
                Anz[0].SelectionCharOffset = 0;
            }
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            if (Modul1.DAus[85] == "1" && Modul1.Kont[42].Trim() != "")
            {
                Anz[0].SelectedText = " Urkunde: " + Modul1.Kont[42].Trim();
            }
            if (Modul1.DAus[8] == "1" && Modul1.Kont[17].Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[17];
                Bemaus(Anz[0], Modul1.UbgT1);
            }
            if (Modul1.DAus[9] == "1" && Modul1.Kont[22].Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[22];
                Bemaus(Anz[0], Modul1.UbgT1);
            }
            if (Modul1.Kont[52].Trim() != "")
            {
                Anz[0].SelectedText = " ";
                if (Modul1.DAus[100] == "1")
                {
                    M_Sgg = 0.8f;
                }
                else
                {
                    M_Sgg = 1f;
                }
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Underline);
                Anz[0].SelectedText = "Zeugen:";
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Regular);
                Anz[0].SelectedText = " " + Modul1.Kont[52].Trim() + ".";
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
        }
        PerSp1 = Modul1.PersInArb;
        if (Modul1.DAus[48] == "1")
        {
            Paten2(Modul1.FamInArb, GetPerSp1());
            Modul1.PersInArb = PerSp1;
            if (Pattext != "")
            {
                Anz[0].SelectedText = " ";
                if (Modul1.DAus[100] == "1")
                {
                    M_Sgg = 0.8f;
                }
                else
                {
                    M_Sgg = 1f;
                }
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Underline);
                Anz[0].SelectedText = "Paten:";
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Regular);
                Pattext = Pattext.Trim();
                Pattext = Pattext.Left(checked(Pattext.Length - 1)) + Pattext.Right(1).Replace("\n", "");
                Anz[0].SelectedText = " " + Pattext.Trim() + ".";
                Pattext = "";
            }
        }
        Modul1.Datschalt = 3;
        Listart = 0;
        Modul1.Datles10(ref Listart, M1_Ki);
        if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
        {
            if (Modul1.DAus[106] == "1")
            {
                Anz[0].SelectedText = "\n";
            }
            else
            {
                Anz[0].SelectedText = " ";
            }
        }
        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        if ((Modul1.Kont[13].Trim() != "") | (Modul1.Kont[18].Trim().Length > 0) | (Modul1.Kont[53].Trim() != "") | (Modul1.Kont[23].Trim().Length > 0) | (Modul1.Kont[33].Trim().Length > 0))
        {
            Anz[0].SelectedText = Modul1.DTxt[3];
            Anz[0].SelectedText = " " + Modul1.Kont[13].Trim() + ".";
            if (Modul1.Kont[33].Trim() != "")
            {
                Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                Anz[0].SelectedText = Modul1.Kont[33].Trim();
                Anz[0].SelectionCharOffset = 0;
            }
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            if (Modul1.DAus[85] == "1" && Modul1.Kont[43].Trim() != "")
            {
                Anz[0].SelectedText = " Urkunde: " + Modul1.Kont[43].Trim();
            }
            if (Modul1.DAus[8] == "1" && Modul1.Kont[18].Trim().Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[18];
                Bemaus(Anz[0], Modul1.UbgT1);
            }
            if (Modul1.DAus[9] == "1" && Modul1.Kont[23].Trim().Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[23];
                Bemaus(Anz[0], Modul1.UbgT1);
            }
            if (Modul1.Kont[53].Trim() != "")
            {
                Anz[0].SelectedText = " ";
                if (Modul1.DAus[100] == "1")
                {
                    M_Sgg = 0.8f;
                }
                else
                {
                    M_Sgg = 1f;
                }
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Underline);
                Anz[0].SelectedText = "Zeugen:";
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Regular);
                Anz[0].SelectedText = " " + Modul1.Kont[53].Trim() + ".";
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
        }
        if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
        {
            if (Modul1.DAus[106] == "1")
            {
                Anz[0].SelectedText = "\n";
            }
            else
            {
                Anz[0].SelectedText = " ";
            }
        }
        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        if ((Modul1.Kont[14].Trim() != "") | (Modul1.Kont[19].Trim().Length > 0) | (Modul1.Kont[54].Trim() != "") | (Modul1.Kont[24].Trim().Length > 0) | (Modul1.Kont[34].Trim().Length > 0))
        {
            Anz[0].SelectedText = Modul1.DTxt[4];
            Anz[0].SelectedText = " " + Modul1.Kont[14].Trim() + ".";
            if (Modul1.Kont[34].Trim() != "")
            {
                Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                Anz[0].SelectedText = Modul1.Kont[34].Trim();
                Anz[0].SelectionCharOffset = 0;
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
            if (Modul1.DAus[85] == "1" && Modul1.Kont[44].Trim() != "")
            {
                Anz[0].SelectedText = " Urkunde: " + Modul1.Kont[44].Trim();
            }
            if (Modul1.DAus[8] == "1" && Modul1.Kont[19].Trim().Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[19];
                Bemaus(Anz[0], Modul1.UbgT1);
            }
            if (Modul1.DAus[9] == "1" && Modul1.Kont[24].Trim().Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[24];
                Bemaus(Anz[0], Modul1.UbgT1);
            }
            if (Modul1.Kont[54].Trim() != "")
            {
                Anz[0].SelectedText = " ";
                if (Modul1.DAus[100] == "1")
                {
                    M_Sgg = 0.8f;
                }
                else
                {
                    M_Sgg = 1f;
                }
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Underline);
                Anz[0].SelectedText = "Zeugen:";
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Regular);
                Anz[0].SelectedText = " " + Modul1.Kont[54].Trim() + ".";
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
        }
        Anz[0].SelectionCharOffset = 0;
        leerweg();
    }

    public void Zeilenumbruch(RichTextBox richTextBox)
    {
        string text = richTextBox.Text.Right(500);
        byte b2 = 1;
        do
        {
            byte b = 0;
            if (richTextBox.Text.Right(1) == " ")
            {
                richTextBox.SelectionStart = richTextBox.SelectionStart - 1;
                richTextBox.SelectionLength = 1;
                richTextBox.SelectedText = "";
                b = 1;
            }
            if (richTextBox.Text.Right(1) == "\n")
            {
                richTextBox.SelectionStart = richTextBox.SelectionStart - 1;
                richTextBox.SelectionLength = 1;
                richTextBox.SelectedText = "";
                b = 1;
            }
            if (b == 0)
            {
                break;
            }
            b2 = (byte)unchecked((uint)(b2 + 1));

        }
        while (b2 <= 20u);
    }

    public void altFWohn(EEventArt Art)
    {
        List3.Items.Clear();
        DataModul.DB_EventTable.Index = "Besu";
        DataModul.DB_EventTable.Seek("=", Art.AsString(), Modul1.FamInArb.AsString());
        if (DataModul.DB_EventTable.NoMatch)
        {
            DataModul.DB_EventTable.Index = "ArtNr";
            return;
        }
        Modul1.PrintDat.Ja = 1;
        object CounterResult = default;
        object LoopForResult = default;
        if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 1, 70, 1, ref LoopForResult, ref CounterResult))
        {
            while (!DataModul.DB_EventTable.EOF)
            {
                if (DataModul.DB_EventTable.NoMatch)
                {
                    DataModul.DB_EventTable.Index = "ArtNr";
                    return;
                }
                if (!Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 0, TextCompare: false))
                {
                    Modul1_J = 0;
                    do
                    {
                        Modul1.Kont1[Modul1_J] = "";
                        checked
                        {
                            Modul1_J = (byte)unchecked((uint)(Modul1_J + 1));
                        }
                    }
                    while (Modul1_J <= 15u);
                    Modul1.sDatu = "";
                    Modul1.Kont1[1] = "";
                    if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != Modul1.FamInArb)
                        | (DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != Art)))
                    {
                        DataModul.DB_EventTable.Index = "ArtNr";
                        break;
                    }
                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                    {
                        Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                        Modul1.Kont1[1] = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                    }
                    Modul1.UbgT = "";
                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value) && DataModul.DB_EventTable.Fields[EventFields.ArtText].Value.AsDouble() > 0.0)
                    {
                        int AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                        Modul1.Kont[0] = DataModul.TextLese1(AAA);
                        if (Modul1.Kont[0] != "")
                        {
                            Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim() + ": ";
                        }
                    }
                    string text = (!Modul1.DAus[103].AsBool()) ? (Modul1.Kont1[7] + Modul1.Kont1[1] + new string(' ', 240).Left(240) + DataModul.DB_EventTable.Fields[EventFields.LfNr].AsString()) : (Modul1.Kont1[1] + Modul1.Kont1[7] + new string(' ', 240).Left(240) + DataModul.DB_EventTable.Fields[EventFields.LfNr].AsString());
                    if (Art == EEventArt.eA_602
                        && (DataModul.DB_EventTable.Fields[EventFields.Reg].AsString() != " "))
                    {
                        text = "+" + text;
                    }
                    if (text.Trim() != "")
                    {
                        List3.Items.Add(text);
                    }
                }
                DataModul.DB_EventTable.MoveNext();
                if (!ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult, ref CounterResult))
                {
                    break;
                }
            }
        }
        Famwohn(Art);
    }
    public void Famwohn(EEventArt Art)
    {
        int try0000_dispatch = -1;
        int num = default;
        string text = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        short num5 = default;
        string Job = default;
        short num6 = default;
        string text2 = default;
        byte b = default;
        byte b2 = default;
        int famInArb = default;
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
                    object[] array;
                    int ortNr;
                    byte Schalt;
                    int AAA;
                    short Listart;
                    string LD;
                    int Nr;
                    short LfNR;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            text = "";
                            goto IL_0009;
                        case 5454:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_1240;
                                    default:
                                        goto end_IL_0000;
                                }
                                lErl = 200;
                                if (Information.Err().Number == 5)
                                {
                                    goto end_IL_0000_2;
                                }
                                if (Information.Err().Number == 3021)
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
                                goto IL_1244;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            num = 2;
                            Job = "";
                            array = new object[6];
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            num6 = (short)(List3.Items.Count - 1);
                            num5 = 0;
                            goto IL_1151;
                        IL_023f: // <========== 3
                            num = 23;
                            Modul1.sDatu = "";
                            if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != Modul1.FamInArb)))
                            {
                                DataModul.DB_EventTable.Index = "ArtNr";
                                goto end_IL_0000_2;
                            }
                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].Value.AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                LD = "";
                                Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim();
                            }
                            goto IL_0349;
                        IL_0349:
                            num = 32;
                            if (Art == EEventArt.eA_602)
                            {
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Hausnr].Value))
                                {
                                    if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Hausnr].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.Hausnr].AsInt();
                                        LD = "";
                                        Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                        Modul1.Kont1[7] = Modul1.Kont1[7] + " " + Modul1.Kont[0].Trim() + " ";
                                        Modul1.Kont[0] = "";
                                    }
                                    goto IL_0442;
                                }
                            }
                            goto IL_0442;
                        IL_0442: // <========== 3
                            num = 41;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                            {
                                Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                Modul1.sDatu = "00000000" + Modul1.sDatu.Right(8);
                                text2 = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                if (Conversions.ToBoolean(Operators.CompareString(text2.Trim(), "", TextCompare: false) == 0
                                      & DataModul.DB_EventTable.Fields[EventFields.DatumB].AsDate() != default))
                                {
                                    text2 = " von";
                                }
                                Modul1.Datwand1(ref Modul1.sDatu, text2);
                                Modul1.Kont1[1] = Modul1.sDatu;
                            }
                            goto IL_0556;
                        IL_0556:
                            num = 51;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                Modul1.sDatu = "00000000" + Modul1.sDatu.Right(8);
                                text2 = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                Modul1.Datwand1(ref Modul1.sDatu, text2);
                                if (Modul1.Kont1[1].Left(5) != "zwisc")
                                {
                                    if (Modul1.sDatu.Trim() != "")
                                    {
                                        Modul1.sDatu = " bis " + Modul1.sDatu.Trim();
                                    }
                                    goto IL_0654;
                                }
                                goto IL_0693;
                            }
                            goto IL_06a3;
                        IL_0654:
                            num = 60;
                            if (Modul1.Kont1[1].Trim() != "")
                            {
                                Modul1.Kont1[1] = " von " + Modul1.Kont1[1].Trim();
                            }
                            goto IL_0693;
                        IL_0693: // <========== 3
                            num = 64;
                            Modul1.Kont1[3] = Modul1.sDatu;
                            goto IL_06a3;
                        IL_06a3: // <========== 3
                            num = 66;
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                            {
                                Modul1.Kont1[5] = " " + Modul1.ortles1(DataModul.DB_EventTable.Fields[EventFields.Ort].AsInt(), 1, (i, s) => Modul1.ExportPlace(i, s, Modul1.Ind1, M_Namen));
                                Modul1.UbgT = "";
                            }
                            goto IL_0739;
                        IL_0739:
                            num = 71;
                            if (DataModul.DB_EventTable.Fields[EventFields.Platz].Value.AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                LD = "";
                                Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                if (Modul1.Kont[0] != "")
                                {
                                    Modul1.Kont1[6] = " " + Modul1.Kont[0].Trim();
                                    goto IL_07f1;
                                }
                            }
                            goto IL_07f1;
                        IL_07f1: // <========== 3
                            num = 77;
                            if (((Modul1.DAus[38] == "1") & !M1_Ki) | ((Modul1.DAus[42] == "1") & M1_Ki))
                            {
                                Job = Modul1.Kont1[7].TrimEnd();
                                goto IL_08b7;
                            }
                            if (((Modul1.DAus[39] == "1") & !M1_Ki) | ((Modul1.DAus[43] == "1") & M1_Ki))
                            {
                                Job = "";
                                Job = Module2.Jobdreh(Job);
                                Job += text;
                                text = "";
                            }
                            goto IL_08b7;
                        IL_08b7: // <========== 3
                            num = 86;
                            if (Job != "")
                            {
                                if (b == 1)
                                {
                                    if (Anz[0].SelectionStart > 0)
                                    {
                                        if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == ".")
                                        {
                                            Anz[0].SelectionStart = Anz[0].SelectionStart - 1;
                                            Anz[0].SelectionLength = 2;
                                            Anz[0].SelectedText = ", ";
                                        }
                                        goto IL_0986;
                                    }
                                }
                                goto IL_0986;
                            }
                            goto IL_1132;
                        IL_0986: // <========== 3
                            num = 96;
                            Anz[0].SelectionIndent = 40;
                            if (b == 0)
                            {
                                if ((Modul1.DAus[106].AsDouble() == 1.0) & (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0))
                                {
                                    Anz[0].SelectedText = "\n";
                                }
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                                Anz[0].SelectionIndent = 40;
                                if (Art == EEventArt.eA_602)
                                {
                                    Anz[0].SelectedText = "Wohnort der Familie:";
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    Anz[0].SelectedText = " ";
                                    b = 1;
                                    goto IL_0acb;
                                }
                            }
                            goto IL_0acb;
                        IL_0acb: // <========== 3
                            num = 110;
                            if (Art == EEventArt.eA_603)
                            {
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                                if (Modul1.Kont1[10].Trim() != "")
                                {
                                    Anz[0].SelectedText = Modul1.Kont1[10].Trim();
                                    goto IL_0b6b;
                                }
                                Anz[0].SelectedText = "Sonst. Daten der Familie:";
                                goto IL_0b6b;
                            }
                            goto IL_0bb5;
                        IL_0b6b: // <========== 3
                            num = 118;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectedText = " ";
                            goto IL_0bb5;
                        IL_0bb5: // <========== 3
                            num = 121;
                            Anz[0].SelectedText = Job + ".";
                            if (unchecked((Modul1.DAus[40] == "1" && Art == EEventArt.eA_603) | (Modul1.DAus[22] == "1" && Art == EEventArt.eA_602)))
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim();
                                    Bemaus(Anz[0], Modul1.UbgT1);
                                    goto IL_0c86;
                                }
                            }
                            goto IL_0c86;
                        IL_0c86: // <========== 3
                            num = 128;
                            if (unchecked((Modul1.DAus[41] == "1" && Art == EEventArt.eA_603) | (Modul1.DAus[23] == "1" && Art == EEventArt.eA_602)))
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim();
                                    Bemaus(Anz[0], Modul1.UbgT1);
                                    goto IL_0d42;
                                }
                            }
                            goto IL_0d42;
                        IL_0d42: // <========== 3
                            num = 134;
                            b2 = (byte)Modul1.DAus[96].AsInt();
                            famInArb = Modul1.FamInArb;
                            if (b2 == 1)
                            {
                                Listart = 2;
                                LD = Modul1.FamInArb.AsString();
                                Modul1.Zeugsu(Art, Modul1.PrintDat.LfNR, Listart, Modul1.FamInArb);
                                Modul1.FamInArb = LD.AsInt();
                            }
                            text3 = Modul1.Kont1[20];
                            Modul1.Kont1[20] = "";
                            Modul1.FamInArb = famInArb;
                            if (b2 == 1)
                            {
                                if (text3 != "")
                                {
                                    if (!Fb)
                                    {
                                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                                        Anz[0].SelectedText = "Angaben zur Familie: ";
                                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                        Fb = true;
                                    }
                                    Anz[0].SelectedText = " ";
                                    if (Modul1.DAus[100] == "1")
                                    {
                                        M_Sgg = 0.8f;
                                        goto IL_0f07;
                                    }
                                    M_Sgg = 1f;
                                    goto IL_0f07;
                                }
                            }
                            goto IL_0ffa;
                        IL_0f07: // <========== 3
                            num = 157;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Underline);
                            Anz[0].SelectedText = "Zeugen: ";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Regular);
                            Anz[0].SelectedText = text3.Trim();
                            text3 = "";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            goto IL_0ffa;
                        IL_0ffa: // <========== 3
                            num = 165;
                            Nr = Modul1.FamInArb;
                            LfNR = Modul1.PrintDat.LfNR;
                            Modul1.PrintDat.QuellenDatum(ref Nr, Art, ref LfNR);
                            Modul1.PrintDat.LfNR = Conversions.ToByte(LfNR);
                            Modul1.FamInArb = Nr.AsInt();
                            if (Modul1.Kont1[9].Trim() != "")
                            {
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                                Anz[0].SelectedText = " " + Modul1.Kont1[9];
                                Anz[0].SelectionCharOffset = 0;
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            }
                            goto IL_1132;
                        IL_1132: // <========== 3
                            num = 174;
                            DataModul.DB_EventTable.MoveNext();
                            num5 = (short)unchecked(num5 + 1);
                            goto IL_1151;
                        IL_1151:
                            if (num5 <= num6)
                            {
                                Modul1.PrintDat.LfNR = Conversions.ToByte(List3.Items[num5].AsString().Right(10));
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", Art.AsString(), Modul1.FamInArb.AsString(), Modul1.PrintDat.LfNR);
                                if (Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 0, TextCompare: false))
                                {
                                    Interaction.MsgBox("Stop 14");
                                }
                                Modul1_J = 0;
                                while (unchecked(Modul1_J) <= 15u)
                                {
                                    Modul1.Kont1[Modul1_J] = "";
                                    Modul1_J = (byte)unchecked((uint)(Modul1_J + 1));
                                }
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value))
                                {
                                    if (DataModul.DB_EventTable.Fields[EventFields.ArtText].Value.AsDouble() > 0.0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                                        LD = "";
                                        Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                        if (Modul1.Kont[0] != "")
                                        {
                                            Modul1.Kont1[10] = " " + Modul1.Kont[0].Trim() + ": ";
                                        }
                                        goto IL_023f;
                                    }
                                }
                                goto IL_023f;
                            }
                            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) == 0)
                            {
                                goto end_IL_0000_2;
                            }
                            Anz[0].SelectedText = "\n";
                            goto end_IL_0000_2;
                        IL_1240:
                            num4 = unchecked(num2 + 1);
                            goto IL_1244;
                        IL_1244:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 20:
                                case 21:
                                case 22:
                                case 23:
                                    goto IL_023f;
                                case 31:
                                case 32:
                                    goto IL_0349;
                                case 38:
                                case 39:
                                case 40:
                                case 41:
                                    goto IL_0442;
                                case 50:
                                case 51:
                                    goto IL_0556;
                                case 59:
                                case 60:
                                    goto IL_0654;
                                case 62:
                                case 63:
                                case 64:
                                    goto IL_0693;
                                case 65:
                                case 66:
                                    goto IL_06a3;
                                case 70:
                                case 71:
                                    goto IL_0739;
                                case 75:
                                case 76:
                                case 77:
                                    goto IL_07f1;
                                case 79:
                                case 85:
                                case 86:
                                    goto IL_08b7;
                                case 93:
                                case 94:
                                case 95:
                                case 96:
                                    goto IL_0986;
                                case 108:
                                case 109:
                                case 110:
                                    goto IL_0acb;
                                case 114:
                                case 117:
                                case 118:
                                    goto IL_0b6b;
                                case 120:
                                case 121:
                                    goto IL_0bb5;
                                case 126:
                                case 127:
                                case 128:
                                    goto IL_0c86;
                                case 132:
                                case 133:
                                case 134:
                                    goto IL_0d42;
                                case 153:
                                case 156:
                                case 157:
                                    goto IL_0f07;
                                case 163:
                                case 164:
                                case 165:
                                    goto IL_0ffa;
                                case 172:
                                case 173:
                                case 174:
                                    goto IL_1132;
                                case 26:
                                case 178:
                                case 179:
                                case 182:
                                case 185:
                                case 191:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 5454;
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
    public void Bild(string BKennz, int Nr)
    {
        if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
        {
            Anz[0].SelectedText = "\n";
        }
        if (Modul1.DAus[88] != "1")
        {
            return;
        }

        DataModul.DB_PictureTable.Index = "Perkenn  ";
        DataModul.DB_PictureTable.Seek("=", BKennz, Nr);
        while (!DataModul.DB_PictureTable.EOF && !DataModul.DB_PictureTable.NoMatch && !(DataModul.DB_PictureTable.Fields[PictureFields.ZuNr].AsInt() != Nr)
            && !(DataModul.DB_PictureTable.Fields[PictureFields.Kennz].AsString() != BKennz))
        {
            string text = (DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Left(1) != "#") ? (DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString() + DataModul.DB_PictureTable.Fields[PictureFields.Datei].AsString()) : Conversions.ToString(Modul1.Verz + Strings.Mid(DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString(), 2, DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Length) + DataModul.DB_PictureTable.Fields[PictureFields.Datei].Value);
            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
            {
                Anz[0].SelectedText = "\n";
            }
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            Modul1.UbgT1 = "Bild: " + text;
            if (Modul1.DAus[70] == "0")
            {
                Modul1.UbgT1 = Modul1.PrintDat.Retweg(Modul1.UbgT1);
            }
            if (BKennz == "F")
            {
            }
            Anz[0].SelectionColor = Color.Purple;
            Anz[0].SelectedText = Modul1.UbgT1 + "\n";
            Modul1.UbgT1 = "";
            DataModul.DB_PictureTable.MoveNext();
        }
    }

    public void Perabschluss(int persInArb, int famInArb)
    {
        Zeilenumbruch(Anz[0]);
        PerSp1 = persInArb;
        Modul1.PrintDat.FamSp = famInArb;
        Modul1.UbgT = "";
        Modul1.UbgT1 = "";
        if (((Modul1.DAus[92] == "1") & !M1_Ki) | ((Modul1.DAus[93] == "1") & M1_Ki))
        {
            short Listart = 0;
            Modul1.PrintDat.Zeuge_Bei(Modul1.PersInArb, ref Listart);
        }
        persInArb = PerSp1;
        if ((((Modul1.DAus[37] == "1") & !M1_Ki) | ((Modul1.DAus[49] == "1") & M1_Ki)) && Modul1.DAus[37] == "1")
        {
            Pate_bei(persInArb);
        }
        persInArb = PerSp1;
        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        string sBem1 = (DataModul.Person.Seek(persInArb)?.Fields["Bem1"]).AsString();
        if ((((Modul1.DAus[1] == "1") & !M1_Ki) | ((Modul1.DAus[7] == "1") & M1_Ki))
            && Operators.CompareString(sBem1.Trim(), "", TextCompare: false) != 0)
        {
            Modul1.UbgT1 = sBem1;
            if (Modul1.DAus[72] == "1")
            {
                M_Sgg = 0.8f;
            }
            else
            {
                M_Sgg = 1f;
            }
            leerweg();
            if (Modul1.DAus[94].AsDouble() == 1.0)
            {
                Anz[0].SelectedText = "\n";
            }
            else
            {
                Anz[0].SelectedText = " ";
            }
            Modul1.UbgT1 = sBem1;
            if (Modul1.DTxt[16].Trim() != "")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Underline);
                Anz[0].SelectedText = Modul1.DTxt[16];
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Regular);
                Anz[0].SelectedText = ": ";
            }
            Bemaus(Anz[0], Modul1.UbgT1);
        }
        leerweg();
        Anz[0].SelectionIndent = 20;
        if (M1_Ki)
        {
            Anz[0].SelectionIndent = 50;
        }
        M1_Ki = false;
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
                    case 704:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0220;
                                default:
                                    goto end_IL_0000;
                            }
                            if (Interaction.MsgBox(Conversion.ErrorToString() + "Retweg2", MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_0223;
                        }
                    end_IL_0000:
                        break;
                    IL_0007:
                        num = 2;
                        if (Anz[0].Text.Length > 0)
                        {
                            leerweg();
                            b = 0;
                            goto IL_0031;
                        }
                        goto IL_01b7;
                    IL_0031: // <========== 3
                        num = 5;
                        lErl = 1;
                        if (Anz[0].Text.Length == 0)
                        {
                            goto end_IL_0000_2;
                        }
                        if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == "\n")
                        {
                            Anz[0].SelectionStart = checked(Anz[0].SelectionStart - 1);
                            Anz[0].SelectionLength = 1;
                            Anz[0].SelectedText = "";
                            b = 1;
                        }
                        if (Anz[0].Text.Length == 0)
                        {
                            goto end_IL_0000_2;
                        }
                        if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == "\r")
                        {
                            Anz[0].SelectionStart = checked(Anz[0].SelectionStart - 1);
                            Anz[0].SelectionLength = 1;
                            Anz[0].SelectedText = "";
                            b = 1;
                        }
                        leerweg();
                        if (b == 1)
                        {
                            b = 0;
                            goto IL_0031;
                        }
                        goto IL_01b7;
                    IL_01b7: // <========== 3
                        num = 30;
                        Anz[0].SelectionCharOffset = 0;
                        goto end_IL_0000_2;
                    IL_0220:
                        num4 = num2 + 1;
                        goto IL_0223;
                    IL_0223:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 5:
                            case 27:
                                goto IL_0031;
                            case 28:
                            case 29:
                            case 30:
                                goto IL_01b7;
                            case 7:
                            case 16:
                            case 31:
                            case 36:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 704;
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
}
