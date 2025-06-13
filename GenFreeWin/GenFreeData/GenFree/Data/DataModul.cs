//using DAO;
using BaseLib.Helper;
using GenFree.Model;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Interfaces.Sys;
using GenFree.Sys;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GenFree.Data;


public static partial class DataModul
{
    public static ILink Link { get; } = new CLink(() => DB_LinkTable!); // new IoC.GetReqiredService(ILink);
    public static IEvent Event { get; } = new CEvent(() => DB_EventTable!); // new IoC.GetReqiredService(IEvent);
    public static IPlace Place { get; } = new CPlace(() => DB_PlaceTable!); // new IoC.GetReqiredService(IPlace);
    public static IPerson Person { get; } = new CPerson(() => DB_PersonTable!, new CSysTime()); // new IoC.GetReqiredService(IPerson);
    public static IFamily Family { get; } = new CFamily(() => DB_FamilyTable!, new CSysTime()); // new IoC.GetReqiredService(IFamily);
    public static INames Names { get; } = new CNames(() => DB_NameTable!); // new IoC.GetReqiredService(INames);
    public static IWitness Witness { get; } = new CWitness(() => DB_WitnessTable!);
    public static IOFB OFB { get; } = new COFB(() => DB_OFBTable!); // new IoC.GetReqiredService(IOFB);
    public static ISourceLink SourceLink { get; } = new CSourceLink(() => DB_SourceLinkTable!); // new IoC.GetReqiredService(ISourceLink);
    public static IRepository Repositories { get; } = new CRepository(() => DB_RepoTable!); // new IoC.GetReqiredService(IRepository); 
  
    public static IWB_Frau WB_Frau { get; } = new CWB_Frau(() => WB_FrauTable!); // new IoC.GetReqiredService(IWB_Frau);
    public static INB_Person NB_Person { get; } = new CNB_Person(() => NB_PersonTable!, Link_MoveAllPaten_ToNBWitn); // new IoC.GetReqiredService(INB_Person);
    public static INB_Family NB_Family { get; } = new CNB_Family(() => NB_FamilyTable!); // new IoC.GetReqiredService(INB_Family);
    public static INB_Frau NB_Frau{ get; } = new CNB_Frau(() => NB_Frau1Table!); // new IoC.GetReqiredService(INB_Family);
    public static INB_Ahnen NB_Ahnen{ get; } = new CNB_Ahnen(() => NB_Ahn1Table!); // new IoC.GetReqiredService(INB_Family);
    public static ICitationData CitationData { get; } = new CCitationData(DB_SourceLinkTable,true); // new IoC.GetReqiredService(ICitationData);

    public static object[] Mandanten = new string[0];

    public static IDBWorkSpace wrkDefault;
    public static IDatabase MandDB { get; set; }
    public static IDatabase DOSB { get; set; }
    public static IDatabase DSB { get; set; }
    public static IDatabase TempDB { get; set; }
    public static IDatabase TempSort_DB { get; set; }
    public static IDatabase DbsNew { get; set; }
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
    public static IRecordset DB_HGATable;
    public static IRecordset DB_GbeTable;
    public static IRecordset DB_OFBTable;
    public static IRecordset DB_WDTable;
    // Temp-DB
    public static IRecordset DT_AncesterTable;
    public static IRecordset DT_RelgionTable;
    public static IRecordset DT_DescendentTable;
    public static IRecordset DSB_SearchTable;
    public static IRecordset DT_KindAhnTable;
    public static IRecordset OrtindTable;
    public static IRecordset DOSB_OrtSTable;
    public static IRecordset Osy;
    public static IRecordset DB_LeerTable ;
    // NB-DB
    public static IRecordset? NB_AhnTable;
    public static IRecordset? NB_Ahn1Table;
    public static IRecordset? NB_Ahn2Table;
    public static IRecordset? NB_FrauTable;
    public static IRecordset? NB_Frau1Table;
    public static IRecordset? NB_Frau2Table;
    public static IRecordset? NB_SourceTable;
    public static IRecordset? NB_OrtTable;
    public static IRecordset? NB_PersonTable;
    public static IRecordset? NB_FamilyTable;
    public static IRecordset? NB_PictureTable;
    public static IRecordset? NB_WitnessTable;
    public static IRecordset? NB_SperrPersTable;
    public static IRecordset? NB_SperrFamsTable;
    public static IRecordset? NB_BemTable;
    public static IRecordset? NB_TVerkTable;
    public static IRecordset? NB_SurTable;
    public static IRecordset? NB_Zeu2Table;
    public static IRecordset? NB_TexteTable { get; set; }

    //WB-DB
    public static IRecordset? WB_FrauTable;

    public static IRecordset BildTab;
    public static IRecordset RechDB_Frauen1;
    // DB-Engine
    public static IDBEngine DAODBEngine_definst;
    /// <summary>
    /// Opens a database and returns an <see cref="IDatabase"/> instance representing the opened database.
    /// </summary>
    /// <param name="sPath">The name or path of the database to open. This cannot be null or empty.</param>
    /// <param name="xExclusive">A boolean value indicating whether the database should be opened in exclusive mode.  <see langword="true"/> to
    /// open the database exclusively; otherwise, <see langword="false"/>.</param>
    /// <param name="xReadOnly">A boolean value indicating whether the database should be opened in read-only mode.  <see langword="true"/> to
    /// open the database as read-only; otherwise, <see langword="false"/>.</param>
    /// <param name="sAdditional">An optional connection string or additional parameters for opening the database.  This can be null or empty if
    /// no additional parameters are required.</param>
    /// <returns>An <see cref="IDatabase"/> instance representing the opened database.</returns>
    public static IDatabase OpenDatabase(string sPath, bool xExclusive, bool xReadOnly, string sAdditional="")
    => DAODBEngine_definst.OpenDatabase(sPath, xExclusive, xReadOnly, sAdditional);
    /// <summary>
    /// Opens multiple databases in read-only mode and initializes recordsets for data access.
    /// </summary>
    /// <remarks>This method opens the specified main database and several related databases in the same
    /// directory. It initializes recordsets for specific tables within these databases to facilitate data access. The
    /// databases are opened in read-only mode to ensure no modifications are made.  If the directory of the provided
    /// <paramref name="Mandant"/> cannot be determined, an empty string is used as the directory path. Any previously
    /// opened databases or recordsets are closed before opening new ones.</remarks>
    /// <param name="Mandant">The file path of the main database to open. This parameter cannot be null or empty.</param>
    public static void DataOpenRO(string Mandant)
    {
        string Verz = Path.GetDirectoryName(Mandant) ?? "";
        var dbEng = DAODBEngine_definst;
        MandDB?.Close();
        TempDB?.Close();
        DOSB?.Close();
        DSB?.Close();
        wrkDefault = dbEng.Workspaces[0];
        MandDB = dbEng.OpenDatabase(Mandant.ToUpper(), false, true, "");
        TempDB = dbEng.OpenDatabase(Path.Combine(Verz, "Tempo.mdb"), false, true, "");
        DOSB = dbEng.OpenDatabase(Path.Combine(Verz, "Ort1.mdb"), false, true, "");
        DSB = dbEng.OpenDatabase(Path.Combine(Verz, "Such.mdb"), false, true, "");
        DT_DescendentTable = TempDB.OpenRecordset("Nachk", RecordsetTypeEnum.dbOpenTable);
        DSB_SearchTable = DSB.OpenRecordset("Such", RecordsetTypeEnum.dbOpenTable);
        DOSB_OrtSTable = DOSB.OpenRecordset("Ortsuch", RecordsetTypeEnum.dbOpenTable);
        DT_RelgionTable = TempDB.OpenRecordset("Konf", RecordsetTypeEnum.dbOpenTable);
        DT_AncesterTable = TempDB.OpenRecordset("Ahnen1", RecordsetTypeEnum.dbOpenTable);
        DT_KindAhnTable = TempDB.OpenRecordset("Ahnew", RecordsetTypeEnum.dbOpenTable);
        DT_AncesterTable.Index = "PerNr";
    }

