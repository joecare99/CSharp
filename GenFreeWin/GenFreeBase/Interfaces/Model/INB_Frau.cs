//using DAO;
namespace GenFree.Interfaces.Model;

public interface INB_Frau :
    IUsesRecordset<int>, // Interface for recordset access
    IUsesID<int> // Interface for ID access
{
    void AddRow(int iNr, int iGen, int iPersNr, int iFamNr, int iKek1, int iKek2);
    bool CReadData(int num6, out (int iGen, int iPers, int iFam)? value);
    bool PersonExists(int iPersNr);
    bool ReadData(int LfdNr, out int persNr, out int famNr, out int gen, out int kek2, out int kek1);
}