using GenFree.Helper;
using System;
using System.Linq;
using GenFree.Interfaces.Data;

namespace GenFree.Data
{
    public static partial class DataModul
    {

        public struct stFieldDef
        {
            public string Name;
            public TypeCode Typ;
            public int Length = 0;
            public bool xNull = true;
            public stFieldDef(string aName, TypeCode aType)
            { Name = aName; Typ = aType; }
            public stFieldDef(Enum eFld, TypeCode aType)
            : this($"{eFld}", aType) { }

        }
        public struct stIndex
        {
            public string Name;
            public string[] Fields;
            public bool Unique = false;
            public bool IgnoreNull = false;
            public stIndex(string aName, string[] aFields)
            { Name = aName; Fields = aFields; }
            public stIndex(Enum eFld, Enum[] eFlds)
            : this($"{eFld}", eFlds.ToStrings((e) => $"{e}").ToArray()) { }
        }
        public struct stTableDef
        {
            public string Name;
            public string? IntName;
            //   public List<stFieldDef> Fields;
            public stFieldDef[] Fields;
            public stIndex[] Indexes;
            public bool xDrop;
        }

        public static stTableDef[] DOSBDef = [ new() {
             Name= "Ortsuch",
             Fields = [
                new("Name",TypeCode.String) { Length = 100 },
                new("Nr",TypeCode.Int32)],
             Indexes =[
                 new("Ortnr",["Nr"]),
                 new("Ortsu",["Name"])] } ];
        public static stTableDef[] DSBDef = [ new() {
             Name= "Such",
             Fields = [
                new("Name",TypeCode.String) { Length = 50 },
                new("K_Phon",TypeCode.String){ Length = 60 },
                new("Leit",TypeCode.String){ Length = 60 },
                new("Alias",TypeCode.String){ Length = 60 },
                new("Sound",TypeCode.String){ Length = 60 },
                new("Datum",TypeCode.String),
                new("Nummer",TypeCode.Int32),
                new("iKenn",TypeCode.String){ Length = 1 },
                new("Sich",TypeCode.String){ Length = 1 }],
             Indexes =[
                 new("Nummer",["Nummer"]) { Unique = true },
                 new("Namen",["Name"]),
                 new("Persuch",["Name","Datum"]),
                 new("Soundsuch",["Sound","Name","Datum"]),
                 new("K_Phonsuch",["K_Phon","Name","Datum"]),
                 new("Aliassuch",["Alias","Datum"]),
                 new("Leitsuch",["Leit","Datum"])] } ];
        public static stTableDef[] DTDef = [ new() {
             Name= "Ahnen1",
             Fields = [
                new("Gen",TypeCode.Int16) ,
                new("PerNr",TypeCode.Int32),
                new("Weiter",TypeCode.String){ Length = 1 },
                new("EHE",TypeCode.Int32),
                new("Ahn",TypeCode.String){ Length = 40 },
                new("Name",TypeCode.String){ Length = 255 },
                new("aiSpitz",TypeCode.String){ Length = 1 }],
             Indexes =[
                 new("aiSpitz",["aiSpitz"]),
                 new("Namen",["Name"]),
                 new("PerNr",["PerNr"]),
                 new("Gen",["Gen"]),
                 new("Ahnen",["Ahn"]),
                 new("Implex",["PerNr", "Ahn"])
             ] },
        new() {
             Name= "Ahnew",
             Fields = [
                new("Ahn",TypeCode.String){ Length = 40 },
                new("PerNr",TypeCode.Int32)],
             Indexes =[
                 new("Kind",["PerNr"])
             ] },
        new() {
            xDrop = true,
             Name= "Konf",
             Fields = [
                new("PerNr",TypeCode.Int32),
                new("Textnr",TypeCode.Int32) ],
             Indexes =[
                 new("T",["TextNr"])
             ] },
        new() {
             Name= "Nachk",
             Fields = [
                new("Gen",TypeCode.Int16),
                new("Nr",TypeCode.String){Length = 240,xNull = false },
                new("Pr",TypeCode.Int32),
                new("LfNr",TypeCode.Int32),
                new("kia",TypeCode.String){Length = 2 }
             ],
             Indexes =[
                 new("PerNr",["Pr"]){Unique = true },
                 new("Nr",["Nr"]),
                 new("GNr",["Gen","Nr"]),
                 new("LNr",["Gen","LfNr"])
             ] }];


