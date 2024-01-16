//using DAO;
using GenFree.Helper;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using GenFree.Interfaces;
using GenFree.Interfaces.Model;
using GenFree.Interfaces.DB;
using System.Reflection;
using GenFree.Sys;
using System.Diagnostics.SymbolStore;
using GenFree.Model;

namespace GenFree.Data;


public static partial class DataModul
{
    public static ILink Link { get; } = new CLink(() => DB_LinkTable); // new IoC.GetReqiredService(ILink);
    public static IEvent Event { get; } = new CEvent(() => DB_EventTable); // new IoC.GetReqiredService(IEvent);
    public static IPlace Place { get; } = new CPlace(() => DB_PlaceTable); // new IoC.GetReqiredService(IPlace);
    public static IPerson Person { get; } = new CPerson(() => DB_PersonTable, new CSysTime()); // new IoC.GetReqiredService(IPerson);
    public static IFamily Family { get; } = new CFamily(() => DB_FamilyTable, new CSysTime()); // new IoC.GetReqiredService(IFamily);
    public static INames Names { get; } = new CNames(() => DB_NameTable); // new IoC.GetReqiredService(INames);
    public static IWitness Witness { get; } = new CWitness(() => DB_WitnessTable);

    public static INB_Person NB_Person { get; } = new CNB_Person(() => NB_PersonTable, Link_MoveAllPaten_ToNBWitn); // new IoC.GetReqiredService(INB_Person);
    public static INB_Family NB_Family { get; } = new CNB_Family(() => NB_FamilyTable); // new IoC.GetReqiredService(INB_Family);

    public static object[] Mandanten = new string[0];

    public static IDBWorkSpace wrkDefault;
    public static IDatabase MandDB { get; set; }
    public static IDatabase DOSB { get; set; }
    public static IDatabase DSB { get; set; }
    public static IDatabase TempDB { get; set; }
    public static IDatabase NB { get; set; }
    public static IDatabase RechDB { get; set; }
    public static IDatabase WB { get; set; }
    public static int NB_SperrPers_Count => NB_SperrPersTable?.RecordCount ?? 0;

    public static IRecordset DB_PersonTable;
    public static IRecordset DB_FamilyTable;
    public static IRecordset DB_EventTable { get; set; }
    public static IRecordset DB_PlaceTable;
    public static IRecordset DB_NameTable;
    public static IRecordset DB_PropertyTable;
    public static IRecordset DB_TexteTable;
    public static IRecordset DB_PictureTable;
    public static IRecordset DB_LinkTable { get; set; }
    public static IRecordset DB_WitnessTable;
    public static IRecordset DB_SourceLinkTable;
    public static IRecordset DB_QuTable;
    public static IRecordset DB_RepoTable;
    public static IRecordset DB_RepoTab;
    public static IRecordset HGATable;
    public static IRecordset DB_GbeTable;
    public static IRecordset DB_OFBTable;
    public static IRecordset DB_WDTable;
    // Temp-DB
    public static IRecordset DT_AncesterTable;
    public static IRecordset DT_RelgionTable;
    public static IRecordset DT_DescendentTable;
    public static IRecordset DSB_SearchTable;
    public static IRecordset KindAhnTable;
    public static IRecordset OrtindTable;
    public static IRecordset OrtSTable;
    public static IRecordset Osy;
    public static IRecordset LeerTable;
    // NB-DB
    public static IRecordset NB_Ahn1Table;
    public static IRecordset NB_Ahn2Table;
    public static IRecordset NB_FrauTable;
    public static IRecordset NB_Frau1Table;
    public static IRecordset NB_Frau2Table;
    public static IRecordset NB_SourceTable;
    public static IRecordset NB_OrtTable;
    public static IRecordset NB_PersonTable;
    public static IRecordset NB_FamilyTable;
    public static IRecordset NB_PictureTable;
    public static IRecordset NB_WitnessTable;
    public static IRecordset NB_SperrPersTable;
    public static IRecordset NB_SperrFamsTable;
    public static IRecordset NB_BemTable;
    //WB-DB
    public static IRecordset WB_FrauTable;

    public static IRecordset BildTab;
    public static IRecordset RechDB_Frauen1;
    // DB-Engine
    public static IDBEngine DAODBEngine_definst;
    public static void DataOpenRO(string Mandant)
    {
        string Verz = Path.GetDirectoryName(Mandant) ?? "";
        var dbEng = DAODBEngine_definst;
        MandDB?.Close();
        TempDB?.Close();
        DOSB?.Close();
        DSB?.Close();
        wrkDefault = dbEng.Workspaces[0];
        MandDB = dbEng.OpenDatabase(Mandant.ToUpper(), true, true, "");
        TempDB = dbEng.OpenDatabase(Path.Combine(Verz, "Tempo.mdb"), true, true, "");
        DOSB = dbEng.OpenDatabase(Path.Combine(Verz, "Ort1.mdb"), true, true, "");
        DSB = dbEng.OpenDatabase(Path.Combine(Verz, "Such.mdb"), true, true, "");
        DT_DescendentTable = TempDB.OpenRecordset("Nachk", RecordsetTypeEnum.dbOpenTable);
        DSB_SearchTable = DSB.OpenRecordset("Such", RecordsetTypeEnum.dbOpenTable);
        OrtSTable = DOSB.OpenRecordset("Ortsuch", RecordsetTypeEnum.dbOpenTable);
        DT_RelgionTable = TempDB.OpenRecordset("Konf", RecordsetTypeEnum.dbOpenTable);
        DT_AncesterTable = TempDB.OpenRecordset("Ahnen1", RecordsetTypeEnum.dbOpenTable);
        KindAhnTable = TempDB.OpenRecordset("Ahnew", RecordsetTypeEnum.dbOpenTable);
        DT_AncesterTable.Index = "PerNr";
    }
    public static void PeekMandant(string mandantname, out int PersonCount, out int FamilyCount, bool xIsReadOnly)
    {
        if (xIsReadOnly)
        {
            MandDB = DAODBEngine_definst.OpenDatabase(mandantname.ToUpper(), true, true, "");
        }
        else
        {
            File.SetAttributes(mandantname, FileAttributes.Normal);
            var path = Path.GetDirectoryName(mandantname) ?? "";
            File.SetAttributes(Path.Combine(path, "Letzter.DAT"), FileAttributes.Normal);
            File.SetAttributes(Path.Combine(path, "Such.Dat"), FileAttributes.Normal);
            MandDB = DAODBEngine_definst.OpenDatabase(mandantname, true, false, "");
        }
        DB_PersonTable = MandDB.OpenRecordset(nameof(dbTables.Personen), RecordsetTypeEnum.dbOpenTable);
        DB_FamilyTable = MandDB.OpenRecordset(nameof(dbTables.Familie), RecordsetTypeEnum.dbOpenTable);
        DB_PersonTable.Index = nameof(PersonIndex.PerNr);
        DB_PersonTable.MoveLast();
        PersonCount = DB_PersonTable.RecordCount;
        DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
        DB_FamilyTable.MoveLast();
        FamilyCount = DB_FamilyTable.RecordCount;
    }