    /// <summary>
    /// Opens the specified database and retrieves the total number of persons and families.
    /// </summary>
    /// <remarks>If <paramref name="xIsReadOnly"/> is <see langword="false"/>, the method modifies the file 
    /// attributes of the specified database file and related files before opening the database.</remarks>
    /// <param name="mandantname">The path to the database file to be opened.</param>
    /// <param name="xIsReadOnly">A value indicating whether the database should be opened in read-only mode.  If <see langword="false"/>, file
    /// attributes for certain files will be set to normal.</param>
    /// <param name="SetAttributes">A callback action used to set file attributes. This is invoked for specific files  when <paramref
    /// name="xIsReadOnly"/> is <see langword="false"/>.</param>
    /// <returns>A tuple containing two integers: <list type="number"> <item>The total number of persons in the database.</item>
    /// <item>The total number of families in the database.</item> </list></returns>
    public static (int,int) PeekMandant(string mandantname, bool xIsReadOnly, Action<string,FileAttributes> SetAttributes)
    {
        if (!xIsReadOnly)
        {
            SetAttributes(mandantname, FileAttributes.Normal);
            var path = Path.GetDirectoryName(mandantname) ?? "";
            SetAttributes(Path.Combine(path, "Letzter.DAT"), FileAttributes.Normal);
            SetAttributes(Path.Combine(path, "Such.Dat"), FileAttributes.Normal);
        }
        MandDB = OpenDatabase(mandantname, false, xIsReadOnly, "");
        DB_PersonTable = MandDB.OpenRecordset(nameof(dbTables.Personen), RecordsetTypeEnum.dbOpenTable);
        DB_FamilyTable = MandDB.OpenRecordset(nameof(dbTables.Familie), RecordsetTypeEnum.dbOpenTable);
        DB_PersonTable.Index = nameof(PersonIndex.PerNr);
        DB_PersonTable.MoveLast();
        var PersonCount = DB_PersonTable.RecordCount;
        DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
        DB_FamilyTable.MoveLast();
        var FamilyCount = DB_FamilyTable.RecordCount;
        return (PersonCount, FamilyCount);
    }
    /// <summary>
    /// Opens and initializes multiple database connections and recordsets for the specified data source.
    /// </summary>
    /// <remarks>This method establishes connections to several databases and prepares recordsets for use in
    /// subsequent operations.  If the specified database file or any related database files are missing, the method
    /// will not create new databases  but will skip opening those files. The method also ensures that any previously
    /// opened connections are closed before  initializing new ones.  The method assumes that certain auxiliary database
    /// files (e.g., "Tempo.mdb", "Ort1.mdb", "Such.mdb") are located  in the same directory as the primary database. If
    /// these files are present, they will be opened; otherwise, they  will be skipped.  The caller is responsible for
    /// ensuring that the specified file paths are valid and accessible. Additionally, the  method initializes several
    /// recordsets for specific tables within the databases, which are expected to exist.  If any required table is
    /// missing, an exception may be thrown during recordset initialization.</remarks>
    /// <param name="Mandant">The file path to the primary database to be opened. This must be a valid file path to an existing database file.</param>
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
        DOSB_OrtSTable = DOSB.OpenRecordset("Ortsuch", RecordsetTypeEnum.dbOpenTable);
        DT_RelgionTable = TempDB.OpenRecordset("Konf", RecordsetTypeEnum.dbOpenTable);
        DT_AncesterTable = TempDB.OpenRecordset("Ahnen1", RecordsetTypeEnum.dbOpenTable);
        DT_KindAhnTable = TempDB.OpenRecordset("Ahnew", RecordsetTypeEnum.dbOpenTable);
   //     DT_AncesterTable.Index = "PerNr";

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
        DB_HGATable = MandDB.OpenRecordset(nameof(dbTables.HGA), RecordsetTypeEnum.dbOpenTable);
        DB_LinkTable = MandDB.OpenRecordset(nameof(dbTables.Tab), RecordsetTypeEnum.dbOpenTable);
        DB_WitnessTable = MandDB.OpenRecordset(nameof(dbTables.Tab2), RecordsetTypeEnum.dbOpenTable);
        DB_LeerTable = MandDB.OpenRecordset(nameof(dbTables.Leer1), RecordsetTypeEnum.dbOpenTable);
        DB_LeerTable.Index = "Leer";
        DB_RepoTable = MandDB.OpenRecordset(nameof(dbTables.Repo), RecordsetTypeEnum.dbOpenTable);
        DB_RepoTab = MandDB.OpenRecordset(nameof(dbTables.RepoTab), RecordsetTypeEnum.dbOpenTable);
        DB_QuTable = MandDB.OpenRecordset(nameof(dbTables.Quellen), RecordsetTypeEnum.dbOpenTable);
        DB_OFBTable = MandDB.OpenRecordset(nameof(dbTables.INDNam), RecordsetTypeEnum.dbOpenTable);

    }
    /// <summary>
    /// Opens a recordset for the "Lingua" table in the specified database.
    /// </summary>
    /// <param name="name">The name or path of the database to open.</param>
    /// <returns>An <see cref="IRecordset"/> object representing the "Lingua" table, with the index set to "PrimaryKey"  and the
    /// cursor positioned at the first record.</returns>
    public static IRecordset OpenLinguaRecordSet(string name)
    {
        TempDB = OpenDatabase(name, false, false, "");
        IRecordset recordset = TempDB.OpenRecordset(nameof(dbTables.Lingua), RecordsetTypeEnum.dbOpenTable);
        recordset.Index = "PrimaryKey";
        recordset.MoveFirst();
        return recordset;
    }

    #region NB-DB Methods
    /// <summary>
    /// Opens the specified NB database and initializes the associated recordsets.
    /// </summary>
    /// <remarks>This method closes any previously opened NB database before opening the specified one.  It
    /// initializes recordsets for the tables <see cref="nbTables.Ahnen1"/>, <see cref="nbTables.Ahnen2"/>,  <see
    /// cref="nbTables.Frauen1"/>, and <see cref="nbTables.Frauen2"/>.  Ensure that the database file exists and is
    /// accessible before calling this method.</remarks>
    /// <param name="name">The name of the database to open. This must be a valid database file name.</param>
    public static void OpenNBDatabase(string name)
    {
        NB?.Close();
        NB = OpenDatabase(name, false, false, "");
        NB_Ahn1Table = NB.OpenRecordset(nameof(nbTables.Ahnen1), RecordsetTypeEnum.dbOpenTable);
        NB_Ahn2Table = NB.OpenRecordset(nameof(nbTables.Ahnen2), RecordsetTypeEnum.dbOpenTable);
        NB_Frau1Table = NB.OpenRecordset(nameof(nbTables.Frauen1), RecordsetTypeEnum.dbOpenTable);
        NB_Frau2Table = NB.OpenRecordset(nameof(nbTables.Frauen2), RecordsetTypeEnum.dbOpenTable);
    }
    /// <summary>
    /// Creates a new temporary NB database and opens it for use.
    /// </summary>
    /// <remarks>This method closes any previously opened NB database before creating and opening a new one.
    /// The temporary database file is created using the <paramref name="persistence"/> instance.</remarks>
    /// <param name="persistence">An implementation of <see cref="IGenPersistence"/> used to create the temporary database file.</param>
    public static void CreateNewNBDatabase(IGenPersistence persistence)
    {
        NB?.Close();
        string name = persistence.CreateTempFilefromInit("GedAus.mdb");
        OpenNBDatabase(name);
    }

    #region NB-SperrPers Methods
    /// <summary>
    /// Determines whether a record exists in the NB_SperrPers table for the specified person ID.
    /// </summary>
    /// <remarks>This method checks for the existence of a record in the NB_SperrPers table by seeking the
    /// specified person ID.</remarks>
    /// <param name="iPers">The ID of the person to check for in the NB_SperrPers table.</param>
    /// <returns><see langword="true"/> if a record exists for the specified person ID; otherwise, <see langword="false"/>.</returns>
    public static bool NB_SperrPers_Exists(int iPers)
    {
        NB_SperrPersTable?.Seek("=", iPers);
        return !NB_SperrPersTable?.NoMatch ?? false;
    }
    /// <summary>
    /// Attempts to add a person identifier to the restricted persons list if it does not already exist.
    /// </summary>
    /// <remarks>This method checks whether the specified person identifier is already present in the
    /// restricted persons list. If it is not present, the identifier is added to the list and the method returns <see
    /// langword="true"/>.  If the identifier already exists, no changes are made and the method returns <see
    /// langword="false"/>.</remarks>
    /// <param name="iPersInArb">The identifier of the person to be added to the restricted persons list.</param>
    /// <returns><see langword="true"/> if the person identifier was successfully added to the restricted persons list; 
    /// otherwise, <see langword="false"/> if the identifier already exists in the list.</returns>
    public static bool NB_SperrPers_AppendC(int iPersInArb)
    {
        bool result = false;
        if (NB_SperrPersTable is not null && (result = !NB_SperrPers_Exists(iPersInArb)))
        {
            NB_SperrPersTable.AddNew();
            NB_SperrPersTable.Fields["Nr"].Value = iPersInArb;
            NB_SperrPersTable.Update();
        }
        return result;
    }
    #endregion

    #region NB-SperrFams Methods
    public static bool NB_SperrFams_Exists(int iFam)
    {
        NB_SperrFamsTable?.Seek("=", iFam);
        var xExist = !NB_SperrFamsTable?.NoMatch ?? false;
        return xExist;
    }

    public static bool NB_SperrFams_AppendC(int iFam)
    {
        bool result = false;
        if (NB_SperrFamsTable is not null && (result = !NB_SperrFams_Exists(iFam)))
        {
            NB_SperrFamsTable.AddNew();
            NB_SperrFamsTable.Fields["Nr"].Value = iFam;
            NB_SperrFamsTable.Update();
        }
        return result;
    }
    #endregion

    public static void Link_MoveAllPaten_ToNBWitn(int persInArb)
    {
        var b = 1;
        if (NB_WitnessTable is not null)
            foreach (var cLink in Link.ReadAllFams(persInArb, ELinkKennz.lkGodparent))
            {
                NB_WitnessTable.AddNew();
                NB_WitnessTable.Fields["Person"].Value = cLink.iPersNr;
                NB_WitnessTable.Update();
                b++;
            }
    }

    public static void BemTable_AppendRaw(int iNr, string sPF, string sArt, string sBem, object iLfNr)
    {
        if (NB_OrtTable is IRecordset nB_BemTable)
        {
            nB_BemTable.AddNew();
            nB_BemTable.Fields["Nr"].Value = iNr;
            nB_BemTable.Fields["PF"].Value = sPF;
            nB_BemTable.Fields["Art"].Value = sArt;
            nB_BemTable.Fields["BemText"].Value = sBem;
            nB_BemTable.Fields["LFnr"].Value = iLfNr;
            nB_BemTable.Update();
        }
    }
    #endregion

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
        NB_FrauTable = NB.OpenRecordset($"{dbTables.Frauen}", RecordsetTypeEnum.dbOpenTable);
        OrtindTable = NB.OpenRecordset("Ortind", RecordsetTypeEnum.dbOpenTable);
        action?.Invoke();
        if (!nbLeaveOpen)
            NB.Close();
    }

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
        WB.TryExecute($"ALTER TABLE {dbTables.Frauen} ADD COLUMN Name Text(50);");
        WB.TryExecute($"CREATE INDEX Name ON {dbTables.Frauen} ([Name]);");
        WB_FrauTable = WB.OpenRecordset($"{dbTables.Frauen}", RecordsetTypeEnum.dbOpenTable);
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



    #region Names Methods
    public static void Namen_RemovePerson(int persInArb)
    {
        var dB_NameTable = DB_NameTable;
        dB_NameTable.Index = nameof(NameIndex.PNamen);
        dB_NameTable.Seek("=", persInArb);
        if (!dB_NameTable.NoMatch)
        {
            int num5 = dB_NameTable.RecordCount - 1;
            var M1_Iter = 1;
            while (M1_Iter <= num5
                && !dB_NameTable.NoMatch
                && !dB_NameTable.EOF
                && !dB_NameTable.NoMatch
                && !(dB_NameTable.Fields[NameFields.PersNr].AsInt().AsInt() != persInArb))
            {
                dB_NameTable.Delete();
                dB_NameTable.MoveNext();
                M1_Iter++;
            }
        }
    }

    #endregion

    #region Witness Methods
    public static void Witness_DeleteAllF(int persInArb)
    {
        var dB_WitnessTable = DB_WitnessTable;
        dB_WitnessTable.Index = nameof(WitnessIndex.FamSu);
        dB_WitnessTable.Seek("=", persInArb, "10");
        if (!dB_WitnessTable.NoMatch)
        {
            while (!dB_WitnessTable.EOF)
            {
                if (dB_WitnessTable.NoMatch)
                {
                    _ = Interaction.MsgBox("F35");
                    Debugger.Break();
                }
                if (dB_WitnessTable.Fields[WitnessFields.FamNr].AsInt() != persInArb
                        || dB_WitnessTable.Fields[WitnessFields.Kennz].AsInt() != 10)
                {
                    break;
                }
                if (dB_WitnessTable.Fields[WitnessFields.Art].AsInt()< 500)
                {
                    dB_WitnessTable.Delete();
                }
                dB_WitnessTable.MoveNext();
            }
        }
    }
    public static void Witness_DeleteAllE(int persInArb)
    {
        var dB_WitnessTable = DB_WitnessTable;
        dB_WitnessTable.Index = nameof(WitnessIndex.ElSu);
        dB_WitnessTable.Seek("=", persInArb, "10");
        if (!dB_WitnessTable.NoMatch)
        {
            while (!dB_WitnessTable.EOF
                && !dB_WitnessTable.EOF)
            {
                if (dB_WitnessTable.NoMatch)
                {
                    _ = Interaction.MsgBox("F35");
                    Debugger.Break();
                }
                if (dB_WitnessTable.Fields[WitnessFields.PerNr].AsInt() == persInArb
                && dB_WitnessTable.Fields[WitnessFields.Kennz].AsInt() == 10)
                {
                    break;
                }
                dB_WitnessTable.Delete();
                dB_WitnessTable.MoveNext();
            }
        }
    }
    public static void Witness_CopyToNB(int iNr, bool xAR)
    {
        var num5 = 1;
        if (NB_WitnessTable != null)
        foreach (var cWitness in Witness.ReadAllFams(iNr, 10))
        {
            if (xAR ^ (cWitness.eArt <= EEventArt.eA_499))
            {
                NB_WitnessTable.AddNew();
                NB_WitnessTable.Fields["Person"].Value = cWitness.iPers;
                NB_WitnessTable.Update();
            }
            num5++;
        }
    }
    public static void Witness_ChangeEventArt(int famInArb, EEventArt eArtOld, EEventArt eArtNew)
    {
        var dB_WitnessTable = DB_WitnessTable;
        dB_WitnessTable.Index = nameof(WitnessIndex.FamSu);
        dB_WitnessTable.Seek("=", famInArb, "10");
        while (!dB_WitnessTable.EOF
            && !dB_WitnessTable.NoMatch
            && !(dB_WitnessTable.Fields[WitnessFields.FamNr].AsInt() != famInArb))
        {
            if (dB_WitnessTable.Fields[WitnessFields.Art].AsEnum<EEventArt>() == eArtOld)
            {
                dB_WitnessTable.Edit();
                dB_WitnessTable.Fields[WitnessFields.Art].Value = eArtNew;
                dB_WitnessTable.Update();
            }
            dB_WitnessTable.MoveNext();
        }
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
            sText = DB_TexteTable.Fields[TexteFields.Txt].AsString();
            if (DB_TexteTable.Fields[TexteFields.Leitname].Value is string s)
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

    public static string Texte_GetLeitName(string sName,string sSurnames_Text, string Kont3 = "")
    {
        string sLeitName = "";

        IRecordset dB_TexteTable = DB_TexteTable;
        dB_TexteTable.Index = nameof(TexteIndex.ITexte);
        dB_TexteTable.Seek("=", sSurnames_Text.Trim(), "N");
        if (!dB_TexteTable.NoMatch)
        {

            sLeitName = dB_TexteTable.Fields[TexteFields.Leitname].AsString().Trim() == ""
                ? sName
                : (dB_TexteTable.Fields[TexteFields.Leitname].AsString() + "," + Kont3.Trim()).Left(50);
        }

        return sLeitName;
    }
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
            if ((fld = dB_TexteTable.Fields[TexteFields.Leitname]).Value is string s
                && s != Leitn)
            {
                dB_TexteTable.Edit();
                fld.Value = Leitn;
            }
            if ((fld = dB_TexteTable.Fields[TexteFields.Bem]).Value is string s2
                && s2 != " ")
            {
                dB_TexteTable.Edit();
                fld.Value = Leitn;
            }
            if (dB_TexteTable.EditMode != 0)
                dB_TexteTable.Update();
            Satz = dB_TexteTable.Fields[TexteFields.TxNr].AsInt();
            return;
        }
        dB_TexteTable.Index = nameof(TexteIndex.TxNr);
        if (dB_TexteTable.RecordCount == 0)
            Satz = 1;
        else
        {
            dB_TexteTable.MoveLast();
            Satz = dB_TexteTable.Fields[TexteFields.TxNr].AsInt() + 1;
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
        dB_TexteTable.Fields[TexteFields.Kennz].Value = Kennz;
        dB_TexteTable.Fields[TexteFields.Txt].Value = Wort;
        dB_TexteTable.Fields[TexteFields.Bem].Value = sBem;
        dB_TexteTable.Fields[TexteFields.Leitname].Value = Leitn;
        dB_TexteTable.Fields[TexteFields.TxNr].Value = Satz;
        dB_TexteTable.Update();
    }

    public static bool RemoveOrphanText(out ETextKennz value)
    {
        bool flag = false;
        int TextNr = DB_TexteTable.Fields[TexteFields.TxNr].AsInt();
        value = DB_TexteTable.Fields[TexteFields.Kennz].AsEnum<ETextKennz>();
        switch (value)
        {
            case ETextKennz.tkName or ETextKennz.F_ or ETextKennz.V_ or ETextKennz.A_ or ETextKennz.B_ or ETextKennz.C_ or ETextKennz.D_ or ETextKennz.U_:
                if (!Names.ExistText(TextNr))
                {
                    if (!OFB.TextExist(TextNr))
                    {
                        DB_TexteTable.Delete();
                    }
                }
                break;
            case ETextKennz.H_:
                if (!Place.Exists(PlaceIndex.O, TextNr))
                {
                    DB_TexteTable.Delete();
                }
                break;
            case ETextKennz.I_:
                if (!Place.Exists(PlaceIndex.OT, TextNr))
                {
                    DB_TexteTable.Delete();
                }
                break;
            case ETextKennz.J_:
                if (!Place.Exists(PlaceIndex.K, TextNr))
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
                    if (!OFB.TextExist(TextNr))
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

    public static void Convert_OldReligion()
    {
        IRecordset dB_PersonTable1 = DB_PersonTable;
        _ = Interaction.MsgBox("Die Religionseinträge werden jetzt bearbeitet. Vermutlich wurde der Mandant mit einer älteren Programmversion bearbeitet. Dieser Vorgang kann einige Minuten dauern.");
        dB_PersonTable1.Index = nameof(PersonIndex.PerNr);
        dB_PersonTable1.MoveLast();
        int num5 = dB_PersonTable1.Fields[PersonFields.PersNr].AsInt();
        dB_PersonTable1.MoveFirst();
        int num6 = num5;
        var M1_Iter = 1;
        while (M1_Iter <= num6)
        {
            dB_PersonTable1.Edit();
            if (Strings.Trim(dB_PersonTable1.Fields[PersonFields.Konv].AsString()) != "")
            {
                var field = dB_PersonTable1.Fields[PersonFields.Konv];
                var Wort = field.Value.AsString();
                Texte_Schreib(Wort, "", ETextKennz.tk7_, out var Satz);
                field.Value = Wort;
                dB_PersonTable1.Fields[PersonFields.religi].Value = Satz;
            }
            else
            {
                dB_PersonTable1.Fields[PersonFields.religi].Value = 0;
            }

            dB_PersonTable1.Update();
            dB_PersonTable1.MoveNext();
            M1_Iter++;
        }

        _ = Interaction.MsgBox("Fertig");
    }
    public static void Pictures_DeletePerson(int persInArb)
    {
        IRecordset dB_PictureTable = DB_PictureTable;
        dB_PictureTable.Index = nameof(PictureIndex.PerKenn);
            dB_PictureTable.Seek("=", "P", persInArb);
            while (!dB_PictureTable.EOF
                && !dB_PictureTable.NoMatch
                && !(dB_PictureTable.Fields[PictureFields.ZuNr].AsInt() != persInArb))
            {
                dB_PictureTable.Delete();
                dB_PictureTable.MoveNext();
                //=================
            }
    }

    public static void Search_DeletePeson(int persInArb)
    {
        IRecordset dSB_SearchTable = DSB_SearchTable;
        dSB_SearchTable.Index = "Nummer";
            dSB_SearchTable.Seek(">=", persInArb);
            if (!dSB_SearchTable.NoMatch)
            {
                if (dSB_SearchTable.Fields["Nummer"].AsInt() == persInArb)
                {
                    dSB_SearchTable.Delete();
                }
                //=================
            }
        
    }

    public static void Events_DeleteAllPersVitEv(int persInArb)
    {
        var eArt = EEventArt.eA_Birth;
        while (eArt <= EEventArt.eA_120)
        {
            Event.DeleteBeSu(eArt, persInArb);
            eArt++;
        }

    }

    public static string Event_GetLabelText(int PersInArb, EEventArt eEvtArt, Func<bool, bool, bool, bool, bool, string> event_PreDisplay)
    {
        if ((eEvtArt != EEventArt.eA_105)
            && Event.ReadData(eEvtArt, PersInArb, out var cEvt))
        {
            string sDate = "";
            string sDate2 = "";
            string sDatumText = "";
            string sPlace = "";
            string sDeathBem = "";
            string sCausalExt = "";
            string sAn;
            short LfNR = 0;
            if (cEvt!.dDatumV != default)
            {
                sDate = cEvt.dDatumV.ToShortDateString();
            }
            sDate = sDate + " " + cEvt.sDatumV_S;

            string sDoW = cEvt.dDatumV.DayOfWeekStr();

            if (cEvt.dDatumB != default)
            {
                sDate2 = "/ " + cEvt.dDatumB.ToShortDateString();
            }
            string sOrt = "";
            if (0 != cEvt.iDatumText)
            {
                sDatumText = " " + cEvt.sDatumText + " ";
            }
            if (eEvtArt == EEventArt.eA_Death && (cEvt.iCausal > 0))
            {
                sAn = cEvt.sAn;
                if (sAn.Trim() == "")
                {
                    sAn = "an";
                }
                else if (sAn.Trim() == "°")
                {
                    sAn = "";
                }
                sCausalExt = $" {sAn}{(string.IsNullOrEmpty(sAn)?"":" ")}{cEvt.sCausal} ";
            }

            if (cEvt.iKBem > 0)
            {
                sDeathBem = $" {cEvt.sKBem.Trim()} ";
                //=================
            }
            else if (cEvt.sDeath == "J")
            {
                sDeathBem = " verstorben ";
            }

            if (cEvt.iOrt > 0)
            {
                if (Place.ReadData(cEvt.iOrt, out var cPlace))
                {
                    sOrt = cPlace!.sOrt;
                    sOrt += $"{(string.IsNullOrEmpty(cPlace.sOrtsteil)?"": " ")}{cPlace.sOrtsteil}";
                    sOrt = $"{(string?)cEvt.sZusatz} {sOrt}";
                }
            }
            if (cEvt.sOrt_S.Trim() != "")
            {
                sOrt = sOrt.TrimEnd() + " " + cEvt.sOrt_S.Trim();
            }
            string sDatB_S = " " + cEvt.sDatumB_S;
            if (cEvt.iPlatz > 0)
            {
                sPlace = " " + cEvt.sPlatz.Trim() + " ";
            }

            string text = event_PreDisplay(
                SourceLink_Exists(3, PersInArb, eEvtArt, LfNR) || cEvt.sBem[3].TrimEnd() != "",
                cEvt.sBem[4].TrimEnd() != "" || Witness.ExistZeug(PersInArb, eEvtArt, LfNR, 10),
                cEvt.sBem[1] != "" || cEvt.sBem[2] != "",
                cEvt.sVChr != "0",
                cEvt.sReg.TrimEnd() != "" );

            return $"{text} {sDoW} {sDate} {sDate2}{sDatB_S}{sDatumText}{sCausalExt}{sDeathBem}{sPlace} {sOrt}".Replace( "  ", " ");
        }
        else
            return "";

    }

    public static void Descendents_DeleteAll(int persInArb)
    {
        IRecordset dT_DescendentTable = DT_DescendentTable;
        dT_DescendentTable.Index = "Pernr";
            dT_DescendentTable.Seek("=", persInArb);
            if (!dT_DescendentTable.NoMatch)
            {
                var M1_Iter = 1;
                while (M1_Iter <= 99
                    && !dT_DescendentTable.EOF
                    && !(dT_DescendentTable.Fields["Pr"].AsInt() != persInArb))
                {
                    dT_DescendentTable.Delete();
                    dT_DescendentTable.MoveNext();
                    M1_Iter++;
                }
            }
        
    }

    public static void Ancestors_DeleteAll(int persInArb)
    {
        IRecordset dT_AncesterTable = DT_AncesterTable;
        dT_AncesterTable.Index = "Pernr";
            dT_AncesterTable.Seek("=", persInArb);
            if (!dT_AncesterTable.NoMatch)
            {
                var M1_Iter = 1;
                while (M1_Iter <= 99
                    && !dT_AncesterTable.EOF
                    && !(dT_AncesterTable.Fields["Pernr"].AsInt() != persInArb))
                {
                    dT_AncesterTable.Delete();
                    dT_AncesterTable.MoveNext();
                    M1_Iter++;
                }
            }
        
    }








    public static void Db_Def(IDatabase Database, Action<string> print)
    {
        print("public static stTableDef[] Def = {");

        foreach (ITableDef tbl in Database.TableDefs())
            if (!tbl.Name?.StartsWith("MSys") ?? false)
            {
                print($"\tnew() {{ Name = \"{tbl.Name}\",");
                print("\t  Fields = new stFieldDef[]{");
                //             new("Ortnr",new[] { "PerNr" }),
                //             new("Ortsu",new[] { "Name" }),

                foreach (FieldDef fld in tbl.Fields)
                {
                    print($"\t\tnew(\"{fld.Name}\", TypeCode.{fld.Type}) " +
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
                            print($"\t\tnew(\"{idx.Name}\", new[] {{ {string.Join(", ", fields)} }} )"
                                + $"{(idx.Unique ? "{ Unique = true }," : "")}"
                                + $"{(idx.IgnoreNulls ? "{ IgnoreNull = true }," : "")}");
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


    /// <summary>
    /// Deletes a family record identified by the specified key and performs an action on a field value before deletion.
    /// </summary>
    /// <remarks>The method searches for a family record using the specified identifier. If the record is
    /// found, the value of the field corresponding to the provided enumeration is passed to the <paramref
    /// name="action"/> callback (if not <see langword="null"/>), and the record is then deleted. If no matching record
    /// is found, the method returns <see langword="false"/> without performing any action.</remarks>
    /// <param name="Fam1">The unique identifier of the family record to delete.</param>
    /// <param name="v">An enumeration value representing the field to retrieve before deletion.</param>
    /// <param name="action">An optional callback action to invoke with the value of the specified field before the record is deleted. Can be
    /// <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the family record was found and deleted; otherwise, <see langword="false"/>.</returns>
    public static bool Family_DeleteA(int Fam1, Enum v, Action<object> action)
    {
        bool xRes;
        IRecordset dB_FamilyTable = DB_FamilyTable;
        dB_FamilyTable.Index = nameof(FamilyIndex.Fam);
        dB_FamilyTable.Seek("=", Fam1);
        if (xRes = !dB_FamilyTable.NoMatch)
        {
            action?.Invoke(dB_FamilyTable.Fields[v.AsFld()].Value);
            dB_FamilyTable.Delete();
        }
        return xRes;
    }

    public static bool Family_CheckParent(int iPerson, string sParDesc, string sExpSex, IRecordset dB_FamilyTable, Action<string> Output)
    {
        if (iPerson > 0)
        {
            if (!Person.Exists(iPerson))
            {
                Output($"    {sParDesc} nicht vorhanden");
                return false;
            }
            else if (dB_FamilyTable.Fields[FamilyFields.ggv].AsInt() != 1 && Person.GetSex(iPerson) != sExpSex)
            {
                Output($"    {sParDesc} hat das falsche Geschlecht");
                return false;
            }
        }
        return true;
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

    public static void SearchTable_Update(int PersInArb, string inputStr, string value, string sName, string surname, string sKenn, string sLeitName, string sAlias, Func<string, string> koelner_Phonetic, Func<string, string> getSoundEx)
    {
        IRecordset dSB_SearchTable = DSB_SearchTable;
        dSB_SearchTable.Index = "Nummer";
        dSB_SearchTable.Seek("=", PersInArb);
        if (!dSB_SearchTable.NoMatch)
        {
            dSB_SearchTable.Edit();
        }
        else
        {
            dSB_SearchTable.AddNew();
        }
        dSB_SearchTable.Fields["Name"].Value = sName;
        dSB_SearchTable.Fields["iKenn"].Value = sKenn;
        dSB_SearchTable.Fields["Alias"].Value = sAlias;
        dSB_SearchTable.Fields["Leit"].Value = sLeitName;
        dSB_SearchTable.Fields["K_Phon"].Value = koelner_Phonetic(surname);
        dSB_SearchTable.Fields["Sound"].Value = getSoundEx(surname);
        dSB_SearchTable.Fields["Datum"].Value = inputStr.AsInt();
        dSB_SearchTable.Fields["Sich"].Value = value;
        dSB_SearchTable.Fields["Nummer"].Value = PersInArb;


        dSB_SearchTable.Update();
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
   
    public static void TTable_RemovePerson(int persInArb, int Param)
    {
        var dB_TTable = DB_SourceLinkTable;
        dB_TTable.Index = "Tab";
        dB_TTable.Seek("=", Param, persInArb);
        if (!dB_TTable.NoMatch)
        {
            while (!dB_TTable.EOF
                && !dB_TTable.NoMatch
                && !
                    (
                        (dB_TTable.Fields["1"].Value.AsInt() != Param) ||
                (dB_TTable.Fields["2"].Value.AsInt() != persInArb)))
            {
                dB_TTable.Delete();
                dB_TTable.MoveNext();
                //=================
            }
        }
    }

    public static void SourceLink_AppendRaw(int iLinkType, int iLink, int num15, EEventArt eArt, int iLfNr)
    {
        DB_SourceLinkTable.AddNew();
        DB_SourceLinkTable.Fields[SourceLinkFields._1.AsFld()].Value = iLinkType;
        DB_SourceLinkTable.Fields[SourceLinkFields._2.AsFld()].Value = iLink;
        DB_SourceLinkTable.Fields[SourceLinkFields._3.AsFld()].Value = num15;
        DB_SourceLinkTable.Fields[SourceLinkFields._4.AsFld()].Value = "";
        DB_SourceLinkTable.Fields[SourceLinkFields.Art.AsFld()].Value = eArt;
        DB_SourceLinkTable.Fields[SourceLinkFields.LfNr.AsFld()].Value = iLfNr;
        DB_SourceLinkTable.Update();
    }

    public static void SourceLink_DeleteAllPF(int persInArb, int iSKennz)
    {
        IRecordset dB_SourceLinkTable = DB_SourceLinkTable;
        dB_SourceLinkTable.Index = nameof(SourceLinkIndex.Tab);
        dB_SourceLinkTable.Seek("=", iSKennz, persInArb);
        while (!dB_SourceLinkTable.NoMatch
            && !dB_SourceLinkTable.EOF
            && dB_SourceLinkTable.Fields[SourceLinkFields._1].AsInt() == iSKennz
            && dB_SourceLinkTable.Fields[SourceLinkFields._2].AsInt() == persInArb)
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
            && DB_SourceLinkTable.Fields[SourceLinkFields._1].AsInt() == 3
            && DB_SourceLinkTable.Fields[SourceLinkFields._2].AsInt() == iLink
            && DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsEnum<EEventArt>() == eArt
            && DB_SourceLinkTable.Fields[SourceLinkFields.LfNr].AsInt() == iLfNr)
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
            && DB_SourceLinkTable.Fields[SourceLinkFields._1].AsInt() == iSKennz
            && DB_SourceLinkTable.Fields[SourceLinkFields._2].AsInt() == iLinkNr)
        {
            if (pWhere(DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsEnum<EEventArt>()))
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

    public static void SourceLink_ChangeEvent(int famInArb, EEventArt eArtOld, EEventArt eArtNew)
    {
        var dB_TTable = DataModul.DB_SourceLinkTable;
        dB_TTable.Index = "Tab";
        dB_TTable.Seek("=", 3, famInArb);
        while (!dB_TTable.EOF
            && !dB_TTable.NoMatch
            && !(
                dB_TTable.Fields[0].AsInt() != 3 
                ||dB_TTable.Fields[1].AsInt() > famInArb))
        {
            if (dB_TTable.Fields["Art"].AsEnum<EEventArt>() == eArtOld)
            {
                dB_TTable.Edit();
                dB_TTable.Fields["Art"].Value = eArtNew;
                dB_TTable.Update();
            }
            dB_TTable.MoveNext();
        }
    }

    #endregion
    /// <summary>
    /// Appends a new record to the repository with the specified identifier and data fields.
    /// </summary>
    /// <remarks>The method trims whitespace from each field in <paramref name="lsData"/> before storing it in
    /// the repository. Additionally, a "search name" is generated by concatenating the first and third elements of
    /// <paramref name="lsData"/>  (name and city) with a space, and this value is also stored in the
    /// repository.</remarks>
    /// <param name="iNr">The unique identifier for the new record.</param>
    /// <param name="lsData">A list of strings containing the data fields for the new record. The list must contain at least 8 elements:
    /// <list type="number"> <item><description>The name of the entity.</description></item> <item><description>The
    /// street address.</description></item> <item><description>The city or locality.</description></item>
    /// <item><description>The postal code.</description></item> <item><description>The phone
    /// number.</description></item> <item><description>The email address.</description></item> <item><description>The
    /// website URL.</description></item> <item><description>Additional remarks or comments.</description></item>
    /// </list></param>
    public static void Repository_AppendRaw(int iNr, IList<string> lsData)
    {
        IRecordset dB_RepoTable = DB_RepoTable;
        dB_RepoTable.AddNew();
        dB_RepoTable.Fields[RepoFields.Nr].Value = iNr;
        dB_RepoTable.Fields[RepoFields.Name].Value = lsData[0].Trim();
        dB_RepoTable.Fields[RepoFields.Strasse].Value = lsData[1].Trim();
        dB_RepoTable.Fields[RepoFields.Ort].Value = lsData[2].Trim();
        dB_RepoTable.Fields[RepoFields.PLZ].Value = lsData[3].Trim();
        dB_RepoTable.Fields[RepoFields.Fon].Value = lsData[4].Trim();
        dB_RepoTable.Fields[RepoFields.Mail].Value = lsData[5].Trim();
        dB_RepoTable.Fields[RepoFields.Http].Value = lsData[6].Trim();
        dB_RepoTable.Fields[RepoFields.Bem].Value = lsData[7].Trim();
        dB_RepoTable.Fields[RepoFields.Suchname].Value = (lsData[0] + " " + lsData[2]).Trim();
        dB_RepoTable.Update();
    }

    #region Picture Methods
    /// <summary>
    /// Determines whether a picture exists for the specified family number and identifier.
    /// </summary>
    /// <remarks>This method performs a database lookup to check for the existence of a picture based on the
    /// provided parameters.</remarks>
    /// <param name="iPerFamNr">The family number to search for.</param>
    /// <param name="ePKennz">The identifier associated with the picture.</param>
    /// <returns><see langword="true"/> if a picture exists for the specified family number and identifier; otherwise, <see
    /// langword="false"/>.</returns>
    public static bool Picture_Exists(int iPerFamNr, char ePKennz)
    {
        DB_PictureTable.Index = nameof(PictureIndex.PerKenn);
        DB_PictureTable.Seek("=", ePKennz, iPerFamNr);
        return !DB_PictureTable.NoMatch;
    }
    /// <summary>
    /// Determines whether a picture associated with the specified family identifier exists in the database.
    /// </summary>
    /// <remarks>This method queries the database to check for the existence of a picture record with a
    /// specific family identifier. Ensure that the database connection is properly initialized before calling this
    /// method.</remarks>
    /// <param name="FamInArb">The identifier of the family to check for an associated picture.</param>
    /// <returns><see langword="true"/> if a picture associated with the specified family identifier exists; otherwise, <see
    /// langword="false"/>.</returns>
    public static bool Picture_ExistsFam(int FamInArb)
    {
        var sPKennz = "F";
        BildTab = MandDB.OpenRecordset($"select * from {dbTables.Bilder} Where {dbTables.Bilder}.{PictureFields.Kennz}='{sPKennz}'");
        BildTab.FindFirst($"{dbTables.Bilder}.{PictureFields.ZuNr} = {FamInArb}");
        bool noMatch = BildTab.NoMatch;
        return !noMatch;
    }
    /// <summary>
    /// Determines whether the specified person's data for the given field exists in the database.
    /// </summary>
    /// <remarks>This method checks if the specified field for the given person is empty or consists only of
    /// whitespace.</remarks>
    /// <param name="iPersonNr">The unique identifier of the person.</param>
    /// <param name="eField">The field of the person's data to check for existence.</param>
    /// <returns><see langword="true"/> if the specified field for the person contains data; otherwise, <see langword="false"/>.</returns>
    public static bool Picture_PersonDataExist(int iPersonNr, PersonFields eField)
    {
        DB_PictureTable.Index = nameof(PictureIndex.Nr);
        DB_PersonTable.Seek("=", iPersonNr);
        return string.IsNullOrWhiteSpace(DB_PersonTable.Fields[$"{eField}"].AsString());
    }
    /// <summary>
    /// Deletes all picture records associated with the specified person and identifier.
    /// </summary>
    /// <remarks>This method searches for picture records in the database that match the specified person
    /// identifier (<paramref name="persInArb"/>) and picture identifier (<paramref name="pPKennz"/>). If matching
    /// records are found, they are deleted. The method returns a boolean indicating whether any matching records were
    /// found.</remarks>
    /// <param name="persInArb">The unique identifier of the person whose picture records are to be deleted.</param>
    /// <param name="pPKennz">A character representing the specific identifier for the picture records to delete.</param>
    /// <returns><see langword="true"/> if one or more picture records matching the specified criteria were found and deleted;
    /// otherwise, <see langword="false"/>.</returns>
    public static bool Picture_DeleteAll(int persInArb, char pPKennz)
    {
        DB_PictureTable.Index = nameof(PictureIndex.PerKenn);
        DB_PictureTable.Seek("=", pPKennz, persInArb);
        bool result = !DB_PictureTable.NoMatch;
        while (!DB_PictureTable.EOF
                 && !DB_PictureTable.NoMatch
                 && !(DB_PictureTable.Fields[PictureFields.ZuNr].AsInt() != persInArb)
                 && !(DB_PictureTable.Fields[PictureFields.Kennz].AsString() != $"{pPKennz}"))
        {
            DB_PictureTable.Delete();
            DB_PictureTable.MoveNext();
            //=================
        }
        return result;
    }
    #endregion

    /// <summary>
    /// Searches for a person in the specified index based on the given search text and returns the person's unique
    /// identifier.
    /// </summary>
    /// <remarks>The search operation is performed using the specified index and search text. If no match is
    /// found, the method returns the fallback identifier provided in <paramref name="persInArb"/>.</remarks>
    /// <param name="Index">The index to search within. This determines the scope of the search.</param>
    /// <param name="sText">The search text used to locate the person. Leading and trailing whitespace will be trimmed. If empty, a default
    /// value of <see langword="&quot;"/> is used.</param>
    /// <param name="persInArb">The fallback person identifier to return if no match is found.</param>
    /// <param name="rev">A value indicating whether to perform a reverse search. If <see langword="true"/>, the search is performed in
    /// reverse order; otherwise, it is performed in forward order.</param>
    /// <returns>The unique identifier of the person if a match is found; otherwise, the value of <paramref name="persInArb"/>.</returns>
    public static int Person_DoSearch(PersonIndex Index, string sText, int persInArb, bool rev = false)
    {
        sText = sText.Trim();
        if (sText == "")
        {
            sText = "\"";
        }

        var dB_PersonTable = DB_PersonTable;
        dB_PersonTable.Index = Index.AsString();
        dB_PersonTable.Seek(rev ? "<" : ">", sText, persInArb);
        if (!dB_PersonTable.NoMatch)
        {
            return dB_PersonTable.Fields[PersonFields.PersNr].AsInt();
        }
        else
            return persInArb;
    }
    /// <summary>
    /// Clears and resets specific fields in the person record with the provided values for sex and check status.
    /// </summary>
    /// <remarks>This method modifies the person record by resetting several fields to default or empty values
    /// and updating the sex and check status fields with the provided values. Ensure that the provided  values for
    /// <paramref name="sSex"/> and <paramref name="sCheck"/> are valid and non-null.</remarks>
    /// <param name="sSex">The value to set for the person's sex field. This value cannot be null.</param>
    /// <param name="sCheck">The value to set for the person's check status field. This value cannot be null.</param>
    public static void Person_Clear_EntryRaw(string sSex, string sCheck)
    {
        IRecordset dB_PersonTable = DB_PersonTable;
        dB_PersonTable.Edit();
        dB_PersonTable.Fields[PersonFields.AnlDatum].Value = 0;
        dB_PersonTable.Fields[PersonFields.EditDat].Value = 0;
        dB_PersonTable.Fields[PersonFields.Konv].Value = " ";
        dB_PersonTable.Fields[PersonFields.religi].Value = " ";
        dB_PersonTable.Fields[PersonFields.Sex].Value = sSex;
        dB_PersonTable.Fields[PersonFields.Bem1].Value = " ";
        dB_PersonTable.Fields[PersonFields.Pruefen].Value = sCheck;
        dB_PersonTable.Update();
    }
    public static void Person_Sichlöschloesch(int persInArb)
    {
        Names.DeleteAllP(persInArb);
        SourceLink_DeleteAllPF(persInArb, 1);

        ////Discarded unreachable code: IL_10c5
        //int try0001_dispatch = -1;
        //int num = default;
        //int num2 = default;
        //int num3 = default;
        //int lErl = default;
        ELinkKennz num6 = 1.AsEnum<ELinkKennz>();
        while (num6 <= ELinkKennz.lk9)
        {
            _ = Link.DeleteAllE(persInArb, num6);
            num6++;
        }

        _ = Link.DeleteAllF(persInArb, ELinkKennz.lkGodparent);

        Witness.DeleteAllE(persInArb, 10);

        Witness.DeleteAllF(persInArb, 10);

        Ancestrers_DeleteAll(persInArb);
        Descendent_DeleteAll(persInArb);
        EEventArt num6b = EEventArt.eA_Birth;
        while (num6b <= EEventArt.eA_120)
        {
            Event.DeleteBeSu(num6b, persInArb);
            num6b++;
        }

        num6b = EEventArt.eA_300;
        while (num6b <= EEventArt.eA_302)
        {
            Event.DeleteAll(num6b, persInArb);
            num6b++;
        }

        SearchTab_Delete(persInArb);

        _ = Picture_DeleteAll(persInArb, 'P');

        Person.Delete(persInArb);
    }

    public static void Person_Leertest()
    {
        var dB_PersonTable = DB_PersonTable;
        dB_PersonTable.Index = nameof(PersonIndex.PerNr);
        dB_PersonTable.MoveLast();
        long iMaxPerson = dB_PersonTable.Fields[PersonFields.PersNr].AsLong();
        dB_PersonTable.MoveFirst();

        var dB_FamilyTable = DB_FamilyTable;
        if (dB_FamilyTable.RecordCount <= 0)
        {
            return;
        }

        dB_FamilyTable.Index = nameof(FamilyIndex.Fam);
        dB_FamilyTable.MoveLast();
        long iMaxFamily = dB_FamilyTable.Fields[FamilyFields.FamNr].AsLong();
        dB_FamilyTable.MoveFirst();

        int iFamNrFree = -1;
        checked
        {
            var I = 1;
            while (!dB_FamilyTable.EOF)
            {
                var FamInArb = I;
                if (dB_FamilyTable.Fields[FamilyFields.FamNr].AsInt() != I)
                {
                    if (dB_FamilyTable.Fields[FamilyFields.FamNr].AsInt() >= I
                        || dB_FamilyTable.Fields[FamilyFields.FamNr].AsInt() != iFamNrFree)
                    {
                        if (!Link.ExistFam(FamInArb, [ELinkKennz.lkGodparent, ELinkKennz.lk9]))
                        {
                            Family.AppendRaw(FamInArb, 0, 0, " ");
                        }
                        I++;
                        dB_FamilyTable.MoveNext();
                        continue;
                    }
                    else
                    {
                        I--;
                        dB_FamilyTable.Delete();
                    }
                }
                else
                {
                    iFamNrFree = dB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();
                }
                dB_FamilyTable.MoveNext();
                I++;
            }
        }
    }

    public static int Person_NewPerson(Action frmPerson_Clear,short Schalt)
    {
        IRecordset dB_PersonTable = DB_PersonTable;
        dB_PersonTable.Index = nameof(PersonIndex.PerNr);
        int persInArb = 0;

        if (Schalt != 5)
        {
            frmPerson_Clear?.Invoke();
            DB_WDTable.MoveFirst();
            int iWDTable_Nr = DB_WDTable.Fields["NR"].AsInt();

            if (iWDTable_Nr == 1)
            {
                if (LeerTab_GetEmptyPerson(out persInArb))
                {
                    dB_PersonTable.Seek("=", persInArb);
                    if (dB_PersonTable.NoMatch)
                    {
                        Person_Sichlöschloesch(persInArb);
                    }
                    else
                    {
                        persInArb = 0;
                    }
                }
                else if (persInArb == 0)
                {
                    dB_PersonTable.Index = nameof(PersonIndex.PerNr);
                    dB_PersonTable.MoveLast();
                    persInArb = dB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                    persInArb++;
                }
            }
            else if (dB_PersonTable.RecordCount > 0)
            {
                dB_PersonTable.Index = nameof(PersonIndex.PerNr);
                dB_PersonTable.MoveLast();
                persInArb = dB_PersonTable.Fields[PersonFields.PersNr].AsInt()+ 1;
            }
            else
            {
                persInArb = 1;
            }
            if (persInArb == 0)
            {
                dB_PersonTable.Index = nameof(PersonIndex.PerNr);
                dB_PersonTable.MoveLast();
                persInArb = dB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                persInArb++;
            }
        }
        else
        {
            persInArb = 1;
        }
        return persInArb;
    }

    public static void Person_UpdateOFB(int iPers, bool xChecked)
    {
        IRecordset dB_PersonTable = DB_PersonTable;
        dB_PersonTable.Seek("=", iPers);
        if (!dB_PersonTable.NoMatch)
        {
            throw new ArgumentException($"Person with ID {iPers} does not exist in the database.");
        }
        dB_PersonTable.Edit();
        dB_PersonTable.Fields[PersonFields.EditDat].Value = DateTime.Now.ToString("yyyyMMdd");
        dB_PersonTable.Fields[PersonFields.OFB].Value = xChecked ? "J" : (object)"N";
        dB_PersonTable.Update();
    }

    #region LeetTab-Methods
    public static bool LeerTab_GetEmptyPerson(out int persInArb)
    {
        persInArb = 0;
        IRecordset db_leerTable = DB_LeerTable;
        db_leerTable.Index = "Leer";
        db_leerTable.Seek("=", "P");
        bool xLeerPNoMatch = db_leerTable.NoMatch;
        int Leer_iNr = db_leerTable.Fields["Nr"].AsInt();
        if (!xLeerPNoMatch)
        {
            persInArb = Leer_iNr;
            db_leerTable.Delete();
        }

        return !xLeerPNoMatch;
    }

    public static bool LeerTab_GetEmptyFam(out int famInArb)
    {
        famInArb = 0;
        IRecordset dB_LeerTable = DB_LeerTable;
        dB_LeerTable.Index = "Leer";
        dB_LeerTable.Seek("=", "F");
        var xMatch = !dB_LeerTable.NoMatch;
        if (xMatch)
        {
            famInArb = dB_LeerTable.Fields["Nr"].AsInt();
            dB_LeerTable.Delete();
        }

        return xMatch;
    }

    public static void LeerTab_AddRaw(int persInArb, string sArt)
    {
        IRecordset dB_LeerTable = DB_LeerTable;
        dB_LeerTable.AddNew();
        dB_LeerTable.Fields["Nr"].Value = persInArb;
        dB_LeerTable.Fields["Art"].Value = sArt;
        dB_LeerTable.Update();
    }

    #endregion
    

}