        public static stTableDef[] DbDef = [
        new() { Name = nameof(dbTables.BesitzTab),
          Fields = [
                new(PropertyFields.Nr, TypeCode.Int32) ,
                new(PropertyFields.Akte, TypeCode.String) { Length = 240 },
                new(PropertyFields.Pers, TypeCode.Int32) ],
          Indexes = [
                new(PropertyIndex.Akt, [PropertyFields.Akte] ),
                new(PropertyIndex.NuAkPer, [PropertyFields.Nr, PropertyFields.Akte, PropertyFields.Pers] ){ Unique = true },
                new(PropertyIndex.Per, [PropertyFields.Pers] )]
                },
        new() { Name = nameof(dbTables.Bilder),
                IntName = "Picture - Table", //from MandDB Table: DB_PictureTable
          Fields = [
                new(PictureFields.Bem, TypeCode.String) ,
                new(PictureFields.Beschreibung, TypeCode.String) { Length = 255 },
                new(PictureFields.Datei, TypeCode.String) { Length = 255 },
                new(PictureFields.Form, TypeCode.Boolean) ,
                new(PictureFields.Format, TypeCode.String) { Length = 1 },
                new(PictureFields.Kennz, TypeCode.String) { Length = 1 },
                new(PictureFields.LfNr, TypeCode.Int32) ,
                new(PictureFields.Pfad, TypeCode.String) { Length = 255 },
                new(PictureFields.ZuNr, TypeCode.Int32) ],
          Indexes = [
                new(PictureIndex.Nr, [ PictureFields.LfNr ]){ Unique = true },
                new(PictureIndex.Bild, [ PictureFields.Datei ]),
                new(PictureIndex.PerKen2, [ PictureFields.Kennz, PictureFields.ZuNr, PictureFields.Beschreibung ]),
                new(PictureIndex.PerKenn, [ PictureFields.Kennz, PictureFields.ZuNr ]) ]
                },
       new() { Name = nameof(dbTables.Doppel),
                IntName = "Doppel - Table", //from MandDB Table: DB_DoppelTable
          Fields = [
                new(DoppelFields.Nr, TypeCode.String) { Length = 240 },
                new(DoppelFields.Pr, TypeCode.Int32) ],
          Indexes = [
                new(DoppelIndex.Nr, [ DoppelFields.Pr ]){ Unique = true } ]
               },
       new() { Name = nameof(dbTables.Ereignis),
          Fields = [
                new(EventFields.Art, TypeCode.Int16) ,
                new(EventFields.PerFamNr, TypeCode.Int32) ,
                new(EventFields.DatumV, TypeCode.Int32) ,
                new(EventFields.DatumB_S, TypeCode.String) { Length = 1 },
                new(EventFields.DatumV_S, TypeCode.String) { Length = 1 },
                new(EventFields.DatumB, TypeCode.Int32) ,
                new(EventFields.KBem, TypeCode.Int32) ,
                new(EventFields.Ort, TypeCode.Int32) ,
                new(EventFields.Ort_S, TypeCode.String) { Length = 1, xNull = true },
                new(EventFields.Reg, TypeCode.String) { Length = 50 },
                new(EventFields.Bem1, TypeCode.String) ,
                new(EventFields.Bem2, TypeCode.String) ,
                new(EventFields.Platz, TypeCode.Int32) ,
                new(EventFields.VChr, TypeCode.Boolean) ,
                new(EventFields.LfNr, TypeCode.Int16) ,
                new(EventFields.Bem3, TypeCode.String) ,
                new(EventFields.Bem4, TypeCode.String) ,
                new(EventFields.ArtText, TypeCode.String) { Length = 240 },
                new(EventFields.Zusatz, TypeCode.String) { Length = 240 },
                new(EventFields.priv, TypeCode.String) { Length = 1 },
                new(EventFields.tot, TypeCode.String) { Length = 1 },
                new(EventFields.DatumText, TypeCode.Int32) ,
                new(EventFields.Causal, TypeCode.Int32) ,
                new(EventFields.an, TypeCode.Int32) ,
                new(EventFields.Hausnr, TypeCode.Int32) ,
                new(EventFields.GrabNr, TypeCode.Int32) ],
          Indexes = [
                new(EventIndex.ArtNr, [EventFields.Art,EventFields.PerFamNr, EventFields.LfNr] ){ Unique = true },
                new(EventIndex.BeSu, [EventFields.Art,EventFields.PerFamNr] ),
                new(EventIndex.Datbs, [EventFields.DatumB_S] ),
                new(EventIndex.DatInd, [EventFields.DatumV] ),
                new(EventIndex.Datvs, [EventFields.DatumV_S] ),
                new(EventIndex.EOrt, [EventFields.Ort] ),
                new(EventIndex.HaNu, [EventFields.Hausnr] ),
                new(EventIndex.JaTa, [EventFields.Art] ),
                new(EventIndex.CText, [EventFields.Causal] ),
                new(EventIndex.KText, [EventFields.KBem] ),
                new(EventIndex.NText, [EventFields.ArtText] ),
                new(EventIndex.PText, [EventFields.Platz] ),
                new(EventIndex.Reg, [EventFields.Reg] ),
                new(EventIndex.Reg1, [EventFields.Art, EventFields.Reg] )]
                },
        new() { Name = nameof(dbTables.Familie),
          Fields = [
                new(nameof(FamilyFields.AnlDatum), TypeCode.Int32) ,
                new(nameof(FamilyFields.EditDat), TypeCode.Int32) ,
                new(nameof(FamilyFields.Prüfen), TypeCode.String) { Length = 5 },
                new(nameof(FamilyFields.Bem1), TypeCode.String) ,
                new(nameof(FamilyFields.FamNr), TypeCode.Int32) ,
                new(nameof(FamilyFields.Aeb), TypeCode.Boolean) ,
                new(nameof(FamilyFields.Name), TypeCode.Int32) ,
                new(nameof(FamilyFields.Bem2), TypeCode.String) ,
                new(nameof(FamilyFields.Bem3), TypeCode.String) ,
                new(nameof(FamilyFields.Eltern), TypeCode.Single) ,
                new(nameof(FamilyFields.Fuid), TypeCode.String) { Length = 40 },
                new(nameof(FamilyFields.Prae), TypeCode.String) { Length = 240 },
                new(nameof(FamilyFields.Suf), TypeCode.String) { Length = 240 },
                new(nameof(FamilyFields.ggv), TypeCode.String) { Length = 1 }],
          Indexes = [
                new(FamilyIndex.BeaDat, [FamilyFields.EditDat] ),
                new(FamilyIndex.Fam, [FamilyFields.FamNr] ){ Unique = true },
                new(FamilyIndex.Fuid, [FamilyFields.Fuid] )]
                },
        new() { Name = nameof(dbTables.GBE),
          Fields = [
                new(GBEFields.Nr, TypeCode.Int32) ,
                new(GBEFields.Akte, TypeCode.String) { Length = 240 },
                new(GBEFields.Jahr, TypeCode.String) { Length = 24 },
                new(GBEFields.Name, TypeCode.String) ,
                new(GBEFields.Geb, TypeCode.String) ,
                new(GBEFields.Erb, TypeCode.String) { Length = 24 },
                new(GBEFields.Abg, TypeCode.String) { Length = 24 }],
          Indexes = [
                new("Akte", ["Akte"] ),
                new("AkteJa", ["Akte", "Jahr"] ),
                new("Nr", ["Nr"] ){ Unique = true }]
                },
        new() { Name = nameof(dbTables.GED),
          Fields = [
                new("PNr", TypeCode.Int32) ],
          Indexes = [
                new("pNr", ["PNr"] ){ Unique = true }]
                },
        new() { Name = nameof(dbTables.HGA),
          Fields = [
                new(HGAFields.Nr, TypeCode.Int32) ,
                new(HGAFields.Akte, TypeCode.String) { Length = 240 },
                new(HGAFields.Kirchspiel, TypeCode.String) { Length = 240 },
                new(HGAFields.Beschr, TypeCode.String) { Length = 240 },
                new(HGAFields.Flur, TypeCode.String) { Length = 240 },
                new(HGAFields.Parzelle, TypeCode.String) { Length = 240 },
                new(HGAFields.Hof, TypeCode.String) { Length = 240 },
                new(HGAFields.Brandkasse, TypeCode.String) ,
                new(HGAFields.Bem, TypeCode.String) ],
          Indexes = [
                new("Akte", ["Akte"] ){ Unique = true },
                new("Nr", ["Nr"] ){ Unique = true }]
                },
        new() { Name = nameof(dbTables.INamen),
          Fields = [
                new(NameFields.PersNr, TypeCode.Int32) ,
                new(NameFields.Kennz, TypeCode.String) { Length = 1 },
                new(NameFields.Text, TypeCode.Int32) ,
                new(NameFields.LfNr, TypeCode.String) { Length = 2 },
                new(NameFields.Ruf, TypeCode.Byte) ,
                new(NameFields.Spitz, TypeCode.Byte) ],
          Indexes = [
                new(NameIndex.NamKenn, [] ),
                new(NameIndex.PNamen, [] ),
                new(NameIndex.TxNr, [] ),
                new(NameIndex.Vollname, [NameFields.PersNr, NameFields.Kennz, NameFields.LfNr] ){ Unique = true }]
                },
        new() { Name = nameof(dbTables.IndNam), //OFB-Table
          Fields = [
                new(OFBFields.PerNr, TypeCode.Int32) ,
                new(OFBFields.Kennz, TypeCode.String) { Length = 2 },
                new(OFBFields.TextNr, TypeCode.Int32) ],
          Indexes = [
                new(OFBIndex.Indn, [OFBFields.PerNr, OFBFields.Kennz ,OFBFields.TextNr] ){ Unique = true},
                new(OFBIndex.InDNr, [OFBFields.PerNr,OFBFields.Kennz] ),
                new(OFBIndex.IndNum, [OFBFields.TextNr] )]
                },
        new() { Name = nameof(dbTables.Leer1),
          Fields = [
                new("Nr", TypeCode.Int32) ,
                new("Art", TypeCode.String) { Length = 1 }],
          Indexes = [
                new("Leer", ["Art"] )]
                },
        new() { Name = nameof(dbTables.Nachk),
          Fields = [
                new("Gen", TypeCode.Int32) ,
                new("Nr", TypeCode.String) { Length = 240, xNull = true },
                new("Pr", TypeCode.Int32) ],
          Indexes = [
                new("Nr", ["Nr"] ),
                new("PerNr", ["Pr", "Nr"] ){ Unique = true }]
                },
        new() { Name = nameof(dbTables.Orte),
          Fields = [
                new(PlaceFields.Ort, TypeCode.Int32) ,
                new(PlaceFields.Ortsteil, TypeCode.Int32) ,
                new(PlaceFields.Kreis, TypeCode.Int32) ,
                new(PlaceFields.Land, TypeCode.Int32) ,
                new(PlaceFields.Staat, TypeCode.Int32) ,
                new(PlaceFields.Staatk, TypeCode.String) { Length = 3 },
                new(PlaceFields.PLZ, TypeCode.String) { Length = 10 },
                new(PlaceFields.Terr, TypeCode.String) { Length = 5 },
                new(PlaceFields.Loc, TypeCode.String) { Length = 6 },
                new(PlaceFields.L, TypeCode.String) { Length = 10 },
                new(PlaceFields.B, TypeCode.String) { Length = 10 },
                new(PlaceFields.Bem, TypeCode.String) ,
                new(PlaceFields.OrtNr, TypeCode.Int32) { xNull = true },
                new(PlaceFields.Zusatz, TypeCode.String) { Length = 240 },
                new(PlaceFields.GOV, TypeCode.String) { Length = 20 },
                new(PlaceFields.PolName, TypeCode.String) { Length = 240 },
                new(PlaceFields.g, TypeCode.Single) ],
          Indexes = [
                new(PlaceIndex.K, [PlaceFields.Kreis] ),
                new(PlaceIndex.L, [PlaceFields.Land] ),
                new(PlaceIndex.O, [] ),
                new(PlaceIndex.Orte, [PlaceFields.Ort] ),
                new(PlaceIndex.OrtNr, [PlaceFields.OrtNr] ){ Unique = true },
                new(PlaceIndex.OT, [PlaceFields.Ortsteil] ),
                new(PlaceIndex.Pol, [PlaceFields.PolName] ),
                new(PlaceIndex.S, [PlaceFields.Staat] )]
                },
        new() { Name = nameof(dbTables.OrtSuch),
          Fields = [
                new("Name", TypeCode.String) { Length = 100 },
                new("Nr", TypeCode.Int32) ],
          Indexes = [
                new("OrtNr", ["Nr"] ),
                new("ortsu", ["Name"] ){ Unique = true }]
                },
        new() { Name = nameof(dbTables.Personen),
          Fields = [
                new(PersonFields.AnlDatum, TypeCode.Int32) ,
                new(PersonFields.Sex, TypeCode.String) { Length = 1, xNull = true },
                new(PersonFields.EditDat, TypeCode.Int32) ,
                new(PersonFields.Bem1, TypeCode.String) ,
                new(PersonFields.Pruefen, TypeCode.String) { Length = 5 },
                new(PersonFields.PersNr, TypeCode.Int32) ,
                new(PersonFields.Konv, TypeCode.String) { Length = 240 },
                new(PersonFields.Such1, TypeCode.String) { Length = 240 },
                new(PersonFields.Bem2, TypeCode.String) ,
                new(PersonFields.Bem3, TypeCode.String) ,
                new(PersonFields.OFB, TypeCode.String) { Length = 1 },
                new(PersonFields.religi, TypeCode.Int32) ,
                new(PersonFields.PUid, TypeCode.String) { Length = 40 },
                new(PersonFields.Such2, TypeCode.String) { Length = 240 },
                new(PersonFields.Such3, TypeCode.String) { Length = 240 },
                new(PersonFields.Such4, TypeCode.String) { Length = 240 },
                new(PersonFields.Such5, TypeCode.String) { Length = 240 },
                new(PersonFields.Such6, TypeCode.String) { Length = 240 }],
          Indexes = [
                new(PersonIndex.BeaDat,[PersonFields.EditDat] ),
                new(PersonIndex.PerNr, [PersonFields.PersNr]){ Unique = true },
                new(PersonIndex.Puid,  [PersonFields.PUid] ),
                new(PersonIndex.reli,  [PersonFields.religi] ),
                new(PersonIndex.Such1, [PersonFields.Such1, PersonFields.PersNr]) { IgnoreNull = true} ,
                new(PersonIndex.Such2, [PersonFields.Such2, PersonFields.PersNr] ),
                new(PersonIndex.Such3, [PersonFields.Such3, PersonFields.PersNr] ),
                new(PersonIndex.Such4, [PersonFields.Such4, PersonFields.PersNr] ){ IgnoreNull = true},
                new(PersonIndex.Such5, [PersonFields.Such5, PersonFields.PersNr] ),
                new(PersonIndex.Such6, [PersonFields.Such6, PersonFields.PersNr] )]
                },
        new() { Name = nameof(dbTables.Quellen),
          Fields = [
                new(SourceFields._1, TypeCode.Int32) ,  // PerNr
                new(SourceFields._2, TypeCode.String) { Length = 240 }, // Titel
                new(SourceFields._3, TypeCode.String) { Length = 240 }, // Ort
                new(SourceFields._4, TypeCode.String) { Length = 240 }, // Zitat
                new(SourceFields._5, TypeCode.String) { Length = 240 }, // Author
                new(SourceFields._7, TypeCode.String) { Length = 240 },
                new(SourceFields._8, TypeCode.String) { Length = 240 },
                new(SourceFields._9, TypeCode.String) { Length = 240 },
                new(SourceFields._10, TypeCode.String) { Length = 240 },
                new(SourceFields._11, TypeCode.String) { Length = 240 },
                new(SourceFields._12, TypeCode.String) { Length = 240 },
                new(SourceFields._13, TypeCode.String) ],
          Indexes = [
                new(SourceIndex.Autor, [SourceFields._5] ),
                new(SourceIndex.Dopp, [SourceFields._2, SourceFields._4] ),
                new(SourceIndex.Nam, [SourceFields._2] ),
                new(SourceIndex.Nr, [SourceFields._1] ),
                new(SourceIndex.Zitat, [SourceFields._4] ){ Unique = true }]
                },
        new() { Name = nameof(dbTables.Repo),
          Fields = [
                new(RepoFields.Nr, TypeCode.Int32) ,
                new(RepoFields.Name, TypeCode.String) { Length = 240 },
                new(RepoFields.Strasse, TypeCode.String) { Length = 240 },
                new(RepoFields.Ort, TypeCode.String) { Length = 240 },
                new(RepoFields.PLZ, TypeCode.String) { Length = 240 },
                new(RepoFields.Fon, TypeCode.String) { Length = 240 },
                new(RepoFields.Mail, TypeCode.String) { Length = 240 },
                new(RepoFields.Http, TypeCode.String) { Length = 240 },
                new(RepoFields.Bem, TypeCode.String) ,
                new(RepoFields.Suchname, TypeCode.String) { Length = 240 }],
          Indexes = [
                new("Name", ["SuchName"] ),
                new("Nr", ["Nr"] ),
                new("Such", ["SuchName", "Ort"] ){ Unique = true }]
                },
        new() { Name = nameof(dbTables.RepoTab),
          Fields = [
                new("Quelle", TypeCode.Int32) ,
                new("Repo", TypeCode.Int32) ],
          Indexes = [
                new("Dop", ["Quelle", "Repo"] ),
                new("Leer", ["Repo"] ),
                new("Nr", ["Quelle"] )]
                },
        new() { Name = nameof(dbTables.Such),
          Fields = [
                new("Name", TypeCode.String) { Length = 50 },
                new("Datum", TypeCode.Int16) ,
                new("Nummer", TypeCode.Int32) ,
                new("iKenn", TypeCode.String) { Length = 1 },
                new("Sich", TypeCode.String) { Length = 1 }],
          Indexes = [
                new("Namen", [""] ),
                new("Nummer", [""] ){ Unique = true },
                new("Persuch", [""] )]
                },
        new() { Name = nameof(dbTables.Tab),
          Fields = [
                new(ILinkData.LinkFields.Kennz, TypeCode.Byte) ,
                new(ILinkData.LinkFields.FamNr, TypeCode.Int32) ,
                new(ILinkData.LinkFields.PerNr, TypeCode.Int32) ],
          Indexes = [
                new(LinkIndex.ElSu,    [ILinkData.LinkFields.PerNr, ILinkData.LinkFields.Kennz] ),
                new(LinkIndex.FamNr,   [ILinkData.LinkFields.FamNr] ),
                new(LinkIndex.FamPruef,[ILinkData.LinkFields.FamNr, ILinkData.LinkFields.PerNr, ILinkData.LinkFields.Kennz] ){ Unique = true },
                new(LinkIndex.FamSu,   [ILinkData.LinkFields.FamNr, ILinkData.LinkFields.Kennz] ),
                new(LinkIndex.FamSu1,  [ILinkData.LinkFields.FamNr, ILinkData.LinkFields.Kennz] ),
                new(LinkIndex.PAFI,    [ILinkData.LinkFields.Kennz] ),
                new(LinkIndex.Per,     [ILinkData.LinkFields.PerNr] )]
                },
        new() { Name = nameof(dbTables.Tab1),
          Fields = [
                new(SourceLinkFields._1, TypeCode.Single) ,
                new(SourceLinkFields._2, TypeCode.Int32) ,
                new(SourceLinkFields._3, TypeCode.Int32) ,
                new(SourceLinkFields._4, TypeCode.String) { Length = 240 },
                new(SourceLinkFields.LfNr, TypeCode.Single) ,
                new(SourceLinkFields.Art, TypeCode.Single) ,
                new(SourceLinkFields.Aus, TypeCode.String) { Length = 240 },
                new(SourceLinkFields.Orig, TypeCode.String) ,
                new(SourceLinkFields.Kom, TypeCode.String) ],
          Indexes = [
                new(SourceLinkIndex.Tab, [SourceLinkFields._1,SourceLinkFields._2] ),
                new(SourceLinkIndex.Tab2.AsFld(), [""] ),
                new(SourceLinkIndex.Tab21.AsFld(), [""] ),
                new(SourceLinkIndex.Tab22, [SourceLinkFields._1, SourceLinkFields._2, SourceLinkFields.Art,SourceLinkFields.LfNr] ),
                new(SourceLinkIndex.Tab23.AsFld(), [""] ),
                new(SourceLinkIndex.Verw.AsFld(), [""] )]
                },
        new() { Name = nameof(dbTables.Tab2),
          Fields = [
                new(WitnessFields.FamNr, TypeCode.Int32) ,
                new(WitnessFields.PerNr, TypeCode.Int32) ,
                new(WitnessFields.Kennz, TypeCode.String) { Length = 2 },
                new(WitnessFields.Art, TypeCode.Single) ,
                new(WitnessFields.LfNr, TypeCode.Single) ],
          Indexes = [
                new(WitnessIndex.ElSu,    [WitnessFields.Art, WitnessFields.PerNr, WitnessFields.Kennz] ),
                new(WitnessIndex.Fampruef,[WitnessFields.FamNr, WitnessFields.PerNr, WitnessFields.Kennz, WitnessFields.Art, WitnessFields.LfNr] ){ Unique = true },
                new(WitnessIndex.FamSu,   [WitnessFields.Art, WitnessFields.FamNr, WitnessFields.Kennz] ),
                new(WitnessIndex.Zeug,    [WitnessFields.FamNr, WitnessFields.PerNr, WitnessFields.Kennz, WitnessFields.Art, WitnessFields.LfNr] ),
                new(WitnessIndex.ZeugSu,  [WitnessFields.FamNr, WitnessFields.Kennz, WitnessFields.Art, WitnessFields.LfNr] )]
                },
        new() { Name = nameof(dbTables.Texte),
          Fields = [
                new(TexteFields.Txt, TypeCode.String) { Length = 240 },
                new(TexteFields.Kennz, TypeCode.String) { Length = 1, xNull = true },
                new(TexteFields.TxNr, TypeCode.Int32) ,
                new(TexteFields.Leitname, TypeCode.String) { Length = 32 },
                new(TexteFields.Bem, TypeCode.String) { xNull = true }],
          Indexes = [
                new(TexteIndex.ITexte, [TexteFields.Txt, TexteFields.Kennz] ){ Unique = true },
                new(TexteIndex.KText,  [] ),
                new(TexteIndex.LTexte, [TexteFields.Leitname, TexteFields.Kennz] ),
                new(TexteIndex.RTexte, [] ),
                new(TexteIndex.SSTexte,[TexteFields.Txt] ),
                new(TexteIndex.STexte, [TexteFields.Kennz, TexteFields.Txt] ),
                new(TexteIndex.TxNr,   [TexteFields.TxNr] ),
                new(TexteIndex.TxNr1,  [TexteFields.TxNr] ){ Unique = true }]
                },
        new() { Name = nameof(dbTables.WDBL),
          Fields = [
                new(WDBLFields.Nr, TypeCode.Byte) ]} ];

    }
}
