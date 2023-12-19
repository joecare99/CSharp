using GenFree.Interfaces;
using GenFree.Helper;
using System;
using System.Linq;

namespace GenFree.Data
{
    public static partial class DataModul
    {

        public struct stFieldDef
        {
            public string Name;
            public TypeCode Typ;
            public int Laenge = 0;
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
            //   public List<stFieldDef> Fields;
            public stFieldDef[] Fields;
            public stIndex[] Indexes;
            public bool xDrop;
        }

        public static stTableDef[] DOSBDef = { new() {
             Name= "Ortsuch",
             Fields = new stFieldDef[]{
                new("Name",TypeCode.String) { Laenge = 100 },
                new("Nr",TypeCode.Int64)},
             Indexes =new stIndex[]{
                 new("Ortnr",new[] { "Nr" }),
                 new("Ortsu",new[] { "Name" })} } };
        public static stTableDef[] DSBDef = { new() {
             Name= "Such",
             Fields = new stFieldDef[]{
                new("Name",TypeCode.String) { Laenge = 50 },
                new("K_Phon",TypeCode.String){ Laenge = 60 },
                new("Leit",TypeCode.String){ Laenge = 60 },
                new("Alias",TypeCode.String){ Laenge = 60 },
                new("Sound",TypeCode.String){ Laenge = 60 },
                new("Datum",TypeCode.String),
                new("Nummer",TypeCode.Int64),
                new("iKenn",TypeCode.String){ Laenge = 1 },
                new("Sich",TypeCode.String){ Laenge = 1 }},
             Indexes =new stIndex[]{
                 new("Nummer",new[] { "Nummer" }) { Unique = true },
                 new("Namen",new[] { "Name" }),
                 new("Persuch",new[] { "Name","Datum" }),
                 new("Soundsuch",new[] { "Sound","Name","Datum" }),
                 new("K_Phonsuch",new[] { "K_Phon","Name","Datum" }),
                 new("Aliassuch",new[] { "Alias","Datum" }),
                 new("Leitsuch",new[] { "Leit","Datum" })} } };
        public static stTableDef[] DTDef = { new() {
             Name= "Ahnen1",
             Fields = new stFieldDef[]{
                new("Gen",TypeCode.Int32) ,
                new("PerNr",TypeCode.Int64),
                new("Weiter",TypeCode.String){ Laenge = 1 },
                new("EHE",TypeCode.Int64),
                new("Ahn",TypeCode.String){ Laenge = 40 },
                new("Name",TypeCode.String){ Laenge = 255 },
                new("aiSpitz",TypeCode.String){ Laenge = 1 }},
             Indexes =new stIndex[]{
                 new("aiSpitz",new[] { "aiSpitz" }),
                 new("Namen",new[] { "Name" }),
                 new("PerNr",new[] { "PerNr" }),
                 new("Gen",new[] { "Gen" }),
                 new("Ahnen",new[] { "Ahn" }),
                 new("Implex",new[] { "PerNr", "Ahn" })
             } },
        new() {
             Name= "Ahnew",
             Fields = new stFieldDef[]{
                new("Ahn",TypeCode.String){ Laenge = 40 },
                new("PerNr",TypeCode.Int64)},
             Indexes =new stIndex[]{
                 new("Kind",new[] { "PerNr" })
             } },
        new() {
            xDrop = true,
             Name= "Konf",
             Fields = new stFieldDef[]{
                new("PerNr",TypeCode.Int64),
                new("Textnr",TypeCode.Int64) },
             Indexes =new stIndex[]{
                 new("T",new[] { "TextNr" })
             } },
        new() {
             Name= "Nachk",
             Fields = new stFieldDef[]{
                new("Gen",TypeCode.Int32),
                new("Nr",TypeCode.String){Laenge = 240,xNull = false },
                new("Pr",TypeCode.Int64),
                new("LfNr",TypeCode.Int64),
                new("kia",TypeCode.String){Laenge = 2 }
             },
             Indexes =new stIndex[]{
                 new("PerNr",new[] { "Pr" }){Unique = true },
                 new("Nr",new[] { "Nr" }),
                 new("GNr",new[] { "Gen","Nr" }),
                 new("LNr",new[] { "Gen","LfNr" })
             } }};


