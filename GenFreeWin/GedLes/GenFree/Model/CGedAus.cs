using BaseLib.Helper;
using Gedcomles.My;
using GenFree.Constants;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Interfaces.Sys;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Policy;
using static GenFree.Constants.GedComTags;

namespace GenFree.Models;

public class CGedAus : IGedAus
{
    IGedAus GedAus => this;
    IModul1 Modul1 => _Modul1.Instance;

    ISysTime SysTime;

    private bool[] _kont3;

    public string ADRText { get; set; }

    public bool[] Kont3 => _kont3 ??= new bool[7];
    public string FILENAM { get; set; } = "Gedcom.ged";

    public int Leer { get; set; }

    string Dat;
    string NamenSuch_Wort;
    private string UbgT1;
    private string UbgT;
    public byte HLT { get; set; } = 0;
    public int Options_Uml { get; set; } = 0;
    public int[] Datschu { get; set; } = new int[9];
    public byte Options_Bildja { get; set; }
    public bool Kontakt { get; set; }
    public bool Internet { get; set; }
    public bool[] Hi { get; set; } = new bool[8];
    public EExportPrivacy Options_priv { get; set; } = default;
    public short Options_Paten { get; set; }
    public short Options_Quellaus { get; set; }
    public bool Options_OFB { get; set; }
    public bool Options_Orts { get; set; } = true;
    public byte Options_Schalt1 { get; set; }
    public bool Options_Sperraus { get; set; }
    public int Options_Famleer { get; set; } = 0;
    public bool Options_Loe { get; set; }
    public bool Options_Mehrvorn { get; set; } = false;
    public bool Options_Hausnr { get; set; } = false;
    public bool Options_EigQuelle { get; set; } = false;


    public int PeekDB(string sFilename)
    {
        IDatabase? NB = default;
        try
        {
            NB = DataModul.DAODBEngine_definst.OpenDatabase(sFilename, false, true, "");
            var NB_PersonTable = NB?.OpenRecordset(dbTables.Personen1, RecordsetTypeEnum.dbOpenTable);
            NB_PersonTable?.MoveLast();
            return NB_PersonTable?.RecordCount ?? 0;
        }
        catch
        {
            return 0;
        }
        finally
        {
            if (NB != null && NB.IsOpen)
            {
                NB.Close();
            }
        }
    }

    public void berufles(int iPersInArb)
    {
        short num5 = default;
        string Fld = "";
        int Ortnr;
        string text = "";
        string Wort = "";

        EEventArt eArt = EEventArt.eA_300;
        while (eArt <= EEventArt.eA_303)
        {
            foreach (var cEv in DataModul.Event.ReadEventsBeSu(iPersInArb, eArt))
            {
                if (cEv.iPrivacy <= GedAus.Options_priv.AsInt())
                {
                    if (cEv.iKBem > 0
                        | cEv.dDatumV != default
                        | cEv.dDatumB != default |
                                    cEv.iOrt > 0 |
                                cEv.sBem[1].Trim() != "" |
                            cEv.sBem[2].Trim() != "")
                    {
                        switch (eArt)
                        {
                            case EEventArt.eA_300:
                                WriteGedLine($"1 {cGed_OCCU} {cEv.sKBem}", true);
                                break;
                            case EEventArt.eA_301:
                                WriteGedLine($"1 {cGed_TITL} {cEv.sKBem}", true);
                                break;
                            case EEventArt.eA_302:
                                WriteGedLine($"1 {cGed_RESI} ", true);
                                break;
                            case EEventArt.eA_303:
                                WriteGedLine($"1 {cGed_CIVI} {cEv.sKBem}", true);
                                break;
                            default:
                                break;
                        }

                        if (cEv.iPrivacy > 0)
                        {
                            if (cEv.iPrivacy == 1)
                            {
                                WriteGedLine($"2 {cGed_RESN} privacy", false);
                            }
                            else
                             if (cEv.iPrivacy == 2)
                            {
                                WriteGedLine($"2 {cGed_RESN} confidential", false);
                            }
                        }

                        string Datu = Event_GedDate(eArt, cEv);
                        if (Datu != "")
                        {
                            text = cEv.iDatumText != 0 && cEv.dDatumV != default ? "INT " : "";
                            WriteGedLine($"2 {cGed_DATE} {text}{Datu.Trim()}", true);
                        }

                        if (cEv.iOrt > 0)
                        {
                            Ortnr = cEv.iOrt;
                            _Modul1.Instance.UbgT = _Modul1.Instance.ortles1(Ortnr, 0);
                        }

                        if (cEv.sOrt_S.Trim() != "")
                        {
                            WriteGedLine("3 " + cGed__SIC + " " + cEv.sOrt_S.Trim(), true);
                        }

                        if (cEv.sZusatz.Trim() != "")
                        {
                            WriteGedLine("3 " + cGed__ZUS + " " + cEv.sZusatz.Trim(), true);
                        }

                        if (!string.IsNullOrWhiteSpace(cEv.sReg))
                        {
                            Fld = cEv.sReg.Replace("\n", " ").Trim();
                            if (GedAus.Options_Uml > 0)
                            {
                                Fld = _Modul1.Instance.Strngs_Umlaute4(Fld, GedAus.Options_Uml);
                            }
                            WriteGedLine($"2 {cGed_REFN} {Fld}", true);
                        }

                        if (cEv.iPlatz > 0)
                        {
                            Fld = cEv.sPlatz.Trim().Replace("\n", " ");
                            if (GedAus.Options_Uml > 0)
                            {
                                Fld = _Modul1.Instance.Strngs_Umlaute4(Fld, GedAus.Options_Uml);
                            }
                            WriteGedLine("2 " + cGed__SITE + " " + Fld, true);
                        }

                        if (cEv.sBem[1].Trim() != "" && _Modul1.Instance.GedAus.Hi[2])
                        {
                            Historie("2 " + cGed__COM + " " + cEv.sBem[1].Trim());
                        }

                        if (eArt == EEventArt.eA_302)
                        {
                            Fld = "";
                            if (cEv.iHausNr > 0)
                            {
                                Fld = cEv.sHausNr.Trim();
                                if (GedAus.Options_Uml > 0)
                                {
                                    Fld = _Modul1.Instance.Strngs_Umlaute4(Fld, GedAus.Options_Uml);
                                }
                            }
                            if (cEv.sKBem != "")
                            {
                                WriteGedLine(($"2 {cGed_ADDR} {cEv.sKBem} {Fld}").Trim(), false);
                                if (!_Modul1.Instance.GedAus.Options_Hausnr)
                                {
                                    WriteGedLine($"{3} {cGed__STRASSE} {cEv.sKBem} {Fld}".Trim(), false);
                                    ADRText = "";
                                }
                                else
                                {
                                    WriteGedLine($"{3} {cGed__STRASSE} {cEv.sKBem}", true);
                                    ADRText = "";
                                    if (Fld.Trim() != "")
                                    {
                                        WriteGedLine($"3 {cGed__NUM} {Fld}".Trim(), true);
                                        Fld = "";
                                    }
                                }
                            }
                        }
                        if (cEv.sBem[2].Trim() != "")
                        {
                            if (_Modul1.Instance.GedAus.Hi[3])
                            {
                                Historie("2 " + cGed_NOTE + " " + cEv.sBem[2].Trim());
                            }
                        }
                        if (_Modul1.Instance.GedAus.Options_Quellaus == 6)
                        {
                            _Modul1.Instance.Quellenaus(eArt);
                        }

                        WriteWitnesses(iPersInArb, eArt, cEv);
                    }

                }
                num5++;
            }
            eArt++;
        }
        goto end_IL_0000_2;
    end_IL_0000_2: // <========== 3
        return;
    }

    private void WriteWitnesses(int iPersInArb, EEventArt eArt, IEventData cEv)
    {
        IRecordset dB_WitnessTable = DataModul.DB_WitnessTable;
        if (dB_WitnessTable.RecordCount > 0)
        {
            dB_WitnessTable.MoveFirst();
            dB_WitnessTable.Index = "ZeugSu";
            dB_WitnessTable.Seek("=", iPersInArb, "10", eArt, cEv.iLfNr);
            short num6 = 1;
            while (num6 <= 99
                && !dB_WitnessTable.EOF
                && !dB_WitnessTable.NoMatch
                && !(dB_WitnessTable.Fields[WitnessFields.FamNr].AsInt() != iPersInArb |
                dB_WitnessTable.Fields[WitnessFields.Kennz].AsInt() != 10
                | dB_WitnessTable.Fields[WitnessFields.Art].AsEnum<EEventArt>() != eArt))
            {
                int Witness_iPerNr = dB_WitnessTable.Fields[WitnessFields.PerNr].AsInt();
                IRecordset nB_Witness2Table = DataModul.NB_Witness2Table!;
                nB_Witness2Table.Index = "Per";
                nB_Witness2Table.Seek("=", Witness_iPerNr);
                if (nB_Witness2Table.RecordCount <= 0 || !nB_Witness2Table.NoMatch)
                {
                    WriteGedLine($"{2} {cGed_ASSO} @I{Witness_iPerNr}@", true);
                    WriteGedLine($"{3} {cGed_RELA} Zeuge", true);
                }

                dB_WitnessTable.MoveNext();
                num6++;
            }

        }
        if (cEv.sBem[4].Trim() != "")
        {
            string Fld = cEv.sBem[4];
            TextTeilen(Fld, "2 " + cGed__WITN + " ");
        }
    }

    private string Event_GedDate(EEventArt eArt, IEventData cEv)
    {
        var Datu = "";
        var datsicha = "";
        if (cEv.sDatumV_S.Trim() != "")
        {
            var Dasich = cEv.sDatumV_S.Trim();
            _Modul1.Instance.Sichwand(Dasich, DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString().Trim(), DataModul.DB_EventTable.Fields[EventFields.DatumB].AsDate(), eArt);
            Datu = Dasich;
            datsicha = Dasich;
        }
        DateTime Dat;
        if (cEv.dDatumV.AsInt() > 0)
        {
            Dat = cEv.dDatumV;
            Datu = Datu.TrimEnd() + " " + Dat.AsString().Trim();
        }

        if (!((cEv.sDatumB_S.ToUpper().Trim() == "A") & cEv.dDatumB.AsInt() > 0))
        {
            if ((cEv.dDatumV == default) &
                cEv.dDatumB != default &
                cEv.sDatumB_S.Trim() == "")
            {
                Dat = cEv.dDatumB;
                Datu = Datu.TrimEnd() + " TO " + Dat.AsString().Trim();
            }
            else
            if ((cEv.dDatumV == default) &
                cEv.dDatumB != default &
                (cEv.sDatumB_S.ToUpper().Trim() == "B"))
            {
                Dat = cEv.dDatumB;
                Datu = Datu.TrimEnd() + " TO " + Dat.AsString().Trim();
            }
        }
        else
        {
            Datu += " AND ";
            if (cEv.dDatumB != default)
            {
                Dat = cEv.dDatumB;
                Datu = Datu.TrimEnd() + " " + Dat.AsString().Trim();
            }
            else
            {
                if ((cEv.dDatumV != default
                    & cEv.sDatumB_S.Trim() != "") |
                    cEv.dDatumB != default)
                {
                    if ((Datu.Trim() != "") & cEv.sDatumV_S.Trim() == "")
                    {
                        Datu = "FROM " + Datu.Trim();
                        datsicha = "FROM";
                    }
                    Zweitdatum(ref Datu, datsicha, eArt, cEv);
                }
            }
        }
        if (cEv.iDatumText != 0)
        {
            var Wort = cEv.sDatumText;
            if (GedAus.Options_Uml > 0)
            {
                Wort = _Modul1.Instance.Strngs_Umlaute4(Wort, GedAus.Options_Uml);
            }
            if (Wort.Trim() != "")
            {
                Datu += " (" + Wort.Trim() + ")";
            }
        }
        return Datu;
    }

    public void Personenpaten(int PersInArb, bool xGodChildRelSkip)
    {
        foreach (var cLink in DataModul.Link.ReadAllFams(PersInArb, ELinkKennz.lkGodparent))
        {
            if (!DataModul.NB_SperrPers_Exists(cLink.iPersNr))
            {
                string text = "1 " + cGed_ASSO + " @I" + cLink.iPersNr.AsString().Trim() + "@";
                if (!DataModul.NB_Witness2_Table_ExistsPer(cLink.iPersNr))
                {
                    WriteGedLine(text, false);
                    text = "2 " + cGed_RELA + " Godfather";
                    WriteGedLine(text, true);
                }
            }

        }
        if (!xGodChildRelSkip)
        {
            foreach (var cLink in DataModul.Link.ReadAllPers(PersInArb, ELinkKennz.lkGodparent))
            {
                if (!DataModul.NB_SperrPers_Exists(cLink.iFamNr))
                {
                    string text = "1 " + cGed_ASSO + " @I" + cLink.iFamNr.AsString().Trim() + "@";
                    if (!DataModul.NB_Witness2_Table_ExistsPer(cLink.iFamNr))
                    {
                        WriteGedLine(text, false);
                        text = "2 " + cGed_RELA + " Godchild";
                        WriteGedLine(text, true);
                    }
                }
            }
        }
    }

    public void WriteGedLine(string Fld, bool xCheck)
    {
        _Modul1.Instance.sw.WriteLine(Fld);
        _Modul1.Instance.d_ += Fld.Length;
        if (xCheck && _Modul1.Instance.d_ >= _Modul1.Instance.Gedaus_DiskSize)
        {
            _Modul1.Instance.GedAus_Diskvoll();
        }
    }

    public void WriteAnDatum(string _Cr, string CHa)
    {
        if (_Cr.AsInt() > 0)
        {
            WriteGedLine("1 " + cGed__CREAT, false);
            WriteDate(_Cr, 2);
        }
        if (CHa.AsInt() > 0)
        {
            WriteGedLine("1 " + cGed_CHAN, false);
            WriteDate(CHa, 2);
        }
        else if (_Cr.AsInt() > 0)
        {
            WriteGedLine("1 " + cGed_CHAN, false);
            WriteDate(_Cr, 2);
        }
    }

    public void WriteDate(string _Cr, int iLvl)
    {
        string sDay = _Cr.Right(2);
        if (sDay.AsInt() == 0)
        {
            sDay = "  ";
        }
        string sMonth = Strings.Mid(_Cr.AsString(), 6, 2);
        sMonth = sMonth.AsInt().MonthToMonthShort(true);
        string text2 = _Cr.AsString().Left(5);
        WriteGedLine($"{iLvl} {cGed_DATE} {sDay} {sMonth}{text2}", true);
    }

    public void WriteFamDat(int FamInArb)
    {
        EEventArt _eArt = EEventArt.eA_500;
        checked
        {
            while (_eArt <= EEventArt.eA_507)
            {
                if (DataModul.Event.ReadData(_eArt, FamInArb, out var cEv))
                {
                    if ((GedAus.Datschu[4] <= 0
                        || cEv.dDatumV.AsInt() <= GedAus.Datschu[4])
                        && cEv.iPrivacy <= GedAus.Options_priv.AsInt())
                    {
                        switch (_eArt)
                        {
                            case EEventArt.eA_500:
                                WriteGedLine("1 " + cGed_MARB + " ", true);
                                break;
                            case EEventArt.eA_501:
                                WriteGedLine("1 " + cGed_ENGA + "  ", true);
                                break;
                            case EEventArt.eA_Marriage:
                                WriteGedLine("1 " + cGed_MARR + " ", true);
                                break;
                            case EEventArt.eA_MarrReligious:
                                WriteGedLine("1 " + cGed_MARR + " ", false);
                                WriteGedLine("2 " + cGed_TYPE + "  RELI", true);
                                break;
                            case EEventArt.eA_504:
                                WriteGedLine("1 " + cGed_DIV + " ", true);
                                break;
                            case EEventArt.eA_505:
                                WriteGedLine("1 " + cGed__LIV + " ", true);
                                break;
                            case EEventArt.eA_507:
                                WriteGedLine("1 " + cGed_MARR + " ", false);
                                WriteGedLine("2 " + cGed_TYPE + " Dim", true);
                                break;
                        }
                        WriteEventData(_eArt, cEv);
                        _Modul1.Instance.GedAus.Trauaus = 0;
                    }
                }

                _eArt++;
            }

            if (DataModul.Event.ReadData(EEventArt.eA_601, FamInArb, out var cEv2))
            {
                WriteGedLine("1 " + cGed_MARR + " ", false);
                WriteGedLine("2 " + cGed_TYPE + "  Fikt.", true);
                WriteEventData(_eArt, cEv2);
            }
        }
    }

    public bool CheckFamPersons(int famInArb)
    {
        byte b3 = 0;
        int iPers;
        if (DataModul.Link.GetFamPerson(famInArb, ELinkKennz.lkFather, out iPers) && DataModul.NB_SperrPers_Exists(iPers))
        {
            if (b3 < 2)
            {
                b3++;
            }
            else return true;
        }
        if (DataModul.Link.GetFamPerson(famInArb, ELinkKennz.lkMother, out iPers) && DataModul.NB_SperrPers_Exists(iPers))
        {
            if (b3 < 2)
            {
                b3++;
            }
            else return true;
        }
        var num5 = 0;
        foreach (var cLink in DataModul.Link.ReadAllFams(famInArb, ELinkKennz.lkChild))
        {
            if (DataModul.NB_SperrPers_Exists(cLink.iPersNr))
            {
                if (b3 < 2)
                {
                    b3++;
                }
                else return true;
            }
            num5++;
        }
        if (_Modul1.Instance.GedAus.Options_Famleer == 0 && b3 < 2)
        {
            return true;
        }

        return false;
    }

    public void ReadKopf()
    {
        int try0000_dispatch = -1;
        int num6;
        string nDrive;
        int lpBytesPerSector;
        int lpNumberOfFreeClusters;
        int lpTtoalNumberOfClusters;
        int diskFreeSpace;
        ProjectData.ClearProjectError();
        FileSystem.MkDir(Modul1.GenFreeDir + "Temp");

        Modul1.Dateienopen();
        string source = Modul1.InitDir + "\\GedAUS.mdb";
        string destination = Modul1.TempPath + "GEDAUS.mdb";
        FileSystem.FileCopy(source, destination);
        string name = Modul1.TempPath + "GEDAUS.mdb";

        OpenTempDB(name);
        var Absend = new string[11];
        Modul1.Persistence.ReadStringsProg("Adresse", Absend);
        Modul1.User.Name = Absend[1].TrimStart() + " " + Absend[2].TrimEnd() + " " + Absend[3].TrimEnd();
        Modul1.User.Street = Absend[4].TrimEnd();
        Modul1.User.Place = Absend[5] + " " + Absend[6];
        Modul1.User.Staat = Absend[10];
        Modul1.User.tel = Absend[7];
        Modul1.User.mail = Absend[8];
        SpDisk = FILENAM.Trim().Left(1);
        if (SpDisk.Length == 0)
        {
            DataModul.MandDB?.Close();
            DataModul.DOSB?.Close();
            DataModul.TempDB?.Close();
            DataModul.DSB?.Close();
            MyProject.Forms.Menue1.Close();
            ProjectData.ClearProjectError();
        }
        SpDisk = SpDisk.Trim().Left(1);
        nDrive = SpDisk + ":\\";
        Typ = (byte)GetDriveType(ref nDrive);
        byte typ = Typ;
        if (typ == 5)
        {
            Interaction.MsgBox("Speicherversuch auf CD ! Speichern auf CD ist nicht möglich !");
            ProjectData.EndApp();
        }

        nDrive = GedAus.SpDisk + ":\\";
        lpBytesPerSector = (int)bytes;
        lpNumberOfFreeClusters = (int)freeclusters;
        lpTtoalNumberOfClusters = (int)clusters;
        diskFreeSpace = GetDiskFreeSpace(ref nDrive, ref sectors, ref lpBytesPerSector, ref lpNumberOfFreeClusters, ref lpTtoalNumberOfClusters);
        clusters = lpTtoalNumberOfClusters;
        freeclusters = lpNumberOfFreeClusters;
        bytes = lpBytesPerSector;
        ok = (short)diskFreeSpace;
        if (ok != 0)
        {
            Modul1.Free = sectors * bytes * freeclusters;
        }
        else
        {
            Modul1.Free = 0L;
        }
    end_IL_0000_2: // <========== 4
        return;
    }

