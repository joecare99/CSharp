using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Compatibility.VB6;
using Microsoft.VisualBasic.CompilerServices;
using BaseLib.Helper;
using Druck.My;
using GenFree;
using GenFree.Helper;
using GenFree.Data;
using GenFree.Interfaces;
using GenFree.Interfaces.Sys;

namespace Druck.Views;

[DesignerGenerated]
public partial class FaBu : Form
{

    private long IZahl;

    private int FamKi;

    private int Persp1;

    private int Persp2;

    private long Ahne; // Ahnennummer

    private string start;

    private ELinkKennz KennzBeh;

    private byte Tz;

    private short UrEnkelIdent;

    private short EnkelIdent;

    private short KIident;

    private string Ahn;

    private bool Kindfer;

    private string Fami;

    private string QuText;

    private string Mannname;

    private string Frauname;

    private byte Qu3;

    private int FamKi1;

    private bool Fb;

    private short MasterRück;

    private short SpRück;

    private string Lauf;

    private short Kindeinzug;

    private short EnkelEinzug;

    private byte UrEnkelEinzug;

    private int Texz1;

    private short Texz;

    private float Fackt;

    private string Fn;

    private IDocument document;
    private bool M1_Ki;
    private int M1_J;
    private short M_Indent;
    private int M_A;
    private string M_Namen;
    private byte _M_KonLen;

    public bool IsEmpty => throw new NotImplementedException();

    public IModul1 Modul1 => _Modul1.Instance;

    [DebuggerNonUserCode]
    public FaBu()
    {
        base.Load += FaBu_Load;
        base.FormClosing += FaBu_FormClosing;
        UrEnkelIdent = 120;
        EnkelIdent = 100;
        KIident = 40;
        Ahn = "";
        Lauf = "";
        Kindeinzug = 30;
        EnkelEinzug = 60;
        UrEnkelEinzug = 90;
        Fackt = 1f;
        this.Anz = new Microsoft.VisualBasic.Compatibility.VB6.RichTextBoxArray(this.components);
        this.Befehl = new ControlArray<System.Windows.Forms.Button>();
        this.Bezeichnung1 = new ControlArray<System.Windows.Forms.Label>();
        this.Command_Renamed = new ControlArray<System.Windows.Forms.Button>();
        this.Command2 = new ControlArray<System.Windows.Forms.Button>();
        this.Label1 = new ControlArray<System.Windows.Forms.Label>();
        InitializeComponent();
        this.Befehl.SetIndex(this._Befehl_8, 8);
        this.Command_Renamed.SetIndex(this._Command_1, 1);
        this.Command_Renamed.SetIndex(this._Command_0, 0);
        this.Label1.SetIndex(this._Label1_5, 5);
        this.Label1.SetIndex(this._Label1_4, 4);
        this.Label1.SetIndex(this._Label1_3, 3);
        this.Label1.SetIndex(this._Label1_2, 2);
        this.Label1.SetIndex(this._Label1_1, 1);
        this.Label1.SetIndex(this._Label1_0, 0);
        this.Befehl.SetIndex(this._Befehl_7, 7);
        this.Command2.SetIndex(this._Command2_4, 4);
        this.Command2.SetIndex(this._Command2_3, 3);
        this.Command2.SetIndex(this._Command2_0, 0);
        this.Command2.SetIndex(this._Command2_1, 1);
        this.Command2.SetIndex(this._Command2_2, 2);
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


    public Bitmap PicResizeByWidth(Image SourceImage, int Newheigth)
    {
        decimal d = new decimal(Newheigth / (double)SourceImage.Height);
        int num = Convert.ToInt32(decimal.Multiply(d, new decimal(SourceImage.Width)));
        Bitmap bitmap = new Bitmap(num, Newheigth);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Rectangle rect = new Rectangle(0, 0, num, Newheigth);
            graphics.DrawImage(SourceImage, rect);
        }
        return bitmap;
    }

