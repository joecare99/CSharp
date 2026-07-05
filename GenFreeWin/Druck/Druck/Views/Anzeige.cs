using BaseLib.Helper;
using Druck.My;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Sys;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Compatibility.VB6;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Druck.Views;

[DesignerGenerated]
public partial class Anzeige : Form
{

    private byte Qu1;

    private string Persuname;

    private short Listnam;

    private byte PerPos;

    private string Kidat;

    private byte Datmit;

    private byte Fk;

    private int IZahl;
    private short M1_J;
    private string Aufhoer;
    private string Anfang;
    private int M_Start;
    private string M_Namen;

    [Obsolete]
    IList Hinter_List1_Items => MyProject.Forms.Hinter.List1.Items;
    [Obsolete]
    IList Hinter_List2_Items => MyProject.Forms.Hinter.List2.Items;
    [Obsolete]
    FileDialog Hinter_DlgSave => MyProject.Forms.Hinter.CommonDialog1Save;
    [Obsolete]
    string Hinter_Anz_Texz { get => MyProject.Forms.Hinter.Anz.Text; set => MyProject.Forms.Hinter.Anz.Text = value; }

    [DebuggerNonUserCode]
    public Anzeige()
    {
        base.Load += Anzeige_Load;
        this.Befehl = new ControlArray<System.Windows.Forms.Button>();
        this.Command2 = new ControlArray<System.Windows.Forms.Button>();
        this.RichTextBox1 = new Microsoft.VisualBasic.Compatibility.VB6.RichTextBoxArray(this.components);
        InitializeComponent();
        Befehl.AddKeyPress(Befehl_KeyPress);
        Befehl.AddClick(Befehl_Click);
        Command2.AddClick(Command2_Click);
        this.Befehl.SetIndex(this._Befehl_9, 9);
        this.Befehl.SetIndex(this._Befehl_8, 8);
        this.Command2.SetIndex(this._Command2_4, 4);
        this.Command2.SetIndex(this._Command2_2, 2);
        this.Command2.SetIndex(this._Command2_1, 1);
        this.Command2.SetIndex(this._Command2_0, 0);
        this.Command2.SetIndex(this._Command2_3, 3);
        this.Befehl.SetIndex(this._Befehl_0, 0);
        this.RichTextBox1.SetIndex(this._RichTextBox1_1, 1);
        this.RichTextBox1.SetIndex(this._RichTextBox1_0, 0);
        this.Befehl.SetIndex(this._Befehl_1, 1);
        this.Befehl.SetIndex(this._Befehl_2, 2);
        this.Befehl.SetIndex(this._Befehl_3, 3);
        this.Befehl.SetIndex(this._Befehl_4, 4);
        this.Befehl.SetIndex(this._Befehl_5, 5);
        this.Befehl.SetIndex(this._Befehl_6, 6);
        this.Befehl.SetIndex(this._Befehl_7, 7);
        this.RichTextBox1.SetIndex(this._RichTextBox1_2, 2);
        this.RichTextBox1.SetIndex(this._RichTextBox1_3, 3);
        this.RichTextBox1.SetIndex(this._RichTextBox1_4, 4);
    }