        public static stTableDef[] DbDef = {
        new() { Name = nameof(dbTables.BesitzTab),
          Fields = new stFieldDef[]{
                new(PropertyFields.Nr, TypeCode.Int64) ,
                new(PropertyFields.Akte, TypeCode.String) { Laenge = 240 },
                new(PropertyFields.Pers, TypeCode.Int64) },
          Indexes = new stIndex[]{
                new(PropertyIndex.Akt, new Enum[] { PropertyFields.Akte } ),
                new(PropertyIndex.NuAkPer, new Enum[] { PropertyFields.Nr, PropertyFields.Akte, PropertyFields.Pers } ){ Unique = true },
                new(PropertyIndex.Per, new Enum[] { PropertyFields.Pers } )}
                },
        new() { Name = nameof(dbTables.Bilder),
          Fields = new stFieldDef[]{
                new("ZuNr", TypeCode.Int64) ,
                new("Kennz", TypeCode.String) { Laenge = 1 },
                new("Bem", TypeCode.String) ,
                new("Datei", TypeCode.String) { Laenge = 255, xNull = true },
                new("Pfad", TypeCode.String) { Laenge = 255 },
                new("LfNr", TypeCode.Int64) ,
                new("Form", TypeCode.Boolean) ,
                new("Beschreibung", TypeCode.String) { Laenge = 255, xNull = true },
                new("Format", TypeCode.String) { Laenge = 1 }},
          Indexes = new stIndex[]{
                new("Bild", new[] { "" } ),
                new("Nr", new[] { "LfNr" } ){ Unique = true },
                new("PerKen2", new[] { "" } ),
                new("PerKenn", new[] { "Kennz", "ZuNr" } )}
                },
        new() { Name = nameof(dbTables.Doppel),
          Fields = new stFieldDef[]{
                new("Nr", TypeCode.String) { Laenge = 240, xNull = true },
                new("Pr", TypeCode.Int64) },
          Indexes = new stIndex[]{
                new("Nr", new[] { "Nr" } ){ Unique = true }}
                },
        new() { Name = nameof(dbTables.Ereignis),
          Fields = new stFieldDef[]{
                new(EventFields.Art, TypeCode.Int32) ,
                new(EventFields.PerFamNr, TypeCode.Int64) ,
                new(EventFields.DatumV, TypeCode.Int64) ,
                new(EventFields.DatumB_S, TypeCode.String) { Laenge = 1 },
                new(EventFields.DatumV_S, TypeCode.String) { Laenge = 1 },
                new(EventFields.DatumB, TypeCode.Int64) ,
                new(EventFields.KBem, TypeCode.Int64) ,
                new(EventFields.Ort, TypeCode.Int64) ,
                new(EventFields.Ort_S, TypeCode.String) { Laenge = 1, xNull = true },
                new(EventFields.Reg, TypeCode.String) { Laenge = 50 },
                new(EventFields.Bem1, TypeCode.String) ,
                new(EventFields.Bem2, TypeCode.String) ,
                new(EventFields.Platz, TypeCode.Int64) ,
                new(EventFields.VChr, TypeCode.Boolean) ,
                new(EventFields.LfNr, TypeCode.Int32) ,
                new(EventFields.Bem3, TypeCode.String) ,
                new(EventFields.Bem4, TypeCode.String) ,
                new(EventFields.ArtText, TypeCode.String) { Laenge = 240 },
                new(EventFields.Zusatz, TypeCode.String) { Laenge = 240 },
                new(EventFields.priv, TypeCode.String) { Laenge = 1 },
                new(EventFields.tot, TypeCode.String) { Laenge = 1 },
                new(EventFields.DatumText, TypeCode.Int64) ,
                new(EventFields.Causal, TypeCode.Int64) ,
                new(EventFields.an, TypeCode.Int64) ,
                new(EventFields.Hausnr, TypeCode.Int64) ,
                new(EventFields.GrabNr, TypeCode.Int64) },
          Indexes = new stIndex[]{
                new("ArtNr", new[] { "Art","PerFamNr", "LfNnr" } ){ Unique = true },
                new("BeSu", new[] { "Art", "PerFamNr" } ),
                new("CText", new[] { "Causal" } ),
                new("Datbs", new[] { "Datumb_s" } ),
                new("DatInd", new[] { "DatumV" } ),
                new("Datvs", new[] { "DatumV_s" } ),
                new("EOrt", new[] { "Ort" } ),
                new("HaNu", new[] { "Hausnr" } ),
                new("JaTa", new[] { "" } ),
                new(EventIndex.KText, new Enum[] { EventFields.KBem } ),
                new("NText", new[] { "ArtText" } ),
                new(EventIndex.PText, new Enum[] { EventFields.Platz } ),
                new("Reg", new[] { "Reg" } ),
                new("Reg1", new[] { "Art", "Reg" } )}
                },
        new() { Name = nameof(dbTables.Familie),
          Fields = new stFieldDef[]{
                new(nameof(FamilyFields.AnlDatum), TypeCode.Int64) ,
                new(nameof(FamilyFields.EditDat), TypeCode.Int64) ,
                new(nameof(FamilyFields.Prüfen), TypeCode.String) { Laenge = 5 },
                new(nameof(FamilyFields.Bem1), TypeCode.String) ,
                new(nameof(FamilyFields.FamNr), TypeCode.Int64) ,
                new(nameof(FamilyFields.Aeb), TypeCode.Boolean) ,
                new(nameof(FamilyFields.Name), TypeCode.Int64) ,
                new(nameof(FamilyFields.Bem2), TypeCode.String) ,
                new(nameof(FamilyFields.Bem3), TypeCode.String) ,
                new(nameof(FamilyFields.Eltern), TypeCode.Single) ,
                new(nameof(FamilyFields.Fuid), TypeCode.String) { Laenge = 40 },
                new(nameof(FamilyFields.Prae), TypeCode.String) { Laenge = 240 },
                new(nameof(FamilyFields.Suf), TypeCode.String) { Laenge = 240 },
                new(nameof(FamilyFields.ggv), TypeCode.String) { Laenge = 1 }},
          Indexes = new stIndex[]{
                new(FamilyIndex.BeaDat, new Enum[] { FamilyFields.EditDat } ),
                new(FamilyIndex.Fam, new Enum[] { FamilyFields.FamNr } ){ Unique = true },
                new(FamilyIndex.Fuid, new Enum[] { FamilyFields.Fuid } )}
                },
        new() { Name = nameof(dbTables.GBE),
          Fields = new stFieldDef[]{
                new("Nr", TypeCode.Int64) ,
                new("Akte", TypeCode.String) { Laenge = 240 },
                new("Jahr", TypeCode.String) { Laenge = 24 },
                new("Name", TypeCode.String) ,
                new("Geb", TypeCode.String) ,
                new("Erb", TypeCode.String) { Laenge = 24 },
                new("Abg", TypeCode.String) { Laenge = 24 }},
          Indexes = new stIndex[]{
                new("Akte", new[] { "Akte" } ),
                new("AkteJa", new[] { "Akte", "Jahr" } ),
                new("Nr", new[] { "Nr" } ){ Unique = true }}
                },
        new() { Name = nameof(dbTables.GED),
          Fields = new stFieldDef[]{
                new("PNr", TypeCode.Int64) },
          Indexes = new stIndex[]{
                new("pNr", new[] { "PNr" } ){ Unique = true }}
                },
        new() { Name = nameof(dbTables.HGA),
          Fields = new stFieldDef[]{
                new("Nr", TypeCode.Int64) ,
                new("Akte", TypeCode.String) { Laenge = 240 },
                new("Kirchspiel", TypeCode.String) { Laenge = 240 },
                new("Beschr", TypeCode.String) { Laenge = 240 },
                new("Flur", TypeCode.String) { Laenge = 240 },
                new("Parzelle", TypeCode.String) { Laenge = 240 },
                new("Hof", TypeCode.String) { Laenge = 240 },
                new("Brandkasse", TypeCode.String) ,
                new("Bem", TypeCode.String) },
          Indexes = new stIndex[]{
                new("Akte", new[] { "Akte" } ){ Unique = true },
                new("Nr", new[] { "Nr" } ){ Unique = true }}
                },
        new() { Name = nameof(dbTables.Inamen),
          Fields = new stFieldDef[]{
                new(NameFields.PersNr, TypeCode.Int64) ,
                new(NameFields.Kennz, TypeCode.String) { Laenge = 1 },
                new(NameFields.Text, TypeCode.Int64) ,
                new(NameFields.LfNr, TypeCode.String) { Laenge = 2 },
                new(NameFields.Ruf, TypeCode.Byte) ,
                new(NameFields.Spitz, TypeCode.Byte) },
          Indexes = new stIndex[]{
                new(NameIndex.NamKenn, new Enum[] {  } ),
                new(NameIndex.PNamen, new Enum[] {  } ),
                new(NameIndex.TxNr, new Enum[] {  } ),
                new(NameIndex.Vollname, new Enum[] { NameFields.PersNr, NameFields.Kennz, NameFields.LfNr } ){ Unique = true },                }
                },
        new() { Name = nameof(dbTables.INDNam),
          Fields = new stFieldDef[]{
                new("PerNr", TypeCode.Int64) ,
                new("Kennz", TypeCode.String) { Laenge = 2 },
                new("Textnr", TypeCode.Int64) },
          Indexes = new stIndex[]{
                new("Indn", new[] { "" } ),
                new("Indnr", new[] { "" } ),
                new("Indnum", new[] { "" } )}
                },
        new() { Name = nameof(dbTables.Leer1),
          Fields = new stFieldDef[]{
                new("Nr", TypeCode.Int64) ,
                new("Art", TypeCode.String) { Laenge = 1 }},
          Indexes = new stIndex[]{
                new("Leer", new[] { "Art" } )}
                },
        new() { Name = nameof(dbTables.Nachk),
          Fields = new stFieldDef[]{
                new("Gen", TypeCode.Int64) ,
                new("Nr", TypeCode.String) { Laenge = 240, xNull = true },
                new("Pr", TypeCode.Int64) },
          Indexes = new stIndex[]{
                new("Nr", new[] { "Nr" } ),
                new("PerNr", new[] { "Pr", "Nr" } ){ Unique = true }}
                },
        new() { Name = nameof(dbTables.Orte),
          Fields = new stFieldDef[]{
                new(PlaceFields.Ort, TypeCode.Int64) ,
                new(PlaceFields.Ortsteil, TypeCode.Int64) ,
                new(PlaceFields.Kreis, TypeCode.Int64) ,
                new(PlaceFields.Land, TypeCode.Int64) ,
                new(PlaceFields.Staat, TypeCode.Int64) ,
                new(PlaceFields.Staatk, TypeCode.String) { Laenge = 3 },
                new(PlaceFields.PLZ, TypeCode.String) { Laenge = 10 },
                new(PlaceFields.Terr, TypeCode.String) { Laenge = 5 },
                new(PlaceFields.Loc, TypeCode.String) { Laenge = 6 },
                new(PlaceFields.L, TypeCode.String) { Laenge = 10 },
                new(PlaceFields.B, TypeCode.String) { Laenge = 10 },
                new(PlaceFields.Bem, TypeCode.String) ,
                new(PlaceFields.OrtNr, TypeCode.Int64) { xNull = true },
                new(PlaceFields.Zusatz, TypeCode.String) { Laenge = 240 },
                new(PlaceFields.GOV, TypeCode.String) { Laenge = 20 },
                new(PlaceFields.PolName, TypeCode.String) { Laenge = 240 },
                new(PlaceFields.g, TypeCode.Single) },
          Indexes = new stIndex[]{
                new(PlaceIndex.K, new Enum[] { PlaceFields.Kreis } ),
                new(PlaceIndex.L, new Enum[] { PlaceFields.Land } ),
                new(PlaceIndex.O, new Enum[] { } ),
                new(PlaceIndex.Orte, new Enum[] { PlaceFields.Ort } ),
                new(PlaceIndex.OrtNr, new Enum[] { PlaceFields.OrtNr } ){ Unique = true },
                new(PlaceIndex.OT, new Enum[] { PlaceFields.Ortsteil } ),
                new(PlaceIndex.Pol, new Enum[] { PlaceFields.PolName } ),
                new(PlaceIndex.S, new Enum[] { PlaceFields.Staat } )}
                },
        new() { Name = nameof(dbTables.Ortsuch),
          Fields = new stFieldDef[]{
                new("Name", TypeCode.String) { Laenge = 100 },
                new("Nr", TypeCode.Int64) },
          Indexes = new stIndex[]{
                new("OrtNr", new[] { "Nr" } ),
                new("ortsu", new[] { "Name" } ){ Unique = true }}
                },
        new() { Name = nameof(dbTables.Personen),
          Fields = new stFieldDef[]{
                new(PersonFields.AnlDatum, TypeCode.Int64) ,
                new(PersonFields.Sex, TypeCode.String) { Laenge = 1, xNull = true },
                new(PersonFields.EditDat, TypeCode.Int64) ,
                new(PersonFields.Bem1, TypeCode.String) ,
                new(PersonFields.Pruefen, TypeCode.String) { Laenge = 5 },
                new(PersonFields.PersNr, TypeCode.Int64) ,
                new(PersonFields.Konv, TypeCode.String) { Laenge = 240 },
                new(PersonFields.Such1, TypeCode.String) { Laenge = 240 },
                new(PersonFields.Bem2, TypeCode.String) ,
                new(PersonFields.Bem3, TypeCode.String) ,
                new(PersonFields.OFB, TypeCode.String) { Laenge = 1 },
                new(PersonFields.religi, TypeCode.Int64) ,
                new(PersonFields.PUid, TypeCode.String) { Laenge = 40 },
                new(PersonFields.Such2, TypeCode.String) { Laenge = 240 },
                new(PersonFields.Such3, TypeCode.String) { Laenge = 240 },
                new(PersonFields.Such4, TypeCode.String) { Laenge = 240 },
                new(PersonFields.Such5, TypeCode.String) { Laenge = 240 },
                new(PersonFields.Such6, TypeCode.String) { Laenge = 240 }},
          Indexes = new stIndex[]{
                new(PersonIndex.BeaDat,new Enum[] { PersonFields.EditDat } ),
                new(PersonIndex.PerNr, new Enum[] { PersonFields.PersNr }){ Unique = true },
                new(PersonIndex.Puid,  new Enum[] { PersonFields.PUid } ),
                new(PersonIndex.reli,  new Enum[] { PersonFields.religi } ),
                new(PersonIndex.Such1, new Enum[] { PersonFields.Such1, PersonFields.PersNr }) { IgnoreNull = true} ,
                new(PersonIndex.Such2, new Enum[] { PersonFields.Such2, PersonFields.PersNr } ),
                new(PersonIndex.Such3, new Enum[] { PersonFields.Such3, PersonFields.PersNr } ),
                new(PersonIndex.Such4, new Enum[] { PersonFields.Such4, PersonFields.PersNr } ){ IgnoreNull = true},
                new(PersonIndex.Such5, new Enum[] { PersonFields.Such5, PersonFields.PersNr } ),
                new(PersonIndex.Such6, new Enum[] { PersonFields.Such6, PersonFields.PersNr } )}
                },
        new() { Name = nameof(dbTables.Quellen),
          Fields = new stFieldDef[]{
                new("1", TypeCode.Int64) ,
                new("2", TypeCode.String) { Laenge = 240 },
                new("3", TypeCode.String) { Laenge = 240 },
                new("4", TypeCode.String) { Laenge = 240 },
                new("5", TypeCode.String) { Laenge = 240 },
                new("7", TypeCode.String) { Laenge = 240 },
                new("8", TypeCode.String) { Laenge = 240 },
                new("9", TypeCode.String) { Laenge = 240 },
                new("10", TypeCode.String) { Laenge = 240 },
                new("11", TypeCode.String) { Laenge = 240 },
                new("12", TypeCode.String) { Laenge = 240 },
                new("13", TypeCode.String) },
          Indexes = new stIndex[]{
                new("Autor", new[] { "5" } ),
                new("Dopp", new[] {"2", "4" } ),
                new("Nam", new[] { "2" } ),
                new("Nr", new[] { "1" } ),
                new("ZITAT", new[] { "4" } ){ Unique = true }}
                },
        new() { Name = nameof(dbTables.Repo),
          Fields = new stFieldDef[]{
                new("Nr", TypeCode.Int64) ,
                new("Name", TypeCode.String) { Laenge = 240 },
                new("Strasse", TypeCode.String) { Laenge = 240 },
                new("Ort", TypeCode.String) { Laenge = 240 },
                new("PLZ", TypeCode.String) { Laenge = 240 },
                new("Fon", TypeCode.String) { Laenge = 240 },
                new("Mail", TypeCode.String) { Laenge = 240 },
                new("Http", TypeCode.String) { Laenge = 240 },
                new("Bem", TypeCode.String) ,
                new("Suchname", TypeCode.String) { Laenge = 240 }},
          Indexes = new stIndex[]{
                new("Name", new[] { "SuchName" } ),
                new("Nr", new[] { "Nr" } ),
                new("Such", new[] { "SuchName", "Ort" } ){ Unique = true }}
                },
        new() { Name = nameof(dbTables.RepoTab),
          Fields = new stFieldDef[]{
                new("Quelle", TypeCode.Int64) ,
                new("Repo", TypeCode.Int64) },
          Indexes = new stIndex[]{
                new("Dop", new[] { "Quelle", "Repo" } ),
                new("Leer", new[] { "Repo" } ),
                new("Nr", new[] { "Quelle" } )}
                },
        new() { Name = nameof(dbTables.Such),
          Fields = new stFieldDef[]{
                new("Name", TypeCode.String) { Laenge = 50 },
                new("Datum", TypeCode.Int32) ,
                new("Nummer", TypeCode.Int64) ,
                new("iKenn", TypeCode.String) { Laenge = 1 },
                new("Sich", TypeCode.String) { Laenge = 1 }},
          Indexes = new stIndex[]{
                new("Namen", new[] { "" } ),
                new("Nummer", new[] { "" } ){ Unique = true },
                new("Persuch", new[] { "" } )}
                },
        new() { Name = nameof(dbTables.Tab),
          Fields = new stFieldDef[]{
                new(ILinkData.LinkFields.Kennz, TypeCode.Byte) ,
                new(ILinkData.LinkFields.FamNr, TypeCode.Int64) ,
                new(ILinkData.LinkFields.PerNr, TypeCode.Int64) },
          Indexes = new stIndex[]{
                new(LinkIndex.ElSu,    new Enum[] { ILinkData.LinkFields.PerNr, ILinkData.LinkFields.Kennz } ),
                new(LinkIndex.FamNr,   new Enum[] { ILinkData.LinkFields.FamNr  } ),
                new(LinkIndex.FamPruef,new Enum[] { ILinkData.LinkFields.FamNr, ILinkData.LinkFields.PerNr, ILinkData.LinkFields.Kennz } ){ Unique = true },
                new(LinkIndex.FamSu,   new Enum[] { ILinkData.LinkFields.FamNr, ILinkData.LinkFields.Kennz } ),
                new(LinkIndex.FamSu1,  new Enum[] {  } ),
                new(LinkIndex.PAFI,    new Enum[] {  } ),
                new(LinkIndex.Per,     new Enum[] {  } )}
                },
        new() { Name = nameof(dbTables.Tab1),
          Fields = new stFieldDef[]{
                new("1", TypeCode.Single) ,
                new("2", TypeCode.Int64) ,
                new("3", TypeCode.Int64) ,
                new("4", TypeCode.String) { Laenge = 240 },
                new("LfNr", TypeCode.Single) ,
                new("Art", TypeCode.Single) ,
                new("Aus", TypeCode.String) { Laenge = 240 },
                new("ORIG", TypeCode.String) ,
                new("Kom", TypeCode.String) },
          Indexes = new stIndex[]{
                new("Tab", new[] { "" } ),
                new("Tab2", new[] { "" } ),
                new("Tab21", new[] { "" } ),
                new("Tab22", new[] { "" } ),
                new("Tab23", new[] { "" } ),
                new("Verw", new[] { "" } )}
                },
        new() { Name = nameof(dbTables.Tab2),
          Fields = new stFieldDef[]{
                new(WitnessFields.FamNr, TypeCode.Int64) ,
                new(WitnessFields.PerNr, TypeCode.Int64) ,
                new(WitnessFields.Kennz, TypeCode.String) { Laenge = 2 },
                new(WitnessFields.Art, TypeCode.Single) ,
                new(WitnessFields.LfNr, TypeCode.Single) },
          Indexes = new stIndex[]{
                new(WitnessIndex.ElSu,    new Enum[] {  } ),
                new(WitnessIndex.Fampruef,new Enum[] { WitnessFields.FamNr, WitnessFields.PerNr, WitnessFields.Kennz, WitnessFields.Art, WitnessFields.LfNr } ),
                new(WitnessIndex.FamSu,   new Enum[] {  } ),
                new(WitnessIndex.Zeug,    new Enum[] { WitnessFields.FamNr, WitnessFields.PerNr, WitnessFields.Kennz, WitnessFields.Art, WitnessFields.LfNr } ),
                new(WitnessIndex.ZeugSu,  new Enum[] {  } )}
                },
        new() { Name = nameof(dbTables.Texte),
          Fields = new stFieldDef[]{
                new(TexteFields.Txt, TypeCode.String) { Laenge = 240 },
                new(TexteFields.Kennz, TypeCode.String) { Laenge = 1, xNull = true },
                new(TexteFields.TxNr, TypeCode.Int64) ,
                new(TexteFields.Leitname, TypeCode.String) { Laenge = 32 },
                new(TexteFields.Bem, TypeCode.String) { xNull = true }},
          Indexes = new stIndex[]{
                new(TexteIndex.ITexte, new Enum[] { TexteFields.Txt, TexteFields.Kennz } ){ Unique = true },
                new(TexteIndex.KText,  new Enum[] {  } ),
                new(TexteIndex.LTexte, new Enum[] {  } ),
                new(TexteIndex.RTexte, new Enum[] {  } ),
                new(TexteIndex.SSTexte,new Enum[] {  } ),
                new(TexteIndex.STexte, new Enum[] { TexteFields.Kennz, TexteFields.Txt } ),
                new(TexteIndex.TxNr,   new Enum[] { TexteFields.TxNr } ),
                new(TexteIndex.TxNr1,  new Enum[] { TexteFields.TxNr, TexteFields.Txt } ){ Unique = true }}
                },
        new() { Name = nameof(dbTables.WDBL),
          Fields = new stFieldDef[]{
                new(WDBLFields.Nr, TypeCode.Byte) }} };

    }
}
