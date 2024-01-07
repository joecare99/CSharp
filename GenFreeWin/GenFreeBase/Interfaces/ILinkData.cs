using GenFree.Data;
using GenFree.Interfaces.DB;

namespace GenFree.Interfaces;
public interface ILinkData: IHasID<(int iFamily, int iPerson, ELinkKennz eKennz)>, IHasPropEnum<ELinkProp>, IHasIRecordset
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
    void SetFam(int iFamNr);
    void SetPers(int iPersNr);
}
