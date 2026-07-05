using BaseLib.Helper;
using Druck.My;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Sys;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Druck
{
    [StandardModule]
    internal sealed class Module2
    {
        public static string Endwert;
        public static string sCode;
        public static IModul1 Modul1;
        public static string Koelner_Phonetic(string Eingabe)
        {
            Eingabe = Eingabe.Trim();
            Eingabe = Strings.LCase(Eingabe);
            Eingabe = Eingabe.Replace("ph", "f");
            Eingabe = Eingabe.Replace("ü", "u");
            Eingabe = Eingabe.Replace("ä", "a");
            Eingabe = Eingabe.Replace("ö", "o");
            Eingabe = Eingabe.Replace("ß", "ss");
            Eingabe = "#" + Eingabe;
            Eingabe += "#";
            checked
            {
                int num = Eingabe.Length - 2;
                string text2 = default;
                for (int i = 1; i <= num; i++)
                {
                    string kette = Strings.Mid(Eingabe, i, 3);
                    string text = (i != 1) ? Conv_Rest(kette) : Conv_Ersten(kette);
                    text2 += text;
                }
                text2 = text2.Replace("-", "");
                text2 = (text2.Left(1) != "0") ? text2.Replace("0", "") : ("0" + text2.Replace("0", ""));
                Endwert = text2.Left(1);
                int num2 = text2.Length;
                for (int i = 2; i <= num2; i++)
                {
                    if (Strings.Mid(text2, i, 1) != Endwert.Right(1))
                    {
                        Endwert += Strings.Mid(text2, i, 1);
                    }
                }
                Endwert = Endwert + "00000000".Left(6);
                return Endwert;
            }
        }

        public static string Conv_Ersten(string Kette)
        {
            if (Strings.Mid(Kette, 2, 1) == "a")
            {
                return "0";
            }
            if (Strings.Mid(Kette, 2, 1) == "e")
            {
                return "0";
            }
            if (Strings.Mid(Kette, 2, 1) == "i")
            {
                return "0";
            }
            if (Strings.Mid(Kette, 2, 1) == "j")
            {
                return "0";
            }
            if (Strings.Mid(Kette, 2, 1) == "y")
            {
                return "0";
            }
            if (Strings.Mid(Kette, 2, 1) == "o")
            {
                return "0";
            }
            if (Strings.Mid(Kette, 2, 1) == "u")
            {
                return "0";
            }
            if (Strings.Mid(Kette, 2, 2) == "ca")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 2) == "ch")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 2) == "ck")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 2) == "cl")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 2) == "co")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 2) == "cq")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 2) == "cr")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 2) == "cu")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 2) == "cx")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 1) == "c")
            {
                return 8.AsString();
            }
            return Conv_Rest(Kette);
        }

        public static string Conv_Rest(string Kette)
        {
            if (Strings.Mid(Kette, 2, 2) == "ds")
            {
                return "8";
            }
            if (Strings.Mid(Kette, 2, 2) == "dc")
            {
                return "8";
            }
            if (Strings.Mid(Kette, 2, 2) == "dz")
            {
                return "8";
            }
            if (Strings.Mid(Kette, 2, 2) == "ts")
            {
                return "8";
            }
            if (Strings.Mid(Kette, 2, 2) == "tc")
            {
                return "8";
            }
            if (Strings.Mid(Kette, 2, 2) == "tz")
            {
                return "8";
            }
            if (Strings.Mid(Kette, 2, 1) == "d")
            {
                return "2";
            }
            if (Strings.Mid(Kette, 2, 1) == "t")
            {
                return "2";
            }
            if (Kette.Left(2) == "cx")
            {
                return "8";
            }
            if (Kette.Left(2) == "kx")
            {
                return "8";
            }
            if (Kette.Left(2) == "qx")
            {
                return "8";
            }
            if (Strings.Mid(Kette, 2, 1) == "x")
            {
                return "48";
            }
            if (Kette.Left(2) == "sc")
            {
                return "8";
            }
            if (Kette.Left(2) == "sz")
            {
                return "8";
            }
            if (Strings.Mid(Kette, 2, 2) == "ca")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 2) == "co")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 2) == "cu")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 2) == "ch")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 2) == "ck")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 2) == "cx")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 2) == "cq")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 1) == "c")
            {
                return "8";
            }
            if (Strings.Mid(Kette, 2, 1) == "a")
            {
                return "0";
            }
            if (Strings.Mid(Kette, 2, 1) == "e")
            {
                return "0";
            }
            if (Strings.Mid(Kette, 2, 1) == "i")
            {
                return "0";
            }
            if (Strings.Mid(Kette, 2, 1) == "j")
            {
                return "0";
            }
            if (Strings.Mid(Kette, 2, 1) == "y")
            {
                return "0";
            }
            if (Strings.Mid(Kette, 2, 1) == "o")
            {
                return "0";
            }
            if (Strings.Mid(Kette, 2, 1) == "u")
            {
                return "0";
            }
            if (Strings.Mid(Kette, 2, 1) == "h")
            {
                return "-";
            }
            if (Strings.Mid(Kette, 2, 1) == "l")
            {
                return "5";
            }
            if (Strings.Mid(Kette, 2, 1) == "r")
            {
                return "7";
            }
            if (Strings.Mid(Kette, 2, 1) == "m")
            {
                return "6";
            }
            if (Strings.Mid(Kette, 2, 1) == "n")
            {
                return "6";
            }
            if (Strings.Mid(Kette, 2, 1) == "s")
            {
                return "8";
            }
            if (Strings.Mid(Kette, 2, 1) == "z")
            {
                return "8";
            }
            if (Strings.Mid(Kette, 2, 1) == "b")
            {
                return "1";
            }
            if (Strings.Mid(Kette, 2, 1) == "p")
            {
                return "1";
            }
            if (Strings.Mid(Kette, 2, 1) == "f")
            {
                return "3";
            }
            if (Strings.Mid(Kette, 2, 1) == "v")
            {
                return "3";
            }
            if (Strings.Mid(Kette, 2, 1) == "w")
            {
                return "3";
            }
            if (Strings.Mid(Kette, 2, 1) == "g")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 1) == "k")
            {
                return "4";
            }
            if (Strings.Mid(Kette, 2, 1) == "q")
            {
                return "4";
            }
            return "?";
        }

        public static string GetSoundEx(string sName)
        {
            if (sName.Length == 0)
            {
                return "";
            }
            sName = sName.ToUpper().Replace("ß", "SS");
            sCode = sName.Substring(0, 1);
            string text = "";
            checked
            {
                int num = sName.Length - 1;
                for (int i = 1; i <= num; i++)
                {
                    switch (sName.Substring(i, 1))
                    {
                        case "B":
                        case "F":
                        case "P":
                        case "V":
                            text = "1";
                            break;
                        case "C":
                        case "G":
                        case "J":
                        case "K":
                        case "Q":
                        case "S":
                        case "X":
                        case "Z":
                            text = "2";
                            break;
                        case "D":
                        case "T":
                            text = "3";
                            break;
                        case "L":
                            text = "4";
                            break;
                        case "M":
                        case "N":
                            text = "5";
                            break;
                        case "R":
                            text = "6";
                            break;
                    }
                    if (sCode.Length == 1 || !sCode.EndsWith(text))
                    {
                        sCode += text;
                    }
                }
                sCode = sCode + "0000".Left(4);
                return sCode;
            }
        }

        public static string Jobdreh(string Job)
        {
            byte b = 1;
            do
            {
                Modul1.Kont1[b] = Modul1.Kont1[b].Trim();
                if (Modul1.Kont1[b] != "")
                {
                    Modul1.Kont1[b] = Modul1.Kont1[b] + " ";
                }
                checked
                {
                    b = (byte)unchecked((uint)(b + 1));
                }
            }
            while (b <= 7u);
            if (Modul1.EreiRf)
            {
                if (Modul1.DAus[112].AsDouble() == 0.0)
                {
                    Job = (Modul1.Kont1[4] + Modul1.Kont1[1] + Modul1.Kont1[3] + Modul1.Kont1[7] + Modul1.Kont1[6] + Modul1.Kont1[8] + " " + Modul1.Kont1[5]).Trim();
                }
                if (Modul1.DAus[112].AsDouble() == 1.0)
                {
                    Job = (Modul1.Kont1[4] + Modul1.Kont1[5] + Modul1.Kont1[1]).Trim() + " " + Modul1.Kont1[3] + Modul1.Kont1[7] + Modul1.Kont1[6] + Modul1.Kont1[8];
                }
                Job = Job.Replace("  ", " ");
            }
            else
            {
                if (Modul1.DAus[112].AsDouble() == 0.0)
                {
                    Job = (Modul1.Kont1[4] + Modul1.Kont1[1] + Modul1.Kont1[3] + Modul1.Kont1[6] + Modul1.Kont1[7] + Modul1.Kont1[5] + " " + Modul1.Kont1[8]).Trim();
                }
                if (Modul1.DAus[112].AsDouble() == 1.0)
                {
                    Job = (Modul1.Kont1[4] + Modul1.Kont1[7] + Modul1.Kont1[5] + Modul1.Kont1[1]).Trim() + " " + Modul1.Kont1[3] + Modul1.Kont1[6] + Modul1.Kont1[8];
                }
                Job = Job.Replace("  ", " ");
            }
            return Job;
        }

        public static string Repoles(int iRepo)
        {
            var Sta = "";
            IRecordset dB_RepoTab = DataModul.DB_RepoTab;
            IRecordset dB_RepoTable = DataModul.DB_RepoTable;
            dB_RepoTab.Index = "Nr";
            dB_RepoTab.Seek("=", iRepo);
            while (!dB_RepoTab.NoMatch 
                && !dB_RepoTab.EOF)
            {
                if (!DataModul.Repositories.ReadData(dB_RepoTab.Fields[RepoTabFields.Repo].AsInt(), out var repoData) ||
                    repoData.ID != iRepo)
                {
                    break;
                }
                byte b = 0;
                do
                {
                    Modul1.Kont1[b] = "";
                    b = (byte)unchecked((uint)(b + 1));
                }
                while (unchecked(b) <= 6u);

                byte b2 = 0;
                if (dB_RepoTable.Fields["Name"].AsString().Trim() != "")
                {
                    Modul1.Kont1[0] = dB_RepoTable.Fields["Name"].AsString().Trim() + ", ";
                    b2 = 1;
                }
                if (dB_RepoTable.Fields["Strasse"].AsString().Trim() != "")
                {
                    Modul1.Kont1[1] = dB_RepoTable.Fields["Strasse"].AsString().Trim() + ", ";
                    b2 = 1;
                }
                if (dB_RepoTable.Fields["Ort"].AsString().Trim() != "")
                {
                    Modul1.Kont1[2] = dB_RepoTable.Fields["Ort"].AsString().Trim() + ", ";
                    b2 = 1;
                }
                if (Operators.CompareString(dB_RepoTable.Fields["Plz"].AsString().Trim(), "", TextCompare: false) != 0)
                {
                    Modul1.Kont1[3] = dB_RepoTable.Fields["Plz"].AsString().Trim() + " ";
                    b2 = 1;
                }
                if (Operators.CompareString(dB_RepoTable.Fields["Fon"].AsString().Trim(), "", TextCompare: false) != 0)
                {
                    Modul1.Kont1[4] = dB_RepoTable.Fields["Fon"].AsString().Trim() + ", ";
                    b2 = 1;
                }
                if (Operators.CompareString(dB_RepoTable.Fields["Mail"].AsString().Trim(), "", TextCompare: false) != 0)
                {
                    Modul1.Kont1[5] = dB_RepoTable.Fields["Mail"].AsString().Trim() + ", ";
                    b2 = 1;
                }
                if (b2 > 0)
                {
                    Sta = Sta + "Standort: " + Modul1.Kont1[0] + Modul1.Kont1[1] + Modul1.Kont1[3] + Modul1.Kont1[2] + Modul1.Kont1[4] + Modul1.Kont1[5] + Modul1.Kont1[6].TrimEnd();
                    Sta = Sta.Left(Sta.TrimEnd().Length - 1);
                    Sta += ".\n";
                }
                dB_RepoTab.MoveNext();
            }

            return Sta;
        }

        public static void Drucktexte()
        {
            if (Modul1.DTxt[1] == "")
            {
                Modul1.DTxt[1] = "Geboren";
            }
            if (Modul1.DTxt[2] == "")
            {
                Modul1.DTxt[2] = "Getauft";
            }
            if (Modul1.DTxt[3] == "")
            {
                Modul1.DTxt[3] = "†";
            }
            if (Modul1.DTxt[4] == "")
            {
                Modul1.DTxt[4] = "Begraben";
            }
            if (Modul1.DTxt[5] == "")
            {
                Modul1.DTxt[5] = "Proklamation";
            }
            if (Modul1.DTxt[6] == "")
            {
                Modul1.DTxt[6] = "Verlobung";
            }
            if (Modul1.DTxt[7] == "")
            {
                Modul1.DTxt[7] = "Heirat";
            }
            if (Modul1.DTxt[8] == "")
            {
                Modul1.DTxt[8] = "Kirchliche Heirat";
            }
            if (Modul1.DTxt[9] == "")
            {
                Modul1.DTxt[9] = "Scheidung";
            }
            if (Modul1.DTxt[10] == "")
            {
                Modul1.DTxt[10] = "Eheähnliche Beziehung";
            }
            if (Modul1.DTxt[11] == "")
            {
                Modul1.DTxt[11] = "Verbindung:";
            }
            if (Modul1.DTxt[12] == "")
            {
                Modul1.DTxt[12] = "Aussereheliche Verbindung";
            }
            if (Modul1.DTxt[13] == "")
            {
                Modul1.DTxt[13] = "oo? ";
            }
            if (Modul1.DTxt[14] == "")
            {
                Modul1.DTxt[14] = "gesperrt";
            }
            if (Modul1.DTxt[15] == "")
            {
                Modul1.DTxt[15] = "Dimissiorale";
            }
            if (Modul1.DTxt[16] == "")
            {
                Modul1.DTxt[16] = "Bemerkungen";
            }
            if (Modul1.DTxt[17] == "")
            {
                Modul1.DTxt[17] = "Familienbemerkungen";
            }
            if (Modul1.DTxt[1] == ".")
            {
                Modul1.DTxt[1] = "";
            }
            if (Modul1.DTxt[2] == ".")
            {
                Modul1.DTxt[2] = "";
            }
            if (Modul1.DTxt[3] == ".")
            {
                Modul1.DTxt[3] = "";
            }
            if (Modul1.DTxt[4] == ".")
            {
                Modul1.DTxt[4] = "";
            }
            if (Modul1.DTxt[5] == ".")
            {
                Modul1.DTxt[5] = "";
            }
            if (Modul1.DTxt[6] == ".")
            {
                Modul1.DTxt[6] = "";
            }
            if (Modul1.DTxt[7] == ".")
            {
                Modul1.DTxt[7] = "";
            }
            if (Modul1.DTxt[8] == ".")
            {
                Modul1.DTxt[8] = "";
            }
            if (Modul1.DTxt[9] == ".")
            {
                Modul1.DTxt[9] = "";
            }
            if (Modul1.DTxt[10] == ".")
            {
                Modul1.DTxt[10] = "";
            }
            if (Modul1.DTxt[11] == ".")
            {
                Modul1.DTxt[11] = "";
            }
            if (Modul1.DTxt[12] == ".")
            {
                Modul1.DTxt[12] = "";
            }
            if (Modul1.DTxt[13] == ".")
            {
                Modul1.DTxt[13] = "";
            }
            if (Modul1.DTxt[14] == ".")
            {
                Modul1.DTxt[14] = "";
            }
            if (Modul1.DTxt[15] == ".")
            {
                Modul1.DTxt[15] = "";
            }
            if (Modul1.DTxt[16] == ".")
            {
                Modul1.DTxt[16] = "";
            }
            if (Modul1.DTxt[17] == ".")
            {
                Modul1.DTxt[17] = "";
            }
        }

        public static void Bildaus(string BiKe, string Form)
        {
            int try0000_dispatch = -1;
            int num3 = default;
            int num2 = default;
            int num = default;
            string text = default;
            string text2 = default;
            int lErl = default;
            int num5 = default;
            string text3 = default;
            FileStream fileStream = default;
            Bitmap bitmap = default;
            PictureBox pictureBox = default;
            Image image = default;
            string left = default;
            FileStream fileStream2 = default;
            PictureBox pictureBox2 = default;
            Image image2 = default;
            string left2 = default;
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
                        case 6846:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_1678;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 76)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num2 = 0;
                                    goto IL_1518;
                                }
                                if (Information.Err().Number == 53)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num2 = 0;
                                    goto IL_1518;
                                }
                                if (Information.Err().Number == 5)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num2 = 0;
                                    goto IL_1518;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString() + "Bildaus", MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num2 = 0;
                                goto IL_1518;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            text = "";
                            text2 = "";
                            if ((Modul1.DAus[115] == "1") | (Modul1.DAus[116] == "1"))
                            {
                                DataModul.DB_PictureTable.Index = "Perkenn";
                                if (BiKe == "P")
                                {
                                    num5 = Modul1.PersInArb;
                                    goto IL_008c;
                                }
                                num5 = Modul1.FamInArb;
                                goto IL_008c;
                            }
                            goto IL_1549;
                        IL_008c: // <========== 3
                            num = 12;
                            DataModul.DB_PictureTable.Seek("=", BiKe, num5);
                            goto IL_0aa3;
                        IL_0231: // <========== 3
                            num = 27;
                            if ((Modul1.DAus[116] == "1") | (Modul1.DAus[115] == "1"))
                            {
                                if (((DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].Value == "Personenbild") | (DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].Value == "Familienbild")).AsBool())
                                {
                                    text3 = text3.Replace("#", "");
                                    fileStream2 = new FileStream(text3, FileMode.Open);
                                    bitmap = new Bitmap(fileStream2);
                                    fileStream2.Close();
                                    if (Modul1.DAus[117] == "")
                                    {
                                        Modul1.DAus[117] = 100.AsString();
                                    }
                                    if (Modul1.DAus[119].AsDouble() == 0.0)
                                    {
                                        pictureBox2 = MyProject.Forms.AW.PictureBox1;
                                        pictureBox2.Image = Modul1.AutoSizeImage(bitmap, pictureBox2.ClientRectangle.Width, pictureBox2.ClientRectangle.Height);
                                        pictureBox2.Image = Modul1.PicResizeByWidth(bitmap, Modul1.DAus[117].AsInt());
                                        pictureBox2 = null;
                                        image2 = MyProject.Forms.AW.PictureBox1.Image;
                                        goto IL_03dc;
                                    }
                                    image2 = bitmap;
                                    goto IL_03dc;
                                }
                            }
                            goto IL_0a92;
                        IL_03dc: // <========== 3
                            num = 46;
                            Clipboard.SetImage(image2);
                            if (!Information.IsDBNull(DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].Value))
                            {
                                text = ('\n' + DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].AsString());
                            }
                            if (!Information.IsDBNull(DataModul.DB_PictureTable.Fields[PictureFields.Bem].Value))
                            {
                                text2 = DataModul.DB_PictureTable.Fields[PictureFields.Bem].AsString();
                            }
                            left2 = Form;
                            if (left2 == "FaBu")
                            {
                                if (text.Trim() != "")
                                {
                                    MyProject.Forms.FaBu.Anz[0].SelectedText = "\n";
                                    MyProject.Forms.FaBu.Anz[0].Paste();
                                    MyProject.Forms.FaBu.Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    if (text2.Trim() != "")
                                    {
                                        text = "";
                                    }
                                    if (text.Trim() != "")
                                    {
                                        MyProject.Forms.FaBu.Anz[0].SelectedText = text;
                                    }
                                    if (text2.Trim() != "")
                                    {
                                        MyProject.Forms.FaBu.Anz[0].SelectedText = text2;
                                    }
                                    MyProject.Forms.FaBu.Retweg2(MyProject.Forms.FaBu.Anz[0]);
                                    MyProject.Forms.FaBu.Anz[0].SelectedText = "\n";
                                }
                                goto IL_0a86;
                            }
                            if (left2 == "generatio")
                            {
                                if (text.Trim() != "")
                                {
                                    MyProject.Forms.Generatio.RtB[0].SelectedText = "\n";
                                    MyProject.Forms.Generatio.RtB[0].Paste();
                                    MyProject.Forms.Generatio.RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    if (text2.Trim() != "")
                                    {
                                        text = "";
                                    }
                                    if (text.Trim() != "")
                                    {
                                        MyProject.Forms.Generatio.RtB[0].SelectedText = text;
                                    }
                                    if (text2.Trim() != "")
                                    {
                                        MyProject.Forms.Generatio.RtB[0].SelectedText = text2;
                                    }
                                    MyProject.Forms.Generatio.Retweg2();
                                    MyProject.Forms.Generatio.RtB[0].SelectedText = "\n";
                                }
                                goto IL_0a86;
                            }
                            if (left2 == "Ahnen")
                            {
                                if ((text.Trim() != "") | (text2.Trim() != ""))
                                {
                                    MyProject.Forms.Ahnen.Anz[0].SelectedText = "\n";
                                    MyProject.Forms.Ahnen.Anz[0].Paste();
                                    MyProject.Forms.Ahnen.Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    if (text2.Trim() != "")
                                    {
                                        text = "";
                                    }
                                    if (text.Trim() != "")
                                    {
                                        MyProject.Forms.Ahnen.Anz[0].SelectedText = text;
                                    }
                                    if (text2.Trim() != "")
                                    {
                                        MyProject.Forms.Ahnen.Anz[0].SelectedText = text2;
                                    }
                                    MyProject.Forms.Ahnen.Retweg2();
                                    MyProject.Forms.Ahnen.Anz[0].SelectedText = "\n";
                                }
                                goto IL_0a86;
                            }
                            if (left2 == "STF")
                            {
                                if (text.Trim() != "")
                                {
                                    MyProject.Forms.Stammfolge.Anz[0].SelectedText = "\n";
                                    MyProject.Forms.Stammfolge.Anz[0].Paste();
                                    MyProject.Forms.Stammfolge.Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    if (text2.Trim() != "")
                                    {
                                        text = "";
                                    }
                                    if (text.Trim() != "")
                                    {
                                        MyProject.Forms.Stammfolge.Anz[0].SelectedText = text;
                                    }
                                    if (text2.Trim() != "")
                                    {
                                        MyProject.Forms.Stammfolge.Anz[0].SelectedText = text2;
                                    }
                                    MyProject.Forms.Stammfolge.Retweg2();
                                    MyProject.Forms.Stammfolge.Anz[0].SelectedText = "\n";
                                    goto IL_0a86;
                                }
                            }
                            goto IL_0a86;
                        IL_0a86: // <========== 6
                            num = 128;
                            Clipboard.Clear();
                            goto IL_0a92;
                        IL_0a92: // <========== 4
                            num = 132;
                            DataModul.DB_PictureTable.MoveNext();
                            goto IL_0aa3;
                        IL_0aa3: // <========== 3
                            num = 14;
                            if (!DataModul.DB_PictureTable.EOF)
                            {
                                if (!DataModul.DB_PictureTable.NoMatch)
                                {
                                    text = "";
                                    text2 = "";
                                    if (!(DataModul.DB_PictureTable.Fields[PictureFields.ZuNr].AsInt() != num5))
                                    {
                                        if (DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Left(1) == "#")
                                        {
                                            text3 = Conversions.ToString(Modul1.Verz + DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Left(DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Length) + DataModul.DB_PictureTable.Fields[PictureFields.Datei].Value);
                                            goto IL_0231;
                                        }
                                        text3 = (DataModul.DB_PictureTable.Fields[PictureFields.Pfad].Value + DataModul.DB_PictureTable.Fields[PictureFields.Datei].AsString());
                                        goto IL_0231;
                                    }
                                    goto IL_0ab6;
                                }
                                goto IL_0a92;
                            }
                            goto IL_0ab6;
                        IL_0ab6: // <========== 3
                            num = 134;
                            DataModul.DB_PictureTable.Seek("=", BiKe, num5);
                            goto IL_1533;
                        IL_0c76: // <========== 3
                            num = 149;
                            if (Modul1.DAus[115] == "1")
                            {
                                text3 = text3.Replace("#", "");
                                if (!Information.IsDBNull(DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].Value))
                                {
                                    if (!((DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].Value == "Personenbild") | (DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].Value == "Familienbild")).AsBool())
                                    {
                                        if (!(DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].AsString() == "Personenbild"))
                                        {
                                            goto IL_0d74;
                                        }
                                    }
                                    goto IL_1518;
                                }
                                goto IL_0d74;
                            }
                            goto IL_1518;
                        IL_0d74: // <========== 3
                            num = 159;
                            fileStream = new FileStream(text3, FileMode.Open);
                            bitmap = new Bitmap(fileStream);
                            fileStream.Close();
                            if (Modul1.DAus[119].AsDouble() == 0.0)
                            {
                                pictureBox = MyProject.Forms.AW.PictureBox1;
                                pictureBox.Image = Modul1.AutoSizeImage(bitmap, pictureBox.ClientRectangle.Width, pictureBox.ClientRectangle.Height);
                                pictureBox.Image = Modul1.PicResizeByWidth(bitmap, Modul1.DAus[117].AsInt());
                                pictureBox = null;
                                image = MyProject.Forms.AW.PictureBox1.Image;
                                goto IL_0e65;
                            }
                            image = bitmap;
                            goto IL_0e65;
                        IL_0e65: // <========== 3
                            num = 172;
                            Clipboard.SetImage(image);
                            if (!Information.IsDBNull(DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].Value))
                            {
                                text = ('\n' + DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].AsString());
                            }
                            if (!Information.IsDBNull(DataModul.DB_PictureTable.Fields[PictureFields.Bem].Value))
                            {
                                text2 = ('\n' + DataModul.DB_PictureTable.Fields[PictureFields.Bem].AsString());
                            }
                            left = Form;
                            if (left == "FaBu")
                            {
                                if (text.Trim() != "")
                                {
                                    MyProject.Forms.FaBu.Anz[0].SelectedText = "\n";
                                    MyProject.Forms.FaBu.Anz[0].Paste();
                                    MyProject.Forms.FaBu.Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    if (text.Trim() != "")
                                    {
                                        MyProject.Forms.FaBu.Anz[0].SelectedText = text;
                                    }
                                    if (text2.Trim() != "")
                                    {
                                        MyProject.Forms.FaBu.Anz[0].SelectedText = text2;
                                    }
                                    MyProject.Forms.FaBu.Retweg2(MyProject.Forms.FaBu.Anz[0]);
                                    MyProject.Forms.FaBu.Anz[0].SelectedText = "\n";
                                }
                                goto IL_150c;
                            }
                            if (left == "generatio")
                            {
                                if (text.Trim() != "")
                                {
                                    MyProject.Forms.Generatio.RtB[0].SelectedText = "\n";
                                    MyProject.Forms.Generatio.RtB[0].Paste();
                                    MyProject.Forms.Generatio.RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    if (text.Trim() != "")
                                    {
                                        MyProject.Forms.Generatio.RtB[0].SelectedText = text;
                                    }
                                    if (text2.Trim() != "")
                                    {
                                        MyProject.Forms.Generatio.RtB[0].SelectedText = text2;
                                    }
                                    MyProject.Forms.Generatio.Retweg2();
                                    MyProject.Forms.Generatio.RtB[0].SelectedText = "\n";
                                }
                                goto IL_150c;
                            }
                            if (left == "Ahnen")
                            {
                                if (text.Trim() != "")
                                {
                                    MyProject.Forms.Ahnen.Anz[0].SelectedText = "\n";
                                    MyProject.Forms.Ahnen.Anz[0].Paste();
                                    MyProject.Forms.Ahnen.Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    if (text.Trim() != "")
                                    {
                                        MyProject.Forms.Ahnen.Anz[0].SelectedText = text;
                                    }
                                    if (text2.Trim() != "")
                                    {
                                        MyProject.Forms.Ahnen.Anz[0].SelectedText = text2;
                                    }
                                    MyProject.Forms.Ahnen.Retweg2();
                                    MyProject.Forms.Ahnen.Anz[0].SelectedText = "\n";
                                }
                                goto IL_150c;
                            }
                            if (left == "STF")
                            {
                                if (text.Trim() != "")
                                {
                                    MyProject.Forms.Stammfolge.Anz[0].SelectedText = "\n";
                                    MyProject.Forms.Stammfolge.Anz[0].Paste();
                                    MyProject.Forms.Stammfolge.Anz[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    if (text.Trim() != "")
                                    {
                                        MyProject.Forms.Stammfolge.Anz[0].SelectedText = text;
                                    }
                                    if (text2.Trim() != "")
                                    {
                                        MyProject.Forms.Stammfolge.Anz[0].SelectedText = text2;
                                    }
                                    MyProject.Forms.Stammfolge.Retweg2();
                                    MyProject.Forms.Stammfolge.Anz[0].SelectedText = "\n";
                                    goto IL_150c;
                                }
                            }
                            goto IL_150c;
                        IL_150c: // <========== 6
                            num = 242;
                            Clipboard.Clear();
                            goto IL_1518;
                        IL_1518: // <========== 9
                            num = 245;
                            lErl = 5;
                            DataModul.DB_PictureTable.MoveNext();
                            goto IL_1533;
                        IL_1533: // <========== 3
                            num = 136;
                            if (!DataModul.DB_PictureTable.EOF)
                            {
                                if (!DataModul.DB_PictureTable.NoMatch)
                                {
                                    text = "";
                                    text2 = "";
                                    if (!(DataModul.DB_PictureTable.Fields[PictureFields.ZuNr].AsInt() != num5))
                                    {
                                        if (DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Left(1) == "#")
                                        {
                                            text3 = Conversions.ToString(Modul1.Verz + DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Left(DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Length) + DataModul.DB_PictureTable.Fields[PictureFields.Datei].Value);
                                            goto IL_0c76;
                                        }
                                        text3 = (DataModul.DB_PictureTable.Fields[PictureFields.Pfad].Value + DataModul.DB_PictureTable.Fields[PictureFields.Datei].AsString());
                                        goto IL_0c76;
                                    }
                                    goto IL_1549;
                                }
                                goto IL_1518;
                            }
                            goto IL_1549;
                        IL_1549: // <========== 4
                            num = 249;
                            lErl = 3;
                            goto end_IL_0000_2;
                        IL_1678:
                            num4 = num2 + 1;
                            while (true)
                            {
                                num2 = 0;
                                switch (num4)
                                {
                                    case 1:
                                        break;
                                    case 8:
                                    case 11:
                                    case 12:
                                        goto IL_008c;
                                    case 23:
                                    case 26:
                                    case 27:
                                        goto IL_0231;
                                    case 42:
                                    case 45:
                                    case 46:
                                        goto IL_03dc;
                                    case 54:
                                    case 72:
                                    case 73:
                                    case 90:
                                    case 91:
                                    case 108:
                                    case 109:
                                    case 126:
                                    case 127:
                                    case 128:
                                        goto IL_0a86;
                                    case 129:
                                    case 130:
                                    case 131:
                                    case 132:
                                        goto IL_0a92;
                                    case 13:
                                    case 14:
                                    case 133:
                                        goto IL_0aa3;
                                    case 19:
                                    case 134:
                                        goto IL_0ab6;
                                    case 145:
                                    case 148:
                                    case 149:
                                        goto IL_0c76;
                                    case 157:
                                    case 158:
                                    case 159:
                                        goto IL_0d74;
                                    case 168:
                                    case 171:
                                    case 172:
                                        goto IL_0e65;
                                    case 180:
                                    case 195:
                                    case 196:
                                    case 210:
                                    case 211:
                                    case 225:
                                    case 226:
                                    case 240:
                                    case 241:
                                    case 242:
                                        goto IL_150c;
                                    case 153:
                                    case 156:
                                    case 243:
                                    case 244:
                                    case 245:
                                    case 253:
                                    case 257:
                                    case 261:
                                    case 267:
                                        goto IL_1518;
                                    case 135:
                                    case 136:
                                    case 247:
                                        goto IL_1533;
                                    case 141:
                                    case 248:
                                    case 249:
                                        goto IL_1549;
                                    case 268:
                                        goto IL_1652;
                                }
                                break;
                            IL_1652:
                                num = 268;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                            }
                            goto default;
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj, lErl);
                    try0000_dispatch = 6846;
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
        public static void TotPrüf1(ref byte Tot)
        {
            Tot = 1;
            Modul1.Datklein = 30000000;
            DataModul.DB_EventTable.Index = "ArtNr";
            Modul1.Ubg = 103;
            DataModul.DB_EventTable.Seek("=", Modul1.Ubg.AsString(), Modul1.PersInArb.AsString(), "0");
            if (DataModul.DB_EventTable.NoMatch)
            {
                Modul1.Ubg = 104;
                DataModul.DB_EventTable.Seek("=", Modul1.Ubg.AsString(), Modul1.PersInArb.AsString(), "0");
                if (!DataModul.DB_EventTable.NoMatch)
                {
                    Modul1.Datklein = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsInt();
                }
            }
            else
            {
                if (DataModul.DB_EventTable.Fields[EventFields.DatumV].AsInt() > 0)
                {
                    Modul1.Datklein = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsInt();
                }
                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.tot].Value) && Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.tot].AsString().Trim(), "", TextCompare: false) != 0)
                {
                    Tot = 2;
                }
            }
            if (Modul1.Datklein < Modul1.DAus[123].AsDouble())
            {
                Tot = 2;
            }
            if ((Modul1.Datklein != 30000000) & (Modul1.Datklein > Modul1.DAus[123].AsDouble()))
            {
                Tot = 3;
            }
        }

        public static void Datcheck()
        {
            //Discarded unreachable code: IL_0108
            Modul1.Datklein = 0;
            DataModul.DB_EventTable.Index = "ArtNr";
            Modul1.Ubg = 101;
            DataModul.DB_EventTable.Seek("=", Modul1.Ubg.AsString(), Modul1.PersInArb.AsString(), "0");
            if (!DataModul.DB_EventTable.NoMatch)
            {
                Modul1.Datklein = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsInt();
                if (!(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsInt() == 0))
                {
                    goto IL_017e;
                }
            }
            Modul1.Ubg = 102;
            DataModul.DB_EventTable.Seek("=", Modul1.Ubg.AsString(), Modul1.PersInArb.AsString(), "0");
            if (!DataModul.DB_EventTable.NoMatch)
            {
                Modul1.Datklein = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsInt();
            }
            goto IL_017e;
        IL_017e:
            if (Modul1.Datklein == 30000000)
            {
                Modul1.Datklein = 0;
            }
            Modul1.Datum6 = Modul1.Datklein;
        }
    }
}