    public static void DataOpen(string Mandant)
    {
        string Verz = Path.GetDirectoryName(Mandant) ?? "";
        var dbEng = DAODBEngine_definst;
        DataClose();

        //            if (dbEng.Workspaces.Count > 0)
        wrkDefault = dbEng.Workspaces[0];
        //          else
        //           ;  
        if (File.Exists(Mandant))
            MandDB = dbEng.OpenDatabase(Mandant, true, true, "");
        else
        {
            //  MandDB = wrkDefault.CreateDatabase(file, dbCreationFlags, DatabaseTypeEnum.dbVersion120);
        }

        string file;
        if (File.Exists(file = Path.Combine(Verz, "Tempo.mdb")))
            TempDB = dbEng.OpenDatabase(file, true, true, "");
        else
        {
            //  TempDB = wrkDefault.CreateDatabase(file, dbCreationFlags, DatabaseTypeEnum.dbVersion40);
        }
        if (File.Exists(file = Path.Combine(Verz, "Ort1.mdb")))
            DOSB = dbEng.OpenDatabase(file, true, true, "");
        else
        {
            // DOSB = wrkDefault.CreateDatabase(file, dbCreationFlags, DatabaseTypeEnum.dbVersion40);
        }
        if (File.Exists(file = Path.Combine(Verz, "Such.mdb")))
            DSB = dbEng.OpenDatabase(file, true, true, "");
        else
        {
            //  DSB = wrkDefault.CreateDatabase(file, dbCreationFlags, DatabaseTypeEnum.dbVersion40);
        }

        CheckDB(TempDB, DTDef);

        DT_DescendentTable = TempDB.OpenRecordset("Nachk", RecordsetTypeEnum.dbOpenTable);
        DSB_SearchTable = DSB.OpenRecordset("Such", RecordsetTypeEnum.dbOpenTable);
        OrtSTable = DOSB.OpenRecordset("Ortsuch", RecordsetTypeEnum.dbOpenTable);
        DT_RelgionTable = TempDB.OpenRecordset("Konf", RecordsetTypeEnum.dbOpenTable);
        DT_AncesterTable = TempDB.OpenRecordset("Ahnen1", RecordsetTypeEnum.dbOpenTable);
        KindAhnTable = TempDB.OpenRecordset("Ahnew", RecordsetTypeEnum.dbOpenTable);
        DT_AncesterTable.Index = "PerNr";

        DB_PersonTable = MandDB.OpenRecordset(nameof(dbTables.Personen), RecordsetTypeEnum.dbOpenTable);
        DB_PersonTable.Index = nameof(PersonIndex.PerNr);
        DB_FamilyTable = MandDB.OpenRecordset(nameof(dbTables.Familie), RecordsetTypeEnum.dbOpenTable);
        DB_NameTable = MandDB.OpenRecordset(nameof(dbTables.Inamen), RecordsetTypeEnum.dbOpenTable);
        DB_NameTable.Index = nameof(NameIndex.PNamen);
        DB_PlaceTable = MandDB.OpenRecordset(nameof(dbTables.Orte), RecordsetTypeEnum.dbOpenTable);
        DB_PlaceTable.Index = nameof(PlaceIndex.OrtNr);
        DB_EventTable = MandDB.OpenRecordset(nameof(dbTables.Ereignis), RecordsetTypeEnum.dbOpenTable);
        DB_EventTable.Index = nameof(EventIndex.ArtNr);
        DB_PropertyTable = MandDB.OpenRecordset(nameof(dbTables.BesitzTab), RecordsetTypeEnum.dbOpenTable);
        DB_PictureTable = MandDB.OpenRecordset(nameof(dbTables.Bilder), RecordsetTypeEnum.dbOpenTable);
        DB_SourceLinkTable = MandDB.OpenRecordset(nameof(dbTables.Tab1), RecordsetTypeEnum.dbOpenTable);
        DB_TexteTable = MandDB.OpenRecordset(nameof(dbTables.Texte), RecordsetTypeEnum.dbOpenTable);
        HGATable = MandDB.OpenRecordset(nameof(dbTables.HGA), RecordsetTypeEnum.dbOpenTable);
        DB_LinkTable = MandDB.OpenRecordset(nameof(dbTables.Tab), RecordsetTypeEnum.dbOpenTable);
        DB_WitnessTable = MandDB.OpenRecordset(nameof(dbTables.Tab2), RecordsetTypeEnum.dbOpenTable);
        LeerTable = MandDB.OpenRecordset(nameof(dbTables.Leer1), RecordsetTypeEnum.dbOpenTable);
        LeerTable.Index = "Leer";
        DB_RepoTable = MandDB.OpenRecordset(nameof(dbTables.Repo), RecordsetTypeEnum.dbOpenTable);
        DB_RepoTab = MandDB.OpenRecordset(nameof(dbTables.RepoTab), RecordsetTypeEnum.dbOpenTable);
        DB_QuTable = MandDB.OpenRecordset(nameof(dbTables.Quellen), RecordsetTypeEnum.dbOpenTable);
        DB_OFBTable = MandDB.OpenRecordset(nameof(dbTables.INDNam), RecordsetTypeEnum.dbOpenTable);

    }

    public static IRecordset OpenLinguaRecordSet(string name)
    {
        TempDB = DAODBEngine_definst.OpenDatabase(name, false, false, "");
        IRecordset recordset = TempDB.OpenRecordset("Lingua", RecordsetTypeEnum.dbOpenTable);
        recordset.Index = "PrimaryKey";
        recordset.MoveFirst();
        return recordset;
    }

    public static void OpenNBDatabase(string name)
    {
        NB?.Close();
        NB = DAODBEngine_definst.OpenDatabase(name, false, false, "");
        NB_Ahn1Table = NB.OpenRecordset(nameof(nbTables.Ahnen1), RecordsetTypeEnum.dbOpenTable);
        NB_Ahn2Table = NB.OpenRecordset(nameof(nbTables.Ahnen2), RecordsetTypeEnum.dbOpenTable);
        NB_Frau1Table = NB.OpenRecordset(nameof(nbTables.Frauen1), RecordsetTypeEnum.dbOpenTable);
        NB_Frau2Table = NB.OpenRecordset(nameof(nbTables.Frauen2), RecordsetTypeEnum.dbOpenTable);
    }

    public static bool ReadNBFrau1Data(int LfdNr, out int persNr, out int famNr, out short gen, out int kek2, out int kek1)
    {
        bool flag;
        gen = default;
        kek2 = default;
        kek1 = default;
        persNr = default;
        famNr = default;
        NB_Frau1Table.Index = "LfNr";
        NB_Frau1Table.Seek("=", LfdNr);
        if (flag = !NB_Frau1Table.NoMatch)
        {
            gen = (short)NB_Frau1Table.Fields["Gen"].AsInt();
            persNr = NB_Frau1Table.Fields["Nr"].AsInt();
            famNr = NB_Frau1Table.Fields["Alt"].AsInt();
            kek1 = NB_Frau1Table.Fields["Kek1"].AsInt();
            kek2 = NB_Frau1Table.Fields["Kek2"].AsInt();
        }

        return flag;
    }


    public static bool NB_SperrPers_Exists(int iPers)
    {
        NB_SperrPersTable.Seek("=", iPers);
        var xExist = !NB_SperrPersTable.NoMatch;
        return xExist;
    }

    public static bool NB_SperrPers_AppendC(int iPersInArb)
    {
        bool result = false;
        if (result = !NB_SperrPers_Exists(iPersInArb))
        {
            NB_SperrPersTable.AddNew();
            NB_SperrPersTable.Fields["Nr"].Value = iPersInArb;
            NB_SperrPersTable.Update();
        }
        return result;
    }

