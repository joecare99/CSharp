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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Druck.Views;

public partial class Druck : Form
{
    private bool xGED;

    [STAThread]
    public static void Main()
    {
        Application.Run(MyProject.Forms.Druck);
    }

    [DebuggerNonUserCode]
    public Druck()
    {
        base.FormClosing += Druck_FormClosing;
        base.Load += Druck_Load;
        InitializeComponent();
        Bef.SetIndex(this._Bef_28, 28);
        Bef.SetIndex(this._Bef_27, 27);
        Bef.SetIndex(this._Bef_26, 26);
        Bef.SetIndex(this._Bef_25, 25);
        Bef.SetIndex(this._Bef_24, 24);
        Bef.SetIndex(this._Bef_23, 23);
        Bef.SetIndex(this._Bef_16, 16);
        Bef.SetIndex(this._Bef_15, 15);
        Bef.SetIndex(this._Bef_14, 14);
        Bef.SetIndex(this.btnAncestorsByClan, 13);
        Bef.SetIndex(this._Bef_12, 12);
        Bef.SetIndex(this.btnClansList, 11);
        Bef.SetIndex(this._Bef_10, 10);
        Bef.SetIndex(this._Bef_5, 5);
        Bef.SetIndex(this.btnFamilyBook, 3);
        Bef.SetIndex(this.btnPersonSheet, 0);
        Bef.SetIndex(this.btnAncestorsWChildren, 7);
        Bef.SetIndex(this.btnFamilySheet, 1);
        Bef.SetIndex(this.btnAncestorsList, 2);
        Bef.SetIndex(this._Bef_4, 4);
        Bef.SetIndex(this.btnMainmenue, 6);
        Bef.SetIndex(this.btnPlaceList, 8);
        Bef.SetIndex(this.btnDescendentsList, 9);
        Bef.SetIndex(this.btnPlaceFamilyBook, 17);
        Bef.SetIndex(this._Bef_18, 18);
        Bef.SetIndex(this._Bef_19, 19);
        Bef.SetIndex(this.btnClanSequenceList, 20);
        Bef.SetIndex(this.btnVariousLists, 21);
        Bef.SetIndex(this._Bef_22, 22);
        Label1.SetIndex(this.lblHdr1, 0);
        Label1.SetIndex(this.lblHdr2, 1);
        Label1.SetIndex(this.lblHdr3, 2);
        Bef.AddClick(Bef_Click);
        Bef.AddKeyPress(Bef_KeyPress);

    }

