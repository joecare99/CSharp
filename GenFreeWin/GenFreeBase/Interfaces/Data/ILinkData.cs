using GenFree.Data;

namespace GenFree.Interfaces.Data;
public interface ILinkData: IHasID<(int iFamily, int iPerson, ELinkKennz eKennz)>, IHasPropEnum<ELinkProp>, IHasIRecordset
{
    public enum LinkFields
    {
        /// <summary>
        /// The Kennzeichen, the type of link.
        /// </summary>
        Kennz,
        /// <summary>
        /// The ID of the family/Entity.
        /// </summary>  
        FamNr,
        /// <summary>
        /// The ID of the person.
        /// </summary>
        PerNr
    }

    /// <summary>
    /// Gets the Kennzeichen, the type of the link.
    /// </summary>
    /// <value>The Kennzeichen.</value>
    ELinkKennz eKennz { get; }
    /// <summary>
    /// Gets the family(entity) number.
    /// </summary>
    /// <value>The family(entity) number.</value>
    int iFamNr { get; }
    /// <summary>
    /// Gets the person number.
    /// </summary>
    /// <value>The person number.</value>
    int iPersNr { get; }

    void AppendDB();
    void SetFam(int iFamNr);
    void SetPers(int iPersNr);
}