    private void Befehl_Click(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int index = default;
        int num2 = default;
        int num3 = default;
        int number = default;
        string right = default;
        string text = default;
        string prompt = default;
        byte b = default;
        string text2 = default;
        object CounterResult = default;
        object LoopForResult = default;
        object LoopForResult2 = default;
        object CounterResult2 = default;
        object LoopForResult3 = default;
        object LoopForResult4 = default;
        object LoopForResult5 = default;
        object LoopForResult6 = default;
        object LoopForResult7 = default;
        object LoopForResult8 = default;
        object LoopForResult9 = default;
        object LoopForResult10 = default;
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
                        index = Befehl.GetIndex((Button)eventSender);
                        goto IL_0015;
                    case 8810:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_1d44;
                                default:
                                    goto end_IL_0000;
                            }
                            number = Information.Err().Number;
                            if (number == 7)
                            {
                                goto end_IL_0000_2;
                            }
                            if (number == 25)
                            {
                                prompt = "Das angegebene Gerät ist nicht bereit.\rBitte einschalten oder abbrechen.";
                                b = checked((byte)Interaction.MsgBox(prompt, MsgBoxStyle.OkCancel, "Fehler"));
                                if (b != 2)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_1d40;
                                }
                                goto end_IL_0000_2;
                            }
                            if (number == 55)
                            {
                                FileSystem.FileClose();
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1d40;
                            }
                            else if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_1d40;
                        }
                    end_IL_0000:
                        break;
                    IL_0015:
                        num = 2;
                        right = "";
                        text = "";
                        text2 = "";
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        Befehl[0].Visible = true;
                        Befehl[6].Visible = true;
                        Befehl[4].Visible = true;
                        if (!Befehl[4].Visible)
                        {
                            for (var i = 0; i <= 7; i++)
                            {
                                Befehl[i].Visible = true;
                            }
                            Befehl[0].Visible = false;
                            Befehl[4].Visible = false;
                            Befehl[6].Visible = false;
                        }
                        else for (var i = 0; i <= 7; i++)
                            {
                                Befehl[i].Visible = true;
                            }
                        switch (index)
                        {
                            case 0:
                                Befehl_0_Click(ref right, ref text, ref text2, ref CounterResult2, ref LoopForResult10);
                                break;
                            case 1:
                                Befehl_1_Click(ref CounterResult2, ref LoopForResult9);
                                break;
                            case 2:
                                Befehl_2_Click();
                                break;
                            case 3:
                                Befehl_3_Click(ref CounterResult2, ref LoopForResult8); break;
                            case 4:
                                Befehl_4_Click(ref CounterResult2, ref LoopForResult7);
                                break;
                            case 5:
                                Befehl_5_Click();
                                break;
                            case 6:
                                Befehl_6_Click(ref right, ref text, ref CounterResult2, ref LoopForResult6);
                                break;
                            case 7:
                                Befehl_7_Click();
                                break;
                            case 8:
                                for (var i = 0; i <= 4; i++)
                                {
                                    RichTextBox1[(short)i].Visible = false;
                                }
                                RichTextBox1[4].Text = "";
                                RichTextBox1[4].Visible = true;
                                DataModul.DSB_QuellIdxTable.Index = "Quelle";
                                DataModul.DSB_QuellIdxTable.MoveFirst();
                                for (var i = 1; i <= DataModul.DSB_QuellIdxTable.RecordCount; i++)
                                {
                                    text = DataModul.DSB_QuellIdxTable.Fields["Nr"].AsString();
                                    if (text.AsDouble() != _Modul1.Instance.AltNr)
                                    {
                                        _Modul1.Instance.AltNr = text.AsInt();
                                        DataModul.DB_QuTable.Index = "NR";
                                        DataModul.DB_QuTable.Seek("=", text);
                                        RichTextBox1[4].SelectionIndent = 0;
                                        RichTextBox1[4].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                        RichTextBox1[4].SelectedText = DataModul.DB_QuTable.Fields[QuFields._4].AsString() + '\n';
                                        RichTextBox1[4].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                        RichTextBox1[4].SelectionIndent = 40;
                                        if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._2].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            RichTextBox1[4].SelectedText = ("Titel: " + DataModul.DB_QuTable.Fields[QuFields._2].Value + '\n').AsString();
                                        }
                                        if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._5].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            RichTextBox1[4].SelectedText = ("Autor: " + DataModul.DB_QuTable.Fields[QuFields._5].Value + '\n').AsString();
                                        }
                                        _Modul1.Instance.UbgT = Module2.Repoles(DataModul.DB_QuTable.Fields[QuFields._1].AsInt());
                                        if (_Modul1.Instance.UbgT.Trim() != "")
                                        {
                                            RichTextBox1[4].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                            RichTextBox1[4].SelectedText = _Modul1.Instance.UbgT;
                                        }
                                        _Modul1.Instance.UbgT = "";
                                        if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._7].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            RichTextBox1[4].SelectedText = ("Herausgeber: " + DataModul.DB_QuTable.Fields[QuFields._7].Value + '\n').AsString();
                                        }
                                        if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._8].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            RichTextBox1[4].SelectedText = ("Erscheinungsort: " + DataModul.DB_QuTable.Fields[QuFields._8].Value + '\n').AsString();
                                        }
                                        if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._9].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            RichTextBox1[4].SelectedText = ("Erscheinungsdatum: " + DataModul.DB_QuTable.Fields[QuFields._9].Value + '\n').AsString();
                                        }
                                        if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._10].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            RichTextBox1[4].SelectedText = ("in: " + DataModul.DB_QuTable.Fields[QuFields._10].Value + '\n').AsString();
                                        }
                                        if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._11].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            RichTextBox1[4].SelectedText = ("Jahrgang: " + DataModul.DB_QuTable.Fields[QuFields._11].Value + " ").AsString();
                                        }
                                        if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._12].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            RichTextBox1[4].SelectedText = ("Nr.: " + DataModul.DB_QuTable.Fields[QuFields._12].AsString());
                                        }
                                        if ((Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._11].AsString().Trim(), "", TextCompare: false) != 0) | (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._12].AsString().Trim(), "", TextCompare: false) != 0))
                                        {
                                            RichTextBox1[4].SelectedText = "\n";
                                        }
                                        if (Operators.CompareString(DataModul.DB_QuTable.Fields[QuFields._13].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            RichTextBox1[4].SelectionIndent = 80;
                                            RichTextBox1[4].SelectedText = ("Bemerkungen: " + DataModul.DB_QuTable.Fields[QuFields._13].Value + '\n').AsString();
                                        }
                                    }
                                    if (RichTextBox1[4].Text.Right(1) != "\n")
                                    {
                                        RichTextBox1[4].SelectedText = "\n";
                                    }
                                    DataModul.DSB_QuellIdxTable.MoveNext();

                                }
                                break;
                            default:
                                break;
                        }
                        goto end_IL_0000_2;
                    IL_1c23:
                        num = 299;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_1d44;
                    IL_1d40: // <========== 3
                             // <========== 3
                             // <========== 3
                        num4 = num2;
                        goto IL_1d48;
                    IL_1d44:
                        num4 = num2 + 1;
                        goto IL_1d48;
                    IL_1d48:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 93:
                            case 96:
                            case 117:
                            case 120:
                            case 180:
                            case 181:
                            case 202:
                            case 237:
                            case 299:
                                goto IL_1c23;
                            case 23:
                            case 87:
                            case 88:
                            case 89:
                            case 99:
                            case 109:
                            case 110:
                            case 124:
                            case 127:
                            case 131:
                            case 134:
                            case 135:
                            case 136:
                            case 140:
                            case 141:
                            case 154:
                            case 155:
                            case 156:
                            case 157:
                            case 162:
                            case 209:
                            case 210:
                            case 225:
                            case 226:
                            case 227:
                            case 290:
                            case 291:
                            case 292:
                            case 293:
                            case 295:
                            case 298:
                            case 300:
                            case 301:
                            case 307:
                            case 308:
                            case 310:
                            case 311:
                            case 312:
                            case 316:
                            case 317:
                            case 323:
                            case 324:
                            case 325:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 8810;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 23
                       // <========== 25
                       // <========== 25
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    /// <summary>
    /// Behandelt das Klick-Ereignis für den Button "Befehl_7".
    /// </summary>
    /// <remarks>
    /// Diese Methode steuert die Sichtbarkeit verschiedener Steuerelemente (Buttons und RichTextBoxen) auf dem Formular.
    /// <para>
    /// Folgende Aktionen werden durchgeführt:
    /// <list type="bullet">
    /// <item>
    /// <description>Setzt <c>Befehl[4]</c> und <c>Befehl[6]</c> auf sichtbar (<c>true</c>).</description>
    /// </item>
    /// <item>
    /// <description>Setzt <c>Befehl[7]</c> auf unsichtbar (<c>false</c>).</description>
    /// </item>
    /// <item>
    /// <description>Setzt alle <c>RichTextBox1</c>-Elemente von Index 0 bis 4 auf unsichtbar (<c>false</c>).</description>
    /// </item>
    /// <item>
    /// <description>Setzt <c>RichTextBox1[0]</c> auf sichtbar (<c>true</c>).</description>
    /// </item>
    /// <item>
    /// <description>
    /// Falls <c>Listnam</c> den Wert 130 oder 119 hat, werden <c>Befehl[0]</c>, <c>Befehl[6]</c> und <c>Befehl[9]</c> auf unsichtbar gesetzt.
    /// Zusätzlich wird bei <c>Listnam == 119</c> auch <c>Befehl[4]</c> auf unsichtbar gesetzt.
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    private void Befehl_7_Click()
    {
        Befehl[4].Visible = true;
        Befehl[6].Visible = true;
        Befehl[7].Visible = false;
        for (var i = 0; i <= 4; i++)
        {
            RichTextBox1[(short)i].Visible = false;
        }
        RichTextBox1[0].Visible = true;
        if ((Listnam == 130) | (Listnam == 119))
        {
            Befehl[0].Visible = false;
            Befehl[6].Visible = false;
            Befehl[9].Visible = false;
            if (Listnam == 119)
                Befehl[4].Visible = false;
        }
    }

    /// <summary>
    /// Behandelt das Klicken auf den Button "Befehl_6" und zeigt einen Namen-Index (Kurzform) im RichTextBox1[1] an.
    /// </summary>
    /// <iQuelle name="right">
    /// Referenz auf einen String, der die zuletzt verarbeitete Nummer (Nr) speichert, um doppelte Einträge zu vermeiden.
    /// </iQuelle>
    /// <iQuelle name="text">
    /// Referenz auf einen String, der die aufgelisteten Nummern (Nr) für den aktuellen Namen sammelt und formatiert.
    /// </iQuelle>
    /// <iQuelle name="CounterResult2">
    /// Referenz auf ein Objekt, das als Zählervariable für die Schleife dient, um die Anz[0]-Elemente zu iterieren.
    /// </iQuelle>
    /// <iQuelle name="LoopForResult6">
    /// Referenz auf ein Objekt, das den Zustand der For-Loop-Kontrolle speichert.
    /// </iQuelle>
    /// <remarks>
    /// Diese Methode blendet zunächst den Button "Befehl[6]" aus und versteckt alle RichTextBox1-Elemente von 0 bis 4.
    /// Anschließend wird RichTextBox1[1] sichtbar gemacht und, falls sie leer ist, mit einem Namen-Index (Kurzform) befüllt.
    /// Die Namen werden aus der Datenquelle "DataModul.DSB_NamIdxTable" gelesen und gruppiert dargestellt.
    /// Für jeden neuen Namen wird der Name fett geschrieben und die zugehörigen Nummern werden aufgelistet.
    /// Die Methode verwendet verschiedene Formatierungen für die Anzeige und setzt die Indizes und Schriftarten entsprechend.
    /// </remarks>
    private void Befehl_6_Click(ref string right, ref string text, ref object CounterResult2, ref object LoopForResult6)
    {
        Befehl[6].Visible = false;
        for (var i = 0; i <= 4; i++)
        {
            RichTextBox1[(short)i].Visible = false;
            RichTextBox1[(short)i].Font = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
        }
        RichTextBox1[1].Visible = true;
        if (RichTextBox1[1].Text == "")
        {
            RichTextBox1[1].Text = "";
            var M_Namen = "";
            DataModul.DSB_NamIdxTable.Index = "Kurzname";
            RichTextBox1[1].SelectionIndent = 20;
            RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
            RichTextBox1[1].SelectedText = "Namen-Index (Kurzform)\n\n";
            RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            DataModul.DSB_NamIdxTable.Seek(">=", " ", 0);
            Bezeichnung1.Text = "";
            while (!DataModul.DSB_NamIdxTable.EOF)
            {
                Bezeichnung1.Text += ">";
                if (Bezeichnung1.Text.Length == 75)
                {
                    Bezeichnung1.Text = "";
                }
                if (M_Namen != DataModul.DSB_NamIdxTable.Fields["Name1"].AsString())
                {
                    if (M_Namen != "")
                    {
                        RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                        RichTextBox1[1].SelectedText = M_Namen;
                        RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        RichTextBox1[1].SelectedText = text + "\n\n";
                        text = "";
                        right = "";
                    }
                    M_Namen = DataModul.DSB_NamIdxTable.Fields["Name1"].AsString();
                }
                if (DataModul.DSB_NamIdxTable.Fields["Nr"].AsInt() != right.AsInt())
                {
                    text = text + ", " + DataModul.DSB_NamIdxTable.Fields["Nr"].AsString().Trim();
                    right = DataModul.DSB_NamIdxTable.Fields["Nr"].AsString();
                }
                DataModul.DSB_NamIdxTable.MoveNext();
            }
            RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
            RichTextBox1[1].SelectedText = M_Namen;
            RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            RichTextBox1[1].SelectedText = text + "\n\n";
            text = "";
            right = "";
        }
    }

    private void Befehl_5_Click()
    {
        Close();
        MyProject.Forms.Ausw.Close();
        MyProject.Forms.Druck.Show();
    }

    private void Befehl_4_Click(ref object CounterResult2, ref object LoopForResult7)
    {
        if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult2, 0, 4, 1, ref LoopForResult7, ref CounterResult2))
        {
            while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult2, LoopForResult7, ref CounterResult2))
            {
                RichTextBox1[(short)CounterResult2.AsInt()].Visible = false;
            }
        }
        RichTextBox1[3].Visible = true;
        Frame2.Visible = true;
        if ((Listnam == 130)
            || (Listnam == 119))
        {
            Befehl[0].Visible = false;
            Befehl[6].Visible = false;
            Befehl[9].Visible = false;
            if (Listnam == 119)
                Befehl[4].Visible = false;
        }
    }

    private void Befehl_3_Click(ref object CounterResult2, ref object LoopForResult8)
    {
        Hinter_DlgSave.FileName = "";
        if (Befehl[4].Visible)
        {
            Hinter_DlgSave.Filter = "Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF";
            if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult2, 0, 4, 1, ref LoopForResult8, ref CounterResult2))
            {
                while (!RichTextBox1[(short)CounterResult2.AsInt()].Visible
                    && ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult2, LoopForResult8, ref CounterResult2))
                {
                }
            }
            Hinter_DlgSave.InitialDirectory = _Modul1.Instance.GenFreeDir + "list\\";
            Hinter_DlgSave.FilterIndex = 2;
            Hinter_DlgSave.ShowDialog();
            if (Hinter_DlgSave.FileName != "")
            {
                switch (Hinter_DlgSave.FilterIndex)
                {
                    case 1:
                        RichTextBox1[(short)CounterResult2.AsInt()].SaveFile(Hinter_DlgSave.FileName, RichTextBoxStreamType.PlainText);
                        break;
                    case 2:
                        RichTextBox1[(short)CounterResult2.AsInt()].SaveFile(Hinter_DlgSave.FileName, RichTextBoxStreamType.RichText);
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            FileSystem.FileCopy(_Modul1.Instance.Verz1 + "Temp\\Register.mdb", _Modul1.Instance.Verz1 + "List\\Register.mdb");
            Interaction.MsgBox("Datei wurde unter " + _Modul1.Instance.Verz1 + "List\\Register.mdb gespeichert.\nVor Erstellen einer neuen Datei bitte sichern");
        }
    }

    private void Befehl_2_Click()
    {
        _Modul1.Instance.Ubg = 0;
        _Modul1.Instance.Ubg1 = 0;
        //       _Modul1.Instance.Modul1.Modul1.Kek = default;
        Application.DoEvents();
        Close();
        MyProject.Forms.Ausw.Close();
        MyProject.Forms.Ausw.Show();
    }

    private void Befehl_1_Click(ref object CounterResult2, ref object LoopForResult9)
    {
        if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult2, 0, 4, 1, ref LoopForResult9, ref CounterResult2))
        {
            while (!RichTextBox1[(short)CounterResult2.AsInt()].Visible
                    && ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult2, LoopForResult9, ref CounterResult2))
            {
            }
        }
        RichTextBox1[(short)CounterResult2.AsInt()].SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
        RichTextBox1[(short)CounterResult2.AsInt()].LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
        Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Temp\\Text2.RTF", AppWinStyle.MaximizedFocus);
    }

    private void Befehl_0_Click(ref string right, ref string text, ref string text2, ref object CounterResult2, ref object LoopForResult10)
    {
        Befehl[0].Visible = false;
        if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult2, 0, 4, 1, ref LoopForResult10, ref CounterResult2))
        {
            while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult2, LoopForResult10, ref CounterResult2))
            {
                RichTextBox1[(short)CounterResult2.AsInt()].Visible = false;
                RichTextBox1[(short)CounterResult2.AsInt()].Font = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            }
        }
        RichTextBox1[2].Visible = true;
        if (RichTextBox1[2].Text != "")
        {
            return;
        }
        RichTextBox1[2].Text = "";
        RichTextBox1[2].SelectionIndent = 10;
        RichTextBox1[2].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
        RichTextBox1[2].SelectedText = "Namen-Index (Langform)\n\n";
        RichTextBox1[2].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
        DataModul.DSB_NamIdxTable.Index = "Langi";
        DataModul.DSB_NamIdxTable.Seek(">=", " ", " ", 0);
        var M_Namen = "";
        Bezeichnung1.Text = "";
        while (!DataModul.DSB_NamIdxTable.EOF)
        {
            Bezeichnung1.Text += ">";
            if (Bezeichnung1.Text.Length == 75)
            {
                Bezeichnung1.Text = "";
            }
            RichTextBox1[2].SelectionIndent = 20;
            if (M_Namen != DataModul.DSB_NamIdxTable.Fields["Name1"].AsString())
            {
                if (text2 != "")
                {
                    RichTextBox1[2].SelectionIndent = 40;
                    RichTextBox1[2].SelectedText = text2 + "; " + Strings.Mid(text, 2, text.Length) + ".\n";
                    text = "";
                    right = "";
                    text2 = "";
                }
                RichTextBox1[2].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                RichTextBox1[2].SelectionIndent = 30;
                RichTextBox1[2].SelectedText = DataModul.DSB_NamIdxTable.Fields["Name1"].AsString() + '\n';
                RichTextBox1[2].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                text = "";
                right = "";
                M_Namen = DataModul.DSB_NamIdxTable.Fields["Name1"].AsString();
            }
            if (text2 == "")
            {
                text2 = DataModul.DSB_NamIdxTable.Fields["Name"].AsString();
            }
            if (text2 != DataModul.DSB_NamIdxTable.Fields["Name"].AsString())
            {
                RichTextBox1[2].SelectionIndent = 40;
                RichTextBox1[2].SelectedText = text2 + ": " + Strings.Mid(text, 2, text.Length) + ".\n";
                text = "";
                right = "";
                text2 = DataModul.DSB_NamIdxTable.Fields["Name"].AsString();
            }
            if (DataModul.DSB_NamIdxTable.Fields["Nr"].AsInt() != right.AsInt())
            {
                text = text + ", " + DataModul.DSB_NamIdxTable.Fields["Nr"].AsString().Trim();
                right = DataModul.DSB_NamIdxTable.Fields["Nr"].AsString();
            }
            DataModul.DSB_NamIdxTable.MoveNext();
        }
        if (text2 != "")
        {
            RichTextBox1[2].SelectionIndent = 70;
            RichTextBox1[2].SelectedText = text2 + "; " + Strings.Mid(text, 2, text.Length) + ".\n";
            text = "";
            right = "";
            text2 = "";
        }

    }

    private void Befehl_KeyPress(object eventSender, KeyPressEventArgs eventArgs)
    {
        short num = checked((short)Strings.Asc(eventArgs.KeyChar));
        int index = Befehl.GetIndex((Button)eventSender);
        eventArgs.KeyChar = Strings.Chr(num);
        if (num == 0)
        {
            eventArgs.Handled = true;
        }
    }

    private void Command2_Click(object eventSender, EventArgs eventArgs)
    {
        int index = Command2.GetIndex((Button)eventSender);
        Frame2.Visible = false;
        _Modul1.Instance.Ind1 = "";
        RichTextBox1[3].Text = "";
        RichTextBox1[3].SelectionHangingIndent = 0;
        RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
        switch (index)
        {
            case 0:
                {
                    RichTextBox1[3].SelectionAlignment = HorizontalAlignment.Center;
                    RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                    RichTextBox1[3].SelectedText = "Ortsindex";
                    RichTextBox1[3].SelectedText = "\n";
                    DataModul.DSB_OrtIdxTable.Index = "Ort";
                    DataModul.DSB_OrtIdxTable.Seek(">=", "");
                    int num = default;
                    while (!DataModul.DSB_OrtIdxTable.EOF)
                    {
                        if (DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt() != _Modul1.Instance.AltNr)
                        {
                            RichTextBox1[3].SelectionAlignment = HorizontalAlignment.Left;
                            RichTextBox1[3].SelectedText = "\n";
                            RichTextBox1[3].SelectionIndent = 0;
                            _Modul1.Instance.UbgT = _Modul1.Instance.ortles(DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt(), 0);
                            RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                            RichTextBox1[3].SelectedText = _Modul1.Instance.UbgT;
                            _Modul1.Instance.AltNr = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                            RichTextBox1[3].SelectedText = "\n";
                            RichTextBox1[3].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            RichTextBox1[3].SelectionIndent = 40;
                        }
                        if (Listnam != 130 && DataModul.DSB_OrtIdxTable.Fields["Ind"].Value.AsDouble() != num)
                        {
                            RichTextBox1[3].SelectedText = DataModul.DSB_OrtIdxTable.Fields["Ind"].AsString().Trim() + "; ";
                            num = checked((int)Math.Round(DataModul.DSB_OrtIdxTable.Fields["Ind"].Value.AsDouble()));
                        }
                        DataModul.DSB_OrtIdxTable.MoveNext();
                    }
                    break;
                }
            case 1:
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
                        _Modul1.Instance.UbgT = _Modul1.Instance.ortles(DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt(), 0);
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
                    if (Listnam != 130)
                    {
                        RichTextBox1[3].SelectedText = DataModul.DSB_OrtIdxTable.Fields["Ind"].AsString().Trim() + "; ";
                    }
                    DataModul.DSB_OrtIdxTable.MoveNext();
                }
                break;
            case 2:
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
                    if (Listnam != 130)
                    {
                        RichTextBox1[3].SelectedText = DataModul.DSB_OrtIdxTable.Fields["Ind"].AsString().Trim() + "; ";
                    }
                    DataModul.DSB_OrtIdxTable.MoveNext();
                }
                break;
            case 3:
                _Modul1.Instance.PrintDat.Flagsch = 1;
                DataModul.DSB_OrtIdxTable.Index = "Ort";
                DataModul.DSB_OrtIdxTable.Seek(">=", " ");
                while (!DataModul.DSB_OrtIdxTable.EOF)
                {
                    if (DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt() != _Modul1.Instance.AltNr)
                    {
                        RichTextBox1[3].SelectionIndent = 0;
                        _Modul1.Instance.UbgT = _Modul1.Instance.ortles(DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt(), 2);
                        RichTextBox1[3].SelectedText = _Modul1.Instance.UbgT;
                        _Modul1.Instance.AltNr = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                        RichTextBox1[3].SelectedText = "\n";
                    }
                    DataModul.DSB_OrtIdxTable.MoveNext();
                }
                break;
            default:
                Interaction.MsgBox(index);
                break;
        }
    }

    private void Anzeige_Load(object eventSender, EventArgs eventArgs)
    {
        Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
        Befehl[0].Enabled = false;
        Befehl[1].Enabled = false;
        Befehl[3].Enabled = false;
        Befehl[4].Enabled = false;
        Befehl[6].Enabled = false;
        Befehl[7].Enabled = false;
        Befehl[8].Enabled = false;
        Width = 1024;
        Height = 600;
        _Modul1.Instance.eWindowState = _Modul1.Instance.Persistence.ReadEnumInit<FormWindowState>("Windowstate");
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
        Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
        FileSystem.FileClose(6);
        Show();
    }

    private void Button1_Click(object sender, EventArgs e)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int num4 = default;
        int iListArt = default;
        string UbgT = default;
        string text = default;
        string inputStr = default;
        string text2 = default;
        string text3 = default;
        string text4 = default;
        int num7 = default;
        string text5 = default;
        byte b2 = default;
        int num9 = default;
        int num10 = default;
        int num11 = default;
        int num13 = default;
        int lErl = default;
        double d = default;
        string text8 = default;
        int num14 = default;
        int num16 = default;
        int num18 = default;
        int num22 = default;
        int famInArb = default;
        byte b5 = default;
        byte b6 = default;
        byte b7 = default;
        short num23 = default;
        short num24 = default;
        string left = default;
        bool flag = default;
        int num29 = default;
        int num30 = default;
        byte b9 = default;
        int num31 = default;
        int num32 = default;
        byte b10 = default;
        int famInArb2 = default;
        int persInArb = 0;
        Ausw ausw = MyProject.Forms.Ausw;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num5;
                    string text6;
                    int start;
                    Type typeFromHandle;
                    object[] array;
                    IField field;
                    object[] array2;
                    object[] arguments;
                    bool[] array3;
                    object obj;
                    int start2;
                    short Listart;
                    long Ahne;
                    byte PerPos;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            Show();
                            goto IL_0009;
                        case 55682:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 3:
                                    case 4:
                                        goto IL_b63d;
                                    case 1:
                                        goto IL_b718;
                                    default:
                                        goto end_IL_0000;
                                }
                                int number = Information.Err().Number;
                                if (number == 5)
                                {
                                    goto end_IL_0000_2;
                                }
                                if (number == 53)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_b718;
                                }
                                else if (number == 55)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_b718;
                                }
                                else if (number == 3022)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_b718;
                                }
                                else if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_b714;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            num = 2;
                            UbgT = "";
                            text = "";
                            inputStr = "";
                            string key = "";
                            text2 = "";
                            _Modul1.Instance.DAus[101] = _Modul1.Instance.Font1;
                            _Modul1.Instance.DAus[102] = "10";
                            Height = ausw.Height;
                            Width = ausw.Width;
                            Application.DoEvents();
                            num7 = 0;
                            while (num7 <= 4)
                            {
                                RichTextBox1[(short)num7].Top = 30;
                                RichTextBox1[(short)num7].Left = 20;
                                RichTextBox1[(short)num7].Width = Width - 40;
                                RichTextBox1[(short)num7].RightMargin = Width - 50;
                                RichTextBox1[(short)num7].Height = Height - 145;
                                RichTextBox1[(short)num7].Font = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                num7++;
                            }
                            Befehl[3].Text = _Modul1.Instance.IText[47];
                            Application.DoEvents();
                            text5 = "";
                            iListArt = _Modul1.Instance.Ubg;
                            Befehl[7].Visible = false;
                            Show();
                            Aufhoer = "";
                            Anfang = "";
                            GetListArtTexts(iListArt, out text3, out text4);
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            _Modul1.Instance.Verz = _Modul1.Instance.Persistence.ReadStringInit("GEN-verz.ini");
                            text6 = _Modul1.Instance.Verz.Left(2);
                            if (_Modul1.Instance.Verz.Right(1) != "\\")
                            {
                                _Modul1.Instance.Verz += "\\";
                            }
                            _Modul1.Instance.Verz1 = _Modul1.Instance.Verz.Left(15);
                            _Modul1.Instance.Dateienopen();
                            RichTextBox richTextBox1_0 = RichTextBox1[0];
                            string text7;
                            if (iListArt < 110)
                            {
                                if (Hinter_List2_Items.Count == 0)
                                {
                                    text7 = "Soll bei fehlendem" + text4 + " das " + text3 + " verwendet werden?";
                                    b2 = (byte)Interaction.MsgBox(text7, MsgBoxStyle.YesNo, "");
                                }
                                else
                                {
                                    b2 = 7;
                                }
                                num9 = 0;
                                if (M_Start > 0)
                                {
                                    num9 = M_Start;
                                }
                                num10 = (int)Math.Round(Conversion.Val(Conversion.Str(DateTime.Today.Year.ToString().AsDouble()) + "0000"));
                                num10 += 10000;
                                if (_Modul1.Instance.Ubg1 > 0)
                                {
                                    num10 = _Modul1.Instance.Ubg1 + 10000;
                                }
                                Anfang = " Jahr: " + Conversion.Str(Conversion.Val(Conversions.ToString(Conversions.ToDouble(Strings.Mid(M_Start.AsString(), 2, 4)) + 1.0)));
                                Aufhoer = " Jahr: " + Strings.Mid((num10 - 10000).AsString(), 2, 4);
                            }
                            Bezeichnung8.Text = "Einträge sortieren";
                            if (iListArt < 400 | iListArt > 1000)
                            {
                                Listnam = (short)iListArt;
                                Bezeichnung1.Text = "Sortierte Personenliste nach" + text4;
                                if (iListArt == 130)
                                {
                                    Bezeichnung1.Text = "Kontaktliste Namen";
                                }
                                if (iListArt is 101 or 102 or 103 or 104)
                                {
                                    Handle_ListArt_101f(iListArt, b2, num9, num10);
                                    goto IL_6640;
                                }
                                else if (iListArt == 120)
                                {
                                    PerPos = Handle_ListArt_120(out num7, out num11, out num13, out start, PerPos);
                                    goto IL_6640;
                                }
                                else if (iListArt is 121 or 119 or 130)
                                {
                                    Handle_ListArt_119f(iListArt, ref num23, ref Listart, ref PerPos);
                                    goto IL_6640;
                                }
                                else if (iListArt == 122)
                                {
                                    Handle_ListArt_112();
                                    goto IL_6640;
                                }
                                else if (iListArt == 135)
                                {
                                    Handle_ListArt_135();
                                    goto IL_6640;
                                }
                                else if (iListArt == 124)
                                {
                                    Handle_ListArt_124(out num3, out num11, ref lErl, ref d, ref PerPos);
                                    goto IL_6640;
                                }
                                else if (iListArt is 1101 or 1102 or 1103 or 1104)
                                {
                                    inputStr = Handle_ListArt_1101f(iListArt, inputStr, out num9, out num10);
                                    goto IL_6640;
                                }
                                else if (iListArt == 123)
                                {
                                    Handle_ListArt_123(ref lErl, out typeFromHandle, out array, out field, out array2, out arguments, out array3, out obj, ref PerPos);
                                    goto IL_6640;
                                }
                                else if (iListArt == 1500)
                                {
                                    Debugger.Break();
                                    goto IL_6640;
                                }
                                else if (iListArt == 1501)
                                {
                                    Debugger.Break();
                                    goto IL_6640;
                                }
                                else if (iListArt is 1502 or 1503)
                                {
                                    Bezeichnung1.Text = "Sortierte Familienliste nach" + text4;
                                    Bezeichnung2.Text = "Familie:";
                                    num10 = _Modul1.Instance.Ubg1 + 1231;
                                    DataModul.DB_FamilyTable.Index = "Fam";
                                    DataModul.DB_FamilyTable.MoveLast();
                                    num14 = DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();
                                    num7 = 1;
                                    while (num7 <= num14)
                                    {
                                        _Modul1.Instance.FamInArb = num7;
                                        Bezeichnung5.Text = num7.AsString();
                                        Bezeichnung5.Refresh();
                                        text8 = "";
                                        if (Hinter_List1_Items.Count > 0)
                                        {
                                            _Modul1.Instance.Datschalt = 2;
                                            _Modul1.Instance.Famdatles1(0, out _);
                                            if (_Modul1.Instance.Datschalt == 2)
                                            {
                                                _Modul1.Instance.Datschalt = 0;
                                                num7++;
                                                continue;
                                            }
                                            if (_Modul1.Instance.Datschalt == 3)
                                            {
                                                _Modul1.Instance.Datschalt = 0;
                                            }
                                        }
                                        int num15;
                                        if (Hinter_List2_Items.Count > 0)
                                        {
                                            if (DataModul.Event.ReadData(Listnam.AsEnum<EEventArt>(), _Modul1.Instance.FamInArb, out var cEvt))
                                            {
                                                num15 = Hinter_List2_Items.Count - 1;
                                                num4 = 0;
                                                while (num4 <= num15)
                                                {
                                                    var Kol = Hinter_List2_Items[num4].ItemData<int>();
                                                    if (cEvt.iOrt != Kol) num4++;
                                                }
                                            }
                                            else
                                            {
                                                num7++;
                                                continue;
                                            }
                                        }
                                        _Modul1.Instance.Datschalt = 10;
                                        _Modul1.Instance.Famdatles1(0, out var FamDat);
                                        _Modul1.Instance.Datschalt = 0;
                                        if (iListArt == 1502)
                                        {
                                            text8 = FamDat[2];
                                            inputStr = FamDat[12];
                                            if (FamDat[7] != "")
                                            {
                                                if (FamDat[2].Trim() == "")
                                                {
                                                    text8 = FamDat[7];
                                                }
                                            }
                                        }
                                        if (iListArt == 1503)
                                        {
                                            text8 = FamDat[3];
                                            inputStr = FamDat[13];
                                            if ((FamDat[3].Trim() == "") & (FamDat[0] != ""))
                                            {
                                                inputStr = FamDat[10];
                                                text8 = FamDat[0];
                                            }
                                        }
                                        if (text8.TrimEnd() != "")
                                        {
                                            if ((inputStr.AsDouble() >= M_Start) & (inputStr.AsDouble() <= num10))
                                            {
                                                DataModul_Sorttable_AddRow(_Modul1.Instance.FamInArb, text8);
                                            }
                                        }
                                        num7++;
                                    }
                                }
                                else if (iListArt != 1504)
                                {
                                }
                                goto IL_6640;
                            }
                            else if (iListArt > 499)
                            {
                                num9 = 0;
                                if (M_Start > 0)
                                {
                                    num9 = M_Start;
                                }
                                Anfang = " Jahr: " + Conversion.Str(Conversion.Val(Conversions.ToString(Conversions.ToDouble(Strings.Mid(M_Start.AsString(), 2, 4)) + 1.0)));
                                num10 = 20000000;
                                if (_Modul1.Instance.Ubg1 > 0)
                                {
                                    num10 = _Modul1.Instance.Ubg1 + 10000;
                                }
                                Aufhoer = " Jahr: " + Strings.Mid((num10 - 10000).AsString(), 2, 4);
                            }
                            if (unchecked(iListArt > 499 && iListArt < 506))
                            {
                                if (Hinter_List2_Items.Count == 0)
                                {
                                    text7 = "Soll bei fehlendem" + text4 + " ein anderes Familiendatum verwendet werden?\n Achtung!! Es werden alle Familien aufgelistet";
                                    text7 += "\n Eine solche Liste ist nur in wenigen Fällen sinnvoll";
                                    b2 = (byte)Interaction.MsgBox(text7, MsgBoxStyle.YesNo, "");
                                }
                                else b2 = 7;
                            }
                            Application.DoEvents();
                            if (b2 == 6)
                            {
                                Bezeichnung1.Text = "Sortierte Familienliste nach" + text4 + " oder einem andern Familiendatum.";
                            }
                            else
                            {
                                Bezeichnung1.Text = "Sortierte Familienliste nach" + text4;
                            }
                            Bezeichnung2.Text = "Familie:";
                            Application.DoEvents();
                            if (iListArt is 500 or 501 or 502 or 503 or 504 or 505)
                            {
                                DataModul.DB_FamilyTable.Index = "Fam";
                                DataModul.DB_FamilyTable.MoveLast();
                                num32 = DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();
                                num7 = 1;
                                while (num7 <= num32)
                                {
                                    Bezeichnung5.Text = num7.AsString();
                                    Bezeichnung5.Refresh();
                                    _Modul1.Instance.FamInArb = num7;
                                    if (Hinter_List1_Items.Count > 0)
                                    {
                                        _Modul1.Instance.Datschalt = 2;
                                        _Modul1.Instance.Famdatles(_Modul1.Instance.FamInArb, out _);
                                        if (_Modul1.Instance.Datschalt == 2)
                                        {
                                            _Modul1.Instance.Datschalt = 0;
                                            num7++;
                                            continue;
                                        }
                                        else if (_Modul1.Instance.Datschalt == 3)
                                        {
                                            _Modul1.Instance.Datschalt = 0;
                                        }
                                    }
                                    int num6;
                                    if (Hinter_List2_Items.Count > 0)
                                    {
                                        DataModul.DB_EventTable.Index = "ArtNr";
                                        DataModul.DB_EventTable.Seek("=", iListArt, _Modul1.Instance.FamInArb, "0");
                                        if (!DataModul.DB_EventTable.NoMatch)
                                        {
                                            num4 = 0;
                                            while (num4 < Hinter_List2_Items.Count
                                                && DataModul.DB_EventTable.Fields[EventFields.Ort].AsInt() != Hinter_List2_Items[num4].ItemData<int>())
                                            {
                                                num4++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _Modul1.Instance.Schalt = 1;
                                        _Modul1.Instance.Famdatles(_Modul1.Instance.FamInArb, out var asFamDates);
                                        text8 = "";
                                        _Modul1.Instance.Schalt = 0;
                                        if (unchecked(asFamDates[checked(iListArt - 500)] == "" && b2 == 6))
                                        {
                                            if (asFamDates[iListArt - 500] == "")
                                            {
                                                asFamDates[iListArt - 500] = asFamDates[2];
                                            }
                                            if (asFamDates[iListArt - 500] == "")
                                            {
                                                asFamDates[iListArt - 500] = asFamDates[3];
                                            }
                                            if (asFamDates[iListArt - 500] == "")
                                            {
                                                asFamDates[iListArt - 500] = asFamDates[4];
                                            }
                                            if (asFamDates[iListArt - 500] == "")
                                            {
                                                asFamDates[iListArt - 500] = asFamDates[1];
                                            }
                                            if (asFamDates[iListArt - 500] == "")
                                            {
                                                asFamDates[iListArt - 500] = asFamDates[0];
                                            }
                                            text8 = asFamDates[iListArt - 500];
                                        }
                                        else text8 = asFamDates[iListArt - 500];
                                        if (text8.TrimEnd() != "")
                                        {
                                            if ((text8.AsDouble() > num9) & (text8.AsDouble() < num10))
                                            {
                                                DataModul_Sorttable_AddRow(_Modul1.Instance.FamInArb, text8);
                                            }
                                        }
                                    }
                                    num7++;
                                }
                                goto IL_6640;
                            }
                            else if (iListArt == 401)
                            {
                                num11 = _Modul1.Instance.Ubg1;
                                _Modul1.Instance.Ubg1 = 0;
                                DataModul.DB_FamilyTable.Index = "Fam";
                                DataModul.DB_FamilyTable.MoveLast();
                                int num21 = DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();
                                M_Start = (int)Math.Round(ausw.bereich3.Text.AsDouble());
                                if (M_Start == 0)
                                {
                                    M_Start = 1;
                                }
                                if (unchecked(num11 == 0 || num11 > num21))
                                {
                                    num11 = num21;
                                }
                                Aufhoer = "Nr.:" + num11.AsString();
                                Anfang = "Nr.:" + M_Start.AsString();
                                start2 = M_Start;
                                num22 = num11;
                                num7 = start2;
                                while (num7 <= num22)
                                {
                                    Bezeichnung5.Text = num7.AsString();
                                    Bezeichnung5.Refresh();
                                    if (Hinter_List1_Items.Count > 0)
                                    {
                                        _Modul1.Instance.Datschalt = 2;
                                        _Modul1.Instance.FamInArb = num7;
                                        _Modul1.Instance.Famdatles(_Modul1.Instance.FamInArb, out _);
                                        if (_Modul1.Instance.Datschalt == 2)
                                        {
                                            _Modul1.Instance.Datschalt = 0;
                                        }
                                        else if (_Modul1.Instance.Datschalt == 3)
                                        {
                                            _Modul1.Instance.Datschalt = 0;
                                            DataModul_Sorttable_AddRow(num7, "                                                   " + num7.AsString().Right(50));
                                        }
                                    }
                                    else DataModul_Sorttable_AddRow(num7, "                                                   " + num7.AsString().Right(50));
                                    lErl = 112;
                                    num7++;
                                }
                                WriteList401(richTextBox1_0, ausw, ref famInArb, ref persInArb);
                                goto end_IL_0000_2;
                            }
                            else if (iListArt is not 402 and not 403)
                            {
                                goto end_IL_0000_2;
                            }
                            else
                            {
                                b7 = (byte)Interaction.MsgBox("Sollen Personen mit mehreren Ehen nur einmal in der Liste erscheinen (mit allen Ehen)?", MsgBoxStyle.YesNo | MsgBoxStyle.Question, "");
                                Anfang = " " + _Modul1.Instance.UbgT.ToUpper();
                                Persuname = Anfang;
                                Anfang = Persuname;
                                Show();
                                Application.DoEvents();
                                Show();
                                if (_Modul1.Instance.Ubg1T == "")
                                {
                                    _Modul1.Instance.Ubg1T = "z";
                                }
                                Aufhoer = " " + _Modul1.Instance.Ubg1T.ToUpper();
                                Persuname = Aufhoer;
                                Aufhoer = Persuname.Trim();
                                DataModul.DSB_SearchTable.Index = "Persuch";
                                DataModul.DSB_SearchTable.Seek(">=", _Modul1.Instance.UbgT, 0);
                                while (!DataModul.DSB_SearchTable.EOF)
                                {
                                    persInArb = DataModul.DSB_SearchTable.Fields["Nummer"].AsInt();
                                    _Modul1.Instance.PerSatzLes(persInArb);
                                    if (!Conversions.ToBoolean((DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "M") & iListArt == 403))
                                    {
                                        if (!Conversions.ToBoolean((DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F") & iListArt == 402))
                                        {
                                            Persuname = DataModul.DSB_SearchTable.Fields["Name"].AsString();
                                            if (Operators.CompareString(Anfang.ToUpper(), Strings.UCase(Persuname.Left(Anfang.Length)), TextCompare: false) > 0)
                                            {
                                                Debugger.Break();
                                                //                                          goto IL_153d;
                                            }
                                            else if (Operators.CompareString(Strings.UCase(Persuname.Left(Aufhoer.Length)), Aufhoer.ToUpper(), TextCompare: false) > 0 && Strings.Asc(Persuname.Left(1)) < 90)
                                                goto IL_6640;
                                            Bezeichnung5.Text = DataModul.DSB_SearchTable.Fields["Name"].AsString();
                                            Bezeichnung5.Refresh();
                                            persInArb = DataModul.DSB_SearchTable.Fields["Nummer"].AsInt();
                                            if (!_Modul1.Instance.SortVND)
                                            {
                                                num23 = (short)Strings.InStr(Persuname, ",");
                                                if (num23 == 0)
                                                {
                                                    num23 = 20;
                                                }
                                                Persuname = Persuname.Left(num23 - 1);
                                            }
                                            _Modul1.Instance.eLKennz = ELinkKennz.lkNone;
                                            if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "M")
                                            {
                                                _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                            }
                                            else if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                                            {
                                                _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                                            }
                                            if (_Modul1.Instance.eLKennz != ELinkKennz.lkNone)
                                            {
                                                var aiFams = DataModul.Link.GetPersonFams(persInArb, _Modul1.Instance.eLKennz);
                                                text5 = _Modul1.Instance.UbgT;
                                                num31 = (int)Math.Round(text5.Length / 10.0);
                                                num7 = 1;
                                                foreach (var iFam in aiFams)
                                                {
                                                    if (Hinter_List1_Items.Count > 0)
                                                    {
                                                        _Modul1.Instance.Datschalt = 2;
                                                        _Modul1.Instance.Famdatles(iFam, out _);
                                                        if (_Modul1.Instance.Datschalt == 2)
                                                        {
                                                            _Modul1.Instance.Datschalt = 0;
                                                        }
                                                        else if (_Modul1.Instance.Datschalt == 3)
                                                        {
                                                            _Modul1.Instance.Datschalt = 0;
                                                        }
                                                    }
                                                    _Modul1.Instance.Schalt = 1;
                                                    _Modul1.Instance.Famdatles(iFam, out var asFamDates);
                                                    _Modul1.Instance.Schalt = 0;
                                                    if (asFamDates[2] == "")
                                                    {
                                                        asFamDates[2] = asFamDates[3];
                                                    }
                                                    if (asFamDates[2] == "")
                                                    {
                                                        asFamDates[2] = asFamDates[4];
                                                    }
                                                    if (asFamDates[2] == "")
                                                    {
                                                        asFamDates[2] = asFamDates[5];
                                                    }
                                                    if (asFamDates[2] == "")
                                                    {
                                                        asFamDates[2] = asFamDates[1];
                                                    }
                                                    if (asFamDates[2] == "")
                                                    {
                                                        asFamDates[2] = asFamDates[0];
                                                    }
                                                    if (asFamDates[2] == "")
                                                    {
                                                        asFamDates[2] = "        ";
                                                    }
                                                    _Modul1.Instance.Datschalt = 0;
                                                    DataModul_Sorttable_AddRow(iFam, Persuname + asFamDates[2] + "                    " + iFam.AsString().Right(20));
                                                    text5 = Strings.Mid(text5, 11, text5.Length);
                                                    num7++;
                                                }
                                            }
                                        }
                                    }
                                    DataModul.DSB_SearchTable.MoveNext();
                                }
                            }
                            goto IL_6640;

                        IL_6640: // <========== 15
                            if (iListArt is 1502 or 1503 or 500 or 501 or 502 or 503 or 504 or 505)
                            {
                                WriteFamilyList(ref num, iListArt, ref text, ref famInArb, ref b6, ref b10, ref famInArb2, ref persInArb, ausw, richTextBox1_0);
                            }
                            else
                                WritePersonList(iListArt, richTextBox1_0, ref persInArb, ref famInArb, ausw, ref text5, ref num24, ref PerPos);
                            goto end_IL_0000_2;

                        IL_b63d:
                            num = 2182;
                            if (Information.Err().Number == 3021)
                            {
                                Interaction.MsgBox("Keine Ahnenberechnung");
                                Close();
                                goto end_IL_0000_2;
                            }
                            if (Information.Err().Number == 5)
                            {
                                goto end_IL_0000_2;
                            }
                            if (Information.Err().Number == 3022)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_b718;
                            }
                            else if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_b714;
                        IL_b714:
                            num5 = num2;
                            goto IL_b71c;
                        IL_b718: // <========== 5
                            num5 = unchecked(num2 + 1);
                            goto IL_b71c;
                        IL_b71c:
                            num2 = 0;
                            switch (num5)
                            {
                                case 1:
                                    break;
                                case 247:
                                case 254:
                                case 264:
                                case 309:
                                case 314:
                                case 319:
                                case 324:
                                case 329:
                                case 330:
                                case 331:
                                case 287:
                                case 328:
                                case 343:
                                case 837:
                                case 282:
                                case 283:
                                case 345:
                                case 371:
                                case 378:
                                case 392:
                                case 430:
                                case 364:
                                case 365:
                                case 394:
                                case 420:
                                case 421:
                                case 446:
                                case 545:
                                case 567:
                                case 568:
                                case 531:
                                case 532:
                                case 569:
                                case 571:
                                    num = 571;
                                    DataModul_Update();
                                    goto IL_6640;
                                case 709:
                                case 719:
                                case 725:
                                case 764:
                                case 765:
                                case 766:
                                case 856:
                                case 859:
                                case 862:
                                case 863:
                                case 872:
                                case 907:
                                case 908:
                                case 829:
                                case 832:
                                case 861:
                                case 906:
                                case 910:
                                case 913:
                                case 824:
                                case 825:
                                case 915:
                                case 656:
                                case 768:
                                case 946:
                                case 1080:
                                case 1130:
                                case 1131:
                                case 1132:
                                case 1133:
                                case 955:
                                case 956:
                                case 1141:
                                case 1295:
                                case 1315:
                                case 1316:
                                case 1323:
                                case 1324:
                                case 1325:
                                case 1338:
                                case 1339:
                                case 1340:
                                case 1361:
                                case 1362:
                                case 1181:
                                case 1182:
                                case 1363:
                                case 156:
                                case 226:
                                case 266:
                                case 291:
                                case 346:
                                case 367:
                                case 395:
                                case 399:
                                case 402:
                                case 403:
                                case 424:
                                case 447:
                                case 519:
                                case 537:
                                case 570:
                                case 584:
                                case 587:
                                case 590:
                                case 657:
                                case 659:
                                case 660:
                                case 841:
                                case 916:
                                case 1379:
                                    goto IL_6640;
                                case 1921:
                                case 1922:
                                case 1923:
                                case 1924:
                                case 2025:
                                case 2050:
                                case 2051:
                                case 2009:
                                case 2052:
                                case 2053:
                                case 2131:
                                case 2141:
                                case 2142:
                                case 2157:
                                case 2160:
                                case 2161:
                                case 2162:
                                case 2166:
                                case 2167:
                                case 2168:
                                case 2169:
                                case 2170:
                                case 1521:
                                case 1548:
                                case 2177:
                                case 2178:
                                case 1418:
                                case 1419:
                                case 2180:
                                case 661:
                                case 695:
                                case 769:
                                case 807:
                                case 917:
                                case 918:
                                case 919:
                                case 921:
                                case 924:
                                case 925:
                                case 928:
                                case 929:
                                case 932:
                                case 933:
                                case 936:
                                case 937:
                                case 943:
                                case 944:
                                case 945:
                                case 962:
                                case 965:
                                case 1171:
                                case 1188:
                                case 1191:
                                case 1374:
                                case 1377:
                                case 1378:
                                case 1487:
                                case 1490:
                                case 2181:
                                case 2185:
                                case 2188:
                                case 2198:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj2) when (obj2 is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj2, lErl);
                try0000_dispatch = 55682;
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
        void Handle_ListArt_112()
        {
            checked
            {
                int iAhn = 112;
                ProjectData.ClearProjectError();
                num3 = 3;
                if (M_Start == 0)
                {
                    M_Start = 1;
                }
                Anfang = "Nr.:" + ausw.bereich3.Text;
                IRecordset dT_AncesterTable = DataModul.DT_AncesterTable;
                dT_AncesterTable.Index = "Ahnen";
                dT_AncesterTable.MoveLast();
                if ((iAhn == 0) || (iAhn > dT_AncesterTable.Fields["Ahn"].AsInt()))
                {
                    iAhn = dT_AncesterTable.Fields["Ahn"].AsInt();
                }
                Aufhoer = "Nr.:" + iAhn;
                var key = (new string(' ', 40) + ausw.bereich3.Text).Right(40);
                dT_AncesterTable.Index = "Ahnen";
                dT_AncesterTable.Seek(">=", key);
                while (!dT_AncesterTable.EOF
                        && !(dT_AncesterTable.Fields["Ahn"].AsInt() > iAhn.AsDouble()))
                {
                    int Ancestor_iAhn = dT_AncesterTable.Fields["Ahn"].AsInt();
                    int Ancestor_iPerNr = dT_AncesterTable.Fields["Pernr"].AsInt();
                    if (Ancestor_iAhn > 2)
                    {
                        if (
                                        Ancestor_iAhn / 2 != Ancestor_iAhn / 2
                                        & (((int)d + 1) == Ancestor_iAhn))
                        {
                            dT_AncesterTable.MoveNext();
                            continue;
                        }
                    }
                    Bezeichnung5.Text = Ancestor_iPerNr.AsString();
                    Bezeichnung5.Refresh();
                    if (Ancestor_iPerNr != 0)
                    {
                        if (Hinter_List1_Items.Count > 0)
                        {
                            _Modul1.Instance.Datschalt = 2;
                            PerPos = 0;
                            _Modul1.Instance.Datles(Ancestor_iPerNr, out _);
                            if (_Modul1.Instance.Datschalt == 3)
                            {
                                _Modul1.Instance.Datschalt = 0;
                            }
                        }
                        string sSortText = Ancestor_iAhn.AsString().PadLeft(50);
                        DataModul_Sorttable_AddRow(Ancestor_iPerNr, sSortText);
                        d = dT_AncesterTable.Fields["Ahn"].AsDouble();
                    }
                    dT_AncesterTable.MoveNext();
                }
            }
        }
        void Handle_ListArt_135()
        {
            if (ausw.Label4.Text == "Sonstiges Ereignis")
            {
                Ereilist(MyProject.Forms.Ausw.List2.Items);
            }
            else
            {
                Beruflist();
            }
        }
        void Handle_ListArt_124(out int num3, out int num11, ref int lErl, ref double d, ref byte PerPos)
        {
            checked
            {
                num11 = _Modul1.Instance.Ubg1;
                _Modul1.Instance.Ubg1 = 0;
                ProjectData.ClearProjectError();
                num3 = 4;
                if (M_Start == 0)
                {
                    M_Start = 1;
                }
                Anfang = "Nr.:" + M_Start.AsString();
                IRecordset dT_AncesterTable = DataModul.DT_AncesterTable;
                dT_AncesterTable.Index = "Ahnen";
                dT_AncesterTable.MoveLast();
                if (Conversions.ToBoolean(num11 == 0
                                | (num11 > dT_AncesterTable.Fields["Ahn1"].AsInt())))
                {
                    num11 = dT_AncesterTable.Fields["Ahn1"].AsInt();
                }
                Aufhoer = "Nr.:" + num11.AsString();
                IRecordset NachKTabelle = DataModul.DT_DescendentTable;
                NachKTabelle.Index = "Ahnen";
                NachKTabelle.Seek(">=", 1);
                while (!NachKTabelle.EOF
                        && !(NachKTabelle.Fields["Nr"].AsInt() > num11))
                {
                    int NachKOmmen_iNr = NachKTabelle.Fields["Nr"].AsInt();
                    Interaction.MsgBox(NachKOmmen_iNr);
                    Bezeichnung5.Text = dT_AncesterTable.Fields["Pernr"].AsString();
                    Bezeichnung5.Refresh();
                    persInArb = dT_AncesterTable.Fields["Pernr"].AsInt();
                    if (persInArb == 0)
                    {
                        continue;
                    }
                    if (Hinter_List1_Items.Count > 0)
                    {
                        _Modul1.Instance.Datschalt = 2;
                        PerPos = 0;
                        _Modul1.Instance.Datles(persInArb, out _);
                        if (_Modul1.Instance.Datschalt == 3)
                        {
                            _Modul1.Instance.Datschalt = 0;
                        }
                    }
                    string sSuchText = Strings.Right("                                                   " + dT_AncesterTable.Fields["Ahn1"].AsString(), 50);
                    DataModul_Sorttable_AddRow(dT_AncesterTable.Fields["PerNr"].AsInt(), sSuchText);
                    d = dT_AncesterTable.Fields["Ahn1"].AsDouble();
                    lErl = 147;
                    dT_AncesterTable.MoveNext();
                }
            }
        }
        string Handle_ListArt_1101f(int iListArt, string inputStr)
        {
            int num9;
            int num10;
            checked
            {
                num9 = 0;
                num10 = (int)Math.Round(Conversion.Val(Conversion.Str(DateTime.Today.Year.ToString().AsDouble()) + "0000"));
                num10 += 10000;
                if (_Modul1.Instance.Ubg1 > 0)
                {
                    num10 = _Modul1.Instance.Ubg1 + 10000;
                }
                Anfang = " Jahr: " + Conversion.Str(Conversion.Val(Conversions.ToString(Conversions.ToDouble(Strings.Mid(M_Start.AsString(), 2, 4)) + 1.0)));
                if (M_Start > 0)
                {
                    num9 = M_Start - 1;
                }
                Aufhoer = " Jahr: " + Strings.Mid((num10 - 10000).AsString(), 2, 4);
                FillSortTable1(iListArt, (p) =>
                {
                    _Modul1.Instance.Datschalt = 10;
                    _Modul1.Instance.Datles1(p, out var asPersDates);
                    _Modul1.Instance.Datschalt = 0;
                    var text8 = "";
                    if (iListArt == 1101)
                    {
                        text8 = asPersDates[1];
                        inputStr = asPersDates[11];
                    }
                    if (iListArt == 1102)
                    {
                        text8 = asPersDates[2];
                        inputStr = asPersDates[12];
                    }
                    if (iListArt == 1103)
                    {
                        text8 = asPersDates[3];
                        inputStr = asPersDates[13];
                    }
                    if (iListArt == 1104)
                    {
                        text8 = asPersDates[4];
                        inputStr = asPersDates[14];
                    }
                    if (text8.TrimEnd() != "")
                    {
                        if ((inputStr.AsDouble() > num9) & (inputStr.AsDouble() < num10))
                        {
                            DataModul_Sorttable_AddRow(p, text8);
                        }
                    }
                }
                );
                return inputStr;
            }
        }
        static void GetListArtTexts(int iListArt, out string text3, out string text4)
        {
            text3 = "";
            text4 = "";
            if (iListArt == 101)
            {
                text4 = " Geburtsdatum";
                text3 = " Taufdatum";
            }
            else if (iListArt == 102)
            {
                text4 = " Taufdatum";
                text3 = " Geburtsdatum";
            }
            else if (iListArt == 103)
            {
                text4 = " Sterbedatum";
                text3 = " Begräbnisdatum";
            }
            else if (iListArt == 104)
            {
                text4 = " Begräbnisdatum";
                text3 = " Sterbedatum";
            }
            else if (iListArt == 119)
            {
                text4 = " Namen";
            }
            else if (iListArt == 120)
            {
                text4 = " Personennummern";
            }
            else if (iListArt == 121)
            {
                text4 = " Namen";
            }
            else if (iListArt == 122)
            {
                text4 = " Ahnennummern";
            }
            else if (iListArt == 123)
            {
                text4 = " Sippe";
            }
            else if (iListArt == 135)
            {
                text4 = " " + MyProject.Forms.Ausw.Label4.Text;
            }
            else if (iListArt == 401)
            {
                text4 = " Familiennummern";
            }
            else if (iListArt == 402)
            {
                text4 = " Name des Ehemannes";
            }
            else if (iListArt == 403)
            {
                text4 = " Name der Ehefrau";
            }
            else if (iListArt == 500)
            {
                text4 = " Proklamationsdatum";
            }
            else if (iListArt == 501)
            {
                text4 = " Verlobungsdatum";
            }
            else if (iListArt == 502)
            {
                text4 = " Heiratsdatum";
                text3 = " kirchl. Heiratsdatum";
            }
            else if (iListArt == 503)
            {
                text4 = " kirchl. Heiratsdatum";
                text3 = " Heiratsdatum";
            }
            else if (iListArt == 504)
            {
                text4 = " Scheidungsdatum";
            }
            else if (iListArt == 505)
            {
                text4 = " Datum für eheähnliche Beziehung";
                text3 = " Eheänliche Beziehung";
            }
            else if (iListArt == 1101)
            {
                text4 = " Geburtsregister";
            }
            else if (iListArt == 1102)
            {
                text4 = " Taufregister";
            }
            if (iListArt == 1103)
            {
                text4 = " Sterberegister";
            }
            else if (iListArt == 1104)
            {
                text4 = " Begräbnisregister";
            }
            else if (iListArt == 1201)
            {
                text4 = " Geburtsregister";
            }
            else if (iListArt == 1502)
            {
                text4 = " Heiratsregister";
            }
            else if (iListArt == 1503)
            {
                text4 = " kirchl. Heiratsregister";
            }
            else text4 = text4;
        }
        void Handle_ListArt_101f(int iListArt, byte b2, int num9, int num10)
        {
            checked
            {
                FillSortTable1(iListArt, (p) =>
                {
                    _Modul1.Instance.Datschalt = 1;
                    _Modul1.Instance.Datles(p, out var asPersDates);
                    _Modul1.Instance.Datschalt = 0;
                    var text8 = "";
                    if (iListArt == 101)
                    {
                        text8 = asPersDates[1];
                        if (unchecked(text8 == "" && b2 == 6))
                        {
                            text8 = asPersDates[2];
                        }
                    }
                    if (iListArt == 102)
                    {
                        text8 = asPersDates[2];
                        if (unchecked(text8 == "" && b2 == 6))
                        {
                            text8 = asPersDates[1];
                        }
                    }
                    if (iListArt == 103)
                    {
                        text8 = asPersDates[3];
                        if (unchecked(text8 == "" && b2 == 6))
                        {
                            text8 = asPersDates[4];
                        }
                    }
                    if (iListArt == 104)
                    {
                        text8 = asPersDates[4];
                        if (unchecked(text8 == "" && b2 == 6))
                        {
                            text8 = asPersDates[3];
                        }
                    }
                    if (text8.TrimEnd() != "")
                    {
                        if ((text8.AsDouble() > num9) & (text8.AsDouble() < num10))
                        {
                            DataModul.DSB_SortTable.AddNew();
                            DataModul.DSB_SortTable.Fields["Nr"].Value = p;
                            DataModul.DSB_SortTable.Fields["Sorttext"].Value = text8;
                            DataModul.DSB_SortTable.Update();
                        }
                    }
                }
                );
            }
        }
        void Handle_ListArt_123(ref int lErl, out Type typeFromHandle, out object[] array, out IField field, out object[] array2, out object[] arguments, out bool[] array3, out object obj, ref byte PerPos)
        {
            Anfang = _Modul1.Instance.UbgT.ToUpper().Trim();
            if (Anfang == "")
            {
                Anfang = "!";
            }
            if (_Modul1.Instance.Ubg1T == "")
            {
                _Modul1.Instance.Ubg1T = Strings.Chr(253).AsString();
            }
            Aufhoer = " " + _Modul1.Instance.Ubg1T.ToUpper();
            DataModul_Texte_ForeachDo(ref lErl, out typeFromHandle, out array, out field, out array2, out arguments, out array3, out obj, ref PerPos);
        }
        byte Handle_ListArt_120(out int num7, out int num11, out int num13, out int start, byte PerPos)
        {
            checked
            {
                num11 = _Modul1.Instance.Ubg1;
                Aufhoer = "Nr.:" + num11.AsString();
                _Modul1.Instance.Ubg1 = 0;
                var M_Start = (int)Math.Round(ausw.bereich3.Text.AsDouble());
                if (M_Start == 0)
                {
                    M_Start = 1;
                }
                Anfang = "Nr.:" + M_Start.AsString();
                DataModul.DB_PersonTable.MoveLast();
                int num12 = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                if (unchecked(num11 == 0 || num11 > num12))
                {
                    num11 = num12;
                }
                start = M_Start;
                num13 = num11;
                num7 = start;
                while (num7 <= num13)
                {
                    Bezeichnung5.Text = num7.AsString();
                    Bezeichnung5.Refresh();
                    persInArb = num7;
                    DataModul.Person.ReadData(num7, out var cPerson);
                    if (persInArb == num7)
                    {
                        if (Hinter_List1_Items.Count > 0)
                        {
                            _Modul1.Instance.Datschalt = 2;
                            PerPos = 0;
                            _Modul1.Instance.Datles(persInArb, out _);
                            if (_Modul1.Instance.Datschalt == 2)
                            {
                                _Modul1.Instance.Datschalt = 0;
                                num7++;
                                continue;
                            }
                            else if (_Modul1.Instance.Datschalt == 3)
                            {
                                _Modul1.Instance.Datschalt = 0;
                            }
                        }
                        string sSortText = "                                                   " + num7.AsString().Right(50);
                        DataModul_Sorttable_AddRow(num7, sSortText);
                    }
                    num7++;
                }
                return PerPos;
            }
        }
        void Handle_ListArt_119f(int iListArt, ref short num23, ref short Listart, ref byte PerPos)
        {
            checked
            {
                if (iListArt == 119)
                {
                    _Modul1.Instance.SortVND = true;
                }
                Anfang = " " + _Modul1.Instance.UbgT.ToUpper();
                Persuname = Anfang;
                Anfang = Persuname;
                if (_Modul1.Instance.Ubg1T == "")
                {
                    _Modul1.Instance.Ubg1T = "z";
                }
                Aufhoer = " " + _Modul1.Instance.Ubg1T.ToUpper();
                Persuname = Aufhoer;
                Aufhoer = Persuname.Trim();
                DataModul.DSB_SearchTable.Index = "Persuch";
                DataModul.DSB_SearchTable.Seek(">=", _Modul1.Instance.UbgT, 0);
                while (!DataModul.DSB_SearchTable.EOF)
                {
                    Persuname = DataModul.DSB_SearchTable.Fields["Name"].AsString();
                    if (Operators.CompareString(Anfang.ToUpper(), Strings.UCase(Persuname.Left(Anfang.Length)), TextCompare: false) > 0)
                    {
                        Debugger.Break();
                        DataModul.DSB_SearchTable.MoveNext();
                        continue;
                    }
                    else if (Operators.CompareString(Strings.UCase(Persuname.Left(Aufhoer.Length)), Aufhoer.ToUpper(), TextCompare: false) > 0 && Strings.Asc(Persuname.Left(1)) < 90) break;
                    Bezeichnung5.Text = DataModul.DSB_SearchTable.Fields["Name"].AsString();
                    Bezeichnung5.Refresh();
                    persInArb = DataModul.DSB_SearchTable.Fields["Nummer"].AsInt();
                    if (!_Modul1.Instance.SortVND)
                    {
                        num23 = (short)Strings.InStr(Persuname, ",");
                        if (num23 == 0)
                        {
                            num23 = 20;
                        }
                        Persuname = Persuname.Left(num23 - 1);
                    }
                    if (Hinter_List1_Items.Count > 0)
                    {
                        _Modul1.Instance.Datschalt = 2;
                        PerPos = 0;
                        _Modul1.Instance.Datles(persInArb, out _);
                        if (_Modul1.Instance.Datschalt == 3)
                        {
                            _Modul1.Instance.Datschalt = 0;
                        }
                        else
                        {
                            Listart = 300;
                            Berufles(ref Listart);
                            if (_Modul1.Instance.Datschalt == 3)
                            {
                                _Modul1.Instance.Datschalt = 0;
                            }
                            else
                            {
                                Listart = 301;
                                Berufles(ref Listart);
                                if (_Modul1.Instance.Datschalt != 3)
                                {
                                    Berufles(302);
                                    if (_Modul1.Instance.Datschalt == 2)
                                    {
                                        _Modul1.Instance.Datschalt = 0;
                                        DataModul.DSB_SearchTable.MoveNext();
                                        continue;
                                    }
                                    else if (_Modul1.Instance.Datschalt == 3)
                                    {
                                        _Modul1.Instance.Datschalt = 0;
                                    }
                                }
                                else
                                {
                                    _Modul1.Instance.Datschalt = 0;
                                }
                            }
                        }
                    }
                    _Modul1.Instance.Kont[1] = "";
                    _Modul1.Instance.Kont[2] = "";
                    _Modul1.Instance.Datschalt = 1;
                    _Modul1.Instance.Datles(persInArb, out var asPersDates);
                    if (asPersDates[1].Trim() == "")
                    {
                        asPersDates[1] = asPersDates[2];
                    }
                    _Modul1.Instance.Datschalt = 0;
                    DataModul_Sorttable_AddRow(persInArb, Persuname + asPersDates[1]);
                    DataModul.DSB_SearchTable.MoveNext();
                }
            }
        }
    }

    private void WriteFamilyList(ref int num, int iListArt, ref string text, ref int famInArb, ref byte b6, ref byte b10, ref int famInArb2, ref int persInArb, Ausw ausw, RichTextBox richTextBox1_0)
    {
        var ausw_opt3_0 = ausw.Option3[0].Checked;
        checked
        {
            if (iListArt > 1000)
            {
                // num8 = (short)(num8 - 1000);
                b10 = 1;
            }
            richTextBox1_0.SelectionIndent = 50;
            Write_Kopf(richTextBox1_0);
            DataModul.DSB_SortTable.Index = "Sort";
            DataModul.DSB_SortTable.Seek(">=", " ");
            IZahl = 0;
            while (!DataModul.DSB_SortTable.EOF)
            {
                IZahl++;
                _Modul1.Instance.Ind1 = IZahl.AsString();
                var M_Namen = "";
                if (DataModul.DSB_SortTable.EOF)
                {
                    richTextBox1_0.Visible = true;
                    return;
                }
                if (DataModul.DSB_SortTable.NoMatch)
                {
                    return;
                }
                _Modul1.Instance.FamInArb = DataModul.DSB_SortTable.Fields["Nr"].AsInt();
                richTextBox1_0.SelectedText = "\n";
                richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                richTextBox1_0.SelectedText = IZahl.AsString().Trim() + "\n";
                if (text != DataModul.DSB_SortTable.Fields["Sorttext"].AsString().Left(4))
                {
                    text = DataModul.DSB_SortTable.Fields["Sorttext"].AsString().Left(4);
                    richTextBox1_0.SelectedText = text.Trim() + "\n";
                }
                richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                if (unchecked(0 - (ausw.Kontroll[2].Checked ? 1 : 0)) == 1)
                {
                    Datmit = 1;
                }
                _Modul1.Instance.Famles();
                if ((_Modul1.Instance.Family.Mann > 0) | (_Modul1.Instance.Family.Frau > 0))
                {
                    _Modul1.Instance.Famdatles(_Modul1.Instance.FamInArb, out var asFamDates3);
                    if (asFamDates3[iListArt - 500] == "")
                    {
                        if (asFamDates3[iListArt - 500] == "")
                        {
                            asFamDates3[iListArt - 500] = asFamDates3[2];
                        }
                        if (asFamDates3[iListArt - 500] == "")
                        {
                            asFamDates3[iListArt - 500] = asFamDates3[3];
                        }
                        if (asFamDates3[iListArt - 500] == "")
                        {
                            asFamDates3[iListArt - 500] = asFamDates3[4];
                        }
                        if (asFamDates3[iListArt - 500] == "")
                        {
                            asFamDates3[iListArt - 500] = asFamDates3[1];
                        }
                        if (asFamDates3[iListArt - 500] == "")
                        {
                            asFamDates3[iListArt - 500] = asFamDates3[0];
                        }
                    }
                    if (b10 == 1)
                    {
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                        if (ausw_opt3_0)
                        {
                            richTextBox1_0.SelectedText = DataModul.DSB_SortTable.Fields["Sorttext"].AsString().Trim() + "\n";
                        }
                        else
                        {
                            richTextBox1_0.SelectedText = DataModul.DSB_SortTable.Fields["Sorttext"].AsString().Trim() + " ";
                        }
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                    }
                    else
                    {
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                        richTextBox1_0.SelectedText = asFamDates3[iListArt - 500];
                        switch (iListArt)
                        {
                            case 500:
                                richTextBox1_0.SelectedText = " werden Proklamiert: ";
                                break;
                            case 501:
                                richTextBox1_0.SelectedText = " verloben sich: ";
                                break;
                            case 502:
                                richTextBox1_0.SelectedText = " heiraten: ";
                                break;
                            case 503:
                                richTextBox1_0.SelectedText = " heiraten kirchlich: ";
                                break;
                            case 504:
                                richTextBox1_0.SelectedText = " werden geschieden: ";
                                break;
                            case 505:
                                richTextBox1_0.SelectedText = " leben zusammen: ";
                                break;
                            default:
                                break;
                        }
                    }
                    richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                    Bezeichnung5.Text = asFamDates3[iListArt - 500];
                    Bezeichnung5.Refresh();
                    persInArb = _Modul1.Instance.Family.Mann;
                    if (_Modul1.Instance.Family.Mann > 0)
                    {
                        Qu1 = (byte)ausw.Kontroll[5].CheckState;
                        Personles();
                    }
                    if ((_Modul1.Instance.Family.Mann > 0) & (_Modul1.Instance.Family.Frau > 0))
                    {
                        if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) == ".")
                        {
                            richTextBox1_0.SelectionStart = richTextBox1_0.SelectionStart - 1;
                            richTextBox1_0.SelectionLength = 2;
                            richTextBox1_0.SelectedText = "";
                        }
                        if (ausw_opt3_0)
                        {
                            richTextBox1_0.SelectedText = "\n und \n";
                        }
                        else richTextBox1_0.SelectedText = " und ";
                    }
                    persInArb = _Modul1.Instance.Family.Frau;
                    if (_Modul1.Instance.Family.Frau > 0)
                    {
                        Qu1 = (byte)ausw.Kontroll[5].CheckState;
                        Personles();
                    }
                }
                List1.Items.Clear();
                List2.Items.Clear();
                _Modul1.Instance.PersInArbsp = _Modul1.Instance.Family.Frau;
                _Modul1.Instance.Famdatles(_Modul1.Instance.FamInArb, out _);
                famInArb = _Modul1.Instance.FamInArb;
                if (ausw_opt3_0)
                {
                    richTextBox1_0.SelectedText = "\n";
                    richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                }
                else
                {
                    richTextBox1_0.SelectedText = " ";
                }
                Heidatsch();
                famInArb2 = _Modul1.Instance.FamInArb;
                b6 = 1;
                while (unchecked(b6) <= 2u)
                {
                    num = 1080;
                    if (b6 == 1)
                    {
                        persInArb = _Modul1.Instance.Family.Mann;
                    }
                    if (b6 == 2)
                    {
                        persInArb = _Modul1.Instance.PersInArbsp;
                    }
                    bool ausw_opt3_0 = ausw.Option3[0].Checked;
                    DataModul.Link.GetPersonFam(persInArb, ELinkKennz.lkChild, out _Modul1.Instance.FamInArb);
                    Datmit = 2;
                    _Modul1.Instance.Famles();
                    if ((_Modul1.Instance.Family.Mann > 0) | (_Modul1.Instance.Family.Frau > 0)
                            && _Modul1.Instance.Aus[51] == "Y")
                    {
                        if (b6 == 1)
                        {
                            if (ausw_opt3_0)
                            {
                                richTextBox1_0.SelectedText = "\n";
                            }
                            richTextBox1_0.SelectedText = "Seine Eltern: ";
                        }
                        _ = richTextBox1_0;
                        if (b6 == 2)
                        {
                            Font font = new(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            if (ausw_opt3_0)
                            {
                                richTextBox1_0.SelectedText = "\n";
                            }
                            else
                            {
                                richTextBox1_0.SelectionFont = font;
                                if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != ".")
                                {
                                    richTextBox1_0.SelectedText = ".";
                                }
                                richTextBox1_0.SelectedText = " ";
                            }
                            richTextBox1_0.SelectedText = "Ihre Eltern: ";
                        }
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        persInArb = _Modul1.Instance.Family.Mann;
                        if (_Modul1.Instance.Family.Mann > 0)
                        {
                            Qu1 = (byte)ausw.Kontroll[8].CheckState;
                            Personles();
                        }
                        if ((_Modul1.Instance.Family.Mann > 0) & (_Modul1.Instance.Family.Frau > 0))
                        {
                            if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) == ".")
                            {
                                richTextBox1_0.SelectionStart = richTextBox1_0.SelectionStart - 1;
                                richTextBox1_0.SelectionLength = 2;
                                richTextBox1_0.SelectedText = "";
                            }
                            richTextBox1_0.SelectedText = " und ";
                        }
                        persInArb = _Modul1.Instance.Family.Frau;
                        if (_Modul1.Instance.Family.Frau > 0)
                        {
                            Qu1 = (byte)ausw.Kontroll[8].CheckState;
                            Personles();
                        }
                    }
                    b6++;
                }
                _Modul1.Instance.FamInArb = famInArb2;
                if (ausw.Kontroll[1].Checked)
                {
                    _Modul1.Instance.FamInArb = DataModul.DSB_SortTable.Fields["Nr"].AsInt();
                    WriteKinder(richTextBox1_0);
                    if (Qu1 == 1) WriteFamQu(richTextBox1_0, _Modul1.Instance.FamInArb);
                    Qu1 = 0;
                }
                DataModul.DSB_SortTable.MoveNext();
            }
            richTextBox1_0.Visible = true;
            _Modul1.Instance.DAus[101] = _Modul1.Instance.Font1;
            _Modul1.Instance.DAus[102] = "10";
            richTextBox1_0.Visible = true;
            Befehl[0].Top = richTextBox1_0.Height + 70;
            Befehl[1].Top = richTextBox1_0.Height + 34;
            Befehl[2].Top = richTextBox1_0.Height + 70;
            Befehl[3].Top = richTextBox1_0.Height + 70;
            Befehl[4].Top = richTextBox1_0.Height + 34;
            Befehl[5].Top = richTextBox1_0.Height + 70;
            Befehl[6].Top = richTextBox1_0.Height + 34;
            Befehl[7].Top = richTextBox1_0.Height + 34;
            Befehl[8].Top = richTextBox1_0.Height + 34;
            Button2.Top = richTextBox1_0.Height + 34;
            Befehl[9].Top = richTextBox1_0.Height + 70;
            richTextBox1_0.SelectionIndent = 0;
            if (DataModul.DSB_QuellIdxTable.RecordCount > 0)
            {
                Befehl[8].Visible = true;
            }
            else
            {
                Befehl[8].Visible = false;
            }
            Befehl[0].Enabled = true;
            Befehl[1].Enabled = true;
            Befehl[3].Enabled = true;
            Befehl[4].Enabled = true;
            Befehl[6].Enabled = true;
            Befehl[7].Enabled = true;
            Befehl[8].Enabled = true;
        }
    }

    private void WriteList401(RichTextBox richTextBox1_0, Ausw ausw, ref int famInArb, ref int persInArb)
    {
        var ausw_opt3_0 = ausw.Option3[0].Checked;

        int num;
        _Modul1.Instance.DAus[101] = _Modul1.Instance.Font1;
        _Modul1.Instance.DAus[102] = "10";
        Write_Kopf(richTextBox1_0);
        DataModul.DSB_SortTable.Index = "Sort";
        DataModul.DSB_SortTable.Seek(">=", " ");
        IZahl = 0;
        richTextBox1_0.Visible = true;
        richTextBox1_0.Enabled = false;
        Application.DoEvents();
        while (!DataModul.DSB_SortTable.EOF)
        {
            Application.DoEvents();
            Bezeichnung5.Text = IZahl.AsString();
            Bezeichnung5.Refresh();
            if (DataModul.DSB_SortTable.EOF)
            {
                richTextBox1_0.Visible = true;
                return;
            }
            if (DataModul.DSB_SortTable.NoMatch)
            {
                return;
            }
            _Modul1.Instance.FamInArb = DataModul.DSB_SortTable.Fields["Nr"].AsInt();
            DataModul.DB_FamilyTable.Index = "Fam";
            DataModul.DB_FamilyTable.Seek("=", _Modul1.Instance.FamInArb);
            if (!DataModul.DB_FamilyTable.NoMatch)
            {
                IZahl = DataModul.DSB_SortTable.Fields["Nr"].AsInt();
                richTextBox1_0.SelectedText = "\n";
                richTextBox1_0.SelectionIndent = 20;
                richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                richTextBox1_0.SelectedText = IZahl.AsString().Trim() + "\n";
                richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                if (ausw.Kontroll[2].Checked)
                {
                    Datmit = 1;
                }
                _Modul1.Instance.Famles();
                if ((_Modul1.Instance.Family.Mann > 0) | (_Modul1.Instance.Family.Frau > 0))
                {
                    persInArb = _Modul1.Instance.Family.Mann;
                    if (_Modul1.Instance.Family.Mann > 0)
                    {
                        Qu1 = (byte)ausw.Kontroll[5].CheckState;
                        Personles();
                    }
                    if ((_Modul1.Instance.Family.Mann > 0) & (_Modul1.Instance.Family.Frau > 0))
                    {
                        richTextBox1_0.SelectedText = " ";
                    }
                    _Modul1.Instance.Famdatles(_Modul1.Instance.FamInArb, out var asFamDates2);
                    famInArb = _Modul1.Instance.FamInArb;
                    if (ausw.Kontroll[5].Checked)
                    {
                        var num7 = 20;
                        while (num7 <= 25)
                        {
                            var UbgT = asFamDates2[num7];
                            if (UbgT.Trim() != "")
                            {
                                UbgT = _Modul1.Instance.Retweg(UbgT);
                            }
                            asFamDates2[num7] = UbgT;
                            UbgT = "";
                            num7++;
                        }
                    }
                    if (ausw.Kontroll[3].Checked)
                    {
                        richTextBox1_0.SelectedText = " [" + _Modul1.Instance.FamInArb.AsString().Trim() + "] ";
                    }
                    if (asFamDates2[0] != "")
                    {
                        richTextBox1_0.SelectedText = _Modul1.Instance.DTxt[5] + " " + asFamDates2[0] + ". ";
                    }
                    if (asFamDates2[20] != "")
                    {
                        richTextBox1_0.SelectedText = " {" + asFamDates2[20].Trim() + "} ";
                    }
                    if (asFamDates2[1] != "")
                    {
                        richTextBox1_0.SelectedText = _Modul1.Instance.DTxt[6] + " " + asFamDates2[1] + ". ";
                    }
                    if (asFamDates2[21] != "")
                    {
                        richTextBox1_0.SelectedText = " {" + asFamDates2[21].Trim() + "} ";
                    }
                    if (asFamDates2[2] != "")
                    {
                        richTextBox1_0.SelectedText = _Modul1.Instance.DTxt[7] + " " + asFamDates2[2] + ". ";
                    }
                    if (asFamDates2[22] != "")
                    {
                        richTextBox1_0.SelectedText = " {" + asFamDates2[22].Trim() + "} ";
                    }
                    if (asFamDates2[3] != "")
                    {
                        richTextBox1_0.SelectedText = _Modul1.Instance.DTxt[8] + " " + asFamDates2[3] + ". ";
                    }
                    if (asFamDates2[23] != "")
                    {
                        richTextBox1_0.SelectedText = " {" + asFamDates2[23].Trim() + "} ";
                    }
                    if (asFamDates2[4] != "")
                    {
                        richTextBox1_0.SelectedText = _Modul1.Instance.DTxt[9] + " " + asFamDates2[4] + ". ";
                    }
                    if (asFamDates2[24] != "")
                    {
                        richTextBox1_0.SelectedText = " {" + asFamDates2[24].Trim() + "} ";
                    }
                    if (asFamDates2[5] != "")
                    {
                        richTextBox1_0.SelectedText = _Modul1.Instance.DTxt[10] + " " + asFamDates2[5] + ". ";
                    }
                    if (asFamDates2[25] != "")
                    {
                        richTextBox1_0.SelectedText = " {" + asFamDates2[25].Trim() + "} ";
                    }
                    var num7_ = 20;
                    while (num7_ <= 27)
                    {
                        asFamDates2[num7_] = "";
                        num7_++;
                    }
                    var Listart = 3;
                    _Modul1.Instance.FWohn(EEventArt.eA_602, ref Listart);
                    Famwohn(EEventArt.eA_602);
                    Listart = 3;
                    _Modul1.Instance.FWohn(EEventArt.eA_603, ref Listart);
                    Famwohn(EEventArt.eA_603);
                    persInArb = _Modul1.Instance.Family.Frau;
                    if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart - 1, 1) == ".")
                    {
                        richTextBox1_0.SelectionStart = richTextBox1_0.SelectionStart - 2;
                        richTextBox1_0.SelectionLength = 2;
                        richTextBox1_0.SelectedText = "";
                    }
                    richTextBox1_0.SelectedText = " mit ";
                    if (_Modul1.Instance.Family.Frau > 0)
                    {
                        Qu1 = (byte)ausw.Kontroll[5].CheckState;
                        Personles();
                    }
                }
                if (List1.Items.Count > 0)
                {
                    byte b4 = (byte)(List1.Items.Count - 1);
                    var b5 = 0;
                    while (unchecked(b5 <= (uint)b4))
                    {
                        List1.Items.RemoveAt(0);
                        b5 = (byte)unchecked((uint)(b5 + 1));
                    }
                }
                var b6 = 1;
                if (unchecked(b6) <= 2u)
                {
                    num = 1295;
                    if (b6 == 1)
                    {
                        persInArb = _Modul1.Instance.Family.Mann;
                    }
                    if (b6 == 2)
                    {
                        persInArb = _Modul1.Instance.Family.Frau;
                    }
                    DataModul.Link.GetPersonFam(persInArb, ELinkKennz.lkChild, out _Modul1.Instance.FamInArb);
                    _Modul1.Instance.Famles();
                    if ((_Modul1.Instance.Family.Mann > 0) | (_Modul1.Instance.Family.Frau > 0))
                    {
                        if (ausw_opt3_0)
                        {
                            richTextBox1_0.SelectedText = "\n";
                        }
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                        if (ausw_opt3_0)
                        {
                            if (b6 == 1)
                            {
                                richTextBox1_0.SelectedText = "Seine Eltern: ";
                            }
                            if (b6 == 2)
                            {
                                richTextBox1_0.SelectedText = "Ihre Eltern: ";
                            }
                        }
                        else if (b6 == 1)
                        {
                            richTextBox1_0.SelectedText = " Seine Eltern: ";
                        }
                        else if (b6 == 2)
                        {
                            richTextBox1_0.SelectedText = " Ihre Eltern: ";
                        }
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        persInArb = _Modul1.Instance.Family.Mann;
                        if (_Modul1.Instance.Family.Mann > 0)
                        {
                            Qu1 = (byte)ausw.Kontroll[8].CheckState;
                            Personles();
                        }
                        if ((_Modul1.Instance.Family.Mann > 0) & (_Modul1.Instance.Family.Frau > 0))
                        {
                            richTextBox1_0.SelectedText = " und ";
                        }
                        persInArb = _Modul1.Instance.Family.Frau;
                        if (_Modul1.Instance.Family.Frau > 0)
                        {
                            Qu1 = (byte)ausw.Kontroll[8].CheckState;
                            Personles();
                        }
                    }
                    b6 = (byte)unchecked((uint)(b6 + 1));
                }
                else if (ausw.Kontroll[1].Checked)
                {
                    _Modul1.Instance.FamInArb = DataModul.DSB_SortTable.Fields["Nr"].AsInt();
                    WriteKinder(richTextBox1_0);
                }
                if (_Modul1.Instance.Aus[4] == "Y")
                {
                    _Modul1.Instance.FamInArb = famInArb;
                    DataModul.DB_FamilyTable.Index = "Fam";
                    DataModul.DB_FamilyTable.Seek("=", _Modul1.Instance.FamInArb);
                    if (!DataModul.DB_FamilyTable.NoMatch)
                    {
                        if (DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].AsString() != " ")
                        {
                            var UbgT = "";
                            if (ausw.Kontroll[12].CheckState == CheckState.Unchecked)
                            {
                                UbgT = DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].AsString();
                                UbgT = _Modul1.Instance.RetWeg(UbgT);
                            }
                            richTextBox1_0.SelectedText = " {" + UbgT + "} ";
                        }
                    }
                }
                Qu1 = (byte)ausw.Kontroll[5].CheckState;
                if (Qu1 == 1) WriteFamQu(richTextBox1_0, _Modul1.Instance.FamInArb);
                Qu1 = 0;
            }
            DataModul.DSB_SortTable.MoveNext();
        }
        richTextBox1_0.Enabled = true;
        Befehl[0].Enabled = true;
        Befehl[1].Enabled = true;
        Befehl[3].Enabled = true;
        Befehl[4].Enabled = true;
        Befehl[6].Enabled = true;
        Befehl[7].Enabled = true;
        Befehl[8].Enabled = true;
        if (DataModul.DSB_QuellIdxTable.RecordCount > 0)
        {
            Befehl[8].Visible = true;
        }
        else
        {
            Befehl[8].Visible = false;
        }

    }

    private void WritePersonList(int iListArt, RichTextBox richTextBox1_0, int persInArb, int famInArb, Ausw ausw, ref string text5, ref short num24, ref byte PerPos)
    {
        var ausw_opt3_0 = ausw.Option3[0].Checked;
        long Ahne;
        _Modul1.Instance.DAus[101] = _Modul1.Instance.Font1;
        _Modul1.Instance.DAus[102] = "10";
        richTextBox1_0.Visible = true;
        richTextBox1_0.Enabled = false;
        Befehl[0].Top = richTextBox1_0.Height + 70;
        Befehl[1].Top = richTextBox1_0.Height + 34;
        Befehl[2].Top = richTextBox1_0.Height + 70;
        Befehl[3].Top = richTextBox1_0.Height + 70;
        Befehl[4].Top = richTextBox1_0.Height + 34;
        Befehl[5].Top = richTextBox1_0.Height + 70;
        Befehl[6].Top = richTextBox1_0.Height + 34;
        Befehl[7].Top = richTextBox1_0.Height + 34;
        Befehl[8].Top = richTextBox1_0.Height + 34;
        Befehl[9].Top = richTextBox1_0.Height + 70;
        Button2.Top = richTextBox1_0.Height + 34;
        richTextBox1_0.SelectionIndent = 0;
        if (Anfang == "!")
        {
            Anfang = "";
        }
        if (Operators.CompareString(Aufhoer.Trim(), Strings.Chr(253).AsString(), TextCompare: false) == 0)
        {
            Aufhoer = "";
        }
        Write_Kopf(richTextBox1_0);
        if (iListArt == 119)
        {
            richTextBox1_0.SelectedText = "\n";
        }
        DataModul.DSB_SortTable.Index = "Sort";
        DataModul.DSB_SortTable.Seek(">=", " ");
        IZahl = 0;
        if ((Listnam == 130) | (Listnam == 119))
        {
            Befehl[0].Visible = false;
            Befehl[6].Visible = false;
            Befehl[9].Visible = false;
            if (Listnam == 119)
            {
                Befehl[4].Visible = false;
            }
        }
        Bezeichnung1.Text = "";
        Visible = true;
        while (!DataModul.DSB_SortTable.EOF)
        {
            Bezeichnung1.Text += ">";
            if (Bezeichnung1.Text.Length == 75)
            {
                Bezeichnung1.Text = "";
            }
            Application.DoEvents();
            this.PerPos = 1;
            if (richTextBox1_0.SelectionStart > 50000)
            {
                richTextBox1_0.SelectedText = "\n";
                richTextBox1_0.SelectionIndent = 0;
                richTextBox1_0.SelectedText = "\n";
                var selectionLength = richTextBox1_0.SelectionStart;
                richTextBox1_0.SelectionStart = 0;
                richTextBox1_0.SelectionLength = selectionLength;
                num24 = (short)(num24 + 1);
                FileSystem.FileClose(99);
                var text9 = _Modul1.Instance.GenFreeDir + "Temp\\Sptext" + num24.AsString().Trim() + ".rtf";
                FileSystem.FileOpen(99, text9, OpenMode.Output);
                FileSystem.PrintLine(99, richTextBox1_0.SelectedRtf);
                FileSystem.FileClose(99);
                richTextBox1_0.Text = "";
                richTextBox1_0.SelectionAlignment = HorizontalAlignment.Left;
            }
            richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            if (DataModul.DSB_SortTable.EOF)
            {
                if (DataModul.DSB_QuellIdxTable.RecordCount > 0)
                {
                    Befehl[8].Visible = true;
                }
                else
                {
                    Befehl[8].Visible = false;
                }
                richTextBox1_0.SelectedText = "\n";
                richTextBox1_0.SelectionIndent = 0;
                richTextBox1_0.SelectedText = "\n";
                int selectionLength = richTextBox1_0.SelectionStart;
                richTextBox1_0.SelectionStart = 0;
                richTextBox1_0.SelectionLength = selectionLength;
                num24 = (short)(num24 + 1);
                FileSystem.FileClose(99);
                string text9 = _Modul1.Instance.GenFreeDir + "Temp\\Sptext" + num24.AsString().Trim() + ".rtf";
                FileSystem.FileOpen(99, text9, OpenMode.Output);
                FileSystem.PrintLine(99, richTextBox1_0.SelectedRtf);
                short num25 = num24;
                FileSystem.FileClose(99);
                richTextBox1_0.Text = "";
                int num26 = num25;
                var num7 = 1;
                while (num7 <= num26)
                {
                    Label2.Visible = true;
                    Label2.Text = "Lade Teil" + num7.AsString() + " von" + num25.AsString();
                    Application.DoEvents();
                    text9 = _Modul1.Instance.GenFreeDir + "Temp\\Sptext" + num7.AsString().Trim() + ".rtf";
                    RichTextBox1[3].LoadFile(text9);
                    FileSystem.Kill(text9);
                    selectionLength = RichTextBox1[3].Text.Length;
                    RichTextBox1[3].SelectionStart = 0;
                    RichTextBox1[3].SelectionLength = selectionLength;
                    richTextBox1_0.SelectedRtf = RichTextBox1[3].SelectedRtf;
                    RichTextBox1[3].Text = "";
                    num7++;
                }
                Label2.Visible = false;
                richTextBox1_0.Visible = true;
                richTextBox1_0.Enabled = true;
                Befehl[0].Enabled = true;
                Befehl[1].Enabled = true;
                Befehl[3].Enabled = true;
                Befehl[4].Enabled = true;
                Befehl[6].Enabled = true;
                Befehl[7].Enabled = true;
                Befehl[8].Enabled = true;
                return;
            }
            if (DataModul.DSB_SortTable.NoMatch)
            {
                return;
            }
            persInArb = DataModul.DSB_SortTable.Fields["Nr"].AsInt();
            _Modul1.Instance.Person_ReadNames(persInArb, _Modul1.Instance.Person);
            Bezeichnung8.Text = "Personen aufbereiten";
            if (iListArt == 122)
            {
                IZahl = Convert.ToInt32(DataModul.DSB_SortTable.Fields["Sorttext"].AsDouble());
            }
            else
            {
                IZahl++;
            }
            if (unchecked(iListArt != 119 && iListArt != 130))
            {
                richTextBox1_0.SelectedText = "\n";
                richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                richTextBox1_0.SelectionIndent = 5;
                richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                richTextBox1_0.SelectedText = IZahl.AsString().Trim() + "\n";
                richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                richTextBox1_0.SelectionIndent = 50;
            }
            if (iListArt is 101 or 102 or 103 or 104)
            {
                if (Operators.CompareString(text, Strings.Mid(DataModul.DSB_SortTable.Fields["sorttext"].AsString().Trim(), 1, 4), TextCompare: false) != 0)
                {
                    text = Strings.Mid(DataModul.DSB_SortTable.Fields["sorttext"].AsString().Trim(), 1, 4);
                    richTextBox1_0.SelectedText = Strings.Mid(DataModul.DSB_SortTable.Fields["sorttext"].AsString().Trim(), 1, 4) + "\n";
                }
            }
            else if (iListArt == 119)
            {
                richTextBox1_0.SelectedText = _Modul1.Instance.Kont[0] + ", " + _Modul1.Instance.Person.Givennames + "  " + persInArb.AsString() + "\n";
                DataModul.DSB_SortTable.MoveNext();
                continue;
            }
            else if (iListArt is not 120 and not 122 and not 401)
            {
                if (iListArt == 130)
                {
                    richTextBox1_0.SelectionHangingIndent = 30;
                    Ahne = IZahl;
                    _Modul1.Instance.Namenindex(Ahne);
                    richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                    richTextBox1_0.SelectedText = _Modul1.Instance.Kont[0] + ", " + _Modul1.Instance.Person.Givennames + " ";
                    PerPos = 0;
                    _Modul1.Instance.Datles(persInArb, out var asPersDates2);
                    if (asPersDates2[11].Trim() != "")
                    {
                        asPersDates2[11] = " " + _Modul1.Instance.DTxt[1] + " " + asPersDates2[11].Trim() + ".";
                    }
                    if (asPersDates2[12].Trim() != "")
                    {
                        asPersDates2[12] = " " + _Modul1.Instance.DTxt[2] + " " + asPersDates2[12].Trim() + ".";
                    }
                    if (asPersDates2[13].Trim() != "")
                    {
                        asPersDates2[13] = " " + _Modul1.Instance.DTxt[3] + " " + asPersDates2[13].Trim() + ".";
                    }
                    if (asPersDates2[14].Trim() != "")
                    {
                        asPersDates2[14] = " " + _Modul1.Instance.DTxt[4] + " " + asPersDates2[14].Trim() + ".";
                    }
                    if (iListArt == 101)
                    {
                        Bezeichnung5.Text = persInArb.AsString() + asPersDates2[11];
                        Bezeichnung5.Refresh();
                    }
                    richTextBox1_0.SelectedText = asPersDates2[11] + asPersDates2[12] + asPersDates2[13] + asPersDates2[14] + "\n";
                    DataModul.DSB_SortTable.MoveNext();
                    continue;
                }
                else if (iListArt == 123)
                {
                    if (text != _Modul1.Instance.Kont[5])
                    {
                        text = _Modul1.Instance.Kont[5];
                        richTextBox1_0.SelectedText = _Modul1.Instance.Kont[5] + "\n";
                    }
                }
                else if (iListArt is 121 or 135 or 403 or 402)
                {
                    if (text != _Modul1.Instance.Kont[0])
                    {
                        text = _Modul1.Instance.Kont[0];
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                        richTextBox1_0.SelectedText = _Modul1.Instance.Kont[0];
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        richTextBox1_0.SelectedText = "\n";
                    }
                }
                else if (iListArt is 1101 or 1102 or 1103 or 1104)
                {
                    if (text != DataModul.DSB_SortTable.Fields["sorttext"].AsString().Left(4))
                    {
                        text = DataModul.DSB_SortTable.Fields["sorttext"].AsString().Left(4);
                        richTextBox1_0.SelectedText = DataModul.DSB_SortTable.Fields["sorttext"].AsString().Left(4) + "\n";
                    }
                    richTextBox1_0.SelectedText = (DataModul.DSB_SortTable.Fields["sorttext"].Value + " ").AsString();
                }
                else if (iListArt is 1502 or 1503)
                {
                    if (text != DataModul.DSB_SortTable.Fields["sorttext"].AsString().Left(4))
                    {
                        text = DataModul.DSB_SortTable.Fields["sorttext"].AsString().Left(4);
                        richTextBox1_0.SelectedText = DataModul.DSB_SortTable.Fields["sorttext"].AsString().Left(4) + "\n";
                    }
                    richTextBox1_0.SelectedText = (DataModul.DSB_SortTable.Fields["sorttext"].Value + " ").AsString();
                }
                else if (iListArt != 135)
                {
                }
            }
            richTextBox1_0.SelectionIndent = 50;
            richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            if (ausw.Kontroll[3].Checked)
            {
                richTextBox1_0.SelectedText = "<" + persInArb.AsString().Trim() + "> ";
            }
            richTextBox1_0.SelectedText = _Modul1.Instance.Kont[3] + " ";
            if (_Modul1.Instance.Kont[1] != "")
            {
                richTextBox1_0.SelectedText = _Modul1.Instance.Kont[1] + " ";
            }
            Ahne = IZahl;
            _Modul1.Instance.Person_ReadNames(persInArb, _Modul1.Instance.Person);
            _Modul1.Instance.Person.SetFullSurname(_Modul1.Instance.BuildFullSurName(_Modul1.Instance.Person, _Modul1.Instance.DAus[89] == "1"));
            _Modul1.Instance.Namenindex(Ahne);
            DataModul_PerStatTab_Append(persInArb);
            Bezeichnung5.Text = _Modul1.Instance.Person.Givennames + " " + _Modul1.Instance.Person.SurName;
            Bezeichnung5.Refresh();
            richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
            richTextBox1_0.SelectedText = _Modul1.Instance.Person.SurName;
            richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            if (_Modul1.Instance.Kont[2].Trim() != "")
            {
                richTextBox1_0.SelectedText = " " + _Modul1.Instance.Kont[2];
            }
            if (_Modul1.Instance.Kont[4].Trim() != "")
            {
                richTextBox1_0.SelectedText = " (" + _Modul1.Instance.Kont[4] + ")";
            }
            if (_Modul1.Instance.Kont[5].Trim() != "")
            {
                richTextBox1_0.SelectedText = ", Sippe " + _Modul1.Instance.Kont[5];
            }
            WriteNachnr(richTextBox1_0);
            if (ausw_opt3_0)
            {
                richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            }
            _Modul1.Instance.PerSatzLes(persInArb);
            var left = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString();
            if (_Modul1.Instance.Aus[1] == "Y")
            {
                if (Operators.CompareString(DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                {
                    if (ausw_opt3_0)
                    {
                        richTextBox1_0.SelectedText = "\n";
                    }
                    richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                    if (ausw.Kontroll[12].Checked)
                    {
                        richTextBox1_0.SelectedText = (" {" + DataModul.DB_PersonTable.Fields[PersonFields.Bem1].Value + "}").AsString();
                    }
                    else
                    {
                        var UbgT = DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString();
                        UbgT = _Modul1.Instance.Retweg(UbgT);
                        richTextBox1_0.SelectedText = " {" + UbgT + "}";
                    }
                }
            }
            _Modul1.Instance.Datles(persInArb, out var asPersDates);
            if (asPersDates[11].Trim() != "")
            {
                asPersDates[11] = " " + _Modul1.Instance.DTxt[1] + " " + asPersDates[11].Trim() + ".";
            }
            if (asPersDates[12].Trim() != "")
            {
                asPersDates[12] = " " + _Modul1.Instance.DTxt[2] + " " + asPersDates[12].Trim() + ".";
            }
            if (asPersDates[13].Trim() != "")
            {
                asPersDates[13] = " " + _Modul1.Instance.DTxt[3] + " " + asPersDates[13].Trim() + ".";
            }
            if (asPersDates[14].Trim() != "")
            {
                asPersDates[14] = " " + _Modul1.Instance.DTxt[4] + " " + asPersDates[14].Trim() + ".";
            }
            if (iListArt == 101)
            {
                Bezeichnung5.Text = persInArb.AsString() + asPersDates[11];
                Bezeichnung5.Refresh();
            }
            asPersDates[11] = asPersDates[11] + asPersDates[16];
            asPersDates[12] = asPersDates[12] + asPersDates[17];
            asPersDates[13] = asPersDates[13] + asPersDates[18];
            asPersDates[14] = asPersDates[14] + asPersDates[19];
            if (ausw_opt3_0)
            {
                if (asPersDates[11].Trim() != "")
                {
                    richTextBox1_0.SelectedText = "\n" + asPersDates[11].Trim();
                }
                if (asPersDates[12].Trim() != "")
                {
                    richTextBox1_0.SelectedText = "\n" + asPersDates[12].Trim();
                }
                if (ausw.Kontroll[4].Checked)
                {
                    _Modul1.Instance.RegPaten();
                }
                _Modul1.Instance.Datles(persInArb, out asPersDates);
                if (asPersDates[13].Trim() != "")
                {
                    asPersDates[13] = " " + _Modul1.Instance.DTxt[3] + " " + asPersDates[13].Trim() + ".";
                }
                if (asPersDates[14].Trim() != "")
                {
                    asPersDates[14] = " " + _Modul1.Instance.DTxt[4] + " " + asPersDates[14].Trim() + ".";
                }
                asPersDates[13] = asPersDates[13] + asPersDates[18];
                asPersDates[14] = asPersDates[14] + asPersDates[19];
                if (asPersDates[13].Trim() != "")
                {
                    richTextBox1_0.SelectedText = "\n" + asPersDates[13].Trim();
                }
                if (_Modul1.Instance.Kont[14].Trim() != "")
                {
                    richTextBox1_0.SelectedText = "\n" + _Modul1.Instance.Kont[14].Trim();
                }
            }
            else
            {
                richTextBox1_0.SelectedText = asPersDates[11] + asPersDates[12];
                if (ausw.Kontroll[4].Checked)
                {
                    _Modul1.Instance.RegPaten();
                }
                _Modul1.Instance.Datles(persInArb, out asPersDates);
                if (asPersDates[13].Trim() != "")
                {
                    asPersDates[13] = " " + _Modul1.Instance.DTxt[3] + " " + asPersDates[13].Trim() + ".";
                }
                if (asPersDates[14].Trim() != "")
                {
                    asPersDates[14] = " " + _Modul1.Instance.DTxt[4] + " " + asPersDates[14].Trim() + ".";
                }
                asPersDates[13] = asPersDates[13] + asPersDates[18];
                asPersDates[14] = asPersDates[14] + asPersDates[19];
                richTextBox1_0.SelectedText = asPersDates[13] + asPersDates[14];
            }
            _Modul1.Instance.PersInArbsp = persInArb;
            Hinter_Anz_Texz = "";
            PerPos = this.PerPos;
            _Modul1.Instance.Erei(persInArb, EEventArt.eA_300, ref PerPos);
            if (Hinter_Anz_Texz != "")
            {
                if (ausw_opt3_0)
                {
                    leerweg(richTextBox1_0);
                    if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != "\n")
                    {
                        richTextBox1_0.SelectedText = "\n";
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                    }
                }
                if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) == "\n")
                {
                    richTextBox1_0.SelectedText = Hinter_Anz_Texz.Trim();
                }
                else
                {
                    leerweg(richTextBox1_0);
                    richTextBox1_0.SelectedText = " " + Hinter_Anz_Texz.Trim();
                }
            }
            Hinter_Anz_Texz = "";
            PerPos = this.PerPos;
            _Modul1.Instance.Erei(persInArb, EEventArt.eA_301, ref PerPos);
            if (Hinter_Anz_Texz != "")
            {
                if (ausw_opt3_0)
                {
                    leerweg(richTextBox1_0);
                    if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != "\n")
                    {
                        richTextBox1_0.SelectedText = "\n";
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                    }
                }
                if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) == "\n")
                {
                    richTextBox1_0.SelectedText = Hinter_Anz_Texz.Trim();
                }
                else
                {
                    leerweg(richTextBox1_0);
                    richTextBox1_0.SelectedText = " " + Hinter_Anz_Texz.Trim();
                }
            }
            Hinter_Anz_Texz = "";
            PerPos = this.PerPos;
            _Modul1.Instance.Erei(persInArb, EEventArt.eA_302, ref PerPos);
            if (Hinter_Anz_Texz != "")
            {
                if (ausw_opt3_0)
                {
                    leerweg(richTextBox1_0);
                    if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != "\n")
                    {
                        richTextBox1_0.SelectedText = "\n";
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                    }
                }
                if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) == "\n")
                {
                    richTextBox1_0.SelectedText = Hinter_Anz_Texz.Trim();
                }
                else
                {
                    leerweg(richTextBox1_0);
                    richTextBox1_0.SelectedText = " " + Hinter_Anz_Texz.Trim();
                }
            }
            Hinter_Anz_Texz = "";
            _Modul1.Instance.Erei(persInArb, EEventArt.eA_105, ref this.PerPos);
            if (Hinter_Anz_Texz != "")
            {
                leerweg(richTextBox1_0);
                if (ausw_opt3_0)
                {
                    if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != "\n")
                    {
                        richTextBox1_0.SelectedText = "\n";
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                    }
                }
                if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) == "\n")
                {
                    richTextBox1_0.SelectedText = Hinter_Anz_Texz.Trim();
                }
                else
                {
                    richTextBox1_0.SelectedText = " " + Hinter_Anz_Texz.Trim();
                }
            }
            Hinter_Anz_Texz = "";
            _Modul1.Instance.Erei(persInArb, EEventArt.eA_106, ref this.PerPos);
            if (Hinter_Anz_Texz != "")
            {
                leerweg(richTextBox1_0);
                if (ausw_opt3_0)
                {
                    if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != "\n")
                    {
                        richTextBox1_0.SelectedText = "\n";
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                    }
                }
                if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) == "\n")
                {
                    richTextBox1_0.SelectedText = "Heimatort: " + Hinter_Anz_Texz.Trim();
                }
                else richTextBox1_0.SelectedText = " Heimatort: " + Hinter_Anz_Texz.Trim();
            }
            if (ausw.Kontroll[5].Checked)
            {
                string UbgT = "";
                if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields[PersonFields.Bem3].Value)
                            && DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString().Length > 0)
                {
                    UbgT = DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString();
                }
                DataModul.DB_SourceLinkTable.Index = "Tab";
                DataModul.DB_SourceLinkTable.Seek("=", 1, persInArb);
                while (!DataModul.DB_SourceLinkTable.EOF
                            && !DataModul.DB_SourceLinkTable.NoMatch
                            && !Conversions.ToBoolean((DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._1].AsInt() != 1)
                                            | (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].AsInt() != persInArb)))
                {
                    DataModul.DB_QuTable.Index = "NR";
                    DataModul.DB_QuTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3]);
                    if (!DataModul.DB_QuTable.NoMatch)
                    {
                        if (UbgT.Trim() != "")
                        {
                            UbgT += "; ";
                        }
                        UbgT = (UbgT + DataModul.DB_QuTable.Fields[QuFields._2].AsString());
                        if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[3].Value) & (Operators.CompareString(DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim(), "", TextCompare: false) != 0))
                        {
                            if (Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value))
                            {
                                UbgT = (UbgT + " Seiten: " + DataModul.DB_SourceLinkTable.Fields[3].AsString());
                            }
                            else UbgT = Conversions.ToString(string.Concat(UbgT + " ", DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim()) + " " + DataModul.DB_SourceLinkTable.Fields[3].Value);
                        }
                        if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value) && Operators.CompareString(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString().Trim(), "", TextCompare: false) != 0)
                        {
                            UbgT = UbgT + " >" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString().Trim() + "<";
                        }
                        DataModul.DSB_QuellIdxTable.AddNew();
                        DataModul.DSB_QuellIdxTable.Fields["Quelle"].Value = DataModul.DB_QuTable.Fields[QuFields._4].Value;
                        DataModul.DSB_QuellIdxTable.Fields["Nr"].Value = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3].Value;
                        DataModul.DSB_QuellIdxTable.Update();
                    }
                    DataModul.DB_SourceLinkTable.MoveNext();
                }
                if (UbgT.Trim() != "")
                {
                    UbgT = _Modul1.Instance.Retweg(UbgT);
                    if (ausw_opt3_0)
                    {
                        if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != "\n")
                        {
                            richTextBox1_0.SelectedText = "\n";
                            richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        }
                    }
                    if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != "\n")
                    {
                        richTextBox1_0.SelectedText = " ";
                    }
                    richTextBox1_0.SelectedText = "Quellen: " + UbgT.Trim() + ". ";
                    UbgT = "";
                    if (ausw_opt3_0)
                    {
                        richTextBox1_0.SelectedText = "\n";
                    }
                }
            }
            if (ausw.Kontroll[4].Checked)
            {
                _Modul1.Instance.Pate_bei(persInArb);
            }
            DataModul.Link.GetPersonFam(persInArb, ELinkKennz.lkChild, out _Modul1.Instance.FamInArb);
            _Modul1.Instance.Famles();
            this.PerPos = 2;
            if (_Modul1.Instance.Aus[51] == "Y")
            {
                if ((_Modul1.Instance.Family.Mann > 0) | (_Modul1.Instance.Family.Frau > 0))
                {
                    if (ausw_opt3_0)
                    {
                        if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != "\n")
                        {
                            richTextBox1_0.SelectedText = "\n";
                        }
                    }
                    if (richTextBox1_0.Text.Right(1) != "\n")
                    {
                        if (richTextBox1_0.Text.Right(1) != " ")
                        {
                            richTextBox1_0.SelectedText = " ";
                        }
                    }
                    if (ausw_opt3_0)
                    {
                        if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != "\n")
                        {
                            richTextBox1_0.SelectedText = "\n";
                        }
                    }
                    else
                    {
                        if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) == " ")
                        {
                            richTextBox1_0.SelectionStart = richTextBox1_0.SelectionStart - 1;
                            richTextBox1_0.SelectionLength = 1;
                            richTextBox1_0.SelectedText = "";
                        }
                        if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != ".")
                        {
                            richTextBox1_0.SelectedText = ".";
                        }
                        if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != " ")
                        {
                            richTextBox1_0.SelectedText = " ";
                        }
                    }
                    persInArb = _Modul1.Instance.Family.Mann;
                    if (_Modul1.Instance.Family.Mann > 0)
                    {
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                        richTextBox1_0.SelectedText = "Vater: ";
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        Qu1 = (byte)ausw.Kontroll[8].CheckState;
                        Personles();
                    }
                    persInArb = _Modul1.Instance.Family.Frau;
                    if (_Modul1.Instance.Family.Frau > 0)
                    {
                        if (ausw_opt3_0)
                        {
                            if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != "\n")
                            {
                                richTextBox1_0.SelectedText = "\n";
                            }
                        }
                        else
                        {
                            if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) == " ")
                            {
                                richTextBox1_0.SelectionStart = richTextBox1_0.SelectionStart - 1;
                                richTextBox1_0.SelectionLength = 1;
                                richTextBox1_0.SelectedText = "";
                            }
                            if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != ".")
                            {
                                richTextBox1_0.SelectedText = ".";
                            }
                            if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != " ")
                            {
                                richTextBox1_0.SelectedText = " ";
                            }
                        }
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                        richTextBox1_0.SelectedText = "Mutter: ";
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        Qu1 = (byte)ausw.Kontroll[8].CheckState;
                        Personles();
                    }
                }
            }
            List1.Items.Clear();
            persInArb = _Modul1.Instance.PersInArbsp;
            _Modul1.Instance.PerSatzLes(persInArb);
            if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "M")
            {
                _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                flag = false;
            }
            if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
            {
                _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                flag = true;
            }
            var aiFams2 = DataModul.Link.GetPersonFams(persInArb, _Modul1.Instance.eLKennz);
            text5 = _Modul1.Instance.UbgT;
            int num28 = aiFams2.Count;
            var num7 = 1;
            foreach (var iFam in aiFams2)
            {
                _Modul1.Instance.FamInArb = iFam;
                _Modul1.Instance.Schalt = 1;
                _Modul1.Instance.Famdatles(_Modul1.Instance.FamInArb, out var asFamDates2);
                _Modul1.Instance.Schalt = 0;
                if (asFamDates2[2].Trim() == "")
                {
                    asFamDates2[2] = asFamDates2[3];
                }
                if (asFamDates2[2].Trim() == "")
                {
                    asFamDates2[2] = asFamDates2[4];
                }
                if (asFamDates2[2].Trim() == "")
                {
                    asFamDates2[2] = asFamDates2[1];
                }
                if (asFamDates2[2].Trim() == "")
                {
                    asFamDates2[2] = asFamDates2[0];
                }
                if (asFamDates2[2].Trim() == "")
                {
                    asFamDates2[2] = _Modul1.Instance.Person.Prae;
                }
                if (asFamDates2[2].Trim() == "")
                {
                    asFamDates2[2] = "        ";
                }
                if (asFamDates2[2].Trim().Length > 8)
                {
                    Debugger.Break();
                }
                List1.Items.Add(asFamDates2[2] + text5.Left(10));
            }
            DataModul.DSB_PerStatTable.Index = "Per";
            DataModul.DSB_PerStatTable.Seek("=", persInArb);
            var num30 = 0;
            while (num30 < List1.Items.Count)
            {
                if (_Modul1.Instance.Aus[52] == "Y")
                {
                    _Modul1.Instance.FamInArb = Conversions.ToInteger(Strings.Mid(List1.Items[num30].AsString(), 9, 20));
                    famInArb = _Modul1.Instance.FamInArb;
                    _Modul1.Instance.Famdatles(_Modul1.Instance.FamInArb, out _);
                    if (List1.Items.Count == 1)
                    {
                        leerweg(richTextBox1_0);
                        if (ausw_opt3_0)
                        {
                            if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != "\n")
                            {
                                richTextBox1_0.SelectedText = "\n";
                            }
                        }
                        if (richTextBox1_0.Text.Right(1) != "\n")
                        {
                            if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) == ".") richTextBox1_0.SelectedText = " ";
                            else
                            {
                                richTextBox1_0.SelectedText = ". ";
                            }
                        }
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        leerweg(richTextBox1_0);
                        if (richTextBox1_0.Text.Right(1) != "\n")
                        {
                            if (richTextBox1_0.Text.Right(1) != " ")
                            {
                                richTextBox1_0.SelectedText = " ";
                            }
                        }
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        if (_Modul1.Instance.DTxt[11] != "")
                        {
                            richTextBox1_0.SelectedText = _Modul1.Instance.DTxt[11] + " ";
                        }
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        if (ausw_opt3_0)
                        {
                            if (richTextBox1_0.Text.Right(1) != "\n")
                            {
                                richTextBox1_0.SelectedText = "\n";
                            }
                        }
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                    }
                    else
                    {
                        if (ausw.Kontroll[3].Checked)
                        {
                            if (ausw_opt3_0)
                            {
                                richTextBox1_0.SelectedText = "\n";
                            }
                            if (richTextBox1_0.Text.Right(1) != "\n")
                            {
                                if (richTextBox1_0.Text.Right(1) != " ")
                                {
                                    richTextBox1_0.SelectedText = " ";
                                }
                            }
                            richTextBox1_0.SelectedText = (num30 + 1.AsString()).Trim() + ". " + _Modul1.Instance.DTxt[11] + " ";
                            if (ausw_opt3_0)
                            {
                                richTextBox1_0.SelectedText = "\n";
                            }
                            richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        }
                        else
                        {
                            if (ausw_opt3_0)
                            {
                                richTextBox1_0.SelectedText = "\n";
                            }
                            if (richTextBox1_0.Text.Right(1) != "\n")
                            {
                                if (richTextBox1_0.Text.Right(1) != " ")
                                {
                                    richTextBox1_0.SelectedText = " ";
                                }
                            }
                            richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            b5 = 1;
                            while (unchecked(b5) <= 10u
                                    && Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) == " ")
                            {
                                richTextBox1_0.SelectionStart = richTextBox1_0.SelectionStart - 1;
                                richTextBox1_0.SelectionLength = 1;
                                richTextBox1_0.SelectedText = "";
                                b5++;
                            }
                            if (richTextBox1_0.Text.Right(1) != "\n")
                            {
                                richTextBox1_0.SelectedText = " ";
                            }
                            richTextBox1_0.SelectedText = (num30 + 1.AsString()).Trim() + ". " + _Modul1.Instance.DTxt[11] + " ";
                        }
                        richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                    }
                    Heidatsch();
                    var b9 = 0;
                    if (_Modul1.Instance.Aus[4] == "Y")
                    {
                        DataModul.DB_FamilyTable.Index = "Fam";
                        DataModul.DB_FamilyTable.Seek("=", _Modul1.Instance.FamInArb);
                        if (DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].AsString() != " ")
                        {
                            if (ausw.Kontroll[12].Checked)
                            {
                                if (ausw_opt3_0)
                                {
                                    richTextBox1_0.SelectedText = "\n";
                                }
                                richTextBox1_0.SelectedText = (" {" + DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].Value + "}").AsString();
                            }
                            else
                            {
                                var UbgT = DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].AsString();
                                UbgT = _Modul1.Instance.Retweg(UbgT);
                                if (ausw_opt3_0)
                                {
                                    richTextBox1_0.SelectedText = "\n";
                                }
                                richTextBox1_0.SelectedText = " {" + UbgT + "}";
                                b9 = 1;
                            }
                        }
                    }
                    if (b9 != 1)
                    {
                        if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart - 1, 1) == ".")
                        {
                            richTextBox1_0.SelectionStart = richTextBox1_0.SelectionStart - 2;
                            richTextBox1_0.SelectionLength = 2;
                            richTextBox1_0.SelectedText = "";
                        }
                        if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != "\n")
                        {
                            richTextBox1_0.SelectedText = " ";
                        }
                        leerweg(richTextBox1_0);
                        richTextBox1_0.SelectedText = " mit ";
                    }
                    else
                    {
                        richTextBox1_0.SelectedText = " Mit ";
                    }
                    b9 = 0;
                    if (ausw_opt3_0)
                    {
                        richTextBox1_0.SelectedText = "\n";
                    }
                    _Modul1.Instance.Famles();
                    if (left == "M")
                    {
                        persInArb = _Modul1.Instance.Family.Frau;
                    }
                    if (left == "F")
                    {
                        persInArb = _Modul1.Instance.Family.Mann;
                    }
                    if (persInArb == 0)
                    {
                        leerweg(richTextBox1_0);
                        richTextBox1_0.SelectedText = " unbekanntem Partner.";
                    }
                    else
                    {
                        Qu1 = (byte)ausw.Kontroll[8].CheckState;
                        Personles();
                    }
                    DataModul.Link.GetPersonFam(persInArb, ELinkKennz.lkChild, out _Modul1.Instance.FamInArb);
                    if (_Modul1.Instance.Aus[51] == "Y")
                    {
                        _Modul1.Instance.Famles();
                        if ((_Modul1.Instance.Family.Mann > 0) | (_Modul1.Instance.Family.Frau > 0))
                        {
                            if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != ".")
                            {
                                richTextBox1_0.SelectedText = ".";
                            }
                            if (ausw_opt3_0)
                            {
                                if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != "\n")
                                {
                                    richTextBox1_0.SelectedText = "\n";
                                    richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                }
                                if (flag)
                                {
                                    richTextBox1_0.SelectedText = "Dessen";
                                }
                                if (!flag)
                                {
                                    richTextBox1_0.SelectedText = "Deren";
                                }
                                richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                richTextBox1_0.SelectedText = " Eltern: ";
                            }
                            else
                            {
                                if (flag)
                                {
                                    richTextBox1_0.SelectedText = " Dessen";
                                }
                                if (!flag)
                                {
                                    richTextBox1_0.SelectedText = " Deren";
                                }
                                richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                richTextBox1_0.SelectedText = " Eltern: ";
                            }
                            richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            persInArb = _Modul1.Instance.Family.Mann;
                            if (_Modul1.Instance.Family.Mann > 0)
                            {
                                Qu1 = (byte)ausw.Kontroll[8].CheckState;
                                Personles();
                            }
                            if ((_Modul1.Instance.Family.Mann > 0) & (_Modul1.Instance.Family.Frau > 0))
                            {
                                if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) == ".")
                                {
                                    richTextBox1_0.SelectionStart = richTextBox1_0.SelectionStart - 1;
                                    richTextBox1_0.SelectionLength = 1;
                                    richTextBox1_0.SelectedText = "";
                                }
                                richTextBox1_0.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                if (ausw_opt3_0)
                                {
                                    richTextBox1_0.SelectedText = "\nund\n";
                                }
                                else richTextBox1_0.SelectedText = " und ";
                            }
                            persInArb = _Modul1.Instance.Family.Frau;
                            if (_Modul1.Instance.Family.Frau > 0)
                            {
                                Qu1 = (byte)ausw.Kontroll[8].CheckState;
                                Personles();
                            }
                        }
                    }
                }
                num30++;
            }
            if (List1.Items.Count > 0)
            {
                byte b8 = (byte)(List1.Items.Count - 1);
                Fk = 0;
                while (unchecked(Fk <= (uint)b8))
                {
                    _Modul1.Instance.FamInArb = Conversions.ToInteger(Strings.Mid(List1.Items[Fk].AsString(), 9, 20));
                    WriteKinder(richTextBox1_0);
                    if (Qu1 == 1) WriteFamQu(richTextBox1_0, _Modul1.Instance.FamInArb);
                    Qu1 = 0;
                    Fk++;
                }
            }
            DataModul.DSB_SortTable.MoveNext();
        }
    }
    

    private void DataModul_Sorttable_AddRow(int persInArb, string sSorttext)
    {
        DataModul.DSB_SortTable.AddNew();
        DataModul.DSB_SortTable.Fields["Nr"].Value = persInArb;
        DataModul.DSB_SortTable.Fields["Sorttext"].Value = sSorttext;
        DataModul.DSB_SortTable.Update();
    }

    private void DataModul_Texte_ForeachDo(ref int lErl, out Type typeFromHandle, out object[] array, out IField field, out object[] array2, out object[] arguments, out bool[] array3, out object obj, ref byte PerPos)
    {
        checked
        {
            IRecordset dB_TexteTable = DataModul.DB_TexteTable;
            dB_TexteTable.Index = "ITexte";
            dB_TexteTable.Seek(">=", Anfang, "D");
            while (!dB_TexteTable.EOF
                && dB_TexteTable.Fields["Kennz"].AsString() == "D")
            {
                Bezeichnung5.Text = dB_TexteTable.Fields["Txt"].AsString();
                Bezeichnung5.Refresh();
                typeFromHandle = typeof(Strings);
                array = new object[1];
                field = dB_TexteTable.Fields["Txt"];
                array[0] = field.Value;
                array2 = array;
                arguments = array2;
                array3 = [true];
                obj = NewLateBinding.LateGet(null, typeFromHandle, "UCase", arguments, null, null, array3);
                if (array3[0])
                {
                    field.Value = array2[0];
                }
                if (Operators.CompareString(obj.AsString().Left(Aufhoer.Trim().Length), Aufhoer.Trim(), TextCompare: false) <= 0)
                {
                    DataModul.DB_NameTable.Index = "TxNr";
                    DataModul.DB_NameTable.Seek("=", dB_TexteTable.Fields["Txnr"]);
                    while (!DataModul.DB_NameTable.EOF
                    && !DataModul.DB_NameTable.NoMatch
                    && !(DataModul.DB_NameTable.Fields[NameFields.Text].AsInt() > dB_TexteTable.Fields["Txnr"].AsInt()))
                    {
                        if (Hinter_List1_Items.Count > 0)
                        {
                            _Modul1.Instance.PersInArb = DataModul.DB_NameTable.Fields[NameFields.PersNr].AsInt();
                            _Modul1.Instance.Datschalt = 2;
                            PerPos = 0;
                            _Modul1.Instance.Datles(_Modul1.Instance.PersInArb, out _);
                            if (_Modul1.Instance.Datschalt == 2)
                            {
                                _Modul1.Instance.Datschalt = 0;
                                DataModul.DB_NameTable.MoveNext();
                                continue;
                            }
                            else if (_Modul1.Instance.Datschalt == 3)
                            {
                                _Modul1.Instance.Datschalt = 0;
                            }
                        }
                        DataModul.DSB_SortTable.AddNew();
                        DataModul.DSB_SortTable.Fields["Nr"].Value = DataModul.DB_NameTable.Fields[NameFields.PersNr].Value;
                        DataModul.DSB_SortTable.Fields["Sorttext"].Value = Strings.Left((dB_TexteTable.Fields["Txt"].Value + "                                                   ").AsString(), 20);
                        DataModul.DSB_SortTable.Update();
                        lErl = 178;
                        DataModul.DB_NameTable.MoveNext();
                    }
                }
                dB_TexteTable.MoveNext();
            }
        }
    }

    private static void DataModul_Update()
    {
        IRecordset SortTable = DataModul.DSB_SortTable;
        SortTable.Index = "Nr";
        SortTable.Seek(">=", 0);
        while (!SortTable.EOF)
        {
            _Modul1.Instance.Person_ReadNames(SortTable.Fields["Nr"].AsInt(), _Modul1.Instance.Person);
            var Namen = SortTable.Fields["Sorttext"].AsString() + _Modul1.Instance.Person.SurName;

            SortTable.Edit();
            SortTable.Fields["Sorttext"].Value = Namen;
            SortTable.Update();
            SortTable.MoveNext();
        }
    }

    private void FillSortTable1(int num8, Action<int> aAppend)
    {
        DataModul.DB_PersonTable.MoveLast();
        var num18 = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
        var num7 = 1;
        while (num7 <= num18)
        {
            int persInArb = num7;
            Bezeichnung5.Text = num7.AsString();
            Bezeichnung5.Refresh();
            if (Hinter_List1_Items.Count > 0)
            {
                _Modul1.Instance.Datschalt = 2;
                _Modul1.Instance.Datles(persInArb, out _);
                if (_Modul1.Instance.Datschalt == 2)
                {
                    _Modul1.Instance.Datschalt = 0;
                    num7++;
                    continue;
                }
                if (_Modul1.Instance.Datschalt == 3)
                {
                    _Modul1.Instance.Datschalt = 0;
                }
            }
            int num19;
            if (Hinter_List2_Items.Count > 0)
            {
                DataModul.DB_EventTable.Index = "ArtNr";
                DataModul.DB_EventTable.Seek("=", num8 % 1000, persInArb, 0);
                if (!DataModul.DB_EventTable.NoMatch)
                {
                    num19 = Hinter_List2_Items.Count - 1;
                    var num4 = 0;
                    while (num4 <= num19)
                    {
                        var Kol = Hinter_List2_Items[num4].ItemData<int>();
                        if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() == Kol)
                        {
                            break;
                        }
                        num4++;
                    }
                    if (num4 > num19)
                    {
                        num7++;
                        continue;
                    }
                }
                else
                {
                    num7++;
                    continue;
                }
            }
            aAppend(persInArb);
            num7++;
        }

    }


    public void WriteNachnr(RichTextBox richTextBox1_0)
    {
        Ausw ausw = MyProject.Forms.Ausw;
        Font fntUnderline = new(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Underline);
        Font fntRegular = new(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
        int persInArb = _Modul1.Instance.PersInArb;
        bool xAusw_10 = ausw.Kontroll[10].Checked;
        bool xAusw_11 = ausw.Kontroll[11].Checked;

        if (xAusw_10 || xAusw_11)
        {
            _Modul1.Instance.Ahnles(persInArb, out var asAhnData);

            if (xAusw_11)
            {
                if (asAhnData[11].Trim() != "")
                {
                    richTextBox1_0.SelectedText = " ";
                }
                richTextBox1_0.SelectionFont = fntUnderline;
                if (asAhnData[11].Trim() != "")
                {
                    richTextBox1_0.SelectedText = "Ahnen-Nr.: " + asAhnData[11].Trim() + ".";
                }
                richTextBox1_0.SelectionFont = fntRegular;
            }
            richTextBox1_0.SelectionFont = fntRegular;
            if (xAusw_10)
            {
                if (asAhnData[13].Trim() != "")
                {
                    richTextBox1_0.SelectedText = " ";
                }
                richTextBox1_0.SelectionFont = fntUnderline;
                if (asAhnData[13].Trim() != "")
                {
                    richTextBox1_0.SelectedText = asAhnData[13];
                }
            }
        }
        richTextBox1_0.SelectionFont = fntRegular;
    }

    public void WriteFamQu(RichTextBox richTextBox1_0, int famInArb)
    {
        Font fntRegular = new(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);

        string Family_sBem3 = DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].AsString();
        string text = "";
        richTextBox1_0.SelectionFont = fntRegular;
        if (Family_sBem3.Length > 0)
        {
            text = Family_sBem3;
        }

        IRecordset dB_SourceLinkTable = DataModul.DB_SourceLinkTable;
        dB_SourceLinkTable.Index = "Tab";
        dB_SourceLinkTable.Seek("=", 2, famInArb);
        while (!dB_SourceLinkTable.EOF
            && !dB_SourceLinkTable.NoMatch)
        {
            int SourceLink_iField1 = dB_SourceLinkTable.Fields["1"].AsInt();
            int SourceLink_iField2 = dB_SourceLinkTable.Fields["2"].AsInt();
            int SourceLink_iField3 = dB_SourceLinkTable.Fields["3"].AsInt();
            string SourceLink_sField4 = dB_SourceLinkTable.Fields["4"].AsString();
            string SourceFields_sOrig = dB_SourceLinkTable.Fields["Orig"].AsString();
            string SourceLink_sAus = dB_SourceLinkTable.Fields["Aus"].AsString();

            if (Conversions.ToBoolean(SourceLink_iField1 != 2 | SourceLink_iField2 != famInArb))
            {
                break;
            }
            DataModul.DB_QuTable.Index = "NR";
            DataModul.DB_QuTable.Seek("=", SourceLink_iField3);
            string Quelle_sField2 = DataModul.DB_QuTable.Fields[QuFields._2].AsString();
            string Quelle_sTitle = DataModul.DB_QuTable.Fields[QuFields._4].AsString();

            text += "; " + Quelle_sField2;
            if (SourceLink_sField4.Trim() != "")
            {
                text += (SourceLink_sAus != "")
                    ? $" {SourceLink_sAus.Trim()} {SourceLink_sField4}"
                    : $" Seiten: {SourceLink_sField4}";
            }
            if (SourceFields_sOrig.Trim() != "")
            {
                text += $" >{SourceFields_sOrig.Trim()}<";
            }
            DataModul_Quellen_AddRaw(Quelle_sTitle, SourceLink_iField3);

            dB_SourceLinkTable.MoveNext();
        }
        if (text.Trim() != "")
        {
            richTextBox1_0.SelectedText = "\n Quellen zur Familie: " + text.Trim() + ".\n";
        }
    }
    public void Heidatsch()
    {

        var flag = false;
        Ausw ausw = MyProject.Forms.Ausw;
        RichTextBox richTextBox0 = RichTextBox1[0];
        int famInArb = _Modul1.Instance.FamInArb;
        bool xEmitWitness = ausw.Kontroll[4].Checked;
        bool xEmitSource = ausw.Kontroll[5].Checked;
        string asFamData = _Modul1.Instance.Kont[0];

        if (ausw.Kontroll[3].Checked)
        {
            richTextBox0.SelectedText = $" [{famInArb}] ";
        }
        if (asFamData.Trim() != "")
        {
            richTextBox0.SelectedText = _Modul1.Instance.DTxt[5] + " " + asFamData + ". ";
            flag = true;
        }
        if (_Modul1.Instance.Kont[20].Trim() != "")
        {
            richTextBox0.SelectedText = " {" + _Modul1.Instance.Kont[20].Trim() + "} ";
        }
        if (xEmitWitness && _Modul1.Instance.Kont[40].Trim() != "")
        {
            richTextBox0.SelectedText = " Zeugen: " + _Modul1.Instance.Kont[40].Trim() + " ";
        }
        if (xEmitSource)
        {
            FamDatQuelle(EEventArt.eA_500, famInArb, richTextBox0);
        }
        if (_Modul1.Instance.Kont[1].Trim() != "")
        {
            richTextBox0.SelectedText = _Modul1.Instance.DTxt[6] + " " + _Modul1.Instance.Kont[1] + ". ";
            flag = true;
        }
        if (_Modul1.Instance.Kont[21].Trim() != "")
        {
            richTextBox0.SelectedText = " {" + _Modul1.Instance.Kont[21].Trim() + "} ";
        }
        if (xEmitWitness && _Modul1.Instance.Kont[41].Trim() != "")
        {
            richTextBox0.SelectedText = " Zeugen: " + _Modul1.Instance.Kont[41].Trim() + " ";
        }
        if (xEmitSource)
        {
            FamDatQuelle(EEventArt.eA_501, famInArb, richTextBox0);
        }
        if (_Modul1.Instance.Kont[2].Trim() != "")
        {
            richTextBox0.SelectedText = _Modul1.Instance.DTxt[7] + " " + _Modul1.Instance.Kont[2] + ". ";
            flag = true;
        }
        if (_Modul1.Instance.Kont[22].Trim() != "")
        {
            richTextBox0.SelectedText = " {" + _Modul1.Instance.Kont[22].Trim() + "} ";
        }
        if (xEmitWitness && _Modul1.Instance.Kont[42].Trim() != "")
        {
            richTextBox0.SelectedText = " Zeugen: " + _Modul1.Instance.Kont[42].Trim() + " ";
        }
        if (xEmitSource)
        {
            FamDatQuelle(EEventArt.eA_Marriage, famInArb, richTextBox0);
        }
        if (_Modul1.Instance.Kont[3].Trim() != "")
        {
            richTextBox0.SelectedText = _Modul1.Instance.DTxt[8] + " " + _Modul1.Instance.Kont[3] + ". ";
            flag = true;
        }
        if (_Modul1.Instance.Kont[23].Trim() != "")
        {
            richTextBox0.SelectedText = " {" + _Modul1.Instance.Kont[23].Trim() + "} ";
        }
        if (xEmitWitness && _Modul1.Instance.Kont[43].Trim() != "")
        {
            richTextBox0.SelectedText = " Zeugen: " + _Modul1.Instance.Kont[43].Trim() + " ";
        }
        if (xEmitSource)
        {
            FamDatQuelle(EEventArt.eA_MarrReligious, famInArb, richTextBox0);
        }

        // EEventArt.eA_504
        if (_Modul1.Instance.Kont[4].Trim() != "")
        {
            richTextBox0.SelectedText = _Modul1.Instance.DTxt[9] + " " + _Modul1.Instance.Kont[4] + ". ";
            flag = true;
        }
        if (_Modul1.Instance.Kont[24].Trim() != "")
        {
            richTextBox0.SelectedText = " {" + _Modul1.Instance.Kont[24].Trim() + "} ";
        }
        if (xEmitWitness)
        {
            if (_Modul1.Instance.Kont[44].Trim() != "")
            {
                richTextBox0.SelectedText = " Zeugen: " + _Modul1.Instance.Kont[44].Trim() + " ";
            }
        }
        if (xEmitSource)
        {
            FamDatQuelle(EEventArt.eA_504, famInArb, richTextBox0);
        }

        // EEventArt.eA_505
        if (_Modul1.Instance.Kont[5].Trim() != "")
        {
            richTextBox0.SelectedText = _Modul1.Instance.DTxt[10] + " " + _Modul1.Instance.Kont[5] + ". ";
            flag = true;
        }
        if (_Modul1.Instance.Kont[25].Trim() != "")
        {
            richTextBox0.SelectedText = " {" + _Modul1.Instance.Kont[25].Trim() + "} ";
        }
        if (xEmitWitness)
        {
            if (_Modul1.Instance.Kont[45].Trim() != "")
            {
                richTextBox0.SelectedText = " Zeugen: " + _Modul1.Instance.Kont[45].Trim() + " ";
            }
        }
        if (xEmitSource)
        {
            FamDatQuelle(EEventArt.eA_505, famInArb, richTextBox0);
        }

        if (_Modul1.Instance.Person.Prae.Trim() != "")
        {
            richTextBox0.SelectedText = _Modul1.Instance.DTxt[15] + " " + _Modul1.Instance.Person.Prae + ". ";
            flag = true;
        }
        // EEventArt.eA_602
        if (_Modul1.Instance.Kont[27].Trim() != "")
        {
            richTextBox0.SelectedText = " {" + _Modul1.Instance.Kont[27].Trim() + "} ";
        }
        if (xEmitWitness)
        {
            if (_Modul1.Instance.Kont[47].Trim() != "")
            {
                richTextBox0.SelectedText = " Zeugen: " + _Modul1.Instance.Kont[47].Trim() + " ";
            }
        }

        if (!flag && _Modul1.Instance.DTxt[11] == "")
        {
            richTextBox0.SelectedText = _Modul1.Instance.DTxt[13];
        }
        var b = 20;
        while (b <= 27)
        {
            _Modul1.Instance.Kont[b] = "";
            b++;
        }
        Listart = 3;
        _Modul1.Instance.FWohn(EEventArt.eA_602, ref Listart);
        Famwohn(EEventArt.eA_602);
        Listart = 3;
        _Modul1.Instance.FWohn(EEventArt.eA_603, ref Listart);
        Famwohn(EEventArt.eA_603);
    }

    public bool EventLes(short eArt)
    {
        int persInArb = _Modul1.Instance.PersInArb;
        var CounterResult2 = 0;
        foreach (IEventData cEvebt in DataModul.Event.ReadAllBeSu(eArt.AsEnum<EEventArt>(), persInArb))
        {
            if (cEvebt.iLfNr > 0
            && cEvebt.iOrt > 0
            && CounterResult2 < Hinter_List1_Items.Count)
            {
                var Kol = Hinter_List1_Items[CounterResult2].ItemData<int>();
                if (cEvebt.iOrt == Kol)
                {
                    return false;
                }
            }
            if (CounterResult2++ > 200) break;
        }
        return true;
    }

    public void Beruflist()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        object CounterResult = default;
        object LoopForResult = default;
        int num5 = default;
        int num6 = default;
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
                    case 864:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_02c6;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_024e;
                        }
                    IL_024e:
                        num = 26;
                        if (Information.Err().Number == 3022)
                        {
                            goto IL_0263;
                        }
                        goto IL_027d;
                    IL_027d:
                        num = 30;
                        if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                        {
                            ProjectData.EndApp();
                        }
                        goto IL_02a3;
                    IL_01c7:
                        num = 19;
                        DataModul.DSB_SortTable.Fields["Sorttext"].Value = _Modul1.Instance.Kont[0].Trim() + "," + _Modul1.Instance.Person.Givennames.Trim();
                        goto IL_0206;
                    IL_02a3:
                        num = 33;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        num4 = num2;
                        goto IL_02ca;
                    IL_0206:
                        num = 20;
                        DataModul.DSB_SortTable.Update();
                        goto IL_0216;
                    IL_0216:
                        num = 22;
                        DataModul.DB_EventTable.MoveNext();
                        goto IL_0224;
                    IL_02ca:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_0008;
                            case 3:
                                goto IL_0046;
                            case 4:
                                goto IL_0070;
                            case 5:
                                goto IL_0082;
                            case 8:
                                goto IL_00db;
                            case 9:
                                goto IL_00ed;
                            case 11:
                            case 12:
                                goto IL_0127;
                            case 13:
                                goto IL_012f;
                            case 14:
                                goto IL_0153;
                            case 15:
                                goto IL_0162;
                            case 16:
                                goto IL_0189;
                            case 17:
                                goto IL_0192;
                            case 18:
                                goto IL_01a0;
                            case 19:
                                goto IL_01c7;
                            case 20:
                                goto IL_0206;
                            case 21:
                            case 22:
                                goto IL_0216;
                            case 6:
                            case 7:
                            case 23:
                                goto IL_0224;
                            case 10:
                            case 24:
                                goto IL_0236;
                            case 26:
                                goto IL_024e;
                            case 27:
                                goto IL_0263;
                            case 28:
                            case 30:
                                goto IL_027d;
                            case 31:
                            case 33:
                                goto IL_02a3;
                            default:
                                goto end_IL_0000;
                            case 25:
                            case 34:
                                goto end_IL_0000_2;
                        }
                        goto default;
                    IL_0263:
                        num = 27;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_02c6;
                    IL_01a0:
                        num = 18;
                        DataModul.DSB_SortTable.Fields["Nr"].Value = _Modul1.Instance.PersInArb;
                        goto IL_01c7;
                    IL_02c6:
                        num4 = num2 + 1;
                        goto IL_02ca;
                    IL_0008:
                        num = 2;
                        if (!ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, checked(MyProject.Forms.Ausw.List2.Items.Count - 1), 1, ref LoopForResult, ref CounterResult))
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_0046;
                    IL_0046:
                        num = 3;
                        num5 = Conversions.ToInteger(Strings.Right(MyProject.Forms.Ausw.List2.Items[CounterResult.AsInt()].AsString(), 10));
                        goto IL_0070;
                    IL_0070:
                        num = 4;
                        DataModul.DB_EventTable.Index = "KText";
                        goto IL_0082;
                    IL_0082:
                        num = 5;
                        DataModul.DB_EventTable.Seek("=", num5);
                        goto IL_0224;
                    IL_0224:
                        num = 7;
                        if (!DataModul.DB_EventTable.EOF)
                        {
                            goto IL_00db;
                        }
                        goto IL_0236;
                    IL_00db:
                        num = 8;
                        if (!DataModul.DB_EventTable.NoMatch)
                        {
                            goto IL_00ed;
                        }
                        goto IL_0216;
                    IL_00ed:
                        num = 9;
                        if (!Operators.ConditionalCompareObjectNotEqual(DataModul.DB_EventTable.Fields[EventFields.KBem].Value, Conversion.Val(num5.AsString()), TextCompare: false))
                        {
                            goto IL_0127;
                        }
                        goto IL_0236;
                    IL_0236:
                        num = 24;
                        if (!ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult, ref CounterResult))
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_0046;
                    IL_0127:
                        num = 12;
                        num6 = checked(num6 + 1);
                        goto IL_012f;
                    IL_012f:
                        num = 13;
                        Label2.Text = num6.AsString() + " Einträge";
                        goto IL_0153;
                    IL_0153:
                        num = 14;
                        Label2.Refresh();
                        goto IL_0162;
                    IL_0162:
                        num = 15;
                        _Modul1.Instance.PersInArb = DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt();
                        goto IL_0189;
                    IL_0189:
                        num = 16;
                        _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                        goto IL_0192;
                    IL_0192:
                        num = 17;
                        DataModul.DSB_SortTable.AddNew();
                        goto IL_01a0;
                    end_IL_0000:
                        break;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 864;
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

    public void Umlaut()
    {
        checked
        {
            while (true)
            {
                short num = (short)Strings.InStr(Persuname, "Ö");
                if (num > 0)
                {
                    string text = Strings.Mid(Persuname, num + 1, Persuname.Length);
                    StringType.MidStmtStr(ref Persuname, num, num + 1, "OE");
                    Persuname = Persuname.Left(num + 1) + text;
                    continue;
                }
                num = (short)Strings.InStr(Persuname, "ö");
                if (num > 0)
                {
                    string text = Strings.Mid(Persuname, num + 1, Persuname.Length);
                    StringType.MidStmtStr(ref Persuname, num, num + 1, "OE");
                    Persuname = Persuname.Left(num + 1) + text;
                    continue;
                }
                num = (short)Strings.InStr(Persuname, "Ü");
                if (num > 0)
                {
                    string text = Strings.Mid(Persuname, num + 1, Persuname.Length);
                    StringType.MidStmtStr(ref Persuname, num, num + 1, "UE");
                    Persuname = Persuname.Left(num + 1) + text;
                    continue;
                }
                num = (short)Strings.InStr(Persuname, "ü");
                if (num > 0)
                {
                    string text = Strings.Mid(Persuname, num + 1, Persuname.Length);
                    StringType.MidStmtStr(ref Persuname, num, num + 1, "UE");
                    Persuname = Persuname.Left(num + 1) + text;
                    continue;
                }
                num = (short)Strings.InStr(Persuname, "Ä");
                if (num > 0)
                {
                    string text = Strings.Mid(Persuname, num + 1, Persuname.Length);
                    StringType.MidStmtStr(ref Persuname, num, num + 1, "AE");
                    Persuname = Persuname.Left(num + 1) + text;
                    continue;
                }
                num = (short)Strings.InStr(Persuname, "ä");
                if (num > 0)
                {
                    string text = Strings.Mid(Persuname, num + 1, Persuname.Length);
                    StringType.MidStmtStr(ref Persuname, num, num + 1, "AE");
                    Persuname = Persuname.Left(num + 1) + text;
                    continue;
                }
                num = (short)Strings.InStr(Persuname, "Ş");
                if (num > 0)
                {
                    string text = Strings.Mid(Persuname, num + 1, Persuname.Length);
                    StringType.MidStmtStr(ref Persuname, num, num + 1, "S");
                    Persuname = Persuname.Left(num + 1) + text;
                    continue;
                }
                num = (short)Strings.InStr(Persuname, "Ś");
                if (num > 0)
                {
                    string text = Strings.Mid(Persuname, num + 1, Persuname.Length);
                    StringType.MidStmtStr(ref Persuname, num, num + 1, "S");
                    Persuname = Persuname.Left(num + 1) + text;
                    continue;
                }
                num = (short)Strings.InStr(Persuname, "Ţ");
                if (num > 0)
                {
                    string text = Strings.Mid(Persuname, num + 1, Persuname.Length);
                    StringType.MidStmtStr(ref Persuname, num, num + 1, "T");
                    Persuname = Persuname.Left(num + 1) + text;
                    continue;
                }
                break;
            }
        }
    }

    public void Ereilist(IList items)
    {
        IRecordset dB_EventTable = DataModul.DB_EventTable;

        dB_EventTable.Index = EventIndex.NText.AsFld();
        for (int i = 0; i < items.Count; i++)
        {
            var num5 = items[i].ItemData<int>();
            dB_EventTable.Seek("=", num5);
            while (dB_EventTable.EOF
                && !dB_EventTable.NoMatch
                && !Operators.ConditionalCompareObjectNotEqual(dB_EventTable.Fields["ArtText"].Value, Conversion.Val(num5.AsString()), TextCompare: false))
            {
                if (dB_EventTable.Fields["Art"].AsInt() != 603)
                {
                    int Event_iPerFamNr = dB_EventTable.Fields["PerFamNr"].AsInt();
                    _Modul1.Instance.Person_ReadNames(Event_iPerFamNr, _Modul1.Instance.Person);
                    string sText = _Modul1.Instance.Person.SurName.Trim() + "," + _Modul1.Instance.Person.Givennames.Trim();
                    DataModul_DSB_SortTable_Add(Event_iPerFamNr, sText);
                }
                dB_EventTable.MoveNext();
            }
        }
    }

    private static void DataModul_DSB_SortTable_Add(int Event_iPerFamNr, string sText)
    {
        IRecordset dSB_SortTable = DataModul.DSB_SortTable;
        dSB_SortTable.AddNew();
        dSB_SortTable.Fields["Nr"].Value = Event_iPerFamNr;
        dSB_SortTable.Fields["Sorttext"].Value = sText;
        dSB_SortTable.Update();
    }

    public void leerweg(RichTextBox RichTextBox1)
    {
        while (RichTextBox1.SelectionStart > 0 && Strings.Mid(RichTextBox1.Text, RichTextBox1.SelectionStart, 1) == " ")
        {
            RichTextBox1.SelectionStart = checked(RichTextBox1.SelectionStart - 1);
            RichTextBox1.SelectionLength = 1;
            RichTextBox1.SelectedText = "";
        }
    }

    public void DataModul_PerStatTab_Append(int persInArb)
    {
        string value = DataModul.Person.GetSex(persInArb);

        IRecordset dSB_PerStatTable = DataModul.DSB_PerStatTable;
        dSB_PerStatTable.AddNew();
        dSB_PerStatTable.Fields["PerNr"].Value = persInArb;
        if (value.AsString() == "M")
        {
            dSB_PerStatTable.Fields["M"].Value = true;
        }
        else if (value.AsString() == "F")
        {
            dSB_PerStatTable.Fields["F"].Value = true;
        }
        else if (value.AsString() == "U")
        {
            dSB_PerStatTable.Fields["U"].Value = true;
        }

        if (DataModul.Event.ReadData(EEventArt.eA_Birth, persInArb, out var cEv) && cEv!.dDatumV != default)
        {
            dSB_PerStatTable.Fields["geb"].Value = cEv.dDatumV.Year;
        }
        else if (DataModul.Event.ReadData(EEventArt.eA_Baptism, persInArb, out var cEv2) && cEv2!.dDatumV != default)
        {
            dSB_PerStatTable.Fields["geb"].Value = cEv2.dDatumV.Year;
        }
        else
            dSB_PerStatTable.Fields["geb"].Value = 0;

        if (DataModul.Event.ReadData(EEventArt.eA_Death, persInArb, out var cEv3) && cEv3!.dDatumV != default)
        {
            dSB_PerStatTable.Fields["gest"].Value = cEv3.dDatumV.Year;
        }
        else if (DataModul.Event.ReadData(EEventArt.eA_Burial, persInArb, out var cEv4) && cEv4!.dDatumV != default)
        {
            dSB_PerStatTable.Fields["gest"].Value = cEv4.dDatumV.Year;
        }
        else
            dSB_PerStatTable.Fields["gest"].Value = 0;

        dSB_PerStatTable.Fields["Sich"].Value = false;
        dSB_PerStatTable.Fields["An"].Value = 0;

        if (dSB_PerStatTable.Fields["geb"].AsDate() != default
                    & dSB_PerStatTable.Fields["gest"].AsDate() != default)
        {
            dSB_PerStatTable.Fields["Alt"].Value = Operators.SubtractObject(dSB_PerStatTable.Fields["gest"].Value, dSB_PerStatTable.Fields["geb"].Value);
        }
        else
        {
            dSB_PerStatTable.Fields["Alt"].Value = -1;
        }
        dSB_PerStatTable.Update();
    }

    private void Write_Kopf(RichTextBox richTextBox1_0)
    {
        Font fntBold = new(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
        Font fntRegular = new(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
        EEventArt eArt = _Modul1.Instance.Art;
        var Anfang = this.Anfang;
        var Aufhoer = this.Aufhoer;
        IList items = MyProject.Forms.Ausw.List2.Items;
        string sBez = Bezeichnung1.Text;
        DateTime today = DateTime.Today;

        richTextBox1_0.SelectionFont = fntBold;
        if (eArt == EEventArt.eA_135)
        {
            var sList = string.Join("; ", items.Cast<object>().Select(i => i.AsString().Left(100).Trim()));
            if (sList.Length > 0)
            {
                sBez = $"{sBez}: {sList}";
            }
        }
        richTextBox1_0.SelectedText = sBez + "\n";
        richTextBox1_0.SelectionFont = fntRegular;
        if ((Anfang != "") | (Aufhoer != ""))
        {
            if (Anfang != "")
            {
                richTextBox1_0.SelectedText = $" von {Anfang}";
            }
            if (Aufhoer != "")
            {
                richTextBox1_0.SelectedText = $" bis {Aufhoer}";
            }
            richTextBox1_0.SelectedText = "\n";
        }
        richTextBox1_0.SelectedText = $"Erstellt am {today}";
        richTextBox1_0.SelectedText = $" von {_Modul1.Instance.User.Owner.Trim()} mit {_Modul1.Instance.AppName} aus Mandant: {_Modul1.Instance.Verz}\n";
    }


    public void WriteKinder(RichTextBox richTextBox1_0)
    {
        Ausw ausw = MyProject.Forms.Ausw;
        Font fntBold = new(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
        Font fntRegular = new(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);

        if (ausw.Kontroll[1].CheckState == CheckState.Unchecked)
        {
            return;
        }
        List2.Items.Clear();
        _Modul1.Instance.Famles();
        if (_Modul1.Instance.Family.Kind[1] > 0)
        {
            PerPos = 3;
            richTextBox1_0.SelectionFont = fntBold;
            if (List1.Items.Count > 1)
            {
                richTextBox1_0.SelectedText = "\nKinder:" + unchecked(Fk) + 1.AsString() + ". Ehe";
            }
            else
            {
                richTextBox1_0.SelectedText = "\nKinder:";
            }
            richTextBox1_0.SelectionFont = fntRegular;
        }
        byte b = 1;
        while (_Modul1.Instance.Family.Kind[b] != 0)
        {
            int persInArb = _Modul1.Instance.Family.Kind[b];
            _Modul1.Instance.Datschalt = 1;
            _Modul1.Instance.Datles(persInArb, out var asPersDates);
            Kidat = "        ";
            if (_Modul1.Instance.Kont[1] != "")
            {
                Kidat = Kidat + _Modul1.Instance.Kont[1].Right(8);
            }
            else
            {
                Kidat = Kidat + _Modul1.Instance.Kont[2].Right(8);
            }
            List2.Items.Add(Kidat + "          " + _Modul1.Instance.Family.Kind[b].AsString().Right(10));
            b = (byte)unchecked((uint)(b + 1));
            if (unchecked(b) > 100u)
            {
                break;
            }
        }
        _Modul1.Instance.Datschalt = 0;
        for (var num = 0; num <= List2.Items.Count - 1; num++)
        {
            richTextBox1_0.SelectionFont = fntRegular;
            if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) != "\n")
            {
                richTextBox1_0.SelectedText = "\n";
            }
            richTextBox1_0.SelectionFont = fntRegular;
            _Modul1.Instance.PersInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List2.Items[num].AsString(), 9, 10)));
            Datmit = 1;
            Qu1 = (byte)ausw.Kontroll[7].CheckState;
            Personles();
            Datmit = 0;
        }
    }

    public void Personles()
    {
        RichTextBox richTextBox1_0 = RichTextBox1[0];
        Font fntBold = new(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
        Font fntRegular = new(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
        int persInArb = _Modul1.Instance.PersInArb;
        Ausw ausw = MyProject.Forms.Ausw;
        string hinter_Anz_Texz = Hinter_Anz_Texz;

        IPersonData person = _Modul1.Instance.Person;

        int num = default;
        string UbgT = default;
        byte b = default;
        Application.DoEvents();
        DataModul_PerStatTab_Append(_Modul1.Instance.PersInArb);
        _Modul1.Instance.Person_ReadNames(persInArb, person);
        richTextBox1_0.SelectionIndent = 50;
        if (ausw.Kontroll[3].Checked)
        {
            richTextBox1_0.SelectedText = "<" + persInArb.AsString().Trim() + "> ";
        }
        richTextBox1_0.SelectedText = person.Givennames + " ";
        if (_Modul1.Instance.Kont[1] != "")
        {
            richTextBox1_0.SelectedText = _Modul1.Instance.Kont[1] + " ";
        }
        var Ahne = IZahl;
        _Modul1.Instance.Namenindex(Ahne);
        leerweg(richTextBox1_0);
        richTextBox1_0.SelectionFont = fntBold;
        richTextBox1_0.SelectedText = " " + _Modul1.Instance.Kont[0];
        richTextBox1_0.SelectionFont = fntRegular;
        if (_Modul1.Instance.Kont[2].Trim() != "")
        {
            richTextBox1_0.SelectedText = " " + _Modul1.Instance.Kont[2];
        }
        if (_Modul1.Instance.Kont[4].Trim() != "")
        {
            richTextBox1_0.SelectedText = " (" + _Modul1.Instance.Kont[4] + ")";
        }
        if (_Modul1.Instance.Kont[5].Trim() != "")
        {
            richTextBox1_0.SelectedText = ", Sippe " + _Modul1.Instance.Kont[5];
        }
        if (ausw.Kontroll[10].Checked | ausw.Kontroll[11].Checked)
        {
            WriteNachnr(richTextBox1_0);
        }
        richTextBox1_0.SelectionFont = fntRegular;
        if (ausw.Kontroll[2].Checked)
        {
            Datmit = 2;
        }
        if (Datmit == 1)
        {
            b = checked((byte)ausw.Kontroll[0].CheckState);
        }
        else if (Datmit == 2)
        {
            b = checked((byte)ausw.Kontroll[2].CheckState);
        }

        if (b == 1)
        {
            _Modul1.Instance.Datles(persInArb, out var asPersDates);
            if (asPersDates[11].Trim() != "")
            {
                asPersDates[11] = " " + _Modul1.Instance.DTxt[1] + " " + asPersDates[11].Trim() + ".";
            }
            if (asPersDates[12].Trim() != "")
            {
                asPersDates[12] = " " + _Modul1.Instance.DTxt[2] + " " + asPersDates[12].Trim() + ".";
            }
            if (asPersDates[13].Trim() != "")
            {
                asPersDates[13] = " " + _Modul1.Instance.DTxt[3] + " " + asPersDates[13].Trim() + ".";
            }
            if (asPersDates[14].Trim() != "")
            {
                asPersDates[14] = " " + _Modul1.Instance.DTxt[4] + " " + asPersDates[14].Trim() + ".";
            }
            richTextBox1_0.SelectedText = asPersDates[11] + asPersDates[12] + asPersDates[13] + asPersDates[14];
            hinter_Anz_Texz = "";
            var PerPos = this.PerPos;
            _Modul1.Instance.Erei(persInArb, EEventArt.eA_300, ref PerPos);
            if (hinter_Anz_Texz != "")
            {
                richTextBox1_0.SelectedText = " " + hinter_Anz_Texz.Trim();
            }
            hinter_Anz_Texz = "";
            _Modul1.Instance.Erei(persInArb, EEventArt.eA_301, ref PerPos);
            if (hinter_Anz_Texz != "")
            {
                if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) == "\n")
                {
                    richTextBox1_0.SelectedText = hinter_Anz_Texz.Trim();
                }
                else richTextBox1_0.SelectedText = " " + hinter_Anz_Texz.Trim();
            }
            hinter_Anz_Texz = "";
            _Modul1.Instance.Erei(persInArb, EEventArt.eA_302, ref PerPos);
            if (hinter_Anz_Texz != "")
            {
                if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) == "\n")
                {
                    richTextBox1_0.SelectedText = hinter_Anz_Texz.Trim();
                }
                else richTextBox1_0.SelectedText = " " + hinter_Anz_Texz.Trim();
            }
            hinter_Anz_Texz = "";
            _Modul1.Instance.Erei(persInArb, EEventArt.eA_105, ref this.PerPos);
            if (hinter_Anz_Texz != "")
            {
                if (Strings.Mid(richTextBox1_0.Text, richTextBox1_0.SelectionStart, 1) == "\n")
                {
                    richTextBox1_0.SelectedText = hinter_Anz_Texz.Trim();
                }
                else richTextBox1_0.SelectedText = " " + hinter_Anz_Texz.Trim();
            }
        }
        richTextBox1_0.SelectionFont = fntRegular;
        if (Qu1 == 1)
        {
            _Modul1.Instance.PerSatzLes(persInArb);
            if (DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString() != "")
            {
                UbgT = DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString();
            }

            DataModul.DB_SourceLinkTable.Index = "Tab";
            UbgT = DataModul_SourceLink_PersForeach(persInArb, UbgT);
            if (UbgT.Trim() != "")
            {
                UbgT = _Modul1.Instance.Retweg(UbgT);
                richTextBox1_0.SelectedText = " Quellen: " + UbgT.Trim() + ".";
                UbgT = "";
            }
        }
        Qu1 = 0;
    }

    private static string DataModul_SourceLink_PersForeach(int persInArb, string UbgT)
    {
        IRecordset dB_SourceLinkTable = DataModul.DB_SourceLinkTable;
        dB_SourceLinkTable.Seek("=", 1, persInArb);
        while (!dB_SourceLinkTable.EOF && !dB_SourceLinkTable.NoMatch
            && !Conversions.ToBoolean(dB_SourceLinkTable.Fields[SourceLinkFields._1].AsInt() != 1
            | dB_SourceLinkTable.Fields["2"].AsInt() != persInArb))
        {
            int SourceLink_iField3 = dB_SourceLinkTable.Fields[SourceLinkFields._3].AsInt();
            string SourceLink_sAus = dB_SourceLinkTable.Fields["Aus"].AsString();
            string SourceLink_sOrig = dB_SourceLinkTable.Fields["Orig"].AsString();

            if (DataModul_QuTable_ReadTitle(SourceLink_iField3, out var QuTable_sField4))
            {
                if (UbgT.Trim() != "")
                {
                    UbgT += "; ";
                }
                UbgT = UbgT + QuTable_sField4;
                if (SourceLink_iField3 != 0)
                {
                    if (SourceLink_sAus == "")
                    {
                        UbgT += $" Seiten: {SourceLink_iField3}";
                    }
                    else
                        UbgT += $" {SourceLink_sAus.Trim()} {SourceLink_iField3}";
                }
                if (SourceLink_sOrig != "")
                {
                    UbgT = UbgT + " >" + SourceLink_sOrig.AsString().Trim() + "<";
                }
                DataModul_Quellen_AddRaw(QuTable_sField4, SourceLink_iField3);
            }


            dB_SourceLinkTable.MoveNext();
        }

        return UbgT;
    }

    private static bool DataModul_QuTable_ReadTitle(int iQuelle, out string sTitle)
    {
        DataModul.DB_QuTable.Index = "NR";
        DataModul.DB_QuTable.Seek("=", iQuelle);
        if (!DataModul.DB_QuTable.NoMatch)
        {
            sTitle = DataModul.DB_QuTable.Fields[QuFields._4].AsString();
            return true;
        }
        else
        {
            sTitle = "";
            return false;
        }
    }

    private static void DataModul_Quellen_AddRaw(string value, int value1)
    {
        DataModul.DSB_QuellIdxTable.AddNew();
        DataModul.DSB_QuellIdxTable.Fields["Quelle"].Value = value;
        DataModul.DSB_QuellIdxTable.Fields["Nr"].Value = value1;
        DataModul.DSB_QuellIdxTable.Update();
    }

    public void Famwohn(EEventArt Art)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        short num5 = default;
        string Job = default;
        short num6 = default;
        string text3 = default;
        byte b2 = default;
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
                    int AAA;
                    string LD;
                    int Nr;
                    short LfNR;
                    switch (try0000_dispatch)
                    {
                        default:
                            {
                                num = 1;
                                Job = "";
                                array = new object[6];
                                ProjectData.ClearProjectError();
                                num3 = 2;
                                num6 = (short)(List3.Items.Count - 1);
                                num5 = 0;
                                goto IL_0ebf;
                            }
                        case 4659:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0f95;
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
                                goto IL_0f99;
                            }
                        end_IL_0000:
                            break;
                        IL_023e: // <========== 3
                            num = 23;
                            _Modul1.Instance.sDatu = "";
                            if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.FamInArb)))
                            {
                                DataModul.DB_EventTable.Index = "ArtNr";
                                goto end_IL_0000_2;
                            }
                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].Value.AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                LD = "";
                                DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                                _Modul1.Instance.Kont1[7] = " " + _Modul1.Instance.Kont[0].Trim();
                            }
                            goto IL_0348;
                        IL_0348:
                            num = 32;
                            if (Art == EEventArt.eA_602)
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
                            goto IL_0441;
                        IL_0441: // <========== 3
                            num = 41;
                            string text2;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                            {
                                _Modul1.Instance.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                _Modul1.Instance.sDatu = "00000000" + _Modul1.Instance.sDatu.Right(8);
                                text2 = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                if ((text2.Trim() == "") & DataModul.DB_EventTable.Fields[EventFields.DatumB].AsDate() != default)
                                {
                                    text2 = " von";
                                }
                                _Modul1.Instance.Datwand1(ref _Modul1.Instance.sDatu, text2);
                                _Modul1.Instance.Kont1[1] = _Modul1.Instance.sDatu;
                            }
                            goto IL_0555;
                        IL_0555:
                            num = 51;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                _Modul1.Instance.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                _Modul1.Instance.sDatu = "00000000" + _Modul1.Instance.sDatu.Right(8);
                                text2 = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                _Modul1.Instance.Datwand1(ref _Modul1.Instance.sDatu, text2);
                                if (_Modul1.Instance.Kont1[1].Left(5) != "zwisc")
                                {
                                    if (_Modul1.Instance.sDatu.Trim() != "")
                                    {
                                        _Modul1.Instance.sDatu = " bis " + _Modul1.Instance.sDatu.Trim();
                                    }
                                    goto IL_0653;
                                }
                                goto IL_0692;
                            }
                            goto IL_06a2;
                        IL_0653:
                            num = 60;
                            if (_Modul1.Instance.Kont1[1].Trim() != "")
                            {
                                _Modul1.Instance.Kont1[1] = " von " + _Modul1.Instance.Kont1[1].Trim();
                            }
                            goto IL_0692;
                        IL_0692: // <========== 3
                            num = 64;
                            _Modul1.Instance.Kont1[3] = _Modul1.Instance.sDatu;
                            goto IL_06a2;
                        IL_06a2: // <========== 3
                            num = 66;
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                            {
                                _Modul1.Instance.Kont1[5] = " " +
                                    _Modul1.Instance.ortles1(DataModul.DB_EventTable.Fields[EventFields.Ort].AsInt(), 1, (i, s) => _Modul1.Instance.ExportPlace(i, s, _Modul1.Instance.Ind1, M_Namen));
                            }
                            goto IL_0738;
                        IL_0738:
                            num = 71;
                            if (DataModul.DB_EventTable.Fields[EventFields.Platz].Value.AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                LD = "";
                                DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                                if (_Modul1.Instance.Kont[0] != "")
                                {
                                    _Modul1.Instance.Kont1[8] = " " + _Modul1.Instance.Kont[0].Trim();

                                }
                            }
                            goto IL_07f0;
                        IL_07f0: // <========== 3
                            num = 77;
                            Job = "";
                            Module2.Jobdreh(Job);
                            if (Job != "")
                            {
                                if (RichTextBox1[0].SelectionStart > 0)
                                {
                                    if (Strings.Mid(RichTextBox1[0].Text, RichTextBox1[0].SelectionStart, 1) == ".")
                                    {
                                        RichTextBox1[0].SelectionStart = RichTextBox1[0].SelectionStart - 1;
                                        RichTextBox1[0].SelectionLength = 2;
                                        RichTextBox1[0].SelectedText = ", ";

                                    }
                                }
                                goto IL_08ca;
                            }
                            goto IL_0ea0;
                        IL_08ca: // <========== 3
                            num = 87;
                            if (Art == EEventArt.eA_602)
                            {
                                RichTextBox1[0].SelectedText = "Wohnort der Familie:";
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                RichTextBox1[0].SelectedText = " ";
                            }
                            goto IL_0940;
                        IL_0940:
                            num = 93;
                            if (Art == EEventArt.eA_603)
                            {
                                if (_Modul1.Instance.Kont1[4].Trim() == "")
                                {
                                    RichTextBox1[0].SelectedText = "Sonst. Daten der Familie:";
                                }
                                goto IL_0984;
                            }
                            goto IL_099e;
                        IL_0984:
                            num = 97;
                            RichTextBox1[0].SelectedText = " ";
                            goto IL_099e;
                        IL_099e: // <========== 3
                            num = 99;
                            RichTextBox1[0].SelectedText = Job + ".";
                            _Modul1.Instance.UbgT1 = "";
                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                            {
                                _Modul1.Instance.UbgT1 += DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim();
                            }
                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                            {
                                _Modul1.Instance.UbgT1 = _Modul1.Instance.UbgT1 + " " + DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim();
                            }
                            if (_Modul1.Instance.UbgT1.Trim() != "")
                            {
                                _Modul1.Instance.UbgT1 = _Modul1.Instance.Retweg(_Modul1.Instance.UbgT1);
                                RichTextBox1[0].SelectedText = " {" + _Modul1.Instance.UbgT1 + "} ";
                                _Modul1.Instance.UbgT1 = "";
                            }

                            int famInArb = _Modul1.Instance.FamInArb;
                            if (MyProject.Forms.Ausw.Kontroll[4].Checked)
                            {
                                LD = _Modul1.Instance.FamInArb.AsString();
                                _Modul1.Instance.Zeugsu(Art, _Modul1.Instance.LfNR, 2, (long)_Modul1.Instance.FamInArb);
                                _Modul1.Instance.FamInArb = LD.AsInt();
                            }
                            text3 = _Modul1.Instance.Kont1[20];
                            _Modul1.Instance.Kont1[20] = "";
                            _Modul1.Instance.FamInArb = famInArb;
                            if (b2 == 1)
                            {
                                if (text3 != "")
                                {
                                    RichTextBox1[0].SelectedText = " ";
                                    if (_Modul1.Instance.DAus[100] == "1")
                                    {
                                        RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
                                    }
                                    else
                                    {

                                        RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Underline);
                                    }
                                    goto IL_0c54;
                                }
                            }
                            goto IL_0d68;
                        IL_0c54: // <========== 3
                            num = 128;
                            RichTextBox1[0].SelectedText = "Zeugen: ";
                            if (_Modul1.Instance.DAus[100] == "1")
                            {
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                            }
                            else
                            {

                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            }
                            goto IL_0d08;
                        IL_0d08: // <========== 3
                            num = 135;
                            RichTextBox1[0].SelectedText = text3.Trim();
                            text3 = "";
                            RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            goto IL_0d68;
                        IL_0d68: // <========== 3
                            num = 140;
                            Nr = _Modul1.Instance.FamInArb;
                            LfNR = _Modul1.Instance.LfNR;
                            _Modul1.Instance.QuellenDatum(ref Nr, Art, ref LfNR);
                            _Modul1.Instance.LfNR = Conversions.ToByte(LfNR);
                            _Modul1.Instance.FamInArb = Nr.AsInt();
                            if (_Modul1.Instance.Kont1[9].Trim() != "")
                            {
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)(_Modul1.Instance.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                RichTextBox1[0].SelectionCharOffset = _Modul1.Instance.PrintDat.Hoch;
                                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont1[9];
                                RichTextBox1[0].SelectionCharOffset = 0;
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            }
                            goto IL_0ea0;
                        IL_0ea0: // <========== 3
                            num = 149;
                            DataModul.DB_EventTable.MoveNext();
                            num5 = (short)unchecked(num5 + 1);
                            goto IL_0ebf;
                        IL_0ebf:
                            if (num5 <= num6)
                            {
                                _Modul1.Instance.LfNR = Conversions.ToByte(List3.Items[num5].AsString().Right(10));
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", Art.AsString(), _Modul1.Instance.FamInArb.AsString(), _Modul1.Instance.LfNR);
                                if (Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 0, TextCompare: false))
                                {
                                    Interaction.MsgBox("Stop 14");
                                }
                                M1_J = 0;
                                while (unchecked(M1_J) <= 15u)
                                {
                                    _Modul1.Instance.Kont1[M1_J] = "";
                                    M1_J = (byte)unchecked((uint)(M1_J + 1));
                                }
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value))
                                {
                                    if (DataModul.DB_EventTable.Fields[EventFields.ArtText].Value.AsDouble() > 0.0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                                        LD = "";
                                        DataModul.Textlese(AAA, out _Modul1.Instance.Kont[0], out LD);
                                        if (_Modul1.Instance.Kont[0] != "")
                                        {
                                            _Modul1.Instance.Kont1[4] = " " + _Modul1.Instance.Kont[0].Trim() + ": ";
                                        }

                                    }
                                }
                                goto IL_023e;
                            }
                            else if (Operators.CompareString(RichTextBox1[0].Text.Trim().Right(1), "\n", TextCompare: false) == 0)
                            {
                            }
                            else
                            {

                                RichTextBox1[0].SelectedText = "\n";
                            }
                            goto end_IL_0000_2;
                        IL_0f95:
                            num4 = unchecked(num2 + 1);
                            goto IL_0f99;
                        IL_0f99:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 20:
                                case 21:
                                case 22:
                                case 23:
                                    goto IL_023e;
                                case 31:
                                case 32:
                                    goto IL_0348;
                                case 38:
                                case 39:
                                case 40:
                                case 41:
                                    goto IL_0441;
                                case 50:
                                case 51:
                                    goto IL_0555;
                                case 59:
                                case 60:
                                    goto IL_0653;
                                case 62:
                                case 63:
                                case 64:
                                    goto IL_0692;
                                case 65:
                                case 66:
                                    goto IL_06a2;
                                case 70:
                                case 71:
                                    goto IL_0738;
                                case 75:
                                case 76:
                                case 77:
                                    goto IL_07f0;
                                case 85:
                                case 86:
                                case 87:
                                    goto IL_08ca;
                                case 92:
                                case 93:
                                    goto IL_0940;
                                case 96:
                                case 97:
                                    goto IL_0984;
                                case 98:
                                case 99:
                                    goto IL_099e;
                                case 124:
                                case 127:
                                case 128:
                                    goto IL_0c54;
                                case 131:
                                case 134:
                                case 135:
                                    goto IL_0d08;
                                case 138:
                                case 139:
                                case 140:
                                    goto IL_0d68;
                                case 147:
                                case 148:
                                case 149:
                                    goto IL_0ea0;
                                case 26:
                                case 153:
                                case 154:
                                case 157:
                                case 163:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 4659;
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

    public void FamDatQuelle(EEventArt Art, int Faminarb, RichTextBox richTextBox0)
    {
        short LfNR = 0;
        _Modul1.Instance.SLQuellenDatum(ref Faminarb, Art, ref LfNR);

        _Modul1.Instance.UbgT = "";
        if (_Modul1.Instance.Kont1[9].Trim() != "")
        {
            _Modul1.Instance.UbgT = " Quelle: " + _Modul1.Instance.Kont1[9].Trim();
            _Modul1.Instance.Kont1[9] = "";
        }

        if (_Modul1.Instance.UbgT.Trim() != "")
        {
            richTextBox0.SelectedText = _Modul1.Instance.UbgT;
            _Modul1.Instance.UbgT = "";
        }
    }

    private void Button2_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_01c4
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string text = default;
        long num6 = default;
        long num7 = default;
        string text2 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                long num5;
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0008;
                    case 578:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_01c8;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_0193;
                        }
                    IL_0193:
                        num = 24;
                        if (Information.Err().Number != 75)
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_01a5;
                    IL_01a5:
                        num = 25;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_01c8;
                    IL_0179:
                        num = 21;
                        Interaction.MsgBox("ein Dateiname muss angegeben werden", MsgBoxStyle.OkOnly, "");
                        goto end_IL_0000_2;
                    IL_01c8:
                        num4 = num2 + 1;
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_0008;
                            case 3:
                                goto IL_0022;
                            case 4:
                                goto IL_0037;
                            case 5:
                                goto IL_004e;
                            case 6:
                                goto IL_0067;
                            case 7:
                                goto IL_0076;
                            case 8:
                                goto IL_0090;
                            case 9:
                                goto IL_00a2;
                            case 10:
                                goto IL_00b0;
                            case 11:
                                goto IL_00c8;
                            case 12:
                                goto IL_00f0;
                            case 13:
                                goto IL_010c;
                            case 14:
                                goto IL_011a;
                            case 15:
                                goto IL_0128;
                            case 16:
                                goto IL_0141;
                            case 17:
                                goto IL_0156;
                            case 18:
                                goto IL_0166;
                            case 20:
                                goto IL_0175;
                            case 21:
                                goto IL_0179;
                            case 24:
                                goto IL_0193;
                            case 25:
                                goto IL_01a5;
                            default:
                                goto end_IL_0000;
                            case 19:
                            case 22:
                            case 23:
                            case 26:
                                goto end_IL_0000_2;
                        }
                        goto default;
                    IL_0008:
                        num = 2;
                        text = Interaction.InputBox("Bitte geben Sie einen Namen für die Steuerdatei ein.", "Achtung");
                        goto IL_0022;
                    IL_0022:
                        num = 3;
                        if (text != "")
                        {
                            goto IL_0037;
                        }
                        goto IL_0175;
                    IL_0037:
                        num = 4;
                        FileSystem.MkDir(_Modul1.Instance.Verz + "ged");
                        goto IL_004e;
                    IL_004e:
                        num = 5;
                        text = _Modul1.Instance.Verz + "ged\\" + text + ".sdat";
                        goto IL_0067;
                    IL_0067:
                        num = 6;
                        FileSystem.FileOpen(99, text, OpenMode.Output);
                        goto IL_0076;
                    IL_0076:
                        num = 7;
                        FileSystem.PrintLine(99, _Modul1.Instance.Verz);
                        goto IL_0090;
                    IL_0090:
                        num = 8;
                        DataModul.DB_GedTable.Index = "pNR";
                        goto IL_00a2;
                    IL_00a2:
                        num = 9;
                        DataModul.DB_GedTable.MoveFirst();
                        goto IL_00b0;
                    IL_00b0:
                        num = 10;
                        num5 = 1L;
                        num6 = checked(DataModul.DB_GedTable.RecordCount - 1);
                        num7 = num5;
                        goto IL_0123;
                    IL_0123:
                        if (num7 <= num6)
                        {
                            goto IL_00c8;
                        }
                        goto IL_0128;
                    IL_0128:
                        num = 15;
                        FileSystem.FileClose(99);
                        goto IL_0141;
                    IL_0141:
                        num = 16;
                        text2 = "Die Steuerdatei wird unter \n" + text + "\ngespeichert";
                        goto IL_0156;
                    IL_0156:
                        num = 17;
                        text2 += "\nIm Im/Exportmenü können Sie diese Datei zum Erstellen einer Gedcom-Datei, die alle Personen dieses Ausdrucks enthält, unter\n>>Spezial-Gedcom nach Ausdrucken<<\nauswählen";
                        goto IL_0166;
                    IL_0166:
                        num = 18;
                        Interaction.MsgBox(text2);
                        goto end_IL_0000_2;
                    IL_00c8:
                        num = 11;
                        Text = DataModul.DB_GedTable.Fields[GedFields.Pnr].AsString();
                        goto IL_00f0;
                    IL_00f0:
                        num = 12;
                        FileSystem.PrintLine(99, Text);
                        goto IL_010c;
                    IL_010c:
                        num = 13;
                        DataModul.DB_GedTable.MoveNext();
                        goto IL_011a;
                    IL_011a:
                        num = 14;
                        num7 = checked(num7 + 1);
                        goto IL_0123;
                    IL_0175:
                        num = 20;
                        goto IL_0179;
                    end_IL_0000:
                        break;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 578;
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

    private void _Befehl_8_Click(object sender, EventArgs e)
    {
    }

    private void Label2_Click(object sender, EventArgs e)
    {
    }
}
