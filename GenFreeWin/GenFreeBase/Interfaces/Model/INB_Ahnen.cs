namespace GenFree.Interfaces.Model;

public interface INB_Ahnen :
    IUsesRecordset<int>, // Interface for recordset access
    IUsesID<int> // Interface for ID access
{
    void AddRow(int iPersNr, int iGen, int iWtr, int iKek1, int iKek2, int iFamNr);
    void EditRaw(int iGene, int iAhn1, int iAhn2, int famInArb, string surName);
    bool PersonExists(int iPersNr);
    void SetWeiterRaw();
    void Commit(int persInArb, int famInArb, int iGen, int iAhn1, int iAhn2, string name);
}