    public void Fertig()
    {
        FileSystem.FileClose(99);
        FileSystem.FileOpen(99, TempPath + "Gedfehl.dat", OpenMode.Append);
        FileSystem.FileClose(99);
        if (FileSystem.FileLen(TempPath + "Gedfehl.dat") > 0)
        {
            MyProject.Forms.Bildlist.ShowDialog(1);
        }
        if (DataModul.NB_PictureTable.RecordCount > 0)
        {
            MyProject.Forms.Bildlist.ShowDialog(2);
        }
        DataModul.MandDB?.Close();
        DataModul.DOSB?.Close();
        DataModul.TempDB?.Close();
        DataModul.DSB?.Close();
        if (Internet)
        {
            Interaction.Shell(Aus[22] + " " + Verz1 + "INTERAhn\\Ahn.HTM", AppWinStyle.MaximizedFocus);
        }
        Interaction.MsgBox("Fertig");
        Interaction.Shell(MainProg + "", AppWinStyle.NormalFocus);
        ProjectData.EndApp();
    }

    public void WriteHeader(string gedAus_FILENAM)
    {
        int num;
        num = 86;
        GedAus.SpDisk = GedAus.SpDisk.Left(1) + ":";
        if (gedAus_FILENAM.Length == 0)
        {
            MyProject.Forms.Menue1.Show();
            DataModul.NB_OrtTable.Close();
            DataModul.NB.Close();
            return;
        }

        FileSystem.FileOpen(11, gedAus_FILENAM, OpenMode.Output);
        FileSystem.FileClose();
        FileSystem.Kill(gedAus_FILENAM);
        FileSystem.FileOpen(99, Modul1.TempPath + "Gedfehl.dat", OpenMode.Output);
        FileSystem.FileClose(99);
        ProjectData.ClearProjectError();
        var num3 = 5;
        Modul1.Gedaus_DiskSize = (long)Math.Round(Free - Free / 10.0);
        d_ = 0;
        WriteGedLine("0 " + cGed_HEAD, false);
        WriteGedLine("1 " + cGed_SOUR + $" {Modul1.AppName.ToUpper()}", false);
        WriteGedLine("2 " + cGed_VERS + $" {Modul1.System.VerSpecial} {Modul1.Version1}", false);
        WriteGedLine("2 " + cGed_CORP + $" {Modul1.Author}", false);
        WriteGedLine("3 " + cGed_ADDR + " Friedrich-Holthaus-Str. 18", false);
        WriteGedLine("4 " + cGed_CONT + " 49082 Osnabrück", false);
        WriteGedLine("4 " + cGed_CONT + " GERMANY", false);
        string text = DateTime.Today.Month.ToString() + "." + DateTime.Today.Day.ToString() + "." + DateTime.Today.Year.ToString();
        WriteGedLine("1 " + cGed_DATE + " " + text, false);
        WriteGedLine("1 " + cGed_FILE + " " + Path.GetFileName(gedAus_FILENAM), false);
        if (GedAus.Options_Uml == 0)
        {
            WriteGedLine("1 " + cGed_CHAR + " ANSI", false);
        }
        else if (GedAus.Options_Uml == 3)
        {
            WriteGedLine("1 " + cGed_CHAR + " UTF-8", false);
        }
        else
        {
            WriteGedLine("1 " + cGed_CHAR + " ASCII", false);
        }
        WriteGedLine($"1 {cGed_SUBM} @SUB1@", false);
        WriteGedLine($"1 {cGed_GEDC}", false);
        WriteGedLine("2 " + cGed_VERS + " 5.5.1", false);
        WriteGedLine("1 " + cGed_PLAC, false);
        WriteGedLine("2 " + cGed_FORM + " place, county/district, state, Country, not used, locality", false);
        WriteGedLine("1 " + cGed_SCHEMA, false);
        WriteGedLine("2 " + cGed_INDI, false);
        WriteGedLine("3 " + cGed__CREAT, false);
        WriteGedLine("4 " + cGed__DEFN + " Erstellungsdatum des Datensatzes", false);
        WriteGedLine("3 " + cGed__GODP, false);
        WriteGedLine("4 " + cGed__DEFN + " Paten als Text", false);
        WriteGedLine("3 " + cGed_NAME, false);
        WriteGedLine("4 _RUFNAME", false);
        WriteGedLine("5 " + cGed__DEFN + " Rufname", false);
        WriteGedLine("3 " + cGed__UID, false);
        WriteGedLine("4 " + cGed__DEFN + " Eindeutige Kennzeichnung des Datensatzes", false);
        WriteGedLine("2 " + cGed_FAM, false);
        WriteGedLine("3 " + cGed__LIV, false);
        WriteGedLine("4 " + cGed__DEFN + " Lebensgemeinschaft", false);
        WriteGedLine("3 " + cGed__CREAT, false);
        WriteGedLine("4 " + cGed__DEFN + " Erstellungsdatum des Datensatzes", false);
        WriteGedLine("3 " + cGed__NAME, false);
        WriteGedLine("4 " + cGed__DEFN + " Gemeinsamer Familienname nach neuem Namenrecht", false);
        WriteGedLine("3 " + cGed__UID, false);
        WriteGedLine("4 " + cGed__DEFN + " Eindeutige Kennzeichnung des Datensatzes", false);
        WriteGedLine("2 " + cGed_DATE, false);
        WriteGedLine("3 " + cGed__DEP, false);
        WriteGedLine("4 " + cGed__DEFN + " Zeuge als Text", false);
        WriteGedLine("3 " + cGed__SITE, false);
        WriteGedLine("4 " + cGed__DEFN + " Text für Kiche / Friedhof etc.", false);
        WriteGedLine("3 " + cGed_PLAC, false);
        WriteGedLine("4 " + cGed__ZUS, false);
        WriteGedLine("5 " + cGed__DEFN + " Zusatztext zum Ort beim Datum", false);
        WriteGedLine("2 " + cGed_SOUR, false);
        WriteGedLine("3 " + cGed__ZUS, false);
        WriteGedLine("4 " + cGed__DEFN + " Zusatztext zur Quelle", false);
        WriteGedLine("3 " + cGed__ORI, false);
        WriteGedLine("4 " + cGed__DEFN + " Originaltext der Quelle", false);
        WriteGedLine("3 " + cGed__KTIT, false);
        WriteGedLine("4 " + cGed__DEFN + " Kurztitel der Quelle", false);
        WriteGedLine("3 " + cGed__PEI, false);
        WriteGedLine("4 " + cGed__DEFN + " Quelle erschienen in...", false);
        WriteGedLine("3 " + cGed__JAG, false);
        WriteGedLine("3 " + cGed__DEFN + " Jahrgang von ''_PEI''", false);
        WriteGedLine("2 " + cGed_PLAC, false);
        WriteGedLine("3 " + cGed__POST, false);
        WriteGedLine("4 " + cGed__DEFN + " Postleitzahl", false);
        WriteGedLine("3 " + cGed__SIC, false);
        WriteGedLine("4 " + cGed__DEFN + " Sicherheit der Ortsangabe", false);
        WriteGedLine("3 " + cGed__AON, false);
        WriteGedLine("4 " + cGed__DEFN + " Alternativer Ortsname", false);
        WriteGedLine($"3 {cGed__MAIDENHEAD}", false);
        WriteGedLine("4 " + cGed__DEFN + " Locator-Code", false);
        WriteGedLine("3 " + cGed__GOV, false);
        WriteGedLine("4 " + cGed__DEFN + " Für das genealogische Ortsverzeichnis die Code-Nr ", false);
        WriteGedLine("3 " + cGed__FSTAE, false);
        WriteGedLine("4 " + cGed__DEFN + " Territorium für FoKo", false);
        WriteGedLine("3 " + cGed__FCTRY, false);
        WriteGedLine("4 " + cGed__DEFN + " Staatskennzeichen für FoKo", false);
        WriteGedLine("3 " + cGed__ZUS, false);
        WriteGedLine("4 " + cGed__DEFN + " Zusatztext zum Ort", false);
        WriteGedLine("1 " + cGed__TXT, false);
        WriteGedLine("2 " + cGed__DEFN + " Ergänzungen zu den Texten für Name,Berufe Orte usw., nur für {AppName}", false);
        WriteGedLine("2 " + cGed_TEXT, false);
        WriteGedLine("3 " + cGed__DEFN + " der Text", false);
        WriteGedLine("2 " + cGed_KENN, false);
        WriteGedLine("3 " + cGed__DEFN + " Kennzeichen", false);
        WriteGedLine("2 " + cGed_LEIT, false);
        WriteGedLine("3 " + cGed__DEFN + " Leitname", false);
        WriteGedLine("2 " + cGed_NOTE, false);
        WriteGedLine("3 " + cGed__DEFN + " Bemerkungen", false);
        WriteGedLine("2 " + cGed__HEIM, false);
        WriteGedLine("3 " + cGed__DEFN + " Heimat oder Bürgerort", false);
        WriteGedLine("2 " + cGed_RESI, false);
        WriteGedLine("3 _STRASSE", false);
        WriteGedLine("4 " + cGed__DEFN + " Straßenangabe", false);
        WriteGedLine("3 " + cGed__NUM, false);
        WriteGedLine("4 " + cGed__DEFN + " Hausnummer", false);
        if (Modul1.User.Name == "")
        {
            Modul1.User.Name = "Unbekannt";
        }
        WriteGedLine("0 @SUB1@ SUBM", false);
        string text2 = Modul1.User.Name;
        text2 = text2 + "\n" + Modul1.User.Street;
        text2 = text2 + "\n" + Modul1.User.Place;
        text2 = text2 + "\n" + Modul1.User.tel;
        text2 = text2 + "\n" + Modul1.User.mail;
        WriteGedLine($"1 {cGed_NAME} {Modul1.User.Name}", false);
        WriteGedLine($"1 {cGed_ADDR} {Modul1.User.Street}", false);
        WriteGedLine($"2 {cGed_CONT} {Modul1.User.Place}", false);
        if (Modul1.User.tel.Length > 0)
        {
            WriteGedLine($"2 {cGed_PHON} {Modul1.User.tel}", false);
        }
        if (Modul1.User.mail != "")
        {

            WriteGedLine($"2 {cGed_EMAIL} {Modul1.User.mail}", false);
        }
    }

    public void OpenTempDB(string sFileName)
    {
        DataModul.NB = DataModul.DAODBEngine_definst.OpenDatabase(sFileName, false, false, "");
        DataModul.NB_OrtTable = DataModul.NB.OpenRecordset(dbTables.Orte, RecordsetTypeEnum.dbOpenTable);
        DataModul.NB_OrtTable.Index = "ORT";
        DataModul.NB.Execute($"CREATE Table {dbTables.Texte}  ({IndexFields.SatzNr} Long );");
        DataModul.NB.Execute($"ALTER Table {dbTables.Texte} ADD COLUMN TXT TEXT;");
        DataModul.NB.Execute($"CREATE UNIQUE INDEX Nr ON {dbTables.Texte} ([{IndexFields.SatzNr}]);");
        DataModul.NB.Execute($"CREATE INDEX ALTexte ON {dbTables.Texte} ([TXT]);");
        DataModul.NB_TexteTable = DataModul.NB.OpenRecordset(dbTables.Texte, RecordsetTypeEnum.dbOpenTable);
        DataModul.NB_TexteTable.Index = "Nr";

        DataModul.NB.Execute($"CREATE Table {dbTables.SperrPer}  ({SperrIdxFields.Nr} Long );");
        DataModul.NB.Execute($"CREATE UNIQUE INDEX Nr ON {dbTables.SperrPer} ([{SperrIdxFields.Nr}]);");
        DataModul.NB_SperrPersTable = DataModul.NB.OpenRecordset(dbTables.SperrPer, RecordsetTypeEnum.dbOpenTable);
        DataModul.NB_SperrPersTable.Index = "Nr";

        DataModul.NB.Execute($"CREATE Table {dbTables.SperrFam}  ({SperrIdxFields.Nr} Long );");
        DataModul.NB.Execute($"CREATE UNIQUE INDEX Nr ON {dbTables.SperrFam} ([{SperrIdxFields.Nr}]);");
        DataModul.NB_SperrFamsTable = DataModul.NB.OpenRecordset(dbTables.SperrFam, RecordsetTypeEnum.dbOpenTable);
        DataModul.NB_SperrFamsTable.Index = "Nr";

        DataModul.NB.Execute($"CREATE Table {dbTables.Lag}  ({IndexFields.SatzNr} Long );");
        DataModul.NB.Execute($"CREATE UNIQUE INDEX Nr ON {dbTables.Lag} ([{IndexFields.SatzNr}]);");
        DataModul.NB_LagTable = DataModul.NB.OpenRecordset(dbTables.Lag, RecordsetTypeEnum.dbOpenTable);
        DataModul.NB_LagTable.Index = "Nr";

        DataModul.NB_SourceTable = DataModul.NB.OpenRecordset(dbTables.QuellTemp, RecordsetTypeEnum.dbOpenTable);
        DataModul.NB_PictureTable = DataModul.NB.OpenRecordset(dbTables.Bilder, RecordsetTypeEnum.dbOpenTable);

        DataModul.NB.Execute($"CREATE Table {dbTables.Personen1} ({IndexFields.Person} Integer );");
        DataModul.NB.Execute($"CREATE UNIQUE INDEX Per ON {dbTables.Personen1} ([{IndexFields.Person}]);");
        DataModul.NB_PersonTable = DataModul.NB.OpenRecordset(dbTables.Personen1, RecordsetTypeEnum.dbOpenTable);

        DataModul.NB.Execute($"CREATE Table {dbTables.Familie1}  ({IndexFields.Fam} Integer );");
        DataModul.NB.Execute($"CREATE UNIQUE INDEX {IndexFields.Fam} ON {dbTables.Familie1} ([{IndexFields.Fam}]);");
        DataModul.NB_FamilyTable = DataModul.NB.OpenRecordset(dbTables.Familie1, RecordsetTypeEnum.dbOpenTable);

        DataModul.NB.Execute($"CREATE Table {dbTables.Zeugen}  ({IndexFields.Person} Integer );");
        DataModul.NB.Execute($"CREATE UNIQUE INDEX Per ON {dbTables.Zeugen} ([{IndexFields.Person}]);");
        DataModul.NB_WitnessTable = DataModul.NB.OpenRecordset(dbTables.Zeugen, RecordsetTypeEnum.dbOpenTable);

        DataModul.NB.Execute($"DROP Table {dbTables.Zeugen2}");
        DataModul.NB.Execute($"CREATE Table {dbTables.Zeugen2}  ({IndexFields.Person} Integer );");
        DataModul.NB.Execute($"CREATE UNIQUE INDEX Per ON {dbTables.Zeugen2} ([{IndexFields.Person}]);");
        DataModul.NB_Witness2Table = DataModul.NB.OpenRecordset(dbTables.Zeugen2, RecordsetTypeEnum.dbOpenTable);

    }

    public void Individuum(IPersonData cPerson)
    {
        do
        {
            var Fld = $"0 @I{cPerson.ID}@ INDI";
            WriteGedLine(Fld, true);
            _Modul1.Instance.Pataus = 0;
            Namensteil(cPerson.ID);

            if (!_Modul1.Instance.GedAus_Person_DatSchutz(cPerson.ID, GedAus.Datschu))
                break;
            if (_Modul1.Instance.GedAus.Hi[1] && cPerson.sBem[1].Trim() != "")
            {
                _Modul1.Instance.UbgT = "1 NOTE " + cPerson.sBem[1].Trim();
                Historie(_Modul1.Instance.UbgT);
            }
            _Modul1.Instance.Kont[1] = cPerson.dAnlDatum.AsString();
            _Modul1.Instance.Kont[2] = cPerson.dEditDat.AsString();
            WriteAnDatum(_Modul1.Instance.Kont[1], _Modul1.Instance.Kont[2]);

            var b4 = 0;
            var Modul1_eArt = EEventArt.eA_Birth;
            while (Modul1_eArt <= EEventArt.eA_Burial)
            {
                DataModul.DB_EventTable.Index = "Besu";
                DataModul.DB_EventTable.Seek("=", Modul1_eArt.AsString(), cPerson.ID.AsString());
                if (!DataModul.DB_EventTable.NoMatch)
                {
                    switch (Modul1_eArt)
                    {
                        case EEventArt.eA_Birth:
                            WriteGedLine("1 BIRT ", false);
                            break; ;
                        case EEventArt.eA_Baptism:
                            WriteGedLine("1 CHR  ", false);
                            b4 = 1;
                            break;
                        case EEventArt.eA_Death:
                            WriteGedLine("1 DEAT ", false);
                            break;
                        case EEventArt.eA_Burial:
                            WriteGedLine("1 BURI ", false);
                            break;
                        default:
                            break;
                    }
                    WriteEventData(Modul1_eArt, new CEventData(DataModul.DB_EventTable));
                }
                if (unchecked(Modul1_eArt == EEventArt.eA_Baptism && b4 == 0))
                {
                    _Modul1.Instance.Paten_O_Taufe();
                }
                Modul1_eArt++;
            }
            Sonstles(_Modul1.Instance.PersInArb);
            berufles(cPerson.ID);
            if (_Modul1.Instance.GedAus.Options_Paten != 6)
            {
                Personenpaten(cPerson.ID, !MyProject.Forms.Einstellungen.CheckBox7.Checked);
            }
            if (_Modul1.Instance.GedAus.Options_Quellaus == 6)
            {
                Quelleaus(1, cPerson.ID);
                if (cPerson.sBem[3] != "" && cPerson.sBem[3].Length > 0 && cPerson.sBem[3].Trim() != "")
                {
                    _Modul1.Instance.UbgT = "1 SOUR " + cPerson.sBem[3].Trim();
                    Historie(_Modul1.Instance.UbgT);
                }
            }
            if (GedAus.Options_Bildja == 6)
            {
                Bilderaus("P", cPerson.ID);
            }
        }
        while (false);
        _Modul1.Instance.Fampersuch_Auswahl();

    }