    public static bool NB_SperrFams_Exists(int iFam)
    {
        NB_SperrFamsTable.Seek("=", iFam);
        var xExist = !NB_SperrFamsTable.NoMatch;
        return xExist;
    }

    public static bool NB_SperrFams_AppendC(int iFam)
    {
        bool result = false;
        if (result = !NB_SperrFams_Exists(iFam))
        {
            NB_SperrFamsTable.AddNew();
            NB_SperrFamsTable.Fields["Nr"].Value = iFam;
            NB_SperrFamsTable.Update();
        }
        return result;
    }

    public static void Link_MoveAllPaten_ToNBWitn(int persInArb)
    {
        var b = 1;
        foreach (var cLink in Link.ReadAllFams(persInArb, ELinkKennz.lkGodparent))
        {
            NB_WitnessTable.AddNew();
            NB_WitnessTable.Fields["Person"].Value = cLink.iPersNr;
            NB_WitnessTable.Update();
            b++;
        }
    }

    public static void Witness_CopyToNB(int iNr, bool xAR)
    {
        var num5 = 1;
        foreach (var cWitness in Witness.ReadAllFams(iNr, 10))
        {
            if (xAR ^ DB_WitnessTable.Fields[nameof(WitnessFields.Art)].AsInt() <= 499)
            {
                NB_WitnessTable.AddNew();
                NB_WitnessTable.Fields["Person"].Value = cWitness.iPers;
                NB_WitnessTable.Update();
            }
            DB_WitnessTable.MoveNext();
            num5++;
        }
    }


    public static void Ahn1_Update(int persInArb, int famInArb, int num5, decimal num6, short gen, string name)
    {
        NB_Ahn1Table.Index = "PerNr";
        NB_Ahn1Table.Seek("=", persInArb);
        if (NB_Ahn1Table.NoMatch)
        {
            NB_Ahn1Table.AddNew();
            NB_Ahn1Table.Fields["PerNr"].Value = persInArb;
            NB_Ahn1Table.Fields["Gene"].Value = gen;
            NB_Ahn1Table.Fields["Ahn1"].Value = num6;
            NB_Ahn1Table.Fields["Ahn2"].Value = num5;
            NB_Ahn1Table.Fields["Ahn3"].Value = 0;
            NB_Ahn1Table.Fields["Weiter"].Value = 0;
            NB_Ahn1Table.Fields["Ehe"].Value = famInArb;
            NB_Ahn1Table.Update();
        }
        else if (NB_Ahn1Table.Fields["Ahn1"].AsInt() != 0)
        {
            NB_Ahn1Table.Edit();
            NB_Ahn1Table.Fields["Weiter"].Value = 1;
            NB_Ahn1Table.Update();

            NB_Ahn1Table.AddNew();
            NB_Ahn1Table.Fields["PerNr"].Value = persInArb;
            NB_Ahn1Table.Fields["Gene"].Value = gen;
            NB_Ahn1Table.Fields["Ahn1"].Value = num6;
            NB_Ahn1Table.Fields["Ahn2"].Value = num5;
            NB_Ahn1Table.Fields["Ahn3"].Value = 0;
            NB_Ahn1Table.Fields["Weiter"].Value = 1;//??
            NB_Ahn1Table.Fields["Ehe"].Value = famInArb;
            NB_Ahn1Table.Update();
        }
        else
        {
            NB_Ahn1Table.Edit();
            NB_Ahn1Table.Fields["Ahn1"].Value = num6;
            NB_Ahn1Table.Fields["Ahn2"].Value = num5;
            NB_Ahn1Table.Fields["Gene"].Value = gen;
            NB_Ahn1Table.Fields["Ehe"].Value = famInArb;
            NB_Ahn1Table.Fields["Name"].Value = name;
            NB_Ahn1Table.Update();
        }
    }
#nullable enable
    public static void ReplaceNBDatafile(string destination, string source, Action? action = null, bool nbLeaveOpen = false)
    {
        WB?.Close();
        NB?.Close();
        if (File.Exists(source))
        {
            if (File.Exists(destination))
                File.Delete(destination);
            File.Copy(source, destination);
        }
        NB = DAODBEngine_definst.OpenDatabase(destination, false, false, "");
        NB.TryExecute($"ALTER TABLE {dbTables.Frauen} ADD COLUMN {FrauenFields.Kek} Text(40);");
        NB.TryExecute($"ALTER TABLE {dbTables.Frauen} ADD COLUMN {FrauenFields.PNr} Integer);");
        NB.TryExecute($"CREATE UNIQUE INDEX {FrauenIndex.Nr} ON {dbTables.Frauen} ([{FrauenFields.PNr}]);");
        NB_FrauTable = NB.OpenRecordset("Frauen", RecordsetTypeEnum.dbOpenTable);
        OrtindTable = NB.OpenRecordset("Ortind", RecordsetTypeEnum.dbOpenTable);
        action?.Invoke();
        if (!nbLeaveOpen)
            NB.Close();
    }
#nullable restore
    public static void CreateWBDatabase(string destination, string source)
    {
        WB_FrauTable?.Close();
        WB?.Close();
        NB?.Close();
        if (File.Exists(source))
        {
            if (File.Exists(destination))
                File.Delete(destination);
            File.Copy(source, destination);
        }
        WB = DAODBEngine_definst.OpenDatabase(destination, false, false, "");
        WB.TryExecute("ALTER TABLE Frauen ADD COLUMN Name Text(50);");
        WB.TryExecute("CREATE INDEX Name ON Frauen ([Name]);");
        WB_FrauTable = WB.OpenRecordset("Frauen", RecordsetTypeEnum.dbOpenTable);
        WB_FrauTable.Index = "LfNr";
    }
    public static void PrepRechData(string destination, string source)
    {
        var nB = RechDB;
        nB?.Close();
        if (!Directory.Exists(Path.GetDirectoryName(destination)))
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        else
            Directory.EnumerateFiles(Path.GetDirectoryName(destination) ?? "").ToList()
                .ForEach(File.Delete);
        File.Copy(source, destination);
        nB = DAODBEngine_definst.OpenDatabase(destination, false, false, "");
        NB_OrtTable = nB.OpenRecordset("ORTE", RecordsetTypeEnum.dbOpenTable);
        NB_OrtTable.Index = "ORT";
        NB_SourceTable = nB.OpenRecordset("QuellTemp", RecordsetTypeEnum.dbOpenTable);
        NB_PictureTable = nB.OpenRecordset("Bilder", RecordsetTypeEnum.dbOpenTable);
        RechDB_Frauen1 = nB.OpenRecordset("Frauen1", RecordsetTypeEnum.dbOpenTable);
        NB_PersonTable = nB.OpenRecordset("Personen1", RecordsetTypeEnum.dbOpenTable);
        NB_FamilyTable = nB.OpenRecordset("Familie1", RecordsetTypeEnum.dbOpenTable);
    }

