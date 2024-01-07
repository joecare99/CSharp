//using DAO;
using GenFree.Data;
using GenFree.Interfaces.DB;

namespace GenFree.Interfaces
{
    public interface IPlaceData : IHasPropEnum<EPlaceProp>, IHasID<int>, IHasIRecordset
    {
        int iLand { get; set; }
        int iStaat { get; set; }
        int iKreis { get; set; }
        int iOrt { get; set; }
        int iOrtsteil { get; set; }
        string sStaatk { get; }
        string sPLZ { get; }
        string sTerr { get; }
        string sLoc { get; }
        string sL { get; }
        string sB { get; }
        string sBem { get; set; }
        string sZusatz { get; }
        string sGOV { get; }
        string sPolName { get; }
        int ig { get; }
        string sOrt { get; }
        string sOrtsteil { get; }
        string sKreis { get; }
        string sLand { get; }
        string sStaat { get; }
    }
}