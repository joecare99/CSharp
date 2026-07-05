using BaseLib.Helper;
using Gedcomles.ViewModels;
using Gedcomles.My;
using Gedcomles.Views;
using GenFree.ADODB;
using GenFree.Constants;
using GenFree.Data;
using GenFree.Data.Models;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.UI;
using GenFree.Models;
using GenFree.Sys;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static GenFree.Constants.GedComTags;
using GenFree.Interfaces.VB;

namespace GenFree;

public class _Modul1 : IModul1
{
    private static IModul1? _instance = null;
    public static IModul1 Instance => _instance ??= new _Modul1();

    public string AppName => "Gen_FreeWin";
    public string VendorName => "JC-Soft";
    public string Author => "Joe Care";

    public string VersionT => $"{AppName} das freie Genealogieprogramm";
    public string Version1 => $"(c) 2025 {Author}";
    public string Version => "Version 24.09.02";
    public string VersDat => "Stand 15.07.2018";
    private const int LB_SETTABSTOPS = 402;

    // FileSystem
    public string TempPath => $"\\{AppName}\\Temp\\"; // 
    public string InitDir => $"\\{AppName}\\Init\\"; //  
    public string ListDir => $"\\{AppName}\\List\\"; //  
    public string HelpDir => $"\\{AppName}\\Help\\"; //  
    public string MainProg => $"\\{AppName}\\{AppName}.exe"; //
    public string GenFreeDir => $"\\{AppName}\\"; //
    public string InstPath => throw new NotImplementedException();
    public string PictureDir => Path.Combine(Verz,"Bilder");

    public DriveInfo cMandDrive => new DriveInfo(Verz1);

    public string Verz { get; set; }
    public string Verz1 { get; set; }

    //==================================================================================================
    public string Einles_Einfueg { get; set; } = "";

    public string Zusatzquelle { get; set; } = "";
    public FileStream neudat;
    public StreamWriter sw;
    public IList<string> Zeug { get; } = new string[101];

    // I18N
    public IApplUserTexts IText { get; }
    public IList<string> DTxt { get; } = new string[21];

    // User-Data
    public IUserData User { get; } = new CUserData();
    #region DataModul

    public IRecordset DT_NachkTable { get; set; }
    public IRecordset DT_AhnTable { get; set; }
    public IRecordset Bild { get; set; }
    #endregion

    #region Areas
    public IGedAus GedAus { get; }
    public IEinles Einles { get; }
    public IPrintDat PrintDat { get; }

    #endregion
    
    
    public string AutoupD { get; set; } = "";
    public string Kennz { get; set; }

    public int Ubg { get; set; } = 0;
    public string UbgT { get; set; } = "";
    public string UbgT1 { get; set; } = "";
    public string Mandant { get; set; }
    public int Behand;
    public int d_;
    public int d => d_;

    public int FamugNr;
    public short FeG { get; set; } = 0;
    public short aufr;
    public float Fs { get; set; }
    public string Datsicha;


    public string[] Quellarry = new string[101];



    public int LfNR { get; set; }
    public string NamenSuch_Wort { get; set; }
    public short Les { get; set; }
    public short SF;
    public short Ruf;
    public short Pataus;
    public byte Histor { get; set; }
    public byte Leer;
    public string Menue_Ziel { get; }
    public FormWindowState State;
    public short Schalt;
    public int PersInArb { get; set; } = 0;
    public int FamInArb { get; set; } = 0;
    public int Ortnr;
    public IList<string> Kont { get; } = new string[101];
    public IList<string> Kont1 { get; } = new string[101];
    public IList<string> KontP { get; } = [];
    public IList<string> KontF { get; } = [];
    public IList<string> KontM { get; } = [];
    public IList<string> Kin { get; } = new string[101];
    public IList<string> TxT { get; } = new string[151];

    //o04_Family
    public IFamilyData Family { get; set; }
    public int[] Family_Kind = new int[100];

    public IList<string> Aus { get; } = new string[61];

    public string Mldg;
    public short Op;
    public short Suchschalt;
    public short Trauaus;
    public short Jahrg;
    public bool DatPersonenstand;


    public Color HintFarb { get; set; }
    public Color Farb { get; set; }
    public Color Farb1 { get; set; }

    public EEventArt Art { get; set; }
    public int Einles_LaufNr { get; set; }

    public short Quell { get; set; }
    public int Namensuch_Suchper;
    public int Namensuch_Suchfam;
    public string Font2;
    public string Font1;
    public string Konv;

    public short Mon;

    public string mon1;
    public IList<string> Absend { get; } = new string[12];

    public long freeclusters;
    public long Free;
    public long clusters;
    public long bytes;
    public short ok;
    public int sectors;
    public DriveType Typ { get; }

    public ELinkKennz eLKennz { get; set; }
    public ETextKennz eTKennz { get; set; }
    public string sPKennz { get; set; } = "";
    public ETextKennz eNKennz { get; set; }




    public bool Allaus = false;

    public string[] Childarray = new string[100];

    private string Heute = DateTime.Today.ToString("yyyyMMdd");

    public int Heut;

    public int AnzP;

    public string Datumd { get; set; }
    public bool isshon;
    public long Gedaus_DiskSize;
    public string SpDisk;
    private const short GWW_HINSTANCE = -6;

    [SpecialName]
    private short _Diskvoll_Fn;

    [SpecialName]
    private byte _Diskvoll_LfDisk;
    public string sOKennz { get; set; }
    public string sWKennz { get; set; }

    #region Methods
    private _Modul1()
    {
        neudat = new FileStream(TempPath + "Gedcom.ged", FileMode.Create);
        sw = new StreamWriter(neudat, Encoding.UTF8);
        Heut = checked((int)Math.Round(Heute.AsDouble()));
        IText = IoC.GetRequiredService<IApplUserTexts>();
    }

    #region unused
    public void toansi(string sFileName, string sDestFile)
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
                int num4;
                switch (try0000_dispatch)
                {
                    default:
                        num = 1;
                        UTF8_Encode(GedAus.FILENAM);
                        goto IL_000e;
                    case 385:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_012f;
                                default:
                                    goto end_IL_0000;
                            }
                            Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                            if (Interaction.MsgBox(Mldg, MsgBoxStyle.OkCancel | MsgBoxStyle.Question, "Fehler") == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_0133;
                        }
                    end_IL_0000:
                        break;
                    IL_000e:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        FileStream fileStream2 = default;
                        fileStream2 = new FileStream(Verz + sFileName, FileMode.Open);
                        byte[] array = new byte[checked((int)fileStream2.Length + 1)];
                        fileStream2.Read(array, 0, array.Length);
                        Encoding.Convert(Encoding.Unicode, Encoding.Default, array);
                        FileStream fileStream = new FileStream(sDestFile, FileMode.Create);
                        fileStream.Write(array, 0, array.Length);
                        fileStream.Close();
                        goto end_IL_0000_2;
                    IL_012f:
                        num4 = num2 + 1;
                        goto IL_0133;
                    IL_0133:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 385;
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