    public static void OpenQuellData(string destination, string source, bool xReplace)
    {
        NB.Close();
        if (xReplace)
        {
            if (!Directory.Exists(Path.GetDirectoryName(destination)))
                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            else
                Directory.EnumerateFiles(Path.GetDirectoryName(destination) ?? "").ToList()
                    .ForEach(File.Delete);
            File.Copy(source, destination);
        }
        NB = DAODBEngine_definst.OpenDatabase(destination, false, false, "");
        NB_OrtTable = NB.OpenRecordset("ORTE", RecordsetTypeEnum.dbOpenTable);
        NB_OrtTable.Index = "ORT";
        NB_SourceTable = NB.OpenRecordset("QuellTemp", RecordsetTypeEnum.dbOpenTable);
        NB_PictureTable = NB.OpenRecordset("Bilder", RecordsetTypeEnum.dbOpenTable);
        NB_FrauTable = NB.OpenRecordset("Frauen1", RecordsetTypeEnum.dbOpenTable);
        NB_PersonTable = NB.OpenRecordset("Personen1", RecordsetTypeEnum.dbOpenTable);
        NB_FamilyTable = NB.OpenRecordset("Familie1", RecordsetTypeEnum.dbOpenTable);
    }

    public static void BemTable_AppendRaw(int iNr, string sPF, string sArt, string sBem, object iLfNr)
    {
        IRecordset nB_BemTable = NB_OrtTable;
        nB_BemTable.AddNew();
        nB_BemTable.Fields["Nr"].Value = iNr;
        nB_BemTable.Fields["PF"].Value = sPF;
        nB_BemTable.Fields["Art"].Value = sArt;
        nB_BemTable.Fields["BemText"].Value = sBem;
        nB_BemTable.Fields["LFnr"].Value = iLfNr;
        nB_BemTable.Update();
    }


    #region Names Methods

    #endregion

    #region Witness Methods
    public static void Witness_Delete(long perfamNr, long persInArb, int kennz, object erArt, object lfNR)
    {
        DB_WitnessTable.Index = nameof(WitnessIndex.Fampruef);
        DB_WitnessTable.Seek("=", perfamNr, persInArb, kennz, erArt, lfNR);
        if (!DB_WitnessTable.NoMatch)
        {
            DB_WitnessTable.Delete();
        }
    }

    public static void Witness_DeleteAllE(int persInArb, string eWKennz)
    {
        DB_WitnessTable.Index = nameof(WitnessIndex.ElSu);
        DB_WitnessTable.Seek("=", persInArb, eWKennz);
        var I = 1;
        while (I <= 99
            && !DB_WitnessTable.NoMatch
            && !DB_WitnessTable.EOF)
        {
            if (!((DB_WitnessTable.Fields[nameof(WitnessFields.PerNr)].AsInt() != persInArb)
              || (DB_WitnessTable.Fields[nameof(WitnessFields.Kennz)].AsString() != eWKennz)))
            {
                DB_WitnessTable.Delete();
                DB_WitnessTable.MoveNext();
                I++;
            }
        }
    }

    public static void Witness_DeleteAllF(int persInArb, string sWKennz)
    {
        DB_WitnessTable.Index = nameof(WitnessIndex.FamSu);
        DB_WitnessTable.Seek("=", persInArb, sWKennz);
        var I = 1;
        while (I <= 99
            && !DB_WitnessTable.NoMatch
            && !DB_WitnessTable.EOF
            && DB_WitnessTable.Fields[nameof(WitnessFields.FamNr)].AsInt() == persInArb
            && DB_WitnessTable.Fields[nameof(WitnessFields.Kennz)].AsString() == sWKennz)
        {
            if (DB_WitnessTable.Fields[nameof(WitnessFields.Art)].AsInt() < 500)
            {
                DB_WitnessTable.Delete();
            }
            DB_WitnessTable.MoveNext();
            I++;
        }
    }

    public static void Witness_DeleteAllZ(int persInArb, int sWKennz, EEventArt eArt, short iLfNr)
    {
        DB_WitnessTable.Index = nameof(WitnessIndex.ZeugSu);
        DB_WitnessTable.Seek("=", persInArb, sWKennz, eArt, iLfNr);
        while (!DB_WitnessTable.NoMatch
            && !DB_WitnessTable.EOF
            && DB_WitnessTable.Fields[nameof(WitnessFields.FamNr)].AsInt() == persInArb
            && DB_WitnessTable.Fields[nameof(WitnessFields.Kennz)].AsInt() == sWKennz
            && DB_WitnessTable.Fields[nameof(WitnessFields.Kennz)].AsEnum<EEventArt>() == eArt
            && DB_WitnessTable.Fields[nameof(WitnessFields.Kennz)].AsInt() == iLfNr)
        {
            DB_WitnessTable.Delete();
            DB_WitnessTable.MoveNext();
        }
    }

    public static void Witness_DeleteAllOrphanFam()
    {
        DB_WitnessTable.Index = nameof(WitnessIndex.Fampruef);
        DB_WitnessTable.MoveFirst();
        while (!DB_WitnessTable.EOF
            && !DB_WitnessTable.NoMatch)
        {
            if (DB_WitnessTable.Fields[nameof(WitnessFields.Art)].AsInt() > 499
                && !Family.Exists(DB_WitnessTable.Fields[nameof(WitnessFields.FamNr)].AsInt()))
            {
                DB_WitnessTable.Delete();
            }
            DB_WitnessTable.MoveNext();
        }
    }

    public static void Witness_Append(int suchPer, int perfamNr, int kennz1, EEventArt erArt, short lfNR)
    {
        DB_WitnessTable.Index = nameof(WitnessIndex.Fampruef);
        DB_WitnessTable.Seek("=", perfamNr, suchPer, kennz1, erArt, lfNR);
        if (DB_WitnessTable.NoMatch)
        {
            Witness_AppendRaw(suchPer, perfamNr, erArt, lfNR, kennz1);
        }
    }

    public static void Witness_AppendRaw(int suchPer, int perfamNr, EEventArt erArt, short lfNR, int kennz1)
    {
        DB_WitnessTable.AddNew();
        DB_WitnessTable.Fields[nameof(WitnessFields.PerNr)].Value = suchPer;
        DB_WitnessTable.Fields[nameof(WitnessFields.FamNr)].Value = perfamNr;
        DB_WitnessTable.Fields[nameof(WitnessFields.Kennz)].Value = kennz1;
        DB_WitnessTable.Fields[nameof(WitnessFields.Art)].Value = erArt;
        DB_WitnessTable.Fields[nameof(WitnessFields.LfNr)].Value = lfNR;
        DB_WitnessTable.Update();
    }

    public static void Witness_Add(int personNr, int iPerfam, EEventArt art, short lfNR, int iWKennz = 10)
        => Witness_AppendRaw(personNr, iPerfam, art, lfNR, iWKennz);

    public static bool Witness_Exist(int persInArb, EEventArt eEvtArt, short lfNR, int eWKennz = 10)
    {
        DB_WitnessTable.Index = nameof(WitnessIndex.ZeugSu);
        DB_WitnessTable.Seek("=", persInArb, eWKennz, eEvtArt, lfNR);
        return !DB_WitnessTable.NoMatch;
    }



    #endregion

    #region Texte Methods
    public static bool Textlese(int nIdx, out string sText, out string sLeitName)
    {
        bool result;
        sLeitName = "";
        sText = "";
        if (nIdx == 0)
        {
            return false;
        }
        DB_TexteTable.Index = nameof(TexteIndex.TxNr);
        DB_TexteTable.Seek("=", nIdx);
        if (result = !DB_TexteTable.NoMatch)
        {
            sText = DB_TexteTable.Fields[nameof(TexteFields.Txt)].AsString();
            if (DB_TexteTable.Fields[nameof(TexteFields.Leitname)].Value is string s)
            {
                sLeitName = s.Trim();
            }
        }
        // ??
        sText = sText.Replace("ssss", "ß");
        sLeitName = sLeitName.Replace("ssss", "ß");
        return result;
    }

