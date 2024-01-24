namespace GenFree.Interfaces.Model
{
    public interface IGedAus
    {
        string ADRText { get; set; }
        bool[] Kont3 { get; } //new bool[7];

        void berufles(int iPersInArb);
        void Personenpaten(int PersInArb, bool xGodChildRelSkip);
        void WriteAnDatum(string _Cr, string CHa);
        void WriteDate(string _Cr, int iLvl);
        void WriteFamDat(int FamInArb);
        void WriteGedLine(string Fld, bool xCheck);
    }
}
