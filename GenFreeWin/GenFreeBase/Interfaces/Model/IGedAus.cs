using GenFree.Data;
using GenFree.Interfaces.Data;

namespace GenFree.Interfaces.Model
{
    public interface IGedAus
    {
        string ADRText { get; set; }
        bool[] Kont3 { get; } //new bool[7];
        int Leer { get; set; }

        void berufles(int iPersInArb);
        bool CheckFamPersons(int famInArb);
        void ChildSort(int FamInArb);
        void FamPersuch();
        void FamSonstles(int FamInArb);
        void Individuum(IPersonData cPerson);
        void Kopf();
        void Personenpaten(int PersInArb, bool xGodChildRelSkip);
        bool Person_DatSchutz(int M1_PersInArb, int[] datschu);
        void Sonstles(int PersInArb);
        void Sperrdaterst(int iPersInArb, int iFamInArb);
        void Wandel();
        void WriteAnDatum(string _Cr, string CHa);
        void WriteDate(string _Cr, int iLvl);
        void WriteFamDat(int FamInArb);
        void WriteGedLine(string Fld, bool xCheck);
        void Zweitdatum(ref string Datu, string Datsicha, EEventArt eArt, IEventData cEv);
    }
}