    public static bool Textlese(int nIdx, out string sText) => Textlese(nIdx, out sText, out _);

    public static string TextLese1(int nIdx) => Textlese(nIdx, out string sText, out _)
        ? sText
        : "";

    public static (string, string) TextLese2(int nIdx) => Textlese(nIdx, out string sText, out string sText2)
        ? (sText, sText2)
        : ("", "");

    public static void Texte_Schreib(string Wort, string Leitn, ETextKennz Kennz, out int Satz)
    {
        Wort = Wort.Trim();
        if (Wort == "")
        {
            Satz = 0;
            return;
        }
        Wort = Wort.Replace("ß", "ssss");
        Leitn = Leitn.Replace("ß", "ssss");
        IRecordset dB_TexteTable = DB_TexteTable;

        dB_TexteTable.Index = nameof(TexteIndex.ITexte);
        dB_TexteTable.Seek("=", Wort, Kennz);
        // update
        if (!dB_TexteTable.NoMatch)
        {
            IField fld;
            if ((fld = dB_TexteTable.Fields[nameof(TexteFields.Leitname)]).Value is string s
                && s != Leitn)
            {
                dB_TexteTable.Edit();
                fld.Value = Leitn;
            }
            if ((fld = dB_TexteTable.Fields[nameof(TexteFields.Bem)]).Value is string s2
                && s2 != " ")
            {
                dB_TexteTable.Edit();
                fld.Value = Leitn;
            }
            if (dB_TexteTable.EditMode != 0)
                dB_TexteTable.Update();
            Satz = dB_TexteTable.Fields[nameof(TexteFields.TxNr)].AsInt();
            return;
        }
        dB_TexteTable.Index = nameof(TexteIndex.TxNr);
        if (dB_TexteTable.RecordCount == 0)
            Satz = 1;
        else
        {
            dB_TexteTable.MoveLast();
            Satz = dB_TexteTable.Fields[nameof(TexteFields.TxNr)].AsInt() + 1;
        }
        switch (Kennz)
        {
            case ETextKennz.tkName:
                break;
            case ETextKennz.H_:
            case ETextKennz.I_:
            case ETextKennz.J_:
            case ETextKennz.K_:
            case ETextKennz.L_:
                Wort = Wort.Left(1).ToUpper() + (Wort.Length > 1 ? Wort.Substring(1).ToLower() : "");
                break;
            default:
                Wort = Wort.Left(240);
                break;

        }
        Texte_AppendRaw(Kennz, Wort, " ", Leitn, Satz);
        // ??
        dB_TexteTable.MoveLast();
    }

    private static void Texte_AppendRaw(ETextKennz Kennz, string Wort, string sBem, string Leitn, int Satz)
    {
        IRecordset dB_TexteTable = DB_TexteTable;
        dB_TexteTable.AddNew();
        dB_TexteTable.Fields[nameof(TexteFields.Kennz)].Value = Kennz;
        dB_TexteTable.Fields[nameof(TexteFields.Txt)].Value = Wort;
        dB_TexteTable.Fields[nameof(TexteFields.Bem)].Value = sBem;
        dB_TexteTable.Fields[nameof(TexteFields.Leitname)].Value = Leitn;
        dB_TexteTable.Fields[nameof(TexteFields.TxNr)].Value = Satz;
        dB_TexteTable.Update();
    }

    public static bool RemoveOrphanText(out ETextKennz value)
    {
        bool flag = false;
        int TextNr = DB_TexteTable.Fields[nameof(TexteFields.TxNr)].AsInt();
        value = DB_TexteTable.Fields[nameof(TexteFields.Kennz)].AsEnum<ETextKennz>();
        switch (value)
        {
            case ETextKennz.tkName or ETextKennz.F_ or ETextKennz.V_ or ETextKennz.A_ or ETextKennz.B_ or ETextKennz.C_ or ETextKennz.D_ or ETextKennz.U_:
                if (!Names.ExistText(TextNr))
                {
                    if (!OFB_TextExist(TextNr))
                    {
                        DB_TexteTable.Delete();
                    }
                }
                break;
            case ETextKennz.H_:
                if (!Place_TextExistO(TextNr))
                {
                    DB_TexteTable.Delete();
                }
                break;
            case ETextKennz.I_:
                if (!Place_TextExistOT(TextNr))
                {
                    DB_TexteTable.Delete();
                }
                break;
            case ETextKennz.J_:
                if (!Place_TextExistK(TextNr))
                {
                    DB_TexteTable.Delete();
                }
                break;
            case ETextKennz.K_:
                if (!Place.Exists(PlaceIndex.L, TextNr))
                {
                    DB_TexteTable.Delete();
                }
                break;
            case ETextKennz.L_:
                if (!Place.Exists(PlaceIndex.S, TextNr))
                {
                    DB_TexteTable.Delete();
                }
                break;
            case ETextKennz.E_ or ETextKennz.M_ or ETextKennz.G_ or ETextKennz.Q_ or ETextKennz.W_:
                if (!Event.Exists(EventIndex.KText, TextNr))
                {
                    if (!OFB_TextExist(TextNr))
                    {
                        DB_TexteTable.Delete();
                    }
                }
                break;
            case ETextKennz.O_:
                if (!Event.Exists(EventIndex.PText, TextNr))
                {
                    DB_TexteTable.Delete();
                }
                break;
            case ETextKennz.T_:
                if (!Event.Exists(EventIndex.NText, TextNr))
                {
                    DB_TexteTable.Delete();
                }
                break;
            case ETextKennz.Z_:
                DT_RelgionTable.Close();
                TempDB.TryExecute("DROP Table Konf");
                TempDB.TryExecute("Create Table Konf (PerNr long)");
                TempDB.TryExecute("Alter table Konf Add column Textnr long;");
                TempDB.TryExecute("create Index T on Konf ([TextNr]);");
                DT_RelgionTable = TempDB.OpenRecordset("Konf", RecordsetTypeEnum.dbOpenTable);
                flag = true;
                //=============================
                break;
        }
        return flag;
    }

    public static bool Place_TextExistK(int TextNr)
        => Place.Exists(PlaceIndex.K, TextNr);

    public static bool Place_TextExistOT(int TextNr)
        => Place.Exists(PlaceIndex.OT, TextNr);

    public static bool Place_TextExistO(int TextNr)
    => Place.Exists(PlaceIndex.O, TextNr);

    private static bool OFB_TextExist(int TextNr)
    {
        DB_OFBTable.Index = "IndNum";
        DB_OFBTable.Seek("=", TextNr);
        return !DB_OFBTable.NoMatch;
    }

    public static bool OFB_DataExists(int persInArb, string index, object sKennz)
    {
        DB_OFBTable.Index = index;
        DB_OFBTable.Seek("=", persInArb, sKennz);
        return !DB_OFBTable.NoMatch;
    }