    public bool UTF8_EncodeToFile(string sData, string sFile)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        bool flag;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                StreamReader streamReader;
                FileStream stream;
                switch (try0000_dispatch)
                {
                    default:
                        num = 1;
                        goto IL_0019;
                    case 920:
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
                            while (true)
                            {
                                num2 = 0;
                                switch (num4)
                                {
                                    case 1:
                                        break;
                                    case 27:
                                        goto end_IL_0000_2;
                                    case 29:
                                        num = 29;
                                        Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                                        if (Interaction.MsgBox(Mldg, MsgBoxStyle.OkCancel | MsgBoxStyle.Question, "Fehler") == MsgBoxResult.Cancel)
                                        {
                                            ProjectData.EndApp();
                                        }
                                        goto case 31;
                                    case 31:
                                    case 33:
                                        num = 33;
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
                            goto default;
                        }
                    end_IL_0000_2:
                        break;
                    IL_0019:
                        num = 2;
                        stream = new FileStream(Verz + "33.bak", FileMode.Open);
                        streamReader = new StreamReader(stream, Encoding.Default);
                        if (sData.Length == 0)
                        {
                            break;
                        }
                        ProjectData.ClearProjectError();
                        num3 = 1;
                        AdoDbProcs.ADODBStream(sData, sFile);
                        ProjectData.ClearProjectError();
                        num3 = 0;
                        break;
                }
                num = 27;
                flag = Information.Err().Number == 0;
                break;
            end_IL_0000:
                ;
            }
            catch (Exception obj3) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj3);
                try0000_dispatch = 920;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        }
        bool result = flag;
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
        return result;
    }
    public string UTF8_DecodeFromFile(string sFile)
    {
        //Discarded unreachable code: IL_027f
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string text = default;
        string text2;
        int num4;
        object instance;
        object[] array;
        object[] arguments;
        bool[] array2;
        switch (try0000_dispatch)
        {
            default:
                ProjectData.ClearProjectError();
                num3 = 2;
                num = 2;
                text = "";
                ProjectData.ClearProjectError();
                num3 = 1;
                object obj = Interaction.CreateObject("ADODB.Stream");
                if (Information.Err().Number == 0)
                {
                    object obj2 = obj;
                    NewLateBinding.LateSet(obj2, null, "Charset", new object[1]
                    {
                            "utf-8"
                    }
                    , null, null);
                    NewLateBinding.LateSet(obj2, null, "Mode", new object[1]
                    {
                            3
                    }
                    , null, null);
                    NewLateBinding.LateSet(obj2, null, "Type", new object[1]
                    {
                            2
                    }
                    , null, null);
                    NewLateBinding.LateCall(obj2, null, "Open", new object[0], null, null, null, IgnoreReturn: true);
                    instance = obj2;
                    array = new object[1]
                    {
                            sFile
                    }
                    ;
                    arguments = array;
                    array2 = new bool[1]
                    {
                            true
                    }
                    ;
                    NewLateBinding.LateCall(instance, null, "LoadFromFile", arguments, null, null, array2, IgnoreReturn: true);
                    if (array2[0])
                    {
                        sFile = (string)Conversions.ChangeType(array[0], typeof(string));
                    }
                    NewLateBinding.LateSet(obj2, null, "Position", new object[1]
                    {
                            0
                    }
                    , null, null);
                    text = NewLateBinding.LateGet(obj2, null, "Readstring", new object[0], null, null, null).AsString();
                    NewLateBinding.LateCall(obj2, null, "SaveToFile", new object[1]
                    {
                                "C:\\4\\btest.ged"
                    }, null, null, null, IgnoreReturn: true);
                    NewLateBinding.LateCall(obj2, null, "Close", new object[0], null, null, null, IgnoreReturn: true);
                    obj2 = null;
                    obj = null;
                }
                goto IL_01c9;
            IL_01c9:
                ProjectData.ClearProjectError();
                num3 = 0;
                text2 = text;
                goto end_IL_0000_2;
            IL_0283:
                num4 = num2 + 1;
                num2 = 0;
                switch (num4)
                {
                    case 1:
                        break;
                    case 18:
                    case 19:
                        goto IL_01c9;
                }
                goto default;
        }
    end_IL_0000_2:
        return text;
    }
    public string UTF8_Encode(string sData)
    {
        //Discarded unreachable code: IL_0226
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        object obj2 = default;
        string text = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                object instance;
                object[] array;
                object[] arguments;
                bool[] array2;
                switch (try0000_dispatch)
                {
                    default:
                        num = 1;
                        if (sData.Length == 0)
                        {
                            break;
                        }
                        goto IL_0011;
                    case 680:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 1:
                                    break;
                                default:
                                    goto end_IL_0000_2;
                            }
                            int num4 = num2 + 1;
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 19:
                                case 20:
                                    goto IL_01e3;
                                case 23:
                                case 24:
                                    goto IL_0206;
                                case 26:
                                    goto end_IL_0000_3;
                            }
                            goto default;
                        }
                    end_IL_0000_3:
                        break;
                    IL_0011:
                        ProjectData.ClearProjectError();
                        num3 = 1;
                        object obj = Interaction.CreateObject("ADODB.Stream");
                        if (Information.Err().Number == 0)
                        {
                            obj2 = obj;
                            NewLateBinding.LateSet(obj2, null, "Charset", new object[1]
                            {
                            "UTF-8"
                            }
                            , null, null);
                            NewLateBinding.LateSet(obj2, null, "Mode", new object[1]
                            {
                            3
                            }
                            , null, null);
                            NewLateBinding.LateSet(obj2, null, "Type", new object[1]
                            {
                            2
                            }
                            , null, null);
                            NewLateBinding.LateCall(obj2, null, "Open", new object[0], null, null, null, IgnoreReturn: true);
                            if (Information.Err().Number == 0)
                            {
                                instance = obj2;
                                array = new object[1]
                                {
                                sData
                                }
                                ;
                                arguments = array;
                                array2 = new bool[1]
                                {
                                true
                                }
                                ;
                                NewLateBinding.LateCall(instance, null, "WriteText", arguments, null, null, array2, IgnoreReturn: true);
                                if (array2[0])
                                {
                                    sData = (string)Conversions.ChangeType(array[0], typeof(string));
                                }
                                NewLateBinding.LateCall(obj2, null, "Flush", new object[0], null, null, null, IgnoreReturn: true);
                                NewLateBinding.LateSet(obj2, null, "Position", new object[1]
                                {
                                0
                                }
                                , null, null);
                                NewLateBinding.LateSet(obj2, null, "Charset", new object[1]
                                {
                                "x-ansi"
                                }
                                , null, null);
                                NewLateBinding.LateSet(obj2, null, "Position", new object[1]
                                {
                                3
                                }
                                , null, null);
                                sData = NewLateBinding.LateGet(obj2, null, "ReadText", new object[0], null, null, null).AsString();
                            }
                            goto IL_01e3;
                        }
                        goto IL_0206;
                    IL_01e3:
                        num = 20;
                        NewLateBinding.LateCall(obj2, null, "Close", new object[0], null, null, null, IgnoreReturn: true);
                        obj2 = null;
                        obj = null;
                        goto IL_0206;
                    IL_0206: // <========== 3
                        ProjectData.ClearProjectError();
                        num3 = 0;
                        text = sData;
                        break;
                }
                num = 26;
                Interaction.MsgBox(sData);
                break;
            end_IL_0000_2:
                ;
            }
            catch (Exception obj3) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj3);
                try0000_dispatch = 680;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        }
        return text;
    }
    #endregion


    public void Info()
    {
        string text = "Entwicklungshistorie Version 24  Gedcom-Modul";
        text += "\nBitte keine Anfragen hierzu";
        text += "\n30.10. CommitTrans abschalten im Bereich Orte";
        text += "\n24.11. Einzelpersonen Abbruch ohne Auswahl abfangen.";
        text += "\n16.12. Abschneiden überlanger Referenz-Nr..";
        text += "\n05.01. Falsche Datumsangaben Ancestry.com Family Trees nach Datumsphrase";
        text += "\n11.02. mehr als 15 Vornamen abfangen.";
        text += "\n25.03. weitere Einstellungen und Voreinstellung Datenschutz, Lizenznummer.";
        text += "\n31.03. einbuchstabige Vorname übernehmen, Prüfung Person vorhanden vor Sperrprüfung .";
        text += "\n01.04. Teilweise Verschiebung der Geschlechter .";
        text += "\n08.04. @@ doppeln";
        text += "\n09.04. Nochmal Ahnen bei extremen Leernummern.";
        text += "\n24.04. EST und ? Einlesen und ausgeben.";
        text += "\n14.05. Fehler bei leerem Mandanten Daten zufügen abfangen.";
        text += "\n29.05. Leeren Ort aus Ahnenblatt abfangen.";
        text += "\n28.06. Fehlerhafte >NOTE Referenz< abfangen.";
        text += "\n28.06. Export der Orte, Ende nach letztem Ort";
        Interaction.MsgBox(text);
    }


    public void Dateienopen()
    {
        int try0000_dispatch = -1;
        int num = default;
        string Value = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        int Besttest = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                string text;
                string DateiName;
                object obj;
                switch (try0000_dispatch)
                {
                    default:
                        num = 1;
                        Value = "";

                        num = 2;
                        Verz = Persistence.ReadStringInit("GEN-verz.ini");
                        text = Verz.Left(2);

                        num3 = 2;
                        Value = Persistence.ReadStringProg("IDF.Dat");
                        goto IL_00da;
                    IL_00da: // <========== 3
                        num = 13;
                        lErl = 1;
                        ProjectData.ClearProjectError();
                        num3 = 3;
                        FileSystem.Kill(TempPath + "*.*");
                        ProjectData.ClearProjectError();
                        num3 = 4;
                        string ziel = Menue_Ziel;
                        if (!((ziel.Trim() == "") & (Verz != "")))
                        {
                            Verz = Verz1 + ziel + "\\";
                            Einles.Verz_Pictures = Verz + "BILDER";
                            Ubg = 0;
                            FileSystem.MkDir(Verz1 + ziel);
                            if (Ubg == 0)
                            {
                                Ubg = 4;
                            }
                            goto IL_01ba;
                        }
                        goto IL_0723;
                    IL_01ba:
                        num = 29;
                        Behand = Ubg;
                        switch (Behand)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_02ed;
                            case 3:
                                goto IL_03a1;
                            case 4:
                                goto IL_03cc;
                            default:
                                goto IL_044f;
                            case 5:
                                goto end_IL_0000_2;
                        }
                        DataModul.MandDB.Close();
                        DataModul.DOSB.Close();
                        DataModul.TempDB.Close();
                        DataModul.DSB.Close();
                        DataModul.NB.Close();
                        FileSystem.Kill(TempPath + "NumTemp.mdb");
                        FileSystem.Kill(Verz + "*.mdb");
                        Value = InitDir + "\\NUMTEMP.mdb";
                        string destination = TempPath + "NumTemp.mdb";
                        FileSystem.FileCopy(Value, destination);
                        DateiName = "Gen_plusdaten3.mdb";
                        if (Persistence.ExistFile(DateiName))
                        {
                            Value = GenFreeDir + "Gen_plusdaten3.mdb";
                        }
                        else
                        {
                            Value = InitDir + "\\Gen_plusdaten1.mdb";
                        }
                        goto IL_02ba;
                    IL_02ba: // <========== 3
                        num = 50;
                        destination = Verz + "Gen_plusdaten.mdb";
                        FileSystem.FileCopy(Value, destination);
                        FileSystem.FileClose();
                        goto IL_044f;
                    IL_02ed:
                        num = 55;
                        short num5 = checked((short)Interaction.MsgBox("Sie gefährden Ihre Daten\nVorher eine Datensicherung vornehmen?", MsgBoxStyle.YesNo, ""));
                        if (num5 == 6)
                        {
                            DataModul.MandDB.Close();
                            DataModul.DOSB.Close();
                            DataModul.TempDB.Close();
                            DataModul.DSB.Close();
                            MyProject.Forms.Menue1.Close();
                            Interaction.Shell(MainProg + "", AppWinStyle.NormalFocus);
                            ProjectData.EndApp();
                        }
                        goto IL_036f;
                    IL_036f:
                        num = 65;
                        FileSystem.Kill(TempPath + "*.*");
                        Value = InitDir + "\\NUMTEMP.mdb";
                        destination = TempPath + "NumTemp.mdb";
                        FileSystem.FileCopy(Value, destination);
                        goto IL_044f;
                    IL_03a1:
                        num = 71;
                 //       Ziel = "";
                        MyProject.Forms.Menue1.Show();
                        goto end_IL_0000_2;
                    IL_03cc:
                        num = 76;
                        Value = InitDir + "\\NUMTEMP.mdb";
                        destination = TempPath + "NumTemp.mdb";
                        FileSystem.FileCopy(Value, destination);
                        DateiName = "Gen_plusdaten3.mdb";
                        if (Persistence.ExistFile(DateiName))
                        {
                            Value = GenFreeDir + "Gen_plusdaten3.mdb";
                        }
                        else
                        {
                            Value = InitDir + "\\Gen_plusdaten1.mdb";
                        }
                        goto IL_0429;
                    IL_0429: // <========== 3
                        num = 85;
                        destination = Verz + "Gen_plusdaten.mdb";
                        FileSystem.FileCopy(Value, destination);
                        goto IL_044f;
                    IL_044f: // <========== 5
                        num = 91;
                        string name = TempPath + "NUMTemp.mdb";
                        DataModul.NB = DataModul.DAODBEngine_definst.OpenDatabase(name, false, false, "");

                        DataModul.NB.TryExecute($"CREATE Table {dbTables.Bem}(Nr Long,PF Text(1),Art TEXT(6),BemText TEXT(30));");
                        DataModul.NB.TryExecute($"ALTER Table {dbTables.Bem} ADD COLUMN LFNR single;");
                        DataModul.NB.TryExecute($"CREATE INDEX Neu ON {dbTables.Bem} ([Art]);");
                        DataModul.NB.TryExecute($"CREATE INDEX Neu1 ON {dbTables.Bem} ([BemText]);");

                        DataModul.NB.TryExecute($"Drop Index Such on {dbTables.Temp}");
                        DataModul.NB.TryExecute($"CREATE INDEX Such ON {dbTables.Temp} ([AlteNr]);");

                        DataModul.NB.TryExecute($"CREATE Table {dbTables.TempVerk}(AFam Text(120),PNr Long,Kennz Text(1),NFam Long,APer Text(20));");
                        DataModul.NB.TryExecute($"CREATE INDEX such ON {dbTables.TempVerk} (AFam)");
                        DataModul.NB.TryExecute($"CREATE INDEX Test ON {dbTables.TempVerk} ([NFam],[Kennz]);");
                        DataModul.NB.TryExecute($"CREATE INDEX FamBi ON {dbTables.TempVerk} ([NFam],[APer]);");

                        DataModul.NB.TryExecute($"CREATE Table {dbTables.TempZeug}(AFam Text(20),PNr Text(20),Kennz single, Art single,LFnr Single);");
                        DataModul.NB_TZeutable = DataModul.NB.OpenRecordset(dbTables.TempZeug, RecordsetTypeEnum.dbOpenTable);
                        DataModul.NB_TVerkTable = DataModul.NB.OpenRecordset(dbTables.TempVerk, RecordsetTypeEnum.dbOpenTable);
                        DataModul.NB_NumTable = DataModul.NB.OpenRecordset(dbTables.Temp, RecordsetTypeEnum.dbOpenTable);
                        DataModul.NB_BemTable = DataModul.NB.OpenRecordset(dbTables.Bem, RecordsetTypeEnum.dbOpenTable);
                        DataModul.NB_NumTable.Index = "Such";

                        DataModul.NB.TryExecute($"CREATE Table {dbTables.Sur}(Nr Long,PF Text(1),Art TEXT(6),BemText Text(240),Neunr long);");
                        DataModul.NB.TryExecute($"ALTER Table {dbTables.Sur} ADD COLUMN Aus Text(240);");
                        DataModul.NB.TryExecute($"ALTER Table {dbTables.Sur} ADD COLUMN Seite Text(240);");
                        DataModul.NB.TryExecute($"ALTER Table {dbTables.Sur} ADD COLUMN LFNR single;");
                        DataModul.NB.TryExecute($"ALTER Table {dbTables.Sur} ADD COLUMN OrIGI Memo;");
                        DataModul.NB.TryExecute($"ALTER Table {dbTables.Sur} ADD COLUMN Kom Memo;");
                        DataModul.NB.TryExecute($"CREATE INDEX Neu ON {dbTables.Sur} ([BemText]);");
                        DataModul.NB.TryExecute($"CREATE INDEX Such ON {dbTables.Sur} ([Nr],[Art],[LFNR]);");
                        DataModul.NB_SurTable = DataModul.NB.OpenRecordset(dbTables.Sur, RecordsetTypeEnum.dbOpenTable);
                        goto IL_0723;
                    IL_0723: // <========== 3
                        num = 118;
                        lErl = 11;
                        FileSystem.FileClose();
                        ProjectData.ClearProjectError();
                        num3 = 5;
                        Mandant = Verz + "Gen_plusdaten.mdb";
                        DataModul.wrkDefault = DataModul.DAODBEngine_definst.Workspaces[0];
                        int num6 = DateTime.Today.Year;
                        if (num6 < 2018)
                        {
                            DataModul.MandDB = DataModul.DAODBEngine_definst.OpenDatabase(Mandant.ToUpper(), true, false, ";pwd=geheim");
                        }
                        else
                        {
                            DataModul.MandDB = DataModul.DAODBEngine_definst.OpenDatabase(Mandant, true, false, "");
                        }
                        DataModul.MandDB.Execute($"CREATE Table {dbTables.Repo} ({RepoFields.Nr} Long )");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Repo} ADD COLUMN {RepoFields.Name} Text(240);");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Repo} ADD COLUMN {RepoFields.Strasse} Text(240)");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Repo} ADD COLUMN {RepoFields.Ort} text(240)");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Repo} ADD COLUMN {RepoFields.PLZ} text(240)");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Repo} ADD COLUMN {RepoFields.Fon} Text(240)");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Repo} ADD COLUMN {RepoFields.Mail} Text(240)");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Repo} ADD COLUMN {RepoFields.Http} Text(240)");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Repo} ADD COLUMN {RepoFields.Bem} Memo");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Repo} ADD COLUMN {RepoFields.Suchname} Text(240)");
                        DataModul.MandDB.Execute($"CREATE INDEX Name ON {dbTables.Repo} ([{RepoFields.Suchname}]);");
                        DataModul.MandDB.Execute($"CREATE unique INDEX Such ON {dbTables.Repo} ([{RepoFields.Suchname}],[{RepoFields.Ort}]) WITH DISALLOW NULL;");
                        DataModul.MandDB.Execute($"CREATE INDEX Nr ON {dbTables.Repo} ([{RepoFields.Nr}]);");

                        DataModul.MandDB.Execute($"CREATE Table {dbTables.RepoTab} ({RepoTabFields.Quelle} Long )");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.RepoTab} ADD COLUMN {RepoTabFields.Repo} Long");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.RepoTab} ADD COLUMN {RepoTabFields.Repoalt} Text(240)");
                        DataModul.MandDB.Execute($"CREATE INDEX Nr ON {dbTables.RepoTab} ([{RepoTabFields.Quelle}]);");
                        DataModul.MandDB.Execute($"CREATE INDEX Dop ON {dbTables.RepoTab} ([{RepoTabFields.Quelle}],[{RepoTabFields.Repo}]);");
                        DataModul.MandDB.Execute($"CREATE INDEX leer ON {dbTables.RepoTab} ([{RepoTabFields.Repo}]);");
                        DataModul.MandDB.Execute($"CREATE INDEX Alt ON {dbTables.RepoTab} ([{RepoTabFields.Repoalt}]);");

                        DataModul.MandDB.Execute($"CREATE INDEX {TexteIndex.RTexte} ON {dbTables.Texte} (  [TxNr],[Kennz]);");
                        DataModul.DB_RepoTable = DataModul.MandDB.OpenRecordset(dbTables.Repo, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_RepoTab = DataModul.MandDB.OpenRecordset(dbTables.RepoTab, RecordsetTypeEnum.dbOpenTable);

                        DataModul.TempDB = DataModul.DAODBEngine_definst.Workspaces[0].CreateDatabase(Verz + "Tempo.mdb", ";LANGID=0x0409;CP=1252;COUNTRY=0");
                        DataModul.TempDB = DataModul.DAODBEngine_definst.OpenDatabase(Verz + "Tempo.mdb", false, false, "");

                        DataModul.DOSB = DataModul.DAODBEngine_definst.Workspaces[0].CreateDatabase(Verz + "Ort1.mdb", ";LANGID=0x0409;CP=1252;COUNTRY=0");
                        DataModul.DOSB = DataModul.DAODBEngine_definst.OpenDatabase(Verz + "Ort1.mdb", false, false, "");
                        DataModul.DOSB.Execute($"CREATE Table {dbTables.OrtSuch}(Name Text(100));");
                        DataModul.DOSB.Execute($"ALTER Table {dbTables.OrtSuch} add Column Nr Long;");
                        DataModul.DOSB.Execute($"CREATE  INDEX Ortnr ON {dbTables.OrtSuch} (  [Nr]);");
                        DataModul.DOSB.Execute($"CREATE  INDEX Ortsu ON {dbTables.OrtSuch} (  [Name]);");
                        DataModul.DSB = DataModul.DAODBEngine_definst.Workspaces[0].CreateDatabase(Verz + "Such.mdb", ";LANGID=0x0409;CP=1252;COUNTRY=0");

                        DataModul.DSB = DataModul.DAODBEngine_definst.OpenDatabase(Verz + "Such.mdb", false, false, "");
                        DataModul.DSB.Execute($"CREATE Table {dbTables.Such}(Name Text(50));");
                        DataModul.DSB.Execute($"ALTER Table {dbTables.Such} add Column Datum Integer;");
                        DataModul.DSB.Execute($"ALTER Table {dbTables.Such} add Column Nummer long;");
                        DataModul.DSB.Execute($"ALTER Table {dbTables.Such} add Column eLKenn Text (1);");
                        DataModul.DSB.Execute($"ALTER Table {dbTables.Such} add Column Sich Text(1);");
                        DataModul.DSB.Execute($"CREATE UNIQUE INDEX Nummer ON {dbTables.Such} (  [Nummer]);");
                        DataModul.DSB.Execute($"CREATE  INDEX Namen ON {dbTables.Such} (  [Name]);");
                        DataModul.DSB.Execute($"CREATE  INDEX Persuch ON {dbTables.Such} (  [Name],[Datum]);");
                        DataModul.TempDB.Execute($"CREATE Table {dbTables.Ahnen1} (Gen long)");
                        DataModul.TempDB.Execute($"ALTER Table {dbTables.Ahnen1} ADD COLUMN PerNr long;");
                        DataModul.TempDB.Execute($"ALTER Table {dbTables.Ahnen1} ADD COLUMN Weiter Text(1);");
                        DataModul.TempDB.Execute($"ALTER Table {dbTables.Ahnen1} ADD COLUMN EHE long;");
                        DataModul.TempDB.Execute($"ALTER Table {dbTables.Ahnen1} ADD COLUMN Ahn Text(40);");
                        DataModul.TempDB.Execute($"ALTER Table {dbTables.Ahnen1} ADD COLUMN Name Text(255);");
                        DataModul.TempDB.Execute($"ALTER Table {dbTables.Ahnen1} ADD COLUMN spitz Text (1);");
                        DataModul.TempDB.Execute($"CREATE INDEX spitz ON {dbTables.Ahnen1} ([Spitz],[Ahn]);");
                        DataModul.TempDB.Execute($"CREATE INDEX Namen ON {dbTables.Ahnen1} ([Name],[Ahn]);");
                        DataModul.TempDB.Execute($"CREATE INDEX PerNr ON {dbTables.Ahnen1} ([PerNr]);");
                        DataModul.TempDB.Execute($"CREATE INDEX Gen ON {dbTables.Ahnen1} ([Gen]);");
                        DataModul.TempDB.Execute($"CREATE INDEX Ahnen ON {dbTables.Ahnen1} ([Ahn]);");
                        DataModul.TempDB.Execute($"CREATE Table {dbTables.Nachk} (Gen integer,Nr TEXT(240)Not NULL,Pr long);");
                        DataModul.TempDB.Execute($"CREATE UNIQUE INDEX PerNr ON {dbTables.Nachk} ([Pr]);");
                        DataModul.TempDB.Execute($"CREATE INDEX Nr ON {dbTables.Nachk} ([Nr]);");
                        DataModul.TempDB.Execute($"CREATE INDEX GNr ON {dbTables.Nachk} ([Gen],[Nr]);");
                        DT_NachkTable = DataModul.TempDB.OpenRecordset(dbTables.Nachk, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DSB_SearchTable = DataModul.DSB.OpenRecordset(dbTables.Such, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DOSB_OrtSTable = DataModul.DOSB.OpenRecordset(dbTables.OrtSuch, RecordsetTypeEnum.dbOpenTable);
                        obj = DataModul.TempDB.OpenRecordset(dbTables.Konf, RecordsetTypeEnum.dbOpenTable);
                        DT_AhnTable = DataModul.TempDB.OpenRecordset(dbTables.Ahnen1, RecordsetTypeEnum.dbOpenTable);
                        DT_AhnTable.Index = "PerNr";
                        if (Menue_Ziel.Trim() != "")
                        {
                            DataModul.MandDB.Execute($"ALTER Table {dbTables.Personen} ADD COLUMN Such1 Text(240)NULL;");
                            DataModul.MandDB.Execute($"CREATE INDEX FamSu1 ON {dbTables.Tab} (  [FamNr],[Kennz]);");
                            DataModul.MandDB.Execute($"CREATE INDEX DatInd ON {dbTables.Ereignis} ([DatumV]);");
                            DataModul.MandDB.Execute($"ALTER Table {dbTables.Bilder} ADD COLUMN Format Text(1)NULL;");
                            DataModul.MandDB.Execute($"CREATE INDEX PerKenn ON {dbTables.Bilder} ([Kennz],[ZuNr]);");
                            DataModul.MandDB.Execute($"CREATE INDEX Bild ON {dbTables.Bilder} ([Datei]);");
                            DataModul.MandDB.Execute($"CREATE Table {dbTables.Quellen} (1 Long )");
                            DataModul.MandDB.Execute($"ALTER Table {dbTables.Quellen} ADD COLUMN 2 Text(240);");
                            DataModul.MandDB.Execute($"ALTER Table {dbTables.Quellen} ADD COLUMN 3 Text(240)");
                            DataModul.MandDB.Execute($"ALTER Table {dbTables.Quellen} ADD COLUMN 4 text(240)");
                            DataModul.MandDB.Execute($"ALTER Table {dbTables.Quellen} ADD COLUMN 5 text(240)");
                            DataModul.MandDB.Execute($"ALTER Table {dbTables.Quellen} ADD COLUMN 6 Text(240)");
                            DataModul.MandDB.Execute($"ALTER Table {dbTables.Quellen} ADD COLUMN 7 Text(240)");
                            DataModul.MandDB.Execute($"ALTER Table {dbTables.Quellen} ADD COLUMN 8 Text(240)");
                            DataModul.MandDB.Execute($"ALTER Table {dbTables.Quellen} ADD COLUMN 9 Text(240)");
                            DataModul.MandDB.Execute($"ALTER Table {dbTables.Quellen} ADD COLUMN 10 Text(240)");
                            DataModul.MandDB.Execute($"ALTER Table {dbTables.Quellen} ADD COLUMN 11 Text(240)");
                            DataModul.MandDB.Execute($"ALTER Table {dbTables.Quellen} ADD COLUMN 12 Text(240)");
                            DataModul.MandDB.Execute($"ALTER Table {dbTables.Quellen} ADD COLUMN 13 Memo");
                            DataModul.MandDB.Execute($"CREATE INDEX Nr ON {dbTables.Quellen} ([1]);");
                            DataModul.MandDB.Execute($"CREATE INDEX Nam ON {dbTables.Quellen} ([2]);");
                            DataModul.MandDB.Execute($"CREATE unique INDEX ZITAT ON {dbTables.Quellen} ([4]) WITH DISALLOW NULL;");
                            DataModul.MandDB.Execute($"ALTER Table {dbTables.Personen} ADD COLUMN Bem2 Memo;");
                            DataModul.MandDB.Execute($"ALTER Table {dbTables.Personen} ADD COLUMN Bem3 Memo;");
                            DataModul.MandDB.Execute($"ALTER Table {dbTables.Personen} ADD COLUMN OFB Text(1)NULL;");
                            if (Behand == 1)
                            {
                                DataModul.MandDB.Execute($"ALTER Table {dbTables.Personen} DROP COLUMN Konv;");
                                DataModul.MandDB.Execute($"ALTER Table {dbTables.Personen} ADD COLUMN Konv Text(240)NULL;");
                            }
                            goto IL_130b;
                        }
                        goto IL_180d;
                    IL_130b:
                        num = 225;
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Personen} ADD COLUMN religi integer;");
                        DataModul.MandDB.Execute($"CREATE INDEX reli ON {dbTables.Personen} ([religi])WITH  IGNORE NULL;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Familie} ADD COLUMN Bem2 Memo;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Familie} ADD COLUMN Bem3 Memo;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Ereignis} ADD COLUMN Bem3 Memo;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Ereignis} ADD COLUMN Bem4 Memo;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Ereignis} ADD COLUMN ArtText Text(240);");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Ereignis} ADD COLUMN Zusatz Text(240);");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Ereignis} ADD COLUMN priv Text(1);");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Ereignis} ADD COLUMN tot Text(1)NULL;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Familie} ADD COLUMN Eltern single;");
                        DataModul.MandDB.Execute($"CREATE Table {dbTables.Tab1} (1 single )");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Tab1} ADD COLUMN 2 long;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Tab1} ADD COLUMN 3 long;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Tab1} ADD COLUMN 4 Text(240);");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Tab1} ADD COLUMN LFNR single;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Tab1} ADD COLUMN Art single;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Tab1} ADD COLUMN Aus Text(240);");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Tab1} ADD COLUMN ORIG Memo;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Tab1} ADD COLUMN Kom Memo Null");
                        DataModul.MandDB.Execute($"CREATE INDEX Tab ON {dbTables.Tab1} ([1],[2]);");
                        DataModul.MandDB.Execute($"CREATE  INDEX Tab21 ON {dbTables.Tab1} ([1],[2],[3]);");
                        DataModul.MandDB.Execute($"CREATE  INDEX Tab22 ON {dbTables.Tab1} ([1],[2],[Art],[LFNR]);");
                        DataModul.MandDB.Execute($"CREATE  INDEX Tab23 ON {dbTables.Tab1} ([1],[2],[3],[Art],[LFNR]);");
                        DataModul.MandDB.Execute($"CREATE INDEX Tab ON {dbTables.Tab1} ([1],[2]);");
                        DataModul.MandDB.Execute($"CREATE INDEX Tab1 ON {dbTables.Tab1} ([1],[2],[3]);");
                        DataModul.MandDB.Execute($"CREATE INDEX Tab2 ON {dbTables.Tab1} ([3]);");
                        DataModul.MandDB.Execute($"CREATE INDEX STexte ON {dbTables.Texte} (  [Kennz],[Txt]);");
                        DataModul.MandDB.Execute($"CREATE INDEX RTexte ON {dbTables.Texte} (  [TxNr],[Kennz]);");
                        DataModul.MandDB.Execute($"CREATE UNIQUE INDEX Nr ON {dbTables.Bilder} ([LfNr]);");
                        DataModul.MandDB.Execute($"CREATE Table {dbTables.Nachk} (Gen integer);");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Nachk} add column Nr TEXT(240) Not NULL;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Nachk} add column Pr long;");
                        DataModul.MandDB.Execute($"CREATE UNIQUE INDEX PerNr ON {dbTables.Nachk} ([Pr]);");
                        DataModul.MandDB.Execute($"CREATE INDEX Nr ON {dbTables.Nachk} ([Nr]);");
                        DataModul.MandDB.Execute($"CREATE INDEX Per ON {dbTables.Tab} ([PerNr]);");
                        DataModul.MandDB.Execute($"CREATE INDEX NText ON {dbTables.Ereignis} ([ArtText]);");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Orte} ADD COLUMN Zusatz Text(240)NULL;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Orte} ADD COLUMN GOV Text(20)NULL;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Orte} ADD COLUMN POLName Text(240)NULL;");
                        DataModul.MandDB.Execute($"CREATE INDEX LTexte ON {dbTables.Texte} (  [Leitname],[Kennz]);");
                        DataModul.MandDB.Execute($"CREATE Table {dbTables.IndNam} ([PerNr] Long )");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.IndNam} ADD COLUMN [Kennz] Text(2);");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.IndNam} ADD COLUMN [Textnr] long;");
                        DataModul.MandDB.Execute($"CREATE INDEX Indnum ON {dbTables.IndNam} ([TextNr]);");
                        DataModul.MandDB.Execute($"CREATE INDEX Indn ON {dbTables.IndNam} ([PerNr],[Kennz],[TextNr]);");
                        DataModul.MandDB.Execute($"CREATE INDEX Indnr ON {dbTables.IndNam} ([PerNr],[Kennz]);");
                        Schalt = 1;
                        goto IL_180d;
                    IL_180d: // <========== 3
                        num = 274;
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Personen} ADD COLUMN Puid Text(40)NULL;");
                        DataModul.MandDB.Execute($"CREATE INDEX Puid ON {dbTables.Personen} (Puid);");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Familie} ADD COLUMN Fuid Text(40)NULL;");
                        DataModul.MandDB.Execute($"CREATE INDEX Fuid ON {dbTables.Familie} (Fuid);");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Ereignis} ADD COLUMN Zusatz Text(240);");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Ereignis} ADD COLUMN DatumText integer;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Ereignis} ADD COLUMN causal integer;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Ereignis} ADD COLUMN an integer;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Ereignis} ADD COLUMN Hausnr integer;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Ereignis} ADD COLUMN tot Text(1)NULL;");
                        DataModul.DB_QuTable = DataModul.MandDB.OpenRecordset(dbTables.Quellen, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_SourceLinkTable = DataModul.MandDB.OpenRecordset(dbTables.Tab1, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_OFBTable = DataModul.MandDB.OpenRecordset(dbTables.IndNam, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_PersonTable = DataModul.MandDB.OpenRecordset(dbTables.Personen, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_PersonTable.Index = "PerNr";
                        DataModul.MandDB.Execute($"CREATE Table {dbTables.Tab2} (FamNr long )");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Tab2} ADD COLUMN PerNr long;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Tab2} ADD COLUMN Kennz Text(2);");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Tab2} ADD COLUMN Art Single;");
                        DataModul.MandDB.Execute($"ALTER Table {dbTables.Tab2} ADD COLUMN LFNR Single;");
                        DataModul.MandDB.Execute($"CREATE  INDEX Fampruef ON {dbTables.Tab2} (  [FamNr],[PerNr],[Kennz],[Art],[LFNR]);");
                        DataModul.MandDB.Execute($"CREATE INDEX Zeug ON {dbTables.Tab2} ([FamNr],[PerNr],[Kennz],[Art],[LFNR]);");
                        DataModul.MandDB.Execute($"CREATE INDEX Zeugsu ON {dbTables.Tab2} ([FamNr],[Kennz],[Art],[LFNR]);");
                        DataModul.MandDB.Execute($"CREATE INDEX Famsu ON {dbTables.Tab2} ([FamNr],[Kennz]);");
                        DataModul.MandDB.Execute($"CREATE INDEX Elsu ON {dbTables.Tab2} ([PerNr],[Kennz]);");
                        DataModul.DB_WitnessTable = DataModul.MandDB.OpenRecordset(dbTables.Tab2, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_FamilyTable = DataModul.MandDB.OpenRecordset(dbTables.Familie, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_NameTable = DataModul.MandDB.OpenRecordset(dbTables.INamen, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_PictureTable = DataModul.MandDB.OpenRecordset(dbTables.Bilder, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DSB_SearchTable = DataModul.DSB.OpenRecordset(dbTables.Such, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DSB_SearchTable.Index = "Persuch";
                        DataModul.DB_TexteTable = DataModul.MandDB.OpenRecordset(dbTables.Texte, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_TexteTable.Index = "ITexte";
                        DataModul.DB_PlaceTable = DataModul.MandDB.OpenRecordset(dbTables.Orte, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_PlaceTable.Index = "Orte";
                        DataModul.DB_EventTable = DataModul.MandDB.OpenRecordset(dbTables.Ereignis, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_EventTable.Index = "ArtNr";
                        DataModul.DB_LinkTable = DataModul.MandDB.OpenRecordset(dbTables.Tab, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_LinkTable.Index = "ElSu";
                        DT_AhnTable = DataModul.TempDB.OpenRecordset(dbTables.Ahnen1, RecordsetTypeEnum.dbOpenTable);
                        DT_AhnTable.Index = "PerNr";
                        DataModul.DOSB_OrtSTable = DataModul.DOSB.OpenRecordset(dbTables.OrtSuch);
                        goto end_IL_0000_2;
                    //==================================================
                    IL_1dd6:
                        num = 327;
                        int number = Information.Err().Number;
                        if (number == 53)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_237f;
                        }
                        if (number == 55)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_237f;
                        }
                        if (number == 70)
                        {
                            Interaction.MsgBox("Sie können nicht die Daten des aktuellen Mandanten löschen.Bitte wählen Sie erst einen anderen Mandanten oder geben Sie dem neuen Mandanten einen anderen Namen", MsgBoxStyle.OkOnly, "Bedienungsfehler");
                            DataModul.MandDB.Close();
                            ProjectData.EndApp();
                            lErl = 6111;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_237f;
                        }
                        if (number == 75)
                        {
                            Besttest = DataModul_PeekMandant_RecordCount(Verz);
                            DataModul.MandDB.Close();
                            if (Besttest == 0)
                            {
                                Ubg = 1;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_237f;
                            }
                            MyProject.Forms.Gene.Close();
                            if (Ubg == 0)
                            {
                                MyProject.Forms.BestGenBehandeln.ShowDialog();
                                Behand = checked((int)Math.Round(MyProject.Forms.BestGenBehandeln.Bezubg.Text.AsDouble()));
                            }
                            else
                            {
                                MyProject.Forms.BestGenBehandeln.Close();
                            }
                            goto IL_1f85;
                        }
                        if (number == 91)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_237f;
                        }
                        if (number == 3010)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_237f;
                        }
                        if (number == 3011)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_237f;
                        }
                        if (number == 3024)
                        {
                            goto end_IL_0000_2;
                        }
                        if (number == 3204)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_237f;
                        }
                        if (number == 3211)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_237f;
                        }
                        if (number == 3262)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_237f;
                        }
                        if (number == 3375)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_237f;
                        }
                        if (number == 3380)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_237f;
                        }
                        if (number == 3420)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_237f;
                        }
                        Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                        if (Interaction.MsgBox(Mldg, MsgBoxStyle.OkCancel | MsgBoxStyle.Question, "Fehler") == MsgBoxResult.Cancel)
                        {
                            ProjectData.EndApp();
                        }
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_237b;
                    IL_1f85: // <========== 3
                        num = 360;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_237f;
                    IL_2276:
                        num = 413;
                        if (Information.Err().Number == 53)
                        {
                            Interaction.MsgBox("Demoversion");
                            System.xDemo = true;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num2 = 0;
                            goto IL_00da;
                        }
                        Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                        if (Interaction.MsgBox(Mldg, MsgBoxStyle.OkCancel | MsgBoxStyle.Question, "Fehler") == MsgBoxResult.Cancel)
                        {
                            ProjectData.EndApp();
                        }
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_237b;
                    IL_237b:
                        num4 = num2;
                        goto IL_2383;
                    IL_237f: // <========== 18
                        num4 = num2 + 1;
                        goto IL_2383;
                    IL_2383:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 13:
                            case 417:
                                goto IL_00da;
                            case 28:
                            case 29:
                                goto IL_01ba;
                            case 46:
                            case 49:
                            case 50:
                                goto IL_02ba;
                            case 63:
                            case 64:
                            case 65:
                                goto IL_036f;
                            case 81:
                            case 84:
                            case 85:
                                goto IL_0429;
                            case 31:
                            case 53:
                            case 69:
                            case 74:
                            case 87:
                            case 90:
                            case 91:
                                goto IL_044f;
                            case 20:
                            case 118:
                                goto IL_0723;
                            case 126:
                            case 129:
                            case 130:
                            case 224:
                            case 225:
                                goto IL_130b;
                            case 273:
                            case 274:
                                goto IL_180d;
                            case 356:
                            case 359:
                            case 360:
                                goto IL_1f85;
                            case 73:
                            case 88:
                            case 89:
                            case 316:
                            case 376:
                            case 424:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj2) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj2, lErl);
                try0000_dispatch = 10801;
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

    public void OFBTextPruefenSpeichern(string UbgT, string Kennz, int LfNR)
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
                    int num4;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            goto IL_0009;
                        case 2097:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_069f;
                                    default:
                                        goto end_IL_0000;
                                }
                                int number = Information.Err().Number;
                                if (number == 3021)
                                {
                                    Einles.Satz = 1;
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num2 = 0;
                                    goto IL_0009;
                                }
                                if (number == 3022)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_069f;
                                }
                                if (number == 3052)
                                {
                                    Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                                    if (Interaction.MsgBox(Mldg, MsgBoxStyle.OkCancel | MsgBoxStyle.Question, "Fehler") == MsgBoxResult.Cancel)
                                    {
                                        ProjectData.EndApp();
                                    }
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_069b;
                                }
                                Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                                if (Interaction.MsgBox(Mldg, MsgBoxStyle.OkCancel | MsgBoxStyle.Question, "Fehler") == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_069b;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            num = 2;
                            UbgT = UbgT.Replace("ß", "ssss");
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            Einles.Satz = 0;
                            if (Kennz is "Y" or "NN")
                            {
                                eTKennz = ETextKennz.tkName;
                            }
                            else
                            if (Kennz is "EE")
                            {
                                eTKennz = ETextKennz.E_;
                            }
                            else
                                eTKennz = default;

                            if (UbgT.Trim() == "")
                            {
                                Einles.Satz = 0;
                                goto end_IL_0000_2;
                            }
                            Einles.Satz = DataModul.Texte_Schreib(UbgT.Trim(), "", eTKennz);
                            UbgT = "";
                            string kennz = Kennz;
                            if (kennz == "Y")
                            {
                                DataModul.Names.Update(PersInArb, Einles.Satz, eTKennz, 0);
                            }
                            else
                            if (kennz == "NN" || kennz == "EE")
                            {
                                DataModul.OFB.Update(Kennz, PersInArb, Einles.Satz);
                            }
                            goto end_IL_0000_2;
                        IL_069b:
                            num4 = num2;
                            goto IL_06a3;
                        IL_069f:
                            num4 = unchecked(num2 + 1);
                            goto IL_06a3;
                        IL_06a3:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 25:
                                case 43:
                                case 53:
                                case 63:
                                case 64:
                                case 65:
                                case 66:
                                case 67:
                                case 69:
                                case 75:
                                case 78:
                                case 79:
                                case 86:
                                case 87:
                                case 94:
                                case 95:
                                case 96:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 2097;
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


    public int Person_TextSpeichern(string sText, int iPerson, ETextKennz eKennz1, int iLfNR = 0, short Ruf = 0)
    {
        if (sText.Trim() != "")
        {
            var Satz = DataModul.Texte_Schreib(sText, "", eKennz1);
            switch (eKennz1)
            {
                case ETextKennz.A_:
                case ETextKennz.B_:
                case ETextKennz.C_:
                case ETextKennz.D_:
                case ETextKennz.tkName:
                case ETextKennz.U_:
                case ETextKennz.tk2_:
                case ETextKennz.tk3_:
                    DataModul.Names.Update(iPerson, Satz, eKennz1, 0);
                    break;
                case ETextKennz.V_:
                case ETextKennz.F_:
                    DataModul.Names.Update(iPerson, Satz, eKennz1, iLfNR, Ruf % 1 !=0 , Ruf % 2!=0);
                    break;
                default:
                    break;
            }
            return Satz;
        }
        else
            return 0;
    }


    public string Strngs_Umlaute4(string Fld, int uml)
    {
        bool IsLowerCase(string s) => !string.IsNullOrWhiteSpace(s) && char.IsLower(s[0]);

        switch (uml)
        {
            default:
                return Fld;
            case 2:
                IList<string[]> replacements2 = [
                        [ "Ä", "AE", "Ae" ],
                            [ "ä", "ae" ],
                            [ "Ö" , "OE", "Oe"],
                            [ "ö" , "oe" ],
                            [ "Ü" , "UE", "Ue"],
                            [ "ü" , "ue" ],
                            [ "ß" , "ss" ]
                ];

                foreach (var replacement in replacements2)
                {
                    if (replacement.Length > 2)
                        while (Fld.Contains(replacement[0]))
                        {
                            var iIdx = Fld.IndexOf(replacement[0]);
                            if ((Fld.Length > iIdx + replacement[0].Length)
                                && IsLowerCase(Fld.Substring(iIdx + replacement[0].Length, 1)))
                                Fld = Fld.Substring(0, iIdx) + replacement[2] + Fld.Substring(iIdx + replacement[0].Length);
                            else
                                Fld = Fld.Substring(0, iIdx) + replacement[1] + Fld.Substring(iIdx + replacement[0].Length);
                        }
                    else
                        Fld.Replace(replacement[0], replacement[1]);
                }

                return Fld;
            case 1:
                IList<string[]> replacements = [
                    ["ü", $"{(char)129}"],
                    ["é", $"{(char)130}"],
                    ["â", $"{(char)131}"],
                    [ "ä", $"{(char)132}"],
                    ["à", $"{(char)133}"],
                    ["å", $"{(char)134}"],
                    ["ç", $"{(char)135}"],
                    ["ê", $"{(char)136}"],
                    ["ë", $"{(char)137}"],
                    ["è", $"{(char)138}"],
                    ["ï", $"{(char)139}"],
                    ["î", $"{(char)140}"],
                    ["ì", $"{(char)141}"],
                    [ "Ä", $"{(char)142}" ],
                    ["Å", $"{(char)143}"],
                    ["É", $"{(char)144}"],

                    ["ô", $"{(char)147}"],
                    [ "ö", $"{(char)148}"],
                    ["ò", $"{(char)149}"],
                    ["û", $"{(char)150}"],
                    ["ù", $"{(char)151}"],
                    ["ÿ", $"{(char)152}"],
                    [ "Ö", $"{(char)153}"],

                    ["á", $"{(char)160}"],
                    ["í", $"{(char)161}"],
                    [ "ó", $"{(char)162}" ],
                    [ "ú", $"{(char)163}" ],
                    [ "ñ", $"{(char)164}" ],
                    [ "Ñ", $"{(char)165}" ],

                    [ "ß", $"{(char)225}" ],
               ];

                int index;
                while ((index = Fld.IndexOf('Ü')) != -1)
                {
                    string replacement = (index + 1 < Fld.Length && char.IsLower(Fld[index + 1])) ? "Ue" : "UE";
                    Fld = string.Concat(Fld.Substring(0, index), replacement, Fld.Substring(index + 1));
                }

                foreach (string[] replacement in replacements)
                {
                    Fld = Fld.Replace(replacement[0], replacement[1]);
                }
                return Fld;

        }
    }

    public IGenPersistence Persistence { get; } = new CPersistence();
    
    byte IModul1.Schalt { get => field; set => throw new NotImplementedException(); }
    byte IModul1.Suchschalt { get => field; set => throw new NotImplementedException(); }


    public float Aschalt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Color Feld1Farb { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Color ErFarb { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Color Hintfarb1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public short Feg { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IList<string> DAus => throw new NotImplementedException();

    public IList<string> Quells => throw new NotImplementedException();

    public IList<ESearchSelection> Suchfeld => throw new NotImplementedException();

    public bool EreiRf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public string Version2 => throw new NotImplementedException();

    public string Titel2 => throw new NotImplementedException();

 
    public int thisYear => throw new NotImplementedException();

    public byte Programtesttemp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool FAendmerk { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool PAendmerk { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Lw { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float Aend { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float FontSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool Aendf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int PFSatz { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int Startpers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public IPersonData Person { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int eWKennz { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public short Qkenn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public string AppHostName => throw new NotImplementedException();

    public Enum eWindowState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public EWindowSize eWindowSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


    public string AltName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int AltNr { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string sDatu { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int Datklein { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public short Druck_Tast { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public string Message_sNoChangesOnCD => throw new NotImplementedException();

    public int Suchfam { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int SuchPer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public short Trans { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Inhaber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
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

    public bool Ad { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Ind1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IModul1.Frauen Frauen_Renamed => Frauen_Renamed;

    public IList<short> Posi { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Job { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool reorga { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public string Message_sDemoVerNotPossibl => throw new NotImplementedException();

    public int PersInArbsp => throw new NotImplementedException();

    public string sGeocodeXMLAddress => throw new NotImplementedException();

    DriveInfo IModul1.cMandDrive { get => cMandDrive; set => throw new NotImplementedException(); }
    int IModul1.Histor { get => Histor; set => throw new NotImplementedException(); }
    int IModul1.Quell { get => Quell; set => throw new NotImplementedException(); }
    int IModul1.FeG { get => FeG; set => throw new NotImplementedException(); }
    short IModul1.LfNR { get => throw new NotImplementedException(); set => LfNR = value; }
    public IModul1.Letzter Letzte { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public byte Datschalt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IProjectData ProjectData => throw new NotImplementedException();

    public IVBInformation Information => throw new NotImplementedException();

    public IVBConversions Conversions => throw new NotImplementedException();

    public IStrings Strings => throw new NotImplementedException();

    public IOperators Operators => throw new NotImplementedException();

    public ISystem System => throw new NotImplementedException();

    public long Kek { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Perles(int PersInArb)
    {
        //Discarded unreachable code: IL_086d
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string text = default;
        string[] array = default;
        short num5 = default;
        int[] array2 = default;
        int num6 = default;
        int num7 = default;
        int num8 = default;
        int num9 = default;
        int num12 = default;
        int num13 = default;
        int num14 = default;
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
                    int num10;
                    int num11;
                    Type typeFromHandle;
                    object[] array3;
                    object[] array4;
                    bool[] array5;
                    string left2;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 2763:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                    case 3:
                                        break;
                                    case 1:
                                        goto IL_0871;
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
                                    goto IL_0871;
                                }
                                Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                                if (Interaction.MsgBox(Mldg, MsgBoxStyle.OkCancel | MsgBoxStyle.Question, "Fehler") == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0871;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            text = "";
                            ProjectData.ClearProjectError();
                            num3 = 3;
                            array2 = new int[16];
                            array = new string[16];
                            num5 = 0;
                            while (num5 <= 11)
                            {
                                Kont[num5] = "";
                                Kont1[num5] = "";
                                num5 = (short)unchecked(num5 + 1);
                            }
                            DataModul.DB_NameTable.Index = "PNamen";
                            DataModul.DB_NameTable.Seek("=", PersInArb);
                            if (DataModul.DB_NameTable.NoMatch)
                            {
                                goto end_IL_0000_2;
                            }
                            num5 = 1;
                            while (num5 <= 99
                                && !DataModul.DB_NameTable.EOF
                            && !DataModul.DB_NameTable.NoMatch
                            && !(DataModul.DB_NameTable.Fields[NameFields.PersNr].AsInt() != PersInArb)
                            && !DataModul.DB_NameTable.NoMatch)
                            {
                                Kont1[0] = DataModul.DB_NameTable.Fields[NameFields.Kennz].AsString();
                                Kont1[1] = DataModul.DB_NameTable.Fields[NameFields.Text].AsString();
                                Kont1[2] = DataModul.DB_NameTable.Fields[NameFields.LfNr].AsString();
                                Kont1[3] = DataModul.DB_NameTable.Fields[NameFields.Ruf].AsString();
                                string left = Kont1[0];
                                switch (left)
                                {
                                    case "A":
                                        num6 = Kont1[1].AsInt();
                                        break;
                                    case "B":
                                        num7 = Kont1[1].AsInt();
                                        break;
                                    case "C":
                                        num8 = Kont1[1].AsInt();
                                        break;
                                    case "D":
                                        num9 = Kont1[1].AsInt();
                                        break;
                                    case "E":
                                        num10 = Kont1[1].AsInt();
                                        break;
                                    case "G":
                                        num11 = Kont1[1].AsInt();
                                        break;
                                    case "N":
                                        num12 = Kont1[1].AsInt();
                                        break;
                                    case "V":
                                    case "F":
                                        if (Kont1[2].AsInt() <= 15)
                                        {
                                            array2[Kont1[2].AsInt()] = Kont1[1].AsInt();
                                            array[Kont1[2].AsInt()] = Kont1[3];
                                        }
                                        break;
                                    case "U":
                                        num13 = Kont1[1].AsInt();
                                        break;
                                    case "2":
                                        num14 = Kont1[1].AsInt();
                                        break;
                                }
                                lErl = 12;
                                DataModul.DB_NameTable.MoveNext();
                                num5++;
                            }
                            goto IL_042b;
                        IL_042b: // <========== 4
                            num = 74;
                            DataModul.DB_TexteTable.Index = "TxNr";
                            if (num12 > 0)
                            {
                                NamenSuch_Wort = DataModul.TextLese1(num12);
                                Kont[0] = NamenSuch_Wort;
                                NamenSuch_Wort = "";
                            }
                            if (num6 > 0)
                            {
                                NamenSuch_Wort = DataModul.TextLese1(num6);
                                Kont[1] = NamenSuch_Wort;
                                NamenSuch_Wort = "";
                            }
                            if (num7 > 0)
                            {
                                NamenSuch_Wort = DataModul.TextLese1(num7);
                                Kont[2] = NamenSuch_Wort;
                                NamenSuch_Wort = "";
                            }
                            if (num8 > 0)
                            {
                                NamenSuch_Wort = DataModul.TextLese1(num8);
                                Kont[4] = NamenSuch_Wort;
                                NamenSuch_Wort = "";
                            }
                            Ubg = num9;
                            if (num9 > 0)
                            {
                                NamenSuch_Wort = DataModul.TextLese1(num9);
                                Kont[5] = NamenSuch_Wort;
                                NamenSuch_Wort = "";
                            }
                            goto IL_0571;
                        IL_0571:
                            num = 101;
                            if (num13 > 0)
                            {
                                NamenSuch_Wort = DataModul.TextLese1(num13);
                                Kont[6] = NamenSuch_Wort;
                                NamenSuch_Wort = "";
                            }
                            goto IL_05ad;
                        IL_05ad:
                            num = 106;
                            if (num14 > 0)
                            {
                                NamenSuch_Wort = DataModul.TextLese1(num14);
                                Kont[7] = NamenSuch_Wort;
                                NamenSuch_Wort = "";
                            }
                            goto IL_05e9;
                        IL_05e9:
                            num = 111;
                            num5 = 1;
                            goto IL_05ef;
                        IL_05ef: // <========== 3
                            num = 112;
                            Ubg = array2[num5];
                            if (Ubg > 0)
                            {
                                string str = "";
                                typeFromHandle = typeof(Strings);
                                array3 = new object[1];
                                array3[0] = DataModul.DB_PersonTable.Fields[PersonFields.Sex].Value;
                                array4 = array3;
                                array5 = new bool[1]
                                {
                                true
                                }
                                ;
                                left2 = NewLateBinding.LateGet(null, typeFromHandle, "UCase", array4, null, null, array5).AsString();
                                if (array5[0])
                                {
                                    DataModul.DB_PersonTable.Fields[PersonFields.Sex].Value = array4[0];
                                }
                                if (left2 == "F")
                                {
                                    NamenSuch_Wort = DataModul.TextLese1(Ubg);
                                }
                                else
                                    NamenSuch_Wort = DataModul.TextLese1(Ubg);
                                if (GedAus.Options_Mehrvorn)
                                {
                                    NamenSuch_Wort = NamenSuch_Wort.Replace(" ", "_");
                                }
                                str = NamenSuch_Wort;
                                NamenSuch_Wort = "";
                                if (array[num5] == "1")
                                {
                                    Kont[8] = str.TrimEnd();
                                }
                                text = text + str.TrimEnd() + " ";
                                num5 = (short)unchecked(num5 + 1);
                                if (num5 <= 15)
                                {
                                    goto IL_05ef;
                                }
                            }
                            Kont[3] = text;
                            goto end_IL_0000_2;
                        IL_0871: // <========== 3
                            num4 = unchecked(num2 + 1);
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 17:
                                case 20:
                                case 23:
                                case 69:
                                case 74:
                                    goto IL_042b;
                                case 100:
                                case 101:
                                    goto IL_0571;
                                case 105:
                                case 106:
                                    goto IL_05ad;
                                case 110:
                                case 111:
                                    goto IL_05e9;
                                case 112:
                                    goto IL_05ef;
                                case 13:
                                case 136:
                                case 146:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 2763;
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

    private void HistOld()
    {
        int A = 0;
        int num5 = 0;
        int lErl;
        var b = (byte)Math.Round(UbgT.Left(1).AsDouble() + 1.0);
        var text = "\r\n";
        var @string = "\n";
        var string2 = "\r";
        string text3 = "";
        (int, string)? text2;
        UbgT = UbgT.Replace("\v", " ");
        UbgT = UbgT.Replace(text, "\n");
        if (Strings.InStr(UbgT, text) != 0)
        {
            while (A > 0)
            {
                lErl = 1;
                A = (short)Strings.InStr(UbgT, text);
                if (A > 0)
                {
                    UbgT = UbgT.Left(A - 1) + "\n" + Strings.Mid(UbgT, A + 2, UbgT.Length);
                }
            }
        }
        if (Strings.InStr(UbgT, string2) != 0)
        {
            while (A > 0)
            {
                lErl = 4;
                A = (short)Strings.InStr(UbgT, string2);
                if (A > 0)
                {
                    UbgT = UbgT.Left(A - 1) + "\n" + Strings.Mid(UbgT, A + 2, UbgT.Length);
                }
            }
        }
        string Fld = "";
        if (Strings.InStr(UbgT, @string) != 0)
        {
            while (unchecked((0u - ((UbgT.Length > 80) ? 1u : 0u)) | (uint)Strings.InStr(UbgT, @string)) != 0)
            {
                if ((Strings.InStr(UbgT, @string) == 0) | (Strings.InStr(UbgT, @string) > 80))
                {
                    int u = num5;
                    A = Strings_Lerz(UbgT);
                    num5 = (short)u;
                    if (A == num5)
                    {
                        num5 = (short)(num5 - 2);
                    }
                    Fld = UbgT.Left(num5);
                    if (GedAus.Options_Uml > 0)
                    {
                        Fld = Strngs_Umlaute4(Fld, uml: GedAus.Options_Uml);
                    }
                    GedAus.WriteGedLine(Fld, true);
                    Fld = "";
                    UbgT = b.AsString().Trim() + " CONC " + Strings.Mid(UbgT, num5 + 1, UbgT.Length);
                    if (UbgT.Length <= 7)
                    {
                        goto end_IL_0000_2;
                    }
                }
                else
                {
                    Fld = UbgT.Left(Strings.InStr(UbgT, @string) - 1);
                    if (GedAus.Options_Uml > 0)
                    {
                        Fld = Strngs_Umlaute4(Fld, uml: GedAus.Options_Uml);
                    }
                    GedAus.WriteGedLine(Fld, true);
                    Fld = "";
                    UbgT = b.AsString().Trim() + " CONT " + Strings.Mid(UbgT, Strings.InStr(UbgT, @string) + 1, UbgT.Length);
                    if (UbgT.TrimEnd().Length <= 7)
                    {
                        goto end_IL_0000_2;
                    }
                }
            }
            if (GedAus.Options_Uml > 0)
            {
                UbgT = Strngs_Umlaute4(UbgT, uml: GedAus.Options_Uml);
            }
            GedAus.WriteGedLine(UbgT.Trim(), true);
            GedAus_Diskvoll();
            goto end_IL_0000_2;
        }
        while (UbgT.Length > 7)
        {
            lErl = 3;
            if (UbgT.Length > 80)
            {
                int u = num5;
                A = Strings_Lerz(UbgT);
                num5 = (short)u;
                if (A == num5)
                {
                    num5 = (short)(num5 - 2);
                }
                if (GedAus.Options_Uml > 0)
                {
                    UbgT = Strngs_Umlaute4(UbgT, uml: GedAus.Options_Uml);
                }
                GedAus.WriteGedLine(UbgT.Left(num5).TrimEnd(), false);
                if (UbgT.Left(1).AsDouble() <= 3.0)
                {
                    text2 = new TGedLine(UbgT).tLvlTag;
                    if (text2 == (2, cGed_SOUR) || text2 == (2, cGed_NOTE))
                    {
                        text3 = "3 ";

                    }
                    else if (text2 == (2, cGed_CONC) || text2 == (2, cGed_CONT))
                    {
                        text3 = "2 ";

                    }
                    else if (text2 == (3, cGed_CONT) || text2 == (3, cGed_CONC))
                    {
                        text3 = "3 ";

                    }
                    else if (text2 == (2, cGed_COMM))
                    {
                        text3 = "3 ";
                    }
                    else
                    {
                        switch (text2)
                        {
                            case (1, cGed_NOTE):
                            case (1, cGed_SOUR):
                            case (1, cGed_TEXT):
                            case (1, cGed_WITN):
                                text3 = "2 ";
                                break;
                            case (3, cGed_NOTE):
                            case (3, cGed_SOUR):
                            case (3, cGed_TEXT):
                                text3 = "4 ";
                                break;
                            default:
                                Interaction.MsgBox(UbgT.Left(6));
                                break;
                        }
                    }
                }
                UbgT = text3 + "CONC " + Strings.Mid(UbgT, num5 + 1, UbgT.Length);
                continue;
            }
            goto end_IL_0000_2;
        }
        if (GedAus.Options_Uml > 0)
        {
            UbgT = Strngs_Umlaute4(UbgT, uml: GedAus.Options_Uml);
        }
        GedAus.WriteGedLine(UbgT, true);
        UbgT = "";
        goto end_IL_0000_2;
    end_IL_0000_2: // <========== 6
        return;
    }

    public void GedAus_Diskvoll()
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        int num4;
        FileStream fileStream;
        string nDrive;
        int lpBytesPerSector;
        int lpNumberOfFreeClusters;
        int lpTtoalNumberOfClusters;
        long diskFreeSpace;
        switch (try0000_dispatch)
        {
            default:
                num = 1;
                SpDisk = SpDisk.Trim().Left(1);
                num = 2;
                FileSystem.FileClose(11);
                nDrive = SpDisk + ":\\";
                switch (new DriveInfo(SpDisk).DriveType)
                {
                    case DriveType.Removable:
                        goto IL_0076;
                    case DriveType.Fixed:
                        Interaction.MsgBox("Die Festplatte " + SpDisk + " ist voll. Vor einem erneuten Versuch müssen Sie Speicherplatz freimachen !");
                        ProjectData.EndApp();
                        goto end_IL_0000_2;
                    default:
                        break;
                }
                goto end_IL_0000_2;
            IL_0076: // <========== 3
                while (true)
                {
                    if (Interaction.MsgBox("Der Datenträger Laufwerk " + SpDisk + ": ist voll !\nBitte neue Diskette einlegen!\nFertig?", MsgBoxStyle.OkCancel, "") != MsgBoxResult.Cancel)
                        break;
                    else
                    if (Interaction.MsgBox("Wirklich Abbrechen??", MsgBoxStyle.YesNo, "") == MsgBoxResult.Yes)
                    {
                        ProjectData.EndApp();
                        goto end_IL_0000_2;
                    }
                }
                ProjectData.ClearProjectError();
                num3 = 2;
                nDrive = SpDisk + ":\\";
                lpBytesPerSector = (int)bytes;
                lpNumberOfFreeClusters = (int)freeclusters;
                lpTtoalNumberOfClusters = (int)clusters;
                diskFreeSpace = new DriveInfo(nDrive).AvailableFreeSpace;
                clusters = lpTtoalNumberOfClusters;
                freeclusters = lpNumberOfFreeClusters;
                bytes = lpBytesPerSector;
                ok = (short)diskFreeSpace;
                if (ok != 0)
                {
                    Free = sectors * bytes * freeclusters;
                }
                else
                    Free = 0L;
                Gedaus_DiskSize = (long)Math.Round(Free - Free / 10.0);
                d_ = 0;
                _Diskvoll_LfDisk++;
                _Diskvoll_Fn = (short)Strings.InStr(GedAus.FILENAM, ".");
                string text2 = "G" + Strings.Right("00" + Strings.Trim(Conversion.Str(unchecked(_Diskvoll_LfDisk) - 1)), 2);
                GedAus.FILENAM = GedAus.FILENAM.Left(_Diskvoll_Fn) + text2;
                FileSystem.FileOpen(11, SpDisk + ":\\" + GedAus.FILENAM, OpenMode.Output);
                string text = "Test.ged";
                fileStream = new FileStream(SpDisk + ":\\" + text, FileMode.Create);
                goto end_IL_0000_2;
        }
    end_IL_0000_2: // <========== 5
        return;
    }

    public void Datles(int Ubg, int PersInArb)
    {
        //Discarded unreachable code: IL_089f
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        short num7 = default;
        int num8 = default;
        string Datu = default;
        string Ds = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    string text3;
                    string text2;
                    string text;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            text = "";
                            goto IL_000a;
                        case 2641:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_08a3;
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
                                    goto IL_08a3;
                                }
                                Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                                if (Interaction.MsgBox(Mldg, MsgBoxStyle.OkCancel | MsgBoxStyle.Question, "Fehler") == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_08a3;
                            }
                        end_IL_0000:
                            break;
                        IL_000a:
                            num = 2;
                            text2 = "";
                            short num5 = 0;
                            short num6 = 0;
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            byte b = 1;
                            while (b <= 25u)
                            {
                                Kont[b] = "";
                                b++;
                            }
                            b = 101;
                            while (b <= 107u)
                            {
                                byte b2 = 0;
                                while (b2 <= 15u)
                                {
                                    Kont1[b2] = "";
                                    b2++;
                                }
                                Ubg = b;
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", Ubg, PersInArb, "0");
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.priv].Value))
                                    {
                                        if (DataModul.DB_EventTable.Fields[EventFields.priv].AsInt() > GedAus.Options_priv.AsInt())
                                        {
                                            b++;
                                            continue;
                                        }
                                    }
                                    if (num7 == 2)
                                    {
                                        if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsInt() == num8)
                                        {
                                            num7 = 3;
                                            goto end_IL_0000_2;
                                        }
                                        b++;
                                        continue;
                                    }
                                    if (DataModul.DB_EventTable.Fields[EventFields.DatumV].AsInt() > 0)
                                    {
                                        Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                                        text3 = Conversion.Str(Datu.Left(2).AsDouble() + 1.0);
                                        if (num7 == 1)
                                        {
                                            Kont[Ubg - 100] = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                                            b++;
                                            continue;
                                        }
                                        if (DataModul.DB_EventTable.Fields[EventFields.VChr].AsInt() > 0)
                                        {
                                            Datu += " B.C.";
                                        }
                                        Ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                        Datwand1(ref Datu, ref Ds);
                                        Kont1[1] = Datu;
                                        if (Ubg == 101)
                                        {
                                            text2 = ("           " + Datu).Right(10);
                                            num5 = 1;
                                        }
                                        else
                                        if (Ubg == 102 && num5 == 0)
                                        {
                                            text2 = ("           " + Datu).Right(10);
                                        }
                                        else
                                        if (Ubg == 103)
                                        {
                                            text = ("           " + Datu).Right(10);
                                            num6 = 1;
                                        }
                                        else
                                        if (Ubg == 104 && num6 == 0)
                                        {
                                            text = ("           " + Datu).Right(10);
                                        }
                                    }
                                    if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() != 0)
                                    {
                                        Datu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString().Trim(), 8);
                                        Ds = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                        Datwand1(ref Datu, ref Ds);
                                        Kont1[3] = Datu;
                                    }
                                    UbgT = "";
                                    if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                                    {
                                        Ortnr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble());
                                        DataModul.DB_PlaceTable.Index = "OrtNr";
                                        DataModul.DB_PlaceTable.Seek("=", Ortnr);
                                        if (!DataModul.DB_PlaceTable.NoMatch)
                                        {
                                            UbgT = DataModul.TextLese1(DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt());
                                            UbgT = UbgT + "-" + DataModul.TextLese1(DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt());
                                        }
                                    }
                                    Kont[Ubg - 90] = Kont1[1] + " " + Kont1[2] + Kont1[3] + Kont1[4] + Kont1[5] + Kont1[7] + " " + UbgT + Kont1[6];
                                    UbgT = "";
                                    Kont[Ubg - 80] = Kont1[1];
                                }
                                lErl = 2;
                                b++;
                            }
                            goto end_IL_0000_2;
                        IL_08a3: // <========== 3
                            num4 = unchecked(num2 + 1);
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 27:
                                case 87:
                                case 89:
                                case 93:
                                case 94:
                                case 101:
                                case 102:
                                case 103:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 2641;
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
    #region GedAus - Methods


    #endregion
    [Obsolete("Auftrennen in 3 Funktionen")]
    public int Persatzles(int PersInArb)
    {
        int MaxPersID = DataModul.Person.MaxID;
        _ = DataModul.Person.Seek(PersInArb);
        PersInArb = DataModul.Person.ValidateID(PersInArb, Schalt, MaxPersID, MsgBoxResult.Yes,
                        (i) => Interaction.MsgBox("Diese Person ist gelöscht und kann neu belegt werden", MsgBoxStyle.Exclamation | MsgBoxStyle.YesNo | MsgBoxStyle.DefaultButton2, "Jetzt neu eingeben"));

        if (PersInArb == 0)
            return 0;

        return PersInArb;
    }


    public void Datwand1(ref string Datu, ref string Ds)
    {
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
                    byte b = (byte)Strings.InStr(Datu, " ");
                    if (b > 0)
                    {
                        StringType.MidStmtStr(ref Datu, b, 1, ".");
                    }
                    num = (short)unchecked(num + 1);
                }
                while (num <= 2);
            }
            if (Ds != "")
            {
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
                    default:
                        Datu = Datu + " " + Ds.Trim();
                        break;
                }
                Ds = "";
            }
        }
    }

 
    public string Conform(string sText)
    {
        if (sText.Left(15) == "1 " + cGed_FAMILY + "_SPOUSE")
        {
            sText = "1 " + cGed_FAMS + Strings.Mid(sText, 16, sText.Length);
        }
        else if (sText.Right(10) == "INDIVIDUAL")
        {
            byte length = (byte)Strings.InStr(sText, "INDIVIDUAL");
            sText = sText.Left(length) + cGed_INDI;
        }
        else if (sText.Left(14) == "1 " + cGed_FAMILY + "_CHILD")
        {
            sText = "1 " + cGed_FAMC + Strings.Mid(sText, 15, sText.Length);
        }
        else if (sText.Left(9) == "1 BAPTISM")
        {
            sText = "1 " + cGed_BAPM + Strings.Mid(sText, 15, sText.Length);
        }
        else if (sText.Left(7) == "2 " + cGed_PLACE)
        {
            sText = "2 " + cGed_PLAC + sText;
        }
        else if (sText == "1 RELIGION")
        {
            sText = "1 " + cGed_RELI;
        }
        else if (sText == "1 " + cGed_OCCUP)
        {
            sText = "1 " + cGed_OCCU;
        }
        else if (sText.Right(6) == cGed_FAMILY)
        {
            byte length = (byte)Strings.InStr(sText, cGed_FAMILY);
            sText = Strings.Left(sText, unchecked(length) - 1) + cGed_FAM;
        }
        else if (sText.Left(9) == "1 HUSBAND")
        {
            sText = "1 " + cGed_HUSB + Strings.Mid(sText, 10, sText.Length);
        }
        else if (sText.Left(7) == "1 " + cGed_CHILD)
        {
            sText = "1 " + cGed_CHIL + sText;
        }
        else if (sText == "1 MARRIAGE")
        {
            sText = "1 " + cGed_MARR;
        }
        else if (sText == "1 " + cGed_BIRTH)
        {
            sText = "1 " + cGed_BIRT;
        }
        else if (sText == "0 " + cGed_HEADER)
        {
            sText = "0 " + cGed_HEAD;
        }
        else if (sText.Left(11) == "1 CHARACTER")
        {
            sText = "1 " + cGed_CHAR + Strings.Mid(sText, 12, sText.Length);
        }
        else if (sText.Left(8) == "1 " + cGed_SOURCE)
        {
            sText = "1 " + cGed_SOUR + Strings.Mid(sText, 9, sText.Length);
        }
        else if (sText == "0 TRAILER")
        {
            sText = "0 " + cGed_TRLR + Strings.Mid(sText, 9, sText.Length);
        }
        return sText;
    }


    public void Sperrfehler()
    {
        var text = "Durch die Grösse der Gedcom-Datei ist es nicht möglich das Einlesen rückgängig zu machen.";
        text += "\n Sie können das Einlesen der Datei jetzt abbrechen und erst eine Datensicherung machen.";
        text += "\nMit OK wird das Einlesen fortgesetzt, Sie können die Änderungen nicht mehr rückgängig machen.";
        text += "\nMit Abbrechen bleibt Ihre Datei unverändert";
        var left = Interaction.MsgBox(text, MsgBoxStyle.Exclamation | MsgBoxStyle.OkCancel, "");
        if (left.AsInt() == 1)
        {
            Op = 0;
            DataModul.wrkDefault.Commit();
            return;
        }
        MyProject.Forms.Einles.Close();
        DataModul.MandDB.Close();
        DataModul.DOSB.Close();
        DataModul.TempDB.Close();
        DataModul.DSB.Close();
        MyProject.Forms.Menue1.Close();
        ProjectData.ClearProjectError();
        Interaction.Shell(MainProg + "", AppWinStyle.NormalFocus);
        ProjectData.EndApp();
    }


    public void Quellenaus(EEventArt L)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
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
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0007;
                        case 2978:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_09a8;
                                    default:
                                        goto end_IL_0000;
                                }
                                Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                                if (Interaction.MsgBox(Mldg, MsgBoxStyle.OkCancel | MsgBoxStyle.Question, "Fehler") == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_09ac;
                            }
                        end_IL_0000:
                            break;
                        IL_0007:
                            num = 2;
                            num5 = PersInArb;
                            if (L > EEventArt.eA_499)
                            {
                                num5 = FamInArb;
                            }
                            if (GedAus.Options_Quellaus != 6)
                            {
                                goto end_IL_0000_2;
                            }
                            DataModul.DB_SourceLinkTable.Index = "Tab22";
                            DataModul.DB_SourceLinkTable.Seek("=", 3, num5, L, DataModul.DB_EventTable.Fields[EventFields.LfNr].AsInt());
                            goto IL_0827;
                        IL_01df: // <========== 3
                            num = 28;
                            lErl = 2;
                            DataModul.NB_SourceTable.Index = "OrgNr";
                            DataModul.NB_SourceTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[2]);
                            if (DataModul.NB_SourceTable.NoMatch)
                            {
                                DataModul.NB_SourceTable.AddNew();
                                DataModul.NB_SourceTable.Fields["AusNr"].Value = DataModul.NB_SourceTable.RecordCount + 1;
                                DataModul.NB_SourceTable.Fields["OrgNr"].Value = DataModul.DB_SourceLinkTable.Fields[2].Value;
                                DataModul.NB_SourceTable.Update();
                                Quell = 1;
                                goto IL_01df;
                            }
                            string text = "2 " + cGed_SOUR + " @S" + DataModul.NB_SourceTable.Fields["AusNr"].AsString().Trim() + "@";
                            GedAus.WriteGedLine(text, true);
                            text = "";
                            goto IL_038b;
                        IL_0382:
                            num = 44;
                            goto IL_038b;
                        IL_038b: // <========== 3
                            num = 46;
                            if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[3].Value))
                            {
                                if (DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim() != "")
                                {
                                    text = "3 " + cGed_PAGE + " " + DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim();
                                    text = text.Replace("\n", " ");
                                    text = text.Replace("\r", " ");
                                    text = text.Replace("  ", " ");
                                    GedAus.WriteGedLine(text, true);
                                    text = "";
                                    goto IL_04b4;
                                }
                            }
                            goto IL_0680;
                        IL_04ab:
                            num = 56;
                            goto IL_04b4;
                        IL_04b4: // <========== 3
                            num = 58;
                            if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value))
                            {
                                if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim() != "")
                                {
                                    if (DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim() != "")
                                    {
                                        text = "3 " + cGed__ZUS + " " + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim();
                                        GedAus.WriteGedLine(text, true);
                                        text = "";
                                        goto IL_0680;
                                    }
                                    if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim() != "Seite:")
                                    {
                                        text = "3 " + cGed__ZUS + " " + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim();
                                        GedAus.WriteGedLine(text, true);
                                        text = "";
                                    }

                                }
                            }
                            goto IL_0680;
                        IL_0680: // <========== 6
                            num = 84;
                            if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value))
                            {
                                text = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString().Replace("\n", "");
                                if (text.Trim() != "")
                                {
                                    text = "3 " + cGed_DATA;
                                    GedAus.WriteGedLine(text, true);
                                    text = "";
                                    text = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString().Trim();
                                    GedAus.TextTeilen(text, "4 " + cGed_TEXT + " ");
                                    text = "";

                                }
                            }
                            goto IL_0772;
                        IL_0772: // <========== 3
                            num = 96;
                            if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value))
                            {
                                if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString().Trim() != "")
                                {
                                    text = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString().Trim();
                                    GedAus.TextTeilen(text, "3 " + cGed_NOTE + " ");
                                    text = "";

                                }
                            }
                            goto IL_0811;
                        IL_0811: // <========== 5
                            num = 104;
                            lErl = 45;
                            DataModul.DB_SourceLinkTable.MoveNext();
                            goto IL_0827;
                        IL_0827: // <========== 3
                            num = 10;
                            if (!DataModul.DB_SourceLinkTable.EOF)
                            {
                                if (!DataModul.DB_SourceLinkTable.NoMatch)
                                {
                                    if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[2].Value))
                                    {
                                        if (DataModul.DB_SourceLinkTable.Fields[2].AsInt() == 0)
                                        {
                                            DataModul.DB_SourceLinkTable.Delete();
                                            goto IL_0811;
                                        }
                                        if (!(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].AsInt() > num5))
                                        {
                                            if (!(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsEnum<EEventArt>() != L))
                                            {
                                                if (!(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.LfNr].AsInt() != DataModul.DB_EventTable.Fields[EventFields.LfNr].AsInt()))
                                                {
                                                    goto IL_01df;
                                                }

                                            }
                                        }
                                        goto IL_083a;
                                    }
                                }
                                goto IL_0811;
                            }
                            goto IL_083a;
                        IL_083a: // <========== 4
                            num = 107;
                            if (Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem3].Value))
                            {
                                goto end_IL_0000_2;
                            }
                            if (DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Length <= 0)
                            {
                                goto end_IL_0000_2;
                            }
                            if (DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim() == "")
                            {
                            }
                            else
                            {

                                UbgT = "2 " + cGed_SOUR + " " + DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim();
                                GedAus.Historie(_Modul1.Instance.UbgT);
                            }
                            goto end_IL_0000_2;
                        IL_09a8:
                            num4 = unchecked(num2 + 1);
                            goto IL_09ac;
                        IL_09ac:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 27:
                                case 28:
                                case 37:
                                    goto IL_01df;
                                case 44:
                                    goto IL_0382;
                                case 45:
                                case 46:
                                    goto IL_038b;
                                case 56:
                                    goto IL_04ab;
                                case 57:
                                case 58:
                                    goto IL_04b4;
                                case 67:
                                case 68:
                                case 77:
                                case 78:
                                case 79:
                                case 80:
                                case 81:
                                case 82:
                                case 83:
                                case 84:
                                    goto IL_0680;
                                case 94:
                                case 95:
                                case 96:
                                    goto IL_0772;
                                case 13:
                                case 17:
                                case 101:
                                case 102:
                                case 103:
                                case 104:
                                    goto IL_0811;
                                case 9:
                                case 10:
                                case 106:
                                    goto IL_0827;
                                case 20:
                                case 23:
                                case 26:
                                case 107:
                                    goto IL_083a;
                                case 112:
                                case 113:
                                case 114:
                                case 115:
                                case 116:
                                case 122:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 2978;
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
    public string Strings_Leerweg1(string sText)
    {
        int num = 1;
        while (Strings.Mid(sText, 2, 2) == "  ")
        {
            sText = sText.Left(2) + Strings.Mid(sText, 4, sText.Length);
            sText = sText.Trim();
            num = checked(num + 1);
            if (num > 500)
            {
                break;
            }
        }
        return sText;
    }

    public short Strings_Lerz(string ubgT)
    {
        //Discarded unreachable code: IL_003f
        short A = 0;
        int num = ubgT.Length;
        for (var u = 1; u <= num && u <= 80; u++)
        {
            if (Strings.Mid(ubgT, u, 1) is " " or "\t")
            {
                A = (short)u;
            }
        }
        if (num > 80 && A < 8)
        {
            A = 81;
        }
        return A;
    }

    public void Sichwand(string Dasich, string sDatumV_S, DateTime dDatumB, EEventArt eArt)
    {
        switch (Dasich)
        {
            case "Z":
            case "z":
                Dasich = CDateModifyer.cGedDate_BET;
                return;
            case "V":
            case "v":
                Dasich = CDateModifyer.cGedDate_BEF;
                return;
            case "N":
            case "n":
                Dasich = CDateModifyer.cGedDate_AFT;
                return;
            case "R":
            case "r":
                Dasich = CDateModifyer.cGedDate_CAL;
                return;
            case "U":
            case "u":
                Dasich = CDateModifyer.cGedDate_ABT;
                return;
            case "?":
                Dasich = CDateModifyer.cGedDateUnkn;
                return;
            case "C":
            case "c":
                Dasich = CDateModifyer.cGedDate_EST;
                return;
            case "B":
            case "b":
                Dasich = CDateModifyer.cGedDate_TO;
                return;
        }
        if (dDatumB != default)
        {
            GedAus.Fehlliste(eArt, sDatumV_S, this.PersInArb, "1");
        }
    }

    public void Orttextspeichern()
    {
        IPlaceData place = Einles.Place;

        string text = "";
        string text2 = "";
        string text3 = "";
        string text4 = "";
        string text5 = "";
        place.iOrt = 0;
        place.iOrtsteil = 0;
        place.iKreis = 0;
        place.iLand = 0;
        place.iStaat = 0;
        if (text5.Length > 0)
        {
            UbgT = text5;
            Person_TextSpeichern(UbgT, _Modul1.Instance.PersInArb, ETextKennz.H_, LfNR);
            place.iOrt = Einles.Satz;
        }
        if (text4.Length > 0)
        {
            UbgT = text4;
            Person_TextSpeichern(UbgT, _Modul1.Instance.PersInArb, ETextKennz.I_, LfNR);
            place.iOrtsteil = Einles.Satz;
        }
        if (text3.Length > 0)
        {
            UbgT = text3;
            Person_TextSpeichern(UbgT, _Modul1.Instance.PersInArb, ETextKennz.J_, LfNR);
            place.iKreis = Einles.Satz;
        }
        if (text2.Length > 0)
        {
            UbgT = text2;
            Person_TextSpeichern(UbgT, _Modul1.Instance.PersInArb, ETextKennz.K_, LfNR);
            place.iLand = Einles.Satz;
        }
        if (text.Length > 0)
        {
            UbgT = text;
            Person_TextSpeichern(UbgT, _Modul1.Instance.PersInArb, ETextKennz.L_, LfNR);
            place.iStaat = Einles.Satz;
        }
    }


    public void Paten_O_Taufe()
    {
        string text = "";
        checked
        {
            if (GedAus.Options_Paten == 6)
            {
                text = "";
                int persInArb = PersInArb;
                foreach (var cLink in DataModul.Link.ReadAllFams(PersInArb, ELinkKennz.lkGodparent))
                {
                    Perles(cLink.iPersNr);
                    text = text + Kont[3].Trim() + " " + Kont[0].Trim() + "; ";
                }
                PersInArb = persInArb;
                if (text != "" && text.Right(2) == "; ")
                {
                    text = text.Left(text.Length - 2);
                }
            }
            if (Information.IsDBNull(DataModul.DB_PersonTable.Fields[PersonFields.Bem2].Value))
            {
                return;
            }
            if (DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString().Length > 0 && DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString().Trim() != "")
            {
                if (text != "")
                {
                    text += ";";
                }
                text = (text + " " + DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString());
            }
            text = text.Trim();
            if (text != "")
            {
                GedAus.WriteGedLine("1 " + cGed_CHR + " ", false);
                GedAus.TextTeilen(text, "2 " + cGed__GODP + " ");
            }
        }
    }

    public void Fampersuch_Auswahl()
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
                checked
                {
                    int num4;
                    int number;
                    Type typeFromHandle;
                    object[] array;
                    IField field;
                    object[] array2;
                    bool[] array3;
                    object left;
                    int nR;
                    string S_C;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 2897:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_09a7;
                                    default:
                                        goto end_IL_0000;
                                }
                                number = Information.Err().Number;
                                Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                                if (Interaction.MsgBox(Mldg, MsgBoxStyle.OkCancel | MsgBoxStyle.Question, "Fehler") == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_09ab;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            Persatzles(_Modul1.Instance.PersInArb);
                            typeFromHandle = typeof(Strings);
                            array = new object[1];
                            field = DataModul.DB_PersonTable.Fields[PersonFields.Sex];
                            array[0] = field.Value;
                            array2 = array;
                            array3 = new bool[1]
                            {
                            true
                            }
                            ;
                            left = NewLateBinding.LateGet(null, typeFromHandle, "UCase", array2, null, null, array3);
                            if (array3[0])
                            {
                                field.Value = array2[0];
                            }
                            eLKennz = left.AsString() == "F" ? ELinkKennz.lkMother : ELinkKennz.lkFather;
                            string FamugNrS = "";
                            foreach (var cLink in DataModul.Link.ReadAllPers(PersInArb, eLKennz))
                            {
                                if (Leer == 0)
                                {
                                    if (!DataModul.NB_SperrFams_Exists(cLink.iFamNr))
                                    {
                                        FamugNrS += Strings.Right("          " + cLink.iFamNr.AsString(), 10);
                                    }
                                }
                                else
                                {
                                    DataModul.NB_FamilyTable.Index = "Fam";
                                    DataModul.NB_FamilyTable.Seek("=", cLink.iFamNr);
                                    if (!DataModul.NB_FamilyTable.NoMatch)
                                    {
                                        if (!DataModul.NB_SperrFams_Exists(cLink.iFamNr))
                                        {
                                            FamugNrS += Strings.Right("          " + cLink.iFamNr.AsString(), 10);
                                        }
                                    }
                                }
                            }
                            int num5 = (int)Math.Round(FamugNrS.Length / 10.0 - 1.0);
                            int num6 = 0;
                            while (num6 <= num5)
                            {
                                nR = Strings.Mid(FamugNrS, num6 * 10 + 1, 10).AsInt();
                                S_C = "S";
                                Viel_Personen(nR, ref S_C);
                                num6++;
                            }
                            eLKennz = ELinkKennz.lkChild;
                            if (DataModul.Link.GetPersonFam(PersInArb, ELinkKennz.lkChild, out FamugNr))
                            {
                                if (Leer == 0)
                                {
                                    if (!DataModul.NB_SperrFams_Exists(FamugNr))
                                    {
                                        S_C = "C";
                                        Viel_Personen(FamugNr, ref S_C);
                                    }
                                }
                                else
                                {
                                    DataModul.NB_FamilyTable.Index = "Fam";
                                    DataModul.NB_FamilyTable.Seek("=", FamugNr);
                                    if (!DataModul.NB_FamilyTable.NoMatch)
                                    {
                                        if (!DataModul.NB_SperrFams_Exists(FamugNr))
                                        {
                                            S_C = "C";
                                            _ = Viel_Personen(FamugNr, ref S_C);
                                        }
                                    }
                                }
                            }
                            if (DataModul.Link.GetPersonFam(PersInArb, ELinkKennz.lkAdoptedChild, out FamugNr))
                            {
                                if (!DataModul.NB_SperrFams_Exists(FamugNr))
                                {
                                    S_C = "C";
                                    AnzP = Viel_Personen(FamugNr, ref S_C);
                                    if (AnzP > 1)
                                    {
                                        string text = "2 " + cGed_PEDI + " Adopted";
                                        GedAus.WriteGedLine(text, true);
                                        text = "";
                                    }
                                }

                            }
                            goto end_IL_0000_2;
                        IL_09a7:
                            num4 = unchecked(num2 + 1);
                            goto IL_09ab;
                        IL_09ab:
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
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 2897;
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

 

    public void Quelldateiaus()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int num5 = default;
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
                        case 3978:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 3:
                                        break;
                                    case 2:
                                        goto IL_0c60;
                                    case 1:
                                        goto IL_0d08;
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
                                    num2 = 0;
                                    goto IL_0617;
                                }
                                Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                                if (Interaction.MsgBox(Mldg, MsgBoxStyle.OkCancel | MsgBoxStyle.Question, "Fehler") == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0d04;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            if (DataModul.NB_SourceTable.RecordCount > 0)
                            {
                                DataModul.NB_SourceTable.Index = "AusNr";
                                DataModul.NB_SourceTable.MoveFirst();
                                goto IL_0b5b;
                            }
                            goto IL_0b6d;
                        IL_0255: // <========== 3
                            num = 31;
                            string text;
                            if (DataModul.DB_QuTable.Fields[QuFields._4].AsString().Trim() != "")
                            {
                                text = "1 " + cGed_ABBR + " " + DataModul.DB_QuTable.Fields[QuFields._4].AsString().Trim();
                                text = text.Replace("@", "@@");
                                GedAus.WriteGedLine(text, true);
                            }
                            goto IL_0315;
                        IL_0315: // <========== 3
                            num = 40;
                            if (DataModul.DB_QuTable.Fields[QuFields._5].AsString().Trim() != "")
                            {
                                text = "1 " + cGed_AUTH + " " + DataModul.DB_QuTable.Fields[QuFields._5].AsString().Trim();
                                text = text.Replace("@", "@@");
                                GedAus.WriteGedLine(text, true);
                            }
                            goto IL_03d5;
                        IL_03d5: // <========== 3
                            num = 49;
                            DataModul.DB_RepoTab.Index = "Nr";
                            DataModul.DB_RepoTab.Seek("=", DataModul.DB_QuTable.Fields[QuFields._1].Value);
                            goto IL_062c;
                        IL_0617: // <========== 3
                            num = 71;
                            lErl = 3;
                            DataModul.DB_RepoTab.MoveNext();
                            goto IL_062c;
                        IL_062c: // <========== 3
                            num = 52;
                            if (!DataModul.DB_RepoTab.NoMatch)
                            {
                                if (!DataModul.DB_RepoTab.EOF)
                                {
                                    DataModul.DB_RepoTable.Index = "Nr";
                                    DataModul.DB_RepoTable.Seek("=", DataModul.DB_RepoTab.Fields["Repo"].Value);
                                    if (!(DataModul.DB_RepoTab.Fields["Quelle"].Value != DataModul.DB_QuTable.Fields[QuFields._1].Value))
                                    {
                                        text = "1 " + cGed_REPO + " @R" + DataModul.DB_RepoTab.Fields["Repo"].AsString().Trim() + "@";
                                        GedAus.WriteGedLine(text, true);

                                        ProjectData.ClearProjectError();
                                        num3 = 3;
                                        DataModul.NB_LagTable.AddNew();
                                        DataModul.NB_LagTable.Fields["SatzNr"].Value = Conversion.Val(DataModul.DB_RepoTab.Fields["Repo"].AsString().Trim());
                                        DataModul.NB_LagTable.Update();
                                        goto IL_0617;
                                    }

                                }
                            }
                            goto IL_063f;
                        IL_063f: // <========== 3
                            num = 74;
                            if (DataModul.DB_QuTable.Fields[QuFields._7].AsString().Trim() != "")
                            {
                                text = "1 " + cGed_PUBL + " " + DataModul.DB_QuTable.Fields[QuFields._7].AsString().Trim();
                                text = text.Replace("@", "@@");
                                GedAus.WriteGedLine(text, true);
                            }
                            goto IL_06ff;
                        IL_06ff: // <========== 3
                            num = 83;
                            if (DataModul.DB_QuTable.Fields[QuFields._8].AsString().Trim() != "")
                            {
                                text = "1 " + cGed_PLAC + " " + DataModul.DB_QuTable.Fields[QuFields._8].AsString().Trim();
                                text = text.Replace("@", "@@");
                                GedAus.WriteGedLine(text, true);
                            }
                            goto IL_07bf;
                        IL_07bf: // <========== 3
                            num = 92;
                            if (DataModul.DB_QuTable.Fields[QuFields._9].AsString().Trim() != "")
                            {
                                text = "1 " + cGed_DATE + " " + DataModul.DB_QuTable.Fields[QuFields._9].AsString().Trim();
                                text = text.Replace("@", "@@");
                                GedAus.WriteGedLine(text, true);
                            }
                            goto IL_087f;
                        IL_087f: // <========== 3
                            num = 101;
                            if (DataModul.DB_QuTable.Fields[QuFields._10].AsString().Trim() != "")
                            {
                                text = "1 " + cGed__PEI + " " + DataModul.DB_QuTable.Fields[QuFields._10].AsString().Trim();
                                GedAus.WriteGedLine(text, true);
                            }
                            goto IL_0924;
                        IL_0924: // <========== 3
                            num = 109;
                            if (DataModul.DB_QuTable.Fields[QuFields._11].AsString().Trim() != "")
                            {
                                text = "1 " + cGed__JAG + " " + DataModul.DB_QuTable.Fields[QuFields._11].AsString().Trim();
                                GedAus.WriteGedLine(text, true);
                            }
                            goto IL_09c9;
                        IL_09c9: // <========== 3
                            num = 117;
                            if (DataModul.DB_QuTable.Fields[QuFields._12].AsString().Trim() != "")
                            {
                                text = "1 " + cGed_REFN + " " + DataModul.DB_QuTable.Fields[QuFields._12].AsString().Trim();
                                GedAus.WriteGedLine(text, true);
                            }
                            goto IL_0a6e;
                        IL_0a6e: // <========== 3
                            num = 125;
                            if (DataModul.DB_QuTable.Fields[QuFields._13].AsString().Trim() != "")
                            {
                                UbgT = "1 " + cGed_NOTE + " " + DataModul.DB_QuTable.Fields[QuFields._13].AsString().Trim();
                                GedAus.Historie(_Modul1.Instance.UbgT);
                            }
                            goto IL_0ae2;
                        IL_0ae2: // <========== 3
                            num = 130;
                            if (GedAus.Options_Bildja == 6)
                            {
                                int perfamNr = (int)Math.Round(DataModul.NB_SourceTable.Fields["OrgNr"].AsDouble());
                                GedAus.Bilderaus("Q", perfamNr);
                            }
                            goto IL_0b34;
                        IL_0b34: // <========== 3
                            num = 134;
                            lErl = 45;
                            DataModul.NB_SourceTable.MoveNext();
                            num5++;
                            goto IL_0b5b;
                        IL_0b5b: // <========== 3
                            num = 6;
                            if (!DataModul.NB_SourceTable.EOF)
                            {
                                if (!(DataModul.NB_SourceTable.Fields["OrgNr"].AsInt() == 0))
                                {
                                    if (!DataModul.NB_SourceTable.NoMatch)
                                    {
                                        text = "0 @S" + DataModul.NB_SourceTable.Fields["AusNr"].AsString().Trim() + "@ SOUR";
                                        GedAus.WriteGedLine(text, true);
                                        DataModul.DB_QuTable.Index = "Nr";
                                        DataModul.DB_QuTable.Seek("=", DataModul.NB_SourceTable.Fields["OrgNr"].AsString());
                                        if (!DataModul.DB_QuTable.NoMatch)
                                        {
                                            if (DataModul.DB_QuTable.Fields[QuFields._2].AsString().Trim() != "")
                                            {
                                                text = "1 " + cGed_TITL + " " + DataModul.DB_QuTable.Fields[QuFields._2].AsString().Trim();
                                                text = text.Replace("@", "@@");
                                                GedAus.WriteGedLine(text, true);
                                            }
                                            goto IL_0255;
                                        }
                                        goto IL_0ae2;
                                    }
                                }
                                else goto IL_0b34;
                            }
                            goto IL_0b6d;
                        IL_0b6d: // <========== 4
                            num = 139;
                            GedAus.Repoausgeb();
                            goto end_IL_0000_2;
                        IL_0c60:
                            num = 151;
                            Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                            if (Interaction.MsgBox(Mldg, MsgBoxStyle.OkCancel | MsgBoxStyle.Question, "Fehler") == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_0d04;
                        IL_0d04:
                            num4 = num2;
                            goto IL_0d0c;
                        IL_0d08:
                            num4 = unchecked(num2 + 1);
                            goto IL_0d0c;
                        IL_0d0c:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 29:
                                case 30:
                                case 31:
                                    goto IL_0255;
                                case 38:
                                case 39:
                                case 40:
                                    goto IL_0315;
                                case 47:
                                case 48:
                                case 49:
                                    goto IL_03d5;
                                case 71:
                                case 143:
                                    goto IL_0617;
                                case 51:
                                case 52:
                                case 73:
                                    goto IL_062c;
                                case 54:
                                case 59:
                                case 74:
                                    goto IL_063f;
                                case 81:
                                case 82:
                                case 83:
                                    goto IL_06ff;
                                case 90:
                                case 91:
                                case 92:
                                    goto IL_07bf;
                                case 99:
                                case 100:
                                case 101:
                                    goto IL_087f;
                                case 107:
                                case 108:
                                case 109:
                                    goto IL_0924;
                                case 115:
                                case 116:
                                case 117:
                                    goto IL_09c9;
                                case 123:
                                case 124:
                                case 125:
                                    goto IL_0a6e;
                                case 128:
                                case 129:
                                case 130:
                                    goto IL_0ae2;
                                case 8:
                                case 133:
                                case 134:
                                    goto IL_0b34;
                                case 5:
                                case 6:
                                case 137:
                                    goto IL_0b5b;
                                case 11:
                                case 138:
                                case 139:
                                    goto IL_0b6d;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 3978;
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


    public bool Rechnen(string Lo, out string Ba3, out string Ba5)
    {
        string text = Lo;
        int num = Strings.InStr(text, ".");
        if (num == 0)
        {
            Interaction.MsgBox("Eingabefehler");
            Ba3 = Ba5 = "";
            return false;
        }
        string text2 = Strings.Mid(text, checked(num + 1), text.Length);
        float num2 = default;
        float num4 = default;
        if (text2.AsDouble() > 0.0)
        {
            num2 = (text2.Left(1).AsDouble() != 0.0) ? ((float)(text2.AsDouble() / 1000000.0 * 60.0)) : ((float)(text2.AsDouble() / 1000000.0 * 60.0));
            float num3 = num2 - Conversion.Int(num2);
            num2 = Conversion.Int(num2);
            num4 = Conversion.Int(num3 / 100f * 6000f);
        }
        Ba3 = Strings.Format(num2, "##00");
        Ba5 = Strings.Format(num4, "##00");
        return true;
    }

    public void DezRechnen(ref string A4)
    {
        float num = (!(UbgT.Left(2).AsDouble() < 10.0)) ? Conversions.ToInteger(Strings.Trim((UbgT.Left(2).AsDouble() / 0.006).AsString())) : Conversions.ToInteger(Strings.Trim((UbgT.Left(2).AsDouble() / 0.006).AsString()));
        float num2 = (!(num < 1000f)) ? Conversions.ToInteger(Strings.Trim((UbgT.Right(2).AsDouble() / 0.36).AsString())) : Conversions.ToInteger(Strings.Trim((UbgT.Right(2).AsDouble() / 0.36).AsString()));
        A4 = (num + num2).AsString();
        if (A4.AsDouble() < 1000.0)
        {
            A4 = "0000" + A4.Right(4);
        }
        A4 = A4 + "00000".Left(6);
    }

    public void DataModul_OpenDB()
    {
        int try0000_dispatch = -1;
        int num = default;
        int lErl = default;
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
                        lErl = 11;
                        goto IL_0007;
                    case 2340:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0742;
                                default:
                                    goto end_IL_0000;
                            }
                            int number = Information.Err().Number;
                            if (number == 53)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0742;
                            }
                            if (number == 55)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0742;
                            }
                            if (number == 70)
                            {
                                Interaction.MsgBox("Sie können nicht die Daten des aktuellen Mandanten überschreiben. Bitte wählen Sie erst einen anderen Mandanten oder geben Sie dem neuen Mandanten einen anderen Namen", MsgBoxStyle.OkOnly, "Bedienungsfehler");
                                DataModul.MandDB.Close();
                                ProjectData.EndApp();
                                lErl = 6111;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0742;
                            }
                            if (number == 75)
                            {
                                MyProject.Forms.Gene.Close();
                                if (Ubg == 0)
                                {
                                    MyProject.Forms.BestGenBehandeln.ShowDialog();
                                    Behand = checked((int)Math.Round(MyProject.Forms.BestGenBehandeln.Bezubg.Text.AsDouble()));
                                }
                                else
                                {
                                    MyProject.Forms.BestGenBehandeln.Close();
                                }
                                goto IL_04d9;
                            }
                            if (number == 91)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0742;
                            }
                            if (number == 3010)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0742;
                            }
                            if (number == 3011)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0742;
                            }
                            if (number == 3024)
                            {
                                goto end_IL_0000_2;
                            }
                            if (number == 3204)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0742;
                            }
                            if (number == 3211)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0742;
                            }
                            if (number == 3262)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0742;
                            }
                            if (number == 3375)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0742;
                            }
                            if (number == 3380)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0742;
                            }
                            Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                            if (Interaction.MsgBox(Mldg, MsgBoxStyle.OkCancel | MsgBoxStyle.Question, "Fehler") == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_0746;
                        }
                    end_IL_0000:
                        break;
                    IL_0007:
                        num = 2;
                        FileSystem.FileClose();
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        Mandant = Verz + "Gen_plusdaten.mdb";
                        DataModul.wrkDefault = DataModul.DAODBEngine_definst.Workspaces[0];
                        int num5 = DateTime.Today.Year;
                        if (num5 < 2018)
                        {
                            DataModul.MandDB = DataModul.DAODBEngine_definst.OpenDatabase(Mandant.ToUpper(), true, false, ";pwd=geheim");
                        }
                        else
                        {
                            DataModul.MandDB = DataModul.DAODBEngine_definst.OpenDatabase(Mandant, true, false, "");
                        }
                        goto IL_00c5;
                    IL_00c5: // <========== 3
                        num = 13;
                        DataModul.DB_RepoTable = DataModul.MandDB.OpenRecordset(dbTables.Repo, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_RepoTab = DataModul.MandDB.OpenRecordset(dbTables.RepoTab, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_WitnessTable = DataModul.MandDB.OpenRecordset(dbTables.Tab2, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_FamilyTable = DataModul.MandDB.OpenRecordset(dbTables.Familie, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_NameTable = DataModul.MandDB.OpenRecordset(dbTables.INamen, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_PictureTable = DataModul.MandDB.OpenRecordset(dbTables.Bilder, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DSB_SearchTable = DataModul.DSB.OpenRecordset(dbTables.Such, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DSB_SearchTable.Index = "Persuch";
                        DataModul.DB_TexteTable = DataModul.MandDB.OpenRecordset(dbTables.Texte, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_TexteTable.Index = "ITexte";
                        DataModul.DB_PlaceTable = DataModul.MandDB.OpenRecordset(dbTables.Orte, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_PlaceTable.Index = "Orte";
                        DataModul.DB_EventTable = DataModul.MandDB.OpenRecordset(dbTables.Ereignis, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_EventTable.Index = "ArtNr";
                        DataModul.DB_LinkTable = DataModul.MandDB.OpenRecordset(dbTables.Tab, RecordsetTypeEnum.dbOpenTable);
                        DataModul.DB_LinkTable.Index = "ElSu";
                        DT_AhnTable = DataModul.TempDB.OpenRecordset(dbTables.Ahnen1, RecordsetTypeEnum.dbOpenTable);
                        DT_AhnTable.Index = "PerNr";
                        DataModul.DOSB_OrtSTable = DataModul.DOSB.OpenRecordset(dbTables.OrtSuch);
                        goto end_IL_0000_2;
                    IL_04d9: // <========== 3
                        num = 68;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_0742;
                    IL_0742: // <========== 16
                        num4 = num2 + 1;
                        goto IL_0746;
                    IL_0746:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 9:
                            case 12:
                            case 13:
                                goto IL_00c5;
                            case 33:
                                num = 33;
                                if (Information.Err().Number == 53)
                                {
                                    goto case 34;
                                }
                                goto case 35;
                            case 34:
                                num = 34;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0742;
                            case 35:
                            case 37:
                                num = 37;
                                if (Information.Err().Number == 75)
                                {
                                    goto case 38;
                                }
                                goto case 39;
                            case 38:
                                num = 38;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0742;
                            case 39:
                            case 41:
                                num = 41;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0742;
                            case 64:
                            case 67:
                            case 68:
                                goto IL_04d9;
                            case 32:
                            case 84:
                            case 116:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 2340;
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

    public int Viel_Personen(int iFam, ref string S_C)
    {
        var AnzP = 0;
        if (DataModul.NB_SperrFams_Exists(iFam))
        {
            return 0;
        }
        foreach (var cLink in DataModul.Link.ReadAllFams(iFam))
        {
            if (cLink.eKennz == ELinkKennz.lkChild
                || cLink.eKennz == ELinkKennz.lkAdoptedChild
                || cLink.eKennz == ELinkKennz.lkFather
                || cLink.eKennz == ELinkKennz.lkMother)
            {
                if (++AnzP > 1)
                {
                    break;
                }
            }
        }
        if ((AnzP > 1) | (GedAus.Options_Famleer == 1))
        {
            string text = "1 " + cGed_FAM + S_C + " @F" + iFam.AsString().TrimStart() + "@";
            GedAus.WriteGedLine(text, true);
        }

        return AnzP;
    }

    public int DataModul_PeekMandant_RecordCount(string Verz)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int num5 = default;
        ProjectData.ClearProjectError();
        num3 = 1;
        var Mandant = Verz + "Gen_plusdaten.mdb";
        FileSystem.SetAttr(Mandant, FileAttribute.Normal);
        num5 = DateTime.Today.Year;
        if (num5 < 2018)
        {
            DataModul.MandDB = DataModul.DAODBEngine_definst.OpenDatabase(Mandant.ToUpper(), true, false, ";pwd=geheim");
        }
        else
            DataModul.MandDB = DataModul.DAODBEngine_definst.OpenDatabase(Mandant, true, false, "");

        DataModul.DB_TexteTable = DataModul.MandDB.OpenRecordset(dbTables.Texte, RecordsetTypeEnum.dbOpenTable);
        DataModul.DB_PlaceTable = DataModul.MandDB.OpenRecordset(dbTables.Orte, RecordsetTypeEnum.dbOpenTable);
        DataModul.DB_QuTable = DataModul.MandDB.OpenRecordset(dbTables.Quellen, RecordsetTypeEnum.dbOpenTable);
        DataModul.DB_PersonTable = DataModul.MandDB.OpenRecordset(dbTables.Personen, RecordsetTypeEnum.dbOpenTable);
        DataModul.DB_FamilyTable = DataModul.MandDB.OpenRecordset(dbTables.Familie, RecordsetTypeEnum.dbOpenTable);
        int Besttest = DataModul.DB_PersonTable.RecordCount;
        Besttest += DataModul.DB_FamilyTable.RecordCount;
        Besttest += DataModul.DB_TexteTable.RecordCount;
        Besttest += DataModul.Place.Count;
        Besttest += DataModul.DB_QuTable.RecordCount;
        return Besttest;
    }
    public void Ahnles(int PersInArb, out string[] asAhnData)
    {
        throw new NotImplementedException();
    }

    public DateTime AtomicTime(string sTimeServer)
    {
        throw new NotImplementedException();
    }

    public Image AutoSizeImage(Image oBitmap, int maxWidth, int maxHeight, bool bStretch = false)
    {
        throw new NotImplementedException();
    }

    public string Datwand1(string Datu, string Ds)
    {
        throw new NotImplementedException();
    }


    public float Datcheck(int eArt)
    {
        throw new NotImplementedException();
    }

    public void Datles(int PersInArb, out IList<string> asPersDates)
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

    public string DezRechnen(string A4, string ubgT)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> EnumerateMandants(string drive)
    {
        throw new NotImplementedException();
    }

    public void Erei(int PersInArb, EEventArt eArt, ref byte PerPos)
    {
        throw new NotImplementedException();
    }

    public IList<int> Link_Famsuch(int PersInArb, ELinkKennz eLKennz)
    {
        throw new NotImplementedException();
    }

    public void Famdatles(int FamInArb, out string[] Kont)
    {
        throw new NotImplementedException();
    }

    public void Famdatles2()
    {
        throw new NotImplementedException();
    }

    public string GoogleInstallPath()
    {
        throw new NotImplementedException();
    }

    public short IsFormloaded(object Formtocheck)
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

    public string ortles1(int Ortnr, byte Schalt = 0, Action<int, string>? action = null)
    {
        throw new NotImplementedException();
    }

    public string ortles(int OrtNr, byte Schalt)
    {
        throw new NotImplementedException();
    }

    public string[] Ortles(IPlaceData place, int Schalt = 0, Action<int, string>? export = null)
    {
        throw new NotImplementedException();
    }

    public void Paten2(int PersInArb, ref string Pattext, long Ahne)
    {
        throw new NotImplementedException();
    }

    public void Person_ReadNames(int PersInArb, IPersonData person)
    {
        throw new NotImplementedException();
    }

    public string Rech(ref string datum1, ref string datum2)
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

    public void Vornam_Namles(int personNr)
    {
        throw new NotImplementedException();
    }

    public void Zeugsu(EEventArt Art, short LfNR, short Listart, long Ahne)
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

    public void Berufles(int PersInArb, EEventArt Beruf, object combo1)
    {
        throw new NotImplementedException();
    }

    public IList<T> DeleteDoublicates<T>(IList<T> oList)
    {
        throw new NotImplementedException();
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

    public void KTextles(string Formnam, ETextKennz eTKennz, IList oIIList, (string sText, ETextKennz eTKnz) Bezeichnu)
    {
        throw new NotImplementedException();
    }

    public void ExportPlace(int OrtNr, string sOrt, string ind1, string namen)
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

    public void Famles(int famInArb, IFamilyData family)
    {
        throw new NotImplementedException();
    }

    // Missing interface members for IModul1
    public string OrtBem { get => _sOrtBem; set => _sOrtBem = value; }
    private string _sOrtBem = "";

    public string Datum1 { get; set; } = "";
    public string Datum2 { get; set; } = "";
    public string Ausdat { get; set; } = "";
    public int iPriv_aus { get; set; }
    string IModul1.Font1 { get => Font1; set => Font1 = value; }
    public int PersSp { get; set; }
    public string Datschuname { get; set; } = "";
    public string Eltq { get; set; } = "";

    /// <summary>
    /// Reads family data.
    /// </summary>
    public void Family_Les(int famInArb, IFamilyData family)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Reads supplementary date data.
    /// </summary>
    public void Datles2()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the full surname for a person.
    /// </summary>
    public string Person_FullSurname(IPersonData person, bool xFamToUpper)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets the full surname for a list of name components.
    /// </summary>
    public void Person_FullSurname(IList<string> kont, bool xFamToUpper)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Reads family data for the given switch value.
    /// </summary>
    public void Famdatles1(int schalt, out string[] asFamDate)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Reads person record.
    /// </summary>
    public void PerSatzLes(int persInArb)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Reads family data.
    /// </summary>
    public void Famles()
    {
        throw new NotImplementedException();
    }
    #endregion
}