    public Image AutoSizeImage(Image oBitmap, int maxWidth, int maxHeight, bool bStretch = false)
    {
        //Discarded unreachable code: IL_0096
        float num = (float)(maxWidth / (double)maxHeight);
        int num2 = oBitmap.Width;
        int num3 = oBitmap.Height;
        float num4 = (float)(num2 / (double)num3);
        if (num2 > maxWidth || num3 > maxHeight || bStretch)
        {
            checked
            {
                if (num4 <= num)
                {
                    num2 = (int)Math.Round(num2 / (num3 / (double)maxHeight));
                    num3 = maxHeight;
                }
                else
                {
                    num3 = (int)Math.Round(num3 / (num2 / (double)maxWidth));
                    num2 = maxWidth;
                }
                Bitmap bitmap = new Bitmap(num2, num3);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    Rectangle rect = new Rectangle(0, 0, num2, num3);
                    graphics.DrawImage(oBitmap, rect);
                }
                return bitmap;
            }
        }
        return oBitmap;
    }
    private void Befehl_Click(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int index = default;
        int num2 = default;
        int num3 = default;
        string right = default;
        string text = default;
        string text2 = default;
        string text3 = default;
        string text4 = default;
        object CounterResult = default;
        object LoopForResult = default;
        byte b = default;
        object LoopForResult2 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    string text5;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            index = Befehl.GetIndex((Button)eventSender);
                            goto IL_0015;
                        case 8293:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 3:
                                        goto IL_1ba9;
                                    case 1:
                                        goto IL_1be3;
                                    default:
                                        goto end_IL_0000;
                                }
                                switch (Information.Err().Number)
                                {
                                    case 55:
                                        break;
                                    case 57:
                                        goto IL_1b27;
                                    default:
                                        goto IL_1b5e;
                                }
                                FileSystem.FileClose();
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1bdf;
                            }
                        end_IL_0000:
                            break;
                        IL_0015:
                            num = 2;
                            text3 = "";
                            right = "";
                            text = "";
                            text4 = "";
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            text5 = "Datum " + DateTime.Today.Month.ToString() + "." + DateTime.Today.Day.ToString() + "." + DateTime.Today.Year.ToString();
                            switch (index)
                            {
                                case 0:
                                    b = 0;
                                    while (unchecked(b) <= 3u)
                                    {
                                        Anz[b].Visible = false;
                                        b++;
                                    }
                                    Anz[2].Visible = true;
                                    Anz[2].Text = "";
                                    DataModul.DSB_NamIdxTable.Index = "Langi";
                                    Anz[2].SelectionIndent = 0;
                                    Anz[2].SelectionAlignment = HorizontalAlignment.Center;
                                    Anz[2].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 1.3), FontStyle.Bold);
                                    Anz[2].SelectedText = "Namen-Index (Langform)\n\n";
                                    Anz[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    Anz[2].SelectionAlignment = HorizontalAlignment.Left;
                                    DataModul.DSB_NamIdxTable.Seek(">=", " ", " ", 0);
                                    var M_Namen = "";
                                    while (!DataModul.DSB_NamIdxTable.EOF)
                                    {
                                        Anz[2].SelectionIndent = 20;
                                        Anz[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                        if (M_Namen != DataModul.DSB_NamIdxTable.Fields["Name1"].AsString())
                                        {
                                            if (text4 != "")
                                            {
                                                Anz[2].SelectionIndent = 30;
                                                Anz[2].SelectedText = text4 + "; " + Strings.Mid(text, 2, text.Length) + ".\n";
                                                text = "";
                                                right = "";
                                                text4 = "";
                                            }
                                            Anz[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                                            Anz[2].SelectionIndent = 20;
                                            Anz[2].SelectedText = DataModul.DSB_NamIdxTable.Fields["Name1"].AsString() + '\n';
                                            Anz[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                            text = "";
                                            right = "";
                                            M_Namen = DataModul.DSB_NamIdxTable.Fields["Name1"].AsString();
                                        }
                                        if (text4 == "")
                                        {
                                            text4 = DataModul.DSB_NamIdxTable.Fields["Name"].AsString();
                                        }
                                        if (text4 != DataModul.DSB_NamIdxTable.Fields["Name"].AsString())
                                        {
                                            Anz[2].SelectionIndent = 30;
                                            Anz[2].SelectedText = text4 + ": " + Strings.Mid(text, 2, text.Length) + ".\n";
                                            text = "";
                                            right = 0.AsString();
                                            text4 = DataModul.DSB_NamIdxTable.Fields["Name"].AsString();
                                        }
                                        if (!Information.IsDBNull(DataModul.DSB_NamIdxTable.Fields["Nr"].Value))
                                        {
                                            if (DataModul.DSB_NamIdxTable.Fields["Nr"].AsInt() != right.AsInt())
                                            {
                                                text = text + ", " + DataModul.DSB_NamIdxTable.Fields["Nr"].AsString().Trim();
                                                right = DataModul.DSB_NamIdxTable.Fields["Nr"].AsString();
                                            }
                                        }
                                        DataModul.DSB_NamIdxTable.MoveNext();
                                    }
                                    if (text4 != "")
                                    {
                                        Anz[2].SelectionIndent = 30;
                                        Anz[2].SelectedText = text4 + "; " + Strings.Mid(text, 2, text.Length) + ".\n";
                                        text = "";
                                        right = "";
                                        text4 = "";
                                    }
                                    Anz[2].SaveFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
                                    Anz[2].LoadFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
                                    goto end_IL_0000_2;
                                case 1:
                                    Befehl_1_Click(out text2, out b);
                                    goto end_IL_0000_2;
                                case 2:
                                    goto IL_07c5;
                                case 3:
                                    goto IL_07ec;
                                case 4:
                                    goto IL_094c;
                                case 5:
                                    goto IL_09e2;
                                case 6:
                                    goto IL_0ec8;
                                case 7:
                                    goto IL_0f30;
                                case 8:
                                    goto IL_1a40;
                                default:
                                    break;
                            }
                            goto IL_1ab2;
                        IL_07c5:
                            num = 86;
                            Close();
                            MyProject.Forms.Druck.Show();
                            goto end_IL_0000_2;
                        IL_07ec:
                            num = 91;
                            MyProject.Forms.Hinter.CommonDialog1Save.Filter = "Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF";
                            b = 0;
                            goto IL_0810;
                        IL_0810: // <========== 3
                                 // <========== 3
                            num = 93;
                            if (!Anz[b].Visible)
                            {
                                b = (byte)unchecked((uint)(b + 1));
                                if (unchecked(b) <= 2u)
                                {
                                    goto IL_0810;
                                }
                            }
                            MyProject.Forms.Hinter.CommonDialog1Save.InitialDirectory = Modul1.GenFreeDir + "list\\";
                            MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex = 2;
                            MyProject.Forms.Hinter.CommonDialog1Save.ShowDialog();
                            if (MyProject.Forms.Hinter.CommonDialog1Save.FileName.Trim() == "")
                            {
                                goto end_IL_0000_2;
                            }
                            switch (MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex)
                            {
                                case 1:
                                    Anz[b].SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);
                                    break;
                                case 2:
                                    goto IL_0917;
                                default:
                                    break;
                            }
                            goto end_IL_0000_2;
                        IL_0917:
                            num = 108;
                            Anz[b].SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.RichText);
                            goto end_IL_0000_2;
                        IL_094c:
                            num = 114;
                            if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, 6, 1, ref LoopForResult2, ref CounterResult))
                            {
                                while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult2, ref CounterResult))
                                {
                                    Befehl[(short)CounterResult.AsInt()].Visible = true;
                                }
                            }
                            b = 0;
                            while (unchecked(b) <= 3u)
                            {
                                Anz[b].Visible = false;
                                b = (byte)unchecked((uint)(b + 1));
                            }
                            Anz[0].Visible = true;
                            goto end_IL_0000_2;
                        IL_09e2:
                            num = 123;
                            b = 0;
                            while (unchecked(b) <= 3u)
                            {
                                Anz[b].Visible = false;
                                b = (byte)unchecked((uint)(b + 1));
                            }
                            Anz[1].Text = "";
                            Anz[1].Visible = true;
                            Anz[1].Text = "";
                            DataModul.DSB_NamIdxTable.Index = "Kurzname";
                            Anz[1].SelectionIndent = 20;
                            Anz[1].SelectionAlignment = HorizontalAlignment.Center;
                            Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 1.3), FontStyle.Bold);
                            Anz[1].SelectedText = "Namen-Index (Kurzform)\n\n";
                            Anz[1].SelectionAlignment = HorizontalAlignment.Left;
                            Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            DataModul.DSB_NamIdxTable.Seek(">=", " ", 0);
                            goto IL_0d9e;
                        IL_0cd7:
                            // <========== 3
                            num = 150;
                            if (!Information.IsDBNull(DataModul.DSB_NamIdxTable.Fields["Nr"].Value))
                            {
                                if (DataModul.DSB_NamIdxTable.Fields["Nr"].AsInt() != right.AsInt())
                                {
                                    text = text + ", " + DataModul.DSB_NamIdxTable.Fields["Nr"].AsString().Trim();
                                    right = DataModul.DSB_NamIdxTable.Fields["Nr"].AsString();
                                }
                            }
                            goto IL_0d8d;
                        IL_0d8d: // <========== 3
                                 // <========== 3
                            num = 156;
                            DataModul.DSB_NamIdxTable.MoveNext();
                            goto IL_0d9e;
                        IL_0d9e: // <========== 3
                                 // <========== 3
                            num = 138;
                            if (!DataModul.DSB_NamIdxTable.EOF)
                            {
                                if (text3 != DataModul.DSB_NamIdxTable.Fields["Name1"].AsString())
                                {
                                    if (text3 != "")
                                    {
                                        Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                                        Anz[1].SelectedText = text3;
                                        Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                        Anz[1].SelectedText = text + "\n\n";
                                        text = "";
                                        right = "";
                                    }
                                    text3 = DataModul.DSB_NamIdxTable.Fields["Name1"].AsString();
                                }
                                goto IL_0cd7;
                            }
                            Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            Anz[1].SelectedText = text3;
                            Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[1].SelectedText = text + "\n\n";
                            text = "";
                            right = "";
                            Anz[1].SaveFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
                            Anz[1].LoadFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
                            goto end_IL_0000_2;
                        IL_0ec8:
                            num = 168;
                            b = 0;
                            while (unchecked(b) <= 3u)
                            {
                                Anz[b].Visible = false;
                                b++;
                            }
                            Anz[3].Visible = true;
                            Frame2.Visible = true;
                            goto end_IL_0000_2;
                        IL_0f30:
                            num = 175;
                            Anz[1].Text = "";
                            Anz[0].Visible = false;
                            Anz[1].Visible = true;
                            Anz[1].SelectionAlignment = HorizontalAlignment.Center;
                            Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 1.3), FontStyle.Bold);
                            Anz[1].SelectedText = "Quellenanhang\n\n";
                            Anz[1].SelectionAlignment = HorizontalAlignment.Left;
                            Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            DataModul.DSB_QuellIdxTable.Index = "Quelle";
                            DataModul.DSB_QuellIdxTable.MoveFirst();
                            for (var i = 1; i < DataModul.DSB_QuellIdxTable.RecordCount; i++)
                            {
                                text = DataModul.DSB_QuellIdxTable.Fields["Nr"].AsString();
                                if (text.AsDouble() != Modul1.AltNr)
                                {
                                    Modul1.AltNr = text.AsInt();
                                    DataModul.DB_QuTable.Index = "NR";
                                    DataModul.DB_QuTable.Seek("=", text);
                                    Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    Anz[1].SelectedText = "\n";
                                    Anz[1].SelectionIndent = 20;
                                    Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                                    Anz[1].SelectedText = DataModul.DB_QuTable.Fields[QuFields._4].AsString() + '\n';
                                    Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    Anz[1].SelectionIndent = 30;
                                    if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._2].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                        Anz[1].SelectedText = ("Titel: " + DataModul.DB_QuTable.Fields[QuFields._2].Value + '\n').AsString();
                                    }
                                    if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._5].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                        Anz[1].SelectedText = ("Autor: " + DataModul.DB_QuTable.Fields[QuFields._5].Value + '\n').AsString();
                                    }
                                    Modul1.UbgT = "";
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
                                        Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                        Anz[1].SelectedText = ("in: " + DataModul.DB_QuTable.Fields[QuFields._10].Value + '\n').AsString();
                                    }
                                    if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._11].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        Anz[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                        Anz[1].SelectedText = ("Jahrgang: " + DataModul.DB_QuTable.Fields[QuFields._11].Value + '\n').AsString();
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
                                        Anz[1].SelectedText = ("Bemerkungen: " + DataModul.DB_QuTable.Fields[QuFields._13].Value + '\n').AsString();
                                    }
                                }
                                DataModul.DSB_QuellIdxTable.MoveNext();
                            }
                            goto end_IL_0000_2;
                        IL_1a40:
                            num = 249;
                            DataModul.MandDB.Close();
                            DataModul.DOSB.Close();
                            DataModul.TempDB.Close();
                            DataModul.DSB.Close();
                            ProjectData.ClearProjectError();
                            num3 = 3;
                            Interaction.Shell(Modul1.GenFreeDir + "Gen_Plus.exe", AppWinStyle.NormalFocus);
                            ProjectData.EndApp();
                            goto end_IL_0000_2;
                        IL_1ab2:
                            num = 258;
                            Interaction.MsgBox(index);
                            goto end_IL_0000_2;
                        IL_1b27:
                            num = 270;
                            text2 += "a";
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_1bdf;
                        IL_1b5e:
                            num = 275;
                            if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_1bdf;
                        IL_1ba9:
                            num = 282;
                            if (Information.Err().Number != 5)
                            {
                                goto end_IL_0000_2;
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_1be3;
                        IL_1bdf: // <========== 3
                                 // <========== 3
                            num4 = num2;
                            goto IL_1be7;
                        IL_1be3:
                            num4 = unchecked(num2 + 1);
                            goto IL_1be7;
                        IL_1be7:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 60:
                                case 61:
                                case 62:
                                case 26:
                                case 27:
                                case 63:
                                case 76:
                                case 93:
                                    goto IL_0810;
                                case 149:
                                case 150:
                                    goto IL_0cd7;
                                case 154:
                                case 155:
                                case 156:
                                    goto IL_0d8d;
                                case 137:
                                case 138:
                                case 157:
                                    goto IL_0d9e;
                                case 186:
                                case 243:
                                case 244:
                                case 245:
                                case 9:
                                case 73:
                                case 84:
                                case 88:
                                case 89:
                                case 102:
                                case 106:
                                case 109:
                                case 110:
                                case 111:
                                case 112:
                                case 121:
                                case 166:
                                case 173:
                                case 247:
                                case 255:
                                case 256:
                                case 259:
                                case 260:
                                case 262:
                                case 267:
                                case 268:
                                case 272:
                                case 273:
                                case 279:
                                case 280:
                                case 281:
                                case 284:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 8293;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 16
                       // <========== 16
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void Befehl_1_Click(out string text2, out byte b)
    {
        checked
        {
            b = 0;
            while (b <= 2 && !Anz[b].Visible)
            {
                b++;
            }
            text2 = "Text2";
            Anz[b].SaveFile(Modul1.Verz1 + "TEMP\\" + text2 + ".RTF", RichTextBoxStreamType.RichText);
            Anz[b].LoadFile(Modul1.Verz1 + "TEMP\\" + text2 + ".RTF", RichTextBoxStreamType.RichText);
            Interaction.Shell(Modul1.Aus[7] + " " + Modul1.Verz1 + "Temp\\" + text2 + ".RTF", AppWinStyle.MaximizedFocus);
        }
    }

    private void Command_Renamed_Click(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int M_An = default;
        short num6 = default;
        short num5 = default;
        short texz = default;
        string left = default;
        short num8 = default;
        int lErl = default;
        string inputStr = default;
        byte b3 = default;
        int num9 = default;
        short num11 = default;
        string text = default;
        byte b6 = default;
        byte b7 = default;
        int famInArb = default;
        byte b8 = default;
        ELinkKennz kennz = default;
        byte b9 = default;
        string ubgT = default;
        byte b10 = default;
        byte b11 = default;
        int famInArb2 = default;
        ELinkKennz eLKennz2 = default;
        string text3 = default;
        byte b12 = default;
        byte b13 = default;
        byte b14 = default;
        byte b15 = default;
        byte Tot = default;
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
                    byte b;
                    short Listart;
                    string LD;
                    bool neb;
                    EEventArt Art;
                    byte LDC;
                    int index;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            index = Command_Renamed.GetIndex((Button)eventSender);
                            goto IL_0016;
                        case 17064:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_38e6;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 3021)
                                {
                                    Frame1.Visible = false;
                                    LDC = 0;
                                    Kinder(ref LDC, M_An);
                                    M_An = 0;
                                    if (DataModul.DSB_QuellIdxTable.RecordCount == 0)
                                    {
                                        Befehl[7].Enabled = false;
                                    }
                                    goto IL_33f8;
                                }
                                if (Information.Err().Number == 5)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_38e6;
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
                                goto IL_38ea;
                            }
                        end_IL_0000:
                            break;
                        IL_0016:
                            num = 2;
                            left = "";
                            Kindfer = false;
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            Frame1.Visible = false;
                            switch (index)
                            {
                                case 0:
                                    break;
                                case 1:
                                    goto IL_007f;
                                default:
                                    goto IL_3361;
                            }
                            Close();
                            MyProject.Forms.Druck.Show();
                            goto end_IL_0000_2;
                        IL_007f:
                            num = 15;
                            MyProject.Forms.AW.Close();
                            goto IL_026a;
                        IL_026a: // <========== 3
                            num = 34;
                            Anz[0].Visible = true;
                            MyProject.Forms.AW.ShowDialog("FAB");
                            if (Strings.Mid(Label1[3].Text, 16, 10).AsDouble() < 1.0)
                            {
                                Close();
                                MyProject.Forms.Druck.Show();
                                goto end_IL_0000_2;
                            }
                            DataModul.DB_PersonTable.Index = "PerNr";
                            DataModul.DB_PersonTable.MoveFirst();
                            goto IL_03fe;
                        IL_0366: // <========== 3
                            num = 52;
                            if (Modul1.DAus[120] == "1")
                            {
                                Module2.Datcheck();
                                if (Modul1.Datum6.AsDouble() > Modul1.DAus[121].AsDouble())
                                {
                                    DataModul.DT_SperrTable.AddNew();
                                    DataModul.DT_SperrTable.Fields["Pernr"].Value = Modul1.PersInArb;
                                    DataModul.DT_SperrTable.Update();

                                }
                            }
                            goto IL_03f0;
                        IL_03f0: // <========== 4
                            num = 60;
                            DataModul.DB_PersonTable.MoveNext();
                            goto IL_03fe;
                        IL_03fe: // <========== 3
                            num = 44;
                            if (!DataModul.DB_PersonTable.EOF)
                            {
                                Modul1.PersInArb = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                                if (Modul1.DAus[122].AsDouble() == 1.0)
                                {
                                    Module2.TotPrüf1(ref Tot);
                                    if (Tot != 2)
                                    {
                                    }
                                    else goto IL_03f0;
                                }
                                goto IL_0366;
                            }
                            Anz[0].Font = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            num6 = 0;
                            while ((short)unchecked(num6 + 1) <= 3)
                            {
                                Anz[num6].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            }
                            DataModul.DT_AncesterTable.Index = "Ahnen";
                            DataModul.DT_AncesterTable.MoveFirst();
                            Modul1.PersInArb = DataModul.DT_AncesterTable.Fields["PerNr"].AsInt();
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            DataModul.DT_AncesterTable.MoveLast();
                            _M_KonLen = (byte)Strings.Len(DataModul.DT_AncesterTable.Fields["Ahn"].AsDouble().AsString().Trim());
                            b = (Byte)DataModul.DT_AncesterTable.Fields["Gen"].AsInt();
                            Anz[0].SelectionAlignment = HorizontalAlignment.Center;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 1.3), FontStyle.Bold);
                            Anz[0].Enabled = false;
                            num6 = 0;
                            while ((short)unchecked(num6 + 1) <= 7)
                            {
                                Befehl[num6].Visible = false;
                            }
                            Befehl[2].Visible = true;
                            Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person, Modul1.DAus[89] == "1"));
                            Anz[0].SelectedText = "Familienbuch " + DataModul.DT_AncesterTable.Fields["Gen"].AsString() + " Generationen für " + (Modul1.Person.Prae.TrimEnd() + " " + Modul1.Kont[3]).Trim() + " " + Modul1.Kont[0] + "\n";
                            Bezeichnung1[0].Text = "Familienbuch " + DataModul.DT_AncesterTable.Fields["Gen"].AsString() + " Generationen für " + (Modul1.Person.Prae.TrimEnd() + " " + Modul1.Kont[3]).Trim() + " " + Modul1.Kont[0];
                            M1_Ki = false;
                            Bezeichnung1[0].Refresh();
                            Anz[0].SelectedText = "Erstellt am " + DateTime.Today.AsString();
                            Anz[0].SelectedText = $" von {Modul1.User.Name.Trim()} mit {Modul1.AppName} aus Mandant: {Modul1.Verz}\n";
                            Anz[0].SelectedText = "\n";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectionAlignment = HorizontalAlignment.Left;
                            DataModul.DT_AncesterTable.Index = "Ahnen";
                            DataModul.DT_AncesterTable.MoveFirst();
                            num8 = 1;
                            goto IL_0855;
                        IL_0855: // <========== 5
                            num = 93;
                            lErl = 1;
                            Anz[0].SelectionIndent = 0;
                            M1_Ki = false;
                            Anz[0].SelectionHangingIndent = 15;
                            if (Anz[0].SelectionStart > 5000)
                            {
                                Retweg2(Anz[0]);
                                Anz[0].SelectionIndent = 0;
                                Anz[0].SelectedText = "\n";
                                Texz1 = Anz[0].SelectionStart;
                                Anz[0].SelectionStart = 0;
                                Anz[0].SelectionLength = Texz1;
                                Texz++;
                                FileSystem.FileClose(99);
                                Fn = Modul1.GenFreeDir + "Temp\\Sptext" + Texz.AsString().Trim() + ".rtf";
                                FileSystem.FileOpen(99, Fn, OpenMode.Output);
                                FileSystem.PrintLine(99, Anz[0].SelectedRtf);
                                FileSystem.FileClose(99);
                                Anz[0].Text = "";
                                Anz[0].SelectionAlignment = HorizontalAlignment.Left;
                            }
                            goto IL_0a26;
                        IL_0a26:
                            num = 114;
                            Anz[0].SelectionIndent = 0;
                            Anz[0].SelectionHangingIndent = 15;
                            inputStr = Strings.Right("00000000" + DataModul.DT_AncesterTable.Fields["Ahn"].AsDouble().AsString().Trim(), 8);
                            num8 = (short)(num8 + 1);
                            if (num8 == 12)
                            {
                                num8 = 10;
                            }
                            goto IL_0aad;
                        IL_0aad:
                            num = 121;
                            if (b3 == 1)
                            {
                                b3 = 0;
                                goto IL_11ac;
                            }
                            if (num9 != 0)
                            {
                                Modul1.FamInArb = num9;
                            }
                            goto IL_0ad4;
                        IL_0ad4:
                            num = 128;
                            if (Anz[0].SelectionStart > 5000)
                            {
                                Aufteil();
                            }
                            goto IL_0b00;
                        IL_0b00:
                            num = 131;
                            Modul1.Famles();
                            if (Conversions.ToBoolean((DataModul.DT_AncesterTable.Fields["Ehe"].AsInt() == 0) & DataModul.DT_AncesterTable.Fields["Ahn"].AsDouble() == 1.0))
                            {
                                Quellen();
                            }
                            goto IL_0b84;
                        IL_0b84:
                            num = 135;
                            if (DataModul.DT_AncesterTable.Fields["Ehe"].AsInt() != 0)
                            {
                                if (num8 > 3)
                                {
                                    if (Modul1.Family.Frau == 0)
                                    {
                                        LDC = 0;
                                        Kinder(ref LDC, M_A);
                                        M_An = 0;
                                    }
                                    goto IL_0be2;
                                }
                                goto IL_0c3f;
                            }
                            goto IL_0c49;
                        IL_0be2:
                            num = 140;
                            if (Ahn.AsDouble() / 2.0 != Conversion.Int(Ahn.AsDouble() / 2.0))
                            {
                                M_Indent = (byte)KIident;
                                LDC = 0;
                                Kinder(ref LDC, M_An);
                                M_An = 0;
                            }
                            goto IL_0c3f;
                        IL_0c3f: // <========== 3
                            num = 145;
                            goto IL_0c49;
                        IL_0c49: // <========== 3
                            num = 147;
                            if (Kindfer)
                            {
                                if (Anz[0].SelectionStart > 0)
                                {
                                    Retweg2(Anz[0]);
                                    Anz[0].SelectedText = "\n\n\n";

                                }
                            }
                            goto IL_0c9d;
                        IL_0c9d: // <========== 3
                            num = 153;
                            Kindfer = false;
                            if (!DataModul.DT_AncesterTable.NoMatch)
                            {
                                var M_Namen = "";
                                if (Modul1.Schalt == 0)
                                {
                                    if (DataModul.DT_AncesterTable.Fields["Ahn"].AsDouble() == Ahn.AsDouble() + 1.0)
                                    {
                                        Modul1.PersInArb = DataModul.DT_AncesterTable.Fields["PerNr"].AsInt();
                                        if ((left == "M") & (Ahn.AsDouble() != 1.0))
                                        {
                                            Fami = "";
                                            Modul1.eLKennz = ELinkKennz.lkFather;
                                            aiFams = DataModul.Link.GetPersonFams(Modul1.PersInArb, Modul1.eLKennz);
                                            if (Modul1.UbgT.Length != 10)
                                            {
                                                Fami = Modul1.UbgT;
                                                MannHeilist.Items.Clear();
                                                short num10 = (short)Math.Round(Fami.Length / 10.0);
                                                num6 = 1;
                                                while (num6 <= num10)
                                                {
                                                    MannHeilist.Items.Add(Strings.Mid(Fami, num6 * 10 - 9, 10));
                                                    num6 = (short)unchecked(num6 + 1);
                                                }
                                                Modul1.PersInArb = DataModul.DT_AncesterTable.Fields["PerNr"].AsInt();
                                                Modul1.eLKennz = ELinkKennz.lkMother;
                                                aiFams = DataModul.Link.GetPersonFams(Modul1.PersInArb, Modul1.eLKennz);
                                                if (Modul1.UbgT.Length > 10)
                                                {
                                                    num11 = (short)Math.Round(Fami.Length / 10.0);
                                                    num6 = 1;
                                                    goto IL_1025;
                                                }
                                                Modul1.FamInArb = (int)Math.Round(Modul1.UbgT.AsDouble());

                                            }
                                            else
                                            {
                                                Modul1.FamInArb = (int)Math.Round(Modul1.UbgT.AsDouble());
                                            }
                                            goto IL_1078;
                                        }

                                    }
                                }
                                goto IL_11ac;
                            }
                            goto IL_31ed;
                        IL_0fd9:
                            num = 185;
                            b7 = (byte)unchecked((uint)(b7 + 1));
                            goto IL_0fe7;
                        IL_0fe7:
                            if (unchecked(b7 <= (uint)b6))
                            {
                                if (MannHeilist.Items[b7].AsString() == text)
                                {
                                    MannHeilist.Items.RemoveAt(b7);
                                }
                                goto IL_0fd9;
                            }
                            goto IL_1006;
                        IL_1006: // <========== 3
                            num = 189;
                            if (Modul1.Schalt != 1)
                            {
                                num6 = (short)unchecked(num6 + 1);
                                goto IL_1025;
                            }
                            goto IL_1078;
                        IL_1025:
                            if (num6 <= num11)
                            {
                                text = Strings.Mid(Fami, num6 * 10 - 9, 10);
                                byte b4 = (byte)Math.Round(Modul1.UbgT.Length / 10.0);
                                byte b5 = 1;
                                while (unchecked(b5 <= (uint)b4))
                                {
                                    if (Operators.CompareString(text, Strings.Mid(Modul1.UbgT, unchecked(b5) * 10 - 9, 10), TextCompare: false) == 0)
                                    {
                                        Modul1.FamInArb = text.AsInt();
                                        Modul1.Schalt = 1;
                                        b6 = (byte)MannHeilist.Items.Count;
                                        b7 = 0;
                                        goto IL_0fe7;
                                    }
                                    b5 = (byte)unchecked((uint)(b5 + 1));
                                }
                                goto IL_1006;
                            }
                            goto IL_1078;
                        IL_1078: // <========== 5
                            num = 201;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            famInArb = Modul1.FamInArb;
                            Retweg2(Anz[0]);
                            Anz[0].SelectedText = "\n";
                            Heidat(0, 0);
                            leerweg(Anz[0]);
                            Retweg2(Anz[0]);
                            Anz[0].SelectedText = "\n";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectedText = " mit\n";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            goto IL_11ac;
                        IL_11ac: // <========== 5
                            num = 215;
                            Lauf += ">";
                            Bezeichnung1[2].Text = Lauf;
                            Bezeichnung1[2].Refresh();
                            if (Lauf.Length >= 50)
                            {
                                Lauf = "";
                            }
                            goto IL_1227;
                        IL_1227:
                            num = 221;
                            Anz[0].SelectionHangingIndent = 20;
                            if (DataModul.DT_AncesterTable.Fields["Ehe"].AsInt() != 0)
                            {
                                num9 = DataModul.DT_AncesterTable.Fields["Ehe"].AsInt();
                            }
                            goto IL_1295;
                        IL_1295:
                            num = 225;
                            Modul1.Schalt = 0;
                            Ahn = DataModul.DT_AncesterTable.Fields["Ahn"].AsString();
                            Modul1.PersInArb = DataModul.DT_AncesterTable.Fields["PerNr"].AsInt();
                            if (Modul1.PersInArb != 0)
                            {
                                if (Modul1.PersInArb == 0)
                                {
                                    Modul1.FamInArb = DataModul.DT_AncesterTable.Fields["Ehe"].AsInt();
                                    goto IL_31ed;
                                }
                                DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb.AsString());
                                left = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString();
                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                Application.DoEvents();
                                if (Modul1.Kont[4] != "")
                                {
                                    Modul1.Alias_Renamed(Modul1.Kont[4]);
                                }
                                goto IL_1411;
                            }
                            goto IL_31ed;
                        IL_1411:
                            num = 242;
                            Anz[0].SelectionIndent = 0;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            if (DataModul.DT_AncesterTable.Fields["Gen"].AsInt() > b8
                                | (DataModul.DT_AncesterTable.Fields["Gen"].AsInt() == 0))
                            {
                                Bezeichnung1[1].Text = "Bearbeite Generation " + DataModul.DT_AncesterTable.Fields["Gen"].AsString();
                                M1_Ki = false;
                                Bezeichnung1[1].Refresh();
                                if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 3) != "\n\n\n")
                                {
                                    if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 3) == "\n\n")
                                    {
                                        Anz[0].SelectedText = "\n";
                                        goto IL_1681;
                                    }
                                    if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 3) == "\n")
                                    {
                                        Anz[0].SelectedText = "\n\n\n";
                                        goto IL_1681;
                                    }
                                    if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
                                    {
                                        Anz[0].SelectedText = "\n\n\n\n";

                                    }
                                }
                                goto IL_1681;
                            }
                            goto IL_1754;
                        IL_1681: // <========== 5
                            num = 263;
                            lErl = 11;
                            string selectedText = "Generation " + DataModul.DT_AncesterTable.Fields["Gen"].AsString() + "\n";
                            Anz[0].SelectionCharOffset = 0;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                            Anz[0].SelectedText = selectedText;
                            b8 = (Byte)DataModul.DT_AncesterTable.Fields["Gen"].AsInt();
                            goto IL_1754;
                        IL_1754: // <========== 3
                            num = 270;
                            if (DataModul.DT_AncesterTable.Fields["Weiter"].AsString() != "0")
                            {
                                Ahn = DataModul.DT_AncesterTable.Fields["Ahn"].AsString();
                                DataModul.DT_AncesterTable.Index = "PerNr";
                                DataModul.DT_AncesterTable.Seek("=", Modul1.PersInArb);
                                goto IL_1821;
                            }
                            goto IL_1a11;
                        IL_1821: // <========== 3
                            num = 274;
                            lErl = 2;
                            if (!DataModul.DT_AncesterTable.NoMatch)
                            {
                                if (DataModul.DT_AncesterTable.Fields["PerNr"].AsInt() != Modul1.PersInArb)
                                {
                                    DataModul.DT_AncesterTable.Index = "Ahnen";
                                    DataModul.DT_AncesterTable.Seek("=", Ahn);
                                    goto IL_1a11;
                                }
                                AhnList.Items.Add(Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsDouble().AsString().Trim(), 28) + Strings.Right("   " + DataModul.DT_AncesterTable.Fields["Gen"].AsString(), 3));
                                DataModul.DT_AncesterTable.MoveNext();
                                if (!DataModul.DT_AncesterTable.EOF)
                                {
                                    goto IL_1821;
                                }
                                DataModul.DT_AncesterTable.Index = "Ahnen";
                                DataModul.DT_AncesterTable.Seek("=", Ahn);
                            }
                            goto IL_1a11;
                        IL_1a11: // <========== 4
                            num = 291;
                            lErl = 3;
                            Ahne = DataModul.DT_AncesterTable.Fields["Ahn"].AsLong();
                            DataModul.DT_SperrTable.Index = "Nr";
                            DataModul.DT_SperrTable.Seek("=", DataModul.DT_AncesterTable.Fields["PerNr"].Value);
                            if (!DataModul.DT_SperrTable.NoMatch)
                            {
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                Anz[0].SelectedText = Ahne + " " + Modul1.Datschuname + "\n";
                                goto IL_2a3b;
                            }
                            Modul1.Namenindex(Ahne);
                            M_Namen = Modul1.Kont[0];
                            Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person, Modul1.DAus[89] == "1"));
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectionCharOffset = 0;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            if ((AhnList.Items.Count == 0) | (Operators.CompareString(Strings.Trim(AhnList.Items[0].AsString().Left(28)), Ahne.AsString(), TextCompare: false) == 0))
                            {
                                if (AhnList.Items.Count > 0)
                                {
                                    if (Modul1.DAus[97].AsDouble() == 0.0)
                                    {
                                        Anz[0].SelectedText = Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsDouble().AsString(), _M_KonLen) + " = ";
                                        Anz[0].SelectedText = " " + Strings.Trim(AhnList.Items[1].AsString());
                                        if (AhnList.Items.Count > 2)
                                        {
                                            Anz[0].SelectedText = " >> ";
                                        }

                                    }
                                    else
                                    {
                                        Anz[0].SelectedText = Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsDouble().AsString(), _M_KonLen) + " >> ";
                                    }
                                    goto IL_1de3;
                                }
                                Anz[0].SelectedText = Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsDouble().AsString(), _M_KonLen);
                                goto IL_1ea7;
                            }
                            if (AhnList.Items.Count > 0)
                            {
                                if (Modul1.DAus[97].AsDouble() == 1.0)
                                {
                                    AhnList.Items.Clear();
                                    b3 = 1;
                                    DataModul.DT_AncesterTable.MoveNext();
                                    goto IL_0855;
                                }
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                Anz[0].SelectionCharOffset = 0;
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                Ahne = DataModul.DT_AncesterTable.Fields["Ahn"].AsLong();
                                if (Modul1.DAus[97].AsDouble() == 0.0)
                                {
                                    Anz[0].SelectedText = Ahne + " = ";
                                    Modul1.Schalt = 1;
                                    if (Operators.CompareString(Strings.Trim(AhnList.Items[0].AsString().Left(28)), Ahne.AsString(), TextCompare: false) != 0)
                                    {
                                        Anz[0].SelectedText = Strings.Trim(AhnList.Items[0].AsString().Left(28)) + "; " + Strings.Trim(Strings.Mid(AhnList.Items[0].AsString(), 29, 3));
                                        M_An++;
                                        goto IL_2863;
                                    }
                                    Anz[0].SelectedText = Strings.Trim(AhnList.Items[1].AsString().Left(28));
                                    M_An++;
                                }
                                goto IL_2863;
                            }
                            Anz[0].SelectedText = "        " + Strings.Right("          " + DataModul.DT_AncesterTable.Fields["Ahn"].AsString(), 10);
                            goto IL_290c;
                        IL_1de3: // <========== 3
                            num = 318;
                            Anz[0].SelectionCharOffset = 0;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            AhnList.Items.Clear();
                            goto IL_1ea7;
                        IL_1ea7: // <========== 3
                            num = 325;
                            if (Modul1.DAus[76] == "1")
                            {
                                Anz[0].SelectedText = " <" + Modul1.PersInArb.AsString().Trim() + ">";
                            }
                            goto IL_1eff;
                        IL_1eff:
                            num = 328;
                            Anz[0].SelectionHangingIndent = 20;
                            Anz[0].SelectedText = " " + (Modul1.Person.Prae.TrimEnd() + " " + Modul1.Kont[3]).Trim() + " ";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            Anz[0].SelectedText = Modul1.Kont[0];
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            if (Modul1.Kont[5].Trim() != "")
                            {
                                Anz[0].SelectedText = ", Sippe " + Modul1.Kont[5].TrimEnd();
                            }
                            goto IL_2036;
                        IL_2036:
                            num = 336;
                            if (Modul1.Kont[4].Trim() != "")
                            {
                                Anz[0].SelectedText = " (" + Modul1.Kont[4].ToUpper() + ")";
                            }
                            goto IL_208a;
                        IL_208a:
                            num = 339;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Modul1.PerSatzLes(Modul1.PersInArb);
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            if (Modul1.DAus[62] == "1")
                            {
                                PerQu1();
                            }
                            goto IL_2126;
                        IL_2126:
                            num = 345;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields["religi"].Value))
                            {
                                if (Operators.CompareString(DataModul.DB_PersonTable.Fields["religi"].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    Modul1.Ubg = DataModul.DB_PersonTable.Fields["religi"].AsInt();
                                    LD = "";
                                    Modul1.UbgT = DataModul.TextLese1(Modul1.Ubg);
                                    if (Modul1.UbgT.Trim() != "")
                                    {
                                        Anz[0].SelectedText = " Religion: " + Modul1.UbgT;
                                    }

                                }
                            }
                            goto IL_2252;
                        IL_2252: // <========== 3
                            num = 355;
                            if ((Modul1.DAus[115] == "1") | (Modul1.DAus[116] == "1"))
                            {
                                Module2.Bildaus("P", "FaBu");
                            }
                            goto IL_229e;
                        IL_229e:
                            num = 358;
                            if (Modul1.DAus[88] == "1")
                            {
                                Bild("P", Modul1.PersInArb);
                            }
                            goto IL_22d2;
                        IL_22d2:
                            num = 361;
                            Fackt = 1f;
                            if ((Modul1.DAus[99].AsDouble() == 1.0) & (Modul1.Kont[6].Trim() != ""))
                            {
                                Anz[0].SelectedText = " " + Modul1.Kont[6].TrimEnd() + " ";
                            }
                            goto IL_2356;
                        IL_2356:
                            num = 365;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Modul1.Datschalt = 1;
                            Listart = 2;
                            LD = 0.AsString();
                            Art = default;
                            neb = false;
                            Modul1.Datles3(Listart, default, Art, ref neb);
                            Modul1.Datschalt = 0;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            M_Indent = 20;
                            Datschreib(Modul1.PersInArb, document);
                            Modul1.Ja = 0;
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            M_Namen = Modul1.Kont[0];
                            if ((Modul1.DAus[38] == "1") | (Modul1.DAus[39] == "1"))
                            {
                                Sonst();
                            }
                            goto IL_24b8;
                        IL_24b8:
                            num = 379;
                            if ((Modul1.DAus[0] == "1") | (Modul1.DAus[13] == "1"))
                            {
                                Art = EEventArt.eA_300;
                                Berufe(ref Art);
                            }
                            goto IL_2503;
                        IL_2503:
                            num = 382;
                            if ((Modul1.DAus[16] == "1") | (Modul1.DAus[17] == "1"))
                            {
                                Art = EEventArt.eA_301;
                                Berufe(ref Art);
                            }
                            goto IL_254f;
                        IL_254f:
                            num = 385;
                            if ((Modul1.DAus[20] == "1") | (Modul1.DAus[21] == "1"))
                            {
                                Art = EEventArt.eA_302;
                                Berufe(ref Art);
                            }
                            goto IL_259b;
                        IL_259b:
                            num = 388;
                            if (Modul1.Ja == 1)
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            goto IL_2a2e;
                        IL_2863: // <========== 3
                            num = 416;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            AhnList.Items.Clear();
                            goto IL_290c;
                        IL_290c: // <========== 3
                            num = 422;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectedText = "   " + Modul1.Kont[3] + " ";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            Anz[0].SelectedText = Modul1.Kont[0];
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            if (Modul1.DAus[106].AsDouble() == 1.0)
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            goto IL_2a2e;
                        IL_2a2e: // <========== 3
                            num = 431;
                            Perabschluss();
                            goto IL_2a3b;
                        IL_2a3b: // <========== 3
                            num = 432;
                            if (inputStr.AsDouble() == 1.0)
                            {
                                Modul1.PerSatzLes(Modul1.PersInArb);
                                Modul1.eLKennz = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F" ? ELinkKennz.lkMother : ELinkKennz.lkFather;
                                goto IL_2abc;
                            }
                            M1_Ki = false;
                            Modul1.PerSatzLes(Modul1.PersInArb);
                            if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                            {
                                Modul1.eLKennz = ELinkKennz.lkMother;
                            }
                            else
                            {

                                Modul1.eLKennz = ELinkKennz.lkFather;
                            }
                            goto IL_2df2;
                        IL_2abc: // <========== 3
                            num = 440;
                            kennz = Modul1.eLKennz;
                            b9 = Conversions.ToByte(Modul1.eLKennz);
                            aiFams = DataModul.Link.GetPersonFams(Modul1.PersInArb, Modul1.eLKennz);
                            ubgT = Modul1.UbgT;
                            b10 = (byte)Math.Round(ubgT.Length / 10.0);
                            b11 = 1;
                            goto IL_2d6d;
                        IL_2bed:
                            num = 453;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Modul1.Famles();
                            switch (b9)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_2ca2;
                                default:
                                    goto IL_2cf9;
                            }
                            if (Modul1.Family.Mann != Modul1.PersInArb)
                            {
                                Anz[0].SelectedText = "\n";
                                Heidat(1, 1);
                            }
                            else
                            {

                                Heidat(1, 0);
                            }
                            goto IL_2cf9;
                        IL_2ca2:
                            num = 468;
                            if (Modul1.Family.Frau != Modul1.PersInArb)
                            {
                                Anz[0].SelectedText = "\n";
                                Heidat(1, 1);
                            }
                            else
                            {

                                Heidat(1, 0);
                            }
                            goto IL_2cf9;
                        IL_2cf9: // <========== 6
                            num = 476;
                            Anz[0].SelectedText = " mit\n";
                            Modul1.eLKennz = kennz;
                            Gatte();
                            Modul1.FamInArb = famInArb;
                            LDC = 0;
                            Kinder(ref LDC, M_An);
                            M_An = 0;
                            M_Indent = 20;
                            b11 = (byte)unchecked((uint)(b11 + 1));
                            goto IL_2d6d;
                        IL_2d6d:
                            if (unchecked(b11 <= (uint)b10))
                            {
                                text = Strings.Mid(ubgT, unchecked(b11) * 10 - 9, 10);
                                Modul1.FamInArb = (int)Math.Round(text.AsDouble());
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                M1_Ki = false;
                                famInArb = Modul1.FamInArb;
                                if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                                {
                                    Anz[0].SelectedText = "\n";
                                }
                                goto IL_2bed;
                            }
                            goto IL_2df2;
                        IL_2df2: // <========== 4
                            num = 494;
                            Modul1.FamInArb = DataModul.DT_AncesterTable.Fields["Ehe"].AsInt();
                            famInArb2 = Modul1.FamInArb;
                            aiFams = DataModul.Link.GetPersonFams(Modul1.PersInArb, Modul1.eLKennz);
                            eLKennz2 = default;
                            if (Modul1.UbgT.Length > 10)
                            {
                                text3 = "";
                                b12 = (byte)Math.Round(Modul1.UbgT.Length / 10.0);
                                b13 = 1;
                                goto IL_2eed;
                            }
                            goto IL_31d3;
                        IL_2edf:
                            num = 504;
                            b13 = (byte)unchecked((uint)(b13 + 1));
                            goto IL_2eed;
                        IL_2eed:
                            if (unchecked(b13 <= (uint)b12))
                            {
                                if (Strings.Mid(Modul1.UbgT, unchecked(b13) * 10 - 9, 10).AsDouble() != Modul1.FamInArb)
                                {
                                    text3 += Strings.Mid(Modul1.UbgT, unchecked(b13) * 10 - 9, 10);
                                }
                                goto IL_2edf;
                            }
                            b14 = (byte)Math.Round(text3.Length / 10.0);
                            b15 = 1;
                            goto IL_31ad;
                        IL_3040:
                            num = 518;
                            if (Modul1.eLKennz == ELinkKennz.lkFather)
                            {
                                Anz[0].SelectedText = "er in anderer Verbindung:";
                                goto IL_309d;
                            }
                            Anz[0].SelectedText = "sie in anderer Verbindung:";
                            goto IL_309d;
                        IL_309d: // <========== 3
                            num = 524;
                            if (eLKennz2 == default)
                            {
                                eLKennz2 = Modul1.eLKennz;
                            }
                            goto IL_30c2;
                        IL_30c2:
                            num = 527;
                            Modul1.FamInArb = (int)Math.Round(Strings.Mid(text3, unchecked(b15) * 10 - 9, 10).AsDouble());
                            Heidat(1, 0);
                            leerweg(Anz[0]);
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectedText = " mit \n";
                            int famInArb3 = Modul1.FamInArb;
                            Modul1.eLKennz = eLKennz2;
                            Gatte();
                            Modul1.FamInArb = famInArb3;
                            LDC = 1;
                            Kinder(ref LDC, M_An);
                            M_An = 0;
                            b15 = (byte)unchecked((uint)(b15 + 1));
                            goto IL_31ad;
                        IL_31ad:
                            if (unchecked(b15 <= (uint)b14))
                            {
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                SpRück = M_Indent;
                                Retweg2(Anz[0]);
                                Anz[0].SelectedText = "\n";
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                Anz[0].SelectionCharOffset = -10;
                                Anz[0].SelectedText = "*********************************************\n";
                                Anz[0].SelectionCharOffset = 0;
                                M1_Ki = false;
                                if (eLKennz2 != default)
                                {
                                    Modul1.eLKennz = eLKennz2;
                                }
                                goto IL_3040;
                            }
                            Anz[0].SelectedText = "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n";
                            goto IL_31d3;
                        IL_31d3: // <========== 3
                            num = 540;
                            Modul1.FamInArb = famInArb2;
                            Modul1.Famles();
                            goto IL_31ed;
                        IL_31ed: // <========== 5
                            num = 542;
                            lErl = 4;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            Retweg2(Anz[0]);
                            if (inputStr.AsDouble() == 1.0)
                            {
                                Quellen();
                            }
                            goto IL_325c;
                        IL_325c:
                            num = 548;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            DataModul.DT_AncesterTable.Index = "Ahnen";
                            string key = Strings.Right(new string(' ', 40) + Conversion.Str(Ahn.AsDouble() + 1d), 40);
                            DataModul.DT_AncesterTable.Seek(">=", key);
                            if (!DataModul.DT_AncesterTable.NoMatch)
                            {
                            }
                            else
                            {

                                DataModul.DT_AncesterTable.MoveNext();
                            }
                            goto IL_0855;
                        IL_3361:
                            num = 558;
                            Debugger.Break();
                            Anz[0].Visible = true;
                            goto end_IL_0000_2;
                        IL_33f8:
                            num = 568;
                            Texz1 = Anz[0].SelectionStart;
                            Anz[0].SelectionStart = 0;
                            Anz[0].SelectionLength = Texz1;
                            Texz++;
                            FileSystem.FileClose(99);
                            Fn = Modul1.GenFreeDir + "Temp\\Sptext" + Texz.AsString().Trim() + ".rtf";
                            FileSystem.FileOpen(99, Fn, OpenMode.Output);
                            FileSystem.PrintLine(99, Anz[0].SelectedRtf);
                            texz = Texz;
                            FileSystem.FileClose(99);
                            Anz[0].Text = "";
                            Anz[0].Text = "";
                            short num7 = texz;
                            num6 = 1;
                            while (num6 <= num7)
                            {
                                Fn = Modul1.GenFreeDir + "Temp\\Sptext" + num6.AsString().Trim() + ".rtf";
                                Anz[3].LoadFile(Fn);
                                FileSystem.Kill(Fn);
                                Texz1 = Anz[3].Text.Length;
                                Anz[3].SelectionStart = 0;
                                Anz[3].SelectionLength = Texz1;
                                Anz[0].SelectedRtf = Anz[3].SelectedRtf;
                                Anz[3].Text = "";
                                num6 = (short)unchecked(num6 + 1);
                            }
                            Anz[0].Enabled = true;
                            num6 = 0;
                            while ((short)unchecked(num6 + 1) <= 7)
                            {
                                Befehl[num6].Visible = true;
                            }
                            Anz[0].Visible = true;
                            goto end_IL_0000_2;
                        IL_38e6:
                            num4 = unchecked(num2 + 1);
                            goto IL_38ea;
                        IL_38ea:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 18:
                                    num = 18;
                                    MyProject.Forms.AW.ToolTip1.SetToolTip(MyProject.Forms.AW.Kontroll[73], "Ein Datum ab diesem Jahr wird nicht ausgegeben");
                                    MyProject.Forms.AW.Kontroll[75].Visible = false;
                                    MyProject.Forms.AW.Kontroll[89].Visible = true;
                                    MyProject.Forms.AW.Kontroll[91].Visible = true;
                                    MyProject.Forms.AW.Kontroll[92].Visible = true;
                                    MyProject.Forms.AW.Kontroll[93].Visible = true;
                                    MyProject.Forms.AW.Kontroll[97].Visible = true;
                                    MyProject.Forms.AW.Kontroll[100].Visible = true;
                                    MyProject.Forms.AW.Kontroll[73].Visible = false;
                                    MyProject.Forms.AW.Kontroll[73].CheckState = CheckState.Unchecked;
                                    num6 = 77;
                                    while (num6 <= 80)
                                    {
                                        MyProject.Forms.AW.Kontroll[num6].Visible = false;
                                        num6 = (short)unchecked(num6 + 1);
                                    }
                                    MyProject.Forms.AW.Kontroll[78].Visible = true;
                                    num6 = 57;
                                    goto case 33;
                                case 33:
                                    do
                                    {
                                        num = 33;
                                        num6 = (short)unchecked(num6 + 1);
                                    }
                                    while (num6 <= 61);
                                    goto IL_026a;
                                case 17:
                                case 34:
                                    goto IL_026a;
                                case 50:
                                case 51:
                                case 52:
                                    goto IL_0366;
                                case 49:
                                case 58:
                                case 59:
                                case 60:
                                    goto IL_03f0;
                                case 43:
                                case 44:
                                case 61:
                                    goto IL_03fe;
                                case 93:
                                case 398:
                                case 554:
                                case 555:
                                    goto IL_0855;
                                case 113:
                                case 114:
                                    goto IL_0a26;
                                case 120:
                                case 121:
                                    goto IL_0aad;
                                case 127:
                                case 128:
                                    goto IL_0ad4;
                                case 130:
                                case 131:
                                    goto IL_0b00;
                                case 134:
                                case 135:
                                    goto IL_0b84;
                                case 139:
                                case 140:
                                    goto IL_0be2;
                                case 143:
                                case 144:
                                case 145:
                                    goto IL_0c3f;
                                case 146:
                                case 147:
                                    goto IL_0c49;
                                case 151:
                                case 152:
                                case 153:
                                    goto IL_0c9d;
                                case 184:
                                case 185:
                                    goto IL_0fd9;
                                case 186:
                                case 189:
                                    goto IL_1006;
                                case 190:
                                case 193:
                                case 196:
                                case 197:
                                case 200:
                                case 201:
                                    goto IL_1078;
                                case 123:
                                case 212:
                                case 213:
                                case 214:
                                case 215:
                                    goto IL_11ac;
                                case 220:
                                case 221:
                                    goto IL_1227;
                                case 224:
                                case 225:
                                    goto IL_1295;
                                case 241:
                                case 242:
                                    goto IL_1411;
                                case 249:
                                case 253:
                                case 257:
                                case 261:
                                case 262:
                                case 263:
                                    goto IL_1681;
                                case 269:
                                case 270:
                                    goto IL_1754;
                                case 274:
                                case 287:
                                case 288:
                                    goto IL_1821;
                                case 279:
                                case 286:
                                case 289:
                                case 290:
                                case 291:
                                    goto IL_1a11;
                                case 313:
                                case 314:
                                case 317:
                                case 318:
                                    goto IL_1de3;
                                case 321:
                                case 324:
                                case 325:
                                    goto IL_1ea7;
                                case 327:
                                case 328:
                                    goto IL_1eff;
                                case 335:
                                case 336:
                                    goto IL_2036;
                                case 338:
                                case 339:
                                    goto IL_208a;
                                case 344:
                                case 345:
                                    goto IL_2126;
                                case 352:
                                case 353:
                                case 354:
                                case 355:
                                    goto IL_2252;
                                case 357:
                                case 358:
                                    goto IL_229e;
                                case 360:
                                case 361:
                                    goto IL_22d2;
                                case 364:
                                case 365:
                                    goto IL_2356;
                                case 378:
                                case 379:
                                    goto IL_24b8;
                                case 381:
                                case 382:
                                    goto IL_2503;
                                case 384:
                                case 385:
                                    goto IL_254f;
                                case 387:
                                case 388:
                                    goto IL_259b;
                                case 410:
                                case 414:
                                case 415:
                                case 416:
                                    goto IL_2863;
                                case 418:
                                case 421:
                                case 422:
                                    goto IL_290c;
                                case 390:
                                case 391:
                                case 429:
                                case 430:
                                case 431:
                                    goto IL_2a2e;
                                case 298:
                                case 432:
                                    goto IL_2a3b;
                                case 436:
                                case 439:
                                case 440:
                                    goto IL_2abc;
                                case 452:
                                case 453:
                                    goto IL_2bed;
                                case 456:
                                case 462:
                                case 465:
                                case 466:
                                case 471:
                                case 474:
                                case 475:
                                case 476:
                                    goto IL_2cf9;
                                case 483:
                                case 489:
                                case 492:
                                case 493:
                                case 494:
                                    goto IL_2df2;
                                case 503:
                                case 504:
                                    goto IL_2edf;
                                case 517:
                                case 518:
                                    goto IL_3040;
                                case 520:
                                case 523:
                                case 524:
                                    goto IL_309d;
                                case 526:
                                case 527:
                                    goto IL_30c2;
                                case 539:
                                case 540:
                                    goto IL_31d3;
                                case 155:
                                case 229:
                                case 233:
                                case 542:
                                    goto IL_31ed;
                                case 547:
                                case 548:
                                    goto IL_325c;
                                case 567:
                                case 568:
                                    goto IL_33f8;
                                case 596:
                                    num = 596;
                                    num5 = texz;
                                    num6 = 1;
                                    Fn = Modul1.GenFreeDir + "Temp\\Sptext" + num6.AsString().Trim() + ".rtf";
                                    Anz[3].LoadFile(Fn);
                                    FileSystem.Kill(Fn);
                                    Texz1 = Anz[3].Text.Length;
                                    Anz[3].SelectionStart = 0;
                                    Anz[3].SelectionLength = Texz1;
                                    MyProject.Computer.Clipboard.SetText(Anz[3].SelectedRtf, TextDataFormat.Rtf);
                                    Anz[0].SelectedText = MyProject.Computer.Clipboard.GetText(TextDataFormat.Rtf);
                                    num6 = (short)unchecked(num6 + 1);
                                    if (num6 <= num5)
                                    {
                                        goto case 596;
                                    }
                                    Anz[0].Visible = true;
                                    goto end_IL_0000_2;
                                case 12:
                                case 39:
                                case 561:
                                case 595:
                                case 607:
                                case 614:
                                case 620:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 17064;
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
    private void Command2_Click(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
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
                    int AAA;
                    string LD;
                    int ortNr;
                    int ortNr2;
                    int ortNr3;
                    byte Schalt;
                    int index;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            index = Command2.GetIndex((Button)eventSender);
                            goto IL_0015;
                        case 6452:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_15ee;
                                    default:
                                        goto end_IL_0000;
                                }
                                int number = Information.Err().Number;
                                if (number is 3022 or 3211 or 3010 or 3376)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_15ee;
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
                                goto IL_15f2;
                            }
                        end_IL_0000:
                            break;
                        IL_0015:
                            num = 2;
                            Frame2.Visible = false;
                            Modul1.Ind1 = "";
                            Anz[3].Text = "";
                            Anz[3].SelectionHangingIndent = (int)Math.Round(0.0 * 1440.0 / DeviceDpi);
                            switch (index)
                            {
                                case 0:
                                    break;
                                case 1:
                                    goto IL_04ec;
                                case 2:
                                    goto IL_0a59;
                                case 3:
                                    goto IL_0eaf;
                                case 4:
                                    goto IL_1077;
                                default:
                                    goto IL_1526;
                            }
                            Anz[3].SelectionAlignment = HorizontalAlignment.Center;
                            Anz[3].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 1.3), FontStyle.Bold);
                            Anz[3].SelectedText = "Ortsindex";
                            Anz[3].SelectedText = "\n";
                            DataModul.DSB_OrtIdxTable.Index = "Ort";
                            DataModul.DSB_OrtIdxTable.Seek(">=", " ");
                            goto IL_0474;
                        IL_03d6: // <========== 3
                            num = 39;
                            if (DataModul.DSB_OrtIdxTable.Fields["Ind"].AsInt() != num5)
                            {
                                Anz[3].SelectedText = DataModul.DSB_OrtIdxTable.Fields["Ind"].AsString().Trim() + "; ";
                                num5 = DataModul.DSB_OrtIdxTable.Fields["Ind"].AsInt();
                            }
                            goto IL_0466;
                        IL_0466:
                            num = 43;
                            DataModul.DSB_OrtIdxTable.MoveNext();
                            goto IL_0474;
                        IL_0474: // <========== 3
                            num = 17;
                            if (!DataModul.DSB_OrtIdxTable.EOF)
                            {
                                if (DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt() != Modul1.AltNr)
                                {
                                    Anz[3].SelectionAlignment = HorizontalAlignment.Left;
                                    Anz[3].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                                    Anz[3].SelectedText = "\n";
                                    Anz[3].SelectionIndent = 0;
                                    Modul1.UbgT = Modul1.ortles(DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt(), 21);
                                    Anz[3].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                                    Anz[3].SelectedText = Modul1.UbgT;
                                    Modul1.AltNr = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                                    num5 = 0;
                                    Anz[3].SelectedText = "\n";
                                    Anz[3].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    Anz[3].SelectionIndent = 20;
                                    if (Check1.Checked)
                                    {
                                        if (Operators.CompareString(DataModul.DB_PlaceTable.Fields[PlaceFields.Bem].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            Anz[3].SelectedText = DataModul.DB_PlaceTable.Fields[PlaceFields.Bem].AsString();
                                            Anz[3].SelectedText = "\n";
                                        }

                                    }
                                }
                                goto IL_03d6;
                            }
                            Befehl[4].Visible = true;
                            Anz[3].SaveFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
                            Anz[3].LoadFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
                            goto end_IL_0000_2;
                        IL_04ec:
                            num = 50;
                            Anz[3].SelectionAlignment = HorizontalAlignment.Center;
                            Anz[3].Font = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 1.3), FontStyle.Bold);
                            Anz[3].SelectedText = "Index Orte-Namen";
                            Anz[3].SelectedText = "\n";
                            Anz[3].SelectionHangingIndent = (int)Math.Round(0.0 * 1440.0 / DeviceDpi);
                            DataModul.DSB_OrtIdxTable.Index = "ortnam";
                            DataModul.DSB_OrtIdxTable.Seek(">=", " ");
                            goto IL_09e1;
                        IL_085f: // <========== 3
                            num = 80;
                            if (Operators.CompareString(DataModul.DSB_OrtIdxTable.Fields["Name"].AsString().Trim(), Modul1.AltName.Trim(), TextCompare: false) != 0)
                            {
                                if (Modul1.AltName != "")
                                {
                                    Anz[3].SelectedText = "\n";
                                }
                                goto IL_08cd;
                            }
                            goto IL_0991;
                        IL_08cd:
                            num = 84;
                            Anz[3].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            Anz[3].SelectedText = (DataModul.DSB_OrtIdxTable.Fields["Name"].Value + "  ").AsString();
                            Anz[3].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Modul1.AltName = DataModul.DSB_OrtIdxTable.Fields["Name"].AsString();
                            goto IL_0991;
                        IL_0991: // <========== 3
                            num = 89;
                            Anz[3].SelectedText = DataModul.DSB_OrtIdxTable.Fields["Ind"].AsString().Trim() + "; ";
                            DataModul.DSB_OrtIdxTable.MoveNext();
                            goto IL_09e1;
                        IL_09e1: // <========== 3
                            num = 58;
                            if (!DataModul.DSB_OrtIdxTable.EOF)
                            {
                                if (DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt() != Modul1.AltNr)
                                {
                                    Anz[3].SelectedText = "\n";
                                    Anz[3].SelectionAlignment = HorizontalAlignment.Left;
                                    Anz[3].SelectionIndent = 0;
                                    Modul1.AltName = "";
                                    Modul1.AltNr = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                                    Anz[3].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    ortNr2 = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                                    Modul1.UbgT = Modul1.ortles(ortNr2, 21);
                                    Anz[3].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                                    Anz[3].SelectedText = Modul1.UbgT;
                                    Anz[3].SelectedText = "\n";
                                    Anz[3].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    Anz[3].SelectionIndent = 20;
                                    if (Check1.Checked)
                                    {
                                        if (Operators.CompareString(DataModul.DB_PlaceTable.Fields[PlaceFields.Bem].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            Anz[3].SelectedText = DataModul.DB_PlaceTable.Fields[PlaceFields.Bem].AsString();
                                            Anz[3].SelectedText = "\n";
                                        }

                                    }
                                }
                                goto IL_085f;
                            }
                            Befehl[4].Visible = true;
                            Anz[3].SaveFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
                            Anz[3].LoadFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
                            goto end_IL_0000_2;
                        IL_0a59:
                            num = 97;
                            Anz[3].SelectionAlignment = HorizontalAlignment.Center;
                            Anz[3].Font = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 1.3), FontStyle.Bold);
                            Anz[3].SelectedText = "Index Namen-Orte";
                            Anz[3].SelectedText = "\n";
                            DataModul.DSB_OrtIdxTable.Index = "NameOrt";
                            DataModul.DSB_OrtIdxTable.Seek(">=", " ");
                            goto IL_0e2e;
                        IL_0cc2:
                            num = 117;
                            if (DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt() != Modul1.AltNr)
                            {
                                if (Modul1.AltNr > 0)
                                {
                                    Anz[3].SelectedText = "\n";
                                }
                                goto IL_0d1a;
                            }
                            goto IL_0dde;
                        IL_0d1a:
                            num = 121;
                            Anz[3].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            Anz[3].SelectedText = (DataModul.DSB_OrtIdxTable.Fields["Ort"].Value + "  ").AsString();
                            Anz[3].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Modul1.AltNr = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                            goto IL_0dde;
                        IL_0dde: // <========== 3
                            num = 126;
                            Anz[3].SelectedText = DataModul.DSB_OrtIdxTable.Fields["Ind"].AsString().Trim() + "; ";
                            DataModul.DSB_OrtIdxTable.MoveNext();
                            goto IL_0e2e;
                        IL_0e2e: // <========== 3
                            num = 104;
                            if (!DataModul.DSB_OrtIdxTable.EOF)
                            {
                                if (Operators.CompareString(DataModul.DSB_OrtIdxTable.Fields["Name"].AsString().Trim(), Modul1.AltName.Trim(), TextCompare: false) != 0)
                                {
                                    Anz[3].SelectionAlignment = HorizontalAlignment.Left;
                                    Anz[3].SelectedText = "\n";
                                    Anz[3].SelectionIndent = 0;
                                    Modul1.AltNr = 0;
                                    Anz[3].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                                    Anz[3].SelectedText = DataModul.DSB_OrtIdxTable.Fields["Name"].AsString();
                                    Modul1.AltName = DataModul.DSB_OrtIdxTable.Fields["Name"].AsString();
                                    Anz[3].SelectedText = "\n";
                                    Anz[3].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    Anz[3].SelectionIndent = 20;
                                }
                                goto IL_0cc2;
                            }
                            Befehl[4].Visible = true;
                            Anz[3].SaveFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
                            Anz[3].LoadFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
                            goto end_IL_0000_2;
                        IL_0eaf:
                            num = 134;
                            Modul1.PrintDat.Flagsch = 1;
                            DataModul.DSB_OrtIdxTable.Index = "Ort";
                            DataModul.DSB_OrtIdxTable.Seek(">=", " ");
                            goto IL_105c;
                        IL_104b:
                            num = 147;
                            DataModul.DSB_OrtIdxTable.MoveNext();
                            goto IL_105c;
                        IL_105c: // <========== 3
                            num = 138;
                            if (DataModul.DSB_OrtIdxTable.EOF)
                            {
                                goto end_IL_0000_2;
                            }
                            if (DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt() != Modul1.AltNr)
                            {
                                Anz[3].SelectionIndent = (int)Math.Round(0.0 * 1440.0 / DeviceDpi);
                                Modul1.UbgT = Modul1.ortles(DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt(), 2);
                                Anz[3].SelectedText = Modul1.UbgT;
                                Modul1.AltNr = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                                Anz[3].SelectedText = "\n";
                            }
                            goto IL_104b;
                        IL_1077:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            Anz[3].SelectionAlignment = HorizontalAlignment.Center;
                            DataModul.TempDB.Execute($"DROP Table {dbTables.OT};");
                            DataModul.TempDB.Execute($"CREATE Table {dbTables.OT} (OT TEXT(240)Not NULL,OrT TEXT(240)Not NULL);");
                            DataModul.TempDB.Execute($"CREATE UNIQUE INDEX Name ON {dbTables.OT} ([OT],[Ort]);");
                            DataModul.DT_OTTable = DataModul.TempDB.OpenRecordset(dbTables.OT, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DSB_OrtIdxTable.MoveFirst();
                            goto IL_12f9;
                        IL_12e8: // <========== 3
                            num = 171;
                            DataModul.DSB_OrtIdxTable.MoveNext();
                            goto IL_12f9;
                        IL_12f9: // <========== 3
                            num = 159;
                            if (!DataModul.DSB_OrtIdxTable.EOF)
                            {
                                DataModul.DB_PlaceTable.Seek("=", DataModul.DSB_OrtIdxTable.Fields["OrtNr"]);
                                if (!DataModul.DB_PlaceTable.NoMatch)
                                {
                                    if (DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsDouble() > 0.0)
                                    {
                                        AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt();
                                        LD = "";
                                        Modul1.Kont[1] = DataModul.TextLese1(AAA);
                                        AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt();
                                        LD = "";
                                        Modul1.Kont[2] = DataModul.TextLese1(AAA);
                                        DataModul.DT_OTTable.AddNew();
                                        DataModul.DT_OTTable.Fields["OT"].Value = Modul1.Kont[1];
                                        DataModul.DT_OTTable.Fields["OrT"].Value = Modul1.Kont[2];
                                        DataModul.DT_OTTable.Update();

                                    }
                                }
                                goto IL_12e8;
                            }
                            Anz[3].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 1.3), FontStyle.Bold);
                            Anz[3].SelectedText = "\nOrtsteil-Liste\n\n";
                            Anz[3].SelectionAlignment = HorizontalAlignment.Left;
                            DataModul.DT_OTTable.Index = "Name";
                            DataModul.DT_OTTable.MoveFirst();
                            while (!DataModul.DT_OTTable.EOF)
                            {
                                Modul1.Kont[1] = DataModul.DT_OTTable.Fields["OT"].AsString();
                                Modul1.Kont[2] = DataModul.DT_OTTable.Fields["OrT"].AsString();
                                Anz[3].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                                Anz[3].SelectedText = Modul1.Kont[1];
                                Anz[3].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                Anz[3].SelectedText = " siehe " + Modul1.Kont[2] + "-" + Modul1.Kont[1] + "\n\n";
                                DataModul.DT_OTTable.MoveNext();
                            }
                            Anz[3].Visible = true;
                            goto end_IL_0000_2;
                        IL_1526:
                            num = 191;
                            Interaction.MsgBox(index);
                            goto end_IL_0000_2;
                        IL_15ee:
                            num4 = unchecked(num2 + 1);
                            goto IL_15f2;
                        IL_15f2:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 36:
                                case 37:
                                case 38:
                                case 39:
                                    goto IL_03d6;
                                case 42:
                                case 43:
                                    goto IL_0466;
                                case 16:
                                case 17:
                                case 44:
                                    goto IL_0474;
                                case 77:
                                case 78:
                                case 79:
                                case 80:
                                    goto IL_085f;
                                case 83:
                                case 84:
                                    goto IL_08cd;
                                case 88:
                                case 89:
                                    goto IL_0991;
                                case 57:
                                case 58:
                                case 91:
                                    goto IL_09e1;
                                case 116:
                                case 117:
                                    goto IL_0cc2;
                                case 120:
                                case 121:
                                    goto IL_0d1a;
                                case 125:
                                case 126:
                                    goto IL_0dde;
                                case 103:
                                case 104:
                                case 128:
                                    goto IL_0e2e;
                                case 146:
                                case 147:
                                    goto IL_104b;
                                case 137:
                                case 138:
                                case 148:
                                    goto IL_105c;
                                case 169:
                                case 170:
                                case 171:
                                    goto IL_12e8;
                                case 158:
                                case 159:
                                case 172:
                                    goto IL_12f9;
                                case 7:
                                case 48:
                                case 95:
                                case 132:
                                case 149:
                                case 189:
                                case 192:
                                case 193:
                                case 205:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 6452;
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

    private void FaBu_Load(object eventSender, EventArgs eventArgs)
    {
        int num = default;
        int num3 = default;
        int number = default;
        int lErl = default;
        string source = default;
        string destination = default;
        string name = default;
        object instance = default;
        string text = default;

        int num4;
        short Listart;
        bool neb;
        object obj;
        object obj2;

        num = 1;
        BackColor = Modul1.HintFarb;

        num = 2;
        Befehl[3].Text = Modul1.IText[47];
        Modul1.Dateienopen();
        ProjectData.ClearProjectError();
        num3 = 2;
        FileSystem.MkDir(Modul1.Verz + "FABU");
        var hinter = MyProject.Forms.Hinter;
        var sAhne = Modul1.Verz + "FABU";
        hinter.Att(sAhne);
        FileSystem.Kill(Modul1.Verz + "FABU\\*.*");
        Modul1.Verz1 = Modul1.Verz.Left(15);
        source = Modul1.GenFreeDir + "INIT\\GedAUS.mdb";
        destination = Modul1.Verz + "FABU\\GEDAUS.mdb";
        FileSystem.FileCopy(source, destination);
        name = Modul1.Verz + "FABU\\GEDAUS.mdb";
        DataModul.NB = UpgradeSupport.DAODBEngine_definst.OpenDatabase(name, false, false, "");
        instance = DataModul.NB.OpenRecordset(dbTables.Orte, RecordsetTypeEnum.dbOpenTable);
        NewLateBinding.LateSet(instance, null, "Index", new object[1] { "ORT" }, null, null);
        obj = DataModul.NB.OpenRecordset(dbTables.QuellTemp, RecordsetTypeEnum.dbOpenTable);
        obj2 = DataModul.NB.OpenRecordset(dbTables.Bilder, RecordsetTypeEnum.dbOpenTable);
        DataModul.NB_PersonTable = DataModul.NB.OpenRecordset(dbTables.Personen1, RecordsetTypeEnum.dbOpenTable);
        DataModul.NB_FamilyTable = DataModul.NB.OpenRecordset(dbTables.Familie1, RecordsetTypeEnum.dbOpenTable);
        Font = Font.ChangeFName(Modul1.Font1);
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
        Font = new Font("Arial", Modul1.Fs, FontStyle.Regular);
        Show();
        Bezeichnung1[0].Width = Width;
        ProjectData.ClearProjectError();
        num3 = 3;
        Modul1.Druck_Tast = 1;
        FileSystem.FileClose(30);
        Command_Renamed[1].Enabled = false;
        Label1[3].Text = "Keine Berechnung vorhanden";
        Label1[0].Text = "Sie müssen erst die Ahnen berechnen.";
        DataModul.DT_AncesterTable.Index = "Ahnen";
        DataModul.DT_AncesterTable.Seek("=", 1);
        DataModul.DT_AncesterTable.MoveFirst();
        if (!DataModul.DT_AncesterTable.EOF)
        {
            if (!DataModul.DT_AncesterTable.NoMatch)
            {
                Command_Renamed[1].Enabled = true;
                text = DataModul.DT_AncesterTable.Fields["Ahn"].AsString();
                DataModul.DT_AncesterTable.MoveLast();
                Label1[3].Text = "Ahnenberechnung " + DataModul.DT_AncesterTable.Fields["Gen"].AsString() + " Generationen vorhanden für Ahnenziffer " + text;
                DataModul.DT_AncesterTable.MoveFirst();
                Modul1.PersInArb = DataModul.DT_AncesterTable.Fields["PerNr"].AsInt();
                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person, Modul1.DAus[89] == "1"));
                Label1[0].Text = (Modul1.Person.Prae.TrimEnd() + " " + Modul1.Kont[3] + " " + Modul1.Kont[0]).Trim();
                Listart = 0;
                neb = false;
                Modul1.Datles3(Listart, default, default, ref neb);

            }
        }
        lErl = 1;
        Label1[5].Text = Modul1.IText[3] + " " + Modul1.Kont[11];
        Label1[2].Text = Modul1.IText[5] + " " + Modul1.Kont[13];
        Label1[1].Text = Modul1.IText[4] + " " + Modul1.Kont[12];
        Label1[4].Text = Modul1.DTxt[4] + " " + Modul1.Kont[14];
    }

    public void Bemaus()
    {
        if (Modul1.DAus[72] == "1")
        {
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
        }
        Anz[0].SelectedText = "{";
        if (Modul1.DAus[70].AsDouble() == 0.0)
        {
            Modul1.UbgT1 = Modul1.Retweg(Modul1.UbgT1);
        }
        Anz[0].SelectedText = Modul1.UbgT1;
        Anz[0].SelectionCharOffset = 0;
        Anz[0].SelectedText = "}";
        Modul1.UbgT1 = "";
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
                checked
                {
                    int num4;
                    int AAA;
                    string LD;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 1492:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_04d2;
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
                                goto IL_04d6;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            List2.Items.Clear();
                            DataModul.DB_EventTable.Index = "Besu";
                            DataModul.DB_EventTable.Seek("=", "105", Modul1.PersInArb.AsString());
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                DataModul.DB_EventTable.Index = "ArtNr";
                                goto end_IL_0000_2;
                            }
                            Modul1.Ja = 1;
                            b = 1;
                            goto IL_00c0;
                        IL_00c0: // <========== 3
                            num = 11;
                            if (!DataModul.DB_EventTable.EOF)
                            {
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    DataModul.DB_EventTable.Index = "ArtNr";
                                    goto end_IL_0000_2;
                                }
                                if (!Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                                {
                                    M1_J = 0;
                                    while (unchecked(M1_J) <= 15u)
                                    {
                                        Modul1.Kont1[M1_J] = "";
                                        M1_J = (byte)unchecked((uint)(M1_J + 1));
                                    }
                                    Modul1.sDatu = "";
                                    if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != Modul1.PersInArb) | (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() != 105)))
                                    {
                                        DataModul.DB_EventTable.Index = "ArtNr";
                                        goto IL_047a;
                                    }
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                    {
                                        Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                        Modul1.Kont1[1] = Modul1.sDatu;
                                    }
                                    Modul1.UbgT = "";
                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value))
                                    {
                                        if (DataModul.DB_EventTable.Fields[EventFields.ArtText].AsDouble() > 0.0)
                                        {
                                            AAA = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.ArtText].AsDouble());
                                            LD = "";
                                            Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                            if (Modul1.Kont[0] != "")
                                            {
                                                Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim() + ": ";
                                            }
                                        }
                                    }
                                    goto IL_035d;
                                }
                                goto IL_0450;
                            }
                            goto IL_047a;
                        IL_035d: // <========== 3
                            num = 42;
                            if (Modul1.DAus[103].AsBool())
                            {
                                text = Modul1.Kont1[1] + Modul1.Kont1[7] + new string(' ', 240).Left(240) + DataModul.DB_EventTable.Fields[EventFields.LfNr].AsString();

                            }
                            else
                            {
                                text = Modul1.Kont1[7] + Modul1.Kont1[1] + new string(' ', 240).Left(240) + DataModul.DB_EventTable.Fields[EventFields.LfNr].AsString();
                            }
                            goto IL_0422;
                        IL_0422: // <========== 3
                            num = 48;
                            if (text.Trim() != "")
                            {
                                List2.Items.Add(text);
                            }
                            goto IL_0450;
                        IL_0450: // <========== 3
                            num = 51;
                            lErl = 299;
                            DataModul.DB_EventTable.MoveNext();
                            b = (byte)unchecked((uint)(b + 1));
                            if (unchecked(b) <= 70u)
                            {
                                goto IL_00c0;
                            }
                            goto IL_047a;
                        IL_047a: // <========== 4
                            num = 54;
                            Sonstdat();
                            goto end_IL_0000_2;
                        IL_04d2:
                            num4 = unchecked(num2 + 1);
                            goto IL_04d6;
                        IL_04d6:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 11:
                                    goto IL_00c0;
                                case 39:
                                case 40:
                                case 41:
                                case 42:
                                    goto IL_035d;
                                case 44:
                                case 47:
                                case 48:
                                    goto IL_0422;
                                case 19:
                                case 50:
                                case 51:
                                    goto IL_0450;
                                case 12:
                                case 27:
                                case 54:
                                    goto IL_047a;
                                case 7:
                                case 16:
                                case 55:
                                case 60:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 1492;
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
    public void Sonstdat()
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        float M_Sgg = 1f;
        string text = default;
        int num5 = default;
        string Job = default;
        string text3 = default;
        int num6 = default;
        string text4 = default;
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
                            {
                                num = 1;
                                Job = "";
                                text3 = "";
                                array = new object[6];
                                ProjectData.ClearProjectError();
                                num3 = 2;
                                num6 = List2.Items.Count - 1;
                                num5 = 0;
                                goto IL_1033;
                            }
                        case 5035:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_10b5;
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
                                goto IL_10b9;
                            }
                        end_IL_0000:
                            break;
                        IL_02a9: // <========== 3
                            num = 27;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                            {
                                Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                Modul1.sDatu = "00000000" + Modul1.sDatu.Right(8);
                                text3 = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                if ((text3.Trim() == "") & DataModul.DB_EventTable.Fields[EventFields.DatumB].AsDate() != default)
                                {
                                    text3 = " von";
                                }
                                Modul1.sDatu = Modul1.Datwand1(Modul1.sDatu, text3);
                                Modul1.Kont1[1] = Modul1.sDatu;
                            }
                            text4 = text3;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                if (text3 == "")
                                {
                                    Modul1.Kont1[1] = "von " + Modul1.Kont1[1];
                                }
                                Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                Modul1.sDatu = "00000000" + Modul1.sDatu.Right(8);
                                text3 = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                if (text3 == "")
                                {
                                    string left = text4.Trim();
                                    if (left == "von")
                                    {
                                        text3 = "b";

                                    }
                                }
                                goto IL_04c7;
                            }
                            goto IL_05b5;
                        IL_04c7: // <========== 3
                            num = 53;
                            Modul1.sDatu = Modul1.Datwand1(Modul1.sDatu, text3);
                            if (text4 == "")
                            {
                                if (Modul1.sDatu.Trim() != "")
                                {
                                    Modul1.sDatu = " bis " + Modul1.sDatu.Trim();

                                }
                            }
                            goto IL_0523;
                        IL_0523: // <========== 3
                            num = 59;
                            if (text4 == "z")
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString().Trim(), "", TextCompare: false) == 0)
                                {
                                    if (Modul1.sDatu.Trim() != "")
                                    {
                                        Modul1.sDatu = " und " + Modul1.sDatu.Trim();
                                    }

                                }
                            }
                            goto IL_05a5;
                        IL_05a5: // <========== 3
                            num = 66;
                            Modul1.Kont1[3] = Modul1.sDatu;
                            goto IL_05b5;
                        IL_05b5: // <========== 3
                            num = 68;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.DatumText].Value))
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt() > 0)
                                {
                                    string value = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsString();
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
                            goto IL_06b0;
                        IL_06b0: // <========== 3
                            num = 78;
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                            {
                                ortNr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble());
                                Modul1.UbgT = Modul1.ortles(ortNr, 1);
                                Modul1.Kont1[5] = " " + Modul1.UbgT.Trim();
                                Modul1.UbgT = "";
                            }
                            goto IL_0746;
                        IL_0746:
                            num = 83;
                            if (DataModul.DB_EventTable.Fields[EventFields.Platz].AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                LD = "";
                                Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                if (Modul1.Kont[0] != "")
                                {
                                    Modul1.Kont1[8] = " " + Modul1.Kont[0].Trim();

                                }
                            }
                            goto IL_07fe;
                        IL_07fe: // <========== 3
                            num = 89;
                            if (Modul1.DAus[62] == "1")
                            {
                                Nr = Modul1.PersInArb;
                                _eArt = EEventArt.eA_105;
                                LfNR = Modul1.LfNR;
                                Modul1.QuellenDatum(ref Nr, _eArt, ref LfNR);
                                Modul1.LfNR = Conversions.ToByte(LfNR);
                                Modul1.PersInArb = Nr.AsInt();
                            }
                            if (((Modul1.DAus[38] == "1") & !M1_Ki) | ((Modul1.DAus[42] == "1") & M1_Ki))
                            {
                                Job = Modul1.Kont1[7].TrimEnd();
                            }
                            else
                            {

                                if (((Modul1.DAus[39] == "1") & !M1_Ki) | ((Modul1.DAus[43] == "1") & M1_Ki))
                                {
                                    Job = "";
                                    Job = Module2.Jobdreh(Job);
                                }
                            }
                            goto IL_0909;
                        IL_0909: // <========== 3
                            num = 99;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value))
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.ArtText].AsDouble() > 0.0)
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                                    LD = "";
                                    Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                    if (Modul1.Kont[0] != "")
                                    {
                                        Modul1.Kont1[10] = " " + Modul1.Kont[0].Trim() + ": ";
                                    }

                                }
                            }
                            goto IL_09f3;
                        IL_09f3: // <========== 3
                            num = 107;
                            if (Modul1.Kont1[10].Trim() != "")
                            {
                                if (M1_Ki & (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0))
                                {
                                    if (Modul1.DAus[106] == "1")
                                    {
                                        Anz[0].SelectedText = "\n";

                                    }
                                }
                                goto IL_0a82;
                            }
                            goto IL_0baa;
                        IL_0a82: // <========== 3
                            num = 113;
                            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                            {
                                leerweg(Anz[0]);
                                if (Modul1.DAus[106] == "1")
                                {
                                    Anz[0].SelectedText = "\n";
                                    goto IL_0b0e;
                                }
                                Anz[0].SelectedText = " ";
                            }
                            goto IL_0b0e;
                        IL_0b0e: // <========== 3
                            num = 122;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                            Anz[0].SelectedText = Modul1.Kont1[10].Trim();
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectedText = " ";
                            goto IL_0baa;
                        IL_0baa: // <========== 3
                            num = 127;
                            Anz[0].SelectedText = Job + ". ";
                            if (((Modul1.DAus[40] == "1") & !M1_Ki) | ((Modul1.DAus[44] == "1") & M1_Ki))
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim();
                                    Bemaus();

                                }
                            }
                            goto IL_0c84;
                        IL_0c84: // <========== 3
                            num = 134;
                            if (((Modul1.DAus[41] == "1") & !M1_Ki) | ((Modul1.DAus[45] == "1") & M1_Ki))
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim();
                                    Bemaus();

                                }
                            }
                            goto IL_0d3d;
                        IL_0d3d: // <========== 3
                            num = 140;
                            if (Modul1.Kont1[9].Trim() != "")
                            {
                                Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                Anz[0].SelectedText = Modul1.Kont1[9];
                                Anz[0].SelectionCharOffset = 0;
                            }

                            byte b = 0;
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
                                Modul1.Zeugsu(_eArt, Modul1.LfNR, 2, Ahne);
                            }
                            text = Modul1.Kont1[20];
                            Modul1.Kont1[20] = "";
                            Modul1.PersInArb = Modul1.PersSp;
                            if (b == 1)
                            {
                                if (text != "")
                                {
                                    if (Modul1.DAus[100] == "1")
                                    {
                                        M_Sgg = 0.8f;
                                    }
                                    else
                                    {

                                        M_Sgg = 1f;
                                    }
                                    goto IL_0f29;
                                }
                            }
                            goto IL_1015;
                        IL_0f29: // <========== 3
                            num = 168;
                            Anz[0].SelectedText = " ";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Underline);
                            Anz[0].SelectedText = "Zeugen:";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Regular);
                            Anz[0].SelectedText = " " + text.Trim() + ".";
                            text = "";
                            goto IL_1015;
                        IL_1015: // <========== 3
                            num = 176;
                            DataModul.DB_EventTable.MoveNext();
                            num5++;
                            goto IL_1033;
                        IL_1033:
                            if (num5 <= num6)
                            {
                                Modul1.LfNR = Conversions.ToByte(List2.Items[num5].AsString().Right(10));
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", "105", Modul1.PersInArb.AsString(), Modul1.LfNR);
                                if (Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                                {
                                    Interaction.MsgBox("Stop 14");
                                }
                                M1_J = 0;
                                while (unchecked(M1_J) <= 15u)
                                {
                                    Modul1.Kont1[M1_J] = "";
                                    M1_J = (byte)unchecked((uint)(M1_J + 1));
                                }
                                Modul1.sDatu = "";
                                if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != Modul1.PersInArb) | (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() != 105)))
                                {
                                    DataModul.DB_EventTable.Index = "ArtNr";
                                    goto end_IL_0000_2;
                                }
                                if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsDouble() > 0.0)
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                    LD = "";
                                    Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                    if (Modul1.Kont[0] != "")
                                    {
                                        Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim();

                                    }
                                }
                                goto IL_02a9;
                            }
                            List2.Items.Clear();
                            goto end_IL_0000_2;
                        IL_10b5:
                            num4 = unchecked(num2 + 1);
                            goto IL_10b9;
                        IL_10b9:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 25:
                                case 26:
                                case 27:
                                    goto IL_02a9;
                                case 47:
                                case 51:
                                case 52:
                                case 53:
                                    goto IL_04c7;
                                case 57:
                                case 58:
                                case 59:
                                    goto IL_0523;
                                case 63:
                                case 64:
                                case 65:
                                case 66:
                                    goto IL_05a5;
                                case 67:
                                case 68:
                                    goto IL_05b5;
                                case 75:
                                case 76:
                                case 77:
                                case 78:
                                    goto IL_06b0;
                                case 82:
                                case 83:
                                    goto IL_0746;
                                case 87:
                                case 88:
                                case 89:
                                    goto IL_07fe;
                                case 94:
                                case 98:
                                case 99:
                                    goto IL_0909;
                                case 104:
                                case 105:
                                case 106:
                                case 107:
                                    goto IL_09f3;
                                case 111:
                                case 112:
                                case 113:
                                    goto IL_0a82;
                                case 117:
                                case 120:
                                case 121:
                                case 122:
                                    goto IL_0b0e;
                                case 126:
                                case 127:
                                    goto IL_0baa;
                                case 132:
                                case 133:
                                case 134:
                                    goto IL_0c84;
                                case 138:
                                case 139:
                                case 140:
                                    goto IL_0d3d;
                                case 164:
                                case 167:
                                case 168:
                                    goto IL_0f29;
                                case 174:
                                case 175:
                                case 176:
                                    goto IL_1015;
                                case 19:
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
                try0000_dispatch = 5035;
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

    private void FaBu_FormClosing(object eventSender, FormClosingEventArgs eventArgs)
    {
        //Discarded unreachable code: IL_00f8
        int try0000_dispatch = -1;
        int num = default;
        bool cancel = default;
        int num2 = default;
        int num3 = default;
        CloseReason closeReason = default;

        cancel = eventArgs.Cancel;

        num = 2;
        closeReason = eventArgs.CloseReason;
        ProjectData.ClearProjectError();
        num3 = 2;
        FileSystem.FileClose(6);
        FileSystem.FileOpen(6, Modul1.GenFreeDir + "\\Init\\Windowstate", OpenMode.Output);
        FileSystem.PrintLine(6, WindowState);
        DataModul.NB.Close();
        if (closeReason == 0)
        {
            DataModul.MandDB?.Close();
            DataModul.DOSB?.Close();
            DataModul.TempDB?.Close();
            DataModul.DSB?.Close();
        }
    }

    public void Urenkel()
    {
        Anz[0].SelectedText = "\n";
        Retweg2(Anz[0]);
        Anz[0].SelectionIndent = UrEnkelEinzug;
        if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n" && Modul1.Family.Kind[1] > 0)
        {
            Anz[0].SelectedText = "\n";
        }
        UrenList.Items.Clear();
        byte b = 1;
        checked
        {
            while (Modul1.Family.Kind[b] != 0)
            {
                DataModul.DB_EventTable.Seek("=", 101, Modul1.Family.Kind[b].AsString(), "0");
                if (!DataModul.DB_EventTable.NoMatch)
                {
                    Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                    if (Modul1.sDatu.Trim() != "0")
                    {
                        goto IL_01ff;
                    }
                }
                DataModul.DB_EventTable.Seek("=", 102.AsString(), Modul1.Family.Kind[b].AsString(), "0");
                if (DataModul.DB_EventTable.NoMatch)
                {
                    Modul1.sDatu = 0.AsString();
                }
                if (!DataModul.DB_EventTable.NoMatch)
                {
                    Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                }
                goto IL_01ff;
            IL_01ff:
                UrenList.Items.Add(Strings.Right("        " + Modul1.sDatu.AsDouble().AsString(), 8) + Modul1.Family.Kind[b].AsString());
                b = (byte)unchecked((uint)(b + 1));
                if (unchecked(b) > 99u)
                {
                    break;
                }
            }
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
            for (int i = 0; i < UrenList.Items.Count; i++)
            {
                Modul1.PersInArb = Conversions.ToInteger(Strings.Mid(UrenList.Items[i.AsInt()].AsString(), 9, Strings.Len(UrenList.Items[i.AsInt()].AsString())));
                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                Modul1.Namenindex(Ahne);
                Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person, Modul1.DAus[89] == "1"));
                Retweg2(Anz[0]);
                Anz[0].SelectedText = "\n";
                if (Modul1.DAus[76] == "1")
                {
                    Anz[0].SelectedText = " <" + Modul1.PersInArb.AsString().Trim() + "> ";
                }
                Anz[0].SelectedText = Conversions.ToString(Strings.Chr((97 + i).AsInt())) + Conversions.ToString(Strings.Chr((97 + i).AsInt())) + ") " + (Modul1.Person.Prae.TrimEnd() + " " + Modul1.Kont[3]).Trim();
                if (Operators.CompareString(Mannname.ToUpper().Trim(), Modul1.Kont[0].ToUpper().Trim(), TextCompare: false) != 0)
                {
                    Anz[0].SelectedText = " " + Modul1.Kont[0];
                }
                Modul1.PerSatzLes(Modul1.PersInArb);
                if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields["religi"].Value) && Operators.CompareString(DataModul.DB_PersonTable.Fields["religi"].AsString().Trim(), "", TextCompare: false) != 0)
                {
                    Modul1.Ubg = DataModul.DB_PersonTable.Fields["religi"].AsInt();
                    string LD;
                    Modul1.UbgT = DataModul.TextLese1(Modul1.Ubg);
                    if (Modul1.UbgT.Trim() != "")
                    {
                        Anz[0].SelectedText = " Religion: " + Modul1.UbgT;
                    }
                }
                if (Modul1.Kont[4].Trim() != "")
                {
                    Anz[0].SelectedText = " (" + Modul1.Kont[4] + ")";
                }
                if (Modul1.Kont[5].Trim() != "")
                {
                    Anz[0].SelectedText = ", Sippe " + Modul1.Kont[5];
                }
                DataModul.DT_AncesterTable.Index = "PerNr";
                DataModul.DT_AncesterTable.Seek("=", Modul1.PersInArb);
                if (!DataModul.DT_AncesterTable.NoMatch)
                {
                    Anz[0].SelectedText = ", siehe Nummer " + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim() + "\n";
                    continue;
                }
                if ((Modul1.DAus[115] == "1") | (Modul1.DAus[116] == "1"))
                {
                    Module2.Bildaus("P", "FaBu");
                }
                if (Modul1.DAus[88] == "1")
                {
                    Bild("P", Modul1.PersInArb);
                }
                Datschreib(Modul1.PersInArb, document);
                if ((Modul1.DAus[42] == "1") | (Modul1.DAus[43] == "1"))
                {
                    Sonst();
                }
                if ((Modul1.DAus[24] == "1") | (Modul1.DAus[25] == "1"))
                {
                    EEventArt Beruf = EEventArt.eA_300;
                    Berufe(ref Beruf);
                }
                if ((Modul1.DAus[28] == "1") | (Modul1.DAus[29] == "1"))
                {
                    EEventArt Beruf = EEventArt.eA_301;
                    Berufe(ref Beruf);
                }
                if ((Modul1.DAus[32] == "1") | (Modul1.DAus[33] == "1"))
                {
                    EEventArt Beruf = EEventArt.eA_302;
                    Berufe(ref Beruf);
                }
                Anz[0].SelectionCharOffset = 0;
                Retweg2(Anz[0]);
                Anz[0].SelectedText = "\n";
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
            Retweg2(Anz[0]);
            Anz[0].SelectedText = "\n";
        }
    }


    public void Retweg2(RichTextBox richTextBox)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        byte b = default;
        int lErl = default;

        num = 2;
        if (richTextBox.Text.Length == 0)
            richTextBox.SelectionCharOffset = 0;
        else
        {
            b = 1;
            while (b == 1)
            {
                leerweg(richTextBox);
                b = 0;
                if (richTextBox.Text.Length != 0)
                {
                    if (Strings.Mid(richTextBox.Text, richTextBox.SelectionStart, 1) == "\n")
                    {
                        richTextBox.SelectionStart = checked(richTextBox.SelectionStart - 1);
                        richTextBox.SelectionLength = 1;
                        richTextBox.SelectedText = "";
                        b = 1;
                    }
                    if (richTextBox.Text.Length != 0)
                    {
                        if (Strings.Mid(richTextBox.Text, richTextBox.SelectionStart, 1) == "\r")
                        {
                            richTextBox.SelectionStart = checked(richTextBox.SelectionStart - 1);
                            richTextBox.SelectionLength = 1;
                            richTextBox.SelectedText = "";
                            b = 1;
                        }
                    }
                }
            }
            richTextBox.SelectionCharOffset = 0;
        }
    }


    public void leerweg(RichTextBox richTextBox)
    {
        while (richTextBox.SelectionStart > 0
            && Strings.Mid(richTextBox.Text, richTextBox.SelectionStart, 1) == " ")
        {
            richTextBox.SelectionStart = checked(richTextBox.SelectionStart - 1);
            richTextBox.SelectionLength = 1;
            richTextBox.SelectedText = "";
        }
        richTextBox.SelectionColor = Color.Black;
    }

    public void Famwohn(EEventArt Art, byte Kin)
    {
        int try0000_dispatch = -1;
        int num = default;
        string text = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        short num5 = default;
        string Job = default;
        byte b = default;
        short num6 = default;
        string text2 = default;
        string value = default;
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
                        case 7063:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_1761;
                                    default:
                                        goto end_IL_0000;
                                }
                                lErl = 200;
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
                                goto IL_1765;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            num = 2;
                            Job = "";
                            array = new object[6];
                            b = 0;
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            if (Modul1.PamPam == 1)
                            {
                                Modul1.PamPam = 0;
                                goto end_IL_0000_2;
                            }
                            num6 = (short)(List3.Items.Count - 1);
                            num5 = 0;
                            goto IL_15bb;
                        IL_0267: // <========== 3
                            num = 28;
                            Modul1.sDatu = "";
                            if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != Modul1.FamInArb)))
                            {
                                DataModul.DB_EventTable.Index = "ArtNr";
                                goto end_IL_0000_2;
                            }
                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                LD = "";
                                Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim();
                            }
                            goto IL_0371;
                        IL_0371:
                            num = 37;
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
                            }
                            goto IL_045b;
                        IL_045b: // <========== 3
                            num = 44;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                            {
                                Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                Modul1.sDatu = "00000000" + Modul1.sDatu.Right(8);
                                text2 = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                if ((text2.Trim() == "") & (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumB].AsDate()) > 0.0))
                                {
                                    text2 = " von";
                                }
                                Modul1.sDatu = Modul1.Datwand1(Modul1.sDatu, text2);
                                Modul1.Kont1[1] = Modul1.sDatu;
                            }
                            goto IL_056a;
                        IL_056a:
                            num = 54;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                Modul1.sDatu = "00000000" + Modul1.sDatu.Right(8);
                                text2 = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                Modul1.sDatu = Modul1.Datwand1(Modul1.sDatu, text2);
                                if (Modul1.Kont1[1].Left(5) != "zwisc")
                                {
                                    if (Strings.InStr(Modul1.sDatu, "bis") == 0 && Modul1.sDatu.Trim() != "")
                                    {
                                        Modul1.sDatu = " bis " + Modul1.sDatu.Trim();
                                    }
                                    goto IL_068a;
                                }
                                goto IL_06c9;
                            }
                            goto IL_06d9;
                        IL_068a: // <========== 3
                            num = 67;
                            if (Modul1.Kont1[1].Trim() != "")
                            {
                                Modul1.Kont1[1] = " von " + Modul1.Kont1[1].Trim();
                            }
                            goto IL_06c9;
                        IL_06c9: // <========== 3
                            num = 71;
                            Modul1.Kont1[3] = Modul1.sDatu;
                            goto IL_06d9;
                        IL_06d9: // <========== 3
                            num = 73;
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
                            goto IL_07d7;
                        IL_07d7: // <========== 3
                            num = 83;
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                            {
                                Modul1.Kont1[5] = " " + Modul1.ortles1(DataModul.DB_EventTable.Fields[EventFields.Ort].AsInt(), 1, (i, s) => Modul1.ExportPlace(i, s, Modul1.Ind1, M_Namen));
                                Modul1.UbgT = "";
                            }
                            goto IL_086d;
                        IL_086d:
                            num = 88;
                            if (DataModul.DB_EventTable.Fields[EventFields.Platz].AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                LD = "";
                                Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                if (Modul1.Kont[0] != "")
                                {
                                    Modul1.Kont1[8] = " " + Modul1.Kont[0].Trim();
                                }
                            }
                            goto IL_0925;
                        IL_0925: // <========== 3
                            num = 94;
                            if (((Modul1.DAus[38] == "1") & !M1_Ki) | ((Modul1.DAus[42] == "1") & M1_Ki))
                            {
                                Job = Modul1.Kont1[7].TrimEnd();
                                goto IL_09eb;
                            }
                            if (((Modul1.DAus[39] == "1") & !M1_Ki) | ((Modul1.DAus[43] == "1") & M1_Ki))
                            {
                                Job = "";
                                Job = Module2.Jobdreh(Job);
                                Job += text;
                                text = "";
                            }
                            goto IL_09eb;
                        IL_09eb: // <========== 3
                            num = 103;
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
                                    }
                                }
                                goto IL_0aba;
                            }
                            goto IL_152b;
                        IL_0aba: // <========== 3
                            num = 113;
                            Retweg2(Anz[0]);
                            if (Kin > 0)
                            {
                                Anz[0].SelectedText = "\n\n";
                            }
                            goto IL_0ae6;
                        IL_0ae6:
                            num = 117;
                            if (b == 0)
                            {
                                if ((Modul1.DAus[106].AsDouble() == 1.0) & (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0))
                                {
                                    Anz[0].SelectedText = "\n";
                                }
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                                if (Art == EEventArt.eA_602)
                                {
                                    Anz[0].SelectedText = "Wohnort der Familie: ";
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    Anz[0].SelectedText = " ";
                                    b = 1;
                                }
                            }
                            goto IL_0bfd;
                        IL_0bfd: // <========== 3
                            num = 129;
                            if (Art == EEventArt.eA_603)
                            {
                                Retweg2(Anz[0]);
                                if (Modul1.DAus[106].AsDouble() == 1.0)
                                {
                                    Anz[0].SelectedText = "\n";
                                }
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                                if (Modul1.Kont1[10].Trim() != "")
                                {
                                    Anz[0].SelectedText = "\n" + Modul1.Kont1[10].Trim();
                                    goto IL_0d02;
                                }
                                Anz[0].SelectedText = "\nSonst. Daten der Familie:";
                                goto IL_0d02;
                            }
                            goto IL_0d52;
                        IL_0d02: // <========== 3
                            num = 141;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectedText = " ";
                            goto IL_0d52;
                        IL_0d52: // <========== 3
                            num = 144;
                            Retweg2(Anz[0]);
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectedText = " ";
                            if (Modul1.DAus[106].AsDouble() == 1.0)
                            {
                                if (List3.Items.Count > 1)
                                {
                                    if (Art == EEventArt.eA_602)
                                    {
                                        if (Kin > 0)
                                        {
                                            Anz[0].SelectedText = "\n";
                                        }
                                    }
                                    goto IL_0e51;
                                }
                                Anz[0].SelectedText = " ";
                            }
                            goto IL_0e51;
                        IL_0e51: // <========== 4
                            num = 161;
                            if (Modul1.DAus[106] == "0")
                            {
                                if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == ";")
                                {
                                    Anz[0].SelectedText = " ";
                                }
                            }
                            goto IL_0ec8;
                        IL_0ec8: // <========== 3
                            num = 166;
                            Anz[0].SelectedText = Job.Trim();
                            if ((List3.Items.Count > 1) & (Modul1.DAus[106].AsDouble() == 1.0))
                            {
                                if (Art != EEventArt.eA_602)
                                {
                                }
                            }
                            goto IL_0f34;
                        IL_0f34: // <========== 3
                            num = 173;
                            if (unchecked((Modul1.DAus[40] == "1" && Art == EEventArt.eA_603) | (Modul1.DAus[22] == "1" && Art == EEventArt.eA_602)))
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim();
                                    Bemaus();
                                }
                            }
                            goto IL_0ff0;
                        IL_0ff0: // <========== 3
                            num = 179;
                            if (unchecked((Modul1.DAus[41] == "1" && Art == EEventArt.eA_603) | (Modul1.DAus[23] == "1" && Art == EEventArt.eA_602)))
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim();
                                    Bemaus();
                                }
                            }
                            goto IL_10ac;
                        IL_10ac: // <========== 3
                            num = 185;
                            b2 = (byte)Modul1.DAus[96].AsInt();
                            famInArb = Modul1.FamInArb;
                            if (b2 == 1)
                            {
                                Modul1.Zeugsu(Art, Modul1.LfNR, 2, (long)Modul1.FamInArb);
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
                                        Retweg2(Anz[0]);
                                        if (Kin > 0)
                                        {
                                            Anz[0].SelectedText = "\n";
                                        }
                                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                                        Anz[0].SelectedText = "Angaben zur Familie: \n";
                                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                        Fb = true;
                                    }
                                    Anz[0].SelectedText = " ";
                                    if (Modul1.DAus[100] == "1")
                                    {
                                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);

                                    }
                                    else
                                    {
                                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                                    }
                                    goto IL_12f5;
                                }
                            }
                            goto IL_1426;
                        IL_12f5: // <========== 3
                            num = 212;
                            Anz[0].SelectedText = "Zeugen:";
                            if (Modul1.DAus[100] == "1")
                            {
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);

                            }
                            else
                            {
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            }
                            goto IL_13a9;
                        IL_13a9: // <========== 3
                            num = 219;
                            Anz[0].SelectedText = " ";
                            Anz[0].SelectedText = text3.Trim();
                            text3 = "";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            goto IL_1426;
                        IL_1426: // <========== 3
                            num = 225;
                            Nr = Modul1.FamInArb;
                            LfNR = Modul1.LfNR;
                            Modul1.QuellenDatum(ref Nr, Art, ref LfNR);
                            Modul1.LfNR = Conversions.ToByte(LfNR);
                            Modul1.FamInArb = Nr.AsInt();
                            if (Modul1.Kont1[9].Trim() != "")
                            {
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                                Anz[0].SelectedText = " " + Modul1.Kont1[9];
                                Anz[0].SelectionCharOffset = 0;
                            }
                            goto IL_152b;
                        IL_152b: // <========== 3
                            num = 233;
                            DataModul.DB_EventTable.MoveNext();
                            if (Modul1.DAus[106].AsDouble() == 0.0)
                            {
                                Anz[0].SelectedText = "; ";
                            }
                            Retweg2(Anz[0]);
                            if (Kin > 0)
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            goto IL_15ad;
                        IL_15ad:
                            num = 241;
                            num5 = (short)unchecked(num5 + 1);
                            goto IL_15bb;
                        IL_15bb:
                            if (num5 <= num6)
                            {
                                Modul1.LfNR = (byte)Math.Round(Conversion.Val(List3.Items[num5].AsString().Right(10)));
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", Art.AsString(), Modul1.FamInArb.AsString(), Modul1.LfNR);
                                if (Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 0, TextCompare: false))
                                {
                                    Interaction.MsgBox("Stop 14");
                                }
                                M1_J = 0;
                                while (unchecked(M1_J) <= 15u)
                                {
                                    Modul1.Kont1[M1_J] = "";
                                    M1_J = (byte)unchecked((uint)(M1_J + 1));
                                }
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value))
                                {
                                    if (DataModul.DB_EventTable.Fields[EventFields.ArtText].AsDouble() > 0.0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                                        LD = "";
                                        Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                        if (Modul1.Kont[0] != "")
                                        {
                                            Modul1.Kont1[10] = " " + Modul1.Kont[0].Trim() + ": ";
                                        }
                                    }
                                }
                                goto IL_0267;
                            }
                            if (Kin > 0)
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            Retweg2(Anz[0]);
                            if (Anz[0].Text.Length > 1)
                            {
                                if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == ";")
                                {
                                    Anz[0].SelectionStart = Anz[0].SelectionStart - 1;
                                    Anz[0].SelectionLength = 1;
                                    Anz[0].SelectedText = ".";
                                }
                            }
                            goto IL_16ba;
                        IL_16ba: // <========== 3
                            num = 253;
                            if (Kin <= 0)
                            {

                            }
                            else
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            goto end_IL_0000_2;
                        IL_1761:
                            num4 = unchecked(num2 + 1);
                            goto IL_1765;
                        IL_1765:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 25:
                                case 26:
                                case 27:
                                case 28:
                                    goto IL_0267;
                                case 36:
                                case 37:
                                    goto IL_0371;
                                case 42:
                                case 43:
                                case 44:
                                    goto IL_045b;
                                case 53:
                                case 54:
                                    goto IL_056a;
                                case 61:
                                case 65:
                                case 66:
                                case 67:
                                    goto IL_068a;
                                case 69:
                                case 70:
                                case 71:
                                    goto IL_06c9;
                                case 72:
                                case 73:
                                    goto IL_06d9;
                                case 80:
                                case 81:
                                case 82:
                                case 83:
                                    goto IL_07d7;
                                case 87:
                                case 88:
                                    goto IL_086d;
                                case 92:
                                case 93:
                                case 94:
                                    goto IL_0925;
                                case 96:
                                case 102:
                                case 103:
                                    goto IL_09eb;
                                case 110:
                                case 111:
                                case 112:
                                case 113:
                                    goto IL_0aba;
                                case 116:
                                case 117:
                                    goto IL_0ae6;
                                case 127:
                                case 128:
                                case 129:
                                    goto IL_0bfd;
                                case 137:
                                case 140:
                                case 141:
                                    goto IL_0d02;
                                case 143:
                                case 144:
                                    goto IL_0d52;
                                case 152:
                                case 153:
                                case 155:
                                case 156:
                                case 159:
                                case 160:
                                case 161:
                                    goto IL_0e51;
                                case 164:
                                case 166:
                                    goto IL_0ec8;
                                case 169:
                                case 171:
                                case 172:
                                case 173:
                                    goto IL_0f34;
                                case 177:
                                case 178:
                                case 179:
                                    goto IL_0ff0;
                                case 183:
                                case 184:
                                case 185:
                                    goto IL_10ac;
                                case 208:
                                case 211:
                                case 212:
                                    goto IL_12f5;
                                case 215:
                                case 218:
                                case 219:
                                    goto IL_13a9;
                                case 223:
                                case 224:
                                case 225:
                                    goto IL_1426;
                                case 231:
                                case 232:
                                case 233:
                                    goto IL_152b;
                                case 240:
                                case 241:
                                    goto IL_15ad;
                                case 251:
                                case 252:
                                case 253:
                                    goto IL_16ba;
                                case 8:
                                case 31:
                                case 255:
                                case 256:
                                case 259:
                                case 265:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 7063;
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

    public void Gatte()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        short einrück = default;
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
                    int frau;
                    string LD;
                    EEventArt Beruf;
                    int famInArb;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 2314:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_075c;
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
                                goto IL_0760;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            if (MasterRück > 0)
                            {
                                MasterRück += 10;
                                M_Indent = (byte)MasterRück;
                            }
                            Modul1.Famles();
                            if (Modul1.eLKennz == ELinkKennz.lkFather)
                            {
                                Modul1.PersInArb = Modul1.Family.Frau;
                                frau = Modul1.Family.Frau;

                            }
                            else
                            {
                                Modul1.PersInArb = Modul1.Family.Mann;
                                frau = Modul1.Family.Mann;
                            }
                            goto IL_0089;
                        IL_0089: // <========== 3
                            num = 15;
                            if (Modul1.PersInArb == 0)
                            {
                                Anz[0].SelectedText = "unbekanntem Partner";
                                goto end_IL_0000_2;
                            }
                            DataModul.DT_SperrTable.Index = "Nr";
                            DataModul.DT_SperrTable.Seek("=", Modul1.PersInArb);
                            if (!DataModul.DT_SperrTable.NoMatch)
                            {
                                Anz[0].SelectedText = Modul1.Datschuname + "\n";
                                goto end_IL_0000_2;
                            }
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            Modul1.Namenindex(Ahne);
                            M_Namen = Modul1.Kont[0];
                            Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person, Modul1.DAus[89] == "1"));
                            if (Modul1.DAus[76] == "1")
                            {
                                Anz[0].SelectedText = " <" + Modul1.PersInArb.AsString().Trim() + "> ";
                            }
                            if (Modul1.Kont[3].Trim() != "")
                            {
                                Anz[0].SelectedText = Strings.Trim(Modul1.Person.Prae.TrimEnd() + " " + Modul1.Kont[3].Trim()) + " ";
                            }
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            Anz[0].SelectedText = Modul1.Kont[0].Trim();
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Modul1.PerSatzLes(Modul1.PersInArb);
                            if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields["religi"].Value))
                            {
                                if (Operators.CompareString(DataModul.DB_PersonTable.Fields["religi"].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    Modul1.Ubg = DataModul.DB_PersonTable.Fields["religi"].AsInt();
                                    LD = "";
                                    Modul1.UbgT = DataModul.TextLese1(Modul1.Ubg);
                                    if (Modul1.UbgT.Trim() != "")
                                    {
                                        Anz[0].SelectedText = " Religion: " + Modul1.UbgT;
                                    }
                                }
                            }
                            goto IL_03b1;
                        IL_03b1: // <========== 3
                            num = 48;
                            if ((Modul1.DAus[115] == "1") | (Modul1.DAus[116] == "1"))
                            {
                                Module2.Bildaus("P", "FaBu");
                            }
                            if (Modul1.DAus[88] == "1")
                            {
                                Bild("P", Modul1.PersInArb);
                            }
                            Retweg2(Anz[0]);
                            Anz[0].SelectedText = "\n";
                            if (Modul1.DAus[106].AsDouble() == 1.0)
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            Datschreib(Modul1.PersInArb, document);
                            Retweg2(Anz[0]);
                            if ((Modul1.DAus[38] == "1") | (Modul1.DAus[39] == "1"))
                            {
                                Sonst();
                            }
                            if ((Modul1.DAus[0] == "1") | (Modul1.DAus[13] == "1"))
                            {
                                Beruf = EEventArt.eA_300;
                                Berufe(ref Beruf);
                            }
                            if ((Modul1.DAus[16] == "1") | (Modul1.DAus[17] == "1"))
                            {
                                Beruf = EEventArt.eA_301;
                                Berufe(ref Beruf);
                            }
                            if ((Modul1.DAus[20] == "1") | (Modul1.DAus[21] == "1"))
                            {
                                Beruf = EEventArt.eA_302;
                                Berufe(ref Beruf);
                            }
                            Retweg2(Anz[0]);
                            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            Retweg2(Anz[0]);
                            Retweg2(Anz[0]);
                            famInArb = Modul1.FamInArb;
                            Modul1.eLKennz = ELinkKennz.lkChild;
                            aiFams = DataModul.Link.GetPersonFams(Modul1.PersInArb, Modul1.eLKennz);
                            if (Modul1.DAus[106].AsDouble() == 1.0)
                            {
                                Retweg2(Anz[0]);
                                Anz[0].SelectedText = "\n";
                            }
                            if (Modul1.UbgT != "")
                            {
                                einrück = M_Indent;
                                Modul1.FamInArb = (int)Math.Round(Modul1.UbgT.AsDouble());
                                Eltles();
                                M_Indent = einrück;
                            }
                            Retweg2(Anz[0]);
                            Anz[0].SelectedText = "\n";
                            goto end_IL_0000_2;
                        IL_075c:
                            num4 = unchecked(num2 + 1);
                            goto IL_0760;
                        IL_0760:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 10:
                                case 14:
                                case 15:
                                    goto IL_0089;
                                case 45:
                                case 46:
                                case 47:
                                case 48:
                                    goto IL_03b1;
                                case 99:
                                case 100:
                                    num = 100;
                                    M_Indent -= 10;
                                    if (MasterRück <= 0)
                                    {

                                    }
                                    else
                                    {
                                        MasterRück -= 10;
                                    }
                                    goto end_IL_0000_2;
                                case 17:
                                case 23:
                                case 94:
                                case 103:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 2314;
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
    public void PerQu1()
    {
        int FamPer = 1;
        Modul1.PerQu(ref FamPer);
        if (Modul1.Kont[30].Trim() != "")
        {
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
            Anz[0].SelectedText = Modul1.Kont[30].Trim();
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            Anz[0].SelectionCharOffset = 0;
        }
    }
    public void Berufe(ref EEventArt Beruf)
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
        string text = default;
        string left2 = default;
        string value = default;
        short num7 = default;
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
                        case 8795:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_1d6d;
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
                                goto IL_1d71;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            List3.Items.Clear();
                            DataModul.DB_EventTable.Index = "Besu";
                            DataModul.DB_EventTable.Seek("=", Beruf.AsString(), Modul1.PersInArb.AsString());
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                DataModul.DB_EventTable.Index = "ArtNr";
                                goto end_IL_0000_2;
                            }
                            goto IL_052b;
                        IL_037a: // <========== 3
                            num = 39;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value))
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.ArtText].AsDouble() > 0.0)
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                                    LD = "";
                                    Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                    if (Modul1.Kont[0] != "")
                                    {
                                        Modul1.Kont[10] = " " + Modul1.Kont[0].Trim() + ": ";
                                    }
                                }
                            }
                            goto IL_0464;
                        IL_0464: // <========== 3
                            num = 47;
                            Job = Modul1.Kont1[1] + Modul1.Kont1[3] + Modul1.Kont1[7] + new string(' ', 240).Left(240) + DataModul.DB_EventTable.Fields[EventFields.LfNr].AsString();
                            if (DataModul.DB_EventTable.Fields[EventFields.Reg].AsString() != " ")
                            {
                                Job = "+" + Job;
                            }
                            goto IL_04fe;
                        IL_04fe:
                            num = 51;
                            List3.Items.Add(Job);
                            goto IL_0515;
                        IL_0515: // <========== 3
                            num = 52;
                            lErl = 12;
                            DataModul.DB_EventTable.MoveNext();
                            goto IL_052b;
                        IL_052b: // <========== 3
                            num = 11;
                            if (!DataModul.DB_EventTable.EOF)
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != Beruf)
                                {
                                    DataModul.DB_EventTable.Index = "ArtNr";
                                    goto end_IL_0000_2;
                                }
                                if (!Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                                {
                                    M1_J = 0;
                                    while (unchecked(M1_J) <= 15u)
                                    {
                                        Modul1.Kont1[M1_J] = "";
                                        M1_J = (byte)unchecked((uint)(M1_J + 1));
                                    }
                                    Modul1.sDatu = "";
                                    if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != Modul1.PersInArb) | (DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != Beruf)))
                                    {
                                        DataModul.DB_EventTable.Index = "ArtNr";
                                        goto IL_053e;
                                    }
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                    {
                                        Modul1.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                                        Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                        Modul1.Kont1[1] = Modul1.sDatu;
                                    }
                                    Modul1.UbgT = "";
                                    if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsDouble() > 0.0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                        LD = "";
                                        Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                        if (Modul1.Kont[0] != "")
                                        {
                                            Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim() + " ";
                                        }
                                    }
                                    goto IL_037a;
                                }
                                goto IL_0515;
                            }
                            goto IL_053e;
                        IL_053e: // <========== 3
                            num = 55;
                            lErl = 13;
                            leerweg(Anz[0]);
                            if (List3.Items.Count == 0)
                            {
                                goto end_IL_0000_2;
                            }
                            if (Modul1.DAus[106].AsDouble() == 1.0)
                            {
                                if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                                {
                                    Anz[0].SelectedText = "\n";
                                }
                            }
                            goto IL_05d0;
                        IL_05d0: // <========== 3
                            num = 65;
                            Anz[0].SelectionCharOffset = 0;
                            if (Modul1.DAus[106].AsDouble() == 0.0)
                            {
                                Anz[0].SelectedText = " ";
                            }
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * Fackt), FontStyle.Underline);
                            switch (Beruf)
                            {
                                case EEventArt.eA_300:
                                    goto IL_0677;
                                case EEventArt.eA_301:
                                    goto IL_06db;
                                case EEventArt.eA_302:
                                    goto IL_0709;
                                default:
                                    break;
                            }
                            goto IL_0734;
                        IL_0677:
                            num = 74;
                            if (List3.Items.Count == 1)
                            {
                                Anz[0].SelectedText = "Beruf:";
                            }
                            goto IL_06a8;
                        IL_06a8:
                            num = 77;
                            if (List3.Items.Count > 1)
                            {
                                Anz[0].SelectedText = "Berufe:";
                            }
                            goto IL_0734;
                        IL_06db:
                            num = 82;
                            Anz[0].SelectedText = Modul1.IText[70].Trim() + " ";
                            goto IL_0734;
                        IL_0709:
                            num = 85;
                            Anz[0].SelectedText = Modul1.IText[8].Trim() + " ";
                            goto IL_0734;
                        IL_0734: // <========== 5
                            num = 87;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * Fackt), FontStyle.Regular);
                            Anz[0].SelectedText = " ";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * Fackt), FontStyle.Regular);
                            num5 = (short)(List3.Items.Count - 1);
                            num6 = 0;
                            goto IL_1c58;
                        IL_0aa1: // <========== 3
                            num = 117;
                            text = "";
                            if (left == "1")
                            {
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    Modul1.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                                    text = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                    Modul1.sDatu = Modul1.Datwand1(Modul1.sDatu, text);
                                    Modul1.Kont1[1] = Modul1.sDatu;
                                }
                                goto IL_0b71;
                            }
                            goto IL_0fe8;
                        IL_0b71:
                            num = 125;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                if ((text == "") & (Modul1.Kont1[1].Trim() != ""))
                                {
                                    Modul1.Kont1[1] = "von " + Modul1.Kont1[1];
                                }
                                Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                Modul1.sDatu = "00000000" + Modul1.sDatu.Right(8);
                                text = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                left2 = text;
                                Modul1.sDatu = Modul1.Datwand1(Modul1.sDatu, text);
                                if (left2 == "")
                                {
                                    if (Modul1.sDatu.Trim() != "")
                                    {
                                        Modul1.sDatu = " bis " + Modul1.sDatu.Trim();
                                    }
                                }
                                goto IL_0cd1;
                            }
                            goto IL_0d72;
                        IL_0cd1: // <========== 3
                            num = 139;
                            if (left2 == "z")
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString().Trim(), "", TextCompare: false) == 0)
                                {
                                    if (Modul1.sDatu.Trim() != "")
                                    {
                                        Modul1.sDatu = " und " + Modul1.sDatu.Trim();
                                    }
                                }
                            }
                            goto IL_0d5f;
                        IL_0d5f: // <========== 3
                            num = 146;
                            Modul1.Kont1[3] = Modul1.sDatu;
                            goto IL_0d72;
                        IL_0d72: // <========== 3
                            num = 148;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.DatumText].Value))
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt() > 0)
                                {
                                    value = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsString();
                                    AAA = value.AsInt();
                                    Modul1.UbgT = DataModul.TextLese1(AAA);
                                    value = AAA.AsString();
                                    if (Modul1.UbgT.Trim() != "")
                                    {
                                        Modul1.Kont1[3] = Modul1.Kont1[3] + " (" + Modul1.UbgT.Trim() + ")";
                                        Modul1.UbgT = "";
                                    }
                                }
                            }
                            goto IL_0e82;
                        IL_0e82: // <========== 3
                            num = 158;
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                            {
                                ortNr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble());
                                Modul1.UbgT = Modul1.ortles1(ortNr, 1, (i, s) => Modul1.ExportPlace(i, s, Modul1.Ind1, M_Namen));
                                Modul1.Kont1[5] = " " + Modul1.UbgT.Trim();
                                Modul1.UbgT = "";
                            }
                            goto IL_0f24;
                        IL_0f24:
                            num = 163;
                            if (DataModul.DB_EventTable.Fields[EventFields.Platz].AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                if (Modul1.Kont[0] != "")
                                {
                                    Modul1.Kont1[8] = " " + Modul1.Kont[0].Trim();
                                }
                            }
                            goto IL_0fe8;
                        IL_0fe8: // <========== 4
                            num = 170;
                            Modul1.UbgT = "";
                            if (Beruf == EEventArt.eA_302)
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                                {
                                    ortNr2 = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble());
                                    Modul1.UbgT = Modul1.ortles1(ortNr2, 1, (i, s) => Modul1.ExportPlace(i, s, Modul1.Ind1, M_Namen));
                                    Modul1.Kont1[5] = " " + Modul1.UbgT.Trim();
                                    Modul1.UbgT = "";
                                }
                                goto IL_10ae;
                            }
                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                LD = "";
                                Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                if (Modul1.Kont[0] != "")
                                {
                                    Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim() + " ";
                                }
                            }
                            goto IL_136c;
                        IL_10ae:
                            num = 177;
                            if (left == "1")
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsDouble() > 0.0)
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                    LD = "";
                                    Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                    if (Modul1.Kont[0] != "")
                                    {
                                        Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim();
                                        Modul1.Kont[0] = "";
                                    }
                                }
                            }
                            goto IL_119e;
                        IL_119e: // <========== 3
                            num = 186;
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
                            }
                            goto IL_136c;
                        IL_136c: // <========== 5
                            num = 202;
                            left = "0";
                            Job = "";
                            Job = Module2.Jobdreh(Job);
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * Fackt), FontStyle.Regular);
                            if (Job.Trim() != "")
                            {
                                Retweg2(Anz[0]);
                                if (Modul1.DAus[106].AsDouble() == 1.0)
                                {
                                    if (List3.Items.Count > 1)
                                    {
                                        Anz[0].SelectedText = "\n";
                                        goto IL_14a6;
                                    }
                                    Anz[0].SelectedText = "\n";
                                    Retweg2(Anz[0]);
                                    Anz[0].SelectedText = " ";
                                }
                                goto IL_14a6;
                            }
                            goto IL_153d;
                        IL_14a6: // <========== 3
                            num = 218;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * Fackt), FontStyle.Regular);
                            if (Modul1.DAus[106].AsDouble() == 0.0)
                            {
                                Anz[0].SelectedText = " ";
                            }
                            goto IL_151e;
                        IL_151e:
                            num = 222;
                            Anz[0].SelectedText = Job.TrimEnd();
                            goto IL_153d;
                        IL_153d: // <========== 3
                            num = 224;
                            if (Modul1.DAus[62] == "1")
                            {
                                Nr = Modul1.PersInArb;
                                LfNR = Modul1.LfNR;
                                Modul1.QuellenDatum(ref Nr, Beruf, ref LfNR);
                                Modul1.LfNR = Conversions.ToByte(LfNR);
                                Modul1.PersInArb = Nr.AsInt();
                            }
                            leerweg(Anz[0]);
                            if (Modul1.Kont1[9].Trim() != "")
                            {
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                Anz[0].SelectedText = " " + Modul1.Kont1[9];
                            }
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * Fackt), FontStyle.Regular);
                            Anz[0].SelectionCharOffset = 0;
                            left = "0";
                            if (M1_Ki)
                            {
                                if (((Beruf == EEventArt.eA_300) & (Modul1.DAus[26] == "1"))
                                        | ((Beruf == EEventArt.eA_301) & (Modul1.DAus[30] == "1"))
                                        | ((Beruf == EEventArt.eA_302) & (Modul1.DAus[34] == "1")))
                                {
                                    left = "1";
                                }
                                goto IL_17f9;
                            }
                            if (((Beruf == EEventArt.eA_300) & (Modul1.DAus[14] == "1"))
                                        | ((Beruf == EEventArt.eA_301) & (Modul1.DAus[18] == "1"))
                                        | ((Beruf == EEventArt.eA_302) & (Modul1.DAus[22] == "1")))
                            {
                                left = "1";
                            }
                            goto IL_17f9;
                        IL_17f9: // <========== 3
                            num = 247;
                            if (left == "1")
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim();
                                    Bemaus();
                                }
                            }
                            goto IL_1883;
                        IL_1883: // <========== 3
                            num = 253;
                            left = "0";
                            if (M1_Ki)
                            {
                                if (((Beruf == EEventArt.eA_300) & (Modul1.DAus[27] == "1"))
                                        | ((Beruf == EEventArt.eA_301) & (Modul1.DAus[31] == "1"))
                                        | ((Beruf == EEventArt.eA_302) & (Modul1.DAus[35] == "1")))
                                {
                                    left = "1";
                                }
                                goto IL_1997;
                            }
                            if (((Beruf == EEventArt.eA_300) & (Modul1.DAus[15] == "1"))
                                        | ((Beruf == EEventArt.eA_301) & (Modul1.DAus[19] == "1"))
                                        | ((Beruf == EEventArt.eA_302) & (Modul1.DAus[23] == "1")))
                            {
                                left = "1";
                            }
                            goto IL_1997;
                        IL_1997: // <========== 3
                            num = 264;
                            if (left == "1")
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim();
                                    Bemaus();
                                }
                            }
                            goto IL_1a21;
                        IL_1a21: // <========== 3
                            num = 270;
                            if (!M1_Ki)
                            {
                                num7 = (short)Modul1.DAus[96].AsInt();
                            }
                            if (M1_Ki)
                            {
                                num7 = (short)Modul1.DAus[98].AsInt();
                            }
                            persInArb = Modul1.PersInArb;
                            if (num7 == 1)
                            {
                                Modul1.Zeugsu(Beruf, Modul1.LfNR, 2, Ahne);
                            }
                            text2 = Modul1.Kont1[20];
                            Modul1.Kont1[20] = "";
                            Modul1.PersInArb = persInArb;
                            if (num7 == 1)
                            {
                                if (text2 != "")
                                {
                                    leerweg(Anz[0]);
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * Fackt), FontStyle.Regular);
                                    if (Modul1.DAus[100] == "1")
                                    {
                                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                    }
                                    Anz[0].SelectedText = " Zeugen: " + text2.Trim();
                                    text2 = "";
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * Fackt), FontStyle.Regular);
                                }
                            }
                            goto IL_1c0f;
                        IL_1c0f: // <========== 3
                            num = 295;
                            left = "0";
                            Anz[0].SelectedText = ";";
                            DataModul.DB_EventTable.MoveNext();
                            num6 = (short)unchecked(num6 + 1);
                            goto IL_1c58;
                        IL_1c58:
                            if (num6 <= num5)
                            {
                                Modul1.LfNR = (byte)Math.Round(Conversion.Val(Strings.Mid(List3.Items[num6].AsString(), 240, 10)));
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", Beruf.AsString(), Modul1.PersInArb.AsString(), Modul1.LfNR);
                                if (Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                                {
                                    Interaction.MsgBox("7");
                                    Debugger.Break();
                                }
                                M1_J = 0;
                                while (unchecked(M1_J) <= 15u)
                                {
                                    Modul1.Kont1[M1_J] = "";
                                    M1_J = (byte)unchecked((uint)(M1_J + 1));
                                }
                                Modul1.Ubg = num6;
                                Modul1.sDatu = "";
                                if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != Modul1.PersInArb) | (DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != Beruf)))
                                {
                                    DataModul.DB_EventTable.Index = "ArtNr";
                                    goto end_IL_0000_2;
                                }
                                if (M1_Ki)
                                {
                                    if (((Beruf == EEventArt.eA_300) & (Modul1.DAus[25] == "1"))
                                        | ((Beruf == EEventArt.eA_301) & (Modul1.DAus[29] == "1"))
                                        | ((Beruf == EEventArt.eA_302) & (Modul1.DAus[33] == "1")))
                                    {
                                        left = "1";
                                    }
                                    goto IL_0aa1;
                                }
                                if (((Beruf == EEventArt.eA_300) & (Modul1.DAus[13] == "1"))
                                        | ((Beruf == EEventArt.eA_301) & (Modul1.DAus[17] == "1"))
                                        | ((Beruf == EEventArt.eA_302) & (Modul1.DAus[21] == "1")))
                                {
                                    left = "1";
                                }
                                goto IL_0aa1;
                            }
                            if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == ";")
                            {
                                Anz[0].SelectionStart = Anz[0].SelectionStart - 1;
                                Anz[0].SelectionLength = 1;
                                Anz[0].SelectedText = ".";
                            }
                            Retweg2(Anz[0]);
                            left = "0";
                            goto end_IL_0000_2;
                        IL_1d6d:
                            num4 = unchecked(num2 + 1);
                            goto IL_1d71;
                        IL_1d71:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 37:
                                case 38:
                                case 39:
                                    goto IL_037a;
                                case 44:
                                case 45:
                                case 46:
                                case 47:
                                    goto IL_0464;
                                case 50:
                                case 51:
                                    goto IL_04fe;
                                case 17:
                                case 52:
                                    goto IL_0515;
                                case 9:
                                case 10:
                                case 11:
                                case 54:
                                    goto IL_052b;
                                case 25:
                                case 55:
                                    goto IL_053e;
                                case 63:
                                case 64:
                                case 65:
                                    goto IL_05d0;
                                case 76:
                                case 77:
                                    goto IL_06a8;
                                case 71:
                                case 79:
                                case 80:
                                case 83:
                                case 86:
                                case 87:
                                    goto IL_0734;
                                case 110:
                                case 111:
                                case 115:
                                case 116:
                                case 117:
                                    goto IL_0aa1;
                                case 124:
                                case 125:
                                    goto IL_0b71;
                                case 137:
                                case 138:
                                case 139:
                                    goto IL_0cd1;
                                case 143:
                                case 144:
                                case 145:
                                case 146:
                                    goto IL_0d5f;
                                case 147:
                                case 148:
                                    goto IL_0d72;
                                case 155:
                                case 156:
                                case 157:
                                case 158:
                                    goto IL_0e82;
                                case 162:
                                case 163:
                                    goto IL_0f24;
                                case 167:
                                case 168:
                                case 169:
                                case 170:
                                    goto IL_0fe8;
                                case 176:
                                case 177:
                                    goto IL_10ae;
                                case 183:
                                case 184:
                                case 185:
                                case 186:
                                    goto IL_119e;
                                case 191:
                                case 192:
                                case 193:
                                case 199:
                                case 200:
                                case 201:
                                case 202:
                                    goto IL_136c;
                                case 211:
                                case 216:
                                case 217:
                                case 218:
                                    goto IL_14a6;
                                case 221:
                                case 222:
                                    goto IL_151e;
                                case 223:
                                case 224:
                                    goto IL_153d;
                                case 240:
                                case 241:
                                case 245:
                                case 246:
                                case 247:
                                    goto IL_17f9;
                                case 251:
                                case 252:
                                case 253:
                                    goto IL_1883;
                                case 257:
                                case 258:
                                case 262:
                                case 263:
                                case 264:
                                    goto IL_1997;
                                case 268:
                                case 269:
                                case 270:
                                    goto IL_1a21;
                                case 293:
                                case 294:
                                case 295:
                                    goto IL_1c0f;
                                case 8:
                                case 14:
                                case 58:
                                case 105:
                                case 306:
                                case 311:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 8795;
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
    public void Eltles()
    {
        int try0000_dispatch = -1;
        int num = default;
        string text2 = default;
        int num2 = default;
        int num3 = default;
        string text = default;
        int lErl = default;
        int famInArb = default;
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
                    EEventArt Art;
                    string Ahne;
                    short Listart;
                    bool neb;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            text2 = "";
                            goto IL_0009;
                        case 4593:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0f0b;
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
                                    goto IL_0f0b;
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
                                goto IL_0f0f;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                            Modul1.PerSatzLes(Modul1.PersInArb);
                            Modul1.eLKennz = ELinkKennz.lkMother;
                            if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                            {
                                Modul1.eLKennz = ELinkKennz.lkFather;
                            }
                            text = Modul1.DAus[106];
                            Modul1.DAus[106] = "0";
                            M_Indent++;
                            if (Modul1.eLKennz.AsDouble() == 2.0)
                            {
                                Anz[0].SelectedText = " (Sohn von ";
                            }
                            if (Modul1.eLKennz.AsDouble() == 1.0)
                            {
                                Anz[0].SelectedText = " (Tochter von ";
                            }
                            DataModul.NB_FamilyTable.AddNew();
                            DataModul.NB_FamilyTable.Fields[IndexFields.Fam].Value = Modul1.FamInArb;
                            DataModul.NB_FamilyTable.Update();
                            if (DataModul.Link.GetFamPerson(Modul1.FamInArb, ELinkKennz.lkFather, out Modul1.PersInArb))
                            {
                                Modul1.Family.Mann = Modul1.PersInArb;
                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                Modul1.Namenindex(this.Ahne);
                                M_Namen = Modul1.Kont[0];
                                Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person, Modul1.DAus[89] == "1"));
                                text2 = Modul1.Kont[99];
                                Modul1.Kont[0] = Modul1.Kont[0].TrimEnd();
                                Anz[0].SelectedText = Modul1.Kont[3] + " ";
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Bold);
                                Anz[0].SelectedText = Modul1.Kont[0];
                                leerweg(Anz[0]);
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                Anz[0].SelectedText = " ";
                                if (Modul1.Kont[4] != "")
                                {
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Italic);
                                    Anz[0].SelectedText = "(" + Modul1.Kont[4].TrimEnd() + ")";
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                    Anz[0].SelectedText = " ";
                                }
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                leerweg(Anz[0]);
                                if (Modul1.DAus[78] == "1")
                                {
                                    Listart = 2;
                                    Ahne = 0.AsString();
                                    Art = default;
                                    neb = false;
                                    Modul1.Datles3(Listart, 0, Art, ref neb);
                                    M_Indent = 50;
                                    Fackt = 0.8f;
                                    Datschreib(Modul1.PersInArb, document);
                                    leerweg(Anz[0]);
                                    if ((Modul1.DAus[0] == "1") | (Modul1.DAus[13] == "1"))
                                    {
                                        Art = EEventArt.eA_300;
                                        Berufe(ref Art);
                                    }
                                    goto IL_0528;
                                }
                                goto IL_056e;
                            }
                            Anz[0].SelectedText = "unbekanntem Vater ";
                            goto IL_063d;
                        IL_0528:
                            num = 55;
                            if ((Modul1.DAus[16] == "1") | (Modul1.DAus[17] == "1"))
                            {
                                Art = EEventArt.eA_301;
                                Berufe(ref Art);
                            }
                            goto IL_056e;
                        IL_056e: // <========== 3
                            num = 59;
                            Modul1.Aschalt = unchecked(Modul1.Datschalt);
                            Modul1.Datschalt = 1;
                            Art = default;
                            Ahne = 0.AsString();
                            Listart = 2;
                            neb = false;
                            Modul1.Datles3(Listart, 0, Art, ref neb);
                            if (Modul1.Kont[1] == "")
                            {
                                Modul1.Kont[1] = Modul1.Kont[2];
                            }
                            if (Modul1.Kont[1] == "")
                            {
                                Modul1.Kont[1] = "    ";
                            }
                            Modul1.Datschalt = (byte)Math.Round(Modul1.Aschalt);
                            obj = 0;
                            goto IL_063d;
                        IL_063d: // <========== 3
                            num = 74;
                            lErl = 33;
                            DataModul.NB_FamilyTable.AddNew();
                            DataModul.NB_FamilyTable.Fields[IndexFields.Fam].Value = Modul1.FamInArb;
                            DataModul.NB_FamilyTable.Update();
                            if (DataModul.Link.GetFamPerson(Modul1.FamInArb, ELinkKennz.lkMother, out Modul1.PersInArb))
                            {
                                Modul1.Family.Frau = Modul1.PersInArb;
                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                M_Namen = Modul1.Kont[0];
                                Modul1.Namenindex(this.Ahne);
                                Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person, Modul1.DAus[89] == "1"));
                                Modul1.Kont[0] = Modul1.Kont[0].TrimEnd();
                                leerweg(Anz[0]);
                                Anz[0].SelectedText = " ";
                                if ((Modul1.Kont[0].Trim() == "") & (Modul1.Kont[3].Trim() == ""))
                                {
                                    Anz[0].SelectedText = "und unbekannter Mutter";
                                    if (Modul1.DAus[78] == "1")
                                    {
                                        Art = default;
                                        Ahne = 0.AsString();
                                        Listart = 2;
                                        neb = false;
                                        Modul1.Datles3(Listart, default, Art, ref neb);
                                        Datschreib(Modul1.PersInArb, document);
                                        leerweg(Anz[0]);
                                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                        if ((Modul1.DAus[0] == "1") | (Modul1.DAus[13] == "1"))
                                        {
                                            Art = EEventArt.eA_300;
                                            Berufe(ref Art);
                                        }
                                        goto IL_08c2;
                                    }
                                    goto IL_0e03;
                                }
                                if (Modul1.DAus[78] == "1")
                                {
                                    Heidat(1, 0);
                                }
                                leerweg(Anz[0]);
                                Modul1.PersInArb = Modul1.Family.Frau;
                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person, Modul1.DAus[89] == "1"));
                                Retweg2(Anz[0]);
                                if (Modul1.DAus[78] == "1")
                                {
                                    if (Modul1.DAus[106].AsDouble() == 1.0)
                                    {
                                        Anz[0].SelectedText = "\n";

                                    }
                                }
                                goto IL_09bb;
                            }
                            leerweg(Anz[0]);
                            Anz[0].SelectedText = " ";
                            Anz[0].SelectedText = "und unbekannter Mutter";
                            Modul1.FamInArb = famInArb;
                            Modul1.PersInArb = Persp1;
                            Punktweg();
                            Anz[0].SelectedText = ".)";
                            Modul1.DAus[106] = text;
                            goto end_IL_0000_2;
                        IL_08c2:
                            num = 99;
                            if ((Modul1.DAus[16] == "1") | (Modul1.DAus[17] == "1"))
                            {
                                Art = EEventArt.eA_301;
                                Berufe(ref Art);
                            }
                            goto IL_0e03;
                        IL_09bb: // <========== 3
                            num = 118;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                            if (Modul1.DAus[78] == "1")
                            {
                                Anz[0].SelectedText = " mit ";
                            }
                            else
                            {
                                Anz[0].SelectedText = " und ";
                            }
                            goto IL_0a4a;
                        IL_0a4a: // <========== 3
                            num = 125;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                            Anz[0].SelectedText = Strings.Trim(Modul1.Person.Prae.TrimEnd() + " " + (Modul1.Person.Prae.TrimEnd() + " " + Modul1.Kont[3]).Trim());
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Bold);
                            Anz[0].SelectedText = " " + Modul1.Kont[0] + " ";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                            if (Modul1.DAus[78] == "1")
                            {
                                Art = default;
                                Ahne = 0.AsString();
                                Listart = 2;
                                neb = false;
                                Modul1.Datles3(Listart, default, Art, ref neb);
                                Datschreib(Modul1.PersInArb, document);
                                leerweg(Anz[0]);
                                if ((Modul1.DAus[0] == "1") | (Modul1.DAus[13] == "1"))
                                {
                                    Art = EEventArt.eA_300;
                                    Berufe(ref Art);
                                }
                                goto IL_0c28;
                            }
                            goto IL_0c74;
                        IL_0c28:
                            num = 137;
                            if ((Modul1.DAus[16] == "1") | (Modul1.DAus[17] == "1"))
                            {
                                Art = EEventArt.eA_301;
                                Berufe(ref Art);
                            }
                            goto IL_0c74;
                        IL_0c74: // <========== 3
                            num = 141;
                            Modul1.Aschalt = unchecked(Modul1.Datschalt);
                            Modul1.Datschalt = 1;
                            Art = default;
                            Ahne = 0.AsString();
                            Listart = 2;
                            neb = false;
                            Modul1.Datles3(Listart, default, Art, ref neb);
                            if (Modul1.Kont[1] == "")
                            {
                                Modul1.Kont[1] = Modul1.Kont[2];
                            }
                            goto IL_0ce9;
                        IL_0ce9:
                            num = 147;
                            if (Modul1.Kont[1] == "")
                            {
                                Modul1.Kont[1] = "    ";
                            }
                            goto IL_0d18;
                        IL_0d18:
                            num = 150;
                            if (text2.Length > 23)
                            {
                                text2 = text2.Left(23);
                            }
                            goto IL_0d3a;
                        IL_0d3a:
                            num = 153;
                            Modul1.Datschalt = (byte)Math.Round(Modul1.Aschalt);
                            goto IL_0e03;
                        IL_0e03: // <========== 4
                            num = 167;
                            Punktweg();
                            Anz[0].SelectedText = ".)";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Modul1.DAus[106] = text;
                            Fackt = 1f;
                            goto end_IL_0000_2;
                        IL_0f0b:
                            num4 = unchecked(num2 + 1);
                            goto IL_0f0f;
                        IL_0f0f:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 54:
                                case 55:
                                    goto IL_0528;
                                case 57:
                                case 58:
                                case 59:
                                    goto IL_056e;
                                case 70:
                                case 73:
                                case 74:
                                    goto IL_063d;
                                case 98:
                                case 99:
                                    goto IL_08c2;
                                case 116:
                                case 118:
                                    goto IL_09bb;
                                case 121:
                                case 124:
                                case 125:
                                    goto IL_0a4a;
                                case 136:
                                case 137:
                                    goto IL_0c28;
                                case 139:
                                case 140:
                                case 141:
                                    goto IL_0c74;
                                case 146:
                                case 147:
                                    goto IL_0ce9;
                                case 149:
                                case 150:
                                    goto IL_0d18;
                                case 152:
                                case 153:
                                    goto IL_0d3a;
                                case 101:
                                case 102:
                                case 103:
                                case 154:
                                case 155:
                                case 166:
                                case 167:
                                    goto IL_0e03;
                                case 165:
                                case 172:
                                case 181:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj2) when (obj2 is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj2, lErl);
                try0000_dispatch = 4593;
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

    public void Datschreib(int persInArb, IDocument doc)
    {
        var Persp1 = persInArb;
        Modul1.Person_ReadNames(persInArb, Modul1.Person);
        M_Namen = Modul1.Kont[0];
        short Listart = 2;
        var ahne = Ahne;
        EEventArt Art = EEventArt.eA_Birth;
        bool neb = false;
        Modul1.Datles3(Listart, ahne, Art, ref neb);
        if (Modul1.DAus[106].AsInt() == 0)
        {
            if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
            {
                if (M_Indent < 20)
                {
                    doc.AppendText("\n");
                }
                else
                {
                    doc.AppendText(" ");
                }
            }
        }
        else if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
        {
            Anz[0].SelectedText = "\n";
        }
        if ((Modul1.Kont[11].Trim() != "") | (Modul1.Kont[16].Trim() != "") | (Modul1.Kont[21].Trim() != ""))
        {
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * Fackt), FontStyle.Regular);
            Anz[0].SelectedText = Modul1.DTxt[1];
            if (Modul1.Kont[11].Trim() != "")
            {
                Anz[0].SelectedText = " " + Modul1.Kont[11].Trim() + ".";
            }
            if (Modul1.DAus[62] == "1" && Modul1.Kont[31].Trim() != "")
            {
                Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                Anz[0].SelectedText = " " + Modul1.Kont[31].Trim();
                Anz[0].SelectionCharOffset = 0;
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * Fackt), FontStyle.Regular);
            }
            if (Modul1.DAus[85] == "1")
            {
                string UbgT = Modul1.Kont[41].Trim();
                Modul1.UbgT1 = Modul1.Retweg(Modul1.UbgT1);
                if (Modul1.Kont[41].Trim() != "")
                {
                    Anz[0].SelectedText = " Urkunde: " + Modul1.Kont[41].Trim();
                }
            }
            if (((M1_Ki & (Modul1.DAus[8] == "1")) | (!M1_Ki & (Modul1.DAus[2] == "1"))) && Modul1.Kont[16].Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[16];
                Bemaus();
            }
            if (((M1_Ki & (Modul1.DAus[9] == "1")) | (!M1_Ki & (Modul1.DAus[3] == "1"))) && Modul1.Kont[21].Trim() != "")
            {
                Modul1.UbgT1 = Modul1.Kont[21];
                Bemaus();
            }
        }
        short num = default;
        if (!M1_Ki)
        {
            num = (short)Modul1.DAus[96].AsInt();
        }
        if (M1_Ki)
        {
            num = (short)Modul1.DAus[98].AsInt();
        }
        if (num == 1 && Modul1.Kont[51].Trim() != "")
        {
            leerweg(Anz[0]);
            if (Modul1.DAus[72] == "1")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            }
            Anz[0].SelectedText = " ";
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
            if (Modul1.DAus[72] == "1")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
            }
            else
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
            }
            Anz[0].SelectedText = "Zeugen:";
            if (Modul1.DAus[72] == "1")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            }
            else
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
            Modul1.Kont[51] = Modul1.Retweg(Modul1.Kont[51]);
            Anz[0].SelectedText = " " + Modul1.Kont[51].Trim();
            Punktweg();
            Anz[0].SelectedText = ".";
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        }
        persInArb = Persp1;
        Modul1.Person_ReadNames(persInArb, Modul1.Person);
        M_Namen = Modul1.Kont[0];
        Art = EEventArt.eA_Baptism;
        ref long ahne2 = ref Ahne;
        Listart = 2;
        neb = false;
        Modul1.Datles3(Listart, ahne2, Art, ref neb);
        if ((Modul1.Kont[12].Trim() != "") | (Modul1.Kont[17].Trim() != "") | (Modul1.Kont[22].Trim() != ""))
        {
            Retweg2(Anz[0]);
            if ((Modul1.DAus[106].AsDouble() == 1.0) & (M_Indent.AsString().Right(1) != "1"))
            {
                Anz[0].SelectedText = "\n";
            }
            else
            {
                Anz[0].SelectedText = " ";
            }
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * Fackt), FontStyle.Regular);
            Anz[0].SelectedText = Modul1.DTxt[2];
            if (Modul1.Kont[12].Trim() != "")
            {
                Anz[0].SelectedText = " " + Modul1.Kont[12].Trim() + ".";
            }
            if (Modul1.Kont[32].Trim() != "")
            {
                Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                Anz[0].SelectedText = " " + Modul1.Kont[32].Trim();
                Anz[0].SelectionCharOffset = 0;
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
            if (Modul1.DAus[85].AsDouble() == 1.0)
            {
                string UbgT = Modul1.Kont[42].Trim();
                UbgT = Modul1.Retweg(UbgT);
                if (Modul1.Kont[42].Trim() != "")
                {
                    Anz[0].SelectedText = " Urkunde: " + Modul1.Kont[42].Trim();
                }
            }
            if (((M1_Ki & (Modul1.DAus[8] == "1")) | (!M1_Ki & (Modul1.DAus[2] == "1"))) && Modul1.Kont[17].Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[17];
                Bemaus();
            }
            if (((M1_Ki & (Modul1.DAus[9] == "1")) | (!M1_Ki & (Modul1.DAus[3] == "1"))) && Modul1.Kont[22].Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[22];
                Bemaus();
            }
        }
        if (Modul1.Kont[52].Trim() != "")
        {
            leerweg(Anz[0]);
            if (Modul1.DAus[72] == "1")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            }
            Anz[0].SelectedText = " ";
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
            if (Modul1.DAus[72] == "1")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
            }
            else
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
            }
            Anz[0].SelectedText = "Zeugen:";
            if (Modul1.DAus[72] == "1")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            }
            else
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
            Modul1.Kont[52] = Modul1.Retweg(Modul1.Kont[52]);
            Anz[0].SelectedText = " " + Modul1.Kont[52].Trim() + ".";
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        }
        Persp1 = persInArb;
        if (!M1_Ki)
        {
            num = (short)Modul1.DAus[36].AsInt();
        }
        if (M1_Ki)
        {
            num = (short)Modul1.DAus[48].AsInt();
        }
        string UbgT2 = "";
        if (num == "1".AsDouble())
        {
            Modul1.Paten2(persInArb, ref UbgT2, Ahne);
        }
        if (UbgT2 != "")
        {
            if (Modul1.DAus[100] == "1")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            }
            Anz[0].SelectedText = " ";
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
            if (Modul1.DAus[100] == "1")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
            }
            else
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
            }
            Anz[0].SelectedText = "Paten:";
            if (Modul1.DAus[100] == "1")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            }
            else
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
            if (Modul1.DAus[87].AsDouble() == 0.0)
            {
                UbgT2 = Modul1.Retweg(UbgT2);
            }
            Anz[0].SelectedText = " " + UbgT2.Trim() + ".";
            UbgT2 = "";
            Anz[0].SelectionCharOffset = 0;
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            if (Modul1.DAus[106].AsDouble() == 1.0)
            {
                Anz[0].SelectedText = "\n";
            }
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        }
        persInArb = Persp1;
        Modul1.Person_ReadNames(persInArb, Modul1.Person);
        M_Namen = Modul1.Kont[0];
        leerweg(Anz[0]);
        if (UbgT2 != "")
        {
            if (Modul1.DAus[100].AsDouble() == 1.0)
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            }
            Anz[0].SelectedText = " ";
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
            Anz[0].SelectedText = "Paten:";
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            if (Modul1.DAus[87].AsDouble() == 0.0)
            {
                UbgT2 = Modul1.Retweg(UbgT2);
            }
            Anz[0].SelectedText = " " + UbgT2.Trim();
            UbgT2 = "";
            Punktweg();
            Anz[0].SelectedText = ".";
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        }
        persInArb = Persp1;
        Modul1.Person_ReadNames(persInArb, Modul1.Person);
        M_Namen = Modul1.Kont[0];
        Art = EEventArt.eA_Death;
        ref long ahne3 = ref Ahne;
        Listart = 2;
        neb = false;
        Modul1.Datles3(Listart, ahne3, Art, ref neb);
        if ((Modul1.Kont[13].Trim() != "") | (Modul1.Kont[18].Trim() != "") | (Modul1.Kont[23].Trim() != ""))
        {
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * Fackt), FontStyle.Regular);
            Retweg2(Anz[0]);
            if ((Modul1.DAus[106].AsDouble() == 1.0) & (M_Indent.AsString().Right(1) != "1"))
            {
                Anz[0].SelectedText = "\n";
            }
            else
            {
                Anz[0].SelectedText = " ";
            }
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * Fackt), FontStyle.Regular);
            Anz[0].SelectedText = Modul1.DTxt[3];
            if (Modul1.Kont[13].Trim() != "")
            {
                Anz[0].SelectedText = " " + Modul1.Kont[13].Trim() + ".";
            }
            if (Modul1.DAus[62] == "1" && Modul1.Kont[33].Trim() != "")
            {
                Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                Anz[0].SelectedText = " " + Modul1.Kont[33].Trim();
                Anz[0].SelectionCharOffset = 0;
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
            if (Modul1.DAus[85].AsDouble() == 1.0)
            {
                string UbgT = Modul1.Kont[43].Trim();
                UbgT = Modul1.Retweg(UbgT);
                if (Modul1.Kont[43].Trim() != "")
                {
                    Anz[0].SelectedText = " Urkunde: " + Modul1.Kont[43].Trim();
                }
            }
            if (((M1_Ki & (Modul1.DAus[8] == "1")) | (!M1_Ki & (Modul1.DAus[2] == "1"))) && Modul1.Kont[18].Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[18];
                Bemaus();
            }
            if (((M1_Ki & (Modul1.DAus[9] == "1")) | (!M1_Ki & (Modul1.DAus[3] == "1"))) && Modul1.Kont[23].Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[23];
                Bemaus();
            }
        }
        if (Modul1.DAus[96].AsBool() && Modul1.Kont[53].Trim() != "")
        {
            leerweg(Anz[0]);
            if (Modul1.DAus[72] == "1")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            }
            Anz[0].SelectedText = " ";
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
            if (Modul1.DAus[72] == "1")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
            }
            else
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
            }
            Anz[0].SelectedText = "Zeugen:";
            if (Modul1.DAus[72] == "1")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            }
            else
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
            Modul1.Kont[53] = Modul1.Retweg(Modul1.Kont[53]);
            Anz[0].SelectedText = " " + Modul1.Kont[53].Trim() + ".";
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        }
        persInArb = Persp1;
        Modul1.Person_ReadNames(persInArb, Modul1.Person);
        M_Namen = Modul1.Kont[0];
        Art = EEventArt.eA_Burial;
        ref long ahne4 = ref Ahne;
        Listart = 2;
        neb = false;
        Modul1.Datles3(Listart, ahne4, Art, ref neb);
        if ((Modul1.Kont[14].Trim() != "") | (Modul1.Kont[19].Trim() != "") | (Modul1.Kont[24].Trim() != ""))
        {
            Retweg2(Anz[0]);
            if ((Modul1.DAus[106].AsDouble() == 1.0) & (M_Indent.AsString().Right(1) != "1"))
            {
                Anz[0].SelectedText = "\n";
            }
            else
            {
                Anz[0].SelectedText = " ";
            }
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * Fackt), FontStyle.Regular);
            Anz[0].SelectedText = Modul1.DTxt[4];
            if (Modul1.Kont[14].Trim() != "")
            {
                Anz[0].SelectedText = " " + Modul1.Kont[14].Trim() + ".";
            }
            if (Modul1.Kont[34].Trim() != "")
            {
                Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                Anz[0].SelectedText = " " + Modul1.Kont[34].Trim() + " ";
                Anz[0].SelectionCharOffset = 0;
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
            if (Modul1.DAus[85].AsDouble() == 1.0)
            {
                string UbgT = Modul1.Kont[44].Trim();
                UbgT = Modul1.Retweg(UbgT);
                if (Modul1.Kont[44].Trim() != "")
                {
                    Anz[0].SelectedText = " Urkunde: " + Modul1.Kont[44].Trim();
                }
            }
            if (((M1_Ki & (Modul1.DAus[8] == "1")) | (!M1_Ki & (Modul1.DAus[2] == "1"))) && Modul1.Kont[19].Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[19];
                Bemaus();
            }
            if (((M1_Ki & (Modul1.DAus[9] == "1")) | (!M1_Ki & (Modul1.DAus[3] == "1"))) && Modul1.Kont[24].Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[24];
                Bemaus();
            }
        }
        if (Modul1.DAus[96].AsBool() && Modul1.Kont[54].Trim() != "")
        {
            leerweg(Anz[0]);
            if (Modul1.DAus[72] == "1")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            }
            Anz[0].SelectedText = " ";
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
            if (Modul1.DAus[72] == "1")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
            }
            else
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
            }
            Anz[0].SelectedText = "Zeugen:";
            if (Modul1.DAus[72] == "1")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            }
            else
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
            Modul1.Kont[54] = Modul1.Retweg(Modul1.Kont[54]);
            Anz[0].SelectedText = " " + Modul1.Kont[54].Trim() + ".";
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        }
        if (Modul1.DAus[67] == "1")
        {
            Modul1.Datschalt = 1;
            Art = 0;
            string UbgT = 0.AsString();
            Listart = 0;
            neb = false;
            Modul1.Datles3(Listart, default, Art, ref neb);
            Modul1.Datschalt = 0;
            if (Modul1.Kont[25] != "")
            {
                Anz[0].SelectedText = " " + Modul1.Kont[25].Trim();
            }
        }
    }

    public void Punktweg()
    {
        while (true)
        {
            leerweg(Anz[0]);
            if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == ".")
            {
                Anz[0].SelectionStart = checked(Anz[0].SelectionStart - 1);
                Anz[0].SelectionLength = 1;
                Anz[0].SelectedText = "";
                leerweg(Anz[0]);
                continue;
            }
            break;
        }
    }

    public void Pate_bei(int persInArb)
    {
        Persp1 = persInArb;
        List2.Items.Clear();
        foreach (var cLink in DataModul.Link.ReadAllPers(persInArb, ELinkKennz.lkGodparent))
        {
            persInArb = cLink.iFamNr;
            DataModul.DB_EventTable.Index = "ArtNr";
            DataModul.DB_EventTable.Seek("=", 102.AsString(), persInArb.AsString(), "0");
            if (DataModul.DB_EventTable.NoMatch)
            {
                DataModul.DB_EventTable.Seek("=", 101.AsString(), persInArb.AsString(), "0");
            }
            string text = DataModul.DB_EventTable.NoMatch
            ? "          "
            : ((!(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate() != default))
                ? "          "
                : Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8));
            List2.Items.Add(new ListItem(text + ("          " + persInArb.AsString()).Right(10), persInArb));
        }
        checked
        {
            if (List2.Items.Count > 0)
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                object CounterResult = default;
                object LoopForResult = default;
                if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, List2.Items.Count - 1, 1, ref LoopForResult, ref CounterResult))
                {
                    do
                    {
                        persInArb = (int)Math.Round(Conversion.Val(Strings.Right(List2.Items[CounterResult.AsInt()].AsString(), 10)));
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
                        Retweg2(Anz[0]);
                        if (Modul1.DAus[106].AsDouble() == 1.0)
                        {
                            Anz[0].SelectedText = "\n";
                        }
                        else
                        {
                            Anz[0].SelectedText = " ";
                        }
                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        if (!Modul1.Pat)
                        {
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                            Anz[0].SelectedText = "Pate:";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
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
                        Anz[0].SelectedText = " " + text.Trim() + " bei " + Modul1.Kont[3].Trim() + " " + Modul1.Kont[0].Trim() + ".";
                        Modul1.Pat = true;
                    }
                    while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult, ref CounterResult));
                }
            }
        }
    }

    public void Quellen()
    {
        string text = 60.AsString();
        if (Modul1.PrintDat.BemZahl > 0)
        {
            if (Modul1.DAus[72] == "1")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            }
            else
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
            Anz[0].SelectedText = "Quellen:";
            int bemZahl = Modul1.PrintDat.BemZahl;
            int num;
            for (num = 1; num <= bemZahl; num = checked(num + 1))
            {
                if ((Modul1.DAus[71] == "1") & (num <= Modul1.PrintDat.BemZahl))
                {
                    Anz[0].SelectedText = "\n";
                }
                Modul1.UbgT1 = Modul1.KontBem[num];
                Modul1.KontBem[num] = "";
                Modul1.UbgT1 = Modul1.Retweg(Modul1.UbgT1);
                Anz[0].SelectedText = " " + Modul1.UbgT1;
                Modul1.UbgT1 = "";
            }
            Anz[0].SelectionCharOffset = 0;
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            Anz[0].SelectedText = "\n";
        }
        Modul1.PrintDat.BemZahl = 0;
        AhnList.Items.Clear();
        DataModul.DT_AncesterTable.Index = "Ahnen";
        text = Strings.Right(new string(' ', 40) + Ahn.AsDouble() + 1.0.AsString(), 40);
        DataModul.DT_AncesterTable.Seek(">=", text);
    }

    public void Kinder(ref byte LDC, int M_An)
    {
        IList<int> aiFams;
        if (LDC == 0)
        {
            T2(1);
        }
        M1_Ki = true;
        if (LDC == 0)
        {
            Anz[0].SelectionIndent = Kindeinzug;
        }
        if (M_An > 0)
        {
            if (M_An == 2)
            {
                return;
            }
            if ((M_An == 1) & (Modul1.Family.Mann == 0))
            {
                return;
            }
            if ((M_An == 1) & (Modul1.Family.Frau == 0))
            {
                return;
            }
        }
        Kindfer = true;
        M_An = 0;
        Anz[0].SelectionCharOffset = 0;
        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        Modul1.Famles();
        if (Anz[0].SelectionStart > 0 && Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n" && Modul1.Family.Kind[1] > 0)
        {
            Anz[0].SelectedText = "\n";
        }
        AhnList.Items.Clear();
        byte b = 1;
        checked
        {
            while (Modul1.Family.Kind[b] != 0)
            {
                DataModul.DB_EventTable.Seek("=", 101, Modul1.Family.Kind[b].AsString(), "0");
                DataModul.DT_SperrTable.Index = "Nr";
                DataModul.DT_SperrTable.Seek("=", Modul1.Family.Kind[b]);
                if (DataModul.DT_SperrTable.NoMatch)
                {
                    if (!DataModul.DB_EventTable.NoMatch)
                    {
                        Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                        if (Modul1.sDatu.Trim() == "0")
                        {
                            goto IL_037a;
                        }
                    }
                    else
                    {
                        DataModul.DB_EventTable.Seek("=", 102.AsString(), Modul1.Family.Kind[b].AsString(), "0");
                        if (DataModul.DB_EventTable.NoMatch)
                        {
                            Modul1.sDatu = 0.AsString();
                        }
                        if (!DataModul.DB_EventTable.NoMatch)
                        {
                            Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                        }
                    }
                    AhnList.Items.Add(Strings.Right("        " + Modul1.sDatu.AsDouble().AsString(), 8) + Modul1.Family.Kind[b].AsString());
                }
                goto IL_037a;
            IL_037a:
                b = (byte)unchecked((uint)(b + 1));
                if (unchecked(b) > 99u)
                {
                    break;
                }
            }
            M_Indent = (byte)KIident;
            Retweg2(Anz[0]);
            Anz[0].SelectedText = "\n";
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
            if (LDC == 0)
            {
                Anz[0].SelectionIndent = Kindeinzug;
            }
            if (AhnList.Items.Count > 0)
            {
                if (b > 2)
                {
                    Anz[0].SelectedText = "Kinder:\n";
                }
                else
                {
                    Anz[0].SelectedText = "Kind:\n";
                }
            }
            Anz[0].SelectionHangingIndent = 20;
            Anz[0].SelectionCharOffset = 0;
            int num = AhnList.Items.Count - 1;
            for (int i = 0; i <= num; i++)
            {
                Modul1.PersInArb = Conversions.ToInteger(Strings.Mid(AhnList.Items[i].AsString(), 9, AhnList.Items[i].AsString().Length));
                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                Modul1.Namenindex(Ahne);
                Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person, Modul1.DAus[89] == "1"));
                Anz[0].SelectionHangingIndent = 20;
                Anz[0].SelectionCharOffset = 0;
                Retweg2(Anz[0]);
                Anz[0].SelectedText = "\n";
                Anz[0].SelectionIndent = Kindeinzug;
                if (Modul1.DAus[76] == "1")
                {
                    Anz[0].SelectedText = " <" + Modul1.PersInArb.AsString().Trim() + "> ";
                }
                Anz[0].SelectedText = i + 1.AsString() + ".) " + (Modul1.Person.Prae.TrimEnd() + " " + Modul1.Kont[3]).Trim();
                if (Modul1.Kont[4].Trim() != "")
                {
                    Anz[0].SelectedText = " (" + Modul1.Kont[4] + ")";
                }
                if (Modul1.Kont[5].Trim() != "")
                {
                    Anz[0].SelectedText = ", Sippe " + Modul1.Kont[5];
                }
                Modul1.PerSatzLes(Modul1.PersInArb);
                if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields["religi"].Value) && Operators.CompareString(DataModul.DB_PersonTable.Fields["religi"].AsString().Trim(), "", TextCompare: false) != 0)
                {
                    Modul1.Ubg = DataModul.DB_PersonTable.Fields["religi"].AsInt();
                    string LD;
                    Modul1.UbgT = DataModul.TextLese1(Modul1.Ubg);
                    if (Modul1.UbgT.Trim() != "")
                    {
                        Anz[0].SelectedText = " Religion: " + Modul1.UbgT;
                    }
                }
                List1.Items.Clear();
                DataModul.DT_AncesterTable.Index = "PerNr";
                DataModul.DT_AncesterTable.MoveFirst();
                DataModul.DT_AncesterTable.Seek("=", Modul1.PersInArb);
                while (!DataModul.DT_AncesterTable.EOF)
                {
                    if (!DataModul.DT_AncesterTable.NoMatch)
                    {
                        if (DataModul.DT_AncesterTable.Fields["PerNr"].AsInt() != Modul1.PersInArb)
                        {
                            break;
                        }
                        if (DataModul.DT_AncesterTable.Fields["Ahn"].AsInt() > 0)
                        {
                            List1.Items.Add(DataModul.DT_AncesterTable.Fields["Ahn"].Value);
                        }
                    }
                    DataModul.DT_AncesterTable.MoveNext();
                }
                DataModul.DT_AncesterTable.Index = "PerNr";
                DataModul.DT_AncesterTable.Seek("=", Modul1.PersInArb);
                if (List1.Items.Count > 0)
                {
                    if (List1.Items.Count > 1)
                    {
                        Anz[0].SelectedText = ", siehe Nummer " + Strings.Trim(Conversion.Str(List1.Items[0].AsString().AsDouble())) + ">>";
                    }
                    else
                    {
                        Anz[0].SelectedText = ", siehe Nummer " + Strings.Trim(Conversion.Str(List1.Items[0].AsString().AsDouble()));
                    }
                    if (Modul1.DAus[90].AsDouble() == 0.0)
                    {
                        Anz[0].SelectedText = "\n";
                        continue;
                    }
                }
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                if ((Modul1.DAus[115] == "1") | (Modul1.DAus[116] == "1"))
                {
                    Module2.Bildaus("P", "FaBu");
                }
                if (Modul1.DAus[88] == "1")
                {
                    Bild("P", Modul1.PersInArb);
                }
                if (Modul1.DAus[62] == "1")
                {
                    PerQu1();
                }
                Datschreib(Modul1.PersInArb, document);
                if ((Modul1.DAus[42] == "1") | (Modul1.DAus[43] == "1"))
                {
                    Sonst();
                }
                if ((Modul1.DAus[24] == "1") | (Modul1.DAus[25] == "1"))
                {
                    EEventArt Beruf = EEventArt.eA_300;
                    Berufe(ref Beruf);
                }
                if ((Modul1.DAus[28] == "1") | (Modul1.DAus[29] == "1"))
                {
                    EEventArt Beruf = EEventArt.eA_301;
                    Berufe(ref Beruf);
                }
                if ((Modul1.DAus[32] == "1") | (Modul1.DAus[33] == "1"))
                {
                    EEventArt Beruf = EEventArt.eA_302;
                    Berufe(ref Beruf);
                }
                Perabschluss();
                Persp1 = Modul1.PersInArb;
                Persp2 = Modul1.PersInArb;
                M1_Ki = true;
                Modul1.PersInArb = Persp1;
                if (List1.Items.Count > 0)
                {
                    continue;
                }
                Anz[0].SelectionCharOffset = 0;
                Modul1.PerSatzLes(Modul1.PersInArb);
                Modul1.eLKennz = ELinkKennz.lkFather;
                if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                {
                    Modul1.eLKennz = ELinkKennz.lkMother;
                }
                aiFams = DataModul.Link.GetPersonFams(Modul1.PersInArb, Modul1.eLKennz);
                Fami = Modul1.UbgT;
                KiFamList.Items.Clear();
                int num2 = (int)Math.Round(Fami.Length / 10.0);
                for (int j = 1; j <= num2; j++)
                {
                    Modul1.FamInArb = Fami.Left(10).AsInt();
                    Modul1.Schalt = 1;
                    Modul1.Famdatles(Modul1.FamInArb, out var asFamDates);
                    Modul1.Schalt = 0;
                    if (Modul1.Kont[2].Trim() == "")
                    {
                        Modul1.Kont[2] = Modul1.Kont[3];
                    }
                    if (Modul1.Kont[2].Trim() == "")
                    {
                        Modul1.Kont[2] = Modul1.Kont[4];
                    }
                    if (Modul1.Kont[2].Trim() == "")
                    {
                        Modul1.Kont[2] = Modul1.Kont[1];
                    }
                    if (Modul1.Kont[2].Trim() == "")
                    {
                        Modul1.Kont[2] = Modul1.Kont[0];
                    }
                    if (Modul1.Kont[2].Trim() == "")
                    {
                        Modul1.Kont[2] = Modul1.Person.Prae;
                    }
                    if (Modul1.Kont[2].Trim() == "")
                    {
                        Modul1.Kont[2] = "        ";
                    }
                    if (Modul1.Kont[2].Trim() == "")
                    {
                        DataModul.DB_EventTable.Seek("=", 601, Modul1.FamInArb.AsString(), "0");
                        if (!DataModul.DB_EventTable.NoMatch)
                        {
                            Modul1.Kont[2] = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                        }
                    }
                    KiFamList.Items.Add(Modul1.Kont[2] + Fami.Left(10));
                    Fami = Strings.Mid(Fami, 11, Fami.Length);
                    if (Fami.Length < 10)
                    {
                        break;
                    }
                }
                if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "M")
                {
                    KennzBeh = ELinkKennz.lkFather;
                }
                if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                {
                    KennzBeh = ELinkKennz.lkMother;
                }
                byte b2 = Conversions.ToByte(KennzBeh);
                int num3 = KiFamList.Items.Count - 1;
                for (int j = 0; j <= num3; j++)
                {
                    Modul1.FamInArb = (int)Math.Round(Conversion.Val(KiFamList.Items[j].AsString().Right(10)));
                    M_Indent = (byte)KIident;
                    Anz[0].SelectionIndent = Kindeinzug;
                    Anz[0].SelectionHangingIndent = 20;
                    Modul1.Famles();
                    switch (b2)
                    {
                        case 1:
                            if (Modul1.Family.Mann != Modul1.PersInArb)
                            {
                                Anz[0].SelectedText = "\n";
                                Heidat(1, 1);
                            }
                            else
                            {
                                Heidat(1, 0);
                            }
                            break;
                        case 2:
                            if (Modul1.Family.Frau != Modul1.PersInArb)
                            {
                                Anz[0].SelectedText = "\n";
                                Heidat(1, 1);
                            }
                            else
                            {
                                Heidat(1, 0);
                            }
                            break;
                    }
                    DataModul.DB_FamilyTable.Index = "Fam";
                    DataModul.DB_FamilyTable.Seek("=", Modul1.FamInArb);
                    if (Modul1.DAus[106].AsDouble() == 1.0)
                    {
                        if ((Modul1.DAus[106].AsDouble() == 1.0) & (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0))
                        {
                            Anz[0].SelectedText = "\n";
                        }
                        Anz[0].SelectedText = "mit ";
                        if ((Modul1.DAus[106].AsDouble() == 1.0) & (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0))
                        {
                            Anz[0].SelectedText = "\n";
                        }
                    }
                    else
                    {
                        if (Anz[0].SelectionStart > 0 && Strings.Mid(Anz[0].Text, Anz[0].SelectionStart - 1, 2) == ". ")
                        {
                            Anz[0].SelectionStart = Anz[0].SelectionStart - 2;
                            Anz[0].SelectionLength = 2;
                            Anz[0].SelectedText = "";
                        }
                        leerweg(Anz[0]);
                        Anz[0].SelectedText = " mit ";
                    }
                    if (Modul1.Family.Kind[1] > 0)
                    {
                        FamKi = Modul1.FamInArb;
                    }
                    FamKi1 = Modul1.FamInArb;
                    Modul1.Famles();
                    Mannname = "";
                    Frauname = "";
                    Modul1.PersInArb = Modul1.Family.Mann;
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    Mannname = (Modul1.Kont[1] + " " + Modul1.Kont[0] + " " + Modul1.Kont[2]).Trim();
                    Modul1.PersInArb = Modul1.Family.Frau;
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    Frauname = (Modul1.Kont[1] + " " + Modul1.Kont[0] + " " + Modul1.Kont[2]).Trim();
                    Modul1.PersInArb = Persp2;
                    Modul1.PerSatzLes(Modul1.PersInArb);
                    if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                    {
                        KennzBeh = ELinkKennz.lkMother;
                    }
                    if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "M")
                    {
                        KennzBeh = ELinkKennz.lkFather;
                    }
                    if (KennzBeh == ELinkKennz.lkMother)
                    {
                        Modul1.PersInArb = Modul1.Family.Mann;
                    }
                    if (KennzBeh == ELinkKennz.lkFather)
                    {
                        Modul1.PersInArb = Modul1.Family.Frau;
                    }
                    if (Modul1.PersInArb == 0)
                    {
                        Anz[0].SelectedText = "unbekanntem Partner";
                    }
                    else
                    {
                        Modul1.eLKennz = KennzBeh;
                        Gatte();
                    }
                    Modul1.FamInArb = FamKi1;
                    Modul1.Famles();
                    T2(0);
                    if (Modul1.Family.Kind[1] <= 0)
                    {
                        continue;
                    }
                    short num4 = 1;
                    while (Modul1.Family.Kind[num4] != 0)
                    {
                        num4 = (short)unchecked(num4 + 1);
                        if (num4 > 99)
                        {
                            break;
                        }
                    }
                    if (num4 > 0)
                    {
                        Retweg2(Anz[0]);
                        Anz[0].SelectedText = "\n";
                        if (Modul1.DAus[89] == "1")
                        {
                            Mannname = Mannname.ToUpper();
                            Frauname = Frauname.ToUpper();
                        }
                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                        Anz[0].SelectedText = "Enkel " + Mannname + "/" + Frauname + ":";
                        Kindeskind();
                    }
                    if (Anz[0].SelectionStart > 0)
                    {
                    }
                    Retweg2(Anz[0]);
                    Anz[0].SelectedText = "\n";
                    Anz[0].SelectionIndent = Kindeinzug;
                    Anz[0].SelectionHangingIndent = 20;
                }
                M_Indent = (byte)KIident;
                Anz[0].SelectionHangingIndent = 20;
                if (Anz[0].SelectionStart > 0 && Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
                {
                    Anz[0].SelectedText = "\n";
                }
            }
            if (Anz[0].SelectionStart > 0 && Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
            {
                Anz[0].SelectedText = "\n";
            }
            if (LDC == 0)
            {
                Anz[0].SelectionIndent = 0;
                Anz[0].SelectionHangingIndent = 20;
                if (Modul1.PrintDat.BemZahl > 0)
                {
                    if (Modul1.DAus[72] == "1")
                    {
                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                    }
                    else
                    {
                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                    }
                    Anz[0].SelectedText = "Quellen: ";
                    int bemZahl = Modul1.PrintDat.BemZahl;
                    for (int k = 1; k <= bemZahl; k++)
                    {
                        if ((Modul1.DAus[71] == "1") & (k <= Modul1.PrintDat.BemZahl))
                        {
                            Anz[0].SelectedText = "\n";
                        }
                        Modul1.UbgT1 = Modul1.KontBem[k];
                        Modul1.KontBem[k] = "";
                        Modul1.UbgT1 = Modul1.Retweg(Modul1.UbgT1);
                        Anz[0].SelectedText = " " + Modul1.UbgT1;
                        Modul1.UbgT1 = "";
                    }
                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                    Anz[0].SelectedText = "\n";
                }
                Modul1.PrintDat.BemZahl = 0;
            }
            AhnList.Items.Clear();
            DataModul.DT_AncesterTable.Index = "Ahnen";
            string text = Strings.Right(new string(' ', 40) + Conversion.Str(Ahn.AsDouble() + 1d), 40);
            DataModul.DT_AncesterTable.Seek(">=", text);
            if (LDC == 0)
            {
                Anz[0].SelectionIndent = 0;
            }
            Anz[0].SelectedText = "\n";
        }
    }

    public void Kindeskind()
    {
        IList<int> aiFams;
        M1_Ki = true;
        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        Modul1.FamInArb = FamKi1;
        Modul1.Famles();
        if (Anz[0].SelectionStart > 0)
        {
            leerweg(Anz[0]);
            Anz[0].SelectionIndent = EnkelEinzug;
            if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
            {
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                if (Modul1.Family.Kind[1] > 0)
                {
                    Anz[0].SelectedText = "\n";
                    Anz[0].SelectionIndent = EnkelEinzug;
                }
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
        }
        Anz[0].SelectionIndent = EnkelEinzug;
        List4.Items.Clear();
        byte b = 1;
        checked
        {
            while (Modul1.Family.Kind[b] != 0)
            {
                DataModul.DB_EventTable.Seek("=", 101, Modul1.Family.Kind[b].AsString(), "0");
                if (!DataModul.DB_EventTable.NoMatch)
                {
                    Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                    if (Modul1.sDatu.Trim() != "0")
                    {
                        goto IL_02cb;
                    }
                }
                DataModul.DB_EventTable.Seek("=", 102.AsString(), Modul1.Family.Kind[b].AsString(), "0");
                if (DataModul.DB_EventTable.NoMatch)
                {
                    Modul1.sDatu = 0.AsString();
                }
                if (!DataModul.DB_EventTable.NoMatch)
                {
                    Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                }
                goto IL_02cb;
            IL_02cb:
                List4.Items.Add(Strings.Right("        " + Modul1.sDatu.AsDouble().AsString(), 8) + Modul1.Family.Kind[b].AsString());
                b = (byte)unchecked((uint)(b + 1));
                if (unchecked(b) > 99u)
                {
                    break;
                }
            }
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            if (Anz[0].SelectionStart > 0 && Strings.Mid(Anz[0].Text, Anz[0].SelectionStart - 1, 2) == "\n\n")
            {
                Anz[0].SelectionStart = Anz[0].SelectionStart - 1;
                Anz[0].SelectionLength = 1;
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                Anz[0].SelectedText = "";
            }
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            int num = List4.Items.Count - 1;
            for (int i = 0; i <= num; i++)
            {
                Modul1.PersInArb = Conversions.ToInteger(Strings.Mid(List4.Items[i].AsString(), 9, List4.Items[i].AsString().Length));
                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                Modul1.Namenindex(Ahne);
                Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person, Modul1.DAus[89] == "1"));
                Anz[0].SelectedText = "";
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                leerweg(Anz[0]);
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
                {
                    Anz[0].SelectedText = " ";
                    Anz[0].SelectedText = "\n";
                }
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                if (Modul1.DAus[76] == "1")
                {
                    Anz[0].SelectedText = " <" + Modul1.PersInArb.AsString().Trim() + "> ";
                }
                Anz[0].SelectedText = Strings.Chr(97 + i).AsString() + ") " + (Modul1.Person.Prae.TrimEnd() + " " + Modul1.Person.Givennames).Trim();
                if (Operators.CompareString(Mannname.ToUpper().Trim(), Modul1.Kont[0].ToUpper().Trim(), TextCompare: false) != 0)
                {
                    Anz[0].SelectedText = " " + Modul1.Kont[0];
                }
                Modul1.PerSatzLes(Modul1.PersInArb);
                if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields["religi"].Value) && Operators.CompareString(DataModul.DB_PersonTable.Fields["religi"].AsString().Trim(), "", TextCompare: false) != 0)
                {
                    Modul1.Ubg = DataModul.DB_PersonTable.Fields["religi"].AsInt();
                    string LD;
                    Modul1.UbgT = DataModul.TextLese1(Modul1.Ubg);
                    if (Modul1.UbgT.Trim() != "")
                    {
                        Anz[0].SelectedText = " Religion: " + Modul1.UbgT;
                    }
                }
                if (Modul1.Kont[4].Trim() != "")
                {
                    Anz[0].SelectedText = " (" + Modul1.Kont[4] + ")";
                }
                if (Modul1.Kont[5].Trim() != "")
                {
                    Anz[0].SelectedText = ", Sippe " + Modul1.Kont[5];
                }
                DataModul.DT_AncesterTable.Index = "PerNr";
                DataModul.DT_AncesterTable.Seek("=", Modul1.PersInArb);
                if (!DataModul.DT_AncesterTable.NoMatch)
                {
                    Anz[0].SelectedText = ", siehe Nummer " + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim() + "\n";
                    return;
                }
                if ((Modul1.DAus[115] == "1") | (Modul1.DAus[116] == "1"))
                {
                    Module2.Bildaus("P", "FaBu");
                }
                if (Modul1.DAus[88] == "1")
                {
                    Bild("P", Modul1.PersInArb);
                }
                if (Modul1.DAus[62] == "1")
                {
                    PerQu1();
                }
                Datschreib(Modul1.PersInArb, document);
                if ((Modul1.DAus[42] == "1") | (Modul1.DAus[43] == "1"))
                {
                    Sonst();
                }
                if ((Modul1.DAus[24] == "1") | (Modul1.DAus[25] == "1"))
                {
                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                    EEventArt Beruf = EEventArt.eA_300;
                    Berufe(ref Beruf);
                }
                if ((Modul1.DAus[28] == "1") | (Modul1.DAus[29] == "1"))
                {
                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                    EEventArt Beruf = EEventArt.eA_301;
                    Berufe(ref Beruf);
                }
                if ((Modul1.DAus[32] == "1") | (Modul1.DAus[33] == "1"))
                {
                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                    EEventArt Beruf = EEventArt.eA_302;
                    Berufe(ref Beruf);
                }
                Perabschluss();
                Persp1 = Modul1.PersInArb;
                Modul1.PersInArb = Persp1;
                Anz[0].SelectionCharOffset = 0;
                Modul1.PerSatzLes(Modul1.PersInArb);
                Modul1.eLKennz = ELinkKennz.lkFather;
                if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                {
                    Modul1.eLKennz = ELinkKennz.lkMother;
                }
                aiFams = DataModul.Link.GetPersonFams(Modul1.PersInArb, Modul1.eLKennz);
                Fami = Modul1.UbgT;
                KiKiFamlist.Items.Clear();
                short num2 = (short)Math.Round(Fami.Length / 10.0);
                for (short num3 = 1; num3 <= num2; num3 = (short)unchecked(num3 + 1))
                {
                    Modul1.FamInArb = Fami.Left(10).AsInt();
                    Modul1.Schalt = 1;
                    Modul1.Famdatles(Modul1.FamInArb, out var asFamDates);
                    Modul1.Schalt = 0;
                    if (Modul1.Kont[2].Trim() == "")
                    {
                        Modul1.Kont[2] = Modul1.Kont[3];
                    }
                    if (Modul1.Kont[2].Trim() == "")
                    {
                        Modul1.Kont[2] = Modul1.Kont[4];
                    }
                    if (Modul1.Kont[2].Trim() == "")
                    {
                        Modul1.Kont[2] = Modul1.Kont[1];
                    }
                    if (Modul1.Kont[2].Trim() == "")
                    {
                        Modul1.Kont[2] = Modul1.Kont[0];
                    }
                    if (Modul1.Kont[2].Trim() == "")
                    {
                        Modul1.Kont[2] = "        ";
                    }
                    if (Modul1.Kont[2].Trim().Length > 8)
                    {
                        Debugger.Break();
                    }
                    KiKiFamlist.Items.Add(Modul1.Kont[2] + Fami.Left(10));
                    Fami = Strings.Mid(Fami, 11, Fami.Length);
                    if (Fami.Length < 10)
                    {
                        break;
                    }
                }
                if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "M")
                {
                    KennzBeh = ELinkKennz.lkFather;
                }
                if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                {
                    KennzBeh = ELinkKennz.lkMother;
                }
                byte b2 = Conversions.ToByte(KennzBeh);
                short num4 = (short)(KiKiFamlist.Items.Count - 1);
                for (short num3 = 0; num3 <= num4; num3 = (short)unchecked(num3 + 1))
                {
                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                    Modul1.FamInArb = (int)Math.Round(Conversion.Val(KiKiFamlist.Items[num3].AsString().Right(10)));
                    Anz[0].SelectionIndent = EnkelEinzug;
                    if (num3 > 0)
                    {
                        Anz[0].SelectedText = "\n";
                    }
                    Modul1.Famles();
                    switch (b2)
                    {
                        case 1:
                            if (Modul1.Family.Mann != Modul1.PersInArb)
                            {
                                Anz[0].SelectedText = "\n";
                                Heidat(1, 1);
                            }
                            else
                            {
                                Heidat(1, 0);
                            }
                            break;
                        case 2:
                            if (Modul1.Family.Frau != Modul1.PersInArb)
                            {
                                Anz[0].SelectedText = "\n";
                                Heidat(1, 1);
                            }
                            else
                            {
                                Heidat(1, 0);
                            }
                            break;
                    }
                    DataModul.DB_FamilyTable.Index = "Fam";
                    DataModul.DB_FamilyTable.Seek("=", Modul1.FamInArb);
                    if (Qu3 == 1)
                    {
                        Anz[0].SelectedText = "Mit ";
                    }
                    else
                    {
                        if (Anz[0].SelectionStart > 0 && Strings.Mid(Anz[0].Text, Anz[0].SelectionStart - 1, 2) == ". ")
                        {
                            Anz[0].SelectionStart = Anz[0].SelectionStart - 2;
                            Anz[0].SelectionLength = 2;
                            Anz[0].SelectedText = "";
                        }
                        Retweg2(Anz[0]);
                        Anz[0].SelectedText = " mit ";
                    }
                    Qu3 = 0;
                    if (Modul1.Family.Kind[1] > 0)
                    {
                        FamKi = Modul1.FamInArb;
                    }
                    if (Modul1.PersInArb == 0)
                    {
                        Anz[0].SelectedText = "unbekanntem Partner";
                    }
                    else
                    {
                        Modul1.eLKennz = KennzBeh;
                        Modul1.Famles();
                        int famInArb = Modul1.FamInArb;
                        Gatte();
                        Modul1.FamInArb = famInArb;
                        Modul1.Famles();
                        Mannname = "";
                        Frauname = "";
                        Modul1.PersInArb = Modul1.Family.Mann;
                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                        Mannname = (Modul1.Kont[1] + " " + Modul1.Kont[0] + " " + Modul1.Kont[2]).Trim();
                        Modul1.PersInArb = Modul1.Family.Frau;
                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                        Frauname = (Modul1.Kont[1] + " " + Modul1.Kont[0] + " " + Modul1.Kont[2]).Trim();
                        if (Modul1.Family.Kind[1] > 0)
                        {
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Anz[0].SelectedText = "\n";
                            T2(1);
                            if (Anz[0].SelectionStart > 0 && Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            byte b3 = 1;
                            while (Modul1.Family.Kind[b3] != 0)
                            {
                                b3 = (byte)unchecked((uint)(b3 + 1));
                                if (unchecked(b3) > 99u)
                                {
                                    break;
                                }
                            }
                            if (b3 > 0)
                            {
                                Retweg2(Anz[0]);
                                Anz[0].SelectedText = "\n";
                                M_Indent = (byte)UrEnkelIdent;
                                if (Modul1.DAus[89] == "1")
                                {
                                    Mannname = Mannname.ToUpper();
                                    Frauname = Frauname.ToUpper();
                                }
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                                Anz[0].SelectedText = "Urenkel " + Mannname + "/" + Frauname + ":\n";
                                Anz[0].SelectionFont = Anz[0].SelectionFont.ChangeFName(Modul1.DAus[101]);
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            }
                            Urenkel();
                            Retweg2(Anz[0]);
                            Anz[0].SelectedText = "\n";
                            Anz[0].SelectionIndent = EnkelEinzug;
                        }
                    }
                }
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                if (Anz[0].SelectionStart > 0 && Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != "\n")
                {
                    Anz[0].SelectedText = "\n";
                }
                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
            leerweg(Anz[0]);
        }
    }

    public void Perabschluss()
    {
        Persp1 = Modul1.PersInArb;
        int famInArb = Modul1.FamInArb;
        Modul1.UbgT = "";
        Modul1.UbgT1 = "";
        Lauf += ">";
        if (Anz[0].SelectionStart > 5000)
        {
            Aufteil();
        }
        Bezeichnung1[2].Text = Lauf;
        Bezeichnung1[2].Refresh();
        Application.DoEvents();
        if (Lauf.Length >= 50)
        {
            Lauf = "";
        }
        if (((Modul1.DAus[92] == "1") & !M1_Ki) | ((Modul1.DAus[93] == "1") & M1_Ki))
        {
            short Listart = 2;
            Modul1.Zeuge_Bei(Modul1.PersInArb, ref Listart);
        }
        Modul1.PersInArb = Persp1;
        Modul1.FamInArb = famInArb;
        if ((((Modul1.DAus[37] == "1") & !M1_Ki) | ((Modul1.DAus[49] == "1") & M1_Ki)) && Modul1.DAus[37] == "1")
        {
            Pate_bei(Modul1.PersInArb);
        }
        Modul1.PersInArb = Persp1;
        Modul1.FamInArb = famInArb;
        Retweg2(Anz[0]);
        if (Modul1.DAus[106].AsDouble() == 1.0)
        {
            Anz[0].SelectedText = "\n";
        }
        else
        {
            Anz[0].SelectedText = " ";
        }
        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        if (Modul1.DAus[1] == "1")
        {
            Modul1.PerSatzLes(Modul1.PersInArb);
            if (Operators.CompareString(DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
            {
                float num = (Modul1.DAus[72] != "1") ? 1f : 0.8f;
                leerweg(Anz[0]);
                if (Modul1.DAus[94].AsDouble() == 1.0)
                {
                    Anz[0].SelectedText = "\n";
                }
                else
                {
                    Anz[0].SelectedText = " ";
                }
                Modul1.UbgT1 = DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString();
                if (Modul1.DTxt[16].Trim() != "")
                {
                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * (double)num), FontStyle.Underline);
                    Anz[0].SelectedText = Modul1.DTxt[16];
                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * (double)num), FontStyle.Regular);
                    Anz[0].SelectedText = ": ";
                }
                Bemaus();
            }
        }
        Retweg2(Anz[0]);
        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        Anz[0].SelectedText = "\n";
        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        Persp1 = Modul1.PersInArb;
        if (Anz[0].SelectionStart > 50000)
        {
            Aufteil();
        }
        M1_Ki = false;
    }
    public void Bild(string BKennz, int Nr)
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
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0007;
                    case 1291:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_041f;
                                default:
                                    goto end_IL_0000;
                            }
                            Debugger.Break();
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_0422;
                        }
                    end_IL_0000:
                        break;
                    IL_0007:
                        num = 2;
                        if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                        {
                            Anz[0].SelectedText = "\n";
                        }
                        if (Modul1.DAus[88] != "1")
                        {
                            goto end_IL_0000_2;
                        }
                        text = "";
                        DataModul.DB_PictureTable.Index = "Perkenn  ";
                        DataModul.DB_PictureTable.Seek("=", BKennz, Nr);
                        goto IL_0391;
                    IL_0230: // <========== 3
                        num = 24;
                        if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
                        {
                            Anz[0].SelectedText = "\n";
                        }
                        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        Retweg2(Anz[0]);
                        Anz[0].SelectedText = "\n";
                        Modul1.UbgT1 = "Bild: " + text;
                        if (Modul1.DAus[70] == "0")
                        {
                            Modul1.UbgT1 = Modul1.Retweg(Modul1.UbgT1);
                        }
                        if (BKennz == "F")
                        {
                            Retweg2(Anz[0]);
                            Anz[0].SelectedText = "\n";
                        }
                        Anz[0].SelectionColor = Color.Purple;
                        Anz[0].SelectedText = Modul1.UbgT1 + "\n";
                        Modul1.UbgT1 = "";
                        DataModul.DB_PictureTable.MoveNext();
                        goto IL_0391;
                    IL_0391: // <========== 3
                        num = 10;
                        if (!DataModul.DB_PictureTable.EOF)
                        {
                            if (!DataModul.DB_PictureTable.NoMatch)
                            {
                                if (!(DataModul.DB_PictureTable.Fields[PictureFields.ZuNr].AsInt() != Nr))
                                {
                                    if (!(DataModul.DB_PictureTable.Fields[PictureFields.Kennz].AsString() != BKennz))
                                    {
                                        if (DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Left(1) != "#")
                                            text = DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString() + DataModul.DB_PictureTable.Fields[PictureFields.Datei].AsString();
                                        else
                                        {
                                            text = Conversions.ToString(Modul1.Verz + Strings.Mid(DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString(), 2, DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Length) + DataModul.DB_PictureTable.Fields[PictureFields.Datei].Value);
                                        }
                                        goto IL_0230;
                                    }
                                }
                            }
                        }
                        goto IL_03a3;
                    IL_03a3: // <========== 3
                        num = 47;
                        Retweg2(Anz[0]);
                        if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) == 0)
                        {

                        }
                        else
                        {
                            Anz[0].SelectedText = "\n";
                        }
                        goto end_IL_0000_2;
                    IL_041f:
                        num4 = num2 + 1;
                        goto IL_0422;
                    IL_0422:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 20:
                            case 23:
                            case 24:
                                goto IL_0230;
                            case 9:
                            case 10:
                            case 46:
                                goto IL_0391;
                            case 13:
                            case 16:
                            case 43:
                            case 47:
                                goto IL_03a3;
                            case 50:
                            case 51:
                            case 52:
                            case 55:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 1291;
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

    public void Heidat(byte Block, byte FKK = 0)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        string ds = default;
        string text = default;
        string str = default;
        string Datu = default;
        int lErl = default;
        bool flag = default;
        byte b = default;
        byte selectionIndent = default;
        EEventArt num5 = default;
        EEventArt _eArt = default;
        byte b2 = default;
        byte b3 = default;
        string namen = default;
        byte b4 = default;
        short Listart = default;
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
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            Modul1.PamPam = 0;
                            goto IL_0009;
                        case 8591:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_1c15;
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
                                goto IL_1c19;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            ds = "";
                            text = "";
                            str = "";
                            Datu = "";
                            if (Block == 0)
                            {
                                Anz[0].SelectionIndent = 30;
                            }
                            if (M_Indent.AsString().Right(1) == "1")
                            {
                            }
                            if (Modul1.DAus[124].AsDouble() == 1.0)
                            {
                                num5 = EEventArt.eA_500;
                                goto IL_00a0;
                            }
                            goto IL_0282;
                        IL_00a0: // <========== 3
                            num = 14;
                            if (num5 != EEventArt.eA_504)
                            {
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", num5, Modul1.FamInArb.AsString(), "0");
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                    {
                                        if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > Modul1.DAus[125].AsDouble())
                                        {
                                            Modul1.PamPam = 1;
                                            goto end_IL_0000_2;
                                        }
                                    }
                                }
                            }
                            goto IL_01ac;
                        IL_01ac: // <========== 3
                            num = 27;
                            lErl = 1;
                            num5++;
                            if (num5 <= EEventArt.eA_505)
                            {
                                goto IL_00a0;
                            }
                            DataModul.DB_EventTable.Seek("=", 601, Modul1.FamInArb.AsString(), "0");
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > Modul1.DAus[125].AsDouble())
                                {
                                    Modul1.PamPam = 1;
                                    goto end_IL_0000_2;
                                }
                            }
                            goto IL_0282;
                        IL_0282: // <========== 3
                            num = 37;
                            Anz[0].SelectionHangingIndent = 15;
                            leerweg(Anz[0]);
                            flag = true;
                            b = 0;
                            selectionIndent = 0;
                            if (Anz[0].Text != "")
                            {
                                if (unchecked(Block > 0 && FKK == 0))
                                {
                                    Retweg2(Anz[0]);
                                    goto IL_031f;
                                }
                                if (FKK > 0)
                                {
                                    b = 2;
                                    selectionIndent = (byte)Anz[0].SelectionIndent;
                                }
                            }
                            goto IL_031f;
                        IL_031f: // <========== 4
                            num = 53;
                            if (M_Indent.AsString().Right(1) == "1")
                            {
                            }
                            Anz[0].SelectionHangingIndent = 15;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            if (Modul1.DAus[76].AsDouble() == 1.0)
                            {
                                Anz[0].SelectedText = " [" + Modul1.FamInArb.AsString().Trim() + "]";
                                flag = false;
                            }
                            goto IL_03e3;
                        IL_03e3:
                            num = 61;
                            num5 = EEventArt.eA_500;
                            goto IL_03ee;
                        IL_03ee: // <========== 3
                            num = 62;
                            if (num5 == EEventArt.eA_500)
                            {
                                _eArt = EEventArt.eA_501;
                                goto IL_042e;
                            }
                            if (num5 == EEventArt.eA_501)
                            {
                                _eArt = EEventArt.eA_500;

                            }
                            else
                            {
                                _eArt = num5;
                            }
                            goto IL_042e;
                        IL_042e: // <========== 4
                            num = 71;
                            Datu = "";
                            b2 = 1;
                            while (unchecked(b2) <= 8u)
                            {
                                Modul1.Kont1[b2] = "";
                                b2 = (byte)unchecked((uint)(b2 + 1));
                            }
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", _eArt, Modul1.FamInArb.AsString(), "0");
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                    ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                    Datu = Modul1.Datwand1(Datu, ds);
                                    Modul1.Kont1[1] = Datu;
                                }
                                goto IL_0592;
                            }
                            goto IL_19dd;
                        IL_0592:
                            num = 86;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                ds = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                Datu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumB].AsDate().AsString().Trim(), 8);
                                Datu = Modul1.Datwand1(Datu, ds);
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    Modul1.Kont1[3] = "/" + Datu;
                                    goto IL_06b3;
                                }
                                Modul1.Kont1[3] = " " + Datu;
                            }
                            goto IL_06b3;
                        IL_06b3: // <========== 3
                            num = 98;
                            Modul1.UbgT = "";
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                            {
                                ortNr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble());
                                Modul1.UbgT = Modul1.ortles(ortNr, 1);
                            }
                            goto IL_072a;
                        IL_072a:
                            num = 102;
                            if (DataModul.DB_EventTable.Fields[EventFields.Platz].AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                LD = "";
                                Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                if (Modul1.Kont[0] != "")
                                {
                                    Modul1.Kont1[8] = " " + Modul1.Kont[0].Trim();
                                }
                            }
                            goto IL_07e2;
                        IL_07e2: // <========== 3
                            num = 108;
                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                LD = "";
                                Modul1.Kont[0] = DataModul.TextLese1(AAA);
                                if (Modul1.Kont[0] != "")
                                {
                                    Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim();
                                }
                            }
                            goto IL_089a;
                        IL_089a: // <========== 3
                            num = 114;
                            b2 = 1;
                            while (unchecked(b2) <= 6u)
                            {
                                if (Modul1.Kont1[b2] == "0")
                                {
                                    Modul1.Kont1[b2] = "";
                                }
                                b2 = (byte)unchecked((uint)(b2 + 1));
                            }
                            if ((Modul1.Kont1[1].Trim() != "") | (Modul1.Kont1[2].Trim() != "") | (Modul1.Kont1[3].Trim() != "") | (Modul1.Kont1[5].Trim() != "") | (Modul1.Kont1[6].Trim() != "") | (Modul1.UbgT.Trim() != "") | ((Strings.Len(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim()) > 0) & (Modul1.DAus[5] == "1")) | ((Strings.Len(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim()) > 0) & (Modul1.DAus[6] == "1")))
                            {
                                text = Modul1.DTxt[13];
                                b = 1;
                                switch (_eArt)
                                {
                                    case EEventArt.eA_501:
                                        goto IL_0a65;
                                    case EEventArt.eA_500:
                                        goto IL_0ace;
                                    case EEventArt.eA_Marriage:
                                        goto IL_0b3a;
                                    case EEventArt.eA_MarrReligious:
                                        goto IL_0ba6;
                                    case EEventArt.eA_504:
                                        goto IL_0c12;
                                    case EEventArt.eA_505:
                                        goto IL_0c7f;
                                    case EEventArt.eA_507:
                                        goto IL_0ce9;
                                    default:
                                        break;
                                }
                                text = Modul1.DTxt[13];
                                goto IL_0d64;
                            }
                            goto IL_1648;
                        IL_0a65:
                            num = 126;
                            if (!(!M1_Ki & (Modul1.DAus[53] == "0")))
                            {
                                if (!(M1_Ki & (Modul1.DAus[58] == "0")))
                                {
                                    text = Modul1.DTxt[6];
                                    goto IL_0d64;
                                }
                            }
                            goto IL_19dd;
                        IL_0ace:
                            num = 135;
                            if (!(!M1_Ki & (Modul1.DAus[52] == "0")))
                            {
                                if (!(M1_Ki & (Modul1.DAus[57] == "0")))
                                {
                                    text = Modul1.DTxt[5];
                                    goto IL_0d64;
                                }
                            }
                            goto IL_19dd;
                        IL_0b3a:
                            num = 144;
                            if (!(!M1_Ki & (Modul1.DAus[64] == "0")))
                            {
                                if (!(M1_Ki & (Modul1.DAus[65] == "0")))
                                {
                                    text = Modul1.DTxt[7];
                                    goto IL_0d64;
                                }
                            }
                            goto IL_19dd;
                        IL_0ba6:
                            num = 153;
                            if (!(!M1_Ki & (Modul1.DAus[54] == "0")))
                            {
                                if (!(M1_Ki & (Modul1.DAus[59] == "0")))
                                {
                                    text = Modul1.DTxt[8];
                                    goto IL_0d64;
                                }
                            }
                            goto IL_19dd;
                        IL_0c12:
                            num = 162;
                            if (!(!M1_Ki & (Modul1.DAus[55] == "0")))
                            {
                                if (!(M1_Ki & (Modul1.DAus[60] == "0")))
                                {
                                    text = Modul1.DTxt[9];
                                    goto IL_0d64;
                                }
                            }
                            goto IL_19dd;
                        IL_0c7f:
                            num = 171;
                            if (!(!M1_Ki & (Modul1.DAus[56] == "0")))
                            {
                                if (!(M1_Ki & (Modul1.DAus[61] == "0")))
                                {
                                    text = Modul1.DTxt[10];
                                    goto IL_0d64;
                                }
                            }
                            goto IL_19dd;
                        IL_0ce9:
                            num = 180;
                            if (!(!M1_Ki & (Modul1.DAus[84] == "0")))
                            {
                                if (!(M1_Ki & (Modul1.DAus[88] == "0")))
                                {
                                    text = Modul1.DTxt[15];
                                    goto IL_0d64;
                                }
                            }
                            goto IL_19dd;
                        IL_0d64: // <========== 9
                            num = 191;
                            if ((Modul1.Kont1[1].Trim() != "") | (Modul1.Kont1[3].Trim() != "") | (Modul1.Kont1[5].Trim() != "") | (Modul1.Kont1[6].Trim() != "") | (Modul1.Kont1[7].Trim() != "") | (Modul1.Kont1[8].Trim() != "") | (Modul1.UbgT.Trim() != ""))
                            {
                                if (Anz[0].SelectionStart > 0)
                                {
                                    if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != " ")
                                    {
                                        Anz[0].SelectedText = " ";
                                    }

                                }
                                else
                                {
                                    Anz[0].SelectedText = " ";
                                }
                                goto IL_0eda;
                            }
                            goto IL_112b;
                        IL_0eda: // <========== 3
                            num = 200;
                            if (M_Indent.AsString().Right(1) != "1")
                            {
                                if (unchecked(Modul1.DAus[106].AsDouble() == 1.0 && flag))
                                {
                                    Retweg2(Anz[0]);
                                    Anz[0].SelectedText = "\n";
                                }
                            }
                            goto IL_0f4d;
                        IL_0f4d: // <========== 3
                            num = 206;
                            Anz[0].SelectionHangingIndent = 15;
                            flag = true;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * Fackt), FontStyle.Regular);
                            Anz[0].SelectedText = Strings.Trim(text + " " + Modul1.Kont1[1] + Modul1.Kont1[3] + Modul1.Kont1[5] + Modul1.Kont1[6] + Modul1.Kont1[7] + Modul1.Kont1[8] + " " + Modul1.UbgT.Trim()) + " ";
                            Modul1.UbgT = "";
                            text = "";
                            if (Modul1.DAus[85] == "1")
                            {
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Reg].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Reg].AsString().Trim();
                                    Modul1.UbgT1 = Modul1.Retweg(Modul1.UbgT1);
                                    Anz[0].SelectedText = "Urkunde: " + Modul1.UbgT1;
                                }
                            }
                            goto IL_112b;
                        IL_112b: // <========== 4
                            num = 220;
                            if (Strings.Len(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim()) > 0)
                            {
                                if (Modul1.DAus[5] == "1")
                                {
                                    if (text.Trim() != "")
                                    {
                                        if (Anz[0].SelectionStart > 2)
                                        {
                                            if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != " ")
                                            {
                                                Anz[0].SelectedText = " ";
                                            }
                                        }
                                        goto IL_1213;
                                    }
                                    goto IL_1245;
                                }
                            }
                            goto IL_12b4;
                        IL_1213: // <========== 3
                            num = 228;
                            Anz[0].SelectedText = text + " ";
                            text = "";
                            goto IL_1245;
                        IL_1245: // <========== 3
                            num = 231;
                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                            {
                                Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString();
                                Bemaus();
                            }
                            goto IL_12b4;
                        IL_12b4: // <========== 3
                            num = 237;
                            if (Strings.Len(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim()) > 0)
                            {
                                if (Modul1.DAus[6] == "1")
                                {
                                    if (text.Trim() != "")
                                    {
                                        if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) != " ")
                                        {
                                            Anz[0].SelectedText = " ";
                                        }
                                        Anz[0].SelectedText = text + " ";
                                        text = "";
                                    }
                                    Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString();
                                    Bemaus();
                                }
                            }
                            goto IL_13ea;
                        IL_13ea: // <========== 3
                            num = 250;
                            Modul1.Kont1[9] = "";
                            Nr = Modul1.FamInArb;
                            LfNR = 0;
                            Modul1.QuellenDatum(ref Nr, _eArt, ref LfNR);
                            Modul1.FamInArb = Nr.AsInt();
                            if (Modul1.Kont1[9].Trim() != "")
                            {
                                if (Anz[0].SelectionStart > 1)
                                {
                                    if (Strings.Mid(Anz[0].Text, Anz[0].SelectionStart, 1) == " ")
                                    {
                                        Anz[0].SelectionStart = Anz[0].SelectionStart - 1;
                                        Anz[0].SelectionLength = 1;
                                        Anz[0].SelectedText = "";
                                    }
                                }
                                goto IL_1511;
                            }
                            goto IL_1600;
                        IL_1511: // <========== 3
                            num = 260;
                            Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                            Anz[0].SelectedText = " " + Modul1.Kont1[9].Trim() + " ";
                            Modul1.Kont1[9] = "";
                            Anz[0].SelectionCharOffset = 0;
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            goto IL_1600;
                        IL_1600: // <========== 3
                            num = 267;
                            if (!M1_Ki)
                            {
                                b3 = (byte)Modul1.DAus[96].AsInt();
                            }
                            goto IL_1624;
                        IL_1624:
                            num = 270;
                            if (M1_Ki)
                            {
                                b3 = (byte)Modul1.DAus[98].AsInt();
                            }
                            goto IL_1648;
                        IL_1648: // <========== 3
                            num = 274;
                            if (b3 == 1)
                            {
                                namen = M_Namen;
                                Modul1.PersSp = Modul1.PersInArb;
                                b4 = 1;
                                while (unchecked(b4) <= 100u)
                                {
                                    Modul1.KontSP1[b4] = Modul1.Kont1[b4];
                                    Modul1.KontSP[b4] = Modul1.Kont[b4];
                                    Modul1.Kont[b4] = "";
                                    Modul1.Kont1[b4] = "";
                                    b4 = (byte)unchecked((uint)(b4 + 1));
                                }
                                Schalt = 0;
                                Modul1.Zeugsu(_eArt, 1, Listart, Ahne);
                                M_Namen = namen;
                                Modul1.PersInArb = Modul1.PersSp;
                                str = Modul1.Kont1[20];
                                b4 = 1;
                                while (unchecked(b4) <= 100u)
                                {
                                    Modul1.Kont1[b4] = Modul1.KontSP1[b4];
                                    Modul1.Kont[b4] = Modul1.KontSP[b4];
                                    Modul1.KontSP[b4] = "";
                                    Modul1.KontSP1[b4] = "";
                                    b4 = (byte)unchecked((uint)(b4 + 1));
                                }
                            }
                            goto IL_17a8;
                        IL_17a8:
                            num = 294;
                            if (str.Trim() != "")
                            {
                                leerweg(Anz[0]);
                                Anz[0].SelectedText = " ";
                                if (Modul1.DAus[100] == "1")
                                {
                                    leerweg(Anz[0]);
                                    Anz[0].SelectedText = " ";
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);

                                }
                                else
                                {
                                    Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                                }
                                goto IL_18b2;
                            }
                            goto IL_19d0;
                        IL_18b2: // <========== 3
                            num = 305;
                            Anz[0].SelectedText = "Zeugen:";
                            if (Modul1.DAus[72] == "1")
                            {
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);

                            }
                            else
                            {
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            }
                            goto IL_1966;
                        IL_1966: // <========== 3
                            num = 312;
                            Anz[0].SelectedText = " " + str.Trim();
                            str = "";
                            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            goto IL_19d0;
                        IL_19d0: // <========== 3
                            num = 316;
                            Retweg2(Anz[0]);
                            goto IL_19dd;
                        IL_19dd: // <========== 10
                            num = 317;
                            lErl = 22;
                            num5++;
                            if (num5 <= EEventArt.eA_507)
                            {
                                goto IL_03ee;
                            }
                            if (!flag)
                            {
                                Retweg2(Anz[0]);
                                Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * Fackt), FontStyle.Regular);
                                Anz[0].SelectedText = " " + Modul1.DTxt[13];
                            }
                            Retweg2(Anz[0]);
                            if (b == 2)
                            {
                                Anz[0].SelectedText = "\n";
                                Anz[0].SelectionIndent = selectionIndent;
                                Anz[0].SelectedText = Modul1.DTxt[13];
                            }
                            if (Block == 0)
                            {
                                Anz[0].SelectedText = "\n";
                            }
                            if ((Modul1.DAus[115] == "1") | (Modul1.DAus[116] == "1"))
                            {
                                Module2.Bildaus("F", "FaBu");
                            }
                            if (Modul1.DAus[88] == "1")
                            {
                                Bild("F", Modul1.FamInArb);
                            }
                            Retweg2(Anz[0]);
                            Anz[0].SelectedText = "\n";
                            goto end_IL_0000_2;
                        IL_1c15:
                            num4 = unchecked(num2 + 1);
                            goto IL_1c19;
                        IL_1c19:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 14:
                                    goto IL_00a0;
                                case 15:
                                case 24:
                                case 25:
                                case 26:
                                case 27:
                                    goto IL_01ac;
                                case 34:
                                case 35:
                                case 36:
                                case 37:
                                    goto IL_0282;
                                case 45:
                                case 50:
                                case 51:
                                case 52:
                                case 53:
                                    goto IL_031f;
                                case 60:
                                case 61:
                                    goto IL_03e3;
                                case 62:
                                    goto IL_03ee;
                                case 64:
                                case 67:
                                case 70:
                                case 71:
                                    goto IL_042e;
                                case 85:
                                case 86:
                                    goto IL_0592;
                                case 93:
                                case 96:
                                case 97:
                                case 98:
                                    goto IL_06b3;
                                case 101:
                                case 102:
                                    goto IL_072a;
                                case 106:
                                case 107:
                                case 108:
                                    goto IL_07e2;
                                case 112:
                                case 113:
                                case 114:
                                    goto IL_089a;
                                case 123:
                                case 133:
                                case 142:
                                case 151:
                                case 160:
                                case 169:
                                case 178:
                                case 187:
                                case 190:
                                case 191:
                                    goto IL_0d64;
                                case 195:
                                case 196:
                                case 199:
                                case 200:
                                    goto IL_0eda;
                                case 204:
                                case 205:
                                case 206:
                                    goto IL_0f4d;
                                case 217:
                                case 218:
                                case 219:
                                case 220:
                                    goto IL_112b;
                                case 226:
                                case 227:
                                case 228:
                                    goto IL_1213;
                                case 230:
                                case 231:
                                    goto IL_1245;
                                case 234:
                                case 235:
                                case 236:
                                case 237:
                                    goto IL_12b4;
                                case 248:
                                case 249:
                                case 250:
                                    goto IL_13ea;
                                case 258:
                                case 259:
                                case 260:
                                    goto IL_1511;
                                case 266:
                                case 267:
                                    goto IL_1600;
                                case 269:
                                case 270:
                                    goto IL_1624;
                                case 272:
                                case 273:
                                case 274:
                                    goto IL_1648;
                                case 293:
                                case 294:
                                    goto IL_17a8;
                                case 301:
                                case 304:
                                case 305:
                                    goto IL_18b2;
                                case 308:
                                case 311:
                                case 312:
                                    goto IL_1966;
                                case 315:
                                case 316:
                                    goto IL_19d0;
                                case 78:
                                case 127:
                                case 130:
                                case 136:
                                case 139:
                                case 145:
                                case 148:
                                case 154:
                                case 157:
                                case 163:
                                case 166:
                                case 172:
                                case 175:
                                case 181:
                                case 184:
                                case 317:
                                    goto IL_19dd;
                                case 23:
                                case 33:
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
                try0000_dispatch = 8591;
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
    public void T2(byte Kind)
    {
        int famInArb = Modul1.FamInArb;
        M1_Ki = false;
        Fb = false;
        Retweg2(Anz[0]);
        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        if (Kind > 0)
        {
            Anz[0].SelectedText = "\n";
        }
        Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        if (Modul1.DAus[106].AsDouble() == 1.0)
        {
            Anz[0].SelectedText = "\n\n";
        }
        if (Kind == 0)
        {
            if ((Modul1.DAus[20] == "1") | (Modul1.DAus[21] == "1"))
            {
                short Listart = 2;
                Modul1.FWohn(EEventArt.eA_602, ref Listart);
                Famwohn(EEventArt.eA_602, 0);
            }
            if ((Modul1.DAus[38] == "1") | (Modul1.DAus[39] == "1"))
            {
                short Listart = 2;
                Modul1.FWohn(EEventArt.eA_603, ref Listart);
                Famwohn(EEventArt.eA_603, 0);
            }
        }
        else
        {
            if ((Modul1.DAus[32] == "1") | (Modul1.DAus[33] == "1"))
            {
                short Listart = 2;
                Modul1.FWohn(EEventArt.eA_602, ref Listart);
                Famwohn(EEventArt.eA_602, 1);
            }
            if ((Modul1.DAus[42] == "1") | (Modul1.DAus[43] == "1"))
            {
                short Listart = 2;
                Modul1.FWohn(EEventArt.eA_603, ref Listart);
                Famwohn(EEventArt.eA_603, 1);
            }
        }
        if (Modul1.DAus[62] == "1")
        {
            int FamPer = 2;
            Modul1.PerQu(ref FamPer);
        }
        if (Modul1.Kont[30].Trim() != "")
        {
            Retweg2(Anz[0]);
            if (Kind > 0)
            {
                Anz[0].SelectedText = "\n\n";
            }
            if (Operators.CompareString(Anz[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
            {
                Anz[0].SelectedText = "\n";
            }
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            Retweg2(Anz[0]);
            if (Modul1.DAus[106].AsDouble() == 1.0)
            {
                Anz[0].SelectedText = "\n";
            }
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
            Anz[0].SelectedText = "\n";
            Anz[0].SelectedText = "Quellen zur Familie: ";
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            Anz[0].SelectionCharOffset = Modul1.PrintDat.Hoch;
            Anz[0].SelectedText = Modul1.Kont[30].Trim();
            Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        }
        if (famInArb > 0 && (((Kind == 0) & (Modul1.DAus[4] == "1")) | ((Kind == 1) & (Modul1.DAus[10] == "1"))))
        {
            DataModul.DB_FamilyTable.Index = "Fam";
            DataModul.DB_FamilyTable.Seek("=", famInArb);
            if (Operators.CompareString(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
            {
                Retweg2(Anz[0]);
                if (Kind > 0)
                {
                    Anz[0].SelectedText = "\n";
                }
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
                Bemaus();
                Modul1.UbgT1 = "";
                if (Kind > 0)
                {
                    Anz[0].SelectedText = "\n";
                }
            }
        }
    }

    public void Aufteil()
    {
    }//Discarded unreachable code: IL_0005, IL_0032


    private void _Befehl_3_Click(object sender, EventArgs e)
    {
    }

    public void AppendImage(Image image)
    {
        throw new NotImplementedException();
    }

    public void AppendText(string text)
    {
        throw new NotImplementedException();
    }

    public bool AppendTextIfNd(string sText = "\n")
    {
        throw new NotImplementedException();
    }

    public void ClearDocument()
    {
        throw new NotImplementedException();
    }

    public int GetIndent()
    {
        throw new NotImplementedException();
    }

    public void SetAlignment<T>(T eTextAlign) where T : Enum
    {
        throw new NotImplementedException();
    }

    public void SetFont(Font font)
    {
        throw new NotImplementedException();
    }

    public void SetIndent(int iIndent)
    {
        throw new NotImplementedException();
    }

    public void SetHangingIndent(int iHIndent)
    {
        throw new NotImplementedException();
    }

    public bool TrimEnd()
    {
        throw new NotImplementedException();
    }

    public bool TrimEnd(string sText)
    {
        throw new NotImplementedException();
    }

    public void ReplaceLast(string v1, string v2)
    {
        throw new NotImplementedException();
    }
}