    public bool Person_DatSchutzEvt(int iPersNr, int[] M_Datschu)
    {
        int iToday = SysTime.TodayInt;
        var dDate = default(DateTime);
        EEventArt eArt;
        if (M_Datschu[1] > 0)
        {
            eArt = EEventArt.eA_Birth;
            while (eArt <= EEventArt.eA_107)
            {
                if ((dDate = DataModul.Event.GetDate(eArt, iPersNr)) != default
                    && dDate.AsInt() > M_Datschu[1])
                    return false;
                eArt++;
            }

            eArt = EEventArt.eA_300;
            while (eArt <= EEventArt.eA_302)
            {
                if ((dDate = DataModul.Event.GetDate(eArt, iPersNr)) != default
                    && dDate.AsInt() > M_Datschu[1])
                    return false;
                eArt++;
            }
        }
        if (M_Datschu[2] > 0)
        {
            eArt = EEventArt.eA_Birth;
            while (eArt <= EEventArt.eA_Baptism)
            {
                if ((dDate = DataModul.Event.GetDate(eArt, iPersNr)) != default
                   && dDate.AsInt() > M_Datschu[2])
                    return false;
                eArt++;
            }
        }
        if (M_Datschu[3] > 0)
        {
            eArt = EEventArt.eA_Death;
            while (eArt <= EEventArt.eA_Burial)
            {
                if ((dDate = DataModul.Event.GetDate(eArt, iPersNr)) != default
                && dDate.AsInt() > M_Datschu[3])
                    return false;
                eArt++;
            }
        }
        if (M_Datschu[5] > 0)
        {
            eArt = EEventArt.eA_Death;
            while (eArt < EEventArt.eA_Burial)
            {
                if ((dDate = DataModul.Event.GetDate(eArt, iPersNr)) != default
                    && dDate.AsInt() > iToday - 300000)
                    return false;
                eArt++;
            }
            eArt = EEventArt.eA_Birth;
            while (eArt < EEventArt.eA_Baptism)
            {
                if ((dDate = DataModul.Event.GetDate(eArt, iPersNr)) != default
                    && dDate.AsInt() > iToday - 1200000)
                    return false;
                eArt++;
            }
        }

        return true;
    }


