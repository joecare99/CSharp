using GenFree.Data;
using GenFree.Interfaces.DB;

namespace GenFree.Interfaces;
public interface ILinkData: IHasID<(int iFamily, int iPerson, ELinkKennz eKennz)>, IHasPropEnum<ELinkProp> 
{
    public enum LinkFields
    {
        Kennz,
        FamNr,
        PerNr
    }

    ELinkKennz eKennz { get; }
    int iFamNr { get; }
    int iKennz { get; }
    int iPersNr { get; }

    void AppendDB();
    void Delete();
    void FillLink(IRecordset dB_LinkTable);
    void SetFam(int iFamNr);
    void SetPers(int iPersNr);
}