    private void Bef_Click(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    string text2;
                    short num6;
                    int num7, num8, num11;

                    int num4;
                    int num9;
                    int num12;
                    int index;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            index = Bef.GetIndex((Button)eventSender);
                            goto IL_0015;
                        case 5037:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                    case 4:
                                        break;
                                    case 3:
                                    case 5:
                                        goto IL_0edf;
                                    case 1:
                                        goto IL_0f37;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 91)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0f37;
                                }
                                if (Information.Err().Number == 3021)
                                {
                                    Interaction.MsgBox("Es ist keine Ahnenberechnung vorhanden. Sie müssen zuerst die Ahnen berechnen (Hauptmenue).");
                                    goto end_IL_0000_2;
                                }
                                if (Information.Err().Number == 3420)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0f37;
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
                                goto IL_0f3b;
                            }
                        end_IL_0000_3:
                            break;
                        IL_0015:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            _Modul1.Instance.DAus[73] = "0";
                            _Modul1.Instance.Druck_Tast = 0;
                            xGED = true;
                            int num5 = 0;
                            while (num5 <= 120)
                            {
                                _Modul1.Instance.DAus[num5] = "0";
                                num5++;
                            }
                            switch (index)
                            {
                                case 0:
                                    Bef_0_Click();
                                    break;
                                case 1:
                                    Bef_1_Click();
                                    break;
                                case 2:
                                    Bef_2_Click();
                                    break;
                                case 3:
                                    Bef_3_Click();
                                    break;
                                case 4:
                                    Bef_4_Click();
                                    break;
                                case 6:
                                    Bef_6_Click();
                                    break;
                                case 7:
                                    goto IL_0867;
                                    break;
                                case 8:
                                    goto IL_088f;
                                    break;
                                case 9:
                                    goto IL_08aa;
                                    break;
                                case 10:
                                    goto IL_090e;
                                    break;
                                case 11:
                                    goto IL_0929;
                                    break;
                                case 12:
                                    goto IL_0944;
                                case 13:
                                    goto IL_0a98;
                                case 14:
                                    goto IL_0ada;
                                case 15:
                                    goto IL_0b0f;
                                case 16:
                                    goto IL_0b52;
                                case 17:
                                    goto IL_0b70;
                                case 19:
                                    goto IL_0ba6;
                                case 20:
                                    goto IL_0bce;
                                case 21:
                                    goto IL_0bee;
                                case 22:
                                    goto IL_0c1b;
                                case 23:
                                    goto IL_0c44;
                                case 24:
                                    goto IL_0c6c;
                                case 25:
                                    goto IL_0cca;
                                case 26:
                                case 27:
                                    goto IL_0d2d;
                                case 28:
                                    goto IL_0dcc;
                                default:
                                    break;
                            }

                            goto end_IL_0000_2;
                        IL_0867:
                            num = 141;
                            _Modul1.Instance.Druck_Tast = 1;
                            MyProject.Forms.Ahnen.Show();
                            goto end_IL_0000_2;
                        IL_088f:
                            num = 145;
                            MyProject.Forms.Ort.Show();
                            goto end_IL_0000_2;
                        IL_08aa:
                            num = 148;
                            MyProject.Forms.Nachlist.Hide();
                            MyProject.Forms.Nachlist.ShowDialog();
                            if (_Modul1.Instance.PersInArb != 0)
                            {
                            }
                            else
                            {

                                MyProject.Forms.Nachlist.Close();
                                Show();
                            }
                            goto end_IL_0000_2;
                        IL_090e:
                            num = 156;
                            MyProject.Forms.Ahnengem.Show();
                            goto end_IL_0000_2;
                        IL_0929:
                            num = 159;
                            MyProject.Forms.Sippenlist.Show();
                            goto end_IL_0000_2;
                        IL_0944:
                            num = 162;
                            MyProject.Forms.Hinter.Visible = true;
                            FileSystem.FileClose(99);
                            FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "init\\DruckTexte.dat", OpenMode.Input);
                            num5 = 0;
                            while (!FileSystem.EOF(99))
                            {
                                num5++;
                                _Modul1.Instance.DTxt[num5] = FileSystem.LineInput(99);
                            }
                            Module2.Drucktexte();
                            MyProject.Forms.Hinter.Frame1.Visible = true;
                            Visible = false;
                            num5 = 1;
                            while (num5 <= 15)
                            {
                                MyProject.Forms.Hinter.Text1[(short)(num5 - 1)].Text = _Modul1.Instance.DTxt[num5];
                                num5++;
                            }
                            MyProject.Forms.Hinter.TextBox4.Text = _Modul1.Instance.DTxt[16];
                            MyProject.Forms.Hinter.TextBox5.Text = _Modul1.Instance.DTxt[17];
                            goto end_IL_0000_2;
                        IL_0a98:
                            num = 181;
                            _Modul1.Instance.Schalt = 0;
                            _Modul1.Instance.Druck_Tast = 0;
                            _Modul1.Instance.Datschalt = 0;
                            MyProject.Forms.AhnenST.Show();
                            goto end_IL_0000_2;
                        IL_0ada:
                            num = 187;
                            _Modul1.Instance.Druck_Tast = 1;
                            _Modul1.Instance.Datschalt = 0;
                            MyProject.Forms.AhnenST.Show();
                            goto end_IL_0000_2;
                        IL_0b0f:
                            num = 192;
                            Debugger.Break();
                            if (_Modul1.Instance.PersInArb != 0)
                            {
                            }
                            else
                            {

                                MyProject.Forms.Nachlist.Close();
                                Show();
                            }
                            goto end_IL_0000_2;
                        IL_0b52:
                            num = 199;
                            _Modul1.Instance.Schalt = 4;
                            Debugger.Break();
                            goto end_IL_0000_2;
                        IL_0b70:
                            ProjectData.ClearProjectError();
                            num3 = 5;
                            Interaction.Shell(_Modul1.Instance.GenFreeDir + "OSB.EXE", AppWinStyle.NormalFocus);
                            ProjectData.EndApp();
                            goto end_IL_0000_2;
                        IL_0ba6:
                            num = 210;
                            _Modul1.Instance.Druck_Tast = 1;
                            MyProject.Forms.AhneneST.Show();
                            goto end_IL_0000_2;
                        IL_0bce:
                            num = 214;
                            MyProject.Forms.Stammfolge.Show();
                            goto end_IL_0000_2;
                        IL_0bee:
                            num = 218;
                            xGED = false;
                            MyProject.Forms.Ausw.Show();
                            goto end_IL_0000_2;
                        IL_0c1b:
                            num = 223;
                            _Modul1.Instance.Schalt = 22;
                            MyProject.Forms.Generatio.Show();
                            goto end_IL_0000_2;
                        IL_0c44:
                            num = 227;
                            _Modul1.Instance.Druck_Tast = 2;
                            MyProject.Forms.Ahnen.Show();
                            goto end_IL_0000_2;
                        IL_0c6c:
                            num = 231;
                            MyProject.Forms.Ort.Show();
                            MyProject.Forms.Ort.Command1[5].Visible = true;
                            MyProject.Forms.Ort.Command1[5].PerformClick();
                            goto end_IL_0000_2;
                        IL_0cca:
                            num = 236;
                            string text = "Um diese Ausgabe zu nutzen brauchen Sie das Programm >Das PLZ-Diagramm< von Klaus Wessiepe.";
                            text += "\nEine kostenlose Demoversion erhalten Sie unter";
                            text += "\n\nwww.klaus-wessiepe.de/Plz.htm";
                            text += "\n\nDas Programm erstellt eine Deutschlandkarte und zeigt zu den jeweiligen Plz die Häufigkeit der Ereignisse an";
                            Interaction.MsgBox(text);
                            goto end_IL_0000_2;
                        IL_0d2d:
                            num = 243;
                            text = "Es ist  möglich mehrere Personen und Familienblätter auf einmal zu drucken.";
                            text += "\nGeben Sie die Familien- oder Personennummern in folgender Weise ein:";
                            text += "\n1;6;45;9 druckt die Blätter 1 6 45 und 9";
                            text += "\n5-67 druckt die Blätter 5 bis 67.";
                            text += "\n";
                            text += "\nDie Aufteilung auf einzelne Blätter müssen Sie anschließend in Ihrer Textverarbeitung manuell vornehmen, ";
                            text += "\nda ich ja nicht wissen kann, welches Format Sie verwenden.";
                            Interaction.MsgBox(text);
                            goto end_IL_0000_2;
                        IL_0dcc:
                            num = 253;
                            Close();
                            Debugger.Break();
                            Debugger.Break();
                            goto end_IL_0000_2;
                        IL_0edf:
                            num = 276;
                            if (Information.Err().Number != 5)
                            {
                                break;
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_0f37;
                        IL_0f37: // <========== 4
                            num4 = unchecked(num2 + 1);
                            goto IL_0f3b;
                        IL_0f3b:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 23:
                                case 27:
                                case 28:
                                case 37:
                                case 38:
                                case 39:
                                case 44:
                                case 48:
                                case 80:
                                case 84:
                                case 85:
                                case 94:
                                case 95:
                                case 96:
                                    goto IL_0669;
                                case 102:
                                case 106:
                                case 278:
                                case 280:
                                    goto end_IL_0000_3;
                                case 10:
                                case 45:
                                case 46:
                                case 55:
                                case 56:
                                case 68:
                                case 76:
                                case 103:
                                case 104:
                                case 113:
                                case 114:
                                case 118:
                                case 121:
                                case 124:
                                case 125:
                                case 126:
                                case 139:
                                case 143:
                                case 146:
                                case 153:
                                case 154:
                                case 157:
                                case 160:
                                case 179:
                                case 185:
                                case 190:
                                case 196:
                                case 197:
                                case 201:
                                case 205:
                                case 206:
                                case 207:
                                case 208:
                                case 212:
                                case 215:
                                case 216:
                                case 220:
                                case 221:
                                case 225:
                                case 229:
                                case 234:
                                case 241:
                                case 251:
                                case 256:
                                case 257:
                                case 258:
                                case 265:
                                case 281:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                    num = 280;
                    Interaction.MsgBox(Information.Err().Number);
                    break;
                }
            end_IL_0000:
                ;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 5037;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 35
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void Bef_6_Click()
    {
        DataModul.MandDB?.Close();
        DataModul.DOSB?.Close();
        DataModul.TempDB?.Close();
        DataModul.DSB?.Close();
        Close();
        ProjectData.ClearProjectError();
        Interaction.MsgBox(_Modul1.Instance.GenFreeDir);
        int processId = Interaction.Shell(_Modul1.Instance.GenFreeDir + "Gen_Plus.exe", AppWinStyle.NormalFocus);
        Interaction.AppActivate(processId);
        ProjectData.EndApp();
        ProjectData.ClearProjectError();    
    }

    private static void Bef_4_Click()
    {
        checked
        {
            MyProject.Forms.ATMenue.Show();
        }
    }

    private static void Bef_3_Click()
    {
        checked
        {
            MyProject.Forms.FaBu.Show();
        }
    }

    private static void Bef_2_Click()
    {
        checked
        {
            _Modul1.Instance.Druck_Tast = 0;
            MyProject.Forms.Ahnen.Show();
        }
    }

    private static int Bef_1_Click(out int num, int lErl, out string text2, out short num6, out int num7, out int num8, out int num11, out int num9)
    {
        checked
        {
        IL_0398:
            num = 58;
            _Modul1.Instance.Dateienopen();
            text2 = Interaction.InputBox("Nummer der Familie, Leer/Abbruch = Suche nach Namen");
            _Modul1.Instance.FamInArb = (int)Math.Round(text2.AsDouble());
            int num10;
            if (Strings.InStr(text2, "-") != 0)
            {
                num6 = (short)Strings.InStr(text2, "-");
                num7 = (int)Math.Round(text2.Left(num6 - 1).AsDouble());
                num8 = (int)Math.Round(Strings.Mid(text2, num6 + 1, text2.Length).AsDouble());
                DataModul.DB_FamilyTable.MoveLast();
                if (Operators.ConditionalCompareObjectLess(DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].Value, num7, TextCompare: false))
                {
                    Interaction.MsgBox("Höchste Familiennummer ist " + DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsString());
                    goto end_IL_0000_2;
                }
                if (Operators.ConditionalCompareObjectLess(DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].Value, num8, TextCompare: false))
                {
                    Interaction.MsgBox("Höchste Familiennummer ist " + DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsString());
                    num8 = DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();
                }
                if (num8 < num7)
                {
                    Interaction.MsgBox("Endnummer muß größer sein als Startnummer");
                    goto end_IL_0000_2;
                }
                num9 = num7;
                num10 = num8;
                num11 = num9;
                while (num11 <= num10 && num11 <= num8)
                {
                    MyProject.Forms.Namensuch.List7.Items.Add(num11.AsString());
                    num11++;
                }
                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(MyProject.Forms.Namensuch.List7.Items[0].AsString()));
            }
            else if (Strings.InStr(text2, ";") != 0)
            {
                text2 += ";";
                while (text2.Length > 1)
                {
                    lErl = 33;
                    num6 = (short)Strings.InStr(text2, ";");
                    MyProject.Forms.Namensuch.List7.Items.Add(text2.Left(num6 - 1));
                    text2 = Strings.Mid(text2, num6 + 1, text2.Length);
                }
            }
            if (_Modul1.Instance.FamInArb == 0)
            {
                _Modul1.Instance.Schalt = 1;
                _Modul1.Instance.Suchschalt = 2;
                MyProject.Forms.Namensuch.ShowDialog();
                _Modul1.Instance.FamInArb = _Modul1.Instance.Suchfam;
            }
            if (_Modul1.Instance.FamInArb > 0)
            {
                MyProject.Forms.Namensuch.Label1[1].Text = _Modul1.Instance.FamInArb.AsString();
                _Modul1.Instance.Schalt = 4;
                MyProject.Forms.Namensuch.Hide();
                MyProject.Forms.Namensuch.Command1[2].Visible = true;
                MyProject.Forms.Namensuch.Show();
                MyProject.Forms.Namensuch.Command1[2].PerformClick();
            }

            return lErl;
        }
    }

    private static void Bef_0_Click()
    {
        string text2;
        short num6;
        int num11;

        checked
        {
            _Modul1.Instance.Dateienopen();
            text2 = Interaction.InputBox("Nummer der Person, Leer/Abbruch = Suche nach Namen");
            _Modul1.Instance.PersInArb = (int)Math.Round(text2.AsDouble());
            MyProject.Forms.Namensuch.List7.Items.Clear();
            if (Strings.InStr(text2, "-") != 0)
            {
                num6 = (short)Strings.InStr(text2, "-");
                num11 = (int)Math.Round(text2.Left(num6 - 1).AsDouble());
                while (num11 <= (int)Math.Round(Strings.Mid(text2, num6 + 1, text2.Length).AsDouble()))
                {
                    MyProject.Forms.Namensuch.List7.Items.Add(num11.AsString());
                    num11++;
                }
                _Modul1.Instance.PersInArb = MyProject.Forms.Namensuch.List7.Items[0].AsInt();
            }
            else if (Strings.InStr(text2, ";") != 0)
            {
                text2 += ";";
                while (text2.Length > 1)
                {
                    num6 = (short)Strings.InStr(text2, ";");
                    MyProject.Forms.Namensuch.List7.Items.Add(text2.Left(num6 - 1));
                    text2 = Strings.Mid(text2, num6 + 1, text2.Length);
                }
            }
            if (_Modul1.Instance.PersInArb == 0)
            {
                _Modul1.Instance.Schalt = 2;
                MyProject.Forms.Namensuch.ShowDialog();
                _Modul1.Instance.PersInArb = _Modul1.Instance.Suchper;
            }
            if (_Modul1.Instance.PersInArb != 0)
            {
                MyProject.Forms.Namensuch.Label1[0].Text = _Modul1.Instance.PersInArb.AsString();
                _Modul1.Instance.Schalt = 4;
                MyProject.Forms.Namensuch.Hide();
                MyProject.Forms.Namensuch.Command1[1].Visible = true;
                MyProject.Forms.Namensuch.Show();
                MyProject.Forms.Namensuch.Command1[1].PerformClick();
            }

        }
    }

    private void Bef_KeyPress(object eventSender, KeyPressEventArgs eventArgs)
    {
        short num = checked((short)Strings.Asc(eventArgs.KeyChar));
        int index = Bef.GetIndex((Button)eventSender);
        eventArgs.KeyChar = Strings.Chr(num);
        if (num == 0)
        {
            eventArgs.Handled = true;
        }
    }

    private void Druck_Load(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        string Value = default;
        int num2 = default;
        int num3 = default;
        byte b = default;
        short num5 = default;
        short num6 = default;
        int lErl = default;
        int Value2 = default;
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
                    string DateiName;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            Value = "";
                            goto IL_0009;
                        case 5586:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 3:
                                    case 5:
                                        goto IL_1069;
                                    case 4:
                                        goto IL_113f;
                                    case 1:
                                        goto IL_1194;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 380)
                                {
                                    _Modul1.Instance.Font1 = "Arial";
                                    _Modul1.Instance.Font2 = "Courier New";
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_1190;
                                }
                                if (Information.Err().Number == 401)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_1194;
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
                                goto IL_1190;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            xGED = true;
                            _Modul1.Instance.cMandDrive = MyProject.Computer.FileSystem.GetDriveInfo(FileSystem.CurDir());
                            Label1[0].Text = _Modul1.Instance.VersionT;
                            Label1[1].Text = _Modul1.Instance.Titel2;
                            Label1[2].Text = _Modul1.Instance.Version;
                            WindowState = _Modul1.Instance.Persistence.ReadEnumInit<FormWindowState>("Windowstate");
                            FileSystem.FileClose(6);
                            Label1[0].Width = Width;
                            FileSystem.FileClose(99);
                            var Cols = _Modul1.Instance.Persistence.ReadFarbenInit("Farb.dat", 2);
                            _Modul1.Instance.HintFarb = ColorTranslator.FromOle(0x808080);
                            _Modul1.Instance.Feld1Farb = ColorTranslator.FromOle(0xFFFFFF);
                            _Modul1.Instance.HintFarb = Cols[1];
                            _Modul1.Instance.Feld1Farb = Cols[2];
                            BackColor = _Modul1.Instance.HintFarb;
                            b = 0;
                            FileSystem.FileClose(99);

                            FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "init\\Textegerm.dat", OpenMode.Input);
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
                            goto IL_03e8;
                        IL_03e8: // <========== 12
                            num = 78;
                            Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            Label1[0].Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Bold);
                            Label1[1].Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Bold);
                            Label1[2].Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Bold);
                            _Modul1.Instance.DAus[b] = 0.AsString();
                            b = 0;
                            while (!FileSystem.EOF(99))
                            {
                                b = (byte)(unchecked(b) + 1);
                                _Modul1.Instance.IText[b] = FileSystem.LineInput(99);
                            }
                            FileSystem.FileClose(99);
                            b = 0;
                            FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "init\\DruckTexte.dat", OpenMode.Input);
                            while (!FileSystem.EOF(99))
                            {
                                b = (byte)(unchecked(b) + 1);
                                _Modul1.Instance.DTxt[b] = FileSystem.LineInput(99);
                            }
                            Module2.Drucktexte();
                            Show();
                            _Modul1.Instance.System.VerSpecial = true;
                            Label2.BackColor = _Modul1.Instance.HintFarb;
                            lblMandant.BackColor = _Modul1.Instance.HintFarb;
                            _Modul1.Instance.eWindowState = FormWindowState.Maximized;
                            _Modul1.Instance.eWindowState = _Modul1.Instance.Persistence.ReadEnumInit<FormWindowState>("Windowstate");
                            WindowState = _Modul1.Instance.eWindowState.AsEnum<FormWindowState>();
                            Show();
                            FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\Zeich.dat", OpenMode.Append);
                            FileSystem.FileClose(6);
                            if (FileSystem.FileLen(_Modul1.Instance.GenFreeDir + "\\Init\\Zeich.dat") > 0)
                            {
                                FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\Zeich.dat", OpenMode.Input);
                                FileSystem.Input(6, ref _Modul1.Instance.Font1);
                                FileSystem.Input(6, ref _Modul1.Instance.Font2);
                                FileSystem.FileClose(6);
                            }
                            Label2.Font = Label2.Font.ChangeFName(_Modul1.Instance.Font1);
                            b = 0;
                            while (unchecked(b) <= 2u)
                            {
                                Label1[b].Width = Width;
                                b = (byte)unchecked((uint)(b + 1));
                            }
                            b = 1;
                            while (unchecked(b) <= 22u)
                            {
                                Bef[b].Font = Bef[b].Font.ChangeFName(_Modul1.Instance.Font1);
                                b = (byte)unchecked((uint)(b + 1));
                            }
                            _Modul1.Instance.System.VerSpecial = true;
                            Label1[0].Text = _Modul1.Instance.VersionT;
                            Label1[1].Text = _Modul1.Instance.Titel2;
                            Label1[2].Text = _Modul1.Instance.Version;
                            BackColor = _Modul1.Instance.HintFarb;
                            ProjectData.ClearProjectError();
                            num3 = 3;
                            FileSystem.FileClose();
                            FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "\\init\\GEN-verz.ini", OpenMode.Input);
                            FileSystem.Input(99, ref _Modul1.Instance.Verz);
                            lblMandant.Font = lblMandant.Font.ChangeFName(_Modul1.Instance.Font1);
                            lblMandant.Text = "Aktueller Mandant: " + _Modul1.Instance.Verz;
                            text = _Modul1.Instance.Verz.Left(2);
                            FileSystem.FileClose();
                            _Modul1.Instance.Fehler = "Es ist ein Fehler aufgetreten. Wenden Sie sich bitte mit einer kurzen Beschreibung Ihrer Tätigkeit (z,B, Stammfolgeliste erstellen für Person 100) unter Beifügung der Datei an mich. Ich werde umgehen für Abhilfe sorgen. Gisbert Berwe.";
                            if (_Modul1.Instance.Verz.Right(1) != "\\")
                            {
                                _Modul1.Instance.Verz += "\\";
                            }
                            _Modul1.Instance.Verz1 = _Modul1.Instance.Verz.Left(15);
                            text = _Modul1.Instance.Verz.Left(2);

                            // Todo: Datei1.Path = _Modul1.Instance.GenFreeDir + "\\";

                            FileSystem.FileClose();
                            ProjectData.ClearProjectError();
                            num3 = 4;
                            FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "IDF.Dat", OpenMode.Input);
                            FileSystem.Input(99, ref Value);
                            if (Strings.Mid(Value, 3, 1).ToUpper() != "Q")
                            {
                                Interaction.MsgBox("Dieses Druck-Modul gehört nicht zu dieser Version!\rDas Programm muss beendet werden!");
                                ProjectData.EndApp();
                            }
                            ProjectData.ClearProjectError();
                            num3 = 5;
                            num5 = 0;
                            b = 1;
                            while (unchecked(b) <= 12u)
                            {
                                num5 = (short)Math.Round(num5 + Strings.Mid(Value, 14 + unchecked(b), 1).AsDouble());
                                b = (byte)unchecked((uint)(b + 1));
                            }
                            num6 = (short)Math.Round((num5 - 1) / Strings.Mid(Value, 30, 1).AsDouble());
                            if (num6 != Strings.Mid(Value, 28, 2).AsDouble())
                            {
                                Interaction.MsgBox("Lizenz wurde manipuliert\nProgramm wird beendet");
                                ProjectData.EndApp();
                            }
                            goto IL_0a3c;
                        IL_0a3c:
                            num = 163;
                            FileSystem.FileClose(99);
                            goto IL_0a58;
                        IL_0a58: // <========== 3
                            num = 164;
                            lErl = 1;
                            FileSystem.FileClose(6);
                            FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\DAK", OpenMode.Append);
                            FileSystem.FileClose(6);
                            FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\DAK", OpenMode.Input);
                            FileSystem.Input(6, ref Value2);
                            if (Value2 == 400000)
                            {
                                goto end_IL_0000_2;
                            }
                            FileSystem.FileClose(6);
                            FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\DAK", OpenMode.Append);
                            Value2 = 0;
                            FileSystem.FileClose(6);
                            FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\DAK", OpenMode.Input);
                            FileSystem.Input(6, ref Value2);
                            FileSystem.FileClose(6);
                            if (DateTime.Today.ToOADate() - Value2 > 0.0)
                            {
                                FileSystem.FileClose(99);
                                FileSystem.FileClose(6);
                                DateiName = "Update_ini.dat";
                                if (_Modul1.Instance.Persistence.ExistFileInit(DateiName))
                                {
                                    FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "\\Init\\Update_ini.dat", OpenMode.Input);
                                    FileSystem.Input(99, ref _Modul1.Instance.AutoupD);
                                    FileSystem.FileClose(99);
                                }
                                if (_Modul1.Instance.AutoupD.AsDouble() == 1.0)
                                {
                                    btnUpdateCheck.PerformClick();
                                    goto IL_0dee;
                                }
                                if (DateTime.Today.ToOADate() - Value2 > 4.0)
                                {
                                    if (Value2 < DateTime.Today.ToOADate())
                                    {
                                        pnlUpdateCheck.Visible = true;
                                        if (Value2 > 0)
                                        {
                                            if (DateTime.Today.ToOADate() - Value2 == 1.0)
                                            {
                                                Label22.Text = "vor " + (DateTime.Today.ToOADate() - Value2).AsString() + " Tag!";
                                            }
                                            else
                                            {
                                                Label22.Text = "vor " + (DateTime.Today.ToOADate() - Value2).AsString() + " Tagen!";
                                            }
                                            goto IL_0dd3;
                                        }
                                        Label22.Text = "nicht erkennbar!";

                                    }
                                }
                                goto IL_0dd3;
                            }
                            goto IL_0dee;
                        IL_0dd3: // <========== 5
                            num = 208;
                            FileSystem.FileClose(6);
                            goto IL_0dee;
                        IL_0dee: // <========== 4
                            num = 211;
                            FileSystem.FileClose(99);
                            FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "init\\Gr.dat", OpenMode.Append);
                            if (FileSystem.FileLen(_Modul1.Instance.GenFreeDir + "init\\Gr.dat") == 0)
                            {
                                FileSystem.FileClose(99);
                                _Modul1.Instance.EreiRf = false;
                                FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "init\\Gr.dat", OpenMode.Output);
                                FileSystem.PrintLine(99, _Modul1.Instance.EreiRf);
                            }
                            FileSystem.FileClose(99);
                            FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "init\\Gr.dat", OpenMode.Input);
                            FileSystem.Input(99, ref _Modul1.Instance.EreiRf);
                            FileSystem.FileClose(99);
                            if (_Modul1.Instance.EreiRf)
                            {
                                RadioButton1.Checked = true;
                                RadioButton2.Checked = false;
                            }
                            else
                            {
                                RadioButton1.Checked = false;
                                RadioButton2.Checked = true;
                            }
                            goto end_IL_0000_2;
                        IL_1069:
                            num = 247;
                            if (Information.Err().Number == 62)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1194;
                            }
                            if (Information.Err().Number == 76)
                            {
                                Interaction.MsgBox("Der aktuelle Mandant ist nicht vorhanden.Zuerst Mandanten wählen");
                                FileSystem.FileClose();
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1190;
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
                            goto IL_1190;
                        IL_113f:
                            num = 262;
                            if (Information.Err().Number != 53)
                            {
                                goto end_IL_0000_2;
                            }
                            Interaction.MsgBox("Demoversion");
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num2 = 0;
                            goto IL_0a58;
                        IL_1190: // <========== 4
                            num4 = num2;
                            goto IL_1198;
                        IL_1194: // <========== 3
                            num4 = unchecked(num2 + 1);
                            goto IL_1198;
                        IL_1198:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 22:
                                case 26:
                                case 27:
                                case 30:
                                case 34:
                                case 35:
                                case 46:
                                case 50:
                                case 53:
                                case 56:
                                case 59:
                                case 62:
                                case 65:
                                case 68:
                                case 71:
                                case 74:
                                case 77:
                                case 78:
                                    goto IL_03e8;
                                case 161:
                                case 162:
                                case 163:
                                    goto IL_0a3c;
                                case 164:
                                case 265:
                                    goto IL_0a58;
                                case 198:
                                case 201:
                                case 202:
                                case 205:
                                case 206:
                                case 207:
                                case 208:
                                    goto IL_0dd3;
                                case 190:
                                case 209:
                                case 210:
                                case 211:
                                    goto IL_0dee;
                                case 171:
                                case 226:
                                case 230:
                                case 231:
                                case 266:
                                case 267:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 5586;
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
    private void Druck_FormClosing(object eventSender, FormClosingEventArgs eventArgs)
    {
        //Discarded unreachable code: IL_0104
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
                    case 366:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0108;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_00c8;
                        }
                    IL_00c8:
                        num = 16;
                        if (Information.Err().Number != 91)
                        {
                            break;
                        }
                        goto IL_00da;
                    IL_00da:
                        num = 17;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_0108;
                    IL_00b0:
                        num = 12;
                        DataModul.DSB.Close();
                        ProjectData.EndApp();
                        goto end_IL_0000_2;
                    IL_0108:
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
                                goto IL_007f;
                            case 9:
                                goto IL_0086;
                            case 10:
                                goto IL_0094;
                            case 11:
                                goto IL_00a2;
                            case 12:
                                goto IL_00b0;
                            case 16:
                                goto IL_00c8;
                            case 17:
                                goto IL_00da;
                            case 18:
                            case 20:
                                goto end_IL_0000_3;
                            default:
                                goto end_IL_0000;
                            case 13:
                            case 14:
                            case 15:
                            case 21:
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
                        FileSystem.FileClose(6);
                        goto IL_007f;
                    IL_007f:
                        num = 8;
                        if (closeReason != 0)
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_0086;
                    IL_0086:
                        num = 9;
                        DataModul.MandDB.Close();
                        goto IL_0094;
                    IL_0094:
                        num = 10;
                        DataModul.DOSB.Close();
                        goto IL_00a2;
                    IL_00a2:
                        num = 11;
                        DataModul.TempDB.Close();
                        goto IL_00b0;
                    end_IL_0000_3:
                        break;
                }
                num = 20;
                eventArgs.Cancel = cancel;
                break;
            end_IL_0000:;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 366;
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

    private void Button1_Click(object sender, EventArgs e)
    {
        int try0000_dispatch = -1;
        int num = default;
        string Value = default;
        int num2 = default;
        int num3 = default;
        string text = default;
        string text2 = default;
        string text3 = default;
        bool flag = default;
        byte b = default;
        string inhaber = default;
        string urlString = default;
        int lErl = default;
        string documentText = default;
        byte b2 = default;
        int num5 = default;
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
                    string DateiName;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            Value = "";
                            goto IL_0009;
                        case 2083:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0665;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 76)
                                {
                                    goto end_IL_0000_2;
                                }
                                if (Information.Err().Number == 5)
                                {
                                    goto end_IL_0000_2;
                                }
                                if (Information.Err().Number == 52)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0665;
                                }
                                if (Information.Err().Number == 53)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0665;
                                }
                                if (Information.Err().Number != 62)
                                {
                                    break;
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0665;
                            }
                        end_IL_0000_3:
                            break;
                        IL_0009:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            btnUpdateCheck.Enabled = false;
                            FileSystem.FileClose(99);
                            FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "IDF.Dat", OpenMode.Input);
                            FileSystem.Input(99, ref Value);
                            text = Strings.Mid(Value, 20, 5);
                            text2 = Value;
                            text3 = Strings.Mid("Version 24.09.04 Stand 25.11.2018", 9, 8);
                            flag = false;
                            if (MyProject.Computer.Network.IsAvailable)
                            {
                                flag = MyProject.Computer.Network.Ping("www.w3.org", 1500);
                                if (flag)
                                {
                                    WebBrowser1.DocumentText = "";
                                    if (!_Modul1.Instance.Persistence.ExistFile("GPUpd1.exe"))
                                    {
                                        Interaction.MsgBox("Updater laden");
                                        download();
                                    }
                                    FileSystem.FileClose(99);
                                    FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "Adresse", OpenMode.Input);
                                    Value = "";
                                    b = 1;
                                    while (unchecked(b) <= 5u)
                                    {
                                        _Modul1.Instance.Kont[b] = FileSystem.LineInput(99);
                                        b = (byte)unchecked((uint)(b + 1));
                                    }
                                    inhaber = _Modul1.Instance.Kont[2].Trim() + " " + _Modul1.Instance.Kont[3].Trim() + " " + _Modul1.Instance.Kont[4];
                                    FileSystem.FileClose(99);
                                    _Modul1.Instance.User.Name = inhaber;
                                    if (_Modul1.Instance.User.Name.Trim() == "")
                                    {
                                        _Modul1.Instance.User.Name = "Keine Adresse";
                                    }
                                    FileSystem.FileClose(99);
                                    WebBrowser1.DocumentText = "";
                                    urlString = "http://www.Genpluswin.de/Up24/Versiondruck.php?v=" + text3 + text + Environment.MachineName + " " + _Modul1.Instance.User.Owner + " " + _Modul1.Instance.Verz + " " + text2;
                                    WebBrowser1.Navigate(urlString);
                                    goto IL_02fc;
                                }
                                goto IL_054e;
                            }
                            if (_Modul1.Instance.AutoupD != "1")
                            {
                                Interaction.MsgBox("Es besteht keine Internetverbindung!");
                            }
                            goto IL_054e;
                        IL_02fc: // <========== 3
                            num = 43;
                            lErl = 10;
                            WebBrowser1.Update();
                            Application.DoEvents();
                            documentText = WebBrowser1.DocumentText;
                            pnlUpdateCheck.Visible = false;
                            if (Strings.InStr(documentText, "nicht aktuell") > 0)
                            {
                                Interaction.MsgBox("Ihre Programmversion ist nicht aktuell.\r\nEs wird eine Aktualisierung empfohlen!", MsgBoxStyle.Exclamation, "Aktualitätskontrolle");
                                b2 = (byte)Interaction.MsgBox("Aktualisierung jetzt durchführen?", MsgBoxStyle.YesNo | MsgBoxStyle.Question, "");
                                num5 = (int)Math.Round(DateTime.Today.ToOADate());
                                FileSystem.FileClose(6);
                                FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\DAK", OpenMode.Output);
                                FileSystem.PrintLine(6, num5);
                                FileSystem.FileClose(6);
                                if (b2 == 6)
                                {
                                    Interaction.Shell(_Modul1.Instance.GenFreeDir + "\\GPUpd1.exe Druck", AppWinStyle.NormalFocus);
                                }
                                goto IL_054e;
                            }
                            if (Strings.InStr(documentText, "ist aktuell") > 0)
                            {
                                Interaction.MsgBox("Ihr Druck-Modul ist aktuell!\nEs sind keine Maßnahmen erforderlich!", MsgBoxStyle.Information, "Aktualitätskontrolle");
                                num6 = (int)Math.Round(DateTime.Today.ToOADate());
                                FileSystem.FileClose(6);
                                FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\DAK", OpenMode.Output);
                                FileSystem.PrintLine(6, num6);
                                FileSystem.FileClose(6);
                                goto IL_054e;
                            }
                            if (Strings.InStr(documentText, "Version eingestellt") > 0)
                            {
                                Interaction.MsgBox("Updates für diese Version eingestellt!\nEs gibt eine neue Version!", MsgBoxStyle.Information, "Aktualitätskontrolle");
                                goto IL_054e;
                            }
                            if (Strings.InStr(documentText, "nicht gefunden") > 0)
                            {
                                Interaction.MsgBox("Verbindung fehlgeschlagen!\nVersuchen Sie es später noch einmal!", MsgBoxStyle.Information, "Aktualitätskontrolle");
                                goto IL_054e;
                            }
                            goto IL_02fc;
                        IL_054e: // <========== 7
                            num = 82;
                            btnUpdateCheck.Enabled = true;
                            goto end_IL_0000_2;
                        IL_0665: // <========== 4
                            num4 = unchecked(num2 + 1);
                            while (true)
                            {
                                num2 = 0;
                                switch (num4)
                                {
                                    case 1:
                                        break;
                                    case 43:
                                    case 78:
                                        goto IL_02fc;
                                    case 16:
                                    case 19:
                                    case 58:
                                    case 59:
                                    case 67:
                                    case 68:
                                    case 71:
                                    case 72:
                                    case 75:
                                    case 76:
                                    case 79:
                                    case 80:
                                    case 81:
                                    case 82:
                                        goto IL_054e;
                                    case 100:
                                    case 102:
                                        goto end_IL_0000_3;
                                    case 106:
                                        goto IL_0642;
                                    case 83:
                                    case 85:
                                    case 88:
                                    case 103:
                                    case 105:
                                    case 107:
                                        goto end_IL_0000_2;
                                }
                                break;
                            IL_0642:
                                num = 106;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                            }
                            goto default;
                    }
                    num = 102;
                    if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                    {
                        ProjectData.EndApp();
                    }
                    break;
                }
            end_IL_0000:
                ;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 2083;
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

    private void download()
    {
        WebClient webClient = new WebClient();
        string uriString = "http://www.genpluswin.de/Up24/GPUpd1.exe";
        string fileName = _Modul1.Instance.GenFreeDir + "GPUpd1.exe";
        btnUpdateCheck.Enabled = false;
        try
        {
            webClient.DownloadFile(new Uri(uriString), fileName);
        }
        catch (Exception ex)
        {
            ProjectData.SetProjectError(ex);
            Exception ex2 = ex;
            Interaction.MsgBox("Fehler!\r\n" + ex2.Message, MsgBoxStyle.Exclamation);
            btnUpdateCheck.Enabled = true;
            ProjectData.ClearProjectError();
        }
    }

    private void Button25_Click(object sender, EventArgs e)
    {
        if (CheckBox1.Checked)
        {
            int num = 400000;
            FileSystem.FileClose(6);
            FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\DAK", OpenMode.Output);
            FileSystem.PrintLine(6, num);
            FileSystem.FileClose(6);
        }
        pnlUpdateCheck.Visible = false;
    }

    private void _Label1_2_Click(object sender, EventArgs e)
    {
        _Modul1.Instance.Info();
    }

    [DllImport("shell32.dll", CharSet = CharSet.Ansi, EntryPoint = "ShellExecuteA", ExactSpelling = true, SetLastError = true)]
    private static extern long ShellExecute(long hwnd, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpOperation, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpFile, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpParameters, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpDirectory, long nShowCmd);

    private void Button2_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_0085
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
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 1;
                        goto IL_0007;
                    case 196:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 1:
                                    break;
                                default:
                                    goto end_IL_0000;
                            }
                            int num4 = num2 + 1;
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_0007;
                                case 3:
                                    goto IL_0024;
                                case 6:
                                    goto IL_002b;
                                case 7:
                                    goto IL_0033;
                                case 4:
                                case 5:
                                case 8:
                                    goto IL_0048;
                                case 9:
                                    goto IL_0054;
                                case 10:
                                    goto end_IL_0000_2;
                                default:
                                    goto end_IL_0000;
                                case 11:
                                    goto end_IL_0000_3;
                            }
                            goto default;
                        }
                    IL_0033:
                        num = 7;
                        FileSystem.Input(99, ref _Modul1.Instance.Aus[b]);
                        goto IL_0048;
                    IL_0007:
                        num = 2;
                        FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "\\Init\\Druck_ini.dat", OpenMode.Input);
                        goto IL_0024;
                    IL_0024:
                        num = 3;
                        b = 0;
                        goto IL_0048;
                    IL_0048:
                        num = 5;
                        if (!FileSystem.EOF(99))
                        {
                            goto IL_002b;
                        }
                        goto IL_0054;
                    IL_0054:
                        num = 9;
                        FileSystem.FileClose(99);
                        break;
                    IL_002b:
                        num = 6;
                        b = checked((byte)(unchecked(b) + 1));
                        goto IL_0033;
                    end_IL_0000_2:
                        break;
                }
                num = 10;
                Process.Start(_Modul1.Instance.GenFreeDir + "\\Hilfe\\TeilB.PDF");
                break;
            end_IL_0000:;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 196;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_3:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void RadioButton_Click(object sender, EventArgs e)
    {
        if (RadioButton2.Checked)
        {
            _Modul1.Instance.EreiRf = false;
        }
        else
        {
            _Modul1.Instance.EreiRf = true;
        }
        FileSystem.FileClose(99);
        FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "init\\Gr.dat", OpenMode.Output);
        FileSystem.PrintLine(99, _Modul1.Instance.EreiRf);
    }

    private void Button3_Click(object sender, EventArgs e)
    {
        MyProject.Forms.Generatio.Show();
    }
}
