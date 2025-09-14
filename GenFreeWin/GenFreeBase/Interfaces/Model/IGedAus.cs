using GenFree.Data;
using GenFree.Interfaces.Data;

namespace GenFree.Interfaces.Model
{
    public interface IGedAus
    {
        string ADRText { get; set; }
        bool[] Kont3 { get; } //new bool[7];
        int Leer { get; set; }
        string FILENAM { get; set; }
        byte HLT { get; set; }
        bool Kontakt { get; set; }
        int[] Datschu { get; set; }
        EExportPrivacy Options_priv { get; set; }
        bool Internet { get; set; }
        bool[] Hi { get; set; }
        byte Options_Bildja { get; set; }
        bool Options_Mehrvorn { get; set; }
        int Options_Uml { get; set; }
        bool Options_Orts { get; set; }
        short Options_Paten { get; set; }
        short Options_Quellaus { get; set; }
        bool Options_OFB { get; set; }
        byte Options_Schalt1 { get; set; }
        bool Options_Sperraus { get; set; }
        bool Options_Loe { get; set; }
        bool Options_Hausnr { get; set; }
        int Options_Famleer { get; set; }
        bool Options_EigQuelle { get; set; }

        void berufles(int iPersInArb);
        void Bilderaus(string Kenn, int PerfamNr);
        bool CheckFamPersons(int famInArb);
        void ChildSort(int FamInArb);
        void FamPersuch();
        void FamSonstles(int FamInArb);
        void Fertig();
        void Historie(string UbgT);
        void Individuum(IPersonData cPerson);
        void ReadKopf();
        void Namensteil(int PersInArb);
        void OpenTempDB(string sFileName);
        string ortles(int Ortnr);
        int PeekDB(string dateiName);
        void Personenpaten(int PersInArb, bool xGodChildRelSkip);
        bool Person_DatSchutzEvt(int M1_PersInArb, int[] datschu);
        void Quelleaus(byte Kenn, int PerfamNr);
        void Repoausgeb();
        void Sonstles(int PersInArb);
        void Sperrdaterst(int iPersInArb, int iFamInArb);
        void Textaus();
        void TextTeilen(string UbgT4, string Kennung);
        void Wandel();
        void WriteAnDatum(string _Cr, string CHa);
        void WriteDate(string _Cr, int iLvl);
        void WriteFamDat(int FamInArb);
        void WriteGedLine(string Fld, bool xCheck);
        void WriteHeader(string gedAus_FILENAM);
        void Zweitdatum(ref string Datu, string Datsicha, EEventArt eArt, IEventData cEv);
        void WriteEventData(EEventArt eArt, IEventData cEv);
        void Fehlliste(EEventArt eArt, string Dasi,int iPerFamNr, string grund = "");
    }
}