    public static void OFB_Update(string Kennz, int persInArb, int satz)
    {
        DB_OFBTable.Index = "Indn";
        DB_OFBTable.Seek("=", persInArb, Kennz, satz);
        if (DB_OFBTable.NoMatch)
        {
            DB_OFBTable.AddNew();
            DB_OFBTable.Fields["PerNr"].Value = persInArb;
            DB_OFBTable.Fields["Kennz"].Value = Kennz;
            DB_OFBTable.Fields["TextNr"].Value = satz;
            DB_OFBTable.Update();
        }
    }


    #endregion

    public static IEnumerable<int> FindParentialFamilies(int persInArb)
    {
        foreach (var link in Link.ReadAllPers(persInArb, ELinkKennz.lkChild))
        {
            yield return link.iFamNr;
        }
    }

    private static void CheckDB(IDatabase Database, stTableDef[] Def)
    {
        foreach (var tbldef in Def)
        {
            if (tbldef.xDrop)
            {
                try
                {
                    //TOdo                        Database.TableDefs.Delete(tbldef.Name);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                try
                {
                    /*
                    var tbl = Database.TableDefs[tbldef.Name];
                    foreach (var fld in tbldef.Fields)
                    {
                        if (!DbFieldExists(tbl, fld.Name))
                        {
                            var f = tbl.CreateField(fld.Name, fld.Typ, fld.Laenge);
                            f.Required = !fld.xNull;
                            tbl.Fields.Append(f);
                        }
                    }
                    */
                }
                catch (Exception)
                {
                    /*
                    var tbl = Database.CreateTableDef(tbldef.Name);
                    foreach (var fld in tbldef.Fields)
                    {
                        var f = tbl.CreateField(fld.Name, fld.Typ, fld.Laenge);
                        f.Required = !fld.xNull;
                        tbl.Fields.Append(f);
                    }
                    foreach (var idx in tbldef.Indexes)
                    {
                        var i = tbl.CreateIndex(idx.Name);
                        i.Unique = idx.Unique;
                        i.IgnoreNulls = idx.IgnoreNull;
                        i.Fields = string.Join(", ", idx.Fields);
                        tbl.Indexes.Append(i);
                    }
                    Database.TableDefs.Append(tbl);
                    */
                }
            }
        }
    }

    public static void Db_Def(IDatabase Database, Action<string> print)
    {
        print("public static stTableDef[] Def = {");

        foreach (TableDef tbl in Database.TableDefs())
            if (!tbl.Name.StartsWith("MSys"))
            {
                print($"\tnew() {{ Name = \"{tbl.Name}\",");
                print("\t  Fields = new stFieldDef[]{");
                //             new("Ortnr",new[] { "Nr" }),
                //             new("Ortsu",new[] { "Name" }),

                foreach (FieldDef fld in tbl.Fields)
                {
                    print($"\t\tnew(\"{fld.Name}\", DataTypeEnum.{fld.Type}) " +
                        $"{(fld.Type == TypeCode.String ? $"{{ Laenge = {fld.Size} }}" : "")}" +
                        $"{(fld.Required ? $"{{ xNull = true }}" : "")}, ");
                }
                print("\t\t},");
                try
                {
                    if (tbl.Indexes.Count > 0)
                    {
                        print("\t  Indexes = new stIndex[]{");
                        foreach (IndexDef idx in tbl.Indexes)
                        {
                            var fields = idx.Fields;
                            print($"\t\tnew(\"{idx.Name}\", new[] {{ {fields} }} )"
                                + $"{(idx.Unique ? "{ Unique = true }" : "")},"
                                + $"{(idx.IgnoreNulls ? "{ IgnoreNull = true }" : "")},");
                        }
                        print("\t\t}");
                    }
                }
                catch (Exception)
                {
                }
                print("\t\t},");
            }

    }

    public static void DataClose()
    {
        MandDB?.Close();
        TempDB?.Close();
        DOSB?.Close();
        DSB?.Close();
    }

    public static void CompactDatabase(string v1, string v2)
    {
        throw new NotImplementedException();
    }

    public static IDatabase OpenDatabase(string v1, bool v2, bool v3, string v4)
        => DAODBEngine_definst.OpenDatabase(v1, v2, v3, v4);

    public static void Event_SetValAppend(EEventArt eArt, int iLink, short iLfNr, EventFields eSetField, string sNewVal)
    {
        var dB_EventTable = Event.Seek((eArt, iLink, iLfNr));
        if (dB_EventTable?.NoMatch != false)
        {
            dB_EventTable = DB_EventTable;
            dB_EventTable.AddNew();
            dB_EventTable.Fields[nameof(EventFields.Art)].Value = eArt;
            dB_EventTable.Fields[nameof(EventFields.PerFamNr)].Value = iLink;
            dB_EventTable.Fields[nameof(EventFields.LfNr)].Value = iLfNr;
            dB_EventTable.Update();
        }
        dB_EventTable.Edit();
        var field = dB_EventTable.Fields[nameof(eSetField)];
        field.Value = field.AsString().Trim() == ""
            ? sNewVal + " "
            : field.AsString() + " " + sNewVal;
        dB_EventTable.Update();
    }

    public static T Event_GetValue<T>(int persInArb, EEventArt iEventType, EventFields eGetField, Func<IField, T> conv)
    {
        var dB_EventTable = DB_EventTable;
        dB_EventTable.Index = nameof(EventIndex.ArtNr);
        dB_EventTable.Seek("=", iEventType, persInArb, 0);
        T sEvtBem4 = conv(dB_EventTable.Fields[nameof(eGetField)]);
        return sEvtBem4;
    }

    public static void Events_DeleteAllPersNonVitEv(int persInArb)
    {
        var dB_EventTable = DB_EventTable;
        dB_EventTable.Index = nameof(EventIndex.BeSu);

        var UVar = EEventArt.eA_300;
        while (UVar <= EEventArt.eA_302)
        {
            dB_EventTable.Seek("=", UVar.AsString().Trim(), persInArb);
            if (!dB_EventTable.NoMatch)
            {
                while (!dB_EventTable.EOF
                    && !(dB_EventTable.Fields[nameof(EventFields.PerFamNr)].AsInt() != persInArb)
                    && !(dB_EventTable.Fields[nameof(EventFields.Art)].AsEnum<EEventArt>() != UVar))
                {
                    if (!dB_EventTable.NoMatch)
                    {
                        dB_EventTable.Delete();
                    }
                    dB_EventTable.MoveNext();
                }
            }
            UVar += 1;
        }

    }
    public static IEnumerable<IEventData> Event_ReadAllGt(EventIndex eIndex, int iIndexVal)
    {
        var dB_EventTable = DB_EventTable;
        dB_EventTable.Index = $"{eIndex}";
        dB_EventTable.Seek(">=", iIndexVal);

        while (!dB_EventTable.EOF
            && !dB_EventTable.NoMatch)
        {
            yield return new CEventData(dB_EventTable);
            dB_EventTable.MoveNext();
        }
    }

    public static void Family_AllSetEditDate()
    {
        DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
        if (DB_FamilyTable?.RecordCount > 0)
        {
            DB_FamilyTable.MoveFirst();

            DB_FamilyTable.Seek("=", 1);
            while (!DB_FamilyTable.EOF)
            {
                if (DB_FamilyTable.Fields[nameof(FamilyFields.EditDat)].AsDate() == default)
                {
                    DB_FamilyTable.Edit();
                    DB_FamilyTable.Fields[nameof(FamilyFields.EditDat)].Value = DB_FamilyTable.Fields[nameof(FamilyFields.AnlDatum)].Value;
                    DB_FamilyTable.Update();
                }
                DB_FamilyTable.MoveNext();
            }
        }

    }

    public static void Family_AppendRaw(int iFamNr, int iName, int iAeb, string sBem1)
    {
        DB_FamilyTable.AddNew();
        DB_FamilyTable.Fields[nameof(FamilyFields.AnlDatum)].Value = DateTime.Now.ToString("yyyyMMdd");
        DB_FamilyTable.Fields[nameof(FamilyFields.EditDat)].Value = DateTime.Now.ToString("yyyyMMdd");
        DB_FamilyTable.Fields[nameof(FamilyFields.Prüfen)].Value = "1    ";
        DB_FamilyTable.Fields[nameof(FamilyFields.Bem1)].Value = sBem1;
        DB_FamilyTable.Fields[nameof(FamilyFields.FamNr)].Value = iFamNr;
        DB_FamilyTable.Fields[nameof(FamilyFields.Name)].Value = iName;
        DB_FamilyTable.Fields[nameof(FamilyFields.Aeb)].Value = iAeb;
        DB_FamilyTable.Fields[nameof(FamilyFields.Fuid)].Value = Guid.NewGuid();
        DB_FamilyTable.Update();
    }

    public static bool Family_DeleteA(int Fam1, string v, Action<object> action)
    {
        bool xRes;
        DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
        DB_FamilyTable.Seek("=", Fam1);
        if (xRes = !DB_FamilyTable.NoMatch)
        {
            action?.Invoke(DB_FamilyTable.Fields[v].Value);
            DB_FamilyTable.Delete();
        }
        return xRes;
    }


    public static void FrauTab_SetParentTo1(IFamilyPersons family)
    {
        IRecordset wB_FrauTable = WB_FrauTable;
        wB_FrauTable.Index = "LfNR";
        foreach (var iParent in new[] { family.Mann, family.Frau })
            if (iParent > 0)
            {
                wB_FrauTable.Seek("=", iParent);
                if (!wB_FrauTable.NoMatch)
                {
                    wB_FrauTable.Edit();
                    wB_FrauTable.Fields["NR"].Value = 1;
                    wB_FrauTable.Update();
                }
            }
    }

    public static void FrauTab_AddParent(IFamilyPersons family)
    {
        IRecordset wB_FrauTable = WB_FrauTable;
        foreach (var iParent in new[] { family.Mann, family.Frau })
            if (iParent > 0)
            {
                wB_FrauTable.AddNew();
                wB_FrauTable.Fields["LfNr"].Value = iParent;
                wB_FrauTable.Fields["Nr"].Value = 0;
                wB_FrauTable.Update();
            }
    }

    public static void Names_UpdateAllSetVal(NameIndex eIndex, NameFields eIndexField, int iIndexVal, int iNewVal)
    {
        IRecordset dB_NameTable = DB_NameTable;
        dB_NameTable.Index = $"{eIndex}";
        dB_NameTable.Seek("=", iIndexVal);
        while (!dB_NameTable.EOF
                && !dB_NameTable.NoMatch
                && !(dB_NameTable.Fields[$"{eIndexField}"].AsInt() != iIndexVal))
        {
            dB_NameTable.Edit();
            dB_NameTable.Fields[$"{eIndexField}"].Value = iNewVal;
            dB_NameTable.Update();
            dB_NameTable.MoveNext();
        }
    }


    public static void SearchTab_Delete(int persInArb)
    {
        DSB_SearchTable.Index = "Nummer";
        DSB_SearchTable.Seek(">=", persInArb);
        if (!DSB_SearchTable.NoMatch
            && DSB_SearchTable.Fields["Nummer"].AsInt() == persInArb)
        {
            DSB_SearchTable.Delete();
        }
    }

    public static void Descendent_DeleteAll(int persInArb)
    {
        DT_DescendentTable.Index = "Pernr";
        DT_DescendentTable.Seek("=", persInArb);
        var I = 1;
        while (!DT_DescendentTable.NoMatch
            && I <= 99
            && !DT_DescendentTable.EOF
            && !(DT_DescendentTable.Fields["Pr"].AsInt() != persInArb))
        {
            DT_DescendentTable.Delete();
            DT_DescendentTable.MoveNext();
            I++;
        }
    }


    #region Ancesters Methods
    public static void Ancestrers_DeleteAll(int persInArb)
    {
        DT_AncesterTable.Index = "Pernr";
        DT_AncesterTable.Seek("=", persInArb);
        var I = 1;
        if (I <= 99
            && !DT_AncesterTable.NoMatch
            && !DT_AncesterTable.EOF
            && !(DT_AncesterTable.Fields["Pernr"].AsInt() != persInArb))
        {
            DT_AncesterTable.Delete();
            DT_AncesterTable.MoveNext();
        }
    }

    public static (int iPerson, int iAhn, int iWeiter, int iGen) Ancester_GetAncData(int iAnc)
    {
        var dT_AncesterTable = DT_AncesterTable;
        dT_AncesterTable.Index = "Ahnen";
        dT_AncesterTable.Seek("=", iAnc);
        if (!dT_AncesterTable.NoMatch && !dT_AncesterTable.EOF
                && dT_AncesterTable.Fields["Ahn"].AsInt() == iAnc)
        {
            int Anc_iAhn = dT_AncesterTable.Fields["Ahn"].AsInt();
            int Anc_iPerson = dT_AncesterTable.Fields["PerNr"].AsInt();
            int Anc_iWeiter = dT_AncesterTable.Fields["Weiter"].AsInt();
            int Anc_iGen = dT_AncesterTable.Fields["Gen"].AsInt();
            return (Anc_iPerson, Anc_iAhn, Anc_iWeiter, Anc_iGen);

        }
        else
            return (0, 0, 0, 0);
    }
    public static IEnumerable<(int iAhn, int iWeiter, int iGen)> Ancester_EmitPersonData(int PersonNr)
    {
        IRecordset dT_AncesterTable = DT_AncesterTable;
        dT_AncesterTable.Index = "Pernr";
        dT_AncesterTable.Seek("=", PersonNr);
        if (!dT_AncesterTable.NoMatch)
        {
            while (!dT_AncesterTable.EOF
                && dT_AncesterTable.Fields["PerNr"].AsInt() == PersonNr)
            {
                int Anc_iAhn = dT_AncesterTable.Fields["Ahn"].AsInt();
                int Anc_iWeiter = dT_AncesterTable.Fields["Weiter"].AsInt();
                int Anc_iGen = dT_AncesterTable.Fields["Gen"].AsInt();
                yield return (Anc_iAhn, Anc_iWeiter, Anc_iGen);
                dT_AncesterTable.MoveNext();

            }

        }
    }


    #endregion

    #region SourceLink Methods
    public static IEnumerable<ISourceLinkData> SourceLink_ReadData(int persInArb, EEventArt eEventArt)
    {
        DB_SourceLinkTable.Index = "Tab22";
        DB_SourceLinkTable.Seek("=", 3, persInArb, eEventArt, 0);
        while (!DB_SourceLinkTable.EOF)
        {
            var Src = new CSourceLinkData(DB_SourceLinkTable);
            if (!DB_SourceLinkTable.NoMatch
                && EEventArt.eA_Unknown != Src.eArt)
            {
                if (Src.iLinkType != 3 // Event
                    || Src.iPersNr > persInArb
                    || Src.eArt != eEventArt
                    || Src.iLfdNr != 0)
                    break;
                yield return Src;
            }
            DB_SourceLinkTable.MoveNext();
        }
        yield break;
    }

    public static void SourceLink_AppendRaw(int iLinkType, int iLink, int num15, EEventArt eArt, int iLfNr)
    {
        DB_SourceLinkTable.AddNew();
        DB_SourceLinkTable.Fields[nameof(SourceLinkFields._1)].Value = iLinkType;
        DB_SourceLinkTable.Fields[nameof(SourceLinkFields._2)].Value = iLink;
        DB_SourceLinkTable.Fields[nameof(SourceLinkFields._3)].Value = num15;
        DB_SourceLinkTable.Fields[nameof(SourceLinkFields._4)].Value = "";
        DB_SourceLinkTable.Fields[nameof(SourceLinkFields.Art)].Value = eArt;
        DB_SourceLinkTable.Fields[nameof(SourceLinkFields.LfNr)].Value = iLfNr;
        DB_SourceLinkTable.Update();
    }

    public static void SourceLink_DeleteAllPF(int persInArb, int iSKennz)
    {
        IRecordset dB_SourceLinkTable = DB_SourceLinkTable;
        dB_SourceLinkTable.Index = nameof(SourceLinkIndex.Tab);
        dB_SourceLinkTable.Seek("=", iSKennz, persInArb);
        while (!dB_SourceLinkTable.NoMatch
            && !dB_SourceLinkTable.EOF
            && dB_SourceLinkTable.Fields[nameof(SourceLinkFields._1)].AsInt() == iSKennz
            && dB_SourceLinkTable.Fields[nameof(SourceLinkFields._2)].AsInt() == persInArb)
        {
            dB_SourceLinkTable.Delete();
            dB_SourceLinkTable.MoveNext();
        }
    }

    public static void SourceLink_DeleteAllEvLk(int iLink, EEventArt eArt, int iLfNr)
    {
        DB_SourceLinkTable.Index = nameof(SourceLinkIndex.Tab22);
        DB_SourceLinkTable.Seek("=", 3, iLink, eArt, iLfNr);
        while (!DB_SourceLinkTable.NoMatch
            && !DB_SourceLinkTable.EOF
            && DB_SourceLinkTable.Fields[nameof(SourceLinkFields._1)].AsInt() == 3
            && DB_SourceLinkTable.Fields[nameof(SourceLinkFields._2)].AsInt() == iLink
            && DB_SourceLinkTable.Fields[nameof(SourceLinkFields.Art)].AsEnum<EEventArt>() == eArt
            && DB_SourceLinkTable.Fields[nameof(SourceLinkFields.LfNr)].AsInt() == iLfNr)
        {
            DB_SourceLinkTable.Delete();
            DB_SourceLinkTable.MoveNext();
        }
    }

    public static void SourceLink_DeleteAllWhere(int iLinkNr, int iSKennz, Predicate<EEventArt> pWhere)
    {
        DB_SourceLinkTable.Index = "Tab";
        DB_SourceLinkTable.Seek("=", iSKennz, iLinkNr);
        while (!DB_SourceLinkTable.NoMatch
            && !DB_SourceLinkTable.EOF
            && DB_SourceLinkTable.Fields[nameof(SourceLinkFields._1)].AsInt() == iSKennz
            && DB_SourceLinkTable.Fields[nameof(SourceLinkFields._2)].AsInt() == iLinkNr)
        {
            if (pWhere(DB_SourceLinkTable.Fields[nameof(SourceLinkFields.Art)].AsEnum<EEventArt>()))
                DB_SourceLinkTable.Delete();
            DB_SourceLinkTable.MoveNext();
        }
    }

    public static void SourceLink_DeleteAllWhere(Predicate<CSourceLinkData> predicate)
    {
        DB_SourceLinkTable.Index = "Tab";
        DB_SourceLinkTable.MoveFirst();
        while (!DB_SourceLinkTable.EOF)
        {
            if (predicate(new CSourceLinkData(DB_SourceLinkTable)))
                DB_SourceLinkTable.Delete();
            DB_SourceLinkTable.MoveNext();
        }
    }

    public static bool SourceLink_Exists(int v, int nr, EEventArt ubg, short lfNR)
    {
        DB_SourceLinkTable.Index = "Tab22";
        DB_SourceLinkTable.Seek("=", v, nr, ubg, lfNR);
        return !DB_SourceLinkTable.NoMatch;
    }

    #endregion

    public static void Repository_AppendRaw(int num5, string[] kont1)
    {
        IRecordset dB_RepoTable = DB_RepoTable;
        dB_RepoTable.AddNew();
        dB_RepoTable.Fields[nameof(RepoFields.Nr)].Value = num5;
        dB_RepoTable.Fields[nameof(RepoFields.Name)].Value = kont1[0].Trim();
        dB_RepoTable.Fields[nameof(RepoFields.Strasse)].Value = kont1[1].Trim();
        dB_RepoTable.Fields[nameof(RepoFields.Ort)].Value = kont1[2].Trim();
        dB_RepoTable.Fields[nameof(RepoFields.PLZ)].Value = kont1[3].Trim();
        dB_RepoTable.Fields[nameof(RepoFields.Fon)].Value = kont1[4].Trim();
        dB_RepoTable.Fields[nameof(RepoFields.Mail)].Value = kont1[5].Trim();
        dB_RepoTable.Fields[nameof(RepoFields.Http)].Value = kont1[6].Trim();
        dB_RepoTable.Fields[nameof(RepoFields.Bem)].Value = kont1[7].Trim();
        dB_RepoTable.Fields[nameof(RepoFields.Suchname)].Value = (kont1[0] + " " + kont1[2]).Trim();
        dB_RepoTable.Update();
    }

    #region Picture Methods
    public static bool Picture_DeleteAll(int persInArb, char pPKennz)
    {
        DB_PictureTable.Index = nameof(PictureIndex.PerKenn);
        DB_PictureTable.Seek("=", pPKennz, persInArb);
        bool result = !DB_PictureTable.NoMatch;
        while (!DB_PictureTable.EOF
                 && !DB_PictureTable.NoMatch
                 && !(DB_PictureTable.Fields[nameof(PictureFields.ZuNr)].AsInt() != persInArb)
                 && !(DB_PictureTable.Fields[nameof(PictureFields.Kennz)].AsString() != $"{pPKennz}"))
        {
            DB_PictureTable.Delete();
            DB_PictureTable.MoveNext();
            //=================
        }
        return result;
    }

    public static bool Picture_Exists(int iPersonNr, char ePKennz)
    {
        DB_PictureTable.Index = nameof(PictureIndex.PerKenn);
        DB_PictureTable.Seek("=", ePKennz, iPersonNr);
        return !DB_PictureTable.NoMatch;
    }

    public static bool Picture_PersonDataExist(int iPersonNr, PersonFields eField)
    {
        DB_PictureTable.Index = nameof(PictureIndex.Nr);
        DB_PersonTable.Seek("=", iPersonNr);
        return string.IsNullOrWhiteSpace(DB_PersonTable.Fields[$"{eField}"].AsString());
    }



    #endregion

}