    public void Sperrdaterst(int iPersInArb, int iFamInArb)
    {
        Cursor.Current = Cursors.WaitCursor;
        int iNowM110;
        int iNowM030;
        long num5;
        EEventArt num14;
        if (Datschu[6] == 1)
        {
            iNowM110 = (int)Math.Round(Conversion.Val(Conversions.ToDouble(Strings.Mid(DateTime.Now.AsString(), 7, 4) + Strings.Mid(DateTime.Now.AsString(), 4, 2) + DateTime.Now.AsString().Left(2)) - 1100000.0));
            int iNowM080 = (int)Math.Round(Conversion.Val(Conversions.ToDouble(Strings.Mid(DateTime.Now.AsString(), 7, 4) + Strings.Mid(DateTime.Now.AsString(), 4, 2) + DateTime.Now.AsString().Left(2)) - 800000.0));
            iNowM030 = (int)Math.Round(Conversion.Val(Conversions.ToDouble(Strings.Mid(DateTime.Now.AsString(), 7, 4) + Strings.Mid(DateTime.Now.AsString(), 4, 2) + DateTime.Now.AsString().Left(2)) - 300000.0));
            DataModul.DB_PersonTable.MoveLast();
            long num9 = 1L;
            long num10 = (long)Math.Round(Conversion.Val(DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt()));
            num5 = num9;
            while (num5 <= num10)
            {
                iPersInArb = (int)num5;
                DataModul.DB_EventTable.Index = "BeSu";
                num14 = EEventArt.eA_Death;
                while (num14 <= EEventArt.eA_Burial)
                {
                    DataModul.DB_EventTable.Seek("=", num14.AsString(), iPersInArb.AsString());
                    if (!DataModul.DB_EventTable.NoMatch
                        && DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDouble() > 0.0)
                    {
                        if (!(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDouble() < iNowM030))
                        {
                            if (DataModul.NB_SperrPers_AppendC(iPersInArb))
                                break;
                        }
                        else
                            break;
                    }
                    num14++;
                }
                if (num14 > EEventArt.eA_Burial)
                {
                    num14 = EEventArt.eA_Birth;
                    while (num14 <= EEventArt.eA_Baptism)
                    {
                        DataModul.DB_EventTable.Seek("=", num14.AsString(), iPersInArb.AsString());
                        if (!DataModul.DB_EventTable.NoMatch
                            && DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDouble() > 0.0)
                        {
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDouble() < iNowM110)
                                break;
                            else
                            {
                                if (DataModul.NB_SperrPers_AppendC(iPersInArb))
                                    break;
                            }
                        }
                        num14++;
                    }
                }

                num5++;
            }
            DataModul.DB_FamilyTable.MoveLast();
            long num11 = (long)Math.Round(Conversion.Val(DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt()));
            long num12 = 1L;
            long num13 = (long)Math.Round(Conversion.Val(DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt()));
            num5 = num12;
            while (num5 <= num13)
            {
                iFamInArb = (int)num5;
                DataModul.DB_EventTable.Index = "BeSu";
                num14 = EEventArt.eA_507;
                while (num14 >= EEventArt.eA_500)
                {
                    DataModul.DB_EventTable.Seek("=", num14.AsString(), iFamInArb.AsString());
                    if (!DataModul.DB_EventTable.NoMatch)
                    {
                        if (!((DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDouble() > 0.0) & (DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDouble() < iNowM080)))
                        {
                            if (DataModul.NB_SperrFams_AppendC(iFamInArb))
                                break;
                        }
                        else
                            break;
                    }
                    num14 += -1;
                }

                num5++;
            }
        }
        if (Datschu[7] == 1)
        {
            iNowM110 = (int)Math.Round(Conversion.Val(Conversions.ToDouble(Strings.Mid(DateTime.Now.AsString(), 7, 4) + Strings.Mid(DateTime.Now.AsString(), 4, 2) + DateTime.Now.AsString().Left(2)) - 1100000.0));
            iNowM030 = (int)Math.Round(Conversion.Val(Strings.Mid(DateTime.Now.AsString(), 7, 4) + Strings.Mid(DateTime.Now.AsString(), 4, 2) + DateTime.Now.AsString().Left(2)));
            DataModul.DB_PersonTable.MoveLast();
            var num15 = 1L;
            var num16 = (long)Math.Round(Conversion.Val(DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt()));
            num5 = num15;
            while (num5 <= num16)
            {
                iPersInArb = (int)num5;
                DataModul.DB_EventTable.Index = "BeSu";
                num14 = EEventArt.eA_Death;
                while (num14 <= EEventArt.eA_Burial)
                {
                    DataModul.DB_EventTable.Seek("=", num14.AsString(), iPersInArb.AsString());
                    if (!DataModul.DB_EventTable.NoMatch)
                    {
                        if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.tot].Value) && DataModul.DB_EventTable.Fields[EventFields.tot].AsString() == "J")
                            break;
                        else
                        if (DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDouble() > 0.0)
                        {
                            if (!(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDouble() < iNowM030))
                            {
                                if (DataModul.NB_SperrPers_AppendC(iPersInArb))
                                    break;
                            }
                            else
                                break;
                        }
                    }
                    num14++;
                }
                if (num14 > EEventArt.eA_Burial)
                {
                    num14 = EEventArt.eA_Birth;
                    while (num14 <= EEventArt.eA_Baptism)
                    {
                        DataModul.DB_EventTable.Seek("=", num14.AsString(), iPersInArb.AsString());
                        if (!DataModul.DB_EventTable.NoMatch
                            && DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDouble() > 0.0)
                        {
                            if (!(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDouble() < iNowM110))
                            {
                                if (DataModul.NB_SperrPers_AppendC(iPersInArb))
                                    break;
                            }
                            else
                                break;
                        }
                        num14++;
                    }
                }

                num5++;
            }
        }
        Cursor.Current = Cursors.UpArrow;
    }
    public void FamSonstles(int FamInArb)
    {
        int try0000_dispatch = -1;
        int num = default;
        string text = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        short num5 = default;
        short num6 = default;
        string Fld = default;
        short num7 = default;
        string Datu = default;
        string Dasich = default;
        int famInArb = default;
        short num8 = default;
        EEventArt _eArt = default;
        string NamenSuch_Wort;
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
                            num = 1;
                            text = "";
                            goto IL_000a;
                        case 8819:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_1cb1;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 3021)
                                {
                                    var Mldg2 = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                                    if (Interaction.MsgBox(Mldg2, MsgBoxStyle.OkCancel | MsgBoxStyle.Question, "Fehler") == MsgBoxResult.Cancel)
                                    {
                                        ProjectData.EndApp();
                                    }
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_1cad;
                                }
                                if (Information.Err().Number == 94)
                                {
                                    _Modul1.Instance.UbgT = "";
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_1cb1;
                                }
                                var Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                                if (Interaction.MsgBox(Mldg, MsgBoxStyle.OkCancel | MsgBoxStyle.Question, "Fehler") == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1cad;
                            }
                        end_IL_0000:
                            break;
                        IL_000a:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            _eArt = EEventArt.eA_602;
                            while (_eArt <= EEventArt.eA_603)
                            {
                                DataModul.DB_EventTable.Index = "Besu";
                                DataModul.DB_EventTable.Seek("=", _eArt.AsString(), FamInArb.AsString());
                                num5 = 1;
                                while (num5 <= 200)
                                {
                                    num6 = 0;
                                    Fld = "";
                                    IEventData cEv;
                                    if (!DataModul.DB_EventTable.EOF
                                        && !DataModul.DB_EventTable.NoMatch)
                                    {
                                        cEv = new CEventData(DataModul.DB_EventTable);
                                        if ((Information.IsDBNull(cEv.iPrivacy) || cEv.iPrivacy <= Options_priv.AsInt()) && !(cEv.iLfNr == 0))
                                        {
                                            if (!Information.IsDBNull(cEv.iDatumText)
                                                && (cEv.iDatumText != 0)
                                                && cEv.dDatumV != default)
                                            {
                                                text = "INT ";
                                            }
                                            if (DataModul.DB_WitnessTable.RecordCount > 0)
                                            {
                                                DataModul.DB_WitnessTable.MoveFirst();
                                                DataModul.DB_WitnessTable.Index = "ZeugSu";
                                                DataModul.DB_WitnessTable.Seek("=", FamInArb, "10", _eArt, cEv.iLfNr);
                                                if (!DataModul.DB_WitnessTable.NoMatch)
                                                {
                                                    num6 = 1;
                                                }
                                            }
                                            if (Conversions.ToBoolean(cEv.iKBem > 0
                                                | cEv.dDatumV != default
                                                | cEv.dDatumB != default
                                                | cEv.iOrt > 0
                                                | num6 > 0))
                                            {
                                                if (!(cEv.iPerFamNr != FamInArb)
                                                    && !(cEv.eArt != _eArt))
                                                {
                                                    num7 = 0;
                                                    switch (_eArt)
                                                    {
                                                        case EEventArt.eA_603:
                                                            if (Datschu[4] > 0 && cEv.dDatumV.AsInt() > Datschu[4])
                                                            {
                                                                num5 = 201;
                                                                continue; // goto IL_001f;
                                                            }

                                                            if (cEv.iKBem > 0)
                                                            {
                                                                NamenSuch_Wort = DataModul.TextLese1(cEv.iKBem);
                                                                Fld = NamenSuch_Wort.Trim();
                                                                NamenSuch_Wort = "";
                                                                if (GedAus.Options_Uml > 0)
                                                                {
                                                                    Fld = _Modul1.Instance.GedAus.Text_ChngEncoding4(Fld, GedAus.Options_Uml);
                                                                }
                                                                WriteGedLine("1 " + cGed_EVEN + " " + Fld, false);
                                                                num7 = 1;
                                                            }
                                                            else
                                                            {
                                                                WriteGedLine("1 " + cGed_EVEN, true);
                                                                num7 = 1;
                                                            }
                                                            if (!Information.IsDBNull(cEv.iArtText))
                                                            {
                                                                if (cEv.iArtText > 0)
                                                                {
                                                                    NamenSuch_Wort = DataModul.TextLese1(cEv.iArtText);
                                                                    Fld = "2 " + cGed_TYPE + " " + NamenSuch_Wort.Trim();
                                                                    NamenSuch_Wort = "";
                                                                    if (GedAus.Options_Uml > 0)
                                                                    {
                                                                        Fld = _Modul1.Instance.GedAus.Text_ChngEncoding4(Fld, GedAus.Options_Uml);
                                                                    }
                                                                    WriteGedLine(Fld, true);
                                                                }
                                                                else
                                                                {
                                                                    Fld = "2 " + cGed_TYPE + " Sonst. Ereignis";
                                                                    WriteGedLine(Fld, true);
                                                                }
                                                            }
                                                            break;

                                                        case EEventArt.eA_602:
                                                            if (Datschu[4] > 0 && cEv.dDatumV.AsInt() > Datschu[4])
                                                            {
                                                                num5 = 201;
                                                                continue; // goto IL_001f;
                                                            }
                                                            WriteGedLine("1 " + cGed_RESI + " ", true);
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                    Datu = "";
                                                    Datsicha = "";
                                                    if (cEv.sDatumV_S.Trim() != "")
                                                    {
                                                        Dasich = cEv.sDatumV_S.Trim();
                                                        _Modul1.Instance.Sichwand(Dasich, cEv.sDatumV_S.Trim(), cEv.dDatumB, _eArt);
                                                        Datu = Dasich;
                                                        Datsicha = Datu;
                                                    }
                                                    if (cEv.dDatumV != default)
                                                    {
                                                        Dat = cEv.dDatumV.AsString().Trim();
                                                        Dat = Dat.Date2DotDateStr2();
                                                        Datu = Datu.TrimEnd() + " " + Dat.Trim();
                                                        Dat = "";
                                                    }
                                                    if ((cEv.sDatumB_S.Trim().ToUpper() == "A")
                                                            && cEv.dDatumB != default)
                                                    {
                                                        Datu += " AND ";
                                                        if (cEv.dDatumB != default)
                                                        {
                                                            Dat = cEv.dDatumB.AsString().Trim();
                                                            Dat = Dat.Date2DotDateStr2();
                                                            Datu = Datu.TrimEnd() + " " + Dat.Trim();
                                                            Dat = "";
                                                        }
                                                    }
                                                    else
                                                    if (Conversions.ToBoolean((cEv.sDatumB_S.Trim() != "")
                                                        || cEv.dDatumB != default))
                                                    {
                                                        if ((Datu.Trim() != "") & (cEv.sDatumV_S.Trim() == ""))
                                                        {
                                                            Datu = "FROM " + Datu.Trim();
                                                            Datsicha = "FROM";
                                                        }
                                                        Zweitdatum(ref Datu, Datsicha, _eArt, cEv);
                                                    }
                                                    lErl = 5;
                                                    if (!Information.IsDBNull(cEv.iPrivacy))
                                                    {
                                                        if (cEv.iPrivacy == 1)
                                                        {
                                                            WriteGedLine("2 " + cGed_RESN + " privacy", true);
                                                        }
                                                        else
                                                        if (cEv.iPrivacy == 2)
                                                        {
                                                            WriteGedLine("2 " + cGed_RESN + " confidential", true);
                                                        }
                                                    }
                                                    if (!Information.IsDBNull(cEv.iDatumText))
                                                    {
                                                        NamenSuch_Wort = DataModul.TextLese1(cEv.iDatumText);
                                                        if (GedAus.Options_Uml > 0)
                                                        {
                                                            NamenSuch_Wort = _Modul1.Instance.GedAus.Text_ChngEncoding4(NamenSuch_Wort, GedAus.Options_Uml);
                                                        }
                                                        if (NamenSuch_Wort.Trim() != "")
                                                        {
                                                            Datu = Datu + " (" + NamenSuch_Wort.Trim() + ")";
                                                            NamenSuch_Wort = "";
                                                        }
                                                    }
                                                    if (Datu != "")
                                                    {
                                                        WriteGedLine("2 " + cGed_DATE + " " + text + Datu.Trim(), true);
                                                        text = "";
                                                    }
                                                    if (cEv.iOrt > 0)
                                                    {
                                                        Ortnr = cEv.iOrt;
                                                        _Modul1.Instance.UbgT = _Modul1.Instance.UbgT = _Modul1.Instance.ortles1(Ortnr, 0);
                                                    }
                                                    if (cEv.sOrt_S.Trim() != "")
                                                    {
                                                        WriteGedLine("3 " + cGed__SIC + " " + cEv.sOrt_S.Trim(), true);
                                                    }
                                                    if (!Information.IsDBNull(cEv.sZusatz)
                                                        && cEv.sZusatz.Trim() != "")
                                                    {
                                                        WriteGedLine("3 " + cGed__ZUS + " " + cEv.sZusatz.Trim(), true);
                                                    }
                                                    if (!Information.IsDBNull(cEv.sReg)
                                                        && (cEv.sReg != " "))
                                                    {
                                                        Fld = "2 " + cGed_REFN + " " + cEv.sReg.Trim();
                                                        if (GedAus.Options_Uml > 0)
                                                        {
                                                            Fld = _Modul1.Instance.Strngs_Umlaute4(Fld, GedAus.Options_Uml);
                                                        }
                                                        WriteGedLine(Fld.Replace("\n", " "), true);
                                                    }
                                                    if (num7 == 0)
                                                    {
                                                        if (cEv.iKBem > 0)
                                                        {
                                                            if (_eArt == EEventArt.eA_602)
                                                            {
                                                                NamenSuch_Wort = DataModul.TextLese1(cEv.iKBem);
                                                            }
                                                            if (_eArt == EEventArt.eA_603)
                                                            {
                                                                NamenSuch_Wort = DataModul.TextLese1(cEv.iKBem);
                                                            }
                                                            Fld = NamenSuch_Wort.Trim();
                                                            NamenSuch_Wort = "";
                                                            if (GedAus.Options_Uml > 0)
                                                            {
                                                                Fld = _Modul1.Instance.Strngs_Umlaute4(Fld, GedAus.Options_Uml);
                                                            }
                                                            ADRText = Fld;
                                                            Fld = "";
                                                        }
                                                    }
                                                    if (_eArt == EEventArt.eA_602)
                                                    {
                                                        Fld = "";
                                                        if (!Information.IsDBNull(cEv.iHausNr))
                                                        {
                                                            if (cEv.iHausNr != 0)
                                                            {
                                                                NamenSuch_Wort = DataModul.TextLese1(cEv.iHausNr);
                                                                Fld = NamenSuch_Wort.Trim();
                                                                NamenSuch_Wort = "";
                                                                if (GedAus.Options_Uml > 0)
                                                                {
                                                                    Fld = _Modul1.Instance.Strngs_Umlaute4(Fld, GedAus.Options_Uml);
                                                                }
                                                            }
                                                        }
                                                        if (ADRText != "")
                                                        {
                                                            WriteGedLine(("2 " + cGed_ADDR + " " + ADRText + " " + Fld).Trim(), false);
                                                            if (!Options_Hausnr)
                                                            {
                                                                WriteGedLine(("3 _STRASSE " + ADRText + " " + Fld).Trim(), true);
                                                                ADRText = "";
                                                            }
                                                            else
                                                            {
                                                                WriteGedLine("3 _STRASSE " + ADRText, true);
                                                                ADRText = "";
                                                                if (Fld.Trim() != "")
                                                                {
                                                                    WriteGedLine(("3 " + cGed__NUM + " " + Fld).Trim(), true);
                                                                    Fld = "";
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    if (cEv.iPlatz > 0)
                                                    {
                                                        NamenSuch_Wort = DataModul.TextLese1(cEv.iPlatz);
                                                        Fld = "2 " + cGed__SITE + " " + NamenSuch_Wort.Trim();
                                                        NamenSuch_Wort = "";
                                                        if (GedAus.Options_Uml > 0)
                                                        {
                                                            Fld = _Modul1.Instance.Strngs_Umlaute4(Fld, GedAus.Options_Uml);
                                                        }
                                                        WriteGedLine(Fld, true);
                                                    }
                                                    if (cEv.sBem[1].Trim() != "")
                                                    {
                                                        if (Hi[2])
                                                        {
                                                            _Modul1.Instance.UbgT = "2 " + cGed__COM + " " + cEv.sBem[1].Trim();
                                                            Historie(_Modul1.Instance.UbgT);
                                                        }
                                                    }
                                                    if (cEv.sBem[2].Trim() != "")
                                                    {
                                                        if (Hi[3])
                                                        {
                                                            _Modul1.Instance.UbgT = "2 " + cGed_NOTE + " " + cEv.sBem[2].Trim();
                                                            Historie(_Modul1.Instance.UbgT);
                                                        }
                                                    }
                                                    if (Options_Quellaus == 6)
                                                    {
                                                        Quellenaus((EEventArt)_eArt);
                                                    }
                                                }
                                                else
                                                {
                                                    num5 = 201;
                                                    continue; // goto IL_001f;
                                                }
                                            }
                                            num6 = 0;
                                            famInArb = FamInArb;
                                            if (DataModul.DB_WitnessTable.RecordCount > 0)
                                            {
                                                num8 = 1;
                                                DataModul.DB_WitnessTable.MoveFirst();
                                                DataModul.DB_WitnessTable.Index = "ZeugSu";
                                                DataModul.DB_WitnessTable.Seek("=", famInArb, "10", _eArt, cEv.iLfNr);
                                                while (num8 <= 99
                                                    && !DataModul.DB_WitnessTable.EOF
                                                    && !DataModul.DB_WitnessTable.NoMatch
                                                    && !Conversions.ToBoolean(DataModul.DB_WitnessTable.Fields[WitnessFields.FamNr].AsInt() != famInArb
                                                        | DataModul.DB_WitnessTable.Fields[WitnessFields.Kennz].AsInt() != 10
                                                        | DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsEnum<EEventArt>() != _eArt) && !DataModul.DB_WitnessTable.NoMatch)
                                                {
                                                    if (!DataModul.DB_WitnessTable.NoMatch
                                                        && (DataModul.NB_SperrPers_Count <= 0 || !DataModul.NB_SperrPers_Exists(DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsInt())))
                                                    {
                                                        Fld = "2 " + cGed_ASSO + " @I" + DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsInt().AsString().Trim() + "@";
                                                        DataModul.NB_Witness2Table.Index = "Per";
                                                        DataModul.NB_Witness2Table.Seek("=", DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsInt());
                                                        if (DataModul.NB_Witness2Table.RecordCount <= 0 || !DataModul.NB_Witness2Table.NoMatch)
                                                        {
                                                            WriteGedLine(Fld, false);
                                                            Fld = "3 " + cGed_RELA + " Zeuge";
                                                            WriteGedLine(Fld, true);
                                                        }
                                                    }
                                                    lErl = 22;
                                                    DataModul.DB_WitnessTable.MoveNext();
                                                    num8++;
                                                }
                                            }
                                            Fld = "";
                                            if (!Information.IsDBNull(cEv.sBem[4]) && cEv.sBem[4].Trim() != "")
                                            {
                                                Fld = cEv.sBem[4].Trim();
                                                TextTeilen(Fld, "2 " + cGed__WITN + " ");

                                            }
                                        }
                                        lErl = 33;
                                        DataModul.DB_EventTable.MoveNext();
                                        num5++;
                                    }
                                }
                                lErl = 188;
                                _eArt++;
                            }
                        IL_1cad:
                            num4 = num2;
                            goto IL_1cb5;
                        IL_1cb1:
                            num4 = unchecked(num2 + 1);
                            goto IL_1cb5;
                        IL_1cb5:
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
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 8819;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void Sonstles(int PersInArb)
    {
        int try0000_dispatch = -1;
        int num = default;
        string text = default;
        int num2 = default;
        int num3 = default;
        byte b = default;
        EEventArt L = default;
        int lErl = default;
        string Fld = default;
        short num5 = default;
        string Datu = default;
        string Dasich = default;
        int persInArb = default;
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
                    int Ortnr;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            text = "";
                            goto IL_0009;
                        case 7413:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_181f;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 3021)
                                {
                                    Interaction.MsgBox(DataModul.DB_EventTable.EOF);
                                }
                                if (Information.Err().Number == 13)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_181f;
                                }
                                if (Information.Err().Number == 94)
                                {
                                    _Modul1.Instance.UbgT = "";
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_181f;
                                }
                                if (Interaction.MsgBox("Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description, MsgBoxStyle.OkCancel | MsgBoxStyle.Question, "Fehler") == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_1823;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            L = EEventArt.eA_105;
                            goto IL_0018;
                        IL_0018: // <========== 3
                                 // <========== 3
                            num = 4;
                            Datsicha = "";
                            b = 1;
                            foreach (var cEv in DataModul.Event.ReadEventsBeSu(PersInArb, L))
                            {
                                IFieldsCollection cEvF = DataModul.DB_EventTable.Fields;
                                if (Information.IsDBNull(cEvF[nameof(EventFields.priv)].Value)
                                    || cEvF[nameof(EventFields.priv)].AsInt() <= Options_priv.AsInt())
                                {
                                    Datsicha = "";
                                    Fld = "";
                                    Fld = "";
                                    if (!Information.IsDBNull(cEvF[nameof(EventFields.DatumText)].Value)
                                        && (cEvF[nameof(EventFields.DatumText)].AsInt() != 0)
                                        && cEvF[nameof(EventFields.DatumV)].AsInt() > 0)
                                    {
                                        text = "INT ";
                                    }
                                    Fld = (


                                                $"{cEvF[nameof(EventFields.KBem)].AsString()}{cEvF[nameof(EventFields.ArtText)].AsString()}" + cEvF[nameof(EventFields.Bem1)].Value + cEvF[nameof(EventFields.Bem2)].Value).AsString();
                                    if (Fld.Trim() != "")
                                    {
                                        num5 = 0;
                                        switch (L)
                                        {
                                            case EEventArt.eA_105:
                                                Fld = "";
                                                Fld = (cEvF[EventFields.KBem].AsString() + cEvF[EventFields.ArtText].AsString() + cEvF[EventFields.Bem1].Value + cEvF[EventFields.Bem2].AsString());
                                                if (Fld.Trim() != "")
                                                {
                                                    NamenSuch_Wort = DataModul.TextLese1(cEvF[EventFields.KBem].AsInt());
                                                    Fld = NamenSuch_Wort.Trim();
                                                    NamenSuch_Wort = "";
                                                    if (GedAus.Options_Uml > 0)
                                                    {
                                                        Fld = _Modul1.Instance.Strngs_Umlaute4(Fld, GedAus.Options_Uml);
                                                    }
                                                    WriteGedLine("1 " + cGed_EVEN + " " + Fld, false);
                                                    num5 = 1;
                                                }
                                                else
                                                {
                                                    WriteGedLine("1 " + cGed_EVEN, false);
                                                    num5 = 1;
                                                }
                                                if (cEvF[EventFields.ArtText].AsInt() > 0)
                                                {
                                                    NamenSuch_Wort = DataModul.TextLese1(cEvF[EventFields.ArtText].AsInt());
                                                    Fld = "2 " + cGed_TYPE + " " + NamenSuch_Wort.Trim();
                                                    NamenSuch_Wort = "";
                                                    if (GedAus.Options_Uml > 0)
                                                    {
                                                        Fld = _Modul1.Instance.Strngs_Umlaute4(Fld, GedAus.Options_Uml);
                                                    }
                                                    WriteGedLine(Fld, false);
                                                    if (d >= Gedaus_DiskSize)
                                                    {
                                                        _Modul1.Instance.GedAus_Diskvoll();
                                                    }
                                                }
                                                else
                                                {
                                                    Fld = "2 " + cGed_TYPE + " Sonst. Ereignis";
                                                    WriteGedLine(Fld, true);
                                                }
                                                break;
                                            case EEventArt.eA_106:
                                                WriteGedLine("1 " + cGed__HEIM + " ", false);
                                                break;
                                            default:
                                                break;
                                        }
                                        Datu = "";
                                        if (cEvF[EventFields.DatumV_S].AsString().Trim() != "")
                                        {
                                            Dasich = cEvF[EventFields.DatumV_S].AsString().Trim();
                                            _Modul1.Instance.Sichwand(Dasich, DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString().Trim(), DataModul.DB_EventTable.Fields[EventFields.DatumB].AsDate(), L);
                                            Datu = Dasich;
                                            Datsicha = Datu;
                                        }
                                        if (cEvF[EventFields.DatumV].AsInt() > 0)
                                        {
                                            Dat = cEvF[EventFields.DatumV].AsString().Trim();
                                            Dat = Dat.Date2DotDateStr2();
                                            Datu = Datu.TrimEnd() + " " + Dat.Trim();
                                            Dat = "";
                                        }
                                        if (Conversions.ToBoolean(Operators.AndObject(Operators.CompareString(Strings.UCase(cEvF[EventFields.DatumB_S].AsString().Trim()), "A", TextCompare: false) == 0, cEvF[EventFields.DatumB].AsInt() > 0)))
                                        {
                                            Datu += " AND ";
                                            if (cEvF[EventFields.DatumB].AsInt() > 0)
                                            {
                                                Dat = cEvF[EventFields.DatumB].AsString().Trim();
                                                Dat = Dat.Date2DotDateStr2();
                                                Datu = Datu.TrimEnd() + " " + Dat.Trim();
                                                Dat = "";
                                            }
                                        }
                                        else if (Conversions.ToBoolean(Operators.CompareString(cEvF[EventFields.DatumB_S].AsString().Trim(), "", TextCompare: false) != 0 | cEvF[EventFields.DatumB].AsInt() > 0))
                                        {
                                            if ((Datu.Trim() != "") & (cEvF[EventFields.DatumV_S].AsString().Trim() == ""))
                                            {
                                                Datu = "FROM " + Datu.Trim();
                                                Datsicha = "FROM";
                                            }
                                            Zweitdatum(ref Datu, Datsicha, L, cEv);
                                        }
                                        lErl = 5;
                                        if (!Information.IsDBNull(cEvF[EventFields.DatumText].Value))
                                        {
                                            NamenSuch_Wort = DataModul.TextLese1(cEvF[EventFields.DatumText].AsInt());
                                            if (GedAus.Options_Uml > 0)
                                            {
                                                NamenSuch_Wort = _Modul1.Instance.Strngs_Umlaute4(NamenSuch_Wort, GedAus.Options_Uml);
                                            }
                                            if (NamenSuch_Wort.Trim() != "")
                                            {
                                                Datu = Datu + " (" + NamenSuch_Wort.Trim() + ")";
                                                NamenSuch_Wort = "";
                                            }
                                        }
                                        if (!Information.IsDBNull(cEvF[EventFields.priv].Value))
                                        {
                                            string value = cEvF[EventFields.priv].AsString();
                                            if (value == "1")
                                            {
                                                WriteGedLine("2 " + cGed_RESN + " privacy", true);
                                            }
                                            else if (value == "2")
                                            {
                                                WriteGedLine("2 " + cGed_RESN + " confidential", true);
                                            }
                                        }
                                        if (Datu != "")
                                        {
                                            WriteGedLine("2 " + cGed_DATE + " " + text + Datu.Trim(), true);
                                        }
                                        if (cEvF[EventFields.Ort].AsInt() > 0)
                                        {
                                            Ortnr = cEvF[EventFields.Ort].AsInt();
                                            _Modul1.Instance.UbgT = _Modul1.Instance.ortles1(Ortnr, 0);
                                        }
                                        if (cEvF[EventFields.Ort_S].AsString().Trim() != "")
                                        {
                                            WriteGedLine("3 " + cGed__SIC + " " + cEvF[EventFields.Ort_S].AsString().Trim(), true);
                                        }
                                        if (!Information.IsDBNull(cEvF[EventFields.Zusatz].Value) && cEvF[EventFields.Zusatz].AsString().Trim() != "")
                                        {
                                            WriteGedLine("2 " + cGed__ZUS + " " + cEvF[EventFields.Zusatz].AsString().Trim(), true);
                                        }
                                        if (!Information.IsDBNull(cEvF[EventFields.Reg].Value) && (cEvF[EventFields.Reg].AsString() != " "))
                                        {
                                            Fld = "2 " + cGed_REFN + " " + cEvF[EventFields.Reg].AsString().Trim();
                                            if (GedAus.Options_Uml > 0)
                                            {
                                                Fld = _Modul1.Instance.Strngs_Umlaute4(Fld, GedAus.Options_Uml);
                                            }
                                            WriteGedLine(Fld.Replace("\n", " "), true);
                                        }
                                        if (num5 == 0 && cEvF[EventFields.KBem].AsInt() > 0)
                                        {
                                            NamenSuch_Wort = DataModul.TextLese1(cEvF[EventFields.KBem].AsInt());
                                            Fld = NamenSuch_Wort.Trim();
                                            NamenSuch_Wort = "";
                                            if (GedAus.Options_Uml > 0)
                                            {
                                                Fld = _Modul1.Instance.Strngs_Umlaute4(Fld, GedAus.Options_Uml);
                                            }
                                            WriteGedLine("2 " + cGed_ADDR + " " + Fld, false);
                                        }
                                        if (cEvF[EventFields.Platz].AsInt() > 0)
                                        {
                                            NamenSuch_Wort = DataModul.TextLese1(cEvF[EventFields.Platz].AsInt());
                                            Fld = "2 " + cGed__SITE + " " + NamenSuch_Wort.Trim();
                                            NamenSuch_Wort = "";
                                            NamenSuch_Wort = "";
                                            if (GedAus.Options_Uml > 0)
                                            {
                                                Fld = _Modul1.Instance.Strngs_Umlaute4(Fld, GedAus.Options_Uml);
                                            }
                                            WriteGedLine(Fld, true);
                                            Fld = "";
                                        }
                                        if (cEvF[EventFields.Bem1].AsString().Trim() != "" && Hi[2])
                                        {
                                            _Modul1.Instance.UbgT = "2 " + cGed__COM + " " + cEvF[EventFields.Bem1].AsString().Trim();
                                            Historie(_Modul1.Instance.UbgT);
                                        }
                                        if (cEvF[EventFields.Bem2].AsString().Trim() != "" && Hi[3])
                                        {
                                            _Modul1.Instance.UbgT = "2 " + cGed_NOTE + " " + cEvF[EventFields.Bem2].AsString().Trim();
                                            Historie(_Modul1.Instance.UbgT);
                                        }
                                        if (_Modul1.Instance.GedAus.Options_Quellaus == 6)
                                        {
                                            _Modul1.Instance.Quellenaus((EEventArt)L);
                                        }
                                    }
                                    persInArb = PersInArb;
                                    if (DataModul.DB_WitnessTable.RecordCount > 0)
                                    {
                                        DataModul.DB_WitnessTable.MoveFirst();
                                        DataModul.DB_WitnessTable.Index = "ZeugSu";
                                        DataModul.DB_WitnessTable.Seek("=", persInArb, "10", L, cEvF[EventFields.LfNr]);
                                        if (!DataModul.DB_WitnessTable.NoMatch)
                                        {
                                            b2 = 1;
                                            while (unchecked(b2) <= 99u
                                                && !DataModul.DB_WitnessTable.EOF
                                                && !DataModul.DB_WitnessTable.NoMatch
                                                && !Conversions.ToBoolean(DataModul.DB_WitnessTable.Fields[WitnessFields.FamNr].AsInt() != persInArb
                                                  | DataModul.DB_WitnessTable.Fields[WitnessFields.Kennz].AsString() != "10" | DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsInt() != (int)L))
                                            {
                                                if (!DataModul.DB_WitnessTable.NoMatch
                                                    && (DataModul.NB_SperrPers_Count <= 0 || !DataModul.NB_SperrPers_Exists(DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsInt())))
                                                {
                                                    Fld = "2 " + cGed_ASSO + " @I" + DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsInt().AsString().Trim() + "@";
                                                    DataModul.NB_Witness2Table.Index = "Per";
                                                    DataModul.NB_Witness2Table.Seek("=", DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsInt());
                                                    if (DataModul.NB_Witness2Table.RecordCount <= 0
                                                        || !DataModul.NB_Witness2Table.NoMatch)
                                                    {
                                                        WriteGedLine(Fld, false);
                                                        Fld = "3 " + cGed_RELA + " Zeuge";
                                                        WriteGedLine(Fld, true);
                                                    }
                                                }
                                                lErl = 22;
                                                DataModul.DB_WitnessTable.MoveNext();
                                                b2++;
                                            }
                                        }
                                    }
                                    Fld = "";
                                    if (!Information.IsDBNull(cEvF[EventFields.Bem4].Value) && cEvF[EventFields.Bem4].AsString().Trim() != "")
                                    {
                                        Fld = cEvF[EventFields.Bem4].AsString().Trim();
                                        TextTeilen(Fld, "2 " + cGed__WITN + " ");
                                    }
                                }
                                lErl = 33;
                                DataModul.DB_EventTable.MoveNext();
                                b++;
                            }
                            L = (EEventArt)unchecked(L + 1);
                            if (L <= EEventArt.eA_106)
                            {
                                goto IL_0018;
                            }
                            else
                            {
                                break;
                            }
                        IL_181f: // <========== 3
                                 // <========== 3
                            num4 = unchecked(num2 + 1);
                            goto IL_1823;
                        IL_1823:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 4:
                                    goto IL_0018;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 7413;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void FamPersuch()
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
                    int nR;
                    string S_C;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 1972:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_067a;
                                    default:
                                        goto end_IL_0000;
                                }
                                number = Information.Err().Number;
                                _Modul1.Instance.UbgT = "Gedaus FamPersuch";
                                Interaction.MsgBox(_Modul1.Instance.UbgT, MsgBoxStyle.OkOnly, "Fehler " + Information.Err().Number.AsString() + " in Modul");
                                ProjectData.EndApp();
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_067e;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            _Modul1.Instance.eLKennz = DataModul.Person.GetSex(_Modul1.Instance.PersInArb) == "F" ? ELinkKennz.lkMother : ELinkKennz.lkFather;
                            string FamugNrS = "";
                            foreach (var cLink in DataModul.Link.ReadAllPers(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz))
                            {
                                if (!DataModul.NB_SperrFams_Exists(cLink.iFamNr))
                                {
                                    FamugNrS += Strings.Right("          " + cLink.iFamNr.AsString(), 10);
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
                            foreach (var cLink in DataModul.Link.ReadAllPers(_Modul1.Instance.PersInArb, ELinkKennz.lkChild))
                                if (!DataModul.NB_SperrFams_Exists(cLink.iFamNr))
                                {
                                    S_C = "C";
                                    Viel_Personen(cLink.iFamNr, ref S_C);
                                }
                            foreach (var cLink in DataModul.Link.ReadAllPers(_Modul1.Instance.PersInArb, ELinkKennz.lkAdoptedChild))
                                if (!DataModul.NB_SperrFams_Exists(cLink.iFamNr))
                                {
                                    FamugNr = cLink.iFamNr;
                                    S_C = "C";
                                    AnzP = Viel_Personen(FamugNr, ref S_C);
                                    if (AnzP > 1)
                                    {
                                        string text = "2 " + cGed_PEDI + " adopted";
                                        WriteGedLine(text, true);
                                        text = "";
                                    }
                                }
                            goto end_IL_0000_2;
                        IL_067a:
                            num4 = unchecked(num2 + 1);
                            goto IL_067e;
                        IL_067e:
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
                try0000_dispatch = 1972;
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
    public void Zweitdatum(ref string Datu, string Datsicha, EEventArt eArt, IEventData cEv)
    {
        string Dasich = cEv.sDatumB_S.Trim();
        if (cEv.sDatumB_S.Trim() != "")
        {
            _Modul1.Instance.Sichwand(Dasich, cEv.sDatumV_S, cEv.dDatumB, eArt);
            if (Datsicha != "")
            {
                switch (Datsicha)
                {
                    case CDateModifyer.cGedDate_BEF:
                        if ((cEv.sDatumB_S.Trim() != "") & (cEv.sDatumB_S.Trim() != CDateModifyer.cGedDate_AND))
                        {
                            Fehlliste(eArt, cEv.sDatumB_S.Trim(), cEv.iPerFamNr);
                        }
                        Datu = Datu + " " + cEv.sDatumB_S.Trim();
                        break;
                    case CDateModifyer.cGedDate_AFT:
                        if ((cEv.sDatumB_S.Trim() != "") & (cEv.sDatumB_S.Trim() != CDateModifyer.cGedDate_AND))
                        {
                            Fehlliste(eArt, cEv.sDatumB_S.Trim(), cEv.iPerFamNr);
                        }
                        Datu = Datu + " " + cEv.sDatumB_S.Trim();
                        break;
                    case CDateModifyer.cGedDate_ABT:
                        if (cEv.sDatumB_S.Trim() != "")
                        {
                            Fehlliste(eArt, cEv.sDatumB_S.Trim(), cEv.iPerFamNr);
                        }
                        Datu = Datu + " " + cEv.sDatumB_S.Trim();
                        break;
                    case CDateModifyer.cGedDate_CAL:
                        if (cEv.sDatumB_S.Trim() != "")
                        {
                            Fehlliste(eArt, cEv.sDatumB_S.Trim(), cEv.iPerFamNr);
                        }
                        Datu = Datu + " " + cEv.sDatumB_S.Trim();
                        Dasich = "";
                        break;
                    case CDateModifyer.cGedDate_FROM:
                        if ((cEv.sDatumB_S.Trim() != "") & (cEv.sDatumB_S.Trim() != CDateModifyer.cGedDate_TO))
                        {
                            Fehlliste(eArt, cEv.sDatumB_S.Trim(), cEv.iPerFamNr);
                        }
                        Datu = Datu + " " + cEv.sDatumB_S.Trim();
                        Dasich = "";
                        break;
                    case CDateModifyer.cGedDate_BET:
                        if ((cEv.sDatumB_S.Trim() != "") & (cEv.sDatumB_S.Trim() != CDateModifyer.cGedDate_AND))
                        {
                            Fehlliste(eArt, cEv.sDatumB_S.Trim(), cEv.iPerFamNr);
                        }
                        Datu = Datu + " " + cEv.sDatumB_S.Trim();
                        Dasich = "";
                        break;
                }
            }
        }
        else
        {
            if ((Datsicha != CDateModifyer.cGedDate_BET) & (Datsicha != "") & (Datsicha != CDateModifyer.cGedDate_FROM))
            {
                Fehlliste(eArt, cEv.sDatumB_S.Trim(), cEv.iPerFamNr, "a");
                Datu += " / ";
            }
            if (Datsicha == CDateModifyer.cGedDate_BET)
            {
                Datu += " AND ";
                Dasich = "";
            }
            if (Datsicha == CDateModifyer.cGedDate_FROM)
            {
                Datu += " TO ";
                Dasich = "";
            }
        }
        if (cEv.sDatumB_S.Trim() != "")
        {
            Datu = Datu + " " + cEv.sDatumB_S.Trim();
            Dasich = "";
        }

        if (cEv.dDatumV != default
                & cEv.dDatumB != default
                & cEv.sDatumB_S.Trim() == "")
        {
            Dat = cEv.dDatumB.AsString().Trim();
            Dat = Dat.Date2DotDateStr2();
            Datu = Datu.TrimEnd() + " TO " + Dat.Trim();
            Dat = "";
        }
        else if (cEv.dDatumB != default)
        {
            Dat = cEv.dDatumB.AsString().Trim();
            Dat = Dat.Date2DotDateStr2();
            Datu = Datu.TrimEnd() + " " + Dat.Trim();
            Dat = "";
        }

    }


    public void ChildSort(int FamInArb)
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
                        case 2146:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0714;
                                    default:
                                        goto end_IL_0000;
                                }
                                var Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
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
                                goto IL_0718;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            byte b2 = 0;
                            while (b2 <= 99u)
                            {
                                Childarray[b2] = "30000000";
                                b2++;
                            }
                            byte b = 0;
                            byte b3 = 1;
                            num = 8;
                            if (b3 <= 2u)
                            {
                                _Modul1.Instance.eLKennz = b3 switch
                                {
                                    1 => ELinkKennz.lkChild,
                                    2 => ELinkKennz.lkAdoptedChild,
                                    _ => ELinkKennz.lkNone,
                                };
                                b2 = b;
                                foreach (var cLink in DataModul.Link.ReadAllFams(FamInArb, _Modul1.Instance.eLKennz))
                                {
                                    if (!DataModul.NB_SperrPers_Exists(cLink.iPersNr))
                                    {

                                        var dt = DataModul.Event.GetPersonBirthOrBapt(cLink.iPersNr);
                                        string text2 = Strings.Right("00000000" + dt.AsInt().AsString(), 8);
                                        Childarray[b2] = text2 + cLink.iPersNr;
                                    }
                                    b++;
                                    b2++;
                                }
                                b3++;
                            }
                            Array.Sort(Childarray);
                            b2 = 0;
                            if (unchecked(b2) <= 99u)
                            {
                                if (Childarray[b2] == "30000000")
                                {
                                    goto end_IL_0000_2;
                                }
                                DataModul.NB_PersonTable.Index = "Per";
                                DataModul.NB_PersonTable.Seek("=", Strings.Mid(Childarray[b2], 10, 10));
                                if (!DataModul.NB_PersonTable.NoMatch | Allaus)
                                {
                                    string text = "1 " + cGed_CHIL + " @I" + Strings.Trim(Strings.Mid(Childarray[b2], 10, 10).AsString()) + "@";
                                    WriteGedLine(text, true);
                                }
                                b2++;
                            }
                            goto end_IL_0000_2;
                        IL_0714:
                            num4 = unchecked(num2 + 1);
                            goto IL_0718;
                        IL_0718:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 57:
                                case 73:
                                case 79:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 2146;
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

    public void Wandel()
    {
        _Modul1.Instance.sw.Close();
        FileSystem.Rename(TempPath + "Gedcom.ged", FILENAM);
        if (GedAus.Options_Uml != 3)
        {
            FileSystem.FileOpen(11, _Modul1.Instance.Verz + "33.bak", OpenMode.Output);
            FileSystem.FileClose();
            FileSystem.Kill(_Modul1.Instance.Verz + "33.bak");
            FileSystem.Rename(FILENAM, _Modul1.Instance.Verz + "33.bak");
            toansi("33.bak", FILENAM);
        }
    }

    public void TextTeilen(string UbgT4, string Kennung)
    {
        int u = default;
        short num8 = default;
        var UbgT = UbgT4;
        if (UbgT == "")
        {
            return;
        }
        UbgT = UbgT.Replace("\r\n", "\n").TrimEnd();
        UbgT = Kennung + UbgT.Trim();
        int bLevel = Kennung.Left(2).AsInt();
        var asLines = UbgT.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var xStart = true;
        foreach (var line in asLines)
        {
            var sLine = line ?? "";
            while (sLine.Length > 80)
            {
                // Find the last space before the 80th character
                int lastSpaceIndex = sLine.LastIndexOf(' ', 80 - Kennung.Length);
                if (lastSpaceIndex == -1) lastSpaceIndex = 80 - Kennung.Length;

                string part = sLine.Substring(0, lastSpaceIndex);

                Kennung = MLKennung(Kennung, bLevel, xStart, line != sLine);
                WriteGedLine(Kennung + part.Trim(), true);

                sLine = sLine.Substring(lastSpaceIndex).Trim();
            }
            if (sLine.Length > 0)
            {
                // Write the remaining part of the line
                Kennung = MLKennung(Kennung, bLevel, xStart, line != sLine);
                WriteGedLine(Kennung + sLine.Trim(), true);
            }
            xStart = false;
        }

        static string MLKennung(string Kennung, int bLevel, bool xStart, bool lineChanged)
        {
            if (lineChanged)
            {
                Kennung = $"{bLevel + 1} {cGed_CONC} ";
            }
            else if (!xStart)
            {
                Kennung = $"{bLevel + 1} {cGed_CONT} ";
            }
            return Kennung;
        }
    }
    public string ortles(int Ortnr)
    {
        int try0000_dispatch = -1;
        int num = default;
        string[] array = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        string A = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    string[] array2;
                    string[] array3;
                    Type typeFromHandle;
                    object[] array4;
                    IField field;
                    object[] array5;
                    bool[] array6;
                    object obj;
                    string[] array7;
                    Type typeFromHandle2;
                    object[] array8;
                    IField field2;
                    object[] array9;
                    bool[] array10;
                    object obj2;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            array = new string[13];
                            goto IL_000b;
                        case 6901:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_172b;
                                    default:
                                        goto end_IL_0000;
                                }
                                lErl = 10;
                                var Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
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
                                goto IL_172f;
                            }
                        end_IL_0000:
                            break;
                        IL_000b:
                            num = 2;
                            A = "";
                            byte b = 1;
                            while (unchecked(b) <= 12u)
                            {
                                array[b] = "";
                                b = (byte)unchecked((uint)(b + 1));
                            }
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            DataModul.DB_PlaceTable.Index = "OrtNr";
                            DataModul.DB_PlaceTable.Seek("=", Ortnr);
                            if (!DataModul.DB_PlaceTable.NoMatch)
                            {
                                NamenSuch_Wort = DataModul.TextLese1(DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt());
                                array[1] = NamenSuch_Wort;
                                NamenSuch_Wort = "";
                                array[1] = Strings.Replace(array[1], ",", ";");
                                NamenSuch_Wort = DataModul.TextLese1(DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt());
                                array[2] = NamenSuch_Wort;
                                NamenSuch_Wort = "";
                                array[2] = Strings.Replace(array[2], ",", ";");
                                if (array[2].Trim() != "")
                                {
                                    if (Options_Schalt1 == 6)
                                    {
                                        array[2] = "-" + array[2];
                                    }
                                }
                                NamenSuch_Wort = DataModul.TextLese1(DataModul.DB_PlaceTable.Fields[PlaceFields.Kreis].AsInt());
                                array[3] = NamenSuch_Wort;
                                NamenSuch_Wort = "";
                                array[3] = Strings.Replace(array[3], ",", ";");
                                NamenSuch_Wort = DataModul.TextLese1(DataModul.DB_PlaceTable.Fields[PlaceFields.Land].AsInt());
                                array[4] = NamenSuch_Wort;
                                NamenSuch_Wort = "";
                                array[4] = Strings.Replace(array[4], ",", ";");
                                NamenSuch_Wort = DataModul.TextLese1(DataModul.DB_PlaceTable.Fields[PlaceFields.Staat].AsInt());
                                array[5] = NamenSuch_Wort;
                                NamenSuch_Wort = "";
                                array[5] = Strings.Replace(array[5], ",", ";");
                                if (!Modul1.GedAus.Internet)
                                {
                                    if (Modul1.GedAus.Options_Schalt1 == 6)
                                    {
                                        if (array[3].TrimEnd() != "")
                                        {
                                            array[3] = ", " + array[3].TrimEnd();
                                        }
                                        if (array[4].TrimEnd() != "")
                                        {
                                            array[4] = ", " + array[4].TrimEnd();
                                        }
                                        if (array[5].TrimEnd() != "")
                                        {
                                            array[5] = ", " + array[5].TrimEnd();
                                        }
                                        UbgT = array[1].TrimEnd() + array[2].TrimEnd() + array[3].TrimEnd() + array[4].TrimEnd() + array[5].TrimEnd();
                                        if (GedAus.Options_Uml > 0)
                                        {
                                            UbgT = Modul1.Strngs_Umlaute4(UbgT, uml: GedAus.Options_Uml);
                                        }
                                        WriteGedLine("2 " + cGed_PLAC + " " + UbgT, true);
                                        UbgT = "";
                                        goto IL_0861;
                                    }
                                    if (Options_Schalt1 == 7)
                                    {
                                        UbgT = array[1].TrimEnd() + "," + array[3].TrimEnd() + "," + array[4].TrimEnd() + "," + array[5].TrimEnd();
                                        array2 = new string[7]
                                        {
                                    UbgT, ",,",                          array[2].TrimEnd(), ",,",
                      null,
                      null,
                      null
                                        }
                                        ;
                                        array3 = array2;
                                        typeFromHandle = typeof(Strings);
                                        array4 = new object[1];
                                        field = DataModul.DB_PlaceTable.Fields[PlaceFields.Terr];
                                        array4[0] = field.Value;
                                        array5 = array4;
                                        array6 = new bool[1]
                                        {
                                    true
                                        }
                                        ;
                                        obj = NewLateBinding.LateGet(null, typeFromHandle, "UCase", array5, null, null, array6);
                                        if (array6[0])
                                        {
                                            field.Value = array5[0];
                                        }
                                        array3[4] = obj.AsString().Trim();
                                        array2[5] = ",";
                                        array7 = array2;
                                        typeFromHandle2 = typeof(Strings);
                                        array8 = new object[1];
                                        field2 = DataModul.DB_PlaceTable.Fields[PlaceFields.Staatk];
                                        array8[0] = field2.Value;
                                        array9 = array8;
                                        array10 = new bool[1]
                                        {
                                    true
                                        }
                                        ;
                                        obj2 = NewLateBinding.LateGet(null, typeFromHandle2, "UCase", array9, null, null, array10);
                                        if (array10[0])
                                        {
                                            field2.Value = array9[0];
                                        }
                                        array7[6] = obj2.AsString().Trim();
                                        UbgT = string.Concat(array2);
                                        if (GedAus.Options_Uml > 0)
                                        {
                                            UbgT = Modul1.Strngs_Umlaute4(UbgT, uml: GedAus.Options_Uml);
                                        }
                                        WriteGedLine("2 " + cGed_PLAO + " " + UbgT, true);
                                        UbgT = "";
                                        goto end_IL_0000_2;
                                    }
                                    if (DataModul.DB_PlaceTable.Fields[PlaceFields.PLZ].AsString() != "0")
                                    {
                                        array[10] = DataModul.DB_PlaceTable.Fields[PlaceFields.PLZ].AsString().Trim();
                                    }
                                    UbgT = array[1].TrimEnd() + array[3].TrimEnd() + array[4].TrimEnd() + array[5].TrimEnd();
                                    if (UbgT == "")
                                    {
                                        goto end_IL_0000_2;
                                    }
                                    UbgT = "";
                                    UbgT = array[1].TrimEnd() + "," + array[3].TrimEnd() + "," + array[4].TrimEnd() + "," + array[5].TrimEnd();
                                    UbgT = UbgT + ",," + array[2].TrimEnd();
                                    if (GedAus.Options_Uml > 0)
                                    {
                                        UbgT = Modul1.Strngs_Umlaute4(UbgT, uml: GedAus.Options_Uml);
                                    }
                                    WriteGedLine("2 " + cGed_PLAC + " " + UbgT, true);
                                    UbgT = "";
                                    goto IL_0861;
                                }
                                if (array[2] != "")
                                {
                                    array[2] = "-" + array[2];
                                }
                                UbgT = array[1].TrimEnd() + array[2].TrimEnd();
                                if (GedAus.Options_Uml > 0)
                                {
                                    UbgT = Modul1.Strngs_Umlaute4(UbgT, uml: GedAus.Options_Uml);
                                }
                                WriteGedLine("2 " + cGed_PLAC + " " + UbgT, true);
                                UbgT = "";

                            }
                            else
                            {
                                UbgT = "";
                            }
                            goto end_IL_0000_2;
                        IL_0861: // <========== 3
                                 // <========== 3
                            num = 92;
                            array[11] = DataModul.DB_PlaceTable.Fields[PlaceFields.L].AsString().Trim();
                            array[12] = DataModul.DB_PlaceTable.Fields[PlaceFields.B].AsString().Trim();
                            DataModul.NB_OrtTable.Seek("=", DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr]);
                            if (!DataModul.NB_OrtTable.NoMatch)
                            {
                                goto end_IL_0000_2;
                            }
                            if ((array[12].Trim().Length > 1) | (array[11].Trim().Length > 1))
                            {
                                WriteGedLine("3 " + cGed_MAP + " ", true);
                            }
                            if (array[12].Trim().Length > 1)
                            {
                                array[12] = Strings.Replace(array[12], ";", ",");
                                array[12] = Strings.Replace(array[12], ".", ",");
                                UbgT = array[12];
                                if (UbgT.Left(1) == "-")
                                {
                                    UbgT1 = "S";
                                    UbgT = Strings.Mid(UbgT, 2, UbgT.Length);
                                }
                                else
                                {

                                    UbgT1 = "N";
                                }
                                goto IL_0a37;
                            }
                            goto IL_0b89;
                        IL_0a37: // <========== 3
                                 // <========== 3
                            num = 111;
                            if (UbgT.Left(1) == "+")
                            {
                                UbgT = Strings.Mid(UbgT, 2, UbgT.Length);
                            }
                            short num5;
                            if (Strings.InStr(UbgT, ",") != 0)
                            {
                                num5 = (short)Strings.InStr(UbgT, ",");
                                UbgT1 = UbgT1 + Strings.Trim("    " + UbgT.Left(num5 - 1).Right(4)) + ".";
                                UbgT = Strings.Mid(UbgT, num5 + 1, UbgT.Length);
                                Modul1.DezRechnen(ref A);
                                UbgT1 += A.Trim();
                                if (UbgT1.Trim().Length > 1)
                                {
                                    WriteGedLine("4 " + cGed_LATI + " " + UbgT1.Trim(), false);
                                    UbgT1 = "";
                                }
                            }
                            goto IL_0b89;
                        IL_0b89: // <========== 3
                                 // <========== 4
                            num = 127;
                            if (array[11].Trim().Length > 1)
                            {
                                array[11] = Strings.Replace(array[11], ";", ",");
                                array[11] = Strings.Replace(array[11], ".", ",");
                                UbgT = array[11];
                                if (UbgT.Left(1) == "-")
                                {
                                    UbgT1 = "W";
                                    UbgT = Strings.Mid(UbgT, 2, UbgT.Length);
                                }
                                else
                                {

                                    UbgT1 = "E";
                                }
                                goto IL_0c5f;
                            }
                            goto IL_0dd5;
                        IL_0c5f: // <========== 3
                                 // <========== 3
                            num = 138;
                            if (UbgT.Left(1) == "+")
                            {
                                UbgT = Strings.Mid(UbgT, 2, UbgT.Length);
                            }
                            if (Strings.InStr(UbgT, ",") != 0)
                            {
                                num5 = (short)Strings.InStr(UbgT, ",");
                                UbgT1 = UbgT1 + Strings.Trim("    " + UbgT.Left(num5 - 1).Right(4)) + ".";
                                UbgT = Strings.Mid(UbgT, num5 + 1, UbgT.Length);
                                Modul1.DezRechnen(ref A);
                                UbgT1 += A.Trim();
                                if (UbgT1.Trim().Length > 1)
                                {
                                    WriteGedLine("4 " + cGed_LONG + " " + UbgT1.Trim(), false);
                                    UbgT1 = "";
                                }
                            }
                            goto IL_0dd5;
                        IL_0dd5: // <========== 3
                                 // <========== 4
                            num = 154;
                            if (Conversions.ToBoolean((DataModul.DB_PlaceTable.Fields[PlaceFields.PLZ].AsString() != "0")
                                & DataModul.DB_PlaceTable.Fields[PlaceFields.PLZ].AsString().Trim() != ""))
                            {
                                WriteGedLine("3 " + cGed__POST + " " + DataModul.DB_PlaceTable.Fields[PlaceFields.PLZ].AsString().Trim(), false);
                            }
                            if (!Information.IsDBNull(DataModul.DB_PlaceTable.Fields[PlaceFields.Loc].Value))
                            {
                                if (DataModul.DB_PlaceTable.Fields[PlaceFields.Loc].AsString().Trim() != "")
                                {
                                    WriteGedLine("3 _MAIDENHEAD " + DataModul.DB_PlaceTable.Fields[PlaceFields.Loc].AsString().Trim(), false);
                                }
                            }
                            if (!Information.IsDBNull(DataModul.DB_PlaceTable.Fields[PlaceFields.GOV].Value))
                            {
                                if (DataModul.DB_PlaceTable.Fields[PlaceFields.GOV].AsString().Trim() != "")
                                {
                                    WriteGedLine("3 " + cGed__GOV + " " + DataModul.DB_PlaceTable.Fields[PlaceFields.GOV].AsString().Trim(), false);
                                }
                            }
                            if (!Information.IsDBNull(DataModul.DB_PlaceTable.Fields[PlaceFields.Terr].Value))
                            {
                                if (DataModul.DB_PlaceTable.Fields[PlaceFields.Terr].AsString().Trim() != "")
                                {
                                    WriteGedLine("3 " + cGed__FSTAE + " " + DataModul.DB_PlaceTable.Fields[PlaceFields.Terr].AsString().Trim(), false);
                                }
                            }
                            if (!Information.IsDBNull(DataModul.DB_PlaceTable.Fields[PlaceFields.Staatk].Value))
                            {
                                if (DataModul.DB_PlaceTable.Fields[PlaceFields.Staatk].AsString().Trim() != "")
                                {
                                    WriteGedLine("3 " + cGed__FCTRY + " " + DataModul.DB_PlaceTable.Fields[PlaceFields.Staatk].AsString().Trim(), false);
                                }
                            }
                            if (!Information.IsDBNull(DataModul.DB_PlaceTable.Fields[PlaceFields.PolName].Value))
                            {
                                if (DataModul.DB_PlaceTable.Fields[PlaceFields.PolName].AsDouble() > 0.0)
                                {
                                    NamenSuch_Wort = DataModul.TextLese1(DataModul.DB_PlaceTable.Fields[PlaceFields.PolName].AsInt());
                                    WriteGedLine("3 " + cGed__AON + " " + NamenSuch_Wort.Trim(), false);
                                    NamenSuch_Wort = "";
                                }
                            }
                            if ((DataModul.DB_PlaceTable.Fields[PlaceFields.Bem].AsString().Trim() != "") & (DataModul.DB_PlaceTable.Fields[PlaceFields.Bem].AsString().Trim() != "RichText"))
                            {
                                if (Modul1.GedAus.Hi[7])
                                {
                                    UbgT = "3 " + cGed_NOTE + " " + DataModul.DB_PlaceTable.Fields[PlaceFields.Bem].AsString().Trim();
                                    Historie(_Modul1.Instance.UbgT);
                                }
                                if (Options_Bildja == 6)
                                {
                                    int perfamNr = (int)Math.Round(Conversion.Val(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt()));
                                    Bilderaus("O", perfamNr);
                                }
                            }
                            goto IL_1451;
                        IL_1451:
                            // <========== 3
                            num = 200;
                            if (Options_Orts)
                            {
                                goto end_IL_0000_2;
                            }
                            DataModul.NB_OrtTable.AddNew();
                            DataModul.NB_OrtTable.Fields["OrtNr"].Value = DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt();
                            DataModul.NB_OrtTable.Update();
                            if (Information.IsDBNull(DataModul.DB_PlaceTable.Fields[PlaceFields.Zusatz].Value))
                            {
                                goto end_IL_0000_2;
                            }
                            if (DataModul.DB_PlaceTable.Fields[PlaceFields.Zusatz].AsString().Trim() == "")
                            {
                            }
                            else
                            {

                                WriteGedLine("3 " + cGed_TEXT + " " + DataModul.DB_PlaceTable.Fields[PlaceFields.Zusatz].AsString().Trim(), false);
                            }
                            goto end_IL_0000_2;
                        IL_172b:
                            num4 = unchecked(num2 + 1);
                            goto IL_172f;
                        IL_172f:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 55:
                                case 56:
                                case 70:
                                case 90:
                                case 91:
                                case 92:
                                    goto IL_0861;
                                case 107:
                                case 110:
                                case 111:
                                    goto IL_0a37;
                                case 124:
                                case 125:
                                case 126:
                                case 127:
                                    goto IL_0b89;
                                case 134:
                                case 137:
                                case 138:
                                    goto IL_0c5f;
                                case 151:
                                case 152:
                                case 153:
                                case 154:
                                    goto IL_0dd5;
                                case 198:
                                case 199:
                                case 200:
                                    goto IL_1451;
                                case 68:
                                case 69:
                                case 77:
                                case 207:
                                case 208:
                                case 209:
                                case 210:
                                case 211:
                                case 225:
                                case 226:
                                case 227:
                                case 230:
                                case 231:
                                case 238:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj3) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj3, lErl);
                try0000_dispatch = 6901;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 10
                       // <========== 12
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    public void Namensteil(int PersInArb)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string Fld = default;
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
                        case 5811:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                    case 3:
                                        break;
                                    case 1:
                                        goto IL_1269;
                                    default:
                                        goto end_IL_0000;
                                }
                                var Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
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
                                goto IL_126d;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            //
                            Modul1.Persatzles(PersInArb);
                            var iKreis = 1;
                            while (iKreis <= 9)
                            {
                                Modul1.Kont[iKreis] = "";
                                iKreis++; //uses the local variable
                            }

                            Modul1.Perles(Modul1.PersInArb);
                            if (Strings.InStr(Kont[0], "/") != 0)
                            {
                                byte b = 1;
                                byte b2;
                                while (b <= 100u
                                    && (b2 = (byte)Strings.InStr(Kont[0], "/")) != 0)
                                {
                                    Kont[0] = Strings.Mid(Kont[0], 1, (int)Math.Round(unchecked(b2) - 1.0)) + "\\" + Strings.Mid(Kont[0], (int)Math.Round(unchecked(b2) + 1.0), Kont[0].Length);
                                    b++;
                                }
                            }
                            if (Kont[7].Trim() != "")
                            {
                                Kont[7] = Kont[7].Trim() + " ";
                            }
                            Fld = "1 " + cGed_NAME + " " + Kont[7] + Kont[3].TrimEnd() + " /" + (Kont[1].TrimEnd() + " " + Kont[0].TrimEnd()).Trim() + "/" + Kont[2];
                            Fld = Fld.Replace("  ", " ").TrimEnd();
                            if (GedAus.Options_Uml > 0)
                            {
                                Fld = Modul1.Strngs_Umlaute4(Fld, uml: GedAus.Options_Uml);
                            }
                            WriteGedLine(Fld, true);
                            Fld = "";
                            if (Kont[3].Trim() != "")
                            {
                                WriteGedLine("2 " + cGed_GIVN + " " + Kont[3].Trim(), true);
                            }
                            if (Kont[8].Trim() != "")
                            {
                                WriteGedLine("2 _RUFNAME " + Kont[8], true);
                            }
                            if (Kont[0].Trim() != "")
                            {
                                WriteGedLine("2 " + cGed_SURN + " " + Kont[0], true);
                            }
                            if (Kont[4].Length > 0)
                            {
                                Fld = "2 " + cGed__AKA + " " + Kont[4].TrimEnd();
                                if (GedAus.Options_Uml > 0)
                                {
                                    Fld = Modul1.Strngs_Umlaute4(Fld, uml: GedAus.Options_Uml);
                                }
                                WriteGedLine(Fld, true);
                                Fld = "";
                            }
                            if (Kont[5].Length > 0)
                            {
                                Fld = "2 " + cGed_NAMS + " " + Kont[5].TrimEnd();
                                if (GedAus.Options_Uml > 0)
                                {
                                    Fld = Modul1.Strngs_Umlaute4(Fld, uml: GedAus.Options_Uml);
                                }
                                WriteGedLine(Fld, true);
                                Fld = "";
                            }
                            Fld = "";
                            if (Kont[1].Length > 0)
                            {
                                Fld = "2 " + cGed_SPFX + " " + Kont[1].TrimEnd();
                                if (GedAus.Options_Uml > 0)
                                {
                                    Fld = Modul1.Strngs_Umlaute4(Fld, uml: GedAus.Options_Uml);
                                }
                                WriteGedLine(Fld, true);
                                Fld = "";
                            }
                            Fld = "";
                            if (Kont[2].Length > 0)
                            {
                                Fld = "2 " + cGed_NSFX + " " + Kont[2].TrimEnd();
                                if (GedAus.Options_Uml > 0)
                                {
                                    Fld = Modul1.Strngs_Umlaute4(Fld, uml: GedAus.Options_Uml);
                                }
                                WriteGedLine(Fld, true);
                                Fld = "";
                            }
                            Fld = "";
                            if (Kont[7].Length > 0)
                            {
                                Fld = "2 " + cGed_NPFX + " " + Kont[7].TrimEnd();
                                if (GedAus.Options_Uml > 0)
                                {
                                    Fld = Modul1.Strngs_Umlaute4(Fld, uml: GedAus.Options_Uml);
                                }
                                WriteGedLine(Fld, true);
                                Fld = "";
                            }
                            Fld = "";
                            if (Kont[6].Length > 0)
                            {
                                Fld = "1 " + cGed_STAT + " " + Kont[6].TrimEnd();
                                if (GedAus.Options_Uml > 0)
                                {
                                    Fld = Modul1.Strngs_Umlaute4(Fld, uml: GedAus.Options_Uml);
                                }
                                WriteGedLine(Fld, true);
                                Fld = "";
                            }
                            Fld = "";
                            IRecordset dB_PersonTable = DataModul.DB_PersonTable;
                            string Person_sSex = dB_PersonTable.Fields[PersonFields.Sex].AsString();
                            string Person_sReligion = DataModul.TextLese1(dB_PersonTable.Fields[PersonFields.religi].AsInt());

                            WriteGedLine("1 " + cGed_SEX + " " + Person_sSex, true);
                            if (Person_sReligion.Trim() != "")
                            {
                                WriteGedLine($"1 {cGed_RELI} {Person_sReligion}", true);
                            }
                            if (Modul1.GedAus.Options_OFB)
                            {
                                ProjectData.ClearProjectError();
                                num3 = 3;
                                bool flag = false;
                                if (!Information.IsDBNull(dB_PersonTable.Fields[PersonFields.OFB].Value))
                                {
                                    if (dB_PersonTable.Fields[PersonFields.OFB].AsString().Trim() == "J")
                                    {
                                        if (!flag)
                                        {
                                            WriteGedLine("1 " + cGed__OFB, true);
                                            flag = true;
                                        }
                                        WriteGedLine("2 " + cGed_SPER + " J", true);
                                    }
                                }
                                IRecordset dB_NameTable = DataModul.DB_NameTable;
                                dB_NameTable.Index = "NamKenn";
                                dB_NameTable.Seek("=", _Modul1.Instance.PersInArb, "Y");
                                if (!dB_NameTable.NoMatch)
                                {
                                    NamenSuch_Wort = DataModul.TextLese1(dB_NameTable.Fields[NameFields.Text].AsInt());
                                    if (NamenSuch_Wort != "")
                                    {
                                        if (!flag)
                                        {
                                            WriteGedLine("1 " + cGed__OFB, true);
                                            flag = true;
                                        }
                                        WriteGedLine("2 " + cGed_SORT + " " + NamenSuch_Wort, true);
                                        NamenSuch_Wort = "";
                                    }
                                }
                                IRecordset dB_OFBTable = DataModul.DB_OFBTable;
                                dB_OFBTable.Index = "InDNr";
                                dB_OFBTable.Seek("=", _Modul1.Instance.PersInArb, "NN");
                                while (!dB_OFBTable.EOF
                                  && !dB_OFBTable.NoMatch
                                  && !(dB_OFBTable.Fields[OFBFields.Kennz].AsString() != "NN")
                                  && !(dB_OFBTable.Fields[OFBFields.PerNr].AsInt() > _Modul1.Instance.PersInArb))
                                {
                                    NamenSuch_Wort = DataModul.TextLese1(dB_OFBTable.Fields[OFBFields.TextNr].AsInt());
                                    if (NamenSuch_Wort != "")
                                    {
                                        if (!flag)
                                        {
                                            WriteGedLine("1 " + cGed__OFB, true);
                                            flag = true;
                                        }
                                        WriteGedLine("2 " + cGed_NAMO + " " + NamenSuch_Wort, true);
                                        NamenSuch_Wort = "";
                                    }
                                    dB_OFBTable.MoveNext();
                                }
                                dB_OFBTable.Index = "InDNr";
                                dB_OFBTable.Seek("=", _Modul1.Instance.PersInArb, "EE");
                                while (!dB_OFBTable.EOF
                                      && !dB_OFBTable.NoMatch
                                      && !(dB_OFBTable.Fields[OFBFields.Kennz].AsString() != "EE")
                                      && !(dB_OFBTable.Fields[OFBFields.PerNr].AsInt() > _Modul1.Instance.PersInArb) && !(dB_OFBTable.Fields[OFBFields.PerNr].AsInt() > _Modul1.Instance.PersInArb))
                                {
                                    NamenSuch_Wort = DataModul.TextLese1(dB_OFBTable.Fields[OFBFields.TextNr].AsInt());
                                    if (NamenSuch_Wort != "")
                                    {
                                        if (!flag)
                                        {
                                            WriteGedLine("1 " + cGed__OFB, true);
                                            flag = true;
                                        }
                                        WriteGedLine("2 " + cGed_BERO + " ", true);
                                        NamenSuch_Wort = "";
                                    }
                                    dB_OFBTable.MoveNext();
                                }
                                dB_OFBTable.Index = "InDNr";
                                dB_OFBTable.Seek("=", _Modul1.Instance.PersInArb, "OO");
                                while (!dB_OFBTable.EOF
                                      && !dB_OFBTable.NoMatch
                                      && !(dB_OFBTable.Fields[OFBFields.Kennz].AsString() != "OO")
                                      && !(dB_OFBTable.Fields[OFBFields.PerNr].AsInt() > _Modul1.Instance.PersInArb))
                                {
                                    if (!flag)
                                    {
                                        WriteGedLine(Fld, true);
                                        flag = true;
                                    }
                                    Modul1.GedAus.Options_Schalt1 = 7;
                                    Ortnr = (int)Math.Round(dB_OFBTable.Fields[OFBFields.TextNr].AsDouble());
                                    UbgT = ortles(Ortnr);
                                    Modul1.GedAus.Options_Schalt1 = 0;
                                    dB_OFBTable.MoveNext();
                                }
                            }
                            NamenSuch_Wort = "";
                            if (!Information.IsDBNull(dB_PersonTable.Fields[PersonFields.PUid].Value))
                            {
                                NamenSuch_Wort = dB_PersonTable.Fields[PersonFields.PUid].AsString();
                            }
                            if (NamenSuch_Wort.Trim() != "")
                                WriteGedLine("1 " + cGed__UID + " " + NamenSuch_Wort, true);
                            goto end_IL_0000_2;
                        IL_1269:
                            num4 = unchecked(num2 + 1);
                            goto IL_126d;
                        IL_126d:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 263:
                                case 264:
                                case 270:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 5811;
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

    public void Repoausgeb()
    {
        string text = "";
        if (DataModul.NB_LagTable.RecordCount <= 0)
        {
            return;
        }
        DataModul.NB_LagTable.Index = "Nr";
        DataModul.NB_LagTable.MoveFirst();
        IRecordset dB_RepoTable = DataModul.DB_RepoTable;
        while (!DataModul.NB_LagTable.EOF && !DataModul.NB_LagTable.NoMatch)
        {
            dB_RepoTable.Index = "Nr";
            dB_RepoTable.Seek("=", DataModul.NB_LagTable.Fields["Satznr"].AsDouble());
            if (dB_RepoTable.NoMatch
                || dB_RepoTable.Fields[RepoFields.Nr].AsInt() != DataModul.NB_LagTable.Fields["Satznr"].AsDouble())
            {
                break;
            }

            text = "0 @R" + dB_RepoTable.Fields[RepoFields.Nr].AsString().Trim() + "@ REPO";
            WriteGedLine(text, true);
            if (dB_RepoTable.Fields[RepoFields.Name].AsString().Trim() != "")
            {
                text = "1 " + cGed_NAME + " " + Strings.Trim(dB_RepoTable.Fields[RepoFields.Name].AsString().Trim());
                text = text.Replace("@", "@@");
                WriteGedLine(text, true);
            }
            if (dB_RepoTable.Fields[RepoFields.Strasse].AsString().Trim() != "")
            {
                text = "1 " + cGed_ADDR + " " + Strings.Trim(dB_RepoTable.Fields[RepoFields.Strasse].AsString().Trim());
                text = text.Replace("@", "@@");
                WriteGedLine(text, true);
            }
            if (dB_RepoTable.Fields[RepoFields.Ort].AsString().Trim() != "")
            {
                text = "2 " + cGed_CITY + " " + Strings.Trim(dB_RepoTable.Fields[RepoFields.Ort].AsString().Trim());
                text = text.Replace("@", "@@");
                WriteGedLine(text, true);
            }
            if (dB_RepoTable.Fields[RepoFields.PLZ].AsString().Trim() != "")
            {
                text = "2 " + cGed_POST + " " + Strings.Trim(dB_RepoTable.Fields[RepoFields.PLZ].AsString().Trim());
                WriteGedLine(text, true);
            }
            if (dB_RepoTable.Fields[RepoFields.Fon].AsString().Trim() != "")
            {
                text = "1 " + cGed_PHON + " " + Strings.Trim(dB_RepoTable.Fields[RepoFields.Fon].AsString().Trim());
                text = text.Replace("@", "@@");
                WriteGedLine(text, true);
            }
            if (dB_RepoTable.Fields[RepoFields.Mail].AsString().Trim() != "")
            {
                text = "1 " + cGed_EMAIL + " " + Strings.Trim(dB_RepoTable.Fields[RepoFields.Mail].AsString().Trim());
                text = text.Replace("@", "@@");
                WriteGedLine(text, true);
            }
            if (dB_RepoTable.Fields[RepoFields.Http].AsString().Trim() != "")
            {
                text = "1 " + cGed_WWW + " " + Strings.Trim(dB_RepoTable.Fields[RepoFields.Http].AsString().Trim());
                text = text.Replace("@", "@@");
                WriteGedLine(text, true);
            }
            if (dB_RepoTable.Fields[RepoFields.Bem].AsString().Trim() != "")
            {
                UbgT = "2 " + cGed_NOTE + " " + dB_RepoTable.Fields[RepoFields.Bem].AsString().Trim();
                Historie(UbgT);
            }
            DataModul.NB_LagTable.MoveNext();
        }
    }

    public void Textaus()
    {
        IRecordset dB_TexteTable = DataModul.DB_TexteTable;

        if (dB_TexteTable.RecordCount <= 0)
        {
            return;
        }
        dB_TexteTable.Index = TexteIndex.ALTexte.AsFld();
        dB_TexteTable.MoveFirst();
        while (!dB_TexteTable.EOF)
        {
            string text = "";
            dB_TexteTable.Seek("=", dB_TexteTable.Fields[TexteFields.TxNr].AsInt());
            if (!dB_TexteTable.NoMatch)
            {
                string Texte_sLeitname = dB_TexteTable.Fields[TexteFields.Leitname].AsString();
                string Texte_sBem = dB_TexteTable.Fields[TexteFields.Bem].AsString();
                string Texte_sTxt = dB_TexteTable.Fields[TexteFields.Txt].AsString();
                string Texte_sKennz = dB_TexteTable.Fields[TexteFields.Kennz].AsString();

                if (Texte_sLeitname != "")
                {
                    text += UnPackTrim(Texte_sLeitname);
                }
                if (Texte_sBem != "")
                {
                    text += Texte_sBem.Trim();
                }
                if (text != "")
                {
                    WriteGedLine($"0 {cGed__TXT}", false);
                    WriteGedLine($"1 {cGed_TEXT} {UnPackTrim(Texte_sTxt)}", true);
                    WriteGedLine($"1 {cGed_KENN} {Texte_sKennz.Trim()}", true);
                    if (Texte_sLeitname.Trim() != "")
                    {
                        WriteGedLine($"1 {cGed_LEIT} {UnPackTrim(Texte_sLeitname)}", true);
                    }
                    if (Texte_sBem.Trim() != "")
                    {
                        TextTeilen(UnPackTrim(Texte_sBem), $"1 {cGed_NOTE} ");
                    }
                }
            }
            dB_TexteTable.MoveNext();
        }

        static string UnPackTrim(string v)
        {
            return Strings.Trim(v.Replace("ssss", "ß"));
        }
    }

    public void Bilderaus(string Kenn, int PerfamNr)
    {
        IRecordset dB_PictureTable = DataModul.DB_PictureTable;
        IRecordset nB_PictureTable = DataModul.NB_PictureTable;

        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string text = default;

        int num4;
        Type typeFromHandle;
        object[] array;
        IField field;
        object[] array2;
        bool[] array3;
        object obj;
        string str;
        Type typeFromHandle2;
        object[] array4;
        IField field2;
        object[] array5;
        bool[] array6;
        object obj2;
        switch (try0000_dispatch)
        {
            default:
                ProjectData.ClearProjectError();
                num3 = 2;

                num = 2;
                if (Kenn == "P")
                {
                    Modul1.Kennz = "P";
                }

                dB_PictureTable.Index = "Perkenn";
                dB_PictureTable.Seek("=", Modul1.Kennz, Modul1.PersInArb);
                if (!dB_PictureTable.NoMatch)
                {
                    goto IL_0866;
                }
                goto IL_0879;
            IL_05ea: // <========== 3
                num = 57;
                text = "3 " + cGed_FORM + " " + Strings.Right(dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim(), 3);
                GedAus.WriteGedLine(text, false);
                text = "2 _Prim Y ";
                GedAus.WriteGedLine(text, false);
                text = "2 " + cGed_TITL + " " + dB_PictureTable.Fields[PictureFields.Beschreibung].AsString().Trim();
                GedAus.WriteGedLine(text, true);

                nB_PictureTable.Index = "Ind";
                nB_PictureTable.Seek("=", text);
                if (nB_PictureTable.NoMatch)
                {
                    nB_PictureTable.AddNew();
                    nB_PictureTable.Fields["Bildname"].Value = text;
                    text = "";
                    nB_PictureTable.Update();
                }
                goto IL_07bb;
            IL_07bb:
                num = 80;
                if (!Information.IsDBNull(dB_PictureTable.Fields[PictureFields.Bem].Value))
                {
                    if (dB_PictureTable.Fields[PictureFields.Bem].AsString().Trim() != "")
                    {
                        UbgT = "2 " + cGed_NOTE + " " + dB_PictureTable.Fields[PictureFields.Bem].AsString().Trim();
                        GedAus.Historie(_Modul1.Instance.UbgT);

                    }
                }
                goto IL_0858;
            IL_0858: // <========== 4
                num = 87;
                dB_PictureTable.MoveNext();
                goto IL_0866;
            IL_0866: // <========== 3
                num = 9;
                string text2;
                if (!dB_PictureTable.EOF)
                {
                    int Picture_iPerFamNr = dB_PictureTable.Fields[PictureFields.ZuNr].AsInt();
                    string Picture_sBeschr = dB_PictureTable.Fields[PictureFields.Beschreibung].AsString();
                    string Picture_sPfad = dB_PictureTable.Fields[PictureFields.Pfad].AsString();

                    if (!(Picture_iPerFamNr != PerfamNr))
                    {
                        if (Picture_sBeschr == "Personenbild")
                        {
                            text = "1 " + cGed_OBJE;
                            GedAus.WriteGedLine(text, true);
                            text2 = "";


                            if (Strings.InStr(Picture_sPfad.ToUpper(), "#BILDER\\") != 0)
                            {
                                str = Picture_sPfad;

                                text2 = Strings.Mid(str, Strings.InStr(Picture_sPfad.ToUpper(), "#BILDER\\") + 7, Picture_sPfad.Length);
                                text2 = "\\Bilder" + text2;
                            }
                            text2 = "";
                            if (Strings.InStr(Picture_sPfad, "#") != 0)
                            {
                                text2 = Strings.Replace((Modul1.Verz + Picture_sPfad), "#", "");
                                text = text2 + dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim();
                                nB_PictureTable.Index = "Ind";
                                nB_PictureTable.Seek("=", text);
                                if (nB_PictureTable.NoMatch)
                                {
                                    nB_PictureTable.AddNew();
                                    nB_PictureTable.Fields["Bildname"].Value = text;
                                    nB_PictureTable.Update();
                                }
                                text = "2 " + cGed_FILE + " " + text2 + dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim();
                                GedAus.WriteGedLine(text, true);
                            }
                            else
                            {
                                text2 = Picture_sPfad;
                                text = text2 + dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim();
                                nB_PictureTable.Index = "Ind";
                                nB_PictureTable.Seek("=", text);
                                if (nB_PictureTable.NoMatch)
                                {
                                    nB_PictureTable.AddNew();
                                    nB_PictureTable.Fields["Bildname"].Value = text;
                                    nB_PictureTable.Update();
                                }
                                text = "2 " + cGed_FILE + " " + text2 + dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim();
                                GedAus.WriteGedLine(text, true);
                            }
                            goto IL_05ea;
                        }
                        goto IL_0858;
                    }
                }
                goto IL_0879;
            IL_0879: // <========== 3
                num = 90;
                dB_PictureTable.Index = "Perkenn";
                dB_PictureTable.Seek("=", Kenn, PerfamNr);
                goto IL_13a2;
            IL_09aa: // <========== 3
                num = 108;
                GedAus.WriteGedLine(text, true);
                text2 = "";
                text2 = Strings.Replace((Modul1.Verz + dB_PictureTable.Fields[PictureFields.Pfad].AsString()), "#", "");
                text = "2 " + cGed_FILE + " " + text2 + dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim();
                if (Kenn == "O")
                {
                    if (Strings.InStr(dB_PictureTable.Fields[PictureFields.Pfad].AsString(), "#") != 0)
                    {
                        text2 = Strings.Replace((Modul1.Verz + dB_PictureTable.Fields[PictureFields.Pfad].AsString()), "#", "");
                        text = text2 + dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim();
                        text = "4 " + cGed_FILE + " " + text2 + dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim();
                        GedAus.WriteGedLine(text, false);
                    }
                    else
                    {
                        text2 = dB_PictureTable.Fields[PictureFields.Pfad].AsString();
                        text = text2 + dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim();
                        text = "4 " + cGed_FILE + " " + text2 + dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim();
                        GedAus.WriteGedLine(text, false);
                        text = "";
                    }
                    goto IL_0c51;
                }
                if (Strings.InStr(dB_PictureTable.Fields[PictureFields.Pfad].AsString(), "#") != 0)
                {
                    text2 = Strings.Replace((Modul1.Verz + dB_PictureTable.Fields[PictureFields.Pfad].AsString()), "#", "");
                    text = text2 + dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim();
                    text = "2 " + cGed_FILE + " " + text2 + dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim();
                    GedAus.WriteGedLine(text, false);
                }
                else
                {
                    text2 = dB_PictureTable.Fields[PictureFields.Pfad].AsString();
                    text = text2 + dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim();
                    text = "2 " + cGed_FILE + " " + text2 + dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim();
                    GedAus.WriteGedLine(text, false);
                }
                goto IL_0eb7;
            IL_0c51: // <========== 3
                num = 134;
                text = "5 " + cGed_FORM + " " + Strings.Right(dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim(), 3);
                GedAus.WriteGedLine(text, true);
                goto IL_0f29;
            IL_0eb7: // <========== 3
                num = 156;
                text = "3 " + cGed_FORM + " " + Strings.Right(dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim(), 3);
                GedAus.WriteGedLine(text, true);
                goto IL_0f29;
            IL_0f29: // <========== 3
                num = 161;
                if (Kenn == "O")
                {
                    text = "4 " + cGed_TITL + " " + dB_PictureTable.Fields[PictureFields.Beschreibung].AsString().Trim();
                }
                else
                {

                    text = "2 " + cGed_TITL + " " + dB_PictureTable.Fields[PictureFields.Beschreibung].AsString().Trim();
                }
                goto IL_0fb2;
            IL_0fb2: // <========== 3
                num = 167;
                GedAus.WriteGedLine(text, true);
                text2 = "";
                if (Strings.InStr(dB_PictureTable.Fields[PictureFields.Pfad].AsString(), "#") != 0)
                {
                    text2 = Strings.Replace((Verz + dB_PictureTable.Fields[PictureFields.Pfad].AsString()), "#", "");
                    text = text2 + dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim();
                    text = "2 " + cGed_FILE + " " + text2 + dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim();
                }
                else
                {

                    text2 = dB_PictureTable.Fields[PictureFields.Pfad].AsString();
                    text = text2 + dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim();
                    text = "2 " + cGed_FILE + " " + text2 + dB_PictureTable.Fields[PictureFields.Datei].AsString().Trim();
                }
                goto IL_1190;
            IL_1190: // <========== 3
                num = 184;
                nB_PictureTable.Index = "Ind";
                nB_PictureTable.Seek("=", text.Left(239));
                if (nB_PictureTable.NoMatch)
                {
                    nB_PictureTable.AddNew();
                    nB_PictureTable.Fields["Bildname"].Value = Strings.Mid(text, 8, 239);
                    text = "";
                    nB_PictureTable.Update();
                }
                goto IL_1274;
            IL_1274:
                num = 192;
                if (!Information.IsDBNull(dB_PictureTable.Fields[PictureFields.Bem].Value))
                {
                    if (dB_PictureTable.Fields[PictureFields.Bem].AsString().Trim() != "")
                    {
                        if (Kenn == "O")
                        {
                            UbgT = "4 " + cGed_NOTE + " " + dB_PictureTable.Fields[PictureFields.Bem].AsString().Trim();
                        }
                        else
                        {

                            UbgT = "2 " + cGed_NOTE + " " + dB_PictureTable.Fields[PictureFields.Bem].AsString().Trim();
                        }
                        goto IL_136f;
                    }
                }
                goto IL_1391;
            IL_136f: // <========== 3
                num = 200;
                GedAus.Historie(_Modul1.Instance.UbgT);
                goto IL_1391;
            IL_1391: // <========== 4
                num = 211;
                dB_PictureTable.MoveNext();
                goto IL_13a2;
            IL_13a2: // <========== 3
                num = 93;
                if (dB_PictureTable.EOF)
                {
                    goto end_IL_0000_2;
                }
                if (!dB_PictureTable.NoMatch)
                {
                    if (dB_PictureTable.Fields[PictureFields.ZuNr].AsInt() != PerfamNr)
                    {
                        goto end_IL_0000_2;
                    }
                    if (!(dB_PictureTable.Fields[PictureFields.Beschreibung].AsString() == "Personenbild"))
                    {
                        if (dB_PictureTable.Fields[PictureFields.Kennz].AsString() == Kenn)
                        {
                            if (Kenn == "O")
                            {
                                text = "3 " + cGed_OBJE;
                            }
                            else
                            {

                                text = "1 " + cGed_OBJE;
                            }
                            goto IL_09aa;
                        }
                    }
                    else goto IL_1391;
                }
                goto end_IL_0000_2;
            IL_14a2:
                num4 = unchecked(num2 + 1);
                goto IL_14a6;
            IL_14a6:
                num2 = 0;
                switch (num4)
                {
                    case 1:
                        break;
                    case 41:
                    case 56:
                    case 57:
                        goto IL_05ea;
                    case 79:
                    case 80:
                        goto IL_07bb;
                    case 84:
                    case 85:
                    case 86:
                    case 87:
                        goto IL_0858;
                    case 8:
                    case 9:
                    case 88:
                        goto IL_0866;
                    case 11:
                    case 89:
                    case 90:
                        goto IL_0879;
                    case 104:
                    case 107:
                    case 108:
                        goto IL_09aa;
                    case 125:
                    case 133:
                    case 134:
                        goto IL_0c51;
                    case 147:
                    case 155:
                    case 156:
                        goto IL_0eb7;
                    case 138:
                    case 160:
                    case 161:
                        goto IL_0f29;
                    case 163:
                    case 166:
                    case 167:
                        goto IL_0fb2;
                    case 178:
                    case 183:
                    case 184:
                        goto IL_1190;
                    case 191:
                    case 192:
                        goto IL_1274;
                    case 196:
                    case 199:
                    case 200:
                        goto IL_136f;
                    case 99:
                    case 201:
                    case 202:
                    case 203:
                    case 206:
                    case 207:
                    case 210:
                    case 211:
                        goto IL_1391;
                    case 92:
                    case 93:
                    case 212:
                        goto IL_13a2;
                    case 96:
                    case 205:
                    case 209:
                    case 213:
                    case 223:
                        goto end_IL_0000_2;
                }
                goto default;
        }
    end_IL_0000_2: // <========== 5
        return;
    }

    public void Historie(string UbgT)
    {
        short A = default;
        ProjectData.ClearProjectError();
        UbgT = UbgT.Trim();
        UbgT = UbgT.Replace("@", "@@");
        UbgT = UbgT.Replace("\0", "");
        if (Strings.InStr(UbgT, "\u001a") != 0)
        {
            A = (short)Strings.InStr(UbgT, "\u001a");
            if (A == UbgT.Length)
            {
                UbgT = UbgT.Left(A - 1);
            }
        }
        if (UbgT != "" && Histor != 0)
            TextTeilen(Strings.Trim(Strings.Mid(UbgT, 8, UbgT.Length)), UbgT.Left(7));
        return;
    }

    public void Quelleaus(byte Kenn, int PerfamNr)
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
                            goto IL_0007;
                        case 2725:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_08d1;
                                    default:
                                        goto end_IL_0000;
                                }
                                var Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
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
                                goto IL_08d4;
                            }
                        end_IL_0000:
                            break;
                        IL_0007:
                            num = 2;
                            if (DataModul.DB_SourceLinkTable.RecordCount == 0)
                            {
                                goto end_IL_0000_2;
                            }
                            DataModul.DB_SourceLinkTable.MoveFirst();
                            DataModul.DB_SourceLinkTable.Index = "Tab";
                            DataModul.DB_SourceLinkTable.Seek("=", Kenn, PerfamNr);
                            goto IL_081c;
                        IL_0178: // <========== 4
                            num = 24;
                            lErl = 1;
                            DataModul.NB_SourceTable.Index = "OrgNr";
                            DataModul.NB_SourceTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[2]);
                            if (DataModul.NB_SourceTable.NoMatch)
                            {
                                if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[2].Value))
                                {
                                    if (!(DataModul.DB_SourceLinkTable.Fields[2].AsInt() > 0))
                                    {
                                    }
                                    else
                                    {

                                        DataModul.NB_SourceTable.AddNew();
                                        DataModul.NB_SourceTable.Fields["AusNr"].Value = DataModul.NB_SourceTable.RecordCount + 1;
                                        DataModul.NB_SourceTable.Fields["OrgNr"].Value = DataModul.DB_SourceLinkTable.Fields[2].Value;
                                        DataModul.NB_SourceTable.Update();
                                        Quell = 1;
                                    }
                                    goto IL_0178;
                                }
                            }
                            string text = "1 " + cGed_SOUR + " @S" + DataModul.NB_SourceTable.Fields["AusNr"].AsString().Trim() + "@";
                            GedAus.WriteGedLine(text, true);
                            goto IL_0380;
                        IL_0380:
                            num = 46;
                            if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[3].Value)
                                && DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim() != "")
                            {
                                text = "2 " + cGed_PAGE + " " + DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim();
                                text = text.Replace("\n", " ");
                                text = text.Replace("\r", " ");
                                text = text.Replace("  ", " ");
                                GedAus.WriteGedLine(text, true);
                                if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value) && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim() != "")
                                {
                                    if (DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim() != "")
                                    {
                                        text = "3 " + cGed__ZUS + " " + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim();
                                        GedAus.WriteGedLine(text, true);
                                    }
                                    else
                                    if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim() != "Seite:")
                                    {
                                        text = "3 " + cGed__ZUS + " " + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim();
                                        GedAus.WriteGedLine(text, true);
                                    }
                                }
                            }
                            goto IL_0675;
                        IL_0675: // <========== 6
                            num = 84;
                            if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value))
                            {
                                text = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString().Replace("\n", "");
                                if (text.Trim() != "")
                                {
                                    text = "2 " + cGed_DATA;
                                    GedAus.WriteGedLine(text, false);
                                    text = "";
                                    text = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString().Trim();
                                    GedAus.TextTeilen(text, "3 " + cGed_TEXT + " ");
                                    text = "";
                                }
                            }
                            goto IL_0767;
                        IL_0767: // <========== 3
                            num = 96;
                            if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value))
                            {
                                if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString().Trim() != "")
                                {
                                    text = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString().Trim();
                                    GedAus.TextTeilen(text, "2 " + cGed_NOTE + " ");
                                    text = "";
                                }
                            }
                            goto IL_0806;
                        IL_0806: // <========== 5
                            num = 104;
                            lErl = 45;
                            DataModul.DB_SourceLinkTable.MoveNext();
                            goto IL_081c;
                        IL_081c: // <========== 3
                            num = 9;
                            if (DataModul.DB_SourceLinkTable.EOF)
                            {
                                goto end_IL_0000_2;
                            }
                            if (!DataModul.DB_SourceLinkTable.NoMatch)
                            {
                                if (DataModul.DB_SourceLinkTable.Fields[0].AsInt() != Kenn
                                    || (DataModul.DB_SourceLinkTable.Fields[1].AsInt() != PerfamNr))
                                {
                                    goto end_IL_0000_2;
                                }
                                if (!Information.IsDBNull(DataModul.DB_SourceLinkTable.Fields[2].Value))
                                {
                                    if (DataModul.DB_SourceLinkTable.Fields[2].AsInt() == 0)
                                    {
                                        DataModul.DB_SourceLinkTable.Delete();
                                        goto IL_0806;
                                    }
                                    goto IL_0178;
                                }
                            }
                            goto IL_0806;
                        IL_08d1:
                            num4 = unchecked(num2 + 1);
                            goto IL_08d4;
                        IL_08d4:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 23:
                                case 24:
                                case 35:
                                case 36:
                                    goto IL_0178;
                                case 45:
                                case 46:
                                    goto IL_0380;
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
                                    goto IL_0675;
                                case 94:
                                case 95:
                                case 96:
                                    goto IL_0767;
                                case 18:
                                case 22:
                                case 101:
                                case 102:
                                case 103:
                                case 104:
                                    goto IL_0806;
                                case 8:
                                case 9:
                                case 106:
                                    goto IL_081c;
                                case 3:
                                case 12:
                                case 15:
                                case 107:
                                case 113:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 2725;
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


    public void WriteEventData(EEventArt eArt, IEventData cEv)
    {
        int try0000_dispatch = -1;
        switch (try0000_dispatch)
        {
            default:
                int num = 1;
                num = 2;
                string text = "";
                ProjectData.ClearProjectError();
                string datsicha = "";
                if (!Information.IsDBNull(cEv.iDatumText)
                    && (cEv.iDatumText != 0)
                    && cEv.dDatumV != default)
                {
                    text = "INT ";
                }
                string Datu = "";
                if (cEv.sDatumV_S != "")
                {
                    string Dasich = "";
                    if (cEv.sDatumV_S.Trim() != "")
                    {
                        Dasich = cEv.sDatumV_S.Trim();
                    }
                    Modul1.Sichwand(Dasich, DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString().Trim(), DataModul.DB_EventTable.Fields[EventFields.DatumB].AsDate(), eArt);
                    Datu = Dasich;
                    datsicha = Datu;
                }
                if (cEv.iPrivacy > GedAus.Options_priv.AsInt())
                {
                    goto end_IL_0000_2;
                }
                if (cEv.dDatumV != default)
                {
                    var sDat = cEv.dDatumV.AsString().Trim();
                    sDat = sDat.Date2DotDateStr2();
                    Datu = Datu + " " + sDat;
                    sDat = "";
                }
                if (cEv.sVChr != "")
                {
                    Datu += " B.C.";
                }
                if (Conversions.ToBoolean(Operators.AndObject(Operators.CompareString(Strings.UCase(
                    cEv.sDatumB_S.Trim()), "A", TextCompare: false) == 0, cEv.dDatumB != default)))
                {
                    Datu += " AND ";
                    if (cEv.dDatumB != default)
                    {
                        var sDat = cEv.dDatumB.AsString().Trim();
                        sDat = sDat.Date2DotDateStr2();
                        Datu = Datu.TrimEnd() + " " + sDat.Trim();
                        sDat = "";
                    }
                }
                else
                if (Conversions.ToBoolean(Operators.CompareString(cEv.sDatumB_S.Trim(), "", TextCompare: false) != 0 | (cEv.dDatumB != default)))
                {
                    if (cEv.sDatumV_S == "")
                    {
                        if ((Datu.Trim() != "") & (cEv.sDatumV_S.Trim() == ""))
                        {
                            Datu = "BET " + Datu.Trim();
                            datsicha = "BET";
                        }
                    }
                    GedAus.Zweitdatum(ref Datu, datsicha, eArt, cEv);
                }
                int lErl = 5;
                if (!Information.IsDBNull(cEv.iDatumText))
                {
                    if (NamenSuch_Wort.Trim() != "")
                    {
                        Datu = Datu + " (" + cEv.sDatumText.Trim() + ")";
                        NamenSuch_Wort = "";
                    }
                }
                if (Datu == "" && cEv.xIsDead)
                {
                    Datu = "Deceased";
                }
                if (!Information.IsDBNull(cEv.iPrivacy))
                {
                    var value = cEv.iPrivacy;
                    if (value == 1)
                    {
                        GedAus.WriteGedLine("2 " + cGed_RESN + " privacy", false);
                    }
                    else
                    if (value == 2)
                    {
                        GedAus.WriteGedLine("2 " + cGed_RESN + " confidential", false);
                    }
                }
                WriteCondGedTag(2, cGed_DATE, text + Datu);
                if (!Information.IsDBNull(cEv.iOrt)
                    && cEv.iOrt > 0)
                {
                    int Ortnr = cEv.iOrt;
                    UbgT = GedAus.ortles(Ortnr);
                }
                WriteCondGedTag(3, cGed__SIC, cEv.sOrt_S);
                WriteCondGedTag(3, cGed__ZUS, cEv.sZusatz);
                if (GedAus.Kontakt)
                {
                    goto end_IL_0000_2;
                }
                string Fld = default;
                if (eArt == EEventArt.eA_105 && eArt == EEventArt.eA_603) // Changed & to &&
                {
                    WriteCondGedTagUmwandl(3, cGed_TYPE, cEv.sArtText);
                }

                WriteCondGedTagUmwandl(2, cGed_ADDR, cEv.sKBem);
                WriteCondGedTagUmwandl(2, cGed_REFN, cEv.sReg.Replace("\n", " "));
                if (eArt == EEventArt.eA_201
                    || eArt == EEventArt.eA_202
                    || eArt == EEventArt.eA_203
                    || eArt == EEventArt.eA_601)
                {
                    WriteCondGedTagUmwandl(2, cGed_TEMP, cEv.sPlatz);
                }
                else
                {
                    WriteCondGedTagUmwandl(2, cGed__SITE, cEv.sPlatz);
                }
                if (eArt == EEventArt.eA_Death && Datschu[8] == 1)
                {
                    WriteCondGedTagUmwandl(2, cGed_CAUS, cEv.sCausal);
                    WriteCondGedTagUmwandl(3, cGed_CONT, cEv.sAn);
                }

                int num6 = eArt > EEventArt.eA_499 ? Modul1.FamInArb : Modul1.PersInArb;

                WriteEventWitness(eArt, num6);
                Fld = "";
                if (!Information.IsDBNull(cEv.sBem[4]))
                {
                    if (cEv.sBem[4].Trim() != "")
                    {
                        Fld = cEv.sBem[4];
                    }
                    Fld = Fld.Trim();
                    if (Fld != "")
                    {
                        GedAus.TextTeilen(Fld, "2 " + cGed__WITN + " ");
                    }
                }
                Fld = "";
                if (eArt == EEventArt.eA_Baptism)
                {
                    if (Options_Paten == 6)
                    {
                        Fld = "";
                        int persInArb = PersInArb;
                        foreach (var cLink in DataModul.Link.ReadAllFams(PersInArb, ELinkKennz.lkGodparent))
                        {
                            Perles(cLink.iPersNr);
                            Fld = Fld + Kont[3].Trim() + " " + Kont[0].Trim() + "; ";
                        }
                        PersInArb = persInArb;
                        if (Fld != "" && Fld.Right(2) == "; ")
                        {
                            Fld = Fld.Left(Fld.Length - 2);
                        }
                    }
                    if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields[PersonFields.Bem2].Value)
                        && DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString().Length > 0
                        && DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString().Trim() != "")
                    {
                        if (Fld != "")
                        {
                            Fld += ";";
                        }
                        Fld = (Fld + " " + DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString());
                    }
                    Fld = Fld.Trim();
                    if (Fld != "")
                    {
                        GedAus.TextTeilen(Fld, "2 " + cGed__GODP + " ");
                        Pataus = 66;
                    }
                }
                else
                if (eArt == EEventArt.eA_501 || eArt == EEventArt.eA_Marriage || eArt == EEventArt.eA_MarrReligious)
                {
                    ELinkKennz eLKennz = default;
                    if (eArt == EEventArt.eA_501)
                    {
                        eLKennz = ELinkKennz.lkWitnOfEngage;
                    }
                    if (eArt == EEventArt.eA_Marriage)
                    {
                        eLKennz = ELinkKennz.lkMarrWitness;
                    }
                    if (eArt == EEventArt.eA_MarrReligious)
                    {
                        eLKennz = ELinkKennz.lkWitnOfMarr;
                    }
                    if (Options_Paten == 6)
                    {
                        foreach (var cLink in DataModul.Link.ReadAllFams(FamInArb, eLKennz))
                        {
                            Perles(cLink.iPersNr);
                            Fld = Fld + Kont[3].Trim() + " " + Kont[0].Trim() + "; ";
                        }

                        if (Fld != "")
                        {
                            if (Fld.Right(2) == "; ")
                            {
                                Fld = Fld.Left(Fld.Length - 2);
                            }
                        }
                    }
                    if (!Information.IsDBNull(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].Value))
                    {
                        if (DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].AsString().Trim() != "")
                        {
                            if (Fld != "")
                            {
                                Fld += ";";
                            }
                            Fld = Fld + " " + DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].AsString().Trim();
                        }
                    }
                    Fld = Fld.Trim();
                    if (Fld != "")
                    {
                        if ((eArt > EEventArt.eA_Marriage) & (Trauaus == 0))
                        {
                            GedAus.TextTeilen(Fld, "2 " + cGed__WITN + " ");
                        }
                        else
                        if (eArt == EEventArt.eA_Marriage)
                        {
                            Trauaus = 1;
                            GedAus.TextTeilen(Fld, "2 " + cGed__WITN + " ");
                        }
                    }
                }
                if (cEv.sBem[1].Trim() != "")
                {
                    if ((Hi[2]) & (eArt < EEventArt.eA_499))
                    {
                        UbgT = "2 " + cGed__COM + " " + cEv.sBem[1].Trim();
                        GedAus.Historie(_Modul1.Instance.UbgT);
                    }
                    if ((Hi[5]) & (eArt > EEventArt.eA_499))
                    {
                        UbgT = ("2 " + cGed__COM + " " + cEv.sBem[1]).AsString();
                        GedAus.Historie(_Modul1.Instance.UbgT);
                    }
                }
                if (cEv.sBem[2].Trim() != "")
                {
                    if ((Hi[3]) & (eArt < EEventArt.eA_499))
                    {
                        UbgT = "2 " + cGed_NOTE + " " + cEv.sBem[2].Trim();
                        GedAus.Historie(_Modul1.Instance.UbgT);
                    }
                    if ((Hi[6]) & (eArt > EEventArt.eA_499))
                    {
                        UbgT = "2 " + cGed_NOTE + " " + cEv.sBem[2].Trim();
                        GedAus.Historie(_Modul1.Instance.UbgT);
                    }
                }
                if (Options_Quellaus == 6)
                    Quellenaus(eArt);
                goto end_IL_0000_2;
        }
    end_IL_0000_2: // <========== 5
        return;
    }

    private void WriteCondGedTag(int iLevel, string cGedTag, string sData)
    {
        if (sData.Trim() != "")
        {
            GedAus.WriteGedLine($"{iLevel} {cGedTag} {sData.Trim()}", true);
        }
    }

    private void WriteCondGedTagUmwandl(int iLevel, string cGedTag, string sData)
    {
        if (string.IsNullOrWhiteSpace(sData))
        {
            var Fld = sData.Trim();
            if (GedAus.Options_Uml > 0)
            {
                Fld = Modul1.Strngs_Umlaute4(Fld, uml: GedAus.Options_Uml);
            }
            GedAus.WriteGedLine($"{iLevel} {cGedTag} {Fld}", true);
        }

    }

    private void WriteEventWitness(EEventArt eArt, int iFamNr)
    {
        int iter;
        IRecordset dB_WitnessTable = DataModul.DB_WitnessTable;
        if (dB_WitnessTable.RecordCount > 0)
        {
            dB_WitnessTable.MoveFirst();
            dB_WitnessTable.Index = "ZeugSu";
            dB_WitnessTable.Seek("=", iFamNr, "10", eArt, 0);
            for (iter = 1; iter <= 99; iter++)
            {
                if (!dB_WitnessTable.EOF
                    && !dB_WitnessTable.NoMatch
                    && dB_WitnessTable.Fields[WitnessFields.FamNr].AsInt() == iFamNr
                    && dB_WitnessTable.Fields[WitnessFields.Kennz].AsInt() == 10
                    && dB_WitnessTable.Fields[WitnessFields.Art].AsEnum<EEventArt>() == eArt)
                {
                    if (DataModul.NB_SperrPers_Count <= 0
                        || !DataModul.NB_SperrPers_Exists(dB_WitnessTable.Fields[WitnessFields.PerNr].AsInt()))
                    {
                        DataModul.NB_Witness2Table.Index = "Per";
                        DataModul.NB_Witness2Table.Seek("=", dB_WitnessTable.Fields[WitnessFields.PerNr].AsInt());
                        if (DataModul.NB_Witness2Table.RecordCount <= 0 || !DataModul.NB_Witness2Table.NoMatch)
                        {
                            GedAus.WriteGedLine($"2 {cGed_ASSO} @I{dB_WitnessTable.Fields[WitnessFields.PerNr].AsInt().AsString().Trim()}@", true);
                            GedAus.WriteGedLine($"3 {cGed_RELA} Zeuge", true);
                        }
                    }
                    dB_WitnessTable.MoveNext();
                }
            }
        }

    }

    public void Fehlliste(EEventArt eArt, string Dasi, int iPerFamNr, string grund = "")
    {
        string text = "";
        string text2 = "";
        int Nr = iPerFamNr;
        if (eArt == EEventArt.eA_Birth)
        {
            text = "Geburts";
            text2 = "datum bei Person ";
        }
        else if (eArt == EEventArt.eA_Baptism)
        {
            text = "Tauf";
            text2 = "datum bei Person ";
        }
        else if (eArt == EEventArt.eA_Death)
        {
            text = "Todes";
            text2 = "datum bei Person ";
        }
        else if (eArt == EEventArt.eA_Burial)
        {
            text2 = "datum bei Person ";
            text = "Begräbnis";
        }
        else if (eArt == EEventArt.eA_105)
        {
            text = "sonst. Datum";
            text2 = " bei Person ";
        }
        else if (eArt == EEventArt.eA_300)
        {
            text = "Berufs";
            text2 = "datum bei Person ";
        }
        else if (eArt == EEventArt.eA_301)
        {
            text = "Titel";
            text2 = "datum bei Person ";
        }
        else if (eArt == EEventArt.eA_302)
        {
            text = "Wohnort";
            text2 = "datum bei Person ";
        }
        else if (eArt == EEventArt.eA_500)
        {
            text = "Proklamations";
            text2 = "datum bei Familie ";
        }
        else if (eArt == EEventArt.eA_501)
        {
            text = "Verlobungs";
            text2 = "datum bei Familie ";
        }
        else if (eArt == EEventArt.eA_Marriage)
        {
            text = "Heirats";
            text2 = "datum bei Familie ";
        }
        else if (eArt == EEventArt.eA_MarrReligious)
        {
            text = "kirchl.Heirats";
            text2 = "datum bei Familie ";
        }
        else if (eArt == EEventArt.eA_504)
        {
            text = "Scheidunngs";
            text2 = "datum bei Familie ";
        }
        else if (eArt == EEventArt.eA_506)
        {
            text = "Heirats";
            text2 = "datum bei Familie ";
        }
        else if (eArt == EEventArt.eA_507)
        {
            text = "Dimissionale";
            text2 = "datum bei Familie ";
        }
        else if (eArt == EEventArt.eA_505)
        {
            text = "Eheähnl.";
            text2 = "Datum bei Familie ";
        }
        else if (eArt == EEventArt.eA_601)
        {
            text = "Fikt. Heirats";
            text2 = "datum bei Familie ";
        }
        else if (eArt == EEventArt.eA_602)
        {
            text = "Wohnort";
            text2 = "datum bei Familie ";
        }
        else if (eArt == EEventArt.eA_603)
        {
            text = "Sonst. ";
            text2 = "Datum bei Familie ";
        }
        FileSystem.FileClose(99);
        FileSystem.FileOpen(99, Modul1.TempPath + "Gedfehl.dat", OpenMode.Append);
        if (grund == "")
        {
            FileSystem.PrintLine(99, $"Sicherheitszusatz ({Dasi}) des zweiten Feldes beim {text}{text2}{Nr} ist nach Gedcom-Standard nicht zulässig ");
        }
        else
        {
            FileSystem.PrintLine(99, $"Bei Sicherheitszusatz ({DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString().Trim()})  beim {text}{text2}{Nr} ist nach Gedcom-Standard ein zweites Datum nicht zulässig");
        }
        FileSystem.FileClose(99);
    }

    // Missing interface members
    public int Trauaus { get; set; }
    public string SpDisk { get; set; } = "";

    /// <summary>
    /// Changes the text encoding.
    /// </summary>
    /// <param name="sText">The text to convert.</param>
    /// <param name="iOptions_Uml">The umlaut conversion options.</param>
    /// <returns>The converted text.</returns>
    public string Text_ChngEncoding4(string sText, int iOptions_Uml)
    {
        if (string.IsNullOrEmpty(sText))
            return sText;

        if (iOptions_Uml > 0)
        {
            return Modul1.Strngs_Umlaute4(sText, iOptions_Uml);
        }
        return sText;
    }

}