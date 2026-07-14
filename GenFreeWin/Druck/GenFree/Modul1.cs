using Druck.My;
using GenFree.Helper;
using GenFree.Interfaces.DB;
using GenFree.Data;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Compatibility.VB6;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Druck;
using System.Collections.Generic;
using System.Linq;
using GenFree.Sys;
using Gen_FreeWin;
using BaseLib.Helper;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.UI;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.Model;
using GenFree.Data.Models;
using System.Collections;
using GenFree.Interfaces.VB;
using Druck.Views;
using System.Net;
using System.Net.Sockets;

namespace GenFree;

public class _Modul1 : IModul1
{
    private static IModul1? _instance = null;
    public static IModul1 Instance => _instance ??= new _Modul1();

    // Missing interface members
    private string _sFont1 = "";
    private string _sEltq = "";
    private string _sAusdat = "";
    private int _iPersSp;
    public string Datum1 { get; set; } = "";
    public string Datum2 { get; set; } = "";
    public int iPriv_aus { get; set; }
    public string Datschuname { get; set; } = "";
    public string Ubg1T { get; set; } = "";

    #region Types


    public struct Ahnenl
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        [VBFixedString(50)]
        public char[] Name1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        [VBFixedString(50)]
        public char[] Name2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        [VBFixedString(50)]
        public char[] Name3;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        [VBFixedString(50)]
        public char[] Name4;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        [VBFixedString(50)]
        public char[] Name5;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        [VBFixedString(50)]
        public char[] Name6;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        [VBFixedString(50)]
        public char[] Name7;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        [VBFixedString(50)]
        public char[] Name8;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        [VBFixedString(50)]
        public char[] Name9;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        [VBFixedString(50)]
        public char[] Name10;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        [VBFixedString(50)]
        public char[] Name11;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        [VBFixedString(50)]
        public char[] Name12;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        [VBFixedString(50)]
        public char[] Name13;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        [VBFixedString(50)]
        public char[] Name14;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        [VBFixedString(50)]
        public char[] Name15;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        [VBFixedString(50)]
        public char[] Name16;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        [VBFixedString(50)]
        public char[] Name17;
    }
    #endregion

    public string AppName => "Gen_FreeWin";
    public string VendorName => "JC-Soft";
    public string Author => "Joe Care";
    public const string AKontact = "49082 Osnabrück Friedrich-Holthaus-Str. 18 Tel.: 0541-80 00 79 00";
    public string VersionT => $"{AppName} das freie Genealogieprogramm";
    public string Version1 => $"(c) 2023 {Author}";
    public const string Version_ = "Version 24.09.02";
    public string VersDat => "Stand 15.07.2018";
    public string Version => $"{Version_} {VersDat}";

    public const byte Kindindent = 40;
    public const byte Heiratindent = 60;
    public const byte Hangindent = 15;
    public string Titel2 => $"(c) 2023-2024 {Author} {AKontact}";
    public string Version2 { get; set; }

    public IGenPersistence Persistence { get; private set; } = new CPersistence();

    #region Properties
    public byte PamPam = 0;
    public int Datum6;
    public int Datklein;
    public int FamSp;
    public int Perspa;
    public DriveInfo cMandDrive;

    //Tabels => DataModul
    //   public  IRecordset NTable;


    #region Model
    public IPersonData Person { get; set; }
    public IFamilyData Family { get; set; }
    #endregion
    #region FileSystem
    public string Verz { get; set; } = "";
    public string Verz1 { get; set; } = "";
    public string GenFreeDir => new FileInfo(Application.ExecutablePath).DirectoryName;
    public string InitDir => $"{GenFreeDir}\\Init";
    public string TempPath { get; set; }
    #endregion

    public string AutoupD;
    public byte Prädi;
    public bool EreiRf;
    public byte Einrück;

    public int iPrivacy;
    public int FamInArb { get; set; } = 0;
    public int PersInArb { get; set; } = 0;
    public int PFSatz { get; set; } = 0;
    public int Kol;

    public string sql;
    public string ind;
    #region Übergabe-Parameter
    [Obsolete]
    public IList<string> Kont { get; } = new string[101];
    public IList<string> Kont1 { get; } = new string[101];
    public IList<string> KontP { get; } = [];
    public IList<string> KontM { get; } = [];
    public IList<string> KontF { get; } = [];

    [Obsolete]
    public string UbgT { get; set; } = "";
    [Obsolete]
    public int Ubg { get; set; } = 0;
    #endregion
    public IApplUserTexts IText { get; }


    public ELinkKennz eLKennz { get; set; }
    public string eWKennz { get; set; }
    public string sPKennz { get; set; }
    public EEventArt Art { get; set; }

    public IList<string> Aus { get; } = new string[101];
    public IList<string> TxT { get; } = new string[101];
    public IList<string> DTxt { get; } = new string[31];
    public IList<string> KontSP1 { get; } = new string[101];
    public IList<string> KontSP { get; } = new string[101];
    public IList<string> Temp { get; } = new string[21];
    public IList<string> DAus { get; } = new string[132];
    public IList<string> KontBem { get; } = new string[10001];

    public Color Feld1Farb { get; set; }
    public Color HintFarb { get; set; }
    public Color Hintfarb1 { get; set; }
    public Color Farb { get; set; }


    public short LfNR { get; set; }
    public string Mandant { get; set; }
    public byte Schalt { get; set; }
    public byte Datschalt { get; set; }
    public byte Suchschalt { get; set; }
    public short Druck_Tast { get; set; }
    public short Feg { get; set; }
    public float Fs;
    public int Startpers { get; set; } = 0;
    public int PersInArbsp { get; set; }
    public int Ubg1 { get; set; }
    public string Inhaber { get; set; }

    public string AltName { get; set; } = "";
    public byte KonLen;
    public bool Trau;
    public int AltNr { get; set; } = 0;
    public byte Hoch = 3;

    public float Aschalt { get; set; } = 0f;
    public string Ind1 { get; set; }
    public string LiText;

    public bool VerS;
    public int OrtNr;
    public bool GED;

    public bool Ad { get; set; }

    public int Suchper { get; set; }
    public int Suchfam { get; set; }
    public Enum eWindowState { get; set; }

    public long Kek;
    public string sDatu { get; set; } = "";
    public EEventArt Beruf { get; set; }
    public int PersSp { get => _iPersSp; set => _iPersSp = value; }
    public string Namen;
    public short Flagsch;

    public short[] G = new short[11];
    public short[] Al = new short[1201];
    public int[] OA = new int[11];

    public string Fehler;
    public int BemZahl;
    public int High;
    public string Ausdat { get => _sAusdat; set => _sAusdat = value; }
    public string UbgT1 { get; set; } = "";
    public Color ErFarb { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Color Farb1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    float IModul1.Fs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IList<string> Quells => throw new NotImplementedException();

    public IList<string> Absend => throw new NotImplementedException();

    public IList<ESearchSelection> Suchfeld => throw new NotImplementedException();

    bool IModul1.EreiRf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public string InstPath => throw new NotImplementedException();

    public string ListDir => throw new NotImplementedException();

    public string HelpDir => throw new NotImplementedException();

    public string MainProg => throw new NotImplementedException();

    DriveInfo IModul1.cMandDrive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public DriveType Typ => throw new NotImplementedException();

    public int thisYear => throw new NotImplementedException();

    public byte Programtesttemp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    string IModul1.AutoupD { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool FAendmerk { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool PAendmerk { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Lw { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float Aend { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float FontSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool Aendf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public TGedLine Eing { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public ETextKennz eNKennz { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    int IModul1.eWKennz { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public ETextKennz eTKennz { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public short Qkenn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public string AppHostName => throw new NotImplementedException();

    public EWindowSize eWindowSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int Histor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int Quell { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int FeG { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IGedAus GedAus => throw new NotImplementedException();

    public IEinles Einles => throw new NotImplementedException();

    public string Zusatzquelle { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public string Menue_Ziel => throw new NotImplementedException();

    public string NamenSuch_Wort { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    int IModul1.Datklein { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public short Les { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public string Message_sNoChangesOnCD => throw new NotImplementedException();

    public IModul1.Letzter Letzte { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int SuchPer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public short Trans { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public short ErSchalt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int FamPerschalt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int Nr { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IList<string> Te => throw new NotImplementedException();

    public bool Reli { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int Frauenkek1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int Frauenkek2 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public string sBirthMark => throw new NotImplementedException();

    public string sBaptismMark => throw new NotImplementedException();

    public string sDeathMark => throw new NotImplementedException();

    public string sBurialMark => throw new NotImplementedException();

    public IModul1.Frauen Frauen_Renamed => throw new NotImplementedException();

    public IList<short> Posi { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Job { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool reorga { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public string PictureDir => throw new NotImplementedException();

    public string Message_sDemoVerNotPossibl => throw new NotImplementedException();

    public string sGeocodeXMLAddress => throw new NotImplementedException();

    public IRecordset DT_AhnTable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IProjectData ProjectData => throw new NotImplementedException();

    public IVBInformation Information => throw new NotImplementedException();

    public IVBConversions Conversions => throw new NotImplementedException();

    public IStrings Strings => throw new NotImplementedException();

    public IUserData User => throw new NotImplementedException();

    public IOperators Operators => throw new NotImplementedException();

    public ISystem System => throw new NotImplementedException();

    long IModul1.Kek { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string OrtBem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public short Ja;
    public string Font1 { get => _sFont1; set => _sFont1 = value; }
    public string Font2;
    public bool Pat;

    public string Eltq { get => _sEltq; set => _sEltq = value; }

    public AW oForm;

    public IModul1.Adresse Adr;
    public IModul1.Ausgab ausg;
    //   public  Ahnenl Ah;
    private const short GWW_HINSTANCE = -6;
    #endregion

    public _Modul1()
    {
        // Constructor logic here
        // Initialize properties, if needed
        // Example: this.Version2 = "Some version string";
        Version2 = $"(c) 2023-2024 {Author} {AKontact}";
    }

    public Bitmap PicResizeByWidth(Image SourceImage, int Newheigth)
    {
        decimal d = new decimal(Newheigth / (double)SourceImage.Height);
        int width = Convert.ToInt32(decimal.Multiply(d, new decimal(SourceImage.Width)));
        Bitmap bitmap = new Bitmap(width, Newheigth);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Rectangle rect = new Rectangle(0, 0, width, Newheigth);
            graphics.DrawImage(SourceImage, rect);
        }
        return bitmap;
    }

    public Image AutoSizeImage(Image oBitmap, int maxWidth, int maxHeight, bool bStretch = false)
    {
        //Discarded unreachable code: IL_0095
        float num = (float)(maxWidth / (double)maxHeight);
        int width = oBitmap.Width;
        int height = oBitmap.Height;
        float num2 = (float)(width / (double)height);
        if (width > maxWidth || height > maxHeight || bStretch)
        {
            checked
            {
                if (num2 <= num)
                {
                    width = (int)Math.Round(width / (height / (double)maxHeight));
                    height = maxHeight;
                }
                else
                {
                    height = (int)Math.Round(height / (width / (double)maxWidth));
                    width = maxWidth;
                }
                Bitmap bitmap = new Bitmap(width, height);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    Rectangle rect = new Rectangle(0, 0, width, height);
                    graphics.DrawImage(oBitmap, rect);
                }
                return bitmap;
            }
        }
        return oBitmap;
    }

    public void Info()
    {
        string text = "Entwicklungshistorie Version 24 Druckmodul";
        text += "\nBitte keine Anfragen hierzu";
        text += "\n";
        text += "\n12.10. Generation vor Ausgabe nochmal lesen(w.g. Mehrfachahnen (Schiller).";
        text += "\n07.11. Bilderausgabe und Religion im Familienbuch, neue Einstellseite";
        text += "\n11.11. Fambuch Abbruch (H.Friedrich).";
        text += "\n11.11. Bilder Generationenliste und Ahnenliste";
        text += "\n14.12. Textmarker entfernt (Deltgen).";
        text += "\n20.01. Fambuch Abbruch wegen ungünstiger Speichersituation(H.Friedrich).";
        text += "\n30.01. Ahnenliste div. Änderungen (Steinmetz).";
        text += "\n12.02. Anpassungen Bilderausgabe Ahnenliste und Fam-Buch.";
        text += "\n22.03. Datenschutz und Bilder Generationenliste, Lizenznummer.";
        text += "\n02.06. Datenschutz Familienbuch.";
        text += "\n05.06. Datenschutz Generationenliste";
        text += "\n11.06. Datenschutz Stammfolgelisteliste";
        text += "\n30.06. Personenbilder im Familienbuch, Ausgabe speichern (Format)";
        text += "\n02.07. fehlende Einstellseite abfangen.";
        text += "\n18.07. Familienbuch, Ausgabe speichern (Format)";
        Interaction.MsgBox(text);
    }

    public void Famdatles1(byte at, out string[] Kont)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        EEventArt num5 = default;
        object CounterResult = default;
        object LoopForResult = default;
        Kont = new string[101];
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
                    int ortNr;
                    int ortNr2;
                    byte Schalt;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 2475:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_07e5;
                                    default:
                                        goto end_IL_0000;
                                }
                                int number = Information.Err().Number;
                                if (number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_07e5;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString() + "Famdatles1", MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_07e9;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            Kont.Initialize();
                            num5 = EEventArt.eA_500;
                            goto IL_0032;
                        IL_0032: // <========== 3
                            num = 6;
                            Ubg = (int)num5;
                            Art = num5;
                            if (num5 > EEventArt.eA_507 && num5 < EEventArt.eA_600)
                            {
                                Art = EEventArt.eA_601;
                            }
                            if (DataModul.Event.ReadData(Art, FamInArb, out var cEvt))
                            {

                                iPrivacy = cEvt.iPrivacy;
                                if (!(iPrivacy > iPriv_aus))
                                {
                                    if (Datschalt == 2
                                        && ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, MyProject.Forms.Hinter.List1.Items.Count - 1, 1, ref LoopForResult, ref CounterResult))
                                    {
                                        while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult, ref CounterResult))
                                        {
                                            Kol = (int)Math.Round(Conversion.Val(Strings.Right(MyProject.Forms.Hinter.List1.Items[CounterResult.AsInt()].AsString(), 6)));
                                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() == Kol)
                                            {
                                                Datschalt = 3;
                                                goto end_IL_0000_2;
                                            }
                                        }

                                    }
                                    var J = 0;
                                    while (unchecked(J) <= 15u)
                                    {
                                        Kont1[J] = "";
                                        J++;
                                    }
                                    if (Datschalt == 10)
                                    {
                                        Kont[Ubg - 500] = DataModul.DB_EventTable.Fields[EventFields.Reg].AsString();
                                        Kont[Ubg - 490] = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                    }
                                    else if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                    {
                                        if (_Modul1.Instance.Schalt == 1)
                                        {
                                            Kont[Ubg - 500] = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                        }
                                        else
                                        {
                                            sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                            string ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                            sDatu = Datwand1(sDatu, ds);
                                            Kont1[1] = sDatu;
                                            goto IL_03ed;
                                        }
                                    }
                                    else if (_Modul1.Instance.Schalt != 1)
                                    {
                                        goto IL_03ed;
                                    }
                                }
                            }
                            goto IL_073c;
                        IL_03ed: // <========== 3
                            num = 57;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                string ds2 = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                sDatu = Datwand1(sDatu, ds2);
                                if (DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString().Trim()== "")
                                {
                                    Kont1[3] = " / " + sDatu;
                                }
                                else Kont1[3] = sDatu;
                            }
                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                Kont1[7] = DataModul.TextLese1(AAA);
                            }
                            UbgT = "";
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                            {
                                if (_Modul1.Instance.Schalt == 6)
                                {
                                    ortNr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble());
                                    UbgT = ortles(ortNr, 6);
                                    Kont1[5] = UbgT;
                                    UbgT = "";
                                }
                                else
                                {
                                    ortNr2 = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble());
                                    UbgT = ortles(ortNr2, 1);
                                    Kont1[5] = UbgT;
                                    UbgT = "";
                                }
                            }
                            if (at == 1)
                            {
                                Kont[Ubg - 500] = Kont1[1] + " " + Kont1[5];
                                Kont[Ubg - 450] = Kont1[5];
                            }
                            else
                            {

                                Kont[Ubg - 500] = Kont1[1] + " " + Kont1[7] + Kont1[3] + Kont1[4] + Kont1[5] + Kont1[6];
                                Kont[Ubg - 500] = Module2.Jobdreh(Kont[Ubg - 500]);
                            }
                            goto IL_073c;

                        IL_073c: // <========== 8
                            num = 92;
                            lErl = 2;
                            num5 = unchecked(num5 + 1);
                            if (num5 <= (EEventArt)508)
                                goto IL_0032;
                            else
                            {
                                goto end_IL_0000_2;
                            }
                        IL_07e5:
                            num4 = unchecked(num2 + 1);
                            goto IL_07e9;
                        IL_07e9:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 6:
                                    goto IL_0032;
                                case 18:
                                case 21:
                                case 22:
                                case 33:
                                case 34:
                                case 51:
                                case 55:
                                case 56:
                                case 57:
                                    goto IL_03ed;
                                case 63:
                                case 66:
                                case 67:
                                case 68:
                                case 77:
                                case 82:
                                case 83:
                                case 84:
                                case 14:
                                case 23:
                                case 40:
                                case 45:
                                case 54:
                                case 87:
                                case 91:
                                case 92:
                                    goto IL_073c;
                                case 30:
                                case 94:
                                case 96:
                                case 100:
                                case 101:
                                case 107:
                                case 108:
                                case 109:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 2475;
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
    public void Datles(int PersInArb, out IList<string> asPersDates)
    {
        asPersDates = new string[101];
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int number = default;
        short num5 = default;
        object CounterResult = default;
        object LoopForResult = default;
        string ds = default;
        string ds2 = default;
        string UbgT = default;
        int lErl = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int ortNr;
                    int AAA;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            Datum1 = "";
                            goto IL_000d;
                        case 2667:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 1:
                                        break;
                                    default:
                                        goto end_IL_0000;
                                }
                                while (true)
                                {
                                    int num4 = unchecked(num2 + 1);
                                    while (true)
                                    {
                                        num2 = 0;
                                        switch (num4)
                                        {
                                            case 1:
                                                break;
                                            case 87:
                                                goto end_IL_0000_2;
                                            case 89:
                                                num = 89;
                                                number = Information.Err().Number;
                                                goto case 91;
                                            case 91:
                                            case 92:
                                                num = 92;
                                                if (number == 94)
                                                {
                                                    ProjectData.ClearProjectError();
                                                    if (num2 == 0)
                                                    {
                                                        throw ProjectData.CreateProjectError(-2146828268);
                                                    }
                                                }
                                                goto case 96;
                                            case 96:
                                            case 97:
                                                num = 97;
                                                if (Interaction.MsgBox(Conversion.ErrorToString() + "Datles1", MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                                                {
                                                    ProjectData.EndApp();
                                                }
                                                goto case 98;
                                            case 98:
                                            case 100:
                                                num = 100;
                                                ProjectData.ClearProjectError();
                                                if (num2 == 0)
                                                {
                                                    throw ProjectData.CreateProjectError(-2146828268);
                                                }
                                                num4 = num2;
                                                continue;
                                        }
                                        break;
                                    }
                                    break;
                                }
                                goto default;
                            }
                        end_IL_0000_2:
                            break;
                        IL_000d:
                            num = 2;
                            Datum2 = "";
                            num5 = 1;
                            while (num5 <= 25)
                            {
                                asPersDates[num5] = "";
                                num5 = (short)unchecked(num5 + 1);
                            }
                            num5 = 101;
                            while (num5 <= 107)
                            {
                                var J = 0;
                                while (unchecked(J) <= 20u)
                                {
                                    Kont1[J] = "";
                                    J++;
                                }
                                var Ubg = num5;
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", Ubg.AsString(), PersInArb.AsString(), "0");
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    if (Datschalt == 2
                                        && ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, MyProject.Forms.Hinter.List1.Items.Count - 1, 1, ref LoopForResult, ref CounterResult))
                                    {
                                        while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult, ref CounterResult))
                                        {
                                            Kol = (int)Math.Round(Conversion.Val(Strings.Right(MyProject.Forms.Hinter.List1.Items[CounterResult.AsInt()].AsString(), 6)));
                                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() == Kol)
                                            {
                                                Datschalt = 3;
                                                goto end_IL_0000_3;
                                            }
                                        }
                                    }
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                    {
                                        sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                        if (Datschalt == 1)
                                        {
                                            asPersDates[Ubg - 100] = sDatu;
                                            num5++;
                                            continue;
                                        }
                                        else
                                        if (Datschalt == 10)
                                        {
                                            asPersDates[Ubg - 100] = DataModul.DB_EventTable.Fields[EventFields.Reg].AsString();
                                            asPersDates[Ubg - 90] = sDatu;
                                            num5++;
                                            continue;
                                        }
                                        else
                                        {
                                            ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                            sDatu = Datwand1(sDatu, ds);
                                            Kont1[1] = sDatu;
                                        }
                                    }
                                    if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                                    {
                                        sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString().Trim(), 8);
                                        ds2 = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                        sDatu = Datwand1(sDatu, ds2);
                                        if (DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString().Trim()== "")
                                        {
                                            Kont1[3] = " / " + sDatu;
                                        }
                                        else
                                            Kont1[3] = sDatu;
                                    }
                                    _Modul1.Instance.UbgT = "";
                                    if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                                    {
                                        ortNr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble());
                                        UbgT = ortles(ortNr, 1);
                                    }
                                    if (DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString().Trim()== "")
                                    {
                                        _Modul1.Instance.UbgT = _Modul1.Instance.UbgT.Trim() + " ?";
                                    }
                                    if (DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt() != 0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                        asPersDates[0] = DataModul.TextLese1(AAA);
                                        if (asPersDates[0] != "")
                                        {
                                            _Modul1.Instance.UbgT = " " + asPersDates[0].Trim() + " ";
                                        }
                                    }
                                    if (DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().TrimEnd() != "" | DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().TrimEnd() != "")
                                    {
                                        asPersDates[Ubg - 90] = (Kont1[1] + " " + Kont1[2] + Kont1[3] + Kont1[4] + Kont1[5] + Kont1[6]).Trim() + " " + _Modul1.Instance.UbgT;
                                        _Modul1.Instance.UbgT = "";
                                        UbgT = "";
                                        if (Aus[2] == "Y")
                                        {
                                            UbgT = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().TrimEnd();
                                            UbgT = Text_Retweg(UbgT);
                                            asPersDates[Ubg - 85] = UbgT;
                                        }
                                        UbgT = "";
                                        if (Aus[3] == "Y")
                                        {
                                            UbgT = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().TrimEnd();
                                            UbgT = Text_Retweg(UbgT);
                                            asPersDates[Ubg - 80] = UbgT;
                                        }
                                    }
                                    else
                                    {
                                        asPersDates[Ubg - 90] = (Kont1[1] + " " + Kont1[2] + Kont1[3] + Kont1[4] + Kont1[5] + Kont1[6]).Trim() + " " + _Modul1.Instance.UbgT;
                                        _Modul1.Instance.UbgT = "";
                                    }
                                }
                                lErl = 2;
                                num5++;
                            }
                            asPersDates[25] = RechWithProps();
                            break;
                    }
                    num = 87;
                    break;
                }
            end_IL_0000:
                ;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 2667;
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
    public void Dateienopen()
    {
        int try0000_dispatch = -1;
        int num = default;
        int num6 = default;
        int num2 = default;
        int num3 = default;
        short num4 = default;
        string inhaber = default;
        string source = default;
        bool flag = default;
        int lErl = default;
        string destination = default;
        string name = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num5;
                    string text;
                    object obj;
                    Hinter hinter;
                    string[] asLines = new string[10];
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            num6 = DateTime.Today.Year;
                            goto IL_000f;
                        case 5878:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                    case 7:
                                        break;
                                    case 6:
                                        goto IL_0e9a;
                                    case 3:
                                    case 5:
                                        goto IL_0fe3;
                                    case 4:
                                        goto IL_12a7;
                                    case 1:
                                        goto IL_1328;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 3044)
                                {
                                    Debugger.Break();
                                }
                                if (Information.Err().Number == 3010)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_1328;
                                }
                                if (Information.Err().Number == 3375)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_1328;
                                }
                                if (Information.Err().Number == 75)
                                {
                                    DataModul.DSB_SortTable.Close();
                                    DataModul.DSB_NamIdxTable.Close();
                                    DataModul.DSB_OrtIdxTable.Close();
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_1324;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString() + "Dateienopen", MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1324;
                            }
                        end_IL_0000:
                            break;
                        IL_000f:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            FileSystem.FileClose();
                            Ind1 = "";
                            FileSystem.FileClose(99);
                            FileSystem.FileOpen(99, Verz1 + "\\Adresse", OpenMode.Append);
                            FileSystem.FileClose(99);
                            FileSystem.FileOpen(99, Verz1 + "\\Adresse", OpenMode.Input);
                            source = "";
                            num4 = 1;
                            goto IL_00ac;
                        IL_00ac: // <========== 3
                            num = 11;
                            flag = FileSystem.EOF(99);
                            if (!flag)
                            {
                                asLines[num4] = FileSystem.LineInput(99);
                                num4 = (short)unchecked(num4 + 1);
                                if (num4 <= 3)
                                {
                                    goto IL_00ac;
                                }
                            }
                            FileSystem.FileClose(99);
                            inhaber = asLines[2].Trim() + " " + asLines[3].Trim();
                            FileSystem.FileClose(99);
                            ProjectData.ClearProjectError();
                            num3 = 3;
                            goto IL_0143;
                        IL_0143: // <========== 3
                            num = 21;
                            lErl = 1;
                            Inhaber = inhaber;
                            Verz1 = Verz.Left(15);
                            text = Verz.Left(2);
                            Mandant = Verz + "Gen_plusdaten.mdb";
                            DataModul.wrkDefault = UpgradeSupport.DAODBEngine_definst.Workspaces[0];
                            DataModul.TempDB.Close();
                            DataModul.TempDB = UpgradeSupport.DAODBEngine_definst.OpenDatabase(Verz + "Tempo.mdb", false, false, "");
                            DataModul.TempDB.Execute($"DROP Table {dbTables.Sperrliste};");
                            DataModul.TempDB.Execute($"CREATE INDEX Implex ON {dbTables.Ahnen1} ([PerNr],[Ahn]);");
                            DataModul.DT_DescendentTable = DataModul.TempDB.OpenRecordset(dbTables.Nachk, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DSB = UpgradeSupport.DAODBEngine_definst.OpenDatabase(Verz + "Such.mdb", false, false, "");
                            DataModul.DOSB = UpgradeSupport.DAODBEngine_definst.OpenDatabase(Verz + "Ort1.mdb", false, false, "");
                            DataModul.DSB_SearchTable = DataModul.DSB.OpenRecordset(dbTables.Such, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DOSB_OrtSTable = DataModul.DOSB.OpenRecordset(dbTables.OrtSuch, RecordsetTypeEnum.dbOpenTable);
                            obj = DataModul.TempDB.OpenRecordset(dbTables.Konf, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DT_AncesterTable = DataModul.TempDB.OpenRecordset(dbTables.Ahnen1, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DT_AncesterTable.Index = "PerNr";
                            DataModul.TempDB.Execute($"DROP Table {dbTables.Sperrliste};");
                            DataModul.TempDB.Execute($"CREATE Table {dbTables.Sperrliste}([PerNr] Long )");
                            DataModul.TempDB.Execute($"CREATE INDEX nr ON {dbTables.Sperrliste} ([PerNr]);");
                            DataModul.DT_SperrTable = DataModul.TempDB.OpenRecordset(dbTables.Sperrliste, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DT_KindAhnTable = DataModul.TempDB.OpenRecordset(dbTables.Ahnew, RecordsetTypeEnum.dbOpenTable);
                            if (num6 < 2018)
                            {
                                DataModul.MandDB = UpgradeSupport.DAODBEngine_definst.OpenDatabase(Mandant.ToUpper(), true, false, ";pwd=geheim");
                            }
                            else
                            {

                                DataModul.MandDB = UpgradeSupport.DAODBEngine_definst.OpenDatabase(Mandant, true, false, "");
                            }
                            goto IL_0488;
                        IL_0488: // <========== 3
                            num = 51;
                            DataModul.wrkDefault = UpgradeSupport.DAODBEngine_definst.Workspaces[0];
                            DataModul.MandDB.Execute($"CREATE INDEX PerKen2 ON {dbTables.Bilder} ([Kennz],[ZuNr],[Beschreibung]);");
                            DataModul.DB_QuTable = DataModul.MandDB.OpenRecordset(dbTables.Quellen, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DB_SourceLinkTable = DataModul.MandDB.OpenRecordset(dbTables.Tab1, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DB_BildTab = DataModul.MandDB.OpenRecordset("select * from Bilder Where Bilder.Kennz='" + eLKennz + "'");
                            DataModul.DB_FamilyTable = DataModul.MandDB.OpenRecordset(dbTables.Familie, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DB_WitnessTable = DataModul.MandDB.OpenRecordset(dbTables.Tab2, RecordsetTypeEnum.dbOpenTable);

                            DataModul.MandDB.Execute($"DELETE * FROM {dbTables.GED}");
                            DataModul.MandDB.Execute($"CREATE TABLE {dbTables.GED}(PNr long);");
                            DataModul.MandDB.Execute($"CREATE UNIQUE INDEX pNr ON {dbTables.GED} ([PNR]);");
                            DataModul.DB_GedTable = DataModul.MandDB.OpenRecordset(dbTables.GED, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DB_GedTable.Index = "PNr";

                            DataModul.MandDB.Execute($"DELETE * FROM {dbTables.Doppel}");
                            DataModul.MandDB.Execute($"CREATE TABLE {dbTables.Doppel}(Nr TEXT(240)Not NULL,Pr long);");
                            DataModul.MandDB.Execute($"CREATE UNIQUE INDEX Nr ON {dbTables.Doppel} ([PR]);");
                            DataModul.DB_DoppelTable = DataModul.MandDB.OpenRecordset(dbTables.Doppel, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DB_DoppelTable.Index = "Nr";

                            DataModul.DB_TexteTable = DataModul.MandDB.OpenRecordset(dbTables.Texte, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DB_TexteTable.Index = "TxNr";
                            DataModul.DOSB_OrtSTable.Index = "OrtNr";
                            DataModul.DB_PlaceTable = DataModul.MandDB.OpenRecordset(dbTables.Orte, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DB_PlaceTable.Index = "OrtNr";
                            DataModul.DB_PictureTable = DataModul.MandDB.OpenRecordset(dbTables.Bilder, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DB_EventTable = DataModul.MandDB.OpenRecordset(dbTables.Ereignis, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_NameTable = DataModul.MandDB.OpenRecordset(dbTables.INamen, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DB_NameTable.Index = "PNamen";
                            DataModul.DB_RepoTable = DataModul.MandDB.OpenRecordset(dbTables.Repo, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DB_RepoTab = DataModul.MandDB.OpenRecordset(dbTables.RepoTab, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DB_PersonTable = DataModul.MandDB.OpenRecordset(dbTables.Personen, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DB_PersonTable.Index = "PerNr";
                            DataModul.DB_LinkTable = DataModul.MandDB.OpenRecordset(dbTables.Tab, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DSB_SortTable.Close();
                            DataModul.DSB_NamIdxTable.Close();
                            DataModul.DSB_OrtIdxTable.Close();
                            DataModul.DSB_PerStatTable.Close();
                            DataModul.DSB_FamStatTable.Close();
                            FileSystem.FileOpen(99, GenFreeDir + "TEMP\\V.dat", OpenMode.Append);
                            FileSystem.FileClose(99);
                            DataModul.TempSort_DB.Close();
                            hinter = MyProject.Forms.Hinter;
                            hinter.Att(GenFreeDir + "TEMP");
                            ProjectData.ClearProjectError();
                            num3 = 4;
                            FileSystem.Kill(GenFreeDir + "TEMP\\*.*");
                            ProjectData.ClearProjectError();
                            num3 = 5;
                            source = GenFreeDir + "INIT\\SortTem.mdb";
                            destination = GenFreeDir + "TEMP\\SortTem.mdb";
                            FileSystem.FileCopy(source, destination);
                            name = GenFreeDir + "TEMP\\SortTem.mdb";
                            DataModul.TempSort_DB = UpgradeSupport.DAODBEngine_definst.OpenDatabase(name, false, false, "");
                            DataModul.TempSort_DB.Execute($"Drop TABLE {dbTables.QuellVerz} ");
                            DataModul.TempSort_DB.Execute($"CREATE Table {dbTables.QuellVerz} (Quelle Text(240),Nr Long);");
                            DataModul.TempSort_DB.Execute($"CREATE UNIQUE INDEX Quelle ON {dbTables.QuellVerz} ([Quelle],[Nr]);");
                            DataModul.TempSort_DB.Execute("Drop TABLE Namind ");
                            DataModul.TempSort_DB.Execute($"CREATE Table {dbTables.NamInd} (Nr Text(50),Name Text(200),Name1 Text(200),Ind Text(240));");
                            DataModul.TempSort_DB.Execute($"CREATE INDEX Vollname ON {dbTables.NamInd} ([Name],[Nr]);");
                            DataModul.TempSort_DB.Execute($"CREATE INDEX Langi ON {dbTables.NamInd} ([Name1],[Name],[Nr]);");
                            DataModul.TempSort_DB.Execute($"CREATE INDEX Kurzname ON {dbTables.NamInd} ([Name1],[Nr]);");
                            DataModul.TempSort_DB.Execute($"CREATE UNIQUE INDEX Nr ON {dbTables.Sort} (Nr);");
                            DataModul.DSB_SortTable = DataModul.TempSort_DB.OpenRecordset(dbTables.Sort, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DSB_NamIdxTable = DataModul.TempSort_DB.OpenRecordset(dbTables.NamInd, RecordsetTypeEnum.dbOpenTable);
                            DataModul.TempSort_DB.Execute("Drop TABLE Ortindex ");
                            DataModul.TempSort_DB.Execute($"CREATE Table {dbTables.OrtIndex} (Name Text(240),Ort Text(200),Ind Text(240),OrtNr Long);");
                            DataModul.TempSort_DB.Execute($"CREATE UNIQUE INDEX Ges ON {dbTables.OrtIndex} ([Ort],[OrtNr],[Name],[Ind]);");
                            DataModul.TempSort_DB.Execute($"CREATE INDEX NameOrt ON {dbTables.OrtIndex} ([Name],[Ort],[OrtNr],[Ind]);");
                            DataModul.TempSort_DB.Execute($"CREATE INDEX Ortind ON {dbTables.OrtIndex} ([Ort],[OrtNr]);");
                            DataModul.TempSort_DB.Execute($"CREATE INDEX Ort ON {dbTables.OrtIndex} ([Ort],[OrtNr],[Ind]);");
                            DataModul.TempSort_DB.Execute($"CREATE INDEX OrtNam ON {dbTables.OrtIndex} ([Ort],[OrtNr],[Name],[Ind]);");
                            DataModul.TempSort_DB.Execute($"CREATE INDEX NamJahr ON {dbTables.OrtIndex} ([Name],[Ort],[Ind]);");
                            DataModul.DSB_QuellIdxTable = DataModul.TempSort_DB.OpenRecordset(dbTables.QuellVerz, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DSB_QuellIdxTable.Index = "Quelle";
                            DataModul.DSB_OrtIdxTable = DataModul.TempSort_DB.OpenRecordset(dbTables.OrtIndex, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DSB_SortTable.Index = "Sort";
                            DataModul.DSB_PerStatTable = DataModul.TempSort_DB.OpenRecordset(dbTables.Per_Stat, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DSB_PerStatTable.Index = "Per";
                            DataModul.DSB_FamStatTable = DataModul.TempSort_DB.OpenRecordset(dbTables.Fam_Stat, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DSB_FamStatTable.Index = "Nr";
                            ProjectData.ClearProjectError();
                            num3 = 6;
                            Persistence.ReadStringsInit("Druck_ini.dat", Aus, false);
                            goto end_IL_0000_2;
                        IL_0e9a:
                            num = 162;
                            if (Information.Err().Number == 53)
                            {
                                num4 = 1;
                                while (num4 <= 6)
                                {
                                    Aus[num4] = num4.AsString();
                                    num4 = (short)unchecked(num4 + 1);
                                }
                                Aus[7] = "c:\\Programme\\zubehör\\wordpad.exe ";
                                FileSystem.FileOpen(99, Verz1 + "Init\\Druck_ini.dat", OpenMode.Append);
                                num4 = 1;
                                while (num4 <= 7)
                                {
                                    FileSystem.PrintLine(99, Aus[num4]);
                                    num4 = (short)unchecked(num4 + 1);
                                }
                                FileSystem.FileClose(99);
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1324;
                            }
                            if (Interaction.MsgBox(Conversion.ErrorToString() + "Druckini:", MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_1324;
                        IL_0fe3:
                            num = 180;
                            if (Information.Err().Number == 53)
                            {
                                inhaber = "Demoversion";
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num2 = 0;
                                goto IL_0143;
                            }
                            if (Information.Err().Number == 3078)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1328;
                            }
                            if (Information.Err().Number == 3010)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1328;
                            }
                            if (Information.Err().Number == 3211)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1328;
                            }
                            if (Information.Err().Number == 3375)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1328;
                            }
                            if (Information.Err().Number == 3376)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1328;
                            }
                            if (Information.Err().Number == 3380)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1328;
                            }
                            if (Information.Err().Number == 3420)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1328;
                            }
                            if (Information.Err().Number == 91)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1328;
                            }
                            if (Information.Err().Number == 75)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1328;
                            }
                            if (Information.Err().Number == 70)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1328;
                            }
                            if (Interaction.MsgBox(Conversion.ErrorToString() + "Lizerr:", MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_1324;
                        IL_12a7:
                            num = 231;
                            if (Information.Err().Number == 55)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1328;
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
                            goto IL_1324;
                        IL_1324: // <========== 6
                            num5 = num2;
                            goto IL_132c;
                        IL_1328: // <========== 14
                            num5 = unchecked(num2 + 1);
                            goto IL_132c;
                        IL_132c:
                            num2 = 0;
                            switch (num5)
                            {
                                case 1:
                                    break;
                                case 11:
                                    goto IL_00ac;
                                case 21:
                                case 183:
                                    goto IL_0143;
                                case 47:
                                case 50:
                                case 51:
                                    goto IL_0488;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj2) when (obj2 is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj2, lErl);
                try0000_dispatch = 5878;
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

    public IList<int> Link_Famsuch(int PersInArb, ELinkKennz eLKennz)
    {
        return DataModul.Link.GetPersonFams(PersInArb, eLKennz);
    }

    public void Person_ReadNames(int PersInArb, IPersonData person)
    {
        if (PersInArb == 0)
        {
            return;
        }
        if (GED)
        {
            DataModul.NB_PersonTable.AddNew();
            DataModul.NB_PersonTable.Fields[IndexFields.Person].Value = PersInArb;
            DataModul.NB_PersonTable.Update();
        }
        DataModul.DB_GedTable.AddNew();
        DataModul.DB_GedTable.Fields[GEDFields.PNr].Value = PersInArb;
        DataModul.DB_GedTable.Update();
        _ = DataModul.Names.ReadPersonNames(PersInArb, out var iName, out var aiVorns);

        Person.SetPersonNames(iName, aiVorns, Aus[(int)EOutCfg.o20] == "Y");
    }


    //public  void DataModul.Textlese(ref int AAA, ref string BBB, ref string LD)
    //{
    //    LD = "";

    //    if (AAA == 0)
    //    {
    //        BBB = "";
    //        return;
    //    }
    //    LD = "";
    //    BBB = "";
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

    public void Family_Les(int famInArb, IFamilyData family)
    {
        short num5 = default;
        int num4;
        short num6 = 1;
        while (num6 <= 99)
        {
            family.Kind[num6] = 0;
            num6++;
        }
        family.Mann = 0;
        family.Frau = 0;
        if (famInArb == 0) return;

        IRecordset nB_FamilyTable = DataModul.NB_FamilyTable!;
        nB_FamilyTable.AddNew();
        nB_FamilyTable.Fields[IndexFields.Fam].Value = famInArb;
        nB_FamilyTable.Update();

        if (DataModul.Link.ReadFamily(famInArb,family))
        {
            IRecordset dSB_FamStatTable = DataModul.DSB_FamStatTable;
            dSB_FamStatTable.Index = "Nr";
            dSB_FamStatTable.Seek("=", famInArb);
            if (dSB_FamStatTable.NoMatch)
            {
                dSB_FamStatTable.AddNew();
                dSB_FamStatTable.Fields["Fam"].Value = famInArb;
                dSB_FamStatTable.Fields["Heir"].Value = 0;
                dSB_FamStatTable.Fields["Mann"].Value = 0;
                dSB_FamStatTable.Fields["Frau"].Value = 0;
                dSB_FamStatTable.Fields["Kind"].Value = 0;
                dSB_FamStatTable.Fields["Vor"].Value = 0;
                dSB_FamStatTable.Fields["Sich"].Value = 0;
                dSB_FamStatTable.Update();
            }
            dSB_FamStatTable.Edit();
            if (DataModul.Event.GetPersonBirthOrBapt(family.Mann) is DateTime dt && dt != default)
            {
                dSB_FamStatTable.Fields["Mann"].Value = dt.Year;
            }
            if (DataModul.Event.GetPersonBirthOrBapt(family.Frau) is DateTime dt2 && dt2 != default)
            {
                dSB_FamStatTable.Fields["Frau"].Value = dt2.Year;
            }
            if (num5 > 0 && dSB_FamStatTable.Fields["Kind"].AsInt() == 0)
            {
                dSB_FamStatTable.Fields["Kind"].Value = num5;
            }
            dSB_FamStatTable.Update();
        }
    }
    public void Datles1(int PersInArb, out string[] Kont)
    {
        var PerPos = 0;
        int try0000_dispatch = -1;
        int num = default;
        string ind = default;
        int num2 = default;
        int num3 = default;
        int number = default;
        int lErl = default;
        short num5 = default;
        object CounterResult = default;
        object LoopForResult = default;
        string ds = default;
        string ds2 = default;
        string value = default;
        string text = default;
        byte b = default;
        Kont = new string[41];
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
                    short LfNR;
                    string LD;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            ind = "";
                            goto IL_0009;
                        case 6661:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_164f;
                                    default:
                                        goto end_IL_0000;
                                }
                                number = Information.Err().Number;
                                if (number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_164f;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString() + "Datles", MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_1653;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            num = 2;
                            Datum1 = "";
                            Datum2 = "";
                            High = 0;
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            num5 = 1;
                            while (num5 <= 40)
                            {
                                Kont[num5] = "";
                                num5 = (short)unchecked(num5 + 1);
                            }
                            num5 = 101;
                            goto IL_006e;
                        IL_006e: // <========== 3
                            num = 12;
                            var J = 0;
                            while (unchecked(J) <= 15u)
                            {
                                Kont1[J] = "";
                                J++;
                            }
                            Ubg = num5;
                            Art = (EEventArt)Ubg;
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", Ubg.AsString(), PersInArb.AsString(), "0");
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields["priv"].AsInt()))
                                {
                                    iPrivacy = DataModul.DB_EventTable.Fields["priv"].AsInt();
                                }
                                else
                                {
                                    iPrivacy = 0;
                                }
                                goto IL_01b5;
                            }
                            goto IL_1585;
                        IL_01b5: // <========== 3
                            num = 28;
                            if (!(iPrivacy.AsDouble() > iPriv_aus.AsDouble()))
                            {
                                if (Datschalt == 2)
                                {
                                    if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, MyProject.Forms.Hinter.List1.Items.Count - 1, 1, ref LoopForResult, ref CounterResult))
                                    {
                                        while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult, ref CounterResult))
                                        {
                                            Kol = (int)Math.Round(Conversion.Val(Strings.Right(MyProject.Forms.Hinter.List1.Items[CounterResult.AsInt()].AsString(), 6)));
                                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() == Kol)
                                            {
                                                Datschalt = 3;
                                                goto end_IL_0000_2;
                                            }
                                        }
                                    }
                                    goto IL_1585;
                                }
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                                    ind = Conversion.Str(sDatu.Left(2).AsDouble() + 1.0);
                                    if (Datschalt == 1)
                                    {
                                        Kont[Ubg - 100] = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                                        goto IL_1585;
                                    }
                                    if (Ubg == 101)
                                    {
                                        Kont[26] = sDatu.Left(4);
                                    }
                                    if (Ubg == 102 & Kont[26] == "")
                                    {
                                        Kont[26] = sDatu.Left(4);
                                    }
                                    if (Ubg == 103)
                                    {
                                        Kont[27] = sDatu.Left(4);
                                    }
                                    if (Ubg == 104 & Kont[27] == "")
                                    {
                                        Kont[27] = sDatu.Left(4);
                                    }
                                    ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                    sDatu = Datwand1(sDatu, ds);
                                    Kont1[1] = sDatu;
                                }
                                goto IL_04a5;
                            }
                            goto IL_1585;
                        IL_04a5:
                            num = 64;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString().Trim(), 8);
                                ds2 = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                sDatu = Datwand1(sDatu, ds2);
                                if (DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString().Trim() == "")
                                {
                                    Kont1[3] = " / " + sDatu;
                                }
                                else
                                    Kont1[3] = sDatu;
                            }
                            goto IL_05a8;
                        IL_05a8: // <========== 3
                            num = 75;
                            UbgT = "";
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.DatumText].Value))
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt() > 0)
                                {
                                    value = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsString();
                                    AAA = value.AsInt();
                                    LD = "";
                                    UbgT = DataModul.TextLese1(AAA);
                                    if (UbgT.Trim() != "")
                                    {
                                        Kont1[1] = Kont1[1] + " (" + UbgT.Trim() + ")";
                                        UbgT = "";
                                    }

                                }
                            }
                            goto IL_06b4;
                        IL_06b4: // <========== 3
                            num = 86;
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                            {
                                if (_Modul1.Instance.Schalt == 200)
                                {
                                    DataModul.DB_PlaceTable.Seek("=", DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble());
                                    if (!DataModul.DB_PlaceTable.NoMatch)
                                    {
                                        AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt();
                                        LD = "";
                                        UbgT = DataModul.TextLese1(AAA);
                                        AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt();
                                        LD = "";
                                        Kont1[0] = DataModul.TextLese1(AAA);
                                        if (Kont1[0].Trim() != "")
                                        {
                                            UbgT = UbgT + "-" + Kont1[0];

                                        }
                                    }
                                    goto IL_0fc8;
                                }
                                if (_Modul1.Instance.Schalt == 10)
                                {
                                    Ind1 = ind;
                                }
                                goto IL_086b;
                            }
                            goto IL_08a2;
                        IL_086b:
                            num = 101;
                            ortNr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble());
                            UbgT = ortles(ortNr, 1);
                            goto IL_08a2;
                        IL_08a2: // <========== 3
                            num = 103;
                            if (Art == EEventArt.eA_Death)
                            {
                                Kont[0] = "";
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields["causal"].Value))
                                {
                                    if (DataModul.DB_EventTable.Fields["causal"].AsDouble() > 0.0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields["causal"].AsInt();
                                        LD = "";
                                        DataModul.Textlese(AAA, out Kont[0], out LD);
                                        if (Kont[0] != "")
                                        {
                                            Kont1[7] = " " + Kont[0].Trim() + " ";
                                            Kont[0] = "";
                                        }
                                        goto IL_09bb;
                                    }

                                }
                            }
                            goto IL_0ae1;
                        IL_09bb:
                            num = 112;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields["an"].Value))
                            {
                                if (DataModul.DB_EventTable.Fields["an"].AsInt() > 0)
                                {
                                    AAA = DataModul.DB_EventTable.Fields["an"].AsInt();
                                    LD = "";
                                    DataModul.Textlese(AAA, out Kont[0], out LD);

                                }
                            }
                            goto IL_0a58;
                        IL_0a58: // <========== 3
                            num = 117;
                            if (Kont[0].Trim() == "")
                            {
                                Kont[0] = "an";
                            }
                            goto IL_0a86;
                        IL_0a86:
                            num = 120;
                            if (Kont[0].Trim() == "°")
                            {
                                Kont[0] = "";
                            }
                            goto IL_0ab4;
                        IL_0ab4:
                            num = 123;
                            Kont1[7] = Kont[0].Trim() + " " + Kont1[7] + " ";
                            goto IL_0ae1;
                        IL_0ae1: // <========== 4
                            num = 127;
                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                LD = "";
                                Kont[0] = DataModul.TextLese1(AAA);
                                if (Kont[0] != "")
                                {
                                    Kont1[7] = Strings.Trim(Kont1[7] + " " + Kont[0].Trim()) + " ";

                                }
                            }
                            goto IL_0bb8;
                        IL_0bb8: // <========== 3
                            num = 133;
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString().Trim() == "")
                            {
                                UbgT = UbgT.Trim() + " ?";
                            }
                            goto IL_0c10;
                        IL_0c10:
                            num = 136;
                            if (DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt() != 0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                //bBB5 = ref asPersDates[0];
                                LD = "";
                                DataModul.Textlese(AAA, out Kont[0], out LD);
                                if (Kont[0] != "")
                                {
                                    Kont1[6] = " " + Kont[0].Trim() + " ";

                                }
                            }
                            goto IL_0cd2;
                        IL_0cd2: // <========== 3
                            num = 142;
                            if (DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().TrimEnd() != "" | DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().TrimEnd() != "")
                            {
                                Kont[Ubg - 90] = (Kont1[1] + " " + Kont1[2] + Kont1[3] + Kont1[4] + Kont1[5] + Kont1[7]).Trim() + " " + UbgT + Kont1[6];
                                UbgT = "";
                                if (Aus[2] == "Y" | Aus[3] == "Y")
                                {
                                    Kont[Ubg - 85] = " {";
                                }
                                goto IL_0e30;
                            }
                            goto IL_0fc8;
                        IL_0e30:
                            num = 148;
                            if (Aus[2] == "Y")
                            {
                                Kont[Ubg - 85] = Kont[Ubg - 85] + DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().TrimEnd();
                            }
                            goto IL_0e97;
                        IL_0e97:
                            num = 151;
                            if (Aus[3] == "Y")
                            {
                                Kont[Ubg - 85] = Kont[Ubg - 85] + " " + DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().TrimEnd();
                            }
                            goto IL_0f03;
                        IL_0f03:
                            num = 154;
                            if (Aus[2] == "Y" | Aus[3] == "Y")
                            {
                                Kont[Ubg - 85] = Kont[Ubg - 85].Trim() + "}";
                            }
                            goto IL_0f69;
                        IL_0f69:
                            num = 157;
                            if (Datschalt != 22)
                            {
                                if (MyProject.Forms.Ausw.Kontroll[12].CheckState == CheckState.Unchecked)
                                {
                                    Kont[Ubg - 85] = Text_Retweg(Kont[Ubg - 85]);

                                }
                            }
                            goto IL_11cc;
                        IL_0fc8: // <========== 4
                            num = 164;
                            lErl = 33;
                            Kont[Ubg - 90] = Kont1[1].Trim();
                            Kont[Ubg - 90] = Kont[Ubg - 90].Trim() + " " + Kont1[2].Trim();
                            Kont[Ubg - 90] = Kont[Ubg - 90].Trim() + " " + Kont1[3].Trim();
                            Kont[Ubg - 90] = Kont[Ubg - 90].Trim() + " " + Kont1[4].Trim();
                            Kont[Ubg - 90] = Kont[Ubg - 90].Trim() + " " + Kont1[5].Trim();
                            Kont[Ubg - 90] = Kont[Ubg - 90].Trim() + " " + Kont1[7].Trim();
                            Kont[Ubg - 90] = Kont[Ubg - 90].Trim() + " " + UbgT.Trim();
                            Kont[Ubg - 90] = Kont[Ubg - 90].Trim() + " " + Kont1[6].Trim();
                            Kont[Ubg - 90] = Kont[Ubg - 90].Trim();
                            goto IL_11cc;
                        IL_11cc: // <========== 4
                            num = 175;
                            if (_Modul1.Instance.Schalt != 200)
                            {
                                if (MyProject.Forms.Ausw.Kontroll[5].Checked & PerPos == 1 | MyProject.Forms.Ausw.Kontroll[7].Checked & PerPos == 2 | MyProject.Forms.Ausw.Kontroll[8].Checked & PerPos == 3)
                                {
                                    LfNR = 0;
                                    SLQuellenDatum(ref PersInArb, Art, ref LfNR);
                                    if (Kont1[9].Trim() != "")
                                    {
                                        UbgT = " Quelle: " + Kont1[9].Trim();
                                        Kont[Ubg - 90] = Kont[Ubg - 90].Trim() + UbgT;

                                    }
                                }
                                goto IL_12ed;
                            }
                            goto IL_1585;
                        IL_12ed: // <========== 3
                            num = 185;
                            if (MyProject.Forms.Ausw.Kontroll[4].Checked & PerPos == 1)
                            {
                                PersSp = PersInArb;
                                text = "";
                                b = 1;
                                while (unchecked(b) <= 100u)
                                {
                                    KontSP1[b] = Kont1[b];
                                    KontSP[b] = Kont[b];
                                    Kont[b] = "";
                                    Kont1[b] = "";
                                    b = (byte)unchecked((uint)(b + 1));
                                }

                                LfNR = 0;
                                LD = "";
                                Zeugsu(Art, 0, LfNR, 0L);
                                PersInArb = PersSp;
                                if (Kont1[20] != "")
                                {
                                    text = ", Zeugen: " + Kont1[20];
                                }
                                b = 1;
                                while (unchecked(b) <= 100u)
                                {
                                    Kont1[b] = KontSP1[b];
                                    Kont[b] = KontSP[b];
                                    KontSP[b] = "";
                                    KontSP1[b] = "";
                                    b = (byte)unchecked((uint)(b + 1));
                                }
                                Kont[Ubg - 90] = Kont[Ubg - 90].Trim() + text;
                            }
                            Kont[Ubg - 80] = Kont1[1];
                            Kont[Ubg - 70] = UbgT;
                            if (MyProject.Forms.Ausw.Kontroll[12].CheckState == CheckState.Unchecked)
                            {
                                Kont[Ubg - 70] = Text_Retweg(Kont[Ubg - 70]);
                                Kont[Ubg - 80] = Text_Retweg(Kont[Ubg - 80]);
                                Kont[Ubg - 90] = Text_Retweg(Kont[Ubg - 90]);
                            }
                            goto IL_1574;
                        IL_1574:
                            num = 214;
                            UbgT = "";
                            goto IL_1585;
                        IL_1585: // <========== 7
                            num = 215;
                            lErl = 2;
                            num5 = (short)unchecked(num5 + 1);
                            if (num5 <= 107)
                            {
                                goto IL_006e;
                            }
                            Kont[25] = RechWithProps();
                            goto end_IL_0000_2;
                        IL_164f:
                            num4 = unchecked(num2 + 1);
                            goto IL_1653;
                        IL_1653:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 12:
                                    goto IL_006e;
                                case 24:
                                case 27:
                                case 28:
                                    goto IL_01b5;
                                case 63:
                                case 64:
                                    goto IL_04a5;
                                case 70:
                                case 73:
                                case 74:
                                case 75:
                                    goto IL_05a8;
                                case 83:
                                case 84:
                                case 85:
                                case 86:
                                    goto IL_06b4;
                                case 100:
                                case 101:
                                    goto IL_086b;
                                case 102:
                                case 103:
                                    goto IL_08a2;
                                case 111:
                                case 112:
                                    goto IL_09bb;
                                case 115:
                                case 116:
                                case 117:
                                    goto IL_0a58;
                                case 119:
                                case 120:
                                    goto IL_0a86;
                                case 122:
                                case 123:
                                    goto IL_0ab4;
                                case 124:
                                case 125:
                                case 126:
                                case 127:
                                    goto IL_0ae1;
                                case 131:
                                case 132:
                                case 133:
                                    goto IL_0bb8;
                                case 135:
                                case 136:
                                    goto IL_0c10;
                                case 140:
                                case 141:
                                case 142:
                                    goto IL_0cd2;
                                case 147:
                                case 148:
                                    goto IL_0e30;
                                case 150:
                                case 151:
                                    goto IL_0e97;
                                case 153:
                                case 154:
                                    goto IL_0f03;
                                case 156:
                                case 157:
                                    goto IL_0f69;
                                case 94:
                                case 95:
                                case 96:
                                case 164:
                                    goto IL_0fc8;
                                case 160:
                                case 161:
                                case 162:
                                case 174:
                                case 175:
                                    goto IL_11cc;
                                case 183:
                                case 184:
                                case 185:
                                    goto IL_12ed;
                                case 213:
                                case 214:
                                    goto IL_1574;
                                case 20:
                                case 29:
                                case 39:
                                case 46:
                                case 176:
                                case 215:
                                    goto IL_1585;
                                case 36:
                                case 218:
                                case 220:
                                case 224:
                                case 225:
                                case 231:
                                case 232:
                                case 233:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 6661;
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
    public string Datwand1(string Datu, string Ds)
    {
        Datu = Datu + "        ".Left(8);
        checked
        {
            if (DAus[73] == "1" & DAus[105].AsDouble() > 0.0 && Datu.AsDouble() > (DAus[105] + "0000").AsDouble())
            {
                Datu = DTxt[14];
            }
            else if (DAus[74] == "1" & DAus[109].AsDouble() > 0.0 && Datu.AsDouble() > (DAus[109] + "0000").AsDouble())
            {
                Datu = "(" + Datu.Left(4) + ")";
            }
            else
            {
                if (Datu.Trim() == "")
                {
                    return Datu;
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
                    short num = 1;
                    do
                    {
                        short num2 = (short)Strings.InStr(Datu, " ");
                        if (num2 > 0)
                        {
                            StringType.MidStmtStr(ref Datu, num2, 1, ".");
                        }
                        num = (short)unchecked(num + 1);
                    }
                    while (num <= 2);
                }
                if (Art == EEventArt.eA_Birth)
                {
                    Datum2 = "           " + Datu.Right(10);
                }
                if (Art == EEventArt.eA_Baptism & Datum2.Trim() == "")
                {
                    Datum2 = "           " + Datu.Right(10);
                }
                if (Art == EEventArt.eA_Death)
                {
                    Datum1 = "           " + Datu.Right(10);
                }
                if (Art == EEventArt.eA_Burial & Datum1.Trim() == "")
                {
                    Datum1 = "           " + Datu.Right(10);
                }
                switch (Ds.Trim())
                {
                    case "U":
                    case "u":
                        Datu = "um " + Datu;
                        break;
                    case "V":
                    case "v":
                        Datu = "vor " + Datu;
                        break;
                    case "N":
                    case "n":
                        Datu = "nach " + Datu;
                        break;
                    case "?":
                        Datu += " ?";
                        break;
                    case "r":
                    case "R":
                        Datu = "errech " + Datu;
                        break;
                    case "Z":
                    case "z":
                        Datu = "zwischen " + Datu;
                        break;
                    case "a":
                    case "A":
                        Datu = " und " + Datu;
                        break;
                    case "von":
                        Datu = "von " + Datu;
                        break;
                    case "b":
                    case "B":
                        Datu = " bis " + Datu;
                        break;
                    default:
                        if (Conversions.ToDouble(DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString().Trim()) == 0.0 && Art > EEventArt.eA_100 & Art < EEventArt.eA_105 | Art > EEventArt.eA_400 && Datschalt != 14 && Datu.Length == 10 & Schalt != 10 && DAus[68] == "1")
                        {
                            Datu = "am " + Datu;
                        }
                        break;
                }
            }
        }
        return Datu;
    }


    //public void ortles(int OrtNr, byte Schalt)
    //{
    //    int try0000_dispatch = -1;
    //    int num3 = default;
    //    int num2 = default;
    //    int num = default;
    //    int lErl = default;
    //    string[] array = default;
    //    byte b = default;
    //    while (true)
    //    {
    //        try
    //        {
    //            /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
    //            ;
    //            int num4;
    //            int AAA;
    //            string LD;
    //            switch (try0000_dispatch)
    //            {
    //                default:
    //                    ProjectData.ClearProjectError();
    //                    num3 = 2;
    //                    goto IL_0008;
    //                case 2671:
    //                    {
    //                        num2 = num;
    //                        switch (num3)
    //                        {
    //                            case 3:
    //                                break;
    //                            case 2:
    //                            case 4:
    //                            case 5:
    //                                goto IL_0821;
    //                            case 1:
    //                                goto IL_08c5;
    //                            default:
    //                                goto end_IL_0000;
    //                        }
    //                        goto IL_07e8;
    //                    }
    //                IL_07e8:
    //                    num = 85;
    //                    lErl = 2;
    //                    goto IL_07ef;
    //                IL_07ef:
    //                    num = 86;
    //                    if (Information.Err().Number == 3021)
    //                    {
    //                        goto IL_0804;
    //                    }
    //                    goto IL_0821;
    //                IL_0821:
    //                    num = 90;
    //                    if (Information.Err().Number == 3022)
    //                    {
    //                        goto IL_0836;
    //                    }
    //                    goto IL_0850;
    //                IL_0850:
    //                    num = 94;
    //                    if (Information.Err().Number == 94)
    //                    {
    //                        goto IL_0862;
    //                    }
    //                    goto IL_087c;
    //                IL_087c:
    //                    num = 98;
    //                    if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
    //                    {
    //                        ProjectData.EndApp();
    //                    }
    //                    goto IL_08a2;
    //                IL_0725:
    //                    num = 74;
    //                    DataModul.DSB_OrtIdxTable.Fields["Name"].Value = Namen;
    //                    goto IL_0747;
    //                IL_08a2:
    //                    num = 101;
    //                    ProjectData.ClearProjectError();
    //                    if (num2 == 0)
    //                    {
    //                        throw ProjectData.CreateProjectError(-2146828268);
    //                    }
    //                    num4 = num2;
    //                    goto IL_08c9;
    //                IL_0747:
    //                    num = 75;
    //                    DataModul.DSB_OrtIdxTable.Fields["OrtNr"].Value = OrtNr;
    //                    goto IL_076a;
    //                IL_076a:
    //                    num = 76;
    //                    DataModul.DSB_OrtIdxTable.Fields["Ind"].Value = Ind1;
    //                    goto IL_078c;
    //                IL_08c9:
    //                    num2 = 0;
    //                    switch (num4)
    //                    {
    //                        case 1:
    //                            break;
    //                        case 2:
    //                            goto IL_0008;
    //                        case 3:
    //                            goto IL_0012;
    //                        case 4:
    //                            goto IL_0017;
    //                        case 5:
    //                            goto IL_0022;
    //                        case 6:
    //                            goto IL_002e;
    //                        case 7:
    //                            goto IL_0082;
    //                        case 8:
    //                            goto IL_0094;
    //                        case 9:
    //                            goto IL_00d6;
    //                        case 10:
    //                            goto IL_0119;
    //                        case 11:
    //                            goto IL_012e;
    //                        case 12:
    //                        case 13:
    //                            goto IL_0147;
    //                        case 14:
    //                            goto IL_018a;
    //                        case 15:
    //                            goto IL_019f;
    //                        case 16:
    //                        case 17:
    //                            goto IL_01b3;
    //                        case 18:
    //                            goto IL_01f6;
    //                        case 19:
    //                            goto IL_020b;
    //                        case 20:
    //                        case 21:
    //                            goto IL_021f;
    //                        case 22:
    //                            goto IL_0248;
    //                        case 23:
    //                            goto IL_028b;
    //                        case 24:
    //                            goto IL_02a0;
    //                        case 25:
    //                        case 26:
    //                        case 27:
    //                            goto IL_02b9;
    //                        case 28:
    //                            goto IL_02fc;
    //                        case 29:
    //                            goto IL_0311;
    //                        case 30:
    //                        case 31:
    //                            goto IL_0325;
    //                        case 32:
    //                            goto IL_034e;
    //                        case 33:
    //                        case 34:
    //                            goto IL_0378;
    //                        case 35:
    //                            goto IL_0380;
    //                        case 36:
    //                            goto IL_03a9;
    //                        case 37:
    //                        case 38:
    //                            goto IL_03d3;
    //                        case 39:
    //                            goto IL_03db;
    //                        case 40:
    //                            goto IL_03f0;
    //                        case 41:
    //                        case 42:
    //                            goto IL_03fc;
    //                        case 43:
    //                            goto IL_0409;
    //                        case 44:
    //                            goto IL_0454;
    //                        case 45:
    //                            goto IL_049f;
    //                        case 47:
    //                        case 48:
    //                            goto IL_04ef;
    //                        case 49:
    //                            goto IL_04fb;
    //                        case 50:
    //                            goto IL_0510;
    //                        case 51:
    //                        case 52:
    //                            goto IL_0522;
    //                        case 54:
    //                            goto IL_057b;
    //                        case 55:
    //                            goto IL_0588;
    //                        case 57:
    //                            goto IL_05ab;
    //                        case 58:
    //                            goto IL_05b5;
    //                        case 60:
    //                            goto IL_0612;
    //                        case 61:
    //                            goto IL_061b;
    //                        case 63:
    //                            goto IL_063b;
    //                        case 64:
    //                            goto IL_063f;
    //                        case 53:
    //                        case 56:
    //                        case 59:
    //                        case 62:
    //                        case 65:
    //                        case 66:
    //                            goto IL_066a;
    //                        case 67:
    //                            goto IL_0681;
    //                        case 68:
    //                        case 69:
    //                            goto IL_06a7;
    //                        case 70:
    //                            goto IL_06af;
    //                        case 71:
    //                            goto IL_06cb;
    //                        case 72:
    //                            goto IL_06e5;
    //                        case 73:
    //                            goto IL_06f3;
    //                        case 74:
    //                            goto IL_0725;
    //                        case 75:
    //                            goto IL_0747;
    //                        case 76:
    //                            goto IL_076a;
    //                        case 77:
    //                            goto IL_078c;
    //                        case 78:
    //                        case 79:
    //                        case 80:
    //                        case 81:
    //                            goto IL_079c;
    //                        case 82:
    //                            goto IL_07b6;
    //                        case 85:
    //                            goto IL_07e8;
    //                        case 86:
    //                            goto IL_07ef;
    //                        case 87:
    //                            goto IL_0804;
    //                        case 88:
    //                        case 90:
    //                            goto IL_0821;
    //                        case 91:
    //                            goto IL_0836;
    //                        case 92:
    //                        case 94:
    //                            goto IL_0850;
    //                        case 95:
    //                            goto IL_0862;
    //                        case 96:
    //                        case 98:
    //                            goto IL_087c;
    //                        case 99:
    //                        case 101:
    //                            goto IL_08a2;
    //                        default:
    //                            goto end_IL_0000;
    //                        case 46:
    //                        case 83:
    //                        case 84:
    //                        case 102:
    //                            goto end_IL_0000_2;
    //                    }
    //                    goto default;
    //                IL_0862:
    //                    num = 95;
    //                    ProjectData.ClearProjectError();
    //                    if (num2 == 0)
    //                    {
    //                        throw ProjectData.CreateProjectError(-2146828268);
    //                    }
    //                    goto IL_08c5;
    //                IL_078c:
    //                    num = 77;
    //                    DataModul.DSB_OrtIdxTable.Commit();
    //                    goto IL_079c;
    //                IL_0836:
    //                    num = 91;
    //                    ProjectData.ClearProjectError();
    //                    if (num2 == 0)
    //                    {
    //                        throw ProjectData.CreateProjectError(-2146828268);
    //                    }
    //                    goto IL_08c5;
    //                IL_079c:
    //                    num = 81;
    //                    if (DAus[91] != "1")
    //                    {
    //                        goto end_IL_0000_2;
    //                    }
    //                    goto IL_07b6;
    //                IL_0804:
    //                    num = 87;
    //                    ProjectData.ClearProjectError();
    //                    if (num2 == 0)
    //                    {
    //                        throw ProjectData.CreateProjectError(-2146828268);
    //                    }
    //                    goto IL_08c5;
    //                IL_07b6:
    //                    num = 82;
    //                    UbgT = UbgT + " [" + OrtNr.AsString().Trim() + "] ";
    //                    goto end_IL_0000_2;
    //                IL_08c5:
    //                    num4 = num2 + 1;
    //                    goto IL_08c9;
    //                IL_0008:
    //                    num = 2;
    //                    array = new string[8];
    //                    goto IL_0012;
    //                IL_0012:
    //                    num = 3;
    //                    b = 1;
    //                    goto IL_0017;
    //                IL_0017:
    //                    num = 4;
    //                    array[b] = "";
    //                    goto IL_0022;
    //                IL_0022:
    //                    num = 5;
    //                    b = checked((byte)unchecked((uint)(b + 1)));
    //                    if (b <= 6u)
    //                    {
    //                        goto IL_0017;
    //                    }
    //                    goto IL_002e;
    //                IL_002e:
    //                    num = 6;
    //                    DataModul.DB_PlaceTable.Seek("=", OrtNr);
    //                    goto IL_0082;
    //                IL_0082:
    //                    num = 7;
    //                    if (!DataModul.DB_PlaceTable.NoMatch)
    //                    {
    //                        goto IL_0094;
    //                    }
    //                    goto IL_079c;
    //                IL_0094:
    //                    num = 8;
    //                    AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt();
    //                    LD = "";
    //                    DataModul.Textlese(AAA, out array[1], out LD);
    //                    goto IL_00d6;
    //                IL_00d6:
    //                    num = 9;
    //                    AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.OrtsTeil].AsInt();
    //                    LD = "";
    //                    DataModul.Textlese(AAA, out array[2], out LD);
    //                    goto IL_0119;
    //                IL_0119:
    //                    num = 10;
    //                    if (array[2] != "")
    //                    {
    //                        goto IL_012e;
    //                    }
    //                    goto IL_0147;
    //                IL_012e:
    //                    num = 11;
    //                    array[2] = "-" + array[2].Trim();
    //                    goto IL_0147;
    //                IL_0147:
    //                    num = 13;
    //                    AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Kreis].AsInt();
    //                    LD = "";
    //                    DataModul.Textlese(AAA, out array[3], out LD);
    //                    goto IL_018a;
    //                IL_018a:
    //                    num = 14;
    //                    if (array[3] != "")
    //                    {
    //                        goto IL_019f;
    //                    }
    //                    goto IL_01b3;
    //                IL_019f:
    //                    num = 15;
    //                    array[3] = ", " + array[3];
    //                    goto IL_01b3;
    //                IL_01b3:
    //                    num = 17;
    //                    AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Land].AsInt();
    //                    LD = "";
    //                    DataModul.Textlese(AAA, out array[4], out LD);
    //                    goto IL_01f6;
    //                IL_01f6:
    //                    num = 18;
    //                    if (array[4] != "")
    //                    {
    //                        goto IL_020b;
    //                    }
    //                    goto IL_021f;
    //                IL_020b:
    //                    num = 19;
    //                    array[4] = ", " + array[4];
    //                    goto IL_021f;
    //                IL_021f:
    //                    num = 21;
    //                    if (!Information.IsDBNull(DataModul.DB_PlaceTable.Fields[PlaceFields.Polname].Value))
    //                    {
    //                        goto IL_0248;
    //                    }
    //                    goto IL_02b9;
    //                IL_0248:
    //                    num = 22;
    //                    AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.PolName].AsInt();
    //                    LD = "";
    //                    DataModul.Textlese(AAA, out array[7], out LD);
    //                    goto IL_028b;
    //                IL_028b:
    //                    num = 23;
    //                    if (array[7] != "")
    //                    {
    //                        goto IL_02a0;
    //                    }
    //                    goto IL_02b9;
    //                IL_02a0:
    //                    num = 24;
    //                    array[7] = " (" + array[7] + ")";
    //                    goto IL_02b9;
    //                IL_02b9:
    //                    num = 27;
    //                    AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Staat].AsInt();
    //                    LD = "";
    //                    DataModul.Textlese(AAA, out array[5], out LD);
    //                    goto IL_02fc;
    //                IL_02fc:
    //                    num = 28;
    //                    if (array[5] != "")
    //                    {
    //                        goto IL_0311;
    //                    }
    //                    goto IL_0325;
    //                IL_0311:
    //                    num = 29;
    //                    array[5] = ", " + array[5];
    //                    goto IL_0325;
    //                IL_0325:
    //                    num = 31;
    //                    if (!Information.IsDBNull(DataModul.DB_PlaceTable.Fields[PlaceFields.Zusatz].Value))
    //                    {
    //                        goto IL_034e;
    //                    }
    //                    goto IL_0378;
    //                IL_034e:
    //                    num = 32;
    //                    array[6] = DataModul.DB_PlaceTable.Fields[PlaceFields.Zusatz].AsString().Trim();
    //                    goto IL_0378;
    //                IL_0378:
    //                    ProjectData.ClearProjectError();
    //                    num3 = 3;
    //                    goto IL_0380;
    //                IL_0380:
    //                    num = 35;
    //                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Zusatz].Value))
    //                    {
    //                        goto IL_03a9;
    //                    }
    //                    goto IL_03d3;
    //                IL_03a9:
    //                    num = 36;
    //                    array[6] = DataModul.DB_EventTable.Fields[EventFields.Zusatz].AsString().Trim();
    //                    goto IL_03d3;
    //                IL_03d3:
    //                    ProjectData.ClearProjectError();
    //                    num3 = 4;
    //                    goto IL_03db;
    //                IL_03db:
    //                    num = 39;
    //                    if (array[6] == "")
    //                    {
    //                        goto IL_03f0;
    //                    }
    //                    goto IL_03fc;
    //                IL_03f0:
    //                    num = 40;
    //                    array[6] = "in";
    //                    goto IL_03fc;
    //                IL_03fc:
    //                    num = 42;
    //                    if (Schalt == 20)
    //                    {
    //                        goto IL_0409;
    //                    }
    //                    goto IL_04ef;
    //                IL_0409:
    //                    num = 43;
    //                    UbgT = UbgT + Strings.Left(DataModul.DB_PlaceTable.Fields[PlaceFields.Terr].AsString().Trim() + "   ", 3) + "    ";
    //                    goto IL_0454;
    //                IL_0454:
    //                    num = 44;
    //                    UbgT = UbgT + Strings.Left(DataModul.DB_PlaceTable.Fields[PlaceFields.Staatk].AsString().Trim() + " ", 1) + "    ";
    //                    goto IL_049f;
    //                IL_049f:
    //                    num = 45;
    //                    UbgT = UbgT + Strings.Left(DataModul.DB_PlaceTable.Fields[PlaceFields.PLZ].AsString().Trim() + "      ", 6) + " ";
    //                    goto end_IL_0000_2;
    //                IL_04ef:
    //                    num = 48;
    //                    if (Schalt == 0)
    //                    {
    //                        goto IL_04fb;
    //                    }
    //                    goto IL_057b;
    //                IL_04fb:
    //                    num = 49;
    //                    if (array[7] != "")
    //                    {
    //                        goto IL_0510;
    //                    }
    //                    goto IL_0522;
    //                IL_0510:
    //                    num = 50;
    //                    array[1] += array[7];
    //                    goto IL_0522;
    //                IL_0522:
    //                    num = 52;
    //                    UbgT = array[1].TrimEnd() + array[2].TrimEnd() + array[3].TrimEnd() + array[4].TrimEnd() + array[5].TrimEnd();
    //                    goto IL_066a;
    //                IL_057b:
    //                    num = 54;
    //                    if (Schalt == 200)
    //                    {
    //                        goto IL_0588;
    //                    }
    //                    goto IL_05ab;
    //                IL_0588:
    //                    num = 55;
    //                    UbgT = array[1].TrimEnd() + array[2].TrimEnd();
    //                    goto IL_066a;
    //                IL_05ab:
    //                    num = 57;
    //                    if (Schalt == 21)
    //                    {
    //                        goto IL_05b5;
    //                    }
    //                    goto IL_0612;
    //                IL_05b5:
    //                    num = 58;
    //                    UbgT = array[1].TrimEnd() + array[7] + array[2].TrimEnd() + array[3].TrimEnd() + array[4].TrimEnd() + array[5].TrimEnd();
    //                    goto IL_066a;
    //                IL_0612:
    //                    num = 60;
    //                    if (Schalt == 6)
    //                    {
    //                        goto IL_061b;
    //                    }
    //                    goto IL_063b;
    //                IL_061b:
    //                    num = 61;
    //                    UbgT = array[1].TrimEnd() + array[2].TrimEnd();
    //                    goto IL_066a;
    //                IL_063b:
    //                    num = 63;
    //                    goto IL_063f;
    //                IL_063f:
    //                    num = 64;
    //                    UbgT = array[6].Trim() + " " + array[1].TrimEnd() + array[2].TrimEnd();
    //                    goto IL_066a;
    //                IL_066a:
    //                    num = 66;
    //                    if (UbgT == "")
    //                    {
    //                        goto IL_0681;
    //                    }
    //                    goto IL_06a7;
    //                IL_0681:
    //                    num = 67;
    //                    UbgT = array[3].TrimEnd() + array[4].TrimEnd() + array[5].TrimEnd();
    //                    goto IL_06a7;
    //                IL_06a7:
    //                    ProjectData.ClearProjectError();
    //                    num3 = 5;
    //                    goto IL_06af;
    //                IL_06af:
    //                    num = 70;
    //                    if ((Ind1).AsDouble() > 0.0)
    //                    {
    //                        goto IL_06cb;
    //                    }
    //                    goto IL_079c;
    //                IL_06cb:
    //                    num = 71;
    //                    if (Namen != "")
    //                    {
    //                        goto IL_06e5;
    //                    }
    //                    goto IL_079c;
    //                IL_06e5:
    //                    num = 72;
    //                    DataModul.DSB_OrtIdxTable.AddNew();
    //                    goto IL_06f3;
    //                IL_06f3:
    //                    num = 73;
    //                    DataModul.DSB_OrtIdxTable.Fields["Ort"].Value = array[1].TrimEnd() + array[2].TrimEnd();
    //                    goto IL_0725;
    //                end_IL_0000:
    //                    break;
    //            }
    //        }
    //        catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
    //        {
    //            ProjectData.SetProjectError(obj, lErl);
    //            try0000_dispatch = 2671;
    //            continue;
    //        }
    //        throw ProjectData.CreateProjectError(-2146828237);
    //    end_IL_0000_2:
    //        break;
    //    }
    //    if (num2 != 0)
    //    {
    //        ProjectData.ClearProjectError();
    //    }
    //}
    public string ortles(int OrtNr, int Schalt)
    {
        string[] array = new string[8];
        array.Initialize();

        if (DataModul.Place.ReadData(OrtNr, out var placeData))
        {
            array[1] = placeData.sOrt;
            array[2] = placeData.sOrtsteil;
            array[3] = placeData.sKreis;
            array[4] = placeData.sLand;
            array[5] = placeData.sStaat;
            array[6] = placeData.sZusatz;
            array[7] = placeData.sPolName;

            if (array[2] != "")
            {
                array[2] = "-" + array[2].Trim();
            }
            if (array[3] != "")
            {
                array[3] = ", " + array[3];
            }
            if (array[4] != "")
            {
                array[4] = ", " + array[4];
            }
            if (array[5] != "")
            {
                array[5] = ", " + array[5];
            }
            if (array[7] != "")
            {
                array[7] = " (" + array[7] + ")";
            }

            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Zusatz].Value))
            {
                array[6] = DataModul.DB_EventTable.Fields[EventFields.Zusatz].AsString().Trim();
            }

            if (array[6] == "")
            {
                array[6] = "in";
            }

            switch (Schalt)
            {
                case 20:
                    UbgT = UbgT + placeData.sTerr.PadRight(3) + "    ";
                    UbgT = UbgT + placeData.sStaatk.PadRight(1) + "    ";
                    UbgT = UbgT + placeData.sPLZ.PadRight(6) + " ";
                    return UbgT;
                case 0:
                    if (array[7] != "")
                    {
                        array[1] += array[7];
                    }
                    UbgT = array[1].TrimEnd() + array[2].TrimEnd() + array[3].TrimEnd() + array[4].TrimEnd() + array[5].TrimEnd();
                    break;
                case 200:
                    UbgT = array[1].TrimEnd() + array[2].TrimEnd();
                    break;
                case 21:
                    UbgT = array[1].TrimEnd() + array[7] + array[2].TrimEnd() + array[3].TrimEnd() + array[4].TrimEnd() + array[5].TrimEnd();
                    break;
                case 6:
                    UbgT = array[1].TrimEnd() + array[2].TrimEnd();
                    break;
                default:
                    UbgT = array[6].Trim() + " " + array[1].TrimEnd() + array[2].TrimEnd();
                    break;
            }
            if (UbgT == "")
            {
                UbgT = array[3].TrimEnd() + array[4].TrimEnd() + array[5].TrimEnd();
            }

            if (Ind1.AsInt() > 0)
            {
                if (Namen != "")
                {
                    DataModul.DSB_OrtIdxTable.AddNew();
                    DataModul.DSB_OrtIdxTable.Fields["Ort"].Value = array[1].TrimEnd() + array[2].TrimEnd();
                    DataModul.DSB_OrtIdxTable.Fields["Name"].Value = Namen;
                    DataModul.DSB_OrtIdxTable.Fields["OrtNr"].Value = OrtNr;
                    DataModul.DSB_OrtIdxTable.Fields["Ind"].Value = Ind1;
                    DataModul.DSB_OrtIdxTable.Update();
                }
            }
        }
        if (DAus[91] == "1")
        {
            UbgT = UbgT + " [" + OrtNr.AsString().Trim() + "] ";
        }
        return UbgT;

    }
    public void Paten2(int PersInArb, ref string Pattext, long Ahne)
    {
        Pat = false;
        int persInArb = PersInArb;
        foreach (var cLink in DataModul.Link.ReadAllFams(PersInArb, ELinkKennz.lkGodparent))
        {
            PersInArb = cLink.iPersNr;
            Person_ReadNames(PersInArb, _Modul1.Instance.Person);
            Namenindex(Ahne);
            Person.SetFullSurname(Person_FullSurname(_Modul1.Instance.Person, _Modul1.Instance.DAus[89] == "1"));
            if (Pattext == "")
            {
                if (DAus[76] == "1")
                {
                    Pattext = " <" + PersInArb.AsString().Trim() + ">" + Strings.Trim(_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames.Trim() + " " + _Modul1.Instance.Person.FullSurName.Trim());
                }
                else
                {
                    Pattext = Strings.Trim(_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames.Trim() + " " + _Modul1.Instance.Person.FullSurName.Trim());
                }
            }
            else if (DAus[76] == "1")
            {
                Pattext = Pattext + "; [" + PersInArb.AsString().Trim() + "] " + Strings.Trim(_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames.Trim() + " " + _Modul1.Instance.Person.FullSurName.Trim());
            }
            else
            {
                Pattext = Pattext + "; " + Strings.Trim(_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames.Trim() + " " + _Modul1.Instance.Person.FullSurName.Trim());
            }
        }

        string sBem2 = (DataModul.Person.Seek(persInArb)?.Fields["Bem2"]).AsString();
        if (Information.IsDBNull(sBem2))
        {
            return;
        }
        if (sBem2.Trim()!= "" )
        {
            UbgT1 = sBem2;
            _Modul1.Instance.UbgT1 = Text_Retweg(_Modul1.Instance.UbgT1);
            if (Pattext == "")
            {
                Pattext = UbgT1.Trim();
                UbgT1 = "";
            }
            else
            {
                Pattext = Pattext.Trim() + "; " + UbgT1.Trim();
                UbgT1 = "";
            }
        }
        if (Pattext != "" && DAus[87].AsDouble() == 0.0)
        {
            Pattext = Text_Retweg(Pattext);
        }
    }

    [Obsolete]
    public void PerSatzLes(int PersInArb)
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
                checked
                {
                    int num4;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 1619:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0501;
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
                                goto IL_0505;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            if (PersInArb == 0)
                            {
                                goto end_IL_0000_2;
                            }
                            lErl = 1;
                            do
                            {
                                if (PersInArb < 1)
                                {
                                    PersInArb = 1;
                                }
                                if (DataModul.DB_PersonTable.RecordCount == 0)
                                {
                                    PersInArb = 0;
                                    Schalt = 5;
                                    goto end_IL_0000_2;
                                }
                                DataModul.DB_PersonTable.MoveLast();
                                int num5 = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                                DataModul.DB_PersonTable.Seek("=", PersInArb.AsString());
                                if (DataModul.DB_PersonTable.NoMatch)
                                {
                                    switch (Schalt)
                                    {
                                        case 0:
                                        case 4:
                                            if (PersInArb > 1 & PersInArb < num5)
                                            {
                                                PersInArb--;
                                                continue;
                                            }
                                            if (PersInArb > num5)
                                            {
                                                PersInArb = num5;
                                                continue;
                                            }
                                            break;
                                        case 3:
                                            if (PersInArb > 1 & PersInArb < num5)
                                            {
                                                PersInArb++;
                                                continue;
                                            }
                                            if (PersInArb > num5)
                                            {
                                                PersInArb = num5;
                                                continue;
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                if (Information.IsDBNull(DataModul.DB_PersonTable.Fields[PersonFields.Pruefen].Value))
                                {
                                    DataModul.DB_PersonTable.Edit();
                                    DataModul.DB_PersonTable.Fields[PersonFields.Pruefen].Value = "     ";
                                    DataModul.DB_PersonTable.Update();
                                }
                                if (DataModul.DB_PersonTable.Fields[PersonFields.Pruefen].AsString().Trim()== "G")
                                {
                                    string prompt = "Diese Person ist gelöscht und kann neu belegt werden";
                                    if (Interaction.MsgBox(prompt, MsgBoxStyle.Exclamation | MsgBoxStyle.YesNo | MsgBoxStyle.DefaultButton2, "Jetzt neu eingeben") == MsgBoxResult.Yes)
                                    {
                                        DataModul.DB_PersonTable.Edit();
                                        DataModul.DB_PersonTable.Fields[PersonFields.Pruefen].Value = "     ";
                                        DataModul.DB_PersonTable.Update();
                                        break;
                                    }
                                    PersInArb++;
                                    continue;
                                }
                            } while (false);

                            Kont[0] = DataModul.DB_PersonTable.Fields[PersonFields.AnlDatum].AsString();
                            if (Kont[0].Length > 0)
                            {
                                UbgT = Kont[0];
                                Kont[0] = "Anl.Datum : " + Strings.Mid(UbgT, 7, 2) + "." + Strings.Mid(UbgT, 5, 2) + "." + UbgT.Left(4);
                            }
                            else
                                Kont[0] = "Anl.Datum : " + DateTime.Today.Month.ToString() + "." + DateTime.Today.Day.ToString() + "." + DateTime.Today.Year.ToString();
                            if (DataModul.DB_PersonTable.Fields[PersonFields.EditDat].AsDouble() > 0.0)
                            {
                                UbgT = DataModul.DB_PersonTable.Fields[PersonFields.EditDat].AsString();
                                Kont[2] = "Letzte Änd.: " + Strings.Mid(UbgT, 7, 2) + "." + Strings.Mid(UbgT, 5, 2) + "." + UbgT.Left(4);
                            }
                            else
                                Kont[2] = "";
                            goto end_IL_0000_2;
                        IL_0501:
                            num4 = unchecked(num2 + 1);
                            goto IL_0505;
                        IL_0505:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 3:
                                case 12:
                                case 57:
                                case 71:
                                case 74:
                                case 75:
                                case 80:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 1619;
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

    /// <summary>
    /// Calls Rech with Datum1/Datum2 properties using local variables for ref parameters.
    /// </summary>
    private string RechWithProps()
    {
        var _d1 = Datum1;
        var _d2 = Datum2;
        var _r = Rech(ref _d1, ref _d2);
        Datum1 = _d1;
        Datum2 = _d2;
        return _r;
    }

    public string Rech(ref string datum1, ref string datum2)
    {
        double num5 = default;
        string text = default;
        double num6 = default;
        float num7 = default;

        if (!(datum2.Left(2).AsDouble() > 31.0) && !(datum1.Left(2).AsDouble() > 31.0))
        {
            if (datum2 == "")
            {
                text = "";
            }
            else

                if (datum2.Left(2).AsDouble() != 0.0 && Strings.Mid(datum2, 4, 2).AsDouble() != 0.0)
            {
                if (datum1 == "" && Aus[17] == "Y")
                {
                    datum1 = Strings.Format(DateTime.Now, "Short Date");
                }
                if (datum1.Left(2).AsDouble() != 0.0
                    && Strings.Mid(datum1, 4, 2).AsDouble() != 0.0)
                {
                    DateTime date1 = Conversions.ToDate(datum2);
                    DateTime date2 = Conversions.ToDate(datum1);
                    num5 = (date2 - date1).Days / 365.25;
                    if (num5 <= 100f || DateTime.Compare(date2, DateTime.Now) != 0)
                    {
                        if (Operators.CompareString(Strings.Mid(datum2, 4, 2), Strings.Mid(datum1, 4, 2), TextCompare: false) > 0)
                        {
                            num5 -= 1f;
                        }
                        datum1 = date2.AddYears(-(int)num5).AsString();
                        num6 = (date2 - date1).Days / 30.44;
                        if (Operators.CompareString(datum2.Left(2), datum1.Left(2), TextCompare: false) > 0)
                        {
                            num6 -= 1f;
                        }
                        if (num6 < 0f)
                        {
                            num5 -= 1f;
                            num6 += 12f;
                            datum2 = date1.AddMonths((int)(num6 - 12f)).AsString();
                        }
                        else if (num6 > 12f)
                        {
                            num5 += 1f;
                            num6 -= 12f;
                        }
                        else
                        {
                            datum1 = date2.AddMonths(-(int)num6).AsString();
                        }
                        num7 = (float)((date2 - date1).Days % 30.44);
                        text = IText[117] + num5.ToString() + IText[119] + num6.ToString() + IText[120] + num7.ToString() + IText[121];
                    }
                    else
                    {
                        text = "";
                    }
                }
                else
                {
                    text = IText[117] + " " + IText[118] + Conversion.Str(Strings.Mid(datum1, 7, 4).AsDouble() - Strings.Mid(datum2, 7, 4).AsDouble()) + IText[216];
                    if (datum1 == "")
                        text = "";
                }
            }

        }
        return text;
    }

    public void Famdatles(int FamInArb, out string[] Kont)
    {
        int try0000_dispatch = -1;
        int num = default;
        string BBB = default;
        int num2 = default;
        int num3 = default;
        int number = default;
        string ind = default;
        short num5 = default;
        int num6 = default;
        int num7 = default;
        string ds = default;
        string ds2 = default;
        byte b = default;
        string text = default;
        int lErl = default;
        Kont = new string[31];
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
                    int AAA;
                    int ortNr3;
                    EEventArt _eArt;
                    short Listart;
                    string LD;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            BBB = "";
                            goto IL_0009;
                        case 5743:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_12e9;
                                    default:
                                        goto end_IL_0000;
                                }
                                number = Information.Err().Number;
                                if (number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_12e9;
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
                                goto IL_12ed;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            num = 2;
                            ind = "";
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            num5 = 0;
                            while (num5 <= 30)
                            {
                                Kont[num5] = "";
                                num5 = (short)unchecked(num5 + 1);
                            }
                            num5 = 500;
                            goto IL_0044;
                        IL_0044: // <========== 3
                            num = 8;
                            Ubg = num5;
                            Art = (EEventArt)Ubg;
                            if (Ubg == 508)
                            {
                                Art = EEventArt.eA_602;
                            }
                            if (Ubg == 509)
                            {
                                Art = EEventArt.eA_603;
                            }
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", Ubg.AsString(), FamInArb.AsString(), "0");
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields["priv"].Value))
                                {
                                    iPrivacy = DataModul.DB_EventTable.Fields["priv"].AsInt();

                                }
                                else
                                {
                                    iPrivacy = 0;
                                }
                                goto IL_018d;
                            }
                            goto IL_0fdc;
                        IL_018d: // <========== 3
                            num = 27;
                            if (!(iPrivacy.AsDouble() > iPriv_aus.AsDouble()))
                            {
                                if (Datschalt == 2)
                                {
                                    num6 = MyProject.Forms.Hinter.List1.Items.Count - 1;
                                    num7 = 0;
                                    while (num7 <= num6)
                                    {
                                        Kol = (int)Math.Round(Conversion.Val(MyProject.Forms.Hinter.List1.Items[num7].AsString().Right(6)));
                                        if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() == Kol)
                                        {
                                            Datschalt = 3;
                                            goto end_IL_0000_2;
                                        }
                                        num7++;
                                    }
                                    goto IL_0fdc;
                                }
                                var J = 0;
                                while (unchecked(J) <= 15u)
                                {
                                    Kont1[J] = "";
                                    J++;
                                }
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                                    if (_Modul1.Instance.Schalt == 1)
                                    {
                                        Kont[Ubg - 500] = sDatu;
                                        goto IL_0fdc;
                                    }
                                    if (DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt() != 0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                        LD = "";
                                        Kont1[0] = DataModul.TextLese1(AAA);
                                        if (Kont1[0] != "")
                                        {
                                            Kont1[8] = " " + Kont1[0].Trim();
                                        }
                                    }
                                    goto IL_03e9;
                                }
                                goto IL_069a;
                            }
                            goto IL_0fdc;
                        IL_03e9: // <========== 3
                            num = 55;
                            ind = Conversion.Str(sDatu.Left(2).AsDouble() + 1.0);
                            if (Ubg == 502)
                            {
                                Kont[30] = sDatu.Left(4);
                            }
                            if (Ubg == 503 & Kont[30] == "")
                            {
                                Kont[30] = sDatu.Left(4);
                            }
                            ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                            sDatu = Datwand1(sDatu, ds);
                            Kont1[1] = sDatu;
                            if (_Modul1.Instance.Schalt == 5)
                            {
                                if (Ubg == 502)
                                {
                                    UbgT = "";
                                    if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                                    {
                                        ortNr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble());
                                        UbgT = ortles(ortNr, 1);
                                    }
                                    goto IL_0556;
                                }
                                if (Ubg == 503)
                                {
                                    UbgT = "";
                                    if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                                    {
                                        ortNr2 = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble());
                                        UbgT = ortles(ortNr2, 1);
                                    }
                                    goto IL_0604;
                                }
                            }
                            goto IL_0628;
                        IL_0556:
                            num = 71;
                            Kont[2] = sDatu + " " + UbgT;
                            goto end_IL_0000_2;
                        IL_0604:
                            num = 79;
                            Kont[3] = sDatu + " " + UbgT;
                            goto end_IL_0000_2;
                        IL_0628:
                            num = 83;
                            if (_Modul1.Instance.Schalt == 6)
                            {
                                if (Ubg == 502)
                                {
                                    UbgT = "";
                                    Kont[2] = sDatu;
                                    goto end_IL_0000_2;
                                }
                                if (Ubg == 503)
                                {
                                    UbgT = "";
                                    Kont[3] = sDatu;
                                    goto end_IL_0000_2;
                                }
                            }
                            goto IL_069a;
                        IL_069a: // <========== 3
                            num = 96;
                            if (_Modul1.Instance.Schalt != 1)
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                                {
                                    sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString().Trim(), 8);
                                    ds2 = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                    sDatu = Datwand1(sDatu, ds2);
                                    if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString().Trim(), "", TextCompare: false) == 0)
                                    {
                                        Kont1[3] = " / " + sDatu;
                                        goto IL_07ae;
                                    }
                                    Kont1[3] = sDatu;
                                }
                                goto IL_07ae;
                            }
                            goto IL_0fdc;
                        IL_07ae: // <========== 3
                            num = 110;
                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                LD = "";
                                DataModul.Textlese(AAA, out BBB, out LD);
                                if (BBB != "")
                                {
                                    Kont1[7] = " " + BBB.Trim() + " ";
                                }
                            }
                            goto IL_0853;
                        IL_0853: // <========== 3
                            num = 116;
                            BBB = "";
                            UbgT = "";
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                            {
                                if (_Modul1.Instance.Schalt == 10)
                                {
                                    Ind1 = ind;
                                }
                                goto IL_08b4;
                            }
                            goto IL_08eb;
                        IL_08b4:
                            num = 122;
                            ortNr3 = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble());
                            UbgT = ortles(ortNr3, 1);
                            goto IL_08eb;
                        IL_08eb: // <========== 3
                            num = 124;
                            b = 0;
                            while (unchecked(b) <= 100u)
                            {
                                KontSP1[b] = Kont1[b];
                                KontSP[b] = Kont[b];
                                Kont[b] = "";
                                Kont1[b] = "";
                                b = (byte)unchecked((uint)(b + 1));
                            }
                            text = "";
                            PersSp = PersInArb;
                            Kont1[20] = "";
                            _eArt = (EEventArt)Ubg;
                            Listart = 0;
                            LD = "";
                            Zeugsu(_eArt, 0, Listart, 0L);
                            Ubg = (int)_eArt;
                            PersInArb = PersSp;
                            if (Kont1[20].Trim() != "")
                            {
                                text = Kont1[20].Trim();
                            }
                            b = 0;
                            while (unchecked(b) <= 100u)
                            {
                                Kont1[b] = KontSP1[b];
                                Kont[b] = KontSP[b];
                                KontSP[b] = "";
                                KontSP1[b] = "";
                                b = (byte)unchecked((uint)(b + 1));
                            }
                            if (text != "")
                            {
                                Kont[Ubg - 460] = Kont[Ubg - 460] + text;
                            }
                            goto IL_0aba;
                        IL_0aba:
                            num = 147;
                            if (Aus[5] == "Y" | Aus[6] == "Y")
                            {
                                if ((DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString() != " " & Aus[5] == "Y").AsBool())
                                {
                                    Kont[Ubg - 480] = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString();
                                }
                                goto IL_0b78;
                            }
                            goto IL_0c90;
                        IL_0b78:
                            num = 151;
                            if ((DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString() != " " & Aus[6] == "Y").AsBool())
                            {
                                Kont[Ubg - 480] = Kont[Ubg - 480] + " " + DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString();
                            }
                            goto IL_0c1f;
                        IL_0c1f:
                            num = 154;
                            if (Kont[Ubg - 480].Trim() != "")
                            {
                                if (MyProject.Forms.Ausw.Kontroll[12].CheckState == CheckState.Unchecked)
                                {
                                    Kont[Ubg - 480] = Text_Retweg(Kont[Ubg - 480]);
                                }
                            }
                            goto IL_0c90;
                        IL_0c90: // <========== 4
                            num = 160;
                            if (_Modul1.Instance.Schalt == 10)
                            {
                                if (Kont1[1].Trim().Length > 0)
                                {
                                    if (Kont1[1].Trim().Length < 8 | Strings.Asc(Kont1[1]) > 57)
                                    {
                                        Kont1[1] = " (" + Kont1[1] + ")";
                                    }
                                }
                                goto IL_0d10;
                            }
                            Kont[Ubg - 500] = Kont1[1].Trim();
                            Kont[Ubg - 500] = Kont[Ubg - 500].Trim() + " " + Kont1[2].Trim();
                            Kont[Ubg - 500] = Kont[Ubg - 500].Trim() + " " + Kont1[7].Trim();
                            Kont[Ubg - 500] = Kont[Ubg - 500].Trim() + " " + Kont1[3].Trim();
                            Kont[Ubg - 500] = Kont[Ubg - 500].Trim() + " " + Kont1[4].Trim();
                            Kont[Ubg - 500] = Kont[Ubg - 500].Trim() + " " + Kont1[5].Trim();
                            Kont[Ubg - 500] = Kont[Ubg - 500].Trim() + " " + Kont1[6].Trim();
                            Kont[Ubg - 500] = Kont[Ubg - 500].Trim() + " " + UbgT.Trim();
                            Kont[Ubg - 500] = Kont[Ubg - 500].Trim() + " " + Kont1[6].Trim();
                            UbgT = "";
                            Kont[Ubg - 500] = Kont[Ubg - 500].Trim();
                            goto IL_0fdc;
                        IL_0d10: // <========== 3
                            num = 166;
                            Kont[Ubg - 500] = UbgT + " " + Kont1[1];
                            UbgT = "";
                            goto IL_0fdc;
                        IL_0fdc: // <========== 8
                            num = 182;
                            lErl = 2;
                            num5 = (short)unchecked(num5 + 1);
                            if (num5 <= 509)
                            {
                                goto IL_0044;
                            }
                            goto IL_0ffd;
                        IL_0ffd: // <========== 3
                            num = 184;
                            lErl = 3;
                            DataModul.DSB_FamStatTable.Index = "Nr";
                            DataModul.DSB_FamStatTable.Seek("=", FamInArb);
                            if (DataModul.DSB_FamStatTable.NoMatch)
                            {
                                DataModul.DSB_FamStatTable.AddNew();
                                DataModul.DSB_FamStatTable.Fields["Fam"].Value = FamInArb;
                                DataModul.DSB_FamStatTable.Fields["Heir"].Value = 0;
                                DataModul.DSB_FamStatTable.Fields["Mann"].Value = 0;
                                DataModul.DSB_FamStatTable.Fields["Frau"].Value = 0;
                                DataModul.DSB_FamStatTable.Fields["Kind"].Value = 0;
                                DataModul.DSB_FamStatTable.Fields["Vor"].Value = 0;
                                DataModul.DSB_FamStatTable.Fields["Sich"].Value = 0;
                                DataModul.DSB_FamStatTable.Update();
                                goto IL_0ffd;
                            }
                            if (DataModul.DSB_FamStatTable.Fields["Heir"].AsInt() == 0)
                            {
                                DataModul.DSB_FamStatTable.Edit();
                                DataModul.DSB_FamStatTable.Fields["Heir"].Value = Kont[30].AsDouble();
                                DataModul.DSB_FamStatTable.Update();
                            }
                            goto end_IL_0000_2;
                        IL_12e9:
                            num4 = unchecked(num2 + 1);
                            goto IL_12ed;
                        IL_12ed:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 8:
                                    goto IL_0044;
                                case 23:
                                case 26:
                                case 27:
                                    goto IL_018d;
                                case 53:
                                case 54:
                                case 55:
                                    goto IL_03e9;
                                case 70:
                                case 71:
                                    goto IL_0556;
                                case 78:
                                case 79:
                                    goto IL_0604;
                                case 81:
                                case 82:
                                case 83:
                                    goto IL_0628;
                                case 93:
                                case 94:
                                case 95:
                                case 96:
                                    goto IL_069a;
                                case 105:
                                case 108:
                                case 109:
                                case 110:
                                    goto IL_07ae;
                                case 114:
                                case 115:
                                case 116:
                                    goto IL_0853;
                                case 121:
                                case 122:
                                    goto IL_08b4;
                                case 123:
                                case 124:
                                    goto IL_08eb;
                                case 146:
                                case 147:
                                    goto IL_0aba;
                                case 150:
                                case 151:
                                    goto IL_0b78;
                                case 153:
                                case 154:
                                    goto IL_0c1f;
                                case 157:
                                case 158:
                                case 159:
                                case 160:
                                    goto IL_0c90;
                                case 164:
                                case 165:
                                case 166:
                                    goto IL_0d10;
                                case 19:
                                case 28:
                                case 38:
                                case 47:
                                case 97:
                                case 168:
                                case 181:
                                case 182:
                                    goto IL_0fdc;
                                case 184:
                                case 197:
                                    goto IL_0ffd;
                                case 35:
                                case 72:
                                case 80:
                                case 87:
                                case 92:
                                case 198:
                                case 204:
                                case 205:
                                case 206:
                                case 208:
                                case 212:
                                case 213:
                                case 219:
                                case 220:
                                case 221:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 5743;
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
    public string Text_Retweg(string UbgT1)
    {
        UbgT1 = UbgT1.Trim();
        UbgT1 = UbgT1.Replace("\r", " ");
        UbgT1 = UbgT1.Replace("\n", " ");
        UbgT1 = UbgT1.Replace("  ", " ");
        return UbgT1;
    }

    public void Ahnles(int PersInArb, out string[] asAhnData)
    {
        asAhnData = new string[31];
        asAhnData.Initialize();
        DataModul.DT_AncesterTable.Index = "PerNr";
        asAhnData[20] = "";
        DataModul.DT_AncesterTable.Seek("=", PersInArb);
        if (!DataModul.DT_AncesterTable.NoMatch && DataModul.DT_AncesterTable.Fields["Ahn"].AsInt() > 0)
        {
            if (DataModul.DT_AncesterTable.Fields["Weiter"].AsInt() != 0)
            {
                asAhnData[20] = ">>";
            }
            asAhnData[11] = DataModul.DT_AncesterTable.Fields["Ahn"].AsString();
            asAhnData[10] = IText[123] + " " + DataModul.DT_AncesterTable.Fields["Gen"].AsString() + " " + IText[124] + " " + asAhnData[11].Trim();
        }
        DataModul.DT_DescendentTable.Index = "PerNr";
        DataModul.DT_DescendentTable.Seek("=", PersInArb);
        if (!DataModul.DT_DescendentTable.NoMatch)
        {
            if (DAus[113].AsDouble() == 0.0)
            {
                asAhnData[13] = (DataModul.DT_DescendentTable.Fields["Gen"].AsString() + "-" + DataModul.DT_DescendentTable.Fields["Nr"].Value + '\n').AsString();
            }
            else
            {
                asAhnData[13] = DataModul.DT_DescendentTable.Fields["Gen"].AsString() + "-" + DataModul.DT_DescendentTable.Fields["LFNr"].AsString().Trim() + " ";
            }
        }
    }

    public void Erei(int PersInArb, EEventArt eArt, ref byte PerPos)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string text2 = default;
        short num5 = default;
        short num6 = default;
        string Job = default;
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
                    byte Schalt;
                    short LfNR;
                    int AAA;
                    string LD;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 7062:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_17a8;
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
                                    goto IL_17a8;
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
                                goto IL_17ac;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            MyProject.Forms.Hinter.List3.Items.Clear();
                            var UbgT = "";
                            DataModul.DB_EventTable.Index = "Besu";
                            DataModul.DB_EventTable.Seek("=", eArt, PersInArb);
                            while (!DataModul.DB_EventTable.EOF
                                && !DataModul.DB_EventTable.NoMatch
                                && !Conversions.ToBoolean(DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != eArt
                                   | DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != PersInArb))
                            {
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value)
                                    && DataModul.DB_EventTable.Fields[EventFields.ArtText].AsDouble() > 0.0)
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                                    LD = "";
                                    DataModul.Textlese(AAA, out string sArtText, out LD);
                                    if (sArtText != "")
                                    {
                                        UbgT = " " + sArtText.Trim() + ": ";
                                    }

                                }
                                string text = Strings.Left(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 4);
                                MyProject.Forms.Hinter.List3.Items.Add(UbgT + new string(' ', 240).Left(240) + text + "     " + DataModul.DB_EventTable.Fields[EventFields.LfNr].AsString());
                                DataModul.DB_EventTable.MoveNext();
                            }
                            goto IL_02d1;
                        IL_02d1: // <========== 3
                            num = 26;
                            if (MyProject.Forms.Hinter.List3.Items.Count == 0)
                            {
                                goto end_IL_0000_2;
                            }
                            text2 = "";
                            switch (eArt)
                            {
                                case EEventArt.eA_300:
                                    if (MyProject.Forms.Hinter.List3.Items.Count == 1)
                                    {
                                        text2 = " Beruf: ";
                                    }
                                    if (MyProject.Forms.Hinter.List3.Items.Count > 1)
                                    {
                                        text2 = " Berufe: ";
                                    }
                                    break;
                                case EEventArt.eA_301:
                                    if (MyProject.Forms.Hinter.List3.Items.Count == 1)
                                    {
                                        text2 = " " + IText[70] + " ";
                                    }
                                    if (MyProject.Forms.Hinter.List3.Items.Count > 1)
                                    {
                                        text2 = " " + IText[70] + " ";
                                    }
                                    break;
                                case EEventArt.eA_302:
                                    if (MyProject.Forms.Hinter.List3.Items.Count == 1)
                                    {
                                        text2 = " " + IText[8];
                                    }
                                    if (MyProject.Forms.Hinter.List3.Items.Count > 1)
                                    {
                                        text2 = " Wohnorte: ";
                                    }
                                    break;
                                default:
                                    break;
                            }
                            if (text2 != "")
                            {
                                if (MyProject.Forms.Ausw.Option3[0].Checked)
                                {
                                    MyProject.Forms.Hinter.Anz.Font = MyProject.Forms.Hinter.Anz.SelectionFont.ChangeFBold(Bold: true);
                                    MyProject.Forms.Hinter.Anz.Font = MyProject.Forms.Hinter.Anz.SelectionFont.ChangeFBold(Bold: true);
                                    MyProject.Forms.Hinter.Anz.SelectedText = text2.Trim();
                                }
                                else
                                {
                                    MyProject.Forms.Hinter.Anz.Font = MyProject.Forms.Hinter.Anz.SelectionFont.ChangeFBold(Bold: true);
                                    MyProject.Forms.Hinter.Anz.SelectedText = text2.Trim();
                                }
                            }
                            goto IL_0569;
                        IL_0569: // <========== 3
                            num = 68;
                            text2 = "";
                            MyProject.Forms.Hinter.Anz.Font = MyProject.Forms.Hinter.Anz.SelectionFont.ChangeFBold(Bold: false);
                            if (MyProject.Forms.Hinter.List3.Items.Count <= 0)
                            {
                                goto end_IL_0000_2;
                            }
                            num5 = (short)(MyProject.Forms.Hinter.List3.Items.Count - 1);
                            num6 = 0;
                            goto IL_1719;
                        IL_1719:
                            while (num6 <= num5)
                            {
                                UbgT = MyProject.Forms.Hinter.List3.Items[num6].AsString().Right(4);
                                _Modul1.Instance.LfNR = (byte)Math.Round(UbgT.AsDouble());
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", eArt, PersInArb, _Modul1.Instance.LfNR);
                                var J = 0;
                                while (unchecked(J) <= 15u)
                                {
                                    Kont1[J] = "";
                                    J++;
                                }
                                sDatu = "";
                                if (!(DataModul.DB_EventTable.NoMatch
                                    | DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != PersInArb
                                    | DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != eArt))
                                {
                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt())
                                        && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt()) > 0.0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt().AsString().AsInt();
                                        LD = "";
                                        DataModul.Textlese(AAA, out string sArtText, out LD);
                                        if (sArtText != "")
                                        {
                                            text2 = " " + sArtText.Trim() + ": ";
                                        }

                                    }
                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt()) && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt()) > 0.0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt().AsString().AsInt();
                                        LD = "";
                                        DataModul.Textlese(AAA, out string sArtText, out LD);
                                        if (sArtText != "")
                                        {
                                            Kont1[4] = " " + sArtText.Trim() + ": ";
                                        }

                                    }
                                    string text3;
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                    {
                                        sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                                        text3 = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                        if (text3.Trim() == "" & DataModul.DB_EventTable.Fields[EventFields.DatumB].AsDate() != default)
                                        {
                                            text3 = "von";
                                        }
                                        sDatu = Datwand1(sDatu, text3);
                                        Kont1[1] = sDatu;
                                    }
                                    if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                                    {
                                        sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString().Trim(), 8);
                                        text3 = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                        sDatu = Datwand1(sDatu, text3);
                                        if (sDatu != "" & text3.Trim() == "")
                                        {
                                            sDatu = " bis " + sDatu;
                                        }
                                        Kont1[3] = sDatu;
                                    }
                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt()) && DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt() > 0)
                                    {
                                        string value = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsString();
                                        AAA = value.AsInt();
                                        LD = "";
                                        DataModul.Textlese(AAA, out string sDatumText, out LD);
                                        value = AAA.AsString();
                                        if (sDatumText.Trim() != "")
                                        {
                                            Kont1[1] = Kont1[1] + " (" + sDatumText.Trim() + ")";
                                            sDatumText = "";
                                        }
                                    }
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt()) > 0.0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt().AsString().AsInt();
                                        LD = "";
                                        var sKBem = DataModul.TextLese1(AAA);
                                        if (sKBem != "")
                                        {
                                            Kont1[7] = " " + sKBem.Trim();
                                        }
                                    }
                                    if (eArt == EEventArt.eA_302 && !Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Hausnr].AsInt()) && Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Hausnr].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.Hausnr].AsInt().AsString().AsInt();
                                        LD = "";
                                        var sHausnr = DataModul.TextLese1(AAA);
                                        Kont1[7] = Kont1[7] + " " + sHausnr.Trim() + " ";
                                        sHausnr = "";
                                    }
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.Ort].AsInt()) > 0.0)
                                    {
                                        ortNr = (int)Math.Round(Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.Ort].AsInt()));
                                        UbgT = ortles(ortNr, 1);
                                        Kont1[5] = " " + UbgT.Trim();
                                        UbgT = "";
                                    }
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt()) > 0.0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt().AsString().AsInt();
                                        //bBB5 = ref asPersDates[0];
                                        LD = "";
                                        DataModul.Textlese(AAA, out string sPlatz, out LD);
                                        if (sPlatz != "")
                                        {
                                            Kont1[6] = " " + sPlatz.Trim();
                                        }
                                    }
                                    Job = " " + text2.Trim();
                                    Job = Job.Trim() + " " + Kont1[1].Trim();
                                    Job = Job.Trim() + " " + Kont1[3].Trim();
                                    Job = Job.Trim() + " " + Kont1[7].Trim();
                                    Job = Job.Trim() + " " + Kont1[8].Trim();
                                    Job = Job.Trim() + " " + Kont1[5].Trim();
                                    Job = Job.Trim() + " " + Kont1[6].Trim();
                                    Job = " " + Job.TrimEnd();
                                    Job = "";
                                    Job = Module2.Jobdreh(Job);
                                    Kont1[2] = "";
                                    Kont1[4] = "";
                                    Kont1[6] = "";
                                    if (Aus[2] == "Y" && Operators.ConditionalCompareObjectNotEqual(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString(), " ", TextCompare: false))
                                    {
                                        Kont1[2] = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim();
                                    }
                                    if (Aus[3] == "Y" && Operators.ConditionalCompareObjectNotEqual(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString(), " ", TextCompare: false))
                                    {
                                        Kont1[4] = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim();
                                    }
                                    if (Kont1[2].Trim() != "" | Kont1[4].Trim() != "")
                                    {
                                        Kont1[6] = Kont1[2].Trim() + " " + Kont1[4].Trim();
                                        Kont1[6] = " {" + Kont1[6].Trim() + "} ";
                                    }
                                    Job += Kont1[6];
                                    if (MyProject.Forms.Ausw.Kontroll[5].Checked & PerPos == 1
                                        | MyProject.Forms.Ausw.Kontroll[7].Checked & PerPos == 2
                                        | MyProject.Forms.Ausw.Kontroll[8].Checked & PerPos == 3)
                                    {
                                        LfNR = _Modul1.Instance.LfNR;
                                        SLQuellenDatum(ref PersInArb, eArt, ref LfNR);
                                        _Modul1.Instance.LfNR = (byte)LfNR;
                                        Job += ". ";
                                        if (Kont1[9].Trim() != "")
                                        {
                                            string sSource = "Quelle: " + Kont1[9].Trim();
                                            if (MyProject.Forms.Ausw.Option3[0].Checked)
                                            {
                                                if (sSource != "")
                                                {
                                                    Job = Job + " " + sSource;
                                                }
                                            }
                                            else
                                            if (sSource != "")
                                            {
                                                Job = Job + " " + sSource;
                                            }

                                        }
                                    }
                                    PersSp = PersInArb;
                                    byte b = 1;
                                    while (unchecked(b) <= 100u)
                                    {
                                        KontSP1[b] = Kont1[b];
                                        KontSP[b] = Kont[b];
                                        Kont[b] = "";
                                        Kont1[b] = "";
                                        b = (byte)unchecked((uint)(b + 1));
                                    }
                                    LfNR = 0;
                                    LD = "";
                                    Zeugsu(eArt, _Modul1.Instance.LfNR, LfNR, 0L);
                                    PersInArb = PersSp;
                                    if (Kont1[20] != "")
                                    {
                                        Job = Job + "Zeugen: " + Kont1[20];
                                    }
                                    b = 1;
                                    while (unchecked(b) <= 100u)
                                    {
                                        Kont1[b] = KontSP1[b];
                                        Kont[b] = KontSP[b];
                                        KontSP[b] = "";
                                        KontSP1[b] = "";
                                        b = (byte)unchecked((uint)(b + 1));
                                    }
                                    if (Job.Trim() != "")
                                    {
                                        if (Job.Trim().Right(1) != ".")
                                        {
                                            Job = Job.Trim() + ".";
                                        }
                                        if (MyProject.Forms.Hinter.List3.Items.Count > 1 & num6 < MyProject.Forms.Hinter.List3.Items.Count - 1 && eArt > EEventArt.eA_105)
                                        {
                                            Job = Job.Left(Job.Length - 1) + ";";

                                        }
                                    }
                                    if (MyProject.Forms.Ausw.Kontroll[12].CheckState == CheckState.Unchecked)
                                    {
                                        Job = Text_Retweg(Job);
                                    }
                                    MyProject.Forms.Hinter.Anz.SelectedText = " " + Job.Trim();
                                    DataModul.DB_EventTable.MoveNext();
                                    num6++;
                                }
                                else
                                {
                                    DataModul.DB_EventTable.Index = "ArtNr";
                                    break;
                                }
                            }
                            goto end_IL_0000_2;
                        IL_17a8:
                            num4 = unchecked(num2 + 1);
                            goto IL_17ac;
                        IL_17ac:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 9:
                                case 12:
                                case 26:
                                    goto IL_02d1;
                                case 62:
                                case 66:
                                case 67:
                                case 68:
                                    goto IL_0569;
                                case 27:
                                case 85:
                                case 237:
                                case 238:
                                case 247:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 7062;
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
    public void Famdatles2()
    {
        int try0000_dispatch = -1;
        int num = default;
        string ind = default;
        int num2 = default;
        int num3 = default;
        int number = default;
        short num5 = default;
        object CounterResult = default;
        object LoopForResult = default;
        string ds = default;
        string ds2 = default;
        string value = default;
        string UbgT = default;
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
                    int ortNr;
                    byte Schalt;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            ind = "";
                            goto IL_0009;
                        case 3990:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0d10;
                                    default:
                                        goto end_IL_0000;
                                }
                                number = Information.Err().Number;
                                if (number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0d10;
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
                                goto IL_0d14;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            num5 = 0;
                            while (num5 <= 30)
                            {
                                Kont[num5] = "";
                                num5 = (short)unchecked(num5 + 1);
                            }
                            num5 = 500;
                            goto IL_003b;
                        IL_003b: // <========== 3
                            num = 7;
                            Ubg = num5;
                            Art = (EEventArt)Ubg;
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", Ubg.AsString(), FamInArb.AsString(), "0");
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields["priv"].AsInt()))
                                {
                                    iPrivacy = DataModul.DB_EventTable.Fields["priv"].AsInt();

                                }
                                else
                                {
                                    iPrivacy = 0;
                                }
                                goto IL_0147;
                            }
                            goto IL_0a21;
                        IL_0147: // <========== 3
                            num = 20;
                            if (!(iPrivacy.AsDouble() > iPriv_aus.AsDouble()))
                            {
                                if (Datschalt == 2)
                                {
                                    if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, MyProject.Forms.Hinter.List1.Items.Count - 1, 1, ref LoopForResult, ref CounterResult))
                                    {
                                        while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult, ref CounterResult))
                                        {
                                            Kol = (int)Math.Round(Conversion.Val(Strings.Right(MyProject.Forms.Hinter.List1.Items[CounterResult.AsInt()].AsString(), 6)));
                                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() == Kol)
                                            {
                                                Datschalt = 3;
                                                goto end_IL_0000_2;
                                            }
                                        }
                                    }
                                    goto IL_0a21;
                                }
                                var J = 0;
                                while (unchecked(J) <= 15u)
                                {
                                    Kont1[J] = "";
                                    J = (byte)unchecked((uint)(J + 1));
                                }
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    if (_Modul1.Instance.Schalt == 1)
                                    {
                                        Kont[Ubg - 500] = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                                        goto IL_0a21;
                                    }
                                    sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                                    ind = Conversion.Str(sDatu.Left(2).AsDouble() + 1.0);
                                    if (Ubg == 502)
                                    {
                                        Kont[30] = sDatu.Left(4);
                                    }
                                    if (Ubg == 503 & Kont[30] == "")
                                    {
                                        Kont[30] = sDatu.Left(4);
                                    }
                                    ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                    sDatu = Datwand1(sDatu, ds);
                                    Kont1[1] = sDatu;
                                }
                                goto IL_0417;
                            }
                            goto IL_0a21;
                        IL_0417:
                            num = 53;
                            if (_Modul1.Instance.Schalt != 1)
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                                {
                                    sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString().Trim(), 8);
                                    ds2 = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                    sDatu = Datwand1(sDatu, ds2);
                                    if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString().Trim(), "", TextCompare: false) == 0)
                                    {
                                        Kont1[3] = " / " + sDatu;
                                        goto IL_052b;
                                    }
                                    Kont1[3] = sDatu;
                                }
                                goto IL_052b;
                            }
                            goto IL_0a21;
                        IL_052b: // <========== 3
                            num = 67;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.DatumText].Value))
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt() > 0)
                                {
                                    value = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsString();
                                    AAA = value.AsInt();
                                    _Modul1.Instance.UbgT = DataModul.TextLese1(AAA);
                                    value = AAA.AsString();
                                    if (_Modul1.Instance.UbgT.Trim() != "")
                                    {
                                        Kont1[3] = Kont1[3] + " (" + _Modul1.Instance.UbgT.Trim() + ")";
                                        _Modul1.Instance.UbgT = "";
                                    }
                                }
                            }
                            goto IL_0626;
                        IL_0626: // <========== 3
                            num = 77;
                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                Kont[0] = DataModul.TextLese1(AAA);
                                if (Kont[0] != "")
                                {
                                    Kont1[7] = " " + Kont[0].Trim();
                                    Kont[0] = "";
                                }
                            }
                            goto IL_06ee;
                        IL_06ee: // <========== 3
                            num = 84;
                            _Modul1.Instance.UbgT = "";
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                            {
                                if (_Modul1.Instance.Schalt == 10)
                                {
                                    Ind1 = ind;
                                }
                                Kont1[5] = ortles1(DataModul.DB_EventTable.Fields[EventFields.Ort].AsInt(), 1, (i, s) => ExportPlace(i, s, Ind1, Namen));

                                _Modul1.Instance.UbgT = "";
                            }
                            goto IL_079a;
                        IL_079a:
                            num = 93;
                            if (Aus[5] == "Y" | Aus[6] == "Y")
                            {
                                if ((DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString() != " " & Aus[5] == "Y").AsBool())
                                {
                                    Kont[Ubg - 480] = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString();
                                }
                                goto IL_084f;
                            }
                            goto IL_095c;
                        IL_084f:
                            num = 97;
                            if ((DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString() != " " & Aus[6] == "Y").AsBool())
                            {
                                Kont[Ubg - 480] = Kont[Ubg - 480] + " " + DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString();
                            }
                            goto IL_08f0;
                        IL_08f0:
                            num = 100;
                            if (Kont[Ubg - 480].Trim() != "")
                            {
                                UbgT = Kont[Ubg - 480];
                                UbgT = Text_Retweg(UbgT);
                                Kont[Ubg - 480] = UbgT;
                                UbgT = "";
                            }
                            goto IL_095c;
                        IL_095c: // <========== 3
                            num = 107;
                            if (_Modul1.Instance.Schalt == 10)
                            {
                                if (Kont1[1].Trim().Length > 0)
                                {
                                    if (Kont1[1].Trim().Length < 8 | Strings.Asc(Kont1[1]) > 57)
                                    {
                                        Kont1[1] = " (" + Kont1[1] + ")";
                                    }
                                }
                                goto IL_09d0;
                            }
                            Kont[Ubg - 500] = Module2.Jobdreh(Kont[Ubg - 500]);
                            goto IL_0a21;
                        IL_09d0: // <========== 3
                            num = 113;
                            Kont[Ubg - 500] = Kont1[5] + " " + Kont1[1];
                            goto IL_0a21;
                        IL_0a21: // <========== 8
                            num = 118;
                            lErl = 2;
                            num5 = (short)unchecked(num5 + 1);
                            if (num5 <= 507)
                            {
                                goto IL_003b;
                            }
                            goto IL_0a3c;
                        IL_0a3c: // <========== 3
                            num = 120;
                            lErl = 3;
                            DataModul.DSB_FamStatTable.Index = "Nr";
                            DataModul.DSB_FamStatTable.Seek("=", FamInArb);
                            if (DataModul.DSB_FamStatTable.NoMatch)
                            {
                                DataModul.DSB_FamStatTable.AddNew();
                                DataModul.DSB_FamStatTable.Fields["Fam"].Value = FamInArb;
                                DataModul.DSB_FamStatTable.Fields["Heir"].Value = 0;
                                DataModul.DSB_FamStatTable.Fields["Mann"].Value = 0;
                                DataModul.DSB_FamStatTable.Fields["Frau"].Value = 0;
                                DataModul.DSB_FamStatTable.Fields["Kind"].Value = 0;
                                DataModul.DSB_FamStatTable.Fields["Vor"].Value = 0;
                                DataModul.DSB_FamStatTable.Fields["Sich"].Value = 0;
                                DataModul.DSB_FamStatTable.Update();
                                goto IL_0a3c;
                            }
                            if (!(DataModul.DSB_FamStatTable.Fields["Heir"].AsInt() == 0))
                            {

                            }
                            else
                            {
                                DataModul.DSB_FamStatTable.Edit();
                                DataModul.DSB_FamStatTable.Fields["Heir"].Value = Kont[30].AsDouble();
                                DataModul.DSB_FamStatTable.Update();
                            }
                            goto end_IL_0000_2;
                        IL_0d10:
                            num4 = unchecked(num2 + 1);
                            goto IL_0d14;
                        IL_0d14:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 7:
                                    goto IL_003b;
                                case 16:
                                case 19:
                                case 20:
                                    goto IL_0147;
                                case 52:
                                case 53:
                                    goto IL_0417;
                                case 62:
                                case 65:
                                case 66:
                                case 67:
                                    goto IL_052b;
                                case 74:
                                case 75:
                                case 76:
                                case 77:
                                    goto IL_0626;
                                case 82:
                                case 83:
                                case 84:
                                    goto IL_06ee;
                                case 92:
                                case 93:
                                    goto IL_079a;
                                case 96:
                                case 97:
                                    goto IL_084f;
                                case 99:
                                case 100:
                                    goto IL_08f0;
                                case 105:
                                case 106:
                                case 107:
                                    goto IL_095c;
                                case 111:
                                case 112:
                                case 113:
                                    goto IL_09d0;
                                case 12:
                                case 21:
                                case 31:
                                case 39:
                                case 54:
                                case 114:
                                case 117:
                                case 118:
                                    goto IL_0a21;
                                case 120:
                                case 133:
                                    goto IL_0a3c;
                                case 28:
                                case 134:
                                case 140:
                                case 141:
                                case 142:
                                case 144:
                                case 148:
                                case 149:
                                case 155:
                                case 156:
                                case 157:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 3990;
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
    public void Datles2()
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int number = default;
        int lErl = default;
        short num5 = default;
        int num9 = default;
        string ds = default;
        string ds2 = default;
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
                    int AAA;
                    switch (try0000_dispatch)
                    {
                        default:
                            {
                                num = 1;
                                Datum1 = "";
                                Datum2 = "";
                                ProjectData.ClearProjectError();
                                num3 = 2;
                                num5 = 1;
                                while (num5 <= 40)
                                {
                                    Kont[num5] = "";
                                    num5 = (short)unchecked(num5 + 1);
                                }
                                num5 = 101;
                                goto IL_0064;
                            }
                        case 3406:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0b34;
                                    default:
                                        goto end_IL_0000;
                                }
                                number = Information.Err().Number;
                                if (number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0b34;
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
                                goto IL_0b38;
                            }
                        end_IL_0000:
                            break;
                        IL_0064: // <========== 3
                                 // <========== 3
                            num = 11;
                            var J = 0;
                            while (J <= 15u)
                            {
                                Kont1[J++] = "";
                            }
                            Ubg = num5;
                            Art = (EEventArt)Ubg;
                            IRecordset dB_EventTable = DataModul.DB_EventTable;
                            dB_EventTable.Index = "ArtNr";
                            dB_EventTable.Seek("=", Ubg.AsString(), PersInArb, 0);
                            if (!dB_EventTable.NoMatch)
                            {
                                if (Datschalt == 2)
                                {
                                    num9 = 0;
                                    while (num9 <= MyProject.Forms.Hinter.List1.Items.Count - 1)
                                    {
                                        Kol = (int)Math.Round(Conversion.Val(MyProject.Forms.Hinter.List1.Items[num9].AsString().Right(6)));
                                        if (dB_EventTable.Fields["Ort"].AsDouble() == Kol)
                                        {
                                            Datschalt = 3;
                                            goto end_IL_0000_2;
                                        }
                                        num9++;
                                    }
                                    goto IL_0a72;
                                }
                                if (Conversion.Val(dB_EventTable.Fields["DatumV"].AsDate()) > 0.0)
                                {
                                    sDatu = Strings.Right("00000000" + dB_EventTable.Fields["DatumV"].AsString().Trim(), 8);
                                    if (Datschalt == 1)
                                    {
                                        Kont[Ubg - 100] = Strings.Right("00000000" + dB_EventTable.Fields["DatumV"].AsString().Trim(), 8);
                                        goto IL_0a72;
                                    }
                                    if (Ubg == 101)
                                    {
                                        Kont[26] = sDatu.Left(4);
                                    }
                                    if (Ubg == 102 & Kont[26] == "")
                                    {
                                        Kont[26] = sDatu.Left(4);
                                    }
                                    if (Ubg == 103)
                                    {
                                        Kont[27] = sDatu.Left(4);
                                    }
                                    if (Ubg == 104 & Kont[27] == "")
                                    {
                                        Kont[27] = sDatu.Left(4);
                                    }
                                    ds = dB_EventTable.Fields["Datumv_S"].AsString();
                                    sDatu = Datwand1(sDatu, ds);
                                    Kont1[1] = sDatu;
                                }
                                if (dB_EventTable.Fields["DatumB"].AsInt() > 0)
                                {
                                    sDatu = Strings.Right("00000000" + dB_EventTable.Fields["DatumB"].AsString().Trim(), 8);
                                    ds2 = dB_EventTable.Fields["Datumb_S"].AsString();
                                    sDatu = Datwand1(sDatu, ds2);
                                    if (Operators.CompareString(dB_EventTable.Fields["DatumB_S"].AsString().Trim(), "", TextCompare: false) == 0)
                                    {
                                        Kont1[3] = " / " + sDatu;
                                    }
                                    else Kont1[3] = " " + sDatu;
                                }
                                UbgT = "";
                                if (dB_EventTable.Fields["Ort"].AsDouble() > 0.0)
                                {
                                    Schalt = 1;
                                    UbgT = ortles(dB_EventTable.Fields["Ort"].AsInt(), 1);
                                }
                                if (dB_EventTable.Fields["Kbem"].AsDouble() > 0.0)
                                {
                                    AAA = dB_EventTable.Fields["Kbem"].AsInt();
                                    Kont[0] = DataModul.TextLese1(AAA);
                                    if (Kont[0] != "")
                                    {
                                        Kont1[7] = " " + Kont[0].Trim() + " ";
                                    }
                                }
                                if (dB_EventTable.Fields["Platz"].AsInt() != 0)
                                {
                                    AAA = dB_EventTable.Fields["Platz"].AsInt();
                                    Kont[0] = DataModul.TextLese1(AAA);
                                    if (Kont[0] != "")
                                    {
                                        Kont1[6] = " " + Kont[0].Trim() + " ";
                                    }
                                }
                                if (dB_EventTable.Fields["Bem1"].AsString().TrimEnd() != "" | dB_EventTable.Fields["Bem2"].AsString().TrimEnd() != "")
                                {
                                    Kont[Ubg - 90] = (Kont1[1] + " " + Kont1[2] + Kont1[3] + Kont1[4] + Kont1[5] + Kont1[7]).Trim() + " " + UbgT + Kont1[6];
                                    UbgT = "";
                                    if (Aus[2] == "Y" | Aus[3] == "Y")
                                    {
                                        Kont[Ubg - 85] = " {";
                                    }
                                    if (Aus[2] == "Y")
                                    {
                                        Kont[Ubg - 85] = Kont[Ubg - 85] + dB_EventTable.Fields["Bem1"].AsString().TrimEnd();
                                    }
                                    if (Aus[3] == "Y")
                                    {
                                        Kont[Ubg - 85] = Kont[Ubg - 85] + " " + dB_EventTable.Fields["Bem2"].AsString().TrimEnd();
                                    }
                                    if (Aus[2] == "Y" | Aus[3] == "Y")
                                    {
                                        Kont[Ubg - 85] = Kont[Ubg - 85].Trim() + "}";
                                    }
                                    text2 = Kont[Ubg - 85];
                                    UbgT1 = text2;
                                    _Modul1.Instance.UbgT1 = Text_Retweg(_Modul1.Instance.UbgT1);
                                    text2 = UbgT1;
                                    UbgT1 = "";
                                    Kont[Ubg - 85] = text2;
                                    text2 = "";
                                }
                                else Kont[Ubg - 90] = Strings.Trim((Kont1[1] + " " + Kont1[2]).Trim() + Kont1[3] + Kont1[4] + Kont1[5] + Kont1[7]) + " " + UbgT + Kont1[6];
                                Kont[Ubg - 80] = Kont1[1];
                                Kont[Ubg - 70] = UbgT;
                                UbgT = "";
                            }
                            goto IL_0a72;
                        IL_0a72: // <========== 4
                                 // <========== 5
                            num = 110;
                            lErl = 2;
                            num5 = (short)unchecked(num5 + 1);
                            if (num5 <= 107)
                            {
                                goto IL_0064;
                            }
                            Kont[25] = RechWithProps();
                            goto end_IL_0000_2;
                        IL_0b11:
                            num = 127;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_0b34;
                        IL_0b34: // <========== 3
                                 // <========== 3
                            num4 = unchecked(num2 + 1);
                            goto IL_0b38;
                        IL_0b38:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 11:
                                    goto IL_0064;
                                case 19:
                                case 29:
                                case 35:
                                case 110:
                                    goto IL_0a72;
                                case 126:
                                case 127:
                                    goto IL_0b11;
                                case 26:
                                case 113:
                                case 115:
                                case 119:
                                case 120:
                                case 128:
                                case 129:
                                case 130:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 3406;
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
    public void RegPaten()
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
                int num4;
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0008;
                    case 2089:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_06df;
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
                                goto IL_06df;
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
                            goto IL_06e3;
                        }
                    end_IL_0000:
                        break;
                    IL_0008:
                        num = 2;
                        int persInArb = PersInArb;
                        foreach (var cLink in DataModul.Link.ReadAllFams(PersInArb, ELinkKennz.lkGodparent))
                        {
                            PersInArb = cLink.iPersNr;
                            Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            _Modul1.Instance.Person.SetFullSurname(
                                Person_FullSurname(_Modul1.Instance.Person, false));
                            MyProject.Forms.Anzeige.RichTextBox1[0].SelectionFont =
                                MyProject.Forms.Anzeige.RichTextBox1[0].SelectionFont.ChangeFUnderline(Underline: false);
                            if (b == 0)
                            {
                                if (Strings.Mid(MyProject.Forms.Anzeige.RichTextBox1[0].Text, MyProject.Forms.Anzeige.RichTextBox1[0].SelectionStart, 1) != "\n")
                                {
                                    MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = "\n";
                                }
                                MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = "Paten: " + Strings.Trim(_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames.Trim() + " " + _Modul1.Instance.Person.FullSurName.Trim());
                                b = 1;
                            }
                            else
                                MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = "; " + Strings.Trim(_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames.Trim() + " " + _Modul1.Instance.Person.FullSurName.Trim());
                            Datles(_Modul1.Instance.PersInArb, out var asPersDates);
                            if (asPersDates[11] == "")
                            {
                                asPersDates[11] = asPersDates[12];
                            }
                            if (asPersDates[11].Trim() != "")
                            {
                                MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = " * " + asPersDates[11].Trim() + ".";
                            }
                            if (asPersDates[13] == "")
                            {
                                asPersDates[13] = asPersDates[14];
                            }
                            if (asPersDates[14].Trim() != "")
                            {
                                MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = " + " + asPersDates[13].Trim() + ".";
                            }
                            PersInArb = persInArb;
                        }
                        if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields[PersonFields.Bem2].Value)
                            && Strings.Len(DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString().Trim()) > 1)
                        {
                            if (b == 0)
                            {
                                if (Strings.Mid(MyProject.Forms.Anzeige.RichTextBox1[0].Text, MyProject.Forms.Anzeige.RichTextBox1[0].SelectionStart, 1) != "\n")
                                {
                                    MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = "\n";
                                }
                                MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = "Paten:";
                                b = 1;
                            }
                            else
                                MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = ";";
                            MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = " " + DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString();
                        }
                        if (b > 0)
                        {
                            MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = ".";
                        }
                        PersInArb = persInArb;
                        goto end_IL_0000_2;
                    IL_06df:
                        num4 = num2 + 1;
                        goto IL_06e3;
                    IL_06e3:
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
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 2089;
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
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        List<(int, ELinkKennz)> text2 = new List<(int, ELinkKennz)>();
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    string ds;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            ds = "";
                            goto IL_0009;
                        case 5632:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_12c6;
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
                                    goto IL_12c6;
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
                                goto IL_12ca;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            MyProject.Forms.Hinter.List3.Items.Clear();
                            foreach (var cLink in DataModul.Link.ReadAllPers(persInArb, ELinkKennz.lkGodparent))
                            {
                                persInArb = cLink.iFamNr;
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", 102.AsString(), persInArb, 0);
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    DataModul.DB_EventTable.Seek("=", 101.AsString(), persInArb, 0);
                                }
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    sDatu = "          ";
                                }
                                else if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                }
                                else
                                {
                                    sDatu = "          ";
                                }
                                MyProject.Forms.Hinter.List3.Items.Add(new ListBoxItem(sDatu, persInArb));
                            }

                            int num6;
                            if (MyProject.Forms.Hinter.List3.Items.Count > 0)
                            {
                                int num5 = MyProject.Forms.Hinter.List3.Items.Count - 1;
                                num6 = 0;
                                while (num6 <= num5)
                                {
                                    persInArb = MyProject.Forms.Hinter.List3.Items[num6].ItemData<int>();
                                    Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                    DataModul.DB_EventTable.Index = "ArtNr";
                                    DataModul.DB_EventTable.Seek("=", 102.AsString(), persInArb, 0);
                                    if (DataModul.DB_EventTable.NoMatch)
                                    {
                                        DataModul.DB_EventTable.Seek("=", 101.AsString(), persInArb, 0);
                                    }
                                    if (DataModul.DB_EventTable.NoMatch)
                                    {
                                        sDatu = "          ";
                                    }
                                    else
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                    {
                                        sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                        ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                        sDatu = Datwand1(sDatu, ds);
                                    }
                                    else
                                    {
                                        sDatu = "          ";
                                    }
                                    if (MyProject.Forms.Ausw.Option3[0].Checked)
                                    {
                                        MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = "\n";
                                    }
                                    else
                                    {
                                        MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = " ";
                                    }
                                    if (num6 == 0)
                                    {
                                        MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = "Pate: " + sDatu + " bei " + Strings.Trim(_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames.Trim() + " " + _Modul1.Instance.Person.FullSurName.Trim());
                                    }
                                    else if (MyProject.Forms.Ausw.Option3[0].Checked)
                                    {
                                        MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = "Pate: " + sDatu + " bei " + Strings.Trim(_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames.Trim() + " " + _Modul1.Instance.Person.FullSurName.Trim());
                                    }
                                    else
                                    {
                                        MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = " ; " + sDatu + " bei " + Strings.Trim(_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames.Trim() + " " + _Modul1.Instance.Person.FullSurName.Trim());
                                    }
                                    num6++;
                                }
                            }
                            text2.Clear();
                            int num7 = 5;
                            foreach (var cLink in DataModul.Link.ReadAllPers(persInArb, ELinkKennz.lkMarrWitness))
                            {
                                text2.Add((cLink.iFamNr, eLKennz));
                            }
                            foreach (var cLink in DataModul.Link.ReadAllPers(persInArb, ELinkKennz.lkWitnOfEngage))
                            {
                                text2.Add((cLink.iFamNr, eLKennz));
                            }
                            foreach (var cLink in DataModul.Link.ReadAllPers(persInArb, ELinkKennz.lkWitnOfMarr))
                            {
                                text2.Add((cLink.iFamNr, eLKennz));
                            }

                            int num8;
                            num7 = num8 = text2.Count;
                            num6 = 1;
                            while (num6 <= num8)
                            {
                                FamInArb = text2.First().Item1;
                                eLKennz = text2.First().Item2;
                                text2.RemoveAt(0);
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", 502, FamInArb.AsString(), "0");
                                Art = EEventArt.eA_Marriage;
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    sDatu = "    ";
                                }
                                else
                                {
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                    {
                                        sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                    }
                                }
                                if (sDatu.Trim() == "")
                                {
                                    DataModul.DB_EventTable.Seek("=", 503, FamInArb.AsString(), "0");
                                    if (!DataModul.DB_EventTable.NoMatch)
                                    {
                                        if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                        {
                                            sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                        }
                                    }
                                }
                                if (sDatu.Trim() != "")
                                {
                                    ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                }
                                if (sDatu.Trim() != "")
                                {
                                    sDatu = Datwand1(sDatu, ds);
                                }
                                LiText = " " + sDatu + " bei ";
                                sDatu = "";
                                Family_Les(FamInArb, Family);
                                persInArb = Family.Mann;
                                Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                string text3 = _Modul1.Instance.Person.Givennames.Trim() + " " + _Modul1.Instance.Person.FullSurName.Trim();
                                LiText += text3;
                                persInArb = Family.Frau;
                                Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                text3 = " " + _Modul1.Instance.Person.Givennames.Trim() + " " + _Modul1.Instance.Person.FullSurName.Trim();
                                if (MyProject.Forms.Ausw.Option3[0].Checked)
                                {
                                    MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = "\nTrauzeuge#alt#:";
                                    MyProject.Forms.Anzeige.RichTextBox1[0].SelectionFont = MyProject.Forms.Anzeige.RichTextBox1[0].SelectionFont.ChangeFBold(Bold: false);
                                    MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = LiText + " und " + text3 + ".";
                                }
                                else
                                {
                                    if (num6 == 1)
                                    {
                                        MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = "Trauzeuge#alt#:";
                                    }
                                    else
                                    {
                                        MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = "; ";
                                    }
                                    MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = LiText + " und " + text3;
                                    if (num7 == num6)
                                    {
                                        MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = ".";
                                    }
                                }
                                LiText = "";
                                num6++;
                            }
                            goto end_IL_0000_2;
                        IL_12c6:
                            num4 = unchecked(num2 + 1);
                            goto IL_12ca;
                        IL_12ca:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 5632;
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

    public void Datles3(short Listart, long Ahne, EEventArt Art1, ref bool neb, bool ki)
    {
        neb = false;
        int try0000_dispatch = -1;
        int num = default;
        string Datu = default;
        int num2 = default;
        int num3 = default;
        EEventArt num4 = default;
        int lErl = default;
        string Ds = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num5;
                    int ortNr;
                    int AAA;
                    int Nr;
                    short LfNR;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            Datu = "";
                            goto IL_000a;
                        case 5271:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_1119;
                                    default:
                                        goto end_IL_0000;
                                }
                                int number = Information.Err().Number;
                                if (number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_1119;
                                }
                                if (number == 3420)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_1119;
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
                                goto IL_111d;
                            }
                        end_IL_0000:
                            break;
                        IL_000a:
                            num = 2;
                            High = 0;
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            Datum1 = "";
                            Datum2 = "";
                            short num6 = 0;
                            var _num4 = 1;
                            while (_num4 <= 40)
                            {
                                Kont[_num4] = "";
                                _num4 = (short)unchecked(num4 + 1);
                            }

                            num4 = EEventArt.eA_Birth;
                            goto IL_006f;
                        IL_006f: // <========== 3
                            num = 12;
                            if (Art1 > 0
                                && num4 != Art1)
                                goto IL_101d;
                            var J = 0;
                            while (unchecked(J) <= 20u)
                            {
                                Kont1[J] = "";
                                J++;
                            }

                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", num4.AsString(), PersInArb.AsString(), "0");
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields["priv"].Value))
                                {
                                    iPrivacy = DataModul.DB_EventTable.Fields["priv"].AsInt();
                                }
                                else
                                    iPrivacy = 0;
                                if (!(iPrivacy.AsDouble() > iPriv_aus.AsDouble()))
                                {
                                    if (Datschalt == 2)
                                    {
                                        int num8 = MyProject.Forms.Hinter.List1.Items.Count - 1;
                                        int num9 = 0;
                                        while (num9 <= num8)
                                        {
                                            Kol = (int)Math.Round(Conversion.Val(MyProject.Forms.Hinter.List1.Items[num9].AsString().Right(6)));
                                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() == Kol)
                                            {
                                                Datschalt = 3;
                                                goto end_IL_0000_2;
                                            }
                                            num9++;
                                        }
                                    }
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                    {
                                        _Modul1.Instance.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                        if (num4 < EEventArt.eA_Death)
                                        {
                                            if (_Modul1.Instance.sDatu.AsDouble() > High)
                                            {
                                                High = (int)Math.Round(_Modul1.Instance.sDatu.AsDouble());
                                            }
                                        }
                                        if (Datschalt == 1)
                                        {
                                            Kont[(int)num4 - 100] = _Modul1.Instance.sDatu;
                                            Ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                            _Modul1.Instance.sDatu = Datwand2(_Modul1.Instance.sDatu, ref Ds, ref Datu);
                                            Kont1[1] = _Modul1.Instance.sDatu;
                                            goto IL_101d;
                                        }
                                        if (Datschalt == 10)
                                        {
                                            Kont[(int)num4 - 100] = DataModul.DB_EventTable.Fields[EventFields.Reg].AsString();
                                            Kont[(int)num4 - 90] = _Modul1.Instance.sDatu;
                                            goto IL_101d;
                                        }
                                        Ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                        _Modul1.Instance.sDatu = Datwand2(_Modul1.Instance.sDatu, ref Ds, ref Datu);
                                        Kont1[1] = _Modul1.Instance.sDatu;
                                        if (Operators.CompareString(_Modul1.Instance.sDatu.Left(1), "(", TextCompare: false) == 0)
                                        {
                                            Kont[(int)num4 - 90] = Kont1[1];
                                            if (DAus[110] != "1")
                                            {
                                                goto end_IL_0000_2;
                                            }
                                        }
                                    }
                                    if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                                    {
                                        _Modul1.Instance.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString().Trim(), 8);
                                        Ds = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                        _Modul1.Instance.sDatu = Datwand2(_Modul1.Instance.sDatu, ref Ds, ref Datu);
                                        if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString().Trim(), "", TextCompare: false) == 0)
                                        {
                                            Kont1[2] = " / " + _Modul1.Instance.sDatu;
                                        }
                                        else
                                            Kont1[2] = " " + _Modul1.Instance.sDatu;
                                        Kont1[1] = Kont1[1] + Kont1[2];
                                        Kont1[2] = "";
                                    }
                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.DatumText].Value))
                                    {
                                        if (DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt() > 0)
                                        {
                                            string value = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsString();
                                            AAA = value.AsInt();
                                            UbgT = DataModul.TextLese1(AAA);
                                            value = AAA.AsString();
                                            if (UbgT.Trim() != "")
                                            {
                                                Kont1[1] = Kont1[1] + " (" + UbgT.Trim() + ")";
                                                UbgT = "";
                                            }
                                        }
                                    }
                                    if (num4 == EEventArt.eA_Death)
                                    {
                                        if (DAus[108].AsDouble() == 1.0)
                                        {
                                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields["causal"].Value))
                                            {
                                                if (DataModul.DB_EventTable.Fields["causal"].AsInt() > 0)
                                                {
                                                    AAA = DataModul.DB_EventTable.Fields["causal"].AsInt();
                                                    Kont[0] = DataModul.TextLese1(AAA);
                                                    Kont1[17] = " " + Kont[0].Trim() + " ";
                                                    Kont[0] = "";
                                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields["an"].Value))
                                                    {
                                                        if (DataModul.DB_EventTable.Fields["an"].AsInt() > 0)
                                                        {
                                                            AAA = DataModul.DB_EventTable.Fields["an"].AsInt();
                                                            Kont[0] = DataModul.TextLese1(AAA);
                                                        }
                                                    }
                                                    if (Kont[0].Trim() == "")
                                                    {
                                                        Kont[0] = "an";
                                                    }
                                                    if (Kont[0].Trim() == "°")
                                                    {
                                                        Kont[0] = "";
                                                    }
                                                    Kont1[1] = Kont1[1] + " " + Kont[0].Trim() + Kont1[17] + " ";
                                                }
                                            }
                                        }
                                    }
                                    UbgT = "";
                                    if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt() != 0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                        Kont[0] = DataModul.TextLese1(AAA);
                                        if (Kont[0] != "")
                                        {
                                            Kont1[3] = " " + Kont[0].Trim();
                                        }
                                    }
                                    if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                                    {
                                        Kont1[5] = ortles1(DataModul.DB_EventTable.Fields[EventFields.Ort].AsInt(), 1, (i, s) => ExportPlace(i, s, Ind1, Namen));
                                    }
                                    if (DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt() != 0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                        Kont[0] = DataModul.TextLese1(AAA);
                                        if (Kont[0] != "")
                                        {
                                            Kont1[6] = " " + Kont[0].Trim();
                                        }
                                    }
                                    if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        UbgT = UbgT.Trim() + " ?";
                                    }
                                    if (DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().TrimEnd() != "" | DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().TrimEnd() != "")
                                    {
                                        Kont[(int)num4 - 90] = "";
                                        Kont[(int)num4 - 90] = Module2.Jobdreh(Kont[(int)num4 - 90]);
                                        string text2 = "";
                                        text2 = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().TrimEnd();
                                        Kont[(int)num4 - 85] = text2;
                                        text2 = "";
                                        text2 = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().TrimEnd();
                                        Kont[(int)num4 - 80] = text2;
                                    }
                                    else
                                    {
                                        Kont[(int)num4 - 90] = "";
                                        Kont[(int)num4 - 90] = Module2.Jobdreh(Kont[(int)num4 - 90]);
                                    }
                                    if (Datschalt == 0)
                                    {
                                        if (DAus[62] == "1")
                                        {
                                            Nr = PersInArb;
                                            LfNR = 0;
                                            QuellenDatum(ref Nr, num4, ref LfNR);
                                            PersInArb = Nr.AsInt();
                                        }
                                        if (Kont1[9].Trim() != "")
                                        {
                                            Kont[(int)num4 - 70] = " " + Kont1[9].Trim();
                                        }
                                        Kont[(int)num4 - 60] = DataModul.DB_EventTable.Fields[EventFields.Reg].AsString().Trim();
                                        if (DAus[96] == "")
                                        {
                                            DAus[96] = 0.AsString();
                                        }

                                        if (unchecked(0 - (ki ? 1 : 0)) == 0)
                                        {
                                            num6 = (short)DAus[96].AsInt();
                                        }
                                        if (unchecked(0 - (ki ? 1 : 0)) == 1)
                                        {
                                            num6 = (short)DAus[98].AsInt();
                                        }
                                        num6 = 1;
                                        if (num6 == 1)
                                        {
                                            string namen = Namen;
                                            int persInArb = PersInArb;
                                            short num10 = 1;
                                            while (num10 <= 100)
                                            {
                                                KontSP1[num10] = Kont1[num10];
                                                KontSP[num10] = Kont[num10];
                                                Kont[num10] = "";
                                                Kont1[num10] = "";
                                                num10 = (short)unchecked(num10 + 1);
                                            }

                                            Zeugsu(num4, 0, Listart, Ahne);
                                            Namen = namen;
                                            PersInArb = persInArb;
                                            string text = Kont1[20];
                                            num10 = 1;
                                            while (num10 <= 100)
                                            {
                                                Kont1[num10] = KontSP1[num10];
                                                Kont[num10] = KontSP[num10];
                                                KontSP[num10] = "";
                                                KontSP1[num10] = "";
                                                num10 = (short)unchecked(num10 + 1);
                                            }
                                            Kont[(int)num4 - 50] = text;
                                            text = "";
                                        }
                                    }
                                }
                            }
                            goto IL_101d;

                        IL_101d: // <========== 8
                            num = 196;
                            lErl = 2;
                            num4 = unchecked(num4 + 1);
                            if (num4 <= EEventArt.eA_Burial)
                            {
                                goto IL_006f;
                            }
                            Kont[25] = RechWithProps();
                            Art1 = 0;
                            goto end_IL_0000_2;
                        IL_1119: // <========== 3
                            num5 = unchecked(num2 + 1);
                            goto IL_111d;
                        IL_111d:
                            num2 = 0;
                            switch (num5)
                            {
                                case 1:
                                    break;
                                case 12:
                                    goto IL_006f;
                                case 14:
                                case 25:
                                case 34:
                                case 57:
                                case 62:
                                case 194:
                                case 195:
                                case 196:
                                    goto IL_101d;
                                case 41:
                                case 70:
                                case 200:
                                case 202:
                                case 206:
                                case 207:
                                case 210:
                                case 211:
                                case 217:
                                case 218:
                                case 219:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 5271;
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
    public string Datwand2(string Datu, ref string Ds, ref string Datu1)
    {
        if (DAus[73] == "1" & DAus[105].AsDouble() > 0.0 && Datu.AsDouble() > (DAus[105] + "0000").AsDouble())
        {
            Datu = DTxt[14];
            return Datu;
        }
        if (DAus[74] == "1" & DAus[109].AsDouble() > 0.0 && Datu.AsDouble() > (DAus[109] + "0000").AsDouble())
        {
            Datu = "(" + Datu.Left(4) + ")";
            return Datu;
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
        checked
        {
            if (Datu.Length > 0)
            {
                short num = 1;
                do
                {
                    short num2 = (short)Strings.InStr(Datu, " ");
                    if (num2 > 0)
                    {
                        StringType.MidStmtStr(ref Datu, num2, 1, ".");
                    }
                    num = (short)unchecked(num + 1);
                }
                while (num <= 2);
            }
            if (Art == EEventArt.eA_Birth)
            {
                Datum2 = "           " + Datu.Right(10);
            }
            if (Art == EEventArt.eA_Baptism & Datum2.Trim() == "")
            {
                Datum2 = "           " + Datu.Right(10);
            }
            if (Art == EEventArt.eA_Death)
            {
                Datum1 = "           " + Datu.Right(10);
            }
            if (Art == EEventArt.eA_Burial & Datum1.Trim() == "")
            {
                Datum1 = "           " + Datu.Right(10);
            }
            switch (Ds)
            {
                case "U":
                case "u":
                    Datu = "um " + Datu;
                    break;
                case "V":
                case "v":
                    Datu = "vor " + Datu;
                    break;
                case "N":
                case "n":
                    Datu = "nach " + Datu;
                    break;
                case "?":
                    Datu += " ?";
                    break;
                case "r":
                case "R":
                    Datu = "errech " + Datu;
                    break;
                case "Z":
                case "z":
                    Datu = "zwischen " + Datu;
                    break;
                case "a":
                case "A":
                    Datu = "und " + Datu;
                    break;
                case "von":
                    Datu = "von " + Datu;
                    break;
                case "b":
                case "B":
                    Datu = " bis " + Datu;
                    break;
                default:
                    if (Conversions.ToDouble(DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString().Trim()) == 0.0 && Art > EEventArt.eA_100 & Art < EEventArt.eA_105 | Art > EEventArt.eA_400 && Datschalt != 14 && Datu.Length == 10 & DAus[68] == "1")
                    {
                        Datu = "am " + Datu;
                    }
                    break;
            }
            Ds = "";
        }
        return Datu;
    }


    //public void ortles1(int OrtNr, byte Schalt)
    //{
    //    int try0000_dispatch = -1;
    //    int num3 = default;
    //    int num2 = default;
    //    int num = default;
    //    string[] array = default;
    //    byte b = default;
    //    string prompt = default;
    //    while (true)
    //    {
    //        try
    //        {
    //            /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
    //            ;
    //            int num4;
    //            int AAA;
    //            string LD;
    //            switch (try0000_dispatch)
    //            {
    //                default:
    //                    ProjectData.ClearProjectError();
    //                    num3 = 2;
    //                    goto IL_0008;
    //                case 2598:
    //                    {
    //                        num2 = num;
    //                        switch (num3)
    //                        {
    //                            case 2:
    //                            case 3:
    //                                break;
    //                            case 1:
    //                                goto IL_08a4;
    //                            default:
    //                                goto end_IL_0000;
    //                        }
    //                        goto IL_0800;
    //                    }
    //                IL_0800:
    //                    num = 80;
    //                    if (Information.Err().Number == 3022)
    //                    {
    //                        goto IL_0815;
    //                    }
    //                    goto IL_082f;
    //                IL_082f:
    //                    num = 84;
    //                    if (Information.Err().Number == 94)
    //                    {
    //                        goto IL_0841;
    //                    }
    //                    goto IL_085b;
    //                IL_085b:
    //                    num = 88;
    //                    if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
    //                    {
    //                        ProjectData.EndApp();
    //                    }
    //                    goto IL_0881;
    //                IL_07a4:
    //                    num = 72;
    //                    DataModul.DSB_OrtIdxTable.Commit();
    //                    goto IL_07b4;
    //                IL_0881:
    //                    num = 91;
    //                    ProjectData.ClearProjectError();
    //                    if (num2 == 0)
    //                    {
    //                        throw ProjectData.CreateProjectError(-2146828268);
    //                    }
    //                    num4 = num2;
    //                    goto IL_08a8;
    //                IL_07b4:
    //                    num = 76;
    //                    if (DAus[91] != "1")
    //                    {
    //                        goto end_IL_0000_2;
    //                    }
    //                    goto IL_07ce;
    //                IL_07ce:
    //                    num = 77;
    //                    UbgT = UbgT + " [" + OrtNr.AsString().Trim() + "] ";
    //                    goto end_IL_0000_2;
    //                IL_08a8:
    //                    num2 = 0;
    //                    switch (num4)
    //                    {
    //                        case 1:
    //                            break;
    //                        case 2:
    //                            goto IL_0008;
    //                        case 3:
    //                            goto IL_0012;
    //                        case 4:
    //                            goto IL_0017;
    //                        case 5:
    //                            goto IL_0022;
    //                        case 6:
    //                            goto IL_002e;
    //                        case 7:
    //                            goto IL_0082;
    //                        case 8:
    //                            goto IL_0094;
    //                        case 9:
    //                            goto IL_00d7;
    //                        case 10:
    //                            goto IL_011b;
    //                        case 11:
    //                            goto IL_0130;
    //                        case 12:
    //                        case 13:
    //                            goto IL_0149;
    //                        case 14:
    //                            goto IL_018d;
    //                        case 15:
    //                            goto IL_01a2;
    //                        case 16:
    //                        case 17:
    //                            goto IL_01b6;
    //                        case 18:
    //                            goto IL_01fa;
    //                        case 19:
    //                            goto IL_020f;
    //                        case 20:
    //                        case 21:
    //                            goto IL_0223;
    //                        case 22:
    //                            goto IL_024c;
    //                        case 23:
    //                            goto IL_0290;
    //                        case 24:
    //                            goto IL_02a5;
    //                        case 25:
    //                        case 26:
    //                        case 27:
    //                            goto IL_02be;
    //                        case 28:
    //                            goto IL_0302;
    //                        case 29:
    //                            goto IL_0317;
    //                        case 30:
    //                        case 31:
    //                            goto IL_032b;
    //                        case 32:
    //                            goto IL_0354;
    //                        case 33:
    //                        case 34:
    //                            goto IL_037e;
    //                        case 35:
    //                            goto IL_03a7;
    //                        case 36:
    //                        case 37:
    //                            goto IL_03d1;
    //                        case 38:
    //                            goto IL_03e6;
    //                        case 39:
    //                            goto IL_0400;
    //                        case 40:
    //                        case 42:
    //                            goto IL_040c;
    //                        case 43:
    //                            goto IL_0419;
    //                        case 44:
    //                            goto IL_0464;
    //                        case 45:
    //                            goto IL_04af;
    //                        case 47:
    //                        case 48:
    //                            goto IL_04ff;
    //                        case 49:
    //                            goto IL_0508;
    //                        case 51:
    //                            goto IL_057c;
    //                        case 52:
    //                            goto IL_0580;
    //                        case 53:
    //                            goto IL_05ab;
    //                        case 54:
    //                            goto IL_05bc;
    //                        case 50:
    //                        case 55:
    //                        case 56:
    //                        case 57:
    //                            goto IL_0613;
    //                        case 58:
    //                            goto IL_0624;
    //                        case 59:
    //                            goto IL_0643;
    //                        case 60:
    //                        case 61:
    //                            goto IL_0655;
    //                        case 62:
    //                            goto IL_0671;
    //                        case 63:
    //                        case 64:
    //                            goto IL_06bf;
    //                        case 65:
    //                            goto IL_06c7;
    //                        case 66:
    //                            goto IL_06e3;
    //                        case 67:
    //                            goto IL_06fd;
    //                        case 68:
    //                            goto IL_070b;
    //                        case 69:
    //                            goto IL_073d;
    //                        case 70:
    //                            goto IL_075f;
    //                        case 71:
    //                            goto IL_0782;
    //                        case 72:
    //                            goto IL_07a4;
    //                        case 73:
    //                        case 74:
    //                        case 75:
    //                        case 76:
    //                            goto IL_07b4;
    //                        case 77:
    //                            goto IL_07ce;
    //                        case 80:
    //                            goto IL_0800;
    //                        case 81:
    //                            goto IL_0815;
    //                        case 82:
    //                        case 84:
    //                            goto IL_082f;
    //                        case 85:
    //                            goto IL_0841;
    //                        case 86:
    //                        case 88:
    //                            goto IL_085b;
    //                        case 89:
    //                        case 91:
    //                            goto IL_0881;
    //                        default:
    //                            goto end_IL_0000;
    //                        case 46:
    //                        case 78:
    //                        case 79:
    //                        case 92:
    //                            goto end_IL_0000_2;
    //                    }
    //                    goto default;
    //                IL_0841:
    //                    num = 85;
    //                    ProjectData.ClearProjectError();
    //                    if (num2 == 0)
    //                    {
    //                        throw ProjectData.CreateProjectError(-2146828268);
    //                    }
    //                    goto IL_08a4;
    //                IL_0782:
    //                    num = 71;
    //                    DataModul.DSB_OrtIdxTable.Fields["Ind"].Value = Ind1;
    //                    goto IL_07a4;
    //                IL_0815:
    //                    num = 81;
    //                    ProjectData.ClearProjectError();
    //                    if (num2 == 0)
    //                    {
    //                        throw ProjectData.CreateProjectError(-2146828268);
    //                    }
    //                    goto IL_08a4;
    //                IL_075f:
    //                    num = 70;
    //                    DataModul.DSB_OrtIdxTable.Fields["OrtNr"].Value = OrtNr;
    //                    goto IL_0782;
    //                IL_08a4:
    //                    num4 = num2 + 1;
    //                    goto IL_08a8;
    //                IL_0008:
    //                    num = 2;
    //                    array = new string[8];
    //                    goto IL_0012;
    //                IL_0012:
    //                    num = 3;
    //                    b = 1;
    //                    goto IL_0017;
    //                IL_0017:
    //                    num = 4;
    //                    array[b] = "";
    //                    goto IL_0022;
    //                IL_0022:
    //                    num = 5;
    //                    b = checked((byte)unchecked((uint)(b + 1)));
    //                    if (b <= 6u)
    //                    {
    //                        goto IL_0017;
    //                    }
    //                    goto IL_002e;
    //                IL_002e:
    //                    num = 6;
    //                    DataModul.DB_PlaceTable.Seek("=", OrtNr);
    //                    goto IL_0082;
    //                IL_0082:
    //                    num = 7;
    //                    if (!DataModul.DB_PlaceTable.NoMatch)
    //                    {
    //                        goto IL_0094;
    //                    }
    //                    goto IL_07b4;
    //                IL_0094:
    //                    num = 8;
    //                    AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt();
    //                    LD = "";
    //                    DataModul.Textlese(AAA, out array[1], out LD);
    //                    goto IL_00d7;
    //                IL_00d7:
    //                    num = 9;
    //                    AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.OrtsTeil].AsInt();
    //                    LD = "";
    //                    DataModul.Textlese(AAA, out array[2], out LD);
    //                    goto IL_011b;
    //                IL_011b:
    //                    num = 10;
    //                    if (array[2] != "")
    //                    {
    //                        goto IL_0130;
    //                    }
    //                    goto IL_0149;
    //                IL_0130:
    //                    num = 11;
    //                    array[2] = "-" + array[2].Trim();
    //                    goto IL_0149;
    //                IL_0149:
    //                    num = 13;
    //                    AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Kreis].AsInt();
    //                    LD = "";
    //                    DataModul.Textlese(AAA, out array[3], out LD);
    //                    goto IL_018d;
    //                IL_018d:
    //                    num = 14;
    //                    if (array[3] != "")
    //                    {
    //                        goto IL_01a2;
    //                    }
    //                    goto IL_01b6;
    //                IL_01a2:
    //                    num = 15;
    //                    array[3] = ", " + array[3];
    //                    goto IL_01b6;
    //                IL_01b6:
    //                    num = 17;
    //                    AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Land].AsInt();
    //                    LD = "";
    //                    DataModul.Textlese(AAA, out array[4], out LD);
    //                    goto IL_01fa;
    //                IL_01fa:
    //                    num = 18;
    //                    if (array[4] != "")
    //                    {
    //                        goto IL_020f;
    //                    }
    //                    goto IL_0223;
    //                IL_020f:
    //                    num = 19;
    //                    array[4] = ", " + array[4];
    //                    goto IL_0223;
    //                IL_0223:
    //                    num = 21;
    //                    if (!Information.IsDBNull(DataModul.DB_PlaceTable.Fields[PlaceFields.Polname].Value))
    //                    {
    //                        goto IL_024c;
    //                    }
    //                    goto IL_02be;
    //                IL_024c:
    //                    num = 22;
    //                    AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.PolName].AsInt();
    //                    LD = "";
    //                    DataModul.Textlese(AAA, out array[7], out LD);
    //                    goto IL_0290;
    //                IL_0290:
    //                    num = 23;
    //                    if (array[7] != "")
    //                    {
    //                        goto IL_02a5;
    //                    }
    //                    goto IL_02be;
    //                IL_02a5:
    //                    num = 24;
    //                    array[7] = " (" + array[7] + ")";
    //                    goto IL_02be;
    //                IL_02be:
    //                    num = 27;
    //                    AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Staat].AsInt();
    //                    LD = "";
    //                    DataModul.Textlese(AAA, out array[5], out LD);
    //                    goto IL_0302;
    //                IL_0302:
    //                    num = 28;
    //                    if (array[5] != "")
    //                    {
    //                        goto IL_0317;
    //                    }
    //                    goto IL_032b;
    //                IL_0317:
    //                    num = 29;
    //                    array[5] = ", " + array[5];
    //                    goto IL_032b;
    //                IL_032b:
    //                    num = 31;
    //                    if (!Information.IsDBNull(DataModul.DB_PlaceTable.Fields[PlaceFields.Zusatz].Value))
    //                    {
    //                        goto IL_0354;
    //                    }
    //                    goto IL_037e;
    //                IL_0354:
    //                    num = 32;
    //                    array[6] = DataModul.DB_PlaceTable.Fields[PlaceFields.Zusatz].AsString().Trim();
    //                    goto IL_037e;
    //                IL_037e:
    //                    num = 34;
    //                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Zusatz].Value))
    //                    {
    //                        goto IL_03a7;
    //                    }
    //                    goto IL_03d1;
    //                IL_03a7:
    //                    num = 35;
    //                    array[6] = DataModul.DB_EventTable.Fields[EventFields.Zusatz].AsString().Trim();
    //                    goto IL_03d1;
    //                IL_03d1:
    //                    num = 37;
    //                    if (array[6] == "")
    //                    {
    //                        goto IL_03e6;
    //                    }
    //                    goto IL_040c;
    //                IL_03e6:
    //                    num = 38;
    //                    if (DAus[69] == "1")
    //                    {
    //                        goto IL_0400;
    //                    }
    //                    goto IL_040c;
    //                IL_0400:
    //                    num = 39;
    //                    array[6] = "in";
    //                    goto IL_040c;
    //                IL_040c:
    //                    num = 42;
    //                    if (Schalt == 20)
    //                    {
    //                        goto IL_0419;
    //                    }
    //                    goto IL_04ff;
    //                IL_0419:
    //                    num = 43;
    //                    UbgT = UbgT + Strings.Left(DataModul.DB_PlaceTable.Fields[PlaceFields.Terr].AsString().Trim() + "   ", 3) + "    ";
    //                    goto IL_0464;
    //                IL_0464:
    //                    num = 44;
    //                    UbgT = UbgT + Strings.Left(DataModul.DB_PlaceTable.Fields[PlaceFields.Staatk].AsString().Trim() + " ", 1) + "    ";
    //                    goto IL_04af;
    //                IL_04af:
    //                    num = 45;
    //                    UbgT = UbgT + Strings.Left(DataModul.DB_PlaceTable.Fields[PlaceFields.PLZ].AsString().Trim() + "      ", 6) + " ";
    //                    goto end_IL_0000_2;
    //                IL_04ff:
    //                    num = 48;
    //                    if (Schalt == 0)
    //                    {
    //                        goto IL_0508;
    //                    }
    //                    goto IL_057c;
    //                IL_0508:
    //                    num = 49;
    //                    UbgT = array[1].TrimEnd() + array[2].TrimEnd() + " " + array[3].TrimEnd() + " " + array[4].TrimEnd() + " " + array[5].TrimEnd();
    //                    goto IL_0613;
    //                IL_057c:
    //                    num = 51;
    //                    goto IL_0580;
    //                IL_0580:
    //                    num = 52;
    //                    UbgT = array[6].Trim() + " " + array[1].TrimEnd() + array[2].TrimEnd();
    //                    goto IL_05ab;
    //                IL_05ab:
    //                    num = 53;
    //                    if (UbgT.Length <= 3)
    //                    {
    //                        goto IL_05bc;
    //                    }
    //                    goto IL_0613;
    //                IL_05bc:
    //                    num = 54;
    //                    UbgT = UbgT + array[3].TrimEnd() + " " + array[4].TrimEnd() + " " + array[5].TrimEnd();
    //                    goto IL_0613;
    //                IL_0613:
    //                    num = 57;
    //                    if (UbgT.Length <= 3)
    //                    {
    //                        goto IL_0624;
    //                    }
    //                    goto IL_0655;
    //                IL_0624:
    //                    num = 58;
    //                    prompt = "Der Ort PerNr.:" + OrtNr.AsString() + " ist leer.\nBitte berichtigen";
    //                    goto IL_0643;
    //                IL_0643:
    //                    num = 59;
    //                    Interaction.MsgBox(prompt, MsgBoxStyle.Exclamation, "Problem!");
    //                    goto IL_0655;
    //                IL_0655:
    //                    num = 61;
    //                    if (UbgT.Trim() == "")
    //                    {
    //                        goto IL_0671;
    //                    }
    //                    goto IL_06bf;
    //                IL_0671:
    //                    num = 62;
    //                    UbgT = array[3].TrimEnd() + " " + array[4].TrimEnd() + " " + array[5].TrimEnd();
    //                    goto IL_06bf;
    //                IL_06bf:
    //                    ProjectData.ClearProjectError();
    //                    num3 = 3;
    //                    goto IL_06c7;
    //                IL_06c7:
    //                    num = 65;
    //                    if ((Ind1).AsDouble() > 0.0)
    //                    {
    //                        goto IL_06e3;
    //                    }
    //                    goto IL_07b4;
    //                IL_06e3:
    //                    num = 66;
    //                    if (Namen != "")
    //                    {
    //                        goto IL_06fd;
    //                    }
    //                    goto IL_07b4;
    //                IL_06fd:
    //                    num = 67;
    //                    DataModul.DSB_OrtIdxTable.AddNew();
    //                    goto IL_070b;
    //                IL_070b:
    //                    num = 68;
    //                    DataModul.DSB_OrtIdxTable.Fields["Ort"].Value = array[1].TrimEnd() + array[2].TrimEnd();
    //                    goto IL_073d;
    //                IL_073d:
    //                    num = 69;
    //                    DataModul.DSB_OrtIdxTable.Fields["Name"].Value = Namen;
    //                    goto IL_075f;
    //                end_IL_0000:
    //                    break;
    //            }
    //        }
    //        catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
    //        {
    //            ProjectData.SetProjectError(obj);
    //            try0000_dispatch = 2598;
    //            continue;
    //        }
    //        throw ProjectData.CreateProjectError(-2146828237);
    //    end_IL_0000_2:
    //        break;
    //    }
    //    if (num2 != 0)
    //    {
    //        ProjectData.ClearProjectError();
    //    }
    //}
    public string ortles1(int OrtNr, byte Schalt, Action<int, string> exportPlace)
    {
        string[] array = default;
        string prompt = default;
        string UbgT = "";
        array = new string[8];
        array.Initialize();
        if (!DataModul.Place.ReadData(OrtNr, out var placeData))
        {
            array[1] = placeData.sOrt;
            array[2] = placeData.sOrtsteil;
            array[3] = placeData.sKreis;
            array[4] = placeData.sLand;
            array[5] = placeData.sStaat;
            array[6] = placeData.sZusatz;
            array[7] = placeData.sPolName;

            if (array[2] != "")
            {
                array[2] = "-" + array[2].Trim();
            }
            if (array[3] != "")
            {
                array[3] = ", " + array[3];
            }
            if (array[4] != "")
            {
                array[4] = ", " + array[4];
            }
            if (array[5] != "")
            {
                array[5] = ", " + array[5];
            }
            if (array[7] != "")
            {
                array[7] = " (" + array[7] + ")";
            }

            // ??
            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Zusatz].Value))
            {
                array[6] = DataModul.DB_EventTable.Fields[EventFields.Zusatz].AsString().Trim();
            }
            if (array[6] == "")
            {
                if (DAus[69] == "1")
                {
                    array[6] = "in";
                }
            }

            UbgT = array[6].Trim() + " " + array[1].TrimEnd() + array[2].TrimEnd();
            if (UbgT.Length <= 3)
            {
                UbgT = UbgT + array[3].TrimEnd() + " " + array[4].TrimEnd() + " " + array[5].TrimEnd();
            }

            if (UbgT.Length <= 3)
            {
                prompt = "Der Ort Nr.:" + OrtNr.AsString() + " ist leer.\nBitte berichtigen";
                Interaction.MsgBox(prompt, MsgBoxStyle.Exclamation, "Problem!");
            }
            if (UbgT.Trim() == "")
            {
                UbgT = array[3].TrimEnd() + " " + array[4].TrimEnd() + " " + array[5].TrimEnd();
            }
            exportPlace?.Invoke(OrtNr, array[1].TrimEnd() + array[2].TrimEnd());
        }
        if (DAus[91] == "1")
        {
            UbgT = UbgT + " [" + OrtNr.AsString().Trim() + "] ";
        }
        return UbgT;
    }

    public void ExportPlace(int OrtNr, string sOrt, string ind1, string namen)
    {
        if (ind1.AsInt() > 0 && Namen != "")
        {
            DataModul.DSB_OrtIdxTable.AddNew();
            DataModul.DSB_OrtIdxTable.Fields["Ort"].Value = sOrt;
            DataModul.DSB_OrtIdxTable.Fields["Name"].Value = namen;
            DataModul.DSB_OrtIdxTable.Fields["OrtNr"].Value = OrtNr;
            DataModul.DSB_OrtIdxTable.Fields["Ind"].Value = ind1;
            DataModul.DSB_OrtIdxTable.Update();
        }
    }


    public void Datles4(bool Ki)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        EEventArt num5 = default;
        string Datu = default;
        object CounterResult = default;
        object LoopForResult = default;
        string Ds2 = default;
        string Ds3 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    IField field;
                    int ortNr;
                    byte Schalt;
                    int AAA;
                    string Ds;
                    switch (try0000_dispatch)
                    {
                        default:
                            {
                                num = 1;
                                Datu = "";
                                High = 0;
                                ProjectData.ClearProjectError();
                                num3 = 2;
                                Datum1 = "";
                                Datum2 = "";
                                var _num5 = 1;
                                while (_num5 <= 40)
                                {
                                    Kont[_num5] = "";
                                    _num5 = (short)unchecked(_num5 + 1);
                                }
                                num5 = EEventArt.eA_Birth;
                                goto IL_0079;
                            }
                        case 3731:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0c49;
                                    default:
                                        goto end_IL_0000;
                                }
                                int number = Information.Err().Number;
                                if (number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0c49;
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
                                goto IL_0c4d;
                            }
                        end_IL_0000:
                            break;
                        IL_0079: // <========== 3
                            num = 13;
                            var J = 0;
                            while (unchecked(J) <= 20u)
                            {
                                Kont1[J] = "";
                                J++;
                            }
                            Ubg = (int)num5;
                            Art = num5;
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", Ubg.AsString(), PersInArb.AsString(), "0");
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                if (Datschalt == 2)
                                {
                                    if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, MyProject.Forms.Hinter.List1.Items.Count - 1, 1, ref LoopForResult, ref CounterResult))
                                    {
                                        while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult, ref CounterResult))
                                        {
                                            Kol = (int)Math.Round(Conversion.Val(Strings.Right(MyProject.Forms.Hinter.List1.Items[CounterResult.AsInt()].AsString(), 6)));
                                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() == Kol)
                                            {
                                                Datschalt = 3;
                                                goto end_IL_0000_2;
                                            }
                                        }

                                    }
                                }
                                goto IL_022d;
                            }
                            goto IL_0b92;
                        IL_022d: // <========== 3
                            num = 32;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                            {
                                _Modul1.Instance.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                if (Ubg < 103)
                                {
                                    if (_Modul1.Instance.sDatu.AsDouble() > High)
                                    {
                                        High = (int)Math.Round(_Modul1.Instance.sDatu.AsDouble());

                                    }
                                }
                                goto IL_02df;
                            }
                            goto IL_03f5;
                        IL_02df: // <========== 3
                            num = 39;
                            if (Datschalt == 1)
                            {
                                Kont[Ubg - 100] = _Modul1.Instance.sDatu;
                                field = DataModul.DB_EventTable.Fields[EventFields.DatumV_S];
                                Ds = field.AsString();
                                _Modul1.Instance.sDatu = Datwand2(_Modul1.Instance.sDatu, ref Ds, ref Datu);
                                field.Value = Ds;
                                Kont1[1] = _Modul1.Instance.sDatu;
                                goto IL_0b92;
                            }
                            if (Datschalt == 10)
                            {
                                Kont[Ubg - 100] = DataModul.DB_EventTable.Fields[EventFields.Reg].AsString();
                                Kont[Ubg - 90] = _Modul1.Instance.sDatu;
                                goto IL_0b92;
                            }
                            Ds2 = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                            _Modul1.Instance.sDatu = Datwand2(_Modul1.Instance.sDatu, ref Ds2, ref Datu);
                            Kont1[1] = _Modul1.Instance.sDatu;
                            goto IL_03f5;
                        IL_03f5: // <========== 3
                            num = 54;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                _Modul1.Instance.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString().Trim(), 8);
                                Ds3 = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                _Modul1.Instance.sDatu = Datwand2(_Modul1.Instance.sDatu, ref Ds3, ref Datu);
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString().Trim(), "", TextCompare: false) == 0)
                                {
                                    Kont1[2] = " / " + _Modul1.Instance.sDatu;
                                }
                                else
                                {

                                    Kont1[2] = " " + _Modul1.Instance.sDatu;
                                }
                                goto IL_0504;
                            }
                            goto IL_0532;
                        IL_0504: // <========== 3
                            num = 64;
                            Kont1[1] = Kont1[1] + Kont1[2];
                            Kont1[2] = "";
                            goto IL_0532;
                        IL_0532: // <========== 3
                            num = 67;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.DatumText].Value))
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt() > 0)
                                {
                                    string value = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsString();
                                    AAA = value.AsInt();
                                    Ds = "";
                                    UbgT = DataModul.TextLese1(AAA);
                                    value = AAA.AsString();
                                    if (UbgT.Trim() != "")
                                    {
                                        Kont1[1] = Kont1[1] + " (" + UbgT.Trim() + ")";
                                        UbgT = "";
                                    }

                                }
                            }
                            goto IL_0630;
                        IL_0630: // <========== 3
                            num = 77;
                            UbgT = "";
                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt() != 0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                Ds = "";
                                Kont[0] = DataModul.TextLese1(AAA);
                                if (Kont[0] != "")
                                {
                                    Kont1[3] = " " + Kont[0].Trim();

                                }
                            }
                            goto IL_06ef;
                        IL_06ef: // <========== 3
                            num = 84;
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                            {
                                UbgT = ortles1(DataModul.DB_EventTable.Fields[EventFields.Ort].AsInt(), 1, (i, s) => ExportPlace(i, s, Ind1, Namen));
                            }
                            goto IL_0758;
                        IL_0758:
                            num = 87;
                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString().Trim(), "", TextCompare: false) != 0)
                            {
                                UbgT = UbgT.Trim() + " ?";
                            }
                            goto IL_07aa;
                        IL_07aa:
                            num = 90;
                            if (DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt() != 0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                Ds = "";
                                Kont[0] = DataModul.TextLese1(AAA);
                                if (Kont[0] != "")
                                {
                                    Kont1[6] = " " + Kont[0].Trim();

                                }
                            }
                            goto IL_085b;
                        IL_085b: // <========== 3
                            num = 96;
                            if (DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().TrimEnd() != "" | DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().TrimEnd() != "")
                            {
                                Kont[Ubg - 90] = UbgT + " " + (Kont1[1] + Kont1[2] + Kont1[3] + Kont1[6]).Trim();
                                UbgT = "";
                                string text2 = "";
                                text2 = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().TrimEnd();
                                Kont[Ubg - 85] = text2;
                                text2 = "";
                                text2 = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().TrimEnd();
                                Kont[Ubg - 80] = text2;
                            }
                            else
                            {

                                Kont[Ubg - 90] = UbgT + " " + (Kont1[1] + Kont1[2] + Kont1[3]).Trim() + Kont1[6];
                                UbgT = "";
                            }
                            goto IL_0a07;
                        IL_0a07: // <========== 3
                            num = 110;
                            if (Datschalt == 0)
                            {
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem3].Value))
                                {
                                    if (unchecked(DAus[62] == "1" & 0 - (Ki ? 1 : 0) == 0 | DAus[63] == "1" & 0 - (Ki ? 1 : 0) == 1))
                                    {
                                        if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            BemZahl++;
                                            KontBem[BemZahl] = BemZahl.AsString().Trim() + ".) " + DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim();
                                            Kont1[9] = BemZahl.AsString().Trim() + ".) ";

                                        }
                                    }
                                    goto IL_0b54;
                                }
                            }
                            goto IL_0b92;
                        IL_0b54: // <========== 3
                            num = 119;
                            if (Kont1[9].Trim() != "")
                            {
                                Kont[Ubg - 70] = Kont1[9].Trim();
                            }
                            goto IL_0b92;
                        IL_0b92: // <========== 6
                            num = 124;
                            lErl = 2;
                            num5 = unchecked(num5 + 1);
                            if (num5 <= EEventArt.eA_Burial)
                            {
                                goto IL_0079;
                            }
                            Kont[25] = RechWithProps();
                            goto end_IL_0000_2;
                        IL_0c49:
                            num4 = unchecked(num2 + 1);
                            goto IL_0c4d;
                        IL_0c4d:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 13:
                                    goto IL_0079;
                                case 31:
                                case 32:
                                    goto IL_022d;
                                case 37:
                                case 38:
                                case 39:
                                    goto IL_02df;
                                case 53:
                                case 54:
                                    goto IL_03f5;
                                case 60:
                                case 63:
                                case 64:
                                    goto IL_0504;
                                case 66:
                                case 67:
                                    goto IL_0532;
                                case 74:
                                case 75:
                                case 76:
                                case 77:
                                    goto IL_0630;
                                case 82:
                                case 83:
                                case 84:
                                    goto IL_06ef;
                                case 86:
                                case 87:
                                    goto IL_0758;
                                case 89:
                                case 90:
                                    goto IL_07aa;
                                case 94:
                                case 95:
                                case 96:
                                    goto IL_085b;
                                case 105:
                                case 109:
                                case 110:
                                    goto IL_0a07;
                                case 117:
                                case 118:
                                case 119:
                                    goto IL_0b54;
                                case 21:
                                case 43:
                                case 48:
                                case 121:
                                case 122:
                                case 123:
                                case 124:
                                    goto IL_0b92;
                                case 28:
                                case 127:
                                case 129:
                                case 133:
                                case 134:
                                case 140:
                                case 141:
                                case 142:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 3731;
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

    public void Datles10(short Listart, bool ki)
    {
        int try0000_dispatch = -1;
        int num = default;
        string Datu = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        EEventArt num5 = default;
        string Ds = default;
        short num6 = default;
        EEventArt num7 = default;
        short num8 = default;
        EEventArt num11 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    short num10;
                    int ortNr;
                    int AAA2;
                    int Nr;
                    short LfNR;
                    byte Schalt;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            Datu = "";
                            goto IL_000a;
                        case 5037:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_106b;
                                    default:
                                        goto end_IL_0000;
                                }
                                int number = Information.Err().Number;
                                if (number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_106b;
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
                                goto IL_106f;
                            }
                        end_IL_0000:
                            break;
                        IL_000a:
                            num = 2;
                            Ds = "";
                            if (Datschalt == 1)
                            {
                                num6 = 101;
                                num7 = EEventArt.eA_Baptism;
                            }
                            if (Datschalt == 3)
                            {
                                num6 = 103;
                                num7 = EEventArt.eA_Burial;
                            }
                            High = 0;
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            Datum1 = "";
                            Datum2 = "";
                            num8 = 0;
                            var _num5 = 1;
                            while (_num5 <= 40)
                            {
                                Kont[_num5] = "";
                                _num5++;
                            }
                            num10 = num6;
                            num11 = num7;
                            num5 = (EEventArt)num10;
                            goto IL_0fcf;
                        IL_020b: // <========== 3
                            num = 38;
                            if (!(iPrivacy.AsDouble() > iPriv_aus.AsDouble()))
                            {
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    _Modul1.Instance.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                    if (Ubg < 103)
                                    {
                                        if (_Modul1.Instance.sDatu.AsDouble() > High)
                                        {
                                            High = (int)Math.Round(_Modul1.Instance.sDatu.AsDouble());

                                        }
                                    }
                                    goto IL_02dc;
                                }
                                goto IL_05f9;
                            }
                            goto IL_0fb7;
                        IL_02dc: // <========== 3
                            num = 48;
                            Ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                            _Modul1.Instance.sDatu = Datwand2(_Modul1.Instance.sDatu, ref Ds, ref Datu);
                            int AAA;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.DatumText].Value))
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt();
                                UbgT = DataModul.TextLese1(AAA);
                                if (UbgT.Trim() != "")
                                {
                                    _Modul1.Instance.sDatu = _Modul1.Instance.sDatu + " (" + UbgT.Trim() + ")";
                                    UbgT = "";

                                }
                            }
                            goto IL_03cb;
                        IL_03cb: // <========== 3
                            num = 58;
                            Kont1[1] = _Modul1.Instance.sDatu;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                _Modul1.Instance.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString().Trim(), 8);
                                Ds = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                _Modul1.Instance.sDatu = Datwand2(_Modul1.Instance.sDatu, ref Ds, ref Datu);
                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString().Trim(), "", TextCompare: false) == 0)
                                {
                                    Kont1[3] = " / " + _Modul1.Instance.sDatu;
                                    goto IL_04e9;
                                }
                                Kont1[3] = " " + _Modul1.Instance.sDatu;
                            }
                            goto IL_04e9;
                        IL_04e9: // <========== 3
                            num = 70;
                            if (Operators.CompareString(_Modul1.Instance.sDatu.Left(1), "(", TextCompare: false) == 0)
                            {
                                Kont[(int)Art - 90] = Kont1[1];
                                if (DAus[110] != "1")
                                {
                                    goto end_IL_0000_2;
                                }
                            }
                            goto IL_053f;
                        IL_053f:
                            num = 76;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.DatumText].Value))
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt();
                                UbgT = DataModul.TextLese1(AAA);
                                if (UbgT.Trim() != "")
                                {
                                    _Modul1.Instance.sDatu = _Modul1.Instance.sDatu + " (" + UbgT.Trim() + "22)";
                                    UbgT = "";

                                }
                            }
                            goto IL_05f9;
                        IL_05f9: // <========== 4
                            num = 85;
                            if (Ubg == 103)
                            {
                                if (DAus[108].AsDouble() == 1.0)
                                {
                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields["causal"].Value))
                                    {
                                        if (DataModul.DB_EventTable.Fields["causal"].AsInt() > 0)
                                        {
                                            AAA2 = DataModul.DB_EventTable.Fields["causal"].AsInt();
                                            Kont[0] = DataModul.TextLese1(AAA2);
                                            Kont1[17] = " " + Kont[0].Trim() + " ";
                                            Kont[0] = "";
                                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields["an"].Value))
                                            {
                                                if (DataModul.DB_EventTable.Fields["an"].AsInt() > 0)
                                                {
                                                    AAA2 = DataModul.DB_EventTable.Fields["an"].AsInt();
                                                    Kont[0] = DataModul.TextLese1(AAA2);

                                                }
                                            }
                                            goto IL_079f;
                                        }
                                    }

                                }
                            }
                            goto IL_084e;
                        IL_079f: // <========== 3
                            num = 97;
                            if (Kont[0].Trim() == "")
                            {
                                Kont[0] = "an";
                            }
                            goto IL_07cd;
                        IL_07cd:
                            num = 100;
                            if (Kont[0].Trim() == "°")
                            {
                                Kont[0] = "";
                            }
                            goto IL_07fb;
                        IL_07fb:
                            num = 103;
                            Kont1[1] = Kont1[1] + " " + Kont[0].Trim() + Kont1[17] + " ";
                            goto IL_084e;
                        IL_084e: // <========== 4
                            num = 108;
                            UbgT = "";
                            if (_Modul1.Instance.sDatu == "")
                            {
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.DatumText].Value))
                                {
                                    if (DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt() > 0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt();
                                        UbgT = DataModul.TextLese1(AAA);
                                        if (UbgT.Trim() != "")
                                        {
                                            _Modul1.Instance.sDatu = _Modul1.Instance.sDatu + " (" + UbgT.Trim() + "22)";
                                            UbgT = "";
                                        }
                                        goto IL_095e;
                                    }

                                }
                            }
                            goto IL_096e;
                        IL_095e:
                            num = 118;
                            Kont1[1] = _Modul1.Instance.sDatu;
                            goto IL_096e;
                        IL_096e: // <========== 4
                            num = 122;
                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt() != 0)
                            {
                                AAA2 = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                Kont[0] = DataModul.TextLese1(AAA2);
                                if (Kont[0] != "")
                                {
                                    Kont1[3] = " " + Kont[0].Trim();

                                }
                            }
                            goto IL_0a1f;
                        IL_0a1f: // <========== 3
                            num = 128;
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                            {
                                UbgT = ortles1(DataModul.DB_EventTable.Fields[EventFields.Ort].AsInt(), 1, (i, s) => ExportPlace(i, s, Ind1, Namen));
                            }
                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString().Trim(), "", TextCompare: false) != 0)
                            {
                                UbgT = UbgT.Trim() + " ?";
                            }
                            Kont1[5] = UbgT;
                            UbgT = "";
                            if (DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt() != 0)
                            {
                                AAA2 = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                Kont[0] = DataModul.TextLese1(AAA2);
                                if (Kont[0] != "")
                                {
                                    Kont1[6] = " " + Kont[0].Trim();

                                }
                            }
                            goto IL_0bc7;
                        IL_0bc7: // <========== 3
                            num = 142;
                            Kont[Ubg - 60] = DataModul.DB_EventTable.Fields[EventFields.Reg].AsString().Trim();
                            if (DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().TrimEnd() != "" | DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().TrimEnd() != "")
                            {
                                Kont[Ubg - 90] = "";
                                Kont[Ubg - 90] = Module2.Jobdreh(Kont[Ubg - 90]);
                                string text2 = "";
                                text2 = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().TrimEnd();
                                Kont[Ubg - 85] = text2;
                                text2 = "";
                                text2 = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().TrimEnd();
                                Kont[Ubg - 80] = text2;
                            }
                            else
                            {

                                Kont[Ubg - 90] = "";
                                Kont[Ubg - 90] = Module2.Jobdreh(Kont[Ubg - 90]);
                            }
                            goto IL_0d89;
                        IL_0d89: // <========== 3
                            num = 157;
                            Nr = PersInArb;
                            LfNR = 0;
                            QuellenDatum(ref Nr, Art, ref LfNR);
                            PersInArb = Nr.AsInt();
                            if (Kont1[9].Trim() != "")
                            {
                                Kont[Ubg - 70] = Kont1[9].Trim();
                            }

                            int persInArb = PersInArb;
                            short num12 = 1;
                            while (num12 <= 100)
                            {
                                KontSP1[num12] = Kont1[num12];
                                KontSP[num12] = Kont[num12];
                                Kont[num12] = "";
                                Kont1[num12] = "";
                                num12 = (short)unchecked(num12 + 1);
                            }

                            if (unchecked(0 - (ki ? 1 : 0)) == 0)
                            {
                                num8 = (short)DAus[96].AsInt();
                            }
                            if (unchecked(0 - (ki ? 1 : 0)) == 1)
                            {
                                num8 = (short)DAus[98].AsInt();
                            }
                            if (num8 == 1)
                            {
                                Zeugsu(Art, 0, Listart, 0L);
                            }
                            PersInArb = persInArb;
                            string text = Kont1[20];
                            num12 = 1;
                            while (num12 <= 100)
                            {
                                Kont1[num12] = KontSP1[num12];
                                Kont[num12] = KontSP[num12];
                                KontSP[num12] = "";
                                KontSP1[num12] = "";
                                num12 = (short)unchecked(num12 + 1);
                            }
                            Kont[(int)Art - 50] = text;
                            text = "";
                            goto IL_0fb7;
                        IL_0fb7: // <========== 4
                            num = 187;
                            lErl = 2;
                            num5 = unchecked(num5 + 1);
                            goto IL_0fcf;
                        IL_0fcf:
                            if (num5 > num11)
                            {
                                goto end_IL_0000_2;
                            }
                            var J = 0;
                            while (unchecked(J) <= 20u)
                            {
                                Kont1[J] = "";
                                J++;
                            }
                            Ubg = (int)num5;
                            Art = num5;
                            _Modul1.Instance.sDatu = "";
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", Ubg.AsString(), PersInArb.AsString(), "0");
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields["priv"].AsInt()))
                                {
                                    iPrivacy = DataModul.DB_EventTable.Fields["priv"].AsInt();
                                }
                                else
                                {

                                    iPrivacy = 0;
                                }
                                goto IL_020b;
                            }
                            goto IL_0fb7;
                        IL_106b:
                            num4 = unchecked(num2 + 1);
                            goto IL_106f;
                        IL_106f:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 34:
                                case 37:
                                case 38:
                                    goto IL_020b;
                                case 46:
                                case 47:
                                case 48:
                                    goto IL_02dc;
                                case 56:
                                case 57:
                                case 58:
                                    goto IL_03cb;
                                case 65:
                                case 68:
                                case 69:
                                case 70:
                                    goto IL_04e9;
                                case 74:
                                case 75:
                                case 76:
                                    goto IL_053f;
                                case 82:
                                case 83:
                                case 84:
                                case 85:
                                    goto IL_05f9;
                                case 95:
                                case 96:
                                case 97:
                                    goto IL_079f;
                                case 99:
                                case 100:
                                    goto IL_07cd;
                                case 102:
                                case 103:
                                    goto IL_07fb;
                                case 104:
                                case 105:
                                case 106:
                                case 107:
                                case 108:
                                    goto IL_084e;
                                case 117:
                                case 118:
                                    goto IL_095e;
                                case 119:
                                case 120:
                                case 121:
                                case 122:
                                    goto IL_096e;
                                case 126:
                                case 127:
                                case 128:
                                    goto IL_0a1f;
                                case 140:
                                case 141:
                                case 142:
                                    goto IL_0bc7;
                                case 152:
                                case 156:
                                case 157:
                                    goto IL_0d89;
                                case 30:
                                case 39:
                                case 187:
                                    goto IL_0fb7;
                                case 73:
                                case 189:
                                case 191:
                                case 195:
                                case 196:
                                case 202:
                                case 203:
                                case 204:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 5037;
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

    public void PerQu(ref int FamPer)
    {
        //Discarded unreachable code: IL_07ff
        int try0000_dispatch = -1;
        int num = default;
        string text = default;
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
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            text = "";
                            goto IL_0009;
                        case 2350:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0802;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number != 3022)
                                {
                                    goto end_IL_0000_2;
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0802;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            Kont[30] = "";
                            DataModul.DB_SourceLinkTable.Index = "Tab";
                            if (FamPer == 1)
                            {
                                DataModul.DB_SourceLinkTable.Seek("=", 1, PersInArb);
                                num5 = PersInArb;
                            }
                            goto IL_009c;
                        IL_009c:
                            num = 9;
                            if (FamPer == 2)
                            {
                                DataModul.DB_SourceLinkTable.Seek("=", 2, FamInArb);
                                num5 = FamInArb;
                            }
                            goto IL_0539;
                        IL_0363: // <========== 3
                            num = 30;
                            if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value))
                            {
                                if (Operators.CompareString(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    text = text.Trim() + " >" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString().Trim() + "<";
                                }
                            }
                            goto IL_03fe;
                        IL_03fe: // <========== 3
                            num = 35;
                            text = text.Trim() + ".";
                            BemZahl++;
                            Kont[30] = Kont[30] + " " + BemZahl.AsString().Trim() + ".) ";
                            KontBem[BemZahl] = BemZahl.AsString().Trim() + ".) " + text.Trim();
                            text = "";
                            DataModul.DSB_QuellIdxTable.AddNew();
                            DataModul.DSB_QuellIdxTable.Fields["Quelle"].Value = DataModul.DB_QuTable.Fields[QuFields._4].Value;
                            DataModul.DSB_QuellIdxTable.Fields["Nr"].Value = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3].Value;
                            DataModul.DSB_QuellIdxTable.Update();
                            goto IL_052b;
                        IL_052b: // <========== 3
                            num = 45;
                            DataModul.DB_SourceLinkTable.MoveNext();
                            goto IL_0539;
                        IL_0539: // <========== 3
                            num = 14;
                            if (!DataModul.DB_SourceLinkTable.EOF)
                            {
                                if (!DataModul.DB_SourceLinkTable.NoMatch)
                                {
                                    if (!(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._1].AsInt() != FamPer | DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].AsInt() != num5).AsBool())
                                    {
                                        DataModul.DB_QuTable.Index = "NR";
                                        DataModul.DB_QuTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3]);
                                        text = text + " " + DataModul.DB_QuTable.Fields[QuFields._4].AsString().Trim() + " ";
                                        if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[3].Value) & Operators.CompareString(DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            if (Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value))
                                            {
                                                text = text.Trim() + Operators.AddObject(", Seiten: ", DataModul.DB_SourceLinkTable.Fields[3].AsString());
                                            }
                                            else
                                                text = Conversions.ToString(string.Concat(text + ", ", DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim()) + " " + DataModul.DB_SourceLinkTable.Fields[3].Value);
                                        }
                                        goto IL_0363;
                                    }
                                    goto IL_054c;
                                }
                                goto IL_052b;
                            }
                            goto IL_054c;
                        IL_054c: // <========== 3
                            num = 47;
                            if (FamPer == 1)
                            {
                                PerSatzLes(_Modul1.Instance.PersInArb);
                                if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields[PersonFields.Bem3].Value) && Operators.CompareString(DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    BemZahl++;
                                    Kont[30] = Kont[30] + " " + BemZahl.AsString().Trim() + ".) ";
                                    KontBem[BemZahl] = BemZahl.AsString().Trim() + ".) " + DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString().Trim();
                                }
                                goto end_IL_0000_2;
                            }
                            DataModul.DB_FamilyTable.Index = "Fam";
                            DataModul.DB_FamilyTable.Seek("=", FamInArb);
                            if (Information.IsDBNull(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].Value))
                            {
                                goto end_IL_0000_2;
                            }
                            if (Operators.CompareString(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].AsString().Trim(), "", TextCompare: false) == 0)
                            {

                            }
                            else
                            {
                                BemZahl++;
                                KontBem[BemZahl] = BemZahl.AsString().Trim() + ".) " + DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].AsString().Trim();
                                Kont[30] = Kont[30] + BemZahl.AsString().Trim() + ".) ";
                            }
                            goto end_IL_0000_2;
                        IL_0802:
                            num4 = unchecked(num2 + 1);
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 8:
                                case 9:
                                    goto IL_009c;
                                case 33:
                                case 34:
                                case 35:
                                    goto IL_03fe;
                                case 44:
                                case 45:
                                    goto IL_052b;
                                case 12:
                                case 13:
                                case 14:
                                case 46:
                                    goto IL_0539;
                                case 17:
                                case 47:
                                    goto IL_054c;
                                case 54:
                                case 55:
                                case 56:
                                case 65:
                                case 66:
                                case 67:
                                case 68:
                                case 71:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 2350;
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
    [Obsolete]
    public string Person_FullSurname(string[] kont, bool xFamToUpper)
    {
        string sName = kont[0];
        sName = xFamToUpper ? sName.ToUpper() : sName;
        if (kont[1] != "")
        {
            sName = kont[1] + " " + sName;
        }
        if (kont[2] != "")
        {
            sName = sName + " " + kont[2];
        }
        return kont[0] = sName;
    }

    public string Person_FullSurname(IPersonData person, bool xFamToUpper)
    {
        string sName = person.SurName;
        sName = xFamToUpper ? sName.ToUpper() : sName;
        if (person.Prefix != "")
        {
            sName = person.Prefix + " " + sName;
        }
        if (person.Suffix != "")
        {
            sName = sName + " " + person.Suffix;
        }
        return sName;
    }

    public void Zeugsu(EEventArt Art, short LfNR, short Listart, long Ahne)
    {
        //Discarded unreachable code: IL_0720
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        byte b = default;
        string text = default;
        int num5 = default;
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
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 2230:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                    case 3:
                                        break;
                                    case 1:
                                        goto IL_0724;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number != 13)
                                {
                                    goto end_IL_0000_2;
                                }
                                Listart = 200;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0724;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            text = "";
                            if (Art < EEventArt.eA_499)
                            {
                                num5 = PersInArb;
                            }
                            else
                            {

                                num5 = FamInArb;
                            }
                            goto IL_0035;
                        IL_0035: // <========== 3
                            num = 9;
                            DataModul.DB_WitnessTable.Index = "FamSu";
                            DataModul.DB_WitnessTable.Seek("=", num5, "10");
                            DataModul.DB_WitnessTable.Index = "ZeugSu";
                            var eWKennz = "10";
                            DataModul.DB_WitnessTable.Seek("=", num5, eWKennz, Art, LfNR);
                            b = 1;
                            goto IL_011d;
                        IL_011d: // <========== 3
                            num = 15;
                            string text2;
                            if (!DataModul.DB_WitnessTable.EOF)
                            {
                                if (!DataModul.DB_WitnessTable.NoMatch)
                                {
                                    if (!(DataModul.DB_WitnessTable.Fields[WitnessFields.FamNr].AsInt() != num5 | DataModul.DB_WitnessTable.Fields[WitnessFields.Kennz].AsString() != "10").AsBool())
                                    {
                                        if (!(DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsEnum<EEventArt>() != Art | DataModul.DB_WitnessTable.Fields[WitnessFields.LfNr].AsInt() != LfNR).AsBool())
                                        {
                                            if (DataModul.DB_WitnessTable.NoMatch)
                                            {
                                                Interaction.MsgBox("F26");
                                            }
                                            else
                                            {

                                                text2 = DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsString() + Strings.Right("          " + DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsString(), 10);
                                                text += text2;
                                            }
                                            goto IL_029c;
                                        }
                                    }

                                }
                            }
                            goto IL_02bb;
                        IL_029c: // <========== 3
                            num = 34;
                            DataModul.DB_WitnessTable.MoveNext();
                            b = (byte)unchecked((uint)(b + 1));
                            if (unchecked(b) <= 99u)
                            {
                                goto IL_011d;
                            }
                            goto IL_02bb;
                        IL_02bb: // <========== 4
                            num = 36;
                            b2 = (byte)Math.Round(text.Length / 14.0);
                            b = 1;
                            goto IL_052a;
                        IL_0521: // <========== 4
                            num = 71;
                            b = (byte)unchecked((uint)(b + 1));
                            goto IL_052a;
                        IL_052a:
                            if (unchecked(b <= (uint)b2))
                            {
                                text2 = text.Left(14);
                                text = Strings.Mid(text, 15, text.Length);
                                PersInArb = (int)Math.Round(Strings.Mid(text2, 5, 10).AsDouble());
                                DataModul.DT_SperrTable.Index = "Nr";
                                DataModul.DT_SperrTable.Seek("=", PersInArb);
                                if (DataModul.DT_SperrTable.NoMatch)
                                {
                                    Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                    ProjectData.ClearProjectError();
                                    num3 = 3;
                                    if (Listart != 0)
                                    {
                                        switch (Listart)
                                        {
                                            case 1:
                                                MyProject.Forms.Stammfolge.Namenindex();
                                                break;
                                            case 2:
                                                Namenindex(Ahne);
                                                break;
                                            default:
                                                break;
                                        }
                                        Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                    }
                                    Person.SetFullSurname(Person_FullSurname(_Modul1.Instance.Person, _Modul1.Instance.DAus[89] == "1"));

                                    string sGivennames = DAus[76] == "1" ? " <" + PersInArb.AsString().Trim() + ">" + _Modul1.Instance.Person.Givennames : _Modul1.Instance.Person.Givennames;

                                    if (Kont1[20].Trim() != "")
                                    {
                                        Kont1[20] = Kont1[20] + "; " + Strings.Trim(_Modul1.Instance.Person.Prae.TrimEnd() + " " + sGivennames + " " + _Modul1.Instance.Person.FullSurName.Trim());
                                    }
                                    else
                                    {
                                        Kont1[20] = " " + sGivennames + " " + _Modul1.Instance.Person.FullSurName.Trim();
                                    }

                                }
                                goto IL_0521;
                            }
                            short b3 = LfNR;
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", Art, num5, b3);
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                goto end_IL_0000_2;
                            }
                            if (Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem4].Value))
                            {
                                goto end_IL_0000_2;
                            }
                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim(), "", TextCompare: false) == 0)
                            {
                                goto end_IL_0000_2;
                            }
                            if (Kont1[20].Trim() != "")
                            {
                                Kont1[20] = Kont1[20] + "; " + DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim();
                            }
                            else
                            {

                                Kont1[20] = DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim();
                            }
                            goto IL_06b0;
                        IL_06b0: // <========== 3
                            num = 84;
                            if (DAus[87] != "0")
                            {
                            }
                            else
                            {

                                Kont1[20] = Text_Retweg(Kont1[20]);
                            }
                            goto end_IL_0000_2;
                        IL_0724:
                            num4 = unchecked(num2 + 1);
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 5:
                                case 8:
                                case 9:
                                    goto IL_0035;
                                case 15:
                                    goto IL_011d;
                                case 29:
                                case 33:
                                case 34:
                                    goto IL_029c;
                                case 16:
                                case 19:
                                case 22:
                                case 25:
                                case 36:
                                    goto IL_02bb;
                                case 43:
                                case 67:
                                case 70:
                                case 71:
                                    goto IL_0521;
                                case 80:
                                case 83:
                                case 84:
                                    goto IL_06b0;
                                case 86:
                                case 87:
                                case 88:
                                case 89:
                                case 90:
                                case 94:
                                case 95:
                                case 96:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 2230;
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
    public void geoles(ref string UbgT1)
    {
        checked
        {
            short num = (short)Strings.InStr(UbgT1, ",");
            if (num == 0)
            {
                num = (short)Strings.InStr(UbgT1, ".");
            }
            UbgT = UbgT1.Left(num - 1) + "° ";
            UbgT = UbgT + Strings.Mid(UbgT1, num + 1, 2) + "' ";
            UbgT = UbgT + Strings.Mid(UbgT1, num + 3, 2) + "'' ";
            UbgT = UbgT.Trim();
        }
    }

    public void QuellenDatum(ref int Nr, EEventArt Art, ref short LfNR)
    {
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
                checked
                {
                    int num4;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            if (DAus[62] == "0")
                            {
                                goto end_IL_0000;
                            }
                            goto IL_001d;
                        case 2149:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_075d;
                                    default:
                                        goto end_IL_0000_2;
                                }
                                goto IL_06ec;
                            }
                        IL_06ec:
                            num = 54;
                            if (Information.Err().Number == 3022)
                            {
                                goto IL_0700;
                            }
                            goto IL_0718;
                        IL_0718:
                            num = 58;
                            if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            goto IL_073d;
                        IL_04b2:
                            num = 33;
                            KontBem[BemZahl] = KontBem[BemZahl].Trim() + " >" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString().Trim() + "<";
                            goto IL_0502;
                        IL_073d:
                            num = 61;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_0760;
                        IL_0502:
                            num = 36;
                            KontBem[BemZahl] = KontBem[BemZahl].Trim() + ". ";
                            goto IL_052a;
                        IL_052a:
                            num = 39;
                            DataModul.DB_SourceLinkTable.MoveNext();
                            goto IL_0537;
                        IL_0760:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 3:
                                case 4:
                                    goto IL_001d;
                                case 5:
                                    goto IL_0024;
                                case 6:
                                    goto IL_0035;
                                case 9:
                                    goto IL_0093;
                                case 10:
                                    goto IL_00a5;
                                case 12:
                                case 13:
                                    goto IL_0152;
                                case 14:
                                    goto IL_0164;
                                case 15:
                                    goto IL_01c6;
                                case 16:
                                    goto IL_01d8;
                                case 17:
                                    goto IL_01e7;
                                case 18:
                                    goto IL_0218;
                                case 19:
                                    goto IL_0271;
                                case 20:
                                    goto IL_027e;
                                case 21:
                                    goto IL_02b8;
                                case 22:
                                    goto IL_02f2;
                                case 23:
                                    goto IL_0301;
                                case 24:
                                    goto IL_0366;
                                case 25:
                                    goto IL_038e;
                                case 27:
                                    goto IL_03d7;
                                case 28:
                                    goto IL_03da;
                                case 26:
                                case 29:
                                case 30:
                                case 31:
                                    goto IL_0453;
                                case 32:
                                    goto IL_047e;
                                case 33:
                                    goto IL_04b2;
                                case 34:
                                case 35:
                                case 36:
                                    goto IL_0502;
                                case 37:
                                case 38:
                                case 39:
                                    goto IL_052a;
                                case 7:
                                case 8:
                                case 40:
                                    goto IL_0537;
                                case 11:
                                case 41:
                                    goto IL_0548;
                                case 42:
                                    goto IL_0557;
                                case 43:
                                case 44:
                                    goto IL_0560;
                                case 45:
                                    goto IL_0572;
                                case 46:
                                    goto IL_05d5;
                                case 47:
                                    goto IL_0600;
                                case 48:
                                    goto IL_0637;
                                case 49:
                                    goto IL_0646;
                                case 50:
                                    goto IL_06b6;
                                case 54:
                                    goto IL_06ec;
                                case 55:
                                    goto IL_0700;
                                case 56:
                                case 58:
                                    goto IL_0718;
                                case 59:
                                case 61:
                                    goto IL_073d;
                                default:
                                    goto end_IL_0000_2;
                                case 2:
                                case 51:
                                case 52:
                                case 53:
                                case 62:
                                    goto end_IL_0000;
                            }
                            goto default;
                        IL_0700:
                            num = 55;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_075d;
                        IL_047e:
                            num = 32;
                            if (Operators.CompareString(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString().Trim(), "", TextCompare: false) != 0)
                            {
                                goto IL_04b2;
                            }
                            goto IL_0502;
                        IL_075d:
                            num4 = unchecked(num2 + 1);
                            goto IL_0760;
                        IL_001d:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0024;
                        IL_0024:
                            num = 5;
                            DataModul.DB_SourceLinkTable.Index = "Tab22";
                            goto IL_0035;
                        IL_0035:
                            num = 6;
                            DataModul.DB_SourceLinkTable.Seek("=", 3, Nr, Art, LfNR);
                            goto IL_0537;
                        IL_0537:
                            num = 8;
                            if (!DataModul.DB_SourceLinkTable.EOF)
                            {
                                goto IL_0093;
                            }
                            goto IL_0548;
                        IL_0093:
                            num = 9;
                            if (!DataModul.DB_SourceLinkTable.NoMatch)
                            {
                                goto IL_00a5;
                            }
                            goto IL_052a;
                        IL_00a5:
                            num = 10;
                            if (!Conversions.ToBoolean(DataModul.DB_SourceLinkTable.Fields[0].AsInt() != 3
                                | DataModul.DB_SourceLinkTable.Fields[1].AsInt() > Nr
                                | DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsEnum<EEventArt>() != Art
                                | DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.LfNr].AsInt() != LfNR))
                            {
                                goto IL_0152;
                            }
                            goto IL_0548;
                        IL_0548:
                            num = 41;
                            if (BemZahl > 999)
                            {
                                goto IL_0557;
                            }
                            goto IL_0560;
                        IL_0557:
                            num = 42;
                            BemZahl = 0;
                            goto IL_0560;
                        IL_0560:
                            num = 44;
                            DataModul.DB_EventTable.Index = "ArtNr";
                            goto IL_0572;
                        IL_0572:
                            num = 45;
                            DataModul.DB_EventTable.Seek("=", Art.AsString(), Nr.AsString(), LfNR);
                            goto IL_05d5;
                        IL_05d5:
                            num = 46;
                            if (Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem3].Value))
                            {
                                goto end_IL_0000;
                            }
                            goto IL_0600;
                        IL_0600:
                            num = 47;
                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim(), "", TextCompare: false) == 0)
                            {
                                goto end_IL_0000;
                            }
                            goto IL_0637;
                        IL_0637:
                            num = 48;
                            BemZahl++;
                            goto IL_0646;
                        IL_0646:
                            num = 49;
                            KontBem[BemZahl] = " " + BemZahl.AsString().Trim() + ".) " + DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim() + ". ";
                            goto IL_06b6;
                        IL_06b6:
                            num = 50;
                            Kont1[9] = Kont1[9] + BemZahl.AsString().Trim() + ".) ";
                            goto end_IL_0000;
                        IL_0152:
                            num = 13;
                            DataModul.DB_QuTable.Index = "NR";
                            goto IL_0164;
                        IL_0164:
                            num = 14;
                            DataModul.DB_QuTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3]);
                            goto IL_01c6;
                        IL_01c6:
                            num = 15;
                            if (!DataModul.DB_QuTable.NoMatch)
                            {
                                goto IL_01d8;
                            }
                            goto IL_052a;
                        IL_01d8:
                            num = 16;
                            BemZahl++;
                            goto IL_01e7;
                        IL_01e7:
                            num = 17;
                            Kont1[9] = Kont1[9] + BemZahl.AsString().Trim() + ".) ";
                            goto IL_0218;
                        IL_0218:
                            num = 18;
                            KontBem[BemZahl] = Conversions.ToString(Operators.ConcatenateObject(" " + BemZahl.AsString().Trim(), Operators.AddObject(".) ", DataModul.DB_QuTable.Fields[QuFields._4].Value)));
                            goto IL_0271;
                        IL_0271:
                            num = 19;
                            DataModul.DSB_QuellIdxTable.AddNew();
                            goto IL_027e;
                        IL_027e:
                            num = 20;
                            DataModul.DSB_QuellIdxTable.Fields["Quelle"].Value = DataModul.DB_QuTable.Fields[QuFields._4].Value;
                            goto IL_02b8;
                        IL_02b8:
                            num = 21;
                            DataModul.DSB_QuellIdxTable.Fields["Nr"].Value = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3].Value;
                            goto IL_02f2;
                        IL_02f2:
                            num = 22;
                            DataModul.DSB_QuellIdxTable.Update();
                            goto IL_0301;
                        IL_0301:
                            num = 23;
                            if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[3].Value) & Operators.CompareString(DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim(), "", TextCompare: false) != 0)
                            {
                                goto IL_0366;
                            }
                            goto IL_0453;
                        IL_0366:
                            num = 24;
                            if (Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value))
                            {
                                goto IL_038e;
                            }
                            goto IL_03d7;
                        IL_038e:
                            num = 25;
                            KontBem[BemZahl] = (KontBem[BemZahl] + Operators.AddObject(", Seiten: ", DataModul.DB_SourceLinkTable.Fields[3].Value)).AsString();
                            goto IL_0453;
                        IL_03d7:
                            num = 27;
                            goto IL_03da;
                        IL_03da:
                            num = 28;
                            KontBem[BemZahl] = Conversions.ToString(string.Concat(KontBem[BemZahl] + ", ", DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim()) + " " + DataModul.DB_SourceLinkTable.Fields[3].Value);
                            goto IL_0453;
                        IL_0453:
                            num = 31;
                            if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value))
                            {
                                goto IL_047e;
                            }
                            goto IL_0502;
                        end_IL_0000_2:
                            break;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 2149;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    public void Namenindex(long Ahne)
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
                    case 683:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0217;
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
                            goto IL_021a;
                        }
                    end_IL_0000:
                        break;
                    IL_0007:
                        num = 2;
                        if (Kont[4] != "")
                        {
                            Alias_Renamed(Kont[4]);
                        }
                        if (Kont[0] == "" || Kont[0] == "NN" || Kont[0] == "N.N.")
                        {
                            goto end_IL_0000_2;
                        }
                        DataModul.DSB_NamIdxTable.AddNew();
                        if (Kont[99] == "")
                        {
                            DataModul.DSB_NamIdxTable.Fields["Name"].Value = "?";
                        }
                        else
                            DataModul.DSB_NamIdxTable.Fields["Name"].Value = Strings.Trim(Kont[99].Left(DataModul.DSB_NamIdxTable.Fields["Name"].Size));

                        DataModul.DSB_NamIdxTable.Fields["Name1"].Value = Strings.Trim(Kont[0].Left(DataModul.DSB_NamIdxTable.Fields["Name"].Size));
                        Namen = Kont[0];
                        DataModul.DSB_NamIdxTable.Fields["Nr"].Value = Ahne;
                        Ind1 = $"{Ahne,20}";
                        if (Strings.InStr(Ind1, "?") != 0)
                        {
                            Debugger.Break();
                        }
                        DataModul.DSB_NamIdxTable.Update();
                        goto end_IL_0000_2;
                    IL_0217:
                        num4 = num2 + 1;
                        goto IL_021a;
                    IL_021a:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 7:
                            case 10:
                            case 27:
                            case 28:
                            case 33:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 683;
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
    public void Alias_Renamed(string sAlias)
    {
        short num = 1;
        while (num > 0)
        {
            num = (short)Strings.InStr(sAlias, ";");
            string text;
            if (num > 0)
            {
                text = sAlias.Left(num - 1);
                sAlias = Strings.Mid(sAlias, num + 1, sAlias.Length);
            }
            else
            {
                text = sAlias;
            }
            DataModul.DSB_NamIdxTable.AddNew();
            DataModul.DSB_NamIdxTable.Fields["Nr"].Value = "";
            DataModul.DSB_NamIdxTable.Fields["Name"].Value = "";
            DataModul.DSB_NamIdxTable.Fields["Name1"].Value = Strings.Trim((text + " siehe " + sAlias.Trim()).Left(DataModul.DSB_NamIdxTable.Fields["Name1"].Size));
            DataModul.DSB_NamIdxTable.Update();
        }
        sAlias = sAlias.Trim();
    }

    public void SLQuellenDatum(ref int Nr, EEventArt Art, ref short LfNR)
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
                    case 1934:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_069a;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_0629;
                        }
                    IL_0629:
                        num = 49;
                        if (Information.Err().Number == 3022)
                        {
                            goto IL_063d;
                        }
                        goto IL_0655;
                    IL_0655:
                        num = 53;
                        if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                        {
                            ProjectData.EndApp();
                        }
                        goto IL_067a;
                    IL_044f:
                        num = 29;
                        DataModul.DSB_QuellIdxTable.Fields["Nr"].Value = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3].Value;
                        goto IL_0489;
                    IL_067a:
                        num = 56;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        num4 = num2;
                        goto IL_069d;
                    IL_0489:
                        num = 30;
                        DataModul.DSB_QuellIdxTable.Update();
                        goto IL_0498;
                    IL_0498:
                        num = 33;
                        DataModul.DB_SourceLinkTable.MoveNext();
                        goto IL_04a5;
                    IL_069d:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_0007;
                            case 3:
                                goto IL_0018;
                            case 6:
                                goto IL_0076;
                            case 7:
                                goto IL_0087;
                            case 9:
                            case 10:
                                goto IL_013d;
                            case 11:
                                goto IL_014f;
                            case 12:
                                goto IL_01bb;
                            case 13:
                                goto IL_01cd;
                            case 14:
                                goto IL_0212;
                            case 15:
                                goto IL_0277;
                            case 16:
                                goto IL_029f;
                            case 18:
                                goto IL_02e7;
                            case 19:
                                goto IL_02ea;
                            case 17:
                            case 20:
                            case 21:
                            case 22:
                                goto IL_0362;
                            case 23:
                                goto IL_038a;
                            case 24:
                                goto IL_03be;
                            case 25:
                            case 26:
                            case 27:
                                goto IL_0408;
                            case 28:
                                goto IL_0415;
                            case 29:
                                goto IL_044f;
                            case 30:
                                goto IL_0489;
                            case 31:
                            case 32:
                            case 33:
                                goto IL_0498;
                            case 4:
                            case 5:
                            case 34:
                                goto IL_04a5;
                            case 8:
                            case 35:
                                goto IL_04b6;
                            case 36:
                                goto IL_04c8;
                            case 37:
                                goto IL_052b;
                            case 38:
                                goto IL_053d;
                            case 39:
                                goto IL_0568;
                            case 40:
                                goto IL_059f;
                            case 41:
                                goto IL_05c5;
                            case 42:
                                goto IL_05e6;
                            case 43:
                            case 44:
                                goto IL_05f3;
                            case 49:
                                goto IL_0629;
                            case 50:
                                goto IL_063d;
                            case 51:
                            case 53:
                                goto IL_0655;
                            case 54:
                            case 56:
                                goto IL_067a;
                            default:
                                goto end_IL_0000;
                            case 45:
                            case 46:
                            case 47:
                            case 48:
                            case 57:
                                goto end_IL_0000_2;
                        }
                        goto default;
                    IL_063d:
                        num = 50;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_069a;
                    IL_0415:
                        num = 28;
                        DataModul.DSB_QuellIdxTable.Fields["Quelle"].Value = DataModul.DB_QuTable.Fields[QuFields._4].Value;
                        goto IL_044f;
                    IL_069a:
                        num4 = num2 + 1;
                        goto IL_069d;
                    IL_0007:
                        num = 2;
                        DataModul.DB_SourceLinkTable.Index = "Tab22";
                        goto IL_0018;
                    IL_0018:
                        num = 3;
                        DataModul.DB_SourceLinkTable.Seek("=", 3, Nr, Art, LfNR);
                        goto IL_04a5;
                    IL_04a5:
                        num = 5;
                        if (!DataModul.DB_SourceLinkTable.EOF)
                        {
                            goto IL_0076;
                        }
                        goto IL_04b6;
                    IL_0076:
                        num = 6;
                        if (!DataModul.DB_SourceLinkTable.NoMatch)
                        {
                            goto IL_0087;
                        }
                        goto IL_0498;
                    IL_0087:
                        num = 7;
                        if (!Conversions.ToBoolean(DataModul.DB_SourceLinkTable.Fields[0].AsInt() != 3
                            | DataModul.DB_SourceLinkTable.Fields[1].AsInt() > Nr
                            | DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsEnum<EEventArt>() != Art
                            | DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.LfNr].AsInt() != LfNR))
                        {
                            goto IL_013d;
                        }
                        goto IL_04b6;
                    IL_04b6:
                        num = 35;
                        DataModul.DB_EventTable.Index = "ArtNr";
                        goto IL_04c8;
                    IL_04c8:
                        num = 36;
                        DataModul.DB_EventTable.Seek("=", Art.AsString(), Nr.AsString(), LfNR);
                        goto IL_052b;
                    IL_052b:
                        num = 37;
                        if (DataModul.DB_EventTable.NoMatch)
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_053d;
                    IL_053d:
                        num = 38;
                        if (Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString()))
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_0568;
                    IL_0568:
                        num = 39;
                        if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim(), "", TextCompare: false) == 0)
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_059f;
                    IL_059f:
                        num = 40;
                        UbgT = DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString();
                        goto IL_05c5;
                    IL_05c5:
                        num = 41;
                        if (MyProject.Forms.Ausw.Kontroll[12].CheckState == CheckState.Unchecked)
                        {
                            goto IL_05e6;
                        }
                        goto IL_05f3;
                    IL_05e6:
                        num = 42;
                        UbgT = Text_Retweg(UbgT);
                        goto IL_05f3;
                    IL_05f3:
                        num = 44;
                        Kont1[9] = Kont1[9].Trim() + " [" + UbgT.Trim() + ".] ";
                        goto end_IL_0000_2;
                    IL_013d:
                        num = 10;
                        DataModul.DB_QuTable.Index = "NR";
                        goto IL_014f;
                    IL_014f:
                        num = 11;
                        DataModul.DB_QuTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3].AsInt());
                        goto IL_01bb;
                    IL_01bb:
                        num = 12;
                        if (!DataModul.DB_QuTable.NoMatch)
                        {
                            goto IL_01cd;
                        }
                        goto IL_0498;
                    IL_01cd:
                        num = 13;
                        Kont1[9] = (Kont1[9].Trim() + " " + DataModul.DB_QuTable.Fields[QuFields._2].AsInt()).AsString();
                        goto IL_0212;
                    IL_0212:
                        num = 14;
                        if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[3].AsInt()) & (DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim()!= ""))
                        {
                            goto IL_0277;
                        }
                        goto IL_0362;
                    IL_0277:
                        num = 15;
                        if (Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsInt()))
                        {
                            goto IL_029f;
                        }
                        goto IL_02e7;
                    IL_029f:
                        num = 16;
                        Kont1[9] = $"{Kont1[9].Trim()}, Seiten: {DataModul.DB_SourceLinkTable.Fields[3].AsInt()}";
                        goto IL_0362;
                    IL_02e7:
                        num = 18;
                        goto IL_02ea;
                    IL_02ea:
                        num = 19;
                        Kont1[9] = $"{Kont1[9].Trim()}, { DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim()} {DataModul.DB_SourceLinkTable.Fields[3].AsInt()}";
                        goto IL_0362;
                    IL_0362:
                        num = 22;
                        if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value))
                        {
                            goto IL_038a;
                        }
                        goto IL_0408;
                    IL_038a:
                        num = 23;
                        if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString().Trim()!= "" )
                        {
                            goto IL_03be;
                        }
                        goto IL_0408;
                    IL_03be:
                        num = 24;
                        Kont1[9] = Kont1[9].Trim() + " >" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString().Trim() + "<";
                        goto IL_0408;
                    IL_0408:
                        num = 27;
                        DataModul.DSB_QuellIdxTable.AddNew();
                        goto IL_0415;
                    end_IL_0000:
                        break;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 1934;
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

    public void Zeuge_Bei(int PersInArb, ref short Listart)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        short num5 = default;
        string text = default;
        string text2 = default;
        short num8 = default;
        int lErl = default;
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
                    int AAA;
                    string LD;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 6871:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_16a1;
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
                                goto IL_16a5;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            text = "";
                            text2 = "";
                            DataModul.DB_WitnessTable.Index = "ElSu";
                            DataModul.DB_WitnessTable.Seek("=", PersInArb, "10");
                            text = "";
                            MyProject.Forms.Hinter.List3.Items.Clear();
                            short num6 = 1;
                            text2 = "";
                            while (num6 <= 99
                                && !DataModul.DB_WitnessTable.EOF
                                && !DataModul.DB_WitnessTable.NoMatch
                                && !Conversions.ToBoolean(DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsInt() != PersInArb
                              | DataModul.DB_WitnessTable.Fields[WitnessFields.Kennz].AsInt() != 10))
                            {
                                text2 = DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsInt().AsString() + DataModul.DB_WitnessTable.Fields[WitnessFields.LfNr].AsInt().AsString() + Strings.Right("          " + DataModul.DB_WitnessTable.Fields[WitnessFields.FamNr].AsInt().AsString(), 10);
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsInt().AsString(), DataModul.DB_WitnessTable.Fields[WitnessFields.FamNr], DataModul.DB_WitnessTable.Fields[WitnessFields.LfNr]);
                                string text3;
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    text3 = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                    if (text3.AsDouble() == 0.0)
                                    {
                                        text3 = "        ";
                                    }
                                }
                                else
                                    text3 = "        ";
                                text2 = text3 + text2;
                                MyProject.Forms.Hinter.List3.Items.Add(new ListItem(text2, 0));
                                text2 = "";
                                DataModul.DB_WitnessTable.MoveNext();
                                num6 = (short)unchecked(num6 + 1);
                            }

                            short num7 = (short)(MyProject.Forms.Hinter.List3.Items.Count - 1);
                            num6 = 0;
                            while (num6 <= num7)
                            {
                                text += Strings.Mid(MyProject.Forms.Hinter.List3.Items[num6].AsString(), 9, 16);
                                num6 = (short)unchecked(num6 + 1);
                            }
                            num8 = (short)Math.Round(text.Length / 16.0);
                            num5 = 1;
                            goto IL_1644;
                        IL_0914: // <========== 3
                            num = 101;
                            lErl = 22;
                            string text5 = "";
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                    string ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                    sDatu = Datwand1(sDatu, ds);
                                    text5 = sDatu + " ";
                                }
                                if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                                {
                                    sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                    string ds2 = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                    sDatu = Datwand1(sDatu, ds2);
                                    text5 = text5 + sDatu + " ";
                                }
                            }

                            string text6 = "";
                            string left = "";
                            string selectedText = "";
                            selectedText = "Zeuge:";
                            string left2 = text2.Left(4);
                            if (left2 == " 101")
                            {
                                left = " (" + text5 + "bei der Geburt)";
                                text6 = text5 + "bei der Geburt von ";
                            }
                            else
                            if (left2 == " 102")
                            {
                                left = " (" + text5 + "bei der Taufe)";
                                text6 = text5 + "bei der Taufe von ";
                            }
                            else
                            if (left2 == " 103")
                            {
                                left = " (" + text5 + "beim Tod)";
                                text6 = text5 + "beim Tod von ";
                            }
                            else
                            if (left2 == " 104")
                            {
                                left = " (" + text5 + "beim Begräbnis)";
                                text6 = text5 + "beim Begräbnis von ";
                            }
                            else
                            if (left2 == " 105")
                            {
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", text2.Left(4), PersInArb, Strings.Mid(text2, 5, 2).AsDouble());
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                                    LD = "";
                                    DataModul.Textlese(AAA, out var sArtText, out LD);
                                    if (sArtText != "")
                                    {
                                        left = " (" + text5 + " bei " + sArtText.Trim() + ")";
                                        text6 = text5 + " bei " + sArtText.Trim() + " von ";
                                    }
                                    else
                                    {
                                        left = " (" + text5 + "bei der Sonst.Datum)";
                                        text6 = text5 + "beim Sonst. Datum von ";
                                    }
                                }
                            }
                            else
                            if (left2 == " 106")
                            {
                                left = " (" + text5 + "beim Heimatort)";
                                text6 = text5 + "beim Heimatort von ";
                            }
                            else
                            if (left2 == " 107") ;
                            else

                                if (left2 == " 300")
                            {
                                left = " (" + text5 + "beim Beruf)";
                                text6 = text5 + "beim Beruf von ";
                            }
                            else
                            if (left2 == " 301")
                            {
                                left = " (" + text5 + "beim Titel)";
                                text6 = text5 + "beim Titel von ";
                            }
                            else
                            if (left2 == " 302")
                            {
                                left = " (" + text5 + "beim Wohnort)";
                                text6 = text5 + "beim Wohnort von ";
                            }
                            else
                            if (left2 == " 500")
                            {
                                left = " (" + text5 + "bei der Proklamation)";
                                text6 = text5 + "bei bei der Proklamation von ";
                            }
                            else
                            if (left2 == " 501")
                            {
                                left = " (" + text5 + "bei der Verlobung)";
                                text6 = text5 + "bei der Verlobung von ";
                            }
                            else
                            if (left2 == " 502")
                            {
                                left = " (" + text5 + "bei der Heirat)";
                                text6 = text5 + "bei der Heirat von ";
                                selectedText = "Trauzeuge:";
                            }
                            else
                            if (left2 == " 503")
                            {
                                left = " (" + text5 + "bei der Kirchl. Heir.)";
                                text6 = text5 + "bei der Kirchl. Heir. von ";
                                selectedText = "Trauzeuge:";
                            }
                            else
                            if (left2 == " 504")
                            {
                                left = " (" + text5 + "bei der Scheidung)";
                                text6 = text5 + "bei der Scheidung von ";
                            }
                            else
                            if (left2 == " 505")
                            {
                                left = " (" + text5 + "bei der Eheänl. Beziehung)";
                                text6 = text5 + "bei der Eheänl. Beziehung von ";
                            }
                            else
                            if (left2 == " 506")
                            {
                                left = " (" + text5 + "Eheänl. Beziehung)";
                            }
                            else
                            if (left2 == " 507")
                            {
                                left = " (" + text5 + "Dimissoriale)";
                                text6 = text5 + "bei der Dimissoriale von ";
                            }
                            else
                            if (left2 == " 602")
                            {
                                left = " (" + text5 + "bei der Wohnung)";
                                text6 = text5 + "beim Wohnungseintag von ";
                            }
                            else
                            if (left2 == " 603")
                            {
                                if (Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText]))
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                                    LD = "";
                                    DataModul.Textlese(AAA, out var sArtText, out LD);
                                    if (sArtText != "")
                                    {
                                        left = " (" + text5 + "bei " + sArtText.Trim() + ")";
                                        text6 = text5 + "bei " + sArtText.Trim() + "von ";
                                    }
                                    else
                                    {
                                        left = " (" + text5 + "beim Sonst.Datum)";
                                        text6 = text5 + "beim Sonst. Datum von ";
                                    }
                                }
                                if (text6 == "")
                                    goto IL_162b;
                            }
                            if (text4 != "")
                            {
                                switch (Listart)
                                {
                                    case 0:
                                        MyProject.Forms.Ahnen.Anz[0].SelectedText = "\n";
                                        MyProject.Forms.Ahnen.Anz[0].SelectionFont = new Font(DAus[101], (float)DAus[102].AsDouble(), FontStyle.Underline);
                                        MyProject.Forms.Ahnen.Anz[0].SelectedText = selectedText;
                                        MyProject.Forms.Ahnen.Anz[0].SelectionFont = new Font(DAus[101], (float)DAus[102].AsDouble(), FontStyle.Regular);
                                        MyProject.Forms.Ahnen.Anz[0].SelectedText = " " + text6 + text4;
                                        MyProject.Forms.Ahnen.Anz[0].SelectionFont = new Font(DAus[101], (float)DAus[102].AsDouble(), FontStyle.Regular);
                                        break;
                                    case 2:
                                        MyProject.Forms.FaBu.Retweg2(MyProject.Forms.FaBu.Anz[0]);
                                        MyProject.Forms.FaBu.Anz[0].SelectedText = " ";
                                        MyProject.Forms.FaBu.Anz[0].SelectionFont = new Font(DAus[101], (float)DAus[102].AsDouble(), FontStyle.Underline);
                                        MyProject.Forms.FaBu.Anz[0].SelectedText = selectedText;
                                        selectedText = "";
                                        MyProject.Forms.FaBu.Anz[0].SelectionFont = new Font(DAus[101], (float)DAus[102].AsDouble(), FontStyle.Regular);
                                        MyProject.Forms.FaBu.Anz[0].SelectedText = " " + text6 + text4;
                                        MyProject.Forms.FaBu.Anz[0].SelectionFont = new Font(DAus[101], (float)DAus[102].AsDouble(), FontStyle.Regular);
                                        break;
                                    default:
                                        break;
                                }
                                goto IL_162b;
                            }
                            else
                            {
                                goto end_IL_0000_2;
                            }
                        IL_162b: // <========== 5
                            num = 258;
                            lErl = 25;
                            num5 = (short)unchecked(num5 + 1);
                            goto IL_1644;
                            string str2;
                            string str;
                        IL_1644:
                            if (num5 > num8)
                            {
                                goto end_IL_0000_2;
                            }
                            else
                            {
                                text2 = text.Left(16);
                                text = Strings.Mid(text, 17, text.Length);
                                if (text2.Left(4).AsDouble() > 499.0)
                                {
                                    FamInArb = (int)Math.Round(Strings.Mid(text2, 7, 10).AsDouble());
                                    Family_Les(FamInArb, Family);
                                    str2 = "";
                                    if (Family.Mann > 0)
                                    {
                                        PersInArb = Family.Mann;
                                    }
                                    str = "";
                                    if (Family.Frau > 0)
                                    {
                                        PersInArb = Family.Frau;
                                        lErl = 2;
                                    }
                                    if (str2.Trim() != "" & str.Trim() != "")
                                    {
                                        text4 = str2.Trim() + " und " + str.Trim();
                                    }
                                    else
                                    {
                                        text4 = (str2.Trim() + " " + str.Trim()).Trim();
                                    }
                                    DataModul.DB_EventTable.Index = "ArtNr";
                                    DataModul.DB_EventTable.Seek("=", text2.Left(4), FamInArb, Strings.Mid(text2, 5, 2).AsDouble());
                                }
                                else
                                {
                                    PersInArb = (int)Math.Round(Strings.Mid(text2, 7, 10).AsDouble());
                                    Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                    Person.SetFullSurname(Person_FullSurname(_Modul1.Instance.Person, _Modul1.Instance.DAus[89] == "1"));
                                    if (DAus[76] == "1")
                                    {
                                        text4 = " <" + PersInArb.AsString().Trim() + "> " + Strings.Trim(_Modul1.Instance.Person.Prae.TrimEnd() + " " + (_Modul1.Instance.Person.Givennames + " " + _Modul1.Instance.Person.FullSurName).Trim());
                                    }
                                    else
                                    {
                                        text4 = Strings.Trim(_Modul1.Instance.Person.Prae.TrimEnd() + " " + (_Modul1.Instance.Person.Givennames + " " + _Modul1.Instance.Person.FullSurName).Trim());
                                    }
                                    DataModul.DB_EventTable.Index = "ArtNr";
                                    DataModul.DB_EventTable.Seek("=", text2.Left(4), PersInArb, Strings.Mid(text2, 5, 2).AsDouble());
                                }
                                goto IL_0914;
                            }
                        IL_16a1:
                            num4 = unchecked(num2 + 1);
                            goto IL_16a5;
                        IL_16a5:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 54:
                                    num = 54;
                                    Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                    Person.SetFullSurname(Person_FullSurname(_Modul1.Instance.Person, _Modul1.Instance.DAus[89] == "1"));
                                    if (DAus[76] == "1")
                                    {
                                        str2 = " <" + PersInArb.AsString().Trim() + "> " + (_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames + " " + _Modul1.Instance.Person.FullSurName).Trim();
                                    }
                                    else
                                        str2 = (_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames + " " + _Modul1.Instance.Person.FullSurName).Trim();
                                    break;//  goto IL_057b;
                                case 68:
                                    num = 68;
                                    Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                    Person.SetFullSurname(Person_FullSurname(_Modul1.Instance.Person, _Modul1.Instance.DAus[89] == "1"));
                                    if (DAus[76] == "1")
                                    {
                                        str = " <" + PersInArb.AsString().Trim() + "> " + (_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames + " " + _Modul1.Instance.Person.FullSurName).Trim();
                                    }
                                    else
                                        str = (_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames + " " + _Modul1.Instance.Person.FullSurName).Trim();
                                    break;// goto IL_069e;
                                case 86:
                                case 101:
                                    goto IL_0914;
                                case 225:
                                case 228:
                                case 232:
                                case 244:
                                case 257:
                                case 258:
                                    goto IL_162b;
                                case 236:
                                case 247:
                                case 260:
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
                try0000_dispatch = 6871;
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
    public void FWohn(EEventArt Art, ref short Listart)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        object CounterResult = default;
        object LoopForResult = default;
        string text = default;
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
                    case 2021:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_066f;
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
                            goto IL_0673;
                        }
                    end_IL_0000:
                        break;
                    IL_0008:
                        num = 2;
                        if (Listart == 0)
                        {
                            MyProject.Forms.Ahnen.List3.Items.Clear();
                            goto IL_0079;
                        }
                        if (Listart == 2)
                        {
                            MyProject.Forms.FaBu.List3.Items.Clear();
                            goto IL_0079;
                        }
                        if (Listart == 3)
                        {
                            MyProject.Forms.Anzeige.List3.Items.Clear();
                        }
                        goto IL_0079;
                    IL_0079: // <========== 4
                        num = 11;
                        DataModul.DB_EventTable.Index = "Besu";
                        DataModul.DB_EventTable.Seek("=", Art.AsString(), FamInArb.AsString());
                        if (DataModul.DB_EventTable.NoMatch)
                        {
                            DataModul.DB_EventTable.Index = "ArtNr";
                            goto end_IL_0000_2;
                        }
                        Ja = 1;
                        if (!ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 1, 70, 1, ref LoopForResult, ref CounterResult))
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_0148;
                    IL_0148: // <========== 3
                        num = 19;
                        if (DataModul.DB_EventTable.EOF)
                        {
                            goto end_IL_0000_2;
                        }
                        if (DataModul.DB_EventTable.NoMatch)
                        {
                            DataModul.DB_EventTable.Index = "ArtNr";
                            goto end_IL_0000_2;
                        }
                        if (!(DataModul.DB_EventTable.Fields[EventFields.LfNr].AsInt()< 0))
                        {
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields["priv"].AsInt()))
                            {
                                iPrivacy = DataModul.DB_EventTable.Fields["priv"].AsInt();
                            }
                            else
                                iPrivacy = 0;
                            goto IL_021a;
                        }
                        goto IL_05f5;
                    IL_021a: // <========== 3
                        num = 35;
                        if (!(iPrivacy.AsDouble() > iPriv_aus.AsDouble()))
                        {
                            var J = 0;
                            while (J <= 15u)
                            {
                                Kont1[J] = "";
                                J++;
                            }
                            sDatu = "";
                            Kont1[1] = "";
                            if ((DataModul.DB_EventTable.NoMatch | DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != FamInArb | DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != Art).AsBool())
                            {
                                DataModul.DB_EventTable.Index = "ArtNr";
                                goto end_IL_0000_2;
                            }
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                            {
                                sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                Kont1[1] = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                            }
                            UbgT = "";
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value))
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.ArtText].AsDouble() > 0.0)
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                                    LD = "";
                                    DataModul.Textlese(AAA, out string sArtText, out LD);
                                    if (sArtText != "")
                                    {
                                        Kont1[7] = " " + sArtText.Trim() + ": ";
                                    }
                                }
                            }
                            goto IL_0492;
                        }
                        goto IL_05f5;
                    IL_0492: // <========== 3
                        num = 60;
                        text = Kont1[1] + Kont1[7] + new string(' ', 240).Left(240) + DataModul.DB_EventTable.Fields[EventFields.LfNr].AsString();
                        if (Art == EEventArt.eA_602)
                        {
                            if (DataModul.DB_EventTable.Fields[EventFields.Reg].AsString() != " ")
                            {
                                text = "+" + text;
                            }
                        }
                        goto IL_052e;
                    IL_052e: // <========== 3
                        num = 66;
                        if (Listart == 0)
                        {
                            if (text.Trim() != "")
                            {
                                MyProject.Forms.Ahnen.List3.Items.Add(text);
                            }
                            goto IL_05f5;
                        }
                        if (Listart == 2)
                        {
                            if (text.Trim() != "")
                            {
                                MyProject.Forms.FaBu.List3.Items.Add(text);
                            }
                            goto IL_05f5;
                        }
                        if (Listart == 3)
                        {
                            if (text.Trim() != "")
                            {
                                MyProject.Forms.Anzeige.List3.Items.Add(text);
                            }
                        }
                        goto IL_05f5;
                    IL_05f5: // <========== 7
                        num = 81;
                        lErl = 299;
                        DataModul.DB_EventTable.MoveNext();
                        if (!ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult, ref CounterResult))
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_0148;
                    IL_066f:
                        num4 = num2 + 1;
                        goto IL_0673;
                    IL_0673:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 4:
                            case 7:
                            case 10:
                            case 11:
                                goto IL_0079;
                            case 19:
                                goto IL_0148;
                            case 31:
                            case 34:
                            case 35:
                                goto IL_021a;
                            case 57:
                            case 58:
                            case 59:
                            case 60:
                                goto IL_0492;
                            case 64:
                            case 65:
                            case 66:
                                goto IL_052e;
                            case 27:
                            case 36:
                            case 69:
                            case 70:
                            case 74:
                            case 75:
                            case 79:
                            case 80:
                            case 81:
                                goto IL_05f5;
                            case 15:
                            case 20:
                            case 24:
                            case 45:
                            case 84:
                            case 89:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 2021;
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
    public void Zeugsu1(ref EEventArt Art, ref byte LfNR, ref short Listart, long Ahne)
    {
        //Discarded unreachable code: IL_06cf
        int try0000_dispatch = -1;
        int num = default;
        string text = default;
        int num2 = default;
        int num3 = default;
        byte b = default;
        int num5 = default;
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
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            text = "";
                            goto IL_000a;
                        case 2125:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_06d3;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number != 13)
                                {
                                    goto end_IL_0000_2;
                                }
                                Listart = 200;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_06d3;
                            }
                        end_IL_0000:
                            break;
                        IL_000a:
                            num = 2;
                            if (Art < EEventArt.eA_499)
                            {
                                num5 = PersInArb;
                            }
                            else
                            {

                                num5 = FamInArb;
                            }
                            goto IL_002d;
                        IL_002d: // <========== 3
                            num = 8;
                            DataModul.DB_WitnessTable.Index = "FamSu";
                            DataModul.DB_WitnessTable.Seek("=", num5, "10");
                            DataModul.DB_WitnessTable.Index = "ZeugSu";
                            eWKennz = "10";
                            DataModul.DB_WitnessTable.Seek("=", num5, eWKennz, Art, LfNR);
                            b = 1;
                            goto IL_0114;
                        IL_0114: // <========== 3
                            num = 14;
                            string text2;
                            if (!DataModul.DB_WitnessTable.EOF && !DataModul.DB_WitnessTable.NoMatch && !(DataModul.DB_WitnessTable.Fields[WitnessFields.FamNr].AsInt() != num5
                                   | DataModul.DB_WitnessTable.Fields[WitnessFields.Kennz].AsInt() != 10) && !(DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsEnum<EEventArt>() != Art
                               | DataModul.DB_WitnessTable.Fields[WitnessFields.LfNr].AsInt() != LfNR))
                            {
                                if (DataModul.DB_WitnessTable.NoMatch)
                                {
                                    Interaction.MsgBox("F26");
                                }
                                else
                                {

                                    text2 = DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsString() + Strings.Right("          " + DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsString(), 10);
                                    text += text2;
                                }
                                goto IL_0293;
                            }
                            goto IL_02b2;
                        IL_0293: // <========== 3
                            num = 33;
                            DataModul.DB_WitnessTable.MoveNext();
                            b = (byte)unchecked((uint)(b + 1));
                            if (unchecked(b) <= 99u)
                            {
                                goto IL_0114;
                            }
                            goto IL_02b2;
                        IL_02b2: // <========== 4
                            num = 35;
                            b2 = (byte)Math.Round(text.Length / 14.0);
                            b = 1;
                            goto IL_04d9;
                        IL_04d0: // <========== 3
                            num = 65;
                            b = (byte)unchecked((uint)(b + 1));
                            goto IL_04d9;
                        IL_04d9:
                            if (unchecked(b <= (uint)b2))
                            {
                                text2 = text.Left(14);
                                text = Strings.Mid(text, 15, text.Length);
                                PersInArb = (int)Math.Round(Strings.Mid(text2, 5, 10).AsDouble());
                                Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                ProjectData.ClearProjectError();
                                num3 = 2;
                                if (Listart != "".AsDouble())
                                {
                                    switch (Listart)
                                    {
                                        case 1:
                                            MyProject.Forms.Stammfolge.Namenindex();
                                            break;
                                        case 2:
                                            Namenindex(Ahne);
                                            break;
                                        default:
                                            break;
                                    }
                                    Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                }
                                Person.SetFullSurname(Person_FullSurname(_Modul1.Instance.Person, _Modul1.Instance.DAus[89] == "1"));
                                var sGivennames = DAus[76] == "1"
                                    ? " <" + PersInArb.AsString().Trim() + ">" + (_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames).Trim()
                                    : _Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames;
                                if (Kont1[20].Trim() != "")
                                {
                                    Kont1[20] = Kont1[20] + "; " + sGivennames + " " + _Modul1.Instance.Person.FullSurName.Trim();
                                }
                                else
                                {
                                    Kont1[20] = " " + Strings.Trim(sGivennames + " " + _Modul1.Instance.Person.FullSurName.Trim());
                                }
                                goto IL_04d0;
                            }
                            byte b3 = LfNR;
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", Art, num5, b3);
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                goto end_IL_0000_2;
                            }
                            if (Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem4].Value))
                            {
                                goto end_IL_0000_2;
                            }
                            if (DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim()== "")
                            {
                                goto end_IL_0000_2;
                            }
                            if (Kont1[20].Trim() != "")
                            {
                                Kont1[20] = Kont1[20] + "; " + DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim();
                            }
                            else
                            {

                                Kont1[20] = DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim();
                            }
                            goto IL_065f;
                        IL_065f: // <========== 3
                            num = 78;
                            if (DAus[87] == "0")
                            {
                                Kont1[20] = Text_Retweg(Kont1[20]);
                            }
                            goto end_IL_0000_2;
                        IL_06d3:
                            num4 = unchecked(num2 + 1);
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 4:
                                case 7:
                                case 8:
                                    goto IL_002d;
                                case 14:
                                    goto IL_0114;
                                case 28:
                                case 32:
                                case 33:
                                    goto IL_0293;
                                case 15:
                                case 18:
                                case 21:
                                case 24:
                                case 35:
                                    goto IL_02b2;
                                case 61:
                                case 64:
                                case 65:
                                    goto IL_04d0;
                                case 74:
                                case 77:
                                case 78:
                                    goto IL_065f;
                                case 80:
                                case 81:
                                case 82:
                                case 83:
                                case 84:
                                case 88:
                                case 89:
                                case 90:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 2125;
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

    public bool IstDa(string DateiName) => FileSystem.Dir(DateiName) != "";

    public DateTime AtomicTime(string sTimeServer)
    {
        // Pseudocode:
        // 1. Erzeuge eine Anfrage an den NTP-Server (Network Time Protocol) mit sTimeServer als Hostnamen.
        // 2. Sende ein NTP-Paket (48 Bytes) an Port 123/UDP.
        // 3. Empfange die Antwort und lese die Zeitdaten aus dem Paket.
        // 4. Konvertiere die empfangene Zeit (Sekunden seit 1900-01-01) in DateTime.
        // 5. Gib die UTC-Zeit als DateTime zurück.
        // 6. Bei Fehlern: Gib DateTime.MinValue zurück.

        try
        {
            const int NTPDataLength = 48;
            byte[] ntpData = new byte[NTPDataLength];
            ntpData[0] = 0x1B; // NTP-Anfrage, Version 3, Client-Mode

            var addresses = Dns.GetHostEntry(sTimeServer).AddressList;
            var ipEndPoint = new System.Net.IPEndPoint(addresses[0], 123);
            using (var socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.ReceiveTimeout = 3000;
                socket.Connect(ipEndPoint);
                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();
            }

            const byte serverReplyTime = 40;
            ulong intPart = (ulong)ntpData[serverReplyTime] << 24 |
                            (ulong)ntpData[serverReplyTime + 1] << 16 |
                            (ulong)ntpData[serverReplyTime + 2] << 8 |
                            (ulong)ntpData[serverReplyTime + 3];

            ulong fractPart = (ulong)ntpData[serverReplyTime + 4] << 24 |
                              (ulong)ntpData[serverReplyTime + 5] << 16 |
                              (ulong)ntpData[serverReplyTime + 6] << 8 |
                              (ulong)ntpData[serverReplyTime + 7];

            ulong milliseconds = (intPart * 1000) + (fractPart * 1000 / 0x100000000L);

            // NTP Zeit beginnt am 1.1.1900
            DateTime ntpEpoch = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime networkDateTime = ntpEpoch.AddMilliseconds((long)milliseconds);

            return networkDateTime;
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    public string Conform(string sText)
    {
        throw new NotImplementedException();
    }

    public float Datcheck(int eArt)
    {
        throw new NotImplementedException();
    }

    public void Datles(int Ubg, int PersInArb)
    {
        throw new NotImplementedException();
    }

    public (string sDat_Birth, string sDat_Death) Datles(int PersInArb, IPersonData person, bool xPlace = false)
    {
        throw new NotImplementedException();
    }

    public void DatPruef(int Pschalt)
    {
        throw new NotImplementedException();
    }

    public string Date2DotDateStr2(string Dat)
    {
        throw new NotImplementedException();
    }

    public void DezRechnen(ref string A4)
    {
        throw new NotImplementedException();
    }

    public void GedAus_Diskvoll()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> EnumerateMandants(string drive)
    {
        throw new NotImplementedException();
    }

    public string GoogleInstallPath()
    {
        throw new NotImplementedException();
    }

    public short IsFormloaded(Form Formtocheck)
    {
        throw new NotImplementedException();
    }

    public void Lerz(ref short A, ref int u)
    {
        throw new NotImplementedException();
    }

    public string Strings_Leerweg1(string sText)
    {
        throw new NotImplementedException();
    }

    public void OFBTextPruefenSpeichern(string UbgT, string Kennz, int LfNR)
    {
        throw new NotImplementedException();
    }

    public bool OfficeAppInstalled(IModul1.MSOfficeComponent nComponent)
    {
        throw new NotImplementedException();
    }

    public string OfficeInstallPath(IModul1.MSOfficeVersion nVersion)
    {
        throw new NotImplementedException();
    }

    public object OpenRecordSet()
    {
        throw new NotImplementedException();
    }

    public string ortles(int OrtNr, byte Schalt)
    {
        throw new NotImplementedException();
    }

    public void Orttextspeichern()
    {
        throw new NotImplementedException();
    }

    public void Paten_O_Taufe()
    {
        throw new NotImplementedException();
    }

    public void Perles(int PersInArb)
    {
        throw new NotImplementedException();
    }

    public int Persatzles(int PersInArb)
    {
        throw new NotImplementedException();
    }

    public int Person_TextSpeichern(string sText, int iPerson, ETextKennz eKennz1, int iLfNR = 0, short Ruf = 0)
    {
        throw new NotImplementedException();
    }

    public void Quellenaus(EEventArt L)
    {
        throw new NotImplementedException();
    }

    public void Sichwand(string Dasich, string sDatumV_S, DateTime dDatumB, EEventArt eArt)
    {
        throw new NotImplementedException();
    }

    public void Sperrfehler()
    {
        throw new NotImplementedException();
    }

    public void STextles(string Formnam, ETextKennz Kennz, string UbgT, ListBox.ObjectCollection ocItems)
    {
        throw new NotImplementedException();
    }

    public int TextSpeich(string sText, string sLeitName, ETextKennz eTKennz, int PersInArb = 0, int LfNR = 0)
    {
        throw new NotImplementedException();
    }

    public string Strngs_Umlaute4(string Fld, int uml)
    {
        throw new NotImplementedException();
    }

    public void Vornam_Namles(int personNr)
    {
        throw new NotImplementedException();
    }

    public string[] F_GetAllFiles(string sPath, int funk)
    {
        throw new NotImplementedException();
    }

    public IList<int> Ehesuch(int personNr, string Persex)
    {
        throw new NotImplementedException();
    }

    public string BuildFullSurName(IPersonData person, bool xFamToUpper = true)
    {
        throw new NotImplementedException();
    }

    public void FrmPerson_EventUpd(int PersInArb)
    {
        throw new NotImplementedException();
    }

    public void Berufles(int PersInArb, EEventArt Beruf, ComboBox combo1)
    {
        throw new NotImplementedException();
    }

    public string ortles1(int OrtNr, byte Schalt)
    {
        throw new NotImplementedException();
    }

    public IList<T> DeleteDoublicates<T>(IList<T> oList, IList<T> gList)
    {
        var oResult = new List<T>();
        foreach (T o in oList)
        {
            if (!oResult.Contains(o))
            {
                oResult.Add(o);
            }
        }

        return oResult;
    }

    public int Eltsuch(int persInArb)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IListItem<(int, DateTime, ELinkKennz)>> Family_Kindsuch(int iFamNr)
    {
        throw new NotImplementedException();
    }

    public string Ancesters_GetPersonData(int PersonNr, out int Ahnsp, out string Kont20)
    {
        throw new NotImplementedException();
    }

    public string Ancester_GetAncesterData(int iAnc)
    {
        throw new NotImplementedException();
    }

    public void Ausdruck(string Datnam)
    {
        throw new NotImplementedException();
    }

    public bool Bildzeig1(string biart, int PBW, int PBH, string Form, out string BiText1, out string Bitext2)
    {
        throw new NotImplementedException();
    }

    public string DezRechnen(string A4, string ubgT)
    {
        throw new NotImplementedException();
    }

    public short IsFormloaded(object Formtocheck)
    {
        throw new NotImplementedException();
    }

    public string[] Ortles(IPlaceData place, int Schalt = 0, Action<int, string>? export = null)
    {
        throw new NotImplementedException();
    }

    public void STextles(string Formnam, ETextKennz Kennz, string UbgT, IList ocItems)
    {
        throw new NotImplementedException();
    }

    public int TextSpeich(string sText, string sLeitName, ETextKennz eTKennz, int PersInArb = 0, int LfNR = 0, short ruf = 0, bool xCalln = false, bool xNickn = false)
    {
        throw new NotImplementedException();
    }

    public void Berufles(int PersInArb, EEventArt Beruf, object combo1)
    {
        throw new NotImplementedException();
    }

    public IList<T> DeleteDoublicates<T>(IList<T> oList)
    {
        throw new NotImplementedException();
    }

    public void KTextles(string Formnam, ETextKennz eTKennz, IList oIIList, (string sText, ETextKennz eTKnz) Bezeichnu)
    {
        throw new NotImplementedException();
    }

    public bool RemoveWriteProtection(string sFile)
    {
        throw new NotImplementedException();
    }

    public string Wochtag(string Datu)
    {
        throw new NotImplementedException();
    }

    public string Event_PreDisplay(bool xCitation = false, bool xWitness = false, bool xAnnotation = false, bool xBC = false, bool xReg = false)
    {
        throw new NotImplementedException();
    }

    public IDictionary<EEventArt, ((EEventArt, int, short), string, DateTime)> FamPerDatles(int PersInArb, int schalt)
    {
        throw new NotImplementedException();
    }

    public void KTextlesTL5(ETextKennz txknz, IList items, (string, ETextKennz) m_Bezeichnu)
    {
        throw new NotImplementedException();
    }

    public IListItem<int> Event_ToShortLine(IEventData cEvent)
    {
        throw new NotImplementedException();
    }

    public void FamDatLes_int(int famInArb, Action disableIllg, Action<string, int, string> setEventText)
    {
        throw new NotImplementedException();
    }

    public bool EstDateLes(out string text)
    {
        throw new NotImplementedException();
    }

    public void Listbox3Clip(IList lList, short A)
    {
        throw new NotImplementedException();
    }

    public int? FamDatYear(int FamInArb, short schalt)
    {
        throw new NotImplementedException();
    }

    public bool GeolesPlace(IPlaceData cPlace, Action<(string, string)>? action = null, bool v = true)
    {
        throw new NotImplementedException();
    }

    public void GEExportPlace(string sName, (string sLongitude, string sLatiude) cKoords, bool xAppend = true)
    {
        throw new NotImplementedException();
    }

    public string Umlaute(string Fld)
    {
        throw new NotImplementedException();
    }

    public string Umlaute_UCase(string sText)
    {
        throw new NotImplementedException();
    }

    public short Famsatzles(int FamInArb, short Rich, IFamilyData cFamily)
    {
        throw new NotImplementedException();
    }

    public IListItem<int> Famzeig(int Fam, ELinkKennz Kenn)
    {
        throw new NotImplementedException();
    }

    public void DataModul_Texte_ListDistLeitname(ETextKennz eTKennz, string UbgT, IList items)
    {
        throw new NotImplementedException();
    }

    public short Strings_Lerz(string s)
    {
        throw new NotImplementedException();
    }

    public int TextSpeich(string sText, string sLeitName, ETextKennz eTKennz, int PersInArb = 0, int LfNR = 0, bool xCalln = false, bool xNickn = false)
    {
        throw new NotImplementedException();
    }

    public void Datles3(short listart, long v, object value, ref bool neb)
    {
        throw new NotImplementedException();
    }

    public void Datles10(ref short listart, bool m1_Ki)
    {
        throw new NotImplementedException();
    }

    public int System_TestForm_Height()
    {
        throw new NotImplementedException();
    }

    public int DataModul_PeekMandant_RecordCount()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets the full surname for a list of name components.
    /// </summary>
    /// <param name="kont">The list containing name components.</param>
    /// <param name="xFamToUpper">If true, converts family name to uppercase.</param>
    public void Person_FullSurname(IList<string> kont, bool xFamToUpper)
    {
        if (kont == null || kont.Count == 0)
            return;

        string sName = kont[0];
        sName = xFamToUpper ? sName.ToUpper() : sName;
        if (kont.Count > 1 && kont[1] != "")
        {
            sName = kont[1] + " " + sName;
        }
        if (kont.Count > 2 && kont[2] != "")
        {
            sName = sName + " " + kont[2];
        }
        kont[0] = sName;
    }

    /// <summary>
    /// Reads family data for the given switch value.
    /// </summary>
    /// <param name="schalt">The switch/mode value.</param>
    /// <param name="asFamDate">The output array with family data.</param>
    public void Famdatles1(int schalt, out string[] asFamDate)
    {
        // Delegate to the existing implementation with byte parameter
        Famdatles1((byte)schalt, out asFamDate);
    }

    /// <summary>
    /// Reads family data.
    /// </summary>
    public void Famles()
    {
        throw new NotImplementedException();
    }

    /// <summary>Removes trailing newlines and spaces from text.</summary>
    public string Retweg(string sText) => Text_Retweg(sText);

    /// <summary>Reads source data with dot notation for citations.</summary>
    public void QuelledotnDatum(ref int iNr, EEventArt eArt, ref short iLfNR)
    {
        // Delegates to QuellenDatum - implement specific dot notation behavior if needed
        QuellenDatum(ref iNr, eArt, ref iLfNR);
    }

    #region IPrintDat Implementation
    /// <summary>Gets the print-specific data interface.</summary>
    public IPrintDat PrintDat => _printDat ??= new CPrintDat(this);
    private IPrintDat? _printDat;

    /// <summary>
    /// Inner class implementing IPrintDat, delegating to _Modul1 fields.
    /// </summary>
    private class CPrintDat : IPrintDat
    {
        private readonly _Modul1 _modul1;

        public CPrintDat(_Modul1 modul1)
        {
            _modul1 = modul1;
        }

        public int Hoch { get => _modul1.Hoch; set => _modul1.Hoch = (byte)value; }
        public int BemZahl { get => _modul1.BemZahl; set => _modul1.BemZahl = value; }
        public IList<string> KontBem => _modul1.KontBem;
        public IList<string> KontSP => _modul1.KontSP;
        public IList<string> KontSP1 => _modul1.KontSP1;
        public int FamSp { get => _modul1.FamSp; set => _modul1.FamSp = value; }
        public bool Pat { get => _modul1.Pat; set => _modul1.Pat = value; }
        public int Flagsch { get => _modul1.Flagsch; set => _modul1.Flagsch = (short)value; }
        public byte KonLen { get => _modul1.KonLen; set => _modul1.KonLen = value; }
        public int Ja { get => _modul1.Ja; set => _modul1.Ja = (short)value; }
        public byte LfNR { get => (byte)_modul1.LfNR; set => _modul1.LfNR = value; }

        public string Retweg(string sText) => _modul1.Retweg(sText);
        public void Namenindex(long lAhne) => _modul1.Namenindex(lAhne);
        public void PerQu(ref int iFamPer) => _modul1.PerQu(ref iFamPer);
        public void QuellenDatum(ref int iNr, EEventArt eArt, ref short iLfNR) => _modul1.QuellenDatum(ref iNr, eArt, ref iLfNR);
        public void QuelledotnDatum(ref int iNr, EEventArt eArt, ref short iLfNR) => _modul1.QuelledotnDatum(ref iNr, eArt, ref iLfNR);
        public void FWohn(EEventArt eArt, ref short iListart) => _modul1.FWohn(eArt, ref iListart);
        public void Zeuge_Bei(int iPersInArb, ref short iListart) => _modul1.Zeuge_Bei(iPersInArb, ref iListart);
    }
    #endregion
}
